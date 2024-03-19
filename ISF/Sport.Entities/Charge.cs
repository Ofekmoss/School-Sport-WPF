using System;
using Sport.Data;
using Sport.Types;

namespace Sport.Entities
{
	public class Charge : EntityBase
	{
		public static readonly string TypeName = "charge";
		public static readonly EntityType Type;
		
		public enum Fields
		{
			Id = 0,
			Region,
			Account,
			Product,
			Amount,
			Price,
			Date,
			Status, 
			Additional,
			ChampionshipCategory,
			LastModified,
			DataFields,
			PriceTotal = DataFields,
			FieldCount
		}
		
		static Charge()
		{
			Type = new EntityType(TypeName, (int) Fields.FieldCount, (int) Fields.DataFields, new FieldEntityId((int) Fields.Id));
			
			//Data fields:
			Type[(int) Fields.Id] = new NumberEntityField(Type, (int) Fields.Id);
			Type[(int) Fields.Id].CanEdit = false;
			Type[(int) Fields.Region] = new EntityEntityField(Type, (int) Fields.Region, Region.TypeName);
			Type[(int) Fields.Region].MustExist = true;
			Type[(int) Fields.Region].CanEdit = false;
			Type[(int) Fields.Account] = new EntityEntityField(Type, (int) Fields.Account, Account.TypeName);
			Type[(int) Fields.Account].MustExist = true;
			Type[(int) Fields.Product] = new EntityEntityField(Type, (int) Fields.Product, Product.TypeName);
			Type[(int) Fields.Product].MustExist = true;
			Type[(int) Fields.Amount] = new NumberEntityField(Type, (int) Fields.Amount, 1, 999999);
			Type[(int) Fields.Amount].MustExist = true;
			Type[(int) Fields.Price] = new NumberEntityField(Type, (int) Fields.Price, -9999, 9999999, 7, 2);
			Type[(int) Fields.Price].MustExist = true;
			Type[(int) Fields.Date] = new DateTimeEntityField(Type, (int) Fields.Date, "dd/MM/yyyy");
			Type[(int) Fields.Date].MustExist = true;
			Type[(int) Fields.Status] = new LookupEntityField(Type, (int) Fields.Status, new ChargeStatusLookup());
			Type[(int) Fields.Status].MustExist = true;
			Type[(int) Fields.ChampionshipCategory] = new EntityEntityField(Type, (int) Fields.ChampionshipCategory, ChampionshipCategory.TypeName);
			Type[(int) Fields.ChampionshipCategory].MustExist = false;
			Type[(int) Fields.LastModified] = new DateTimeEntityField(Type, (int) Fields.LastModified, "dd/MM/yyyy HH:mm:ss");
			Type[(int) Fields.LastModified].CanEdit = false;
			Type[(int) Fields.PriceTotal] = new TotalEntityField(Type, (int) Fields.PriceTotal, (int) Fields.Amount, (int) Fields.Price);
			Type[(int) Fields.PriceTotal].CanEdit = false;
			
			Type.NameField = new FormatEntityField(Type, "{0} {1}", 
				new int[] { (int) Fields.Amount, (int) Fields.Product });
			Type.DateLastModified = Type[(int) Fields.LastModified];

			Type.NewEntity = new EntityType.EntityNewEntity(NewEntity);
			Type.ValueChanged = new EntityType.EntityValueChange(ValueChanged);
		}

		private static void NewEntity(EntityEdit entityEdit)
		{
			Type[(int) Fields.Price].SetValue(entityEdit, 0);
			Type[(int) Fields.Amount].SetValue(entityEdit, 0);
			Type[(int) Fields.Status].SetValue(entityEdit, (int) Types.ChargeStatusType.NotPaid);
			Type[(int) Fields.Date].SetValue(entityEdit, DateTime.Now);
		}

		private static void ValueChanged(EntityEdit entityEdit, int field)
		{
			if (field == (int) Fields.Product)
			{
				Charge charge = new Charge(entityEdit);
				Type[(int) Fields.Price].SetValue(entityEdit, charge.Product.Price);
				if (charge.Amount == 0)
					Type[(int) Fields.Amount].SetValue(entityEdit, 1);

			}
		}
	
