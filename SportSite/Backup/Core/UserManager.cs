using System;
using System.Linq;
using System.Data;
using Sport.Data;
using Sport.Entities;
using SportSite.Common;
using Sport.Common;
using System.Web;
using System.Collections.Generic;
using System.IO;

namespace SportSite.Core
{
	public struct UserData
	{
		public int Id;
		public string Login;
		public string Name;
		public int Type;
		public int Permissions;
		public SimpleData Region;
		public SimpleData School;
		public string Password;

		public static readonly UserData Empty;

		public UserData(SessionService.UserData user)
		{
			this.Id = user.Id;
			this.Login = user.Username;
			this.Name = user.UserFullName;
			this.Type = user.UserType;
			this.Permissions = user.UserPermissions;
			this.Region = new SimpleData(user.UserRegion, "");
			this.School = new SimpleData(user.UserSchool, "");
			this.Password = "";
		}

		static UserData()
		{
			Empty = new UserData();
			Empty.Id = -1;
			Empty.Login = null;
		}
	}

	/// <summary>
	/// describe the current logged in user, per user session.
	/// </summary>
	public class UserManager
	{
		public static readonly string SessionKey = "current_user";
		public static readonly string RememberMeKey = "remember_me_user";
		private System.Web.SessionState.HttpSessionState Session = null;

		public static void RemoveRememberMeCookiesFromBrowser()
		{
			CookieManager.Remove(UserManager.RememberMeKey);
			CookieManager.Remove("rmp");
		}

		/// <summary>
		/// create new instance of UserManager for given user Session.
		/// </summary>
		public UserManager(System.Web.SessionState.HttpSessionState session)
		{
			Session = session;
		}

		/// <summary>
		/// check given user against database.
		/// if user exist in database, return true and put UserData object in Session.
		/// otherwise, return false.
		/// </summary>
		public bool VerifyUser(string userName, string password)
		{
			System.Net.CookieContainer cookies = new System.Net.CookieContainer();

			UserData loggedInUser = UserData.Empty;
			SessionService.UserData objUser = null;

			//create instance of the session service:
			SessionService.SessionService service = new SportSite.SessionService.SessionService();
			service.CookieContainer = cookies;

			//get list of seasons:
			Sport.Core.Session.Cookies = cookies;
			Entity[] seasons = Sport.Entities.Season.Type.GetEntities(null);
			Sport.Core.Session.Cookies = null;

			//find max (current) season:
			int maxSeason = seasons.ToList().Max(s => s.Id);

			//invoke web method to verify against database:
			string strEncryped = Crypto.Encode(password);
			string strCurVersion = Sport.Core.Data.CurrentVersion.ToString();
			objUser = service.Login(userName + "#website#" + strCurVersion,
				strEncryped, maxSeason);

			//check if empty:
			if (objUser.Username == null || objUser.Username.Length == 0)
			{
				FailedLoginManager.AddFailedLogin(userName, password);
				return false;
			}

			Sport.Core.Session.Cookies = cookies;

			//user exist in database. apply details:
			loggedInUser = new UserData(objUser);
			//get details from database:
			EntityType regionType = EntityType.GetEntityType(Sport.Entities.Region.TypeName);
			Entity region = regionType.Lookup(loggedInUser.Region.ID);
			loggedInUser.Region.Name = region.Name;
			loggedInUser.Password = strEncryped;
			if (loggedInUser.School.ID >= 0)
			{
				EntityType schoolType = EntityType.GetEntityType(Sport.Entities.School.TypeName);
				Entity school = schoolType.Lookup(loggedInUser.School.ID);
				loggedInUser.School.Name = school.Name;
			}

			//apply in session:
			Session[SessionKey] = loggedInUser;
			Session["cookies"] = cookies;

			Sport.Core.Session.Cookies = null;
			return true;
		}

		private static class FailedLoginManager
		{
			private static string FILE_NAME = "FailedLogins.txt";
			private static string filePath = "";
			private static object fileLocker = new object();
			static FailedLoginManager()
			{
				filePath = HttpContext.Current.Server.MapPath(FILE_NAME);
			}

			public static void AddFailedLogin(string userName, string password)
			{
				string visitorIpAddress = SportSite.Common.Tools.GetIPAddress();
				if (visitorIpAddress.Length > 0)
				{
					List<FailedLoginItem> allItems = GetAllItems();
					FailedLoginItem failedLoginItem = allItems.Find(item => item.IpAddress.Equals(visitorIpAddress));
					if (failedLoginItem == null)
					{
						failedLoginItem = new FailedLoginItem
						{
							IpAddress = visitorIpAddress, 
							FailureCount = 0, 
							LoginRawData = ""
						};
						allItems.Add(failedLoginItem);
					}
					failedLoginItem.FailureCount++;
					failedLoginItem.LastAttempt = DateTime.Now;
					failedLoginItem.AddLoginData(userName, password);
					SaveToFile(allItems);
				}
			}

			private static void SaveToFile(List<FailedLoginItem> allItems)
			{
				List<string> lines = allItems.ConvertAll(item => item.ToString());
				lock (fileLocker)
				{
					File.WriteAllLines(filePath, lines.ToArray());
				}
			}

			private static List<FailedLoginItem> GetAllItems()
			{
				List<FailedLoginItem> items = new List<FailedLoginItem>();
				if (File.Exists(filePath))
				{
					List<string> lines = new List<string>();
					lock (fileLocker)
					{
						lines.AddRange(File.ReadAllLines(filePath));
					}
					items.AddRange(lines.ConvertAll(line => FailedLoginItem.Parse(line)));
					items.RemoveAll(item => item == null);
				}
				return items;
			}

			private class FailedLoginItem
			{
				private readonly char loginRawDataMainSeparator = '|';
				private readonly string loginRawDataSubSeparator = "-";

				public string IpAddress { get; set; }
				public string LoginRawData { get; set; }
				public int FailureCount { get; set; }
				public DateTime LastAttempt { get; set; }

				public void AddLoginData(string userName, string password)
				{
					List<string> existingParts = this.LoginRawData.Split(loginRawDataMainSeparator).ToList();
					existingParts.Add(string.Format("{0},{1}",
						SportSite.Common.Tools.SplitString(userName, loginRawDataSubSeparator),
						SportSite.Common.Tools.SplitString(password, loginRawDataSubSeparator)));
					this.LoginRawData = string.Join(loginRawDataMainSeparator.ToString(), existingParts);
				}

				public override string ToString()
				{
					return string.Format("{0}\t{1}\t{2}\t{3}", this.IpAddress, this.FailureCount, this.LoginRawData, this.LastAttempt.Ticks);
				}

				public static FailedLoginItem Parse(string rawLine)
				{
					if (rawLine.Length == 0)
						return null;

					string[] parts = rawLine.Split('\t');
					if (parts.Length <= 3)
						return null;

					string ipAddress = parts[0], rawFailureCount = parts[1], rawLastAttempt = parts[3];
					if (ipAddress.Length == 0 || rawFailureCount.Length == 0 || rawLastAttempt.Length == 0)
						return null;

					int failureCount;
					if (!Int32.TryParse(rawFailureCount, out failureCount) || failureCount < 0)
						return null;

					long ticks;
					if (!Int64.TryParse(rawLastAttempt, out ticks) || ticks <= 0)
						return null;

					return new FailedLoginItem
					{
						IpAddress = ipAddress, 
						LoginRawData = parts[2], 
						FailureCount = failureCount, 
						LastAttempt = new DateTime(ticks)
					};
				}
			}
		}
	}
}
