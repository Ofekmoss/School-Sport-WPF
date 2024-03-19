using System;
using System.Collections;
using System.Windows.Forms;
using Sport.Common;

namespace Sport.UI.Controls
{
	public class TextItemsControl : ControlWrapper
	{
		#region TextBox Properties

		public bool ReadOnly
		{
			get { return textBox.ReadOnly; }
			set { textBox.ReadOnly = value; }
		}

		public int SelectionStart
		{
			get { return textBox.SelectionStart; }
			set { textBox.SelectionStart = value; }
		}

		public int SelectionLength
		{
			get { return textBox.SelectionLength; }
			set { textBox.SelectionLength = value; }
		}

		public override bool Focused
		{
			get
			{
				return base.Focused || textBox.Focused;
			}
		}

		#endregion

		#region Spin control

		private class Spin : System.Windows.Forms.Control
		{
			private TextItemsControl _textItemsControl;

			private System.Windows.Forms.Timer timer;

			public Spin(TextItemsControl textItemsControl)
			{
				_textItemsControl = textItemsControl;
				TabStop = false;

				timer = new System.Windows.Forms.Timer();
				timer.Tick += new EventHandler(timer_Tick);
				timer.Interval = 500;
			}

			private bool upPushed = false;
			private bool downPushed = false;

			protected override void OnLostFocus(EventArgs e)
			{
				base.OnLostFocus (e);

				timer.Stop();
				upPushed = false;
				downPushed = false;
			}

			protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
			{
				base.OnMouseUp (e);

				timer.Stop();
				upPushed = false;
				downPushed = false;
				Refresh();
			}

			protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
			{
				base.OnMouseDown (e);

				upPushed = false;
				downPushed = false;

				if (e.X >= 0 && e.X < Width)
				{
					if (e.Y >= 0 && e.Y < Height)
					{
						if (e.Y <= Height / 2)
						{
							upPushed = true;
							_textItemsControl.SpinItem(1);
						}
						else
						{
							downPushed = true;
							_textItemsControl.SpinItem(-1);
						}


						timer.Interval = 500;
						timer.Start();
						Refresh();
					}
				}
			}

			private void timer_Tick(object sender, EventArgs e)
			{
				if (!upPushed && !downPushed)
				{
					timer.Stop();
				}
				else
				{
					if (timer.Interval == 500)
						timer.Interval = 50;

					if (upPushed)
						_textItemsControl.SpinItem(1);
					else
						_textItemsControl.SpinItem(-1);
				}
			}

			protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
			{
				int h = Height / 2;
				System.Windows.Forms.ButtonState bs = System.Windows.Forms.ButtonState.Normal;
				if (_textItemsControl.BorderStyle != System.Windows.Forms.BorderStyle.Fixed3D)
					bs |= System.Windows.Forms.ButtonState.Flat;

				System.Windows.Forms.ControlPaint.DrawScrollButton(e.Graphics, 0, 0, Width, h, 
					System.Windows.Forms.ScrollButton.Up, 
					upPushed ? bs | System.Windows.Forms.ButtonState.Pushed : bs);
				System.Windows.Forms.ControlPaint.DrawScrollButton(e.Graphics, 0, h, Width, Height - h, 
					System.Windows.Forms.ScrollButton.Down, 
					downPushed ? bs | System.Windows.Forms.ButtonState.Pushed : bs);
			}
		}

		#endregion

		#region TextItem class

		public class TextItem
		{
			private bool _editable;
			public bool Editable
			{
				get { return _editable; }
				set { _editable = value; }
			}

			private bool _overwrite;
			public bool Overwrite
			{
				get { return _overwrite; }
				set 
				{ 
					_overwrite = value;

					if (!_overwrite && _maximumLength < _text.Length)
					{
						_text = _text.Substring(0, _maximumLength);
					}
				}
			}

			private void SetPlaceholder(char placeholder)
			{
				_text = _text.Replace(_placeholder, placeholder);
				_placeholder = placeholder;
			}

			private char _placeholder;
			public char Placeholder
			{
				get { return _placeholder; }
				set { SetPlaceholder(value); }
			}

