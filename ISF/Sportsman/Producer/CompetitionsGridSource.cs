using System;

namespace Sportsman.Producer
{
	public class CompetitionsGridSource : Sport.UI.Controls.IGridSource
	{
		public enum Fields
		{
			Name,
			Place,
			Time
		}

		private Sport.UI.Controls.Grid					_grid;
		private Sport.Championships.CompetitionGroup	_group;

		private object[] _rows;

		public void Rebuild()
		{
			if (_group == null)
			{
				_rows = new object[0];
			}
			else
			{
				int count = 0;
				foreach (Sport.Championships.Competition competition in _group.Competitions)
					count += competition.Heats.Count + 1;

				_rows = new object[count];
				int i = 0;

				foreach (Sport.Championships.Competition competition in _group.Competitions)
				{
					_rows[i] = competition;
					i++;
					foreach (Sport.Championships.Heat heat in competition.Heats)
					{
						_rows[i] = heat;
						i++;
					}
				}
			}

			_grid.ExpandedRows.Clear();
			_grid.RefreshSource();
		}


		public Sport.Championships.CompetitionGroup Group
		{
			get { return _group; }
			set
			{
				if (_group != value)
				{
					_group = value;
					Rebuild();
				}
			}
		}


		public CompetitionsGridSource()
		{
			_group = null;
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
			return 3;
		}

		public int GetGroup(int row)
		{
			if (_rows[row] is Sport.Championships.Competition)
				return 0;
			return 1;
		}

		public string GetText(int row, int field)
		{
			Sport.Championships.Competition competition = _rows[row] as Sport.Championships.Competition;
			if (competition != null)
			{
				switch ((Fields) field)
				{
					case (Fields.Name):
						return competition.SportField.Name;
					case (Fields.Place):
						if (competition.Court != null)
						{
							return competition.Facility.Name + " - " + competition.Court.Name;
						}
						if (competition.Facility != null)
						{
							return competition.Facility.Name;
						}
						break;
					case (Fields.Time):
						if (Sport.Common.Tools.IsMinDate(competition.Time))
							return null;
						return competition.Time.ToString("g");
				}

				return null;
			}

			Sport.Championships.Heat heat = (Sport.Championships.Heat) _rows[row];

			switch ((Fields) field)
			{
				case (Fields.Name):
					return "מקצה " + (heat.Index + 1).ToString();
				case (Fields.Place):
					if (heat.Court != null)
					{
						return heat.Facility.Name + " - " + heat.Court.Name;
					}
					if (heat.Facility != null)
					{
						return heat.Facility.Name;
					}

					break;
				case (Fields.Time):
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

		public void Dispose()
		{
			_rows = null;
		}

		private Sport.Championships.Competition _competition;
		public Sport.Championships.Competition SelectedCompetition
		{
			get { return _competition; }
		}
		
		private Sport.Championships.Heat _heat;
		public Sport.Championships.Heat SelectedHeat
		{
			get { return _heat; }
		}

		public event System.EventHandler SelectionChanged;

		private void ResetSelection()
		{
			Sport.Championships.Competition competition = null;
			Sport.Championships.Heat heat = null;

			if (_grid.Selection.Size == 1)
			{
				int row = _grid.Selection.Rows[0];
				competition = _rows[row] as Sport.Championships.Competition;
				if (competition == null)
				{
					heat = (Sport.Championships.Heat) _rows[row];
					competition = heat.Competition;
				}
			}

			bool selectionChanged = false;

			if (competition != _competition)
			{
				_competition = competition;
				selectionChanged = true;
			}
			if (heat != _heat)
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
	}
}
