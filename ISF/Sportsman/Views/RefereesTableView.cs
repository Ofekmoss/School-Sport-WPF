using System;
using System.Collections;
using Sport.UI;
using Sport.Data;
using System.Windows.Forms;
using Sport.UI.Controls;
using Sportsman.Core;

namespace Sportsman.Views
{
	/// <summary>
	/// Summary description for RefereesTableView.
	/// </summary>
	public class RefereesTableView : TableView
	{
		private LookupType lookupRefereeType;
		private ComboBoxFilter typeFilter;
		private EntityFilter filter;
		
		private enum ColumnTitles
		{
			Name=0,
			Type,
			LastModified
		}

		public RefereesTableView()
		{
			Items.Add((int) Sport.Entities.Referee.Fields.Name, "שם השופט", 200);
			Items.Add((int) Sport.Entities.Referee.Fields.Type, "סוג שופט", 150);
			Items.Add((int) Sport.Entities.Referee.Fields.LastModified, "תאריך שינוי רשומה", 200);
		}

		public override void Open()
		{
			//create list view:
			EntityListView = new EntityListView(Sport.Entities.Referee.Type);
			
			//define details and columns fields:
			Columns = new int[] { (int) ColumnTitles.Name, (int) ColumnTitles.Type };
			Details = new int[] { (int) ColumnTitles.LastModified };
			
			//Filters:
			//type filter
			lookupRefereeType = new Sport.Types.RefereeTypeLookup();
			typeFilter = new ComboBoxFilter("סוג שופט:", lookupRefereeType.Items, null, "<כל הסוגים>", 150);
			typeFilter.FilterChanged += new System.EventHandler(RefereeTypeFiltered);
			Filters.Add(typeFilter);
			
			ReloadView();
			base.Open();
		} //end function Open

		/// <summary>
		/// refresh all filters of this view.
		/// </summary>
		private void RefreshFilters()
		{
			//reset global filter:
			filter = new EntityFilter();
			
			//get state values:
			object refType = Core.Tools.GetStateValue(State[Sport.Entities.Referee.TypeName]);
			
			//add filter if some value selected:
			if (refType != null)
			{
				//select only the referees with selected type:
				filter.Add(new EntityFilterField((int) Sport.Entities.Referee.Fields.Type, 
					(int) refType));
				//set filter value as well:
				Core.Tools.SetFilterValue(
					typeFilter, lookupRefereeType[(int) refType]);
			}
		} //end function RefreshFilters
		
		/// <summary>
		/// called when the referee type filter (combo box) is changed.
		/// </summary>
		private void RefereeTypeFiltered(object sender, EventArgs e)
		{
			string identify=Sport.Entities.Referee.TypeName;
			State[identify] = Core.Tools.GetFilterValue(typeFilter.Value);
			ReloadView();
		}

		/// <summary>
		/// called when user click delete button.
		/// return true to perform the delete,
		/// false in order to cancel the delete.
		/// </summary>
		/// <returns></returns>
		protected override bool OnDeleteEntity(Entity entity)
		{
			return Sport.UI.MessageBox.Ask("האם למחוק את השופט '" + entity.Name + "'?", false);
		}

		/// <summary>
		/// build the context menu items
		/// </summary>
		public override MenuItem[] GetContextMenu(Sport.UI.TableView.SelectionType selectionType)
		{
			MenuItem[] menuItems = null;
			
			Entity referee=this.Current;
			if (referee == null)
				return null;
			
			switch (selectionType)
			{
				case (SelectionType.Single):
					menuItems = new MenuItem[2];
					menuItems[0] = new MenuItem("פתח", new System.EventHandler(RefereeDetailsClick));
					menuItems[0].DefaultItem = true;
					menuItems[1] = new MenuItem("-");
					break;
			}
			
			return menuItems;
		}
		
		/// <summary>
		/// called when user add new entity.
		/// </summary>
		protected override void OnNewEntity(EntityEdit entityEdit)
		{
			
		}
		
		/// <summary>
		/// called whenever user selects an  entity in the grid.
		/// </summary>
		protected override void OnSelectEntity(Entity entity)
		{
			
		}

		/// <summary>
		/// calls when user selects Details from context menu or toolbar button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void RefereeDetailsClick(object sender, EventArgs e)
		{
			//open details of selected referee in new dialog.
		}

		/// <summary>
		/// reload the view according to current filters. read from database.
		/// </summary>
		private void Requery()
		{
			Cursor c = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;
			
			//read from database using the global filter:
			EntityListView.Read(filter);

			//define window title:
			Title = "שופטים";

			//add type:
			if (typeFilter.Value != null)
				Title += " "+(typeFilter.Value as LookupItem).Text+"ים";
			
			Cursor.Current = c;
		} //end function Requery
		
		private void ReloadView()
		{
			//refresh all filters and requery the information.
			RefreshFilters();
			Requery();
		}
	} //end class RefereesTableView
}
