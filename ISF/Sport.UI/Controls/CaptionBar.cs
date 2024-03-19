using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using Sport.Common;

namespace Sport.UI.Controls
{
	/// <summary>
	/// CaptionBar implements the control for displaying
	/// the opened views, and enables selecting the
	/// current view that is displayed.
	/// The implementation is for a list of captions and a close
	/// button that gives event on caption selection change.
	/// </summary>
	public class CaptionBar : ControlWrapper
	{		
		private const int MinCaptionWidth = 80;
		
		public CaptionBar()
		{			
			captions = new CaptionCollection(this);
			captions.Changed += new CollectionEventHandler(ItemChanged);

			Size = new System.Drawing.Size(0, Font.Height + 6);
			Dock = DockStyle.Top;
			SetStyle(ControlStyles.ResizeRedraw, true);
		}


		#region CaptionItem Class

		/// <summary>
		/// CaptionItem holds the text and width of a single
		/// caption in the caption list.
		/// </summary>
		public sealed class CaptionItem : GeneralCollection.CollectionItem
		{
			internal string _text;

			internal int width;

			public string Text
			{
				get { return _text; }
				set 
				{
					if (_text != value)
					{
						_text = value;
						if (Owner != null)
						{
							((CaptionBar) Owner).ResizeCaptions();
							((CaptionBar) Owner).Refresh();
						}
					}
				}
			}

			public CaptionItem(string text)
			{
				_text = text;
			}

			public CaptionItem()
			{
				width = MinCaptionWidth;
			}
		}

		#endregion

		#region CaptionCollection Class

		/// <summary>
		/// CaptionCollection inherits GeneralCollection to
		/// use CaptionItem.
		/// </summary>
		public class CaptionCollection : GeneralCollection
		{
			public CaptionItem this[int index]
			{
				get { return (CaptionItem) GetItem(index); }
			}

			public void Insert(int index, CaptionItem value)
			{
				InsertItem(index, value);
			}

			public void Insert(int index, string value)
			{
				InsertItem(index, new CaptionItem(value));
			}

			public void Remove(CaptionItem value)
			{
				RemoveItem(value);
			}

			public void Remove(string value)
			{
				foreach (CaptionItem ci in this)
				{
					if (ci.Text == value)
					{
						RemoveItem(ci);
						return ;
					}
				}
			}

			public bool Contains(CaptionItem value)
			{
				return base.Contains(value);
			}

			public int IndexOf(CaptionItem value)
			{
				return base.IndexOf(value);
			}

			public int Add(CaptionItem value)
			{
				return AddItem(value);
			}

			public int Add(string value)
			{
				return Add(new CaptionItem(value));
			}

			public CaptionCollection(CaptionBar owner)
				: base(owner)
			{
			}
		}

		#endregion

		#region Collection

		/// <summary>
		/// The caption bar holds a collection of CaptionItem
		/// to store captions information.
		/// </summary>

		private CaptionCollection captions;

		public CaptionCollection Items
		{
			get { return captions; }
		}

		private void ItemChanged(object sender, CollectionEventArgs e)
		{
			if (e.Old != null)
			{
				ResizeCaptions();
				Refresh();
			}
			if (e.New != null)
			{
				ResizeCaptions();
				Refresh();
			}
		}

		#endregion

		#region Close Event

		/// <summary>
		/// The close event occurs when the user presses on
		/// the close button.
		/// </summary>

		private EventHandler onClose;

		private void OnClose(EventArgs e)
		{
			if (onClose != null)
			{
				onClose.Invoke(this, e);
			}
		}

		public event EventHandler Close
		{
			add 
			{
				onClose += value;
			}
			remove
			{
				onClose -= value;
			}
		}

		#endregion

		#region Selection

		/// <summary>
		/// The current caption selected is stored in 
		/// selectedCaption. When the selection is
		/// changed by the user the SelectionChanged
		/// event occurs.
		/// </summary>

		private int	selectedCaption = -1;
		private EventHandler onSelectionChanged;

		private void OnSelectionChanged(EventArgs e)
		{
			if (onSelectionChanged != null)
			{
				onSelectionChanged.Invoke(this, e);
			}
		}

		public event EventHandler SelectionChanged
		{
			add 
			{
				onSelectionChanged += value;
			}
			remove
			{
				onSelectionChanged -= value;
			}
		}

		public int SelectedIndex
		{
			get { return selectedCaption; }
			set 
			{
				if (value != selectedCaption)
				{
					if (value >= -1 && value < captions.Count)
					{
						selectedCaption = value;
						Refresh();
					}
				}
			}
		}

