using System;
using Sport.Data;
using Sport.Common;
using Sport.Types;
using Sport.Core;
using System.Collections;
using System.Collections.Generic;

namespace Sport.Entities
{
	public class TeamPlayersEntityField : EntityField
	{
		public TeamPlayersEntityField()
			: base(Team.Type, (int)Team.Fields.TeamPlayerCount)
		{
		}

		public override bool CanEdit
		{
			get { return false; }
		}

		public override bool MustExist
		{
			get { return false; }
		}

		// Overrides GetText to return the GetText value
		// of the relative field
		public override string GetText(Entity e)
		{
			return Team.GetTeamPlayers(e.Id).Length.ToString();
		}

		public override int Compare(Entity x, Entity y)
		{
			int c1 = Team.GetTeamPlayers(x.Id).Length;
			int c2 = Team.GetTeamPlayers(y.Id).Length;
			return c1.CompareTo(c2);
		}

		// Overrides SetValue to disable change to the relative
		// field
		public override void SetValue(EntityEdit e, object value)
		{
			throw new EntityException("Cannot edit a relative field");
		}
	}

	public class Team : EntityBase
	{
		public static readonly string TypeName = "team";
		public static readonly EntityType Type;

		public enum Fields
		{
			Id = 0,
			School,
			Category,
			Championship,
			Status,
			Index,
			Supervisor,
			PlayerNumberFrom,
			PlayerNumberTo,
			RegisterDate,
			LastModified,
			Charge,
			PlayersCount,
			DataFields,
			Name = DataFields,
			Sport,
			SchoolSymbol,
			TeamPlayerCount, 
			CityId, 
			FieldCount
		}

		static Team()
		{
			Type = new EntityType(TypeName, (int)Fields.FieldCount, (int)Fields.DataFields, new FieldEntityId((int)Fields.Id), Session.GetSeasonCache());

			Type[(int)Fields.Id] = new NumberEntityField(Type, (int)Fields.Id);
			Type[(int)Fields.Id].CanEdit = false;
			Type[(int)Fields.School] = new EntityEntityField(Type, (int)Fields.School, School.TypeName);
			Type[(int)Fields.School].MustExist = true;
			Type[(int)Fields.School].CanEdit = false;
			Type[(int)Fields.Championship] = new EntityEntityField(Type, (int)Fields.Championship, Championship.TypeName);
			Type[(int)Fields.Championship].MustExist = true;
			Type[(int)Fields.Category] = new EntityEntityField(Type, (int)Fields.Category, ChampionshipCategory.TypeName);
			Type[(int)Fields.Category].MustExist = true;
			Type[(int)Fields.Status] = new LookupEntityField(Type, (int)Fields.Status, new TeamStatusLookup());
			Type[(int)Fields.Status].MustExist = true;
			Type[(int)Fields.Index] = new LetterNumberEntityField(Type, (int)Fields.Index, 0, 99);
			Type[(int)Fields.Supervisor] = new EntityEntityField(Type, (int)Fields.Supervisor, User.TypeName);
			Type[(int)Fields.PlayerNumberFrom] = new NumberEntityField(Type, (int)Fields.PlayerNumberFrom, 0, 99999);
			Type[(int)Fields.PlayerNumberTo] = new NumberEntityField(Type, (int)Fields.PlayerNumberTo, 0, 99999);
			Type[(int)Fields.RegisterDate] = new DateTimeEntityField(Type, (int)Fields.RegisterDate, "dd/MM/yyyy HH:mm");
			Type[(int)Fields.RegisterDate].CanEdit = false;
			Type[(int)Fields.LastModified] = new DateTimeEntityField(Type, (int)Fields.LastModified, "dd/MM/yyyy HH:mm:ss");
			Type[(int)Fields.LastModified].CanEdit = false;
			Type[(int)Fields.Charge] = new EntityEntityField(Type, (int)Fields.Charge, Charge.TypeName);
			Type[(int)Fields.Charge].CanEdit = false;
			Type[(int)Fields.PlayersCount] = new NumberEntityField(Type, (int)Fields.PlayersCount, 0, 999);
			Type[(int)Fields.PlayersCount].CanEdit = false;
			Type[(int)Fields.PlayersCount].MustExist = false;

			Type[(int)Fields.Name] = new FormatEntityField(Type,
				"{0} {1}", new int[] { (int)Fields.School, (int)Fields.Index }, (int)Fields.Index);
			Type.DateLastModified = Type[(int)Fields.LastModified];

			// Relative fields
			Type[(int)Fields.Sport] = new EntityRelationEntityField(Type,
				(int)Fields.Sport,
				(int)Fields.Championship,
				Championship.TypeName, (int)Championship.Fields.Sport);
			Type[(int)Fields.SchoolSymbol] = new EntityRelationEntityField(Type,
				(int)Fields.SchoolSymbol,
				(int)Fields.School,
				School.TypeName, (int)School.Fields.Symbol);
			Type[(int)Fields.TeamPlayerCount] = new TeamPlayersEntityField();
			Type[(int)Fields.CityId] = new EntityRelationEntityField(Type,
				(int)Fields.CityId,
				(int)Fields.School,
				School.TypeName, (int)School.Fields.City);

			Type.NameField = Type[(int)Fields.Name];

			Type.NewEntity = new EntityType.EntityNewEntity(NewEntity);
			Type.EntityCheck += new EntityType.EntityEntityCheck(EntityCheck);
			Type.ValueChanged = new EntityType.EntityValueChange(ValueChanged);
		}

