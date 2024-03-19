using System;
using Sport.Data;
using Sport.Types;

namespace Sport.Entities
{
	public class User : EntityBase
	{
		public static readonly string TypeName = "user";
		public static readonly EntityType Type;

		public static User CurrentUser = null;

		public enum Fields
		{
			Id = 0,
			Login,
			FirstName,
			LastName,
			Region,
			School,
			UserType,
			Permissions,
			Email,
			LastModified,
			FieldCount
		}

		static User()
		{
			Type = new EntityType(TypeName, (int) Fields.FieldCount, (int) Fields.FieldCount, new FieldEntityId((int) Fields.Id));

			Type[(int) Fields.Id] = new NumberEntityField(Type, (int) Fields.Id);
			Type[(int) Fields.Id].CanEdit = false;
			Type[(int) Fields.Region] = new EntityEntityField(Type, (int) Fields.Region, Region.TypeName);
			Type[(int) Fields.Region].MustExist = true;
			Type[(int) Fields.School] = new EntityEntityField(Type, (int) Fields.School, School.TypeName);
			Type[(int) Fields.School].MustExist = false;
			Type[(int) Fields.UserType] = new LookupEntityField(Type, (int) Fields.UserType, new UserTypeLookup());
			Type[(int) Fields.UserType].MustExist = true;
			Type[(int) Fields.FirstName].MustExist = true;
			Type[(int) Fields.Login].MustExist = true;
			Type[(int) Fields.Permissions] = new LookupEntityField(Type, (int) Fields.Permissions, new PermissionTypeLookup());
			Type[(int) Fields.Permissions].MustExist = true;
			Type[(int) Fields.LastModified] = new DateTimeEntityField(Type, (int) Fields.LastModified, "dd/MM/yyyy HH:mm:ss");
			Type[(int) Fields.LastModified].CanEdit = false;
			
			Type.NameField = new FormatEntityField(Type, "{0} {1}", 
				new int[] { (int) Fields.FirstName, (int) Fields.LastName });
			Type.DateLastModified = Type[(int) Fields.LastModified];
			
			Type.ValueChanged = new EntityType.EntityValueChange(ValueChanged);
		}

		private static void ValueChanged(EntityEdit entityEdit, int field)
		{
			if (field == (int) Fields.Region)
			{
				Type[(int) Fields.School].SetValue(entityEdit, null);
			}
			else if (field == (int) Fields.UserType)
			{
				if (new User(entityEdit).UserType == UserType.Internal)
				{
					int permissions = PermissionTypeLookup.Add(0, Permission.Championships);
					permissions = PermissionTypeLookup.Add(permissions, Permission.General);
					Type[(int) Fields.Permissions].SetValue(entityEdit, permissions);
				}
				else
				{
					Type[(int) Fields.Permissions].SetValue(entityEdit, 0);
				}
			}
		}

		private static object[] FieldValues(Entity entity, int field)
		{
			EntityEntityField entityField = Type[field] as EntityEntityField;

			if (entityField == null)
				throw new EntityException("Field must be entity field");

			if (field == (int) Fields.Region)
			{
				return entityField.EntityType.GetEntities(null);
			}
			else if (field == (int) Fields.School)
			{
				User user = new User(entity);
				return entityField.EntityType.GetEntities(
					new EntityFilter((int) School.Fields.Region, user.Region.Id));
			}

			return null;
		}


		public User(Entity entity)
			: base(entity)
		{
		}

		public User(EntityEdit entityEdit)
			: base(entityEdit)
		{
		}

		public User(int userId)
			: base(Type, userId)
		{
		}

		public string Login
		{
			get { return (string) GetValue((int) Fields.Login); }
			set { SetValue((int) Fields.Login, value); }
		}

		public string FirstName
		{
			get { return (string) GetValue((int) Fields.FirstName); }
			set { SetValue((int) Fields.FirstName, value); }
		}

		public string LastName
		{
			get { return (string) GetValue((int) Fields.LastName); }
			set { SetValue((int) Fields.LastName, value); }
		}

		public string Email
		{
			get
			{
				object emailObject = GetValue((int) Fields.Email);
				return (emailObject == null)?"":emailObject.ToString();
			}
			set
			{
				SetValue((int)Fields.Email,value);
			}
		}

		public Region Region
		{
			get
			{
				Entity entity=null;
				try
				{
					entity = GetEntityValue((int) Fields.Region);
				}
				catch (Exception e)
				{
					System.Diagnostics.Debug.WriteLine("failed to get user Region. user: "+this.Id+", error: "+e.Message);
				}
				if (entity == null)
					return null;
				return new Region(entity);
			}
			set 
			{ 
				SetValue((int) Fields.Region, value.Entity); 
			}
		}

