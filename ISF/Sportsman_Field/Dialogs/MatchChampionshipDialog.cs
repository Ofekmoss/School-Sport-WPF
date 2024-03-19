using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Sport.Common;

namespace Sportsman_Field.Dialogs
{
	/// <summary>
	/// Summary description for MatchChampionshipDialog.
	/// </summary>
	public class MatchChampionshipDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.ListBox lbCategories;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.ListBox lbPhases;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.ListBox lbGroups;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.ListBox lbRounds;
		private System.Windows.Forms.GroupBox groupBox5;
		private System.Windows.Forms.ListBox lbCycles;
		private System.Windows.Forms.GroupBox groupBox6;
		private System.Windows.Forms.ListBox lbMatches;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label lbMatchTime;
		private System.Windows.Forms.Label lbFacility;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label lbCourt;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label lbResult;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label lbPartsResult;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Button btnClose;
		private System.Windows.Forms.Label lbChampName;
		private System.Windows.Forms.Button btnEditResult;
		private System.Windows.Forms.Button btnEditPartsResult;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private bool _setResultFlag=false;

		private int _championshipID=-1;
		private System.Windows.Forms.GroupBox gbMatchDetails;
		private ChampionshipData _champData=null;

		private int _lastSelectedCategory=-1;
		private int _lastSelectedPhase=-1;
		private int _lastSelectedGroup=-1;
		private int _lastSelectedRound=-1;
		private int _lastSelectedCycle=-1;
		private int _lastSelectedMatch=-1;

