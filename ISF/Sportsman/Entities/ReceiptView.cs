using System;
using Sport.UI;

namespace Sportsman.Entities
{
	public class ReceiptView : EntityView
	{
		public ReceiptView()
			: base (Sport.Entities.Receipt.Type)
		{
			//
			// Entity
			//
			Name = "����";
			PluralName = "�����";

			//
			// Fields
			//

			// Id
			EntityFieldView efv = Fields[(int) Sport.Entities.Receipt.Fields.Id];
			efv.Name = "���";
			efv.Width = 30;
			// Number
			efv = Fields[(int) Sport.Entities.Receipt.Fields.Number];
			efv.Name = "����";
			efv.Width = 80;
			// Region
			efv = Fields[(int) Sport.Entities.Receipt.Fields.Region];
			efv.Name = "����";
			efv.Width = 150;
			efv.Values = Sport.Entities.Region.Type.GetEntities(null);
			efv.CanEdit = false;
			// Account
			efv = Fields[(int) Sport.Entities.Receipt.Fields.Account];
			efv.Name = "�����";
			efv.Width = 180;
			efv.CanEdit = false;
			// Sum
			efv = Fields[(int) Sport.Entities.Receipt.Fields.Sum];
			efv.Name = "����";
			efv.Width = 120;
			efv.CanEdit = false;
			// Date
			efv = Fields[(int) Sport.Entities.Receipt.Fields.Date];
			efv.Name = "�����";
			efv.Width = 120;
			// Remarks
			efv = Fields[(int) Sport.Entities.Receipt.Fields.Remarks];
			efv.Name = "�����";
			efv.Size = new System.Drawing.Size(400, 75);
			// Season
			efv = Fields[(int) Sport.Entities.Receipt.Fields.Season];
			efv.Name = "����";
			efv.Width = 120;
			efv.Values = Sport.Entities.Season.Type.GetEntities(null);
			// LastModified
			efv = Fields[(int) Sport.Entities.Receipt.Fields.LastModified];
			efv.Name = "�' ����� �����";
			efv.Width = 120;
		}
	}
}
