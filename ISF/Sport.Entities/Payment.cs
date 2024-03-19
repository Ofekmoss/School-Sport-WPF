using System;
using Sport.Data;
using Sport.Types;

namespace Sport.Entities
{
	public class Payment : EntityBase
	{
		public static readonly string TypeName = "payment";
		public static readonly EntityType Type;
		
		public enum Fields
		{
			Id = 0,
			Receipt,
			Type,
			Sum,
			Bank,
			BankBranch,
			BankAccount,
			Reference,
			Date,
			CreditCardType,
			CreditCardNumber,
			CreditCardExpire,
			CreditCardPayments,
			LastModified,
			FieldCount
		}

		static Payment()
		{
			Type = new EntityType(TypeName, (int) Fields.FieldCount, (int) Fields.FieldCount, new FieldEntityId((int) Fields.Id));
			
			//Data fields:
			Type[(int) Fields.Id] = new NumberEntityField(Type, (int) Fields.Id);
			Type[(int) Fields.Id].CanEdit = false;
			Type[(int) Fields.Receipt] = new EntityEntityField(Type, (int) Fields.Receipt, Receipt.TypeName);
			Type[(int) Fields.Receipt].MustExist = true;
			Type[(int) Fields.Receipt].CanEdit = false;
			Type[(int) Fields.Type] = new LookupEntityField(Type, (int) Fields.Type, new PaymentTypeLookup());
			Type[(int) Fields.Type].MustExist = true;
			Type[(int) Fields.Sum] = new NumberEntityField(Type, (int) Fields.Sum, 0, 99999999, 7, 2);
			Type[(int) Fields.Sum].MustExist = true;
			Type[(int) Fields.Bank] = new NumberEntityField(Type, (int) Fields.Bank, 0, 999, 3, 0);
			Type[(int) Fields.BankBranch] = new NumberEntityField(Type, (int) Fields.BankBranch, 0, 9999, 4, 0);
			Type[(int) Fields.Date] = new DateTimeEntityField(Type, (int) Fields.Date, "dd/MM/yyyy");
			Type[(int) Fields.Date].MustExist = true;
			Type[(int)Fields.CreditCardType] = new LookupEntityField(Type, (int)Fields.CreditCardType, new CreditCardTypeLookup());
			Type[(int)Fields.CreditCardNumber] = new NumberEntityField(Type, (int)Fields.CreditCardNumber, 0, 9999, 4, 0);
			Type[(int)Fields.CreditCardExpire] = new DateTimeEntityField(Type, (int)Fields.CreditCardExpire, "MM/yyyy");
			Type[(int)Fields.CreditCardPayments] = new NumberEntityField(Type, (int)Fields.CreditCardPayments, 1, 999, 4, 0);
			Type[(int) Fields.LastModified] = new DateTimeEntityField(Type, (int) Fields.LastModified, "dd/MM/yyyy HH:mm:ss");
			Type[(int) Fields.LastModified].CanEdit = false;
			
			Type.NameField = new FormatEntityField(Type, "{0}: {1} {2}", 
				new int[] { (int) Fields.Receipt, (int) Fields.Type, (int) Fields.Sum });
			Type.DateLastModified = Type[(int) Fields.LastModified];
		}

		public Payment(Entity entity)
			: base(entity)
		{
		}

		public Payment(EntityEdit entityEdit)
			: base(entityEdit)
		{
		}

		public Payment(int paymentID)
			: base(Type, paymentID)
		{
		}

		public Receipt Receipt
		{
			get 
			{
				Entity entity = GetEntityValue((int) Fields.Receipt);
				if (entity == null)
					return null;
				return new Receipt(entity);
			}
			set 
			{ 
				SetValue((int) Fields.Receipt, value.Entity); 
			}
		}
		
		public double Sum
		{
			get
			{
				object value=GetValue((int) Fields.Sum);
				return Common.Tools.CDblDef(value, 0);
			}
			set { SetValue((int) Fields.Sum, value); }
		}

		public PaymentType PaymentType
		{
			get { return (PaymentType) GetValue((int) Fields.Type); }
			set { SetValue((int) Fields.Type, (int) value); }
		}

		public int Bank
		{
			get
			{
				object value=GetValue((int) Fields.Bank);
				return Common.Tools.CIntDef(value, 0);
			}
			set 
			{ 
				if (value == 0)
					SetValue((int) Fields.Bank, null);
				else
					SetValue((int) Fields.Bank, value); 
			}
		}

		public int BankBranch
		{
			get
			{
				object value=GetValue((int) Fields.BankBranch);
				return Common.Tools.CIntDef(value, 0);
			}
			set 
			{ 
				if (value == 0)
					SetValue((int) Fields.BankBranch, null);
				else
					SetValue((int) Fields.BankBranch, value); 
			}
		}

		public string BankAccount
		{
			get
			{
				return (string) GetValue((int) Fields.BankAccount);
			}
			set 
			{ 
				SetValue((int) Fields.BankAccount, value);
			}
		}

		public string Reference
		{
			get
			{
				return (string) GetValue((int) Fields.Reference);
			}
			set 
			{ 
				SetValue((int) Fields.Reference, value);
			}
		}

		public DateTime Date
		{
			get
			{
				object value=GetValue((int) Fields.Date);
				return Common.Tools.ToDateDef(value, DateTime.MinValue);
			}
			set { SetValue((int) Fields.Date, value); }
		}

		public CreditCardType CreditCardType
		{
			get
			{
				object oValue = GetValue((int)Fields.CreditCardType);
				return oValue == null ? CreditCardType.Visa : (CreditCardType)oValue;
			}
			set { SetValue((int)Fields.CreditCardType, (int)value); }
		}

		public int CreditCardNumber
		{
			get
			{
				object value = GetValue((int)Fields.CreditCardNumber);
				return Common.Tools.CIntDef(value, 0);
			}
			set
			{
				if (value == 0)
					SetValue((int)Fields.CreditCardNumber, null);
				else
					SetValue((int)Fields.CreditCardNumber, value);
			}
		}

		public DateTime CreditCardExpire
		{
			get
			{
				object value = GetValue((int)Fields.CreditCardExpire);
				return Common.Tools.ToDateDef(value, DateTime.MinValue);
			}
			set { SetValue((int)Fields.CreditCardExpire, value); }
		}

		public int CreditCardPayments
		{
			get
			{
				object value = GetValue((int)Fields.CreditCardPayments);
				return Common.Tools.CIntDef(value, 0);
			}
			set
			{
				if (value == 0)
					SetValue((int)Fields.CreditCardPayments, null);
				else
					SetValue((int)Fields.CreditCardPayments, value);
			}
		}

		public override string GetCustomText(int field)
		{
			if (field == (int) Fields.Bank)
			{
				int bankCode = this.Bank;
				string bankName = bankCode.ToString();
				for (int i = 0; i < Core.Data.bankItems.Length; i++)
				{
					Core.Data.BankItem item = Core.Data.bankItems[i];
					if (item.BankCode == bankCode)
					{
						bankName += " - " + item.BankName;
						break;
					}
				}
				return bankName;
			}
			else if (field == (int)Fields.CreditCardNumber)
			{
				string number = this.CreditCardNumber.ToString();
				return number.PadLeft(4, '0');
			}
			
			return base.GetCustomText(field);
		}
	}
}
