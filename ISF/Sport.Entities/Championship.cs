using System;
using System.Linq;
using Sport.Data;
using Sport.Types;
using Sport.Core;
using System.Collections;
using System.Collections.Generic;

namespace Sport.Entities
{
	public class Championship : EntityBase
	{
		public static readonly string TypeName = "championship";
		public static readonly EntityType Type;

		public enum Fields
		{
			Id = 0,
			Season,
			Name,
			Region,
			Sport,
			IsClubs,
			Status,
			LastRegistrationDate,
			StartDate,
			EndDate,
			AltStartDate,
			AltEndDate,
			FinalsDate,
			AltFinalsDate,
			Ruleset,
			IsOpen,
			Supervisor,
			StandardChampionship,
			Number,
			Remarks,
			LastModified,
			DataFields,
			ChampionshipDates = DataFields,
			FinalsDates,
			FieldCount
		}

		public void Init()
		{
		}

		static Championship()
		{
			Type = new EntityType(TypeName, (int) Fields.FieldCount, (int) Fields.DataFields, new FieldEntityId((int) Fields.Id), Session.GetSeasonCache());

			Type[(int) Fields.Id] = new NumberEntityField(Type, (int) Fields.Id);
			Type[(int) Fields.Id].CanEdit = false;
			Type[(int) Fields.Season] = new EntityEntityField(Type, (int) Fields.Season, Season.TypeName);
			Type[(int) Fields.Season].MustExist = true;
			Type[(int) Fields.Name].MustExist = true;
			Type[(int) Fields.IsClubs] = new LookupEntityField(Type, (int) Fields.IsClubs, new BooleanTypeLookup());
			Type[(int) Fields.IsClubs].MustExist = true;
			Type[(int) Fields.Region] = new EntityEntityField(Type, (int) Fields.Region, Region.TypeName);
			Type[(int) Fields.Sport] = new EntityEntityField(Type, (int) Fields.Sport, Sport.TypeName);
			Type[(int) Fields.Sport].MustExist = true;
			Type[(int) Fields.Status] = new LookupEntityField(Type, (int) Fields.Status, new ChampionshipStatusLookup());
			Type[(int) Fields.Status].MustExist = true;
			Type[(int) Fields.LastRegistrationDate] = new DateTimeEntityField(Type, (int) Fields.LastRegistrationDate, "dd/MM/yyyy");
			Type[(int) Fields.StartDate] = new DateTimeEntityField(Type, (int) Fields.StartDate, "dd/MM/yyyy");
			Type[(int) Fields.EndDate] = new DateTimeEntityField(Type, (int) Fields.EndDate, "dd/MM/yyyy");
			Type[(int) Fields.AltStartDate] = new DateTimeEntityField(Type, (int) Fields.AltStartDate, "dd/MM/yyyy");
			Type[(int) Fields.AltEndDate] = new DateTimeEntityField(Type, (int) Fields.AltEndDate, "dd/MM/yyyy");
			Type[(int) Fields.FinalsDate] = new DateTimeEntityField(Type, (int) Fields.FinalsDate, "dd/MM/yyyy");
			Type[(int) Fields.AltFinalsDate] = new DateTimeEntityField(Type, (int) Fields.AltFinalsDate, "dd/MM/yyyy");
			Type[(int) Fields.ChampionshipDates] = new DateSetEntityField(Type, (int) Fields.ChampionshipDates, (int) Fields.StartDate, (int) Fields.EndDate, (int) Fields.AltStartDate, (int) Fields.AltEndDate);
			Type[(int) Fields.FinalsDates] = new DateSetEntityField(Type, (int) Fields.FinalsDates, (int) Fields.FinalsDate, (int) Fields.AltFinalsDate);
			Type[(int) Fields.Ruleset] = new EntityEntityField(Type, (int) Fields.Ruleset, Ruleset.TypeName);
			Type[(int) Fields.IsOpen] = new LookupEntityField(Type, (int) Fields.IsOpen, new BooleanTypeLookup("פתוחה", "סגורה"));
			Type[(int) Fields.Supervisor] = new EntityEntityField(Type, (int) Fields.Supervisor, User.TypeName);
			Type[(int) Fields.StandardChampionship] = new EntityEntityField(Type, (int) Fields.StandardChampionship, StandardChampionship.TypeName);
			Type[(int) Fields.LastModified] = new DateTimeEntityField(Type, (int) Fields.LastModified, "dd/MM/yyyy HH:mm:ss");
			Type[(int) Fields.LastModified].CanEdit = false;
			Type[(int) Fields.Number] = new NumberEntityField(Type, (int) Fields.Number);
			//Type[(int) Fields.Number].MustExist();
			Type.NameField = Type[(int) Fields.Name];
			Type.DateLastModified = Type[(int) Fields.LastModified];
		}

		public Championship(Entity entity)
			: base(entity)
		{
		}

		public Championship(EntityEdit entityEdit)
			: base(entityEdit)
		{
		}

