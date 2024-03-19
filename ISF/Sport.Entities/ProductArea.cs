using System;
using Sport.Data;

namespace Sport.Entities
{
	public class ProductArea : EntityBase
	{
		public static readonly string TypeName = "productarea";
		public static readonly EntityType Type;

		public enum Fields
		{
			Id = 0,
			Name, 
			LastModified,
			FieldCount
		}

		static ProductArea()
		{
			Type = new EntityType(TypeName, (int) Fields.FieldCount, (int) Fields.FieldCount, new FieldEntityId((int) Fields.Id));
			
			Type[(int) Fields.Id] = new NumberEntityField(Type, (int) Fields.Id);
			Type[(int) Fields.Id].CanEdit = false;
			Type[(int) Fields.Name].MustExist = true;
			Type[(int) Fields.LastModified] = new DateTimeEntityField(Type, (int) Fields.LastModified, "dd/MM/yyyy HH:mm:ss");
			Type[(int) Fields.LastModified].CanEdit = false;

			Type.NameField = Type[(int) Fields.Name];
			Type.DateLastModified = Type[(int) Fields.LastModified];
		}

		public ProductArea(Entity entity)
			: base(entity)
		{
		}

		public ProductArea(EntityEdit entityEdit)
			: base(entityEdit)
		{
		}

		public ProductArea(int productAreaId)
			: base(Type, productAreaId)
		{
		}
	}
}
