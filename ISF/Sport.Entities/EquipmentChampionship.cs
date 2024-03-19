using System;
using Sport.Data;
using Sport.Types;

namespace Sport.Entities
{
	public class EquipmentChampionship : EntityBase
	{
		public static readonly string TypeName = "equipmentchampionship";
		public static readonly EntityType Type;
		
		public enum Fields
		{
			Id = 0,
			Type,
			Championship,
			Price,
			LastModified,
			FieldCount
		}
		
		static EquipmentChampionship()
		{
			Type = new EntityType(TypeName, (int) Fields.FieldCount, (int) Fields.FieldCount, new FieldEntityId((int) Fields.Id));

			Type[(int) Fields.Id] = new NumberEntityField(Type, (int) Fields.Id);
			Type[(int) Fields.Id].CanEdit = false;
			Type[(int) Fields.Type] = new EntityEntityField(Type, 
				(int) Fields.Type, EquipmentType.TypeName);
			Type[(int) Fields.Championship] = new EntityEntityField(Type, 
				(int) Fields.Championship, Championship.TypeName);
			Type[(int) Fields.Price] = new NumberEntityField(Type, 
				(int) Fields.Price, (double) 0, (double) 999999, 7, 2);
			Type[(int) Fields.LastModified] = new DateTimeEntityField(Type, (int) Fields.LastModified, "dd/MM/yyyy HH:mm:ss");
			Type[(int) Fields.LastModified].CanEdit = false;
			
			//Type.NameField = Type[(int) Fields.Region];
			Type.DateLastModified = Type[(int) Fields.LastModified];
			//Type.OwnerField = Type[(int) Fields.Championship];
		}
		
		public EquipmentChampionship(Entity entity)
			: base(entity)
		{
		}

		public EquipmentChampionship(EntityEdit entityEdit)
			: base(entityEdit)
		{
		}

		public EquipmentChampionship(int ID)
			: base(Type, ID)
		{
		}
	}
}
