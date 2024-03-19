using System;
using Sport.Data;

namespace Sport.Entities
{
	public class Credit : EntityBase
	{
		public static readonly string TypeName = "credit";
		public static readonly EntityType Type;

		public enum Fields
		{
			Id = 0,
			Region,
			Receipt,
			Account,
			Sum,
			LastModified,
			FieldCount
		}

		static Credit()
		{
			Type = new EntityType(TypeName, (int) Fields.FieldCount, (int) Fields.FieldCount, new FieldEntityId((int) Fields.Id));
			
			//Data fields:
			Type[(int) Fields.Id] = new NumberEntityField(Type, (int) Fields.Id);
			Type[(int) Fields.Id].CanEdit = false;
			Type[(int) Fields.Region] = new EntityEntityField(Type, (int) Fields.Region, Region.TypeName);
			Type[(int) Fields.Region].MustExist = true;
			Type[(int) Fields.Region].CanEdit = false;
			Type[(int) Fields.Receipt] = new EntityEntityField(Type, (int) Fields.Receipt, Receipt.TypeName);
			Type[(int) Fields.Receipt].MustExist = true;
			Type[(int) Fields.Receipt].CanEdit = false;
			Type[(int) Fields.Account] = new EntityEntityField(Type, (int) Fields.Account, Account.TypeName);
			Type[(int) Fields.Account].MustExist = true;
			Type[(int) Fields.Account].CanEdit = false;
			Type[(int) Fields.Sum] = new NumberEntityField(Type, (int) Fields.Sum, 0, 9999999, 7, 2);
			Type[(int) Fields.Sum].MustExist = true;
			Type[(int) Fields.LastModified] = new DateTimeEntityField(Type, (int) Fields.LastModified, "dd/MM/yyyy HH:mm:ss");
			Type[(int) Fields.LastModified].CanEdit = false;
			
			Type.NameField = new FormatEntityField(Type, "{0}: {1} {2}", 
				new int[] { (int) Fields.Receipt, (int) Fields.Account, (int) Fields.Sum });
			Type.DateLastModified = Type[(int) Fields.LastModified];
		}

		public Credit(Entity entity)
			: base(entity)
		{
		}

		public Credit(EntityEdit entityEdit)
			: base(entityEdit)
		{
		}

		public Credit(int paymentID)
			: base(Type, paymentID)
		{
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
				SetValue((int) Fields.Region, value.Entity); 
			}
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
				SetValue((int) Fields.Account, value.Entity); 
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
	}
}
