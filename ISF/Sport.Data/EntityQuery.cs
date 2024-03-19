using System;

namespace Sport.Data
{
	public class EntityQuery
	{
		#region Properties

		private EntityType _entityType;
		public EntityType EntityType
		{
			get { return _entityType; }
		}

		private EntityFilter _entityFilter;
		public EntityFilter EntityFilter
		{
			get { return _entityFilter; }
		}

		private EntityFilter _baseFilter;
		public EntityFilter BaseFilter
		{
			get { return _baseFilter; }
			set 
			{ 
				_baseFilter = value;
				RebuildFilter();
			}
		}

		public bool Prepared
		{
			get { return _entityFilter != null; }
		}

		private ParameterCollection _parameters;
		public ParameterCollection Parameters
		{
			get { return _parameters; }
		}

		public Parameter this[int parameter]
		{
			get { return _parameters[parameter]; }
		}
        
		#endregion

		public EntityQuery(EntityType entityType)
		{
			_entityType = entityType;
			_entityFilter = null;
			_parameters = new ParameterCollection(this);
			_parameters.Changed += new Sport.Common.CollectionEventHandler(ParametersChanged);
			_entityFilter = EntityFilter.Empty;
		}

		#region Parameter

		public class Parameter : Sport.Common.GeneralCollection.CollectionItem
		{
			public static readonly object NoValue = "empty";

			#region Properties

			public EntityQuery EntityQuery
			{
				get { return Owner as EntityQuery; }
			}

			private int _field;
			public int Field
			{
				get { return _field; }
				set 
				{ 
					if (_field != value)
					{
						_field = value;
						OnChange();
					}
				}
			}

			private bool _required;
			public bool Required
			{
				get { return _required; }
				set 
				{ 
					if (_required != value)
					{
						_required = value;
						OnChange();
					}
				}
			}

			public bool Empty
			{
				get { return _value == NoValue; }
				set 
				{
					Value = value ? NoValue : (_value == NoValue ? null : _value);
				}
			}

			private object _value;
			public object Value
			{
				get { return _value; }
				set 
				{
					if (_value != value)
					{
						_value = value;
						OnChange();
					}
				}
			}
            
			#endregion

			public event EventHandler Changed;

			private void OnChange()
			{
				if (EntityQuery != null)
				{
					EntityQuery.RebuildFilter();
				}

				if (Changed != null)
					Changed(this, EventArgs.Empty);
			}

			#region Constructors

			public Parameter(int field, bool required, object value)
			{
				_field = field;
				_required = required;
				_value = value;
			}

			public Parameter(int field, bool required)
				: this(field, required, NoValue)
			{
			}

			public Parameter(int field)
				: this(field, true, NoValue)
			{
			}

			public Parameter(int field, object value)
				: this(field, true, value)
			{
			}

			#endregion
		}

		#endregion

		#region ParameterCollection

		public class ParameterCollection : Sport.Common.GeneralCollection
		{
			public ParameterCollection(EntityQuery entityQuery)
				: base(entityQuery)
			{
			}

			public EntityQuery EntityQuery
			{
				get { return (EntityQuery) base.Owner; }
			}

			public Parameter this[int index]
			{
				get { return (Parameter) GetItem(index); }
				set { SetItem(index, value); }
			}

			public void Remove(Parameter value)
			{
				RemoveItem(value);
			}

			public void Remove(int field)
			{
				int index = IndexOf(field);

				if (index >= 0)
					RemoveAt(index);
			}

			public bool Contains(Parameter value)
			{
				return base.Contains(value);
			}

			public int IndexOf(Parameter value)
			{
				return base.IndexOf(value);
			}

			public int IndexOf(int field)
			{
				for (int n = 0; n < Count; n++)
				{
					if (this[n].Field == field)
						return n;
				}

				return -1;
			}

			#region Add

			public int Add(Parameter value)
			{
				return AddItem(value);
			}

			public int Add(int field, bool required, object value)
			{
				return AddItem(new Parameter(field, required, value));
			}

			public int Add(int field, bool required)
			{
				return AddItem(new Parameter(field, required));
			}

			public int Add(int field, object value)
			{
				return AddItem(new Parameter(field, value));
			}
			
			public int Add(int field)
			{
				return AddItem(new Parameter(field));
			}

			#endregion

			#region Insert
			
			public void Insert(int index, Parameter value)
			{
				InsertItem(index, value);
			}

			public void Insert(int index, int field, bool required, object value)
			{
				InsertItem(index, new Parameter(field, required, value));
			}

			public void Insert(int index, int field, bool required)
			{
				InsertItem(index, new Parameter(field, required));
			}

			public void Insert(int index, int field, object value)
			{
				InsertItem(index, new Parameter(field, value));
			}

			public void Insert(int index, int field)
			{
				InsertItem(index, new Parameter(field));
			}

			#endregion
		}

		#endregion

		private void RebuildFilter()
		{
			EntityFilter entityFilter;

			if (_parameters.Count == 0)
			{
				entityFilter = _baseFilter == null ? EntityFilter.Empty : _baseFilter;
			}
			else
			{
				if (_baseFilter == null)
					entityFilter = new EntityFilter();
				else
					entityFilter = new EntityFilter(_baseFilter);

				for (int p = 0; p < _parameters.Count && entityFilter != null; p++)
				{
					Parameter parameter = _parameters[p];

					if (parameter.Empty)
					{
						if (parameter.Required)
						{
							entityFilter = null;
						}
					}
					else
					{
						EntityField ef = _entityType[parameter.Field];
						EntityFilterField eff = new EntityFilterField(parameter.Field, 
							ef.ConvertValue(parameter.Value));
						entityFilter.Add(eff);
					}
				}
			}

			if (entityFilter != null || _entityFilter != null)
			{
				if ((entityFilter == null && _entityFilter != null) ||
					(entityFilter != null && _entityFilter == null) ||
					(!entityFilter.Equals(_entityFilter)))
				{
					_entityFilter = entityFilter;
					OnChange();
				}
			}
		}

		private void ParametersChanged(object sender, Sport.Common.CollectionEventArgs e)
		{
			RebuildFilter();
		}

		#region Events

		public event EventHandler Changed;

		private void OnChange()
		{
			if (Changed != null)
				Changed(this, EventArgs.Empty);
		}

		#endregion

		public Entity[] GetEntities()
		{
			if (_entityFilter == null)
				return null;

			return _entityType.GetEntities(_entityFilter);
		}

		public EntityList GetEntityList()
		{
			if (_entityFilter == null)
				return null;
			
			return new EntityList(_entityType, _entityFilter);
		}
	}
}
