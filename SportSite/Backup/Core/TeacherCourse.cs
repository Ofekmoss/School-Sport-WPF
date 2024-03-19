using System;
using System.Linq;
using System.Collections.Generic;
using System.Web;

namespace SportSite.Core
{
	public static class TeacherCourseManager
	{
		private static Dictionary<string, string> _defaultCaptions = new Dictionary<string, string>();

		static TeacherCourseManager()
		{
			Sport.Types.TeacherExpertiseLookup teacherExpertiseLookup = new Sport.Types.TeacherExpertiseLookup();
			Sport.Types.CourseAgeRangeLookup courseAgeRangeLookup = new Sport.Types.CourseAgeRangeLookup();

			_defaultCaptions.Clear();
			_defaultCaptions.Add("lname", "שם משפחה");
			_defaultCaptions.Add("fname", "שם פרטי");
			_defaultCaptions.Add("id", "ת.ז.");
			_defaultCaptions.Add("b_day", "תאריך לידה");
			_defaultCaptions.Add("address", "כתובת");
			_defaultCaptions.Add("city", "עיר");
			_defaultCaptions.Add("zip_code", "מיקוד");
			_defaultCaptions.Add("email", "דואר אלקטרוני");
			_defaultCaptions.Add("home_phone", "טלפון בית");
			_defaultCaptions.Add("fax_number", "פקס");
			_defaultCaptions.Add("cell_phone", "טלפון סלולרי");
			_defaultCaptions.Add("gender", "מין");
			_defaultCaptions.Add("man", "גבר");
			_defaultCaptions.Add("woman", "אישה");
			_defaultCaptions.Add("school_name", "שם ביה\"ס / מועדון ספורט");
			_defaultCaptions.Add("school_city", "שם הרשות");
			_defaultCaptions.Add("school_address", "כתובת ביה\"ס");
			_defaultCaptions.Add("sport_1", "ענף התמחות ראשון");
			_defaultCaptions.Add("sport_2", "ענף התמחות שני");
			_defaultCaptions.Add("veteranship", "ותק באימון (שנים)");
			_defaultCaptions.Add("expertise", "הכשרה");
			for (int i = 1; i <= 6; i++)
				_defaultCaptions.Add("expertise_" + i, teacherExpertiseLookup.Lookup(i - 1));
			_defaultCaptions.Add("age_range", "קבוצות אימון ע\"פ גילאים");
			for (int i = 1; i <= 8; i++)
				_defaultCaptions.Add("age_range_" + i, courseAgeRangeLookup.Lookup(i));
			_defaultCaptions.Add("course", "פרטי השתלמות:");
		}

		public static string[] GetDisplayKeys()
		{
			return _defaultCaptions.Keys.ToArray();
		}

		public static string[] HiddenElements
		{
			get
			{
				return (Common.Tools.ReadIniValue("TeacherCourse", "HiddenElements", HttpContext.Current.Server) + "").Split(',');
			}
			set
			{
				Common.Tools.WriteIniValue("TeacherCourse", "HiddenElements", string.Join(",", value), HttpContext.Current.Server);
			}
		}

		public static Dictionary<string, string> ElementCaptions
		{
			get
			{
				string[] rawData = (Common.Tools.ReadIniValue("TeacherCourse", "ElementCaptions", HttpContext.Current.Server) + "").Split('|');
				Dictionary<string, string> overridenCaptions = new Dictionary<string, string>();
				rawData.ToList().ForEach(d =>
				{
					string[] elementData = d.Split('=');
					if (elementData.Length > 1 && !string.IsNullOrEmpty(elementData[1]) && !overridenCaptions.ContainsKey(elementData[0]))
						overridenCaptions.Add(elementData[0], elementData[1]);
				});

				Dictionary<string, string> captions = new Dictionary<string, string>();
				_defaultCaptions.Keys.ToList().ForEach(key => { captions.Add(key, overridenCaptions.ContainsKey(key) ? overridenCaptions[key] : _defaultCaptions[key]); });
				return captions;
			}
			set
			{
				List<string> captions = new List<string>();
				value.Keys.ToList().ForEach(key =>
				{
					if (_defaultCaptions.ContainsKey(key) && !string.Equals(value[key], _defaultCaptions[key], StringComparison.CurrentCultureIgnoreCase))
						captions.Add(key + "=" + value[key]);
				});
				Common.Tools.WriteIniValue("TeacherCourse", "ElementCaptions", string.Join("|", captions.ToArray()), HttpContext.Current.Server);
			}
		}

