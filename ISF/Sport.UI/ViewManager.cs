using System;
using System.Linq;
using System.Collections;
using Microsoft.Win32;
using System.Windows.Forms;
using System.Collections.Generic;
using Sport.Synchronization;

namespace Sport.UI
{
	#region View Events

	/// <summary>
	/// ViewEventArgs is passed to events of the view manager
	/// (open, close and select) giving them the view object
	/// and index
	/// </summary>
	public class ViewEventArgs : EventArgs
	{
		private readonly View _view;
		private readonly int _index;

		public ViewEventArgs(View view, int index)
		{
			_view = view;
			_index = index;
		}

		public View View
		{
			get { return _view; }
		}
		public int Index
		{
			get { return _index; }
		}
	}

	// Handler delegate for view events
	public delegate void ViewEventHandler(ViewEventArgs e);

	#endregion

	/// <summary>
	/// Summary description for ViewManager.
	/// </summary>
	public class ViewManager
	{
		// Stores all opened views
		private static List<View> views = null;

		static ViewManager()
		{
			views = new List<View>();
			history = new List<View>();
			current = -1;

		}

		public static View GetView(int index)
		{
			return views[index];
		}


		public static int ViewsCount()
		{
			return (views == null) ? 0 : views.Count;
		}

		#region View Selection

		public static View SelectedView
		{
			get
			{
				return current == -1 ? null : history[current];
			}
			set
			{
				SelectView(value);
			}
		}

		public static int SelectedViewIndex
		{
			get
			{
				View view = SelectedView;
				if (view == null)
					return -1;

				return views.IndexOf(view);
			}
			set
			{
				SelectView(value);
			}
		}

		/// <summary>
		/// SelectView display the view in position index
		/// on the main form and stores the selection history.
		/// </summary>
		public static void SelectView(int index)
		{
			if (index >= 0 && index < views.Count)
			{
				// The view to be displayed
				View view = views[index];
				// The view currently displayed
				View currentView = current == -1 ? null : history[current];

				if (view != currentView)
				{
					if (currentView != null)
						currentView.Deactivate();

					// Deleting forward history
					if (current < history.Count - 1)
					{
						history.RemoveRange(current + 1,
							history.Count - current - 1);
					}

					history.Add(view);
					current = history.Count - 1;

					OnViewSelected(new ViewEventArgs(view, index));

					view.Activate();
				}
			}
		}

		/// <summary>
		/// SelectView display the view given
		/// on the main form and stores the selection history.
		/// </summary>
		public static void SelectView(View view)
		{
			int index = views.IndexOf(view);
			if (index >= 0)
				SelectView(index);
		}

		#endregion

		#region View History

		// Stores history of views selection
		private static List<View> history = null;
		// Stores the current position in the history array
		private static int current;

		// Returns the current index in history.
		public static int HistoryCurrent
		{
			get { return current; }
		}
		// Returns the last index in history
		public static int HistoryLast
		{
			get { return history.Count - 1; }
		}


		/// <summary>
		/// PreviousView selects (SelectView) the previous
		/// view in the history list.
		/// </summary>
		public static bool PreviousView()
		{
			if (current <= 0)
				return false;

			// The view currently displayed
			View currentView = current == -1 ? null : history[current];
			// The view to be displayed
			View view = history[current - 1];

			if (view != currentView)
			{
				if (currentView != null)
					currentView.Deactivate();

				OnViewSelected(new ViewEventArgs(view, IndexOf(view)));

				view.Activate();

				current--;
			}

			return true;
		}

		/// <summary>
		/// NextView selects (SelectView) the next
		/// view in the history list.
		/// </summary>
		public static bool NextView()
		{
			//if (current >= history.Count - 1)
			//	return false;

			// The view currently displayed
			View currentView = current == -1 ? null : history[current];
			// The view to be displayed
			View view;
			if (current == history.Count - 1)
			{
				view = history[0];
				current = 0;
			}
			else
			{
				view = history[current + 1];
				current++;
			}

			if (view != currentView)
			{
				if (currentView != null)
					currentView.Deactivate();

				OnViewSelected(new ViewEventArgs(view, IndexOf(view)));

				view.Activate();
			}
			return true;
		}