		public School School
		{
			get
			{
				Entity entity=null;
				try
				{
					entity = GetEntityValue((int) Fields.School);
				}
				catch(Exception ex)
				{
					System.Diagnostics.Debug.WriteLine("failed to get entity value for user "+this.Id+" - "+ex.Message);
					System.Diagnostics.Debug.WriteLine(ex.StackTrace);
					return null;
				}
				if (entity == null)
					return null;
				return new School(entity);
			}
			set 
			{ 
				SetValue((int) Fields.School, value.Entity); 
			}
		}

		public UserType UserType
		{
			get { return (UserType) GetValue((int) Fields.UserType); }
			set { SetValue((int) Fields.UserType, (int) value); }
		}

		public int Permissions
		{
			get { return Common.Tools.CIntDef(GetValue((int) Fields.Permissions), 0); }
			set { SetValue((int) Fields.Permissions, value); }
		}
		
		public override string CanDelete()
		{
			string result="";
			
			//begin checkings... first check if the user is region supervisor:
			Region[] regions=this.GetSupervisedRegions();
			if ((regions != null)&&(regions.Length > 0))
			{
				string strRegions="";
				foreach (Region region in regions)
					strRegions += region.Name+"\n";
				result += "המשתמש '"+this.Name+"' מוגדר רכז עבור המחוזות הבאים: \n";
				result += strRegions+"\nלא ניתן למחוק משתמש זה";
			}
			
			if (result.Length == 0)
			{
				//maybe supervising championships?
				Championship[] champs=this.GetSupervisedChampionships();
				if ((champs != null)&&(champs.Length > 0))
				{
					string strChamps="";
					foreach (Championship champ in champs)
						strChamps += champ.Name+"\n";
					result += "המשתמש '"+this.Name+"' מוגדר אחראי עבור האליפויות הבאות: \n";
					result += strChamps+"\nלא ניתן למחוק משתמש זה";
				}
			}
			
			return result;
		}
		
		public Entities.Championship[] GetSupervisedChampionships()
		{
			EntityFilter filter=new EntityFilter(
				(int) Championship.Fields.Supervisor, this.Id);
			Entity[] entities=Entities.Championship.Type.GetEntities(filter);
			
			if ((entities != null)&&(entities.Length > 0))
			{
				Entities.Championship[] champs=
					new Entities.Championship[entities.Length];
				for (int i=0; i<entities.Length; i++)
				{
					champs[i] = new Entities.Championship(entities[i]);
				}
				return champs;
			}
			
			return null;
		}
		
		//return all instant messages this user sent or received.
		public Entities.InstantMessage[] GetInstantMessages()
		{
			//removed 18/05/2014 - not used, not needed any more.
			return new InstantMessage[] { };

			/*
			System.Collections.ArrayList result=new System.Collections.ArrayList();
			
			EntityFilter filter=new EntityFilter(
				(int) InstantMessage.Fields.Sender, this.Id);
			Entity[] entities=Entities.InstantMessage.Type.GetEntities(filter);
			if (entities != null)
			{
				foreach (Entity entity in entities)
				{
					InstantMessage message=new InstantMessage(entity);
					if (result.IndexOf(message) < 0)
						result.Add(message);
				}
			}
			
			filter = new EntityFilter(
				(int) InstantMessage.Fields.Recipient, this.Id);
			entities = Entities.InstantMessage.Type.GetEntities(filter);
			if (entities != null)
			{
				foreach (Entity entity in entities)
				{
					InstantMessage message=new InstantMessage(entity);
					if (result.IndexOf(message) < 0)
						result.Add(message);
				}
			}
			
			if (result.Count == 0)
				return null;
			return (InstantMessage[]) result.ToArray(typeof(InstantMessage));
			*/
		}
		
		public Entities.Region[] GetSupervisedRegions()
		{
			EntityFilter filter=new EntityFilter(
				(int) Region.Fields.Coordinator, this.Id);
			Entity[] entities=Entities.Region.Type.GetEntities(filter);
			
			if ((entities != null)&&(entities.Length > 0))
			{
				Entities.Region[] regions=new Entities.Region[entities.Length];
				for (int i=0; i<entities.Length; i++)
				{
					regions[i] = new Entities.Region(entities[i]);
				}
				return regions;
			}
			
			return null;
		}
		
		private static System.Collections.Hashtable _passwords=
			new System.Collections.Hashtable();
		
		public bool HasPassword()
		{
			if (!Core.Session.Connected)
				return false;
			
			int userID=this.Id;
			
			if (_passwords[userID] == null)
			{
				DataServices.DataService service=new DataServices.DataService();
				bool result=service.HasPassword(userID);
				_passwords[userID] = result;
			}
			
			return Convert.ToBoolean(_passwords[userID]);
		}
		
		public static int[] GetUsersWithoutPassword()
		{
			if (!Core.Session.Connected)
				return null;
			return (new DataServices.DataService().UsersWithoutPassword());
		}
	}
}
