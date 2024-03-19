using System;
using Sport.Data;
using Sport.Types;
using Sport.Core;

namespace Sport.Entities
{
	public class School : EntityBase
	{
		public static readonly string TypeName = "school";
		public static readonly EntityType Type;

		public enum Fields
		{
			Id = 0,
			Symbol,
			Name,
			City,
			Address,
			MailAddress,
			MailCity,
			ZipCode,
			Email,
			Phone,
			Fax,
			ManagerName,
			FromGrade,
			ToGrade,
			Supervision,
			Sector,
			Region,
			ClubStatus,
			PlayerNumberFrom,
			PlayerNumberTo,
			ManagerCellPhone,
			LastModified,
			DataFields,
			Category = DataFields,
			FieldCount
		}

		static School()
		{
			Type = new EntityType(TypeName, (int)Fields.FieldCount, (int)Fields.DataFields, new FieldEntityId((int)Fields.Id));

			Type[(int)Fields.City] = new EntityEntityField(Type, (int)Fields.City, City.TypeName);
			Type[(int)Fields.MailCity] = new EntityEntityField(Type, (int)Fields.MailCity, City.TypeName);
			Type[(int)Fields.Region] = new EntityEntityField(Type, (int)Fields.Region, Region.TypeName);
			Type[(int)Fields.ClubStatus] = new LookupEntityField(Type, (int)Fields.ClubStatus, new BooleanTypeLookup("מועדון רשום", "לא מועדון"));
			Type[(int)Fields.Id] = new NumberEntityField(Type, (int)Fields.Id);
			Type[(int)Fields.Id].CanEdit = false;
			//Type[(int) Fields.Symbol] = new NumberEntityField(Type, (int) Fields.Symbol, (double) 100000, (double) 9999999, 7, 0);
			Type[(int)Fields.Symbol].MustExist = true;
			Type[(int)Fields.Name].MustExist = true;
			Type[(int)Fields.Region].MustExist = true;
			Type[(int)Fields.ClubStatus].MustExist = true;
			Type[(int)Fields.PlayerNumberFrom] = new NumberEntityField(Type, (int)Fields.PlayerNumberFrom, 0, 99999);
			Type[(int)Fields.PlayerNumberTo] = new NumberEntityField(Type, (int)Fields.PlayerNumberTo, 0, 99999);
			Type[(int)Fields.Supervision] = new LookupEntityField(Type, (int)Fields.Supervision, new SchoolSupervisionLookup());
			Type[(int)Fields.Sector] = new LookupEntityField(Type, (int)Fields.Sector, new SchoolSectorLookup());
			Type[(int)Fields.LastModified] = new DateTimeEntityField(Type, (int)Fields.LastModified, "dd/MM/yyyy HH:mm:ss");
			Type[(int)Fields.LastModified].CanEdit = false;
			Type[(int)Fields.FromGrade] = new LookupEntityField(Type, (int)Fields.FromGrade, new GradeTypeLookup(false));
			Type[(int)Fields.ToGrade] = new LookupEntityField(Type, (int)Fields.ToGrade, new GradeTypeLookup(false));
			Type[(int)Fields.Category] = new CategoryFields(Type, (int)Fields.Category,
				(int)Fields.FromGrade, (int)Fields.ToGrade, -1);
			Type[(int)Fields.Category].CanEdit = false;

			Type.NameField = new FormatEntityField(Type, "{0} {1}",
				new int[] { (int)Fields.Name, (int)Fields.City });
			//Type[(int) Fields.Name];

			Type.DateLastModified = Type[(int)Fields.LastModified];
			Type.DateLastModified = Type[(int)Fields.LastModified];

			Type.NewEntity = new EntityType.EntityNewEntity(NewEntity);
			Type.EntityCheck += new EntityType.EntityEntityCheck(EntityCheck);
			Type.ValueChanged = new EntityType.EntityValueChange(ValueChanged);

			Type.RefreshRate = 120;
		}

		private static void NewEntity(EntityEdit entityEdit)
		{
			Type[(int)Fields.ClubStatus].SetValue(entityEdit, 1);
			Type[(int)Fields.FromGrade].SetValue(entityEdit, 1);
			Type[(int)Fields.ToGrade].SetValue(entityEdit, 12);
		}

