using System;
using Sport.UI;

namespace Sportsman.Entities
{
	public class ChampionshipCategoryView : EntityView
	{
		public ChampionshipCategoryView()
			: base (Sport.Entities.ChampionshipCategory.Type)
		{
			//
			// Entity
			//
			Name = "������� ������";
			PluralName = "�������� ������";

			//
			// Fields
			//

			// Championship
			EntityFieldView efv = Fields[(int) Sport.Entities.ChampionshipCategory.Fields.Championship];
			efv.Name = "������";
			efv.Width = 200;
			// Id
			efv = Fields[(int) Sport.Entities.ChampionshipCategory.Fields.Id];
			efv.Name = "���";
			efv.Width = 30;
			// Category
			efv = Fields[(int) Sport.Entities.ChampionshipCategory.Fields.Category];
			efv.Name = "�������";
			efv.Width = 150;
			// Status
			efv = Fields[(int) Sport.Entities.ChampionshipCategory.Fields.Status];
			efv.Name = "�����";
			efv.Width = 100;
			// RegistrationPrice
			efv = Fields[(int) Sport.Entities.ChampionshipCategory.Fields.RegistrationPrice];
			efv.Name = "����� �����";
			efv.Width = 150;
			// LastModified
			efv = Fields[(int) Sport.Entities.ChampionshipCategory.Fields.LastModified];
			efv.Name = "�' ����� �����";
			efv.Width = 120;
		}
	}
}
