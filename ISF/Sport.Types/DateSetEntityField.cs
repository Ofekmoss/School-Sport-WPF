using System;
using Sport.Data;

namespace Sport.Types
{
	public class DateSetType : ICloneable
	{
		private object _start;
		public object Start
		{
			get { return _start; }
			set { _start = value; }
		}

		private object _end;
		public object End
		{
			get { return _end; }
			set { _end = value; }
		}

		private object _altStart;
		public object AltStart
		{
			get { return _altStart; }
			set { _altStart = value; }
		}

		private object _altEnd;
		public object AltEnd
		{
			get { return _altEnd; }
			set { _altEnd = value; }
		}

		public DateSetType(object start, object end,
			object altStart, object altEnd)
		{
			_start = start;
			_end = end;
			_altStart = altStart;
			_altEnd = altEnd;
		}

		public DateSetType(object start, object altStart)
		{
			_start = start;
			_end = null;
			_altStart = altStart;
			_altEnd = null;
		}

		public DateSetType()
		{
			_start = null;
			_end = null;
			_altStart = null;
			_altEnd = null;
		}

		private string GetDatesText(object s, object e)
		{
			if (s is DateTime && e is DateTime)
			{
				DateTime sd = (DateTime) s;
				DateTime ed = (DateTime) e;
				string eds = ed.ToString("dd/MM/yyyy");

				if (sd.Year == ed.Year)
				{
					if (sd.Month == ed.Month)
					{
						if (sd.Day == ed.Day)
							return eds;
						return sd.ToString("dd-") + eds;
					}

					return sd.ToString("dd/MM-") + eds;
				}

				return sd.ToString("dd/MM/yyyy-") + eds;
			}
			else if (s is DateTime)
			{
				return ((DateTime) s).ToString("dd/MM/yyyy");
			}
			else if (e is DateTime)
			{
				return ((DateTime) e).ToString("dd/MM/yyyy");
			}

			return null;
		}

		public override string ToString()
		{
			string original = GetDatesText(_start, _end);
			string alternate = GetDatesText(_altStart, _altEnd);
		
			if (alternate == null)
				return original;
			if (original == null)
				return alternate;

			return original + "\nημετι: " + alternate;
		}

		#region ICloneable Members

		public object Clone()
		{
			return new DateSetType(_start, _end, _altStart, _altEnd);
		}

		#endregion
	}

	/// <summary>
	/// DateSetEntityField inherits EntityField to implement
	/// a field as a combination of other date fields
	/// </summary>
	public class DateSetEntityField : EntityField
	{
		#region Properties

		public override bool MustExist
		{
			get { return false; }
		}

		private int _startField;
		public int StartField
		{
			get { return _startField; }
		}

		private int _endField;
		public int EndField
		{
			get { return _endField; }
		}

		private int _altStartField;
		public int AltStartField
		{
			get { return _altStartField; }
		}

		private int _altEndField;
		public int AltEndField
		{
			get { return _altEndField; }
		}

		#endregion

		#region Constructor

		// DateSetEntityField constructor, receives start, end,
		// alternate start and alternate end date fields
		public DateSetEntityField(EntityType type, int index,
			int startField, int endField, int altStartField, int altEndField)
			: base(type, index)
		{
			_startField = startField;
			_endField = endField;
			_altStartField = altStartField;
			_altEndField = altEndField;
		}

		// DateSetEntityField constructor, receives start and
		// alternate start date fields
		public DateSetEntityField(EntityType type, int index,
			int startField, int altStartField)
			: this(type, index, startField, -1, altStartField, -1)
		{
		}

		#endregion

		#region Value Operations

		// Overrides GetText to return the GetText value
		// of the relative field
		public override string GetText(Entity e)
		{
			return GetValue(e).ToString();
		}

		// Overrides SetValue to disable change to the format
		// field
		public override void SetValue(EntityEdit e, object value)
		{
			DateSetType dst = value as DateSetType;
			if (dst != null)
			{
				if (_startField != -1)
				{
					_type.Fields[_startField].SetValue(e, dst.Start);
				}
				if (_endField != -1)
				{
					_type.Fields[_endField].SetValue(e, dst.End);
				}
				if (_altStartField != -1)
				{
					_type.Fields[_altStartField].SetValue(e, dst.AltStart);
				}
				if (_altEndField != -1)
				{
					_type.Fields[_altEndField].SetValue(e, dst.AltEnd);
				}
			}
			else
				throw new EntityException("Given value cannot be set to field");
		}

		// Overrides GetValue to return a date set
		public override object GetValue(Entity e)
		{
			DateSetType dst = new DateSetType();

			if (_startField != -1)
			{
				dst.Start = _type.Fields[_startField].GetValue(e);
			}
			if (_endField != -1)
			{
				dst.End = _type.Fields[_endField].GetValue(e);
			}
			if (_altStartField != -1)
			{
				dst.AltStart = _type.Fields[_altStartField].GetValue(e);
			}
			if (_altEndField != -1)
			{
				dst.AltEnd = _type.Fields[_altEndField].GetValue(e);
			}

			return dst;
		}

		#endregion

		#region Value Comoparison

		// Overrides IsEmpty to whether the text
		// is empty
		public override bool IsEmpty(Entity e)
		{
			string text=GetText(e);
			if (text == null)
				return true;
			if (text.Length == 0)
				return true;
			return false;
		}

		#endregion
	}
}
