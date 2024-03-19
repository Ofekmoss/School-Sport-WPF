using System;
using Sport.Data;

namespace Sport.Types
{
	public enum TeamStatusType
	{
		Registered=1,
		Confirmed
	}

	public class TeamStatusLookup : LookupType
	{
		public static LookupItem[] types = new LookupItem[]
			{ 
				new LookupItem((int) TeamStatusType.Registered, "רשומה"),
				new LookupItem((int) TeamStatusType.Confirmed, "מאושרת")
		};

		public override string Lookup(int id)
		{
			if (!IsLegal(id))
				throw new ArgumentException("Unknown team status: "+id.ToString());
			return types[id-1].Text;
		}

		public override LookupItem this[int id]
		{
			get
			{
				if (!IsLegal(id))
					throw new ArgumentException("Unknown team status: "+id.ToString());
				return types[id - 1];
			}
		}

		private bool IsLegal(int id)
		{
			bool result=false;
			switch (id)
			{
				case (int) TeamStatusType.Registered:
				case (int) TeamStatusType.Confirmed:
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
	} //end class TeamStatusLookup
}
