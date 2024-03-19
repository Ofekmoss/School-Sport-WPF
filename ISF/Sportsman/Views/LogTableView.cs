using System;
using System.Windows.Forms;
using Sport.Data;
using Sport.UI.Display;
using Sport.UI;

namespace Sportsman.Views
{
	public class LogTableView : Sport.UI.TableView
	{
		private enum ColumnTitles
		{
			User=0,
			Version,
			Date,
			Description,
			LastModified
		}
		
		public LogTableView()
		{
			FilterBarEnabled = false;
			this.CanInsert = false;
			Items.Add((int) Sport.Entities.Log.Fields.User, "משתמש", 150);
			Items.Add((int) Sport.Entities.Log.Fields.Version, "גרסה", 80);
			Items.Add((int) Sport.Entities.Log.Fields.Date, "תאריך", 150);
			Items.Add((int) Sport.Entities.Log.Fields.Description, "הערות", 150, 150);
			Items.Add((int) Sport.Entities.Log.Fields.LastModified, "שינוי אחרון", 150);
		}

		public override void Open()
		{
			Title = "מעקב משתמשים";
			EntityListView = new EntityListView(Sport.Entities.Log.TypeName);
			
			Columns = new int[] {(int) ColumnTitles.User, (int) ColumnTitles.Version, 
							(int) ColumnTitles.Date};
			//Details = new int[] {0, 1, 2, 3};
			
			EntityListView.Read(null);
			base.Open();
		}
	} //end class LogTableView
}
