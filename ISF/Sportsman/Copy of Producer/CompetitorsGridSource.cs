using System;

namespace Sportsman.Producer
{
	public class CompetitorsGridSource : Sport.UI.Controls.IGridSource
	{
		public enum Fields
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

		private Sport.UI.Controls.Grid			_grid;
		private Sport.Championships.Competition	_competition;
		private Sport.Championships.Heat		_heat;

		private Sport.Championships.Competitor[] _competitors;

		private Sport.UI.Controls.Style		_errorStyle;
		private Sport.UI.Controls.Style		_warnStyle;
		private int							maxCompetitionCount = Int32.MaxValue;
		private int							minCompetitionCount = 0;

		public void Rebuild()
		{
			if (_competition == null)
			{
				_competitors = new Sport.Championships.Competitor[0];
			}
			else
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

				System.Collections.ArrayList al = new System.Collections.ArrayList();

				foreach (Sport.Championships.Competitor competitor in _competition.Competitors)
				{
					if (_heat == null || competitor.Heat == _heat.Index)
						al.Add(competitor);
				}

				_competitors = (Sport.Championships.Competitor[]) al.ToArray(typeof(Sport.Championships.Competitor));
			}

			_grid.RefreshSource();

			ResetSelection();
		}


		public Sport.Championships.Competition Competition
		{
			get { return _competition; }
			set
			{
				_competition = value;
				_heat = null;
				Rebuild();
			}
		}

		public Sport.Championships.Heat Heat
		{
			get { return _heat; }
			set
			{
				_heat = value;
				if (_heat != null)
					_competition = _heat.Competition;
				Rebuild();
			}
		}


		public CompetitorsGridSource()
		{
			_competition = null;
			_heat = null;
			_competitors = new Sport.Championships.Competitor[0];

			_errorStyle = new Sport.UI.Controls.Style();
			_errorStyle.Foreground = new System.Drawing.SolidBrush(System.Drawing.Color.Red);

			_warnStyle = new Sport.UI.Controls.Style();
			_warnStyle.Foreground = new System.Drawing.SolidBrush(System.Drawing.Color.Orange);
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
			return _competitors.Length;
		}

		public int GetFieldCount(int row)
		{
			return this._grid.Columns.Count;
			//return 4;
		}

		public int GetGroup(int row)
		{
			return 0;
		}

		public string GetText(int row, int field)
		{
			if (_competitors.Length > row)
			{
				return GetCompetitorFieldString(_competitors[row], field);
			}

			return null;
		}
		
		public Sport.UI.Controls.Style GetStyle(int row, int field, Sport.UI.Controls.GridDrawState state)
		{
			Sport.Championships.CompetitionPlayer player = _competitors[row].Player;
			if (player != null)
			{
				if (player.CompetitionCount > maxCompetitionCount)
					return _errorStyle;
				if (player.CompetitionCount < minCompetitionCount)
					return _warnStyle;
			}
			
			return null;
		}

		public string GetTip(int row)
		{
			return null;
		}

		private static string GetCompetitorFieldString(Sport.Championships.Competitor competitor,
			int field)
		{
			switch ((Fields) field)
			{
				case (Fields.Heat):
					return competitor.Heat == -1 ? null : "מקצה "+(competitor.Heat+1).ToString();
				case (Fields.Position):
					return (competitor.Position + 1).ToString();
				case (Fields.Number):
					return competitor.PlayerNumber.ToString();
				case (Fields.Name):
					string result=null;
					if ((competitor.Player != null)&&(competitor.Player.PlayerEntity != null))
					{
						result = competitor.Player.PlayerEntity.Name;
					}
					return result;
				case (Fields.Team):
					return competitor.GroupTeam.Name;
				case (Fields.ResultPosition):
					return competitor.ResultPosition == -1 ? null :
						(competitor.ResultPosition + 1).ToString();
				case (Fields.Result):
					return competitor.Competition.ResultType == null ? null :
						competitor.Competition.ResultType.FormatResult(competitor.Result);
				case (Fields.Score):
					return competitor.Score == -1 ? null :
						competitor.Score.ToString();
			}

			return null;
		}

		private class CompetitorComparer : Sport.Common.IFieldComparer
		{
			#region IFieldComparer Members

			public int Compare(int field, object x, object y)
			{
				Sport.Championships.Competitor cx = (Sport.Championships.Competitor) x;
				Sport.Championships.Competitor cy = (Sport.Championships.Competitor) y;

				switch ((Fields) field)
				{
					case (Fields.Heat):
						return ((Int32) cx.Heat).CompareTo(cy.Heat);
					case (Fields.Position):
						return ((Int32) cx.Position).CompareTo(cy.Position);
					case (Fields.Number):
						return ((Int32) cx.PlayerNumber).CompareTo(cy.PlayerNumber);
					case (Fields.ResultPosition):
						return ((Int32) cx.ResultPosition).CompareTo(cy.ResultPosition);
					case (Fields.Result):
						return ((Int32) cx.Result).CompareTo(cy.Result);
					case (Fields.Score):
						return ((Int32) cx.Score).CompareTo(cy.Score);
				}

				string sx = cx == null ? null : GetCompetitorFieldString(cx, field);
				string sy = cy == null ? null : GetCompetitorFieldString(cy, field);

				if (sx == null)
				{
					if (sy == null)
						return 0;

					return -1;
				}
				else if (sy == null)
				{
					return 1;
				}

				return sx.CompareTo(sy);
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

				Sport.Common.ArraySort.Sort(_competitors, _sort, new CompetitorComparer());
			}
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
			_competitors = null;
		}

		private Sport.Championships.Competitor _competitor;
		public Sport.Championships.Competitor SelectedCompetitor
		{
			get { return _competitor; }
		}
		
		public event System.EventHandler SelectionChanged;

		private void ResetSelection()
		{
			Sport.Championships.Competitor competitor = null;

			if (_grid.Selection.Size == 1)
			{
				int row = _grid.Selection.Rows[0];
				if (row < _competitors.Length)
					competitor = _competitors[row];
			}

			if (competitor != _competitor)
			{
				_competitor = competitor;
				if (SelectionChanged != null)
					SelectionChanged(this, EventArgs.Empty);
			}
		}

		private void GridSelectionChanged(object sender, EventArgs e)
		{
			ResetSelection();
		}
	}
}
