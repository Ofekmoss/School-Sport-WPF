using System;
using Sport.Data;
using Sport.Types;

namespace Sport.Entities
{
	public class Bug : EntityBase
	{
		public static readonly string TypeName = "bug";
		public static readonly EntityType Type;

		public enum Fields
		{
			Id,
			Date,
			Title,
			Description,
			Status,
			User,
			Type,
			LastModified,
			FieldCount
		}

		static Bug()
		{
			Type = new EntityType(TypeName, (int) Fields.FieldCount, (int) Fields.FieldCount, new FieldEntityId((int) Fields.Id));

			Type[(int) Fields.Id] = new NumberEntityField(Type, (int) Fields.Id);
			Type[(int) Fields.Id].CanEdit = false;
			Type[(int) Fields.Title].MustExist = true;
			Type[(int) Fields.Date] = new DateTimeEntityField(Type, (int) Fields.Date, "dd/MM/yyyy");
			Type[(int) Fields.Status] = new LookupEntityField(Type, (int) Fields.Status, new BugStatusLookup());
			Type[(int) Fields.Description] = new TextEntityField(Type, (int) Fields.Description, true);
			Type[(int) Fields.User] = new EntityEntityField(Type, (int) Fields.User, User.TypeName);
			Type[(int) Fields.User].CanEdit = false;
			Type[(int) Fields.Type] = new LookupEntityField(Type, (int) Fields.Type, new BugTypeLookup());
			Type[(int) Fields.Type].MustExist = true;
			Type[(int) Fields.LastModified] = new DateTimeEntityField(Type, (int) Fields.LastModified, "dd/MM/yyyy HH:mm:ss");
			Type[(int) Fields.LastModified].CanEdit = false;

			Type.NewEntity = new EntityType.EntityNewEntity(NewEntity);
			Type.NameField = Type[(int) Fields.Title];
			Type.DateLastModified = Type[(int) Fields.LastModified];
		}

		private static void NewEntity(EntityEdit entityEdit)
		{
			Type[(int) Fields.Status].SetValue(entityEdit, 0);
			Type[(int) Fields.Type].SetValue(entityEdit, (int) Types.BugType.Question);
			Type[(int) Fields.Date].SetValue(entityEdit, DateTime.Now);
		}

		public Bug(Entity entity)
			: base(entity)
		{
		}

		public Bug(EntityEdit entityEdit)
			: base(entityEdit)
		{
		}

		public Bug(int bugId)
			: base(Type, bugId)
		{
		}

		public string Title
		{
			get { return (string) GetValue((int) Fields.Title); }
			set { SetValue((int) Fields.Title, value); }
		}

		public DateTime Date
		{
			get { return (DateTime) GetValue((int) Fields.Date); }
			set { SetValue((int) Fields.Date, value); }
		}

		public string Description
		{
			get { return (string) GetValue((int) Fields.Description); }
			set { SetValue((int) Fields.Description, value); }
		}
		
		public int Status
		{
			get { return (int) GetValue((int) Fields.Status); }
			set { SetValue((int) Fields.Status, value); }
		}

		public int WrittenBy
		{
			get
			{
				object value=GetValue((int) Fields.User);
				return (value == null)?-1:(int) value;
			}
			set { SetValue((int) Fields.User, value); }
		}
	}
}
