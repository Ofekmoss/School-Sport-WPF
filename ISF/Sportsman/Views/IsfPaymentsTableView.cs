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
	/// Summary description for IsfPaymentsTableView.
	/// </summary>
	[Sport.UI.ViewPermissions(Sport.Types.Permission.General)]
	public class IsfPaymentsTableView : TableView
	{
		private EntityFilter _filter;
		EntitySelectionDialog _equipDialog;
		
		private enum ColumnTitles
		{
			Equipment=0,
			PaymentSum,
			Description,
			DatePaid,
			Type,
			PaidBy,
			DateLastModified
		}
		
		public IsfPaymentsTableView()
		{
			Items.Add((int) Sport.Entities.IsfPayment.Fields.Equipment, "שולם עבור", 200);
			Items.Add((int) Sport.Entities.IsfPayment.Fields.Sum, "סכום התשלום", 120);
			Items.Add((int) Sport.Entities.IsfPayment.Fields.Description, "תיאור התשלום", 200, 120);
			Items.Add((int) Sport.Entities.IsfPayment.Fields.PaymentDate, "תאריך תשלום", 120);
			Items.Add((int) Sport.Entities.IsfPayment.Fields.PaymentType, "צורת תשלום", 150);
			Items.Add((int) Sport.Entities.IsfPayment.Fields.PaidBy, "שולם ע\"י", 120);
			Items.Add((int) Sport.Entities.IsfPayment.Fields.LastModified, "תאריך שינוי אחרון", 120);
		}
		
		public override void Open()
		{
			EntityListView = new EntityListView(Sport.Entities.IsfPayment.TypeName);
			
			//selection dialogs:
			EquipmentTableView equipView=new EquipmentTableView();
			equipView.State[SelectionDialog] = "1";
			_equipDialog = new EntitySelectionDialog(equipView);
			EntityListView.Fields[(int) Sport.Entities.IsfPayment.Fields.Equipment].GenericItemType =
				Sport.UI.Controls.GenericItemType.Button;
			EntityListView.Fields[(int) Sport.Entities.IsfPayment.Fields.Equipment].Values =
				Sport.UI.Controls.GenericItem.ButtonValues(new ButtonBox.SelectValue(_equipDialog.ValueSelector));
			
			//populate fields:
			
			// users
			Entity[] users=Sport.Entities.User.Type.GetEntities(new EntityFilter(
				(int) Sport.Entities.User.Fields.Id, Core.UserManager.CurrentUser.Id));
			EntityListView.Fields[(int) Sport.Entities.IsfPayment.Fields.PaidBy].Values = 
				users;
			
			Columns = new int[] { (int) ColumnTitles.Equipment, (int) ColumnTitles.Type, 
									(int) ColumnTitles.DatePaid, (int) ColumnTitles.PaidBy,
									(int) ColumnTitles.PaymentSum };
			Details = new int[] { (int) ColumnTitles.Equipment, (int) ColumnTitles.Description,
									(int) ColumnTitles.PaymentSum };
			
			//Filters:
			
			ReloadView();
			base.Open();
		} //end function Open
		
		protected override void OnNewEntity(EntityEdit entityEdit)
		{
			EntityListView.Fields[(int) Sport.Entities.IsfPayment.Fields.PaidBy].EntityField.SetValue(
				entityEdit, Core.UserManager.CurrentUser.Id);
			
			entityEdit.Fields[(int) Sport.Entities.IsfPayment.Fields.PaymentDate] = 
				System.DateTime.Now;

			entityEdit.Fields[(int) Sport.Entities.IsfPayment.Fields.Description] = 
				"";
		}
		
		protected override bool OnDeleteEntity(Entity entity)
		{
			if (entity == null)
				return false;
			Sport.Entities.IsfPayment payment=new Sport.Entities.IsfPayment(entity);
			
			return Sport.UI.MessageBox.Ask("האם למחוק את התשלום '" + payment.Name + "'?", false);
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
			
			Title = "ניהול תשלומי התאחדות";
			Cursor.Current = c;
		} //end function Requery
		
		private void ReloadView()
		{
			//refresh all filters and requery the information.
			RefreshFilters();
			Requery();
		}
	} //end class IsfPaymentTableView
}