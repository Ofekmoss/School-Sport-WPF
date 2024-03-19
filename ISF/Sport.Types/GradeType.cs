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
				new LookupItem(1, "�"),
				new LookupItem(2, "�"),
				new LookupItem(3, "�"),
				new LookupItem(4, "�"),
				new LookupItem(5, "�"),
				new LookupItem(6, "�"),
				new LookupItem(7, "�"),
				new LookupItem(8, "�"),
				new LookupItem(9, "�"),
				new LookupItem(10, "�"),
				new LookupItem(11, "�\"�"),
				new LookupItem(12, "�\"�"),
				new LookupItem(13, "�\"�"),
				new LookupItem(14, "�\"�")
			};


		public static void ResetRelativeGrades()
		{
			relativeGrades = new LookupItem[] { 
				new LookupItem(season, "�"),
				new LookupItem(season - 1, "�"),
				new LookupItem(season - 2, "�"),
				new LookupItem(season - 3, "�"),
				new LookupItem(season - 4, "�"),
				new LookupItem(season - 5, "�"),
				new LookupItem(season - 6, "�"),
				new LookupItem(season - 7, "�"),
				new LookupItem(season - 8, "�"),
				new LookupItem(season - 9, "�"),
				new LookupItem(season - 10, "�\"�"),
				new LookupItem(season - 11, "�\"�"),
				new LookupItem(season - 12, "�\"�"),
				new LookupItem(season - 13, "�\"�")
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
						return new LookupItem(id, "����");
					if (id > season)
							return new LookupItem(id, "��");
					if ((season - id) >= 0 && (season - id) < relativeGrades.Length)
						return relativeGrades[season - id];
				}
				
				if (id <= 0)
					return new LookupItem(id, "��");
				if (id > grades.Length)
					return new LookupItem(id, "����������");
				try
				{
					return grades[id-1];
				}
				catch (Exception e)
				{
					System.Diagnostics.Debug.WriteLine("failed to return grade lookup: "+e.Message+" index: "+id.ToString());
					System.Diagnostics.Debug.WriteLine(e.StackTrace);
					return new LookupItem(id, "-�����-");
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
