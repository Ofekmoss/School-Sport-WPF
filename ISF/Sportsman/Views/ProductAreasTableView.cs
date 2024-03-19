using System;

namespace Sportsman.Views
{
	/// <summary>
	/// Summary description for ProductAreasTableView.
	/// </summary>
	[Sport.UI.ViewPermissions(Sport.Types.Permission.Administration, true)]
	public class ProductAreasTableView : Sport.UI.TableView2
	{
		public ProductAreasTableView()
			: base (new Entities.ProductAreaView())
		{
			DetailsBarEnabled = false;
			FilterBarEnabled = false;
		}

		public override void Open()
		{
			// Default columns
			Columns = new int[] { 
									(int) Sport.Entities.ProductArea.Fields.Name
								};
			base.Open();
		}

		protected override void OnCurrentChanged(Sport.Data.Entity entity)
		{
			CanDelete = entity != null && entity.Id != 1;// Area is not registrations
			base.OnCurrentChanged (entity);
		}
	}
}