		private static void NewEntity(EntityEdit entityEdit)
		{
			Type[(int)Fields.RegisterDate].SetValue(entityEdit, DateTime.Now);
		}

		private static EntityResult EntityCheck(Entity entity)
		{
			Team team = new Team(entity);

			int pNumFrom = team.PlayerNumberFrom;
			int pNumTo = team.PlayerNumberTo;
			if (pNumFrom > 0)
			{
				if (pNumFrom > pNumTo)
					return new EntityMessageResult(EntityResultCode.ValueOutOfRange, "'עד מספר חולצה' חייב להיות גבוה מ'ממספר חולצה'");

				if (team.Championship.Sport.SportType == SportType.Competition)
				{
					Team[] otherTeams = team.Category.GetTeams();
					if (otherTeams != null)
					{
						foreach (Team otherTeam in otherTeams)
						{
							if (!team.Equals(otherTeam))
							{
								if (team.PlayerNumberFrom <= otherTeam.PlayerNumberTo &&
									team.PlayerNumberTo >= otherTeam.PlayerNumberFrom)
									return new EntityMessageResult(EntityResultCode.UniqueValueFailure, "מספרי חולצה בקבוצה מתנגשים עם הקבוצה '" + otherTeam.TeamName() + "'");
							}
						}
					}

					Player[] players = team.GetPlayers();
					if ((players != null) && (players.Length > 0))
					{
						if (players.Length > (pNumTo - pNumFrom + 1))
							return new EntityMessageResult(EntityResultCode.ValueOutOfRange, "לקבוצה זו כבר רשומים " + players.Length + " שחקנים, טווח חדש קטן מידי.");
					}
				}
			}

			return EntityResult.Ok;
		}

		private static void ValueChanged(EntityEdit entityEdit, int field)
		{
			if (field == (int)Fields.PlayerNumberFrom)
			{
				object o = Type[(int)Fields.PlayerNumberFrom].GetValue(entityEdit);
				if (o == null)
					Type[(int)Fields.PlayerNumberTo].SetValue(entityEdit, null);
				else
					Type[(int)Fields.PlayerNumberTo].SetValue(entityEdit, (int)o + 19);
			}
		}


		private static object[] FieldValues(Entity entity, int field)
		{
			EntityEntityField entityField = Type[field] as EntityEntityField;

			if (entityField == null)
				throw new EntityException("Field must be entity field");

			if (field == (int)Fields.School || field == (int)Fields.Championship)
			{
				return entityField.EntityType.GetEntities(null);
			}
			else
			{
				if (field == (int)Fields.Category)
				{
					try
					{
						Team team = new Team(entity);
						return entityField.EntityType.GetEntities(
							new EntityFilter((int)ChampionshipCategory.Fields.Championship, team.Championship.Id));
					}
					catch
					{
						return null;
					}
				}
			}

			return null;
		}

		public Team(Entity entity)
			: base(entity)
		{
		}

		public Team(EntityEdit entityEdit)
			: base(entityEdit)
		{
		}

		public Team(int teamId)
			: base(Type, teamId)
		{
		}

		public int Index
		{
			get
			{
				object value = GetValue((int)Fields.Index);
				return value == null ? 0 : (int)value;
			}
			set
			{
				SetValue((int)Fields.Index, value);
			}
		}

