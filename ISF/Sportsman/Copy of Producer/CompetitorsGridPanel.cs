using System;

namespace Sportsman.Producer
{
	public class CompetitorsGridPanel : System.Windows.Forms.Panel,
		Sport.UI.Controls.IGridSource
	{
		private Sport.UI.Controls.Grid			_grid;

		public CompetitorsGridPanel()
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

			_columns = new CompetitorField[8] {
												  CompetitorField.Heat,
												  CompetitorField.Position,
												  CompetitorField.Number,
												  CompetitorField.Name,
												  CompetitorField.Team,
												  CompetitorField.ResultPosition,
												  CompetitorField.Result,
												  CompetitorField.Score
											  };


			ResetGridColumns();
		}

		#region Columns

		public enum CompetitorField
		{
			Heat,
			Position,
			Number,
			Name,
			Team,
			ResultPosition,
			Result,
			Score
		}

		private CompetitorField[] _columns;
		public CompetitorField[] Columns
		{
			get { return _columns; }
			set
			{
				_columns = value;
				ResetGridColumns();
			}
		}

		private void GridColumnMoved(object sender, Sport.UI.Controls.Grid.ColumnMoveEventArgs e)
		{
			_columns = new CompetitorField[_grid.Columns.Count];
			for (int n = 0; n < _grid.Columns.Count; n++)
			{
				_columns[n] = (CompetitorField) _grid.Columns[n].Field;
			}
		}

		public readonly Sport.UI.Controls.Grid.GridColumn[] FieldColumns = new Sport.UI.Controls.Grid.GridColumn[]
			{
				new Sport.UI.Controls.Grid.GridColumn(0, "מקצה", 30),
				new Sport.UI.Controls.Grid.GridColumn(1, "מיקום", 30),
				new Sport.UI.Controls.Grid.GridColumn(2, "מספר", 30),
				new Sport.UI.Controls.Grid.GridColumn(3, "שם", 100),
				new Sport.UI.Controls.Grid.GridColumn(4, "קבוצה", 100),
				new Sport.UI.Controls.Grid.GridColumn(5, "דירוג", 30),
				new Sport.UI.Controls.Grid.GridColumn(6, "תוצאה", 70),
				new Sport.UI.Controls.Grid.GridColumn(7, "ניקוד", 30)
			};

		private void ResetGridColumns()
		{
			_grid.Columns.Clear();

			foreach (CompetitorField field in _columns)
			{
				_grid.Columns.Add(FieldColumns[(int) field]);
			}

			_grid.Refresh();
		}

		#endregion

		#region Selection

		private Sport.Championships.Competition _competition;
		public Sport.Championships.Competition Competition
		{
			get { return _competition; }
			set
			{
				if (_competition != value)
				{
					_competition = value;
					_heat = null;
					Rebuild();
				}
			}
		}

		private Sport.Championships.Heat _heat;
		public Sport.Championships.Heat Heat
		{
			get { return _heat; }
			set
			{
				if (_heat != value)
				{
					SelectRow(null);
					_heat = value;
					if (_heat != null)
						_competition = _heat.Competition;
					Rebuild();
				}
			}
		}

