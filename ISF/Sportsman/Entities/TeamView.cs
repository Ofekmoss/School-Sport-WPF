using System;
using Sport.UI;

namespace Sportsman.Entities
{
	public class TeamView : EntityView
	{
		// School members
		private EntitySelectionDialog schoolDialog;

		// Supervisor members
		private EntitySelectionDialog userDialog;

		// New team membmers
		private EntityFieldView fieldSport;
		private EntityFieldView fieldChampionship;
		private EntityFieldView fieldCategory;
		
		public TeamView()
			: base (Sport.Entities.Team.Type)
		{
			//
			// Entity
			//
			Name = "קבוצה";
			PluralName = "קבוצות";

			//
			// Fields
			//

			// Id
			EntityFieldView efv = Fields[(int) Sport.Entities.Team.Fields.Id];
			efv.Name = "קוד";
			efv.Width = 30;
			// School
			efv = Fields[(int) Sport.Entities.Team.Fields.School];
			efv.Name = "בית ספר";
			efv.Width = 180;
			schoolDialog = new EntitySelectionDialog(new Views.SchoolsTableView());
			schoolDialog.View.State[View.SelectionDialog] = "1";
			efv.GenericItemType= Sport.UI.Controls.GenericItemType.Button;
			efv.Values = Sport.UI.Controls.GenericItem.ButtonValues(new Sport.UI.Controls.ButtonBox.SelectValue(schoolDialog.ValueSelector));
			// Category
			fieldCategory = Fields[(int) Sport.Entities.Team.Fields.Category];
			fieldCategory.Name = "קטגוריה";
			fieldCategory.Width = 150;
			fieldCategory.CanEdit = false;
			// Championship
			fieldChampionship = Fields[(int) Sport.Entities.Team.Fields.Championship]; 
			fieldChampionship.Name = "אליפות";
			fieldChampionship.Width = 200;
			fieldChampionship.CanEdit = false;
			// Status
			efv = Fields[(int) Sport.Entities.Team.Fields.Status];
			efv.Name = "סטטוס";
			efv.Width = 120;
			// Index
			efv = Fields[(int) Sport.Entities.Team.Fields.Index];
			efv.Name = "מספר סידורי";
			efv.Width = 80;
			// Supervisor
			efv = Fields[(int) Sport.Entities.Team.Fields.Supervisor];
			efv.Name = "אחראי";
			efv.Width = 150;
			userDialog = new EntitySelectionDialog(new Views.UsersTableView());
			userDialog.View.State[View.SelectionDialog] = "1";
			userDialog.View.State["UserType"] = ((int) Sport.Types.UserType.Internal).ToString();
			efv.GenericItemType = Sport.UI.Controls.GenericItemType.Button;
			efv.Values = Sport.UI.Controls.GenericItem.ButtonValues(new Sport.UI.Controls.ButtonBox.SelectValue(userDialog.ValueSelector));
			// PlayerNumberFrom
			efv = Fields[(int) Sport.Entities.Team.Fields.PlayerNumberFrom];
			efv.Name = "ממספר חולצה";
			efv.Width = 50;
			// PlayerNumberTo
			efv = Fields[(int) Sport.Entities.Team.Fields.PlayerNumberTo];
			efv.Name = "עד מספר חולצה";
			efv.Width = 50;
			// RegisterDate
			efv = Fields[(int) Sport.Entities.Team.Fields.RegisterDate];
			efv.Name = "ת' רישום";
			efv.Width = 120;
			// LastModified
			efv = Fields[(int) Sport.Entities.Team.Fields.LastModified];
			efv.Name = "ת' שינוי אחרון";
			efv.Width = 120;
			// Charge
			efv = Fields[(int) Sport.Entities.Team.Fields.Charge];
			efv.Name = "חיוב";
			efv.Width = 150;
			// Name
			efv = Fields[(int) Sport.Entities.Team.Fields.Name];
			efv.Name = "שם";
			efv.Width = 200;
			// Sport
			fieldSport = Fields[(int) Sport.Entities.Team.Fields.Sport];
			fieldSport.Name = "ענף ספורט";
			fieldSport.Width = 150;
			// SchoolSymbol
			efv = Fields[(int) Sport.Entities.Team.Fields.SchoolSymbol];
			efv.Name = "סמל בית ספר";
			efv.Width = 80;
		}

		public override void OnSelectEntity(Sport.Data.Entity entity)
		{
			if ((entity == null)||(entity.Id < 0))
				return;
			
			Sport.Entities.Team team = new Sport.Entities.Team(entity);
			if (team == null)
				return;
			
			Sport.Entities.Region region = null;
			Sport.Entities.School school = team.School;
			
			if (school == null)
				return;
		
			region = school.Region;
			if (region == null)
				return;
			
			schoolDialog.View.State["school"] = school.Id.ToString();
			if (school.City != null)
				schoolDialog.View.State[Sport.Entities.City.TypeName] = school.City.Id.ToString();
			else
				schoolDialog.View.State[Sport.Entities.City.TypeName] = null;
			schoolDialog.View.State[Sport.Entities.Region.TypeName] = region.Id.ToString();
			
			Sport.Entities.User user = team.Supervisor;
			if (user != null)
			{
				if (user.Region != null)
					userDialog.View.State[Sport.Entities.Region.TypeName] = user.Region.Id.ToString();
				userDialog.View.State[Sport.Entities.School.TypeName] = user.School == null ? null : user.School.Id.ToString();
			}
			else
			{
				userDialog.View.State[Sport.Entities.Region.TypeName] = region.Id.ToString();
				userDialog.View.State[Sport.Entities.School.TypeName] = null;
			}
		}

		private Sport.Entities.ChampionshipCategory SelectChampionshipCategory()
		{
			// need to select here only championships that the school can register
			// to
			return Producer.OpenChampionshipCommand.SelectChampionshipCategory(null, null, null, null);
		}

/*		public override bool New(ParameterList parameters, out bool succeeded)
		{
			succeeded = false;
			string schoolId = parameters["school"];
			if (schoolId != null)
			{
				Sport.Entities.School school = new Sport.Entities.School(Int32.Parse(schoolId));
				Sport.Entities.ChampionshipCategory championshipCategory = SelectChampionshipCategory();
				if (school.IsValid() && championshipCategory != null)
				{
					Sport.Entities.Team team = new Sport.Entities.Team(Sport.Entities.Team.Type.New());
					team.Category = championshipCategory;
					team.Championship = championshipCategory.Championship;
					team.School = school;
					team.Status = Sport.Types.TeamStatusType.Confirmed;
					succeeded = team.Save().Succeeded;
				}
			}
			else
			{
				string champCatId = parameters["championshipcategory"];
				if (champCatId != null)
				{
				}
			}
			return true;
		}*/

	}
}
