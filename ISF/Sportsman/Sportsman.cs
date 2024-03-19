using System;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections;
using Sportsman.Forms;
using Sportsman.Core;
using System.Threading;
using Mir.Common;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Net;


/*
 Internal Compiler Error (0xc0000005 at address 536250C6): likely culprit is 'BIND'.

An internal error has occurred in the compiler. To work around this problem, try simplifying or changing the program near the locations listed below. Locations at the top of the list are closer to the point at which the internal error occurred.
  */

/*
Additional information: The type initializer for "Sport.Entities.ChampionshipCategory" threw an exception.
*/

/*
 תקנון א"ק ליגת על
 משתתפי קבוצה במקצוע
 קרב רב
 1-4
 תשע"ב
 אתלטיקה קלה
 מטה
 ליגת על אתלטיקה קלה
 ז-ח תלמידות
 מחזור קרב רב
 2770 - 2799
 * 
 * 
 * 
 קבלה 14996-3
*/

/*
Select c.CHAMPIONSHIP_ID, c.CHAMPIONSHIP_NAME, r.REGION_NAME, s.SPORT_NAME, 
	Case c.IS_CLUBS When 1 Then 'כן' Else 'לא' End As IS_CLUBS, 
	Case c.CHAMPIONSHIP_STATUS When 0 Then 'בתכנון' When 1 Then 'רישום קבוצות' When 2 Then 'רישום שחקנים' Else 'מאושרת' End As CHAMPIONSHIP_STATUS, 
	Case c.IS_OPEN When 1 Then 'כן' Else 'לא' End As IS_OPEN
From CHAMPIONSHIPS c Inner Join REGIONS r On c.REGION_ID=r.REGION_ID
	Inner Join SPORTS s On c.SPORT_ID=s.SPORT_ID
Where c.SEASON=70 And c.DATE_DELETED Is Null And r.DATE_DELETED Is Null
Order By c.CHAMPIONSHIP_STATUS, c.CHAMPIONSHIP_ID Desc
*/

namespace Sportsman
{
	/// <summary>
	/// Sport.UI.Manager
	/// </summary>
	public class Sportsman
	{
		public static bool IsClosing = false;
		public static string ExitMessage = "%views מסכים פתוחים \nהאם ברצונך לצאת מהתוכנה?";

		private static SportsmanContext context;
		public static SportsmanContext Context
		{
			get { return context; }
		}

		private static bool _instantMessagesFormOpened = false;
		private static Forms.MainForm _mainForm = null;

		private static MethodInvoker _openInstantMessageInvoker = null;
		private static int _recipientID = -1;

		static bool MyCertHandler(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors error)
		{
			// Ignore errors
			return true;
		}

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			System.Windows.Forms.Form.CheckForIllegalCrossThreadCalls = false;
			ServicePointManager.ServerCertificateValidationCallback = MyCertHandler;

			TextFileLogWriter logWriter = new TextFileLogWriter("Sportsman_Log.txt", true, false);
			logWriter.MaxSizeBytes = 5242880; //5 MB
			Logger.Instance.AddLogWriter(logWriter);
			Logger.Instance.LogLevel = 350;

			bool blnCommandLineOffline = (args != null && args.Length > 0 && args[0] == "-offline");
			//MessageBox.Show(args.Length.ToString());

			//delete DAT files?
			if ((args.Length > 0) && (args[0] == "deletedatfiles"))
				DeleteDatFiles(Sport.Common.Tools.PartialJoin(args, 3, args.Length));

			//jump to existing process if exist.
			int pID = Tools.AlreadyRunning();
			if (pID >= 0)
			{
				System.Diagnostics.Debug.WriteLine("process already running (pID: " + pID + "), terminating application.");
				return;
			}

