using System;
using Sport.Data;
using Sport.Types;

namespace Sport.Entities
{
	public class Functionary : EntityBase
	{
		public static readonly string TypeName = "functionary";
		public static readonly EntityType Type;

		public enum Fields
		{
			Id = 0,
			Name,
			Type,
			Region,
			City,
			School,
			Address,
			Phone,
			Fax,
			ZipCode,
			Email,
			CellPhone,
			Number,
			ID_Number,
			Status,
			HasAnotherJob,
			WorkEnviroment,
			SexType,
			BirthDate,
			Seniority,
			Payment,
			Remarks,
			GotSticker,
			LastModified,
			FieldCount
		}

		static Functionary()
		{
			Type = new EntityType(TypeName, (int)Fields.FieldCount, (int)Fields.FieldCount, new FieldEntityId((int)Fields.Id));

			Type[(int)Fields.Id] = new NumberEntityField(Type, (int)Fields.Id);
			Type[(int)Fields.Id].CanEdit = false;
			Type[(int)Fields.Name].MustExist = true;
			Type[(int)Fields.Type] = new LookupEntityField(Type, (int)Fields.Type, new FunctionaryTypeLookup());
			Type[(int)Fields.Type].MustExist = true;
			Type[(int)Fields.Region] = new EntityEntityField(Type, (int)Fields.Region, Region.TypeName);
			Type[(int)Fields.City] = new EntityEntityField(Type, (int)Fields.City, City.TypeName);
			Type[(int)Fields.School] = new EntityEntityField(Type, (int)Fields.School, School.TypeName);
			Type[(int)Fields.Number] = new NumberEntityField(Type, (int)Fields.Number);
			Type[(int)Fields.ID_Number] = new NumberEntityField(Type, (int)Fields.ID_Number);
			Type[(int)Fields.Status] = new LookupEntityField(Type, (int)Fields.Status, new FunctionaryStatusTypeLookup());
			Type[(int)Fields.HasAnotherJob] = new LookupEntityField(Type, (int)Fields.HasAnotherJob, new BooleanTypeLookup());
			Type[(int)Fields.WorkEnviroment] = new LookupEntityField(Type, (int)Fields.WorkEnviroment, new FunctionaryWorkEnviromentLookup());
			Type[(int)Fields.SexType] = new LookupEntityField(Type, (int)Fields.SexType, new SexTypeLookup());
			Type[(int)Fields.BirthDate] = new DateTimeEntityField(Type, (int)Fields.BirthDate, "dd/MM/yyyy");
			Type[(int)Fields.Seniority] = new NumberEntityField(Type, (int)Fields.Seniority, 0, 100);
			Type[(int)Fields.GotSticker] = new LookupEntityField(Type, (int)Fields.GotSticker, new BooleanTypeLookup("יש מדבקה", "אין מדבקה"));
			Type[(int)Fields.LastModified] = new DateTimeEntityField(Type, (int)Fields.LastModified, "dd/MM/yyyy HH:mm:ss");
			Type[(int)Fields.LastModified].CanEdit = false;

			Type.NameField = Type[(int)Fields.Name];
			Type.DateLastModified = Type[(int)Fields.LastModified];

			Type.NewEntity = new EntityType.EntityNewEntity(NewEntity);
		}

		private static void NewEntity(EntityEdit entityEdit)
		{
			Type[(int)Fields.Type].SetValue(entityEdit, 1);
		}

		public Functionary(Entity entity)
			: base(entity)
		{
		}

		public Functionary(EntityEdit entityEdit)
			: base(entityEdit)
		{
		}

		public Functionary(int facilityId)
			: base(Type, facilityId)
		{
		}

		public new string Name
		{
			get { return Common.Tools.CStrDef(base.Name, ""); }
			set { SetValue((int)Fields.Name, value); }
		}

		public Region Region
		{
			get
			{
				Entity entity = GetEntityValue((int)Fields.Region);
				if (entity == null)
					return null;
				return new Region(entity);
			}
			set
			{
				SetValue((int)Fields.Region, (value == null ? null : value.Entity));
			}
		}

		public School School
		{
			get
			{
				Entity entity = GetEntityValue((int)Fields.School);
				if (entity == null)
					return null;
				return new School(entity);
			}
			set
			{
				SetValue((int)Fields.School, (value == null ? null : value.Entity));
			}
		}

		public City City
		{
			get
			{
				Entity entity = GetEntityValue((int)Fields.City);
				if (entity == null)
					return null;
				return new City(entity);
			}
			set
			{
				SetValue((int)Fields.City, (value == null ? null : value.Entity));
			}
		}

		public FunctionaryType FunctionaryType
		{
			get
			{
				object value = GetValue((int)Fields.Type);
				if (value == null)
					return FunctionaryType.UNDEFINED;
				return (FunctionaryType)value;
			}
			set { SetValue((int)Fields.Type, (int)value); }
		}