		public Championship(int championshipId)
			: base(Type, championshipId)
		{
		}

		public new string Name
		{
			get { return base.Name; }
			set { SetValue((int) Fields.Name, value); }
		}

		public Season Season
		{
			get
			{
				Entity entity = GetEntityValue((int) Fields.Season);
				if (entity == null)
					return null;
				return new Season(entity);
			}
			set 
			{ 
				SetValue((int) Fields.Season, value.Entity); 
			}
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

		public Sport Sport
		{
			get
			{
				Entity entity = GetEntityValue((int) Fields.Sport);
				if (entity == null)
					return null;
				return new Sport(entity);
			}
			set 
			{ 
				SetValue((int) Fields.Sport, value.Entity); 
			}
		}

		public bool IsClubs
		{
			get { return (int) GetValue((int) Fields.IsClubs) != 0; }
			set { SetValue((int) Fields.IsClubs, value ? 1 : 0); }
		}

		public bool IsOpen
		{
			get { return (int) GetValue((int) Fields.IsOpen) != 0; }
			set { SetValue((int) Fields.IsOpen, value ? 1 : 0); }
		}

		public ChampionshipType Status
		{
			get { return (ChampionshipType) GetValue((int) Fields.Status); }
			set { SetValue((int) Fields.Status, (int) value); }
		}

		public DateTime LastRegistrationDate
		{
			get
			{ 
				object o = GetValue((int) Fields.LastRegistrationDate);
				if (o == null)
					return DateTime.MinValue;
				else
					return (DateTime) o;
			}
			set { SetValue((int) Fields.LastRegistrationDate, value); }
		}

		public DateTime StartDate
		{
			get 
			{ 
				object o = GetValue((int) Fields.StartDate);
				if (o == null)
					return DateTime.MinValue;
				else
					return (DateTime) o;
			}
			set { SetValue((int) Fields.StartDate, value); }
		}

		public DateTime EndDate
		{
			get 
			{ 
				object o = GetValue((int) Fields.EndDate);
				if (o == null)
					return DateTime.MinValue;
				else
					return (DateTime) o;
			}
			set { SetValue((int) Fields.EndDate, value); }
		}

		public DateTime AltStartDate
		{
			get 
			{ 
				object o = GetValue((int) Fields.AltStartDate);
				if (o == null)
					return DateTime.MinValue;
				else
					return (DateTime) o;
			}
			set { SetValue((int) Fields.AltStartDate, value); }
		}

		public DateTime AltEndDate
		{
			get 
			{ 
				object o = GetValue((int) Fields.AltEndDate);
				if (o == null)
					return DateTime.MinValue;
				else
					return (DateTime) o;
			}
			set { SetValue((int) Fields.AltEndDate, value); }
		}

		public DateTime FinalsDate
		{
			get 
			{ 
				object o = GetValue((int) Fields.FinalsDate);
				if (o == null)
					return DateTime.MinValue;
				else
					return (DateTime) o;
			}
			set { SetValue((int) Fields.FinalsDate, value); }
		}
		
		public DateTime AltFinalsDate
		{
			get 
			{ 
				object o = GetValue((int) Fields.AltFinalsDate);
				if (o == null)
					return DateTime.MinValue;
				else
					return (DateTime) o;
			}
			set { SetValue((int) Fields.AltFinalsDate, value); }
		}
		
		public Ruleset Ruleset
		{
			get
			{
				Entity entity = GetEntityValue((int) Fields.Ruleset);
				if (entity == null)
					return null;
				return new Ruleset(entity);
			}
			set 
			{ 
				SetValue((int) Fields.Ruleset, value.Entity); 
			}
		}

		public User Supervisor
		{
			get
			{
				Entity entity = GetEntityValue((int) Fields.Supervisor);
				if (entity == null)
					return null;
				return new User(entity);
			}
			set 
			{ 
				SetValue((int) Fields.Supervisor, value.Entity); 
			}
		}

		public StandardChampionship StandardChampionship
		{
			get
			{
				Entity entity = GetEntityValue((int) Fields.StandardChampionship);
				if (entity == null)
					return null;
				return new StandardChampionship(entity);
			}
			set 
			{ 
				SetValue((int) Fields.StandardChampionship, value.Entity); 
			}
		}

		public int Number
		{
			get 
			{ 
				object o = GetValue((int) Fields.Number);
				if ((o == null)||(o.ToString().Length == 0))
					return 0;
				else
					return (int) o;
			}
			set { SetValue((int) Fields.Number, value); }
		}
		
		public string Remarks
		{
			get 
			{ 
				object o = GetValue((int) Fields.Remarks);
				if ((o == null)||(o.ToString().Length == 0))
					return "";
				else
					return o.ToString();
			}
			set { SetValue((int) Fields.Remarks, value); }
		}
		
		public double RegisterPrice
		{
			get
			{
				//decide the basic price from products table according to type of sport
				//for match type sport - team registration price
				//for competition type sport - player registration price
				int productID=0;
				switch (this.Sport.SportType)
				{
					case SportType.Match:
						productID = 1;
						break;
					case SportType.Competition:
						productID = 2;
						break;
				}
				if (productID > 0)
				{
					Product product=new Product(productID);
					return product.Price;
				}
				return 0;
			}
		}

		public ChampionshipRegion[] GetRegions()
		{
			Entity[] entities = ChampionshipRegion.Type.GetEntities(
				new EntityFilter((int) ChampionshipRegion.Fields.Championship, Id));

			ChampionshipRegion[] regions = new ChampionshipRegion[entities.Length];
			for (int n = 0; n < entities.Length; n++)
			{
				regions[n] = new ChampionshipRegion(entities[n]);
			}

			return regions;
		}

		public ChampionshipCategory[] GetCategories()
		{
			Entity[] entities = ChampionshipCategory.Type.GetEntities(
				new EntityFilter((int) ChampionshipCategory.Fields.Championship, Id));

			ChampionshipCategory[] categories = new ChampionshipCategory[entities.Length];
			for (int n = 0; n < entities.Length; n++)
			{
				categories[n] = new ChampionshipCategory(entities[n]);
			}
			
			//sort by index:
			ArrayList arrCategories=new ArrayList(categories);
			arrCategories.Sort(new ChampionshipCategoryIndexComparer());
			
			//done.
			return (ChampionshipCategory[])
				arrCategories.ToArray(typeof(ChampionshipCategory));
		}

		public Facility[] GetFacilities()
		{
			EntityFilter facilityFilter = null;
			ChampionshipRegion[] championshipRegions = GetRegions();
			if (championshipRegions.Length > 0)
			{
				int[] regions = new int[championshipRegions.Length];
				for (int n = 0; n < regions.Length; n++)
					regions[n] = championshipRegions[n].Region.Id;
				facilityFilter = new EntityFilter((int) Facility.Fields.Region, regions);
			}

			Entity[] entities = Facility.Type.GetEntities(facilityFilter);
			Sport sport = Sport;
			System.Collections.ArrayList arrFacilities=new System.Collections.ArrayList();

			//iterate over the facilities, add only those having at least one court
			for (int n = 0; n < entities.Length; n++)
			{
				//initialize current facility:
				Facility facility = new Facility(entities[n]);
				
				//add only if has items:
				/* if (facility.GetCourts(sport).Length > 0) */
					arrFacilities.Add(facility);
			}

			return (Facility[]) arrFacilities.ToArray(typeof(Facility));
		}

		public static EntityFilterField SeasonFilter(int season)
		{
			EntityFilterField result=new EntityFilterField(
				(int) Fields.Season, season);
			return result;
		}

		public static EntityFilterField CurrentSeasonFilter()
		{
			int season = Core.Session.Season;
			if (season < 0)
			{
				season = Season.GetLatestSeason();
				Core.Session.Season = season;
			}
			return SeasonFilter(Core.Session.Season);
		}

		public static EntityFilterField LatestSeasonFilter()
		{
			return SeasonFilter(Season.GetLatestSeason());
		}

		public static EntityFilterField OpenSeasonFilter()
		{
			return SeasonFilter(Season.GetOpenSeason());
		}

		private class ChampionshipCategoryIndexComparer : IComparer
		{
			public int Compare(object x, object y)
			{
				ChampionshipCategory cc1=(ChampionshipCategory) x;
				ChampionshipCategory cc2=(ChampionshipCategory) y;
				int index1=cc1.Index;
				int index2=cc2.Index;
				return index1.CompareTo(index2);
			}
		}
	}

