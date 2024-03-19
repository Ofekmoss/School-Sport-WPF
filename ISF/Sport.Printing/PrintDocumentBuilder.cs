using System;
using System.Drawing.Printing;
using System.Collections;
using Sport.Common;
using System.Drawing;

namespace Sport.Printing
{
	public enum PrintDirection
	{
		Inherit,
		Right,
		Left
	}
	
	#region TwipConverter
	public class TwipConverter
	{
		public static int ToTwips(int hundredth)
		{
			return (int) ((double) (hundredth * 1440) / 100);
		}

		public static Point ToTwips(Point point)
		{
			return new Point(ToTwips(point.X), ToTwips(point.Y));
		}

		public static Size ToTwips(Size size)
		{
			return new Size(ToTwips(size.Width), ToTwips(size.Height));
		}

		public static Rectangle ToTwips(Rectangle rectangle)
		{
			return new Rectangle(ToTwips(rectangle.Location), ToTwips(rectangle.Size));
		}

		public static int ToHundredth(int twips)
		{
			return (int) ((double) (twips * 100) / 1440);
		}

		public static Point ToHundredth(Point point)
		{
			return new Point(ToHundredth(point.X), ToHundredth(point.Y));
		}

		public static Size ToHundredth(Size size)
		{
			return new Size(ToHundredth(size.Width), ToHundredth(size.Height));
		}

		public static Rectangle ToHundredth(Rectangle rectangle)
		{
			return new Rectangle(ToHundredth(rectangle.Location), ToHundredth(rectangle.Size));
		}
	}
	#endregion

	#region Objects

	#region SectionObject Class

	public class SectionObject : GeneralCollection.CollectionItem
	{
		public SectionObject(Rectangle bounds)
		{
			_bounds = bounds;
			_direction = PrintDirection.Inherit;
			BackColor = Color.Transparent;
			_foreColor = SystemColors.WindowText;
			_foreBrush = SystemBrushes.WindowText;
		}

		public SectionObject()
			: this(Rectangle.Empty)
		{
		}

		public Section Section
		{
			get { return ((Section) Owner); }
		}

		protected Rectangle _bounds;
		public Rectangle Bounds
		{
			get { return _bounds; }
			set { _bounds = value; }
		}
		public Size Size
		{
			get { return _bounds.Size; }
			set { _bounds.Size = value; }
		}
		public Point Location
		{
			get { return _bounds.Location; }
			set { _bounds.Location = value; }
		}

		protected Font _font;
		public Font Font
		{
			get 
			{ 
				if (_font == null)
					return Section.Font;
				return _font; 
			}
			set 
			{ 
				_font = value; 
			}
		}

		protected PrintDirection _direction;
		public PrintDirection Direction
		{
			get 
			{ 
				if (_direction == PrintDirection.Inherit)
					return Section.Direction;
				return _direction;
			}
			set
			{
				_direction = value;
			}
		}

		protected Brush _backBrush;
		protected Color _backColor;
		public Color BackColor
		{
			get { return _backColor; }
			set 
			{ 
				_backColor = value; 
				if (_backColor == Color.Transparent)
					_backBrush = null;
				else
					_backBrush = new SolidBrush(_backColor);
			}
		}

		protected Brush _foreBrush;
		protected Color _foreColor;
		public Color ForeColor
		{
			get { return _foreColor; }
			set 
			{ 
				_foreColor = value;
				_foreBrush = new SolidBrush(_foreColor);
			}
		}

		protected Pen _border;
		public Pen Border
		{
			get { return _border; }
			set { _border = value; }
		}

		protected Sport.Rtf.Style GetRtfStyle()
		{
			Sport.Rtf.Style style = new Sport.Rtf.Style();
			
			style.BackColor = BackColor;
			style.ForeColor = ForeColor;

			if (_border != null)
			{
				style.FrameSize = TwipConverter.ToTwips((int) _border.Width);
				style.FrameColor = _border.Color;
			}

			Font font = Font;
			if (font != null)
			{
				style.Font = new Sport.Rtf.Font(font.FontFamily.Name, TwipConverter.ToTwips((int) font.Size), font.Bold);
			}

			style.RightToLeft = Direction == PrintDirection.Right;

			return style;
		}
		
		public virtual void Save(Sport.WordParser.WordParser wp)
		{
		}

		public virtual int Save(Sport.ExcelParser.ExcelParser ep,int currentRow)
		{
			return currentRow;
		}

		public virtual bool Save(Sport.Rtf.Page page)
		{
			return false;
		}

		public virtual bool Print(Graphics g)
		{
			if (_backBrush != null)
			{
				g.FillRectangle(_backBrush, _bounds);
			}
			if (_border != null)
			{
				g.DrawRectangle(_border, _bounds);
			}

			return false;
		}

		public virtual int GetPageCount(Graphics g)
		{
			return 1;
		}
	}

	#endregion

	#region TextObject Class

	public enum TextField
	{
		Page,
		PageCount,
		Section,
		SectionPage,
		SectionPageCount,
		Title,
		Author
	}

	public class TextObject : SectionObject
	{
		public TextObject(Rectangle bounds, string text)
			: base(bounds)
		{
			_text = text;
			stringFormat = new StringFormat();
		}

		public TextObject(string text)
			: this(new Rectangle(0, 0, 50, 20), text)
		{
		}

		public TextObject(Rectangle bounds)
			: this(bounds, null)
		{
		}

