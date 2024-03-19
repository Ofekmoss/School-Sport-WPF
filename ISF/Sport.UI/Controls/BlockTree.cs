using System;
using System.Windows.Forms;
using Sport.Common;
using System.Drawing;

namespace Sport.UI.Controls
{
	public class BlockTree : ControlWrapper
	{
		#region BlockCollection Class

		public sealed class BlockCollection : GeneralCollection
		{
			public Block this[int index]
			{
				get { return (Block) GetItem(index); }
				set { SetItem(index, value); }
			}

			public void Insert(int index, Block value)
			{
				InsertItem(index, value);
			}

			public void Remove(Block value)
			{
				RemoveItem(value);
			}

			public bool Contains(Block value)
			{
				return base.Contains(value);
			}

			public int IndexOf(Block value)
			{
				return base.IndexOf(value);
			}

			public int Add(Block value)
			{
				return AddItem(value);
			}
		}

		#endregion

		#region Block Class

		public sealed class Block
		{
			private Block _parent;
			public Block Parent
			{
				get { return _parent; }
			}

			private BlockCollection _children;
			public BlockCollection Children
			{
				get { return _children; }
			}

			private string _text;
			public string Text
			{
				get { return _text; }
				set 
				{ 
					_text = value;
					OnChange();
				}
			}

			private object _tag;
			public object Tag
			{
				get { return _tag; }
				set 
				{ 
					_tag = value; 
					OnChange();
				}
			}
            
			public Block()
			{
				_children = new BlockCollection();
				_children.Changed += new CollectionEventHandler(ChildChanged);
			}

			private void ChildChanged(object sender, CollectionEventArgs e)
			{
				if (e.Old != null)
				{
					((Block) e.Old)._parent.OnChange();
					((Block) e.Old)._parent = null;
				}
				if (e.New != null)
				{
					((Block) e.New)._parent = this;
					((Block) e.New)._parent.OnChange();
				}
			}

			public event EventHandler Changed;

			private void OnChange()
			{
				if (_parent != null)
					_parent.OnChange();

				if (Changed != null)
					Changed(this, EventArgs.Empty);
			}
		}

		#endregion

		#region Properties

		private Block _current;
		public Block Current
		{
			get
			{
				return _current;
			}
			set
			{
				if (value != null)
				{
					Block root = value;
					while (root.Parent != null)
						root = root.Parent;
					if (root != _rootBlock)
						value = null;
				}

				if (_current != value)
				{
					_current = value;
					Refresh();
				}
			}
		}

		private int _rows;
		public int Rows
		{
			get { return _rows; }
			set
			{
				_rows = value;
				Refresh();
			}
		}

		private Block _rootBlock;
		public Block RootBlock
		{
			get { return _rootBlock; }
		}

		#endregion

		#region Constructor

		public BlockTree()
		{
			Clear();
			_rows = 3;
		}

		#endregion

		public void Clear()
		{
			_rootBlock = new Block();
			_current = _rootBlock;
			_rootBlock.Changed +=new EventHandler(RootBlockChanged);
		}

		private Block GetBlockAt(Block current, Rectangle rect, Point pt)
		{
			if (pt.X >= rect.Left && pt.X <= rect.Right &&
				pt.Y >= rect.Top && pt.Y <= rect.Bottom)
				return current;

			if (current.Children.Count > 0)
			{
				Rectangle childRect;
				int width = rect.Width / current.Children.Count;
				int top = rect.Bottom;
				int left = rect.X;
				for (int n = current.Children.Count - 1; n >= 0; n--)
				{
					childRect = new Rectangle(left, top, width, rect.Height);
					Block block = GetBlockAt(current.Children[n], childRect, pt);

					if (block != null)
						return block;

					left += width;
				}
			}

			return null;
		}

		private Block GetBlockAt(Point pt)
		{
			Block root = RootBlock;
			
			if (_current != null)
				root = _current.Parent == null ? _current : _current.Parent;

			Rectangle rect = new Rectangle(0, 0, Width, (Height / _rows) - 1);
			return GetBlockAt(root, rect, pt);
		}

		private void PaintBlock(Graphics g, Block block, Rectangle rect, int level)
		{
			if (level >= 0)
			{
				StringFormat sf = new StringFormat(StringFormatFlags.DirectionRightToLeft);
				sf.LineAlignment = StringAlignment.Center;

				g.FillRectangle(
					block == _current ? SystemBrushes.Highlight : 
					SystemBrushes.Window, rect);

				g.DrawRectangle(SystemPens.WindowText, rect);

				g.DrawString(block.Text, Font, 
					block == _current ? SystemBrushes.HighlightText : SystemBrushes.WindowText, 
					rect, sf);

				if (block.Children.Count > 0)
				{
					Rectangle childRect;
					int width = rect.Width / block.Children.Count;
					int top = rect.Bottom;
					int left = rect.X;
					for (int n = block.Children.Count - 1; n >= 0; n--)
					{
						if (n == 0)
							childRect = new Rectangle(left, top, rect.Width - (left - rect.X), rect.Height);
						else
							childRect = new Rectangle(left, top, width, rect.Height);
						PaintBlock(g, block.Children[n], childRect, level - 1);
						left += width;
					}
				}
			}
		}

		public event EventHandler BlockSelect;

		protected void OnBlockSelect(Block block)
		{
			Current = block;

			if (BlockSelect != null)
				BlockSelect(this, EventArgs.Empty);
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize (e);
			Refresh();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint (e);

			e.Graphics.FillRectangle(SystemBrushes.Control,
				new Rectangle(0, 0, Width, Height));

			Rectangle rect = new Rectangle(0, 0, Width - 1, (Height / _rows) - 1);

			Block root = _rootBlock;

			if (_current != null)
				root = _current.Parent == null ? _current : _current.Parent;

			PaintBlock(e.Graphics, root, rect, _rows - 1);
		}

		protected override void OnPaintBackground(PaintEventArgs pevent)
		{
		}


		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown (e);

			if (e.Button == MouseButtons.Left)
			{
				Block block = GetBlockAt(new Point(e.X, e.Y));
				if (block != null)
					OnBlockSelect(block);
			}
		}

		private void RootBlockChanged(object sender, EventArgs e)
		{
			Refresh();
		}
	}
}
