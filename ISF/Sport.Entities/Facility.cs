using System;
using Sport.Data;
using System.Collections;

namespace Sport.Entities
{
	public class Facility : EntityBase
	{
		public static readonly string TypeName = "facility";
		public static readonly EntityType Type;

		public enum Fields
		{
			Id = 0,
			Name,
			Region,
			School,
			Address,
			Phone,
			Fax,
			City,
			Number,
			LastModified,
			FieldCount
		}

		static Facility()
		{
			Type = new EntityType(TypeName, (int) Fields.FieldCount, (int) Fields.FieldCount, new FieldEntityId((int) Fields.Id));

			Type[(int) Fields.Id] = new NumberEntityField(Type, (int) Fields.Id);
			Type[(int) Fields.Id].CanEdit = false;
			Type[(int) Fields.Name].MustExist = true;
			Type[(int) Fields.Region] = new EntityEntityField(Type, (int) Fields.Region, Region.TypeName);
			Type[(int) Fields.School] = new EntityEntityField(Type, (int) Fields.School, School.TypeName);
			Type[(int) Fields.City] = new EntityEntityField(Type, (int) Fields.City, City.TypeName);
			Type[(int) Fields.Number] = new NumberEntityField(Type, (int) Fields.Number);
			Type[(int) Fields.LastModified] = new DateTimeEntityField(Type, (int) Fields.LastModified, "dd/MM/yyyy HH:mm:ss");
			Type[(int) Fields.LastModified].CanEdit = false;

			Type.NameField = Type[(int) Fields.Name];
			Type.DateLastModified = Type[(int) Fields.LastModified];
		}

		public Facility(Entity entity)
			: base(entity)
		{
		}

		public Facility(EntityEdit entityEdit)
			: base(entityEdit)
		{
		}

		public Facility(int facilityId)
			: base(Type, facilityId)
		{
		}

		public new string Name
		{
			get { return base.Name; }
			set { SetValue((int) Fields.Name, value); }
		}

		public Region Region
		{
			get 
			{
				Entity entity = GetEntityValue((int) Fields.Region);
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
				Entity entity = GetEntityValue((int) Fields.School);
				if (entity == null)
					return null;
				return new School(entity);
			}
			set 
			{ 
				SetValue((int) Fields.School, value.Entity); 
			}
		}
		
		public City City
		{
			get 
			{
				Entity entity = GetEntityValue((int) Fields.City);
				if (entity == null)
					return null;
				return new City(entity);
			}
			set 
			{ 
				SetValue((int) Fields.City, value.Entity); 
			}
		}
		
		public string Address
		{
			get { return Common.Tools.CStrDef(GetValue((int) Fields.Address), ""); }
			set { SetValue((int) Fields.Address, value); }
		}

		public string Phone
		{
			get { return Common.Tools.CStrDef(GetValue((int) Fields.Phone), ""); }
			set { SetValue((int) Fields.Phone, value); }
		}
		
		public string Fax
		{
			get { return Common.Tools.CStrDef(GetValue((int) Fields.Fax), ""); }
			set { SetValue((int) Fields.Fax, value); }
		}
		
		public int Number
		{
			get { return Common.Tools.CIntDef(GetValue((int) Fields.Number), 0); }
			set { SetValue((int) Fields.Number, value); }
		}
		
		public Court[] GetCourts()
		{
			Court[] result;
			Entity[] courts = Court.Type.GetEntities(
				new EntityFilter((int) Court.Fields.Facility, Id));
			result = new Court[courts.Length];
			for (int i=0; i<courts.Length; i++)
			{
				result[i] = new Court(courts[i]);
			}
			return result;
		}

		public Court[] GetCourts(Sport sport)
		{
			System.Collections.ArrayList al = new System.Collections.ArrayList();

			Entity[] sportCourts = CourtTypeSport.Type.GetEntities(
				new EntityFilter((int) CourtTypeSport.Fields.Sport, sport.Id));

			Entity[] courts = Court.Type.GetEntities(
				new EntityFilter((int) Court.Fields.Facility, Id));

			foreach (Entity entity in courts)
			{
				Court court = new Court(entity);

				bool added = false;
				for (int n = 0; n < sportCourts.Length && !added; n++)
				{
					if (court.CourtType == null ||
						court.CourtType.Id == (int) sportCourts[n].Fields[(int) CourtTypeSport.Fields.CourtType])
					{
						added = true;
						al.Add(court);
					}
				}
			}

			return (Court[]) al.ToArray(typeof(Court));
		}
		
		public override string CanDelete()
		{
			//begin checkings... first check for courts:
			Court[] courts=this.GetCourts();
			if (courts.Length > 0)
			{
				string names="";
				for (int i=0; i<courts.Length; i++)
				{
					names += courts[i].Name+"\n";
					if (i >= 15)
					{
						names += "...\n";
						break;
					}
				}
				return "המתקן '"+this.Name+"' כולל את המגרשים הבאים: \n"+names+
					"יש להסיר מגרשים אלו";
			}
			
			//initialize service:
			DataServices.DataService dataService=new DataServices.DataService();
			
			//check if part of any competition:
			int[] arrCategoriesID=
				dataService.GetContainingChampionships(-1, this.Id, -1);
			
			if ((arrCategoriesID != null)&&(arrCategoriesID.Length > 0))
			{
				string strChamps="";
				foreach (int categoryID in arrCategoriesID)
				{
					ChampionshipCategory category=null;
					try
					{
						category = new ChampionshipCategory(categoryID);
					}
					catch {}
					if ((category != null)&&(category.Championship != null))
						strChamps += category.Championship.Name+" "+category.Name+"\n";
				}
				
				if (strChamps.Length > 0)
				{
					return "באליפויות הבאות יש תחרויות הנערכות במתקן זה:\n"+strChamps+
						"\nיש להסיר תחרויות אלו מהאליפות";
				}
			}
			
			return "";
		}
		
		public int GetTripRate(Functionary functionary)
		{
			string key="f"+this.Id+"f"+functionary.Id;
			string strRate = Core.Configuration.ReadString("TripRates", key);
			if ((strRate == null)||(strRate.Length == 0))
				return 0;
			if (!Common.Tools.IsInteger(strRate))
				return 0;
			return Int32.Parse(strRate);
		}
		
		public void SetTripRate(Functionary functionary, int rate)
		{
			string key="f"+this.Id+"f"+functionary.Id;
			Core.Configuration.WriteString("TripRates", key, rate.ToString());
		}
	}
}
