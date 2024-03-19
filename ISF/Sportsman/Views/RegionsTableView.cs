using System;
using System.Windows.Forms;
using Sport.Data;
using Sport.UI.Display;
using Sport.UI;
using Sport.UI.Controls;

namespace Sportsman.Views
{
	[Sport.UI.ViewPermissions(Sport.Types.Permission.Administration)]
	public class RegionsTableView : Sport.UI.TableView
	{
		EntitySelectionDialog userDialog;

		public RegionsTableView()
		{
			FilterBarEnabled = false;
			Items.Add((int) Sport.Entities.Region.Fields.Number, "מספר", 70);
			Items.Add((int) Sport.Entities.Region.Fields.Name, "שם", 150);
			Items.Add((int) Sport.Entities.Region.Fields.Address, "כתובת", 200);
			Items.Add((int) Sport.Entities.Region.Fields.Phone, "טלפון", 100);
			Items.Add((int) Sport.Entities.Region.Fields.Fax, "פקס", 100);
			Items.Add((int) Sport.Entities.Region.Fields.LastModified, "תאריך שינוי אחרון", 120);
			Items.Add((int) Sport.Entities.Region.Fields.Coordinator, "רכז מחוז", 120);
		}

		public override void Open()
		{
			Title = "מחוזות";

			EntityListView = new EntityListView(Sport.Entities.Region.TypeName);

			UsersTableView tempUsersTableView = new UsersTableView();
			tempUsersTableView.FilterBarEnabled = false;
			userDialog = new EntitySelectionDialog(tempUsersTableView);
			userDialog.View.State[SelectionDialog] = "1";
			userDialog.View.State["ParentView"] = "Regions";

			Columns = new int[] {0, 1, 2, 3, 5};
			Details = new int[] {0, 1, 2, 3, 5};
			
			Fields[(int) Sport.Entities.Region.Fields.Coordinator].GenericItemType = Sport.UI.Controls.GenericItemType.Button;
			Fields[(int) Sport.Entities.Region.Fields.Coordinator].Values = Sport.UI.Controls.GenericItem.ButtonValues(new ButtonBox.SelectValue(userDialog.ValueSelector));

			EntityListView.Read(null);

			base.Open();
		}

		protected override void OnSelectEntity(Entity entity)
		{
			if (entity != null)
			{
				userDialog.View.State[Sport.Entities.Region.TypeName] = entity.Id.ToString();			
			}
		}

		protected override bool OnDeleteEntity(Entity entity)
		{
			if (entity == null)
				return false;
			Sport.Entities.Region region=new Sport.Entities.Region(entity);
			
			if (region.Id == Sport.Entities.Region.CentralRegion)
			{
				Sport.UI.MessageBox.Warn("לא ניתן למחוק את מחוז המטה", "שגיאה");
				return false;
			}
			
			//begin checkings... first check championships:
			Sport.Entities.Championship[] champs=region.GetChampionships();
			if (champs.Length > 0)
			{
				string names="";
				for (int i=0; i<champs.Length; i++)
				{
					names += champs[i].Name+"\n";
					if (i > 15)
					{
						names += "...\n";
						break;
					}
				}
				Sport.UI.MessageBox.Warn("המחוז '"+region.Name+"' כולל את האליפויות הבאות: "+
					"\n"+names+"יש להסיר אליפויות אלו", "מחיקת מחוז");
				return false;
			}
			
			//check for cities:
			Sport.Entities.City[] cities=region.GetCities();
			if (cities.Length > 0)
			{
				string names="";
				for (int i=0; i<cities.Length; i++)
				{
					names += cities[i].Name+"\n";
					if (i > 15)
					{
						names += "...\n";
						break;
					}
				}
				Sport.UI.MessageBox.Warn("המחוז '"+region.Name+"' כולל את היישובים הבאים: "+
					"\n"+names+"יש להסיר יישובים אלו", "מחיקת מחוז");
				return false;
			}
			
			//check for schools:
			int schoolsCount=region.GetSchoolsCount();
			if (schoolsCount > 0)
			{
				Sport.UI.MessageBox.Warn("המחוז '"+region.Name+"' כולל "+
					schoolsCount.ToString()+" בתי ספר\nיש להסיר את בתי הספר", "מחיקת מחוז");
				return false;
			}
			
			//check equipments:
			if (region.GetEquipments().Length > 0)
			{
				Sport.UI.MessageBox.Warn("המחוז '"+region.Name+"' מוגדר עבור הזמנות ציוד\n"+
					schoolsCount.ToString()+"יש להסיר את ההזמנות", "מחיקת מחוז");
				return false;
			}
			
			//check users:
			int usersCount=region.GetUsers().Length;
			if (usersCount > 0)
			{
				Sport.UI.MessageBox.Warn("מוגדרים "+usersCount+" משתמשים במחוז זה, לא ניתן למחוק", "מחיקת מחוז");
				return false;
			}
			
			return Sport.UI.MessageBox.Ask("האם למחוק את המחוז '" + entity.Name + "'?", false);
		}
	}
}