		public MatchChampionshipDialog(int champID, bool setResult)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			_setResultFlag = setResult;
			Sport.UI.Dialogs.WaitForm.ShowWait("טוען נתוני אליפות אנא המתן...");
			Cursor oldCursor=Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;
			_championshipID = champID;
			_champData = 
				LocalDatabaseManager.LocalDatabase.GetFullChampionship(champID);
			Cursor.Current = oldCursor;
			Sport.UI.Dialogs.WaitForm.HideWait();
		}

		public MatchChampionshipDialog(int champID)
			: this (champID, false)
		{
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(MatchChampionshipDialog));
			this.lbChampName = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.lbCategories = new System.Windows.Forms.ListBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.lbPhases = new System.Windows.Forms.ListBox();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.lbGroups = new System.Windows.Forms.ListBox();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.lbRounds = new System.Windows.Forms.ListBox();
			this.groupBox5 = new System.Windows.Forms.GroupBox();
			this.lbCycles = new System.Windows.Forms.ListBox();
			this.groupBox6 = new System.Windows.Forms.GroupBox();
			this.lbMatches = new System.Windows.Forms.ListBox();
			this.gbMatchDetails = new System.Windows.Forms.GroupBox();
			this.lbPartsResult = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.lbResult = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.lbCourt = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.lbFacility = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.lbMatchTime = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.btnClose = new System.Windows.Forms.Button();
			this.btnEditResult = new System.Windows.Forms.Button();
			this.btnEditPartsResult = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.groupBox5.SuspendLayout();
			this.groupBox6.SuspendLayout();
			this.gbMatchDetails.SuspendLayout();
			this.SuspendLayout();
			// 
			// lbChampName
			// 
			this.lbChampName.Font = new System.Drawing.Font("Tahoma", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbChampName.ForeColor = System.Drawing.Color.Blue;
			this.lbChampName.Location = new System.Drawing.Point(184, 2);
			this.lbChampName.Name = "lbChampName";
			this.lbChampName.Size = new System.Drawing.Size(360, 23);
			this.lbChampName.TabIndex = 0;
			this.lbChampName.Text = "שם אליפות";
			// 
			// groupBox1
			// 
			this.groupBox1.BackColor = System.Drawing.Color.White;
			this.groupBox1.Controls.Add(this.lbCategories);
			this.groupBox1.Location = new System.Drawing.Point(360, 27);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(185, 96);
			this.groupBox1.TabIndex = 6;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "קטגוריות אליפות";
			// 
			// lbCategories
			// 
			this.lbCategories.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbCategories.ForeColor = System.Drawing.Color.Black;
			this.lbCategories.Location = new System.Drawing.Point(2, 24);
			this.lbCategories.Name = "lbCategories";
			this.lbCategories.Size = new System.Drawing.Size(180, 69);
			this.lbCategories.TabIndex = 1;
			this.lbCategories.SelectedIndexChanged += new System.EventHandler(this.lbCategories_SelectedIndexChanged);
			// 
			// groupBox2
			// 
			this.groupBox2.BackColor = System.Drawing.Color.White;
			this.groupBox2.Controls.Add(this.lbPhases);
			this.groupBox2.Location = new System.Drawing.Point(456, 130);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(90, 96);
			this.groupBox2.TabIndex = 7;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "שלבים";
			// 
			// lbPhases
			// 
			this.lbPhases.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbPhases.ForeColor = System.Drawing.Color.Black;
			this.lbPhases.Location = new System.Drawing.Point(2, 24);
			this.lbPhases.Name = "lbPhases";
			this.lbPhases.Size = new System.Drawing.Size(85, 69);
			this.lbPhases.TabIndex = 1;
			this.lbPhases.SelectedIndexChanged += new System.EventHandler(this.lbPhases_SelectedIndexChanged);
			// 
			// groupBox3
			// 
			this.groupBox3.BackColor = System.Drawing.Color.White;
			this.groupBox3.Controls.Add(this.lbGroups);
			this.groupBox3.Location = new System.Drawing.Point(360, 130);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(90, 96);
			this.groupBox3.TabIndex = 8;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "בתים";
			// 
			// lbGroups
			// 
			this.lbGroups.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbGroups.ForeColor = System.Drawing.Color.Black;
			this.lbGroups.Location = new System.Drawing.Point(2, 24);
			this.lbGroups.Name = "lbGroups";
			this.lbGroups.Size = new System.Drawing.Size(85, 69);
			this.lbGroups.TabIndex = 1;
			this.lbGroups.SelectedIndexChanged += new System.EventHandler(this.lbGroups_SelectedIndexChanged);
			// 
			// groupBox4
			// 
			this.groupBox4.BackColor = System.Drawing.Color.White;
			this.groupBox4.Controls.Add(this.lbRounds);
			this.groupBox4.Location = new System.Drawing.Point(456, 235);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(90, 96);
			this.groupBox4.TabIndex = 9;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "סיבובים";
			// 
			// lbRounds
			// 
			this.lbRounds.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbRounds.ForeColor = System.Drawing.Color.Black;
			this.lbRounds.Location = new System.Drawing.Point(2, 24);
			this.lbRounds.Name = "lbRounds";
			this.lbRounds.Size = new System.Drawing.Size(85, 69);
			this.lbRounds.TabIndex = 1;
			this.lbRounds.SelectedIndexChanged += new System.EventHandler(this.lbRounds_SelectedIndexChanged);
			// 
			// groupBox5
			// 
			this.groupBox5.BackColor = System.Drawing.Color.White;
			this.groupBox5.Controls.Add(this.lbCycles);
			this.groupBox5.Location = new System.Drawing.Point(360, 235);
			this.groupBox5.Name = "groupBox5";
			this.groupBox5.Size = new System.Drawing.Size(90, 96);
			this.groupBox5.TabIndex = 10;
			this.groupBox5.TabStop = false;
			this.groupBox5.Text = "מחזורים";
			// 
			// lbCycles
			// 
			this.lbCycles.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbCycles.ForeColor = System.Drawing.Color.Black;
			this.lbCycles.Location = new System.Drawing.Point(2, 24);
			this.lbCycles.Name = "lbCycles";
			this.lbCycles.Size = new System.Drawing.Size(85, 69);
			this.lbCycles.TabIndex = 1;
			this.lbCycles.SelectedIndexChanged += new System.EventHandler(this.lbCycles_SelectedIndexChanged);
			// 
			// groupBox6
			// 
			this.groupBox6.BackColor = System.Drawing.Color.White;
			this.groupBox6.Controls.Add(this.lbMatches);
			this.groupBox6.Location = new System.Drawing.Point(5, 27);
			this.groupBox6.Name = "groupBox6";
			this.groupBox6.Size = new System.Drawing.Size(350, 200);
			this.groupBox6.TabIndex = 11;
			this.groupBox6.TabStop = false;
			this.groupBox6.Text = "התמודדויות";
			// 
			// lbMatches
			// 
			this.lbMatches.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbMatches.ForeColor = System.Drawing.Color.Blue;
			this.lbMatches.Items.AddRange(new object[] {
														   "מכבי תל אביב -- הכח רמת גן מאוחדת ב\'"});
			this.lbMatches.Location = new System.Drawing.Point(2, 24);
			this.lbMatches.Name = "lbMatches";
			this.lbMatches.Size = new System.Drawing.Size(345, 173);
			this.lbMatches.TabIndex = 1;
			this.lbMatches.SelectedIndexChanged += new System.EventHandler(this.lbMatches_SelectedIndexChanged);
			// 
			// gbMatchDetails
			// 
			this.gbMatchDetails.BackColor = System.Drawing.Color.White;
			this.gbMatchDetails.Controls.Add(this.lbPartsResult);
			this.gbMatchDetails.Controls.Add(this.label8);
			this.gbMatchDetails.Controls.Add(this.lbResult);
			this.gbMatchDetails.Controls.Add(this.label7);
			this.gbMatchDetails.Controls.Add(this.lbCourt);
			this.gbMatchDetails.Controls.Add(this.label5);
			this.gbMatchDetails.Controls.Add(this.lbFacility);
			this.gbMatchDetails.Controls.Add(this.label4);
			this.gbMatchDetails.Controls.Add(this.lbMatchTime);
			this.gbMatchDetails.Controls.Add(this.label2);
			this.gbMatchDetails.Location = new System.Drawing.Point(72, 235);
			this.gbMatchDetails.Name = "gbMatchDetails";
			this.gbMatchDetails.Size = new System.Drawing.Size(282, 100);
			this.gbMatchDetails.TabIndex = 12;
			this.gbMatchDetails.TabStop = false;
			this.gbMatchDetails.Text = "פרטי התמודדות";
			// 
			// lbPartsResult
			// 
			this.lbPartsResult.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbPartsResult.ForeColor = System.Drawing.Color.Blue;
			this.lbPartsResult.Location = new System.Drawing.Point(5, 69);
			this.lbPartsResult.Name = "lbPartsResult";
			this.lbPartsResult.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.lbPartsResult.Size = new System.Drawing.Size(130, 25);
			this.lbPartsResult.TabIndex = 9;
			this.lbPartsResult.Text = "20-15 , 30-20 , 25-20 , 15-30";
			this.lbPartsResult.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(16, 48);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(120, 23);
			this.label8.TabIndex = 8;
			this.label8.Text = "תוצאות חלקים: ";
			// 
			// lbResult
			// 
			this.lbResult.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbResult.ForeColor = System.Drawing.Color.Blue;
			this.lbResult.Location = new System.Drawing.Point(2, 24);
			this.lbResult.Name = "lbResult";
			this.lbResult.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.lbResult.Size = new System.Drawing.Size(62, 23);
			this.lbResult.TabIndex = 7;
			this.lbResult.Text = "85-90";
			this.lbResult.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(64, 24);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(50, 23);
			this.label7.TabIndex = 6;
			this.label7.Text = "תוצאה: ";
			// 
			// lbCourt
			// 
			this.lbCourt.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbCourt.ForeColor = System.Drawing.Color.Blue;
			this.lbCourt.Location = new System.Drawing.Point(142, 72);
			this.lbCourt.Name = "lbCourt";
			this.lbCourt.Size = new System.Drawing.Size(83, 23);
			this.lbCourt.TabIndex = 5;
			this.lbCourt.Text = "מגרש כדורסל";
			this.lbCourt.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(229, 72);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(50, 23);
			this.label5.TabIndex = 4;
			this.label5.Text = "מגרש: ";
			// 
			// lbFacility
			// 
			this.lbFacility.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbFacility.ForeColor = System.Drawing.Color.Blue;
			this.lbFacility.Location = new System.Drawing.Point(142, 48);
			this.lbFacility.Name = "lbFacility";
			this.lbFacility.Size = new System.Drawing.Size(83, 23);
			this.lbFacility.TabIndex = 3;
			this.lbFacility.Text = "אולם שמיר";
			this.lbFacility.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(229, 48);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(50, 23);
			this.label4.TabIndex = 2;
			this.label4.Text = "מתקן: ";
			// 
			// lbMatchTime
			// 
			this.lbMatchTime.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbMatchTime.ForeColor = System.Drawing.Color.Blue;
			this.lbMatchTime.Location = new System.Drawing.Point(120, 24);
			this.lbMatchTime.Name = "lbMatchTime";
			this.lbMatchTime.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.lbMatchTime.Size = new System.Drawing.Size(112, 23);
			this.lbMatchTime.TabIndex = 1;
			this.lbMatchTime.Text = "01/01/1900";
			this.lbMatchTime.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(229, 24);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(50, 23);
			this.label2.TabIndex = 0;
			this.label2.Text = "תאריך:";
			// 
			// btnClose
			// 
			this.btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnClose.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnClose.ForeColor = System.Drawing.Color.Blue;
			this.btnClose.Image = ((System.Drawing.Image)(resources.GetObject("btnClose.Image")));
			this.btnClose.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
			this.btnClose.Location = new System.Drawing.Point(8, 288);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(56, 40);
			this.btnClose.TabIndex = 13;
			this.btnClose.Text = "אישור";
			this.btnClose.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// btnEditResult
			// 
			this.btnEditResult.BackColor = System.Drawing.SystemColors.Control;
			this.btnEditResult.Enabled = false;
			this.btnEditResult.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnEditResult.Location = new System.Drawing.Point(7, 230);
			this.btnEditResult.Name = "btnEditResult";
			this.btnEditResult.Size = new System.Drawing.Size(55, 23);
			this.btnEditResult.TabIndex = 14;
			this.btnEditResult.Text = "תוצאה";
			this.btnEditResult.Click += new System.EventHandler(this.btnEditResult_Click);
			// 
			// btnEditPartsResult
			// 
			this.btnEditPartsResult.BackColor = System.Drawing.SystemColors.Control;
			this.btnEditPartsResult.Enabled = false;
			this.btnEditPartsResult.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnEditPartsResult.Location = new System.Drawing.Point(7, 257);
			this.btnEditPartsResult.Name = "btnEditPartsResult";
			this.btnEditPartsResult.Size = new System.Drawing.Size(55, 23);
			this.btnEditPartsResult.TabIndex = 15;
			this.btnEditPartsResult.Text = "חלקים";
			this.btnEditPartsResult.Click += new System.EventHandler(this.btnEditPartsResult_Click);
			// 
			// MatchChampionshipDialog
			// 
			this.AcceptButton = this.btnClose;
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
			this.BackColor = System.Drawing.Color.White;
			this.CancelButton = this.btnClose;
			this.ClientSize = new System.Drawing.Size(546, 335);
			this.Controls.Add(this.btnEditPartsResult);
			this.Controls.Add(this.btnEditResult);
			this.Controls.Add(this.btnClose);
			this.Controls.Add(this.gbMatchDetails);
			this.Controls.Add(this.groupBox6);
			this.Controls.Add(this.groupBox5);
			this.Controls.Add(this.groupBox4);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.lbChampName);
			this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "MatchChampionshipDialog";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "נתוני אליפות";
			this.Load += new System.EventHandler(this.MatchChampionshipDialog_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.groupBox4.ResumeLayout(false);
			this.groupBox5.ResumeLayout(false);
			this.groupBox6.ResumeLayout(false);
			this.gbMatchDetails.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void MatchChampionshipDialog_Load(object sender, System.EventArgs e)
		{
			if (_setResultFlag)
				this.Text += " - קביעת תוצאות";

			//name:
			lbChampName.Text = _champData.Name;

			//categories:
			FillCategories();

			if (_setResultFlag)
			{
				btnEditResult.Visible = true;
				btnEditPartsResult.Visible = true;
			}
		}

		#region Fill Methods
		private void FillCategories()
		{
			lbCategories.Items.Clear();
			foreach (CategoryData category in _champData.Categories)
			{
				string strCategory=
					Sport.Types.CategoryTypeLookup.ToString(category.Category);
				lbCategories.Items.Add(new ListItem(strCategory, category.ID));
			}

			//auto select...
			if (_champData.Categories.Length == 1)
				lbCategories.SelectedIndex = 0;
		}

		private void FillPhases(CategoryData category)
		{
			//reset groups:
			FillGroups(null);

			//clear phases list:
			lbPhases.Items.Clear();
			_lastSelectedPhase = -1;
			
			//got any data?
			if (category == null)
				return;

			//fill phases:
			foreach (PhaseData phase in category.Phases)
				lbPhases.Items.Add(new ListItem(phase.PhaseName, phase));
			
			//auto select...
			if (category.Phases.Length == 1)
				lbPhases.SelectedIndex = 0;
		}

		private void FillGroups(PhaseData phase)
		{
			//reset rounds:
			FillRounds(null);
			
			//clear groups list:
			lbGroups.Items.Clear();
			_lastSelectedGroup = -1;
			
			//got any data?
			if (phase == null)
				return;

			//fill groups:
			foreach (GroupData group in phase.Groups)
				lbGroups.Items.Add(new ListItem(group.GroupName, group));
			
			//auto select...
			if (phase.Groups.Length == 1)
				lbGroups.SelectedIndex = 0;
		}
		
		private void FillRounds(GroupData group)
		{
			//reset cycles:
			FillCycles(null);
			
			//clear rounds list:
			lbRounds.Items.Clear();
			_lastSelectedRound = -1;

			//got any data?
			if (group == null)
				return;

			//fill rounds:
			foreach (RoundData round in group.Rounds)
				lbRounds.Items.Add(new ListItem(round.RoundName, round));
			
			//auto select...
			if (group.Rounds.Length == 1)
				lbRounds.SelectedIndex = 0;
		}
		
		private void FillCycles(RoundData round)
		{
			//reset matches:
			FillMatches(null, -1);
			
			//clear cycles list:
			lbCycles.Items.Clear();
			_lastSelectedCycle = -1;

			//got any data?
			if (round == null)
				return;
			
			//fill cycles:
			ArrayList arrCycles=new ArrayList();
			foreach (MatchData match in round.Matches)
			{
				if (arrCycles.IndexOf(match.Cycle) < 0)
					arrCycles.Add(match.Cycle);
			}
			foreach (int cycle in arrCycles)
				lbCycles.Items.Add(new ListItem("מחזור "+(cycle+1), cycle));
			
			//auto select...
			if (arrCycles.Count == 1)
				lbCycles.SelectedIndex = 0;
		}

		private void FillMatches(RoundData round, int cycle)
		{
			//reset match details:
			FillMatchDetails(null);
			
			//clear matches list:
			lbMatches.Items.Clear();
			_lastSelectedMatch = -1;
			
			//got any data?
			if (round == null)
				return;
			
			//fill matches:
			foreach (MatchData match in round.Matches)
			{
				if (match.Cycle == cycle)
				{
					string strName=match.TeamA.School.Name+" ";
					if (match.TeamA.TeamIndex > 0)
					{
						string strHebLetter=Tools.ToHebLetter(match.TeamA.TeamIndex);
						if (strHebLetter.Length == 1)
							strHebLetter += "'";
						strName += strHebLetter;
					}
					strName += " -- "+match.TeamB.School.Name;
					if (match.TeamB.TeamIndex > 0)
					{
						string strHebLetter=Tools.ToHebLetter(match.TeamB.TeamIndex);
						if (strHebLetter.Length == 1)
							strHebLetter += "'";
						strName += " "+strHebLetter;
					}
					lbMatches.Items.Add(new ListItem(strName, match));
				}
			}
			
			//auto select...
			if (lbMatches.Items.Count == 1)
				lbMatches.SelectedIndex = 0;
		}

		private void FillMatchDetails(MatchData match)
		{
			//hide details:
			gbMatchDetails.Visible = false;

			//hide buttons:
			//btnEditResult.Visible = false;
			//btnEditPartsResult.Visible = false;
			
			//disable buttons:
			btnEditResult.Enabled = false;
			btnEditPartsResult.Enabled = false;

			//got any data?
			if (match == null)
				return;
			
			//fill match details:
			lbMatchTime.Text = Tools.IIF((match.Time.Year>=1900), 
				Tools.FormatDate(match.Time, "dd/mm/yy hh:nn"), "---").ToString();
			lbFacility.Text = Tools.IIF(((match.Facility != null)&&
				(match.Facility.ID >= 0)), match.Facility.Name, "---").ToString();
			lbCourt.Text = Tools.IIF(((match.Court != null)&&
				(match.Court.ID >= 0)), match.Court.Name, "---").ToString();

			string strResult="";
			Sport.Championships.MatchOutcome matchOutcome=
				(Sport.Championships.MatchOutcome) match.Result;
			switch (matchOutcome)
			{
				case Sport.Championships.MatchOutcome.None:
					strResult = "---";
					break;
				case Sport.Championships.MatchOutcome.TechnicalA:
					strResult = "טכני א'";
					break;
				case Sport.Championships.MatchOutcome.TechnicalB:
					strResult = "טכני ב'";
					break;
				case Sport.Championships.MatchOutcome.Tie:
				case Sport.Championships.MatchOutcome.WinA:
				case Sport.Championships.MatchOutcome.WinB:
					strResult = match.TeamB_Score.ToString()+"-"+
						match.TeamA_Score.ToString();
					break;
			}
			lbResult.Text = strResult;
			lbPartsResult.Text = Tools.IIF((Tools.CStrDef(
				match.PartsResult, "").Length > 0), 
				ParsePartsResult(match.PartsResult), "---").ToString();

			gbMatchDetails.Visible = true;
			btnEditResult.Enabled = true;
			btnEditPartsResult.Enabled = true;
		}
		#endregion

		private string ParsePartsResult(string partsResult)
		{
			string result="";
			string seperator=", ";
			string[] arrParts=partsResult.Split(new char[] {'|'});
			for (int i=0; i<arrParts.Length; i++)
			{
				string[] arrTemp=arrParts[i].Split(new char[] {'-'});
				if (arrTemp.Length == 2)
					result += arrTemp[1]+"-"+arrTemp[0]+seperator;
			}
			if (result.Length > 0)
				result = result.Substring(0, result.Length-seperator.Length);

			return result;
		}

		private CategoryData GetCategoryData(int categoryID)
		{
			foreach (CategoryData category in _champData.Categories)
			{
				if (category.ID == categoryID)
				{
					return category;
				}
			}
			return null;
		}

		private void lbCategories_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			int selIndex=lbCategories.SelectedIndex;
			if (selIndex == _lastSelectedCategory)
				return;
			
			if (selIndex >= 0)
			{
				int categoryID=(int) (lbCategories.SelectedItem as ListItem).Value;
				CategoryData category=GetCategoryData(categoryID);
				FillPhases(category);
			}
			_lastSelectedCategory = selIndex;
		}

		private void lbPhases_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			int selIndex=lbPhases.SelectedIndex;
			if (selIndex == _lastSelectedPhase)
				return;
			
			if (selIndex >= 0)
			{
				PhaseData phase=(PhaseData) (lbPhases.SelectedItem as ListItem).Value;
				FillGroups(phase);
			}
			_lastSelectedPhase = selIndex;
		}

		private void lbGroups_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			int selIndex=lbGroups.SelectedIndex;
			if (selIndex == _lastSelectedGroup)
				return;
			
			if (selIndex >= 0)
			{
				GroupData group=(GroupData) (lbGroups.SelectedItem as ListItem).Value;
				FillRounds(group);
			}
			_lastSelectedGroup = selIndex;
		}

		private void lbRounds_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			int selIndex=lbRounds.SelectedIndex;
			if (selIndex == _lastSelectedRound)
				return;
			
			if (selIndex >= 0)
			{
				RoundData round=(RoundData) (lbRounds.SelectedItem as ListItem).Value;
				FillCycles(round);
			}
			_lastSelectedRound = selIndex;
		}

		private void lbCycles_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			int selIndex=lbCycles.SelectedIndex;
			if (selIndex == _lastSelectedCycle)
				return;
			
			if (selIndex >= 0)
			{
				RoundData round=(RoundData) (lbRounds.SelectedItem as ListItem).Value;
				int cycle=(int) (lbCycles.SelectedItem as ListItem).Value;
				FillMatches(round, cycle);
			}
			_lastSelectedCycle = selIndex;
		}

		private void lbMatches_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			int selIndex=lbMatches.SelectedIndex;
			if (selIndex == _lastSelectedMatch)
				return;
			
			if (selIndex >= 0)
			{
				MatchData match=(MatchData) (lbMatches.SelectedItem as ListItem).Value;
				FillMatchDetails(match);
			}
			_lastSelectedMatch = selIndex;		
		}

		private void btnEditResult_Click(object sender, System.EventArgs e)
		{
			int selIndex=lbMatches.SelectedIndex;
			if (selIndex < 0)
				return;

			MatchData match=(MatchData) (lbMatches.SelectedItem as ListItem).Value;
			SetMatchResultDialog _dialog=
				new SetMatchResultDialog(match);
			if (_dialog.ShowDialog(this) == DialogResult.OK)
			{
				//update match data:
				double scoreA=_dialog.TeamA_Score;
				double scoreB=_dialog.TeamB_Score;
				match.TeamA_Score = scoreA;
				match.TeamB_Score = scoreB;
				Sport.Championships.MatchOutcome matchOutcome=
					Sport.Championships.MatchOutcome.None;
				if (_dialog.TeachnicalWin_A)
					matchOutcome = Sport.Championships.MatchOutcome.TechnicalA;
				else if (_dialog.TeachnicalWin_B)
					matchOutcome = Sport.Championships.MatchOutcome.TechnicalB;
				else if (scoreA > scoreB)
					matchOutcome = Sport.Championships.MatchOutcome.WinA;
				else if (scoreB > scoreA)
					matchOutcome = Sport.Championships.MatchOutcome.WinB;
				else
					matchOutcome = Sport.Championships.MatchOutcome.Tie;
				match.Result = (int) matchOutcome;
				
				//set match round:
				match.Round = (RoundData) (lbRounds.SelectedItem as ListItem).Value;

				//set round group:
				match.Round.Group = 
					(GroupData) (lbGroups.SelectedItem as ListItem).Value;

				//set group phase:
				match.Round.Group.Phase = 
					(PhaseData) (lbPhases.SelectedItem as ListItem).Value;
				
				//update database:
				LocalDatabaseManager.LocalDatabase.UpdateMatchData(match);

				//appply the changes:
				FillMatchDetails(match);
			}
		}

		private void btnEditPartsResult_Click(object sender, System.EventArgs e)
		{
			int selIndex=lbMatches.SelectedIndex;
			if (selIndex < 0)
				return;

			//get rule:
			string strGameStructureRule=_champData.GameStructureRule;
			MatchData match=(MatchData) (lbMatches.SelectedItem as ListItem).Value;
			
			if ((strGameStructureRule == null)||(strGameStructureRule.Length == 0))
			{
				Sport.UI.MessageBox.Error("חוק מבנה משחק לא מוגדר עבור אליפות זו", "שגיאה");
				return;
			}
			
			//extract parts count:
			string[] arrTemp=strGameStructureRule.Split(new char[] {'.'});
			if (arrTemp.Length != 3)
			{
				Sport.UI.MessageBox.Error("חוק מבנה משחק שגוי", "שגיאת מערכת");
				return;
			}
			int partsCount=Int32.Parse(arrTemp[1]);

			//call the dialog:
			SetPartsResultDialog _dialog=
				new SetPartsResultDialog(partsCount, match);
			if (_dialog.ShowDialog(this) == DialogResult.OK)
			{
				//get parts result:
				string partsResult=_dialog.PartsResult;
				
				//set match round:
				match.Round = (RoundData) 
					(lbRounds.SelectedItem as ListItem).Value;
				
				//set match group:
				match.Round.Group = (GroupData)
					(lbGroups.SelectedItem as ListItem).Value;

				//set match phase:
				match.Round.Group.Phase = (PhaseData)
					(lbPhases.SelectedItem as ListItem).Value;
				
				//update match data:
				match.PartsResult = partsResult;
				LocalDatabaseManager.LocalDatabase.UpdateMatchData(match);
				
				//apply changes:
				FillMatchDetails(match);
			}
		}
	}
}
