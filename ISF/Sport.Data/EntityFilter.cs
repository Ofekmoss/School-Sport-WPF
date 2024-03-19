using System;
using Sport.Common;
using System.Collections;

namespace Sport.Data
{
	/// <summary>
	/// EntityFilterField stores the field and the value
	/// of a single field in a filter.
	/// An EntityFilter is determined by a set of EntityFilterFields.
	/// </summary>
	[Serializable]
	public class EntityFilterField
	{
		// Stores the field index that will be compared in the filter.
		protected int _field;
		public int Field
		{ 
			get 
			{ 
				return _field; 
			}
		}

		// Stores the value that will be compared with the field in the filter
		protected object _value;
		public object Value			
		{ 
			get 
			{ 
				return _value; 
			} 
		}

		// Stores the 'not' flag for the filter field, if 'not' is set
		// than the filter field will be matched as not true
		protected bool _not;
		public bool Not
		{
			get { return _not; }
		}

		// The EntityFilterField constructor, receives a field
		// and a value to be compared with a given entity
		// and a 'not' flag
		public EntityFilterField(int field, object value, bool not)
		{
			_field = field;
			_value = value;
			_not = not;
		}

		// The EntityFilterField constructor, receives a field
		// and a value to be compared with a given entity
		public EntityFilterField(int field, object value)
			: this(field, value, false)
		{
		}

		public ArrayList GetValueAsStringArray()
		{
			if (_value is System.Array)
			{
				System.Array array = (System.Array)_value;
				if (array != null)
				{
					ArrayList list = new ArrayList();
					for (int n = 0; n < array.Length; n++)
					{
						object item = array.GetValue(n);
						if (item != null)
							list.Add(item.ToString());
					}
					return list;
				}
			}
			return null;
		}

		// Equals compares two EntityFilterFields
		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;

			// In order for them to be equal they both need
			// to be of type EntityFilterField...
			if (obj is EntityFilterField)
			{
				EntityFilterField field=(EntityFilterField) obj;
				
				// ... they both have to have the same field...
				if (field._field != _field)
					return false;
				
				//maybe both null?
				if ((field._value == null)&&(this._value == null))
					return field.Not == this._not;
				
				//maybe null?
				if (field._value == null)
					return false;
				
				// ... and they both have to have the same value.
				if (field._value is System.Array)
				{
					if (!(this._value is System.Array))
						return false;
					ArrayList list1 = field.GetValueAsStringArray();
					ArrayList list2 = this.GetValueAsStringArray();
					if (list1.Count != list2.Count)
						return false;
					for (int n = 0; n < list1.Count; n++)
					{
						if (!list1[n].ToString().Equals(list2[n].ToString()))
						{
							return false;
						}
					}
				}
				else
				{
					if (!field._value.Equals(this._value))
						return false;
				}
				
				return field.Not == this._not;
			}

			return false;
		}

		// Returns the hash code for the filter field
		public override int GetHashCode()
		{
			// The hash code is a mix of the field's index and the value
			// but maybe we have null value
			if (_value == null)
				return base.GetHashCode();
			return (_field << 20) ^ _value.GetHashCode();
		}

		// Overriding == to use Equals
		public static bool operator==(EntityFilterField a, EntityFilterField b)
		{
			if ((object) a == null)
				return (object) b == null;
			return a.Equals(b);
		}

		// Overriding != to use !Equals
		public static bool operator!=(EntityFilterField a, EntityFilterField b)
		{
			if ((object) a == null)
				return (object) b != null;
			return !a.Equals(b);
		}

		/// <summary>
		/// CompareValue is called to compare the value of the entity's
		/// field with the filter field value.
		/// CheckValue can be overriden to implement specific
		/// filters.
		/// </summary>
		public virtual bool CompareValue(Entity e)
		{
			// Default behavior, compare entity's
			// value if the filter field to the filter value
			if (_value is System.Array)
			{
				System.Array values = (System.Array) _value;
				for (int v = 0; v < values.Length; v++)
				{
					if (e.EntityType.Fields[_field].Equals(e, values.GetValue(v)))
						return true;
				}

				return false;
			}

			return e.EntityType.Fields[_field].Equals(e, _value);
		}

		/// <summary>
		/// Filtered is called by the EntityFilter to check
		/// is the given entity fits in the filter.
		/// </summary>
		public bool Filtered(Entity e)
		{
			// If 'not' flag is set...
			if (_not)
				//... the entity is filtered if it matches the filter field
				return CompareValue(e);
			else
				//... otherwise the entity is filtered if it doesn't match
				// the filter field
				return !CompareValue(e);
		}

		public virtual EntityFilterField ToArrayFilterField()
		{
			/*
			if (_value is System.Array)
				return null;
			*/
			return new EntityFilterField(_field, _value, _not);
		}

		public virtual Sport.Data.SportServices.FilterField ToFilterField(EntityType entityType)
		{
			/*
			if (_value is System.Array)
				return null; // Not setting 'or' filter for read from server
				
				WHY NOT??????????????????
			*/
			
			if (_field >= entityType.DataFields || _field < 0)
				return null;
			Sport.Data.SportServices.FilterField ff = new Sport.Data.SportServices.FilterField();
			ff.Field = _field;
			ff.Value = _value;
			ff.Not = _not;
			return ff;
		}

