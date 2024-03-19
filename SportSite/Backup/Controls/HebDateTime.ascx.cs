namespace SportSite.Controls
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
    using System.Globalization;

	/*
			list of supported formats:
			%yyyy - full year, 1000-...
			%hwd - Hebrew Week Day
			%hm - Hebrew Month (month name in hebrew)
			%hd - Hebrew Date (jewish calendar day, month and year)
			%yy - year as two digits, 00-99
			%d - day of month, 1-31
			%m - month of year, 1-12
			%h - hour of day, 0-23
			%n - minutes, 00-59
			%s - seconds, 00-59
	*/

	/// <summary>
	///		output current date and time according to hebrew calendar.
	/// </summary>
	public class HebDateTime : System.Web.UI.UserControl
	{
		protected System.Web.UI.WebControls.Label lbDateTime;
		public string Format="";

		private void Page_Load(object sender, System.EventArgs e)
		{
			if (lbDateTime == null)
			{
				lbDateTime = new Label();
				this.Controls.Add(lbDateTime);
			}
			
			foreach (string key in this.Attributes.Keys)
				lbDateTime.Attributes.Add(key, this.Attributes[key]);
			
			lbDateTime.Text = GetHebrewDate();
		}

		private string GetHebrewDate()
		{
			//default format:
			string format="%hwd %hd";
			string result="";

			//get from user:
			if (Format.Length > 0)
				format = Format;
			
			//get current date and time:
            DateTime dtCurDate = DateTime.Now;
			
			//format date string:
			result = format;
			result = result.Replace("%yyyy", FullYear(dtCurDate));
			result = result.Replace("%hwd", HebWeekDay(dtCurDate));
			result = result.Replace("%hm", HebDateTime.HebMonthName(dtCurDate));
			result = result.Replace("%hd", HebDate(dtCurDate));
			result = result.Replace("%yy", ShortYear(dtCurDate));
			result = result.Replace("%d", DayOfMonth(dtCurDate));
			result = result.Replace("%m", MonthOfYear(dtCurDate));
			result = result.Replace("%h", HourOfDay(dtCurDate));
			result = result.Replace("%n", MinuteOfHour(dtCurDate));
			result = result.Replace("%s", Seconds(dtCurDate));
			
			/* string strSpecial=GetSpecialOccasions(dtCurDate);
			if (strSpecial.Length > 0)
				result += "<br />"+strSpecial; */
			
			return result;
		}
		
		private string GetSpecialOccasions(DateTime date)
		{
			string result="";
			string seperator="&nbsp;&nbsp;";
			
			if (IsSaturday(date))
				result += "שבת שלום"+seperator;
			
			if (IsWeekBegin(date))
				result += "שבוע טוב"+seperator;
			
			if (IsTzom(date))
				result += "צום קל"+seperator;

			if (IsRoshShana(date))
				result += "שנה טובה"+seperator;
			
			if ((date.Month == 9)&&(date.Day < 10))
				result += "שנת לימודים פורייה"+seperator;
			
			if (result.Length > 0)
				result = result.Substring(0, result.Length-seperator.Length);
			
			return result;
		}
		
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
		
		private String DayOfMonth(DateTime date)
		{
			return date.Day.ToString();
		}

		private String MonthOfYear(DateTime date)
		{
			return date.Month.ToString();
		}

		private String ShortYear(DateTime date)
		{
			String result=date.Year.ToString();
			if (result.Length <= 3)
				return result;
			result = result.Substring(result.Length-2, 2);
			return result;
		}

		private String FullYear(DateTime date)
		{
			return date.Year.ToString();
		}

		private String HourOfDay(DateTime date)
		{
			return date.ToString("HH");
		}

		private String MinuteOfHour(DateTime date)
		{
			return AddZero(date.Minute);
		}

		private String Seconds(DateTime date)
		{
			return AddZero(date.Second);
		}

		private String HebWeekDay(DateTime date)
		{
			return HebDates.JewishDate(date, "%hwd");
		}
		
		public static string HebMonthName(int month)
		{
			return Sport.Common.Tools.HebMonthName(month);
		}
		
		public static string HebMonthName(DateTime date)
		{
			return HebMonthName(date.Month);
		}
		
		private bool IsSaturday(DateTime dt)
		{
			switch (dt.DayOfWeek)
			{
				case DayOfWeek.Friday:
					return (dt.Hour>=15);
				case DayOfWeek.Saturday:
					return (dt.Hour<=19);
				default:
					return false;
			}
		}

		private bool IsWeekBegin(DateTime dt)
		{
			switch (dt.DayOfWeek)
			{
				case DayOfWeek.Saturday:
					return (dt.Hour>19);
				case DayOfWeek.Sunday:
					return (dt.Hour<=12);
				default:
					return false;
			}
		}

		private bool IsTzom(DateTime dt)
		{
			CultureInfo myCulture=CultureInfo.CreateSpecificCulture("he-IL");
			myCulture.DateTimeFormat.Calendar = new HebrewCalendar();
			string strHebDate=dt.ToString("m", myCulture);
			
			//tet av?
			if (strHebDate == "ט' אב")
				return true;

			//גדליה?
			if (strHebDate == "ג' תשרי")
				return true;
			
			return false;
		}
		
		private bool IsRoshShana(DateTime dt)
		{
			CultureInfo myCulture=CultureInfo.CreateSpecificCulture("he-IL");
			myCulture.DateTimeFormat.Calendar = new HebrewCalendar();
			string strHebDate=dt.ToString("m", myCulture);
			
			return ((strHebDate == "א' תשרי")||(strHebDate == "ב' תשרי"));
		}
		
		private String HebDate(DateTime date)
		{
			return HebDates.JewishDate(date, "%hmd ב%y");
		}

		private String AddZero(int num)
		{
			if ((num >= 10)||(num < 0))
				return num.ToString();
			return "0"+num.ToString();
		}

		public class HebDates
		{
			public static String JewishDate(DateTime date, String format)
			{
				/*
					list of supported formats:
					%hwd - Hebrew Week Day (rishon,sheni... ראשון, שני,)
					%hmd - Hebrew Month Day (א', ב', ..., י"א, ..., ל"א)
					%y - hebrew month and year
				*/
				//System.Text.StringBuilder dateResult=new System.Text.StringBuilder();

				String result=format;
				CultureInfo myCulture=CultureInfo.CreateSpecificCulture("he-IL");
				myCulture.DateTimeFormat.Calendar = new HebrewCalendar();

				result = result.Replace("%hwd", HebDates.DayOfWeek(date, myCulture));
				result = result.Replace("%hmd", HebDates.DayOfMonth(date, myCulture));
				result = result.Replace("%y", HebDates.MonthAndYear(date, myCulture));

				return result;
			}

			private static String DayOfWeek(DateTime date, CultureInfo culture)
			{
				return date.ToString("dddd", culture);
			}

			private static String DayOfMonth(DateTime date, CultureInfo culture)
			{
				return date.ToString("dd", culture);
			}

			private static String MonthAndYear(DateTime date, CultureInfo culture)
			{
				return date.ToString("y", culture);
			}
		} //end class HebDates
	} //end class HebDateTime
}
