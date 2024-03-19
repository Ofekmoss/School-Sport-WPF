using System;
using System.Collections;

namespace Sport.Data
{
	/// <summary>
	/// EntityArray stores a set of entities of a specified
	/// filter.
	/// EntityArray is used by the EntityType to store entities.
	/// </summary>
	[Serializable]
	public class EntityArray : IDisposable, System.Runtime.Serialization.ISerializable
	{
		#region Properties

		private EntityType	_type;
		/// <summary>
		/// Gets the type of entities stored in the array
		/// </summary>
		public EntityType EntityType
		{
			get { return _type; }
		}

		// Stores the entities
		private Entity[] _entities;
		public Entity[] Entities
		{
			get { return _entities; }
			set { _entities = value; }
		}

		private EntityFilter _filter;
		/// <summary>
		/// Gets the filter of the array
		/// </summary>
 		public EntityFilter Filter
		{
			get { return _filter; }
			set { _filter = value; }
		}

		private DateTime _readTime;
		/// <summary>
		/// Gets the time the array was created
		/// </summary>
		public DateTime ReadTime
		{
			get { return _readTime; }
		}

		private decimal _timestamp;
		/// <summary>
		/// Gets the maximum row timestamp of entities in the array
		/// </summary>
		public decimal Timestamp
		{
			get { return _timestamp; }
		}
		
		/// <summary>
		/// Gets the amount of entities in the array 
		/// </summary>
		public int Count
		{
			get
			{
				return (_entities == null)?0:_entities.Length;
			}
		}

		/// <summary>
		/// Gets the Entity in the given index in the array
		/// </summary>
		public Entity this[int index]
		{
			get
			{
				if ((index < 0)||(index >= _entities.Length))
					return null;
				return _entities[index];
			}
		}

		#endregion

		#region Constructor

		/// <summary>
		/// EntityArray constructor, stores the entities
		/// and register for event from the type
		/// </summary>
		/// <param name="type"></param>
		/// <param name="filter"></param>
		/// <param name="entities"></param>
		public EntityArray(EntityType type, EntityFilter filter, Entity[] entities, decimal timestamp)
		{
			_filter = filter;
			_type = type;
			_entities = entities;

			// Registering for type events
			_type.EntityAdded += new EntityEventHandler(OnEntityAdded);
			_type.EntityRemoved += new EntityEventHandler(OnEntityRemoved);

			_timestamp = timestamp;
			_readTime = DateTime.Now;
		}

		#endregion

		#region Event Handling

		// Called when a new entity is added
		private void OnEntityAdded(object sender, EntityEventArgs e)
		{
			if (!_filter.Filtered(e.Entity))
				Add(e.Entity);
		}

		// Called when an entity is removed
		private void OnEntityRemoved(object sender, EntityEventArgs e)
		{
			if (!_filter.Filtered(e.Entity))
				Remove(e.Entity);
		}

		// Adding the entity to the array
		internal void Add(Entity e)
		{
			Entity[] entities = new Entity[_entities.Length + 1];
			Array.Copy(_entities, entities, _entities.Length);
			entities[_entities.Length] = e;
			_entities = entities;
		}

		internal void Add(Entity[] es)
		{
			Entity[] entities = new Entity[_entities.Length + es.Length];
			Array.Copy(_entities, entities, _entities.Length);
			Array.Copy(es, 0, entities, _entities.Length, es.Length);
			_entities = entities;
		}

		public int IndexOf(int id)
		{
			for (int n = 0; n < _entities.Length; n++)
			{
				if (_entities[n].Id == id)
					return n;
			}
			
			return -1;
		}

		public int IndexOf(Entity e)
		{
			return IndexOf(e.Id);
		}

		// Removing the entity from the array
		private void Remove(Entity e)
		{
			int index = IndexOf(e);

			if (index != -1)
			{
				Entity[] entities = new Entity[_entities.Length - 1];
				if (index > 0)
					Array.Copy(_entities, entities, index);
				if (index < _entities.Length - 1)
					Array.Copy(_entities, index + 1, entities, index, _entities.Length - index - 1);

				_entities = entities;
			}
		}

