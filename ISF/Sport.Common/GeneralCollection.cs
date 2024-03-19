using System;
using System.Collections;

namespace Sport.Common
{
	public enum CollectionEventType
	{
		Insert,
		Remove,
		Replace
	}

	/// <summary>
	/// CollectionEventArgs is used when GeneralCollection
	/// event is called
	/// </summary>
	public class CollectionEventArgs : System.EventArgs
	{
		private int		_index;
		private object	_old;
		private object	_new;
		private CollectionEventType _eventType;

		/// <summary>
		/// Gets the index of the changed item
		/// </summary>
		public int		Index	{ get { return _index; } }
		/// <summary>
		/// Gets the value of the removed or replaced item 
		/// </summary>
		public object	Old		{ get { return _old; } }
		/// <summary>
		/// Gets the value of the new item
		/// </summary>
		public object	New		{ get { return _new; } }
		/// <summary>
		/// Gets the type of collection event
		/// </summary>
		public CollectionEventType EventType { get { return _eventType; } }

		public CollectionEventArgs(int index, object o, object n, CollectionEventType eventType)
		{
			_index = index;
			_old = o;
			_new = n;
			_eventType = eventType;
		}
	}

	public delegate void CollectionEventHandler(object sender, CollectionEventArgs e);

	/// <summary>
	/// GeneralCollection implements IList, ICollection and IEnumerable
	/// using an ArrayList and can be inherited to be specialized
	/// for a specific item class.
	/// The collection also adds insert remove and change events.
	/// </summary>
	public class GeneralCollection : IList, ICollection, IEnumerable
	{
		public class CollectionItem
		{
			private object _owner;
			public object Owner
			{
				get { return _owner; }
			}

			internal void SetOwner(object owner)
			{
				if (owner != _owner)
				{
					object oo = _owner;
					_owner = owner;
					OnOwnerChange(oo, owner);
				}
			}

			/// <summary>
			/// OnOnwerChange is called after a CollectionItem is inserted or
			/// removed from a GeneralCollection
			/// </summary>
			public virtual void OnOwnerChange(object oo, object no)
			{
			}
		}

		private object _owner;
		public object Owner
		{
			get { return _owner; }
		}

		protected ArrayList items;

		public GeneralCollection()
		{
			items = new ArrayList();
			_owner = null;
		}

		public GeneralCollection(object owner)
		{
			items = new ArrayList();
			_owner = owner;
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
			set	{ SetItem(index, value); }
		}

		public void RemoveAt(int index)
		{
			RemoveItem(index);
		}

		void IList.Insert(int index, object value)
		{
			InsertItem(index, value);
		}


		void IList.Remove(object value)
		{
			RemoveItem(value);
		}

		bool IList.Contains(object value)
		{
			return items.Contains(value);
		}

		protected bool Contains(object value)
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

		protected int IndexOf(object value)
		{
			return items.IndexOf(value);
		}

		int IList.Add(object value)
		{
			return AddItem(value);
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

		protected object GetItem(int index)
		{
			if ((index < 0)||(index >= items.Count))
				return null;
			return items[index];
		}

		protected virtual void SetItem(int index, object value)
		{
			object o = items[index];
			items[index] = value;

			if (o is CollectionItem)
				((CollectionItem) o).SetOwner(null);

			if (value is CollectionItem)
				((CollectionItem) value).SetOwner(_owner);

			if (Changed != null)
				Changed(this, new CollectionEventArgs(index, o, value, CollectionEventType.Replace));
		}

		protected int AddItem(object value)
		{
			int index = Count;
			InsertItem(index, value);
			return index;
		}

		protected virtual void InsertItem(int index, object value)
		{
			items.Insert(index, value);

			if (value is CollectionItem)
				((CollectionItem) value).SetOwner(_owner);

			if (Changed != null)
				Changed(this, new CollectionEventArgs(index, null, value, CollectionEventType.Insert));
		}

		protected void RemoveItem(object value)
		{
			int index = items.IndexOf(value);
			if (index >= 0)
				RemoveItem(index);
		}

		protected virtual void RemoveItem(int index)
		{
			object item = GetItem(index);
			items.RemoveAt(index);

			if (item is CollectionItem)
				((CollectionItem) item).SetOwner(null);

			if (Changed != null)
				Changed(this, new CollectionEventArgs(index, item, null, CollectionEventType.Remove));
		}

		#endregion

		#region Events

		/// <summary>
		/// Changed event is called when an item is replaced
		/// inserted or removed from the GeneralCollection
		/// </summary>
		public event CollectionEventHandler Changed;

		#endregion

	}
}
