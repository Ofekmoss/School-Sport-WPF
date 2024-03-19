using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using Sport.Common;

namespace Sport.UI.Controls
{
/*
	/// <summary>
	/// LayoutPanel implements a control containing a list of 
	/// layout items which are a control and a 
	/// label control (for caption display).
	/// The LayoutPanel places the controls in rows and columns,
	/// aligning the labels and captions of each column and
	/// resizing controls of each column to match width.
	/// </summary>
	public class LayoutPanel : ControlWrapper
	{

		private const int LabelSpacing = 5;
		private const int ControlSpacing = 8;
		private const int BorderSpacing = 4;

		public LayoutPanel()
		{
			autoSize = false;
			items = new LayoutItemCollection();
			items.Changed += new CollectionEventHandler(ItemChanged);
		}

		#region LayoutItem

		public sealed class LayoutItem
		{
			private Label			label;
			private Control		_control;
			private int			minimumWidth;
			internal LayoutPanel	owner;

			public Label Label
			{
				get { return label; }
			}

			public int MinimumWidth
			{
				get { return minimumWidth; }
				set { minimumWidth = value; }
			}

			public Control Control
			{
				get { return _control; }
				set 
				{ 
					if (_control != null && owner != null)
					{
						owner.Controls.Remove(_control);
					}

					_control = value; 

					if (_control != null && owner != null)
					{
						owner.Controls.Add(_control);
					}
				}
			}

			public string Caption
			{
				get
				{
					return label.Text;
				}
				set
				{
					label.Text = value;
					owner.ResetItems();
				}
			}

			public LayoutItem()
			{
				label = new Label();
				label.AutoSize = true;
				_control = null;
				minimumWidth = 100;
			}

			public LayoutItem(string caption, Control control)
			{
				label = new Label();
				label.AutoSize = true;
				label.Text = caption;
				_control = control;
				minimumWidth = _control.Width;
			}

			internal void Position(int left, int top, int labelWidth, int controlWidth)
			{
				_control.Width = controlWidth;

				if (owner.RightToLeft == RightToLeft.Yes)
				{
					//label.Location = new Point(owner.Width - left - label.Width, 
					//	top + _control.Height - label.Height);
					label.Location = new Point(owner.Width - left - label.Width, top);
					_control.Location = 
						new Point(owner.Width - left - labelWidth - controlWidth - LabelSpacing, top);

					label.Anchor = AnchorStyles.Right | AnchorStyles.Top;
					_control.Anchor = AnchorStyles.Right | AnchorStyles.Top;
				}
				else
				{
					label.Location = new Point(left, top);
					_control.Location = new Point(left + labelWidth + LabelSpacing, top);

					label.Anchor = AnchorStyles.Left | AnchorStyles.Top;
					_control.Anchor = AnchorStyles.Left | AnchorStyles.Top;
				}
			}
		}

		#endregion
		
		#region LayoutItemCollection Class

		public class LayoutItemCollection : GeneralCollection
		{
			public LayoutItem this[int index]
			{
				get { return (LayoutItem) GetItem(index); }
			}

			public void Insert(int index, LayoutItem value)
			{
				InsertItem(index, value);
			}

			public void Insert(int index, string caption, Control control)
			{
				InsertItem(index, new LayoutItem(caption, control));
			}

			public void Remove(LayoutItem value)
			{
				RemoveItem(value);
			}

			public void Remove(Control value)
			{
				foreach (LayoutItem li in this)
				{
					if (li.Control == value)
					{
						RemoveItem(li);
						return ;
					}
				}
			}

			public bool Contains(LayoutItem value)
			{
				return base.Contains(value);
			}

			public int IndexOf(LayoutItem value)
			{
				return base.IndexOf(value);
			}

			public int Add(LayoutItem value)
			{
				return AddItem(value);
			}

			public int Add(string caption, Control control)
			{
				return Add(new LayoutItem(caption, control));
			}
		}

		#endregion
		
		#region Collection

		private LayoutItemCollection items;

		public LayoutItemCollection Items
		{
			get { return items; }
		}

		private void ItemChanged(object sender, CollectionEventArgs e)
		{
			if (e.Old != null)
			{
				LayoutItem li = (LayoutItem) e.Old;
				li.owner = null;
				Controls.Remove(li.Label);
				if (li.Control != null)
					Controls.Remove(li.Control);
				ResetItems();
			}

			if (e.New != null)
			{
				LayoutItem li = (LayoutItem) e.New;
				if (li.owner == null)
				{
					li.owner = this;
					Controls.Add(li.Label);
					if (li.Control != null)
						Controls.Add(li.Control);
					ResetItems();
				}
				else
					items.RemoveAt(e.Index);
			}
		}

		#endregion

		#region Positioning

		// Stores size range to keep size
		int minWidth;
		int maxWidth;

		// Stores the needed height for all controls to be seen
		int neededHeight;

		// Stores wether to set height of control
		bool autoSize;

		public bool AutoSize
		{
			get { return autoSize; }
			set { autoSize = value; }
		}

		public int NeededHeight
		{
			get { return neededHeight; }
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize (e);

			if (Size.Width > maxWidth || Size.Width < minWidth)
				ResetItems();
		}

		private void ResetItems()
		{
			minWidth = 0;

			// Maximum width of each row
			int max = Size.Width - BorderSpacing * 2;

			// Calculating max column count in row
			// by trying to fit as many items in one row
			int colcount = 1;
			int cur = -ControlSpacing;
			for (int i = 0; i < items.Count; i++)
			{
				LayoutItem li = (LayoutItem) items[i];
				if (li.Control != null)
				{
					// Also reseting control width and tab index
					li.Control.Width = li.MinimumWidth;
					li.Control.TabIndex = i;

					if (cur < max)
					{
						cur += li.MinimumWidth + li.Label.Width + LabelSpacing + ControlSpacing;
						if (cur < max)
							colcount++;
						else
							maxWidth = cur + BorderSpacing * 2;
					}
				}
			}

			// Sizing up column 
			int[,] cols = null; // [labelWidth,controlWidth] 
			// Trying from the max col count calculated and
			// decreasing
			while (cols == null && colcount > 0)
			{
				cols = new int[colcount,2];
				cur = -ControlSpacing;
				// For each column calculating max control and label width
				for (int c = 0; c < colcount; c++)
				{
					cols[c,0] = 0;
					cols[c,1] = 0;
					// Checking width of each control and label
					// in column
					for (int i = c; i < items.Count; i += colcount)
					{
						LayoutItem li = (LayoutItem) items[i];
						if (li.Control != null)
						{
							if (li.Label.Width > cols[c,0])
								cols[c,0] = li.Label.Width;
							if (li.MinimumWidth > cols[c,1])
								cols[c,1] = li.MinimumWidth;
						}
					}

					cur += cols[c,0] + cols[c,1] + LabelSpacing + ControlSpacing;
				}

				// If the total width of column to large
				// we'll try again with one less column
				if (cur > max)
				{
					cols = null;
					maxWidth = cur + BorderSpacing * 2;
					colcount--;
				}
				else
					// otherwise its good so just storing minimum width
					// posible for setting
					minWidth = cur + BorderSpacing * 2;
			}

			int top = BorderSpacing;
			// colcount > 0 means a reasonable
			// columns setting was found
			if (colcount > 0)
			{
				int rowcount = items.Count / colcount;
				if (items.Count % colcount != 0)
					rowcount++;

				// If only one row, maxWidth is not needed
				if (rowcount == 1)
					maxWidth = max * 2;

				// Positioning each item
				// by the column setting
				int col = 0;
				int left = BorderSpacing;
				int rowHeight = 0;
				foreach (LayoutItem li in items)
				{
					if (li.Control != null)
					{
						li.Position(left, top, cols[col,0], cols[col,1]);
						left += cols[col,0] + cols[col,1] + LabelSpacing + ControlSpacing;
						if (li.Control.Height > rowHeight)
							rowHeight = li.Control.Height;
						col++;
						if (col == colcount)
						{
							top += rowHeight + BorderSpacing;
							col = 0;
							left = BorderSpacing;
							rowHeight = 0;
						}
					}
				}

				// If row didn't end we still need to increase top
				if (col != colcount)
				{
					top += rowHeight + BorderSpacing;
				}
			}
			else // there was no good column settings
				// so there will be only one and it would fill the entire width
			{
				foreach (LayoutItem li in items)
				{
					if (li.Control != null)
					{
						li.Control.Width = Size.Width - BorderSpacing * 2 - LabelSpacing -
							li.Label.Width;
						li.Position(BorderSpacing, top, li.Label.Width, li.Control.Width);
						li.Control.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
						top += li.Control.Height + BorderSpacing;
					}
				}
			}

			neededHeight = top;
			if (autoSize)
				Height = neededHeight;

			if (ItemLayoutChanged != null)
				ItemLayoutChanged(this, EventArgs.Empty);
		}

		public event System.EventHandler ItemLayoutChanged;

		#endregion
	}
*/
}