		public TextObject()
			: this(new Rectangle(0, 0, 50, 20), null)
		{
		}

		private string _text;
		public string Text
		{
			get { return _text; }
			set { _text = value; }
		}

		private StringFormat stringFormat;

		public StringAlignment Alignment
		{
			get { return stringFormat.Alignment; }
			set { stringFormat.Alignment = value; }
		}

		public StringAlignment LineAlignment
		{
			get { return stringFormat.LineAlignment; }
			set { stringFormat.LineAlignment = value; }
		}

		private string GetText()
		{
			object[] fields = new object[] {
											   Section.PrintDocumentBuilder.Page + 1,
											   Section.PrintDocumentBuilder.PageCount,
											   Section.PrintDocumentBuilder.Section + 1,
											   Section.PrintDocumentBuilder.SectionPage + 1,
											   Section.PrintDocumentBuilder.SectionPageCount,
											   Section.PrintDocumentBuilder.Title,
											   Section.PrintDocumentBuilder.Author
										   };
			return String.Format(_text, fields);
		}
		
		public override void Save(Sport.WordParser.WordParser wp)
		{
			if (GetText().IndexOf("עמוד ") < 0)
				wp.writeLine(GetText(),1);
		}

		public override int Save(Sport.ExcelParser.ExcelParser ep,int currentRow)
		{
			int currentColumn = 1;
			ep.writeToCell(currentRow,currentColumn + 6,GetText(),BackColor);
			currentRow++;
			return currentRow;
		}

		public override bool Save(Sport.Rtf.Page page)
		{
			Sport.Rtf.Paragraph paragraph = new Sport.Rtf.Paragraph(GetText(), 
				TwipConverter.ToTwips(_bounds));

			paragraph.Style = GetRtfStyle();

			paragraph.Alignment = Alignment;
			paragraph.LineAlignment = LineAlignment;

			page.Items.Add(paragraph);

			return false;
		}

		public override bool Print(Graphics g)
		{
			base.Print(g);

			if (Direction == PrintDirection.Right)
				stringFormat.FormatFlags = StringFormatFlags.DirectionRightToLeft;
			else
				stringFormat.FormatFlags = (StringFormatFlags) 0;

			string text = GetText();
			if (text != null)
			{
				g.DrawString(text, Font, _foreBrush, _bounds, stringFormat);
			}

			return false;
		}

	}

	#endregion

	#region ImageObject Class

	public class ImageObject : SectionObject
	{
		public ImageObject(Rectangle bounds, Image image)
			: base(bounds)
		{
			if (image != null)
			{
				_image = image;
				_tempFilePath = System.Windows.Forms.Application.StartupPath + "\\ptemp.bmp";
				image.Save(_tempFilePath,System.Drawing.Imaging.ImageFormat.Bmp);
			}
		}

		public ImageObject(Image image)
			: this(new Rectangle(0, 0, 50, 50), image)
		{
		}

		public ImageObject(Rectangle bounds)
			: this(bounds, null)
		{
		}

		private Image _image;
		public Image Image
		{
			get { return _image; }
			set { _image = value; }
		}
		private string _tempFilePath;
		
		public string tempFilePath
		{
			get { return _tempFilePath; }
			set { _tempFilePath = value; }
		}

		public override void Save(Sport.WordParser.WordParser wp)
		{
			wp.insertPicture(_tempFilePath,3);
		}

		public override int Save(Sport.ExcelParser.ExcelParser ep,int currentRow)
		{
			ep.insertPicture(0,0,_tempFilePath);
			return currentRow;
		}

		public override bool Print(Graphics g)
		{
			base.Print(g);

			if (_image != null)
				g.DrawImage(_image, _bounds);
			
			return false;
		}
	}

	#endregion

	#region PaintObject Class

	public delegate void PaintObjectHandler(Graphics g, Rectangle bounds);

	public class PaintObject : SectionObject
	{
		public PaintObject(Rectangle bounds, PaintObjectHandler paintHandler)
			: base(bounds)
		{
			_paintHandler = paintHandler;
		}

		private PaintObjectHandler _paintHandler;

		public PaintObject(PaintObjectHandler paintHandler)
			: this(new Rectangle(0, 0, 50, 50), paintHandler)
		{
		}

		public PaintObject(Rectangle bounds)
			: this(bounds, null)
		{
		}

		public override void Save(Sport.WordParser.WordParser wp)
		{
		}

		public override int Save(Sport.ExcelParser.ExcelParser ep,int currentRow)
		{
			return 0;
		}

		public override bool Print(Graphics g)
		{
			_paintHandler(g, _bounds);
		
			base.Print(g);

			return false;
		}
	}

	#endregion

	#region TableObject Class

	public class TableObject : SectionObject
	{
		public TableObject(Rectangle bounds)
			: base(bounds)
		{
			_columns = new TableColumnCollection(this);
			_rows = new TableRowCollection(this);

			HeaderBackColor = Color.Transparent;
			_headerForeColor = SystemColors.WindowText;
			_headerForeBrush = SystemBrushes.WindowText;
		}

		public TableObject()
			: this(new Rectangle(0, 0, 200, 200))
		{
		}
	
