using System;
using Sport.UI;
using Sportsman.Details;

namespace Sportsman.Views
{
	/// <summary>
	/// Summary description for PracticeCampsView.
	/// </summary>
	[Sport.UI.ViewPermissions(Sport.Types.Permission.General)]
	public class PracticeCampsView : TableView2
	{
		public PracticeCampsView()
			: base (new Entities.PracticeCampView())
		{
			//constructor code here.
		}
		
		#region Filters
		private ComboBoxFilter sportFilter;
		
		private void CreateFilters()
		{
			sportFilter = new ComboBoxFilter("ענף ספורט:", Sport.Entities.Sport.Type.GetEntities(null), null, "<כל ענפי הספורט>", 200);
			sportFilter.Parameters.Add(EntityListView.EntityQuery.Parameters[0]);
			sportFilter.FilterChanged += new EventHandler(SportFiltered);
			
			Filters.Add(sportFilter);
		}
		
		private void SportFiltered(object sender, EventArgs e)
		{
			if (sportFilter.Value == null)
			{
				State["sport"] = null;
			}
			else
			{
				State["sport"] = ((Sport.Data.Entity) sportFilter.Value).Id.ToString();
			}
		}
		#endregion
		
		public override void Open()
		{
			// Default columns
			Columns = new int[] { 
									(int) Sport.Entities.PracticeCamp.Fields.Sport,
									(int) Sport.Entities.PracticeCamp.Fields.DateStart,
									(int) Sport.Entities.PracticeCamp.Fields.DateFinish,
									(int) Sport.Entities.PracticeCamp.Fields.BasePrice,
									(int) Sport.Entities.PracticeCamp.Fields.Remarks
								};

			// Default sort columns
			if (this.State[Sport.UI.View.SelectionDialog] == "1")
			{
				Sort = new int[] { Int32.MinValue + (int)Sport.Entities.PracticeCamp.Fields.DateStart };
			}
			else
			{
				Sort = new int[] { (int) Sport.Entities.PracticeCamp.Fields.Sport };
			}
			

			//
			// Query
			//
			EntityListView.EntityQuery.Parameters.Add((int) Sport.Entities.PracticeCamp.Fields.Sport, false);
			
			//
			// Filters
			//
			CreateFilters();
			
			Sport.Entities.Sport sport = this.CampSport;
			if (sport != null)
			{
				sportFilter.Value = sport;
				EntityListView.EntityQuery.Parameters[0].Value = sport;
			}
			
			base.Open ();
		}

		#region State Properties
		public Sport.Entities.Sport CampSport
		{
			get
			{
				if (State["sport"] == null)
					return null;
				return new Sport.Entities.Sport(Int32.Parse(State["sport"]));
			}
			set
			{
				if (value == null)
					State["sport"] = null;
				else
					State["sport"] = value.Id.ToString();
			}
		}
		#endregion
		
		#region overriden methods
		public override void OnContextMenu(Sport.UI.TableView2.SelectionType selectionType, Sport.UI.Controls.RightContextMenu menu)
		{
			base.OnContextMenu (selectionType, menu);
			
			if (selectionType == Sport.UI.TableView2.SelectionType.Single)
			{
				if (this.Current != null && this.Current.Id > 0)
					menu.MenuItems.Add(new System.Windows.Forms.MenuItem("משתתפים", new System.EventHandler(PracticeCampClicked)));
				
			}
		}
		#endregion

		#region handlers
		private void PracticeCampClicked(object sender, EventArgs e)
		{
			Sport.Data.Entity entity = Current;
			if (entity != null)
			{
				//build the state string and open the view:
				string state = Sport.Entities.PracticeCamp.TypeName + "=" + entity.Id;
				ViewManager.OpenView(typeof(Views.PracticeCampParticipantsView), state);
			}
		}
		#endregion
	}
}
