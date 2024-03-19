using System;
using System.Linq;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using Sport.UI;
using System.Collections;
using Mir.Common;
using System.Collections.Generic;

namespace Sportsman.Producer
{
	public class ChampionshipEditorView : Sport.UI.View
	{
		#region Group Panel
		protected System.Windows.Forms.Panel	panelGroupView;
		protected PhaseGroupGridPanel			phaseGroupGridPanel;
		private System.Windows.Forms.Label		labelGroup;
		protected System.Windows.Forms.Panel	pnlRight;
		protected Sport.UI.Controls.ThemeButton	tbCustom;
		protected Sport.UI.Controls.ThemeButton tbExport;
		protected Sport.UI.Controls.ThemeButton tbImport;
		protected Sport.UI.Controls.ThemeButton	tbSetResults;
		protected Sport.UI.Controls.ThemeButton tbOfflineEntities;
		private Sport.UI.Controls.ThemeButton	tbRankingView;
		#endregion

		private enum WebsitePermanentChampState
		{
			Exists = 0,
			DoesNotExist = 1
		}

		private string[] websitePermanentChampButtonTexts = new string[] { "עריכת פרטי אליפות קבועה באתר", 
			"הגדר כאליפות קבועה באתר" };

		#region Ranking Panel

		private System.Windows.Forms.Panel		panelRankingView;
		private RankingGridPanel				rankingGridPanel;
		protected Sport.UI.Controls.ThemeButton	tbGroupView;
		private Sport.UI.Controls.ThemeButton	tbTeamDown;
		private Sport.UI.Controls.ThemeButton	tbTeamUp;
		protected Sport.UI.Controls.ThemeButton tbWebsitePermanentChamp;

		private void SetTeamRankButtons()
		{
			if (rankingGridPanel != null)
			{
				Sport.Championships.Team team = rankingGridPanel.Team;
				if (team == null)
				{
					tbTeamUp.Enabled = false;
					tbTeamDown.Enabled = false;
				}
				else
				{
					tbTeamUp.Enabled = team.Position > 0;
					if (team.Group != null && team.Group.Teams != null)
						tbTeamDown.Enabled = team.Position < team.Group.Teams.Count - 1;
				}
			}
		}
		
		private void RankingGridPanelSelectionChanged(object sender, System.EventArgs e)
		{
			SetTeamRankButtons();
		}

		private void tbGroupView_Click(object sender, System.EventArgs e)
		{
			SetGroupDisplay();		
		}

		private void tbTeamUp_Click(object sender, System.EventArgs e)
		{
			Sport.Championships.Team teamA = rankingGridPanel.Team;

			if (teamA != null && teamA.Position > 0)
			{
				Sport.Championships.Group group = teamA.Group;
				Sport.Championships.Team teamB = null;
				for (int n = 0; n < group.Teams.Count && teamB == null; n++)
				{
					if (group.Teams[n].Position == teamA.Position - 1)
						teamB = group.Teams[n];
				}

				if (teamB != null)
				{
					string strError = group.ReplaceTeams(teamA.Index, teamB.Index);
					if (strError.Length == 0)
					{
						rankingGridPanel.Rebuild();
						SetTeamRankButtons();
					}
					else
					{
						Sport.UI.MessageBox.Error("שגיאה בעת ביצוע הפעולה. פרטים טכניים: " + "\n" + strError, "עריכת מבנה");
					}
				}
			}
		}

		private void tbTeamDown_Click(object sender, System.EventArgs e)
		{
			Sport.Championships.Team teamA = rankingGridPanel.Team;

			if (teamA != null)
			{
				Sport.Championships.Group group = teamA.Group;

				if (teamA.Position < group.Teams.Count - 1)
				{
					Sport.Championships.Team teamB = null;
					for (int n = 0; n < group.Teams.Count && teamB == null; n++)
					{
						if (group.Teams[n].Position == teamA.Position + 1)
							teamB = group.Teams[n];
					}

					if (teamB != null)
					{
						string strError = group.ReplaceTeams(teamA.Index, teamB.Index);
						if (strError.Length == 0)
						{
							rankingGridPanel.Rebuild();
							SetTeamRankButtons();
						}
						else
						{
							Sport.UI.MessageBox.Error("שגיאה בעת ביצוע הפעולה. פרטים טכניים: " + "\n" + strError, "עריכת מבנה");
						}
					}
				}
			}
		}

		#endregion

		#region Edit Panels
		private System.Windows.Forms.Panel			panelEdit;
		private System.Windows.Forms.Label			labelPhase;
		private Sport.UI.Controls.ThemeButton		tbNextPhase;
		protected Sport.UI.Controls.ThemeButton		tbPrevPhase;
		protected Sport.UI.Controls.ThemeButton		tbEditGroup;
		private Sport.UI.Controls.ThemeButton		tbEditStructure;
		protected Sport.UI.Controls.ThemeButton		tbPrint;
		protected Sport.UI.Controls.ThemeButton		tbPrintCompetition;
		protected Sport.UI.Controls.ThemeButton		tbReports;
		protected Sport.UI.Controls.ThemeButton		tbReferreReport;
		protected Sport.UI.Controls.ThemeButton		tbRegisteredTeams;
		protected Sport.UI.Controls.ThemeButton tbLeagueSummary;
		
		private void tbEditStructure_Click(object sender, System.EventArgs e)
		{
			if (!Sport.Core.Session.Connected)
			{
				Sport.UI.MessageBox.Error("לא ניתן לערוך מבנה במצב לא מקוון", "עריכת אליפות");
				return;
			}
			
			try
			{
				EditStructure();
			}
			catch (Exception ex)
			{
				if (ex.Message.IndexOf("Championship already in edit") >= 0)
				{
					Sport.UI.MessageBox.Error("האליפות כבר במצב עריכה אנא בטל או שמור כדי לערוך שינויים", "שינוי מבנה אליפות");
				}
				else
				{
					throw;
				}
			}
		}
		
		private void tbEditGroup_Click(object sender, System.EventArgs e)
		{
			try
			{
				EditGroup();
			}
			catch (Exception ex)
			{
				Sport.UI.MessageBox.Error("לא ניתן לערוך:\n" + ex.Message, "עריכת בתים");
			}
		}
		
		#region Group Panel

		protected System.Windows.Forms.Panel		panelGroupEdit;
		private Sport.UI.Controls.ThemeButton		tbCancelGroup;
		private Sport.UI.Controls.ThemeButton		tbSaveGroup;

		#endregion

		#region Structure Panel

		protected System.Windows.Forms.Panel			panelStructureEdit;

		private void tbSaveStructure_Click(object sender, System.EventArgs e)
		{
			SaveStructure();
		}

		private void tbCancelStructure_Click(object sender, System.EventArgs e)
		{
			Cancel();
		}


		#region Phase

		private System.Windows.Forms.GroupBox	gbPhase;
		private System.Windows.Forms.TextBox	tbPhaseName;
		private Sport.UI.Controls.ThemeButton	tbMovePhase;
		private Sport.UI.Controls.ThemeButton	tbAddPhase;
		private Sport.UI.Controls.ThemeButton	tbRemovePhase;
		private Sport.UI.Controls.ThemeButton	tbPhaseDefinition;

		private void tbPhaseName_TextChanged(object sender, System.EventArgs e)
		{
			Sport.Championships.Phase phase = Phase;
			if (phase != null && !selecting)
			{
				phase.Name = tbPhaseName.Text;
				phases.SelectedItem.Text = tbPhaseName.Text;
			}
		}

		private void tbMovePhase_Click(object sender, System.EventArgs e)
		{
			Sport.Championships.Phase phase = Phase;
			if (phase != null)
			{
				Sport.UI.Dialogs.GenericEditDialog ged = 
					new Sport.UI.Dialogs.GenericEditDialog("בחר מיקום");

				int current = phase.Index;

				ged.Items.Add(Sport.UI.Controls.GenericItemType.Number, 
					current + 1, new object[] { 1d, (double) championship.Phases.Count });

				if (ged.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					int pos = (int) (double) ged.Items[0].Value;
					int index = pos - 1;

					if (index == current)
						return ;

					championship.Phases.RemoveAt(current);
					phases.Items.RemoveAt(current);
					championship.Phases.Insert(index, phase);
					phases.Items.Insert(index, phase.Name);
					phases.SelectedIndex = index;
				}
			}
		}

		private void tbAddPhase_Click(object sender, System.EventArgs e)
		{
			Sport.Championships.Phase phase = CreatePhase("שלב " + (championship.Phases.Count + 1).ToString());

			if (phase != null)
			{
				int index = championship.Phases.Add(phase);

				phases.Items.Insert(index, phase.Name);
				phases.SelectedIndex = index;

				Phase = phase;
			}
		}

		private void tbRemovePhase_Click(object sender, System.EventArgs e)
		{
			Sport.Championships.Phase phase = Phase;
			if (phase != null)
			{
				int current = phase.Index;

				string warn = "פעולה זאת תמחוק את השלב '" + phase.Name + "'";
				if (current < championship.Phases.Count - 1)
				{
					warn += " ואת הקבוצות והמשחקים משלב '" + championship.Phases[current + 1].Name + "'";
				}

				if (!Sport.UI.MessageBox.Ask(warn + ", האם להמשיך?", 
					System.Windows.Forms.MessageBoxIcon.Warning, false))
					return ;

				championship.Phases.RemoveAt(current);
				phases.Items.RemoveAt(current);
				int p = current - 1;
				if (p < 0 && phases.Items.Count > 0)
					p = 0;
				phases.SelectedIndex = p;
				Phase = null;
				if (p != -1)
					Phase = championship.Phases[p];
			}
		}
		
		private void tbPhaseDefinition_Click(object sender, System.EventArgs e)
		{
			Sport.Championships.Phase phase = Phase;
			if (phase != null)
			{
				PhaseDefinitionsForm pdf = new PhaseDefinitionsForm(phase);
				pdf.ShowDialog();
			}
		}
		
		#endregion

		#region Group

		private System.Windows.Forms.GroupBox	gbGroup;
		private System.Windows.Forms.TextBox	tbGroupName;
		private Sport.UI.Controls.ThemeButton	tbMoveGroup;
		private Sport.UI.Controls.ThemeButton	tbAddGroup;
		private Sport.UI.Controls.ThemeButton	tbRemoveGroup;

		private void tbGroupName_TextChanged(object sender, System.EventArgs e)
		{
			if (Group != null && !selecting)
			{
				Group.Name = tbGroupName.Text;
				phaseGroupGridPanel.Refresh();
			}
		}

		private void tbMoveGroup_Click(object sender, System.EventArgs e)
		{
			Sport.Championships.Group group = Group;
			if (group != null)
			{
				Sport.UI.Dialogs.GenericEditDialog ged = 
					new Sport.UI.Dialogs.GenericEditDialog("בחר מיקום");

				int current = group.Index;

				ged.Items.Add(Sport.UI.Controls.GenericItemType.Number, 
					current + 1, new object[] { 1d, (double) Phase.Groups.Count });

				if (ged.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					int pos = (int) (double) ged.Items[0].Value;
					int index = pos - 1;

					if (index == current)
						return ;

					Phase.Groups.RemoveAt(current);
					Phase.Groups.Insert(index, group);
					phaseGroupGridPanel.Rebuild();
				}
			}
		}

		private void tbAddGroup_Click(object sender, System.EventArgs e)
		{
			if (Phase != null)
			{
				Sport.Championships.Group group = CreateGroup("בית " + (Phase.Groups.Count + 1).ToString());

				if (group != null)
				{
					Phase.Groups.Add(group);
					phaseGroupGridPanel.Rebuild();

					Group = group;
				}
			}
		}


		private void tbRemoveGroup_Click(object sender, System.EventArgs e)
		{
			Sport.Championships.Group group = Group;
			if (group != null)
			{
				Sport.Championships.Phase phase = Phase;
				string warn = "פעולה זאת תמחוק את הבית '" + group.Name + "'";
				if (phase.Index < championship.Phases.Count - 1)
				{
					warn += " ואת הקבוצות והמשחקים משלב '" + phase.Name + "' המקושרים לבית זה";
				}

				if (!Sport.UI.MessageBox.Ask(warn + ", האם להמשיך?", 
					System.Windows.Forms.MessageBoxIcon.Warning, false))
					return ;

				int current = group.Index;

				phase.Groups.RemoveAt(current);
				phaseGroupGridPanel.Rebuild();

				int g = current - 1;
				if (g < 0 && phase.Groups.Count > 0)
					g = 0;

				Group = g == -1 ? null : phase.Groups[g];
			}
		}

		#endregion

		#region Team

		private System.Windows.Forms.GroupBox	gbTeam;
		private System.Windows.Forms.TextBox	tbTeamName;
		private Sport.UI.Controls.ThemeButton	tbMoveTeam;
		private Sport.UI.Controls.ThemeButton	tbDivideTeams;
		private Sport.UI.Controls.ThemeButton	tbAddTeam;
		private Sport.UI.Controls.ThemeButton	tbRemoveTeam;

		private void tbDivideTeams_Click(object sender, System.EventArgs e)
		{
			if (Phase != null)
			{
				if (championship != null)
				{
					TeamsDivisionForm tdf = null;
					try
					{
						tdf = new TeamsDivisionForm(championship, Phase.Index);
					}
					catch(Exception ex)
					{
						Logger.Instance.WriteLog(LogType.Error, "ChampionshipEditorView", "Error creating TeamsDivisionForm: " + ex.ToString());
						Sport.UI.MessageBox.Error("אירעה שגיאה כללית בעת טעינת נתונים אנא נסה שנית מאוחר יותר", "חלוקת שחקנים");
					}

					if (tdf != null)
					{
						if (tdf.ShowDialog() == DialogResult.OK)
						{
							phaseGroupGridPanel.Rebuild();
						}
					}
				}
			}
		}

