using System;
using System.Drawing;
using Sport.Common;
using System.Collections;

namespace Sport.Documents
{
	#region IDocumentItemStyle Interface

	public interface IDocumentItemStyle
	{
		Direction Direction { get; }
		Font Font { get; }
		Brush BackBrush { get; }
		Brush ForeBrush { get; }
		Pen Border { get; }
	}

	#endregion

	#region DocumentItemStyle class

	public class DocumentItemStyle : IDocumentItemStyle
	{
		private Sport.Documents.Direction _direction;
		public Sport.Documents.Direction Direction
		{
			get { return _direction; }
			set { _direction = value; }
		}

        private Font _font;
		public Font Font
		{
			get { return _font; }
			set { _font = value; }
		}

		private Brush _backBrush;
		public Brush BackBrush
		{
			get { return _backBrush; }
			set { _backBrush = value; }
		}

		public Color BackColor
		{
			get 
			{
				if (_backBrush is SolidBrush)
					return ((SolidBrush) _backBrush).Color;
				return Color.Transparent;
			}
			set
			{
				if (value == Color.Transparent)
					_backBrush = null;
				else
					_backBrush = new SolidBrush(value);
			}
		}

		private Brush _foreBrush;
		public Brush ForeBrush
		{
			get { return _foreBrush; }
			set { _foreBrush = value; }
		}

		public Color ForeColor
		{
			get 
			{
				if (_foreBrush is SolidBrush)
					return ((SolidBrush) _foreBrush).Color;
				return Color.Transparent;
			}
			set
			{
				if (value == Color.Transparent)
					_foreBrush = null;
				else
					_foreBrush = new SolidBrush(value);
			}
		}

		private Pen _border;
		public Pen Border
		{
			get { return _border; }
			set { _border = value; }
		}
	}

	#endregion

	#region DocumentItem Class

