using System;

namespace Sportsman.Producer
{
	public class CompetitionsGridPanel : System.Windows.Forms.Panel,
		Sport.UI.Controls.IGridSource
	{
		private Sport.UI.Controls.Grid			_grid;

		public CompetitionsGridPanel()
		{
			_rows = new object[0];

			_grid = new Sport.UI.Controls.Grid();

			_grid.Dock = System.Windows.Forms.DockStyle.Fill;
			_grid.SelectionMode = System.Windows.Forms.SelectionMode.One;
			_grid.Source = this;
			_grid.SelectionChanged += new EventHandler(GridSelectionChanged);
			_grid.MouseUp += new System.Windows.Forms.MouseEventHandler(GridMouseUp);
			_grid.DoubleClick += new EventHandler(GridDoubleClick);

			Controls.Add(_grid);

			_phase = null;
			_group = null;
			_competition = null;
			_heat = null;

			_showGroups = false;
			_showHeats = true;

			ResetGridGroups();
		}

		#region Grouping

		private bool _showGroups;

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

		private bool _showHeats;
		public bool ShowHeats
		{
			get { return _showHeats; }
			set
			{
				if (_showHeats != value)
				{
					_showHeats = value;
					ResetGridGroups();
					Rebuild();
				}
			}
		}

		private void ResetGridGroups()
		{
			_grid.Groups.Clear();
			_grid.Columns.Add(0, "תחרות", 100);
			_grid.Columns.Add(1, "מתקן", 100);
			_grid.Columns.Add(2, "מועד", 100);

			if (_showHeats)
			{
				_grid.Groups.Add();
				_grid.Groups[1].Columns.Add(0, "מקצה", 100);
				_grid.Groups[1].Columns.Add(1, "מגרש", 100);
				_grid.Groups[1].Columns.Add(2, "מועד", 100);
			}
			if (_showGroups)
			{
				_grid.Groups.Insert(0);
				_grid.Groups[0].Columns.Add(0, "בית", 1);
			}
		}

		#endregion

		#region Selection

		private Sport.Championships.CompetitionPhase _phase;
		public Sport.Championships.CompetitionPhase Phase
		{
			get { return _phase; }
			set
			{
				if (_phase != value)
				{
					_phase = value;
					_group = null;
					_competition = null;
					_heat = null;
					Rebuild();
				}
			}
		}

		private Sport.Championships.CompetitionGroup _group;
		public Sport.Championships.CompetitionGroup Group
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

		private Sport.Championships.Competition _competition;
		public Sport.Championships.Competition Competition
		{
			get { return _competition; }
			set 
			{ 
				if (_competition != value)
					SelectRow(value); 
			}
		}

		private Sport.Championships.Heat _heat;
		public Sport.Championships.Heat Heat
		{
			get { return _heat; }
			set 
			{ 
				if (_heat != value)
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
			Sport.Championships.CompetitionGroup group = null;
			Sport.Championships.Competition competition = null;
			Sport.Championships.Heat heat = null;

			if (_grid.Selection.Size == 1)
			{
				int row = _grid.Selection.Rows[0];
				_selectedRow = _rows[row];
				group = _selectedRow as Sport.Championships.CompetitionGroup;
				if (group == null)
				{
					competition = _selectedRow as Sport.Championships.Competition;
					if (competition == null)
					{
						heat = _selectedRow as Sport.Championships.Heat;
						competition = heat.Competition;
					}
					group = competition.Group;
				}
			}

			bool selectionChanged = false;

			if (_group != group && _showGroups)
			{
				_group = group;
				selectionChanged = true;
			}
			if (_competition != competition)
			{
				_competition = competition;
				selectionChanged = true;
			}
			if (_heat != heat)
			{
				_heat = heat;
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

		public void Rebuild()
		{
			System.Collections.ArrayList rows = new System.Collections.ArrayList();

			if (_phase != null)
			{
				if (_showGroups)
				{
					foreach (Sport.Championships.CompetitionGroup group in _phase.Groups)
					{
						rows.Add(group);

						foreach (Sport.Championships.Competition competition in group.Competitions)
						{
							rows.Add(competition);

							if (_showHeats)
							{
								foreach (Sport.Championships.Heat heat in competition.Heats)
								{
									rows.Add(heat);
								}
							}
						}
					}
				}
				else if (_group != null)
				{
					foreach (Sport.Championships.Competition competition in _group.Competitions)
					{
						rows.Add(competition);

						if (_showHeats)
						{
							foreach (Sport.Championships.Heat heat in competition.Heats)
							{
								rows.Add(heat);
							}
						}
					}
				}
			}

			_rows = rows.ToArray();

			_grid.ExpandedRows.Clear();
			_grid.RefreshSource();

			SelectRow(_selectedRow);
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
			if (_rows[row] is Sport.Championships.CompetitionGroup)
				return 1;

			return 3;
		}

		public int GetGroup(int row)
		{
			int group = 0;
			if (_showGroups)
			{
				if (_rows[row] is Sport.Championships.CompetitionGroup)
					return group;
				group++;
			}
			if (_rows[row] is Sport.Championships.Competition)
				return group;
			return group + 1;
		}

		public string GetText(int row, int field)
		{
			Sport.Championships.CompetitionGroup group = _rows[row] as Sport.Championships.CompetitionGroup;
			if (group != null)
				return group.Name;

			Sport.Championships.Competition competition = _rows[row] as Sport.Championships.Competition;
			if (competition != null)
			{
				switch (field)
				{
					case (0): // name
						return competition.SportField.Name;
					case (1): // place
						if (competition.Court != null)
						{
							return competition.Facility.Name + " - " + competition.Court.Name;
						}
						if (competition.Facility != null)
						{
							return competition.Facility.Name;
						}
						break;
					case (2): // time
						if (Sport.Common.Tools.IsMinDate(competition.Time))
							return null;
						return competition.Time.ToString("g");
				}

				return null;
			}

			Sport.Championships.Heat heat = (Sport.Championships.Heat) _rows[row];

			switch (field)
			{
				case (0): // name
					return "מקצה " + (heat.Index + 1).ToString();
				case (1): // place
					if (heat.Court != null)
					{
						return heat.Facility.Name + " - " + heat.Court.Name;
					}
					if (heat.Facility != null)
					{
						return heat.Facility.Name;
					}

					break;
				case (2): // time
					if (Sport.Common.Tools.IsMinDate(heat.Time))
						return null;
					return heat.Time.ToString("g");
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

		#endregion

		protected override void Dispose(bool disposing)
		{
			_rows = null;
			base.Dispose (disposing);
		}

		private void GridMouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
		}

		private void GridDoubleClick(object sender, EventArgs e)
		{
			if (!Sport.Core.Session.Connected)
				return;
			
			if (_competition != null && !_phase.Championship.Editing)
			{
				//show competition form:
				CompetitionForm cf = new CompetitionForm(_competition, _heat);
				if (cf.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					_grid.Refresh();
				}
			}
		}

		public void Print()
		{
		}
	}
}
