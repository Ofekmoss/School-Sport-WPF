using System;

namespace Sportsman.Producer
{
	public class TeamsDivisionForm : System.Windows.Forms.Form,
		Sport.UI.Controls.IGridSource
	{

		private int _phase;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOK;
		private Sport.UI.Controls.Grid gridTeams;
		private System.Windows.Forms.CheckBox cbPreviousPhase;
		private Sport.Championships.Championship _championship;

		#region initialize component
		private void InitializeComponent()
		{
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.gridTeams = new Sport.UI.Controls.Grid();
			this.cbPreviousPhase = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnCancel.Location = new System.Drawing.Point(89, 383);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.TabIndex = 32;
			this.btnCancel.Text = "ביטול";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnOK.Location = new System.Drawing.Point(9, 383);
			this.btnOK.Name = "btnOK";
			this.btnOK.TabIndex = 31;
			this.btnOK.Text = "אישור";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// gridTeams
			// 
			this.gridTeams.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.gridTeams.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.gridTeams.Editable = true;
			this.gridTeams.GroupingStyle = Sport.UI.Controls.GridGroupingStyle.Grid;
			this.gridTeams.HorizontalLines = true;
			this.gridTeams.Location = new System.Drawing.Point(9, 9);
			this.gridTeams.Name = "gridTeams";
			this.gridTeams.SelectedIndex = -1;
			this.gridTeams.SelectionMode = System.Windows.Forms.SelectionMode.One;
			this.gridTeams.ShowCheckBoxes = false;
			this.gridTeams.ShowRowNumber = false;
			this.gridTeams.Size = new System.Drawing.Size(408, 368);
			this.gridTeams.TabIndex = 30;
			this.gridTeams.VerticalLines = true;
			// 
			// cbPreviousPhase
			// 
			this.cbPreviousPhase.Location = new System.Drawing.Point(256, 382);
			this.cbPreviousPhase.Name = "cbPreviousPhase";
			this.cbPreviousPhase.Size = new System.Drawing.Size(160, 24);
			this.cbPreviousPhase.TabIndex = 33;
			this.cbPreviousPhase.Text = "חלק קבוצות שלב קודם";
			this.cbPreviousPhase.CheckedChanged += new System.EventHandler(this.cbPreviousPhase_CheckedChanged);
			// 
			// TeamsDivisionForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.ClientSize = new System.Drawing.Size(426, 414);
			this.Controls.Add(this.cbPreviousPhase);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.gridTeams);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Name = "TeamsDivisionForm";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.ResumeLayout(false);

		}
		#endregion

		public TeamsDivisionForm(Sport.Championships.Championship championship, int phase)
		{
			InitializeComponent();

			_phase = phase;
			_championship = championship;

			System.Collections.Hashtable teamRow = new System.Collections.Hashtable();
			rows = new Row[_championship.Teams.Count];

			for (int t = 0; t < _championship.Teams.Count; t++)
			{
				rows[t] = new Row(_championship.Teams[t], null, -1);
				teamRow[_championship.Teams[t].Id] = t;
			}

			bool showPreviousPhaseTeams = false;
			if ((_championship is Sport.Championships.MatchChampionship) &&
				_phase > 0)
			{
				cbPreviousPhase.Visible = true;

				int previousCount = 0;

				foreach (Sport.Championships.Group group in _championship.Phases[_phase - 1].Groups)
				{
					previousCount += group.Teams.Count;
				}

				previousPhaseRows = new Row[previousCount];

				int i = 0;
				foreach (Sport.Championships.Group group in _championship.Phases[_phase - 1].Groups)
				{
					for (int t = 0; t < group.Teams.Count; t++)
					{
						previousPhaseRows[i] = new Row(group, t, null, -1);
						i++;
					}
				}
			}
			else
			{
				cbPreviousPhase.Visible = false;
				previousPhaseRows = null;
			}

			if (_championship is Sport.Championships.MatchChampionship && previousPhaseRows != null)
			{
				foreach (Sport.Championships.Group group in _championship.Phases[_phase].Groups)
				{
					foreach (Sport.Championships.MatchTeam team in group.Teams)
					{
						if (team.TeamEntity != null)
						{
							int row = (int) teamRow[team.TeamEntity.Id];
							rows[row].Group = group;
							rows[row].Position = team.Index;
						}
						else if (team.PreviousGroup >= 0 && team.PreviousPosition >= 0)
						{
							int index = 0;
							for (int g = 0; g < team.PreviousGroup; g++)
							{
								if (g < _championship.Phases[_phase - 1].Groups.Count)
									index += _championship.Phases[_phase - 1].Groups[g].Teams.Count;
							}
							index += team.PreviousPosition;
							if (index < previousPhaseRows.Length)
							{
								previousPhaseRows[index].Group = group;
								previousPhaseRows[index].Position = team.Index;
								showPreviousPhaseTeams = true;
							}
						}
					}
				}
			}
			else
			{
				foreach (Sport.Championships.Group group in _championship.Phases[_phase].Groups)
				{
					foreach (Sport.Championships.Team team in group.Teams)
					{
						if (team.TeamEntity != null)
						{
							int row = (int) teamRow[team.TeamEntity.Id];
							rows[row].Group = group;
							rows[row].Position = team.Index;
						}
					}
				}
			}

			if (showPreviousPhaseTeams)
			{
				cbPreviousPhase.Checked = true;
			}

			gridTeams.Columns.Add(0, "קבוצה", 80);
			gridTeams.Columns.Add(1, "בית", 30);
			gridTeams.Columns.Add(2, "מיקום", 10);
			gridTeams.Source = this;
		}

