using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Web;
using System.Web.Services;
using System.Data.SqlClient;
using Sport.Core;
using System.Collections.Generic;
using ISF.DataLayer;

namespace SportServices
{
	/// <summary>
	/// Summary description for SessionService.
	/// </summary>
	public class SessionService : System.Web.Services.WebService
	{
		#region Structs
		public struct UserData
		{
			public int Id;
			public string Username;
			public string UserFullName;
			public int UserType;
			public int UserPermissions;
			public int UserRegion;
			public int UserSchool;
		}

		public struct SeasonData
		{
			public int Season;
			public string Name;
		}
		#endregion


		public static readonly string SessionKey_UserID = "user_id";
		public static readonly string SessionKey_Season = "season";

		public SessionService()
		{
			//CODEGEN: This call is required by the ASP.NET Web Services Designer
			InitializeComponent();
		}

		#region Component Designer generated code

		//Required by the Web Services Designer 
		private IContainer components = null;

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#endregion

		/// <summary>
		/// try to perform login for the given user.
		/// if successful, store the user information in the session.
		/// returns UserData struct with the user information - empty if login failed.
		/// </summary>
		[WebMethod(EnableSession = true)]
		public UserData Login(string username, string password, int season)
		{
			UserData result;
			string strUserName = "";
			Action action = Action.Login_Client;

			if ((username != null) && (username.Length > 0))
			{
				string[] arrTemp = username.Split(new char[] { '#' });
				strUserName = arrTemp[0];
				if (arrTemp.Length > 1)
				{
					string strVersion = arrTemp[1].ToLower();
					if (strVersion == "website")
					{
						action = Action.Login_Website;
						if (arrTemp.Length > 2)
							strVersion = arrTemp[2];
					}
					if (strVersion == "field")
					{
						action = Action.Login_Field;
						if (arrTemp.Length > 2)
							strVersion = arrTemp[2];
					}
					Session["version"] = Common.ToDoubleDef(strVersion, 0);
				}
			}

			//verify given user against database:
			result = _VerifyUser(strUserName, password);

			//check if login successfull:
			if (Sport.Common.Tools.CStrDef(result.Username, "").Length > 0)
			{
				//put in session:
				Session[SessionKey_UserID] = result.Id;
				Session[SessionKey_Season] = season;

				//add user action:
				Common.AddUserAction(result.Id, action, "user login (" + username + ")",
					Common.ToDoubleDef(Session["version"], 0));
			}

			return result;
		}

		[WebMethod(EnableSession = true)]
		public int AddUserAction(Action action, string description, double version)
		{
			/* if (Session[SessionKey_UserID] == null)
				throw new Exception("can't add action: session expired"); */
			int result = Common.AddUserAction(
				Common.ToIntDef(Session[SessionKey_UserID], -1),
				action, description, version);
			return result;
		}

		[WebMethod(EnableSession = true)]
		public int AddUserAction_2(int userid,
			Action action, string description, double version)
		{
			int result = Common.AddUserAction(userid,
				action, description + " (*)", version);
			return result;
		}

		/// <summary>
		/// keeps the session alive. returns whether user is logged in or not
		/// </summary>
		[WebMethod(EnableSession = true)]
		public bool KeepAlive()
		{
			return (Session[SessionKey_UserID] != null);
		}

		/// <summary>
		/// verifies given user if authorized to use the program. if authorized, 
		/// this function will return the user details in the struct.
		/// otherwise, empty struct would be returned.
		/// </summary>
		/// <param name="username">unique username of the user.</param>
		/// <param name="password">password to be checked against database.</param>
		/// <returns>data of the verified user if authorized, empty if invalid.</returns>
		[WebMethod(EnableSession = true)]
		public UserData VerifyUser(string username, string password)
		{
			return _VerifyUser(username, password);
		}

		/// <summary>
		/// return all data of user with given username.
		/// </summary>
		[WebMethod]
		public UserData GetUserData(string username)
		{
			UserData result = new UserData();

			string fields = GetUserFields();
			string strSQL = "Select " + fields + " From USERS " + 
				"Where USER_LOGIN=@1 AND DATE_DELETED IS NULL";
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL,
				new SimpleParameter("@1", username));
			if (table.Rows.Count > 0)
			{
				SimpleRow row = table.Rows[0];
				FillUserData(out result, row);
			}
			
			return result;
		} //end function GetUserData