		public static int GetDynamicPageID(System.Web.HttpServerUtility Server)
		{
			string strValue = Common.Tools.ReadIniValue("TeacherCourse", "DynamicPage", Server);
			return Common.Tools.CIntDef(strValue, -1);
		}

		public static void SetDynamicPageID(int pageID, System.Web.HttpServerUtility Server)
		{
			Common.Tools.WriteIniValue("TeacherCourse", "DynamicPage", pageID.ToString(), Server);
		}


		public static int[] GetSports(string key, System.Web.HttpServerUtility Server)
		{
			string rawValue = Common.Tools.ReadIniValue("TeacherCourse", key + "Sports", Server) + "";
			List<int> values = new List<int>();
			int curValue;
			if (rawValue.Length > 0)
			{
				string[] temp = rawValue.Split(',');
				foreach (string id in temp)
				{
					if (id.Length > 0 && Int32.TryParse(id, out curValue) && values.IndexOf(curValue) < 0)
					{
						values.Add(curValue);
					}
				}
			}
			return values.ToArray();
		}

		public static int[] GetExpertiseSports(System.Web.HttpServerUtility Server)
		{
			return GetSports("Expertise", Server);
		}

		public static int[] GetCourseSports(System.Web.HttpServerUtility Server)
		{
			return GetSports("Course", Server);
		}

		public static void SetSports(int[] sportIDs, string key, System.Web.HttpServerUtility Server)
		{
			Common.Tools.WriteIniValue("TeacherCourse", key + "Sports", string.Join(",", Sport.Common.Tools.ToStringArray(sportIDs)), Server);
		}

		public static void SetExpertiseSports(int[] expertiseSportIDs, System.Web.HttpServerUtility Server)
		{
			SetSports(expertiseSportIDs, "Expertise", Server);
		}

		public static void SetCourseSports(int[] courseSportIDs, System.Web.HttpServerUtility Server)
		{
			SetSports(courseSportIDs, "Course", Server);
		}

		public static string Caption
		{
			get
			{
				string caption = (Common.Tools.ReadIniValue("TeacherCourse", "Caption", HttpContext.Current.Server) + "").Trim();
				if (caption.Length == 0)
					caption = "טופס הרשמה להשתלמות ארצית";
				return caption;
			}
			set { Common.Tools.WriteIniValue("TeacherCourse", "Caption", value, HttpContext.Current.Server); }
		}

		public static int GetMaxCoursesToShow(System.Web.HttpServerUtility Server)
		{
			string rawValue = Common.Tools.ReadIniValue("TeacherCourse", "CoursesToDisplay", Server) + "";
			if (rawValue.Length > 0)
			{
				int value;
				if (Int32.TryParse(rawValue, out value))
					return value;
			}
			return 6;
		}

		public static void SetMaxCoursesToShow(int value, System.Web.HttpServerUtility Server)
		{
			Common.Tools.WriteIniValue("TeacherCourse", "CoursesToDisplay", value.ToString(), Server);
		}

		public static void BuildTeacherCourses(ref KeyValuePair<string, string>[] arrCourses)
		{
			Sport.Types.HolidayTypeLookup holidayLookup = new Sport.Types.HolidayTypeLookup();
			int curYear = DateTime.Now.Year;
			int y = curYear;
			Dictionary<string, DateTime> holidaysDateMapping = new Dictionary<string, DateTime>();
			int c = 0;
			while (c < arrCourses.Length)
			{
				//sanity check:
				if (y > curYear + 10)
					break;

				for (int i = 0; i < holidayLookup.Items.Length; i++)
				{
					if (c >= arrCourses.Length)
						break;

					string curHolidayName = holidayLookup.Lookup(i);

					//maybe already passed?
					DateTime dtCurHolidatDate = HebrewDates.Instance.GetGregorianJewishHoliday(curHolidayName, y - curYear);
					if (dtCurHolidatDate.Year > 1900 && dtCurHolidatDate > DateTime.Now)
					{
						int realYear = dtCurHolidatDate.Year;
						string strCurCourse = string.Format("{0} {1}", curHolidayName, realYear);
						string strCurrentKey = string.Format("{0}_{1}", i, realYear);
						arrCourses[c] = new KeyValuePair<string, string>(strCurrentKey, strCurCourse);
						holidaysDateMapping.Add(strCurrentKey, dtCurHolidatDate);
						c++;
					}
				}
				y++;
			}

			if (c > 0)
			{
				Array.Sort<KeyValuePair<string, string>>(arrCourses, delegate(KeyValuePair<string, string> kvp1, KeyValuePair<string, string> kvp2)
				{
					return holidaysDateMapping[kvp1.Key].CompareTo(holidaysDateMapping[kvp2.Key]);
				});
			}
		}
	}
}
