using System;
using System.Collections;
using Sport.Common;
using System.Data;

namespace Sport.Data
{
	#region EntityList Events

	/// <summary>
	/// EntityListEventArgs adds an entity and its index
	/// in the list to the event arguments.
	/// </summary>
	public class EntityListEventArgs : System.EventArgs
	{
		private int		_index;
		private Entity	_entity;

		public int		Index { get { return _index; } }
		public Entity	Entity { get { return _entity; } }

		public EntityListEventArgs(int index, Entity entity)
		{
			_index = index;
			_entity = entity;;
		}
	}

	// Delegate that receives EntityListEventArgs
	public delegate void EntityListEventHandler(object sender, EntityListEventArgs e);

	public enum EntityChangeOperation
	{
		New,
		Edit,
		Delete,
		Save,
		Cancel
	}

	/// <summary>
	/// EntityChangeEventArgs adds an entity, its index and
	/// the edit operation to the event arguments.
	/// </summary>
	public class EntityChangeEventArgs : System.EventArgs
	{
		private int					_index;
		private Entity				_entity;
		private EntityChangeOperation _operation;

		public int					Index { get { return _index; } }
		public Entity				Entity { get { return _entity; } }
		public EntityChangeOperation	Operation { get { return _operation; } }

		public EntityChangeEventArgs(int index, Entity entity,
			EntityChangeOperation operation)
		{
			_index = index;
			_entity = entity;
			_operation = operation;
		}
	}

	// Delegate that receives EntityChangeEventArgs
	public delegate void EntityChangeEventHandler(object sender, EntityChangeEventArgs e);

	#endregion

	/// <summary>
	/// EntityList implements a sorted list of entities.
	/// </summary>
	public class EntityList : IDisposable
	{
		#region Properties

		// Stores the type of entities stored in the list
		private EntityType	_type;
		public EntityType EntityType
		{
			get { return _type; }
		}

		// Stores the entities
		private SortedArray _entities;

		// Stores the entity array
		private EntityArray _array;

		private EntityArray Array
		{
			get
			{
				//string strLogPath="D:\\Sportsman\\FinalCheck.log";
				//Sport.Core.LogFiles.AppendToLogFile(strLogPath, 
				//	"get Array for "+this._type.Name+" called...");
				if (_array == null)
				{
					//Sport.Core.LogFiles.AppendToLogFile(strLogPath, 
					//	"array is null, reading from type...");
					_array = _type.GetEntityArray(_filter);
					//Sport.Core.LogFiles.AppendToLogFile(strLogPath, 
					//	"count: "+_array.Count);
					_array.Disposed += new EventHandler(ArrayDisposed);
				}

				return _array;
			}
		}

		private SortedArray Entities
		{
			get
			{
				//string strLogPath="D:\\Sportsman\\FinalCheck.log";
				//Sport.Core.LogFiles.AppendToLogFile(strLogPath, "getting EntityList entities...");
				if (_entities == null)
				{
					//Sport.Core.LogFiles.AppendToLogFile(strLogPath, 
					//	"entities are null, creating new list...");
					_entities = new SortedArray(sort);

					// Adding the given entities to the list
					// according to filter
					//Sport.Core.LogFiles.AppendToLogFile(strLogPath, 
					//	"getting entities from array...");
					Entity[] entities = null;
					try
					{
						entities = Array.GetEntities(_filter);
					}
					catch (Exception ex)
					{
						System.Diagnostics.Debug.WriteLine("failed to get entities: "+ex.Message);
						System.Diagnostics.Debug.WriteLine(ex.StackTrace);
						System.Diagnostics.Debug.WriteLine("filter: "+((_filter == null)?"null":_filter.ToString()));
					}
					if (entities != null)
						foreach (Entity e in entities)
							_entities.Add(e);
				}
				
				return _entities;
			}
		}

