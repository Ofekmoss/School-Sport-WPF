using System;
using System.Collections;

namespace Sport.Common
{
	/// <summary>
	/// RangeArray stores an array of ranges
	/// </summary>
	public class RangeArray : IEnumerable, ICloneable
	{
		#region Limit Values

		public static readonly int Start = Int32.MinValue;
		public static readonly int End = Int32.MaxValue;

		#endregion

		#region Range Class

		public class Range
		{
			#region Properties

			// The Range stores the first value
			// in the range and the size of the range
			private int _first;
			private int _last;

			/// <summary>
			/// Gets the first value in the range
			/// </summary>
			public int First	{ get { return _first; } }
			/// <summary>
			/// Gets the last value in the range
			/// </summary>
			public int Last		{ get { return _last; } }
			/// <summary>
			/// Gets the size of the range of value
			/// </summary>
			public int Size		{ get { return _last - _first + 1; } }

			#endregion

			#region Constructor

			internal Range(int first, int last)
			{
				if (last < first)
					throw new ArgumentOutOfRangeException("first,last", "Size must be positive");

				_first = first;
				_last = last;
			}

			#endregion

			#region String Format Functions

			/// <summary>
			/// Formats the Range to a string
			/// </summary>
			/// <returns></returns>
			public override string ToString()
			{
				if (_first == Start)
				{
					if (_last == End)
						return "...";

					return "..." + _last.ToString();
				}
				else if (_last == End)
					return _first.ToString() + "...";
				
				return _first.ToString() + "-" + _last.ToString();
			}

			/// <summary>
			/// Parse a string into a Range
			/// </summary>
			/// <param name="s">Range string</param>
			/// <returns></returns>
			public static Range Parse(string s)
			{
				if (s == null)
					return null;

				s.Trim();

				if (s == "")
					return null;

				if (s == "...")
				{
					return new Range(Start, End);
				}
				else if (s.Length > 3)
				{
					if (s.Substring(0, 3) == "...")
					{
						int last = Int32.Parse(s.Substring(3));
						return new Range(Start, last);
					}
					else if (s.Substring(s.Length - 3, 3) == "...")
					{
						int first = Int32.Parse(s.Substring(0, s.Length - 3));
						return new Range(first, End);
					}
				}
				
				string[] spl = s.Split(new char[] {'-'});

				if (spl.Length == 1)
				{
					int v = Int32.Parse(spl[0]);
					return new Range(v, v);
				}
				else if (spl.Length == 2)
				{
					int a = Int32.Parse(spl[0]);
					int b = Int32.Parse(spl[1]);
					if (a < b)
						return new Range(a, b);
					else
						return new Range(b, a);
				}

				throw new ArgumentException("Cannot parse string to range");
			}

			#endregion
		}

		#endregion

		#region String Format Functions

		/// <summary>
		/// Formats the RangeArray to a string
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			string[] values = new string[ranges.Count];
			for (int n = 0; n < ranges.Count; n++)
				values[n] = ranges[n].ToString();

			return String.Join(",", values);
		}

		/// <summary>
		/// Parse a string into a RangeArray
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static RangeArray Parse(string s)
		{
			RangeArray ra = new RangeArray();

			if (s != null)
			{
				string[] values = s.Split(new char[] {','});

				foreach (string value in values)
				{
					Range range = Range.Parse(value);
					if (range != null)
						ra.Add(range.First, range.Last);
				}
			}

			return ra;
		}

		#endregion

		#region Storage

		// Array of ranges
		private ArrayList ranges = null;

		#endregion

		#region Properties

		// Total size
		private int _size;
		/// <summary>
		/// Gets the total sum of all ranges sizes
		/// </summary>
		public int Size
		{
			get { return _size; }
		}

		/// <summary>
		/// Gets or sets the whether the given value is in 
		/// the range array
		/// </summary>
 		public bool this[int val]
		{
			get 
			{
				foreach (Range range in ranges)
				{
					if (val < range.First)
						return false;
					if (val <= range.Last)
						return true;
				}
				return false;
			}
			set
			{
				if (value)
					Add(val);
				else
					Remove(val);
			}
		}

