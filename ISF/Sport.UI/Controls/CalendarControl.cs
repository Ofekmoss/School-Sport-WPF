using System;
using System.Windows.Forms;
using System.Drawing;
using System.Globalization;

namespace Sport.UI.Controls
{
	public class Calendar : System.Collections.ICollection, 
		System.Collections.IEnumerable, System.Collections.IDictionary
	{
		public class CalendarItem
		{
			private string _text;
			public string Text
			{
				get { return _text; }
				set { _text = value; }
			}

			private StringAlignment _alignment;
			public StringAlignment Alignment
			{
				get { return _alignment; }
				set { _alignment = value; }
			}

			private Brush[] _brushes;
			public Brush[] Brushes
			{
				get { return _brushes; }
				set { _brushes = value; }
			}

			private object _tag;
			public object Tag
			{
				get { return _tag; }
				set { _tag = value; }
			}

			public CalendarItem(string text, StringAlignment alignment, object tag)
			{
				_text = text;
				_alignment = alignment;
				_tag = tag;
			}

			public CalendarItem(string text, StringAlignment alignment)
				: this(text, alignment, null)
			{
			}

			public CalendarItem(string text)
				: this(text, StringAlignment.Near)
			{
			}

			public CalendarItem()
				: this(null)
			{
			}
		}

		private System.Collections.SortedList _calendar;

		public Calendar()
		{
			_calendar = new System.Collections.SortedList();
		}

		public void SetItem(DateTime dateTime, CalendarItem value)
		{
			_calendar[dateTime.Date] = value;
		}

		public CalendarItem GetItem(DateTime dateTime)
		{
			return (CalendarItem) _calendar[dateTime.Date];
		}

		public CalendarItem this[DateTime key]
		{
			get
			{
				return GetItem(key);
			}
			set
			{
				SetItem(key, value);
			}
		}

		public void Remove(DateTime key)
		{
			_calendar.Remove(key.Date);
		}

		public bool Contains(DateTime key)
		{
			return _calendar.Contains(key.Date);
		}

		public void Add(DateTime key, CalendarItem value)
		{
			_calendar.Add(key.Date, value);
		}

		public void Add(DateTime key, string text)
		{
			_calendar.Add(key.Date, new CalendarItem(text));
		}

		public void Add(DateTime key, string text, StringAlignment alignment)
		{
			_calendar.Add(key.Date, new CalendarItem(text, alignment));
		}

		public DateTime GetKey(int index)
		{
			return (DateTime) _calendar.GetKey(index);
		}

		public CalendarItem GetValue(int index)
		{
			return (CalendarItem) _calendar.GetByIndex(index);
		}

		public void RemoveAt(int index)
		{
			_calendar.RemoveAt(index);
		}

		#region ICollection Members

		public bool IsSynchronized
		{
			get { return _calendar.IsSynchronized; }
		}

		public int Count
		{
			get { return _calendar.Count; }
		}

		public void CopyTo(Array array, int index)
		{
			_calendar.CopyTo(array, index);
		}

		public object SyncRoot
		{
			get { return _calendar.SyncRoot; }
		}

		#endregion

		#region IEnumerable Members

		public System.Collections.IEnumerator GetEnumerator()
		{
			return _calendar.GetEnumerator();
		}

		#endregion

		#region IDictionary Members

		public bool IsReadOnly
		{
			get { return _calendar.IsReadOnly; }
		}

		System.Collections.IDictionaryEnumerator System.Collections.IDictionary.GetEnumerator()
		{
			return _calendar.GetEnumerator();
		}

		object System.Collections.IDictionary.this[object key]
		{
			get
			{
				if (!(key is DateTime))
					throw new ArgumentException("Key must be DateTime");
				return _calendar[((DateTime)key).Date];
			}
			set
			{
				if (!(key is DateTime))
					throw new ArgumentException("Key must be DateTime");
				if (!(value is CalendarItem))
					throw new ArgumentException("Value must be CalendarItem");
				_calendar[((DateTime)key).Date] = value;
			}
		}

		void System.Collections.IDictionary.Remove(object key)
		{
			if (!(key is DateTime))
				throw new ArgumentException("Key must be DateTime");
			_calendar.Remove(((DateTime)key).Date);
		}

		bool System.Collections.IDictionary.Contains(object key)
		{
			if (!(key is DateTime))
				throw new ArgumentException("Key must be DateTime");
			return _calendar.Contains(((DateTime)key).Date);
		}

		public void Clear()
		{
			_calendar.Clear();
		}

		public System.Collections.ICollection Values
		{
			get { return _calendar.Values; }
		}

		void System.Collections.IDictionary.Add(object key, object value)
		{
			if (!(key is DateTime))
				throw new ArgumentException("Key must be DateTime");
			if (!(value is CalendarItem))
				throw new ArgumentException("Value must be CalendarItem");
			_calendar.Add(((DateTime)key).Date, value);
		}

		public System.Collections.ICollection Keys
		{
			get { return _calendar.Keys; }
		}

		public bool IsFixedSize
		{
			get { return _calendar.IsFixedSize; }
		}

		#endregion
	}

