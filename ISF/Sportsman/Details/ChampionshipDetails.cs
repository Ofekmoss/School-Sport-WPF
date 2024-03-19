using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Sport.Data;
using Sport.Common;

namespace Sportsman.Details
{
	/// <summary>
	/// Summary description for ChampionshipDetails.
	/// </summary>
	public class ChampionshipDetails : System.Windows.Forms.Form
	{
		private int _championshipID=-1;
		
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox edChampionshipNumber;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel2;
		private Sport.UI.Controls.ThemeButton btnTableView;
		private Sport.UI.Controls.ThemeButton btnClose;
		private System.Windows.Forms.TabControl tcDetails;
		private System.Windows.Forms.TabPage tpGeneral;
		private System.Windows.Forms.TabPage tpTeams;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label lbChampionshipsCount;
		private System.Windows.Forms.ListBox lstTeams;
		private Sport.UI.Controls.ThemeButton btnTeamsView;
		private System.Windows.Forms.Label lbName;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label lbSport;
		private System.Windows.Forms.Label lbStatus;
		private System.Windows.Forms.TabPage tpMore;
		private System.Windows.Forms.Label lbRegion;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label lbEndRegDate;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label lbStartDate;
		private System.Windows.Forms.Label lbEndDate;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.Label lbFinalDate;
		private System.Windows.Forms.Label lbAltEndDate;
		private System.Windows.Forms.Label lbAltStartDate;
		private System.Windows.Forms.Label lbAltFinalDate;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ChampionshipDetails()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			/*
			foreach (string caption in captions)
			{
				TabPage page=new TabPage(caption);
				tcDetails.TabPages.Add(page);
			}
			*/

			if (Sport.UI.ViewManager.SelectedView is Sport.UI.TableView)
			{
				Sport.UI.TableView tableView=(Sport.UI.TableView) Sport.UI.ViewManager.SelectedView;
				if (tableView.EntityListView.EntityName == Sport.Entities.Championship.TypeName)
				{
					if (tableView.Current != null)
						this.EntityID = tableView.Current.Id;
				}
			}

			this.Text = "פרטי האליפות";
		}
		
