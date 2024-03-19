using System;
using Mir;

namespace Sport.Data
{
	#region EntityField Class

	/// <summary>
	/// EntityField stores information about a single field
	/// of an entity type.
	/// The EntityField is used to retrieve a specific fields's data 
	/// from an Entity, change a specific field's data in an EntityEdit
	/// and compare between two entities by their field.
	/// This class is inherited to implement different types of fields.
	/// </summary>
	public class EntityField
	{
		#region Properties

		// The index of the field in the entity's fields array
		protected int			_index;
		public int Index
		{
			get { return _index; }
		}

		// The entity type to which this field is belong
		protected EntityType	_type;
		public EntityType Type
		{
			get { return _type; }
		}

		// Stores whether this field can be edited
		protected bool			_canEdit;
		public virtual bool CanEdit
		{
			get { return _canEdit; }
			set { _canEdit = value; }
		}

		// Stores whether this field can have an empty value
		protected bool			_mustExist;
		public virtual bool MustExist
		{
			get { return _mustExist; }
			set { _mustExist = value; }
		}

		private string _name;
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		#endregion

		#region Constructor

		// EntityField constructor, receives the type
		// to which this field belongs and the index
		// in its type's fields array
		public EntityField(EntityType type, int index)
		{
			_type = type;
			_index = index;
			// By default a field can be edited and can have
			// an empty value
			_canEdit = true;
			_mustExist = false;
		}

		#endregion

		#region Value Operations

		// GetText returns the displayed text of the field's
		// value in the given Entity
		// This function is overriden for different types
		// of fields
		public virtual string GetText(Entity e)
		{
			if (e == null || e.Fields[_index] == null)
				return null;

			// The base field type just returns the default
			// string value of the object
			return e.Fields[_index].ToString();
		}

		// GetObjectValue returns the value's value to
		// be set in the field
		// This function is overriden for different types
		// of fields
		protected virtual object GetObjectValue(object value)
		{
			// The base field type stores the default
			// string value of the given object as
			// the field's value
			return value == null ? null : value.ToString();
		}

		// Gets the CanEdit value of the field for the given
		// entity, from the entity type.
		public bool CanEditEntity(Entity entity)
		{
			return _type.CanEditField(_index, entity);
		}

		// SetValue stores the given value in the field of
		// the given EntityEdit
		// This function calls GetObjectValue to get the correct
		// object for the field
		public virtual void SetValue(EntityEdit e, object value)
		{
			// If this EntityEdit is not for a new entity (e.Entity == null)
			// than can only set value if this field can be edited
			if (e == null)
				return;
			if (_canEdit || e.Entity == null)
			{
				object val = GetObjectValue(value);

				if (Equals(e, val)) // Setting to same value
				{
					//somehow need to cancel pending events.......
					return ;// nothing needs to be done
				}

				if ((e.Fields != null)&&(_index < e.Fields.Length))
					e.Fields[_index] = val;
				
				e.OnValueChange(this);
			}
			else
			{
				throw new EntityException("Cannot edit non-editable field");
			}
		}

		// GetValue returns the value of the given field of
		// the given Entity
		// This function can be overriden for different types
		// of fields
		public virtual object GetValue(Entity e)
		{
			// Default behavior, return field value by index
			return e.Fields[_index];
		}

		// GetValueItem returns the item associated to the given
		// field of the given Entity
		// This function can be overriden for different types
		// of fields
		public virtual object GetValueItem(Entity e)
		{
			// Default behavior, return the field's value
			return GetValue(e);
		}

		// Convert value returns the given value converted to
		// the type of the field
		// This function can be overriden for different types
		// of fields
		public virtual object ConvertValue(object value)
		{
			// Default behavior, return the object value
			return GetObjectValue(value);
		}

		#endregion

		#region Value Comparison

