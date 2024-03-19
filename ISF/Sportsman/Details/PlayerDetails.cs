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
	/// Summary description for PlayerDetails.
	/// </summary>
	public class PlayerDetails : System.Windows.Forms.Form
	{
		private int _playerID=-1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel2;
		private Sport.UI.Controls.ThemeButton btnTableView;
		private Sport.UI.Controls.ThemeButton btnClose;
		private System.Windows.Forms.TabControl tcDetails;
		private System.Windows.Forms.TabPage tpGeneral;
		private System.Windows.Forms.TabPage tpSchool;
		private System.Windows.Forms.TabPage tpTeams;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label lbFirstName;
		private System.Windows.Forms.Label lbLastName;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label lbBirthDate;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label lbSchoolSymbol;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label lbSchoolName;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label lbPlayersCount;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label teamNameLabel;
		private System.Windows.Forms.Label championshipNameLabel;
		private System.Windows.Forms.Label lbTeamName;
		private System.Windows.Forms.Label lbChampionshipName;
		private System.Windows.Forms.ListBox lstPlayers;
		private Sport.UI.Controls.ThemeButton btnChampionshipView;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public PlayerDetails()
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
				if (tableView.EntityListView.EntityName == Sport.Entities.Player.TypeName)
				{
					if (tableView.Current != null)
					{
						this.EntityID = tableView.Current.Id;
					}
				}
			}

			this.Text = "פרטי שחקן";
		}
		
		public int EntityID
		{
			get { return _playerID; }
			set
			{
				try
				{
					_playerID = value;
					RefreshDetails();
					//Sport.Entities.Player player=new Sport.Entities.Player(value);
					
					//_playerID = value;
				}
				catch (Exception e)
				{
					System.Diagnostics.Debug.WriteLine("failed to create player details. ID: "+value.ToString());
					System.Diagnostics.Debug.WriteLine("error: "+e.Message);
					_playerID = -1;
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
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel2 = new System.Windows.Forms.Panel();
			this.btnClose = new Sport.UI.Controls.ThemeButton();
			this.btnTableView = new Sport.UI.Controls.ThemeButton();
			this.tcDetails = new System.Windows.Forms.TabControl();
			this.tpGeneral = new System.Windows.Forms.TabPage();
			this.lbBirthDate = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.lbLastName = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.lbFirstName = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.tpSchool = new System.Windows.Forms.TabPage();
			this.lbPlayersCount = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.lbSchoolSymbol = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.lbSchoolName = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.tpTeams = new System.Windows.Forms.TabPage();
			this.btnChampionshipView = new Sport.UI.Controls.ThemeButton();
			this.lbChampionshipName = new System.Windows.Forms.Label();
			this.lbTeamName = new System.Windows.Forms.Label();
			this.championshipNameLabel = new System.Windows.Forms.Label();
			this.teamNameLabel = new System.Windows.Forms.Label();
			this.lstPlayers = new System.Windows.Forms.ListBox();
			this.panel2.SuspendLayout();
			this.tcDetails.SuspendLayout();
			this.tpGeneral.SuspendLayout();
			this.tpSchool.SuspendLayout();
			this.tpTeams.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
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
			this.btnTableView.Text = "עבור למסך שחקנים";
			this.btnTableView.Transparent = System.Drawing.Color.Black;
			this.btnTableView.Click += new System.EventHandler(this.btnTableView_Click);
			// 
			// tcDetails
			// 
			this.tcDetails.Controls.Add(this.tpGeneral);
			this.tcDetails.Controls.Add(this.tpSchool);
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
			this.tpGeneral.Controls.Add(this.lbBirthDate);
			this.tpGeneral.Controls.Add(this.label5);
			this.tpGeneral.Controls.Add(this.lbLastName);
			this.tpGeneral.Controls.Add(this.label4);
			this.tpGeneral.Controls.Add(this.lbFirstName);
			this.tpGeneral.Controls.Add(this.label2);
			this.tpGeneral.Location = new System.Drawing.Point(4, 22);
			this.tpGeneral.Name = "tpGeneral";
			this.tpGeneral.Size = new System.Drawing.Size(408, 139);
			this.tpGeneral.TabIndex = 0;
			this.tpGeneral.Text = "כללי";
			// 
			// lbBirthDate
			// 
			this.lbBirthDate.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbBirthDate.ForeColor = System.Drawing.Color.Blue;
			this.lbBirthDate.Location = new System.Drawing.Point(216, 64);
			this.lbBirthDate.Name = "lbBirthDate";
			this.lbBirthDate.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.lbBirthDate.Size = new System.Drawing.Size(88, 19);
			this.lbBirthDate.TabIndex = 6;
			this.lbBirthDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Font = new System.Drawing.Font("Arial", 10F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label5.Location = new System.Drawing.Point(312, 64);
			this.label5.Name = "label5";
			this.label5.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.label5.Size = new System.Drawing.Size(75, 19);
			this.label5.TabIndex = 5;
			this.label5.Text = "תאריך לידה:";
			// 
			// lbLastName
			// 
			this.lbLastName.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbLastName.ForeColor = System.Drawing.Color.Blue;
			this.lbLastName.Location = new System.Drawing.Point(24, 16);
			this.lbLastName.Name = "lbLastName";
			this.lbLastName.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.lbLastName.Size = new System.Drawing.Size(88, 19);
			this.lbLastName.TabIndex = 4;
			this.lbLastName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("Arial", 10F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label4.Location = new System.Drawing.Point(120, 16);
			this.label4.Name = "label4";
			this.label4.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.label4.Size = new System.Drawing.Size(75, 19);
			this.label4.TabIndex = 3;
			this.label4.Text = "שם משפחה:";
			// 
			// lbFirstName
			// 
			this.lbFirstName.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbFirstName.ForeColor = System.Drawing.Color.Blue;
			this.lbFirstName.Location = new System.Drawing.Point(216, 16);
			this.lbFirstName.Name = "lbFirstName";
			this.lbFirstName.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.lbFirstName.Size = new System.Drawing.Size(88, 19);
			this.lbFirstName.TabIndex = 2;
			this.lbFirstName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Arial", 10F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label2.Location = new System.Drawing.Point(328, 16);
			this.label2.Name = "label2";
			this.label2.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.label2.Size = new System.Drawing.Size(59, 19);
			this.label2.TabIndex = 1;
			this.label2.Text = "שם פרטי:";
			// 
			// tpSchool
			// 
			this.tpSchool.Controls.Add(this.lbPlayersCount);
			this.tpSchool.Controls.Add(this.label7);
			this.tpSchool.Controls.Add(this.lbSchoolSymbol);
			this.tpSchool.Controls.Add(this.label6);
			this.tpSchool.Controls.Add(this.lbSchoolName);
			this.tpSchool.Controls.Add(this.label10);
			this.tpSchool.Location = new System.Drawing.Point(4, 22);
			this.tpSchool.Name = "tpSchool";
			this.tpSchool.Size = new System.Drawing.Size(408, 139);
			this.tpSchool.TabIndex = 1;
			this.tpSchool.Text = "בית ספר";
			// 
			// lbPlayersCount
			// 
			this.lbPlayersCount.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbPlayersCount.ForeColor = System.Drawing.Color.Blue;
			this.lbPlayersCount.Location = new System.Drawing.Point(232, 96);
			this.lbPlayersCount.Name = "lbPlayersCount";
			this.lbPlayersCount.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.lbPlayersCount.Size = new System.Drawing.Size(56, 19);
			this.lbPlayersCount.TabIndex = 14;
			this.lbPlayersCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Font = new System.Drawing.Font("Arial", 10F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label7.Location = new System.Drawing.Point(304, 96);
			this.label7.Name = "label7";
			this.label7.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.label7.Size = new System.Drawing.Size(94, 19);
			this.label7.TabIndex = 13;
			this.label7.Text = "מספר תלמידים:";
			// 
			// lbSchoolSymbol
			// 
			this.lbSchoolSymbol.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbSchoolSymbol.ForeColor = System.Drawing.Color.Blue;
			this.lbSchoolSymbol.Location = new System.Drawing.Point(200, 56);
			this.lbSchoolSymbol.Name = "lbSchoolSymbol";
			this.lbSchoolSymbol.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.lbSchoolSymbol.Size = new System.Drawing.Size(88, 19);
			this.lbSchoolSymbol.TabIndex = 12;
			this.lbSchoolSymbol.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Font = new System.Drawing.Font("Arial", 10F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label6.Location = new System.Drawing.Point(304, 56);
			this.label6.Name = "label6";
			this.label6.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.label6.Size = new System.Drawing.Size(86, 19);
			this.label6.TabIndex = 11;
			this.label6.Text = "סמל בית ספר:";
			// 
			// lbSchoolName
			// 
			this.lbSchoolName.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbSchoolName.ForeColor = System.Drawing.Color.Blue;
			this.lbSchoolName.Location = new System.Drawing.Point(72, 16);
			this.lbSchoolName.Name = "lbSchoolName";
			this.lbSchoolName.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.lbSchoolName.Size = new System.Drawing.Size(216, 19);
			this.lbSchoolName.TabIndex = 8;
			this.lbSchoolName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Font = new System.Drawing.Font("Arial", 10F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label10.Location = new System.Drawing.Point(304, 16);
			this.label10.Name = "label10";
			this.label10.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.label10.Size = new System.Drawing.Size(80, 19);
			this.label10.TabIndex = 7;
			this.label10.Text = "שם בית ספר:";
			// 
			// tpTeams
			// 
			this.tpTeams.Controls.Add(this.btnChampionshipView);
			this.tpTeams.Controls.Add(this.lbChampionshipName);
			this.tpTeams.Controls.Add(this.lbTeamName);
			this.tpTeams.Controls.Add(this.championshipNameLabel);
			this.tpTeams.Controls.Add(this.teamNameLabel);
			this.tpTeams.Controls.Add(this.lstPlayers);
			this.tpTeams.Location = new System.Drawing.Point(4, 22);
			this.tpTeams.Name = "tpTeams";
			this.tpTeams.Size = new System.Drawing.Size(408, 139);
			this.tpTeams.TabIndex = 2;
			this.tpTeams.Text = "קבוצות";
			// 
			// btnChampionshipView
			// 
			this.btnChampionshipView.Alignment = System.Drawing.StringAlignment.Center;
			this.btnChampionshipView.AutoSize = false;
			this.btnChampionshipView.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnChampionshipView.Hue = 160F;
			this.btnChampionshipView.Image = null;
			this.btnChampionshipView.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btnChampionshipView.ImageList = null;
			this.btnChampionshipView.ImageSize = new System.Drawing.Size(0, 0);
			this.btnChampionshipView.Location = new System.Drawing.Point(320, 96);
			this.btnChampionshipView.Name = "btnChampionshipView";
			this.btnChampionshipView.Saturation = 0.6F;
			this.btnChampionshipView.Size = new System.Drawing.Size(75, 32);
			this.btnChampionshipView.TabIndex = 12;
			this.btnChampionshipView.Text = "עבור למסך אליפות";
			this.btnChampionshipView.Transparent = System.Drawing.Color.Black;
			this.btnChampionshipView.Click += new System.EventHandler(this.btnChampionshipView_Click);
			// 
			// lbChampionshipName
			// 
			this.lbChampionshipName.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbChampionshipName.ForeColor = System.Drawing.Color.Blue;
			this.lbChampionshipName.Location = new System.Drawing.Point(24, 32);
			this.lbChampionshipName.Name = "lbChampionshipName";
			this.lbChampionshipName.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.lbChampionshipName.Size = new System.Drawing.Size(296, 19);
			this.lbChampionshipName.TabIndex = 11;
			this.lbChampionshipName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lbTeamName
			// 
			this.lbTeamName.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbTeamName.ForeColor = System.Drawing.Color.Blue;
			this.lbTeamName.Location = new System.Drawing.Point(24, 8);
			this.lbTeamName.Name = "lbTeamName";
			this.lbTeamName.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.lbTeamName.Size = new System.Drawing.Size(296, 19);
			this.lbTeamName.TabIndex = 10;
			this.lbTeamName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// championshipNameLabel
			// 
			this.championshipNameLabel.AutoSize = true;
			this.championshipNameLabel.Font = new System.Drawing.Font("Arial", 10F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.championshipNameLabel.Location = new System.Drawing.Point(320, 32);
			this.championshipNameLabel.Name = "championshipNameLabel";
			this.championshipNameLabel.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.championshipNameLabel.Size = new System.Drawing.Size(80, 19);
			this.championshipNameLabel.TabIndex = 9;
			this.championshipNameLabel.Text = "שם האליפות:";
			// 
			// teamNameLabel
			// 
			this.teamNameLabel.AutoSize = true;
			this.teamNameLabel.Font = new System.Drawing.Font("Arial", 10F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.teamNameLabel.Location = new System.Drawing.Point(320, 8);
			this.teamNameLabel.Name = "teamNameLabel";
			this.teamNameLabel.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.teamNameLabel.Size = new System.Drawing.Size(75, 19);
			this.teamNameLabel.TabIndex = 8;
			this.teamNameLabel.Text = "שם הקבוצה:";
			// 
			// lstPlayers
			// 
			this.lstPlayers.Location = new System.Drawing.Point(16, 56);
			this.lstPlayers.Name = "lstPlayers";
			this.lstPlayers.Size = new System.Drawing.Size(288, 69);
			this.lstPlayers.TabIndex = 0;
			// 
			// PlayerDetails
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(416, 229);
			this.Controls.Add(this.tcDetails);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.panel1);
			this.Name = "PlayerDetails";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "PlayerDetails";
			this.panel2.ResumeLayout(false);
			this.tcDetails.ResumeLayout(false);
			this.tpGeneral.ResumeLayout(false);
			this.tpSchool.ResumeLayout(false);
			this.tpTeams.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void btnClose_Click(object sender, System.EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
		}

		private void RefreshDetails()
		{
			//decide what to do based on the current id
			btnTableView.Enabled = (_playerID >= 0);
			
			if (_playerID >= 0)
			{
				Sport.UI.Dialogs.WaitForm.ShowWait("טוען נתונים אנא המתן...");
				
				Sport.Entities.Player player=new Sport.Entities.Player(_playerID);
				lbFirstName.Text = player.FirstName;
				lbLastName.Text = player.LastName;
				lbBirthDate.Text = player.Student.BirthDate.ToShortDateString();
				lbSchoolName.Text = player.Student.School.Name;
				lbSchoolSymbol.Text = player.Student.School.Symbol;
				lbPlayersCount.Text = player.Student.School.StudentsCount.ToString();
				lbTeamName.Text = player.Team.Name;
				lbChampionshipName.Text = player.Team.Championship.Name;
				Sport.Entities.Team team = new Sport.Entities.Team(player.Team.Id);
				FillPlayers(team);
				
				Sport.UI.Dialogs.WaitForm.HideWait();
			}
			else
			{
				lbFirstName.Text = "";
				lbLastName.Text = "";
				lbBirthDate.Text = "";
				lbSchoolName.Text = "";
				lbSchoolSymbol.Text = "";
				lbPlayersCount.Text = "";
				lstPlayers.Items.Clear();
			}
			this.Text = "פרטי שחקן";
			if (_playerID >= 0)
				this.Text += " - "+Sport.Entities.Player.Type.Lookup(_playerID).Name;
		}
		
		private void FillPlayers(Sport.Entities.Team team)
		{
			lstPlayers.Items.Clear();
			EntityFilter filter=new EntityFilter((int) Sport.Entities.Player.Fields.Team, team.Id);
			Entity[] players=Sport.Entities.Player.Type.GetEntities(filter);
			foreach (Entity ent in players)
			{
				Sport.Entities.Player other_player=new Sport.Entities.Player(ent);
				lstPlayers.Items.Add(new Sport.Common.ListItem(other_player.Name, other_player.IdNumber));
			}
		}
		
		private void btnTableView_Click(object sender, System.EventArgs e)
		{
			if (_playerID >= 0)
			{
				Sport.UI.ViewManager.OpenView(typeof(Views.PlayersTableView), "player="+_playerID.ToString());
				this.DialogResult = DialogResult.OK;
			}
		}

		private void btnChampionshipView_Click(object sender, System.EventArgs e)
		{
			Sport.Entities.Player player=new Sport.Entities.Player(_playerID);
			Sport.UI.ViewManager.OpenView(typeof(Views.ChampionshipsTableView), "championship=" + player.Team.Championship.Id.ToString());
			this.DialogResult = DialogResult.OK;
		}
	} //end class PlayerDetails
}
