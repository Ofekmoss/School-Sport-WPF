using System;

namespace Sportsman.Producer
{
	public class MatchesGridPanel : System.Windows.Forms.Panel,
		Sport.UI.Controls.IGridSource
	{
		private Sport.UI.Controls.Grid			_grid;

		public MatchesGridPanel()
		{
			_rows = new object[0];

			_grid = new Sport.UI.Controls.Grid();

			_grid.Dock = System.Windows.Forms.DockStyle.Fill;
			_grid.GroupingStyle = Sport.UI.Controls.GridGroupingStyle.List;
			_grid.ExpandOnDoubleClick = false;
			_grid.SelectionMode = System.Windows.Forms.SelectionMode.One;
			_grid.Source = this;
			_grid.SelectionChanged += new EventHandler(GridSelectionChanged);
			_grid.MouseUp += new System.Windows.Forms.MouseEventHandler(GridMouseUp);
			_grid.DoubleClick += new EventHandler(GridDoubleClick);
			_grid.ColumnMoved += new Sport.UI.Controls.Grid.ColumnMoveEventHandler(GridColumnMoved);

			Controls.Add(_grid);

			_phase = null;
			_group = null;
			_round = null;
			_cycle = null;
			_match = null;
			_tournament = null;

			_showGroups = false;
			_groupGroups = true;
			_groupRounds = true;
			_groupCycles = true;
			_groupTournaments = false;

			_columns = new MatchVisualizer.MatchField[] { 
											MatchVisualizer.MatchField.Number, 
											MatchVisualizer.MatchField.TeamA,
											MatchVisualizer.MatchField.TeamB, 
											MatchVisualizer.MatchField.Time,
											MatchVisualizer.MatchField.Facility, 
											MatchVisualizer.MatchField.Result
										};

			ResetGridGroups();
		}

		#region Grouping

		private bool _showGroups;
		private bool _groupGroups;
		private bool _groupRounds;
		private bool _groupCycles;
		private bool _groupTournaments;

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

		public bool GroupTournaments
		{
			get { return _groupTournaments; }
			set
			{
				if (_groupTournaments != value)
				{
					_groupTournaments = value;

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

			if (_groupTournaments)
			{
				_grid.Groups.Insert(0);
				_grid.Groups[0].Columns.Add(0, "טורניר", 1);
			}
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

		#region Selection

		private Sport.Championships.MatchPhase	_phase;
		public Sport.Championships.MatchPhase Phase
		{
			get { return _phase; }
			set
			{
				if (_phase != value)
				{
					_phase = value;
					_group = null;
					_round = null;
					_cycle = null;
					_tournament = null;
					_match = null;
					Rebuild();
				}
			}
		}

		private Sport.Championships.MatchGroup	_group;
		public Sport.Championships.MatchGroup Group
		{
			get { return _group; }
			set
			{
				if (_group != value)
				{
					if (_showGroups)
					{
						SelectRow(value);
					}
					else
					{
						SelectRow(null);
						_group = value;
						_phase = _group == null ? null : _group.Phase;
						Rebuild();
					}
				}
			}
		}

		private Sport.Championships.Round _round;
		public Sport.Championships.Round Round
		{
			get { return _round; }
			set 
			{ 
				if (_round != value)
					SelectRow(value); 
			}
		}

		private Sport.Championships.Cycle _cycle;
		public Sport.Championships.Cycle Cycle
		{
			get { return _cycle; }
			set 
			{ 
				if (_cycle != value)
					SelectRow(value); 
			}
		}

		private Sport.Championships.Tournament _tournament;
		public Sport.Championships.Tournament Tournament
		{
			get { return _tournament; }
			set
			{
				if (_tournament != value)
					SelectRow(value);
			}
		}
		
		private Sport.Championships.Match _match;
		public Sport.Championships.Match Match
		{
			get { return _match; }
			set 
			{ 
				if (_match != value)
					SelectRow(value); 
			}
		}

		private object _selectedRow;
		public object SelectedRow
		{
			get { return _selectedRow; }
			set 
			{ 
				if (_selectedRow != value)
					SelectRow(value);
			}
		}

		private void SelectRow(object row)
		{
			if (row != null)
			{
				for (int n = 0; n < _rows.Length; n++)
				{
					if (_rows[n] == row)
					{
						_grid.SelectRow(n);
						_grid.ScrollToRow(n);
						return ;
					}
				}
			}

			_grid.SelectRow(-1);
		}

		public event System.EventHandler SelectionChanged;

		private void ResetSelection()
		{
			Sport.Championships.MatchGroup group = null;
			Sport.Championships.Round round = null;
			Sport.Championships.Cycle cycle = null;
			Sport.Championships.Tournament tournament = null;
			Sport.Championships.Match match = null;
			
			if (_grid == null)
				return;
			
			if ((_grid.Selection == null)||(_grid.Selection.Rows == null))
				return;
			
			if (_grid.Selection.Size == 1)
			{
				int row = _grid.Selection.Rows[0];
				_selectedRow = _rows[row];
				group = _selectedRow as Sport.Championships.MatchGroup;
				if (group == null)
				{
					round = _selectedRow as Sport.Championships.Round;
					if (round == null)
					{
						cycle = _selectedRow as Sport.Championships.Cycle;
						if (cycle == null)
						{
							tournament = _selectedRow as Sport.Championships.Tournament;
							if (tournament == null)
							{
								match = (Sport.Championships.Match) _selectedRow;
								if (match != null)
								{
									cycle = match.Cycle;
									tournament = null;
									if (match.Tournament >= 0 && cycle != null)
									{
										if (cycle.Tournaments != null)
											tournament = cycle.Tournaments[match.Tournament];
									}
								}
							}
							else
							{
								cycle = tournament.Cycle;
							}
						}
						if (cycle != null)
							round = cycle.Round;
					}
					if (round != null)
						group = round.Group;
				}
			}
			else
			{
				_selectedRow = null;
			}

			bool selectionChanged = false;

			if (_group != group && _showGroups)
			{
				_group = group;
				selectionChanged = true;
			}
			if (_round != round)
			{
				_round = round;
				selectionChanged = true;
			}
			if (_cycle != cycle)
			{
				_cycle = cycle;
				selectionChanged = true;
			}
			if (_tournament != tournament)
			{
				_tournament = tournament;
				selectionChanged = true;
			}
			if (_match != match)
			{
				_match = match;
				selectionChanged = true;
			}

			if (selectionChanged && SelectionChanged != null)
				SelectionChanged(this, EventArgs.Empty);
		}

		private void GridSelectionChanged(object sender, EventArgs e)
		{
			ResetSelection();
		}

		#endregion

		#region Rows

		private object[] _rows;

		private bool rebuildLocked = false;
		public void LockRebuild(bool lockRebuild)
		{
			if (lockRebuild != rebuildLocked)
			{
				rebuildLocked = lockRebuild;
				Rebuild();
			}
		}


		public void Rebuild()
		{
			if (rebuildLocked)
				return ;

			Sport.UI.Dialogs.WaitForm.ShowWait("טוען משחקים אנא המתן...", true);
			Sport.UI.Dialogs.WaitForm.SetProgress(0);

			System.Collections.ArrayList rows = new System.Collections.ArrayList();

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
					Sport.UI.Dialogs.WaitForm.SetProgress(30);
				}
				else if (_group != null)
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
					Sport.UI.Dialogs.WaitForm.SetProgress(30);
				}
			}

			if (_groupTournaments)
			{
				int start = 0;
				int end;
				int tournament;
				System.Collections.IComparer comparer = new Sport.Championships.MatchComparer();

				while (start < rows.Count)
				{
					while (start < rows.Count && !(rows[start] is Sport.Championships.Match))
						start++;
					if (start < rows.Count)
					{
						Sport.Championships.Cycle cycle = ((Sport.Championships.Match) rows[start]).Cycle;
						end = start;
						while (end < rows.Count && rows[end] is Sport.Championships.Match &&
							((Sport.Championships.Match) rows[end]).Cycle == cycle)
							end++;

						rows.Sort(start, end - start, comparer);

						tournament = -1;

						while (start < end)
						{
							Sport.Championships.Match match = (Sport.Championships.Match) rows[start];
							if (tournament == match.Tournament)
							{
								start++;
							}
							else
							{
								tournament = match.Tournament;
								rows.Insert(start, match.Cycle.Tournaments[tournament]);
								start += 2;
								end++;
							}
						}
					}
				}
			}
			Sport.UI.Dialogs.WaitForm.SetProgress(60);
			_rows = rows.ToArray();

			if (_rows.Length > 0)
				_grid.ExpandedRows.Add(0, _rows.Length - 1);
			_grid.RefreshSource();
			Sport.UI.Dialogs.WaitForm.SetProgress(90);

			SelectRow(_selectedRow);
			
			Sport.UI.Dialogs.WaitForm.SetProgress(100);
			Sport.UI.Dialogs.WaitForm.HideWait();
		}

		#endregion

		#region Columns

		private MatchVisualizer matchVisualizer = new MatchVisualizer();

		private MatchVisualizer.MatchField[] _columns;
		public MatchVisualizer.MatchField[] Columns
		{
			get { return _columns; }
			set
			{
				_columns = value;
				ResetGridColumns();
			}
		}

		public Sport.UI.IVisualizer Visualizer
		{
			get { return matchVisualizer; }
		}

		private void GridColumnMoved(object sender, Sport.UI.Controls.Grid.ColumnMoveEventArgs e)
		{
			Sport.UI.Controls.Grid.GridGroup matchesGroup = 
				_grid.Groups[_grid.Groups.Count - 1];

			_columns = new MatchVisualizer.MatchField[matchesGroup.Columns.Count];
			for (int n = 0; n < matchesGroup.Columns.Count; n++)
			{
				_columns[n] = (MatchVisualizer.MatchField) matchesGroup.Columns[n].Field;
			}
		}

		private void ResetGridColumns()
		{
			Sport.UI.Controls.Grid.GridGroup matchesGroup = 
				_grid.Groups[_grid.Groups.Count - 1];

			matchesGroup.Columns.Clear();

			foreach (MatchVisualizer.MatchField field in _columns)
			{
				Sport.UI.IVisualizerField visualField = matchVisualizer.GetField((int) field);
				matchesGroup.Columns.Add(visualField.Field, visualField.Title, visualField.DefaultWidth, visualField.Alignment);
			}

			_grid.Refresh();
		}

		#endregion

		#region IGridSource

		public void SetGrid(Sport.UI.Controls.Grid grid)
		{
		}

		public int GetRowCount()
		{
			return _rows.Length;
		}

		public int GetFieldCount(int row)
		{
			if (_rows[row] is Sport.Championships.Match)
				return 9;

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
			if (_groupTournaments)
			{
				if (_rows[row] is Sport.Championships.Tournament || _rows[row] == null)
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

			if (_groupTournaments)
			{
				Sport.Championships.Tournament tournament = _rows[row] as Sport.Championships.Tournament;
				if (tournament != null)
				{
					string stour = "טורניר " + tournament.Number.ToString();
					return stour;
				}
			}

			Sport.Championships.Match m = _rows[row] as Sport.Championships.Match;

			return matchVisualizer.GetText(m, (MatchVisualizer.MatchField) field);
		}
		
		private Sport.UI.Controls.Style _winnerStyle = new Sport.UI.Controls.Style(
			new System.Drawing.SolidBrush(System.Drawing.Color.Red), null, null);
		
		public Sport.UI.Controls.Style GetStyle(int row, int field, Sport.UI.Controls.GridDrawState state)
		{
			Sport.Championships.MatchGroup group = _rows[row] as Sport.Championships.MatchGroup;
			if (group != null)
				return null;
			
			Sport.Championships.Round round = _rows[row] as Sport.Championships.Round;
			if (round != null)
				return null;

			Sport.Championships.Cycle cycle = _rows[row] as Sport.Championships.Cycle;
			if (cycle != null)
				return null;
			
			if (_groupTournaments)
			{
				Sport.Championships.Tournament tournament = _rows[row] as Sport.Championships.Tournament;
				if (tournament != null)
					return null;
			}

			Sport.Championships.Match match = _rows[row] as Sport.Championships.Match;
			MatchVisualizer.MatchField matchField = (MatchVisualizer.MatchField) field;
			Sport.Championships.MatchOutcome outcome = match.Outcome;
			Sport.Championships.MatchOutcome winA = Sport.Championships.MatchOutcome.WinA;
			Sport.Championships.MatchOutcome winB = Sport.Championships.MatchOutcome.WinB;
			Sport.Championships.MatchOutcome technicalA = Sport.Championships.MatchOutcome.TechnicalA;
			Sport.Championships.MatchOutcome technicalB = Sport.Championships.MatchOutcome.TechnicalB;
			switch (matchField)
			{
				case MatchVisualizer.MatchField.TeamA:
					if (outcome == winA || outcome == technicalA)
						return _winnerStyle;
					break;
				case MatchVisualizer.MatchField.TeamB:
					if (outcome == winB || outcome == technicalB)
						return _winnerStyle;
					break;
			}
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
		
		private Sport.UI.Controls.GenericItem gridEditGenericItem=null;
		public System.Windows.Forms.Control Edit(int row, int field)
		{
			if (field != ((int) MatchVisualizer.MatchField.RefereeCount))
				return null;
			
			if (_rows == null)
				return null;
			
			if ((row < 0)||(row >= _rows.Length))
				return null;
			
			Sport.Championships.Match match = _rows[row] as Sport.Championships.Match;
			if (match == null)
				return null;
			
			if (gridEditGenericItem != null)
				gridEditGenericItem.ValueChanged -= new EventHandler(GridEditItemValueChanged);
			
			gridEditGenericItem = new Sport.UI.Controls.GenericItem(
				Sport.UI.Controls.GenericItemType.Number, 
				match.RefereeCount, null);
			
			gridEditGenericItem.Tag = row;
			gridEditGenericItem.Nullable = false;
			gridEditGenericItem.ValueChanged += new EventHandler(GridEditItemValueChanged);

			return gridEditGenericItem.Control;
		}
		
		private void GridEditItemValueChanged(object sender, EventArgs e)
		{
		}
		
		public void EditEnded(System.Windows.Forms.Control control)
		{
			//int row=(int) gridEditGenericItem.Tag;
			
			Sport.Championships.Match match=Match;
			if (match == null)
				return;
			
			int refCount=Sport.Common.Tools.CIntDef(gridEditGenericItem.Value, 0);
			if (refCount == match.RefereeCount)
				return;
			
			if (match.SetRefereeCount(refCount))
				this.Rebuild();
		}

		#endregion

		protected override void Dispose(bool disposing)
		{
			_rows = null;
			base.Dispose (disposing);
		}

		private void GridMouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (e.Button == System.Windows.Forms.MouseButtons.Right && 
				_phase != null && !_phase.Championship.Editing)
			{
				if (_selectedRow is Sport.Championships.Cycle ||
					_selectedRow is Sport.Championships.Round)
				{
					System.Windows.Forms.ContextMenu menu = new System.Windows.Forms.ContextMenu(
						new System.Windows.Forms.MenuItem[] {
																new System.Windows.Forms.MenuItem("מועדים ומתקנים", new EventHandler(TimeFacilityClick))
															});
					menu.Show(this, new System.Drawing.Point(e.X, e.Y));
				}
			}
		}

		private void GridDoubleClick(object sender, EventArgs e)
		{
			if (!_phase.Championship.Editing)
			{
				if (_selectedRow is Sport.Championships.Match)
				{
					MatchForm mf = new MatchForm(_match);
					if (mf.ShowDialog() == System.Windows.Forms.DialogResult.OK)
						Rebuild();
				}
				else if (_selectedRow is Sport.Championships.Cycle ||
					_selectedRow is Sport.Championships.Round || 
					_selectedRow is Sport.Championships.Tournament)
				{
					SetMatchesTimeFacility();
				}
			}
		}

		private void SetMatchesTimeFacility()
		{
			System.Collections.ArrayList al = new System.Collections.ArrayList();

			string owner = "";
			Sport.Championships.MatchComparer comparer = new Sport.Championships.MatchComparer();
			
			System.Collections.ArrayList cycles=new System.Collections.ArrayList(); 
			
			if (_selectedRow is Sport.Championships.Cycle)
			{
				owner = _cycle.Name;
				cycles.Add(_cycle);
			}
			else
			{
				if (_selectedRow is Sport.Championships.Round)
				{
					owner = _round.Name;
					foreach (Sport.Championships.Cycle cycle in _round.Cycles)
						cycles.Add(cycle);
				}
				else
				{
					if (_selectedRow is Sport.Championships.Tournament)
					{
						owner = "טורניר "+_tournament.Number;
						cycles.Add(_tournament.Cycle);
					}
				}
			}
			
			foreach (Sport.Championships.Cycle curCycle in cycles)
			{
				int start = al.Count;
				foreach (Sport.Championships.Match match in curCycle.Matches)
				{
					if ((_selectedRow is Sport.Championships.Tournament)&&
						(match.Tournament != _tournament.Index))
					{
						continue;
					}
					al.Add(match);
				}
				if (al.Count > start)
					al.Sort(start, al.Count - start, comparer);
			}
			
			if (al.Count > 0)
			{
				if (matchesTimeFacilityForm == null)
				{
					matchesTimeFacilityForm = new MatchesTimeFacilityForm();
				}

				matchesTimeFacilityForm.SetMatches(owner, (Sport.Championships.Match[]) al.ToArray(typeof(Sport.Championships.Match)));

				if (matchesTimeFacilityForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					Rebuild();
				}
			}
		}

		private MatchesTimeFacilityForm matchesTimeFacilityForm = null;
		private void TimeFacilityClick(object sender, EventArgs e)
		{
			if (_phase.Championship.Editing)
				return ;

			SetMatchesTimeFacility();
		}
	}
}
