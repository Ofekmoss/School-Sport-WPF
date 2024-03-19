using System;
using Sport.Common;
using System.ComponentModel;
using System.Windows.Forms;

namespace Sport.UI.Controls
{
	/// <summary>
	/// Summary description for LegendControl.
	/// </summary>
	public class LegendControl : Control
	{
		#region Collection

		#region LegendItem Class

		public sealed class LegendItem : GeneralCollection.CollectionItem
		{
			private object _value;

			[DefaultValue((string) null), Bindable(true), TypeConverter(typeof(StringConverter)), Localizable(false)]
			public object Value
			{
				get { return _value; }
				set 
				{
					if (_value != value)
					{
						_value = value;
						if (Owner != null)
						{
							((LegendControl) Owner).RefreshLegend();
						}
					}
				}
			}

			private System.Drawing.Brush _brush;
			public System.Drawing.Brush Brush
			{
				get { return _brush; }
				set
				{
					if (_brush != value)
					{
						_brush = value;
						if (Owner != null)
						{
							((LegendControl) Owner).RefreshLegend();
						}
					}
				}
			}

			public LegendItem(object value, System.Drawing.Brush brush)
			{
				_value = value;
				_brush = brush;
			}

			public LegendItem(object value)
				: this(value, null)
			{
			}

			public LegendItem()
			{
			}
		}

		#endregion

		#region LegendItemCollection Class

		/// <summary>
		/// LegendItemCollection inherits GeneralCollection to
		/// use LegendItem.
		/// </summary>
		public class LegendItemCollection : GeneralCollection
		{
			public LegendItem this[int index]
			{
				get { return (LegendItem) GetItem(index); }
			}

			public void Insert(int index, LegendItem value)
			{
				InsertItem(index, value);
			}

			public void Insert(int index, string value)
			{
				InsertItem(index, new LegendItem(value));
			}

			public void Remove(LegendItem value)
			{
				RemoveItem(value);
			}

			public void Remove(object value)
			{
				foreach (LegendItem ci in this)
				{
					if (ci.Value == value)
					{
						RemoveItem(ci);
						return ;
					}
				}
			}

			public bool Contains(LegendItem value)
			{
				return base.Contains(value);
			}

			public int IndexOf(LegendItem value)
			{
				return base.IndexOf(value);
			}

			public int Add(LegendItem value)
			{
				return AddItem(value);
			}

			public int Add(object value, System.Drawing.Brush brush)
			{
				return Add(new LegendItem(value, brush));
			}

			public int Add(object value)
			{
				return Add(new LegendItem(value));
			}

			public LegendItemCollection(LegendControl owner)
				: base(owner)
			{
			}
		}

		#endregion

		private LegendItemCollection items;

		public LegendItemCollection Items
		{
			get { return items; }
		}

		private void ItemChanged(object sender, CollectionEventArgs e)
		{
			RefreshLegend();
		}

		#endregion

		#region Properties

		private LegendControlDirection _direction;
		public LegendControlDirection Direction
		{
			get { return _direction; }
			set
			{
				if (_direction != value)
				{
					_direction = value;

					RefreshLegend();
				}
			}
		}

		private BorderStyle _borderStyle;
		public BorderStyle BorderStyle
		{
			get { return _borderStyle; }
			set
			{
				if (_borderStyle != value)
				{
					_borderStyle = value;
					
					RefreshLegend();
				}
			}
		}

		#endregion

		public LegendControl()
		{
			items = new LegendItemCollection(this);
			items.Changed += new CollectionEventHandler(ItemChanged);

			scrollVisible = false;

			RefreshLegend();
		}

		public System.Drawing.Imaging.Metafile CreateMetafile(int width, int height)
		{
			System.Drawing.Graphics g = CreateGraphics();

			System.IntPtr hdc = g.GetHdc();

			System.Drawing.Imaging.Metafile metafile = new System.Drawing.Imaging.Metafile(hdc, 
				new System.Drawing.Rectangle(0, 0, width, height),
				System.Drawing.Imaging.MetafileFrameUnit.Pixel, System.Drawing.Imaging.EmfType.EmfPlusOnly);

			g.ReleaseHdc(hdc);

			System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(metafile);

			PaintLegend(graphics, new System.Drawing.Rectangle(0, 0, width, height));

			graphics.Dispose();

			return metafile;
		}

		#region Paint Sizing

		private int rowHeight;
		private int boxWidth;
		private int itemExtraWidth;
		private int rows;
		private int[] columnsWidth;
		private System.Drawing.Rectangle paintRectangle;