		// IsEmpty returns whether the field of the given entity
		// has a value
		// This function can be overriden for different types
		// of fields
		public virtual bool IsEmpty(Entity e)
		{
			// Default behavior value is null or its text is null
			if (e == null)
				return true;
			if (e.Fields == null)
				return true;
			if (e.Fields[_index] == null)
				return true;
			object text=GetText(e);
			if (text == null)
				return true;
			if (text.ToString().Length == 0)
				return true;
			return false;
		}

		// Compare between the field of one entity to
		// a given object
		// This function is override for different types
		// of fields.
		public virtual int Compare(Entity x, object y)
		{
			// The base field type compares strings
			string sx = GetText(x);
			string sy = (y == null) ? null : y.ToString();

			if (sx != null)
				return sx.ToString().CompareTo(sy);
			if (sy != null)
				return -1;
			return 0;
		}

		// Compare between the fields of two entities
		// This function is overriden for different types
		// of fields.
		public virtual int Compare(Entity x, Entity y)
		{
			// The base field type compares the texts
			// of the entities' fields
			string sx = GetText(x);
			string sy = GetText(y);

			if (sx != null)
				return sx.ToString().CompareTo(sy);
			if (sy != null)
				return -1;
			return 0;
		}

		// Equals checks if the value of the field in the
		// given entity is equal to the given value
		public virtual bool Equals(Entity e, object value)
		{
			// The base field type compare the string
			// value of the field with the given value
			if ((e == null)&&(value == null))
				return true;
			string s  = GetText(e);
			if (s == null)
				return value == null;
            
			if (value == null)
				return false;

			return s.CompareTo(value.ToString()) == 0;	
		}

		public bool Equals(Entity x, Entity y)
		{
			return Equals(x, GetValue(y));
		}

		#endregion

		#region Object Members

		public override string ToString()
		{
			return _name;
		}

		#endregion
	}

	#endregion

	#region TextEntityField Class

	/// <summary>
	/// TextEntityField inherits EntityField to add text
	/// field parameters to the, already text oriented, EntityField
	/// </summary>
	public class TextEntityField : EntityField
	{
		#region Properties

		private bool _multiline;
		public bool Multiline
		{
			get { return _multiline; }
		}

		private bool _expandVariables;
		public bool ExpandVariables
		{
			get { return _expandVariables; }
		}

		#endregion

		#region Constructors

		public TextEntityField(EntityType type, int index, bool multiline, bool expandVariables)
			: base(type, index)
		{
			_multiline = multiline;
			_expandVariables = expandVariables;
		}

		public TextEntityField(EntityType type, int index, bool multiline)
			: this(type, index, multiline, false)
		{
		}

		public TextEntityField(EntityType type, int index)
			: this(type, index, false)
		{
		}

		#endregion

		#region Value Operations

		// Overrides GetText to give different implementaion if variables
		// are to be expanded
		public override string GetText(Entity e)
		{
			if (!_expandVariables)
				return base.GetText(e);

			if (e.Fields[_index] == null)
				return null;

			return ExpandString.Expand(e.Fields[_index].ToString());
		}

		// Overrides GetValueItem to return an ExpandString if variables
		// are to be expanded
		public override object GetValueItem(Entity e)
		{
			if (!_expandVariables)
				return base.GetValueItem(e);

			return new ExpandString((string) GetValue(e));
		}

		// Overrides ConvertValue to return the string of the value
		public override object ConvertValue(object value)
		{
			if (value is ExpandString)
			{
				return ((ExpandString) value).Text;
			}

			return base.ConvertValue(value);
		}


		#endregion

	}

	#endregion

	#region NumberEntityField Class

	/// <summary>
	/// NumberEntityField inherits EntityField to implement
	/// different behavior needed for fields with int
	/// values.
	/// </summary>
	public class NumberEntityField : EntityField
	{
		#region Value Range Properties

		private double _minValue;
		public double MinValue
		{
			get { return _minValue; }
		}

		private double _maxValue;
		public double MaxValue
		{
			get { return _maxValue; }
			set
			{
				_maxValue = value;
			}
		}

