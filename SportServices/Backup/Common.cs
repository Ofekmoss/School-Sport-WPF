using System;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using Sport.Common;
using Sport.Core;
using System.Configuration;
using ISF.DataLayer;
using System.Collections.Generic;

namespace SportServices
{
	public struct VersionInfo
	{
		public double VersionNumber;
		public string VersionName;
		public string VersionDetails;
	}

	public enum Action
	{
		Login_Client = 1,
		Login_Website,
		Login_Field,
		TableView_Opened,
		Update_Downloaded,
		Register_Page_Load,
		Field_Export,
		Field_Import
	}

	/// <summary>
	/// Summary description for Common.
	/// </summary>
	public class Common
	{
		public static readonly string INI_FILE_NAME = "SportServices.ini";

		private static string connectionString = "";

		/// <summary>
		/// the one and only connection string.
		/// </summary>
		public static string ConnectionString()
		{
			if (connectionString.Length == 0)
				connectionString = ConfigurationManager.AppSettings["ConnectionString"] + "";
			return connectionString;
		}

		public static void ApplyNullValues(ref object[] values)
		{
			if (values != null)
			{
				for (int i = 0; i < values.Length; i++)
				{
					object curValue = values[i];
					if (curValue != null)
					{
						if ((curValue is int && ((int)curValue == -1)) || (curValue is long && ((long)curValue == -1)) || (curValue is double && ((double)curValue == -1)))
							values[i] = null;
						if (curValue is DateTime && ((DateTime)curValue).Year < 1900)
							values[i] = null;
					}
				}
			}
		}

		public static string BuildTeamName(string schoolName, string cityName, int teamIndex)
		{
			string teamFinalName = schoolName.Trim();
			if (cityName.Length > 0)
			{
				if (cityName.IndexOf("תל אביב") >= 0)
					cityName = "תל אביב";
				string strInitials = Tools.MakeInitials(cityName);
				if (cityName == "ראשון לציון")
					cityName = "ראשל\"צ";
				if (!Tools.FindCityInSchool(teamFinalName, cityName) && teamFinalName.IndexOf(strInitials) < 0)
					teamFinalName += " " + cityName;
			}
			if (teamIndex > 0)
			{
				teamFinalName += " " + Tools.ToHebLetter(teamIndex);
				if (teamIndex.ToString().Length < 2)
					teamFinalName += "'";
			}
			return teamFinalName;
		}

		public static byte[] BuildStudentPicture(System.Web.HttpServerUtility server, string Id_Number)
		{
			if (string.IsNullOrEmpty(Id_Number))
				return null;

			while (Id_Number.Length > 1 && Id_Number[0] == '0')
				Id_Number = Id_Number.Substring(1);

			List<string> arrExtensions = (new string[] { "jpg", "gif", "bmp", "png", "jpeg" }).ToList();
			List<string> possibleFileNames = new List<string>();
			while (true)
			{
				arrExtensions.ForEach(ext => possibleFileNames.Add("Pictures/st_" + Id_Number + "." + ext));
				if (Id_Number.Length >= 12)
					break;
				Id_Number = "0" + Id_Number;
			}

			//find path:
			string strPath = "";
			foreach (string possibleName in possibleFileNames)
			{
				strPath = server.MapPath(possibleName);
				if (System.IO.File.Exists(strPath))
					break;
				else
					strPath = "";
			}

			//verify that exist:
			if (strPath.Length == 0)
				return null;

			//read and return array of binary data:
			System.IO.FileStream objReader = System.IO.File.OpenRead(strPath);
			byte[] binData = new byte[(int)objReader.Length];
			objReader.Read(binData, 0, (int)objReader.Length);
			objReader.Close();
			objReader.Dispose();
			return binData;
		}

		public static decimal ToDecimal(System.Array timestamp)
		{
			decimal d = 0;
			for (int n = 0; n < timestamp.Length; n++)
			{
				d *= 256;
				d += (byte)timestamp.GetValue(n);
			}

			return d;
		}

		public static System.Array TimestampToArray(decimal timestamp)
		{
			byte[] arr = new byte[8];
			decimal temp;
			for (int n = 0; n < 8 && timestamp > 0; n++)
			{
				temp = Decimal.Floor(timestamp / 256);
				arr[7 - n] = (byte)(timestamp - (temp * 256));
				timestamp = temp;
			}

			return arr;
		}

		/// <summary>
		/// return the ToString() of given object if possible, or the given default 
		/// string if the given object is null.
		/// </summary>
		public static string ToStringDef(object obj, string def)
		{
			if (obj == null)
				return def;
			if (obj is System.DBNull)
				return def;
			return obj.ToString();
		}

		/// <summary>
		/// return the numeric value of given object if possible, or the given default 
		/// integer if the given object is null or can't be converted to integer.
		/// </summary>
		public static int ToIntDef(object obj, int def)
		{
			if (obj == null)
				return def;
			if (obj is System.DBNull)
				return def;
			try
			{
				//return System.Convert.ToInt32(obj);
				return Int32.Parse(obj.ToString());
			}
			catch
			{
				return def;
			}
		}

