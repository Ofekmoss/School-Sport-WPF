using System;
using Sport.Data;
using Sport.Types;
using Rulesets = Sport.Rulesets;
using Sport.Core;

namespace Sport.Entities
{
	public class ChampionshipCategory : EntityBase
	{
		public static readonly string TypeName = "championshipcategory";
		public static readonly EntityType Type;
		
		public enum Fields
		{
			Championship = 0,
			Id,
			Category,
			Status,
			RegistrationPrice,
			MaxStudentBirthday,
			Index,
			LastModified,
			FieldCount
		}

		static ChampionshipCategory()
		{
			Type = new EntityType(TypeName, (int) Fields.FieldCount, (int) Fields.FieldCount, new FieldEntityId((int) Fields.Id), Session.GetSeasonCache());

			Type[(int) Fields.Championship] = new EntityEntityField(Type, (int) Fields.Championship, Championship.TypeName);
			Type[(int) Fields.Championship].MustExist = true;
			Type[(int) Fields.Id] = new NumberEntityField(Type, (int) Fields.Id);
			Type[(int) Fields.Id].CanEdit = false;
			Type[(int) Fields.Category] = new LookupEntityField(Type, (int) Fields.Category, new CategoryTypeLookup());
			Type[(int) Fields.Category].MustExist = true;
			Type[(int) Fields.Status] = new LookupEntityField(Type, (int) Fields.Status, new BooleanTypeLookup("בביצוע", "בתכנון"));
			Type[(int) Fields.RegistrationPrice] = new NumberEntityField(Type, (int) Fields.RegistrationPrice, 0, 9999999, 7, 2);
			Type[(int) Fields.RegistrationPrice].MustExist = true;
			Type[(int) Fields.MaxStudentBirthday] = new DateTimeEntityField(Type, (int) Fields.MaxStudentBirthday, "dd/MM/yyyy");
			Type[(int) Fields.Index] = new NumberEntityField(Type, (int) Fields.Index);
			Type[(int) Fields.Index].MustExist = false;
			Type[(int) Fields.Index].CanEdit = true;
			Type[(int) Fields.LastModified] = new DateTimeEntityField(Type, (int) Fields.LastModified, "dd/MM/yyyy HH:mm:ss");
			Type[(int) Fields.LastModified].CanEdit = false;

			Type.NameField = Type[(int) Fields.Category];
			Type.DateLastModified = Type[(int) Fields.LastModified];
			Type.OwnerField = Type[(int) Fields.Championship];

			Type.NewEntity = new EntityType.EntityNewEntity(NewEntity);
		}

		private static void NewEntity(EntityEdit entityEdit)
		{
			Type[(int) Fields.Status].SetValue(entityEdit, 0);
		}


		public ChampionshipCategory(Entity entity)
			: base(entity)
		{
		}

		public ChampionshipCategory(EntityEdit entityEdit)
			: base(entityEdit)
		{
		}

		public ChampionshipCategory(int championshipCategoryId)
			: base(Type, championshipCategoryId)
		{
		}

		public Championship Championship
		{
			get
			{
				Entity entity = null;
				//try
				//{
					entity = GetEntityValue((int) Fields.Championship);
				//}
				//catch
				//{
				//	entity = null;
				//}
				//if (entity == null)
				//	return null;
				return new Championship(entity);
			}
			set 
			{ 
				SetValue((int) Fields.Championship, value.Entity); 
			}
		}
		
		/*
		public new string Name
		{
			get
			{
				string name = "";
				if ((ChampionshipCategory.ShowFullName)&&(this.Championship != null))
					name = this.Championship.Name+" ";
				name += _categoryLookup.Lookup(this.Category);
				return name;
			}
		}
		*/
		
		public int Category
		{
			get { return (int) GetValue((int) Fields.Category); }
			set { SetValue((int) Fields.Category, value); }
		}

		public int Status
		{
			get { return (int) GetValue((int) Fields.Status); }
			set { SetValue((int) Fields.Status, value); }
		}

		public double RegistrationPrice
		{
			get
			{
				object value=GetValue((int) Fields.RegistrationPrice);
				return Common.Tools.CDblDef(value, 0);
			}
			set { SetValue((int) Fields.RegistrationPrice, value); }
		}
		
