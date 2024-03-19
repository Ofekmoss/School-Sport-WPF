using System;
using System.Drawing;
using Sport.Documents;
using Sport.Core;

namespace Sportsman.Documents
{
	public class PaymentDocuments : BaseDocumentBuilder
	{
		public PaymentDocuments(System.Drawing.Printing.PrinterSettings settings)
			: base(settings)
		{
		}

		public void CreateChargesSection(DocumentBuilder documentBuilder, Sport.Entities.Charge[] charges)
		{
			if (charges == null || charges.Length == 0)
				return;
			Sport.Entities.Account account = charges[0].Account;
			Sport.Entities.School school = account.School;

			Section section = CreateSection(documentBuilder);

			string address;
			if (school != null)
				address = "לכבוד:\nביה\"ס " + school.Name + "\n" + school.Address;
			else
				address = "לכבוד:\n" + account.AccountName + "\n" + account.Address;
			TextItem ti = new TextItem(
				new System.Drawing.Rectangle(
				documentBuilder.DefaultMargins.Left,
				documentBuilder.DefaultMargins.Top + 100,
				documentBuilder.DefaultMargins.Width,
				50), address);
			section.Items.Add(ti);

			ti = new TextItem(
				new System.Drawing.Rectangle(
				documentBuilder.DefaultMargins.Left,
				documentBuilder.DefaultMargins.Top + 150,
				documentBuilder.DefaultMargins.Width,
				50), "טופס חיוב");
			ti.Alignment = TextAlignment.Center;
			ti.Font = new System.Drawing.Font("Tahoma", 18,
				System.Drawing.FontStyle.Bold,
				System.Drawing.GraphicsUnit.Pixel);
			section.Items.Add(ti);


			double sum = 0;
			foreach (Sport.Entities.Charge charge in charges)
			{
				sum += charge.Amount * charge.Price;
			}
			Sport.UI.TableDesign tableDesign = new Sport.UI.TableDesign(new Entities.ChargeView());
			tableDesign.Columns.Add(new Sport.UI.TableDesign.Column((int)Sport.Entities.Charge.Fields.Product));
			tableDesign.Columns.Add(new Sport.UI.TableDesign.Column((int)Sport.Entities.Charge.Fields.Amount));
			tableDesign.Columns.Add(new Sport.UI.TableDesign.Column((int)Sport.Entities.Charge.Fields.Price));
			tableDesign.Columns.Add(new Sport.UI.TableDesign.Column((int)Sport.Entities.Charge.Fields.PriceTotal));
			TableItem tableItem = CreateTableItem(
				new System.Drawing.Rectangle(
					documentBuilder.DefaultMargins.Left + 100,
					documentBuilder.DefaultMargins.Top + 200,
					documentBuilder.DefaultMargins.Width - 200,
					documentBuilder.DefaultMargins.Height - 400),
				tableDesign, charges);
			TableItem.TableCell[] sumRowCells = new Sport.Documents.TableItem.TableCell[4];
			sumRowCells[0] = new Sport.Documents.TableItem.TableCell("סה\"כ חיוב:");
			sumRowCells[3] = new Sport.Documents.TableItem.TableCell(sum.ToString());
			sumRowCells[3].Border = System.Drawing.SystemPens.WindowFrame;
			TableItem.TableRow sumRow = new Sport.Documents.TableItem.TableRow(sumRowCells);
			sumRow.Border = null;
			tableItem.Rows.Add(sumRow);
			section.Items.Add(tableItem);

			ti = new TextItem(
				new System.Drawing.Rectangle(
					documentBuilder.DefaultMargins.Left,
					documentBuilder.DefaultMargins.Bottom - 80,
					100,
					80),
				"חתימה");
			ti.Font = new System.Drawing.Font("Tahoma", 18, System.Drawing.GraphicsUnit.Pixel);
			ti.Alignment = TextAlignment.Center;
			ti.Border = System.Drawing.SystemPens.WindowFrame;
			ti.Borders = Borders.Top;
			section.Items.Add(ti);
			documentBuilder.Sections.Add(section);
		}