		private byte _scale;
		public byte Scale
		{
			get { return _scale; }
		}

		private byte _precision;
		public byte Precision
		{
			get { return _precision; }
		}

		#endregion

		#region Constructors

		public NumberEntityField(EntityType type, int index,
			double min, double max, byte scale, byte precision)
			: base(type, index)
		{
			_minValue = min;
			_maxValue = max;
			_scale = scale;
			_precision = precision;
		}

		public NumberEntityField(EntityType type, int index,
			double min, double max)
			: this(type, index, min, max, byte.MaxValue, 0)
		{
		}

		public NumberEntityField(EntityType type, int index,
			byte scale, byte precision)
			: this(type, index, double.MinValue, double.MaxValue, scale, precision)
		{
		}

		// The NumberEntityField costructor
		public NumberEntityField(EntityType type, int index)
			: this(type, index, double.MinValue, double.MaxValue)
		{
		}

		#endregion

		#region Value Operations

		// Overrides GetObjectValue to receive int value
		protected override object GetObjectValue(object value)
		{
			if (value == null)
				return null;
			else if (value is int || value is double)
			{
				double n = System.Convert.ToDouble(value); //(double) value;
				// If min and max values are defined...
				if (_minValue <= _maxValue)
				{
					// ... check that value is in range
					if (n < _minValue)
						n = _minValue;
					else if (n > _maxValue)
						n = _maxValue;
					/* if (n < _minValue ||
						n > _maxValue)
						throw new EntityException("Ivalid argument, int value out of field specified range"); */
				}
				if (_precision == 0)
					return System.Convert.ToInt32(n);
				return n;
			}
			else
				throw new EntityException("Invalid argument, expecting int value");
		}

		public override object ConvertValue(object value)
		{
			if (_precision == 0)
				return System.Convert.ToInt32(value);

			return System.Convert.ToDouble(value);
		}

		#endregion

		#region Value Comparison

		// Overrides Compare to preform int comparison
		public override int Compare(Entity x, Entity y)
		{
			double ix = Sport.Common.Tools.CDblDef(x.Fields[_index], Double.MinValue); //(x.Fields[_index] == null) ? Double.MinValue : (double) x.Fields[_index];
			double iy = Sport.Common.Tools.CDblDef(y.Fields[_index], Double.MinValue); //(y.Fields[_index] == null) ? Double.MinValue : (double) y.Fields[_index];

			if (ix < iy)
				return -1;
			else if (iy < ix)
				return 1;

			return 0;
		}

		// Overrides Equals to preform int comparison
		public override bool Equals(Entity e, object value)
		{
			object fieldValue=e.Fields[_index];
			if ((fieldValue == null)&&(value == null))
				return true;
			if ((fieldValue == null)||(value == null))
				return false;
			return value.Equals(fieldValue);
		}

		#endregion
	}

	#endregion

	#region LetterNumberEntityField Class

	/// <summary>
	/// LetterNumberEntityField inherits EntityField to implement
	/// different behavior needed for fields with int
	/// values and letter representation.
	/// </summary>
	public class LetterNumberEntityField : EntityField
	{
		#region Value Range Properties

		private int _minValue;
		public int MinValue
		{
			get { return _minValue; }
		}

		private int _maxValue;
		public int MaxValue
		{
			get { return _maxValue; }
		}

		#endregion

		#region Constructors

		public LetterNumberEntityField(EntityType type, int index,
			int min, int max)
			: base(type, index)
		{
			_minValue = min;
			_maxValue = max;
		}

		// The LetterNumberEntityField costructor
		public LetterNumberEntityField(EntityType type, int index)
			: this(type, index, 0, -1)
		{
		}

		#endregion

		#region Value Operations

		public override string GetText(Entity e)
		{
			if (e.Fields[_index] == null)
				return null;
			return Sport.Common.LetterNumber.ToString((int) e.Fields[_index]);
		}