		public DateTime MaxStudentBirthday
		{
			get	
			{
				return Common.Tools.CDateTimeDef(
					  GetValue((int) Fields.MaxStudentBirthday), DateTime.MinValue);
			}
			set { SetValue((int) Fields.MaxStudentBirthday, value); }
		}
		
		public int Index
		{
			get { return Common.Tools.CIntDef(GetValue((int) Fields.Index), 0); }
			set { SetValue((int) Fields.Index, value); }
		}
		
		public override string CanDelete()
		{
			string result="";
			
			//begin checkings... first check for teams in this category:
			Team[] teams=this.GetTeams();
			if ((teams != null)&&(teams.Length > 0))
			{
				string strTeams="";
				foreach (Team team in teams)
				{
					if (team.Id >= 0)
						strTeams += team.Name+"\n";
				}
				if (strTeams.Length > 0)
				{
					result += "הקבוצות הבאות רשומות בקטגורית אליפות זו: \n";
					result += strTeams+"יש להסיר קבוצות אלו כדי למחוק את האליפות\n";
				}
			}
			
			return result;
		}
		
		public Team[] GetTeams()
		{
			Entity[] entities = Team.Type.GetEntities(
				new EntityFilter((int) Team.Fields.Category, Id));

			if (entities == null)
				return null;

			Team[] teams = new Team[entities.Length];
			for (int t = 0; t < entities.Length; t++)
			{
				teams[t] = new Team(entities[t]);
				/*
				if (!teams[t].IsValid())
				{
					System.Diagnostics.Debug.WriteLine("failed to create team for championship category "+this.Id);
					teams[t] = null;
				}
				*/
			}
			//teams = (Team[]) Common.Tools.RemoveNullValues(teams, typeof(Team));

			return teams;
		}

		public int GetMaximumIndex(int schoolId)
		{
			Team[] teams = this.GetTeams();
			int maxIndex = -1;
			foreach (Team team in teams)
			{
				if (team.School.Id == schoolId)
				{
					if (team.Index > maxIndex)
					{
						maxIndex = team.Index;
					}
				}
			}
			return maxIndex;
		}

		public Rulesets.Ruleset GetRuleset()
		{
			Ruleset rs = Championship.Ruleset;

			if (rs == null)
			{
				rs = Championship.Sport.Ruleset;

				if (rs == null)
					return null;
			}

			return Rulesets.Ruleset.LoadRuleset(rs.Id);
		}

		public object GetRule(System.Type type)
		{
			Rulesets.Ruleset ruleset = GetRuleset();

			if (ruleset == null)
				return null;

			return ruleset.GetRule(new Rulesets.RuleScope(Category), type);
		}

		public object GetRule(System.Type type, SportField sportField)
		{
			Rulesets.Ruleset ruleset = GetRuleset();

			if (ruleset == null)
				return null;

			return ruleset.GetRule(new Rulesets.RuleScope(Category, sportField.SportFieldType.Id,
				sportField.Id), type);
		}

		public object GetRule(System.Type type, SportFieldType sportFieldType)
		{
			Ruleset rs = Championship.Ruleset;

			if (rs == null)
				return null;

			Rulesets.Ruleset ruleset = Rulesets.Ruleset.LoadRuleset(rs.Id);

			if (ruleset == null)
				return null;

			return ruleset.GetRule(new Rulesets.RuleScope(Category, sportFieldType.Id, -1), type);
		}

		public Equipment[] GetEquipments()
		{
			Entity[] entities = Equipment.Type.GetEntities(
				new EntityFilter((int) Equipment.Fields.Category, Id));
			
			if (entities == null)
				return null;

			Equipment[] result = new Equipment[entities.Length];

			for (int t = 0; t < entities.Length; t++)
			{
				result[t] = new Equipment(entities[t]);
			}
			//result = (Equipment[]) Common.Tools.RemoveNullValues(result, typeof(Equipment));
			
			return result;
		}
		
		public int GameRate
		{
			get
			{
				string strRate = Core.Configuration.ReadString("GameRates", "champ"+this.Id);
				if ((strRate == null)||(strRate.Length == 0))
					return 0;
				if (!Common.Tools.IsInteger(strRate))
					return 0;
				return Int32.Parse(strRate);
			}
			set
			{
				Core.Configuration.WriteString("GameRates", "champ"+this.Id, 
					value.ToString());
			}
		}
	}
}
