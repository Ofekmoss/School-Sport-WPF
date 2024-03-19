using System;
using System.Collections;
using Sport.UI;

using Sportsman.Details;

namespace Sportsman.Views
{
	/// <summary>
	/// Summary description for ChargesTableView.
	/// </summary>
	[Sport.UI.ViewPermissions(Sport.Types.Permission.Accounts)]
	public class ChargesTableView : TableView2
	{
		private System.Windows.Forms.ToolBarButton tbbChargeForm;

		public ChargesTableView()
			: base(new Entities.ChargeView())
		{
			//
			// toolBar
			//
			tbbChargeForm = new System.Windows.Forms.ToolBarButton();
			tbbChargeForm.ImageIndex = (int)Sport.Resources.ColorImages.Receipt;
			tbbChargeForm.Text = "טופס חיוב";
			tbbChargeForm.Enabled = false;

			toolBar.Buttons.Add(tbbChargeForm);
			toolBar.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(ToolBarButtonClicked);

			// search
			SearchBarEnabled = true;
		}

		private void ToolBarButtonClicked(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			if (e.Button == tbbChargeForm)
			{
				PrintChargeForm();
			}
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
			{
				State[Sport.Entities.Region.TypeName] = null;
			}
			else
			{
				State[Sport.Entities.Region.TypeName] = ((Sport.Data.Entity)regionFilter.Value).Id.ToString();
			}
			//ApplyChargeChampionships();
		}

		private void AccountFiltered(object sender, EventArgs e)
		{
			if (accountFilter.Value == null)
			{
				State[Sport.Entities.Account.TypeName] = null;
				tbbChargeForm.Enabled = false;
			}
			else
			{
				State[Sport.Entities.Account.TypeName] = ((Sport.Data.Entity)accountFilter.Value).Id.ToString();
				tbbChargeForm.Enabled = true;
			}
			//ApplyChargeChampionships();
		}
		#endregion

		private void ApplyChargeChampionships()
		{
			//got list?
			if (this.EntityListView == null)
				return;

			//get count:
			int count = this.EntityListView.Count;

			//got anything?
			if (count == 0)
				return;

			//remove those in different season.
			ArrayList arrDifferentSeason = new ArrayList();
			int[] charges = new int[count];
			for (int i = 0; i < count; i++)
				charges[i] = this.EntityListView[i].Id;
			AdvancedServices.AdvancedService service = new AdvancedServices.AdvancedService();
			int[] differentSeason = service.GetDifferentSeasonCharges(charges, Sport.Core.Session.Season);

			if (differentSeason != null && differentSeason.Length > 0)
			{
				ArrayList alDifferentSeason = new ArrayList(differentSeason);
				for (int i = 0; i < count; i++)
				{
					Sport.Data.Entity entity = this.EntityListView[i];
					if (alDifferentSeason.IndexOf(entity.Id) >= 0)
						arrDifferentSeason.Add(entity);
				}
				foreach (Sport.Data.Entity entToRemove in arrDifferentSeason)
					this.EntityListView.EntityList.Remove(entToRemove);
			}

			count = this.EntityListView.Count;
			if (count == 0)
				return;

			charges = new int[count];
			for (int i = 0; i < count; i++)
				charges[i] = this.EntityListView[i].Id;

			//put championships
			int[] arrChampCategories = null;
			int tryCount = 0;
			while (arrChampCategories == null && tryCount < 3)
			{
				try
				{
					arrChampCategories = service.GetChargesChampionships(charges, Sport.Core.Session.Season);
				}
				catch
				{
				}
				tryCount++;
				if (arrChampCategories == null)
					System.Threading.Thread.Sleep(500);
			}

			if (arrChampCategories == null)
				return;

			Hashtable tblChargeChampionships = new Hashtable();
			for (int i = 0; i < count; i++)
			{
				Sport.Data.Entity entity = this.EntityListView[i];
				if (entity != null && entity.Id >= 0 && entity.Fields[(int)Sport.Entities.Charge.Fields.ChampionshipCategory] == null)
					tblChargeChampionships[entity] = arrChampCategories[i];
			}

			//got charges?
			if (tblChargeChampionships.Count == 0)
				return;

			//iterate over charges, try to apply championship to each:
			Sport.UI.Dialogs.WaitForm.ShowWait("מעדכן אליפויות אנא המתן...", true);
			double chargeProgress = (((double)100) / ((double)tblChargeChampionships.Count));
			double curProgress = 0;
			foreach (Sport.Data.Entity entCharge in tblChargeChampionships.Keys)
			{
				//get current charge:
				Sport.Entities.Charge curCharge = new Sport.Entities.Charge(entCharge);

				//update championship:
				int category = (int)tblChargeChampionships[entCharge];
				curCharge.Edit();
				curCharge.ChampionshipCategory = new Sport.Entities.ChampionshipCategory(category);
				curCharge.Save();

				//advance.
				curProgress += chargeProgress;
				Sport.UI.Dialogs.WaitForm.SetProgress((int)curProgress);
			}
			Sport.UI.Dialogs.WaitForm.HideWait();
		}

