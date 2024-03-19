using System;
using Sport.UI;

namespace Sportsman.Views
{
	[Sport.UI.ViewPermissions(Sport.Types.Permission.Championships)]
	public class SchoolTeamsTableView : TableView2
	{
		public SchoolTeamsTableView()
			: base (new Entities.TeamView())
		{
		}

		#region Filters

		private ComboBoxFilter regionFilter;
		private ComboBoxFilter cityFilter;
		private ComboBoxFilter schoolFilter;

		private void CreateFilters()
		{

			// School filter
			Sport.Data.EntityQuery schoolQuery = new Sport.Data.EntityQuery(Sport.Entities.School.Type);
			schoolQuery.Parameters.Add((int) Sport.Entities.School.Fields.City);
			schoolFilter = new ComboBoxFilter("בית ספר:", null, null, "<בחר בית ספר>", 180);
			schoolFilter.ValuesQuery = schoolQuery;
			schoolFilter.Parameters.Add(EntityListView.EntityQuery.Parameters[0]);

			// City filter
			Sport.Data.EntityQuery cityQuery = new Sport.Data.EntityQuery(Sport.Entities.City.Type);
			cityQuery.Parameters.Add((int) Sport.Entities.City.Fields.Region);
			cityFilter = new ComboBoxFilter("ישוב:", null, null, "<בחר ישוב>", 120);
			cityFilter.Parameters.Add(schoolQuery[0]);
			cityFilter.ValuesQuery = cityQuery;

			// Region filter
			regionFilter = new ComboBoxFilter("מחוז:", 
				Sport.Entities.Region.Type.GetEntities(null), Region, "<בחר מחוז>", 120);
			regionFilter.Parameters.Add(cityQuery[0]);

			cityFilter.Value = City;
			schoolFilter.Value = School;

			Filters.Add(regionFilter);
			Filters.Add(cityFilter);
			Filters.Add(schoolFilter);
		}

		#endregion

		public override void Open()
		{
			// Default columns
			Columns = new int[] { 
									(int) Sport.Entities.Team.Fields.Sport, 
									(int) Sport.Entities.Team.Fields.Championship,
									(int) Sport.Entities.Team.Fields.Category, 
									(int) Sport.Entities.Team.Fields.Index, 
									(int) Sport.Entities.Team.Fields.Status, 
									(int) Sport.Entities.Team.Fields.Supervisor 
								};

			// Default sort columns
			Sort = new int[] {
								 (int) Sport.Entities.Team.Fields.Sport, 
								 (int) Sport.Entities.Team.Fields.Championship, 
								 ((int) Sport.Entities.Team.Fields.Index)+Int32.MinValue

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
			EntityListView.EntityQuery.Parameters.Add((int) Sport.Entities.Team.Fields.School);

			//
			// Filters
			//
			CreateFilters();

			base.Open ();
		}

		#region State Properties

		public new Sport.Entities.Region Region
		{
			get
			{
				if (State[Sport.Entities.Region.TypeName] == null)
				{
					Sport.Entities.City city = City;
					if (city != null)
					{
						Sport.Entities.Region region = city.Region;
						State[Sport.Entities.Region.TypeName] = region.Id.ToString();

						return region;
					}

					return null;
				}

				return new Sport.Entities.Region(Int32.Parse(State[Sport.Entities.Region.TypeName]));
			}
			set
			{
				if (value == null)
					State[Sport.Entities.Region.TypeName] = null;
				else
					State[Sport.Entities.Region.TypeName] = value.Id.ToString();
			}
		}

		public Sport.Entities.City City
		{
			get
			{
				if (State[Sport.Entities.City.TypeName] == null)
				{
					Sport.Entities.School school = School;
					if (school != null)
					{
						Sport.Entities.City city = school.City;
						if (city != null)
							State[Sport.Entities.City.TypeName] = city.Id.ToString();
						
						return city;
					}

					return null;
				}

				if (State[Sport.Entities.City.TypeName] == null)
					return null;
				
				return new Sport.Entities.City(Int32.Parse(State[Sport.Entities.City.TypeName]));
			}
			set
			{
				if (value == null)
					State[Sport.Entities.City.TypeName] = null;
				else
					State[Sport.Entities.City.TypeName] = value.Id.ToString();
			}
		}

		public Sport.Entities.School School
		{
			get
			{
				if (State[Sport.Entities.School.TypeName] == null)
					return null;

				return new Sport.Entities.School(Int32.Parse(State[Sport.Entities.School.TypeName]));
			}
			set
			{
				if (value == null)
					State[Sport.Entities.School.TypeName] = null;
				else
					State[Sport.Entities.School.TypeName] = value.Id.ToString();
			}
		}
		
		#endregion
	
	}
}