		/// <summary>
		/// Gets the amount of ranges in the RangeArray
		/// </summary>
		public int RangeCount
		{
			get
			{
				return ranges.Count; 
			}
		}

		#endregion

		#region Constructors

		// Constructor
		public RangeArray()
		{
			ranges = new ArrayList();
			_size = 0;
		}

		public RangeArray(int first, int last)
			: this()
		{
			Add(first, last);
		}

		#endregion

		#region Events

		/// <summary>
		/// Changed is called when a range or ranges in the 
		/// RangeArray are changed
		/// </summary>
		public event System.EventHandler Changed;

		protected virtual void OnRangeChange()
		{
			if (Changed != null)
				Changed(this, EventArgs.Empty);
		}

		#endregion

		#region Array Functions

		/// <summary>
		/// Gets the index of the given Range
		/// </summary>
		/// <param name="range"></param>
		/// <returns></returns>
		public int IndexOf(Range range)
		{
			return ranges.IndexOf(range);
		}

		/// <summary>
		/// Gets the Range item of the given value
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public Range GetRange(int value)
		{
			foreach (Range range in ranges)
			{
				if (value < range.First)
					return null;
				if (value <= range.Last)
					return range;
			}
			return null;
		}

		/// <summary>
		/// Gets an array of ranges that intersects with the
		/// given range
		/// </summary>
		/// <param name="first"></param>
		/// <param name="last"></param>
		/// <returns></returns>
		public Range[] GetRanges(int first, int last)
		{
			if (last < first)
				throw new ArgumentOutOfRangeException("first,last", "Size must be positive");

			ArrayList al = new ArrayList();
			for (int r = 0; r < ranges.Count; r++)
			{
				Range range = (Range) ranges[r];
				
				if (last < range.First)
					break;

				if (first <= range.Last && last >= range.First)
					al.Add(range);
			}

			Range[] result = new Range[al.Count];
			al.CopyTo(result);
			return result;
		}

		/// <summary>
		/// Gets the Range in the given index
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public Range RangeAt(int index)
		{
			return (Range) ranges[index];
		}

		/// <summary>
		/// Adds the given value to the RangeArray
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public Range Add(int value)
		{
			return Add(value, value);
		}

		/// <summary>
		/// Adds the given range to the RangeArray
		/// </summary>
		/// <param name="range"></param>
		/// <returns></returns>
		public Range Add(Range range)
		{
			return Add(range.First, range.Last);
		}


		/// <summary>
		/// Adds the given range of values to the RangeArray
		/// </summary>
		/// <param name="first"></param>
		/// <param name="last"></param>
		/// <returns></returns>
		public Range Add(int first, int last)
		{
			if (last < first)
				throw new ArgumentOutOfRangeException("first,last", "Size must be positive");

			// Checking all existing ranges if the given
			// range intersects with them
			for (int r = 0; r < ranges.Count; r++)
			{
				Range range = (Range) ranges[r];

				// The given range does not intersect and is
				// previous to the current range...
				if (last != End && range.First > last + 1)
				{
					//... so inserting a new range before the current ...
					Range result = new Range(first, last);
					ranges.Insert(r, result);
					//... setting total count ...
					_size += result.Size;
					//... and informing of the change
					OnRangeChange();
					return result;
				}

				// The given range insersects with the
				// current range...
				if ((range.First == Start || last >= range.First - 1) &&
					(range.Last == End || first <= range.Last + 1))
				{
					// Calculating intersection 'first' value
					int f = first < range.First ? first : range.First;
					// Calculating intersection 'last' value
					int l = last > range.Last ? last : range.Last;
					int n = r + 1;
					// Checking if also intersecting next ranges
					while (n < ranges.Count)
					{
						Range nr = (Range) ranges[n];
						// If intersects...
						if (nr.First - 1 <= l)
						{
							//... resetting intersection 'last' row ...
							if (nr.Last > l)
								l = nr.Last;
							//... and removing the range (as it would be 
							// added with the given range)
							_size -= nr.Size;
							ranges.RemoveAt(n);
						}
						else
						{
							n = ranges.Count;
						}
					}

					// ... setting total count ...
					_size = _size + (range.First - f) + (l - range.Last);

					// ... resetting current range ...
					ranges[r] = new Range(f, l);

					//... and informing of the change
					OnRangeChange();
					return range;
				}
			}

			// No intersection found
			// Adding range after all others
			Range res = new Range(first, last);
			_size += res.Size;
			ranges.Add(res);
			//... and informing of the change
			OnRangeChange();
			return res;
		}

