using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Sport.Common;
using Sport.Core;

namespace Sportsman.Forms
{
	/// <summary>
	/// Summary description for GeneralSettingsForm.
	/// </summary>
	public class GeneralSettingsForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.NumericUpDown nudTimeAddition;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button button1;
		private Label label3;
		private ComboBox cbRefereeReportSupervisor;
		private Label label4;
		private NumericUpDown nudPlayerCardTopMargin;
		private Label label5;
		private Label label6;
		private NumericUpDown nudPlayerCardLineHeight;
		private Label label7;
		private Label label8;
		private NumericUpDown nudPlayerCardVerticalGap;
		private Label label9;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public GeneralSettingsForm()
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
			this.label1 = new System.Windows.Forms.Label();
			this.nudTimeAddition = new System.Windows.Forms.NumericUpDown();
			this.label2 = new System.Windows.Forms.Label();
			this.button1 = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.cbRefereeReportSupervisor = new System.Windows.Forms.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.nudPlayerCardTopMargin = new System.Windows.Forms.NumericUpDown();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.nudPlayerCardLineHeight = new System.Windows.Forms.NumericUpDown();
			this.label7 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.nudPlayerCardVerticalGap = new System.Windows.Forms.NumericUpDown();
			this.label9 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.nudTimeAddition)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudPlayerCardTopMargin)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudPlayerCardLineHeight)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudPlayerCardVerticalGap)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(240, 13);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(217, 23);
			this.label1.TabIndex = 0;
			this.label1.Text = "תוספת זמן בעת שינוי מועד משחקים:";
			// 
			// nudTimeAddition
			// 
			this.nudTimeAddition.Location = new System.Drawing.Point(152, 14);
			this.nudTimeAddition.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.nudTimeAddition.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
			this.nudTimeAddition.Name = "nudTimeAddition";
			this.nudTimeAddition.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.nudTimeAddition.Size = new System.Drawing.Size(89, 23);
			this.nudTimeAddition.TabIndex = 1;
			this.nudTimeAddition.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.nudTimeAddition.UpDownAlign = System.Windows.Forms.LeftRightAlignment.Left;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(112, 14);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(39, 16);
			this.label2.TabIndex = 2;
			this.label2.Text = "שעות";
			// 
			// button1
			// 
			this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button1.Location = new System.Drawing.Point(192, 226);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(101, 28);
			this.button1.TabIndex = 3;
			this.button1.Text = "אישור";
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(240, 48);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(217, 23);
			this.label3.TabIndex = 4;
			this.label3.Text = " רכז מחוזי בדו\"ח שיבוץ שופטים:";
			// 
			// cbRefereeReportSupervisor
			// 
			this.cbRefereeReportSupervisor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbRefereeReportSupervisor.FormattingEnabled = true;
			this.cbRefereeReportSupervisor.Items.AddRange(new object[] {
            "רכז מחוז המוגדר בתוכנה",
            "משתמש המחובר כרגע לתוכנה"});
			this.cbRefereeReportSupervisor.Location = new System.Drawing.Point(8, 48);
			this.cbRefereeReportSupervisor.Name = "cbRefereeReportSupervisor";
			this.cbRefereeReportSupervisor.Size = new System.Drawing.Size(233, 24);
			this.cbRefereeReportSupervisor.TabIndex = 5;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(89, 92);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(57, 16);
			this.label4.TabIndex = 8;
			this.label4.Text = "פיקסלים";
			// 
			// nudPlayerCardTopMargin
			// 
			this.nudPlayerCardTopMargin.Location = new System.Drawing.Point(152, 86);
			this.nudPlayerCardTopMargin.Maximum = new decimal(new int[] {
            120,
            0,
            0,
            0});
			this.nudPlayerCardTopMargin.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
			this.nudPlayerCardTopMargin.Name = "nudPlayerCardTopMargin";
			this.nudPlayerCardTopMargin.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.nudPlayerCardTopMargin.Size = new System.Drawing.Size(89, 23);
			this.nudPlayerCardTopMargin.TabIndex = 7;
			this.nudPlayerCardTopMargin.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.nudPlayerCardTopMargin.UpDownAlign = System.Windows.Forms.LeftRightAlignment.Left;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(240, 85);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(217, 23);
			this.label5.TabIndex = 6;
			this.label5.Text = "מרווח עליון של כרטיסי שחקן:";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(89, 128);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(57, 16);
			this.label6.TabIndex = 11;
			this.label6.Text = "פיקסלים";
			// 
			// nudPlayerCardLineHeight
			// 
			this.nudPlayerCardLineHeight.Location = new System.Drawing.Point(152, 122);
			this.nudPlayerCardLineHeight.Maximum = new decimal(new int[] {
            120,
            0,
            0,
            0});
			this.nudPlayerCardLineHeight.Name = "nudPlayerCardLineHeight";
			this.nudPlayerCardLineHeight.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.nudPlayerCardLineHeight.Size = new System.Drawing.Size(89, 23);
			this.nudPlayerCardLineHeight.TabIndex = 10;
			this.nudPlayerCardLineHeight.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.nudPlayerCardLineHeight.UpDownAlign = System.Windows.Forms.LeftRightAlignment.Left;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(240, 121);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(217, 23);
			this.label7.TabIndex = 9;
			this.label7.Text = "גובה שורה בכרטיס שחקן:";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(89, 164);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(57, 16);
			this.label8.TabIndex = 14;
			this.label8.Text = "פיקסלים";
			// 
			// nudPlayerCardVerticalGap
			// 
			this.nudPlayerCardVerticalGap.Location = new System.Drawing.Point(152, 158);
			this.nudPlayerCardVerticalGap.Maximum = new decimal(new int[] {
            120,
            0,
            0,
            0});
			this.nudPlayerCardVerticalGap.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
			this.nudPlayerCardVerticalGap.Name = "nudPlayerCardVerticalGap";
			this.nudPlayerCardVerticalGap.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.nudPlayerCardVerticalGap.Size = new System.Drawing.Size(89, 23);
			this.nudPlayerCardVerticalGap.TabIndex = 13;
			this.nudPlayerCardVerticalGap.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.nudPlayerCardVerticalGap.UpDownAlign = System.Windows.Forms.LeftRightAlignment.Left;
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(240, 157);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(217, 23);
			this.label9.TabIndex = 12;
			this.label9.Text = "מרווח בין שורות בכרטיס שחקן:";
			// 
			// GeneralSettingsForm
			// 
			this.AcceptButton = this.button1;
			this.AutoScaleBaseSize = new System.Drawing.Size(7, 16);
			this.ClientSize = new System.Drawing.Size(602, 341);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.nudPlayerCardVerticalGap);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.nudPlayerCardLineHeight);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.nudPlayerCardTopMargin);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.cbRefereeReportSupervisor);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.nudTimeAddition);
			this.Controls.Add(this.label1);
			this.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "GeneralSettingsForm";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "הגדרות כלליות";
			this.Load += new System.EventHandler(this.GeneralSettingsForm_Load);
			((System.ComponentModel.ISupportInitialize)(this.nudTimeAddition)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudPlayerCardTopMargin)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudPlayerCardLineHeight)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudPlayerCardVerticalGap)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private void GeneralSettingsForm_Load(object sender, System.EventArgs e)
		{
			int timeAddition = Tools.CIntDef(Configuration.ReadString("GeneralSettings", "TimeAddition"), 0);
			int playerCardTopMargin = Tools.CIntDef(Configuration.ReadString("GeneralSettings", "PlayerCardTopMargin"), 0);
			int playerCardLineHeight = Tools.CIntDef(Configuration.ReadString("GeneralSettings", "PlayerCardLineHeight"), 15);
			int playerCardVerticalGap = Tools.CIntDef(Configuration.ReadString("GeneralSettings", "PlayerCardVerticalGap"), 7);
			nudTimeAddition.Value = (decimal) timeAddition;
			nudPlayerCardTopMargin.Value = (decimal)playerCardTopMargin;
			nudPlayerCardLineHeight.Value = (decimal)playerCardLineHeight;
			nudPlayerCardVerticalGap.Value = (decimal)playerCardVerticalGap;
			int refereeReportSupervisorIndex = Tools.CIntDef(Configuration.ReadString("GeneralSettings", "RefereeReportSupervisor"), 0);
			if (refereeReportSupervisorIndex >= cbRefereeReportSupervisor.Items.Count)
				refereeReportSupervisorIndex = 0;
			cbRefereeReportSupervisor.SelectedIndex = refereeReportSupervisorIndex;
		}

		private void button1_Click(object sender, System.EventArgs e)
		{
			Configuration.WriteString("GeneralSettings", "TimeAddition", nudTimeAddition.Value.ToString());
			Configuration.WriteString("GeneralSettings", "RefereeReportSupervisor", cbRefereeReportSupervisor.SelectedIndex.ToString());
			Configuration.WriteString("GeneralSettings", "PlayerCardTopMargin", nudPlayerCardTopMargin.Value.ToString());
			Configuration.WriteString("GeneralSettings", "PlayerCardLineHeight", nudPlayerCardLineHeight.Value.ToString());
			Configuration.WriteString("GeneralSettings", "PlayerCardVerticalGap", nudPlayerCardVerticalGap.Value.ToString());
		}
	}
}
