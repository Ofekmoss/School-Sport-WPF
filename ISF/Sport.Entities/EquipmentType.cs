using System;
using Sport.Data;
using Sport.Types;

namespace Sport.Entities
{
	public class EquipmentType : EntityBase
	{
		public static readonly string TypeName = "equipmentsport";
		public static readonly EntityType Type;
		
		public enum Fields
		{
			Id = 0,
			Name,
			BasePrice,
			Type,
			LastModified,
			FieldCount
		}
		
		static EquipmentType()
		{
			Type = new EntityType(TypeName, (int) Fields.FieldCount, (int) Fields.FieldCount, new FieldEntityId((int) Fields.Id));

			Type[(int) Fields.Id] = new NumberEntityField(Type, (int) Fields.Id);
			Type[(int) Fields.Id].CanEdit = false;
			Type[(int) Fields.Name].MustExist = true;
			Type[(int) Fields.BasePrice] = new NumberEntityField(Type, 
				(int) Fields.BasePrice, (double) 0, (double) 9999999, 7, 2);
			Type[(int) Fields.Type] = new LookupEntityField(
				Type, (int) Fields.Type, new EquipmentTypeLookup());
			Type[(int) Fields.Type].MustExist = true;
			Type[(int) Fields.LastModified] = new DateTimeEntityField(Type, (int) Fields.LastModified, "dd/MM/yyyy HH:mm:ss");
			Type[(int) Fields.LastModified].CanEdit = false;
			
			Type.NameField = Type[(int) Fields.Name];
			Type.DateLastModified = Type[(int) Fields.LastModified];
			//Type.OwnerField = Type[(int) Fields.Championship];
		}
		
		public EquipmentType(Entity entity)
			: base(entity)
		{
		}

		public EquipmentType(EntityEdit entityEdit)
			: base(entityEdit)
		{
		}

		public EquipmentType(int ID)
			: base(Type, ID)
		{
		}

		public Equipment[] GetEquipments()
		{
			EntityFilter filter=new EntityFilter(
				(int) Equipment.Fields.Type, this.Id);
			Entity[] entities=Equipment.Type.GetEntities(filter);
			Equipment[] result=new Equipment[entities.Length];
			for (int i=0; i<entities.Length; i++)
			{
				result[i] = new Equipment(entities[i]);
			}
			return result;
		}

		public Types.EquipmentSeperationType SeperationType
		{
			get { return (EquipmentSeperationType) GetValue((int) Fields.Type); }
			set { SetValue((int) Fields.Type, (int) value); }
		}
		
		public double BasePrice
		{
			get { return (double) GetValue((int) Fields.BasePrice); }
			set { SetValue((int) Fields.BasePrice, value); }
		}
	}
}
