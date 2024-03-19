using System;
using Sport.UI;

namespace Sportsman.Entities
{
	public class StudentView : EntityView
	{
		// School members
		private EntitySelectionDialog schoolDialog;

		public StudentView()
			: base (Sport.Entities.Student.Type)
		{
			//
			// Entity
			//
			Name = "תלמיד";
			PluralName = "תלמידים";

			//
			// Fields
			//
			
			// Id
			EntityFieldView efv = Fields[(int) Sport.Entities.Student.Fields.Id];
			efv.Name = "קוד";
			efv.Width = 30;
			// IdNumberType
			efv = Fields[(int) Sport.Entities.Student.Fields.IdNumberType];
			efv.Name = "סוג זיהוי";
			efv.Width = 30;
			// IdNumber
			efv = Fields[(int) Sport.Entities.Student.Fields.IdNumber];
			efv.Name = "ת.ז.";
			efv.Width = 120;
			// FirstName
			efv = Fields[(int) Sport.Entities.Student.Fields.FirstName];
			efv.Name = "שם פרטי";
			efv.Width = 140;
			// LastName
			efv = Fields[(int) Sport.Entities.Student.Fields.LastName];
			efv.Name = "שם משפחה";
			efv.Width = 140;
			// BirthDate
			efv = Fields[(int) Sport.Entities.Student.Fields.BirthDate];
			efv.Name = "ת' לידה";
			efv.Width = 120;
			// School
			efv = Fields[(int) Sport.Entities.Student.Fields.School];
			efv.Name = "בית ספר";
			efv.Width = 180;
			Views.SchoolsTableView schoolView = new Views.SchoolsTableView();
			schoolView.State[View.SelectionDialog] = "1";
			schoolDialog = new EntitySelectionDialog(schoolView);
			efv.GenericItemType = Sport.UI.Controls.GenericItemType.Button;
			efv.Values = Sport.UI.Controls.GenericItem.ButtonValues(new Sport.UI.Controls.ButtonBox.SelectValue(schoolDialog.ValueSelector));
			// Grade
			efv = Fields[(int) Sport.Entities.Student.Fields.Grade];
			efv.Name = "כיתה";
			efv.Width = 80;
			// SexType
			efv = Fields[(int) Sport.Entities.Student.Fields.SexType];
			efv.Name = "מין";
			efv.Width = 40;
			efv.MustExist = true;
			// LastModified
			efv = Fields[(int) Sport.Entities.Student.Fields.LastModified];
			efv.Name = "ת' שינוי אחרון";
			efv.Width = 120;
			// SchoolSymbol
			efv = Fields[(int) Sport.Entities.Student.Fields.SchoolSymbol];
			efv.Name = "סמל בית ספר";
			efv.Width = 80;
		}


		public override void OnSelectEntity(Sport.Data.Entity entity)
		{
			//got entity?
			if (entity == null)
				return;
			
			//create student:
			Sport.Entities.Student student = new Sport.Entities.Student(entity);
			
			// School field set
			Sport.Entities.School school = student.School;
			
			schoolDialog.View.State[Sport.Entities.Region.TypeName] = Sport.Core.Session.Region.ToString();
			schoolDialog.View.State[Sport.Entities.City.TypeName] = null;
			schoolDialog.View.State[Sport.Entities.School.TypeName] = null;
			
			//got school?
			if (school != null)
			{
				if (school.Region != null)
					schoolDialog.View.State[Sport.Entities.Region.TypeName] = school.Region.Id.ToString();
				if (school.City != null)
					schoolDialog.View.State[Sport.Entities.City.TypeName] = school.City.Id.ToString();
				schoolDialog.View.State[Sport.Entities.School.TypeName] = school.Id.ToString();
			}
		}
	}
}
