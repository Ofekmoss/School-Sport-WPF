using System;
using Sport.Rulesets.Rules;
using Sport.Rulesets;
using System.Drawing;
using System.Windows.Forms;
using Sport.Producer.UI.Rules.Dialogs;

namespace Sport.Producer.UI.Rules.Dialogs
{
	public class TeamPhaseScoringDialog : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.NumericUpDown nudResults;
		private System.Windows.Forms.Label labelResults;
		private System.Windows.Forms.Button btnUpdate;
		private System.Windows.Forms.Button btnDown;
		private System.Windows.Forms.Panel panelScoringPlan;
		private System.Windows.Forms.Label labelCounter;
		private System.Windows.Forms.TextBox tbCounterName;
		private System.Windows.Forms.ListBox lbScoringPlanCounters;
		private System.Windows.Forms.Label labelScoringPlan;
		private System.Windows.Forms.ComboBox cbScoringPlan;
		private System.Windows.Forms.Button btnRemoveCounter;
		private System.Windows.Forms.Button btnAddCounter;
		private System.Windows.Forms.Button btnRemovePlan;
		private System.Windows.Forms.Button btnAddPlan;
		private System.Windows.Forms.RadioButton rbCounters;
		private System.Windows.Forms.RadioButton rbMultiChallenge;
		private System.Windows.Forms.Panel panelCounters;
		private System.Windows.Forms.Label labelAdditionalResults;
		private System.Windows.Forms.NumericUpDown nudAdditionalResults;
		private System.Windows.Forms.Label labelMultiChallengeResults;
		private System.Windows.Forms.NumericUpDown nudMultiChallengeResults;
		private System.Windows.Forms.Panel panelMultiChallenge;
		private System.Windows.Forms.Button btnRename;
		private System.Windows.Forms.Button btnUp;

