using System;
using System.Data.SqlClient;

namespace SportServices
{
	public class ConnectionLocker
	{
		private SqlConnection connection;
		public SqlConnection Connection
		{
			get { return connection; }
		}
		
		private string _source="";
		public string Source
		{
			get { return _source; }
			set { _source = value; }
		}
		
		private DateTime _lockTime=DateTime.MinValue;
		public DateTime LockTime { get {return _lockTime;} }
		
		private bool _locked;
		public bool Locked
		{
			get { return _locked; }
			set
			{
				_locked = value;
				_lockTime = DateTime.Now;
			}
		}

		public void Unlock()
		{
			ConnectionPool.Unlock(this);
			this.Source = "";
		}

		public ConnectionLocker()
		{
			connection = new SqlConnection(Common.ConnectionString());
			connection.Open();
			_locked = false;
		}
	}

	public class ConnectionPool
	{
		private static readonly int InitialConnections = 3;
		private static readonly int MaxConnections = 20;

		private static System.Collections.ArrayList connections;
		static ConnectionPool()
		{
			connections = new System.Collections.ArrayList();

			for (int n = 0; n < InitialConnections; n++)
				connections.Add(new ConnectionLocker());
		}

		public static ConnectionLocker Lock(string source)
		{
			//Sport.Core.LogFiles.AppendToLogFile(
			//	System.Web.HttpContext.Current.Server.MapPath("ConnectionLog.txt"), 
			//	"hello");
			lock (typeof(ConnectionPool))
			{
				foreach (ConnectionLocker locker in connections)
				{
					if (locker.Locked)
					{
						//Sport.Core.LogFiles.AppendToLogFile(
						//	System.Web.HttpContext.Current.Server.MapPath("ConnectionLog.txt"), 
						//	"found locked connection. source: "+locker.Source+", locked at: "+locker.LockTime.ToString("dd/MM/yyyy HH:mm:ss")+" (locked for "+(DateTime.Now-locker.LockTime).TotalMinutes+" minutes)");
						if (locker.LockTime.Year > 1900)
						{
							if ((DateTime.Now-locker.LockTime).TotalMinutes >= 10)
							{
								Sport.Core.LogFiles.AppendToLogFile(
									System.Web.HttpContext.Current.Server.MapPath("ConnectionLog.txt"), 
									"found idle connection ("+locker.Source+", idle since: "+locker.LockTime.ToString("dd/MM/yyyy HH:mm:ss")+") closing connection and unlocking");
								try
								{
									locker.Connection.Close();
									locker.Connection.Open();
								}
								catch
								{}
								locker.Locked = false;
							}
						}
					}
					
					if (!locker.Locked)
					{
						locker.Locked = true;
						locker.Source = source;
						return locker;
					}
				}
				
				if (connections.Count < MaxConnections)
				{
					ConnectionLocker newLocker = new ConnectionLocker();
					newLocker.Locked = true;
					newLocker.Source = source;
					connections.Add(newLocker);
					return newLocker;
				}
			}
			
			string strMessage="Exceeded connection pool maximum connections. Max: "+
				MaxConnections+", connections: "+connections.Count+", sources: ";
			strMessage += GetActiveConnections(", ");
			
			Sport.Core.LogFiles.AppendToLogFile(
				System.Web.HttpContext.Current.Server.MapPath("ConnectionLog.txt"), 
				strMessage);
			
			throw new Exception("Failed to lock a connection to the database (connections: "+connections.Count+")");
		}
		
		public static void Unlock(ConnectionLocker locker)
		{
			lock (typeof(ConnectionPool))
			{
				locker.Locked = false;
			}
		}
		
		public static string GetActiveConnections(string seperator)
		{
			string result="";
			for (int i=0; i<connections.Count; i++)
			{
				ConnectionLocker curLocker=(ConnectionLocker) connections[i];
				if (!curLocker.Locked)
					continue;
				result += curLocker.Source+" ("+
					curLocker.LockTime.ToString("dd/MM/yyyy HH:mm:ss")+")"+seperator;
			}
			if (result.Length > seperator.Length)
				result = result.Substring(0, result.Length-seperator.Length);
			return result;
		}
	}
}