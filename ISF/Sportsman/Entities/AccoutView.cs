using System;
using Sport.UI;

namespace Sportsman.Entities
{
	public class AccountView : EntityView
	{
		public AccountView()
			: base (Sport.Entities.Account.Type)
		{
			//
			// Entity
			//
			Name = "חשבון";
			PluralName = "חשבונות";

			//
			// Fields
			//

			// Id
			EntityFieldView efv = Fields[(int) Sport.Entities.Account.Fields.Id];
			efv.Name = "קוד";
			efv.Width = 30;
			// Region
			efv = Fields[(int) Sport.Entities.Account.Fields.Region];
			efv.Name = "מחוז";
			efv.Width = 150;
			efv.Values = Sport.Entities.Region.Type.GetEntities(null);
			// Name
			efv = Fields[(int) Sport.Entities.Account.Fields.Name];
			efv.Name = "שם חשבון";
			efv.Width = 180;
			// School
			efv = Fields[(int) Sport.Entities.Account.Fields.School];
			efv.Name = "בית ספר";
			efv.Width = 180;
			efv.CanEdit = false;
			// Balance
			efv = Fields[(int) Sport.Entities.Account.Fields.Balance];
			efv.Name = "מאזן";
			efv.Width = 120;
			efv.CanEdit = false;
			// LastModified
			efv = Fields[(int) Sport.Entities.Account.Fields.LastModified];
			efv.Name = "ת' שינוי אחרון";
			efv.Width = 120;
		}

		public override void OnSelectEntity(Sport.Data.Entity entity)
		{
			if (entity != null)
			{
				Sport.Entities.Account account = new Sport.Entities.Account(entity);

				Fields[(int) Sport.Entities.Account.Fields.Name].CanEdit = account.School == null;
			}
		}

	}
}