		internal void Remove(int entityId)
		{
			int index = -1;
			for (int n = 0; n < _entities.Length && index == -1; n++)
			{
				if (_entities[n].Id == entityId)
					index = n;
			}

			if (index != -1)
			{
				Entity[] entities = new Entity[_entities.Length - 1];
				if (index > 0)
					Array.Copy(_entities, entities, index);
				if (index < _entities.Length - 1)
					Array.Copy(_entities, index + 1, entities, index, _entities.Length - index - 1);

				_entities = entities;
			}
		}

		#endregion

		#region Read Functions

		/// <summary>
		/// Returns a subset of the entities array according to
		/// the given filter
		/// </summary>
		/// <param name="filter"></param>
		/// <returns></returns>
		public Entity[] GetEntities(EntityFilter filter)
		{
			//string strLogPath="D:\\Sportsman\\FinalCheck.log";
			//Sport.Core.LogFiles.AppendToLogFile(strLogPath, 
			//	"getting "+_type.Name+"entities: comparing current filter with given...");
			
			EntityFilter.Differences diff = _filter.Compare(filter);
			
			//Sport.Core.LogFiles.AppendToLogFile(strLogPath, 
			//	"difference: "+diff.ToString());
			
			if (diff == EntityFilter.Differences.Equal)
			{
				//Sport.Core.LogFiles.AppendToLogFile(strLogPath, 
				//	"equal: Cloning...");
				Entity[] result=(Entity[]) _entities.Clone();
				//Sport.Core.LogFiles.AppendToLogFile(strLogPath, 
				//	"amount: "+((result == null)?0:result.Length));
				return result;
			}
			else if (diff == EntityFilter.Differences.Contained)
			{
				ArrayList al = new ArrayList();
				//Sport.Core.LogFiles.AppendToLogFile(strLogPath, 
				//	"contained: checking "+_entities.Length+" entities...");
				foreach (Entity e in _entities)
				{
					//Sport.Core.LogFiles.AppendToLogFile(strLogPath, 
					//	"checking filter for "+this._type.Name+" #"+e.Id+"...");
					bool blnFilter=filter.Filtered(e);
					//Sport.Core.LogFiles.AppendToLogFile(strLogPath, "filter: "+blnFilter);
					if (!blnFilter)
					{
						//Sport.Core.LogFiles.AppendToLogFile(strLogPath, 
						//	"adding entity to the array...");
						al.Add(e);
					}
				}
				//Sport.Core.LogFiles.AppendToLogFile(strLogPath, "returning "+al.Count+" entities...");
				
				return (Entity[]) al.ToArray(typeof(Entity));
			}
			
			throw new ArgumentException("Given filter not exist in array filter");
		}

		public void Reset()
		{
			_readTime = DateTime.MinValue;
		}

		public void Refresh(decimal timestamp)
		{
			_timestamp = timestamp;
			_readTime = DateTime.Now;
		}

		#endregion

		#region IDisposable Members

		public event System.EventHandler Disposed;

		public void Dispose()
		{
			_type.EntityAdded -= new EntityEventHandler(OnEntityAdded);
			_type.EntityRemoved -= new EntityEventHandler(OnEntityRemoved);

			_entities = null;

			if (Disposed != null)
				Disposed(this, EventArgs.Empty);
		}

		#endregion

		#region ISerializable Members

		protected EntityArray(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			_type = context.Context as EntityType;
			_filter = (EntityFilter) info.GetValue("filter", typeof(EntityFilter));
			_timestamp = info.GetDecimal("timestamp");
			_entities = (Entity[]) info.GetValue("entities", typeof(Entity[]));

			// Registering for type events
			_type.EntityAdded += new EntityEventHandler(OnEntityAdded);
			_type.EntityRemoved += new EntityEventHandler(OnEntityRemoved);

			_readTime = DateTime.MinValue;
		}

		public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			info.AddValue("filter", _filter);
			info.AddValue("timestamp", _timestamp);
			info.AddValue("entities", _entities);
		}

		#endregion
	}
}
