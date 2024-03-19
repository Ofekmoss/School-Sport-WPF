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
			Name = "���� ����";
			PluralName = "����� ����";

			//
			// Fields
			//

			// Id
			EntityFieldView efv = Fields[(int) Sport.Entities.ProductArea.Fields.Id];
			efv.Name = "���";
			efv.Width = 30;
			// Name
			efv = Fields[(int) Sport.Entities.ProductArea.Fields.Name];
			efv.Name = "�� ����";
			efv.Width = 180;
			// LastModified
			efv = Fields[(int) Sport.Entities.ProductArea.Fields.LastModified];
			efv.Name = "�' ����� �����";
			efv.Width = 120;
		}
	}
}
