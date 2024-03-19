using System;

namespace Sport.Core
{
	public class Session
	{
		#region Parameter Class

		public class Parameter
		{
			private object _value;
			public object Value
			{
				get { return _value; }
				set
				{
					_value = value;
					OnValueChange();
				}
			}

			private string _name;
			public string Name
			{
				get { return _name; }
			}

			public event EventHandler ValueChanged;

			private void OnValueChange()
			{
				if (ValueChanged != null)
					ValueChanged(this, EventArgs.Empty);
			}

			public Parameter(string name)
			{
				_name = name;
				parameters[_name] = this;
			}
		}

		#endregion

		private static System.Collections.Hashtable parameters;

		private static System.Net.CookieContainer cookies;
		private static bool connected;
		private static bool isLogActive;

		public static System.Net.CookieContainer Cookies
		{
			get { return cookies; }
			set { cookies = value; }
		}

		public static bool Connected
		{
			get { return connected; }
			set { connected = value; }
		}

		public static bool IsLogActive
		{
			get { return isLogActive; }
			set { isLogActive = value; }
		}

		#region Parameters
		//region:
		private static Parameter regionParameter;
		public static Parameter RegionParameter
		{
			get { return regionParameter; }
		}
		public static int Region
		{
			get { return (int)regionParameter.Value; }
			set { regionParameter.Value = value; }
		}

		//season:
		private static Parameter seasonParameter;
		public static Parameter SeasonParameter
		{
			get { return seasonParameter; }
		}
		public static int Season
		{
			get { return (int)seasonParameter.Value; }
			set { seasonParameter.Value = value; }
		}

		public static string GetSeasonCache(bool blnReplaceSeason)
		{
			int season = Season;
			if (season <= 0)
			{
				try
				{
					string strLastSeason = Core.Configuration.ReadString(
						"LastSeason", "ID");
					if ((strLastSeason != null) && (strLastSeason.Length > 0))
						season = Int32.Parse(strLastSeason);
				}
				catch
				{ }
			}
			string strApplicationPath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
			string strApplicationFolder = System.IO.Path.GetDirectoryName(strApplicationPath);
			try
			{
				string strFolderName = "Cache";
				string strFolderPath = strApplicationFolder + System.IO.Path.DirectorySeparatorChar + strFolderName;
				if (!System.IO.Directory.Exists(strFolderPath))
					System.IO.Directory.CreateDirectory(strFolderPath);
				strFolderPath += System.IO.Path.DirectorySeparatorChar + "Season" + season;
				if (!System.IO.Directory.Exists(strFolderPath))
					System.IO.Directory.CreateDirectory(strFolderPath);
				if (blnReplaceSeason)
					strFolderPath = strFolderPath.Replace(season.ToString(), "@season");
				return strFolderPath;
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("failed to get season cache: " + ex.Message);
				System.Diagnostics.Debug.WriteLine(ex.StackTrace);
				return strApplicationFolder;
			}
		}

		public static string GetSeasonCache()
		{
			return GetSeasonCache(true);
		}

		//user:
		private static Parameter userParameter;
		public static Parameter UserParameter
		{
			get { return userParameter; }
		}
		public static UserData User
		{
			get { return (UserData)userParameter.Value; }
			set { userParameter.Value = value; }
		}
		#endregion

		public static Parameter Get(string name)
		{
			return (Parameter)parameters[name];
		}

		/// <summary>
		/// common indexer: returns the parameter in given index.
		/// </summary>
		public Parameter this[string name]
		{
			get { return Get(name); }
		}

		static Session()
		{
			parameters = new System.Collections.Hashtable();

			cookies = new System.Net.CookieContainer();
			connected = true;
			isLogActive = false;

			regionParameter = new Parameter("region");
			regionParameter.Value = -1;

			seasonParameter = new Parameter("season");
			seasonParameter.Value = -1;

			userParameter = new Parameter("user");
			userParameter.Value = UserData.Empty;
		}
	}
}
