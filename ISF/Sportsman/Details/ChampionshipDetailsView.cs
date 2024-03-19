using System;

namespace Sportsman.Details
{
	/// <summary>
	/// Inherits DetailView for schools details
	/// </summary>
	[Sport.UI.ViewPermissions(Sport.Types.Permission.Championships)]
	public class ChampionshipDetailsView : Sport.UI.DetailsView
	{
		public ChampionshipDetailsView()
			: base (new Entities.ChampionshipView())
		{
		}

		public override void Open()
		{
			Searchers.Add(new Searcher("מספר:", EntityView.EntityType.Fields[(int) Sport.Entities.Championship.Fields.Number], 100));
			
			FieldsPage generalPage = new FieldsPage("כללי",
				new int[] {
							(int) Sport.Entities.Championship.Fields.Number,
							(int) Sport.Entities.Championship.Fields.Region,
							(int) Sport.Entities.Championship.Fields.Sport,
							(int) Sport.Entities.Championship.Fields.Name,
							(int) Sport.Entities.Championship.Fields.IsClubs,
							(int) Sport.Entities.Championship.Fields.Status,
							(int) Sport.Entities.Championship.Fields.IsOpen,
							(int) Sport.Entities.Championship.Fields.Ruleset,
							(int) Sport.Entities.Championship.Fields.Supervisor,
							(int) Sport.Entities.Championship.Fields.Season,
							(int) Sport.Entities.Championship.Fields.LastModified
						  });

			Pages.Add(generalPage);

			FieldsPage datesPage = new FieldsPage("מועדים",
				new int[] {
							(int) Sport.Entities.Championship.Fields.LastRegistrationDate,
							(int) Sport.Entities.Championship.Fields.StartDate,
							(int) Sport.Entities.Championship.Fields.EndDate,
							(int) Sport.Entities.Championship.Fields.AltStartDate,
							(int) Sport.Entities.Championship.Fields.AltEndDate,
							(int) Sport.Entities.Championship.Fields.FinalsDate,
							(int) Sport.Entities.Championship.Fields.AltFinalsDate
						  });

			Pages.Add(datesPage);

			base.Open();
		}
	}
}
