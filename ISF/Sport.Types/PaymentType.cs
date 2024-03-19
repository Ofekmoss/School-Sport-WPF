using System;
using Sport.Data;

namespace Sport.Types
{
	public enum PaymentType
	{
		Cheque=0,
		BankTransfer,
		Cash, 
		CreditCard
	}

	public class PaymentTypeLookup : LookupType
	{
		public static LookupItem[] types = new LookupItem[]
		{ 
			new LookupItem((int) PaymentType.Cheque, "המחאה"),
			new LookupItem((int) PaymentType.BankTransfer, "העברה בנקאית"),
			new LookupItem((int) PaymentType.Cash, "מזומן"), 
			new LookupItem((int) PaymentType.CreditCard, "כרטיס אשראי")
		};

		public override string Lookup(int id)
		{
			if ((id < 0)||(id >= types.Length))
				return "-לא ידוע-";
			return types[id].Text;
		}

		public override LookupItem this[int id]
		{
			get
			{
				if ((id < 0)||(id >= types.Length))
					return new LookupItem(id, "-לא ידוע-");
				return types[id];
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
