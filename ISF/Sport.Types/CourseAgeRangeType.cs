using System;
using Sport.Data;

namespace Sport.Types
{
	public enum CourseAgeRange
	{
		None=0,
		BoysNineTwelve,
		GirlsNineTwelve,
		BoysThirteenFifteen,
		GirlsThirteenFifteen,
		BoysSixteenEighteen,
		GirlsSixteenEighteen,
		BoysOverEighteen,
		GirlsOverEighteen
	}
	
	/// <summary>
	/// Summary description for CourseAgeRangeType.
	/// </summary>
	public class CourseAgeRangeLookup : LookupType
	{
		public static LookupItem[] types = new LookupItem[]
			{ 
				new LookupItem((int) CourseAgeRange.None, "לא מוגדר"),
				new LookupItem((int) CourseAgeRange.BoysNineTwelve, "בנים 9-12"),
				new LookupItem((int) CourseAgeRange.GirlsNineTwelve, "בנות 9-12"),
				new LookupItem((int) CourseAgeRange.BoysThirteenFifteen, "בנים 13-15"),
				new LookupItem((int) CourseAgeRange.GirlsThirteenFifteen, "בנות 13-15"),
				new LookupItem((int) CourseAgeRange.BoysSixteenEighteen, "בנים 16-18"),
				new LookupItem((int) CourseAgeRange.GirlsSixteenEighteen, "בנות 16-18"),
				new LookupItem((int) CourseAgeRange.BoysOverEighteen, "בנים 18+"),
				new LookupItem((int) CourseAgeRange.GirlsOverEighteen, "בנות 18+")
			};
		
		public override LookupItem this[int id]
		{
			get
			{
				if (!IsLegal(id))
					return new LookupItem(-1, "לא ידוע");
				//throw new ArgumentException("Unknown functionary type: "+id.ToString());
				return types[id];
			}
		}
		
		public override string Lookup(int id)
		{
			return this[id].Text;
		}
		
		private bool IsLegal(int id)
		{
			int count = Enum.GetValues(typeof(CourseAgeRange)).Length;
			return ((id >= 0)&&(id < count));
		}
		
		public override LookupItem[] Items
		{
			get
			{
				return types;
			}
		}
	}
}
