namespace Sportsman.Forms
{
	partial class ChooseChampionshipsReportDialog
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
			this.pnlRadioButtons = new System.Windows.Forms.Panel();
			this.lbAdminReportDisableReason = new System.Windows.Forms.Label();
			this.lbOtherSportsDisableReason = new System.Windows.Forms.Label();
			this.lbClubDisableReason = new System.Windows.Forms.Label();
			this.rbSportAdminReport = new System.Windows.Forms.RadioButton();
			this.rbOtherSportsReport = new System.Windows.Forms.RadioButton();
			this.rbClubReport = new System.Windows.Forms.RadioButton();
			this.btnConfirm = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.nudMinCategoryCellWidth = new System.Windows.Forms.NumericUpDown();
			this.pnlRadioButtons.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudMinCategoryCellWidth)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(336, 18);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(185, 29);
			this.label1.TabIndex = 0;
			this.label1.Text = "בחר דו\"ח להדפסה:";
			this.label1.DoubleClick += new System.EventHandler(this.label1_DoubleClick);
			// 
			// pnlRadioButtons
			// 
			this.pnlRadioButtons.Controls.Add(this.lbAdminReportDisableReason);
			this.pnlRadioButtons.Controls.Add(this.lbOtherSportsDisableReason);
			this.pnlRadioButtons.Controls.Add(this.lbClubDisableReason);
			this.pnlRadioButtons.Controls.Add(this.rbSportAdminReport);
			this.pnlRadioButtons.Controls.Add(this.rbOtherSportsReport);
			this.pnlRadioButtons.Controls.Add(this.rbClubReport);
			this.pnlRadioButtons.Location = new System.Drawing.Point(20, 68);
			this.pnlRadioButtons.Name = "pnlRadioButtons";
			this.pnlRadioButtons.Size = new System.Drawing.Size(496, 216);
			this.pnlRadioButtons.TabIndex = 1;
			// 
			// lbAdminReportDisableReason
			// 
			this.lbAdminReportDisableReason.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.lbAdminReportDisableReason.Location = new System.Drawing.Point(25, 149);
			this.lbAdminReportDisableReason.Name = "lbAdminReportDisableReason";
			this.lbAdminReportDisableReason.Size = new System.Drawing.Size(240, 31);
			this.lbAdminReportDisableReason.TabIndex = 5;
			this.lbAdminReportDisableReason.Text = "(למה אפור)";
			this.lbAdminReportDisableReason.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lbAdminReportDisableReason.Visible = false;
			// 
			// lbOtherSportsDisableReason
			// 
			this.lbOtherSportsDisableReason.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.lbOtherSportsDisableReason.Location = new System.Drawing.Point(25, 89);
			this.lbOtherSportsDisableReason.Name = "lbOtherSportsDisableReason";
			this.lbOtherSportsDisableReason.Size = new System.Drawing.Size(240, 31);
			this.lbOtherSportsDisableReason.TabIndex = 4;
			this.lbOtherSportsDisableReason.Text = "(למה אפור)";
			this.lbOtherSportsDisableReason.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lbOtherSportsDisableReason.Visible = false;
			// 
			// lbClubDisableReason
			// 
			this.lbClubDisableReason.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.lbClubDisableReason.Location = new System.Drawing.Point(25, 27);
			this.lbClubDisableReason.Name = "lbClubDisableReason";
			this.lbClubDisableReason.Size = new System.Drawing.Size(243, 31);
			this.lbClubDisableReason.TabIndex = 3;
			this.lbClubDisableReason.Text = "(למה אפור)";
			this.lbClubDisableReason.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lbClubDisableReason.Visible = false;
			// 
			// rbSportAdminReport
			// 
			this.rbSportAdminReport.AutoSize = true;
			this.rbSportAdminReport.ForeColor = System.Drawing.Color.Blue;
			this.rbSportAdminReport.Location = new System.Drawing.Point(272, 149);
			this.rbSportAdminReport.Name = "rbSportAdminReport";
			this.rbSportAdminReport.Size = new System.Drawing.Size(205, 33);
			this.rbSportAdminReport.TabIndex = 2;
			this.rbSportAdminReport.TabStop = true;
			this.rbSportAdminReport.Tag = "lbAdminReportDisableReason";
			this.rbSportAdminReport.Text = "דו\"ח מנהל הספורט";
			this.rbSportAdminReport.UseVisualStyleBackColor = true;
			// 
			// rbOtherSportsReport
			// 
			this.rbOtherSportsReport.AutoSize = true;
			this.rbOtherSportsReport.ForeColor = System.Drawing.Color.Blue;
			this.rbOtherSportsReport.Location = new System.Drawing.Point(272, 89);
			this.rbOtherSportsReport.Name = "rbOtherSportsReport";
			this.rbOtherSportsReport.Size = new System.Drawing.Size(205, 33);
			this.rbOtherSportsReport.TabIndex = 1;
			this.rbOtherSportsReport.TabStop = true;
			this.rbOtherSportsReport.Tag = "lbOtherSportsDisableReason";
			this.rbOtherSportsReport.Text = "דו\"ח אירועי ספורט";
			this.rbOtherSportsReport.UseVisualStyleBackColor = true;
			// 
			// rbClubReport
			// 
			this.rbClubReport.AutoSize = true;
			this.rbClubReport.ForeColor = System.Drawing.Color.Blue;
			this.rbClubReport.Location = new System.Drawing.Point(316, 27);
			this.rbClubReport.Name = "rbClubReport";
			this.rbClubReport.Size = new System.Drawing.Size(161, 33);
			this.rbClubReport.TabIndex = 0;
			this.rbClubReport.TabStop = true;
			this.rbClubReport.Tag = "lbClubDisableReason";
			this.rbClubReport.Text = "דו\"ח מועדונים";
			this.rbClubReport.UseVisualStyleBackColor = true;
			this.rbClubReport.CheckedChanged += new System.EventHandler(this.rbClubReport_CheckedChanged);
			// 
			// btnConfirm
			// 
			this.btnConfirm.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnConfirm.Enabled = false;
			this.btnConfirm.Location = new System.Drawing.Point(394, 308);
			this.btnConfirm.Name = "btnConfirm";
			this.btnConfirm.Size = new System.Drawing.Size(122, 45);
			this.btnConfirm.TabIndex = 2;
			this.btnConfirm.Text = "אישור";
			this.btnConfirm.UseVisualStyleBackColor = true;
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(20, 308);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(90, 45);
			this.btnCancel.TabIndex = 3;
			this.btnCancel.Text = "ביטול";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// nudMinCategoryCellWidth
			// 
			this.nudMinCategoryCellWidth.Location = new System.Drawing.Point(20, 16);
			this.nudMinCategoryCellWidth.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
			this.nudMinCategoryCellWidth.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.nudMinCategoryCellWidth.Name = "nudMinCategoryCellWidth";
			this.nudMinCategoryCellWidth.Size = new System.Drawing.Size(120, 34);
			this.nudMinCategoryCellWidth.TabIndex = 4;
			this.nudMinCategoryCellWidth.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.nudMinCategoryCellWidth.Visible = false;
			// 
			// ChooseChampionshipsReportDialog
			// 
			this.AcceptButton = this.btnConfirm;
			this.AutoScaleDimensions = new System.Drawing.SizeF(14F, 29F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(533, 374);
			this.Controls.Add(this.nudMinCategoryCellWidth);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnConfirm);
			this.Controls.Add(this.pnlRadioButtons);
			this.Controls.Add(this.label1);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Margin = new System.Windows.Forms.Padding(5);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ChooseChampionshipsReportDialog";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "בחירת דו\"ח";
			this.Load += new System.EventHandler(this.ChooseChampionshipsReportDialog_Load);
			this.pnlRadioButtons.ResumeLayout(false);
			this.pnlRadioButtons.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudMinCategoryCellWidth)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Panel pnlRadioButtons;
		private System.Windows.Forms.RadioButton rbSportAdminReport;
		private System.Windows.Forms.RadioButton rbOtherSportsReport;
		private System.Windows.Forms.RadioButton rbClubReport;
		private System.Windows.Forms.Button btnConfirm;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Label lbAdminReportDisableReason;
		private System.Windows.Forms.Label lbOtherSportsDisableReason;
		private System.Windows.Forms.Label lbClubDisableReason;
		private System.Windows.Forms.NumericUpDown nudMinCategoryCellWidth;
	}
}