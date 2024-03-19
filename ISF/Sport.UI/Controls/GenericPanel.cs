using System;

namespace Sport.UI.Controls
{
	public interface IGenericPanelLayout
	{
		// LayoutItems should return the needed height
		// for the panel
		int LayoutItems(GenericPanel panel);
	}

	/// <summary>
	/// Summary description for GenericPanel.
	/// </summary>
	public class GenericPanel : System.Windows.Forms.Panel, IGenericContainer
	{
		private GenericItemCollection _items;
		public GenericItemCollection Items
		{
			get { return _items; }
		}

		private IGenericPanelLayout _itemsLayout;
		public IGenericPanelLayout ItemsLayout
		{
			get { return _itemsLayout; }
			set
			{
				if (_itemsLayout != value)
				{
					_itemsLayout = value;
					ResetItems();
				}
			}
		}

		private int lastWidth;
		private int _itemsHeight;
		public int ItemsHeight
		{
			get { return _itemsHeight; }
		}

		private bool _autoSize;
		public override bool AutoSize
		{
			get { return _autoSize; }
			set
			{
				_autoSize = value;
				if (_autoSize)
					ResetItems();
			}
		}

		public GenericPanel()
		{
			_items = new GenericItemCollection(this);
			_itemsLayout = null;
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize (e);

			if (Width != lastWidth)
			{
				lastWidth = Width;
				ResetItems();
			}
		}

		protected override void OnRightToLeftChanged(EventArgs e)
		{
			base.OnRightToLeftChanged (e);
			ResetItems();
		}


		#region IGenericContainer Members

		public System.Windows.Forms.BorderStyle GenericItemBorder
		{
			get { return System.Windows.Forms.BorderStyle.Fixed3D; }
		}

		public void RemoveControl(GenericItem item, System.Windows.Forms.Control control)
		{
			Controls.Remove(control);
		}

		public void AddControl(GenericItem item, System.Windows.Forms.Control control)
		{
			Controls.Add(control);
		}

		public void OnValueChange(GenericItem item, object ov, object nv)
		{
		}

		public void ResetItems()
		{
			if (_itemsLayout != null)
			{
				
				_itemsHeight = _itemsLayout.LayoutItems(this);

				if (_autoSize)
				{
					Height = _itemsHeight;
				}

				OnLayoutChange();
			}
		}

		#endregion

		public event EventHandler LayoutChanged;

		private void OnLayoutChange()
		{
			if (LayoutChanged != null)
				LayoutChanged(this, EventArgs.Empty);
		}
		
	}

	public class GenericDefaultLayout : IGenericPanelLayout
	{
		public static readonly int DefaultSpace = 4;
		public static readonly int DefaultMargin = 4;
		public static readonly int DefaultLabelAlign = 20;

		private int _space;
		public int Space
		{
			get { return _space; }
			set { _space = value; }
		}

		private int _margin;
		public int Margin
		{
			get { return _margin; }
			set { _margin = value; }
		}

		private int _labelAlign;
		public int LabelAlign
		{
			get { return _labelAlign; }
			set { _labelAlign = value; }
		}

		public GenericDefaultLayout()
			: this(DefaultMargin)
		{
		}

		public GenericDefaultLayout(int margin)
			: this(margin, DefaultSpace)
		{
		}

		public GenericDefaultLayout(int margin, int space)
			: this(margin, space, DefaultLabelAlign)
		{
		}

		public GenericDefaultLayout(int margin, int space, int labelAlign)
		{
			_space = space;
			_margin = margin;
			_labelAlign = labelAlign;
		}



		private void PlaceControl(GenericPanel panel, System.Windows.Forms.Control control, int left, int top)
		{
			if (panel.RightToLeft == System.Windows.Forms.RightToLeft.Yes)
			{
				control.Location = new System.Drawing.Point(panel.Width - left - control.Width, top);
			}
			else
			{
				control.Location = new System.Drawing.Point(left, top);
			}
		}

		#region IGenericPanelLayout Members

		public int LayoutItems(GenericPanel panel)
		{
			int left = _margin;
			int top = _margin;
			int controlWidth;

			int tallest = 0;
			bool first = true;

			foreach (GenericItem item in panel.Items)
			{
				controlWidth = item.ControlSize.Width;

				if (item.Label != null)
				{
					if (!first && (item.Label.Width + left + _space + controlWidth + _margin > panel.Width))
					{
						left = _margin;
						top += tallest + _space;
						tallest = 0;
					}

					PlaceControl(panel, item.Label, left, top + _labelAlign - item.Label.Height);
					left += item.Label.Width + _space;
				}
				else
				{
					if (!first && (controlWidth + left + _margin > panel.Width))
					{
						left = _margin;
						top += tallest + _space;
						tallest = 0;
					}
				}

				if (left + controlWidth > panel.Width - _margin)
					item.Control.Width = panel.Width - _margin - left;
				else
					item.Control.Width = controlWidth;
				PlaceControl(panel, item.Control, left, top);
				left += item.Control.Width + _space;
				if (item.Control.Height > tallest)
					tallest = item.Control.Height;

				if (left > panel.Width)
				{
					left = _margin;
					top += tallest + _space;
					tallest = 0;
					first = true;
				}
				else
				{
					first = false;
				}
			}

			return top + tallest + _margin;
		}

		#endregion
	}
	
	public class GenericDetailsLayout : IGenericPanelLayout
	{
		public static readonly int DefaultSpace = 4;
		public static readonly int DefaultMargin = 4;

		private int _space;
		public int Space
		{
			get { return _space; }
			set { _space = value; }
		}

		private int _margin;
		public int Margin
		{
			get { return _margin; }
			set { _margin = value; }
		}

		public GenericDetailsLayout()
			: this(DefaultMargin)
		{
		}

		public GenericDetailsLayout(int margin)
			: this(margin, DefaultSpace)
		{
		}

		public GenericDetailsLayout(int margin, int space)
		{
			_margin = margin;
			_space = space;
		}



		private void PlaceControl(GenericPanel panel, System.Windows.Forms.Control control, int left, int top)
		{
			if (panel.RightToLeft == System.Windows.Forms.RightToLeft.Yes)
			{
				control.Location = new System.Drawing.Point(panel.Width - left - control.Width, top);
			}
			else
			{
				control.Location = new System.Drawing.Point(left, top);
			}
		}

		#region IGenericPanelLayout Members

		public int LayoutItems(GenericPanel panel)
		{
			int maxLabelWidth = 0;
			int maxControlWidth = 0;
			
			foreach (GenericItem item in panel.Items)
			{
				maxLabelWidth = (item.Label.Width > maxLabelWidth) ? item.Label.Width : maxLabelWidth;

				if (item.ControlSize.Width > maxControlWidth)
					maxControlWidth = item.ControlSize.Width;
			}

            int controlWidth = panel.Width - (_margin * 2) - maxLabelWidth - _space;
			if (controlWidth > maxControlWidth)
				controlWidth = maxControlWidth;

			int left = _margin;
			int top = _margin;

			foreach (GenericItem item in panel.Items)
			{
				if (item.Label != null)
				{
					PlaceControl(panel, item.Label, _margin, top);
					left = maxLabelWidth + _margin + _space;
				}

				item.Control.Width = controlWidth;
				PlaceControl(panel, item.Control, left, top);
				left = _margin;
				top = item.Control.Bottom + _space;
			}
			
			return top;
		}

		#endregion
	}

}
