using System;
using System.Collections;
using Sport.UI;

using Sportsman.Details;

namespace Sportsman.Views
{
	/// <summary>
	/// Summary description for AccountEntriesTableView.
	/// </summary>
	[Sport.UI.ViewPermissions(Sport.Types.Permission.Accounts)]
	public class AccountEntriesTableView : TableView2
	{
		private string _lastPanelMessage = "";
		private System.Windows.Forms.ToolBarButton tbbDebtorsReport = null;
		private System.Windows.Forms.ToolBarButton tbbAccountReport = null;

		public AccountEntriesTableView()
			: base(new Entities.AccountEntryView())
		{
			//
			// toolBar
			//
			tbbDebtorsReport = new System.Windows.Forms.ToolBarButton();
			tbbDebtorsReport.ImageIndex = (int)Sport.Resources.ColorImages.Receipt;
			tbbDebtorsReport.Text = "דו\"ח חייבים";
			tbbDebtorsReport.Enabled = false;
			tbbAccountReport = new System.Windows.Forms.ToolBarButton();
			tbbAccountReport.ImageIndex = (int)Sport.Resources.ColorImages.Receipt;
			tbbAccountReport.Text = "דו\"ח מצב חשבון";
			tbbAccountReport.Enabled = false;

			toolBar.Buttons.Add(tbbDebtorsReport);
			toolBar.Buttons.Add(tbbAccountReport);
			toolBar.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(ToolBarButtonClicked);

			// search
			SearchBarEnabled = true;
		}

		private void ToolBarButtonClicked(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			if (e.Button == tbbDebtorsReport)
				PrintDebtorsReport();
			else if (e.Button == tbbAccountReport)
				PrintAccountReport();
		}

		#region Filters
		private ComboBoxFilter regionFilter;
		private ComboBoxFilter accountFilter;

		private void CreateFilters()
		{
			regionFilter = new ComboBoxFilter("מחוז:", Sport.Entities.Region.Type.GetEntities(null), null, "<בחר מחוז>", 150);
			regionFilter.Parameters.Add(EntityListView.EntityQuery.Parameters[0]);
			regionFilter.FilterChanged += new EventHandler(RegionFiltered);
			Filters.Add(regionFilter);

			Sport.Data.EntityQuery accountQuery = new Sport.Data.EntityQuery(Sport.Entities.Account.Type);
			accountQuery.Parameters.Add((int)Sport.Entities.Account.Fields.Region);
			Sport.Entities.Account[] accounts = Region == null ? null : Region.GetAccounts();
			accountFilter = new ComboBoxFilter("חשבון:", accounts, Account, "<כל החשבונות>", 260);
			accountFilter.ValuesQuery = accountQuery;
			accountFilter.Parameters.Add(EntityListView.EntityQuery.Parameters[1]);
			accountFilter.FilterChanged += new EventHandler(AccountFiltered);
			Filters.Add(accountFilter);

			regionFilter.Parameters.Add(accountQuery.Parameters[0]);

			regionFilter.Value = Region;
			accountFilter.Value = Account;
		}

		private void RegionFiltered(object sender, EventArgs e)
		{
			if (regionFilter.Value == null)
				State[Sport.Entities.Region.TypeName] = null;
			else
				State[Sport.Entities.Region.TypeName] = ((Sport.Data.Entity)regionFilter.Value).Id.ToString();
			tbbDebtorsReport.Enabled = (regionFilter.Value != null);
		}

		private void AccountFiltered(object sender, EventArgs e)
		{
			string strBalance = "";
			if (accountFilter.Value == null)
			{
				State[Sport.Entities.Account.TypeName] = null;
			}
			else
			{
				int accountId = ((Sport.Data.Entity)accountFilter.Value).Id;
				if (regionFilter.Value != null && ((Sport.Data.Entity)regionFilter.Value).Id != Sport.Entities.Region.CentralRegion)
				{
					Sport.Data.EntityFilter filter = new Sport.Data.EntityFilter();
					filter.Add(new Sport.Data.EntityFilterField((int)Sport.Entities.AccountEntry.Fields.Account, accountId));
					filter.Add(new Sport.Data.EntityFilterField((int)Sport.Entities.AccountEntry.Fields.Region, Sport.Entities.Region.CentralRegion));
					Sport.Data.Entity[] accountEntries = Sport.Entities.AccountEntry.Type.GetEntities(filter);
					if (accountEntries != null && accountEntries.Length > 0)
					{
						foreach (Sport.Data.Entity accountEntry in accountEntries)
						{
							this.EntityListView.EntityList.Add(accountEntry);
						}
					}
				}
				State[Sport.Entities.Account.TypeName] = accountId.ToString();
				double balance = 0;
				for (int i = 0; i < this.EntityListView.Count; i++)
					balance += Sport.Common.Tools.CDblDef(this.EntityListView[i].Fields[(int)Sport.Entities.AccountEntry.Fields.Sum], 0);
				if (this.EntityListView.Count > 0)
				{
					strBalance = " ₪ מאזן: ";
					if (balance < 0)
					{
						strBalance += "מינוס ";
					}
					strBalance += Math.Abs(balance);
				}
			}
			Sportsman.Context.SetStatusBar(Forms.MainForm.StatusBarPanels.Error, strBalance);
			tbbAccountReport.Enabled = (State[Sport.Entities.Account.TypeName] != null);
		}
		#endregion

