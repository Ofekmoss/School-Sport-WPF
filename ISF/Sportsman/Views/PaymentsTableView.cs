using System;
using System.Collections;
using System.Windows.Forms;
using Sport.UI;
using Sport.Data;
using Sportsman.Core;
using Sport.UI.Controls;

namespace Sportsman.Views
{
	/// <summary>
	/// Summary description for PaymentsTableView.
	/// </summary>
	[Sport.UI.ViewPermissions(Sport.Types.Permission.General)]
	public class PaymentsTableView : TableView
	{
		private EntityFilter _filter;
		private ButtonBoxFilter schoolFilter;
		private EntitySelectionDialog schoolDialog;
		private EntitySelectionDialog chargeDialog;
		//private Sport.UI.TableView.GridDetailItem gdiCharges;

		private System.Windows.Forms.ToolBarButton tbbReceipt;

		private enum PaymentItems
		{
			Number = 0,
			Sum,
			Date,
			Type,
			Description,
			PaidBy,
			LastModified,
			Charges
		}

		public PaymentsTableView()
		{
			// Creating Categories detail grid
			/*gdiCharges = new GridDetailItem("חיובים:", 
				Sport.Entities.Charge.Type, 
				(int) Sport.Entities.Charge.Fields.Payment, 
				new System.Drawing.Size(420, 120));
			gdiCharges.Columns.Add((int) Sport.Entities.Charge.Fields.Product, "סוג חיוב", 150);
			gdiCharges.Columns.Add((int) Sport.Entities.Charge.Fields.Price, "סכום", 100);
			gdiCharges.Columns.Add((int) Sport.Entities.Charge.Fields.Date, "תאריך", 150);
			gdiCharges.EntityListView.Fields[(int) Sport.Entities.Charge.Fields.Product].CanEdit = false;
			gdiCharges.AutoInsert = false;
			gdiCharges.NewClick += new EventHandler(NewChargeClicked);
			gdiCharges.AutoDelete = false;
			gdiCharges.DeleteClick += new EventHandler(DeleteChargeClicked);*/

			//Items.Add((int) Sport.Entities.Payment.Fields.Number, "מספר תשלום", 100);
			Items.Add((int)Sport.Entities.Payment.Fields.Sum, "סכום", 120);
			Items.Add((int)Sport.Entities.Payment.Fields.Date, "תאריך", 120);
			Items.Add((int)Sport.Entities.Payment.Fields.Type, "אמצעי תשלום", 120);
			//Items.Add((int) Sport.Entities.Payment.Fields.Description, "פרטי תשלום", 150);
			//Items.Add((int) Sport.Entities.Payment.Fields.PaidBy, "שולם ע\"י", 120);
			Items.Add((int)Sport.Entities.Payment.Fields.LastModified, "ת' עדכון אחרון", 120);

			//Items.Add("חיובים", gdiCharges);

			//
			// toolBar
			//
			tbbReceipt = new System.Windows.Forms.ToolBarButton();
			tbbReceipt.ImageIndex = (int)Sport.Resources.ColorImages.Receipt;
			tbbReceipt.Text = "קבלה";
			tbbReceipt.Enabled = false;

			toolBar.Buttons.Add(tbbReceipt);
			toolBar.ButtonClick += new ToolBarButtonClickEventHandler(ToolBarButtonClicked);
		}

		public override void Open()
		{
			EntityListView = new EntityListView(Sport.Entities.Payment.TypeName);

			Columns = new int[] { (int) PaymentItems.Number, (int) PaymentItems.Sum, 
									(int) PaymentItems.Date, (int) PaymentItems.Type, 
									(int) PaymentItems.PaidBy };

			Details = new int[] { (int)PaymentItems.Description, (int)PaymentItems.Charges };

			ChargesTableView chargeView = new ChargesTableView();
			chargeView.State[SelectionDialog] = "1";
			chargeDialog = new EntitySelectionDialog(chargeView);

			schoolDialog = new EntitySelectionDialog(new Views.SchoolsTableView());
			schoolDialog.View.State[Sport.UI.View.SelectionDialog] = "1";

			//Filters:
			schoolFilter = new ButtonBoxFilter("בית ספר:",
				new Sport.UI.Controls.ButtonBox.SelectValue(
				schoolDialog.ValueSelector),
				null, "<בחר בית ספר>", 220);
			schoolFilter.FilterChanged += new EventHandler(SchoolFiltered);

			Filters.Add(schoolFilter);

			//set default school, if possible:
			if ((State[Sport.Entities.School.TypeName] == null) &&
				(UserManager.CurrentUser.UserSchool > 0))
			{
				Sport.Entities.School school = null;
				try
				{
					school = new Sport.Entities.School(UserManager.CurrentUser.UserSchool);
				}
				catch { }
				if ((school != null) && (school.Id >= 0))
				{
					State[Sport.Entities.School.TypeName] = school.Id.ToString();
					RefreshSchoolFilter();
				}
			}

			ReloadView();
			base.Open();
		} //end function Open

