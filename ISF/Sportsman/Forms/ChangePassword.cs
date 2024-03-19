using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using Sport.Entities;
using Sportsman.Core;

namespace Sportsman.Forms
{
	/// <summary>
	/// Summary description for ChangePassword.
	/// </summary>
	public class ChangePassword : System.Windows.Forms.Form
	{
		private string _username;
		private string _password;
		private int _usertype;

		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnLogin;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox edConfPassword;
		private System.Windows.Forms.TextBox edNewPassword;
		private System.Windows.Forms.Label lbUserLogin;
		private System.Windows.Forms.Label lbUserName;
		private Label lbPasswordStrength;
		private Label label6;
		private Button btnShow;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ChangePassword()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			_username = "";
			_password = null;
		}

		/// <summary>
		/// get or set user login string for user to which to change password.
		/// </summary>
		public string Username
		{
			get { return _username; }
			set { _username = value; }
		}

		/// <summary>
		/// returns the password typed by user.
		/// </summary>
		public string Password
		{
			get { return _password; }
		}

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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChangePassword));
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnLogin = new System.Windows.Forms.Button();
			this.edConfPassword = new System.Windows.Forms.TextBox();
			this.edNewPassword = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.lbUserLogin = new System.Windows.Forms.Label();
			this.lbUserName = new System.Windows.Forms.Label();
			this.lbPasswordStrength = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.btnShow = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.btnCancel.ForeColor = System.Drawing.Color.Red;
			this.btnCancel.Location = new System.Drawing.Point(183, 242);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(120, 23);
			this.btnCancel.TabIndex = 11;
			this.btnCancel.Text = "ביטול שינוי סיסמא";
			// 
			// btnLogin
			// 
			this.btnLogin.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.btnLogin.ForeColor = System.Drawing.Color.Blue;
			this.btnLogin.Location = new System.Drawing.Point(39, 242);
			this.btnLogin.Name = "btnLogin";
			this.btnLogin.Size = new System.Drawing.Size(120, 23);
			this.btnLogin.TabIndex = 10;
			this.btnLogin.Text = "שינוי סיסמא";
			this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
			// 
			// edConfPassword
			// 
			this.edConfPassword.Font = new System.Drawing.Font("Times New Roman", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.edConfPassword.Location = new System.Drawing.Point(39, 144);
			this.edConfPassword.MaxLength = 20;
			this.edConfPassword.Name = "edConfPassword";
			this.edConfPassword.PasswordChar = '*';
			this.edConfPassword.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.edConfPassword.Size = new System.Drawing.Size(144, 24);
			this.edConfPassword.TabIndex = 9;
			this.edConfPassword.TextChanged += new System.EventHandler(this.Password_TextChanged);
			// 
			// edNewPassword
			// 
			this.edNewPassword.Font = new System.Drawing.Font("Times New Roman", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.edNewPassword.Location = new System.Drawing.Point(39, 104);
			this.edNewPassword.MaxLength = 20;
			this.edNewPassword.Name = "edNewPassword";
			this.edNewPassword.PasswordChar = '*';
			this.edNewPassword.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.edNewPassword.Size = new System.Drawing.Size(144, 24);
			this.edNewPassword.TabIndex = 8;
			this.edNewPassword.TextChanged += new System.EventHandler(this.Password_TextChanged);
			// 
			// label2
			// 
			this.label2.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.label2.ForeColor = System.Drawing.Color.Blue;
			this.label2.Location = new System.Drawing.Point(191, 56);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(112, 24);
			this.label2.TabIndex = 7;
			this.label2.Text = "שם מלא: ";
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.label1.ForeColor = System.Drawing.Color.Blue;
			this.label1.Location = new System.Drawing.Point(191, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(112, 24);
			this.label1.TabIndex = 6;
			this.label1.Text = "זיהוי משתמש:";
			// 
			// label3
			// 
			this.label3.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.label3.ForeColor = System.Drawing.Color.Blue;
			this.label3.Location = new System.Drawing.Point(191, 104);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(112, 24);
			this.label3.TabIndex = 12;
			this.label3.Text = "סיסמא חדשה: ";
			// 
			// label4
			// 
			this.label4.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.label4.ForeColor = System.Drawing.Color.Blue;
			this.label4.Location = new System.Drawing.Point(189, 144);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(112, 24);
			this.label4.TabIndex = 14;
			this.label4.Text = "אישור סיסמא: ";
			// 
			// lbUserLogin
			// 
			this.lbUserLogin.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.lbUserLogin.ForeColor = System.Drawing.Color.Black;
			this.lbUserLogin.Location = new System.Drawing.Point(31, 16);
			this.lbUserLogin.Name = "lbUserLogin";
			this.lbUserLogin.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.lbUserLogin.Size = new System.Drawing.Size(160, 24);
			this.lbUserLogin.TabIndex = 15;
			this.lbUserLogin.Text = "username";
			this.lbUserLogin.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lbUserName
			// 
			this.lbUserName.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.lbUserName.ForeColor = System.Drawing.Color.Black;
			this.lbUserName.Location = new System.Drawing.Point(31, 56);
			this.lbUserName.Name = "lbUserName";
			this.lbUserName.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.lbUserName.Size = new System.Drawing.Size(160, 24);
			this.lbUserName.TabIndex = 16;
			this.lbUserName.Text = "full name";
			this.lbUserName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lbPasswordStrength
			// 
			this.lbPasswordStrength.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.lbPasswordStrength.ForeColor = System.Drawing.Color.Black;
			this.lbPasswordStrength.Location = new System.Drawing.Point(29, 171);
			this.lbPasswordStrength.Name = "lbPasswordStrength";
			this.lbPasswordStrength.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.lbPasswordStrength.Size = new System.Drawing.Size(160, 50);
			this.lbPasswordStrength.TabIndex = 18;
			this.lbPasswordStrength.Text = "-";
			this.lbPasswordStrength.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label6
			// 
			this.label6.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.label6.ForeColor = System.Drawing.Color.Blue;
			this.label6.Location = new System.Drawing.Point(191, 189);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(112, 24);
			this.label6.TabIndex = 17;
			this.label6.Text = "חוזק סיסמא:";
			// 
			// btnShow
			// 
			this.btnShow.Image = ((System.Drawing.Image)(resources.GetObject("btnShow.Image")));
			this.btnShow.Location = new System.Drawing.Point(9, 124);
			this.btnShow.Name = "btnShow";
			this.btnShow.Size = new System.Drawing.Size(21, 23);
			this.btnShow.TabIndex = 19;
			this.btnShow.UseVisualStyleBackColor = true;
			this.btnShow.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnShow_MouseDown);
			this.btnShow.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnShow_MouseUp);
			// 
			// ChangePassword
			// 
			this.AcceptButton = this.btnLogin;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(316, 287);
			this.Controls.Add(this.btnShow);
			this.Controls.Add(this.lbPasswordStrength);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.lbUserName);
			this.Controls.Add(this.lbUserLogin);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnLogin);
			this.Controls.Add(this.edConfPassword);
			this.Controls.Add(this.edNewPassword);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "ChangePassword";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.Text = "שינוי סיסמא";
			this.Activated += new System.EventHandler(this.ChangePassword_Activated);
			this.Load += new System.EventHandler(this.ChangePassword_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private void ChangePassword_Load(object sender, System.EventArgs e)
		{
			if (_username.Length == 0)
			{
				Sport.UI.MessageBox.Show("לא ניתן לשנות סיסמא: זיהוי משתמש שגוי או חסר",
									"שגיאת מערכת", MessageBoxButtons.OK, MessageBoxIcon.Error,
									MessageBoxDefaultButton.Button1);
				this.Close();
				return;
			}

			SessionServices.SessionService service = new SessionServices.SessionService();
			service.CookieContainer = Sport.Core.Session.Cookies;
			SessionServices.UserData user = service.GetUserData(_username);
			if (user.Username == null)
			{
				Sport.UI.MessageBox.Show("משתמש לא נמצא במאגר הנתונים: " + _username, "שגיאת מערכת",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
				this.Close();
				return;
			}

			_usertype = user.UserType;
			lbUserLogin.Text = user.Username;
			lbUserName.Text = user.UserFullName;
		}

		private void ChangePassword_Activated(object sender, System.EventArgs e)
		{

		}

		/// <summary>
		/// click event handler for the confirmation button.
		/// check that password and confirm password match, warn if the password
		/// is empty. upon success, close the dialog with OK result.
		/// </summary>
		private void btnLogin_Click(object sender, System.EventArgs e)
		{
			//get passwords:
			string password = edNewPassword.Text;
			string password2 = edConfPassword.Text;

			//check that equal:
			if (!password.Equals(password2))
			{
				MessageBox.Show("אישור סיסמא לא תואם לסיסמא", "שגיאה",
								MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			password = password.Trim();
			if (password.Length == 0)
			{
				MessageBox.Show("סיסמא לא יכולה להיות ריקה או להכיל רווחים בלבד", "שגיאה",
								MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			/*
			//check if empty:
			if (password.Length == 0)
			{
				string message = "הינך מגדיר משתמש עם סיסמא ריקה.\n";
				message += "משמעות פעולה זו היא שכל אחד יוכל להתחבר בתור משתמש זה למערכת.\n";
				message += "האם אתה בטוח שברצונך להמשיך?";
				DialogResult dr = Sport.UI.MessageBox.Show(message, "אזהרה", MessageBoxButtons.YesNo,
										MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
				if (dr != DialogResult.Yes)
					return;
			}
			*/

			var passwordStrength = Sport.Core.Utils.GetPasswordStrength(edNewPassword.Text, lbUserLogin.Text);
			if (_usertype == 1 && passwordStrength < Sport.Core.PasswordStrength.Strong)
			{
				MessageBox.Show("סיסמא עבור משתמש התאחדות חייבת להיות חזקה או חזקה מאוד", "שגיאה",
								MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			if (passwordStrength < Sport.Core.PasswordStrength.Weak)
			{
				string msg = "סיסמא ";
				if (_usertype == 2)
					msg += "עבור משתמש חיצוני ";
				msg += "לא יכולה להיות ריקה או חלשה מאוד";
				MessageBox.Show(msg, "שגיאה", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			_password = password;
			this.DialogResult = DialogResult.OK;
		}

		private void Password_TextChanged(object sender, EventArgs e)
		{
			string text = "[אישור סיסמא לא תואם לסיסמא]";
			Color bgColor = SystemColors.Control;
			if (edNewPassword.Text == edConfPassword.Text)
			{
				var passwordStrength = Sport.Core.Utils.GetPasswordStrength(edNewPassword.Text, lbUserLogin.Text);
				text = "-";
				switch (passwordStrength)
				{
					case Sport.Core.PasswordStrength.Trivial:
						text = "חלשה מאוד";
						bgColor = ColorTranslator.FromHtml("#C55B6F");
						break;
					case Sport.Core.PasswordStrength.Weak:
						text = "חלשה";
						bgColor = ColorTranslator.FromHtml("#FCD381");
						break;
					case Sport.Core.PasswordStrength.Strong:
						text = "חזקה";
						bgColor = ColorTranslator.FromHtml("#4BA9DD");
						break;
					case Sport.Core.PasswordStrength.Pentagon:
						text = "חזקה מאוד";
						bgColor = ColorTranslator.FromHtml("#4BA900");
						break;
				}
				
			}
			lbPasswordStrength.Text = text;
			lbPasswordStrength.BackColor = bgColor;
		}

		private void btnShow_MouseDown(object sender, MouseEventArgs e)
		{
			edNewPassword.PasswordChar = '\0';
			edConfPassword.PasswordChar = '\0';
		}

		private void btnShow_MouseUp(object sender, MouseEventArgs e)
		{
			edNewPassword.PasswordChar = '*';
			edConfPassword.PasswordChar = '*';
		}
	}
	//new System.Globalization.CultureInfo("he-IL")
}