		public CaptionItem SelectedItem
		{
			get 
			{ 
				return selectedCaption == -1 ? null : 
					(CaptionItem) captions[selectedCaption]; 
			}

			set
			{
				if (value == null)
				{
					if (selectedCaption != -1)
					{
						selectedCaption = -1;
						Refresh();
					}
				}
				else
				{
					int sel = captions.IndexOf(value);
					if (sel != -1 && sel != selectedCaption)
					{
						selectedCaption = sel;
						Refresh();
					}
				}
			}
		}

		#endregion
	
		#region Design

		private BorderStyle _borderStyle = BorderStyle.None;
		public BorderStyle BorderStyle
		{
			get { return _borderStyle; }
			set 
			{ 
				_borderStyle = value; 
				Refresh();
			}
		}

		private CaptionBarAppearance _appearance = CaptionBarAppearance.Buttons;
		public CaptionBarAppearance Appearance
		{
			get { return _appearance; }
			set
			{
				_appearance = value;
				Refresh();
			}
		}

		private bool scrollable = false;
		private int firstItem = 0;
		private int lastItem = 0;

		/// <summary>
		/// ResizeCaptions recalculate the width of each
		/// caption and checks scrolling parameters.
		/// </summary>
		private void ResizeCaptions()
		{
			int totalWidth = 0;
			int maxWidth = Size.Width - 4;
			int i = 0;
			
			Graphics g = null;
			try
			{
				g = CreateGraphics();
			}
			catch
			{}
            
			if (g == null)
				return;
			
			foreach (CaptionItem ci in captions)
			{
				// Minimal caption width is the width needed by
				// its text
				ci.width = g.MeasureString(ci._text, Font).ToSize().Width;
				ci.width += 4;
				totalWidth += 4 + ci.width;
			}

			// Enlarging the width of the captions
			// to MinCaptionWidth until fills entire control width
			i = 0;
			while (totalWidth < maxWidth && i < captions.Count)
			{
				CaptionItem ci = captions[i];
				if (ci.width < MinCaptionWidth)
				{
					int dif = MinCaptionWidth - ci.width;
					if (totalWidth + dif < maxWidth)
					{
						ci.width += dif;
						totalWidth += dif;
					}
					else
					{
						dif = maxWidth - totalWidth - 1;
						ci.width += dif;
						totalWidth += dif;
					}
				}
				i++;
			}

			// Checking if and how much scrolling is needed
			if (totalWidth > maxWidth)
			{
				scrollable = true;

				lastItem = 0;
				while (totalWidth > maxWidth && lastItem < captions.Count)
				{
					totalWidth -= captions[lastItem].width;
					lastItem++;
				}

				if (firstItem > lastItem)
					firstItem = lastItem;
			}
			else
			{
				scrollable = false;
				firstItem = 0;
			}

			g.Dispose();
		}


		#region Hit Test

		/// <summary>
		/// Hit test implementation.
		/// The HitTest function receives control coordinates
		/// and returns where those hit on the control:
		/// on a caption and on which caption,
		/// on the scroll buttons and on which button,
		/// on the close button or none of them.
		/// </summary>
		public enum HitTestType
		{
			None,
			Caption,
			Scroll
		}

		public sealed class HitTestInfo
		{
			internal HitTestType _type;
			internal Rectangle _bounds;

			// For caption states the caption index
			// For scroll: 1 is the next (left) button 
			//        and -1 is the prev (right) button
			internal int _item;

			public HitTestType Type
			{
				get { return _type; }
			}
			public int Item
			{
				get { return _item; }
			}
			public Rectangle Bounds
			{
				get { return _bounds; }
			}

			internal HitTestInfo()
			{
				_type = HitTestType.None;
				_item = -1;
			}

			internal HitTestInfo(HitTestType type, Rectangle bounds)
			{
				_type = type;
				_bounds = bounds;
				_item = -1;
			}

			internal HitTestInfo(HitTestType type, Rectangle bounds, int item)
			{
				_type = type;
				_bounds = bounds;
				_item = item;
			}
		}

		public HitTestInfo HitTest(Point position)
		{
			return HitTest(position.X, position.Y);
		}

