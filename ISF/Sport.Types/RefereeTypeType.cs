using System;
using Sport.Data;

namespace Sport.Types
{
	public enum RefereeType
	{
		Junior=0,
		Senior
	}
	
	public class RefereeTypeLookup : LookupType
	{
		public static LookupItem[] types = new LookupItem[]
			{ 
				new LookupItem((int) RefereeType.Junior, "זוטר"),
				new LookupItem((int) RefereeType.Senior, "בכיר")
		};
		
		public override string Lookup(int id)
		{
			if (!IsLegal(id))
				throw new ArgumentException("Unknown Referee type: "+id.ToString());
			return types[id].Text;
		}
		
		public override LookupItem this[int id]
		{
			get
			{
				if (!IsLegal(id))
					throw new ArgumentException("Unknown Referee type: "+id.ToString());
				return types[id];
			}
		}
		
		private bool IsLegal(int id)
		{
			bool result=false;
			switch (id)
			{
				case (int) RefereeType.Junior:
				case (int) RefereeType.Senior:
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
	}  //end class RefereeTypeLookup
}
