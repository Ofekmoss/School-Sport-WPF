using System;
using System.Linq;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using Sport.UI;
using Sportsman.Documents;
using System.Collections.Generic;

namespace Sportsman.Producer
{
	[Sport.UI.ViewPermissions(Sport.Types.Permission.Championships)]
	public class MatchChampionshipEditorView : ChampionshipEditorView
	{
		#region Edit Panels

		#region Group Panel

		private Sport.UI.Controls.ThemeButton tbBuildMatches;
		private Sport.UI.Controls.ThemeButton tbArrangeNumbers;
		private Sport.UI.Controls.ThemeButton tbArrangeByDate;

		private System.Windows.Forms.CheckBox cbKeepDates;

		private void tbBuildMatches_Click(object sender, System.EventArgs e)
		{
			Sport.Championships.MatchGroup group = Group;

			if (group != null)
			{
				Sport.Data.EntityField rangeField = Sport.Entities.GameBoard.Type.Fields[(int)Sport.Entities.GameBoard.Fields.Range];
				Sport.Data.Entity[] boards = Sport.Entities.GameBoard.Type.GetEntities(null);
				List<Sport.Data.Entity> matchingBoards = boards.ToList().FindAll(board =>
				{
					string boardText = rangeField.GetText(board);
					return Sport.Common.RangeArray.Parse(boardText)[group.Teams.Count];
					
				});

				if (matchingBoards.Count == 0)
				{
					Sport.UI.MessageBox.Show("אין לוח משחקים עבור כמות הקבוצות בבית");
					return;
				}

				Sport.UI.Dialogs.GenericEditDialog ged = new Sport.UI.Dialogs.GenericEditDialog("בניה מתבנית");
				ged.Items.Add("לוח משחקים:", Sport.UI.Controls.GenericItemType.Selection, null, matchingBoards.ToArray());
				ged.Items.Add("סיבובים:", Sport.UI.Controls.GenericItemType.Number,
					1, new object[] { 1d, 1d });
				ged.Items[1].Enabled = false;
				ged.ValueChanged += new EventHandler(BuildMatchesChanged);
				lastBoard = -1;

				if (ged.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					Sport.Data.Entity board = ged.Items[0].Value as Sport.Data.Entity;
					int rounds = (int)(double)ged.Items[1].Value;
					if (board != null)
					{
						Sport.Producer.GameBoard gameBoard = new Sport.Producer.GameBoard();
						gameBoard.Load(board.Id);
						gameBoard.CreateMatches(group, rounds);
						matchesGridPanel.Rebuild();
					}
				}
			}
		}

		private int lastBoard = -1;

		private void BuildMatchesChanged(object sender, EventArgs e)
		{
			Sport.UI.Dialogs.GenericEditDialog ged = sender as
				Sport.UI.Dialogs.GenericEditDialog;

			Sport.Data.Entity board = ged.Items[0].Value as Sport.Data.Entity;

			if ((board == null && lastBoard != -1) || (board != null && board.Id != lastBoard))
			{
				Sport.Producer.GameBoardItem gameBoardItem = null;
				if (board != null)
				{
					Sport.Producer.GameBoard gameBoard = new Sport.Producer.GameBoard();
					gameBoard.Load(board.Id);
					gameBoardItem = gameBoard.GetGameBoardItem(Group.Teams.Count);
				}

				if (gameBoardItem == null)
				{
					ged.Items[1].Enabled = false;
					ged.Confirmable = false;
				}
				else
				{
					ged.Items[1].Enabled = true;
					ged.Items[1].Values = Sport.UI.Controls.GenericItem.NumberValues(1d, (double)gameBoardItem.Rounds, 5, 0);
					ged.Items[1].Value = gameBoardItem.Rounds;
					ged.Confirmable = true;
				}

				lastBoard = board == null ? -1 : board.Id;
			}
		}

		#region Round

		private System.Windows.Forms.GroupBox gbRound;
		private System.Windows.Forms.TextBox tbRoundName;
		private Sport.UI.Controls.ThemeButton tbMoveRound;
		private Sport.UI.Controls.ThemeButton tbRemoveRound;
		private Sport.UI.Controls.ThemeButton tbAddRound;

		private void tbRoundName_TextChanged(object sender, System.EventArgs e)
		{
			Sport.Championships.Round round = Round;
			if (round != null && !selecting)
			{
				round.Name = tbRoundName.Text;
				matchesGridPanel.Refresh();
			}
		}

		private void tbMoveRound_Click(object sender, System.EventArgs e)
		{
			Sport.Championships.Round round = Round;
			if (round != null)
			{
				Sport.UI.Dialogs.GenericEditDialog ged =
					new Sport.UI.Dialogs.GenericEditDialog("בחר מיקום");

				int current = round.Index;

				ged.Items.Add(Sport.UI.Controls.GenericItemType.Number,
					current + 1, new object[] { 1d, (double)Group.Rounds.Count });

				if (ged.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					int pos = (int)(double)ged.Items[0].Value;
					int index = pos - 1;

					if (index == current)
						return;

					Group.Rounds.RemoveAt(current);
					Group.Rounds.Insert(index, round);
					matchesGridPanel.Rebuild();
				}
			}
		}

		private void tbAddRound_Click(object sender, System.EventArgs e)
		{
			if (Group != null)
			{
				Sport.Championships.Round round =
					new Sport.Championships.Round("סיבוב " + (Group.Rounds.Count + 1).ToString());

				Group.Rounds.Add(round);
				matchesGridPanel.Rebuild();

				Round = round;
			}
		}

		private void tbRemoveRound_Click(object sender, System.EventArgs e)
		{
			if (Round != null)
			{
				if (!Sport.UI.MessageBox.Ask("פעולה זאת תמחוק את הסיבוב '" +
					Round.Name + "' ואת כל המשחקים שבו, האם להמשיך?", false))
					return;

				int index = Round.Index;
				if (index > 0)
					Round = Group.Rounds[index - 1];

				Group.Rounds.RemoveAt(index);
				matchesGridPanel.Rebuild();
			}
		}

		#endregion

		#region Cycle

		private System.Windows.Forms.GroupBox gbCycle;
		private System.Windows.Forms.TextBox tbCycleName;
		private Sport.UI.Controls.ThemeButton tbMoveCycle;
		private Sport.UI.Controls.ThemeButton tbRemoveCycle;
		private Sport.UI.Controls.ThemeButton tbAddCycle;

		private void tbCycleName_TextChanged(object sender, System.EventArgs e)
		{
			Sport.Championships.Cycle cycle = Cycle;
			if (cycle != null && !selecting)
			{
				cycle.Name = tbCycleName.Text;
				matchesGridPanel.Refresh();
			}
		}

		private void tbMoveCycle_Click(object sender, System.EventArgs e)
		{
			Sport.Championships.Cycle cycle = Cycle;
			if (cycle != null)
			{
				Sport.UI.Dialogs.GenericEditDialog ged =
					new Sport.UI.Dialogs.GenericEditDialog("בחר מיקום");

				int current = cycle.Index;

				ged.Items.Add(Sport.UI.Controls.GenericItemType.Number,
					current + 1, new object[] { 1d, (double)Round.Cycles.Count });

				if (ged.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					int pos = (int)(double)ged.Items[0].Value;
					int index = pos - 1;

					if (index == current)
						return;

					Round.Cycles.RemoveAt(current);
					Round.Cycles.Insert(index, cycle);
					matchesGridPanel.Rebuild();
				}
			}
		}

		private void tbAddCycle_Click(object sender, System.EventArgs e)
		{
			if (Round != null)
			{
				Sport.Championships.Cycle cycle =
					new Sport.Championships.Cycle("מחזור " + (Round.Cycles.Count + 1).ToString());

				Round.Cycles.Add(cycle);
				matchesGridPanel.Rebuild();

				Cycle = cycle;
			}
		}

