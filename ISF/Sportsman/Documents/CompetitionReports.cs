using System;
using System.Linq;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using Sport.Documents;
using Sport.UI;
using System.Collections.Generic;

namespace Sportsman.Documents
{
	public class CompetitionReports
	{
		#region private members
		private CompetitionReportType _type = CompetitionReportType.Undefined;
		private System.Drawing.Printing.PrinterSettings _settings = null;
		private object _data = null;
		private readonly int _baseFontSize = 12;
		private readonly string _baseFontFamily = "Tahoma";
		private readonly Font _fontNormal;
		private readonly Font _fontBold;
		private readonly Font _fontHuge;
		#endregion

		#region constructors
		public CompetitionReports(CompetitionReportType type, object data)
		{
			_type = type;
			_data = data;
			_fontNormal = new Font(_baseFontFamily, _baseFontSize, GraphicsUnit.Pixel);
			_fontBold = new Font(_baseFontFamily, _baseFontSize + 1, FontStyle.Bold, GraphicsUnit.Pixel);
			_fontHuge = new Font(_baseFontFamily, _baseFontSize + 3, FontStyle.Bold,
				GraphicsUnit.Pixel); //|FontStyle.Underline
		}

		public CompetitionReports(CompetitionReportType type)
			: this(type, null)
		{
		}
		#endregion

		#region public methods
		public int Print(bool blnLeagueSummary)
		{
			return ExecutePrintDialog(blnLeagueSummary);
		}

		public int Print()
		{
			return Print(false);
		}
		#endregion

		#region general private methods
		#region Execute Print Dialog
		private int ExecutePrintDialog(bool blnLeagueSummary)
		{
			System.Drawing.Printing.PrinterSettings ps;
			Sport.UI.Dialogs.PrintSettingsForm settingsForm;
			if (!Sport.UI.Helpers.GetPrinterSettings(out ps, out settingsForm))
			{
				return 1;
			}
			_settings = ps;
			if (settingsForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				Document document = null;
				switch (_type)
				{
					case CompetitionReportType.CompetitionCompetitorsReport:
						document = CreateCompetitionCompetitorsReport();
						break;
					case CompetitionReportType.CompetitorVoucher:
						document = CreateCompetitorVoucher();
						break;
					case CompetitionReportType.TeamVoucher_School:
					case CompetitionReportType.TeamVoucher_Student:
						document = CreateTeamVoucher(_type);
						break;
					case CompetitionReportType.GroupTeamsReport:
						document = CreateGroupTeamsReport();
						break;
					case CompetitionReportType.RefereeReport:
						document = CreateRefereeReport();
						break;
					case CompetitionReportType.TeamFullReport:
						document = CreateTeamFullReport();
						break;
					case CompetitionReportType.MultiCompetitionReport:
						document = CreateMultiCompetitionReport(blnLeagueSummary);
						break;
					case CompetitionReportType.ClubCompetitionsReport:
						document = CreateClubCompetitionsReport();
						break;
					case CompetitionReportType.TeamCompetitorsReport:
						document = CreateTeamCompetitorsReport();
						break;
				}

				if (document == null)
					return 5;

				if (settingsForm.ShowPreview)
				{
					Sport.UI.Dialogs.PrintForm printForm =
						new Sport.UI.Dialogs.PrintForm(document, _settings);

					if (!printForm.Canceled)
						printForm.ShowDialog();

					printForm.Dispose();
				}
				else
				{
					System.Drawing.Printing.PrintDocument pd = document.CreatePrintDocument(_settings);
					pd.PrintController = new PrintControllerWithPageForm(pd.PrintController, 0);
					pd.Print();
				}

				return 0;
			}

			return 2;
		} //end function ExecutePrintDialog
		#endregion

		#region Standard Report
		private bool _leagueSummary = false;
		private DocumentBuilder BuildStandardReport(
			Sport.Championships.CompetitionPhase phase,
			Sport.Championships.CompetitionGroup group,
			Sport.Championships.Competition competition,
			Sport.Championships.CompetitionTeam team,
			CompetitionReportType reportType, ref Rectangle margins,
			ref Section section, ref int curTop, Data.Table[] dataTables,
			int heatCount, bool blnGotLanes, int totalScore, int verticalTableDiff)
		{
			//get title:
			string strTitle = Forms.ChooseCompetitionReportDialog.CompetitionReportTypeToString(reportType);

			if (_leagueSummary)
				strTitle += " - סיכום ליגה";

			//need to add to the title?
			if (reportType == CompetitionReportType.RefereeReport)
				strTitle += " ל" + competition.SportField.SportFieldType.Name;

			//initlaize builder:
			DocumentBuilder db = GetDocumentBuilder(strTitle, ref margins);

			//report time:
			string strReportTime = "זמן הפקת דו\"ח: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm");

			//team full report?
			ArrayList multipleTeams = null;
			if (reportType == CompetitionReportType.TeamFullReport)
			{
				multipleTeams = new ArrayList();

				//add single or multiple teams.
				if (_data is Sport.Championships.CompetitionTeam)
				{
					multipleTeams.Add(_data as Sport.Championships.CompetitionTeam);
				}
				else if (_data is Sport.Championships.Team[])
				{
					Sport.Championships.Team[] teams = (Sport.Championships.Team[])_data;
					for (int i = 0; i < teams.Length; i++)
						multipleTeams.Add(teams[i] as Sport.Championships.CompetitionTeam);
				}
			}

			//initialize location:
			int initialLeft = margins.Left;
			int curLeft = initialLeft;
			curTop = margins.Top - 15;
			int totalWidth = margins.Width;
			int headerWidth = (int)(((double)totalWidth) / ((double)3));

			//get championship category:
			Sport.Entities.ChampionshipCategory category = null;
			if (competition != null)
				category = competition.Group.Phase.Championship.ChampionshipCategory;
			else if (team != null)
				category = team.Group.Phase.Championship.ChampionshipCategory;
			else if (multipleTeams != null && multipleTeams.Count > 0)
				category = (multipleTeams[0] as Sport.Championships.CompetitionTeam).Group.Phase.Championship.ChampionshipCategory;
			else if (group != null)
				category = group.Phase.Championship.ChampionshipCategory;
			else if (phase != null)
				category = phase.Championship.ChampionshipCategory;

			//got anything?
			if (category == null)
				return null;

			//get championship season and time:
			string strSeason = category.Championship.Season.Name;
			string strYears = Sport.Common.Tools.GetSeasonYears(category.Championship.Season.End);

			//championship and sport type:
			string strChampName = category.Championship.Name + " " + category.Name;
			string strSportField = "";
			if (competition != null)
				strSportField = competition.SportField.Name;

