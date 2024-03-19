using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Sportsman.Forms
{
	/// <summary>
	/// Summary description for InstantMessageForm.
	/// </summary>
	public class InstantMessageForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button btnClose;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label lbSender;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox tbContents;
		private System.Windows.Forms.CheckBox cbMessageRead;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label lbDateSent;
		private System.Windows.Forms.Button btnSendReply;
		
		private int _instantMessageID=-1;
		private Sport.Entities.InstantMessage _message=null;
		
		private int _replyRecipientID=-1;
		public int ReplyRecipient
		{
			get {return _replyRecipientID;}
		}
		
		public InstantMessageForm(int instantMessageID)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			
			_instantMessageID = instantMessageID;
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
			this.btnClose = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.lbSender = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.tbContents = new System.Windows.Forms.TextBox();
			this.cbMessageRead = new System.Windows.Forms.CheckBox();
			this.label4 = new System.Windows.Forms.Label();
			this.lbDateSent = new System.Windows.Forms.Label();
			this.btnSendReply = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// btnClose
			// 
			this.btnClose.BackColor = System.Drawing.Color.Gray;
			this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnClose.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnClose.ForeColor = System.Drawing.Color.White;
			this.btnClose.Location = new System.Drawing.Point(16, 336);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(96, 32);
			this.btnClose.TabIndex = 10;
			this.btnClose.Text = "סגור חלון";
			this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(128, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(207, 26);
			this.label1.TabIndex = 11;
			this.label1.Text = "התקבלה הודעה מיידית!";
			// 
			// lbSender
			// 
			this.lbSender.ForeColor = System.Drawing.Color.Blue;
			this.lbSender.Location = new System.Drawing.Point(8, 56);
			this.lbSender.Name = "lbSender";
			this.lbSender.Size = new System.Drawing.Size(176, 32);
			this.lbSender.TabIndex = 12;
			this.lbSender.Text = "[לא מוגדר]";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(184, 56);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(149, 26);
			this.label3.TabIndex = 13;
			this.label3.Text = "שולח/ת ההודעה:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(208, 136);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(121, 26);
			this.label2.TabIndex = 14;
			this.label2.Text = "תוכן ההודעה:";
			// 
			// tbContents
			// 
			this.tbContents.AutoSize = false;
			this.tbContents.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tbContents.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.tbContents.ForeColor = System.Drawing.Color.Blue;
			this.tbContents.Location = new System.Drawing.Point(16, 168);
			this.tbContents.Multiline = true;
			this.tbContents.Name = "tbContents";
			this.tbContents.ReadOnly = true;
			this.tbContents.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.tbContents.Size = new System.Drawing.Size(320, 136);
			this.tbContents.TabIndex = 15;
			this.tbContents.Text = "";
			// 
			// cbMessageRead
			// 
			this.cbMessageRead.Checked = true;
			this.cbMessageRead.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbMessageRead.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.cbMessageRead.Location = new System.Drawing.Point(200, 312);
			this.cbMessageRead.Name = "cbMessageRead";
			this.cbMessageRead.Size = new System.Drawing.Size(136, 24);
			this.cbMessageRead.TabIndex = 16;
			this.cbMessageRead.Text = "קראתי הודעה זו";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(200, 96);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(129, 26);
			this.label4.TabIndex = 18;
			this.label4.Text = "תאריך שליחה:";
			// 
			// lbDateSent
			// 
			this.lbDateSent.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbDateSent.ForeColor = System.Drawing.Color.Blue;
			this.lbDateSent.Location = new System.Drawing.Point(8, 96);
			this.lbDateSent.Name = "lbDateSent";
			this.lbDateSent.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.lbDateSent.Size = new System.Drawing.Size(184, 26);
			this.lbDateSent.TabIndex = 19;
			this.lbDateSent.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// btnSendReply
			// 
			this.btnSendReply.BackColor = System.Drawing.Color.Gray;
			this.btnSendReply.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnSendReply.ForeColor = System.Drawing.Color.White;
			this.btnSendReply.Location = new System.Drawing.Point(136, 344);
			this.btnSendReply.Name = "btnSendReply";
			this.btnSendReply.Size = new System.Drawing.Size(96, 24);
			this.btnSendReply.TabIndex = 20;
			this.btnSendReply.Text = "שליחת תשובה";
			this.btnSendReply.Click += new System.EventHandler(this.btnSendReply_Click);
			// 
			// InstantMessageForm
			// 
			this.AcceptButton = this.btnClose;
			this.AutoScaleBaseSize = new System.Drawing.Size(10, 23);
			this.BackColor = System.Drawing.Color.White;
			this.CancelButton = this.btnClose;
			this.ClientSize = new System.Drawing.Size(352, 376);
			this.Controls.Add(this.btnSendReply);
			this.Controls.Add(this.lbDateSent);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.tbContents);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.cbMessageRead);
			this.Controls.Add(this.lbSender);
			this.Controls.Add(this.btnClose);
			this.Font = new System.Drawing.Font("Arial", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "InstantMessageForm";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "הודעה מיידית";
			this.Load += new System.EventHandler(this.InstantMessageForm_Load);
			this.Closed += new System.EventHandler(this.InstantMessageForm_Closed);
			this.ResumeLayout(false);

		}
		#endregion

		private void InstantMessageForm_Load(object sender, System.EventArgs e)
		{
			if (_instantMessageID < 0)
				return;
			
			_message = null;
			try
			{
				_message = new Sport.Entities.InstantMessage(_instantMessageID);
			}
			catch {}
			
			if ((_message == null)||(_message.Id < 0))
				return;
			
			lbSender.Text = _message.Sender.Name;
			lbDateSent.Text = _message.DateSent.ToString("dd/MM/yyyy HH:mm:ss");
			string strContents=_message.Contents;
			strContents = strContents.Replace("\r\n", "\n");
			strContents = strContents.Replace("\n", "\r\n");
			tbContents.Text = strContents;
		}

		private void btnClose_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void InstantMessageForm_Closed(object sender, System.EventArgs e)
		{
			if (cbMessageRead.Checked)
			{
				if ((_message == null)||(_message.Id < 0))
					return;
				_message.MarkAsRead();
			}
		}

		private void btnSendReply_Click(object sender, System.EventArgs e)
		{
			//assign private flag.
			_replyRecipientID = _message.Sender.Id;
			this.DialogResult = DialogResult.OK;
		}
		
	}
}
