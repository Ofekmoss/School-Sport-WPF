namespace ImportBlackListedPlayers
{
	partial class Form1
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
			this.pnlIndices = new System.Windows.Forms.GroupBox();
			this.nudLastNameIndex = new System.Windows.Forms.NumericUpDown();
			this.lbLastNameIndexTitle = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.nudSportNameIndex = new System.Windows.Forms.NumericUpDown();
			this.lbSportNamendexTitle = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.nudFullNameIndex = new System.Windows.Forms.NumericUpDown();
			this.lbFullNameIndexTitle = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.nudIdNumberIndex = new System.Windows.Forms.NumericUpDown();
			this.lbIdNumberIndexTitle = new System.Windows.Forms.Label();
			this.tbOutput = new System.Windows.Forms.TextBox();
			this.tbFilePath = new System.Windows.Forms.TextBox();
			this.btnBrowse = new System.Windows.Forms.Button();
			this.btnAnalyzeFile = new System.Windows.Forms.Button();
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.lbRowProgress = new System.Windows.Forms.Label();
			this.btnStopImport = new System.Windows.Forms.Button();
			this.btnStartImport = new System.Windows.Forms.Button();
			this.nudImportFirstIndex = new System.Windows.Forms.NumericUpDown();
			this.label2 = new System.Windows.Forms.Label();
			this.nudImportLastIndex = new System.Windows.Forms.NumericUpDown();
			this.chkSilent = new System.Windows.Forms.CheckBox();
			this.pnlIndices.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudLastNameIndex)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudSportNameIndex)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudFullNameIndex)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudIdNumberIndex)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudImportFirstIndex)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudImportLastIndex)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(9, 7);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(34, 17);
			this.label1.TabIndex = 9;
			this.label1.Text = "File:";
			// 
			// pnlIndices
			// 
			this.pnlIndices.Controls.Add(this.nudLastNameIndex);
			this.pnlIndices.Controls.Add(this.lbLastNameIndexTitle);
			this.pnlIndices.Controls.Add(this.label5);
			this.pnlIndices.Controls.Add(this.nudSportNameIndex);
			this.pnlIndices.Controls.Add(this.lbSportNamendexTitle);
			this.pnlIndices.Controls.Add(this.label4);
			this.pnlIndices.Controls.Add(this.nudFullNameIndex);
			this.pnlIndices.Controls.Add(this.lbFullNameIndexTitle);
			this.pnlIndices.Controls.Add(this.label3);
			this.pnlIndices.Controls.Add(this.nudIdNumberIndex);
			this.pnlIndices.Controls.Add(this.lbIdNumberIndexTitle);
			this.pnlIndices.Location = new System.Drawing.Point(12, 32);
			this.pnlIndices.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.pnlIndices.Name = "pnlIndices";
			this.pnlIndices.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.pnlIndices.Size = new System.Drawing.Size(745, 57);
			this.pnlIndices.TabIndex = 15;
			this.pnlIndices.TabStop = false;
			this.pnlIndices.Text = "Indices";
			// 
			// nudLastNameIndex
			// 
			this.nudLastNameIndex.Location = new System.Drawing.Point(549, 25);
			this.nudLastNameIndex.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.nudLastNameIndex.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
			this.nudLastNameIndex.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
			this.nudLastNameIndex.Name = "nudLastNameIndex";
			this.nudLastNameIndex.Size = new System.Drawing.Size(52, 22);
			this.nudLastNameIndex.TabIndex = 19;
			// 
			// lbLastNameIndexTitle
			// 
			this.lbLastNameIndexTitle.AutoSize = true;
			this.lbLastNameIndexTitle.Location = new System.Drawing.Point(469, 25);
			this.lbLastNameIndexTitle.Name = "lbLastNameIndexTitle";
			this.lbLastNameIndexTitle.Size = new System.Drawing.Size(76, 17);
			this.lbLastNameIndexTitle.TabIndex = 18;
			this.lbLastNameIndexTitle.Text = "Last Name";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.label5.Location = new System.Drawing.Point(455, 12);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(26, 39);
			this.label5.TabIndex = 17;
			this.label5.Text = "|";
			// 
			// nudSportNameIndex
			// 
			this.nudSportNameIndex.Location = new System.Drawing.Point(395, 25);
			this.nudSportNameIndex.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.nudSportNameIndex.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
			this.nudSportNameIndex.Name = "nudSportNameIndex";
			this.nudSportNameIndex.Size = new System.Drawing.Size(52, 22);
			this.nudSportNameIndex.TabIndex = 16;
			// 
			// lbSportNamendexTitle
			// 
			this.lbSportNamendexTitle.AutoSize = true;
			this.lbSportNamendexTitle.Location = new System.Drawing.Point(309, 25);
			this.lbSportNamendexTitle.Name = "lbSportNamendexTitle";
			this.lbSportNamendexTitle.Size = new System.Drawing.Size(83, 17);
			this.lbSportNamendexTitle.TabIndex = 15;
			this.lbSportNamendexTitle.Text = "Sport Name";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.label4.Location = new System.Drawing.Point(291, 12);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(26, 39);
			this.label4.TabIndex = 14;
			this.label4.Text = "|";
			// 
			// nudFullNameIndex
			// 
			this.nudFullNameIndex.Location = new System.Drawing.Point(235, 25);
			this.nudFullNameIndex.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.nudFullNameIndex.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
			this.nudFullNameIndex.Name = "nudFullNameIndex";
			this.nudFullNameIndex.Size = new System.Drawing.Size(52, 22);
			this.nudFullNameIndex.TabIndex = 13;
			// 
			// lbFullNameIndexTitle
			// 
			this.lbFullNameIndexTitle.AutoSize = true;
			this.lbFullNameIndexTitle.Location = new System.Drawing.Point(156, 25);
			this.lbFullNameIndexTitle.Name = "lbFullNameIndexTitle";
			this.lbFullNameIndexTitle.Size = new System.Drawing.Size(71, 17);
			this.lbFullNameIndexTitle.TabIndex = 12;
			this.lbFullNameIndexTitle.Text = "Full Name";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.label3.Location = new System.Drawing.Point(141, 12);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(26, 39);
			this.label3.TabIndex = 11;
			this.label3.Text = "|";
			// 
			// nudIdNumberIndex
			// 
			this.nudIdNumberIndex.Location = new System.Drawing.Point(83, 25);
			this.nudIdNumberIndex.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.nudIdNumberIndex.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
			this.nudIdNumberIndex.Name = "nudIdNumberIndex";
			this.nudIdNumberIndex.Size = new System.Drawing.Size(52, 22);
			this.nudIdNumberIndex.TabIndex = 9;
			this.nudIdNumberIndex.Tag = "";
			// 
			// lbIdNumberIndexTitle
			// 
			this.lbIdNumberIndexTitle.AutoSize = true;
			this.lbIdNumberIndexTitle.Location = new System.Drawing.Point(5, 25);
			this.lbIdNumberIndexTitle.Name = "lbIdNumberIndexTitle";
			this.lbIdNumberIndexTitle.Size = new System.Drawing.Size(73, 17);
			this.lbIdNumberIndexTitle.TabIndex = 7;
			this.lbIdNumberIndexTitle.Text = "Id Number";
			// 
			// tbOutput
			// 
			this.tbOutput.Location = new System.Drawing.Point(12, 94);
			this.tbOutput.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.tbOutput.Multiline = true;
			this.tbOutput.Name = "tbOutput";
			this.tbOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.tbOutput.Size = new System.Drawing.Size(744, 562);
			this.tbOutput.TabIndex = 13;
			// 
			// tbFilePath
			// 
			this.tbFilePath.Location = new System.Drawing.Point(49, 4);
			this.tbFilePath.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.tbFilePath.Name = "tbFilePath";
			this.tbFilePath.Size = new System.Drawing.Size(399, 22);
			this.tbFilePath.TabIndex = 10;
			// 
			// btnBrowse
			// 
			this.btnBrowse.Location = new System.Drawing.Point(453, 4);
			this.btnBrowse.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.btnBrowse.Name = "btnBrowse";
			this.btnBrowse.Size = new System.Drawing.Size(75, 23);
			this.btnBrowse.TabIndex = 11;
			this.btnBrowse.Text = "Browse...";
			this.btnBrowse.UseVisualStyleBackColor = true;
			this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
			// 
			// btnAnalyzeFile
			// 
			this.btnAnalyzeFile.Location = new System.Drawing.Point(13, 661);
			this.btnAnalyzeFile.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.btnAnalyzeFile.Name = "btnAnalyzeFile";
			this.btnAnalyzeFile.Size = new System.Drawing.Size(100, 32);
			this.btnAnalyzeFile.TabIndex = 12;
			this.btnAnalyzeFile.Text = "Analyze File";
			this.btnAnalyzeFile.UseVisualStyleBackColor = true;
			this.btnAnalyzeFile.Click += new System.EventHandler(this.btnAnalyzeFile_Click);
			// 
			// openFileDialog1
			// 
			this.openFileDialog1.FileName = "openFileDialog1";
			// 
			// lbRowProgress
			// 
			this.lbRowProgress.AutoSize = true;
			this.lbRowProgress.Location = new System.Drawing.Point(570, 661);
			this.lbRowProgress.Name = "lbRowProgress";
			this.lbRowProgress.Size = new System.Drawing.Size(28, 17);
			this.lbRowProgress.TabIndex = 16;
			this.lbRowProgress.Text = "0/0";
			this.lbRowProgress.Visible = false;
			// 
			// btnStopImport
			// 
			this.btnStopImport.Location = new System.Drawing.Point(657, 661);
			this.btnStopImport.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.btnStopImport.Name = "btnStopImport";
			this.btnStopImport.Size = new System.Drawing.Size(100, 32);
			this.btnStopImport.TabIndex = 17;
			this.btnStopImport.Text = "Stop Import";
			this.btnStopImport.UseVisualStyleBackColor = true;
			this.btnStopImport.Click += new System.EventHandler(this.btnStopImport_Click);
			// 
			// btnStartImport
			// 
			this.btnStartImport.Location = new System.Drawing.Point(348, 662);
			this.btnStartImport.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.btnStartImport.Name = "btnStartImport";
			this.btnStartImport.Size = new System.Drawing.Size(100, 32);
			this.btnStartImport.TabIndex = 18;
			this.btnStartImport.Text = "Start Import";
			this.btnStartImport.UseVisualStyleBackColor = true;
			this.btnStartImport.Click += new System.EventHandler(this.btnStartImport_Click);
			// 
			// nudImportFirstIndex
			// 
			this.nudImportFirstIndex.Location = new System.Drawing.Point(129, 668);
			this.nudImportFirstIndex.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.nudImportFirstIndex.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
			this.nudImportFirstIndex.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.nudImportFirstIndex.Name = "nudImportFirstIndex";
			this.nudImportFirstIndex.Size = new System.Drawing.Size(87, 22);
			this.nudImportFirstIndex.TabIndex = 20;
			this.nudImportFirstIndex.Tag = "";
			this.nudImportFirstIndex.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(221, 673);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(21, 17);
			this.label2.TabIndex = 21;
			this.label2.Text = " - ";
			this.label2.Visible = false;
			// 
			// nudImportLastIndex
			// 
			this.nudImportLastIndex.Location = new System.Drawing.Point(244, 668);
			this.nudImportLastIndex.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.nudImportLastIndex.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
			this.nudImportLastIndex.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.nudImportLastIndex.Name = "nudImportLastIndex";
			this.nudImportLastIndex.Size = new System.Drawing.Size(87, 22);
			this.nudImportLastIndex.TabIndex = 22;
			this.nudImportLastIndex.Tag = "";
			this.nudImportLastIndex.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// chkSilent
			// 
			this.chkSilent.AutoSize = true;
			this.chkSilent.Location = new System.Drawing.Point(459, 673);
			this.chkSilent.Name = "chkSilent";
			this.chkSilent.Size = new System.Drawing.Size(73, 21);
			this.chkSilent.TabIndex = 23;
			this.chkSilent.Text = "Silent?";
			this.chkSilent.UseVisualStyleBackColor = true;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(772, 724);
			this.Controls.Add(this.chkSilent);
			this.Controls.Add(this.nudImportLastIndex);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.nudImportFirstIndex);
			this.Controls.Add(this.btnStartImport);
			this.Controls.Add(this.btnStopImport);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.pnlIndices);
			this.Controls.Add(this.tbOutput);
			this.Controls.Add(this.tbFilePath);
			this.Controls.Add(this.btnBrowse);
			this.Controls.Add(this.btnAnalyzeFile);
			this.Controls.Add(this.lbRowProgress);
			this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.Name = "Form1";
			this.Text = "Form1";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.pnlIndices.ResumeLayout(false);
			this.pnlIndices.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudLastNameIndex)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudSportNameIndex)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudFullNameIndex)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudIdNumberIndex)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudImportFirstIndex)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudImportLastIndex)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox pnlIndices;
		public System.Windows.Forms.NumericUpDown nudSportNameIndex;
		private System.Windows.Forms.Label lbSportNamendexTitle;
		private System.Windows.Forms.Label label4;
		public System.Windows.Forms.NumericUpDown nudFullNameIndex;
		private System.Windows.Forms.Label lbFullNameIndexTitle;
		private System.Windows.Forms.Label label3;
		public System.Windows.Forms.NumericUpDown nudIdNumberIndex;
		private System.Windows.Forms.Label lbIdNumberIndexTitle;
		private System.Windows.Forms.TextBox tbOutput;
		private System.Windows.Forms.TextBox tbFilePath;
		private System.Windows.Forms.Button btnBrowse;
		private System.Windows.Forms.Button btnAnalyzeFile;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private System.Windows.Forms.Label lbRowProgress;
		private System.Windows.Forms.Button btnStopImport;
		public System.Windows.Forms.NumericUpDown nudLastNameIndex;
		private System.Windows.Forms.Label lbLastNameIndexTitle;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Button btnStartImport;
		public System.Windows.Forms.NumericUpDown nudImportFirstIndex;
		private System.Windows.Forms.Label label2;
		public System.Windows.Forms.NumericUpDown nudImportLastIndex;
		private System.Windows.Forms.CheckBox chkSilent;
	}
}

