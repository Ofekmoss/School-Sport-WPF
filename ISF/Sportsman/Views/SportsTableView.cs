using System;
using Sport.UI;
using Sport.UI.Display;
using Sport.Data;

namespace Sportsman.Views
{
	/// <summary>
	/// Summary description for SportsTableView.
	/// </summary>
	[Sport.UI.ViewPermissions(Sport.Types.Permission.Sports, true)]
	public class SportsTableView : TableView
	{
		public SportsTableView()
		{
			DetailsBarEnabled = false;
			FilterBarEnabled = false;
			Items.Add((int)Sport.Entities.Sport.Fields.Name, "��", 150);
			Items.Add((int)Sport.Entities.Sport.Fields.SportType, "���", 100);
			Items.Add((int)Sport.Entities.Sport.Fields.Ruleset, "�����", 100);
			Items.Add((int)Sport.Entities.Sport.Fields.LastModified, "����� ����� �����", 120);
			Items.Add((int)Sport.Entities.Sport.Fields.PointsName, "����� �����", 120);
			Items.Add((int)Sport.Entities.Sport.Fields.CentralRegionOnly, "��� ����?", 100);
		}

		public override void Open()
		{
			Title = "���� �����";

			EntityListView = new EntityListView(Sport.Entities.Sport.TypeName);

			Columns = new int[] { 0, 1, 2 };

			EntityListView.Read(null);

			base.Open();
		}

		protected override bool OnDeleteEntity(Entity entity)
		{
			//cancel if no entity selected:
			if (entity == null)
				return false;

			//build proper entity:
			Sport.Entities.Sport sport =
				new Sport.Entities.Sport(entity);

			//begin checkings... first check for championships:
			Sport.Entities.Championship[] champs = sport.GetChampionships();
			if (champs.Length > 0)
			{
				string names = "";
				for (int i = 0; i < champs.Length; i++)
				{
					names += champs[i].Name + "\n";
					if (i >= 15)
					{
						names += "...\n";
						break;
					}
				}
				Sport.UI.MessageBox.Warn("��� ������ '" + sport.Name + "' ���� �� ��������� �����: \n" +
					names + "�� ����� �������� ���", "����� ��� �����");
				return false;
			}

			return Sport.UI.MessageBox.Ask("��� ����� �� ��� ������ '" + sport.Name + "'?", false);
		}


		protected override void OnSelectEntity(Entity entity)
		{
			if (entity != null)
			{
				EntityListView.Fields[(int)Sport.Entities.Sport.Fields.Ruleset].Values =
					Sport.Entities.Ruleset.Type.GetEntities(
					new EntityFilter((int)Sport.Entities.Ruleset.Fields.Sport,
					entity.Id));
			}
		}
	}
}