		private bool showingPreviousPhaseTeams = false;

		private void ShowPreviousPhaseTeams(bool show)
		{
			gridTeams.CancelEdit();

			if (showingPreviousPhaseTeams == show)
				return ;

			if (show)
			{
				if (previousPhaseRows == null)
					return ;

				// Resetting previous teams' positions
				int[] positions = new int[_championship.Phases[_phase].Groups.Count];
				for (int g = 0; g < positions.Length; g++)
				{
					positions[g] = GetGroupSize(_championship.Phases[_phase].Groups[g]);
				}

				for (int n = 0; n < previousPhaseRows.Length; n++)
				{
					if (previousPhaseRows[n].Group != null)
					{
						int g = previousPhaseRows[n].Group.Index;
						previousPhaseRows[n].Position = positions[g];
						positions[g]++;
					}
				}

				Row[] newRows = new Row[_championship.Teams.Count + previousPhaseRows.Length];

				int i = 0;
				for (int n = 0; n < rows.Length; n++)
				{
					if (rows[n].Team != null)
					{
						newRows[i] = rows[n];
						i++;
					}
				}

				for (int n = 0; n < previousPhaseRows.Length; n++)
				{
					newRows[i] = previousPhaseRows[n];
					i++;
				}

				rows = newRows;
			}
			else
			{
				// Removing previous teams' positions
				for (int n = 0; n < rows.Length; n++)
				{
					if (rows[n].PreviousGroup != null && rows[n].Group != null)
					{
						for (int a = 0; a < rows.Length; a++)
						{
							if (n != a && rows[n].Group == rows[a].Group &&
								rows[n].Position < rows[a].Position)
							{
								rows[a].Position--;
							}
						}
					}
				}

				Row[] newRows = new Row[_championship.Teams.Count];

				int i = 0;
				for (int n = 0; n < rows.Length; n++)
				{
					if (rows[n].Team != null)
					{
						newRows[i] = rows[n];
						i++;
					}
				}

				rows = newRows;
			}

			showingPreviousPhaseTeams = show;

			if (_sort != null)
				Sport.Common.ArraySort.Sort(rows, _sort, new TeamComparer());

			gridTeams.RefreshSource();
		}

		private class TeamComparer : Sport.Common.IFieldComparer
		{
			#region IFieldComparer Members

			public int Compare(int field, object x, object y)
			{
				switch (field)
				{
					case (0): // Team name
						return ((Row) x).GetName().CompareTo(((Row) y).GetName());
					case (1): // Group
						return ((Int32) (((Row) x).Group == null ? -1 : ((Row) x).Group.Index)).CompareTo(
							((Row) y).Group == null ? -1 : ((Row) y).Group.Index);
					case (2): // Position
						return ((Int32) ((Row) x).Position).CompareTo(
							((Row) y).Position);
				}

				return 0;
			}

			#endregion
		}


