using System;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Sport.Data;
using System.Collections;
using System.Collections.Generic;

namespace Sportsman.Core
{
	/// <summary>
	/// Summary description for Tools.
	/// </summary>
	public class Tools
	{
		[DllImport("user32.dll")]
		private static extern
			bool SetForegroundWindow(IntPtr hWnd);
		[DllImport("user32.dll")]
		private static extern
			bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
		[DllImport("user32.dll")]
		private static extern
			bool IsIconic(IntPtr hWnd);

		public static object GetStateValue(string state)
		{
			if (state == null)
				return null;

			if (state[0] == '@') // Session parameter
			{
				return Sport.Core.Session.Get(state.Substring(1)).Value;
			}

			try
			{
				int n = Int32.Parse(state);
				return n;
			}
			catch
			{
				System.Diagnostics.Debug.WriteLine("failed to get state value: " + state);
			}

			return null;
		} //end function GetStateValue

		/// <summary>
		/// get filter value as parameter and returns the unique ID of its 
		/// corresponding Entity if exists, or null if the value is null.
		/// supported filters are of type Entity or LookupItem.
		/// </summary>
		public static string GetFilterValue(object filter)
		{
			//check if null:
			if (filter == null)
				return null;

			//check if entity:
			if (filter is Entity)
				return ((Entity)filter).Id.ToString();

			//check if LookupItem:
			if (filter is Sport.Data.LookupItem)
				return ((Sport.Data.LookupItem)filter).Id.ToString();

			//unknown type!
			throw new Exception("invalid filter type: " + filter.GetType().Name);
		} //end function GetFilterValue

		/// <summary>
		/// set given filter value to given object
		/// </summary>
		public static void SetFilterValue(Sport.UI.TableView.Filter filter, object value)
		{
			filter.StopEvents = true;
			filter.Value = value;
			filter.StopEvents = false;
		}

		/// <summary>
		/// check if current process already running. if running, set focus to 
		/// existing process and returns the other process ID. otherwise, returns -1.
		/// </summary>
		/// <returns></returns>
		public static int AlreadyRunning()
		{
			/*
			const int SW_HIDE = 0;
			const int SW_SHOWNORMAL = 1;
			const int SW_SHOWMINIMIZED = 2;
			const int SW_SHOWMAXIMIZED = 3;
			const int SW_SHOWNOACTIVATE = 4;
			const int SW_RESTORE = 9;
			const int SW_SHOWDEFAULT = 10;
			*/
			const int SW_RESTORE = 9;

			//string myName=Path.GetFileNameWithoutExtension(Application.ExecutablePath);
			Process me = Process.GetCurrentProcess();
			Process currentProccess = Process.GetCurrentProcess();
			int sessionID = (currentProccess == null) ? 0 : currentProccess.SessionId;
			Process[] arrProcesses = Process.GetProcessesByName(me.ProcessName).Where(p => p.SessionId == sessionID).ToArray();
			int pID = -1;
			if (arrProcesses.Length > 1)
			{
				for (int i = 0; i < arrProcesses.Length; i++)
				{
					pID = arrProcesses[i].Id;
					if (pID != me.Id)
					{
						// get the window handle
						IntPtr hWnd = arrProcesses[i].MainWindowHandle;
						// if iconic, we need to restore the window
						if (IsIconic(hWnd))
						{
							ShowWindowAsync(hWnd, SW_RESTORE);
						}
						// bring it to the foreground
						SetForegroundWindow(hWnd);
						break;
					}
				}
				return (pID == -1) ? 0 : pID;
			}

			return -1;
		}

		public static Control FindControlDeep(Control parent, string childName)
		{
			//search children:
			foreach (Control childControl in parent.Controls)
			{
				//maybe direct child?
				if (childControl.Name == childName)
					return childControl;

				//maybe nested inside the child then:
				Control control = FindControlDeep(childControl, childName);

				//check if we found something:
				if (control != null)
					return control;
			}

			//nada...
			return null;
		}

		/// <summary>
		/// attach given tooltip string to every child control with given text.
		/// </summary>
		public static void AttachGlobalTooltip(Control parent, string text,
			string strTooltip)
		{
			foreach (Control childControl in parent.Controls)
			{
				if (childControl.Text == text)
				{
					Sport.Common.Tools.AttachTooltip(childControl, strTooltip);
				}
				AttachGlobalTooltip(childControl, text, strTooltip);
			}
		}

		public static System.Drawing.Size MeasureText(Control container, string text)
		{
			Label label = new Label();
			label.AutoSize = true;
			label.Text = text;
			container.Controls.Add(label);
			System.Drawing.Size size = new System.Drawing.Size(label.Width, label.Height);
			//container.Controls.Remove(label);
			return size;
		}

		public static Label QuickLabel(string text)
		{
			Label result = new Label();
			result.Text = text;
			return result;
		}

		public static string GetUserOrTeamRegion(
			Sport.Entities.Championship championship, ref int regionID)
		{
			string result = "ארצי";
			regionID = Sport.Entities.Region.CentralRegion;
			if (Core.UserManager.CurrentUser.UserRegion != Sport.Entities.Region.CentralRegion)
			{
				regionID = Core.UserManager.CurrentUser.UserRegion;
				Sport.Entities.Region region = new Sport.Entities.Region(regionID);
				result = region.Name;
			}
			else
			{
				if (!championship.Region.IsNationalRegion())
				{
					regionID = championship.Region.Id;
					result = championship.Region.Name;
				}
			}
			return result;
		}