		private void tbRemoveCycle_Click(object sender, System.EventArgs e)
		{
			if (Cycle != null)
			{
				if (!Sport.UI.MessageBox.Ask("פעולה זאת תמחוק את המחזור '" +
					Cycle.Name + "' ואת כל המשחקים שבו, האם להמשיך?", false))
					return;

				int index = Cycle.Index;
				Sport.Championships.Round round = Round;

				if (index > 0)
					matchesGridPanel.SelectedRow = round.Cycles[index - 1];
				else if (round.Cycles.Count - 1 > index)
					matchesGridPanel.SelectedRow = round.Cycles[index + 1];
				else
					matchesGridPanel.SelectedRow = round;

				round.Cycles.RemoveAt(index);
				matchesGridPanel.Rebuild();
			}
		}

		#endregion

		#region Match

		private System.Windows.Forms.GroupBox gbMatch;
		private Sport.UI.Controls.ButtonBox bbMatch;
		private Sport.UI.Controls.ThemeButton tbMatchUp;
		private Sport.UI.Controls.ThemeButton tbMatchDown;
		private Sport.UI.Controls.ThemeButton tbRemoveMatch;
		private Sport.UI.Controls.ThemeButton tbAddMatch;

		private void bbMatch_ValueSelect(object sender, System.EventArgs e)
		{
			Sport.Championships.Match match = Match;
			if (match != null)
			{
				int index = match.Index;
				match = EditMatch(Group, match);
				if (match != null)
				{
					Cycle.Matches[index] = match;
					matchesGridPanel.Rebuild();
					Match = match;
				}
			}
		}

		private void MoveMatch(bool blnMoveUp)
		{
			//get match:
			Sport.Championships.Match match = Match;

			//got anything?
			if (match == null)
				return;

			//get match index:
			int index = match.Index;

			//get match tournament:
			int tournament = match.Tournament;

			//fix index if needed..
			if (matchesGridPanel.GroupTournaments)
			{
				int tempIndex = 0;
				foreach (Sport.Championships.Match curMatch in match.Cycle.Matches)
				{
					if (curMatch.Index == index)
						break;
					if (curMatch.Tournament <= tournament)
						tempIndex++;
				}
				if (tempIndex != index)
					index = tempIndex;
			}

			//get cycle:
			Sport.Championships.Cycle cycle = match.Cycle;

			//get matches count:
			int matchCount = cycle.Matches.Count;
			if (matchesGridPanel.GroupTournaments)
			{
				matchCount = 0;
				foreach (Sport.Championships.Match curMatch in cycle.Matches)
					matchCount += (curMatch.Tournament == tournament) ? 1 : 0;
			}

			//tournament index:
			int matchTourIndex = 0;
			foreach (Sport.Championships.Match curMatch in match.Cycle.Matches)
			{
				if (curMatch.Index == match.Index)
					break;
				if (curMatch.Tournament == match.Tournament)
					matchTourIndex++;
			}

			//need to move cycle up or down?
			int leftIndex = (matchesGridPanel.GroupTournaments) ? matchTourIndex : index;
			int rightIndex = (blnMoveUp) ? (0) : (matchCount - 1);
			bool blnMoveOutside = (leftIndex == rightIndex);
			if (blnMoveOutside)
			{
				//move cycle/tournament up or down.

				//decide what to do according to display
				if (matchesGridPanel.GroupTournaments)
				{
					if (blnMoveUp)
					{
						// Moving to previous tournament
						// Checking if in no tournament
						if (tournament == -1)
						{
							if (!match.ChangeCycle(cycle.GetPrevCycle(), true))
								return;
						}
						else
						{
							//got tournament, move up:
							match.Tournament--;
						} //end if match has tournament
					}
					else
					{
						// Checking if in last tournament
						if (match.Tournament == cycle.Tournaments.Count - 1)
						{
							// Checking if last match in tournament
							if (matchCount == 1)
							{
								if (!match.ChangeCycle(cycle.GetNextCycle(), false))
									return;
							}
							else
							{
								// Moving match to a new tournament
								match.Tournament = cycle.Tournaments.Add(new Sport.Championships.Tournament(cycle.Round.Group.GetMaxTournamentNumber() + 1));
							}
						}
						else
						{
							//got tournament, move down:
							int newTournament = match.Tournament + 1;

							//change index, if needed..
							int newIndex = index;
							foreach (Sport.Championships.Match curMatch in cycle.Matches)
							{
								if ((curMatch.Index != match.Index) &&
									(curMatch.Tournament == newTournament))
								{
									newIndex = curMatch.Index;
									break;
								}
							}
							match.Tournament = newTournament;
							if (newIndex < match.Index)
								match.ChangeIndex(newIndex);
						}
					}
				} //end if tournaments display
				else if (matchesGridPanel.GroupCycles)
				{
					// Moving to previous cycle

					//get new cycle:
					Sport.Championships.Cycle newCycle =
						(blnMoveUp) ? cycle.GetPrevCycle() : cycle.GetNextCycle();

					//change the cycle:
					if (!match.ChangeCycle(newCycle, blnMoveUp))
						return;

					//move to proper index as well.
					if (!blnMoveUp)
					{
					}
				}
				else if (matchesGridPanel.GroupRounds)
				{
					// Moving to previous round

					//get new cycle:
					Sport.Championships.Cycle newCycle = null;
					if (blnMoveUp)
						newCycle = cycle.Round.Cycles[0].GetPrevCycle();
					else
						newCycle = cycle.Round.Cycles[cycle.Round.Cycles.Count - 1].GetNextCycle();

					//change the cycle:
					if (!match.ChangeCycle(newCycle, blnMoveUp))
						return;
				}
				else
				{
					// doing nothing
					return;
				}

				//need to remove tournament?
				if ((matchCount == 1) && (tournament >= 0))
					cycle.Tournaments.RemoveAt(tournament);
			}
			else
			{
				//change match index
				int diff = (blnMoveUp) ? -1 : 1;
				int newIndex = index + diff;
				if (matchesGridPanel.GroupTournaments)
				{
					int lowerBound = (blnMoveUp) ? 1 : 0;
					int upperBound = (blnMoveUp) ? cycle.Matches.Count :
						cycle.Matches.Count - 1;
					while ((newIndex >= lowerBound) && (newIndex < upperBound) &&
						(cycle.Matches[newIndex].Tournament != tournament))
					{
						newIndex += diff;
					}
				}
				match.ChangeIndex(newIndex, cbKeepDates.Checked);
			}

			//done.
			_match = match.Index;
			_cycle = match.Cycle.Index;
			_tournament = match.Tournament;
			_round = match.Cycle.Round.Index;
			matchesGridPanel.Rebuild();
			OnMatchChange();
		}

		private void tbMatchUp_Click(object sender, System.EventArgs e)
		{
			MoveMatch(true);
		}

		private void tbMatchDown_Click(object sender, System.EventArgs e)
		{
			MoveMatch(false);
		}

		private void tbRemoveMatch_Click(object sender, System.EventArgs e)
		{
			Sport.Championships.Match match = Match;
			if (match != null)
			{
				if (!Sport.UI.MessageBox.Ask("האם למחוק את המשחק '"
					+ match.ToString() + "'?", false))
					return;

				int index = match.Index;
				Sport.Championships.Cycle cycle = Cycle;
				if (index > 0)
					matchesGridPanel.SelectedRow = cycle.Matches[index - 1];
				else if (cycle.Matches.Count - 1 > index)
					matchesGridPanel.SelectedRow = cycle.Matches[index + 1];
				else
					matchesGridPanel.SelectedRow = cycle;


				cycle.Matches.RemoveAt(index);
				matchesGridPanel.Rebuild();
			}
		}

		private void tbAddMatch_Click(object sender, System.EventArgs e)
		{
			Sport.Championships.Cycle cycle = Cycle;
			if (cycle != null)
			{
				int index = _match == -1 ? cycle.Matches.Count : _match + 1;

				Sport.Championships.Match match = EditMatch(Group, null);

				if (match != null)
				{
					if (index < 0)
						index = 0;
					if (index > cycle.Matches.Count)
						index = cycle.Matches.Count;
					cycle.Matches.Insert(index, match);
					matchesGridPanel.Rebuild();
					Match = match;
				}
			}
		}

		private Sport.Championships.Match EditMatch(Sport.Championships.MatchGroup group, Sport.Championships.Match match)
		{
			MatchTeamsSelectionForm mtsf = new MatchTeamsSelectionForm(group, match);

			if (mtsf.ShowDialog() == DialogResult.OK)
			{
				return mtsf.Match;
			}

			return null;
		}