		public Entity[] GetListEntities()
		{
			Entity[] result = new Entity[Entities.Count];

			for (int n = 0; n < result.Length; n++)
			{
				result[n] = (Entity) Entities[n];
			}

			return result;
		}

		// Stores the filter of the list
		private EntityFilter _filter;

		// Stores the current sort of the entities
		private EntitySort	sort;
		public EntitySortField[] SortFields
		{
			get { return sort.Fields; }
			set { sort.Fields = value; }
		}

		// Returns the amount of entities in the list
		public int Count
		{
			get
			{
				return Entities.Count;
			}
		}

		// Returns the Entity in the given index in the list
		public Entity this[int index]
		{
			get
			{
				if (index == _current)
					return Current;

				if ((index < 0)||(index >= Entities.Count))
					return null;

				return (Entity) Entities[index];
			}
		}

		// Returns the fields of the entity type
		public EntityField[] Fields
		{
			get 
			{
				return _type.Fields;
			}
		}

		#endregion

		// The EntityList constructor, receives a type of entity
		// and an array of entities.
		public EntityList(EntityType type, EntityFilter filter)
		{
			_filter = filter;
			_type = type;
			sort = new EntitySort(this);
			/*_entities = new SortedArray(sort);

			_array = _type.GetEntityArray(filter);
			
			// Adding the given entities to the list
			// according to filter
			Entity[] entities = _array.GetEntities(filter);
			foreach (Entity e in entities)
			{
				_entities.Add(e);
			}*/

			_entities = null;
			_array = null;

			_type.EntityAdded += new EntityEventHandler(OnEntityAdded);
			_type.EntityUpdated += new EntityEventHandler(OnEntityUpdated);
			_type.EntityRemoved += new EntityEventHandler(OnEntityRemoved);
		}

		#region Sorting

		// Resorts the list, called when the sort fields are changed
		public void Sort()
		{
			if (Sorting != null)
				Sorting(this, EventArgs.Empty);

			// Storing current entity
			Entity e = Current;

			Entities.Sort();

			// Resetting to current entity
			Current = e;

			if (Sorted != null)
				Sorted(this, EventArgs.Empty);
		}

		// Change the sort fields, the change cause Sort() to be called
		public void Sort(EntitySortField[] sortFields)
		{
			// Storing current entity
			Entity e = Current;

			sort.Fields = sortFields;

			// Resetting to current entity
			Current = e;
		}

		#endregion

		#region Searching

		private class EntityListSearch : IComparer
		{
			private EntitySortField _sortField;

			public EntityListSearch(EntitySortField sortField)
			{
				_sortField = sortField;
			}

			#region IComparer Members

			private int CompareEntities(Entity x, Entity y)
			{
				int r = _sortField.Field.Compare(x, y);
				if (r != 0)
				{
					return _sortField.Direction == EntitySortDirection.Ascending ? r : -r;
				}

				return 0;
			}

			private int CompareValue(Entity x, object y)
			{
				int r = _sortField.Field.Compare(x, y);
				if (r != 0)
				{
					return _sortField.Direction == EntitySortDirection.Ascending ? r : -r;
				}

				return 0;
			}

			public int Compare(object x, object y)
			{
				if (x is Entity)
				{
					Entity entX = (Entity) x;
					if (y is Entity)
						return CompareEntities(entX, (Entity) y);

					return CompareValue((Entity) x, y);
				}
				else if (y is Entity)
				{
					return - CompareValue((Entity) y, x);
				}
				
				throw new EntityException("One object must be entity");
			}

			#endregion
		}