		#region TableColumn
		public class TableColumn
		{
			public TableColumn(string title, int width)
			{
				_title = title;
				_width = width;
				stringFormat = new StringFormat();
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

			private StringFormat stringFormat;
			public StringFormat StringFormat
			{
				get { return stringFormat; }
			}

			public StringAlignment Alignment
			{
				get { return stringFormat.Alignment; }
				set { stringFormat.Alignment = value; }
			}

			public StringAlignment LineAlignment
			{
				get { return stringFormat.LineAlignment; }
				set { stringFormat.LineAlignment = value; }
			}
		}

		public class TableColumnCollection : GeneralCollection
		{
			public TableColumnCollection(TableObject owner)
				: base(owner)
			{
			}

			public TableObject TableObject
			{
				get { return ((TableObject) Owner); }
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

		}
		#endregion

		#region TableRowCollection
		public class TableRowCollection : GeneralCollection
		{
			public TableRowCollection(TableObject owner)
				: base(owner)
			{
			}

			public TableObject TableObject
			{
				get { return ((TableObject) Owner); }
			}

			public string[] this[int index]
			{
				get { return (string[]) GetItem(index); }
				set { SetItem(index, value); }
			}

			public void Insert(int index, string[] value)
			{
				InsertItem(index, value);
			}

			public void Remove(string[] value)
			{
				RemoveItem(value);
			}

			public bool Contains(string[] value)
			{
				return base.Contains(value);
			}

			public int IndexOf(string[] value)
			{
				return base.IndexOf(value);
			}

			public int Add(string[] value)
			{
				return AddItem(value);
			}

		}
		#endregion


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

		private Font _headerFont;
		public Font HeaderFont
		{
			get 
			{ 
				if (_headerFont == null)
					return Font;

				return _headerFont; 
			}
			set { _headerFont = value; }
		}

		protected Brush _headerBackBrush;
		protected Color _headerBackColor;
		public Color HeaderBackColor
		{
			get { return _headerBackColor; }
			set 
			{ 
				_headerBackColor = value; 
				if (_headerBackColor == Color.Transparent)
					_headerBackBrush = null;
				else
					_headerBackBrush = new SolidBrush(_headerBackColor);
			}
		}

		protected Brush _headerForeBrush;
		protected Color _headerForeColor;
		public Color HeaderForeColor
		{
			get { return _headerForeColor; }
			set 
			{ 
				_headerForeColor = value;
				_headerForeBrush = new SolidBrush(_headerForeColor);
			}
		}

		private Pen _headerBorder;
		public Pen HeaderBorder
		{
			get { return _headerBorder; }
			set { _headerBorder = value; }
		}

		private Pen _headerColumnBorder;
		public Pen HeaderColumnBorder
		{
			get { return _headerColumnBorder; }
			set { _headerColumnBorder = value; }
		}

		private Pen _cellBorder;
		public Pen CellBorder
		{
			get { return _cellBorder; }
			set { _cellBorder = value; }
		}

		private Pen _rowBorder;
		public Pen RowBorder
		{
			get { return _rowBorder; }
			set { _rowBorder = value; }
		}

		private int lastRow = 0;
		private int[] widthes = null;
		private int headerHeight = 0;

		private void Initialize(Graphics g)
		{
			// Initializing...
			lastRow = 0;
			widthes = new int[_columns.Count];
			int total = 0;
			int sum = 0;
			headerHeight = 0;
			Font headerFont = HeaderFont;
			foreach (TableColumn column in _columns)
			{
				total += column.Width;
			}
			for (int n = 0; n < _columns.Count; n++)
			{
				TableColumn column = _columns[n];
				if (n == _columns.Count - 1)
				{
					widthes[n] = _bounds.Width - sum;
				}
				else
				{
					widthes[n] = (int) ((float) _bounds.Width * (float) column.Width / (float) total);
					sum += widthes[n];
				}

				int h = (int) g.MeasureString(column.Title, headerFont, widthes[n]).Height;
				if (h > headerHeight)
					headerHeight = h;
			}

			headerHeight += (int) ((float) headerHeight * 0.2F);
		}

		public override int GetPageCount(Graphics g)
		{
			if (widthes == null)
				Initialize(g);

			int pageCount = 1;
			int height = _bounds.Height - headerHeight;
			int pageHeight = height;
			int rowHeight;
			for (int r = 0; r < _rows.Count; r++)
			{
				string[] row = _rows[r];
				int cells = widthes.Length > row.Length ? row.Length : widthes.Length;
				rowHeight = 0;
				for (int n = 0; n < cells; n++)
				{
					int h = (int) g.MeasureString(row[n], Font, widthes[n], _columns[n].StringFormat).Height;
					if (h > rowHeight)
						rowHeight = h;
				}
				pageHeight -= rowHeight + 10;
				if (pageHeight < 0)
				{
					pageCount++;
					pageHeight = height - (rowHeight + 10);
				}
			}

			return pageCount;
		}
		
		public override void Save(Sport.WordParser.WordParser wp)
		{
			int tableNum = wp.insertTable(_rows.Count + 1,_columns.Count,2);
			// Write the titles
			for (int column = 0; column < _columns.Count; column++)
			{
				wp.writeTable(tableNum,1,column + 1,HeaderBackColor,_columns[column].Title);
			}

			// The row data
			for (int row = 0; row < _rows.Count;row++)
			{
				string[] currentRowString = _rows[row];
				for (int column = 0;column < currentRowString.Length;column++)
				{
					wp.writeTable(tableNum,row + 2,column + 1,BackColor,currentRowString[column]);
				}
			}
			
		}

