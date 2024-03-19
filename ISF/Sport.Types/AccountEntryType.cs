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
			new LookupItem((int) AccountEntryType.Debit, "����"),
			new LookupItem((int) AccountEntryType.Credit, "�����")
		};

		public override string Lookup(int id)
		{
			if ((id < 1)||(id > types.Length))
				return "-�� ����-";
			return types[id - 1].Text;
		}

		public override LookupItem this[int id]
		{
			get
			{
				if ((id < 1)||(id > types.Length))
					return new LookupItem(id, "-�� ����-");
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