		// Overrides GetObjectValue to receive int value
		protected override object GetObjectValue(object value)
		{
			if (value == null)
				return null;
			else if (value is int)
			{
				int n = (int) value;
				// If min and max values are defined...
				if (_minValue <= _maxValue)
				{
					// ... check that value is in range
					/*
					if (n < _minValue)
						n = _minValue;
					if (n > _maxValue)
						n = _maxValue;
					*/
					if (n < _minValue ||
						n > _maxValue)
						throw new EntityException("Ivalid argument, int value out of field specified range");
				}
				return n;
			}
			else if (value is string)
			{
				return Sport.Common.LetterNumber.Parse(value.ToString());
			}
			else
				throw new EntityException("Invalid argument, expecting int value");
		}

		public override object ConvertValue(object value)
		{
			return System.Convert.ToInt32(value);
		}


		#endregion

		#region Value Comparison

		// Overrides Compare to preform int comparison
		public override int Compare(Entity x, Entity y)
		{
			int ix = (x.Fields[_index] == null) ? Int32.MinValue : (int) x.Fields[_index];
			int iy = (y.Fields[_index] == null) ? Int32.MinValue : (int) y.Fields[_index];

			if (ix < iy)
				return -1;
			else if (iy < ix)
				return 1;

			return 0;
		}

		// Overrides Equals to preform int comparison
		public override bool Equals(Entity e, object value)
		{
			object fieldValue=e.Fields[_index];
			if ((fieldValue == null)&&(value == null))
				return true;
			if ((fieldValue == null)||(value == null))
				return false;
			return (int) value == (int) fieldValue;
		}

		#endregion
	}

	#endregion

	#region DateTimeEntityField Class

	/// <summary>
	/// DateTimeEntityField inherits EntityField to implement
	/// different behavior needed for fields with DateTime
	/// values.
	/// </summary>
	public class DateTimeEntityField : EntityField
	{
		#region Properties

		// Stores the display format for the DateTime
		// value of the field
		private string _format;
		public string Format
		{
			get { return _format; }
		}

		#endregion

		#region Constructor

		// The DateTimeEntityField costructor, receives the display
		// format for this field
		public DateTimeEntityField(EntityType type, int index, string format)
			: base(type, index)
		{
			_format = format;
		}

		#endregion

		#region Value Operations

		// Overrides GetText to return the value of the
		// field's DateTime formatted by _format
		public override string GetText(Entity e)
		{
			if (e.Fields[_index] == null)
				return null;
			DateTime dt = (DateTime) e.Fields[_index];
			if (dt.Ticks == 0)
				return null;
			return dt.ToString(_format);
		}

		// Overrides GetObjectValue to receive DateTime value
		protected override object GetObjectValue(object value)
		{
			if (value == null)
				return null;
			else if (value is DateTime)
			{
				if (((DateTime) value).Ticks == 0)
					return null;
				return value;
			}
			else
				throw new EntityException("Invalid argument, expecting DateTime value");
		}

		public override object ConvertValue(object value)
		{
			return System.Convert.ToDateTime(value);
		}


		#endregion

		#region Value Comparison

		// Overrides Compare to preform a DateTime comparison
		public override int Compare(Entity x, Entity y)
		{
			DateTime dtx = (x.Fields[_index] == null) ? System.DateTime.MinValue : (System.DateTime) x.Fields[_index];
			DateTime dty = (y.Fields[_index] == null) ? System.DateTime.MinValue : (System.DateTime) y.Fields[_index];
			return dtx.CompareTo(dty);
		}

		// Overrides Equals to preform a DateTime comparison
		public override bool Equals(Entity e, object value)
		{
			if (e.Fields[_index] == null)
				return value == null;
			if (value == null)
				return false;
			return (DateTime) value == (DateTime) e.Fields[_index];
		}

		#endregion
	}

	#endregion

	#region LookupEntityField Class