		public string Address
		{
			get { return Common.Tools.CStrDef(GetValue((int)Fields.Address), ""); }
			set { SetValue((int)Fields.Address, value); }
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

		public string ZipCode
		{
			get { return Common.Tools.CStrDef(GetValue((int)Fields.ZipCode), ""); }
			set { SetValue((int)Fields.ZipCode, value); }
		}

		public string Email
		{
			get { return Common.Tools.CStrDef(GetValue((int)Fields.Email), ""); }
			set { SetValue((int)Fields.Email, value); }
		}

		public string CellPhone
		{
			get { return Common.Tools.CStrDef(GetValue((int)Fields.CellPhone), ""); }
			set { SetValue((int)Fields.CellPhone, value); }
		}

		public int Number
		{
			get { return Common.Tools.CIntDef(GetValue((int)Fields.Number), 0); }
			set { SetValue((int)Fields.Number, value); }
		}

		public string IdNumber
		{
			get { return Common.Tools.CIntDef(GetValue((int)Fields.ID_Number), 0).ToString(); }
			set { SetValue((int)Fields.ID_Number, Common.Tools.CIntDef(value, 0)); }
		}

		public FunctionaryStatus Status
		{
			get
			{
				object value = GetValue((int)Fields.Status);
				return (value == null) ? FunctionaryStatus.UNDEFINED : (FunctionaryStatus)value;
			}
			set { SetValue((int)Fields.Status, (int)value); }
		}

		public bool HasAnotherJob
		{
			get { return (Common.Tools.CIntDef(GetValue((int)Fields.HasAnotherJob), 0) == 1); }
			set { SetValue((int)Fields.HasAnotherJob, (value) ? 1 : 0); }
		}

		public FunctionaryWorkEnviroment WorkEnviroment
		{
			get
			{
				object value = GetValue((int)Fields.WorkEnviroment);
				return (value == null) ? FunctionaryWorkEnviroment.UNDEFINED :
					(FunctionaryWorkEnviroment)value;
			}
			set { SetValue((int)Fields.WorkEnviroment, (int)value); }
		}

		public Sex SexType
		{
			get
			{
				object value = GetValue((int)Fields.SexType);
				return (value == null) ? Types.Sex.None : (Sex)value;
			}
			set { SetValue((int)Fields.SexType, (int)value); }
		}

		public DateTime BirthDate
		{
			get
			{
				object value = GetValue((int)Fields.BirthDate);
				return (value == null) ? (DateTime.MinValue) : ((DateTime)value);
			}
			set { SetValue((int)Fields.BirthDate, value); }
		}

		public int Seniority
		{
			get { return Common.Tools.CIntDef(GetValue((int)Fields.Seniority), 0); }
			set { SetValue((int)Fields.Seniority, value); }
		}

		public string Payment
		{
			get { return Common.Tools.CStrDef(GetValue((int)Fields.Payment), ""); }
			set { SetValue((int)Fields.Payment, value); }
		}

		public string Remarks
		{
			get { return Common.Tools.CStrDef(GetValue((int)Fields.Remarks), ""); }
			set { SetValue((int)Fields.Remarks, value); }
		}

		public bool GotSticker
		{
			get { return Common.Tools.CIntDef(GetValue((int)Fields.GotSticker), 0) == 1; }
			set { SetValue((int)Fields.GotSticker, (value) ? 1 : 0); }
		}

		public override string CanDelete()
		{
			string result = "";

			//begin checkings... first check if the functionary is part of any games.
			ChampionshipCategory[] categories = this.GetChampionships();
			if ((categories != null) && (categories.Length > 0))
			{
				FunctionaryTypeLookup funcLookup = new FunctionaryTypeLookup();
				int funcType = Common.Tools.CIntDef(
					this.Entity.Fields[(int)Fields.Type], 0);
				string championships = "";
				string strFuncType = "ה" + funcLookup.Lookup(funcType);
				foreach (ChampionshipCategory category in categories)
				{
					if (category != null && category.Id > 0 && category.Championship != null)
					{
						championships += category.Championship.Name + " " + category.Name + "\n";
					}
				}
				if (championships.Length > 0)
				{
					result += strFuncType + " '" + this.Name + "' רשום לאליפויות הבאות: \n";
					result += championships + "יש להסיר את " + strFuncType + " מאליפויות אלו\n";
				}
			}

			return result;
		}

		public ChampionshipCategory[] GetChampionships()
		{
			if (!Core.Session.Connected)
				return null;

			DataServices.DataService service = new DataServices.DataService();
			int[] arrCategories = service.GetFunctionaryGames(this.Id);
			System.Collections.ArrayList result = new System.Collections.ArrayList();
			if ((arrCategories != null) && (arrCategories.Length > 0))
			{
				foreach (int categoryID in arrCategories)
				{
					ChampionshipCategory category = null;
					try
					{
						category = new ChampionshipCategory(categoryID);
					}
					catch { }
					if ((category != null) && (category.Id >= 0))
					{
						if (result.IndexOf(category) < 0)
							result.Add(category);
					}
				}
			}
			return (ChampionshipCategory[])result.ToArray(typeof(ChampionshipCategory));
		}

		public DataServices.FunctionaryMatchData[] GetMatchData(DateTime start,
			DateTime end)
		{
			DataServices.DataService service = new DataServices.DataService();
			return service.GetFunctionaryGamesByDate(this.Id, start, end);
		}
	}
}
