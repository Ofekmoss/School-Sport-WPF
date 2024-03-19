using System;
using System.Drawing;
using Sport.Documents;
using Sportsman.Producer;
using Sport.Rulesets.Rules;
using System.Collections;

namespace Sportsman.Documents
{
	public class RankingSectionObject : PageItem, ISectionObject
	{
		private Font		_groupFont;

		private RankingVisualizer visualizer;
		private bool _competitionType = false;

		public RankingSectionObject(bool blnCompetition)
		{
			visualizer = new RankingVisualizer(blnCompetition);
			
			_groupFont = new System.Drawing.Font("Tahoma", 
				16, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			_competitionType = blnCompetition;
			
			//_groupFont = new System.Drawing.Font("Tahoma", 
				//20, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
		}

		private Sport.Championships.Team[] teams;

		private Sport.Championships.Phase _phase;
		public Sport.Championships.Phase Phase
		{
			get { return _phase; }
			set 
			{ 
				if (_phase != value)
				{
					_phase = value; 
					visualizer.RankingTable = null;
					if (_phase != null)
					{
						RankingTables rankingTables=null;
						if (Sport.Core.Session.Connected)
						{
							rankingTables = 
								_phase.Championship.ChampionshipCategory.GetRule(typeof(RankingTables)) 
								as RankingTables;
						}
						else
						{
							rankingTables = Sport.Rulesets.Ruleset.LoadOfflineRule(
								typeof(Sport.Rulesets.Rules.RankingTablesRule), 
								_phase.Championship.CategoryID, -1) as RankingTables;
						}

						if (rankingTables != null)
						{
							RankingTable table = null;
							int ruleTypeId = Sport.Rulesets.RuleType.GetRuleType(typeof(RankingTables)).Id;
							string tableName = _phase.Definitions.Get(ruleTypeId, RankingTablesRule.PhaseTable);
							if (tableName == null)
							{
								table = rankingTables.DefaultRankingTable;
							}
							else
							{
								for (int n = 0; n < rankingTables.Tables.Count && table == null; n++)
								{
									if (rankingTables.Tables[n].Name == tableName)
									{
										table = rankingTables.Tables[n];
									}
								}
							}

							if (table != null)
							{
								visualizer.RankingTable = table;
							}
						}
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
				{
					_group = value;
					Phase = _group.Phase;
				}
			}
		}

		#region ISectionObject Members

		private TextItem lastHeader;

		private bool AddHeader(DocumentBuilder builder, Page page, string text, Font font)
		{
			int height = (int) builder.MeasureString(text, font, Bounds.Width).Height;

			if (_position + height > Bounds.Bottom)
				return false;

			lastHeader = new TextItem(text);
			lastHeader.Font = font;
			lastHeader.Bounds = new Rectangle(Bounds.Left, _position, Bounds.Width, height);
			page.Items.Add(lastHeader);

			_position += height + height / 3;

			return true;
		}

		private int _position;
		private TableItem _baseTable;
		private int[] _validFieldIndices;

		private int currentGroup;
		private int currentTeam;
		
		private bool SaveRanking(DocumentBuilder builder, Page page, Sport.Championships.Group group)
		{
			if (currentTeam == -1)
			{
				if (!AddHeader(builder, page, group.Name, _groupFont))
					return true;
				currentTeam = 0;

				teams = new Sport.Championships.Team[group.Teams.Count];
				group.Teams.CopyTo(teams, 0);
				Array.Sort(teams, new Sport.Championships.TeamPositionComparer());
			}
			else
			{
				AddHeader(builder, page, group.Name + " (המשך)", _groupFont);
			}

			TableItem tableItem = new TableItem(_baseTable);
			page.Items.Add(tableItem);
			int height = tableItem.Rows[0].Height;

			while (currentTeam < teams.Length)
			{
				Sport.Championships.Team team = teams[currentTeam];

				TableItem.TableCell[] cells = new TableItem.TableCell[_validFieldIndices.Length];
				for (int i = 0; i < _validFieldIndices.Length; i++)
				{
					int n = _validFieldIndices[i];
					string text = visualizer.GetText(team, n);
					TableItem.TableCell cell = new TableItem.TableCell();
					if (text != null)
					{
						int index = text.IndexOf(":::");
						if (index > 0)
						{
							string[] arrTemp = text.Split(':');
							string strColors = arrTemp[arrTemp.Length - 1];
							if (strColors != null && strColors.Length > 0)
							{
								string[] arrColors = strColors.Split(',');
								if (arrColors.Length == 3)
								{
									cell.BackColor = Color.FromArgb(Int32.Parse(arrColors[0]), Int32.Parse(arrColors[1]), Int32.Parse(arrColors[2]));
									text = text.Substring(0, index);
								}
							}
						}
					}
					cell.Text = text;
					cells[i] = cell;
				}

				TableItem.TableRow row = new TableItem.TableRow(cells);
				tableItem.MeasureRow(row, builder.MeasureGraphics);

				if (height + _position + row.Height > Bounds.Bottom)
				{
					tableItem.Bounds = new Rectangle(Bounds.Left, _position, Bounds.Width, height);
					return true;
				}

				tableItem.Rows.Add(row);
				height += row.Height;
				currentTeam++;
			}
			
			tableItem.Bounds = new Rectangle(Bounds.Left, _position, Bounds.Width, height);
			_position += height + 10;

			return false;
		}

		private bool SaveGroup(DocumentBuilder builder, Page page)
		{
			Sport.Championships.Group group = _phase.Groups[currentGroup];
			
			if (currentTeam < teams.Length)
			{
				if (SaveRanking(builder, page, group))
					return true;
			}

			currentTeam = -1;
			currentGroup++;

			return false;
		}

		public bool SavePage(DocumentBuilder builder, Page page)
		{
			if (_phase == null)
				return false;

			_position = Bounds.Top;

			if (SaveGroup(builder, page))
				return true;

			return _group == null && currentGroup < _phase.Groups.Count;
		}

		public void InitializeSave(DocumentBuilder builder)
		{
			_baseTable = new TableItem();
			_baseTable.RelativeColumnWidth = true;

			TableItem.TableColumn tc;
			InitializeValidFields();
			TableItem.TableCell[] header = new TableItem.TableCell[_validFieldIndices.Length];
			for (int i = 0; i < _validFieldIndices.Length; i++)
			{
				int n = _validFieldIndices[i];
				
				tc = new TableItem.TableColumn();
				Sport.UI.IVisualizerField visualField = visualizer.GetField(n);
				tc.Width = visualField.DefaultWidth;
				tc.Alignment = (TextAlignment) visualField.Alignment;
				_baseTable.Columns.Add(tc);

				TableItem.TableCell objHeaderCell = new TableItem.TableCell(visualField.Title);
				objHeaderCell.Border = SystemPens.WindowFrame;
				header[i] = objHeaderCell;
			}


			_baseTable.Border = SystemPens.WindowFrame;
			_baseTable.Bounds = Bounds;

			TableItem.TableRow headerRow = new TableItem.TableRow(header);
			headerRow.BackColor = System.Drawing.Color.SkyBlue;
			headerRow.Font = new System.Drawing.Font("Tahoma", 12, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			headerRow.Alignment = Sport.Documents.TextAlignment.Center;
			headerRow.LineAlignment = Sport.Documents.TextAlignment.Center;

			_baseTable.Rows.Add(headerRow);


			_baseTable.InitializeSave(builder);

			teams = new Sport.Championships.Team[0];
			currentTeam = -1;
			currentGroup = _group == null ? 0 : _group.Index;
		}

		public void FinalizeSave(DocumentBuilder builder)
		{
		}

		#endregion

		private void InitializeValidFields()
		{
			Hashtable tblEmptyColumns = null;
			if (_phase != null && currentGroup >= 0 && currentGroup < _phase.Groups.Count && _phase.Groups[currentGroup].Teams != null)
			{
				tblEmptyColumns = new Hashtable();
				foreach (Sport.Championships.Team team in _phase.Groups[currentGroup].Teams)
				{
					for (int n = 0; n < visualizer.Fields.Count; n++)
					{
						string text = visualizer.GetText(team, n);
						if (text == "???")
						{
							if (tblEmptyColumns[n] == null)
							{
								tblEmptyColumns[n] = true;
							}
						}
						else
						{
							if (tblEmptyColumns[n] != null)
							{
								tblEmptyColumns[n] = false;
							}
						}
					}
				}
			}
			
			ArrayList arrIndices = new ArrayList();
			for (int n = 0; n < visualizer.Fields.Count; n++)
			{
				if (tblEmptyColumns != null)
				{
					if (tblEmptyColumns[n] != null)
					{
						if ((bool)tblEmptyColumns[n] == true)
						{
							continue;
						}
					}
				}
				arrIndices.Add(n);
			}
			_validFieldIndices = (int[])arrIndices.ToArray(typeof(int));
		}

		protected override void Dispose(bool disposing)
		{
			_baseTable.Dispose();
			base.Dispose (disposing);
		}

	}
}
