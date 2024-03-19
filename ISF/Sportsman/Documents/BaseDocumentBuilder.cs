using System;
using Sport.Documents;

namespace Sportsman.Documents
{
	public class BaseDocumentBuilder
	{
		System.Drawing.Printing.PrinterSettings _settings;
		public BaseDocumentBuilder(System.Drawing.Printing.PrinterSettings settings)
		{
			_settings = settings;
		}

		public Section CreateSection(DocumentBuilder documentBuilder, bool blnTopTitle)
		{
			Section section = new Section();

			System.Drawing.Image logo = (System.Drawing.Image)Sport.Resources.ImageLists.GetLogo();
			DateTime now = DateTime.Now;

			System.Drawing.Font fontTitle = new System.Drawing.Font("Tahoma",
				20, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);

			// Logo image
			ImageItem ii = new ImageItem(new System.Drawing.Rectangle(documentBuilder.DefaultMargins.Right - 200, 0, 200, 40), logo); //documentBuilder.DefaultMargins.Top - 40
			ii.ImagePosition = ImagePosition.Normal;
			section.Items.Add(ii);

			// Date
			TextItem ti;
			System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("he-IL");
			System.Globalization.DateTimeFormatInfo dtfi = ci.DateTimeFormat;
			ti = new TextItem(now.ToString("dd ·MMMM yyyy", dtfi));
			ti.Bounds = new System.Drawing.Rectangle(documentBuilder.DefaultMargins.Left, documentBuilder.DefaultMargins.Top - 20, 170, 20);
			ti.Alignment = TextAlignment.Far;
			section.Items.Add(ti);
			dtfi.Calendar = new System.Globalization.HebrewCalendar();
			ti = new TextItem(now.ToString("dd ·MMMM yyyy", dtfi));
			ti.Bounds = new System.Drawing.Rectangle(documentBuilder.DefaultMargins.Left, documentBuilder.DefaultMargins.Top - 40, 170, 20);
			ti.Alignment = TextAlignment.Far;
			section.Items.Add(ti);

			// Title
			ti = new FieldTextItem("{" + ((int)TextField.Title).ToString() + "}");
			if (blnTopTitle)
			{
				ti.Font = fontTitle;
				ti.ForeColor = System.Drawing.Color.Blue;
				ti.Bounds = new System.Drawing.Rectangle(documentBuilder.DefaultMargins.Right - (documentBuilder.DefaultMargins.Width - 160), documentBuilder.DefaultMargins.Top + 25, documentBuilder.DefaultMargins.Width - 160, 25);
			}
			else
			{
				ti.Bounds = new System.Drawing.Rectangle(documentBuilder.DefaultMargins.Right - (documentBuilder.DefaultMargins.Width - 160), documentBuilder.DefaultMargins.Bottom + 40, documentBuilder.DefaultMargins.Width - 160, 25);
			}
			section.Items.Add(ti);

			// Page Number
			ti = new FieldTextItem("ÚÓÂ„ {" + ((int)TextField.Page).ToString() +
				"} Ó˙ÂÍ {" + ((int)TextField.PageCount).ToString() + "}");
			ti.Bounds = new System.Drawing.Rectangle(documentBuilder.DefaultMargins.Left, documentBuilder.DefaultMargins.Bottom + 20, 150, 20);
			ti.Alignment = TextAlignment.Far;
			section.Items.Add(ti);

			return section;
		}

		public Section CreateSection(DocumentBuilder documentBuilder)
		{
			return CreateSection(documentBuilder, false);
		}

		public DocumentBuilder CreateDocumentBuilder(string title)
		{
			DocumentBuilder documentBuilder = new DocumentBuilder(title);
			documentBuilder.Direction = Direction.Right;

			documentBuilder.Font = new System.Drawing.Font("Tahoma", 12, System.Drawing.GraphicsUnit.Pixel);

			documentBuilder.SetSettings(_settings);

			return documentBuilder;
		}

