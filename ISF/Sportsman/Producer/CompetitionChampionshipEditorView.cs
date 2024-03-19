using System;
using System.Linq;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using Sport.Data;
using Sport.UI;
using Sportsman.Documents;
using System.Collections.Generic;

namespace Sportsman.Producer
{
	[Sport.UI.ViewPermissions(Sport.Types.Permission.Championships)]
	public class CompetitionChampionshipEditorView : ChampionshipEditorView
	{
		CompetitionResultForm _activeResultForm = null;
		List<CompetitionResultForm> resultForms = new List<CompetitionResultForm>();
		public bool GotOpenResultForm { get { return resultForms.Count > 0; } }

		private Dictionary<int, Sport.Championships.Competition> _preEditCompetitions = new Dictionary<int, Sport.Championships.Competition>();
		private Dictionary<int, Sport.Championships.Competitor> _preEditCompetitors = new Dictionary<int, Sport.Championships.Competitor>();

		private bool CompareEntities(EntityBase e1, EntityBase e2)
		{
			if (e1 == null && e2 != null)
				return true;
			if (e1 != null && e2 == null)
				return true;
			if (e1 != null && e2 != null && e1.Id != e2.Id)
				return true;
			return false;
		}

		private bool AreDifferent(Sport.Championships.Competition serverCompetition, Sport.Championships.Competition localCompetition)
		{
			if (serverCompetition.Time != localCompetition.Time)
				return true;
			if (CompareEntities(serverCompetition.Facility, localCompetition.Facility))
				return true;
			if (CompareEntities(serverCompetition.Court, localCompetition.Court))
				return true;
			return false;
		}

		private bool SynchronizeCompetition(Sport.Championships.Competition serverCompetition, Sport.Championships.Competition localCompetition)
		{
			if (AreDifferent(serverCompetition, localCompetition))
			{
				localCompetition.Set(serverCompetition.Time, serverCompetition.Facility, serverCompetition.Court, true);
				return true;
			}
			return false;
		}

		private void SynchronizeCompetitions(Sport.Championships.CompetitionGroup serverGroup, List<object> changedObjects)
		{
			Dictionary<int, Sport.Championships.Competition> serverCompetitionsMapping = serverGroup.MapCompetitions();
			Dictionary<int, Sport.Championships.Competition> localCompetitionsMapping = this.Group.MapCompetitions();
			Dictionary<int, Sport.Common.Tools.MappingDiffState> competitionsDiff = Sport.Common.Tools.GetMappingDiff<Sport.Championships.Competition>(serverCompetitionsMapping, localCompetitionsMapping);
			Sport.Championships.Competition curCompetition;
			competitionsDiff.Keys.ToList().ForEach(sportFieldId =>
			{
				curCompetition = null;
				switch (competitionsDiff[sportFieldId])
				{
					case Sport.Common.Tools.MappingDiffState.OnlyFirst:
						//exists on server but not locally, need to add.
						curCompetition = serverCompetitionsMapping[sportFieldId];
						Sport.Championships.Competition competition = new Sport.Championships.Competition(curCompetition.SportField);
						if (!this.Championship.Editing)
							this.Championship.Edit();
						this.Group.Competitions.Add(competition);
						break;
					case Sport.Common.Tools.MappingDiffState.OnlySecond:
						//exists locally, but not on server, need to delete.
						curCompetition = localCompetitionsMapping[sportFieldId];
						if (!this.Championship.Editing)
							this.Championship.Edit();
						this.Group.Competitions.Remove(curCompetition);
						if (this.Competition != null && this.Competition.Index == curCompetition.Index)
							this.Competition = null;
						break;
				}
				if (curCompetition != null)
					changedObjects.Add(curCompetition);
			});
			if (changedObjects.Count > 0)
				this.Championship.CancelEdit();

			int differentCompetitions = 0;
			competitionsDiff.Keys.ToList().FindAll(k => competitionsDiff[k] == Sport.Common.Tools.MappingDiffState.Both).ForEach(sportFieldId =>
			{
				if (SynchronizeCompetition(serverCompetitionsMapping[sportFieldId], localCompetitionsMapping[sportFieldId]))
				{
					differentCompetitions++;
				}
			});
			if (differentCompetitions > 0)
				this.competitionsGridPanel.Refresh();
		}

		private bool AreDifferent(Sport.Championships.Competitor serverCompetitor, Sport.Championships.Competitor localCompetitor)
		{
			if (serverCompetitor.Result != localCompetitor.Result)
				return true;
			return false;
		}

		private bool SynchronizeCompetitor(Sport.Championships.Competitor serverCompetitor, Sport.Championships.Competitor localCompetitor)
		{
			if (AreDifferent(serverCompetitor, localCompetitor))
			{
				this.Competition.SetResults(localCompetitor.Heat, new Sport.Championships.CompetitorResult[] { 
					new Sport.Championships.CompetitorResult(localCompetitor.PlayerNumber, serverCompetitor.Result) }, false);
				return true;
			}
			return false;
		}

		private void SynchronizeCompetitors(Sport.Championships.Competition serverCompetition, List<object> changedObjects)
		{
			Dictionary<int, Sport.Championships.Competitor> serverCompetitorsMapping = serverCompetition.MapCompetitors();
			Dictionary<int, Sport.Championships.Competitor> localCompetitorsMapping = this.Competition.MapCompetitors();
			Dictionary<int, Sport.Common.Tools.MappingDiffState> competitorsDiff = Sport.Common.Tools.GetMappingDiff<Sport.Championships.Competitor>(serverCompetitorsMapping, localCompetitorsMapping);
			Sport.Championships.Competitor curCompetitor;
			competitorsDiff.Keys.ToList().ForEach(playerNumber =>
			{
				curCompetitor = null;
				switch (competitorsDiff[playerNumber])
				{
					case Sport.Common.Tools.MappingDiffState.OnlyFirst:
						//exists on server but not locally, need to add.
						curCompetitor = serverCompetitorsMapping[playerNumber];
						Sport.Championships.Competitor competitor = new Sport.Championships.Competitor(playerNumber);
						competitor.Heat = curCompetitor.Heat;
						this.Competition.Competitors.Add(competitor);
						this.Competition.SetResults(curCompetitor.Heat, new Sport.Championships.CompetitorResult[] { 
												new Sport.Championships.CompetitorResult(playerNumber, curCompetitor.Result) }, false);
						break;
					case Sport.Common.Tools.MappingDiffState.OnlySecond:
						//exists locally, but not on server, need to delete.
						curCompetitor = localCompetitorsMapping[playerNumber];
						this.Competition.Competitors.Remove(curCompetitor);
						break;
				}
				if (curCompetitor != null)
					changedObjects.Add(curCompetitor);
			});

			int differentCompetitors = 0;
			competitorsDiff.Keys.ToList().FindAll(k => competitorsDiff[k] == Sport.Common.Tools.MappingDiffState.Both).ForEach(playerNumber =>
			{
				if (SynchronizeCompetitor(serverCompetitorsMapping[playerNumber], localCompetitorsMapping[playerNumber]))
				{
					differentCompetitors++;
				}
			});
			if (differentCompetitors > 0)
				this.competitorsGridPanel.Refresh();
		}

		public override bool Synchronize(out object data)
		{
			data = null;
			if (this.Championship == null || this.Championship.Editing || this.Championship.ChampionshipCategory == null)
				return false;

			int champCategoryId = this.Championship.ChampionshipCategory.Id;
			Sport.UI.Dialogs.WaitForm.Suppress();
			Sport.Championships.Championship championship = Sport.Championships.Championship.GetChampionship(champCategoryId, true);
			Sport.UI.Dialogs.WaitForm.UnSuppress();
			if (Sport.Championships.Championship.GotDifferentStructure(championship, this.Championship))
			{
				if (Sport.UI.MessageBox.Ask("מבנה האליפות עבר שינוי על ידי משתמש אחר, האם  לטעון  אליפות מחדש?", "סנכרון נתונים", MessageBoxIcon.Exclamation, false))
				{
					this.State["championshipcategory"] = champCategoryId.ToString();
					Sport.UI.Dialogs.WaitForm.Suppress();
					base.Open();
					Sport.UI.Dialogs.WaitForm.UnSuppress();
					return false;
				}
			}

			if (this.Phase == null || this.Group == null)
				return false;
			
			Sport.Championships.CompetitionPhase phase = championship.Phases.OfType<Sport.Championships.CompetitionPhase>().ToList().FirstOrDefault(p => p.Index == this.Phase.Index);
			if (phase == null)
				return false;
			
			Sport.Championships.CompetitionGroup group = phase.Groups.OfType<Sport.Championships.CompetitionGroup>().ToList().FirstOrDefault(g => g.Index == this.Group.Index);
			if (group == null)
				return false;

			List<object> changedObjects = new List<object>();
			SynchronizeCompetitions(group, changedObjects);
			if (this.Competition != null)
			{
				Sport.Championships.Competition competition = group.Competitions.OfType<Sport.Championships.Competition>().ToList().FirstOrDefault(c => c.Index == this.Competition.Index);
				if (competition != null)
					SynchronizeCompetitors(competition, changedObjects);
			}

			if (changedObjects.Count > 0)
			{
				data = changedObjects.ToList();
				return true;
			}
			return false;
		}

		public override void OnSuccess(object data)
		{
			List<object> changedObjects = data as List<object>;
			if (changedObjects != null && changedObjects.Count > 0)
			{
				bool competitorsChanged = changedObjects.OfType<Sport.Championships.Competitor>().Count() > 0;
				bool competitionsChanged = changedObjects.OfType<Sport.Championships.Competition>().Count() > 0;
				if (competitionsChanged)
				{
					competitionsGridPanel.Rebuild();
				}
				if (competitorsChanged && this.Competition != null)
				{
					this.Competition.CalculateCompetitorsPosition();
					this.competitorsGridPanel.Rebuild();
				}
				Sportsman.Context.SetStatusBar(Forms.MainForm.StatusBarPanels.Error, "סנכרון נתונים  עם שרת בוצע בהצלחה");
			}
		}

		public override void OnError(string errorMessage)
		{
			Sportsman.Context.SetStatusBar(Forms.MainForm.StatusBarPanels.Error, "שגיאה בעת סנכרון נתונים");
		}

		#region Edit Panels

		#region Group Panel

		private Sport.UI.Controls.ThemeButton tbDividePlayers;

		private void tbDividePlayers_Click(object sender, System.EventArgs e)
		{
			if (Group != null)
			{
				if (this.GotOpenResultForm)
				{
					Sport.UI.MessageBox.Error("לא ניתן לחלק מתמודדים כאשר חלון הכנסת תוצאות פתוח", "חלוקת מתמודדים");
					ShowResultForm();
					return;
				}

				CompetitorsDivisionForm cdf = new CompetitorsDivisionForm(Group);

				if (cdf.ShowDialog() == DialogResult.OK)
				{
					competitorsGridPanel.Rebuild();
				}
			}
		}

