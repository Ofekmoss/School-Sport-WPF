using System;
using Sport.UI;

namespace Sportsman.Entities
{
	public class SchoolView : EntityView
	{
		public SchoolView()
			: base (Sport.Entities.School.Type)
		{
			//
			// Entity
			//
			Name = "בית ספר";
			PluralName = "בתי ספר";

			//
			// Fields
			//

			// Id
			EntityFieldView efv = Fields[(int) Sport.Entities.School.Fields.Id];
			efv.Name = "קוד";
			efv.Width = 30;
			// Symbol
			efv = Fields[(int) Sport.Entities.School.Fields.Symbol];
			efv.Name = "סמל";
			efv.Width = 80;
			// Name
			efv = Fields[(int) Sport.Entities.School.Fields.Name];
			efv.Name = "שם";
			efv.Width = 180;
			// City
			efv = Fields[(int) Sport.Entities.School.Fields.City];
			efv.Name = "ישוב";
			efv.Width = 100;
			// Address
			efv = Fields[(int) Sport.Entities.School.Fields.Address];
			efv.Name = "כתובת";
			efv.Width = 150;
			// MailAddress
			efv = Fields[(int) Sport.Entities.School.Fields.MailAddress];
			efv.Name = "מען";
			efv.Width = 150;
			// MailCity
			efv = Fields[(int) Sport.Entities.School.Fields.MailCity];
			efv.Name = "ישוב מען";
			efv.Width = 100;
			// ZipCode
			efv = Fields[(int) Sport.Entities.School.Fields.ZipCode];
			efv.Name = "מיקוד";
			efv.Width = 80;
			// Email
			efv = Fields[(int) Sport.Entities.School.Fields.Email];
			efv.Name = "דוא\"ל";
			efv.Width = 100;
			// Phone
			efv = Fields[(int) Sport.Entities.School.Fields.Phone];
			efv.Name = "טלפון";
			efv.Width = 80;
			// Fax
			efv = Fields[(int) Sport.Entities.School.Fields.Fax];
			efv.Name = "פקס";
			efv.Width = 80;
			// ManagerName
			efv = Fields[(int) Sport.Entities.School.Fields.ManagerName];
			efv.Name = "מנהל";
			efv.Width = 120;
			// FromGrade
			efv = Fields[(int) Sport.Entities.School.Fields.FromGrade];
			efv.Name = "משכבה";
			efv.Width = 60;
			// ToGrade
			efv = Fields[(int) Sport.Entities.School.Fields.ToGrade];
			efv.Name = "עד שכבה";
			efv.Width = 60;
			// Supervision
			efv = Fields[(int) Sport.Entities.School.Fields.Supervision];
			efv.Name = "סוג פיקוח";
			efv.Width = 100;
			// Sector
			efv = Fields[(int) Sport.Entities.School.Fields.Sector];
			efv.Name = "מגזר";
			efv.Width = 100;
			// Region
			efv = Fields[(int) Sport.Entities.School.Fields.Region];
			efv.Name = "מחוז";
			efv.Width = 150;
			efv.Values = Sport.Entities.Region.Type.GetEntities(null);
			// ClubStatus
			efv = Fields[(int) Sport.Entities.School.Fields.ClubStatus];
			efv.Name = "סטטוס מועדון";
			efv.Width = 120;
			// PlayerNumberFrom
			efv = Fields[(int) Sport.Entities.School.Fields.PlayerNumberFrom];
			efv.Name = "ממספר חולצה";
			efv.Width = 50;
			// PlayerNumberTo
			efv = Fields[(int) Sport.Entities.School.Fields.PlayerNumberTo];
			efv.Name = "עד מספר חולצה";
			efv.Width = 50;
			// LastModified
			efv = Fields[(int) Sport.Entities.School.Fields.LastModified];
			efv.Name = "ת' שינוי אחרון";
			efv.Width = 120;
			// Category
			efv = Fields[(int) Sport.Entities.School.Fields.Category];
			efv.Name = "קטגוריה";
			efv.Width = 150;
		}

		private int lastRegion = -1;
		private void SetRegionCities(Sport.Data.Entity entity)
		{
			if (entity != null)
			{
				Sport.Entities.School school = new Sport.Entities.School(entity);
				int region = school.Region == null ? -1 : school.Region.Id;
				if (region != lastRegion)
				{
					Sport.Data.Entity[] cities = null;
					cities = Sport.Entities.City.Type.GetEntities(
						new Sport.Data.EntityFilter((int) Sport.Entities.City.Fields.Region, region));
					Fields[(int) Sport.Entities.School.Fields.City].Values = cities;
					Fields[(int) Sport.Entities.School.Fields.MailCity].Values = cities;
					lastRegion = region;
				}
			}
		}

		public override void OnSelectEntity(Sport.Data.Entity entity)
		{
			SetRegionCities(entity);
		}

		public override void OnValueChange(Sport.Data.EntityEdit entityEdit,
			Sport.Data.EntityField entityField)
		{
			if (entityField.Index == (int) Sport.Entities.School.Fields.Region)
			{
				SetRegionCities(entityEdit);
			}
		}
	}
}