	/// <summary>
	/// LookupEntityField inherits EntityField to implement
	/// different behavior needed for fields that
	/// stores id values of specified lookup tables.
	/// </summary>
	public class LookupEntityField : EntityField
	{
		#region Properties

		// Stores the lookup table by which the text
		// of the field will be determined
		private LookupType _lookupType;
		public LookupType LookupType
		{
			get
			{
				return _lookupType;
			}
		}

		#endregion

		#region Constructor

		// The LookupEntityField constructor, receives the lookup table
		// for the field values
		public LookupEntityField(EntityType type, int index, LookupType lookupType)
			: base(type, index)
		{
			_lookupType = lookupType;
		}

		#endregion

		#region Properties

		// Overrides GetText to return the text in the lookup table
		// for the field's id value
		public override string GetText(Entity e)
		{
			object value = GetValue(e);
			if (value == null)
				return null;
			int iValue=0;
			try
			{
				iValue = (int) value;
			}
			catch
			{
				System.Diagnostics.Debug.WriteLine("invalid value. entity: "+e.EntityType.Name);
				System.Diagnostics.Debug.WriteLine("value: "+value.ToString()+", type: "+value.GetType().Name);
				throw new Exception("invalid value for entity " + e.EntityType.Name + " (" + value.ToString() + "). expecting integer value for lookup field.");
			}
			if (iValue < 0)
				iValue = 0;
			return LookupType.Lookup(iValue);
		}

		#endregion

		#region Value Operations

		// Overrides GetObjectValue to receive either an int value
		// or a LookupItem
		protected override object GetObjectValue(object value)
		{
			if (value == null)
				return null;
			else if (value is int)
				return value;
			else if (value is LookupItem)
				return ((LookupItem)value).Id;
			else
				throw new EntityException("Invalid argument, expecting LookupItem or int");
		}

		// Overrides GetValueItem to return a LookupItem
		public override object GetValueItem(Entity e)
		{
			object value = GetValue(e);
			int id = -1;
			if (value != null)
				id = (int)value;
			return (id < 0) ? null : _lookupType[id];
		}

		#endregion

		#region Value Comparison

		// Overrides Equals to compare the int value of the
		// given entity's field and the given int value
		public override bool Equals(Entity e, object value)
		{
			object fieldValue = GetValue(e);
			if ((fieldValue == null)&&(value == null))
				return true;
			if ((fieldValue == null)||(value == null))
				return false;
			
			//verify we have int value: *doesn't work for enumerations*
			/*
			if (!((value is Int32)||(value is LookupType)))
			{
				throw new ArgumentException("LookupField: Equals expected "+
					"int or LookupType but got "+value.GetType().Name);
			}
			*/
			
			return ((int) fieldValue == (int) (value));
		}

		#endregion

	}

	#endregion

	#region EntityEntityField Class

	/// <summary>
	/// EntityEntityField inherits EntityField to implement
	/// different behavior needed for fields that
	/// stores ids of entities of a specific type.
	/// </summary>
	public class EntityEntityField : EntityField
	{
		#region Properties

		// Stores the name of the entity type
		// The EntityType might not yet be constructed
		// when the EntityEntityField is constructed
		// so intially storing only the entity type name
		// and retrieving the entity type only when needed
		private string		_entityName;

		// Stores the type of the entity of which
		// the stored id is
		private EntityType	_entityType;
		public EntityType EntityType
		{
			get
			{
				// If there is no entity type yet, getting
				// the type from the entity manager
				if (_entityType == null)
					_entityType = EntityType.GetEntityType(_entityName);
				return _entityType;
			}
		}

		#endregion

		#region Constructor

		// The EntityEntityField constructor receives the name
		// of the entity type
		public EntityEntityField(EntityType type, int index, string entityName)
			: base(type, index)
		{
			_entityName = entityName;
			_entityType = null;
		}

		#endregion

		#region Value Operations