		public override int Save(Sport.ExcelParser.ExcelParser ep,int currentRow)
		{
			// Write the titles
			for (int column = 0; column < _columns.Count; column++)
			{
				ep.writeToCell(currentRow,column + 1,_columns[column].Title,HeaderBackColor);
				ep.changeCellBorders(currentRow,column + 1,true);
			}

			currentRow++;

			// The row data
			for (int row = 0; row < _rows.Count;row++)
			{
				string[] currentRowString = _rows[row];
				for (int column = 0;column < currentRowString.Length;column++)
				{
					ep.writeToCell(currentRow,column + 1,currentRowString[column],BackColor);
					ep.changeCellBorders(currentRow,column + 1,true);
				}
				currentRow++;
			}
		
			
			return currentRow;
		}

		public override bool Save(Sport.Rtf.Page page)
		{
			Sport.Rtf.Table table = new Sport.Rtf.Table();
			page.Items.Add(table);
			table.Bounds = TwipConverter.ToTwips(_bounds);

			table.Style = GetRtfStyle();

			Font headerFont = HeaderFont;

			if (headerFont != null)
			{
				table.HeaderStyle.Font = new Sport.Rtf.Font(headerFont.FontFamily.Name, TwipConverter.ToTwips((int) headerFont.Size), headerFont.Bold);
			}

			Graphics g = Graphics.FromImage(new Bitmap(100, 100));

			if (widthes == null)
				Initialize(g);

			table.HeaderStyle.BackColor = _headerBackColor;
			table.HeaderStyle.ForeColor = _headerForeColor;
			if (_headerBorder != null)
			{
				table.HeaderStyle.FrameSize = TwipConverter.ToTwips((int) _headerBorder.Width);
				table.HeaderStyle.FrameColor = _headerBorder.Color;
			}

			table.HeaderStyle.Alignment = StringAlignment.Center;
			table.HeaderStyle.LineAlignment = StringAlignment.Center;

			table.HeaderSize = TwipConverter.ToTwips(headerHeight);

			for (int column = 0; column < _columns.Count; column++)
			{
				Sport.Rtf.Table.TableColumn tb = new Sport.Rtf.Table.TableColumn(_columns[column].Title, TwipConverter.ToTwips(widthes[column]));
				if (_cellBorder != null)
				{
					tb.Style.FrameSize = TwipConverter.ToTwips((int) _cellBorder.Width);
					tb.Style.FrameColor = _cellBorder.Color;
				}

				table.Columns.Add(tb);
			}

			int top = _bounds.Top + headerHeight;
			int bottom = top;
			int rowHeight;

			while (lastRow < _rows.Count && bottom <= _bounds.Bottom)
			{
				string[] row = _rows[lastRow];
				if (row != null)
				{
					int cells = widthes.Length > row.Length ? row.Length : widthes.Length;
					rowHeight = 0;
					for (int n = 0; n < cells; n++)
					{
						int h = (int) g.MeasureString(row[n], Font, widthes[n], _columns[n].StringFormat).Height;
						if (h > rowHeight)
							rowHeight = h;
					}

					if (rowHeight > 0)
					{
						rowHeight += 10;
						bottom = top + rowHeight;

						if (bottom <= _bounds.Bottom)
						{
							table.Rows.Add(row, TwipConverter.ToTwips(rowHeight));

							lastRow++;
						}

						top = bottom;
					}
					else
					{
						lastRow++;
					}
				}
				else
				{
					lastRow++;
				}
			}

			g.Dispose();

			if (lastRow < _rows.Count)
				return true;

			widthes = null;
			return false;
		}

