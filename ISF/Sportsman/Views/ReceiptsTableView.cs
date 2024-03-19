using System;
using System.Linq;
using Sport.UI;
using Sportsman.Details;
using Sport.UI.Dialogs;
using Sport.UI.Controls;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Sportsman.Views
{
	/// <summary>
	/// Summary description for ReceiptsTableView.
	/// </summary>
	[Sport.UI.ViewPermissions(Sport.Types.Permission.Accounts)]
	public class ReceiptsTableView : TableView
	{
		private System.Windows.Forms.ToolBarButton tbbReceiptForm;
		private System.Windows.Forms.ToolBarButton tbbExportToExcel;

		public ReceiptsTableView()
		{
			Items.Add((int)Sport.Entities.Receipt.Fields.Id, "קוד", 30);
			Items.Add((int)Sport.Entities.Receipt.Fields.Number, "מספר", 80);
			Items.Add((int)Sport.Entities.Receipt.Fields.Region, "מחוז", 150);
			Items.Add((int)Sport.Entities.Receipt.Fields.Account, "חשבון", 180);
			Items.Add((int)Sport.Entities.Receipt.Fields.Sum, "סכום", 120);
			Items.Add((int)Sport.Entities.Receipt.Fields.Date, "תאריך", 120);
			Items.Add((int)Sport.Entities.Receipt.Fields.Remarks, "הערות", 400, 75);
			Items.Add((int)Sport.Entities.Receipt.Fields.Season, "עונה", 120);
			Items.Add((int)Sport.Entities.Receipt.Fields.LastModified, "ת' שינוי אחרון", 120);

			//
			// toolBar
			//
			tbbReceiptForm = new System.Windows.Forms.ToolBarButton();
			tbbReceiptForm.ImageIndex = (int)Sport.Resources.ColorImages.Receipt;
			tbbReceiptForm.Text = "קבלה";
			tbbReceiptForm.Enabled = false;

			tbbExportToExcel = new System.Windows.Forms.ToolBarButton();
			tbbExportToExcel.ImageIndex = (int)Sport.Resources.ColorImages.HashavshevetExport;
			tbbExportToExcel.Text = "ייצוא";

			toolBar.Buttons.Add(tbbReceiptForm);
			toolBar.Buttons.Add(tbbExportToExcel);
			toolBar.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(ToolBarButtonClicked);

			// search
			SearchBarEnabled = true;

			// delete
			this.CanDelete = false;
		}

		private void ToolBarButtonClicked(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			if (e.Button == tbbReceiptForm)
			{
				Sport.Data.Entity entity = Current;
				if (entity != null)
				{
					PrintReceipt(new Sport.Entities.Receipt(entity));
				}
			}
			else if (e.Button == tbbExportToExcel)
			{
				ShowExportReceiptsDialog();
			}
		}

		protected override void OnSelectEntity(Sport.Data.Entity entity)
		{
			tbbReceiptForm.Enabled = entity != null;
		}

		//protected override void OnCurrentChanged(Sport.Data.Entity entity)
		//{
		//	tbbReceiptForm.Enabled = entity != null;
		//}

		#region Filters

		private ComboBoxFilter regionFilter;
		private ComboBoxFilter accountFilter;

		private void CreateFilters()
		{
			regionFilter = new ComboBoxFilter("מחוז:", Sport.Entities.Region.Type.GetEntities(null), null, "<בחר מחוז>", 150);
			//regionFilter.Parameters.Add(EntityListView.EntityQuery.Parameters[0]);
			regionFilter.FilterChanged += new EventHandler(RegionFiltered);
			Filters.Add(regionFilter);

			//Sport.Data.EntityQuery accountQuery = new Sport.Data.EntityQuery(Sport.Entities.Account.Type);
			//accountQuery.Parameters.Add((int) Sport.Entities.Account.Fields.Region);
			Sport.Entities.Account[] accounts = Region == null ? null : Region.GetAccounts();
			accountFilter = new ComboBoxFilter("חשבון:", accounts, Account, "<כל החשבונות>", 260);
			//accountFilter.ValuesQuery = accountQuery;
			//accountFilter.Parameters.Add(EntityListView.EntityQuery.Parameters[1]);
			accountFilter.FilterChanged += new EventHandler(AccountFiltered);
			Filters.Add(accountFilter);

			//regionFilter.Parameters.Add(accountQuery.Parameters[0]);

			regionFilter.Value = Region;
			accountFilter.Value = Account;
		}

		private void RegionFiltered(object sender, EventArgs e)
		{
			if (regionFilter.Value == null)
			{
				State[Sport.Entities.Region.TypeName] = null;
				accountFilter.SetValues(null);
			}
			else
			{
				int regionID = ((Sport.Data.Entity)regionFilter.Value).Id;
				State[Sport.Entities.Region.TypeName] = regionID.ToString();
				accountFilter.SetValues((new Sport.Entities.Region(regionID)).GetAccounts());
			}

			Requery();
		}

		private void AccountFiltered(object sender, EventArgs e)
		{
			if (accountFilter.Value == null)
			{
				State[Sport.Entities.Account.TypeName] = null;
			}
			else
			{
				State[Sport.Entities.Account.TypeName] = ((Sport.Entities.Account)accountFilter.Value).Id.ToString();
			}

			Requery();
		}

		#endregion

		public override void Open()
		{
			Title = "קבלות";

			EntityListView = new Sport.UI.EntityListView(Sport.Entities.Receipt.TypeName);
			EntityListView.Fields[(int)Sport.Entities.Receipt.Fields.Region].Values = Sport.Entities.Region.Type.GetEntities(null);
			EntityListView.Fields[(int)Sport.Entities.Receipt.Fields.Season].Values = Sport.Entities.Season.Type.GetEntities(null);
			EntityListView.Fields[(int)Sport.Entities.Receipt.Fields.Number].CanEdit = false;
			EntityListView.Fields[(int)Sport.Entities.Receipt.Fields.Region].CanEdit = false;
			EntityListView.Fields[(int)Sport.Entities.Receipt.Fields.Account].CanEdit = false;
			EntityListView.Fields[(int)Sport.Entities.Receipt.Fields.Sum].CanEdit = false;
			EntityListView.Fields[(int)Sport.Entities.Receipt.Fields.LastModified].CanEdit = false;

			// Default columns
			Columns = new int[] { 
									(int) Sport.Entities.Receipt.Fields.Number,
									(int) Sport.Entities.Receipt.Fields.Account,
									(int) Sport.Entities.Receipt.Fields.Sum,
									(int) Sport.Entities.Receipt.Fields.Date
								};

			//add search items:
			Searcher objReceiptSearch = new Searcher("מספר קבלה:",
				EntityListView.EntityType.Fields[(int)Sport.Entities.Receipt.Fields.Number], 90);
			Searcher objAccountSearch = new Searcher("שם חשבון:",
				EntityListView.EntityType.Fields[(int)Sport.Entities.Receipt.Fields.Account], 120);
			Searchers.Add(objReceiptSearch);
			Searchers.Add(objAccountSearch);

			// Default sort columns
			Sort = new int[] {
								 (int) Sport.Entities.Receipt.Fields.Account,
								 (int) Sport.Entities.Receipt.Fields.Date
							 };

			// Details fields
			Details = new int[] {
									(int) Sport.Entities.Receipt.Fields.Remarks
								};

			//DetailsView = typeof(Forms.ReceiptForm);

			//
			// Query
			//
			//EntityListView.EntityQuery.Parameters.Add((int) Sport.Entities.Receipt.Fields.Region);
			//EntityListView.EntityQuery.Parameters.Add((int) Sport.Entities.Receipt.Fields.Account, false);

			//
			// Filters
			//
			CreateFilters();

			Sport.Entities.Region region = Region;
			if (region != null)
			{
				regionFilter.Value = region;
				//EntityListView.EntityQuery.Parameters[0].Value = region;
			}

			Sport.Entities.Account account = Account;
			if (account != null)
			{
				accountFilter.Value = account;
				//EntityListView.EntityQuery.Parameters[1].Value = account;
			}

			//objAccountSearch.AffectedFilter = accountFilter;

			base.Open();
		}

		#region State Properties

		public new Sport.Entities.Region Region
		{
			get
			{
				if (State[Sport.Entities.Region.TypeName] == null)
					return null;

				return new Sport.Entities.Region(Int32.Parse(State[Sport.Entities.Region.TypeName]));
			}
			set
			{
				if (value == null)
					State[Sport.Entities.Region.TypeName] = null;
				else
					State[Sport.Entities.Region.TypeName] = value.Id.ToString();
			}
		}

		public Sport.Entities.Account Account
		{
			get
			{
				if (State[Sport.Entities.Account.TypeName] == null)
					return null;

				return new Sport.Entities.Account(Int32.Parse(State[Sport.Entities.Account.TypeName]));
			}
			set
			{
				if (value == null)
					State[Sport.Entities.Account.TypeName] = null;
				else
					State[Sport.Entities.Account.TypeName] = value.Id.ToString();
			}
		}

		#endregion

		protected override void DeleteEntity()
		{
			/*
			if (Current != null)
			{
				Sport.Entities.Receipt receipt = new Sport.Entities.Receipt(Current);
				Sport.Entities.Credit[] credits = receipt.GetCredits();
				for (int i = 0; i < credits.Length; i++)
				{
					credits[i].Entity.Delete();
				}
			}
			*/
			base.DeleteEntity();
		}


		protected override void NewEntity()
		{
			Sport.Entities.Region region = Region;
			Sport.Entities.Account account = Account;

			Forms.ReceiptForm rf = new Forms.ReceiptForm(region, account);
			if (rf.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				Region = rf.Region;
				regionFilter.Value = Region;
				Account = rf.Account;
				Sport.Entities.Account[] accounts = Region == null ? null : Region.GetAccounts();
				accountFilter.SetValues(accounts);
				//accountFilter.RequeryValues();
				accountFilter.Value = Account;
				if (rf.Receipt != null)
				{
					if (Sport.UI.MessageBox.Ask("האם ברצונך להדפיס קבלה '" + rf.Receipt.Name + "'?", true))
						PrintReceipt(rf.Receipt);
				}
			}
		}

		protected override void OpenDetails(Sport.Data.Entity entity)
		{
			Sport.Entities.Receipt receipt = new Sport.Entities.Receipt(entity);
			Forms.ReceiptForm rf = new Forms.ReceiptForm(receipt);
			rf.ShowDialog();
		}

		private Sport.Documents.Document CreateReceiptForm(System.Drawing.Printing.PrinterSettings settings,
			Sport.Entities.Receipt receipt, bool blnRegionChange)
		{
			Documents.PaymentDocuments paymentDocuments = new Documents.PaymentDocuments(settings);

			Sport.Documents.DocumentBuilder db = paymentDocuments.CreateDocumentBuilder("קבלה מספר " + receipt.Number);

			string strRegion = "";
			if (blnRegionChange)
			{
				strRegion = new Sport.Entities.Region(Core.UserManager.CurrentUser.UserRegion).Name;
			}
			else
			{
				if (receipt != null)
					strRegion = receipt.Region.Name;
			}
			paymentDocuments.CreateReceiptSection(db, receipt, 3, strRegion,
				new Sport.Entities.Season(Sport.Core.Session.Season));

			Sport.Documents.Document result = db.CreateDocument();
			result.Border = System.Drawing.SystemPens.WindowFrame;
			return result;
		}

		private void PrintReceipt(Sport.Entities.Receipt receipt)
		{
			System.Drawing.Printing.PrinterSettings ps;
			Sport.UI.Dialogs.PrintSettingsForm settingsForm;
			if (!Sport.UI.Helpers.GetPrinterSettings(out ps, out settingsForm))
			{
				return;
			}

			settingsForm.AllowRegionChange = true;
			if (settingsForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				Sport.Documents.Document document =
					CreateReceiptForm(ps, receipt, settingsForm.ForceRegionChecked);

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

		/*
		public override void OnContextMenu(Sport.UI.TableView2.SelectionType selectionType, Sport.UI.Controls.RightContextMenu menu)
		{
			base.OnContextMenu(selectionType, menu);

			if (selectionType == Sport.UI.TableView2.SelectionType.Single)
			{
				Sport.Entities.Receipt receipt = new Sport.Entities.Receipt(Current);
				if (receipt.Account.School != null)
					menu.MenuItems.Add(new System.Windows.Forms.MenuItem("בית ספר", new System.EventHandler(ReceiptSchoolClicked)));

			}
		}
		*/

		private void ReceiptsDetailsClick(object sender, EventArgs e)
		{
			OpenDetails(this.Current);
		}

		/// <summary>
		/// build the context menu items
		/// </summary>
		public override MenuItem[] GetContextMenu(Sport.UI.TableView.SelectionType selectionType)
		{
			List<MenuItem> menuItems = new List<MenuItem>();
			if (this.Current != null)
			{
				switch (selectionType)
				{
					case (SelectionType.Single):
						Sport.Entities.Receipt receipt = new Sport.Entities.Receipt(Current);
						bool gotSchool = receipt.Account.School != null;
						MenuItem detailsItem = new MenuItem("פתח", new System.EventHandler(ReceiptsDetailsClick));
						detailsItem.DefaultItem = true;
						menuItems.Add(detailsItem);
						if (gotSchool)
						{
							menuItems.Add(new MenuItem("-"));
							menuItems.Add(new MenuItem("בית ספר", new System.EventHandler(ReceiptSchoolClicked)));
						}
						break;
				}
			}

			return menuItems.ToArray();
		}

		private void ReceiptSchoolClicked(object sender, EventArgs e)
		{
			Sport.Data.Entity entity = Current;
			if (entity != null)
			{
				Sport.Entities.Receipt receipt = new Sport.Entities.Receipt(entity);
				Sport.Entities.School school = receipt.Account.School;
				if (school != null)
				{
					new OpenDialogCommand().Execute(typeof(SchoolDetailsView), "id=" + school.Id);
				}
			}
		}

		private void ShowExportReceiptsDialog()
		{
			string strLastSelectedNumber = Sport.Core.Configuration.ReadString("General", "LastReceiptExportNumber");
			int lastSelectedNumber = 0;
			Int32.TryParse(strLastSelectedNumber, out lastSelectedNumber);
			if (lastSelectedNumber < 0)
				lastSelectedNumber = 0;
			GenericItem item = new GenericItem("התחל ייצוא מקבלה מספר:", GenericItemType.Number, lastSelectedNumber);
			GenericEditDialog dialog = new GenericEditDialog("בחירת מספר קבלה", new GenericItem[] { item });
			dialog.SetAcceptButton();
			if (dialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
			{
				int startReceiptNumber = Int32.Parse(dialog.Items[0].Value.ToString());
				ExportReceiptsDialog dlgExport = new ExportReceiptsDialog(startReceiptNumber);
				if (dlgExport.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
				{
					Sport.Core.Configuration.WriteString("General", "LastReceiptExportNumber", startReceiptNumber.ToString());
				}
			}
		}

		private void Requery()
		{
			string title = "קבלות";
			object region = Core.Tools.GetStateValue(State[Sport.Entities.Region.TypeName]);
			object account = Core.Tools.GetStateValue(State[Sport.Entities.Account.TypeName]);
			if (region == null)
			{
				EntityListView.Clear();
			}
			else
			{
				int regionID = (int)region;
				title += " - " + (new Sport.Entities.Region(regionID)).Name;
				Sport.Data.EntityFilter filter = new Sport.Data.EntityFilter(
					(int)Sport.Entities.Receipt.Fields.Region, regionID);
				if (account != null)
				{
					int accountID = (int)account;
					filter.Add(new Sport.Data.EntityFilterField((int)Sport.Entities.Receipt.Fields.Account, accountID));
				}
				EntityListView.Read(filter);
			}
			this.Title = title;
			//grid.Sort(new int[] { (int) Sport.Entities.City.Fields.Region, (int) Sport.Entities.City.Fields.Name });
		}
	}
}
