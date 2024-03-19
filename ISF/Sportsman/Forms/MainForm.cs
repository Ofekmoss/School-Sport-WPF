using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using Sport.UI;
using Sport.UI.Controls;
using Sport.UI.Display;
using Sportsman.Core;
using Sport.Entities;
using Sport.Data;
using System.Configuration;

namespace Sportsman.Forms
{
	/// <summary>
	/// Summary description for MainForm.
	/// </summary>
	public class MainForm : System.Windows.Forms.Form, ICommandTarget
	{
		//private UserLogin loginForm;

		private Panel viewsPanel = new Panel();
		private CaptionBar viewsCaptionBar = new CaptionBar();
		private ThemeButton viewsCloseButton = new ThemeButton();
		private ThemeButton viewsNextButton = new ThemeButton();
		private ThemeButton viewsPrevButton = new ThemeButton();
		private string invalidClockError = "הגדרות שעון אינן תקינות";

		private System.Windows.Forms.ImageList buttonImages;
		public Sport.UI.Controls.RightStatusBar isfStatusBar;
		private Forms.MessageBar messageBar;
		private System.Windows.Forms.Splitter messageBarSplitter;
		private Forms.CalendarBar calendarBar;
		private System.Windows.Forms.Splitter calendarBarSplitter;
		private System.Windows.Forms.PictureBox backgroundPicture;
		private System.ComponentModel.IContainer components;

		public MainForm()
		{
			Font = new Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular);
			MinimumSize = new Size(500, 300);

			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			SuspendLayout();

			this.Menu = (MainMenu)ConfigurationManager.GetSection("Sportsman/Menu");
			//System.Configuration.ConfigurationSettings.GetConfig("Sportsman/Menu");

			viewsPanel.Dock = DockStyle.Top;

			viewsCaptionBar.SelectionChanged += new System.EventHandler(CaptionSelectionChanged);
			viewsCaptionBar.Dock = DockStyle.Fill;
			viewsCaptionBar.TabStop = false;
			viewsPanel.Controls.Add(viewsCaptionBar);

			viewsPanel.Height = viewsCaptionBar.Height;

			viewsPrevButton.Dock = DockStyle.Left;
			viewsPrevButton.TabStop = false;
			viewsPrevButton.Size = new Size(viewsCaptionBar.Height + 2, 18);
			viewsPrevButton.Image = buttonImages.Images[1];
			viewsPrevButton.ImageAlign = ContentAlignment.MiddleCenter;
			viewsPrevButton.Click += new System.EventHandler(PrevViewClick);
			viewsPanel.Controls.Add(viewsPrevButton);

			viewsNextButton.Dock = DockStyle.Left;
			viewsNextButton.TabStop = false;
			viewsNextButton.Size = new Size(viewsCaptionBar.Height + 2, 18);
			viewsNextButton.Image = buttonImages.Images[2];
			viewsNextButton.Hue = 120f;
			viewsNextButton.Saturation = 0.5f;
			viewsNextButton.ImageAlign = ContentAlignment.MiddleCenter;
			viewsNextButton.Click += new System.EventHandler(NextViewClick);
			viewsPanel.Controls.Add(viewsNextButton);


			viewsCloseButton.Dock = DockStyle.Left;
			viewsCloseButton.TabStop = false;
			viewsCloseButton.Size = new Size(viewsCaptionBar.Height + 2, 18);
			viewsCloseButton.Image = buttonImages.Images[0];
			viewsCloseButton.Hue = 0f;
			viewsCloseButton.Saturation = 0.8f;
			viewsCloseButton.ImageAlign = ContentAlignment.MiddleCenter;
			viewsCloseButton.Click += new System.EventHandler(CloseViewClick);

			viewsPanel.Controls.Add(viewsCloseButton);

			viewsPanel.Visible = false;

			Controls.Add(viewsPanel);

			ViewManager.ViewOpened += new ViewEventHandler(ViewOpened);
			ViewManager.ViewClosed += new ViewEventHandler(ViewClosed);
			ViewManager.ViewSelected += new ViewEventHandler(ViewSelected);

