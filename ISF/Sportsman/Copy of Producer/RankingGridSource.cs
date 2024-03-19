using System;
using Sport.Rulesets.Rules;

namespace Sportsman.Producer
{
	public class RankingGridSource : Sport.UI.Controls.IGridSource
	{
		// The groupStyle will be given to rows presenting the group name
		private static Sport.UI.Controls.Style groupStyle;
		private static Sport.UI.Controls.Style selectedGroupStyle;
		static RankingGridSource()
		{
			groupStyle = new Sport.UI.Controls.Style();
			groupStyle.Background = new System.Drawing.SolidBrush(
				System.Drawing.Color.FromArgb(50, 150, 196));
			groupStyle.Foreground = System.Drawing.Brushes.White;
			selectedGroupStyle = new Sport.UI.Controls.Style();
			selectedGroupStyle.Background = new System.Drawing.SolidBrush(
				System.Drawing.Color.FromArgb(25, 120, 146));
			selectedGroupStyle.Foreground = System.Drawing.Brushes.White;
		}

		private Sport.UI.Controls.Grid		_grid;
		private Sport.Championships.Phase	_phase;
		private RankingTable				_rankingTable;

		private object[] _rows;

		public void Rebuild()
		{
			if (_phase == null)
			{
				_rows = new object[0];
			}
			else
			{
				int count = 0;
				foreach (Sport.Championships.Group group in _phase.Groups)
					count += group.Teams.Count + 1;

				_rows = new object[count];

				int i = 0;
				int s;

				foreach (Sport.Championships.Group group in _phase.Groups)
				{
					_rows[i] = group;
					i++;

					s = i;

					foreach (Sport.Championships.Team team in group.Teams)
					{
						_rows[i] = team;
						i++;
					}

					Array.Sort(_rows, s, i - s, new Sport.Championships.TeamPositionComparer());
				}
			}

			if (_rows.Length > 0)
				_grid.ExpandedRows.Add(0, _rows.Length - 1);
			_grid.RefreshSource();
		}

		public Sport.Championships.Phase Phase
		{
			get { return _phase; }
			set
			{
				if (_phase != value)
				{
					_phase = value;
					object rule=null;
					if (_phase != null)
					{
						if (Sport.Core.Session.Connected)
						{
							rule = _phase.Championship.ChampionshipCategory.GetRule(
								typeof(Sport.Rulesets.Rules.RankingTables));
						}
						else
						{
							rule = Sport.Rulesets.Ruleset.LoadOfflineRule(
								typeof(Sport.Rulesets.Rules.RankingTablesRule), 
								_phase.Championship.CategoryID, -1);
						}
					}

					_rankingTable = null;
					RankingTables rankingTables = rule as RankingTables;
					if (rankingTables != null)
					{
						int ruleTypeId = Sport.Rulesets.RuleType.GetRuleType(typeof(RankingTables)).Id;
						string tableName = _phase.Definitions.Get(ruleTypeId, RankingTablesRule.PhaseTable);
						if (tableName == null)
						{
							_rankingTable = rankingTables.DefaultRankingTable;
						}
						else
						{
							for (int n = 0; n < rankingTables.Tables.Count && _rankingTable == null; n++)
							{
								if (rankingTables.Tables[n].Name == tableName)
								{
									_rankingTable = rankingTables.Tables[n];
								}
							}
						}
					}
					
					Rebuild();
				}
			}
		}

		public RankingGridSource()
		{
			_phase = null;
			_rankingTable = null;
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
			}
		}

		public int GetRowCount()
		{
			return _rows.Length;
		}

		public int GetFieldCount(int row)
		{
			if (_rows[row] is Sport.Championships.Group)
				return 1;

			return  _rankingTable == null ? 2 : _rankingTable.Fields.Count + 2;
		}

		public int GetGroup(int row)
		{
			if (_rows[row] is Sport.Championships.Group)
				return 0;
			return 1;
		}

		public string GetText(int row, int field)
		{
			Sport.Championships.Group group = _rows[row] as Sport.Championships.Group;
			if (group != null)
			{
				return group.Name;
			}

			Sport.Championships.Team team = (Sport.Championships.Team) _rows[row];

			switch (field)
			{
				case (0): // position
					return (team.Position + 1).ToString();
				case (1): // team
					return team.Name;
				default: // RankFields rule field
					if (_rankingTable != null && field > 1 && field - 2 < _rankingTable.Fields.Count)
					{
						Sport.Common.EquationVariables var = new Sport.Common.EquationVariables();
						team.SetFields(var);
						return _rankingTable.Fields[field - 2].Evaluate(var);
					}
					break;
			}

			return null;
		}

		public Sport.UI.Controls.Style GetStyle(int row, int field, Sport.UI.Controls.GridDrawState state)
		{
			if (_rows[row] is Sport.Championships.Group)
				return (state & Sport.UI.Controls.GridDrawState.Selected) == 0 ?
				groupStyle : selectedGroupStyle;

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

		public Sport.Championships.Team GetTeam(int row)
		{
			return _rows[row] as Sport.Championships.Team;
		}

		private Sport.Championships.Group _group;
		public Sport.Championships.Group SelectedGroup
		{
			get { return _group; }
		}
		
		private Sport.Championships.Team _team;
		public Sport.Championships.Team SelectedTeam
		{
			get { return _team; }
		}

        public event System.EventHandler SelectionChanged;

		private void ResetSelection()
		{
			Sport.Championships.Group group = null;
			Sport.Championships.Team team = null;

			if (_grid.Selection.Size == 1)
			{
				int row = _grid.Selection.Rows[0];
				group = _rows[row] as Sport.Championships.Group;
				if (group == null)
				{
					team = (Sport.Championships.Team) _rows[row];
					group = team.Group;
				}
			}

			bool selectionChanged = false;

			if (group != _group)
			{
				_group = group;
				selectionChanged = true;
			}
			if (team != _team)
			{
				_team = team;
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
