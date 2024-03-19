using System;
using Sport.Data;
using SportServices = Sport.Data.SportServices;

namespace Sport.Types
{
	public enum Sex
	{
		None = 0,
		Boys = 1,
		Girls = 2,
		Both = 3
	}

	public class CategoryTypeLookup : LookupType
	{
		public static readonly int AllBoys = 0xFFF;
		public static readonly int AllGirls = 0xFFF0000;
		public static readonly int All = AllBoys | AllGirls;
		public static readonly string[] Sexes = 
			{null, "תלמידים", "תלמידות", "תלמידים/ות"};

		public static readonly string[] Grades = 
			{"א'", "ב'", "ג'", "ד'", "ה'", "ו'", "ז'", "ח'", "ט'", "י'", "י\"א", "י\"ב", "י\"ג", "י\"ד" };

		private static string GradesToString(int grades)
		{
			string result = null;

			int grade = 0;
			int start;

			while (grades != 0)
			{
				while (grades != 0 && (grades & 0x1) == 0)
				{
					grade++;
					grades = grades >> 1;
				}

				if (grades != 0)
				{
					start = grade;

					while ((grades & 0x1) == 1)
					{
						grade++;
						grades = grades >> 1;
					}
					//throw new Exception("invalid grade start index: "+start.ToString());
					string g=FindGrade(start < 0 ? 0 : start); // Only from first grade to 12th grade
					if (start != (grade-1))
						g += "-" + FindGrade(grade > 12 ? 11 : grade - 1); // Only from first grade to 12th grade
					if (result == null)
						result = g;
					else
						result = result + ", " + g;
				}
			}
			
			return result;
		}

		/// <summary>
		/// convert single grade in given index to string - safe.
		/// </summary>
		public static string FindGrade(int index)
		{
			string result="";
			if (index < 0)
			{
				result = "גן";
				System.Diagnostics.Debug.WriteLine("invalid grade index: "+index.ToString());
			}
			else
			{
				if (index >= Grades.Length)
				{
					result = "אוניברסיטה";
					System.Diagnostics.Debug.WriteLine("invalid grade index: "+index.ToString());
				}
				else
				{
					result = Grades[index];
				}
			}
			return result;
		}

		public static string ToString(int category)
		{
			int boys = category & 0xFFFF;
			int girls = (category >> 16) & 0xFFFF;

			if (boys == girls)
			{
				if (boys == 0)
					return null;

				return GradesToString(boys) + " " + 
					Sexes[(int)Sex.Boys] + "/" + Sexes[(int)Sex.Girls];
			}

			if (boys == 0)
				return GradesToString(girls) + " " + Sexes[(int)Sex.Girls];
			if (girls == 0)
				return GradesToString(boys) + " " + Sexes[(int)Sex.Boys];

			return GradesToString(boys) + " " + Sexes[(int)Sex.Boys] + ", " +
				GradesToString(girls) + " " + Sexes[(int)Sex.Girls];
		}

		public static int ToCategory(Sex sex, int grade)
		{
			int r = 1 << grade;
			if (sex == Sex.Girls)
				r = r << 16;
			else if (sex == Sex.Both)
				r |= r << 16;
			return r;
		}

		public static int ToCategory(int grade)
		{
			int r = 1 << grade;
			return r | (r << 16);
		}

		public static int ToCategory(int fromGrade, int toGrade, Sex sexes)
		{
			int grades = 0;
			for (int n = fromGrade; n <= toGrade; n++)
			{
				grades |= 1 << n;
			}

			switch (sexes)
			{
				case (Sex.Boys):
					return grades;
				case (Sex.Girls):
					return grades << 16;
				case (Sex.Both):
					return grades | (grades << 16);
			}

			return 0;
		}

		public static bool Contains(int category, Sex sex, int grade)
		{
			return (category & ToCategory(sex, grade)) != 0;
		}

		public static bool Contains(int category, int grade)
		{
			return (category & ToCategory(grade)) != 0;
		}

