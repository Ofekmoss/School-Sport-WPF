using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Sportsman_Field.SessionServices;
using Sportsman_Field.DataServices;

namespace Sportsman_Field.Dialogs
{
	/// <summary>
	/// Summary description for LoginDialog.
	/// </summary>
	public class LoginDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox tbUserName;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox tbPassword;
		private SessionService _sessionService=null;
		private DataService _dataService=null;
		private SeasonData _season;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public LoginDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			_sessionService = new SessionService();
			_dataService = new DataService();
			_season = _sessionService.GetCurrentSeason();
			string strSeasonName=_season.Name;
			if (strSeasonName.Length > 0)
			{
				this.Text += " ("+strSeasonName+")";
			}
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
			this.label1 = new System.Windows.Forms.Label();
			this.tbUserName = new System.Windows.Forms.TextBox();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.tbPassword = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label1.Location = new System.Drawing.Point(128, 10);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(82, 18);
			this.label1.TabIndex = 1;
			this.label1.Text = "שם משתמש:";
			// 
			// tbUserName
			// 
			this.tbUserName.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.tbUserName.ForeColor = System.Drawing.Color.Blue;
			this.tbUserName.Location = new System.Drawing.Point(10, 10);
			this.tbUserName.MaxLength = 20;
			this.tbUserName.Name = "tbUserName";
			this.tbUserName.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.tbUserName.Size = new System.Drawing.Size(105, 22);
			this.tbUserName.TabIndex = 1;
			this.tbUserName.Text = "";
			// 
			// btnCancel
			// 
			this.btnCancel.BackColor = System.Drawing.Color.White;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnCancel.ForeColor = System.Drawing.Color.Red;
			this.btnCancel.Location = new System.Drawing.Point(15, 75);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.TabIndex = 6;
			this.btnCancel.Text = "ביטול";
			// 
			// btnOK
			// 
			this.btnOK.BackColor = System.Drawing.Color.White;
			this.btnOK.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnOK.ForeColor = System.Drawing.Color.Blue;
			this.btnOK.Location = new System.Drawing.Point(130, 75);
			this.btnOK.Name = "btnOK";
			this.btnOK.TabIndex = 3;
			this.btnOK.Text = "אישור";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// tbPassword
			// 
			this.tbPassword.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.tbPassword.ForeColor = System.Drawing.Color.Blue;
			this.tbPassword.Location = new System.Drawing.Point(10, 42);
			this.tbPassword.MaxLength = 20;
			this.tbPassword.Name = "tbPassword";
			this.tbPassword.PasswordChar = '*';
			this.tbPassword.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.tbPassword.Size = new System.Drawing.Size(105, 22);
			this.tbPassword.TabIndex = 2;
			this.tbPassword.Text = "";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label2.Location = new System.Drawing.Point(160, 40);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(52, 18);
			this.label2.TabIndex = 7;
			this.label2.Text = "סיסמא:";
			// 
			// LoginDialog
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.Color.White;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(224, 109);
			this.Controls.Add(this.tbPassword);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.tbUserName);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "LoginDialog";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "כניסה למערכת";
			this.Load += new System.EventHandler(this.LoginDialog_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void LoginDialog_Load(object sender, System.EventArgs e)
		{
		
		}

		private void btnOK_Click(object sender, System.EventArgs e)
		{
			string strUserName=tbUserName.Text;
			string strPassword=Sport.Common.Crypto.Encode(tbPassword.Text);

			if ((strUserName == null)||(strUserName.Length == 0))
			{
				Sport.UI.MessageBox.Error("אנא הכנס שם משתמש", "רישום למערכת");
				ResetFocus();
				return;
			}
			
			int season=_season.Season;
			UserData userData=_sessionService.Login(
				strUserName+"#field#"+Sport.Core.Data.Field_CurrentVersion, strPassword, season);
			if ((userData.Username == null)||(userData.Username.Length == 0))
			{
				Sport.UI.MessageBox.Error("שם משתמש או סיסמא שגויים", "רישום למערכת");
				ResetFocus();
				return;
			}
			if (userData.UserType == (int) Sport.Types.UserType.External)
			{
				Sport.UI.MessageBox.Error("משתמש זה אינו מורשה לשימוש במערכת", "רישום למערכת");
				ResetFocus();
				return;
			}
			
			Core.UserManager.CurrentUser = new Core.User();
			Core.UserManager.CurrentUser.ID = userData.Id;
			Core.UserManager.CurrentUser.Name = userData.UserFullName;
			Core.UserManager.CurrentUser.Region = _dataService.GetRegion(userData.UserRegion);
			Core.Data.CurrentSeason = new Core.Season();
			Core.Data.CurrentSeason.ID = _season.Season;
			Core.Data.CurrentSeason.Name = _season.Name;
			
			this.DialogResult = DialogResult.OK;
		}

		private void ResetFocus()
		{
			tbUserName.Focus();
			tbUserName.SelectAll();
		}
	}
}