		private void btnOK_Click(object sender, System.EventArgs e)
		{
			gridTeams.CancelEdit();
			Sport.Common.ArraySort.Sort(rows, new int[] {1, 2}, new TeamComparer());
			gridTeams.Refresh();
			
			System.Collections.Hashtable tblCompetitors=new System.Collections.Hashtable();
			foreach (Sport.Championships.Group group in _championship.Phases[_phase].Groups)
			{
				//store competitors:
				if (group is Sport.Championships.CompetitionGroup)
				{
					Sport.Championships.CompetitionGroup cGroup=
						(Sport.Championships.CompetitionGroup) group;
					foreach (Sport.Championships.Competition competition in cGroup.Competitions)
					{
						if (competition.Competitors != null)
						{
							tblCompetitors[competition] = 
								new System.Collections.ArrayList(competition.Competitors);
						}
					}
				}
				if (group.Teams != null)
					group.Teams.Clear();
			}
			
			if (_championship is Sport.Championships.MatchChampionship)
			{
				for (int n = 0; n < rows.Length; n++)
				{
					if (rows[n] != null && rows[n].Group != null)
					{
						if (rows[n].Team == null)
							rows[n].Group.Teams.Add(new Sport.Championships.MatchTeam(rows[n].PreviousGroup.Index, rows[n].PreviousPosition));
						else
							rows[n].Group.Teams.Add(new Sport.Championships.MatchTeam(rows[n].Team));
					}
				}
			}
			else
			{
				for (int n = 0; n < rows.Length; n++)
				{
					if (rows[n].Group != null)
					{
						Sport.Entities.Team team=rows[n].Team;
						rows[n].Group.Teams.Add(new Sport.Championships.CompetitionTeam(team));
						
						//put competitors back:
						Sport.Championships.CompetitionGroup cGroup=
							(Sport.Championships.CompetitionGroup) rows[n].Group;
						foreach (Sport.Championships.Competition competition in tblCompetitors.Keys)
						{
							//get competitors:
							System.Collections.ArrayList competitors=
								(System.Collections.ArrayList) tblCompetitors[competition];
							
							//iterate over competitors:
							foreach (Sport.Championships.Competitor competitor in competitors)
							{
								//add only if part of current team.
								Sport.Championships.CompetitionTeam cTeam=null;
								if (competitor.Player != null)
								{
									cTeam = competitor.Player.CompetitionTeam;
								}
								else
								{
									int index=cGroup.GetPlayerTeam(competitor.PlayerNumber);
									if ((index >= 0)&&(index < cGroup.Teams.Count))
										cTeam = cGroup.Teams[index];
								}
								if ((cTeam != null)&&(cTeam.TeamEntity != null)&&
									(cTeam.TeamEntity.Id == team.Id))
								{
									cGroup.Competitions[competition.Index].Competitors.Add(competitor);
								}
							}
						}
					}
				}
			}

			DialogResult = System.Windows.Forms.DialogResult.OK;
			Close();
		}

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			DialogResult = System.Windows.Forms.DialogResult.Cancel;
			Close();
		}

		private class Row
		{
			public Sport.Entities.Team			Team;
			public Sport.Championships.Group	PreviousGroup;
			public int							PreviousPosition;
			public Sport.Championships.Group	Group;
			public int							Position;

			public Row(Sport.Entities.Team team, Sport.Championships.Group group,
				int position)
			{
				Team = team;
				PreviousGroup = null;
				PreviousPosition = -1;
				Group = group;
				Position = position;
			}

			public Row(Sport.Championships.Group previousGroup, int previousPosition, Sport.Championships.Group group,
				int position)
			{
				Team = null;
				PreviousGroup = previousGroup;
				PreviousPosition = previousPosition;
				Group = group;
				Position = position;
			}

			public string GetName()
			{
				if (Team == null)
					return PreviousGroup.Name + " מיקום " + (PreviousPosition + 1).ToString();
				return Team.TeamName();
			}
		}

		private Row[] rows;

		private Row[] previousPhaseRows;
	
		#region IGridSource Members

		public void SetGrid(Sport.UI.Controls.Grid grid)
		{
		}

		private int[] _sort;
		public int[] GetSort(int group)
		{
			return _sort;
		}

		public void Sort(int group, int[] columns)
		{
			if (columns == null)
			{
				_sort = null;
			}
			else
			{
				_sort = (int[]) columns.Clone();

				Sport.Common.ArraySort.Sort(rows, _sort, new TeamComparer());
			}
		}

		public Sport.UI.Controls.Style GetStyle(int row, int field, Sport.UI.Controls.GridDrawState state)
		{
			return null;
		}

		public string GetTip(int row)
		{
			return null;
		}

		public int GetGroup(int row)
		{
			return 0;
		}

		private Sport.UI.Controls.NullComboBox ncbGroup = null;
		private Sport.UI.Controls.TextControl tcPosition = null;

		private class GroupItem
		{
			private Sport.Championships.Group _group;
			public Sport.Championships.Group Group
			{
				get { return _group; }
			}

			public GroupItem(Sport.Championships.Group group)
			{
				_group = group;
			}

			public override int GetHashCode()
			{
				return base.GetHashCode ();
			}

			public override bool Equals(object obj)
			{
				if (obj is Sport.Championships.Group)
					return _group.Equals(obj);

				if (obj is GroupItem)
					return _group.Equals(((GroupItem) obj).Group);

				return base.Equals (obj);
			}


			public override string ToString()
			{
				return _group.Name;
			}

		}

		private int GetGroupSize(Sport.Championships.Group group)
		{
			int size = 0;
			for (int n = 0; n < rows.Length; n++)
			{
				if (group == rows[n].Group)
					size++;
			}

			return size;
		}

		public System.Windows.Forms.Control Edit(int row, int field)
		{
			switch (field)
			{
				case (1): // group
					if (ncbGroup == null)
					{
						ncbGroup = new Sport.UI.Controls.NullComboBox();
						ncbGroup.Text = null;
						ncbGroup.Items.Add(Sport.UI.Controls.NullComboBox.Null);
						foreach (Sport.Championships.Group group in _championship.Phases[_phase].Groups)
						{
							ncbGroup.Items.Add(new GroupItem(group));
						}
					}

					ncbGroup.SelectedItem = rows[row].Group == null ?
						Sport.UI.Controls.NullComboBox.Null : new GroupItem(rows[row].Group);
					ncbGroup.Tag = row;

					return ncbGroup;
				case (2): // position
					if (tcPosition == null)
					{
						tcPosition = new Sport.UI.Controls.TextControl();
						tcPosition.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
						tcPosition.ShowSpin = true;
					}

					if (rows[row].Group == null)
						return null;

					tcPosition.Controller = new Sport.UI.Controls.NumberController(
						1, GetGroupSize(rows[row].Group));

					tcPosition.Tag = row;
					tcPosition.Value = rows[row].Position + 1;

					return tcPosition;
			}

			return null;
		}

		public void EditEnded(System.Windows.Forms.Control control)
		{
			if (rows == null)
				return;
			
			if (control == ncbGroup)
			{
				if (ncbGroup.Tag == null)
					return;
				
				int row = (int) ncbGroup.Tag;
				GroupItem gi = ncbGroup.SelectedItem as GroupItem;
				Sport.Championships.Group newGroup = gi == null ? null :
					gi.Group;
				
				if (rows[row] == null)
					return;
				
				if (rows[row].Group != newGroup)
				{
					for (int n = 0; n < rows.Length; n++)
					{
						if (row != n)
						{
							if ((rows[n] != null)&&(rows[n].Group == rows[row].Group) &&
								(rows[n].Position > rows[row].Position))
							{
								rows[n].Position--;
							}
						}
					}
					
					if (newGroup == null)
					{
						rows[row].Group = null;
						rows[row].Position = -1;
					}
					else
					{
						rows[row].Group = newGroup;
						rows[row].Position = GetGroupSize(rows[row].Group) - 1;
					}
				}
			}
			else if (control == tcPosition)
			{
				if ((tcPosition.Tag == null)||(tcPosition.Value == null))
					return;
				
				int row = (int) tcPosition.Tag;
				int newPos = (int) (double) tcPosition.Value - 1;
				
				if (rows[row] == null)
					return;
				
				for (int n = 0; n < rows.Length; n++)
				{
					if (row != n)
					{
						if ((rows[n] != null)&&(rows[n].Group == rows[row].Group))
						{
							if (rows[n].Position > rows[row].Position)
							{
								rows[n].Position--;
							}
							if (rows[n].Position >= newPos)
							{
								rows[n].Position++;
							}
						}
					}
				}

				rows[row].Position = newPos;
			}
		}

		public int GetRowCount()
		{
			return rows.Length;
		}

		public string GetText(int row, int field)
		{
			switch (field)
			{
				case (0):
					return rows[row].GetName();
				case (1):
					return rows[row].Group == null ? null : rows[row].Group.Name;
				case (2):
					return rows[row].Position == -1 ? null : (rows[row].Position + 1).ToString();
			}

			return null;
		}

		public int GetFieldCount(int row)
		{
			return 3;
		}

		#endregion

		private void cbPreviousPhase_CheckedChanged(object sender, System.EventArgs e)
		{
			ShowPreviousPhaseTeams(cbPreviousPhase.Checked);
		}

	}
}
