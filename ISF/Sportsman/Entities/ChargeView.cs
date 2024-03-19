using System;
using Sport.UI;

namespace Sportsman.Entities
{
	public class ChargeView : EntityView
	{
		private EntitySelectionDialog champDialog;
		
		public ChargeView()
			: base (Sport.Entities.Charge.Type)
		{
			//
			// Entity
			//
			Name = "����";
			PluralName = "������";

			//
			// Fields
			//

			// Id
			EntityFieldView efv = Fields[(int) Sport.Entities.Charge.Fields.Id];
			efv.Name = "���";
			efv.Width = 30;
			// Region
			efv = Fields[(int) Sport.Entities.Charge.Fields.Region];
			efv.Name = "����";
			efv.Width = 150;
			efv.CanEdit = false;
			// Account
			efv = Fields[(int) Sport.Entities.Charge.Fields.Account];
			efv.Name = "�����";
			efv.Width = 180;
			efv.CanEdit = false;
			// Product
			efv = Fields[(int) Sport.Entities.Charge.Fields.Product];
			efv.Name = "��� ����";
			efv.Width = 120;
			efv.Values = Sport.Entities.Product.Type.GetEntities(null);
			// Amount
			efv = Fields[(int) Sport.Entities.Charge.Fields.Amount];
			efv.Name = "����";
			efv.Width = 40;
			// Price
			efv = Fields[(int) Sport.Entities.Charge.Fields.Price];
			efv.Name = "����";
			efv.Width = 60;
			// Championship Category
			efv = Fields[(int) Sport.Entities.Charge.Fields.ChampionshipCategory];
			efv.Name = "������";
			efv.Width = 180;
			champDialog = new Sport.UI.EntitySelectionDialog(
				new Views.ChampionshipCategoryTableView());
			champDialog.View.State[View.SelectionDialog] = "1";
			efv.GenericItemType = Sport.UI.Controls.GenericItemType.Button;
			efv.Values = Sport.UI.Controls.GenericItem.ButtonValues(
				new Sport.UI.Controls.ButtonBox.SelectValue(champDialog.ValueSelector));
			// Date
			efv = Fields[(int) Sport.Entities.Charge.Fields.Date];
			efv.Name = "�����";
			efv.Width = 120;
			// Status
			efv = Fields[(int) Sport.Entities.Charge.Fields.Status];
			efv.Name = "�����";
			efv.Width = 100;
			// Additional
			efv = Fields[(int) Sport.Entities.Charge.Fields.Additional];
			efv.Name = "���� ����";
			efv.Width = 180;
			efv.CanEdit = false;
			// LastModified
			efv = Fields[(int) Sport.Entities.Charge.Fields.LastModified];
			efv.Name = "�' ����� �����";
			efv.Width = 120;
			// PriceTotal
			efv = Fields[(int) Sport.Entities.Charge.Fields.PriceTotal];
			efv.Name = "����";
			efv.Width = 60;
		}
	}
}
