using System;
using System.Drawing;
using Sport.Common;

namespace Sport.Rtf
{
	public class Font
	{
		private string _name;
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		private int _size;
		public int Size
		{
			get { return _size; }
			set { _size = value; }
		}

		private bool _bold;
		public bool Bold
		{
			get { return _bold; }
			set { _bold = value; }
		}

		public Font(string name, int size, bool bold)
		{
			_name = name;
			_size = size;
			_bold = bold;
		}

		public Font(string name, int size)
		{
			_name = name;
			_size = size;
			_bold = false;
		}

		public void Write(System.IO.TextWriter writer, Page page)
		{
			if (_name != null)
				writer.Write("\\f" + page.Document.Fonts.GetFont(_name));

			if (_size != 0)
				writer.Write("\\fs" + (_size / 10).ToString());

			if (_bold)
				writer.Write("\\b");
		}
	}

	public class Style
	{
		private Color _backColor;
		public Color BackColor
		{
			get { return _backColor; }
			set { _backColor = value; }
		}

		private Color _foreColor;
		public Color ForeColor
		{
			get { return _foreColor; }
			set { _foreColor = value; }
		}

		private Color _frameColor;
		public Color FrameColor
		{
			get { return _frameColor; }
			set { _frameColor = value; }
		}

		private int _frameSize;
		public int FrameSize
		{
			get { return _frameSize; }
			set { _frameSize = value; }
		}

		private Font _font;
		public Font Font
		{
			get { return _font; }
			set { _font = value; }
		}

		private StringAlignment _alignment;
		public StringAlignment Alignment
		{
			get { return _alignment; }
			set { _alignment = value; }
		}

		private StringAlignment _lineAlignment;
		public StringAlignment LineAlignment
		{
			get { return _lineAlignment; }
			set { _lineAlignment = value; }
		}

		private bool _rightToLeft;
		public bool RightToLeft
		{
			get { return _rightToLeft; }
			set { _rightToLeft = value; }
		}

		public Style()
		{
			_backColor = Color.Transparent;
			_foreColor = Color.Black;
			_frameColor = Color.Transparent;
			_frameSize = 0;
			_font = null;
			_alignment = StringAlignment.Near;
			_lineAlignment = StringAlignment.Near;
			_rightToLeft = false;
		}

		public Style(Style style)
			: this()
		{
			_backColor = style.BackColor;
			_foreColor = style.ForeColor;
			_frameColor = style.FrameColor;
			_frameSize = style.FrameSize;
			_font = style.Font;
			_alignment = style.Alignment;
			_lineAlignment = style.LineAlignment;
			_rightToLeft = style.RightToLeft;
		}
	}

	public abstract class PageItem : GeneralCollection.CollectionItem
	{
		public Page Page
		{
			get { return ((Page) Owner); }
		}

		public PageItem()
		{
			_bounds = Rectangle.Empty;
			style = new Style();
		}

		private Rectangle _bounds;
		public Rectangle Bounds
		{
			get { return _bounds; }
			set { _bounds = value; }
		}

		private Style style;
		public Style Style
		{
			get { return style; }
			set 
			{ 
				if (value == null)
					style = new Style();
				else
					style = new Style(value);
			}
		}

		public Color BackColor
		{
			get { return style.BackColor; }
			set { style.BackColor = value; }
		}

		public Color ForeColor
		{
			get { return style.ForeColor; }
			set { style.ForeColor = value; }
		}

		public Color FrameColor
		{
			get { return style.FrameColor; }
			set { style.FrameColor = value; }
		}

		public int FrameSize
		{
			get { return style.FrameSize; }
			set { style.FrameSize = value; }
		}

		public Font Font
		{
			get { return style.Font; }
			set { style.Font = value; }
		}

		public StringAlignment Alignment
		{
			get { return style.Alignment; }
			set { style.Alignment = value; }
		}

		public StringAlignment LineAlignment
		{
			get { return style.LineAlignment; }
			set { style.LineAlignment = value; }
		}

		public bool RightToLeft
		{
			get { return style.RightToLeft; }
			set { style.RightToLeft = value; }
		}

