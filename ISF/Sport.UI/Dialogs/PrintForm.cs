using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Sport.UI.Dialogs
{
	/// <summary>
	/// Summary description for PrintForm.
	/// </summary>
	public class PrintForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button btnClose;
		private Sport.UI.Controls.PrintPreview printPreview;
		private Sport.UI.Controls.RightToolBar toolBar;
		private System.Windows.Forms.ImageList imageList;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.ToolBarButton tbbFirst;
		private System.Windows.Forms.ToolBarButton tbbPrevious;
		private System.Windows.Forms.ToolBarButton tbbNext;
		private System.Windows.Forms.ToolBarButton tbbLast;
		private System.Windows.Forms.Label labelPage;
		private System.Windows.Forms.Label labelZoom;
		private System.Windows.Forms.ComboBox cbZoom;
		private System.Windows.Forms.ToolBarButton tbbPrint;
		private System.Windows.Forms.ToolBarButton tbbSave;

		private Sport.Documents.Document _document;
		private System.Drawing.Printing.PrinterSettings _settings;
		
		private System.Windows.Forms.SaveFileDialog saveDialog;
		private System.Windows.Forms.Button btnChoosePages;
		private System.Windows.Forms.Button btnResetPages;
		
		private int[] _pagesToPrint=null;
		private int _originalCount=0;
		private System.Windows.Forms.Label lbCopies;
		
		private bool canceled;
		public bool Canceled
		{
			get { return canceled; }
		}

		private PrintForm(System.Drawing.Printing.PrintDocument printDocument, string title)
		{
			InitializeComponent();

			Text = "הדפסה - " + title;
			
			printPreview.Document = printDocument;
			
			canceled = printPreview.Canceled;
			
			ResetDocument();
			
			cbZoom.Items.Add("400%");
			cbZoom.Items.Add("200%");
			cbZoom.Items.Add("100%");
			cbZoom.Items.Add("50%");
			cbZoom.Items.Add("התאם לרוחב");
			string zoom = ((int) (printPreview.Zoom * 100)).ToString() + "%";
			cbZoom.Items.Add(zoom);
			cbZoom.Text = zoom;
			
			_pagesToPrint = new int[printPreview.PageCount];
			for (int i=0; i<_pagesToPrint.Length; i++)
				_pagesToPrint[i] = i;
			
			_originalCount = printPreview.PageCount;
		}

		public PrintForm(Sport.Documents.Document document, System.Drawing.Printing.PrinterSettings settings)
			: this(document.CreatePrintDocument(settings), document.Title)
		{
			_document = document;
			_document.StoreCurrentPages();
			_settings = settings;
			
			if (_settings != null)
			{
				int copies=(int) _settings.Copies;
				if (_settings.Copies > 1)
				{
					lbCopies.Text = Sport.Common.Tools.HebrewCount(copies, true)+" עותקים";
					lbCopies.Visible = true;
				}
			}
		}

		private void SetPage(int page)
		{
			printPreview.Page = page;
			if (page >= 0)
			{
				labelPage.Text = "עמוד " + (page + 1).ToString() + " מתוך " + printPreview.PageCount.ToString();
			}
			else
			{
				labelPage.Text = "";
			}

			tbbFirst.Enabled = printPreview.Page > 0;
			tbbPrevious.Enabled = tbbFirst.Enabled;
			tbbLast.Enabled = printPreview.Page != -1 &&
				printPreview.Page < printPreview.PageCount - 1;
			tbbNext.Enabled = tbbLast.Enabled;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(PrintForm));
			this.btnClose = new System.Windows.Forms.Button();
			this.printPreview = new Sport.UI.Controls.PrintPreview();
			this.toolBar = new Sport.UI.Controls.RightToolBar();
			this.tbbFirst = new System.Windows.Forms.ToolBarButton();
			this.tbbPrevious = new System.Windows.Forms.ToolBarButton();
			this.tbbNext = new System.Windows.Forms.ToolBarButton();
			this.tbbLast = new System.Windows.Forms.ToolBarButton();
			this.tbbPrint = new System.Windows.Forms.ToolBarButton();
			this.tbbSave = new System.Windows.Forms.ToolBarButton();
			this.imageList = new System.Windows.Forms.ImageList(this.components);
			this.labelPage = new System.Windows.Forms.Label();
			this.labelZoom = new System.Windows.Forms.Label();
			this.cbZoom = new System.Windows.Forms.ComboBox();
			this.saveDialog = new System.Windows.Forms.SaveFileDialog();
			this.btnChoosePages = new System.Windows.Forms.Button();
			this.btnResetPages = new System.Windows.Forms.Button();
			this.lbCopies = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// btnClose
			// 
			this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnClose.Location = new System.Drawing.Point(8, 472);
			this.btnClose.Name = "btnClose";
			this.btnClose.TabIndex = 0;
			this.btnClose.Text = "סגור";
			// 
			// printPreview
			// 
			this.printPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.printPreview.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.printPreview.Document = null;
			this.printPreview.Location = new System.Drawing.Point(8, 40);
			this.printPreview.Name = "printPreview";
			this.printPreview.Page = -1;
			this.printPreview.Size = new System.Drawing.Size(616, 424);
			this.printPreview.TabIndex = 1;
			this.printPreview.Zoom = 1F;
			// 
			// toolBar
			// 
			this.toolBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.toolBar.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
																					   this.tbbFirst,
																					   this.tbbPrevious,
																					   this.tbbNext,
																					   this.tbbLast,
																					   this.tbbPrint,
																					   this.tbbSave});
			this.toolBar.ButtonSize = new System.Drawing.Size(25, 16);
			this.toolBar.Divider = false;
			this.toolBar.Dock = System.Windows.Forms.DockStyle.None;
			this.toolBar.DropDownArrows = true;
			this.toolBar.ImageList = this.imageList;
			this.toolBar.Location = new System.Drawing.Point(472, 8);
			this.toolBar.Name = "toolBar";
			this.toolBar.ShowToolTips = true;
			this.toolBar.Size = new System.Drawing.Size(152, 26);
			this.toolBar.TabIndex = 2;
			this.toolBar.TextAlign = System.Windows.Forms.ToolBarTextAlign.Right;
			this.toolBar.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.toolBar_ButtonClick);
			// 
			// tbbFirst
			// 
			this.tbbFirst.ImageIndex = 0;
			// 
			// tbbPrevious
			// 
			this.tbbPrevious.ImageIndex = 1;
			// 
			// tbbNext
			// 
			this.tbbNext.ImageIndex = 2;
			// 
			// tbbLast
			// 
			this.tbbLast.ImageIndex = 3;
			// 
			// tbbPrint
			// 
			this.tbbPrint.ImageIndex = 4;
			// 
			// tbbSave
			// 
			this.tbbSave.ImageIndex = 5;
			// 
			// imageList
			// 
			this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			this.imageList.ImageSize = new System.Drawing.Size(16, 16);
			this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
			this.imageList.TransparentColor = System.Drawing.Color.FromArgb(((System.Byte)(0)), ((System.Byte)(128)), ((System.Byte)(255)));
			// 
			// labelPage
			// 
			this.labelPage.AutoSize = true;
			this.labelPage.Location = new System.Drawing.Point(8, 18);
			this.labelPage.Name = "labelPage";
			this.labelPage.Size = new System.Drawing.Size(34, 17);
			this.labelPage.TabIndex = 3;
			this.labelPage.Text = "label1";
			this.labelPage.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelZoom
			// 
			this.labelZoom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelZoom.Location = new System.Drawing.Point(416, 18);
			this.labelZoom.Name = "labelZoom";
			this.labelZoom.Size = new System.Drawing.Size(48, 16);
			this.labelZoom.TabIndex = 4;
			this.labelZoom.Text = "הגדלה:";
			// 
			// cbZoom
			// 
			this.cbZoom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cbZoom.Location = new System.Drawing.Point(304, 13);
			this.cbZoom.Name = "cbZoom";
			this.cbZoom.Size = new System.Drawing.Size(104, 21);
			this.cbZoom.Sorted = true;
			this.cbZoom.TabIndex = 5;
			this.cbZoom.SelectedValueChanged += new System.EventHandler(this.cbZoom_SelectedValueChanged);
			// 
			// saveDialog
			// 
			this.saveDialog.Filter = "Word 2003 File|*.doc|Excel File|*.xls";
			this.saveDialog.Title = "שמירה לקובץ...";
			// 
			// btnChoosePages
			// 
			this.btnChoosePages.Location = new System.Drawing.Point(200, 8);
			this.btnChoosePages.Name = "btnChoosePages";
			this.btnChoosePages.Size = new System.Drawing.Size(96, 23);
			this.btnChoosePages.TabIndex = 6;
			this.btnChoosePages.Text = "בחירת עמודים";
			this.btnChoosePages.Click += new System.EventHandler(this.btnChoosePages_Click);
			// 
			// btnResetPages
			// 
			this.btnResetPages.Location = new System.Drawing.Point(96, 8);
			this.btnResetPages.Name = "btnResetPages";
			this.btnResetPages.Size = new System.Drawing.Size(96, 23);
			this.btnResetPages.TabIndex = 7;
			this.btnResetPages.Text = "[איפוס עמודים]";
			this.btnResetPages.Click += new System.EventHandler(this.btnResetPages_Click);
			// 
			// lbCopies
			// 
			this.lbCopies.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lbCopies.BackColor = System.Drawing.Color.White;
			this.lbCopies.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lbCopies.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbCopies.ForeColor = System.Drawing.Color.Red;
			this.lbCopies.Location = new System.Drawing.Point(224, 472);
			this.lbCopies.Name = "lbCopies";
			this.lbCopies.Size = new System.Drawing.Size(152, 24);
			this.lbCopies.TabIndex = 8;
			this.lbCopies.Text = "יותר מעותק אחד";
			this.lbCopies.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lbCopies.Visible = false;
			// 
			// PrintForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.ClientSize = new System.Drawing.Size(632, 502);
			this.Controls.Add(this.lbCopies);
			this.Controls.Add(this.btnResetPages);
			this.Controls.Add(this.btnChoosePages);
			this.Controls.Add(this.cbZoom);
			this.Controls.Add(this.labelZoom);
			this.Controls.Add(this.labelPage);
			this.Controls.Add(this.toolBar);
			this.Controls.Add(this.printPreview);
			this.Controls.Add(this.btnClose);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.Name = "PrintForm";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "הדפסה";
			this.ResumeLayout(false);

		}
		#endregion

		private void toolBar_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			if (e.Button == tbbFirst)
			{
				if (printPreview.PageCount > 0)
					SetPage(0);
			}
			else if (e.Button == tbbPrevious)
			{
				if (printPreview.Page > 0)
					SetPage(printPreview.Page - 1);
			}
			else if (e.Button == tbbNext)
			{
				if (printPreview.Page < printPreview.PageCount - 1)
					SetPage(printPreview.Page + 1);
			}
			else if (e.Button == tbbLast)
			{
				if (printPreview.PageCount > 0)
					SetPage(printPreview.PageCount - 1);
			}
			else if (e.Button == tbbPrint)
			{
				System.Drawing.Printing.PrintController last = printPreview.Document.PrintController;
				printPreview.Document.PrintController = new PrintControllerWithPageForm(last, 0);
				try
				{
					printPreview.Document.Print();
				}
				catch (Exception ex)
				{
					Sport.UI.MessageBox.Warn("שגיאה פנימית בעת הדפסה, וודא תקינות הגדרות מדפסת\n" + ex.Message, "הדפסה");
				}
				printPreview.Document.PrintController = last;
				this.DialogResult = DialogResult.OK;
			}
			else if (e.Button == tbbSave)
			{
				if (saveDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					Update();
					string strFilePath=saveDialog.FileName;
					WaitForm.ShowWait("יוצר קובץ, אנא המתן...");
					switch (saveDialog.FilterIndex)
					{
						case 1:
							Sport.Common.ProgressHelper helper = new Sport.Common.ProgressHelper();
							WaitForm.SetProgress(0);
							helper.ProgressChanged += new EventHandler(helper_ProgressChanged);
							try
							{
								//((Sport.Documents.Document) _document).SaveAsWordDocument(strFilePath, helper);
							}
							catch (Exception ex)
							{
								Sport.UI.MessageBox.Error("שגיאה בעת יצירת הקובץ: \n" + ex.Message + "\n" + ex.StackTrace, 
									"שמירת קובץ");
							}
							WaitForm.SetProgress(100);
							break;
						case 2:
							try
							{
								CreateExcelFile(strFilePath);
							}
							catch (Exception ex)
							{
								Sport.UI.MessageBox.Error("שגיאה בעת יצירת הקובץ: \n" + ex.Message + "\n" + ex.StackTrace, 
									"שמירת קובץ");
							}
							break;
					}
					WaitForm.HideWait();
				}
			}
		}

		void helper_ProgressChanged(object sender, EventArgs e)
		{
			Sport.Common.ProgressHelper helper = (Sport.Common.ProgressHelper)sender;
			Dialogs.WaitForm.SetProgress(helper.CurrentProgress);
			helper.IsCancelled = Dialogs.WaitForm.Cancelled;
		}
		
		private int _tableStartPage=-1;
		private void CreateExcelFile(string strFilePath)
		{
			System.IO.StreamWriter objWriter=
				new System.IO.StreamWriter(strFilePath, false, System.Text.Encoding.GetEncoding("ISO-8859-8"));
			_tableStartPage = _document.Pages.Count+1;
			ArrayList arrAllLines = new ArrayList();
			for (int i=0; i<_document.Pages.Count; i++)
			{
				Documents.Page page=_document.Pages[i];
				ArrayList arrLines = GetPageLines(page);
				foreach (Documents.PageItem[] items in arrLines)
				{
					if (CheckLineExists(items, ref arrAllLines))
						continue;
					System.Text.StringBuilder curLine = new System.Text.StringBuilder();
					for (int j=0; j<items.Length; j++)
					{
						string strCurText = null;
						try
						{
							strCurText = GetItemText(items[j], i);
						}
						catch
						{
						}
						if ((strCurText != null)&&(strCurText.Length > 0))
							curLine.Append(strCurText);
						if (j < (items.Length-1))
							curLine.Append("\t");
					}
					objWriter.WriteLine(curLine.ToString());
				}
			}
			objWriter.Close();
		}
		
		private bool CheckLineExists(Documents.PageItem[] items, ref ArrayList lines)
		{
			string[] arrText = new string[items.Length];
			for (int i=0; i<items.Length; i++)
				if (items[i] is Documents.TextItem)
					arrText[i] = (items[i] as Documents.TextItem).Text;
			foreach (string[] cells in lines)
				if (Common.Tools.SameArrays(arrText, cells))
					return true;
			if (!Common.Tools.IsArrayEmpty(arrText))
				lines.Add(arrText);
			return false;
		}
		
		private ArrayList GetPageLines(Documents.Page page)
		{
			Hashtable lines = new Hashtable();
			ArrayList arrLeftPositions = new ArrayList();
			ArrayList arrTopPositions = new ArrayList();
			
			foreach (Documents.PageItem item in page.Items)
			{
				if ((item is Documents.TextItem) || (item is Documents.TableItem))
				{
					int curTop = item.Bounds.Top;
					int curLeft = item.Bounds.Left;
					if (lines[curTop] == null)
					{
						lines[curTop] = new ArrayList();
						arrTopPositions.Add(curTop);
					}
					(lines[curTop] as ArrayList).Add(item);
					if (arrLeftPositions.IndexOf(curLeft) < 0)
						arrLeftPositions.Add(curLeft);
				}
			}
			
			foreach (int key in arrTopPositions)
			{
				ArrayList arrItems = (ArrayList) lines[key];
				if (arrItems.Count == 0)
					continue;
				Rectangle curBounds = (arrItems[0] as Documents.PageItem).Bounds;
				ArrayList arrLineLeftPositions = new ArrayList();
				foreach (Documents.PageItem item2 in arrItems)
					arrLineLeftPositions.Add(item2.Bounds.Left);
				foreach (int left in arrLeftPositions)
					if (arrLineLeftPositions.IndexOf(left) < 0)
						arrItems.Add(new Documents.TextItem(new Rectangle(left, curBounds.Top, 10, curBounds.Height), " "));
				lines[key] = arrItems;
			}
			
			ArrayList arrKeys = new ArrayList(lines.Keys);
			arrKeys.Sort();
			ArrayList result = new ArrayList();
			foreach (int top in arrKeys)
			{
				ArrayList arrItems = (ArrayList) lines[top];
				arrItems.Sort(new PageItemsComparer());
				result.Add(arrItems.ToArray(typeof(Documents.PageItem)));
			}
			return result;
		}
		
		private string GetItemText(Documents.PageItem item, int pageIndex)
		{
			if ((item is Documents.TextItem)&&(!(item is Documents.FieldTextItem)))
				return ((Documents.TextItem) item).Text;
			if (item is Documents.TableItem)
			{
				if (pageIndex > _tableStartPage)
					return "";
				_tableStartPage = pageIndex;
				ArrayList arrTables=new ArrayList();
				for (int i=pageIndex; i<_document.Pages.Count; i++)
				{
					Documents.Page page=_document.Pages[i];
					Documents.TableItem curTable=null;
					foreach (Documents.PageItem curItem in page.Items)
					{
						if (curItem is Documents.TableItem)
						{
							curTable = (Documents.TableItem) curItem;
							break;
						}
					}
					if (curTable == null)
						break;
					arrTables.Add(curTable);
				}
				System.Text.StringBuilder sb=new System.Text.StringBuilder();
				for (int tableIndex=0; tableIndex<arrTables.Count; tableIndex++)
				{
					Documents.TableItem table=(Documents.TableItem) arrTables[tableIndex];
					for (int rowIndex=0; rowIndex<table.Rows.Count; rowIndex++)
					{
						if ((rowIndex == 0)&&(tableIndex > 0))
						{
							sb.Append("\n");
							continue;
						}
						Documents.TableItem.TableRow row=table.Rows[rowIndex];
						for (int colIndex=0; colIndex<table.Columns.Count; colIndex++)
						{
							object cell=row.Values[colIndex];
							string strCurText=(cell == null)?"":cell.ToString();
							sb.Append(strCurText);
							if (colIndex < (table.Columns.Count-1))
								sb.Append("\t");
						}
						if (rowIndex < (table.Rows.Count-1))
							sb.Append("\n");
					}
				}
				return sb.ToString();
			}
			return null;
		}
		
		private void SetZoom()
		{
			if (cbZoom.Text == "התאם לרוחב")
			{
				printPreview.FitWidth();
			}
			else
			{
				try
				{
					string sz = cbZoom.Text.Replace("%", "");
					int z = Int32.Parse(sz);
					printPreview.Zoom = (float) z / 100;
				}
				catch
				{
					return ;
				}
			}
			string zoom = ((int) (printPreview.Zoom * 100)).ToString() + "%";
			if (!cbZoom.Items.Contains(zoom))
				cbZoom.Items.Add(zoom);
			cbZoom.Text = zoom;
		}

		private void cbZoom_SelectedValueChanged(object sender, System.EventArgs e)
		{
			SetZoom();
		}
		
		private void ResetDocument()
		{
			if (_document != null)
			{
				System.Drawing.Printing.PrintDocument document=_document.CreatePrintDocument(_settings);
				printPreview.Document = document;
			}
			if (printPreview.PageCount > 0)
			{
				SetPage(0);
			}
			else
			{
				SetPage(-1);
			}
			printPreview.FitWidth();
		}
		
		private void btnChoosePages_Click(object sender, System.EventArgs e)
		{
			ChoosePrintPagesForm objForm=new ChoosePrintPagesForm(_pagesToPrint, _originalCount);
			if (objForm.ShowDialog(this) == DialogResult.OK)
			{
				_document.SetPages(objForm.PagesToPrint);
				ResetDocument();
			}
		}

		private void btnResetPages_Click(object sender, System.EventArgs e)
		{
			_document.LoadStoredPages();
			ResetDocument();
		}
		
		private class PageItemsComparer : IComparer
		{
			public int Compare(object x, object y)
			{
				Documents.PageItem p1 = (Documents.PageItem) x;
				Documents.PageItem p2 = (Documents.PageItem) y;
				return p1.Bounds.Left.CompareTo(p2.Bounds.Left);
			}
		}
	}
}