		public override void Open()
		{
			_lastPanelMessage =
				Sportsman.Context.GetStatusText(Forms.MainForm.StatusBarPanels.Error);

			// Default columns
			Columns = new int[] { 
									(int) Sport.Entities.AccountEntry.Fields.Account,
									(int) Sport.Entities.AccountEntry.Fields.EntryType,
									(int) Sport.Entities.AccountEntry.Fields.Description,
									(int) Sport.Entities.AccountEntry.Fields.Sum,
									(int) Sport.Entities.AccountEntry.Fields.EntryDate
								};

			//add search items:
			Searcher objAccountSearch = new Searcher("שם חשבון:", EntityListView.EntityType.Fields[(int)Sport.Entities.AccountEntry.Fields.Account], 120);
			Searchers.Add(objAccountSearch);

			// Default sort columns
			Sort = new int[] {
								 (int) Sport.Entities.AccountEntry.Fields.Account,
								 (int) Sport.Entities.AccountEntry.Fields.EntryDate
							 };

			DetailsBarEnabled = false;

			//DetailsView = typeof(Details.TeamDetailsView);

			//
			// Query
			//
			EntityListView.EntityQuery.Parameters.Add((int)Sport.Entities.AccountEntry.Fields.Region);
			EntityListView.EntityQuery.Parameters.Add((int)Sport.Entities.AccountEntry.Fields.Account, false);

			//
			// Filters
			//
			CreateFilters();

			Sport.Entities.Region region = Region;
			if (region != null)
			{
				regionFilter.Value = region;
				EntityListView.EntityQuery.Parameters[0].Value = region;
			}

			Sport.Entities.Account account = Account;
			if (account != null)
			{
				accountFilter.Value = account;
				EntityListView.EntityQuery.Parameters[1].Value = account;
			}

			CanInsert = false;
			CanDelete = false;

			objAccountSearch.AffectedFilter = accountFilter;

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

		public override void OnContextMenu(Sport.UI.TableView2.SelectionType selectionType, Sport.UI.Controls.RightContextMenu menu)
		{
			base.OnContextMenu(selectionType, menu);

			if (selectionType == Sport.UI.TableView2.SelectionType.Single)
			{
				Sport.Entities.AccountEntry accountEntry = new Sport.Entities.AccountEntry(Current);
				if (accountEntry.Account.School != null)
					menu.MenuItems.Add(new System.Windows.Forms.MenuItem("בית ספר", new System.EventHandler(AccountEntrySchoolClicked)));
				if (accountEntry.Receipt != null)
					menu.MenuItems.Add(new System.Windows.Forms.MenuItem("קבלה", new System.EventHandler(AccountEntryReceiptClicked)));
				if (accountEntry.EntryType == Sport.Types.AccountEntryType.Debit)
					menu.MenuItems.Add(new System.Windows.Forms.MenuItem("חיוב", new System.EventHandler(AccountEntryChargeClicked)));
			}
		}

		private void AccountEntrySchoolClicked(object sender, EventArgs e)
		{
			Sport.Data.Entity entity = Current;
			if (entity != null)
			{
				Sport.Entities.AccountEntry accountEntry = new Sport.Entities.AccountEntry(entity);
				Sport.Entities.School school = accountEntry.Account.School;
				if (school != null)
				{
					new OpenDialogCommand().Execute(typeof(SchoolDetailsView), "id=" + school.Id);
				}
			}
		}

		private void AccountEntryReceiptClicked(object sender, EventArgs e)
		{
			Sport.Data.Entity entity = Current;
			if (entity != null)
			{
				Sport.Entities.AccountEntry accountEntry = new Sport.Entities.AccountEntry(entity);
				Sport.Entities.Receipt receipt = accountEntry.Receipt;
				if (receipt != null)
				{
					Forms.ReceiptForm rf = new Forms.ReceiptForm(receipt);
					rf.ShowDialog();
				}
			}
		}

		private void AccountEntryChargeClicked(object sender, EventArgs e)
		{
			Sport.Data.Entity entity = Current;
			if (entity != null)
			{
				Sport.Entities.AccountEntry accountEntry = new Sport.Entities.AccountEntry(entity);
				string state = "id=" + (accountEntry.Id / 10);
				ViewManager.OpenView(typeof(ChargesTableView), state);
			}
		}

		public override void Close()
		{
			Sportsman.Context.SetStatusBar(Forms.MainForm.StatusBarPanels.Error,
				_lastPanelMessage);
			base.Close();
		}

		public override void Activate()
		{
			_lastPanelMessage =
				Sportsman.Context.GetStatusText(Forms.MainForm.StatusBarPanels.Error);
			base.Activate();
		}

		public override void Deactivate()
		{
			Sportsman.Context.SetStatusBar(Forms.MainForm.StatusBarPanels.Error,
				_lastPanelMessage);
			base.Deactivate();
		}

		private void PrintDebtorsReport()
		{
			//get selected region:
			Sport.Entities.Region region = this.Region;

			//got anything?
			if (region == null)
				return;

			System.Drawing.Printing.PrinterSettings ps;
			Sport.UI.Dialogs.PrintSettingsForm settingsForm;
			if (!Sport.UI.Helpers.GetPrinterSettings(out ps, out settingsForm))
			{
				return;
			}

			//get accounts for this region:
			Sport.Data.EntityFilter filter = new Sport.Data.EntityFilter(
				(int)Sport.Entities.AccountEntry.Fields.Region, region.Id);
			Sport.Data.Entity[] entities =
				Sport.Entities.AccountEntry.Type.GetEntities(filter);
			Sport.Entities.AccountEntry[] accounts = null;
			if (entities != null)
			{
				accounts = new Sport.Entities.AccountEntry[entities.Length];
				for (int i = 0; i < entities.Length; i++)
					accounts[i] = new Sport.Entities.AccountEntry(entities[i]);
			}
			if ((accounts == null) || (accounts.Length == 0))
			{
				Sport.UI.MessageBox.Error("במחוז זה אין חשבונות", "דו\"ח חייבים");
				return;
			}

			//build balance list:
			Hashtable tblAccountBalance = new Hashtable();
			foreach (Sport.Entities.AccountEntry account in accounts)
			{
				Sport.Entities.Account key = account.Account;
				double sum = account.Sum;
				if (tblAccountBalance[key] == null)
					tblAccountBalance[key] = ((double)0);
				tblAccountBalance[key] = ((double)tblAccountBalance[key]) + sum;
			}

			//get negative accounts:
			ArrayList arrAccounts = new ArrayList();
			foreach (object curKey in tblAccountBalance.Keys)
			{
				double curBalance = (double)tblAccountBalance[curKey];
				if (curBalance < 0)
					arrAccounts.Add(curKey);
			}
			if (arrAccounts.Count == 0)
			{
				Sport.UI.MessageBox.Error("לא נמצאו חשבונות בעלי מאזן שלילי",
					"דו\"ח חייבים");
				return;
			}

			//let user choose settings:
			if (settingsForm.ShowDialog() != System.Windows.Forms.DialogResult.OK)
				return;

			//create the document:
			Sport.Documents.Document document = CreateDebtorsReport(ps,
				(Sport.Entities.Account[])arrAccounts.ToArray(typeof(Sport.Entities.Account)));
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

		private void PrintAccountReport()
		{
			//get selected account:
			Sport.Entities.Account account = this.Account;

			//got anything?
			if (account == null)
				return;

			System.Drawing.Printing.PrinterSettings ps;
			Sport.UI.Dialogs.PrintSettingsForm settingsForm;
			if (!Sport.UI.Helpers.GetPrinterSettings(out ps, out settingsForm))
			{
				return;
			}

			//get entries for this account:
			Sport.Data.EntityFilter filter = new Sport.Data.EntityFilter(
				(int)Sport.Entities.AccountEntry.Fields.Account, account.Id);
			Sport.Data.Entity[] entities =
				Sport.Entities.AccountEntry.Type.GetEntities(filter);
			Sport.Entities.AccountEntry[] entries = null;
			if (entities != null)
			{
				entries = new Sport.Entities.AccountEntry[entities.Length];
				for (int i = 0; i < entities.Length; i++)
					entries[i] = new Sport.Entities.AccountEntry(entities[i]);
			}

			if ((entries == null) || (entries.Length == 0))
			{
				Sport.UI.MessageBox.Error("לחשבון זה אין תנועות", "דו\"ח מצב חשבון");
				return;
			}

			//let user choose settings:
			if (settingsForm.ShowDialog() != System.Windows.Forms.DialogResult.OK)
				return;

			//create the document:
			Sport.Documents.Document document = CreateAccountReport(ps, account, entries);
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

		private Sport.Documents.Document CreateDebtorsReport(
			System.Drawing.Printing.PrinterSettings settings,
			Sport.Entities.Account[] accounts)
		{
			Documents.PaymentDocuments paymentDocuments = new Documents.PaymentDocuments(settings);

			Sport.Documents.DocumentBuilder db = paymentDocuments.CreateDocumentBuilder("דו\"ח חייבים");
			paymentDocuments.CreateDebtorsSection(db, accounts);

			return db.CreateDocument();
		}

		private Sport.Documents.Document CreateAccountReport(System.Drawing.Printing.PrinterSettings settings,
			Sport.Entities.Account account, Sport.Entities.AccountEntry[] entries)
		{
			Documents.PaymentDocuments paymentDocuments = new Documents.PaymentDocuments(settings);

			Sport.Documents.DocumentBuilder db = paymentDocuments.CreateDocumentBuilder("דו\"ח מצב חשבון");
			paymentDocuments.CreateAccountSection(db, account, entries);

			return db.CreateDocument();
		}
	}
}
