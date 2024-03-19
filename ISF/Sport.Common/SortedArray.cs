using System;
using System.Collections;

namespace Sport.Common
{
	/// <summary>
	/// SortedArray implements IList, ICollection and IEnumerable
	/// to store a sorted array of items using the given comparer.
	/// </summary>
	public class SortedArray : IList, ICollection, IEnumerable
	{
		#region SortedArrayEnumerator Class

		/// <summary>
		/// SortedArrayEnumerator implements an enumerator for
		/// a sorted array
		/// </summary>
		private class SortedArrayEnumerator : IEnumerator, ICloneable
		{
			// Stores the array enumerated
			private SortedArray	_array;
			// Stores current position in array
			private int			index;
			// Stores the array's version when the enumerator
			// was constructed
			private int			version;
			// Stores the value in the sorted array in the current
			// position
			private object		current;

			// The constructor is called by SortedArray 
			internal SortedArrayEnumerator(SortedArray array)
			{
				_array = array;
				index = -1;
				version = array._version;
			}

			public object Clone()
			{
				return base.MemberwiseClone();
			}

			// Sets the enumerator position on the next item
			public virtual bool MoveNext()
			{
				// If the version of the array changed
				// the enumerator can no longer be used
				if (version != _array._version)
				{
					throw new InvalidOperationException("InvalidOperation_EnumFailedVersion");
				}
				// Checking if reached the end of the array
				if (index < (_array.Count - 1))
				{
					index++;
					current = _array[index];
					return true;
				}
				// Setting to end of array
				current = _array;
				index = _array.Count;
				return false;
			}

			// Resets the enumerator to the start of the array
			public virtual void Reset()
			{
				// If the version of the array changed
				// the enumerator can no longer be used
				if (version != _array._version)
				{
					throw new InvalidOperationException("InvalidOperation_EnumFailedVersion");
				}
				current = _array;
				index = -1;
			}

			// Returns the value current item in the array
			public virtual object Current 
			{
				get
				{
					// Checking if current has been set
					if (current != _array)
						return current;
					if (this.index == -1)
					{
						throw new InvalidOperationException("InvalidOperation_EnumNotStarted");
					}
					throw new InvalidOperationException("InvalidOperation_EnumEnded");
				}
			}
		}

		#endregion

		#region Data Members

		// Defines the initial capacity of the array
		private const int	_defaultCapacity = 0x10;
		// Stores the amount of items stored in the array
		private int			_size;
		// An array of items
		private object[]	_items;
		// Stores the current version of the array, the
		// version is changed with every change of the array
		private int			_version;
		// Stores the comparer that compares the items
		private IComparer	_comparer;

		#endregion

		#region Constructors

		// Default constructor, construct the array to the default capacity
		public SortedArray()
		{
			_items = new object[_defaultCapacity];
		}

		// Construct the array to the default capacity and sets the comparer
		public SortedArray(IComparer comparer)
		{
			_items = new object[_defaultCapacity];
			_comparer = comparer;
		}

		#endregion

		#region Search Functions

		public int BinarySearch(object value)
		{
			return BinarySearch(0, _size, value, _comparer);
		}

		public int BinarySearch(object value, IComparer comparer)
		{
			return BinarySearch(0, _size, value, comparer);
		}

		public int BinarySearch(int index, int count, object value, IComparer comparer)
		{
			if ((index < 0) || (count < 0))
			{
				throw new ArgumentOutOfRangeException("index", index < 0 ? "Index out of range" : "Count out of range");
			}
			if ((_size - index) < count)
			{
				throw new ArgumentException("Count out of range", "count");
			}
			return Array.BinarySearch(_items, index, count, value, comparer);
		}

		#endregion

		#region ICollection Members

		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		// Returns the amount of items stored in the array
		public int Count
		{
			get
			{
				return _size;
			}
		}

		// Copies all items to the given array
		public void CopyTo(Array array, int index)
		{
			if ((array != null) && (array.Rank != 1))
			{
				throw new ArgumentException("Arg_RankMultiDimNotSupported");
			}
			Array.Copy(_items, 0, array, index, _size);
		}

		public object SyncRoot
		{
			get
			{
				return this;
			}
		}

		#endregion

		#region IEnumerable Members

		// Creates and returns an enumerator for the array
		public IEnumerator GetEnumerator()
		{
			return new SortedArrayEnumerator(this);
		}

		#endregion
	
		#region IList Members

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		// Gets and sets the item in the given index
		public object this[int index]
		{
			get
			{
				// Checking if index in range
				if ((index < 0) || (index >= _size))
				{
					throw new ArgumentOutOfRangeException("index", "ArgumentOutOfRange_Index");
				}
				return _items[index];
			}
			set
			{
				// Checking if index in range
				if ((index < 0) || (index >= this._size))
				{
					throw new ArgumentOutOfRangeException("index", "ArgumentOutOfRange_Index");
				}
				// Replacing the item in the given index with value
				// Removing and adding in order to place the item sorted
				RemoveAt(index);
				Add(value);
			}
		}

