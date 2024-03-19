using System;
using Sport.Data;

namespace Sport.Entities
{
	public class InstantMessage : EntityBase
	{
		public static readonly string TypeName = "instantmessage";
		public static readonly EntityType Type;
		public static Data.Entity NewRecipient=null;
		
		public enum Fields
		{
			Id = 0,
			Sender,
			Recipient,
			DateSent,
			Contents,
			DateRead,
			LastModified,
			FieldCount
		}

		static InstantMessage()
		{
			Type = new EntityType(TypeName, (int) Fields.FieldCount, (int) Fields.FieldCount, new FieldEntityId((int) Fields.Id));
			
			//Data fields:
			Type[(int) Fields.Id] = new NumberEntityField(Type, (int) Fields.Id);
			Type[(int) Fields.Id].CanEdit = false;
			Type[(int) Fields.Sender] = new EntityEntityField(Type, (int) Fields.Sender, User.TypeName);
			Type[(int) Fields.Sender].CanEdit = false;
			Type[(int) Fields.Sender].MustExist = true;
			Type[(int) Fields.Recipient] = new EntityEntityField(Type, (int) Fields.Recipient, User.TypeName);
			Type[(int) Fields.Recipient].MustExist = true;
			Type[(int) Fields.DateSent] = new DateTimeEntityField(Type, (int) Fields.DateSent, "dd/MM/yyyy HH:mm:ss");
			Type[(int) Fields.DateSent].CanEdit = false;
			Type[(int) Fields.Contents] = new TextEntityField(Type, (int) Fields.Contents, true);
			Type[(int) Fields.Contents].MustExist = true;
			Type[(int) Fields.DateRead] = new DateTimeEntityField(Type, (int) Fields.DateRead, "dd/MM/yyyy HH:mm:ss");
			Type[(int) Fields.DateRead].CanEdit = false;
			Type[(int) Fields.LastModified] = new DateTimeEntityField(Type, (int) Fields.LastModified, "dd/MM/yyyy HH:mm:ss");
			Type[(int) Fields.LastModified].CanEdit = false;
			
			Type.NameField = new FormatEntityField(Type, "הודעה מיידית לכבוד {0}", 
				new int[] { (int) Fields.Recipient });
			Type.DateLastModified = Type[(int) Fields.LastModified];
			
			Type.NewEntity = new EntityType.EntityNewEntity(NewEntity);
		}
		
		private static void NewEntity(EntityEdit entityEdit)
		{
			Type[(int) Fields.Sender].SetValue(entityEdit, (new User(Core.Session.User.Id)).Entity);
			Type[(int) Fields.Recipient].SetValue(entityEdit, NewRecipient);
			Type[(int) Fields.DateSent].SetValue(entityEdit, DateTime.Now);
		}
		
		public InstantMessage(Entity entity)
			: base(entity)
		{
		}

		public InstantMessage(EntityEdit entityEdit)
			: base(entityEdit)
		{
		}

		public InstantMessage(int instantMessageID)
			: base(Type, instantMessageID)
		{
		}

		public User Sender
		{
			get 
			{
				Entity entity = GetEntityValue((int) Fields.Sender);
				if (entity == null)
					return null;
				return new User(entity);
			}
			set 
			{ 
				SetValue((int) Fields.Sender, value.Entity); 
			}
		}
		
		public User Recipient
		{
			get 
			{
				Entity entity = GetEntityValue((int) Fields.Recipient);
				if (entity == null)
					return null;
				return new User(entity);
			}
			set 
			{ 
				SetValue((int) Fields.Recipient, value.Entity); 
			}
		}

		public DateTime DateSent
		{
			get { return Common.Tools.ToDateDef(GetValue((int) Fields.DateSent), DateTime.MinValue); }
			set { SetValue((int) Fields.DateSent, value); }
		}
		
		public string Contents
		{
			get { return Common.Tools.CStrDef(GetValue((int) Fields.Contents), ""); }
			set { SetValue((int) Fields.Contents, value); }
		}
		
		public DateTime DateRead
		{
			get { return Common.Tools.ToDateDef(GetValue((int) Fields.DateRead), DateTime.MinValue); }
			set { SetValue((int) Fields.DateRead, value); }
		}
		
		public Data.EntityResultCode MarkAsRead()
		{
			this.Edit();
			Type[(int) Fields.DateRead].CanEdit = true;
			this.DateRead = DateTime.Now;
			Type[(int) Fields.DateRead].CanEdit = false;
			return this.Save().ResultCode;;
		}
	}
}