		// Overrides GetText to return the name of the entity of
		// which the id is stored in the field
		System.Collections.ArrayList arrBadEntities = new System.Collections.ArrayList();
		public override string GetText(Entity e)
		{
			if (e.Fields[_index] == null)
				return null;
			if (arrBadEntities.IndexOf(e) >= 0)
				return null;
			Entity entity=null;
			try
			{
				entity = EntityType.Lookup((int)e.Fields[_index]);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("Get Text ("+this._type.Name+"): failed to read enitity: "+ex.Message);
				System.Diagnostics.Debug.WriteLine(ex.StackTrace);
				entity = null;
				if (arrBadEntities.IndexOf(e) < 0)
					arrBadEntities.Add(e);
			}
			if (entity == null)
				return null;
			return entity.Name;
		}

		// Overrides GetObjectValue to receive either an int value 
		// (the entity's id) or an Entity value
		protected override object GetObjectValue(object value)
		{
			if (value == null)
				return null;
            else if (value is int)
				return value;
			else if (value is Entity)
				return ((Entity)value).Id;
			else if (value is EntityBase)
				return ((EntityBase) value).Id;
			else
				throw new EntityException("Invalid argument, expecting Entity or int");
		}

		// Overrides GetValueItem to return an Entity
		public override object GetValueItem(Entity e)
		{
			object value = GetValue(e);
			bool isNull = (value == null);
			if (!isNull && value is int && ((int)value == -1))
				isNull = true;
			return (isNull) ? null : EntityType.Lookup((int) value);
		}

		#endregion

		#region Value Comparison
        
		// Overrides Equals to compare the int value of the
		// given entity's field and the given int value
		public override bool Equals(Entity e, object value)
		{
			if (value == null)
				return e.Fields[_index] == null;
			object o=null;
			if ((e != null)&&(e.Fields != null)&&(_index < e.Fields.Length))
				o = e.Fields[_index];
			if (o == null)
				return false;
			return value.Equals(o);
		}

		#endregion

	}

	#endregion

	#region EntityRelationEntityField Class

	/// <summary>
	/// EntityRelationEntityField inherits EntityField to implement
	/// relative entity's field lookup behavior.
	/// </summary>
	public class EntityRelationEntityField : EntityField
	{
		#region Properties

		// The relative entity field to which the field is belong
		private int _localIndex;
		private int _relativeIndex;
		private EntityField _relativeField;
		public EntityField RelativeField
		{
			get 
			{ 
				// If there is no entity field yet, getting
				// the field from the relative type
				if (_relativeField == null)
					_relativeField = RelativeType.Fields[_relativeIndex];
				return _relativeField; 
			}
		}

		// The relative entity type to which the field is belong
		private string _entityName;
		private EntityType	_relativeType;
		public EntityType RelativeType
		{
			get 
			{ 
				// If there is no entity type yet, getting
				// the type from the entity manager
				if (_relativeType == null)
					_relativeType = EntityType.GetEntityType(_entityName);
				return _relativeType; 
			}
		}

		// A relation field cannot be editable and should
		// not exist
		public override bool CanEdit
		{
			get { return false; }
		}

		public override bool MustExist
		{
			get { return false; }
		}

		#endregion

		#region Constructor

		// EntityRelationEntityField constructor, receives the name
		// of the relative entity type and the field index of
		// the relative field
		public EntityRelationEntityField(EntityType type, int index,
			int localIndex, string entityName, int relativeField)
			: base(type, index)
		{
			_index = index;
			_entityName = entityName;
			_relativeIndex = relativeField;
			_localIndex = localIndex;
		}

		#endregion

		#region Value Operations

		private Entity GetRelativeEntity(Entity e)
		{
			if (e.Fields[_localIndex] == null)
				return null;

			int id = (int)e.Fields[_localIndex];
			if (id < 0)
				return null;

			return RelativeType.Lookup(id);
		}

