using System;

namespace Sportsman.Producer
{
	/// <summary>
	/// Summary description for PhaseGroupGridPanel.
	/// </summary>
	[System.ComponentModel.Designer("System.Windows.Forms.Design.PanelDesigner, System.Design")]
	public class PhaseGroupGridPanel : System.Windows.Forms.Panel,
		Sport.UI.Controls.IGridSource
	{
		private Sport.UI.Controls.Grid			_grid;
		private int _minTeamCompetitors=-1;
		private int _maxTeamCompetitors=-1;
		private int _competitionIndex=-1;
		public PhaseGroupGridPanel(int minTeamCompetitors, int maxTeamCompetitors)
		{
			_rows = new object[0];

			_grid = new Sport.UI.Controls.Grid();

			_grid.Dock = System.Windows.Forms.DockStyle.Fill;
			_grid.SelectionMode = System.Windows.Forms.SelectionMode.One;
			_grid.Source = this;
			_grid.SelectionChanged += new EventHandler(GridSelectionChanged);
			_grid.MouseUp += new System.Windows.Forms.MouseEventHandler(GridMouseUp);
			_grid.DoubleClick += new EventHandler(GridDoubleClick);

			_grid.Columns.Add(0, "בתים", 1);

			Controls.Add(_grid);

			_phase = null;
			_group = null;
			_team = null;
			
			_minTeamCompetitors = minTeamCompetitors;
			_maxTeamCompetitors = maxTeamCompetitors;
		}
		
		public PhaseGroupGridPanel()
			: this(-1, -1)
		{
		}
		
		public int MinTeamCompetitors
		{
			get { return _minTeamCompetitors; }
			set { _minTeamCompetitors = value; }
		}
		
		public int MaxTeamCompetitors
		{
			get { return _maxTeamCompetitors; }
			set { _maxTeamCompetitors = value; }
		}
		
		public int CompetitionIndex
		{
			get { return _competitionIndex; }
			set { _competitionIndex = value; }
		}
		
		#region Selection

		private Sport.Championships.Phase _phase;
		public Sport.Championships.Phase Phase
		{
			get { return _phase; }
			set
			{
				if (_phase != value)
				{
					_phase = value;
					_group = null;
					_team = null;
					Rebuild();
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

			if (_grid.Selection.Size == 1)
			{
				int row = _grid.Selection.Rows[0];
				selectedRow = _rows[row];
				_group = selectedRow as Sport.Championships.Group;
				if (_group == null)
				{
					_team = selectedRow as Sport.Championships.Team;
					if (_team == null)
					{
						Sport.Entities.OfflineTeam offlineTeam = 
							selectedRow as Sport.Entities.OfflineTeam;
						if (offlineTeam != null)
						{
							_group = _phase.Groups[offlineTeam.Group];
						}
					}
					else
					{
						_group = _team.Group;
					}
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
					Sport.Entities.OfflineTeam[] arrOfflineTeams = 
						group.GetOfflineTeams();
					rows.AddRange(arrOfflineTeams);
				}
			}
			
			_rows = rows.ToArray();

			if (_rows.Length > 0)
				_grid.ExpandedRows.Add(0, _rows.Length - 1);
			_grid.RefreshSource();
			
			SelectRow(_selectedRow);
		}

		#endregion

		#region IGridSource

		// The groupStyle will be given to rows presenting the group name
		private static Sport.UI.Controls.Style groupStyle;
		private static Sport.UI.Controls.Style selectedGroupStyle;
		private static Sport.UI.Controls.Style unconfirmedTeamStyle;
		private static Sport.UI.Controls.Style invalidShirtRangeStyle;
		private static Sport.UI.Controls.Style tooLittleCompetitorsStyle;
		private static Sport.UI.Controls.Style tooMuchCompetitorsStyle;
		
		static PhaseGroupGridPanel()
		{
			groupStyle = new Sport.UI.Controls.Style();
			groupStyle.Background = new System.Drawing.SolidBrush(
				System.Drawing.Color.FromArgb(50, 150, 196));
			groupStyle.Foreground = System.Drawing.Brushes.White;
			
			selectedGroupStyle = new Sport.UI.Controls.Style();
			selectedGroupStyle.Background = new System.Drawing.SolidBrush(
				System.Drawing.Color.FromArgb(25, 120, 146));
			selectedGroupStyle.Foreground = System.Drawing.Brushes.White;
			
			unconfirmedTeamStyle = new Sport.UI.Controls.Style();
			unconfirmedTeamStyle.Foreground = System.Drawing.Brushes.Red;
			
			invalidShirtRangeStyle = new Sport.UI.Controls.Style();
			invalidShirtRangeStyle.Foreground = System.Drawing.Brushes.Orange;
			
			tooLittleCompetitorsStyle = new Sport.UI.Controls.Style();
			tooLittleCompetitorsStyle.Foreground = System.Drawing.Brushes.Black;
			tooLittleCompetitorsStyle.Background = System.Drawing.Brushes.Orange;
			
			tooMuchCompetitorsStyle = new Sport.UI.Controls.Style();
			tooMuchCompetitorsStyle.Foreground = System.Drawing.Brushes.Black;
			tooMuchCompetitorsStyle.Background = System.Drawing.Brushes.Red;
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
			return 1;
		}

		public int GetGroup(int row)
		{
			return 0;
		}

		public string GetText(int row, int field)
		{
			object data = _rows[row];
			string text = "";
			if (data is Sport.Championships.Group)
			{
				Sport.Championships.Group group = (Sport.Championships.Group) data;
				if (group != null)
					text = group.Name;
			}
			else if (data is Sport.Championships.Team)
			{
				Sport.Championships.Team team = (Sport.Championships.Team) data;
				if (team != null)
					text = (team.Index + 1).ToString() + " " + team.ToString();
			}
			else if (data is Sport.Entities.OfflineTeam)
			{
				Sport.Entities.OfflineTeam offlineTeam = 
					(Sport.Entities.OfflineTeam) data;
				if (offlineTeam != null)
				{
					text = (row+1).ToString() + " " + offlineTeam.ToString();
				}
			}
			
			return text;
		}

		public Sport.UI.Controls.Style GetStyle(int row, int field, Sport.UI.Controls.GridDrawState state)
		{
			if (_rows[row] is Sport.Championships.Group)
			{
				return (state & Sport.UI.Controls.GridDrawState.Selected) == 0 ?
				groupStyle : selectedGroupStyle;
			}
			
			if (_rows[row] is Sport.Championships.Team)
			{
				Sport.Championships.Team team=(Sport.Championships.Team) _rows[row];
				
				if (!team.IsConfirmed())
					return unconfirmedTeamStyle;
			
				if (!team.IsValidShirtRange())
					return invalidShirtRangeStyle;
				
				if (team is Sport.Championships.CompetitionTeam)
				{
					if ((_competitionIndex >= 0)&&(_minTeamCompetitors >= 0)&&
						(_maxTeamCompetitors >= 0))
					{
						int teamCompetitors=
							(this.Group as Sport.Championships.CompetitionGroup).GetTeamCompetitionCompetitors(
							(Sport.Championships.CompetitionTeam) team, _competitionIndex);
						
						if (teamCompetitors < _minTeamCompetitors)
							return tooLittleCompetitorsStyle;
						
						if (teamCompetitors > _maxTeamCompetitors)
							return tooMuchCompetitorsStyle;
					}														   
				}
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
		}
	}
	
	[System.ComponentModel.Designer("System.Windows.Forms.Design.PanelDesigner, System.Design")]
	public class RegisteredTeamsGridPanel : System.Windows.Forms.Panel,
		Sport.UI.Controls.IGridSource
	{
		private enum Column
		{
			TeamName=0,
			SchoolSymbol,
			FromNumber,
			ToNumber
		}
		
		private Sport.UI.Controls.Grid			_grid;

		public RegisteredTeamsGridPanel()
		{
			_rows = new object[0];
			
			_grid = new Sport.UI.Controls.Grid();
			_grid.Dock = System.Windows.Forms.DockStyle.Fill;
			_grid.SelectionMode = System.Windows.Forms.SelectionMode.One;
			_grid.Source = this;
			_grid.Columns.Add((int) Column.TeamName, "שם קבוצה", 220, System.Drawing.StringAlignment.Center);
			_grid.Columns.Add((int) Column.SchoolSymbol, "סמל בית ספר", 120, System.Drawing.StringAlignment.Center);
			_grid.Columns.Add((int) Column.FromNumber, "ממספר חולצה", 90, System.Drawing.StringAlignment.Center);
			_grid.Columns.Add((int) Column.ToNumber, "עד מספר חולצה", 90, System.Drawing.StringAlignment.Center);
			
			Controls.Add(_grid);
			
			_category = null;
		}
		
		public Sport.UI.Controls.Grid Grid
		{
			get { return _grid; }
		}
		
		#region Selection
		private Sport.Entities.ChampionshipCategory _category=null;
		public Sport.Entities.ChampionshipCategory ChampionshipCategory
		{
			get { return _category; }
			set
			{
				if (_category != value)
				{
					_category = value;
					Rebuild();
				}
			}
		}
		#endregion

		#region Rows
		private object[] _rows;
		
		public void Rebuild()
		{
			System.Collections.ArrayList rows = new System.Collections.ArrayList();
			
			if (_category != null)
				rows.AddRange(_category.GetTeams());
			
			_rows = rows.ToArray();
			
			if (_rows.Length > 0)
				_grid.ExpandedRows.Add(0, _rows.Length - 1);
			_grid.RefreshSource();
		}
		#endregion

		#region IGridSource
		// The groupStyle will be given to rows presenting the group name
		private static Sport.UI.Controls.Style unconfirmedTeamStyle;
		private static Sport.UI.Controls.Style invalidShirtRangeStyle;
		static RegisteredTeamsGridPanel()
		{
			unconfirmedTeamStyle = new Sport.UI.Controls.Style();
			unconfirmedTeamStyle.Foreground = System.Drawing.Brushes.Red;
			invalidShirtRangeStyle = new Sport.UI.Controls.Style();
			invalidShirtRangeStyle.Foreground = System.Drawing.Brushes.Orange;
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
			return 3+1;
		}

		public int GetGroup(int row)
		{
			return 0;
		}

		public string GetText(int row, int field)
		{
			Sport.Entities.Team team=(Sport.Entities.Team) _rows[row];
			Column column=(Column) field;
			switch (column)
			{
				case Column.TeamName:
					return team.TeamName();
				case Column.SchoolSymbol:
					return team.School.Symbol;
				case Column.FromNumber:
				case Column.ToNumber:
					int numFrom=team.PlayerNumberFrom;
					int numTo=team.PlayerNumberTo;
					if ((numFrom < 0)||(numTo < 0)||(numFrom >= numTo))
						return "";
					return (column == Column.FromNumber)?numFrom.ToString():numTo.ToString();
			}
			return null;
		}
		
		public Sport.UI.Controls.Style GetStyle(int row, int field, Sport.UI.Controls.GridDrawState state)
		{
			if (((Sport.Entities.Team) _rows[row]).Status != Sport.Types.TeamStatusType.Confirmed)
				return unconfirmedTeamStyle;
			if (!((Sport.Entities.Team) _rows[row]).School.ValidShirtRange())
					return invalidShirtRangeStyle;
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
	}
}
