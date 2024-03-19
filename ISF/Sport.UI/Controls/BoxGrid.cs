using System;
using System.Drawing;
using System.Windows.Forms;
using Sport.Common;

namespace Sport.UI.Controls
{
	public class BoxGrid : ControlWrapper
	{
        #region BoxGrid Cell Class

		public struct Cell
		{
			public static readonly Cell Empty = new Cell(-1);

			public int Box;
			public int Part;
			public int SubPart;

			public Cell(int box, int part, int subpart)
			{
				Box = box;
				Part = part;
				SubPart = subpart;
			}

			public Cell(int box)
				: this(box, -1, -1)
			{
			}

		
			public override int GetHashCode()
			{
				return (SubPart << 24) ^ (Part << 16) ^ Box;
			}

			public override bool Equals(object obj)
			{
				if (obj == null)
					return false;

				if (obj is Cell)
				{
					Cell oc = (Cell) obj;
					if (Box == oc.Box && Part == oc.Part && SubPart == oc.SubPart)
						return true;
				}

				return false;
			}
		}

		#endregion

		public class CellDragEventArgs : EventArgs
		{
			private Cell _cell;
			public Cell Cell
			{
				get { return _cell; }
			}

			public CellDragEventArgs(Cell cell)
			{
				_cell = cell;
			}
		}

		public delegate void CellDragEventHandler(object sender, CellDragEventArgs e);

	
		public event CellDragEventHandler CellDrag;

		private Cursor dragCursor;

		private float[] partSizes = null;
		private void RecalcPartsSize()
		{
			if (_boxList == null)
			{
				partSizes = null;
			}
			else
			{
				partSizes = new float[_boxList.GetPartCount()];

				int total = 0;
				int size;
				for (int p = 0; p < partSizes.Length; p++)
				{
					size = _boxList.GetPartSize(p);
					if (size < 1)
						size = 1;
					total += size;
				}

				for (int p = 0; p < partSizes.Length; p++)
				{
					size = _boxList.GetPartSize(p);
					if (size < 1)
						size = 1;
					partSizes[p] = (float) size / (float) total;
				}
			}
		}

		private IBoxList _boxList;
		public IBoxList BoxList
		{
			get
			{
				return _boxList;
			}
			set
			{
				if (_boxList != value)
				{
					if (_boxList != null)
					{
						_boxList.ListChanged -= new EventHandler(ListChanged);
						_boxList.ValueChanged -= new EventHandler(ValueChanged);
					}

					_boxList = value;

					if (_boxList != null)
					{
						_boxList.ListChanged += new EventHandler(ListChanged);
						_boxList.ValueChanged += new EventHandler(ValueChanged);
					}

					RecalcPartsSize();
					ResetScroll();
					Refresh();
				}
			}
		}

		private void ListChanged(object sender, EventArgs e)
		{
			RecalcPartsSize();
			ResetScroll();
			Refresh();
		}

		private void ValueChanged(object sender, EventArgs e)
		{
			Refresh();
		}

		private Cell _selectedCell;
		public Cell SelectedCell
		{
			get { return _selectedCell; }
			set
			{
				if (!_selectedCell.Equals(value))
				{
					_selectedCell = value;
					Refresh();
				}
			}
		}

		public event EventHandler SelectedCellChanged;

		private void SelectCell(Cell cell)
		{
			if (!_selectedCell.Equals(cell))
			{
				_selectedCell = cell;
				if (SelectedCellChanged != null)
					SelectedCellChanged(this, EventArgs.Empty);
				Refresh();
			}
		}

		public BoxGrid()
		{
			_direction = BoxGridDirection.Horizontal;
			_scrollDirection = BoxGridDirection.Vertical;

			_selectedCell = Cell.Empty;

			scroll = new VScrollBar();
			scroll.Dock = RightToLeft == RightToLeft.Yes ? DockStyle.Left : DockStyle.Right;
			scroll.Enabled = false;
			scroll.Minimum = 0;
			scroll.Maximum = 0;
			scroll.Value = 0;
			scroll.SmallChange = 1;
			scroll.LargeChange = 1;
			scroll.Scroll += new ScrollEventHandler(Scrolled);
			Controls.Add(scroll);

			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(BoxGrid));
			byte[] cursor = (byte[]) resources.GetObject("drag.cur");
			
			dragCursor = new Cursor(new System.IO.MemoryStream(cursor));

			SetStyle(ControlStyles.ResizeRedraw, true);
		}

