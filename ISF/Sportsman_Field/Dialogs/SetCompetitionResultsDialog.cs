using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Sport.Common;

namespace Sportsman_Field.Dialogs
{
	/// <summary>
	/// Summary description for SetCompetitionResultsDialog.
	/// </summary>
	public class SetCompetitionResultsDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.ListBox lbCompetitors;
		private System.Windows.Forms.Label lbChampName;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox tb_IdNumber;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label lbHeat;
		private System.Windows.Forms.Label lbTeamName;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label lbMeasureType;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOK;
		private System.ComponentModel.IContainer components;
		private bool _changedByCode=false;
		private string _champName="";
		private System.Windows.Forms.GroupBox gbCompetitorDetails;
		private System.Windows.Forms.Label lbPlayerName;
		private System.Windows.Forms.Panel pnlResult;
		private CompetitionData _competition=null;
		private System.Windows.Forms.Timer timer1;
		private ArrayList _resultControls=null;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox tbChipNumber;
		private System.Windows.Forms.PictureBox pbDigit1;
		private System.Windows.Forms.PictureBox pbDigit2;
		private System.Windows.Forms.PictureBox pbDigit3;
		private System.Windows.Forms.PictureBox img_0;
		private System.Windows.Forms.PictureBox img_1;
		private System.Windows.Forms.PictureBox img_2;
		private System.Windows.Forms.PictureBox img_3;
		private System.Windows.Forms.PictureBox img_4;
		private System.Windows.Forms.PictureBox img_5;
		private System.Windows.Forms.PictureBox img_6;
		private System.Windows.Forms.PictureBox img_7;
		private System.Windows.Forms.PictureBox img_9;
		private System.Windows.Forms.PictureBox img_8;
		private System.Windows.Forms.GroupBox gbShirtNumber;
		public CompetitorData defaultCompetitor=null;
		private Image[] _digits=null;
		