			//check if need to get full version from website:
			double currentVersion = Sport.Core.Data.CurrentVersion;
			if (currentVersion < 1.202)
			{
				string strMessage = "עקב שינוי שרת מרכזי לא ניתן לעדכן אוטומטית את התוכנה\n" +
					"אנא הורד גרסה מלאה דרך כלי הרישום באתר האינטרנט\n" +
					"(לאחר רישום בחר הורדת עדכונים --> עדכון תוכנה)\n" +
					"לאחר התקנת הגרסה החדשה, יחזרו העדכונים האוטומטיים לפעולה";
				Sport.UI.MessageBox.Error(strMessage, "שגיאת מערכת");
				string strURL = Sport.Core.Data.ProgramUpdateURL;
				System.Diagnostics.Process.Start(strURL);
				return;
			}

			double lastVersion = 0;
			if (blnCommandLineOffline || Sport.Core.Utils.GetAppSetting("ForceOfflineMode").Equals("1"))
			{
				Sport.Core.Session.Connected = false;
			}
			else
			{
				//check the version and also check if we're connected:
				SessionServices.SessionService service = new SessionServices.SessionService();
				service.Timeout = 6000;
				Sport.UI.Dialogs.WaitForm.ShowWait("מתחבר לשרת אנא המתן...");
				try
				{
					lastVersion = service.GetLatestVersion().VersionNumber;
				}
				catch (Exception ex)
				{
					//Sport.UI.MessageBox.Show(ex.Message);
					System.Diagnostics.Debug.WriteLine("failed to get latest version: " + ex.Message);
					Logger.Instance.WriteLog(LogType.Error, "Error getting latest version: " + ex.ToString());
					Sport.Core.Session.Connected = false;
				}
				finally
				{
					Sport.UI.Dialogs.WaitForm.HideWait();
				}
			}

			string strOfflineFolder =
				Path.GetDirectoryName(Application.ExecutablePath) +
				Path.DirectorySeparatorChar + "Offline";

			//new version available?
			if (lastVersion > currentVersion)
			{
				Sport.UI.MessageBox.Warn("גרסת התוכנה אינה מעודכנת, דברים מסויימים עשויים לא לעבוד\n\n" + 
					"יש להוריד גרסה עדכנית לפי הוראות במסמך שנשלח במייל", 
					"אזהרה");
				/*
				//this will delete all offline data. store this data.

				//get list of the offline championship with result:
				int recentFileIndex = -1;
				string[] arrFiles = null;
				int[] arrOfflineChampID =
					Producer.OpenChampionshipCommand.GetOfflineChamionships(true,
					ref recentFileIndex, ref arrFiles);

				//got anything?
				if ((arrFiles != null) && (arrFiles.Length > 0))
				{
					if (!Directory.Exists(strOfflineFolder))
						Directory.CreateDirectory(strOfflineFolder);
					foreach (string strSourceFile in arrFiles)
					{
						string strDestFile = strOfflineFolder + Path.DirectorySeparatorChar +
							Path.GetFileName(strSourceFile);
						File.Copy(strSourceFile, strDestFile, true);
					}
				}

				//spawn the update process:
				System.Diagnostics.Process.Start("SportVersionUpdate.exe");

				//kill current process:
				return;
				*/
			}

			//got offline files to copy?
			if (Directory.Exists(strOfflineFolder))
			{
				//get files:
				string[] arrFiles = Directory.GetFiles(strOfflineFolder);
				string strCacheFolder = Sport.Core.Session.GetSeasonCache(false);

				//copy:
				foreach (string strSourceFile in arrFiles)
				{
					string strDestFile = strCacheFolder + Path.DirectorySeparatorChar +
						Path.GetFileName(strSourceFile);
					File.Copy(strSourceFile, strDestFile, true);
				}

				//remove temporary folder:
				Directory.Delete(strOfflineFolder, true);
			}

			//connected?
			if (!Sport.Core.Session.Connected)
			{
				if (!Sport.UI.MessageBox.Ask("לא נמצא חיבור אינטרנט, התוכנה יכולה לעבוד במצב לא מקוון.\n" +
					"במצב זה ניתן לעשות פעולות מוגבלות ביותר בתוכנה.\nהאם להמשיך?",
					System.Windows.Forms.MessageBoxIcon.Warning, true))
				{
					return;
				}

				if (!CheckLastData(new string[] {"LastUser|Password|אין סיסמא", 
					"LastUser|Username|אין שם משתמש", "LastUser|ID|אין זיהוי משתמש", 
					"LastRegion|ID|אין זיהוי מחוז", "LastSeason|ID|אין עונה"}))
				{
					return;
				}
			}

