using System;

namespace Sportsman.Views
{
	[Sport.UI.ViewPermissions(Sport.Types.Permission.General)]
	public class FacilitiesTableView : Sport.UI.TableView
	{
		#region States

		public Sport.Entities.Region SelectedRegion
		{
			get
			{
				return State[Sport.Entities.Region.TypeName] == null ? null :
					new Sport.Entities.Region((int) Core.Tools.GetStateValue(State[Sport.Entities.Region.TypeName]));
			}
			set
			{
				if (value == null)
				{
					State[Sport.Entities.Region.TypeName] = null;
				}
				else
				{
					State[Sport.Entities.Region.TypeName] = value.Id.ToString();
				}
			}
		}

		public Sport.Entities.School SelectedSchool
		{
			get
			{
				return State[Sport.Entities.School.TypeName] == null ? null :
					new Sport.Entities.School((int) Core.Tools.GetStateValue(State[Sport.Entities.School.TypeName]));
			}
			set
			{
				if (value == null)
				{
					State[Sport.Entities.School.TypeName] = null;
				}
				else
				{
					State[Sport.Entities.School.TypeName] = value.Id.ToString();
				}
			}
		}

		#endregion

		#region Filters

		private ComboBoxFilter regionFilter;
		private ComboBoxFilter schoolFilter;
		private Sport.Data.EntityFilter filter;
		
		private void ResetEntityFilter()
		{
			filter = new Sport.Data.EntityFilter();

			Sport.Entities.School school = SelectedSchool;

			if (school != null)
			{
				filter.Add(new Sport.Data.EntityFilterField((int) Sport.Entities.Facility.Fields.School, school.Id));
			}
			else
			{
				Sport.Entities.Region region = SelectedRegion;
				if (region != null)
				{
					filter.Add(new Sport.Data.EntityFilterField((int) Sport.Entities.Facility.Fields.Region, region.Id));
				}
			}
		}

		private void RefreshSchoolFilter()
		{
			if (SelectedRegion == null)
			{
				schoolFilter.SetValues(null);
			}
			else
			{
				schoolFilter.SetValues(SelectedRegion.GetSchools());
			}
		}

		#endregion

		public FacilitiesTableView()
		{
			Items.Add((int) Sport.Entities.Facility.Fields.Name, "שם", 250);
			Items.Add((int) Sport.Entities.Facility.Fields.Region, "מחוז", 250);
			Items.Add((int) Sport.Entities.Facility.Fields.School, "בית ספר", 250);
			Items.Add((int) Sport.Entities.Facility.Fields.Address, "כתובת", 150);
			Items.Add((int) Sport.Entities.Facility.Fields.Phone, "טלפון", 80);
			Items.Add((int) Sport.Entities.Facility.Fields.Fax, "פקס", 80);
			
			gdiCourts = new GridDetailItem("מגרשים:", 
				Sport.Entities.Court.Type, 
				(int) Sport.Entities.Court.Fields.Facility, 
				new System.Drawing.Size(400, 150));
			
			gdiCourts.Columns.Add((int) Sport.Entities.Court.Fields.Name, "שם", 200);
			gdiCourts.Columns.Add((int) Sport.Entities.Court.Fields.CourtType, "סוג", 200);

			gdiCourts.EntityListView.Fields[(int) Sport.Entities.Court.Fields.CourtType].Values =
				Sport.Entities.CourtType.Type.GetEntities(null);

			Items.Add("מגרשים", gdiCourts);

			Items.Add((int) Sport.Entities.Facility.Fields.LastModified, "תאריך שינוי אחרון", 120);
			Items.Add((int) Sport.Entities.Facility.Fields.City, "יישוב", 120);
			Items.Add((int) Sport.Entities.Facility.Fields.Number, "מספר", 80);
			
			// search
			SearchBarEnabled = true;
		}

		GridDetailItem gdiCourts;

		Sport.UI.EntitySelectionDialog schoolDialog;
		Sport.UI.EntitySelectionDialog cityDialog;

