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
	/// Summary description for TeamDetails.
	/// </summary>
	public class TeamDetails : System.Windows.Forms.Form
	{
		private int _teamID=-1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel2;
		private Sport.UI.Controls.ThemeButton btnTableView;
		private Sport.UI.Controls.ThemeButton btnClose;
		private System.Windows.Forms.TabControl tcDetails;
		private System.Windows.Forms.TabPage tpGeneral;
		private System.Windows.Forms.TabPage tpSchool;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label lbSchoolSymbol;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label lbSchoolName;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label lbTeamsCount;
		private System.Windows.Forms.Label label7;
		private Sport.UI.Controls.ThemeButton btnChampionshipView;
		private System.Windows.Forms.Label lbName;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label lbStatus;
		private System.Windows.Forms.TabPage tpPlayers;
		private System.Windows.Forms.Label lbRegistrationDate;
		private System.Windows.Forms.Label lbChampionship;
		private System.Windows.Forms.ListBox lstPlayers;
		private Sport.UI.Controls.ThemeButton btnPlayerView;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public TeamDetails()
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
				if (tableView.EntityListView.EntityName == Sport.Entities.Team.TypeName)
				{
					if (tableView.Current != null)
					{
						this.EntityID = tableView.Current.Id;
					}
				}
			}

			this.Text = "פרטי קבוצה";
		}
		
		public int EntityID
		{
			get { return _teamID; }
			set
			{
				try
				{
					_teamID = value;
					RefreshDetails();
					//Sport.Entities.Team team=new Sport.Entities.Team(value);
					
					//_teamID = value;
				}
				catch (Exception e)
				{
					System.Diagnostics.Debug.WriteLine("failed to create team details. ID: "+value.ToString());
					System.Diagnostics.Debug.WriteLine("error: "+e.Message);
					_teamID = -1;
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
			this.lbStatus = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.lbRegistrationDate = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.lbChampionship = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.lbName = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.tpSchool = new System.Windows.Forms.TabPage();
			this.lbTeamsCount = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.lbSchoolSymbol = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.lbSchoolName = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.tpPlayers = new System.Windows.Forms.TabPage();
			this.btnChampionshipView = new Sport.UI.Controls.ThemeButton();
			this.lstPlayers = new System.Windows.Forms.ListBox();
			this.btnPlayerView = new Sport.UI.Controls.ThemeButton();
			this.panel2.SuspendLayout();
			this.tcDetails.SuspendLayout();
			this.tpGeneral.SuspendLayout();
			this.tpSchool.SuspendLayout();
			this.tpPlayers.SuspendLayout();
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
			this.btnTableView.Text = "עבור למסך קבוצות";
			this.btnTableView.Transparent = System.Drawing.Color.Black;
			this.btnTableView.Click += new System.EventHandler(this.btnTableView_Click);
			// 
			// tcDetails
			// 
			this.tcDetails.Controls.Add(this.tpGeneral);
			this.tcDetails.Controls.Add(this.tpSchool);
			this.tcDetails.Controls.Add(this.tpPlayers);
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
			this.tpGeneral.Controls.Add(this.lbStatus);
			this.tpGeneral.Controls.Add(this.label1);
			this.tpGeneral.Controls.Add(this.lbRegistrationDate);
			this.tpGeneral.Controls.Add(this.label5);
			this.tpGeneral.Controls.Add(this.lbChampionship);
			this.tpGeneral.Controls.Add(this.label4);
			this.tpGeneral.Controls.Add(this.lbName);
			this.tpGeneral.Controls.Add(this.label2);
			this.tpGeneral.Location = new System.Drawing.Point(4, 22);
			this.tpGeneral.Name = "tpGeneral";
			this.tpGeneral.Size = new System.Drawing.Size(408, 139);
			this.tpGeneral.TabIndex = 0;
			this.tpGeneral.Text = "כללי";
			// 
			// lbStatus
			// 
			this.lbStatus.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbStatus.ForeColor = System.Drawing.Color.Blue;
			this.lbStatus.Location = new System.Drawing.Point(24, 112);
			this.lbStatus.Name = "lbStatus";
			this.lbStatus.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.lbStatus.Size = new System.Drawing.Size(88, 19);
			this.lbStatus.TabIndex = 8;
			this.lbStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Arial", 10F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label1.Location = new System.Drawing.Point(144, 112);
			this.label1.Name = "label1";
			this.label1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.label1.Size = new System.Drawing.Size(47, 19);
			this.label1.TabIndex = 7;
			this.label1.Text = "סטטוס:";
			// 
			// lbRegistrationDate
			// 
			this.lbRegistrationDate.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbRegistrationDate.ForeColor = System.Drawing.Color.Blue;
			this.lbRegistrationDate.Location = new System.Drawing.Point(216, 112);
			this.lbRegistrationDate.Name = "lbRegistrationDate";
			this.lbRegistrationDate.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.lbRegistrationDate.Size = new System.Drawing.Size(88, 19);
			this.lbRegistrationDate.TabIndex = 6;
			this.lbRegistrationDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Font = new System.Drawing.Font("Arial", 10F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label5.Location = new System.Drawing.Point(320, 112);
			this.label5.Name = "label5";
			this.label5.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.label5.Size = new System.Drawing.Size(82, 19);
			this.label5.TabIndex = 5;
			this.label5.Text = "תאריך רישום:";
			// 
			// lbChampionship
			// 
			this.lbChampionship.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbChampionship.ForeColor = System.Drawing.Color.Blue;
			this.lbChampionship.Location = new System.Drawing.Point(24, 64);
			this.lbChampionship.Name = "lbChampionship";
			this.lbChampionship.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.lbChampionship.Size = new System.Drawing.Size(280, 19);
			this.lbChampionship.TabIndex = 4;
			this.lbChampionship.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("Arial", 10F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label4.Location = new System.Drawing.Point(320, 64);
			this.label4.Name = "label4";
			this.label4.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.label4.Size = new System.Drawing.Size(80, 19);
			this.label4.TabIndex = 3;
			this.label4.Text = "שם האליפות:";
			// 
			// lbName
			// 
			this.lbName.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbName.ForeColor = System.Drawing.Color.Blue;
			this.lbName.Location = new System.Drawing.Point(24, 16);
			this.lbName.Name = "lbName";
			this.lbName.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.lbName.Size = new System.Drawing.Size(280, 19);
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
			this.label2.Size = new System.Drawing.Size(75, 19);
			this.label2.TabIndex = 1;
			this.label2.Text = "שם הקבוצה:";
			// 
			// tpSchool
			// 
			this.tpSchool.Controls.Add(this.lbTeamsCount);
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
			// lbTeamsCount
			// 
			this.lbTeamsCount.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbTeamsCount.ForeColor = System.Drawing.Color.Blue;
			this.lbTeamsCount.Location = new System.Drawing.Point(232, 96);
			this.lbTeamsCount.Name = "lbTeamsCount";
			this.lbTeamsCount.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.lbTeamsCount.Size = new System.Drawing.Size(56, 19);
			this.lbTeamsCount.TabIndex = 14;
			this.lbTeamsCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
			// tpPlayers
			// 
			this.tpPlayers.Controls.Add(this.btnPlayerView);
			this.tpPlayers.Controls.Add(this.btnChampionshipView);
			this.tpPlayers.Controls.Add(this.lstPlayers);
			this.tpPlayers.Location = new System.Drawing.Point(4, 22);
			this.tpPlayers.Name = "tpPlayers";
			this.tpPlayers.Size = new System.Drawing.Size(408, 139);
			this.tpPlayers.TabIndex = 2;
			this.tpPlayers.Text = "שחקנים";
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
			// lstPlayers
			// 
			this.lstPlayers.Location = new System.Drawing.Point(16, 16);
			this.lstPlayers.Name = "lstPlayers";
			this.lstPlayers.Size = new System.Drawing.Size(288, 108);
			this.lstPlayers.TabIndex = 0;
			this.lstPlayers.SelectedIndexChanged += new System.EventHandler(this.lstPlayers_SelectedIndexChanged);
			// 
			// btnPlayerView
			// 
			this.btnPlayerView.Alignment = System.Drawing.StringAlignment.Center;
			this.btnPlayerView.AutoSize = false;
			this.btnPlayerView.Enabled = false;
			this.btnPlayerView.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnPlayerView.Hue = 160F;
			this.btnPlayerView.Image = null;
			this.btnPlayerView.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btnPlayerView.ImageList = null;
			this.btnPlayerView.ImageSize = new System.Drawing.Size(0, 0);
			this.btnPlayerView.Location = new System.Drawing.Point(320, 16);
			this.btnPlayerView.Name = "btnPlayerView";
			this.btnPlayerView.Saturation = 0.6F;
			this.btnPlayerView.Size = new System.Drawing.Size(75, 32);
			this.btnPlayerView.TabIndex = 13;
			this.btnPlayerView.Text = "עבור למסך שחקנים";
			this.btnPlayerView.Transparent = System.Drawing.Color.Black;
			this.btnPlayerView.Click += new System.EventHandler(this.btnPlayerView_Click);
			// 
			// TeamDetails
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(416, 229);
			this.Controls.Add(this.tcDetails);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.panel1);
			this.Name = "TeamDetails";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "TeamDetails";
			this.panel2.ResumeLayout(false);
			this.tcDetails.ResumeLayout(false);
			this.tpGeneral.ResumeLayout(false);
			this.tpSchool.ResumeLayout(false);
			this.tpPlayers.ResumeLayout(false);
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
			btnTableView.Enabled = (_teamID >= 0);
			
			if (_teamID >= 0)
			{
				Sport.UI.Dialogs.WaitForm.ShowWait("טוען נתונים אנא המתן...");
				Sport.Entities.Team team=new Sport.Entities.Team(_teamID);
				lbName.Text = team.Name;
				lbChampionship.Text = team.Championship.Name;
				lbRegistrationDate.Text = team.RegisterDate.ToShortDateString();
				if (team.Status == Sport.Types.TeamStatusType.Registered)
					lbStatus.ForeColor = System.Drawing.Color.Red;
				else
					lbStatus.ForeColor = System.Drawing.Color.Green;
				lbStatus.Text = (new Sport.Types.TeamStatusLookup()).Lookup((int) team.Status);
				lbSchoolName.Text = team.School.Name;
				lbSchoolSymbol.Text = team.School.Symbol;
				lbTeamsCount.Text = team.School.StudentsCount.ToString();
				
				FillTeams(team);
				
				Sport.UI.Dialogs.WaitForm.HideWait();
			}
			else
			{
				
				lbName.Text = "";
				lbChampionship.Text = "";
				lbRegistrationDate.Text = "";
				lbSchoolName.Text = "";
				lbSchoolSymbol.Text = "";
				lbTeamsCount.Text = "";
				lstPlayers.Items.Clear();
			
			}
			this.Text = "פרטי קבוצה";
			if (_teamID >= 0)
				this.Text += " - "+Sport.Entities.Team.Type.Lookup(_teamID).Name;
		}
		
		private void FillTeams(Sport.Entities.Team team)
		{
			lstPlayers.Items.Clear();
			EntityFilter filter=new EntityFilter((int) Sport.Entities.Player.Fields.Team, team.Id);
			Entity[] players=Sport.Entities.Player.Type.GetEntities(filter);
			foreach (Entity ent in players)
			{
				Sport.Entities.Player other_player=new Sport.Entities.Player(ent);
				lstPlayers.Items.Add(new Sport.Common.ListItem(other_player.Name, other_player.Id));
			}
		}
		
		private void btnTableView_Click(object sender, System.EventArgs e)
		{
			if (_teamID >= 0)
			{
				Sport.UI.ViewManager.OpenView(typeof(Views.TeamsTableView), "team="+_teamID.ToString());
				this.DialogResult = DialogResult.OK;
			}
		}

		private void btnChampionshipView_Click(object sender, System.EventArgs e)
		{
			Sport.Entities.Team team=new Sport.Entities.Team(_teamID);
			Sport.UI.ViewManager.OpenView(typeof(Views.ChampionshipsTableView), "championship=" + team.Championship.Id.ToString());
			this.DialogResult = DialogResult.OK;
		}

		private void btnPlayerView_Click(object sender, System.EventArgs e)
		{
			int selIndex=lstPlayers.SelectedIndex;
			if (selIndex < 0)
				return;
			ListItem selectedItem=(ListItem) lstPlayers.Items[selIndex];
			int playerID=(int) selectedItem.Value;
			Sport.UI.ViewManager.OpenView(typeof(Views.PlayersTableView), "player="+playerID.ToString());
			this.DialogResult = DialogResult.OK;
		}

		private void lstPlayers_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			btnPlayerView.Enabled = (lstPlayers.SelectedIndex >= 0);
		}



	} //end class TeamDetails
}