	public class CalendarControl : ControlWrapper
	{
		private VScrollBar scroll;
		private int scrollPosition;

		private Calendar _items;
		public Calendar Items
		{
			get { return _items; }
		}

		private System.Globalization.DateTimeFormatInfo formatInfo;

		public System.Globalization.Calendar Calendar
		{
			get
			{
				return formatInfo.Calendar;
			}
			set
			{
				formatInfo.Calendar = value;
				ResetSize();
				Refresh();
			}
		}

		private System.Globalization.CultureInfo _currentInfo;
		public System.Globalization.CultureInfo CurrentInfo
		{
			get
			{
				return _currentInfo;
			}
			set
			{
				if (_currentInfo.LCID != value.LCID)
					_currentInfo = new CultureInfo(value.LCID);
				formatInfo = _currentInfo.DateTimeFormat;
				ResetSize();
			}
		}

		public CalendarControl()
		{
			_currentInfo = new CultureInfo(CultureInfo.CurrentCulture.LCID);
			formatInfo = _currentInfo.DateTimeFormat;

			scroll = new VScrollBar();
			scroll.Minimum = 0;
			scroll.SmallChange = 1;
			scroll.Scroll += new ScrollEventHandler(ScrollScroll);
			Controls.Add(scroll);

			int year = formatInfo.Calendar.GetYear(DateTime.Now);

			_start = new DateTime(year, 1, 1, formatInfo.Calendar);
			_end = new DateTime(year + 1, 1, 1, formatInfo.Calendar).AddDays(-1);
			_selected = DateTime.MinValue;

			_items = new Calendar();

			_yearHeaderBrush = new SolidBrush(Color.LightGoldenrodYellow);

			_headerBrushes = new Brush[]
				{
					new SolidBrush(Color.LightGreen),
					new SolidBrush(Color.LimeGreen)
				};

			ResetScroll();
		}

		private void ResetDateRange()
		{
			int from = 0;
			int to = _items.Count - 1;
			for (int n = 0; n < _items.Count; n++)
			{
				DateTime dt = _items.GetKey(n);
				if (dt < _start)
					from = n + 1;
				if (dt > _end)
					to = n - 1;
			}

			if (from > to)
			{
				_items.Clear();
			}
			else
			{
				while (to > _items.Count - 1)
				{
					_items.RemoveAt(to + 1);
				}
				
				while (from > 0)
				{
					_items.RemoveAt(0);
					from--;
				}
			}
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize (e);

			ResetSize();
		}

		protected override void OnRightToLeftChanged(EventArgs e)
		{
			base.OnRightToLeftChanged (e);

			ResetSize();
		}

		protected override void OnFontChanged(EventArgs e)
		{
			base.OnFontChanged (e);

			_yearFont = new Font(Font.FontFamily, Font.Size, Font.Style | System.Drawing.FontStyle.Bold);
		}


		private int BorderSize
		{
			get
			{
				int border = 0;
				if (_borderStyle == BorderStyle.Fixed3D)
					border = 2;
				else if (_borderStyle == BorderStyle.FixedSingle)
					border = 1;
				return border;
			}
		}