		// Overrides GetText to return the GetText value
		// of the relative field
		public override string GetText(Entity e)
		{
			Entity r = GetRelativeEntity(e);

			if (r == null)
				return null;

			return RelativeField.GetText(r);
		}

		// Overrides SetValue to disable change to the relative
		// field
		public override void SetValue(EntityEdit e, object value)
		{
			throw new EntityException("Cannot edit a relative field");
		}

		// Overrides GetValue to return the value of the
		// relative field
		public override object GetValue(Entity e)
		{
			Entity r = GetRelativeEntity(e);

			if (r == null)
				return null;

			return RelativeField.GetValue(r);
		}

		// Overrides GetValueItem to return the item of the
		// relative field
		public override object GetValueItem(Entity e)
		{
			Entity r = GetRelativeEntity(e);

			if (r == null)
				return null;

			return RelativeField.GetValueItem(r);
		}


		#endregion

		#region Value Comparison

		// Overrides IsEmpty to return whether the relative field
		// is empty
		public override bool IsEmpty(Entity e)
		{
			Entity r = GetRelativeEntity(e);
			return RelativeField.IsEmpty(r);
		}

		// Override Compare to compare between the relative
		// fields of two entities.
		public override int Compare(Entity x, Entity y)
		{
			Entity rx = GetRelativeEntity(x);
			Entity ry = GetRelativeEntity(y);

			return RelativeField.Compare(rx, ry);
		}

		// Overrides Equals to check if the value of the 
		// relative field is equal to the given value
		public override bool Equals(Entity e, object value)
		{
			Entity r = GetRelativeEntity(e);

			return RelativeField.Equals(r, value);
		}

		#endregion
	}

	#endregion

	#region FormatEntityField Class

	/// <summary>
	/// FormatEntityField inherits EntityField to implement
	/// a field as a format combination of other fields
	/// </summary>
	public class FormatEntityField : EntityField
	{
		#region Properties

		// A format field cannot be editable and should
		// not exist
		public override bool CanEdit
		{
			get { return false; }
		}

		public override bool MustExist
		{
			get { return false; }
		}

		private string _formatString;
		public string FormatString
		{
			get { return _formatString; }
		}

		private int[] _fields;
		public int[] Fields
		{
			get { return _fields; }
		}

		#endregion

		#region Constructor

		// FormatEntityField constructor, receives the format
		// string and the fields in the format
		public FormatEntityField(EntityType type, 
			string formatString, int[] fields, int index)
			: base(type, index)
		{
			_formatString = formatString;
			_fields = fields;
		}

		// An index-less constructor for fields not
		// associated to a specific type field
		public FormatEntityField(EntityType type,
			string formatString, int[] fields)
			: this(type, formatString, fields, -1)
		{
		}

		#endregion

		#region Value Operations

		// Overrides GetText to return the GetText value
		// of the relative field
		public override string GetText(Entity e)
		{
			string[] s = new string[_fields.Length];
			for (int f = 0; f < _fields.Length; f++)
			{
				s[f] = _type.Fields[_fields[f]].GetText(e);
				if ((s[f] == null)||(s[f].Length == 0))
					continue;
				//s[f] = s[f].Replace("(", "---tmpstring---");
				//s[f] = s[f].Replace(")", "(");
				//s[f] = s[f].Replace("---tmpstring---", ")");
				for (int i=0; i<f; i++)
				{
					string strCurrent=_type.Fields[_fields[i]].GetText(e);
					if ((strCurrent != null)&&(strCurrent.Length > 0))
					{
						if (strCurrent.IndexOf(s[f]) >= 0)
						{
							s[f] = "";
							break;
						}
					}
				}
			}
			return String.Format(_formatString, s);
		}

		// Overrides SetValue to disable change to the format
		// field
		public override void SetValue(EntityEdit e, object value)
		{
			throw new EntityException("Cannot edit a format field");
		}

		// Overrides GetValue to return the text
		public override object GetValue(Entity e)
		{
			return GetText(e);
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

	#endregion
}