		private void RefreshPaintRectangle()
		{
			paintRectangle = new System.Drawing.Rectangle(0, 0, Width, Height);

			if (_borderStyle == BorderStyle.FixedSingle)
			{
				paintRectangle.Offset(1, 1);
				paintRectangle.Inflate(-2, -2);
			}
			else if (_borderStyle == BorderStyle.Fixed3D)
			{
				paintRectangle.Offset(2, 2);
				paintRectangle.Inflate(-4, -4);
			}

			if (scrollVisible)
			{
				if (scroll is VScrollBar)
				{
					paintRectangle.Width -= scroll.Width;
					if (RightToLeft == RightToLeft.Yes)
						paintRectangle.X += scroll.Width;
				}
				else if (scroll is HScrollBar)
				{
					paintRectangle.Height -= scroll.Height;
				}
			}
		}

		private void SetScroll(bool visible)
		{
			if (!visible)
			{
				if (scroll != null)
					scroll.Visible = false;
				scrollVisible = false;
			}
			else
			{
				scrollPosition = 0;
				scrollVisible = true;
				if (_direction == LegendControlDirection.Horizontal)
				{
					if (scroll is HScrollBar)
					{
						scroll.Visible = true;
						return ;
					}
					else if (scroll != null)
					{
						scroll.Scroll -= new ScrollEventHandler(ScrollScroll);
						Controls.Remove(scroll);
					}
					scroll = new HScrollBar();
					Controls.Add(scroll);
				}
				else
				{
					if (scroll is VScrollBar)
					{
						scroll.Visible = true;
						return ;
					}
					else if (scroll != null)
					{
						scroll.Scroll -= new ScrollEventHandler(ScrollScroll);
						Controls.Remove(scroll);
					}
					scroll = new VScrollBar();
					Controls.Add(scroll);
				}

				scroll.Scroll += new ScrollEventHandler(ScrollScroll);
			}
		}

		private void RefreshLegend()
		{
			SetScroll(false);
			rowHeight = Font.Height;
			boxWidth = Font.Height * 2;
			itemExtraWidth = boxWidth + 16;

			if (_direction == LegendControlDirection.Horizontal)
			{
				RefreshHorizontalLegend();
			}
			else
			{
				RefreshVerticalLegend();
			}

			Refresh();
		}

		private void RefreshHorizontalLegend()
		{
			System.Drawing.Graphics g = CreateGraphics();

			RefreshPaintRectangle();
			rows = paintRectangle.Height / rowHeight;

			if (rows <= 0)
			{
				rows = 0;
				return ;
			}

			int r;
			int columns = System.Math.DivRem(items.Count, rows, out r);
			if (r != 0)
				columns++;
			columnsWidth = new int[columns];

			int space = paintRectangle.Width;
			int width;
			int c = 0;
			r = 0;
			foreach (LegendItem item in items)
			{
				width = itemExtraWidth;
				if (item.Value != null)
					width += (int) g.MeasureString(item.Value.ToString(), Font).Width;

				if (width > columnsWidth[c])
				{
					columnsWidth[c] = width;
				}
	
				r++;
				if (r == rows)
				{
					if (columnsWidth[c] > paintRectangle.Width)
						columnsWidth[c] = paintRectangle.Width;

					space -= columnsWidth[c];

					c++;
					r = 0;
				}
			}

			g.Dispose();

			if (r < rows && c < columnsWidth.Length)
				space -= columnsWidth[c];

			// Reseting to scroll refresh
			if (space < 0 && !scrollVisible)
			{
				SetScroll(true);
				RefreshHorizontalLegend();
				return ;
			}



			if (scrollVisible)
			{
				scroll.Maximum = columnsWidth.Length - 1;
				scroll.Minimum = 0;
				if (scrollPosition != scroll.Value)
				{
					scrollPosition = scroll.Value;
				}
				scroll.LargeChange = 1;
			}
		}