		public static string GetUserOrTeamRegion(
			Sport.Entities.Championship championship)
		{
			int dummy = 0;
			return GetUserOrTeamRegion(championship, ref dummy);
		}

		public static string GetUserOrTeamRegion(Sport.Entities.Team team)
		{
			return GetUserOrTeamRegion(team.Championship);
		}

		public static void InputMargins(string key, int defTopMargin,
			int defSideMargin)
		{
			if (Sport.Core.Configuration.ReadString(key, "TopMargin") == null)
			{
				Sport.Core.Configuration.WriteString(key, "TopMargin", defTopMargin.ToString());
				Sport.Core.Configuration.WriteString(key, "SideMargin", defSideMargin.ToString());
			}
			Forms.InputMarginsForm objForm = new Forms.InputMarginsForm(key);
			if (objForm.ShowDialog() == DialogResult.OK)
			{
				string strTopMargin = objForm.TopMarginValue.ToString();
				string strSideMargin = objForm.SideMarginValue.ToString();
				Sport.Core.Configuration.WriteString(key, "TopMargin", strTopMargin);
				Sport.Core.Configuration.WriteString(key, "SideMargin", strSideMargin);
				Sport.UI.MessageBox.Success("נתונים עודכנו בהצלחה", "הודעת מערכת");
			}
		}

		public static Dictionary<Sport.Entities.ChampionshipCategory, Dictionary<Sport.Entities.School, List<Sport.Entities.Team>>>
			GetAllChampionships(int regionID, bool blnClubs)
		{
			Sport.Data.EntityFilter filter = new Sport.Data.EntityFilter(
				(int)Sport.Entities.Championship.Fields.IsClubs, blnClubs ? 1 : 0);
			filter.Add(new Sport.Data.EntityFilterField((int)Sport.Entities.Championship.Fields.Season, Sport.Core.Session.Season));
			if (regionID >= 0)
				filter.Add(new Sport.Data.EntityFilterField((int)Sport.Entities.Championship.Fields.Region, regionID));
			Sport.Data.Entity[] arrChampEnts = Sport.Entities.Championship.Type.GetEntities(filter);
			Dictionary<Sport.Entities.ChampionshipCategory, Dictionary<Sport.Entities.School, List<Sport.Entities.Team>>> categoryTeams =
				new Dictionary<Sport.Entities.ChampionshipCategory, Dictionary<Sport.Entities.School, List<Sport.Entities.Team>>>();
			Dictionary<Sport.Entities.School, List<Sport.Entities.Team>> schoolTeams;
			foreach (Sport.Data.Entity champEnt in arrChampEnts)
			{
				Sport.Entities.Championship championship = new Sport.Entities.Championship(champEnt);
				Sport.Entities.ChampionshipCategory[] allCategories = championship.GetCategories();
				foreach (Sport.Entities.ChampionshipCategory currentCategory in allCategories)
				{
					Sport.Entities.Team[] teams = currentCategory.GetTeams();
					if (teams != null && teams.Length > 0)
					{
						if (!categoryTeams.TryGetValue(currentCategory, out schoolTeams))
						{
							schoolTeams = new Dictionary<Sport.Entities.School, List<Sport.Entities.Team>>();
							categoryTeams.Add(currentCategory, schoolTeams);
						}
						foreach (Sport.Entities.Team team in teams)
						{
							Sport.Entities.School currentSchool = team.School;
							if (!schoolTeams.ContainsKey(currentSchool))
								schoolTeams.Add(currentSchool, new List<Sport.Entities.Team>());
							schoolTeams[currentSchool].Add(team);
						}
					}
				}
			}
			return categoryTeams;
		}

		public static string[] ReadFileSelectedByUser()
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Tab Delimeted Text File|*.txt";
			openFileDialog.ShowReadOnly = true;
			openFileDialog.Title = "יבא מקובץ";
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				string strFilePath = openFileDialog.FileName;
				try
				{
					string[] lines = Sport.Common.Tools.ReadTextFile(strFilePath, true);
					return lines;
				}
				catch (IOException)
				{
					Sport.UI.MessageBox.Error("שגיאה בעת קריאת קובץ, אנא וודאו שהקובץ לא פתוח במקום אחר", "פתיחת קובץ");
				}
			}
			return null;
		}

		public static Dictionary<string, string> ParseDefinitionFile(string filePath)
		{
			Dictionary<string, string> values = new Dictionary<string, string>();
			if (File.Exists(filePath))
			{
				File.ReadLines(filePath).ToList().ConvertAll(line => line.Trim()).FindAll(line => line.IndexOf("=") > 0).ForEach(line =>
				{
					string[] parts = line.Split('=');
					if (parts.Length == 2)
					{
						string key = parts[0].Trim();
						if (key.Length > 0 && !values.ContainsKey(key))
						{
							string value = parts[1].Trim();
							values.Add(key, value);
						}
					}
				});
			}
			return values;
		}

		public static int GetValueOrDefault(IDictionary dictionary, object key, int defaultValue)
		{
			if (!dictionary.Contains(key))
				return defaultValue;
			string rawValue = dictionary[key] + "";
			int value;
			if (rawValue.Length == 0 || !Int32.TryParse(rawValue, out value))
				return defaultValue;
			return value;
		}
	}
}
