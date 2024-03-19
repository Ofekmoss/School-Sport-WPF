using System;
using System.Linq;
using System.Collections;
using Sport.UI.Dialogs;
using Sport.UI.Controls;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Sportsman.Producer
{
	public class CompetitorsGridPanel : System.Windows.Forms.Panel,
		Sport.UI.Controls.IGridSource
	{
		private Sport.UI.Controls.Grid _grid;
		public delegate void DeleteCompetitorsEventHandler(object sender, Sport.Championships.Competitor[] competitors);
		public event DeleteCompetitorsEventHandler CompetitorsDeleted;

		public bool GotDisqualifications()
		{
			if (_rows != null && _rows.Length > 0)
			{
				List<Sport.Championships.Competitor> competitors = _rows.OfType<Sport.Championships.Competitor>().ToList();
				if (competitors.Count > 0)
					return competitors.Exists(c => c.TotalDisqualifications > 0);
			}
			return false;
		}

		public bool GotWind()
		{
			if (_competition != null)
			{
				Sport.Rulesets.Rules.GeneralSportTypeData generalData = _competition.GeneralData;
				return generalData == null ? false : generalData.Wind;
			}
			return false;
		}

		public CompetitorsGridPanel()
		{
			_rows = new object[0];

			_grid = new Sport.UI.Controls.Grid();

			_grid.Dock = System.Windows.Forms.DockStyle.Fill;
			_grid.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended; //System.Windows.Forms.SelectionMode.One;
			_grid.Source = this;
			_grid.SelectionChanged += new EventHandler(GridSelectionChanged);
			_grid.MouseUp += new System.Windows.Forms.MouseEventHandler(GridMouseUp);
			_grid.KeyDown += new System.Windows.Forms.KeyEventHandler(Grid_KeyDown);
			_grid.DoubleClick += new EventHandler(GridDoubleClick);

			Controls.Add(_grid);

			_columns = new CompetitorField[10] {
												  CompetitorField.Heat,
												  CompetitorField.Position,
												  CompetitorField.Number,
												  CompetitorField.Name,
												  CompetitorField.Team,
												  CompetitorField.ResultPosition,
												  CompetitorField.Result,
												  CompetitorField.Score,
												  CompetitorField.Disqualifications,
												  CompetitorField.Wind
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
			Score,
			Disqualifications,
			Wind
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
				_columns[n] = (CompetitorField)_grid.Columns[n].Field;
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
				new Sport.UI.Controls.Grid.GridColumn(7, "ניקוד", 30), 
				new Sport.UI.Controls.Grid.GridColumn(8, "פסילות", 30), 
				new Sport.UI.Controls.Grid.GridColumn(9, "רוח", 30)
			};

		private void ResetGridColumns()
		{
			_grid.Columns.Clear();
			bool gotDisqualifications = GotDisqualifications();
			bool gotWind = GotWind();
			_columns.ToList().FindAll(field =>
			{
				if (field == CompetitorField.Disqualifications)
					return gotDisqualifications;
				if (field == CompetitorField.Wind)
					return gotWind;
				return true;
			}).ForEach(field => _grid.Columns.Add(FieldColumns[(int)field]));
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
						return;
					}
				}
			}

			_grid.SelectRow(-1);
		}

		public event System.EventHandler SelectionChanged;

		private void ResetSelection()
		{
			if ((_grid.Selection == null) || (_grid.Selection.Size == 0))
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

		private Hashtable _tblCompetitorDuplicateResults = new Hashtable();

		public ArrayList GetPlayersWithSameResult(Sport.Championships.Competitor competitor)
		{
			if (competitor != null)
			{
				int result = competitor.Result;
				if (_tblCompetitorDuplicateResults[result] != null)
					return (ArrayList)_tblCompetitorDuplicateResults[result];
			}
			return null;
		}

		public void Rebuild()
		{
			_rebuilding = true;

			_tblCompetitorDuplicateResults.Clear();

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
				{
					int compResult = competitor.Result;
					if (compResult > 0)
					{
						//store result in hash table.
						if (_tblCompetitorDuplicateResults[compResult] == null)
							_tblCompetitorDuplicateResults[compResult] = new ArrayList();
						(_tblCompetitorDuplicateResults[compResult] as ArrayList).Add(competitor.PlayerNumber);
					}

					if (_heat == null || competitor.Heat == _heat.Index)
					{
						rows.Add(competitor);
					}
				}

				ArrayList arrToRemove = new ArrayList();
				foreach (int key in _tblCompetitorDuplicateResults.Keys)
					if ((_tblCompetitorDuplicateResults[key] as ArrayList).Count <= 1)
						arrToRemove.Add(key);
				foreach (object key in arrToRemove)
					_tblCompetitorDuplicateResults.Remove(key);

				Sport.Entities.OfflinePlayer[] arrOfflinePlayers =
					_competition.GetOfflinePlayers();
				rows.AddRange(arrOfflinePlayers);
			}

			_rows = rows.ToArray();

			if (_sort != null)
				Sport.Common.ArraySort.Sort(_rows, _sort, new CompetitorComparer());

			if (_rows.Length > 0)
				_grid.ExpandedRows.Add(0, _rows.Length - 1);
			_grid.RefreshSource();

			SelectRow(_selectedRow);

			ResetGridColumns();
			_rebuilding = false;
		}
		#endregion

		private int maxCompetitionCount = Int32.MaxValue;
		private int minCompetitionCount = 0;

		private static Sport.UI.Controls.Style errorStyle;
		private static Sport.UI.Controls.Style warnStyle;
		private static Sport.UI.Controls.Style duplicateScoreStyle;
		static CompetitorsGridPanel()
		{
			errorStyle = new Sport.UI.Controls.Style();
			errorStyle.Foreground = new System.Drawing.SolidBrush(System.Drawing.Color.Red);

			warnStyle = new Sport.UI.Controls.Style();
			warnStyle.Foreground = new System.Drawing.SolidBrush(System.Drawing.Color.Orange);

			duplicateScoreStyle = new Sport.UI.Controls.Style();
			duplicateScoreStyle.Background = new System.Drawing.SolidBrush(System.Drawing.Color.Red);

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
			int fieldCount = 8;
			if (GotDisqualifications())
				fieldCount++;
			if (GotWind())
				fieldCount++;
			return fieldCount;
		}

		public int GetGroup(int row)
		{
			return 0;
		}

		public string GetText(int row, int field)
		{
			if (_rows.Length > row)
				return GetCompetitorFieldString(_rows[row], field, _competition, GotDisqualifications(), GotWind());
			return null;
		}

		public Sport.UI.Controls.Style GetStyle(int row, int field, Sport.UI.Controls.GridDrawState state)
		{
			if (_rows[row] is Sport.Championships.Competitor)
			{
				Sport.Championships.Competitor competitor = (Sport.Championships.Competitor)_rows[row];
				Sport.Championships.CompetitionPlayer player = competitor.Player;
				if (player != null)
				{
					if (_tblCompetitorDuplicateResults[competitor.Result] != null)
						return duplicateScoreStyle;
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

		private static string GetCompetitorFieldString(object oData, int field,	Sport.Championships.Competition competition, 
			bool gotDisqualifications, bool gotWind)
		{
			Sport.Championships.Competitor competitor = null;
			Sport.Entities.OfflinePlayer player = null;
			if (oData is Sport.Championships.Competitor)
				competitor = (Sport.Championships.Competitor)oData;
			else if (oData is Sport.Entities.OfflinePlayer)
				player = (Sport.Entities.OfflinePlayer)oData;
			switch ((CompetitorField)field)
			{
				case (CompetitorField.Heat):
					if (competitor != null)
						return competitor.Heat == -1 ? null : "מקצה " + (competitor.Heat + 1).ToString();
					return null;
				case (CompetitorField.Position):
					if (competitor != null)
						return (competitor.Position + 1).ToString();
					return null;
				case (CompetitorField.Number):
					if (competitor != null)
					{
						if (competitor.SharedResultNumbers != null && competitor.SharedResultNumbers.Length > 1)
							return string.Join(", ", competitor.SharedResultNumbers);
						return competitor.PlayerNumber.ToString();
					}
					else if (player != null)
					{
						return player.ShirtNumber.ToString();
					}
					return null;
				case (CompetitorField.Name):
					if (competitor != null)
					{
						if (competitor.SharedResultNumbers != null && competitor.SharedResultNumbers.Length > 1)
						{
							string competitorName = "[תוצאה משותפת]";
							string names = competitor.GetSharedResultNames();
							if (names.Length > 0)
								competitorName += " " + names;
							return competitorName;
						}
						else if ((competitor.Player != null) && (competitor.Player.PlayerEntity != null))
						{
							return competitor.Player.PlayerEntity.Name;
						}
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
						return (player.Rank < 0) ? null : (player.Rank + 1).ToString();
					return null;
				case (CompetitorField.Result):
					if (competitor != null)
					{
						if ((competitor.Competition != null) && (competitor.Competition.ResultType != null))
							return competitor.Competition.ResultType.FormatResult(competitor.Result);
					}
					else if ((player != null) && (player.Result >= 0))
					{
						if ((competition != null) && (competition.ResultType != null))
							return competition.ResultType.FormatResult(player.Result);
					}

					return null;
				case (CompetitorField.Score):
					if (competitor != null)
						return competitor.Score == -1 ? null : competitor.Score.ToString();
					else if (player != null)
						return (player.Score < 0) ? null : player.Score.ToString();
					return null;
				case (CompetitorField.Disqualifications):
					if (gotDisqualifications && competitor != null && competitor.TotalDisqualifications > 0)
						return string.Format("{0} ({1})", competitor.LastResultDisqualifications, competitor.TotalDisqualifications);
					return null;
				case (CompetitorField.Wind):
					if (gotWind && competitor != null)
						return competitor.Wind;
					return null;
			}

			return null;
		}

		private static string GetCompetitorFieldString(object oData, int field)
		{
			return GetCompetitorFieldString(oData, field, null, false, false);
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
				switch ((CompetitorField)field)
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

				if ((s1 == null) && (s2 == null))
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
				_sort = (int[])columns.Clone();

				Sport.Common.ArraySort.Sort(_rows, _sort, new CompetitorComparer());
			}
		}

		private Sport.UI.Controls.GenericItem gridEditGenericItem = null;
		public System.Windows.Forms.Control Edit(int row, int field)
		{
			if (field != ((int)CompetitorField.ResultPosition))
				return null;

			if (_rows == null)
				return null;

			if ((row < 0) || (row >= _rows.Length))
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
				(position + 1), null);

			gridEditGenericItem.Tag = data;
			gridEditGenericItem.Nullable = false;
			gridEditGenericItem.ValueChanged += new EventHandler(GridEditItemValueChanged);

			return gridEditGenericItem.Control;
		}

		private void GridEditItemValueChanged(object sender, EventArgs e)
		{
		}

		private bool _rebuilding = false;
		public void EditEnded(System.Windows.Forms.Control control)
		{
			if (_rebuilding)
				return;

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

			if ((originalRank < 0) && (originalCustomPosition < 0))
				return;

			int newCustomPosition =
				Sport.Common.Tools.CIntDef(gridEditGenericItem.Value, 0) - 1;

			if (newCustomPosition == originalRank)
				return;

			if (newCustomPosition == originalCustomPosition)
				return;

			bool blnChange = ((competitor != null) || (oPlayer != null));
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
			base.Dispose(disposing);
		}

		private void GridMouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (e.Button == System.Windows.Forms.MouseButtons.Right)
			{
				if (_selectedRow is Sport.Championships.Competitor)
				{
					System.Windows.Forms.ContextMenu menu = new System.Windows.Forms.ContextMenu(
						new System.Windows.Forms.MenuItem[] {
							new System.Windows.Forms.MenuItem("שינוי מספר שחקן", new EventHandler(ChangePlayerNumber))
															});
					menu.Show(this, new System.Drawing.Point(e.X, e.Y));
				}
			}
		}

		private void GridDoubleClick(object sender, EventArgs e)
		{
			int selectedIndex = _grid.SelectedIndex;
			if (selectedIndex >= 0)
			{
				Sport.Championships.Competitor competitor = (Sport.Championships.Competitor)_rows[selectedIndex];
				if (competitor != null)
				{
					int result = competitor.Result;
					if (result > 0 && _tblCompetitorDuplicateResults[result] != null)
					{
						ArrayList arrSameResults = (ArrayList)_tblCompetitorDuplicateResults[result];
						if (arrSameResults.Count > 1)
						{
							int index = arrSameResults.IndexOf(competitor.PlayerNumber);
							if (index >= 0)
							{
								int otherIndex = ((index + 1) % arrSameResults.Count);
								int otherPlayerNumber = (int)arrSameResults[otherIndex];
								object rowToSelect = null;
								for (int i = 0; i < _rows.Length; i++)
								{
									object row = _rows[i];
									if (row is Sport.Championships.Competitor)
									{
										if ((row as Sport.Championships.Competitor).PlayerNumber == otherPlayerNumber)
										{
											rowToSelect = row;
											break;
										}
									}
								}
								if (rowToSelect != null)
									this.SelectRow(rowToSelect);
							}
						}
					}
				}
			}
		}

		private void ChangePlayerNumber(object sender, EventArgs e)
		{
			Sport.Championships.Competitor competitor = (Sport.Championships.Competitor)_selectedRow;
			if (competitor != null)
			{
				GenericEditDialog dialog = new GenericEditDialog("הכנס מספר חזה חדש");
				GenericItem GenericNumericUpDown = new GenericItem("מספר", GenericItemType.Number, null, new object[] { 0, 10000 });
				dialog.Items.Add(GenericNumericUpDown);
				dialog.Confirmable = true;
				dialog.Items[0].Nullable = false;
				int newPlayerNumber = 0;
				if (dialog.ShowDialog() == DialogResult.OK)
				{
					if (dialog.Items[0].Value != null)
					{
						newPlayerNumber = Convert.ToInt32(dialog.Items[0].Value);
					}
				}
				dialog.Dispose();

				if (newPlayerNumber > 0)
				{
					bool blnExists = false;
					for (int i = 0; i < _rows.Length; i++)
					{
						if (_rows[i] is Sport.Championships.Competitor)
						{
							if ((_rows[i] as Sport.Championships.Competitor).PlayerNumber == newPlayerNumber)
							{
								blnExists = true;
								break;
							}
						}
					}

					if (blnExists)
					{
						Sport.UI.MessageBox.Error("שחקן בעל מספר זה כבר משתתף בתחרות.\nכדי להחליף תוצאות אנא השתמש במסך תוצאות.", "שינוי מספר חזה");
						return;
					}

					string newPlayerTeam = string.Empty;
					string newPlayerName = string.Empty;
					Sport.Championships.CompetitionPlayer player = _competition.Group.Players[newPlayerNumber];
					if (player != null && player.CompetitionTeam != null && player.PlayerEntity != null)
					{
						newPlayerTeam = player.CompetitionTeam.Name;
						newPlayerName = player.PlayerEntity.Name;
					}
					if (newPlayerTeam.Length == 0 || newPlayerName.Length == 0)
					{
						Sport.UI.MessageBox.Error("מספר לא תואם לשחקן קיים", "שינוי מספר חזה");
						return;
					}

					Hashtable tblCompetitorResults = new Hashtable();
					for (int i = 0; i < _rows.Length; i++)
					{
						if (_rows[i] is Sport.Championships.Competitor)
						{
							Sport.Championships.Competitor curCompetitor = (Sport.Championships.Competitor)_rows[i];
							int curPlayerNumber = (curCompetitor.Index == competitor.Index) ? newPlayerNumber : curCompetitor.PlayerNumber;
							tblCompetitorResults[curPlayerNumber] = curCompetitor.Result;
						}
					}


					Sport.Championships.CompetitorResult[] competitorsResults = new Sport.Championships.CompetitorResult[tblCompetitorResults.Count];
					int n = 0;
					foreach (int curPlayerNumber in tblCompetitorResults.Keys)
					{
						competitorsResults[n] = new Sport.Championships.CompetitorResult(
							curPlayerNumber, (int)tblCompetitorResults[curPlayerNumber]);
						n++;
					}

					this.Competition.Competitors.RemoveAt(competitor.Index);
					this.Competition.Group.Phase.Championship.Save();
					_competition.SetResults(_heat == null ? -1 : _heat.Index, competitorsResults);

					this.Rebuild();
				}
			}
		}

		public void Print()
		{
		}

		private void Grid_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyCode == System.Windows.Forms.Keys.Delete)
			{
				if (CompetitorsDeleted != null && _grid.Selection.Size > 0)
				{
					ArrayList arrCompetitors = new ArrayList();
					foreach (int rowIndex in _grid.Selection.Rows)
					{
						if (_rows[rowIndex] is Sport.Championships.Competitor)
						{
							arrCompetitors.Add((Sport.Championships.Competitor)_rows[rowIndex]);
						}
					}
					if (arrCompetitors.Count < _grid.Selection.Size)
						Sport.UI.MessageBox.Warn("לא ניתן למחוק מתמודדים שנוספו למאגר לא מקוון דרך מסך זה", "מחיקת מתמודדים");

					if (arrCompetitors.Count > 0)
						CompetitorsDeleted(this, (Sport.Championships.Competitor[])arrCompetitors.ToArray(typeof(Sport.Championships.Competitor)));
				}
			}
		}

		public string BuildSameResultErrorMessage(ArrayList arrAllNumbers, int selectedPlayerNumber)
		{
			int count = arrAllNumbers.Count;
			string strErrorMessage = "תוצאה זהה קיימת! מספר";
			if (count > 2)
				strErrorMessage += "י";
			strErrorMessage += " חזה: ";
			string[] arrPlayerNumbers = new string[count - 1];
			int index = 0;
			for (int i = 0; i < count; i++)
			{
				int curPlayerNumber = (int)arrAllNumbers[i];
				if (curPlayerNumber != selectedPlayerNumber)
				{
					arrPlayerNumbers[index] = curPlayerNumber.ToString();
					index++;
				}
			}
			strErrorMessage += string.Join(", ", arrPlayerNumbers);
			return strErrorMessage;
		}
	}
}
