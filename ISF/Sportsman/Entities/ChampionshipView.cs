using System;
using Sport.UI;

namespace Sportsman.Entities
{
	public class ChampionshipView : EntityView
	{
		// Supervisor members
		private EntitySelectionDialog userDialog;

		public ChampionshipView()
			: base (Sport.Entities.Championship.Type)
		{
			//
			// Entity
			//
			Name = "אליפות";
			PluralName = "אליפויות";

			//
			// Fields
			//

			// Id
			EntityFieldView efv = Fields[(int) Sport.Entities.Championship.Fields.Id];
			efv.Name = "קוד";
			efv.Width = 30;
			// Season
			efv = Fields[(int) Sport.Entities.Championship.Fields.Season];
			efv.Name = "עונה";
			efv.Width = 60;
			// Name
			efv = Fields[(int) Sport.Entities.Championship.Fields.Name];
			efv.Name = "שם";
			efv.Width = 200;
			// Region
			efv = Fields[(int) Sport.Entities.Championship.Fields.Region];
			efv.Name = "מחוז";
			efv.Width = 100;
			// Sport
			efv = Fields[(int) Sport.Entities.Championship.Fields.Sport];
			efv.Name = "ענף ספורט";
			efv.Width = 100;
			// IsClubs
			efv = Fields[(int) Sport.Entities.Championship.Fields.IsClubs];
			efv.Name = "אליפות מועדונים";
			efv.Width = 80;
			// Status
			efv = Fields[(int) Sport.Entities.Championship.Fields.Status];
			efv.Name = "סטטוס";
			efv.Width = 100;
			// LastRegistrationDate
			efv = Fields[(int) Sport.Entities.Championship.Fields.LastRegistrationDate];
			efv.Name = "ת' סיום רישום";
			efv.Width = 120;
			// StartDate
			efv = Fields[(int) Sport.Entities.Championship.Fields.StartDate];
			efv.Name = "ת' פתיחה";
			efv.Width = 120;
			// EndDate
			efv = Fields[(int) Sport.Entities.Championship.Fields.EndDate];
			efv.Name = "ת' סיום";
			efv.Width = 120;
			// AltStartDate
			efv = Fields[(int) Sport.Entities.Championship.Fields.AltStartDate];
			efv.Name = "ת' פתיחה חלופי";
			efv.Width = 120;
			// AltEndDate
			efv = Fields[(int) Sport.Entities.Championship.Fields.AltEndDate];
			efv.Name = "ת' סיום חלופי";
			efv.Width = 120;
			// FinalsDate
			efv = Fields[(int) Sport.Entities.Championship.Fields.FinalsDate];
			efv.Name = "ת' גמר";
			efv.Width = 120;
			// AltFinalsDate
			efv = Fields[(int) Sport.Entities.Championship.Fields.AltFinalsDate];
			efv.Name = "ת' גמר חלופי";
			efv.Width = 120;
			// Ruleset
			efv = Fields[(int) Sport.Entities.Championship.Fields.Ruleset];
			efv.Name = "תקנון";
			efv.Width = 200;
			// IsOpen
			efv = Fields[(int) Sport.Entities.Championship.Fields.IsOpen];
			efv.Name = "סוג אליפות";
			efv.Width = 80;
			// Supervisor
			efv = Fields[(int) Sport.Entities.Championship.Fields.Supervisor];
			efv.Name = "אחראי";
			efv.Width = 150;
			userDialog = new Sport.UI.EntitySelectionDialog(new Views.UsersTableView());
			userDialog.View.State[View.SelectionDialog] = "1";
			userDialog.View.State["UserType"] = ((int) Sport.Types.UserType.Internal).ToString();
			efv.GenericItemType = Sport.UI.Controls.GenericItemType.Button;
			efv.Values = Sport.UI.Controls.GenericItem.ButtonValues(new Sport.UI.Controls.ButtonBox.SelectValue(userDialog.ValueSelector));
            // StandardChampionship
			efv = Fields[(int) Sport.Entities.Championship.Fields.StandardChampionship];
			efv.Name = "אליפות קבועה";
			efv.Width = 150;
			// Number
			efv = Fields[(int) Sport.Entities.Championship.Fields.Number];
			efv.Name = "מספר";
			efv.Width = 80;
			// LastModified
			efv = Fields[(int) Sport.Entities.Championship.Fields.LastModified];
			efv.Name = "ת' שינוי אחרון";
			efv.Width = 120;
			// ChampionshipDates
			efv = Fields[(int) Sport.Entities.Championship.Fields.ChampionshipDates];
			efv.Name = "מועדי אליפות";
			efv.Width = 200;
			// FinalsDates
			efv = Fields[(int) Sport.Entities.Championship.Fields.FinalsDates];
			efv.Name = "מועד גמר";
			efv.Width = 150;
		}

		public override void OnSelectEntity(Sport.Data.Entity entity)
		{
			if (entity != null)
			{
				Sport.Entities.Championship champioship = new Sport.Entities.Championship(entity);

				Sport.Entities.Sport sport = champioship.Sport;
				if (sport != null)
				{
					Fields[(int) Sport.Entities.Championship.Fields.Ruleset].Values = sport.GetRulesets();
				}
				else
				{
					Fields[(int) Sport.Entities.Championship.Fields.Ruleset].Values = null;
				}

				Sport.Entities.User user = champioship.Supervisor;
				if (user != null)
				{
					userDialog.View.State[Sport.Entities.Region.TypeName] = user.Region.Id.ToString();
					userDialog.View.State[Sport.Entities.School.TypeName] = user.School == null ? null : user.School.Id.ToString();
				}
				else
				{
					userDialog.View.State[Sport.Entities.Region.TypeName] = Sport.Core.Session.Region.ToString();
					userDialog.View.State[Sport.Entities.School.TypeName] = null;
				}
			}
		}
	}
}