		private void RefreshSchoolFilter()
		{
			int schoolID = Sport.Common.Tools.CIntDef(State[Sport.Entities.School.TypeName], -1);
			Entity school = null;
			if (schoolID >= 0)
				school = (new Sport.Entities.School(schoolID)).Entity;
			schoolFilter.StopEvents = true;
			schoolFilter.Value = school;
			schoolFilter.StopEvents = false;
			SetSelectorsState(Sport.Entities.School.TypeName, (schoolID < 0) ? null : schoolID.ToString());
		}

		protected override void NewEntity()
		{
			if (schoolFilter.Value == null)
			{
				Sport.UI.MessageBox.Show("בחר בית ספר");
				return;
			}

			/*Sport.Entities.School school = new Sport.Entities.School((Sport.Data.Entity) schoolFilter.Value);

			Forms.PaymentForm paymentForm = new Forms.PaymentForm(school);

			if (paymentForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
			}*/

			base.NewEntity();
		}


		protected override void OnNewEntity(EntityEdit entityEdit)
		{
			EntityField entityField;

			//set school:
			if ((schoolFilter != null) && (schoolFilter.Value != null))
			{
				Sport.Entities.School school = new Sport.Entities.School((Entity)schoolFilter.Value);
				Sport.Entities.Payment payment = new Sport.Entities.Payment(entityEdit);
				//payment.School = school;
			}

			//set payment type:
			entityField = EntityListView.EntityType.Fields[(int)Sport.Entities.Payment.Fields.Type];
			entityField.SetValue(EntityListView.EntityEdit, (int)Sport.Types.PaymentType.Cheque);

			//set payment date:
			entityField = EntityListView.EntityType.Fields[(int)Sport.Entities.Payment.Fields.Date];
			entityField.SetValue(EntityListView.EntityEdit, DateTime.Now);
		}

		protected override void OnSelectEntity(Entity entity)
		{
			tbbReceipt.Enabled = entity != null;
		}

		protected override void OnValueChange(EntityEdit entityEdit, EntityField entityField)
		{
			//abort if no entity:
			if (entityEdit == null)
				return;
		}


		/// <summary>
		/// called when the school filter (combo box) is changed.
		/// </summary>
		private void SchoolFiltered(object sender, EventArgs e)
		{
			string strSchoolID = Core.Tools.GetFilterValue(schoolFilter.Value);
			int schoolID = Sport.Common.Tools.CIntDef(strSchoolID, -1);
			State[Sport.Entities.School.TypeName] = strSchoolID;
			ReloadView();
		}

		protected override void OnSaveEntity(Entity entity)
		{
			base.OnSaveEntity(entity);
		}

		private void RefreshFilters()
		{
			object school = Core.Tools.GetStateValue(State[Sport.Entities.School.TypeName]);

			if (school == null)
			{
				_filter = null;
			}
			else
			{
				_filter = new EntityFilter();
				int schoolID = Sport.Common.Tools.CIntDef(school, -1);
				//				_filter.Add(new EntityFilterField(
				//					(int) Sport.Entities.Payment.Fields.School, schoolID));
				schoolFilter.Value = Sport.Entities.School.Type.Lookup(schoolID);
			}
		}

		protected override bool OnDeleteEntity(Entity entity)
		{
			//check if empty:
			if (entity == null)
				return false;

			//create new entity:
			Sport.Entities.Payment payment = new Sport.Entities.Payment(entity);

			//for now no checkings are needed.

			return Sport.UI.MessageBox.Ask("האם למחוק את התשלום '" + payment.Entity.Name + "'?", false);
		}

