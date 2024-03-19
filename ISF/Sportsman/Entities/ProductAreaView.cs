using System;
using Sport.UI;

namespace Sportsman.Entities
{
	public class ProductAreaView : EntityView
	{
		public ProductAreaView()
			: base (Sport.Entities.ProductArea.Type)
		{
			//
			// Entity
			//
			Name = "תחום חיוב";
			PluralName = "תחומי חיוב";

			//
			// Fields
			//

			// Id
			EntityFieldView efv = Fields[(int) Sport.Entities.ProductArea.Fields.Id];
			efv.Name = "קוד";
			efv.Width = 30;
			// Name
			efv = Fields[(int) Sport.Entities.ProductArea.Fields.Name];
			efv.Name = "שם תחום";
			efv.Width = 180;
			// LastModified
			efv = Fields[(int) Sport.Entities.ProductArea.Fields.LastModified];
			efv.Name = "ת' שינוי אחרון";
			efv.Width = 120;
		}
	}
}