			private int _maximumLength; // Used only for non-overwrite items
			public int MaximumLength
			{
				get { return _maximumLength; }
				set 
				{
					if (_maximumLength < 1)
						throw new Exception("Maximum length must be greater than 0");

					_maximumLength = value; 

					if (!_overwrite && _text.Length > _maximumLength)
					{
						_text = _text.Substring(0, _maximumLength);
					}
				}
			}

			protected string _text;
			public string Text
			{
				get { return _text; }
				set { _text = value == null ? _placeholder.ToString() : value; }
			}

			public int Length
			{
				get { return _text.Length; }
			}

			private void SetAccepted(string accepted)
			{
				if (_editable)
				{
					_accepted = accepted;
					if (_accepted == null)
						_accepted = "";

					if (_overwrite)
					{
						char[] chars = _text.ToCharArray();
						for (int n = 0; n < _text.Length; n++)
							if (_accepted.IndexOf(chars[n]) < 0)
								chars[n] = _placeholder;

						_text = new String(chars);
					}
					else
					{
						int n = 0;
						while (n < _text.Length)
						{
							if (_accepted.IndexOf(_text[n]) < 0)
								_text = _text.Remove(n, 1);
							else
								n++;
						}

						if (_text.Length == 0)
							_text = _placeholder.ToString();
					}
				}
			}

			private string _accepted;
			public string Accepted
			{
				get { return _accepted; }
				set { SetAccepted(value); }
			}

			public TextItem(string text, bool editable, bool overwrite, char placeholder, string accepted)
			{
				_editable = editable;
				_overwrite = overwrite;
				_placeholder = placeholder;
				_text = text == null ? placeholder.ToString() : text;
				SetAccepted(accepted);
			}

			public TextItem(string text)
				: this(text, false, true, '_', null)
			{
			}

			public virtual bool Spin(int value)
			{
				return false;
			}

			public virtual bool Insert(char c, ref int position)
			{
				if (position > _text.Length)
					throw new Exception("Position must be lower or equal to the item length");

				if (!_editable)
					return false; // not editable - insert not handled

				if (_overwrite)
				{
					if (position == _text.Length)
						return false; // on overwrite at the end - insert not handled

					if (_accepted.IndexOf(c) >= 0)
					{
						_text = _text.Remove(position, 1).Insert(position, c.ToString());
						position++;
					}
						
					// insert handled (also if no character was set)
				}
				else
				{
					if (_text.Length == _maximumLength)
					{
						// Cannot insert any more characters..

						if (position == _text.Length) // if in the end of the item -
							return false; // Insert was not handled

						// Insert was handled
					}

					if (_accepted.IndexOf(c) < 0)
					{
						return position < _text.Length; // if at end of item, insert
						// was not handled
					}

					if (_text == _placeholder.ToString())
					{
						// The string is empty - set to inserted character
						_text = c.ToString();
						position = 1;
						// Insert was handled
					}
					else
					{
						_text = _text.Insert(position, c.ToString());
						position++;
						// Insert was handled
					}
				}

				return true;
			}

			public virtual bool Back(ref int position)
			{
				if (position > _text.Length)
					throw new Exception("Position must be lower or equal to the item length");

				if (!_editable || position == 0)
					return false;

				position--;
				if (_overwrite)
				{
					char[] chars = _text.ToCharArray();
					chars[position] = _placeholder;
					_text = new String(chars);
				}
				else
				{
					_text = _text.Remove(position, 1);
					if (_text.Length == 0)
						_text = _placeholder.ToString();
				}

				return true;
			}

			public virtual bool Delete()
			{
				if (!_editable)
					return false;

				if (_overwrite)
				{
					char[] chars = _text.ToCharArray();
					for (int n = 0; n < _text.Length; n++)
						chars[n] = _placeholder;
					_text = new String(chars);
				}
				else
				{
					_text = _placeholder.ToString();
				}

				return true;
			}
		}

		#endregion

		#region NumberTextItem

		public class NumberTextItem : TextItem
		{
			private int _min;
			private int _max;
			public NumberTextItem(string text, bool overwrite, int min, int max)
				: base(text, true, overwrite, '0', min < 0 ? "-0123456789" : "0123456789")
			{
				if (min > max)
					throw new Exception("Minimum value must be lower than maximum value");
				_min = min;
				_max = max;
			}