		public override void Open()
		{
			Title = "מתקנים";
			
			MoreButtonEnabled = false;
			
			SchoolsTableView schoolView=new SchoolsTableView();
			schoolView.State[SelectionDialog] = "1";
			schoolDialog = new Sport.UI.EntitySelectionDialog(schoolView);
			
			CitiesTableView cityView=new CitiesTableView();
			cityView.State[SelectionDialog] = "1";
			cityDialog = new Sport.UI.EntitySelectionDialog(cityView);
			
			EntityListView = new Sport.UI.EntityListView(Sport.Entities.Facility.TypeName);

			EntityListView.Fields[(int) Sport.Entities.Facility.Fields.Region].Values =
				Sport.Entities.Region.Type.GetEntities(null);
			
			EntityListView.Fields[(int) Sport.Entities.Facility.Fields.School].GenericItemType =
				Sport.UI.Controls.GenericItemType.Button;
			EntityListView.Fields[(int) Sport.Entities.Facility.Fields.School].Values =
                Sport.UI.Controls.GenericItem.ButtonValues(
				new Sport.UI.Controls.ButtonBox.SelectValue(schoolDialog.ValueSelector));
			
			EntityListView.Fields[(int) Sport.Entities.Facility.Fields.City].GenericItemType =
				Sport.UI.Controls.GenericItemType.Button;
			EntityListView.Fields[(int) Sport.Entities.Facility.Fields.City].Values =
				Sport.UI.Controls.GenericItem.ButtonValues(
				new Sport.UI.Controls.ButtonBox.SelectValue(cityDialog.ValueSelector));
			
			if (State[SelectionDialog] == "1")
				Columns = new int[] { 0, 1, 2, 8 };
			else
				Columns = new int[] { 0, 1, 2, 3, 4, 5, 8 };
			Details = new int[] { 6 };
			
			Searchers.Add(new Searcher("מספר:", EntityListView.EntityType.Fields[(int) Sport.Entities.Facility.Fields.Number], 80));
			Searchers.Add(new Searcher("שם:", EntityListView.EntityType.Fields[(int) Sport.Entities.Facility.Fields.Name], 130));
			
			Sport.Data.Entity[] regions = Sport.Entities.Region.Type.GetEntities(null);
			regionFilter = new ComboBoxFilter("מחוז:", regions, SelectedRegion == null ? null : SelectedRegion.Entity, "<כל המחוזות>");
			regionFilter.FilterChanged += new EventHandler(RegionFiltered); 
			schoolFilter = new ComboBoxFilter("בית ספר:", null, null, "<כל בתי הספר>");
			schoolFilter.FilterChanged += new EventHandler(SchoolFiltered);
			
			Filters.Add(regionFilter);
			Filters.Add(schoolFilter);
			
			RefreshSchoolFilter();
			
			if (State[Sport.Entities.Region.TypeName] != null)
			{
				regionFilter.Value = (new Sport.Entities.Region((int) Core.Tools.GetStateValue(State[Sport.Entities.Region.TypeName]))).Entity;
				if (State[Sport.Entities.School.TypeName] != null)
					schoolFilter.Value = (new Sport.Entities.School(Int32.Parse(State[Sport.Entities.School.TypeName]))).Entity;
			}
			
			Requery();
			
			base.Open();
		}

		protected override bool OnDeleteEntity(Sport.Data.Entity entity)
		{
			//cancel if no entity selected:
			if (entity == null)
				return false;
			
			//build proper entity:
			Sport.Entities.Facility facility=
				new Sport.Entities.Facility(entity);
			
			//can delete?
			string strMessage=facility.CanDelete();
			if (strMessage.Length > 0)
			{
				Sport.UI.MessageBox.Show(strMessage, "מחיקת מתקן", 
					System.Windows.Forms.MessageBoxIcon.Warning);
				return false;
			}
			
			return Sport.UI.MessageBox.Ask("האם למחוק את המתקן '"+facility.Name+"'?", false);
		}


		protected override void OnSelectEntity(Sport.Data.Entity entity)
		{
			if (entity != null)
			{
				SetSchoolDialog(entity);
				gdiCourts.Editable = true;
			}
		}

		protected override void OnValueChange(Sport.Data.EntityEdit entityEdit, Sport.Data.EntityField entityField)
		{
			int index=entityField.Index;
			switch (index)
			{
				case (int) Sport.Entities.Facility.Fields.Region:
				case (int) Sport.Entities.Facility.Fields.School:
				case (int) Sport.Entities.Facility.Fields.City:
					SetSchoolDialog(entityEdit);
					if (index == (int) Sport.Entities.Facility.Fields.School)
						SetCity(entityEdit);
					break;
			}
		}
		