		private void tbAddTeam_Click(object sender, System.EventArgs e)
		{
			if (Group != null)
			{
				int position = Group.Teams.Count;
				Sport.Championships.Team team = CreateTeam(ref position);

				if (team != null)
				{
					Group.Teams.Insert(position, team);
					phaseGroupGridPanel.Rebuild();

					Team = team;
				}
			}
		}

		private void tbMoveTeam_Click(object sender, System.EventArgs e)
		{
			Sport.Championships.Team team = Team;
			if (team != null)
			{
				Sport.UI.Dialogs.GenericEditDialog ged = 
					new Sport.UI.Dialogs.GenericEditDialog("בחר מיקום");

				int current = team.Index;

				ged.Items.Add(Sport.UI.Controls.GenericItemType.Number, 
					current + 1, new object[] { 1d, (double) Group.Teams.Count });

				if (ged.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					int pos = (int) (double) ged.Items[0].Value;
					int index = pos - 1;

					if (index == current)
						return ;

					Group.Teams.RemoveAt(current);
					Group.Teams.Insert(index, team);
					phaseGroupGridPanel.Rebuild();
				}
			}
		}

		private void tbRemoveTeam_Click(object sender, System.EventArgs e)
		{
			Sport.Championships.Team team = Team;
			if (team != null)
			{
				if (!Sport.UI.MessageBox.Ask("פעולה זאת תמחוק את הקבוצה '" +
					team.Name + "' ואת את כל המשחקים בה היא משתתפת, האם להמשיך?",
					System.Windows.Forms.MessageBoxIcon.Warning, false))
					return ;

				int current = team.Index;

				Group.Teams.RemoveAt(current);
				
				int t = current - 1;
				if (t < 0 && Group.Teams.Count > 0)
					t = 0;
				team = t == -1 ? null : Group.Teams[t];

				phaseGroupGridPanel.Rebuild();

				Team = team;
			}
		}

		#endregion

		private Sport.UI.Controls.ThemeButton		tbCancelStructure;
		private Sport.UI.Controls.ThemeButton		tbSaveStructure;
		
		#endregion

		private Sport.UI.Controls.CaptionBar		phases;

		protected void ResetPhases()
		{
			phases.Items.Clear();
			foreach (Sport.Championships.Phase phase in championship.Phases)
			{
				phases.Items.Add(phase.Name);
			}

			phases.SelectedIndex = _phase;
		}

		#endregion

