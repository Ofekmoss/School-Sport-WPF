using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Sport.Data;
using System.IO;
using System.Diagnostics;

namespace Sport.UI.Dialogs
{
	public partial class ExportReceiptsDialog : Form
	{
		//select * from RECEIPTS
		//where CAST(LEFT(NUMBER, CHARINDEX('-', NUMBER) - 1) AS int)>9200 and DATE_DELETED IS NULL

		private readonly string[] _captions = new string[] { "אסמכתא 1", "אסמכתא 2", "תאריך", "ת. ערך", "חשבון חובה", "חשבון זכות", "פרטים", "₪" };
		private readonly string[] _receiptRemarkNoiseWords = new string[] { "תשלום עבור ", "שולם עבור ", "לשנה\"ל ", "שנה\"ל ", "תשלום ", "רישום ל" };
		private readonly int MAX_RECENT_FILES = 5;

		private int _startReceiptNumber = 0;
		private bool _dataChanged = false;

		private List<ExportedReceiptData> _rows = new List<ExportedReceiptData>();
		private List<string> _recentReportFiles = new List<string>();

		public ExportReceiptsDialog(int startReceiptNumber)
		{
			_startReceiptNumber = startReceiptNumber;

			InitializeComponent();
		}

		private void ExportReceiptsDialog_Load(object sender, EventArgs e)
		{
			_rows.Clear();
			_recentReportFiles.Clear();

			Sport.UI.Dialogs.WaitForm.ShowWait("טוען קבלות אנא המתן...", true);
			Entity[] payments = Sport.Entities.Payment.Type.GetEntities(null);
			Sport.UI.Dialogs.WaitForm.HideWait();

			//DateTime date1 = DateTime.Now;
			Sport.UI.Dialogs.WaitForm.ShowWait("מעבד נתונים אנא המתן...", true);
			Sport.Entities.DataServices.ReceiptData_Basic[] receiptRawData = Sport.Entities.Receipt.GetReceiptsByNumber_Basic(_startReceiptNumber + 1);
			Dictionary<string, Sport.Entities.DataServices.ReceiptData_Basic> receipts = new Dictionary<string, Sport.Entities.DataServices.ReceiptData_Basic>();
			foreach (Sport.Entities.DataServices.ReceiptData_Basic basicData in receiptRawData)
				receipts.Add(basicData.ID, basicData);

			//each receipt might contain several payments.
			foreach (Entity payment in payments)
			{
				string rawNumber = payment.Fields[(int)Sport.Entities.Payment.Fields.Receipt] + "";
				int receiptID = -1;
				if (rawNumber.Length > 0)
					receiptID = Int32.Parse(rawNumber);
				if (receiptID > _startReceiptNumber)
				{
					if (receipts.ContainsKey(rawNumber))
					{
						//found receipt matching to this payment.. add to grid:
						AddReceiptRow(receipts[rawNumber], rawNumber, payment);
					}
				}
			}
			Sport.UI.Dialogs.WaitForm.HideWait();
			//DateTime date2 = DateTime.Now;
			//MessageBox.Show("took " + (date2 - date1).TotalMilliseconds + " milliseconds");

			ReceiptsGrid.DataSource = _rows;
			for (int i = 0; i < _captions.Length; i++)
				ReceiptsGrid.Columns[i].HeaderText = _captions[i];

			//recent files list:
			string strAllRecentReports = Sport.Core.Configuration.ReadString("General", "ReceiptExportRecentFiles") + "";
			if (strAllRecentReports.Length > 0)
			{
				_recentReportFiles.AddRange(strAllRecentReports.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
				while (_recentReportFiles.Count > MAX_RECENT_FILES)
					_recentReportFiles.RemoveAt(_recentReportFiles.Count - 1);
			}

			FillRecentFilesCombo();
		}

		private void ExportReceiptsDialog_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (e.CloseReason == CloseReason.UserClosing || e.CloseReason == CloseReason.None)
			{
				if (_dataChanged)
				{
					e.Cancel = !Sport.UI.MessageBox.Ask("האם לחזור למסך קבלות? נתונים לא נשמרו", false);
				}
			}
		}