		#region Competition

		private Sport.UI.Dialogs.GenericEditDialog sportFieldsDialog = null;

		private void SportFieldTypeChanged(object sender, EventArgs e)
		{
			Sport.Entities.SportFieldType sportFieldType =
				sportFieldsDialog.Items[0].Value as Sport.Entities.SportFieldType;
			if (sportFieldType == null)
			{
				sportFieldsDialog.Items[1].Values = null;
			}
			else
			{
				sportFieldsDialog.Items[1].Values = sportFieldType.GetSportFields();
			}
		}

		private bool SelectSportField(ref Sport.Entities.SportField sportField)
		{
			if (sportFieldsDialog == null)
			{
				sportFieldsDialog = new Sport.UI.Dialogs.GenericEditDialog("בחר מקצוע");
				sportFieldsDialog.Items.Add("סוג מקצוע:", Sport.UI.Controls.GenericItemType.Selection);
				sportFieldsDialog.Items.Add("מקצוע:", Sport.UI.Controls.GenericItemType.Selection);
				sportFieldsDialog.Items[0].ValueChanged += new EventHandler(SportFieldTypeChanged);
			}

			sportFieldsDialog.Items[0].Values = Championship.ChampionshipCategory.Championship.Sport.GetSportFieldTypes();

			if (sportField == null)
			{
				sportFieldsDialog.Items[0].Value = null;
				sportFieldsDialog.Items[1].Value = null;
			}
			else
			{
				sportFieldsDialog.Items[0].Value = sportField.SportFieldType;
				sportFieldsDialog.Items[1].Values = sportField.SportFieldType.GetSportFields();
				sportFieldsDialog.Items[1].Value = sportField;
			}

			if (sportFieldsDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				sportField = sportFieldsDialog.Items[1].Value as Sport.Entities.SportField;
				return sportField != null;
			}

			return false;
		}


		private System.Windows.Forms.GroupBox gbCompetition;
		private Sport.UI.Controls.ThemeButton tbCompetitionDown;
		private Sport.UI.Controls.ThemeButton tbCompetitionUp;
		private Sport.UI.Controls.ButtonBox bbCompetition;
		private Sport.UI.Controls.ThemeButton tbRemoveCompetition;
		private Sport.UI.Controls.ThemeButton tbAddCompetition;

		private void bbCompetition_ValueSelect(object sender, System.EventArgs e)
		{
			if (_competition != -1)
			{
				Sport.Entities.SportField sportField = Competition.SportField;

				if (SelectSportField(ref sportField))
				{
					Competition.SportField = sportField;

					bbCompetition.Value = sportField;
					competitionsGridPanel.Refresh();
				}
			}
		}

		private void tbCompetitionUp_Click(object sender, System.EventArgs e)
		{
			if (_competition > 0)
			{
				Sport.Championships.Competition competition = Competition;

				Sport.Championships.CompetitionGroup group = Group;
				group.Competitions.RemoveAt(_competition);
				group.Competitions.Insert(_competition - 1, competition);
				_competition--;
				competitionsGridPanel.Rebuild();
				OnCompetitionChange();
				OnHeatChange();
				OnCompetitorChange();
			}
		}

		private void tbCompetitionDown_Click(object sender, System.EventArgs e)
		{
			Sport.Championships.Competition competition = Competition;
			if (competition != null && _competition < Group.Competitions.Count - 1)
			{
				Sport.Championships.CompetitionGroup group = Group;
				group.Competitions.RemoveAt(_competition);
				group.Competitions.Insert(_competition + 1, competition);
				_competition++;
				competitionsGridPanel.Rebuild();
				OnCompetitionChange();
				OnHeatChange();
				OnCompetitorChange();
			}
		}

		private void tbRemoveCompetition_Click(object sender, System.EventArgs e)
		{
			if (!Sport.Core.Session.Connected)
				return;

			if (Competition != null)
			{
				if (!Sport.UI.MessageBox.Ask("האם למחוק את התחרות '"
					+ Competition.ToString() + "'?", false))
					return;

				int index = Competition.Index;
				if (index > 0)
					Competition = Group.Competitions[index - 1];

				Group.Competitions.RemoveAt(index);
				competitionsGridPanel.Rebuild();
			}
		}

		private void tbAddCompetition_Click(object sender, System.EventArgs e)
		{
			if (!Sport.Core.Session.Connected)
				return;

			if (Group != null)
			{
				Sport.Entities.SportField sportField = null;

				if (SelectSportField(ref sportField))
				{
					Sport.Championships.Competition competition = new Sport.Championships.Competition(sportField);

					Group.Competitions.Add(competition);
					competitionsGridPanel.Rebuild();

					Competition = competition;
				}
			}
		}

		#endregion

		#region Heat

		private System.Windows.Forms.GroupBox gbLane;
		private Sport.UI.Controls.TextControl tcLanes;
		private Sport.UI.Controls.ThemeButton tbDivideToHeats;

		/*
		private void tcHeats_TextChanged(object sender, EventArgs e)
		{
			if (_competition != -1)
			{
				int heats = (int) (double) tcHeats.Value;

				Sport.Championships.Competition competition = Competition;

				if (heats != competition.Heats.Count)
				{
					while (heats > competition.Heats.Count)
						competition.Heats.Add(new Sport.Championships.Heat());

					while (heats < competition.Heats.Count)
						competition.Heats.RemoveAt(competition.Heats.Count - 1);

					competitionsGridPanel.Rebuild();
					competitorsGridPanel.Refresh();
				}
			}
		}
		*/

		private void tbDivideToHeats_Click(object sender, System.EventArgs e)
		{

		}
		#endregion

		#region Competitor
		private System.Windows.Forms.GroupBox gbCompetitors;
		private System.Windows.Forms.TextBox tbCompetitor;
		private Sport.UI.Controls.ThemeButton tbCompetitorDown;
		private Sport.UI.Controls.ThemeButton tbCompetitorUp;
		private Sport.UI.Controls.ThemeButton tbRemoveCompetitor;
		private Sport.UI.Controls.ThemeButton tbAddCompetitor;

		private void tbCompetitorUp_Click(object sender, System.EventArgs e)
		{
			if (_competition != -1)
			{
				Sport.Championships.Competition competition = Competition;
				Sport.Championships.Competitor competitor = Competitor;

				if (competitor != null && competitor.Index > 0)
				{
					int i1 = competitor.Index;
					int i2 = -1;

					if (_heat != -1)
					{
						Sport.Championships.Heat heat = competition.Heats[_heat];

						for (int i = i1 - 1; i >= 0 && i2 == -1; i--)
						{
							if (competition.Competitors[i].Heat == competitor.Heat)
								i2 = i;
						}
					}

					if (i2 == -1)
						i2 = i1 - 1;

					Sport.Championships.Competitor temp = competition.Competitors[i2];
					competition.Competitors.RemoveAt(i1);
					competition.Competitors.RemoveAt(i2);
					competition.Competitors.Insert(i2, competitor);
					competition.Competitors.Insert(i1, temp);
					competition.ResetCompetitorsPosition();
					competitorsGridPanel.Rebuild();

					_competitor = competitor.Index;
					OnCompetitorChange();

					//gridCompetitors.Competitor = competitor;
				}
			}
		}

		private void tbCompetitorDown_Click(object sender, System.EventArgs e)
		{
			if (_competition != -1)
			{
				Sport.Championships.Competition competition = Competition;
				Sport.Championships.Competitor competitor = Competitor;

				if (competitor != null && competitor.Index < competition.Competitors.Count - 1)
				{
					int i1 = competitor.Index;
					int i2 = -1;

					if (_heat != -1)
					{
						Sport.Championships.Heat heat = competition.Heats[_heat];

						for (int i = i1 + 1; i < competition.Competitors.Count && i2 == -1; i++)
						{
							if (competition.Competitors[i].Heat == competitor.Heat)
								i2 = i;
						}
					}

					if (i2 == -1)
						i2 = i1 + 1;

					Sport.Championships.Competitor temp = competition.Competitors[i2];
					competition.Competitors.RemoveAt(i2);
					competition.Competitors.RemoveAt(i1);
					competition.Competitors.Insert(i1, temp);
					competition.Competitors.Insert(i2, competitor);
					competition.ResetCompetitorsPosition();
					competitorsGridPanel.Rebuild();

					_competitor = competitor.Index;
					OnCompetitorChange();
					//competitorsGridPanel.Competitor = competitor;
				}
			}
		}

		private void tbRemoveCompetitor_Click(object sender, System.EventArgs e)
		{
			Sport.Championships.Competitor competitor = Competitor;
			if (competitor != null)
			{
				if (this.GotOpenResultForm)
				{
					Sport.UI.MessageBox.Error("לא ניתן להסיר מתמודד כאשר חלון הכנסת תוצאות פתוח", "מחיקת מתמודד");
					ShowResultForm();
					return;
				}

				Sport.Championships.Competition competition = competitor.Competition;
				if (!Sport.UI.MessageBox.Ask("האם להסיר את השחקן '" + competitor.Name +
					"' מהתחרות '" + competition.SportField.Name + "'?", false))
					return;

				competition.Competitors.RemoveAt(_competitor);
				competitorsGridPanel.Rebuild();
			}
		}

		private void tbAddCompetitor_Click(object sender, System.EventArgs e)
		{
			if (_competition == -1)
				return;

			if (this.GotOpenResultForm)
			{
				Sport.UI.MessageBox.Error("לא ניתן להוסיף מתמודד כאשר חלון הכנסת תוצאות פתוח", "הוספת מתמודד");
				ShowResultForm();
				return;
			}

			Sport.Championships.CompetitionPhase phase = Phase;
			Sport.Championships.CompetitionGroup group = Group;
			Sport.Championships.Competition competition = Competition;

			Sport.Entities.Team[] teams = new Sport.Entities.Team[group.Teams.Count];
			for (int n = 0; n < teams.Length; n++)
			{
				teams[n] = group.Teams[n].TeamEntity;
			}

			Sport.Entities.Team selTeam = null;
			if (Team != null)
				selTeam = group.Teams[Team.Index].TeamEntity;

			Sport.Championships.Heat[] heats = new Sport.Championships.Heat[competition.Heats.Count];
			for (int i = 0; i < heats.Length; i++)
			{
				heats[i] = competition.Heats[i];
			}

			Sport.Championships.Heat selHeat = null;
			if (_heat != -1)
				selHeat = competition.Heats[-1];

			Sport.UI.Dialogs.GenericEditDialog ged = new Sport.UI.Dialogs.GenericEditDialog("הוספת שחקן לתחרות");
			ged.Items.Add("קבוצה:", Sport.UI.Controls.GenericItemType.Selection,
				selTeam, teams);
			ged.Items.Add("שחקן:", Sport.UI.Controls.GenericItemType.Selection);
			ged.Items.Add("מקצה:", Sport.UI.Controls.GenericItemType.Selection,
				selHeat, heats);
			ged.Items[0].Nullable = false;
			ged.Items[1].Nullable = false;
			ged.ValueChanged += new EventHandler(CompetitorAddChanged);
			ged.Items[0].ValueChanged += new EventHandler(CompetitorAddTeamChanged);
			ged.Confirmable = false;

			if (selTeam != null)
				CompetitorAddTeamChanged(ged.Items[0], EventArgs.Empty);

			if (ged.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				Sport.Data.Entity entPlayer = ged.Items[1].Value as Sport.Data.Entity;
				Sport.Championships.Heat heat = ged.Items[2].Value as Sport.Championships.Heat;
				if (entPlayer != null)
				{
					Sport.Entities.Player player = new Sport.Entities.Player(entPlayer);
					Sport.Championships.Competitor competitor =
						new Sport.Championships.Competitor(player.Number);
					competitor.Heat = heat == null ? -1 : heat.Index;
					competition.Competitors.Add(competitor);
					competition.ResetCompetitorsPosition();
					competitorsGridPanel.Rebuild();
				}
			}
		}

