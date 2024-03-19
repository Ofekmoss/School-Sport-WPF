using System;

namespace Sport.Common
{
	public interface IFieldComparer
	{
		int Compare(int field, object x, object y);
	}

	/// <summary>
	/// Summary description for ArraySort.
	/// </summary>
	public class ArraySort : System.Collections.IComparer
	{
		public static void Sort(Array array, int index, int length,
			int[] sortFields, System.Collections.IComparer[] fieldComparers)
		{
			Array.Sort(array, index, length, new ArraySort(sortFields, fieldComparers));
		}

		public static void Sort(Array array, int index, int length,
			int[] sortFields, IFieldComparer fieldComparer)
		{
			Array.Sort(array, index, length, new ArraySort(sortFields, fieldComparer));
		}

		public static void Sort(Array array, int[] sortFields, System.Collections.IComparer[] fieldComparers)
		{
			Sort(array, 0, array.Length, sortFields, fieldComparers);
		}

		public static void Sort(Array array, int[] sortFields, IFieldComparer fieldComparer)
		{
			Sort(array, 0, array.Length, sortFields, fieldComparer);
		}

		private int[]							_sortFields;
		private System.Collections.IComparer[]	_fieldComparers;
		private IFieldComparer					_fieldComparer;

		public ArraySort(int[] sortFields, System.Collections.IComparer[] fieldComparers)
		{
			_sortFields = sortFields;
			_fieldComparers = fieldComparers;
			_fieldComparer = null;
		}

		public ArraySort(int[] sortFields, IFieldComparer fieldComparer)
		{
			_sortFields = sortFields;
			_fieldComparers = null;
			_fieldComparer = fieldComparer;
		}

		#region IComparer Members

		public int Compare(object x, object y)
		{
			for (int n = 0; n < _sortFields.Length; n++)
			{
				bool desc = false;
				int field = _sortFields[n];
				if (field < 0)
				{
					field -= Int32.MinValue;
					desc = true;
				}

				int r = _fieldComparer == null ? _fieldComparers[field].Compare(x, y) :
					_fieldComparer.Compare(field, x, y);
				if (r != 0)
				{
					return desc ? -r : r;
				}
			}

			return 0;
		}

		#endregion
	}
}
