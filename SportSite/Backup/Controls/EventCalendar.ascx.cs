namespace SportSite.Controls
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using Sport.Data;
	using System.Collections.Generic;

	/// <summary>
	///		Summary description for EventCalendar.
	/// </summary>
	public class EventCalendar : System.Web.UI.UserControl
	{
		private readonly int DAYS_PER_WEEK = 7;
		private List<SportSite.DataServices1.ChampionshipData> _championships = new List<DataServices1.ChampionshipData>();
		private List<SportSite.DataServices1.MatchData> _matches = new List<DataServices1.MatchData>();
		private List<SportSite.DataServices1.CompetitionData> _competitions = new List<DataServices1.CompetitionData>();
		private List<SportSite.WebSiteServices.EventData> _events = new List<WebSiteServices.EventData>();
		private Controls.MainView IsfMainView = null;

		#region Properties
		private int _rowHeight = 65;
		private int _cellAddLight = 0;
		private int _year = DateTime.Now.Year;
		private int _month = DateTime.Now.Month;

		public int RowHeight
		{
			get { return _rowHeight; }
			set
			{
				if ((value < 0) || (value > 999))
					throw new ArgumentException("invalid row height: " + value);
				_rowHeight = value;
			}
		}

		public int Year
		{
			get { return _year; }
			set
			{
				if ((value < 1) || (value > 2999))
					throw new ArgumentException("invalid year: " + value);
				_year = value;
			}
		}

		public int Month
		{
			get { return _month; }
			set
			{
				if ((value < 1) || (value > 12))
					throw new ArgumentException("invalid month: " + value);
				_month = value;
			}
		}

		public int CellAddLight
		{
			get { return _cellAddLight; }
			set { _cellAddLight = value; }
		}
		#endregion

		#region Initializtion
		private void Page_Load(object sender, System.EventArgs e)
		{
			//initialize main view:
			IsfMainView = (SportSite.Controls.MainView)
				Common.Tools.FindControlByType(this, "MainView");

			//initialize month:
			if (Request.Form["month"] != null)
				_month = Common.Tools.CIntDef(Request.Form["month"], _month);

			//initialize year:
			if (Request.Form["year"] != null)
				_year = Common.Tools.CIntDef(Request.Form["year"], _year);

			//initialize services:
			DataServices1.DataService _service = new DataServices1.DataService();
			WebSiteServices.WebsiteService websiteService = new WebSiteServices.WebsiteService();

			//get start and end time of the month:
			DateTime start = new DateTime(_year, _month, 1);
			DateTime end = new DateTime(_year, _month, DateTime.DaysInMonth(_year, _month));

			//get events data:
			_matches.Clear();
			_competitions.Clear();
			_events.Clear();

			object data;
			if (CacheStore.Instance.Get("EventCalendar_Championships", out data))
			{
				_championships = (List<DataServices1.ChampionshipData>)data;
			}
			else
			{
				_championships.Clear();
				_championships.AddRange(_service.GetChampionshipData(-1, -1));
				CacheStore.Instance.Update("EventCalendar_Championships", _championships, 5);
			}

			if (CacheStore.Instance.Get("EventCalendar_Matches", out data))
			{
				_matches = (List<DataServices1.MatchData>)data;
			}
			else
			{
				_matches.Clear();
				_matches.AddRange(_service.GetMatchesByDate(start, end));
				CacheStore.Instance.Update("EventCalendar_Matches", _matches, 5);
			}

			if (CacheStore.Instance.Get("EventCalendar_Competitions", out data))
			{
				_competitions = (List<DataServices1.CompetitionData>)data;
			}
			else
			{
				_competitions.Clear();
				_competitions.AddRange(_service.GetCompetitionsByDate(start, end));
				CacheStore.Instance.Update("EventCalendar_Competitions", _competitions, 5);
			}

			_events.AddRange(websiteService.GetEvents(start, end));

			AddStyle();
			AddJavascript();
		}

		private void AddStyle()
		{
			string strCode = "<link rel=\"stylesheet\" type=\"text/css\" href=\"" + Common.Data.AppPath + "/SportSite.css\" />";
			Page.ClientScript.RegisterClientScriptBlock(Page.GetType(), "global", strCode, false);
		}

		private void AddJavascript()
		{
			if (_cellAddLight <= 0)
				return;

			IsfMainView.clientSide.RegisterAddLight(this.Page);

			//string cellColor=Sport.Common.Tools.ColorToHex(_cellBgColor);
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append("<script type=\"text/javascript\" ");
			sb.Append("src=\"" + Common.Data.AppPath + "/Common/Common.js\"></script>");
			sb.Append("<script type=\"text/javascript\">");
			sb.Append(" function InitEventCalendar() {");
			/*
			sb.Append("   var objTable=document.getElementById(\"EventCalendarTable\");");
			sb.Append("   for (var row=1; row<objTable.rows.length; row++) {");
			sb.Append("      for (var cell=0; cell<objTable.rows[row].cells.length; cell++) {");
			sb.Append("         var objCell=objTable.rows[row].cells[cell];");
			sb.Append("         if ((objCell.innerHTML != \"&nbsp;\")&&(objCell.className != \"highlight\")) {");
			sb.Append("            objCell.onmouseover = new Function(\""+
				"PutMoreLight(this, this.style.backgroundColor, "+_cellAddLight+");\");");
			sb.Append("            objCell.onmouseout = new Function(\""+
				"RestoreColor(this);\");");
			sb.Append("         }");
			sb.Append("      }");
			sb.Append("   }");
			*/
			sb.Append(" }");
			sb.Append("");
			sb.Append(" function GoCalendar(month, year) {");
			sb.Append("    document.forms[0].elements[\"month\"].value = month;");
			sb.Append("    document.forms[0].elements[\"year\"].value = year;");
			sb.Append("    document.forms[0].submit();");
			sb.Append(" }");
			sb.Append("");
			sb.Append("</script>");
			Page.ClientScript.RegisterClientScriptBlock(Page.GetType(), "javascript", sb.ToString(), false);
			IsfMainView.clientSide.AddOnloadCommand("InitEventCalendar()", true);
		}
		#endregion

		#region Render
		protected override void Render(System.Web.UI.HtmlTextWriter writer)
		{
			int daysCount = DateTime.DaysInMonth(_year, _month);
			int weekday = 0;
			int day = 0;
			int counter = 1;
			int firstDay = FirstWeekday();
			int lastDay = firstDay + daysCount - 1;
			int nextMonth, nextYear;
			int prevMonth, prevYear;

			nextMonth = _month + 1;
			nextYear = _year;
			if (nextMonth > 12)
			{
				nextMonth = 1;
				nextYear = _year + 1;
			}

			prevMonth = _month - 1;
			prevYear = _year;
			if (prevMonth < 0)
			{
				prevMonth = 12;
				prevYear = _year - 1;
			}

			writer.Write("<div align=\"right\" dir=\"rtl\">");

			writer.Write("<table id=\"CalendarHeader\" cellpadding=\"0\" cellspacing=\"0\">");
			writer.Write("<tr>");
			writer.Write("<td align=\"right\" style=\"width: 49px; height: 25px;\">");
			writer.Write("<a href=\"javascript:GoCalendar(" + prevMonth + ", " + prevYear + ");\">");
			writer.Write("<img src=\"" + Common.Data.AppPath + "/Images/prev_month.bmp\" /></a></td>");
			writer.Write("<td align=\"center\">");
			writer.Write(HebDateTime.HebMonthName(_month) + "&nbsp;&nbsp;" + _year + "</td>");
			writer.Write("<td align=\"left\" style=\"width: 38px; height: 25px;\">");
			writer.Write("<a href=\"javascript:GoCalendar(" + nextMonth + ", " + nextYear + ");\">");
			writer.Write("<img src=\"" + Common.Data.AppPath + "/Images/next_month.bmp\" /></a></td>");
			writer.Write("</tr>");
			writer.Write("</table><br /><br />");

			writer.Write("<table id=\"EventCalendarTable\" cellpadding=\"0\" cellspacing=\"0\">");
			writer.Write("<tr>");
			for (day = 1; day <= DAYS_PER_WEEK; day++)
			{
				writer.Write("<th>" + IntToHebWeekDay(day) + "</th>");
			}
			writer.Write("</tr>");

			counter = 1;
			day = 0;
			int curYear = DateTime.Now.Year;
			int curMonth = DateTime.Now.Month;
			int curDay = DateTime.Now.Day;
			while ((day < daysCount) || (((counter - 1) % DAYS_PER_WEEK != 0)))
			{
				if (((counter - 1) % DAYS_PER_WEEK) == 0)
				{
					if ((counter - 1) > 0)
						writer.Write("</tr>");
					writer.Write("<tr style=\"height: " + _rowHeight + "px;\">");
				}
				for (weekday = 1; weekday <= DAYS_PER_WEEK; weekday++)
				{
					string strHTML = "<td";
					string strCellText = "";
					if ((counter >= firstDay) && (counter <= lastDay))
					{
						day = counter - firstDay + 1;
						strCellText = GetDayEvents(day);
						string strClassName = "";
						if (strCellText.Length > 0)
							strClassName = "calendar_has_events";
						if (weekday == 7)
							strClassName = "calendar_shabat_day";
						if ((_year == curYear) && (_month == curMonth) && (day == curDay))
							strClassName = "calendar_highlight";
						if (strClassName.Length > 0)
							strHTML += " class=\"" + strClassName + "\"";
						strHTML += ">" + day + "<br />";
						if (strCellText.Length > 0)
						{
							strCellText = "<span class=\"EventCellText\">" +
								strCellText + "</span>";
						}
					}
					else
					{
						strHTML += ">";
					}
					writer.Write(strHTML);
					writer.Write(strCellText + "&nbsp;");
					writer.Write("</td>");
					counter++;
				}
			}
			writer.Write("</tr></table><br />");
			writer.Write("<input type=\"hidden\" name=\"month\" value=\"\" />");
			writer.Write("<input type=\"hidden\" name=\"year\" value=\"\" />");
			/*
			writer.Write("<center>");
			writer.Write("<select name=\"month\" onchange=\"this.form.submit();\">");
			for (int month=1; month<=12; month++)
			{
				writer.Write("<option value=\""+month+"\"");
				if (month == _month)
				{
					writer.Write(" selected=\"selected\"");
				}
				writer.Write(">"+HebDateTime.HebMonthName(month)+"</option>");
			}
			writer.Write("</select>");
			writer.Write(Common.Tools.MultiString("&nbsp;", 5));
			writer.Write("<select name=\"year\" onchange=\"this.form.submit();\">");
			for (int year=2005; year<=DateTime.Now.Year+2; year++)
			{
				writer.Write("<option value=\""+year+"\"");
				if (year == _year)
				{
					writer.Write(" selected=\"selected\"");
				}
				writer.Write(">"+year+"</option>");
			}
			writer.Write("</select>");
			writer.Write("</center>");
			*/
			writer.Write("</div>");
		}
		#endregion

		#region general methods
		private string GetDayEvents(int day)
		{
			string result = "";

			//general events
			string strGeneralEvents = GetGeneralEvents(day);
			result += strGeneralEvents;

			//championships
			string strChampEvents = GetChampionshipEvents(day);
			if (strChampEvents.Length > 0)
			{
				result += "<br />" + strChampEvents;
			}

			//matches
			string strMatchEvent = GetMatchEvents(day);
			if (strMatchEvent.Length > 0)
			{
				result += "<br />" + strMatchEvent;
			}

			//competitions
			string strCompetitionEvent = GetCompetitionEvents(day);
			if (strCompetitionEvent.Length > 0)
			{
				result += "<br />" + strCompetitionEvent;
			}

			return result;
		}

		private string GetGeneralEvents(int day)
		{
			List<WebSiteServices.EventData> events = _events.FindAll(eventData => eventData.Date.Day == day);
			if (events.Count == 0)
				return "";

			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			int eventCount = events.Count;
			string containerID = "general_event_" + day;
			string strDisplay = Sport.Common.Tools.BuildOneOrMany(
				"אירוע", "אירועים", eventCount, true);
			string strTitle = "";
			//sb.Append("<a href=\"javascript:ToggleVisibility('"+containerID+"');\" ");
			//sb.Append("class=\"EventLink\" title=\"%title\">");
			//sb.Append("["+strDisplay+"]</a>");
			sb.Append("<div id=\"" + containerID + "\">");
			events.ForEach(eventData =>
			{
				string strLink = Common.Tools.CStrDef(eventData.URL, "");
				if (strLink.Length > 0)
					sb.Append("<a href=\"" + strLink + "\">");
				sb.Append(Common.Tools.ToHTML(eventData.Description));
				if (strLink.Length > 0)
					sb.Append("</a>");
				sb.Append("<br />");
				strTitle += eventData.Description + "\n";
			});
			sb.Append("</div>");

			strTitle = strTitle.Replace("\"", "&quot;");
			return sb.ToString().Replace("%title", strTitle);
		}

		private string GetChampionshipEvents(int day)
		{
			DateTime date = new DateTime(_year, _month, day);
			List<SportSite.DataServices1.ChampionshipData> champs = _championships.FindAll(champ =>
			{
				DateTime start = champ.StartDate;
				DateTime end = champ.EndDate;
				return (start.Year > 2000 && end.Year > 2000 && Sport.Common.Tools.DateInRange(date, start, end));
			});
			
			if (champs.Count == 0)
				return "";

			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			int champCount = champs.Count;
			string containerID = "champ_event_" + day;
			string strDisplay = Sport.Common.Tools.BuildOneOrMany(
				"אליפות", "אליפויות", champCount, false);
			string strBaseLink = "<a href=\"Results.aspx?championship=%id\">";
			string strTitle = "";
			//sb.Append("<a href=\"javascript:ToggleVisibility('"+containerID+"');\" ");
			//sb.Append("class=\"EventLink\" title=\"%title\">");
			//sb.Append("["+strDisplay+"]</a>");
			sb.Append("<div id=\"" + containerID + "\">");
			champs.ForEach(champ =>
			{
				sb.Append(strBaseLink.Replace("%id", champ.ID.ToString()));
				sb.Append(champ.Name + "</a><br />");
				strTitle += champ.Name + "\n";
			});
			sb.Append("</div>");

			strTitle = strTitle.Replace("\"", "&quot;");
			return sb.ToString().Replace("%title", strTitle);
		}

		private string GetMatchEvents(int day)
		{
			List<DataServices1.MatchData> matches = _matches.FindAll(match => match.Time.Day == day);
			if (matches.Count == 0)
				return "";

			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			int matchCount = matches.Count;
			string containerID = "match_event_" + day;
			string strDisplay = Sport.Common.Tools.BuildOneOrMany(
				"משחק", "משחקים", matchCount, true);
			string strBaseLink = "<a href=\"Results.aspx?category=%1&phase=%2&group=%3";
			strBaseLink += "&round=%4&cycle=%5&match=%6\">";
			string strTitle = "";
			sb.Append("<a href=\"javascript:ToggleVisibility('" + containerID + "');\" ");
			sb.Append("class=\"EventLink\" title=\"%title\">");
			sb.Append("[" + strDisplay + "]</a>");
			sb.Append("<div id=\"" + containerID + "\" style=\"display: none;\"><br />");
			matches.ForEach(match =>
			{
				string strLink = strBaseLink;
				string phaseIndex = "";
				string groupIndex = "";
				string roundIndex = "";
				if (match.Round != null)
				{
					if (match.Round.Group != null)
					{
						if (match.Round.Group.Phase != null)
							phaseIndex = match.Round.Group.Phase.PhaseIndex.ToString();
						groupIndex = match.Round.Group.GroupIndex.ToString();
					}
					roundIndex = match.Round.RoundIndex.ToString();
				}
				strLink = strLink.Replace("%1", match.CategoryID.ToString());
				strLink = strLink.Replace("%2", phaseIndex);
				strLink = strLink.Replace("%3", groupIndex);
				strLink = strLink.Replace("%4", roundIndex);
				strLink = strLink.Replace("%5", match.MatchIndex.ToString());
				string strTeam_A = ParseTeamName(match.TeamA);
				string strTeam_B = ParseTeamName(match.TeamB);
				if (strTeam_A.Length > 0 && strTeam_B.Length > 0)
				{
					string strMatch = strTeam_A + " -- " + strTeam_B;
					sb.Append(strLink + strMatch + "</a><br />");
					strTitle += strMatch + "\n";
				}
				//sb.Append(strMatch + "<br />");
			});
			sb.Append("</div>");

			strTitle = strTitle.Replace("\"", "&quot;");
			return sb.ToString().Replace("%title", strTitle);
		}

		private string GetCompetitionEvents(int day)
		{
			List<DataServices1.CompetitionData> competitions = _competitions.FindAll(competition => competition.Time.Day == day);
			if (competitions.Count == 0)
				return "";

			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			int compCount = competitions.Count;
			string containerID = "competition_event_" + day;
			string strDisplay = Sport.Common.Tools.BuildOneOrMany(
				"תחרות", "תחרויות", compCount, false);
			string strBaseLink = "<a href=\"Results.aspx?category=%1&phase=%2&group=%3";
			strBaseLink += "&competition=%5\">";
			string strTitle = "";
			sb.Append("<a href=\"javascript:ToggleVisibility('" + containerID + "');\" ");
			sb.Append("class=\"EventLink\" title=\"%title\">");
			sb.Append("[" + strDisplay + "]</a>");
			sb.Append("<div id=\"" + containerID + "\" style=\"display: none;\"><br />");
			competitions.ForEach(competition =>
			{
				if (competition.SportField != null)
				{
					string strLink = strBaseLink;
					strLink = strLink.Replace("%1", competition.CategoryID.ToString());
					strLink = strLink.Replace("%2", competition.PhaseIndex.ToString());
					strLink = strLink.Replace("%3", competition.GroupIndex.ToString());
					strLink = strLink.Replace("%5", competition.CompetitionIndex.ToString());
					string sportField = competition.SportField.Name;
					string strCompetition = sportField;
					if (competition.SportField.SportFieldType != null)
						strCompetition += ": " + competition.SportField.SportFieldType.Name;
					sb.Append(strLink + strCompetition + "</a><br />");
					strTitle += strCompetition + "\n";
				}
			});
			sb.Append("</div>");

			strTitle = strTitle.Replace("\"", "&quot;");
			return sb.ToString().Replace("%title", strTitle);
		}

		private int FirstWeekday()
		{
			DateTime date = new DateTime(_year, _month, 1);
			switch (date.DayOfWeek)
			{
				case DayOfWeek.Sunday:
					return 1;
				case DayOfWeek.Monday:
					return 2;
				case DayOfWeek.Tuesday:
					return 3;
				case DayOfWeek.Wednesday:
					return 4;
				case DayOfWeek.Thursday:
					return 5;
				case DayOfWeek.Friday:
					return 6;
				case DayOfWeek.Saturday:
					return 7;
			}
			return 0;
		}

		private string IntToHebWeekDay(int day)
		{
			string[] days = new string[] {"ראשון", "שני", "שלישי", "רביעי", "חמישי", 
							"שישי", "שבת"};
			/* string[] days=new string[] {"א'", "ב'", "ג'", "ד'", "ה'", 
										   "ו'", "שבת"}; */
			day = day - 1;
			if ((day >= 0) && (day < days.Length))
				return days[day];
			return "לא ידוע";
		}

		private string ParseTeamName(DataServices1.TeamData team)
		{
			string name = "";
			if (team != null && team.School != null)
			{
				name = team.School.Name;
				if (team.TeamIndex > 0)
					name += " " + Sport.Common.Tools.ToHebLetter(team.TeamIndex) + "'";
			}
			return name;
		}
		#endregion

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Load += new System.EventHandler(this.Page_Load);
		}
		#endregion
	}
}