		public Charge(Entity entity)
			: base(entity)
		{
		}

		public Charge(EntityEdit entityEdit)
			: base(entityEdit)
		{
		}
		
		public Charge(int chargeId)
			: base(Type, chargeId)
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

		public ChargeStatusType Status
		{
			get { return (ChargeStatusType) GetValue((int) Fields.Status); }
			set { SetValue((int) Fields.Status, (int) value); }
		}
		
		/*
		public Championship Championship
		{
			get 
			{
				Entity entity = GetEntityValue((int) Fields.Additional);
				if (entity == null)
					return null;
				return new Championship(entity);
			}
			set 
			{ 
				SetValue((int) Fields.Additional, value == null ? null : value.Entity); 
			}
		}
		*/
		

		public Product Product
		{
			get 
			{
				Entity entity = GetEntityValue((int) Fields.Product);
				if (entity == null)
					return null;
				return new Product(entity);
			}
			set 
			{ 
				SetValue((int) Fields.Product, (value == null) ? null : value.Entity);
			}
		}
		
		public ChampionshipCategory ChampionshipCategory
		{
			get 
			{
				Entity entity = GetEntityValue((int) Fields.ChampionshipCategory);
				if (entity == null)
					return null;
				return new ChampionshipCategory(entity);
			}
			set 
			{ 
				SetValue((int) Fields.ChampionshipCategory, (value == null) ? null : value.Entity);
			}
		}
		
		public int Amount
		{
			get
			{
				object amount=this.GetValue((int) Fields.Amount);
				return amount == null ? 0 : (int) amount;
			}
			set
			{
				SetValue((int) Fields.Amount, value);
			}
		}

		public double Price
		{
			get
			{
				object price=this.GetValue((int) Fields.Price);
				return Common.Tools.CDblDef(price, 0);
			}
			set
			{
				SetValue((int) Fields.Price, value);
			}
		}

		public double PriceTotal
		{
			get
			{
				return (double) GetValue((int) Fields.PriceTotal);
			}
		}
	}

	#region TotalEntityField Class

	/// <summary>
	/// TotalEntityField inherits EntityField to implement
	/// a field as a amount and value multipication
	/// </summary>
	public class TotalEntityField : EntityField
	{
		#region Properties

		// A total field cannot be editable and should
		// not exist
		public override bool CanEdit
		{
			get { return false; }
		}

		public override bool MustExist
		{
			get { return false; }
		}

		private int _amountField;
		public int AmountField
		{
			get { return _amountField; }
			set { _amountField = value; }
		}

		private int _valueField;
		public int ValueField
		{
			get { return _valueField; }
			set { _valueField = value; }
		}

		#endregion

		#region Constructor

		// TotalEntityField constructor, receives the fields in 
		// the calculation
		public TotalEntityField(EntityType type, int index, int amountField, int valueField)
			: base(type, index)
		{
			_amountField = amountField;
			_valueField = valueField;
		}

		#endregion

		#region Value Operations

		// Overrides GetText to return the GetText value
		// of the relative field
		public override string GetText(Entity e)
		{
			object value = GetValue(e);
			return value == null ? "" : value.ToString();
		}

		// Overrides GetValue to return the text
		public override object GetValue(Entity e)
		{
			double amount = e.Fields[_amountField] == null ? 0 : (int) e.Fields[_amountField];
			double value = e.Fields[_valueField] == null ? 0 : (double) e.Fields[_valueField];

			return amount * value;
		}

		#endregion

		#region Value Comoparison

		// Overrides Compare to preform int comparison
		public override int Compare(Entity x, Entity y)
		{
			double ix = (double) GetValue(x);
			double iy = (double) GetValue(y);

			if (ix < iy)
				return -1;
			else if (iy < ix)
				return 1;

			return 0;
		}

		// Overrides Equals to preform int comparison
		public override bool Equals(Entity e, object value)
		{
			object fieldValue=GetValue(e);
			if ((fieldValue == null)&&(value == null))
				return true;
			if ((fieldValue == null)||(value == null))
				return false;
			return value.Equals(fieldValue);
		}

		#endregion
	}

	#endregion

}