			messageBar = new Forms.MessageBar();
			messageBar.Height = 100;
			messageBar.Visible = false;
			messageBar.Dock = DockStyle.Bottom;

			messageBarSplitter = new Splitter();
			messageBarSplitter.Dock = DockStyle.Bottom;
			messageBarSplitter.Visible = false;
			messageBarSplitter.BackColor = SystemColors.ControlLight;

			StateItem si = StateManager.States["messagebar"];
			if (si != null)
			{
				si.StateChanged += new EventHandler(MessageBarStateChanged);
				si.Checked = true;
				si.Visible = true;
			}

			Controls.Add(messageBarSplitter);
			Controls.Add(messageBar);

			calendarBar = new Forms.CalendarBar();
			calendarBar.Width = 200;
			calendarBar.Visible = false;
			calendarBar.Dock = DockStyle.Right;

			calendarBarSplitter = new Splitter();
			calendarBarSplitter.Dock = DockStyle.Right;
			calendarBarSplitter.Visible = false;
			calendarBarSplitter.BackColor = SystemColors.ControlLight;

			si = StateManager.States["calendarbar"];
			if (si != null)
			{
				if (Sport.UI.View.IsViewPermitted(typeof(Views.CalendarView)))
				{
					si.StateChanged += new EventHandler(CalendarBarStateChanged);
					si.Visible = true;
				}
				else
				{
					si.Visible = false;
				}
			}

			Controls.Add(calendarBarSplitter);
			Controls.Add(calendarBar);

			CreateStatusBar();

			this.Closing += new CancelEventHandler(OnClosing);

			string windowMaximized = Sport.Core.Configuration.ReadString("General", "MainWindowMaximized");
			if (windowMaximized == "1")
				this.WindowState = FormWindowState.Maximized;

			ResumeLayout(false);

			RegisterKeyCommands();

			KeyDown += new KeyEventHandler(MainForm_KeyDown);

			//ViewManager.OpenView(typeof(Views.CalendarView), null);
		}

		public void OpenInstantMessagesView(int recipientID)
		{
			//connected?
			if (!Sport.Core.Session.Connected)
				return;

			//build the state string and open the view:
			string strState = "recipient=" + recipientID.ToString() + "&new_entity=1";
			Sport.UI.ViewManager.OpenView(typeof(Views.InstantMessagesView), strState);
		}

		private void RegisterKeyCommand(string command, string name, Keys defaultKeys)
		{
			string keys = Sport.Core.Configuration.ReadString("Keyboard", command);
			Keys k;
			if (keys == null)
			{
				k = defaultKeys;
			}
			else
			{
				try
				{
					k = (Keys)Int32.Parse(keys);
				}
				catch
				{
					k = defaultKeys;
				}
			}

			KeyListener.RegisterCommand(command, name, k);
		}

		private void RegisterKeyCommands()
		{
			RegisterKeyCommand(Sport.Core.Data.KeyCommands.CloseView, "סגור מסך", Keys.F4 | Keys.Control);
			RegisterKeyCommand(Sport.Core.Data.KeyCommands.NextView, "מסך הבא", Keys.Tab | Keys.Control);
			RegisterKeyCommand(Sport.Core.Data.KeyCommands.PreviousView, "מסך קודם", Keys.Tab | Keys.Control | Keys.Shift);
			RegisterKeyCommand(Sport.Core.Data.KeyCommands.FilterTable, "חיתוך טבלה", Keys.F4);
			RegisterKeyCommand(Sport.Core.Data.KeyCommands.SearchTable, "חיפוש טבלה", Keys.F3);
			RegisterKeyCommand(Sport.Core.Data.KeyCommands.CustomizeTable, "התאמת טבלה", Keys.F6);
			RegisterKeyCommand(Sport.Core.Data.KeyCommands.Print, "הדפס", Keys.Control | Keys.P);
			RegisterKeyCommand(Sport.Core.Data.KeyCommands.SaveItem, "שמור", Keys.Control | Keys.S);
			RegisterKeyCommand(Sport.Core.Data.KeyCommands.Cancel, "בטל", Keys.Escape);
			RegisterKeyCommand(Sport.Core.Data.KeyCommands.EditItem, "ערוך", Keys.F2);
			RegisterKeyCommand(Sport.Core.Data.KeyCommands.MarkItem, "סמן", Keys.Control | Keys.Enter);
			RegisterKeyCommand(Sport.Core.Data.KeyCommands.DeleteItem, "מחק", Keys.Control | Keys.Delete);
			RegisterKeyCommand(Sport.Core.Data.KeyCommands.NewItem, "חדש", Keys.Control | Keys.N);
			RegisterKeyCommand(Sport.Core.Data.KeyCommands.ItemDetails, "פרטים", Keys.Control | Keys.F5);

			RegisterKeyCommand(KeyCommands.SchoolDetails, "פרטי בית ספר", Keys.F7);
			RegisterKeyCommand(KeyCommands.StudentDetails, "פרטי תלמיד", Keys.F8);
			RegisterKeyCommand(KeyCommands.ChampionshipDetails, "פרטי אליפות", Keys.F9);
		}

