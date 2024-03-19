using System;
using Sport.Data;
using Sport.UI;

namespace Sportsman.Views
{
	/// <summary>
	/// Summary description for SportsTableView.
	/// </summary>
	[Sport.UI.ViewPermissions(Sport.Types.Permission.Sports, true)]
	public class SportFieldsTableView : TableView
	{
		private GridDetailItem gdiSportFields;

		public SportFieldsTableView()
		{
			Items.Add((int) Sport.Entities.SportFieldType.Fields.Sport, "�����", 100);
			Items.Add((int) Sport.Entities.SportFieldType.Fields.Name, "��", 250);
			Items.Add((int) Sport.Entities.SportFieldType.Fields.CompetitionType, "���", 100);

			gdiSportFields = new GridDetailItem("�������:", 
				Sport.Entities.SportField.Type, 
				(int) Sport.Entities.SportField.Fields.SportFieldType, 
				new System.Drawing.Size(400, 120));
			
			gdiSportFields.Columns.Add((int) Sport.Entities.SportField.Fields.Name, "��", 200);
			
			Items.Add("�������", gdiSportFields);
		}

		public override void Open()
		{
			Title = "�������";

			MoreButtonEnabled = false;

			EntityListView = new EntityListView(Sport.Entities.SportFieldType.TypeName);
			EntityListView.Fields[(int) Sport.Entities.SportFieldType.Fields.Sport].Values =
				Sport.Entities.Sport.Type.GetEntities(
				new Sport.Data.EntityFilter((int) Sport.Entities.Sport.Fields.SportType,
				Sport.Types.SportType.Competition));

			Columns = new int[] { 0, 1, 2 };
			Details = new int[] { 3 };

			EntityListView.Read(null);

			base.Open();
		}

		protected override void OnSelectEntity(Entity entity)
		{
			gdiSportFields.Editable = true;
		}


		protected override bool OnDeleteEntity(Entity entity)
		{
			bool result=false;
			if (entity == null)
				return false;
			Sport.Entities.SportFieldType sportFieldType=
				new Sport.Entities.SportFieldType(entity);

			//begin checkings... first check for sport fields:
			Sport.Entities.SportField[] sportFields=sportFieldType.GetSportFields();
			if (sportFields.Length > 0)
			{
				string names="";
				for (int i=0; i<sportFields.Length; i++)
				{
					names += sportFields[i].Name+"\n";
					if (i >= 15)
					{
						names += "...\n";
						break;
					}
				}
				Sport.UI.MessageBox.Show("������ '"+sportFieldType.Name+"' ���� �� ������� ����� �����: \n"+names+
					"�� ����� ������� ���� ���", "����� �����", System.Windows.Forms.MessageBoxIcon.Warning);
				return false;
			}
			
			if (Sport.UI.MessageBox.Ask("��� ����� �� ������ '"+sportFieldType.Name+"'?", false) == true)
				result = true;
			
			return result;
		}

	}
}