		private void SetCity(Sport.Data.Entity entity)
		{
			Sport.Entities.Facility facility = new Sport.Entities.Facility(entity);
			Sport.Entities.School school=facility.School;
			if (school == null)
			{
				cityDialog.View.State["cancel_region_filter"] = null;
				cityDialog.View.State[Sport.Entities.Region.TypeName] = null;
				cityDialog.View.State[Sport.Entities.City.TypeName] = null;
			}
			else
			{
				//EntityListView.Fields[(int) Sport.Entities.Facility.Fields.City].EntityField.SetValue(entity as Sport.Data.EntityEdit, school.City.Entity);
				facility.City = school.City;
				cityDialog.View.State["cancel_region_filter"] = "1";
				cityDialog.View.State[Sport.Entities.Region.TypeName] = school.Region.Id.ToString();
				cityDialog.View.State[Sport.Entities.City.TypeName] = school.City.Id.ToString();
				string strAddress=Sport.Common.Tools.CStrDef(school.Address, "");
				if (strAddress.Length > 0)
					facility.Address = strAddress;
			}
		}

		private void SetSchoolDialog(Sport.Data.Entity entity)
		{
			Sport.Entities.Facility facility = new Sport.Entities.Facility(entity);
			
			if (facility.Region == null)
			{
				EntityListView.Fields[(int) Sport.Entities.Facility.Fields.School].CanEdit = false;
				EntityListView.Fields[(int) Sport.Entities.Facility.Fields.City].CanEdit = false;
			}
			else
			{
				EntityListView.Fields[(int) Sport.Entities.Facility.Fields.School].CanEdit = true;
				EntityListView.Fields[(int) Sport.Entities.Facility.Fields.City].CanEdit = true;
				int regionID=facility.Region.Id;
				if (regionID == Sport.Entities.Region.CentralRegion)
				{
					schoolDialog.View.State[Sport.Entities.Region.TypeName] = null;
					cityDialog.View.State[Sport.Entities.Region.TypeName] = null;
					schoolDialog.View.State["cancel_region_filter"] = null;
					cityDialog.View.State["cancel_region_filter"] = null;
				}
				else
				{

					schoolDialog.View.State[Sport.Entities.Region.TypeName] = regionID.ToString();
					cityDialog.View.State[Sport.Entities.Region.TypeName] = regionID.ToString();
					schoolDialog.View.State["cancel_region_filter"] = "1";
					cityDialog.View.State["cancel_region_filter"] = "1";
				}
			}
			
			int cityID=-1;
			if (facility.City != null)
				cityID = facility.City.Id;
			if (cityID >= 0)
			{
				schoolDialog.View.State[Sport.Entities.City.TypeName] = cityID.ToString();
				cityDialog.View.State[Sport.Entities.City.TypeName] = cityID.ToString();
				schoolDialog.View.State["cancel_city_filter"] = "1";
			}
			else
			{
				schoolDialog.View.State["cancel_city_filter"] = null;
				schoolDialog.View.State[Sport.Entities.City.TypeName] = null;
				cityDialog.View.State[Sport.Entities.City.TypeName] = null;
			}
			
			if (facility.School != null)
				schoolDialog.View.State[Sport.Entities.School.TypeName] = facility.School.Id.ToString();
		}

		private void RegionFiltered(object sender, EventArgs e)
		{
			if (regionFilter.Value == null)
			{
				SelectedRegion = null;
			}
			else
			{
				SelectedRegion = new Sport.Entities.Region((Sport.Data.Entity)regionFilter.Value);
			}

			RefreshSchoolFilter();
			Requery();
		}

		private void SchoolFiltered(object sender, EventArgs e)
		{
			if (schoolFilter.Value == null)
			{
				SelectedSchool = null;
			}
			else
			{
				SelectedSchool = (Sport.Entities.School) schoolFilter.Value;
			}

			Requery();
		}

		private void Requery()
		{
			//save currrect cursor:
			System.Windows.Forms.Cursor c = System.Windows.Forms.Cursor.Current;
			System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
			
			ResetEntityFilter();

			//read from database or memory:
			if (filter != null)
				EntityListView.Read(filter);
			else
				EntityListView.Clear();
			
			//build title:
			string title;
			if (SelectedSchool != null)
			{
				title = "מתקנים - " + SelectedSchool.Name;
			}
			else if (SelectedRegion != null)
			{
				title = "מתקנים - " + SelectedRegion.Name;
			}
			else
			{
				title = "מתקנים";
			}
			
			//change view title:
			Title = title;
			System.Windows.Forms.Cursor.Current = c;
		}
	}
}
