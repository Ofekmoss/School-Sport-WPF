using System;
using System.Linq;
using System.Collections;
using Sport.UI;
using Sport.Data;
using System.Collections.Generic;

namespace Sportsman.Views
{
	[Sport.UI.ViewPermissions(Sport.Types.Permission.Championships)]
	public class ChampionshipCategoryTableView : TableView2
	{
		public ChampionshipCategoryTableView()
			: base(new Entities.ChampionshipCategoryView())
		{
		}

		#region Filters
		private ComboBoxFilter regionFilter;
		private ComboBoxFilter sportFilter;
		private bool _changedByCode = false;

		private void CreateFilters()
		{
			regionFilter = new ComboBoxFilter("מחוז:", Sport.Entities.Region.Type.GetEntities(null), null, "<כל המחוזות>", 150);
			regionFilter.FilterChanged += new EventHandler(RegionFiltered);
			regionFilter.FilterChanging += new EventHandler(RegionFiltering);

			sportFilter = new ComboBoxFilter("ענף ספורט:", Sport.Entities.Sport.Type.GetEntities(null), null, "<כל ענפי הספורט>", 200);
			sportFilter.FilterChanged += new EventHandler(SportFiltered);
			sportFilter.FilterChanging += new EventHandler(SportFiltering);

			Filters.Add(regionFilter);
			Filters.Add(sportFilter);

			_changedByCode = true;

			regionFilter.Value = Region;
			sportFilter.Value = ChampSport;

			_changedByCode = false;
		}

		private void ApplyFilters()
		{
			if (_changedByCode)
				return;

			ArrayList arrFields = new ArrayList();
			if (regionFilter != null && regionFilter.Value != null)
			{
				Entity entity = (Entity)regionFilter.Value;
				arrFields.Add(new Sport.Entities.ChampionshipRegionFilter(entity.Id));
			}
			if (sportFilter != null && sportFilter.Value != null)
			{
				Entity entity = (Entity)sportFilter.Value;
				arrFields.Add(new Sport.Entities.ChampionshipSportFilter(entity.Id));
			}
			EntityFilter filter = new EntityFilter();
			foreach (EntityFilterField field in arrFields)
			{
				filter.Add(field);
			}
			EntityListView.EntityQuery.BaseFilter = filter;
			//EntityListView.Requery();
		}

		private void RegionFiltering(object sender, EventArgs e)
		{
			ApplyFilters();
		}

		private void RegionFiltered(object sender, EventArgs e)
		{
			//MessageBox.Show("region changed");
			EntityFilter entitityFilter = null;
			if (regionFilter.Value == null)
			{
				State[Sport.Entities.Region.TypeName] = null;
			}
			else
			{
				int selectedRegion = ((Entity)regionFilter.Value).Id;
				State[Sport.Entities.Region.TypeName] = selectedRegion.ToString();
				if (selectedRegion != Sport.Entities.Region.CentralRegion)
					entitityFilter = new EntityFilter(new EntityFilterField((int)Sport.Entities.Sport.Fields.CentralRegionOnly, 1, true));
			}
			sportFilter.SetValues(Sport.Entities.Sport.Type.GetEntities(entitityFilter));
		}

		private void SportFiltering(object sender, EventArgs e)
		{
			ApplyFilters();
		}

		private void SportFiltered(object sender, EventArgs e)
		{
			if (sportFilter.Value == null)
			{
				State[Sport.Entities.Sport.TypeName] = null;
			}
			else
			{
				State[Sport.Entities.Sport.TypeName] = ((Entity)sportFilter.Value).Id.ToString();
			}
		}
		#endregion

		public override void Open()
		{
			// Default columns
			Columns = new int[] { 
									(int) Sport.Entities.ChampionshipCategory.Fields.Championship, 
									(int) Sport.Entities.ChampionshipCategory.Fields.Category
								};

			// Default sort columns
			Sort = new int[] {
								 (int) Sport.Entities.ChampionshipCategory.Fields.Championship, 
								 (int) Sport.Entities.ChampionshipCategory.Fields.Category

							 };

			//DetailsView = typeof(Details.TeamDetailsView);

			//
			// Filters
			//
			CreateFilters();

			//
			// Query
			//
			EntityListView.ListChanged += new EventHandler(EntityListView_ListChanged);
			Sport.Entities.Region region = Region;
			Sport.Entities.Sport sport = ChampSport;
			if (region != null || sport != null)
			{
				EntityListView.EntityQuery.BaseFilter = new EntityFilter();
				if (region != null)
					EntityListView.EntityQuery.BaseFilter.Add(new Sport.Entities.ChampionshipRegionFilter(region.Id));
				if (sport != null)
					EntityListView.EntityQuery.BaseFilter.Add(new Sport.Entities.ChampionshipSportFilter(sport.Id));
				//EntityListView.Requery();
			}

			base.Open();
		}


		void EntityListView_ListChanged(object sender, EventArgs e)
		{
			if (_changedByCode)
				return;
			
			if (regionFilter == null || regionFilter.Value == null)
				return;

			if (EntityListView == null || EntityListView.EntityList == null)
				return;

			int filteredRegion = (regionFilter.Value as Entity).Id;
			Entity[] entities = EntityListView.EntityList.GetListEntities();
			if (entities == null || entities.Length == 0)
				return;

			Dictionary<int, int> champRegions = new Dictionary<int, int>();
			List<Entity> irrelevantCategories = new List<Entity>();
			int currentRegion;
			entities.ToList().ForEach(entity =>
			{
				int currentChampId = (int)entity.Fields[(int)Sport.Entities.ChampionshipCategory.Fields.Championship];
				if (!champRegions.TryGetValue(currentChampId, out currentRegion))
				{
					Sport.Entities.Championship championship = null;
					try
					{
						championship = new Sport.Entities.Championship(currentChampId);
					}
					catch
					{
					}
					if (championship != null && championship.Region != null)
					{
						currentRegion = championship.Region.Id;
						champRegions.Add(currentChampId, currentRegion);
					}
					else
					{
						currentRegion = -1;
					}
				}

				if (currentRegion != filteredRegion)
					irrelevantCategories.Add(entity);

			});

			_changedByCode = true;
			irrelevantCategories.ForEach(catEnt => EntityListView.EntityList.Remove(catEnt));
			_changedByCode = false;
		}

		#region State Properties
		public new Sport.Entities.Region Region
		{
			get
			{
				if (State[Sport.Entities.Region.TypeName] == null)
				{
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

		public Sport.Entities.Sport ChampSport
		{
			get
			{
				if (State[Sport.Entities.Sport.TypeName] == null)
					return null;
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
		#endregion
	}
}
