using System;
using Sport.Data;

namespace Sport.Entities
{
	public class Ruleset : EntityBase
	{
		public static readonly string TypeName = "ruleset";
		public static readonly EntityType Type;

		public enum Fields
		{
			Id = 0,
			Name,
			Sport,
			Region,
			LastModified,
			FieldCount
		}
		static Ruleset()
		{
			Type = new EntityType(TypeName, (int) Fields.FieldCount, (int) Fields.FieldCount, new FieldEntityId((int) Fields.Id));

			Type[(int) Fields.Id] = new NumberEntityField(Type, (int) Fields.Id);
			Type[(int) Fields.Id].CanEdit = false;
			Type[(int) Fields.Name].MustExist = true;
			Type[(int) Fields.Sport] = new EntityEntityField(Type, (int) Fields.Sport, Sport.TypeName);
			Type[(int) Fields.Sport].MustExist = true;
			Type[(int) Fields.Region] = new EntityEntityField(Type, (int) Fields.Sport, Region.TypeName);
			Type[(int) Fields.LastModified] = new DateTimeEntityField(Type, (int) Fields.LastModified, "dd/MM/yyyy HH:mm:ss");
			Type[(int) Fields.LastModified].CanEdit = false;

			Type.NameField = Type[(int) Fields.Name];
			Type.DateLastModified = Type[(int) Fields.LastModified];
		}

		public Ruleset(Entity entity)
			: base(entity)
		{
		}

		public Ruleset(EntityEdit entityEdit)
			: base(entityEdit)
		{
		}

		public Ruleset(int rulesetId)
			: base(Type, rulesetId)
		{
		}

		public new string Name
		{
			get
			{
				object value=GetValue((int) Fields.Name);
				return (value == null)?"":value.ToString();
			}
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

		public Championship[] GetChapmionships()
		{
			EntityFilter filter=new EntityFilter((int) Championship.Fields.Ruleset, Id);
			filter.Add(Championship.CurrentSeasonFilter());
			Entity[] entities = Championship.Type.GetEntities(filter);
			
			Championship[] champs = new Championship[entities.Length];
			for (int n = 0; n < entities.Length; n++)
			{
				champs[n] = new Championship(entities[n]);
			}

			return champs;
		}
	}
}