		public void CreateReceiptSection(DocumentBuilder documentBuilder,
			Sport.Entities.Receipt receipt, int copies, string strRegionName, Sport.Entities.Season season)
		{
			if (receipt == null)
				return;

			Sport.Entities.Account account = receipt.Account;
			Sport.Entities.School school = account.School;

			Section section = new Section();

			// Logo image
			System.Drawing.Image logo = (System.Drawing.Image)Sport.Resources.ImageLists.GetLogo();
			ImageItem ii = new ImageItem(new System.Drawing.Rectangle(documentBuilder.DefaultMargins.Right - 200, 25, 200, 40), logo);
			ii.ImagePosition = ImagePosition.Normal;
			section.Items.Add(ii);

			// Getting year string
			string year = season.Name + " " + season.Start.Year + "-" + (season.Start.Year + 1).ToString();

			// Year
			TextItem ti = new TextItem(
				new System.Drawing.Rectangle(
				documentBuilder.DefaultMargins.Left + 150,
				documentBuilder.DefaultMargins.Top - 50,
				documentBuilder.DefaultMargins.Width - 300,
				50), "מחוז " + strRegionName + "\n" + year);
			ti.Alignment = TextAlignment.Center;
			section.Items.Add(ti);

			// Isf name
			ti = new TextItem(
				new System.Drawing.Rectangle(
				documentBuilder.DefaultMargins.Left,
				documentBuilder.DefaultMargins.Top - 50,
				150,
				50), "התאחדות הספורט\nלבתי ספר בישראל(ע\"ר)");
			section.Items.Add(ti);

			string address;
			if (school != null)
				address = "לכבוד:\nביה\"ס " + school.Name + "\n" + school.Address;
			else
				address = "לכבוד:\n" + account.AccountName + "\n" + account.Address;

			// Address
			ti = new TextItem(
				new System.Drawing.Rectangle(
				documentBuilder.DefaultMargins.Left + 150,
				documentBuilder.DefaultMargins.Top + 55,
				documentBuilder.DefaultMargins.Width - 150,
				50), address);
			section.Items.Add(ti);

			string irsNumber = Utils.GetAppSetting("irs");

			// Irs number and receipt date
			ti = new TextItem(
				new System.Drawing.Rectangle(
				documentBuilder.DefaultMargins.Left,
				documentBuilder.DefaultMargins.Top,
				150,
				40), "תיק מ\"ה: " + irsNumber + "\nתאריך: " + receipt.Date.ToString("dd/MM/yyyy"));
			section.Items.Add(ti);

			// Receipt number title
			ti = new TextItem(
				new System.Drawing.Rectangle(
				documentBuilder.DefaultMargins.Left,
				documentBuilder.DefaultMargins.Top + 150,
				documentBuilder.DefaultMargins.Width,
				50), "קבלה מספר " + receipt.Number);
			ti.Alignment = TextAlignment.Center;
			ti.Font = new System.Drawing.Font("Tahoma", 18,
				System.Drawing.FontStyle.Bold,
				System.Drawing.GraphicsUnit.Pixel);
			section.Items.Add(ti);

			// Payments table
			Sport.Entities.Payment[] payments = receipt.GetPayments();
			bool blnCreditCardOnly = (payments.Length > 0);
			foreach (Sport.Entities.Payment payment in payments)
			{
				if (payment.PaymentType != Sport.Types.PaymentType.CreditCard)
				{
					blnCreditCardOnly = false;
					break;
				}
			}
			Sport.UI.TableDesign tableDesign = new Sport.UI.TableDesign(new Entities.PaymentView());
			tableDesign.Columns.Add(new Sport.UI.TableDesign.Column((int)Sport.Entities.Payment.Fields.Type));
			tableDesign.Columns.Add(new Sport.UI.TableDesign.Column((int)Sport.Entities.Payment.Fields.Sum));
			if (blnCreditCardOnly)
			{
				tableDesign.Columns.Add(new Sport.UI.TableDesign.Column((int)Sport.Entities.Payment.Fields.CreditCardType));
				tableDesign.Columns.Add(new Sport.UI.TableDesign.Column((int)Sport.Entities.Payment.Fields.CreditCardNumber));
				tableDesign.Columns.Add(new Sport.UI.TableDesign.Column((int)Sport.Entities.Payment.Fields.CreditCardExpire));
				tableDesign.Columns.Add(new Sport.UI.TableDesign.Column((int)Sport.Entities.Payment.Fields.CreditCardPayments));
			}
			else
			{
				tableDesign.Columns.Add(new Sport.UI.TableDesign.Column((int)Sport.Entities.Payment.Fields.Bank));
				tableDesign.Columns.Add(new Sport.UI.TableDesign.Column((int)Sport.Entities.Payment.Fields.BankBranch));
				tableDesign.Columns.Add(new Sport.UI.TableDesign.Column((int)Sport.Entities.Payment.Fields.BankAccount));
				tableDesign.Columns.Add(new Sport.UI.TableDesign.Column((int)Sport.Entities.Payment.Fields.Reference));
				tableDesign.Columns.Add(new Sport.UI.TableDesign.Column((int)Sport.Entities.Payment.Fields.Date));
			}
			int paymentsTableTop = documentBuilder.DefaultMargins.Top + 200;
			TableItem tableItem = CreateTableItem(
				new System.Drawing.Rectangle(
				documentBuilder.DefaultMargins.Left,
				paymentsTableTop,
				documentBuilder.DefaultMargins.Width,
				documentBuilder.DefaultMargins.Height - 500),
				tableDesign, payments);

			TableItem.TableCell[] sumRowCells = new Sport.Documents.TableItem.TableCell[2];
			sumRowCells[0] = new Sport.Documents.TableItem.TableCell("סה\"כ לקבלה:");
			sumRowCells[0].BackColor = System.Drawing.Color.LightYellow;
			sumRowCells[1] = new Sport.Documents.TableItem.TableCell(receipt.Sum.ToString());
			sumRowCells[1].BackColor = System.Drawing.Color.LightYellow;
			TableItem.TableRow sumRow = new Sport.Documents.TableItem.TableRow(sumRowCells);
			sumRow.Border = null;
			tableItem.Rows.Add(sumRow);
			section.Items.Add(tableItem);

			// Credits
			Sport.Entities.Credit[] credits = receipt.GetCredits();
			if (credits.Length > 1)
			{
				int creditsTableTop = paymentsTableTop + (25 * (payments.Length + 1)) + 45;
				int creditsTableWidth = documentBuilder.DefaultMargins.Width - 50;
				System.Drawing.Font creditsFont =
					new System.Drawing.Font("Tahoma", 14, System.Drawing.FontStyle.Bold,
					System.Drawing.GraphicsUnit.Pixel);
				ti = new TextItem(
					new System.Drawing.Rectangle(
					documentBuilder.DefaultMargins.Left + 25,
					creditsTableTop,
					creditsTableWidth, 25),
					"שולם עבור המוסדות הבאים:");
				ti.Font = creditsFont;
				section.Items.Add(ti);

				tableItem = new Sport.Documents.TableItem(new System.Drawing.Rectangle(
					documentBuilder.DefaultMargins.Left + 25, creditsTableTop + 30, creditsTableWidth, 25 * credits.Length));
				tableItem.RelativeColumnWidth = true;
				string[] arrHeaders = new string[] { "סמל מוסד", "שם מוסד", "סכום" };
				double[] arrWidths = new double[] { 0.15, 0.7, 0.15 };
				Sport.Documents.TableItem.TableCell[] header = new Sport.Documents.TableItem.TableCell[arrHeaders.Length];
				for (int i = 0; i < arrHeaders.Length; i++)
				{
					Sport.Documents.TableItem.TableColumn tc = new Sport.Documents.TableItem.TableColumn();
					tc.Width = (int)(creditsTableWidth * arrWidths[i]);
					tc.Alignment = Sport.Documents.TextAlignment.Near;
					tableItem.Columns.Add(tc);
					header[i] = new Sport.Documents.TableItem.TableCell(arrHeaders[i]);
					header[i].Border = SystemPens.WindowFrame;
				}

				Sport.Documents.TableItem.TableRow headerRow = new Sport.Documents.TableItem.TableRow(header);
				headerRow.BackColor = System.Drawing.Color.SkyBlue;
				headerRow.Font = new System.Drawing.Font("Tahoma", 12, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
				headerRow.Alignment = Sport.Documents.TextAlignment.Center;
				headerRow.LineAlignment = Sport.Documents.TextAlignment.Center;
				tableItem.Rows.Add(headerRow);

				foreach (Sport.Entities.Credit credit in credits)
				{
					Sport.Entities.Account creditAccount = credit.Account;
					if (creditAccount == null)
						continue;
					if ((receipt.Account != null) && (creditAccount.Id == receipt.Account.Id))
						continue;
					string[] record = new string[arrHeaders.Length];
					string strSchoolSymbol = "";
					string strAccountName = "";
					string strPayment = credit.Sum.ToString();
					strAccountName = creditAccount.Name;
					int schoolID = -1;
					if (creditAccount.Entity.Fields[(int)Sport.Entities.Account.Fields.School] != null)
						schoolID = (int)creditAccount.Entity.Fields[(int)Sport.Entities.Account.Fields.School];
					if (schoolID >= 0)
					{
						Sport.Data.Entity schoolEnt = null;
						try
						{
							schoolEnt = Sport.Entities.School.Type.Lookup(schoolID);
						}
						catch
						{ }
						if (schoolEnt != null)
						{
							strSchoolSymbol = Sport.Common.Tools.CStrDef(schoolEnt.Fields[(int)Sport.Entities.School.Fields.Symbol], "");
							strAccountName = schoolEnt.Name;
						}
					}
					record[0] = strSchoolSymbol;
					record[1] = strAccountName;
					record[2] = strPayment;
					tableItem.Rows.Add(record);
				}

				section.Items.Add(tableItem);
			}

			// Remarks
			System.Drawing.Font remarksFont =
				new System.Drawing.Font("Tahoma", 15, System.Drawing.FontStyle.Bold,
				System.Drawing.GraphicsUnit.Pixel);
			ti = new TextItem(
				new System.Drawing.Rectangle(
				documentBuilder.DefaultMargins.Left + 25,
				documentBuilder.DefaultMargins.Bottom - 400,
				documentBuilder.DefaultMargins.Width - 50, 25),
				"הערות");
			ti.Font = remarksFont;
			section.Items.Add(ti);
			ti = new TextItem(
				new System.Drawing.Rectangle(
				documentBuilder.DefaultMargins.Left + 25,
				documentBuilder.DefaultMargins.Bottom - 370,
				documentBuilder.DefaultMargins.Width - 50, 170),
				receipt.Remarks);
			ti.Border = System.Drawing.SystemPens.WindowFrame;
			ti.Borders = Borders.All;
			ti.Font = remarksFont;
			section.Items.Add(ti);

			int width = (documentBuilder.DefaultMargins.Width - 40) / 3;

			// Signature
			ti = new TextItem(
				new System.Drawing.Rectangle(
				documentBuilder.DefaultMargins.Left,
				documentBuilder.DefaultMargins.Bottom - 80,
				width,
				80),
				"חתימה");
			ti.Font = new System.Drawing.Font("Tahoma", 18, System.Drawing.GraphicsUnit.Pixel);
			ti.Alignment = TextAlignment.Center;
			ti.Border = System.Drawing.SystemPens.WindowFrame;
			ti.Borders = Borders.Top;
			section.Items.Add(ti);

			// Stamp
			ti = new TextItem(
				new System.Drawing.Rectangle(
				documentBuilder.DefaultMargins.Left + width + 20,
				documentBuilder.DefaultMargins.Bottom - 80,
				width,
				80),
				"חותמת");
			ti.Font = new System.Drawing.Font("Tahoma", 18, System.Drawing.GraphicsUnit.Pixel);
			ti.Alignment = TextAlignment.Center;
			ti.Border = System.Drawing.SystemPens.WindowFrame;
			ti.Borders = Borders.Top;
			section.Items.Add(ti);

			// Name
			ti = new TextItem(
				new System.Drawing.Rectangle(
				documentBuilder.DefaultMargins.Right - width,
				documentBuilder.DefaultMargins.Bottom - 80,
				width,
				80),
				"שם");
			ti.Font = new System.Drawing.Font("Tahoma", 18, System.Drawing.GraphicsUnit.Pixel);
			ti.Alignment = TextAlignment.Center;
			ti.Border = System.Drawing.SystemPens.WindowFrame;
			ti.Borders = Borders.Top;
			section.Items.Add(ti);

			// Footer Title
			ti = new FieldTextItem("{" + ((int)TextField.Title).ToString() + "}");
			ti.Bounds = new System.Drawing.Rectangle(documentBuilder.DefaultMargins.Right - (documentBuilder.DefaultMargins.Width - 160), documentBuilder.DefaultMargins.Bottom + 20, documentBuilder.DefaultMargins.Width - 160, 20);
			section.Items.Add(ti);

			// Footer Page Number
			ti = new FieldTextItem("עמוד {" + ((int)TextField.SectionPage).ToString() +
				"} מתוך {" + ((int)TextField.SectionPageCount).ToString() + "}");
			ti.Bounds = new System.Drawing.Rectangle(documentBuilder.DefaultMargins.Left, documentBuilder.DefaultMargins.Bottom + 20, 150, 20);
			ti.Alignment = TextAlignment.Far;
			section.Items.Add(ti);

			for (int copy = 0; copy < copies; copy++)
			{
				Section copySection = new Section(section);
				copySection.SectionBorder = System.Drawing.SystemPens.WindowFrame;
				copySection.SectionBorders = Borders.All;
				if (copy == 0)
				{
					ti = new TextItem(
						new System.Drawing.Rectangle(
						documentBuilder.DefaultMargins.Left,
						documentBuilder.DefaultMargins.Top + 40,
						150,
						50), "****מקור");
				}
				else
				{
					ti = new TextItem(
						new System.Drawing.Rectangle(
						documentBuilder.DefaultMargins.Left,
						documentBuilder.DefaultMargins.Top + 50,
						150,
						50), "העתק " + copy.ToString());
				}

				ti.Alignment = TextAlignment.Far;
				copySection.Items.Add(ti);
				documentBuilder.Sections.Add(copySection);
			}
		}

		public void CreateDebtorsSection(DocumentBuilder documentBuilder,
			Sport.Entities.Account[] accounts)
		{
			if ((accounts == null) || (accounts.Length == 0))
				return;

			Sport.Entities.Region region = accounts[0].Region;
			string strRegionName = (region == null) ? "" : region.Name;
			Section section = CreateSection(documentBuilder);
			if (strRegionName.Length > 0)
				strRegionName = " מחוז " + strRegionName;

			Font font = new System.Drawing.Font("Tahoma", 18,
				System.Drawing.FontStyle.Bold,
				System.Drawing.GraphicsUnit.Pixel);

			TextItem ti = new TextItem(
				new System.Drawing.Rectangle(
				documentBuilder.DefaultMargins.Left,
				documentBuilder.DefaultMargins.Top + 10,
				documentBuilder.DefaultMargins.Width,
				30), "דו\"ח חייבים" + strRegionName);
			ti.Alignment = TextAlignment.Center;
			ti.Font = font;
			section.Items.Add(ti);

			double sum = 0;
			foreach (Sport.Entities.Account account in accounts)
				sum += account.Balance;

			Sport.UI.TableDesign tableDesign = new Sport.UI.TableDesign(new Entities.AccountView());
			tableDesign.Columns.Add(new Sport.UI.TableDesign.Column((int)Sport.Entities.Account.Fields.Name));
			tableDesign.Columns.Add(new Sport.UI.TableDesign.Column((int)Sport.Entities.Account.Fields.School));
			tableDesign.Columns.Add(new Sport.UI.TableDesign.Column((int)Sport.Entities.Account.Fields.Balance));
			Array.Sort(accounts, new AccountBalanceComparer());
			TableItem tableItem = CreateTableItem(
				new System.Drawing.Rectangle(
				documentBuilder.DefaultMargins.Left + 20,
				documentBuilder.DefaultMargins.Top + 50,
				documentBuilder.DefaultMargins.Width - 20,
				documentBuilder.DefaultMargins.Height - 100),
				tableDesign, accounts);
			TableItem.TableCell[] sumRowCells =
				new Sport.Documents.TableItem.TableCell[3];
			sumRowCells[0] = new Sport.Documents.TableItem.TableCell("סה\"כ חוב:");
			sumRowCells[2] =
				new Sport.Documents.TableItem.TableCell(Math.Abs(sum).ToString() + " ₪");
			sumRowCells[2].Border = System.Drawing.SystemPens.WindowFrame;
			TableItem.TableRow sumRow =
				new Sport.Documents.TableItem.TableRow(sumRowCells);
			sumRow.Border = null;
			tableItem.Rows.Add(sumRow);
			section.Items.Add(tableItem);

			documentBuilder.Sections.Add(section);
		}

		public void CreateAccountSection(DocumentBuilder documentBuilder, Sport.Entities.Account account,
			Sport.Entities.AccountEntry[] entries)
		{
			if (account == null || account.Id < 0 || entries.Length == 0)
				return;

			System.Collections.ArrayList arrEntries = new System.Collections.ArrayList(entries);
			arrEntries.Sort(new EntryDateComparer());
			entries = (Sport.Entities.AccountEntry[])arrEntries.ToArray(typeof(Sport.Entities.AccountEntry));

			string strAccountName = "";
			if (account.School != null)
				strAccountName = account.School.Name;
			else
				strAccountName = account.Name;

			Section section = CreateSection(documentBuilder);

			Font font = new System.Drawing.Font("Tahoma", 18,
				System.Drawing.FontStyle.Bold,
				System.Drawing.GraphicsUnit.Pixel);

			TextItem ti = new TextItem(
				new System.Drawing.Rectangle(
				documentBuilder.DefaultMargins.Left,
				documentBuilder.DefaultMargins.Top + 10,
				documentBuilder.DefaultMargins.Width,
				30), "דו\"ח מצב חשבון: " + strAccountName);
			ti.Alignment = TextAlignment.Center;
			ti.Font = font;
			section.Items.Add(ti);

			TableItem table = new TableItem();
			table.RelativeColumnWidth = false;
			table.Direction = Sport.Documents.Direction.Left;
			table.Bounds = new System.Drawing.Rectangle(documentBuilder.DefaultMargins.Left + 20,
				documentBuilder.DefaultMargins.Top + 50,
				documentBuilder.DefaultMargins.Width - 20,
				documentBuilder.DefaultMargins.Height - 100);
			string[] captions = new string[] { "תיאור", "קטגוריה", "סוג", "סכום", "סה\"כ", "תאריך" };
			double[] cellsWidth = new double[] { 0.25, 0.25, 0.1, 0.1, 0.15, 0.15 };
			for (int i = 0; i < captions.Length; i++)
			{
				TableItem.TableColumn column = new TableItem.TableColumn();
				column.Width = (int)(table.Bounds.Width * cellsWidth[i]);
				table.Columns.Add(column);
			}

			TableItem.TableRow row = new TableItem.TableRow(captions);
			row.Alignment = TextAlignment.Center;
			row.Font = new System.Drawing.Font("Tahoma", 14,
				System.Drawing.FontStyle.Bold,
				System.Drawing.GraphicsUnit.Pixel);
			table.Rows.Add(row);

			font = new System.Drawing.Font("Tahoma", 14, System.Drawing.GraphicsUnit.Pixel);
			Sport.Types.AccountEntryTypeLookup entryTypeLookup = new Sport.Types.AccountEntryTypeLookup();
			Sport.Entities.AccountEntry entry;
			Sport.Entities.ChampionshipCategory category;
			int totalSum = 0;
			int curSum;
			string strDescription;
			string strCategoty;
			string strEntryType;
			string strPrice;
			string strTotal;
			string strDate;
			string[] values;
			for (int i = 0; i < entries.Length; i++)
			{
				entry = entries[i];
				curSum = Math.Abs((int)entry.Sum);
				switch (entry.EntryType)
				{
					case Sport.Types.AccountEntryType.Debit:
						totalSum -= curSum;
						break;
					case Sport.Types.AccountEntryType.Credit:
						totalSum += curSum;
						break;
				}
				category = entry.Category;
				strDescription = entry.Description;
				strCategoty = (category == null) ? ("") : (category.Championship.Name + " " + category.Name);
				strEntryType = entryTypeLookup.Lookup((int)entry.EntryType);
				strPrice = curSum.ToString();
				strTotal = totalSum.ToString();
				if (strTotal[0] == '-')
					strTotal = strTotal.Substring(1, strTotal.Length - 1) + "-";
				strDate = entry.EntryDate.ToString("dd/MM/yyyy");
				values = new string[] { strDescription, strCategoty, strEntryType, strPrice, strTotal, strDate };
				row = new TableItem.TableRow(values);
				row.Font = font;
				table.Rows.Add(row);
			}

			strTotal = Math.Abs(totalSum).ToString();
			if (totalSum > 0)
				strTotal += " זכות";
			else if (totalSum < 0)
				strTotal += " חובה";
			values = new string[] { "", "", "", "סה\"כ", strTotal, "" };
			row = new TableItem.TableRow(values);
			row.Font = font;
			table.Rows.Add(row);

			section.Items.Add(table);
			documentBuilder.Sections.Add(section);
		}

		private class AccountBalanceComparer : System.Collections.IComparer
		{
			public int Compare(object x, object y)
			{
				Sport.Entities.Account a1 = (Sport.Entities.Account)x;
				Sport.Entities.Account a2 = (Sport.Entities.Account)y;
				return a1.Balance.CompareTo(a2.Balance);
			}
		}

		private class EntryDateComparer : System.Collections.IComparer
		{
			public int Compare(object x, object y)
			{
				Sport.Entities.AccountEntry a1 = (Sport.Entities.AccountEntry)x;
				Sport.Entities.AccountEntry a2 = (Sport.Entities.AccountEntry)y;
				return a1.EntryDate.CompareTo(a2.EntryDate);
			}
		}
	}
}