		public virtual void Write(System.IO.TextWriter writer)
		{
			if (_bounds != Rectangle.Empty)
			{
				writer.Write("\\posx" + _bounds.Left.ToString() +
					"\\posy" + _bounds.Top.ToString() +
					"\\absw" + _bounds.Width.ToString() +
					"\\absh-" + _bounds.Height.ToString());
			}

			writer.Write("\\cf" + Page.Document.Colors.GetColor(style.ForeColor));

			if (style.BackColor != Color.Transparent)
			{
				writer.Write("\\cbpat" + Page.Document.Colors.GetColor(style.BackColor));
			}

			if (style.FrameSize != 0)
			{
				writer.Write("\\box\\brdrs\\brdrw" + style.FrameSize.ToString());

				if (style.FrameColor != Color.Transparent)
				{
					writer.Write("\\brdrcf" + Page.Document.Colors.GetColor(style.FrameColor));
				}
			}

			if (style.Font != null)
			{
				style.Font.Write(writer, Page);
			}

			switch (style.LineAlignment)
			{
				case (StringAlignment.Center):
					writer.Write("\\vertalc");
					break;
				case (StringAlignment.Far):
					writer.Write("\\vertalb");
					break;
				default:
					writer.Write("\\vertalt");
					break;
			}

			switch (style.Alignment)
			{
				case (StringAlignment.Center):
					writer.Write("\\qc");
					break;
				case (StringAlignment.Far):
					if (style.RightToLeft)
						writer.Write("\\ql");
					else
						writer.Write("\\qr");
					break;
				default:
					if (style.RightToLeft)
						writer.Write("\\qr");
					else
						writer.Write("\\ql");
					break;
			}

			if (style.RightToLeft)
				writer.Write("\\rltpar");
		}
	}

	#region PageItemCollection

	public class PageItemCollection : GeneralCollection
	{
		public PageItemCollection(Page page)
			: base(page)
		{
		}

		public Page Page
		{
			get { return ((Page) Owner); }
		}

		public PageItem this[int index]
		{
			get { return (PageItem) GetItem(index); }
			set { SetItem(index, value); }
		}

		public void Insert(int index, PageItem value)
		{
			InsertItem(index, value);
		}

		public void Remove(PageItem value)
		{
			RemoveItem(value);
		}

		public bool Contains(PageItem value)
		{
			return base.Contains(value);
		}

		public int IndexOf(PageItem value)
		{
			return base.IndexOf(value);
		}

		public int Add(PageItem value)
		{
			return AddItem(value);
		}
	}

	#endregion

	public class Paragraph : PageItem
	{
		public Paragraph(string text, Rectangle bounds)
			: this(text)
		{
			Bounds = bounds;
		}

		public Paragraph(string text)
		{
			_text = text;
		}

		public Paragraph()
			: this(null)
		{
		}

		private string _text;
		public string Text
		{
			get { return _text; }
			set { _text = value; }
		}

		public override void Write(System.IO.TextWriter writer)
		{
			writer.WriteLine();
			writer.Write("\\par\\pard");

			base.Write(writer);

			if (_text != null)
			{
				string str = _text;
				str.Replace("\n", "\\line");
				writer.Write(" " + str + " ");
			}
		}
	}

	public class Picture : PageItem
	{
		private System.Drawing.Image _image;
		public System.Drawing.Image Image
		{
			get { return _image; }
			set { _image = value; }
		}

		public override void Write(System.IO.TextWriter writer)
		{
			//base.Write (writer);
		}
	}

	public class Table : PageItem
	{
		#region TableColumn

		public class TableColumn
		{
			public TableColumn(string title, int width)
			{
				_title = title;
				_width = width;
				style = new Style();
			}

			private string _title;
			public string Title
			{
				get { return _title; }
				set { _title = value; }
			}

			private int _width;
			public int Width
			{
				get { return _width; }
				set { _width = value; }
			}

			private Style style;
			public Style Style
			{
				get { return style; }
			}

			public StringAlignment Alignment
			{
				get { return style.Alignment; }
				set { style.Alignment = value; }
			}

			public StringAlignment LineAlignment
			{
				get { return style.LineAlignment; }
				set { style.LineAlignment = value; }
			}
		}

		#endregion

		#region TableColumnCollection

		public class TableColumnCollection : GeneralCollection
		{
			public TableColumnCollection(Table owner)
				: base(owner)
			{
			}

			public Table Table
			{
				get { return ((Table) Owner); }
			}

			public TableColumn this[int index]
			{
				get { return (TableColumn) GetItem(index); }
				set { SetItem(index, value); }
			}

			public void Insert(int index, TableColumn value)
			{
				InsertItem(index, value);
			}

			public void Insert(int index, string title, int width)
			{
				InsertItem(index, new TableColumn(title, width));
			}

			public void Remove(TableColumn value)
			{
				RemoveItem(value);
			}

			public bool Contains(TableColumn value)
			{
				return base.Contains(value);
			}

			public int IndexOf(TableColumn value)
			{
				return base.IndexOf(value);
			}

			public int Add(TableColumn value)
			{
				return AddItem(value);
			}

