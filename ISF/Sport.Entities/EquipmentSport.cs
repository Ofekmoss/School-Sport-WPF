using System;
using Sport.Data;
using Sport.Types;

namespace Sport.Entities
{
	public class EquipmentSport : EntityBase
	{
		public static readonly string TypeName = "equipmentsport";
		public static readonly EntityType Type;
		
		public enum Fields
		{
			Id = 0,
			Type,
			Sport,
			Price,
			LastModified,
			FieldCount
		}
		
		static EquipmentSport()
		{
			Type = new EntityType(TypeName, (int) Fields.FieldCount, (int) Fields.FieldCount, new FieldEntityId((int) Fields.Id));

			Type[(int) Fields.Id] = new NumberEntityField(Type, (int) Fields.Id);
			Type[(int) Fields.Id].CanEdit = false;
			Type[(int) Fields.Type] = new EntityEntityField(Type, 
				(int) Fields.Type, EquipmentType.TypeName);
			Type[(int) Fields.Sport] = new EntityEntityField(Type, 
				(int) Fields.Sport, Sport.TypeName);
			Type[(int) Fields.Price] = new NumberEntityField(Type, 
				(int) Fields.Price, (double) 0, (double) 9999999, 7, 2);
			Type[(int) Fields.LastModified] = new DateTimeEntityField(Type, (int) Fields.LastModified, "dd/MM/yyyy HH:mm:ss");
			Type[(int) Fields.LastModified].CanEdit = false;
			
			//Type.NameField = Type[(int) Fields.Region];
			Type.DateLastModified = Type[(int) Fields.LastModified];
			//Type.OwnerField = Type[(int) Fields.Championship];
		}
		
		public EquipmentSport(Entity entity)
			: base(entity)
		{
		}

		public EquipmentSport(EntityEdit entityEdit)
			: base(entityEdit)
		{
		}

		public EquipmentSport(int ID)
			: base(Type, ID)
		{
		}
	}
}