		public override bool Print(Graphics g)
		{
			Font headerFont = HeaderFont;
			if (widthes == null)
				Initialize(g);

			// Header
			if (_headerBackBrush != null)
			{
				g.FillRectangle(_headerBackBrush, _bounds.X, _bounds.Y,
					_bounds.Width, headerHeight);
			}
			else if (_backBrush != null)
			{
				g.FillRectangle(_backBrush, _bounds.X, _bounds.Y,
					_bounds.Width, headerHeight);
			}

			if (_headerBorder != null)
			{
				g.DrawRectangle(_headerBorder, _bounds.X, _bounds.Y,
					_bounds.Width, headerHeight);
			}

			StringFormat sf = new StringFormat();
			sf.LineAlignment = StringAlignment.Center;
			sf.Alignment = StringAlignment.Center;
			if (Direction == PrintDirection.Right)
				sf.FormatFlags = StringFormatFlags.DirectionRightToLeft;
			else
				sf.FormatFlags = (StringFormatFlags) 0;

			int left = _bounds.Left;
			if (Direction == PrintDirection.Right)
			{
				for (int n = _columns.Count - 1; n >= 0; n--)
				{
					Rectangle rect = new Rectangle(left, _bounds.Top, widthes[n], headerHeight);
					g.DrawString(_columns[n].Title, headerFont, _headerForeBrush, rect, sf);
					if (_headerColumnBorder != null)
						g.DrawRectangle(_headerColumnBorder, rect);
					left += widthes[n];
					_columns[n].StringFormat.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
				}
			}
			else
			{
				for (int n = 0; n < _columns.Count; n++)
				{
					Rectangle rect = new Rectangle(left, _bounds.Top, widthes[n], headerHeight);
					g.DrawString(_columns[n].Title, headerFont, _headerForeBrush, rect, sf);
					if (_headerColumnBorder != null)
						g.DrawRectangle(_headerColumnBorder, rect);
					left += widthes[n];
					_columns[n].StringFormat.FormatFlags &= ~StringFormatFlags.DirectionRightToLeft;
				}
			}

			int top = _bounds.Top + headerHeight;
			int bottom = top;
			int rowHeight;

			while (lastRow < _rows.Count && bottom <= _bounds.Bottom)
			{
				string[] row = _rows[lastRow];
				if (row != null)
				{
					int cells = widthes.Length > row.Length ? row.Length : widthes.Length;
					rowHeight = 0;
					for (int n = 0; n < cells; n++)
					{
						int h = (int) g.MeasureString(row[n], Font, widthes[n], _columns[n].StringFormat).Height;
						if (h > rowHeight)
							rowHeight = h;
					}

					if (rowHeight > 0)
					{
						rowHeight += 10;
						bottom = top + rowHeight;

						if (bottom <= _bounds.Bottom)
						{
							if (_backBrush != null)
							{
								g.FillRectangle(_backBrush, _bounds.Left, top, _bounds.Width, rowHeight);
							}

							left = _bounds.Left;

							if (Direction == PrintDirection.Right)
							{
								for (int n = _columns.Count - 1; n >= 0; n--)
								{
									if (n < cells)
									{
										g.DrawString(row[n], Font, _foreBrush,
											new Rectangle(left, top + 5, widthes[n], rowHeight), _columns[n].StringFormat);
										if (_cellBorder != null)
											g.DrawRectangle(_cellBorder, left, top, widthes[n], rowHeight);
									}

									left += widthes[n];
								}
							}
							else
							{
								for (int n = 0; n < cells; n++)
								{
									g.DrawString(row[n], Font, _foreBrush,
										new Rectangle(left, top + 5, widthes[n], rowHeight), _columns[n].StringFormat);
									if (_cellBorder != null)
										g.DrawRectangle(_cellBorder, left, top, widthes[n], rowHeight);
									left += widthes[n];
								}
							}

							if (_rowBorder != null)
							{
								g.DrawRectangle(_rowBorder, _bounds.Left, top, _bounds.Width, rowHeight);
							}

							lastRow++;
						}

						top = bottom;
					}
					else
					{
						lastRow++;
					}
				}
				else
				{
					lastRow++;
				}
			}

			if (lastRow < _rows.Count)
				return true;

			widthes = null;
			return false;
		}

	}

	#endregion

	#region PlayerCardsObject
	public class PlayerCardsObject : SectionObject
	{
		#region PlayerData
		public struct PlayerData
		{
			public string schoolSymbol;
			public string teamName;
			public string sportsName;
			public string studentName;
			public string studentNumber;
			public string studentBirthday;
			public string studentGrade;
			public string issueDate;
			public static PlayerData Empty;

			static PlayerData()
			{ Empty = new PlayerData(); }

			public static PlayerData Clone(PlayerData player)
			{
				PlayerData result=new PlayerData();
				result.schoolSymbol = player.schoolSymbol;
				result.teamName = player.teamName;
				result.sportsName = player.sportsName;
				result.studentName = player.studentName;
				result.studentNumber = player.studentNumber;
				result.studentBirthday = player.studentBirthday;
				result.studentGrade = player.studentGrade;
				result.issueDate = player.issueDate;
				return result;
			}
		}

		#endregion
		
		#region PlayerCard
		public class PlayerCard
		{
			private PlayerData _player;
			private System.Drawing.Image _picture=null;
			
			public PlayerCard(PlayerData player)
			{
				//store clone of given player data:
				_player = PlayerData.Clone(player);
			}
			
			public PlayerData Player
			{
				get {return _player;}
				set {_player = PlayerData.Clone(value);}
			}

			public System.Drawing.Image Picture
			{
				get {return _picture;}
				set {_picture = value;}
			}
		}

		/// <summary>
		/// collection of Player Cards to be printed.
		/// </summary>
		public class PlayerCardCollection : GeneralCollection
		{
			public PlayerCardCollection()
				: base()
			{
			}
			
			public PlayerCard this[int index]
			{
				get { return (PlayerCard) GetItem(index); }
				set { SetItem(index, value); }
			}

			public void Insert(int index, PlayerCard value)
			{
				InsertItem(index, value);
			}

			public void Remove(PlayerCard value)
			{
				RemoveItem(value);
			}

			public bool Contains(PlayerCard value)
			{
				return base.Contains(value);
			}

			public int IndexOf(PlayerCard value)
			{
				return base.IndexOf(value);
			}

			public int Add(PlayerCard value)
			{
				return AddItem(value);
			}
			
			// Overriding += to use Add
			public static PlayerCardCollection operator + (PlayerCardCollection collection, PlayerCard player)
			{
				collection.Add(player);
				return collection;
			}
		}
		#endregion
		
		private PlayerCardCollection _cards;
		private int _lastCard=0;
		public static readonly int CardsPerPage=8;

		public PlayerCardsObject(Rectangle bounds)
			: base(bounds)
		{
			_cards = new PlayerCardCollection();
		}

		public PlayerCardCollection Cards
		{
			get {return _cards;}
			set {_cards = value;}
		}
		
		public override int GetPageCount(Graphics g)
		{
			if (_cards.Count == 0)
				return 1;
			return (((int) ((_cards.Count-1)/CardsPerPage))+1);
		}

