using System;
using Sport.UI;

namespace Sportsman.Entities
{
	public class PlayerView : EntityView
	{
		// Student members
		private EntitySelectionDialog studentDialog;

		public PlayerView()
			: base (Sport.Entities.Player.Type)
		{
			//
			// Entity
			//
			Name = "שחקן";
			PluralName = "שחקנים";

			//
			// Fields
			//

			// Id
			EntityFieldView efv = Fields[(int) Sport.Entities.Player.Fields.Id];
			efv.Name = "קוד";
			efv.Width = 30;
			// Student
			efv = Fields[(int) Sport.Entities.Player.Fields.Student];
			efv.Name = "תלמיד";
			efv.Width = 120;
			studentDialog = new EntitySelectionDialog(new Views.StudentsTableView());
			studentDialog.View.State[View.SelectionDialog] = "1";
			((TableView) studentDialog.View).ToolBarEnabled = true;
			efv.GenericItemType = Sport.UI.Controls.GenericItemType.Button;
			efv.Values = Sport.UI.Controls.GenericItem.ButtonValues(new Sport.UI.Controls.ButtonBox.SelectValue(studentDialog.ValueSelector));
			// Team
			efv = Fields[(int) Sport.Entities.Player.Fields.Team];
			efv.Name = "קבוצה";
			efv.Width = 100;
			// Number
			efv = Fields[(int) Sport.Entities.Player.Fields.Number];
			efv.Name = "מספר";
			efv.Width = 80;
			// Status
			efv = Fields[(int) Sport.Entities.Player.Fields.Status];
			efv.Name = "סטטוס";
			efv.Width = 100;
			// Remarks
			efv = Fields[(int) Sport.Entities.Player.Fields.Remarks];
			efv.Name = "הערות";
			efv.Size = new System.Drawing.Size(180, 75);
			// RegisterDate
			efv = Fields[(int) Sport.Entities.Player.Fields.RegisterDate];
			efv.Name = "ת' רישום";
			efv.Width = 120;
			// LastModified
			efv = Fields[(int) Sport.Entities.Player.Fields.LastModified];
			efv.Name = "ת' שינוי אחרון";
			efv.Width = 120;
			// IdNumber
			efv = Fields[(int) Sport.Entities.Player.Fields.IdNumber];
			efv.Name = "ת.ז.";
			efv.Width = 120;
			// FirstName
			efv = Fields[(int) Sport.Entities.Player.Fields.FirstName];
			efv.Name = "שם פרטי";
			efv.Width = 140;
			// LastName
			efv = Fields[(int) Sport.Entities.Player.Fields.LastName];
			efv.Width = 140;
			// Grage
			efv = Fields[(int) Sport.Entities.Player.Fields.Grade];
			efv.Name = "כיתה";
			efv.Width = 80;
			// BirthDate
			efv = Fields[(int) Sport.Entities.Player.Fields.BirthDate];
			efv.Name = "ת' לידה";
			efv.Width = 120;
			// SexType
			efv = Fields[(int) Sport.Entities.Player.Fields.SexType];
			efv.Name = "מין";
			efv.Width = 40;
			// Championship
			efv = Fields[(int) Sport.Entities.Player.Fields.Championship];
			efv.Name = "אליפות";
			efv.Width = 200;
			// Category
			efv = Fields[(int) Sport.Entities.Player.Fields.Category];
			efv.Name = "קטגוריה";
			efv.Width = 150;
		}

		public override void OnSelectEntity(Sport.Data.Entity entity)
		{
			if (entity != null)
			{
				Sport.Entities.Player player = new Sport.Entities.Player(entity);
				Sport.Entities.Team team = player.Team;

				if (team != null)
				{
					studentDialog.View.State[Sport.Entities.Region.TypeName] = team.School.Region.Id.ToString();
					studentDialog.View.State[Sport.Entities.City.TypeName] = team.School.City.Id.ToString();
					studentDialog.View.State[Sport.Entities.School.TypeName] = team.School.Id.ToString();
					studentDialog.View.State["category"] = team.Category.Category.ToString();
					studentDialog.View.State["team"] = team.Id.ToString();
				}
				else
				{
					studentDialog.View.State[Sport.Entities.Region.TypeName] = Sport.Core.Session.Region.ToString();
					studentDialog.View.State[Sport.Entities.City.TypeName] = null;
					studentDialog.View.State[Sport.Entities.School.TypeName] = null;
					studentDialog.View.State["category"] = null;
					studentDialog.View.State["team"] = null;
				}

				Sport.Entities.Student student = player.Student;
				if (student != null)
				{
					studentDialog.View.State[Sport.Entities.Student.TypeName] = student.Id.ToString();
				}
				else
				{
					studentDialog.View.State[Sport.Entities.Student.TypeName] = null;
				}
			}
		}

	}
}
