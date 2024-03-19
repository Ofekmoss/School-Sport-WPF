using System;
using System.Drawing;
using System.Windows.Forms;

namespace Sport.UI.Controls
{
	public class ButtonBox : ControlWrapper
	{
		public delegate object SelectValue(ButtonBox buttonBox, object value);
		private SelectValue _valueSelector;
		public SelectValue ValueSelector
		{
			get { return _valueSelector; }
			set { _valueSelector = value; }
		}


		public event EventHandler ValueChanged;

		private object _value;
		public object Value
		{
			get { return _value; }
			set
			{
				if (_value != value)
				{
					_value = value;
					Refresh();
				}
			}
		}

		private BorderStyle _borderStyle = BorderStyle.Fixed3D;
		public BorderStyle BorderStyle
		{
			get { return _borderStyle; }
			set 
			{
				_borderStyle = value;
				Refresh();
			}
		}

		private static readonly int minimumHeight = 10;
		private static readonly int maximumBoxSize = 25;
	    
		public ButtonBox()
		{
			Size = new Size(60, 20);
			SetStyle(ControlStyles.ResizeRedraw, true);
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize (e);

			if (Height < minimumHeight)
				Height = minimumHeight;
		}


		private Rectangle GetBoxRect()
		{
			int o = 0;
			int boxSize = Height - 1;
			if (_borderStyle == BorderStyle.Fixed3D)
			{
				o += 2;
				boxSize -= 3;
			}

			if (boxSize > maximumBoxSize)
				boxSize = maximumBoxSize;

			if (RightToLeft == RightToLeft.Yes)
			{
				return new Rectangle(o, o, boxSize, boxSize);
			}
			else
			{
				if (_borderStyle == BorderStyle.Fixed3D)
					return new Rectangle(Width - boxSize - o, o, boxSize, boxSize);
				else
					return new Rectangle(Width - boxSize - o - 1, o, boxSize, boxSize);
			}
		}

		private Rectangle GetTextRect()
		{
			int o = 1;
			int boxSize = Height;
			if (_borderStyle == BorderStyle.Fixed3D)
			{
				o += 2;
				boxSize -= 4;
			}
			else if (_borderStyle == BorderStyle.FixedSingle)
			{
				o += 1;
				boxSize -= 1;
			}

			if (boxSize > maximumBoxSize)
				boxSize = maximumBoxSize;

			if (RightToLeft == RightToLeft.Yes)
			{
				return new Rectangle(o + boxSize, o, Width - o * 2 - boxSize, Height - o * 2);
			}
			else
			{
				return new Rectangle(o, o, Width - o * 2 - boxSize, Height - o * 2);
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint (e);

			if (_borderStyle == BorderStyle.None)
			{
				e.Graphics.FillRectangle(Enabled ? SystemBrushes.Window : SystemBrushes.Control,
					0, 0, Width, Height);
			}
			else if (_borderStyle == BorderStyle.FixedSingle)
			{
				e.Graphics.FillRectangle(Enabled ? SystemBrushes.Window : SystemBrushes.Control,
					1, 1, Width - 2, Height - 2);
				e.Graphics.DrawRectangle(SystemPens.ControlDarkDark,
					0, 0, Width - 1, Height - 1);
			}
			else
			{
				ControlPaint.DrawBorder3D(e.Graphics, 
					new Rectangle(0, 0, Width, Height), 
					Border3DStyle.Sunken, Border3DSide.All);
				e.Graphics.FillRectangle(Enabled ? SystemBrushes.Window : SystemBrushes.Control,
					2, 2, Width - 4, Height - 4);
			}


			StringFormat sf = new StringFormat();
			sf.LineAlignment = StringAlignment.Center;
			sf.Alignment = StringAlignment.Center;

			Rectangle boxRect = GetBoxRect();
			
			if (_borderStyle == BorderStyle.Fixed3D)
			{
				ControlPaint.DrawButton(e.Graphics, boxRect, buttonDown ? ButtonState.Pushed | ButtonState.Flat : ButtonState.Normal);
			}
			else
			{
				e.Graphics.FillRectangle(SystemBrushes.Control, boxRect);
				e.Graphics.DrawRectangle(SystemPens.WindowFrame, boxRect);
				if (buttonDown)
					e.Graphics.DrawRectangle(SystemPens.ControlDark, boxRect.Left + 1, boxRect.Top + 1, 
						boxRect.Height - 2, boxRect.Height - 2);
			}

			e.Graphics.DrawString("...", Font, 
				Enabled ? SystemBrushes.WindowText : SystemBrushes.ControlDark, 
				boxRect, sf);

			Rectangle textRect = GetTextRect();

			if (RightToLeft == RightToLeft.Yes)
				sf = new StringFormat(StringFormatFlags.DirectionRightToLeft);
			else
				sf = new StringFormat();
			sf.LineAlignment = StringAlignment.Center;

			string text = _value == null ? Text : _value.ToString();

			if (Focused)
			{
				e.Graphics.FillRectangle(SystemBrushes.Highlight, textRect);

				ControlPaint.DrawFocusRectangle(e.Graphics, textRect, SystemColors.WindowFrame,
					SystemColors.Highlight);

				e.Graphics.DrawString(text, Font, SystemBrushes.HighlightText, textRect, sf);
			}
			else
				e.Graphics.DrawString(text, Font, 
					Enabled ? SystemBrushes.WindowText : SystemBrushes.ControlDark, 
					textRect, sf);
		}

		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus (e);

			Refresh();
		}

		protected override void OnLostFocus(EventArgs e)
		{
			base.OnLostFocus (e);
				
			buttonDown = false;

			Refresh();
		}


		bool buttonDown = false;

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown (e);

			if (e.Button == MouseButtons.Left)
			{
				Rectangle rect = GetBoxRect();
				if (e.X >= rect.Left && e.X <= rect.Right && 
					e.Y >= rect.Top && e.Y <= rect.Bottom)
				{
					buttonDown = true;
					Refresh();
				}

				if (!Focused)
					Focus();
			}
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp (e);

			if (buttonDown && e.Button == MouseButtons.Left)
			{
				buttonDown = false;
				Refresh();

				Rectangle rect = GetBoxRect();
				if (e.X >= rect.Left && e.X <= rect.Right && 
					e.Y >= rect.Top && e.Y <= rect.Bottom)
				{
					OnClick(EventArgs.Empty);

					OnSelectValue();
				}
			}
		}

		protected override bool IsInputKey(Keys keyData)
		{
			if (keyData == Keys.Enter)
				return true;
			return base.IsInputKey (keyData);
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				OnSelectValue();
			}

			base.OnKeyDown (e);
		}

		public event EventHandler ValueSelect;

		private void OnSelectValue()
		{
			if (ValueSelect != null)
				ValueSelect(this, EventArgs.Empty);

			if (_valueSelector != null)
			{
				object newVal = _valueSelector(this, _value);
				if (newVal != _value)
				{
					_value = newVal;
					if (ValueChanged != null)
						ValueChanged(this, EventArgs.Empty);
				}
				Refresh();
			}
		}

	}
}