		/// <summary>
		/// prints the section, return whether there are more pages or not
		/// </summary>
		public override bool Print(Graphics g)
		{
			int initialLeft=8;//_bounds.Left;
			int initialTop=8;//_bounds.Top;
			int top=initialTop;
			int labelHeight=15;		//height of label
			int headerHeight=48;//20;	//height of header
			int teamNameGap=12;//25;		//vertical gap between team name and school symbol
			int teamNameLeft=0;//30;	//horizontal gap between school symbol and team name.
			int labelWidth=260;//150;		//maximum width of the labels.
			int captionWidth=80;	//maximum width of label caption.
			int horizontalGap=28;	//gap between each student card.
			int verticalGap=60;		//gap between each student card.
			int seperatorWidth=20;	//width of seperator between label and caption
			int labelVerticalGap=3; //vertical gap between each label
			string seperator=" : ";
			int cardTotalWidth=teamNameLeft+labelWidth+seperatorWidth+captionWidth;
			Font headerFont=new Font("Tahoma", 16, System.Drawing.GraphicsUnit.Pixel);
			Font labelFont=new Font("Tahoma", 12, System.Drawing.GraphicsUnit.Pixel);
			Font captionFont=new Font("Tahoma", 12, FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			StringFormat labelFormat=new StringFormat(StringFormatFlags.DirectionRightToLeft);
			string[] labels;
			string[] captions=new string[] {"ענף/קבוצה", "שם", "ת.ז.", 
									   "ת. לידה", "כיתה", "ת. הוצאה"};
			int cardTotalHeight=headerHeight+teamNameGap+headerHeight+(captions.Length*labelHeight)+((captions.Length-1)*labelVerticalGap);
			int pictureWidth=100; //cardTotalWidth-captionWidth-labelWidth-seperatorWidth;
			int pictureHeight=100; //cardTotalHeight-headerHeight-teamNameGap-headerHeight;
			
			StringFormat nameFormat = new StringFormat(StringFormatFlags.DirectionRightToLeft);
			nameFormat.LineAlignment = StringAlignment.Center;
			nameFormat.Alignment = StringAlignment.Far;
			
			//loop over cards:
			int cardIndex=_lastCard;
			int cardsCount=0;
			while ((cardIndex<Cards.Count)&&(cardsCount < CardsPerPage))
			{
				//get current card:
				PlayerCard card=Cards[cardIndex];
				PlayerData data=card.Player;
				
				//build labels:
				labels =   new string[] {data.sportsName, data.studentName, data.studentNumber, 
							data.studentBirthday, data.studentGrade, data.issueDate};
				
				//decide position:
				int left=initialLeft;
				if ((cardsCount % 2) == 1)
					left += cardTotalWidth+horizontalGap;
				if ((cardsCount > 0)&&((cardsCount % 2) == 0))
					top += (cardTotalHeight+verticalGap);

				//print first header - team name:
				g.DrawString(data.teamName, headerFont, _foreBrush,		//left.ToString()+","+top.ToString()+","+Cards.Count.ToString()+","+cardTotalHeight.ToString()
					new Rectangle(left+teamNameLeft, top, labelWidth, headerHeight), nameFormat);
				
				//second header is the school symbol:
				g.DrawString(data.schoolSymbol, headerFont, _foreBrush,
					new Rectangle(left, top+headerHeight+teamNameGap, labelWidth, headerHeight));
				
				//picture, if exists:
				if (card.Picture != null)
				{
					Size picSize=Tools.SmartResize(card.Picture, 
						Math.Max(pictureWidth, pictureHeight));
					g.DrawImage(card.Picture, left, 
						top+headerHeight+teamNameGap+headerHeight+10, 
						picSize.Width, picSize.Height);
				}
				
				//update position:
				left += teamNameLeft;

				//labels and captions:
				for (int i=0; i<captions.Length; i++)
				{
					//decide position:
					int labelTop=top+(2*headerHeight)+teamNameGap+(i*(labelHeight+labelVerticalGap));
					
					//label
					g.DrawString(labels[i], labelFont, _foreBrush,
						new Rectangle(left, labelTop, labelWidth, labelHeight), labelFormat);
					
					//seperator
					g.DrawString(seperator, captionFont, _foreBrush,
						new Rectangle(left+labelWidth, labelTop, seperatorWidth, labelHeight), labelFormat);
					
					//caption
					g.DrawString(captions[i], captionFont, _foreBrush,
						new Rectangle(left+labelWidth+seperatorWidth, labelTop, captionWidth, labelHeight), labelFormat);
				} //end loop over labels and captions
				
				//advance counter:
				cardsCount++;
				cardIndex++;
			}
			_lastCard = cardIndex;
			
			if (_lastCard < Cards.Count)
				return true;

			_lastCard = 0;
			return false;
		}
	}
	#endregion

	#endregion

	#region SectionObjectCollection Class

	public class SectionObjectCollection : GeneralCollection
	{
		public SectionObjectCollection(Section owner)
			: base(owner)
		{
		}

		public Section Section
		{
			get { return ((Section) Owner); }
		}

		public SectionObject this[int index]
		{
			get { return (SectionObject) GetItem(index); }
			set { SetItem(index, value); }
		}

		public void Insert(int index, SectionObject value)
		{
			InsertItem(index, value);
		}

		public void Remove(SectionObject value)
		{
			RemoveItem(value);
		}

		public bool Contains(SectionObject value)
		{
			return base.Contains(value);
		}

		public int IndexOf(SectionObject value)
		{
			return base.IndexOf(value);
		}

