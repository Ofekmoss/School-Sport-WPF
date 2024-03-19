using System;
using Sport.UI;

namespace Sportsman.Views
{
	/// <summary>
	/// Summary description for ChampionshipsTableView.
	/// </summary>
	[Sport.UI.ViewPermissions(Sport.Types.Permission.Championships, true)]
	public class StandardChampionshipsTableView : Sport.UI.TableView
	{
		private GridDetailItem gdiCategories;
		
		public StandardChampionshipsTableView()
		{
			Items.Add((int) Sport.Entities.StandardChampionship.Fields.Name, "שם", 200);
			Items.Add((int) Sport.Entities.StandardChampionship.Fields.Sport, "ענף", 100);
			Items.Add((int) Sport.Entities.StandardChampionship.Fields.IsRegional, "סוג אליפות", 100);
			Items.Add((int) Sport.Entities.StandardChampionship.Fields.IsOpen, "פתוחה", 100);
			Items.Add((int) Sport.Entities.StandardChampionship.Fields.IsClubs, "מועדון", 80);
			Items.Add((int) Sport.Entities.StandardChampionship.Fields.Ruleset, "תקנון", 200);
			
			// Creating Categories detail grid

			gdiCategories = new GridDetailItem("קטגוריות:", 
				Sport.Entities.StandardChampionshipCategory.Type, 
				(int) Sport.Entities.StandardChampionshipCategory.Fields.Championship, 
				new System.Drawing.Size(400, 120));

			gdiCategories.Columns.Add((int) Sport.Entities.StandardChampionshipCategory.Fields.Category, "קטגוריה", 200);

			EntityListView.Field field = gdiCategories.EntityListView.Fields[
				(int) Sport.Entities.StandardChampionshipCategory.Fields.Category];
			field.GenericItemType = Sport.UI.Controls.GenericItemType.Button;
			field.Values = Sport.UI.Controls.GenericItem.ButtonValues(new Sport.UI.Controls.ButtonBox.SelectValue(Forms.CategorySelectionDialog.ValueSelector));
			Items.Add("קטגוריות", gdiCategories);

			Items.Add((int) Sport.Entities.StandardChampionship.Fields.LastModified, "תאריך שינוי אחרון", 120);
		}

		public override void Open()
		{
			EntityListView = new EntityListView(Sport.Entities.StandardChampionship.TypeName);

			EntityListView.Fields[(int) Sport.Entities.StandardChampionship.Fields.Sport].Values =
				Sport.Entities.Sport.Type.GetEntities(null);

			

			Columns = new int[] { 0, 1, 2, 3, 4, 5 };
			Details = new int[] { 6 };

			Title = "אליפויות קבועות";

			EntityListView.Read(null);

			base.Open();
		}

		private void SetFields(Sport.Entities.StandardChampionship championship)
		{
			EntityListView.Fields[(int) Sport.Entities.StandardChampionship.Fields.IsOpen].CanEdit =
				!championship.IsRegional;
			if (championship.Sport == null)
			{
				EntityListView.Fields[(int) Sport.Entities.StandardChampionship.Fields.Ruleset].Values = null;
				EntityListView.Fields[(int) Sport.Entities.StandardChampionship.Fields.Ruleset].CanEdit = false;
			}
			else
			{
				EntityListView.Fields[(int) Sport.Entities.StandardChampionship.Fields.Ruleset].Values = 
					championship.Sport.GetRulesets();
				EntityListView.Fields[(int) Sport.Entities.StandardChampionship.Fields.Ruleset].CanEdit = true;
			}
		}

		protected override void OnSelectEntity(Sport.Data.Entity entity)
		{
			if (entity != null)
			{
				SetFields(new Sport.Entities.StandardChampionship(entity));
			}

			base.OnSelectEntity (entity);
		}

		protected override void OnValueChange(Sport.Data.EntityEdit entityEdit, Sport.Data.EntityField entityField)
		{
			if (entityField.Index == (int) Sport.Entities.StandardChampionship.Fields.IsRegional)
			{
				Sport.Entities.StandardChampionship championship = new Sport.Entities.StandardChampionship(entityEdit);
				if (championship.IsRegional)
					championship.IsOpen = true;
				SetFields(championship);
			}

			base.OnValueChange (entityEdit, entityField);
		}



		protected override bool OnDeleteEntity(Sport.Data.Entity entity)
		{
			return Sport.UI.MessageBox.Ask("האם למחוק את האליפות '" + entity.Name + "'?", false);
		}
	
		protected override void OnNewEntity(Sport.Data.EntityEdit entityEdit)
		{
			Sport.Entities.StandardChampionship championship = new Sport.Entities.StandardChampionship(entityEdit);
			championship.IsRegional = false;
			championship.IsOpen = true;
			championship.IsClubs = false;
			SetFields(championship);
		}
	
	}
}