		/// <summary>
		/// Clears all ranges
		/// </summary>
		public void Clear()
		{
			ranges.Clear();
			_size = 0;
			//... and informing of the change
			OnRangeChange();
		}

		/// <summary>
		/// Removes the given value from the RangeArray
		/// </summary>
		/// <param name="value"></param>
		public void Remove(int value)
		{
			Remove(value, value);
		}

		/// <summary>
		/// Removes the given Range from the RangeArray
		/// </summary>
		/// <param name="range"></param>
		public void Remove(Range range)
		{
			Remove(range.First, range.Last);
		}

		/// <summary>
		/// Removes the given range of values from the RangeArray
		/// </summary>
		/// <param name="first"></param>
		/// <param name="last"></param>
		public void Remove(int first, int last)
		{
			if (last < first)
				throw new ArgumentOutOfRangeException("first,last", "Size must be positive");

			int r = 0;
			// Checking for ranges the intersect with
			// the given range
			while (r < ranges.Count)
			{
				Range range = (Range) ranges[r];

				// If range starts before given range ends...
				if (last == End || range.First < last + 1)
				{
					// ... if the range start before given range ...
					if (range.First < first)
					{
						// ... if the range ends after given range
						// starts ...
						if (range.Last >= first)
						{
							// ... if the range ends after the given
							// range ends
							if (range.Last > last)
							{
								// The given range is inside
								// the current range, so splitting
								// the current range to, before
								// the given and after the given
								ranges.Insert(r + 1, 
									new Range(last + 1, range.Last));
								ranges[r] = new Range(range.First, first - 1);
								// Setting total selection count
								_size -= last - first + 1;
								//... and informing of the change
								OnRangeChange();
								return ;
							}

							// The given range covers the end
							// of the current range, so just
							// shrinking the current range
							_size -= range.Size - (first - range.First);
							ranges[r] = new Range(range.First, first - 1);
						}

						r++;
					}
					else
					{
						//... the current range starts in the start or middle
						// of the given range
						if (last >= range.Last)
						{
							// given range covers all current range, so
							// removing it
							_size -= ((Range) ranges[r]).Size;
							ranges.RemoveAt(r);
						}
						else
						{
							// given range covers start of current range
							// so shrinking it as needed
							_size -= (range.Size - (range.Last - last));
							ranges[r] = new Range(last + 1, range.Last);
							//... and informing of the change
							OnRangeChange();
							return ;
						}
					}
				}
				else
				{
					// Past last range
					OnRangeChange();
					return ;
				}
			}

			//... and informing of the change
			OnRangeChange();
		}

		#endregion

		#region Group Functions

		public void Union(RangeArray ra)
		{
			if (ra != null)
			{
				foreach (Range range in ra)
				{
					Add(range);
				}
			}
		}

		public void Intersect(RangeArray ra)
		{
			if (ra == null)
			{
				Clear();
			}
			else
			{
				int last = Start;
				foreach (Range range in ra)
				{
					if (range.First > last)
						Remove(last, range.First - 1);
					last = range.Last + 1;
				}
				if (last != End)
					Remove(last, End);
			}
		}

		#endregion

		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			return ranges.GetEnumerator();
		}

		#endregion

		#region ICloneable Members

		public object Clone()
		{
			RangeArray ra = new RangeArray();
			ra._size = _size;
			ra.ranges = (ArrayList) ranges.Clone();
			return ra;
		}

		#endregion
	}
}
