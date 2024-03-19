namespace Sportsman.Forms
{
	partial class FixTeamRegionsDialog
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
			Sport.UI.Controls.GridDefaultSource gridDefaultSource3 = new Sport.UI.Controls.GridDefaultSource();
			this.tblTeams = new Sport.UI.Controls.Grid();
			this.btnLoadData = new System.Windows.Forms.Button();
			this.btnFixSelectedRow = new System.Windows.Forms.Button();
			this.btnFixAll = new System.Windows.Forms.Button();
			this.btnStopFixAll = new System.Windows.Forms.Button();
			this.lbLiveProgress = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// tblTeams
			// 
			this.tblTeams.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.tblTeams.Editable = true;
			this.tblTeams.ExpandOnDoubleClick = true;
			this.tblTeams.GroupingStyle = Sport.UI.Controls.GridGroupingStyle.Grid;
			this.tblTeams.HeaderHeight = 19;
			this.tblTeams.HorizontalLines = true;
			this.tblTeams.Location = new System.Drawing.Point(12, 47);
			this.tblTeams.Name = "tblTeams";
			this.tblTeams.SelectedIndex = -1;
			this.tblTeams.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.tblTeams.SelectOnSpace = false;
			this.tblTeams.ShowCheckBoxes = false;
			this.tblTeams.ShowRowNumber = false;
			this.tblTeams.Size = new System.Drawing.Size(999, 544);
			this.tblTeams.Source = gridDefaultSource3;
			this.tblTeams.TabIndex = 0;
			this.tblTeams.Text = "grid1";
			this.tblTeams.VerticalLines = true;
			this.tblTeams.VisibleRow = 0;
			// 
			// btnLoadData
			// 
			this.btnLoadData.Location = new System.Drawing.Point(12, 610);
			this.btnLoadData.Name = "btnLoadData";
			this.btnLoadData.Size = new System.Drawing.Size(98, 31);
			this.btnLoadData.TabIndex = 1;
			this.btnLoadData.Text = "טעינת נתונים";
			this.btnLoadData.UseVisualStyleBackColor = true;
			this.btnLoadData.Click += new System.EventHandler(this.btnLoadData_Click);
			// 
			// btnFixSelectedRow
			// 
			this.btnFixSelectedRow.Location = new System.Drawing.Point(154, 610);
			this.btnFixSelectedRow.Name = "btnFixSelectedRow";
			this.btnFixSelectedRow.Size = new System.Drawing.Size(130, 44);
			this.btnFixSelectedRow.TabIndex = 2;
			this.btnFixSelectedRow.Text = "תיקון שורה מסומנת";
			this.btnFixSelectedRow.UseVisualStyleBackColor = true;
			this.btnFixSelectedRow.Click += new System.EventHandler(this.btnFixSelectedRow_Click);
			// 
			// btnFixAll
			// 
			this.btnFixAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.btnFixAll.Location = new System.Drawing.Point(345, 610);
			this.btnFixAll.Name = "btnFixAll";
			this.btnFixAll.Size = new System.Drawing.Size(130, 44);
			this.btnFixAll.TabIndex = 3;
			this.btnFixAll.Text = "תיקון כל הקבוצות";
			this.btnFixAll.UseVisualStyleBackColor = true;
			this.btnFixAll.Click += new System.EventHandler(this.btnFixAll_Click);
			// 
			// btnStopFixAll
			// 
			this.btnStopFixAll.Location = new System.Drawing.Point(516, 610);
			this.btnStopFixAll.Name = "btnStopFixAll";
			this.btnStopFixAll.Size = new System.Drawing.Size(130, 44);
			this.btnStopFixAll.TabIndex = 4;
			this.btnStopFixAll.Text = "הפסקת תיקון גורף";
			this.btnStopFixAll.UseVisualStyleBackColor = true;
			this.btnStopFixAll.Visible = false;
			this.btnStopFixAll.Click += new System.EventHandler(this.btnStopFixAll_Click);
			// 
			// lbLiveProgress
			// 
			this.lbLiveProgress.Location = new System.Drawing.Point(669, 610);
			this.lbLiveProgress.Name = "lbLiveProgress";
			this.lbLiveProgress.Size = new System.Drawing.Size(342, 31);
			this.lbLiveProgress.TabIndex = 5;
			this.lbLiveProgress.Text = "live progress";
			this.lbLiveProgress.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lbLiveProgress.Visible = false;
			// 
			// FixTeamRegionsDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1023, 666);
			this.Controls.Add(this.lbLiveProgress);
			this.Controls.Add(this.btnStopFixAll);
			this.Controls.Add(this.btnFixAll);
			this.Controls.Add(this.btnFixSelectedRow);
			this.Controls.Add(this.btnLoadData);
			this.Controls.Add(this.tblTeams);
			this.Name = "FixTeamRegionsDialog";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.RightToLeftLayout = true;
			this.Text = "תיקון מחוזות של קבוצות";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FixTeamRegionsDialog_FormClosing);
			this.Load += new System.EventHandler(this.FixTeamRegionsDialog_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private Sport.UI.Controls.Grid tblTeams;
		private System.Windows.Forms.Button btnLoadData;
		private System.Windows.Forms.Button btnFixSelectedRow;
		private System.Windows.Forms.Button btnFixAll;
		private System.Windows.Forms.Button btnStopFixAll;
		private System.Windows.Forms.Label lbLiveProgress;
	}
}