		public int Add(SectionObject value)
		{
			return AddItem(value);
		}
	}

	#endregion

	#region Sections

	#region Section Class

	public class Section : GeneralCollection.CollectionItem
	{
		public Section()
		{
			_objects = new SectionObjectCollection(this);
			_direction = PrintDirection.Inherit;
		}

		public PrintDocumentBuilder PrintDocumentBuilder
		{
			get { return ((PrintDocumentBuilder) Owner); }
		}

		private SectionObjectCollection _objects;
		public SectionObjectCollection Objects
		{
			get { return _objects; }
		}

		private Font _font;
		public Font Font
		{
			get 
			{ 
				if (_font == null)
					return PrintDocumentBuilder.Font;
				return _font; 
			}
			set 
			{ 
				_font = value; 
			}
		}

		protected PrintDirection _direction;
		public PrintDirection Direction
		{
			get 
			{ 
				if (_direction == PrintDirection.Inherit)
					return PrintDocumentBuilder.Direction;
				return _direction;
			}
			set
			{
				_direction = value;
			}
		}
		
		public void Save(Sport.WordParser.WordParser wp)
		{
			ArrayList headerStrings = new ArrayList();
			ArrayList footerStrings = new ArrayList();
			ImageObject headerImage = null;
			TableObject mainTable = null;
			SectionObject sct = null;
			int currentObject = 0;
			
			while (currentObject < _objects.Count && (!(_objects[currentObject] is TableObject)))
			{
				sct = _objects[currentObject];
				currentObject++;
				if ((sct is ImageObject)&&(headerImage == null))
					headerImage = (sct as ImageObject);
				else
					headerStrings.Add((sct as TextObject));
			}
			if (currentObject == _objects.Count)
				throw (new Exception("Error! Invalid Section Object!"));
			mainTable = (_objects[currentObject] as TableObject);
			
			currentObject++;
			while (currentObject < _objects.Count)
			{
				sct = _objects[currentObject];
				currentObject++;
				if (sct is TextObject)
					footerStrings.Add((sct as TextObject));
			}

			wp.goToHeader();
			headerImage.Save(wp);
			for (int i = 0;i < headerStrings.Count;i++)
				(headerStrings[i] as TextObject).Save(wp);

			wp.goToMainDocument();
			mainTable.Save(wp);

			wp.goToFooter();
			for (int i = 0;i < footerStrings.Count;i++)
				(footerStrings[i] as TextObject).Save(wp);
			
		}

		public void Save(Sport.ExcelParser.ExcelParser ep)
		{
			int currentRow = 1;
			foreach (SectionObject so in _objects)
			{
				if (!(so is TextObject && (so as TextObject).Text.IndexOf("עמוד ") != -1))
					currentRow = so.Save(ep,currentRow);			
			}
		}


		public bool Save(Sport.Rtf.Page page)
		{
			bool c = false;
			foreach (SectionObject so in _objects)
			{
				if (so.Save(page))
					c = true;
			}

			return c;
		}

		public bool Print(Graphics g)
		{
			bool c = false;
			foreach (SectionObject so in _objects)
			{
				if (so.Print(g))
					c = true;
			}

			return c;
		}

		public int GetPageCount(Graphics g)
		{
			int spc = 1;
			foreach (SectionObject so in _objects)
			{
				int pc = so.GetPageCount(g);
				if (pc > spc)
					spc = pc;
			}

			return spc;
		}
	}

	#endregion

	#region SectionCollection

	public class SectionCollection : GeneralCollection
	{
		public SectionCollection(PrintDocumentBuilder owner)
			: base(owner)
		{
		}

		public PrintDocumentBuilder PrintDocumentBuilder
		{
			get { return ((PrintDocumentBuilder) Owner); }
		}

		public Section this[int index]
		{
			get { return (Section) GetItem(index); }
			set { SetItem(index, value); }
		}

		public void Insert(int index, Section value)
		{
			InsertItem(index, value);
		}

		public void Remove(Section value)
		{
			RemoveItem(value);
		}

		public bool Contains(Section value)
		{
			return base.Contains(value);
		}

		public int IndexOf(Section value)
		{
			return base.IndexOf(value);
		}

		public int Add(Section value)
		{
			return AddItem(value);
		}
	}

	#endregion

	#endregion

	public class PrintDocumentBuilder
	{
		public PrintDocumentBuilder()
			: this(new PrinterSettings())
		{
		}

		public PrintDocumentBuilder(PrinterSettings settings)
		{
			_sections = new SectionCollection(this);
			_font = new Font("Tahoma", 8.25F);
			_direction = PrintDirection.Left;
			printerSettings = settings;
		}

		public PrintDocumentBuilder(PrinterSettings settings, string title, string author)
			: this(settings)
		{
			_title = title;
			_author = author;
		}

		public PrintDocumentBuilder(PrinterSettings settings, string title)
			: this(settings, title, null)
		{
		}

		public PrintDocument Document
		{
			get
			{
				PrintDocument pd = new PrintDocument();
				pd.PrinterSettings = printerSettings;
				pd.DefaultPageSettings = printerSettings.DefaultPageSettings;
				pd.PrintController = new StandardPrintController();
				pd.BeginPrint += new PrintEventHandler(PrintBegin);
				pd.PrintPage += new PrintPageEventHandler(PrintPage);
				pd.EndPrint += new PrintEventHandler(PrintEnd);
				return pd;
			}
		}


