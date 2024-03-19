using System;
using Sport.Data;

namespace Sport.Types
{
	[Flags]
	public enum SportType
	{
		None = 0,
		Competition = 1,
		Match = 2,
		Both = Competition | Match
	}

	public class SportTypeLookup : LookupType
	{
		public static LookupItem[] Types = new LookupItem[]
			{ 
				new LookupItem((int) SportType.Competition, "תחרויות"),
				new LookupItem((int) SportType.Match, "התמודדות"),
		};
		public static LookupItem GetItem(SportType type)
		{
			if (type == SportType.Competition ||
				type == SportType.Match)
				return Types[(int) type - 1];

			throw new ArgumentException("Unknown sport type id");

		}

		public override string Lookup(int id)
		{
			return this[id].Text;
		}

		public override LookupItem this[int id]
		{
			get
			{
				return GetItem((SportType) id);
			}
		}

		public override LookupItem[] Items
		{
			get
			{
				return Types;
			}
		}
	}
}
