using System;
using Sport.Data;

namespace Sport.Types
{
	public enum ChampionshipType
	{
		InitialPlanning = 0,
		TeamRegister,
		PlayerRegister,
		Confirmed
	}

	public class ChampionshipStatusLookup : LookupTable
	{
		public static LookupItem[] types = new LookupItem[]
			{ 
				new LookupItem((int) ChampionshipType.InitialPlanning, "בתכנון"),
				new LookupItem((int) ChampionshipType.TeamRegister, "רישום קבוצות"),
				new LookupItem((int) ChampionshipType.PlayerRegister, "רישום שחקנים"), 
				new LookupItem((int) ChampionshipType.Confirmed, "מאושרת")
			};

		public override string Lookup(int id)
		{
			if (!IsLegal(id))
				throw new ArgumentException("Unknown championship type id: "+id.ToString());
			return types[id].Text;
		}

		public override LookupItem this[int id]
		{
			get
			{
				if (!IsLegal(id))
					throw new ArgumentException("Unknown championship type id: "+id.ToString());
				return types[id];
			}
		}

		private bool IsLegal(int id)
		{
			bool result=false;
			switch (id)
			{
				case (int) ChampionshipType.Confirmed:
				case (int) ChampionshipType.InitialPlanning:
				case (int) ChampionshipType.PlayerRegister:
				case (int) ChampionshipType.TeamRegister:
					result = true;
					break;
			}
			return result;
		}

		public override LookupItem[] Items
		{
			get
			{
				return types;
			}
		}
	} //end class ChampionshipStatusLookup
}
