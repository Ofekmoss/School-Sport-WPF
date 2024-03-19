using System;
using Sport.Data;
using Sport.Types;
using Sport.Core;

namespace Sport.Entities
{
	public class Player : EntityBase
	{
		public static readonly string TypeName = "player";
		public static readonly EntityType Type;

		public enum Fields
		{
			Id = 0,
			Student,
			Team,
			Number,
			Charge,
			Status,
			Remarks,
			RegisterDate,
			Got_Sticker,
			LastModified,
			DataFields,
			IdNumber = DataFields,
			FirstName,
			LastName,
			Grade,
			BirthDate,
			SexType,
			Championship,
			Category,
			FieldCount
		}
		
		static Player()
		{
			try
			{
				Type = new EntityType(TypeName, (int) Fields.FieldCount, (int) Fields.DataFields, new FieldEntityId((int) Fields.Id), Session.GetSeasonCache());

				Type[(int) Fields.Id] = new NumberEntityField(Type, (int) Fields.Id);
				Type[(int) Fields.Id].CanEdit = false;
				Type[(int) Fields.Team] = new EntityEntityField(Type, (int) Fields.Team, Team.TypeName);
				Type[(int) Fields.Team].MustExist = true;
				Type[(int) Fields.Student] = new EntityEntityField(Type, (int) Fields.Student, Student.TypeName);
				Type[(int) Fields.Student].MustExist = true;
				Type[(int) Fields.Number] = new NumberEntityField(Type, (int) Fields.Number, 0, 99999);
				Type[(int) Fields.Charge] = new EntityEntityField(Type, (int) Fields.Charge, Charge.TypeName);
				Type[(int) Fields.Charge].CanEdit = false;
				Type[(int) Fields.Status] = new LookupEntityField(Type, (int) Fields.Status, new PlayerStatusLookup());
				Type[(int) Fields.RegisterDate] = new DateTimeEntityField(Type, (int) Fields.RegisterDate, "dd/MM/yyyy HH:mm");
				//Type[(int) Fields.RegisterDate].MustExist = true;
				Type[(int) Fields.RegisterDate].CanEdit = false;
				Type[(int)Fields.Got_Sticker] = new LookupEntityField(Type, (int)Fields.Got_Sticker, new BooleanTypeLookup());
				Type[(int) Fields.LastModified] = new DateTimeEntityField(Type, (int) Fields.LastModified, "dd/MM/yyyy HH:mm:ss");
				Type[(int) Fields.LastModified].CanEdit = false;
			
				// Relative fields
				Type[(int) Fields.IdNumber] = new EntityRelationEntityField(Type, 
					(int) Fields.IdNumber,
					(int) Fields.Student,
					Student.TypeName, (int) Student.Fields.IdNumber);
				Type[(int) Fields.FirstName] = new EntityRelationEntityField(Type, 
					(int) Fields.FirstName,
					(int) Fields.Student, 
					Student.TypeName, (int) Student.Fields.FirstName);
				Type[(int) Fields.LastName] = new EntityRelationEntityField(Type, 
					(int) Fields.LastName,
					(int) Fields.Student,
					Student.TypeName, (int) Student.Fields.LastName);
				Type[(int) Fields.Grade] = new EntityRelationEntityField(Type, 
					(int) Fields.Grade,
					(int) Fields.Student,
					Student.TypeName, (int) Student.Fields.Grade);
				Type[(int) Fields.BirthDate] = new EntityRelationEntityField(Type, 
					(int) Fields.BirthDate,
					(int) Fields.Student,
					Student.TypeName, (int) Student.Fields.BirthDate);
				Type[(int) Fields.SexType] = new EntityRelationEntityField(Type, 
					(int) Fields.SexType,
					(int) Fields.Student,
					Student.TypeName, (int) Student.Fields.SexType);

				Type[(int) Fields.Championship] = new EntityRelationEntityField(Type, 
					(int) Fields.Championship,
					(int) Fields.Team,
					Team.TypeName, (int) Team.Fields.Championship);
				Type[(int) Fields.Category] = new EntityRelationEntityField(Type, 
					(int) Fields.Category,
					(int) Fields.Team,
					Team.TypeName, (int) Team.Fields.Category);

				Type.NameField = new FormatEntityField(Type, "{1}, {0}", 
					new int[] { (int) Fields.FirstName, (int) Fields.LastName });
				Type.DateLastModified = Type[(int) Fields.LastModified];

				Type.NewEntity = new EntityType.EntityNewEntity(NewEntity);
				Type.EntityCheck += new EntityType.EntityEntityCheck(EntityCheck);
				Type.EntityAdded += new EntityEventHandler(EntityAdded);
				Type.EntityRemoved += new EntityEventHandler(EntityRemoved);
				Type.EntityUpdated += new EntityEventHandler(EntityUpdated);
			}
			catch (Exception ex)
			{
				Data.AdvancedTools.ReportExcpetion(ex);
				throw;
			}
		}

