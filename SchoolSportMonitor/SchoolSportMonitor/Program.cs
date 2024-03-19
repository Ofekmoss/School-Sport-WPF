using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mir.Common;

namespace SchoolSportMonitor
{
	class Program
	{
		private static readonly string logSection = "SchoolSportMonitor";
		static void Main(string[] args)
		{
			//logger:
			try
			{
				InitializeLogger();
			}
			catch (Exception ex)
			{
				string msg = "Failed to initialize logger: " + ex.ToString();
				System.Diagnostics.Debug.WriteLine(msg);
				Console.WriteLine(msg);
			}

			//start hosting the main service.
			MonitorServiceHost service = new MonitorServiceHost();
			if (Environment.UserInteractive)
			{
				Logger.Instance.WriteLog(LogType.Information, logSection, "Console mode, manually calling OnStart...");
				service.DoStart();

				Console.Write("Press any key to exit");
				Console.WriteLine();
				Console.Read();

				Console.WriteLine("Closing service...");
				Logger.Instance.WriteLog(LogType.Information, logSection, "Stopped. Console mode, manually calling OnStop...");
				service.DoStop();
				Logger.Instance.WriteLog(LogType.Information, logSection, "Goodbye.");
			}
			else
			{
				Logger.Instance.WriteLog(LogType.Information, logSection, "Service mode, running");
				MonitorServiceHost.RunService(service);
			}
		}

		private static void InitializeLogger()
		{
			TextFileLogWriter logWriter = new TextFileLogWriter("SchoolSportMonitor.log", true, true);
			Console.WriteLine("Text logger created successfully, file path: '{0}', max size: {1} (bytes), daily? {2}", logWriter.FilePath, logWriter.MaxSizeBytes, logWriter.DailyLog);
			logWriter.WriteToLog(LogType.Information, "Initial log test - success");
			string sError = logWriter.GetLastError();
			if (!string.IsNullOrEmpty(sError))
				Console.WriteLine("Error writing to log: " + sError);

			Logger.Instance.AddLogWriter(logWriter);
			Logger.Instance.AddLogWriter(new ConsoleLogWriter());
			//Logger.Instance.AddLogWriter(new WindowsEventLogLogWriter());
		}
	}
}