		private void CompetitorAddTeamChanged(object sender, EventArgs e)
		{
			Sport.UI.Controls.GenericItem item = sender as Sport.UI.Controls.GenericItem;
			Sport.UI.Dialogs.GenericEditDialog ged = item.Container as Sport.UI.Dialogs.GenericEditDialog;
			if (ged != null)
			{
				Sport.Entities.Team team = ged.Items[0].Value as Sport.Entities.Team;
				if (team == null)
				{
					ged.Items[1].Values = null;
				}
				else
				{
					Sport.Data.EntityFilter filter = new Sport.Data.EntityFilter(
						(int)Sport.Entities.Player.Fields.Team, team.Id);
					ArrayList arrPlayers =
						new ArrayList(Sport.Entities.Player.Type.GetEntities(filter));
					if (_competition != -1)
					{
						Sport.Championships.CompetitionPhase phase = Phase;
						Sport.Championships.CompetitionGroup group = Group;
						Sport.Championships.Competition competition = Competition;
						foreach (Sport.Championships.Competitor competitor in competition.Competitors)
						{
							if ((competitor.Player != null) && (competitor.Player.PlayerEntity != null))
							{
								arrPlayers.Remove(competitor.Player.PlayerEntity.Entity);
							}
						}
					}
					ged.Items[1].Values = (Sport.Data.Entity[])
						arrPlayers.ToArray(typeof(Sport.Data.Entity));
				}
				ged.Confirmable = ged.Items[1].Value != null;
			}
		}

		private void CompetitorAddChanged(object sender, EventArgs e)
		{
			Sport.UI.Dialogs.GenericEditDialog ged = sender as Sport.UI.Dialogs.GenericEditDialog;
			ged.Confirmable = ged.Items[1].Value != null;
		}


		#endregion

		#endregion

		#endregion

