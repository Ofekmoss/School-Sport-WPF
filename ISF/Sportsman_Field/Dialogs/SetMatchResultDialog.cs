using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Sport.Common;

namespace Sportsman_Field.Dialogs
{
	/// <summary>
	/// Summary description for SetMatchResultDialog.
	/// </summary>
	public class SetMatchResultDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label lbTeamName_A;
		private System.Windows.Forms.NumericUpDown nudTeamScore_A;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.NumericUpDown nudTeamScore_B;
		private System.Windows.Forms.Label lbTeamName_B;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOK;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.CheckBox cbTechnicalWin_A;
		private System.Windows.Forms.CheckBox cbTechnicalWin_B;

		private MatchData _match=null;

		public SetMatchResultDialog(MatchData match)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			_match = match;
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(SetMatchResultDialog));
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.lbTeamName_A = new System.Windows.Forms.Label();
			this.nudTeamScore_A = new System.Windows.Forms.NumericUpDown();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.nudTeamScore_B = new System.Windows.Forms.NumericUpDown();
			this.lbTeamName_B = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.cbTechnicalWin_A = new System.Windows.Forms.CheckBox();
			this.cbTechnicalWin_B = new System.Windows.Forms.CheckBox();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudTeamScore_A)).BeginInit();
			this.groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudTeamScore_B)).BeginInit();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.cbTechnicalWin_A);
			this.groupBox1.Controls.Add(this.nudTeamScore_A);
			this.groupBox1.Controls.Add(this.lbTeamName_A);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Location = new System.Drawing.Point(288, 8);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(272, 88);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "קבוצה א\'";
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label1.Location = new System.Drawing.Point(192, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(72, 23);
			this.label1.TabIndex = 0;
			this.label1.Text = "שם קבוצה: ";
			// 
			// label2
			// 
			this.label2.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label2.Location = new System.Drawing.Point(192, 56);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(72, 23);
			this.label2.TabIndex = 1;
			this.label2.Text = "תוצאה: ";
			// 
			// lbTeamName_A
			// 
			this.lbTeamName_A.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbTeamName_A.ForeColor = System.Drawing.Color.Blue;
			this.lbTeamName_A.Location = new System.Drawing.Point(8, 24);
			this.lbTeamName_A.Name = "lbTeamName_A";
			this.lbTeamName_A.Size = new System.Drawing.Size(180, 23);
			this.lbTeamName_A.TabIndex = 2;
			this.lbTeamName_A.Text = "עירוני ט\' ראשון לציון א\'";
			// 
			// nudTeamScore_A
			// 
			this.nudTeamScore_A.DecimalPlaces = 1;
			this.nudTeamScore_A.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.nudTeamScore_A.ForeColor = System.Drawing.Color.Blue;
			this.nudTeamScore_A.Location = new System.Drawing.Point(120, 56);
			this.nudTeamScore_A.Maximum = new System.Decimal(new int[] {
																		   999,
																		   0,
																		   0,
																		   0});
			this.nudTeamScore_A.Name = "nudTeamScore_A";
			this.nudTeamScore_A.Size = new System.Drawing.Size(64, 22);
			this.nudTeamScore_A.TabIndex = 1;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.cbTechnicalWin_B);
			this.groupBox2.Controls.Add(this.nudTeamScore_B);
			this.groupBox2.Controls.Add(this.lbTeamName_B);
			this.groupBox2.Controls.Add(this.label4);
			this.groupBox2.Controls.Add(this.label5);
			this.groupBox2.Location = new System.Drawing.Point(8, 8);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(272, 88);
			this.groupBox2.TabIndex = 99;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "קבוצה ב\'";
			this.groupBox2.Enter += new System.EventHandler(this.groupBox2_Enter);
			// 
			// nudTeamScore_B
			// 
			this.nudTeamScore_B.DecimalPlaces = 1;
			this.nudTeamScore_B.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.nudTeamScore_B.ForeColor = System.Drawing.Color.Blue;
			this.nudTeamScore_B.Location = new System.Drawing.Point(120, 56);
			this.nudTeamScore_B.Maximum = new System.Decimal(new int[] {
																		   999,
																		   0,
																		   0,
																		   0});
			this.nudTeamScore_B.Name = "nudTeamScore_B";
			this.nudTeamScore_B.Size = new System.Drawing.Size(64, 22);
			this.nudTeamScore_B.TabIndex = 2;
			// 
			// lbTeamName_B
			// 
			this.lbTeamName_B.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbTeamName_B.ForeColor = System.Drawing.Color.Blue;
			this.lbTeamName_B.Location = new System.Drawing.Point(8, 24);
			this.lbTeamName_B.Name = "lbTeamName_B";
			this.lbTeamName_B.Size = new System.Drawing.Size(180, 23);
			this.lbTeamName_B.TabIndex = 2;
			this.lbTeamName_B.Text = "עירוני ט\' ראשון לציון א\'";
			// 
			// label4
			// 
			this.label4.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label4.Location = new System.Drawing.Point(192, 56);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(72, 23);
			this.label4.TabIndex = 1;
			this.label4.Text = "תוצאה: ";
			// 
			// label5
			// 
			this.label5.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label5.Location = new System.Drawing.Point(192, 24);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(72, 23);
			this.label5.TabIndex = 0;
			this.label5.Text = "שם קבוצה: ";
			// 
			// btnCancel
			// 
			this.btnCancel.BackColor = System.Drawing.Color.White;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnCancel.ForeColor = System.Drawing.Color.Red;
			this.btnCancel.Image = ((System.Drawing.Image)(resources.GetObject("btnCancel.Image")));
			this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btnCancel.Location = new System.Drawing.Point(16, 112);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(88, 23);
			this.btnCancel.TabIndex = 6;
			this.btnCancel.Text = "ביטול";
			// 
			// btnOK
			// 
			this.btnOK.BackColor = System.Drawing.Color.White;
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnOK.ForeColor = System.Drawing.Color.Black;
			this.btnOK.Image = ((System.Drawing.Image)(resources.GetObject("btnOK.Image")));
			this.btnOK.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btnOK.Location = new System.Drawing.Point(128, 112);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(80, 23);
			this.btnOK.TabIndex = 3;
			this.btnOK.Text = "אישור";
			// 
			// cbTechnicalWin_A
			// 
			this.cbTechnicalWin_A.ForeColor = System.Drawing.Color.Blue;
			this.cbTechnicalWin_A.Location = new System.Drawing.Point(5, 56);
			this.cbTechnicalWin_A.Name = "cbTechnicalWin_A";
			this.cbTechnicalWin_A.TabIndex = 3;
			this.cbTechnicalWin_A.Text = "ניצחון טכני";
			this.cbTechnicalWin_A.CheckedChanged += new System.EventHandler(this.cbTechnicalWin_A_CheckedChanged);
			// 
			// cbTechnicalWin_B
			// 
			this.cbTechnicalWin_B.ForeColor = System.Drawing.Color.Blue;
			this.cbTechnicalWin_B.Location = new System.Drawing.Point(8, 56);
			this.cbTechnicalWin_B.Name = "cbTechnicalWin_B";
			this.cbTechnicalWin_B.TabIndex = 4;
			this.cbTechnicalWin_B.Text = "ניצחון טכני";
			this.cbTechnicalWin_B.CheckedChanged += new System.EventHandler(this.cbTechnicalWin_B_CheckedChanged);
			// 
			// SetMatchResultDialog
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
			this.BackColor = System.Drawing.Color.White;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(570, 151);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SetMatchResultDialog";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "קביעת תוצאת משחק בודד";
			this.Load += new System.EventHandler(this.SetMatchResultDialog_Load);
			this.groupBox1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.nudTeamScore_A)).EndInit();
			this.groupBox2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.nudTeamScore_B)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		private void groupBox2_Enter(object sender, System.EventArgs e)
		{
		
		}

		private void SetMatchResultDialog_Load(object sender, System.EventArgs e)
		{
			lbTeamName_A.Text = _match.TeamA.School.Name;
			if (_match.TeamA.TeamIndex > 0)
				lbTeamName_A.Text += Tools.ToHebLetter(_match.TeamA.TeamIndex);
			lbTeamName_B.Text = _match.TeamB.School.Name;
			if (_match.TeamB.TeamIndex > 0)
				lbTeamName_B.Text += Tools.ToHebLetter(_match.TeamB.TeamIndex);
			nudTeamScore_A.Value = (decimal) _match.TeamA_Score;
			nudTeamScore_B.Value = (decimal) _match.TeamB_Score;
			switch (_match.Result)
			{
				case (int) Sport.Championships.MatchOutcome.TechnicalA:
					//cbTechnicalWin_A_CheckedChanged(cbTechnicalWin_A, EventArgs.Empty);
					cbTechnicalWin_A.Checked = true;
					break;
				case (int) Sport.Championships.MatchOutcome.TechnicalB:
					cbTechnicalWin_B.Checked = true;
					break;
			}
		}

		private void cbTechnicalWin_A_CheckedChanged(object sender, System.EventArgs e)
		{
			cbTechnicalWin_B.Enabled = !(sender as CheckBox).Checked;
			nudTeamScore_A.Enabled = !(sender as CheckBox).Checked;
			nudTeamScore_B.Enabled = !(sender as CheckBox).Checked;
		}

		private void cbTechnicalWin_B_CheckedChanged(object sender, System.EventArgs e)
		{
			cbTechnicalWin_A.Enabled = !(sender as CheckBox).Checked;
			nudTeamScore_A.Enabled = !(sender as CheckBox).Checked;
			nudTeamScore_B.Enabled = !(sender as CheckBox).Checked;
		}

		public double TeamA_Score
		{
			get
			{
				return (double) nudTeamScore_A.Value;
			}
		}

		public double TeamB_Score
		{
			get
			{
				return (double) nudTeamScore_B.Value;
			}
		}

		public bool TeachnicalWin_A
		{
			get
			{
				return cbTechnicalWin_A.Checked;
			}
		}
		
		public bool TeachnicalWin_B
		{
			get
			{
				return cbTechnicalWin_B.Checked;
			}
		}
	}
}