		#endregion

		#endregion

		#region Structure Panel

		private Sport.UI.Controls.ThemeButton tbBuild;

		private void tbBuild_Click(object sender, System.EventArgs e)
		{
			if (Championship.Phases.Count > 0)
			{
				if (!Sport.UI.MessageBox.Ask("פעולה זאת תגרום למחיקה כל השלבים וכל המשחקים. האם להמשיך?",
					System.Windows.Forms.MessageBoxIcon.Warning, false))
					return;
			}

			int phase = Phase == null ? -1 : Phase.Index;

			if (PhaseBuildForm.BuildChampionship(Championship as Sport.Championships.MatchChampionship))
			{
				ResetPhases();
				Phase = phase == -1 || phase >= Championship.Phases.Count ? null : Championship.Phases[phase];
			}
		}

		#endregion

		#endregion

		#region Initialization

		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(MatchChampionshipEditorView));
			this.gbCycle = new System.Windows.Forms.GroupBox();
			this.tbMoveCycle = new Sport.UI.Controls.ThemeButton();
			this.tbCycleName = new System.Windows.Forms.TextBox();
			this.tbRemoveCycle = new Sport.UI.Controls.ThemeButton();
			this.tbAddCycle = new Sport.UI.Controls.ThemeButton();
			this.gbMatch = new System.Windows.Forms.GroupBox();
			this.tbMatchDown = new Sport.UI.Controls.ThemeButton();
			this.tbMatchUp = new Sport.UI.Controls.ThemeButton();
			this.bbMatch = new Sport.UI.Controls.ButtonBox();
			this.tbRemoveMatch = new Sport.UI.Controls.ThemeButton();
			this.tbAddMatch = new Sport.UI.Controls.ThemeButton();
			this.gbRound = new System.Windows.Forms.GroupBox();
			this.tbMoveRound = new Sport.UI.Controls.ThemeButton();
			this.tbRoundName = new System.Windows.Forms.TextBox();
			this.tbRemoveRound = new Sport.UI.Controls.ThemeButton();
			this.tbAddRound = new Sport.UI.Controls.ThemeButton();
			this.tbBuildMatches = new Sport.UI.Controls.ThemeButton();
			this.tbArrangeNumbers = new Sport.UI.Controls.ThemeButton();
			this.tbArrangeByDate = new Sport.UI.Controls.ThemeButton();
			this.cbKeepDates = new System.Windows.Forms.CheckBox();
			this.tbBuild = new Sport.UI.Controls.ThemeButton();
			this.panelGroupEdit.SuspendLayout();
			this.panelStructureEdit.SuspendLayout();
			this.gbCycle.SuspendLayout();
			this.gbMatch.SuspendLayout();
			this.gbRound.SuspendLayout();
			// 
			// panelGroupView
			// 
			this.panelGroupView.Location = new System.Drawing.Point(0, 235);
			this.panelGroupView.Name = "panelGroupView";
			this.panelGroupView.Size = new System.Drawing.Size(807, 178);
			// 
			// tbSetResults
			// 
			this.tbSetResults.Name = "tbSetResults";
			// 
			// tbGroupView
			// 
			this.tbGroupView.Image = ((System.Drawing.Image)(resources.GetObject("tbGroupView.Image")));
			this.tbGroupView.Name = "tbGroupView";
			this.tbGroupView.Size = new System.Drawing.Size(71, 17);
			this.tbGroupView.Text = "משחקים";
			// 
			// tbEditGroup
			// 
			this.tbEditGroup.Image = ((System.Drawing.Image)(resources.GetObject("tbEditGroup.Image")));
			this.tbEditGroup.Name = "tbEditGroup";
			this.tbEditGroup.Size = new System.Drawing.Size(99, 17);
			this.tbEditGroup.Text = "ערוך משחקים";
			// 
			// panelGroupEdit
			// 
			this.panelGroupEdit.Controls.Add(this.gbCycle);
			this.panelGroupEdit.Controls.Add(this.gbMatch);
			this.panelGroupEdit.Controls.Add(this.gbRound);
			this.panelGroupEdit.Controls.Add(this.tbBuildMatches);
			this.panelGroupEdit.Controls.Add(this.tbArrangeNumbers);
			this.panelGroupEdit.Controls.Add(this.tbArrangeByDate);
			this.panelGroupEdit.Controls.Add(this.cbKeepDates);
			this.panelGroupEdit.Name = "panelGroupEdit";
			this.panelGroupEdit.Size = new System.Drawing.Size(807, 96);
			this.panelGroupEdit.Controls.SetChildIndex(this.tbBuildMatches, 0);
			this.panelGroupEdit.Controls.SetChildIndex(this.tbArrangeNumbers, 0);
			this.panelGroupEdit.Controls.SetChildIndex(this.tbArrangeByDate, 0);
			this.panelGroupEdit.Controls.SetChildIndex(this.cbKeepDates, 0);
			this.panelGroupEdit.Controls.SetChildIndex(this.gbRound, 0);
			this.panelGroupEdit.Controls.SetChildIndex(this.gbMatch, 0);
			this.panelGroupEdit.Controls.SetChildIndex(this.gbCycle, 0);
			// 
			// panelStructureEdit
			// 
			this.panelStructureEdit.Controls.Add(this.tbBuild);
			this.panelStructureEdit.Name = "panelStructureEdit";
			this.panelStructureEdit.Size = new System.Drawing.Size(722, 96);
			this.panelStructureEdit.Controls.SetChildIndex(this.tbBuild, 0);
			// 
			// gbCycle
			// 
			this.gbCycle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.gbCycle.Controls.Add(this.tbMoveCycle);
			this.gbCycle.Controls.Add(this.tbCycleName);
			this.gbCycle.Controls.Add(this.tbRemoveCycle);
			this.gbCycle.Controls.Add(this.tbAddCycle);
			this.gbCycle.Location = new System.Drawing.Point(439, 8);
			this.gbCycle.Name = "gbCycle";
			this.gbCycle.Size = new System.Drawing.Size(176, 64);
			this.gbCycle.TabIndex = 48;
			this.gbCycle.TabStop = false;
			this.gbCycle.Text = "מחזור";
			// 
			// tbMoveCycle
			// 
			this.tbMoveCycle.Alignment = System.Drawing.StringAlignment.Center;
			this.tbMoveCycle.AutoSize = true;
			this.tbMoveCycle.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbMoveCycle.Hue = 200F;
			this.tbMoveCycle.Image = ((System.Drawing.Image)(resources.GetObject("tbMoveCycle.Image")));
			this.tbMoveCycle.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbMoveCycle.ImageList = null;
			this.tbMoveCycle.ImageSize = new System.Drawing.Size(12, 12);
			this.tbMoveCycle.Location = new System.Drawing.Point(113, 40);
			this.tbMoveCycle.Name = "tbMoveCycle";
			this.tbMoveCycle.Saturation = 0.5F;
			this.tbMoveCycle.Size = new System.Drawing.Size(55, 17);
			this.tbMoveCycle.TabIndex = 39;
			this.tbMoveCycle.Text = "העבר";
			this.tbMoveCycle.Transparent = System.Drawing.Color.Black;
			this.tbMoveCycle.Click += new System.EventHandler(this.tbMoveCycle_Click);
			// 
			// tbCycleName
			// 
			this.tbCycleName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
				| System.Windows.Forms.AnchorStyles.Right)));
			this.tbCycleName.Location = new System.Drawing.Point(8, 16);
			this.tbCycleName.Name = "tbCycleName";
			this.tbCycleName.Size = new System.Drawing.Size(160, 20);
			this.tbCycleName.TabIndex = 31;
			this.tbCycleName.Text = "";
			this.tbCycleName.TextChanged += new System.EventHandler(this.tbCycleName_TextChanged);
			// 
			// tbRemoveCycle
			// 
			this.tbRemoveCycle.Alignment = System.Drawing.StringAlignment.Center;
			this.tbRemoveCycle.AutoSize = true;
			this.tbRemoveCycle.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbRemoveCycle.Hue = 0F;
			this.tbRemoveCycle.Image = ((System.Drawing.Image)(resources.GetObject("tbRemoveCycle.Image")));
			this.tbRemoveCycle.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbRemoveCycle.ImageList = null;
			this.tbRemoveCycle.ImageSize = new System.Drawing.Size(12, 12);
			this.tbRemoveCycle.Location = new System.Drawing.Point(61, 40);
			this.tbRemoveCycle.Name = "tbRemoveCycle";
			this.tbRemoveCycle.Saturation = 0.9F;
			this.tbRemoveCycle.Size = new System.Drawing.Size(49, 17);
			this.tbRemoveCycle.TabIndex = 30;
			this.tbRemoveCycle.Text = "מחק";
			this.tbRemoveCycle.Transparent = System.Drawing.Color.Black;
			this.tbRemoveCycle.Click += new System.EventHandler(this.tbRemoveCycle_Click);
			// 
			// tbAddCycle
			// 
			this.tbAddCycle.Alignment = System.Drawing.StringAlignment.Center;
			this.tbAddCycle.AutoSize = true;
			this.tbAddCycle.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbAddCycle.Hue = 220F;
			this.tbAddCycle.Image = ((System.Drawing.Image)(resources.GetObject("tbAddCycle.Image")));
			this.tbAddCycle.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbAddCycle.ImageList = null;
			this.tbAddCycle.ImageSize = new System.Drawing.Size(12, 12);
			this.tbAddCycle.Location = new System.Drawing.Point(8, 40);
			this.tbAddCycle.Name = "tbAddCycle";
			this.tbAddCycle.Saturation = 0.9F;
			this.tbAddCycle.Size = new System.Drawing.Size(50, 17);
			this.tbAddCycle.TabIndex = 29;
			this.tbAddCycle.Text = "חדש";
			this.tbAddCycle.Transparent = System.Drawing.Color.Black;
			this.tbAddCycle.Click += new System.EventHandler(this.tbAddCycle_Click);
			// 
			// gbMatch
			// 
			this.gbMatch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.gbMatch.Controls.Add(this.tbMatchDown);
			this.gbMatch.Controls.Add(this.tbMatchUp);
			this.gbMatch.Controls.Add(this.bbMatch);
			this.gbMatch.Controls.Add(this.tbRemoveMatch);
			this.gbMatch.Controls.Add(this.tbAddMatch);
			this.gbMatch.Location = new System.Drawing.Point(72, 8);
			this.gbMatch.Name = "gbMatch";
			this.gbMatch.Size = new System.Drawing.Size(360, 64);
			this.gbMatch.TabIndex = 47;
			this.gbMatch.TabStop = false;
			this.gbMatch.Text = "משחק";
			// 
			// tbMatchDown
			// 
			this.tbMatchDown.Alignment = System.Drawing.StringAlignment.Center;
			this.tbMatchDown.AutoSize = false;
			this.tbMatchDown.Enabled = false;
			this.tbMatchDown.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbMatchDown.Hue = 200F;
			this.tbMatchDown.Image = ((System.Drawing.Image)(resources.GetObject("tbMatchDown.Image")));
			this.tbMatchDown.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbMatchDown.ImageList = null;
			this.tbMatchDown.ImageSize = new System.Drawing.Size(12, 12);
			this.tbMatchDown.Location = new System.Drawing.Point(8, 16);
			this.tbMatchDown.Name = "tbMatchDown";
			this.tbMatchDown.Saturation = 0.5F;
			this.tbMatchDown.Size = new System.Drawing.Size(51, 17);
			this.tbMatchDown.TabIndex = 33;
			this.tbMatchDown.Text = "הבא";
			this.tbMatchDown.Transparent = System.Drawing.Color.Black;
			this.tbMatchDown.Click += new System.EventHandler(this.tbMatchDown_Click);
			// 
			// tbMatchUp
			// 
			this.tbMatchUp.Alignment = System.Drawing.StringAlignment.Center;
			this.tbMatchUp.AutoSize = false;
			this.tbMatchUp.Enabled = false;
			this.tbMatchUp.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbMatchUp.Hue = 200F;
			this.tbMatchUp.Image = ((System.Drawing.Image)(resources.GetObject("tbMatchUp.Image")));
			this.tbMatchUp.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbMatchUp.ImageList = null;
			this.tbMatchUp.ImageSize = new System.Drawing.Size(12, 12);
			this.tbMatchUp.Location = new System.Drawing.Point(64, 16);
			this.tbMatchUp.Name = "tbMatchUp";
			this.tbMatchUp.Saturation = 0.5F;
			this.tbMatchUp.Size = new System.Drawing.Size(51, 17);
			this.tbMatchUp.TabIndex = 34;
			this.tbMatchUp.Text = "קודם";
			this.tbMatchUp.Transparent = System.Drawing.Color.Black;
			this.tbMatchUp.Click += new System.EventHandler(this.tbMatchUp_Click);
			// 
			// bbMatch
			// 
			this.bbMatch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
				| System.Windows.Forms.AnchorStyles.Right)));
			this.bbMatch.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.bbMatch.Location = new System.Drawing.Point(120, 16);
			this.bbMatch.Name = "bbMatch";
			this.bbMatch.Size = new System.Drawing.Size(232, 40);
			this.bbMatch.TabIndex = 32;
			this.bbMatch.Value = null;
			this.bbMatch.ValueSelector = null;
			this.bbMatch.ValueSelect += new System.EventHandler(this.bbMatch_ValueSelect);
			// 
			// tbRemoveMatch
			// 
			this.tbRemoveMatch.Alignment = System.Drawing.StringAlignment.Center;
			this.tbRemoveMatch.AutoSize = false;
			this.tbRemoveMatch.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbRemoveMatch.Hue = 0F;
			this.tbRemoveMatch.Image = ((System.Drawing.Image)(resources.GetObject("tbRemoveMatch.Image")));
			this.tbRemoveMatch.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbRemoveMatch.ImageList = null;
			this.tbRemoveMatch.ImageSize = new System.Drawing.Size(12, 12);
			this.tbRemoveMatch.Location = new System.Drawing.Point(64, 40);
			this.tbRemoveMatch.Name = "tbRemoveMatch";
			this.tbRemoveMatch.Saturation = 0.9F;
			this.tbRemoveMatch.Size = new System.Drawing.Size(51, 17);
			this.tbRemoveMatch.TabIndex = 30;
			this.tbRemoveMatch.Text = "מחק";
			this.tbRemoveMatch.Transparent = System.Drawing.Color.Black;
			this.tbRemoveMatch.Click += new System.EventHandler(this.tbRemoveMatch_Click);
			// 
			// tbAddMatch
			// 
			this.tbAddMatch.Alignment = System.Drawing.StringAlignment.Center;
			this.tbAddMatch.AutoSize = false;
			this.tbAddMatch.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbAddMatch.Hue = 220F;
			this.tbAddMatch.Image = ((System.Drawing.Image)(resources.GetObject("tbAddMatch.Image")));
			this.tbAddMatch.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbAddMatch.ImageList = null;
			this.tbAddMatch.ImageSize = new System.Drawing.Size(12, 12);
			this.tbAddMatch.Location = new System.Drawing.Point(8, 40);
			this.tbAddMatch.Name = "tbAddMatch";
			this.tbAddMatch.Saturation = 0.9F;
			this.tbAddMatch.Size = new System.Drawing.Size(51, 17);
			this.tbAddMatch.TabIndex = 29;
			this.tbAddMatch.Text = "חדש";
			this.tbAddMatch.Transparent = System.Drawing.Color.Black;
			this.tbAddMatch.Click += new System.EventHandler(this.tbAddMatch_Click);
			// 
			// gbRound
			// 
			this.gbRound.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.gbRound.Controls.Add(this.tbMoveRound);
			this.gbRound.Controls.Add(this.tbRoundName);
			this.gbRound.Controls.Add(this.tbRemoveRound);
			this.gbRound.Controls.Add(this.tbAddRound);
			this.gbRound.Location = new System.Drawing.Point(621, 8);
			this.gbRound.Name = "gbRound";
			this.gbRound.Size = new System.Drawing.Size(176, 64);
			this.gbRound.TabIndex = 46;
			this.gbRound.TabStop = false;
			this.gbRound.Text = "סיבוב";
			// 
			// tbMoveRound
			// 
			this.tbMoveRound.Alignment = System.Drawing.StringAlignment.Center;
			this.tbMoveRound.AutoSize = true;
			this.tbMoveRound.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbMoveRound.Hue = 200F;
			this.tbMoveRound.Image = ((System.Drawing.Image)(resources.GetObject("tbMoveRound.Image")));
			this.tbMoveRound.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbMoveRound.ImageList = null;
			this.tbMoveRound.ImageSize = new System.Drawing.Size(12, 12);
			this.tbMoveRound.Location = new System.Drawing.Point(113, 40);
			this.tbMoveRound.Name = "tbMoveRound";
			this.tbMoveRound.Saturation = 0.5F;
			this.tbMoveRound.Size = new System.Drawing.Size(55, 17);
			this.tbMoveRound.TabIndex = 38;
			this.tbMoveRound.Text = "העבר";
			this.tbMoveRound.Transparent = System.Drawing.Color.Black;
			this.tbMoveRound.Click += new System.EventHandler(this.tbMoveRound_Click);
			// 
			// tbRoundName
			// 
			this.tbRoundName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
				| System.Windows.Forms.AnchorStyles.Right)));
			this.tbRoundName.Location = new System.Drawing.Point(8, 16);
			this.tbRoundName.Name = "tbRoundName";
			this.tbRoundName.Size = new System.Drawing.Size(160, 20);
			this.tbRoundName.TabIndex = 31;
			this.tbRoundName.Text = "";
			this.tbRoundName.TextChanged += new System.EventHandler(this.tbRoundName_TextChanged);
			// 
			// tbRemoveRound
			// 
			this.tbRemoveRound.Alignment = System.Drawing.StringAlignment.Center;
			this.tbRemoveRound.AutoSize = true;
			this.tbRemoveRound.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbRemoveRound.Hue = 0F;
			this.tbRemoveRound.Image = ((System.Drawing.Image)(resources.GetObject("tbRemoveRound.Image")));
			this.tbRemoveRound.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbRemoveRound.ImageList = null;
			this.tbRemoveRound.ImageSize = new System.Drawing.Size(12, 12);
			this.tbRemoveRound.Location = new System.Drawing.Point(61, 40);
			this.tbRemoveRound.Name = "tbRemoveRound";
			this.tbRemoveRound.Saturation = 0.9F;
			this.tbRemoveRound.Size = new System.Drawing.Size(49, 17);
			this.tbRemoveRound.TabIndex = 30;
			this.tbRemoveRound.Text = "מחק";
			this.tbRemoveRound.Transparent = System.Drawing.Color.Black;
			this.tbRemoveRound.Click += new System.EventHandler(this.tbRemoveRound_Click);
			// 
			// tbAddRound
			// 
			this.tbAddRound.Alignment = System.Drawing.StringAlignment.Center;
			this.tbAddRound.AutoSize = true;
			this.tbAddRound.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbAddRound.Hue = 220F;
			this.tbAddRound.Image = ((System.Drawing.Image)(resources.GetObject("tbAddRound.Image")));
			this.tbAddRound.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbAddRound.ImageList = null;
			this.tbAddRound.ImageSize = new System.Drawing.Size(12, 12);
			this.tbAddRound.Location = new System.Drawing.Point(8, 40);
			this.tbAddRound.Name = "tbAddRound";
			this.tbAddRound.Saturation = 0.9F;
			this.tbAddRound.Size = new System.Drawing.Size(50, 17);
			this.tbAddRound.TabIndex = 29;
			this.tbAddRound.Text = "חדש";
			this.tbAddRound.Transparent = System.Drawing.Color.Black;
			this.tbAddRound.Click += new System.EventHandler(this.tbAddRound_Click);
			// 
			// tbBuildMatches
			// 
			this.tbBuildMatches.Alignment = System.Drawing.StringAlignment.Center;
			this.tbBuildMatches.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.tbBuildMatches.AutoSize = true;
			this.tbBuildMatches.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbBuildMatches.Hue = 200F;
			this.tbBuildMatches.Image = ((System.Drawing.Image)(resources.GetObject("tbBuildMatches.Image")));
			this.tbBuildMatches.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbBuildMatches.ImageList = null;
			this.tbBuildMatches.ImageSize = new System.Drawing.Size(12, 12);
			this.tbBuildMatches.Location = new System.Drawing.Point(704, 77);
			this.tbBuildMatches.Name = "tbBuildMatches";
			this.tbBuildMatches.Saturation = 0.5F;
			this.tbBuildMatches.Size = new System.Drawing.Size(93, 17);
			this.tbBuildMatches.TabIndex = 45;
			this.tbBuildMatches.Text = "בנה מתבנית";
			this.tbBuildMatches.Transparent = System.Drawing.Color.Black;
			this.tbBuildMatches.Click += new System.EventHandler(this.tbBuildMatches_Click);
			// 
			// tbArrangeNumbers
			// 
			this.tbArrangeNumbers.Alignment = System.Drawing.StringAlignment.Center;
			this.tbArrangeNumbers.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.tbArrangeNumbers.AutoSize = true;
			this.tbArrangeNumbers.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbArrangeNumbers.Hue = 150F;
			this.tbArrangeNumbers.ImageList = null;
			this.tbArrangeNumbers.Location = new System.Drawing.Point(604, 77);
			this.tbArrangeNumbers.Name = "tbArrangeNumbers";
			this.tbArrangeNumbers.Saturation = 0.5F;
			this.tbArrangeNumbers.Size = new System.Drawing.Size(93, 17);
			this.tbArrangeNumbers.TabIndex = 90;
			this.tbArrangeNumbers.Text = "מספור מחדש";
			this.tbArrangeNumbers.Transparent = System.Drawing.Color.Black;
			this.tbArrangeNumbers.Click += new EventHandler(tbArrangeNumbers_Click);
			// 
			// tbArrangeByDate
			// 
			//this.tbArrangeByDate.Alignment = System.Drawing.StringAlignment.
			this.tbArrangeByDate.Anchor = System.Windows.Forms.AnchorStyles.Right; //((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.tbArrangeByDate.AutoSize = true;
			this.tbArrangeByDate.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbArrangeByDate.Hue = 150F;
			this.tbArrangeByDate.ImageList = null;
			this.tbArrangeByDate.Location = new System.Drawing.Point(494, 77);
			this.tbArrangeByDate.Name = "tbArrangeByDate";
			this.tbArrangeByDate.Saturation = 0.5F;
			this.tbArrangeByDate.Size = new System.Drawing.Size(103, 17);
			this.tbArrangeByDate.TabIndex = 91;
			this.tbArrangeByDate.Text = "סדר לפי תאריך";
			this.tbArrangeByDate.Transparent = System.Drawing.Color.Black;
			this.tbArrangeByDate.Enabled = false;
			this.tbArrangeByDate.Visible = false;
			this.tbArrangeByDate.Click += new EventHandler(tbArrageByDate_Click);
			// 
			// cbKeepDates
			// 
			this.cbKeepDates.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			//this.cbKeepDates.TextAlign = ContentAlignment.MiddleCenter;
			this.cbKeepDates.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.cbKeepDates.Location = new System.Drawing.Point(434, 77);
			this.cbKeepDates.Name = "cbKeepDates";
			this.cbKeepDates.Size = new System.Drawing.Size(150, 17);
			this.cbKeepDates.TabIndex = 92;
			this.cbKeepDates.Checked = false;
			this.cbKeepDates.Text = "שמור על תאריכי משחק";
			// 
			// tbBuild
			// 
			this.tbBuild.Alignment = System.Drawing.StringAlignment.Center;
			this.tbBuild.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.tbBuild.AutoSize = true;
			this.tbBuild.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbBuild.Hue = 200F;
			this.tbBuild.Image = ((System.Drawing.Image)(resources.GetObject("tbBuild.Image")));
			this.tbBuild.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbBuild.ImageList = null;
			this.tbBuild.ImageSize = new System.Drawing.Size(12, 12);
			this.tbBuild.Location = new System.Drawing.Point(619, 77);
			this.tbBuild.Name = "tbBuild";
			this.tbBuild.Saturation = 0.5F;
			this.tbBuild.Size = new System.Drawing.Size(93, 17);
			this.tbBuild.TabIndex = 45;
			this.tbBuild.Text = "בנה מתבנית";
			this.tbBuild.Transparent = System.Drawing.Color.Black;
			this.tbBuild.Click += new System.EventHandler(this.tbBuild_Click);
			// 
			// MatchChampionshipEditorView
			// 
			this.Name = "MatchChampionshipEditorView";
			this.Size = new System.Drawing.Size(807, 413);
			this.panelGroupEdit.ResumeLayout(false);
			this.panelStructureEdit.ResumeLayout(false);
			this.gbCycle.ResumeLayout(false);
			this.gbMatch.ResumeLayout(false);
			this.gbRound.ResumeLayout(false);

		}

		private MatchesGridPanel matchesGridPanel;

		public MatchChampionshipEditorView()
		{
			_round = -1;
			_cycle = -1;
			_match = -1;
			InitializeComponent();

			tbPrint.Left = tbEditGroup.Right + 4;

			tbReferreReport.Visible = true;
			tbReferreReport.Click += new EventHandler(tbReferreReport_Click);

			matchesGridPanel = new MatchesGridPanel();
			// 
			// matchesGridPanel
			// 
			this.matchesGridPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
				| System.Windows.Forms.AnchorStyles.Left)
				| System.Windows.Forms.AnchorStyles.Right)));
			this.matchesGridPanel.Location = new System.Drawing.Point(8, 24);
			this.matchesGridPanel.Name = "matchesGridPanel";
			this.matchesGridPanel.Size = new System.Drawing.Size(panelGroupView.Width - 28 - phaseGroupGridPanel.Width, panelGroupView.Height - 32);
			this.matchesGridPanel.SelectionChanged += new EventHandler(MatchesGridPanelSelectionChanged);
			this.panelGroupView.Controls.Add(this.matchesGridPanel);
		}

		#endregion

		#region Editing

		protected override Sport.Championships.Phase CreatePhase(string name)
		{
			return new Sport.Championships.MatchPhase(name, Sport.Championships.Status.Planned);
		}

		protected override Sport.Championships.Group CreateGroup(string name)
		{
			return new Sport.Championships.MatchGroup(name);
		}


		#region Team Add

		private static Sport.UI.Dialogs.GenericEditDialog teamAddDialog = null;
		// This class will be used in the team combo box in the team add
		// dialog for teams from previous phase
		private class PreviousPhaseTeam
		{
			private int _group;
			public int Group
			{
				get { return _group; }
			}
			private int _position;
			public int Position
			{
				get { return _position; }
			}
			private Sport.Championships.MatchPhase _phase;
			public Sport.Championships.MatchPhase Phase
			{
				get { return _phase; }
			}

			public PreviousPhaseTeam(Sport.Championships.MatchPhase phase, int group, int position)
			{
				_phase = phase;
				_group = group;
				_position = position;
			}

			public override string ToString()
			{
				return _phase.Groups[_group].Name + " מיקום " + (_position + 1).ToString();
			}
		}

		private void TeamAddSourceChanged(object sender, EventArgs e)
		{
			if (Group == null)
				return;

			Sport.Data.LookupItem li = teamAddDialog.Items[1].Value as Sport.Data.LookupItem;
			if (li == null)
				return;

			if (this.Phase == null)
				return;

			if (this.Phase.Groups == null)
				return;

			System.Collections.ArrayList teams = new System.Collections.ArrayList();
			if (li.Id == 0)// from championship
			{
				Sport.Championships.MatchChampionship championship = Championship;
				if (championship.Teams == null)
					return;
				foreach (Sport.Entities.Team team in championship.Teams)
				{
					bool exist = false;
					for (int g = 0; g < Phase.Groups.Count && !exist; g++)
					{
						Sport.Championships.MatchGroup group = Phase.Groups[g];
						if (group.Teams == null)
							continue;
						for (int n = 0; n < group.Teams.Count && !exist; n++)
						{
							if (team.Equals(group.Teams[n].TeamEntity))
							{
								exist = true;
								break;
							}
						}
					}
					if (!exist)
						teams.Add(team);
				}

				if (teams.Count == 0)
					teamAddDialog.Items[2].Values = null;
				else
					teamAddDialog.Items[2].Values = (Sport.Entities.Team[])teams.ToArray(typeof(Sport.Entities.Team));
			}
			else // from previous phase
			{
				int iphase = Phase.Index;
				if (iphase > 0)
				{
					Sport.Championships.MatchPhase phase = Championship.Phases[iphase - 1];
					foreach (Sport.Championships.MatchGroup group in phase.Groups)
					{
						if (group.Teams == null)
							continue;
						for (int p = 0; p < group.Teams.Count; p++)
						{
							bool exist = false;
							for (int g = 0; g < Phase.Groups.Count && !exist; g++)
							{
								Sport.Championships.MatchGroup pg = Phase.Groups[g];
								for (int n = 0; n < pg.Teams.Count && !exist; n++)
								{
									Sport.Championships.MatchTeam team = pg.Teams[n];
									if (team != null && team.PreviousGroup == group.Index &&
										team.PreviousPosition == p)
									{
										exist = true;
										break;
									}
								}
							}
							// Using PreviousPhaseTeam and not Sport.Championships.Team 
							// because Sport.Championships.Team need to be added
							// to a phase if the reference to previous phase need
							// to be used
							if (!exist)
								teams.Add(new PreviousPhaseTeam(phase, group.Index, p));
						}
					}
				}

				if (teams.Count == 0)
					teamAddDialog.Items[2].Values = null;
				else
					teamAddDialog.Items[2].Values = (PreviousPhaseTeam[])teams.ToArray(typeof(PreviousPhaseTeam));
			}
		}

		protected override Sport.Championships.Team CreateTeam(ref int position)
		{
			if (teamAddDialog == null)
			{
				teamAddDialog = new Sport.UI.Dialogs.GenericEditDialog("הוספת קבוצה");
				teamAddDialog.Items.Add("מיקום:", Sport.UI.Controls.GenericItemType.Number);
				teamAddDialog.Items.Add("מקור:", Sport.UI.Controls.GenericItemType.Selection,
					null, new Sport.Data.LookupItem[] {
														  new Sport.Data.LookupItem(0, "אליפות"),
														  new Sport.Data.LookupItem(1, "שלב קודם")
													  });

				teamAddDialog.Items[1].ValueChanged += new EventHandler(TeamAddSourceChanged);
				teamAddDialog.Items.Add("קבוצה:", Sport.UI.Controls.GenericItemType.Selection);
			}

			string strText = "הוספת קבוצה - " + Group.Name;
			teamAddDialog.Text = strText;
			teamAddDialog.Items[0].Values = Sport.UI.Controls.GenericItem.NumberValues(1, Group.Teams.Count + 1, byte.MaxValue, 0);
			teamAddDialog.Items[0].Value = position + 1;
			teamAddDialog.Items[1].Value = null;
			teamAddDialog.Items[2].Values = null;

			if (teamAddDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				object pos = teamAddDialog.Items[0].Value;
				Sport.Entities.Team team = teamAddDialog.Items[2].Value as Sport.Entities.Team;
				Sport.Championships.MatchTeam matchTeam = null;
				// team is null if the selected team is a team from previous phase
				if (team == null)
				{
					PreviousPhaseTeam ppt = teamAddDialog.Items[2].Value as PreviousPhaseTeam;
					if (ppt != null)
					{
						// Creating a new team with the previous phase group and position
						matchTeam = new Sport.Championships.MatchTeam(ppt.Group, ppt.Position);
					}
				}
				else
				{
					matchTeam = new Sport.Championships.MatchTeam(team);
				}

				if (matchTeam != null && pos != null)
				{
					position = (int)((double)pos - 1);
					return matchTeam;
				}
			}

			return null;
		}


		#endregion

		#endregion

		#region View Operations

		public override void Open()
		{
			base.Open();


			string grouping = Sport.Core.Configuration.ReadString("Championship" + Championship.ChampionshipCategory.Id, "Grouping");
			if (grouping != null)
			{
				matchesGridPanel.LockRebuild(true);
				matchesGridPanel.ShowGroups = false;
				matchesGridPanel.GroupGroups = false;
				matchesGridPanel.GroupRounds = false;
				matchesGridPanel.GroupCycles = false;
				matchesGridPanel.GroupTournaments = false;

				for (int c = 0; c < grouping.Length; c++)
				{
					if (grouping[c] == 'G')
					{
						matchesGridPanel.ShowGroups = true;
						matchesGridPanel.GroupGroups = true;
					}
					else if (grouping[c] == 'g')
					{
						matchesGridPanel.ShowGroups = true;
					}
					else if (grouping[c] == 'R')
					{
						matchesGridPanel.GroupRounds = true;
					}
					else if (grouping[c] == 'C')
					{
						matchesGridPanel.GroupCycles = true;
					}
					else if (grouping[c] == 'T')
					{
						matchesGridPanel.GroupTournaments = true;
					}
				}

				matchesGridPanel.LockRebuild(false);
			}

			string columns = Sport.Core.Configuration.ReadString("Championship" + Championship.ChampionshipCategory.Id, "Columns");
			if (columns != null)
			{
				string[] cs = columns.Split(new char[] { ',' });

				MatchVisualizer.MatchField[] fields = new MatchVisualizer.MatchField[cs.Length];
				for (int n = 0; n < cs.Length; n++)
					fields[n] = (MatchVisualizer.MatchField)Int32.Parse(cs[n]);

				matchesGridPanel.Columns = fields;
			}

			OnRoundChange();
			OnCycleChange();
			OnMatchChange();
		}


		#endregion

		public new Sport.Championships.MatchChampionship Championship
		{
			get { return (Sport.Championships.MatchChampionship)base.Championship; }
		}

		#region Selection

		public new Sport.Championships.MatchPhase Phase
		{
			get { return (Sport.Championships.MatchPhase)base.Phase; }
			set { base.Phase = value; }
		}

		public new Sport.Championships.MatchGroup Group
		{
			get { return (Sport.Championships.MatchGroup)base.Group; }
			set { base.Group = value; }
		}

		public new Sport.Championships.MatchTeam Team
		{
			get { return (Sport.Championships.MatchTeam)base.Team; }
			set { base.Team = value; }
		}

		private int _round;
		public Sport.Championships.Round Round
		{
			get
			{
				if (_round == -1)
					return null;
				if (this.Group == null || this.Group.Rounds == null)
					return null;
				return this.Group.Rounds[_round];
			}
			set { matchesGridPanel.Round = value; }
		}

		private int _cycle;
		public Sport.Championships.Cycle Cycle
		{
			get { return _cycle == -1 ? null : (Round == null || Round.Cycles == null) ? null : Round.Cycles[_cycle]; }
			set { matchesGridPanel.Cycle = value; }
		}

		private int _tournament;
		public Sport.Championships.Tournament Tournament
		{
			get { return _tournament == -1 ? null : (Cycle == null || Cycle.Tournaments == null) ? null : Cycle.Tournaments[_tournament]; }
			set { matchesGridPanel.Tournament = value; }
		}

		private int _match;
		public Sport.Championships.Match Match
		{
			get { return _match == -1 ? null : (Cycle == null || Cycle.Matches == null) ? null : Cycle.Matches[_match]; }
			set { matchesGridPanel.Match = value; }
		}

		private void MatchesGridPanelSelectionChanged(object sender, EventArgs e)
		{
			int round = matchesGridPanel.Round == null ? -1 : matchesGridPanel.Round.Index;
			int cycle = matchesGridPanel.Cycle == null ? -1 : matchesGridPanel.Cycle.Index;
			int tournament = matchesGridPanel.Tournament == null ? -1 : matchesGridPanel.Tournament.Index;
			int match = matchesGridPanel.Match == null ? -1 : matchesGridPanel.Match.Index;
			if (matchesGridPanel.ShowGroups && Group != matchesGridPanel.Group)
			{
				Group = matchesGridPanel.Group;
				_round = round;
				_cycle = cycle;
				_tournament = tournament;
				_match = match;
				OnRoundChange();
				OnCycleChange();
				OnMatchChange();
			}
			else if (_round != round)
			{
				_round = round;
				_cycle = cycle;
				_tournament = tournament;
				_match = match;
				OnRoundChange();
				OnCycleChange();
				OnMatchChange();
			}
			else if (_cycle != cycle)
			{
				_cycle = cycle;
				_tournament = tournament;
				_match = match;
				OnCycleChange();
				OnMatchChange();
			}
			else if (_tournament != tournament)
			{
				_tournament = tournament;
				_match = match;
				OnMatchChange();
			}
			else if (_match != match)
			{
				_match = match;
				OnMatchChange();
			}

			//tbReferreReport.Enabled = ((this.Match == null) &&
			//	((this.Tournament != null) || (this.Cycle != null)));
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

			matchesGridPanel.Phase = Phase;
		}

		protected override void OnGroupChange()
		{
			base.OnGroupChange();

			tbAddRound.Enabled = Group != null;

			tbBuildMatches.Enabled = tbArrangeNumbers.Enabled = (Group != null);
			tbArrangeByDate.Enabled = false; //(Group != null);

			tbSetResults.Enabled = Group != null && !Championship.Editing; //Phase.Status != Sport.Championships.Status.Planned


			if (!matchesGridPanel.ShowGroups)
				matchesGridPanel.Group = Group;
		}

		private bool selecting = false;
		protected void OnRoundChange()
		{
			bool s = selecting;
			selecting = true;

			tbAddCycle.Enabled = Phase != null;
			tbRoundName.Text = _round == -1 ? null : Round.Name;
			tbRoundName.Enabled = _round != -1;
			tbRemoveRound.Enabled = _round != -1;
			tbMoveRound.Enabled = _round != -1 && Group.Rounds.Count > 1;
			selecting = s;
		}

		protected void OnCycleChange()
		{
			bool s = selecting;
			selecting = true;

			tbCycleName.Text = _cycle == -1 ? null : Cycle.Name;
			tbCycleName.Enabled = _cycle != -1;
			tbAddMatch.Enabled = _cycle != -1;
			tbRemoveCycle.Enabled = _cycle != -1;
			tbMoveCycle.Enabled = _cycle != -1 && Round.Cycles.Count > 1;
			selecting = s;
		}

		protected void OnMatchChange()
		{
			//set buttons status:
			tbRemoveMatch.Enabled = bbMatch.Enabled = _match != -1;

			//initlaize flags:
			bool moveUp = true;
			bool moveDown = true;

			//got any match?
			if (_match == -1)
			{
				//nothing - disable buttons.
				moveUp = false;
				moveDown = false;
			}
			else
			{
				//get current match:
				Sport.Championships.Match match = Match;
				//get current cycle
				Sport.Championships.Cycle cycle = Cycle;
				if (matchesGridPanel.GroupTournaments) // Tournament move
				{
					// Checking if in no tournament
					if (_tournament != -1)
					{
						if ((_tournament == cycle.Tournaments.Count - 1) &&
							(cycle.Tournaments[_tournament].GetMatchCount() == 1) &&
							(cycle.GetNextCycle() == null))
						{
							if (match.Index == (cycle.Matches.Count - 1))
								moveDown = false;
						}
					}
					else
					{
						if ((cycle.GetPrevCycle() == null) && (match.Index == 0))
							moveUp = false;
					}
				}
				else if (matchesGridPanel.GroupCycles) // Cycle move
				{
					moveUp = (cycle.GetPrevCycle() != null) || (match.Index > 0);
					moveDown = (cycle.GetNextCycle() != null) || (match.Index < (cycle.Matches.Count - 1));
				}
				else if (matchesGridPanel.GroupRounds) // Round move
				{
					moveUp = (cycle.Round.Cycles[0].GetPrevCycle() != null) || (match.Index > 0);
					moveDown = (cycle.Round.Cycles[cycle.Round.Cycles.Count - 1].GetNextCycle() != null) || (match.Index < (cycle.Matches.Count - 1));
				}
				else // doing nothing
				{
					moveUp = false;
					moveDown = false;
				}
			}

			tbMatchUp.Enabled = moveUp;
			tbMatchDown.Enabled = moveDown;
			bbMatch.Value = _match == -1 ? null : Cycle.Matches[_match];
		}

		#endregion

		protected override void Customize()
		{
			MatchesGridCustomizeForm mgcf = new MatchesGridCustomizeForm(matchesGridPanel);

			mgcf.ShowDialog();

			string grouping = "";
			if (matchesGridPanel.ShowGroups)
			{
				if (matchesGridPanel.GroupGroups)
					grouping += "G";
				else
					grouping += "g";
			}
			if (matchesGridPanel.GroupRounds)
				grouping += "R";
			if (matchesGridPanel.GroupCycles)
				grouping += "C";
			if (matchesGridPanel.GroupTournaments)
				grouping += "T";

			string[] columns = new string[matchesGridPanel.Columns.Length];
			for (int c = 0; c < matchesGridPanel.Columns.Length; c++)
				columns[c] = ((int)matchesGridPanel.Columns[c]).ToString();

			Sport.Core.Configuration.WriteString("Championship" + Championship.ChampionshipCategory.Id, "Grouping", grouping);
			Sport.Core.Configuration.WriteString("Championship" + Championship.ChampionshipCategory.Id, "Columns", String.Join(",", columns));

			OnMatchChange();
		}

		protected override void SetResults()
		{
			Sport.Championships.MatchGroup group = Group;
			if (group != null)
			{
				Sport.Championships.MatchPhase phase = group.Phase;

				/* if (phase.Status != Sport.Championships.Status.Planned) */
				//{
				int round = -1;
				int cycle = -1;
				if (this.Round != null)
				{
					round = this.Round.Index;
					if (this.Cycle != null)
						cycle = this.Cycle.Index;
				}
				MatchGroupResultForm mgrf =
					new MatchGroupResultForm(group, round, cycle);

				if (mgrf.ShowDialog() == DialogResult.OK)
					matchesGridPanel.Rebuild();
				//}
			}
		}

		protected override Sport.Documents.Document CreateGroupDocument(System.Drawing.Printing.PrinterSettings settings)
		{
			Cursor cur = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;

			int selPhase = (this.Phase == null) ? -1 : this.Phase.Index;
			int selGroup = (this.Group == null) ? -1 : this.Group.Index;
			int selRound = (this.Round == null) ? -1 : this.Round.Index;
			int selCycle = (this.Cycle == null) ? -1 : this.Cycle.Index;
			MatchesSectionObject matchesSectionObject = new MatchesSectionObject(Championship, matchesGridPanel.Columns,
				matchesGridPanel.ShowGroups ? matchesGridPanel.GroupGroups : true,
				matchesGridPanel.GroupRounds, matchesGridPanel.GroupCycles,
				matchesGridPanel.GroupTournaments, selPhase, selGroup,
				selRound, selCycle);

			BaseDocumentBuilder baseDocumentBuilder = new BaseDocumentBuilder(settings);
			Sport.Documents.DocumentBuilder documentBuilder = baseDocumentBuilder.CreateDocumentBuilder(Championship.Name, matchesSectionObject, true);
			Sport.Documents.Document document = documentBuilder.CreateDocument();

			Cursor.Current = cur;

			return document;
		}

		protected override Sport.Documents.Document CreateRankingDocument(System.Drawing.Printing.PrinterSettings settings)
		{
			Cursor cur = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;

			RankingSectionObject rankingSectionObject = new RankingSectionObject(false);
			rankingSectionObject.Phase = Phase;
			BaseDocumentBuilder baseDocumentBuilder = new BaseDocumentBuilder(settings);
			Sport.Documents.DocumentBuilder documentBuilder = baseDocumentBuilder.CreateDocumentBuilder(Championship.Name + " - " + Phase.Name, rankingSectionObject, true);
			Sport.Documents.Document document = documentBuilder.CreateDocument();

			Cursor.Current = cur;
			return document;
		}

		private void tbArrangeNumbers_Click(object sender, EventArgs e)
		{
			int number = 1;
			foreach (Sport.Championships.Round round in this.Group.Rounds)
			{
				foreach (Sport.Championships.Cycle cycle in round.Cycles)
				{
					foreach (Sport.Championships.Match match in cycle.Matches)
					{
						match.Number = number;
						number++;
					}
				}
			}
			matchesGridPanel.Rebuild();
			//OnMatchChange();
		}

		private void tbArrageByDate_Click(object sender, EventArgs e)
		{
			bool blnGlobalArrange = Sport.UI.MessageBox.Ask("האם לסדר ברמת המחזור?",
				true);

			Sport.UI.Dialogs.WaitForm.ShowWait("מסדר משחקים אנא המתן...");
			foreach (Sport.Championships.Round round in this.Group.Rounds)
			{
				if (blnGlobalArrange)
				{
					round.ArrangeMatchesByDate();
					continue;
				}

				foreach (Sport.Championships.Cycle cycle in round.Cycles)
				{
					if (cycle.Matches.Count == 0)
						continue;
					System.Collections.ArrayList arrTournaments =
						cycle.GetTournamentMatches();
					foreach (Sport.Championships.Match[] matches in arrTournaments)
					{
						if (matches.Length == 0)
							continue;
						for (int i = 0; i < matches.Length; i++)
						{
							DateTime minTime = matches[i].Time;
							int minIndex = i;
							for (int j = i + 1; j < matches.Length; j++)
							{
								if (matches[j].Time < minTime)
								{
									minTime = matches[j].Time;
									minIndex = j;
								}
							}
							if (minIndex != i)
							{
								matches[i].ChangeIndex(matches[minIndex].Index);
								Sport.Championships.Match temp = matches[i];
								matches[i] = matches[minIndex];
								matches[minIndex] = temp;
							}
						}
					}
				}
			}
			Sport.UI.Dialogs.WaitForm.HideWait();
			matchesGridPanel.Rebuild();
		}

		private void ExtractMatches(object source, ref List<Sport.Championships.Match> matches)
		{
			if (source == null)
				return;

			if (source is Sport.Championships.Match)
			{
				matches.Add(source as Sport.Championships.Match);
			}
			else if (source is Sport.Championships.Cycle)
			{
				Sport.Championships.Cycle cycle = source as Sport.Championships.Cycle;
				foreach (Sport.Championships.Match match in cycle.Matches)
					ExtractMatches(match, ref matches);
			}
			else if (source is Sport.Championships.Round)
			{
				Sport.Championships.Round round = source as Sport.Championships.Round;
				foreach (Sport.Championships.Cycle cycle in round.Cycles)
					ExtractMatches(cycle, ref matches);
			}
			else if (source is Sport.Championships.MatchGroup)
			{
				Sport.Championships.MatchGroup group = source as Sport.Championships.MatchGroup;
				foreach (Sport.Championships.Round round in group.Rounds)
					ExtractMatches(round, ref matches);
			}
		}

		private void tbReferreReport_Click(object sender, EventArgs e)
		{
			List<Sport.Championships.Match> matches = new List<Sport.Championships.Match>();
			Sport.UI.Controls.Grid.GridSelection gridSelection = matchesGridPanel.GetGridSelection();
			if (gridSelection != null && gridSelection.Rows != null && gridSelection.Rows.Length > 0)
			{
				for (int i = 0; i < gridSelection.Rows.Length; i++)
				{
					var currentSelectedRowIndex = gridSelection.Rows[i];
					object currentRow = matchesGridPanel.GetRowAt(currentSelectedRowIndex);
					ExtractMatches(currentRow, ref matches);
				}
				if (matches.Count > 0)
				{
					matches = matches.Distinct().ToList();
				}
			}

			matches.RemoveAll(m => m.RefereeCount <= 0);
			matches.RemoveAll(m => m.Time.Year <= 1900);
			matches.RemoveAll(m => m.Facility == null || m.Facility.Id < 0);
			if (matches.Count == 0)
			{
				Sport.UI.MessageBox.Error("אין משחקים מתאימים לדו\"ח שיבוץ שופטים\n" + "(יש להגדיר כמות שופטים  מוזמנים, תאריך, ומתקן)", 
					"בניית אליפות");
				return;
			}

			//print:
			Documents.ChampionshipDocumentType reportType = ChampionshipDocumentType.RefereesReport;
			Documents.ChampionshipDocuments champDoc = new Documents.ChampionshipDocuments(reportType, matches.ToArray());
			champDoc.Print();
		}
	}
}