		public SetCompetitionResultsDialog(string champName, 
			CompetitionData competition)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			_champName = champName;
			_competition = competition;
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
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(SetCompetitionResultsDialog));
			this.lbCompetitors = new System.Windows.Forms.ListBox();
			this.lbChampName = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.tb_IdNumber = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.gbCompetitorDetails = new System.Windows.Forms.GroupBox();
			this.pnlResult = new System.Windows.Forms.Panel();
			this.lbPlayerName = new System.Windows.Forms.Label();
			this.lbMeasureType = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.lbTeamName = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.lbHeat = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.label6 = new System.Windows.Forms.Label();
			this.tbChipNumber = new System.Windows.Forms.TextBox();
			this.gbShirtNumber = new System.Windows.Forms.GroupBox();
			this.pbDigit1 = new System.Windows.Forms.PictureBox();
			this.pbDigit2 = new System.Windows.Forms.PictureBox();
			this.pbDigit3 = new System.Windows.Forms.PictureBox();
			this.img_0 = new System.Windows.Forms.PictureBox();
			this.img_1 = new System.Windows.Forms.PictureBox();
			this.img_2 = new System.Windows.Forms.PictureBox();
			this.img_3 = new System.Windows.Forms.PictureBox();
			this.img_4 = new System.Windows.Forms.PictureBox();
			this.img_5 = new System.Windows.Forms.PictureBox();
			this.img_6 = new System.Windows.Forms.PictureBox();
			this.img_7 = new System.Windows.Forms.PictureBox();
			this.img_9 = new System.Windows.Forms.PictureBox();
			this.img_8 = new System.Windows.Forms.PictureBox();
			this.groupBox1.SuspendLayout();
			this.gbCompetitorDetails.SuspendLayout();
			this.gbShirtNumber.SuspendLayout();
			this.SuspendLayout();
			// 
			// lbCompetitors
			// 
			this.lbCompetitors.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbCompetitors.ForeColor = System.Drawing.Color.Blue;
			this.lbCompetitors.ItemHeight = 14;
			this.lbCompetitors.Items.AddRange(new object[] {
															   "אבי כהן אהרונוב"});
			this.lbCompetitors.Location = new System.Drawing.Point(8, 48);
			this.lbCompetitors.Name = "lbCompetitors";
			this.lbCompetitors.Size = new System.Drawing.Size(112, 242);
			this.lbCompetitors.TabIndex = 3;
			this.lbCompetitors.SelectedIndexChanged += new System.EventHandler(this.lbCompetitors_SelectedIndexChanged);
			// 
			// lbChampName
			// 
			this.lbChampName.Font = new System.Drawing.Font("Tahoma", 10F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbChampName.ForeColor = System.Drawing.Color.Blue;
			this.lbChampName.Location = new System.Drawing.Point(8, 0);
			this.lbChampName.Name = "lbChampName";
			this.lbChampName.Size = new System.Drawing.Size(416, 23);
			this.lbChampName.TabIndex = 15;
			this.lbChampName.Text = "שם אליפות, תחרות";
			this.lbChampName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.tb_IdNumber);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.lbCompetitors);
			this.groupBox1.Location = new System.Drawing.Point(288, 24);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(128, 296);
			this.groupBox1.TabIndex = 16;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "מתמודדים";
			// 
			// tb_IdNumber
			// 
			this.tb_IdNumber.ForeColor = System.Drawing.Color.Blue;
			this.tb_IdNumber.Location = new System.Drawing.Point(8, 24);
			this.tb_IdNumber.MaxLength = 10;
			this.tb_IdNumber.Name = "tb_IdNumber";
			this.tb_IdNumber.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.tb_IdNumber.Size = new System.Drawing.Size(80, 22);
			this.tb_IdNumber.TabIndex = 2;
			this.tb_IdNumber.Text = "55122795";
			this.tb_IdNumber.WordWrap = false;
			this.tb_IdNumber.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tb_IdNumber_KeyPress);
			this.tb_IdNumber.TextChanged += new System.EventHandler(this.tb_IdNumber_TextChanged);
			this.tb_IdNumber.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tb_IdNumber_KeyUp);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(96, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(26, 18);
			this.label1.TabIndex = 1;
			this.label1.Text = "ת\"ז:";
			// 
			// gbCompetitorDetails
			// 
			this.gbCompetitorDetails.Controls.Add(this.gbShirtNumber);
			this.gbCompetitorDetails.Controls.Add(this.tbChipNumber);
			this.gbCompetitorDetails.Controls.Add(this.label6);
			this.gbCompetitorDetails.Controls.Add(this.pnlResult);
			this.gbCompetitorDetails.Controls.Add(this.lbPlayerName);
			this.gbCompetitorDetails.Controls.Add(this.lbMeasureType);
			this.gbCompetitorDetails.Controls.Add(this.label5);
			this.gbCompetitorDetails.Controls.Add(this.lbTeamName);
			this.gbCompetitorDetails.Controls.Add(this.label4);
			this.gbCompetitorDetails.Controls.Add(this.lbHeat);
			this.gbCompetitorDetails.Controls.Add(this.label2);
			this.gbCompetitorDetails.Enabled = false;
			this.gbCompetitorDetails.Location = new System.Drawing.Point(8, 32);
			this.gbCompetitorDetails.Name = "gbCompetitorDetails";
			this.gbCompetitorDetails.Size = new System.Drawing.Size(272, 184);
			this.gbCompetitorDetails.TabIndex = 17;
			this.gbCompetitorDetails.TabStop = false;
			this.gbCompetitorDetails.Text = "פרטי מתמודד";
			// 
			// pnlResult
			// 
			this.pnlResult.Location = new System.Drawing.Point(80, 144);
			this.pnlResult.Name = "pnlResult";
			this.pnlResult.Size = new System.Drawing.Size(136, 26);
			this.pnlResult.TabIndex = 8;
			// 
			// lbPlayerName
			// 
			this.lbPlayerName.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbPlayerName.ForeColor = System.Drawing.Color.Blue;
			this.lbPlayerName.Location = new System.Drawing.Point(144, 18);
			this.lbPlayerName.Name = "lbPlayerName";
			this.lbPlayerName.Size = new System.Drawing.Size(120, 32);
			this.lbPlayerName.TabIndex = 7;
			this.lbPlayerName.Text = "אבי כהן אהרונוב";
			// 
			// lbMeasureType
			// 
			this.lbMeasureType.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbMeasureType.Location = new System.Drawing.Point(16, 152);
			this.lbMeasureType.Name = "lbMeasureType";
			this.lbMeasureType.Size = new System.Drawing.Size(64, 24);
			this.lbMeasureType.TabIndex = 6;
			this.lbMeasureType.Text = "(דקות)";
			// 
			// label5
			// 
			this.label5.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label5.Location = new System.Drawing.Point(216, 152);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(48, 24);
			this.label5.TabIndex = 4;
			this.label5.Text = "תוצאה: ";
			// 
			// lbTeamName
			// 
			this.lbTeamName.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbTeamName.ForeColor = System.Drawing.Color.Blue;
			this.lbTeamName.Location = new System.Drawing.Point(16, 120);
			this.lbTeamName.Name = "lbTeamName";
			this.lbTeamName.Size = new System.Drawing.Size(200, 24);
			this.lbTeamName.TabIndex = 3;
			this.lbTeamName.Text = "עירוני ט\' אשדוד א\'";
			// 
			// label4
			// 
			this.label4.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label4.Location = new System.Drawing.Point(216, 120);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(48, 24);
			this.label4.TabIndex = 2;
			this.label4.Text = "קבוצה: ";
			// 
			// lbHeat
			// 
			this.lbHeat.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbHeat.ForeColor = System.Drawing.Color.Blue;
			this.lbHeat.Location = new System.Drawing.Point(150, 88);
			this.lbHeat.Name = "lbHeat";
			this.lbHeat.Size = new System.Drawing.Size(48, 24);
			this.lbHeat.TabIndex = 1;
			this.lbHeat.Text = "מקצה 1";
			// 
			// label2
			// 
			this.label2.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label2.Location = new System.Drawing.Point(208, 88);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(56, 24);
			this.label2.TabIndex = 0;
			this.label2.Text = "מקצה: ";
			// 
			// btnCancel
			// 
			this.btnCancel.BackColor = System.Drawing.Color.White;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnCancel.ForeColor = System.Drawing.Color.Red;
			this.btnCancel.Image = ((System.Drawing.Image)(resources.GetObject("btnCancel.Image")));
			this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btnCancel.Location = new System.Drawing.Point(8, 296);
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
			this.btnOK.Location = new System.Drawing.Point(120, 296);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(80, 23);
			this.btnOK.TabIndex = 5;
			this.btnOK.Text = "אישור";
			// 
			// timer1
			// 
			this.timer1.Interval = 200;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// label6
			// 
			this.label6.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label6.Location = new System.Drawing.Point(208, 56);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(56, 24);
			this.label6.TabIndex = 9;
			this.label6.Text = "מס\' צ\'יפ: ";
			// 
			// tbChipNumber
			// 
			this.tbChipNumber.ForeColor = System.Drawing.Color.Blue;
			this.tbChipNumber.Location = new System.Drawing.Point(168, 56);
			this.tbChipNumber.MaxLength = 3;
			this.tbChipNumber.Name = "tbChipNumber";
			this.tbChipNumber.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.tbChipNumber.Size = new System.Drawing.Size(32, 22);
			this.tbChipNumber.TabIndex = 10;
			this.tbChipNumber.Text = "";
			this.tbChipNumber.WordWrap = false;
			this.tbChipNumber.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tb_IdNumber_KeyPress);
			this.tbChipNumber.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tbChipNumber_KeyUp);
			this.tbChipNumber.Enter += new System.EventHandler(this.tbChipNumber_Enter);
			// 
			// gbShirtNumber
			// 
			this.gbShirtNumber.Controls.Add(this.pbDigit3);
			this.gbShirtNumber.Controls.Add(this.pbDigit2);
			this.gbShirtNumber.Controls.Add(this.pbDigit1);
			this.gbShirtNumber.Location = new System.Drawing.Point(10, 8);
			this.gbShirtNumber.Name = "gbShirtNumber";
			this.gbShirtNumber.Size = new System.Drawing.Size(128, 72);
			this.gbShirtNumber.TabIndex = 11;
			this.gbShirtNumber.TabStop = false;
			this.gbShirtNumber.Text = "מספר חולצה";
			// 
			// pbDigit1
			// 
			this.pbDigit1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pbDigit1.Image = ((System.Drawing.Image)(resources.GetObject("pbDigit1.Image")));
			this.pbDigit1.Location = new System.Drawing.Point(8, 28);
			this.pbDigit1.Name = "pbDigit1";
			this.pbDigit1.Size = new System.Drawing.Size(32, 32);
			this.pbDigit1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.pbDigit1.TabIndex = 18;
			this.pbDigit1.TabStop = false;
			// 
			// pbDigit2
			// 
			this.pbDigit2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pbDigit2.Image = ((System.Drawing.Image)(resources.GetObject("pbDigit2.Image")));
			this.pbDigit2.Location = new System.Drawing.Point(48, 28);
			this.pbDigit2.Name = "pbDigit2";
			this.pbDigit2.Size = new System.Drawing.Size(32, 32);
			this.pbDigit2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.pbDigit2.TabIndex = 19;
			this.pbDigit2.TabStop = false;
			// 
			// pbDigit3
			// 
			this.pbDigit3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pbDigit3.Image = ((System.Drawing.Image)(resources.GetObject("pbDigit3.Image")));
			this.pbDigit3.Location = new System.Drawing.Point(88, 28);
			this.pbDigit3.Name = "pbDigit3";
			this.pbDigit3.Size = new System.Drawing.Size(32, 32);
			this.pbDigit3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.pbDigit3.TabIndex = 20;
			this.pbDigit3.TabStop = false;
			// 
			// img_0
			// 
			this.img_0.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.img_0.Image = ((System.Drawing.Image)(resources.GetObject("img_0.Image")));
			this.img_0.Location = new System.Drawing.Point(24, 224);
			this.img_0.Name = "img_0";
			this.img_0.Size = new System.Drawing.Size(16, 16);
			this.img_0.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.img_0.TabIndex = 19;
			this.img_0.TabStop = false;
			this.img_0.Visible = false;
			// 
			// img_1
			// 
			this.img_1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.img_1.Image = ((System.Drawing.Image)(resources.GetObject("img_1.Image")));
			this.img_1.Location = new System.Drawing.Point(48, 224);
			this.img_1.Name = "img_1";
			this.img_1.Size = new System.Drawing.Size(16, 16);
			this.img_1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.img_1.TabIndex = 20;
			this.img_1.TabStop = false;
			this.img_1.Visible = false;
			// 
			// img_2
			// 
			this.img_2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.img_2.Image = ((System.Drawing.Image)(resources.GetObject("img_2.Image")));
			this.img_2.Location = new System.Drawing.Point(80, 224);
			this.img_2.Name = "img_2";
			this.img_2.Size = new System.Drawing.Size(16, 16);
			this.img_2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.img_2.TabIndex = 21;
			this.img_2.TabStop = false;
			this.img_2.Visible = false;
			// 
			// img_3
			// 
			this.img_3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.img_3.Image = ((System.Drawing.Image)(resources.GetObject("img_3.Image")));
			this.img_3.Location = new System.Drawing.Point(104, 224);
			this.img_3.Name = "img_3";
			this.img_3.Size = new System.Drawing.Size(16, 16);
			this.img_3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.img_3.TabIndex = 22;
			this.img_3.TabStop = false;
			this.img_3.Visible = false;
			// 
			// img_4
			// 
			this.img_4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.img_4.Image = ((System.Drawing.Image)(resources.GetObject("img_4.Image")));
			this.img_4.Location = new System.Drawing.Point(128, 224);
			this.img_4.Name = "img_4";
			this.img_4.Size = new System.Drawing.Size(16, 16);
			this.img_4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.img_4.TabIndex = 23;
			this.img_4.TabStop = false;
			this.img_4.Visible = false;
			// 
			// img_5
			// 
			this.img_5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.img_5.Image = ((System.Drawing.Image)(resources.GetObject("img_5.Image")));
			this.img_5.Location = new System.Drawing.Point(152, 224);
			this.img_5.Name = "img_5";
			this.img_5.Size = new System.Drawing.Size(16, 16);
			this.img_5.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.img_5.TabIndex = 24;
			this.img_5.TabStop = false;
			this.img_5.Visible = false;
			// 
			// img_6
			// 
			this.img_6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.img_6.Image = ((System.Drawing.Image)(resources.GetObject("img_6.Image")));
			this.img_6.Location = new System.Drawing.Point(184, 224);
			this.img_6.Name = "img_6";
			this.img_6.Size = new System.Drawing.Size(16, 16);
			this.img_6.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.img_6.TabIndex = 25;
			this.img_6.TabStop = false;
			this.img_6.Visible = false;
			// 
			// img_7
			// 
			this.img_7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.img_7.Image = ((System.Drawing.Image)(resources.GetObject("img_7.Image")));
			this.img_7.Location = new System.Drawing.Point(216, 224);
			this.img_7.Name = "img_7";
			this.img_7.Size = new System.Drawing.Size(16, 16);
			this.img_7.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.img_7.TabIndex = 26;
			this.img_7.TabStop = false;
			this.img_7.Visible = false;
			// 
			// img_9
			// 
			this.img_9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.img_9.Image = ((System.Drawing.Image)(resources.GetObject("img_9.Image")));
			this.img_9.Location = new System.Drawing.Point(64, 248);
			this.img_9.Name = "img_9";
			this.img_9.Size = new System.Drawing.Size(16, 16);
			this.img_9.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.img_9.TabIndex = 28;
			this.img_9.TabStop = false;
			this.img_9.Visible = false;
			// 
			// img_8
			// 
			this.img_8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.img_8.Image = ((System.Drawing.Image)(resources.GetObject("img_8.Image")));
			this.img_8.Location = new System.Drawing.Point(32, 248);
			this.img_8.Name = "img_8";
			this.img_8.Size = new System.Drawing.Size(16, 16);
			this.img_8.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.img_8.TabIndex = 27;
			this.img_8.TabStop = false;
			this.img_8.Visible = false;
			// 
			// SetCompetitionResultsDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
			this.BackColor = System.Drawing.Color.White;
			this.ClientSize = new System.Drawing.Size(424, 323);
			this.Controls.Add(this.img_9);
			this.Controls.Add(this.img_8);
			this.Controls.Add(this.img_7);
			this.Controls.Add(this.img_6);
			this.Controls.Add(this.img_5);
			this.Controls.Add(this.img_4);
			this.Controls.Add(this.img_3);
			this.Controls.Add(this.img_2);
			this.Controls.Add(this.img_1);
			this.Controls.Add(this.img_0);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.gbCompetitorDetails);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.lbChampName);
			this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SetCompetitionResultsDialog";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "עריכת תוצאות מתמודדים";
			this.Load += new System.EventHandler(this.SetCompetitionResultsDialog_Load);
			this.Activated += new System.EventHandler(this.SetCompetitionResultsDialog_Activated);
			this.groupBox1.ResumeLayout(false);
			this.gbCompetitorDetails.ResumeLayout(false);
			this.gbShirtNumber.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void SetCompetitionResultsDialog_Load(object sender, System.EventArgs e)
		{
			lbChampName.Text = _champName+", "+_competition.SportField.Name;
			
			//digit images:
			_digits = new Image[10];
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(SetCompetitionResultsDialog));
			for (int j=0; j<10; j++)
			{
				_digits[j] = ((System.Drawing.Image)(resources.GetObject("img_"+j+".Image")));
			}
			
			//result panel:
			if (_competition.Competitors.Length > 0)
			{
				string resultFormat=_competition.Competitors[0].ResultFormat;
				if (resultFormat.Length > 0)
					BuildResultPanel(resultFormat);
			}
			
			//fill all competitors:
			FillCompetitors();
			
			//auto select:
			if (lbCompetitors.Items.Count > 0)
				lbCompetitors.SelectedIndex = 0;

			if (defaultCompetitor != null)
			{
				for (int i=0; i<lbCompetitors.Items.Count; i++)
				{
					if ((lbCompetitors.Items[i] as ListItem).Value == 
						defaultCompetitor)
					{
						lbCompetitors.SelectedIndex = i;
						break;
					}
				}
			}
			
			timer1.Enabled = true;
		}

		private void FillCompetitors()
		{
			tb_IdNumber.Text = "";
			lbCompetitors.Items.Clear();
			foreach (CompetitorData competitor in _competition.Competitors)
			{
				string strFirstName=Tools.CStrDef(
					competitor.Player.Student.FirstName, "");
				string strLastName=Tools.CStrDef(
					competitor.Player.Student.LastName, "");
				string strName=strFirstName;
				if (strLastName.Length > 0)
					strName += " "+strLastName;
				lbCompetitors.Items.Add(new ListItem(
					strName, competitor));
			}

			if (lbCompetitors.Items.Count > 0)
				lbCompetitors.SelectedIndex = 0;
		}

		private void FillCompetitorDetails(CompetitorData competitor)
		{
			//first clear values:
			lbHeat.Text = "";
			lbTeamName.Text = "";
			lbPlayerName.Text = "";
			gbCompetitorDetails.Enabled = false;
			gbShirtNumber.Visible = false;

			//abort if no value given:
			if (competitor == null)
				return;
			
			//fill default data:
			_changedByCode = true;
			tb_IdNumber.Text = competitor.Player.Student.IdNumber;
			int heatIndex=(int) Tools.IIF((competitor.Heat != null)&&
				(competitor.Heat.HeatIndex >= 0), competitor.Heat.HeatIndex, -1);
			lbHeat.Text = Tools.IIF((heatIndex >= 0), 
				"מקצה "+(heatIndex+1).ToString(), "---").ToString();
			string teamName=competitor.Player.Team.School.Name;
			if (competitor.Player.Team.TeamIndex > 0)
				teamName += " "+Tools.ToHebLetter(competitor.Player.Team.TeamIndex);
			lbTeamName.Text = teamName;
			string strResult=Tools.CStrDef(competitor.ParseResult(), "");
			string measureType=competitor.MeasureType;
			if (measureType.Length > 0)
				strResult = strResult.Replace(" "+measureType, "");
			//tbResult.Text = Tools.CStrDef(strResult, "");
			lbMeasureType.Text = Tools.IIF((measureType.Length > 0), 
				"("+measureType+")", "").ToString();
			lbPlayerName.Text = (lbCompetitors.SelectedItem as ListItem).Text;
			tbChipNumber.Text = Tools.IIF((competitor.Player.ChipNumber > 0), 
				competitor.Player.ChipNumber.ToString(), "").ToString();

			//clear previous result and fill new result:
			int curIndex=0;
			foreach (TextBox tb in _resultControls)
			{
				string strCurrentChar="";
				if (curIndex < strResult.Length)
				{
					while ((curIndex < strResult.Length)&&
						(!Tools.IsDigit(strResult[curIndex])))
					{
						curIndex++;
					}
					if (curIndex < strResult.Length)
					{
						strCurrentChar = strResult[curIndex].ToString();
						curIndex += 1;
					}
				}
				tb.Text = strCurrentChar;
			}

			//shirt number:
			if (competitor.Player.ShirtNumber > 0)
			{
				string strShirtNumber=competitor.Player.ShirtNumber.ToString();
				int i;
				
				//fill zero:
				for (i=1; i<=3-strShirtNumber.Length; i++)
				{
					PictureBox objImage=(PictureBox)
						Tools.FindControl(gbShirtNumber, "pbDigit"+i);
					objImage.Image = _digits[0];
				}
				
				//fill digits:
				for (i=3-strShirtNumber.Length+1; i<=3; i++)
				{
					int charIndex=i-(3-strShirtNumber.Length+1);
					PictureBox objImage=(PictureBox)
						Tools.FindControl(gbShirtNumber, "pbDigit"+i);
					objImage.Image = _digits[Int32.Parse(strShirtNumber[charIndex].ToString())];
				}
				gbShirtNumber.Visible = true;
			}
			
			gbCompetitorDetails.Enabled = true;

			//set focus:
			if (_resultControls.Count > 0)
				(_resultControls[0] as TextBox).Focus();
		}

		/// <summary>
		/// for each digit character (e.g. '#') build text box. for each delimeter, 
		/// e.g. ':' or '.' build label.
		/// give proper names.
		/// </summary>
		private void BuildResultPanel(string resultFormat)
		{
			int curLeft=2;
			int top=1;
			int textBoxWidth=15;
			int labelWidth=5;
			int height=24;
			int diff=3;
			int i;
			ArrayList delimeters=new ArrayList(Sport.Rulesets.ResultFormat.Delimeters);
			
			_resultControls = new ArrayList();
			pnlResult.Controls.Clear();
			
			for (i=0; i<resultFormat.Length; i++)
			{
				char curChar=resultFormat[i];
				if (curChar == Sport.Rulesets.ResultFormat.DigitCharacter)
				{
					//build text box...
					TextBox tb=new TextBox();
					tb.Name = "result_"+i;
					tb.Top = top;
					tb.Left = curLeft;
					tb.Width = textBoxWidth;
					tb.Height = height;
					tb.MaxLength = 1;
					tb.ForeColor = Color.Blue;
					tb.Enter += new EventHandler(ResultControlEnter);
					tb.Click += new EventHandler(ResultControlEnter);
					tb.KeyPress += new KeyPressEventHandler(ResultControlKeyPress);
					tb.KeyUp += new KeyEventHandler(ResultControlKeyUp);
					tb.MouseWheel += new MouseEventHandler(ResultControlMouseWheel);
					pnlResult.Controls.Add(tb);
					_resultControls.Add(tb);

					curLeft += (textBoxWidth+diff);
				} //end if digit
				else
				{
					if (delimeters.IndexOf(curChar) >= 0)
					{
						//build label...
						Label objLabel=new Label();
						objLabel.Name = "lbResult_"+i;
						objLabel.Top = top;
						objLabel.AutoSize = false;
						objLabel.Left = curLeft;
						objLabel.Width = labelWidth;
						objLabel.Height = height;
						objLabel.TextAlign = ContentAlignment.MiddleCenter;
						objLabel.Text = curChar.ToString();
						pnlResult.Controls.Add(objLabel);
						
						curLeft += (labelWidth+diff);
					} //end if delimeter
				} //end if not digit character
			} //end loop over format characters
		}

		private void MoveCompetitor(int howMuch)
		{
			int selIndex=lbCompetitors.SelectedIndex;
			int newIndex=(selIndex+howMuch);
			if (newIndex >= lbCompetitors.Items.Count)
				newIndex = 0;
			if (newIndex < 0)
				newIndex = lbCompetitors.Items.Count-1;
			lbCompetitors.SelectedIndex = newIndex;
		}

		private void StoreCurrentResult()
		{
			string strResult="";
			if (lbCompetitors.SelectedIndex < 0)
				return;
			foreach (Control control in pnlResult.Controls)
			{
				if (control is TextBox)
				{
					string strText=Tools.CStrDef(control.Text, "");
					strResult += (strText.Length == 0)?"0":strText;
				}
				if (control is Label)
					strResult += Sport.Core.Data.ResultSeperator;
			}
			CompetitorData competitor=(CompetitorData) 
				(lbCompetitors.SelectedItem as ListItem).Value;
			competitor.Result = strResult;
		}

		private void lbCompetitors_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			int selIndex=lbCompetitors.SelectedIndex;
			if (selIndex < 0)
				return;
			
			CompetitorData competitor=(CompetitorData) 
				(lbCompetitors.SelectedItem as ListItem).Value;
			FillCompetitorDetails(competitor);
		}

		private void SetCompetitionResultsDialog_Activated(object sender, System.EventArgs e)
		{
			//Sport.UI.MessageBox.Show("activate");
		}

		private void timer1_Tick(object sender, System.EventArgs e)
		{
			timer1.Enabled = false;
			
			//set focus:
			if (_resultControls.Count > 0)
				(_resultControls[0] as TextBox).Focus();
		}

		private void ResultControlEnter(object sender, EventArgs e)
		{
			if (sender is TextBox)
				(sender as TextBox).SelectAll();
		}

		private void ResultControlKeyPress(object sender, KeyPressEventArgs e)
		{
			int iChar=System.Convert.ToInt32(e.KeyChar);
			if (iChar == 8)
			{
				e.Handled = false;
				return;
			}
			e.Handled = !Tools.IsDigit(e.KeyChar);
		}

		private void ResultControlKeyUp(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Tab:
					return;
				case Keys.Right:
				case Keys.Down:
					MoveCompetitor(1);
					return;
				case Keys.Left:
				case Keys.Up:
					MoveCompetitor(-1);
					return;
				case Keys.End:
					MoveCompetitor((-1)*lbCompetitors.Items.Count);
					return;
				case Keys.Home:
					MoveCompetitor(lbCompetitors.Items.Count);
					return;
			}
			
			if (!Tools.IsDigit((char) e.KeyValue))
				return;
			
			//store current result:
			StoreCurrentResult();
			
			if (sender is TextBox)
			{
				string strText=(sender as TextBox).Text;
				if (strText.Length == 1)
				{
					//look for the sender in the controls array:
					for (int i=0; i<_resultControls.Count; i++)
					{
						if (_resultControls[i].Equals(sender))
						{
							if (i < (_resultControls.Count-1))
							{
								(_resultControls[i+1] as TextBox).Focus();
							}
							else
							{
								MoveCompetitor(1);
							}
							break;
						} //end if control is equal to sender
					} //end loop over the result controls
				} //end if we have text in textbox
			} //end if the sender is textbox
		}
		
		private void ResultControlMouseWheel(object sender, MouseEventArgs e)
		{
			MoveCompetitor((e.Delta > 0)?-1:1);
		}

		private void tb_IdNumber_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			int iChar=System.Convert.ToInt32(e.KeyChar);
			if (iChar == 8)
			{
				e.Handled = false;
				return;
			}
			e.Handled = !Tools.IsDigit(e.KeyChar);
		}

		private void tb_IdNumber_TextChanged(object sender, System.EventArgs e)
		{
			if (_changedByCode)
			{
				_changedByCode = false;
				return;
			}

			string strIdNumber=Tools.CStrDef(tb_IdNumber.Text, "");
			if (strIdNumber.Length >= 5)
			{
				for (int i=0; i<lbCompetitors.Items.Count; i++)
				{
					CompetitorData competitor=(CompetitorData)
						(lbCompetitors.Items[i] as ListItem).Value;
					if (competitor.Player.Student.IdNumber == strIdNumber)
					{
						lbCompetitors.SelectedIndex = i;
						break;
					}
				}
			}
		}

		private void tb_IdNumber_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			//e.Handled = true;
		}

		private void tbChipNumber_Enter(object sender, System.EventArgs e)
		{
			tbChipNumber.SelectAll();
		}

		private void tbChipNumber_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (!Tools.IsDigit((char) e.KeyValue))
				return;		
			if (lbCompetitors.SelectedIndex < 0)
				return;
			CompetitorData competitor=(CompetitorData)
				(lbCompetitors.SelectedItem as ListItem).Value;
			competitor.Player.ChipNumber = Tools.CIntDef(tbChipNumber.Text, 0);
		}
	}
}