		/// <summary>
		/// get password for given user. asker must also send valid login and password. 
		/// </summary>
		/// <param name="username">unique user login string of the user whose password we want.</param>
		/// <param name="askerLogin">login string of user who send the request.</param>
		/// <param name="askerPassword">password of the user who send the request.</param>
		[WebMethod]
		public string GetUserPassword(string username, string askerLogin, string askerPassword)
		{
			//first verify request:
			UserData user = _VerifyUser(askerLogin, askerPassword);
			if (user.Username == null)
			{
				throw new Exception("Failed to retrieve password for " + username + ":\nverification failed.");
			}

			//get password from the database:
			string strSQL = "Select USER_PASSWORD From USERS " + 
				"Where USER_LOGIN=@1 AND DATE_DELETED IS NULL";
			return DB.Instance.ExecuteScalar(strSQL, "", 
				new SimpleParameter("@1", username)).ToString();
		} //end function GetUserPassword

		/// <summary>
		/// change the password of given user. old password must match current password
		/// in order to verify the request.
		/// </summary>
		/// <param name="username">unique user login string of the user.</param>
		/// <param name="oldPassword">current password of the user.</param>
		/// <param name="newPassword">new password to apply to this user.</param>
		[WebMethod(EnableSession = true)]
		public void ChangeUserPassword(string username, string oldPassword, string newPassword)
		{
			//logged in?
			if (Session[SessionKey_UserID] == null)
				throw new Exception("can't change password: session expired");
			
			//update password:
			string strSQL;
			strSQL = "Update USERS Set USER_PASSWORD=@2 Where USER_LOGIN=@1 AND DATE_DELETED IS NULL";
			DB.Instance.Execute(strSQL,
				new SimpleParameter("@1", username),
				new SimpleParameter("@2", newPassword));
		} //end function ChangeUserPassword

		/// <summary>
		/// get latest version
		/// </summary>
		[WebMethod(EnableSession = true)]
		public VersionInfo GetLatestVersion()
		{
			VersionInfo result = new VersionInfo();
			result.VersionNumber = Sport.Core.Data.CurrentVersion;
			result.VersionName = result.VersionNumber.ToString().PadRight(3, '0');
			result.VersionDetails = "details about what's new can be written here.";
			return result;
		}

		/// <summary>
		/// get latest field program version
		/// </summary>
		[WebMethod(EnableSession = true)]
		public VersionInfo GetLatestFieldVersion()
		{
			VersionInfo result = new VersionInfo();
			result.VersionNumber = Sport.Core.Data.Field_CurrentVersion;
			string strVersion = result.VersionNumber.ToString();
			if (strVersion.IndexOf(".") < 0)
				strVersion += ".0";
			result.VersionName = strVersion;
			result.VersionDetails = "details about what's new can be written here.";
			return result;
		}

		[WebMethod]
		public byte[] GetStudentPicture(string Id_Number)
		{
			return Common.BuildStudentPicture(Server, Id_Number);
		}

		[WebMethod]
		public int GetTeamCharge(int teamID)
		{
			//build select statement:
			string strSQL = "SELECT CHARGE_ID FROM TEAMS WHERE TEAM_ID=@team";

			//read result:
			return (int)DB.Instance.ExecuteScalar(strSQL, -1,
				new SimpleParameter("@team", teamID));
		}

		[WebMethod]
		public int[] GetSchoolTeams(int schoolID)
		{
			//build select statement:
			string strSQL = "SELECT DISTINCT t.TEAM_ID FROM TEAMS t, CHAMPIONSHIPS c " +
							"WHERE t.CHAMPIONSHIP_ID=c.CHAMPIONSHIP_ID AND " +
							"c.SEASON=(SELECT MAX(SEASON) FROM SEASONS) AND t.SCHOOL_ID=@school " +
							"AND c.DATE_DELETED IS NULL AND t.DATE_DELETED IS NULL";

			//read result:
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL,
				new SimpleParameter("@school", schoolID));
			List<int> result = new List<int>();
			table.Rows.ForEach(row =>
			{
				result.Add((int)row["TEAM_ID"]);
			});
			