		public int PlayerNumberFrom
		{
			get
			{
				int pNumFrom = Common.Tools.CIntDef(
					GetValue((int)Fields.PlayerNumberFrom), -1);
				if ((pNumFrom < 0) && (this.School != null))
					pNumFrom = this.School.PlayerNumberFrom;
				return pNumFrom;
			}
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
			get
			{
				int pNumTo = Common.Tools.CIntDef(
					GetValue((int)Fields.PlayerNumberTo), -1);
				if ((pNumTo < 0) && (this.School != null))
					pNumTo = this.School.PlayerNumberTo;
				return pNumTo;
			}
			set
			{
				if (value < 0)
					SetValue((int)Fields.PlayerNumberTo, null);
				else
					SetValue((int)Fields.PlayerNumberTo, value);
			}
		}

		public TeamStatusType Status
		{
			get { return (TeamStatusType)GetValue((int)Fields.Status); }
			set { SetValue((int)Fields.Status, (int)value); }
		}

		public DateTime RegisterDate
		{
			get
			{
				object o = GetValue((int)Fields.RegisterDate);
				if (o == null)
					return DateTime.MinValue;
				else
					return (DateTime)o;
			}
			set
			{
				SetValue((int)Fields.RegisterDate, value);
			}
		}

		public School School
		{
			get
			{
				School result = null;
				try
				{
					Entity entity = GetEntityValue((int)Fields.School);
					if (entity != null)
						result = new School(entity);
				}
				catch (Exception e)
				{
					System.Diagnostics.Debug.WriteLine("failed to load team school. team id: " + this.Id.ToString());
					System.Diagnostics.Debug.WriteLine("error: " + e.Message);
					System.Diagnostics.Debug.WriteLine(e.StackTrace);
					//throw e;
				}
				return result;
			}
			set
			{
				SetValue((int)Fields.School, value.Entity);
			}
		}

		public Charge Charge
		{
			get
			{
				Charge result = null;
				try
				{
					Entity entity = GetEntityValue((int)Fields.Charge);
					if (entity != null)
						result = new Charge(entity);
				}
				catch (Exception e)
				{
					System.Diagnostics.Debug.WriteLine("failed to load team charge. team id: " + this.Id.ToString());
					System.Diagnostics.Debug.WriteLine("error: " + e.Message);
					System.Diagnostics.Debug.WriteLine(e.StackTrace);
				}
				return result;
			}
			set
			{
				SetValue((int)Fields.Charge, value.Entity);
			}
		}

		public ChampionshipCategory Category
		{
			get
			{
				Entity entity = null;
				try
				{
					entity = GetEntityValue((int)Fields.Category);
				}
				catch (Exception ex)
				{
					System.Diagnostics.Debug.WriteLine("get team category: failed to get category for team " + this.Id + " (" + ex.Message + ")");
				}
				if ((entity == null) || (entity.Id < 0))
					return null;
				return new ChampionshipCategory(entity);
			}
			set
			{
				SetValue((int)Fields.Category, value.Entity);
			}
		}

		public Championship Championship
		{
			get
			{
				Championship result = null;
				try
				{
					Entity entity = GetEntityValue((int)Fields.Championship);
					if (entity != null)
						result = new Championship(entity);
				}
				catch (Exception e)
				{
					System.Diagnostics.Debug.WriteLine("failed to get team championship: " + e.Message);
					System.Diagnostics.Debug.WriteLine("team id: " + this.Id);
					System.Diagnostics.Debug.WriteLine(e.StackTrace);
				}
				return result;
			}
			set
			{
				SetValue((int)Fields.Championship, value.Entity);
			}
		}

		public User Supervisor
		{
			get
			{
				Entity entity = GetEntityValue((int)Fields.Supervisor);
				return (entity == null || entity.Id < 0) ? null : new User(entity);
			}
			set
			{
				SetValue((int)Fields.Supervisor, value.Entity);
			}
		}