		private void RefreshVerticalLegend()
		{
			System.Drawing.Graphics g = CreateGraphics();

			RefreshPaintRectangle();

			int maxColumns = 0;
			int space = paintRectangle.Width;
			LegendItem item;

			for (int n = 0; n < items.Count && space > 0; n++)
			{
				item = items[n];

				space -= itemExtraWidth;
				if (item.Value != null)
					space -= (int) g.MeasureString(item.Value.ToString(), Font).Width;
				maxColumns++;
			}

			if (space < 0)
				maxColumns--;

			int c;
			int w;
			int width = 0;
			int columns = 1;
			if (maxColumns > columns)
			{
				bool found = false;
				columns = maxColumns;
				while (!found && columns > 1)
				{
					columnsWidth = new int[columns];
					c = 0;
					for (int n = 0; n < items.Count && width <= paintRectangle.Width; n++)
					{
						item = items[n];
						w = itemExtraWidth;
						if (item.Value != null)
							w += (int) g.MeasureString(item.Value.ToString(), Font).Width;
						if (w > columnsWidth[c])
						{
							width += w - columnsWidth[c];
							columnsWidth[c] = w;
						}

						c++;
						if (c == columns)
							c = 0;
					}

					if (width <= paintRectangle.Width)
						found = true;
					else
						columns--;
				}
			}

			if (columns == 1)
				columnsWidth = new int[1] { paintRectangle.Width };

			int r;
			rows = System.Math.DivRem(items.Count, columns, out r);
			if (r != 0)
				rows++;

			if (!scrollVisible && rows * rowHeight > paintRectangle.Height)
			{
				// Reseting to scroll refresh
				SetScroll(true);
				RefreshVerticalLegend();
			}

			g.Dispose();

			if (scrollVisible)
			{
				scroll.Maximum = rows - 1;
				scroll.Minimum = 0;
				if (scrollPosition != scroll.Value)
				{
					scrollPosition = scroll.Value;
				}
				scroll.LargeChange = paintRectangle.Height / rowHeight;
			}
		}

		#endregion

		private ScrollBar scroll;
		private bool scrollVisible;
		private int scrollPosition;

		#region Painting

		private System.Drawing.Rectangle GetRectangle(
			System.Drawing.Rectangle bounds, int x, int y, int width, int height)
		{
			if (RightToLeft == RightToLeft.Yes)
				return new System.Drawing.Rectangle(bounds.Right - x - width, bounds.Top + y, width, height);

			return new System.Drawing.Rectangle(bounds.Left + x, bounds.Top + y, width, height);
		}

		private void PaintHorizontalLegend(System.Drawing.Graphics g, 
			System.Drawing.Rectangle bounds)
		{
			string text;
			System.Drawing.StringFormat sf = new System.Drawing.StringFormat();
			if (RightToLeft == RightToLeft.Yes)
				sf.FormatFlags |= System.Drawing.StringFormatFlags.DirectionRightToLeft;
			sf.LineAlignment = System.Drawing.StringAlignment.Center;
			sf.Alignment = System.Drawing.StringAlignment.Near;

			if (rows == 0)
				return ;

			int x = 0;
			int y = 0;
			int row = 0;
			int col = 0;
			int n = 0;
			if (scrollVisible)
			{
				col = scrollPosition;
				n = rows * scrollPosition;
			}
			LegendItem item;
			System.Drawing.Rectangle rect;
			while (n < items.Count && x < bounds.Width)
			{
				item = items[n];

				text = item.Value == null ? "" : item.Value.ToString();

				rect = GetRectangle(bounds, x + 1, y + 1, boxWidth, rowHeight - 2);
				if (item.Brush != null)
				{
					g.FillRectangle(item.Brush, rect);
				}
				g.DrawRectangle(System.Drawing.SystemPens.WindowFrame, rect);

				rect = GetRectangle(bounds, x + boxWidth + 6, y, columnsWidth[col] - boxWidth - 6, rowHeight);

				g.DrawString(text, Font, System.Drawing.SystemBrushes.ControlText,
					rect, sf);

				y += rowHeight;
				row++;

				if (row == rows)
				{
					row = 0;
					y = 0;
					x += columnsWidth[col];
					col++;
				}

				n++;
			}
		}

