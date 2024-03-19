using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Sportsman_Field.SessionServices;

namespace Sportsman_Field.Dialogs
{
	/// <summary>
	/// Summary description for ChooseStatusDialog.
	/// </summary>
	public class ChooseStatusDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		public System.Windows.Forms.RadioButton rbOffline;
		public System.Windows.Forms.RadioButton rbOnline;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ChooseStatusDialog()
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
			this.label1 = new System.Windows.Forms.Label();
			this.rbOffline = new System.Windows.Forms.RadioButton();
			this.rbOnline = new System.Windows.Forms.RadioButton();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label1.Location = new System.Drawing.Point(24, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(245, 20);
			this.label1.TabIndex = 0;
			this.label1.Text = "אנא בחר את צורת הפעלת התוכנה:";
			// 
			// rbOffline
			// 
			this.rbOffline.Checked = true;
			this.rbOffline.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.rbOffline.ForeColor = System.Drawing.Color.Blue;
			this.rbOffline.Location = new System.Drawing.Point(40, 48);
			this.rbOffline.Name = "rbOffline";
			this.rbOffline.Size = new System.Drawing.Size(216, 24);
			this.rbOffline.TabIndex = 1;
			this.rbOffline.TabStop = true;
			this.rbOffline.Text = "לא מקוון - ללא שרת מרכזי";
			// 
			// rbOnline
			// 
			this.rbOnline.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.rbOnline.ForeColor = System.Drawing.Color.Blue;
			this.rbOnline.Location = new System.Drawing.Point(8, 88);
			this.rbOnline.Name = "rbOnline";
			this.rbOnline.Size = new System.Drawing.Size(248, 24);
			this.rbOnline.TabIndex = 2;
			this.rbOnline.Text = "מקוון - אפשר ייבוא וייצוא נתונים";
			// 
			// btnOK
			// 
			this.btnOK.BackColor = System.Drawing.Color.White;
			this.btnOK.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnOK.ForeColor = System.Drawing.Color.Blue;
			this.btnOK.Location = new System.Drawing.Point(176, 128);
			this.btnOK.Name = "btnOK";
			this.btnOK.TabIndex = 3;
			this.btnOK.Text = "אישור";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.BackColor = System.Drawing.Color.White;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnCancel.ForeColor = System.Drawing.Color.Red;
			this.btnCancel.Location = new System.Drawing.Point(32, 128);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.TabIndex = 4;
			this.btnCancel.Text = "ביטול";
			// 
			// ChooseStatusDialog
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.Color.White;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(274, 175);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.rbOnline);
			this.Controls.Add(this.rbOffline);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ChooseStatusDialog";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "בחירת סוג חיבור";
			this.ResumeLayout(false);

		}
		#endregion

		private void btnOK_Click(object sender, System.EventArgs e)
		{
			if (rbOnline.Checked)
			{
				//try to create service:
				Sport.UI.Dialogs.WaitForm.ShowWait("מתחבר אל שרת הנתונים...");
				try
				{
					SessionService _service=new SessionService();
					_service.VerifyUser("dummy", "-null-");
				}
				catch (Exception err)
				{
					string strMessage="שגיאה בעת התחברות אל שרת הנתונים:";
					strMessage += "\n("+err.Message+")";
					Sport.UI.Dialogs.WaitForm.HideWait();
					Sport.UI.MessageBox.Error(strMessage, "שגיאת מערכת");
					return;
				}
				Sport.UI.Dialogs.WaitForm.HideWait();

				//user must login:
				LoginDialog _dialog=new LoginDialog();
				if (_dialog.ShowDialog() != DialogResult.OK)
				{
					Sport.UI.MessageBox.Error("יש להרשם למערכת על מנת לעבוד במצב מקוון", 
										"שגיאת מערכת");
					return;
				}
			}
			this.DialogResult = DialogResult.OK;
		}
	}
}