			public int Add(string title, int width)
			{
				return AddItem(new TableColumn(title, width));
			}
		}

		#endregion

		#region TableRow

		public class TableRow
		{
			public TableRow(string[] values, int height)
			{
				_values = values;
				_height = height;
			}

			private string[] _values;
			public string[] Values
			{
				get { return _values; }
				set { _values = value; }
			}

			private int _height;
			public int Height
			{
				get { return _height; }
				set { _height = value; }
			}
		}

		#endregion

		#region TableRowCollection

		public class TableRowCollection : GeneralCollection
		{
			public TableRowCollection(Table owner)
				: base(owner)
			{
			}

			public Table Table
			{
				get { return ((Table) Owner); }
			}

			public TableRow this[int index]
			{
				get { return (TableRow) GetItem(index); }
				set { SetItem(index, value); }
			}

			public void Insert(int index, TableRow value)
			{
				InsertItem(index, value);
			}

			public void Insert(int index, string[] values, int height)
			{
				InsertItem(index, new TableRow(values, height));
			}

			public void Remove(TableRow value)
			{
				RemoveItem(value);
			}

			public bool Contains(TableRow value)
			{
				return base.Contains(value);
			}

			public int IndexOf(TableRow value)
			{
				return base.IndexOf(value);
			}

			public int Add(TableRow value)
			{
				return AddItem(value);
			}

			public int Add(string[] values, int height)
			{
				return AddItem(new TableRow(values, height));
			}
		}

		#endregion


		public Table()
		{
			_columns = new TableColumnCollection(this);
			_rows = new TableRowCollection(this);

			headerStyle = new Style();
			headerStyle.Alignment = StringAlignment.Center;
			headerStyle.LineAlignment = StringAlignment.Center;
		}

		private TableColumnCollection _columns;
		public TableColumnCollection Columns
		{
			get { return _columns; }
		}

		private TableRowCollection _rows;
		public TableRowCollection Rows
		{
			get { return _rows; }
		}

		private int headerSize;
		public int HeaderSize
		{
			get { return headerSize; }
			set { headerSize = value; }
		}

		private Style headerStyle;
		public Style HeaderStyle
		{
			get { return headerStyle; }
		}

		private void WriteBorder(System.IO.TextWriter writer, int size, Color color)
		{
			if (size > 0)
			{
				if (size > 75)
				{
					writer.Write("\\brdrth\\brdrw" + (size / 2).ToString());
				}
				else
				{
					writer.Write("\\brdrs\\brdrw" + size.ToString());
				}

				if (color != Color.Transparent)
				{
					writer.Write("\\brdrcf" + Page.Document.Colors.GetColor(color));
				}
			}
		}

		private void WriteHeader(System.IO.TextWriter writer, 
			int column, ref int cellx)
		{
			if (HeaderStyle.FrameSize > 0)
			{
				writer.Write("\\clbrdrt");
				WriteBorder(writer, HeaderStyle.FrameSize, HeaderStyle.FrameColor);
				writer.Write("\\clbrdrl");
				WriteBorder(writer, HeaderStyle.FrameSize, HeaderStyle.FrameColor);
				writer.Write("\\clbrdrb");
				WriteBorder(writer, HeaderStyle.FrameSize, HeaderStyle.FrameColor);
				writer.Write("\\clbrdrr");
				WriteBorder(writer, HeaderStyle.FrameSize, HeaderStyle.FrameColor);
			}
			else if (FrameSize > 0)
			{
				writer.Write("\\clbrdrt");
				WriteBorder(writer, FrameSize, FrameColor);

				if (column == 0)
				{
					writer.Write("\\clbrdrl");
					WriteBorder(writer, FrameSize, FrameColor);
				}

				if (column == _columns.Count - 1)
				{
					writer.Write("\\clbrdrr");
					WriteBorder(writer, FrameSize, FrameColor);
				}
			}

			if (HeaderStyle.BackColor != Color.Transparent)
			{
				writer.Write("\\clcbpat" + Page.Document.Colors.GetColor(HeaderStyle.BackColor));
			}
			else if (BackColor != Color.Transparent)
			{
				writer.Write("\\clcbpat" + Page.Document.Colors.GetColor(BackColor));
			}

			cellx += _columns[column].Width;
			writer.WriteLine("\\cellx" + cellx.ToString());

			writer.Write("{\\pard\\intbl \\cf" + Page.Document.Colors.GetColor(HeaderStyle.ForeColor));

			switch (HeaderStyle.LineAlignment)
			{
				case (StringAlignment.Center):
					writer.Write("\\clvertalc");
					break;
				case (StringAlignment.Far):
					writer.Write("\\clvertalb");
					break;
				default:
					writer.Write("\\clvertalt");
					break;
			}

			switch (HeaderStyle.Alignment)
			{
				case (StringAlignment.Center):
					writer.Write("\\qc");
					break;
				case (StringAlignment.Far):
					if (RightToLeft)
						writer.Write("\\ql");
					else
						writer.Write("\\qr");
					break;
				default:
					if (RightToLeft)
						writer.Write("\\qr");
					else
						writer.Write("\\ql");
					break;
			}

			if (HeaderStyle.RightToLeft)
				writer.Write("\\rltpar");

			if (HeaderStyle.Font != null)
			{
				HeaderStyle.Font.Write(writer, Page);
			}
			else if (Font != null)
			{
				Font.Write(writer, Page);
			}

			writer.Write(" " + _columns[column].Title + " \\cell}");
		}

