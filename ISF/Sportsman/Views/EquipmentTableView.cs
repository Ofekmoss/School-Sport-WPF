using System;
using Sport.UI;
using Sport.Data;
using System.Windows.Forms;
using System.Collections;
using Sportsman.Core;
using Sport.UI.Controls;
using Sport.Types;

namespace Sportsman.Views
{
	/// <summary>
	/// Summary description for EquipmentTableView.
	/// </summary>
	[Sport.UI.ViewPermissions(Sport.Types.Permission.General)]
	public class EquipmentTableView : TableView
	{
		private EntityFilter _filter;
		EntitySelectionDialog _equipDialog;
		
		private enum ColumnTitles
		{
			Type=0,
			Region,
			Sport,
			Championship,
			Category,
			Amount,
			Price,
			DateOrdered,
			DateLastModified
		}
		
		public EquipmentTableView()
		{
			Items.Add((int) Sport.Entities.Equipment.Fields.Type, "סוג ציוד", 200);
			Items.Add((int) Sport.Entities.Equipment.Fields.Region, "מחוז", 110);
			Items.Add((int) Sport.Entities.Equipment.Fields.Sport, "ענף ספורט", 150);
			Items.Add((int) Sport.Entities.Equipment.Fields.Championship, "אליפות", 180);
			Items.Add((int) Sport.Entities.Equipment.Fields.Category, "קטגורית אליפות", 200);
			Items.Add((int) Sport.Entities.Equipment.Fields.Amount, "כמות", 100);
			Items.Add((int) Sport.Entities.Equipment.Fields.Price, "מחיר", 100);
			Items.Add((int) Sport.Entities.Equipment.Fields.DateOrdered, "תאריך הזמנה", 100);
			Items.Add((int) Sport.Entities.Equipment.Fields.LastModified, "תאריך שינוי אחרון", 150);
		}
		
		public override void Open()
		{
			EntityListView = new EntityListView(Sport.Entities.Equipment.TypeName);

			//selection dialogs:
			EquipmentTypesTableView equipTypeView=new EquipmentTypesTableView();
			equipTypeView.State[SelectionDialog] = "1";
			_equipDialog = new EntitySelectionDialog(equipTypeView);
			EntityListView.Fields[(int) Sport.Entities.Equipment.Fields.Type].GenericItemType =
				Sport.UI.Controls.GenericItemType.Button;
			EntityListView.Fields[(int) Sport.Entities.Equipment.Fields.Type].Values =
				Sport.UI.Controls.GenericItem.ButtonValues(new ButtonBox.SelectValue(_equipDialog.ValueSelector));

			//populate fields:

			// regions
			Entity[] regions=Sport.Entities.Region.Type.GetEntities(null);
			//EntityListView.Fields[(int) Sport.Entities.Equipment.Fields.Region].CanEdit = false;
			EntityListView.Fields[(int) Sport.Entities.Equipment.Fields.Region].Values = 
				regions;
			/* if (Core.UserManager.CurrentUser.UserRegion != Sport.Entities.Region.CentralRegion)
				EntityListView.Fields[(int) Sport.Entities.Equipment.Fields.Region].
					EntityField.SetValue(EntityListView.EntityEdit, Core.UserManager.CurrentUser.UserRegion); */
			
			// sports
			Entity[] sports=Sport.Entities.Sport.Type.GetEntities(null);
			//EntityListView.Fields[(int) Sport.Entities.Equipment.Fields.Sport].CanEdit = false;
			EntityListView.Fields[(int) Sport.Entities.Equipment.Fields.Sport].Values = 
				sports;
			
			if (State[SelectionDialog] == "1")
			{
				Columns = new int[] { (int) ColumnTitles.Type, (int) ColumnTitles.DateOrdered, 
										(int) ColumnTitles.Price };
			}
			else
			{
				Columns = new int[] { (int) ColumnTitles.Type, (int) ColumnTitles.Region, 
										(int) ColumnTitles.Sport, (int) ColumnTitles.Championship,
										(int) ColumnTitles.Category, (int) ColumnTitles.DateOrdered, 
										(int) ColumnTitles.Amount, (int) ColumnTitles.Price };
			}
			Details = new int[] { (int) ColumnTitles.Type, (int) ColumnTitles.Region, 
							(int) ColumnTitles.Sport, (int) ColumnTitles.Championship,
							(int) ColumnTitles.Category, (int) ColumnTitles.Amount };
			
			//Filters:
			
			ReloadView();
			base.Open();
		} //end function Open
		
