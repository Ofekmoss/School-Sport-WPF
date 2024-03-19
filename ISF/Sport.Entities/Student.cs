using System;
using Sport.Data;
using Sport.Types;

namespace Sport.Entities
{
	public class Student : EntityBase
	{
		public static readonly string TypeName = "student";
		public static readonly EntityType Type;

		public enum Fields
		{
			Id,
			IdNumberType,
			IdNumber,
			FirstName,
			LastName,
			BirthDate,
			School,
			Grade,
			SexType,
			LastModified,
			DataFields,
			SchoolSymbol = DataFields,
			FieldCount
		}

		static Student()
		{
			Type = new EntityType(TypeName, (int)Fields.FieldCount, (int)Fields.DataFields, new FieldEntityId((int)Fields.Id));

			Type[(int)Fields.Id] = new NumberEntityField(Type, (int)Fields.Id);
			Type[(int)Fields.Id].CanEdit = false;
			Type[(int)Fields.BirthDate] = new DateTimeEntityField(Type, (int)Fields.BirthDate, "dd/MM/yyyy");
			Type[(int)Fields.School] = new EntityEntityField(Type, (int)Fields.School, School.TypeName);
			Type[(int)Fields.School].MustExist = true;
			Type[(int)Fields.Grade] = new LookupEntityField(Type, (int)Fields.Grade, new GradeTypeLookup(true));
			//Type[(int) Fields.Grade] = new NumberEntityField(Type, (int) Fields.Grade);
			Type[(int)Fields.Grade].MustExist = true;
			Type[(int)Fields.LastModified] = new DateTimeEntityField(Type, (int)Fields.LastModified, "dd/MM/yyyy HH:mm:ss");
			Type[(int)Fields.LastModified].CanEdit = false;
			Type[(int)Fields.FirstName].MustExist = true;
			//Type[(int) Fields.LastName].MustExist = true;
			Type[(int)Fields.SexType] = new LookupEntityField(Type, (int)Fields.SexType, new SexTypeLookup());
			//Type[(int) Fields.IdNumber] = new NumberEntityField(Type, (int) Fields.IdNumber);
			Type[(int)Fields.IdNumber].MustExist = true;

			// Relative fields
			Type[(int)Fields.SchoolSymbol] = new EntityRelationEntityField(Type,
				(int)Fields.SchoolSymbol,
				(int)Fields.School,
				School.TypeName, (int)School.Fields.Symbol);

			Type.NameField = new FormatEntityField(Type, "{1}, {0}",
				new int[] { (int)Fields.FirstName, (int)Fields.LastName });
			Type.DateLastModified = Type[(int)Fields.LastModified];

			Type.RefreshRate = 120;
		}

		public Student(Entity entity)
			: base(entity)
		{
		}

		public Student(EntityEdit entityEdit)
			: base(entityEdit)
		{
		}

		public Student(int studentID)
			: base(Type, studentID)
		{
		}

		public int IdNumberType
		{
			get {
				object value = GetValue((int)Fields.IdNumberType);
				return value == null ? 0 : (int)value;
			}
			set { SetValue((int)Fields.IdNumberType, value); }
		}

		public string IdNumber
		{
			get
			{
				int index = (int)Fields.IdNumber;
				object value = GetValue(index);
				return (value == null) ? "" : value.ToString();
			}
			set { SetValue((int)Fields.IdNumber, value); }
		}

		public string FirstName
		{
			get
			{
				object value = GetValue((int)Fields.FirstName);
				return (value == null) ? "" : value.ToString();
			}
			set { SetValue((int)Fields.FirstName, value); }
		}