		public override MenuItem[] GetContextMenu(Sport.UI.TableView.SelectionType selectionType)
		{
			//verify not null:
			if (Current == null)
				return null;

			MenuItem[] menuItems = null;
			switch (selectionType)
			{
				case (SelectionType.Single):
					menuItems = new MenuItem[2];
					menuItems[0] = new MenuItem("פתח", new System.EventHandler(PaymentDetailsClick));
					menuItems[0].DefaultItem = true;
					menuItems[1] = new MenuItem("-");
					break;
			}

			return menuItems;
		}

		private void NewChargeClicked(object sender, EventArgs e)
		{
			Sport.Data.Entity current = Current;
			if (current != null)
			{
				Sport.Entities.Payment payment = new Sport.Entities.Payment(current);
				object c = null;//Forms.PaymentForm.SelectCharge(payment.School);
				if (c != null)
				{
					if (c is Sport.Entities.Product)
					{
						Sport.Entities.Charge charge = new Sport.Entities.Charge(Sport.Entities.Charge.Type.New());
						//charge.School = payment.School;
						//charge.Payment = payment;
						charge.Product = (Sport.Entities.Product)c;
						charge.Save();
					}
					else if (c is Sport.Entities.Charge)
					{
						Sport.Entities.Charge charge = (Sport.Entities.Charge)c;
						charge.Edit();
						//charge.Payment = payment;
						charge.Save();
					}
				}
			}
		}

		private void DeleteChargeClicked(object sender, EventArgs e)
		{
			/*Sport.Data.Entity entity = gdiCharges.EntityListView.Current;
			if (entity != null)
			{
				Sport.Entities.Charge charge = new Sport.Entities.Charge(entity);
				charge.Edit();
				//charge.Payment = null;
				charge.Save();
			}*/
		}

		private void PaymentDetailsClick(object sender, EventArgs e)
		{
			//open details of selected payment in new dialog.
		}

		/// <summary>
		/// reload the view according to current filters. read from database.
		/// </summary>
		private void Requery()
		{
			Cursor c = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;

			Entity school = (Entity)schoolFilter.Value;

			if (school == null)
			{
				EntityListView.Clear();
				Title = "תשלומים";
			}
			else
			{
				EntityListView.Read(_filter);
				Title = "תשלומים - " + school.Name;
			}

			Cursor.Current = c;
		} //end function Requery

		private void ReloadView()
		{
			//refresh all filters and requery the information.
			RefreshFilters();
			Requery();
		}

		private void SetSelectorsState(string strName, string strValue)
		{
			schoolDialog.View.State[strName] = strValue;
			chargeDialog.View.State[strName] = strValue;
		}

		private void ToolBarButtonClicked(object sender, ToolBarButtonClickEventArgs e)
		{
			if (e.Button == tbbReceipt)
			{
				PrintReceipt();
			}
		}

		private Sport.Documents.Document CreateReceipt(System.Drawing.Printing.PrinterSettings settings,
			Sport.Entities.Payment payment)
		{
			Documents.PaymentDocuments paymentDocuments = new Documents.PaymentDocuments(settings);

			Sport.Documents.DocumentBuilder db = paymentDocuments.CreateDocumentBuilder("קבלה");
			//paymentDocuments.CreateChargesSection(db, payment.GetCharges());
			//paymentDocuments.CreatePaymentSection(db, payment);

			return db.CreateDocument();
		}

		private void PrintReceipt()
		{
			Sport.Data.Entity entity = Current;
			if (entity != null)
			{
				System.Drawing.Printing.PrinterSettings ps;
				Sport.UI.Dialogs.PrintSettingsForm settingsForm;
				if (!Sport.UI.Helpers.GetPrinterSettings(out ps, out settingsForm))
					return;
				if (settingsForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					Sport.Documents.Document document = CreateReceipt(ps, new Sport.Entities.Payment(entity));

					if (settingsForm.ShowPreview)
					{
						Sport.UI.Dialogs.PrintForm printForm = new Sport.UI.Dialogs.PrintForm(document, ps);

						if (!printForm.Canceled)
							printForm.ShowDialog();

						printForm.Dispose();
					}
					else
					{
						System.Drawing.Printing.PrintDocument pd = document.CreatePrintDocument(ps);
						pd.PrintController = new PrintControllerWithPageForm(pd.PrintController, 0);
						pd.Print();
					}
				}
			}
		}
	}
}