		// Inserts the given item in the given index
		void InsertAt(int index, object value)
		{
			// Checking if index in range
			if ((index < 0) || (index > this._size))
			{
				throw new ArgumentOutOfRangeException("index", "ArgumentOutOfRange_ArrayListInsert");
			}
			// Resetting capacity if needed
			if (_size == _items.Length)
			{
				EnsureCapacity(this._size + 1);
			}
			// Moving items forward if needed
			if (index < _size)
			{
				Array.Copy(_items, index, _items, (int) (index + 1), (int) (_size - index));
			}
			_items[index] = value;
			_size++;
			_version++;
		}

		// Removes the item in the given index
		public void RemoveAt(int index)
		{
			// Checking if index in range
			if ((index < 0) || (index >= this._size))
			{
				throw new ArgumentOutOfRangeException("index", "ArgumentOutOfRange_Index");
			}
			// Reducing items count
			_size--;
			if (index < _size)
			{
				// If needed copying items over removed item
				Array.Copy(_items, (int) (index + 1), _items, index, (int) (_size - index));
			}
			_items[_size] = null;
			// Increasing array version
			_version++;
		}

		// Insert in a specific position is not possible
		// so just adding the value
		void IList.Insert(int index, object value)
		{
			Add(value);
		}

		// Removes the given item
		public void Remove(object value)
		{
			int index = IndexOf(value);
			if (index >= 0)
				RemoveAt(index);
		}

		// Checks whether the given item exists in the array
		public bool Contains(object value)
		{
			return IndexOf(value) >= 0;
		}

		// Clear all items in array
		public void Clear()
		{
			Array.Clear(_items, 0, _size);
			_size = 0;
			_version++;
		}

		// Return the index of the given item
		public int IndexOf(object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value", "ArgumentNull_Value");
			}
			int index = Array.BinarySearch(_items, 0, _size, value, _comparer);
			if (index < 0)
				return -1;

			// If the comparison is not consistent the binary search
			// might miss the exact spot.
			if (!this[index].Equals(value))
				throw new Exception("Failed to retrieve correct index");

			return index;
		}

		// Adding the given item to the array
		public int Add(object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value", "ArgumentNull_Value");
			}
			int index = Array.BinarySearch(_items, 0, _size, value, _comparer);

			// If the index is negative the comparer could not find
			// the item, only a position for the item
			if (index < 0)
				index = ~index;

            InsertAt(index, value);
			return index;
		}

		// Adding a collection of items to the array
		public void AddRange(ICollection c)
		{
			EnsureCapacity(this._size + c.Count);
			c.CopyTo(_items, this._size);
			_size += c.Count;
			// Resorting the array
			Sort();
		}

		public bool IsFixedSize
		{
			get
			{
				return false;
			}
		}

		#endregion

		#region Capacity

		// Setting the array size to at least min
		private void EnsureCapacity(int min)
		{
			if (_items.Length < min)
			{
				int capacity = (_items.Length == 0) ? _defaultCapacity : (_items.Length * 2);
				if (capacity < min)
				{
					capacity = min;
				}
				this.Capacity = capacity;
			}
		}

		// Gets and sets the capacity of the array
		public virtual int Capacity
		{
			get
			{
				return _items.Length;
			}
			set
			{
				if (value != _items.Length)
				{
					if (value < _size)
					{
						throw new ArgumentOutOfRangeException("value", "ArgumentOutOfRange_SmallCapacity");
					}
					if (value > 0)
					{
						// Creates a new array with the new capacity...
						object[] temp = new object[value];
						if (this._size > 0)
						{
							// ... and copying the items from the old array
							// is needed
							Array.Copy(_items, 0, temp, 0, this._size);
						}
						_items = temp;
					}
					else
					{
						// When trying to set capacity to 0, setting
						// capacity to default
						_items = new object[_defaultCapacity];
					}
				}
			}
		}

		#endregion

		#region Compare and Sort

		// Gets and setts the comparer that compares between the
		// items in the list
		public IComparer Comparer
		{
			get
			{
				return _comparer;
			}
			set
			{
				if (value != _comparer)
				{
					_comparer = value;
					// If comparer changed resort is needed
					Sort();
				}
			}
		}

		// Resorts the items in the array
		public void Sort()
		{
			try
			{
				_version++;
				Array.Sort(_items, 0, _size, _comparer);
			}
			catch
			{
				//better array won't be sorted than total crash of application
			}
		}

		#endregion
	}
}