		public bool Search(EntityField field, object value)
		{
			if (sort.Fields == null || sort.Fields.Length == 0 ||
				sort.Fields[0].Field != field)
			{
				Sort(new EntitySortField[] { new EntitySortField(field, EntitySortDirection.Ascending) });
			}

			bool asc = sort.Fields[0].Direction == EntitySortDirection.Ascending;
			bool result = true;
			int index = Entities.BinarySearch(value, new EntityListSearch(sort.Fields[0]));

			if (index < 0)
			{
				index = ~index;
				result = false;

				if (index == Entities.Count)
					index--;
			}

			if (index > 0)
			{
				string s = value == null ? "" : value.ToString();
				string a = field.GetText(this[index]);
				int am = Sport.Common.Tools.MatchStrings(s, a);
				string b = field.GetText(this[index - 1]);
				int bm = Sport.Common.Tools.MatchStrings(s, b);
				if (asc)
				{
					if (bm >= am)
						index--;
				}
				else if (bm > am)
					index--;
			}

			CurrentIndex = index;

			return result;
		}

		#endregion

		#region Events

		// Called when the current entity is changed
		public event EntityListEventHandler		CurrentChanged;
		// Called when an entity operation occurs (edit, new, delete, save, cancel)
		public event EntityChangeEventHandler	EntityChanged;
		// Called when an entity is added to the list
		public event EntityListEventHandler		EntityAdded;
		// Called when an entity is removed from the list
		public event EntityListEventHandler		EntityRemoved;
		// Called before the list is sorted
		public event EventHandler Sorting;
		// Celled after the list is sorted
		public event EventHandler Sorted;

		#endregion

		#region Entity Editing
		
		private void OnCurrentChange()
		{
			if (CurrentChanged != null)
			{
				CurrentChanged(this, new EntityListEventArgs(_current, Current));
			}
		}

		private void OnEntityChange(int index, Entity entity, EntityChangeOperation operation)
		{
			if (EntityChanged != null)
				EntityChanged(this, new EntityChangeEventArgs(index, entity, operation));
		}

		private int _current = -1;
		public int CurrentIndex
		{
			get { return _current; }
			set
			{
				if (_current != value)
				{
					if (value < -1 || value > Entities.Count)
						throw new ArgumentOutOfRangeException("value", "Entity index out of range");

					if (_entityEdit != null)
						Cancel();

					_current = value;
					OnCurrentChange();
				}			
			}
		}

		public Entity Current
		{
			get 
			{ 
				// On edit return EntityEdit
				if (Editing)
					return _entityEdit;

				if (_current < 0)
					return null;
				
				if (Entities == null || _current >= Entities.Count)
					return null;
				
				return _current == -1 ? null : (Entity) Entities[_current]; 
			}

			set
			{
				if (value == null)
				{
					CurrentIndex = -1;
				}
				else if (value != _entityEdit)
				{
					CurrentIndex = Entities.IndexOf(value);
				}
			}
		}

		private EntityEdit _entityEdit = null;
		public EntityEdit EntityEdit
		{
			get { return _entityEdit; }
		}
		public bool Editing
		{
			get { return _entityEdit != null; }
		}

		/// <summary>
		/// Edits the current entity in the EntityList
		/// </summary>
		public bool Edit()
		{
			// If already editing returning true
			if (Editing)
				return true;

			// An entity needs to be selected
			Entity e = Current;
			if (e == null)
				return false;

			// Creating edit object
			_entityEdit = e.Edit();

			OnEntityChange(_current, _entityEdit, EntityChangeOperation.Edit);

			return true;
		}

		/// <summary>
		/// Edits a new entity in the EntityList
		/// </summary>
		public bool New()
		{
			// If editing, return true only if editing new entity
			if (Editing)
				return _current == Entities.Count;
			
			_entityEdit = _type.New();

			// Setting current to count on new entity
			_current = Entities.Count;

			OnEntityChange(_current, _entityEdit, EntityChangeOperation.New);

			OnCurrentChange();

			return true;
		}

		/// <summary>
		/// Deletes the current entity in the EntityList
		/// </summary>
		/// <returns></returns>
		public bool Delete()
		{
			Entity e = Current;
			int i = CurrentIndex;
			
			if (e == null)
				return false;

			if (Editing)
			{
				Cancel();
				e = Current;
			}
			
			if (e.Delete())
			{
				OnEntityChange(i, e, EntityChangeOperation.Delete);
				e.EntityType.Reset(null);
				EntityType.ClearCacheEntities();
				return true;
			}

			return false;
		}

