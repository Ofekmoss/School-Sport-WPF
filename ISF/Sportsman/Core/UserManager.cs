using System;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using Sport.Entities;
using Sportsman.Forms;
using Sport.Common;
using Sport.Core;

namespace Sportsman.Core
{
	/// <summary>
	/// UserManager: general class to handle the user who is using the program.
	/// </summary>
	public class UserManager
	{
		private static UserData _currentUser = null;
		private static SessionServices.SessionService _service = null;

		public static UserData CurrentUser
		{
			get { return _currentUser; }
		}

		/// <summary>
		/// show login dialog, let user log in to the system.
		/// </summary>
		/// <returns>true if login completed, false otherwise.</returns>
		public static bool PerformLogin(string defaultUserName, string defaultPassword)
		{
			bool bLogin = false;
			try
			{
				UserLogin loginForm = new UserLogin();

				if ((_service == null) && (Sport.Core.Session.Connected))
				{
					_service = new SessionServices.SessionService();
					_service.CookieContainer = Sport.Core.Session.Cookies;
				}

				//put default values:
				if ((defaultUserName != null) && (defaultUserName.Length > 0))
				{
					loginForm.Username = defaultUserName;
					loginForm.AutomaticLogin = true;
				}
				if ((defaultPassword != null) && (defaultPassword.Length > 0))
					loginForm.Password = defaultPassword;

				//show login dialog until either the user log in successfully or cancels.
				int failureCount = 0;
				AdvancedServices.AdvancedService advancedService =
					new AdvancedServices.AdvancedService();
				while (!bLogin)
				{
					//show dialog window:
					loginForm.ShouldFocus = true;
					DialogResult dr = loginForm.ShowDialog();

					//check if user canceled the log in process:
					if (dr == DialogResult.Cancel)
						return false;

					//user did not cancel, check user details.
					//verify user details against database data:
					string username = loginForm.Username;
					string password = Crypto.Encode(loginForm.Password);
					int season = loginForm.Season;
					//MessageBox.Show(season.ToString());
					username += "#" + Sport.Core.Data.CurrentVersion.ToString();

					//trigger login method of the web service:
					SessionServices.UserData user = null;
					if (Sport.Core.Session.Connected)
					{
						user = _service.Login(username, password, season);
					}
					else
					{
						user = new SessionServices.UserData();
						string strCorrectPassword = Sport.Common.Tools.CStrDef(
							Sport.Core.Configuration.ReadString("LastUser", "Password"), "");
						string strLastUsername =
							Sport.Core.Configuration.ReadString("LastUser", "Username");
						if ((strCorrectPassword.Length > 0) &&
							(password == strCorrectPassword) &&
							(loginForm.Username.ToLower() == strLastUsername.ToLower()))
						{
							user.Username = strLastUsername;
							user.Id = Sport.Common.Tools.CIntDef(
								Sport.Core.Configuration.ReadString("LastUser", "ID"), -1);
							user.UserType = (int)Sport.Types.UserType.Internal;
							user.UserRegion = Sport.Common.Tools.CIntDef(
								Sport.Core.Configuration.ReadString("LastRegion", "ID"), -1);
						}
					}

					//dataset will be null if user if not verified.
					UserData userData = null;

					//check result of web service login:
					if ((user.Username != null) && (user.Username.Length > 0))
					{
						//user is verified, get user data:
						userData = new UserData();
						userData.Id = user.Id;
						userData.Username = user.Username;
						userData.Name = user.UserFullName;
						userData.Permissions = user.UserPermissions;
						userData.UserType = user.UserType;
						userData.UserRegion = user.UserRegion;
						userData.UserSchool = user.UserSchool;
						userData.UserPassword = password;
					}

					if (userData != null)
					{
						if (userData.UserType == (int)Sport.Types.UserType.External)
						{
							Sport.UI.MessageBox.Error("אינך מורשה להשתמש בתוכנה זו",
								"שגיאת מערכת");
						}
						else
						{
							Sport.Data.EntityType.ClearCacheEntities();
							_currentUser = userData;

							Sport.Core.Session.Region = userData.UserRegion;
							Sport.Core.Session.Season = season;
							Sport.Core.Session.User = userData;

							bLogin = true;

							if (Sport.Core.Session.Connected)
							{
								//add to the log table:
								if (Sport.Core.Configuration.ReadString(
									"General", "AllowLog") != "0")
								{
									WriteToLog();
								}
							}

							//maybe need to delete DAT files...
							if (loginForm.DeleteDatFiles)
							{
								string args = "deletedatfiles ";
								args += loginForm.Username + " ";
								args += loginForm.Password;
								System.Diagnostics.Process.Start(Application.ExecutablePath, args);
								return false;
							}

							if (Sport.Core.Session.Connected)
							{
								//need to move log contents?
								string strLogPath = System.IO.Path.GetDirectoryName(
									Application.ExecutablePath) +
									System.IO.Path.DirectorySeparatorChar + "TempLog.txt";
								if (System.IO.File.Exists(strLogPath))
								{
									System.IO.StreamReader reader =
										new System.IO.StreamReader(strLogPath);
									string strFileContents = reader.ReadToEnd();
									reader.Close();
									if ((strFileContents != null) && (strFileContents.Length > 0))
									{
										AdvancedServices.AdvancedService service =
											new AdvancedServices.AdvancedService();
										service.AddToSecurityLog(strFileContents);
									}
									System.IO.File.Delete(strLogPath);
								}

								//failed?
								if (failureCount > 0)
								{
									try
									{
										advancedService.AddToSecurityLog("[user: " +
											userData.Id + "] logged in after " + failureCount +
											" failures.");
									}
									catch
									{ }
								}
							}
						}
					}
					else
					{
						Sport.UI.MessageBox.Error("זיהוי משתמש או סיסמא שגויים",
							"שגיאת מערכת");
						if (Sport.Core.Session.Connected)
						{
							try
							{
								advancedService.AddToSecurityLog("failed login: [" +
									loginForm.Username + "] [" + loginForm.Password + "]");
							}
							catch
							{ }
						}
						failureCount++;
					}
				}
			}
			catch (Exception e)
			{
				if (e.Message.IndexOf("ExecuteReader requires an open and available Connection") >= 0)
				{
					Sport.UI.MessageBox.Error("השרת המרכזי נמצא במצב שגיאה.\nלאחר סגירת חלון זה ייפתח עמוד מערכת שיבצע אתחול לשרת הנתונים.\nאנא נסה להתחבר לתוכנה בעוד מספר דקות", "שגיאת מערכת");
					System.Diagnostics.Process.Start("http://www.schoolsport.co.il/ResetWebSite/Reset.asp");
				}
				else
				{
					string strMessage = "שגיאה בעת התחברות אל שרת הנתונים" + "\n" + e.Message;
					if (e.Message.ToLower().IndexOf("too old") >= 0)
					{
						strMessage = "גרסת התוכנה אותה הנך מנסה להפעיל ישנה.\n" +
							"יש להוריד את הגרסה החדשה על מנת להשתמש בתוכנה.";
						Sport.UI.MessageBox.Error(strMessage, "שגיאת מערכת");
						string strURL = Sport.Core.Data.ProgramUpdateURL;
						System.Diagnostics.Process.Start(strURL);
					}
					else
					{
						Sport.UI.MessageBox.Error(strMessage, "שגיאת מערכת");
					}
				}
				return false;
			}

			return bLogin;
		} //end function PerformLogin