			//force login:
			string username = (args.Length > 1) ? args[1] : "";
			string password = (args.Length > 2) ? args[2] : "";
			if (!UserManager.PerformLogin(username, password))
				return;

			//import offline data?
			if (Sport.Core.Session.Connected)
			{
				if (GotOfflineData())
				{
					Forms.ImportOfflineEntities objDialog =
						new Forms.ImportOfflineEntities();
					objDialog.ShowDialog();
				}
			}

			InputLanguage.CurrentInputLanguage =
				InputLanguage.FromCulture(new System.Globalization.CultureInfo("he-IL"));

			//invokers:
			_openInstantMessageInvoker = new MethodInvoker(OpenInstantMessagesView);

			//states:
			TimerState keepAliveState = null;
			TimerState checkMessagesState = null;
			TimerState checkInstantMessagesState = null;

			//timers:
			if (Sport.Core.Session.Connected)
			{
				keepAliveState = new TimerState();
				checkMessagesState = new TimerState();

				//Create the delegate that invokes methods for the timer.
				System.Threading.TimerCallback keepAliveDelegate =
					new System.Threading.TimerCallback(KeepAliveTick);
				System.Threading.TimerCallback checkMessagesDelegate =
					new System.Threading.TimerCallback(CheckNewMessages);

				//Create the timers:
				System.Threading.Timer keepAliveTimer = new System.Threading.Timer(
					keepAliveDelegate, keepAliveState, 60000, 300000); //every 5 minutes
				System.Threading.Timer checkMessagesTimer = new System.Threading.Timer(
					checkMessagesDelegate, checkMessagesState, 5000, 60000 * 3); //every one minute

				//Keep a handle to the timer, so it can be disposed:
				keepAliveState.timer = keepAliveTimer;
				checkMessagesState.timer = checkMessagesTimer;
			}

			//catch all exceptions:
			Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);

			//got offline championships?
			if (!Sport.Core.Session.Connected)
			{
				if (Producer.OpenChampionshipCommand.OfflineChampionshipsCount == 0)
				{
					return;
				}
			}

			_mainForm = new MainForm();
			context = new SportsmanContext(_mainForm);
			_mainForm.Show();

			if (Sport.Core.Session.Connected)
			{
				checkInstantMessagesState = new TimerState();
				System.Threading.TimerCallback checkInstantMessagesDelegate =
					new System.Threading.TimerCallback(CheckInstantMessages);
				System.Threading.Timer checkInstantMessagesTimer = new System.Threading.Timer(
					checkInstantMessagesDelegate, checkInstantMessagesState, 1000, 60000); //every 15 seconds
				checkInstantMessagesState.timer = checkInstantMessagesTimer;
			}

			Application.Run(context);