		private static void NewEntity(EntityEdit entityEdit)
		{
			Type[(int) Fields.RegisterDate].SetValue(entityEdit, DateTime.Now);
		}

		private static EntityResult EntityCheck(Entity entity)
		{
			Player player = new Player(entity);
			if (player.Number >= 0)
			{
				Player[] otherPlayers = player.Team.GetPlayers();
				foreach (Player otherPlayer in otherPlayers)
				{
					if (!player.Equals(otherPlayer))
					{
						if (otherPlayer.Number == player.Number)
							return new EntityMessageResult(EntityResultCode.UniqueValueFailure, "מספר חולצה קיים בקבוצה");
					}
				}
				
				if (player.Team.Championship.Sport.SportType == 
					Types.SportType.Competition)
				{
					/*
					int number=player.Number;
					int min=player.Team.PlayerNumberFrom;
					int max=player.Team.PlayerNumberTo;
					if ((number < min)||(number > max))
						return new EntityMessageResult(EntityResultCode.ValueOutOfRange, "מספר חולצה לא בטווח מספרי חזה של בית הספר");
					*/
				}
			}

			return EntityResult.Ok;
		}

		public Player(Entity entity)
			: base(entity)
		{
		}

		public Player(EntityEdit entityEdit)
			: base(entityEdit)
		{
		}

		public Player(int playerId)
			: base(Type, playerId)
		{
		}

		public string StatusText
		{
			get 
			{
				object statusValue=GetValue((int) Fields.Status);
				if (statusValue == null)
					return "";
				int statusType=(int) statusValue;
				PlayerStatusLookup pl=new PlayerStatusLookup();
				if (statusType != (int) PlayerStatusType.Not_Confirmed)
				{
					return pl[statusType].Text;
				}
				else
				{
					string remarks=(string) GetValue((int) Fields.Remarks);
					return pl[statusType].Text+" - "+remarks;
				}
			}
		}

		public Team Team
		{
			get
			{
				Team result=null;
				try
				{
					Entity entity = GetEntityValue((int) Fields.Team);
					if (entity != null)
						result = new Team(entity);
				}
				catch (Exception e)
				{
					System.Diagnostics.Debug.WriteLine("failed to get player team: "+e.Message);
					System.Diagnostics.Debug.WriteLine("player id: "+this.Id);
					System.Diagnostics.Debug.WriteLine(e.StackTrace);
				}
				return result;
			}
			set 
			{ 
				SetValue((int) Fields.Team, value.Entity); 
			}
		}

		public Student Student
		{
			get
			{
				Entity entity = null;
				try
				{
					entity = GetEntityValue((int) Fields.Student);
				}
				catch {}
				if (entity == null)
					return null;
				return new Student(entity);
			}
			set 
			{ 
				SetValue((int) Fields.Student, value.Entity); 
			}
		}
		
		public Charge Charge
		{
			get
			{
				Charge result=null;
				try
				{
					Entity entity = GetEntityValue((int) Fields.Charge);
					if (entity != null)
						result = new Charge(entity);
				}
				catch (Exception e)
				{
					System.Diagnostics.Debug.WriteLine("failed to load player charge. player id: "+this.Id.ToString());
					System.Diagnostics.Debug.WriteLine("error: "+e.Message);
					System.Diagnostics.Debug.WriteLine(e.StackTrace);
				}
				return result;
			}
			set 
			{ 
				SetValue((int) Fields.Charge, value.Entity); 
			}
		}
		
		public int Status
		{
			get { return Common.Tools.CIntDef(GetValue((int) Fields.Status), (int) Types.PlayerStatusType.Registered); }
			set { SetValue((int) Fields.Status, value); }
		}
		
		public int Number
		{
			get { return Common.Tools.CIntDef(GetValue((int) Fields.Number), -1); }
			set 
			{ 
				if (value == -1)
					SetValue((int) Fields.Number, null);
				else
					SetValue((int) Fields.Number, value);
			}
		}

		public string IdNumber
		{
			get { return Common.Tools.CStrDef(GetValue((int) Fields.IdNumber), ""); }
		}

		public string FirstName
		{
			get { return (string) GetValue((int) Fields.FirstName); }
		}
		
		public string LastName
		{
			get { return (string) GetValue((int) Fields.LastName); }
		}

		public DateTime RegisterDate
		{
			get
			{ 
				object o = GetValue((int) Fields.RegisterDate);
				if (o == null)
					return DateTime.MinValue;
				else
					return (DateTime) o;
			}
			set { SetValue((int) Fields.RegisterDate, value); }
		}

