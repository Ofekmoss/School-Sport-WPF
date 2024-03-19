using System;
using Sport.UI;
using Sport.UI.Display;
using Sport.Data;

namespace Sportsman.Views
{
	/// <summary>
	/// Summary description for SportsTableView.
	/// </summary>
	[Sport.UI.ViewPermissions(Sport.Types.Permission.Sports, true)]
	public class SportsTableView : TableView
	{
		public SportsTableView()
		{
			DetailsBarEnabled = false;
			FilterBarEnabled = false;
			Items.Add((int)Sport.Entities.Sport.Fields.Name, "שם", 150);
			Items.Add((int)Sport.Entities.Sport.Fields.SportType, "סוג", 100);
			Items.Add((int)Sport.Entities.Sport.Fields.Ruleset, "תקנון", 100);
			Items.Add((int)Sport.Entities.Sport.Fields.LastModified, "תאריך שינוי אחרון", 120);
			Items.Add((int)Sport.Entities.Sport.Fields.PointsName, "הגדרת ניקוד", 120);
			Items.Add((int)Sport.Entities.Sport.Fields.CentralRegionOnly, "מטה בלבד?", 100);
		}

		public override void Open()
		{
			Title = "ענפי ספורט";

			EntityListView = new EntityListView(Sport.Entities.Sport.TypeName);

			Columns = new int[] { 0, 1, 2 };

			EntityListView.Read(null);

			base.Open();
		}

		protected override bool OnDeleteEntity(Entity entity)
		{
			//cancel if no entity selected:
			if (entity == null)
				return false;

			//build proper entity:
			Sport.Entities.Sport sport =
				new Sport.Entities.Sport(entity);

			//begin checkings... first check for championships:
			Sport.Entities.Championship[] champs = sport.GetChampionships();
			if (champs.Length > 0)
			{
				string names = "";
				for (int i = 0; i < champs.Length; i++)
				{
					names += champs[i].Name + "\n";
					if (i >= 15)
					{
						names += "...\n";
						break;
					}
				}
				Sport.UI.MessageBox.Warn("ענף הספורט '" + sport.Name + "' כולל את האליפויות הבאות: \n" +
					names + "יש להסיר אליפויות אלו", "מחיקת ענף ספורט");
				return false;
			}

			return Sport.UI.MessageBox.Ask("האם למחוק את ענף הספורט '" + sport.Name + "'?", false);
		}


		protected override void OnSelectEntity(Entity entity)
		{
			if (entity != null)
			{
				EntityListView.Fields[(int)Sport.Entities.Sport.Fields.Ruleset].Values =
					Sport.Entities.Ruleset.Type.GetEntities(
					new EntityFilter((int)Sport.Entities.Ruleset.Fields.Sport,
					entity.Id));
			}
		}
	}
}
