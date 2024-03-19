using System;
using Sport.Data;

namespace Sport.Entities
{
	public class Account : EntityBase
	{
		public static readonly string TypeName = "account";
		public static readonly EntityType Type;

		public enum Fields
		{
			Id = 0,
			Region,
			Name,
			School,
			Balance,
			Address,
			LastModified,
			DataFields,
			SchoolSymbol = DataFields,
			FieldCount
		}

		static Account()
		{
			Type = new EntityType(TypeName, (int)Fields.FieldCount, (int)Fields.DataFields, new FieldEntityId((int)Fields.Id));

			Type[(int)Fields.Id] = new NumberEntityField(Type, (int)Fields.Id);
			Type[(int)Fields.Id].CanEdit = false;
			Type[(int)Fields.Region] = new EntityEntityField(Type, (int)Fields.Region, Region.TypeName);
			Type[(int)Fields.Region].MustExist = true;
			Type[(int)Fields.School] = new EntityEntityField(Type, (int)Fields.School, School.TypeName);
			Type[(int)Fields.Balance] = new NumberEntityField(Type, (int)Fields.Balance, 0, 99999999, 8, 2);
			Type[(int)Fields.Balance].CanEdit = false;
			Type[(int)Fields.LastModified] = new DateTimeEntityField(Type, (int)Fields.LastModified, "dd/MM/yyyy HH:mm:ss");
			Type[(int)Fields.LastModified].CanEdit = false;

			// Relative fields
			Type[(int)Fields.SchoolSymbol] = new EntityRelationEntityField(Type,
				(int)Fields.SchoolSymbol,
				(int)Fields.School,
				School.TypeName, (int)School.Fields.Symbol);

			Type.NameField = Type[(int)Fields.Name];
			//new FormatEntityField(Type, "{0} {1}", 
			//new int[] { (int) Fields.Name, (int) Fields.Address });
			Type.DateLastModified = Type[(int)Fields.LastModified];
		}

		public Account(Entity entity)
			: base(entity)
		{
		}

		public Account(EntityEdit entityEdit)
			: base(entityEdit)
		{
		}

		public Account(int paymentID)
			: base(Type, paymentID)
		{
		}

		public string AccountName
		{
			get
			{
				object value = GetValue((int)Fields.Name);
				return value == null ? "" : (string)value;
			}
			set { SetValue((int)Fields.Name, value); }
		}

		public Region Region
		{
			get
			{
				Entity entity = GetEntityValue((int)Fields.Region);
				if (entity == null)
					return null;
				return new Region(entity);
			}
			set
			{
				SetValue((int)Fields.Region, value.Entity);
			}
		}

		public School School
		{
			get
			{
				Entity entity = GetEntityValue((int)Fields.School);
				if (entity == null)
					return null;
				return new School(entity);
			}
			set
			{
				SetValue((int)Fields.School, value == null ? null : value.Entity);
			}
		}

		public double Balance
		{
			get
			{
				object balance = this.GetValue((int)Fields.Balance);
				return Common.Tools.CDblDef(balance, 0);
			}
		}

		public string Address
		{
			get { return Common.Tools.CStrDef(this.GetValue((int)Fields.Address), ""); }
			set { SetValue((int)Fields.Address, value); }
		}

		public override string CanDelete()
		{
			//check if the account has any charges:
			Entities.Charge[] charges = this.GetCharges();
			if (charges.Length > 0)
			{
				string names = "";
				for (int i = 0; i < charges.Length; i++)
				{
					names += charges[i].Name + "\n";
					if (i >= 15)
					{
						names += "...\n";
						break;
					}
				}
				return "החשבון '" + this.Name + "' נמצא בשימוש בחיובים הבאים: \n" + names +
					"יש להסיר חיובים אלו";
			}

			//check if the account has any receipts:
			Entities.Receipt[] receipts = this.GetReceipts();
			if (receipts.Length > 0)
			{
				string names = "";
				for (int i = 0; i < receipts.Length; i++)
				{
					names += receipts[i].Name + "\n";
					if (i >= 15)
					{
						names += "...\n";
						break;
					}
				}
				return "החשבון '" + this.Name + "' נמצא בשימוש בקבלות הבאות: \n" + names +
					"יש להסיר קבלות אלו";
			}

			return "";
		}


		public Charge[] GetCharges()
		{
			EntityFilter filter = new EntityFilter(
				(int)Entities.Charge.Fields.Account, this.Id);
			Entity[] charges = Entities.Charge.Type.GetEntities(filter);
			Charge[] result = new Charge[charges.Length];
			for (int i = 0; i < charges.Length; i++)
			{
				result[i] = new Charge(charges[i]);
			}
			return result;
		}

		public Receipt[] GetReceipts()
		{
			EntityFilter filter = new EntityFilter(
				(int)Entities.Receipt.Fields.Account, this.Id);
			Entity[] receipts = Entities.Receipt.Type.GetEntities(filter);
			Receipt[] result = new Receipt[receipts.Length];
			for (int i = 0; i < receipts.Length; i++)
				result[i] = new Receipt(receipts[i]);
			return result;
		}
	}
}