		public string LastName
		{
			get
			{
				object value = GetValue((int)Fields.LastName);
				return (value == null) ? "" : value.ToString();
			}
			set { SetValue((int)Fields.LastName, value); }
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

		public School School
		{
			get
			{
				Entity entity = null;
				try
				{
					entity = GetEntityValue((int)Fields.School);
				}
				catch { }
				if (entity == null)
					return null;
				return new School(entity);
			}
			set
			{
				SetValue((int)Fields.School, value.Entity);
			}
		}

		public int Grade
		{
			get
			{
				object value = GetValue((int)Fields.Grade);
				return Common.Tools.CIntDef(value, 0);
			}
			set { SetValue((int)Fields.Grade, value); }
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

		public static System.Drawing.Image StudentPicture(string student_number)
		{
			byte[] binData = null;
			using (SessionServices.SessionService sessionService = new SessionServices.SessionService())
			{
				binData = sessionService.GetStudentPicture(student_number);
			}
			if ((binData == null) || (binData.Length == 0))
				return null;
			//Mir.Common.Logger.Instance.WriteLog(Mir.Common.LogType.Debug, "Got " + binData.Length + " bytes of data for student " + student_number);
			System.Drawing.Image objImage = null;
			using (System.IO.MemoryStream objStream = new System.IO.MemoryStream(binData))
			{
				try
				{
					objImage = System.Drawing.Image.FromStream(objStream);
				}
				catch (Exception ex)
				{
					Mir.Common.Logger.Instance.WriteLog(Mir.Common.LogType.Error, "Error generating picture for student " + student_number + " (Got " + binData.Length + " bytes of data)");
					Mir.Common.Logger.Instance.WriteLog(Mir.Common.LogType.Error, ex.ToString());
					objImage = null;
				}
			}
			return objImage;
		}

		public override string CanDelete()
		{
			//begin checkings... first check if the student is part of any team:
			EntityFilter filter = new EntityFilter((int)Player.Fields.Student, this.Id);
			Player[] players = this.GetPlayers();
			string result = "";
			if (players.Length > 0)
			{
				string teams = "";
				foreach (Player player in players)
				{
					if ((player.Team != null) && (player.Team.Championship != null))
						teams += player.Team.TeamName() + " (" + player.Team.Championship.Name + ")\n";
				}
				if (teams.Length > 0)
				{
					result += "התלמיד '" + this.FirstName + " " + this.LastName + "' ";
					result += "רשום לקבוצות הבאות: \n" + teams;
					result += "יש להסיר את התלמיד מקבוצות אלו";
				}
			}
			return result;
		}

		public Player[] GetPlayers()
		{
			EntityFilter filter = new EntityFilter((int)Player.Fields.Student, Id);
			Entity[] entities = Player.Type.GetEntities(filter);
			Player[] players = new Player[entities.Length];
			for (int i = 0; i < entities.Length; i++)
			{
				players[i] = new Player(entities[i]);
			}
			return players;
		}

		public static Student FromIdNumber(string strIdNumber)
		{
			if (!Common.Tools.IsInteger(strIdNumber))
				return null;
			Data.EntityFilter filter = new Data.EntityFilter((int)Fields.IdNumber, strIdNumber);
			Data.Entity[] arrStudents = Student.Type.GetEntities(filter);
			Student student = null;
			if ((arrStudents != null) && (arrStudents.Length > 0))
			{
				try
				{
					student = new Student(arrStudents[0]);
				}
				catch
				{
					System.Diagnostics.Debug.WriteLine("failed to get student from id number: " + strIdNumber);
				}
			}
			return student;
		}
	}

	public class StudentCategoryFilter : EntityFilterField
	{
		public StudentCategoryFilter(int category)
			: base(-1, category)
		{
		}

		public override bool CompareValue(Entity e)
		{
			Student student = new Student(e);
			int grade = GradeTypeLookup.ToGrade(student.Grade);
			Sex sex = student.SexType;
			if (sex == Sex.None)
				sex = Sex.Both;

			int studentCategory = CategoryTypeLookup.ToCategory(sex, grade - 1);

			return CategoryTypeLookup.Compare((int)_value, studentCategory, CategoryCompareType.Partial);
		}
	}

	public class OfflineStudent : Data.OfflineEntity
	{
		private string _idNumber = "";
		private string _firstName = "";
		private string _lastName = "";
		private int _grade = 0;
		private School _school = null;
		private OfflineSchool _offlineSchool = null;

		protected override string GetOfflineType()
		{
			return "student";
		}

		public string IdNumber
		{
			get { return _idNumber; }
			set { _idNumber = value; }
		}

		public string FirstName
		{
			get { return _firstName; }
			set { _firstName = value; }
		}

		public string LastName
		{
			get { return _lastName; }
			set { _lastName = value; }
		}

		public int Grade
		{
			get { return _grade; }
			set { _grade = value; }
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

		public OfflineStudent()
			: base()
		{
		}

		public OfflineStudent(string idNumber, string firstName, string lastName,
			int grade, object school)
			: this()
		{
			this.IdNumber = idNumber;
			this.FirstName = firstName;
			this.LastName = lastName;
			this.Grade = grade;
			if (school is Entity)
				this.School = new School(school as Entity);
			else if (school is OfflineSchool)
				this.OfflineSchool = (OfflineSchool)school;
		}

		protected override OfflineField[] GetFields()
		{
			return new OfflineField[] {
				new OfflineField("idnumber", this.IdNumber), 
				new OfflineField("firstname", this.FirstName), 
				new OfflineField("lastname", this.LastName), 
				new OfflineField("grade", this.Grade.ToString()), 
				new OfflineField("school", (this.School == null)?"-1":this.School.Id.ToString()), 
				new OfflineField("offlineschool", (this.OfflineSchool == null)?"-1":this.OfflineSchool.OfflineID.ToString())
			};
		}

		protected override OfflineEntity GetExistingEntity()
		{
			OfflineEntity[] students = OfflineEntity.LoadAllEntities(this.GetType());
			foreach (OfflineStudent student in students)
				if (student.IdNumber == this.IdNumber)
					return student;
			return null;
		}

		protected override void Load(OfflineField[] fields)
		{
			foreach (OfflineField field in fields)
			{
				string name = field.Name;
				string value = field.Value;
				if (name == "idnumber")
					this.IdNumber = value;
				else if (name == "firstname")
					this.FirstName = value;
				else if (name == "lastname")
					this.LastName = value;
				else if (name == "grade")
					this.Grade = (value.Length == 0) ? 0 : Int32.Parse(value);
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
			}
		}

		public override string ToString()
		{
			return this.FirstName + " " + this.LastName;
		}

		public int GetRegion()
		{
			if ((this.School != null) && (this.School.Region != null))
				return this.School.Region.Id;
			if (this.OfflineSchool != null)
				return this.OfflineSchool.Region;
			return -1;
		}
	}
}