		private static EntityResult EntityCheck(Entity entity)
		{
			School school = new School(entity);

			if (school.PlayerNumberFrom > 0 && school.PlayerNumberTo > 0)
			{
				if (school.PlayerNumberTo < school.PlayerNumberFrom)
					return new EntityMessageResult(EntityResultCode.ValueOutOfRange, "'עד מספר חולצה' חייב להיות גבוה מ'ממספר חולצה'");

				int myNumberFrom = school.PlayerNumberFrom;
				int myNumberTo = school.PlayerNumberTo;
				School[] otherSchools = school.Region.GetSchools();
				foreach (School otherSchool in otherSchools)
				{
					if (otherSchool.Id == school.Id)
						continue;
					int otherNumberFrom = otherSchool.PlayerNumberFrom;
					int otherNumberTo = otherSchool.PlayerNumberTo;
					if ((otherNumberTo < otherNumberFrom) || (otherNumberTo <= 0))
						continue;
					if (!school.Equals(otherSchool) && myNumberFrom <= otherNumberTo && myNumberTo >= otherNumberFrom)
						return new EntityMessageResult(EntityResultCode.UniqueValueFailure, "מספרי חולצה בקבוצה מתנגשים עם ביה\"ס '" + otherSchool.Name + "'");
				}
			}

			Data.EntityFilter filter = new EntityFilter((int)Fields.Symbol, school.Symbol);
			Data.Entity[] entities = School.Type.GetEntities(filter);
			if (entities != null && entities.Length > 0)
			{
				School existingSchool = null;
				foreach (Entity otherSchool in entities)
				{
					if (otherSchool.Id != school.Id)
					{
						existingSchool = new School(otherSchool);
						break;
					}
				}

				if (existingSchool != null)
				{
					return new EntityMessageResult(EntityResultCode.UniqueValueFailure, "בית ספר עם סמל זהה כבר קיים\n" +
						"שם בית הספר: " + existingSchool.Name + "\nמחוז: " + existingSchool.Region.Name);
				}
			}

			return EntityResult.Ok;
		}

		private static void ValueChanged(EntityEdit entityEdit, int field)
		{
			if (field == (int)Fields.Region)
			{
				Type[(int)Fields.City].SetValue(entityEdit, null);
				Type[(int)Fields.MailCity].SetValue(entityEdit, null);
			}
			else if (field == (int)Fields.PlayerNumberFrom)
			{
				object o = Type[(int)Fields.PlayerNumberFrom].GetValue(entityEdit);
				if (o == null)
					Type[(int)Fields.PlayerNumberTo].SetValue(entityEdit, null);
				else
					Type[(int)Fields.PlayerNumberTo].SetValue(entityEdit, (int)o + 19);
			}
		}

		public School(Entity entity)
			: base(entity)
		{
		}

		public School(EntityEdit entityEdit)
			: base(entityEdit)
		{
		}

		public School(int schoolId)
			: base(Type, schoolId)
		{
		}

		public string Symbol
		{
			get { return Common.Tools.CStrDef(GetValue((int)Fields.Symbol), ""); }
			set { SetValue((int)Fields.Symbol, value); }
		}

		public new string Name
		{
			get
			{
				string strSchoolName = Common.Tools.CStrDef(
					GetValue((int)Fields.Name), "").Trim();
				if (strSchoolName.Length == 0)
					return "";
				string strCityName = "";
				City city = this.City;
				if ((city != null) && (city.Id >= 0))
					strCityName = city.Name.Trim();
				if (strCityName.Length > 0)
				{
					if (strCityName.IndexOf("תל אביב") >= 0)
						strCityName = "תל אביב";
					string strInitials = Common.Tools.MakeInitials(strCityName);
					if (strCityName == "ראשון לציון")
						strInitials = "ראשל\"צ";
					if ((!Common.Tools.FindCityInSchool(strSchoolName, strCityName)) &&
						(strSchoolName.IndexOf(strInitials) < 0))
						strSchoolName += " " + strCityName;
				}
				return strSchoolName;
			}
			set { SetValue((int)Fields.Name, value); }
		}

		public City City
		{
			get
			{
				Entity entity = null;
				try
				{
					entity = GetEntityValue((int)Fields.City);
				}
				catch { }
				if ((entity == null) || (entity.Id < 0))
					return null;
				return new City(entity);
			}
			set
			{
				SetValue((int)Fields.City, value.Entity);
			}
		}