			//dispose:
			if (Sport.Core.Session.Connected)
			{
				keepAliveState.timer.Dispose();
				checkMessagesState.timer.Dispose();
				checkInstantMessagesState.timer.Dispose();
				keepAliveState.timer = null;
				checkMessagesState.timer = null;
				checkInstantMessagesState.timer = null;
			}
		}

		private static bool GotOfflineData()
		{
			string strFilePath = Path.GetDirectoryName(Application.ExecutablePath) +
				Path.DirectorySeparatorChar + Sport.Data.OfflineEntity.FileName;
			if (!File.Exists(strFilePath))
				return false;
			System.Type[] types = new System.Type[] {
				typeof(Sport.Entities.OfflineStudent), typeof(Sport.Entities.OfflineSchool), 
				typeof(Sport.Entities.OfflinePlayer), typeof(Sport.Entities.OfflineTeam) };
			bool result = false;
			foreach (System.Type type in types)
			{
				Sport.Data.OfflineEntity[] entities =
					Sport.Data.OfflineEntity.LoadAllEntities(type);
				if (entities.Length > 0)
				{
					result = true;
					break;
				}
			}
			if (result == false)
				File.Delete(strFilePath);
			return result;
		}

		private static bool CheckLastData(string section, string key, string message)
		{
			string strValue = Sport.Common.Tools.CStrDef(
				Sport.Core.Configuration.ReadString(section, key), "");
			if (strValue.Length == 0)
			{
				Sport.UI.MessageBox.Error("לא ניתן לעבוד במצב לא מקוון:\n" + message,
					"שגיאת מערכת");
				return false;
			}
			return true;
		}

		private static bool CheckLastData(string[] data)
		{
			if (data == null)
				return true;
			for (int i = 0; i < data.Length; i++)
			{
				string curData = data[i];
				if ((curData == null) || (curData.Length == 0))
					continue;
				string[] arrTemp = curData.Split(new char[] { '|' });
				if (arrTemp.Length != 3)
					continue;
				if (!CheckLastData(arrTemp[0], arrTemp[1], arrTemp[2]))
					return false;
			}
			return true;
		}

		private static void OpenInstantMessagesView()
		{
			_mainForm.OpenInstantMessagesView(_recipientID);
		}

		private static void DeleteDatFiles(object[] arrEntityNames)
		{
			if ((arrEntityNames == null) || (arrEntityNames.Length == 0))
			{
				//delete cache
				System.Diagnostics.Debug.WriteLine("deleting all DAT files...");
				string strCachePath = Path.GetDirectoryName(Application.ExecutablePath) +
					Path.DirectorySeparatorChar + "Cache";
				if (System.IO.Directory.Exists(strCachePath))
					System.IO.Directory.Delete(strCachePath, true);
				return;
			}

			System.Diagnostics.Debug.WriteLine("deleting DAT files: " + String.Join(", ", (string[])(new ArrayList(arrEntityNames)).ToArray(typeof(string))));
			System.IO.DirectoryInfo objFolder = null;
			System.IO.FileInfo[] arrFiles = null;
			string strFolderPath = Application.StartupPath + "\\Cache";
			try
			{
				objFolder = new System.IO.DirectoryInfo(strFolderPath);
				arrFiles = objFolder.GetFiles("*.dat");
			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.WriteLine("error opening folder " + strFolderPath + ": " + e.Message);
			}
			if (objFolder != null)
			{
				//Sport.UI.MessageBox.Show("files: "+arrFiles.Length);
				foreach (System.IO.FileInfo file in arrFiles)
				{
					bool blnDelete = true;
					if (arrEntityNames.Length > 0)
					{
						blnDelete = false;
						string strFileName = Path.GetFileNameWithoutExtension(file.Name).ToLower();
						for (int j = 0; j < arrEntityNames.Length; j++)
						{
							//Sport.UI.MessageBox.Show("file in array: "+arrEntityNames[j].ToString().ToLower()+", file in disk: "+strFileName);
							if (arrEntityNames[j].ToString().ToLower() == strFileName)
							{
								blnDelete = true;
								break;
							}
						}
					}
					if (blnDelete == true)
					{
						string message = "deleting file " + file.FullName + "...";
						try
						{
							file.Delete();
							message += " [success]";
						}
						catch (Exception e)
						{
							message += " [error: " + e.Message + "]";
						}
						System.Diagnostics.Debug.WriteLine(message);
					}
				}
			}
			System.Diagnostics.Debug.WriteLine("...done deleting all DAT files");
		}

		private static void KeepAliveTick(Object state)
		{
			if (!Sport.Core.Session.Connected)
				return;

			SessionServices.SessionService _service = new SessionServices.SessionService();
			_service.CookieContainer = Sport.Core.Session.Cookies;
			bool alive = false;
			try
			{
				alive = _service.KeepAlive();
			}
			catch (Exception ex)
			{
				alive = false;
				Logger.Instance.WriteLog(LogType.Warning, "Sportsman", "Error in Keep Alive: " + ex.Message);
			}
			if (!alive)
			{
				//Sport.UI.MessageBox.Show("נותקת מהשרת, שלום!");

				//Application.Exit();
			}
		}

		public static void CheckNewMessages(Object state)
		{
			//connected?
			if (!Sport.Core.Session.Connected)
				return;

			//maybe form is not yet initialized:
			if (context == null)
				return;

			try
			{
				//reset memory entities:
				Sport.Entities.Message.Type.Reset(null);

				//get current logged on user:
				int userid = UserManager.CurrentUser.Id;

				//create proper message filter - only new messages for logged in user:
				Sport.Data.EntityFilter filter = new Sport.Data.EntityFilter((int)Sport.Entities.Message.Fields.User, userid);
				filter.Add(new Sport.Data.EntityFilterField((int)Sport.Entities.Message.Fields.Status, (int)Sport.Types.MessageStatus.New));

				//get all messages:
				//removed 18/05/2014 - not used, not needed any more.
				//ArrayList arrMessages = new ArrayList(
				//	Sport.Entities.Message.Type.GetEntities(filter));
				ArrayList arrMessages = new ArrayList();

				//need to move some to archive?
				ArrayList arrOldMessages = new ArrayList();
				foreach (Sport.Data.Entity entMessage in arrMessages)
				{
					//get time sent:
					DateTime curTimeSent = DateTime.MinValue;
					object objTimeSent = entMessage.Fields[(int)Sport.Entities.Message.Fields.TimeSent];
					if (objTimeSent != null)
						curTimeSent = (DateTime)objTimeSent;
					if ((DateTime.Now - curTimeSent).TotalDays > 14)
						arrOldMessages.Add(entMessage);
				}

				//got anything to move?
				if (arrOldMessages.Count > 0)
				{
					Sport.UI.Dialogs.WaitForm.ShowWait("מעביר הודעות לארכיון אנא המתן...", true);
					Sport.UI.Dialogs.WaitForm.SetProgress(0);
					int curProgress = 0;
					int progressDiff = (int)(((double)100) / ((double)arrOldMessages.Count));
					foreach (Sport.Data.Entity entOldMessage in arrOldMessages)
					{
						arrMessages.Remove(entOldMessage);
						Sport.Data.EntityEdit entEdit = entOldMessage.Edit();
						entEdit.Fields[(int)Sport.Entities.Message.Fields.Status] =
							(int)Sport.Types.MessageStatus.Archieve;
						entEdit.Save();
						curProgress += progressDiff;
						Sport.UI.Dialogs.WaitForm.SetProgress(curProgress);
					}
					Sport.UI.Dialogs.WaitForm.HideWait();
				}

				//get count of new messages:
				int messageCount = arrMessages.Count;

				//update status window:
				string statusText = "";
				switch (messageCount)
				{
					case 0:
						statusText = "אין הודעות חדשות";
						break;
					case 1:
						statusText = "הודעה חדשה אחת";
						break;
					default:
						statusText = "הודעות חדשות: " + messageCount.ToString();
						break;
				}
				context.SetStatusBar(Forms.MainForm.StatusBarPanels.Messages,
					statusText);
			}
			catch
			{
			}
			//System.Windows.Forms.MessageBox.Show("new messages: "+messages.Length.ToString());
		}

		public static void CheckInstantMessages(Object state)
		{
			//connected?
			if (!Sport.Core.Session.Connected)
				return;

			//maybe form is not yet initialized:
			if (context == null)
				return;

			//already opened?
			if (_instantMessagesFormOpened)
				return;

			try
			{
				//get current logged on user:
				int userID = UserManager.CurrentUser.Id;

				//create proper message filter - only new messages for logged in user:
				Sport.Data.EntityFilter filter = new Sport.Data.EntityFilter(
					(int)Sport.Entities.InstantMessage.Fields.Recipient, userID);
				filter.Add(new Sport.Data.EntityFilterField(
					(int)Sport.Entities.InstantMessage.Fields.DateRead, null));

				//get all messages:
				//removed 18/05/2014 - not used, not needed any more.
				Sport.Data.Entity[] messages = new Sport.Data.Entity[] { };
					//Sport.Entities.InstantMessage.Type.GetEntities(filter);

				//got anything?
				if ((messages != null) && (messages.Length > 0))
				{
					_instantMessagesFormOpened = true;
					foreach (Sport.Data.Entity messageEnt in messages)
					{
						Forms.InstantMessageForm objForm = new InstantMessageForm(messageEnt.Id);
						objForm.ShowDialog(_mainForm);
						int recipientID = objForm.ReplyRecipient;
						if (recipientID >= 0)
						{
							_recipientID = recipientID;
							_mainForm.BeginInvoke(_openInstantMessageInvoker);
						}
					}
					_instantMessagesFormOpened = false;
				}
			}
			catch (Exception ex)
			{
				Logger.Instance.WriteLog(LogType.Warning, "Sportsman", "General error checking instant messages: " + ex.ToString());
			}
		}

		private class TimerState
		{
			public System.Threading.Timer timer;
		}

		private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
		{
			//work with latest exception in chain
			Exception exception = e.Exception;
			int failSafeCounter = 0;
			while (exception.InnerException != null)
			{
				if (failSafeCounter >= 100)
					break;
				exception = exception.InnerException;
				failSafeCounter++;
			}

			if (!Sport.Core.Session.Connected)
			{
				Sport.UI.MessageBox.Error("אירעה שגיאה לא מזוהה בתוכנה, לאחר הצגת השגיאה התוכנה תסגר", "שגיאה קריטית");
				MessageBox.Show(exception.Message + "\nStack:\n"
					+ exception.StackTrace, "Details",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
				Application.Exit();
				return;
			}

			if (exception.StackTrace != null && exception.StackTrace.IndexOf("System.Windows.Forms.UnsafeNativeMethods.CallWindowProc") >= 0 && exception.StackTrace.IndexOf("System.Windows.Forms.NativeWindow.Callback") > 0)
			{
				Sport.UI.MessageBox.Error("אירעה שגיאה כללית במערכת הדוט נט המותקנת במחשב זה.\nניתן להוריד קובץ תיקון מאתר האינטרנט בכלי הניהול\nלנוחיותך האתר ייפתח כעת בעמוד המתאים יש להרשם ולהוריד את הקובץ", "שגיאת מערכת");
				System.Diagnostics.Process.Start("http://www.schoolsport.co.il/Register.aspx?action=DownloadUpdates&fileIndex=2");
				return;
			}

			Sport.UI.MessageBox.Error("אירעה שגיאה לא מזוהה, כדי למנוע בעיות נוספות התוכנה כעת תסגר.", "שגיאת מערכת");

			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append("Active Views: " + Sport.UI.ViewManager.ViewsCount() + "<br />");
			for (int i = 0; i < Sport.UI.ViewManager.ViewsCount(); i++)
			{
				Sport.UI.View view = Sport.UI.ViewManager.GetView(i);
				if (view != null)
				{
					sb.Append(view.GetType().FullName + " ");
					sb.Append("state: " + view.State.ToString());
					if (Sport.UI.ViewManager.SelectedViewIndex == i)
						sb.Append(" (SELECTED)");
					sb.Append("<br />");
				}
			}
			AdvancedServices.AdvancedService service = new AdvancedServices.AdvancedService();
			try
			{
				service.ReportUnhandledError(exception.Message,
					exception.StackTrace + "<br />" + sb.ToString(),
					Core.UserManager.CurrentUser.Id, DateTime.Now, Application.ExecutablePath);
			}
			catch
			{
			}

			if (exception.Message.IndexOf("ExecuteReader requires an open and available Connection. The connection's current state is closed.") > 0 || exception.Message.IndexOf("There is already an open DataReader associated with this Command") >= 0 || exception.Message.IndexOf("New transaction is not allowed because there are other threads running in the session") > 0)
				System.Diagnostics.Process.Start("http://www.schoolsport.co.il/ResetWebSite/Reset.asp");

			Application.Exit();
		}
	} //end class Sportsman
}
