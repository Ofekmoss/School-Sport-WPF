using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Sportsman.Forms
{
	/// <summary>
	/// Summary description for UserLogin.
	/// </summary>
	public class UserLogin : System.Windows.Forms.Form
	{
		//internal static MainForm mainForm;
		//einat
		//1978
		
		private string _userName;
		private string _password;
		private int _season;
		private bool _shouldFocus;
		private System.Windows.Forms.TextBox edUserName;
		private System.Windows.Forms.TextBox edPassword;
		private System.Windows.Forms.Button btnLogin;
		private System.Windows.Forms.Button btnCancel;
		private Timer _focusTimer=null;
		private System.Windows.Forms.Label labelUser;
		private System.Windows.Forms.Label labelPassword;
		private System.Windows.Forms.Label labelSeason;
		private System.Windows.Forms.ComboBox cbSeason;
		private System.Windows.Forms.CheckBox chkDeleteDat;
		private System.Windows.Forms.Label lbInvalidTimeSettings;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public bool DeleteDatFiles
		{
			get {return chkDeleteDat.Checked;}
		}

		public UserLogin()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			_userName = "";
			_password = "";
			_season = -1;
			_shouldFocus = false;
			
			int seasonID=Sport.Common.Tools.CIntDef(
				Sport.Core.Configuration.ReadString("LastSeason", "ID"), -1);
			
			if (Sport.Core.Session.Connected)
			{
				//add the non-closed seasons: (where the season type is NOT closed)
				//Sport.Data.EntityFilterField seasonStatus = new Sport.Data.EntityFilterField(
				//	(int) Sport.Entities.Season.Fields.Status, Sport.Types.SeasonStatus.Closed, true);

				Sport.Data.Entity[] arrEntities = Sport.Entities.Season.Type.GetEntities(null); //new Sport.Data.EntityFilter(seasonStatus)
				ArrayList arrSeasons = new ArrayList();
				foreach (Sport.Data.Entity entSeason in arrEntities)
				{
					Sport.Entities.Season season = null;
					try
					{
						season = new Sport.Entities.Season(entSeason);
					}
					catch
					{}
					if (season != null)
						arrSeasons.Add(season.Entity);
				}
				
				arrSeasons.Sort(new SeasonComparer());
				cbSeason.Items.AddRange(arrSeasons.ToArray());
				
				if (seasonID >= 0)
				{
					for (int i=0; i<cbSeason.Items.Count; i++)
					{
						if ((cbSeason.Items[i] as Sport.Data.Entity).Id == seasonID)
						{
							cbSeason.SelectedIndex = i;
							break;
						}
					}
				}
				
				/*
				//current season is the active season... (last new season)
				Sport.Data.Entity currentSeason = null;
				foreach (Sport.Data.Entity entSeason in seasons)
				{
					Sport.Entities.Season season=new Sport.Entities.Season(entSeason);
					if (season.Status == Sport.Types.SeasonStatus.New)
						currentSeason = entSeason;
				}
				
				cbSeason.SelectedItem = currentSeason;
				*/
			}
			else
			{
				if (seasonID >= 0)
				{
					string seasonName=Sport.Core.Configuration.ReadString(
						"LastSeason", "Name");
					cbSeason.Items.Add(new Sport.Common.ListItem(seasonName, seasonID));
				}
			}
			
			if ((cbSeason.SelectedIndex < 0)&&(cbSeason.Items.Count > 0))
				cbSeason.SelectedIndex = cbSeason.Items.Count-1;
		}

		private string GetSql(System.Data.SqlClient.SqlCommand command)
		{
			string result=command.CommandText;
			for (int i=0; i<command.Parameters.Count; i++)
			{
				System.Data.SqlClient.SqlParameter param=command.Parameters[i];
				result = result.Replace(param.ParameterName, param.Value.ToString());
			}
			return result;
		}

		#region Public Properties
		public string Username
		{
			get {return _userName;}
			set
			{
				_userName = value;
				edUserName.Text = value;
			}
		}
		
		public string Password
		{
			get {return _password;}
			set {
				_password = value;
				edPassword.Text = value;
			}
		}

		public int Season
		{
			get { return _season; }
			set
			{
				for (int i=0; i<cbSeason.Items.Count; i++)
				{
					if ((cbSeason.Items[i] as Sport.Data.Entity).Id == value)
					{
						_season = value;
						cbSeason.SelectedIndex = i;
						break;
					}
				}
			}
		}
		
		public string GetSeasonName()
		{
			object selItem=cbSeason.SelectedItem;
			if (selItem is Sport.Data.Entity)
				return (selItem as Sport.Data.Entity).Name;
			else if (selItem is Sport.Common.ListItem)
				return (selItem as Sport.Common.ListItem).Text;
			return "";
		}
		
		public bool ShouldFocus
		{
			get {return _shouldFocus;}
			set {_shouldFocus = value;}
		}

		public bool AutomaticLogin=false;
		#endregion
		
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

		private void ResetValues()
		{
			_userName = "";
			_password = "";
			_season = -1;
		}

		private void StoreValues()
		{
			_userName = edUserName.Text;
			_password = edPassword.Text;
			if (Sport.Core.Session.Connected)
			{
				Sport.Data.Entity season = cbSeason.SelectedItem as Sport.Data.Entity;
				_season = season == null ? -1 : season.Id;
			}
			else
			{
				Sport.Common.ListItem season = cbSeason.SelectedItem as Sport.Common.ListItem;
				_season = (season == null)?(-1):((int) season.Value);
			}
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserLogin));
			this.labelUser = new System.Windows.Forms.Label();
			this.labelPassword = new System.Windows.Forms.Label();
			this.edUserName = new System.Windows.Forms.TextBox();
			this.edPassword = new System.Windows.Forms.TextBox();
			this.btnLogin = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.labelSeason = new System.Windows.Forms.Label();
			this.cbSeason = new System.Windows.Forms.ComboBox();
			this.chkDeleteDat = new System.Windows.Forms.CheckBox();
			this.lbInvalidTimeSettings = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// labelUser
			// 
			this.labelUser.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.labelUser.ForeColor = System.Drawing.Color.Blue;
			this.labelUser.Location = new System.Drawing.Point(176, 30);
			this.labelUser.Name = "labelUser";
			this.labelUser.Size = new System.Drawing.Size(104, 16);
			this.labelUser.TabIndex = 0;
			this.labelUser.Text = "זיהוי משתמש:";
			// 
			// labelPassword
			// 
			this.labelPassword.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.labelPassword.ForeColor = System.Drawing.Color.Blue;
			this.labelPassword.Location = new System.Drawing.Point(176, 64);
			this.labelPassword.Name = "labelPassword";
			this.labelPassword.Size = new System.Drawing.Size(104, 16);
			this.labelPassword.TabIndex = 1;
			this.labelPassword.Text = "סיסמא:";
			// 
			// edUserName
			// 
			this.edUserName.Location = new System.Drawing.Point(16, 25);
			this.edUserName.Name = "edUserName";
			this.edUserName.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.edUserName.Size = new System.Drawing.Size(144, 21);
			this.edUserName.TabIndex = 1;
			// 
			// edPassword
			// 
			this.edPassword.Location = new System.Drawing.Point(16, 59);
			this.edPassword.Name = "edPassword";
			this.edPassword.PasswordChar = '*';
			this.edPassword.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.edPassword.Size = new System.Drawing.Size(144, 21);
			this.edPassword.TabIndex = 2;
			// 
			// btnLogin
			// 
			this.btnLogin.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.btnLogin.ForeColor = System.Drawing.Color.Blue;
			this.btnLogin.Location = new System.Drawing.Point(16, 136);
			this.btnLogin.Name = "btnLogin";
			this.btnLogin.Size = new System.Drawing.Size(112, 23);
			this.btnLogin.TabIndex = 4;
			this.btnLogin.Text = "כניסה למערכת";
			this.btnLogin.Click += new System.EventHandler(this.button1_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.btnCancel.ForeColor = System.Drawing.Color.Red;
			this.btnCancel.Location = new System.Drawing.Point(168, 136);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(112, 23);
			this.btnCancel.TabIndex = 5;
			this.btnCancel.Text = "ביטול כניסה";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// labelSeason
			// 
			this.labelSeason.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.labelSeason.ForeColor = System.Drawing.Color.Blue;
			this.labelSeason.Location = new System.Drawing.Point(176, 98);
			this.labelSeason.Name = "labelSeason";
			this.labelSeason.Size = new System.Drawing.Size(104, 16);
			this.labelSeason.TabIndex = 6;
			this.labelSeason.Text = "עונה:";
			// 
			// cbSeason
			// 
			this.cbSeason.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbSeason.Location = new System.Drawing.Point(16, 93);
			this.cbSeason.Name = "cbSeason";
			this.cbSeason.Size = new System.Drawing.Size(144, 21);
			this.cbSeason.TabIndex = 3;
			// 
			// chkDeleteDat
			// 
			this.chkDeleteDat.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.chkDeleteDat.Location = new System.Drawing.Point(184, 168);
			this.chkDeleteDat.Name = "chkDeleteDat";
			this.chkDeleteDat.Size = new System.Drawing.Size(104, 24);
			this.chkDeleteDat.TabIndex = 7;
			this.chkDeleteDat.Text = "מחק קבצים זמניים";
			// 
			// lbInvalidTimeSettings
			// 
			this.lbInvalidTimeSettings.AutoSize = true;
			this.lbInvalidTimeSettings.Cursor = System.Windows.Forms.Cursors.Hand;
			this.lbInvalidTimeSettings.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.lbInvalidTimeSettings.ForeColor = System.Drawing.Color.Red;
			this.lbInvalidTimeSettings.Location = new System.Drawing.Point(16, 176);
			this.lbInvalidTimeSettings.Name = "lbInvalidTimeSettings";
			this.lbInvalidTimeSettings.Size = new System.Drawing.Size(158, 13);
			this.lbInvalidTimeSettings.TabIndex = 8;
			this.lbInvalidTimeSettings.Text = "הגדרות השעון אינן תקינות!";
			this.lbInvalidTimeSettings.Visible = false;
			this.lbInvalidTimeSettings.Click += new System.EventHandler(this.lbInvalidTimeSettings_Click);
			// 
			// UserLogin
			// 
			this.AcceptButton = this.btnLogin;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(302, 219);
			this.Controls.Add(this.lbInvalidTimeSettings);
			this.Controls.Add(this.edPassword);
			this.Controls.Add(this.edUserName);
			this.Controls.Add(this.chkDeleteDat);
			this.Controls.Add(this.cbSeason);
			this.Controls.Add(this.labelSeason);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnLogin);
			this.Controls.Add(this.labelPassword);
			this.Controls.Add(this.labelUser);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "UserLogin";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "רישום משתמש למערכת";
			this.Activated += new System.EventHandler(this.UserLogin_Activated);
			this.Load += new System.EventHandler(this.UserLogin_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private void button1_Click(object sender, System.EventArgs e)
		{
			StoreValues();
			if (this.Username.Length == 0)
			{
				MessageBox.Show(this, "אנא הכנס זיהוי משתמש", "שגיאת מערכת", 
								MessageBoxButtons.OK, MessageBoxIcon.Error);
				FocusAndSelect();
				return;
			}
			
			if (this.cbSeason.SelectedIndex < 0)
			{
				MessageBox.Show(this, "אנא בחר עונה", "שגיאת מערכת", 
					MessageBoxButtons.OK, MessageBoxIcon.Error);
				cbSeason.Focus();
				return;
			}
			
			if (lbInvalidTimeSettings.Visible)
			{
				/*
				if (Sport.UI.ViewManager.ShowInvalidClockSettingsDialog(true) != DialogResult.OK)
				{
					return;
				}
				*/
			}

			Sport.Core.Configuration.WriteString("Login", "Language", InputLanguage.CurrentInputLanguage.Culture.Name);
			if (Sport.Core.Session.Connected)
				Sport.Core.Configuration.WriteString("LastSeason", "ID", (cbSeason.Items[cbSeason.SelectedIndex] as Sport.Data.Entity).Id.ToString());
			
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			ResetValues();
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}
		
		private void FocusAndSelect()
		{
			bool b;
			b = edUserName.Focus();
			edUserName.Select(0, edUserName.Text.Length);
			_shouldFocus = false;
			_focusTimer.Stop();
		}

		private void UserLogin_Load(object sender, System.EventArgs e)
		{
			string culture = Sport.Core.Configuration.ReadString("Login", "Language");
			InputLanguage.CurrentInputLanguage = 
				InputLanguage.FromCulture(new System.Globalization.CultureInfo(culture == null ? "he-IL" : culture));

			if (AutomaticLogin)
			{
				button1_Click(sender, e);
				return;
			}

			if (_shouldFocus)
			{
				if (_focusTimer == null)
				{
					_focusTimer = new Timer();
					_focusTimer.Interval = 100;
					_focusTimer.Tick += new EventHandler(TimerEventProcessor);
				}
				_focusTimer.Start();
			}

			if (!Sport.Core.Session.Connected)
				this.Text += " - מצב לא מקוון";

			if (!Sport.UI.ViewManager.CheckClockSettings())
				lbInvalidTimeSettings.Visible = true;
		}

		private void UserLogin_Activated(object sender, System.EventArgs e)
		{

		}

		// This is the method to run when the timer is raised.
		private void TimerEventProcessor(Object myObject, EventArgs myEventArgs) 
		{
			FocusAndSelect();
		}
		
		private void lbInvalidTimeSettings_Click(object sender, System.EventArgs e)
		{
			Sport.UI.ViewManager.ShowInvalidClockSettingsDialog(false);
		}

		private class SeasonComparer : IComparer
		{
			public int Compare(object x, object y)
			{
				Sport.Data.Entity s1 = (Sport.Data.Entity) x;
				Sport.Data.Entity s2 = (Sport.Data.Entity) y;
				return s1.Id.CompareTo(s2.Id);
			}
		}

	}
}
