using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using LocalDatabaseManager;

namespace Sportsman_Field
{
	public enum StatusType
	{
		Offline=0,
		Online
	}

	public enum BulbStatus
	{
		Off=0,
		On
	}
	
	/// <summary>
	/// Summary description for MainForm.
	/// </summary>
	public class MainForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.StatusBar statusBar1;
		private System.Windows.Forms.StatusBarPanel sbpStatus;
		private System.Windows.Forms.StatusBarPanel sbpDummyOnline;
		private System.Windows.Forms.ToolBar toolBar1;
		private System.Windows.Forms.ToolBarButton tbbExit;
		private System.Windows.Forms.ImageList imageList1;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.StatusBarPanel sbpSeason;
		private System.Windows.Forms.StatusBarPanel sbpUser;
		private System.Windows.Forms.ToolBarButton toolBarButton1;
		private System.Windows.Forms.ToolBarButton tbbImport;
		private System.Windows.Forms.ToolBarButton toolBarButton2;
		private System.Windows.Forms.ToolBarButton tbbExport;
		private System.Windows.Forms.ToolBarButton toolBarButton3;
		private System.Windows.Forms.ToolBarButton tbbManage;
		private System.Windows.Forms.ToolBarButton toolBarButton4;
		private System.Windows.Forms.ToolBarButton tbbSetResults;
		private System.Windows.Forms.ToolBarButton toolBarButton5;
		private System.Windows.Forms.ToolBarButton tbbTimer;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Label label1;


		private StatusType _statusType;
		private System.Windows.Forms.StatusBarPanel sbpVersion;
		private System.Windows.Forms.StatusBarPanel sbpDummyBulbOn;
		private System.Windows.Forms.StatusBarPanel sbpDummyBulbOff;
		private System.Windows.Forms.Timer timer1;
		public static readonly string IniFileName="sportman_field.INI";
		private BulbStatus _bulbStatus=BulbStatus.On;
		private Icon _iconBulbOff=null;
		private Icon _iconBulbOn=null;

