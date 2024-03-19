namespace ImportSportFlowersPlayers
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
			this.tbFilePath = new System.Windows.Forms.TextBox();
			this.btnBrowse = new System.Windows.Forms.Button();
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.btnAnalyzeFile = new System.Windows.Forms.Button();
			this.tbOutput = new System.Windows.Forms.TextBox();
			this.cbMunicipalities = new System.Windows.Forms.ComboBox();
			this.pnlIndices = new System.Windows.Forms.GroupBox();
			this.nudBirthdateIndex = new System.Windows.Forms.NumericUpDown();
			this.lbBirthdateIndexTitle = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.nudGenderIndex = new System.Windows.Forms.NumericUpDown();
			this.lbGenderIndexTitle = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.nudFirstNameIndex = new System.Windows.Forms.NumericUpDown();
			this.lbFirstNameIndexTitle = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.nudLastNameIndex = new System.Windows.Forms.NumericUpDown();
			this.lbLastNameIndexTitle = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.nudIdNumberIndex = new System.Windows.Forms.NumericUpDown();
			this.lbIdNumberIndexTitle = new System.Windows.Forms.Label();
			this.lbRowProgress = new System.Windows.Forms.Label();
			this.pnlIndices.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudBirthdateIndex)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudGenderIndex)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudFirstNameIndex)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudLastNameIndex)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudIdNumberIndex)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(34, 17);
			this.label1.TabIndex = 0;
			this.label1.Text = "File:";
			// 
			// tbFilePath
			// 
			this.tbFilePath.Location = new System.Drawing.Point(52, 6);
			this.tbFilePath.Name = "tbFilePath";
			this.tbFilePath.Size = new System.Drawing.Size(398, 22);
			this.tbFilePath.TabIndex = 1;
			// 
			// btnBrowse
			// 
			this.btnBrowse.Location = new System.Drawing.Point(456, 6);
			this.btnBrowse.Name = "btnBrowse";
			this.btnBrowse.Size = new System.Drawing.Size(75, 23);
			this.btnBrowse.TabIndex = 2;
			this.btnBrowse.Text = "Browse...";
			this.btnBrowse.UseVisualStyleBackColor = true;
			this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
			// 
			// openFileDialog1
			// 
			this.openFileDialog1.FileName = "openFileDialog1";
			// 
			// btnAnalyzeFile
			// 
			this.btnAnalyzeFile.Location = new System.Drawing.Point(15, 663);
			this.btnAnalyzeFile.Name = "btnAnalyzeFile";
			this.btnAnalyzeFile.Size = new System.Drawing.Size(100, 32);
			this.btnAnalyzeFile.TabIndex = 3;
			this.btnAnalyzeFile.Text = "Start Import";
			this.btnAnalyzeFile.UseVisualStyleBackColor = true;
			this.btnAnalyzeFile.Click += new System.EventHandler(this.btnAnalyzeFile_Click);
			// 
			// tbOutput
			// 
			this.tbOutput.Location = new System.Drawing.Point(15, 96);
			this.tbOutput.Multiline = true;
			this.tbOutput.Name = "tbOutput";
			this.tbOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.tbOutput.Size = new System.Drawing.Size(716, 561);
			this.tbOutput.TabIndex = 4;
			// 
			// cbMunicipalities
			// 
			this.cbMunicipalities.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbMunicipalities.FormattingEnabled = true;
			this.cbMunicipalities.Location = new System.Drawing.Point(537, 6);
			this.cbMunicipalities.Name = "cbMunicipalities";
			this.cbMunicipalities.Size = new System.Drawing.Size(195, 24);
			this.cbMunicipalities.TabIndex = 5;
			// 
			// pnlIndices
			// 
			this.pnlIndices.Controls.Add(this.nudBirthdateIndex);
			this.pnlIndices.Controls.Add(this.lbBirthdateIndexTitle);
			this.pnlIndices.Controls.Add(this.label8);
			this.pnlIndices.Controls.Add(this.nudGenderIndex);
			this.pnlIndices.Controls.Add(this.lbGenderIndexTitle);
			this.pnlIndices.Controls.Add(this.label6);
			this.pnlIndices.Controls.Add(this.nudFirstNameIndex);
			this.pnlIndices.Controls.Add(this.lbFirstNameIndexTitle);
			this.pnlIndices.Controls.Add(this.label4);
			this.pnlIndices.Controls.Add(this.nudLastNameIndex);
			this.pnlIndices.Controls.Add(this.lbLastNameIndexTitle);
			this.pnlIndices.Controls.Add(this.label3);
			this.pnlIndices.Controls.Add(this.nudIdNumberIndex);
			this.pnlIndices.Controls.Add(this.lbIdNumberIndexTitle);
			this.pnlIndices.Location = new System.Drawing.Point(15, 34);
			this.pnlIndices.Name = "pnlIndices";
			this.pnlIndices.Size = new System.Drawing.Size(725, 56);
			this.pnlIndices.TabIndex = 7;
			this.pnlIndices.TabStop = false;
			this.pnlIndices.Text = "Indices";
			// 
			// nudBirthdateIndex
			// 
			this.nudBirthdateIndex.Location = new System.Drawing.Point(664, 25);
			this.nudBirthdateIndex.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
			this.nudBirthdateIndex.Name = "nudBirthdateIndex";
			this.nudBirthdateIndex.Size = new System.Drawing.Size(52, 22);
			this.nudBirthdateIndex.TabIndex = 22;
			// 
			// lbBirthdateIndexTitle
			// 
			this.lbBirthdateIndexTitle.AutoSize = true;
			this.lbBirthdateIndexTitle.Location = new System.Drawing.Point(587, 25);
			this.lbBirthdateIndexTitle.Name = "lbBirthdateIndexTitle";
			this.lbBirthdateIndexTitle.Size = new System.Drawing.Size(65, 17);
			this.lbBirthdateIndexTitle.TabIndex = 21;
			this.lbBirthdateIndexTitle.Text = "Birthdate";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.label8.Location = new System.Drawing.Point(567, 12);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(26, 39);
			this.label8.TabIndex = 20;
			this.label8.Text = "|";
			// 
			// nudGenderIndex
			// 
			this.nudGenderIndex.Location = new System.Drawing.Point(519, 23);
			this.nudGenderIndex.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
			this.nudGenderIndex.Name = "nudGenderIndex";
			this.nudGenderIndex.Size = new System.Drawing.Size(52, 22);
			this.nudGenderIndex.TabIndex = 19;
			// 
			// lbGenderIndexTitle
			// 
			this.lbGenderIndexTitle.AutoSize = true;
			this.lbGenderIndexTitle.Location = new System.Drawing.Point(465, 25);
			this.lbGenderIndexTitle.Name = "lbGenderIndexTitle";
			this.lbGenderIndexTitle.Size = new System.Drawing.Size(56, 17);
			this.lbGenderIndexTitle.TabIndex = 18;
			this.lbGenderIndexTitle.Text = "Gender";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.label6.Location = new System.Drawing.Point(445, 12);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(26, 39);
			this.label6.TabIndex = 17;
			this.label6.Text = "|";
			// 
			// nudFirstNameIndex
			// 
			this.nudFirstNameIndex.Location = new System.Drawing.Point(387, 25);
			this.nudFirstNameIndex.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
			this.nudFirstNameIndex.Name = "nudFirstNameIndex";
			this.nudFirstNameIndex.Size = new System.Drawing.Size(52, 22);
			this.nudFirstNameIndex.TabIndex = 16;
			// 
			// lbFirstNameIndexTitle
			// 
			this.lbFirstNameIndexTitle.AutoSize = true;
			this.lbFirstNameIndexTitle.Location = new System.Drawing.Point(310, 25);
			this.lbFirstNameIndexTitle.Name = "lbFirstNameIndexTitle";
			this.lbFirstNameIndexTitle.Size = new System.Drawing.Size(76, 17);
			this.lbFirstNameIndexTitle.TabIndex = 15;
			this.lbFirstNameIndexTitle.Text = "First Name";
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
			// nudLastNameIndex
			// 
			this.nudLastNameIndex.Location = new System.Drawing.Point(233, 25);
			this.nudLastNameIndex.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
			this.nudLastNameIndex.Name = "nudLastNameIndex";
			this.nudLastNameIndex.Size = new System.Drawing.Size(52, 22);
			this.nudLastNameIndex.TabIndex = 13;
			// 
			// lbLastNameIndexTitle
			// 
			this.lbLastNameIndexTitle.AutoSize = true;
			this.lbLastNameIndexTitle.Location = new System.Drawing.Point(156, 25);
			this.lbLastNameIndexTitle.Name = "lbLastNameIndexTitle";
			this.lbLastNameIndexTitle.Size = new System.Drawing.Size(76, 17);
			this.lbLastNameIndexTitle.TabIndex = 12;
			this.lbLastNameIndexTitle.Text = "Last Name";
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
			this.lbIdNumberIndexTitle.Location = new System.Drawing.Point(6, 25);
			this.lbIdNumberIndexTitle.Name = "lbIdNumberIndexTitle";
			this.lbIdNumberIndexTitle.Size = new System.Drawing.Size(73, 17);
			this.lbIdNumberIndexTitle.TabIndex = 7;
			this.lbIdNumberIndexTitle.Text = "Id Number";
			// 
			// lbRowProgress
			// 
			this.lbRowProgress.AutoSize = true;
			this.lbRowProgress.Location = new System.Drawing.Point(136, 663);
			this.lbRowProgress.Name = "lbRowProgress";
			this.lbRowProgress.Size = new System.Drawing.Size(28, 17);
			this.lbRowProgress.TabIndex = 8;
			this.lbRowProgress.Text = "0/0";
			this.lbRowProgress.Visible = false;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(744, 703);
			this.Controls.Add(this.lbRowProgress);
			this.Controls.Add(this.pnlIndices);
			this.Controls.Add(this.cbMunicipalities);
			this.Controls.Add(this.tbOutput);
			this.Controls.Add(this.btnAnalyzeFile);
			this.Controls.Add(this.btnBrowse);
			this.Controls.Add(this.tbFilePath);
			this.Controls.Add(this.label1);
			this.Name = "Form1";
			this.Text = "Sport Flowers population import";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.pnlIndices.ResumeLayout(false);
			this.pnlIndices.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudBirthdateIndex)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudGenderIndex)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudFirstNameIndex)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudLastNameIndex)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudIdNumberIndex)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox tbFilePath;
		private System.Windows.Forms.Button btnBrowse;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private System.Windows.Forms.Button btnAnalyzeFile;
		private System.Windows.Forms.TextBox tbOutput;
		private System.Windows.Forms.ComboBox cbMunicipalities;
		private System.Windows.Forms.GroupBox pnlIndices;
		private System.Windows.Forms.Label lbIdNumberIndexTitle;
		private System.Windows.Forms.Label lbBirthdateIndexTitle;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label lbGenderIndexTitle;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label lbFirstNameIndexTitle;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label lbLastNameIndexTitle;
		private System.Windows.Forms.Label label3;
		public System.Windows.Forms.NumericUpDown nudBirthdateIndex;
		public System.Windows.Forms.NumericUpDown nudGenderIndex;
		public System.Windows.Forms.NumericUpDown nudFirstNameIndex;
		public System.Windows.Forms.NumericUpDown nudLastNameIndex;
		public System.Windows.Forms.NumericUpDown nudIdNumberIndex;
		private System.Windows.Forms.Label lbRowProgress;
	}
}

