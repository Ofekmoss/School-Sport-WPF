using System;
using System.Linq;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using Sport.Documents;
using Sport.UI;
using Sport.Data;
using System.Collections.Generic;

namespace Sportsman.Documents
{
	/// <summary>
	/// Summary description for ChampionshipDocuments.
	/// </summary>
	public class ChampionshipDocuments
	{
		#region private members
		private ChampionshipDocumentType _type = ChampionshipDocumentType.Undefined;
		private System.Drawing.Printing.PrinterSettings _settings = null;
		private object _data = null;
		private readonly int _baseFontSize = 12;
		private readonly string _baseFontFamily = "Tahoma";
		private readonly string _tinyFontFamily = "Aharoni"; //"Arial Black"; //"Times New Roman";
		private readonly Font _fontNormal;
		private readonly Font _fontBold;
		private readonly Font _fontHuge;
		private readonly Font _fontSmall;
		private readonly Font _fontTiny;
		#endregion

		public static readonly int MinimumCategoryCellWidth = 16;

		#region constructors
		public ChampionshipDocuments(ChampionshipDocumentType type, object data)
		{
			_type = type;
			_data = data;
			_fontNormal = new Font(_baseFontFamily, _baseFontSize, GraphicsUnit.Pixel);
			_fontSmall = new Font(_baseFontFamily, _baseFontSize - 3, GraphicsUnit.Pixel);
			_fontTiny = new Font(_tinyFontFamily, _baseFontSize - 5, FontStyle.Bold, GraphicsUnit.Pixel);
			_fontBold = new Font(_baseFontFamily, _baseFontSize + 1, FontStyle.Bold, GraphicsUnit.Pixel);
			_fontHuge = new Font(_baseFontFamily, _baseFontSize + 3, FontStyle.Bold | FontStyle.Underline,
				GraphicsUnit.Pixel);
		}

		public ChampionshipDocuments(ChampionshipDocumentType type)
			: this(type, null)
		{
		}
		#endregion

		#region public methods
		public int Print()
		{
			return ExecutePrintDialog();
		}
		#endregion