	public class ChampionshipRegionFilter : EntityFilterField
	{
		public ChampionshipRegionFilter(int region)
			: base(-1, region)
		{
		}

		public override bool CompareValue(Entity e)
		{
			Championship championship = null;
			if (e.EntityType == Championship.Type)
			{
				championship = new Championship(e);
			}
			else if (e.EntityType == ChampionshipCategory.Type)
			{
				ChampionshipCategory category = new ChampionshipCategory(e);
				championship = category.Championship;
			}

			if (championship != null)
			{
				int region = (int) _value;
				ChampionshipRegion[] regions = championship.GetRegions();

				foreach (ChampionshipRegion championshipRegion in regions)
				{
					if (championshipRegion.Region.Id == region)
						return true;
				}
			}

			return false;
		}
	}
	
	public class ChampionshipSportFilter : EntityFilterField
	{
		public ChampionshipSportFilter(int sport)
			: base(-1, sport)
		{
		}

		public override bool CompareValue(Entity e)
		{
			Championship championship = null;
			if (e.EntityType == Championship.Type)
			{
				championship = new Championship(e);
			}
			else if (e.EntityType == ChampionshipCategory.Type)
			{
				ChampionshipCategory category = new ChampionshipCategory(e);
				championship = category.Championship;
			}
			
			if (championship != null)
			{
				int sport = (int) _value;
				if (championship.Sport != null)
					return championship.Sport.Id == sport;
			}
			
			return false;
		}
	}
}