		public string Address
		{
			get { return Common.Tools.CStrDef(GetValue((int)Fields.Address), ""); }
			set { SetValue((int)Fields.Address, value); }
		}

		public string MailAddress
		{
			get { return Common.Tools.CStrDef(GetValue((int)Fields.MailAddress), ""); }
			set { SetValue((int)Fields.MailAddress, value); }
		}

		public City MailCity
		{
			get
			{
				Entity entity = null;
				try
				{
					entity = GetEntityValue((int)Fields.MailCity);
				}
				catch { }
				if (entity == null)
					return null;
				return new City(entity);
			}
			set
			{
				SetValue((int)Fields.MailCity, value.Entity);
			}
		}

		public string ZipCode
		{
			get { return Common.Tools.CStrDef(GetValue((int)Fields.ZipCode), ""); }
			set { SetValue((int)Fields.ZipCode, value); }
		}

		public string Phone
		{
			get { return Common.Tools.CStrDef(GetValue((int)Fields.Phone), ""); }
			set { SetValue((int)Fields.Phone, value); }
		}

		public string Fax
		{
			get { return Common.Tools.CStrDef(GetValue((int)Fields.Fax), ""); }
			set { SetValue((int)Fields.Fax, value); }
		}

		public string Email
		{
			get { return Common.Tools.CStrDef(GetValue((int)Fields.Email), ""); }
			set { SetValue((int)Fields.Email, value); }
		}

		public string ManagerName
		{
			get { return Common.Tools.CStrDef(GetValue((int)Fields.ManagerName), ""); }
			set { SetValue((int)Fields.ManagerName, value); }
		}

		public int FromGrade
		{
			get { return Common.Tools.CIntDef(GetValue((int)Fields.FromGrade), 0); }
			set { SetValue((int)Fields.FromGrade, value); }
		}

		public int ToGrade
		{
			get { return Common.Tools.CIntDef(GetValue((int)Fields.ToGrade), 0); }
			set { SetValue((int)Fields.ToGrade, value); }
		}

		public string Supervision
		{
			get { return Common.Tools.CStrDef(GetValue((int)Fields.Supervision), ""); }
			set { SetValue((int)Fields.Supervision, value); }
		}

		public int Sector
		{
			get { return Common.Tools.CIntDef(GetValue((int)Fields.Sector), 0); }
			set { SetValue((int)Fields.Sector, value); }
		}

		public Region Region
		{
			get
			{
				Entity entity = null;
				try
				{
					entity = GetEntityValue((int)Fields.Region);
				}
				catch { }
				if ((entity == null) || (entity.Id < 0))
					return null;
				return new Region(entity);
			}
			set
			{
				SetValue((int)Fields.Region, value.Entity);
			}
		}

		public int ClubStatus
		{
			get { return Common.Tools.CIntDef(GetValue((int)Fields.ClubStatus), 0); }
			set { SetValue((int)Fields.ClubStatus, value); }
		}

		public int PlayerNumberFrom
		{
			get { return Common.Tools.CIntDef(GetValue((int)Fields.PlayerNumberFrom), -1); }
			set
			{
				if (value < 0)
					SetValue((int)Fields.PlayerNumberFrom, null);
				else
					SetValue((int)Fields.PlayerNumberFrom, value);
			}
		}

		public int PlayerNumberTo
		{
			get { return Common.Tools.CIntDef(GetValue((int)Fields.PlayerNumberTo), 0); }
			set
			{
				if (value < 0)
					SetValue((int)Fields.PlayerNumberTo, null);
				else
					SetValue((int)Fields.PlayerNumberTo, value);
			}
		}

		public string ManagerCellPhone
		{
			get { return Common.Tools.CStrDef(GetValue((int)Fields.ManagerCellPhone), ""); }
			set { SetValue((int)Fields.ManagerCellPhone, value); }
		}

		public int StudentsCount
		{
			get
			{
				RegistrationServices.RegistrationService regService = new RegistrationServices.RegistrationService();
				regService.CookieContainer = Core.Session.Cookies;
				int schoolID = this.Id;
				int resultNumber = regService.GetStudentsCount(schoolID);
				if (resultNumber == -1)
					throw new Exception("Invalid school!");
				return resultNumber;
			}
		}

		public bool ValidShirtRange()
		{
			int min = this.PlayerNumberFrom;
			int max = this.PlayerNumberTo;
			if (min < 0)
				return false;
			if (max < min)
				return false;
			if ((min == 0) && (max == 0))
				return false;
			return true;
		}