		protected override void OnValueChange(EntityEdit entityEdit, EntityField entityField)
		{
			Sport.Entities.Equipment newEquipment = new Sport.Entities.Equipment(entityEdit);
			Sport.Entities.Equipment oldEquipment = (entityEdit.Entity == null)?null:new Sport.Entities.Equipment(entityEdit.Entity);
			
			if (newEquipment == null)
				return;
			
			//get what field has been changed:
			Sport.Entities.Equipment.Fields changedField=
				Sport.Entities.Equipment.Fields.FieldCount;
			
			//maybe changed current value:
			if ((oldEquipment != null)&&(oldEquipment.Id >= 0))
			{
				int oldRegion=(oldEquipment.Region == null)?-1:oldEquipment.Region.Id;
				int newRegion=(newEquipment.Region == null)?-1:newEquipment.Region.Id;
				int oldSport=(oldEquipment.Sport == null)?-1:oldEquipment.Sport.Id;
				int newSport=(newEquipment.Sport == null)?-1:newEquipment.Sport.Id;
				int oldChamp=(oldEquipment.Championship == null)?-1:oldEquipment.Championship.Id;
				int newChamp=(newEquipment.Championship == null)?-1:newEquipment.Championship.Id;
				int oldCategory=(oldEquipment.Category == null)?-1:oldEquipment.Category.Id;
				int newCategory=(newEquipment.Category == null)?-1:newEquipment.Category.Id;
				if (oldRegion != newRegion)
					changedField = Sport.Entities.Equipment.Fields.Region;
				else if (oldSport != newSport)
					changedField = Sport.Entities.Equipment.Fields.Sport;
				else if (oldChamp != newChamp)
					changedField = Sport.Entities.Equipment.Fields.Championship;
				else if (oldCategory != newCategory)
					changedField = Sport.Entities.Equipment.Fields.Category;
				else if (oldEquipment.EquipmentType != newEquipment.EquipmentType)
					changedField = Sport.Entities.Equipment.Fields.Type;
			}
			else
			{
				//maybe new value?
				if (entityField.Index == (int) Sport.Entities.Equipment.Fields.Region)
					changedField = Sport.Entities.Equipment.Fields.Region;
				else if (entityField.Index == (int) Sport.Entities.Equipment.Fields.Sport)
					changedField = Sport.Entities.Equipment.Fields.Sport;
				else if (entityField.Index == (int) Sport.Entities.Equipment.Fields.Championship)
					changedField = Sport.Entities.Equipment.Fields.Championship;
				else if (entityField.Index == (int) Sport.Entities.Equipment.Fields.Category)
					changedField = Sport.Entities.Equipment.Fields.Category;
				else if (entityField.Index == (int) Sport.Entities.Equipment.Fields.Type)
					changedField = Sport.Entities.Equipment.Fields.Type;
			}
			
			switch (changedField)
			{
				case Sport.Entities.Equipment.Fields.Sport:
					//get sport:
					int sportID=Sport.Common.Tools.CIntDef(
						newEquipment.Entity.Fields[(int) Sport.Entities.Equipment.Fields.Sport], -1);
					SetEquipmentChamionships(sportID);
					break;
				case Sport.Entities.Equipment.Fields.Championship:
					//get championship:
					int champID=Sport.Common.Tools.CIntDef(
						newEquipment.Entity.Fields[(int) Sport.Entities.Equipment.Fields.Championship], -1);
					SetEquipmentCategories(champID);
					break;
				case Sport.Entities.Equipment.Fields.Type:
					//get equip type:
					int equipTypeID=Sport.Common.Tools.CIntDef(
						newEquipment.Entity.Fields[(int) Sport.Entities.Equipment.Fields.Type], -1);
					Sport.Entities.EquipmentType equipType=
						new Sport.Entities.EquipmentType(equipTypeID);
					if (equipType.Id < 0)
					{
						ResetFieldValues(EquipmentSeperationType.General);
					}
					else
					{
						ResetFieldValues(equipType.SeperationType);
						EntityListView.Fields[(int) Sport.Entities.Equipment.Fields.Price].EntityField.SetValue(
							EntityListView.EntityEdit, equipType.BasePrice);
					}
					break;
			}
			//Sport.UI.MessageBox.Show(changedField.ToString());
		}