			public int Number
			{
				get { return Int32.Parse(_text); }
				set 
				{ 
					if (value < _min)
						SetNumber(_min);
					else if (value > _max)
						SetNumber(_max);
					else
						SetNumber(value);
				}
			}

			private void Validate()
			{
				int n = _text == "-" ? 0 : Int32.Parse(_text);

				if (n < _min)
					SetNumber(_min);
				else if (n > _max)
					SetNumber(_max);
			}

			private void SetNumber(int number)
			{
				string s = number.ToString();

				if (Overwrite)
				{
					char[] chars = _text.ToCharArray();

					int dif = _text.Length - s.Length;
					if (dif < 0)
						dif = 0;

					for (int i = 0; i < dif; i++)
						chars[i] = '0';

					for (int i = dif; i < chars.Length; i++)
						chars[i] = s[i - dif];

					_text = new String(chars);
				}
				else
				{
					_text = s;
				}
			}

			public override bool Insert(char c, ref int position)
			{
				if (c == '-' && position != 0) // '-' can be inserted only at the start
					return position < _text.Length; // If not positioned in the end - the insert was handled

				int minusSign = _text.IndexOf('-');
				if (minusSign >= position)
					return true;

				bool b = base.Insert (c, ref position);

				Validate();

				return b;
			}

			public override bool Delete()
			{
				bool b = base.Delete ();

				Validate();

				return b;
			}

			public override bool Back(ref int position)
			{
				bool b = base.Back (ref position);

				Validate();
				
				return b;
			}

			public override bool Spin(int value)
			{
				Number += value;

				return true;
			}

		}

		#endregion
        
		#region ValueFormatter Functions

		public void SetFormatterItems(ValueFormatter valueFormatter)
		{
			ArrayList al = new ArrayList();
			int lastIndex = 0;
			if (valueFormatter == null || valueFormatter.FormatItems == null)
			{
				return;
			}
			for (int n = 0; n < valueFormatter.FormatItems.Length; n++)
			{
				if (lastIndex < valueFormatter.FormatItems[n].BaseIndex)
				{
					al.Add(new TextItemsControl.TextItem(valueFormatter.BaseText.Substring(lastIndex,
						valueFormatter.FormatItems[n].BaseIndex - lastIndex)));
					lastIndex = valueFormatter.FormatItems[n].BaseIndex;
				}

				string s = "0";
				for (int i = 0; i < valueFormatter.FormatItems[n].Formatter.Length - 1; i++)
					s += '0';

				al.Add(new TextItemsControl.NumberTextItem(s, 
					valueFormatter.FormatItems[n].Formatter.Length > 0, 0,
					valueFormatter.FormatItems[n].Formatter.Maximum));
			}

			if (lastIndex < valueFormatter.BaseText.Length)
				al.Add(new TextItemsControl.TextItem(valueFormatter.BaseText.Substring(lastIndex)));

			Items = (TextItemsControl.TextItem[]) al.ToArray(typeof(TextItemsControl.TextItem));
		}

		public int GetValue(ValueFormatter valueFormatter)
		{
			int[] values = new int[valueFormatter.FormatItems.Length];

			int lastIndex = 0;
			int index = 0;
			for (int n = 0; n < valueFormatter.FormatItems.Length; n++)
			{
				if (valueFormatter.FormatItems[n].BaseIndex > lastIndex)
				{
					index++;
					lastIndex = valueFormatter.FormatItems[n].BaseIndex;
				}

				values[n] = ((TextItemsControl.NumberTextItem) _items[index]).Number;
				index++;
			}

			return valueFormatter.GetValue(values);
		}

		public void SetValue(ValueFormatter valueFormatter, int value)
		{
			int[] values = valueFormatter.GetValues(value);

			int lastIndex = 0;
			int index = 0;
			for (int n = 0; n < valueFormatter.FormatItems.Length; n++)
			{
				if (valueFormatter.FormatItems[n].BaseIndex > lastIndex)
				{
					index++;
					lastIndex = valueFormatter.FormatItems[n].BaseIndex;
				}

				((TextItemsControl.NumberTextItem) _items[index]).Number = values[n];
				index++;
			}

			RefreshText();
		}


		#endregion

		private TextItem[] _items;
		public TextItem[] Items
		{
			get { return _items; }
			set { _items = value; RefreshText(); }
		}