		public int EntityID
		{
			get { return _championshipID; }
			set
			{
				try
				{
					Sport.Entities.Championship championship=new Sport.Entities.Championship(value);
					edChampionshipNumber.Text = championship.Number.ToString();
					//edChampionshipNumber.Text = value.ToString();
					_championshipID = value;
					RefreshDetails();
				}
				catch (Exception e)
				{
					System.Diagnostics.Debug.WriteLine("failed to create championship details. ID: "+value.ToString());
					System.Diagnostics.Debug.WriteLine("error: "+e.Message);
					_championshipID = -1;
				}
			}
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
			this.edChampionshipNumber = new System.Windows.Forms.TextBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel2 = new System.Windows.Forms.Panel();
			this.btnClose = new Sport.UI.Controls.ThemeButton();
			this.btnTableView = new Sport.UI.Controls.ThemeButton();
			this.tcDetails = new System.Windows.Forms.TabControl();
			this.tpGeneral = new System.Windows.Forms.TabPage();
			this.lbSport = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.lbStatus = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.lbName = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.tpMore = new System.Windows.Forms.TabPage();
			this.tpTeams = new System.Windows.Forms.TabPage();
			this.btnTeamsView = new Sport.UI.Controls.ThemeButton();
			this.lstTeams = new System.Windows.Forms.ListBox();
			this.lbChampionshipsCount = new System.Windows.Forms.Label();
			this.lbRegion = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.lbFinalDate = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.lbEndRegDate = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.lbStartDate = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.lbEndDate = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.lbAltEndDate = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.lbAltStartDate = new System.Windows.Forms.Label();
			this.label13 = new System.Windows.Forms.Label();
			this.lbAltFinalDate = new System.Windows.Forms.Label();
			this.label15 = new System.Windows.Forms.Label();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.tcDetails.SuspendLayout();
			this.tpGeneral.SuspendLayout();
			this.tpMore.SuspendLayout();
			this.tpTeams.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Dock = System.Windows.Forms.DockStyle.Right;
			this.label1.Font = new System.Drawing.Font("Arial", 10F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label1.Location = new System.Drawing.Point(331, 0);
			this.label1.Name = "label1";
			this.label1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.label1.Size = new System.Drawing.Size(85, 19);
			this.label1.TabIndex = 0;
			this.label1.Text = "מספר אליפות:";
			// 
			// edChampionshipNumber
			// 
			this.edChampionshipNumber.Enabled = true;
			this.edChampionshipNumber.Location = new System.Drawing.Point(264, 0);
			this.edChampionshipNumber.MaxLength = 8;
			this.edChampionshipNumber.Name = "edChampionshipNumber";
			this.edChampionshipNumber.Size = new System.Drawing.Size(64, 20);
			this.edChampionshipNumber.TabIndex = 0;
			this.edChampionshipNumber.Text = "";
			this.edChampionshipNumber.TextChanged +=new EventHandler(edChampionshipNumber_TextChanged);
			this.edChampionshipNumber.KeyDown += new System.Windows.Forms.KeyEventHandler(this.edChampionshipNumber_KeyDown);
			this.edChampionshipNumber.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.edChampionshipNumber_KeyPress);
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.edChampionshipNumber);
			this.panel1.Controls.Add(this.label1);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(416, 32);
			this.panel1.TabIndex = 2;
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.btnClose);
			this.panel2.Controls.Add(this.btnTableView);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel2.Location = new System.Drawing.Point(0, 197);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(416, 32);
			this.panel2.TabIndex = 3;
			// 
			// btnClose
			// 
			this.btnClose.Alignment = System.Drawing.StringAlignment.Center;
			this.btnClose.AutoSize = false;
			this.btnClose.Dock = System.Windows.Forms.DockStyle.Left;
			this.btnClose.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnClose.Hue = 220F;
			this.btnClose.Image = null;
			this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btnClose.ImageList = null;
			this.btnClose.ImageSize = new System.Drawing.Size(0, 0);
			this.btnClose.Location = new System.Drawing.Point(0, 0);
			this.btnClose.Name = "btnClose";
			this.btnClose.Saturation = 0.9F;
			this.btnClose.Size = new System.Drawing.Size(75, 32);
			this.btnClose.TabIndex = 3;
			this.btnClose.Text = "סגור מסך פרטים";
			this.btnClose.Transparent = System.Drawing.Color.Black;
			this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
			// 
			// btnTableView
			// 
			this.btnTableView.Alignment = System.Drawing.StringAlignment.Center;
			this.btnTableView.AutoSize = false;
			this.btnTableView.Dock = System.Windows.Forms.DockStyle.Right;
			this.btnTableView.Enabled = false;
			this.btnTableView.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnTableView.Hue = 220F;
			this.btnTableView.Image = null;
			this.btnTableView.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btnTableView.ImageList = null;
			this.btnTableView.ImageSize = new System.Drawing.Size(0, 0);
			this.btnTableView.Location = new System.Drawing.Point(341, 0);
			this.btnTableView.Name = "btnTableView";
			this.btnTableView.Saturation = 0.9F;
			this.btnTableView.Size = new System.Drawing.Size(75, 32);
			this.btnTableView.TabIndex = 2;
			this.btnTableView.Text = "עבור למסך בתי ספר";
			this.btnTableView.Transparent = System.Drawing.Color.Black;
			this.btnTableView.Click += new System.EventHandler(this.btnTableView_Click);
			// 
			// tcDetails
			// 
			this.tcDetails.Controls.Add(this.tpGeneral);
			this.tcDetails.Controls.Add(this.tpMore);
			this.tcDetails.Controls.Add(this.tpTeams);
			this.tcDetails.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tcDetails.Location = new System.Drawing.Point(0, 32);
			this.tcDetails.Name = "tcDetails";
			this.tcDetails.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.tcDetails.SelectedIndex = 0;
			this.tcDetails.Size = new System.Drawing.Size(416, 165);
			this.tcDetails.TabIndex = 4;
			// 
			// tpGeneral
			// 
			this.tpGeneral.Controls.Add(this.lbRegion);
			this.tpGeneral.Controls.Add(this.label11);
			this.tpGeneral.Controls.Add(this.lbSport);
			this.tpGeneral.Controls.Add(this.label9);
			this.tpGeneral.Controls.Add(this.lbStatus);
			this.tpGeneral.Controls.Add(this.label7);
			this.tpGeneral.Controls.Add(this.lbName);
			this.tpGeneral.Controls.Add(this.label2);
			this.tpGeneral.Location = new System.Drawing.Point(4, 22);
			this.tpGeneral.Name = "tpGeneral";
			this.tpGeneral.Size = new System.Drawing.Size(408, 139);
			this.tpGeneral.TabIndex = 0;
			this.tpGeneral.Text = "כללי";
			// 
			// lbSport
			// 
			this.lbSport.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbSport.ForeColor = System.Drawing.Color.Blue;
			this.lbSport.Location = new System.Drawing.Point(32, 48);
			this.lbSport.Name = "lbSport";
			this.lbSport.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.lbSport.Size = new System.Drawing.Size(248, 19);
			this.lbSport.TabIndex = 12;
			this.lbSport.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Font = new System.Drawing.Font("Arial", 10F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label9.Location = new System.Drawing.Point(368, 48);
			this.label9.Name = "label9";
			this.label9.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.label9.Size = new System.Drawing.Size(30, 19);
			this.label9.TabIndex = 11;
			this.label9.Text = "ענף:";
			// 
			// lbStatus
			// 
			this.lbStatus.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbStatus.ForeColor = System.Drawing.Color.Blue;
			this.lbStatus.Location = new System.Drawing.Point(32, 112);
			this.lbStatus.Name = "lbStatus";
			this.lbStatus.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.lbStatus.Size = new System.Drawing.Size(248, 19);
			this.lbStatus.TabIndex = 10;
			this.lbStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Font = new System.Drawing.Font("Arial", 10F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label7.Location = new System.Drawing.Point(352, 112);
			this.label7.Name = "label7";
			this.label7.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.label7.Size = new System.Drawing.Size(47, 19);
			this.label7.TabIndex = 9;
			this.label7.Text = "סטטוס:";
			// 
			// lbName
			// 
			this.lbName.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbName.ForeColor = System.Drawing.Color.Blue;
			this.lbName.Location = new System.Drawing.Point(32, 16);
			this.lbName.Name = "lbName";
			this.lbName.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.lbName.Size = new System.Drawing.Size(248, 19);
			this.lbName.TabIndex = 2;
			this.lbName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Arial", 10F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label2.Location = new System.Drawing.Point(320, 16);
			this.label2.Name = "label2";
			this.label2.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.label2.Size = new System.Drawing.Size(80, 19);
			this.label2.TabIndex = 1;
			this.label2.Text = "שם האליפות:";
			// 
			// tpMore
			// 
			this.tpMore.Controls.Add(this.lbAltFinalDate);
			this.tpMore.Controls.Add(this.label15);
			this.tpMore.Controls.Add(this.lbAltEndDate);
			this.tpMore.Controls.Add(this.label10);
			this.tpMore.Controls.Add(this.lbAltStartDate);
			this.tpMore.Controls.Add(this.label13);
			this.tpMore.Controls.Add(this.lbEndDate);
			this.tpMore.Controls.Add(this.label8);
			this.tpMore.Controls.Add(this.lbFinalDate);
			this.tpMore.Controls.Add(this.label3);
			this.tpMore.Controls.Add(this.lbEndRegDate);
			this.tpMore.Controls.Add(this.label5);
			this.tpMore.Controls.Add(this.lbStartDate);
			this.tpMore.Controls.Add(this.label4);
			this.tpMore.Location = new System.Drawing.Point(4, 22);
			this.tpMore.Name = "tpMore";
			this.tpMore.Size = new System.Drawing.Size(408, 139);
			this.tpMore.TabIndex = 1;
			this.tpMore.Text = "לוח זמנים";
			// 
			// tpTeams
			// 
			this.tpTeams.Controls.Add(this.btnTeamsView);
			this.tpTeams.Controls.Add(this.lstTeams);
			this.tpTeams.Location = new System.Drawing.Point(4, 22);
			this.tpTeams.Name = "tpTeams";
			this.tpTeams.Size = new System.Drawing.Size(408, 139);
			this.tpTeams.TabIndex = 2;
			this.tpTeams.Text = "קבוצות";
			// 
			// btnTeamsView
			// 
			this.btnTeamsView.Alignment = System.Drawing.StringAlignment.Center;
			this.btnTeamsView.AutoSize = false;
			this.btnTeamsView.Enabled = false;
			this.btnTeamsView.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnTeamsView.Hue = 160F;
			this.btnTeamsView.Image = null;
			this.btnTeamsView.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btnTeamsView.ImageList = null;
			this.btnTeamsView.ImageSize = new System.Drawing.Size(0, 0);
			this.btnTeamsView.Location = new System.Drawing.Point(320, 48);
			this.btnTeamsView.Name = "btnTeamsView";
			this.btnTeamsView.Saturation = 0.6F;
			this.btnTeamsView.Size = new System.Drawing.Size(75, 32);
			this.btnTeamsView.TabIndex = 3;
			this.btnTeamsView.Text = "עבור למסך קבוצה";
			this.btnTeamsView.Transparent = System.Drawing.Color.Black;
			this.btnTeamsView.Click += new System.EventHandler(this.btnTeamsView_Click);
			// 
			// lstTeams
			// 
			this.lstTeams.Location = new System.Drawing.Point(16, 16);
			this.lstTeams.Name = "lstTeams";
			this.lstTeams.Size = new System.Drawing.Size(288, 95);
			this.lstTeams.TabIndex = 0;
			this.lstTeams.SelectedIndexChanged += new System.EventHandler(this.lstTeams_SelectedIndexChanged);
			// 
			// lbChampionshipsCount
			// 
			this.lbChampionshipsCount.Location = new System.Drawing.Point(0, 0);
			this.lbChampionshipsCount.Name = "lbChampionshipsCount";
			this.lbChampionshipsCount.TabIndex = 0;
			// 
			// lbRegion
			// 
			this.lbRegion.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbRegion.ForeColor = System.Drawing.Color.Blue;
			this.lbRegion.Location = new System.Drawing.Point(32, 80);
			this.lbRegion.Name = "lbRegion";
			this.lbRegion.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.lbRegion.Size = new System.Drawing.Size(248, 19);
			this.lbRegion.TabIndex = 14;
			this.lbRegion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Font = new System.Drawing.Font("Arial", 10F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label11.Location = new System.Drawing.Point(360, 80);
			this.label11.Name = "label11";
			this.label11.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.label11.Size = new System.Drawing.Size(36, 19);
			this.label11.TabIndex = 13;
			this.label11.Text = "מחוז:";
			// 
			// lbFinalDate
			// 
			this.lbFinalDate.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbFinalDate.ForeColor = System.Drawing.Color.Blue;
			this.lbFinalDate.Location = new System.Drawing.Point(192, 104);
			this.lbFinalDate.Name = "lbFinalDate";
			this.lbFinalDate.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.lbFinalDate.Size = new System.Drawing.Size(88, 19);
			this.lbFinalDate.TabIndex = 14;
			this.lbFinalDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Arial", 10F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label3.Location = new System.Drawing.Point(336, 104);
			this.label3.Name = "label3";
			this.label3.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.label3.Size = new System.Drawing.Size(63, 19);
			this.label3.TabIndex = 13;
			this.label3.Text = "מועד גמר:";
			// 
			// lbEndRegDate
			// 
			this.lbEndRegDate.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbEndRegDate.ForeColor = System.Drawing.Color.Blue;
			this.lbEndRegDate.Location = new System.Drawing.Point(192, 8);
			this.lbEndRegDate.Name = "lbEndRegDate";
			this.lbEndRegDate.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.lbEndRegDate.Size = new System.Drawing.Size(88, 19);
			this.lbEndRegDate.TabIndex = 12;
			this.lbEndRegDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Font = new System.Drawing.Font("Arial", 10F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label5.Location = new System.Drawing.Point(288, 8);
			this.label5.Name = "label5";
			this.label5.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.label5.Size = new System.Drawing.Size(111, 19);
			this.label5.TabIndex = 11;
			this.label5.Text = "מועד סיום הרשמה:";
			// 
			// lbStartDate
			// 
			this.lbStartDate.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbStartDate.ForeColor = System.Drawing.Color.Blue;
			this.lbStartDate.Location = new System.Drawing.Point(192, 40);
			this.lbStartDate.Name = "lbStartDate";
			this.lbStartDate.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.lbStartDate.Size = new System.Drawing.Size(88, 19);
			this.lbStartDate.TabIndex = 10;
			this.lbStartDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("Arial", 10F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label4.Location = new System.Drawing.Point(312, 40);
			this.label4.Name = "label4";
			this.label4.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.label4.Size = new System.Drawing.Size(82, 19);
			this.label4.TabIndex = 9;
			this.label4.Text = "מועד התחלה:";
			// 
			// lbEndDate
			// 
			this.lbEndDate.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbEndDate.ForeColor = System.Drawing.Color.Blue;
			this.lbEndDate.Location = new System.Drawing.Point(16, 40);
			this.lbEndDate.Name = "lbEndDate";
			this.lbEndDate.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.lbEndDate.Size = new System.Drawing.Size(88, 19);
			this.lbEndDate.TabIndex = 16;
			this.lbEndDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Font = new System.Drawing.Font("Arial", 10F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label8.Location = new System.Drawing.Point(112, 40);
			this.label8.Name = "label8";
			this.label8.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.label8.Size = new System.Drawing.Size(65, 19);
			this.label8.TabIndex = 15;
			this.label8.Text = "מועד סיום:";
			// 
			// lbAltEndDate
			// 
			this.lbAltEndDate.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbAltEndDate.ForeColor = System.Drawing.Color.Blue;
			this.lbAltEndDate.Location = new System.Drawing.Point(16, 72);
			this.lbAltEndDate.Name = "lbAltEndDate";
			this.lbAltEndDate.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.lbAltEndDate.Size = new System.Drawing.Size(88, 19);
			this.lbAltEndDate.TabIndex = 20;
			this.lbAltEndDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Font = new System.Drawing.Font("Arial", 10F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label10.Location = new System.Drawing.Point(112, 72);
			this.label10.Name = "label10";
			this.label10.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.label10.Size = new System.Drawing.Size(68, 19);
			this.label10.TabIndex = 19;
			this.label10.Text = "סיום חלופי:";
			// 
			// lbAltStartDate
			// 
			this.lbAltStartDate.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbAltStartDate.ForeColor = System.Drawing.Color.Blue;
			this.lbAltStartDate.Location = new System.Drawing.Point(192, 72);
			this.lbAltStartDate.Name = "lbAltStartDate";
			this.lbAltStartDate.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.lbAltStartDate.Size = new System.Drawing.Size(88, 19);
			this.lbAltStartDate.TabIndex = 18;
			this.lbAltStartDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label13
			// 
			this.label13.AutoSize = true;
			this.label13.Font = new System.Drawing.Font("Arial", 10F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label13.Location = new System.Drawing.Point(280, 72);
			this.label13.Name = "label13";
			this.label13.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.label13.Size = new System.Drawing.Size(117, 19);
			this.label13.TabIndex = 17;
			this.label13.Text = "מועד התחלה חלופי:";
			// 
			// lbAltFinalDate
			// 
			this.lbAltFinalDate.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbAltFinalDate.ForeColor = System.Drawing.Color.Blue;
			this.lbAltFinalDate.Location = new System.Drawing.Point(16, 104);
			this.lbAltFinalDate.Name = "lbAltFinalDate";
			this.lbAltFinalDate.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.lbAltFinalDate.Size = new System.Drawing.Size(88, 19);
			this.lbAltFinalDate.TabIndex = 22;
			this.lbAltFinalDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label15
			// 
			this.label15.AutoSize = true;
			this.label15.Font = new System.Drawing.Font("Arial", 10F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label15.Location = new System.Drawing.Point(112, 104);
			this.label15.Name = "label15";
			this.label15.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.label15.Size = new System.Drawing.Size(66, 19);
			this.label15.TabIndex = 21;
			this.label15.Text = "גמר חלופי:";
			// 
			// ChampionshipDetails
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(416, 229);
			this.Controls.Add(this.tcDetails);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.panel1);
			this.Name = "ChampionshipDetails";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "ChampionshipDetails";
			this.panel1.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.tcDetails.ResumeLayout(false);
			this.tpGeneral.ResumeLayout(false);
			this.tpMore.ResumeLayout(false);
			this.tpTeams.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void btnClose_Click(object sender, System.EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
		}

		private void edChampionshipNumber_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			//e.Handled = true;
		}

		private void edChampionshipNumber_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{

		}
		
		private string getDateString(System.DateTime date)
		{
			if (date.Equals(DateTime.MinValue))
				return "אין";
			else
				return date.ToShortDateString();
		}

		private void RefreshDetails()
		{
			//decide what to do based on the current id
			btnTableView.Enabled = (_championshipID >= 0);
			btnTeamsView.Enabled = false;
			Sport.Entities.Championship championship=null;
			if (_championshipID >= 0)
			{
				Sport.UI.Dialogs.WaitForm.ShowWait("טוען נתונים אנא המתן...");
				championship=new Sport.Entities.Championship(_championshipID);
				lbName.Text = championship.Name;
				lbSport.Text = championship.Sport.Name;
				lbRegion.Text = championship.Region.Name;
				lbEndRegDate.Text = getDateString(championship.LastRegistrationDate);
				lbStartDate.Text = getDateString(championship.StartDate);
				lbEndDate.Text = getDateString(championship.EndDate);
				lbAltStartDate.Text = getDateString(championship.AltStartDate);
				lbAltEndDate.Text = getDateString(championship.AltEndDate);
				lbFinalDate.Text = getDateString(championship.FinalsDate);
				lbAltFinalDate.Text = getDateString(championship.AltFinalsDate);
				FillTeams(championship);
				if (championship.Status == Sport.Types.ChampionshipType.Confirmed)
					lbStatus.ForeColor = System.Drawing.Color.Green;
				else
					lbStatus.ForeColor = System.Drawing.Color.Red;
				lbStatus.Text = (new Sport.Types.ChampionshipStatusLookup()).Lookup((int)(championship.Status));
				Sport.UI.Dialogs.WaitForm.HideWait();
			}
			else
			{
				lbName.Text = "";
				lbSport.Text = "";
				lbRegion.Text = "";
				lbEndRegDate.Text = "";
				lbStartDate.Text = "";
				lbEndDate.Text = "";
				lbAltStartDate.Text = "";
				lbAltEndDate.Text = "";
				lbFinalDate.Text = "";
				lbAltFinalDate.Text = "";
								
				lstTeams.Items.Clear();
				
			}
			this.Text = "פרטי האליפות";
			if (_championshipID >= 0)
				this.Text += " - "+championship.Name;
		}
		
		private void FillTeams(Sport.Entities.Championship championship)
		{
			lstTeams.Items.Clear();
			EntityFilter filter=new EntityFilter((int) Sport.Entities.Team.Fields.Championship, championship.Id);
			Entity[] teams=Sport.Entities.Team.Type.GetEntities(filter);
			foreach (Entity ent in teams)
			{
				Sport.Entities.Team team=new Sport.Entities.Team(ent);
				lstTeams.Items.Add(new Sport.Common.ListItem(team.Name, team.Id));
			}
		}
		
		private void edChampionshipNumber_TextChanged(object sender, System.EventArgs e)
		{
			int championshipNumber=Sport.Common.Tools.CIntDef(edChampionshipNumber.Text, -1);
			if (championshipNumber < 0)
			{
				_championshipID = -1;
			}
			else
			{
				EntityFilter filter=new EntityFilter((int) Sport.Entities.Championship.Fields.Number, championshipNumber);
				filter.Add(Sport.Entities.Championship.CurrentSeasonFilter());
				Sport.Data.Entity[] championships=Sport.Entities.Championship.Type.GetEntities(filter);
				if (championships.Length > 0)
				{
					_championshipID = championships[0].Id;	
				}
				else
				{
					_championshipID = -1;
				}
			}
			
			RefreshDetails();
		}
		
		private void lstTeams_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			btnTeamsView.Enabled = (lstTeams.SelectedIndex >= 0);
		}

		private void btnTeamsView_Click(object sender, System.EventArgs e)
		{
			int selIndex=lstTeams.SelectedIndex;
			if (selIndex < 0)
				return;
			ListItem selectedItem=(ListItem) lstTeams.Items[selIndex];
			int teamID=(int) selectedItem.Value;
			Sport.UI.ViewManager.OpenView(typeof(Views.TeamsTableView), "team="+teamID.ToString());
			this.DialogResult = DialogResult.OK;
		}

		private void btnTableView_Click(object sender, System.EventArgs e)
		{
			if (_championshipID >= 0)
			{
				Sport.UI.ViewManager.OpenView(typeof(Views.ChampionshipsTableView), "championship="+_championshipID.ToString());
				this.DialogResult = DialogResult.OK;
			}
		}
	} //end class ChampionshipDetails
}
