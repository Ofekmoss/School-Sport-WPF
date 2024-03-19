using System;
using System.Windows.Forms;
using System.Drawing;

namespace Sport.Documents
{
	public class DocumentPreview : Control
	{
		private int leftOffset;
		private int topOffset;
		private VScrollBar vScroll;
		private HScrollBar hScroll;
		public DocumentPreview()
		{
			vScroll = new VScrollBar();
			vScroll.Minimum = 0;
			vScroll.Scroll += new ScrollEventHandler(vScrollScroll);
			hScroll = new HScrollBar();
			hScroll.Minimum = 0;
			hScroll.Scroll += new ScrollEventHandler(hScrollScroll);
			_zoom = 1;
			Controls.Add(vScroll);
			Controls.Add(hScroll);
			ResetPageInfo();
			ResetSize();
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

		private Rectangle Workarea
		{
			get
			{
				int border = BorderSize;

				Rectangle rect = new Rectangle(border, border, 
					Width - (border * 2) - vScroll.Width,
					Height - (border * 2) - hScroll.Height);

				if (RightToLeft == RightToLeft.Yes)
				{
					rect.Offset(vScroll.Width, 0);
				}

				return rect;
			}
		}

		private void ResetSize()
		{
			int border = BorderSize;
			vScroll.Height = Height - (border * 2) - hScroll.Height;
			hScroll.Width = Width - (border * 2) - vScroll.Width;

			if (RightToLeft == RightToLeft.Yes)
			{
				vScroll.Location = new Point(border, border);
				hScroll.Location = new Point(border + vScroll.Width, Height - border - hScroll.Height);

			}
			else
			{
				vScroll.Location = new Point(Width - border - vScroll.Width, border);
				hScroll.Location = new Point(border, Height - border - hScroll.Height);
			}

			ResetScroll();

			Refresh();
		}

		private static readonly int PageMargin = 30;
		private void ResetScroll()
		{
			int border = BorderSize;
			int width = Width - (border * 2) - vScroll.Width;
			int height = Height - (border * 2) - hScroll.Height;
			int totalWidth = width;
			int totalHeight = height;
			if (_page != null)
			{
				int pageWidth = (int) ((float) _page.Size.Width * _zoom);
				int pageHeight = (int) ((float)_page.Size.Height * _zoom);
				totalWidth = pageWidth + PageMargin * 2;
				totalHeight = pageHeight + PageMargin * 2;
			}

			if (totalWidth > width && width > 0)
			{
				hScroll.Enabled = true;
				hScroll.Maximum = totalWidth;
				hScroll.LargeChange = width;
				hScroll.SmallChange = width / 10;

				if (hScroll.Value > hScroll.Maximum - hScroll.LargeChange + 1)
					hScroll.Value = hScroll.Maximum - hScroll.LargeChange + 1;
			}
			else
			{
				hScroll.Enabled = false;
				hScroll.Value = 0;
				hScroll.LargeChange = 0;
				hScroll.Maximum = 0;
			}

			if (totalHeight > height && height > 0)
			{
				vScroll.Enabled = true;
				vScroll.Maximum = totalHeight;
				vScroll.LargeChange = height;
				vScroll.SmallChange = height / 10;
				if (vScroll.Value > vScroll.Maximum - vScroll.LargeChange + 1)
					vScroll.Value = vScroll.Maximum - vScroll.LargeChange + 1;
			}
			else
			{
				vScroll.Enabled = false;
				vScroll.Value = 0;
			}

			leftOffset = RightToLeft == RightToLeft.Yes ?
				hScroll.Maximum - hScroll.LargeChange - hScroll.Value : hScroll.Value;
			topOffset = vScroll.Value;
		}

		public System.Drawing.Printing.PrinterSettings settings;

		private void ResetPageInfo()
		{
			if (_page != null)
			{
				if (_pageImage == null || _page.Size != _pageImage.Size)
				{
					/* bitmap
					Graphics g = CreateGraphics();
					_pageImage = new Bitmap(_page.Size.Width, _page.Size.Height, g);
					g.Dispose();
					*/

					Graphics g = CreateGraphics();
					Bitmap bitmap = new Bitmap(_page.Size.Width, _page.Size.Height, g);
					g.Dispose();

					Graphics graphics = Graphics.FromImage(bitmap);

					_pageImage = new System.Drawing.Imaging.Metafile(graphics.GetHdc(), 
						new System.Drawing.Rectangle(0, 0, _page.Size.Width, _page.Size.Height));
					
				}

				_page.Paint(Graphics.FromImage(_pageImage));
			}

			ResetScroll();
			Refresh();
		}

		private Image _pageImage;
		private Page _page;
		public Page Page
		{
			get { return _page; }
			set 
			{ 
				_page = value;
				ResetPageInfo();
			}
		}

		private float _zoom;
		public float Zoom
		{
			get { return _zoom; }
			set
			{
				_zoom = value;
				if (_zoom < 0.1F)
					_zoom = 0.1F;
				ResetScroll();
				Refresh();
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


		protected override void OnPaintBackground(PaintEventArgs pevent)
		{
			//			base.OnPaintBackground (pevent);
		}


		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint (e);

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

			int border = BorderSize;

			Rectangle rect = Workarea;

			if (RightToLeft == RightToLeft.Yes)
			{
				e.Graphics.FillRectangle(SystemBrushes.Control, border, 
					Height - border - hScroll.Height, vScroll.Width, hScroll.Height);
			}
			else
			{
				e.Graphics.FillRectangle(SystemBrushes.Control, Width - border - vScroll.Width, 
					Height - border - hScroll.Height, vScroll.Width, hScroll.Height);
			}

			if (_page == null)
			{
				e.Graphics.FillRectangle(SystemBrushes.AppWorkspace, rect);
			}
			else
			{
				e.Graphics.Clip = new Region(rect);
				int pageWidth = (int) ((float) _page.Size.Width * _zoom) + 2;
				int pageHeight = (int) ((float) _page.Size.Height * _zoom) + 2;
				int pageLeft = 0;
				int pageTop = 0;

				if (pageWidth + PageMargin * 2 > rect.Width)
				{
					pageLeft = -leftOffset + PageMargin + rect.Left;
				}
				else
				{
					pageLeft = (rect.Width - pageWidth) / 2 + rect.Left;
				}

				if (pageHeight + PageMargin * 2 > rect.Height)
				{
					pageTop = -topOffset + PageMargin + rect.Top;
				}
				else
				{
					pageTop = (rect.Height - pageHeight) / 2 + rect.Top;
				}

				// Margins
				if (pageLeft > rect.Left)
				{
					e.Graphics.FillRectangle(SystemBrushes.AppWorkspace,
						rect.Left, rect.Top, pageLeft - rect.Left, rect.Height);
				}
				if (pageLeft + pageWidth < rect.Right)
				{
					e.Graphics.FillRectangle(SystemBrushes.AppWorkspace,
						pageLeft + pageWidth, rect.Top, 
						rect.Right - (pageLeft + pageWidth), rect.Height);
				}
				if (pageTop > rect.Top)
				{
					e.Graphics.FillRectangle(SystemBrushes.AppWorkspace,
						rect.Left, rect.Top, rect.Width, pageTop - rect.Top);
				}
				if (pageTop + pageHeight < rect.Bottom)
				{
					e.Graphics.FillRectangle(SystemBrushes.AppWorkspace,
						rect.Left, pageTop + pageHeight,
						rect.Width, rect.Bottom - (pageTop + pageHeight));
				}

				// Frame
				e.Graphics.DrawRectangle(SystemPens.WindowFrame,
					pageLeft, pageTop, pageWidth - 1, pageHeight - 1);

				// Page
				e.Graphics.FillRectangle(SystemBrushes.Window,
					pageLeft + 1, pageTop + 1, pageWidth - 2, pageHeight - 2);
				e.Graphics.DrawImage(_pageImage,
					pageLeft + 1, pageTop + 1, pageWidth - 2, pageHeight - 2);
			}
		}

		private void vScrollScroll(object sender, ScrollEventArgs e)
		{
			if (e.Type != ScrollEventType.EndScroll)
			{
				topOffset = e.NewValue;
				Refresh();
			}
		}

		private void hScrollScroll(object sender, ScrollEventArgs e)
		{
			if (e.Type != ScrollEventType.EndScroll)
			{
				leftOffset = RightToLeft == RightToLeft.Yes ?
					hScroll.Maximum - hScroll.LargeChange - e.NewValue : e.NewValue;
				Refresh();
			}
		}

		public void FitWidth()
		{
			if (_page != null)
			{
				Rectangle rect = Workarea;
				_zoom = (float) (rect.Width - (PageMargin * 2)) / (float) _page.Size.Width;
				ResetScroll();
				Refresh();
			}
		}

		private void ZoomPoint(int x, int y, float zoom)
		{
			if (_page != null)
			{
				Rectangle rect = Workarea;

				int pageWidth = (int) ((float) _page.Size.Width * _zoom) + 2;
				int pageHeight = (int) ((float) _page.Size.Height * _zoom) + 2;
				int pageLeft = 0;
				int pageTop = 0;

				if (pageWidth + PageMargin * 2 > rect.Width)
				{
					pageLeft = -leftOffset + PageMargin + rect.Left;
				}
				else
				{
					pageLeft = (rect.Width - pageWidth) / 2 + rect.Left;
				}

				if (pageHeight + PageMargin * 2 > rect.Height)
				{
					pageTop = -topOffset + PageMargin + rect.Top;
				}
				else
				{
					pageTop = (rect.Height - pageHeight) / 2 + rect.Top;
				}

				float left = (float) (x - pageLeft) / (float) pageWidth;
				float top = (float) (y - pageTop) / (float) pageHeight;

				_zoom += zoom;
				if (_zoom < 0.1F)
					_zoom = 0.1F;

				ResetScroll();

				pageWidth = (int) ((float) _page.Size.Width * _zoom) + 2;
				pageHeight = (int) ((float) _page.Size.Height * _zoom) + 2;

				if (pageWidth + PageMargin * 2 > rect.Width)
				{
					pageLeft = -leftOffset + PageMargin + rect.Left;
				}
				else
				{
					pageLeft = (rect.Width - pageWidth) / 2 + rect.Left;
				}

				if (pageHeight + PageMargin * 2 > rect.Height)
				{
					pageTop = -topOffset + PageMargin + rect.Top;
				}
				else
				{
					pageTop = (rect.Height - pageHeight) / 2 + rect.Top;
				}

				int centerx = pageLeft + (int) ((float) pageWidth * left);
				int centery = pageTop + (int) ((float) pageHeight * top);

				if (vScroll.Enabled)
				{
					topOffset += centery - (Height / 2);
					if (topOffset < 0)
						topOffset = 0;
					if (topOffset > vScroll.Maximum - vScroll.LargeChange + 1)
						topOffset = vScroll.Maximum - vScroll.LargeChange + 1;
					vScroll.Value = topOffset;
				}

				if (hScroll.Enabled)
				{
					leftOffset += centerx - (Width / 2);
					if (leftOffset < 0)
						leftOffset = 0;
					if (leftOffset > hScroll.Maximum - hScroll.LargeChange + 1)
						leftOffset = hScroll.Maximum - hScroll.LargeChange + 1;
					hScroll.Value = leftOffset;
				}

				Refresh();
			}
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown (e);

			if (!Focused)
				Focus();

			ZoomPoint(e.X, e.Y, e.Button == MouseButtons.Left ? 0.2F : -0.2F);
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			base.OnMouseWheel (e);

			ZoomPoint(e.X, e.Y, (float) e.Delta / 1200);

			Refresh();
		}

	}
}