		private void ResetSize()
		{
			int border = BorderSize;

			scroll.Height = Height - (border * 2);
			scroll.Top = border;
			if (RightToLeft == RightToLeft.Yes)
			{
				scroll.Left = border;
			}
			else
			{
				scroll.Left = Width - scroll.Width - border;
			}

			ResetScroll();

			if (bitMap != null)
				bitMap.Dispose();
			bitMap = null;

			ResetYearSizes();

			int monthHeight = (Height - (BorderSize * 2) - Font.Height + 4) / _monthes;
			if (monthHeight > 4 && monthHeight < Font.Height * 2)
				_itemFont = new Font(Font.FontFamily, (monthHeight / 2) - 1, System.Drawing.GraphicsUnit.Pixel);
			else
				_itemFont = Font;

			Refresh();
		}

		private int _titleWidth;
		private int _daysInMonth;
		private int _monthes;
		private void ResetYearSizes()
		{
			Graphics g = CreateGraphics();

			DateTime last = Calendar.AddYears(_start, 1);

			string title;
			if (_start.Month == 1)
				title = _start.ToString("yyyy", formatInfo);
			else
				title = _start.ToString("yyyy", formatInfo) + "-" + last.ToString("yyyy", formatInfo);

			_monthes = Calendar.GetMonthsInYear(Calendar.GetYear(_start));

			g.TextContrast = 0;
			g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

			if (_yearFont == null)
				_yearFont = new Font(Font.FontFamily, Font.Size, Font.Style | System.Drawing.FontStyle.Bold);

			_titleWidth = (int) g.MeasureString(title, _yearFont).Width + 2;
			for (int m = 0; m < _monthes; m++)
			{
				DateTime d = Calendar.AddMonths(_start, m);
				int mw = (int) g.MeasureString(d.ToString("MMMM", formatInfo), Font).Width + 4;
				if (mw > _titleWidth)
					_titleWidth = mw;
			}

			int width = Width - (BorderSize * 2);
			int dm;
			_daysInMonth = 0;
			DateTime cur = _start;
			while (cur < last)
			{
				dm = Calendar.GetDaysInMonth(Calendar.GetYear(cur), Calendar.GetMonth(cur));
				if (dm > _daysInMonth)
					_daysInMonth = dm;
				cur = Calendar.AddMonths(cur, 1);
			}
			_daysInMonth += 6;
			
			int dayWidth = (width - _titleWidth) / _daysInMonth;
			_titleWidth = width - (dayWidth * _daysInMonth);

			g.Dispose();
		}

		private void ResetScroll()
		{
			if (_style == CalendarStyle.List)
			{
				TimeSpan span = _end - _start;

				int border = BorderSize;
				int rows = (Height - border) / _rowHeight;
				if (rows == 0)
					rows = 1;
				scroll.Maximum = span.Days;
				scroll.LargeChange = rows;

				scroll.Visible = scroll.Maximum > rows;
				scrollPosition = scroll.Value;
			}
			else
			{
				scroll.Visible = false;
			}
		}

		public void ScrollTo(DateTime date)
		{
			if (date >= _start && date <= _end)
			{
				TimeSpan span = date - _start;
				scrollPosition = span.Days - (scroll.LargeChange / 2);
				if (scrollPosition > scroll.Maximum - scroll.LargeChange + 1)
					scrollPosition = scroll.Maximum - scroll.LargeChange + 1;
				if (scrollPosition < 0)
					scrollPosition = 0;
				scroll.Value = scrollPosition;
				Refresh();
			}
		}

		private BorderStyle _borderStyle;
		public BorderStyle BorderStyle
		{
			get { return _borderStyle; }
			set
			{
				_borderStyle = value;
				ResetSize();
			}
		}

		private int _rowHeight = 20;
		public int RowHeight
		{
			get { return _rowHeight; }
			set 
			{ 
				_rowHeight = value;
				ResetSize();
			}
		}

