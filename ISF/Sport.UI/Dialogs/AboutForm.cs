using System;
using System.Linq;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Sport.UI.Dialogs
{
	/// <summary>
	/// Summary description for AboutForm.
	/// </summary>
	public class AboutForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Button btnClose;
		private System.Windows.Forms.Panel pnlNewVersion;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.LinkLabel lblLink;
		private System.Windows.Forms.Label lblVersion;
		public double CurrentVersion=0;
		public double LastestVersion=0;
		public string ServiceUrl = "";
		private System.Windows.Forms.PictureBox imgLogo;
		private Label label3;
		private Label lbWebServiceUrl;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public AboutForm()
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.lblVersion = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.btnClose = new System.Windows.Forms.Button();
			this.pnlNewVersion = new System.Windows.Forms.Panel();
			this.lblLink = new System.Windows.Forms.LinkLabel();
			this.label6 = new System.Windows.Forms.Label();
			this.imgLogo = new System.Windows.Forms.PictureBox();
			this.label3 = new System.Windows.Forms.Label();
			this.lbWebServiceUrl = new System.Windows.Forms.Label();
			this.pnlNewVersion.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.imgLogo)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.label1.Location = new System.Drawing.Point(128, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(256, 32);
			this.label1.TabIndex = 0;
			this.label1.Text = "התאחדות הספורט לבתי ספר בישראל - תוכנת ניהול";
			// 
			// label2
			// 
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.label2.Location = new System.Drawing.Point(296, 56);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(88, 32);
			this.label2.TabIndex = 1;
			this.label2.Text = "גרסת התוכנה: ";
			// 
			// lblVersion
			// 
			this.lblVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.lblVersion.ForeColor = System.Drawing.Color.Blue;
			this.lblVersion.Location = new System.Drawing.Point(200, 56);
			this.lblVersion.Name = "lblVersion";
			this.lblVersion.Size = new System.Drawing.Size(88, 32);
			this.lblVersion.TabIndex = 2;
			// 
			// label4
			// 
			this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.label4.Location = new System.Drawing.Point(136, 96);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(200, 16);
			this.label4.TabIndex = 3;
			this.label4.Text = "כל הזכויות שמורות, מ.י.ר";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label5
			// 
			this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.label5.Location = new System.Drawing.Point(136, 112);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(200, 16);
			this.label5.TabIndex = 4;
			this.label5.Text = "2023";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// btnClose
			// 
			this.btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(177)));
			this.btnClose.ForeColor = System.Drawing.Color.Blue;
			this.btnClose.Location = new System.Drawing.Point(184, 216);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(104, 24);
			this.btnClose.TabIndex = 5;
			this.btnClose.Text = "אישור";
			// 
			// pnlNewVersion
			// 
			this.pnlNewVersion.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pnlNewVersion.Controls.Add(this.lblLink);
			this.pnlNewVersion.Controls.Add(this.label6);
			this.pnlNewVersion.Location = new System.Drawing.Point(96, 136);
			this.pnlNewVersion.Name = "pnlNewVersion";
			this.pnlNewVersion.Size = new System.Drawing.Size(288, 72);
			this.pnlNewVersion.TabIndex = 6;
			// 
			// lblLink
			// 
			this.lblLink.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.lblLink.Location = new System.Drawing.Point(64, 32);
			this.lblLink.Name = "lblLink";
			this.lblLink.Size = new System.Drawing.Size(192, 23);
			this.lblLink.TabIndex = 5;
			this.lblLink.TabStop = true;
			this.lblLink.Text = "לחץ כאן להורדה";
			this.lblLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblLink_LinkClicked);
			// 
			// label6
			// 
			this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.label6.Location = new System.Drawing.Point(16, 8);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(264, 16);
			this.label6.TabIndex = 4;
			this.label6.Text = "גרסה מעודכנת של התוכנה זמינה להורדה באתר";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// imgLogo
			// 
			this.imgLogo.AccessibleDescription = "MIR logo";
			this.imgLogo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.imgLogo.Image = ((System.Drawing.Image)(resources.GetObject("imgLogo.Image")));
			this.imgLogo.Location = new System.Drawing.Point(8, 8);
			this.imgLogo.Name = "imgLogo";
			this.imgLogo.Size = new System.Drawing.Size(100, 100);
			this.imgLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.imgLogo.TabIndex = 7;
			this.imgLogo.TabStop = false;
			// 
			// label3
			// 
			this.label3.BackColor = System.Drawing.Color.White;
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.label3.ForeColor = System.Drawing.Color.Blue;
			this.label3.Location = new System.Drawing.Point(113, 38);
			this.label3.Name = "label3";
			this.label3.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.label3.Size = new System.Drawing.Size(82, 50);
			this.label3.TabIndex = 8;
			this.label3.Text = ".NET Framework 4.0";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lbWebServiceUrl
			// 
			this.lbWebServiceUrl.AutoSize = true;
			this.lbWebServiceUrl.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.lbWebServiceUrl.Location = new System.Drawing.Point(10, 222);
			this.lbWebServiceUrl.Name = "lbWebServiceUrl";
			this.lbWebServiceUrl.Size = new System.Drawing.Size(97, 13);
			this.lbWebServiceUrl.TabIndex = 9;
			this.lbWebServiceUrl.Text = "service location";
			// 
			// AboutForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(484, 287);
			this.Controls.Add(this.lbWebServiceUrl);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.imgLogo);
			this.Controls.Add(this.pnlNewVersion);
			this.Controls.Add(this.btnClose);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.lblVersion);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AboutForm";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "אודות התוכנה";
			this.Load += new System.EventHandler(this.AboutForm_Load);
			this.pnlNewVersion.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.imgLogo)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private void AboutForm_Load(object sender, System.EventArgs e)
		{
			//set buttons:
			this.CancelButton = btnClose;
			this.AcceptButton = btnClose;
			
			//set current version:
			lblVersion.Text = CurrentVersion.ToString().PadRight(3, '0'); //+" (Beta)"
			
			//set link:
			this.lblLink.Links.Add(0, lblLink.Text.Length, Sport.Core.Data.ProgramUpdateURL);

			if (CurrentVersion >= LastestVersion)
			{
				pnlNewVersion.Visible = false;
				btnClose.Top -= pnlNewVersion.Height;
				lbWebServiceUrl.Top -= pnlNewVersion.Height;
				this.Height -= pnlNewVersion.Height;
			}

			//service url:
			string serviceUrl = ServiceUrl;
			lbWebServiceUrl.Text = ExtractDomain(serviceUrl);

			//secure?
			if (serviceUrl != null && serviceUrl.ToLower().IndexOf("https://") == 0)
			{
				lbWebServiceUrl.BackColor = ColorTranslator.FromHtml("#188038");
			}

			//set image tooltip:
			Sport.Common.Tools.AttachTooltip(imgLogo, "Machine Integration Retrofit");
		}

		private string ExtractDomain(string url)
		{
			if (url != null && url.Length > 0)
			{
				int index = url.IndexOf("//");
				if (index > 0)
				{
					url = url.Substring(index + 2);
					index = url.IndexOf("/");
					if (index > 0)
						url = url.Substring(0, index);
					return url;
				}
			}
			return "";
		}

		private void lblLink_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			System.Diagnostics.Process.Start(lblLink.Links[0].LinkData.ToString());
		}
	}
}
