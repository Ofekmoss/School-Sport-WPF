using System;
using System.Collections;

namespace Sportsman.Producer
{
	public class MatchesGridSource : Sport.UI.Controls.IGridSource
	{
		private Sport.UI.Controls.Grid			_grid;

		#region Rows

		private object[] _rows;

		public void Rebuild()
		{
			ArrayList rows = new ArrayList();

			if (_phase != null)
			{
				if (_showGroups)
				{
					foreach (Sport.Championships.MatchGroup group in _phase.Groups)
					{
						if (_groupGroups)
						{
							rows.Add(group);
						}

						foreach (Sport.Championships.Round round in group.Rounds)
						{
							if (_groupRounds)
							{
								rows.Add(round);
							}

							foreach (Sport.Championships.Cycle cycle in round.Cycles)
							{
								if (_groupCycles)
								{
									rows.Add(cycle);
								}

								foreach (Sport.Championships.Match m in cycle.Matches)
								{
									rows.Add(m);
								}
							}
						}
					}
				}
				else
				{
					foreach (Sport.Championships.Round round in _group.Rounds)
					{
						if (_groupRounds)
						{
							rows.Add(round);
						}
						foreach (Sport.Championships.Cycle cycle in round.Cycles)
						{
							if (_groupCycles)
							{
								rows.Add(cycle);
							}
							foreach (Sport.Championships.Match m in cycle.Matches)
							{
								rows.Add(m);
							}
						}
					}
				}
			}

			_rows = rows.ToArray();

			if (_rows.Length > 0)
				_grid.ExpandedRows.Add(0, _rows.Length - 1);
			_grid.RefreshSource();
		}

		#endregion

		#region Matches Source Properties

		private Sport.Championships.MatchGroup	_group;
		private Sport.Championships.MatchPhase	_phase;

		public Sport.Championships.MatchGroup Group
		{
			get { return _group; }
			set
			{
				if (_group != value)
				{
					_group = value;
					_phase = _group == null ? null : _group.Phase;
					Rebuild();
				}
			}
		}

		public Sport.Championships.MatchPhase Phase
		{
			get { return _phase; }
			set
			{
				if (_phase != value)
				{
					_phase = value;
					if (_group != null && _group.Phase != _phase)
						_group = null;
					Rebuild();
				}
			}
		}

		#endregion

		#region Grouping Properties

		private bool _showGroups;
		private bool _groupGroups;
		private bool _groupRounds;
		private bool _groupCycles;

		public bool ShowGroups
		{
			get { return _showGroups; }
			set
			{
				if (_showGroups != value)
				{
					_showGroups = value;

					ResetGridGroups();
					Rebuild();
				}
			}
		}

		public bool GroupGroups
		{
			get { return _groupGroups; }
			set
			{
				if (_groupGroups != value)
				{
					_groupGroups = value;

					if (_showGroups) // otherwise no effect for grouping groups
					{
						ResetGridGroups();
						Rebuild();
					}
				}
			}
		}

		public bool GroupRounds
		{
			get { return _groupRounds; }
			set
			{
				if (_groupRounds != value)
				{
					_groupRounds = value;

					ResetGridGroups();
					Rebuild();
				}
			}
		}

		public bool GroupCycles
		{
			get { return _groupCycles; }
			set
			{
				if (_groupCycles != value)
				{
					_groupCycles = value;

					ResetGridGroups();
					Rebuild();
				}
			}
		}

		private void ResetGridGroups()
		{
			_grid.Groups.Clear();
			_grid.Groups[0].RowHeight *= 2;
			ResetGridColumns();

			if (_groupCycles)
			{
				_grid.Groups.Insert(0);
				_grid.Groups[0].Columns.Add(0, "מחזור", 1);
			}
			if (_groupRounds)
			{
				_grid.Groups.Insert(0);
				_grid.Groups[0].Columns.Add(0, "סיבוב", 1);
			}
			if (_groupGroups && _showGroups)
			{
				_grid.Groups.Insert(0);
				_grid.Groups[0].Columns.Add(0, "בית", 1);
			}
		}

		#endregion

		#region Matches Columns

		public enum MatchField
		{
			Group,
			Round,
			Cycle,
			Number,
			TeamA,
			TeamB,
			Time,
			Facility,
			Result
		}