		#region Initialization
		public ChampionshipEditorView()
		{
			_phase = -1;
			_group = -1;
			_team = -1;
			InitializeComponent();

			gbPhase.Location = new Point(panelStructureEdit.Width - 8 - gbPhase.Width, gbPhase.Top);
			gbGroup.Location = new Point(gbPhase.Left - 8 - gbGroup.Width, gbGroup.Top);
			gbTeam.Location = new Point(gbGroup.Left - 8 - gbTeam.Width, gbTeam.Top);
			
			//tbRegisteredTeams.Left = tbPrevPhase.Left-tbRegisteredTeams.Width-10;
			phaseGroupGridPanel = new PhaseGroupGridPanel();
			
			// 
			// phaseGroupGridPanel
			// 
			this.phaseGroupGridPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Right)));
			//this.phaseGroupGridPanel.Location = new System.Drawing.Point(550, 8);
			this.phaseGroupGridPanel.Dock = DockStyle.Right;
			this.phaseGroupGridPanel.Name = "phaseGroupGridPanel";
			this.phaseGroupGridPanel.Size = new System.Drawing.Size(220, 162);
			this.phaseGroupGridPanel.SelectionChanged += new EventHandler(PhaseGroupGridPanelSelectionChanged);
			this.panelGroupView.Controls.Add(this.phaseGroupGridPanel);
			
			rankingGridPanel = new RankingGridPanel();
			// 
			// rankingGridPanel
			// 
			this.rankingGridPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.rankingGridPanel.Location = new System.Drawing.Point(8, 24);
			this.rankingGridPanel.Name = "rankingGridPanel";
			this.rankingGridPanel.Size = new System.Drawing.Size(701, 146);
			this.rankingGridPanel.SelectionChanged += new System.EventHandler(this.RankingGridPanelSelectionChanged);
			this.panelRankingView.Controls.Add(this.rankingGridPanel);
			
			SetGroupDisplay();
		}

		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ChampionshipEditorView));
			this.phases = new Sport.UI.Controls.CaptionBar();
			this.panelStructureEdit = new System.Windows.Forms.Panel();
			this.gbTeam = new System.Windows.Forms.GroupBox();
			this.tbMoveTeam = new Sport.UI.Controls.ThemeButton();
			this.tbDivideTeams = new Sport.UI.Controls.ThemeButton();
			this.tbRemoveTeam = new Sport.UI.Controls.ThemeButton();
			this.tbAddTeam = new Sport.UI.Controls.ThemeButton();
			this.tbTeamName = new System.Windows.Forms.TextBox();
			this.gbGroup = new System.Windows.Forms.GroupBox();
			this.tbMoveGroup = new Sport.UI.Controls.ThemeButton();
			this.tbGroupName = new System.Windows.Forms.TextBox();
			this.tbRemoveGroup = new Sport.UI.Controls.ThemeButton();
			this.tbAddGroup = new Sport.UI.Controls.ThemeButton();
			this.gbPhase = new System.Windows.Forms.GroupBox();
			this.tbMovePhase = new Sport.UI.Controls.ThemeButton();
			this.tbPhaseName = new System.Windows.Forms.TextBox();
			this.tbRemovePhase = new Sport.UI.Controls.ThemeButton();
			this.tbAddPhase = new Sport.UI.Controls.ThemeButton();
			this.tbPhaseDefinition = new Sport.UI.Controls.ThemeButton();
			this.tbCancelStructure = new Sport.UI.Controls.ThemeButton();
			this.tbSaveStructure = new Sport.UI.Controls.ThemeButton();
			this.panelEdit = new System.Windows.Forms.Panel();
			this.tbPrint = new Sport.UI.Controls.ThemeButton();
			this.tbPrintCompetition = new Sport.UI.Controls.ThemeButton();
			this.tbReports = new Sport.UI.Controls.ThemeButton();
			this.tbLeagueSummary = new Sport.UI.Controls.ThemeButton();
			this.tbReferreReport = new Sport.UI.Controls.ThemeButton();
			this.tbRegisteredTeams = new Sport.UI.Controls.ThemeButton();
			this.tbEditGroup = new Sport.UI.Controls.ThemeButton();
			this.tbPrevPhase = new Sport.UI.Controls.ThemeButton();
			this.labelPhase = new System.Windows.Forms.Label();
			this.tbNextPhase = new Sport.UI.Controls.ThemeButton();
			this.tbEditStructure = new Sport.UI.Controls.ThemeButton();
			this.tbSetResults = new Sport.UI.Controls.ThemeButton();
			this.panelGroupView = new System.Windows.Forms.Panel();
			this.tbCustom = new Sport.UI.Controls.ThemeButton();
			this.tbOfflineEntities = new Sport.UI.Controls.ThemeButton();
			this.pnlRight = new System.Windows.Forms.Panel();
			this.tbImport = new Sport.UI.Controls.ThemeButton();
			this.tbExport = new Sport.UI.Controls.ThemeButton();
			this.labelGroup = new System.Windows.Forms.Label();
			this.tbRankingView = new Sport.UI.Controls.ThemeButton();
			this.panelRankingView = new System.Windows.Forms.Panel();
			this.tbGroupView = new Sport.UI.Controls.ThemeButton();
			this.tbTeamDown = new Sport.UI.Controls.ThemeButton();
			this.tbTeamUp = new Sport.UI.Controls.ThemeButton();
			this.tbWebsitePermanentChamp = new Sport.UI.Controls.ThemeButton();
			this.panelGroupEdit = new System.Windows.Forms.Panel();
			this.tbCancelGroup = new Sport.UI.Controls.ThemeButton();
			this.tbSaveGroup = new Sport.UI.Controls.ThemeButton();
			this.panelStructureEdit.SuspendLayout();
			this.gbTeam.SuspendLayout();
			this.gbGroup.SuspendLayout();
			this.gbPhase.SuspendLayout();
			this.panelEdit.SuspendLayout();
			this.panelGroupView.SuspendLayout();
			this.pnlRight.SuspendLayout();
			this.panelRankingView.SuspendLayout();
			this.panelGroupEdit.SuspendLayout();
			this.SuspendLayout();
			// 
			// phases
			// 
			this.phases.Appearance = Sport.UI.Controls.CaptionBarAppearance.Buttons;
			this.phases.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.phases.Dock = System.Windows.Forms.DockStyle.Top;
			this.phases.Location = new System.Drawing.Point(0, 216);
			this.phases.Name = "phases";
			this.phases.SelectedIndex = -1;
			this.phases.SelectedItem = null;
			this.phases.Size = new System.Drawing.Size(734, 19);
			this.phases.TabIndex = 8;
			this.phases.SelectionChanged += new System.EventHandler(this.phases_SelectionChanged);
			// 
			// panelStructureEdit
			// 
			this.panelStructureEdit.Controls.Add(this.gbTeam);
			this.panelStructureEdit.Controls.Add(this.gbGroup);
			this.panelStructureEdit.Controls.Add(this.gbPhase);
			this.panelStructureEdit.Controls.Add(this.tbCancelStructure);
			this.panelStructureEdit.Controls.Add(this.tbSaveStructure);
			this.panelStructureEdit.Dock = System.Windows.Forms.DockStyle.Top;
			this.panelStructureEdit.Location = new System.Drawing.Point(0, 120);
			this.panelStructureEdit.Name = "panelStructureEdit";
			this.panelStructureEdit.Size = new System.Drawing.Size(734, 96);
			this.panelStructureEdit.TabIndex = 28;
			this.panelStructureEdit.Visible = false;
			// 
			// gbTeam
			// 
			this.gbTeam.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.gbTeam.Controls.Add(this.tbMoveTeam);
			this.gbTeam.Controls.Add(this.tbDivideTeams);
			this.gbTeam.Controls.Add(this.tbRemoveTeam);
			this.gbTeam.Controls.Add(this.tbAddTeam);
			this.gbTeam.Controls.Add(this.tbTeamName);
			this.gbTeam.Location = new System.Drawing.Point(62, 8);
			this.gbTeam.Name = "gbTeam";
			this.gbTeam.Size = new System.Drawing.Size(224, 64);
			this.gbTeam.TabIndex = 34;
			this.gbTeam.TabStop = false;
			this.gbTeam.Text = "קבוצה";
			// 
			// tbMoveTeam
			// 
			this.tbMoveTeam.Alignment = System.Drawing.StringAlignment.Center;
			this.tbMoveTeam.AutoSize = true;
			this.tbMoveTeam.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbMoveTeam.Hue = 200F;
			this.tbMoveTeam.Image = ((System.Drawing.Image)(resources.GetObject("tbMoveTeam.Image")));
			this.tbMoveTeam.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbMoveTeam.ImageList = null;
			this.tbMoveTeam.ImageSize = new System.Drawing.Size(12, 12);
			this.tbMoveTeam.Location = new System.Drawing.Point(161, 40);
			this.tbMoveTeam.Name = "tbMoveTeam";
			this.tbMoveTeam.Saturation = 0.5F;
			this.tbMoveTeam.Size = new System.Drawing.Size(55, 17);
			this.tbMoveTeam.TabIndex = 39;
			this.tbMoveTeam.Text = "העבר";
			this.tbMoveTeam.Transparent = System.Drawing.Color.Black;
			this.tbMoveTeam.Click += new System.EventHandler(this.tbMoveTeam_Click);
			// 
			// tbDivideTeams
			// 
			this.tbDivideTeams.Alignment = System.Drawing.StringAlignment.Center;
			this.tbDivideTeams.AutoSize = true;
			this.tbDivideTeams.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbDivideTeams.Hue = 200F;
			this.tbDivideTeams.Image = ((System.Drawing.Image)(resources.GetObject("tbDivideTeams.Image")));
			this.tbDivideTeams.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbDivideTeams.ImageList = null;
			this.tbDivideTeams.ImageSize = new System.Drawing.Size(12, 12);
			this.tbDivideTeams.Location = new System.Drawing.Point(112, 40);
			this.tbDivideTeams.Name = "tbDivideTeams";
			this.tbDivideTeams.Saturation = 0.5F;
			this.tbDivideTeams.Size = new System.Drawing.Size(47, 17);
			this.tbDivideTeams.TabIndex = 36;
			this.tbDivideTeams.Text = "חלק";
			this.tbDivideTeams.Transparent = System.Drawing.Color.Black;
			this.tbDivideTeams.Click += new System.EventHandler(this.tbDivideTeams_Click);
			// 
			// tbRemoveTeam
			// 
			this.tbRemoveTeam.Alignment = System.Drawing.StringAlignment.Center;
			this.tbRemoveTeam.AutoSize = true;
			this.tbRemoveTeam.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbRemoveTeam.Hue = 0F;
			this.tbRemoveTeam.Image = ((System.Drawing.Image)(resources.GetObject("tbRemoveTeam.Image")));
			this.tbRemoveTeam.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbRemoveTeam.ImageList = null;
			this.tbRemoveTeam.ImageSize = new System.Drawing.Size(12, 12);
			this.tbRemoveTeam.Location = new System.Drawing.Point(60, 40);
			this.tbRemoveTeam.Name = "tbRemoveTeam";
			this.tbRemoveTeam.Saturation = 0.9F;
			this.tbRemoveTeam.Size = new System.Drawing.Size(49, 17);
			this.tbRemoveTeam.TabIndex = 34;
			this.tbRemoveTeam.Text = "מחק";
			this.tbRemoveTeam.Transparent = System.Drawing.Color.Black;
			this.tbRemoveTeam.Click += new System.EventHandler(this.tbRemoveTeam_Click);
			// 
			// tbAddTeam
			// 
			this.tbAddTeam.Alignment = System.Drawing.StringAlignment.Center;
			this.tbAddTeam.AutoSize = true;
			this.tbAddTeam.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbAddTeam.Hue = 220F;
			this.tbAddTeam.Image = ((System.Drawing.Image)(resources.GetObject("tbAddTeam.Image")));
			this.tbAddTeam.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbAddTeam.ImageList = null;
			this.tbAddTeam.ImageSize = new System.Drawing.Size(12, 12);
			this.tbAddTeam.Location = new System.Drawing.Point(8, 40);
			this.tbAddTeam.Name = "tbAddTeam";
			this.tbAddTeam.Saturation = 0.9F;
			this.tbAddTeam.Size = new System.Drawing.Size(50, 17);
			this.tbAddTeam.TabIndex = 33;
			this.tbAddTeam.Text = "חדש";
			this.tbAddTeam.Transparent = System.Drawing.Color.Black;
			this.tbAddTeam.Click += new System.EventHandler(this.tbAddTeam_Click);
			// 
			// tbTeamName
			// 
			this.tbTeamName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.tbTeamName.Location = new System.Drawing.Point(8, 16);
			this.tbTeamName.Name = "tbTeamName";
			this.tbTeamName.ReadOnly = true;
			this.tbTeamName.Size = new System.Drawing.Size(208, 20);
			this.tbTeamName.TabIndex = 32;
			this.tbTeamName.Text = "";
			// 
			// gbGroup
			// 
			this.gbGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.gbGroup.Controls.Add(this.tbMoveGroup);
			this.gbGroup.Controls.Add(this.tbGroupName);
			this.gbGroup.Controls.Add(this.tbRemoveGroup);
			this.gbGroup.Controls.Add(this.tbAddGroup);
			this.gbGroup.Location = new System.Drawing.Point(296, 8);
			this.gbGroup.Name = "gbGroup";
			this.gbGroup.Size = new System.Drawing.Size(176, 64);
			this.gbGroup.TabIndex = 33;
			this.gbGroup.TabStop = false;
			this.gbGroup.Text = "בית";
			// 
			// tbMoveGroup
			// 
			this.tbMoveGroup.Alignment = System.Drawing.StringAlignment.Center;
			this.tbMoveGroup.AutoSize = true;
			this.tbMoveGroup.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbMoveGroup.Hue = 200F;
			this.tbMoveGroup.Image = ((System.Drawing.Image)(resources.GetObject("tbMoveGroup.Image")));
			this.tbMoveGroup.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbMoveGroup.ImageList = null;
			this.tbMoveGroup.ImageSize = new System.Drawing.Size(12, 12);
			this.tbMoveGroup.Location = new System.Drawing.Point(113, 40);
			this.tbMoveGroup.Name = "tbMoveGroup";
			this.tbMoveGroup.Saturation = 0.5F;
			this.tbMoveGroup.Size = new System.Drawing.Size(55, 17);
			this.tbMoveGroup.TabIndex = 38;
			this.tbMoveGroup.Text = "העבר";
			this.tbMoveGroup.Transparent = System.Drawing.Color.Black;
			this.tbMoveGroup.Click += new System.EventHandler(this.tbMoveGroup_Click);
			// 
			// tbGroupName
			// 
			this.tbGroupName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.tbGroupName.Location = new System.Drawing.Point(8, 16);
			this.tbGroupName.Name = "tbGroupName";
			this.tbGroupName.Size = new System.Drawing.Size(160, 20);
			this.tbGroupName.TabIndex = 31;
			this.tbGroupName.Text = "";
			this.tbGroupName.TextChanged += new System.EventHandler(this.tbGroupName_TextChanged);
			// 
			// tbRemoveGroup
			// 
			this.tbRemoveGroup.Alignment = System.Drawing.StringAlignment.Center;
			this.tbRemoveGroup.AutoSize = true;
			this.tbRemoveGroup.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbRemoveGroup.Hue = 0F;
			this.tbRemoveGroup.Image = ((System.Drawing.Image)(resources.GetObject("tbRemoveGroup.Image")));
			this.tbRemoveGroup.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbRemoveGroup.ImageList = null;
			this.tbRemoveGroup.ImageSize = new System.Drawing.Size(12, 12);
			this.tbRemoveGroup.Location = new System.Drawing.Point(61, 40);
			this.tbRemoveGroup.Name = "tbRemoveGroup";
			this.tbRemoveGroup.Saturation = 0.9F;
			this.tbRemoveGroup.Size = new System.Drawing.Size(49, 17);
			this.tbRemoveGroup.TabIndex = 30;
			this.tbRemoveGroup.Text = "מחק";
			this.tbRemoveGroup.Transparent = System.Drawing.Color.Black;
			this.tbRemoveGroup.Click += new System.EventHandler(this.tbRemoveGroup_Click);
			// 
			// tbAddGroup
			// 
			this.tbAddGroup.Alignment = System.Drawing.StringAlignment.Center;
			this.tbAddGroup.AutoSize = true;
			this.tbAddGroup.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbAddGroup.Hue = 220F;
			this.tbAddGroup.Image = ((System.Drawing.Image)(resources.GetObject("tbAddGroup.Image")));
			this.tbAddGroup.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbAddGroup.ImageList = null;
			this.tbAddGroup.ImageSize = new System.Drawing.Size(12, 12);
			this.tbAddGroup.Location = new System.Drawing.Point(8, 40);
			this.tbAddGroup.Name = "tbAddGroup";
			this.tbAddGroup.Saturation = 0.9F;
			this.tbAddGroup.Size = new System.Drawing.Size(50, 17);
			this.tbAddGroup.TabIndex = 29;
			this.tbAddGroup.Text = "חדש";
			this.tbAddGroup.Transparent = System.Drawing.Color.Black;
			this.tbAddGroup.Click += new System.EventHandler(this.tbAddGroup_Click);
			// 
			// gbPhase
			// 
			this.gbPhase.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.gbPhase.Controls.Add(this.tbPhaseDefinition);
			this.gbPhase.Controls.Add(this.tbMovePhase);
			this.gbPhase.Controls.Add(this.tbPhaseName);
			this.gbPhase.Controls.Add(this.tbRemovePhase);
			this.gbPhase.Controls.Add(this.tbAddPhase);
			this.gbPhase.Location = new System.Drawing.Point(478, 8);
			this.gbPhase.Name = "gbPhase";
			this.gbPhase.Size = new System.Drawing.Size(246, 64);
			this.gbPhase.TabIndex = 32;
			this.gbPhase.TabStop = false;
			this.gbPhase.Text = "שלב";
			// 
			// tbMovePhase
			// 
			this.tbMovePhase.Alignment = System.Drawing.StringAlignment.Center;
			this.tbMovePhase.AutoSize = true;
			this.tbMovePhase.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbMovePhase.Hue = 200F;
			this.tbMovePhase.Image = ((System.Drawing.Image)(resources.GetObject("tbMovePhase.Image")));
			this.tbMovePhase.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbMovePhase.ImageList = null;
			this.tbMovePhase.ImageSize = new System.Drawing.Size(12, 12);
			this.tbMovePhase.Location = new System.Drawing.Point(113, 40);
			this.tbMovePhase.Name = "tbMovePhase";
			this.tbMovePhase.Saturation = 0.5F;
			this.tbMovePhase.Size = new System.Drawing.Size(55, 17);
			this.tbMovePhase.TabIndex = 37;
			this.tbMovePhase.Text = "העבר";
			this.tbMovePhase.Transparent = System.Drawing.Color.Black;
			this.tbMovePhase.Click += new System.EventHandler(this.tbMovePhase_Click);
			// 
			// tbPhaseName
			// 
			this.tbPhaseName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.tbPhaseName.Location = new System.Drawing.Point(8, 16);
			this.tbPhaseName.Name = "tbPhaseName";
			this.tbPhaseName.Size = new System.Drawing.Size(230, 20);
			this.tbPhaseName.TabIndex = 26;
			this.tbPhaseName.Text = "";
			this.tbPhaseName.TextChanged += new System.EventHandler(this.tbPhaseName_TextChanged);
			// 
			// tbRemovePhase
			// 
			this.tbRemovePhase.Alignment = System.Drawing.StringAlignment.Center;
			this.tbRemovePhase.AutoSize = true;
			this.tbRemovePhase.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbRemovePhase.Hue = 0F;
			this.tbRemovePhase.Image = ((System.Drawing.Image)(resources.GetObject("tbRemovePhase.Image")));
			this.tbRemovePhase.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbRemovePhase.ImageList = null;
			this.tbRemovePhase.ImageSize = new System.Drawing.Size(12, 12);
			this.tbRemovePhase.Location = new System.Drawing.Point(61, 40);
			this.tbRemovePhase.Name = "tbRemovePhase";
			this.tbRemovePhase.Saturation = 0.9F;
			this.tbRemovePhase.Size = new System.Drawing.Size(49, 17);
			this.tbRemovePhase.TabIndex = 25;
			this.tbRemovePhase.Text = "מחק";
			this.tbRemovePhase.Transparent = System.Drawing.Color.Black;
			this.tbRemovePhase.Click += new System.EventHandler(this.tbRemovePhase_Click);
			// 
			// tbAddPhase
			// 
			this.tbAddPhase.Alignment = System.Drawing.StringAlignment.Center;
			this.tbAddPhase.AutoSize = true;
			this.tbAddPhase.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbAddPhase.Hue = 220F;
			this.tbAddPhase.Image = ((System.Drawing.Image)(resources.GetObject("tbAddPhase.Image")));
			this.tbAddPhase.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbAddPhase.ImageList = null;
			this.tbAddPhase.ImageSize = new System.Drawing.Size(12, 12);
			this.tbAddPhase.Location = new System.Drawing.Point(8, 40);
			this.tbAddPhase.Name = "tbAddPhase";
			this.tbAddPhase.Saturation = 0.9F;
			this.tbAddPhase.Size = new System.Drawing.Size(50, 17);
			this.tbAddPhase.TabIndex = 24;
			this.tbAddPhase.Text = "חדש";
			this.tbAddPhase.Transparent = System.Drawing.Color.Black;
			this.tbAddPhase.Click += new System.EventHandler(this.tbAddPhase_Click);
			// 
			// tbCancelStructure
			// 
			this.tbCancelStructure.Alignment = System.Drawing.StringAlignment.Center;
			this.tbCancelStructure.AutoSize = true;
			this.tbCancelStructure.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbCancelStructure.Hue = 0F;
			this.tbCancelStructure.Image = ((System.Drawing.Image)(resources.GetObject("tbCancelStructure.Image")));
			this.tbCancelStructure.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbCancelStructure.ImageList = null;
			this.tbCancelStructure.ImageSize = new System.Drawing.Size(12, 12);
			this.tbCancelStructure.Location = new System.Drawing.Point(64, 77);
			this.tbCancelStructure.Name = "tbCancelStructure";
			this.tbCancelStructure.Saturation = 0.7F;
			this.tbCancelStructure.Size = new System.Drawing.Size(48, 17);
			this.tbCancelStructure.TabIndex = 25;
			this.tbCancelStructure.Text = "בטל";
			this.tbCancelStructure.Transparent = System.Drawing.Color.Black;
			this.tbCancelStructure.Click += new System.EventHandler(this.tbCancelStructure_Click);
			// 
			// tbSaveStructure
			// 
			this.tbSaveStructure.Alignment = System.Drawing.StringAlignment.Center;
			this.tbSaveStructure.AutoSize = true;
			this.tbSaveStructure.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbSaveStructure.Hue = 120F;
			this.tbSaveStructure.Image = ((System.Drawing.Image)(resources.GetObject("tbSaveStructure.Image")));
			this.tbSaveStructure.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbSaveStructure.ImageList = null;
			this.tbSaveStructure.ImageSize = new System.Drawing.Size(12, 12);
			this.tbSaveStructure.Location = new System.Drawing.Point(8, 77);
			this.tbSaveStructure.Name = "tbSaveStructure";
			this.tbSaveStructure.Saturation = 0.5F;
			this.tbSaveStructure.Size = new System.Drawing.Size(53, 17);
			this.tbSaveStructure.TabIndex = 26;
			this.tbSaveStructure.Text = "שמור";
			this.tbSaveStructure.Transparent = System.Drawing.Color.Black;
			this.tbSaveStructure.Click += new System.EventHandler(this.tbSaveStructure_Click);
			// 
			// panelEdit
			// 
			this.panelEdit.Controls.Add(this.tbPrint);
			this.panelEdit.Controls.Add(this.tbReports);
			this.panelEdit.Controls.Add(this.tbReferreReport);
			this.panelEdit.Controls.Add(this.tbRegisteredTeams);
			this.panelEdit.Controls.Add(this.tbLeagueSummary);
			this.panelEdit.Controls.Add(this.tbEditGroup);
			this.panelEdit.Controls.Add(this.tbPrevPhase);
			this.panelEdit.Controls.Add(this.labelPhase);
			this.panelEdit.Controls.Add(this.tbNextPhase);
			this.panelEdit.Controls.Add(this.tbEditStructure);
			this.panelEdit.Controls.Add(this.tbWebsitePermanentChamp);
			this.panelEdit.Dock = System.Windows.Forms.DockStyle.Top;
			this.panelEdit.Location = new System.Drawing.Point(0, 0);
			this.panelEdit.Name = "panelEdit";
			this.panelEdit.Size = new System.Drawing.Size(734, 24);
			this.panelEdit.TabIndex = 29;
			// 
			// tbPrint
			// 
			this.tbPrint.Alignment = System.Drawing.StringAlignment.Center;
			this.tbPrint.AutoSize = true;
			this.tbPrint.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbPrint.Hue = 215F;
			this.tbPrint.Image = ((System.Drawing.Image)(resources.GetObject("tbPrint.Image")));
			this.tbPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbPrint.ImageList = null;
			this.tbPrint.ImageSize = new System.Drawing.Size(12, 12);
			this.tbPrint.Location = new System.Drawing.Point(185, 5);
			this.tbPrint.Name = "tbPrint";
			this.tbPrint.Saturation = 0.8F;
			this.tbPrint.Size = new System.Drawing.Size(65, 17);
			this.tbPrint.TabIndex = 48;
			this.tbPrint.Text = "הדפסה";
			this.tbPrint.Transparent = System.Drawing.Color.Black;
			this.tbPrint.Click += new System.EventHandler(this.tbPrint_Click);
			// 
			// tbPrintCompetition
			// 
			this.tbPrintCompetition.Alignment = System.Drawing.StringAlignment.Center;
			this.tbPrintCompetition.AutoSize = true;
			this.tbPrintCompetition.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbPrintCompetition.Hue = 215F;
			this.tbPrintCompetition.Image = ((System.Drawing.Image)(resources.GetObject("tbPrint.Image")));
			this.tbPrintCompetition.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbPrintCompetition.ImageList = null;
			this.tbPrintCompetition.ImageSize = new System.Drawing.Size(12, 12);
			this.tbPrintCompetition.Location = new System.Drawing.Point(78, 4);
			this.tbPrintCompetition.Name = "tbPrintCompetition";
			this.tbPrintCompetition.Saturation = 0.8F;
			this.tbPrintCompetition.Size = new System.Drawing.Size(65, 17);
			this.tbPrintCompetition.TabIndex = 48;
			this.tbPrintCompetition.Text = "הדפסה";
			this.tbPrintCompetition.Transparent = System.Drawing.Color.Black;
			this.tbPrintCompetition.Visible = false;
			this.tbPrintCompetition.Click += new System.EventHandler(this.tbPrint_Click);
			// 
			// tbReports
			// 
			this.tbReports.Alignment = System.Drawing.StringAlignment.Center;
			this.tbReports.AutoSize = true;
			this.tbReports.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbReports.Hue = 215F;
			this.tbReports.Image = ((System.Drawing.Image)(resources.GetObject("tbReports.Image")));
			this.tbReports.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbReports.ImageList = null;
			this.tbReports.ImageSize = new System.Drawing.Size(12, 12);
			this.tbReports.Location = new System.Drawing.Point(185, 5);
			this.tbReports.Name = "tbReports";
			this.tbReports.Saturation = 0.8F;
			this.tbReports.Size = new System.Drawing.Size(63, 17);
			this.tbReports.TabIndex = 48;
			this.tbReports.Text = "דו\"חות";
			this.tbReports.Transparent = System.Drawing.Color.Black;
			this.tbReports.Visible = false;
			this.tbReports.Click += new System.EventHandler(this.tbReport_Click);
			// 
			// tbLeagueSummary
			// 
			this.tbLeagueSummary.Alignment = System.Drawing.StringAlignment.Center;
			this.tbLeagueSummary.AutoSize = true;
			this.tbLeagueSummary.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbLeagueSummary.Hue = 215F;
			this.tbLeagueSummary.Image = ((System.Drawing.Image)(resources.GetObject("tbReports.Image")));
			this.tbLeagueSummary.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbLeagueSummary.ImageList = null;
			this.tbLeagueSummary.ImageSize = new System.Drawing.Size(12, 12);
			this.tbLeagueSummary.Location = new System.Drawing.Point(290, 5);
			this.tbLeagueSummary.Name = "tbLeagueSummary";
			this.tbLeagueSummary.Saturation = 0.8F;
			this.tbLeagueSummary.Size = new System.Drawing.Size(63, 17);
			this.tbLeagueSummary.TabIndex = 48;
			this.tbLeagueSummary.Text = "סיכום ליגה";
			this.tbLeagueSummary.Transparent = System.Drawing.Color.Black;
			this.tbLeagueSummary.Visible = false;
			this.tbLeagueSummary.Click += new System.EventHandler(this.tbLeagueSummary_Click);
			// 
			// tbReferreReport
			// 
			this.tbReferreReport.Alignment = System.Drawing.StringAlignment.Center;
			this.tbReferreReport.AutoSize = true;
			this.tbReferreReport.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbReferreReport.Hue = 185F;
			this.tbReferreReport.Image = ((System.Drawing.Image)(resources.GetObject("tbReferreReport.Image")));
			this.tbReferreReport.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbReferreReport.ImageList = null;
			this.tbReferreReport.ImageSize = new System.Drawing.Size(12, 12);
			this.tbReferreReport.Location = new System.Drawing.Point(270, 5);
			this.tbReferreReport.Name = "tbReferreReport";
			this.tbReferreReport.Saturation = 0.8F;
			this.tbReferreReport.Size = new System.Drawing.Size(130, 17);
			this.tbReferreReport.TabIndex = 80;
			this.tbReferreReport.Text = "דו\"ח שיבוץ שופטים";
			this.tbReferreReport.Transparent = System.Drawing.Color.Black;
			this.tbReferreReport.Visible = false;
			// 
			// tbRegisteredTeams
			// 
			this.tbRegisteredTeams.Alignment = System.Drawing.StringAlignment.Center;
			this.tbRegisteredTeams.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.tbRegisteredTeams.AutoSize = true;
			this.tbRegisteredTeams.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbRegisteredTeams.Hue = 135F;
			this.tbRegisteredTeams.Image = null;
			this.tbRegisteredTeams.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.tbRegisteredTeams.ImageList = null;
			this.tbRegisteredTeams.ImageSize = new System.Drawing.Size(0, 0);
			this.tbRegisteredTeams.Location = new System.Drawing.Point(262, 5);
			this.tbRegisteredTeams.Name = "tbRegisteredTeams";
			this.tbRegisteredTeams.Saturation = 0.8F;
			this.tbRegisteredTeams.Size = new System.Drawing.Size(92, 17);
			this.tbRegisteredTeams.TabIndex = 81;
			this.tbRegisteredTeams.Text = "רשימת קבוצות";
			this.tbRegisteredTeams.Transparent = System.Drawing.Color.Black;
			this.tbRegisteredTeams.Click += new System.EventHandler(this.tbRegisteredTeams_Click);
			// 
			// tbEditGroup
			// 
			this.tbEditGroup.Alignment = System.Drawing.StringAlignment.Center;
			this.tbEditGroup.AutoSize = true;
			this.tbEditGroup.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbEditGroup.Hue = 70F;
			this.tbEditGroup.Image = ((System.Drawing.Image)(resources.GetObject("tbEditGroup.Image")));
			this.tbEditGroup.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbEditGroup.ImageList = null;
			this.tbEditGroup.ImageSize = new System.Drawing.Size(12, 12);
			this.tbEditGroup.Location = new System.Drawing.Point(96, 5);
			this.tbEditGroup.Name = "tbEditGroup";
			this.tbEditGroup.Saturation = 0.4F;
			this.tbEditGroup.Size = new System.Drawing.Size(83, 17);
			this.tbEditGroup.TabIndex = 33;
			this.tbEditGroup.Text = "ערוך מבנה";
			this.tbEditGroup.Transparent = System.Drawing.Color.Black;
			this.tbEditGroup.Click += new System.EventHandler(this.tbEditGroup_Click);
			// 
			// tbPrevPhase
			// 
			this.tbPrevPhase.Alignment = System.Drawing.StringAlignment.Center;
			this.tbPrevPhase.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.tbPrevPhase.AutoSize = true;
			this.tbPrevPhase.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbPrevPhase.Hue = 20F;
			this.tbPrevPhase.Image = ((System.Drawing.Image)(resources.GetObject("tbPrevPhase.Image")));
			this.tbPrevPhase.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbPrevPhase.ImageList = null;
			this.tbPrevPhase.ImageSize = new System.Drawing.Size(12, 12);
			this.tbPrevPhase.Location = new System.Drawing.Point(372, 5);
			this.tbPrevPhase.Name = "tbPrevPhase";
			this.tbPrevPhase.Saturation = 0.6F;
			this.tbPrevPhase.Size = new System.Drawing.Size(79, 17);
			this.tbPrevPhase.TabIndex = 32;
			this.tbPrevPhase.Text = "שלב קודם";
			this.tbPrevPhase.Transparent = System.Drawing.Color.Black;
			this.tbPrevPhase.Click += new System.EventHandler(this.tbPrevPhase_Click);
			// 
			// tbWebsitePermanentChamp
			// 
			this.tbWebsitePermanentChamp.Alignment = System.Drawing.StringAlignment.Center;
			this.tbWebsitePermanentChamp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.tbWebsitePermanentChamp.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbWebsitePermanentChamp.Hue = 230F;
			this.tbWebsitePermanentChamp.Image = null;
			this.tbWebsitePermanentChamp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.tbWebsitePermanentChamp.ImageList = null;
			this.tbWebsitePermanentChamp.ImageSize = new System.Drawing.Size(0, 0);
			this.tbWebsitePermanentChamp.Location = new System.Drawing.Point(60, 5);
			this.tbWebsitePermanentChamp.Name = "tbWebsitePermanentChamp";
			this.tbWebsitePermanentChamp.Saturation = 0.8F;
			this.tbWebsitePermanentChamp.Size = new System.Drawing.Size(195, 17);
			this.tbWebsitePermanentChamp.TabIndex = 82;
			this.tbWebsitePermanentChamp.Text = websitePermanentChampButtonTexts[(int)WebsitePermanentChampState.DoesNotExist];
			this.tbWebsitePermanentChamp.Transparent = System.Drawing.Color.Black;
			this.tbWebsitePermanentChamp.Click += new EventHandler(tbWebsitePermanentChamp_Click);
			// 
			// labelPhase
			// 
			this.labelPhase.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelPhase.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.labelPhase.Location = new System.Drawing.Point(535, 6);
			this.labelPhase.Name = "labelPhase";
			this.labelPhase.Size = new System.Drawing.Size(192, 16);
			this.labelPhase.TabIndex = 31;
			// 
			// tbNextPhase
			// 
			this.tbNextPhase.Alignment = System.Drawing.StringAlignment.Center;
			this.tbNextPhase.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.tbNextPhase.AutoSize = true;
			this.tbNextPhase.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbNextPhase.Hue = 20F;
			this.tbNextPhase.Image = ((System.Drawing.Image)(resources.GetObject("tbNextPhase.Image")));
			this.tbNextPhase.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbNextPhase.ImageList = null;
			this.tbNextPhase.ImageSize = new System.Drawing.Size(12, 12);
			this.tbNextPhase.Location = new System.Drawing.Point(455, 5);
			this.tbNextPhase.Name = "tbNextPhase";
			this.tbNextPhase.Saturation = 0.6F;
			this.tbNextPhase.Size = new System.Drawing.Size(76, 17);
			this.tbNextPhase.TabIndex = 30;
			this.tbNextPhase.Text = "שלב הבא";
			this.tbNextPhase.Transparent = System.Drawing.Color.Black;
			this.tbNextPhase.Click += new System.EventHandler(this.tbNextPhase_Click);
			// 
			// tbEditStructure
			// 
			this.tbEditStructure.Alignment = System.Drawing.StringAlignment.Center;
			this.tbEditStructure.AutoSize = true;
			this.tbEditStructure.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbEditStructure.Hue = 200F;
			this.tbEditStructure.Image = ((System.Drawing.Image)(resources.GetObject("tbEditStructure.Image")));
			this.tbEditStructure.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbEditStructure.ImageList = null;
			this.tbEditStructure.ImageSize = new System.Drawing.Size(12, 12);
			this.tbEditStructure.Location = new System.Drawing.Point(8, 5);
			this.tbEditStructure.Name = "tbEditStructure";
			this.tbEditStructure.Saturation = 0.5F;
			this.tbEditStructure.Size = new System.Drawing.Size(83, 17);
			this.tbEditStructure.TabIndex = 25;
			this.tbEditStructure.Text = "ערוך מבנה";
			this.tbEditStructure.Transparent = System.Drawing.Color.Black;
			this.tbEditStructure.Click += new System.EventHandler(this.tbEditStructure_Click);
			// 
			// tbSetResults
			// 
			this.tbSetResults.Alignment = System.Drawing.StringAlignment.Center;
			this.tbSetResults.AutoSize = true;
			this.tbSetResults.Enabled = false;
			this.tbSetResults.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbSetResults.Hue = 180F;
			this.tbSetResults.Image = ((System.Drawing.Image)(resources.GetObject("tbSetResults.Image")));
			this.tbSetResults.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbSetResults.ImageList = null;
			this.tbSetResults.ImageSize = new System.Drawing.Size(11, 11);
			this.tbSetResults.Location = new System.Drawing.Point(75, 4);
			this.tbSetResults.Name = "tbSetResults";
			this.tbSetResults.Saturation = 0.2F;
			this.tbSetResults.Size = new System.Drawing.Size(96, 17);
			this.tbSetResults.TabIndex = 45;
			this.tbSetResults.Text = "הכנס תוצאות";
			this.tbSetResults.Transparent = System.Drawing.Color.Black;
			this.tbSetResults.Click += new System.EventHandler(this.tbSetResults_Click);
			// 
			// panelGroupView
			// 
			this.panelGroupView.Controls.Add(this.tbCustom);
			this.panelGroupView.Controls.Add(this.tbOfflineEntities);
			this.panelGroupView.Controls.Add(this.pnlRight);
			this.panelGroupView.Controls.Add(this.tbRankingView);
			this.panelGroupView.Controls.Add(this.tbSetResults);
			this.panelGroupView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelGroupView.Location = new System.Drawing.Point(0, 235);
			this.panelGroupView.Name = "panelGroupView";
			this.panelGroupView.Size = new System.Drawing.Size(734, 231);
			this.panelGroupView.TabIndex = 0;
			// 
			// tbCustom
			// 
			this.tbCustom.Alignment = System.Drawing.StringAlignment.Center;
			this.tbCustom.AutoSize = true;
			this.tbCustom.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbCustom.Hue = 215F;
			this.tbCustom.Image = ((System.Drawing.Image)(resources.GetObject("tbCustom.Image")));
			this.tbCustom.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbCustom.ImageList = null;
			this.tbCustom.ImageSize = new System.Drawing.Size(12, 12);
			this.tbCustom.Location = new System.Drawing.Point(175, 4);
			this.tbCustom.Name = "tbCustom";
			this.tbCustom.Saturation = 0.8F;
			this.tbCustom.Size = new System.Drawing.Size(67, 17);
			this.tbCustom.TabIndex = 46;
			this.tbCustom.Text = "התאמה";
			this.tbCustom.Transparent = System.Drawing.Color.Black;
			this.tbCustom.Click += new System.EventHandler(this.tbCustom_Click);
			// 
			// tbOfflineEntities
			// 
			this.tbOfflineEntities.Alignment = System.Drawing.StringAlignment.Center;
			this.tbOfflineEntities.AutoSize = true;
			this.tbOfflineEntities.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbOfflineEntities.Hue = 55F;
			this.tbOfflineEntities.Image = null;
			this.tbOfflineEntities.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.tbOfflineEntities.ImageList = null;
			this.tbOfflineEntities.ImageSize = new System.Drawing.Size(0, 0);
			this.tbOfflineEntities.Location = new System.Drawing.Point(250, 4);
			this.tbOfflineEntities.Name = "tbOfflineEntities";
			this.tbOfflineEntities.Saturation = 0.8F;
			this.tbOfflineEntities.Size = new System.Drawing.Size(127, 17);
			this.tbOfflineEntities.TabIndex = 110;
			this.tbOfflineEntities.Text = "הוספת נתונים";
			this.tbOfflineEntities.Transparent = System.Drawing.Color.Black;
			this.tbOfflineEntities.Visible = false;
			this.tbOfflineEntities.Click += new System.EventHandler(this.tbOfflineEntities_Click);
			// 
			// pnlRight
			// 
			this.pnlRight.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.pnlRight.Controls.Add(this.tbImport);
			this.pnlRight.Controls.Add(this.tbExport);
			this.pnlRight.Controls.Add(this.labelGroup);
			this.pnlRight.Location = new System.Drawing.Point(214, 4);
			this.pnlRight.Name = "pnlRight";
			this.pnlRight.Size = new System.Drawing.Size(289, 17);
			this.pnlRight.TabIndex = 0;
			// 
			// tbImport
			// 
			this.tbImport.Alignment = System.Drawing.StringAlignment.Center;
			this.tbImport.Dock = System.Windows.Forms.DockStyle.Right;
			this.tbImport.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbImport.Hue = 255F;
			this.tbImport.Name = "tbImport";
			this.tbImport.Saturation = 0.8F;
			this.tbImport.Size = new System.Drawing.Size(109, 17);
			this.tbImport.TabIndex = 81;
			this.tbImport.Text = "ייבוא תוצאות";
			this.tbImport.Transparent = System.Drawing.Color.Black;
			this.tbImport.Visible = false;
			this.tbImport.Click += new System.EventHandler(this.tbImport_Click);
			// 
			// tbExport
			// 
			this.tbExport.Alignment = System.Drawing.StringAlignment.Center;
			this.tbExport.AutoSize = false;
			this.tbExport.Dock = System.Windows.Forms.DockStyle.Right;
			this.tbExport.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbExport.Hue = 15F;
			this.tbExport.Name = "tbExport";
			this.tbExport.Saturation = 0.8F;
			this.tbExport.Size = new System.Drawing.Size(50, 17);
			this.tbExport.TabIndex = 80;
			this.tbExport.Text = "ייצוא";
			this.tbExport.Transparent = System.Drawing.Color.Black;
			this.tbExport.Click += new System.EventHandler(this.tbExport_Click);
			// 
			// labelGroup
			// 
			this.labelGroup.Dock = System.Windows.Forms.DockStyle.Right;
			this.labelGroup.Name = "labelGroup";
			this.labelGroup.Size = new System.Drawing.Size(130, 17);
			this.labelGroup.TextAlign = ContentAlignment.MiddleLeft;
			this.labelGroup.TabIndex = 37;
			this.labelGroup.Text = "בית:";
			// 
			// tbRankingView
			// 
			this.tbRankingView.Alignment = System.Drawing.StringAlignment.Center;
			this.tbRankingView.AutoSize = true;
			this.tbRankingView.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbRankingView.Hue = 200F;
			this.tbRankingView.Image = ((System.Drawing.Image)(resources.GetObject("tbRankingView.Image")));
			this.tbRankingView.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbRankingView.ImageList = null;
			this.tbRankingView.ImageSize = new System.Drawing.Size(12, 12);
			this.tbRankingView.Location = new System.Drawing.Point(8, 4);
			this.tbRankingView.Name = "tbRankingView";
			this.tbRankingView.Saturation = 0.5F;
			this.tbRankingView.Size = new System.Drawing.Size(64, 17);
			this.tbRankingView.TabIndex = 36;
			this.tbRankingView.Text = "דירוגים";
			this.tbRankingView.Transparent = System.Drawing.Color.Black;
			this.tbRankingView.Click += new System.EventHandler(this.tbRankingView_Click);
			// 
			// panelRankingView
			// 
			this.panelRankingView.Controls.Add(this.tbGroupView);
			this.panelRankingView.Controls.Add(this.tbPrintCompetition);
			this.panelRankingView.Controls.Add(this.tbTeamDown);
			this.panelRankingView.Controls.Add(this.tbTeamUp);
			this.panelRankingView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelRankingView.Location = new System.Drawing.Point(0, 235);
			this.panelRankingView.Name = "panelRankingView";
			this.panelRankingView.Size = new System.Drawing.Size(734, 231);
			this.panelRankingView.TabIndex = 31;
			// 
			// tbGroupView
			// 
			this.tbGroupView.Alignment = System.Drawing.StringAlignment.Center;
			this.tbGroupView.AutoSize = true;
			this.tbGroupView.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbGroupView.Hue = 200F;
			this.tbGroupView.Image = ((System.Drawing.Image)(resources.GetObject("tbGroupView.Image")));
			this.tbGroupView.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbGroupView.ImageList = null;
			this.tbGroupView.ImageSize = new System.Drawing.Size(12, 12);
			this.tbGroupView.Location = new System.Drawing.Point(8, 4);
			this.tbGroupView.Name = "tbGroupView";
			this.tbGroupView.Saturation = 0.5F;
			this.tbGroupView.Size = new System.Drawing.Size(64, 17);
			this.tbGroupView.TabIndex = 38;
			this.tbGroupView.Text = "דירוגים";
			this.tbGroupView.Transparent = System.Drawing.Color.Black;
			this.tbGroupView.Click += new System.EventHandler(this.tbGroupView_Click);
			// 
			// tbTeamDown
			// 
			this.tbTeamDown.Alignment = System.Drawing.StringAlignment.Center;
			this.tbTeamDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.tbTeamDown.AutoSize = false;
			this.tbTeamDown.Enabled = false;
			this.tbTeamDown.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbTeamDown.Hue = 200F;
			this.tbTeamDown.Image = ((System.Drawing.Image)(resources.GetObject("tbTeamDown.Image")));
			this.tbTeamDown.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbTeamDown.ImageList = null;
			this.tbTeamDown.ImageSize = new System.Drawing.Size(12, 12);
			this.tbTeamDown.Location = new System.Drawing.Point(613, 4);
			this.tbTeamDown.Name = "tbTeamDown";
			this.tbTeamDown.Saturation = 0.5F;
			this.tbTeamDown.Size = new System.Drawing.Size(51, 17);
			this.tbTeamDown.TabIndex = 36;
			this.tbTeamDown.Text = "הורד";
			this.tbTeamDown.Transparent = System.Drawing.Color.Black;
			this.tbTeamDown.Click += new System.EventHandler(this.tbTeamDown_Click);
			// 
			// tbTeamUp
			// 
			this.tbTeamUp.Alignment = System.Drawing.StringAlignment.Center;
			this.tbTeamUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.tbTeamUp.AutoSize = false;
			this.tbTeamUp.Enabled = false;
			this.tbTeamUp.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbTeamUp.Hue = 200F;
			this.tbTeamUp.Image = ((System.Drawing.Image)(resources.GetObject("tbTeamUp.Image")));
			this.tbTeamUp.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbTeamUp.ImageList = null;
			this.tbTeamUp.ImageSize = new System.Drawing.Size(12, 12);
			this.tbTeamUp.Location = new System.Drawing.Point(671, 4);
			this.tbTeamUp.Name = "tbTeamUp";
			this.tbTeamUp.Saturation = 0.5F;
			this.tbTeamUp.Size = new System.Drawing.Size(51, 17);
			this.tbTeamUp.TabIndex = 37;
			this.tbTeamUp.Text = "עלה";
			this.tbTeamUp.Transparent = System.Drawing.Color.Black;
			this.tbTeamUp.Click += new System.EventHandler(this.tbTeamUp_Click);
			// 
			// panelGroupEdit
			// 
			this.panelGroupEdit.Controls.Add(this.tbCancelGroup);
			this.panelGroupEdit.Controls.Add(this.tbSaveGroup);
			this.panelGroupEdit.Dock = System.Windows.Forms.DockStyle.Top;
			this.panelGroupEdit.Location = new System.Drawing.Point(0, 24);
			this.panelGroupEdit.Name = "panelGroupEdit";
			this.panelGroupEdit.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.panelGroupEdit.Size = new System.Drawing.Size(734, 96);
			this.panelGroupEdit.TabIndex = 32;
			this.panelGroupEdit.Visible = false;
			// 
			// tbCancelGroup
			// 
			this.tbCancelGroup.Alignment = System.Drawing.StringAlignment.Center;
			this.tbCancelGroup.AutoSize = true;
			this.tbCancelGroup.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbCancelGroup.Hue = 0F;
			this.tbCancelGroup.Image = ((System.Drawing.Image)(resources.GetObject("tbCancelGroup.Image")));
			this.tbCancelGroup.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbCancelGroup.ImageList = null;
			this.tbCancelGroup.ImageSize = new System.Drawing.Size(12, 12);
			this.tbCancelGroup.Location = new System.Drawing.Point(64, 77);
			this.tbCancelGroup.Name = "tbCancelGroup";
			this.tbCancelGroup.Saturation = 0.7F;
			this.tbCancelGroup.Size = new System.Drawing.Size(48, 17);
			this.tbCancelGroup.TabIndex = 25;
			this.tbCancelGroup.Text = "בטל";
			this.tbCancelGroup.Transparent = System.Drawing.Color.Black;
			this.tbCancelGroup.Click += new System.EventHandler(this.tbCancelGroup_Click);
			// 
			// tbSaveGroup
			// 
			this.tbSaveGroup.Alignment = System.Drawing.StringAlignment.Center;
			this.tbSaveGroup.AutoSize = true;
			this.tbSaveGroup.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbSaveGroup.Hue = 120F;
			this.tbSaveGroup.Image = ((System.Drawing.Image)(resources.GetObject("tbSaveGroup.Image")));
			this.tbSaveGroup.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbSaveGroup.ImageList = null;
			this.tbSaveGroup.ImageSize = new System.Drawing.Size(12, 12);
			this.tbSaveGroup.Location = new System.Drawing.Point(8, 77);
			this.tbSaveGroup.Name = "tbSaveGroup";
			this.tbSaveGroup.Saturation = 0.5F;
			this.tbSaveGroup.Size = new System.Drawing.Size(53, 17);
			this.tbSaveGroup.TabIndex = 26;
			this.tbSaveGroup.Text = "שמור";
			this.tbSaveGroup.Transparent = System.Drawing.Color.Black;
			this.tbSaveGroup.Click += new System.EventHandler(this.tbSaveGroup_Click);
			// 
			// tbPhaseDefinition
			// 
			this.tbPhaseDefinition.Alignment = System.Drawing.StringAlignment.Center;
			this.tbPhaseDefinition.AutoSize = true;
			this.tbPhaseDefinition.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbPhaseDefinition.Hue = 70F;
			this.tbPhaseDefinition.Image = ((System.Drawing.Image)(resources.GetObject("tbPhaseDefinition.Image")));
			this.tbPhaseDefinition.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbPhaseDefinition.ImageList = null;
			this.tbPhaseDefinition.ImageSize = new System.Drawing.Size(12, 12);
			this.tbPhaseDefinition.Location = new System.Drawing.Point(172, 40);
			this.tbPhaseDefinition.Name = "tbPhaseDefinition";
			this.tbPhaseDefinition.Saturation = 0.5F;
			this.tbPhaseDefinition.Size = new System.Drawing.Size(66, 17);
			this.tbPhaseDefinition.TabIndex = 38;
			this.tbPhaseDefinition.Text = "הגדרות";
			this.tbPhaseDefinition.Transparent = System.Drawing.Color.Black;
			this.tbPhaseDefinition.Click += new System.EventHandler(this.tbPhaseDefinition_Click);
			// 
			// ChampionshipEditorView
			// 
			this.Controls.Add(this.panelRankingView);
			this.Controls.Add(this.panelGroupView);
			this.Controls.Add(this.phases);
			this.Controls.Add(this.panelStructureEdit);
			this.Controls.Add(this.panelGroupEdit);
			this.Controls.Add(this.panelEdit);
			this.Name = "ChampionshipEditorView";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.Size = new System.Drawing.Size(734, 466);
			this.panelStructureEdit.ResumeLayout(false);
			this.gbTeam.ResumeLayout(false);
			this.gbGroup.ResumeLayout(false);
			this.gbPhase.ResumeLayout(false);
			this.panelEdit.ResumeLayout(false);
			this.panelGroupView.ResumeLayout(false);
			this.pnlRight.ResumeLayout(false);
			this.panelRankingView.ResumeLayout(false);
			this.panelGroupEdit.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		void tbWebsitePermanentChamp_Click(object sender, EventArgs e)
		{
			int champCategoryId = GetChampCategoryId();
			if (champCategoryId < 0)
			{
				Sport.UI.MessageBox.Error("לא ניתן להגדיר, לא מוגדרת קטגוריית אליפות", "שגיאה");
				return;
			}

			using (Sport.UI.Dialogs.GenericEditDialog dialog = new Sport.UI.Dialogs.GenericEditDialog("אליפות קבועה באתר"))
			{
				dialog.Size = new System.Drawing.Size(400, 300);
				string DELETE_TEXT = "[מחק]";
				int existingChampsAmount;
				DataServices.WebsitePermanentChampionship existingChampData = GetExistingWebsitePermanentChamp(champCategoryId, out existingChampsAmount);
				string existingTitle = (existingChampData == null) ? "" : existingChampData.Title;
				List<object> indexItems = new List<object>();
				if (existingChampData != null)
					indexItems.Add(DELETE_TEXT);
				for (int i = 1; i <= existingChampsAmount; i++)
					indexItems.Add(i);
				indexItems.Add(existingChampsAmount + 1);
				object selectedItem = (existingChampData == null) ? indexItems.Last() : existingChampData.Index;
				dialog.Items.Add("כותרת", Sport.UI.Controls.GenericItemType.Text, existingTitle);
				dialog.Items.Add("מיקום", Sport.UI.Controls.GenericItemType.Selection, selectedItem, indexItems.ToArray());
				bool reloadState = false;
				if (dialog.ShowDialog(this) == DialogResult.OK)
				{
					object rawIndex = dialog.Items[1].Value;
					if ((rawIndex + "").Length > 0)
					{
						if (rawIndex.ToString() == DELETE_TEXT)
						{
							if (Sport.UI.MessageBox.Ask("האם לבטל הגדרת אליפות זו כקבועה באתר?", false))
							{
								using (DataServices.DataService service = new DataServices.DataService())
								{
									service.DeleteWebsitePermanentChampionship(champCategoryId);
									reloadState = true;
									Sport.UI.MessageBox.Success("אליפות קבועה בוטלה", "שמירת נתונים");
								}
							}
						}
						else
						{
							string title = dialog.Items[0].Value + "";
							if (title.Length > 0)
							{
								int index = (int)rawIndex;
								using (DataServices.DataService service = new DataServices.DataService())
								{
									if (existingChampData == null)
									{
										service.AddWebsitePermanentChampionship(new DataServices.WebsitePermanentChampionship
										{
											ChampionshipCategoryId = champCategoryId, 
											Title = title, 
											Index = index
										});
									}
									else
									{
										existingChampData.Title = title;
										existingChampData.Index = index;
										service.UpdateWebsitePermanentChampionship(existingChampData);
									}
									reloadState = true;
									Sport.UI.MessageBox.Success("נתונים נשמרו בהצלחה", "שמירת נתונים");
								}
							}
							else
							{
								Sport.UI.MessageBox.Warn("יש להזין כותרת עבור האליפות הקבועה", "שמירת נתונים");
							}
						}
					}
				}

				if (reloadState)
					SetWebsitePermanentChampButtonText();
			}
		}

		#endregion

		private Sport.Championships.Championship championship;
		public Sport.Championships.Championship Championship
		{
			get { return championship; }
		}
		
		public string OfflineResultsFile
		{
			get
			{
				string strFolderPath = Sport.Core.Session.GetSeasonCache(false);
				string strFilePath=strFolderPath+
					System.IO.Path.DirectorySeparatorChar+
					"Championship_"+this.Championship.CategoryID+".xml";
				return strFilePath;
			}
		}

		#region Selection
		
		private int _phase;
		public Sport.Championships.Phase Phase
		{
			get
			{
				if (_phase < 0)
					return null;
				if (championship.Phases == null)
					return null;
				return championship.Phases[_phase];
			}
			set 
			{
				int phase = value == null ? -1 : value.Index;
				if (_phase != phase)
				{
					_phase = phase;
					SetGroupGridPhase();
					phases.SelectedIndex = phase;
					OnPhaseChange();
				}
			}
		}
		
		private Sport.Championships.Phase _lastPhase=null;
		private bool _cancelWarnings=false;
		private void SetGroupGridPhase()
		{
			if (_lastPhase == Phase)
				return;
			
			phaseGroupGridPanel.Phase = Phase;
			_lastPhase = Phase;
			
			if ((Phase == null)||(_cancelWarnings))
				return;
			
			Sport.Championships.Team[] teams=Phase.GetTeams();
			string strUnconfirmedTeams="";
			string strInvalidShirtRangeTeams="";
			foreach (Sport.Championships.Team team in teams)
			{
				if (team.TeamEntity != null)
				{
					string strTeamName=team.TeamEntity.TeamName();
					if (!team.IsConfirmed())
						strUnconfirmedTeams += strTeamName+"\n";
					if (!team.IsValidShirtRange())
						strInvalidShirtRangeTeams += strTeamName+"\n";
				}
			}
			if ((strUnconfirmedTeams.Length > 0)||(strInvalidShirtRangeTeams.Length > 0))
			{
				string strErrorMsg="";
				if (strUnconfirmedTeams.Length > 0)
					strErrorMsg += "הקבוצות הבאות אינן מאושרות:\n"+strUnconfirmedTeams+"\n";
				if (strInvalidShirtRangeTeams.Length > 0)
					strErrorMsg += "טווח מספרי חזה של הקבוצות הבאים אינו מוגדר:\n"+strInvalidShirtRangeTeams;
				Sport.UI.MessageBox.Error(strErrorMsg, "בניית אליפות");
			}
		}
		
		private int _group;
		public Sport.Championships.Group Group
		{
			get
			{
				if (_group < 0)
					return null;
				Sport.Championships.Phase phase = this.Phase;
				if (phase == null)
					return null;
				if (phase.Groups == null)
					return null;
				return phase.Groups[_group];
			}
			set { phaseGroupGridPanel.Group = value; }
		}

		private int _team;
		public Sport.Championships.Team Team
		{
			get
			{
				if (_team < 0)
					return null;
				Sport.Championships.Group group = this.Group;
				if (group == null)
					return null;
				if (group.Teams == null)
					return null;
				return group.Teams[_team];
			}
			set { phaseGroupGridPanel.Team = value; }
		}

		private void phases_SelectionChanged(object sender, System.EventArgs e)
		{
			int phase = phases.SelectedIndex;
			if (_phase != phase)
			{
				_phase = phase;
				OnPhaseChange();
			}
		}

		private void PhaseGroupGridPanelSelectionChanged(object sender, EventArgs e)
		{
			int group = phaseGroupGridPanel.Group == null ? -1 : phaseGroupGridPanel.Group.Index;
			int team = phaseGroupGridPanel.Team == null ? -1 : phaseGroupGridPanel.Team.Index;
			if (_group != group)
			{
				_group = group;
				_team = team;
				OnGroupChange();
				OnTeamChange();
			}
			else if (_team != team)
			{
				_team = team;
				OnTeamChange();
			}
		}

		private bool selecting = false;

		protected virtual void OnPhaseChange()
		{
			bool s = selecting;
			selecting = true;
			SetGroupGridPhase();
			rankingGridPanel.Phase = Phase;
			
			tbAddGroup.Enabled = _phase != -1;
			string strPhaseName = null;
			if (_phase >= 0 && Phase != null)
				strPhaseName = Phase.Name;
			tbPhaseName.Text = strPhaseName;
			tbRemovePhase.Enabled = _phase != -1;
			tbPhaseDefinition.Enabled = _phase != -1;
			tbPhaseName.Enabled = _phase != -1;
			bool blnMoreThanOne = false;
			if (championship != null && championship.Phases != null)
				blnMoreThanOne = (championship.Phases.Count > 1);
			tbMovePhase.Enabled = (_phase != -1 && blnMoreThanOne);
			selecting = s;
		}

		protected virtual void OnGroupChange()
		{
			bool s = selecting;
			selecting = true;
			string strGroupName="";
			if (Group != null)
				strGroupName = Group.Name;
			tbAddTeam.Enabled = _group != -1;
			tbGroupName.Text = _group == -1 ? null : strGroupName;
			tbGroupName.Enabled = _group != -1;
			tbRemoveGroup.Enabled = _group != -1;
			tbMoveGroup.Enabled = _group != -1 && Phase.Groups.Count > 1;

			labelGroup.Text = _group == -1 ? "בית:" : "בית: " + strGroupName;
			selecting = s;
		}

		protected virtual void OnTeamChange()
		{
			bool s = selecting;
			selecting = true;
			tbTeamName.Text = (_team == -1)?(null):((Team != null)?Team.Name:null);
			tbDivideTeams.Enabled = _phase != -1;
			tbRemoveTeam.Enabled = _team != -1;
			bool blnMoreThanOne = false;
			if (Group != null && Group.Teams != null)
				blnMoreThanOne = (Group.Teams.Count > 1);
			tbMoveTeam.Enabled = (_team != -1 && blnMoreThanOne);
			selecting = s;
		}
		#endregion

		#region View Operations

		public override void Open()
		{
			string ccid = State["championshipcategory"];

			Title = "אליפות";
			SetWebsitePermanentChampButtonText();

			if (ccid != null)
			{
				int champId = Int32.Parse(ccid);
				
				Sport.UI.Dialogs.WaitForm.SetProgress(0);
				championship = Sport.Championships.Championship.GetChampionship(champId, true);
				Sport.UI.Dialogs.WaitForm.SetProgress(80);
				
				if (championship != null)
				{
					rankingGridPanel.Championship = championship;

					Title = "אליפות - " + championship.Name;
					labelPhase.Text = "שלב נוכחי: " + 
						(championship.CurrentPhase == null ? "" : championship.CurrentPhase.Name);

					ResetPhases();

					if (championship.Phases.Count == 0)
						EditStructure();
					else
						Phase = championship.Phases[0];

					OnPhaseChange();
					OnGroupChange();
					OnTeamChange();
				}
			}
			Sport.UI.Dialogs.WaitForm.SetProgress(100);
			
			/*
			if ((championship != null)&&(championship.Phases != null)&&
				(championship.Phases.Count > 0))
			{
				bool blnCompetitionChamp=
					(championship.SportType == Sport.Types.SportType.Competition);
				Sport.UI.Dialogs.WaitForm.ShowWait("אנא המתן, יוצר מאגר נתונים מקומי...");
				Sport.UI.Dialogs.WaitForm.SetProgress(0);
				double diffPhase=(((double) 100)/((double) championship.Phases.Count));
				double currentProgress=0;
				foreach (Sport.Championships.Phase phase in championship.Phases)
				{
					if ((phase.Groups != null)&&(phase.Groups.Count > 0))
					{
						double diffGroup = (diffPhase/((double) phase.Groups.Count));
						foreach (Sport.Championships.Group group in phase.Groups)
						{
							if ((group.Teams != null)&&(group.Teams.Count > 0))
							{
								double diffTeam = (diffGroup/((double) group.Teams.Count));
								double diffPlayers = (0.9*diffTeam);
								diffTeam = diffTeam-diffPlayers;
								foreach (Sport.Championships.Team team in group.Teams)
								{
									Sport.Entities.Team dummyTeam=team.TeamEntity;
									if (blnCompetitionChamp)
									{
										currentProgress += diffTeam;
										Sport.UI.Dialogs.WaitForm.SetProgress((int) currentProgress);
										Sport.Entities.Player[] dummyPlayers=
										dummyTeam.GetPlayers();
										if ((dummyPlayers != null)&&(dummyPlayers.Length > 0))
										{
											double diffStudent=(diffPlayers/((double) dummyPlayers.Length));
											foreach (Sport.Entities.Player player in dummyPlayers)
											{
												Sport.Entities.Student dummyStudent=player.Student;
												currentProgress += diffStudent;
												Sport.UI.Dialogs.WaitForm.SetProgress((int) currentProgress);
											}
										}
									}
									else
									{
										currentProgress += diffPlayers+diffTeam;
										Sport.UI.Dialogs.WaitForm.SetProgress((int) currentProgress);
									}
								} //end loop over teams
							} //end if got any teams
						} //end loop over groups
					} //end if got any groups
				} //end loop over phases
				Sport.UI.Dialogs.WaitForm.SetProgress(100);
				Sport.UI.Dialogs.WaitForm.HideWait();
			} //end if got any phases
			*/
			
			if (Sport.Core.Session.Connected)
			{
				tbImport.Visible = System.IO.File.Exists(this.OfflineResultsFile);
			}
			else
			{
				if (championship.SportType == Sport.Types.SportType.Competition)
					tbOfflineEntities.Visible = true;
			}
			
			base.Open ();
		}

		public override bool Closing()
		{
			//got anything?
			if (championship == null)
				return true;
			
			if (championship.Editing)
			{
				Sport.UI.MessageBoxReturnType mrt = 
					Sport.UI.MessageBox.AskYesNoCancel("האליפות נמצאת בעריכה, האם לשמור?", Sport.UI.MessageBoxReturnType.Yes);
				if (mrt == Sport.UI.MessageBoxReturnType.Cancel)
					return false;
				if (mrt == Sport.UI.MessageBoxReturnType.No)
				{
					championship.Cancel();
				}
				else
				{
					string strMessage="";
					try
					{
						strMessage = championship.Save(!Sport.Core.Session.Connected);
					}
					catch (Exception ex)
					{
						strMessage = "שגיאה כללית בעת שמירת אליפות:\n"+ex.Message+"\n"+ex.StackTrace;
					}

					if (strMessage.Length > 0)
					{
						Sport.UI.MessageBox.Error(strMessage, "בניית אליפות");
						return true;
					}
				}
			}
			return base.Closing ();
		}

		public override void Close()
		{
			//SelectPhase(-1);
			phases.Items.Clear();
			base.Close ();
		}

		#endregion
		
		public override void Deactivate()
		{
			base.Deactivate ();
		}

		private void ResetDisplay()
		{
			SuspendLayout();
			panelEdit.Visible = !championship.Editing;
			panelStructureEdit.Visible = _editMode == ChampionshipEditMode.Structure;
			panelGroupEdit.Visible = _editMode == ChampionshipEditMode.Group;
			if (championship.Editing)
				SetGroupDisplay();
			string title = "אליפות - " + championship.Name;
			if (_editMode == ChampionshipEditMode.Structure)
				title += " - עריכת מבנה";
			else if (_editMode == ChampionshipEditMode.Group)
				title += " - עריכת בית";
			Title = title;
			ResumeLayout();
		}

		private void tbSaveGroup_Click(object sender, System.EventArgs e)
		{
			string strError=SaveGroup();
			if (strError.Length > 0)
				Sport.UI.MessageBox.Error(strError, "בניית אליפות");
		}

		private void tbCancelGroup_Click(object sender, System.EventArgs e)
		{
			Cancel();
		}

		private void SetGroupDisplay()
		{
			panelGroupView.Visible = true;
			panelRankingView.Visible = false;

			if (this is CompetitionChampionshipEditorView)
				this.tbPrintCompetition.Visible = false;
		}

		private void SetRankingDisplay()
		{
			panelRankingView.Visible = true;
			panelGroupView.Visible = false;

			if (this is CompetitionChampionshipEditorView)
				this.tbPrintCompetition.Visible = true;
		}

		protected virtual void OnCurrentPhaseChange()
		{
			tbNextPhase.Enabled = championship.CurrentPhaseIndex < championship.Phases.Count - 1;
			tbPrevPhase.Enabled = championship.CurrentPhaseIndex > -1;
			if (championship.CurrentPhase == null)
				labelPhase.Text = "שלב נוכחי:";
			else
				labelPhase.Text = "שלב נוכחי: " + championship.CurrentPhase.Name;
			phaseGroupGridPanel.Refresh();
		}

		private void tbNextPhase_Click(object sender, System.EventArgs e)
		{
			if (!Sport.Core.Session.Connected)
			{
				Sport.UI.MessageBox.Error("לא ניתן לעבור שלב במצב לא מקוון", "עריכת אליפות");
				return;
			}
			
			championship.Edit();
			string strError=championship.NextPhase();
			if (strError.Length > 0)
			{
				Sport.UI.MessageBox.Error("שגיאה בעת מעבר שלב:\n"+strError, "עריכת אליפות");
				Cancel();
				return;
			}
			string strMsg=championship.Save();
			if (strMsg.Length > 0)
			{
				Sport.UI.MessageBox.Error("שגיאה בעת שמירת נתונים:\n"+strMsg, 
					"בניית אליפות");
				Cancel();
			}
			else
			{
				OnCurrentPhaseChange();
			}
		}

		private void tbPrevPhase_Click(object sender, System.EventArgs e)
		{
			if (!Sport.Core.Session.Connected)
			{
				Sport.UI.MessageBox.Error("לא ניתן לעבור שלב במצב לא מקוון", "עריכת אליפות");
				return;
			}
			
			Sport.Championships.Phase phase = championship.CurrentPhase;
			if (phase != null)
			{
				if (phase.HasResults())
				{
					if (!Sport.UI.MessageBox.Ask("פעולה זאת תגרום למחיקה תוצאות המשחקים בשלב, האם להמשיך?",
						System.Windows.Forms.MessageBoxIcon.Warning, false))
						return ;
				}

				championship.Edit();
				championship.PreviousPhase();
				string strMsg=championship.Save();
				if (strMsg.Length > 0)
				{
					Sport.UI.MessageBox.Error("שגיאה בעת שמירת נתונים:\n"+strMsg, 
						"בניית אליפות");
					Cancel();
				}
				else
				{
					OnCurrentPhaseChange();
				}
			}
		}

		protected virtual void Customize()
		{
		}

		private void tbCustom_Click(object sender, System.EventArgs e)
		{
			Customize();
		}
		
		private void tbExport_Click(object sender, System.EventArgs e)
		{
			ExportChampionship();
		}
		
		private void tbImport_Click(object sender, System.EventArgs e)
		{
			ImportResults();
		}
		
		private string CheckRules()
		{
			string result="";
			if (this.Championship.SportType == Sport.Types.SportType.Competition)
			{
				Sport.Championships.CompetitionChampionship championship=
					(Sport.Championships.CompetitionChampionship) this.Championship;
				ArrayList arrSportFields=new ArrayList();
				foreach (Sport.Championships.CompetitionPhase phase in championship.Phases)
				{
					foreach (Sport.Championships.CompetitionGroup group in phase.Groups)
					{
						foreach (Sport.Championships.Competition competition in group.Competitions)
						{
							Sport.Entities.SportField curSportField=competition.SportField;
							if (arrSportFields.IndexOf(curSportField) < 0)
								arrSportFields.Add(curSportField);
						}
					}
				}
				
				string strMissingResultType="";
				string strMissingScoreTable="";
				Sport.Rulesets.Rules.GeneralSportTypeData objGeneralRule = 
					(Sport.Rulesets.Rules.GeneralSportTypeData)
					championship.ChampionshipCategory.GetRule(typeof(
					Sport.Rulesets.Rules.GeneralSportTypeData));
				bool blnScoreIsRank=false;
				if (objGeneralRule != null)
					blnScoreIsRank = objGeneralRule.ScoreIsRank;
				foreach (Sport.Entities.SportField sportField in arrSportFields)
				{
					string strCurName=sportField.Name;
					object objResultType = championship.ChampionshipCategory.GetRule(
						typeof(Sport.Rulesets.Rules.ResultType), sportField);
					object objScoreTable = championship.ChampionshipCategory.GetRule(
						typeof(Sport.Rulesets.Rules.ScoreTable), sportField);
					if (objResultType == null)
						strMissingResultType += strCurName+"\n";
					if (objScoreTable == null)
						strMissingScoreTable += strCurName+"\n";
				}
				
				if (strMissingResultType.Length > 0)
					result += "לא מוגדר חוק 'סוג תוצאה' עבור המקצועות הבאים:\n"+strMissingResultType;
				
				if ((blnScoreIsRank == false)&&(strMissingScoreTable.Length > 0))
				{
					if (result.Length > 0)
						result += "\n";
					result += "לא מוגדר חוק 'ניקוד תוצאות' עבור המקצועות הבאים:\n"+strMissingScoreTable;
				}
			}
			return result;
		}
		
		protected virtual void SetResults()
		{
		}

		private void tbSetResults_Click(object sender, EventArgs e)
		{
			SetResults();
		}

		private void tbRankingView_Click(object sender, System.EventArgs e)
		{
			SetRankingDisplay();
		}

		#region Printing
		private void tbPrint_Click(object sender, System.EventArgs e)
		{
			System.Drawing.Printing.PrinterSettings ps;
			Sport.UI.Dialogs.PrintSettingsForm settingsForm;
			if (!Sport.UI.Helpers.GetPrinterSettings(out ps, out settingsForm))
				return;
			if (settingsForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				Sport.Documents.Document document = null;
				if (panelGroupView.Visible)
					document = CreateGroupDocument(ps);
				else if (panelRankingView.Visible || this is CompetitionChampionshipEditorView)
					document = CreateRankingDocument(ps);
				if (document != null)
				{
					if (settingsForm.ShowPreview)
					{
						Sport.UI.Dialogs.PrintForm printForm = new Sport.UI.Dialogs.PrintForm(document, ps);
			
						if (!printForm.Canceled)
							printForm.ShowDialog();

						printForm.Dispose();
					}
					else
					{
						System.Drawing.Printing.PrintDocument pd = document.CreatePrintDocument(ps);
						pd.PrintController = new PrintControllerWithPageForm(pd.PrintController, 0);
						pd.Print();
					}
				}
			}
		}
		
		protected virtual void ReportsButtonClicked()
		{
		}

		protected virtual void LeagueSummaryButtonClicked()
		{
		}
		
		private void tbReport_Click(object sender, System.EventArgs e)
		{
			ReportsButtonClicked();
		}

		private void tbLeagueSummary_Click(object sender, System.EventArgs e)
		{
			LeagueSummaryButtonClicked();
		}
		
		protected virtual Sport.Documents.Document CreateGroupDocument(System.Drawing.Printing.PrinterSettings settings)
		{
			return null;
		}

		protected virtual Sport.Documents.Document CreateRankingDocument(System.Drawing.Printing.PrinterSettings settings)
		{
			return null;
		}

		#endregion

		#region Editing

		protected virtual Sport.Championships.Phase CreatePhase(string name)
		{
			return null;
		}

		protected virtual Sport.Championships.Group CreateGroup(string name)
		{
			return null;
		}

		protected virtual Sport.Championships.Team CreateTeam(ref int position)
		{
			return null;
		}

		public enum ChampionshipEditMode
		{
			None,
			Structure,
			Group
		};

		private ChampionshipEditMode _editMode;
		public ChampionshipEditMode EditMode
		{
			get { return _editMode; }
		}

		protected virtual void OnEditModeChange()
		{
		}

		#region Structure
		
		private void EditStructure()
		{
			if (CanEditStructure() && championship != null)
			{
				championship.Edit();
				_editMode = ChampionshipEditMode.Structure;
				ResetDisplay();
				OnEditModeChange();
			}
		}

		protected virtual bool CanEditStructure()
		{
			return true;
		}

		private void SaveStructure()
		{
			_cancelWarnings = true;
			bool blnExceptionReported = false;
			
			Sport.UI.Dialogs.WaitForm.ShowWait("שומר אליפות '" + championship.Name + "'...");
			string strError = string.Empty;
			try
			{
				strError = championship.Save();
			}
			catch (Exception e)
			{
				strError = "שגיאה כללית בעת שמירת המבנה:\n" + e.Message;
				Sport.Data.AdvancedTools.ReportExcpetion(e);
				blnExceptionReported = true;
			}
			Sport.UI.Dialogs.WaitForm.HideWait();

			if (strError.Length == 0)
			{
				_editMode = ChampionshipEditMode.None;
				ResetDisplay();
				OnEditModeChange();
			}
			else
			{
				Sport.UI.MessageBox.Error("שמירת מבנה אליפות נכשלה. פרטים טכניים: " + "\n" + strError, "שמירת מבנה אליפות");
				if (!blnExceptionReported)
					Sport.Data.AdvancedTools.ReportExcpetion(new Exception("error while saving structure: " + strError));
			}
			
			_cancelWarnings = false;
		}

		#endregion

		#region Group

		private void EditGroup()
		{
			if (CanEditGroup())
			{
				championship.Edit();
				_editMode = ChampionshipEditMode.Group;
				ResetDisplay();
				OnEditModeChange();
			}
		}
		
		protected virtual bool CanEditGroup()
		{
			return true;
		}

		protected virtual void BeforeSaveGroup()
		{
			
		}
		
		protected virtual void AfterSaveGroup()
		{
			
		}
		
		private string SaveGroup()
		{
			_cancelWarnings = true;
			
			BeforeSaveGroup();
			
			Sport.UI.Dialogs.WaitForm.ShowWait("שומר אליפות '" + championship.Name + "'...");
			string saveResult="";
			try
			{
				saveResult = championship.Save(!Sport.Core.Session.Connected);
			}
			catch (Exception e)
			{
				saveResult = "שגיאה כללית בעת שמירה:\n"+e.Message+"\n"+e.StackTrace;
			}
			Sport.UI.Dialogs.WaitForm.HideWait();

			_editMode = ChampionshipEditMode.None;
			
			ResetDisplay();
			
			OnEditModeChange();
			_cancelWarnings = false;
			
			AfterSaveGroup();
			
			return saveResult;
		}

		#endregion

		private void Cancel()
		{
			_cancelWarnings = true;
			if (championship != null)
			{
				Sport.UI.Dialogs.WaitForm.ShowWait("טוען אליפות '" + championship.Name + "'...");

				try
				{
					championship.Cancel();
				}
				catch (Exception ex)
				{
					Sport.UI.MessageBox.Warn("אזהרה: אירעה שגיאה פנימית בעת ביטול השינויים\n" + ex.Message, "ביטול שינויים");
				}

				ResetPhases();
				
				int phase=-1;
				int group=-1;
				int team=-1;
				
				if (championship.Phases != null)
					phase = _phase >= championship.Phases.Count ? -1 : _phase;
				if ((Phase != null)&&(Phase.Groups != null))
					group = _phase == -1 || _group >= Phase.Groups.Count ? -1 : _group;
				if ((Group != null)&&(Group.Teams != null))
					team = _group == -1 || _team >= Group.Teams.Count ? -1 : _team;
				
				if (championship.Phases != null)
					Phase = (phase == -1)?null:championship.Phases[phase];
				else
					Phase = null;
				if ((Phase != null)&&(Phase.Groups != null))
					Group = (group == -1)?null:Phase.Groups[group];
				else
					Group = null;
				if ((Group != null)&&(Group.Teams != null))
					Team = (team == -1)?null:Group.Teams[team];
				else
					Team = null;

				Sport.UI.Dialogs.WaitForm.HideWait();

				_editMode = ChampionshipEditMode.None;
				ResetDisplay();

				OnEditModeChange();
			}
			_cancelWarnings = false;
		}

		#endregion
		
		private void tbRegisteredTeams_Click(object sender, EventArgs e)
		{
			//got phase?
			if (this.Phase == null)
				return;
			
			//get championship category:
			Sport.Entities.ChampionshipCategory champCategory=
				this.Phase.Championship.ChampionshipCategory;
			
			//got anything?
			if (champCategory == null)
				return;
			
			//initialize dialog:
			RegisteredTeamsForm objDialog=new RegisteredTeamsForm(champCategory);
			
			//done.
			objDialog.ShowDialog();
		}
		
		private void ExportChampionship()
		{
			//can export?
			if (this.Championship.SportType != Sport.Types.SportType.Competition)
			{
				Sport.UI.MessageBox.Error("כרגע אפשר לייצא רק תוצאות אליפות מסוג תחרות", 
					"ייצוא תוצאות");
				return;
			}
			
			//get championship:
			Sport.Championships.Championship championship=this.Championship;
			
			//got anything?
			if (championship == null)
				return;
			
			//editing championship?
			if (championship.Editing)
				return;
			
			//maybe one of the groups?
			foreach (Sport.Championships.Phase phase in championship.Phases)
			{
				foreach (Sport.Championships.Group group in phase.Groups)
				{
					//editing?
					if (group.Editing)
						return;
				}
			}
			
			//connected?
			if (!Sport.Core.Session.Connected)
			{
				Sport.UI.MessageBox.Error("לא ניתן ליצא במצב לא מקוון", "ייצוא אליפות");
				return;
			}
			
			//valid phase?
			if (this.Phase == null)
			{
				Sport.UI.MessageBox.Error("לא מוגדר שלב נוכחי", 
					"ייצוא תוצאות");
				return;
			}
			
			//phase started?
			/*
			if (this.Phase.Status == Sport.Championships.Status.Planned)
			{
				Sport.UI.MessageBox.Error("שלב נוכחי לא התחיל, לא ניתן להכניס תוצאות", 
					"ייצוא תוצאות");
				return;
			}
			*/
			
			//check rules
			string strErrorMsg=this.CheckRules();
			if (strErrorMsg.Length > 0)
			{
				Sport.UI.MessageBox.Error("לא ניתן לייצא אליפות:\n"+strErrorMsg, 
					"ייצוא אליפות");
				return;
			}
			
			//put in edit mode:
			this.Championship.Edit();
			
			//this may take a while...
			Sport.UI.Dialogs.WaitForm.ShowWait("שומר אליפות לדיסק...", true);
			
			//disconnect session:
			Sport.Core.Session.Connected = false;
			
			//try to save:
			string result="";
			try
			{
				result = this.Championship.Save();
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("failed to export championship "+this.Championship.ChampionshipCategory.Id+": "+ex.Message+"\n"+ex.StackTrace);
				result = "שגיאה כללית:\n"+ex.Message+"\n"+ex.StackTrace;
				this.Cancel();
			}
			finally
			{
				Sport.Core.Session.Connected = true;
				Sport.UI.Dialogs.WaitForm.HideWait();
			}
			
			//success?
			if (result.Length == 0)
				Sport.UI.MessageBox.Success("אליפות נשמרה בהצלחה לדיסק", "ייצוא אליפות");
			else
				Sport.UI.MessageBox.Error("כישלון בשמירת אליפות:\n"+result, "ייצוא אליפות");
			
			//store last version:
			Sport.Core.Configuration.WriteString(
				"General", "LastVersion", Sport.Core.Data.CurrentVersion.ToString());
			
			//store last season:
			Sport.Core.Configuration.WriteString(
				"LastSeason", "ID", Sport.Core.Session.Season.ToString());
			Sport.Core.Configuration.WriteString(
				"LastSeason", "Name", Sport.Entities.Season.Type.Lookup(
				Sport.Core.Session.Season).Name);
			
			//store last user:
			Sport.Core.Configuration.WriteString(
				"LastUser", "ID", Core.UserManager.CurrentUser.Id.ToString());
			Sport.Core.Configuration.WriteString(
				"LastUser", "Username", Core.UserManager.CurrentUser.Username);
			Sport.Core.Configuration.WriteString(
				"LastUser", "Password", Core.UserManager.CurrentUser.UserPassword);
			
			//store last region:
			Sport.Core.Configuration.WriteString(
				"LastRegion", "ID", Core.UserManager.CurrentUser.UserRegion.ToString());
			Sport.Core.Configuration.WriteString(
				"LastRegion", "Name", 
				new Sport.Entities.Region(Core.UserManager.CurrentUser.UserRegion).Name);
		}
		
		protected virtual int ImportResults()
		{
			//can import?
			if (this.Championship.SportType != Sport.Types.SportType.Competition)
			{
				Sport.UI.MessageBox.Error("כרגע אפשר לייבא רק תוצאות אליפות מסוג תחרות", 
					"ייבוא תוצאות");
				return 0;
			}
			
			//build file path:
			string strFilePath=this.OfflineResultsFile;
			
			//exists?
			if (!System.IO.File.Exists(strFilePath))
			{
				Sport.UI.MessageBox.Error("קובץ תוצאות לא נמצא: "+strFilePath, 
					"ייבוא תוצאות");
				return 0;
			}

			//get phases:
			string strError="";
			Sport.Championships.SportServices.Phase[] phases=
				Sport.Championships.Championship.GetOfflinePhases(
				strFilePath, ref strError);
			
			//got anything?
			if (phases == null)
			{
				Sport.UI.MessageBox.Error("שגיאה בעת קריאת קובץ נתונים:\n"+strError, 
					"ייבוא  תוצאות אליפות");
				return 0;
			}
			
			//this might take a while..
			Sport.UI.Dialogs.WaitForm.ShowWait("מייבא תוצאות אנא המתן...", true);
			
			//initialize tables:
			Hashtable tblCompetitorScores=new Hashtable();
			Hashtable tblCompetitorResults=new Hashtable();
			
			//get championship:
			Sport.Championships.CompetitionChampionship championship=
				(Sport.Championships.CompetitionChampionship) this.Championship;
			
			//edit:
			championship.Edit();
			
			//read score and results: (also add new competitors)
			Sport.Championships.Championship.ReadScoresAndResults(phases, 
				ref tblCompetitorScores, ref tblCompetitorResults, ref championship);
			
			//iterate through the original phases, look for changes.
			Hashtable tblNewResults=new Hashtable();
			ArrayList arrCompetitorsToRemove=new ArrayList();
			int alertCount=0;
			foreach (Sport.Championships.CompetitionPhase phase in championship.Phases)
			{
				foreach (Sport.Championships.CompetitionGroup group in phase.Groups)
				{
					foreach (Sport.Championships.Competition competition in
						group.Competitions)
					{
						foreach (Sport.Championships.Competitor competitor in
							competition.Competitors)
						{
							//get unique key
							string key=Sport.Championships.Championship.GetUniqueKey(
								phase, group, competition, competitor);
							
							//got new result?
							if (tblCompetitorResults[key] == null)
							{
								//need to remove...
								if (competitor.Result <= 0)
								{
									arrCompetitorsToRemove.Add(competitor);
									continue;
								}
							}
							
							//get result:
							int result=Sport.Common.Tools.CIntDef(
								tblCompetitorResults[key], -1);
							
							//valid?
							if (result <= 0)
								continue;
							
							//same result?
							if (result == competitor.Result)
								continue;
							
							//add to table:
							tblNewResults[competitor] = result;
							
							//need to alert
							if (competitor.Result > 0)
								alertCount++;
						} //end loop over competitors
					} //end loop over competitions
				} //end loop over groups
			} //end loop over phases
			
			//need to ask user to confirm?
			if (alertCount > 0)
			{
				Sport.UI.Dialogs.WaitForm.HideWait();
				string strMsg="אזהרה: באליפות זו כבר קיימות תוצאות.\n"+
					"פעולת הייבוא תדרוס תוצאות אלו בתוצאות חדשות.\n"+
					"האם להמשיך?";
				if (!Sport.UI.MessageBox.Ask(strMsg, MessageBoxIcon.Warning, false))
				{
					championship.Cancel();
					return 0;
				}
				Sport.UI.Dialogs.WaitForm.ShowWait("שומר נתונים אנא המתן...", true);
			}
			
			//need to confirm removing competitors?
			if (arrCompetitorsToRemove.Count > 0)
			{
				Sport.UI.Dialogs.WaitForm.HideWait();
				string strMsg="אזהרה: במהלך העבודה הלא מקוונת הוסרו מתמודדים.\n"+
					"פעולת הייבוא תסיר מתמודדים אלו מהאליפות.\n"+
					"האם להמשיך?";
				if (!Sport.UI.MessageBox.Ask(strMsg, MessageBoxIcon.Warning, false))
				{
					championship.Cancel();
					return 0;
				}
				Sport.UI.Dialogs.WaitForm.ShowWait("שומר נתונים אנא המתן...", true);
			}
			
			//remove competitors:
			foreach (Sport.Championships.Competitor competitor in arrCompetitorsToRemove)
				competitor.Competition.Competitors.Remove(competitor);
			
			//build list of competitions for whom to set results.
			Hashtable tblCompetitions=new Hashtable();
			foreach (Sport.Championships.Competitor competitor in tblNewResults.Keys)
			{
				//get competition:
				Sport.Championships.Competition competition=competitor.Competition;
				
				//create item if does not exist yet
				if (tblCompetitions[competition] == null)
					tblCompetitions[competition] = new ArrayList();
				
				//get shirt number and result:
				int shirtNumber=competitor.PlayerNumber;
				int result=(int) tblNewResults[competitor];
				
				//create result object:
				Sport.Championships.CompetitorResult objResult=
					new Sport.Championships.CompetitorResult(shirtNumber, result);
				
				//add to list:
				(tblCompetitions[competition] as ArrayList).Add(objResult);
			}
			
			//iterate over competitions and set results for each:
			strError = string.Empty;
			foreach (Sport.Championships.Competition competition in tblCompetitions.Keys)
			{
				strError = competition.SetResults(-1, (Sport.Championships.CompetitorResult[])
					(tblCompetitions[competition] as ArrayList).ToArray(
					typeof(Sport.Championships.CompetitorResult)));
				if (strError.Length > 0)
					Sport.UI.MessageBox.Warn("שגיאה בעת שמירת תוצאות עבור התחרות '" + competition.Name + "': " + strError, "ייבוא תוצאות");
			}
			
			//done.
			strError = championship.Save();
			Sport.UI.Dialogs.WaitForm.HideWait();
			if (strError.Length > 0)
				Sport.UI.MessageBox.Warn("שגיאה בעת שמירת האליפות: " + strError, "ייבוא תוצאות");
			if (tblNewResults.Count == 0)
			{
				Sport.UI.MessageBox.Warn("לא נמצאו תוצאות חדשות לייבא", "ייבוא תוצאות");
			}
			else
			{
				Sport.UI.MessageBox.Success("תוצאות "+tblNewResults.Count+" מתמודדים עודכנו בהצלחה", 
					"ייבוא תוצאות");
				
			}
			int categoryID=this.Championship.CategoryID;
			System.IO.File.Delete(strFilePath);
			System.IO.File.Delete(strFilePath.Replace("_"+categoryID, 
				categoryID.ToString()));
			tbImport.Visible = false;
			return tblNewResults.Count;
		} //end function ImportResults
		
		private void tbOfflineEntities_Click(object sender, EventArgs e)
		{
			if (this.Championship == null)
				return;
			Forms.OfflineEntitiesForm objDialog = 
				new Forms.OfflineEntitiesForm(this.Championship);
			objDialog.ShowDialog();
			phaseGroupGridPanel.Rebuild();
			phaseGroupGridPanel.Refresh();
		}

		private int GetChampCategoryId()
		{
			if (this.Championship != null && this.Championship.ChampionshipCategory != null)
				return this.Championship.ChampionshipCategory.Id;

			if (State["championshipcategory"] != null)
				return Int32.Parse(State["championshipcategory"]);

			return -1;
		}

		private DataServices.WebsitePermanentChampionship GetExistingWebsitePermanentChamp(int champCategoryId, out int totalCount)
		{
			totalCount = 0;
			DataServices.WebsitePermanentChampionship champ = null;
			using (DataServices.DataService service = new DataServices.DataService())
			{
				List<DataServices.WebsitePermanentChampionship> existingChamps = service.GetWebsitePermanentChampionships().ToList();
				totalCount = existingChamps.Count;
				champ = existingChamps.Find(c => c.ChampionshipCategoryId == champCategoryId);
			}
			return champ;
		}

		private DataServices.WebsitePermanentChampionship GetExistingWebsitePermanentChamp(int champCategoryId)
		{
			int dummy;
			return GetExistingWebsitePermanentChamp(champCategoryId, out dummy);
		}

		private void SetWebsitePermanentChampButtonText()
		{
			int champCategoryId = GetChampCategoryId();
			if (champCategoryId < 0)
				return;

			WebsitePermanentChampState state = GetExistingWebsitePermanentChamp(champCategoryId) == null ?
				WebsitePermanentChampState.DoesNotExist : WebsitePermanentChampState.Exists;
			tbWebsitePermanentChamp.Text = websitePermanentChampButtonTexts[(int)state];
		}

	} //end class ChampionshipEditorView
}