		private void PaintVerticalLegend(System.Drawing.Graphics g, 
			System.Drawing.Rectangle bounds)
		{
			string text;
			System.Drawing.StringFormat sf = new System.Drawing.StringFormat();
			if (RightToLeft == RightToLeft.Yes)
				sf.FormatFlags |= System.Drawing.StringFormatFlags.DirectionRightToLeft;
			sf.LineAlignment = System.Drawing.StringAlignment.Center;
			sf.Alignment = System.Drawing.StringAlignment.Near;

			if (rows == 0)
				return ;

			int x = 0;
			int y = 1;
			int col = 0;
			int n = 0;
			if (scrollVisible)
			{
				n = scrollPosition * columnsWidth.Length;
			}
			LegendItem item;
			System.Drawing.Rectangle rect;
			while (n < items.Count && y < bounds.Height)
			{
				item = items[n];

				text = item.Value == null ? "" : item.Value.ToString();

				rect = GetRectangle(bounds, x + 1, y + 1, boxWidth, rowHeight - 2);
				if (item.Brush != null)
				{
					g.FillRectangle(item.Brush, rect);
				}
				g.DrawRectangle(System.Drawing.SystemPens.WindowFrame, rect);

				rect = GetRectangle(bounds, x + boxWidth + 6, y, columnsWidth[col] - boxWidth - 6, rowHeight);

				g.DrawString(text, Font, System.Drawing.SystemBrushes.ControlText,
					rect, sf);

				x += columnsWidth[col];
				col++;

				if (col == columnsWidth.Length)
				{
					x = 0;
					col = 0;
					y += rowHeight;
				}

				n++;
			}
		}

		private void PaintLegend(System.Drawing.Graphics g, 
			System.Drawing.Rectangle bounds)
		{
			g.Clip = new System.Drawing.Region(bounds);
			if (_direction == LegendControlDirection.Horizontal)
				PaintHorizontalLegend(g, bounds);
			else
				PaintVerticalLegend(g, bounds);
			g.ResetClip();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint (e);

			if (_borderStyle == BorderStyle.Fixed3D)
			{
				int right = Width - 1;
				int bottom = Height - 1;
				e.Graphics.DrawLine(System.Drawing.SystemPens.ControlDark,
					new System.Drawing.Point(0, 0), new System.Drawing.Point(right - 1, 0));
				e.Graphics.DrawLine(System.Drawing.SystemPens.ControlDark,
					new System.Drawing.Point(0, 0), new System.Drawing.Point(0, bottom - 1));
				e.Graphics.DrawLine(System.Drawing.SystemPens.ControlDarkDark,
					new System.Drawing.Point(1, 1), new System.Drawing.Point(right - 2, 1));
				e.Graphics.DrawLine(System.Drawing.SystemPens.ControlDarkDark,
					new System.Drawing.Point(1, 1), new System.Drawing.Point(1, bottom - 2));
				e.Graphics.DrawLine(System.Drawing.SystemPens.ControlLightLight,
					new System.Drawing.Point(right, 0), new System.Drawing.Point(right, bottom));
				e.Graphics.DrawLine(System.Drawing.SystemPens.ControlLightLight,
					new System.Drawing.Point(0, bottom), new System.Drawing.Point(right, bottom));
				e.Graphics.DrawLine(System.Drawing.SystemPens.ControlLight,
					new System.Drawing.Point(right - 1, 1), new System.Drawing.Point(right - 1, bottom - 1));
				e.Graphics.DrawLine(System.Drawing.SystemPens.ControlLight,
					new System.Drawing.Point(1, bottom - 1), new System.Drawing.Point(right - 1, bottom - 1));
			}
			else if (_borderStyle == BorderStyle.FixedSingle)
			{
				e.Graphics.DrawRectangle(System.Drawing.SystemPens.ControlDarkDark,
					0, 0, Width - 1, Height - 1);
			}

			e.Graphics.FillRectangle(System.Drawing.SystemBrushes.Control, paintRectangle);
			PaintLegend(e.Graphics, paintRectangle);
		}

		#endregion

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged (e);

			RefreshLegend();

			if (scrollVisible)
			{
				int border = 0;
				if (_borderStyle == BorderStyle.Fixed3D)
					border = 2;
				else if (_borderStyle == BorderStyle.FixedSingle)
					border = 1;

				int top, left;

				if (_direction == LegendControlDirection.Vertical)
				{
					scroll.Height = Height - (border * 2);
					top = border;
					if (RightToLeft == RightToLeft.Yes)
					{
						left = border;
					}
					else
					{
						left = Width - border - scroll.Width;
					}
				}
				else
				{
					scroll.Width = Width - (border * 2);
					left = border;
					top = Height - border - scroll.Height;
				}

				scroll.Location = new System.Drawing.Point(left, top);
			}
		}

		protected override void OnRightToLeftChanged(EventArgs e)
		{
			base.OnRightToLeftChanged (e);

			RefreshLegend();
		}


		private void ScrollScroll(object sender, ScrollEventArgs e)
		{
			if (e.Type != ScrollEventType.EndScroll)
			{
				scrollPosition = e.NewValue;
				Refresh();
			}
		}
	}

	public enum LegendControlDirection
	{
		Vertical,
		Horizontal
	}
}
