using System;
using System.Drawing;
using Sport.Documents;

namespace Sportsman.Producer
{
/*	public class MatchesDocumentBuilder : DocumentBuilder
	{
		private MatchesSectionObject	matchesSectionObject;

		public MatchesDocumentBuilder(System.Drawing.Printing.PrinterSettings settings)
		{
			Direction = Direction.Right;
			
			Font = new System.Drawing.Font("Tahoma", 12, System.Drawing.GraphicsUnit.Pixel);

			SetSettings(settings);

			Image logo = (System.Drawing.Image) Sport.Resources.ImageLists.GetLogo();
			DateTime now = DateTime.Now;

			Section section = new Section();
			Sections.Add(section);

			// Logo image
			section.Items.Add(new ImageItem(new System.Drawing.Rectangle(DefaultMargins.Right - 200, DefaultMargins.Top - 40, 200, 40), logo));

			// Date
			TextItem ti;
			System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("he-IL");
			System.Globalization.DateTimeFormatInfo dtfi = ci.DateTimeFormat;
			ti = new TextItem(now.ToString("dd בMMMM yyyy", dtfi));
			ti.Bounds = new System.Drawing.Rectangle(DefaultMargins.Left, DefaultMargins.Top - 20, 170, 20);
			ti.Alignment = TextAlignment.Far;
			section.Items.Add(ti);
			dtfi.Calendar = new System.Globalization.HebrewCalendar();
			ti = new TextItem(now.ToString("dd בMMMM yyyy", dtfi));
			ti.Bounds = new System.Drawing.Rectangle(DefaultMargins.Left, DefaultMargins.Top - 40, 170, 20);
			ti.Alignment = TextAlignment.Far;
			section.Items.Add(ti);

			// Table list
			matchesSectionObject = new MatchesSectionObject();
			matchesSectionObject.Bounds = new Rectangle(DefaultMargins.Left, DefaultMargins.Top + 10, DefaultMargins.Width, DefaultMargins.Height - 10);
			section.Items.Add(matchesSectionObject);

			// Title
			ti = new FieldTextItem("{" + ((int) TextField.Title).ToString() + "}");
			ti.Bounds = new System.Drawing.Rectangle(DefaultMargins.Right - (DefaultMargins.Width - 160), DefaultMargins.Bottom + 20, DefaultMargins.Width - 160, 20);
			section.Items.Add(ti);

			// Page Number
			ti = new FieldTextItem("עמוד {" + ((int) TextField.Page).ToString() +
				"} מתוך {" + ((int) TextField.PageCount).ToString() + "}");
			ti.Bounds = new System.Drawing.Rectangle(DefaultMargins.Left, DefaultMargins.Bottom + 20, 150, 20);
			ti.Alignment = TextAlignment.Far;
			section.Items.Add(ti);
		}

		public Sport.Championships.MatchChampionship Championship
		{
			get { return matchesSectionObject.Championship; }
			set { matchesSectionObject.Championship = value; }
		}

		public MatchVisualizer.MatchField[] Fields
		{
			get { return matchesSectionObject.Fields; }
			set { matchesSectionObject.Fields = value; }
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose (disposing);
		}

		public class MatchesSectionObject : PageItem, ISectionObject
		{
			private Font		_cycleFont;
			private Font		_groupFont;

			public MatchesSectionObject()
			{
				_fields = new MatchVisualizer.MatchField[0];
				_cycleFont = new System.Drawing.Font("Tahoma", 
					16, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
				_groupFont = new System.Drawing.Font("Tahoma", 
					20, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			}

			private Sport.Championships.MatchChampionship _championship;
			public Sport.Championships.MatchChampionship Championship
			{
				get { return _championship; }
				set { _championship = value; }
			}

			private MatchVisualizer.MatchField[] _fields;
			public MatchVisualizer.MatchField[] Fields
			{
				get { return _fields; }
				set 
				{ 
					if (value == null)
						_fields = new MatchVisualizer.MatchField[0];
					else
						_fields = value; 
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

			private int _phase;
			private int _group;
			private int _round;
			private int _cycle;
			private int _match;
			private MatchVisualizer visualizer;

			private bool SaveCycle(DocumentBuilder builder, Page page, Sport.Championships.Round round)
			{
				Sport.Championships.Cycle cycle = round.Cycles[_cycle];
				if (_match == -1)
				{
					if (!AddHeader(builder, page, round.Name + " - " + cycle.Name + (cycle.Matches.Count == 0 ? " (ריק)" : null), _cycleFont))
						return true;
				}
				else
				{
					AddHeader(builder, page, round.Name + " - " + cycle.Name + " (המשך)", _cycleFont);
				}

				TableItem tableItem = new TableItem(_baseTable);
				page.Items.Add(tableItem);
				int height = tableItem.Rows[0].Height;

				_match = 0;
				int lastTournament = -999;
				while (_match < cycle.Matches.Count)
				{
					Sport.Championships.Match match = cycle.Matches[_match];

					string[] record = new string[_fields.Length];
					for (int n = 0; n < _fields.Length; n++)
					{
						if (lastTournament != match.Tournament ||
							_fields[n] != MatchVisualizer.MatchField.Tournament)
							record[n] = visualizer.GetText(match, _fields[n]);
					}

					TableItem.TableRow row = new TableItem.TableRow(record);

					tableItem.MeasureRow(row, builder.MeasureGraphics);

					if (height + _position + row.Height > Bounds.Bottom)
					{
						if (_match == 0) // no rows were added yet
						{
							page.Items.Remove(tableItem);
							page.Items.Remove(lastHeader);
							_match = -1;
							return true;
						}

						tableItem.Bounds = new Rectangle(Bounds.Left, _position, Bounds.Width, height);
						return true;
					}

					if (lastTournament == match.Tournament)
					{
						row.Border = System.Drawing.SystemPens.ControlLight;
					}
					else
					{
						lastTournament = match.Tournament;
					}

					tableItem.Rows.Add(row);

					height += row.Height;

					_match++;
				}

				tableItem.Bounds = new Rectangle(Bounds.Left, _position, Bounds.Width, height);
				_position += height + 10;

				return false;
			}

			private bool SaveRound(DocumentBuilder builder, Page page, Sport.Championships.MatchGroup group)
			{
				Sport.Championships.Round round = group.Rounds[_round];
				if (_cycle == -1)
				{
					if (round.Cycles.Count == 0)
					{
						if (!AddHeader(builder, page, round.Name + " (ריק)", _cycleFont))
							return true;
					}

					_cycle = 0;
				}

				while (_cycle < round.Cycles.Count)
				{
					if (SaveCycle(builder, page, round))
						return true;

					_match = -1;
					_cycle++;
				}

				return false;
			}

			private bool SaveGroup(DocumentBuilder builder, Page page, Sport.Championships.MatchPhase phase)
			{
				Sport.Championships.MatchGroup group = phase.Groups[_group];
				if (_round == -1)
				{
					if (!AddHeader(builder, page, phase.Name + " - " + group.Name + (group.Rounds.Count == 0 ? " (ריק)" : null), _groupFont))
						return true;

					_round = 0;
				}

				while (_round < group.Rounds.Count)
				{
					if (SaveRound(builder, page, group))
						return true;

					_cycle = -1;
					_round++;
				}

				return false;
			}

			private bool SavePhase(DocumentBuilder builder, Page page)
			{
				Sport.Championships.MatchPhase phase = _championship.Phases[_phase];

				if (_group == -1)
				{
					if (phase.Groups.Count == 0)
					{
						// No groups in phase, just adding phase name
						if (!AddHeader(builder, page, phase.Name + " (ריק)", _groupFont))
							return true;
					}

					_group = 0;
				}
					
				while (_group < phase.Groups.Count)
				{
					if (SaveGroup(builder, page, phase))
						return true;

					_round = -1;
					_group++;
				}

				return false;
			}

			public bool SavePage(DocumentBuilder builder, Page page)
			{
				if (_championship == null)
					return false;

				_position = Bounds.Top;

				if (SavePhase(builder, page))
					return true;

				_group = -1;
				_phase++;
				return _phase < _championship.Phases.Count;
			}

			public void InitializeSave(DocumentBuilder builder)
			{
				_baseTable = new TableItem();
				_baseTable.RelativeColumnWidth = true;

				visualizer = new MatchVisualizer();

				TableItem.TableColumn tc;

				TableItem.TableCell[] header = new TableItem.TableCell[_fields.Length];
				for (int n = 0; n < _fields.Length; n++)
				{
					tc = new TableItem.TableColumn();
					Sport.UI.IVisualizerField visualField = visualizer.GetField((int) _fields[n]);
					tc.Width = visualField.DefaultWidth;
					tc.Alignment = (TextAlignment) visualField.Alignment;
					_baseTable.Columns.Add(tc);

					header[n] = new TableItem.TableCell(visualField.Title);
					header[n].Border = SystemPens.WindowFrame;
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

				_phase = 0;
				_group = -1;
				_round = -1;
				_cycle = -1;
				_match = -1;
			}

			public void FinalizeSave(DocumentBuilder builder)
			{
			}

			#endregion

			protected override void Dispose(bool disposing)
			{
				_baseTable.Dispose();
				base.Dispose (disposing);
			}

		}
	}*/
}
