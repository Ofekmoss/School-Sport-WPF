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
			new LookupItem((int) PaymentType.Cheque, "�����"),
			new LookupItem((int) PaymentType.BankTransfer, "����� ������"),
			new LookupItem((int) PaymentType.Cash, "�����"), 
			new LookupItem((int) PaymentType.CreditCard, "����� �����")
		};

		public override string Lookup(int id)
		{
			if ((id < 0)||(id >= types.Length))
				return "-�� ����-";
			return types[id].Text;
		}

		public override LookupItem this[int id]
		{
			get
			{
				if ((id < 0)||(id >= types.Length))
					return new LookupItem(id, "-�� ����-");
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
