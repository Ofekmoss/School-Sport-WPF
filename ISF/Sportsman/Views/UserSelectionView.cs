using System;

namespace Sportsman.Views
{
	public class UserSelectionView : Sport.UI.TableView2
	{
		public UserSelectionView()
			: base (new Entities.UserView())
		{
		}

		#region Filters

		private ComboBoxFilter regionFilter;
		private ComboBoxFilter schoolFilter;

		#endregion

		public override void Open()
		{
			// Default columns
			Columns = new int[] { 
									(int) Sport.Entities.User.Fields.FirstName,
									(int) Sport.Entities.User.Fields.LastName,
									(int) Sport.Entities.User.Fields.School,
									(int) Sport.Entities.User.Fields.UserType
								};

			// Default sort columns
			Sort = new int[] {
								 (int) Sport.Entities.User.Fields.FirstName,
								 (int) Sport.Entities.User.Fields.LastName

							 };
            
			//DetailsView = typeof(Details.TeamDetailsView);


			//
			// Query
			//

			EntityListView.EntityQuery.Parameters.Add((int) Sport.Entities.User.Fields.Region);
			EntityListView.EntityQuery.Parameters.Add((int) Sport.Entities.User.Fields.School, false);

			if (UserType != null)
			{
				EntityListView.EntityQuery.Parameters.Add((int) Sport.Entities.User.Fields.UserType, UserType);
			}

			//
			// Filters
			//

			// School filter
			Sport.Data.EntityQuery schoolQuery = new Sport.Data.EntityQuery(Sport.Entities.School.Type);
			schoolQuery.Parameters.Add((int) Sport.Entities.School.Fields.Region);
			schoolFilter = new ComboBoxFilter("בית ספר:", null, null, "<בחר בית ספר>", 180);
			schoolFilter.ValuesQuery = schoolQuery;
			schoolFilter.Parameters.Add(EntityListView.EntityQuery.Parameters[1]);

			// Region filter
			regionFilter = new ComboBoxFilter("מחוז:", 
				Sport.Entities.Region.Type.GetEntities(null), Region, "<בחר מחוז>", 120);
			regionFilter.Parameters.Add(schoolQuery[0]);
			regionFilter.FilterChanged += new EventHandler(regionFilter_FilterChanged);

			schoolFilter.Value = School;

			Filters.Add(regionFilter);
			Filters.Add(schoolFilter);

			base.Open ();
		}

		#region State Properties

		public new Sport.Entities.Region Region
		{
			get
			{
				if (State[Sport.Entities.Region.TypeName] == null)
					return null;

				return new Sport.Entities.Region(Int32.Parse(State[Sport.Entities.Region.TypeName]));
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
		}

		public object UserType
		{
			get 
			{ 
				if (State["UserType"] == null)
					return null;

				return Int32.Parse(State["UserType"]);
			}
		}
		
		#endregion

		private void regionFilter_FilterChanged(object sender, EventArgs e)
		{
			 EntityListView.EntityQuery.Parameters[0].Value = regionFilter.Value;
		}
	}
}
