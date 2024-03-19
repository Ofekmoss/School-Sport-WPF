using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Sport.Common;

namespace Sportsman_Field.Dialogs
{
	/// <summary>
	/// Summary description for CompetitionChampionshipDialog.
	/// </summary>
	public class CompetitionChampionshipDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button btnClose;
		private System.Windows.Forms.Label lbCourt;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label lbFacility;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.GroupBox groupBox6;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.ListBox lbGroups;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.ListBox lbPhases;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.ListBox lbCategories;
		private System.Windows.Forms.Label lbChampName;
		private System.Windows.Forms.GroupBox groupBox5;
		private System.Windows.Forms.GroupBox gbCompetitionDetails;
		private System.Windows.Forms.Label lbCompetitionTime;
		private System.Windows.Forms.ListBox lbCompetitions;
		private System.Windows.Forms.ListBox lbHeats;
		private System.Windows.Forms.ListBox lbCompetitors;
		private System.Windows.Forms.GroupBox gbCompetitorDetails;
		private System.Windows.Forms.Label lbScore;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label lbResult;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label lbCompetitorName;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Button btnEditResult;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private int _championshipID=-1;
		private ChampionshipData _champData=null;
		private bool _setResultFlag=false;

		private int _lastSelectedCategory=-1;
		private int _lastSelectedPhase=-1;
		private int _lastSelectedGroup=-1;
		private int _lastSelectedHeat=-1;
		private int _lastSelectedCompetition=-1;
		private System.Windows.Forms.Label lbHeatCourt;
		private System.Windows.Forms.Label lbHeatFacility;
		private System.Windows.Forms.Label lbHeatTime;
		private System.Windows.Forms.GroupBox gbHeatDetails;
		private System.Windows.Forms.Button btnSetResults;
		private System.Windows.Forms.Label lbShirtNumber;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label lbChipNumber;
		private System.Windows.Forms.Label label8;
		private int _lastSelectedCompetitor=-1;

		public CompetitionChampionshipDialog(int champID, bool setResult)
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

