using System;
using Sport.Data;
using Sport.Types;

namespace Sport.Entities
{
	public class StandardChampionship : EntityBase
	{
		public static readonly string TypeName = "standardchampionship";
		public static readonly EntityType Type;
        
		public enum Fields
		{
			Id = 0,
			Name,
			Sport,
			IsRegional,
			IsOpen,
			IsClubs,
			Ruleset,
			LastModified,
			FieldCount
		}

		public static EntityField SportField
		{
			get { return Type.Fields[(int) Fields.Sport]; }
		}
		public static EntityField IsRegionalField
		{
			get { return Type.Fields[(int) Fields.IsRegional]; }
		}

		static StandardChampionship()
		{
			Type = new EntityType(TypeName, (int) Fields.FieldCount, (int) Fields.FieldCount, new FieldEntityId((int) Fields.Id));

			Type[(int) Fields.Id] = new NumberEntityField(Type, (int) Fields.Id);
			Type[(int) Fields.Id].CanEdit = false;
			Type[(int) Fields.Name].MustExist = true;
			Type[(int) Fields.Sport] = new EntityEntityField(Type, (int) Fields.Sport, Sport.TypeName);
			Type[(int) Fields.Sport].MustExist = true;
			Type[(int) Fields.IsRegional] = new LookupEntityField(Type, (int) Fields.IsRegional, new BooleanTypeLookup("מחוזית", "ארצית"));
			Type[(int) Fields.IsRegional].MustExist = true;
			Type[(int) Fields.IsOpen] = new LookupEntityField(Type, (int) Fields.IsOpen, new BooleanTypeLookup());
			Type[(int) Fields.IsOpen].MustExist = true;
			Type[(int) Fields.IsClubs] = new LookupEntityField(Type, (int) Fields.IsClubs, new BooleanTypeLookup());
			Type[(int) Fields.IsClubs].MustExist = true;
			Type[(int) Fields.Ruleset] = new EntityEntityField(Type, (int) Fields.Ruleset, Ruleset.TypeName);
			Type[(int) Fields.LastModified] = new DateTimeEntityField(Type, (int) Fields.LastModified, "dd/MM/yyyy HH:mm:ss");
			Type[(int) Fields.LastModified].CanEdit = false;
			
			Type.NameField = Type[(int) Fields.Name];
			Type.DateLastModified = Type[(int) Fields.LastModified];
		}

		public StandardChampionship(Entity entity)
			: base(entity)
		{
		}

		public StandardChampionship(EntityEdit entityEdit)
			: base(entityEdit)
		{
		}

		public StandardChampionship(int standardChampionshipId)
			: base(Type, standardChampionshipId)
		{
		}

		public new string Name
		{
			get { return base.Name; }
			set { SetValue((int) Fields.Name, value); }
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

		public bool IsRegional
		{
			get 
			{ 
				object b = GetValue((int) Fields.IsRegional);
				return b == null ? false : (int) b != 0; 
			}
			set { SetValue((int) Fields.IsRegional, value ? 1 : 0); }
		}
		
		public bool IsOpen
		{
			get { return (int) GetValue((int) Fields.IsOpen) != 0; }
			set { SetValue((int) Fields.IsOpen, value ? 1 : 0); }
		}

		public bool IsClubs
		{
			get { return (int) GetValue((int) Fields.IsClubs) != 0; }
			set { SetValue((int) Fields.IsClubs, value ? 1 : 0); }
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

		public StandardChampionshipCategory[] GetCategories()
		{
			Entity[] entities = StandardChampionshipCategory.Type.GetEntities(
				new EntityFilter((int) StandardChampionshipCategory.Fields.Championship, Id));

			if (entities == null)
				return null;

			StandardChampionshipCategory[] categories = new StandardChampionshipCategory[entities.Length];

			for (int n = 0; n < entities.Length; n++)
			{
				categories[n] = new StandardChampionshipCategory(entities[n]);
			}

			return categories;

		}
	}
}
