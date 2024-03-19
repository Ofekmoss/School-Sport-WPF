using System;
using Sport.Data;

namespace Sport.Entities
{
	/// <summary>
	/// Implement an EntityType for a Log (log table item)
	/// </summary>
	public class Log : EntityBase
	{
		public static readonly string TypeName = "log";
		public static readonly EntityType Type;
		
		public enum Fields
		{
			Id = 0,
			User,
			Date,
			Description,
			Version,
			LastModified,
			FieldCount
		}

		static Log()
		{
			Type = new EntityType(TypeName, (int) Fields.FieldCount, (int) Fields.FieldCount, new FieldEntityId((int) Fields.Id));

			Type[(int) Fields.Id] = new NumberEntityField(Type, (int) Fields.Id);
			Type[(int) Fields.Id].CanEdit = false;
			Type[(int) Fields.User] = new EntityEntityField(Type, (int) Fields.User, User.TypeName);
			Type[(int) Fields.User].CanEdit = false;
			Type[(int) Fields.Date] = new DateTimeEntityField(Type, (int) Fields.Date, "dd/MM/yyyy HH:mm:ss");
			Type[(int) Fields.Date].CanEdit = false;
			Type[(int) Fields.Description] = new TextEntityField(Type, (int) Fields.Description, true);
			Type[(int) Fields.Description].CanEdit = false;
			Type[(int) Fields.Version].CanEdit = false;
			Type[(int) Fields.LastModified] = new DateTimeEntityField(Type, (int) Fields.LastModified, "dd/MM/yyyy HH:mm:ss");
			Type[(int) Fields.LastModified].CanEdit = false;
			
			Type.NameField = Type[(int) Fields.Description];
			Type.DateLastModified = Type[(int) Fields.LastModified];
		}
		
		public Log(Entity entity)
			: base(entity)
		{
		}
		
		public Log(EntityEdit entityEdit)
			: base(entityEdit)
		{
		}
		
		public Log(int logID)
			: base(Type, logID)
		{
		}

		public User User
		{
			get
			{
				Entity entity = GetEntityValue((int) Fields.User);
				if (entity == null)
					return null;
				return new User(entity);
			}
			set 
			{ 
				SetValue((int) Fields.User, value.Entity); 
			}
		}

		public DateTime Date
		{
			get
			{
				object value=GetValue((int) Fields.Date);
				return (value == null)?(DateTime.MinValue):((DateTime) value);
			}
			set { SetValue((int) Fields.Date, value); }
		}

		public string Description
		{
			get { return (string) GetValue((int) Fields.Description); }
			set { SetValue((int) Fields.Description, value); }
		}

		public double Version
		{
			get
			{
				object ver=this.GetValue((int) Fields.Version);
				return Common.Tools.CDblDef(ver, 0);
			}
			set
			{
				SetValue((int) Fields.Version, value);
			}
		}

		public DateTime LastModified
		{
			get
			{
				object value=GetValue((int) Fields.LastModified);
				return (value == null)?(DateTime.MinValue):((DateTime) value);
			}
			set { SetValue((int) Fields.Date, value); }
		}
	} //end class Log
}