		public MainForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(MainForm));
			this.panel1 = new System.Windows.Forms.Panel();
			this.label1 = new System.Windows.Forms.Label();
			this.button2 = new System.Windows.Forms.Button();
			this.button1 = new System.Windows.Forms.Button();
			this.statusBar1 = new System.Windows.Forms.StatusBar();
			this.sbpSeason = new System.Windows.Forms.StatusBarPanel();
			this.sbpUser = new System.Windows.Forms.StatusBarPanel();
			this.sbpStatus = new System.Windows.Forms.StatusBarPanel();
			this.sbpDummyOnline = new System.Windows.Forms.StatusBarPanel();
			this.toolBar1 = new System.Windows.Forms.ToolBar();
			this.tbbExit = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton1 = new System.Windows.Forms.ToolBarButton();
			this.tbbImport = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton2 = new System.Windows.Forms.ToolBarButton();
			this.tbbExport = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton3 = new System.Windows.Forms.ToolBarButton();
			this.tbbManage = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton4 = new System.Windows.Forms.ToolBarButton();
			this.tbbSetResults = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton5 = new System.Windows.Forms.ToolBarButton();
			this.tbbTimer = new System.Windows.Forms.ToolBarButton();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.sbpVersion = new System.Windows.Forms.StatusBarPanel();
			this.sbpDummyBulbOn = new System.Windows.Forms.StatusBarPanel();
			this.sbpDummyBulbOff = new System.Windows.Forms.StatusBarPanel();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.sbpSeason)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sbpUser)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sbpStatus)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sbpDummyOnline)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sbpVersion)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sbpDummyBulbOn)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sbpDummyBulbOff)).BeginInit();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel1.BackgroundImage")));
			this.panel1.Controls.Add(this.label1);
			this.panel1.Controls.Add(this.button2);
			this.panel1.Controls.Add(this.button1);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 45);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(794, 458);
			this.panel1.TabIndex = 6;
			// 
			// label1
			// 
			this.label1.BackColor = System.Drawing.Color.Transparent;
			this.label1.Location = new System.Drawing.Point(536, 40);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(120, 23);
			this.label1.TabIndex = 2;
			this.label1.Text = "label1";
			this.label1.Visible = false;
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(8, 104);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(88, 23);
			this.button2.TabIndex = 1;
			this.button2.Text = "Create Tables";
			this.button2.Visible = false;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(8, 64);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(72, 23);
			this.button1.TabIndex = 0;
			this.button1.Text = "Test";
			this.button1.Visible = false;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// statusBar1
			// 
			this.statusBar1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.statusBar1.Location = new System.Drawing.Point(0, 503);
			this.statusBar1.Name = "statusBar1";
			this.statusBar1.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
																						  this.sbpVersion,
																						  this.sbpSeason,
																						  this.sbpUser,
																						  this.sbpStatus,
																						  this.sbpDummyOnline,
																						  this.sbpDummyBulbOn,
																						  this.sbpDummyBulbOff});
			this.statusBar1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.statusBar1.ShowPanels = true;
			this.statusBar1.Size = new System.Drawing.Size(794, 22);
			this.statusBar1.SizingGrip = false;
			this.statusBar1.TabIndex = 5;
			this.statusBar1.Click += new System.EventHandler(this.statusBar1_Click);
			this.statusBar1.PanelClick += new System.Windows.Forms.StatusBarPanelClickEventHandler(this.statusBar1_PanelClick);
			// 
			// sbpSeason
			// 
			this.sbpSeason.Width = 250;
			// 
			// sbpUser
			// 
			this.sbpUser.Width = 250;
			// 
			// sbpStatus
			// 
			this.sbpStatus.Icon = ((System.Drawing.Icon)(resources.GetObject("sbpStatus.Icon")));
			this.sbpStatus.Text = "סטטוס: לא מקוון";
			this.sbpStatus.Width = 192;
			// 
			// sbpDummyOnline
			// 
			this.sbpDummyOnline.Icon = ((System.Drawing.Icon)(resources.GetObject("sbpDummyOnline.Icon")));
			this.sbpDummyOnline.MinWidth = 0;
			this.sbpDummyOnline.Width = 0;
			// 
			// toolBar1
			// 
			this.toolBar1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.toolBar1.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
																						this.tbbExit,
																						this.toolBarButton1,
																						this.tbbImport,
																						this.toolBarButton2,
																						this.tbbExport,
																						this.toolBarButton3,
																						this.tbbManage,
																						this.toolBarButton4,
																						this.tbbSetResults,
																						this.toolBarButton5,
																						this.tbbTimer});
			this.toolBar1.ButtonSize = new System.Drawing.Size(72, 38);
			this.toolBar1.DropDownArrows = true;
			this.toolBar1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.toolBar1.ImageList = this.imageList1;
			this.toolBar1.Location = new System.Drawing.Point(0, 0);
			this.toolBar1.Name = "toolBar1";
			this.toolBar1.ShowToolTips = true;
			this.toolBar1.Size = new System.Drawing.Size(794, 45);
			this.toolBar1.TabIndex = 4;
			this.toolBar1.TabStop = true;
			this.toolBar1.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.toolBar1_ButtonClick);
			// 
			// tbbExit
			// 
			this.tbbExit.ImageIndex = 0;
			this.tbbExit.Text = "יציאה";
			this.tbbExit.ToolTipText = "יציאה מהתוכנה";
			// 
			// toolBarButton1
			// 
			this.toolBarButton1.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			// 
			// tbbImport
			// 
			this.tbbImport.ImageIndex = 1;
			this.tbbImport.Text = "יבוא נתונים";
			this.tbbImport.ToolTipText = "קבלת נתוני אליפויות משרת הנתונים המרכזי";
			// 
			// toolBarButton2
			// 
			this.toolBarButton2.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			// 
			// tbbExport
			// 
			this.tbbExport.ImageIndex = 2;
			this.tbbExport.Text = "יצוא נתונים";
			this.tbbExport.ToolTipText = "עדכון שרת נתונים מרכזי בתוצאות האליפויות";
			// 
			// toolBarButton3
			// 
			this.toolBarButton3.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			// 
			// tbbManage
			// 
			this.tbbManage.ImageIndex = 3;
			this.tbbManage.Text = "כלי ניהול";
			this.tbbManage.ToolTipText = "ניהול מסד נתונים מקומי ומחיקת אליפויות";
			// 
			// toolBarButton4
			// 
			this.toolBarButton4.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			// 
			// tbbSetResults
			// 
			this.tbbSetResults.ImageIndex = 4;
			this.tbbSetResults.Text = "תוצאות";
			this.tbbSetResults.ToolTipText = "קביעת תוצאות משחקים ותחרויות";
			// 
			// toolBarButton5
			// 
			this.toolBarButton5.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			// 
			// tbbTimer
			// 
			this.tbbTimer.ImageIndex = 5;
			this.tbbTimer.Text = "שעון עצר";
			this.tbbTimer.ToolTipText = "מסך שעון עצר";
			// 
			// imageList1
			// 
			this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// sbpVersion
			// 
			this.sbpVersion.Text = "גרסה: ";
			this.sbpVersion.ToolTipText = "גרסה נוכחית של התוכנה";
			// 
			// sbpDummyBulbOn
			// 
			this.sbpDummyBulbOn.Icon = ((System.Drawing.Icon)(resources.GetObject("sbpDummyBulbOn.Icon")));
			this.sbpDummyBulbOn.MinWidth = 0;
			this.sbpDummyBulbOn.Width = 0;
			// 
			// sbpDummyBulbOff
			// 
			this.sbpDummyBulbOff.Icon = ((System.Drawing.Icon)(resources.GetObject("sbpDummyBulbOff.Icon")));
			this.sbpDummyBulbOff.MinWidth = 0;
			this.sbpDummyBulbOff.Width = 0;
			// 
			// timer1
			// 
			this.timer1.Interval = 1000;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// MainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(794, 525);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.statusBar1);
			this.Controls.Add(this.toolBar1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "MainForm";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "תוכנת שטח - התאחדות הספורט של בתי הספר";
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.sbpSeason)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sbpUser)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sbpStatus)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sbpDummyOnline)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sbpVersion)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sbpDummyBulbOn)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sbpDummyBulbOff)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		private void MainForm_Load(object sender, System.EventArgs e)
		{
			Dialogs.ChooseStatusDialog objDialog=new Dialogs.ChooseStatusDialog();
			if (objDialog.ShowDialog() != DialogResult.OK)
			{
				Application.Exit();
				return;
			}
			
			double currentVersion=Sport.Core.Data.Field_CurrentVersion;
			string strVersion=currentVersion.ToString();
			if (strVersion.IndexOf(".") < 0)
			{
				strVersion += ".0";
			}
			sbpVersion.Text = "גרסה: "+strVersion;
			
			_statusType = StatusType.Offline;
			if (objDialog.rbOnline.Checked)
				_statusType = StatusType.Online;

			if (_statusType == StatusType.Online)
			{
				System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(MainForm));
				this.sbpStatus.Icon = ((System.Drawing.Icon)(resources.GetObject("sbpDummyOnline.Icon")));
				this.sbpStatus.Text = "סטטוס: מקוון";
				sbpUser.Text = "משתמש: "+Core.UserManager.CurrentUser.Name;
				sbpUser.Text += " ("+Core.UserManager.CurrentUser.Region+")";
				sbpSeason.Text = "עונה: "+Core.Data.CurrentSeason.Name;
				
				tbbSetResults.Enabled = false;
				tbbTimer.Enabled = false;
				
				SessionServices.SessionService _service=
					new SessionServices.SessionService();
				double lastVersion=_service.GetLatestFieldVersion().VersionNumber;
				if (lastVersion > currentVersion)
				{
					Sport.UI.MessageBox.Success("גרסה חדשה זמינה להורדה באתר", "תוכנת שטח");
					sbpVersion.ToolTipText = "גרסה חדשה זמינה להורדה, לחץ להפעלת דפדפן האינטרנט";
					_iconBulbOn = ((System.Drawing.Icon)(resources.GetObject("sbpDummyBulbOn.Icon")));
					_iconBulbOff = ((System.Drawing.Icon)(resources.GetObject("sbpDummyBulbOff.Icon")));
					timer1.Enabled = true;
				}
			}
			else
			{
				tbbImport.Enabled = false;
				tbbExport.Enabled = false;
			}
			
			if (System.Configuration.ConfigurationSettings.AppSettings["ShowTestButton"] == "1")
				button1.Visible = true;
			
			if (System.Configuration.ConfigurationSettings.AppSettings["ShowCreateTablesButton"] == "1")
				button2.Visible = true;
		}

		private void toolBar1_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			if (e.Button == tbbExit)
			{
					Application.Exit();
					return;
			}

			if (e.Button == tbbImport)
			{
				Dialogs.ImportDialog _dialog=new Dialogs.ImportDialog();
				_dialog.ShowDialog();
				return;
			}
			
			if (e.Button == tbbExport)
			{
				Dialogs.ExportDialog _dialog=new Dialogs.ExportDialog();
				_dialog.ShowDialog();
				return;
			}

			if (e.Button == tbbManage)
			{
				Dialogs.ManageDialog _dialog=new Dialogs.ManageDialog(_statusType);
				_dialog.ShowDialog();
				return;
			}
			
			if (e.Button == tbbSetResults)
			{
				Dialogs.SetResultsDialog _dialog=new Dialogs.SetResultsDialog();
				_dialog.ShowDialog();
				return;
			}
		}

		private void button1_Click(object sender, System.EventArgs e)
		{
			TestDatabase objDialog=new TestDatabase();
			objDialog.ShowDialog();
		}

		private void button2_Click(object sender, System.EventArgs e)
		{
			Sport.UI.Dialogs.WaitForm.ShowWait("יוצר טבלאות אנא המתן...");
			try
			{
				LocalDatabase.CreateLocalTables();
			}
			catch (Exception err)
			{
				Sport.UI.Dialogs.WaitForm.HideWait();
				Sport.UI.MessageBox.Error("שגיאה:\n"+err.Message, "יצירת טבלאות");
				return;
			}
			Sport.UI.Dialogs.WaitForm.HideWait();
			Sport.UI.MessageBox.Success("טבלאות נתונים נוצרו בהצלחה", "יצירת טבלאות");
		}

		private void timer1_Tick(object sender, System.EventArgs e)
		{
			//System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(MainForm));
			switch (_bulbStatus)
			{
				case BulbStatus.On:
					sbpVersion.Icon = _iconBulbOff; //((System.Drawing.Icon)(resources.GetObject("sbpDummyBulbOff.Icon")));
					_bulbStatus = BulbStatus.Off;
					break;
				case BulbStatus.Off:
					sbpVersion.Icon = _iconBulbOn; //((System.Drawing.Icon)(resources.GetObject("sbpDummyBulbOn.Icon")));
					_bulbStatus = BulbStatus.On;
					break;
			}
		}

		private void statusBar1_Click(object sender, System.EventArgs e)
		{
		
		}

		private void statusBar1_PanelClick(object sender, System.Windows.Forms.StatusBarPanelClickEventArgs e)
		{
			if (e.StatusBarPanel == sbpVersion)
			{
				if (sbpVersion.ToolTipText.IndexOf("גרסה חדשה זמינה") >= 0)
				{
					System.Diagnostics.Process.Start(
						Sport.Core.Data.Field_ProgramUpdateURL);
				}
			}
		}
	}
}