		private void WriteCell(System.IO.TextWriter writer,
			int row, int column, ref int cellx)
		{
			if (column == 0 && FrameSize > 0)
			{
				writer.Write("\\clbrdrl");
				WriteBorder(writer, FrameSize, FrameColor);
			}
			else if (_columns[column].Style.FrameSize > 0)
			{
				writer.Write("\\clbrdrl");
				WriteBorder(writer, _columns[column].Style.FrameSize, _columns[column].Style.FrameColor);
			}

			if (column == _columns.Count - 1 && FrameSize > 0)
			{
				writer.Write("\\clbrdrr");
				WriteBorder(writer, FrameSize, FrameColor);
			}
			else if (_columns[column].Style.FrameSize > 0)
			{
				writer.Write("\\clbrdrr");
				WriteBorder(writer, _columns[column].Style.FrameSize, _columns[column].Style.FrameColor);
			}

			if (row == 0 && HeaderStyle.FrameSize > 0)
			{
				writer.Write("\\clbrdrt");
				WriteBorder(writer, HeaderStyle.FrameSize, HeaderStyle.FrameColor);
			}
			else if (_columns[column].Style.FrameSize > 0)
			{
				writer.Write("\\clbrdrt");
				WriteBorder(writer, _columns[column].Style.FrameSize, _columns[column].Style.FrameColor);
			}

			if (row == _rows.Count - 1 && FrameSize > 0)
			{
				writer.Write("\\clbrdrb");
				WriteBorder(writer, FrameSize, FrameColor);
			}
			else if (_columns[column].Style.FrameSize > 0)
			{
				writer.Write("\\clbrdrb");
				WriteBorder(writer, _columns[column].Style.FrameSize, _columns[column].Style.FrameColor);
			}

			if (_columns[column].Style.BackColor != Color.Transparent)
			{
				writer.Write("\\clcbpat" + Page.Document.Colors.GetColor(_columns[column].Style.BackColor));
			}
			else if (BackColor != Color.Transparent)
			{
				writer.Write("\\clcbpat" + Page.Document.Colors.GetColor(BackColor));
			}

			cellx += _columns[column].Width;
			writer.WriteLine("\\cellx" + cellx.ToString());

			writer.Write("{\\pard\\intbl \\cf" + Page.Document.Colors.GetColor(_columns[column].Style.ForeColor));

			switch (_columns[column].Style.LineAlignment)
			{
				case (StringAlignment.Center):
					writer.Write("\\clvertalc");
					break;
				case (StringAlignment.Far):
					writer.Write("\\clvertalb");
					break;
				default:
					writer.Write("\\clvertalt");
					break;
			}

			switch (_columns[column].Style.Alignment)
			{
				case (StringAlignment.Center):
					writer.Write("\\qc");
					break;
				case (StringAlignment.Far):
					writer.Write("\\qr");
					break;
				default:
					writer.Write("\\ql");
					break;
			}

			if (_columns[column].Style.RightToLeft)
				writer.Write("\\rltpar");

			if (_columns[column].Style.Font != null)
			{
				_columns[column].Style.Font.Write(writer, Page);
			}
			else if (Font != null)
			{
				Font.Write(writer, Page);
			}

			string text = null;
			if (_rows[row].Values.Length > column)
				text = _rows[row].Values[column];

			writer.Write(" " + text + " \\cell}");
		}

