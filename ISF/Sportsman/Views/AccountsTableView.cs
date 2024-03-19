using System;
using System.Windows.Forms;

namespace Sportsman.Views
{
	/// <summary>
	/// Summary description for AccountsTableView.
	/// </summary>
	[Sport.UI.ViewPermissions(Sport.Types.Permission.General)]
	public class AccountsTableView : Sport.UI.TableView2
	{
		//private ToolBarButton tbbSearchBySymbol;

		#region Filters

		private ComboBoxFilter regionFilter;

		private void CreateFilters()
		{
			regionFilter = new ComboBoxFilter("מחוז:", Sport.Entities.Region.Type.GetEntities(null), null, "<בחר מחוז>", 150);
			regionFilter.Parameters.Add(EntityListView.EntityQuery.Parameters[0]);
			regionFilter.FilterChanged += new EventHandler(RegionFiltered);

			Filters.Add(regionFilter);
		}

		private void RegionFiltered(object sender, EventArgs e)
		{
			State[Sport.Entities.Region.TypeName] = regionFilter.Value == null ? null :
				((Sport.Data.Entity) regionFilter.Value).Id.ToString();
		}

		#endregion

		public AccountsTableView()
			: base (new Entities.AccountView())
		{
			DetailsBarEnabled = false;
			
			// search
			SearchBarEnabled = true;

			//
			// toolBar
			//
			//tbbSearchBySymbol = new ToolBarButton();
			//tbbSearchBySymbol.ImageIndex = (int)Sport.Resources.ColorImages.ImportPlayers;
			//tbbSearchBySymbol.Text = "סינון לפי סמל מוסד";
			//tbbSearchBySymbol.Enabled = true;

			//toolBar.Buttons.Add(tbbSearchBySymbol);
			//toolBar.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(ToolBarButtonClicked);
		}

		public override void Open()
		{
			// Default columns
			Columns = new int[] { 
									(int) Sport.Entities.Account.Fields.Name, 
									(int) Sport.Entities.Account.Fields.School, 
									(int) Sport.Entities.Account.Fields.SchoolSymbol, 
									(int) Sport.Entities.Account.Fields.Address
								};
			
			//add search items:
			Searchers.Add(new Searcher("שם חשבון:", 
				EntityListView.EntityType.Fields[(int) Sport.Entities.Account.Fields.Name], 120));
			Searchers.Add(new Searcher("סמל מוסד:",
							EntityListView.EntityType.Fields[(int)Sport.Entities.Account.Fields.SchoolSymbol], 120));
			
			//
			// Query
			//
			EntityListView.EntityQuery.Parameters.Add((int) Sport.Entities.Account.Fields.Region);

			//
			// Filters
			//
			CreateFilters();

			base.Open();
			
			Sport.Entities.Region region = Region;
			if (region != null)
			{
				regionFilter.Value = region.Entity;
				EntityListView.EntityQuery.Parameters[0].Value = region;
			}
		}

		//private void ToolBarButtonClicked(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		//{
		//	if (e.Button == tbbSearchBySymbol)
		//	{
		//		SearchBySymbol();
		//	}
		//}

		//private void SearchBySymbol()
		//{
		//	MessageBox.Show("hello");
		//}

		protected override void NewEntity()
		{
			Sport.Entities.Region region = Region;
			if (region == null)
			{
				Sport.UI.MessageBox.Show("בחר מחוז");
				return ;
			}

			Forms.CreateAccountForm caf = new Forms.CreateAccountForm(region);
			caf.ShowDialog();
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
			set
			{
				if (value == null)
					State[Sport.Entities.Region.TypeName] = null;
				else
					State[Sport.Entities.Region.TypeName] = value.Id.ToString();
			}
		}

		#endregion
	}
}
