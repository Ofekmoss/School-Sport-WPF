using System;
using System.Collections;

namespace Sport.Common
{
	public class ValueFormatterItem
	{
		private string _format;
		public string Format
		{
			get { return _format; }
		}

		private int _valuePart;
		public int ValuePart
		{
			get { return _valuePart; }
		}

		private int _maximum;
		public int Maximum
		{
			get { return _maximum; }
		}

		private int _length;
		public int Length
		{
			get { return _length; }
		}

		public ValueFormatterItem(string format, int valuePart,
			int maximum, int length)
		{
			_format = format;
			_valuePart = valuePart;
			_maximum = maximum;
			_length = length;
		}
	}

	public class ValueFormatter
	{
		public static readonly ValueFormatterItem[] TimeValueFormatters = new ValueFormatterItem[]
			{
				new ValueFormatterItem("d", 86400000, 999, 0),
				new ValueFormatterItem("DDD", 86400000, 999, 3),
				new ValueFormatterItem("DD", 86400000, 99, 2),
				new ValueFormatterItem("D", 86400000, 9, 1),
				new ValueFormatterItem("h", 3600000, 9999, 0),
				new ValueFormatterItem("HHHH", 3600000, 9999, 4),
				new ValueFormatterItem("HHH", 3600000, 999, 3),
				new ValueFormatterItem("HH", 3600000, 23, 2),
				new ValueFormatterItem("m", 60000, 9999, 0),
				new ValueFormatterItem("MMMM", 60000, 9999, 4),
				new ValueFormatterItem("MMM", 60000, 999, 3),
				new ValueFormatterItem("MM", 60000, 59, 2),
				new ValueFormatterItem("s", 1000, 99999, 0),
				new ValueFormatterItem("SSSS", 1000, 9999, 4),
				new ValueFormatterItem("SSS", 1000, 999, 3),
				new ValueFormatterItem("SS", 1000, 59, 2),
				new ValueFormatterItem("T", 100, 9, 1),
				new ValueFormatterItem("TT", 10, 99, 2),
				new ValueFormatterItem("TTT", 1, 999, 3)
			};

		public static readonly ValueFormatterItem[] DistanceValueFormatters = new ValueFormatterItem[]
			{
				new ValueFormatterItem("k", 1000000, 999, 0),
				new ValueFormatterItem("KK", 1000000, 99, 2),
				new ValueFormatterItem("K", 1000000, 9, 1),
				new ValueFormatterItem("m", 1000, 999, 0),
				new ValueFormatterItem("mm", 1000, 99999, 0),
				new ValueFormatterItem("MMM", 1000, 999, 3),
				new ValueFormatterItem("MM", 1000, 99, 2),
				new ValueFormatterItem("M", 1000, 9, 1),
				new ValueFormatterItem("H", 100, 9, 1),
				new ValueFormatterItem("c", 10, 9999, 0),
				new ValueFormatterItem("CCC", 10, 999, 3),
				new ValueFormatterItem("CC", 10, 99, 2),
				new ValueFormatterItem("C", 10, 9, 1),
				new ValueFormatterItem("LLL", 1, 999, 3)
			};

		public static readonly ValueFormatterItem[] PointValueFormatters = new ValueFormatterItem[]
			{
				new ValueFormatterItem("p", 1000, 99999999, 0),
				new ValueFormatterItem("PPPP", 1000, 9999, 4),
				new ValueFormatterItem("PPP", 1000, 999, 3),
				new ValueFormatterItem("PP", 1000, 99, 2),
				new ValueFormatterItem("P", 1000, 9, 1),
				new ValueFormatterItem("DDD", 1, 999, 3),
				new ValueFormatterItem("DD", 10, 99, 2),
				new ValueFormatterItem("D", 100, 9, 1)
			};

		#region FormatOrderComparer

		private class FormatOrderComparer : IComparer
		{
			private ValueFormatter _formatter;
			public FormatOrderComparer(ValueFormatter formatter)
			{
				_formatter = formatter;
			}

			#region IComparer Members

			public int Compare(object x, object y)
			{
				return _formatter.FormatItems[(int) y].Formatter.ValuePart.CompareTo(
					_formatter.FormatItems[(int) x].Formatter.ValuePart);
			}