		public HitTestInfo HitTest(int x, int y)
		{
			int h = Font.Height;

			if (y >= 3 && y <= h + 3)
			{
				// Checking if on scroll buttons
				if (scrollable)
				{
					if (x >= 3 && x <= 15)
						return new HitTestInfo(HitTestType.Scroll, 
							new Rectangle(3, 3, 12, h + 1), 1);
					if (x >= Size.Width - 14 && x <= Size.Width - 2)
						return new HitTestInfo(HitTestType.Scroll, 
							new Rectangle(Size.Width - 14, 3, 12, h + 1), -1);
				}

				// Checking if on caption
				int mostLeft = scrollable ? 17 : 3;

				if (x >= mostLeft)
				{
					int right = Size.Width - (scrollable ? 17 : 4);

					int i = firstItem;
					while (right >= x && i < captions.Count)
					{
						CaptionItem ci = (CaptionItem) captions[i];
						
						if (x >= right - ci.width)
						{
							int width = ci.width;
							right = right - ci.width;
							if (right < mostLeft)
							{
								width -= mostLeft - right;
								right = mostLeft;
							}

							return new HitTestInfo(HitTestType.Caption, 
								new Rectangle(right, 3, width + 1, h + 1), i);
						}

						right -= ci.width + 4;
						i++;
					}
				}
			}

			// returns 'none'
			return new HitTestInfo();
		}

		#endregion

		/// <summary>
		/// OnPaint is called when the control needs to
		/// draw itself. OnPaint draws all buttons and captions on
		/// the control.
		/// </summary>
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			if (_appearance == CaptionBarAppearance.Buttons)
			{
				if (_borderStyle == BorderStyle.Fixed3D)
				{
					ControlPaint.DrawBorder3D(e.Graphics, 0, 0, Width, Height,
						Border3DStyle.Raised);
				}
				else if (_borderStyle == BorderStyle.FixedSingle)
				{
					e.Graphics.DrawRectangle(SystemPens.WindowFrame, 0, 0, Width - 1, Height - 1);
				}
			}
			else if (_appearance == CaptionBarAppearance.Tabs)
			{
				if (_borderStyle == BorderStyle.Fixed3D)
				{
					ControlPaint.DrawBorder3D(e.Graphics, 0, Height - 2, Width, 2,
						Border3DStyle.Raised, Border3DSide.Top);
				}
				else if (_borderStyle == BorderStyle.FixedSingle)
				{
					e.Graphics.DrawLine(SystemPens.WindowFrame, 0, Height, Width, Height);
				}
			}

			if (buttonIndex != ButtonIndex.None)
			{
				if (downInfo == null)
				{
					ControlPaint.DrawBorder3D(e.Graphics, buttonFrame, Border3DStyle.RaisedInner, Border3DSide.All);
				}
				else
				{
					ButtonIndex btn = ButtonIndex.None;
					if (downInfo.Type == HitTestType.Scroll)
						btn = downInfo.Item == 1 ? ButtonIndex.Left : ButtonIndex.Right;
					else btn = (ButtonIndex) downInfo.Item;

					if (btn == buttonIndex)
					{
						ControlPaint.DrawBorder3D(e.Graphics, buttonFrame, Border3DStyle.SunkenOuter, Border3DSide.All);
					}
				}
			}

			int h = Font.Height;

			if (scrollable)
			{
				// Draw scroll buttons (left and right) if needed
				ControlPaint.DrawScrollButton(e.Graphics, 3, 3, 12, h + 1, 
					ScrollButton.Left, ButtonState.Flat | (firstItem == lastItem ? ButtonState.Inactive : 0));
				ControlPaint.DrawScrollButton(e.Graphics, Size.Width - 14, 3, 12, h + 1, 
					ScrollButton.Right, ButtonState.Flat | (firstItem == 0 ? ButtonState.Inactive : 0));
			}

			StringFormat f = new StringFormat(StringFormatFlags.DirectionRightToLeft);
			f.Alignment = StringAlignment.Center;
			System.Drawing.Pen pen = new System.Drawing.Pen(SystemColors.ControlText);

			// left most position to draw captions
			int mostLeft = scrollable ? 17 : 3;
			// right most position to draw captions
			int right = Size.Width - (scrollable ? 17 : 4);

			// Drawing all captions, starting with 
			// firstItem - the right most item visible (changed by scrolling)
			for (int i = firstItem; i < captions.Count && right > mostLeft; i++)
			{
				CaptionItem ci = (CaptionItem) captions[i];

				int width = ci.width;
				int left = right - width;
				if (left < mostLeft)
				{
					width -= mostLeft - left;
					left = mostLeft;
				}

				if (_appearance == CaptionBarAppearance.Tabs)
				{
					if (i == selectedCaption)
					{
						ControlPaint.DrawBorder3D(e.Graphics, left - 4, 0,
							width + 8, h + 5, Border3DStyle.Raised,
							Border3DSide.Top | Border3DSide.Right | Border3DSide.Left);
						e.Graphics.FillRectangle(SystemBrushes.Control,
							left - 2, h + 5, width + 4, 2);
					}
				}

				Rectangle rect = new Rectangle(left, 3, width, h);

				e.Graphics.FillRectangle(
					i == selectedCaption ? SystemBrushes.Info : SystemBrushes.Control, 
					rect);

				e.Graphics.DrawRectangle(pen, rect);

				// Changing rect to better place the text in the rectangle
				rect = new Rectangle(left + 1, 3, width, h);
				e.Graphics.DrawString(ci._text, Font, 
					SystemBrushes.ControlText, rect, f);

				right -= ci.width + 4;
			}

/*			if (buttonFrame != null)
			{
				Rectangle rect = (Rectangle) buttonFrame;
				e.Graphics.ExcludeClip(new Rectangle(rect.Left + 1, rect.Top + 1, rect.Width - 2, rect.Height - 2));
				ControlPaint.DrawBorder3D(e.Graphics, rect.Left, rect.Top,
					rect.Width, rect.Height,
					Border3DStyle.RaisedInner, Border3DSide.All);
			}*/

		}
	
