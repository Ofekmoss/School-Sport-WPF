using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace Sportsman.Forms
{
	public partial class FixTeamRegionsDialog : Form, Sport.UI.Controls.IGridSource
	{
		private List<TeamRow> rows = new List<TeamRow>();
		private List<string> fixAllResults = new List<string>();
		private bool cancelFixAll = false;

		public FixTeamRegionsDialog()
		{
			InitializeComponent();
		}

		private void FixTeamRegionsDialog_Load(object sender, EventArgs e)
		{
			rows.Clear();
			tblTeams.Columns.Clear();
			tblTeams.Columns.Add(0, "#", 50);
			tblTeams.Columns.Add(1, "שם בית הספר", 150);
			tblTeams.Columns.Add(2, "סמל מוסד", 100);
			tblTeams.Columns.Add(3, "אליפות", 350);
			tblTeams.Columns.Add(4, "מחוז קבוצה", 100);
			tblTeams.Columns.Add(5, "מחוז אליפות", 100);
			tblTeams.Source = this;
			tblTeams.RefreshSource();
		}

		public void SetGrid(Sport.UI.Controls.Grid grid)
		{
			
		}

		public int GetRowCount()
		{
			return rows.Count;
		}

		public int GetFieldCount(int row)
		{
			return 6;
		}

		public int GetGroup(int row)
		{
			return 0;
		}

		public string GetText(int rowIndex, int field)
		{
			if (rowIndex < 0 || rowIndex >= rows.Count)
				return null;

			TeamRow row = rows[rowIndex];
			switch (field)
			{
				case 0:
					return (rowIndex + 1).ToString();
				case 1:
					return row.SchoolName;
				case 2:
					return row.SchoolSymbol;
				case 3:
					return row.ChampionshipName;
				case 4:
					return row.TeamRegion;
				case 5:
					return row.ChampRegion;
			}

			return "";
		}

		public Sport.UI.Controls.Style GetStyle(int row, int field, Sport.UI.Controls.GridDrawState state)
		{
			return null;
		}

		public string GetTip(int row)
		{
			return "";
		}

		public int[] GetSort(int group)
		{
			return null;
		}

		public void Sort(int group, int[] columns)
		{
			
		}

		public Control Edit(int row, int field)
		{
			return null;
		}

		public void EditEnded(Control control)
		{
			
		}

		private void btnLoadData_Click(object sender, EventArgs e)
		{
			this.Controls.OfType<Button>().ToList().ForEach(b => b.Enabled = false);
			Sport.UI.Dialogs.WaitForm.ShowWait("טוען נתונים אנא המתן...");
			LoadAllData();
			Sport.UI.Dialogs.WaitForm.HideWait();
			this.Controls.OfType<Button>().ToList().ForEach(b => b.Enabled = true);
		}

		private string FixSingleTeam(TeamRow row, out bool blnFixed)
		{
			blnFixed = false; 
			Sport.Entities.Team team;
			if (!TryGetTeam(row.TeamId, out team))
				return "Error: team " + row.TeamId + " is not valid";
			if (team.GetPlayers().Length > 0)
				return "Team '" + team.Name + "' in '" + team.Championship.Name + " " + team.Category.Name + "' got registered players, can't auto fix";
			try
			{
				int teamRegion = team.School.Region.Id, champRegion = team.Championship.Region.Id;
				if (teamRegion == champRegion)
					return "Team region already same as championship region, nothing to fix";

				string champName = team.Championship.Name;
				Sport.Data.Entity properChampEntity = Sport.Entities.Championship.Type.GetEntities(new Sport.Data.EntityFilter(
					(int)Sport.Entities.Championship.Fields.Region, teamRegion)).ToList().Find(c => c.Name.Equals(champName));
				if (properChampEntity == null)
					return "Region '" + team.School.Region.Name + "' does not contain the championship '" + champName + "'";

				Sport.Entities.ChampionshipCategory properCategory = new Sport.Entities.Championship(properChampEntity).GetCategories().ToList().Find(cc => cc.Category.Equals(team.Category.Category));
				if (properCategory == null)
					return "Championship '" + champName + "' in region '" + team.School.Region.Name + "' does not contain the category " + team.Category.Name;

				if (properCategory.GetTeams().ToList().Exists(t => t.School != null && t.School.Symbol.Equals(team.School.Symbol)))
				{
					blnFixed = true;
					return "Team '" + team.Name + "' already exists in '" + properChampEntity.Name + "' under '" + 
						team.School.Region.Name + "' (" + DeleteTeam(team) + ")";
				}

				try
				{
					Sport.Entities.Team newTeam = new Sport.Entities.Team(Sport.Entities.Team.Type.New());
					newTeam.Category = properCategory;
					newTeam.Championship = properCategory.Championship;
					newTeam.School = team.School;
					newTeam.Status = Sport.Types.TeamStatusType.Confirmed;
					Sport.Data.EntityResult saveResult = newTeam.Save();
					if (saveResult.Succeeded)
					{
						blnFixed = true;
						return "Team '" + team.Name + "' added successfully to '" + properChampEntity.Name + "' under '" + 
							team.School.Region.Name + "' (" + DeleteTeam(team) + ")";
					}
					return "Saving new team '" + team.Name + "' for '" + properChampEntity.Name + "' under '" +
						team.School.Region.Name + "' has failed: " + saveResult.GetMessage() + " (" + saveResult.ResultCode + ")";
				}
				catch (Exception ex2)
				{
					return "Error saving new team '" + team.Name + "' for '" + properChampEntity.Name + "' under '" +
						team.School.Region.Name + "':\n" + ex2.ToString();
				}
			}
			catch (Exception ex)
			{
				return "Error fixing team " + row.TeamId + ": " + ex.ToString();
			}
		}

		private string DeleteTeam(Sport.Entities.Team team)
		{
			string deleteResult;
			try
			{
				deleteResult = team.Entity.Delete() ? "deleted successfully" : "failed to delete";
			}
			catch (Exception ex3)
			{
				deleteResult = "Error deleting: " + ex3.Message;
			}
			return deleteResult;
		}

		private bool TryGetTeam(int teamId, out Sport.Entities.Team team)
		{
			team = null;
			try
			{
				team = new Sport.Entities.Team(teamId);
			}
			catch
			{
			}

			return team != null && team.Id > 0 && team.Championship != null && team.School != null;
		}

		private void LoadAllData()
		{
			rows.Clear();
			lbLiveProgress.Text = "0";
			lbLiveProgress.Visible = true;
			/*
			for (int i = 1; i <= 150; i++)
			{
				rows.Add(new TeamRow
				{
					TeamId = i,
					SchoolName = "Team " + i,
					SchoolSymbol = i.ToString().PadLeft(6, '0'),
					ChampionshipName = "Champ " + i,
					TeamRegion = "Region A" + i,
					ChampRegion = "Region B" + i
				});
			}
			*/

			try
			{
				Sport.Entities.Championship.Type.GetEntities(null).ToList().FindAll(champ =>
					(int)champ.Fields[(int)Sport.Entities.Championship.Fields.Region] != Sport.Entities.Region.CentralRegion).ForEach(champ =>
				{
					new Sport.Entities.Championship(champ).GetCategories().ToList().ForEach(category =>
					{
						category.GetTeams().ToList().ForEach(team =>
						{
							if (team.School != null && team.School.Region != null && team.Championship != null && team.Championship.Region != null)
							{
								int teamRegion = team.School.Region.Id;
								int champRegion = team.Championship.Region.Id;
								if (teamRegion != champRegion)
								{
									rows.Add(new TeamRow
									{
										TeamId = team.Id,
										SchoolName = team.School.Name,
										SchoolSymbol = team.School.Symbol,
										ChampionshipName = team.Championship.Name + " " + team.Category.Name,
										TeamRegion = team.School.Region.Name,
										ChampRegion = team.Championship.Region.Name
									});
								}
							}
							lbLiveProgress.Text = (Int32.Parse(lbLiveProgress.Text) + 1).ToString();
							Application.DoEvents();
						});
					});

				});
			}
			catch (Exception ex)
			{
				Sport.UI.MessageBox.Error("Error while reading data:\n" + ex.ToString(), "שגיאה");
			}
			tblTeams.RefreshSource();
			lbLiveProgress.Visible = false;
		}

		private void btnFixSelectedRow_Click(object sender, EventArgs e)
		{
			if (tblTeams.Selection.Rows.Length != 1)
			{
				Sport.UI.MessageBox.Error("לא נבחרה קבוצה יחידה", "שגיאה");
				return;
			}

			TeamRow selectedRow = rows[tblTeams.SelectedIndex];
			this.Controls.OfType<Button>().ToList().ForEach(b => b.Enabled = false);
			Sport.UI.Dialogs.WaitForm.ShowWait(string.Format("מתקן את הקבוצה '{0}'...", selectedRow.SchoolName));
			bool blnFixed;
			string fixResult = FixSingleTeam(selectedRow, out blnFixed);
			Sport.UI.Dialogs.WaitForm.HideWait();
			this.Controls.OfType<Button>().ToList().ForEach(b => b.Enabled = true);
			DisplayFixResult(fixResult);
			if (blnFixed)
				btnLoadData_Click(sender, EventArgs.Empty);
		}

		private void btnFixAll_Click(object sender, EventArgs e)
		{
			fixAllResults.Clear();
			cancelFixAll = false;
			if (rows.Count == 0)
			{
				Sport.UI.MessageBox.Error("אין קבוצות לתיקון", "שגיאה");
				return;
			}

			tblTeams.SelectedIndex = -1;
			lbLiveProgress.Text = "";
			lbLiveProgress.Visible = true;
			btnStopFixAll.Visible = true;
			this.Controls.OfType<Button>().ToList().ForEach(b => b.Enabled = false);
			btnStopFixAll.Enabled = true;
			Sport.UI.Dialogs.WaitForm.ShowWait("מתקן " + rows.Count + " קבוצות אנא המתן...");
			List<bool> arrFixed = new List<bool>();
			bool blnFixed;
			for (int i = 0; i < rows.Count; i++)
			{
				if (cancelFixAll)
					break;
				TeamRow currentRow = rows[i];
				lbLiveProgress.Text = string.Format("{0} - {1}", currentRow.SchoolName, currentRow.ChampionshipName);
				tblTeams.VisibleRow = i;
				string fixResult = FixSingleTeam(currentRow, out blnFixed);
				arrFixed.Add(blnFixed);
				fixAllResults.Add(fixResult);
				Application.DoEvents();
			}
			Sport.UI.Dialogs.WaitForm.HideWait();
			this.Controls.OfType<Button>().ToList().ForEach(b => b.Enabled = true);
			btnStopFixAll.Visible = false;
			if (fixAllResults.Count > 0)
			{
				DisplayFixResult(fixAllResults.ToArray());
				fixAllResults.Clear();
			}
			else
			{
				Sport.UI.MessageBox.Error("אין שורות דיווח", "שגיאה");
			}
			if (arrFixed.Exists(b => b == true))
				btnLoadData_Click(sender, EventArgs.Empty);
			lbLiveProgress.Visible = false;
		}

		private void btnStopFixAll_Click(object sender, EventArgs e)
		{
			cancelFixAll = true;
		}

		private void DisplayFixResult(string[] resultMessages)
		{
			bool success = false;
			string errorMsg = "";
			try
			{
				string filePath = Mir.Common.Utils.Instance.MapPath(string.Format("FixTeamRegion_{0}.txt", DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss")));
				File.WriteAllLines(filePath, resultMessages);
				success = File.Exists(filePath);
				if (success)
				{
					Process process = Process.Start(filePath);
					if (process == null)
					{
						success = false;
						errorMsg = "Failed to start process";
					}
				}
				else
				{
					errorMsg = "Failed to create '" + filePath + "'";
				}
			}
			catch (Exception ex)
			{
				errorMsg = ex.Message;
			}

			if (!success)
				Sport.UI.MessageBox.Show("{" + errorMsg + "}\n" + string.Join("\n", resultMessages), "תוצאות תיקון");
		}

		private void DisplayFixResult(string resultMessage)
		{
			DisplayFixResult(new string[] { resultMessage });
		}

		private void FixTeamRegionsDialog_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (fixAllResults.Count > 0)
				DisplayFixResult(fixAllResults.ToArray());
		}

		private class TeamRow
		{
			public int TeamId { get; set; }
			public string SchoolName { get; set; }
			public string SchoolSymbol { get; set; }
			public string ChampionshipName { get; set; }
			public string TeamRegion { get; set; }
			public string ChampRegion { get; set; }
		}
	}
}
