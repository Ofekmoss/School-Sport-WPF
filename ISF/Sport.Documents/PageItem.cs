using System;
using System.Drawing;
using System.ComponentModel.Design;
//using Word=Microsoft.Office.Interop.Word;

namespace Sport.Documents
{
	#region PageItem Class

	[Flags]
	public enum Borders
	{
		Left = 0x1,
		Top = 0x2,
		Right = 0x4,
		Bottom = 0x8,
		All = Top | Left | Right | Bottom
	}

	/// <summary>
	/// Summary description for PageItem.
	/// </summary>
	public class PageItem : DocumentItem
	{
		public PageItem(Rectangle bounds)
		{
			_bounds = bounds;
			_borders = Borders.All;
		}

		public PageItem()
			: this(Rectangle.Empty)
		{
		}

		#region Cloning

		public PageItem(PageItem item)
			: base(item)
		{
			_bounds = item._bounds;
			_borders = item._borders;
		}

		public override DocumentItem Clone()
		{
			return new PageItem(this);
		}

		#endregion

		#region PageItemCollection Class

		public class PageItemCollection : DocumentItemCollection
		{
			public PageItemCollection(PageItem pageItem)
				: base(pageItem)
			{
			}

			protected override void SetItem(int index, DocumentItem value)
			{
				if (value != null && !(value is PageItem))
				{
					throw new DocumentException("Child item in document must inherit PageItem");
				}

				base.SetItem (index, value);
			}

			public PageItem PageItem
			{
				get { return ((PageItem) DocumentItem); }
			}

			public new PageItem this[int index]
			{
				get { return (PageItem) GetItem(index); }
				set { SetItem(index, value); }
			}

			public void Insert(int index, PageItem value)
			{
				InsertItem(index, value);
			}

			public void Remove(PageItem value)
			{
				RemoveItem(value);
			}

			public bool Contains(PageItem value)
			{
				return base.Contains(value);
			}

			public int IndexOf(PageItem value)
			{
				return base.IndexOf(value);
			}

			public int Add(PageItem value)
			{
				return AddItem(value);
			}
		}

		#endregion

		protected override DocumentItemCollection CreateItemCollection()
		{
			return new PageItemCollection(this);
		}

		[System.ComponentModel.Browsable(false), System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Content)]
		public new PageItemCollection Items
		{
			get { return (PageItemCollection) base.Items; }
		}

		public Page Page
		{
			get
			{
				PageItem item = this;
				while (item != null && !(item is Page))
					item = item.Parent as PageItem;

				return item as Page;
			}
		}

		private Rectangle _bounds;
		public Rectangle Bounds
		{
			get { return _bounds; }
			set { _bounds = value; }
		}

		private Borders _borders;
		public Borders Borders
		{
			get { return _borders; }
			set { _borders = value; }
		}

		public int PageLeft=0;
		public int PageTop=0;
		public virtual void PaintItem(Graphics g)
		{
			if (!_bounds.IsEmpty)
			{
				if (BackBrush != null)
				{
					g.FillRectangle(BackBrush, _bounds);
				}
				if (Border != null)
				{
					if (_borders == Borders.All)
					{
						int left=this.PageLeft;
						int top=this.PageTop;
						if (left == 0)
							left = _bounds.Left;
						if (top == 0)
							top = _bounds.Top;
						g.DrawRectangle(Border, new Rectangle(left, top, 
							_bounds.Width, _bounds.Height));
					}
					else
					{
						if ((_borders & Borders.Left) != 0)
						{
							g.DrawLine(Border, _bounds.Left, _bounds.Top, _bounds.Left, _bounds.Bottom);
						}
						if ((_borders & Borders.Top) != 0)
						{
							g.DrawLine(Border, _bounds.Left, _bounds.Top, _bounds.Right, _bounds.Top);
						}
						if ((_borders & Borders.Right) != 0)
						{
							g.DrawLine(Border, _bounds.Right, _bounds.Top, _bounds.Right, _bounds.Bottom);
						}
						if ((_borders & Borders.Bottom) != 0)
						{
							g.DrawLine(Border, _bounds.Left, _bounds.Bottom, _bounds.Right, _bounds.Bottom);
						}
					}
				}
			}
		}

		public void Paint(Graphics g)
		{
			PaintItem(g);
			
			foreach (PageItem item in Items)
			{
				item.Paint(g);
			}
		}

		/*
		public virtual void Save(Word.Document document, Sport.Common.ProgressHelper progressHelper)
		{
			Bitmap bitmap = new Bitmap(Bounds.Width + 1, Bounds.Height + 1);
			Graphics g = Graphics.FromImage(bitmap);

			g.TranslateTransform(-Bounds.Left, -Bounds.Top);

			PaintItem(g);

			string tempFile = document.Application.Path + "\\temp";

			bitmap.Save(tempFile);

			bitmap.Dispose();

			object missing = Type.Missing;
			object width = (float) Bounds.Width * 72 / 100;
			object height = (float) Bounds.Height * 72 / 100;

			Word.Shape shape = document.Shapes.AddCanvas(
				(float) Bounds.Left * 72 / 100,
				(float) Bounds.Top * 72 / 100,
				(float) Bounds.Width * 72 / 100,
				(float) Bounds.Height * 72 / 100, ref missing);

			object i = 0;

			object a = false, b = true;
			shape.CanvasItems.AddPicture(tempFile, ref a, ref b, 
				ref i, ref i, ref width, ref height);
		}

		public virtual void Save(Word.Document document)
		{
			Save(document, null);
		}
		*/
	}

	#endregion
}
