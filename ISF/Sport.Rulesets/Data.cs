using System;

namespace Sport.Rulesets
{
	/// <summary>
	/// Summary description for Data.
	/// </summary>
	public class Data
	{
		public static readonly string[] ScoreNames = new string[]
			{
				"ניצחון",
				"הפסד",
				"תיקו",
				"הפסד טכני",
				"ניצחון טכני"
			};

		public static readonly string[] ValueNames = new string[]
			{ "נקודות", "מרחק", "זמן" };
		public static readonly string[] DirectionNames = new string[]
			{ "עולה", "יורד" };
		public static readonly string[] DistanceNames = new string[]
			{ "ק\"מ", "מטר", "ס\"מ", "מ\"מ" };
		public static readonly string[] TimeNames = new string[]
			{ "ימים", "שעות", "דקות", "שניות", "אלפיות" };
		
		public static readonly string[] MethodValueDescription = new string[]
			{
				"ניקוד",
				"יחס [%pn]",
				"הפרש [%pn]",
				"[%pn] זכות", 
				"נקודות קטנות זכות"
			};
		public static readonly string MatchedTeamsDescription = "קבוצות שוויון";
	}
}
