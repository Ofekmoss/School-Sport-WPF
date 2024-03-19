using System;

namespace Sport.UI.Controls
{
	public enum TextInput
	{
		Arrow,
		Page,
		Wheel,
		Spin
	}

	public interface ITextController
	{
		bool Validate(ref string text, ref int position);
		object GetValue(string text);
		string SetValue(object value);
		bool HandleInput(TextInput input, bool up, ref string text, 
			ref int position, ref int length);
	}

	public class LetterNumberController : ITextController
	{
		private Sport.Common.LetterNumberFormat _letterNumberFormat;
		public Sport.Common.LetterNumberFormat LetterNumberFormat
		{
			get { return _letterNumberFormat; }
		}

		private int _min;
		public int Min
		{
			get { return _min; }
		}

		private int _max;
		public int Max
		{
			get { return _max; }
		}

		public LetterNumberController()
			: this(new Sport.Common.HebrewNumberFormat())
		{
		}

		public LetterNumberController(int min, int max)
			: this(min, max, new Sport.Common.HebrewNumberFormat())
		{
		}

		public LetterNumberController(int min, int max, Sport.Common.LetterNumberFormat letterNumberFormat)
		{
			_min = min;
			_max = max;
			_letterNumberFormat = letterNumberFormat;
		}

		public LetterNumberController(Sport.Common.LetterNumberFormat letterNumberFormat)
			: this(0, 0, letterNumberFormat)
		{
		}

		#region ITextController Members

		public bool Validate(ref string text, ref int position)
		{
			if (text == null || text.Length == 0)
				return true;

			int n = _letterNumberFormat.Parse(text);
			return n != 0 && n >= _min && (n <= _max || _max == 0);
		}

		public object GetValue(string text)
		{
			if (text == null || text.Length == 0)
				return 0;
			return _letterNumberFormat.Parse(text);
		}

		public string SetValue(object value)
		{
			if (value == null)
				return null;
			return _letterNumberFormat.ToString((int) value);
		}

		public bool HandleInput(Sport.UI.Controls.TextInput input, bool up, ref string text, ref int position, ref int length)
		{
			int val = (int) GetValue(text);
			if (up)
				val++;
			else
				val--;

			if (val < _min)
				val = _min;
			if (val > _max)
				val = _max;

			text = _letterNumberFormat.ToString(val);
			position = text.Length;
			length = 0;

			return true;
		}

		#endregion
	}


	public class RegularExpressionController : ITextController
	{
		public RegularExpressionController(string pattern)
		{
			Pattern = pattern;
		}

		public RegularExpressionController()
		{
		}

		private System.Text.RegularExpressions.Regex regex;

		private string _pattern;
		public string Pattern
		{
			get { return _pattern; }
			set 
			{ 
				_pattern = value; 
				if (_pattern == null)
					regex = null;
				else
					regex = new System.Text.RegularExpressions.Regex(_pattern);
			}
		}

		#region ITextController Members

		public virtual bool Validate(ref string text, ref int position)
		{
			if (regex == null)
				return false;

			System.Text.RegularExpressions.Match match = regex.Match(text);
			return match != null && match.Length == text.Length;
		}

		public virtual object GetValue(string text)
		{
			return text;
		}

		public virtual string SetValue(object value)
		{
			if (value == null)
				return null;

			string text = value.ToString();

			int p = 0;

			if (Validate(ref text, ref p))
				return text;

			return null;
		}

		public virtual bool HandleInput(TextInput input, bool up, ref string text, 
			ref int position, ref int length)
		{
			return false;
		}

		#endregion
	}

	public class NumberController : RegularExpressionController
	{
		/// <summary>
		/// how much the arrow affects the value.
		/// </summary>
		private int _arrowValue=1;
		public int ArrowValue
		{
			get { return _arrowValue; }
			set { _arrowValue = value; }
		}
		
		public NumberController(double min, double max, byte scale, byte precision)
		{
			_scale = scale;
			_precision = precision;
			_min = min;
			_max = max;
			ResetPattern();
		}

		public NumberController(double min, double max)
			: this(min, max, byte.MaxValue, byte.MaxValue)
		{
		}

		public NumberController(byte scale, byte precision)
			: this(double.MinValue, double.MaxValue, scale, precision)
		{
		}

		public NumberController()
			: this(byte.MaxValue, byte.MaxValue)
		{
		}

        private double _min;
		public double Min
		{
			get { return _min; }
			set 
			{ 
				_min = value; 
				ResetPattern();
			}
		}

		private double _max;
		public double Max
		{
			get { return _max; }
			set 
			{ 
				_max = value; 
				ResetPattern();
			}

		}