		protected override void OnNewEntity(EntityEdit entityEdit)
		{
			ResetFieldValues(EquipmentSeperationType.General);
			EntityListView.Fields[(int) Sport.Entities.Equipment.Fields.DateOrdered].EntityField.SetValue(
				entityEdit, DateTime.Now);
		}

		protected override void OnSelectEntity(Entity entity)
		{
			if ((entity != null)&&(entity.Id >= 0))
			{
				Sport.Entities.Equipment equipment=
					new Sport.Entities.Equipment(entity);
				//populate fields:
				if ((equipment.Sport != null)&&(equipment.Sport.Id >= 0))
				{
					SetEquipmentChamionships(equipment.Sport.Id);
					if ((equipment.Championship != null)&&(equipment.Championship.Id >= 0))
					{
						SetEquipmentCategories(equipment.Championship.Id);
						if ((equipment.Category != null)&&(equipment.Category.Id >= 0))
						{
							EntityListView.Fields[(int) Sport.Entities.Equipment.Fields.Category].EntityField.SetValue(
								entity.Edit(), equipment.Category.Id);
						}
					}
				}
				ResetFieldValues(equipment.EquipmentType.SeperationType, false);
			}
		}
		
		protected override bool OnDeleteEntity(Entity entity)
		{
			if (entity == null)
				return false;
			Sport.Entities.Equipment equipment=new Sport.Entities.Equipment(entity);
			
			//check if there are any payments for this equipment:
			Sport.Entities.IsfPayment[] payments=equipment.GetPayments();
			if (payments.Length > 0)
			{
				string strData="";
				for (int i=0; i<payments.Length; i++)
				{
					strData += payments[i].Name+"\n";
					if (i >= 15)
					{
						strData += "...\n";
						break;
					}
				}
				Sport.UI.MessageBox.Warn("התשלומים הבאים שולמו עבור ציוד זה: \n"+
					strData+"יש למחוק תשלומים אלו", "מחיקת ציוד");
				return false;
			}

			return Sport.UI.MessageBox.Ask("האם למחוק את הציוד '" + equipment.Name + "'?", false);
		}
		
		private void SetEquipmentChamionships(int sportID)
		{
			EntityEdit ee=EntityListView.EntityEdit;
			if (ee == null)
				ee = Current.Edit();
			EntityFilter filter=new EntityFilter(
				(int)Sport.Entities.Championship.Fields.Sport, sportID);
			filter.Add(Sport.Entities.Championship.CurrentSeasonFilter());
			Entity[] champs=Sport.Entities.Championship.Type.GetEntities(filter);
			EntityListView.Fields[(int) Sport.Entities.Equipment.Fields.Championship].Values = 
				champs;
			EntityListView.Fields[(int) Sport.Entities.Equipment.Fields.Championship].EntityField.SetValue(
				ee, null);
			EntityListView.Fields[(int) Sport.Entities.Equipment.Fields.Category].Values = null;
			EntityListView.Fields[(int) Sport.Entities.Equipment.Fields.Category].EntityField.SetValue(
				ee, null);
		}
		
