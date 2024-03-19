using System;
using Sport.Rulesets.Rules;

namespace Sportsman.Producer
{
	public class RankingGridPanel : System.Windows.Forms.Panel,
		Sport.UI.Controls.IGridSource
	{
		private Sport.UI.Controls.Grid			_grid;

		public RankingGridPanel()
		{
			_rows = new object[0];

			_grid = new Sport.UI.Controls.Grid();

			_grid.Dock = System.Windows.Forms.DockStyle.Fill;
			_grid.SelectionMode = System.Windows.Forms.SelectionMode.One;
			_grid.SelectionChanged += new EventHandler(GridSelectionChanged);
			_grid.Source = this;

			_grid.Groups.Add();
			_grid.Groups[0].Columns.Add(0, "בית", 1);

			ResetRankingTable();

			Controls.Add(_grid);

			_phase = null;
			_group = null;
			_team = null;
		}

		#region Selection

		private Sport.Championships.Championship _championship;
		public Sport.Championships.Championship Championship
		{
			get { return _championship; }
			set
			{
				if (_championship != value)
				{
					if (_championship != null)
					{
						_championship.RulesetChanged -= new EventHandler(ChampionshipRulesetChanged);
					}

					_championship = value;

					if (_championship != null)
					{
						_championship.RulesetChanged += new EventHandler(ChampionshipRulesetChanged);
					}

					if (_phase != null && _phase.Championship != _championship)
					{
						Phase = null;
					}

					ResetRankingTable();
				}
			}
		}

		private Sport.Championships.Phase _phase;
		public Sport.Championships.Phase Phase
		{
			get { return _phase; }
			set
			{
				if (_phase != value)
				{
					if (_phase != null)
					{
						foreach (Sport.Championships.Group group in _phase.Groups)
						{
							group.TeamsScoreCalculated -= new EventHandler(GroupTeamsScoreCalculated);
						}
					}

					_phase = value;

					if (_phase != null)
					{
						foreach (Sport.Championships.Group group in _phase.Groups)
						{
							group.TeamsScoreCalculated += new EventHandler(GroupTeamsScoreCalculated);
						}
					}

					Rebuild();

					if (_phase != null && _phase.Championship != _championship)
					{
						Championship = _phase.Championship;
					}
					else
					{
						ResetRankingTable();
					}
				}
			}
		}

		private Sport.Championships.Group _group;
		public Sport.Championships.Group Group
		{
			get { return _group; }
			set
			{
				if (_group != value)
					SelectRow(value);
			}
		}

		private Sport.Championships.Team _team;
		public Sport.Championships.Team Team
		{
			get { return _team; }
			set
			{
				if (_team != value)
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
			object selectedRow = null;

			_group = null;
			_team = null;

			if (_grid != null && _grid.Selection != null && _grid.Selection.Size == 1)
			{
				int row = _grid.Selection.Rows[0];
				selectedRow = _rows[row];
				_group = selectedRow as Sport.Championships.MatchGroup;
				if (_group == null)
				{
					_team = selectedRow as Sport.Championships.Team;
					if (_team != null)
						_group = _team.Group;
				}
			}

			if (_selectedRow != selectedRow)
			{
				_selectedRow = selectedRow;
				if (SelectionChanged != null)
					SelectionChanged(this, EventArgs.Empty);
			}
		}

		private void GridSelectionChanged(object sender, EventArgs e)
		{
			ResetSelection();
		}
		
		#endregion

		#region Ranking Fields

		private void ChampionshipRulesetChanged(object sender, EventArgs e)
		{
			ResetRankingTable();
		}

		private RankingTable _rankingTable;

		private void ResetRankingTable()
		{
			_grid.Groups[1].Columns.Clear();
			_grid.Groups[1].Columns.Add(0, "מיקום", 20, System.Drawing.StringAlignment.Center);
			_grid.Groups[1].Columns.Add(1, "קבוצה", 100);
			
			_rankingTable = null;
			if (_championship != null)
			{
				object rule=null;
				if (Sport.Core.Session.Connected)
				{
					rule = _championship.ChampionshipCategory.GetRule(
						typeof(RankingTables));
				}
				else
				{
					rule = Sport.Rulesets.Ruleset.LoadOfflineRule(
						typeof(RankingTablesRule), 
						_championship.CategoryID, -1);
				}
				
				_rankingTable = null;
				RankingTables rankingTables = rule as RankingTables;
				if (rankingTables != null)
				{
					if (_phase != null)
					{
						Sport.Rulesets.RuleType type = Sport.Rulesets.RuleType.GetRuleType(typeof(RankingTables));
						if (type != null)
						{
							int ruleTypeId = type.Id;
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
					}
				}
				
				if (_rankingTable != null)
				{
					for (int n = 0; n < _rankingTable.Fields.Count; n++)
					{
						_grid.Groups[1].Columns.Add(n + 2, _rankingTable.Fields[n].Title, 20, System.Drawing.StringAlignment.Center);
					}
				}
			}
		}
		#endregion
		
		#region Rows

		private object[] _rows;

		public void Rebuild()
		{
			System.Collections.ArrayList rows = new System.Collections.ArrayList();

			if (_phase != null)
			{
				foreach (Sport.Championships.Group group in _phase.Groups)
				{
					rows.Add(group);

					foreach (Sport.Championships.Team team in group.Teams)
						rows.Add(team);
				}
			}

			_rows = rows.ToArray();

			if (_rows.Length > 0)
			{
				_grid.ExpandedRows.Add(0, _rows.Length - 1);
				foreach (Sport.Championships.Group group in _phase.Groups)
				{
					ResortGroup(group);
				}
			}
			_grid.RefreshSource();

			SelectRow(_selectedRow);
		}

		private void ResortGroup(Sport.Championships.Group group)
		{
			int s = -1, e = -1;

			for (int n = 0; n < _rows.Length && s == -1; n++)
			{
				if (_rows[n] == group)
				{
					s = n + 1;
				}
			}

			if (s != -1)
			{
				e = s + 1;

				while (e < _rows.Length && _rows[e] is Sport.Championships.Team)
					e++;

				if (s < _rows.Length)
					Array.Sort(_rows, s, e - s, new Sport.Championships.TeamPositionComparer());
			}
		}

		private void GroupTeamsScoreCalculated(object sender, EventArgs e)
		{
			ResortGroup(sender as Sport.Championships.Group);
		}

		#endregion
		
		#region IGridSource

		// The groupStyle will be given to rows presenting the group name
		private static Sport.UI.Controls.Style groupStyle;
		private static Sport.UI.Controls.Style selectedGroupStyle;
		static RankingGridPanel()
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
		
		public void SetGrid(Sport.UI.Controls.Grid grid)
		{
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

		#endregion
		
		protected override void Dispose(bool disposing)
		{
			if (_championship != null)
			{
				_championship.RulesetChanged -= new EventHandler(ChampionshipRulesetChanged);
			}

			if (_phase != null)
			{
				foreach (Sport.Championships.Group group in _phase.Groups)
				{
					group.TeamsScoreCalculated -= new EventHandler(GroupTeamsScoreCalculated);
				}
			}

			base.Dispose (disposing);
		}
	}
}
