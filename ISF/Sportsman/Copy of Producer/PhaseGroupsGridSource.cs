using System;

namespace Sportsman.Producer
{
	public class PhaseGroupsGridSource : Sport.UI.Controls.IGridSource
	{
		// The groupStyle will be given to rows presenting the group name
		private static Sport.UI.Controls.Style groupStyle;
		private static Sport.UI.Controls.Style selectedGroupStyle;
		private static Sport.UI.Controls.Style unconfirmedTeam;
		static PhaseGroupsGridSource()
		{
			groupStyle = new Sport.UI.Controls.Style();
			groupStyle.Background = new System.Drawing.SolidBrush(
				System.Drawing.Color.FromArgb(50, 150, 196));
			groupStyle.Foreground = System.Drawing.Brushes.White;
			selectedGroupStyle = new Sport.UI.Controls.Style();
			selectedGroupStyle.Background = new System.Drawing.SolidBrush(
				System.Drawing.Color.FromArgb(25, 120, 146));
			selectedGroupStyle.Foreground = System.Drawing.Brushes.White;
			unconfirmedTeam = new Sport.UI.Controls.Style();
			unconfirmedTeam.Foreground = System.Drawing.Brushes.Red;
		}

		private Sport.UI.Controls.Grid		_grid;
		private Sport.Championships.Phase	_phase;

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

				for (int g = 0; g < _phase.Groups.Count; g++)
				{
					Sport.Championships.Group group = _phase.Groups[g];
					_rows[i] = group;
					i++;

					for (int t = 0; t < group.Teams.Count; t++)
					{
						_rows[i] = group.Teams[t];
						i++;
					}
				}
			}

			_grid.RefreshSource();
		}

		public Sport.Championships.Phase Phase
		{
			get { return _phase; }
			set 
			{ 
				_phase = value;
				Rebuild();
			}
		}

		public PhaseGroupsGridSource()
		{
			_phase = null;
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
			return 1;
		}

		public int GetGroup(int row)
		{
			return 0;
		}

		public string GetText(int row, int field)
		{
			Sport.Championships.Group group = _rows[row] as Sport.Championships.Group;
			if (group != null)
			{
				return group.Name;
			}

			Sport.Championships.Team team = (Sport.Championships.Team) _rows[row];

			return (team.Index + 1).ToString() + " " + team.ToString();
		}

		public Sport.UI.Controls.Style GetStyle(int row, int field, Sport.UI.Controls.GridDrawState state)
		{
			if (_rows[row] is Sport.Championships.Group)
				return (state & Sport.UI.Controls.GridDrawState.Selected) == 0 ?
				groupStyle : selectedGroupStyle;

			if (!((Sport.Championships.Team) _rows[row]).IsConfirmed())
				return unconfirmedTeam;

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
