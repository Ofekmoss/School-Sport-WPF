using System;
using Sport.Data;

namespace Sport.Types
{
	[Flags]
	public enum CompetitionType
	{
		None = 0,
		Personal = 1,
		Group = 2,
		MultiCompetition = 3,
		Both = Personal | Group
	}

	public class CompetitionTypeLookup : LookupType
	{
		public static LookupItem[] Types = new LookupItem[]
		{ 
			new LookupItem((int) CompetitionType.Personal, "אישי"),
			new LookupItem((int) CompetitionType.Group, "קבוצתי"),
			new LookupItem((int) CompetitionType.MultiCompetition, "קרב רב")
		};
		public static LookupItem GetItem(CompetitionType type)
		{
			if ((type == CompetitionType.Personal)||
				(type == CompetitionType.Group)||
				(type == CompetitionType.MultiCompetition))
			{
				return Types[(int) type - 1];
			}
			
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
				return GetItem((CompetitionType) id);
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