		private Brush _emptyBrush = SystemBrushes.Control;
		public Brush EmptyBrush
		{
			get { return _emptyBrush; }
			set
			{
				_emptyBrush = value;
				Refresh();
			}
		}

		private Brush _fullBrush = SystemBrushes.Info;
		public Brush FullBrush
		{
			get { return _fullBrush; }
			set
			{
				_fullBrush = value;
				Refresh();
			}
		}

		private Font _yearFont;
		private Font _itemFont;
		private Font _dayFont = new System.Drawing.Font("Tahoma", 7F, System.Drawing.GraphicsUnit.Pixel);

		private Pen _todayPen = new Pen(Color.Red);
		public Color TodayColor
		{
			get { return _todayPen.Color; }
			set { _todayPen.Color = value; }
		}

		private Pen _selectedPen = new Pen(Color.Blue);
		public Color SelectedColor
		{
			get { return _selectedPen.Color; }
			set { _selectedPen.Color = value; }
		}

		private DateTime _start;
		public DateTime Start
		{
			get { return _start; }
			set 
			{ 
				_start = value.Date;
				if (_start > _end)
					_end = _start;
				ResetDateRange();
				ResetScroll();
			}
		}

		private DateTime _end;
		public DateTime End
		{
			get { return _end; }
			set 
			{ 
				_end = value.Date;
				if (_end < _start)
					_start = _end;
				ResetDateRange();
				ResetScroll();
			}
		}

		private DateTime _selected;
		public DateTime Selected
		{
			get { return _selected; }
			set
			{
				if (Sport.Common.Tools.IsMinDate(value))
				{
					_selected = value;
				}
				else
				{
					if (value.Date < _start || value.Date > _end)
						throw new ArgumentOutOfRangeException("value", "Selection must be between start and end dates");

					_selected = value.Date;
				}

				Refresh();
			}
		}

		private Brush[] _headerBrushes;
		public Brush[] HeaderBrushes
		{
			get { return _headerBrushes; }
			set
			{
				_headerBrushes = value;
				Refresh();
			}
		}

		private Brush _yearHeaderBrush;
		public Brush YearHeaderBrush
		{
			get { return _yearHeaderBrush; }
			set
			{
				_yearHeaderBrush = value;
				Refresh();
			}
		}

		public enum CalendarStyle
		{
			List,
			Year
		}

		private CalendarStyle _style;
		public CalendarStyle Style
		{
			get { return _style; }
			set 
			{ 
				_style = value;
				ResetSize();
			}
		}


		protected override void OnPaintBackground(PaintEventArgs pevent)
		{
			//			base.OnPaintBackground (pevent);
		}


		private Bitmap bitMap = null;

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint (e);

			int border = BorderSize;

			if (Width <= border * 2 || Height <= border * 2)
				return ;

			int left = border;
			int top = border;
			int width = Width - (border * 2);
			int height = Height - (border * 2);
			if (scroll.Visible)
			{
				width -= scroll.Width;
				if (RightToLeft == RightToLeft.Yes)
					left += scroll.Width;
			}

			if (_borderStyle == BorderStyle.Fixed3D)
			{
				int right = Width - 1;
				int bottom = Height - 1;
				e.Graphics.DrawLine(SystemPens.ControlDark,
					new Point(0, 0), new Point(right - 1, 0));
				e.Graphics.DrawLine(SystemPens.ControlDark,
					new Point(0, 0), new Point(0, bottom - 1));
				e.Graphics.DrawLine(SystemPens.ControlDarkDark,
					new Point(1, 1), new Point(right - 2, 1));
				e.Graphics.DrawLine(SystemPens.ControlDarkDark,
					new Point(1, 1), new Point(1, bottom - 2));
				e.Graphics.DrawLine(SystemPens.ControlLightLight,
					new Point(right, 0), new Point(right, bottom));
				e.Graphics.DrawLine(SystemPens.ControlLightLight,
					new Point(0, bottom), new Point(right, bottom));
				e.Graphics.DrawLine(SystemPens.ControlLight,
					new Point(right - 1, 1), new Point(right - 1, bottom - 1));
				e.Graphics.DrawLine(SystemPens.ControlLight,
					new Point(1, bottom - 1), new Point(right - 1, bottom - 1));
			}
			else if (_borderStyle == BorderStyle.FixedSingle)
			{
				e.Graphics.DrawRectangle(SystemPens.ControlDarkDark,
					0, 0, Width - 1, Height - 1);
			}