		public int GotSticker
		{
			get { return Common.Tools.CIntDef(GetValue((int)Fields.Got_Sticker), 0); }
			set { SetValue((int)Fields.Got_Sticker, value); }
		}

		public bool IsOverAge()
		{
			if (this.Team != null && this.Team.Category != null)
			{
				DateTime maxBirthday=this.Team.Category.MaxStudentBirthday;
				if (maxBirthday.Year < 1900)
					return false;

				Student student=null;
				try
				{
					student = this.Student;
				}
				catch {}

				if (student != null && student.Id >= 0)
				{
					DateTime studentBirthday = this.Student.BirthDate;
					if (studentBirthday.Year < 1900)
						return false;
					else
						return (studentBirthday < maxBirthday);
				}
			}

			return false;
		}
		
		public override bool IsValid()
		{
			if (_entity == null)
				return false;
			if (this.Id < 0)
				return false;
			Student student=this.Student;
			if ((student == null)||(student.Id < 0))
				return false;
			Team team=this.Team;
			if ((team == null)||(team.Id < 0))
				return false;
			School school=student.School;
			if ((school == null)||(school.Id < 0))
				return false;
			return true;
		}
		
		public static Player FromStudentAndTeam(int studentID, int teamID)
		{
			Data.EntityFilter filter = new Data.EntityFilter(
				(int) Fields.Student, studentID);
			filter.Add(
				new Data.EntityFilterField((int) Fields.Team, teamID));
			Data.Entity[] arrPlayers = Player.Type.GetEntities(filter);
			Player player = null;
			if ((arrPlayers != null)&&(arrPlayers.Length > 0))
			{
				try
				{
					player = new Player(arrPlayers[0]);
				}
				catch
				{
					System.Diagnostics.Debug.WriteLine("failed to get player from student and team: "+studentID+", "+teamID);
				}
			}
			return player;
		}

		private static void EntityAdded(object sender, EntityEventArgs e)
		{
			EntityChanged(e.Entity);
		}

		private static void EntityRemoved(object sender, EntityEventArgs e)
		{
			EntityChanged(e.Entity);
		}

		private static void EntityUpdated(object sender, EntityEventArgs e)
		{
			EntityChanged(e.Entity);
		}

		private static void EntityChanged(Entity entity)
		{
			if (entity != null)
			{
				object oTeamId = entity.Fields[(int)Fields.Team];
				if (oTeamId != null)
					Team.ResetTeamPlayers((int)oTeamId);
				entity.EntityType.ClearAllCache();
			}
		}
	}
	
	public class OfflinePlayer : Data.OfflineEntity
	{
		private Team _team = null;
		private OfflineTeam _offlineTeam = null;
		private Student _student = null;
		private OfflineStudent _offlineStudent = null;
		private int _shirtNumber = 0;
		private int _competition = -1;
		private int _rank = -1;
		private int _result = -1;
		private int _score = -1;
		private int _customPosition = -1;
		
		protected override string GetOfflineType()
		{
			return "player";
		}
		
		public Team Team
		{
			get { return _team; }
			set	{ _team = value; }
		}
		
		public OfflineTeam OfflineTeam
		{
			get { return _offlineTeam; }
			set { _offlineTeam = value; }
		}
		
		public Student Student
		{
			get { return _student; }
			set	{ _student = value; }
		}
		
		public OfflineStudent OfflineStudent
		{
			get { return _offlineStudent; }
			set { _offlineStudent = value; }
		}
		
		public int ShirtNumber
		{
			get { return _shirtNumber; }
			set { _shirtNumber = value; }
		}
		
		public int Competition
		{
			get { return _competition; }
			set { _competition = value; }
		}
		
		public int Rank
		{
			get
			{
				if (_customPosition >= 0)
					return _customPosition;
				return _rank;
			}
			set { _rank = value; }
		}
		
		public int Result
		{
			get { return _result; }
			set { _result = value; }
		}
		
		public int Score
		{
			get { return _score; }
			set { _score = value; }
		}
		
		public int CustomPosition
		{
			get { return _customPosition; }
			set { _customPosition = value; }
		}
		
		public OfflinePlayer()
			: base()
		{
			_customPosition = -1;
		}
		
		public OfflinePlayer(object team, object student, int shirtNumber, 
			int competition, int rank, int result, int score)
			: this()
		{
			if (team is Data.Entity)
				this.Team = new Team(team as Data.Entity);
			else if (team is OfflineTeam)
				this.OfflineTeam = (OfflineTeam) team;
			if (student is Data.Entity)
				this.Student = new Student(student as Data.Entity);
			else if (student is OfflineStudent)
				this.OfflineStudent = (OfflineStudent) student;
			this.ShirtNumber = shirtNumber;
			this.Competition = competition;
			this.Rank = rank;
			this.Result = result;
			this.Score = score;
		}
		