		public DocumentBuilder CreateDocumentBuilder(string title, PageItem pageItem,
			bool blnTopTitle)
		{
			DocumentBuilder documentBuilder = CreateDocumentBuilder(title);

			documentBuilder.Sections.Add(CreateSection(documentBuilder, blnTopTitle));

			int offset = (blnTopTitle) ? 50 : 40;
			pageItem.Bounds = new System.Drawing.Rectangle(documentBuilder.DefaultMargins.Left, documentBuilder.DefaultMargins.Top + offset, documentBuilder.DefaultMargins.Width, documentBuilder.DefaultMargins.Height - 10);
			documentBuilder.Sections[0].Items.Add(pageItem);

			return documentBuilder;
		}

		public DocumentBuilder CreateDocumentBuilder(string title, PageItem pageItem)
		{
			return CreateDocumentBuilder(title, pageItem, false);
		}

		public TableItem CreateTableItem(System.Drawing.Rectangle bounds, Sport.UI.TableDesign design)
		{
			TableItem tableItem = new TableItem();
			tableItem.RelativeColumnWidth = true;
			tableItem.Bounds = bounds;

			Sport.UI.IVisualizer visualizer = design.Visualizer;

			TableItem.TableColumn tc;

			TableItem.TableCell[] header = new TableItem.TableCell[design.Columns.Count];
			for (int n = 0; n < design.Columns.Count; n++)
			{
				tc = new TableItem.TableColumn();
				tc.Width = design.Columns[n].Width;
				tc.Alignment = (TextAlignment)design.Columns[n].Alignment;
				tableItem.Columns.Add(tc);

				header[n] = new TableItem.TableCell(design.Columns[n].Title);
				header[n].Border = System.Drawing.SystemPens.WindowFrame;
			}

			//tableItem.Border = System.Drawing.SystemPens.WindowFrame;

			TableItem.TableRow headerRow = new TableItem.TableRow(header);
			headerRow.BackColor = System.Drawing.Color.SkyBlue;
			headerRow.Font = new System.Drawing.Font("Tahoma", 12, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			headerRow.Alignment = Sport.Documents.TextAlignment.Center;
			headerRow.LineAlignment = Sport.Documents.TextAlignment.Center;

			tableItem.Rows.Add(headerRow);


			return tableItem;
		}

		public TableItem CreateTableItem(System.Drawing.Rectangle bounds, Sport.UI.TableDesign design,
			object[] objects)
		{
			TableItem tableItem = CreateTableItem(bounds, design);

			foreach (object o in objects)
			{
				string[] record = new string[design.Columns.Count];
				for (int n = 0; n < design.Columns.Count; n++)
				{
					record[n] = design.Visualizer.GetText(o, design.Columns[n].Field);
				}

				tableItem.Rows.Add(record);
			}

			return tableItem;
		}

		public TableItem CreateTableItem(System.Drawing.Rectangle bounds, Sport.UI.TableDesign design,
			object[] objects, ref int index)
		{
			TableItem tableItem = CreateTableItem(bounds, design);

			return tableItem;
		}

		private static readonly int _labelHeight = 20;
		public static int GetLabelHeight()
		{
			return _labelHeight;
		}

		public static System.Drawing.Pen textItemBorderPen = null;
		public static TextItem BuildTextItem(string text, ref int x, ref int y,
			int width, System.Drawing.Font font, int hDiff, int vDiff, bool newLine,
			TextAlignment alignment, object borders)
		{
			TextItem result = null;
			if (text == null)
				text = "";
			if ((text.IndexOf("{") >= 0) && (text.IndexOf("}") > 0))
				result = new FieldTextItem();
			else
				result = new TextItem();
			result.Text = text;
			result.Font = font;
			result.ForeColor = System.Drawing.Color.Black;
			//try fix bug
			//width -= 5;
			result.Bounds = new System.Drawing.Rectangle(x, y, width, _labelHeight);
			result.Alignment = alignment;
			if (borders != null)
			{
				if (textItemBorderPen == null)
				{
					textItemBorderPen = new System.Drawing.Pen(System.Drawing.Color.Black, 1f);
				}
				result.Border = textItemBorderPen;
				result.Borders = (Sport.Documents.Borders)borders;
			}
			x += width;
			if (newLine)
			{
				x = 0;
				y += _labelHeight + vDiff;
			}
			x += hDiff;
			return result;
		}

		public static TextItem BuildTextItem(string text, ref int x, ref int y,
			int width, System.Drawing.Font font, int hDiff, int vDiff, bool newLine,
			TextAlignment alignment)
		{
			return BuildTextItem(text, ref x, ref y, width, font, hDiff, vDiff, newLine, alignment, null);
		}

		public static TextItem BuildTextItem(string text, ref int x, ref int y,
			int width, System.Drawing.Font font, int hDiff, int vDiff, bool newLine)
		{
			return BuildTextItem(text, ref x, ref y, width, font, hDiff,
				vDiff, newLine, TextAlignment.Near);
		}

		public static TextItem BuildTextItem(string text, ref int x, ref int y,
			int width, System.Drawing.Font font, int hDiff, bool newLine)
		{
			return BuildTextItem(text, ref x, ref y, width, font, hDiff, 0, newLine);
		}

		public static TableItem.TableRow BuildHeaderRow(TableItem table,
			Data.Table dataTable, int tableWidth, System.Drawing.Font font)
		{
			//initialize cells:
			TableItem.TableCell[] cells =
				new TableItem.TableCell[dataTable.Headers.Length];

			//iterate over headers:
			for (int i = 0; i < dataTable.Headers.Length; i++)
			{
				//create current cell:
				cells[i] = new TableItem.TableCell(dataTable.Headers[i].Text);

				//border?
				cells[i].Border = (dataTable.Headers[i].ShowBorder) ?
					System.Drawing.SystemPens.WindowFrame : null;

				//create column item:
				TableItem.TableColumn column = new TableItem.TableColumn();

				//assign width and alignment:
				int width = 0;
				if (dataTable.Headers[i].IsRelativeWidth)
					width = (int)(tableWidth * dataTable.Headers[i].RelativeWidth);
				else
					width = dataTable.Headers[i].AbsoluteWidth;
				column.Width = width;
				column.Alignment = TextAlignment.Near;

				//add to columns:
				table.Columns.Add(column);
			} //end loop over headers

			//build row item with the headers cells:
			TableItem.TableRow row = new TableItem.TableRow(cells);

			//assign row properties:
			row.Border = null;
			row.Font = font;
			row.Alignment = TextAlignment.Near;
			row.LineAlignment = TextAlignment.Near;

			//done.
			return row;
		}

		public static TableItem.TableRow BuildTableRow(TableItem table,
			string[] captions, double[] cellsWidth, int tableWidth,
			bool blnAddColumn, System.Drawing.Font font)
		{
			TableItem.TableCell[] header =
				new TableItem.TableCell[captions.Length];
			for (int i = 0; i < captions.Length; i++)
			{
				header[i] = new TableItem.TableCell(captions[i]);
				header[i].Border = System.Drawing.SystemPens.WindowFrame;
				if (blnAddColumn)
				{
					TableItem.TableColumn column =
						new TableItem.TableColumn();
					column.Width = (int)(tableWidth * cellsWidth[i]);
					column.Alignment = Sport.Documents.TextAlignment.Near;
					table.Columns.Add(column);
				}
			}
			TableItem.TableRow row = new TableItem.TableRow(header);
			row.Font = font;
			row.Alignment = Sport.Documents.TextAlignment.Near;
			row.LineAlignment = Sport.Documents.TextAlignment.Near;
			return row;
		}

		public static TableItem.TableRow BuildTableRow(string[] captions,
			double[] cellsWidth, System.Drawing.Font font)
		{
			return BuildTableRow(null, captions, cellsWidth, 0, false, font);
		}
	}
}
