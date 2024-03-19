using System;
using Sport.Data;
using Sport.Types;

namespace Sport.Entities
{
	public class EquipmentRegion : EntityBase
	{
		public static readonly string TypeName = "equipmentregion";
		public static readonly EntityType Type;
		
		public enum Fields
		{
			Id = 0,
			Type,
			Region,
			Price,
			LastModified,
			FieldCount
		}
		
		static EquipmentRegion()
		{
			Type = new EntityType(TypeName, (int) Fields.FieldCount, (int) Fields.FieldCount, new FieldEntityId((int) Fields.Id));

			Type[(int) Fields.Id] = new NumberEntityField(Type, (int) Fields.Id);
			Type[(int) Fields.Id].CanEdit = false;
			Type[(int) Fields.Type] = new EntityEntityField(Type, 
				(int) Fields.Type, EquipmentType.TypeName);
			Type[(int) Fields.Region] = new EntityEntityField(Type, 
				(int) Fields.Region, Region.TypeName);
			Type[(int) Fields.Price] = new NumberEntityField(Type, 
				(int) Fields.Price, (double) 0, (double) 9999999, 7, 2);
			Type[(int) Fields.LastModified] = new DateTimeEntityField(Type, (int) Fields.LastModified, "dd/MM/yyyy HH:mm:ss");
			Type[(int) Fields.LastModified].CanEdit = false;
			
			//Type.NameField = Type[(int) Fields.Region];
			Type.DateLastModified = Type[(int) Fields.LastModified];
			//Type.OwnerField = Type[(int) Fields.Championship];
		}
		
		public EquipmentRegion(Entity entity)
			: base(entity)
		{
		}

		public EquipmentRegion(EntityEdit entityEdit)
			: base(entityEdit)
		{
		}

		public EquipmentRegion(int ID)
			: base(Type, ID)
		{
		}
	}
}
