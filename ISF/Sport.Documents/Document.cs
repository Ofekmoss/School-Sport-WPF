using System;
//using Word=Microsoft.Office.Interop.Word;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using Mir.Common;

namespace Sport.Documents
{

	public class TestDocument : Document
	{
		private Sport.Documents.Page page1;
		private Sport.Documents.Page page2;
		private Sport.Documents.Page page3;
		private Sport.Documents.Page page4;
		private Sport.Documents.Page page5;
		private Sport.Documents.TextItem textItem1;
		private Sport.Documents.ImageItem image1;
		private Sport.Documents.TableItem tableItem1;
	
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(TestDocument));
			this.textItem1 = new Sport.Documents.TextItem();
			this.image1 = new Sport.Documents.ImageItem();
			this.tableItem1 = new Sport.Documents.TableItem();
			this.page1 = new Sport.Documents.Page();
			this.page2 = new Sport.Documents.Page();
			this.page3 = new Sport.Documents.Page();
			this.page4 = new Sport.Documents.Page();
			this.page5 = new Sport.Documents.Page();
			// 
			// textItem1
			// 
			this.textItem1.Alignment = Sport.Documents.TextAlignment.Near;
			this.textItem1.BackColor = System.Drawing.Color.Transparent;
			this.textItem1.Border = null;
			this.textItem1.Bounds = new System.Drawing.Rectangle(0, 0, 50, 20);
			this.textItem1.Direction = Sport.Documents.Direction.Left;
			this.textItem1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
			this.textItem1.ForeColor = System.Drawing.Color.Transparent;
			this.textItem1.LineAlignment = Sport.Documents.TextAlignment.Near;
			this.textItem1.Text = "Text";
			// 
			// image1
			// 
			this.image1.BackColor = System.Drawing.Color.Transparent;
			this.image1.Border = null;
			this.image1.Bounds = new System.Drawing.Rectangle(20, 20, 100, 100);
			this.image1.Direction = Sport.Documents.Direction.Left;
			this.image1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
			this.image1.ForeColor = System.Drawing.Color.Transparent;
			this.image1.Image = ((System.Drawing.Image)(resources.GetObject("image1.Image")));
			this.image1.ImagePosition = Sport.Documents.ImagePosition.Stretch;
			// 
			// tableItem1
			// 
			this.tableItem1.Alignment = Sport.Documents.TextAlignment.Near;
			this.tableItem1.BackColor = System.Drawing.Color.Transparent;
			this.tableItem1.Bounds = new System.Drawing.Rectangle(10, 200, 200, 200);
			this.tableItem1.Direction = Sport.Documents.Direction.Left;
			this.tableItem1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
			this.tableItem1.ForeColor = System.Drawing.Color.Transparent;
			this.tableItem1.LineAlignment = Sport.Documents.TextAlignment.Near;
			// 
			// page1
			// 
			this.page1.BackColor = System.Drawing.Color.Transparent;
			this.page1.Border = null;
			this.page1.Direction = Sport.Documents.Direction.Left;
			this.page1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
			this.page1.ForeColor = System.Drawing.Color.Transparent;
			this.page1.Items.Add(this.image1);
			this.page1.Items.Add(this.tableItem1);
			this.page1.Size = new System.Drawing.Size(967, 1169);
			// 
			// page2
			// 
			this.page2.BackColor = System.Drawing.Color.Transparent;
			this.page2.Border = null;
			this.page2.Direction = Sport.Documents.Direction.Left;
			this.page2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
			this.page2.ForeColor = System.Drawing.Color.Transparent;
			this.page2.Size = new System.Drawing.Size(967, 1169);
			// 
			// page3
			// 
			this.page3.BackColor = System.Drawing.Color.Transparent;
			this.page3.Border = null;
			this.page3.Direction = Sport.Documents.Direction.Left;
			this.page3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
			this.page3.ForeColor = System.Drawing.Color.Transparent;
			this.page3.Size = new System.Drawing.Size(967, 1169);
			// 
			// page4
			// 
			this.page4.BackColor = System.Drawing.Color.Transparent;
			this.page4.Border = null;
			this.page4.Direction = Sport.Documents.Direction.Left;
			this.page4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
			this.page4.ForeColor = System.Drawing.Color.Transparent;
			this.page4.Size = new System.Drawing.Size(967, 1169);
			// 
			// page5
			// 
			this.page5.BackColor = System.Drawing.Color.Transparent;
			this.page5.Border = null;
			this.page5.Direction = Sport.Documents.Direction.Left;
			this.page5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
			this.page5.ForeColor = System.Drawing.Color.Transparent;
			this.page5.Size = new System.Drawing.Size(967, 1169);
			// 
			// TestDocument
			// 
			this.Pages.Add(this.page1);
			this.Pages.Add(this.page2);
			this.Pages.Add(this.page3);
			this.Pages.Add(this.page4);
			this.Pages.Add(this.page5);

		}

		public TestDocument()
		{
			InitializeComponent();
		}
	}

	public enum Direction
	{
		Inherit,
		Right,
		Left
	}

	#region DocumentException Class

	public class DocumentException : Exception
	{
		public DocumentException()
		{
		}

		public DocumentException(string message)
			: base(message)
		{
		}

		public DocumentException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}

	#endregion

	[System.ComponentModel.Designer(typeof(DocumentDesigner), typeof(System.ComponentModel.Design.IRootDesigner))]
	public class Document : DocumentItem
	{
		public Document()
			: this(null, null)
		{
		}

		public Document(string title)
			: this(title, null)
		{
		}

		public Document(string title, string author)
		{
			Font = new System.Drawing.Font("Tahoma", 12);
			ForeBrush = System.Drawing.SystemBrushes.WindowText;
			_defaultPageSize = new System.Drawing.Size(967, 1169);
			_defaultMargins = new System.Drawing.Rectangle(30, 30, 907, 1109);
			_title = title;
			_author = author;
		}

		#region Cloning

		public Document(Document document)
			: base(document)
		{
			Font = document.Font;
			ForeBrush = document.ForeBrush;
			_defaultPageSize = document.DefaultPageSize;
			_title = document.Title;
			_author = document.Author;
		}
		
		public override DocumentItem Clone()
		{
			return new Document(this);
		}

		#endregion
		
		private System.Collections.ArrayList _storedPages=null;
		public void StoreCurrentPages()
		{
			if (_storedPages == null)
			{
				_storedPages = new System.Collections.ArrayList();
			}
			_storedPages.Clear();
			_storedPages.AddRange(this.Pages);
		}
		public void LoadStoredPages()
		{
			this.Pages.Clear();
			foreach (Page page in _storedPages)
				this.Pages.Add(page);
		}
		
		public void SetPages(int[] pageIndices)
		{
			if ((pageIndices == null)||(pageIndices.Length == 0))
				return;
			System.Collections.ArrayList arrToPreserve=new System.Collections.ArrayList();
			for (int i=0; i<pageIndices.Length; i++)
				arrToPreserve.Add(this.Pages[pageIndices[i]]);
			this.Pages.Clear();
			foreach (Page page in arrToPreserve)
				this.Pages.Add(page);
		}
		
		protected override DocumentItemCollection CreateItemCollection()
		{
			return new PageCollection(this);
		}

		[System.ComponentModel.Browsable(false), System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Content)]
		public PageCollection Pages
		{
			get { return (PageCollection) Items; }
		}

		private System.Drawing.Size _defaultPageSize;
		public System.Drawing.Size DefaultPageSize
		{
			get { return _defaultPageSize; }
			set { _defaultPageSize = value; }
		}

		private System.Drawing.Rectangle _defaultMargins;
		public System.Drawing.Rectangle DefaultMargins
		{
			get { return _defaultMargins; }
			set { _defaultMargins = value; }
		}

		private string _title;
		public string Title
		{
			get { return _title; }
			set { _title = value; }
		}

		private string _author;
		public string Author
		{
			get { return _author; }
			set { _author = value; }
		}

		public int GetPageNumber(Page page)
		{
			return Pages.IndexOf(page) + 1;
		}

		public int GetSectionPageNumber(Page page)
		{
			int index = Pages.IndexOf(page);
			if (index < 0)
				return 0;

			int sectionPage = 1;
			while (index > 0 && !Pages[index].NewSection)
			{
				index--;
				sectionPage++;
			}

			return sectionPage;
		}

		public int GetPageSection(Page page)
		{
			int index = Pages.IndexOf(page);
			if (index < 0)
				return -1;

			int section = 1;

			for (int n = 1; n <= index; n++)
			{
				if (Pages[n].NewSection)
					section++;
			}

			return section;
		}

		public int GetSectionFirstPage(int section)
		{
			if (Pages.Count == 0)
				return -1;

			int page = 1;
			while (section > 1 && page < Pages.Count)
			{
				if (Pages[page].NewSection)
					section--;
				page++;
			}

			if (section > 1) // Section too large
				return -1; // no section

			return page - 1;
		}

		public int GetSectionPageCount(int section)
		{
			int firstPage = GetSectionFirstPage(section);

			if (firstPage < 0)
				return -1;

			int page = firstPage + 1;
			
			while (page < Pages.Count && Pages[page] != null && !Pages[page].NewSection)
				page++;

			return page - firstPage;
		}

		#region PageCollection Class

		public class PageCollection : DocumentItemCollection
		{
			public PageCollection(Document document)
				: base(document)
			{
			}

			protected override void SetItem(int index, DocumentItem value)
			{
				if (value != null && !(value is Page))
				{
					throw new DocumentException("Child item in document must inherit Page");
				}

				base.SetItem (index, value);
			}

			public Document Document
			{
				get { return ((Document) DocumentItem); }
			}

			public new Page this[int index]
			{
				get { return (Page) GetItem(index); }
				set { SetItem(index, value); }
			}

			public void Insert(int index, Page value)
			{
				InsertItem(index, value);
			}

			public void Remove(Page value)
			{
				RemoveItem(value);
			}

			public bool Contains(Page value)
			{
				return base.Contains(value);
			}

			public int IndexOf(Page value)
			{
				return base.IndexOf(value);
			}

			public int Add(Page value)
			{
				return AddItem(value);
			}
		}

		#endregion

		#region Printing

		public void SetSettings(System.Drawing.Printing.PrinterSettings settings)
		{
			int width = settings.DefaultPageSettings.PaperSize.Width -
				settings.DefaultPageSettings.Margins.Left -
				settings.DefaultPageSettings.Margins.Right;
			int height = settings.DefaultPageSettings.PaperSize.Height -
				settings.DefaultPageSettings.Margins.Top -
				settings.DefaultPageSettings.Margins.Bottom;
			
			if (settings.DefaultPageSettings.Landscape)
			{
				DefaultMargins = new System.Drawing.Rectangle(
					settings.DefaultPageSettings.Margins.Top,
					settings.DefaultPageSettings.Margins.Left, height, width); 
				DefaultPageSize = new System.Drawing.Size(
					settings.DefaultPageSettings.PaperSize.Height,
					settings.DefaultPageSettings.PaperSize.Width);
			}
			else
			{
				DefaultMargins = new System.Drawing.Rectangle(
					settings.DefaultPageSettings.Margins.Left,
					settings.DefaultPageSettings.Margins.Top, width, height);
				DefaultPageSize = new System.Drawing.Size(
					settings.DefaultPageSettings.PaperSize.Width,
					settings.DefaultPageSettings.PaperSize.Height);
			}
		}

		public System.Drawing.Printing.PrintDocument CreatePrintDocument(System.Drawing.Printing.PrinterSettings settings)
		{
			System.Drawing.Printing.PrintDocument printDocument = new System.Drawing.Printing.PrintDocument();
			printDocument.PrinterSettings = settings;
			printDocument.DefaultPageSettings = settings.DefaultPageSettings;
			printDocument.PrintController = new System.Drawing.Printing.StandardPrintController();
			printDocument.BeginPrint += new System.Drawing.Printing.PrintEventHandler(PrintDocumentBeginPrint);
			printDocument.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(PrintDocumentPrintPage);
			printDocument.EndPrint += new System.Drawing.Printing.PrintEventHandler(PrintDocumentEndPrint);
			return printDocument;
		}

        private int printDocumentPage = 0;

		private void PrintDocumentBeginPrint(object sender, System.Drawing.Printing.PrintEventArgs e)
		{
			printDocumentPage = 0;
		}

		private void PrintDocumentPrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
		{
			if (this.Pages == null)
			{
				e.HasMorePages = false;
				return;
			}
			
			Page page=Pages[printDocumentPage];
			if (page == null)
			{
				e.HasMorePages = false;
				return;
			}
			
			if (this.Border != null)
			{
				System.Drawing.Rectangle rect=
					new System.Drawing.Rectangle(0, 0, 
					e.PageBounds.Width-70, e.PageBounds.Height-70);
				page.PageLeft = e.PageBounds.Left+10;
				page.PageTop = e.PageBounds.Top+10;
				page.Bounds = rect;
				page.Border = this.Border;
				page.Borders = Borders.All;
			}
			page.Paint(e.Graphics);
			e.HasMorePages = printDocumentPage < Pages.Count - 1;
			printDocumentPage++;
		}

		private void PrintDocumentEndPrint(object sender, System.Drawing.Printing.PrintEventArgs e)
		{
		}
		#endregion
		 
		#region Word Document
		/*
		public void SaveAsWordDocument(string fileName, Sport.Common.ProgressHelper helper)
		{
			if (helper.IsCancelled)
				return;

			Word.ApplicationClass ac = new Microsoft.Office.Interop.Word.ApplicationClass();
			ac.Visible = false;
			object missing = Type.Missing;
			bool success = false;
			try
			{
				// Creating document
				object template = "normal.dot";
				object newTemplate = false;
				object documentType = 0;
				object visible = true;

				Word.Document document = null;
				try
				{
					document = ac.Documents.Add(ref template, ref newTemplate, ref documentType, ref visible);
				}
				catch
				{
					template = "normal.dotm";
					try
					{
						document = ac.Documents.Add(ref template, ref newTemplate, ref documentType, ref visible);
					}
					catch
					{
					}
				}

				if (document == null)
				{
					System.Windows.Forms.MessageBox.Show("גרסת אופיס לא מוכרת, לא ניתן לשמור", "שמירת קובץ", System.Windows.Forms.MessageBoxButtons.OK,
						System.Windows.Forms.MessageBoxIcon.Error);
					object saveChanges = false;
					ac.Quit(ref saveChanges, ref missing, ref missing);
					return;
				}
				object breakType = Word.WdBreakType.wdPageBreak;

				Stopwatch watch = new Stopwatch();
				watch.Start();
				List<string> times = new List<string>();

				int totalTableItems = 0;
				for (int page = 0; page < Pages.Count; page++)
				{
					foreach (PageItem pageItem in Pages[page].Items)
					{
						if (pageItem is TableItem)
							totalTableItems++;
					}
				}
				int tableItemProgress = (totalTableItems == 0) ? 0 : (int)(((double)90 / (double)totalTableItems) + 0.5);
				for (int page = 0; page < Pages.Count; page++)
				{
					document.PageSetup.PageHeight = Pages[page].Size.Height * 72 / 100;
					document.PageSetup.PageWidth = Pages[page].Size.Width * 72 / 100;

					foreach (PageItem pageItem in Pages[page].Items)
					{
						if (helper.IsCancelled)
						{
							object saveChanges = false;
							ac.Quit(ref saveChanges, ref missing, ref missing);
							return;
						}
						long prevElpased = watch.ElapsedMilliseconds;
						pageItem.Save(document);
						times.Add(pageItem.GetType().Name + " - " + (watch.ElapsedMilliseconds - prevElpased) + " ms");
						if (pageItem is TableItem)
							helper.AddProgress(tableItemProgress);
					}

					if (helper.IsCancelled)
					{
						object saveChanges = false;
						ac.Quit(ref saveChanges, ref missing, ref missing);
						return;
					}

					// Moving to next page
					if (page != Pages.Count - 1)
					{
						document.Paragraphs.Add(ref missing);
						document.Paragraphs.Add(ref missing).Range.InsertBreak(ref breakType);
						document.Application.Selection.GoToNext(Word.WdGoToItem.wdGoToPage).Select();
					}
				}
				long totalTime = watch.ElapsedMilliseconds;
				watch.Stop();

				//Logger.Instance.WriteLog(LogType.Debug, "Document", "Total " + totalTime + " milliseconds (" + 
				//	string.Join(", ", times) + ")");

				object ofilename = fileName;
				try
				{
					document.SaveAs(ref ofilename, ref missing, ref missing, ref missing, ref missing,
						ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing,
						ref missing, ref missing, ref missing, ref missing);

					object saveChanges = Word.WdSaveOptions.wdDoNotSaveChanges;
					document.Close(ref saveChanges, ref missing, ref missing);
					ac.Quit(ref missing, ref missing, ref missing);
					ac = null;
					success = true;
				}
				catch (Exception ex)
				{
					System.Windows.Forms.MessageBox.Show("שגיאה בעת שמירת הקובץ, אנא וודא שקובץ בעל שם זהה לא פתוח כבר על המחשב\n" +
						"במידה ולא פתוח אנא דווח על השגיאה להלן:\n" + ex.Message, "שמירת קובץ", System.Windows.Forms.MessageBoxButtons.OK,
						System.Windows.Forms.MessageBoxIcon.Error);
				}
			}
			catch
			{
				throw;
			}
			finally
			{
				if (ac != null)
				{
					object saveChanges = false;
					ac.Quit(ref saveChanges, ref missing, ref missing);
				}
			}

			if (success && File.Exists(fileName))
			{
				try
				{
					// combine the arguments together
					// it doesn't matter if there is a space after ','
					string argument = @"/select, " + fileName;
					System.Diagnostics.Process.Start("explorer.exe", argument);
				}
				catch
				{
				}
			}
		}
		*/
		#endregion
	}
}