			//phase:
			string strPhaseName = "";
			if (!_leagueSummary)
			{
				if (competition != null)
					strPhaseName = competition.Group.Phase.Name;
				else if (team != null)
					strPhaseName = team.Group.Phase.Name;
				else if (multipleTeams != null && multipleTeams.Count > 0)
					strPhaseName = (multipleTeams[0] as Sport.Championships.CompetitionTeam).Group.Phase.Name;
				else if (group != null)
					strPhaseName = group.Phase.Name;
				else if (phase != null)
					strPhaseName = phase.Name;
			}

			//region
			string strRegion = Core.Tools.GetUserOrTeamRegion(category.Championship);

			//date and time:
			string strCompetitionDate = "";
			string strCompetitionTime = "";
			if (!_leagueSummary)
			{
				DateTime dtCompetitionTime = DateTime.MinValue;
				if (competition != null)
					dtCompetitionTime = competition.Group.GetGroupTime();
				else if (team != null)
					dtCompetitionTime = team.Group.GetGroupTime();
				else if (multipleTeams != null && multipleTeams.Count > 0)
					dtCompetitionTime = (multipleTeams[0] as Sport.Championships.CompetitionTeam).Group.GetGroupTime();
				else if (group != null)
					dtCompetitionTime = group.GetGroupTime();
				if (dtCompetitionTime.Year > 1900)
				{
					strCompetitionDate = "תאריך: " + dtCompetitionTime.ToString("dd/MM/yy");
					if (reportType == CompetitionReportType.CompetitionCompetitorsReport)
						strCompetitionTime = "שעה: " + dtCompetitionTime.ToString("HH:mm");
				}
			}