		public OfflinePlayer(object team, object student, int shirtNumber, 
			int competition)
			: this(team, student, shirtNumber, competition, -1, -1, -1)
		{
		}
		
		protected override OfflineField[] GetFields()
		{
			return new OfflineField[] {
				new OfflineField("team", (this.Team == null)?"-1":this.Team.Id.ToString()), 
				new OfflineField("offlineteam", (this.OfflineTeam == null)?"-1":this.OfflineTeam.OfflineID.ToString()), 
				new OfflineField("student", (this.Student == null)?"-1":this.Student.Id.ToString()), 
				new OfflineField("offlinestudent", (this.OfflineStudent == null)?"-1":this.OfflineStudent.OfflineID.ToString()), 
				new OfflineField("shirtnumber", this.ShirtNumber.ToString()), 
				new OfflineField("competition", this.Competition.ToString()), 
				new OfflineField("rank", this._rank.ToString()), 
				new OfflineField("result", this.Result.ToString()), 
				new OfflineField("score", this.Score.ToString()), 
				new OfflineField("customposition", this.CustomPosition.ToString())
			};
		}
		
		protected override OfflineEntity GetExistingEntity()
		{
			OfflineEntity[] players = OfflineEntity.LoadAllEntities(this.GetType());
			foreach (OfflinePlayer player in players)
			{
				if (player.Competition == this.Competition)
				{
					if ((player.Student != null)&&(this.Student != null))
						if (player.Student.Id == this.Student.Id)
							return player;
					if ((player.OfflineStudent != null)&&(this.OfflineStudent != null))
						if (player.OfflineStudent.OfflineID == this.OfflineStudent.OfflineID)
							return player;
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
				if (name == "team")
				{
					int ID = (value.Length == 0)?-1:Int32.Parse(value);
					if (ID >= 0)
						this.Team = new Team(ID);
				}
				else if (name == "offlineteam")
				{
					int ID = (value.Length == 0)?-1:Int32.Parse(value);
					if (ID >= 0)
					{
						this.OfflineTeam = (OfflineTeam)
							OfflineEntity.LoadEntity(typeof(OfflineTeam), ID);
					}
				}
				else if (name == "student")
				{
					int ID = (value.Length == 0)?-1:Int32.Parse(value);
					if (ID >= 0)
						this.Student = new Student(ID);
				}
				else if (name == "offlinestudent")
				{
					int ID = (value.Length == 0)?-1:Int32.Parse(value);
					if (ID >= 0)
					{
						this.OfflineStudent = (OfflineStudent)
							OfflineEntity.LoadEntity(typeof(OfflineStudent), ID);
					}
				}
				else if (name == "shirtnumber")
					this.ShirtNumber = (value.Length == 0)?-1:Int32.Parse(value);
				else if (name == "competition")
					this.Competition = (value.Length == 0)?-1:Int32.Parse(value);
				else if (name == "rank")
					this.Rank = (value.Length == 0)?-1:Int32.Parse(value);
				else if (name == "result")
					this.Result = (value.Length == 0)?-1:Int32.Parse(value);
				else if (name == "score")
					this.Score = (value.Length == 0)?-1:Int32.Parse(value);
				else if (name == "customposition")
					this.CustomPosition = (value.Length == 0)?-1:Int32.Parse(value);
			}
		}
		
		public override string ToString()
		{
			if (this.Student != null)
				return this.Student.FirstName+" "+this.Student.LastName;
			if (this.OfflineStudent != null)
				return this.OfflineStudent.ToString();
			return null;
		}
		
		public string GetTeamName()
		{
			if (this.Team != null)
				return this.Team.TeamName();
			if (this.OfflineTeam != null)
				return this.OfflineTeam.ToString();
			return null;
		}
		
		public Team GetPlayerTeam()
		{
			if (this.Team != null)
				return this.Team;
			if (!Core.Session.Connected)
				return null;
			if (this.OfflineTeam == null)
				return null;
			Student student = null;
			if (this.Student != null)
				student = this.Student;
			else if (this.OfflineStudent != null)
				student = Student.FromIdNumber(this.OfflineStudent.IdNumber);
			if ((student == null)||(student.School == null))
				return null;
			return Team.FromSchoolAndCategory(student.School.Id, this.OfflineTeam.ChampionshipCategory);
		}
		
		public Student GetPlayerStudent()
		{
			if (this.Student != null)
				return this.Student;
			if (!Core.Session.Connected)
				return null;
			if (this.OfflineStudent == null)
				return null;
			return Student.FromIdNumber(this.OfflineStudent.IdNumber);
		}
		
		public Player GetPlayerEntity()
		{
			Team team = this.GetPlayerTeam();
			if (team == null)
				return null;
			Student student = this.GetPlayerStudent();
			if (student == null)
				return null;
			return Player.FromStudentAndTeam(student.Id, team.Id);
		}
	}
}
