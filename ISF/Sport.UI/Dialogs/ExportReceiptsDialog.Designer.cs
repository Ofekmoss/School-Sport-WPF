namespace Sport.UI.Dialogs
{
	partial class ExportReceiptsDialog
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExportReceiptsDialog));
			this.ReceiptsGrid = new System.Windows.Forms.DataGridView();
			this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
			this.cbRecentFiles = new System.Windows.Forms.ComboBox();
			this.lbError = new System.Windows.Forms.Label();
			this.pnlRecentFileDetails = new System.Windows.Forms.Panel();
			this.label1 = new System.Windows.Forms.Label();
			this.lbRecentFileLastModified = new System.Windows.Forms.Label();
			this.btnReloadRecentFile = new Sport.UI.Controls.ThemeButton();
			this.tbCancel = new Sport.UI.Controls.ThemeButton();
			this.tbOk = new Sport.UI.Controls.ThemeButton();
			((System.ComponentModel.ISupportInitialize)(this.ReceiptsGrid)).BeginInit();
			this.pnlRecentFileDetails.SuspendLayout();
			this.SuspendLayout();
			// 
			// ReceiptsGrid
			// 
			this.ReceiptsGrid.AllowUserToAddRows = false;
			this.ReceiptsGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.ReceiptsGrid.Location = new System.Drawing.Point(12, 30);
			this.ReceiptsGrid.Name = "ReceiptsGrid";
			this.ReceiptsGrid.Size = new System.Drawing.Size(926, 578);
			this.ReceiptsGrid.TabIndex = 14;
			this.ReceiptsGrid.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.ReceiptsGrid_CellValueChanged);
			this.ReceiptsGrid.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.ReceiptsGrid_CellEndEdit);
			this.ReceiptsGrid.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.ReceiptsGrid_DataError);
			this.ReceiptsGrid.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ReceiptsGrid_KeyUp);
			// 
			// folderBrowserDialog1
			// 
			this.folderBrowserDialog1.ShowNewFolderButton = false;
			// 
			// cbRecentFiles
			// 
			this.cbRecentFiles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbRecentFiles.FormattingEnabled = true;
			this.cbRecentFiles.Items.AddRange(new object[] {
            "קבצים אחרונים (בחר כדי לפתוח באקסל)"});
			this.cbRecentFiles.Location = new System.Drawing.Point(12, 614);
			this.cbRecentFiles.Name = "cbRecentFiles";
			this.cbRecentFiles.Size = new System.Drawing.Size(323, 21);
			this.cbRecentFiles.TabIndex = 15;
			this.cbRecentFiles.SelectedIndexChanged += new System.EventHandler(this.cbRecentFiles_SelectedIndexChanged);
			// 
			// lbError
			// 
			this.lbError.ForeColor = System.Drawing.Color.Red;
			this.lbError.Location = new System.Drawing.Point(644, 617);
			this.lbError.MaximumSize = new System.Drawing.Size(300, 39);
			this.lbError.MinimumSize = new System.Drawing.Size(300, 39);
			this.lbError.Name = "lbError";
			this.lbError.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.lbError.Size = new System.Drawing.Size(300, 39);
			this.lbError.TabIndex = 16;
			this.lbError.Text = "error";
			this.lbError.Visible = false;
			// 
			// pnlRecentFileDetails
			// 
			this.pnlRecentFileDetails.Controls.Add(this.btnReloadRecentFile);
			this.pnlRecentFileDetails.Controls.Add(this.lbRecentFileLastModified);
			this.pnlRecentFileDetails.Controls.Add(this.label1);
			this.pnlRecentFileDetails.Location = new System.Drawing.Point(12, 636);
			this.pnlRecentFileDetails.Name = "pnlRecentFileDetails";
			this.pnlRecentFileDetails.Size = new System.Drawing.Size(323, 26);
			this.pnlRecentFileDetails.TabIndex = 17;
			this.pnlRecentFileDetails.Visible = false;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(243, 2);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(83, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "תאריך שמירה:";
			// 
			// lbRecentFileLastModified
			// 
			this.lbRecentFileLastModified.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.lbRecentFileLastModified.ForeColor = System.Drawing.Color.Blue;
			this.lbRecentFileLastModified.Location = new System.Drawing.Point(154, 2);
			this.lbRecentFileLastModified.Name = "lbRecentFileLastModified";
			this.lbRecentFileLastModified.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.lbRecentFileLastModified.Size = new System.Drawing.Size(83, 13);
			this.lbRecentFileLastModified.TabIndex = 2;
			this.lbRecentFileLastModified.Text = "12/12/2012";
			this.lbRecentFileLastModified.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// btnReloadRecentFile
			// 
			this.btnReloadRecentFile.Alignment = System.Drawing.StringAlignment.Center;
			this.btnReloadRecentFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnReloadRecentFile.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.btnReloadRecentFile.Hue = 200F;
			this.btnReloadRecentFile.Image = null;
			this.btnReloadRecentFile.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btnReloadRecentFile.ImageList = null;
			this.btnReloadRecentFile.ImageSize = new System.Drawing.Size(0, 0);
			this.btnReloadRecentFile.Location = new System.Drawing.Point(71, 2);
			this.btnReloadRecentFile.Name = "btnReloadRecentFile";
			this.btnReloadRecentFile.Saturation = 0.9F;
			this.btnReloadRecentFile.Size = new System.Drawing.Size(77, 19);
			this.btnReloadRecentFile.TabIndex = 19;
			this.btnReloadRecentFile.Text = "טען מחדש";
			this.btnReloadRecentFile.Transparent = System.Drawing.Color.Black;
			this.btnReloadRecentFile.Click += new System.EventHandler(this.btnReloadRecentFile_Click);
			// 
			// tbCancel
			// 
			this.tbCancel.Alignment = System.Drawing.StringAlignment.Center;
			this.tbCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.tbCancel.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbCancel.Hue = 0F;
			this.tbCancel.Image = null;
			this.tbCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.tbCancel.ImageList = null;
			this.tbCancel.ImageSize = new System.Drawing.Size(0, 0);
			this.tbCancel.Location = new System.Drawing.Point(533, 613);
			this.tbCancel.Name = "tbCancel";
			this.tbCancel.Saturation = 0.9F;
			this.tbCancel.Size = new System.Drawing.Size(105, 35);
			this.tbCancel.TabIndex = 13;
			this.tbCancel.Text = "ביטול, חזרה למסך קבלות";
			this.tbCancel.Transparent = System.Drawing.Color.Black;
			this.tbCancel.Click += new System.EventHandler(this.tbCancel_Click);
			// 
			// tbOk
			// 
			this.tbOk.Alignment = System.Drawing.StringAlignment.Center;
			this.tbOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.tbOk.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbOk.Hue = 100F;
			this.tbOk.Image = null;
			this.tbOk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.tbOk.ImageList = null;
			this.tbOk.ImageSize = new System.Drawing.Size(0, 0);
			this.tbOk.Location = new System.Drawing.Point(354, 613);
			this.tbOk.Name = "tbOk";
			this.tbOk.Saturation = 0.9F;
			this.tbOk.Size = new System.Drawing.Size(103, 35);
			this.tbOk.TabIndex = 12;
			this.tbOk.Text = "אישור, ייצא לקובץ אקסל";
			this.tbOk.Transparent = System.Drawing.Color.Black;
			this.tbOk.Click += new System.EventHandler(this.tbOk_Click);
			// 
			// ExportReceiptsDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(950, 671);
			this.Controls.Add(this.pnlRecentFileDetails);
			this.Controls.Add(this.lbError);
			this.Controls.Add(this.cbRecentFiles);
			this.Controls.Add(this.ReceiptsGrid);
			this.Controls.Add(this.tbCancel);
			this.Controls.Add(this.tbOk);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ExportReceiptsDialog";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "ייצוא קבלות לחשבשבת";
			this.Load += new System.EventHandler(this.ExportReceiptsDialog_Load);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ExportReceiptsDialog_FormClosing);
			((System.ComponentModel.ISupportInitialize)(this.ReceiptsGrid)).EndInit();
			this.pnlRecentFileDetails.ResumeLayout(false);
			this.pnlRecentFileDetails.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private Sport.UI.Controls.ThemeButton tbCancel;
		private Sport.UI.Controls.ThemeButton tbOk;
		private System.Windows.Forms.DataGridView ReceiptsGrid;
		private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
		private System.Windows.Forms.ComboBox cbRecentFiles;
		private System.Windows.Forms.Label lbError;
		private System.Windows.Forms.Panel pnlRecentFileDetails;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label lbRecentFileLastModified;
		private Sport.UI.Controls.ThemeButton btnReloadRecentFile;
	}
}