		public override string CanDelete()
		{
			//begin checkings... first check if the school is part of any championship:
			string strTeamsMessage = CanDelete(this.GetTeams());

			//check if the school has functionaries:
			string strFunctionariesMessage = CanDelete(this.GetFunctionaries());

			//any user associated with the school?
			string strUsersMessage = CanDelete(this.GetUsers());

			//account maybe?
			string strAccountMessage = string.Empty;
			Account account = this.GetAccount();
			if (account != null)
			{
				strAccountMessage = "לבית הספר '" + this.Name + "' משוייך החשבון '" + account.Name + "'\n";
				strAccountMessage += "יש למחוק חשבון זה ממאגר הנתונים";
			}

			return Utils.JoinNonEmpty("\n\n", new string[] { strTeamsMessage, strFunctionariesMessage, strUsersMessage, strAccountMessage });
		}

		protected string CanDelete(Team[] teams)
		{
			if (teams == null || teams.Length == 0)
				return string.Empty;

			int count = 0;
			string championships = "";
			string result = "";
			foreach (Team team in teams)
			{
				Championship champ = team.Championship;
				if (champ != null)
				{
					if (count >= 5)
					{
						championships += "...\n";
						break;
					}
					else
					{
						championships += champ.Name + " " + team.Category.Name + "\n";
					}
					count++;
				}
			}

			if (championships.Length > 0)
			{
				result += "בית הספר '" + this.Name + "' רשום לאליפויות הבאות: \n";
				result += championships + "יש להסיר את בית הספר מאליפויות אלו";
			}

			return result;
		}

		protected string CanDelete(Functionary[] functionaries)
		{
			if (functionaries == null || functionaries.Length == 0)
				return string.Empty;

			int count = 0;
			string strFuncs = "";
			string result = "";
			Types.FunctionaryTypeLookup lookup = new Types.FunctionaryTypeLookup();
			foreach (Functionary functionary in functionaries)
			{
				if (count >= 5)
				{
					strFuncs += "...\n";
					break;
				}
				strFuncs += functionary.Name + " (" + lookup.Lookup((int)functionary.FunctionaryType) + ")\n";
				count++;
			}

			if (strFuncs.Length > 0)
			{
				result += "לבית הספר '" + this.Name + "' משוייכים בעלי התפקידים הבאים: \n";
				result += strFuncs + "יש למחוק בעלי תפקידים אלו ממאגר הנתונים";
			}

			return result;
		}

		protected string CanDelete(User[] users)
		{
			if (users == null || users.Length == 0)
				return string.Empty;

			int count = 0;
			string strUsers = "";
			string result = "";
			foreach (User user in users)
			{
				if (count >= 5)
				{
					strUsers += "...\n";
					break;
				}
				strUsers += user.Login + " (" + user.Name + ")\n";
				count++;
			}

			if (strUsers.Length > 0)
			{
				result += "לבית הספר '" + this.Name + "' משוייכים המשתמשים הבאים: \n";
				result += strUsers + "יש למחוק משתמשים אלו ממאגר הנתונים";
			}

			return result;
		}

		public Team[] GetTeams()
		{
			EntityFilter filter = new EntityFilter((int)Team.Fields.School, Id);
			Entity[] entities = Team.Type.GetEntities(filter);
			Team[] teams = new Team[entities.Length];
			for (int i = 0; i < entities.Length; i++)
			{
				teams[i] = new Team(entities[i]);
			}
			return teams;
		}

		public Account GetAccount()
		{
			if (this.Id < 0)
				return null;

			Region region = this.Region;
			if ((region == null) || (region.Id < 0))
				return null;

			EntityFilter filter = new EntityFilter((int)Account.Fields.Region, Region.Id);
			filter.Add(new EntityFilterField((int)Account.Fields.School, Id));
			Entity[] entities = Account.Type.GetEntities(filter);
			if ((entities == null) || (entities.Length == 0))
				return null;

			return new Account(entities[0]);
		}

		public Functionary[] GetFunctionaries()
		{
			EntityFilter filter = new EntityFilter(
				(int)Functionary.Fields.School, this.Id);
			Entity[] entities = Functionary.Type.GetEntities(filter);
			System.Collections.ArrayList result = new System.Collections.ArrayList();
			if (entities != null)
			{
				foreach (Entity entity in entities)
					result.Add(new Functionary(entity));
			}
			return (Functionary[])result.ToArray(typeof(Functionary));
		}

