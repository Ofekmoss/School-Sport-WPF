using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using SportSite.Controls;
using SportSite.Common;
using System.IO;
using System.Text;
using System.Linq;
using ISF.DataLayer;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace SportSite
{
	/// <summary>
	/// Summary description for Results.
	/// </summary>
	public class Results : System.Web.UI.Page
	{
		#region class members
		protected SportSite.Controls.MainView IsfMainView;
		protected System.Web.UI.WebControls.Literal lbStyle;
		protected SportSite.Controls.HebDateTime IsfHebDateTime;
		protected System.Web.UI.WebControls.Table IsfMainBody;
		protected SportSite.Controls.SideNavBar SideNavBar;

		private enum ChampionshipFilter
		{
			Phase = 0,
			Group,
			Round,
			Cycle,
			Competition,
			Team
		}
		#endregion

		#region Initialization
		private string _logFilePath = "";
		private void Page_Load(object sender, System.EventArgs e)
		{
			//throw new System.Web.HttpException(404, "Not Found");

			if (Request.QueryString["clearcache"] == "1")
				CacheStore.Instance.RemoveAll();

			_logFilePath = Server.MapPath("ResultsLog.txt");

			Page.ClientScript.RegisterClientScriptInclude(this.GetType(), "results_js", "/Common/Results.js");

			string strLogPath = _logFilePath;
			//Sport.Core.LogFiles.AppendToLogFile(strLogPath, "page loaded step 1");

			//add hitlog data:
			Common.Tools.AddHitLog(WebSiteServices.WebSitePage.Results, this.Request);

			//Sport.Core.LogFiles.AppendToLogFile(strLogPath, "page loaded step 2");

			//IsfMainView.FlashNews.Visible = true;

			System.Net.CookieContainer cookies = new System.Net.CookieContainer();
			Sport.Core.Session.Cookies = cookies;

			AddStyle();

			//Sport.Core.LogFiles.AppendToLogFile(strLogPath, "style added");

			SetSideMenu();

			//Sport.Core.LogFiles.AppendToLogFile(strLogPath, "side menu has been set");

			DisplayMainBody();

			//Sport.Core.LogFiles.AppendToLogFile(strLogPath, "main body OK");

			string strCode;
			strCode = "var arrListItems=document.getElementsByTagName(\"li\"); ";
			strCode += "for (var i=0; i<arrListItems.length; i++) {";
			strCode += "   arrListItems[i].onclick = ListItemClick;}";
			IsfMainView.clientSide.AddOnloadCommand(strCode, true);

			strCode = "ApplyGlobalLinks();";
			IsfMainView.clientSide.AddOnloadCommand(strCode, true);

			//Sport.Core.LogFiles.AppendToLogFile(strLogPath, "client side OK");

			AddLinksTitle();

			//Sport.Core.LogFiles.AppendToLogFile(strLogPath, "page load success");
		}

		private void Page_UnLoad(object sender, System.EventArgs e)
		{
			Sport.Core.Session.Cookies = null;
		}

		public void Page_PreRender(object sender, System.EventArgs e)
		{

		}

		private void SetSideMenu()
		{
			SideNavBar.CheckActiveLinkHighlight = true;
		}

		private void AddStyle()
		{
			lbStyle.Text = "";
		}
		#endregion

		#region Add View Text
		/// <summary>
		/// add given text into the main view with given color and given size (pixels)
		/// </summary>
		private void AddViewText(string text, int fontSize, Color fontColor, int index)
		{
			//create new label control and apply given attributes:
			Label objLabel = new Label();
			objLabel.Attributes["dir"] = "rtl";
			objLabel.Text = text;
			//objLabel.Font.Size = FontUnit.Point(fontSize);
			objLabel.ForeColor = fontColor;

			//add the label to the panel containder:
			IsfMainView.AddContents(objLabel);
		}

		private void AddViewText(string text, int fontSize, Color fontColor)
		{
			AddViewText(text, fontSize, fontColor, 0);
		}

		private void AddViewText(string text, int fontSize)
		{
			AddViewText(text, fontSize, Color.Black);
		}

		private void AddViewText(string text, bool pureHTML)
		{
			if (pureHTML)
			{
				IsfMainView.AddContents(text);
				return;
			}
			AddViewText(text);
		}

		private void AddViewText(string text)
		{
			AddViewText(text, 12);
		}

		private void AddViewError(string strMessage)
		{
			AddViewText(strMessage, 14, Color.Red, 0);
		}
		#endregion

		#region Main Body
		private void DisplayMainBody()
		{
			string strLogPath = _logFilePath;
			//Sport.Core.LogFiles.AppendToLogFile(strLogPath, "display main body step 1");

			//check facility given...
			if (Request.QueryString["court"] != null)
			{
				int ID = Sport.Common.Tools.CIntDef(Request.QueryString["court"], -1);
				if (ID >= 0)
				{
					//Sport.Core.LogFiles.AppendToLogFile(strLogPath, "showing court");
					ShowCourtDetails(ID);
					return;
				}
			}

			//check facility given...
			if (Request.QueryString["facility"] != null)
			{
				int ID = Sport.Common.Tools.CIntDef(Request.QueryString["facility"], -1);
				if (ID >= 0)
				{
					//Sport.Core.LogFiles.AppendToLogFile(strLogPath, "showing facility");
					ShowFacilityDetails(ID);
					return;
				}
			}

			//check team given...
			if (Request.QueryString["team"] != null && Request.QueryString["report"] == null)
			{
				int ID = Sport.Common.Tools.CIntDef(Request.QueryString["team"], -1);
				if (ID >= 0)
				{
					//Sport.Core.LogFiles.AppendToLogFile(strLogPath, "showing team");
					ShowTeamDetails(ID);
					return;
				}
			}

			//check school given...
			if (Request.QueryString["school"] != null)
			{
				int ID = Sport.Common.Tools.CIntDef(Request.QueryString["school"], -1);
				if (ID >= 0)
				{
					//Sport.Core.LogFiles.AppendToLogFile(strLogPath, "showing school");
					ShowSchoolDetails(ID);
					return;
				}
			}

			//check school given...
			if (Request.QueryString["player"] != null)
			{
				int ID = Sport.Common.Tools.CIntDef(Request.QueryString["player"], -1);
				if (ID >= 0)
				{
					//Sport.Core.LogFiles.AppendToLogFile(strLogPath, "showing player");
					ShowPlayerDetails(ID);
					return;
				}
			}

			//check category given...
			if (Page.Request["category"] != null)
			{
				//Sport.Core.LogFiles.AppendToLogFile(strLogPath, "category: "+categoryID);
				DisplaySingleChampionship();
				return;
			}

			//Sport.Core.LogFiles.AppendToLogFile(strLogPath, "nothing");
			AddViewText("שלום עולם");
		}
		#endregion

		#region Championship
		private void DisplaySingleChampionship()
		{
			string strLogPath = _logFilePath;
			//Sport.Core.LogFiles.AppendToLogFile(strLogPath, "display championship step 1");

			//read category ID from the querystring:
			int iCategoryID = Tools.CIntDef(Request.QueryString["category"], -1);

			string strReport = Request.QueryString["report"] + "";

			//ajax?
			bool blnAjaxMode = (strReport.Length > 0);

			//read section from the querystring:
			string strSection = Tools.CStrDef(Request.QueryString["section"], "");

			//set default section if none was given:
			if (strSection.Length == 0)
				strSection = Data.ChampMemuItemsSections[0];

			//for AJAX:
			string strQueryStringPhase = Request.QueryString["phase"] + "";
			string strQueryStringGroup = Request.QueryString["group"] + "";
			string strQueryStringCompetition = Request.QueryString["competition"] + "";
			string strQueryStringTeam = Request.QueryString["team"] + "";

			string cacheKey = BuildChampCacheKey();
			//iCategoryID, strSection, strReport, strQueryStringPhase, strQueryStringGroup, strQueryStringCompetition, strQueryStringTeam
			string cacheKey_caption = "ChampionshipPageCaption_" + iCategoryID;
			string cacheKey_navLink = "ChampionshipNavLink_" + iCategoryID;
			string cacheKey_regionName = "ChampionshipRegionName_" + iCategoryID;
			object data;
			string strRawHTML = "", regionName = "", pageCaption = "";
			NavBarLink navLink = NavBarLink.OtherChamps;
			if (CacheStore.Instance.Get(cacheKey, out data))
			{
				strRawHTML = data.ToString();
				if (strRawHTML.Length > 0)
				{
					if (CacheStore.Instance.Get(cacheKey_caption, out data))
						pageCaption = data.ToString();
					if (CacheStore.Instance.Get(cacheKey_navLink, out data))
						navLink = (NavBarLink)data;
					if (CacheStore.Instance.Get(cacheKey_regionName, out data))
						regionName = data.ToString();
				}
				Tools.RegisterVisitorActionsJavaScript(IsfMainView.clientSide);
			}
			else
			{
				strRawHTML = BuildChampRawHtml(iCategoryID, blnAjaxMode, strSection, out navLink, out regionName, out pageCaption);
				if (strRawHTML.Length > 0)
				{
					CacheStore.Instance.Update(cacheKey, strRawHTML, 5);
					CacheStore.Instance.Update(cacheKey_caption, pageCaption, 5);
					CacheStore.Instance.Update(cacheKey_navLink, navLink, 5);
					CacheStore.Instance.Update(cacheKey_regionName, regionName, 5);
				}
			}

			if (strRawHTML.Contains("$tab_control_data"))
				strRawHTML = strRawHTML.Replace("$tab_control_data", GetTabControlRawHTML());

			//done.
			if (blnAjaxMode)
			{
				Response.Clear();
				Response.Write("<script type=\"text/javascript\">");
				Response.Write("parent.TabLoadCompleted(\"" + Tools.CleanForJavascript(strRawHTML) + "\");");
				Response.Write("</script>");
				Response.End();
			}
			else
			{
				IsfMainView.SetPageCaption(pageCaption);
				SideNavBar.Links.MakeLinkActive(navLink, regionName);
				IsfMainView.AddContents(strRawHTML);
			}
		}

		private string BuildChampRawHtml(int iCategoryID, bool blnAjaxMode, string strSection, 
			out NavBarLink navLink, out string regionName, out string pageCaption)
		{
			navLink = NavBarLink.OtherChamps;
			regionName = "";
			pageCaption = "";

			//try to create:
			Sport.Entities.ChampionshipCategory category = null;
			if (iCategoryID >= 0)
			{
				try
				{
					category = new Sport.Entities.ChampionshipCategory(iCategoryID);
				}
				catch
				{ }
			}

			//got anything?
			if (category == null || category.Championship == null)
			{
				AddViewError("זיהוי קטגורית אליפות לא חוקי או כשלון בטעינת אליפות");
				return "";
			}

			//open?
			if (!category.Championship.IsOpen)
			{
				AddViewError("אליפות סגורה, לא ניתן להציג פרטים בשלב זה");
				return "";
			}

			string strQueryStringPhase = Request.QueryString["phase"] + "";
			string strQueryStringGroup = Request.QueryString["group"] + "";
			string strQueryStringCompetition = Request.QueryString["competition"] + "";
			string strQueryStringTeam = Request.QueryString["team"] + "";

			//get championship:
			Sport.Entities.Championship entChamp = category.Championship;

			//get region:
			Sport.Entities.Region region = entChamp.Region;

			//competition?
			bool blnCompetition = (entChamp.Sport.SportType == Sport.Types.SportType.Competition);

			//national?
			bool blnNational = region.IsNationalRegion();

			//clubs championship?
			bool blnClubsOnly = entChamp.IsClubs;
			bool blnShowPrint = true;
			string sectionName = "";
			if (region != null)
				regionName = region.Name;
			pageCaption = entChamp.Name + " " + category.Name;
			if (!blnAjaxMode)
			{
				//decide what is the active link and change it.
				if (blnNational)
					navLink = NavBarLink.NationalChamps;
				else if (blnClubsOnly)
					navLink = NavBarLink.ClubChamps;
			}

			//Sport.Core.LogFiles.AppendToLogFile(strLogPath, "link active: "+champ.Region.Name);

			//initialize string for the output:
			System.Text.StringBuilder strHTML = new System.Text.StringBuilder();

			//Sport.Core.LogFiles.AppendToLogFile(strLogPath, "section: "+strSection);

			if (!blnAjaxMode)
			{
				//details:
				strHTML.Append(BuildChampionshipDetails(strSection, category, entChamp, ref sectionName));
			}

			//define HTML to go back:
			string strBackHTML = "<br /><br /><center><a href=\"Main.aspx?action=";
			strBackHTML += (blnClubsOnly) ? SportSiteAction.ClubChampionships.ToString()
				: SportSiteAction.OtherChampionships.ToString();
			strBackHTML += "&r=" + region.Id + "&category=" + category.Id;
			strBackHTML += "#champ_" + entChamp.Id + "\">[חזרה לרשימת אליפויות]";
			strBackHTML += "</a></center>";

			//attachments?
			if (strSection == "attachments")
			{
				//Sport.Core.LogFiles.AppendToLogFile(strLogPath, "attachments");
				strHTML.Append(BuildChampionshipAttachments(category));
				strHTML.Append(strBackHTML);
				return strHTML.ToString();
			}

			//load championship.
			Sport.Championships.Championship championship = null;
			string strErrorMessage = "";
			try
			{
				championship = Sport.Championships.Championship.GetChampionship(iCategoryID);
			}
			catch (Exception ex)
			{
				strErrorMessage = "<!--message: " + ex.Message + ", stack: " + ex.StackTrace + "-->";
			}

			//got anything?
			if (championship == null)
			{
				string errMsg = "שגיאה כללית בעת טעינת נתוני אליפות" +
					strErrorMessage + "<br />" + strBackHTML;
				IsfMainView.AddContents("<div style=\"color: red; font-weight: bold;\">" + errMsg + "</div>");
				return "";
			}

			//got any phases?
			if (championship.Phases == null || championship.Phases.Count == 0)
			{
				strHTML.Append("באליפות זו עדיין לא הוגדרו שלבים, " +
					"אנא נסה במועד מאוחר יותר.<br />");
				strHTML.Append(strBackHTML);
				return strHTML.ToString();
			}

			//get selected values:
			int selPhase = -1;
			int selGroup = -1;
			int selRound = -1;
			int selCycle = -1;
			int selCompetition = -1;
			int selTeam = -1;
			if (blnAjaxMode)
			{
				Int32.TryParse(strQueryStringPhase, out selPhase);
				Int32.TryParse(strQueryStringGroup, out selGroup);
				Int32.TryParse(strQueryStringCompetition, out selCompetition);
				Int32.TryParse(strQueryStringTeam, out selTeam);
			}
			else
			{
				string strFiltersError = ApplyChampionshipFilters(blnCompetition,
					championship, ref selPhase, ref selGroup, ref selRound, ref selCycle,
					ref selCompetition, ref selTeam, iCategoryID);
				if (strFiltersError.Length > 0)
				{
					strHTML.Append(strFiltersError + "<br />" + strBackHTML);
					return strHTML.ToString();
				}
			}

			//get selected phase:
			Sport.Championships.Phase phase = championship.Phases[selPhase];

			//filters.
			if (!blnAjaxMode)
			{
				strHTML.Append("<div id=\"championship_main_body\">");
				strHTML.Append("<div class=\"printable\">");
				strHTML.Append("<h1>" + pageCaption + "</h1>");
				strHTML.Append("<center><b>" + sectionName + "</b></center>");
				strHTML.Append("</div>");
				if (strSection == "teams" || strSection == "ranking")
				{
					strHTML.Append("שלב אליפות: " + BuildChampionshipFilter(ChampionshipFilter.Phase,
						blnCompetition, championship, "phase", "שלבים", selPhase, -1, -1, -1));
				}
				else if (strSection == "games")
				{
					strHTML.Append(BuildGameFilters(blnCompetition, pageCaption, championship,
						selPhase, selGroup, selRound, selCycle, selCompetition, selTeam));
				}
				strHTML.Append("<br />");
			}

			//teams, ranking or games?
			strErrorMessage = "";
			if (strSection == "teams")
			{
				string strTeamsHTML = BuildChampionshipTeams(phase);
				strHTML.Append(strTeamsHTML);
				blnShowPrint = (strTeamsHTML.IndexOf("<li>") > 0);
			}
			else if (strSection == "ranking")
			{
				if (blnCompetition)
				{
					Sport.Documents.Data.Table table = (championship as Sport.Championships.CompetitionChampionship).GetClubCompetitionsTable(ref strErrorMessage);
					if (strErrorMessage.Length > 0)
					{
						strHTML.Append(strErrorMessage);
					}
					else
					{
						strHTML.Append(Tools.BuildChampTable(table));
					}
					blnShowPrint = (strErrorMessage.Length == 0);
					strHTML.Append("<br />");
				}
				else
				{
					string strRankingHTML = BuildRankingTable(phase);
					strHTML.Append(strRankingHTML);
					blnShowPrint = (strRankingHTML.IndexOf("<table") >= 0);
				}
			}
			else if (strSection == "games")
			{
				string strGamesHTML = "";
				if (blnCompetition)
				{
					Sport.Championships.CompetitionGroup group = null;
					Sport.Championships.Competition competition = null;
					Sport.Championships.CompetitionTeam team = null;
					if ((phase != null) && (selGroup >= 0))
					{
						group = (phase.Groups[selGroup] as Sport.Championships.CompetitionGroup);
						if (selCompetition >= 0)
							competition = group.Competitions[selCompetition];
						if (selTeam >= 0)
							team = group.Teams[selTeam];
					}
					strGamesHTML = BuildCompetitionReport(group, competition, team);
				}
				else
				{
					strGamesHTML = BuildChampionshipGames(
						championship as Sport.Championships.MatchChampionship,
						selPhase, selGroup, selRound, selCycle);
				}
				strHTML.Append(strGamesHTML);
				blnShowPrint = (strGamesHTML.IndexOf("<table") >= 0);
			}

			if (!blnAjaxMode)
			{
				//visitors panel.
				strHTML.Append("</div>");
				if (blnShowPrint)
				{
					strHTML.Append(Tools.BuildVisitorActionsPanel("championship_main_body", IsfMainView.clientSide));
				}
				strHTML.Append(strBackHTML);
			}

			return strHTML.ToString();
		}

		#region filters
		private string ApplyChampionshipFilters(bool blnCompetition,
			Sport.Championships.Championship championship, ref int selPhase,
			ref int selGroup, ref int selRound, ref int selCycle,
			ref int selCompetition, ref int selTeam, int categoryID)
		{
			try
			{
				selPhase = GetSelectedFilter("phase", championship.Phases.Count, "שלב", categoryID);
				if (selPhase >= 0)
				{
					Sport.Championships.Phase phase = championship.Phases[selPhase];
					int count = (phase.Groups == null) ? 0 : phase.Groups.Count;
					selGroup = GetSelectedFilter("group", count, "בית", categoryID);
					if (selGroup >= 0)
					{
						Sport.Championships.Group group = phase.Groups[selGroup];
						if (blnCompetition)
						{
							Sport.Championships.CompetitionGroup compGroup =
								(group as Sport.Championships.CompetitionGroup);
							count = (compGroup.Competitions == null) ? 0 : compGroup.Competitions.Count;
							selCompetition = GetSelectedFilter("competition", count, "תחרות", categoryID);
							count = (compGroup.Teams == null) ? 0 : compGroup.Teams.Count;
							selTeam = GetSelectedFilter("team", count, "קבוצה", categoryID);
						}
						else
						{
							Sport.Championships.MatchGroup matchGroup = (group as Sport.Championships.MatchGroup);
							count = (matchGroup.Rounds == null) ? 0 : matchGroup.Rounds.Count;
							selRound = GetSelectedFilter("round", count, "סיבוב", categoryID);
							if (selRound >= 0)
							{
								Sport.Championships.Round round = matchGroup.Rounds[selRound];
								count = (round.Cycles == null) ? 0 : round.Cycles.Count;
								selCycle = GetSelectedFilter("cycle", count, "מחזור", categoryID);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				return ex.Message;
			}
			return "";
		}
		#endregion

		#region build stuff
		#region Filters
		private string BuildGameFilters(bool blnCompetition, string strCaption,
			Sport.Championships.Championship championship, int selPhase,
			int selGroup, int selRound, int selCycle, int selCompetition, int selTeam)
		{
			//initialize result:
			System.Text.StringBuilder builder = new System.Text.StringBuilder();

			//cells width:
			int[] arrCellWidths = new int[] { 30, 50, 150, 30, 50, 150, 60 };

			//add basic output.
			builder.Append("<table class=\"GameFilters\" cellspacing=\"7\">");

			//phases:
			builder.Append("<tr>");
			builder.Append("<td style=\"width: " + arrCellWidths[0] + "px;\">&nbsp;</td>");
			builder.Append("<td style=\"width: " + arrCellWidths[1] + "px;\">שלב:</td>");
			builder.Append("<td style=\"width: " + arrCellWidths[2] + "px;\">");
			builder.Append(BuildChampionshipFilter(ChampionshipFilter.Phase,
				blnCompetition, championship, "phase", "שלבים", selPhase, -1, -1, -1));
			builder.Append("</td>");

			//groups:
			builder.Append("<td style=\"width: " + arrCellWidths[3] + "px;\">&nbsp;</td>");
			builder.Append("<td style=\"width: " + arrCellWidths[4] + "px;\">בית:</td>");
			builder.Append("<td style=\"width: " + arrCellWidths[5] + "px;\">");
			builder.Append(BuildChampionshipFilter(ChampionshipFilter.Group,
				blnCompetition, championship, "group", "בתים", selGroup, selPhase, -1, -1));
			builder.Append("</td>");
			builder.Append("<td style=\"width: " + arrCellWidths[6] + "px;\">&nbsp;</td></tr>");
			builder.Append("<tr>");

			//rounds or competitions:
			string strFilterCaption = (blnCompetition) ? "מקצוע" : "סיבוב";
			ChampionshipFilter filter = (blnCompetition) ? ChampionshipFilter.Competition :
				ChampionshipFilter.Round;
			string strFilterName = (blnCompetition) ? "competition" : "round";
			string strMany = (blnCompetition) ? "מקצועות" : "סיבובים";
			int selected = (blnCompetition) ? selCompetition : selRound;
			builder.Append("<td style=\"width: " + arrCellWidths[0] + "px;\">&nbsp;</td>");
			builder.Append("<td style=\"width: " + arrCellWidths[1] + "px;\">" + strFilterCaption + ":</td>");
			builder.Append("<td style=\"width: " + arrCellWidths[2] + "px;\">");
			builder.Append(BuildChampionshipFilter(filter, blnCompetition, championship,
				strFilterName, strMany, selected, selPhase, selGroup, -1));
			builder.Append("</td>");

			//cycles or teams:
			strFilterCaption = (blnCompetition) ? "קבוצה" : "מחזור";
			filter = (blnCompetition) ? ChampionshipFilter.Team : ChampionshipFilter.Cycle;
			strFilterName = (blnCompetition) ? "team" : "cycle";
			strMany = (blnCompetition) ? "קבוצות" : "מחזורים";
			selected = (blnCompetition) ? selTeam : selCycle;
			builder.Append("<td style=\"width: " + arrCellWidths[3] + "px;\">&nbsp;</td>");
			builder.Append("<td style=\"width: " + arrCellWidths[4] + "px;\">" + strFilterCaption + ":</td>");
			builder.Append("<td style=\"width: " + arrCellWidths[5] + "px;\">");
			builder.Append(BuildChampionshipFilter(filter, blnCompetition, championship,
				strFilterName, strMany, selected, selPhase, selGroup, ((blnCompetition) ? -1 : selRound)));
			builder.Append("</td>");
			builder.Append("<td style=\"width: " + arrCellWidths[6] + "px;\">&nbsp;</td></tr>");
			builder.Append("</table>");

			//done.
			return builder.ToString();
		}

		private string BuildChampionshipFilter(ChampionshipFilter filter, bool blnCompetition,
			Sport.Championships.Championship championship, string name, string many,
			int selected, int phaseIndex, int groupIndex, int roundIndex)
		{
			//initialize result:
			System.Text.StringBuilder builder = new System.Text.StringBuilder();

			//add the drop down code:
			string strDropDownHTML = "";
			if (blnCompetition && filter != ChampionshipFilter.Phase && filter != ChampionshipFilter.Group)
			{
				strDropDownHTML = String.Format("<select name=\"{0}\" onchange=\"ApplyTabFilters(this);\">", name);
			}
			else
			{
				switch (filter)
				{
					case ChampionshipFilter.Phase:
					case ChampionshipFilter.Group:
					case ChampionshipFilter.Round:
						strDropDownHTML = Tools.BuildAutoSubmitCombo(name);
						break;
					case ChampionshipFilter.Cycle:
					case ChampionshipFilter.Competition:
					case ChampionshipFilter.Team:
						strDropDownHTML = "<select name=\"" + name + "\"";
						if ((filter == ChampionshipFilter.Competition) || (filter == ChampionshipFilter.Team))
							strDropDownHTML += " onchange=\"CompetitionFilterChanged(this);\"";
						strDropDownHTML += ">";
						break;
				}
			}
			builder.Append(strDropDownHTML);

			//default item:
			if (name != "phase")
			{
				string strOptionText = "כל ה" + many;
				if ((filter == ChampionshipFilter.Competition) ||
					(filter == ChampionshipFilter.Team))
				{
					strOptionText = "בחר " +
						((filter == ChampionshipFilter.Competition) ? "מקצוע" : "קבוצה");
				}
				builder.Append(Tools.BuildOption("-1", strOptionText, (selected == -1)));
			}

			bool blnContinue = true;
			switch (filter)
			{
				case ChampionshipFilter.Round:
				case ChampionshipFilter.Competition:
				case ChampionshipFilter.Team:
					if (groupIndex < 0)
						blnContinue = false;
					break;
				case ChampionshipFilter.Cycle:
					if (roundIndex < 0)
						blnContinue = false;
					break;
			}

			//iterate over phases:
			if (blnContinue)
			{
				foreach (Sport.Championships.Phase phase in championship.Phases)
				{
					//phases filter?
					if (phaseIndex < 0)
					{
						builder.Append(Tools.BuildOption(phase.Index.ToString(),
							phase.Name, (selected == phase.Index)));
						continue;
					}

					//selected phase?
					if (phaseIndex != phase.Index)
						continue;

					//iterate over groups:
					foreach (Sport.Championships.Group group in phase.Groups)
					{
						//groups filter?
						if (groupIndex < 0)
						{
							builder.Append(Tools.BuildOption(group.Index.ToString(),
								group.Name, (selected == group.Index)));
							continue;
						}

						//selected group?
						if (groupIndex != group.Index)
							continue;

						//iterate over rounds or competitions:
						if (blnCompetition)
						{
							Sport.Championships.CompetitionGroup compGroup =
								(Sport.Championships.CompetitionGroup)group;
							if (filter == ChampionshipFilter.Competition)
							{
								foreach (Sport.Championships.Competition competition in compGroup.Competitions)
								{
									builder.Append(Tools.BuildOption(competition.Index.ToString(),
										competition.Name, (selected == competition.Index)));
								}
							}
							else if (filter == ChampionshipFilter.Team)
							{
								foreach (Sport.Championships.CompetitionTeam team in compGroup.Teams)
								{
									if (team.TeamEntity == null)
										continue;
									string strTeamName = team.TeamEntity.TeamName();
									builder.Append(Tools.BuildOption(team.Index.ToString(),
										strTeamName, (selected == team.Index)));
								}
							}
						}
						else
						{
							Sport.Championships.MatchGroup matchGroup =
								(Sport.Championships.MatchGroup)group;
							foreach (Sport.Championships.Round round in matchGroup.Rounds)
							{
								//rounds filter?
								if (roundIndex < 0)
								{
									builder.Append(Tools.BuildOption(round.Index.ToString(),
										round.Name, (selected == round.Index)));
									continue;
								}

								//selected group?
								if (roundIndex != round.Index)
									continue;

								//iterate over cycles:
								foreach (Sport.Championships.Cycle cycle in round.Cycles)
								{
									builder.Append(Tools.BuildOption(cycle.Index.ToString(),
										cycle.Name, (selected == cycle.Index)));
								}
							} //end loop over rounds.
						} //end if competition type.
					} //end loop over groups
				} //end loop over phases
			}

			//done.
			builder.Append("</select>");
			return builder.ToString();
		}
		#endregion

		#region competition reports
		private string BuildCompetitionReport(Sport.Championships.CompetitionGroup group,
			Sport.Championships.Competition competition,
			Sport.Championships.CompetitionTeam team)
		{
			//initialize result:
			System.Text.StringBuilder builder = new System.Text.StringBuilder();

			//got any report?
			if (!String.IsNullOrEmpty(Request.QueryString["report"]))
			{
				//what type of report we have?
				string strReport = Request.QueryString["report"];
				Sport.Documents.Data.Table[] tables = null;
				int totalScore = 0;
				switch (strReport)
				{
					case "team":
						tables = new Sport.Documents.Data.Table[] { group.GetTeamsTable() };
						break;
					case "comp":
						tables = new Sport.Documents.Data.Table[] { competition.GetCompetitorsTable() };
						break;
					case "full":
						tables = team.GetFullReportTables(ref totalScore);
						break;
				}

				if (tables != null && tables.Length > 0 && tables[0].Rows.Length > 0)
				{
					builder.Append(Tools.BuildChampTable(tables));
					if (totalScore > 0)
						builder.Append("<br />סך הכל נקודות: " + totalScore);
				}
				else
				{
					builder.Append("אין נתונים בשלב זה, אנא נסה במועד מאוחר יותר");
				}

				return builder.ToString();
			}

			builder.Append("$tab_control_data");

			//done.
			return builder.ToString();
		}

		private string GetTabControlRawHTML()
		{
			string strBaseURL = "/Results.aspx?report={r}";
			foreach (string key in Request.QueryString.Keys)
			{
				if (key.ToLower() != "report")
				{
					strBaseURL += "&" + key + "=" + Request.QueryString[key];
				}
			}

			TabControl tabControl = (TabControl)Page.LoadControl("~/Controls/TabControl.ascx");
			tabControl.LoadingText = "טוען נתונים אנא המתן...";
			tabControl.ElementsToPost = "phase,group,competition,team";
			tabControl.AddTab("דירוג קבוצתי לתחרות", strBaseURL.Replace("{r}", "team"), "אנא בחר בית");
			tabControl.AddTab("דירוג אישי למקצוע", strBaseURL.Replace("{r}", "comp"), "אנא בחר מקצוע");
			tabControl.AddTab("סיכום קבוצתי מפורט", strBaseURL.Replace("{r}", "full"), "אנא בחר קבוצה");
			tabControl.SetTabs();
			StringBuilder sb = new StringBuilder();
			StringWriter stringWriter = new StringWriter(sb);
			HtmlTextWriter htmlWriter = new HtmlTextWriter(stringWriter);
			tabControl.RenderControl(htmlWriter);

			IsfMainView.clientSide.AddOnloadCommand("ApplyTabFilters(); ChangeTab(0);", false, true);

			return "<div>" + sb.ToString() + "</div>";
		}
		#endregion

		#region teams
		private string BuildChampionshipTeams(Sport.Championships.Phase phase)
		{
			//got any groups?
			if ((phase.Groups == null) || (phase.Groups.Count == 0))
				return "אין קבוצות רשומות בשלב אליפות זה";

			//initialize result:
			System.Text.StringBuilder builder = new System.Text.StringBuilder();

			//iterate over groups:
			int teamsCount = 0;
			foreach (Sport.Championships.Group group in phase.Groups)
			{
				//got any teams?
				if ((group.Teams == null) || (group.Teams.Count == 0))
					continue;

				//get name:
				string strGroupName = "בית " + group.Name.Replace("בית", "");

				//caption:
				builder.Append(Tools.BuildPageSubCaption(strGroupName,
					this.Server, IsfMainView.clientSide));

				//iterate over teams:
				builder.Append("<ul style=\"color: #CC0000;\">");
				string strBaseLink = "<a href=\"?team=%id";
				if (group is Sport.Championships.CompetitionGroup)
					strBaseLink += "&phase=" + phase.Index + "&group=" + group.Index;
				strBaseLink += "\" class=\"text_link\" " +
					"style=\"font-weight: normal;\">";
				foreach (Sport.Championships.Team team in group.Teams)
				{
					int ID = (team.TeamEntity == null) ? -1 : team.TeamEntity.Id;
					builder.Append("<li>");
					if (ID >= 0)
						builder.Append(strBaseLink.Replace("%id", ID.ToString()));
					builder.Append(team.Name);
					if (ID >= 0)
						builder.Append("</a>");
					builder.Append("</li>");
				}
				builder.Append("</ul>");

				//add to counter:
				teamsCount += group.Teams.Count;
			} //end loop over groups

			//done.
			return builder.ToString();
		}
		#endregion

		#region attachments
		private string BuildChampionshipAttachments(
			Sport.Entities.ChampionshipCategory category)
		{
			//get all attachments:
			Core.AttachmentData[] champAttachments = Core.AttachmentManager.GetChampionshipAttachments(
				this.Server, category.Id);

			//got anything?
			if ((champAttachments == null) || (champAttachments.Length == 0))
				return "לאליפות זו אין קבצים  מצורפים";

			//initialize result:
			System.Text.StringBuilder builder = new System.Text.StringBuilder();

			//add HTML.
			builder.Append(BuildSingleDetail("קבצים מצורפים:", "", null, true, champAttachments));
			builder.Append("<br />");

			//done.
			return builder.ToString();
		}
		#endregion

		#region details
		private string BuildChampionshipDetails(string strSection,
			Sport.Entities.ChampionshipCategory category,
			Sport.Entities.Championship championship, ref string sectionName)
		{
			//competition?
			bool blnCompetition = (championship.Sport.SportType == Sport.Types.SportType.Competition);

			//find name of the section:
			sectionName = "&nbsp;";
			string[] arrItemsText = (blnCompetition) ?
				Data.CompetitionChampMenuItemsText : Data.MatchChampMemuItemsText;
			for (int i = 0; i < Data.ChampMemuItemsSections.Length; i++)
			{
				if (Data.ChampMemuItemsSections[i].ToLower() == strSection.ToLower())
				{
					sectionName = arrItemsText[i];
					break;
				}
			}

			//get logo:
			string strLogo = Tools.GetChampionshipLogo(championship.Id, this.Server);

			//build table:
			string result = Tools.BuildDetailsTable(strLogo, sectionName, this.Server,
				this.Page, category.Id);

			//done.
			return result;
		}
		#endregion

		#region games
		private string BuildChampionshipGames(
			Sport.Championships.MatchChampionship championship, int selPhase, int selGroup,
			int selRound, int selCycle)
		{
			//initialize result:
			System.Text.StringBuilder builder = new System.Text.StringBuilder();

			//get all matches:
			Sport.Championships.Match[] matches = GetChampionshipMatches(
				championship, selPhase, selGroup, selRound, selCycle);

			//got anything?
			if ((matches == null) || (matches.Length == 0))
			{
				builder.Append("אין משחקים, אנא נסו שנית במועד מאוחר יותר.");
			}
			else
			{
				builder.Append(BuildMatchesTable(new ArrayList(matches)));
			}

			//done.
			return builder.ToString();
		}
		#endregion

		#region ranking table
		private string BuildRankingTable(Sport.Championships.Phase phase)
		{
			//got anything?
			if ((phase.Groups == null) || (phase.Groups.Count == 0))
				return "שלב אליפות זה לא מכיל משחקים";

			//initialize result:
			System.Text.StringBuilder builder = new System.Text.StringBuilder();

			//add proper HTML to the output.
			//builder.Append("<div id=\"ranking_table\">");

			//initialize captions and rows:
			ArrayList captions = new ArrayList();
			ArrayList rows = new ArrayList();

			//add the default captions.
			captions.Add("מיקום");
			captions.Add("קבוצה");

			//get rank fields from database:
			Sport.Rulesets.Rules.RankingTables rankingTables = (Sport.Rulesets.Rules.RankingTables)
				phase.Championship.ChampionshipCategory.GetRule(typeof(Sport.Rulesets.Rules.RankingTables));
			Sport.Rulesets.Rules.RankingTable rankingTable = rankingTables.DefaultRankingTable;

			//add captions if got anything:
			if ((rankingTable != null) && (rankingTable.Fields != null) && (rankingTable.Fields.Count > 0))
			{
				foreach (Sport.Rulesets.Rules.RankField rankField in rankingTable.Fields)
					captions.Add(rankField.Title);
			}

			//iterate over groups in selected phase:
			int colCount = captions.Count;
			foreach (Sport.Championships.Group group in phase.Groups)
			{
				//add title:
				rows.Add(new string[] { "<b>" + group.Name + "</b>" });

				//sort group teams by position:
				Sport.Championships.Team[] teams = new Sport.Championships.Team[group.Teams.Count];
				for (int i = 0; i < group.Teams.Count; i++)
					teams[i] = group.Teams[i];
				Array.Sort(teams, 0, teams.Length, new Sport.Championships.TeamPositionComparer());

				//iterate over teams:
				for (int pos = 0; pos < teams.Length; pos++)
				{
					Sport.Championships.Team team = teams[pos];
					ArrayList cells = new ArrayList();
					cells.Add((pos + 1).ToString());
					cells.Add(team.Name);
					if ((rankingTable != null) && (rankingTable.Fields != null) && (rankingTable.Fields.Count > 0))
					{
						for (int i = 0; i < rankingTable.Fields.Count; i++)
						{
							Sport.Common.EquationVariables var =
								new Sport.Common.EquationVariables();
							team.SetFields(var);
							string strText = rankingTable.Fields[i].Evaluate(var);
							cells.Add(strText);
						}
					}
					rows.Add((string[])cells.ToArray(typeof(string)));
				} //end loop over teams.
			} //end loop over groups

			//build table from the rows:
			builder.Append(Tools.BuildChampTable(
				(string[])captions.ToArray(typeof(string)), rows));

			//done.
			//builder.Append("</div>");
			//builder.Append(Tools.BuildVisitorActionsPanel(
			//	"ranking_table", this.Server, IsfMainView.clientSide));
			return builder.ToString();
		}
		#endregion

		#region general
		private string BuildChampionshipStatus(Sport.Entities.Championship champ)
		{
			string result = "";

			//regional or global?
			if (champ.Region.Id == Sport.Entities.Region.CentralRegion)
				result += "ארצית";
			else
				result += "מחוזית (מחוז " + champ.Region.Name + ")";

			//open or closed?
			result += (champ.IsOpen) ? ", פתוחה" : ", סגורה";

			//clubs?
			if (champ.IsClubs)
				result += ", מועדונים בלבד";

			return result;
		}
		#endregion
		#endregion
		#endregion

		#region helper functions
		private int GetSelectedFilter(string name, int count, string caption,
			int categoryID)
		{
			/* if (count <= 0)
				throw new Exception("ה"+caption+" שבחרת לא כולל תחרויות או משחקים, אנא נסה במועד מאוחר יותר."); */

			if (count == 1)
				return 0;

			int selValue = -1;
			string strFormValue = Request.Form[name];
			if ((strFormValue != null) && (strFormValue != "-1"))
			{
				selValue = Tools.CIntDef(strFormValue, -1);
				if ((selValue < 0) || (selValue >= count))
					throw new Exception("אינדקס " + caption + " לא חוקי");
			}
			else
			{
				if (name == "phase")
				{
					//DataServices.DataService service = new DataServices.DataService();
					selValue = DB.Instance.GetChampionshipCurrentPhase(categoryID); //service.GetChampionshipCurrentPhase(categoryID);
					if (selValue < 0)
						selValue = 0;
				}
			}

			return selValue;
		}

		private string SortTeamsByGroup(string strTeamsHTML)
		{
			string[] arrTemp = strTeamsHTML.Split(new char[] { '\n' });
			if (arrTemp.Length <= 2)
				return strTeamsHTML;
			ArrayList arrTeams = new ArrayList();
			for (int i = 1; i < arrTemp.Length - 1; i++)
				arrTeams.Add(arrTemp[i]);
			arrTeams.Sort(new TeamGroupComparer());
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append(arrTemp[0]);
			for (int i = 0; i < arrTeams.Count; i++)
				sb.Append(arrTeams[i].ToString());
			sb.Append(arrTemp[arrTemp.Length - 1]);
			return sb.ToString();
		}
		#endregion

		#region Show Details
		#region team
		private void ShowTeamDetails(int teamID)
		{
			//build team entity:
			Sport.Entities.Team team = null;
			try
			{
				team = new Sport.Entities.Team(teamID);
			}
			catch
			{ }
			if (team == null || team.Id < 0)
			{
				AddViewError("זיהוי קבוצה שגוי לא ניתן להציג פרטים");
				return;
			}

			//school:
			Sport.Entities.School school = team.School;
			if (school == null || school.Id < 0)
			{
				AddViewError("תקלה במציאת בית ספר המשויך לקבוצה, לא ניתן להציג פרטים");
				return;
			}

			//category:
			Sport.Entities.ChampionshipCategory champCat = team.Category;
			int categoryId = (champCat == null) ? -1 : champCat.Id;
			if (categoryId < 0)
			{
				AddViewError("תקלה במציאת אליפות משויכת לקבוצה, לא ניתן להציג פרטים");
				return;
			}

			//team name:
			string strTeamName = team.TeamName();

			//initialize HTML output:
			System.Text.StringBuilder sb = new System.Text.StringBuilder();

			//initialize service:
			//DataServices.DataService _service=new DataServices.DataService();

			//name:
			IsfMainView.SetPageCaption(strTeamName);

			//competition?
			bool blnCompetition =
				(team.Championship.Sport.SportType == Sport.Types.SportType.Competition);

			//championship:
			string strChamp = team.Championship.Name + " " + team.Category.Name;

			//generate HTML.
			sb.Append("<div id=\"team_data\" dir=\"rtl\">");
			sb.Append("<div class=\"printable\">פרטים עבור הקבוצה " + strTeamName + "</div>");

			//decide what contents to show.
			if (blnCompetition)
			{
				//load championship:
				Sport.Championships.Championship championship =
					Sport.Championships.Championship.GetChampionship(team.Category.Id);

				//get phase and group:
				int selPhase = Tools.CIntDef(Request.QueryString["phase"], -1);
				int selGroup = Tools.CIntDef(Request.QueryString["group"], -1);

				//build list of competitors
				Hashtable tblTeamCompetitors = new Hashtable();
				foreach (Sport.Championships.Phase phase in championship.Phases)
				{
					if ((phase.Groups == null) || ((selPhase >= 0) && (selPhase != phase.Index)))
						continue;
					foreach (Sport.Championships.CompetitionGroup group in phase.Groups)
					{
						if ((group.Competitions == null) || ((selGroup >= 0) && (selGroup != group.Index)))
							continue;
						foreach (Sport.Championships.Competition competition in group.Competitions)
						{
							if (competition.Competitors == null)
								continue;
							foreach (Sport.Championships.Competitor competitor in competition.Competitors)
							{
								if (competitor.Player == null)
									continue;
								Sport.Entities.Player player = competitor.Player.PlayerEntity;
								if ((player == null) || (player.Team == null))
									continue;
								if (player.Team.Id != team.Id)
									continue;
								if (tblTeamCompetitors[competition] == null)
									tblTeamCompetitors[competition] = new ArrayList();
								(tblTeamCompetitors[competition] as ArrayList).Add(competitor);
							}
						} //end loop over group competitions
						if (tblTeamCompetitors.Count > 0)
							break;
					} //end loop over phase groups
					if (tblTeamCompetitors.Count > 0)
						break;
				} //end loop over phases.

				//got anything?
				if (tblTeamCompetitors.Count == 0)
				{
					sb.Append("קבוצה זו לא משתפת מתמודדים בבית אליפות זה");
				}
				else
				{
					foreach (Sport.Championships.Competition competition in tblTeamCompetitors.Keys)
					{
						Sport.Rulesets.Rules.ResultType resultType =
							competition.ResultType;
						sb.Append(Tools.BuildPageSubCaption(competition.Name,
							this.Server, IsfMainView.clientSide));
						ArrayList arrCompetitors = (ArrayList)tblTeamCompetitors[competition];
						sb.Append("<ol>");
						foreach (Sport.Championships.Competitor competitor in arrCompetitors)
						{
							Sport.Entities.Player player = competitor.Player.PlayerEntity;
							int playerID = player.Id;
							string strName = player.FirstName + " " + player.LastName;
							int result = competitor.Result;
							int score = competitor.Score;
							sb.Append("<li>");
							sb.Append("<a href=\"?player=" + playerID + "\">");
							sb.Append(strName);
							sb.Append("</a>");
							if ((result > 0) && (resultType != null))
								sb.Append("&nbsp;&nbsp;&nbsp;תוצאה: " + resultType.FormatResult(result));
							if (score > 0)
								sb.Append("&nbsp;&nbsp;&nbsp;ניקוד: " + score);
							sb.Append("</li>");
						}
						sb.Append("</ol>");
					}
				}
			}
			else
			{
				//players:
				//Sport.Entities.Player[] players = team.GetPlayers();

				//matches:
				//DataServices.MatchData[] matches = _service.GetMatchesByTeam(team.Id);
				Sport.Common.MatchData[] matches = DB.Instance.GetMatchesByTeam(team.Id);

				//details:
				string strBaseLink = "<a href=\"%href\">%text</a>";
				string section = Data.ChampMemuItemsSections[0];
				string[] arrCaptions = new string[] { "עונה", "אליפות", "בית ספר" }; //, "מספר שחקנים" };
				string[] arrTexts = new string[] {
					team.Championship.Season.Name, 
					strBaseLink.Replace("%href", string.Format("?category={0}&section={1}", categoryId, section)).Replace("%text", strChamp), 
					strBaseLink.Replace("%href", "?school="+school.Id).Replace("%text", school.Name)
				}; //, players.Length.ToString() };
				string strSpecial = "";
				int winCount = 0;
				int loseCount = 0;
				int drawCount = 0;
				matches.ToList().ForEach(match =>
				{
					Sport.Championships.MatchOutcome outcome = (Sport.Championships.MatchOutcome)match.Result;
					switch (outcome)
					{
						case Sport.Championships.MatchOutcome.WinA:
						case Sport.Championships.MatchOutcome.TechnicalA:
							winCount += (match.TeamA.ID == team.Id) ? 1 : 0;
							loseCount += (match.TeamA.ID == team.Id) ? 0 : 1;
							break;
						case Sport.Championships.MatchOutcome.WinB:
						case Sport.Championships.MatchOutcome.TechnicalB:
							winCount += (match.TeamB.ID == team.Id) ? 1 : 0;
							loseCount += (match.TeamB.ID == team.Id) ? 0 : 1;
							break;
						case Sport.Championships.MatchOutcome.Tie:
							drawCount++;
							break;
					}
				});
				if (winCount > 0 || loseCount > 0 || drawCount > 0)
				{
					strSpecial = "קבוצה זו ";
					string strWinCount = Sport.Common.Tools.BuildOneOrMany(
						"משחק", "משחקים", winCount, true);
					string strLoseCount = Sport.Common.Tools.BuildOneOrMany(
						"משחק", "משחקים", loseCount, true);
					string strDrawCount = Sport.Common.Tools.BuildOneOrMany(
						"תוצאת שיוויון", "תוצאות שיוויון", drawCount, false);
					string strWin = "";
					string strLose = "";
					string strDraw = "";
					if (winCount > 0)
						strWin = "ניצחה ב" + strWinCount;
					if (loseCount > 0)
						strLose = "הפסידה ב" + strLoseCount;
					if (drawCount > 0)
						strDraw = "השיגה " + strDrawCount;
					if (strWin.Length > 0)
						strSpecial += " " + strWin;
					if (strLose.Length > 0)
					{
						if (strWin.Length > 0)
							strSpecial += (strDraw.Length == 0) ? " ו" : ", ";
						strSpecial += strLose;
					}
					if (strDraw.Length > 0)
					{
						if ((strWin.Length > 0) || (strLose.Length > 0))
							strSpecial += " ו";
						strSpecial += strDraw;
					}
				}
				sb.Append(Tools.BuildDetailsTable(arrCaptions, arrTexts, strSpecial));

				//players:
				//sb.Append(BuildSingleDetail("שחקנים הרשומים עבור קבוצה זו:"));
				//sb.Append(BuildPlayersTable(players));

				//matches:
				sb.Append(BuildSingleDetail("משחקים בהם משתתפת קבוצה זו:"));
				sb.Append(BuildMatchesTable(new ArrayList(matches)));
			}

			//done.
			sb.Append("</div>");
			sb.Append(Tools.BuildVisitorActionsPanel("team_data", IsfMainView.clientSide));
			AddViewText(sb.ToString(), true);
		}

		private string BuildPlayersTable(Sport.Entities.Player[] players)
		{
			//SessionService.SessionService _service=
			//	new SessionService.SessionService();
			//Sport.Core.Session.Season = _service.GetCurrentSeason().Season;
			if ((players == null) || (players.Length == 0))
				return "אין שחקנים רשומים<br />";
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			ArrayList arrPlayers = new ArrayList(players);
			arrPlayers.Sort(new PlayerGradeComparer());
			sb.Append("<ol>");
			Sport.Types.GradeTypeLookup lookup = new Sport.Types.GradeTypeLookup(true);
			foreach (Sport.Entities.Player player in arrPlayers)
			{
				sb.Append("<li><a href=\"?player=" + player.Id + "\" " +
					"class=\"text_link\" style=\"font-weight: normal;\">");
				sb.Append(player.Student.FirstName);
				if (player.Student.LastName.Length > 0)
					sb.Append(" " + player.Student.LastName);
				sb.Append(" (כיתה " + lookup.Lookup(player.Student.Grade) + ")");
				sb.Append("</a></li>");
			}
			sb.Append("</ol>");
			return sb.ToString();
		}
		#endregion

		#region school
		private void ShowSchoolDetails(int schoolID)
		{
			Sport.Entities.School school = null;
			try
			{
				school = new Sport.Entities.School(schoolID);
			}
			catch
			{ }
			if (school == null)
			{
				AddViewError("זיהוי בית ספר לא חוקי לא ניתן להציג פרטים");
				return;
			}
			
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			IsfMainView.SetPageCaption("פרטי בית ספר");
			string strSchoolName = school.Name;
			string strAddress = school.Address + "";
			bool isClub = (school.ClubStatus == 1);
			string strFax = school.Fax + "";
			string strManager = school.ManagerName + "";
			string strPhone = school.Phone + "";
			string strZipCode = school.ZipCode + "";
			string[] arrCaptions = new string[] {"שם בית הספר", "כתובת", "מיקוד", "טלפון", 
									"פקס", "שם מנהל", "סמל בית ספר"};
			string[] arrTexts = new string[] {strSchoolName,  strAddress, strZipCode, 
									strPhone, strFax, strManager, school.Symbol};
			string strClub = (isClub) ? "בית ספר זה הינו מועדון" : "";
			sb.Append(Tools.BuildDetailsTable(arrCaptions, arrTexts, strClub));
			sb.Append("<br />");
			Sport.Data.EntityFilter filter = new Sport.Data.EntityFilter(
				new Sport.Data.EntityFilterField((int)Sport.Entities.Team.Fields.School, school.Id));
			int curSeason = Tools.CIntDef(System.Web.HttpContext.Current.Session["season"], -1);
			if (curSeason < 0)
				curSeason = Common.Tools.GetLatestSeason();
			List<Sport.Data.Entity> teams = new List<Sport.Data.Entity>(Sport.Entities.Team.Type.GetEntities(filter));
			teams.RemoveAll(curTeam =>
			{
				Sport.Entities.Championship champ = new Sport.Entities.Championship((int)curTeam.Fields[(int)Sport.Entities.Team.Fields.Championship]);
				return (champ.Season == null || champ.Season.Id != curSeason);
			});
			
			if (teams.Count > 0)
			{
				sb.Append(Tools.BuildPageSubCaption("קבוצות", this.Server, IsfMainView.clientSide));
				sb.Append("<br />");
				sb.Append("<ul class=\"OrdinaryList\">");
				teams.ForEach(ent =>
				{
					Sport.Entities.Team team = new Sport.Entities.Team(ent);
					if (team != null && team.Id >= 0 && team.Championship != null &&
						team.Category != null)
					{
						string champName = team.Championship.Name + " " + team.Category.Name;
						sb.AppendFormat("<li><a href=\"?team={0}\">{1} ({2} - {3})</a></li>",
							team.Id, team.TeamName(), champName, team.Championship.Season.Name);
					}
				});
				sb.Append("</ul>");
			}
			IsfMainView.AddContents(sb.ToString());

			//add control:
			Tools.LoadBanner(this, IsfMainView, BannerType.Advertisement_Secondary);
		}
		#endregion

		#region player
		private void ShowPlayerDetails(int playerID)
		{
			Sport.Entities.Player player = null;
			try
			{
				player = new Sport.Entities.Player(playerID);
			}
			catch
			{ }
			if (player == null)
			{
				AddViewError("זיהוי שחקן לא חוקי לא ניתן להציג פרטים");
				return;
			}
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			IsfMainView.SetPageCaption("פרטי שחקן");
			Sport.Entities.Team team = player.Team;
			//Sport.Entities.Student student = player.Student;
			Sport.Entities.Championship championship = null;
			if (team != null)
				championship = team.Championship;
			if (team == null || championship == null) //(student == null)
			{
				AddViewError("שגיאה כללית, לא ניתן לקרוא נתוני שחקן זה");
				return;
			}
			string strTeam = team.TeamName() + " (" + championship.Name + " " + team.Category.Name + ")";
			//string strStudent = student.FirstName + " " + student.LastName;
			string shirtNumber = (player.Number <= 0) ? "" : player.Number.ToString();
			/* removed 29/09/2013 - do not show birth days anymore
			string strBirthDate = "";
			if (student.BirthDate.Year > 1900)
				strBirthDate = student.BirthDate.ToString("dd/MM/yyyy"); */
			string[] arrCaptions = new string[] { "קבוצה", "מספר חולצה" }; // "שם מלא", "תאריך לידה"
			string[] arrTexts = new string[] {
				string.Format("<a href=\"?team={0}\">{1}</a>", team.Id, strTeam),
				shirtNumber
			}; //strStudent, strBirthDate
			sb.Append(Tools.BuildDetailsTable(arrCaptions, arrTexts));
			sb.Append(Tools.MultiString("<br />", 5));

			IsfMainView.AddContents(sb.ToString());

			//add control:
			Tools.LoadBanner(this, IsfMainView, BannerType.Advertisement_Secondary);
		}
		#endregion

		#region Facility
		private void ShowFacilityDetails(int facilityID)
		{
			Sport.Entities.Facility facility = null;
			try
			{
				facility = new Sport.Entities.Facility(facilityID);
			}
			catch
			{ }
			if (facility == null)
			{
				AddViewError("זיהוי מתקן שגוי לא ניתן להציג פרטים");
				return;
			}

			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append("<div dir=\"rtl\">");
			IsfMainView.SetPageCaption("פרטי מתקן");

			string strAddress = Tools.CStrDef(facility.Address, "");
			string strFax = Tools.CStrDef(facility.Fax, "");
			string strPhone = Tools.CStrDef(facility.Phone, "");
			string[] arrCaptions = new string[] { "שם המתקן", "כתובת", "טלפון", "פקס" };
			string[] arrTexts = new string[] { facility.Name, strAddress, strPhone, strFax };
			sb.Append(Tools.BuildDetailsTable(arrCaptions, arrTexts) + "<br />");

			Sport.Entities.Court[] courts = facility.GetCourts();
			if (courts.Length > 0)
			{
				sb.Append("<br /><fieldset><legend>מגרשים</legend>");
				sb.Append("<ol dir=\"rtl\">");
				foreach (Sport.Entities.Court court in courts)
				{
					sb.Append("<li><a href=\"?court=" + court.Id + "\" class=\"text_link\">");
					sb.Append(court.Name + "</a></li>");
				}
				sb.Append("</ol></fieldset>");
			}

			//DataServices.DataService _service = new DataServices.DataService();
			//DataServices.MatchData[] matches = _service.GetMatchesByFacility(facility.Id);
			Sport.Common.MatchData[] matches = DB.Instance.GetMatchesByFacility(facility.Id);
			if (matches.Length > 0)
			{
				sb.Append(BuildSingleDetail("משחקים במתקן זה:"));
				sb.Append(BuildMatchesTable(new ArrayList(matches)));
			}
			sb.Append("</div><br /><br /><br />");
			AddViewText(sb.ToString(), true);

			//add control:
			Tools.LoadBanner(this, IsfMainView, BannerType.Advertisement_Main);
		}
		#endregion

		#region Court
		private void ShowCourtDetails(int courtID)
		{
			Sport.Entities.Court court = null;
			try
			{
				court = new Sport.Entities.Court(courtID);
			}
			catch
			{ }
			if (court == null)
			{
				AddViewError("זיהוי מגרש שגוי לא ניתן להציג פרטים");
				return;
			}

			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append("<div dir=\"rtl\">");
			IsfMainView.SetPageCaption("פרטי מגרש");

			Sport.Entities.Facility facility = court.Facility;
			string strFacility = "";
			if (facility != null)
				strFacility = "<a href=\"?facility=" + facility.Id + "\">" + facility.Name + "</a>";
			string strCourtType = "";
			if (court.CourtType != null)
				strCourtType = court.CourtType.Name;
			string[] arrCaptions = new string[] { "שם מגרש", "מתקן", "סוג מגרש" };
			string[] arrTexts = new string[] { court.Name, strFacility, strCourtType };
			sb.Append(Tools.BuildDetailsTable(arrCaptions, arrTexts) + "<br />");

			//DataServices.DataService _service = new DataServices.DataService();
			//DataServices.MatchData[] matches = _service.GetMatchesByCourt(court.Id);
			Sport.Common.MatchData[] matches = DB.Instance.GetMatchesByCourt(court.Id);
			if (matches.Length > 0)
			{
				sb.Append(BuildSingleDetail("משחקים במגרש זה:"));
				sb.Append(BuildMatchesTable(new ArrayList(matches)));
			}
			sb.Append("</div><br /><br /><br /><br /><br />");
			AddViewText(sb.ToString(), true);

			//add control:
			Tools.LoadBanner(this, IsfMainView, BannerType.Advertisement_Main);
		}
		#endregion
		#endregion

		#region Matches Table
		private string BuildMatchesTable(ArrayList matches)
		{
			matches.Sort(new MatchComparer(this.Response));
			string strBaseLink = "?category=%1&phase=%2&group=%3&round=%4&cycle=%5&match=%6";
			string[] arrCaptions = new string[] {"מס'", "קבוצה א'", "קבוצה ב'", 
				"תאריך משחק", "יום", "מתקן", "אחראי", "תוצאה"};
			string lastPhase = "";
			string lastGroup = "";
			string lastRound = "";
			string lastCycle = "";
			int lastChampCategory = -1;
			ArrayList rows = new ArrayList();
			Hashtable tblColors = new Hashtable();
			bool blnGotDateChange = false;
			for (int i = 0; i < matches.Count; i++)
			{
				string curPhase = "";
				string curGroup = "";
				string curRound = "";
				string curCycle = "";
				int curChampCategory = -1;
				bool blnDateChanged = false;
				string[] arrMatchCells = BuildMatchRow(matches[i], strBaseLink, i,
					ref curPhase, ref curGroup, ref curRound, ref curCycle,
					ref curChampCategory, ref blnDateChanged);
				if (!curGroup.StartsWith("בית"))
					curGroup = "בית " + curGroup;
				if (!curRound.StartsWith("סיבוב"))
					curRound = "סיבוב " + curRound;
				if (!curCycle.StartsWith("מחזור"))
					curCycle = "מחזור " + curCycle;
				if ((lastPhase.Length == 0) || (lastChampCategory != curChampCategory) ||
					(lastPhase != curPhase) || (lastGroup != curGroup) ||
					(lastRound != curRound) || (lastCycle != curCycle))
				{
					Sport.Entities.ChampionshipCategory category =
						new Sport.Entities.ChampionshipCategory(curChampCategory);
					string strChampName = category.Championship.Name + " " + category.Name;
					string strCurCaption = "";
					if ((lastGroup.Length == 0) || (lastPhase != curPhase))
						strCurCaption += curPhase;
					if (strCurCaption.Length > 0)
						strCurCaption += ", ";
					strCurCaption += curGroup;
					if ((lastGroup.Length == 0) || (lastRound != curRound))
					{
						if (strCurCaption.Length > 0)
							strCurCaption += ", ";
						strCurCaption += curRound;
					}
					if (strCurCaption.Length > 0)
						strCurCaption += ", ";
					strCurCaption += curCycle;
					if (lastChampCategory != curChampCategory)
						strCurCaption += " (" + strChampName + ")";
					rows.Add(new string[] { "<b>" + strCurCaption + "</b>" });
				}
				rows.Add(arrMatchCells);
				if (blnDateChanged)
				{
					int index = rows.Count - 1;
					if (tblColors[index] == null)
						tblColors[index] = new Hashtable();
					(tblColors[index] as Hashtable)[3] = Color.FromArgb(255, 0, 0);
					blnGotDateChange = true;
				}
				lastPhase = curPhase;
				lastGroup = curGroup;
				lastRound = curRound;
				lastCycle = curCycle;
				lastChampCategory = curChampCategory;
			}
			System.Text.StringBuilder strHTML = new System.Text.StringBuilder();
			strHTML.Append(Tools.BuildChampTable(arrCaptions, rows,
				new ArrayList(new int[] { arrCaptions.Length - 1 }), tblColors));
			if (blnGotDateChange)
				strHTML.Append("<br />* צבע רקע אדום - תאריך או שעת קיום משחק זה עברו שינוי בשבוע האחרון");
			return strHTML.ToString();
		}

		private string[] BuildMatchRow(object objMatch, string strBaseLink, int row,
			ref string phaseName, ref string groupName, ref string roundName,
			ref string cycleName, ref int champCategory, ref bool blnDateChanged)
		{
			//define all variables:
			ArrayList result = new ArrayList();
			Sport.Entities.Facility facility = null;
			Sport.Entities.Court court = null;
			Sport.Championships.MatchOutcome outcome = Sport.Championships.MatchOutcome.None;
			string partsResult = "";
			double score1 = 0;
			double score2 = 0;
			Sport.Entities.Team teamA = null;
			Sport.Entities.Team teamB = null;
			int categoryID = -1;
			int phaseIndex = -1;
			int groupIndex = -1;
			int roundIndex = -1;
			int matchIndex = -1;
			int cycleIndex = -1;
			int gameNumber = -1;
			phaseName = "";
			groupName = "";
			roundName = "";
			cycleName = "";
			champCategory = -1;
			DateTime matchTime = DateTime.MinValue;
			string strTeamA_name = "";
			string strTeamB_name = "";
			int teamA_id = -1;
			int teamB_id = -1;
			Sport.Entities.Functionary supervisor = null;
			DateTime dateChangedDate = DateTime.MinValue;

			string strDebug = "";
			if (objMatch is Sport.Championships.Match)
			{
				strDebug += "Sport.Championships.Match<br />";

				Sport.Championships.Match match = (Sport.Championships.Match)objMatch;
				try
				{
					facility = match.Facility;
					court = match.Court;
				}
				catch
				{ }
				outcome = match.Outcome;
				partsResult = Tools.CStrDef(match.PartsResult, "");
				score1 = match.TeamAScore;
				score2 = match.TeamBScore;
				if (match.GroupTeamA != null)
					teamA = match.GroupTeamA.TeamEntity;
				if (match.GroupTeamB != null)
					teamB = match.GroupTeamB.TeamEntity;
				if (teamA != null)
				{
					strTeamA_name = teamA.TeamName();
					strDebug += "Team A is not null: " + strTeamA_name + "<br />";
				}
				else
				{
					strDebug += "Team A is null. using match to get name<br />";
					strTeamA_name = match.GetTeamAName();
				}
				if (teamB != null)
				{
					strTeamB_name = teamB.TeamName();
					strDebug += "Team B is not null: " + strTeamB_name + "<br />";
				}
				else
				{
					strDebug += "Team B is null. using match to get name<br />";
					strTeamB_name = match.GetTeamBName();
				}
				if (teamA != null)
					teamA_id = teamA.Id;
				if (teamB != null)
					teamB_id = teamB.Id;
				strDebug += "Team A id: " + teamA_id + ", team B id: " + teamB_id + "<br />";
				categoryID = match.Cycle.Round.Group.Phase.Championship.ChampionshipCategory.Id;
				phaseIndex = match.Cycle.Round.Group.Phase.Index;
				groupIndex = match.Cycle.Round.Group.Index;
				phaseName = match.Cycle.Round.Group.Phase.Name;
				groupName = match.Cycle.Round.Group.Name;
				roundIndex = match.Cycle.Round.Index;
				roundName = match.Cycle.Round.Name;
				cycleIndex = match.Cycle.Index;
				cycleName = match.Cycle.Name;
				matchIndex = match.Index;
				matchTime = match.Time;
				gameNumber = match.Number;
				dateChangedDate = match.DateChangedDate;
				if (match.Functionaries != null)
				{
					foreach (int funcID in match.Functionaries)
					{
						supervisor = new Sport.Entities.Functionary(funcID);
						if ((supervisor != null) && (supervisor.Id > 0) && (supervisor.FunctionaryType == Sport.Types.FunctionaryType.Coordinator))
							break;
						else
							supervisor = null;
					}
				}
			}

			if (objMatch is Sport.Common.MatchData)
			{
				Sport.Common.MatchData match = (Sport.Common.MatchData)objMatch;
				strDebug += "Sport.Common.MatchData<br />";
				if ((match.Facility != null) && (match.Facility.ID >= 0))
					facility = new Sport.Entities.Facility(match.Facility.ID);
				if ((match.Court != null) && (match.Court.ID >= 0))
					court = new Sport.Entities.Court(match.Court.ID);
				outcome = (Sport.Championships.MatchOutcome)match.Result;
				partsResult = Tools.CStrDef(match.PartsResult, "");
				score1 = match.TeamA_Score;
				score2 = match.TeamB_Score;
				teamA_id = match.TeamA.ID;
				teamB_id = match.TeamB.ID;
				strDebug += "Team A id: " + teamA_id + ", team B id: " + teamB_id + "<br />";
				if (teamA_id >= 0)
				{
					teamA = new Sport.Entities.Team(teamA_id);
					strTeamA_name = teamA.TeamName();
				}
				else
				{
					//not supported yet
				}
				if (teamB_id >= 0)
				{
					teamB = new Sport.Entities.Team(teamB_id);
					strTeamB_name = teamB.TeamName();
				}
				else
				{
					//not supported yet
				}
				categoryID = match.CategoryID;
				phaseIndex = match.Round.Group.Phase.PhaseIndex;
				groupIndex = match.Round.Group.GroupIndex;
				roundIndex = match.Round.RoundIndex;
				matchIndex = match.MatchIndex;
				matchTime = match.Time;
				phaseName = match.Round.Group.Phase.PhaseName;
				groupName = match.Round.Group.GroupName;
				roundName = match.Round.RoundName;
			}

			champCategory = categoryID;
			if (Request["debug"] == "1")
				return new string[] { "row #" + row + ": " + strDebug + "<br />" };

			strTeamA_name = Tools.CStrDef(strTeamA_name, "");
			strTeamB_name = Tools.CStrDef(strTeamB_name, "");

			string strFacility = (facility == null) ? "-" : facility.Name;
			string strCourt = (court == null) ? "-" : court.Name;
			bool winA = ((outcome == Sport.Championships.MatchOutcome.WinA) || (outcome == Sport.Championships.MatchOutcome.TechnicalA));
			bool winB = ((outcome == Sport.Championships.MatchOutcome.WinB) || (outcome == Sport.Championships.MatchOutcome.TechnicalB));
			string strScore = "-";
			if ((outcome != Sport.Championships.MatchOutcome.None) && (score1 >= 0) && (score2 >= 0))
			{
				strScore = "";
				string strScore1 = score1.ToString();
				string strScore2 = score2.ToString();
				string strRedHTML = "<font color=\"#cc0000\">%text</font>";
				strScore += (winA) ? strRedHTML.Replace("%text", strScore1) : strScore1;
				strScore += " - ";
				strScore += (winB) ? strRedHTML.Replace("%text", strScore2) : strScore2;
				if (partsResult.Length > 0)
					strScore += Tools.GetSmallPointsHTML(partsResult);
				if ((outcome == Sport.Championships.MatchOutcome.TechnicalA) ||
					(outcome == Sport.Championships.MatchOutcome.TechnicalB))
				{
					strScore += "<br /><span class=\"SmallBorderedText\">טכני</span>";
				}
			}
			string strLinkHTML = "<a href=\"%href\" class=\"ChampLink1\">%text</a>";
			string strMatchLink = strLinkHTML.Replace("%href", strBaseLink);
			strMatchLink = strMatchLink.Replace("%1", categoryID.ToString());
			strMatchLink = strMatchLink.Replace("%2", phaseIndex.ToString());
			strMatchLink = strMatchLink.Replace("%3", groupIndex.ToString());
			strMatchLink = strMatchLink.Replace("%4", roundIndex.ToString());
			strMatchLink = strMatchLink.Replace("%5", cycleIndex.ToString());
			strMatchLink = strMatchLink.Replace("%6", matchIndex.ToString());
			string strTeamLink = strLinkHTML.Replace("%href", "?team=%team\" style=\"%style");
			string strTeamLinkA = strTeamLink.Replace("%style", (winA) ? "color: #CC0000;" : "");
			string strTeamLinkB = strTeamLink.Replace("%style", (winB) ? "color: #CC0000;" : "");
			string strFacilityLink = strMatchLink;
			if (facility != null)
				strFacilityLink = strLinkHTML.Replace("%href", "?facility=" + facility.Id);
			string strCoutrtLink = strMatchLink;
			if (court != null)
				strCoutrtLink = strLinkHTML.Replace("%href", "?court=" + court.Id);

			//game number
			string text = (row + 1).ToString();
			result.Add((cycleIndex >= 0) ? strMatchLink.Replace("%text", text) : text);

			//teamA name
			if (strTeamA_name.Length > 0)
				result.Add(strTeamLinkA.Replace("%team", teamA_id.ToString()).Replace("%text", strTeamA_name));
			else
				result.Add("&nbsp;");

			//team B name
			if (strTeamB_name.Length > 0)
				result.Add(strTeamLinkB.Replace("%team", teamB_id.ToString()).Replace("%text", strTeamB_name));
			else
				result.Add("&nbsp;");

			//match time
			text = (matchTime.Year < 1900) ? "-" : matchTime.ToString("dd/MM/yyyy<br />HH:mm");
			result.Add((cycleIndex >= 0) ? strMatchLink.Replace("%text", text) : text);

			//day
			text = (matchTime.Year < 1900) ? "&nbsp;" : Sport.Common.Tools.GetHebDayOfWeek(matchTime);
			result.Add(text);

			//facility
			result.Add(strFacilityLink.Replace("%text", strFacility));

			//supervisor
			result.Add((supervisor == null) ? "&nbsp;" : supervisor.Name);

			//score
			text = strScore;
			result.Add((cycleIndex >= 0) ? strMatchLink.Replace("%text", text) : text);

			//date changed?
			blnDateChanged = false;
			if (dateChangedDate.Year > 1900)
			{
				blnDateChanged = (((DateTime.Now - dateChangedDate).TotalDays) < 7);
			}

			//done.
			return (string[])result.ToArray(typeof(string));
		}
		#endregion

		#region General Functions
		#region Get Matches
		private Sport.Championships.Match[] GetChampionshipMatches(
			Sport.Championships.MatchChampionship champ, int phaseIndex,
			int groupIndex, int roundIndex, int cycleIndex)
		{
			ArrayList arrMatches = new ArrayList();
			foreach (Sport.Championships.MatchPhase phase in champ.Phases)
			{
				if ((phaseIndex >= 0) && (phaseIndex != phase.Index))
					continue;
				foreach (Sport.Championships.MatchGroup group in phase.Groups)
				{
					if ((groupIndex >= 0) && (groupIndex != group.Index))
						continue;
					foreach (Sport.Championships.Round round in group.Rounds)
					{
						if ((roundIndex >= 0) && (roundIndex != round.Index))
							continue;
						foreach (Sport.Championships.Cycle cycle in round.Cycles)
						{
							if ((cycleIndex >= 0) && (cycleIndex != cycle.Index))
								continue;
							arrMatches.AddRange(cycle.Matches);
						}
					}
				}
			}
			//Response.Write(arrMatches.Count+" matches<br />");
			return (Sport.Championships.Match[])
				arrMatches.ToArray(typeof(Sport.Championships.Match));
		}
		#endregion

		#region Add Links Title
		private void AddLinksTitle()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append("<script type=\"text/javascript\">");
			sb.Append("function SetLinksTitle() {");
			sb.Append(" var arrLinks=document.getElementsByTagName(\"a\");");
			sb.Append(" var arrTitles=new Array();");
			sb.Append(" arrTitles[\"match=\"] = \"הצג נתוני משחק זה\";");
			sb.Append(" arrTitles[\"competition=\"] = \"הצג נתוני תחרות זו\";");
			sb.Append(" arrTitles[\"team=\"] = \"הצג נתוני קבוצה זו\";");
			sb.Append(" arrTitles[\"school=\"] = \"הצג נתוני בית ספר זה\";");
			sb.Append(" arrTitles[\"facility=\"] = \"הצג נתוני מתקן זה\";");
			sb.Append(" arrTitles[\"court=\"] = \"הצג נתוני מגרש זה\";");
			sb.Append(" for (var i=0; i<arrLinks.length; i++) {");
			sb.Append("  if ((!arrLinks[i].href)||(arrLinks[i].href.length < 3))");
			sb.Append("   continue;");
			sb.Append("  var strTitle=\"\";");
			sb.Append("  for (var title in arrTitles) {");
			sb.Append("   if (arrLinks[i].href.indexOf(title) > 0)");
			sb.Append("    arrLinks[i].title = arrTitles[title];");
			sb.Append("  }");
			sb.Append(" }");
			sb.Append("}");
			sb.Append("</script>");
			Page.ClientScript.RegisterClientScriptBlock(Page.GetType(), "SetLinksTitle", sb.ToString(), false);
			IsfMainView.clientSide.AddOnloadCommand("SetLinksTitle();", true);
		}
		#endregion

		#region Single Detail
		private string BuildSingleDetail(string caption, string text, string URL, bool newLine,
			Core.AttachmentData[] attachments)
		{
			string result = "";
			result += "<span class=\"text_caption\">" + caption + "</span>";
			if ((text != null) && (text.Length > 0))
			{
				result += "&nbsp;<span class=\"text_content\">";
				if ((URL != null) && (URL.Length > 0))
					result += "<a href=\"" + URL + "\">";
				result += text;
				if ((URL != null) && (URL.Length > 0))
					result += "</a>";
				result += "</span>";
			}
			if ((attachments != null) && (attachments.Length > 0))
			{
				result += "<br />";
				IsfMainView.clientSide.RegisterAddLight(this.Page);
				result += Tools.BuildAttachmentsTable(attachments, this.Server);
			}
			if (newLine)
				result += "<br />";
			return result;
		}

		private string BuildSingleDetail(string caption, string text, string URL, bool newLine)
		{
			return BuildSingleDetail(caption, text, URL, newLine, null);
		}

		private string BuildSingleDetail(string caption, string text, string URL)
		{
			return BuildSingleDetail(caption, text, URL, true);
		}

		private string BuildSingleDetail(string caption, string text)
		{
			return BuildSingleDetail(caption, text, null);
		}

		private string BuildSingleDetail(string caption)
		{
			return BuildSingleDetail(caption, null);
		}
		#endregion
		#endregion

		#region Comparers
		#region Competitor Comparer
		private class CompetitorComparer : IComparer
		{
			//sort by team:
			public int Compare(Object o1, Object o2)
			{
				if ((o1 is Sport.Championships.Competitor) && (o2 is Sport.Championships.Competitor))
				{
					Sport.Championships.Competitor c1 =
						(Sport.Championships.Competitor)o1;
					Sport.Championships.Competitor c2 =
						(Sport.Championships.Competitor)o2;
					return c1.GroupTeam.Id.CompareTo(c2.GroupTeam.Id);
				}
				return 0;
			}
		}
		#endregion

		#region Match Comparer
		private class MatchComparer : IComparer
		{
			private System.Web.HttpResponse Response = null;

			public MatchComparer(System.Web.HttpResponse Response)
			{
				this.Response = Response;
			}

			public int Compare(object x, object y)
			{
				DateTime time1 = DateTime.MinValue;
				DateTime time2 = DateTime.MinValue;
				int number1 = -1;
				int number2 = -1;
				string place1 = "";
				string place2 = "";
				if (x is Sport.Championships.Match)
				{
					time1 = (x as Sport.Championships.Match).Time;
					number1 = (x as Sport.Championships.Match).Number;
					Sport.Entities.Facility facility1 = (x as Sport.Championships.Match).Facility;
					place1 = "";
					if (facility1 != null)
						place1 += facility1.Name;
				}
				if (x is Sport.Common.MatchData)
				{
					time1 = (x as Sport.Common.MatchData).Time;
					Sport.Common.FacilityData facility1 = (x as Sport.Common.MatchData).Facility;
					place1 = "";
					if (facility1 != null)
						place1 += facility1.Name;
				}
				if (y is Sport.Championships.Match)
				{
					time2 = (y as Sport.Championships.Match).Time;
					number2 = (y as Sport.Championships.Match).Number;
					Sport.Entities.Facility facility2 = (y as Sport.Championships.Match).Facility;
					place2 = "";
					if (facility2 != null)
						place2 += facility2.Name;
				}
				if (y is Sport.Common.MatchData)
				{
					time2 = (y as Sport.Common.MatchData).Time;
					Sport.Common.FacilityData facility2 = (y as Sport.Common.MatchData).Facility;
					place2 = "";
					if (facility2 != null)
						place2 += facility2.Name;
				}

				//date is valid for both items?
				if ((time1.Year > 1900) && (time1.Year < 2100) && (time2.Year > 1900) && (time2.Year < 2100))
				{
					//Response.Write("time1: "+time1+", time2: "+time2+", place1: "+place1+", place2: "+place2+"<br />");
					if ((place1 != place2) && (Sport.Common.Tools.IsSameDate(time1, time2)))
					{
						//Response.Write("places are different and same date, comparing by place.<br />");
						return place1.CompareTo(place2);
					}
					//Response.Write("comparing by time.<br />");
					return time1.CompareTo(time2);
				}

				//date is not available, compare by number:
				if ((number1 >= 0) && (number2 >= 0))
					return number1.CompareTo(number2);

				//equal or not comparable
				return 0;
			}
		}
		#endregion

		private string BuildChampCacheKey()
		{
			List<string> parts = new List<string>();
			BuildChampCacheKey(ref parts, Request.QueryString);
			BuildChampCacheKey(ref parts, Request.Form);
			if (parts.Count > 0)
				return "ResultsChampionship_" + string.Join("_", parts);
			return "";
		}

		private void BuildChampCacheKey(ref List<string> parts, NameValueCollection collection)
		{
			if (collection.Count > 0)
			{
				foreach (string key in collection.Keys)
				{
					if (key.Length > 0 && !key.StartsWith("_"))
					{
						string value = collection[key];
						if (value != "-1")
							parts.Add(key + value);
					}
				}
			}
		}


		#region player grade comparer
		private class PlayerGradeComparer : IComparer
		{
			public int Compare(object x, object y)
			{
				if ((x is Sport.Entities.Player) && (y is Sport.Entities.Player))
				{
					Sport.Entities.Player p1 = (Sport.Entities.Player)x;
					Sport.Entities.Player p2 = (Sport.Entities.Player)y;
					return p1.Student.Grade.CompareTo(p2.Student.Grade);
				}
				return 0;
			}
		}
		#endregion

		#region Team Group Comparer
		private class TeamGroupComparer : IComparer
		{
			public int Compare(object x, object y)
			{
				string team1 = x.ToString();
				string team2 = y.ToString();
				string strToSearch = "בית ";
				int index1 = team1.IndexOf(strToSearch);
				int index2 = team2.IndexOf(strToSearch);
				if ((index1 < 0) || (index2 < 0))
					return 0;
				index1 += strToSearch.Length;
				index2 += strToSearch.Length;
				string group1 = "";
				while (team1[index1] != ')')
				{
					group1 += team1[index1].ToString();
					index1++;
				}
				string group2 = "";
				while (team2[index2] != ')')
				{
					group2 += team2[index2].ToString();
					index2++;
				}
				return group1.CompareTo(group2);
			}
		}
		#endregion
		#endregion

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//userManager = new UserManager(this.Session);
			//ClientSide.Page = this.Page;

			this.lbStyle = IsfMainView.lbStyle;
			this.IsfHebDateTime = IsfMainView.IsfHebDateTime;
			this.SideNavBar = IsfMainView.SideNavBar;

			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Load += new System.EventHandler(this.Page_Load);
			this.Unload += new System.EventHandler(this.Page_UnLoad);
			this.PreRender += new System.EventHandler(this.Page_PreRender);
		}
		#endregion
	}
}