		/// <summary>
		/// OnResize called when the control size has changed.
		/// OnResize keeps the height relative to the font height
		/// and calls ResizeCaptions to reset captions width.
		/// </summary>
		protected override void OnResize(EventArgs e)
		{
			base.OnResize (e);

			int h = Font.Height + 6;
			if (Size.Height != h)
			{
				Size = new Size(Size.Width, h);
			}

			ResizeCaptions();
		}

		#endregion
	
		#region Mouse Events


		// Stores the last button framed in view and
		// whether its pressed
		enum ButtonIndex
		{
			None = -1,
			Left = -2,
			Right = -3
		}
		private ButtonIndex buttonIndex = ButtonIndex.None;
		private Rectangle buttonFrame;
		private HitTestInfo downInfo = null;

		/// <summary>
		/// OnMouseDown is called 
		/// </summary>
		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				HitTestInfo hti = HitTest(e.X, e.Y);

//				buttonPressed = true;

				buttonFrame = new Rectangle(hti.Bounds.Left - 1,
					hti.Bounds.Top - 1, hti.Bounds.Width + 2,
					hti.Bounds.Height + 2);

				if (hti.Type == HitTestType.Caption)
				{
					buttonIndex = (ButtonIndex) hti.Item;
					downInfo = hti;
				}
				else if (hti.Type == HitTestType.Scroll)
				{
					buttonIndex = hti.Item == 1 ? ButtonIndex.Left : ButtonIndex.Right;
					downInfo = hti;
				}

				Invalidate(buttonFrame);

				Capture = true;
			}

			base.OnMouseDown (e);
		}

		protected override void OnLostFocus(EventArgs e)
		{
			base.OnLostFocus (e);
			ReleaseCapture();
		}


		private void ReleaseCapture()
		{
			Capture = false;
			buttonIndex = ButtonIndex.None;
			downInfo = null;
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave (e);
			buttonIndex = ButtonIndex.None;
			Invalidate(buttonFrame);
		}


		protected override void OnMouseMove(MouseEventArgs e)
		{
			HitTestInfo hti = HitTest(e.X, e.Y);

			ButtonIndex btn = ButtonIndex.None;
			if (hti.Type == HitTestType.Scroll)
				btn = hti.Item == 1 ? ButtonIndex.Left : ButtonIndex.Right;
			else btn = (ButtonIndex) hti.Item;

			if (btn != buttonIndex)
			{
				Invalidate(buttonFrame);

				if (hti.Type != HitTestType.None)
				{
					buttonFrame = new Rectangle(hti.Bounds.Left - 1,
						hti.Bounds.Top - 1, hti.Bounds.Width + 2,
						hti.Bounds.Height + 2);
					Invalidate(buttonFrame);
				}


				buttonIndex = btn;
			}
			
			base.OnMouseMove (e);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			if (downInfo != null && e.Button == MouseButtons.Left)
			{
				HitTestInfo hti = HitTest(e.X, e.Y);
			
				if (hti.Type == HitTestType.Scroll && downInfo.Item == hti.Item)
				{
					firstItem += hti.Item;
					if (firstItem < 0) 
						firstItem = 0;
					else if (firstItem > lastItem)
						firstItem = lastItem;
					Refresh();
				}
				else if (hti.Type == HitTestType.Caption &&
					hti.Item == downInfo.Item)
				{
					SelectedIndex = hti.Item;
					OnSelectionChanged(EventArgs.Empty);
				}

				downInfo = null;
				buttonIndex = ButtonIndex.None;
				Invalidate(buttonFrame);
			}
			
			base.OnMouseUp (e);
		}

		#endregion
	}

	public enum CaptionBarAppearance
	{
		Buttons,
		Tabs
	}
}
