using System;
using Sport.Data;

namespace Sport.Entities
{
	public class Receipt : EntityBase
	{
		public static readonly string TypeName = "receipt";
		public static readonly EntityType Type;

		public enum Fields
		{
			Id = 0,
			Number,
			Region,
			Account,
			Sum,
			Date,
			Remarks,
			Season, 
			LastModified,
			FieldCount
		}

		static Receipt()
		{
			Type = new EntityType(TypeName, (int) Fields.FieldCount, (int) Fields.FieldCount, new FieldEntityId((int) Fields.Id));
			
			//Data fields:
			Type[(int) Fields.Id] = new NumberEntityField(Type, (int) Fields.Id);
			Type[(int) Fields.Id].CanEdit = false;
			Type[(int) Fields.Number].CanEdit = false;
			Type[(int) Fields.Region] = new EntityEntityField(Type, (int) Fields.Region, Region.TypeName);
			Type[(int) Fields.Region].MustExist = true;
			Type[(int) Fields.Account] = new EntityEntityField(Type, (int) Fields.Account, Account.TypeName);
			Type[(int) Fields.Account].MustExist = true;
			Type[(int) Fields.Sum] = new NumberEntityField(Type, (int) Fields.Sum, 0, 9999999, 7, 2);
			Type[(int) Fields.Sum].MustExist = true;
			Type[(int) Fields.Date] = new DateTimeEntityField(Type, (int) Fields.Date, "dd/MM/yyyy");
			Type[(int) Fields.Date].MustExist = true;
			Type[(int) Fields.Remarks] = new TextEntityField(Type, (int) Fields.Remarks, true);
			Type[(int) Fields.Season] = new EntityEntityField(Type, (int) Fields.Season, Season.TypeName);
			//Type[(int) Fields.Season].CanEdit = false;
			Type[(int) Fields.LastModified] = new DateTimeEntityField(Type, (int) Fields.LastModified, "dd/MM/yyyy HH:mm:ss");
			Type[(int) Fields.LastModified].CanEdit = false;
			
			Type.NameField = Type[(int) Fields.Number];
			Type.DateLastModified = Type[(int) Fields.LastModified];
		}

		public Receipt(Entity entity)
			: base(entity)
		{
		}

		public Receipt(EntityEdit entityEdit)
			: base(entityEdit)
		{
		}

		public Receipt(int receiptId)
			: base(Type, receiptId)
		{
		}

		public string Number
		{
			get { return (string) GetValue((int) Fields.Number); }
			set { SetValue((int) Fields.Number, value); }
		}

		public Region Region
		{
			get 
			{
				Entity entity = GetEntityValue((int) Fields.Region);
				if (entity == null)
					return null;
				return new Region(entity);
			}
			set 
			{ 
				SetValue((int) Fields.Region, (value == null) ? null : value.Entity); 
			}
		}

		public Account Account
		{
			get 
			{
				Entity entity = GetEntityValue((int) Fields.Account);
				if (entity == null)
					return null;
				return new Account(entity);
			}
			set 
			{ 
				SetValue((int) Fields.Account, (value == null) ? null : value.Entity); 
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

		public DateTime Date
		{
			get
			{
				object value=GetValue((int) Fields.Date);
				return Common.Tools.ToDateDef(value, DateTime.MinValue);
			}
			set { SetValue((int) Fields.Date, value); }
		}

		public string Remarks
		{
			get { return (string) GetValue((int) Fields.Remarks); }
			set { SetValue((int) Fields.Remarks, value); }
		}
		
		public Payment[] GetPayments()
		{
			EntityFilter filter=new EntityFilter(
				(int) Entities.Payment.Fields.Receipt, this.Id);
			Entity[] payments=Entities.Payment.Type.GetEntities(filter);
			Payment[] result=new Payment[payments.Length];
			for (int i=0; i<payments.Length; i++)
			{
				result[i] = new Payment(payments[i]);
			}
			return result;
		}

		public Credit[] GetCredits()
		{
			EntityFilter filter=new EntityFilter(
				(int) Entities.Credit.Fields.Receipt, this.Id);
			Entity[] credits=Entities.Credit.Type.GetEntities(filter);
			Credit[] result=new Credit[credits.Length];
			for (int i=0; i<credits.Length; i++)
			{
				result[i] = new Credit(credits[i]);
			}
			return result;
		}

		public static DataServices.ReceiptData[] GetReceiptsByNumber(int firstReceiptNumber)
		{
			DataServices.DataService service = new DataServices.DataService();
			return service.GetReceiptData(firstReceiptNumber, -1);

		}

		public static DataServices.ReceiptData_Basic[] GetReceiptsByNumber_Basic(int firstReceiptNumber)
		{
			DataServices.DataService service = new DataServices.DataService();
			return service.GetReceiptData_Basic(firstReceiptNumber, -1);

		}
	}
}
