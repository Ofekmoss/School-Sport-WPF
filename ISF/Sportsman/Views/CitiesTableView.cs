using System;
using Sport.UI;
using Sport.UI.Display;
using Sport.Data;
using System.Windows.Forms;

namespace Sportsman.Views
{
	/// <summary>
	/// Summary description for CitiesTableView.
	/// </summary>
	[Sport.UI.ViewPermissions(Sport.Types.Permission.General)]
	public class CitiesTableView : TableView
	{
		ComboBoxFilter				regionFilter;

		public CitiesTableView()
		{
			DetailsBarEnabled = false;
			Items.Add((int) Sport.Entities.City.Fields.Region, "מחוז", 150);
			Items.Add((int) Sport.Entities.City.Fields.Name, "שם", 200);
			Items.Add((int) Sport.Entities.City.Fields.LastModified, "תאריך שינוי אחרון", 120);
		}

		public override void Open()
		{
			Title = "ישובים";

            EntityListView = new EntityListView(Sport.Entities.City.TypeName);

			EntityListView.Fields[(int) Sport.Entities.City.Fields.Region].Values =
				Sport.Entities.Region.Type.GetEntities(null);

			Columns = new int[] { 0, 1 };

			Entity[] regions = Sport.Entities.Region.Type.GetEntities(null);
			regionFilter = new ComboBoxFilter("מחוז:", regions, null, "<כל המחוזות>");
			regionFilter.FilterChanged += new System.EventHandler(RegionFiltered);
			if (State["cancel_region_filter"] == null)
			{
				Filters.Add(regionFilter);
				regionFilter.Value = new Sport.Entities.Region(
					Core.UserManager.CurrentUser.UserRegion).Entity;
				RegionFiltered(null, EventArgs.Empty);
			}

			Requery();

			base.Open();
		}

		private void RegionFiltered(object sender, EventArgs e)
		{
			if (regionFilter.Value == null)
			{
				State[Sport.Entities.Region.TypeName] = null;
			}
			else
			{
				State[Sport.Entities.Region.TypeName] = ((Entity)regionFilter.Value).Id.ToString();
			}
			
			Requery();
		}

		// If the city region change, the schools and facilities
		// region also change, so need the reset entity type
		private bool regionChanged = false;
		protected override void OnValueChange(EntityEdit entityEdit, EntityField entityField)
		{
			if (entityField.Index == (int) Sport.Entities.City.Fields.Region)
			{
				regionChanged = entityEdit.Entity != null &&
					!entityField.GetValue(entityEdit).Equals(
						entityField.GetValue(entityEdit.Entity));
			}

			base.OnValueChange (entityEdit, entityField);
		}

		protected override void OnSaveEntity(Entity entity)
		{
			if (regionChanged)
			{
				Sport.Entities.School.Type.Reset(
					new Sport.Data.EntityFilter(
						(int) Sport.Entities.School.Fields.City, entity.Id));
				Sport.Entities.Facility.Type.Reset(
					new Sport.Data.EntityFilter(
						(int) Sport.Entities.Facility.Fields.City, entity.Id));

				regionChanged = false;
			}

			base.OnSaveEntity (entity);
		}

		protected override void OnCancelEntity(Entity entity)
		{
			regionChanged = false;
			base.OnCancelEntity (entity);
		}


		protected override void OnNewEntity(EntityEdit entityEdit)
		{
			if (regionFilter.Value != null)
			{
				EntityField entityField = EntityListView.EntityType.Fields[(int) Sport.Entities.City.Fields.Region];
				entityField.SetValue(EntityListView.EntityEdit, ((Entity) regionFilter.Value));
			}
			base.OnNewEntity (entityEdit);
		}
	
		protected override bool OnDeleteEntity(Entity entity)
		{
			if (entity == null)
				return false;
			Sport.Entities.City city=new Sport.Entities.City(entity);
			
			//begin checkings... first check if the city contains any school:
			EntityFilter filter=new EntityFilter((int) Sport.Entities.School.Fields.City, city.Id);
			Entity[] schools=Sport.Entities.School.Type.GetEntities(filter);
			if (schools.Length > 0)
			{
				string schoolNames="";
				for (int i=0; i<schools.Length; i++)
				{
					Sport.Entities.School school=new Sport.Entities.School(schools[i]);
					if (school.Name != null)
						schoolNames += school.Name+"\n";
					if (i >= 15)
					{
						schoolNames += (schools.Length-15)+" בתי ספר נוספים...\n";
						break;
					}
				}
				Sport.UI.MessageBox.Show("היישוב '"+city.Name+"' כולל את בתי הספר הבאים: "+
					"\n"+schoolNames+"יש לשנות את היישוב עבור בתי ספר אלו", 
					"מחיקת יישוב", MessageBoxIcon.Warning);
				return false;
			}
			
			return Sport.UI.MessageBox.Ask("האם למחוק את היישוב '"+city.Name+
				"'?", "מחיקת יישוב", false);
		}


		private void Requery()
		{
			EntityFilter filter = null;

			object region = Core.Tools.GetStateValue(State[Sport.Entities.Region.TypeName]);
			
			string title="יישובים";
			if (region != null)
			{
				int regionID=(int) region;
				title += " - "+(new Sport.Entities.Region(regionID)).Name;
				filter = new EntityFilter();
				filter.Add(new EntityFilterField((int) Sport.Entities.City.Fields.Region,
					regionID));
			}
			this.Title = title;

			EntityListView.Read(filter);
			//grid.Sort(new int[] { (int) Sport.Entities.City.Fields.Region, (int) Sport.Entities.City.Fields.Name });
		}
	}
}