		private Sport.Championships.Competitor _competitor;
		public Sport.Championships.Competitor Competitor
		{
			get { return _competitor; }
			set 
			{ 
				if (_competitor != value)
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
			if ((_grid.Selection == null)||(_grid.Selection.Size == 0))
			{
				_selectedRow = null;
			}
			else if (_grid.Selection.Size == 1)
			{
				int row = _grid.Selection.Rows[0];
				_selectedRow = _rows[row];
			}

			if (_competitor != _selectedRow)
			{
				_competitor = _selectedRow as Sport.Championships.Competitor;
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
			if (_competition != null)
			{
				Sport.Rulesets.Rules.CompetitorCompetitions cc =
					_competition.GetRule(typeof(Sport.Rulesets.Rules.CompetitorCompetitions), 
					typeof(Sport.Rulesets.Rules.CompetitorCompetitionsRule)) as
					Sport.Rulesets.Rules.CompetitorCompetitions;

				if (cc == null)
				{
					maxCompetitionCount = Int32.MaxValue;
					minCompetitionCount = 0;
				}
				else
				{
					maxCompetitionCount = cc.Maximum;
					minCompetitionCount = cc.Minimum;
				}

				foreach (Sport.Championships.Competitor competitor in _competition.Competitors)
					if (_heat == null || competitor.Heat == _heat.Index)
						rows.Add(competitor);
				
				Sport.Entities.OfflinePlayer[] arrOfflinePlayers = 
					_competition.GetOfflinePlayers();
				rows.AddRange(arrOfflinePlayers);
			}

			_rows = rows.ToArray();

			if (_rows.Length > 0)
				_grid.ExpandedRows.Add(0, _rows.Length - 1);
			_grid.RefreshSource();

			SelectRow(_selectedRow);
		}

		#endregion

		private int							maxCompetitionCount = Int32.MaxValue;
		private int							minCompetitionCount = 0;

		private static Sport.UI.Controls.Style errorStyle;
		private static Sport.UI.Controls.Style warnStyle;
		static CompetitorsGridPanel()
		{
			errorStyle = new Sport.UI.Controls.Style();
			errorStyle.Foreground = new System.Drawing.SolidBrush(System.Drawing.Color.Red);

			warnStyle = new Sport.UI.Controls.Style();
			warnStyle.Foreground = new System.Drawing.SolidBrush(System.Drawing.Color.Orange);

		}

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
			return 8;
		}

		public int GetGroup(int row)
		{
			return 0;
		}

		public string GetText(int row, int field)
		{
			if (_rows.Length > row)
				return GetCompetitorFieldString(_rows[row], field, _competition);
			return null;
		}

		public Sport.UI.Controls.Style GetStyle(int row, int field, Sport.UI.Controls.GridDrawState state)
		{
			if (_rows[row] is Sport.Championships.Competitor)
			{
				Sport.Championships.CompetitionPlayer player = ((Sport.Championships.Competitor) _rows[row]).Player;
				if (player != null)
				{
					if (player.CompetitionCount > maxCompetitionCount)
						return errorStyle;
					if (player.CompetitionCount < minCompetitionCount)
						return warnStyle;
				}
			}

			return null;
		}

		public string GetTip(int row)
		{
			return null;
		}

		private static string GetCompetitorFieldString(object oData, int field, 
			Sport.Championships.Competition competition)
		{
			Sport.Championships.Competitor competitor = null;
			Sport.Entities.OfflinePlayer player = null;
			if (oData is Sport.Championships.Competitor)
				competitor = (Sport.Championships.Competitor) oData;
			else if (oData is Sport.Entities.OfflinePlayer)
				player = (Sport.Entities.OfflinePlayer) oData;
			switch ((CompetitorField) field)
			{
				case (CompetitorField.Heat):
					if (competitor != null)
						return competitor.Heat == -1 ? null : "מקצה "+(competitor.Heat+1).ToString();
					return null;
				case (CompetitorField.Position):
					if (competitor != null)
						return (competitor.Position + 1).ToString();
					return null;
				case (CompetitorField.Number):
					if (competitor != null)
						return competitor.PlayerNumber.ToString();
					else if (player != null)
						return player.ShirtNumber.ToString();
					return null;
				case (CompetitorField.Name):
					if (competitor != null)
					{
						if ((competitor.Player != null)&&(competitor.Player.PlayerEntity != null))
							return competitor.Player.PlayerEntity.Name;
					}
					else if (player != null)
						return player.ToString();
					return null;
				case (CompetitorField.Team):
					if (competitor != null)
					{
						if (competitor.GroupTeam != null)
							return competitor.GroupTeam.Name;
					}
					else if (player != null)
					{
						if (player.Team != null)
							return player.Team.TeamName();
						else if (player.OfflineTeam != null)
							return player.OfflineTeam.ToString();
					}
					return null;
				case (CompetitorField.ResultPosition):
					if (competitor != null)
					{
						return competitor.ResultPosition == -1 ? null :
							(competitor.ResultPosition + 1).ToString();
					}
					else if (player != null)
						return (player.Rank < 0)?null:(player.Rank+1).ToString();
					return null;
				case (CompetitorField.Result):
					if (competitor != null)
					{
						if ((competitor.Competition != null)&&(competitor.Competition.ResultType != null))
							return competitor.Competition.ResultType.FormatResult(competitor.Result);
					}
					else if ((player != null)&&(player.Result >= 0))
					{
						if ((competition != null)&&(competition.ResultType != null))
							return competition.ResultType.FormatResult(player.Result);
					}
					
					return null;
				case (CompetitorField.Score):
					if (competitor != null)
						return competitor.Score == -1 ? null : competitor.Score.ToString();
					else if (player != null)
						return (player.Score < 0)?null:player.Score.ToString();
					return null;
			}
			
			return null;
		}
		
		private static string GetCompetitorFieldString(object oData, int field)
		{
			return GetCompetitorFieldString(oData, field, null);
		}
		
		private class CompetitorComparer : Sport.Common.IFieldComparer
		{
			#region IFieldComparer Members

			public int Compare(int field, object x, object y)
			{
				int heat1 = -1;
				int heat2 = -1;
				int position1 = -1;
				int position2 = -1;
				int number1 = -1;
				int number2 = -1;
				int rank1 = -1;
				int rank2 = -1;
				int result1 = -1;
				int result2 = -1;
				int score1 = -1;
				int score2 = -1;
				if (x is Sport.Championships.Competitor)
				{
					Sport.Championships.Competitor c = (x as Sport.Championships.Competitor);
					heat1 = c.Heat;
					position1 = c.Position;
					number1 = c.PlayerNumber;
					rank1 = c.ResultPosition;
					result1 = c.Result;
					score1 = c.Score;
				}
				else if (x is Sport.Entities.OfflinePlayer)
				{
					Sport.Entities.OfflinePlayer p = (x as Sport.Entities.OfflinePlayer);
					number1 = p.ShirtNumber;
					rank1 = p.Rank;
					result1 = p.Result;
					score1 = p.Score;
				}
				if (y is Sport.Championships.Competitor)
				{
					Sport.Championships.Competitor c = (y as Sport.Championships.Competitor);
					heat2 = c.Heat;
					position2 = c.Position;
					number2 = c.PlayerNumber;
					rank2 = c.ResultPosition;
					result2 = c.Result;
					score2 = c.Score;
				}
				else if (y is Sport.Entities.OfflinePlayer)
				{
					Sport.Entities.OfflinePlayer p = (y as Sport.Entities.OfflinePlayer);
					number2 = p.ShirtNumber;
					rank2 = p.Rank;
					result2 = p.Result;
					score2 = p.Score;
				}
				switch ((CompetitorField) field)
				{
					case (CompetitorField.Heat):
						return heat1.CompareTo(heat2);
					case (CompetitorField.Position):
						return position1.CompareTo(position2);
					case (CompetitorField.Number):
						return number1.CompareTo(number2);
					case (CompetitorField.ResultPosition):
						return rank1.CompareTo(rank2);
					case (CompetitorField.Result):
						return result1.CompareTo(result2);
					case (CompetitorField.Score):
						return score1.CompareTo(score2);
				}
				
				string s1 = GetCompetitorFieldString(x, field);
				string s2 = GetCompetitorFieldString(y, field);
				
				if ((s1 == null)&&(s2 == null))
					return 0;
				
				if (s1 == null)
					return -1;
				
				if (s2 == null)
					return 1;
				
				return s1.CompareTo(s2);
			}

			#endregion
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

				Sport.Common.ArraySort.Sort(_rows, _sort, new CompetitorComparer());
			}
		}

