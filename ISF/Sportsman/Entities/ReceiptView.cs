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
			Name = "קבלה";
			PluralName = "קבלות";

			//
			// Fields
			//

			// Id
			EntityFieldView efv = Fields[(int) Sport.Entities.Receipt.Fields.Id];
			efv.Name = "קוד";
			efv.Width = 30;
			// Number
			efv = Fields[(int) Sport.Entities.Receipt.Fields.Number];
			efv.Name = "מספר";
			efv.Width = 80;
			// Region
			efv = Fields[(int) Sport.Entities.Receipt.Fields.Region];
			efv.Name = "מחוז";
			efv.Width = 150;
			efv.Values = Sport.Entities.Region.Type.GetEntities(null);
			efv.CanEdit = false;
			// Account
			efv = Fields[(int) Sport.Entities.Receipt.Fields.Account];
			efv.Name = "חשבון";
			efv.Width = 180;
			efv.CanEdit = false;
			// Sum
			efv = Fields[(int) Sport.Entities.Receipt.Fields.Sum];
			efv.Name = "סכום";
			efv.Width = 120;
			efv.CanEdit = false;
			// Date
			efv = Fields[(int) Sport.Entities.Receipt.Fields.Date];
			efv.Name = "תאריך";
			efv.Width = 120;
			// Remarks
			efv = Fields[(int) Sport.Entities.Receipt.Fields.Remarks];
			efv.Name = "הערות";
			efv.Size = new System.Drawing.Size(400, 75);
			// Season
			efv = Fields[(int) Sport.Entities.Receipt.Fields.Season];
			efv.Name = "עונה";
			efv.Width = 120;
			efv.Values = Sport.Entities.Season.Type.GetEntities(null);
			// LastModified
			efv = Fields[(int) Sport.Entities.Receipt.Fields.LastModified];
			efv.Name = "ת' שינוי אחרון";
			efv.Width = 120;
		}
	}
}