		private void Scrolled(object sender, ScrollEventArgs e)
		{
			if (partSizes == null)
				return;

			if (e.Type == ScrollEventType.SmallDecrement)
			{
				if (_partTotal == 0 || _part == 0)
				{
					if (_line > 0)
					{
						_line--;

						if (_partTotal != 0)
							_part = _boxList.GetPartCount() - 1;
					}
				}
				else
					_part--;
			}
			else if (e.Type == ScrollEventType.SmallIncrement)
			{
				if (_partTotal == 0 || _part == _boxList.GetPartCount() - 1)
				{
					if (_line < _lineCount - 1)
					{
						_line++;

						if (_partTotal != 0)
							_part = 0;
					}
				}
				else
					_part++;
			}
			else if (e.Type == ScrollEventType.First)
			{
				_line = 0;
				_part = 0;
			}
			else if (e.Type == ScrollEventType.Last)
			{
				_line = _lineCount - 1;
				_part = _boxList.GetPartCount() - 1;
			}
			else
			{
				_line = e.NewValue / _lineSize;
	
				if (e.Type == ScrollEventType.LargeDecrement ||
					e.Type == ScrollEventType.LargeIncrement ||
					_partTotal == 0)
				{
					// Round to line
					_part = 0;
				}
				else if (e.Type != ScrollEventType.EndScroll)
				{
					// Round to part
					int val = e.NewValue - (_line * _lineSize);

					_part = 0;
					int t = (int) (_partTotal * partSizes[0]);
					while (t < val && _part < _boxList.GetPartCount() - 1)
					{
						_part++;
						t += (int) (_partTotal * partSizes[_part]);
					}
				}
			}

			int partOffset = 0;
			for (int p = 0; p < _part; p++)
				partOffset += (int) (_partTotal * partSizes[_part]);

			e.NewValue = (_line * _lineSize) + partOffset;
			Refresh();
		}

		private BoxGridDirection _direction;
		public BoxGridDirection Direction
		{
			get { return _direction; }
			set 
			{ 
				_direction = value; 
			}
		}

		protected override void OnRightToLeftChanged(EventArgs e)
		{
			base.OnRightToLeftChanged (e);

			if (_scrollDirection == BoxGridDirection.Vertical)
				scroll.Dock = RightToLeft == RightToLeft.Yes ? 
					DockStyle.Left : DockStyle.Right;
		}

		private BoxGridDirection _scrollDirection;
		public BoxGridDirection ScrollDirection
		{
			get { return _scrollDirection; }
			set 
			{ 
				if (_scrollDirection != value)
				{
					_scrollDirection = value; 
					Controls.Remove(scroll);
					scroll.Dispose();

					if (value == BoxGridDirection.Vertical)
					{
						scroll = new VScrollBar();
						if (RightToLeft == RightToLeft.Yes)
							scroll.Dock = DockStyle.Left;
						else
							scroll.Dock = DockStyle.Right;
					}
					else
					{
						scroll = new HScrollBar();
						scroll.Dock = DockStyle.Bottom;
					}

					scroll.Enabled = false;
					scroll.Minimum = 0;
					scroll.Maximum = 0;
					scroll.Value = 0;
					scroll.SmallChange = 1;
					scroll.LargeChange = 1;
					scroll.Scroll += new ScrollEventHandler(Scrolled);
					Controls.Add(scroll);

					ResetScroll();
					Refresh();
				}
			}
		}

		private ScrollBar scroll;

		private string _title;
		public string Title
		{
			get { return _title; }
			set { _title = value; }
		}

		private int GetScrollValue()
		{
			int val = _line * _lineSize;

			if (partSizes != null && _boxList != null)
			{
				for (int p = 0; p < _part; p++)
					val += (int) (_partTotal * partSizes[p]);
			}

			return val;
		}

		private int _lineCount = 0;
		private int _lineSize = 0;
		private int _line = 0;
		public int Line
		{
			get { return _line; }
			set
			{
				if (_line != value)
				{
					_line = value;
					Refresh();
				}
			}
		}

		private int _partTotal = 0;
		private int _part = 0;
		public int Part
		{
			get { return _part; }
			set
			{
				int max = _boxList == null ? 0 : _boxList.GetPartCount() - 1;
				if (value > max)
					throw new ArgumentOutOfRangeException("value", "Part out of range");

				if (_part != value)
				{
					_part = value;
					Refresh();
				}
			}
		}


		private Size _boxSize = new Size(50, 100);
		public Size BoxSize
		{
			get { return _boxSize; }
			set 
			{ 
				_boxSize = value; 
				Refresh();
			}
		}

		private Size _headerSize = new Size(50, 20);
		public Size HeaderSize
		{
			get { return _headerSize; }
			set 
			{ 
				_headerSize = value; 
				Refresh();
			}
		}

		private Rectangle ConvertRect(Rectangle rect)
		{
			if (_direction == BoxGridDirection.Vertical)
			{
				if (RightToLeft == RightToLeft.Yes)
				{
					if (_scrollDirection == BoxGridDirection.Vertical)
						return new Rectangle(Size.Width - rect.Bottom - scroll.Width - 1, rect.Left, rect.Height, rect.Width);

					return new Rectangle(Size.Width - rect.Bottom, rect.Left, rect.Height, rect.Width);
				}
					
				return new Rectangle(rect.Top, rect.Left, rect.Height, rect.Width);
			}
			else
			{
				if (RightToLeft == RightToLeft.Yes)
				{
					if (_scrollDirection == BoxGridDirection.Vertical)
						return new Rectangle(Size.Width - rect.Right - scroll.Width - 1, rect.Top,
							rect.Width, rect.Height);

					return new Rectangle(Size.Width - rect.Right, rect.Top,
						rect.Width, rect.Height);
				}
			}

			return rect;
		}

		private Rectangle ConvertRect(int x, int y, int width, int height)
		{
			return ConvertRect(new Rectangle(x, y, width, height));
		}

		private Size ConvertSize(Size size)
		{
			if (_direction == BoxGridDirection.Vertical)
				return new Size(size.Height, size.Width);

			return size;
		}

