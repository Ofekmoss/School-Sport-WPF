using System;
using System.Drawing;
using System.Windows.Forms;

namespace Sport.UI.Controls
{
	public class NullComboBox : System.Windows.Forms.ComboBox
	{
		private class EmptyItem
		{
			public EmptyItem()
			{
			}

			public override string ToString()
			{
				return "";
			}
		}

		public static readonly object Null = new EmptyItem();

		private string _text;
		public new string Text
		{
			get { return _text; }
			set 
			{
				_text = value;
				Refresh();
			}
		}

		public NullComboBox()
		{
			base.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			base.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			base.Sorted = true;
			KeyDown += new KeyEventHandler(NullComboBox_KeyDown);
		}

		public new System.Windows.Forms.DrawMode DrawMode
		{
			get { return base.DrawMode; }
		}

		public new System.Windows.Forms.ComboBoxStyle DropDownStyle
		{
			get { return base.DropDownStyle; }
		}

		public new object SelectedItem
		{
			get 
			{ 
				object item = base.SelectedItem;
				return item == Null ? null : item;
			}
			set 
			{
				if (value != null)
				{
					int index = Items.IndexOf(value);
					if (index >= 0)
						base.SelectedIndex = index;
					else
						value = null;
				}

				if (value == null)
				{
					try
					{
						base.SelectedItem = Null;
						if (base.SelectedItem != Null)
							base.SelectedItem = null;
					}
					catch
					{
						base.SelectedItem = null;
					}
				}
			}
		}

		public new int SelectedIndex
		{
			get 
			{ 
				object item = base.SelectedItem;
				return item == Null ? -1 : base.SelectedIndex;
			}
			set { base.SelectedIndex = value; }
		}

		protected override void OnDrawItem(System.Windows.Forms.DrawItemEventArgs e)
		{
			object value = e.Index < 0 ? Null : Items[e.Index];

			System.Drawing.StringFormat sf = new System.Drawing.StringFormat(
				RightToLeft == System.Windows.Forms.RightToLeft.Yes ?
				System.Drawing.StringFormatFlags.DirectionRightToLeft :
				(System.Drawing.StringFormatFlags) 0);

			sf.Alignment = value == Null ? System.Drawing.StringAlignment.Center : System.Drawing.StringAlignment.Near;

			System.Drawing.Brush brush = System.Drawing.SystemBrushes.WindowText;

			if (!Enabled)
			{
				e.Graphics.FillRectangle(System.Drawing.SystemBrushes.Control, e.Bounds);
				brush = System.Drawing.SystemBrushes.ControlDark;
			}
			else if ((e.State & System.Windows.Forms.DrawItemState.Selected) != 0)
			{
				e.Graphics.FillRectangle(System.Drawing.SystemBrushes.Highlight, e.Bounds);
				brush = System.Drawing.SystemBrushes.HighlightText;
			}
			else
			{
				e.Graphics.FillRectangle(System.Drawing.SystemBrushes.Window, e.Bounds);

				if (value == Null)
					brush = System.Drawing.SystemBrushes.ControlDark;
			}

			System.Drawing.Rectangle rect = new System.Drawing.Rectangle(e.Bounds.X, 
				e.Bounds.Y + (e.Bounds.Height - e.Font.Height) / 2, e.Bounds.Width, e.Font.Height);

			if (value == Null)
			{
				e.Graphics.DrawString(Text, e.Font, 
					brush, rect, sf);
			}
			else
			{
				e.Graphics.DrawString(value.ToString(), e.Font, 
					brush, rect, sf);
			}
		}

		//private string _keyDownBuffer = "";
		private void NullComboBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyValue >= (int) 'A' && e.KeyValue <= (int) 'Z')
			{
				//char c = (char) ((int) 'à' + (e.KeyValue - ((int) 'A')));
				//_keyDownBuffer += c.ToString();
				//this.Set
				//e.Handled = true;
				//return;
			}
			
			//_keyDownBuffer = "";
			if (KeyListener.HandleEvent(this, e))
				e.Handled = true;
		}
	}
}