	/// <summary>
	/// DocumentItem is the basic class for the document classes.
	/// </summary>
	public class DocumentItem : System.ComponentModel.Component, IDocumentItemStyle,
		ICloneable
	{
		public DocumentItem()
		{
			_direction = Direction.Inherit;
			_font = null;
			_backBrush = null;
			_foreBrush = null;
			_border = null;
			_items = CreateItemCollection();
		}

		public DocumentItem(DocumentItem item)
		{
			_direction = item._direction;
			_font = item._font;
			_backBrush = item._backBrush;
			_foreBrush = item._foreBrush;
			_border = item._border;
			_items = CreateItemCollection();

			foreach (DocumentItem i in item._items)
			{
				_items.Add(i.Clone());
			}
		}

		private DocumentItem _parent;
		[System.ComponentModel.Browsable(false)]
        public DocumentItem Parent
		{
			get { return _parent; }
		}

		#region DocumentItemCollection Class

		public class DocumentItemCollection : IList, ICollection, IEnumerable
		{

			private DocumentItem _documentItem;
			public DocumentItem DocumentItem
			{
				get { return _documentItem; }
			}

			protected ArrayList items;

			public DocumentItemCollection(DocumentItem documentItem)
			{
				items = new ArrayList();
				_documentItem = documentItem;
			}

			#region ICollection Members

			public bool IsSynchronized
			{
				get
				{
					return items.IsSynchronized;
				}
			}

			public int Count
			{
				get
				{
					return items.Count;
				}
			}

			public void CopyTo(Array array, int index)
			{
				items.CopyTo(array, index);
			}

			public object SyncRoot
			{
				get
				{
					return items.SyncRoot;
				}
			}

			#endregion

			#region IEnumerable Members

			public IEnumerator GetEnumerator()
			{
				return items.GetEnumerator();
			}

			#endregion
	
			#region IList Members

			public bool IsReadOnly
			{
				get
				{
					return items.IsReadOnly;
				}
			}

			object IList.this[int index]
			{
				get { return GetItem(index); }
				set	{ SetItem(index, (DocumentItem) value); }
			}

			public void RemoveAt(int index)
			{
				RemoveItem(index);
			}

			void IList.Insert(int index, object value)
			{
				InsertItem(index, (DocumentItem) value);
			}


			void IList.Remove(object value)
			{
				RemoveItem((DocumentItem) value);
			}

			bool IList.Contains(object value)
			{
				return items.Contains(value);
			}

			public void Clear()
			{
				for (int i = items.Count - 1; i >= 0; i--)
					RemoveAt(i);
			}

			int IList.IndexOf(object value)
			{
				return items.IndexOf(value);
			}

			int IList.Add(object value)
			{
				return AddItem((DocumentItem) value);
			}

			public bool IsFixedSize
			{
				get
				{
					return items.IsFixedSize;
				}
			}

			#endregion

			#region Protected Functions

			protected DocumentItem GetItem(int index)
			{
				if ((index < 0)||(index >= items.Count))
					return null;
				return (DocumentItem) items[index];
			}

			protected virtual void SetItem(int index, DocumentItem value)
			{
				DocumentItem di = (DocumentItem) items[index];

				if (di != null)
					di._parent = null;

				items[index] = value;

				if (value != null)
				{
					if (value._parent != null)
						value._parent._items.Remove(value);

					value._parent = _documentItem;
				}
			}

			protected int AddItem(DocumentItem value)
			{
				int index = Count;
				InsertItem(index, value);
				return index;
			}

			protected void InsertItem(int index, DocumentItem value)
			{
				items.Insert(index, null);
				SetItem(index, value);
			}

			protected void RemoveItem(DocumentItem value)
			{
				int index = items.IndexOf(value);
				if (index >= 0)
					RemoveItem(index);
			}

			protected void RemoveItem(int index)
			{
				SetItem(index, null);
				items.RemoveAt(index);
			}

			#endregion

			public DocumentItem this[int index]
			{
				get { return GetItem(index); }
				set { SetItem(index, value); }
			}

			public void Insert(int index, DocumentItem value)
			{
				InsertItem(index, value);
			}

			public void Remove(DocumentItem value)
			{
				RemoveItem(value);
			}

			public bool Contains(DocumentItem value)
			{
				return items.Contains(value);
			}

			public int IndexOf(DocumentItem value)
			{
				return items.IndexOf(value);
			}

			public int Add(DocumentItem value)
			{
				return AddItem(value);
			}
		}

		#endregion


		private DocumentItemCollection _items;
		public DocumentItemCollection Items
		{
			get { return _items; }
		}

		protected virtual DocumentItemCollection CreateItemCollection()
		{
			return new DocumentItemCollection(this);
		}

		private Direction _direction;
		public Direction Direction
		{
			get 
			{
				if (_direction == Direction.Inherit)
				{
					if (Parent == null)
						return Direction.Left;

					return Parent.Direction;
				}

				return _direction; 
			}
			set 
			{ 
				_direction = value; 
			}
		}

		private Font _font;
		public Font Font
		{
			get 
			{ 
				if (_font == null)
				{
					if (Parent == null)
						return null;

					return Parent.Font;
				}

				return _font; 
			}
			set 
			{ 
				_font = value; 
			}
		}

		private Brush _backBrush;
		[System.ComponentModel.Browsable(false), System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		public Brush BackBrush
		{
			get 
			{ 
				if (_backBrush == null)
				{
					if (Parent == null)
						return null;

					return Parent.BackBrush;
				}

				return _backBrush;
			}
			set
			{
				_backBrush = value;
			}
		}

		public Color BackColor
		{
			get 
			{
				if (_backBrush is SolidBrush)
					return ((SolidBrush) _backBrush).Color;
				return Color.Transparent;
			}
			set
			{
				if (value == Color.Transparent)
					_backBrush = null;
				else
					_backBrush = new SolidBrush(value);
			}
		}

		private Brush _foreBrush;
		[System.ComponentModel.Browsable(false), System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
		public Brush ForeBrush
		{
			get 
			{ 
				if (_foreBrush == null)
				{
					if (Parent == null)
						return null;

					return Parent.ForeBrush;
				}

				return _foreBrush;
			}
			set
			{
				_foreBrush = value;
			}
		}

		public Color ForeColor
		{
			get 
			{
				if (_foreBrush is SolidBrush)
					return ((SolidBrush) _foreBrush).Color;
				return Color.Transparent;
			}
			set
			{
				if (value == Color.Transparent)
					_foreBrush = null;
				else
					_foreBrush = new SolidBrush(value);
			}
		}

		private Pen _border;
		public Pen Border
		{
			get
			{
				/*if (_border == null)
				{
					if (Parent == null)
						return null;
					return Parent.Border;
				}*/

				return _border;
			}
			set
			{
				_border = value;
			}
		}

		#region IDocumentItemStyle Members

		Sport.Documents.Direction Sport.Documents.IDocumentItemStyle.Direction
		{
			get { return this.Direction; }
		}

		Font Sport.Documents.IDocumentItemStyle.Font
		{
			get { return this.Font; }
		}

		Brush Sport.Documents.IDocumentItemStyle.BackBrush
		{
			get { return this.BackBrush; }
		}

		Brush Sport.Documents.IDocumentItemStyle.ForeBrush
		{
			get { return this.ForeBrush; }
		}

		Pen Sport.Documents.IDocumentItemStyle.Border
		{
			get { return this.Border; }
		}

		#endregion

		public virtual DocumentItem Clone()
		{
			return new DocumentItem(this);
		}

		#region ICloneable Members

		object ICloneable.Clone()
		{
			return this.Clone();
		}

		#endregion
	}

	#endregion
}