		public TeamPhaseScoringDialog(TeamPhaseScoring teamPhaseScoring)
		{
			if (teamPhaseScoring == null)
				_teamPhaseScoring = new TeamPhaseScoring();
			else
				_teamPhaseScoring = (TeamPhaseScoring) teamPhaseScoring.Clone();
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			SetScoringPlans();
			SetPlanType();
			SetCounters();

			SetButtonState();
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
			this.panel = new System.Windows.Forms.Panel();
			this.btnRemovePlan = new System.Windows.Forms.Button();
			this.btnAddPlan = new System.Windows.Forms.Button();
			this.cbScoringPlan = new System.Windows.Forms.ComboBox();
			this.labelScoringPlan = new System.Windows.Forms.Label();
			this.panelScoringPlan = new System.Windows.Forms.Panel();
			this.labelAdditionalResults = new System.Windows.Forms.Label();
			this.nudAdditionalResults = new System.Windows.Forms.NumericUpDown();
			this.rbMultiChallenge = new System.Windows.Forms.RadioButton();
			this.rbCounters = new System.Windows.Forms.RadioButton();
			this.panelMultiChallenge = new System.Windows.Forms.Panel();
			this.labelMultiChallengeResults = new System.Windows.Forms.Label();
			this.nudMultiChallengeResults = new System.Windows.Forms.NumericUpDown();
			this.panelCounters = new System.Windows.Forms.Panel();
			this.btnDown = new System.Windows.Forms.Button();
			this.btnUp = new System.Windows.Forms.Button();
			this.btnUpdate = new System.Windows.Forms.Button();
			this.labelResults = new System.Windows.Forms.Label();
			this.labelCounter = new System.Windows.Forms.Label();
			this.nudResults = new System.Windows.Forms.NumericUpDown();
			this.tbCounterName = new System.Windows.Forms.TextBox();
			this.btnRemoveCounter = new System.Windows.Forms.Button();
			this.btnAddCounter = new System.Windows.Forms.Button();
			this.lbScoringPlanCounters = new System.Windows.Forms.ListBox();
			this.tbOk = new System.Windows.Forms.Button();
			this.tbCancel = new System.Windows.Forms.Button();
			this.btnRename = new System.Windows.Forms.Button();
			this.panel.SuspendLayout();
			this.panelScoringPlan.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudAdditionalResults)).BeginInit();
			this.panelMultiChallenge.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudMultiChallengeResults)).BeginInit();
			this.panelCounters.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudResults)).BeginInit();
			this.SuspendLayout();
			// 
			// panel
			// 
			this.panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel.Controls.Add(this.btnRemovePlan);
			this.panel.Controls.Add(this.btnAddPlan);
			this.panel.Controls.Add(this.cbScoringPlan);
			this.panel.Controls.Add(this.labelScoringPlan);
			this.panel.Controls.Add(this.panelScoringPlan);
			this.panel.Controls.Add(this.tbOk);
			this.panel.Controls.Add(this.tbCancel);
			this.panel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.panel.Location = new System.Drawing.Point(0, 0);
			this.panel.Name = "panel";
			this.panel.Size = new System.Drawing.Size(264, 296);
			this.panel.TabIndex = 7;
			this.panel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelMouseUp);
			this.panel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelMouseMove);
			this.panel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelMouseDown);
			// 
			// btnRemovePlan
			// 
			this.btnRemovePlan.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnRemovePlan.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnRemovePlan.Location = new System.Drawing.Point(28, 8);
			this.btnRemovePlan.Name = "btnRemovePlan";
			this.btnRemovePlan.Size = new System.Drawing.Size(21, 21);
			this.btnRemovePlan.TabIndex = 27;
			this.btnRemovePlan.Text = "-";
			this.btnRemovePlan.Click += new System.EventHandler(this.btnRemovePlan_Click);
			// 
			// btnAddPlan
			// 
			this.btnAddPlan.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnAddPlan.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnAddPlan.Location = new System.Drawing.Point(8, 8);
			this.btnAddPlan.Name = "btnAddPlan";
			this.btnAddPlan.Size = new System.Drawing.Size(21, 21);
			this.btnAddPlan.TabIndex = 26;
			this.btnAddPlan.Text = "+";
			this.btnAddPlan.Click += new System.EventHandler(this.btnAddPlan_Click);
			// 
			// cbScoringPlan
			// 
			this.cbScoringPlan.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbScoringPlan.Location = new System.Drawing.Point(52, 8);
			this.cbScoringPlan.Name = "cbScoringPlan";
			this.cbScoringPlan.Size = new System.Drawing.Size(132, 21);
			this.cbScoringPlan.TabIndex = 25;
			this.cbScoringPlan.SelectedIndexChanged += new System.EventHandler(this.cbScoringPlan_SelectedIndexChanged);
			// 
			// labelScoringPlan
			// 
			this.labelScoringPlan.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelScoringPlan.Location = new System.Drawing.Point(186, 13);
			this.labelScoringPlan.Name = "labelScoringPlan";
			this.labelScoringPlan.Size = new System.Drawing.Size(70, 16);
			this.labelScoringPlan.TabIndex = 24;
			this.labelScoringPlan.Text = "שיטת ניקוד:";
			// 
			// panelScoringPlan
			// 
			this.panelScoringPlan.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panelScoringPlan.Controls.Add(this.btnRename);
			this.panelScoringPlan.Controls.Add(this.labelAdditionalResults);
			this.panelScoringPlan.Controls.Add(this.nudAdditionalResults);
			this.panelScoringPlan.Controls.Add(this.rbMultiChallenge);
			this.panelScoringPlan.Controls.Add(this.rbCounters);
			this.panelScoringPlan.Controls.Add(this.panelMultiChallenge);
			this.panelScoringPlan.Controls.Add(this.panelCounters);
			this.panelScoringPlan.Location = new System.Drawing.Point(8, 36);
			this.panelScoringPlan.Name = "panelScoringPlan";
			this.panelScoringPlan.Size = new System.Drawing.Size(248, 224);
			this.panelScoringPlan.TabIndex = 23;
			// 
			// labelAdditionalResults
			// 
			this.labelAdditionalResults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelAdditionalResults.Location = new System.Drawing.Point(142, 201);
			this.labelAdditionalResults.Name = "labelAdditionalResults";
			this.labelAdditionalResults.Size = new System.Drawing.Size(96, 16);
			this.labelAdditionalResults.TabIndex = 27;
			this.labelAdditionalResults.Text = "תוצאות נוספות:";
			// 
			// nudAdditionalResults
			// 
			this.nudAdditionalResults.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.nudAdditionalResults.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.nudAdditionalResults.Location = new System.Drawing.Point(8, 195);
			this.nudAdditionalResults.Name = "nudAdditionalResults";
			this.nudAdditionalResults.Size = new System.Drawing.Size(94, 21);
			this.nudAdditionalResults.TabIndex = 26;
			this.nudAdditionalResults.ValueChanged += new System.EventHandler(this.nudAdditionalResults_ValueChanged);
			// 
			// rbMultiChallenge
			// 
			this.rbMultiChallenge.Location = new System.Drawing.Point(112, 2);
			this.rbMultiChallenge.Name = "rbMultiChallenge";
			this.rbMultiChallenge.Size = new System.Drawing.Size(64, 22);
			this.rbMultiChallenge.TabIndex = 24;
			this.rbMultiChallenge.Text = "קרב רב";
			// 
			// rbCounters
			// 
			this.rbCounters.Checked = true;
			this.rbCounters.Location = new System.Drawing.Point(183, 2);
			this.rbCounters.Name = "rbCounters";
			this.rbCounters.Size = new System.Drawing.Size(56, 22);
			this.rbCounters.TabIndex = 23;
			this.rbCounters.TabStop = true;
			this.rbCounters.Text = "מונים";
			this.rbCounters.CheckedChanged += new System.EventHandler(this.PlanTypeCheckChanged);
			// 
			// panelMultiChallenge
			// 
			this.panelMultiChallenge.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panelMultiChallenge.Controls.Add(this.labelMultiChallengeResults);
			this.panelMultiChallenge.Controls.Add(this.nudMultiChallengeResults);
			this.panelMultiChallenge.Location = new System.Drawing.Point(8, 24);
			this.panelMultiChallenge.Name = "panelMultiChallenge";
			this.panelMultiChallenge.Size = new System.Drawing.Size(232, 168);
			this.panelMultiChallenge.TabIndex = 28;
			// 
			// labelMultiChallengeResults
			// 
			this.labelMultiChallengeResults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelMultiChallengeResults.Location = new System.Drawing.Point(126, 16);
			this.labelMultiChallengeResults.Name = "labelMultiChallengeResults";
			this.labelMultiChallengeResults.Size = new System.Drawing.Size(96, 16);
			this.labelMultiChallengeResults.TabIndex = 29;
			this.labelMultiChallengeResults.Text = "תוצאות גבוהות:";
			// 
			// nudMultiChallengeResults
			// 
			this.nudMultiChallengeResults.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.nudMultiChallengeResults.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.nudMultiChallengeResults.Location = new System.Drawing.Point(6, 8);
			this.nudMultiChallengeResults.Name = "nudMultiChallengeResults";
			this.nudMultiChallengeResults.Size = new System.Drawing.Size(92, 21);
			this.nudMultiChallengeResults.TabIndex = 28;
			this.nudMultiChallengeResults.ValueChanged += new System.EventHandler(this.nudMultiChallengeResults_ValueChanged);
			// 
			// panelCounters
			// 
			this.panelCounters.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panelCounters.Controls.Add(this.btnDown);
			this.panelCounters.Controls.Add(this.btnUp);
			this.panelCounters.Controls.Add(this.btnUpdate);
			this.panelCounters.Controls.Add(this.labelResults);
			this.panelCounters.Controls.Add(this.labelCounter);
			this.panelCounters.Controls.Add(this.nudResults);
			this.panelCounters.Controls.Add(this.tbCounterName);
			this.panelCounters.Controls.Add(this.btnRemoveCounter);
			this.panelCounters.Controls.Add(this.btnAddCounter);
			this.panelCounters.Controls.Add(this.lbScoringPlanCounters);
			this.panelCounters.Location = new System.Drawing.Point(8, 24);
			this.panelCounters.Name = "panelCounters";
			this.panelCounters.Size = new System.Drawing.Size(232, 168);
			this.panelCounters.TabIndex = 25;
			// 
			// btnDown
			// 
			this.btnDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnDown.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnDown.Location = new System.Drawing.Point(4, 32);
			this.btnDown.Name = "btnDown";
			this.btnDown.Size = new System.Drawing.Size(41, 20);
			this.btnDown.TabIndex = 22;
			this.btnDown.Text = "הורד";
			this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
			// 
			// btnUp
			// 
			this.btnUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnUp.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnUp.Location = new System.Drawing.Point(44, 32);
			this.btnUp.Name = "btnUp";
			this.btnUp.Size = new System.Drawing.Size(41, 20);
			this.btnUp.TabIndex = 21;
			this.btnUp.Text = "עלה";
			this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
			// 
			// btnUpdate
			// 
			this.btnUpdate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnUpdate.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnUpdate.Location = new System.Drawing.Point(44, 8);
			this.btnUpdate.Name = "btnUpdate";
			this.btnUpdate.Size = new System.Drawing.Size(41, 21);
			this.btnUpdate.TabIndex = 19;
			this.btnUpdate.Text = "עדכן";
			this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
			// 
			// labelResults
			// 
			this.labelResults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelResults.Location = new System.Drawing.Point(188, 40);
			this.labelResults.Name = "labelResults";
			this.labelResults.Size = new System.Drawing.Size(40, 16);
			this.labelResults.TabIndex = 18;
			this.labelResults.Text = "כמות:";
			// 
			// labelCounter
			// 
			this.labelCounter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelCounter.Location = new System.Drawing.Point(196, 16);
			this.labelCounter.Name = "labelCounter";
			this.labelCounter.Size = new System.Drawing.Size(32, 16);
			this.labelCounter.TabIndex = 17;
			this.labelCounter.Text = "מונה:";
			// 
			// nudResults
			// 
			this.nudResults.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.nudResults.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.nudResults.Location = new System.Drawing.Point(87, 32);
			this.nudResults.Minimum = new System.Decimal(new int[] {
																	   1,
																	   0,
																	   0,
																	   0});
			this.nudResults.Name = "nudResults";
			this.nudResults.Size = new System.Drawing.Size(94, 21);
			this.nudResults.TabIndex = 16;
			this.nudResults.Value = new System.Decimal(new int[] {
																	 1,
																	 0,
																	 0,
																	 0});
			this.nudResults.TextChanged += new System.EventHandler(this.nudResults_TextChanged);
			// 
			// tbCounterName
			// 
			this.tbCounterName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.tbCounterName.AutoSize = false;
			this.tbCounterName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tbCounterName.Location = new System.Drawing.Point(87, 8);
			this.tbCounterName.Name = "tbCounterName";
			this.tbCounterName.Size = new System.Drawing.Size(94, 21);
			this.tbCounterName.TabIndex = 15;
			this.tbCounterName.Text = "";
			this.tbCounterName.TextChanged += new System.EventHandler(this.tbCounterName_TextChanged);
			// 
			// btnRemoveCounter
			// 
			this.btnRemoveCounter.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnRemoveCounter.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnRemoveCounter.Location = new System.Drawing.Point(24, 8);
			this.btnRemoveCounter.Name = "btnRemoveCounter";
			this.btnRemoveCounter.Size = new System.Drawing.Size(21, 21);
			this.btnRemoveCounter.TabIndex = 8;
			this.btnRemoveCounter.Text = "-";
			this.btnRemoveCounter.Click += new System.EventHandler(this.btnRemoveCounter_Click);
			// 
			// btnAddCounter
			// 
			this.btnAddCounter.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnAddCounter.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnAddCounter.Location = new System.Drawing.Point(4, 8);
			this.btnAddCounter.Name = "btnAddCounter";
			this.btnAddCounter.Size = new System.Drawing.Size(21, 21);
			this.btnAddCounter.TabIndex = 7;
			this.btnAddCounter.Text = "+";
			this.btnAddCounter.Click += new System.EventHandler(this.btnAddCounter_Click);
			// 
			// lbScoringPlanCounters
			// 
			this.lbScoringPlanCounters.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.lbScoringPlanCounters.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lbScoringPlanCounters.Location = new System.Drawing.Point(4, 56);
			this.lbScoringPlanCounters.Name = "lbScoringPlanCounters";
			this.lbScoringPlanCounters.Size = new System.Drawing.Size(222, 106);
			this.lbScoringPlanCounters.TabIndex = 9;
			this.lbScoringPlanCounters.SelectedIndexChanged += new System.EventHandler(this.SelectedCounterChanged);
			// 
			// tbOk
			// 
			this.tbOk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.tbOk.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.tbOk.Location = new System.Drawing.Point(8, 268);
			this.tbOk.Name = "tbOk";
			this.tbOk.Size = new System.Drawing.Size(64, 20);
			this.tbOk.TabIndex = 11;
			this.tbOk.Text = "אישור";
			this.tbOk.Click += new System.EventHandler(this.tbOk_Click);
			// 
			// tbCancel
			// 
			this.tbCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.tbCancel.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.tbCancel.Location = new System.Drawing.Point(76, 268);
			this.tbCancel.Name = "tbCancel";
			this.tbCancel.Size = new System.Drawing.Size(64, 20);
			this.tbCancel.TabIndex = 10;
			this.tbCancel.Text = "ביטול";
			this.tbCancel.Click += new System.EventHandler(this.tbCancel_Click);
			// 
			// btnRename
			// 
			this.btnRename.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnRename.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnRename.Location = new System.Drawing.Point(8, 2);
			this.btnRename.Name = "btnRename";
			this.btnRename.Size = new System.Drawing.Size(56, 21);
			this.btnRename.TabIndex = 29;
			this.btnRename.Text = "שנה שם";
			this.btnRename.Click += new System.EventHandler(this.btnRename_Click);
			// 
			// TeamPhaseScoringDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(264, 296);
			this.ControlBox = false;
			this.Controls.Add(this.panel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "TeamPhaseScoringDialog";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "שדות";
			this.panel.ResumeLayout(false);
			this.panelScoringPlan.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.nudAdditionalResults)).EndInit();
			this.panelMultiChallenge.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.nudMultiChallengeResults)).EndInit();
			this.panelCounters.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.nudResults)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		private System.Windows.Forms.Button tbOk;
		private System.Windows.Forms.Button tbCancel;
		private System.Windows.Forms.Panel panel;


		private TeamPhaseScoring _teamPhaseScoring;
		public TeamPhaseScoring TeamPhaseScoring
		{
			get { return _teamPhaseScoring; }
		}

		private void SetScoringPlans()
		{
			object selectedPlan = cbScoringPlan.SelectedItem;

			cbScoringPlan.Items.Clear();
			foreach (ScoringPlan scoringPlan in _teamPhaseScoring.ScoringPlans)
			{
				cbScoringPlan.Items.Add(scoringPlan);
			}

			cbScoringPlan.SelectedItem = selectedPlan;
		}

		private void SetPlanType()
		{
			ScoringPlan scoringPlan = cbScoringPlan.SelectedItem as ScoringPlan;
			if (scoringPlan != null)
			{
				panelCounters.Visible = scoringPlan.PlanType == ScoringPlanType.Counters;
				panelMultiChallenge.Visible = scoringPlan.PlanType == ScoringPlanType.MultiChallenge;
				rbCounters.Checked = scoringPlan.PlanType == ScoringPlanType.Counters;
				rbMultiChallenge.Checked = scoringPlan.PlanType == ScoringPlanType.MultiChallenge;
			}
			else
			{
				panelCounters.Visible = true;
				rbCounters.Checked = true;
				panelMultiChallenge.Visible = false;
				rbMultiChallenge.Checked = false;
			}
		}

		private void SetCounters()
		{
			lbScoringPlanCounters.Items.Clear();

			ScoringPlan scoringPlan = cbScoringPlan.SelectedItem as ScoringPlan;

			if (scoringPlan != null)
			{
				foreach (ScoringPlan.Counter counter in scoringPlan.Counters)
				{
					lbScoringPlanCounters.Items.Add(counter);
				}
			}
		}
        

		private void tbOk_Click(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.OK;
			Close();
		}

		private void tbCancel_Click(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}

		private void btnAddCounter_Click(object sender, System.EventArgs e)
		{
			if (tbCounterName.Text.Length > 0 && nudResults.Value > 0)
			{
				ScoringPlan scoringPlan = cbScoringPlan.SelectedItem as ScoringPlan;
				if (scoringPlan != null)
				{
					ScoringPlan.Counter c = new ScoringPlan.Counter(tbCounterName.Text, (int) nudResults.Value);
					scoringPlan.Counters.Add(c);
					lbScoringPlanCounters.Items.Add(c);
				}
			}

			SetButtonState();
		}

		private void btnRemoveCounter_Click(object sender, System.EventArgs e)
		{
			ScoringPlan scoringPlan = cbScoringPlan.SelectedItem as ScoringPlan;

			if (scoringPlan != null)
			{

				ScoringPlan.Counter c = lbScoringPlanCounters.SelectedItem as ScoringPlan.Counter;

				if (c != null)
				{
					scoringPlan.Counters.Remove(c);
					lbScoringPlanCounters.Items.Remove(c);
				}

				SetButtonState();
			}
		}

		private void btnUpdate_Click(object sender, System.EventArgs e)
		{
			ScoringPlan.Counter c = lbScoringPlanCounters.SelectedItem as ScoringPlan.Counter;

			if (c != null && tbCounterName.Text.Length > 0 && nudResults.Value > 0)
			{
				c.Name = tbCounterName.Text;
				c.Results = (int) nudResults.Value;
				int si = lbScoringPlanCounters.SelectedIndex;
				SetCounters();
				lbScoringPlanCounters.SelectedIndex = si;
			}

			SetButtonState();
		}


		private void SetButtonState()
		{
			ScoringPlan scoringPlan = cbScoringPlan.SelectedItem as ScoringPlan;
			rbCounters.Enabled = scoringPlan != null;
			rbMultiChallenge.Enabled = scoringPlan != null;
			btnRemovePlan.Enabled = scoringPlan != null;
			btnRename.Enabled = scoringPlan != null;
			bool countersEnabled = scoringPlan != null && scoringPlan.PlanType == ScoringPlanType.Counters;
			btnRemoveCounter.Enabled = countersEnabled && (lbScoringPlanCounters.SelectedIndex != -1);
			btnAddCounter.Enabled = countersEnabled && ((tbCounterName.Text.Length > 0)&&(nudResults.Value > 0));
			btnUpdate.Enabled = countersEnabled && (btnRemoveCounter.Enabled && btnAddCounter.Enabled);
			btnUp.Enabled = countersEnabled && (lbScoringPlanCounters.SelectedIndex > 0);
			btnDown.Enabled = countersEnabled && ((lbScoringPlanCounters.SelectedIndex >= 0)&&(lbScoringPlanCounters.SelectedIndex < lbScoringPlanCounters.Items.Count-1));
			nudAdditionalResults.Enabled = scoringPlan != null;
			nudMultiChallengeResults.Enabled  = scoringPlan != null && scoringPlan.PlanType == ScoringPlanType.MultiChallenge;
		}

		private void SelectedCounterChanged(object sender, EventArgs e)
		{
			ScoringPlan.Counter c = lbScoringPlanCounters.SelectedItem as ScoringPlan.Counter;
			if (c != null)
			{
				tbCounterName.Text = c.Name;
				nudResults.Value = c.Results;
			}

			SetButtonState();
		}

		bool buttonDown = false;
		Point buttonPoint;

		protected override void OnLostFocus(EventArgs e)
		{
			buttonDown = false;
			base.OnLostFocus (e);
		}

		private void panelMouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (buttonDown)
			{
				Point pt = PointToScreen(new Point(e.X, e.Y));
				Location = new Point(pt.X - buttonPoint.X, pt.Y - buttonPoint.Y);
			}
		
		}

		private void panelMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				buttonDown = true;
				buttonPoint = new Point(e.X, e.Y);
			}
		}

		private void panelMouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			buttonDown = false;
		}


	
		public static bool EditTeamPhaseScoring(ref TeamPhaseScoring tps)
		{
			TeamPhaseScoringDialog tpsd = new TeamPhaseScoringDialog(tps);

			if (tpsd.ShowDialog() == DialogResult.OK)
			{
				tps = (TeamPhaseScoring) tpsd.TeamPhaseScoring.Clone();

				return true;
			}

			return false;
		}

		public static object ValueSelector(Sport.UI.Controls.ButtonBox buttonBox, object value)
		{
			TeamPhaseScoring tps = value as TeamPhaseScoring;
			if (EditTeamPhaseScoring(ref tps))
				return tps;
			return value;
		}

		private void tbCounterName_TextChanged(object sender, System.EventArgs e)
		{
			SetButtonState();
		}

		private void nudResults_TextChanged(object sender, System.EventArgs e)
		{
			SetButtonState();
		}

		private void btnDown_Click(object sender, System.EventArgs e)
		{
			int selIndex=lbScoringPlanCounters.SelectedIndex;
			if ((selIndex < 0)||(selIndex >= lbScoringPlanCounters.Items.Count-1))
				return;
			SwitchCounters(selIndex, selIndex+1);
			SetCounters();
			lbScoringPlanCounters.SelectedIndex = selIndex+1;
		}

		private void btnUp_Click(object sender, System.EventArgs e)
		{
			int selIndex=lbScoringPlanCounters.SelectedIndex;
			if (selIndex <= 0)
				return;
			SwitchCounters(selIndex, selIndex-1);
			SetCounters();
			lbScoringPlanCounters.SelectedIndex = selIndex-1;
		}

		private void SwitchCounters(int index1, int index2)
		{
			ScoringPlan scoringPlan = cbScoringPlan.SelectedItem as ScoringPlan;
			if (scoringPlan != null)
			{
				ScoringPlan.Counter c = scoringPlan.Counters[index1];
				scoringPlan.Counters[index1] = scoringPlan.Counters[index2];
				scoringPlan.Counters[index2] = c;
			}
		}

		private void btnAddPlan_Click(object sender, System.EventArgs e)
		{
			string name = "";
			if (Sport.UI.Dialogs.TextEditDialog.EditText("הכנס שם שיטת ניקוד", ref name))
			{
				ScoringPlan scoringPlan = new ScoringPlan();
				scoringPlan.Name = name;
				_teamPhaseScoring.ScoringPlans.Add(scoringPlan);
				cbScoringPlan.Items.Add(scoringPlan);
				cbScoringPlan.SelectedItem = scoringPlan;
			}
		}

		private void btnRemovePlan_Click(object sender, System.EventArgs e)
		{
			ScoringPlan scoringPlan = cbScoringPlan.SelectedItem as ScoringPlan;
			if (scoringPlan != null)
			{
				cbScoringPlan.Items.Remove(scoringPlan);
				_teamPhaseScoring.ScoringPlans.Remove(scoringPlan);
				SetCounters();
				SetButtonState();
			}
		}

		private void cbScoringPlan_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			ScoringPlan scoringPlan = cbScoringPlan.SelectedItem as ScoringPlan;
			if (scoringPlan == null)
			{
				nudAdditionalResults.Value = 0;
				nudMultiChallengeResults.Value = 0;
			}
			else
			{
				nudAdditionalResults.Value = scoringPlan.AdditionalResults;
				nudMultiChallengeResults.Value = scoringPlan.MultiChallengeResults;

			}
			SetPlanType();
			SetCounters();
			SetButtonState();
		}

		private void PlanTypeCheckChanged(object sender, System.EventArgs e)
		{
			ScoringPlan scoringPlan = cbScoringPlan.SelectedItem as ScoringPlan;
			if (scoringPlan != null)
			{
				scoringPlan.PlanType = rbCounters.Checked ? ScoringPlanType.Counters : ScoringPlanType.MultiChallenge;
				SetPlanType();
				SetButtonState();
			}
		}

		private void nudMultiChallengeResults_ValueChanged(object sender, System.EventArgs e)
		{
			ScoringPlan scoringPlan = cbScoringPlan.SelectedItem as ScoringPlan;
			if (scoringPlan != null)
			{
				scoringPlan.MultiChallengeResults = (int) nudMultiChallengeResults.Value;
			}
		}

		private void nudAdditionalResults_ValueChanged(object sender, System.EventArgs e)
		{
			ScoringPlan scoringPlan = cbScoringPlan.SelectedItem as ScoringPlan;
			if (scoringPlan != null)
			{
				scoringPlan.AdditionalResults = (int) nudAdditionalResults.Value;
			}
		}

		private void btnRename_Click(object sender, System.EventArgs e)
		{
			ScoringPlan scoringPlan = cbScoringPlan.SelectedItem as ScoringPlan;
			if (scoringPlan != null)
			{
				string name = scoringPlan.Name;
				if (Sport.UI.Dialogs.TextEditDialog.EditText("הכנס שם שיטת ניקוד", ref name))
				{
					scoringPlan.Name = name;
				}
			}
		}
	}
}