			//return the result:
			return result.ToArray();
		}

		[WebMethod]
		public Sport.Data.TeamData[] GetSchoolTeamsBySeason(int school, int season)
		{
			//build select statement:
			string strSQL = "Select Distinct t.TEAM_ID, t.REGISTRATION_DATE, t.[STATUS], c.CHAMPIONSHIP_ID, c.CHAMPIONSHIP_NAME, " +
				"	cc.CHAMPIONSHIP_CATEGORY_ID, cc.CATEGORY, s.SPORT_ID, s.SPORT_NAME " + 
				"From TEAMS t Inner Join CHAMPIONSHIP_CATEGORIES cc On t.CHAMPIONSHIP_CATEGORY_ID=cc.CHAMPIONSHIP_CATEGORY_ID " + 
				"	Inner Join CHAMPIONSHIPS c On cc.CHAMPIONSHIP_ID=c.CHAMPIONSHIP_ID " + 
				"	Inner Join SPORTS s On c.SPORT_ID=s.SPORT_ID " +
				"Where c.SEASON=@season AND t.SCHOOL_ID=@school And c.DATE_DELETED Is Null " + 
				"	And t.DATE_DELETED Is Null And cc.DATE_DELETED Is Null";
			
			//read result:
			Sport.Types.CategoryTypeLookup categoryLookup = new Sport.Types.CategoryTypeLookup();
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL, new SimpleParameter("@season", season), 
				new SimpleParameter("@school", school));
			List<Sport.Data.TeamData> teams = new List<Sport.Data.TeamData>();
			table.Rows.ForEach(row =>
			{
				teams.Add(new Sport.Data.TeamData
				{
					TeamId = (int)row["TEAM_ID"],
					RegistrationDate = (DateTime)row["REGISTRATION_DATE"],
					Status = (int)row["STATUS"],
					Championship = new Sport.Common.SimpleData
					{
						ID = (int)row["CHAMPIONSHIP_ID"],
						Name = row["CHAMPIONSHIP_NAME"] + ""
					},
					Category = new Sport.Common.SimpleData
					{
						ID = (int)row["CHAMPIONSHIP_CATEGORY_ID"],
						Name = categoryLookup.Lookup((int)row["CATEGORY"])
					},
					Sport = new Sport.Common.SimpleData
					{
						ID = (int)row["SPORT_ID"],
						Name = row["SPORT_NAME"] + ""
					}
				});
			});

			//return the result:
			return teams.ToArray();
		}

		private string GetUserFields()
		{
			string result = "";
			EntityDefinition ed = EntityDefinition.GetEntityDefinition(Sport.Entities.User.TypeName);
			string[] arrFields = ed.Fields;
			for (int i = 0; i < arrFields.Length; i++)
			{
				result += arrFields[i];
				if (i < (arrFields.Length - 1))
					result += ",";
			}
			return result;
		}

		/// <summary>
		/// internal function to verify user. details in the public version.
		/// </summary>
		private UserData _VerifyUser(string username, string password)
		{
			UserData result = new UserData();
			if (password == null || password.Length == 0)
				return result;

			//check if user with given username and password exist in database:
			string fields = GetUserFields();
			string strSQL = "Select " + fields + " From USERS " + 
				"Where USER_LOGIN=@1 AND DATE_DELETED IS NULL";
			strSQL += " AND ";
			if (password.Length == 0)
				strSQL += "((USER_PASSWORD IS NULL) OR (USER_PASSWORD=@2))";
			else
				strSQL += "USER_PASSWORD=@2";
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL,
				new SimpleParameter("@1", username),
				new SimpleParameter("@2", password));
			if (table.Rows.Count > 0)
			{
				SimpleRow row = table.Rows[0];
				FillUserData(out result, row);
			}
			
			return result;
		} //end function _VerifyUser

		private void FillUserData(out UserData user, SimpleRow dataRow)
		{
			user.Id = dataRow.GetIntOrDefault((int)Sport.Entities.User.Fields.Id, -1);
			user.UserFullName = dataRow[(int)Sport.Entities.User.Fields.FirstName].ToString();
			user.UserFullName += " " + dataRow[(int)Sport.Entities.User.Fields.LastName].ToString();
			user.Username = dataRow[(int)Sport.Entities.User.Fields.Login].ToString();
			user.UserPermissions = (int)dataRow[(int)Sport.Entities.User.Fields.Permissions];
			user.UserRegion = dataRow.GetIntOrDefault((int)Sport.Entities.User.Fields.Region, -1);
			user.UserSchool = dataRow.GetIntOrDefault((int)Sport.Entities.User.Fields.School, -1);
			user.UserType = dataRow.GetIntOrDefault((int)Sport.Entities.User.Fields.UserType, 1);
		}
	} //end class SessionService
}

