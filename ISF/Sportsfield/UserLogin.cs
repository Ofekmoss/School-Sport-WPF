using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Sport.Common;
using Sport.Core;

namespace Sportsfield
{
	/// <summary>
	/// Summary description for UserLogin.
	/// </summary>
	public class UserLogin : System.Windows.Forms.Form
	{
		//internal static MainForm mainForm;
		
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
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

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
			
			//add the non-closed seasons: (where the season type is NOT closed)
			Sport.Data.EntityFilterField seasonStatus = new Sport.Data.EntityFilterField(
				(int) Sport.Entities.Season.Fields.Status, Sport.Types.SeasonStatus.Closed, true);

			Sport.Data.Entity[] seasons = Sport.Entities.Season.Type.GetEntities(
				new Sport.Data.EntityFilter(seasonStatus));

			cbSeason.Items.AddRange(seasons);

			//current season is the active season... (last new season)
			Sport.Data.Entity currentSeason = null;
			foreach (Sport.Data.Entity entSeason in seasons)
			{
				Sport.Entities.Season season=new Sport.Entities.Season(entSeason);
				if (season.Status == Sport.Types.SeasonStatus.New)
					currentSeason = entSeason;
			}

			cbSeason.SelectedItem = currentSeason;
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
			set { _season = value; }
		}


		public bool ShouldFocus
		{
			get {return _shouldFocus;}
			set {_shouldFocus = value;}
		}

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
			Sport.Data.Entity season = cbSeason.SelectedItem as Sport.Data.Entity;
			_season = season == null ? -1 : season.Id;
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.labelUser = new System.Windows.Forms.Label();
			this.labelPassword = new System.Windows.Forms.Label();
			this.edUserName = new System.Windows.Forms.TextBox();
			this.edPassword = new System.Windows.Forms.TextBox();
			this.btnLogin = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.labelSeason = new System.Windows.Forms.Label();
			this.cbSeason = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// labelUser
			// 
			this.labelUser.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.labelUser.ForeColor = System.Drawing.Color.Blue;
			this.labelUser.Location = new System.Drawing.Point(176, 30);
			this.labelUser.Name = "labelUser";
			this.labelUser.Size = new System.Drawing.Size(104, 16);
			this.labelUser.TabIndex = 0;
			this.labelUser.Text = "זיהוי משתמש:";
			// 
			// labelPassword
			// 
			this.labelPassword.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
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
			this.edUserName.Text = "";
			// 
			// edPassword
			// 
			this.edPassword.Location = new System.Drawing.Point(16, 59);
			this.edPassword.Name = "edPassword";
			this.edPassword.PasswordChar = '*';
			this.edPassword.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.edPassword.Size = new System.Drawing.Size(144, 21);
			this.edPassword.TabIndex = 2;
			this.edPassword.Text = "";
			// 
			// btnLogin
			// 
			this.btnLogin.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
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
			this.btnCancel.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
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
			this.labelSeason.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
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
			// UserLogin
			// 
			this.AcceptButton = this.btnLogin;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(298, 176);
			this.Controls.Add(this.cbSeason);
			this.Controls.Add(this.labelSeason);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnLogin);
			this.Controls.Add(this.edPassword);
			this.Controls.Add(this.edUserName);
			this.Controls.Add(this.labelPassword);
			this.Controls.Add(this.labelUser);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "UserLogin";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "רישום משתמש למערכת";
			this.Load += new System.EventHandler(this.UserLogin_Load);
			this.ResumeLayout(false);

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

			Sport.Core.Configuration.WriteString("Login", "Language", InputLanguage.CurrentInputLanguage.Culture.Name);
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
		}

		// This is the method to run when the timer is raised.
		private void TimerEventProcessor(Object myObject, EventArgs myEventArgs) 
		{
			FocusAndSelect();
		}

		private static SessionServices.SessionService _service;
		private static UserData _currentUser;

		public static bool PerformLogin()
		{
			bool bLogin = false;
			try
			{
				UserLogin loginForm = new UserLogin();
			
				if (_service == null)
				{
					_service = new SessionServices.SessionService();
					_service.CookieContainer = Sport.Core.Session.Cookies;
				}

				//show login dialog until either the user log in successfully or cancels.
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
					string username=loginForm.Username;
					string password=Crypto.Encode(loginForm.Password);
					int season = loginForm.Season;
					//MessageBox.Show(season.ToString());
					username += "#"+Sport.Core.Data.CurrentVersion.ToString();

					//trigger login method of the web service:
					SessionServices.UserData user=_service.Login(
						username, password, season);
				
					//dataset will be null if user if not verified.
					UserData userData=null;
				
					//check result of web service login:
					if ((user.Username != null)&&(user.Username.Length > 0))
					{
						//user is verified, get user data:
						userData = new UserData();
						userData.Id = user.Id;
						userData.Username = user.Username;
						userData.Name =  user.UserFullName;
						userData.Permissions = user.UserPermissions;
						userData.UserType = user.UserType;
						userData.UserRegion = user.UserRegion;
						userData.UserSchool = user.UserSchool;
						userData.UserPassword = password;
					}
					if (userData != null)
					{
						if (userData.UserType == (int) Sport.Types.UserType.External)
						{
							Sport.UI.MessageBox.Error("אינך מורשה להשתמש בתוכנה זו", 
								"שגיאת מערכת");
						}
						else
						{
							_currentUser = userData;
							Sport.Core.Session.Region = userData.UserRegion;
							Sport.Core.Session.Season = season;
							Sport.Core.Session.User = userData;

							string lastSession=Sport.Common.Tools.CStrDef(
								Sport.Core.Configuration.ReadString("General", 
								"LastSeason"), "");;
							if (lastSession != season.ToString())
							{
								Sport.Entities.Championship.Type.Reset(null);
								Sport.Entities.ChampionshipCategory.Type.Reset(null);
								Sport.Entities.ChampionshipRegion.Type.Reset(null);
								Sport.Entities.Championship.Type.DeleteDatFile();
								Sport.Entities.ChampionshipCategory.Type.DeleteDatFile();
								Sport.Entities.ChampionshipRegion.Type.DeleteDatFile();
							}
							
							Sport.Core.Configuration.WriteString("General", "LastSeason", season.ToString());
							bLogin = true;
						}
					}
					else
					{
						Sport.UI.MessageBox.Error("זיהוי משתמש או סיסמא שגויים", 
							"שגיאת מערכת");
					}
				}
			}
			catch (Exception e)
			{
				string strMessage="שגיאה בעת התחברות אל שרת הנתונים"+"\n"+e.Message;
				if (e.Message.ToLower().IndexOf("too old") >= 0)
				{
					strMessage="גרסת התוכנה אותה הנך מנסה להפעיל ישנה.\n"+
						"יש להוריד את הגרסה החדשה על מנת להשתמש בתוכנה.";
					Sport.UI.MessageBox.Error(strMessage, "שגיאת מערכת");
					/*
					string strURL="";
					strURL += "http://www.amitbb.co.il/ISF/SportSite/Register.aspx";
					strURL += "?action=DownloadUpdates&fileIndex=1";
					*/
					string strURL=Sport.Core.Data.ProgramUpdateURL;
					System.Diagnostics.Process.Start(strURL);
				}
				else
				{
					Sport.UI.MessageBox.Error(strMessage, "שגיאת מערכת");
				}
				return false;
			}
			
			return bLogin;
		} //end function PerformLogin
	}
}