		public static long ToLongDef(object obj, long def)
		{
			if (obj == null)
				return def;
			if (obj is System.DBNull)
				return def;
			try
			{
				//return System.Convert.ToInt32(obj);
				return Int64.Parse(obj.ToString());
			}
			catch
			{
				return def;
			}
		}

		public static double ToDoubleDef(object obj, double def)
		{
			if (obj == null)
				return def;
			if (obj is System.DBNull)
				return def;
			try
			{
				//return System.Convert.ToDouble(obj);
				return Double.Parse(obj.ToString());
			}
			catch
			{
				return def;
			}
		}

		public static DateTime ToDateTimeDef(object obj, DateTime def)
		{
			if (obj == null)
				return def;
			if (obj is System.DBNull)
				return def;
			try
			{
				return System.Convert.ToDateTime(obj);
			}
			catch
			{
				return def;
			}
		}

		public static string GetSql(SqlCommand command)
		{
			if (command == null)
				return "---no command---";
			string result = command.CommandText;
			if (command.Parameters != null)
			{
				for (int i = 0; i < command.Parameters.Count; i++)
				{
					System.Data.SqlClient.SqlParameter param = command.Parameters[i];
					result = result.Replace(param.ParameterName,
						Common.ToStringDef(param.Value, "N/A"));
				}
			}
			return result;
		}

		public static bool IsLockedEntity(string entityName)
		{
			return (entityName == Sport.Entities.GameBoard.TypeName) ||
				(entityName == Sport.Entities.PhasePattern.TypeName);
		}

		public static bool IsSuperUser(int userID)
		{
			return Sport.Core.PermissionsManager.IsSuperUser(userID);
		}

		public static int AddUserAction(int userid,
			Action action, string description, double version)
		{
			return 0;
			/*
			string strSQL="INSERT INTO USER_ACTIONS (ACTION_TYPE, USER_ID, ";
			strSQL += "ACTION_DATE, DESCRIPTION, VERSION) VALUES ";
			strSQL += "(@1, @2, @3, @4, @5)";
			SqlCommand command=new SqlCommand(strSQL, locker.Connection);
			command.Parameters.Add("@1", (int) action);
			command.Parameters.Add("@2", userid);
			command.Parameters.Add("@3", DateTime.Now);
			if (description == null)
				command.Parameters.Add("@4", System.DBNull.Value);
			else
				command.Parameters.Add("@4", description);
			command.Parameters.Add("@5", version);
			int result=-1;
			try
			{
				result=Common.TryExecuteNonQuery(command, locker);
			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.WriteLine("failed to add action: "+e.Message);
			}
			try
			{
				locker.Unlock();
			}
			catch
			{}
			return result;
			*/
		}

		/// <summary>
		/// returns value from the INI file if exists, empty string otherwise.
		/// </summary>
		public static string ReadIniValue(string key,
			System.Web.HttpServerUtility Server)
		{
			IniFile iniFile = new IniFile(Server.MapPath(INI_FILE_NAME));
			string result = iniFile.ReadValue("AppSettings", key);
			return (result == null) ? "" : result;
		}

		/// <summary>
		/// write the value in the INI file. returns the previous value.
		/// </summary>
		public static string WriteIniValue(string key, string value,
			System.Web.HttpServerUtility Server)
		{
			IniFile iniFile = new IniFile(Server.MapPath(INI_FILE_NAME));
			string result = iniFile.ReadValue("AppSettings", key);
			iniFile.WriteValue("AppSettings", key, value);
			return result;
		}

		public static int GetUserPermissions(int userID)
		{
			string strSQL = "SELECT USER_PERMISSIONS FROM USERS WHERE USER_ID=@1";
			return (int)DB.Instance.ExecuteScalar(strSQL, 0,
				new SimpleParameter("@1", userID));
		}

		public static bool IsAuthorized(string username,
			string password, bool blnFullAccess)
		{
			//build sql statement:
			string strSQL = "SELECT USER_PERMISSIONS, USER_TYPE " +
				"FROM USERS WHERE USER_LOGIN=@login AND USER_PASSWORD=@password " +
				"AND DATE_DELETED IS NULL";

			//got anything?
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL,
				new SimpleParameter("@login", username),
				new SimpleParameter("@password", password));
			bool result = false;
			if (table.Rows.Count > 0)
			{
				SimpleRow row = table.Rows[0];

				//need full access?
				if (blnFullAccess)
				{
					//get permissions and user type:
					int permissions = (int)row["USER_PERMISSIONS"];
					Sport.Types.UserType type = Sport.Types.UserType.External;
					int dbType = (int)row["USER_TYPE"];
					try
					{
						type = (Sport.Types.UserType)dbType;
					}
					catch { }
					result = ((permissions > 0) && (type == Sport.Types.UserType.Internal));
				}
				else
				{
					//enough that user exists
					result = true;
				}
			}
			
			//done.
			return result;
		} //end function IsAuthorized

		public static void Debug(string s)
		{
			Sport.Core.LogFiles.AppendToLogFile(
				System.Web.HttpContext.Current.Server.MapPath("DebugLog.txt"), s);
		}
	}
}
