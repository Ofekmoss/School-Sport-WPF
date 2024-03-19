using System;
using System.Collections;
using System.Data;

namespace Sport.Data
{
	public class LookupItem
	{
		private int		_id;
		private string	_text;

		public LookupItem(int id, string text)
		{
			_id = id;
			_text = text;
		}

		public int		Id		{ get { return _id; } }
		public string	Text	{ get { return _text; } }

		public override string ToString()
		{
			return _text;
		}

		public override int GetHashCode()
		{
			return _id;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			if (obj is int)
				return (int) obj == _id;
			if (obj is LookupItem)
				return ((LookupItem) obj).Id == _id;

			return false;
		}
	}

	/// <summary>
	/// Summary description for LookupType.
	/// </summary>
	public class LookupType
	{
		public virtual LookupItem this[int id]
		{
			get
			{
				return new LookupItem(id, id.ToString());
			}
		}

		public virtual string Lookup(int id)
		{
			return id.ToString();
		}

		public virtual LookupItem[] Items
		{
			get
			{
				return null;
			}
		}
	}
	
	/// <summary>
	/// Summary description for LookupTable.
	/// </summary>
	public class LookupTable : LookupType
	{
		private Hashtable items;

		public LookupTable()
		{
			items = new Hashtable();
		}

		public override LookupItem this[int id]
		{
			get
			{
				return (LookupItem) items[id];
			}
		}

		public override string Lookup(int id)
		{
			LookupItem li = this[id];
			return li == null ? null : li.Text;
		}

		LookupItem[] itemArray = null;
		public override LookupItem[] Items
		{
			get
			{
				if (itemArray == null)
				{
					itemArray = new LookupItem[items.Values.Count];
					items.Values.CopyTo(itemArray, 0);
				}
				return itemArray;
			}
		}
	}

}
