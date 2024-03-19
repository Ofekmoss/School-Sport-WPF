namespace Sportsman.Forms
{
	partial class ConfirmTeamsImportDialog
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
			this.lbTitle = new System.Windows.Forms.Label();
			this.tbSchoolNames = new System.Windows.Forms.TextBox();
			this.btnConfirm = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// lbTitle
			// 
			this.lbTitle.AutoSize = true;
			this.lbTitle.Location = new System.Drawing.Point(256, 9);
			this.lbTitle.Name = "lbTitle";
			this.lbTitle.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.lbTitle.Size = new System.Drawing.Size(167, 17);
			this.lbTitle.TabIndex = 2;
			this.lbTitle.Text = "אנא אשר הוספת {0} קבוצות:";
			// 
			// tbSchoolNames
			// 
			this.tbSchoolNames.Location = new System.Drawing.Point(12, 41);
			this.tbSchoolNames.Multiline = true;
			this.tbSchoolNames.Name = "tbSchoolNames";
			this.tbSchoolNames.ReadOnly = true;
			this.tbSchoolNames.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.tbSchoolNames.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.tbSchoolNames.Size = new System.Drawing.Size(408, 496);
			this.tbSchoolNames.TabIndex = 3;
			// 
			// btnConfirm
			// 
			this.btnConfirm.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnConfirm.Location = new System.Drawing.Point(321, 555);
			this.btnConfirm.Name = "btnConfirm";
			this.btnConfirm.Size = new System.Drawing.Size(99, 30);
			this.btnConfirm.TabIndex = 4;
			this.btnConfirm.Text = "אישור";
			this.btnConfirm.UseVisualStyleBackColor = true;
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(26, 555);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(99, 30);
			this.btnCancel.TabIndex = 5;
			this.btnCancel.Text = "ביטול";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// ConfirmTeamsImportDialog
			// 
			this.AcceptButton = this.btnConfirm;
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(435, 597);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnConfirm);
			this.Controls.Add(this.tbSchoolNames);
			this.Controls.Add(this.lbTitle);
			this.Name = "ConfirmTeamsImportDialog";
			this.RightToLeftLayout = true;
			this.Text = "אישור ייבוא קבוצות";
			this.Load += new System.EventHandler(this.ConfirmTeamsImportDialog_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lbTitle;
		private System.Windows.Forms.TextBox tbSchoolNames;
		private System.Windows.Forms.Button btnConfirm;
		private System.Windows.Forms.Button btnCancel;
	}
}