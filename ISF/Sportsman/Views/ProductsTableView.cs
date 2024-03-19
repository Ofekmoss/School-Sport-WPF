using System;
using Sport.UI;
using Sport.UI.Display;
using Sport.Data;

namespace Sportsman.Views
{
	/// <summary>
	/// Summary description for ProductsTableView.
	/// </summary>
	[Sport.UI.ViewPermissions(Sport.Types.Permission.Accounts, true)]
	public class ProductsTableView : TableView2
	{
		public ProductsTableView()
			: base (new Entities.ProductView())
		{
			DetailsBarEnabled = false;
			FilterBarEnabled = false;
		}

		public override void Open()
		{
			// Default columns
			Columns = new int[] { 
									(int) Sport.Entities.Product.Fields.Name, 
									(int) Sport.Entities.Product.Fields.Area,
									(int) Sport.Entities.Product.Fields.Price
								};
			base.Open();
		}

		protected override void OnCurrentChanged(Entity entity)
		{
			CanDelete = entity != null && entity.Id > 3; // Product is not registration
			// of a team a player or a club
			base.OnCurrentChanged (entity);
		}

	} //end class ProductsTableView
}