		private byte _scale;
		public byte Scale
		{
			get { return _scale; }
			set
			{
				_scale = value;
				ResetPattern();
			}
		}

		private byte _precision;
		public byte Precision
		{
			get { return _precision; }
			set
			{
				_precision = value;
				ResetPattern();
			}
		}

		private void ResetPattern()
		{
			string pattern = _min < 0 ? "-?" : "";
			if (_scale > 0)
				pattern += "\\d{0," + _scale.ToString() + "}";
			if (_precision > 0)
				pattern += "(\\.\\d{0," + _precision.ToString() + "})?";
			base.Pattern = pattern;
		}

		public new string Pattern
		{
			get
			{
				return base.Pattern;
			}
		}

		#region ITextController Members

		public override object GetValue(string text)
		{
			try
			{
				return Double.Parse(text);
			}
			catch
			{
			}

			return 0d;
		}

		public override string SetValue(object value)
		{
			if (value is double ||
				value is float ||
				value is int ||
				value is byte)
			{
				string s = value.ToString();
				int p = 0;

				if (!Validate(ref s, ref p))
					return null;
				return s;
			}
			
			return null;
		}

		public override bool HandleInput(TextInput input, bool up, ref string text, 
			ref int position, ref int length)
		{
			if (_scale == 0 && _precision == 0)
				return false;

			double small;
			double large;
			if (_scale > 0)
			{
				small = this._arrowValue;
				if (_scale == 1)
				{
					large = 1;
				}
				else
				{
					large = 10;
				}
			}
			else
			{
				small = ((double) _arrowValue/(double) 10);
				large = 0.1;
			}


			double value;

			if (text == null || text.Length == 0)
			{
				value = up ? _min : _max;
			}
			else
			{
				value = (double) GetValue(text);

				switch (input)
				{
					case (TextInput.Page):
						value += large * (up ? 1 : -1);
						break;
					default:
						value += small * (up ? 1 : -1);
						break;
				}
			}

			string t = value.ToString();
			if (!Validate(ref t, ref position))
				return false;

			text = t;
			position = text.Length;
			length = 0;

			return true;
		}

		public override bool Validate(ref string text, ref int position)
		{
			//regular expression acting too weird. can't use it.
			/* if (!base.Validate (ref text, ref position))
				return false; */
			
			if (text.Length == 0)
				return true;
			
			double value = (double) GetValue(text);
			if (value < _min || value > _max)
				return false;

			return true;
		}


		#endregion
	}

	public class TextControl : ControlWrapper
	{
		#region TextBox Properties

		public bool ReadOnly
		{
			get { return textBox.ReadOnly; }
			set { textBox.ReadOnly = value; }
		}

		#endregion

		private ITextController _controller;
		public ITextController Controller
		{
			get { return _controller; }
			set { _controller = value; }
		}

		public object Value
		{
			get 
			{ 
				string text = Text;
				if (_controller == null)
					return text;
				if (text == null || text.Length == 0)
					return null;
				return _controller.GetValue(text); 
			}
			set 
			{ 
				if (_controller == null)
				{
					if (value == null)
						textBox.Text = null;
					else
						textBox.Text = value.ToString();
				}
				else
				{
					textBox.Text = _controller.SetValue(value);
				}
			}
		}

		private void HandleInput(TextInput input, bool up)
		{
			if (_controller == null || ReadOnly)
				return ;

			string text = textBox.Text;
			int position = textBox.SelectionStart;
			int length = textBox.SelectionLength;

			if (_controller.HandleInput(input, up, ref text, ref position, ref length))
			{
				textBox.Text = text;
				textBox.SelectionStart = position;
				textBox.SelectionLength = length;
			}
		}
	
		private class TextSpin : System.Windows.Forms.Control
		{
			private TextControl _textControl;

			private System.Windows.Forms.Timer timer;

			public TextSpin(TextControl textControl)
			{
				_textControl = textControl;
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
						}
						else
						{
							downPushed = true;
						}

						_textControl.HandleInput(TextInput.Spin, upPushed);

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
					_textControl.HandleInput(TextInput.Spin, upPushed);
				}
			}

			protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
			{
				int h = Height / 2;
				System.Windows.Forms.ButtonState bs = System.Windows.Forms.ButtonState.Normal;
				if (_textControl.BorderStyle != System.Windows.Forms.BorderStyle.Fixed3D)
					bs |= System.Windows.Forms.ButtonState.Flat;

				System.Windows.Forms.ControlPaint.DrawScrollButton(e.Graphics, 0, 0, Width, h, 
					System.Windows.Forms.ScrollButton.Up, 
					upPushed ? bs | System.Windows.Forms.ButtonState.Pushed : bs);
				System.Windows.Forms.ControlPaint.DrawScrollButton(e.Graphics, 0, h, Width, Height - h, 
					System.Windows.Forms.ScrollButton.Down, 
					downPushed ? bs | System.Windows.Forms.ButtonState.Pushed : bs);
			}
		}

		private TextSpin spin;
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
		static public explicit operator System.Windows.Forms.TextBox(TextControl tc)
		{
			return tc.textBox;
		}

		//static public implicit operator TextControl(System.Windows.Forms.TextBox value) 

		/*
		public System.Windows.Forms.TextBox TxtBox
		{
			get {return textBox;}
		}
		*/


		public TextControl()
		{
			textBox = new System.Windows.Forms.TextBox();
			textBox.AutoSize = false;
			textBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			textBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			textBox.KeyDown += new System.Windows.Forms.KeyEventHandler(textBox_KeyDown);
			textBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(textBox_KeyPress);
			textBox.MouseWheel += new System.Windows.Forms.MouseEventHandler(textBox_MouseWheel);
			textBox.LostFocus += new EventHandler(textBox_LostFocus);
			textBox.GotFocus += new EventHandler(textBox_GotFocus);
			textBox.TextChanged += new EventHandler(textBox_TextChanged);

			base.TabStop = false;

			_borderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			showSpin = false;
			spin = new TextSpin(this);
			spin.Visible = false;
			Controls.Add(textBox);
			Controls.Add(spin);

			Size = new System.Drawing.Size(60, 20);
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

		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus (e);
			textBox.Focus();
			
			if ((this.showSpin)&&(this.Text != null)&&(this.Text.Length > 0))
				textBox.SelectAll();
		}


		protected override void OnRightToLeftChanged(EventArgs e)
		{
			base.OnRightToLeftChanged (e);

			ResetSize();
		}



		private void textBox_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
		//	if (KeyListener.HandleEvent(this, e))
		//		e.Handled = true;

			if (!ReadOnly)
			{
				if (e.KeyCode == System.Windows.Forms.Keys.Delete &&
					_controller != null)
				{
					e.Handled = true;
					string text = textBox.Text;
					int start = textBox.SelectionStart;
					int pos = start;
					int length = textBox.SelectionLength;
					if (length == 0 && start < text.Length)
						length = 1;

					text = text.Remove(start, length);
					if (_controller.Validate(ref text, ref pos))
					{
						textBox.Text = text;
						textBox.SelectionLength = 0;
						textBox.SelectionStart = pos;
					}
				}
				else
				{
					switch (e.KeyCode)
					{
						case (System.Windows.Forms.Keys.Up):
							HandleInput(TextInput.Arrow, true);
							break;
						case (System.Windows.Forms.Keys.Down):
							HandleInput(TextInput.Arrow, false);
							break;
						case (System.Windows.Forms.Keys.PageUp):
							HandleInput(TextInput.Page, true);
							break;
						case (System.Windows.Forms.Keys.PageDown):
							HandleInput(TextInput.Page, false);
							break;
						default:
							base.OnKeyDown (e);
							return ;
					}

					e.Handled = true;
				}
			}

			OnKeyDown(new System.Windows.Forms.KeyEventArgs(e.KeyData));
		}

		private void textBox_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			if (!ReadOnly)
			{
				if (_controller != null)
				{
					e.Handled = true;

					if (e.KeyChar == (char) System.Windows.Forms.Keys.Back ||
						!Char.IsControl(e.KeyChar))
					{
						string text = textBox.Text;
						int start = textBox.SelectionStart;
						int pos = start;
						int length = textBox.SelectionLength;
						if (length > 0)
							text = text.Remove(start, length);

						if (e.KeyChar == (char) System.Windows.Forms.Keys.Back)
						{
							if (start > 0)
							{
								text = text.Remove(start - 1, 1);
								pos = start - 1;
							}
						}
						else
						{
							text = text.Insert(start, e.KeyChar.ToString());
							pos = start + 1;
						}

						if (_controller.Validate(ref text, ref pos))
						{
							textBox.Text = text;
							textBox.SelectionLength = 0;
							textBox.SelectionStart = pos;
						}
					}
				}
			}

			OnKeyPress(new System.Windows.Forms.KeyPressEventArgs(e.KeyChar));
		}

		private void textBox_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (e.Delta > 0)
				HandleInput(TextInput.Wheel, true);
			else
				HandleInput(TextInput.Wheel, false);

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

		public override string Text
		{
			get
			{
				return base.Text;
			}
			set
			{
				if (_controller != null)
				{
					int pos = value.Length;
					if (_controller.Validate(ref value, ref pos))
					{
						textBox.Text = value;
						textBox.SelectionStart = pos;
						textBox.SelectionLength = 0;
					}
				}
				else
				{
					textBox.Text = value;
				}
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