		#endregion

		#region View Open and Close

		/// <summary>
		/// OpenView creates a new view be parsing the view string,
		/// adds it to the views list and selects it.
		/// 
		/// The form of the view string is "class,state"
		/// where class is the name of a class that inherits UI.View
		/// and state is a list of parameters in the form
		/// of "param1=value1&param2=value2&..."
		/// </summary>
		public static int OpenView(string view)
		{
			string[] param = view.Split(new char[] { '?' });

			if (param.Length == 0)
				throw new Exception("Invalid view string");

			// Search for the given class in the entry assembly
			Type type = Type.GetType(param[0]);

			if (type == null)
				throw new Exception("Failed to find view type");

			return OpenView(type, param.Length > 1 ? param[1] : null);
		}

		/// <summary>
		/// OpenView creates a new view be parsing the view string,
		/// adds it to the views list and selects it.
		/// 
		/// The form of the view string is "class,state"
		/// where class is the name of a class that inherits UI.View
		/// and state is a list of parameters in the form
		/// of "param1=value1&param2=value2&..."
		/// </summary>
		public static int OpenView(Type viewType, string state)
		{
			SynchronizeManager.Initialize();

			// Creating View State for the state
			ParameterList vs = state == null ? new ParameterList() : new ParameterList(state);

			// Searching for an existing open view
			// of the same type and state
			int index = FindView(viewType, vs);

			if (index >= 0)
			{
				// This view is already opened...
				// selecting it and returning
				SelectView(index);
				return index;
			}

			// Creating the view given by the class name
			object o = null;
			try
			{
				o = Activator.CreateInstance(viewType);
			}
			catch
			{ }

			if (o == null)
				return -1;

			// Checking if it inherits UI.View
			if (o is UI.View)
			{
				View v = (View)o;
				v.State = vs;

				// Adding view, raising view opened event
				views.Add(v);
				int ind = views.Count - 1;
				Sport.UI.Dialogs.WaitForm.ShowWait("טוען נתונים אנא המתן...", true);
				bool blnSuccess = false;
				try
				{
					v.Open();
					blnSuccess = true;
				}
				catch (Exception ex)
				{
					Sport.UI.Dialogs.WaitForm.HideWait();
					Sport.UI.MessageBox.Error("שגיאה כללית בעת טעינת נתונים. אנא נסה שנית אם שגיאה זו חוזרת אנא צא מהתוכנה והפעל מחדש", "שגיאה כללית");
					Sport.Data.AdvancedTools.ReportExcpetion(ex, "Error while opening view " + v.Name);
				}

				if (blnSuccess)
				{
					Sport.UI.Dialogs.WaitForm.HideWait();
					OnViewOpened(new ViewEventArgs(v, ind));
					SynchronizeManager.Add(v);

					// Selecting the added view
					SelectView(ind);
					return ind;
				}
			}

			return -1;
		}

		/// <summary>
		/// CloseView unselect the view in the given index position and
		/// removes it from the views list and the history list.
		/// </summary>
		public static void CloseView(int index)
		{
			View view = views[index];

			// Asking the view if it can be closed
			if (!view.Closing())
				return;

			// Checking if view is active and deactivating it if it is
			View currentView = current == -1 ? null : history[current];
			if (currentView == view)
				view.Deactivate();

			int i = history.IndexOf(view);
			while (i >= 0)
			{
				history.RemoveAt(i);

				if (i <= current)
					current--;

				i = history.IndexOf(view);
			}

			SynchronizeManager.Remove(view);

			OnViewClosed(new ViewEventArgs(view, index));
			view.Close();
			view.Dispose();

			if (index >= 0 && index < views.Count)
				views.RemoveAt(index);

			// The view currently displayed
			currentView = current == -1 ?
				(history.Count == 0 ? null : history[0]) : history[current];
			// The view index
			index = currentView == null ? -1 : views.IndexOf(currentView);

			OnViewSelected(new ViewEventArgs(currentView, index));
		}