		private Point ConvertPoint(Point point)
		{
			if (_direction == BoxGridDirection.Vertical)
			{
				if (RightToLeft == RightToLeft.Yes)
				{
					if (_scrollDirection == BoxGridDirection.Vertical)
						return new Point(Size.Width - point.Y - scroll.Width - 1, point.X);

					return new Point(Size.Width - point.Y, point.X);
				}
					
				return new Point(point.Y, point.X);
			}
			else
			{
				if (RightToLeft == RightToLeft.Yes)
				{
					if (_scrollDirection == BoxGridDirection.Vertical)
						return new Point(Size.Width - point.X - scroll.Width - 1, point.Y);

					return new Point(Size.Width - point.X, point.Y);
				}
			}

			return point;
		}

		private void DrawItem(Graphics g, Brush brush, 
			string text, Rectangle rect, Brush textBrush)
		{
			Rectangle r = ConvertRect(rect);

			g.FillRectangle(brush, r);
			g.DrawRectangle(SystemPens.WindowFrame, r);

			if (text != null)
			{
				StringFormat sf = RightToLeft == RightToLeft.Yes ?
					new StringFormat(StringFormatFlags.DirectionRightToLeft) :
					new StringFormat();
				sf.LineAlignment = StringAlignment.Center;

				g.DrawString(text, Font, textBrush, r, sf);
			}
		}

		private void DrawBox(Graphics g, Rectangle rect, int box, int boxHeight, bool first)
		{
			DrawItem(g, SystemBrushes.Window, null, rect, null);

			int sum = 0;
			int top = rect.Top;
			int partCount = _boxList.GetPartCount();
			for (int p = first ? _part : 0; p < partCount; p++)
			{
				int subparts = _boxList.GetSubPartCount(p);
				int h = p == partCount - 1 ? boxHeight - sum :
					(int) (boxHeight * ((partSizes == null) ? 1f : partSizes[p]));
				sum += h;

				Rectangle r = new Rectangle(rect.Left, top, rect.Width, h);

				DrawItem(g, SystemBrushes.Window, null, r, null);

				r = ConvertRect(r);

				StringFormat sf = RightToLeft == RightToLeft.Yes ?
					new StringFormat(StringFormatFlags.DirectionRightToLeft) :
					new StringFormat();

				int o = r.Height / subparts;
				int t = r.Top;

				for (int s = 0; s < subparts; s++)
				{
					object obj = _boxList.GetBoxItem(box, p, s);
					Rectangle itemRect = new Rectangle(r.Left, t, r.Width, o);
					if (obj != null)
					{
						g.DrawString(obj.ToString(), Font, SystemBrushes.WindowText,
							itemRect, sf);
					}

					if (_selectedCell.Equals(new Cell(box, p, s)))
					{
						itemRect = new Rectangle(itemRect.Left + 1, itemRect.Top + 1,
							itemRect.Width - 1, itemRect.Height - 1);
						ControlPaint.DrawFocusRectangle(g, itemRect);
					}

					t += o;
				}

				top += h;
			}
		}

		private void DrawGrid(Graphics g, Size size)
		{
			size = ConvertSize(size);
			Size headerSize = ConvertSize(_headerSize);
			Size boxSize = ConvertSize(_boxSize);
			int boxHeight = boxSize.Height;
			int boxWidth = boxSize.Width;
			int boxPerLine = (size.Width - headerSize.Width) / boxWidth;

			int partCount = _boxList.GetPartCount();
			int itemCount = _boxList.GetBoxCount();

			if (_scrollDirection == _direction)
			{
				boxHeight = size.Height - headerSize.Height;
				boxPerLine++;
			}
			else
			{
				if (boxPerLine == 0)
				{
					boxPerLine = 1;
					boxWidth = size.Width - headerSize.Width;
				}
				else if (boxPerLine < itemCount)
				{
					boxWidth = (size.Width - headerSize.Width) / boxPerLine;
				}
			}

			int item = _scrollDirection == _direction ?
				_line : _line * boxPerLine;
			int top = 0;
			bool first = true;
			
			// Drawing lines
			do
			{
				int left = 0;

				// Drawing line header
				Rectangle rect = new Rectangle(left, top,
					headerSize.Width, headerSize.Height);

				DrawItem(g, SystemBrushes.Highlight, _title, rect, SystemBrushes.HighlightText);

				int bh = boxHeight;

				if (first)
				{
					bh = 0;
					for (int p = _part; p < partCount; p++)
					{
						float ps = partSizes == null ? 1f : partSizes[p];
						bh += (int)(boxHeight * ps);
					}
				}

				// Drawing parts headers
				int t = top + headerSize.Height;
				int s = boxHeight - bh;
				for (int p = first ? _part : 0; p < partCount; p++)
				{
					float ps = partSizes == null ? 1f : partSizes[p];
					int h = p == partCount - 1 ? boxHeight - s :
						(int) (boxHeight * ps);
					
					rect = new Rectangle(left, t, headerSize.Width, h);

					DrawItem(g, SystemBrushes.Highlight, 
						_boxList.GetPartTitle(p), rect, SystemBrushes.HighlightText);

					t += h;
					s += h;
				}

				left += headerSize.Width;
				int i = 0;
			
				while (i < boxPerLine && item < itemCount)
				{
					rect = new Rectangle(left, top, 
						boxWidth, headerSize.Height);
					DrawItem(g, SystemBrushes.Highlight, 
						_boxList.GetBoxTitle(item), rect, SystemBrushes.HighlightText);
					DrawBox(g, new Rectangle(left, top + headerSize.Height,
						boxWidth, bh), item, boxHeight, first);
					if (_selectedCell.Equals(new Cell(item)))
					{
						Rectangle focusRect = ConvertRect(new Rectangle(left, top + 1, boxWidth - 1, 
							boxHeight + headerSize.Height - 1));
						ControlPaint.DrawFocusRectangle(g, focusRect);
					}

					left += boxWidth;
					item++;
					i++;
				}

				top += bh+ headerSize.Height;
				first = false;
			} while (top < size.Height && item < itemCount);

		}