		private Sport.UI.Controls.GenericItem gridEditGenericItem=null;
		public System.Windows.Forms.Control Edit(int row, int field)
		{
			if (field != ((int) CompetitorField.ResultPosition))
				return null;
			
			if (_rows == null)
				return null;
			
			if ((row < 0)||(row >= _rows.Length))
				return null;
			
			int position = -1;
			object data = _rows[row];
			if (data is Sport.Championships.Competitor)
			{
				position = (data as Sport.Championships.Competitor).CustomPosition;
				if (position < 0)
					position = (data as Sport.Championships.Competitor).ResultPosition;
				if (position < 0)
					position = (data as Sport.Championships.Competitor).CustomPosition;

			}
			else if (data is Sport.Entities.OfflinePlayer)
			{
				position = (data as Sport.Entities.OfflinePlayer).CustomPosition;
				if (position < 0)
					position = (data as Sport.Entities.OfflinePlayer).Rank;
				if (position < 0)
					position = (data as Sport.Entities.OfflinePlayer).CustomPosition;
			}
			
			/* if (position < 0)
				return null; */
			
			if (gridEditGenericItem != null)
				gridEditGenericItem.ValueChanged -= new EventHandler(GridEditItemValueChanged);
			
			gridEditGenericItem = new Sport.UI.Controls.GenericItem(
				Sport.UI.Controls.GenericItemType.Number, 
				(position+1), null);
			
			gridEditGenericItem.Tag = data;
			gridEditGenericItem.Nullable = false;
			gridEditGenericItem.ValueChanged += new EventHandler(GridEditItemValueChanged);

			return gridEditGenericItem.Control;
		}
		
		private void GridEditItemValueChanged(object sender, EventArgs e)
		{
		}
		
		public void EditEnded(System.Windows.Forms.Control control)
		{
			int originalCustomPosition = -1;
			int originalRank = -1;
			object data = gridEditGenericItem.Tag;
			Sport.Championships.Competitor competitor = null;
			Sport.Entities.OfflinePlayer oPlayer = null;
			if (data is Sport.Championships.Competitor)
			{
				competitor = (data as Sport.Championships.Competitor);
				originalCustomPosition = competitor.CustomPosition;
				originalRank = competitor.ResultPosition;
			}
			else if (data is Sport.Entities.OfflinePlayer)
			{
				oPlayer = (data as Sport.Entities.OfflinePlayer);
				originalCustomPosition = oPlayer.CustomPosition;
				originalRank = oPlayer.Rank;
			}
			
			if ((originalRank < 0)&&(originalCustomPosition < 0))
				return;
			
			int newCustomPosition = 
				Sport.Common.Tools.CIntDef(gridEditGenericItem.Value, 0)-1;
			
			if (newCustomPosition == originalRank)
				return;
			
			if (newCustomPosition == originalCustomPosition)
				return;
			
			bool blnChange = ((competitor != null)||(oPlayer != null));
			if (competitor != null)
			{
				competitor.SetCustomPosition(newCustomPosition);
			}
			else if (oPlayer != null)
			{
				oPlayer.CustomPosition = newCustomPosition;
				oPlayer.Save();
			}
			if (blnChange)
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
		}

		private void GridDoubleClick(object sender, EventArgs e)
		{
			
		}

		public void Print()
		{
		}
	}
}