		private void tbCancel_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
		}

		private void tbOk_Click(object sender, EventArgs e)
		{
			string lastFolderPath = Sport.Core.Configuration.ReadString("General", "LastReceiptExportFolder") + "";
			if (lastFolderPath.Length > 0)
				folderBrowserDialog1.SelectedPath = lastFolderPath;
			if (folderBrowserDialog1.ShowDialog(this) == DialogResult.OK)
			{
				string folderPath = folderBrowserDialog1.SelectedPath;
				if (!folderPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
					folderPath += Path.DirectorySeparatorChar.ToString();
				string filePath = string.Format("{0}SportsmanReceipts_{1}.xls", folderPath, _startReceiptNumber);
				bool blnSuccess = false;
				try
				{
					Sport.ExcelParser.ExcelParser parser = new Sport.ExcelParser.ExcelParser(filePath, false);
					if (WriteToExcel(parser))
					{
						parser.refitAll();
						parser.closeFile();
						blnSuccess = true;
					}
					else
					{
						parser.closeFile(false);
					}
				}
				catch (Exception ex)
				{
					Sport.UI.MessageBox.Error("שמירת קובץ נכשלה", "ייצוא קבלות");
					AdvancedTools.ReportExcpetion(ex, "Failed to export receipts");
				}

				if (blnSuccess)
				{
					Sport.UI.MessageBox.Success("קובץ נשמר בהצלחה", "ייצוא קבלות");
					try
					{
						SaveTextFormat(filePath);
					}
					catch (Exception ex)
					{
						ShowError(ex.ToString());
					}
					StoreReceiptReportConfiguration(filePath);
					_dataChanged = false;
					System.Diagnostics.Process.Start("explorer.exe", "/select, \"" + filePath + "\"");
					this.DialogResult = DialogResult.OK;
				}
			}
		}

		private void ShowError(string errMessage)
		{
			if (errMessage.Length == 0)
			{
				lbError.Visible = false;
			}
			else
			{
				lbError.Text = errMessage;
				lbError.Visible = true;
			}
		}

		private void SaveTextFormat(string excelFilePath)
		{
			string textFilePath = Path.ChangeExtension(excelFilePath, "txt");
			List<string> lines = new List<string>();
			List<string> items = new List<string>();
			for (int i = 0; i < ReceiptsGrid.Columns.Count; i++)
				items.Add(ReceiptsGrid.Columns[i].HeaderText);
			lines.Add(string.Join("|", items.ToArray()));
			for (int i = 0; i < ReceiptsGrid.Rows.Count; i++)
			{
				items.Clear();
				for (int j = 0; j < ReceiptsGrid.Columns.Count; j++)
					items.Add(ReceiptsGrid.Rows[i].Cells[j].FormattedValue.ToString());
				lines.Add(string.Join("|", items.ToArray()));
			}
			File.WriteAllLines(textFilePath, lines.ToArray());
		}

		private void StoreReceiptReportConfiguration(string filePath)
		{
			string folderPath = Path.GetDirectoryName(filePath);
			Sport.Core.Configuration.WriteString("General", "LastReceiptExportFolder", folderPath);

			//remove if exists.. will add anyway
			_recentReportFiles.RemoveAll(delegate(string s) { return s.Equals(filePath, StringComparison.CurrentCultureIgnoreCase); });

			if (_recentReportFiles.Count < MAX_RECENT_FILES)
			{
				_recentReportFiles.Insert(0, filePath);
			}
			else
			{
				//push at beginning:
				for (int i = 0; i < _recentReportFiles.Count - 1; i++)
					_recentReportFiles[i + 1] = _recentReportFiles[i];
				_recentReportFiles[0] = filePath;
			}
			Sport.Core.Configuration.WriteString("General", "ReceiptExportRecentFiles", string.Join(",", _recentReportFiles.ToArray()));
		}

		private bool WriteToExcel(Sport.ExcelParser.ExcelParser parser)
		{
			//headers
			for (int colIndex = 0; colIndex < ReceiptsGrid.Columns.Count; colIndex++)
				parser.writeToCell(1, colIndex + 1, ReceiptsGrid.Columns[colIndex].HeaderText);

			//rows
			Sport.UI.Dialogs.WaitForm.ShowWait("מייצר קובץ אקסל אנא המתן...", true);
			int perecentRows = (int)((ReceiptsGrid.Rows.Count / 100) + 0.5);
			int curPercentage = 0;
			for (int rowIndex = 0; rowIndex < ReceiptsGrid.Rows.Count; rowIndex++)
			{
				if (perecentRows <= 1 || (rowIndex % perecentRows == 0))
				{
					Sport.UI.Dialogs.WaitForm.SetProgress(curPercentage);
					if (Sport.UI.Dialogs.WaitForm.Cancelled)
						return false;
					curPercentage++;
				}
				for (int colIndex = 0; colIndex < ReceiptsGrid.Columns.Count; colIndex++)
				{
					parser.writeToCell(rowIndex + 2, colIndex + 1, ReceiptsGrid.Rows[rowIndex].Cells[colIndex].FormattedValue.ToString());
				}
			}
			Sport.UI.Dialogs.WaitForm.SetProgress(100);
			Sport.UI.Dialogs.WaitForm.HideWait();
			return true;
		}

		private void FillRecentFilesCombo()
		{
			while (cbRecentFiles.Items.Count > 1)
				cbRecentFiles.Items.RemoveAt(1);

			foreach (string filePath in _recentReportFiles)
				cbRecentFiles.Items.Add(filePath);

			cbRecentFiles.SelectedIndex = 0;
		}

		private void AddReceiptRow(Sport.Entities.DataServices.ReceiptData_Basic receipt, string rawReceiptNumber, Entity payment)
		{
			ExportedReceiptData data = new ExportedReceiptData();

			//assign the receipt number and date as given by service:
			data.ReceiptNumber = rawReceiptNumber;
			data.ReceiptDate = receipt.Date;

			//check the payment type, assign proper data accordingly.
			int type = (int)payment.Fields[(int)Sport.Entities.Payment.Fields.Type];
			if (type == (int)Sport.Types.PaymentType.Cheque)
			{
				//Checque should have due date and its own number:
				data.ChequeDueDate = ((DateTime)payment.Fields[(int)Sport.Entities.Payment.Fields.Date]).ToString("dd/MM/yyyy");
				data.ChequeNumber = payment.Fields[(int)Sport.Entities.Payment.Fields.Reference] + "";
			}

			//need to parse the payment sum from its raw value:
			data.ReceiptShekels = double.Parse("0" + payment.Fields[(int)Sport.Entities.Payment.Fields.Sum]);

			//assign details field:
			data.Details = BuildPaymentDetails(type, receipt);

			//done, add to grid:
			_rows.Add(data);
		}

		private string BuildPaymentDetails(int paymentType, Sport.Entities.DataServices.ReceiptData_Basic receipt)
		{
			Dictionary<int, string> arrPaymentTypeMapping = new Dictionary<int, string>();
			arrPaymentTypeMapping.Add((int)Sport.Types.PaymentType.Cash, "מזומן");
			arrPaymentTypeMapping.Add((int)Sport.Types.PaymentType.BankTransfer, "העברה בנקאית");

			string strDetails = "קבלה ";
			if (arrPaymentTypeMapping.ContainsKey(paymentType))
				strDetails += arrPaymentTypeMapping[paymentType] + " ";

			if (!string.IsNullOrEmpty(receipt.Account))
			{
				Entity entAccount = TryGetEntity(Sport.Entities.Account.Type, Int32.Parse(receipt.Account));
				if (entAccount != null)
				{
					string strAccountName = string.Empty;
					string schoolId_Raw = entAccount.Fields[(int)Sport.Entities.Account.Fields.School] + "";
					if (!string.IsNullOrEmpty(schoolId_Raw))
					{
						Entity entSchool = TryGetEntity(Sport.Entities.School.Type, Int32.Parse(schoolId_Raw));
						if (entSchool != null)
							strAccountName = entSchool.Fields[(int)Sport.Entities.School.Fields.Name] + "";
					}

					if (strAccountName.Length == 0)
						strAccountName = (entAccount.Fields[(int)Sport.Entities.Account.Fields.Name] + "").Replace(" (משתתף מחנה אימון)", "");

					strDetails += strAccountName + " ";
				}
			}

			strDetails += ParseReceiptRemark(receipt.Remarks);

			return strDetails;
		}

		private string ParseReceiptRemark(string remarks)
		{
			string sResult = remarks;
			while (sResult.IndexOf("  ") >= 0)
				sResult = sResult.Replace("  ", " ");
			foreach (string noiseWord in _receiptRemarkNoiseWords)
				sResult = sResult.Replace(noiseWord, "");
			sResult = sResult.Trim();
			if (sResult.EndsWith("-"))
				sResult = sResult.Substring(0, sResult.Length - 1);
			return sResult;
		}

		private Entity TryGetEntity(EntityType type, int id)
		{
			Entity entity = null;

			try
			{
				entity = type.Lookup(id);
			}
			catch
			{ }

			//maybe no error but invalid
			if (entity != null && entity.Id != id)
				entity = null;

			return entity;
		}

		private void ReceiptsGrid_DataError(object sender, DataGridViewDataErrorEventArgs e)
		{
			Sport.UI.MessageBox.Warn("ערך זה אינו חוקי, אנא הכנס ערך אחר", "ייצוא קבלות");
			e.Cancel = true;
		}

		private void ReceiptsGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
		{
			//ReceiptsGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag = ReceiptsGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
		}

		private void ReceiptsGrid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
		{
			_dataChanged = true;
		}

		private void cbRecentFiles_SelectedIndexChanged(object sender, EventArgs e)
		{
			//hide details panel:
			pnlRecentFileDetails.Visible = false;

			int selIndex = cbRecentFiles.SelectedIndex;
			if (selIndex > 0)
			{
				string filePath = cbRecentFiles.Items[selIndex].ToString();
				if (File.Exists(filePath))
				{
					Process.Start(filePath);

					//fill and show details panel:
					lbRecentFileLastModified.Text = File.GetLastWriteTime(filePath).ToString("dd/MM/yyyy");
					btnReloadRecentFile.Enabled = File.Exists(Path.ChangeExtension(filePath, "txt"));
					pnlRecentFileDetails.Visible = true;
				}
				else
				{
					Sport.UI.MessageBox.Error("קובץ זה לא קיים, מוריד מהרשימה", "ייצוא קבלות");
					cbRecentFiles.Items.RemoveAt(selIndex);
					_recentReportFiles.RemoveAll(delegate(string s) { return s.Equals(filePath, StringComparison.CurrentCultureIgnoreCase); });

					//put back in config file:
					Sport.Core.Configuration.WriteString("General", "ReceiptExportRecentFiles", string.Join(",", _recentReportFiles.ToArray()));
				}
			}
		}

		private void btnReloadRecentFile_Click(object sender, EventArgs e)
		{
			int selIndex = cbRecentFiles.SelectedIndex;
			if (selIndex > 0)
			{
				string excelFilePath = cbRecentFiles.Items[selIndex].ToString();
				string textFilePath = Path.ChangeExtension(excelFilePath, "txt");
				if (File.Exists(textFilePath))
				{
					if (!_dataChanged || Sport.UI.MessageBox.Ask("נתונים לא נשמרו, האם לטעון קובץ ישן?", "ייצוא קבלות", false))
					{
						string[] lines = File.ReadAllLines(textFilePath);
						List<ExportedReceiptData> rows = new List<ExportedReceiptData>();
						for (int i = 1; i < lines.Length; i++)
						{
							string[] items = lines[i].Split('|');
							if (items.Length >= 8)
							{
								double sum;
								ExportedReceiptData data = new ExportedReceiptData();
								data.ChequeNumber = items[0];
								data.ReceiptNumber = items[1];
								data.ReceiptDate = items[2];
								data.ChequeDueDate = items[3];
								data.CreditAccount = items[5];
								data.Details = items[6];
								if (double.TryParse(items[7], out sum))
									data.ReceiptShekels = sum;
								rows.Add(data);
							}
						}
						ReceiptsGrid.DataSource = rows;
						_rows = new List<ExportedReceiptData>(rows.ToArray());
						_dataChanged = false;
					}
				}
			}
		}

		private void ReceiptsGrid_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Delete)
			{
				int count = ReceiptsGrid.SelectedRows.Count;
				if (count > 0)
				{
					List<string> numbers = new List<string>();
					foreach (DataGridViewRow row in ReceiptsGrid.SelectedRows)
						numbers.Add(((ExportedReceiptData)row.DataBoundItem).ReceiptNumber);
					string msg = "האם להוריד ";
					msg += (count == 1) ? "קבלה מספר " : "את הקבלות ";
					msg += string.Join(", ", numbers.ToArray()) + " ";
					msg += "מהייצוא?";
					if (MessageBox.Ask(msg, false))
					{
						List<int> indexesToRemove = new List<int>();
						foreach (DataGridViewRow row in ReceiptsGrid.SelectedRows)
							indexesToRemove.Add(row.Index);
						List<ExportedReceiptData> arrNewData = new List<ExportedReceiptData>();
						for (int i = 0; i < _rows.Count; i++)
						{
							if (!indexesToRemove.Contains(i))
							{
								arrNewData.Add(_rows[i]);
							}
						}
						//list.RemoveAll(delegate (MyTestData data) { return itemsToRemove.Contains(data); });
						ReceiptsGrid.DataSource = arrNewData;

						_rows.Clear();
						_rows.AddRange(arrNewData.ToArray());
					}
				}
			}
		}

		#region Exported Receipt Data
		protected class ExportedReceiptData
		{
			private string chequeNumber;
			public string ChequeNumber { get { return chequeNumber; } set { chequeNumber = value; } }

			private string receiptNumber;
			public string ReceiptNumber { get { return receiptNumber; } set { receiptNumber = value; } }

			private string receiptDate;
			public string ReceiptDate { get { return receiptDate; } set { receiptDate = value; } }

			private string chequeDueDate;
			public string ChequeDueDate { get { return chequeDueDate; } set { chequeDueDate = value; } }

			private string debitAccount;
			public string DebitAccount { get { return debitAccount; } set { debitAccount = value; } }

			private string creditAccount;
			public string CreditAccount { get { return creditAccount; } set { creditAccount = value; } }

			private string details;
			public string Details { get { return details; } set { details = value; } }

			private double receiptShekels;
			public double ReceiptShekels { get { return receiptShekels; } set { receiptShekels = value; } }

			public ExportedReceiptData()
			{
				this.ChequeNumber = string.Empty;
				this.ReceiptNumber = string.Empty;
				this.ReceiptDate = string.Empty;
				this.ChequeDueDate = string.Empty;
				this.DebitAccount = "10010";
				this.CreditAccount = string.Empty;
				this.Details = string.Empty;
				this.ReceiptShekels = 0;
			}

			public ExportedReceiptData(string chequeNumber, string receiptNumber, DateTime receiptDate,
				DateTime chequeDueDate, string details, double receiptShekels)
				: this()
			{
				this.ChequeNumber = chequeNumber;
				this.ReceiptNumber = receiptNumber;

				if (receiptDate.Year > 1900)
					this.ReceiptDate = receiptDate.ToString("dd/MM/yyyy");

				if (chequeDueDate.Year > 1900)
					this.ChequeDueDate = chequeDueDate.ToString("dd/MM/yyyy");

				this.Details = details;
				this.ReceiptShekels = receiptShekels;

				//clean:
				int index = this.ReceiptNumber.IndexOf("-");
				if (index > 0)
					this.ReceiptNumber = this.ReceiptNumber.Substring(0, index);
			}
		}
		#endregion
	}
}