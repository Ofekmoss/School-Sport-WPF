using System;
using Sport.Data;

namespace Sport.Types
{
	public enum PlayerStatusType
	{
		Registered=1,
		Confirmed,
		Not_Confirmed
	}
	
	/// <summary>
	/// Summary description for PlayerStatusType.
	/// </summary>
	public class PlayerStatusLookup : LookupType
	{
		public static LookupItem[] types = new LookupItem[]
			{ 
				new LookupItem((int) PlayerStatusType.Registered, "רשום"),
				new LookupItem((int) PlayerStatusType.Confirmed, "מאושר"),
				new LookupItem((int) PlayerStatusType.Not_Confirmed, "לא מאושר"),
		};
		
		public override LookupItem this[int id]
		{
			get
			{
				if (!IsLegal(id))
					throw new ArgumentException("Unknown player status: "+id.ToString());
				return types[id - 1];
			}
		}
		
		public override string Lookup(int id)
		{
			return this[id].Text;
		}
		
		private bool IsLegal(int id)
		{
			bool result=false;
			switch (id)
			{
				case (int) PlayerStatusType.Registered:
				case (int) PlayerStatusType.Confirmed:
				case (int) PlayerStatusType.Not_Confirmed:
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
	} //end class PlayerStatusLookup
}