		private static Dictionary<int, List<Player>> tblTeamPlayers = new Dictionary<int, List<Player>>();
		public static Player[] GetTeamPlayers(int teamID)
		{
			if (teamID < 0)
				return null;

			if (!tblTeamPlayers.ContainsKey(teamID))
			{
				tblTeamPlayers.Add(teamID, new List<Player>());
				Team team = new Team(teamID);
				if (team.Category != null)
				{
					Team[] arrChampTeams = team.Category.GetTeams();
					if (arrChampTeams != null)
					{
						int[] teamIds = new int[arrChampTeams.Length];
						for (int i = 0; i < arrChampTeams.Length; i++)
							teamIds[i] = arrChampTeams[i].Id;
						Entity[] arrPlayerEntities = Player.Type.GetEntities(new EntityFilter((int)Player.Fields.Team, teamIds));
						if (arrPlayerEntities != null)
						{
							Dictionary<int, List<Player>> tblChampionshipPlayers = new Dictionary<int, List<Player>>();
							foreach (Entity entity in arrPlayerEntities)
							{
								Player player = new Player(entity);
								int curTeamId = player.Team.Id;
								if (!tblChampionshipPlayers.ContainsKey(curTeamId))
									tblChampionshipPlayers.Add(curTeamId, new List<Player>());
								tblChampionshipPlayers[curTeamId].Add(player);
							}

							foreach (int curChampTeamId in tblChampionshipPlayers.Keys)
							{
								if (tblTeamPlayers.ContainsKey(curChampTeamId))
									tblTeamPlayers[curChampTeamId].Clear();
								else
									tblTeamPlayers.Add(curChampTeamId, new List<Player>());
								tblTeamPlayers[curChampTeamId].AddRange(tblChampionshipPlayers[curChampTeamId]);
							}
						}
					}
				}
			}

			return tblTeamPlayers.ContainsKey(teamID) ? tblTeamPlayers[teamID].ToArray() : new Player[] { };
		}

		public static void ResetTeamPlayers(int teamID)
		{
			tblTeamPlayers.Remove(teamID);
		}

		public Player[] Players
		{
			get
			{
				if (Core.Session.Connected)
				{
					return Team.GetTeamPlayers(this.Id);
				}
				else
				{
					List<Player> result = new List<Player>();
					EntityFilter filter = new EntityFilter((int)Player.Fields.Team, this.Id);
					Entity[] players = Player.Type.GetEntities(filter);
					if (players != null)
					{
						for (int i = 0; i < players.Length; i++)
						{
							Player player = null;
							try
							{
								player = new Player(players[i]);
							}
							catch { }
							if ((player != null) && (player.Id >= 0))
								result.Add(player);
						}
					}
					return result.ToArray();
				}
			}
		}

		/// <summary>
		/// verify current entity is valid.
		/// in case of invalid entity, delete from database
		/// and throw error.
		/// </summary>
		private void VerifyMyself()
		{
			//check school:
			if ((this.School == null) && (this.Id >= 0))
			{
				//delete the team!
				bool result = Type.Delete(this.Entity);
				System.Diagnostics.Debug.WriteLine("deleted team with zombie school.");
				System.Diagnostics.Debug.WriteLine("team id: " + this.Id + " result: " + result);
				throw new Exception("failed to create new team: invalid school for the team.");
			}
		}

		public Player[] GetPlayers()
		{
			return this.Players;
		}

		public Player GetPlayerByNumber(int number)
		{
			//get all players:
			Player[] players = this.GetPlayers();

			//got anything?
			if ((players == null) || (players.Length == 0))
				return null;

			//iterate over players, look for one with given shirt number:
			foreach (Player player in players)
			{
				//found proper player?
				if (player.Number == number)
					return player;
			}

			//did not find.
			return null;
		}

		public string TeamName()
		{
			if ((this.School == null) || (this.School.Id < 0))
				return "[error: team " + this.Id + " does not have school]";
			try
			{
				string schoolName = this.School.Name;
				object value = this.Entity.Fields[(int)Fields.Index];
				int index = Common.Tools.CIntDef(value, 0);
				string result = schoolName;
				if (index > 0)
				{
					result += " " + Common.Tools.ToHebLetter(index);
					if (index.ToString().Length < 2)
						result += "'";
				}
				return result;
			}
			catch (Exception ex)
			{
				return "[error getting team name: " + ex.Message + "]";
			}
		}

		public static Team FromSchoolAndCategory(int schoolID, int champCategoryID)
		{
			Data.EntityFilter filter = new Data.EntityFilter(
				(int)Fields.School, schoolID);
			filter.Add(
				new Data.EntityFilterField((int)Fields.Category, champCategoryID));
			Data.Entity[] arrTeams = Team.Type.GetEntities(filter);
			Team team = null;
			if ((arrTeams != null) && (arrTeams.Length > 0))
			{
				try
				{
					team = new Team(arrTeams[0]);
				}
				catch
				{
					System.Diagnostics.Debug.WriteLine("failed to get team from school and category: " + schoolID + ", " + champCategoryID);
				}
			}
			return team;
		}
	}

	public class TeamCategoryFilter : EntityFilterField
	{
		public TeamCategoryFilter(int category)
			: base(-1, category)
		{
		}

		public override bool CompareValue(Entity e)
		{
			Team team = new Team(e);
			return CategoryTypeLookup.Compare(team.Category.Category, (int)_value, CategoryCompareType.Partial);
		}
	}

