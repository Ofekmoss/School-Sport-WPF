using System;
using Sport.Data;
using Sport.Types;

namespace Sport.Entities
{
	public class Referee : EntityBase
	{
		public static readonly string TypeName = "referee";
		public static readonly EntityType Type;
		
		public enum Fields
		{
			Id = 0,
			Name,
			Type,
			LastModified,
			FieldCount
		}
		
		static Referee()
		{
			Type = new EntityType(TypeName, (int) Fields.FieldCount, (int) Fields.FieldCount, new FieldEntityId((int) Fields.Id));
			
			Type[(int) Fields.Id] = new NumberEntityField(Type, (int) Fields.Id);
			Type[(int) Fields.Id].CanEdit = false;
			Type[(int) Fields.Name].MustExist = true;
			Type[(int) Fields.Type] = new LookupEntityField(Type, (int)Fields.Type, new RefereeTypeLookup());
			Type[(int) Fields.Type].MustExist = true;
			Type[(int) Fields.LastModified] = new DateTimeEntityField(Type, (int) Fields.LastModified, "dd/MM/yyyy HH:mm:ss");
			Type[(int) Fields.LastModified].CanEdit = false;
			
			Type.NameField = Type[(int) Fields.Name];
			Type.DateLastModified = Type[(int) Fields.LastModified];
		}

		public Referee(Entity entity)
			: base(entity)
		{
		}
		
		public Referee(EntityEdit entityEdit)
			: base(entityEdit)
		{
		}

		public Referee(int refereeID)
			: base(Type, refereeID)
		{
		}
		
		public new string Name
		{
			get { return base.Name; }
			set { SetValue((int) Fields.Name, value); }
		}
	}
}