		public static bool PerformLogin()
		{
			return PerformLogin("", "");
		}

		/// <summary>
		/// write the login to the log table
		/// </summary>
		private static void WriteToLog()
		{
			if (!Sport.Core.Session.Connected)
				return;

			try
			{
				Sport.Entities.Log log = new Sport.Entities.Log(Sport.Entities.Log.Type.New());

				log.User = new Sport.Entities.User(_currentUser.Id);
				log.Version = Sport.Core.Data.CurrentVersion;
				log.Date = DateTime.Now;
				log.Description = "כניסה למערכת";
				log.LastModified = DateTime.Now;

				log.Save();
			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.WriteLine("failed to insert into log table: " + e.Message);
				System.Diagnostics.Debug.WriteLine(e.StackTrace);
			}
		}

		/// <summary>
		/// let current user change the password of user with given username.
		/// return true if changed successfully, false otherwise.
		/// </summary>
		/// <param name="username">unique login string of the user.</param>
		/// <returns>true if changed successfully, false otherwise</returns>
		public static bool ChangeUserPassword(string username)
		{
			if (!Sport.Core.Session.Connected)
				throw new Exception("לא ניתן לשנות סיסמא במצב לא מקוון");

			//first, verify that given username is not empty:
			//long userid=Sport.Common.Tools.CLngDef(id, 0);
			if (username.Length == 0)
			{
				throw new Exception("שינוי סיסמא נכשל, לא הועבר זיהוי משתמש");
			}

			if (_service == null)
			{
				_service = new SessionServices.SessionService();
				_service.CookieContainer = Sport.Core.Session.Cookies;
			}

			//open dialog to let user change the password:
			ChangePassword objDialog = new ChangePassword();
			objDialog.Username = username;
			DialogResult dr = objDialog.ShowDialog();
			if (dr == DialogResult.OK)
			{
				//user confirmed the change.

				//get old password of the user whose password we are changing:
				string oldPassword;
				oldPassword = _service.GetUserPassword(username, CurrentUser.Username, CurrentUser.UserPassword);

				//get new password and update password in database:
				string newPassword = Crypto.Encode(objDialog.Password);
				_service.ChangeUserPassword(username, oldPassword, newPassword);

				return true;
			}

			//user canceled the action.
			return false;
		} //end function ChangeUserPassword
	} //end class UserManager
}
