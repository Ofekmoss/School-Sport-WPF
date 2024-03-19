using System;
using System.Linq;
using System.Collections.Generic;

namespace Sportsman.Forms
{
	public class ReceiptForm : System.Windows.Forms.Form,
		Sport.UI.Controls.IGridSource
	{
		public enum GridView
		{
			Payments = 0,
			Credits
		}

		public enum PaymentColumn
		{
			PaymentType = 0,
			Sum,
			BankName,
			BankBranch,
			BankAccount,
			Reference,
			PaymentDate,
			CreditCardType,
			CreditCardNumber,
			CreditCardExpire,
			CreditCardPayments
		}

		private Sport.UI.Controls.ThemeButton tbCancel;
		private System.Windows.Forms.Label labelRegion;
		private Sport.UI.Controls.NullComboBox cbRegion;
		private Sport.UI.EntitySelectionDialog accountDialog;
		private Sport.UI.Controls.ButtonBox bbAccount;
		private System.Windows.Forms.Label labelAccount;
		private System.Windows.Forms.Button btnCreateAccount;
		private System.Windows.Forms.Label labelTotal;
		private Sport.UI.Controls.GridControl gridControl;
		private System.Windows.Forms.TextBox tbRegion;
		private System.Windows.Forms.TextBox tbAccount;
		private System.Windows.Forms.Label labelRemarks;
		private System.Windows.Forms.TextBox tbRemarks;
		private System.Windows.Forms.Label labelDate;
		private System.Windows.Forms.DateTimePicker dtpDate;
		private Sport.UI.Controls.CaptionBar captions;
		private Sport.UI.Controls.ThemeButton tbOk;

		#region Initialize Component
		private void InitializeComponent()
		{
			this.tbCancel = new Sport.UI.Controls.ThemeButton();
			this.tbOk = new Sport.UI.Controls.ThemeButton();
			this.labelRegion = new System.Windows.Forms.Label();
			this.cbRegion = new Sport.UI.Controls.NullComboBox();
			this.bbAccount = new Sport.UI.Controls.ButtonBox();
			this.labelAccount = new System.Windows.Forms.Label();
			this.btnCreateAccount = new System.Windows.Forms.Button();
			this.gridControl = new Sport.UI.Controls.GridControl();
			this.captions = new Sport.UI.Controls.CaptionBar();
			this.labelTotal = new System.Windows.Forms.Label();
			this.tbRegion = new System.Windows.Forms.TextBox();
			this.tbAccount = new System.Windows.Forms.TextBox();
			this.labelRemarks = new System.Windows.Forms.Label();
			this.tbRemarks = new System.Windows.Forms.TextBox();
			this.labelDate = new System.Windows.Forms.Label();
			this.dtpDate = new System.Windows.Forms.DateTimePicker();
			this.SuspendLayout();
			// 
			// tbCancel
			// 
			this.tbCancel.Alignment = System.Drawing.StringAlignment.Center;
			this.tbCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.tbCancel.AutoSize = false;
			this.tbCancel.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbCancel.Hue = 300F;
			this.tbCancel.Image = null;
			this.tbCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.tbCancel.ImageList = null;
			this.tbCancel.ImageSize = new System.Drawing.Size(0, 0);
			this.tbCancel.Location = new System.Drawing.Point(72, 248);
			this.tbCancel.Name = "tbCancel";
			this.tbCancel.Saturation = 0.1F;
			this.tbCancel.Size = new System.Drawing.Size(56, 25);
			this.tbCancel.TabIndex = 11;
			this.tbCancel.Text = "ביטול";
			this.tbCancel.Transparent = System.Drawing.Color.Black;
			this.tbCancel.Click += new System.EventHandler(this.tbCancel_Click);
			// 
			// tbOk
			// 
			this.tbOk.Alignment = System.Drawing.StringAlignment.Center;
			this.tbOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.tbOk.AutoSize = false;
			this.tbOk.Enabled = true;
			this.tbOk.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbOk.Hue = 300F;
			this.tbOk.Image = null;
			this.tbOk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.tbOk.ImageList = null;
			this.tbOk.ImageSize = new System.Drawing.Size(0, 0);
			this.tbOk.Location = new System.Drawing.Point(8, 248);
			this.tbOk.Name = "tbOk";
			this.tbOk.Saturation = 0.1F;
			this.tbOk.Size = new System.Drawing.Size(56, 25);
			this.tbOk.TabIndex = 10;
			this.tbOk.Text = "אישור";
			this.tbOk.Transparent = System.Drawing.Color.Black;
			this.tbOk.Click += new System.EventHandler(this.tbOk_Click);
			// 
			// labelRegion
			// 
			this.labelRegion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelRegion.Location = new System.Drawing.Point(552, 13);
			this.labelRegion.Name = "labelRegion";
			this.labelRegion.Size = new System.Drawing.Size(32, 16);
			this.labelRegion.TabIndex = 12;
			this.labelRegion.Text = "מחוז:";
			// 
			// cbRegion
			// 
			this.cbRegion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cbRegion.Location = new System.Drawing.Point(384, 8);
			this.cbRegion.Name = "cbRegion";
			this.cbRegion.Size = new System.Drawing.Size(160, 22);
			this.cbRegion.Sorted = true;
			this.cbRegion.TabIndex = 13;
			this.cbRegion.SelectedIndexChanged += new System.EventHandler(this.cbRegion_SelectedIndexChanged);
			// 
			// bbAccount
			// 
			this.bbAccount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
				| System.Windows.Forms.AnchorStyles.Right)));
			this.bbAccount.Location = new System.Drawing.Point(8, 8);
			this.bbAccount.Name = "bbAccount";
			this.bbAccount.Size = new System.Drawing.Size(312, 22);
			this.bbAccount.TabIndex = 15;
			this.bbAccount.Value = null;
			this.bbAccount.ValueSelector = null;
			this.bbAccount.ValueChanged += new EventHandler(bbAcount_ValueChanged);
			// 
			// labelAccount
			// 
			this.labelAccount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelAccount.Location = new System.Drawing.Point(328, 13);
			this.labelAccount.Name = "labelAccount";
			this.labelAccount.Size = new System.Drawing.Size(40, 16);
			this.labelAccount.TabIndex = 14;
			this.labelAccount.Text = "חשבון:";
			// 
			// btnCreateAccount
			// 
			this.btnCreateAccount.Location = new System.Drawing.Point(8, 32);
			this.btnCreateAccount.Name = "btnCreateAccount";
			this.btnCreateAccount.Size = new System.Drawing.Size(56, 21);
			this.btnCreateAccount.TabIndex = 16;
			this.btnCreateAccount.Text = "חדש...";
			this.btnCreateAccount.Click += new System.EventHandler(this.btnCreateAccount_Click);
			// 
			// gridControl
			// 
			this.gridControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
				| System.Windows.Forms.AnchorStyles.Left)
				| System.Windows.Forms.AnchorStyles.Right)));
			this.gridControl.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.gridControl.Location = new System.Drawing.Point(8, 82);
			this.gridControl.Name = "gridControl";
			this.gridControl.Size = new System.Drawing.Size(576, 112);
			this.gridControl.TabIndex = 17;
			// 
			// captions
			// 
			this.captions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.captions.Appearance = Sport.UI.Controls.CaptionBarAppearance.Buttons;
			this.captions.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.captions.Location = new System.Drawing.Point(8, 62);
			this.captions.Name = "captions";
			this.captions.SelectedIndex = -1;
			this.captions.SelectedItem = null;
			this.captions.Size = new System.Drawing.Size(576, 20);
			this.captions.TabIndex = 18;
			// 
			// labelTotal
			// 
			this.labelTotal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.labelTotal.Location = new System.Drawing.Point(448, 200);
			this.labelTotal.Name = "labelTotal";
			this.labelTotal.Size = new System.Drawing.Size(136, 16);
			this.labelTotal.TabIndex = 19;
			this.labelTotal.Text = "סה\"כ שולם:";
			// 
			// tbRegion
			// 
			this.tbRegion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.tbRegion.Location = new System.Drawing.Point(384, 8);
			this.tbRegion.Name = "tbRegion";
			this.tbRegion.ReadOnly = true;
			this.tbRegion.Size = new System.Drawing.Size(160, 21);
			this.tbRegion.TabIndex = 20;
			this.tbRegion.Text = "";
			this.tbRegion.Visible = false;
			// 
			// tbAccount
			// 
			this.tbAccount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
				| System.Windows.Forms.AnchorStyles.Right)));
			this.tbAccount.Location = new System.Drawing.Point(8, 8);
			this.tbAccount.Name = "tbAccount";
			this.tbAccount.ReadOnly = true;
			this.tbAccount.Size = new System.Drawing.Size(312, 21);
			this.tbAccount.TabIndex = 21;
			this.tbAccount.Text = "";
			this.tbAccount.Visible = false;
			// 
			// labelRemarks
			// 
			this.labelRemarks.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelRemarks.Location = new System.Drawing.Point(384, 200);
			this.labelRemarks.Name = "labelRemarks";
			this.labelRemarks.Size = new System.Drawing.Size(48, 16);
			this.labelRemarks.TabIndex = 22;
			this.labelRemarks.Text = "הערות:";
			// 
			// tbRemarks
			// 
			this.tbRemarks.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
				| System.Windows.Forms.AnchorStyles.Right)));
			this.tbRemarks.Location = new System.Drawing.Point(8, 200);
			this.tbRemarks.Multiline = true;
			this.tbRemarks.Name = "tbRemarks";
			this.tbRemarks.Size = new System.Drawing.Size(376, 40);
			this.tbRemarks.TabIndex = 23;
			this.tbRemarks.Text = "";
			// 
			// labelDate
			// 
			this.labelDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelDate.Location = new System.Drawing.Point(536, 40);
			this.labelDate.Name = "labelDate";
			this.labelDate.Size = new System.Drawing.Size(48, 16);
			this.labelDate.TabIndex = 24;
			this.labelDate.Text = "תאריך:";
			// 
			// dtpDate
			// 
			this.dtpDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.dtpDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
			this.dtpDate.Location = new System.Drawing.Point(384, 35);
			this.dtpDate.Name = "dtpDate";
			this.dtpDate.Size = new System.Drawing.Size(136, 21);
			this.dtpDate.TabIndex = 25;
			// 
			// ReceiptForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.ClientSize = new System.Drawing.Size(592, 278);
			this.Controls.Add(this.dtpDate);
			this.Controls.Add(this.labelDate);
			this.Controls.Add(this.tbRemarks);
			this.Controls.Add(this.tbAccount);
			this.Controls.Add(this.tbRegion);
			this.Controls.Add(this.labelRemarks);
			this.Controls.Add(this.labelTotal);
			this.Controls.Add(this.captions);
			this.Controls.Add(this.gridControl);
			this.Controls.Add(this.btnCreateAccount);
			this.Controls.Add(this.bbAccount);
			this.Controls.Add(this.labelAccount);
			this.Controls.Add(this.cbRegion);
			this.Controls.Add(this.labelRegion);
			this.Controls.Add(this.tbCancel);
			this.Controls.Add(this.tbOk);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.Name = "ReceiptForm";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "קבלה";
			this.ResumeLayout(false);

		}
		#endregion

		private Sport.Entities.Receipt _receipt;
		public Sport.Entities.Receipt Receipt
		{
			get { return _receipt; }
		}

		private Sport.Entities.Region _region;
		public new Sport.Entities.Region Region
		{
			get { return _region; }
		}

		private Sport.Entities.Account _account;
		public Sport.Entities.Account Account
		{
			get { return _account; }
		}

		private struct PaymentRow
		{
			public Sport.Entities.Payment Payment;
			public Sport.Types.PaymentType PaymentType;
			public double Sum;
			public int Bank;
			public int BankBranch;
			public string BankAccount;
			public string Reference;
			public DateTime PaymentDate;
			public Sport.Types.CreditCardType CreditCardType;
			public int CreditCardNumber;
			public DateTime CreditCardExpire;
			public int CreditCardPayments;
		}

		private struct CreditRow
		{
			public Sport.Entities.Credit Credit;
			public Sport.Entities.Account Account;
			public double Sum;
		}

		private List<PaymentRow> paymentRows = new List<PaymentRow>();
		private List<CreditRow> creditRows = new List<CreditRow>();

		private int receiptAccountCredit;

		// payments to delete
		private List<Sport.Entities.Payment> deletePayments = new List<Sport.Entities.Payment>();
		private List<Sport.Entities.Credit> deleteCredits = new List<Sport.Entities.Credit>();

		private GridView gridView;

		private ReceiptForm()
		{
			InitializeComponent();

			captions.Items.Add("תשלומים");
			captions.Items.Add("זיכוי חשבונות");

			captions.SelectedIndex = 0;
			captions.SelectionChanged += new EventHandler(GridViewSelectionChanged);

			gridControl.Buttons.Add(Sport.Resources.Images.Plus, "הוסף");
			gridControl.Buttons.Add(Sport.Resources.Images.Minus, "מחק");
			gridControl.Buttons[0].Click += new EventHandler(GridAddClicked);
			gridControl.Buttons[1].Enabled = false;
			gridControl.Buttons[1].Click += new EventHandler(GridRemoveClicked);
			gridControl.Grid.SelectionChanged += new EventHandler(GridSelectionChanged);

			gridControl.Grid.Source = this;

			ResetGrid();
		}

		public ReceiptForm(Sport.Entities.Region region, Sport.Entities.Account account)
			: this()
		{
			cbRegion.Items.Clear();
			cbRegion.Items.AddRange(Sport.Entities.Region.Type.GetEntities(null));

			cbRegion.SelectedItem = region;

			Sport.Data.Entity entity = cbRegion.SelectedItem as Sport.Data.Entity;
			_region = entity == null ? null : new Sport.Entities.Region(entity);

			_account = account;

			SetAccounts();

			_receipt = null;

			btnCreateAccount.Enabled = _region != null;

			CreditRow creditRow = new CreditRow();
			creditRow.Credit = null;
			creditRow.Account = null;
			creditRow.Sum = 0;
			creditRows.Add(creditRow);
			receiptAccountCredit = creditRows.Count - 1;

			Text = "קבלה חדשה";
		}

		public ReceiptForm(Sport.Entities.Receipt receipt)
			: this()
		{
			_receipt = receipt;
			if (_receipt != null)
			{
				_account = _receipt.Account;
				_region = _receipt.Region;
				Text = "קבלה מספר " + _receipt.Number;
			}

			cbRegion.Visible = false;
			bbAccount.Visible = false;
			if (_region != null)
				tbRegion.Text = _region.Name;
			if (_account != null)
				tbAccount.Text = _account.Name;
			tbRegion.Visible = true;
			tbAccount.Visible = true;
			btnCreateAccount.Visible = false;

			if (_receipt != null)
			{
				Sport.Entities.Payment[] payments = _receipt.GetPayments();
				foreach (Sport.Entities.Payment payment in payments)
				{
					PaymentRow paymentRow = new PaymentRow();
					paymentRow.Payment = payment;
					paymentRow.PaymentType = payment.PaymentType;
					paymentRow.Sum = payment.Sum;
					paymentRow.Bank = payment.Bank;
					paymentRow.BankBranch = payment.BankBranch;
					paymentRow.BankAccount = payment.BankAccount;
					paymentRow.Reference = payment.Reference;
					paymentRow.PaymentDate = payment.Date;
					paymentRow.CreditCardType = payment.CreditCardType;
					paymentRow.CreditCardNumber = payment.CreditCardNumber;
					paymentRow.CreditCardExpire = payment.CreditCardExpire;
					paymentRow.CreditCardPayments = payment.CreditCardPayments;
					paymentRows.Add(paymentRow);
				}

				receiptAccountCredit = -1;
				Sport.Entities.Credit[] credits = _receipt.GetCredits();
				foreach (Sport.Entities.Credit credit in credits)
				{
					CreditRow creditRow = new CreditRow();
					creditRow.Credit = credit;
					creditRow.Sum = credit.Sum;
					if (credit.Account.Equals(_account))
					{
						creditRow.Account = null;
						creditRows.Add(creditRow);
						receiptAccountCredit = creditRows.Count - 1;
					}
					else
					{
						creditRow.Account = credit.Account;
						creditRows.Add(creditRow);
					}
				}
				
				if (receiptAccountCredit == -1)
				{
					CreditRow creditRow = new CreditRow();
					creditRow.Credit = null;
					creditRow.Account = null;
					creditRow.Sum = 0;
					creditRows.Add(creditRow);
					receiptAccountCredit = creditRows.Count - 1;
				}

				tbRemarks.Text = _receipt.Remarks;
				if (_receipt.Date.Year > 1900)
					dtpDate.Value = _receipt.Date;
			}

			SetPaymentColumns();

			ResetTotal();
		}

		private void ResetGrid()
		{
			gridControl.Grid.Columns.Clear();

			gridView = (GridView)captions.SelectedIndex;
			switch (gridView)
			{
				case (GridView.Payments):
					gridControl.Grid.Columns.Add((int)PaymentColumn.PaymentType, "אמצעי תשלום", 100);
					gridControl.Grid.Columns.Add((int)PaymentColumn.Sum, "סכום", 50);
					gridControl.Grid.Columns.Add((int)PaymentColumn.BankName, "בנק", 160);
					gridControl.Grid.Columns.Add((int)PaymentColumn.BankBranch, "סניף", 50);
					gridControl.Grid.Columns.Add((int)PaymentColumn.BankAccount, "חשבון", 70);
					gridControl.Grid.Columns.Add((int)PaymentColumn.Reference, "אסמכתא", 75);
					gridControl.Grid.Columns.Add((int)PaymentColumn.PaymentDate, "תאריך פרעון", 85);

					//credit card..
					gridControl.Grid.Columns.Add((int)PaymentColumn.CreditCardType, "סוג כרטיס", 160);
					gridControl.Grid.Columns.Add((int)PaymentColumn.CreditCardNumber, "מס' כרטיס", 80);
					gridControl.Grid.Columns.Add((int)PaymentColumn.CreditCardExpire, "תוקף כרטיס", 85);
					gridControl.Grid.Columns.Add((int)PaymentColumn.CreditCardPayments, "מס' תשלומים", 80);
					break;
				case (GridView.Credits):
					gridControl.Grid.Columns.Add(0, "חשבון", 150);
					gridControl.Grid.Columns.Add(1, "סכום", 70);
					break;
			}

			gridControl.Grid.RefreshSource();
		}

		private void SetAccounts()
		{
			Views.AccountsTableView accountTableView = new Views.AccountsTableView();
			accountTableView.State[Sport.UI.View.SelectionDialog] = "1";
			accountDialog = new Sport.UI.EntitySelectionDialog(accountTableView);
			bbAccount.ValueSelector = new Sport.UI.Controls.ButtonBox.SelectValue(
				accountDialog.ValueSelector);
			accountDialog.View.State[Sport.Entities.Region.TypeName] = (_region == null) ? null : _region.Id.ToString();
			bbAccount.Value = (_account == null) ? null : _account.Entity;
			_account = (bbAccount.Value == null) ? null : new Sport.Entities.Account(bbAccount.Value as Sport.Data.Entity);
		}

		private void tbOk_Click(object sender, System.EventArgs e)
		{
			if (_account == null)
			{
				Sport.UI.MessageBox.Error("לא נבחר חשבון", "קבלה חדשה");
				return;
			}

			//don't allow receipt with total sum of 0:
			if (creditRows.Sum(r => r.Sum) <= 0)
			{
				Sport.UI.MessageBox.Error("לא ניתן לשמור קבלה עם סכום כולל של אפס", "עריכת קבלה");
				return;
			}

			if (_receipt == null)
			{
				//maybe total is not correct?

				for (int n = 0; n < creditRows.Count; n++)
				{
					CreditRow creditRow = creditRows[n];
					Sport.Entities.Account curAccount = null;
					if (n == receiptAccountCredit)
						curAccount = _account;
					else
						curAccount = creditRow.Account;
					if (curAccount != null)
					{
						Sport.Entities.Charge[] charges = curAccount.GetCharges();
						if (charges != null && charges.Length > 0)
						{
							int debt = 0;
							foreach (Sport.Entities.Charge charge in charges)
							{
								if (charge.Status != Sport.Types.ChargeStatusType.Paid)
									debt += (int)charge.Price;
							}

							if ((int)creditRow.Sum != debt)
							{
								string strMessage = "הסכום שהוקלד עבור החשבון '" + curAccount.Name + "' שונה מסכום החיובים עבור חשבון זה." + "\n" +
									"סכום החיובים שלא שולמו הינו " + debt + " ש\"ח בעוד שסך כל אשר שולם הוא " +
									((int)creditRow.Sum) + " ש\"ח בלבד." + "\n" + "האם להמשיך בכל זאת?";
								if (!Sport.UI.MessageBox.Ask(strMessage, "קבלות", true))
									return;
							}
						}
					}
				}

				_receipt = new Sport.Entities.Receipt(Sport.Entities.Receipt.Type.New());
				_receipt.Region = _region;
				_receipt.Account = _account;
			}
			else
			{
				_receipt.Edit();
			}

			_receipt.Entity.Fields[(int)Sport.Entities.Receipt.Fields.Season] = Sport.Core.Session.Season;

			_receipt.Sum = total;
			_receipt.Date = dtpDate.Value;
			_receipt.Remarks = tbRemarks.Text;

			Sport.Data.EntityResult result = _receipt.Save();
			if (!result.Succeeded)
			{
				Sport.UI.MessageBox.Show(result.GetMessage(), System.Windows.Forms.MessageBoxIcon.Stop);
				return;
			}

			// Storing receipt payments
			Sport.Entities.Payment payment;
			foreach (PaymentRow paymentRow in paymentRows)
			{
				if (paymentRow.Payment == null)
				{
					payment = new Sport.Entities.Payment(Sport.Entities.Payment.Type.New());
					payment.Receipt = _receipt;
				}
				else
				{
					payment = paymentRow.Payment;
					payment.Edit();
				}

				payment.PaymentType = paymentRow.PaymentType;
				payment.Sum = paymentRow.Sum;
				payment.Bank = paymentRow.Bank;
				payment.BankBranch = paymentRow.BankBranch;
				payment.BankAccount = paymentRow.BankAccount;
				payment.Reference = paymentRow.Reference;
				payment.Date = paymentRow.PaymentDate;
				payment.CreditCardType = paymentRow.CreditCardType;
				payment.CreditCardNumber = paymentRow.CreditCardNumber;
				payment.CreditCardExpire = paymentRow.CreditCardExpire;
				payment.CreditCardPayments = paymentRow.CreditCardPayments;
				result = payment.Save();

				if (!result.Succeeded)
				{
					Sport.UI.MessageBox.Show("כשלון בשמירת תשלום '" + payment.Name + "'\n" +
						result.GetMessage(), System.Windows.Forms.MessageBoxIcon.Stop);
				}
			}

			for (int n = 0; n < deletePayments.Count; n++)
			{
				payment = deletePayments[n];
				if (!payment.Entity.Delete())
					Sport.UI.MessageBox.Show("כשלון במחיקת תשלום '" + payment.Name + "'", System.Windows.Forms.MessageBoxIcon.Stop);
			}

			// Storing receipt credits
			Sport.Entities.Credit credit;
			System.Collections.ArrayList arrAccounts = new System.Collections.ArrayList();
			for (int n = 0; n < creditRows.Count; n++)
			{
				CreditRow creditRow = creditRows[n];
				Sport.Entities.Account curAccount = null;
				if (creditRow.Credit == null)
				{
					credit = new Sport.Entities.Credit(Sport.Entities.Credit.Type.New());
					credit.Region = _receipt.Region;
					credit.Receipt = _receipt;
					if (n == receiptAccountCredit)
						curAccount = _receipt.Account;
					else
						curAccount = creditRow.Account;
					credit.Account = curAccount;
				}
				else
				{
					credit = creditRow.Credit;
					curAccount = credit.Account;
					/*
					Sport.Entities.Account account = credit.Account;
					if (account != null)
					{
						Sport.Data.Entity[] arrExistsEntities = Sport.Entities.AccountEntry.Type.GetEntities(
							new Sport.Data.EntityFilter((int) Sport.Entities.AccountEntry.Fields.Additional, -1*credit.Id));
						if (arrExistsEntities == null || arrExistsEntities.Length == 0)
						{
							Sport.Data.EntityEdit accountEntry = Sport.Entities.AccountEntry.Type.New();
							accountEntry.Fields[(int) Sport.Entities.AccountEntry.Fields.Account] = account.Id;
							accountEntry.Fields[(int) Sport.Entities.AccountEntry.Fields.Additional] = -1*credit.Id;
							accountEntry.Fields[(int) Sport.Entities.AccountEntry.Fields.Description] = "זיכוי חשבון " + account.Name;
							accountEntry.Fields[(int) Sport.Entities.AccountEntry.Fields.EntryDate] = DateTime.Now;
							accountEntry.Fields[(int) Sport.Entities.AccountEntry.Fields.EntryType] = (int) Sport.Types.AccountEntryType.Credit;
							accountEntry.Fields[(int) Sport.Entities.AccountEntry.Fields.LastModified] = DateTime.Now;
							accountEntry.Fields[(int) Sport.Entities.AccountEntry.Fields.Region] = account.Region.Id;
							accountEntry.Fields[(int) Sport.Entities.AccountEntry.Fields.Sum] = credit.Sum;
							accountEntry.Save();
						}
					}
					*/
					credit.Edit();
				}
				if ((curAccount != null) && (arrAccounts.IndexOf(curAccount) < 0))
					arrAccounts.Add(curAccount);

				credit.Sum = creditRow.Sum;
				result = credit.Save();

				if (!result.Succeeded)
				{
					Sport.UI.MessageBox.Show("כשלון בשמירת זיכוי '" + credit.Name + "'\n" +
						result.GetMessage(), System.Windows.Forms.MessageBoxIcon.Stop);
				}
			}

			for (int n = 0; n < deleteCredits.Count; n++)
			{
				credit = deleteCredits[n];
				if (!credit.Entity.Delete())
					Sport.UI.MessageBox.Show("כשלון במחיקת זיכוי '" + credit.Name + "'", System.Windows.Forms.MessageBoxIcon.Stop);
			}

			string strAccountsID = "";
			foreach (Sport.Entities.Account account in arrAccounts)
			{
				Sport.Data.EntityFilter filter = new Sport.Data.EntityFilter(
					(int)Sport.Entities.Charge.Fields.Account, account.Id);
				filter.Add(new Sport.Data.EntityFilterField(
					(int)Sport.Entities.Charge.Fields.Status, (int)Sport.Types.ChargeStatusType.NotPaid));
				Sport.Data.Entity[] curAccountCharges = Sport.Entities.Charge.Type.GetEntities(filter);
				if (curAccountCharges.Length > 0)
					strAccountsID += account.Id + ",";
			}
			if (strAccountsID.Length > 0)
			{
				strAccountsID = strAccountsID.Substring(0, strAccountsID.Length - 1);
				Sport.UI.EntitySelectionDialog dialog = new Sport.UI.EntitySelectionDialog(
					new Views.ChargesTableView(), "אישור", "ביטול", true, new System.Drawing.Size(750, 350));
				dialog.View.State[Sport.UI.View.SelectionDialog] = "1";
				dialog.View.State[Sport.Entities.Region.TypeName] = (arrAccounts[0] as Sport.Entities.Account).Region.Id.ToString();
				dialog.View.State["accounts"] = strAccountsID;
				if (dialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
				{
					Sport.Data.Entity[] arrChargeEntities = dialog.Entities;
					for (int i = 0; i < arrChargeEntities.Length; i++)
					{
						Sport.Entities.Charge charge = new Sport.Entities.Charge(arrChargeEntities[i]);
						charge.Edit();
						charge.Status = Sport.Types.ChargeStatusType.Paid;
						charge.Save();
					}
				}
			}

			DialogResult = System.Windows.Forms.DialogResult.OK;
		}

		private void tbCancel_Click(object sender, System.EventArgs e)
		{
			DialogResult = System.Windows.Forms.DialogResult.Cancel;
		}

		private void cbRegion_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			Sport.Data.Entity entity = cbRegion.SelectedItem as Sport.Data.Entity;
			_region = entity == null ? null : new Sport.Entities.Region(entity);

			btnCreateAccount.Enabled = _region != null;

			SetAccounts();

			gridControl.Grid.Refresh();


			//tbOk.Enabled = _account != null;
		}

		private void bbAcount_ValueChanged(object sender, System.EventArgs e)
		{
			_account = (bbAccount.Value == null) ? null : (new Sport.Entities.Account(bbAccount.Value as Sport.Data.Entity));

			gridControl.Grid.Refresh();

			//tbOk.Enabled = _account != null;
		}

		private void btnCreateAccount_Click(object sender, System.EventArgs e)
		{
			if (_region != null)
			{
				CreateAccountForm caf = new CreateAccountForm(_region);

				if (caf.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					_account = caf.Account;
					SetAccounts();
				}
			}
		}

		private Sport.Entities.Account SelectAccount()
		{
			Views.AccountsTableView accountTableView = new Views.AccountsTableView();
			accountTableView.State[Sport.Entities.Region.TypeName] = _region == null ? null : _region.Id.ToString();

			Sport.UI.EntitySelectionDialog accountSelectionDialog = new Sport.UI.EntitySelectionDialog(accountTableView);

			if (accountSelectionDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				Sport.Data.Entity entity = accountSelectionDialog.Entity;
				if (entity != null)
				{
					SetAccounts(); // Maybe a new account was added
					return new Sport.Entities.Account(entity);
				}
			}

			return null;
		}

		private void GridAddClicked(object sender, EventArgs e)
		{
			if (gridView == GridView.Payments)
			{
				PaymentRow paymentRow = new PaymentRow();
				paymentRow.Payment = null;
				paymentRow.PaymentType = Sport.Types.PaymentType.Cash;
				paymentRow.Sum = 0;
				paymentRow.Bank = 0;
				paymentRow.BankBranch = 0;
				paymentRow.BankAccount = null;
				paymentRow.Reference = null;
				paymentRow.PaymentDate = DateTime.Now;
				paymentRow.CreditCardType = Sport.Types.CreditCardType.Visa;
				paymentRow.CreditCardNumber = 0;
				paymentRow.CreditCardExpire = DateTime.Now.AddMonths(1);
				paymentRow.CreditCardPayments = 1;
				paymentRows.Add(paymentRow);
			}
			else if (gridView == GridView.Credits)
			{
				Sport.Entities.Account account = SelectAccount();
				if (account != null)
				{
					CreditRow creditRow = new CreditRow();
					int index = -1;
					if (account.Equals(_account))
					{
						index = receiptAccountCredit;
					}
					else
					{
						for (int n = 0; n < creditRows.Count && index == -1; n++)
						{
							creditRow = creditRows[n];
							if (account.Equals(creditRow.Account))
								index = n;
						}
					}

					if (index == -1)
					{
						creditRow.Credit = null;
						creditRow.Account = account;
						creditRow.Sum = 0;
						creditRows.Add(creditRow);
						index = creditRows.Count - 1;
					}

					gridControl.Grid.SelectRow(index);
				}
			}

			gridControl.Grid.RefreshSource();
		}

		private void GridRemoveClicked(object sender, EventArgs e)
		{
			int[] rows = gridControl.Grid.Selection.Rows;

			if (gridView == GridView.Payments)
			{
				for (int n = rows.Length - 1; n >= 0; n--)
				{
					int rowIndex = rows[n];
					if (rowIndex >= 0 && rowIndex < paymentRows.Count)
					{
						Sport.Entities.Payment payment = paymentRows[rowIndex].Payment;
						if (payment != null)
							deletePayments.Add(payment);
						paymentRows.RemoveAt(rowIndex);
					}
				}

				ResetTotal();
			}
			else if (gridView == GridView.Credits)
			{
				for (int n = rows.Length - 1; n >= 0; n--)
				{
					if (n != receiptAccountCredit)
					{
						Sport.Entities.Credit credit = creditRows[rows[n]].Credit;
						if (credit != null)
							deleteCredits.Add(credit);
						creditRows.RemoveAt(rows[n]);
					}
				}
			}

			gridControl.Grid.RefreshSource();
		}

		private void GridSelectionChanged(object sender, EventArgs e)
		{
			int[] rows = gridControl.Grid.Selection.Rows;
			gridControl.Buttons[1].Enabled = rows.Length > 0;
		}

		#region IGridSource Members

		public void SetGrid(Sport.UI.Controls.Grid grid)
		{
		}

		public void Sort(int group, int[] columns)
		{
		}

		public Sport.UI.Controls.Style GetStyle(int row, int field, Sport.UI.Controls.GridDrawState state)
		{
			return null;
		}

		public string GetTip(int row)
		{
			return null;
		}

		public int GetGroup(int row)
		{
			return 0;
		}

		private Sport.UI.Controls.TextControl gridNumberEdit = null;

		private Sport.UI.Controls.NullComboBox ncbPaymentType = null;
		private Sport.UI.Controls.NullComboBox ncbBankType = null;
		private Sport.UI.Controls.NullComboBox ncbCreditCardType = null;

		private System.Windows.Forms.DateTimePicker dtpPaymentDate = null;
		private System.Windows.Forms.DateTimePicker dtpCreditCardExpire = null;

		private bool settingGridEdit = false;

		public System.Windows.Forms.Control Edit(int row, int field)
		{
			settingGridEdit = true;

			if (gridNumberEdit == null)
			{
				gridNumberEdit = new Sport.UI.Controls.TextControl();

				gridNumberEdit.TextChanged += new EventHandler(GridNumberEditTextChanged);
				gridNumberEdit.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
				gridNumberEdit.ShowSpin = true;
			}

			if (gridView == GridView.Payments)
			{
				PaymentRow paymentRow = paymentRows[row];
				PaymentColumn column = (PaymentColumn)field;
				switch (column)
				{
					case PaymentColumn.PaymentType:
						if (ncbPaymentType == null)
						{
							ncbPaymentType = new Sport.UI.Controls.NullComboBox();
							ncbPaymentType.Items.AddRange(Sport.Types.PaymentTypeLookup.types);
							ncbPaymentType.SelectedIndexChanged += new EventHandler(PaymentTypeChanged);
						}

						ncbPaymentType.SelectedItem = paymentTypeLookup[(int)paymentRow.PaymentType];

						settingGridEdit = false;
						return ncbPaymentType;
					case PaymentColumn.PaymentDate:
						if (dtpPaymentDate == null)
						{
							dtpPaymentDate = new System.Windows.Forms.DateTimePicker();
							dtpPaymentDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
							dtpPaymentDate.ValueChanged += new EventHandler(PaymentDateChanged);
						}

						dtpPaymentDate.Value = paymentRow.PaymentDate;

						settingGridEdit = false;
						return dtpPaymentDate;
					case PaymentColumn.Sum:
						gridNumberEdit.Controller = new Sport.UI.Controls.NumberController(0, 99999999, 7, 2);
						gridNumberEdit.Value = paymentRow.Sum;
						break;
					case PaymentColumn.BankName:
						if (paymentRow.PaymentType == Sport.Types.PaymentType.Cash)
							return null;

						if (ncbBankType == null)
						{
							ncbBankType = new Sport.UI.Controls.NullComboBox();
							ncbBankType.Items.Add(Sport.UI.Controls.NullComboBox.Null);
							ncbBankType.Items.AddRange(Sport.Core.Data.bankItems);
							ncbBankType.SelectedIndexChanged += new EventHandler(BankTypeChanged);
						}

						int bankCode = paymentRow.Bank;
						Sport.Core.Data.BankItem selItem = null;
						if (bankCode > 0)
						{
							for (int i = 0; i < Sport.Core.Data.bankItems.Length; i++)
							{
								if (Sport.Core.Data.bankItems[i].BankCode == bankCode)
								{
									selItem = Sport.Core.Data.bankItems[i];
									break;
								}
							}
						}
						ncbBankType.SelectedItem = selItem;

						settingGridEdit = false;
						return ncbBankType;
					case PaymentColumn.BankBranch:
						if (paymentRow.PaymentType == Sport.Types.PaymentType.Cash)
							return null;

						gridNumberEdit.Controller = new Sport.UI.Controls.NumberController(0, 99999, 5, 0);
						gridNumberEdit.Value = (double)paymentRow.BankBranch;
						break;
					case PaymentColumn.BankAccount:
						if (paymentRow.PaymentType == Sport.Types.PaymentType.Cash)
							return null;

						gridNumberEdit.Controller = new Sport.UI.Controls.RegularExpressionController(".{0,15}");
						gridNumberEdit.Value = paymentRow.BankAccount;
						break;
					case PaymentColumn.Reference:
						gridNumberEdit.Controller = new Sport.UI.Controls.RegularExpressionController(".{0,15}");
						gridNumberEdit.Value = paymentRow.Reference;
						break;
					case PaymentColumn.CreditCardType:
						if (ncbCreditCardType == null)
						{
							ncbCreditCardType = new Sport.UI.Controls.NullComboBox();
							ncbCreditCardType.Items.AddRange(Sport.Types.CreditCardTypeLookup.types);
							ncbCreditCardType.SelectedIndexChanged += new EventHandler(CreditCardTypeChanged);
						}

						ncbCreditCardType.SelectedItem = creditCardTypeLookup[(int)paymentRow.CreditCardType];

						settingGridEdit = false;
						return ncbCreditCardType;
					case PaymentColumn.CreditCardNumber:
						gridNumberEdit.Controller = new Sport.UI.Controls.NumberController(0, 9999, 5, 0);
						gridNumberEdit.Value = (double)paymentRow.CreditCardNumber;
						break;
					case PaymentColumn.CreditCardExpire:
						if (dtpCreditCardExpire == null)
						{
							dtpCreditCardExpire = new System.Windows.Forms.DateTimePicker();
							dtpCreditCardExpire.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
							dtpCreditCardExpire.CustomFormat = "MM/yyyy"; //"MMM-yyyy"; //"dd/MM/yyyy";
							dtpCreditCardExpire.ValueChanged += new EventHandler(CreditCardExpireChanged);
							dtpCreditCardExpire.KeyDown += CreditCardExpireKeyDown;
							dtpCreditCardExpire.KeyPress += CreditCardExpireKeyPress;
						}

						dtpCreditCardExpire.Value = (paymentRow.CreditCardExpire.Year > 1900) ? paymentRow.CreditCardExpire : DateTime.Now.AddMonths(1);

						settingGridEdit = false;
						return dtpCreditCardExpire;
					case PaymentColumn.CreditCardPayments:
						gridNumberEdit.Controller = new Sport.UI.Controls.NumberController(1, 999, 5, 0);
						gridNumberEdit.Value = (double)paymentRow.CreditCardPayments;
						break;
				}

				settingGridEdit = false;
				return gridNumberEdit;
			}
			else if (gridView == GridView.Credits)
			{
				if (field == 1 && row != receiptAccountCredit) // Sum
				{
					double sum = creditRows[row].Sum;
					double maxCredit = creditRows[receiptAccountCredit].Sum + sum;
					gridNumberEdit.Controller = new Sport.UI.Controls.NumberController(0, maxCredit, 7, 2);
					gridNumberEdit.Value = sum;
					settingGridEdit = false;
					return gridNumberEdit;
				}
			}

			return null;
		}

		private bool riskOfDateTimePickerCrash()
		{
			DateTime now = DateTime.Now;
			var risk = now.Day >= 29 && now.Day <= 31;
			return risk;
		}

		private void CreditCardExpireKeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			if (riskOfDateTimePickerCrash() == true)
			{
				e.Handled = true;
			}
		}

		private void CreditCardExpireKeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (riskOfDateTimePickerCrash() == true)
			{
				e.Handled = true;
			}
		}

		public void EditEnded(System.Windows.Forms.Control control)
		{
		}

		public int GetRowCount()
		{
			switch (gridView)
			{
				case (GridView.Payments):
					return paymentRows.Count;
				case (GridView.Credits):
					return creditRows.Count;
			}

			return 0;
		}

		public int[] GetSort(int group)
		{
			return null;
		}

		private Sport.Types.PaymentTypeLookup paymentTypeLookup = new Sport.Types.PaymentTypeLookup();
		private Sport.Types.CreditCardTypeLookup creditCardTypeLookup = new Sport.Types.CreditCardTypeLookup();

		public string GetText(int row, int field)
		{
			if (gridView == GridView.Payments)
			{
				PaymentRow paymentRow = paymentRows[row];
				PaymentColumn column = (PaymentColumn)field;
				switch (column)
				{
					case PaymentColumn.PaymentType:
						return paymentTypeLookup.Lookup((int)paymentRow.PaymentType);
					case PaymentColumn.Sum:
						return paymentRow.Sum.ToString();
					case PaymentColumn.BankName:
						string bankName = null;
						int bankCode = paymentRow.Bank;
						if (bankCode > 0)
						{
							bankName = bankCode.ToString();
							for (int i = 0; i < Sport.Core.Data.bankItems.Length; i++)
							{
								Sport.Core.Data.BankItem item = Sport.Core.Data.bankItems[i];
								if (item.BankCode == bankCode)
								{
									bankName = item.BankName;
									break;
								}
							}
						}
						return bankName;
					case PaymentColumn.BankBranch:
						return paymentRow.BankBranch == 0 ? null : paymentRow.BankBranch.ToString();
					case PaymentColumn.BankAccount:
						return paymentRow.BankAccount;
					case PaymentColumn.Reference:
						return paymentRow.Reference;
					case PaymentColumn.PaymentDate:
						return Sport.Common.Tools.IsMinDate(paymentRow.PaymentDate) ? null : paymentRow.PaymentDate.ToString("dd/MM/yyyy");
					case PaymentColumn.CreditCardType:
						return creditCardTypeLookup.Lookup((int)paymentRow.CreditCardType);
					case PaymentColumn.CreditCardNumber:
						return paymentRow.CreditCardNumber.ToString().PadLeft(4, '0');
					case PaymentColumn.CreditCardExpire:
						return Sport.Common.Tools.IsMinDate(paymentRow.CreditCardExpire) ? null : paymentRow.CreditCardExpire.ToString("MM/yyyy");
					case PaymentColumn.CreditCardPayments:
						return paymentRow.CreditCardPayments.ToString();
				}
			}
			else if (gridView == GridView.Credits)
			{
				CreditRow creditRow = creditRows[row];

				switch (field)
				{
					case (0): // Account
						return row == receiptAccountCredit ? (_account == null ? null : _account.Name) : creditRow.Account.Name;
					case (1): // Sum
						return creditRow.Sum.ToString();
				}
			}

			return null;
		}

		public int GetFieldCount(int row)
		{
			switch (gridView)
			{
				case (GridView.Payments):
					return (int)PaymentColumn.CreditCardPayments + 1;
				case (GridView.Credits):
					return 2;
			}

			return 0;
		}

		#endregion

		private void GridNumberEditTextChanged(object sender, EventArgs e)
		{
			if (!settingGridEdit)
			{
				if (gridView == GridView.Payments)
				{
					PaymentRow paymentRow = paymentRows[gridControl.Grid.EditCell.Y];
					PaymentColumn column = (PaymentColumn)gridControl.Grid.EditCell.X;
					object value = gridNumberEdit.Value;
					if (value != null)
					{
						switch (column)
						{
							case PaymentColumn.Sum:
								paymentRow.Sum = value == null ? 0 : (double)value;
								break;
							//case (2): // Bank
							//	paymentRow.Bank = gridNumberEdit.Value == null ? 0 : (int) (double) gridNumberEdit.Value;
							//	break;
							case PaymentColumn.BankBranch:
								paymentRow.BankBranch = (value == null) ? 0 : (int)(double)value;
								break;
							case PaymentColumn.BankAccount:
								paymentRow.BankAccount = (string)value;
								break;
							case PaymentColumn.Reference:
								paymentRow.Reference = (string)value;
								break;
							case PaymentColumn.CreditCardNumber:
								paymentRow.CreditCardNumber = (int)(double)value;
								break;
							case PaymentColumn.CreditCardPayments:
								paymentRow.CreditCardPayments = (int)(double)value;
								break;
						}
						paymentRows[gridControl.Grid.EditCell.Y] = paymentRow;
					}
				}
				else if (gridView == GridView.Credits)
				{
					if (gridControl.Grid.EditCell.X == 1) // Sum
					{
						CreditRow creditRow = creditRows[gridControl.Grid.EditCell.Y];
						creditRow.Sum = gridNumberEdit.Value == null ? 0 : (double)gridNumberEdit.Value;
						creditRows[gridControl.Grid.EditCell.Y] = creditRow;
					}
				}

				ResetTotal();
			}
		}

		private void PaymentTypeChanged(object sender, EventArgs e)
		{
			if (!settingGridEdit)
			{
				Sport.Data.LookupItem li = ncbPaymentType.SelectedItem as Sport.Data.LookupItem;

				if (li != null)
				{
					PaymentRow paymentRow = paymentRows[gridControl.Grid.EditCell.Y];
					paymentRow.PaymentType = (Sport.Types.PaymentType)li.Id;
					paymentRows[gridControl.Grid.EditCell.Y] = paymentRow;
					SetPaymentColumns();
				}
			}
		}

		private void PaymentDateChanged(object sender, EventArgs e)
		{
			if (!settingGridEdit)
			{
				PaymentRow paymentRow = paymentRows[gridControl.Grid.EditCell.Y];
				paymentRow.PaymentDate = dtpPaymentDate.Value;
				paymentRows[gridControl.Grid.EditCell.Y] = paymentRow;
			}
		}

		private void CreditCardExpireChanged(object sender, EventArgs e)
		{
			if (!settingGridEdit)
			{
				PaymentRow paymentRow = paymentRows[gridControl.Grid.EditCell.Y];
				paymentRow.CreditCardExpire = dtpCreditCardExpire.Value;
				paymentRows[gridControl.Grid.EditCell.Y] = paymentRow;
			}
		}

		private void CreditCardTypeChanged(object sender, EventArgs e)
		{
			if (!settingGridEdit)
			{
				PaymentRow paymentRow = paymentRows[gridControl.Grid.EditCell.Y];
				paymentRow.CreditCardType = (Sport.Types.CreditCardType)(ncbCreditCardType.SelectedItem as Sport.Data.LookupItem).Id;
				paymentRows[gridControl.Grid.EditCell.Y] = paymentRow;
			}
		}
		

		private void BankTypeChanged(object sender, EventArgs e)
		{
			if (!settingGridEdit)
			{
				int bankCode = 0;
				if (ncbBankType.SelectedItem != null)
				{
					bankCode = (ncbBankType.SelectedItem as Sport.Core.Data.BankItem).BankCode;
				}
				PaymentRow paymentRow = paymentRows[gridControl.Grid.EditCell.Y];
				paymentRow.Bank = bankCode;
				paymentRows[gridControl.Grid.EditCell.Y] = paymentRow;
			}
		}

		private double total = 0;

		private void ResetTotal()
		{
			total = 0;
			foreach (PaymentRow paymentRow in paymentRows)
				total += paymentRow.Sum;

			labelTotal.Text = "סה\"כ שולם: " + total.ToString();

			double credit = total;
			CreditRow creditRow;
			for (int n = 0; n < creditRows.Count; n++)
			{
				if (n != receiptAccountCredit)
				{
					creditRow = creditRows[n];
					if (creditRow.Sum > credit)
					{
						creditRow.Sum = credit;
						credit = 0;
						creditRows[n] = creditRow;
					}
					else
					{
						credit -= creditRow.Sum;
					}
				}
			}

			creditRow = creditRows[receiptAccountCredit];
			creditRow.Sum = credit;
			creditRows[receiptAccountCredit] = creditRow;
			gridControl.Grid.Refresh();
		}

		private void GridViewSelectionChanged(object sender, EventArgs e)
		{
			ResetGrid();
		}

		private bool? _lastCreditCardMode = null;
		private void SetPaymentColumns()
		{
			if (gridView == GridView.Payments)
			{
				int creditCardPaymentCount = 0;
				foreach (PaymentRow paymentRow in paymentRows)
				{
					if (paymentRow.PaymentType == Sport.Types.PaymentType.CreditCard)
						creditCardPaymentCount++;
				}
				bool curCreditCardMode = (creditCardPaymentCount > 0 && creditCardPaymentCount == paymentRows.Count);
				if (_lastCreditCardMode == null || curCreditCardMode != _lastCreditCardMode.Value)
				{
					gridControl.Grid.Columns[(int)PaymentColumn.CreditCardType].Visible = curCreditCardMode;
					gridControl.Grid.Columns[(int)PaymentColumn.CreditCardNumber].Visible = curCreditCardMode;
					gridControl.Grid.Columns[(int)PaymentColumn.CreditCardExpire].Visible = curCreditCardMode;
					gridControl.Grid.Columns[(int)PaymentColumn.CreditCardPayments].Visible = curCreditCardMode;
					gridControl.Grid.Columns[(int)PaymentColumn.BankName].Visible = !curCreditCardMode;
					gridControl.Grid.Columns[(int)PaymentColumn.BankBranch].Visible = !curCreditCardMode;
					gridControl.Grid.Columns[(int)PaymentColumn.BankAccount].Visible = !curCreditCardMode;
					gridControl.Grid.Columns[(int)PaymentColumn.Reference].Visible = !curCreditCardMode;
					gridControl.Grid.Columns[(int)PaymentColumn.PaymentDate].Visible = !curCreditCardMode;
					gridControl.Refresh();
					_lastCreditCardMode = curCreditCardMode;
				}
			}
		}
	}
}
