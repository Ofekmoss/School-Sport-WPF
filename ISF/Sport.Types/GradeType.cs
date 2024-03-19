using System;
using Sport.Data;

namespace Sport.Types
{
	public class GradeTypeLookup : LookupType
	{
		private static int season = 56;
		static GradeTypeLookup()
		{
			season = Core.Session.Season;
			ResetRelativeGrades();
			Core.Session.SeasonParameter.ValueChanged += new EventHandler(SeasonChanged);
		}

		private static void SeasonChanged(object sender, EventArgs e)
		{
			season = Core.Session.Season;
		}

		private static LookupItem[] relativeGrades;

		private static LookupItem[] grades = new LookupItem[]  
			{ 
				new LookupItem(1, "א"),
				new LookupItem(2, "ב"),
				new LookupItem(3, "ג"),
				new LookupItem(4, "ד"),
				new LookupItem(5, "ה"),
				new LookupItem(6, "ו"),
				new LookupItem(7, "ז"),
				new LookupItem(8, "ח"),
				new LookupItem(9, "ט"),
				new LookupItem(10, "י"),
				new LookupItem(11, "י\"א"),
				new LookupItem(12, "י\"ב"),
				new LookupItem(13, "י\"ג"),
				new LookupItem(14, "י\"ד")
			};


		public static void ResetRelativeGrades()
		{
			relativeGrades = new LookupItem[] { 
				new LookupItem(season, "א"),
				new LookupItem(season - 1, "ב"),
				new LookupItem(season - 2, "ג"),
				new LookupItem(season - 3, "ד"),
				new LookupItem(season - 4, "ה"),
				new LookupItem(season - 5, "ו"),
				new LookupItem(season - 6, "ז"),
				new LookupItem(season - 7, "ח"),
				new LookupItem(season - 8, "ט"),
				new LookupItem(season - 9, "י"),
				new LookupItem(season - 10, "י\"א"),
				new LookupItem(season - 11, "י\"ב"),
				new LookupItem(season - 12, "י\"ג"),
				new LookupItem(season - 13, "י\"ד")
			};
		}

		public static int ToRelativeGrade(int grade)
		{
			return season - (grade - 1);
		}

		public static int ToGrade(int relativeGrade)
		{
			return season - relativeGrade + 1;
		}

		public GradeTypeLookup(bool relative)
		{
			_relative = relative;
		}

		private bool _relative;
		public bool Relative
		{
			get { return _relative; }
			set { _relative = value; }
		}

		public override string Lookup(int id)
		{
			return this[id].Text;
		}

		public override LookupItem this[int id]
		{
			get
			{
				if (_relative)
				{
					if (id < (season - relativeGrades.Length - 1))
						return new LookupItem(id, "סיים");
					if (id > season)
							return new LookupItem(id, "גן");
					if ((season - id) >= 0 && (season - id) < relativeGrades.Length)
						return relativeGrades[season - id];
				}
				
				if (id <= 0)
					return new LookupItem(id, "גן");
				if (id > grades.Length)
					return new LookupItem(id, "אוניברסיטה");
				try
				{
					return grades[id-1];
				}
				catch (Exception e)
				{
					System.Diagnostics.Debug.WriteLine("failed to return grade lookup: "+e.Message+" index: "+id.ToString());
					System.Diagnostics.Debug.WriteLine(e.StackTrace);
					return new LookupItem(id, "-שגיאה-");
				}
			}
		}

		public override LookupItem[] Items
		{
			get
			{
				return _relative ? relativeGrades : grades;
			}
		}
	}
}