		/// <summary>
		/// Saves editted entity
		/// </summary>
		public EntityResult Save()
		{
			EntityResult result;
			// Checking if in edit
			if (_entityEdit == null)
			{
				result = new EntityResult(EntityResultCode.NotEditing);
			}
			else
			{
				// Saving edit
				result = _entityEdit.Save();
				if (result.Succeeded)
				{
					// Storing current entity
					Entity e = _entityEdit.Entity;
					// Clearing edit object
					_entityEdit = null;
					// Selecting current entity
					Current = e;
					// Resetting sort
					Sort();

					OnEntityChange(_current, e, EntityChangeOperation.Save);
				}
			}

			return result;
		}

		/// <summary>
		/// Cancels edit
		/// </summary>
		public void Cancel()
		{
			// Checking if in edit
			if (_entityEdit != null)
			{
				// Storing current entity
				Entity e = _entityEdit.Entity;
				// Clearing edit object
				_entityEdit = null;
				// Selecting current entity
				Current = e;

				OnEntityChange(_current, e, EntityChangeOperation.Cancel);
			}
		}

		#endregion

		#region Entity Add/Remove Notification

		private void OnEntityAdded(object sender, EntityEventArgs e)
		{
			if (!_filter.Filtered(e.Entity))
				Add(e.Entity);
		}

		private void OnEntityUpdated(object sender, EntityEventArgs e)
		{
			EntityEdit entityEdit = e.Entity as EntityEdit;

			if (!_filter.Filtered(entityEdit))
			{
				if (_filter.Filtered(entityEdit.Entity))
				{
					Add(entityEdit.Entity); // Adding the Entity and not entityField
					// because this is the pointer of the entity for user
				}
			}
			else
			{
				if (!_filter.Filtered(entityEdit.Entity))
				{
					Remove(entityEdit.Entity);// Remove the Entity and not entityField
					// because this is the pointer of the entity for user
				}
			}
		}

		private void OnEntityRemoved(object sender, EntityEventArgs e)
		{
			if (!_filter.Filtered(e.Entity))
				Remove(e.Entity);
		}

		// Adds an entity to the list
		public void Add(Entity e)
		{
			int n = Entities.Add(e);

			if (n < _current)
				_current++;

			// Raises an entity added event
			if (EntityAdded != null)
				EntityAdded(this, new EntityListEventArgs(n, e));
		}

		// Returns the index of an entity
		public int IndexOf(Entity e)
		{
			return Entities.IndexOf(e);
		}

		// Removes an entity from the list
		public void Remove(Entity e)
		{
			int n = Entities.IndexOf(e);
			if (n >= 0)
			{
				Entities.RemoveAt(n);

				bool currentChanged = false;
				if (n == _current)
				{
					_current = -1;
					currentChanged = true;
				}
				else if (n < _current)
					_current--;

				// Raises an entity removed event
				if (EntityRemoved != null)
					EntityRemoved(this, new EntityListEventArgs(n, e));

				// If current item changed raising current changed event
				if (currentChanged)
					OnCurrentChange();
			}
		}

		#endregion
	
		#region IDisposable Members

		public void Dispose()
		{
			_type.EntityAdded -= new EntityEventHandler(OnEntityAdded);
			_type.EntityUpdated -= new EntityEventHandler(OnEntityUpdated);
			_type.EntityRemoved -= new EntityEventHandler(OnEntityRemoved);

			if (_array != null)
			{
				_array.Disposed -= new EventHandler(ArrayDisposed);
			}
			if (_entities != null)
			{
				_entities.Clear();
				_entities = null;
			}
		}

		#endregion

		private void ArrayDisposed(object sender, EventArgs e)
		{
			_array = null;
			_entities = null;
		}
	}
}