		private void SetEquipmentCategories(int champID)
		{
			EntityEdit ee=EntityListView.EntityEdit;
			if (ee == null)
				ee = Current.Edit();
			EntityFilter filter=new EntityFilter(
				(int)Sport.Entities.ChampionshipCategory.Fields.Championship, champID);
			Entity[] categories=
				Sport.Entities.ChampionshipCategory.Type.GetEntities(filter);
			EntityListView.Fields[(int) Sport.Entities.Equipment.Fields.Category].Values = 
				categories;
			EntityListView.Fields[(int) Sport.Entities.Equipment.Fields.Category].EntityField.SetValue(
				ee, null);
		}

		private void ResetFieldValues(EquipmentSeperationType level, bool blnReset)
		{
			int regionIndex=(int) Sport.Entities.Equipment.Fields.Region;
			int sportIndex=(int) Sport.Entities.Equipment.Fields.Sport;
			int champIndex=(int) Sport.Entities.Equipment.Fields.Championship;
			int categoryIndex=(int) Sport.Entities.Equipment.Fields.Category;
			int priceIndex=(int) Sport.Entities.Equipment.Fields.Price;
			
			if (blnReset)
			{
				EntityListView.Fields[regionIndex].EntityField.SetValue(
					EntityListView.EntityEdit, null);
				EntityListView.Fields[sportIndex].EntityField.SetValue(
					EntityListView.EntityEdit, null);
				EntityListView.Fields[champIndex].Values = null;
				EntityListView.Fields[champIndex].EntityField.SetValue(
					EntityListView.EntityEdit, null);
				EntityListView.Fields[categoryIndex].EntityField.SetValue(
					EntityListView.EntityEdit, null);
				EntityListView.Fields[categoryIndex].Values = null;
				//price
				EntityListView.Fields[priceIndex].EntityField.SetValue(
					EntityListView.EntityEdit, 0);
			}
			EntityListView.Fields[regionIndex].CanEdit = false;
			EntityListView.Fields[sportIndex].CanEdit = false;
			EntityListView.Fields[champIndex].CanEdit = false;
			EntityListView.Fields[categoryIndex].CanEdit = false;
			switch (level)
			{
				case EquipmentSeperationType.Regions:
					EntityListView.Fields[regionIndex].CanEdit = true;
					break;
				case EquipmentSeperationType.Categories:
					EntityListView.Fields[categoryIndex].CanEdit = true;
					EntityListView.Fields[champIndex].CanEdit = true;
					EntityListView.Fields[sportIndex].CanEdit = true;
					break;
				case EquipmentSeperationType.Championships:
					EntityListView.Fields[champIndex].CanEdit = true;
					EntityListView.Fields[sportIndex].CanEdit = true;
					break;
				case EquipmentSeperationType.Sports:
					EntityListView.Fields[sportIndex].CanEdit = true;
					break;
			}
		}

		private void ResetFieldValues(EquipmentSeperationType level)
		{
			ResetFieldValues(level, true);
		}

		#region Filters
		/// <summary>
		/// refresh all filters of this view.
		/// </summary>
		private void RefreshFilters()
		{
			_filter = new EntityFilter();
		}
		#endregion

		/// <summary>
		/// reload the view according to current filters. read from database.
		/// </summary>
		private void Requery()
		{
			Cursor c = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;
			
			EntityListView.Read(_filter); 
			
			/*
			Entity team = (Entity) teamFilter.Value;
			if (team == null)
			{
				EntityListView.Clear();
			}
			else
			{
				EntityListView.Read(filter); 
			}
			*/
			
			Title = "ניהול הזמנות ציוד";
			Cursor.Current = c;
		} //end function Requery

		private void ReloadView()
		{
			//refresh all filters and requery the information.
			RefreshFilters();
			Requery();
		}
	} //end class EquipmentTableView
}