		public override void Open()
		{
			//Sport.Entities.ChampionshipCategory.ShowFullName = true;

			// Default columns
			if (State[Sport.UI.View.SelectionDialog] == "1")
			{
				Columns = new int[] { 
										(int) Sport.Entities.Charge.Fields.Account,
										(int) Sport.Entities.Charge.Fields.Product,
										(int) Sport.Entities.Charge.Fields.PriceTotal,
										(int) Sport.Entities.Charge.Fields.ChampionshipCategory,
										(int) Sport.Entities.Charge.Fields.Date, 
									};
			}
			else
			{
				Columns = new int[] { 
										(int) Sport.Entities.Charge.Fields.Account,
										(int) Sport.Entities.Charge.Fields.Product,
										(int) Sport.Entities.Charge.Fields.Amount,
										(int) Sport.Entities.Charge.Fields.Price,
										(int) Sport.Entities.Charge.Fields.PriceTotal,
										(int) Sport.Entities.Charge.Fields.ChampionshipCategory,
										(int) Sport.Entities.Charge.Fields.Date, 
										(int) Sport.Entities.Charge.Fields.Status
									};
			}

			//add search items:
			Searcher objAccountSearch = new Searcher("שם חשבון:", EntityListView.EntityType.Fields[(int)Sport.Entities.Charge.Fields.Account], 120);
			Searchers.Add(objAccountSearch);

			// Default sort columns
			Sort = new int[] {
								 (int) Sport.Entities.Charge.Fields.Account,
								 (int) Sport.Entities.Charge.Fields.Date
							 };

			DetailsBarEnabled = false;

			//DetailsView = typeof(Details.TeamDetailsView);

			//
			// Query
			//
			EntityListView.EntityQuery.Parameters.Add((int)Sport.Entities.Charge.Fields.Region);
			EntityListView.EntityQuery.Parameters.Add((int)Sport.Entities.Charge.Fields.Account, false);

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

			objAccountSearch.AffectedFilter = accountFilter;

			base.Open();

			CheckAccounts();
			CheckSelectedCharge();
		}

		private void CheckAccounts()
		{
			if (State["accounts"] == null)
				return;

			this.Title = "בחירת חיובים לזיכוי";
			ArrayList arrAccounts = new ArrayList(Sport.Common.Tools.ToIntArray(State["accounts"], ','));
			ArrayList arrToRemove = new ArrayList();
			for (int i = 0; i < this.EntityListView.Count; i++)
			{
				Sport.Data.Entity curCharge = this.EntityListView[i];
				if (Sport.Common.Tools.CIntDef(curCharge.Fields[(int)Sport.Entities.Charge.Fields.Status], -1) ==
					(int)Sport.Types.ChargeStatusType.Paid)
				{
					arrToRemove.Add(curCharge);
					continue;
				}
				object oAccount = curCharge.Fields[(int)Sport.Entities.Charge.Fields.Account];
				int curAccount = -1;
				if (oAccount != null)
					curAccount = (int)oAccount;
				if (arrAccounts.IndexOf(curAccount) < 0)
					arrToRemove.Add(curCharge);
			}

			foreach (Sport.Data.Entity entToRemove in arrToRemove)
				this.EntityListView.EntityList.Remove(entToRemove);
		}