		private SectionCollection _sections;
		public SectionCollection Sections
		{
			get { return _sections; }
		}

		private Font _font;
		public Font Font
		{
			get { return _font; }
			set { _font = value; }
		}

		private PrintDirection _direction;
		public PrintDirection Direction
		{
			get { return _direction; }
			set { _direction = value; }
		}

		private PrinterSettings printerSettings;
		public PrinterSettings PrinterSettings
		{
			get { return printerSettings; }
		}

		public Rectangle Margins
		{
			get 
			{
				int width = printerSettings.DefaultPageSettings.PaperSize.Width -
					printerSettings.DefaultPageSettings.Margins.Left -
					printerSettings.DefaultPageSettings.Margins.Right;
				int height = printerSettings.DefaultPageSettings.PaperSize.Height -
					printerSettings.DefaultPageSettings.Margins.Top -
					printerSettings.DefaultPageSettings.Margins.Bottom;
				if (printerSettings.DefaultPageSettings.Landscape)
				{
					return new Rectangle(
						printerSettings.DefaultPageSettings.Margins.Top,
						printerSettings.DefaultPageSettings.Margins.Left,
						height, width);
				}
				else
				{
					return new Rectangle(
						printerSettings.DefaultPageSettings.Margins.Left,
						printerSettings.DefaultPageSettings.Margins.Top,
						width, height);
				}
			}
		}

		public Size Size
		{
			get 
			{
				if (printerSettings.DefaultPageSettings.Landscape)
				{
					return new Size(
						printerSettings.DefaultPageSettings.PaperSize.Height,
						printerSettings.DefaultPageSettings.PaperSize.Width);
				}
				else
				{
					return new Size(
						printerSettings.DefaultPageSettings.PaperSize.Width,
						printerSettings.DefaultPageSettings.PaperSize.Height);
				}
			}
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

		private int section;
		public int Section
		{
			get { return section; }
		}

		private int sectionPage;
		public int SectionPage
		{
			get { return sectionPage; }
		}

		private int sectionPageCount;
		public int SectionPageCount
		{
			get { return sectionPageCount; }
		}

		private int page;
		public int Page
		{
			get { return page; }
		}

		private int pageCount;
		public int PageCount
		{
			get { return pageCount; }
		}

		private void PrintBegin(object sender, PrintEventArgs e)
		{
			section = 0;
			sectionPage = 0;
			sectionPageCount = 0;
			page = 0;
			pageCount = 0;
		}

		private void PrintPage(object sender, PrintPageEventArgs e)
		{
			if (section >= _sections.Count)
			{
				e.HasMorePages = false;
				return ;
			}

			if (pageCount == 0)
			{
				foreach (Section sct in _sections)
				{
					pageCount += sct.GetPageCount(e.Graphics);
				}
			}

			Section s = _sections[section];

			sectionPageCount = s.GetPageCount(e.Graphics);

			if (s.Print(e.Graphics))
			{
				sectionPage++;
				e.HasMorePages = true;
			}
			else
			{
				section++;
				sectionPage = 0;
				e.HasMorePages = section < _sections.Count;
			}
			page++;
		}

		private void PrintEnd(object sender, PrintEventArgs e)
		{
		}

		public Sport.WordParser.WordParser CreateWord(string fileName)
		{
			Sport.WordParser.WordParser wp = new Sport.WordParser.WordParser(fileName);

			foreach (Section sct in _sections)
			{
				sct.Save(wp);
			}

			return wp;
		}

		public Sport.ExcelParser.ExcelParser CreateExcel(string fileName)
		{
			Sport.ExcelParser.ExcelParser ep = new Sport.ExcelParser.ExcelParser(fileName);
			foreach (Section sct in _sections)
			{
				sct.Save(ep);
			}

			return ep;
		}

		public Sport.Rtf.Document CreateRtf()
		{
			Sport.Rtf.Document document = new Sport.Rtf.Document();
			document.Title = Title;
			document.Author = Author;

			section = 0;
			page = 0;
			pageCount = 0;

			int pageWidth, pageHeight;

			if (printerSettings.DefaultPageSettings.Landscape)
			{
				pageWidth = TwipConverter.ToTwips(printerSettings.DefaultPageSettings.PaperSize.Height);
				pageHeight = TwipConverter.ToTwips(printerSettings.DefaultPageSettings.PaperSize.Width);
			}
			else
			{
				pageWidth = TwipConverter.ToTwips(printerSettings.DefaultPageSettings.PaperSize.Width);
				pageHeight = TwipConverter.ToTwips(printerSettings.DefaultPageSettings.PaperSize.Height);
			}
			
			Graphics g = Graphics.FromImage(new Bitmap(100, 100));

			foreach (Section sct in _sections)
			{
				pageCount += sct.GetPageCount(g);
			}

			Sport.Rtf.Page rtfPage;

			for (section = 0; section < _sections.Count; section++)
			{
				sectionPageCount = _sections[section].GetPageCount(g);
				sectionPage = 0;

				rtfPage = new Sport.Rtf.Page(new System.Drawing.Size(pageWidth, pageHeight));

				while (_sections[section].Save(rtfPage))
				{
					sectionPage++;
					page++;

					document.Pages.Add(rtfPage);
					rtfPage = new Sport.Rtf.Page(new System.Drawing.Size(pageWidth, pageHeight));
				}

				document.Pages.Add(rtfPage);
			}

			g.Dispose();

			return document;
		}
	}
}
