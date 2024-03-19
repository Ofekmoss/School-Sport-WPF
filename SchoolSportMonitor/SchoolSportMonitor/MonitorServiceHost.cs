using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceProcess;
using Mir.Common;
using System.DirectoryServices;
using System.Configuration;
using System.Net;
using System.Diagnostics;
using System.Timers;

namespace SchoolSportMonitor
{
	public class MonitorServiceHost : ServiceBase
	{
		//"C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe" "D:\SVN\Branches\SchoolSport2010\SchoolSportMonitor\SchoolSportMonitor\bin\Debug\SchoolSportMonitor.exe"

		private object downloadLocker = new object();
		private Timer checkAliveTimer = null;
		private DateTime lastRecycle = DateTime.MinValue;

		#region fields
		public static readonly string Service_Name = "SchoolSportMonitor (M.I.R)";
		public static readonly string Service_Description = "Monitors the activity of SchoolSport website";
		public static readonly string logSection = "SchoolSportMonitor";

		private string pageToMonitor = "";
		#endregion

		public static void RunService(ServiceBase service)
		{
			ServiceBase.Run(service);
		}

		public MonitorServiceHost()
		{
			// Name the Windows Service
			this.ServiceName = Service_Name;
		}

		public void DoStart()
		{
			Logger.Instance.WriteLog(LogType.Debug, logSection, "Monitor Service Host started");

			pageToMonitor = ConfigurationManager.AppSettings["PageToMonitor"] + "";
			if (pageToMonitor.Length == 0)
			{
				Logger.Instance.WriteLog(LogType.Error, logSection, "No page to monitor");
				return;
			}

			string rawInterval = ConfigurationManager.AppSettings["PollIntervalMinutes"] + "";
			if (rawInterval.Length == 0)
			{
				Logger.Instance.WriteLog(LogType.Error, logSection, "Missing configuration setting PollIntervalMinutes");
				return;
			}

			int pollInterval;
			if (!Int32.TryParse(rawInterval, out pollInterval))
				pollInterval = 0;
			if (pollInterval <= 0)
			{
				Logger.Instance.WriteLog(LogType.Error, logSection, "Invalid poll interval (" + rawInterval + ")");
				return;
			}

			Logger.Instance.WriteLog(LogType.Information, logSection, "Monitoring page '" + pageToMonitor + "', poll interval " + pollInterval + " minutes");

			checkAliveTimer = new Timer(pollInterval * 1000 * 60);
			checkAliveTimer.Elapsed += new ElapsedEventHandler(checkAliveTimer_Elapsed);
			checkAliveTimer.Start();
		}

		void checkAliveTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			try
			{
				if (!IsPageAlive())
					RecyclePools();
			}
			catch (Exception ex)
			{
				Logger.Instance.WriteLog(LogType.Error, logSection, "Error in check alive timer: " + ex.ToString());
			}
		}

		public void DoStop()
		{
			Logger.Instance.WriteLog(LogType.Debug, logSection, "Monitor Service Host stopped");
			if (checkAliveTimer != null)
			{
				checkAliveTimer.Stop();
				checkAliveTimer.Close();
				checkAliveTimer.Dispose();
			}
		}

		// Start the Windows service.
		protected override void OnStart(string[] args)
		{
			//Logger.Instance.WriteLog(LogType.Information, logSection, "OnStart called");
			DoStart();
		}

		//Stop the Windows service.
		protected override void OnStop()
		{
			//Logger.Instance.WriteLog(LogType.Information, logSection, "OnStop called");
			DoStop();
		}

		public bool IsPageAlive()
		{
			bool alive = true;
			try
			{
				WebClient client = new WebClient();
				string contents = "";
				int secondsToLoad = 0;
				int expireTime = 0;
				lock (downloadLocker)
				{
					Stopwatch watch = new Stopwatch();
					watch.Start();
					try
					{
						contents = client.DownloadString(pageToMonitor);
					}
					catch (Exception downloadException)
					{
						Logger.Instance.WriteLog(LogType.Information, logSection, "Page is not alive, error downloading: " + downloadException.Message);
						alive = false;
					}
					watch.Stop();
					secondsToLoad = watch.Elapsed.Seconds;
				}

				if (alive && contents.Length == 0)
				{
					Logger.Instance.WriteLog(LogType.Information, logSection, "Page is not alive, no error but zero length contents");
					alive = false;
				}

				if (alive && HasPageExpired(secondsToLoad, ref expireTime))
				{
					Logger.Instance.WriteLog(LogType.Information, logSection, "Page is not alive, took " + secondsToLoad + " seconds to load while expire time is " + expireTime + " seconds");
					alive = false;
				}

				if (alive)
					Logger.Instance.WriteLog(LogType.Debug, logSection, "Page is alive, took " + secondsToLoad + " seconds to load and returned " + contents.Length + " bytes of data");
			}
			catch (Exception ex)
			{
				Logger.Instance.WriteLog(LogType.Error, logSection, "General error checking if page alive: " + ex.ToString());
			}

			return alive;
		}

		private bool HasPageExpired(int secondsToLoad, ref int expireTimeSeconds)
		{
			string rawValue = ConfigurationManager.AppSettings["ExpireTimeSeconds"] + "";
			if (!Int32.TryParse(rawValue, out expireTimeSeconds))
				expireTimeSeconds = 0;
			if (expireTimeSeconds <= 0)
			{
				Logger.Instance.WriteLog(LogType.Warning, logSection, "No ExpireTimeSeconds setting, skipping expiry check");
				return false;
			}

			return (secondsToLoad >= expireTimeSeconds);
		}

		public void RecyclePools()
		{
			if (lastRecycle.Year > 1900 && (DateTime.Now - lastRecycle).TotalMinutes < 30)
			{
				Logger.Instance.WriteLog(LogType.Warning, logSection, "Pools were recycled less than 30 minutes ago, skipping.");
				return;
			}

			List<string> poolsToRecycle = (ConfigurationManager.AppSettings["PoolsToRecycle"] + "").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
			if (poolsToRecycle.Count == 0)
			{
				Logger.Instance.WriteLog(LogType.Warning, logSection, "No pools to refresh");
				return;
			}

			Logger.Instance.WriteLog(LogType.Debug, logSection, "Recycling pools: " + string.Join(", ", poolsToRecycle));

			try
			{
				DirectoryEntry appPools = new DirectoryEntry("IIS://localhost/w3svc/apppools");
				int poolCount = 0;
				foreach (DirectoryEntry pool in appPools.Children)
				{
					string name = pool.Name;
					if (poolsToRecycle.Exists(p => p.Equals(name, StringComparison.CurrentCultureIgnoreCase)))
					{
						Logger.Instance.WriteLog(LogType.Information, logSection, "Trying to recycle pool '" + name + "'...");
						try
						{
							pool.Invoke("Recycle", null);
							Logger.Instance.WriteLog(LogType.Information, logSection, "Pool '" + name + "' recycled successfully");
						}
						catch (Exception ex)
						{
							Logger.Instance.WriteLog(LogType.Error, logSection, "Error recycling pool '" + name + "': " + ex.Message);
						}
					}
					poolCount++;
				}
				appPools.Dispose();
				appPools = null;

				if (poolCount == 0)
					Logger.Instance.WriteLog(LogType.Warning, logSection, "Found no pools on server");
			}
			catch (Exception ex)
			{
				Logger.Instance.WriteLog(LogType.Error, logSection, "Error recycling pools: " + ex.ToString());
			}

			lastRecycle = DateTime.Now;
		}
	}
}
