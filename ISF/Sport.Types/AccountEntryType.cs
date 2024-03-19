using System;
using Sport.Data;

namespace Sport.Types
{
	public enum AccountEntryType
	{
		Debit = 1,
		Credit = 2
	}

	public class AccountEntryTypeLookup : LookupType
	{
		public static LookupItem[] types = new LookupItem[]
		{ 
			new LookupItem((int) AccountEntryType.Debit, "חיוב"),
			new LookupItem((int) AccountEntryType.Credit, "זיכוי")
		};

		public override string Lookup(int id)
		{
			if ((id < 1)||(id > types.Length))
				return "-לא ידוע-";
			return types[id - 1].Text;
		}

		public override LookupItem this[int id]
		{
			get
			{
				if ((id < 1)||(id > types.Length))
					return new LookupItem(id, "-לא ידוע-");
				return types[id - 1];
			}
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