		public static bool Compare(int category, int iValue, CategoryCompareType compareType)
		{
			switch (compareType)
			{
				case CategoryCompareType.Grade:
					// Grade compare
					int r = 1 << (Sport.Core.Session.Season - iValue); // make category grade
					int g = (r << 16) | r; // make both sexes
					return (g & category) != 0; // return whether none match
				case CategoryCompareType.Partial:
					return (iValue & category) != 0;
			}

			// Full category compare (defualt)
			return (iValue & ~(int) category) == 0;
		}

		public static LookupItem ToLookupItem(int category)
		{
			return new LookupItem(category, ToString(category));
		}

		public override LookupItem this[int id]
		{
			get
			{
				return ToLookupItem(id);
			}
		}

		public override string Lookup(int id)
		{
			return ToString(id);
		}
	} //end class CategoryTypeLookup


	public class CategoryFields : LookupEntityField
	{
		#region Properties

		private int _fromIndex;
		public int FromIndex
		{
			get { return _fromIndex; }
		}

		private int _toIndex;
		public int ToIndex
		{
			get { return _toIndex; }
		}

		private int _sexIndex;
		public int SexIndex
		{
			get { return _sexIndex; }
		}

		#endregion

		#region Constructor

		// EntityRelationEntityField constructor, receives the name
		// of the relative entity type and the field index of
		// the relative field
		public CategoryFields(EntityType type, int index,
			int fromIndex, int toIndex, int sexIndex)
			: base(type, index, new CategoryTypeLookup())
		{
			_fromIndex = fromIndex;
			_toIndex = toIndex;
			_sexIndex = sexIndex;
		}

		#endregion

		public override object GetValue(Entity e)
		{
			int from = _fromIndex == -1 ? 0 : (int) e.Fields[_fromIndex] - 1;
			int to = _toIndex == -1 ? 11 : (int) e.Fields[_toIndex] - 1;
			Sex sex = _sexIndex == -1 ? Sex.Both : (Sex) e.Fields[_sexIndex];

			return CategoryTypeLookup.ToCategory(from, to, sex);
		}

		// Overrides SetValue to disable change to the relative
		// field
		public override void SetValue(EntityEdit e, object value)
		{
			throw new EntityException("Cannot edit a category fields field");
		}
	}

	/// <summary>
	/// CategoryCompareType enum the type of
	/// field compared with the category and
	/// the method of comparison
	/// </summary>
	public enum CategoryCompareType
	{
		Full,			// The whole field's category exist in the category
		Partial,		// The field's category instersects with the category
		Grade			// The field's grade is in the category
	}

	[Serializable]
	public class CategoryFilterField : EntityFilterField
	{
		private CategoryCompareType _compareType;
		public CategoryCompareType CompareType
		{
			get { return _compareType; }
			set { _compareType = value; }
		}


		public CategoryFilterField(int field, int category)
			: this(field, category, CategoryCompareType.Full)
		{
		}

		public CategoryFilterField(int field, int category, CategoryCompareType compareType)
			: base(field, category)
		{
			_compareType = compareType;
		}

		//private static int season = 56;  what for????????
		
		public override bool CompareValue(Entity e)
		{
			object value = e.EntityType.Fields[_field].GetValue(e);
			int iValue=0;
			if (value != null)
				iValue = (int) value;
			return CategoryTypeLookup.Compare((int) _value, iValue, _compareType);
			/*
			switch (_compareType)
			{
				case (CategoryCompareType.Grade):
					// Grade compare
					int r = 1 << (Sport.Core.Session.Season - iValue); // make category grade
					int g = (r << 16) | r; // make both sexes
					return (g & (int) _value) != 0; // return whether none match
				case (CategoryCompareType.Partial):
					return (iValue & (int) _value) != 0;
			}

			// Full category compare (defualt)
			return (iValue & ~(int) _value) == 0;
			*/
		}

		public override EntityFilterField ToArrayFilterField()
		{
			return null;
		}

		public override SportServices.FilterField ToFilterField(EntityType entityType)
		{
			return null;
		}

	}
}
