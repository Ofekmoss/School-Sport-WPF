using System;

namespace Sport.Producer
{
	public class RangeItem : Sport.Common.GeneralCollection.CollectionItem
	{
		protected virtual void RangeChanging(int min, int max)
		{
		}

		private int _min;
		public int Min
		{
			get { return _min; }
			set
			{
				if (value < 1)
					throw new ArgumentOutOfRangeException("value", "Minimum value must be greater than 0");

				if (_min < value)
				{
					if (value > _max)
						throw new ArgumentOutOfRangeException("value", "Minimum value must be smaller than maximum");
				}

				RangeChanging(value, _max);
				_min = value;
			}
		}

		private int _max;
		public int Max
		{
			get { return _max; }
			set
			{
				if (_max != value)
				{
					if (_max > value)
					{
						if (value < _min)
							throw new ArgumentOutOfRangeException("value", "Minimum value must be smaller than maximum");
					}

					RangeChanging(_min, value);
					_max = value;
				}
			}
		}

		public override string ToString()
		{
			return _min.ToString() + " - " + _max.ToString();
		}

		public RangeItem(int min, int max)
		{
			_min = min;
			_max = max;
		}
	}

	public class RangeCollection : Sport.Common.GeneralCollection
	{
		public RangeCollection()
		{
		}
		public RangeCollection(object owner)
			: base(owner)
		{
		}

		public RangeItem this[int index]
		{
			get { return (RangeItem) GetItem(index); }
		}

		public RangeItem GetRangeItem(int value)
		{
			for (int n = 0; n < Count; n++)
			{
				RangeItem item = this[n];
				if (item.Min > value)
					return null;
				if (item.Max >= value)
					return item;
			}
			
			return null;
		}

		public void Insert(RangeItem value)
		{
			Add(value);
		}

		public void Remove(RangeItem value)
		{
			RemoveItem(value);
		}

		public bool Contains(RangeItem value)
		{
			return base.Contains(value);
		}

		public int IndexOf(RangeItem value)
		{
			return base.IndexOf(value);
		}

		public int Add(RangeItem value)
		{
			for (int n = 0; n < Count; n++)
			{
				if  (value.Max < this[n].Min)
				{
					InsertItem(n, value);
					return n;
				}
				else if (value.Min <= this[n].Max &&
					value.Max >= this[n].Min)
					throw new ArgumentOutOfRangeException("value", "Range item settings don't fit in collection");
			}

			InsertItem(Count, value);
			return Count - 1;
		}

		public bool Fit(int min, int max)
		{
			foreach (RangeItem item in this)
			{
				if (max < item.Min)
					return true;
				if (min <= item.Max && max >= item.Min)
					return false;
			}

			return true;
		}
	}
}
