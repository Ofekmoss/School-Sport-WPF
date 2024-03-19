using System;
using Sport.UI;

namespace Sportsman.Entities
{
	public class ProductView : EntityView
	{
		public ProductView()
			: base (Sport.Entities.Product.Type)
		{
			//
			// Entity
			//
			Name = "סוג חיוב";
			PluralName = "סוגי חיוב";

			//
			// Fields
			//

			// Id
			EntityFieldView efv = Fields[(int) Sport.Entities.Product.Fields.Id];
			efv.Name = "קוד";
			efv.Width = 30;
			// Area
			efv = Fields[(int) Sport.Entities.Product.Fields.Area];
			efv.Name = "תחום";
			efv.Width = 120;
			efv.Values = Sport.Entities.ProductArea.Type.GetEntities(null);
			// Name
			efv = Fields[(int) Sport.Entities.Product.Fields.Name];
			efv.Name = "שם סוג חיוב";
			efv.Width = 180;
			// Price
			efv = Fields[(int) Sport.Entities.Product.Fields.Price];
			efv.Name = "תעריף";
			efv.Width = 100;
			// LastModified
			efv = Fields[(int) Sport.Entities.Product.Fields.LastModified];
			efv.Name = "ת' שינוי אחרון";
			efv.Width = 120;
		}
	}
}