		public override string ToString()
		{
			//deal with nasty null values.
			string strValue = "";
			if (_value != null)
			{
				if (_value is System.Array)
				{
					ArrayList arrValues = this.GetValueAsStringArray();
					for (int i = 0; i < arrValues.Count; i++)
					{
						strValue += arrValues[i].ToString();
						if (i < (arrValues.Count - 1))
							strValue += ",";
					}
				}
				else
				{
					strValue = _value.ToString();
				}
			}
			return _field.ToString() + (_not ? " != " : " = ") + strValue;
		}

	}

	/// <summary>
	/// EntityFilter stores a set of filter fields and preforms
	/// comparisons to check is a given entity is filtered.
	/// The EntityFilter also implements filter comparison
	/// and hash code functions to enable to use of a filter
	/// as a key.
	/// </summary>
	[Serializable]
	public class EntityFilter : IList, ICollection, IEnumerable,
		System.Runtime.Serialization.ISerializable
	{
		public static readonly EntityFilter Empty = new EntityFilter();

		/// <summary>
		/// FieldComparer compare between two EntityFilterFields
		/// by the index of their field
		/// </summary>
		public class FieldComparer : IComparer
		{
			public int Compare(object x, object y)
			{
				int ix = x is EntityFilterField ? ((EntityFilterField)x).Field : (int) x;
				int iy = y is EntityFilterField ? ((EntityFilterField)y).Field : (int) y;

				if (ix < iy)
					return -1;
				if (ix > iy)
					return 1;

				return 0;
			}
		}

		private SortedArray _fields;

		// Returns the EntityFilterField in the given index
		public EntityFilterField this[int index]
		{
			get 
			{
				return (EntityFilterField) _fields[index];
			}
			set
			{
				_fields[index] = value;
			}
		}

		// GetField returns the EntityFilterField of the given
		// field index.
		public EntityFilterField GetField(int field)
		{
			int index = _fields.IndexOf(field);
			if (index < 0)
				return null;
			return this[index];
		}


		// EntityFilter default constructor
		public EntityFilter()
		{
			_fields = new SortedArray(new FieldComparer());
		}

		// The EntityFilter constructor, receives an EntityFilter
		// and copies it
		public EntityFilter(EntityFilter entityFilter)
			: this()
		{
			_fields.AddRange(entityFilter._fields);
		}

		// The EntityFilter constructor, receives an array
		// of EntityFilterFields and adds them to itself
		public EntityFilter(EntityFilterField[] fields)
			: this()
		{
			//_fields = new SortedArray(new FieldComparer());
			_fields.AddRange(fields);
		}

		// The EntityFilter constructor, receives a
		// single EntityFilterField
		public EntityFilter(EntityFilterField field)
			: this()
		{
			Add(field);
		}

		// The EntityFilter constructor, receives
		// a field and its value
		public EntityFilter(int field, object value)
			: this(new EntityFilterField(field, value))
		{
		}

		// Filtered checks if the given entity is filtered
		// by the filter
		public bool Filtered(Entity e)
		{
			try
			{
				foreach (EntityFilterField eff in this)
				{
					if (eff.Filtered(e))
						return true;
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("error in Filtered: "+ex.Message);
				System.Diagnostics.Debug.WriteLine(ex.StackTrace);
				return false;
			}

			return false;
		}

		// Equals compares two EntityFilters
		public override bool Equals(object obj)
		{
			// In order for them to be equal they both need
			// to be of type EntityFilter...
			if (obj is EntityFilter)
			{
				EntityFilter e = (EntityFilter) obj;

				// ... they both have to have the same amount of fields ...
				if (e.Count != this.Count)
					return false;
				
				// ... and all the fields in the filter must be equal.
				for (int n = 0; n < e.Count; n++)
				{
					// This type of comparison is possible because
					// the EntityFilter is a sorted array
					if (e[n] != this[n])
						return false;
				}

				return true;
			}

			return false;
		}

		// Returns a hash code for the EntityFilter
		public override int GetHashCode()
		{
			// The hash code is a mix of the fields amount in the
			// filter and their hash codes
			int hash = this.Count;

			foreach (EntityFilterField eff in this)
			{
				hash ^= eff.GetHashCode();
			}
			return hash;
		}

		// Overriding == to use Equals
		public static bool operator==(EntityFilter a, EntityFilter b)
		{
			if ((object) a == null)
				return (object) b == null;
			return a.Equals(b);
		}

		// Overriding != to use !Equals
		public static bool operator!=(EntityFilter a, EntityFilter b)
		{
			if ((object) a == null)
				return (object) b != null;
			return !a.Equals(b);
		}


		// ToArrayFilter returns an EntityFilter of the
		// filter fields that are used in reading entities
		// from server and storing the array in memory
		public EntityFilter ToArrayFilter()
		{
			EntityFilter filter = new EntityFilter();
			for (int n = 0; n < this.Count; n++)
			{
				EntityFilterField eff = this[n].ToArrayFilterField();
				if (eff != null)
					filter._fields.Add(eff);
			}
            
			return filter;
		}

		// ToFilterFields returns an array of 
		// Sport.Data.SportServices.FilterField objects
		// each containing field index and value of
		// a single EntityFilterField
		public Sport.Data.SportServices.FilterField[] ToFilterFields(EntityType entityType)
		{
			if (this.Count == 0)
				return null;

			ArrayList fields = new ArrayList();

			for (int n = 0; n < this.Count; n++)
			{
				Sport.Data.SportServices.FilterField ff = this[n].ToFilterField(entityType);
				if (ff != null)
					fields.Add(ff);
			}

			if (fields.Count > 0)
				return (Sport.Data.SportServices.FilterField[]) fields.ToArray(typeof(Sport.Data.SportServices.FilterField));

			return null;
		}

		// EntityFilter constructor that create a filter
		// from Sport.Data.SportServices.FilterField objects
		public EntityFilter(Sport.Data.SportServices.FilterField[] fields)
		{
			_fields = new SortedArray(new FieldComparer());
			for (int f = 0; f < fields.Length; f++)
			{
				Add(new EntityFilterField(fields[f].Field, 
					fields[f].Value, fields[f].Not));
			}
		}

		public enum Differences
		{
			NotEqual,
			Contained,
			Containing,
			Equal
		}

		// Compare between two filters
		// Equal: both filters have same filter fields
		// Containing: all fields of the given filter
		//   exist in this filter, this filter is a subset
		//   of the given one
		// Contained: all fields of this filter
		//   exist in the given filter, the given filter is a 
		//   subset of this one
		// NotEqual: one of the fields is not equal to
		//   the field in the given filter
		public Differences Compare(EntityFilter filter)
		{
			if (this.Count == filter.Count)
			{
				return Equals(filter) ? Differences.Equal : Differences.NotEqual;
			}

			bool bigger = this.Count > filter.Count;
			EntityFilter b = bigger ? this : filter;
			EntityFilter s = bigger ? filter : this;

			int bn = 0;

			for (int n = 0; n < s.Count; n++)
			{
				while (bn < b.Count && s[n].Field > b[bn].Field)
					bn++;
				if (bn == b.Count)
					return Differences.NotEqual;
				if (b[bn] != s[n])
					return Differences.NotEqual;
			}

			return bigger ? Differences.Containing : Differences.Contained;
		}

		public override string ToString()
		{
			string result="";
			for (int n = 0; n < _fields.Count; n++)
			{
				result +=  _fields[n].ToString();
				if (n < (_fields.Count-1))
					result += ", ";
			}
			return result;
		}


		#region IList Members

		public bool IsReadOnly
		{
			get
			{
				return _fields.IsReadOnly;
			}
		}

		object System.Collections.IList.this[int index]
		{
			get
			{
				return _fields[index];
			}
			set
			{
				_fields[index] = value;
			}
		}

		public void RemoveAt(int index)
		{
			_fields.RemoveAt(index);
		}

		void IList.Insert(int index, object value)
		{
			((IList)_fields).Insert(index, value);
			
		}

		void IList.Remove(object value)
		{
			_fields.Remove(value);
		}

		public void Remove(EntityFilterField filterField)
		{
			_fields.Remove(filterField);
		}

		bool IList.Contains(object value)
		{
			return _fields.Contains(value);
		}

		public bool Contains(EntityFilterField filterField)
		{
			return _fields.Contains(filterField);
		}

		public void Clear()
		{
			_fields.Clear();
		}

		int IList.IndexOf(object value)
		{
			return _fields.IndexOf(value);
		}

		public int IndexOf(EntityFilterField filterField)
		{
			return _fields.IndexOf(filterField);
		}

		int IList.Add(object value)
		{
			return _fields.Add(value);
		}
		
		public int Add(EntityFilterField filterField)
		{
			return _fields.Add(filterField);
		}

		public bool IsFixedSize
		{
			get
			{
				return _fields.IsFixedSize;
			}
		}

		#endregion

		#region ICollection Members

		public bool IsSynchronized
		{
			get
			{
				return _fields.IsSynchronized;
			}
		}

		public int Count
		{
			get
			{
				return _fields.Count;
			}
		}

		public void CopyTo(Array array, int index)
		{
			_fields.CopyTo(array, index);
		}

		public object SyncRoot
		{
			get
			{
				return _fields.SyncRoot;
			}
		}

		#endregion

		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			return _fields.GetEnumerator();
		}

		#endregion

		#region ISerializable Members

		protected EntityFilter(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			EntityType type = context.Context as EntityType;
			_fields = new SortedArray(new FieldComparer());

			EntityFilterField[] fields = (EntityFilterField[])
				info.GetValue("fields", typeof(EntityFilterField[]));

			if (fields != null)
			{
				for (int f = 0; f < fields.Length; f++)
				{
					Add(fields[f]);
				}
			}
		}

		public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			EntityFilterField[] fields = new EntityFilterField[_fields.Count];
			_fields.CopyTo(fields, 0);

			info.AddValue("fields", fields);
		}

		#endregion
	}
}