		#region general private methods
		private int ExecutePrintDialog()
		{
			System.Drawing.Printing.PrinterSettings ps;
			Sport.UI.Dialogs.PrintSettingsForm settingsForm;
			if (!Sport.UI.Helpers.GetPrinterSettings(out ps, out settingsForm))
			{
				return 1;
			}
			_settings = ps;
			switch (_type)
			{
				case ChampionshipDocumentType.TeamsReport:
					settingsForm.CustomTextKey = "TeamsReport";
					break;
				case ChampionshipDocumentType.ClubReport:
				case ChampionshipDocumentType.AdministrationReport:
				case ChampionshipDocumentType.OtherSportsReport:
					settingsForm.Landscape = true;
					break;
				case ChampionshipDocumentType.RefereePaymentReport:
					settingsForm.Portrait = true;
					break;
			}
			if (settingsForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				Document document = null;
				try
				{
					switch (_type)
					{
						case ChampionshipDocumentType.PlayersReport:
							document = CreatePlayersReport();
							break;
						case ChampionshipDocumentType.TeamsReport:
							document = CreateTeamsReport();
							break;
						case ChampionshipDocumentType.RefereesReport:
							document = CreateRefereesReport();
							break;
						case ChampionshipDocumentType.RefereePaymentReport:
							document = CreateRefereePaymentReport();
							break;
						case ChampionshipDocumentType.ClubReport:
							document = CreateClubOrOtherSportsReport(true);
							break;
						case ChampionshipDocumentType.AdministrationReport:
							document = CreateAdministrationReport();
							break;
						case ChampionshipDocumentType.OtherSportsReport:
							document = CreateClubOrOtherSportsReport(false);
							break;
					}
				}
				catch (DocumentException dex)
				{
					Sport.UI.MessageBox.Error(dex.Message, "הדפסה");
				}
				catch (Exception ex)
				{
					Sport.UI.MessageBox.Error("אירעה שגיאה בעת בניית הדו\"ח המבוקש\nאנא נסו שנית אם שגיאה זו חוזרת על עצמה אנא דווחו", "הדפסה");
					AdvancedTools.ReportExcpetion(ex, "ERROR GENERATING REPORT " + _type.ToString() + " - ");
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

		#region Players Report
		private Document CreatePlayersReport()
		{
			//get team:
			Sport.Entities.Team team = (Sport.Entities.Team)_data;

			//initlaize builder and get margins:
			Rectangle margins = Rectangle.Empty;
			DocumentBuilder db = GetDocumentBuilder("דו\"ח שחקנים רשומים", ref margins);

			//initialize location:
			int initialLeft = margins.Left;
			int curLeft = initialLeft;
			int curTop = margins.Top - 15;
			int totalWidth = margins.Width;
			int headerWidth = (int)(((double)totalWidth) / ((double)3));
			int wideTextWidth = (int)(((double)totalWidth) * (0.6));

			//get championship season and time:
			string strSeason = team.Championship.Season.Name;
			string strYears = Sport.Common.Tools.GetSeasonYears(team.Championship.Season.End);

			//initialize section:
			Section section = new Section();

			//championship:
			string strChampName = team.Championship.Name + " " + team.Category.Name;

			//region
			string strRegion = Core.Tools.GetUserOrTeamRegion(team);

			//add headers:
			section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
				"התאחדות הספורט לבתי הספר",
				ref curLeft, ref curTop, headerWidth, _fontBold, 0, 0, false,
				TextAlignment.Center));
			section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
				"מחוז " + strRegion,
				ref curLeft, ref curTop, headerWidth, _fontBold, 0, 0, false,
				TextAlignment.Center));
			section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
				"משרד החינוך",
				ref curLeft, ref curTop, headerWidth, _fontBold, initialLeft, 3, true));
			section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
				"תאריך: " + DateTime.Now.ToString("dd/MM/yyyy"),
				ref curLeft, ref curTop, headerWidth, _fontBold, 0, 0, false,
				TextAlignment.Center));
			section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
				strSeason + " " + strYears,
				ref curLeft, ref curTop, headerWidth, _fontBold, 0, 0, false,
				TextAlignment.Center));
			section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
				"הפיקוח על החינוך הגופני",
				ref curLeft, ref curTop, headerWidth, _fontBold, initialLeft, 30, true));
			section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
				team.School.Symbol + "  " + team.School.Name,
				ref curLeft, ref curTop, wideTextWidth, _fontBold, 10, false));
			section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
				"תלמידים הרשומים בקבוצה:",
				ref curLeft, ref curTop, totalWidth - wideTextWidth, _fontBold,
				initialLeft, 3, true));
			section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
				strChampName, ref curLeft, ref curTop, wideTextWidth, _fontBold,
				10, false));
			section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
				"באליפות:", ref curLeft, ref curTop, totalWidth - wideTextWidth,
				_fontBold, initialLeft, 3, true));
			section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
				team.Championship.Season.Name, ref curLeft, ref curTop,
				wideTextWidth, _fontBold, 10, false));
			section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
				"ורשאים לשחק בשנת הלימודים:", ref curLeft, ref curTop,
				totalWidth - wideTextWidth, _fontBold, initialLeft, 30, true));

			//players table...

			//initialize service:
			PlayerCardServices.PlayerCardService service =
				new PlayerCardServices.PlayerCardService();

			//assign cookies:
			service.CookieContainer = Sport.Core.Session.Cookies;

			//captions:
			string[] captions = new string[] {"סטטוס", "ת. הפקת כרטיס", "כיתה", 
											   "תאריך לידה", "ת.ז", "שם פרטי", "שם משפחה"};

			//cells width (percentage)
			double[] cellsWidth = new double[] { 0.1, 0.17, 0.07, 0.15, 0.16, 0.17, 0.18 };

			//add captions:
			AddTableRow(section, captions, cellsWidth, 5, ref curLeft, ref curTop,
				_fontBold, totalWidth, initialLeft);

			//get players:
			Sport.Entities.Player[] players = team.GetPlayers();

			//get sport:
			Sport.Entities.Sport sport = team.Championship.Sport;

			//grade lookup:
			Sport.Types.GradeTypeLookup gradeLookup =
				new Sport.Types.GradeTypeLookup(true);

			//iterate over players:
			for (int i = 0; i < players.Length; i++)
			{
				//get current player:
				Sport.Entities.Player player = players[i];

				//get student:
				Sport.Entities.Student student = player.Student;

				//build array of texts:
				ArrayList arrCellsText = new ArrayList();

				//status:
				arrCellsText.Add(player.StatusText);

				//card issue date:
				string strIssueDate = "";
				if (player.GotSticker == 1)
				{
					DateTime dtCardIssue = DateTime.MinValue;
					try
					{
						dtCardIssue = service.CardIssueDate(student.Id, sport.Id);
					}
					catch (Exception ex)
					{
						Sport.Data.AdvancedTools.ReportExcpetion(ex);
						dtCardIssue = DateTime.MinValue;
					}
					if (dtCardIssue.Year > 1900)
						strIssueDate = dtCardIssue.ToString("dd/MM/yyyy");
				}
				arrCellsText.Add(strIssueDate);

				//grade:
				arrCellsText.Add(gradeLookup.Lookup(student.Grade));

				//birthday:
				string strBirthday = "";
				if (student.BirthDate.Year > 1900)
					strBirthday = student.BirthDate.ToString("dd/MM/yyyy");
				arrCellsText.Add(strBirthday);

				//identification:
				arrCellsText.Add(Sport.Common.Tools.BuildIdentificationNumber(student.IdNumber));

				//first name:
				arrCellsText.Add(student.FirstName);

				//last name:
				arrCellsText.Add(student.LastName);

				//vertical diff:
				int vDiff = (i == players.Length - 1) ? 50 : 2;

				//add the cells:
				AddTableRow(section, (string[])arrCellsText.ToArray(typeof(string)),
					cellsWidth, vDiff, ref curLeft, ref curTop, _fontNormal, totalWidth, initialLeft);
			} //end loop over players

			//add footers:
			section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
				"חותמת:_______________", ref curLeft, ref curTop,
				headerWidth - 20, _fontBold, 0, false));
			section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
				"חתימה:_______________", ref curLeft, ref curTop,
				headerWidth - 20, _fontBold, 0, false));
			section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
				"שם הרכז המאשר_______________", ref curLeft, ref curTop,
				headerWidth + 40, _fontBold, initialLeft, 3, true));

			//add the section:
			db.Sections.Add(section);

			//done.
			RestoreLastCursor();
			return db.CreateDocument();
		} //end function CreatePlayersReport

		private void AddTableRow(Section section, string[] cellsText,
			double[] cellsWidth, int vDiff, ref int x, ref int y,
			Font font, int totalWidth, int initialLeft)
		{
			for (int i = 0; i < cellsText.Length; i++)
			{
				int curWidth = (int)(totalWidth * cellsWidth[i]);
				int curDiff = (i == cellsText.Length - 1) ? vDiff : 0;
				int hDiff = (i == cellsText.Length - 1) ? initialLeft : 0;
				bool blnNewLine = (i == cellsText.Length - 1);
				section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
					cellsText[i], ref x, ref y, curWidth, font, hDiff,
					curDiff, blnNewLine));
			}
		} //end class AddTableRow
		#endregion

		#region Teams Report
		private Sport.Documents.Document CreateTeamsReport()
		{
			//get data:
			Sport.Entities.Team[] teams = (Sport.Entities.Team[])_data;

			//got anything?
			if ((teams == null) || (teams.Length == 0))
				return null;

			//initlaize builder and get margins:
			Rectangle margins = Rectangle.Empty;
			DocumentBuilder db = GetDocumentBuilder("דו\"ח רישום קבוצות", ref margins);

			//initialize location:
			int curTop = margins.Top - 25;
			int totalWidth = margins.Width;
			int initialLeft = (int)(totalWidth * 0.2);
			int curLeft = margins.Left;
			int smallTextWidth = (int)(totalWidth * 0.25);
			int wideTextWidth = (int)(totalWidth * 0.9);

			//get first team:
			Sport.Entities.Team team = teams[0];

			//get championship season and time:
			string strSeason = team.Championship.Season.Name;
			string strYears = Sport.Common.Tools.GetSeasonYears(team.Championship.Season.End);

			//initialize section:
			Section section = new Section();

			//championship:
			string strChampName = team.Championship.Name + " " + team.Category.Name;

			//region
			string strRegion = Core.Tools.GetUserOrTeamRegion(team);

			//add headers:
			section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
				"התאחדות הספורט", ref curLeft, ref curTop, smallTextWidth,
				_fontBold, 0, false));
			section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
				"מחוז " + strRegion, ref curLeft, ref curTop, smallTextWidth * 2,
				_fontBold, 0, 0, false, TextAlignment.Center));
			section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
				"משרד החינוך", ref curLeft, ref curTop, smallTextWidth,
				_fontBold, margins.Left, 3, true));
			section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
				"לבתי הספר בישראל", ref curLeft, ref curTop, smallTextWidth,
				_fontBold, 0, false));
			section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
				strSeason + " " + strYears, ref curLeft, ref curTop, smallTextWidth * 2,
				_fontBold, 0, 0, false, TextAlignment.Center));
			section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
				"הפיקוח על החינוך הגופני", ref curLeft, ref curTop, smallTextWidth,
				_fontBold, margins.Left, 30, true));
			section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
				"לכבוד", ref curLeft, ref curTop, totalWidth, _fontBold,
				margins.Left, 3, true));
			section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
				"תאריך: " + DateTime.Now.ToString("dd/MM/yyyy"), ref curLeft, ref curTop,
				smallTextWidth, _fontBold, 0, false));
			section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
				"ביה\"ס " + team.School.Name, ref curLeft, ref curTop,
				(totalWidth - smallTextWidth), _fontBold, margins.Left, 3, true));
			section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
				"דף:    {" + ((int)Sport.Documents.TextField.Page).ToString() + "}",
				ref curLeft, ref curTop, smallTextWidth, _fontBold, 0, false));
			section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
				team.School.Address, ref curLeft, ref curTop,
				(totalWidth - smallTextWidth), _fontBold, margins.Left, 3, true));
			string cityName = team.School.City == null ? "" : team.School.City.Name;
			section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
				cityName, ref curLeft, ref curTop, totalWidth,
				_fontBold, margins.Left, 3, true));
			section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
				"פקס: " + team.School.Fax, ref curLeft, ref curTop, totalWidth,
				_fontBold, margins.Left, 20, true));
			section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
				"אישור רישום למפעלי הספורט לשנת הלימודים: " + strSeason,
				ref curLeft, ref curTop, wideTextWidth - 10, _fontHuge, 10, false));
			section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
				"הנדון:", ref curLeft, ref curTop, (totalWidth - wideTextWidth),
				_fontBold, margins.Left, 5, true));
			section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
				"הננו מאשרים את רישומן של הקבוצות המפורטות מטה",
				ref curLeft, ref curTop, totalWidth, _fontBold, initialLeft, 20, true));

			//custom text:
			string strCustomText =
				Sport.Core.Configuration.ReadString("CustomText", "TeamsReport");
			string[] arrLines =
				Sport.Common.Tools.SplitNoBlank(strCustomText, '|');

			//teams...
			int tableWidth = totalWidth - initialLeft;
			Sport.Documents.TableItem table = new Sport.Documents.TableItem();
			table.RelativeColumnWidth = false;
			table.Direction = Sport.Documents.Direction.Left;
			table.Bounds = new Rectangle(initialLeft, curTop, tableWidth,
				margins.Height - curTop - arrLines.Length * 20 - 50);
			string[] captions = new string[] { "#", "ענף", "שכבה", "מין" };
			Sport.Documents.TableItem.TableCell[] header =
				new Sport.Documents.TableItem.TableCell[captions.Length];
			double[] cellsWidth = new double[] { 0.08, 0.6, 0.14, 0.18 };
			for (int i = 0; i < captions.Length; i++)
			{
				header[i] = new Sport.Documents.TableItem.TableCell(captions[i]);
				header[i].Border = SystemPens.WindowFrame;
				Sport.Documents.TableItem.TableColumn column =
					new Sport.Documents.TableItem.TableColumn();
				column.Width = (int)(tableWidth * cellsWidth[i]);
				column.Alignment = Sport.Documents.TextAlignment.Near;
				table.Columns.Add(column);
			}
			Sport.Documents.TableItem.TableRow row =
				new Sport.Documents.TableItem.TableRow(header);
			row.Font = _fontBold;
			row.Alignment = Sport.Documents.TextAlignment.Near;
			row.LineAlignment = Sport.Documents.TextAlignment.Near;
			table.Rows.Add(row);
			for (int i = 0; i < teams.Length; i++)
			{
				team = teams[i];
				string strIndex = (i + 1).ToString();
				string strSport = team.Championship.Sport.Name; // +" - " + team.Championship.Name;
				string strCategoty = team.Category.Name;
				string strGrades = Sport.Common.Tools.GetOnlyGrades(strCategoty);
				string strSex = Sport.Common.Tools.GetOnlySex(strCategoty);
				string[] cellsText = new string[] {strIndex, strSport, 
													strGrades, strSex};
				row = new Sport.Documents.TableItem.TableRow(cellsText);
				row.Font = _fontNormal;
				table.Rows.Add(row);
			}
			section.Items.Add(table);

			//add footers:
			curTop = margins.Bottom - (arrLines.Length * 20) - 50;
			if (arrLines.Length > 0)
			{
				curLeft = initialLeft;
				foreach (string strLine in arrLines)
				{
					section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
						strLine, ref curLeft, ref curTop, totalWidth,
						_fontNormal, initialLeft, 3, true));
				}
			}

			//add the section:
			db.Sections.Add(section);

			//done.
			RestoreLastCursor();
			return db.CreateDocument();
		} //end function CreateTeamsReport
		#endregion

		#region Referees Report
		private Sport.Documents.Document CreateRefereesReport()
		{
			//get data:
			List<Sport.Championships.Match> allMatches = ((Sport.Championships.Match[])_data).ToList();

			//got anything?
			if ((allMatches == null) || (allMatches.Count == 0))
				return null;

			//get championship category:
			Sport.Entities.ChampionshipCategory category =
				allMatches[0].Cycle.Round.Group.Phase.Championship.ChampionshipCategory;

			//get championship:
			Sport.Entities.Championship championship = category.Championship;

			//get grades only:
			string strGrades = Sport.Common.Tools.GetOnlyGrades(category.Name);
			string strSex = Sport.Common.Tools.GetOnlySex(category.Name);

			//get championship season and time:
			string strSeason = championship.Season.Name;
			string strYears = Sport.Common.Tools.GetSeasonYears(championship.Season.End);

			//get region
			int regionID = -1;
			string strRegion = Core.Tools.GetUserOrTeamRegion(championship, ref regionID);

			//get region supervisor:
			Sport.Entities.Region region = new Sport.Entities.Region(regionID);
			string strRegionSupervisor = "";
			if (Sport.Core.Configuration.ReadString("GeneralSettings", "RefereeReportSupervisor") == "1")
			{
				strRegionSupervisor = Core.UserManager.CurrentUser.Name;
			}
			else if (region.Coordinator != null)
			{
				strRegionSupervisor = region.Coordinator.Name;
			}

			//initlaize builder and get margins:
			Rectangle margins = Rectangle.Empty;
			DocumentBuilder db = GetDocumentBuilder("דו\"ח הזמנת שופטים", ref margins);
			int totalWidth = margins.Width - margins.Left;
			int initialLeft = margins.Left;
			int smallTextWidth = (int)(totalWidth * 0.25);
			int bigTextWidth = (int)(totalWidth * 0.5);
			int tableWidth = totalWidth;
			int singleLabelHeight = Documents.BaseDocumentBuilder.GetLabelHeight();
			int footersHeight = (7 * singleLabelHeight) + 25;
			double[] cellsWidth = new double[] { 0.1, 0.15, 0.15, 0.15, 0.15, 0.15, 0.15 };
			System.Globalization.CultureInfo hebCulture = System.Globalization.CultureInfo.CreateSpecificCulture("he-IL");
			hebCulture.DateTimeFormat.Calendar = new System.Globalization.HebrewCalendar();

			Dictionary<string, List<Sport.Championships.Match>> matchesMapping = new Dictionary<string, List<Sport.Championships.Match>>();
			allMatches.ForEach(match =>
			{
				string key = string.Format("{0}_{1}", match.Time.Date.Ticks, match.Facility.Id);
				if (!matchesMapping.ContainsKey(key))
					matchesMapping.Add(key, new List<Sport.Championships.Match>());
				matchesMapping[key].Add(match);
			});
			List<string> allKeys = matchesMapping.Keys.ToList();
			allKeys.Sort((k1, k2) => long.Parse(k1.Split('_')[0]).CompareTo(long.Parse(k2.Split('_')[0])));
			allKeys.ForEach(key =>
			{
				Sport.Championships.Match[] matches = matchesMapping[key].ToArray();
				Sport.Championships.Match firstMatch = matches[0];

				//get date:
				DateTime date = firstMatch.Time;
				string gamesDate = date.ToString("dd/MM/yyyy");

				//get hebrew date:
				string gamesHebrewDate = date.ToString("dddd dd בMMMM yyyy", hebCulture).Replace("יום ", "");
				
				//get facility:
				Sport.Entities.Facility facility = firstMatch.Facility;
				string strFacilityName = facility.Name;
				string strFacilityAddress = facility.Address;
				string strFacilityPhone = facility.Phone;
				
				//get coordinator:
				List<int> allFunctionaries = new List<int>();
				matches.ToList().FindAll(m => m.Functionaries != null).ForEach(m => allFunctionaries.AddRange(m.Functionaries));
				allFunctionaries = allFunctionaries.Distinct().ToList();
				string strCoordinatorName = "", strCoordinatorPhone = "", strCoordinatorCellPhone = "";
				List<string> refereeNames = new List<string>();
				string orderNumber = Sport.Championships.ChampionshipUtils.BuildReferreOrderNumber(firstMatch);
				Sport.Entities.Functionary functionary = null;
				for (int i = 0; i < allFunctionaries.Count; i++)
				{
					functionary = null;
					try
					{
						functionary = new Sport.Entities.Functionary(allFunctionaries[i]);
					}
					catch
					{
					}
					if (functionary != null && functionary.Id > 0)
					{
						switch (functionary.FunctionaryType)
						{
							case Sport.Types.FunctionaryType.Coordinator:
								strCoordinatorName = functionary.Name;
								strCoordinatorPhone = functionary.Phone;
								strCoordinatorCellPhone = functionary.CellPhone;
								break;
							case Sport.Types.FunctionaryType.Referee:
								refereeNames.Add(functionary.Name);
								break;
						}
					}
				}
				if (refereeNames.Count > 0)
					footersHeight += singleLabelHeight;

				//initialize location:
				int curTop = margins.Top - 25;
				int curLeft = initialLeft;

				//initialize section:
				Section section = new Section();

				//add headers:
				section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
					"התאחדות הספורט", ref curLeft, ref curTop, smallTextWidth,
					_fontNormal, 0, false));
				section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
					"מחוז " + strRegion, ref curLeft, ref curTop, (smallTextWidth * 2) - 15,
					_fontNormal, 0, 0, false, TextAlignment.Center));
				section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
					"משרד החינוך", ref curLeft, ref curTop, smallTextWidth + 15,
					_fontNormal, initialLeft, 3, true));
				section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
					"לבתי הספר בישראל", ref curLeft, ref curTop, smallTextWidth,
					_fontNormal, 0, false));
				section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
					strSeason + " " + strYears, ref curLeft, ref curTop, (smallTextWidth * 2) - 15,
					_fontNormal, 0, 0, false, TextAlignment.Center));
				section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
					"הפיקוח על החינוך הגופני", ref curLeft, ref curTop, smallTextWidth + 15,
					_fontNormal, initialLeft, 30, true));
				section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
					"מס' הזמנה:   " + orderNumber, ref curLeft,
					ref curTop, smallTextWidth + 20, _fontNormal, 0, false));
				section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
					"לכבוד", ref curLeft, ref curTop, (totalWidth - (smallTextWidth + 20)), 
					_fontNormal, initialLeft, 3, true));
				section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
					"תאריך:   " + DateTime.Now.ToString("dd/MM/yyyy"), ref curLeft,
					ref curTop, smallTextWidth + 20, _fontNormal, 0, false));
				section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
					"גב'/מר_________________", ref curLeft, ref curTop,
					(totalWidth - (smallTextWidth + 20)), _fontNormal, initialLeft, 3, true));
				section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
					"דף:   {" + ((int)Sport.Documents.TextField.Page).ToString() + "}",
					ref curLeft, ref curTop, smallTextWidth + 20, _fontNormal, 0, false));
				section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
					"מזכיר/ת איגוד השופטים:", ref curLeft, ref curTop,
					(totalWidth - (smallTextWidth + 20)), _fontNormal, initialLeft, 10, true));
				section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
					"שלום רב,", ref curLeft, ref curTop, totalWidth, _fontNormal,
					initialLeft, 10, true));
				section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
					"הנדון: הזמנה ושיבוץ שופטים לאירועי הספורט בבתי הספר",
					ref curLeft, ref curTop, totalWidth, _fontHuge, initialLeft, 30,
					true, TextAlignment.Center));
				section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
					gamesHebrewDate, ref curLeft, ref curTop, bigTextWidth - 50,
					_fontNormal, 0, false));
				section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
					"אבקש לשלוח שופטים לאליפות בתי הספר שיערכו ביום:", ref curLeft,
					ref curTop, bigTextWidth + 50, _fontNormal, initialLeft, 3, true));
				section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
					strFacilityAddress, ref curLeft, ref curTop, bigTextWidth - 50,
					_fontNormal, 0, false));
				section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
					"בתאריך:  " + gamesDate + "   באולם:  " + strFacilityName, ref curLeft,
					ref curTop, bigTextWidth + 50, _fontNormal, initialLeft, 3, true));
				section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
					"טלפון: " + strFacilityPhone, ref curLeft, ref curTop, bigTextWidth - 50,
					_fontNormal, initialLeft, 3, true));
				section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
					"על פי הפירוט הבא:", ref curLeft, ref curTop, totalWidth,
					_fontNormal, initialLeft, 3, true));
				section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
					"מסגרת המשחקים: בתי ספר", ref curLeft, ref curTop, totalWidth,
					_fontNormal, initialLeft, 10, true));

				//matches...
				int tableHeight = margins.Height - curTop - footersHeight - 30;
				if (tableHeight < (3 * singleLabelHeight))
					throw new DocumentException("יש להדפיס דו\"ח זה בתצוגה אנכית  בלבד");
				Sport.Documents.TableItem table = new Sport.Documents.TableItem();
				table.RelativeColumnWidth = false;
				table.Direction = Sport.Documents.Direction.Left;
				table.Bounds = new Rectangle(initialLeft, curTop, tableWidth, tableHeight);
				string[] captions = new string[] { "מספר", "שכבת", "", "שעת", "", "", "" };
				Sport.Documents.TableItem.TableRow row = BaseDocumentBuilder.BuildTableRow(table, captions, cellsWidth, tableWidth, true, _fontBold);
				table.Rows.Add(row);
				captions = new string[] { "סידורי", "גיל/כיתות", "מין", "המשחק", "קבוצה א'", "קבוצה ב'", "שופטים" };
				row = BaseDocumentBuilder.BuildTableRow(captions, cellsWidth, _fontBold);
				table.Rows.Add(row);
				int curMatchIndex = 0;
				for (int i = 0; i < matches.Length; i++)
				{
					Sport.Championships.Match match = matches[i];
					if (match.RefereeCount == 0)
						continue;
					string strIndex = (i + 1).ToString();
					string strHour = match.Time.ToString("HH:mm");
					string strTeamA = match.GetTeamAName();
					string strTeamB = match.GetTeamBName();
					string strRefereeCount = match.RefereeCount.ToString();
					string[] cellsText = new string[] {(curMatchIndex+1).ToString(), 
											strGrades, strSex, strHour, 
											strTeamA, strTeamB, strRefereeCount};
					row = new Sport.Documents.TableItem.TableRow(cellsText);
					row.Font = _fontNormal;
					table.Rows.Add(row);
					curMatchIndex++;
				}
				section.Items.Add(table);

				//add footers:
				curTop = margins.Bottom - footersHeight - 30;
				curLeft = initialLeft;
				section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
					"טל' בבית: " + strCoordinatorPhone, ref curLeft, ref curTop,
					(int)(bigTextWidth * 0.5), _fontNormal, 0, false));
				section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
					"טל' נייד: " + strCoordinatorCellPhone, ref curLeft, ref curTop,
					(int)(bigTextWidth * 0.5), _fontNormal, 0, false));
				section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
					"אחראי על המשחקים: " + strCoordinatorName, ref curLeft,
					ref curTop, bigTextWidth, _fontNormal, initialLeft, 5, true));
				if (refereeNames.Count > 0)
				{
					section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
						"שמות שופטים מוזמנים: " + string.Join(", ", refereeNames.Distinct()), ref curLeft,
						ref curTop, totalWidth, _fontNormal, initialLeft, 15, true));
				}
				section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
					"שם הרכז המחוזי: " + strRegionSupervisor, ref curLeft,
					ref curTop, totalWidth, _fontNormal, initialLeft, 15, true));


				section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
					"חתימה וחותמת:  __________________", ref curLeft,
					ref curTop, totalWidth, _fontNormal, initialLeft, 5, true));
				section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
					"אישור הרכז הארצי בהתאחדות הספורט לבתי הספר", ref curLeft,
					ref curTop, totalWidth, _fontNormal, initialLeft, 3, true));
				section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
					"הריני מאשר הזמנה זו ומבקש לשלוח שופטים על פי הקריטריונים הבאים: ",
					ref curLeft, ref curTop, totalWidth, _fontNormal, initialLeft, 5, true));
				section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
					"חתימת המאשר:_________________________", ref curLeft, ref curTop,
					bigTextWidth, _fontNormal, 0, false));
				section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
					"שם המאשר:_________________________", ref curLeft,
					ref curTop, bigTextWidth, _fontNormal, initialLeft, 15, true));
				section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
					"חותמת            :_________________________", ref curLeft, ref curTop,
					bigTextWidth, _fontNormal, 0, false));
				section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
					"תאריך        :_________________________", ref curLeft,
					ref curTop, bigTextWidth, _fontNormal, initialLeft, 5, true));

				//add the section:
				db.Sections.Add(section);
			});

			//done.
			RestoreLastCursor();
			return db.CreateDocument();
		} //end function CreateTeamsReport
		#endregion

		#region Referee Payment Report
		private Sport.Documents.Document CreateRefereePaymentReport()
		{
			//get data:
			Sport.Championships.Match[] matches =
				(Sport.Championships.Match[])_data;

			//got anything?
			if ((matches == null) || (matches.Length == 0))
				return null;

			//get referee:
			Sport.Entities.Functionary referee = null;
			foreach (int funcID in matches[0].Functionaries)
			{
				Sport.Entities.Functionary functionary = null;
				try
				{
					functionary = new Sport.Entities.Functionary(funcID);
				}
				catch { }
				if ((functionary != null) && (functionary.FunctionaryType == Sport.Types.FunctionaryType.Referee))
				{
					referee = functionary;
					break;
				}
			}

			//got anything?
			if (referee == null)
				return null;

			//get region:
			Sport.Entities.Region region = referee.Region;
			if (region == null)
				region = matches[0].Cycle.Round.Group.Phase.Championship.ChampionshipCategory.Championship.Region;
			if (region == null)
				region = new Sport.Entities.Region(Core.UserManager.CurrentUser.UserRegion);
			string strRegion = region.Name;

			//get sport:
			string strSportName = matches[0].Cycle.Round.Group.Phase.Championship.ChampionshipCategory.Championship.Sport.Name;

			//get date range:
			DateTime dtStart = matches[0].Time;
			DateTime dtEnd = matches[matches.Length - 1].Time;
			string strMonthStart = Sport.Common.Tools.HebMonthName(dtStart.Month);
			string strMonthEnd = Sport.Common.Tools.HebMonthName(dtEnd.Month);
			string strYearStart = dtStart.Year.ToString();
			string strYearEnd = dtEnd.Year.ToString();
			string strDateRange = strMonthStart;
			if ((dtStart.Month != dtEnd.Month) || (dtStart.Year != dtEnd.Year))
			{
				strDateRange += " ";
				if (dtStart.Year == dtEnd.Year)
					strDateRange += "עד " + strMonthEnd;
				else
					strDateRange += strYearStart + " עד " + strMonthEnd;
			}
			strDateRange += " " + strYearEnd;

			//get functionary data:
			Sport.Types.FunctionaryStatusTypeLookup statusLookup =
				new Sport.Types.FunctionaryStatusTypeLookup();
			Sport.Types.FunctionaryWorkEnviromentLookup workEnviromentLookup =
				new Sport.Types.FunctionaryWorkEnviromentLookup();
			string strRefereeName = referee.Name;
			string strRefereeStatus = statusLookup.Lookup((int)referee.Status);
			string strRefereeIdNumber = referee.IdNumber;
			string strAnotherJob = (referee.HasAnotherJob) ? "כן" : "לא";
			string strWorkEnviroment = workEnviromentLookup.Lookup((int)referee.WorkEnviroment);

			//initlaize builder and get margins:
			Rectangle margins = Rectangle.Empty;
			DocumentBuilder db = GetDocumentBuilder("דו\"ח תשלום לשופט", ref margins);

			//initialize location:
			int curTop = margins.Top - 25;
			int totalWidth = margins.Width - margins.Left;
			int initialLeft = margins.Left;
			int curLeft = initialLeft;
			int smallTextWidth = (int)(totalWidth * 0.25);
			int normalTextWidth = (int)(((double)totalWidth) / ((double)2));

			//initialize section:
			Section section = new Section();

			//add headers:
			section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
				"התאחדות הספורט", ref curLeft, ref curTop, smallTextWidth,
				_fontBold, 0, false));
			section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
				"מחוז " + strRegion, ref curLeft, ref curTop, (smallTextWidth * 2) - 15,
				_fontBold, 0, 0, false, TextAlignment.Center));
			section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
				"משרד החינוך", ref curLeft, ref curTop, smallTextWidth + 15,
				_fontBold, initialLeft, 3, true));
			section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
				"לבתי הספר בישראל", ref curLeft, ref curTop, smallTextWidth,
				_fontBold, 0, false));
			section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
				strDateRange, ref curLeft, ref curTop, (smallTextWidth * 2) - 15,
				_fontBold, 0, 0, false, TextAlignment.Center));
			section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
				"הפיקוח על החינוך הגופני", ref curLeft, ref curTop, smallTextWidth + 15,
				_fontBold, initialLeft, 15, true));
			section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
				"דו\"ח תשלום לשופט", ref curLeft, ref curTop, totalWidth,
				_fontHuge, initialLeft, 15, true, TextAlignment.Center));
			section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
				"מעמד: " + strRefereeStatus, ref curLeft, ref curTop, normalTextWidth,
				_fontNormal, 0, false));
			section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
				"שם מלא: " + strRefereeName, ref curLeft, ref curTop, normalTextWidth,
				_fontNormal, initialLeft, 3, true));
			section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
				"ענף ספורט: " + strSportName, ref curLeft, ref curTop, normalTextWidth,
				_fontNormal, 0, false));
			section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
				"מספר ת\"ז: " + strRefereeIdNumber, ref curLeft, ref curTop, normalTextWidth,
				_fontNormal, initialLeft, 3, true));
			section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
				"מסגרת שיפוט: " + strWorkEnviroment, ref curLeft, ref curTop, normalTextWidth,
				_fontNormal, 0, false));
			section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
				"עובד במקום עבודה נוסף? " + strAnotherJob, ref curLeft, ref curTop, normalTextWidth,
				_fontNormal, initialLeft, 20, true));

			//matches:
			int tableWidth = totalWidth;
			int footersCount = 5;
			int labelHeight = Documents.BaseDocumentBuilder.GetLabelHeight();
			int footerDiff = 25;
			int footersHeight = (footersCount * labelHeight) + ((footersCount - 2) * footerDiff) + 15;
			Sport.Documents.TableItem table = new Sport.Documents.TableItem();
			table.RelativeColumnWidth = false;
			table.Direction = Sport.Documents.Direction.Left;
			table.Bounds = new Rectangle(initialLeft, curTop, tableWidth,
				margins.Height - curTop - footersHeight);
			string[] captions = new string[] { "מס'", "תאריך", "שכבת גיל ומין", 
				"שם אולם", "מספר משחקים", "תעריף למשחק", "סה\"כ עבור משחקים", 
				"נסיעות", "סה\"כ" };
			double[] cellsWidth = new double[] { 0.08, 0.11, 0.12, 0.15, 0.12, 0.1, 
				0.12, 0.1, 0.1 };
			Sport.Documents.TableItem.TableRow row = null;
			row = BaseDocumentBuilder.BuildTableRow(table, captions, cellsWidth,
				tableWidth, true, _fontBold);
			table.Rows.Add(row);
			int gamesCount = 0;
			string strNIS = "₪";
			int grandTotal = 0;
			int totalGames = 0;
			for (int i = 0; i < matches.Length; i++)
			{
				Sport.Championships.Match match = matches[i];
				gamesCount++;
				Sport.Entities.Facility facility = match.Facility;
				int curFacilityID = facility.Id;
				DateTime curMatchTime = match.Time;
				int nextFacilityID = -1;
				DateTime nextMatchTime = DateTime.MinValue;
				if (i < (matches.Length - 1))
				{
					nextFacilityID = matches[i + 1].Facility.Id;
					nextMatchTime = matches[i + 1].Time;
				}
				if ((curFacilityID != nextFacilityID) ||
					(!Sport.Common.Tools.IsSameDate(curMatchTime, nextMatchTime)))
				{
					Sport.Entities.ChampionshipCategory category =
						match.Cycle.Round.Group.Phase.Championship.ChampionshipCategory;
					int tripRate = facility.GetTripRate(referee);
					int gameRate = category.GameRate;
					int gamesPrice = (gameRate * gamesCount);
					int totalPrice = (gamesPrice + tripRate);
					string strFacilityName = facility.Name;
					string strSexAndGrade = category.Name;
					strSexAndGrade = strSexAndGrade.Replace("תלמידים", "בנים");
					strSexAndGrade = strSexAndGrade.Replace("תלמידות", "בנות");
					string strMatchDate = curMatchTime.ToString("dd.M.yy");
					string strIndex = table.Rows.Count.ToString();
					string strTripRate = tripRate + " " + strNIS;
					string strGameRate = gameRate + " " + strNIS;
					string strGamesPrice = gamesPrice + " " + strNIS;
					string strTotalPrice = totalPrice + " " + strNIS;
					string[] cellsText = new string[] { strIndex, strMatchDate, 
						strSexAndGrade, strFacilityName, gamesCount.ToString(), 
						strGameRate, strGamesPrice, strTripRate, strTotalPrice };
					row = new Sport.Documents.TableItem.TableRow(cellsText);
					row.Font = _fontNormal;
					row.VerticalPadding = 5;
					table.Rows.Add(row);
					grandTotal += totalPrice;
					totalGames += gamesCount;
					gamesCount = 0;
				}
			}
			section.Items.Add(table);

			//add footers:
			curTop = margins.Bottom - footersHeight;
			curLeft = initialLeft;
			string strCoordinator = "";
			if (region.Coordinator != null)
				strCoordinator = region.Coordinator.FirstName + " " + region.Coordinator.LastName;
			section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
				"סך הכל לתשלום: " + grandTotal.ToString() + " " + strNIS,
				ref curLeft, ref curTop, normalTextWidth, _fontBold, 0, 0,
				false, Sport.Documents.TextAlignment.Far));
			section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
				"סך הכל משחקים: " + totalGames.ToString(), ref curLeft, ref curTop,
				normalTextWidth, _fontBold, initialLeft, 15, true,
				Sport.Documents.TextAlignment.Far));
			section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
				"רכז מחוז: " + strRegion, ref curLeft, ref curTop, normalTextWidth,
				_fontNormal, 0, 0, false, Sport.Documents.TextAlignment.Far));
			section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
				"שם רכז מחוזי: " + strCoordinator, ref curLeft, ref curTop,
				normalTextWidth, _fontNormal, initialLeft, footerDiff, true));
			section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
				"תאריך: __________________", ref curLeft, ref curTop, normalTextWidth,
				_fontNormal, 0, 0, false, Sport.Documents.TextAlignment.Far));
			section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
				"חתימת הרכז:   __________________", ref curLeft, ref curTop,
				normalTextWidth, _fontNormal, initialLeft, footerDiff, true));
			section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
				"אישור מנהל המחלקה לספורט:                _________________________",
				ref curLeft, ref curTop, totalWidth,
				_fontNormal, initialLeft, footerDiff, true));
			section.Items.Add(Documents.BaseDocumentBuilder.BuildTextItem(
				"אישור מנכ\"ל התאחדות הספורט לבתי הספר: _________________________",
				ref curLeft, ref curTop, totalWidth,
				_fontNormal, initialLeft, footerDiff, true));

			//add the section:
			db.Sections.Add(section);

			//done.
			RestoreLastCursor();
			return db.CreateDocument();
		} //end function CreateTeamsReport
		#endregion

		#region Club Report
		private int CalculateCategoryCellWidth(int categoriesTotalCount, int minWidth)
		{
			int width = minWidth;
			int max = 25;
			int threshold = 50;
			while (width < max)
			{
				if (categoriesTotalCount > threshold)
					break;
				width++;
				threshold -= 3;
			}
			return width;
		}

		private Sport.Documents.Document CreateClubOrOtherSportsReport(bool blnClubs)
		{
			//get data:
			int[] tempData = (int[])_data;
			int regionID = tempData[0];
			int minCategoryCellWidth = tempData[1];
			Sport.Entities.Region region = null;
			if (regionID >= 0)
				region = new Sport.Entities.Region(regionID);

			//get season:
			Sport.Entities.Season curSeason = new Sport.Entities.Season(Sport.Core.Session.Season);

			//get name of region:
			string strRegionName = (regionID < 0) ? "כל המחוזות" : "מחוז " + region.Name;

			//build the title:
			string clubOrOtherCaption = blnClubs ? "מועדונים" : "אירועי ספורט";
			string strTitle = "דו\"ח " + clubOrOtherCaption + " עונת " + curSeason.Name + " " + strRegionName;

			//initlaize builder and get margins:
			Rectangle margins = Rectangle.Empty;
			DocumentBuilder db = GetDocumentBuilder(strTitle, ref margins);
			Rectangle rectTemp = new Rectangle(15, margins.Y, margins.Width + 200, margins.Height);
			margins = rectTemp;

			//get all championships in the selected region:
			Sport.UI.Dialogs.WaitForm.ShowWait("מעבד נתונים אנא המתן...");
			Dictionary<Sport.Entities.ChampionshipCategory, Dictionary<Sport.Entities.School, List<Sport.Entities.Team>>> categoryTeamMapping = 
				Core.Tools.GetAllChampionships(regionID, blnClubs);

			//put into sports and schools:
			Dictionary<int, List<Sport.Entities.ChampionshipCategory>> sportCategories = new Dictionary<int, List<Sport.Entities.ChampionshipCategory>>();
			Dictionary<int, Dictionary<int, int>> schoolTeamCounts =  new Dictionary<int, Dictionary<int, int>>();
			Dictionary<int, int> priceMapping = new Dictionary<int, int>();
			Dictionary<int, int> paymentMapping = new Dictionary<int, int>();
			List<Sport.Entities.School> arrAllSchools = new List<Sport.Entities.School>();
			Dictionary<int, int> currentTeamCountMapping;
			Dictionary<int, string> sportNames = new Dictionary<int, string>();
			categoryTeamMapping.Keys.ToList().ForEach(currentCategory =>
			{
				Sport.Entities.Sport currentSport = currentCategory.Championship.Sport;
				if (!sportCategories.ContainsKey(currentSport.Id))
					sportCategories.Add(currentSport.Id, new List<Sport.Entities.ChampionshipCategory>());
				if (!sportNames.ContainsKey(currentSport.Id))
					sportNames.Add(currentSport.Id, currentSport.Name);
				sportCategories[currentSport.Id].Add(currentCategory);
				Dictionary<Sport.Entities.School, List<Sport.Entities.Team>> currentSchoolTeams = categoryTeamMapping[currentCategory];
				currentSchoolTeams.Keys.ToList().ForEach(currentSchool =>
				{
					if (!schoolTeamCounts.TryGetValue(currentSchool.Id, out currentTeamCountMapping))
					{
						currentTeamCountMapping = new Dictionary<int, int>();
						schoolTeamCounts.Add(currentSchool.Id, currentTeamCountMapping);
					}
					List<Sport.Entities.Team> arrTeams = currentSchoolTeams[currentSchool];
					if (!currentTeamCountMapping.ContainsKey(currentCategory.Id))
						currentTeamCountMapping.Add(currentCategory.Id, 0);
					currentTeamCountMapping[currentCategory.Id] += arrTeams.Count;
					int curPrice = 0;
					int curPayment = 0;
					arrTeams.ForEach(curTeam =>
					{
						Sport.Entities.Charge curCharge = curTeam.Charge;
						if (curCharge != null)
						{
							curPrice += (int)curCharge.Price;
							if (curCharge.Status == Sport.Types.ChargeStatusType.Paid)
								curPayment += (int)curCharge.Price;
						}
					});
					if (!priceMapping.ContainsKey(currentSchool.Id))
						priceMapping.Add(currentSchool.Id, 0);
					priceMapping[currentSchool.Id] += curPrice;
					if (!paymentMapping.ContainsKey(currentSchool.Id))
						paymentMapping.Add(currentSchool.Id, 0);
					paymentMapping[currentSchool.Id] += curPayment;
					if (!arrAllSchools.Exists(s => s.Id.Equals(currentSchool.Id)))
						arrAllSchools.Add(currentSchool);
				});
			});
			arrAllSchools.Sort((s1, s2) =>
			{
				Sport.Entities.City c1 = s1.City;
				Sport.Entities.City c2 = s2.City;
				string name1 = (c1 == null) ? "" : c1.Name;
				string name2 = (c2 == null) ? "" : c2.Name;
				return name1.CompareTo(name2);
			});

			int categoriesTotalCount = sportCategories.Values.ToList().Sum(l => l.Count);
			
			//build the document
			bool blnManyCategories = (categoriesTotalCount > 35);
			bool blnHasMorePages = true;
			int totalWidth = margins.Width - margins.Left;
			int initialLeft = margins.Left;
			int cityWidth = (blnManyCategories) ? 55 : 70;
			int schoolWidth = (blnManyCategories) ? 70 : 85;
			int priceWidth = 30;
			int paidWidth = 30;
			int maxExtraColumns = 10;
			int tableTop = 0;
			int curSchoolIndex = 0;
			int extraCount = 0;
			int grandTotalPaid = 0;
			int grandTotalCharge = 0;
			int grandTotalTeams = 0;
			int subTotalWidth = 25;
			int catWidth = CalculateCategoryCellWidth(categoriesTotalCount, minCategoryCellWidth);
			int extraWidth = catWidth - 10;
			int doubleCatWidth = catWidth * 2;
			if (blnManyCategories)
				doubleCatWidth -= 10;
			Sport.Documents.Borders borderGrades = Sport.Documents.Borders.Top | Sport.Documents.Borders.Left | Sport.Documents.Borders.Right;
			Sport.Documents.Borders borderSexes = Sport.Documents.Borders.Bottom | Sport.Documents.Borders.Left | Sport.Documents.Borders.Right;
			List<TextItem> arrHeaderItems = new List<TextItem>();
			List<int> arrFixedIndices = new List<int>();
			Dictionary<int, int> categoryTotalTeamMapping = new Dictionary<int, int>();
			List<Sport.Entities.ChampionshipCategory> arrAllCategories = new List<Sport.Entities.ChampionshipCategory>();
			sportCategories.Values.ToList().ForEach(l => arrAllCategories.AddRange(l));
			arrAllCategories = arrAllCategories.Distinct().ToList();
			while (blnHasMorePages)
			{
				int curTop = margins.Top - 5;
				int curLeft = initialLeft;
				ArrayList arrItems = new ArrayList();

				//add header:
				if (arrHeaderItems.Count == 0)
				{
					arrHeaderItems.Add(Documents.BaseDocumentBuilder.BuildTextItem(
						"עמוד {" + ((int)Sport.Documents.TextField.Page).ToString() + "} מתוך {" +
						((int)Sport.Documents.TextField.PageCount).ToString() + "}",
						ref curLeft, ref curTop, 250, _fontBold, 0, false));
					arrHeaderItems.Add(Documents.BaseDocumentBuilder.BuildTextItem(strTitle, ref curLeft, ref curTop,
						totalWidth, _fontHuge, initialLeft, 5, true, Sport.Documents.TextAlignment.Center));

					//initial headers:
					int originalTop = curTop;
					curLeft += (priceWidth + paidWidth);
					arrHeaderItems.Add(Documents.BaseDocumentBuilder.BuildTextItem("סה\"כ", ref curLeft, ref curTop,
						subTotalWidth, _fontSmall, 0, 0, false, TextAlignment.Center, Sport.Documents.Borders.All));
					int sportHeight = curTop - originalTop;
					curTop = originalTop;

					//sports:
					int count = 0;
					sportCategories.Keys.ToList().ForEach(currentSportId =>
					{
						int categoryCount = sportCategories[currentSportId].Count;
						bool blnNewLine = (count == sportCategories.Keys.Count - 1);
						arrHeaderItems.Add(Documents.BaseDocumentBuilder.BuildTextItem(sportNames[currentSportId], ref curLeft, ref curTop,
							((categoryCount == 1) ? doubleCatWidth : catWidth * categoryCount), _fontSmall, ((blnNewLine) ? initialLeft : 0), 0,
							blnNewLine, Sport.Documents.TextAlignment.Center, Sport.Documents.Borders.All));
						count++;
					});

					//categories
					arrFixedIndices.AddRange(AddCategoryCell(
						arrHeaderItems, ref curTop, ref curLeft, " ", "שולם", borderGrades, borderSexes, paidWidth, true));
					arrFixedIndices.AddRange(AddCategoryCell(
						arrHeaderItems, ref curTop, ref curLeft, " ", "לתשלום", borderGrades, borderSexes, priceWidth, true));
					int extraLeft = curLeft;
					AddCategoryCell(arrHeaderItems, ref curTop, ref curLeft, " ", " ", borderGrades, borderSexes, subTotalWidth);
					sportCategories.Keys.ToList().ForEach(currentSportId =>
					{
						List<Sport.Entities.ChampionshipCategory> arrCategories = sportCategories[currentSportId];
						int curWidth = (arrCategories.Count > 1) ? catWidth : doubleCatWidth;
						arrCategories.ForEach(currentCategory =>
						{
							string strFullCategory = currentCategory.Name;
							string strGradesOnly = Sport.Common.Tools.GetOnlyGrades(strFullCategory);
							string strSexOnly = Sport.Common.Tools.GetOnlySex(strFullCategory, true); //blnManyCategories
							AddCategoryCell(arrHeaderItems, ref curTop, ref curLeft, strGradesOnly, strSexOnly,
								borderGrades, borderSexes, curWidth);
						});
					});

					//general headers:
					AddCategoryCell(arrHeaderItems, ref curTop, ref curLeft, "רשות מקומית", " ", borderGrades, borderSexes, cityWidth);
					AddCategoryCell(arrHeaderItems, ref curTop, ref curLeft, "שם בית ספר", " ", borderGrades, borderSexes, schoolWidth);
					originalTop = curTop;
					int tempLeft = curLeft;
					AddCategoryCell(arrHeaderItems, ref curTop, ref curLeft, "#", " ", borderGrades, borderSexes, 15,
						true, initialLeft, false);

					//extra columns
					/*
					if (!blnManyCategories)
					{
						int totalExtraWidth = (margins.Width - (tempLeft + 15));
						if (totalExtraWidth > extraWidth)
						{
							int oldTop = curTop;
							int originalLeft = curLeft;
							extraCount = Math.Min(maxExtraColumns, (int)(((double)totalExtraWidth) / ((double)extraWidth)));
							curTop = originalTop;
							curLeft = extraLeft;
							for (int j = 0; j < arrHeaderItems.Count; j++)
							{
								if (arrFixedIndices.IndexOf(j) >= 0)
									continue;
								Rectangle curBounds = (arrHeaderItems[j] as PageItem).Bounds;
								(arrHeaderItems[j] as PageItem).Bounds =
									new Rectangle(curBounds.X + (extraCount * extraWidth), curBounds.Y, curBounds.Width, curBounds.Height);
							}
							for (int i = 0; i < extraCount; i++)
							{
								int tempLeft2 = curLeft;
								int tempTop2 = curTop;
								arrHeaderItems.Insert(2, Documents.BaseDocumentBuilder.BuildTextItem(" ", ref curLeft, ref curTop,
									extraWidth, _fontSmall, 0, 0, true, TextAlignment.Center, borderGrades));
								curLeft = tempLeft2;
								arrHeaderItems.Insert(3, Documents.BaseDocumentBuilder.BuildTextItem(" ", ref curLeft, ref curTop,
									extraWidth, _fontSmall, 0, 0, false, TextAlignment.Center, borderSexes));
								curTop = tempTop2;
							}
							curLeft = originalLeft;
							curTop = oldTop;
						}
					}
					*/
					tableTop = curTop;
				} //end if header items are not populated
				else
				{
					for (int i = 0; i < arrHeaderItems.Count; i++)
					{
						if (arrHeaderItems[i] is FieldTextItem)
							arrHeaderItems[i] = new FieldTextItem(arrHeaderItems[i] as FieldTextItem);
						else
							arrHeaderItems[i] = new TextItem(arrHeaderItems[i] as TextItem);
					}
				}
				curTop = tableTop;
				arrItems.AddRange(arrHeaderItems.ToArray());

				//schools
				blnHasMorePages = false;
				for (int i = curSchoolIndex; i < arrAllSchools.Count; i++)
				{
					if ((curTop + 20) >= margins.Bottom)
					{
						blnHasMorePages = true;
						break;
					}

					Sport.Entities.School school = arrAllSchools[i];
					int totalCount = 0;
					string strCityName = "";
					string strSchoolName = school.Entity.Fields[(int)Sport.Entities.School.Fields.Name].ToString();
					if (school.City != null)
						strCityName = school.City.Name;
					int schoolTotalCharge = priceMapping[school.Id];
					int schoolTotalPayment = paymentMapping[school.Id];
					curLeft = initialLeft;
					grandTotalPaid += schoolTotalPayment;
					grandTotalCharge += schoolTotalCharge;
					arrItems.Add(Documents.BaseDocumentBuilder.BuildTextItem(schoolTotalPayment.ToString(),
						ref curLeft, ref curTop, paidWidth, _fontSmall, 0, 0, false,
						TextAlignment.Center, Sport.Documents.Borders.All));
					arrItems.Add(Documents.BaseDocumentBuilder.BuildTextItem(schoolTotalCharge.ToString(),
						ref curLeft, ref curTop, priceWidth, _fontSmall, 0, 0, false,
						TextAlignment.Center, Sport.Documents.Borders.All));
					for (int j = 0; j < extraCount; j++)
						arrItems.Add(Documents.BaseDocumentBuilder.BuildTextItem(" ", ref curLeft, ref curTop,
							extraWidth, _fontSmall, 0, 0, false, TextAlignment.Center, Sport.Documents.Borders.All));
					Dictionary<int, int> currentTeamCount = schoolTeamCounts[school.Id];
					int totalIndex = arrItems.Add(Documents.BaseDocumentBuilder.BuildTextItem(" ", ref curLeft, ref curTop,
						subTotalWidth, _fontBold, 0, 0, false, TextAlignment.Center, Sport.Documents.Borders.All));
					int curCount;
					arrAllCategories.ForEach(curCategory =>
					{
						int curWidth = (sportCategories[curCategory.Championship.Sport.Id].Count > 1) ? catWidth : doubleCatWidth;
						if (!currentTeamCount.TryGetValue(curCategory.Id, out curCount))
							curCount = 0;
						string strCount = (curCount > 0) ? curCount.ToString() : " ";
						arrItems.Add(Documents.BaseDocumentBuilder.BuildTextItem(strCount, ref curLeft, ref curTop,
							curWidth, _fontBold, 0, 0, false, TextAlignment.Center, Sport.Documents.Borders.All));
						if (!categoryTotalTeamMapping.ContainsKey(curCategory.Id))
							categoryTotalTeamMapping.Add(curCategory.Id, 0);
						categoryTotalTeamMapping[curCategory.Id] += curCount;
						totalCount += curCount;
					});
					(arrItems[totalIndex] as TextItem).Text = totalCount.ToString();
					grandTotalTeams += totalCount;

					arrItems.Add(Documents.BaseDocumentBuilder.BuildTextItem(strCityName, ref curLeft, ref curTop,
						cityWidth, _fontSmall, 0, 0, false, TextAlignment.Center, Sport.Documents.Borders.All));
					arrItems.Add(Documents.BaseDocumentBuilder.BuildTextItem(strSchoolName, ref curLeft, ref curTop,
						schoolWidth, _fontSmall, 0, 0, false, TextAlignment.Center, Sport.Documents.Borders.All));
					arrItems.Add(Documents.BaseDocumentBuilder.BuildTextItem((curSchoolIndex + 1).ToString(), ref curLeft,
						ref curTop, 15, _fontSmall, 0, 0, true, TextAlignment.Center, Sport.Documents.Borders.All));
					curSchoolIndex++;
				} //end loop over schools

				//Grand Totals
				if (blnHasMorePages == false)
				{
					curLeft = initialLeft;
					arrItems.Add(Documents.BaseDocumentBuilder.BuildTextItem(grandTotalPaid.ToString(), ref curLeft,
						ref curTop, paidWidth, _fontSmall, 0, 0, false, TextAlignment.Center, Sport.Documents.Borders.All));
					arrItems.Add(Documents.BaseDocumentBuilder.BuildTextItem(grandTotalCharge.ToString(), ref curLeft,
						ref curTop, priceWidth, _fontSmall, 0, 0, false, TextAlignment.Center, Sport.Documents.Borders.All));
					for (int j = 0; j < extraCount; j++)
						arrItems.Add(Documents.BaseDocumentBuilder.BuildTextItem(" ", ref curLeft, ref curTop,
							extraWidth, _fontSmall, 0, 0, false, TextAlignment.Center, Sport.Documents.Borders.All));
					arrItems.Add(Documents.BaseDocumentBuilder.BuildTextItem(grandTotalTeams.ToString(), ref curLeft,
						ref curTop, subTotalWidth, ((blnManyCategories) ? _fontSmall : _fontNormal), 0, 0, false, 
						TextAlignment.Center, Sport.Documents.Borders.All));
					for (int j = 0; j < arrAllCategories.Count; j++)
					{
						Sport.Entities.ChampionshipCategory curCategory =
							(Sport.Entities.ChampionshipCategory)arrAllCategories[j];
						int curWidth = (sportCategories[curCategory.Championship.Sport.Id].Count > 1) ? catWidth : doubleCatWidth;
						int curTotal = categoryTotalTeamMapping[curCategory.Id];
						arrItems.Add(Documents.BaseDocumentBuilder.BuildTextItem(curTotal.ToString(),
							ref curLeft, ref curTop, curWidth, ((blnManyCategories) ? _fontSmall : _fontNormal), 
							0, 0, false, TextAlignment.Center, Sport.Documents.Borders.All));
					}
					arrItems.Add(Documents.BaseDocumentBuilder.BuildTextItem("סה\"כ",
						ref curLeft, ref curTop, cityWidth, _fontBold, 0, 0, false,
						TextAlignment.Center, Sport.Documents.Borders.All));
				}

				//initialize section:
				Section section = new Section();

				//add items to section:
				foreach (Sport.Documents.PageItem item in arrItems)
					section.Items.Add(item);

				//add the section:
				db.Sections.Add(section);
			} //end main loop over pages
			Sport.UI.Dialogs.WaitForm.HideWait();

			//done.
			RestoreLastCursor();
			return db.CreateDocument();
		} //end function CreateClubOrOtherSportsReport

		private int[] AddCategoryCell(List<TextItem> arrItems, ref int curTop, ref int curLeft,
			string strGradesOnly, string strSexOnly, Sport.Documents.Borders borderGrades,
			Sport.Documents.Borders borderSexes, int curWidth, bool blnNewLine, int initialLeft, bool blnReturnIndices)
		{
			int originalTop = curTop;
			int originalLeft = curLeft;
			arrItems.Add(Documents.BaseDocumentBuilder.BuildTextItem(strGradesOnly, ref curLeft, ref curTop, curWidth, 
				((curWidth == 20) ? _fontTiny : _fontSmall), 0, 0, true, Sport.Documents.TextAlignment.Center, borderGrades));
			int index1 = arrItems.Count - 1;
			curLeft = originalLeft;
			arrItems.Add(Documents.BaseDocumentBuilder.BuildTextItem(strSexOnly, ref curLeft, ref curTop, curWidth, _fontTiny, 
				initialLeft, 0, blnNewLine, Sport.Documents.TextAlignment.Center, borderSexes));
			int index2 = arrItems.Count - 1;
			if (blnNewLine == false)
				curTop = originalTop;

			return (blnReturnIndices) ? (new int[] { index1, index2 }) : (null);
		}

		private int[] AddCategoryCell(List<TextItem> arrItems, ref int curTop, ref int curLeft,
			string strGradesOnly, string strSexOnly, Sport.Documents.Borders borderGrades,
			Sport.Documents.Borders borderSexes, int curWidth, bool blnReturnIndices)
		{
			return AddCategoryCell(arrItems, ref curTop, ref curLeft, strGradesOnly, strSexOnly, borderGrades,
				borderSexes, curWidth, false, 0, blnReturnIndices);
		}

		private void AddCategoryCell(List<TextItem> arrItems, ref int curTop, ref int curLeft,
			string strGradesOnly, string strSexOnly, Sport.Documents.Borders borderGrades,
			Sport.Documents.Borders borderSexes, int curWidth)
		{
			AddCategoryCell(arrItems, ref curTop, ref curLeft, strGradesOnly, strSexOnly, borderGrades,
				borderSexes, curWidth, false, 0, false);
		}
		#endregion

		#region Administration Report
		private Sport.Documents.Document CreateAdministrationReport()
		{
			//get data:
			object[] rawData = (object[])_data;
			Sport.Core.Data.AdministrationReportType reportType = (Sport.Core.Data.AdministrationReportType)rawData[0];
			Sport.Data.Entity[] selectedChamps = (Sport.Data.Entity[])rawData[1];

			//get season:
			Sport.Entities.Season curSeason = new Sport.Entities.Season(Sport.Core.Session.Season);

			//build the title:
			string strTitle = "דו\"ח מנהל הספורט עונת " + curSeason.Name + " " + ((reportType == Sport.Core.Data.AdministrationReportType.Personal) ? "אישי" : "קבוצתי");

			//initlaize builder and get margins:
			Rectangle margins = Rectangle.Empty;
			DocumentBuilder db = GetDocumentBuilder(strTitle, ref margins);
			Rectangle rectTemp = new Rectangle(15, margins.Y, margins.Width + 200, margins.Height);
			margins = rectTemp;

			//get all championships in the selected region:
			Sport.UI.Dialogs.WaitForm.ShowWait("מעבד נתונים אנא המתן...");

			DataServices.DataService service = new DataServices.DataService();
			List<DataServices.SimplePlayerData> arrAllPlayers = service.GetSimplePlayerData(selectedChamps.ToList().ConvertAll(e => e.Id).ToArray()).ToList();
			service.Dispose();

			List<AdminReportItem> reportItems = new List<AdminReportItem>();
			arrAllPlayers.ForEach(p =>
			{
				AdminReportItem currentItem = reportItems.Find(r => r.PlayerData.IdNumber.Equals(p.IdNumber));
				if (currentItem == null)
				{
					currentItem = new AdminReportItem { PlayerData = p, Championships = new List<int>() };
					reportItems.Add(currentItem);
				}
				currentItem.Championships.Add(p.ChampionshipId);
			});
			reportItems.Sort((r1, r2) => r1.PlayerData.TeamId.CompareTo(r2.PlayerData.TeamId));

			//build the document
			bool blnHasMorePages = true;
			int totalWidth = margins.Width - margins.Left;
			int initialLeft = margins.Left;
			int tableTop = 0;
			int champWidth = 130;
			KeyValuePair<string, int>[] tableCaptions = new KeyValuePair<string, int>[] {
				new KeyValuePair<string, int>("שם קבוצה", 90), 
				new KeyValuePair<string, int>("רשות", 60), 
				new KeyValuePair<string, int>("ת.ז", 90), 
				new KeyValuePair<string, int>("שם ספורטאי/ת", 120), 
				new KeyValuePair<string, int>("זכר/נקבה", 60), 
				new KeyValuePair<string, int>("שנת לידה", 60)
			};
			List<PageItem> arrHeaderItems = null;
			int curPlayerIndex = 0;
			while (blnHasMorePages)
			{
				int curTop = margins.Top - 5;
				int curLeft = initialLeft;
				List<PageItem> arrItems = new List<PageItem>();

				//add header:
				if (arrHeaderItems == null)
				{
					arrHeaderItems = new List<PageItem>();
					arrHeaderItems.Add(Documents.BaseDocumentBuilder.BuildTextItem(
						"עמוד {" + ((int)Sport.Documents.TextField.Page).ToString() + "} מתוך {" +
						((int)Sport.Documents.TextField.PageCount).ToString() + "}",
						ref curLeft, ref curTop, 250, _fontBold, 0, false));
					arrHeaderItems.Add(Documents.BaseDocumentBuilder.BuildTextItem(strTitle, ref curLeft, ref curTop,
						totalWidth, _fontHuge, initialLeft, 5, true, Sport.Documents.TextAlignment.Center));
					tableTop = curTop;
				} //end if header items are not populated
				else
				{
					for (int i = 0; i < arrHeaderItems.Count; i++)
					{
						if (arrHeaderItems[i] is FieldTextItem)
							arrHeaderItems[i] = new FieldTextItem(arrHeaderItems[i] as FieldTextItem);
						else
							arrHeaderItems[i] = new TextItem(arrHeaderItems[i] as TextItem);
					}
				}
				curTop = tableTop;
				arrItems.AddRange(arrHeaderItems.ToArray());
				curLeft = margins.Right - 50;

				tableCaptions.ToList().ForEach(c =>
				{
					int curWidth = c.Value;
					curLeft -= curWidth;
					arrItems.Add(Documents.BaseDocumentBuilder.BuildTextItem(c.Key, ref curLeft, ref curTop,
						curWidth, _fontSmall, 0, 0, false, TextAlignment.Center, Sport.Documents.Borders.All));
					curLeft -= curWidth;
				});

				selectedChamps.ToList().ForEach(e =>
				{
					curLeft -= champWidth;
					arrItems.Add(Documents.BaseDocumentBuilder.BuildTextItem(e.Name, ref curLeft, ref curTop,
						champWidth, _fontSmall, 0, 0, false, TextAlignment.Center, Sport.Documents.Borders.All));
					curLeft -= champWidth;
				});

				arrItems.Add(Documents.BaseDocumentBuilder.BuildTextItem("", ref curLeft, ref curTop,
						0, _fontSmall, 0, 0, true, TextAlignment.Center));

				//schools
				blnHasMorePages = false;
				for (int i = curPlayerIndex; i < reportItems.Count; i++)
				{
					DataServices.SimplePlayerData playerData = reportItems[i].PlayerData;
					List<int> participatedChampionships = reportItems[i].Championships;
					if ((curTop + 5) >= (margins.Height + 150))
					{
						blnHasMorePages = true;
						break;
					}

					curLeft = margins.Right - 50;

					string[] currentItems = new string[] { playerData.TeamName, playerData.CityName, 
							playerData.IdNumber, playerData.PlayerName, ((playerData.IsMale) ? "זכר" : "נקבה"), 
							playerData.BirthDate.Year.ToString() };

					for (int j = 0; j < currentItems.Length; j++)
					{
						int curWidth = tableCaptions[j].Value;
						curLeft -= curWidth;
						arrItems.Add(Documents.BaseDocumentBuilder.BuildTextItem(currentItems[j], ref curLeft, ref curTop,
							curWidth, _fontSmall, 0, 0, false, TextAlignment.Center, Sport.Documents.Borders.All));
						curLeft -= curWidth;
					}

					selectedChamps.ToList().ForEach(e =>
					{
						curLeft -= champWidth;
						bool blnParticipating = (participatedChampionships.IndexOf(e.Id) >= 0);
						arrItems.Add(Documents.BaseDocumentBuilder.BuildTextItem(blnParticipating ? "V" : "", ref curLeft, ref curTop,
							champWidth, _fontSmall, 0, 0, false, TextAlignment.Center, Sport.Documents.Borders.All));
						curLeft -= champWidth;
					});

					arrItems.Add(Documents.BaseDocumentBuilder.BuildTextItem("", ref curLeft, ref curTop,
							0, _fontSmall, 0, 0, true, TextAlignment.Center));

					curPlayerIndex++;
				}

				//initialize section:
				Section section = new Section();

				//add items to section:
				arrItems.ForEach(item => section.Items.Add(item));

				//add the section:
				db.Sections.Add(section);
			} //end main loop over pages
			Sport.UI.Dialogs.WaitForm.HideWait();

			//done.
			RestoreLastCursor();
			return db.CreateDocument();
		} //end function CreateAdministrationReport
		//
		#endregion

		private class AdminReportItem
		{
			public DataServices.SimplePlayerData PlayerData { get; set; }
			public List<int> Championships { get; set; }
		}
	} //end class ChampionshipDocuments

}