		private int GetItemPosition(int item)
		{
			int pos = 0;

			for (int n = 0; n < item && n < _items.Length; n++)
			{
				pos += _items[n].Length;
			}

			return pos;
		}

		private int GetPositionItem(int position)
		{
			int pos = 0;
			
			for (int n = 0; n < _items.Length; n++)
			{
				pos += _items[n].Length;
				if (position <= pos)
					return n;
			}

			return -1;
		}

		public void RefreshText()
		{
			int pos = textBox.SelectionStart;
			string s = "";
			for (int n = 0; n < _items.Length; n++)
			{
				s += _items[n].Text;
			}
			textBox.Text = s;
			if (pos != -1)
				textBox.SelectionStart = pos;
		}

		private void SpinItem(int value)
		{
			int item = GetPositionItem(textBox.SelectionStart);
			if (item != -1)
			{
				if (!_items[item].Spin(value))
				{
					if (item < _items.Length - 1)
						_items[item + 1].Spin(value);
				}
			}

			RefreshText();
		}

		private Spin spin;
		private bool showSpin;
		public bool ShowSpin
		{
			get { return showSpin; }
			set
			{
				showSpin = value;
				spin.Visible = showSpin;
				ResetSize();
			}
		}

		private static readonly int spinWidth = 12;

		private System.Windows.Forms.TextBox textBox;
		/// <summary>
		/// allow explicit cast from TextControl to TextBox
		/// </summary>
		static public explicit operator System.Windows.Forms.TextBox(TextItemsControl tic)
		{
			return tic.textBox;
		}


		public TextItemsControl()
		{
			textBox = new System.Windows.Forms.TextBox();
			textBox.AutoSize = false;
			textBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			textBox.KeyDown += new System.Windows.Forms.KeyEventHandler(textBox_KeyDown);
			textBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(textBox_KeyPress);
			textBox.MouseWheel += new System.Windows.Forms.MouseEventHandler(textBox_MouseWheel);
			textBox.LostFocus += new EventHandler(textBox_LostFocus);
			textBox.GotFocus += new EventHandler(textBox_GotFocus);
			textBox.TextChanged += new EventHandler(textBox_TextChanged);

			base.TabStop = false;

			showSpin = false;
			spin = new Spin(this);
			spin.Visible = false;
			Controls.Add(textBox);
			Controls.Add(spin);

			Size = new System.Drawing.Size(60, 20);

			_items = new TextItem[0];

			RefreshText();
		}

		public new bool TabStop
		{
			get
			{
				return textBox.TabStop; 
			}
			set
			{
				textBox.TabStop = value;
			}
		}

		private int BorderSize
		{
			get
			{
				int border = 0;
				if (_borderStyle == System.Windows.Forms.BorderStyle.Fixed3D)
					border = 2;
				else if (_borderStyle == System.Windows.Forms.BorderStyle.FixedSingle)
					border = 1;
				return border;
			}
		}

		private System.Windows.Forms.BorderStyle _borderStyle;
		public System.Windows.Forms.BorderStyle BorderStyle
		{
			get { return _borderStyle; }
			set
			{
				_borderStyle = value;
				ResetSize();
			}
		}

		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
		{
			base.OnPaint (e);

			if (_borderStyle == System.Windows.Forms.BorderStyle.Fixed3D)
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
			else if (_borderStyle == System.Windows.Forms.BorderStyle.FixedSingle)
			{
				e.Graphics.DrawRectangle(System.Drawing.SystemPens.ControlDarkDark,
					0, 0, Width - 1, Height - 1);
			}
		}

		private void ResetSize()
		{
			int border = BorderSize;
			int height = Height - (border * 2);
			int tbLeft = border;
			int tbWidth = Width - (border * 2);
			if (showSpin)
			{
				tbWidth -= spinWidth;
				if (RightToLeft == System.Windows.Forms.RightToLeft.Yes)
				{
					spin.Bounds = new System.Drawing.Rectangle(border, border, spinWidth, height);
					tbLeft += spinWidth;
				}
				else
				{
					spin.Bounds = new System.Drawing.Rectangle(Width - border - spinWidth, border, spinWidth, height);
				}
			}
			textBox.Bounds = new System.Drawing.Rectangle(tbLeft, border, tbWidth, height);

			Refresh();
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

		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus (e);

			textBox.Focus();
		}


