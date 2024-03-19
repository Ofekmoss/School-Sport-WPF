using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.ComponentModel;
using System.Drawing.Design;

namespace Sport.UI.Controls
{
	public class ThemeButton : ControlWrapper
	{
		private static readonly int defaultWidth = 21;
		private static readonly int defaultHeight = 21;
		private static readonly int horzMiddle = 12;
		private static readonly int vertMiddle = 10;
		private System.Windows.Forms.ImageList buttonImages;
		private System.ComponentModel.IContainer components;
		private Bitmap bmpNormal;
		private Bitmap bmpHighlight;
		private Bitmap bmpDown;

		private enum ImageIndexes
		{
			Normal,
			Highlight,
			Down
		}

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ThemeButton));
			this.buttonImages = new System.Windows.Forms.ImageList(this.components);
			// 
			// buttonImages
			// 
			this.buttonImages.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			this.buttonImages.ImageSize = new System.Drawing.Size(21, 21);
			this.buttonImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
			this.buttonImages.TransparentColor = System.Drawing.Color.Transparent;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			bmpNormal.Dispose();
			bmpHighlight.Dispose();
			bmpDown.Dispose();
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		
		public ThemeButton()
		{
			InitializeComponent();
			bmpNormal = (Bitmap) buttonImages.Images[(int) ImageIndexes.Normal];
			bmpHighlight = (Bitmap) buttonImages.Images[(int) ImageIndexes.Highlight];
			bmpDown = (Bitmap) buttonImages.Images[(int) ImageIndexes.Down];
			Font = new Font("Arial", 13, FontStyle.Bold, GraphicsUnit.Pixel);

			SetStyle(System.Windows.Forms.ControlStyles.StandardDoubleClick, false);
		}

		private bool recreate = false;

		private void CreateImages()
		{
			if (!recreate)
				return ;

			Bitmap normal = (Bitmap) buttonImages.Images[(int) ImageIndexes.Normal];
			Bitmap highlight = (Bitmap) buttonImages.Images[(int) ImageIndexes.Highlight];
			Bitmap down = (Bitmap) buttonImages.Images[(int) ImageIndexes.Down];
			if (bmpNormal != normal)
				bmpNormal.Dispose();
			if (bmpHighlight != highlight)
				bmpHighlight.Dispose();
			if (bmpDown != down)
				bmpDown.Dispose();

			bmpNormal = new Bitmap(normal, Size);
			bmpHighlight = new Bitmap(highlight, Size);
			bmpDown = new Bitmap(down, Size);

			int ox;
			int oy;

			for (int x = 0; x < Width; x++)
			{
				for (int y = 0; y < Height; y++)
				{
					if (x < horzMiddle)
						ox = x;
					else if (Width - x < defaultWidth - horzMiddle)
						ox = defaultWidth - (Width - x);
					else
						ox = horzMiddle;
					if (y < vertMiddle)
						oy = y;
					else if (Height - y < defaultHeight - vertMiddle)
						oy = defaultHeight - (Height - y);
					else
						oy = vertMiddle;

					bmpNormal.SetPixel(x, y, 
						CreateHSBColor(_hue, _saturation, 
							normal.GetPixel(ox, oy).GetBrightness()));
					bmpHighlight.SetPixel(x, y, 
						CreateHSBColor(_hue, _saturation, 
						highlight.GetPixel(ox, oy).GetBrightness()));
					bmpDown.SetPixel(x, y, 
						CreateHSBColor(_hue, _saturation, 
						down.GetPixel(ox, oy).GetBrightness()));
				}
			}

			recreate = false;
		}

		private Color CreateHSBColor(float hue, float saturation, float brightness)
		{
			int red;
			int green;
			int blue;

			if (saturation == 0.0) // Grauton, einfacher Fall
			{
				red = (byte)(brightness * 255.0F);
				green = red;
				blue = red;
			}
			else
			{
				float rm1;
				float rm2;
        
				if (brightness <= 0.5f) 
				{
					rm2 = brightness + brightness * saturation;
				}
				else
				{
					rm2 = brightness + saturation - brightness * saturation;
				}
				rm1 = 2.0f * brightness - rm2;
				red   = CreateColor(rm1, rm2, hue + 120.0f);   
				green = CreateColor(rm1, rm2, hue);
				blue  = CreateColor(rm1, rm2, hue - 120.0f);
			}

			return Color.FromArgb(red, green, blue);
		}

        private byte CreateColor(float rm1, float rm2, float rh)
		{
			if (rh > 360.0f) 
			{
				rh -= 360.0f;
			}
			else if (rh <   0.0f) 
			{
				rh += 360.0f;
			}
 
			if (rh <  60.0f) 
			{
				rm1 = rm1 + (rm2 - rm1) * rh / 60.0f;   
			}
			else if (rh < 180.0f) 
			{
				rm1 = rm2;
			}
			else if (rh < 240.0f) 
			{
				rm1 = rm1 + (rm2 - rm1) * (240.0f - rh) / 60.0f; 
			}
                   
			return (byte)(rm1 * 255);
		}

		private float _hue = 220f;
		private float _saturation = 0.9f;
		public float Hue
		{
			get { return _hue; }
			set
			{
				if (_hue != value)
				{
					_hue = value;
					recreate = true;
				}
			}
		}
		public float Saturation
		{
			get { return _saturation; }
			set
			{
				if (_saturation != value)
				{
					_saturation = value;
					recreate = true;
				}
			}
		}

		private StringAlignment _alignment = StringAlignment.Center;
		public StringAlignment Alignment
		{
			get { return _alignment; }
			set { _alignment = value; }
		}

		private bool _autoSize = false;
		public override bool AutoSize
		{
			get { return _autoSize; }
			set
			{
				if (_autoSize != value)
				{
					_autoSize = value;

					if (_autoSize)
					{
						AutoSizeControl();
					}
				}
			}
		}

        private void AutoSizeControl()
		{
			Graphics g = CreateGraphics();

			StringFormat sf = new StringFormat(
				RightToLeft == RightToLeft.Yes ? StringFormatFlags.DirectionRightToLeft : (StringFormatFlags) 0);

			sf.LineAlignment = StringAlignment.Center;

			sf.Alignment = Alignment;

			SizeF s = g.MeasureString(Text, Font, 0, sf);
			s.Height += 1;
            s.Width += 8;

			if ((image != null || (imageList != null && imageIndex >= 0)) && 
				(imageAlign == ContentAlignment.MiddleLeft ||
				imageAlign == ContentAlignment.MiddleRight))
			{
				s.Width += imageSize.Width + 1;

				if (s.Height < imageSize.Height)
					s.Height = imageSize.Height + imageSpacing * 2;
			}

			if (s.Width < 10)
				s.Width = 10;
			if (s.Height < 10)
				s.Height = 10;

			Size = s.ToSize();

			g.Dispose();
		}

		public override string Text
		{
			get
			{
				return base.Text;
			}
			set
			{
				base.Text = value;

				if (_autoSize)
					AutoSizeControl();
			}
		}


		public enum ButtonState
		{
			Normal,
			Highlight,
			Down
		}

		private ButtonState _state = ButtonState.Normal;
		public ButtonState State
		{
			get { return _state; }
		}


		private Image image = null;
		public Image Image
		{
			get { return image; }
			set 
			{ 
				image = value; 
				if (image != null)
					imageSize = image.Size;
			}
		}

		private ImageList imageList = null;
		public ImageList ImageList
		{
			get { return imageList; }
			set
			{
				imageList = value;
                if (imageList != null)
					imageSize = imageList.ImageSize;
			}
		}

		private int imageIndex = -1;
		[Editor("System.Windows.Forms.Design.ImageIndexEditor, System.Design", typeof(UITypeEditor)),
        DefaultValue(-1),TypeConverter(typeof(ImageIndexConverter))]
		public int ImageIndex
		{
			get { return imageIndex; }
			set
			{
				imageIndex = value;
			}
		}

		private ContentAlignment imageAlign = ContentAlignment.MiddleLeft;
		public ContentAlignment ImageAlign
		{
			get { return imageAlign; }
			set { imageAlign = value; }
		}

		private Size imageSize;
		public Size ImageSize
		{
			get { return imageSize; }
			set { imageSize = value; }
		}

		private Color transparent = Color.Black;
		public Color Transparent
		{
			get { return transparent; }
			set { transparent = value; }
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize (e);

			recreate = true;

			if (_autoSize)
				AutoSizeControl();

			Refresh();
		}

		private System.Drawing.Pen focusPen = null;

		private static readonly int imageSpacing = 4;

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint (e);

			Bitmap b = new Bitmap(Width, Height, e.Graphics);
			Graphics g = Graphics.FromImage(b);

			g.FillRectangle(SystemBrushes.WindowText, 0, 0, Width, Height);

			CreateImages();

			if (!Enabled)
			{
				g.DrawImage(bmpNormal, 0, 0);
			}
			else
			{
				switch (_state)
				{
					case (ButtonState.Highlight):
						g.DrawImage(bmpHighlight, 0, 0);
						break;
					case (ButtonState.Down):
						g.DrawImage(bmpDown, 0, 0);
						break;
					default:
						g.DrawImage(bmpNormal, 0, 0);
						break;
				}
			}

			int offset = 0;
			if (image != null || (imageList != null && imageIndex >= 0))
			{
				int ix;
				int iy;
				switch (imageAlign)
				{
					case (ContentAlignment.BottomCenter):
					case (ContentAlignment.BottomLeft):
					case (ContentAlignment.BottomRight):
						iy = Height - imageSize.Height - imageSpacing;
						break;
					case (ContentAlignment.TopCenter):
					case (ContentAlignment.TopLeft):
					case (ContentAlignment.TopRight):
						iy = imageSpacing;
						break;
					default:
						iy = ((Height - imageSize.Height) / 2) +
							((Height - imageSize.Height) % 2);
						break;
				}

				switch (imageAlign)
				{
					case (ContentAlignment.BottomLeft):
					case (ContentAlignment.TopLeft):
					case (ContentAlignment.MiddleLeft):
						ix = imageSpacing;
						break;
					case (ContentAlignment.BottomRight):
					case (ContentAlignment.TopRight):
					case (ContentAlignment.MiddleRight):
						ix = Width - imageSize.Width - imageSpacing + 2;
						break;
					default:
						ix = (Width - imageSize.Width) / 2;
						break;
				}


				if (imageList != null && imageIndex >= 0)
				{
					imageList.Draw(g, ix, iy, imageSize.Width, imageSize.Height, imageIndex);
				}
				else
				{
					ImageAttributes ia = new ImageAttributes();
					ia.SetColorKey(transparent, transparent, ColorAdjustType.Default);

					g.DrawImage(image, new Rectangle(ix, iy, imageSize.Width, imageSize.Height),
						0, 0, image.Width, image.Height, GraphicsUnit.Pixel, ia);
				}

				if (imageAlign == ContentAlignment.MiddleLeft)
					offset = imageSize.Width;
				else if (imageAlign == ContentAlignment.MiddleRight)
					offset = -imageSize.Width;
			}

			StringFormat sf = new StringFormat(
				RightToLeft == RightToLeft.Yes ? StringFormatFlags.DirectionRightToLeft : (StringFormatFlags) 0);

			sf.LineAlignment = StringAlignment.Center;

			sf.Alignment = Alignment;

			Rectangle rect = offset < 0 ?
				new Rectangle(3, 1, Width - 6 + offset, Height - 3)
				: new Rectangle(3 + offset, 1, Width - 6 - offset, Height - 3);

			g.TextContrast = 0;
			g.DrawString(Text, Font, new SolidBrush(Color.White), rect, sf);

			if (Focused)
			{
				if (focusPen == null)
				{
					focusPen = new Pen(Color.White);
					focusPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
				}

				g.DrawRectangle(focusPen, 2, 2, Width - 5, Height - 5);
			}

			b.SetPixel(0, 0, BackColor);
			b.SetPixel(Width - 1, 0, BackColor);
			b.SetPixel(0, Height - 1, BackColor);
			b.SetPixel(Width - 1, Height - 1, BackColor);

			if (!Enabled)
				ControlPaint.DrawImageDisabled(e.Graphics, b, 0, 0, BackColor);
			else
				e.Graphics.DrawImage(b, new Point(0, 0));
		}

		bool over = false;
		bool down = false;

		protected override void OnPaintBackground(PaintEventArgs pevent)
		{
			//			base.OnPaintBackground (pevent);
		}

		protected override void OnMouseEnter(EventArgs e)
		{
			base.OnMouseEnter (e);
			if (!over)
			{
				over = true;
				if (down)
					_state = ButtonState.Down;
				else
					_state = ButtonState.Highlight;
				Refresh();
			}
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave (e);
			if (over)
			{
				over = false;
				_state = ButtonState.Normal;
				Refresh();
			}
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown (e);
			if (e.Button == MouseButtons.Left)
			{
				if (!down)
				{
					down = true;
					_state = ButtonState.Down;
					Refresh();
				}
			}
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp (e);
			if (down)
			{
				down = false;
				if (over)
					_state = ButtonState.Highlight;
				else
					_state = ButtonState.Normal;
				Refresh();
			}
		}

		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus (e);

			Refresh();
		}


		protected override void OnLostFocus(EventArgs e)
		{
			base.OnLostFocus (e);
			_state = ButtonState.Normal;
			Refresh();
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown (e);

			if (!e.Handled && e.Modifiers == Keys.None && Enabled)
			{
				if (e.KeyCode == Keys.Space || e.KeyCode == Keys.Enter)
				{
					down = true;
					_state = ButtonState.Down;
					Refresh();
				}
			}
		}

		protected override void OnKeyUp(KeyEventArgs e)
		{
			base.OnKeyUp (e);

			if (!e.Handled && e.Modifiers == Keys.None && down)
			{
				if (e.KeyCode == Keys.Space || e.KeyCode == Keys.Enter)
				{
					down = false;
					if (over)
						_state = ButtonState.Highlight;
					else
						_state = ButtonState.Normal;
					Refresh();

					OnClick(EventArgs.Empty);
				}
			}
		}


	}
}
