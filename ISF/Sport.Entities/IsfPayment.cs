using System;
using Sport.Data;
using Sport.Types;

namespace Sport.Entities
{
	/// <summary>
	/// Implement an EntityType for Isf Payment
	/// </summary>
	public class IsfPayment : EntityBase
	{
		public static readonly string TypeName = "isf_payment";
		public static readonly EntityType Type;
		
		public enum Fields
		{
			Id = 0,
			Equipment,
			Sum,
			Description,
			PaymentDate,
			PaymentType,
			PaidBy,
			LastModified,
			FieldCount
		}
		
		static IsfPayment()
		{
			Type = new EntityType(TypeName, (int) Fields.FieldCount, (int) Fields.FieldCount, new FieldEntityId((int) Fields.Id));
			
			Type[(int) Fields.Id] = new NumberEntityField(Type, (int) Fields.Id);
			Type[(int) Fields.Id].CanEdit = false;
			Type[(int) Fields.Equipment] = new EntityEntityField(Type, (int) Fields.Equipment, Equipment.TypeName);
			Type[(int) Fields.Sum] = new NumberEntityField(Type, (int) Fields.Sum, 0, 9999999, 7, 2);
			Type[(int) Fields.Sum].MustExist = true;
			Type[(int) Fields.Description] = new TextEntityField(Type, (int) Fields.Description, true);
			Type[(int) Fields.PaymentDate] = new DateTimeEntityField(Type, (int) Fields.LastModified, "dd/MM/yyyy HH:mm");
			Type[(int) Fields.PaymentDate].MustExist = true;
			Type[(int) Fields.PaymentType] = new LookupEntityField(Type, (int) Fields.PaymentType, new PaymentTypeLookup());
			Type[(int) Fields.PaymentType].MustExist = true;
			Type[(int) Fields.PaidBy] = new EntityEntityField(Type, (int) Fields.PaidBy, User.TypeName);
			Type[(int) Fields.PaidBy].CanEdit = false;
			Type[(int) Fields.LastModified] = new DateTimeEntityField(Type, (int) Fields.LastModified, "dd/MM/yyyy HH:mm:ss");
			Type[(int) Fields.LastModified].CanEdit = false;
			
			Type.NameField = new FormatEntityField(Type, "תשלום {0} ש\"ח ({1})", 
				new int[] { (int) Fields.Sum, (int) Fields.Equipment });
			Type.DateLastModified = Type[(int) Fields.LastModified];
		}

		public IsfPayment(Entity entity)
			: base(entity)
		{
		}

		public IsfPayment(EntityEdit entityEdit)
			: base(entityEdit)
		{
		}

		public IsfPayment(int id)
			: base(Type, id)
		{
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

		public string Description
		{
			get
			{
				object value=GetValue((int) Fields.Description);
				return Common.Tools.CStrDef(value, "");
			}
			set { SetValue((int) Fields.Description, value); }
		}

		public DateTime PaymentDate
		{
			get
			{
				object value=GetValue((int) Fields.PaymentDate);
				return (value == null) ? (DateTime.MinValue) : ((DateTime) value);
			}
			set { SetValue((int) Fields.PaymentDate, value); }
		}

		public PaymentType PaymentType
		{
			get { return (PaymentType) GetValue((int) Fields.PaymentType); }
			set { SetValue((int) Fields.PaymentType, (int) value); }
		}

		public Equipment Equipment
		{
			get 
			{
				Entity entity = GetEntityValue((int) Fields.Equipment);
				return (entity == null)?null:new Equipment(entity);
			}
			set 
			{ 
				SetValue((int) Fields.Equipment, value.Entity); 
			}
		}
		
		public User PaidBy
		{
			get 
			{
				Entity entity = GetEntityValue((int) Fields.PaidBy);
				return (entity == null)?null:new User(entity);
			}
		}
	}
}