			bool blnGotData = true;
			int curTeamIndex = 0;
			while (blnGotData)
			{
				curLeft = initialLeft;
				curTop = margins.Top - 15;

				if (multipleTeams != null)
				{
					if (curTeamIndex >= multipleTeams.Count)
						break;
					team = (Sport.Championships.CompetitionTeam)multipleTeams[curTeamIndex];
				}

				if (reportType == CompetitionReportType.TeamFullReport)
				{
					//get data tables:
					dataTables = team.GetFullReportTables(ref totalScore);

					//got anything?
					if (dataTables == null || dataTables.Length == 0)
					{
						curTeamIndex++;
						continue;
					}
				}

				//school:
				string strSchoolName = (team == null) ? "" : team.TeamEntity.School.Name;

				//build headers:
				ArrayList arrHeaderItems = new ArrayList();
				arrHeaderItems.Add(Documents.BaseDocumentBuilder.BuildTextItem(
					"התאחדות הספורט", ref curLeft, ref curTop, headerWidth,
					_fontBold, 0, 0, false, TextAlignment.Far));
				arrHeaderItems.Add(Documents.BaseDocumentBuilder.BuildTextItem(
					"מחוז " + strRegion, ref curLeft, ref curTop, headerWidth,
					_fontBold, 0, 0, false, TextAlignment.Center));
				arrHeaderItems.Add(Documents.BaseDocumentBuilder.BuildTextItem(
					"משרד החינוך", ref curLeft, ref curTop, headerWidth,
					_fontBold, initialLeft, 3, true));
				arrHeaderItems.Add(Documents.BaseDocumentBuilder.BuildTextItem(
					"לבתי הספר בישראל", ref curLeft, ref curTop, headerWidth,
					_fontBold, 0, 0, false, TextAlignment.Far));
				arrHeaderItems.Add(Documents.BaseDocumentBuilder.BuildTextItem(
					strSeason + " " + strYears, ref curLeft, ref curTop, headerWidth,
					_fontBold, 0, 0, false, TextAlignment.Center));
				arrHeaderItems.Add(Documents.BaseDocumentBuilder.BuildTextItem(
					"הפיקוח על החינוך הגופני", ref curLeft, ref curTop, headerWidth,
					_fontBold, initialLeft, 10, true));
				arrHeaderItems.Add(Documents.BaseDocumentBuilder.BuildTextItem(
					strTitle, ref curLeft, ref curTop, totalWidth, _fontHuge,
					initialLeft, 10, true, TextAlignment.Center));

				arrHeaderItems.Add(Documents.BaseDocumentBuilder.BuildTextItem(
					strReportTime, ref curLeft, ref curTop, totalWidth,
					_fontBold, initialLeft, 0, true, TextAlignment.Near));

				if (strCompetitionDate.Length > 0)
				{
					arrHeaderItems.Add(Documents.BaseDocumentBuilder.BuildTextItem(
						strCompetitionDate, ref curLeft, ref curTop, headerWidth,
						_fontBold, 0, 0, false, TextAlignment.Far));
				}
				arrHeaderItems.Add(Documents.BaseDocumentBuilder.BuildTextItem(
					((reportType == CompetitionReportType.TeamCompetitorsReport) ? "לכבוד" : strChampName),
					ref curLeft, ref curTop, (totalWidth - headerWidth), _fontBold, initialLeft, 3, true));
				if (strCompetitionTime.Length > 0)
				{
					TextItem item = Documents.BaseDocumentBuilder.BuildTextItem(
						strCompetitionTime, ref curLeft, ref curTop, headerWidth,
						_fontBold, 0, 3, true, TextAlignment.Far);
					arrHeaderItems.Add(item);
					curLeft = initialLeft;
				}
				if (reportType == CompetitionReportType.TeamCompetitorsReport)
				{
					if (team != null && team.TeamEntity != null)
					{
						Sport.Entities.School school = team.TeamEntity.School;
						arrHeaderItems.Add(Documents.BaseDocumentBuilder.BuildTextItem(
							"טלפון: " + school.Phone, ref curLeft, ref curTop, headerWidth,
							_fontBold, 0, 0, false, TextAlignment.Far));
						arrHeaderItems.Add(Documents.BaseDocumentBuilder.BuildTextItem(
							"המורה לחינוך גופני", ref curLeft, ref curTop,
							(totalWidth - headerWidth), _fontBold, initialLeft, 3, true));
						arrHeaderItems.Add(Documents.BaseDocumentBuilder.BuildTextItem(
							"פקס: " + school.Fax, ref curLeft, ref curTop, headerWidth,
							_fontBold, 0, 0, false, TextAlignment.Far));
						arrHeaderItems.Add(Documents.BaseDocumentBuilder.BuildTextItem(
							"ביה\"ס " + strSchoolName, ref curLeft, ref curTop,
							(totalWidth - headerWidth), _fontBold, initialLeft, 3, true));
						arrHeaderItems.Add(Documents.BaseDocumentBuilder.BuildTextItem(
							"דף: {" + ((int)Sport.Documents.TextField.Page).ToString() + "}",
							ref curLeft, ref curTop, totalWidth, _fontBold, 0, 10, true, TextAlignment.Far));
					}
				}
				else
				{
					arrHeaderItems.Add(Documents.BaseDocumentBuilder.BuildTextItem(
						"דף:    {" + ((int)Sport.Documents.TextField.Page).ToString() + "}",
						ref curLeft, ref curTop, headerWidth, _fontBold, 0, 0, strPhaseName.Length == 0,
						TextAlignment.Far));
					if (strPhaseName.Length > 0)
					{
						arrHeaderItems.Add(Documents.BaseDocumentBuilder.BuildTextItem(
							"שלב: " + strPhaseName, ref curLeft, ref curTop, (totalWidth - headerWidth),
							_fontBold, initialLeft, 3, true));
					}
					if (strSportField != null && strSportField.Length > 0)
					{
						arrHeaderItems.Add(Documents.BaseDocumentBuilder.BuildTextItem(
							"", ref curLeft, ref curTop, headerWidth, _fontBold, 0, 0, false, TextAlignment.Far));
						arrHeaderItems.Add(Documents.BaseDocumentBuilder.BuildTextItem(
							strSportField, ref curLeft, ref curTop, (totalWidth - headerWidth),
							_fontBold, initialLeft, 30, true));
					}
					if (strSportField.Length == 0 && strSchoolName.Length > 0)
					{
						arrHeaderItems.Add(Documents.BaseDocumentBuilder.BuildTextItem(
							"מוסד: " + strSchoolName, ref curLeft, ref curTop,
							(totalWidth - headerWidth), _fontBold, initialLeft, 3, true));
						arrHeaderItems.Add(Documents.BaseDocumentBuilder.BuildTextItem(
							"ניקוד מצטבר: " + totalScore, ref curLeft, ref curTop, totalWidth,
							_fontBold, initialLeft, 30, true));
					}
				}

				//store top location:
				int headersTop = curTop;

				//initialize section:
				section = new Section();

				//add header items:
				foreach (Sport.Documents.PageItem item in arrHeaderItems)
					section.Items.Add(item);

				//got any table?
				if ((dataTables == null) || (dataTables.Length == 0))
					return db;

				//referee report?
				if (reportType == CompetitionReportType.RefereeReport)
				{
					string strLongUnderscore = "_____________________________";
					string strShortUnderscore = "___________________";
					string strHeat = (blnGotLanes) ? "מקצה" : "עמדה";
					strHeat += " מס' " + (dataTables[0].HeatIndex + 1) + " מתוך " + heatCount + " ";
					strHeat += (blnGotLanes) ? "מקצים" : "עמדות";
					section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
						"מהירות הרוח: " + strShortUnderscore, ref curLeft, ref curTop,
						headerWidth + 100, _fontBold, 0, 0, false, TextAlignment.Far));
					section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
						"שופט: " + strLongUnderscore, ref curLeft, ref curTop, (totalWidth - headerWidth - 100),
						_fontBold, initialLeft, 30, true));
					section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
						"אישור הקלדה: " + strShortUnderscore, ref curLeft, ref curTop,
						headerWidth + 100, _fontBold, 0, 0, false, TextAlignment.Far));
					section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
						"חתימה: " + strLongUnderscore, ref curLeft, ref curTop, (totalWidth - headerWidth - 100),
						_fontBold, initialLeft, 15, true));
					section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
						strHeat, ref curLeft, ref curTop, totalWidth,
						_fontBold, initialLeft, 10, true));
				}

				//add tables:
				int tableWidth = totalWidth;
				int labelHeight = BaseDocumentBuilder.GetLabelHeight();
				foreach (Data.Table dataTable in dataTables)
				{
					//estimate table height:
					int tableHeight = margins.Height - curTop;
					if (dataTables.Length > 1)
					{
						tableHeight = labelHeight * (dataTable.Rows.Length + 1);
					}

					//need new page?
					if ((dataTables.Length > 1) &&
						((curTop + labelHeight + tableHeight) > margins.Bottom))
					{
						//add current section to the document:
						db.Sections.Add(section);

						//create new section:
						section = new Section();

						//add headers:
						foreach (Sport.Documents.PageItem item in arrHeaderItems)
							section.Items.Add(item.Clone());

						//reset top location:
						curTop = headersTop;
					}

					//caption?
					string strTableCaption = dataTable.Caption;
					if ((strTableCaption != null) && (strTableCaption.Length > 0))
					{
						section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
							strTableCaption, ref curLeft, ref curTop, tableWidth,
							_fontBold, initialLeft, 10, true));
					}

					//create table item:
					Sport.Documents.TableItem table = new Sport.Documents.TableItem();
					table.RelativeColumnWidth = false;
					table.Direction = Sport.Documents.Direction.Left;
					table.Bounds = new Rectangle(initialLeft, curTop, tableWidth,
						tableHeight);

					//headers:
					Sport.Documents.TableItem.TableRow headerRow =
						BaseDocumentBuilder.BuildHeaderRow(
						table, dataTable, tableWidth, _fontBold);
					table.Rows.Add(headerRow);

					//iterate over rows and add cells of each row
					for (int rowIndex = 0; rowIndex < dataTable.Rows.Length; rowIndex++)
					{
						Data.Row dataRow = dataTable.Rows[rowIndex];
						if (dataRow.Cells == null)
							continue;
						Sport.Documents.TableItem.TableCell[] cells =
							new Sport.Documents.TableItem.TableCell[dataRow.Cells.Length];
						for (int i = 0; i < dataRow.Cells.Length; i++)
						{
							Data.Cell dataCell = dataRow.Cells[i];
							cells[i] = new Sport.Documents.TableItem.TableCell();
							cells[i].Text = dataCell.Text;
							cells[i].Borders = dataCell.Borders;

							if (dataCell.Alignment != Sport.Documents.TextAlignment.Default)
								cells[i].Alignment = dataCell.Alignment;
							if (dataCell.InnerCells > 1)
								cells[i].InnerCells = dataCell.InnerCells;
							cells[i].Border = (dataCell.ShowBorder) ?
								SystemPens.WindowFrame : null;
						}
						Sport.Documents.TableItem.TableRow row =
							new Sport.Documents.TableItem.TableRow(cells);
						row.VerticalPadding = 5;
						row.Border = null;
						row.Font = _fontNormal;
						table.Rows.Add(row);
					} //end loop over table rows.

					//add to section:
					section.Items.Add(table);

					//update top location:
					curTop += tableHeight + verticalTableDiff;
				} //end loop over data tables.

				if (reportType == CompetitionReportType.TeamFullReport)
					db.Sections.Add(section);

				curTeamIndex++;
				if (multipleTeams == null)
					blnGotData = false;
			}

			//done.
			return db;
		} //end function AddStandardHeader

		private DocumentBuilder BuildStandardReport(
			Sport.Championships.CompetitionGroup group,
			Sport.Championships.Competition competition,
			Sport.Championships.CompetitionTeam team,
			CompetitionReportType reportType, ref Rectangle margins,
			ref Section section, ref int curTop, Data.Table[] dataTables,
			int heatCount, bool blnGotLanes, int totalScore, int verticalTableDiff)
		{
			return BuildStandardReport(null, group, competition, team, reportType,
				ref margins, ref section, ref curTop, dataTables, heatCount,
				blnGotLanes, totalScore, verticalTableDiff);
		}

		private DocumentBuilder BuildStandardReport(
			Sport.Championships.Competition competition,
			CompetitionReportType reportType, ref Rectangle margins,
			ref Section section, ref int curTop, Data.Table dataTable,
			int heatCount, bool blnGotLanes)
		{
			Data.Table[] dataTables = null;
			if (!dataTable.Equals(Data.Table.Empty))
				dataTables = new Data.Table[] { dataTable };
			return BuildStandardReport(null, competition, null, reportType,
				ref margins, ref section, ref curTop, dataTables, heatCount,
				blnGotLanes, -1, 0);
		}

		private DocumentBuilder BuildStandardReport(
			Sport.Championships.Competition competition,
			CompetitionReportType reportType, ref Rectangle margins,
			ref Section section, ref int curTop, Data.Table dataTable)
		{
			return BuildStandardReport(competition, reportType, ref margins,
				ref section, ref curTop, dataTable, 0, false);
		}

		private DocumentBuilder BuildStandardReport(
			Sport.Championships.CompetitionGroup group,
			CompetitionReportType reportType, ref Rectangle margins,
			ref Section section, ref int curTop, Data.Table dataTable)
		{
			Data.Table[] dataTables = null;
			if (!dataTable.Equals(Data.Table.Empty))
				dataTables = new Data.Table[] { dataTable };
			return BuildStandardReport(group, null, null, reportType,
				ref margins, ref section, ref curTop, dataTables, 0, false,
				0, 0);
		}

		private DocumentBuilder BuildStandardReport(
			Sport.Championships.CompetitionPhase phase,
			CompetitionReportType reportType, ref Rectangle margins,
			ref Section section, ref int curTop, Data.Table dataTable)
		{
			Data.Table[] dataTables = null;
			if (!dataTable.Equals(Data.Table.Empty))
				dataTables = new Data.Table[] { dataTable };
			return BuildStandardReport(phase, null, null, null, reportType,
				ref margins, ref section, ref curTop, dataTables, 0, false,
				0, 0);
		}

		private DocumentBuilder BuildStandardReport(
			Sport.Championships.CompetitionTeam team,
			CompetitionReportType reportType, ref Rectangle margins,
			ref Section section, ref int curTop, Data.Table dataTable)
		{
			Data.Table[] dataTables = null;
			if (!dataTable.Equals(Data.Table.Empty))
				dataTables = new Data.Table[] { dataTable };
			return BuildStandardReport(null, null, null, team, reportType,
				ref margins, ref section, ref curTop, dataTables, 0, false,
				0, 0);
		}
		#endregion

		#region general stuff
		private Cursor _lastCursor = null;
		private void SetCurrentCursor(Cursor cursor)
		{
			_lastCursor = Cursor.Current;
			Cursor.Current = cursor;
		}
		private void RestoreLastCursor()
		{
			if (_lastCursor != null)
				Cursor.Current = _lastCursor;
		}

		private DocumentBuilder GetDocumentBuilder(string title, ref Rectangle margins)
		{
			//this may take a while...
			SetCurrentCursor(Cursors.WaitCursor);

			//initlaize builder:
			DocumentBuilder db = new DocumentBuilder(title);

			//general settings:
			db.Direction = Direction.Right;
			db.SetSettings(_settings);

			//get margins:
			margins = db.DefaultMargins;

			//done.
			return db;
		} //end function GetDocumentBuilder
		#endregion
		#endregion

		#region Competition Competitors Report
		private Document CreateCompetitionCompetitorsReport()
		{
			//get competition:
			Sport.Championships.Competition competition =
				(Sport.Championships.Competition)_data;

			//declare local variable:
			CompetitionReportType reportType =
				CompetitionReportType.CompetitionCompetitorsReport;
			Rectangle margins = Rectangle.Empty;
			Section section = null;
			int curTop = 0;

			//get competitors table:
			Data.Table table = competition.GetCompetitorsTable();

			//create document with standard header:
			DocumentBuilder db = BuildStandardReport(competition, reportType,
				ref margins, ref section, ref curTop, table);

			//add the section:
			db.Sections.Add(section);

			//done.
			RestoreLastCursor();
			return db.CreateDocument();
		} //end function CreatePlayersReport
		#endregion

		#region Competition Teams Report
		private Document CreateGroupTeamsReport()
		{
			//get competition:
			Sport.Championships.CompetitionGroup group =
				(Sport.Championships.CompetitionGroup)_data;

			//declare local variable:
			CompetitionReportType reportType =
				CompetitionReportType.GroupTeamsReport;
			Rectangle margins = Rectangle.Empty;
			Section section = null;
			int curTop = 0;

			//get teams table:
			Data.Table table = group.GetTeamsTable();

			//create document with standard header:
			DocumentBuilder db = BuildStandardReport(group, reportType,
				ref margins, ref section, ref curTop, table);

			//add the section:
			db.Sections.Add(section);
			//db.Sections.Add(section2);

			//done.
			RestoreLastCursor();
			return db.CreateDocument();
		} //end function CreateCompetitionTeamsReport
		#endregion

		#region Team Full Report
		private Document CreateTeamFullReport()
		{
			//declare local variable:
			CompetitionReportType reportType =
				CompetitionReportType.TeamFullReport;
			Rectangle margins = Rectangle.Empty;
			Section section = null;
			int curTop = 0;

			//create document with standard header:
			DocumentBuilder db = BuildStandardReport(null, null, null, reportType,
				ref margins, ref section, ref curTop, null, 0, false, 0, 20);

			//add the section:
			//db.Sections.Add(section);

			//done.
			RestoreLastCursor();
			return db.CreateDocument();
		} //end function CreateTeamFullReport
		#endregion

		#region Team Competitors Report
		private Document CreateTeamCompetitorsReport()
		{
			//get team:
			Sport.Championships.CompetitionTeam team = (Sport.Championships.CompetitionTeam)_data;

			//declare local variable:
			CompetitionReportType reportType =
				CompetitionReportType.TeamCompetitorsReport;
			Rectangle margins = Rectangle.Empty;
			Section section = null;
			int curTop = 0;

			//get data table:
			Data.Table table = team.GetCompetitorsTable();

			//create document with standard header and add the section:
			DocumentBuilder db = BuildStandardReport(team, reportType, ref margins,
				ref section, ref curTop, table);
			db.Sections.Add(section);

			//done.
			RestoreLastCursor();
			return db.CreateDocument();
		} //end function CreateTeamFullReport
		#endregion

		#region Multi Competition Report
		public Document CreateMultiCompetitionReport(bool blnLeagueSummary)
		{
			//get groups:
			List<Sport.Championships.CompetitionGroup> groups = new List<Sport.Championships.CompetitionGroup>();
			if (blnLeagueSummary)
			{
				Sport.Championships.CompetitionChampionship championship = (Sport.Championships.CompetitionChampionship)_data;
				championship.Phases.OfType<Sport.Championships.CompetitionPhase>().ToList().ForEach(phase => groups.AddRange(phase.Groups.OfType<Sport.Championships.CompetitionGroup>()));
			}
			else
			{
				groups.Add((Sport.Championships.CompetitionGroup)_data);
			}

			//declare local variable:
			CompetitionReportType reportType =
				CompetitionReportType.MultiCompetitionReport;
			Rectangle margins = Rectangle.Empty;
			Section section = null;
			int curTop = 0;

			//get table of all the competitors and competitions with results:
			Dictionary<int, Dictionary<string, List<Sport.Championships.Competitor>>> tblAllCompetitors = 
				new Dictionary<int, Dictionary<string, List<Sport.Championships.Competitor>>>();
			List<string> arrCompetitions = new List<string>();
			groups.ForEach(group =>
			{
				Hashtable tblGroupCompetitors = group.GetGroupMutliCompetitionCompetitors();
				if (tblGroupCompetitors.Count > 0)
				{
					foreach (int shirtNumber in tblGroupCompetitors.Keys)
					{
						if (!tblAllCompetitors.ContainsKey(shirtNumber))
							tblAllCompetitors.Add(shirtNumber, new Dictionary<string, List<Sport.Championships.Competitor>>());
						Hashtable tblCompetitions = (Hashtable)tblGroupCompetitors[shirtNumber];
						foreach (Sport.Championships.Competition competition in tblCompetitions.Keys)
						{
							string curCompName = competition.Name;
							if (arrCompetitions.IndexOf(curCompName) < 0)
								arrCompetitions.Add(curCompName);
							if (!tblAllCompetitors[shirtNumber].ContainsKey(curCompName))
								tblAllCompetitors[shirtNumber].Add(curCompName, new List<Sport.Championships.Competitor>());
							tblAllCompetitors[shirtNumber][curCompName].Add((Sport.Championships.Competitor)tblCompetitions[competition]);
						}
					}
				}
			});

			//got anything?
			if (tblAllCompetitors.Count == 0)
			{
				Sport.UI.MessageBox.Error("לא נמצאו תוצאות בתחרויות בית זה", "דו\"ח קרב רב");
				return null;
			}
			
			//build data table.
			int colCount = 6 + arrCompetitions.Count;
			Data.Table table = new Data.Table();
			table.Headers = new Data.Column[colCount];
			double compColWidth = (0.42 / arrCompetitions.Count);
			int col = 0;
			table.Headers[col++] = new Data.Column("דרוג", 0.06, false);
			table.Headers[col++] = new Data.Column("משפחה", 0.08, false);
			table.Headers[col++] = new Data.Column("פרטי", 0.08, false);
			table.Headers[col++] = new Data.Column("חזה", 0.08, false);
			table.Headers[col++] = new Data.Column("שם קבוצה", 0.2, false);
			arrCompetitions.ForEach(competitionName =>
			{
				table.Headers[col++] = new Data.Column(competitionName, compColWidth, false);
			});
			table.Headers[col++] = new Data.Column("סה\"כ", 0.08, false);
			List<Data.Row> arrTableRows = new List<Data.Row>();
			foreach (int shirtNumber in tblAllCompetitors.Keys)
			{
				//get competitions and competitor:
				Dictionary<string, List<Sport.Championships.Competitor>> tblCompetitions = tblAllCompetitors[shirtNumber];
				Sport.Championships.Competitor competitor = tblCompetitions[tblCompetitions.Keys.First()].First();

				//create current row
				Data.Row row = new Data.Row();

				//initialize cells:
				row.Cells = new Data.Cell[colCount];
				col = 0;

				//rank:
				row.Cells[col++] = new Data.Cell("", false);

				//name:
				string strLastName = "";
				string strFirstName = "";
				string strTeamName = "";
				Sport.Entities.Player player = null;
				if (competitor.Player != null)
				{
					player = competitor.Player.PlayerEntity;
					strTeamName = competitor.Player.CompetitionTeam.TeamEntity.TeamName();
				}
				if (player != null)
				{
					strFirstName = player.FirstName;
					strLastName = player.LastName;
				}
				row.Cells[col++] = new Data.Cell(strLastName, false);
				row.Cells[col++] = new Data.Cell(strFirstName, false);

				//shirt number:
				row.Cells[col++] = new Data.Cell(competitor.PlayerNumber.ToString(), false);

				//team name:
				row.Cells[col++] = new Data.Cell(strTeamName, false);

				//competitions
				int totalScore = 0;
				arrCompetitions.ForEach(competitionName =>
				{
					string strScore = "";
					if (tblCompetitions.ContainsKey(competitionName))
					{
						int curScore = tblCompetitions[competitionName].ConvertAll(c => c.Score).Sum();
						strScore = curScore.ToString();
						totalScore += curScore;
					}
					row.Cells[col++] = new Data.Cell(strScore, false);
					row.Cells[col - 1].Alignment = Sport.Documents.TextAlignment.Center;
				});

				//total score:
				row.Cells[col++] = new Data.Cell(totalScore.ToString(), false);

				//add current row
				arrTableRows.Add(row);
			} //end loop over competitors

			//sort table rows:
			arrTableRows.Sort((r1, r2) =>
			{
				Data.Cell c1 = r1.Cells[r1.Cells.Length - 1];
				Data.Cell c2 = r2.Cells[r2.Cells.Length - 1];
				int s1, s2;
				if (Int32.TryParse(c1.Text, out s1) && Int32.TryParse(c2.Text, out s2))
					return s2.CompareTo(s1);
				return 0;
			});

			//apply new rank:
			for (int i = 0; i < arrTableRows.Count; i++)
				((Data.Row)arrTableRows[i]).Cells[0].Text = (i + 1).ToString();

			//apply rows in data table:
			table.Rows = arrTableRows.ToArray();

			//create document with standard header:
			if (blnLeagueSummary)
				_leagueSummary = true;
			DocumentBuilder db = BuildStandardReport(groups[0], reportType,
				ref margins, ref section, ref curTop, table);
			_leagueSummary = false;

			//add the section:
			db.Sections.Add(section);

			//done.
			RestoreLastCursor();
			return db.CreateDocument();
		} //end function CreateMultiCompetitionReport
		#endregion

		#region Club Competitions Report
		private Document CreateClubCompetitionsReport()
		{
			//get phase:
			Sport.Championships.CompetitionPhase phase =
				(Sport.Championships.CompetitionPhase)_data;

			//declare local variable:
			CompetitionReportType reportType =
				CompetitionReportType.ClubCompetitionsReport;
			Rectangle margins = Rectangle.Empty;
			Section section = null;
			int curTop = 0;

			//get data table:
			string strError = "";
			Data.Table table = phase.Championship.GetClubCompetitionsTable(ref strError);

			//got anything?
			if (strError.Length > 0)
			{
				Sport.UI.MessageBox.Error(strError, "דו\"ח דירוג מועדונים");
				return null;
			}

			//create document with standard header:
			DocumentBuilder db = BuildStandardReport(phase, reportType,
				ref margins, ref section, ref curTop, table);

			//add the section:
			db.Sections.Add(section);

			//done.
			RestoreLastCursor();
			return db.CreateDocument();
		} //end function CreateMultiCompetitionReport
		#endregion

		#region Vouchers
		#region basic voucher
		private Document CreateVoucher(CompetitionReportType voucherType,
			ArrayList competitors, Sport.Championships.CompetitionTeam[] teams,
			Sport.Championships.Competition competition)
		{
			//get margins:
			Rectangle margins = Rectangle.Empty;

			//initlaize builder:
			DocumentBuilder db = GetDocumentBuilder("תעודת הצטיינות", ref margins);

			//initialize location:
			int totalWidth = margins.Width;
			int voucherWidth = (int)(totalWidth * 0.67);
			int captionWidth = (int)(voucherWidth * 0.35);
			int textWidth = (int)(voucherWidth * 0.65);
			int initialLeft = (int)(((double)(totalWidth - voucherWidth)) / ((double)2));
			initialLeft += Sport.Common.Tools.CIntDef(
				Sport.Core.Configuration.ReadString(
				voucherType.ToString() + "_Margins", "SideMargin"), 100);
			int initialTop = Sport.Common.Tools.CIntDef(
					Sport.Core.Configuration.ReadString(
					voucherType.ToString() + "_Margins", "TopMargin"), 500);

			//get group:
			Sport.Championships.CompetitionGroup group = null;
			Sport.Rulesets.Rules.ResultType resultType = null;
			if (competition != null)
			{
				group = competition.Group;
				resultType = competition.ResultType;
			}
			else if (teams != null && teams.Length > 0)
			{
				group = teams[0].Group;
				if (teams[0].Group.Competitions.Count > 0)
					resultType = teams[0].Group.Competitions[0].ResultType;
			}
			else if (competitors != null && competitors.Count > 0)
			{
				for (int i = 0; i < competitors.Count; i++)
				{
					if (competitors[i] is Sport.Championships.Competitor)
					{
						group = (competitors[i] as Sport.Championships.Competitor).GroupTeam.Group;
						resultType = (competitors[i] as Sport.Championships.Competitor).Competition.ResultType;
						break;
					}
				}
			}

			if (group == null)
				return null;

			//get phase:
			string strPhase = group.Phase.Name;

			//get championship category:
			Sport.Entities.ChampionshipCategory category =
				group.Phase.Championship.ChampionshipCategory;

			//championship and sport type:
			string strChampName = category.Championship.Name + " " + category.Name;
			string strSportField = "";
			if (competitors != null && competitors.Count > 0 && competition != null)
				strSportField = competition.SportField.Name;

			//get place and competition time:
			string strPlace = "";
			string strDate = "";
			Sport.Entities.Facility facility = group.GetGroupFacility();
			DateTime time = group.GetGroupTime();
			if (facility != null)
			{
				strPlace = facility.Name;
				if (facility.City != null)
					strPlace += " - " + facility.City.Name;
			}
			if (time.Year > 1900)
				strDate = time.ToString("dd/MM/yyyy");

			//get voucher flags:
			bool blnPersonalVoucher = (competitors != null);
			bool blnTeamVoucherSchool = (voucherType == CompetitionReportType.TeamVoucher_School);
			bool blnTeamVoucherStudent = (voucherType == CompetitionReportType.TeamVoucher_Student);
			if (blnTeamVoucherStudent)
				blnPersonalVoucher = false;

			//how many voucher we need to produce?
			int curVouchersCount = 0;
			int desiredVouchers = 0;
			if (competitors != null)
				desiredVouchers = competitors.Count;
			else if (teams != null)
				desiredVouchers = teams.Length;

			//generate vouchers:
			while (curVouchersCount < desiredVouchers)
			{
				//reset location:
				int curLeft = initialLeft;
				int curTop = initialTop;

				//get school name, result, student name, facility and position:
				int position = -1;
				string strSchoolName = "";
				string strResult = "";
				string strPosition = "";
				string strStudent = "";
				string sharedResultNames = "";
				if (blnPersonalVoucher)
				{
					object oComp = competitors[curVouchersCount];
					int result = 0;
					if (oComp is Sport.Championships.Competitor)
					{
						Sport.Championships.Competitor competitor =
							(Sport.Championships.Competitor)oComp;
						if ((competitor.Player != null) && (competitor.Player.CompetitionTeam != null) &&
							(competitor.Player.CompetitionTeam.TeamEntity != null) &&
							(competitor.Player.CompetitionTeam.TeamEntity.School != null))
						{
							strSchoolName = competitor.Player.CompetitionTeam.TeamEntity.School.Name;
						}
						result = competitor.Result;
						position = competitor.ResultPosition;
						if ((competitor.Player != null) &&
							(competitor.Player.PlayerEntity != null))
						{
							Sport.Entities.Student student = competitor.Player.PlayerEntity.Student;
							if (student != null)
								strStudent = student.FirstName + " " + student.LastName;
						}
						if (competitor.SharedResultNumbers != null && competitor.SharedResultNumbers.Length > 1)
							sharedResultNames = competitor.GetSharedResultNames();
					}
					else if (oComp is Sport.Entities.OfflinePlayer)
					{
						Sport.Entities.OfflinePlayer oPlayer =
							(Sport.Entities.OfflinePlayer)oComp;
						if (oPlayer.Team != null)
							strSchoolName = oPlayer.Team.School.Name;
						else if (oPlayer.OfflineTeam != null)
							strSchoolName = oPlayer.OfflineTeam.ToString();
						result = oPlayer.Result;
						position = oPlayer.Rank;
						strStudent = oPlayer.ToString();
					}
					if (resultType != null)
						strResult = resultType.FormatResult(result);
				}
				else if ((blnTeamVoucherSchool == true) || (blnTeamVoucherStudent == true))
				{
					Sport.Championships.CompetitionTeam team =
						teams[curVouchersCount];
					strSchoolName = team.TeamEntity.School.Name;
					strResult = team.Score.ToString() + " נקודות";
					position = team.Position;
				}

				//relay?
				if (competition != null && competition.IsRelayRace())
					strStudent = "";

				//got position?
				if (position >= 0)
					strPosition = Sport.Common.Tools.IntToHebrew((position + 1), false);

				//initialize section:
				Section section = new Section();

				//add headers:
				AddVoucherLine(section, "בשלב:", strPhase, ref curLeft, ref curTop,
					textWidth, captionWidth, initialLeft);
				AddVoucherLine(section, "באליפות:", strChampName, ref curLeft,
					ref curTop, textWidth, captionWidth, initialLeft);
				if ((blnPersonalVoucher == true) || (blnTeamVoucherStudent == true))
				{
					if (sharedResultNames.Length > 0)
					{
						AddVoucherLine(section, "שמות התלמידים:", sharedResultNames, ref curLeft,
							ref curTop, textWidth, captionWidth, initialLeft);
					}
					else
					{
						AddVoucherLine(section, "שם התלמיד:", strStudent, ref curLeft,
							ref curTop, textWidth, captionWidth, initialLeft);
					}
				}
				AddVoucherLine(section, "בית הספר:", strSchoolName, ref curLeft,
					ref curTop, textWidth, captionWidth, initialLeft);
				if (blnPersonalVoucher == true)
				{
					AddVoucherLine(section, "במקצוע:", strSportField, ref curLeft,
						ref curTop, textWidth, captionWidth, initialLeft);
				}
				AddVoucherLine(section, "בתוצאה:", strResult, ref curLeft, ref curTop,
					textWidth, captionWidth, initialLeft);
				AddVoucherLine(section, "מקום:", strPosition, ref curLeft, ref curTop,
					textWidth, captionWidth, initialLeft, true);
				section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
					strPlace, ref curLeft, ref curTop, textWidth,
					_fontHuge, 0, false));
				section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
					"מקום התחרות:", ref curLeft, ref curTop, captionWidth,
					_fontHuge, initialLeft, 5, true));
				section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
					"תאריך: " + strDate, ref curLeft, ref curTop, voucherWidth,
					_fontHuge, initialLeft, 5, true, TextAlignment.Far));

				//add the section:
				db.Sections.Add(section);

				//advance to next voucher:
				curVouchersCount++;
			} //end loop over vouchers

			//done.
			RestoreLastCursor();
			return db.CreateDocument();
		} //end function CreateVoucher

		private void AddVoucherLine(Section section, string caption, string text,
			ref int curLeft, ref int curTop, int textWidth, int captionWidth,
			int initialLeft, bool blnLastLine)
		{
			var trimmedText = "";
			var extraText = "";
			if (text.Length > 25)
			{
				var words = text.Split(' ');
				var charCount = 0;
				for (var i = 0; i < words.Length; i++)
				{
					var curWord = words[i];
					charCount += curWord.Length + 1;
					if (charCount > 25)
					{
						for (var j = i; j < words.Length; j++)
						{
							extraText += words[j];
							if (j < (words.Length - 1))
								extraText += " ";
						}
						break;
					}
					if (trimmedText.Length > 0)
						trimmedText += " ";
					trimmedText += curWord;
				}
				if (trimmedText.Length > 0 && extraText.Length > 0)
					text = trimmedText;
			}
			section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
				text, ref curLeft, ref curTop, textWidth,
				_fontHuge, 0, false));
			section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
				caption, ref curLeft, ref curTop, captionWidth,
				_fontHuge, initialLeft, ((blnLastLine) ? 30 : 3), true));
			if (trimmedText.Length > 0 && extraText.Length > 0)
			{
				AddVoucherLine(section, "", extraText, ref curLeft, ref curTop, textWidth, captionWidth, initialLeft, blnLastLine);
			}
		}

		private void AddVoucherLine(Section section, string caption, string text,
			ref int curLeft, ref int curTop, int textWidth, int captionWidth,
			int initialLeft)
		{
			AddVoucherLine(section, caption, text, ref curLeft, ref curTop,
				textWidth, captionWidth, initialLeft, false);
		}
		#endregion

		#region other vouchers
		private Document CreateCompetitorVoucher()
		{
			//get data:
			object[] data = (object[])_data;

			//get competition:
			Sport.Championships.Competition competition =
				(Sport.Championships.Competition)data[0];

			//get competitors:
			ArrayList competitors = (ArrayList)data[1];

			//create basic voucher:
			return CreateVoucher(CompetitionReportType.CompetitorVoucher,
				competitors, null, competition);
		}

		private Document CreateTeamVoucher(CompetitionReportType type)
		{
			//get data:
			object[] data = (object[])_data;

			//get competition:
			Sport.Championships.Competition competition =
				(Sport.Championships.Competition)data[0];

			//get team:
			Sport.Championships.CompetitionTeam[] teams =
				(Sport.Championships.CompetitionTeam[])data[1];

			//create basic voucher:
			return CreateVoucher(type, null, teams, competition);
		}
		#endregion
		#endregion

		#region Referee Report
		private Document CreateRefereeReport()
		{
			//get competition:
			Sport.Championships.Competition competition =
				(Sport.Championships.Competition)_data;

			//got anything?
			if (competition == null)
				return null;

			//declare local variable:
			CompetitionReportType reportType =
				CompetitionReportType.RefereeReport;
			Rectangle margins = Rectangle.Empty;
			Section section = null;
			int curTop = 0;

			//get rule:
			Sport.Rulesets.Rules.GeneralSportTypeData objRule =
				(Sport.Rulesets.Rules.GeneralSportTypeData)
				competition.GetRule(typeof(Sport.Rulesets.Rules.GeneralSportTypeData),
				typeof(Sport.Rulesets.Rules.GeneralSportTypeDataRule));

			//get results count and tries count:
			int resultsCount = 1;
			int triesCount = 1;
			bool blnGotLanes = false;
			if (objRule != null)
			{
				resultsCount = objRule.Results;
				triesCount = objRule.Tries;
				blnGotLanes = objRule.HasLanes;
			}

			//build competitor tables.

			//split competitors to their heats:
			Hashtable tblHeatCompetitors = new Hashtable();
			foreach (Sport.Championships.Competitor competitor in competition.Competitors)
			{
				if (competitor == null)
					continue;
				if (tblHeatCompetitors[competitor.Heat] == null)
					tblHeatCompetitors[competitor.Heat] = new ArrayList();
				(tblHeatCompetitors[competitor.Heat] as ArrayList).Add(competitor);
			}

			//calculate tries column width:
			double colTriesWidth = GetTriesWidth(triesCount, resultsCount);
			double remainingWidth = (1 - colTriesWidth);

			//faults column?
			if (resultsCount > 1)
				remainingWidth -= 0.1;

			//build data tables
			DocumentBuilder db = null;
			ArrayList arrHeats = new ArrayList();
			foreach (int heat in tblHeatCompetitors.Keys)
				arrHeats.Add(heat);
			arrHeats.Sort();
			foreach (int heat in arrHeats)
			{
				//get competitors list:
				ArrayList arrCompetitors = (ArrayList)tblHeatCompetitors[heat];

				//create current table:
				Data.Table table = new Data.Table();

				//assign properties:
				table.HeatIndex = heat;

				//headers:
				ArrayList arrColumns = new ArrayList();
				arrColumns.Add(new Data.Column(blnGotLanes ? "מסלול" : "מספר", 0.13 * remainingWidth));
				arrColumns.Add(new Data.Column("שם", 0.2 * remainingWidth));
				arrColumns.Add(new Data.Column("קבוצה", 0.26 * remainingWidth));
				arrColumns.Add(new Data.Column("מספר חזה", 0.13 * remainingWidth));
				arrColumns.Add(new Data.Column("תוצאה" +
					(((triesCount > 1) || (resultsCount > 1)) ? " סופית" : ""), 0.15 * remainingWidth));
				if (resultsCount > 1)
					arrColumns.Add(new Data.Column("פסילות", 0.1));
				if (colTriesWidth > 0)
				{
					double colWidth = (colTriesWidth / resultsCount);
					for (int j = 0; j < resultsCount; j++)
						arrColumns.Add(new Data.Column((resultsCount > 1) ? "" : "נסיונות", colWidth));
				}
				arrColumns.Add(new Data.Column("מיקום", 0.13 * remainingWidth));
				table.Headers = (Data.Column[])arrColumns.ToArray(typeof(Data.Column));

				table.Rows = new Data.Row[arrCompetitors.Count];

				//iterate over competitors:
				int row = 0;
				foreach (Sport.Championships.Competitor competitor in arrCompetitors)
				{
					//initialize column index:
					int col = 0;

					//get player and team entity:
					Sport.Entities.Player player = null;
					Sport.Entities.Team team = null;
					if (competitor.Player != null)
					{
						player = competitor.Player.PlayerEntity;
						if (competitor.Player.CompetitionTeam != null)
							team = competitor.Player.CompetitionTeam.TeamEntity;
					}
					else
					{
						if ((competition.Group != null) && (competition.Group.Teams != null))
						{
							int playerTeam = competition.Group.GetPlayerTeam(competitor.PlayerNumber);
							if (playerTeam >= 0 && competition.Group.Teams[playerTeam] != null)
								team = competition.Group.Teams[playerTeam].TeamEntity;
						}
					}

					//build current row
					table.Rows[row] = new Data.Row();
					table.Rows[row].Cells = new Data.Cell[arrColumns.Count];

					//competitor name:
					string strPlayerName = "";
					if (player != null)
					{
						strPlayerName = player.FirstName + " " + player.LastName;
					}

					//shirt number
					string strShirtNumber = competitor.PlayerNumber.ToString();

					if (competitor.SharedResultNumbers != null && competitor.SharedResultNumbers.Length > 1)
					{
						strPlayerName = competitor.GetSharedResultNames();
						strShirtNumber = string.Join(", ", competitor.SharedResultNumbers);
					}

					//row
					string strRow = (row + 1).ToString();
					table.Rows[row].Cells[col++] = new Data.Cell(strRow);
					
					table.Rows[row].Cells[col++] = new Data.Cell(strPlayerName);

					//team name:
					string strTeamName = "";
					if (team != null)
						strTeamName = team.TeamName();
					table.Rows[row].Cells[col++] = new Data.Cell(strTeamName);
					
					table.Rows[row].Cells[col++] = new Data.Cell(strShirtNumber);

					//result
					table.Rows[row].Cells[col++] = new Data.Cell("");

					//faults?
					if (resultsCount > 1)
						table.Rows[row].Cells[col++] = new Data.Cell("");

					//tries?
					if (colTriesWidth > 0)
					{
						for (int j = 0; j < resultsCount; j++)
							table.Rows[row].Cells[col++] = new Data.Cell("", null, triesCount);
					}

					//position
					table.Rows[row].Cells[col++] = new Data.Cell("");

					//advance to next row...
					row++;
				} //end loop over competitors

				//offset?
				if (blnGotLanes)
				{
					int laneCount = competition.LaneCount;
					if (table.Rows.Length <= (laneCount - 2))
					{
						int offset = (int)(((double)(laneCount - table.Rows.Length)) / ((double)2));
						foreach (Data.Row dataRow in table.Rows)
						{
							int curValue = Sport.Common.Tools.CIntDef(
								dataRow.Cells[0].Text, 0);
							dataRow.Cells[0].Text = (curValue + offset).ToString();
						}
					}
				}

				//create document with standard header:
				DocumentBuilder db2 = BuildStandardReport(competition, reportType,
					ref margins, ref section, ref curTop,
					table, tblHeatCompetitors.Count,
					blnGotLanes);
				if (db == null)
					db = db2;

				//add the section:
				db.Sections.Add(section);
				curTop = 0;
			} //end loop over heats

			//done.
			RestoreLastCursor();
			return (db == null) ? null : db.CreateDocument();
		} //end function CreateRefereeReport

		private double GetTriesWidth(int triesCount, int resultsCount)
		{
			int count = 1;
			if (resultsCount > 1)
			{
				count = resultsCount;
			}
			else if (triesCount > 1)
			{
				count = triesCount;
			}

			if (count < 2)
				return 0;

			if (count < 5)
				return 0.08 * count;

			if (count < 7)
				return 0.07 * count;

			if (count < 9)
				return 0.06 * count;

			return 0.045 * count;
		}
		#endregion

		#region comparers
		#region Competitor Rank comparer
		public class CompetitorsRankComparer : IComparer
		{
			public int Compare(object x, object y)
			{
				if ((x == null) && (y == null))
					return 0;
				if (x == null)
					return 1;
				if (y == null)
					return -1;
				Sport.Championships.Competitor c1 = (Sport.Championships.Competitor)x;
				Sport.Championships.Competitor c2 = (Sport.Championships.Competitor)y;
				if (c1.ResultPosition == c2.ResultPosition)
					return c1.Index.CompareTo(c2.Index);
				return c1.ResultPosition.CompareTo(c2.ResultPosition);
			}
		}
		#endregion

		#region Competitor Heat comparer
		public class CompetitorsHeatComparer : IComparer
		{
			public int Compare(object x, object y)
			{
				if ((x == null) && (y == null))
					return 0;
				if (x == null)
					return 1;
				if (y == null)
					return -1;
				Sport.Championships.Competitor c1 = (Sport.Championships.Competitor)x;
				Sport.Championships.Competitor c2 = (Sport.Championships.Competitor)y;
				return c1.Heat.CompareTo(c2.Heat);
			}
		}
		#endregion

		#region Competitor Position comparer
		public class CompetitorsPositionComparer : IComparer
		{
			public int Compare(object x, object y)
			{
				if ((x == null) && (y == null))
					return 0;
				if (x == null)
					return 1;
				if (y == null)
					return -1;
				Sport.Championships.Competitor c1 = (Sport.Championships.Competitor)x;
				Sport.Championships.Competitor c2 = (Sport.Championships.Competitor)y;
				return c1.Position.CompareTo(c2.Position);
			}
		}
		#endregion
		#endregion
	}
}