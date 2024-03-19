using System;
using Sport.UI;

namespace Sportsman.Entities
{
	public class UserView : EntityView
	{
		// School members
		private EntitySelectionDialog schoolDialog;

		public UserView()
			: base (Sport.Entities.User.Type)
		{
			//
			// Entity
			//
			Name = "�����";
			PluralName = "�������";

			//
			// Fields
			//

			// Id
			EntityFieldView efv = Fields[(int) Sport.Entities.User.Fields.Id];
			efv.Name = "���";
			efv.Width = 30;
			// Login
			efv = Fields[(int) Sport.Entities.User.Fields.Login];
			efv.Name = "�����";
			efv.Width = 50;
			// FirstName
			efv = Fields[(int) Sport.Entities.User.Fields.FirstName];
			efv.Name = "�� ����";
			efv.Width = 80;
			// LastName
			efv = Fields[(int) Sport.Entities.User.Fields.LastName];
			efv.Name = "�� �����";
			efv.Width = 80;
			// Region
			efv = Fields[(int) Sport.Entities.User.Fields.Region];
			efv.Name = "����";
			efv.Width = 120;
			efv.Values = Sport.Entities.Region.Type.GetEntities(null);
			// School
			efv = Fields[(int) Sport.Entities.User.Fields.School];
			efv.Name = "��� ���";
			efv.Width = 180;
			schoolDialog = new EntitySelectionDialog(new Views.SchoolsTableView());
			schoolDialog.View.State[View.SelectionDialog] = "1";
			efv.GenericItemType = Sport.UI.Controls.GenericItemType.Button;
			efv.Values = Sport.UI.Controls.GenericItem.ButtonValues(new Sport.UI.Controls.ButtonBox.SelectValue(schoolDialog.ValueSelector));
			// UserType
			efv = Fields[(int) Sport.Entities.User.Fields.UserType];
			efv.Name = "���";
			efv.Width = 120;
			// Permissions
			efv = Fields[(int) Sport.Entities.User.Fields.Permissions];
			efv.Name = "������";
			efv.Width = 80;
			// Email
			efv = Fields[(int) Sport.Entities.User.Fields.Email];
			efv.Name = "���\"�";
			efv.Width = 150;
			// LastModified
			efv = Fields[(int) Sport.Entities.User.Fields.LastModified];
			efv.Name = "�' ����� �����";
			efv.Width = 120;
		}

		public override void OnSelectEntity(Sport.Data.Entity entity)
		{
			if (entity != null)
			{
				Sport.Entities.User user = new Sport.Entities.User(entity);

				Sport.Entities.Region region = user.Region;
				Sport.Entities.School school = user.School;
				if (school != null)
				{
					if (region == null)
						region = school.Region;
					schoolDialog.View.State[Sport.Entities.City.TypeName] = school.City.Id.ToString();
					schoolDialog.View.State["school"] = school.Id.ToString();
				}
				else
				{
					if (region == null)
						region = new Sport.Entities.Region(Sport.Core.Session.Region);
					schoolDialog.View.State[Sport.Entities.City.TypeName] = null;
					schoolDialog.View.State["school"] = null;
				}

				schoolDialog.View.State[Sport.Entities.Region.TypeName] = region.Id.ToString();
			}
		}
	}
}