			#endregion
		}

		#endregion

		private ValueFormatterItem[] _formatters;
		public ValueFormatterItem[] Formatters
		{
			get { return _formatters; }
		}

		public ValueFormatter(ValueFormatterItem[] formatters, string format)
		{
			_formatters = formatters;
			if (format != null)
				SetFormat(format);
		}

		private string _format;
		public string Format
		{
			get { return _format; }
		}

		private string _textFormat;
		public string TextFormat
		{
			get { return _textFormat; }
		}

		private string _baseText;
		public string BaseText
		{
			get { return _baseText; }
		}

		public struct FormatItem
		{
			private ValueFormatterItem _formatter;
			public ValueFormatterItem Formatter
			{
				get { return _formatter; }
			}

			private int _baseIndex;
			public int BaseIndex
			{
				get { return _baseIndex; }
			}

			public FormatItem(ValueFormatterItem formatter, int baseIndex)
			{
				_formatter = formatter;
				_baseIndex = baseIndex;
			}
		}

		private FormatItem[] _formatItems;
		public FormatItem[] FormatItems
		{
			get { return _formatItems; }
		}

		private int[] _formatItemsOrder;

		public void SetFormat(string format)
		{
			if (format == null || format.Length == 0)
				throw new Exception("Format must have a value");

			_baseText = "";

			ArrayList al = new ArrayList();

			int baseIndex = 0;
			int start = 0;
			int end;
			int pos = format.IndexOf('{');
			while (pos >= 0)
			{
				if (pos > 0)
				{
					_baseText += format.Substring(start, pos - start);
					baseIndex = _baseText.Length;
				}

				end = format.IndexOf('}', pos + 1);

				if (end < 0)
				{
					pos = -1;
				}
				else
				{
					int i = -1;
					string s = format.Substring(pos + 1, end - pos - 1);

					for (int f = 0; f < _formatters.Length && i == -1; f++)
					{
						if (s == _formatters[f].Format)
						{
							i = f;
						}
					}

					if (i != -1)
					{
						al.Add(new FormatItem(_formatters[i], baseIndex));
					}

					start = end + 1;
					pos = format.IndexOf('{', start);
				}
			}

			_baseText += format.Substring(start);

			_formatItems = (FormatItem[]) al.ToArray(typeof(FormatItem));

			_formatItemsOrder = new int[_formatItems.Length];

			_format = _baseText;
			_textFormat = _baseText;
			int d = 0;
			int d2 = 0;
			string par;

			for (int n = 0; n < _formatItems.Length; n++)
			{
				_formatItemsOrder[n] = n;
				_format = _format.Insert(_formatItems[n].BaseIndex + d,
					"{" + _formatItems[n].Formatter.Format + "}");
				d += 2 + _formatItems[n].Formatter.Format.Length;

				par = n.ToString() + ":0";
				for (int i = 0; i < _formatItems[n].Formatter.Length - 1; i++)
					par += '0';
				_textFormat = _textFormat.Insert(_formatItems[n].BaseIndex + d2,
					"{" + par + "}");
				d2 += par.Length + 2;
			}

			Array.Sort(_formatItemsOrder, new FormatOrderComparer(this));
		}

		public int GetValue(int[] values)
		{
			if (values.Length != _formatItems.Length)
				throw new Exception("Values don't match format");

			int v = 0;

			for (int n = 0; n < values.Length; n++)
			{
				FormatItem item = _formatItems[_formatItemsOrder[n]];

				v += values[n] * item.Formatter.ValuePart;
			}

			return v;
		}

		public int[] GetValues(int value)
		{
			int[] result = new int[_formatItems.Length];
			
			int v = value;

			int item;

			for (int n = 0; n < _formatItemsOrder.Length; n++)
			{
				int valuePart = _formatItems[_formatItemsOrder[n]].Formatter.ValuePart;
				item = v / valuePart;
				result[n] = item;
				v -= item * valuePart;
			}

			return result;
		}

		public string GetText(int value)
		{
			int[] values = GetValues(value);
			object[] pars = new object[values.Length];
			values.CopyTo(pars, 0);

			return String.Format(_textFormat, pars);
		}

		public override string ToString()
		{
			return _format;
		}

	}
}