			Size size = new Size(width, height);

			if (bitMap == null)
				bitMap = new Bitmap(size.Width, size.Height, e.Graphics);
			Graphics g = Graphics.FromImage(bitMap);

			if (_style == CalendarStyle.List)
			{
				DrawListCalendar(g, size);
			}
			else if (_style == CalendarStyle.Year)
			{
				DrawYearCalendar(g, size);
			}

			e.Graphics.DrawImage(bitMap, new Point(left, top));
		}

		private void DrawListCalendar(Graphics g, Size size)
		{
			DateTime next;
			DateTime current = _start.AddDays(scrollPosition);

			StringFormat sf = new StringFormat();
			StringFormat msf = new StringFormat();
			msf.LineAlignment = StringAlignment.Center;
			msf.Alignment = StringAlignment.Center;
			msf.FormatFlags = StringFormatFlags.NoWrap;
			StringFormat dsf = new StringFormat();
			dsf.LineAlignment = StringAlignment.Center;
			dsf.Alignment = StringAlignment.Center;

			//int days = (size.Height / _rowHeight) + 1;
			int dayWidth = 0;
			int dw;
			DateTime d = current;
			for (int n = 0; n < 100; n++)
			{
				dw = (int) g.MeasureString(d.ToString("dd", formatInfo), Font).Width;
				if (dayWidth < dw)
					dayWidth = dw;
				d = d.AddDays(1);
			}
			dayWidth += 4;
			int monthWidth = Font.Height + 4;

			int width = size.Width - dayWidth - monthWidth - 1;
			int left = 0;
			int dayLeft;
			int monthLeft;
			if (RightToLeft == RightToLeft.Yes)
			{
				sf.FormatFlags = StringFormatFlags.DirectionRightToLeft;
				msf.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
				dsf.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
				dayLeft = left + width;
				monthLeft = dayLeft + dayWidth;
			}
			else
			{
				msf.FormatFlags |= StringFormatFlags.DirectionVertical;
				left += dayWidth + monthWidth;
				dayLeft = left - dayWidth;
				monthLeft = dayLeft - monthWidth;
			}

			int top = 0;
			int monthTop = top;
			int lastMonth = current.Month;
			int height = _rowHeight;
			int brushCount = _headerBrushes == null ? 0 : _headerBrushes.Length;
			int sy = Calendar.GetYear(_start);
			int cy = Calendar.GetYear(current);
			int currentMonth = 0;
			currentMonth += Calendar.GetMonth(current) - Calendar.GetMonth(_start);
			if (sy < cy)
			{
				currentMonth += Calendar.GetMonthsInYear(sy);
			}
			for (int i = sy; i < cy - 1; i++)
				currentMonth += Calendar.GetMonthsInYear(i);

			Brush monthBrush = SystemBrushes.Window;

			if (brushCount > 0)
			{
				currentMonth = currentMonth % brushCount;
				monthBrush = _headerBrushes[currentMonth];
			}


			while (top < size.Height)
			{
				if (top + height >= size.Height)
					height = size.Height - top - 1;

				Calendar.CalendarItem item = _items[current.Date];

				if (item != null)
				{
					sf.Alignment = item.Alignment;
					g.FillRectangle(_fullBrush,
						left, top, width, height);
					g.DrawString(item.Text, Font, SystemBrushes.ControlText, 
						new Rectangle(left, top, width, _rowHeight), sf);
				}
				else
				{
					g.FillRectangle(_emptyBrush,
						left, top, width, _rowHeight);
				}
				g.DrawRectangle(SystemPens.WindowFrame,
					left, top, width, height);

				g.FillRectangle(monthBrush,
					dayLeft, top, dayWidth, _rowHeight);
				g.DrawString(current.ToString("dd", formatInfo), Font,
					SystemBrushes.ControlText, 
					new Rectangle(dayLeft, top, dayWidth, _rowHeight), dsf);
				g.DrawRectangle(SystemPens.WindowFrame,
					dayLeft, top, dayWidth, height);

				next = current.AddDays(1);
				int month = Calendar.GetMonth(current);
				int nextMonth = Calendar.GetMonth(next);

				if (nextMonth != month || top + _rowHeight >= size.Height)
				{
					int monthHeight = top + height - monthTop;
					g.FillRectangle(monthBrush, monthLeft, monthTop, monthWidth, monthHeight);
					g.DrawRectangle(SystemPens.WindowFrame, monthLeft, monthTop, monthWidth, monthHeight);

					if (RightToLeft == RightToLeft.Yes)
					{
						g.RotateTransform(-90);

						g.DrawString(current.ToString("MMMM", formatInfo), Font,
							SystemBrushes.ControlText, 
							new Rectangle(-monthTop - monthHeight, monthLeft, monthHeight, monthWidth),
							msf);

						g.ResetTransform();
					}
					else
					{
						g.DrawString(current.ToString("MMMM", formatInfo), Font,
							SystemBrushes.ControlText,
							new Rectangle(monthLeft, monthTop, monthWidth, monthHeight),
							msf);
					}

					monthTop += monthHeight;

					if (brushCount > 0)
					{
						currentMonth++;
						if (currentMonth >= brushCount)
							currentMonth = 0;
						monthBrush = _headerBrushes[currentMonth];
					}
				}

				if (current == _selected)
				{
					g.DrawRectangle(_selectedPen,
						left + 1, top + 1, width - 2, height - 2);
				}
				else if (current == DateTime.Now.Date)
				{
					g.DrawRectangle(_todayPen,
						left + 1, top + 1, width - 2, height - 2);
				}
				
				top += _rowHeight;
				current = next;
			}

		}