	public class OfflineTeam : Data.OfflineEntity
	{
		private int _championshipCategory = -1;
		private int _phase = -1;
		private int _group = -1;
		private School _school = null;
		private OfflineSchool _offlineSchool = null;
		private int _playerNumbrFrom = -1;
		private int _playerNumberTo = -1;
		public int Score = 0;

		protected override string GetOfflineType()
		{
			return "team";
		}

		public int ChampionshipCategory
		{
			get { return _championshipCategory; }
			set { _championshipCategory = value; }
		}

		public int Phase
		{
			get { return _phase; }
			set { _phase = value; }
		}

		public int Group
		{
			get { return _group; }
			set { _group = value; }
		}

		public School School
		{
			get { return _school; }
			set { _school = value; }
		}

		public OfflineSchool OfflineSchool
		{
			get { return _offlineSchool; }
			set { _offlineSchool = value; }
		}

		public int PlayerNumberFrom
		{
			get { return _playerNumbrFrom; }
			set { _playerNumbrFrom = value; }
		}

		public int PlayerNumberTo
		{
			get { return _playerNumberTo; }
			set { _playerNumberTo = value; }
		}

		public OfflineTeam()
			: base()
		{
		}

		public OfflineTeam(int champCategory, int phase, int group, object school,
			int playerNumberFrom, int playerNumberTo)
			: this()
		{
			this.ChampionshipCategory = champCategory;
			this.Phase = phase;
			this.Group = group;
			if (school is Data.Entity)
				this.School = new School(school as Entity);
			else if (school is OfflineSchool)
				this.OfflineSchool = (OfflineSchool)school;
			this.PlayerNumberFrom = playerNumberFrom;
			this.PlayerNumberTo = playerNumberTo;
		}

		protected override OfflineField[] GetFields()
		{
			return new OfflineField[] {
				new OfflineField("champcategory", this.ChampionshipCategory.ToString()), 
				new OfflineField("phase", this.Phase.ToString()), 
				new OfflineField("group", this.Group.ToString()),
				new OfflineField("school", (this.School == null)?"-1":this.School.Id.ToString()), 
				new OfflineField("offlineschool", (this.OfflineSchool == null)?"-1":this.OfflineSchool.OfflineID.ToString()), 
				new OfflineField("playernumfrom", this.PlayerNumberFrom.ToString()), 
				new OfflineField("playernumto", this.PlayerNumberTo.ToString())
			};
		}

		protected override OfflineEntity GetExistingEntity()
		{
			OfflineEntity[] teams = OfflineEntity.LoadAllEntities(this.GetType());
			foreach (OfflineTeam team in teams)
			{
				if ((team.Phase == this.Phase) && (team.Group == this.Group))
				{
					if ((team.School != null) && (this.School != null))
						if (team.School.Symbol == this.School.Symbol)
							return team;
					if ((team.OfflineSchool != null) && (this.OfflineSchool != null))
						if (team.OfflineSchool.OfflineID == this.OfflineSchool.OfflineID)
							return team;
				}
			}
			return null;
		}

		protected override void Load(OfflineField[] fields)
		{
			foreach (OfflineField field in fields)
			{
				string name = field.Name;
				string value = field.Value;
				if (name == "champcategory")
					this.ChampionshipCategory = (value.Length == 0) ? -1 : Int32.Parse(value);
				else if (name == "phase")
					this.Phase = (value.Length == 0) ? -1 : Int32.Parse(value);
				else if (name == "group")
					this.Group = (value.Length == 0) ? -1 : Int32.Parse(value);
				else if (name == "school")
				{
					int ID = (value.Length == 0) ? -1 : Int32.Parse(value);
					if (ID >= 0)
						this.School = new School(ID);
				}
				else if (name == "offlineschool")
				{
					int ID = (value.Length == 0) ? -1 : Int32.Parse(value);
					if (ID >= 0)
					{
						this.OfflineSchool = (OfflineSchool)
							OfflineEntity.LoadEntity(typeof(OfflineSchool), ID);
					}
				}
				else if (name == "playernumfrom")
					this.PlayerNumberFrom = (value.Length == 0) ? -1 : Int32.Parse(value);
				else if (name == "playernumto")
					this.PlayerNumberTo = (value.Length == 0) ? -1 : Int32.Parse(value);
			}
		}

		public override string ToString()
		{
			if (this.School != null)
				return this.School.Name;
			if (this.OfflineSchool != null)
				return this.OfflineSchool.ToString();
			return null;
		}
	}
}
