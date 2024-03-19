using System;
using Sport.UI;

namespace Sportsman.Views
{
	[Sport.UI.ViewPermissions(Sport.Types.Permission.Championships)]
	public class ChampionshipTeamsTableView : TableView2
	{
		public ChampionshipTeamsTableView()
			: base(new Entities.TeamView())
		{
		}

		#region Filters

		private ComboBoxFilter sportFilter;
		private ComboBoxFilter championshipFilter;
		private ComboBoxFilter categoryFilter;

		private void CreateFilters()
		{

			// Category filter
			Sport.Data.EntityQuery categoryQuery = new Sport.Data.EntityQuery(Sport.Entities.ChampionshipCategory.Type);
			categoryQuery.Parameters.Add((int) Sport.Entities.ChampionshipCategory.Fields.Championship);
			categoryFilter = new ComboBoxFilter("קטגוריה:", null, null, "<בחר קטגוריה>", 180);
			categoryFilter.ValuesQuery = categoryQuery;
			categoryFilter.Parameters.Add(EntityListView.EntityQuery.Parameters[1]);

			// Championship filter
			Sport.Data.EntityQuery championshipQuery = new Sport.Data.EntityQuery(Sport.Entities.Championship.Type);
			championshipQuery.Parameters.Add((int) Sport.Entities.Championship.Fields.Sport);
			championshipFilter = new ComboBoxFilter("אליפות:", null, null, "<בחר אליפות>", 180);
			championshipFilter.Parameters.Add(categoryQuery[0]);
			championshipFilter.Parameters.Add(EntityListView.EntityQuery.Parameters[0]);
			championshipFilter.ValuesQuery = championshipQuery;

			// Sport filter
			sportFilter = new ComboBoxFilter("ענף ספורט:", 
				Sport.Entities.Sport.Type.GetEntities(null), ChampionshipSport, "<בחר ענף ספורט>", 180);
			sportFilter.Parameters.Add(championshipQuery[0]);

			championshipFilter.Value = Championship;
			categoryFilter.Value = Category;

			Filters.Add(sportFilter);
			Filters.Add(championshipFilter);
			Filters.Add(categoryFilter);
		}

		#endregion

		public override void Open()
		{
			// Default columns
			Columns = new int[] { 
									(int) Sport.Entities.Team.Fields.School,
									(int) Sport.Entities.Team.Fields.SchoolSymbol,
									(int) Sport.Entities.Team.Fields.Index,
									(int) Sport.Entities.Team.Fields.Sport, 
									(int) Sport.Entities.Team.Fields.Category, 
									(int) Sport.Entities.Team.Fields.Status, 
									(int) Sport.Entities.Team.Fields.Supervisor 
								};

			// Default sort columns
			Sort = new int[] {
								 (int) Sport.Entities.Team.Fields.Category, 
								 (int) Sport.Entities.Team.Fields.School,
								 (int) Sport.Entities.Team.Fields.Index

							 };
            
			// Default details
			Details = new int[] { 
									(int) Sport.Entities.Team.Fields.Status, 
									(int) Sport.Entities.Team.Fields.Supervisor, 
									(int) Sport.Entities.Team.Fields.RegisterDate, 
									(int) Sport.Entities.Team.Fields.PlayerNumberFrom,
									(int) Sport.Entities.Team.Fields.PlayerNumberTo 
								};

			DetailsView = typeof(Details.TeamDetailsView);

			//
			// Query
			//
			EntityListView.EntityQuery.Parameters.Add((int) Sport.Entities.Team.Fields.Championship);
			EntityListView.EntityQuery.Parameters.Add((int) Sport.Entities.Team.Fields.Category, false);

			//
			// Filters
			//
			CreateFilters();

			base.Open ();
		}

		#region State Properties

		public Sport.Entities.Sport ChampionshipSport
		{
			get
			{
				if (State[Sport.Entities.Sport.TypeName] == null)
				{
					Sport.Entities.Championship championship = Championship;
					if (championship != null)
					{
						Sport.Entities.Sport sport = championship.Sport;
						State[Sport.Entities.Sport.TypeName] = sport.Id.ToString();

						return sport;
					}

					return null;
				}

				return new Sport.Entities.Sport(Int32.Parse(State[Sport.Entities.Sport.TypeName]));
			}
			set
			{
				if (value == null)
					State[Sport.Entities.Sport.TypeName] = null;
				else
					State[Sport.Entities.Sport.TypeName] = value.Id.ToString();
			}
		}

		public Sport.Entities.Championship Championship
		{
			get
			{
				if (State[Sport.Entities.Championship.TypeName] == null)
				{
					Sport.Entities.ChampionshipCategory category = Category;
					if (category != null)
					{
						Sport.Entities.Championship championship = category.Championship;
						State[Sport.Entities.Championship.TypeName] = championship.Id.ToString();

						return championship;
					}

					return null;
				}

				return new Sport.Entities.Championship(Int32.Parse(State[Sport.Entities.Championship.TypeName]));
			}
			set
			{
				if (value == null)
					State[Sport.Entities.Championship.TypeName] = null;
				else
					State[Sport.Entities.Championship.TypeName] = value.Id.ToString();
			}
		}

		public Sport.Entities.ChampionshipCategory Category
		{
			get
			{
				if (State[Sport.Entities.ChampionshipCategory.TypeName] == null)
					return null;

				return new Sport.Entities.ChampionshipCategory(Int32.Parse(State[Sport.Entities.ChampionshipCategory.TypeName]));
			}
			set
			{
				if (value == null)
					State[Sport.Entities.ChampionshipCategory.TypeName] = null;
				else
					State[Sport.Entities.ChampionshipCategory.TypeName] = value.Id.ToString();
			}
		}
		
		#endregion
	}
}