		private void DrawYearCalendar(Graphics g, Size size)
		{
			g.FillRectangle(SystemBrushes.Control, 0, 0, size.Width, size.Height);

			g.TextContrast = 0;
			g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

			StringFormat sf = new StringFormat();
			sf.LineAlignment = StringAlignment.Center;
			StringFormat msf = new StringFormat();
			msf.LineAlignment = StringAlignment.Center;
			msf.FormatFlags = StringFormatFlags.NoWrap;
			StringFormat dsf = new StringFormat();
			dsf.FormatFlags = StringFormatFlags.NoWrap;
			dsf.LineAlignment = StringAlignment.Center;
			dsf.Alignment = StringAlignment.Center;

			if (RightToLeft == RightToLeft.Yes)
			{
				sf.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
				msf.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
				dsf.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
			}

			DateTime last = Calendar.AddYears(_start, 1);
			string title;
			if (_start.Month == 1)
				title = _start.ToString("yyyy", formatInfo);
			else
				title = _start.ToString("yyyy", formatInfo) + "-" + last.ToString("yyyy", formatInfo);

			int dayWidth = (size.Width - _titleWidth) / _daysInMonth;

			int titleHeight = Font.Height + 4;
			int monthHeight = (size.Height - titleHeight) / _monthes;
			titleHeight = size.Height - (monthHeight * _monthes) - 1;
			int titleLeft = 0;

			if (RightToLeft == RightToLeft.Yes)
			{
				sf.FormatFlags = StringFormatFlags.DirectionRightToLeft;
				msf.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
				titleLeft = size.Width - _titleWidth;
			}

			g.FillRectangle(_yearHeaderBrush, 0, 1, size.Width, titleHeight);


			if (_yearFont == null)
				_yearFont = new Font(Font.FontFamily, Font.Size, Font.Style | System.Drawing.FontStyle.Bold);

			g.DrawString(title, _yearFont, SystemBrushes.ControlText,
				new Rectangle(titleLeft, 0, _titleWidth, titleHeight), dsf);
			int top = titleHeight;
			DateTime cur = _start;
			int brushCount = _headerBrushes == null ? 0 : _headerBrushes.Length;
			Brush monthBrush = brushCount > 0 ? _headerBrushes[0] : SystemBrushes.Highlight;
			int currentBrush = 0;
			for (int m = 0; m < _monthes; m++)
			{
				g.FillRectangle(monthBrush, 0, top, size.Width, monthHeight);
				if (brushCount > 0)
				{
					currentBrush++;
					if (currentBrush >= brushCount)
						currentBrush = 0;
					monthBrush = _headerBrushes[currentBrush];
				}
				g.DrawString(cur.ToString("MMMM", formatInfo), Font,
					SystemBrushes.ControlText, new Rectangle(titleLeft, top, _titleWidth, monthHeight), msf);
				cur = Calendar.AddMonths(cur, 1);
				top += monthHeight;
			}
			int move = dayWidth;
			int left = 0;
			if (RightToLeft != RightToLeft.Yes)
			{
				left = size.Width - dayWidth - 1;
				move = -move;
			}

			for (int d = 0; d < _daysInMonth; d++)
			{
				g.DrawString(formatInfo.GetAbbreviatedDayName((System.DayOfWeek) (d % 7)), 
					Font, SystemBrushes.ControlText, 
					new Rectangle(left, 0, dayWidth, titleHeight), dsf);
				g.DrawRectangle(SystemPens.WindowFrame, left, 0, dayWidth, titleHeight);
				left += move;
			}

			Color backColor = (_emptyBrush as SolidBrush).Color;
			Color foreColor = Color.FromArgb(255 - backColor.R,
				255 - backColor.G, 255 - backColor.B);

			Brush outBrush = new System.Drawing.Drawing2D.HatchBrush(
				System.Drawing.Drawing2D.HatchStyle.BackwardDiagonal,
				foreColor, backColor);

			DateTime current = new DateTime(
				Calendar.GetYear(_start), Calendar.GetMonth(_start), 1, Calendar);

			brushCount = _headerBrushes == null ? 0 : _headerBrushes.Length;
			monthBrush = brushCount > 0 ? _headerBrushes[0] : SystemBrushes.Highlight;
			currentBrush = 0;

			int dayHeight = _dayFont.Height;
			int itemHeight = monthHeight - dayHeight;
			top = titleHeight;
			for (int m = 0; m < _monthes; m++)
			{
				left = (int) current.DayOfWeek * dayWidth;
				if (RightToLeft != RightToLeft.Yes)
				{
					left = size.Width - left - dayWidth - 1;
				}

				int month = Calendar.GetMonth(current);
				while (Calendar.GetMonth(current) == month)
				{
					if (current < _start || current > _end)
					{
						g.FillRectangle(outBrush, left, top, dayWidth, monthHeight);
					}
					else
					{
						Calendar.CalendarItem item = _items[current];

						if (item != null)
						{
							sf.Alignment = item.Alignment;
							if (item.Brushes != null)
							{
								int bh = itemHeight / item.Brushes.Length;
								int t = top + dayHeight;
								for (int b = 0; b < item.Brushes.Length - 1; b++)
								{
									g.FillRectangle(item.Brushes[b], left, t, dayWidth, bh);
									t += bh;
								}

								g.FillRectangle(item.Brushes[item.Brushes.Length - 1], left, t, dayWidth, 
									itemHeight - (bh * (item.Brushes.Length - 1)));
							}
							else
							{
								g.FillRectangle(_fullBrush, left, top + dayHeight, dayWidth, itemHeight);
							}

							if (item.Text != null)
							{
								g.DrawString(item.Text, _itemFont, SystemBrushes.ControlText, 
									new Rectangle(left + 1, top + 1 + dayHeight, dayWidth - 2, itemHeight - 2), sf);
							}
						}
						else
						{
							g.FillRectangle(_emptyBrush, left, top, dayWidth, monthHeight);
						}
					}

					g.FillRectangle(monthBrush, left, top, dayWidth, dayHeight);
					g.DrawLine(SystemPens.WindowFrame, left, top + dayHeight, left + dayWidth, top + dayHeight);

					g.DrawString(current.ToString("dd", formatInfo), _dayFont, SystemBrushes.ControlText,
						new Rectangle(left, top, dayWidth, dayHeight), dsf);

					g.DrawRectangle(SystemPens.WindowFrame, left, top, dayWidth, monthHeight);

					if (current == _selected)
					{
						g.DrawRectangle(_selectedPen,
							left + 1, top + 1, dayWidth - 2, monthHeight - 2);
					}
					else if (current == DateTime.Now.Date)
					{
						g.DrawRectangle(_todayPen,
							left + 1, top + 1, dayWidth - 2, monthHeight - 2);
					}

					left += move;
					current = current.AddDays(1);
				}

				if (brushCount > 0)
				{
					currentBrush++;
					if (currentBrush >= brushCount)
						currentBrush = 0;
					monthBrush = _headerBrushes[currentBrush];
				}

				top += monthHeight;
			}
		}