		private void CheckSelectedCharge()
		{
			if (State["id"] == null)
				return;

			int id = Int32.Parse(State["id"]);
			Sport.Entities.Charge charge = null;
			try
			{
				charge = new Sport.Entities.Charge(id);
			}
			catch
			{ }

			if (charge != null && charge.Id > 0)
			{
				regionFilter.Value = charge.Region.Entity;
				accountFilter.Value = charge.Account.Entity;
				this.Current = charge.Entity;
			}
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

		protected override void NewEntity()
		{
			Sport.Entities.Region region = Region;
			Sport.Entities.Account account = Account;

			Forms.CreateChargesForm ccf = new Forms.CreateChargesForm(region, account);
			if (ccf.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				Region = ccf.Region;

				regionFilter.Value = Region;

				Account = ccf.Account;

				accountFilter.RequeryValues();

				accountFilter.Value = Account;

				if (EntityListView.Count > 0 && Sport.UI.MessageBox.Ask("האם לעבור למסך קבלות?", System.Windows.Forms.MessageBoxIcon.Question, true))
				{
					this.Current = EntityListView[0];
					ChargeReceiptClicked(null, EventArgs.Empty);
				}
			}
		}

		public override void Close()
		{
			Sport.Entities.ChampionshipCategory.Type.NameField =
				Sport.Entities.ChampionshipCategory.Type[(int)Sport.Entities.ChampionshipCategory.Fields.Category];
			base.Close();
		}

		public override void Activate()
		{
			Sport.Entities.ChampionshipCategory.Type.NameField =
				new Sport.Data.FormatEntityField(
				Sport.Entities.ChampionshipCategory.Type, "{0} {1}",
				new int[] { 
					(int) Sport.Entities.ChampionshipCategory.Fields.Championship, 
					(int) Sport.Entities.ChampionshipCategory.Fields.Category });

			base.Activate();
		}

		public override void Deactivate()
		{
			Sport.Entities.ChampionshipCategory.Type.NameField =
				Sport.Entities.ChampionshipCategory.Type[(int)Sport.Entities.ChampionshipCategory.Fields.Category];
			base.Deactivate();
		}


		private Sport.Documents.Document CreateChargeForm(System.Drawing.Printing.PrinterSettings settings,
			Sport.Entities.Charge[] charges)
		{
			Documents.PaymentDocuments paymentDocuments = new Documents.PaymentDocuments(settings);

			Sport.Documents.DocumentBuilder db = paymentDocuments.CreateDocumentBuilder("טופס חיוב");
			paymentDocuments.CreateChargesSection(db, charges);

			return db.CreateDocument();
		}

		private Sport.Entities.Charge[] SelectCharges(Sport.Entities.Account account)
		{
			Sport.Data.EntityFilter filter = new Sport.Data.EntityFilter((int)Sport.Entities.Charge.Fields.Account, account.Id);
			Sport.UI.GeneralTableView chargesTableView =
				new Sport.UI.GeneralTableView("ChargeSelection", new Entities.ChargeView(),
				new int[] {
							  (int) Sport.Entities.Charge.Fields.Product,
							  (int) Sport.Entities.Charge.Fields.Amount,
							  (int) Sport.Entities.Charge.Fields.Price, 
							  (int) Sport.Entities.Charge.Fields.Status
						  },
				new int[] {
							  (int) Sport.Entities.Charge.Fields.Date
						  }, filter);

			chargesTableView.Title = "בחר חיובים";

			Sport.UI.EntitySelectionDialog chargesDialog = new Sport.UI.EntitySelectionDialog(chargesTableView, new System.Drawing.Size(500, 350));

			chargesDialog.Multiple = true;

			System.Collections.ArrayList al = new System.Collections.ArrayList();
			Sport.Entities.Charge[] charges = account.GetCharges();
			foreach (Sport.Entities.Charge charge in charges)
			{
				if (charge.Status == Sport.Types.ChargeStatusType.NotPaid)
				{
					al.Add(charge.Entity);
				}
			}

			chargesDialog.Entities = (Sport.Data.Entity[])al.ToArray(typeof(Sport.Data.Entity));

			if (chargesDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				Sport.Data.Entity[] entities = chargesDialog.Entities;

				if (entities.Length == 0)
					return null;

				charges = new Sport.Entities.Charge[entities.Length];

				for (int n = 0; n < entities.Length; n++)
				{
					charges[n] = new Sport.Entities.Charge(entities[n]);
				}

				return charges;
			}

			return null;
		}

		private void PrintChargeForm()
		{
			Sport.Entities.Account account = Account;
			if (account != null)
			{
				Sport.Entities.Charge[] charges = SelectCharges(account);
				if (charges == null)
					return;

				System.Drawing.Printing.PrinterSettings ps;
				Sport.UI.Dialogs.PrintSettingsForm settingsForm;
				if (!Sport.UI.Helpers.GetPrinterSettings(out ps, out settingsForm))
				{
					return;
				}

				if (settingsForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					Sport.Documents.Document document = CreateChargeForm(ps, charges);

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

		public override void OnContextMenu(Sport.UI.TableView2.SelectionType selectionType, Sport.UI.Controls.RightContextMenu menu)
		{
			base.OnContextMenu(selectionType, menu);

			if (selectionType == Sport.UI.TableView2.SelectionType.Single)
			{
				Sport.Entities.Charge charge = new Sport.Entities.Charge(Current);
				int schoolID = -1;
				if (charge.Account != null)
				{
					if (charge.Account.Entity.Fields[(int)Sport.Entities.Account.Fields.School] != null)
						schoolID = (int)charge.Account.Entity.Fields[(int)Sport.Entities.Account.Fields.School];
				}
				if (schoolID >= 0)
					menu.MenuItems.Add(new System.Windows.Forms.MenuItem("בית ספר", new System.EventHandler(ChargeSchoolClicked)));
				menu.MenuItems.Add(new System.Windows.Forms.MenuItem("קבלות", new System.EventHandler(ChargeReceiptClicked)));
			}
		}

		private void ChargeSchoolClicked(object sender, EventArgs e)
		{
			Sport.Data.Entity entity = Current;
			if (entity != null)
			{
				Sport.Entities.Charge charge = new Sport.Entities.Charge(entity);
				int school = (int)charge.Account.Entity.Fields[(int)Sport.Entities.Account.Fields.School];
				new OpenDialogCommand().Execute(typeof(SchoolDetailsView), "id=" + school);
			}
		}

		private void ChargeReceiptClicked(object sender, EventArgs e)
		{
			Sport.Data.Entity entity = Current;
			if (entity != null)
			{
				Sport.Entities.Charge charge = new Sport.Entities.Charge(entity);
				int account = charge.Account.Id;
				int region = charge.Region.Id;
				string state = Sport.Entities.Region.TypeName + "=" + region;
				state += "&" + Sport.Entities.Account.TypeName + "=" + account;
				ViewManager.OpenView(typeof(ReceiptsTableView), state);
			}
		}
	}
}
