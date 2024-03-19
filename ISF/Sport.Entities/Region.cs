using System;
using Sport.Data;

namespace Sport.Entities
{
	/// <summary>
	/// Implement an EntityType for a Region
	/// </summary>
	public class Region : EntityBase
	{
		public static readonly int CentralRegion = 0;

		public static readonly string TypeName = "region";
		public static readonly EntityType Type;

		public enum Fields
		{
			Id = 0,
			Number,
			Name,
			Address,
			Phone,
			Fax,
			Coordinator,
			LastModified,
			FieldCount
		}

		public static EntityField IdField
		{
			get { return Type.Fields[(int) Fields.Id]; }
		}

		static Region()
		{
			Type = new EntityType(TypeName, (int) Fields.FieldCount, (int) Fields.FieldCount, new FieldEntityId((int) Fields.Id));

			Type[(int) Fields.Id] = new NumberEntityField(Type, (int) Fields.Id);
			Type[(int) Fields.Id].CanEdit = false;
			Type[(int) Fields.Number] = new NumberEntityField(Type, (int) Fields.Number, 0, 9);
			Type[(int) Fields.Number].MustExist = true;
			Type[(int) Fields.Name].MustExist = true;
			Type[(int) Fields.Coordinator] = new EntityEntityField(Type, (int) Fields.Coordinator, User.TypeName);
			Type[(int) Fields.LastModified] = new DateTimeEntityField(Type, (int) Fields.LastModified, "dd/MM/yyyy HH:mm:ss");
			Type[(int) Fields.LastModified].CanEdit = false;

			Type.NameField = Type[(int) Fields.Name];
			Type.DateLastModified = Type[(int) Fields.LastModified];

			Type.RefreshRate = 120;
		}

		public Region(Entity entity)
			: base(entity)
		{
		}

		public Region(EntityEdit entityEdit)
			: base(entityEdit)
		{
		}

		public Region(int regionId)
			: base(Type, regionId)
		{
		}

		public int Number
		{
			get { return (int) GetValue((int) Fields.Number); }
			set { SetValue((int) Fields.Number, value); }
		}

		public new string Name
		{
			get
			{ return base.Name; }
			set { SetValue((int) Fields.Name, value); }
		}

		public string Address
		{
			get { return (string) GetValue((int) Fields.Address); }
			set { SetValue((int) Fields.Address, value); }
		}

		public string Phone
		{
			get { return (string) GetValue((int) Fields.Phone); }
			set { SetValue((int) Fields.Phone, value); }
		}
		
		public string Fax
		{
			get { return (string) GetValue((int) Fields.Fax); }
			set { SetValue((int) Fields.Fax, value); }
		}

		public User Coordinator
		{
			get
			{
				Entity entity = GetEntityValue((int) Fields.Coordinator);
				if (entity == null)
					return null;
				return new User(entity);
			}
			set 
			{ 
				SetValue((int) Fields.Coordinator, value.Entity); 
			}
		}

		public Championship[] GetChampionships()
		{
			EntityFilter filter=new EntityFilter(
				(int) Championship.Fields.Region, this.Id);
			filter.Add(Championship.CurrentSeasonFilter());
			Entity[] champs=Championship.Type.GetEntities(filter);
			Championship[] result=new Championship[champs.Length];
			for (int i=0; i<champs.Length; i++)
			{
				result[i] = new Championship(champs[i]);
			}
			return result;
		}

		public City[] GetCities()
		{
			EntityFilter filter=new EntityFilter(
				(int) City.Fields.Region, this.Id);
			Entity[] cities=City.Type.GetEntities(filter);
			City[] result=new City[cities.Length];
			for (int i=0; i<cities.Length; i++)
			{
				result[i] = new City(cities[i]);
			}
			return result;
		}

		public School[] GetSchools()
		{
			EntityFilter filter = new EntityFilter((int) School.Fields.Region, this.Id);
			Entity[] schools = School.Type.GetEntities(filter);
			School[] result = new School[schools.Length];
			for (int n = 0; n < schools.Length; n++)
				result[n] = new School(schools[n]);
			return result;
		}

		public Account[] GetAccounts()
		{
			EntityFilter filter = new EntityFilter((int) Account.Fields.Region, this.Id);
			Entity[] accounts = Account.Type.GetEntities(filter);
			Account[] result = new Account[accounts.Length];
			for (int n = 0; n < accounts.Length; n++)
				result[n] = new Account(accounts[n]);
			return result;
		}

		public Equipment[] GetEquipments()
		{
			Entity[] entities = Equipment.Type.GetEntities(
				new EntityFilter((int) Equipment.Fields.Region, Id));
			
			if (entities == null)
				return null;
			
			Equipment[] result = new Equipment[entities.Length];
			for (int t = 0; t < entities.Length; t++)
				result[t] = new Equipment(entities[t]);
			
			return result;
		}
		
		public User[] GetUsers()
		{
			EntityFilter filter = new EntityFilter((int) User.Fields.Region, this.Id);
			Entity[] users = User.Type.GetEntities(filter);
			User[] result = new User[users.Length];
			for (int n = 0; n < users.Length; n++)
				result[n] = new User(users[n]);
			return result;
		}

		public int GetSchoolsCount()
		{
			EntityFilter filter=new EntityFilter(
				(int) School.Fields.Region, this.Id);
			Entity[] schools=School.Type.GetEntities(filter);
			int result=schools.Length;
			return result;
		}

		public bool IsNationalRegion()
		{
			return ((this.Id == Region.CentralRegion)||(this.Name == "ISF"));
		}
	}
}
