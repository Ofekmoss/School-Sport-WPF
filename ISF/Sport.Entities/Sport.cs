using System;
using Sport.Data;
using Sport.Types;

namespace Sport.Entities
{
	public class Sport : EntityBase
	{
		public static readonly string TypeName = "sport";
		public static readonly EntityType Type;

		public enum Fields
		{
			Id = 0,
			Name,
			SportType,
			Ruleset,
			PointsName,
			CentralRegionOnly,
			LastModified,
			FieldCount
		}

		static Sport()
		{
			Type = new EntityType(TypeName, (int)Fields.FieldCount, (int)Fields.FieldCount, new FieldEntityId((int)Fields.Id));

			Type[(int)Fields.Id] = new NumberEntityField(Type, (int)Fields.Id);
			Type[(int)Fields.Id].CanEdit = false;
			Type[(int)Fields.Name].MustExist = true;
			Type[(int)Fields.SportType] = new LookupEntityField(Type, (int)Fields.SportType, new SportTypeLookup());
			Type[(int)Fields.SportType].MustExist = true;
			Type[(int)Fields.Ruleset] = new EntityEntityField(Type, (int)Fields.Ruleset, Ruleset.TypeName);
			Type[(int)Fields.CentralRegionOnly] = new LookupEntityField(Type, (int)Fields.CentralRegionOnly, new BooleanTypeLookup());
			Type[(int)Fields.LastModified] = new DateTimeEntityField(Type, (int)Fields.LastModified, "dd/MM/yyyy HH:mm:ss");
			Type[(int)Fields.LastModified].CanEdit = false;

			Type.NameField = Type[(int)Fields.Name];
			Type.DateLastModified = Type[(int)Fields.LastModified];
		}

		public Sport(Entity entity)
			: base(entity)
		{
		}

		public Sport(EntityEdit entityEdit)
			: base(entityEdit)
		{
		}

		public Sport(int sportId)
			: base(Type, sportId)
		{
		}

		public new string Name
		{
			get { return base.Name; }
			set { SetValue((int)Fields.Name, value); }
		}

		public SportType SportType
		{
			get { return (SportType)GetValue((int)Fields.SportType); }
			set { SetValue((int)Fields.SportType, (int)value); }
		}

		public string PointsName
		{
			get
			{
				object value = GetValue((int)Fields.PointsName);
				return (value == null) ? "" : value.ToString();
			}
			set { SetValue((int)Fields.SportType, value); }
		}

		public int CentralRegionOnly
		{
			get { return Common.Tools.CIntDef(GetValue((int)Fields.CentralRegionOnly), 0); }
			set { SetValue((int)Fields.CentralRegionOnly, value); }
		}

		public Ruleset Ruleset
		{
			get
			{
				/*
				EntityFilter filter=new EntityFilter(
					(int) Entities.Ruleset.Fields.Sport, this.Id);
				Entity[] rulesets=Entities.Ruleset.Type.GetEntities(filter);
				if (rulesets.Length > 0)
				{
					return new Ruleset(rulesets[0]);
				}
				else
				{
					return null;
				}
				*/
				try
				{
					Entity entity = GetEntityValue((int)Fields.Ruleset);
					return (entity == null) ? null : new Ruleset(entity.Id);
				}
				catch (Exception ex)
				{
					System.Diagnostics.Debug.WriteLine("error getting ruleset: " + ex.Message);
					return null;
				}
			}
			set
			{
				SetValue((int)Fields.Ruleset, value.Entity);
			}
		}

		public SportFieldType[] GetSportFieldTypes()
		{
			Entity[] entities = SportFieldType.Type.GetEntities(
				new EntityFilter((int)SportFieldType.Fields.Sport, Id));

			SportFieldType[] sportFieldTypes = new SportFieldType[entities.Length];
			for (int n = 0; n < entities.Length; n++)
			{
				sportFieldTypes[n] = new SportFieldType(entities[n]);
			}

			return sportFieldTypes;
		}

		public Championship[] GetChampionships()
		{
			EntityFilter filter = new EntityFilter((int)Championship.Fields.Sport, Id);
			filter.Add(Championship.CurrentSeasonFilter());
			Entity[] entities = Championship.Type.GetEntities(filter);

			Championship[] championships = new Championship[entities.Length];
			for (int n = 0; n < entities.Length; n++)
			{
				championships[n] = new Championship(entities[n]);
			}

			return championships;
		}

		public CourtType[] GetCourtTypes()
		{
			Entity[] entities = CourtTypeSport.Type.GetEntities(
				new EntityFilter((int)CourtTypeSport.Fields.Sport, Id));

			EntityField courtTypeField = CourtTypeSport.Type.Fields[(int)CourtTypeSport.Fields.CourtType];

			CourtType[] courtTypes = new CourtType[entities.Length];
			for (int n = 0; n < entities.Length; n++)
			{
				courtTypes[n] = new CourtType((int)courtTypeField.GetValue(entities[n]));
			}

			return courtTypes;
		}

		public Ruleset[] GetRulesets()
		{
			System.Collections.ArrayList al = new System.Collections.ArrayList();

			Entity[] rulesets = Ruleset.Type.GetEntities(
				new EntityFilter((int)Ruleset.Fields.Sport, Id));

			Ruleset sportRuleset = Ruleset;

			foreach (Entity ruleset in rulesets)
			{
				if (!ruleset.Equals(sportRuleset))
				{
					al.Add(new Ruleset(ruleset));
				}
			}

			return (Ruleset[])al.ToArray(typeof(Ruleset));
		}

		public PracticeCamp[] GetPracticeCamps()
		{
			EntityFilter filter = new EntityFilter((int)PracticeCamp.Fields.Sport, this.Id);
			Entity[] practiceCamps = PracticeCamp.Type.GetEntities(filter);
			PracticeCamp[] result = new PracticeCamp[practiceCamps.Length];
			for (int n = 0; n < practiceCamps.Length; n++)
				result[n] = new PracticeCamp(practiceCamps[n]);
			return result;
		}
	}
}
