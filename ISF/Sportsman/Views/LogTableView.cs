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
			Items.Add((int) Sport.Entities.Log.Fields.User, "�����", 150);
			Items.Add((int) Sport.Entities.Log.Fields.Version, "����", 80);
			Items.Add((int) Sport.Entities.Log.Fields.Date, "�����", 150);
			Items.Add((int) Sport.Entities.Log.Fields.Description, "�����", 150, 150);
			Items.Add((int) Sport.Entities.Log.Fields.LastModified, "����� �����", 150);
		}

		public override void Open()
		{
			Title = "���� �������";
			EntityListView = new EntityListView(Sport.Entities.Log.TypeName);
			
			Columns = new int[] {(int) ColumnTitles.User, (int) ColumnTitles.Version, 
							(int) ColumnTitles.Date};
			//Details = new int[] {0, 1, 2, 3};
			
			EntityListView.Read(null);
			base.Open();
		}
	} //end class LogTableView
}