		#region Initialization

		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(CompetitionChampionshipEditorView));
			this.tbDividePlayers = new Sport.UI.Controls.ThemeButton();
			this.gbCompetitors = new System.Windows.Forms.GroupBox();
			this.tbCompetitor = new System.Windows.Forms.TextBox();
			this.tbCompetitorDown = new Sport.UI.Controls.ThemeButton();
			this.tbCompetitorUp = new Sport.UI.Controls.ThemeButton();
			this.tbRemoveCompetitor = new Sport.UI.Controls.ThemeButton();
			this.tbAddCompetitor = new Sport.UI.Controls.ThemeButton();
			this.gbCompetition = new System.Windows.Forms.GroupBox();
			this.tbCompetitionDown = new Sport.UI.Controls.ThemeButton();
			this.tbCompetitionUp = new Sport.UI.Controls.ThemeButton();
			this.bbCompetition = new Sport.UI.Controls.ButtonBox();
			this.tbRemoveCompetition = new Sport.UI.Controls.ThemeButton();
			this.tbAddCompetition = new Sport.UI.Controls.ThemeButton();
			this.gbLane = new System.Windows.Forms.GroupBox();
			this.tcLanes = new Sport.UI.Controls.TextControl();
			this.tbDivideToHeats = new Sport.UI.Controls.ThemeButton();
			this.panelGroupEdit.SuspendLayout();
			this.gbCompetitors.SuspendLayout();
			this.gbCompetition.SuspendLayout();
			this.gbLane.SuspendLayout();
			// 
			// panelGroupView
			// 
			this.panelGroupView.Location = new System.Drawing.Point(0, 235);
			this.panelGroupView.Name = "panelGroupView";
			this.panelGroupView.Size = new System.Drawing.Size(720, 178);
			// 
			// tbSetResults
			// 
			this.tbSetResults.Name = "tbSetResults";
			// 
			// tbGroupView
			// 
			this.tbGroupView.Image = ((System.Drawing.Image)(resources.GetObject("tbGroupView.Image")));
			this.tbGroupView.Name = "tbGroupView";
			this.tbGroupView.Size = new System.Drawing.Size(69, 17);
			this.tbGroupView.Text = "תחרויות";
			// 
			// tbEditGroup
			// 
			this.tbEditGroup.Image = ((System.Drawing.Image)(resources.GetObject("tbEditGroup.Image")));
			this.tbEditGroup.Name = "tbEditGroup";
			this.tbEditGroup.Size = new System.Drawing.Size(98, 17);
			this.tbEditGroup.Text = "ערוך תחרויות";
			// 
			// panelGroupEdit
			// 
			this.panelGroupEdit.Controls.Add(this.tbDividePlayers);
			this.panelGroupEdit.Controls.Add(this.gbCompetitors);
			this.panelGroupEdit.Controls.Add(this.gbCompetition);
			this.panelGroupEdit.Controls.Add(this.gbLane);
			this.panelGroupEdit.Name = "panelGroupEdit";
			this.panelGroupEdit.Controls.SetChildIndex(this.gbLane, 0);
			this.panelGroupEdit.Controls.SetChildIndex(this.gbCompetition, 0);
			this.panelGroupEdit.Controls.SetChildIndex(this.gbCompetitors, 0);
			this.panelGroupEdit.Controls.SetChildIndex(this.tbDividePlayers, 0);
			// 
			// panelStructureEdit
			// 
			this.panelStructureEdit.Name = "panelStructureEdit";
			// 
			// tbDividePlayers
			// 
			this.tbDividePlayers.Alignment = System.Drawing.StringAlignment.Center;
			this.tbDividePlayers.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.tbDividePlayers.AutoSize = true;
			this.tbDividePlayers.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbDividePlayers.Hue = 200F;
			this.tbDividePlayers.Image = ((System.Drawing.Image)(resources.GetObject("tbDividePlayers.Image")));
			this.tbDividePlayers.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbDividePlayers.ImageList = null;
			this.tbDividePlayers.ImageSize = new System.Drawing.Size(12, 12);
			this.tbDividePlayers.Location = new System.Drawing.Point(616, 74);
			this.tbDividePlayers.Name = "tbDividePlayers";
			this.tbDividePlayers.Saturation = 0.5F;
			this.tbDividePlayers.Size = new System.Drawing.Size(94, 17);
			this.tbDividePlayers.TabIndex = 52;
			this.tbDividePlayers.Text = "חלק שחקנים";
			this.tbDividePlayers.Transparent = System.Drawing.Color.Black;
			this.tbDividePlayers.Click += new System.EventHandler(this.tbDividePlayers_Click);
			// 
			// gbCompetitors
			// 
			this.gbCompetitors.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.gbCompetitors.Controls.Add(this.tbCompetitor);
			this.gbCompetitors.Controls.Add(this.tbCompetitorDown);
			this.gbCompetitors.Controls.Add(this.tbCompetitorUp);
			this.gbCompetitors.Controls.Add(this.tbRemoveCompetitor);
			this.gbCompetitors.Controls.Add(this.tbAddCompetitor);
			this.gbCompetitors.Location = new System.Drawing.Point(50, 6);
			this.gbCompetitors.Name = "gbCompetitors";
			this.gbCompetitors.Size = new System.Drawing.Size(248, 64);
			this.gbCompetitors.TabIndex = 51;
			this.gbCompetitors.TabStop = false;
			this.gbCompetitors.Text = "משתתפים";
			// 
			// tbCompetitor
			// 
			this.tbCompetitor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
				| System.Windows.Forms.AnchorStyles.Right)));
			this.tbCompetitor.Location = new System.Drawing.Point(8, 16);
			this.tbCompetitor.Name = "tbCompetitor";
			this.tbCompetitor.ReadOnly = true;
			this.tbCompetitor.Size = new System.Drawing.Size(232, 20);
			this.tbCompetitor.TabIndex = 32;
			this.tbCompetitor.Text = "";
			// 
			// tbCompetitorDown
			// 
			this.tbCompetitorDown.Alignment = System.Drawing.StringAlignment.Center;
			this.tbCompetitorDown.AutoSize = true;
			this.tbCompetitorDown.Enabled = false;
			this.tbCompetitorDown.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbCompetitorDown.Hue = 200F;
			this.tbCompetitorDown.Image = ((System.Drawing.Image)(resources.GetObject("tbCompetitorDown.Image")));
			this.tbCompetitorDown.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbCompetitorDown.ImageList = null;
			this.tbCompetitorDown.ImageSize = new System.Drawing.Size(12, 12);
			this.tbCompetitorDown.Location = new System.Drawing.Point(131, 40);
			this.tbCompetitorDown.Name = "tbCompetitorDown";
			this.tbCompetitorDown.Saturation = 0.5F;
			this.tbCompetitorDown.Size = new System.Drawing.Size(51, 17);
			this.tbCompetitorDown.TabIndex = 42;
			this.tbCompetitorDown.Text = "הורד";
			this.tbCompetitorDown.Transparent = System.Drawing.Color.Black;
			this.tbCompetitorDown.Click += new System.EventHandler(this.tbCompetitorDown_Click);
			// 
			// tbCompetitorUp
			// 
			this.tbCompetitorUp.Alignment = System.Drawing.StringAlignment.Center;
			this.tbCompetitorUp.AutoSize = false;
			this.tbCompetitorUp.Enabled = false;
			this.tbCompetitorUp.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbCompetitorUp.Hue = 200F;
			this.tbCompetitorUp.Image = ((System.Drawing.Image)(resources.GetObject("tbCompetitorUp.Image")));
			this.tbCompetitorUp.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbCompetitorUp.ImageList = null;
			this.tbCompetitorUp.ImageSize = new System.Drawing.Size(12, 12);
			this.tbCompetitorUp.Location = new System.Drawing.Point(189, 40);
			this.tbCompetitorUp.Name = "tbCompetitorUp";
			this.tbCompetitorUp.Saturation = 0.5F;
			this.tbCompetitorUp.Size = new System.Drawing.Size(51, 17);
			this.tbCompetitorUp.TabIndex = 43;
			this.tbCompetitorUp.Text = "עלה";
			this.tbCompetitorUp.Transparent = System.Drawing.Color.Black;
			this.tbCompetitorUp.Click += new System.EventHandler(this.tbCompetitorUp_Click);
			// 
			// tbRemoveCompetitor
			// 
			this.tbRemoveCompetitor.Alignment = System.Drawing.StringAlignment.Center;
			this.tbRemoveCompetitor.AutoSize = true;
			this.tbRemoveCompetitor.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbRemoveCompetitor.Hue = 0F;
			this.tbRemoveCompetitor.Image = ((System.Drawing.Image)(resources.GetObject("tbRemoveCompetitor.Image")));
			this.tbRemoveCompetitor.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbRemoveCompetitor.ImageList = null;
			this.tbRemoveCompetitor.ImageSize = new System.Drawing.Size(12, 12);
			this.tbRemoveCompetitor.Location = new System.Drawing.Point(62, 40);
			this.tbRemoveCompetitor.Name = "tbRemoveCompetitor";
			this.tbRemoveCompetitor.Saturation = 0.9F;
			this.tbRemoveCompetitor.Size = new System.Drawing.Size(49, 17);
			this.tbRemoveCompetitor.TabIndex = 41;
			this.tbRemoveCompetitor.Text = "מחק";
			this.tbRemoveCompetitor.Transparent = System.Drawing.Color.Black;
			this.tbRemoveCompetitor.Click += new System.EventHandler(this.tbRemoveCompetitor_Click);
			// 
			// tbAddCompetitor
			// 
			this.tbAddCompetitor.Alignment = System.Drawing.StringAlignment.Center;
			this.tbAddCompetitor.AutoSize = true;
			this.tbAddCompetitor.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbAddCompetitor.ForeColor = System.Drawing.SystemColors.ControlText;
			this.tbAddCompetitor.Hue = 220F;
			this.tbAddCompetitor.Image = ((System.Drawing.Image)(resources.GetObject("tbAddCompetitor.Image")));
			this.tbAddCompetitor.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbAddCompetitor.ImageList = null;
			this.tbAddCompetitor.ImageSize = new System.Drawing.Size(12, 12);
			this.tbAddCompetitor.Location = new System.Drawing.Point(8, 40);
			this.tbAddCompetitor.Name = "tbAddCompetitor";
			this.tbAddCompetitor.Saturation = 0.9F;
			this.tbAddCompetitor.Size = new System.Drawing.Size(50, 17);
			this.tbAddCompetitor.TabIndex = 40;
			this.tbAddCompetitor.Text = "חדש";
			this.tbAddCompetitor.Transparent = System.Drawing.Color.Black;
			this.tbAddCompetitor.Click += new System.EventHandler(this.tbAddCompetitor_Click);
			// 
			// gbCompetition
			// 
			this.gbCompetition.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.gbCompetition.Controls.Add(this.tbCompetitionDown);
			this.gbCompetition.Controls.Add(this.tbCompetitionUp);
			this.gbCompetition.Controls.Add(this.bbCompetition);
			this.gbCompetition.Controls.Add(this.tbRemoveCompetition);
			this.gbCompetition.Controls.Add(this.tbAddCompetition);
			this.gbCompetition.Location = new System.Drawing.Point(462, 6);
			this.gbCompetition.Name = "gbCompetition";
			this.gbCompetition.Size = new System.Drawing.Size(248, 64);
			this.gbCompetition.TabIndex = 50;
			this.gbCompetition.TabStop = false;
			this.gbCompetition.Text = "תחרות";
			// 
			// tbCompetitionDown
			// 
			this.tbCompetitionDown.Alignment = System.Drawing.StringAlignment.Center;
			this.tbCompetitionDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.tbCompetitionDown.AutoSize = true;
			this.tbCompetitionDown.Enabled = false;
			this.tbCompetitionDown.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbCompetitionDown.Hue = 200F;
			this.tbCompetitionDown.Image = ((System.Drawing.Image)(resources.GetObject("tbCompetitionDown.Image")));
			this.tbCompetitionDown.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbCompetitionDown.ImageList = null;
			this.tbCompetitionDown.ImageSize = new System.Drawing.Size(12, 12);
			this.tbCompetitionDown.Location = new System.Drawing.Point(138, 40);
			this.tbCompetitionDown.Name = "tbCompetitionDown";
			this.tbCompetitionDown.Saturation = 0.5F;
			this.tbCompetitionDown.Size = new System.Drawing.Size(51, 17);
			this.tbCompetitionDown.TabIndex = 33;
			this.tbCompetitionDown.Text = "הורד";
			this.tbCompetitionDown.Transparent = System.Drawing.Color.Black;
			this.tbCompetitionDown.Click += new System.EventHandler(this.tbCompetitionDown_Click);
			// 
			// tbCompetitionUp
			// 
			this.tbCompetitionUp.Alignment = System.Drawing.StringAlignment.Center;
			this.tbCompetitionUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.tbCompetitionUp.AutoSize = true;
			this.tbCompetitionUp.Enabled = false;
			this.tbCompetitionUp.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbCompetitionUp.Hue = 200F;
			this.tbCompetitionUp.Image = ((System.Drawing.Image)(resources.GetObject("tbCompetitionUp.Image")));
			this.tbCompetitionUp.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbCompetitionUp.ImageList = null;
			this.tbCompetitionUp.ImageSize = new System.Drawing.Size(12, 12);
			this.tbCompetitionUp.Location = new System.Drawing.Point(193, 40);
			this.tbCompetitionUp.Name = "tbCompetitionUp";
			this.tbCompetitionUp.Saturation = 0.5F;
			this.tbCompetitionUp.Size = new System.Drawing.Size(47, 17);
			this.tbCompetitionUp.TabIndex = 34;
			this.tbCompetitionUp.Text = "עלה";
			this.tbCompetitionUp.Transparent = System.Drawing.Color.Black;
			this.tbCompetitionUp.Click += new System.EventHandler(this.tbCompetitionUp_Click);
			// 
			// bbCompetition
			// 
			this.bbCompetition.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
				| System.Windows.Forms.AnchorStyles.Right)));
			this.bbCompetition.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.bbCompetition.Location = new System.Drawing.Point(8, 16);
			this.bbCompetition.Name = "bbCompetition";
			this.bbCompetition.Size = new System.Drawing.Size(232, 20);
			this.bbCompetition.TabIndex = 32;
			this.bbCompetition.Value = null;
			this.bbCompetition.ValueSelector = null;
			this.bbCompetition.ValueSelect += new System.EventHandler(this.bbCompetition_ValueSelect);
			// 
			// tbRemoveCompetition
			// 
			this.tbRemoveCompetition.Alignment = System.Drawing.StringAlignment.Center;
			this.tbRemoveCompetition.AutoSize = true;
			this.tbRemoveCompetition.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbRemoveCompetition.Hue = 0F;
			this.tbRemoveCompetition.Image = ((System.Drawing.Image)(resources.GetObject("tbRemoveCompetition.Image")));
			this.tbRemoveCompetition.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbRemoveCompetition.ImageList = null;
			this.tbRemoveCompetition.ImageSize = new System.Drawing.Size(12, 12);
			this.tbRemoveCompetition.Location = new System.Drawing.Point(61, 40);
			this.tbRemoveCompetition.Name = "tbRemoveCompetition";
			this.tbRemoveCompetition.Saturation = 0.9F;
			this.tbRemoveCompetition.Size = new System.Drawing.Size(49, 17);
			this.tbRemoveCompetition.TabIndex = 30;
			this.tbRemoveCompetition.Text = "מחק";
			this.tbRemoveCompetition.Transparent = System.Drawing.Color.Black;
			this.tbRemoveCompetition.Click += new System.EventHandler(this.tbRemoveCompetition_Click);
			// 
			// tbAddCompetition
			// 
			this.tbAddCompetition.Alignment = System.Drawing.StringAlignment.Center;
			this.tbAddCompetition.AutoSize = true;
			this.tbAddCompetition.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbAddCompetition.Hue = 220F;
			this.tbAddCompetition.Image = ((System.Drawing.Image)(resources.GetObject("tbAddCompetition.Image")));
			this.tbAddCompetition.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbAddCompetition.ImageList = null;
			this.tbAddCompetition.ImageSize = new System.Drawing.Size(12, 12);
			this.tbAddCompetition.Location = new System.Drawing.Point(8, 40);
			this.tbAddCompetition.Name = "tbAddCompetition";
			this.tbAddCompetition.Saturation = 0.9F;
			this.tbAddCompetition.Size = new System.Drawing.Size(50, 17);
			this.tbAddCompetition.TabIndex = 29;
			this.tbAddCompetition.Text = "חדש";
			this.tbAddCompetition.Transparent = System.Drawing.Color.Black;
			this.tbAddCompetition.Click += new System.EventHandler(this.tbAddCompetition_Click);
			// 
			// gbLane
			// 
			this.gbLane.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.gbLane.Controls.Add(this.tcLanes);
			this.gbLane.Controls.Add(this.tbDivideToHeats);
			this.gbLane.Location = new System.Drawing.Point(304, 6);
			this.gbLane.Name = "gbLane";
			this.gbLane.Size = new System.Drawing.Size(152, 64);
			this.gbLane.TabIndex = 49;
			this.gbLane.TabStop = false;
			this.gbLane.Text = "מסלולים";
			// 
			// tcLanes
			// 
			this.tcLanes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
				| System.Windows.Forms.AnchorStyles.Right)));
			this.tcLanes.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.tcLanes.Controller = null;
			this.tcLanes.Location = new System.Drawing.Point(8, 16);
			this.tcLanes.Name = "tcLanes";
			this.tcLanes.ReadOnly = false;
			this.tcLanes.SelectionLength = 0;
			this.tcLanes.SelectionStart = 0;
			this.tcLanes.ShowSpin = true;
			this.tcLanes.Size = new System.Drawing.Size(136, 20);
			this.tcLanes.TabIndex = 32;
			this.tcLanes.Value = "";
			this.tcLanes.TextChanged += new EventHandler(tcLanes_TextChanged);
			// 
			// tbDivideToHeats
			// 
			this.tbDivideToHeats.Alignment = System.Drawing.StringAlignment.Center;
			this.tbDivideToHeats.AutoSize = true;
			this.tbDivideToHeats.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbDivideToHeats.Hue = 200F;
			this.tbDivideToHeats.Image = ((System.Drawing.Image)(resources.GetObject("tbDivideToHeats.Image")));
			this.tbDivideToHeats.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbDivideToHeats.ImageList = null;
			this.tbDivideToHeats.ImageSize = new System.Drawing.Size(12, 12);
			this.tbDivideToHeats.Location = new System.Drawing.Point(8, 40);
			this.tbDivideToHeats.Name = "tbDivideToHeats";
			this.tbDivideToHeats.Saturation = 0.5F;
			this.tbDivideToHeats.Size = new System.Drawing.Size(93, 17);
			this.tbDivideToHeats.TabIndex = 29;
			this.tbDivideToHeats.Text = "חלק למקצים";
			this.tbDivideToHeats.Transparent = System.Drawing.Color.Black;
			this.tbDivideToHeats.Visible = false;
			this.tbDivideToHeats.Click += new System.EventHandler(this.tbDivideToHeats_Click);
			// 
			// CompetitionChampionshipEditorView
			// 
			this.Name = "CompetitionChampionshipEditorView";
			this.panelGroupEdit.ResumeLayout(false);
			this.gbCompetitors.ResumeLayout(false);
			this.gbCompetition.ResumeLayout(false);
			this.gbLane.ResumeLayout(false);

		}

		private CompetitionsGridPanel competitionsGridPanel;
		private CompetitorsGridPanel competitorsGridPanel;
		private System.Windows.Forms.Label labelCompetitors;

		public CompetitionChampionshipEditorView()
		{
			_competition = -1;
			_heat = -1;
			_competitor = -1;
			InitializeComponent();

			tbPrint.Visible = false;
			tbReports.Visible = true;
			tbReports.Left = tbEditGroup.Left + tbEditGroup.Width + 5;

			tbLeagueSummary.Visible = true;
			tbLeagueSummary.Left = tbReports.Left + tbReports.Width + 10;

			tcLanes.Controller = new Sport.UI.Controls.NumberController(1, 100, byte.MaxValue, 0);

			competitionsGridPanel = new CompetitionsGridPanel();
			// 
			// competitionsGridPanel
			// 
			this.competitionsGridPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top)
				| System.Windows.Forms.AnchorStyles.Left)
				| System.Windows.Forms.AnchorStyles.Right)));
			this.competitionsGridPanel.Location = new System.Drawing.Point(8, 24);
			this.competitionsGridPanel.Name = "competitionsGridPanel";
			this.competitionsGridPanel.Size = new System.Drawing.Size(493, 172);//panelGroupView.Height - 32);
			this.competitionsGridPanel.SelectionChanged += new EventHandler(CompetitionsGridPanelSelectionChanged);
			this.panelGroupView.Controls.Add(this.competitionsGridPanel);

			labelCompetitors = new System.Windows.Forms.Label();
			// 
			// labelCompetitors
			// 
			this.labelCompetitors.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelCompetitors.Location = new System.Drawing.Point(426, 204);
			this.labelCompetitors.Name = "labelCompetitors";
			this.labelCompetitors.Size = new System.Drawing.Size(75, 16);
			this.labelCompetitors.TabIndex = 39;
			this.labelCompetitors.Text = "משתתפים:";
			this.panelGroupView.Controls.Add(this.labelCompetitors);

			this.btnResetCustomPosition = new System.Windows.Forms.Button();
			// 
			// btnResetCustomPosition
			// 
			this.btnResetCustomPosition.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnResetCustomPosition.Location = new System.Drawing.Point(320, 204);
			this.btnResetCustomPosition.Name = "btnResetCustomPosition";
			this.btnResetCustomPosition.Size = new System.Drawing.Size(130, 16);
			this.btnResetCustomPosition.TabIndex = 40;
			this.btnResetCustomPosition.Text = "איפוס דירוג ידני";
			this.btnResetCustomPosition.Click += new EventHandler(btnResetCustomPosition_Click);
			this.panelGroupView.Controls.Add(this.btnResetCustomPosition);

			this.btnDeleteAllResults = new System.Windows.Forms.Button();
			// 
			// btnDeleteAllResults
			// 
			this.btnDeleteAllResults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnDeleteAllResults.Location = new System.Drawing.Point(180, 204);
			this.btnDeleteAllResults.Name = "btnDeleteAllResults";
			this.btnDeleteAllResults.Size = new System.Drawing.Size(130, 16);
			this.btnDeleteAllResults.TabIndex = 40;
			this.btnDeleteAllResults.Text = "מחיקת כל המשתתפים";
			this.btnDeleteAllResults.Click += new EventHandler(btnDeleteAllResults_Click);
			this.panelGroupView.Controls.Add(this.btnDeleteAllResults);

			this.btnRecalculateScore = new System.Windows.Forms.Button();
			// 
			// btnRecalculateScore
			// 
			this.btnRecalculateScore.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnRecalculateScore.Location = new System.Drawing.Point(10, 204);
			this.btnRecalculateScore.Name = "btnRecalculateScore";
			this.btnRecalculateScore.Size = new System.Drawing.Size(140, 16);
			this.btnRecalculateScore.TabIndex = 40;
			this.btnRecalculateScore.Text = "חישוב מחדש של ניקוד";
			this.btnRecalculateScore.Click += new EventHandler(btnRecalculateScore_Click);
			this.panelGroupView.Controls.Add(this.btnRecalculateScore);

			competitorsGridPanel = new CompetitorsGridPanel();
			// 
			// competitorsGridPanel
			// 
			this.competitorsGridPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
				| System.Windows.Forms.AnchorStyles.Left)
				| System.Windows.Forms.AnchorStyles.Right)));
			this.competitorsGridPanel.Location = new System.Drawing.Point(8, 220);
			this.competitorsGridPanel.Name = "competitorsGridPanel";
			this.competitorsGridPanel.Size = new System.Drawing.Size(493, panelGroupView.Height - 212);
			this.competitorsGridPanel.SelectionChanged += new EventHandler(CompetitorsGridPanelSelectionChanged);
			this.competitorsGridPanel.CompetitorsDeleted += new CompetitorsGridPanel.DeleteCompetitorsEventHandler(competitorsGridPanel_CompetitorsDeleted);
			this.panelGroupView.Controls.Add(this.competitorsGridPanel);
		}

		private System.Windows.Forms.Button btnResetCustomPosition;
		private System.Windows.Forms.Button btnDeleteAllResults;
		private System.Windows.Forms.Button btnRecalculateScore;

		#endregion

		#region Editing

		protected override Sport.Championships.Phase CreatePhase(string name)
		{
			return new Sport.Championships.CompetitionPhase(name, Sport.Championships.Status.Planned);
		}

		protected override Sport.Championships.Group CreateGroup(string name)
		{
			return new Sport.Championships.CompetitionGroup(name);
		}

		protected override Sport.Championships.Team CreateTeam(ref int position)
		{
			return base.CreateTeam(ref position);
		}


		#endregion

		#region View Operations

		public override void Open()
		{
			base.Open();

			OnCompetitionChange();
			OnHeatChange();
			OnCompetitorChange();
		}

		public override bool Closing()
		{
			if (resultForms.Exists(r => r.ChangesOccured) && Sport.UI.MessageBox.Ask("סגירת חלון זה תבטל את השינויים שנעשו בחלון קביעת תוצאות. האם להמשיך?", "בניית אליפות", false) == false)
				return false;
			resultForms.ForEach(r => r.ForceCloseWithoutConfirmation());
			return base.Closing();
		}

		public override void Close()
		{
			Sportsman.Context.SetStatusBar(Forms.MainForm.StatusBarPanels.Error, "");
			resultForms.ForEach(r =>
			{
				r.DialogResult = DialogResult.Cancel;
				r.Close();
				r.Dispose();
			});
			base.Close();
		}
		#endregion

		public new Sport.Championships.CompetitionChampionship Championship
		{
			get { return (Sport.Championships.CompetitionChampionship)base.Championship; }
		}

		public void SortByPosition()
		{
			this.competitorsGridPanel.Sort(0, new int[] { (int)Producer.CompetitorsGridPanel.CompetitorField.ResultPosition });
			this.competitorsGridPanel.Rebuild();
			this.competitorsGridPanel.Refresh();
			this.Rebuild();
			this.Refresh();
		}

		#region Selection

		public new Sport.Championships.CompetitionPhase Phase
		{
			get { return (Sport.Championships.CompetitionPhase)base.Phase; }
			set { base.Phase = value; }
		}

		public new Sport.Championships.CompetitionGroup Group
		{
			get { return (Sport.Championships.CompetitionGroup)base.Group; }
			set { base.Group = value; }
		}

		public new Sport.Championships.CompetitionTeam Team
		{
			get { return (Sport.Championships.CompetitionTeam)base.Team; }
			set { base.Team = value; }
		}

		private int _competition;
		public Sport.Championships.Competition Competition
		{
			get
			{
				if (_competition >= 0)
				{
					if (this.Group != null && this.Group.Competitions != null && _competition < this.Group.Competitions.Count)
					{
						return Group.Competitions[_competition];
					}
				}
				return null;
			}
			set
			{
				competitionsGridPanel.Competition = value;
			}
		}

		private int _heat;
		public Sport.Championships.Heat Heat
		{
			get { return _heat == -1 ? null : Competition.Heats[_heat]; }
			set { competitionsGridPanel.Heat = value; }
		}

		private int _competitor;
		public Sport.Championships.Competitor Competitor
		{
			get
			{
				if (_competitor == -1)
					return null;
				if (this.Competition == null || this.Competition.Competitors == null)
					return null;
				return this.Competition.Competitors[_competitor];
			}
			set { competitorsGridPanel.Competitor = value; }
		}

		private void CompetitionsGridPanelSelectionChanged(object sender, EventArgs e)
		{
			int competition = competitionsGridPanel.Competition == null ? -1 : competitionsGridPanel.Competition.Index;
			int heat = competitionsGridPanel.Heat == null ? -1 : competitionsGridPanel.Heat.Index;
			Sport.UI.Dialogs.WaitForm.ShowWait("טוען מתמודדים אנא המתן...", true);
			bool blnRebuild = false;
			if (competitionsGridPanel.ShowGroups && Group != competitionsGridPanel.Group)
			{
				Group = competitionsGridPanel.Group;
				_competition = competition;
				_heat = heat;
				_competitor = -1;
				OnCompetitionChange();
				OnHeatChange();
				OnCompetitorChange();
				blnRebuild = true;
			}
			else if (_competition != competition)
			{
				_competition = competition;
				_heat = heat;
				_competitor = -1;
				OnCompetitionChange();
				OnHeatChange();
				OnCompetitorChange();
				blnRebuild = true;
			}
			else if (_heat != heat)
			{
				_heat = heat;
				_competitor = -1;
				OnHeatChange();
				OnCompetitorChange();
				blnRebuild = true;
			}
			Sport.UI.Dialogs.WaitForm.HideWait();

			if (blnRebuild)
				this.SortByPosition();
		}

		private void CompetitorsGridPanelSelectionChanged(object sender, EventArgs e)
		{
			int competitor = competitorsGridPanel.Competitor == null ? -1 : competitorsGridPanel.Competitor.Index;
			if (_competitor != competitor)
			{
				_competitor = competitor;
				OnCompetitorChange();
			}
		}

		protected override void OnCurrentPhaseChange()
		{
			base.OnCurrentPhaseChange();

			tbSetResults.Enabled = Group != null && !Championship.Editing; //Phase.Status != Sport.Championships.Status.Planned
		}

		protected override void OnEditModeChange()
		{
			base.OnEditModeChange();

			tbSetResults.Enabled = Group != null && !Championship.Editing; //Phase.Status != Sport.Championships.Status.Planned
		}



		protected override void OnPhaseChange()
		{
			base.OnPhaseChange();

			competitionsGridPanel.Phase = Phase;
		}

		protected override void OnGroupChange()
		{
			base.OnGroupChange();

			tbAddCompetition.Enabled = Group != null;

			tbSetResults.Enabled = Group != null && !Championship.Editing; //Phase.Status != Sport.Championships.Status.Planned

			if (!competitionsGridPanel.ShowGroups)
				competitionsGridPanel.Group = Group;
		}

		protected override void OnTeamChange()
		{
			base.OnTeamChange();

			string strErrorMessage = "";

			Sport.Championships.CompetitionGroup group = this.Group;
			Sport.Championships.CompetitionTeam team = this.Team;
			Sport.Championships.Competition competition = this.Competition;

			if ((_minTeamCompetitors >= 0) && (_maxTeamCompetitors >= 0))
			{
				if ((group != null) && (team != null) && (competition != null))
				{
					int teamCompetitors = group.GetTeamCompetitionCompetitors(
						team, competition.Index);
					if (teamCompetitors < _minTeamCompetitors)
						strErrorMessage = "קבוצה זו משתפת פחות מידי תלמידים";
					else if (teamCompetitors > _maxTeamCompetitors)
						strErrorMessage = "קבוצה זו משתפת יותר מידי תלמידים";
				}
			}

			if (team != null)
			{
				if (team.IsConfirmed() == false)
					strErrorMessage = "קבוצה זו לא עברה אישור רכז";

				if (team.IsValidShirtRange() == false)
					strErrorMessage = "לא מוגדר טווח מספרי חזה עבור בית הספר";
			}

			Sportsman.Context.SetStatusBar(Forms.MainForm.StatusBarPanels.Error,
				strErrorMessage);
		}

		private bool selecting = false;
		private int _minCompetitorCompetitions = -1;
		private int _maxCompetitorCompetitions = -1;
		private int _minTeamCompetitors = -1;
		private int _maxTeamCompetitors = -1;
		protected void OnCompetitionChange()
		{
			bool s = selecting;
			selecting = true;

			_changedByCode = true;
			tcLanes.Value = _competition == -1 ? 0 : Competition.LaneCount;
			tcLanes.Enabled = _competition != -1;

			tbAddCompetition.Enabled = Phase != null;
			bbCompetition.Enabled = _competition != -1;
			bbCompetition.Value = _competition == -1 ? null : Competition.SportField;
			tbRemoveCompetition.Enabled = _competition != -1;
			tbCompetitionUp.Enabled = _competition != -1 && Competition.Index > 0;
			tbCompetitionDown.Enabled = _competition != -1 && Competition.Index < Group.Competitions.Count - 1;

			tbAddCompetitor.Enabled = _competition != -1;
			selecting = s;
			competitorsGridPanel.Competition = Competition;

			gbLane.Text = "מסלולים";
			if (this.Competition != null)
			{
				Sport.Rulesets.Rules.GeneralSportTypeData objRule =
					(Sport.Rulesets.Rules.GeneralSportTypeData)
					this.Competition.GetRule(typeof(Sport.Rulesets.Rules.GeneralSportTypeData),
					typeof(Sport.Rulesets.Rules.GeneralSportTypeDataRule));
				if (objRule != null)
				{
					if (!objRule.HasLanes)
						gbLane.Text = "עמדות";
				}
			}

			_minCompetitorCompetitions = -1;
			_maxCompetitorCompetitions = -1;
			_minTeamCompetitors = -1;
			_maxTeamCompetitors = -1;
			phaseGroupGridPanel.CompetitionIndex = -1;
			Sport.Championships.Competition competition = this.Competition;
			if (competition != null)
			{
				phaseGroupGridPanel.CompetitionIndex = competition.Index;

				Sport.Rulesets.Rules.CompetitorCompetitions cc =
					competition.GetRule(typeof(Sport.Rulesets.Rules.CompetitorCompetitions),
					typeof(Sport.Rulesets.Rules.CompetitorCompetitionsRule)) as
					Sport.Rulesets.Rules.CompetitorCompetitions;
				Sport.Rulesets.Rules.CompetitionTeamCompetitors ctc =
					competition.GetRule(typeof(Sport.Rulesets.Rules.CompetitionTeamCompetitors),
					typeof(Sport.Rulesets.Rules.CompetitionTeamCompetitorsRule)) as
					Sport.Rulesets.Rules.CompetitionTeamCompetitors;

				if (cc != null)
				{
					_minCompetitorCompetitions = cc.Minimum;
					_maxCompetitorCompetitions = cc.Maximum;
				}

				if (ctc != null)
				{
					_minTeamCompetitors = ctc.Minimum;
					_maxTeamCompetitors = ctc.Maximum;
				}
			}

			phaseGroupGridPanel.MinTeamCompetitors = _minTeamCompetitors;
			phaseGroupGridPanel.MaxTeamCompetitors = _maxTeamCompetitors;
			phaseGroupGridPanel.Refresh();
		}

		protected void OnHeatChange()
		{
			bool s = selecting;
			selecting = true;
			//tcHeats.Value = _competition == -1 ? 0 : Competition.Heats.Count;
			tcLanes.Enabled = _competition != -1;
			selecting = s;
			competitorsGridPanel.Heat = Heat;
		}

		protected void OnCompetitorChange()
		{
			tbRemoveCompetitor.Enabled = _competitor != -1;
			tbCompetitor.Text = _competitor == -1 ? null : Competitor.Name;

			tbCompetitorUp.Enabled = _competitor > 0;
			tbCompetitorDown.Enabled = _competitor != -1 && _competitor < Competition.Competitors.Count - 1;

			string strErrorMessage = string.Empty;
			ArrayList arrSameResult = competitorsGridPanel.GetPlayersWithSameResult(this.Competitor);
			if (arrSameResult != null && arrSameResult.Count > 1)
				strErrorMessage = competitorsGridPanel.BuildSameResultErrorMessage(arrSameResult, this.Competitor.PlayerNumber);
			if (strErrorMessage.Length == 0 && _minCompetitorCompetitions >= 0 && _maxCompetitorCompetitions >= 0)
			{
				Sport.Championships.Competitor competitor = this.Competitor;
				if ((competitor != null) && (competitor.Player != null))
				{
					int competitorCompetitions = competitor.Player.CompetitionCount;
					if (competitorCompetitions < _minCompetitorCompetitions)
						strErrorMessage = "תלמיד זה משתתף בפחות מידי מקצועות";
					else if (competitorCompetitions > _maxCompetitorCompetitions)
						strErrorMessage = "תלמיד זה משתתף ביותר מידי מקצועות";
				}
			}
			Sportsman.Context.SetStatusBar(Forms.MainForm.StatusBarPanels.Error,
				strErrorMessage);
		}

		#endregion

		protected override void Customize()
		{
			//			MatchesGridCustomizeForm mgcf = new MatchesGridCustomizeForm(matchesGridPanel);

			//			mgcf.ShowDialog();
		}



		protected override void SetResults()
		{
			Sport.Championships.Competition competition = this.Competition;
			if (competition == null)
			{
				Sport.UI.MessageBox.Warn("יש לבחור תחרות כדי להגדיר תוצאות", "בניית אליפות");
				return;
			}

			if (competition.ResultType == null)
			{
				Sport.UI.MessageBox.Error("לא מוגדר חוק סוג תוצאה עבור מקצוע זה, " +
					"לא ניתן לערוך תוצאות", "בניית אליפות");
				return;
			}

			bool blnScoreIsRank = false;
			if (competition.GeneralData != null)
				blnScoreIsRank = competition.GeneralData.ScoreIsRank;
			if ((blnScoreIsRank == false) && (competition.ScoreTable == null))
			{
				Sport.UI.MessageBox.Error("לא מוגדרת טבלת ניקוד עבור מקצוע זה, " +
					"לא ניתן לערוך תוצאות", "בניית אליפות");
				return;
			}

			Sport.Championships.CompetitionGroup group = Group;
			if (group != null)
			{
				Sport.Championships.CompetitionPhase phase = group.Phase;
				CompetitionResultForm resultForm = resultForms.Find(r =>
				{
					return (_heat == -1 && this.Competition == r.Competition) ||
						(_heat >= 0 && this.Heat == r.Heat);
				});
				if (resultForm == null)
				{
					if (_heat == -1)
						resultForm = new CompetitionResultForm(Competition);
					else
						resultForm = new CompetitionResultForm(Heat);
					resultForm.ParentView = this;
					resultForm.WindowState = FormWindowState.Normal;
					resultForm.Shown += new EventHandler(resultForm_Shown);
					resultForm.FormClosed += new FormClosedEventHandler(resultForm_FormClosed);
					resultForm.ShowInTaskbar = true;
					resultForms.Add(resultForm);
				}
				_activeResultForm = resultForm;
				ShowResultForm();
			}
		}

		void resultForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			_activeResultForm = null;
			resultForms.Remove(sender as CompetitionResultForm);
		}

		void resultForm_Shown(object sender, EventArgs e)
		{
			_activeResultForm = (sender as CompetitionResultForm);
		}

		public void Rebuild()
		{
			competitorsGridPanel.Rebuild();
		}

		protected override void BeforeSaveGroup()
		{
			//got anything?
			if (this.Group == null)
				return;

			Dictionary<int, Sport.Championships.Competition> localCompetitions = this.Group.MapCompetitions();
			Sport.UI.Dialogs.WaitForm.Suppress();
			Sport.Championships.Championship serverChampionship = Sport.Championships.Championship.GetChampionship(this.Championship.ChampionshipCategory.Id, true);
			Sport.UI.Dialogs.WaitForm.UnSuppress();
			Sport.Championships.CompetitionPhase serverPhase = serverChampionship.Phases.OfType<Sport.Championships.CompetitionPhase>().ToList().FirstOrDefault(p => p.Index == this.Phase.Index);
			if (serverPhase != null)
			{
				Sport.Championships.CompetitionGroup serverGroup = serverPhase.Groups.OfType<Sport.Championships.CompetitionGroup>().ToList().FirstOrDefault(g => g.Index == this.Group.Index);
				if (serverGroup != null)
				{
					Dictionary<int, Sport.Championships.Competition> serverCompetitions = serverGroup.MapCompetitions();
					Dictionary<int, Sport.Common.Tools.MappingDiffState> localDiff = Sport.Common.Tools.GetMappingDiff<Sport.Championships.Competition>(serverCompetitions, localCompetitions);
					Dictionary<int, Sport.Common.Tools.MappingDiffState> preEditDiff = Sport.Common.Tools.GetMappingDiff<Sport.Championships.Competition>(serverCompetitions, _preEditCompetitions);
					localDiff.Keys.ToList().ForEach(sportFieldId =>
					{
						switch (localDiff[sportFieldId])
						{
							case Sport.Common.Tools.MappingDiffState.OnlyFirst:
								//exists on server but not locally, need to add unless it was removed on purpose.
								if (!_preEditCompetitions.ContainsKey(sportFieldId))
								{
									Sport.Championships.Competition competition = new Sport.Championships.Competition(serverCompetitions[sportFieldId].SportField);
									this.Group.Competitions.Add(competition);
								}
								break;
							case Sport.Common.Tools.MappingDiffState.OnlySecond:
								//exists locally, but not on server, need to delete unless it was added while in edit.
								if (_preEditCompetitions.ContainsKey(sportFieldId))
								{
									this.Group.Competitions.Remove(localCompetitions[sportFieldId]);
								}
								break;
						}
					});
				}
			}

			//get heat count:
			int heatCount = Group.GetHeatCount();
			bool blnAssignCompetitors = true;

			//changed?
			if (heatCount > 0)
			{
				if (!_lanesChanged)
					return;
			}

			//going to save, so lanes count didn't change:
			_lanesChanged = false;

			//need to ask user?
			if (heatCount > 0)
			{
				blnAssignCompetitors = Sport.UI.MessageBox.Ask("האם לחלק משתתפים למקצים?",
					MessageBoxIcon.Question, true);
			}

			//cancel?
			if (!blnAssignCompetitors)
				return;

			//assign competitors heats.
			AssignHeats();
		}

		protected override void AfterSaveGroup()
		{
			competitorsGridPanel.Rebuild();
			competitionsGridPanel.Rebuild();
		}

		protected override void ReportsButtonClicked()
		{
			GenerateReports();
		}

		private List<Sport.Championships.Competition> GetMultiCompetitions()
		{
			List<Sport.Championships.Competition> competitions = new List<Sport.Championships.Competition>();
			Sport.Championships.CompetitionChampionship championship = this.Championship;
			if (championship.Phases != null)
			{
				championship.Phases.OfType<Sport.Championships.CompetitionPhase>().ToList().ForEach(phase =>
				{
					if (phase.Groups != null)
					{
						phase.Groups.OfType<Sport.Championships.CompetitionGroup>().ToList().ForEach(group =>
						{
							if (group.Competitions != null)
							{
								competitions.AddRange(group.Competitions.OfType<Sport.Championships.Competition>().ToList().FindAll(c =>
									c.SportField.SportFieldType.CompetitionType == Sport.Types.CompetitionType.MultiCompetition));
							}
						});
					}
				});
			}
			return competitions;
		}

		protected override void LeagueSummaryButtonClicked()
		{
			if (this.Championship == null)
				return;

			List<Sport.Championships.Competition> multiCompetitions = GetMultiCompetitions();
			if (multiCompetitions.Count == 0)
			{
				Sport.UI.MessageBox.Error("לא נמצאו תחרויות קרב רב באליפות זו", "סיכום ליגה");
				return;
			}

			//multiCompetitions[0].Competitors[0].GroupTeam.Name;
			//Sport.UI.MessageBox.Show(string.Join("\n", multiCompetitions.ConvertAll(c => string.Join(", ", c.Competitors.OfType<Sport.Championships.Competitor>().ToList().ConvertAll(cc => cc.GroupTeam.Name)))));

			//initialize report document:
			Documents.CompetitionReports competitionDoc = new Documents.CompetitionReports(CompetitionReportType.MultiCompetitionReport, this.Championship);
			
			//print:
			try
			{
				competitionDoc.Print(true);
			}
			catch(Exception ex)
			{
				Sport.UI.MessageBox.Error("אירעה שגיאה בעת יצירת הדו\"ח אנא וודא נכונות הקלט", "הדפסה");
				System.Windows.Forms.MessageBox.Show("Message: " + ex.Message + "\nStack: " + ex.StackTrace, "Details", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
			}
		}


		//private void AssignHeats(Sport.Entities.Player[] players)
		private void AssignHeats()
		{
			//iterate over competitions in group, assign heats for each:
			foreach (Sport.Championships.Competition competition in Group.Competitions)
			{
				//got any competitors?
				if ((competition.Competitors == null) || (competition.Competitors.Count == 0))
					continue;

				//get rule:
				bool blnGotLanes = false;
				Sport.Rulesets.Rules.GeneralSportTypeData objRule =
					(Sport.Rulesets.Rules.GeneralSportTypeData)
					competition.GetRule(typeof(Sport.Rulesets.Rules.GeneralSportTypeData),
					typeof(Sport.Rulesets.Rules.GeneralSportTypeDataRule));
				if (objRule != null)
					blnGotLanes = objRule.HasLanes;

				//clear current heats:
				competition.Heats.Clear();

				//get competitors having team:
				ArrayList competitors = new ArrayList();
				foreach (Sport.Championships.Competitor competitor in competition.Competitors)
				{
					if (competitor.Player == null)
						continue;
					if (competitor.Player.CompetitionTeam == null)
						continue;
					competitors.Add(competitor);
				}

				//divide competitors to their teams:
				Hashtable tblTeamCompetitors = new Hashtable();
				foreach (Sport.Championships.Competitor competitor in competitors)
				{
					//build current key
					Sport.Entities.Team team = competitor.Player.CompetitionTeam.TeamEntity;

					//create competitors list if needed:
					if (tblTeamCompetitors[team] == null)
						tblTeamCompetitors[team] = new ArrayList();

					//add current competitor:
					(tblTeamCompetitors[team] as ArrayList).Add(competitor);
				}

				//build array list:
				ArrayList arrTeamCompetitors = new ArrayList();
				foreach (object key in tblTeamCompetitors.Keys)
				{
					ArrayList curArray = (ArrayList)tblTeamCompetitors[key];
					arrTeamCompetitors.Add(curArray.ToArray(typeof(Sport.Championships.Competitor)));
				}

				//scramble:
				int heatCount = competition.LaneCount;
				if (blnGotLanes)
				{
					double competitorsPerLane =
						(((double)competitors.Count) / ((double)competition.LaneCount));
					if (competitorsPerLane.ToString().IndexOf(".") > 0)
						competitorsPerLane += 1;
					heatCount = (int)competitorsPerLane;
				}
				ArrayList arrScrambledCompetitors = Sport.Common.Tools.Scramble(
					arrTeamCompetitors, heatCount);

				//clear current competitors:
				competition.Competitors.Clear();

				//add heats and competitors:
				foreach (Sport.Championships.Competitor[] curTeamCompetitors in arrScrambledCompetitors)
				{
					//create new heat:
					Sport.Championships.Heat heat = new Sport.Championships.Heat();

					//add to competition:
					competition.Heats.Add(heat);

					//add the competitors and assign heat:
					foreach (Sport.Championships.Competitor competitor in curTeamCompetitors)
					{
						competition.Competitors.Add(competitor);
						competitor.Heat = competition.Heats.Count - 1;
					}
				}
			} //end loop over competitions in group
		} //end function AssignHeats

		bool _lanesChanged = false;
		bool _changedByCode = false;
		private void tcLanes_TextChanged(object sender, EventArgs e)
		{
			if (_changedByCode)
			{
				_changedByCode = false;
				return;
			}
			if (Competition == null)
				return;
			Competition.LaneCount = Sport.Common.Tools.CIntDef(tcLanes.Value, 0);
			_lanesChanged = true;
		}

		private void GenerateReports()
		{
			Sport.Championships.Team[] arrSelectedTeams = phaseGroupGridPanel.GetSelectedTeams();


			Forms.ChooseCompetitionReportDialog objDialog =
				new Forms.ChooseCompetitionReportDialog(this.Competition,
				this.Group, this.Competitor, this.Team, this.Phase, arrSelectedTeams);
			if (objDialog.ShowDialog() == DialogResult.OK)
			{
				//get selected report:
				Documents.CompetitionReportType reportType = objDialog.SelectedReport;

				//build proper data:
				object data = null;
				switch (reportType)
				{
					case Documents.CompetitionReportType.CompetitionCompetitorsReport:
					case Documents.CompetitionReportType.RefereeReport:
						data = this.Competition;
						break;
					case Documents.CompetitionReportType.CompetitorVoucher:
						data = new object[] { this.Competition, 
												InputCompetitorsByPosition() };
						break;
					case Documents.CompetitionReportType.TeamVoucher_School:
					case Documents.CompetitionReportType.TeamVoucher_Student:
						data = new object[] { this.Competition, 
												InputTeamsByPosition() };
						break;
					case Documents.CompetitionReportType.TeamCompetitorsReport:
						data = this.Team;
						break;
					case Documents.CompetitionReportType.TeamFullReport:
						if (arrSelectedTeams != null && arrSelectedTeams.Length > 1)
							data = arrSelectedTeams;
						else
							data = this.Team;
						break;
					case Documents.CompetitionReportType.MultiCompetitionReport:
					case Documents.CompetitionReportType.GroupTeamsReport:
						data = this.Group;
						break;
					case Documents.CompetitionReportType.ClubCompetitionsReport:
						data = this.Phase;
						break;
				}

				//got anything?
				if (data == null)
					return;

				//initialize report document:
				Documents.CompetitionReports competitionDoc = new Documents.CompetitionReports(reportType, data);

				//print:
				try
				{
					competitionDoc.Print();
				}
				catch (Exception ex)
				{
					Sport.UI.MessageBox.Error("אירעה שגיאה בעת יצירת הדו\"ח אנא וודא נכונות הקלט", "הדפסה");
					System.Windows.Forms.MessageBox.Show("Message: " + ex.Message + "\nStack: " + ex.StackTrace, "Details", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
				}
			}
		}

		private ArrayList InputByPosition(Hashtable tblPositions, string title)
		{
			//get all positions:
			int minPosition = 999;
			int maxPosition = -1;
			foreach (int curPosition in tblPositions.Keys)
			{
				if (curPosition >= 0)
				{
					//min?
					if (curPosition < minPosition)
						minPosition = curPosition;

					//max?
					if (curPosition > maxPosition)
						maxPosition = curPosition;
				}
			}

			//got valid positions?
			if ((minPosition > 998) || (maxPosition < 0))
				return null;

			//initialize result array:
			ArrayList result = new ArrayList();

			//get position range:
			int positionFrom = -1;
			int positionTo = -1;
			InputPositionRange(ref positionFrom, ref positionTo,
				minPosition, maxPosition, title);

			//got anything?
			if ((positionFrom < 0) || (positionTo < 0))
				return null;

			//put positions into array:
			ArrayList arrPositions = new ArrayList();
			foreach (int curPosition in tblPositions.Keys)
			{
				arrPositions.Add(curPosition);
			}

			//sort:
			arrPositions.Sort();

			//insert selected competitors:
			foreach (int curPosition in arrPositions)
			{
				if ((curPosition >= positionFrom) && (curPosition <= positionTo))
					result.AddRange((tblPositions[curPosition] as ArrayList).ToArray());
			}

			//done.
			return result;
		}

		private ArrayList InputCompetitorsByPosition()
		{
			//get selected competition:
			Sport.Championships.Competition competition = this.Competition;

			//got anything?
			if ((competition == null) || (competition.Competitors == null) || (competition.Competitors.Count == 0))
				return null;

			//make array of competitors and offline players.
			ArrayList arrCompetitors = new ArrayList(competition.Competitors);
			arrCompetitors.AddRange(competition.GetOfflinePlayers());

			//build table:
			Hashtable table = new Hashtable();
			foreach (object oComp in arrCompetitors)
			{
				int curPosition = -1;
				if (oComp is Sport.Championships.Competitor)
					curPosition = (oComp as Sport.Championships.Competitor).ResultPosition;
				else if (oComp is Sport.Entities.OfflinePlayer)
					curPosition = (oComp as Sport.Entities.OfflinePlayer).Rank;
				if (curPosition < 0)
					continue;
				if (table[curPosition] == null)
					table[curPosition] = new ArrayList();
				(table[curPosition] as ArrayList).Add(oComp);
			}


			//input competitors:
			ArrayList competitors = InputByPosition(table, "בחר דירוג משתתפים");

			//done.
			return competitors;
		} //end function InputCompetitorsByPosition

		private Sport.Championships.CompetitionTeam[] InputTeamsByPosition()
		{
			//get selected group:
			Sport.Championships.CompetitionGroup group = this.Group;

			//got anything?
			if ((group == null) || (group.Teams == null) || (group.Teams.Count == 0))
				return null;

			//build table:
			Hashtable table = new Hashtable();
			foreach (Sport.Championships.CompetitionTeam team in group.Teams)
			{
				int curPosition = team.Position;
				if (curPosition < 0)
					continue;
				if (table[curPosition] == null)
					table[curPosition] = new ArrayList();
				(table[curPosition] as ArrayList).Add(team);
			}

			//input teams:
			ArrayList teams = InputByPosition(table, "בחר דירוג קבוצות");

			//got anything?
			if (teams == null)
				return null;

			//done.
			return (Sport.Championships.CompetitionTeam[])
				teams.ToArray(typeof(Sport.Championships.CompetitionTeam));
		} //end function InputTeamsByPosition

		private void InputPositionRange(ref int positionFrom, ref int positionTo,
			int minPosition, int maxPosition, string title)
		{
			//build input dialog:
			Sport.UI.Dialogs.GenericEditDialog objDialog =
				new Sport.UI.Dialogs.GenericEditDialog(title);

			//make array:
			object[] values = new object[] { minPosition + 1, maxPosition + 1 };

			//add minimum and maximum input items:
			Size size = new Size(60, 20);
			objDialog.Items.Add("ממקום:", Sport.UI.Controls.GenericItemType.Number,
				minPosition + 1, values, size);
			objDialog.Items.Add("עד מקום:", Sport.UI.Controls.GenericItemType.Number,
				minPosition + 3, values, size);

			//default values:
			positionFrom = -1;
			positionTo = -1;

			//let user choose:
			if (objDialog.ShowDialog() == DialogResult.OK)
			{
				//user confirmed, read selected items:
				if (objDialog.Items[0].Value != null)
					positionFrom = ((int)((double)objDialog.Items[0].Value)) - 1;
				if (objDialog.Items[1].Value != null)
					positionTo = ((int)((double)objDialog.Items[1].Value)) - 1;

				//illegal?
				if (positionTo < positionFrom)
				{
					positionFrom = -1;
					positionTo = -1;
				}
			}
		} //end function InputPositionRange

		protected override int ImportResults()
		{
			int competitorsCount = base.ImportResults();
			if (competitorsCount > 0)
			{
				this.competitorsGridPanel.Rebuild();
			}
			return competitorsCount;
		}

		protected override bool CanEditGroup()
		{
			return CanEdit();
		}

		protected override bool CanEditStructure()
		{
			return CanEdit();
		}

		private bool CanEdit()
		{
			if (this.GotOpenResultForm)
			{
				Sport.UI.MessageBox.Error("לא ניתן לערוך כאשר מסך קליטת תוצאות פתוח", "בניית אליפות");
				ShowResultForm();
				return false;
			}
			else
			{
				bool canEdit = base.CanEditGroup();
				if (canEdit)
				{
					if (this.Group != null)
					{
						_preEditCompetitions = this.Group.MapCompetitions();
						if (this.Competition != null)
							_preEditCompetitors = this.Competition.MapCompetitors();
					}
				}
				return canEdit;
			}
		}

		private void btnResetCustomPosition_Click(object sender, EventArgs e)
		{
			if (this.Competition == null)
				return;

			if (this.Competition.Competitors == null || this.Competition.Competitors.Count == 0)
				return;

			Sport.UI.Dialogs.WaitForm.ShowWait("מאפס דירוג אנא המתן...", true);

			foreach (Sport.Championships.Competitor competitor in this.Competition.Competitors)
				competitor.SetCustomPosition(-1);
			this.Competition.CalculateCompetitorsPosition();

			Sport.UI.Dialogs.WaitForm.HideWait();
			Sport.UI.MessageBox.Success("דירוג משתתפים עבר איפוס", "בניית אליפות");

			this.SortByPosition();
		}

		private void btnDeleteAllResults_Click(object sender, EventArgs e)
		{
			if (this.Competition == null)
				return;

			if (this.Competition.Competitors == null || this.Competition.Competitors.Count == 0)
				return;

			if (this.GotOpenResultForm)
			{
				Sport.UI.MessageBox.Error("לא ניתן להסיר מתמודד כאשר חלון הכנסת תוצאות פתוח", "מחיקת מתמודד");
				ShowResultForm();
				return;
			}

			if (Sport.UI.MessageBox.Ask("האם למחוק את כל המשתתפים?", false) == true)
			{
				Sport.UI.Dialogs.WaitForm.ShowWait("מוחק משתתפים, אנא המתן...", true);
				List<Sport.Championships.Competitor> originalCompetitors = new List<Sport.Championships.Competitor>();
				foreach (Sport.Championships.Competitor competitor in this.Competition.Competitors)
				{
					originalCompetitors.Add(competitor);
				}
				this.Competition.Competitors.Clear();
				string saveResult = "";
				try
				{
					this.Championship.Edit();
					saveResult = this.Championship.Save();
				}
				catch (Exception ex)
				{
					saveResult = "שגיאה כללית בעת שמירה:\n" + ex.Message + "\n" + ex.StackTrace;
				}
				Sport.UI.Dialogs.WaitForm.HideWait();
				if (saveResult.Length == 0)
				{
					Sport.UI.MessageBox.Success("משתתפים נמחקו בהצלחה", "בניית אליפות");
				}
				else
				{
					Sport.UI.MessageBox.Error(saveResult, "בניית אליפות");
					foreach (Sport.Championships.Competitor competitor in this.Competition.Competitors)
						this.Competition.Competitors.Add(competitor);
				}
				competitorsGridPanel.Rebuild();
			}
		}

		private void btnRecalculateScore_Click(object sender, EventArgs e)
		{
			if (this.Competition == null)
				return;

			if (this.Competition.Competitors == null || this.Competition.Competitors.Count == 0)
				return;

			if (this.GotOpenResultForm)
			{
				Sport.UI.MessageBox.Error("לא ניתן לחשב ניקוד מחדש כאשר חלון הכנסת תוצאות פתוח", "חישוב ניקוד מחדש");
				ShowResultForm();
				return;
			}

			if (Sport.UI.MessageBox.Ask("האם לבצע חישוב מחדש של הניקוד והדירוגים?", false) == true)
			{
				Sport.UI.Dialogs.WaitForm.ShowWait("מבצע חישוב מחדש, אנא המתן...", true);
				using (CompetitionResultForm dialog = new CompetitionResultForm(Competition))
				{
					dialog.SetResults();
				}
				this.SortByPosition();
				Sport.UI.Dialogs.WaitForm.HideWait();
			}
		}

		private void ShowResultForm()
		{
			if (resultForms.Count == 0)
				return;
			if (_activeResultForm == null)
				_activeResultForm = resultForms[0];
			_activeResultForm.Show();
			_activeResultForm.Focus();
		}

		private void CompetitionChampionshipEditorView_Deactivated(object sender, EventArgs e)
		{
			resultForms.ForEach(r => r.Hide());
		}

		private void CompetitionChampionshipEditorView_Activated(object sender, EventArgs e)
		{
			ShowResultForm();
		}

		protected override Sport.Documents.Document CreateRankingDocument(System.Drawing.Printing.PrinterSettings settings)
		{
			if (Championship == null || Phase == null)
				return null;

			Cursor cur = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;

			RankingSectionObject rankingSectionObject = new RankingSectionObject(true);
			rankingSectionObject.Phase = Phase;
			BaseDocumentBuilder baseDocumentBuilder = new BaseDocumentBuilder(settings);
			Sport.Documents.DocumentBuilder documentBuilder = baseDocumentBuilder.CreateDocumentBuilder(Championship.Name + " - " + Phase.Name, rankingSectionObject, true);
			Sport.Documents.Document document = documentBuilder.CreateDocument();

			Cursor.Current = cur;
			return document;
		}

		private void competitorsGridPanel_CompetitorsDeleted(object sender, Sport.Championships.Competitor[] competitors)
		{
			if (this.GotOpenResultForm)
			{
				Sport.UI.MessageBox.Error("לא ניתן להסיר מתמודדים כאשר חלון הכנסת תוצאות פתוח", "מחיקת מתמודדים");
				ShowResultForm();
				return;
			}

			foreach (Sport.Championships.Competitor competitor in competitors)
			{
				Sport.Championships.Competition competition = competitor.Competition;
				if (Sport.UI.MessageBox.Ask("האם להסיר את השחקן '" + competitor.Name +
					"' מהתחרות '" + competition.SportField.Name + "'?", false))
				{
					competition.Competitors.RemoveAt(competitor.Index);
				}
			}

			competitorsGridPanel.Rebuild();
		}
	}
}