		private void ScrollScroll(object sender, ScrollEventArgs e)
		{
			if (e.Type != ScrollEventType.EndScroll)
			{
				scrollPosition = e.NewValue;
				Refresh();
			}
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			base.OnMouseWheel (e);

			int d = e.Delta / 120;

			scrollPosition += -d;
			if (scrollPosition > scroll.Maximum - scroll.LargeChange + 1)
				scrollPosition = scroll.Maximum - scroll.LargeChange + 1;
			if (scrollPosition < 0)
				scrollPosition = 0;
			scroll.Value = scrollPosition;
			Refresh();
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown (e);

			if (!Focused)
				Focus();

			if (e.Button == MouseButtons.Left)
			{
				if (_style == CalendarStyle.List)
				{
					int border = BorderSize;
					if (e.X > border && e.Y > border && e.X < Width - border &&
						e.Y < Height - border)
					{
						int d = e.Y - border;
						int row = d / _rowHeight;
						DateTime target = _start.AddDays(scrollPosition + row);
						if (target <= _end)
						{
							if ((row + 1) * _rowHeight > Height - border)
							{
								scrollPosition++;
								scroll.Value = scrollPosition;
							}

							Selected = target;
						}
					}
				}
				else if (_style == CalendarStyle.Year)
				{
					int border = BorderSize;
					int width = Width - (border * 2);
					int height = Height - (border * 2);
					int dayWidth = (width - _titleWidth) / _daysInMonth;
					
					int titleHeight = Font.Height + 4;
					int monthHeight = (height - titleHeight) / _monthes;
					titleHeight = height - (monthHeight * _monthes) - 1;

					DateTime target = new DateTime(
						Calendar.GetYear(_start), Calendar.GetMonth(_start), 1,
						Calendar);
					
					if (e.Y > border + titleHeight && e.Y < Height - border)
					{
						target = Calendar.AddMonths(target, (e.Y - titleHeight - border) / monthHeight);
						int day;
						if (RightToLeft == RightToLeft.Yes)
						{
							day = (e.X - border) / dayWidth;
						}
						else
						{
							day = (Width - border - e.X) / dayWidth;
						}
						int month = Calendar.GetMonth(target);
						target = target.AddDays(day - (int) target.DayOfWeek);
						if (Calendar.GetMonth(target) == month && target >= _start &&
							target <= _end)
							Selected = target;
					}
				}
			}
		}

		public System.Drawing.Imaging.Metafile CreateMetafile(int width, int height)
		{
			Graphics g = CreateGraphics();

			System.IntPtr hdc = g.GetHdc();

			System.Drawing.Imaging.Metafile metafile = new System.Drawing.Imaging.Metafile(hdc, 
				new Rectangle(0, 0, width, height),
				System.Drawing.Imaging.MetafileFrameUnit.Pixel, System.Drawing.Imaging.EmfType.EmfPlusOnly);

			g.ReleaseHdc(hdc);

			Graphics graphics = Graphics.FromImage(metafile);

			DrawYearCalendar(graphics, new Size(width, height));

			graphics.Dispose();

			return metafile;
		}
	}
}
