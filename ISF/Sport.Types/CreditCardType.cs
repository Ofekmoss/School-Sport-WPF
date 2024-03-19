using System;
using Sport.Data;

namespace Sport.Types
{
	public enum CreditCardType
	{
		Visa = 0,
		MasterCard,
		AmericanExpress,
		IsraCard
	}

	public class CreditCardTypeLookup : LookupType
	{
		public static LookupItem[] types = new LookupItem[]
		{ 
			new LookupItem((int) CreditCardType.Visa, "ויזה"),
			new LookupItem((int) CreditCardType.MasterCard, "מסטרקארד"),
			new LookupItem((int) CreditCardType.AmericanExpress, "אמריקן אקספרס"), 
			new LookupItem((int) CreditCardType.IsraCard, "ישראכרט")
		};

		public override string Lookup(int id)
		{
			if ((id < 0) || (id >= types.Length))
				return "-לא ידוע-";
			return types[id].Text;
		}

		public override LookupItem this[int id]
		{
			get
			{
				if ((id < 0) || (id >= types.Length))
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
