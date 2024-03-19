namespace Sportsman.Forms
{
	partial class ChooseAdminReportType
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
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
			this.label1 = new System.Windows.Forms.Label();
			this.btnTeam = new System.Windows.Forms.Button();
			this.btnPersonal = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Dock = System.Windows.Forms.DockStyle.Top;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.label1.Location = new System.Drawing.Point(0, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(282, 25);
			this.label1.TabIndex = 1;
			this.label1.Text = "בחר סוג דו\"ח:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// btnTeam
			// 
			this.btnTeam.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.btnTeam.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.btnTeam.Location = new System.Drawing.Point(0, 87);
			this.btnTeam.Name = "btnTeam";
			this.btnTeam.Size = new System.Drawing.Size(282, 46);
			this.btnTeam.TabIndex = 2;
			this.btnTeam.Text = "קבוצתי";
			this.btnTeam.UseVisualStyleBackColor = true;
			this.btnTeam.Click += new System.EventHandler(this.btnTeam_Click);
			// 
			// btnPersonal
			// 
			this.btnPersonal.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.btnPersonal.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.btnPersonal.Location = new System.Drawing.Point(0, 41);
			this.btnPersonal.Name = "btnPersonal";
			this.btnPersonal.Size = new System.Drawing.Size(282, 46);
			this.btnPersonal.TabIndex = 3;
			this.btnPersonal.Text = "אישי";
			this.btnPersonal.UseVisualStyleBackColor = true;
			this.btnPersonal.Click += new System.EventHandler(this.btnPersonal_Click);
			// 
			// ChooseAdminReportType
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(282, 133);
			this.Controls.Add(this.btnPersonal);
			this.Controls.Add(this.btnTeam);
			this.Controls.Add(this.label1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ChooseAdminReportType";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "דו\"ח מנהל הספורט";
			this.Load += new System.EventHandler(this.ChooseAdminReportType_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnTeam;
		private System.Windows.Forms.Button btnPersonal;
	}
}