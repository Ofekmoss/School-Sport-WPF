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
			Items.Add((int) Sport.Entities.Region.Fields.Number, "����", 70);
			Items.Add((int) Sport.Entities.Region.Fields.Name, "��", 150);
			Items.Add((int) Sport.Entities.Region.Fields.Address, "�����", 200);
			Items.Add((int) Sport.Entities.Region.Fields.Phone, "�����", 100);
			Items.Add((int) Sport.Entities.Region.Fields.Fax, "���", 100);
			Items.Add((int) Sport.Entities.Region.Fields.LastModified, "����� ����� �����", 120);
			Items.Add((int) Sport.Entities.Region.Fields.Coordinator, "��� ����", 120);
		}

		public override void Open()
		{
			Title = "������";

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
				Sport.UI.MessageBox.Warn("�� ���� ����� �� ���� ����", "�����");
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
				Sport.UI.MessageBox.Warn("����� '"+region.Name+"' ���� �� ��������� �����: "+
					"\n"+names+"�� ����� �������� ���", "����� ����");
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
				Sport.UI.MessageBox.Warn("����� '"+region.Name+"' ���� �� �������� �����: "+
					"\n"+names+"�� ����� ������� ���", "����� ����");
				return false;
			}
			
			//check for schools:
			int schoolsCount=region.GetSchoolsCount();
			if (schoolsCount > 0)
			{
				Sport.UI.MessageBox.Warn("����� '"+region.Name+"' ���� "+
					schoolsCount.ToString()+" ��� ���\n�� ����� �� ��� ����", "����� ����");
				return false;
			}
			
			//check equipments:
			if (region.GetEquipments().Length > 0)
			{
				Sport.UI.MessageBox.Warn("����� '"+region.Name+"' ����� ���� ������ ����\n"+
					schoolsCount.ToString()+"�� ����� �� �������", "����� ����");
				return false;
			}
			
			//check users:
			int usersCount=region.GetUsers().Length;
			if (usersCount > 0)
			{
				Sport.UI.MessageBox.Warn("������� "+usersCount+" ������� ����� ��, �� ���� �����", "����� ����");
				return false;
			}
			
			return Sport.UI.MessageBox.Ask("��� ����� �� ����� '" + entity.Name + "'?", false);
		}
	}
}
