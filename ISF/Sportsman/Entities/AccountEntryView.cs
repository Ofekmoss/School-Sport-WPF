using System;
using Sport.UI;

namespace Sportsman.Entities
{
	public class AccountEntryView : EntityView
	{
		public AccountEntryView()
			: base (Sport.Entities.AccountEntry.Type)
		{
			//
			// Entity
			//
			Name = "תנועה";
			PluralName = "תנועות";

			//
			// Fields
			//

			// Id
			EntityFieldView efv = Fields[(int) Sport.Entities.AccountEntry.Fields.Id];
			efv.Name = "קוד";
			efv.Width = 30;
			// Region
			efv = Fields[(int) Sport.Entities.AccountEntry.Fields.Region];
			efv.Name = "מחוז";
			efv.Width = 110;
			// Account
			efv = Fields[(int) Sport.Entities.AccountEntry.Fields.Account];
			efv.Name = "חשבון";
			efv.Width = 180;
			// EntryType
			efv = Fields[(int) Sport.Entities.AccountEntry.Fields.EntryType];
			efv.Name = "סוג תנועה";
			efv.Width = 80;
			// Sum
			efv = Fields[(int) Sport.Entities.AccountEntry.Fields.Sum];
			efv.Name = "סכום";
			efv.Width = 90;
			// Description
			efv = Fields[(int) Sport.Entities.AccountEntry.Fields.Description];
			efv.Name = "תיאור";
			efv.Width = 210;
			// Additional
			efv = Fields[(int) Sport.Entities.AccountEntry.Fields.Additional];
			efv.Name = "מידע נוסף";
			efv.Width = 60;
			// EntryDate
			efv = Fields[(int) Sport.Entities.AccountEntry.Fields.EntryDate];
			efv.Name = "תאריך";
			efv.Width = 110;
			// Season
			efv = Fields[(int) Sport.Entities.AccountEntry.Fields.Season];
			efv.Name = "עונה";
			efv.Width = 110;
			// LastModified
			efv = Fields[(int) Sport.Entities.AccountEntry.Fields.LastModified];
			efv.Name = "ת' שינוי אחרון";
			efv.Width = 120;
		}
	}
}