		private void textBox_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (!ReadOnly)
			{
				if (e.KeyCode == System.Windows.Forms.Keys.Delete)
				{
					e.Handled = true;

					int start = textBox.SelectionStart;
					int end = textBox.SelectionStart + textBox.SelectionLength;

					int firstItem = GetPositionItem(start);
					if (firstItem != -1)
					{
						int lastItem = GetPositionItem(end);
						if (lastItem == -1)
							lastItem = _items.Length - 1;

						if (firstItem == lastItem)
						{
							while (firstItem < _items.Length && 
								!_items[firstItem].Delete())
								firstItem++;
							lastItem = firstItem;
						}
						else
						{
							for (int item = firstItem; item <= lastItem; item++)
							{
								_items[item].Delete();
							}
						}

						int pos = GetItemPosition(lastItem + 1);
						
						RefreshText();
						textBox.SelectionLength = 0;
						textBox.SelectionStart = pos;
					}
				}
				else
				{
					switch (e.KeyCode)
					{
						case (System.Windows.Forms.Keys.Up):
							SpinItem(1);
							e.Handled = true;
							break;
						case (System.Windows.Forms.Keys.Down):
							SpinItem(-1);
							e.Handled = true;
							break;
					}
				}
			}

			OnKeyDown(new System.Windows.Forms.KeyEventArgs(e.KeyData));
		}

		private void textBox_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			if (!ReadOnly)
			{
				e.Handled = true;

				int start = textBox.SelectionStart;
				int end = textBox.SelectionStart + textBox.SelectionLength;
				int firstItem = GetPositionItem(start);
				int lastItem = GetPositionItem(end);

				if (e.KeyChar == (char) System.Windows.Forms.Keys.Back)
				{
					if (firstItem == -1)
						firstItem = _items.Length - 1;
					if (lastItem == -1)
						lastItem = _items.Length - 1;

					int lastPos = GetItemPosition(lastItem);
					int firstPos = GetItemPosition(firstItem);
					if (start <= firstPos && firstItem > 0)
					{
						firstItem--;
						firstPos -= _items[firstItem].Length;
					}

					int digit = end - lastPos;

					int item = lastItem;
					while (item > firstItem)
					{
						while (!_items[item].Back(ref digit) && item > 0)
						{
							item--;
							digit = _items[item].Length;
						}
					}

					int pos = start;
					item = firstItem;
					while (pos >= start && pos > 0)
					{
						while (!_items[item].Back(ref digit) && item > 0)
						{
							item--;
							pos = firstPos;
							digit = _items[item].Length;
							firstPos -= digit;
						}
						pos = firstPos + digit;
					}

					RefreshText();
					textBox.SelectionLength = 0;
					textBox.SelectionStart = firstPos + digit;
				}
				else
				{
					if (firstItem != -1)
					{
						for (int item = firstItem + 1; item <= lastItem; item++)
						{
							_items[item].Delete();
						}

						int itemPos = GetItemPosition(firstItem);
						int digit = start - itemPos;

						while (firstItem < _items.Length && 
							!_items[firstItem].Insert(e.KeyChar, ref digit))
						{
							digit = 0;
							itemPos += _items[firstItem].Length;
							firstItem++;
						}

						RefreshText();
						textBox.SelectionLength = 0;
						textBox.SelectionStart = itemPos + digit;
					}
				}
			}

			OnKeyPress(new System.Windows.Forms.KeyPressEventArgs(e.KeyChar));
		}

		private void textBox_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			SpinItem(e.Delta / 120);

			base.OnMouseWheel (e);
		}

		private void textBox_LostFocus(object sender, EventArgs e)
		{
			OnLostFocus(e);
		}

		private void textBox_GotFocus(object sender, EventArgs e)
		{
			OnGotFocus(e);
		}

		public override string Text
		{
			get
			{
				return base.Text;
			}
		}


		private void textBox_TextChanged(object sender, EventArgs e)
		{
			base.Text = textBox.Text;
		}

		private void ControlKeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			MessageBox.Show(e.KeyCode.ToString());
		}
	}
}