		private MatchField[] _columns;
		public MatchField[] Columns
		{
			get { return _columns; }
			set
			{
				_columns = value;
				ResetGridColumns();
			}
		}

		public readonly Sport.UI.Controls.Grid.GridColumn[] FieldColumns = new Sport.UI.Controls.Grid.GridColumn[]
			{
				new Sport.UI.Controls.Grid.GridColumn(0, "בית", 40),
				new Sport.UI.Controls.Grid.GridColumn(1, "סיבוב", 40),
				new Sport.UI.Controls.Grid.GridColumn(2, "מחזור", 40),
				new Sport.UI.Controls.Grid.GridColumn(3, "מספר", 40),
				new Sport.UI.Controls.Grid.GridColumn(4, "קבוצה מארחת", 130),
				new Sport.UI.Controls.Grid.GridColumn(5, "קבוצה אורחת", 130),
				new Sport.UI.Controls.Grid.GridColumn(6, "מועד", 100),
				new Sport.UI.Controls.Grid.GridColumn(7, "מתקן", 100),
				new Sport.UI.Controls.Grid.GridColumn(8, "תוצאה", 80, System.Drawing.StringAlignment.Center)
			};

		private void ResetGridColumns()
		{
			Sport.UI.Controls.Grid.GridGroup matchesGroup = 
				_grid.Groups[_grid.Groups.Count - 1];

			matchesGroup.Columns.Clear();

			foreach (MatchField field in _columns)
			{
				matchesGroup.Columns.Add(FieldColumns[(int) field]);
			}

			_grid.Refresh();
		}

		#endregion

		public MatchesGridSource()
		{
			_group = null;
			_phase = null;

			_showGroups = false;
			_groupGroups = true;
			_groupRounds = true;
			_groupCycles = true;

			_columns = new MatchField[] { 
											MatchField.Number, MatchField.TeamA,
											MatchField.TeamB, MatchField.Time,
											MatchField.Facility, MatchField.Result
										};

			_rows = new object[0];
		}

		public void SetGrid(Sport.UI.Controls.Grid grid)
		{
			if (_grid != null)
			{
				_grid.SelectionChanged -= new EventHandler(GridSelectionChanged);
			}

			_grid = grid;

			if (_grid != null)
			{
				_grid.SelectionChanged += new EventHandler(GridSelectionChanged);
				ResetGridGroups();
			}
		}

		public int GetRowCount()
		{
			return _rows.Length;
		}

		public int GetFieldCount(int row)
		{
			if (_rows[row] is Sport.Championships.Match)
				return 5;

			return 1;
		}

		public int GetGroup(int row)
		{
			int group = 0;
			if (_groupGroups && _showGroups)
			{
				if (_rows[row] is Sport.Championships.MatchGroup)
					return group;
				group++;
			}
			if (_groupRounds)
			{
				if (_rows[row] is Sport.Championships.Round)
					return group;
				group++;
			}
			if (_groupCycles)
			{
				if (_rows[row] is Sport.Championships.Cycle)
					return group;
				group++;
			}
			return group;
		}

