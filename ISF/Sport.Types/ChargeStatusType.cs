using System;
using Sport.Data;

namespace Sport.Types
{
	public enum ChargeStatusType
	{
		NotPaid=1,
		Paid
	}
	
	public class ChargeStatusLookup : LookupType
	{
		public static LookupItem[] types = new LookupItem[]
			{ 
				new LookupItem((int) ChargeStatusType.NotPaid, "ма щемн"),
				new LookupItem((int) ChargeStatusType.Paid, "щемн"),
		};
		
		public override string Lookup(int id)
		{
			if (!IsLegal(id))
				throw new ArgumentException("Unknown charge status: "+id.ToString());
			return types[id-1].Text;
		}
		
		public override LookupItem this[int id]
		{
			get
			{
				if (!IsLegal(id))
					throw new ArgumentException("Unknown charge status: "+id.ToString());
				return types[id-1];
			}
		}
		
		private bool IsLegal(int id)
		{
			bool result=false;
			switch (id)
			{
				case (int) ChargeStatusType.NotPaid:
				case (int) ChargeStatusType.Paid:
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
	} //end class ChargeStatusType
}