		/// <summary>
		/// CloseView unselect the given view and
		/// removes it from the views list and the history list.
		/// </summary>
		public static void CloseView(View view)
		{
			int index = views.IndexOf(view);
			if (index >= 0)
				CloseView(index);
		}

		#endregion

		#region View Locate

		/// <summary>
		/// FindView search for the index of a view in
		/// the views list having the same type and state as given.
		/// 
		/// type is the view's class
		/// state is a list of parameters in the view (usualy filter)
		/// </summary>
		public static int FindView(Type type, ParameterList state)
		{
			for (int i = 0; i < views.Count; i++)
			{
				if (views[i].GetType() == type)
				{
					if (state == null || views[i].State.Equals(state))
						return i;
				}
			}

			return -1;
		}

		/// <summary>
		/// IndexOf returns the index of a view in the ViewManager
		/// views array.
		/// </summary>
		public static int IndexOf(View view)
		{
			return views.IndexOf(view);
		}

		#endregion

		#region Events

		public static event ViewEventHandler ViewOpened;
		public static event ViewEventHandler ViewClosed;
		public static event ViewEventHandler ViewSelected;

		private static void OnViewOpened(ViewEventArgs e)
		{
			//maybe need to cancel?
			if (e.View != null)
			{
				if (e.View.GetType().FullName.IndexOf("ChampionshipEditorView") >= 0)
				{
					/*
					if (!ViewManager.CheckClockSettings())
					{
						if (ViewManager.ShowInvalidClockSettingsDialog(true) != DialogResult.OK)
						{
							CloseView(e.View);
							return;
						}
					}
					*/
				}
			}
			if (ViewOpened != null)
			{
				ViewOpened(e);
			}
		}

		private static void OnViewClosed(ViewEventArgs e)
		{
			if (ViewClosed != null)
			{
				ViewClosed(e);
			}
		}

		private static void OnViewSelected(ViewEventArgs e)
		{
			if (ViewSelected != null)
			{
				ViewSelected(e);
			}
		}

		#endregion

		public static bool CheckClockSettings()
		{
			try
			{
				TimeZone localZone = TimeZone.CurrentTimeZone;
				DateTime currentDate = DateTime.Now;
				RegistryKey rkHKLM = Registry.LocalMachine;

				Registry.LocalMachine.Flush();
				RegistryKey rkRun = rkHKLM.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\W32Time\Parameters", true);
				//if (((localZone.DaylightName == localZone.StandardName) && (localZone.StandardName == "Jerusalem Standard Time") && ((TimeZone.CurrentTimeZone.GetUtcOffset(currentDate).ToString()) == "02:00:00"))
				if (localZone.IsDaylightSavingTime(currentDate) == false) //&& (rkRun.GetValue("Type").ToString() == "NoSync")
				{
					foreach (string regName in rkRun.GetValueNames())
					{
						if (regName == "Type")
						{
							if (rkRun.GetValue("Type").ToString() != "NoSync") /*Automaticaly synchronize with an internet time server*/
							{
								return false;
							}
						}
					}
				}
				else
				{
					return false;
				}
			}
			catch
			{
				return true;
			}

			return true;
		}

		public static DialogResult ShowInvalidClockSettingsDialog(bool blnAskConfirmation)
		{
			string strText = "הגדרות השעון במחשבך אינן תואמות לתוכנת ניהול הספורט.\nעל מנת לשנות הגדרות אלו יש לקרוא את המסמך שנשלח על ידי ההתאחדות בנושא שעון קיץ.";
			if (blnAskConfirmation)
				strText += "\nהמשך עבודה עם הגדרות אלו עלול לגרום לתקלות עם זמני משחקים. האם להמשיך?";
			string strCaption = "הגדרות שעון אינן תקינות";
			if (blnAskConfirmation)
			{
				return System.Windows.Forms.MessageBox.Show(strText, strCaption, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
			}
			else
			{
				Sport.UI.MessageBox.Show(strText, strCaption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return DialogResult.None;
			}
		}
	}
}
