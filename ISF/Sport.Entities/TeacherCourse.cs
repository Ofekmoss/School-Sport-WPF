using System;
using Sport.Data;
using Sport.Common;
using Sport.Types;
using Sport.Core;

namespace Sport.Entities
{
	public class TeacherCourse : EntityBase
	{
		public static readonly string TypeName = "teachercourse";
		public static readonly EntityType Type;
		
		public enum Fields
		{
			Id = 0,
			FirstName,
			LastName,
			IdNumber, 
			BirthDay,
			Address,
			CityName,
			ZipCode,
			Email,
			CellPhone,
			HomePhone,
			FaxNumber,
			Gender,
			SchoolName,
			SchoolCityName,
			SchoolAddress,
			FirstSport,
			SecondSport,
			Veteranship,
			Expertise,
			TeamAgeRange,
			CourseHoliday,
			CourseYear,
			CourseSport,
			Charge,
			IsConfirmed,
			CoachTrainingType,
			CoachTrainingHours,
			LastModified,
			FieldCount
		}
		
		static TeacherCourse()
		{
			Type = new EntityType(TypeName, (int) Fields.FieldCount, (int) Fields.FieldCount, new FieldEntityId((int) Fields.Id));
			
			Type[(int) Fields.Id] = new NumberEntityField(Type, (int) Fields.Id);
			Type[(int) Fields.Id].CanEdit = false;
			Type[(int) Fields.FirstName].MustExist = true;
			Type[(int) Fields.Gender] = new LookupEntityField(Type, (int) Fields.Gender, new SexTypeLookup());
			Type[(int) Fields.FirstSport] = new EntityEntityField(Type, (int) Fields.FirstSport, Sport.TypeName);
			Type[(int) Fields.SecondSport] = new EntityEntityField(Type, (int) Fields.SecondSport, Sport.TypeName);
			Type[(int) Fields.Veteranship] = new NumberEntityField(Type, (int) Fields.Veteranship, 0, 99);
			Type[(int) Fields.Expertise] = new LookupEntityField(Type, (int) Fields.Expertise, new TeacherExpertiseLookup());
			Type[(int) Fields.TeamAgeRange] = new LookupEntityField(Type, (int) Fields.TeamAgeRange, new CourseAgeRangeLookup());
			Type[(int) Fields.CourseHoliday] = new LookupEntityField(Type, (int) Fields.CourseHoliday, new HolidayTypeLookup());
			Type[(int) Fields.CourseYear] = new NumberEntityField(Type, (int) Fields.CourseYear, 2006, 2099);
			Type[(int) Fields.CourseSport] = new EntityEntityField(Type, (int) Fields.CourseSport, Sport.TypeName);
			Type[(int) Fields.Charge] = new EntityEntityField(Type, (int) Fields.Charge, Charge.TypeName);
			Type[(int) Fields.Charge].CanEdit = false;
			Type[(int) Fields.IsConfirmed] = new LookupEntityField(Type, (int) Fields.IsConfirmed, new BooleanTypeLookup("מאושר", "לא מאושר"));
			Type[(int) Fields.CoachTrainingHours] = new NumberEntityField(Type, (int) Fields.CoachTrainingHours, 0, 9999);
			Type[(int) Fields.LastModified] = new DateTimeEntityField(Type, (int) Fields.LastModified, "dd/MM/yyyy HH:mm:ss");
			Type[(int) Fields.LastModified].CanEdit = false;
			
			Type.NameField = new FormatEntityField(Type, 
				"{0} {1}", new int[] { (int) Fields.FirstName, (int) Fields.LastName});
			Type.DateLastModified = Type[(int) Fields.LastModified];
			
			Type.EntityRemoved += new EntityEventHandler(EntityRemoved);
		}
		
		private static void EntityRemoved(object sender, EntityEventArgs e)
		{
			(new TeacherCourse(e.Entity)).RemoveCharge();
		}
		
		private string BeforeDelete(object sender)
		{
			return null;
		}
		
		public TeacherCourse(Entity entity)
			: base(entity)
		{
		}
		
		public TeacherCourse(EntityEdit entityEdit)
			: base(entityEdit)
		{
		}
		
		public TeacherCourse(int teacherCourseId)
			: base(Type, teacherCourseId)
		{
		}

		public string FirstName
		{
			get { return Common.Tools.CStrDef(GetValue((int) Fields.FirstName), ""); }
			set { SetValue((int) Fields.FirstName, value); }
		}
		
		public string LastName
		{
			get { return Common.Tools.CStrDef(GetValue((int) Fields.LastName), ""); }
			set { SetValue((int) Fields.LastName, value); }
		}
		
