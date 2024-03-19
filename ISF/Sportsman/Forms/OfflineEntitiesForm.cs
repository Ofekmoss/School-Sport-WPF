using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Sportsman.Forms
{
	/// <summary>
	/// Summary description for OfflineEntitiesForm.
	/// </summary>
	public class OfflineEntitiesForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TabPage tpStudents;
		private System.Windows.Forms.TabPage tpPlayers;
		private System.Windows.Forms.TabPage tpSchools;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox tbStudentIdNumber;
		private System.Windows.Forms.TextBox tbStudentFirstName;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox tbStudentLastName;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.ComboBox cbStudentGrade;
		private System.Windows.Forms.Button btnAdd;
		private System.Windows.Forms.Button btnExit;
		private System.Windows.Forms.ComboBox cbStudentSchool;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox tbSchoolName;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TextBox tbSchoolSymbol;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.ComboBox cbSchoolRegion;
		private System.Windows.Forms.ComboBox cbSchoolCity;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.ComboBox cbPlayerTeam;
		private System.Windows.Forms.ComboBox cbPlayerStudent;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.NumericUpDown nudPlayerNumber;
		private System.Windows.Forms.ComboBox cbTeamGroup;
		private System.Windows.Forms.ComboBox cbTeamPhase;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.ComboBox cbTeamSchool;
		private System.Windows.Forms.Label label11;
		private Sport.Championships.Championship _championship;
		private Sport.Entities.City[] _cities = null;
		private Sport.Entities.Student[] _students = null;
		private Sport.Championships.Competition[] _competitions = null;
		private System.Windows.Forms.TabControl tcData;
		private System.Windows.Forms.Control _invalidControl = null;
		private System.Windows.Forms.TabPage tpTeams;
		private System.Windows.Forms.NumericUpDown nudPlayerNumberFrom;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.NumericUpDown nudPlayerNumberTo;
		private System.Windows.Forms.Label label17;
		private System.Windows.Forms.ComboBox cbPlayerCompetition;
		private System.Windows.Forms.Label label18;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public OfflineEntitiesForm(Sport.Championships.Championship championship)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			_championship = championship;
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
			this.tcData = new System.Windows.Forms.TabControl();
			this.tpStudents = new System.Windows.Forms.TabPage();
			this.label5 = new System.Windows.Forms.Label();
			this.cbStudentSchool = new System.Windows.Forms.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.tbStudentLastName = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.tbStudentFirstName = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.tbStudentIdNumber = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.cbStudentGrade = new System.Windows.Forms.ComboBox();
			this.tpPlayers = new System.Windows.Forms.TabPage();
			this.nudPlayerNumber = new System.Windows.Forms.NumericUpDown();
			this.label8 = new System.Windows.Forms.Label();
			this.cbPlayerStudent = new System.Windows.Forms.ComboBox();
			this.cbPlayerTeam = new System.Windows.Forms.ComboBox();
			this.label12 = new System.Windows.Forms.Label();
			this.label13 = new System.Windows.Forms.Label();
			this.tpSchools = new System.Windows.Forms.TabPage();
			this.label6 = new System.Windows.Forms.Label();
			this.cbSchoolRegion = new System.Windows.Forms.ComboBox();
			this.label7 = new System.Windows.Forms.Label();
			this.tbSchoolName = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.tbSchoolSymbol = new System.Windows.Forms.TextBox();
			this.label10 = new System.Windows.Forms.Label();
			this.cbSchoolCity = new System.Windows.Forms.ComboBox();
			this.tpTeams = new System.Windows.Forms.TabPage();
			this.nudPlayerNumberTo = new System.Windows.Forms.NumericUpDown();
			this.label17 = new System.Windows.Forms.Label();
			this.nudPlayerNumberFrom = new System.Windows.Forms.NumericUpDown();
			this.label16 = new System.Windows.Forms.Label();
			this.cbTeamSchool = new System.Windows.Forms.ComboBox();
			this.label11 = new System.Windows.Forms.Label();
			this.cbTeamGroup = new System.Windows.Forms.ComboBox();
			this.cbTeamPhase = new System.Windows.Forms.ComboBox();
			this.label14 = new System.Windows.Forms.Label();
			this.label15 = new System.Windows.Forms.Label();
			this.btnAdd = new System.Windows.Forms.Button();
			this.btnExit = new System.Windows.Forms.Button();
			this.cbPlayerCompetition = new System.Windows.Forms.ComboBox();
			this.label18 = new System.Windows.Forms.Label();
			this.tcData.SuspendLayout();
			this.tpStudents.SuspendLayout();
			this.tpPlayers.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudPlayerNumber)).BeginInit();
			this.tpSchools.SuspendLayout();
			this.tpTeams.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudPlayerNumberTo)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudPlayerNumberFrom)).BeginInit();
			this.SuspendLayout();
			// 
			// tcData
			// 
			this.tcData.Controls.Add(this.tpStudents);
			this.tcData.Controls.Add(this.tpPlayers);
			this.tcData.Controls.Add(this.tpSchools);
			this.tcData.Controls.Add(this.tpTeams);
			this.tcData.Dock = System.Windows.Forms.DockStyle.Top;
			this.tcData.Location = new System.Drawing.Point(0, 0);
			this.tcData.Name = "tcData";
			this.tcData.SelectedIndex = 0;
			this.tcData.Size = new System.Drawing.Size(266, 240);
			this.tcData.TabIndex = 0;
			// 
			// tpStudents
			// 
			this.tpStudents.Controls.Add(this.label5);
			this.tpStudents.Controls.Add(this.cbStudentSchool);
			this.tpStudents.Controls.Add(this.label4);
			this.tpStudents.Controls.Add(this.tbStudentLastName);
			this.tpStudents.Controls.Add(this.label3);
			this.tpStudents.Controls.Add(this.tbStudentFirstName);
			this.tpStudents.Controls.Add(this.label2);
			this.tpStudents.Controls.Add(this.tbStudentIdNumber);
			this.tpStudents.Controls.Add(this.label1);
			this.tpStudents.Controls.Add(this.cbStudentGrade);
			this.tpStudents.Location = new System.Drawing.Point(4, 22);
			this.tpStudents.Name = "tpStudents";
			this.tpStudents.Size = new System.Drawing.Size(258, 214);
			this.tpStudents.TabIndex = 0;
			this.tpStudents.Text = "תלמידים";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(176, 160);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(72, 23);
			this.label5.TabIndex = 8;
			this.label5.Text = "כיתה:";
			// 
			// cbStudentSchool
			// 
			this.cbStudentSchool.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbStudentSchool.Location = new System.Drawing.Point(9, 115);
			this.cbStudentSchool.Name = "cbStudentSchool";
			this.cbStudentSchool.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.cbStudentSchool.Size = new System.Drawing.Size(159, 21);
			this.cbStudentSchool.TabIndex = 7;
			this.cbStudentSchool.Tag = "בית ספר";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(176, 120);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(72, 23);
			this.label4.TabIndex = 6;
			this.label4.Text = "בית ספר:";
			// 
			// tbStudentLastName
			// 
			this.tbStudentLastName.Location = new System.Drawing.Point(9, 80);
			this.tbStudentLastName.MaxLength = 20;
			this.tbStudentLastName.Name = "tbStudentLastName";
			this.tbStudentLastName.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.tbStudentLastName.Size = new System.Drawing.Size(159, 20);
			this.tbStudentLastName.TabIndex = 5;
			this.tbStudentLastName.Tag = "שם משפחה";
			this.tbStudentLastName.Text = "";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(176, 83);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(72, 23);
			this.label3.TabIndex = 4;
			this.label3.Text = "שם משפחה:";
			// 
			// tbStudentFirstName
			// 
			this.tbStudentFirstName.Location = new System.Drawing.Point(9, 48);
			this.tbStudentFirstName.MaxLength = 20;
			this.tbStudentFirstName.Name = "tbStudentFirstName";
			this.tbStudentFirstName.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.tbStudentFirstName.Size = new System.Drawing.Size(159, 20);
			this.tbStudentFirstName.TabIndex = 3;
			this.tbStudentFirstName.Tag = "שם פרטי";
			this.tbStudentFirstName.Text = "";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(176, 50);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(72, 23);
			this.label2.TabIndex = 2;
			this.label2.Text = "שם פרטי:";
			// 
			// tbStudentIdNumber
			// 
			this.tbStudentIdNumber.Location = new System.Drawing.Point(8, 14);
			this.tbStudentIdNumber.MaxLength = 12;
			this.tbStudentIdNumber.Name = "tbStudentIdNumber";
			this.tbStudentIdNumber.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.tbStudentIdNumber.Size = new System.Drawing.Size(160, 20);
			this.tbStudentIdNumber.TabIndex = 1;
			this.tbStudentIdNumber.Tag = "מספר זהות";
			this.tbStudentIdNumber.Text = "";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(176, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(72, 23);
			this.label1.TabIndex = 0;
			this.label1.Text = "מספר ת\"ז:";
			// 
			// cbStudentGrade
			// 
			this.cbStudentGrade.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbStudentGrade.Location = new System.Drawing.Point(96, 155);
			this.cbStudentGrade.Name = "cbStudentGrade";
			this.cbStudentGrade.Size = new System.Drawing.Size(72, 21);
			this.cbStudentGrade.TabIndex = 1;
			this.cbStudentGrade.Tag = "כיתה";
			// 
			// tpPlayers
			// 
			this.tpPlayers.Controls.Add(this.cbPlayerCompetition);
			this.tpPlayers.Controls.Add(this.label18);
			this.tpPlayers.Controls.Add(this.nudPlayerNumber);
			this.tpPlayers.Controls.Add(this.label8);
			this.tpPlayers.Controls.Add(this.cbPlayerStudent);
			this.tpPlayers.Controls.Add(this.cbPlayerTeam);
			this.tpPlayers.Controls.Add(this.label12);
			this.tpPlayers.Controls.Add(this.label13);
			this.tpPlayers.Location = new System.Drawing.Point(4, 22);
			this.tpPlayers.Name = "tpPlayers";
			this.tpPlayers.Size = new System.Drawing.Size(258, 214);
			this.tpPlayers.TabIndex = 1;
			this.tpPlayers.Text = "שחקנים";
			// 
			// nudPlayerNumber
			// 
			this.nudPlayerNumber.Location = new System.Drawing.Point(112, 136);
			this.nudPlayerNumber.Maximum = new System.Decimal(new int[] {
																			99999,
																			0,
																			0,
																			0});
			this.nudPlayerNumber.Name = "nudPlayerNumber";
			this.nudPlayerNumber.Size = new System.Drawing.Size(56, 20);
			this.nudPlayerNumber.TabIndex = 30;
			this.nudPlayerNumber.Tag = "מספר חולצה";
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(176, 136);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(72, 23);
			this.label8.TabIndex = 29;
			this.label8.Text = "מספר חולצה:";
			// 
			// cbPlayerStudent
			// 
			this.cbPlayerStudent.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbPlayerStudent.Location = new System.Drawing.Point(8, 96);
			this.cbPlayerStudent.Name = "cbPlayerStudent";
			this.cbPlayerStudent.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.cbPlayerStudent.Size = new System.Drawing.Size(160, 21);
			this.cbPlayerStudent.TabIndex = 28;
			this.cbPlayerStudent.Tag = "תלמיד";
			// 
			// cbPlayerTeam
			// 
			this.cbPlayerTeam.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbPlayerTeam.Location = new System.Drawing.Point(8, 16);
			this.cbPlayerTeam.Name = "cbPlayerTeam";
			this.cbPlayerTeam.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.cbPlayerTeam.Size = new System.Drawing.Size(160, 21);
			this.cbPlayerTeam.TabIndex = 27;
			this.cbPlayerTeam.Tag = "קבוצה";
			this.cbPlayerTeam.SelectedIndexChanged += new System.EventHandler(this.cbPlayerTeam_SelectedIndexChanged);
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(176, 96);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(72, 23);
			this.label12.TabIndex = 22;
			this.label12.Text = "תלמיד:";
			// 
			// label13
			// 
			this.label13.Location = new System.Drawing.Point(176, 16);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(72, 23);
			this.label13.TabIndex = 19;
			this.label13.Text = "קבוצה:";
			// 
			// tpSchools
			// 
			this.tpSchools.Controls.Add(this.label6);
			this.tpSchools.Controls.Add(this.cbSchoolRegion);
			this.tpSchools.Controls.Add(this.label7);
			this.tpSchools.Controls.Add(this.tbSchoolName);
			this.tpSchools.Controls.Add(this.label9);
			this.tpSchools.Controls.Add(this.tbSchoolSymbol);
			this.tpSchools.Controls.Add(this.label10);
			this.tpSchools.Controls.Add(this.cbSchoolCity);
			this.tpSchools.Location = new System.Drawing.Point(4, 22);
			this.tpSchools.Name = "tpSchools";
			this.tpSchools.Size = new System.Drawing.Size(258, 214);
			this.tpSchools.TabIndex = 2;
			this.tpSchools.Text = "בתי ספר";
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(176, 128);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(72, 23);
			this.label6.TabIndex = 18;
			this.label6.Text = "ישוב:";
			// 
			// cbSchoolRegion
			// 
			this.cbSchoolRegion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbSchoolRegion.Location = new System.Drawing.Point(10, 86);
			this.cbSchoolRegion.Name = "cbSchoolRegion";
			this.cbSchoolRegion.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.cbSchoolRegion.Size = new System.Drawing.Size(158, 21);
			this.cbSchoolRegion.TabIndex = 17;
			this.cbSchoolRegion.Tag = "מחוז";
			this.cbSchoolRegion.SelectedIndexChanged += new System.EventHandler(this.cbSchoolRegion_SelectedIndexChanged);
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(176, 88);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(72, 23);
			this.label7.TabIndex = 16;
			this.label7.Text = "מחוז:";
			// 
			// tbSchoolName
			// 
			this.tbSchoolName.Location = new System.Drawing.Point(10, 46);
			this.tbSchoolName.MaxLength = 50;
			this.tbSchoolName.Name = "tbSchoolName";
			this.tbSchoolName.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.tbSchoolName.Size = new System.Drawing.Size(158, 20);
			this.tbSchoolName.TabIndex = 13;
			this.tbSchoolName.Tag = "שם בית ספר";
			this.tbSchoolName.Text = "";
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(176, 48);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(72, 23);
			this.label9.TabIndex = 12;
			this.label9.Text = "שם בית ספר:";
			// 
			// tbSchoolSymbol
			// 
			this.tbSchoolSymbol.Location = new System.Drawing.Point(9, 14);
			this.tbSchoolSymbol.MaxLength = 10;
			this.tbSchoolSymbol.Name = "tbSchoolSymbol";
			this.tbSchoolSymbol.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.tbSchoolSymbol.Size = new System.Drawing.Size(159, 20);
			this.tbSchoolSymbol.TabIndex = 10;
			this.tbSchoolSymbol.Tag = "סמל בית ספר";
			this.tbSchoolSymbol.Text = "";
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(176, 16);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(72, 23);
			this.label10.TabIndex = 9;
			this.label10.Text = "סמל בית ספר:";
			// 
			// cbSchoolCity
			// 
			this.cbSchoolCity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbSchoolCity.Location = new System.Drawing.Point(8, 125);
			this.cbSchoolCity.Name = "cbSchoolCity";
			this.cbSchoolCity.Size = new System.Drawing.Size(160, 21);
			this.cbSchoolCity.TabIndex = 11;
			this.cbSchoolCity.Tag = "ישוב";
			// 
			// tpTeams
			// 
			this.tpTeams.Controls.Add(this.nudPlayerNumberTo);
			this.tpTeams.Controls.Add(this.label17);
			this.tpTeams.Controls.Add(this.nudPlayerNumberFrom);
			this.tpTeams.Controls.Add(this.label16);
			this.tpTeams.Controls.Add(this.cbTeamSchool);
			this.tpTeams.Controls.Add(this.label11);
			this.tpTeams.Controls.Add(this.cbTeamGroup);
			this.tpTeams.Controls.Add(this.cbTeamPhase);
			this.tpTeams.Controls.Add(this.label14);
			this.tpTeams.Controls.Add(this.label15);
			this.tpTeams.Location = new System.Drawing.Point(4, 22);
			this.tpTeams.Name = "tpTeams";
			this.tpTeams.Size = new System.Drawing.Size(258, 214);
			this.tpTeams.TabIndex = 3;
			this.tpTeams.Text = "קבוצות";
			// 
			// nudPlayerNumberTo
			// 
			this.nudPlayerNumberTo.Location = new System.Drawing.Point(105, 157);
			this.nudPlayerNumberTo.Maximum = new System.Decimal(new int[] {
																			  99999,
																			  0,
																			  0,
																			  0});
			this.nudPlayerNumberTo.Minimum = new System.Decimal(new int[] {
																			  1,
																			  0,
																			  0,
																			  -2147483648});
			this.nudPlayerNumberTo.Name = "nudPlayerNumberTo";
			this.nudPlayerNumberTo.Size = new System.Drawing.Size(56, 20);
			this.nudPlayerNumberTo.TabIndex = 40;
			this.nudPlayerNumberTo.Tag = "מספר חולצה";
			// 
			// label17
			// 
			this.label17.Location = new System.Drawing.Point(160, 160);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(88, 23);
			this.label17.TabIndex = 39;
			this.label17.Text = "עד מספר חולצה:";
			// 
			// nudPlayerNumberFrom
			// 
			this.nudPlayerNumberFrom.Location = new System.Drawing.Point(105, 125);
			this.nudPlayerNumberFrom.Maximum = new System.Decimal(new int[] {
																				99999,
																				0,
																				0,
																				0});
			this.nudPlayerNumberFrom.Minimum = new System.Decimal(new int[] {
																				1,
																				0,
																				0,
																				-2147483648});
			this.nudPlayerNumberFrom.Name = "nudPlayerNumberFrom";
			this.nudPlayerNumberFrom.Size = new System.Drawing.Size(56, 20);
			this.nudPlayerNumberFrom.TabIndex = 38;
			this.nudPlayerNumberFrom.Tag = "מספר חולצה";
			// 
			// label16
			// 
			this.label16.Location = new System.Drawing.Point(176, 128);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(72, 23);
			this.label16.TabIndex = 37;
			this.label16.Text = "ממספר חולצה:";
			// 
			// cbTeamSchool
			// 
			this.cbTeamSchool.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbTeamSchool.Location = new System.Drawing.Point(8, 88);
			this.cbTeamSchool.Name = "cbTeamSchool";
			this.cbTeamSchool.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.cbTeamSchool.Size = new System.Drawing.Size(160, 21);
			this.cbTeamSchool.TabIndex = 36;
			this.cbTeamSchool.Tag = "בית ספר";
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(176, 88);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(72, 23);
			this.label11.TabIndex = 35;
			this.label11.Text = "בית ספר:";
			// 
			// cbTeamGroup
			// 
			this.cbTeamGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbTeamGroup.Location = new System.Drawing.Point(8, 48);
			this.cbTeamGroup.Name = "cbTeamGroup";
			this.cbTeamGroup.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.cbTeamGroup.Size = new System.Drawing.Size(160, 21);
			this.cbTeamGroup.TabIndex = 34;
			this.cbTeamGroup.Tag = "בית";
			this.cbTeamGroup.SelectedIndexChanged += new System.EventHandler(this.cbTeamGroup_SelectedIndexChanged);
			// 
			// cbTeamPhase
			// 
			this.cbTeamPhase.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbTeamPhase.Location = new System.Drawing.Point(8, 16);
			this.cbTeamPhase.Name = "cbTeamPhase";
			this.cbTeamPhase.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.cbTeamPhase.Size = new System.Drawing.Size(160, 21);
			this.cbTeamPhase.TabIndex = 33;
			this.cbTeamPhase.Tag = "שלב";
			this.cbTeamPhase.SelectedIndexChanged += new System.EventHandler(this.cbTeamPhase_SelectedIndexChanged);
			// 
			// label14
			// 
			this.label14.Location = new System.Drawing.Point(176, 48);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(72, 23);
			this.label14.TabIndex = 32;
			this.label14.Text = "בית:";
			// 
			// label15
			// 
			this.label15.Location = new System.Drawing.Point(176, 16);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(72, 23);
			this.label15.TabIndex = 31;
			this.label15.Text = "שלב:";
			// 
			// btnAdd
			// 
			this.btnAdd.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnAdd.Location = new System.Drawing.Point(184, 240);
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.TabIndex = 1;
			this.btnAdd.Text = "הוסף";
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			// 
			// btnExit
			// 
			this.btnExit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnExit.Location = new System.Drawing.Point(8, 264);
			this.btnExit.Name = "btnExit";
			this.btnExit.TabIndex = 2;
			this.btnExit.Text = "יציאה";
			// 
			// cbPlayerCompetition
			// 
			this.cbPlayerCompetition.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbPlayerCompetition.Location = new System.Drawing.Point(8, 56);
			this.cbPlayerCompetition.Name = "cbPlayerCompetition";
			this.cbPlayerCompetition.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.cbPlayerCompetition.Size = new System.Drawing.Size(160, 21);
			this.cbPlayerCompetition.TabIndex = 32;
			this.cbPlayerCompetition.Tag = "תחרות";
			// 
			// label18
			// 
			this.label18.Location = new System.Drawing.Point(176, 56);
			this.label18.Name = "label18";
			this.label18.Size = new System.Drawing.Size(72, 23);
			this.label18.TabIndex = 31;
			this.label18.Text = "תחרות:";
			// 
			// OfflineEntitiesForm
			// 
			this.AcceptButton = this.btnAdd;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.btnExit;
			this.ClientSize = new System.Drawing.Size(266, 296);
			this.Controls.Add(this.btnExit);
			this.Controls.Add(this.btnAdd);
			this.Controls.Add(this.tcData);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "OfflineEntitiesForm";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "ניהול מאגר לא מקוון";
			this.Load += new System.EventHandler(this.OfflineEntitiesForm_Load);
			this.tcData.ResumeLayout(false);
			this.tpStudents.ResumeLayout(false);
			this.tpPlayers.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.nudPlayerNumber)).EndInit();
			this.tpSchools.ResumeLayout(false);
			this.tpTeams.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.nudPlayerNumberTo)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudPlayerNumberFrom)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		private void OfflineEntitiesForm_Load(object sender, System.EventArgs e)
		{
			Sport.UI.Dialogs.WaitForm.ShowWait("טוען נתונים אנא המתן...", true);
			Sport.UI.Dialogs.WaitForm.SetProgress(0);
			
			ArrayList arrPhases = new ArrayList();
			ArrayList arrGroups = new ArrayList();
			ArrayList arrTeams = new ArrayList();
			ArrayList arrSchools = new ArrayList();
			ArrayList arrRegions = new ArrayList();
			ArrayList arrCities = new ArrayList();
			ArrayList arrStudents = new ArrayList();
			ArrayList arrCompetitions = new ArrayList();
			double curProgress = 0;
			double phaseProgress = (_championship.Phases.Count == 0)?0:(((double) 90)/_championship.Phases.Count);
			foreach (Sport.Championships.CompetitionPhase phase in _championship.Phases)
			{
				arrPhases.Add(phase);
				double groupProgress = (phase.Groups.Count == 0)?0:(phaseProgress/phase.Groups.Count);
				foreach (Sport.Championships.CompetitionGroup group in phase.Groups)
				{
					arrGroups.Add(group);
					double teamProgress = (group.Teams.Count == 0)?0:((groupProgress*0.5)/group.Teams.Count);
					double competitionProgress = (group.Competitions.Count == 0)?0:((groupProgress*0.5)/group.Competitions.Count);
					foreach (Sport.Championships.CompetitionTeam compTeam in group.Teams)
					{
						curProgress += teamProgress;
						Sport.UI.Dialogs.WaitForm.SetProgress((int) curProgress);
						Sport.Entities.Team team = compTeam.TeamEntity;
						if (team == null)
							continue;
						arrTeams.Add(team);
						Sport.Entities.School school = team.School;
						if (school == null)
							continue;
						if (arrSchools.IndexOf(school) < 0)
								arrSchools.Add(school);
						Sport.Entities.Region region = school.Region;
						if (region == null)
							continue;
						if (arrRegions.IndexOf(region) < 0)
							arrRegions.Add(region);
						Sport.Entities.City city = school.City;
						if (city == null)
							continue;
						if (arrCities.IndexOf(city) < 0)
							arrCities.Add(city);
					}
					foreach (Sport.Championships.Competition competition in group.Competitions)
					{
						arrCompetitions.Add(competition);
						double competitorProgress = (competition.Competitors.Count == 0)?0:(competitionProgress/competition.Competitors.Count);
						foreach (Sport.Championships.Competitor competitor in competition.Competitors)
						{
							curProgress += competitorProgress;
							Sport.UI.Dialogs.WaitForm.SetProgress((int) curProgress);
							if ((competitor.Player != null)&&(competitor.Player.PlayerEntity != null))
							{
								Sport.Entities.Student student = competitor.Player.PlayerEntity.Student;
								if ((student != null)&&(arrStudents.IndexOf(student) < 0))
									arrStudents.Add(student);
							}
						}
					}
				}
			}
			Sport.UI.Dialogs.WaitForm.SetProgress(90);
			
			cbStudentSchool.Items.Clear();
			foreach (Sport.Entities.School school in arrSchools)
				cbStudentSchool.Items.Add(school.Entity);
			
			cbStudentGrade.Items.Clear();
			cbStudentGrade.Items.AddRange(
				(new Sport.Types.GradeTypeLookup(true)).Items);
			
			cbSchoolRegion.Items.Clear();
			foreach (Sport.Entities.Region region in arrRegions)
				cbSchoolRegion.Items.Add(region.Entity);
			
			cbPlayerTeam.Items.Clear();
			foreach (Sport.Entities.Team team in arrTeams)
				cbPlayerTeam.Items.Add(team.Entity);
			
			cbTeamPhase.Items.Clear();
			foreach (Sport.Championships.CompetitionPhase phase in arrPhases)
				cbTeamPhase.Items.Add(phase);
			
			ReloadOfflineEntities(typeof(Sport.Entities.OfflineSchool), cbStudentSchool);
			ReloadOfflineEntities(typeof(Sport.Entities.OfflineTeam), cbPlayerTeam);
			
			_cities = (Sport.Entities.City[])
				arrCities.ToArray(typeof(Sport.Entities.City));
			
			_students = (Sport.Entities.Student[])
				arrStudents.ToArray(typeof(Sport.Entities.Student));
			
			_competitions = (Sport.Championships.Competition[])
				arrCompetitions.ToArray(typeof(Sport.Championships.Competition));
			
			Sport.UI.Dialogs.WaitForm.SetProgress(100);
			Sport.UI.Dialogs.WaitForm.HideWait();
		}
		
		private void ReloadOfflineEntities(System.Type type, ComboBox combo)
		{
			while (combo.Items.Count > 0)
			{
				if (!combo.Items[combo.Items.Count-1].GetType().FullName.Equals(type.FullName))
					break;
				combo.Items.RemoveAt(combo.Items.Count-1);
			}
			Sport.Data.OfflineEntity[] arrOfflineEntities = 
				Sport.Data.OfflineEntity.LoadAllEntities(type);
			combo.Items.AddRange(arrOfflineEntities);
		}
		
		private void cbPlayerTeam_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (cbPlayerTeam.SelectedIndex < 0)
				return;
			
			Sport.Entities.Team team = null;
			Sport.Entities.OfflineTeam offlineTeam = null;
			
			object oTeam = cbPlayerTeam.SelectedItem;
			if (oTeam is Sport.Data.Entity)
				team = new Sport.Entities.Team(oTeam as Sport.Data.Entity);
			else if (oTeam is Sport.Entities.OfflineTeam)
				offlineTeam = (Sport.Entities.OfflineTeam) oTeam;
			
			int pNumFrom = -1;
			int pNumTo = -1;
			
			int schoolID = -1;
			int offlineSchoolID = -1;
			
			int champCategoryID = -1;
			if (team != null)
			{
				pNumFrom = team.PlayerNumberFrom;
				pNumTo = team.PlayerNumberTo;
				schoolID = team.School.Id;
				champCategoryID = team.Category.Id;
			}
			else if (offlineTeam != null)
			{
				pNumFrom = offlineTeam.PlayerNumberFrom;
				pNumTo = offlineTeam.PlayerNumberTo;
				if (offlineTeam.School != null)
					schoolID = offlineTeam.School.Id;
				else if (offlineTeam.OfflineSchool != null)
					offlineSchoolID = offlineTeam.OfflineSchool.OfflineID;
				champCategoryID = offlineTeam.ChampionshipCategory;
			}
			
			if ((pNumFrom > 0)&&(pNumTo > 0))
			{
				nudPlayerNumber.Minimum = (decimal) pNumFrom;
				nudPlayerNumber.Maximum = (decimal) pNumTo;
				nudPlayerNumber.Value = (decimal) pNumFrom;
			}
			
			cbPlayerStudent.Items.Clear();
			Sport.Data.OfflineEntity[] arrOfflineStudents = 
				Sport.Data.OfflineEntity.LoadAllEntities(typeof(Sport.Entities.OfflineStudent));
			if (schoolID >= 0)
			{
				foreach (Sport.Entities.Student student in _students)
					if (student.School.Id == schoolID)
						cbPlayerStudent.Items.Add(student.Entity);
				foreach (Sport.Entities.OfflineStudent offlineStudent in arrOfflineStudents)
					if ((offlineStudent.School != null)&&(offlineStudent.School.Id == schoolID))
						cbPlayerStudent.Items.Add(offlineStudent);
			}
			else if (offlineSchoolID >= 0)
			{
				foreach (Sport.Entities.OfflineStudent offlineStudent in arrOfflineStudents)
					if ((offlineStudent.OfflineSchool != null)&&(offlineStudent.OfflineSchool.OfflineID == offlineSchoolID))
						cbPlayerStudent.Items.Add(offlineStudent);
			}
			
			cbPlayerCompetition.Items.Clear();
			foreach (Sport.Championships.Competition competition in _competitions)
			{
				if (competition.Group.Phase.Championship.CategoryID != champCategoryID)
					continue;
				cbPlayerCompetition.Items.Add(competition);
			}
		}
		
		private void cbSchoolRegion_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			cbSchoolCity.Items.Clear();
			if (cbSchoolRegion.SelectedIndex < 0)
				return;
			Sport.Entities.Region region = new Sport.Entities.Region(
				cbSchoolRegion.SelectedItem as Sport.Data.Entity);
			foreach (Sport.Entities.City city in _cities)
			{
				if ((city.Region != null)&&(city.Region.Id == region.Id))
					cbSchoolCity.Items.Add(city.Entity);
			}
		}

		private void cbTeamPhase_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			cbTeamGroup.Items.Clear();
			cbTeamSchool.Items.Clear();
			if (cbTeamPhase.SelectedIndex < 0)
				return;
			Sport.Championships.CompetitionPhase phase = 
				(Sport.Championships.CompetitionPhase) cbTeamPhase.SelectedItem;
			foreach (Sport.Championships.CompetitionGroup group in phase.Groups)
				cbTeamGroup.Items.Add(group);
		}

		private void cbTeamGroup_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			cbTeamSchool.Items.Clear();
			if (cbTeamGroup.SelectedIndex < 0)
				return;
			Sport.Championships.CompetitionGroup group = 
				(Sport.Championships.CompetitionGroup) cbTeamGroup.SelectedItem;
			ArrayList arrGroupSchools = new ArrayList();
			foreach (Sport.Championships.CompetitionTeam compTeam in group.Teams)
			{
				Sport.Entities.Team team = compTeam.TeamEntity;
				if ((team != null)&&(team.School != null))
				{
					Sport.Entities.School school = team.School;
					if (arrGroupSchools.IndexOf(school.Entity) < 0)
						arrGroupSchools.Add(school.Entity);
				}
			}
			Sport.Entities.OfflineTeam[] arrOfflineTeams = group.GetOfflineTeams();
			foreach (Sport.Entities.OfflineTeam offlineTeam in arrOfflineTeams)
			{
				if (offlineTeam.School != null)
				{
					arrGroupSchools.Add(offlineTeam.School.Entity);
				}
				else if (offlineTeam.OfflineSchool != null)
				{
					arrGroupSchools.Add(offlineTeam.OfflineSchool);
				}
			}
			for (int i=0; i<cbStudentSchool.Items.Count; i++)
			{
				object oSchool = cbStudentSchool.Items[i];
				if (oSchool is Sport.Data.Entity)
				{
					Sport.Data.Entity ent = (oSchool as Sport.Data.Entity);
					if (arrGroupSchools.IndexOf(ent) < 0)
						cbTeamSchool.Items.Add(ent);
				}
				else if (oSchool is Sport.Entities.OfflineSchool)
				{
					if (arrGroupSchools.IndexOf(oSchool as Sport.Entities.OfflineSchool) < 0)
						cbTeamSchool.Items.Add(oSchool);
				}
			}
		}

		private void btnAdd_Click(object sender, System.EventArgs e)
		{
			if (!VerifyData())
				return;
			
			Sport.Data.OfflineEntity entity = null;
			System.Type type = null;
			ComboBox combo = null;
			if (tcData.SelectedTab == tpStudents)
			{
				string strIdNumber = tbStudentIdNumber.Text;
				string strFirstName = tbStudentFirstName.Text;
				string strLastName = tbStudentLastName.Text;
				object objSchool = cbStudentSchool.SelectedItem;
				int grade = (cbStudentGrade.SelectedItem as Sport.Data.LookupItem).Id;
				entity = new Sport.Entities.OfflineStudent(
					strIdNumber, strFirstName, strLastName, grade, objSchool);
				cbPlayerTeam_SelectedIndexChanged(sender, e);
			}
			else if (tcData.SelectedTab == tpPlayers)
			{
				object objTeam = cbPlayerTeam.SelectedItem;
				object objStudent = cbPlayerStudent.SelectedItem;
				int shirtNumber = (int) nudPlayerNumber.Value;
				Sport.Championships.Competition competition = 
					(Sport.Championships.Competition) cbPlayerCompetition.SelectedItem;
				int compIndex = competition.Index;
				entity = new Sport.Entities.OfflinePlayer(
					objTeam, objStudent, shirtNumber, compIndex);
			}
			else if (tcData.SelectedTab == tpSchools)
			{
				string strSymbol = tbSchoolSymbol.Text;
				string strName = tbSchoolName.Text;
				int region = (cbSchoolRegion.SelectedItem as Sport.Data.Entity).Id;
				int city = -1;
				if (cbSchoolCity.SelectedItem != null)
					city = (cbSchoolCity.SelectedItem as Sport.Data.Entity).Id;
				entity = new Sport.Entities.OfflineSchool(
					strSymbol, strName, region, city);
				type = typeof(Sport.Entities.OfflineSchool);
				combo = cbStudentSchool;
			}
			else if (tcData.SelectedTab == tpTeams)
			{
				int champCategoryID = _championship.ChampionshipCategory.Id;
				int phase = (cbTeamPhase.SelectedItem as Sport.Championships.CompetitionPhase).Index;
				int group = (cbTeamGroup.SelectedItem as Sport.Championships.CompetitionGroup).Index;
				object objSchool = cbTeamSchool.SelectedItem;
				int pNumFrom = (int) nudPlayerNumberFrom.Value;
				int pNumTo = (int) nudPlayerNumberTo.Value;
				if ((pNumFrom > 0)||(pNumTo > 0))
				{
					string strInvalidRangeMessage = "";
					if (pNumTo < pNumFrom)
					{
						strInvalidRangeMessage = "טווח מספרי חולצה לא חוקי";
					}
					else
					{
						string strSchoolName = "";
						foreach (object oItem in cbPlayerTeam.Items)
						{
							int curNumFrom = -1;
							int curNumTo = -1;
							if (oItem is Sport.Data.Entity)
							{
								Sport.Entities.Team team = new Sport.Entities.Team(
									oItem as Sport.Data.Entity);
								curNumFrom = team.PlayerNumberFrom;
								curNumTo = team.PlayerNumberTo;
								strSchoolName = team.School.Name;
							}
							else if (oItem is Sport.Entities.OfflineTeam)
							{
								Sport.Entities.OfflineTeam team = 
									(Sport.Entities.OfflineTeam) oItem;
								curNumFrom = team.PlayerNumberFrom;
								curNumTo = team.PlayerNumberTo;
								if (team.School != null)
								{
									strSchoolName = team.School.Name;
								}
								else if (team.OfflineSchool != null)
								{
									int offlineSchoolID = team.OfflineSchool.OfflineID;
									Sport.Entities.OfflineSchool offlineSchool = 
										(Sport.Entities.OfflineSchool)
										Sport.Data.OfflineEntity.LoadEntity(
										typeof(Sport.Entities.OfflineSchool), offlineSchoolID);
									strSchoolName = offlineSchool.Name;
								}
							}
							if ((pNumFrom <= curNumTo)&&
								(pNumTo >= curNumFrom))
							{
								strInvalidRangeMessage = "טווח מספרי חולצה מתנגש "+
									"עם טווח של '"+strSchoolName+"'";
							}
						}
					}
					if (strInvalidRangeMessage.Length > 0)
					{
						Sport.UI.MessageBox.Error(strInvalidRangeMessage, "ניהול מאגר לא מקוון");
						return;
					}
				}
				entity = new Sport.Entities.OfflineTeam(
					champCategoryID, phase, group, objSchool, pNumFrom, pNumTo);
				type = typeof(Sport.Entities.OfflineTeam);
				combo = cbPlayerTeam;
			}
			
			if (entity != null)
			{
				string strMessage = "";
				if (entity.Save())
					strMessage = "נתונים נוספו בהצלחה למאגר הנתונים הלא מקוון";
				else
					strMessage = "נתוני מאגר נתונים מקומי עודכנו בהצלחה";
				Sport.UI.MessageBox.Success(strMessage, "ניהול מאגר נתונים לא מקוון");
				if (combo != null)
					ReloadOfflineEntities(type, combo);
			}
		}
		
		private bool VerifyData()
		{
			try
			{
				if (tcData.SelectedTab == tpStudents)
				{
					VerifySingleControl(tbStudentIdNumber);
					VerifySingleControl(tbStudentFirstName);
					VerifySingleControl(cbStudentSchool);
					VerifySingleControl(cbStudentGrade);
				}
				else if (tcData.SelectedTab == tpPlayers)
				{
					VerifySingleControl(cbPlayerTeam);
					VerifySingleControl(cbPlayerCompetition);
					VerifySingleControl(cbPlayerStudent);
				}
				else if (tcData.SelectedTab == tpSchools)
				{
					VerifySingleControl(tbSchoolSymbol);
					VerifySingleControl(tbSchoolName);
					VerifySingleControl(cbSchoolRegion);
				}
				else if (tcData.SelectedTab == tpTeams)
				{
					VerifySingleControl(cbTeamPhase);
					VerifySingleControl(cbTeamGroup);
					VerifySingleControl(cbTeamSchool);
				}
			}
			catch (Exception ex)
			{
				Sport.UI.MessageBox.Error(ex.Message, "ניהול מאגר נתונים");
				if (_invalidControl != null)
				{
					_invalidControl.Focus();
				}
				return false;
			}
			return true;
		}

		private void VerifySingleControl(Control control)
		{
			bool blnValid = true;
			_invalidControl = null;
			string strChooseTxt = "";
			if (control is TextBox)
			{
				strChooseTxt = "הזן";
				if ((control as TextBox).Text.Length == 0)
					blnValid = false;
			}
			else if (control is ComboBox)
			{
				strChooseTxt = "בחר";
				if ((control as ComboBox).SelectedIndex < 0)
					blnValid = false;
			}
			if (!blnValid)
			{
				string strMessage = "אנא "+strChooseTxt+" "+
					Sport.Common.Tools.CStrDef(control.Tag, "");
				//control.Focus();
				_invalidControl = control;
				throw new Exception(strMessage);
			}
		}
	}
}