		#region ICommandTarget Members

		public bool ExecuteCommand(string command)
		{
			if (command == Sport.Core.Data.KeyCommands.CloseView)
			{
				ViewManager.CloseView(currentView);
			}
			else if (command == Sport.Core.Data.KeyCommands.NextView)
			{
				int v = ViewManager.SelectedViewIndex;
				int vc = ViewManager.ViewsCount();
				if (v == -1)
				{
					if (vc > 0)
						ViewManager.SelectView(0);
				}
				else
				{
					if (v == vc - 1)
						ViewManager.SelectView(0);
					else
						ViewManager.SelectView(v + 1);
				}
			}
			else if (command == Sport.Core.Data.KeyCommands.PreviousView)
			{
				int v = ViewManager.SelectedViewIndex;
				int vc = ViewManager.ViewsCount();
				if (v == -1)
				{
					if (vc > 0)
						ViewManager.SelectView(0);
				}
				else
				{
					if (v == 0)
						ViewManager.SelectView(vc - 1);
					else
						ViewManager.SelectView(v - 1);
				}
			}
			else if (command == KeyCommands.SchoolDetails)
			{
				new OpenDialogCommand().Execute("Sportsman.Details.SchoolDetailsView,Sportsman");
			}
			else if (command == KeyCommands.StudentDetails)
			{
				new OpenDialogCommand().Execute("Sportsman.Details.StudentDetailsView,Sportsman");
			}
			else if (command == KeyCommands.ChampionshipDetails)
			{
				new OpenDialogCommand().Execute("Sportsman.Details.ChampionshipDetailsView,Sportsman");
			}
			else
			{
				return false;
			}

			return true;
		}