		protected override void OnPaintBackground(PaintEventArgs pevent)
		{
		}

		private Bitmap bitMap = null;
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint (e);

			Size size = _scrollDirection == BoxGridDirection.Horizontal ?
				new Size(Size.Width - 1, Size.Height - scroll.Height - 1) :
				new Size(Size.Width - scroll.Width - 1, Size.Height - 1);

			if (size.Width > 0 && size.Height > 0)
			{
				if (bitMap == null)
					bitMap = new Bitmap(size.Width + 1, size.Height + 1, e.Graphics);
				Graphics g = Graphics.FromImage(bitMap);

				g.FillRectangle(SystemBrushes.Control, 0, 0, size.Width + 1, size.Height + 1);

				if (_boxList != null)
				{
					DrawGrid(g, size);
				}

				int left = 0;
				if (_scrollDirection == BoxGridDirection.Vertical &&
					RightToLeft == RightToLeft.Yes)
					left = scroll.Width;

				e.Graphics.DrawImage(bitMap, new Point(left, 0));
			}
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize (e);

			bitMap = null;

			ResetScroll();
		}


		private void ResetScroll()
		{
			Size size = _scrollDirection == BoxGridDirection.Horizontal ?
				new Size(Size.Width - 1, Size.Height - scroll.Height - 1) :
				new Size(Size.Width - scroll.Width - 1, Size.Height - 1);

			_lineCount = 1;
			int totalSize = 10;

			if (_boxList != null)
			{
				int boxCount = _boxList.GetBoxCount();

				if (_direction == BoxGridDirection.Horizontal)
				{
					if (_scrollDirection == BoxGridDirection.Vertical)
					{
						int boxPerLine = (size.Width - _headerSize.Width) / _boxSize.Width;

						if (boxPerLine == 0)
							boxPerLine = 1;

						_lineCount = boxCount / boxPerLine;
						if (boxCount % boxPerLine != 0)
							_lineCount++;

						_lineSize = _boxSize.Height + _headerSize.Height;
						_partTotal = _boxSize.Height;
						totalSize = size.Height;
					}
					else
					{
						_lineCount = boxCount;
						_lineSize = _boxSize.Width;
						_partTotal = 0;
						totalSize = size.Width - _headerSize.Width;
					}
				}
				else
				{
					if (_scrollDirection == BoxGridDirection.Horizontal)
					{
						int boxPerLine = (size.Height - _headerSize.Height) / _boxSize.Height;

						if (boxPerLine == 0)
							boxPerLine = 1;

						_lineCount = boxCount / boxPerLine;
						if (boxCount % boxPerLine != 0)
							_lineCount++;

						_lineSize = _boxSize.Width + _headerSize.Width;
						_partTotal = _boxSize.Width;
						totalSize = size.Width;
					}
					else
					{
						_lineCount = boxCount;
						_lineSize = _boxSize.Height;
						_partTotal = 0;
						totalSize = size.Height - _headerSize.Height;
					}
				}
			}

			int max = _lineCount * _lineSize;
			int page = totalSize;

			if (max > page && page > 0)
			{
				scroll.Enabled = true;
				scroll.Maximum = max;
				scroll.Minimum = 0;
				scroll.LargeChange = page;

				scroll.Value = GetScrollValue();
			}
			else
			{
				scroll.Enabled = false;
			}
		}


		public enum HitTestType
		{
			None,
			Header,
			PartHeader,
			ItemHeader,
			Item
		}

		public class HitTestInfo
		{
			private HitTestType _type;
			public HitTestType Type { get { return _type; } }

			private Rectangle _bounds;
			public Rectangle Bounds { get { return _bounds; } }

			private int _box;
			public int Box { get { return _box; } }

			private int _part;
			public int Part { get { return _part; } }

			private int _subpart;
			public int SubPart { get { return _subpart; } }

			public HitTestInfo(HitTestType type, Rectangle bounds, 
				int box, int part, int subpart)
			{
				_type = type;
				_bounds = bounds;
				_box = box;
				_part = part;
				_subpart = subpart;
			}

			public HitTestInfo(HitTestType type, Rectangle bounds,
				int box, int part)
				: this(type, bounds, box, part, -1)
			{
			}

			public HitTestInfo(HitTestType type, Rectangle bounds,
				int box)
				: this(type, bounds, box, -1, -1)
			{
			}

			public HitTestInfo(HitTestType type, Rectangle bounds)
				: this(type, bounds, -1, -1, -1)
			{
			}
		}

		public HitTestInfo HitTest(Point pt)
		{
			return HitTest(pt.X, pt.Y);
		}

		public HitTestInfo HitTest(int x, int y)
		{
			if (RightToLeft == RightToLeft.Yes)
			{
				x = Size.Width - x;
			}

			if (_direction == BoxGridDirection.Vertical)
			{
				int t = x;
				x = y;
				y = t;
			}

			if (x < 0 || y < 0 || _boxList == null)
				return new HitTestInfo(HitTestType.None, 
					ConvertRect(new Rectangle(0, 0, Size.Width, Size.Height)));

			Size size = ConvertSize(_scrollDirection == BoxGridDirection.Horizontal ?
				new Size(Size.Width - 1, Size.Height - scroll.Height - 1) :
				new Size(Size.Width - scroll.Width - 1, Size.Height - 1));

			Size boxSize = ConvertSize(_boxSize);
			Size headerSize = ConvertSize(_headerSize);

			int boxHeight = boxSize.Height;
			int boxWidth = boxSize.Width;
			int boxPerLine = (size.Width - headerSize.Width) / boxWidth;
			int itemCount = _boxList.GetBoxCount();

			if (_scrollDirection == _direction)
			{
				boxHeight = size.Height - headerSize.Height;
				boxPerLine++;
			}
			else
			{
				if (boxPerLine == 0)
				{
					boxPerLine = 1;
					boxWidth = size.Width - headerSize.Width;
				}
				else if (boxPerLine < itemCount)
				{
					boxWidth = (size.Width - headerSize.Width) / boxPerLine;
				}
			}

			if (y < headerSize.Height)
			{
				if (x < headerSize.Width)
					return new HitTestInfo(HitTestType.Header,
						ConvertRect(0, 0, headerSize.Width, headerSize.Height));

				x -= headerSize.Width;
				int item = x / boxWidth;
				int left = item * boxWidth + headerSize.Width;
				if (_scrollDirection == _direction)
					item += _line;
				else
					item += _line * boxPerLine;

				if (item >= itemCount)
					return new HitTestInfo(HitTestType.None, 
						ConvertRect(new Rectangle(0, 0, Size.Width, Size.Height)));

				return new HitTestInfo(HitTestType.ItemHeader,
					ConvertRect(left, 0, boxWidth, headerSize.Height),
					item);
			}

			for (int p = 0; p < _part; p++)
			{
				float ps = partSizes == null ? 1f : partSizes[p];
				y += (int)(boxHeight * ps);
			}
		
			int line = y / (boxHeight + headerSize.Height);
			int top = line * (boxHeight + headerSize.Height);
			y = y % (boxHeight + headerSize.Height);

			if (y < headerSize.Height)
			{
				if (x < headerSize.Width)
					return new HitTestInfo(HitTestType.Header,
						ConvertRect(0, 0, headerSize.Width, headerSize.Height));
				x -= headerSize.Width;
				int item = x / boxWidth;
				int left = item * boxWidth + headerSize.Width;
				if (_scrollDirection == _direction)
					item += line * boxPerLine + _line;
				else
					item += (line + _line) * boxPerLine;

				if (item >= itemCount)
					return new HitTestInfo(HitTestType.None, 
						ConvertRect(new Rectangle(0, 0, Size.Width, Size.Height)));

				return new HitTestInfo(HitTestType.ItemHeader,
					ConvertRect(left, top, boxWidth, headerSize.Height),
					item);
			}

			y -= headerSize.Height;
			top += headerSize.Height;
			int pc = _boxList.GetPartCount();
			int s = 0;
			for (int p = 0; p < pc; p++)
			{
				float ps = partSizes == null ? 1f : partSizes[p];
				int h = p == pc - 1 ? boxHeight - s :
					(int) (boxHeight * ps);
				if (y < h)
				{
					if (x < headerSize.Width)
					{
						return new HitTestInfo(HitTestType.PartHeader,
							ConvertRect(0, top + s, headerSize.Width, h), -1, p);
					}

					x -= headerSize.Width;
					int item = x / boxWidth;
					x -= (item * boxWidth);
					int left = item * boxWidth + headerSize.Width;
					if (_scrollDirection == _direction)
						item += line * boxPerLine + _line;
					else
						item += (line + _line) * boxPerLine;

					if (item >= itemCount)
						return new HitTestInfo(HitTestType.None, 
							ConvertRect(new Rectangle(0, 0, Size.Width, Size.Height)));

					int sub;
					if (_direction == BoxGridDirection.Horizontal)
					{
						sub = y / (h / _boxList.GetSubPartCount(p));
					}
					else
					{
						sub = x / (boxWidth / _boxList.GetSubPartCount(p));
					}

					return new HitTestInfo(HitTestType.Item,
						ConvertRect(left, top + s, boxWidth, h),
						item, p, sub);
				}
                
				y -= h;
				s += h;
			}

			return new HitTestInfo(HitTestType.None, 
				ConvertRect(new Rectangle(0, 0, Size.Width, Size.Height)));
		}

		private class DragInfo
		{
			public HitTestType	HitTestType;
			public Rectangle	DragBox;
			public Cell			Cell;
			public bool			Moved;
			public DragInfo(HitTestType type, Cell cell, Rectangle dragBox)
			{
				DragBox = dragBox;
				Cell = cell;
				HitTestType = type;
				Moved = false;
			}
		}

		private DragInfo _dragInfo = null;

		protected override void OnLostFocus(EventArgs e)
		{
			_dragInfo = null;
			base.OnLostFocus (e);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown (e);

			if (!Focused)
				Focus();

			if (e.Button == MouseButtons.Left)
			{
				HitTestInfo hti = HitTest(e.X, e.Y);
				
				Cell cell = new Cell(hti.Box, hti.Part, hti.SubPart);
				switch (hti.Type)
				{
					case (HitTestType.Header):
					case (HitTestType.PartHeader):
						SelectCell(Cell.Empty);
						break;
					case (HitTestType.ItemHeader):
						SelectCell(new Cell(cell.Box));
						break;
					case (HitTestType.Item):
						SelectCell(cell);
						break;
				}

				Size dragSize = SystemInformation.DragSize;

				_dragInfo = new DragInfo(hti.Type, cell,
					new Rectangle(e.X - (dragSize.Width / 2), 
					e.Y - (dragSize.Height / 2), dragSize.Width, dragSize.Height));
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove (e);

			if (e.Button == MouseButtons.Left && _dragInfo != null)
			{
				if (_dragInfo.Moved)
				{
//					DoDragDrop(_dragInfo.Cell, DragDropEffects.Move);
							
				}
				else
				{
					if (!_dragInfo.DragBox.Contains(e.X, e.Y))
					{
						_dragInfo.Moved = true;
						if (CellDrag != null)
						{
							CellDrag(this, new CellDragEventArgs(_dragInfo.Cell));
//							DoDragDrop(_dragInfo.Cell, DragDropEffects.Move);
						}
					}
				}
			}
		}


		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseDown (e);

			if (e.Button == MouseButtons.Left && _dragInfo != null)
			{
				if (!_dragInfo.Moved)
				{
					switch (_dragInfo.HitTestType)
					{
						case (HitTestType.Header):
							OnHeaderClick();
							break;
						case (HitTestType.PartHeader):
							OnPartClick(_dragInfo.Cell.Part);
							break;
						case (HitTestType.ItemHeader):
							OnBoxClick(_dragInfo.Cell.Box, -1, -1);
							break;
						case (HitTestType.Item):
							OnBoxClick(_dragInfo.Cell.Box, 
								_dragInfo.Cell.Part, 
								_dragInfo.Cell.SubPart);
							break;
					}
				}
			}

			_dragInfo = null;
		}

		public class BoxEventArgs : System.EventArgs
		{
			private int _box;
			private int _part;
			private int _subpart;

			public int Box		{ get { return _box; } }
			public int Part		{ get { return _part; } }
			public int SubPart	{ get { return _subpart; } }

			public BoxEventArgs(int box, int part, int subpart)
			{
				_box = box;
				_part = part;
				_subpart = subpart;
			}
		}

		public delegate void BoxEventHandler(object sender, BoxEventArgs e);

		public event EventHandler		HeaderClick;
		public event BoxEventHandler	PartClick;
		public event BoxEventHandler	BoxClick;

		protected void OnHeaderClick()
		{
			if (HeaderClick != null)
				HeaderClick(this, EventArgs.Empty);
		}

		protected void OnPartClick(int part)
		{
			if (PartClick != null)
				PartClick(this, new BoxEventArgs(-1, part, -1));
		}

		protected void OnBoxClick(int box, int part, int subpart)
		{
			if (BoxClick != null)
				BoxClick(this, new BoxEventArgs(box, part, subpart));
		}

		protected override bool IsInputKey(Keys keyData)
		{
			switch (keyData & (~Keys.Shift))
			{
				case (Keys.Down):
				case (Keys.Up):
				case (Keys.Left):
				case (Keys.Right):
				case (Keys.PageDown):
				case (Keys.PageUp):
				case (Keys.Home):
				case (Keys.End):
				case (Keys.Space):
					return true;
			}

			return base.IsInputKey (keyData);
		}

		private void SelectNextCell()
		{
			int boxCount = _boxList.GetBoxCount();
			int partCount = _boxList.GetPartCount();
			if (_selectedCell.Equals(Cell.Empty))
			{
				if (boxCount > 0 && partCount > 0)
					SelectCell(new Cell(0, 0, 0));
			}
			else
			{
				int subpartCount = _boxList.GetSubPartCount(_selectedCell.Part);
				if (_selectedCell.SubPart < subpartCount - 1)
				{
					SelectCell(new Cell(_selectedCell.Box, _selectedCell.Part,
						_selectedCell.SubPart + 1));
				}
				else if (_selectedCell.Part < partCount - 1)
				{
					SelectCell(new Cell(_selectedCell.Box, _selectedCell.Part + 1, 0));
				}
				else if (partCount > 0 && _selectedCell.Box < boxCount - 1)
				{
					SelectCell(new Cell(_selectedCell.Box + 1, 0, 0));
				}
			}
		}

		private void SelectPrevCell()
		{
			int boxCount = _boxList.GetBoxCount();
			int partCount = _boxList.GetPartCount();
			if (_selectedCell.Equals(Cell.Empty))
			{
				if (boxCount > 0 && partCount > 0)
				{
					int subpartCount = _boxList.GetSubPartCount(partCount - 1);
					SelectCell(new Cell(boxCount - 1, partCount - 1, subpartCount - 1));
				}
			}
			else
			{
				if (_selectedCell.SubPart > 0)
				{
					SelectCell(new Cell(_selectedCell.Box, _selectedCell.Part,
						_selectedCell.SubPart - 1));
				}
				else if (_selectedCell.Part > 0)
				{
					int subpartCount = _boxList.GetSubPartCount(_selectedCell.Part - 1);
					SelectCell(new Cell(_selectedCell.Box, _selectedCell.Part - 1,
						subpartCount - 1));
				}
				else if (_selectedCell.Box > 0)
				{
					int subpartCount = _boxList.GetSubPartCount(partCount - 1);
					SelectCell(new Cell(_selectedCell.Box - 1, partCount - 1,
						subpartCount - 1));
				}
			}
		}

		private void SelectNextBox()
		{
			if (_selectedCell.Equals(Cell.Empty))
			{
				SelectNextCell();
			}
			else 
			{
				int boxCount = _boxList.GetBoxCount();
				if (_selectedCell.Box < boxCount - 1)
				{
					SelectCell(new Cell(_selectedCell.Box + 1, _selectedCell.Part, 
						_selectedCell.SubPart));
				}
			}
		}

		private void SelectPrevBox()
		{
			if (_selectedCell.Equals(Cell.Empty))
			{
				SelectPrevCell();
			}
			else 
			{
				if (_selectedCell.Box > 0)
				{
					SelectCell(new Cell(_selectedCell.Box - 1, _selectedCell.Part, 
						_selectedCell.SubPart));
				}
			}
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
            switch (e.KeyCode)
			{
				case (Keys.Down):
					SelectNextCell();
					break;
				case (Keys.Up):
					SelectPrevCell();
					break;
				case (Keys.Left):
					if (RightToLeft == RightToLeft.Yes)
						SelectNextBox();
					else
						SelectPrevBox();
					break;
				case (Keys.Right):
					if (RightToLeft == RightToLeft.Yes)
						SelectPrevBox();
					else
						SelectNextBox();
					break;
				case (Keys.Home):
					_selectedCell = Cell.Empty;
					SelectNextCell();
					break;
				case (Keys.End):
					_selectedCell = Cell.Empty;
					SelectPrevCell();
					break;
/*				case (Keys.PageDown):
					move = scroll.LargeChange;
					break;
				case (Keys.PageUp):
					move = -scroll.LargeChange;
					break;
				case (Keys.Home):
					move = -_visibleRow;
					break;
				case (Keys.End):
					move = scroll.Maximum - scroll.LargeChange - _visibleRow;
					break;
				case (Keys.Space):
					if (selectedIndex >= 0)
					{
						selectionStart = selectedIndex;
						selection[selectedIndex] = !selection[selectedIndex];
						Refresh();
					}
					break;*/
			}

			base.OnKeyDown (e);
		}
	}

	public enum BoxGridDirection
	{
		Vertical,
		Horizontal
	}

	#region IBoxList Interface

	public interface IBoxList
	{
		// Parts
		int GetPartCount();
		int GetSubPartCount(int part);
		string GetPartTitle(int part);
		int GetPartSize(int part);

		// Boxes
		int GetBoxCount();
		string GetBoxTitle(int box);
		object GetBoxItem(int box, int part, int subpart);

		// Events
		event EventHandler ListChanged;
		event EventHandler ValueChanged;
	}

	#endregion
	
	#region BoxList Class

	public class BoxList : IBoxList
	{
		#region Parts
		
		#region BoxPart Class

		/// <summary>
		/// BoxPart
		/// </summary>
		public sealed class BoxPart
		{
			internal BoxList list;

			private string _title;
			public string Title
			{
				get { return _title; }
				set 
				{ 
					if (_title != value)
					{
						_title = value; 
						if (list != null)
							list.PartListChanged(this, 
								new CollectionEventArgs(list.parts.IndexOf(this), this, this, CollectionEventType.Replace));
					}
				}
			}

			private int _subParts;
			public int SubParts
			{
				get { return _subParts; }
				set 
				{ 
					if (_subParts != value)
					{
						_subParts = value; 
						if (list != null)
							list.PartListChanged(this, 
								new CollectionEventArgs(list.parts.IndexOf(this), this, this, CollectionEventType.Replace));
					}
				}
			}

			private int _size;
			public int Size
			{
				get { return _size; }
				set 
				{ 
					if (_size != value)
					{
						_size = value; 
						if (list != null)
							list.PartListChanged(this, 
								new CollectionEventArgs(list.parts.IndexOf(this), this, this, CollectionEventType.Replace));
					}
				}
			}

			public BoxPart(string title, int size)
			{
				_title = title;
				_size = size;
				_subParts = 1;
			}

			public BoxPart(string title)
				: this(title, 0) 
			{
			}
		}

		#endregion

		#region BoxPartCollection Class

		/// <summary>
		/// BoxPartCollection inherits GeneralCollection
		/// to use BoxPart.
		/// </summary>
		public class BoxPartCollection : GeneralCollection
		{
			public BoxPart this[int index]
			{
				get { return (BoxPart) GetItem(index); }
			}

			public void Insert(int index, BoxPart value)
			{
				InsertItem(index, value);
			}

			public void Insert(int index, string title)
			{
				InsertItem(index, new BoxPart(title));
			}

			public void Insert(int index, string title, int relativeSize)
			{
				InsertItem(index, new BoxPart(title, relativeSize));
			}

			public void Remove(BoxPart value)
			{
				RemoveItem(value);
			}

			public bool Contains(BoxPart value)
			{
				return base.Contains(value);
			}

			public int IndexOf(BoxPart value)
			{
				return base.IndexOf(value);
			}

			public int Add(BoxPart value)
			{
				return AddItem(value);
			}

			public int Add(string title)
			{
				return AddItem(new BoxPart(title));
			}

			public int Add(string title, int relativeSize)
			{
				return AddItem(new BoxPart(title, relativeSize));
			}
		}

		#endregion

		#endregion

		#region Items

		#region BoxItem Class

		public class BoxItem
		{
			internal BoxList list;

			private string _title;
			public string Title
			{
				get { return _title; }
				set 
				{ 
					if (_title != value)
					{
						_title = value;
						if (list != null)
							list.ItemListChanged(this, 
								new CollectionEventArgs(list.items.IndexOf(this), this, this, CollectionEventType.Replace));
					}
				}
			}

			public override string ToString()
			{
				return _title;
			}

			private object[,] _parts;
			public object this[int part, int subpart]
			{
				get
				{
					return GetItem(part, subpart);
				}
				set
				{
					SetItem(part, subpart, value);
				}
			}

			public void SetItem(int part, int subpart, object value)
			{
				if (part < 0 || part >= _parts.GetLength(0) ||
					subpart < 0 || subpart >= _parts.GetLength(1))
					throw new ArgumentOutOfRangeException("part,subpart", "Part or subpart out of range");

				_parts[part, subpart] = value;
				if (list != null)
					list.ItemListChanged(this, 
						new CollectionEventArgs(list.items.IndexOf(this), this, this, CollectionEventType.Replace));
			}
			
			public BoxItem(string title, int parts, int subparts)
			{
				_title = title;
				_parts = new object[parts, subparts];
			}

			public string GetTitle()
			{
				return _title;
			}

			public object GetItem(int part, int subpart)
			{
				if (part >= 0 && part < _parts.GetLength(0) &&
					subpart >= 0 && subpart < _parts.GetLength(1))
				{
					return _parts[part, subpart];
				}
				return null;
			}
		}

		#endregion

		#region BoxItemCollection Class

		/// <summary>
		/// BoxItemCollection inherits GeneralCollection
		/// to use IBox.
		/// </summary>
		public class BoxItemCollection : GeneralCollection
		{
			public BoxItem this[int index]
			{
				get { return (BoxItem) GetItem(index); }
			}

			public void Insert(int index, BoxItem value)
			{
				InsertItem(index, value);
			}

			public void Remove(BoxItem value)
			{
				RemoveItem(value);
			}

			public bool Contains(BoxItem value)
			{
				return base.Contains(value);
			}

			public int IndexOf(BoxItem value)
			{
				return base.IndexOf(value);
			}

			public int Add(BoxItem value)
			{
				return AddItem(value);
			}
		}

		#endregion

		#endregion

		private BoxPartCollection parts;
		public BoxPartCollection Parts
		{
			get { return parts; }
		}

		private BoxItemCollection items;
		public BoxItemCollection Items
		{
			get { return items; }
		}

		public BoxList()
		{
			parts = new BoxPartCollection();
			parts.Changed += new CollectionEventHandler(PartListChanged);

			items = new BoxItemCollection();
			items.Changed += new CollectionEventHandler(ItemListChanged);
		}

		private void PartListChanged(object sender, CollectionEventArgs e)
		{
			if (e.Old == null)
				// New Item
				((BoxPart) e.New).list = this;
			else if (e.New == null)
				// Item removed
				((BoxPart) e.Old).list = null;

			if (ListChanged != null)
				ListChanged(this, EventArgs.Empty);
		}

		private void ItemListChanged(object sender, CollectionEventArgs e)
		{
			if (e.Old == null)
			{
				// New Item
				((BoxItem) e.New).list = this;
				if (ListChanged != null)
					ListChanged(this, EventArgs.Empty);
			}
			else if (e.New == null)
			{
				// Item removed
				((BoxItem) e.Old).list = null;
				if (ListChanged != null)
					ListChanged(this, EventArgs.Empty);
			}
			else
			{
				if (ValueChanged != null)
					ValueChanged(this, EventArgs.Empty);
			}
		}

		#region IBoxList Members

		public int GetPartCount()
		{
			return parts.Count;
		}

		public int GetSubPartCount(int part)
		{
			return parts[part].SubParts;
		}

		public string GetPartTitle(int part)
		{
			return parts[part].Title;
		}

		public int GetPartSize(int part)
		{
			return parts[part].Size;
		}

		public int GetBoxCount()
		{
			return items.Count;
		}

		public string GetBoxTitle(int box)
		{
			return items[box].Title;
		}

		public object GetBoxItem(int box, int part, int subpart)
		{
			return items[box].GetItem(part, subpart);
		}

		public event EventHandler ListChanged;
		public event EventHandler ValueChanged;

		#endregion
	}

	#endregion
}
