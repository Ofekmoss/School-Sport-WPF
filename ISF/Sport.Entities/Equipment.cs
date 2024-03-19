using System;
using Sport.Data;
using Sport.Types;

namespace Sport.Entities
{
	public class Equipment : EntityBase
	{
		public static readonly string TypeName = "equipment";
		public static readonly EntityType Type;
		
		public enum Fields
		{
			Id = 0,
			Type,
			Region,
			Sport,
			Championship,
			Category,
			Amount,
			Price,
			DateOrdered,
			LastModified,
			DataFields,
			Name = DataFields,
			FieldCount
		}
		
		static Equipment()
		{
			Type = new EntityType(TypeName, (int) Fields.FieldCount, (int) Fields.DataFields, new FieldEntityId((int) Fields.Id));

			Type[(int) Fields.Id] = new NumberEntityField(Type, (int) Fields.Id);
			Type[(int) Fields.Id].CanEdit = false;
			Type[(int) Fields.Type] = new EntityEntityField(Type, 
				(int) Fields.Type, EquipmentType.TypeName);
			Type[(int) Fields.Region] = new EntityEntityField(Type, 
				(int) Fields.Region, Region.TypeName);
			Type[(int) Fields.Sport] = new EntityEntityField(Type, 
				(int) Fields.Sport, Sport.TypeName);
			Type[(int) Fields.Championship] = new EntityEntityField(Type, 
				(int) Fields.Championship, Championship.TypeName);
			Type[(int) Fields.Category] = new EntityEntityField(Type, 
				(int) Fields.Category, ChampionshipCategory.TypeName);
			Type[(int) Fields.Amount] = new NumberEntityField(Type, 
				(int) Fields.Amount, (double) 0, (double) 9999999);
			Type[(int) Fields.Amount].MustExist = true;
			Type[(int) Fields.Price] = new NumberEntityField(Type, 
				(int) Fields.Price, (double) 0, (double) 9999999, 7, 2);
			Type[(int) Fields.DateOrdered] = new DateTimeEntityField(Type, (int) Fields.DateOrdered, "dd/MM/yyyy");
			Type[(int) Fields.DateOrdered].MustExist = true;
			Type[(int) Fields.LastModified] = new DateTimeEntityField(Type, (int) Fields.LastModified, "dd/MM/yyyy HH:mm:ss");
			Type[(int) Fields.LastModified].CanEdit = false;
			
			// Relative fields
			Type[(int) Fields.Name] = new EntityRelationEntityField(Type, 
				(int) Fields.Name,
				(int) Fields.Type,
				EquipmentType.TypeName, (int) EquipmentType.Fields.Name);

			Type.NameField = new FormatEntityField(Type, "{0}", 
				new int[] { (int) Fields.Name });
			Type.DateLastModified = Type[(int) Fields.LastModified];
			
			//Type.OwnerField = Type[(int) Fields.Championship];
		}
		
		public Equipment(Entity entity)
			: base(entity)
		{
		}

		public Equipment(EntityEdit entityEdit)
			: base(entityEdit)
		{
		}

		public Equipment(int ID)
			: base(Type, ID)
		{
		}

		public Region Region
		{
			get
			{
				Entity entity = GetEntityValue((int) Fields.Region);
				return (entity == null)?null:new Region(entity);
			}
			set 
			{ 
				SetValue((int) Fields.Region, value.Entity); 
			}
		}

		public Sport Sport
		{
			get
			{
				Entity entity = GetEntityValue((int) Fields.Sport);
				return (entity == null)?null:new Sport(entity);
			}
			set 
			{ 
				SetValue((int) Fields.Sport, value.Entity); 
			}
		}

		public Championship Championship
		{
			get
			{
				Entity entity = GetEntityValue((int) Fields.Championship);
				return (entity == null)?null:new Championship(entity);
			}
			set 
			{ 
				SetValue((int) Fields.Championship, value.Entity); 
			}
		}

		public ChampionshipCategory Category
		{
			get
			{
				Entity entity = GetEntityValue((int) Fields.Category);
				return (entity == null)?null:new ChampionshipCategory(entity);
			}
			set 
			{ 
				SetValue((int) Fields.Category, value.Entity); 
			}
		}
		
		public EquipmentType EquipmentType
		{
			get
			{
				Entity entity = GetEntityValue((int) Fields.Type);
				return (entity == null)?null:new EquipmentType(entity);
			}
			set 
			{ 
				SetValue((int) Fields.Category, value.Entity); 
			}
		}
		
		public IsfPayment[] GetPayments()
		{
			IsfPayment[] result;
			Entity[] payments=IsfPayment.Type.GetEntities(new EntityFilter(
				(int) IsfPayment.Fields.Equipment, this.Id));
			result = new IsfPayment[payments.Length];
			for (int i=0; i<payments.Length; i++)
				result[i] = new IsfPayment(payments[i]);
			return result;

		}
	}
}
