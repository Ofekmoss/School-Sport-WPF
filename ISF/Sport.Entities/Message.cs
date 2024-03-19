using System;
using Sport.Data;
using Sport.Types;

namespace Sport.Entities
{
	public class Message : EntityBase
	{
		public static readonly string TypeName = "message";
		public static readonly EntityType Type;
		
		public enum Fields
		{
			Id = 0,
			User,
			Type,
			Status,
			Text,
			TimeSent,
			TimeRead,
			LastModified,
			FieldCount
		}

		static Message()
		{
			Type = new EntityType(TypeName, (int) Fields.FieldCount, (int) Fields.FieldCount, new FieldEntityId((int) Fields.Id));
			
			Type[(int) Fields.Id] = new NumberEntityField(Type, (int) Fields.Id);
			Type[(int) Fields.Id].CanEdit = false;
			Type[(int) Fields.User] = new EntityEntityField(Type, (int) Fields.User, User.TypeName);
			Type[(int) Fields.User].MustExist = true;
			Type[(int) Fields.User].CanEdit = false;
			Type[(int) Fields.Type] = new LookupEntityField(Type, (int) Fields.Type, new MessageTypeLookup());
			Type[(int) Fields.Type].CanEdit = false;
			Type[(int) Fields.Status] = new LookupEntityField(Type, (int) Fields.Status, new MessageStatusLookup());
			Type[(int) Fields.Status].MustExist = true;
			Type[(int) Fields.Text] = new TextEntityField(Type, (int) Fields.Text, true, true);
			Type[(int) Fields.Text].MustExist = true;
			Type[(int) Fields.Text].CanEdit = false;
			Type[(int) Fields.TimeSent] = new DateTimeEntityField(Type, (int) Fields.TimeSent, "dd/MM/yyyy HH:mm");
			Type[(int) Fields.TimeSent].MustExist = true;
			Type[(int) Fields.TimeSent].CanEdit = false;
			Type[(int) Fields.TimeRead] = new DateTimeEntityField(Type, (int) Fields.TimeRead, "dd/MM/yyyy HH:mm");
			Type[(int) Fields.LastModified] = new DateTimeEntityField(Type, (int) Fields.LastModified, "dd/MM/yyyy HH:mm:ss");
			Type[(int) Fields.LastModified].CanEdit = false;
			
			Type.NameField = Type[(int) Fields.Text];
			Type.DateLastModified = Type[(int) Fields.LastModified];
			Type.ValueChanged = new EntityType.EntityValueChange(ValueChanged);
		}
		
		private static void ValueChanged(EntityEdit entityEdit, int field)
		{
			/*
			if (field == (int) Fields.Region)
			{
				Type[(int) Fields.School].SetValue(entityEdit, null);
			}
			*/
		}
		
		public Message(Entity entity)
			: base(entity)
		{
		}
		
		public Message(EntityEdit entityEdit)
			: base(entityEdit)
		{
		}
		
		public Message(int messageId)
			: base(Type, messageId)
		{
		}

		public string Text
		{
			get { return (string) GetValue((int) Fields.Text); }
			set { SetValue((int) Fields.Text, value); }
		}

		public ExpandString ExpandText()
		{
			return new ExpandString(Text);
		}
	}
}