		public Functionary GetFunctionary(FunctionaryType type)
		{
			Functionary[] arrFunctionaries = this.GetFunctionaries();
			foreach (Functionary functionary in arrFunctionaries)
			{
				if (functionary.FunctionaryType == type)
					return functionary;
			}
			return null;
		}

		public Functionary GetCoordinator()
		{
			return GetFunctionary(FunctionaryType.Coordinator);
		}

		public User[] GetUsers()
		{
			EntityFilter filter = new EntityFilter((int)User.Fields.School, this.Id);
			Entity[] entities = User.Type.GetEntities(filter);
			System.Collections.ArrayList result = new System.Collections.ArrayList();
			if (entities != null)
			{
				foreach (Entity entity in entities)
					result.Add(new User(entity));
			}
			return (User[])result.ToArray(typeof(User));
		}

		public Data.EntityResult UpdateCoordinatorData(string name, string address, string phone,
			string fax, string zipCode, string email, string cellPhone)
		{
			Functionary functionary = GetCoordinator();
			if (functionary == null)
			{
				EntityType funcType = Functionary.Type;
				EntityEdit funcEdit = funcType.New();
				functionary = new Functionary(funcEdit);
				functionary.Region = this.Region;
				functionary.City = this.City;
				functionary.School = this;
				functionary.FunctionaryType = FunctionaryType.Coordinator;
			}
			else
			{
				functionary.Edit();
			}
			functionary.Name = name;
			functionary.Address = address;
			functionary.Phone = phone;
			functionary.Fax = fax;
			functionary.ZipCode = zipCode;
			functionary.Email = email;
			functionary.CellPhone = cellPhone;
			return functionary.Save(false);
		}

		public static School FromSymbol(string strSymbol)
		{
			Data.EntityFilter filter = new Data.EntityFilter((int)Fields.Symbol, strSymbol);
			Data.Entity[] arrSchools = School.Type.GetEntities(filter);
			School school = null;
			if ((arrSchools != null) && (arrSchools.Length > 0))
			{
				try
				{
					school = new School(arrSchools[0]);
				}
				catch
				{
					System.Diagnostics.Debug.WriteLine("failed to get school from symbol: " + strSymbol);
				}
			}
			return school;
		}
	}

	public class OfflineSchool : Data.OfflineEntity
	{
		private string _symbol = "";
		private string _name = "";
		private int _region = -1;
		private int _city = -1;

		protected override string GetOfflineType()
		{
			return "school";
		}

		public string Symbol
		{
			get { return _symbol; }
			set { _symbol = value; }
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public int Region
		{
			get { return _region; }
			set { _region = value; }
		}

		public int City
		{
			get { return _city; }
			set { _city = value; }
		}

		public OfflineSchool()
			: base()
		{
		}

		public OfflineSchool(string symbol, string name, int region, int city)
			: this()
		{
			this.Symbol = symbol;
			this.Name = name;
			this.Region = region;
			this.City = city;
		}

		protected override OfflineField[] GetFields()
		{
			return new OfflineField[] {
					new OfflineField("symbol", this.Symbol), 
					new OfflineField("name", this.Name), 
					new OfflineField("region", this.Region.ToString()), 
					new OfflineField("city", this.City.ToString())
			};
		}

		protected override OfflineEntity GetExistingEntity()
		{
			OfflineEntity[] schools = OfflineEntity.LoadAllEntities(this.GetType());
			foreach (OfflineSchool school in schools)
				if (school.Symbol == this.Symbol)
					return school;
			return null;
		}

		protected override void Load(OfflineField[] fields)
		{
			foreach (OfflineField field in fields)
			{
				string name = field.Name;
				string value = field.Value;
				if (name == "symbol")
					this.Symbol = value;
				else if (name == "name")
					this.Name = value;
				else if (name == "region")
					this.Region = Int32.Parse(value);
				else if (name == "city")
					this.City = Int32.Parse(value);
			}
		}

		public override string ToString()
		{
			return this.Name;
		}

		public override int GetHashCode()
		{
			return this.Symbol.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			if (!(obj is OfflineSchool))
				return false;
			return this.Symbol.Equals((obj as OfflineSchool).Symbol);
		}
	}
}