		public bool IsConfirmed
		{
			get { return (Common.Tools.CIntDef(GetValue((int) Fields.IsConfirmed), 0) == 1); }
			set { SetValue((int) Fields.IsConfirmed, (value) ? 1 : 0); }
		}

		public Charge GetCharge()
		{
			Product product = TeacherCourse.GetProduct();
			if (product == null)
				return null;
			
			EntityFilter filter = new EntityFilter((int) Charge.Fields.Additional, this.Id);
			filter.Add(new Data.EntityFilterField((int) Charge.Fields.Product, product.Id));
			Entity[] entities = Charge.Type.GetEntities(filter);
			
			if (entities == null || entities.Length == 0)
				return null;
			
			return new Charge(entities[0]);
		}

		public string DoCharge()
		{
			if (this.GetCharge() != null)
				return "";
			
			Product product = TeacherCourse.GetProduct();
			if (product == null)
				return "לא מוגדר סוג חיוב";
			
			Account account = this.GetAccount();
			EntityResult result;
			if (account == null)
			{
				try
				{
					result = this.CreateAccount();
					if (result != EntityResult.Ok)
						return "יצירת חשבון נכשלה: \n" + result.GetMessage();
					else
						account = this.GetAccount();
				}
				catch (Exception ex)
				{
					return "שגיאה כללית בעת יצירת חשבון:\n" + ex.Message;
				}
			}
			
			Data.EntityEdit entEdit = Charge.Type.New();
			entEdit.Fields[(int) Charge.Fields.Account] = account.Id;
			entEdit.Fields[(int) Charge.Fields.Product] = product.Id;
			entEdit.Fields[(int) Charge.Fields.Region] = account.Region.Id;
			entEdit.Fields[(int) Charge.Fields.Additional] = this.Id;
			entEdit.Fields[(int) Charge.Fields.Amount] = 1;
			entEdit.Fields[(int) Charge.Fields.Price] = product.Price;
			entEdit.Fields[(int) Charge.Fields.LastModified] = DateTime.Now;
			entEdit.Fields[(int) Charge.Fields.Status] = (int) Types.ChargeStatusType.NotPaid;
			try
			{
				result = entEdit.Save();
			}
			catch (Exception exx)
			{
				return "שגיאה כללית בעת יצירת חיוב:\n" + exx.Message;
			}
			
			if (result != EntityResult.Ok)
				return result.GetMessage();
			
			return "";
		}
		
		private void RemoveCharge()
		{
			Charge charge = this.GetCharge();
			if (charge != null)
				charge.Entity.Delete();
		}

		public static void SetProduct(int productId)
		{
			if (Core.Session.Connected)
			{
				DataServices.DataService service = new DataServices.DataService();
				service.SetTeacherCourseProduct(productId);
			}
		}

		public static Entities.Product GetProduct()
		{
			if (!Core.Session.Connected)
				return null;
			
			DataServices.DataService service = new DataServices.DataService();
			int productId = service.GetTeacherCourseProduct();
			if (productId >= 0)
			{
				try
				{
					return new Entities.Product(productId);
				}
				catch
				{}
			}
			return null;
		}

		public static void SetSports(int[] sports)
		{
			if (Core.Session.Connected)
			{
				DataServices.DataService service = new DataServices.DataService();
				service.SetTeacherCourseSports(sports);
			}
		}
		
		public static Entities.Sport[] GetSports()
		{
			if (!Core.Session.Connected)
				return null;
			
			DataServices.DataService service = new DataServices.DataService();
			int[] sports = service.GetTeacherCourseSports();
			if (sports != null)
			{
				System.Collections.ArrayList arrSports = new System.Collections.ArrayList();
				foreach (int sportId in sports)
				{
					try
					{
						arrSports.Add(new Entities.Sport(sportId));
					}
					catch
					{}
				}
				return (Entities.Sport[]) arrSports.ToArray(typeof(Entities.Sport));
			}
			return null;
		}
		
		private Account GetAccount()
		{
			EntityFilter filter = new EntityFilter((int) Account.Fields.Name, this.FirstName + " " + this.LastName);
			filter.Add(new Data.EntityFilterField((int) Account.Fields.Region, Region.CentralRegion));
			Entity[] entities = Account.Type.GetEntities(filter);
			
			if (entities == null || entities.Length == 0)
				return null;
			
			return new Account(entities[0]);
		}

		private EntityResult CreateAccount()
		{
			Data.EntityEdit entEdit = Account.Type.New();
			entEdit.Fields[(int) Account.Fields.Name] = this.FirstName + " " + this.LastName;
			entEdit.Fields[(int) Account.Fields.Region] = Region.CentralRegion;
			entEdit.Fields[(int) Account.Fields.Address] = Common.Tools.CStrDef(this.Entity.Fields[(int) Fields.Address], "");
			return entEdit.Save();
		}
	}
}