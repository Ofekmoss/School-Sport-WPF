namespace Sportsman.Forms
{
	partial class ImportStudentsDialog
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
			this.txtFilePath = new System.Windows.Forms.TextBox();
			this.btnBrowse = new System.Windows.Forms.Button();
			this.txtProgress = new System.Windows.Forms.TextBox();
			this.btnStartImport = new System.Windows.Forms.Button();
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.gbColumns = new System.Windows.Forms.GroupBox();
			this.nudGenderColumn = new System.Windows.Forms.NumericUpDown();
			this.lbGender = new System.Windows.Forms.Label();
			this.nudSchoolColumn = new System.Windows.Forms.NumericUpDown();
			this.lbSchoolSymbol = new System.Windows.Forms.Label();
			this.nudBirthdayColumn = new System.Windows.Forms.NumericUpDown();
			this.lbBirthday = new System.Windows.Forms.Label();
			this.nudFirstNameColumn = new System.Windows.Forms.NumericUpDown();
			this.lbFirstName = new System.Windows.Forms.Label();
			this.nudLastNameColumn = new System.Windows.Forms.NumericUpDown();
			this.lbLastName = new System.Windows.Forms.Label();
			this.nudIdColumn = new System.Windows.Forms.NumericUpDown();
			this.lbIdNumber = new System.Windows.Forms.Label();
			this.btnOpenInExcel = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.lbVersion = new System.Windows.Forms.Label();
			this.gbColumns.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudGenderColumn)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudSchoolColumn)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudBirthdayColumn)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudFirstNameColumn)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudLastNameColumn)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudIdColumn)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(66, 17);
			this.label1.TabIndex = 0;
			this.label1.Text = "File path:";
			// 
			// txtFilePath
			// 
			this.txtFilePath.Location = new System.Drawing.Point(84, 6);
			this.txtFilePath.Name = "txtFilePath";
			this.txtFilePath.Size = new System.Drawing.Size(332, 22);
			this.txtFilePath.TabIndex = 1;
			this.txtFilePath.TextChanged += new System.EventHandler(this.txtFilePath_TextChanged);
			// 
			// btnBrowse
			// 
			this.btnBrowse.Location = new System.Drawing.Point(422, 6);
			this.btnBrowse.Name = "btnBrowse";
			this.btnBrowse.Size = new System.Drawing.Size(75, 23);
			this.btnBrowse.TabIndex = 2;
			this.btnBrowse.Text = "Browse";
			this.btnBrowse.UseVisualStyleBackColor = true;
			this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
			// 
			// txtProgress
			// 
			this.txtProgress.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtProgress.Location = new System.Drawing.Point(15, 45);
			this.txtProgress.Multiline = true;
			this.txtProgress.Name = "txtProgress";
			this.txtProgress.ReadOnly = true;
			this.txtProgress.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtProgress.Size = new System.Drawing.Size(480, 299);
			this.txtProgress.TabIndex = 3;
			// 
			// btnStartImport
			// 
			this.btnStartImport.Location = new System.Drawing.Point(15, 350);
			this.btnStartImport.Name = "btnStartImport";
			this.btnStartImport.Size = new System.Drawing.Size(97, 42);
			this.btnStartImport.TabIndex = 4;
			this.btnStartImport.Text = "Start Import";
			this.btnStartImport.UseVisualStyleBackColor = true;
			this.btnStartImport.Click += new System.EventHandler(this.btnStartImport_Click);
			// 
			// openFileDialog1
			// 
			this.openFileDialog1.FileName = "openFileDialog1";
			// 
			// gbColumns
			// 
			this.gbColumns.Controls.Add(this.nudGenderColumn);
			this.gbColumns.Controls.Add(this.lbGender);
			this.gbColumns.Controls.Add(this.nudSchoolColumn);
			this.gbColumns.Controls.Add(this.lbSchoolSymbol);
			this.gbColumns.Controls.Add(this.nudBirthdayColumn);
			this.gbColumns.Controls.Add(this.lbBirthday);
			this.gbColumns.Controls.Add(this.nudFirstNameColumn);
			this.gbColumns.Controls.Add(this.lbFirstName);
			this.gbColumns.Controls.Add(this.nudLastNameColumn);
			this.gbColumns.Controls.Add(this.lbLastName);
			this.gbColumns.Controls.Add(this.nudIdColumn);
			this.gbColumns.Controls.Add(this.lbIdNumber);
			this.gbColumns.Location = new System.Drawing.Point(503, 12);
			this.gbColumns.Name = "gbColumns";
			this.gbColumns.Size = new System.Drawing.Size(180, 279);
			this.gbColumns.TabIndex = 5;
			this.gbColumns.TabStop = false;
			this.gbColumns.Text = "Column Indices";
			// 
			// nudGenderColumn
			// 
			this.nudGenderColumn.Location = new System.Drawing.Point(107, 247);
			this.nudGenderColumn.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
			this.nudGenderColumn.Name = "nudGenderColumn";
			this.nudGenderColumn.Size = new System.Drawing.Size(51, 22);
			this.nudGenderColumn.TabIndex = 18;
			this.nudGenderColumn.Tag = "lbGender";
			this.nudGenderColumn.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
			// 
			// lbGender
			// 
			this.lbGender.AutoSize = true;
			this.lbGender.Location = new System.Drawing.Point(6, 247);
			this.lbGender.Name = "lbGender";
			this.lbGender.Size = new System.Drawing.Size(60, 17);
			this.lbGender.TabIndex = 17;
			this.lbGender.Text = "Gender:";
			// 
			// nudSchoolColumn
			// 
			this.nudSchoolColumn.Location = new System.Drawing.Point(107, 201);
			this.nudSchoolColumn.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
			this.nudSchoolColumn.Name = "nudSchoolColumn";
			this.nudSchoolColumn.Size = new System.Drawing.Size(51, 22);
			this.nudSchoolColumn.TabIndex = 16;
			this.nudSchoolColumn.Tag = "lbSchoolSymbol";
			this.nudSchoolColumn.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
			// 
			// lbSchoolSymbol
			// 
			this.lbSchoolSymbol.AutoSize = true;
			this.lbSchoolSymbol.Location = new System.Drawing.Point(6, 201);
			this.lbSchoolSymbol.Name = "lbSchoolSymbol";
			this.lbSchoolSymbol.Size = new System.Drawing.Size(103, 17);
			this.lbSchoolSymbol.TabIndex = 15;
			this.lbSchoolSymbol.Text = "School symbol:";
			// 
			// nudBirthdayColumn
			// 
			this.nudBirthdayColumn.Location = new System.Drawing.Point(107, 156);
			this.nudBirthdayColumn.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
			this.nudBirthdayColumn.Name = "nudBirthdayColumn";
			this.nudBirthdayColumn.Size = new System.Drawing.Size(51, 22);
			this.nudBirthdayColumn.TabIndex = 14;
			this.nudBirthdayColumn.Tag = "lbBirthday";
			this.nudBirthdayColumn.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
			// 
			// lbBirthday
			// 
			this.lbBirthday.AutoSize = true;
			this.lbBirthday.Location = new System.Drawing.Point(6, 156);
			this.lbBirthday.Name = "lbBirthday";
			this.lbBirthday.Size = new System.Drawing.Size(64, 17);
			this.lbBirthday.TabIndex = 13;
			this.lbBirthday.Text = "Birthday:";
			// 
			// nudFirstNameColumn
			// 
			this.nudFirstNameColumn.Location = new System.Drawing.Point(107, 113);
			this.nudFirstNameColumn.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
			this.nudFirstNameColumn.Name = "nudFirstNameColumn";
			this.nudFirstNameColumn.Size = new System.Drawing.Size(51, 22);
			this.nudFirstNameColumn.TabIndex = 12;
			this.nudFirstNameColumn.Tag = "lbFirstName";
			this.nudFirstNameColumn.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
			// 
			// lbFirstName
			// 
			this.lbFirstName.AutoSize = true;
			this.lbFirstName.Location = new System.Drawing.Point(6, 113);
			this.lbFirstName.Name = "lbFirstName";
			this.lbFirstName.Size = new System.Drawing.Size(78, 17);
			this.lbFirstName.TabIndex = 11;
			this.lbFirstName.Text = "First name:";
			// 
			// nudLastNameColumn
			// 
			this.nudLastNameColumn.Location = new System.Drawing.Point(107, 72);
			this.nudLastNameColumn.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
			this.nudLastNameColumn.Name = "nudLastNameColumn";
			this.nudLastNameColumn.Size = new System.Drawing.Size(51, 22);
			this.nudLastNameColumn.TabIndex = 10;
			this.nudLastNameColumn.Tag = "lbLastName";
			this.nudLastNameColumn.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// lbLastName
			// 
			this.lbLastName.AutoSize = true;
			this.lbLastName.Location = new System.Drawing.Point(6, 72);
			this.lbLastName.Name = "lbLastName";
			this.lbLastName.Size = new System.Drawing.Size(78, 17);
			this.lbLastName.TabIndex = 9;
			this.lbLastName.Text = "Last name:";
			// 
			// nudIdColumn
			// 
			this.nudIdColumn.Location = new System.Drawing.Point(107, 33);
			this.nudIdColumn.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
			this.nudIdColumn.Name = "nudIdColumn";
			this.nudIdColumn.Size = new System.Drawing.Size(51, 22);
			this.nudIdColumn.TabIndex = 8;
			this.nudIdColumn.Tag = "lbIdNumber";
			// 
			// lbIdNumber
			// 
			this.lbIdNumber.AutoSize = true;
			this.lbIdNumber.Location = new System.Drawing.Point(6, 33);
			this.lbIdNumber.Name = "lbIdNumber";
			this.lbIdNumber.Size = new System.Drawing.Size(77, 17);
			this.lbIdNumber.TabIndex = 7;
			this.lbIdNumber.Text = "ID number:";
			// 
			// btnOpenInExcel
			// 
			this.btnOpenInExcel.Location = new System.Drawing.Point(362, 350);
			this.btnOpenInExcel.Name = "btnOpenInExcel";
			this.btnOpenInExcel.Size = new System.Drawing.Size(115, 27);
			this.btnOpenInExcel.TabIndex = 10;
			this.btnOpenInExcel.Text = "Open in Excel";
			this.btnOpenInExcel.UseVisualStyleBackColor = true;
			this.btnOpenInExcel.Click += new System.EventHandler(this.btnOpenInExcel_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(586, 375);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(60, 17);
			this.label2.TabIndex = 11;
			this.label2.Text = "Version:";
			// 
			// lbVersion
			// 
			this.lbVersion.AutoSize = true;
			this.lbVersion.ForeColor = System.Drawing.Color.Blue;
			this.lbVersion.Location = new System.Drawing.Point(642, 375);
			this.lbVersion.Name = "lbVersion";
			this.lbVersion.Size = new System.Drawing.Size(28, 17);
			this.lbVersion.TabIndex = 12;
			this.lbVersion.Text = "0.0";
			// 
			// ImportStudentsDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(695, 398);
			this.Controls.Add(this.lbVersion);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.btnOpenInExcel);
			this.Controls.Add(this.gbColumns);
			this.Controls.Add(this.btnStartImport);
			this.Controls.Add(this.txtProgress);
			this.Controls.Add(this.btnBrowse);
			this.Controls.Add(this.txtFilePath);
			this.Controls.Add(this.label1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ImportStudentsDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "יבוא תלמידים";
			this.Load += new System.EventHandler(this.ImportStudentsDialog_Load);
			this.gbColumns.ResumeLayout(false);
			this.gbColumns.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudGenderColumn)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudSchoolColumn)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudBirthdayColumn)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudFirstNameColumn)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudLastNameColumn)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudIdColumn)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtFilePath;
		private System.Windows.Forms.Button btnBrowse;
		private System.Windows.Forms.TextBox txtProgress;
		private System.Windows.Forms.Button btnStartImport;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private System.Windows.Forms.GroupBox gbColumns;
		private System.Windows.Forms.NumericUpDown nudGenderColumn;
		private System.Windows.Forms.Label lbGender;
		private System.Windows.Forms.NumericUpDown nudSchoolColumn;
		private System.Windows.Forms.Label lbSchoolSymbol;
		private System.Windows.Forms.NumericUpDown nudBirthdayColumn;
		private System.Windows.Forms.Label lbBirthday;
		private System.Windows.Forms.NumericUpDown nudFirstNameColumn;
		private System.Windows.Forms.Label lbFirstName;
		private System.Windows.Forms.NumericUpDown nudLastNameColumn;
		private System.Windows.Forms.Label lbLastName;
		private System.Windows.Forms.NumericUpDown nudIdColumn;
		private System.Windows.Forms.Label lbIdNumber;
		private System.Windows.Forms.Button btnOpenInExcel;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label lbVersion;
	}
}