		public override void Write(System.IO.TextWriter writer)
		{
			writer.WriteLine("\\par\\pard\\par");

			string rowDefinition = "\\trowd ";
			if (Bounds != Rectangle.Empty)
			{
				rowDefinition += "\\tposx" + Bounds.Left.ToString() +
					"\\tposy" + Bounds.Top.ToString();
			}

			int cellx = 0;

			writer.Write(rowDefinition);

			if (headerSize > 0)
			{
				writer.WriteLine("\\trrh-" + headerSize.ToString());
			}

			for (int column = 0; column < _columns.Count; column++)
			{
				WriteHeader(writer, column, ref cellx);
			}

			writer.WriteLine("\\row");

			for (int row = 0; row < _rows.Count; row++)
			{
				writer.Write(rowDefinition);

				writer.WriteLine("\\trrh-" + _rows[row].Height.ToString());

				cellx = 0;

				for (int column = 0; column < _columns.Count; column++)
				{
					WriteCell(writer, row, column, ref cellx);
				}

				writer.WriteLine("\\row");
			}
		}
	}

	public class Page : GeneralCollection.CollectionItem
	{
		public Document Document
		{
			get { return ((Document) Owner); }
		}

		private PageItemCollection _items;
		public PageItemCollection Items
		{
			get { return _items; }
		}

		public Page(Size size)
		{
			_items = new PageItemCollection(this);
			_size = size;
		}

		public Page()
			: this(Size.Empty)
		{
		}

		private Size _size;
		public Size Size
		{
			get { return _size; }
			set { _size = value; }
		}

		public void Write(System.IO.TextWriter writer)
		{
			Size size = _size;
			if (size == Size.Empty)
				size = Document.DefaultPageSize;

			writer.Write("\\paperw{0}\\paperh{1}\\margl0\\margr0\\margt0\\margb0", new object[] { size.Width, size.Height });

			foreach (PageItem item in _items)
			{
				item.Write(writer);
			}
		}
	}

	#region PageCollection

	public class PageCollection : GeneralCollection
	{
		public PageCollection(Document document)
			: base(document)
		{
		}

		public Document Document
		{
			get { return ((Document) Owner); }
		}

		public Page this[int index]
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

	public class FontTable : GeneralCollection
	{
		public int GetFont(string font)
		{
			int index = base.IndexOf(font);
			if (index < 0)
				index = AddItem(font);

			return index + 1;
		}

		public string this[int index]
		{
			get { return (string) GetItem(index); }
		}
	}

	public class ColorTable : GeneralCollection
	{
		public int GetColor(Color color)
		{
			int index = base.IndexOf(color);
			if (index < 0)
				index = AddItem(color);

			return index + 1;
		}

		public Color this[int index]
		{
			get { return (Color) GetItem(index); }
		}
	}

	public class Document
	{
		public static readonly int PaperWidth = 12240;
		public static readonly int PaperHeight = 15840;

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

		private int _codePage;
		public int CodePage
		{
			get { return _codePage; }
			set { _codePage = value; }
		}
        
		private PageCollection _pages;
		public PageCollection Pages
		{
			get { return _pages; }
		}

		private FontTable _fonts;
		public FontTable Fonts
		{
			get { return _fonts; }
		}

		private ColorTable _colors;
		public ColorTable Colors
		{
			get { return _colors; }
		}

		public Document()
		{
			_pages = new PageCollection(this);
			_fonts = new FontTable();
			_colors = new ColorTable();
			_defaultPageSize = new Size(PaperWidth, PaperHeight);
			_codePage = 1255;
		}

		private Size _defaultPageSize;
		public Size DefaultPageSize
		{
			get { return _defaultPageSize; }
			set { _defaultPageSize = value; }
		}

		public void Write(System.IO.TextWriter writer)
		{
			_colors.Clear();

			writer.WriteLine("{\\rtf1 \\ansi\\ansicpg" + _codePage.ToString());

			writer.Write("{\\info");
			if (_title != null)
				writer.Write("{\\title " + _title + "}");
			if (_author != null)
				writer.Write("{\\author " + _author + "}");

			writer.WriteLine("}");

			System.IO.TextWriter body = new System.IO.StringWriter();

			for (int p = 0; p < _pages.Count; p++)
			{
				_pages[p].Write(body);

				if (p < _pages.Count - 1)
					body.WriteLine("{\\pagebb\\par}");
			}

			writer.Write("{\\fonttbl");
			for (int f = 0; f < _fonts.Count; f++)
			{
				writer.Write("{\\f" + (f + 1).ToString() + "\\fnil\\fcharset177\\fprq2 " + _fonts[f] + "}");
			}
			writer.WriteLine("}");

			writer.Write("{\\colortbl;");
			foreach (Color color in _colors)
			{
				writer.Write("\\red" + color.R.ToString() +
					"\\green" + color.G.ToString() +
					"\\blue" + color.B.ToString() + ";");
			}
			writer.WriteLine("}");

			writer.Write(body);

			writer.Write("}");

			writer.Flush();
		}
	}
}