		#endregion


		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.buttonImages = new System.Windows.Forms.ImageList(this.components);
			this.backgroundPicture = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.backgroundPicture)).BeginInit();
			this.SuspendLayout();
			// 
			// buttonImages
			// 
			this.buttonImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("buttonImages.ImageStream")));
			this.buttonImages.TransparentColor = System.Drawing.Color.Silver;
			this.buttonImages.Images.SetKeyName(0, "");
			this.buttonImages.Images.SetKeyName(1, "");
			this.buttonImages.Images.SetKeyName(2, "");
			// 
			// backgroundPicture
			// 
			this.backgroundPicture.Dock = System.Windows.Forms.DockStyle.Fill;
			this.backgroundPicture.Image = ((System.Drawing.Image)(resources.GetObject("backgroundPicture.Image")));
			this.backgroundPicture.Location = new System.Drawing.Point(0, 0);
			this.backgroundPicture.Name = "backgroundPicture";
			this.backgroundPicture.Size = new System.Drawing.Size(608, 454);
			this.backgroundPicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.backgroundPicture.TabIndex = 0;
			this.backgroundPicture.TabStop = false;
			// 
			// MainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(608, 454);
			this.Controls.Add(this.backgroundPicture);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "MainForm";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.Text = "תוכנת ניהול ספורט התאחדות בתי הספר";
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.MainForm_Paint);
			((System.ComponentModel.ISupportInitialize)(this.backgroundPicture)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		#region View Events

		public Sport.UI.View currentView;

		private void ViewOpened(ViewEventArgs e)
		{
			int index = e.Index;
			viewsCaptionBar.Items.Add(e.View.Title);
			viewsPanel.Visible = viewsCaptionBar.Items.Count > 0;
			backgroundPicture.Visible = !viewsPanel.Visible;
			e.View.TitleChanged += new System.EventHandler(TitleChanged);

			if (!Sport.UI.ViewManager.CheckClockSettings())
				Sportsman.Context.SetStatusBar(Forms.MainForm.StatusBarPanels.Error, invalidClockError);
		}

		private void ViewClosed(ViewEventArgs e)
		{
			if (e.Index >= 0 && e.Index < viewsCaptionBar.Items.Count)
			{
				viewsCaptionBar.Items.RemoveAt(e.Index);
			}
			viewsPanel.Visible = viewsCaptionBar.Items.Count > 0;
			backgroundPicture.Visible = !viewsPanel.Visible;
		}

		private void ViewSelected(ViewEventArgs e)
		{
			SuspendLayout();

			if (e.View != null)
			{
				Controls.Add(e.View);

				// Setting index to zero in order for
				// the docking to work.
				// For some reason the control that uses
				// DockStyle.Fill should be first.
				Controls.SetChildIndex(e.View, 0);
				Text = Sport.Core.Data.MainCaption + " - " + e.View.Title;
			}
			else
			{
				Text = Sport.Core.Data.MainCaption;
			}

			viewsCaptionBar.SelectedIndex = e.Index;

			if (currentView != null)
				Controls.Remove(currentView);


			currentView = e.View;

			if (currentView == null)
				this.Focus();
			else
				currentView.Focus();

			viewsNextButton.Enabled = ViewManager.HistoryCurrent < ViewManager.HistoryLast;
			viewsPrevButton.Enabled = ViewManager.HistoryCurrent > 0;

			ResumeLayout();
		}

		private void TitleChanged(object sender, System.EventArgs e)
		{
			if (sender is Sport.UI.View)
			{
				Sport.UI.View v = (Sport.UI.View)sender;
				int index = ViewManager.IndexOf(v);

				if (index >= 0 && v != null)
				{
					viewsCaptionBar.Items[index].Text = v.Title;

					if (viewsCaptionBar.SelectedIndex == index)
						Text = Sport.Core.Data.MainCaption + " - " + v.Title;
				}
			}
		}

		#endregion

		private void CaptionSelectionChanged(object sender, System.EventArgs e)
		{
			ViewManager.SelectView(viewsCaptionBar.SelectedIndex);
		}

		public void CloseViewClick(object sender, System.EventArgs e)
		{
			ViewManager.CloseView(currentView);
		}

		public void NextViewClick(object sender, System.EventArgs e)
		{
			ViewManager.NextView();
			viewsNextButton.Enabled = ViewManager.HistoryCurrent < ViewManager.HistoryLast;
			viewsPrevButton.Enabled = ViewManager.HistoryCurrent > 0;
		}

		public void PrevViewClick(object sender, System.EventArgs e)
		{
			ViewManager.PreviousView();
			viewsNextButton.Enabled = ViewManager.HistoryCurrent < ViewManager.HistoryLast;
			viewsPrevButton.Enabled = ViewManager.HistoryCurrent > 0;
		}

		private void MainForm_Load(object sender, System.EventArgs e)
		{
			RecreateHandle(); // The only thing I found that fix the menu 
			// items display

			Location = new System.Drawing.Point(
				(Screen.PrimaryScreen.Bounds.Width - Width) / 2,
				(Screen.PrimaryScreen.Bounds.Height - Height) / 2);

			string messageBarVisible = "0";
			string calendarBarVisible = "0";
			if (Sport.Core.Session.Connected)
			{
				messageBarVisible = Sport.Core.Configuration.ReadString("General", "MessageBarVisible");
				calendarBarVisible = Sport.Core.Configuration.ReadString("General", "CalendarBarVisible");
			}
			if (messageBarVisible != "1")
				StateManager.States["messagebar"].Checked = false;

			StateManager.States["calendarbar"].Checked = (calendarBarVisible == "1");

			if (Sport.Core.Session.Connected)
			{
				Sportsman.CheckNewMessages(null);
				CheckOfflineChampionships();
			}
			else
			{
				(new Producer.OpenChampionshipCommand()).Execute(null);
			}

			if (!Sport.UI.ViewManager.CheckClockSettings())
				Sportsman.Context.SetStatusBar(Forms.MainForm.StatusBarPanels.Error, invalidClockError);
		}

		private void CheckOfflineChampionships()
		{
			//get list of the offline championship with result:
			int recentFileIndex = -1;
			int[] arrOfflineChampID = Producer.OpenChampionshipCommand.GetOfflineChamionships(true, ref recentFileIndex);

			//got anything?
			if (arrOfflineChampID == null || arrOfflineChampID.Length == 0)
				return;

			//build array of drop down items.
			ArrayList arrChampItems = new ArrayList();
			foreach (int categoryID in arrOfflineChampID)
			{
				Sport.Entities.ChampionshipCategory category = null;
				try
				{
					category = new Sport.Entities.ChampionshipCategory(categoryID);
				}
				catch { }
				if (category != null && category.Championship != null)
				{
					Sport.Common.ListItem item = new Sport.Common.ListItem(
						category.Championship.Name + " " + category.Name, category.Id);
					arrChampItems.Add(item);
				}
			}

			//got anything?
			if (arrChampItems.Count == 0)
				return;

			//get array:
			Sport.Common.ListItem[] items = (Sport.Common.ListItem[])arrChampItems.ToArray(typeof(Sport.Common.ListItem));

			//recent index:
			if (recentFileIndex < 0 || recentFileIndex >= items.Length)
				recentFileIndex = 0;

			//create selection dialog:
			Sport.UI.Dialogs.GenericEditDialog objDialog = new Sport.UI.Dialogs.GenericEditDialog("ייבוא תוצאות אליפות");

			//add items:
			objDialog.Items.Add(new GenericItem("אליפות", GenericItemType.Selection,
				items[recentFileIndex], items));

			//attach listener:
			//objDialog.Items[0].ValueChanged += new EventHandler(OfflineChampionshipChanged);
			objDialog.Items[0].Nullable = false;

			//let user choose:
			if (objDialog.ShowDialog() == DialogResult.OK)
			{
				//get selected championship categoey:
				int categoryID = -1;
				if (objDialog.Items[0].Value != null)
				{
					categoryID = (int)(objDialog.Items[0].Value as Sport.Common.ListItem).Value;
				}

				//got anything?
				if (categoryID < 0)
					return;

				//build entity:
				ChampionshipCategory category = new ChampionshipCategory(categoryID);

				//open proper view.
				if (category.Championship.Sport.SportType == Sport.Types.SportType.Match)
				{
					ViewManager.OpenView(typeof(Producer.MatchChampionshipEditorView),
						"championshipcategory=" + categoryID.ToString());
				}
				else if (category.Championship.Sport.SportType == Sport.Types.SportType.Competition)
				{
					ViewManager.OpenView(typeof(Producer.CompetitionChampionshipEditorView),
						"championshipcategory=" + categoryID.ToString());
				}
				else
				{
					Sport.UI.MessageBox.Error("סוג התמודדות לא מזוהה", "טעינת אליפות");
				}
			}
		}

		private static void OfflineChampionshipChanged(object sender, EventArgs e)
		{

		}

		private void MainForm_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			//isfStatusBar.Visible = StateManager.States["statusbar"].Checked;
		}

		#region Statusbar
		public enum StatusBarPanels
		{
			User = 0,
			Region,
			Season,
			Messages,
			Version,
			Error
		}
		private System.EventHandler statusbarStateEvent;
		private void CreateStatusBar()
		{
			this.isfStatusBar = new Sport.UI.Controls.RightStatusBar();
			this.isfStatusBar.Dock = DockStyle.Bottom;

			string strUser = "";
			string strRegion = "";
			string strSeason = "";
			string strMessages = "";
			string strVersion = "";
			if (Sport.Core.Session.Connected)
			{
				strUser = "משתמש: " + UserManager.CurrentUser.Name;
				strRegion = Sport.Entities.Region.Type.Lookup(
					UserManager.CurrentUser.UserRegion).Name;
				strSeason = Sport.Entities.Season.Type.Lookup(
					Sport.Core.Session.Season).Name;
				strMessages = "אין הודעות חדשות";
				strVersion = Sport.Core.Data.CurrentVersion.ToString();
				if (Sport.Core.Data.BetaVersion)
					strVersion += " (beta)";
			}
			else
			{
				strUser = "עבודה במצב לא מקוון";

				strRegion = Sport.Core.Configuration.ReadString("LastRegion", "Name");
				strSeason = Sport.Core.Configuration.ReadString("LastSeason", "Name");
				strVersion = Sport.Core.Configuration.ReadString("General", "LastVersion");
			}

			isfStatusBar.Panels.Add(strUser);
			isfStatusBar.Panels.Add("מחוז: " + strRegion);
			isfStatusBar.Panels.Add("עונה: " + strSeason);
			isfStatusBar.Panels.Add(strMessages);
			isfStatusBar.Panels.Add("גרסה: " + strVersion);
			isfStatusBar.Panels.Add("");

			isfStatusBar.Panels[(int)StatusBarPanels.User].Alignment =
				HorizontalAlignment.Right;
			isfStatusBar.Panels[(int)StatusBarPanels.User].Width = 150;

			isfStatusBar.Panels[(int)StatusBarPanels.Region].Alignment =
				HorizontalAlignment.Right;
			isfStatusBar.Panels[(int)StatusBarPanels.Region].Width = 120;

			isfStatusBar.Panels[(int)StatusBarPanels.Season].Alignment =
				HorizontalAlignment.Right;
			isfStatusBar.Panels[(int)StatusBarPanels.Season].Width = 100;

			isfStatusBar.Panels[(int)StatusBarPanels.Messages].Alignment =
				HorizontalAlignment.Right;
			isfStatusBar.Panels[(int)StatusBarPanels.Messages].Width = 120;

			isfStatusBar.Panels[(int)StatusBarPanels.Version].Alignment =
				HorizontalAlignment.Right;
			isfStatusBar.Panels[(int)StatusBarPanels.Version].Width = 150;
			//isfStatusBar.Panels[(int)StatusBarPanels.Version].BorderStyle = StatusBarPanelBorderStyle.Raised;

			isfStatusBar.Panels[(int)StatusBarPanels.Error].Alignment =
				HorizontalAlignment.Right;
			isfStatusBar.Panels[(int)StatusBarPanels.Error].Width = 320;

			//isfStatusBar.Panels[(int) StatusBarPanels.Version].Icon = new Icon(Sport.Core.Data.IconFileName);

			isfStatusBar.PanelClick += new StatusBarPanelClickEventHandler(isfStatusBar_PanelClick);

			//isfStatusBar.Text = "hello world";
			isfStatusBar.ShowPanels = true;
			this.Controls.Add(this.isfStatusBar);

			//Sportsman.CheckNewMessages(null);

			StateItem si = StateManager.States["statusbar"];
			statusbarStateEvent = new EventHandler(StatusBarStateChanged);
			if (si != null)
			{
				si.Checked = true;
				si.Visible = true;
				si.StateChanged += new EventHandler(statusbarStateEvent);
			}
		}

		private void StatusBarStateChanged(object sender, System.EventArgs e)
		{
			if (sender is StateItem)
			{
				isfStatusBar.Visible = ((StateItem)sender).Checked;
			}
		}
		#endregion

		/// <summary>
		/// ask user to confirm exiting the application.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnClosing(object sender, CancelEventArgs e)
		{
			//cancel if user exit via the menu:
			if (Sportsman.IsClosing)
			{
				e.Cancel = true;
				Sportsman.IsClosing = false;
			}
			else
			{
				//let user confirm:
				int viewsCount = Sport.UI.ViewManager.ViewsCount();
				if (viewsCount > 0)
				{
					e.Cancel = !Sport.UI.MessageBox.Ask(
						Sportsman.ExitMessage.Replace("%views", viewsCount.ToString()),
						MessageBoxIcon.Question, false);
				}
				Sportsman.IsClosing = false;
			}
			if (!e.Cancel)
			{
				int visible = (Sport.UI.Display.StateManager.States["messagebar"].Checked) ? 1 : 0;
				int maximized = (this.WindowState == FormWindowState.Maximized) ? 1 : 0;
				int calendar = (Sport.UI.Display.StateManager.States["calendarbar"].Checked) ? 1 : 0;
				if (Sport.Core.Configuration.ReadString("General", "MessageBarVisible") != visible.ToString())
					Sport.Core.Configuration.WriteString("General", "MessageBarVisible", visible.ToString());
				if (Sport.Core.Configuration.ReadString("General", "MainWindowMaximized") != maximized.ToString())
					Sport.Core.Configuration.WriteString("General", "MainWindowMaximized", maximized.ToString());
				if (Sport.Core.Configuration.ReadString("General", "CalendarBarVisible") != calendar.ToString())
					Sport.Core.Configuration.WriteString("General", "CalendarBarVisible", calendar.ToString());
				int vv = 0;
				while (Sport.UI.ViewManager.ViewsCount() > 0)
				{
					if (vv > 99)
						break;
					Sport.UI.View view = Sport.UI.ViewManager.GetView(0);
					Sport.UI.ViewManager.CloseView(view);
					vv++;
				}
			}
		}

		private void MessageBarStateChanged(object sender, EventArgs e)
		{
			if (sender is StateItem)
			{
				messageBarSplitter.Visible = ((StateItem)sender).Checked;
				messageBar.Visible = ((StateItem)sender).Checked;
			}
		}

		private void CalendarBarStateChanged(object sender, EventArgs e)
		{
			if (sender is StateItem)
			{
				calendarBarSplitter.Visible = ((StateItem)sender).Checked;
				calendarBar.Visible = ((StateItem)sender).Checked;
			}
		}

		private void isfStatusBar_PanelClick(object sender, StatusBarPanelClickEventArgs e)
		{
			if (e.Clicks == 2)
			{
				if (e.StatusBarPanel == isfStatusBar.Panels[(int)StatusBarPanels.Messages])
				{
					if (Sport.Core.Session.Connected)
					{
						StateItem si = StateManager.States["messagebar"];
						si.Checked = true;
					}
				}

				if (e.StatusBarPanel == isfStatusBar.Panels[(int)StatusBarPanels.Version])
				{
					Commands.AboutCommand command = new Commands.AboutCommand();
					command.Execute(null);
				}

				if (e.StatusBarPanel == isfStatusBar.Panels[(int)StatusBarPanels.Version])
				{
					Commands.AboutCommand command = new Commands.AboutCommand();
					command.Execute(null);
				}

				if (e.StatusBarPanel == isfStatusBar.Panels[(int)StatusBarPanels.User])
				{
					if (String.Equals(Core.UserManager.CurrentUser.Username, "yahav"))
					{
						Sport.Core.Session.IsLogActive = !Sport.Core.Session.IsLogActive;
						string strToAppend = " (*)";
						string strCurText = Sportsman.Context.GetStatusText(Forms.MainForm.StatusBarPanels.User);
						string strNewText = (Sport.Core.Session.IsLogActive) ? strCurText + strToAppend : strCurText.Replace(strToAppend, "");
						Sportsman.Context.SetStatusBar(Forms.MainForm.StatusBarPanels.User, strNewText);
					}
				}

				if (e.StatusBarPanel == isfStatusBar.Panels[(int)StatusBarPanels.Error])
				{
					string sErrorText = isfStatusBar.Panels[(int)StatusBarPanels.Error].Text;
					if (string.Equals(sErrorText, invalidClockError))
						Sport.UI.ViewManager.ShowInvalidClockSettingsDialog(false);
				}
			}
		}

		private void MainForm_KeyDown(object sender, KeyEventArgs e)
		{
			if (KeyListener.HandleEvent(this, e))
				e.Handled = true;
		}
	}

	public class KeyCommands
	{
		public static readonly string SchoolDetails = "Details.School";
		public static readonly string StudentDetails = "Details.Student";
		public static readonly string ChampionshipDetails = "Details.Championship";
	}
}
