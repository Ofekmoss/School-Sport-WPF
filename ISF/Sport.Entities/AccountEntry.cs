using System;
using Sport.Data;
using Sport.Types;

namespace Sport.Entities
{
	public class AccountEntry : EntityBase
	{
		public static readonly string TypeName = "accountentry";
		public static readonly EntityType Type;

		public enum Fields
		{
			Id = 0,
			Region,
			Account,
			EntryType,
			Sum,
			Description,
			Additional,
			EntryDate,
			Season, 
			LastModified,
			FieldCount
		}

		static AccountEntry()
		{
			Type = new EntityType(TypeName, (int) Fields.FieldCount, (int) Fields.FieldCount, new FieldEntityId((int) Fields.Id));

			//Data fields:
			Type[(int) Fields.Id] = new NumberEntityField(Type, (int) Fields.Id);
			Type[(int) Fields.Id].CanEdit = false;
			Type[(int) Fields.Region] = new EntityEntityField(Type, (int) Fields.Region, Region.TypeName);
			Type[(int) Fields.Region].CanEdit = false;
			Type[(int) Fields.Account] = new EntityEntityField(Type, (int) Fields.Account, Account.TypeName);
			Type[(int) Fields.Account].CanEdit = false;
			Type[(int) Fields.EntryType] = new LookupEntityField(Type, (int) Fields.EntryType, new AccountEntryTypeLookup());
			Type[(int) Fields.EntryType].CanEdit = false;
			Type[(int) Fields.Sum] = new NumberEntityField(Type, (int) Fields.Sum, -9999999, 9999999, 7, 2);
			Type[(int) Fields.Sum].CanEdit = false;
			Type[(int) Fields.Description].CanEdit = false;
			Type[(int) Fields.Additional].CanEdit = false;
			Type[(int) Fields.EntryDate] = new DateTimeEntityField(Type, (int) Fields.EntryDate, "dd/MM/yyyy");
			Type[(int) Fields.EntryDate].CanEdit = false;
			Type[(int) Fields.Season] = new EntityEntityField(Type, (int) Fields.Season, Season.TypeName);
			Type[(int) Fields.Season].CanEdit = false;
			Type[(int) Fields.LastModified] = new DateTimeEntityField(Type, (int) Fields.LastModified, "dd/MM/yyyy HH:mm:ss");
			Type[(int) Fields.LastModified].CanEdit = false;
			
			Type.NameField = Type[(int) Fields.Description];
			Type.DateLastModified = Type[(int) Fields.LastModified];
		}

		public AccountEntry(Entity entity)
			: base(entity)
		{
		}

		public AccountEntry(EntityEdit entityEdit)
			: base(entityEdit)
		{
		}

		public AccountEntry(int accountEntryId)
			: base(Type, accountEntryId)
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
		}

		public AccountEntryType EntryType
		{
			get { return (AccountEntryType) GetValue((int) Fields.EntryType); }
		}

		public Receipt Receipt
		{
			get
			{
				AccountEntryType entryType = EntryType;
				if (entryType == AccountEntryType.Credit)
				{
					object receiptId = GetValue((int) Fields.Additional);
					if (receiptId != null)
					{
						return new Receipt((int) receiptId);
					}
				}

				return null;
			}
		}

		public Product Product
		{
			get
			{
				AccountEntryType entryType = EntryType;
				if (entryType == AccountEntryType.Debit)
				{
					object productId = GetValue((int) Fields.Additional);
					if (productId != null)
					{
						return new Product((int) productId);
					}
				}

				return null;
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
		}
		
		public ChampionshipCategory Category
		{
			get
			{
				if (this.EntryType == Types.AccountEntryType.Debit)
				{
					int chargeID = (this.Id / 10);
					try
					{
						Charge charge = new Charge(chargeID);
						return charge.ChampionshipCategory;
					}
					catch
					{
					}
				}

				return null;
			}
		}

		public string Description
		{
			get	{ return Common.Tools.CStrDef(GetValue((int) Fields.Description), ""); }
		}

		public DateTime EntryDate
		{
			get	{ return Common.Tools.CDateTimeDef(GetValue((int) Fields.EntryDate), DateTime.MinValue); }
		}

		public double Sum
		{
			get
			{
				object value=GetValue((int) Fields.Sum);
				return Common.Tools.CDblDef(value, 0);
			}
		}

		public override string GetCustomText(int field)
		{
			return base.GetCustomText(field);
		}
	}
}