		public CompetitionChampionshipDialog(int champID)
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(CompetitionChampionshipDialog));
			this.btnClose = new System.Windows.Forms.Button();
			this.gbCompetitionDetails = new System.Windows.Forms.GroupBox();
			this.lbCourt = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.lbFacility = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.lbCompetitionTime = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.groupBox6 = new System.Windows.Forms.GroupBox();
			this.btnSetResults = new System.Windows.Forms.Button();
			this.lbCompetitions = new System.Windows.Forms.ListBox();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.gbHeatDetails = new System.Windows.Forms.GroupBox();
			this.lbHeatCourt = new System.Windows.Forms.Label();
			this.lbHeatFacility = new System.Windows.Forms.Label();
			this.lbHeatTime = new System.Windows.Forms.Label();
			this.lbHeats = new System.Windows.Forms.ListBox();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.lbGroups = new System.Windows.Forms.ListBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.lbPhases = new System.Windows.Forms.ListBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.lbCategories = new System.Windows.Forms.ListBox();
			this.lbChampName = new System.Windows.Forms.Label();
			this.groupBox5 = new System.Windows.Forms.GroupBox();
			this.lbCompetitors = new System.Windows.Forms.ListBox();
			this.gbCompetitorDetails = new System.Windows.Forms.GroupBox();
			this.lbScore = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.lbResult = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.lbCompetitorName = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.btnEditResult = new System.Windows.Forms.Button();
			this.lbShirtNumber = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.lbChipNumber = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.gbCompetitionDetails.SuspendLayout();
			this.groupBox6.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.gbHeatDetails.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.groupBox5.SuspendLayout();
			this.gbCompetitorDetails.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnClose
			// 
			this.btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnClose.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnClose.ForeColor = System.Drawing.Color.Blue;
			this.btnClose.Image = ((System.Drawing.Image)(resources.GetObject("btnClose.Image")));
			this.btnClose.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
			this.btnClose.Location = new System.Drawing.Point(8, 286);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(56, 40);
			this.btnClose.TabIndex = 22;
			this.btnClose.Text = "אישור";
			this.btnClose.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// gbCompetitionDetails
			// 
			this.gbCompetitionDetails.BackColor = System.Drawing.Color.White;
			this.gbCompetitionDetails.Controls.Add(this.lbCourt);
			this.gbCompetitionDetails.Controls.Add(this.label5);
			this.gbCompetitionDetails.Controls.Add(this.lbFacility);
			this.gbCompetitionDetails.Controls.Add(this.label4);
			this.gbCompetitionDetails.Controls.Add(this.lbCompetitionTime);
			this.gbCompetitionDetails.Controls.Add(this.label2);
			this.gbCompetitionDetails.Location = new System.Drawing.Point(384, 232);
			this.gbCompetitionDetails.Name = "gbCompetitionDetails";
			this.gbCompetitionDetails.Size = new System.Drawing.Size(176, 100);
			this.gbCompetitionDetails.TabIndex = 21;
			this.gbCompetitionDetails.TabStop = false;
			this.gbCompetitionDetails.Text = "פרטי תחרות";
			// 
			// lbCourt
			// 
			this.lbCourt.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbCourt.ForeColor = System.Drawing.Color.Blue;
			this.lbCourt.Location = new System.Drawing.Point(2, 72);
			this.lbCourt.Name = "lbCourt";
			this.lbCourt.Size = new System.Drawing.Size(110, 23);
			this.lbCourt.TabIndex = 5;
			this.lbCourt.Text = "מגרש כדורסל";
			this.lbCourt.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(120, 72);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(50, 23);
			this.label5.TabIndex = 4;
			this.label5.Text = "מגרש: ";
			// 
			// lbFacility
			// 
			this.lbFacility.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbFacility.ForeColor = System.Drawing.Color.Blue;
			this.lbFacility.Location = new System.Drawing.Point(2, 48);
			this.lbFacility.Name = "lbFacility";
			this.lbFacility.Size = new System.Drawing.Size(110, 23);
			this.lbFacility.TabIndex = 3;
			this.lbFacility.Text = "אולם שמיר";
			this.lbFacility.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(120, 48);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(50, 23);
			this.label4.TabIndex = 2;
			this.label4.Text = "מתקן: ";
			// 
			// lbCompetitionTime
			// 
			this.lbCompetitionTime.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbCompetitionTime.ForeColor = System.Drawing.Color.Blue;
			this.lbCompetitionTime.Location = new System.Drawing.Point(2, 24);
			this.lbCompetitionTime.Name = "lbCompetitionTime";
			this.lbCompetitionTime.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.lbCompetitionTime.Size = new System.Drawing.Size(110, 23);
			this.lbCompetitionTime.TabIndex = 1;
			this.lbCompetitionTime.Text = "01/01/1900";
			this.lbCompetitionTime.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(120, 24);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(50, 23);
			this.label2.TabIndex = 0;
			this.label2.Text = "תאריך:";
			// 
			// groupBox6
			// 
			this.groupBox6.BackColor = System.Drawing.Color.White;
			this.groupBox6.Controls.Add(this.btnSetResults);
			this.groupBox6.Controls.Add(this.lbCompetitions);
			this.groupBox6.Location = new System.Drawing.Point(384, 24);
			this.groupBox6.Name = "groupBox6";
			this.groupBox6.Size = new System.Drawing.Size(165, 199);
			this.groupBox6.TabIndex = 20;
			this.groupBox6.TabStop = false;
			this.groupBox6.Text = "תחרויות";
			// 
			// btnSetResults
			// 
			this.btnSetResults.Enabled = false;
			this.btnSetResults.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnSetResults.Image = ((System.Drawing.Image)(resources.GetObject("btnSetResults.Image")));
			this.btnSetResults.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btnSetResults.Location = new System.Drawing.Point(45, 171);
			this.btnSetResults.Name = "btnSetResults";
			this.btnSetResults.Size = new System.Drawing.Size(80, 23);
			this.btnSetResults.TabIndex = 2;
			this.btnSetResults.Text = "תוצאות";
			this.btnSetResults.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.btnSetResults.Click += new System.EventHandler(this.btnSetResults_Click);
			// 
			// lbCompetitions
			// 
			this.lbCompetitions.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbCompetitions.ForeColor = System.Drawing.Color.Blue;
			this.lbCompetitions.Items.AddRange(new object[] {
																"זריקת כדור ברזל 3 ק\"ג"});
			this.lbCompetitions.Location = new System.Drawing.Point(2, 24);
			this.lbCompetitions.Name = "lbCompetitions";
			this.lbCompetitions.Size = new System.Drawing.Size(160, 147);
			this.lbCompetitions.TabIndex = 1;
			this.lbCompetitions.DoubleClick += new System.EventHandler(this.btnSetResults_Click);
			this.lbCompetitions.SelectedIndexChanged += new System.EventHandler(this.lbCompetitions_SelectedIndexChanged);
			// 
			// groupBox4
			// 
			this.groupBox4.BackColor = System.Drawing.Color.White;
			this.groupBox4.Controls.Add(this.gbHeatDetails);
			this.groupBox4.Controls.Add(this.lbHeats);
			this.groupBox4.Location = new System.Drawing.Point(560, 216);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(176, 112);
			this.groupBox4.TabIndex = 18;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "מקצים";
			// 
			// gbHeatDetails
			// 
			this.gbHeatDetails.Controls.Add(this.lbHeatCourt);
			this.gbHeatDetails.Controls.Add(this.lbHeatFacility);
			this.gbHeatDetails.Controls.Add(this.lbHeatTime);
			this.gbHeatDetails.Location = new System.Drawing.Point(2, 12);
			this.gbHeatDetails.Name = "gbHeatDetails";
			this.gbHeatDetails.Size = new System.Drawing.Size(102, 98);
			this.gbHeatDetails.TabIndex = 9;
			this.gbHeatDetails.TabStop = false;
			this.gbHeatDetails.Text = "פרטי מקצה";
			// 
			// lbHeatCourt
			// 
			this.lbHeatCourt.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbHeatCourt.ForeColor = System.Drawing.Color.Blue;
			this.lbHeatCourt.Location = new System.Drawing.Point(2, 72);
			this.lbHeatCourt.Name = "lbHeatCourt";
			this.lbHeatCourt.Size = new System.Drawing.Size(95, 20);
			this.lbHeatCourt.TabIndex = 11;
			this.lbHeatCourt.Text = "מגרש כדורסל";
			this.lbHeatCourt.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// lbHeatFacility
			// 
			this.lbHeatFacility.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbHeatFacility.ForeColor = System.Drawing.Color.Blue;
			this.lbHeatFacility.Location = new System.Drawing.Point(2, 42);
			this.lbHeatFacility.Name = "lbHeatFacility";
			this.lbHeatFacility.Size = new System.Drawing.Size(95, 20);
			this.lbHeatFacility.TabIndex = 10;
			this.lbHeatFacility.Text = "אולם שמיר";
			this.lbHeatFacility.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// lbHeatTime
			// 
			this.lbHeatTime.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbHeatTime.ForeColor = System.Drawing.Color.Blue;
			this.lbHeatTime.Location = new System.Drawing.Point(2, 15);
			this.lbHeatTime.Name = "lbHeatTime";
			this.lbHeatTime.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.lbHeatTime.Size = new System.Drawing.Size(95, 20);
			this.lbHeatTime.TabIndex = 9;
			this.lbHeatTime.Text = "01/01/1900";
			this.lbHeatTime.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// lbHeats
			// 
			this.lbHeats.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbHeats.ForeColor = System.Drawing.Color.Black;
			this.lbHeats.Location = new System.Drawing.Point(104, 40);
			this.lbHeats.Name = "lbHeats";
			this.lbHeats.Size = new System.Drawing.Size(64, 69);
			this.lbHeats.TabIndex = 1;
			this.lbHeats.SelectedIndexChanged += new System.EventHandler(this.lbHeats_SelectedIndexChanged);
			// 
			// groupBox3
			// 
			this.groupBox3.BackColor = System.Drawing.Color.White;
			this.groupBox3.Controls.Add(this.lbGroups);
			this.groupBox3.Location = new System.Drawing.Point(552, 128);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(90, 88);
			this.groupBox3.TabIndex = 17;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "בתים";
			// 
			// lbGroups
			// 
			this.lbGroups.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbGroups.ForeColor = System.Drawing.Color.Black;
			this.lbGroups.Location = new System.Drawing.Point(2, 24);
			this.lbGroups.Name = "lbGroups";
			this.lbGroups.Size = new System.Drawing.Size(85, 56);
			this.lbGroups.TabIndex = 1;
			this.lbGroups.SelectedIndexChanged += new System.EventHandler(this.lbGroups_SelectedIndexChanged);
			// 
			// groupBox2
			// 
			this.groupBox2.BackColor = System.Drawing.Color.White;
			this.groupBox2.Controls.Add(this.lbPhases);
			this.groupBox2.Location = new System.Drawing.Point(648, 128);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(90, 88);
			this.groupBox2.TabIndex = 16;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "שלבים";
			// 
			// lbPhases
			// 
			this.lbPhases.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbPhases.ForeColor = System.Drawing.Color.Black;
			this.lbPhases.Location = new System.Drawing.Point(2, 24);
			this.lbPhases.Name = "lbPhases";
			this.lbPhases.Size = new System.Drawing.Size(85, 56);
			this.lbPhases.TabIndex = 1;
			this.lbPhases.SelectedIndexChanged += new System.EventHandler(this.lbPhases_SelectedIndexChanged);
			// 
			// groupBox1
			// 
			this.groupBox1.BackColor = System.Drawing.Color.White;
			this.groupBox1.Controls.Add(this.lbCategories);
			this.groupBox1.Location = new System.Drawing.Point(552, 24);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(185, 96);
			this.groupBox1.TabIndex = 15;
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
			// lbChampName
			// 
			this.lbChampName.Font = new System.Drawing.Font("Tahoma", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbChampName.ForeColor = System.Drawing.Color.Blue;
			this.lbChampName.Location = new System.Drawing.Point(376, 0);
			this.lbChampName.Name = "lbChampName";
			this.lbChampName.Size = new System.Drawing.Size(360, 23);
			this.lbChampName.TabIndex = 14;
			this.lbChampName.Text = "שם אליפות";
			// 
			// groupBox5
			// 
			this.groupBox5.BackColor = System.Drawing.Color.White;
			this.groupBox5.Controls.Add(this.lbCompetitors);
			this.groupBox5.Location = new System.Drawing.Point(8, 24);
			this.groupBox5.Name = "groupBox5";
			this.groupBox5.Size = new System.Drawing.Size(368, 199);
			this.groupBox5.TabIndex = 23;
			this.groupBox5.TabStop = false;
			this.groupBox5.Text = "מתמודדים";
			// 
			// lbCompetitors
			// 
			this.lbCompetitors.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbCompetitors.ForeColor = System.Drawing.Color.Blue;
			this.lbCompetitors.Items.AddRange(new object[] {
															   "אבי כהן, עירוני ט\' תל אביב (03:40 מטר)"});
			this.lbCompetitors.Location = new System.Drawing.Point(5, 24);
			this.lbCompetitors.Name = "lbCompetitors";
			this.lbCompetitors.Size = new System.Drawing.Size(358, 173);
			this.lbCompetitors.TabIndex = 1;
			this.lbCompetitors.DoubleClick += new System.EventHandler(this.btnSetResults_Click);
			this.lbCompetitors.SelectedIndexChanged += new System.EventHandler(this.lbCompetitors_SelectedIndexChanged);
			// 
			// gbCompetitorDetails
			// 
			this.gbCompetitorDetails.BackColor = System.Drawing.Color.White;
			this.gbCompetitorDetails.Controls.Add(this.lbChipNumber);
			this.gbCompetitorDetails.Controls.Add(this.label8);
			this.gbCompetitorDetails.Controls.Add(this.lbShirtNumber);
			this.gbCompetitorDetails.Controls.Add(this.label6);
			this.gbCompetitorDetails.Controls.Add(this.lbScore);
			this.gbCompetitorDetails.Controls.Add(this.label3);
			this.gbCompetitorDetails.Controls.Add(this.lbResult);
			this.gbCompetitorDetails.Controls.Add(this.label7);
			this.gbCompetitorDetails.Controls.Add(this.lbCompetitorName);
			this.gbCompetitorDetails.Controls.Add(this.label9);
			this.gbCompetitorDetails.Location = new System.Drawing.Point(72, 232);
			this.gbCompetitorDetails.Name = "gbCompetitorDetails";
			this.gbCompetitorDetails.Size = new System.Drawing.Size(304, 100);
			this.gbCompetitorDetails.TabIndex = 24;
			this.gbCompetitorDetails.TabStop = false;
			this.gbCompetitorDetails.Text = "פרטי מתמודד";
			// 
			// lbScore
			// 
			this.lbScore.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbScore.ForeColor = System.Drawing.Color.Blue;
			this.lbScore.Location = new System.Drawing.Point(128, 72);
			this.lbScore.Name = "lbScore";
			this.lbScore.Size = new System.Drawing.Size(115, 23);
			this.lbScore.TabIndex = 5;
			this.lbScore.Text = "70";
			this.lbScore.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(248, 72);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(50, 23);
			this.label3.TabIndex = 4;
			this.label3.Text = "ניקוד: ";
			// 
			// lbResult
			// 
			this.lbResult.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbResult.ForeColor = System.Drawing.Color.Blue;
			this.lbResult.Location = new System.Drawing.Point(128, 48);
			this.lbResult.Name = "lbResult";
			this.lbResult.Size = new System.Drawing.Size(115, 23);
			this.lbResult.TabIndex = 3;
			this.lbResult.Text = "03:40 מטר";
			this.lbResult.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(248, 48);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(50, 23);
			this.label7.TabIndex = 2;
			this.label7.Text = "תוצאה: ";
			// 
			// lbCompetitorName
			// 
			this.lbCompetitorName.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbCompetitorName.ForeColor = System.Drawing.Color.Blue;
			this.lbCompetitorName.Location = new System.Drawing.Point(128, 24);
			this.lbCompetitorName.Name = "lbCompetitorName";
			this.lbCompetitorName.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.lbCompetitorName.Size = new System.Drawing.Size(115, 23);
			this.lbCompetitorName.TabIndex = 1;
			this.lbCompetitorName.Text = "אבי כהן";
			this.lbCompetitorName.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(248, 24);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(50, 23);
			this.label9.TabIndex = 0;
			this.label9.Text = "שם:";
			// 
			// btnEditResult
			// 
			this.btnEditResult.BackColor = System.Drawing.SystemColors.Control;
			this.btnEditResult.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnEditResult.Location = new System.Drawing.Point(8, 232);
			this.btnEditResult.Name = "btnEditResult";
			this.btnEditResult.Size = new System.Drawing.Size(55, 23);
			this.btnEditResult.TabIndex = 25;
			this.btnEditResult.Text = "תוצאה";
			this.btnEditResult.Click += new System.EventHandler(this.btnEditResult_Click);
			// 
			// lbShirtNumber
			// 
			this.lbShirtNumber.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbShirtNumber.ForeColor = System.Drawing.Color.Blue;
			this.lbShirtNumber.Location = new System.Drawing.Point(8, 24);
			this.lbShirtNumber.Name = "lbShirtNumber";
			this.lbShirtNumber.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.lbShirtNumber.Size = new System.Drawing.Size(35, 23);
			this.lbShirtNumber.TabIndex = 7;
			this.lbShirtNumber.Text = "10";
			this.lbShirtNumber.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(48, 24);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(72, 23);
			this.label6.TabIndex = 6;
			this.label6.Text = "מספר חולצה: ";
			// 
			// lbChipNumber
			// 
			this.lbChipNumber.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbChipNumber.ForeColor = System.Drawing.Color.Blue;
			this.lbChipNumber.Location = new System.Drawing.Point(8, 56);
			this.lbChipNumber.Name = "lbChipNumber";
			this.lbChipNumber.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.lbChipNumber.Size = new System.Drawing.Size(35, 23);
			this.lbChipNumber.TabIndex = 9;
			this.lbChipNumber.Text = "1";
			this.lbChipNumber.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(48, 56);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(72, 23);
			this.label8.TabIndex = 8;
			this.label8.Text = "מספר צ\'יפ: ";
			// 
			// CompetitionChampionshipDialog
			// 
			this.AcceptButton = this.btnClose;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.Color.White;
			this.CancelButton = this.btnClose;
			this.ClientSize = new System.Drawing.Size(744, 333);
			this.Controls.Add(this.btnEditResult);
			this.Controls.Add(this.gbCompetitorDetails);
			this.Controls.Add(this.groupBox5);
			this.Controls.Add(this.btnClose);
			this.Controls.Add(this.gbCompetitionDetails);
			this.Controls.Add(this.groupBox6);
			this.Controls.Add(this.groupBox4);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.lbChampName);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "CompetitionChampionshipDialog";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "פרטי אליפות";
			this.Load += new System.EventHandler(this.CompetitionChampionshipDialog_Load);
			this.gbCompetitionDetails.ResumeLayout(false);
			this.groupBox6.ResumeLayout(false);
			this.groupBox4.ResumeLayout(false);
			this.gbHeatDetails.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox5.ResumeLayout(false);
			this.gbCompetitorDetails.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void CompetitionChampionshipDialog_Load(object sender, System.EventArgs e)
		{
			//add text:
			if (_setResultFlag)
				this.Text += " - קביעת תוצאות";
			
			//name:
			lbChampName.Text = _champData.Name;

			//categories:
			FillCategories();
			
			if (_setResultFlag == false)
			{
				btnEditResult.Visible = false;
				btnSetResults.Visible = false;
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
				lbCategories.Items.Add(new ListItem(strCategory, category));
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
			//reset competitions
			FillCompetitions(null);
			
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
		
		private void FillCompetitions(GroupData group)
		{
			//reset heats:
			FillHeats(null);
			
			//reset competition details:
			FillCompetitionDetails(null);
			btnSetResults.Enabled = false;
			
			//clear competitions list:
			lbCompetitions.Items.Clear();
			_lastSelectedCompetition = -1;
			
			//got any data?
			if (group == null)
				return;
			
			//fill competitions:
			foreach (CompetitionData competition in group.Competitions)
			{
				lbCompetitions.Items.Add(
					new ListItem(competition.SportField.Name, competition));
			}
			
			//auto select...
			if (group.Competitions.Length == 1)
				lbCompetitions.SelectedIndex = 0;
		}
		
		private void FillHeats(CompetitionData competition)
		{
			//reset competitors:
			FillCompetitors(null, -1);
			
			//reset heat details:
			FillHeatDetails(null);
			
			//clear heats list:
			lbHeats.Items.Clear();
			_lastSelectedHeat = -1;

			//got any data?
			if (competition == null)
				return;
			
			//fill heats:
			foreach (HeatData heat in competition.Heats)
				lbHeats.Items.Add(new ListItem("מקצה "+(heat.HeatIndex+1), heat));
			
			//auto select...
			if (competition.Heats.Length == 1)
				lbHeats.SelectedIndex = 0;
		}

		private void FillCompetitors(CompetitionData competition, int heatIndex)
		{
			//reset competitor details:
			FillCompetitorDetails(null);
			
			//clear competitors list:
			lbCompetitors.Items.Clear();
			_lastSelectedCompetitor = -1;
			
			//got any data?
			if (competition == null)
				return;
			
			//fill competitors:
			foreach (CompetitorData competitor in competition.Competitors)
			{
				if (competitor.Heat.HeatIndex == heatIndex)
				{
					string strCompetitor=competitor.Player.Student.FirstName;
					strCompetitor += " "+competitor.Player.Student.LastName;
					if (competitor.Player.ShirtNumber > 0)
						strCompetitor += " ("+competitor.Player.ShirtNumber+")";
					strCompetitor += ", "+competitor.Player.Team.School.Name;
					if ((competitor.Result != null)&&(competitor.Result.Length > 0))
						strCompetitor += " ("+competitor.ParseResult()+")";
					lbCompetitors.Items.Add(new ListItem(strCompetitor, competitor));
				}
			}
			
			//auto select...
			if (lbCompetitors.Items.Count == 1)
				lbCompetitors.SelectedIndex = 0;
		}

		private void FillCompetitorDetails(CompetitorData competitor)
		{
			//hide details:
			gbCompetitorDetails.Visible = false;
			
			//hide buttons:
			btnEditResult.Visible = false;
			
			//got any data?
			if (competitor == null)
				return;
			
			//fill competitor details:
			lbCompetitorName.Text = competitor.Player.Student.Name;
			lbResult.Text = Tools.IIF(((competitor.Rule != null)&&
				(competitor.Rule.Length > 0)), 
				competitor.ParseResult(), "---").ToString();
			lbScore.Text = Tools.IIF((competitor.Result.Length > 0), 
				competitor.Score.ToString(), "---").ToString();
			lbShirtNumber.Text = Tools.IIF((competitor.Player.ShirtNumber > 0), 
				competitor.Player.ShirtNumber.ToString(), "---").ToString();
			lbChipNumber.Text = Tools.IIF((competitor.Player.ChipNumber > 0), 
				competitor.Player.ChipNumber.ToString(), "---").ToString();
			
			gbCompetitorDetails.Visible = true;
			if (_setResultFlag)
				btnEditResult.Visible = true;
		}
		
		private void FillCompetitionDetails(CompetitionData competition)
		{
			//hide details:
			gbCompetitionDetails.Visible = false;
			
			//got any data?
			if (competition == null)
				return;
			
			//fill competition details:
			lbCompetitionTime.Text = Tools.IIF((competition.Time.Year>=1900), 
				Tools.FormatDate(competition.Time, "dd/mm/yy hh:nn"), "---").ToString();
			lbFacility.Text = Tools.IIF(((competition.Facility != null)&&
				(competition.Facility.ID >= 0)), competition.Facility.Name, "---").ToString();
			lbCourt.Text = Tools.IIF(((competition.Court != null)&&
				(competition.Court.ID >= 0)), competition.Court.Name, "---").ToString();
			
			gbCompetitionDetails.Visible = true;
		}
		
		private void FillHeatDetails(HeatData heat)
		{
			//hide details:
			gbHeatDetails.Visible = false;
			
			//got any data?
			if (heat == null)
				return;
			
			//fill competition details:
			lbHeatTime.Text = Tools.IIF((heat.Time.Year>=1900), 
				Tools.FormatDate(heat.Time, "dd/mm/yy hh:nn"), "").ToString();
			lbHeatFacility.Text = Tools.IIF(((heat.Facility != null)&&
				(heat.Facility.ID >= 0)), heat.Facility.Name, "").ToString();
			lbHeatCourt.Text = Tools.IIF(((heat.Court != null)&&
				(heat.Court.ID >= 0)), heat.Court.Name, "").ToString();
			
			gbHeatDetails.Visible = true;
		}
		#endregion

		private void SetCompetitorScore(CompetitorData competitor, 
			CompetitionData competition)
		{
			//get sport field and sport field type:
			int sportField=competition.SportField.ID;
			int sportFieldType=competition.SportField.SportFieldType.ID;

			//get result:
			string strResult=competitor.Result;

			//get rule:
			string strRule=competitor.Rule;

			//verify not empty:
			if ((strRule == null)||(strRule.Length == 0)||(strResult == null)||
				(strResult.Length == 0))
			{
				competitor.Score = 0;
				return;
			}
			
			//get result value:
			int resVal=CalculateResultValue(strRule, strResult);
			if (resVal <= 0)
			{
				competitor.Score = 0;
				return;
			}
			
			//get score range:
			ScoreRangeData scoreRange=FindScoreRange(sportField, 
				sportFieldType, competitor.Player.Student.Grade, resVal);
			
			//update score:
			if (scoreRange != null)
			{
				competitor.Score = scoreRange.Score;
			}
			
			//Sport.UI.MessageBox.Show(competitor.Score.ToString());
		}

		private ScoreRangeData FindScoreRange(int sportField, 
			int sportFieldType, int grade, int resVal)
		{
			ScoreRangeData[] arrScoreRanges=
				LocalDatabaseManager.LocalDatabase.GetScoreRangeData(sportField, sportFieldType);
			foreach (ScoreRangeData scoreRange in arrScoreRanges)
			{
				//first check the category match the competitor:
				if (CategoryMatches(scoreRange.Category, grade))
				{
					//check limits:
					if (ScoreLimitMatches(
						scoreRange.LowerLimit, scoreRange.UpperLimit, resVal))
					{
						//found!
						return scoreRange;
					}
				}
			}
			return null;
		}
		
		private bool CategoryMatches(int category, int grade)
		{
			if ((grade < 0)||(category < 0))
				return true;
			if (category == Sport.Types.CategoryTypeLookup.All)
				return true;
			return Sport.Types.CategoryTypeLookup.Contains(category, grade);
		}

		private bool ScoreLimitMatches(int lowerLimit, int upperLimit, int value)
		{
			if ((lowerLimit < 0)&&(upperLimit < 0))
				return false;
			if ((upperLimit < 0)&&(value >= lowerLimit))
				return true;
			if ((lowerLimit < 0)&&(value <= upperLimit))
				return true;
			return ((value >= lowerLimit)&&(value <= upperLimit));
		}

		private int CalculateResultValue(string strRule, string strResult)
		{
			//split rule to get result value:
			string[] ruleParts=strRule.Split(
				new char[] {Sport.Core.Data.ResultFormatSeperator})[0].Split(
				new char[] {'-'});
			if (ruleParts.Length != 3)
				return 0;

			////////////////////////
			//rule structure:
			//Value-Direction-Measure
			////////////////////////
			Sport.Core.Data.ResultValue resultValue=(Sport.Core.Data.ResultValue)
				Int32.Parse(ruleParts[0]);
			Sport.Core.Data.ResultMeasure resultMeasure=(Sport.Core.Data.ResultMeasure) 
				Int32.Parse(ruleParts[2]);
			
			//split measures into single array:
			System.Collections.ArrayList measures=
				Tools.GetResultMeasures(resultValue, resultMeasure);
			
			//get result parts:
			string[] resultParts=strResult.Split(
				new char[] {Sport.Core.Data.ResultSeperator});
			
			//verify legal result:
			if (resultParts.Length > measures.Count)
				return 0;
			
			//fill with zeros if needed:
			if (resultParts.Length < measures.Count)
			{
				string[] newResultParts=new string[measures.Count];
				int i;
				int diff=measures.Count-resultParts.Length;
				for (i=0; i<diff; i++)
					newResultParts[i] = "0";
				for (i=0; i<resultParts.Length; i++)
					newResultParts[i+diff] = resultParts[i];
				resultParts = newResultParts;
			}
			
			//calculate result value: (e.g. 06:30 minutes are 390)
			int resVal=0;
			for (int i=0; i<resultParts.Length; i++)
			{
				int iPartValue=Int32.Parse(resultParts[i]);
				if (i == (resultParts.Length-1))
				{
					resVal += iPartValue;
					continue;
				}
				Sport.Core.Data.ResultMeasure primeValue=
					(Sport.Core.Data.ResultMeasure) measures[measures.Count-1];
				Sport.Core.Data.ResultMeasure relativeValue=
					(Sport.Core.Data.ResultMeasure) measures[i];
				resVal += Sport.Rulesets.Tools.CalculateRelativeValue(
					resultValue, primeValue, relativeValue, iPartValue);
			}
			
			return resVal;
		}

		private void lbCategories_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			int selIndex=lbCategories.SelectedIndex;
			if (selIndex == _lastSelectedCategory)
				return;
			
			if (selIndex >= 0)
			{
				CategoryData category=(CategoryData) 
					(lbCategories.SelectedItem as ListItem).Value;
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
				FillCompetitions(group);
			}
			_lastSelectedGroup = selIndex;
		}

		private void lbHeats_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			int selIndex=lbHeats.SelectedIndex;
			if (selIndex == _lastSelectedHeat)
				return;
			
			if (selIndex >= 0)
			{
				HeatData heat=(HeatData) (lbHeats.SelectedItem as ListItem).Value;
				CompetitionData competition=(CompetitionData) 
					(lbCompetitions.SelectedItem as ListItem).Value;
				FillCompetitors(competition, heat.HeatIndex);
				FillHeatDetails(heat);
			}
			_lastSelectedHeat = selIndex;
		}

		private void lbCompetitions_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			int selIndex=lbCompetitions.SelectedIndex;
			if (selIndex == _lastSelectedCompetition)
				return;
			
			if (selIndex >= 0)
			{
				CompetitionData competition=(CompetitionData) 
					(lbCompetitions.SelectedItem as ListItem).Value;
				FillHeats(competition);
				FillCompetitionDetails(competition);
			}
			btnSetResults.Enabled = (selIndex >= 0);
			_lastSelectedCompetition = selIndex;
		}

		private void lbCompetitors_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			int selIndex=lbCompetitors.SelectedIndex;
			if (selIndex == _lastSelectedCompetitor)
				return;
			
			if (selIndex >= 0)
			{
				CompetitorData competitor=(CompetitorData) 
					(lbCompetitors.SelectedItem as ListItem).Value;
				FillCompetitorDetails(competitor);
			}
			_lastSelectedCompetitor = selIndex;				
		}

		private void btnSetResults_Click(object sender, System.EventArgs e)
		{
			if (_setResultFlag == false)
				return;
			
			if (lbCompetitions.SelectedIndex < 0)
				return;
			
			CompetitionData competition=(CompetitionData)
				(lbCompetitions.SelectedItem as ListItem).Value;
			
			if (competition.Competitors.Length == 0)
			{
				Sport.UI.MessageBox.Error("אין מתמודדים בתחרות זו", "עריכת תוצאות");
				return;
			}
			
			CompetitorData competitor=competition.Competitors[0];
			if ((competitor.Rule == null)||(competitor.Rule.Length == 0))
			{
				Sport.UI.MessageBox.Error("חוק סוג תוצאה לא מוגדר עבור תחרות זו", "עריכת תוצאות");
				return;
			}
			if (competitor.ResultFormat.Length == 0)
			{
				Sport.UI.MessageBox.Error("תבנית תוצאה לא מוגדרת עבור תחרות זו", "עריכת תוצאות");
				return;
			}
			
			int selCompetitorIndex=lbCompetitors.SelectedIndex;

			SetCompetitionResultsDialog _dialog=
				new SetCompetitionResultsDialog(lbChampName.Text, competition);
			if ((lbCompetitors.Items.Count > 0)&&(lbCompetitors.SelectedIndex >= 0))
			{
				_dialog.defaultCompetitor = (CompetitorData)
					(lbCompetitors.SelectedItem as ListItem).Value;
			}
			if (_dialog.ShowDialog(this) == DialogResult.OK)
			{
				GroupData group=(GroupData) (lbGroups.SelectedItem as ListItem).Value;
				PhaseData phase=(PhaseData) (lbPhases.SelectedItem as ListItem).Value;
				Sport.UI.Dialogs.WaitForm.ShowWait("שומר נתוני מתמודדים אנא המתן...");
				foreach (CompetitorData compData in competition.Competitors)
				{
					compData.Competition = competition;
					compData.Competition.Group = group;
					compData.Competition.Group.Phase = phase;
					SetCompetitorScore(compData, competition);
					LocalDatabaseManager.LocalDatabase.UpdateCompetitorData(compData);

					//save chip number:
					LocalDatabaseManager.LocalDatabase.UpdatePlayerData(compData.Player);
				}
				Sport.UI.Dialogs.WaitForm.HideWait();
				Sport.UI.MessageBox.Success("נתונים נשמרו בהצלחה", "עריכת תוצאות");

				//apply changes:
				if (lbCompetitors.Items.Count > 0)
				{
					if (lbHeats.SelectedIndex >= 0)
					{
						HeatData heat=(HeatData)
							(lbHeats.SelectedItem as ListItem).Value;
						FillCompetitors(competition, heat.HeatIndex);
						if (selCompetitorIndex >= 0)
							lbCompetitors.SelectedIndex = selCompetitorIndex;
					}
				}
			}
		}

		private void btnEditResult_Click(object sender, System.EventArgs e)
		{
			if (_setResultFlag == false)
				return;
			btnSetResults_Click(sender, e);
		}
	}
}
