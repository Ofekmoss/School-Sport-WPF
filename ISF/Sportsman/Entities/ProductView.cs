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
			Name = "��� ����";
			PluralName = "���� ����";

			//
			// Fields
			//

			// Id
			EntityFieldView efv = Fields[(int) Sport.Entities.Product.Fields.Id];
			efv.Name = "���";
			efv.Width = 30;
			// Area
			efv = Fields[(int) Sport.Entities.Product.Fields.Area];
			efv.Name = "����";
			efv.Width = 120;
			efv.Values = Sport.Entities.ProductArea.Type.GetEntities(null);
			// Name
			efv = Fields[(int) Sport.Entities.Product.Fields.Name];
			efv.Name = "�� ��� ����";
			efv.Width = 180;
			// Price
			efv = Fields[(int) Sport.Entities.Product.Fields.Price];
			efv.Name = "�����";
			efv.Width = 100;
			// LastModified
			efv = Fields[(int) Sport.Entities.Product.Fields.LastModified];
			efv.Name = "�' ����� �����";
			efv.Width = 120;
		}
	}
}