		public string GetText(int row, int field)
		{
			Sport.Championships.MatchGroup group = _rows[row] as Sport.Championships.MatchGroup;
			if (group != null)
				return group.Name;

			Sport.Championships.Round round = _rows[row] as Sport.Championships.Round;
			if (round != null)
			{
				if (!_groupGroups && _showGroups)
				{
					return round.Group.Name + " - " + round.Name;
				}

				return round.Name;
			}

			Sport.Championships.Cycle cycle = _rows[row] as Sport.Championships.Cycle;
			if (cycle != null)
			{
				if (!_groupRounds)
				{
					if (!_groupGroups && _showGroups)
						return cycle.Round.Group.Name + " - " + cycle.Round.Name + " - " + cycle.Name;

					return cycle.Round.Name + " - " + cycle.Name;
				}

				return cycle.Name;
			}

			Sport.Championships.Match m = _rows[row] as Sport.Championships.Match;

			switch ((MatchField) field)
			{
				case MatchField.Group:
					return m.Cycle.Round.Group.Name;
				case MatchField.Round:
					return m.Cycle.Round.Name;
				case MatchField.Cycle:
					return m.Cycle.Name;
				case MatchField.Number:
					return m.Number.ToString();
				case MatchField.TeamA:
					//return m.ToString();// TeamA.Name + " -\n" + e.TeamB.Name;
					return m.GetTeamAName();
				case MatchField.TeamB:
					return m.GetTeamBName();
				case MatchField.Time:
					if (m.Time == DateTime.MinValue)
						return null;
					return m.Time.ToString("g");
				case MatchField.Facility:
					if (m.Court != null)
					{
						return m.Facility.Name + " - " + m.Court.Name;
					}
					if (m.Facility != null)
					{
						return m.Facility.Name;
					}
					return null;
				case MatchField.Result:
					switch (m.Outcome)
					{
						case (Sport.Championships.MatchOutcome.Tie):
						case (Sport.Championships.MatchOutcome.WinA):
						case (Sport.Championships.MatchOutcome.WinB):
							return m.TeamBScore.ToString() + "-" + m.TeamAScore.ToString();
						case (Sport.Championships.MatchOutcome.TechnicalA):
							return "ניצחון טכני א (" + m.TeamBScore.ToString() + "-" + m.TeamAScore.ToString() + ")";
						case (Sport.Championships.MatchOutcome.TechnicalB):
							return "ניצחון טכני ב (" + m.TeamBScore.ToString() + "-" + m.TeamAScore.ToString() + ")";
					}
					return null;
			}

			return null;
		}

		public Sport.UI.Controls.Style GetStyle(int row, int field, Sport.UI.Controls.GridDrawState state)
		{
			return null;
		}

		public string GetTip(int row)
		{
			return null;
		}

		public int[] GetSort(int group)
		{
			return null;
		}

		public void Sort(int group, int[] columns)
		{
		}

		public System.Windows.Forms.Control Edit(int row, int field)
		{
			return null;
		}

		public void EditEnded(System.Windows.Forms.Control control)
		{
		}

		public void Dispose()
		{
			_rows = null;
		}

		private Sport.Championships.MatchGroup _selectedGroup;
		public Sport.Championships.MatchGroup SelectedGroup
		{
			get { return _selectedGroup; }
		}

		private Sport.Championships.Round _selectedRound;
		public Sport.Championships.Round SelectedRound
		{
			get { return _selectedRound; }
		}

		private Sport.Championships.Cycle _selectedCycle;
		public Sport.Championships.Cycle SelectedCycle
		{
			get { return _selectedCycle; }
		}
		
		private Sport.Championships.Match _selectedMatch;
		public Sport.Championships.Match SelectedMatch
		{
			get { return _selectedMatch; }
		}

		public event System.EventHandler SelectionChanged;

		private void ResetSelection()
		{
			Sport.Championships.MatchGroup group = null;
			Sport.Championships.Round round = null;
			Sport.Championships.Cycle cycle = null;
			Sport.Championships.Match match = null;

			if (_grid.Selection.Size == 1)
			{
				int row = _grid.Selection.Rows[0];
                group = _rows[row] as Sport.Championships.MatchGroup;
				if (group == null)
				{
					round = _rows[row] as Sport.Championships.Round;
					if (round == null)
					{
						cycle = _rows[row] as Sport.Championships.Cycle;
						if (cycle == null)
						{
							match = (Sport.Championships.Match) _rows[row];
							cycle = match.Cycle;
						}
						round = cycle.Round;
					}
					group = round.Group;
				}
			}

			bool selectionChanged = false;

			if (group != _selectedGroup)
			{
				_selectedGroup = group;
				selectionChanged = true;
			}
			if (round != _selectedRound)
			{
				_selectedRound = round;
				selectionChanged = true;
			}
			if (cycle != _selectedCycle)
			{
				_selectedCycle = cycle;
				selectionChanged = true;
			}
			if (match != _selectedMatch)
			{
				_selectedMatch = match;
				selectionChanged = true;
			}

			if (selectionChanged && SelectionChanged != null)
				SelectionChanged(this, EventArgs.Empty);
		}

		private void GridSelectionChanged(object sender, EventArgs e)
		{
			ResetSelection();
		}
	}
}
