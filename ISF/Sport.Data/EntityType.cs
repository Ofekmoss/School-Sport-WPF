using System;
using System.Linq;
using System.Collections;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Sport.Core;
using System.IO;

namespace Sport.Data
{
	/// <summary>
	/// EntityEventArgs adds an entity to the event arguments.
	/// </summary>
	public class EntityEventArgs : System.EventArgs
	{
		private Entity _entity;

		public Entity Entity { get { return _entity; } }

		public EntityEventArgs(Entity entity)
		{
			_entity = entity;
		}
	}

	// Delegate that receives EntityEventArgs
	public delegate void EntityEventHandler(object sender, EntityEventArgs e);

	public abstract class EntityId
	{
		public abstract int GetId(Entity entity);
		public abstract void SetId(EntityEdit entityEdit, int id);
	}

	public class FieldEntityId : EntityId
	{
		private int _field;

		public FieldEntityId(int field)
		{
			_field = field;
		}

		public override int GetId(Entity entity)
		{
			if (entity.Fields[_field] == null)
				return -1;

			return (int)entity.Fields[_field];
		}

		public override void SetId(EntityEdit entityEdit, int id)
		{
			entityEdit.Fields[_field] = id;
		}
	}

	/// <summary>
	/// EntityType stores information about a type of entity and
	/// enables retrieval and update of entities. EntityType is inherited
	/// to implement the different operations needed by specific
	/// types.
	/// </summary>
	public class EntityType
	{
		// Static

		private static Hashtable entityTypes;
		private static string entitiesLogFilePath = "";

		static EntityType()
		{
			entityTypes = new Hashtable();
			entitiesLogFilePath = System.Configuration.ConfigurationManager.AppSettings["GetEntitiesLogFilePath"] + "";
			if (entitiesLogFilePath.Length > 0)
			{
				try
				{
					File.WriteAllText(entitiesLogFilePath, "");
				}
				catch
				{
				}
			}
		}

		public static EntityType GetEntityType(string typeName)
		{
			EntityType entityType = (EntityType)entityTypes[typeName];

			if (entityType == null)
			{
				string entityTypeClass = Sport.Core.Configuration.ReadString("EntityTypes", typeName);

				if (entityTypeClass == null && Sport.Core.Session.Connected)
				{
					Sport.Data.SportServices.DataService ds = new Sport.Data.SportServices.DataService();
					ds.CookieContainer = Sport.Core.Session.Cookies;
					//ds.SetClientVersion(Sport.Core.Data.CurrentVersion);

					entityTypeClass = ds.GetEntityTypeClass(typeName);

					Sport.Core.Configuration.WriteString("EntityTypes", typeName, entityTypeClass);
				}

				if (entityTypeClass != null)
				{
					// Forcing static constructor
					Type type = Type.GetType(entityTypeClass);

					if (type != null)
					{
						System.Reflection.FieldInfo fi = type.GetField("Type");
						fi.GetValue(null);

						entityType = (EntityType)entityTypes[typeName];
					}
				}
			}


			return entityType;
		}

		#region Properties

		// Stores the fields defined for the entity type
		private EntityField[] fields;
		public EntityField[] Fields
		{
			get { return fields; }
		}

		// Gets and sets the fields of the EntityType 
		public EntityField this[int index]
		{
			get
			{
				return fields[index];
			}
			set
			{
				fields[index] = value;
			}
		}

		// Stores the amount of data fields for the entity type
		private int _dataFields;
		public int DataFields
		{
			get { return _dataFields; }
		}

		// Stores an EntityId class that retrieve and set the id of an entity
		protected EntityId _entityID;
		public EntityId EntityId
		{
			get { return _entityID; }
		}

		// Stores a field that retrieve the display name of an entity
		protected EntityField _nameField;
		public EntityField NameField
		{
			get { return _nameField; }
			set { _nameField = value; }
		}

		// Stores a field that stores the entities owner id
		protected EntityField _ownerField;
		public EntityField OwnerField
		{
			get { return _ownerField; }
			set { _ownerField = value; }
		}

		// Stores a field that stores the Entity last modified date and time.
		protected EntityField _lastModifiedField = null;
		public EntityField DateLastModified
		{
			get { return _lastModifiedField; }
			set { _lastModifiedField = value; }
		}

		// Stores the name of the type
		private string _name;
		public string Name
		{
			get { return _name; }
		}

		// Stores the path of the entity type cache
		private string _cachePath;
		public string CachePath
		{
			get
			{
				return _cachePath;
			}
		}

		// Stores the interval (in minutes) to remove entities from memory
		private int _refreshRate = 0; // default is 0 minutes
		public int RefreshRate
		{
			get { return _refreshRate; }
			set { _refreshRate = value; }
		}

		#endregion

		// Stores a hashtable of the entities read in order to look up
		// entities by ids
		private Hashtable entityLookup;
		// Stores a hashtable of arrays of entities read, keyed by EntityFilter
		private Hashtable entityArrays;

		// The EntityType constructor, receives the type name and field count
		// The constructor is protected and only called by an inherited
		// type.
		public EntityType(string name, int fieldCount, int dataFields, EntityId entityId,
			string cachePath)
		{
			Sport.Core.Session.SeasonParameter.ValueChanged += new EventHandler(SeasonParameter_ValueChanged);

			_name = name;
			_cachePath = cachePath;

			entityTypes[name] = this;

			_dataFields = dataFields;

			_entityID = entityId;

			// Default type construction, creating the fields
			// array according to the given count and creating
			// all fields as the default field type
			fields = new EntityField[fieldCount];

			for (int n = 0; n < dataFields; n++)
				fields[n] = new EntityField(this, n);

			entityLookup = new Hashtable();

			DeserializeArrays();
		}

		public EntityType(string name, int fieldCount, int dataFields, EntityId entityId)
			: this(name, fieldCount, dataFields, entityId, "Cache")
		{
		}

		// LookupEntityFilter receives from the data service
		// the filter needed to read the given entity
		private EntityFilter LookupEntityFilter(int id)
		{
			if (!Sport.Core.Session.Connected)
				throw new EntityException("Cannot lookup entity filter on disconnected session");

			Sport.Data.SportServices.DataService ds = new Sport.Data.SportServices.DataService();
			ds.CookieContainer = Sport.Core.Session.Cookies;
			//ds.SetClientVersion(Sport.Core.Data.CurrentVersion);
			Sport.Data.SportServices.FilterField[] fields =
				ds.LookupEntityFilter(_name, id);

			if (fields == null)
			{
				System.Diagnostics.Debug.WriteLine("no filter fields for entity " +
					"of type " + this._name + " with ID " + id);
				return null;
			}

			return new EntityFilter(fields);
		}

		// Returns the entity with the given id
		public virtual Entity Lookup(int id)
		{
			//valid?
			if (id < 0)
				throw new EntityException("failed to lookup entity of type " + _name + ": negative ID.");

			// Lookup the entity in the hashtable
			Entity e = (Entity)entityLookup[id];

			if (e == null && Sport.Core.Session.Connected)
			{
				// If not found, finding needed filter and
				// reading it
				GetEntities(LookupEntityFilter(id));

				e = (Entity)entityLookup[id];

				if (e == null)
				{
					throw new EntityException(
						String.Format("Failed to load requested entity (type: {0}, id {1})", new object[] { _name, id }));
				}
			}

			return e;
		}

		// Reads entities from server according to given filter
		private static Hashtable tblCache = new Hashtable();
		private static Hashtable tblTimes = new Hashtable();
		private static Hashtable tblStamp = new Hashtable();
		private static Hashtable tblDeleted = new Hashtable();

		public static void ClearCacheEntities()
		{
			tblCache.Clear();
			tblTimes.Clear();
			tblStamp.Clear();
			tblDeleted.Clear();
		}

		/// <summary>
		/// return the keys and amount of items associated with each key.
		/// ArrayList item will be string array with two items, first it
		/// the key name and second amount of associated items.
		/// </summary>
		/// <returns></returns>
		public static ArrayList GetCacheData()
		{
			ArrayList arrData = new ArrayList();
			foreach (string key in tblCache.Keys)
			{
				int count = 0;
				if (tblCache[key] != null)
				{
					count = (tblCache[key] as Entity[]).Length;
				}
				arrData.Add(new string[] { key, count.ToString() });
			}
			return arrData;
		}

		public void ClearCache(EntityFilter filter)
		{
			string key = _name + "|" + filter.ToString();
			tblCache.Remove(key);
			tblTimes.Remove(key);
			tblStamp.Remove(key);
			tblDeleted.Remove(key);
		}

		public void ClearAllCache()
		{
			ArrayList arrToRemove = new ArrayList();
			string strToSearch = _name + "|";
			foreach (string key in tblCache.Keys)
			{
				if (key.StartsWith(strToSearch))
				{
					arrToRemove.Add(key);
				}
			}
			if (arrToRemove.Count > 0)
			{
				foreach (string sCurKey in arrToRemove)
				{
					tblCache.Remove(sCurKey);
					tblTimes.Remove(sCurKey);
					tblStamp.Remove(sCurKey);
					tblDeleted.Remove(sCurKey);
				}
			}
		}

		private Entity[] ReadEntities(EntityFilter filter, ref decimal timestamp, out int[] deleted)
		{
			if (!Sport.Core.Session.Connected)
				throw new EntityException("Cannot read entities on disconnected session");

			string key = _name + "|" + filter.ToString();

			bool blnReadFromServer = ((!Utils.IsRunningLocal()) || (Utils.IsRunningLocal() && tblCache[key] == null));
			if (!blnReadFromServer)
			{
				TimeSpan ts = DateTime.Now - ((DateTime)tblTimes[key]);
				if (ts.TotalSeconds >= 180)
					blnReadFromServer = true;
			}

			string strLogPath = null;
			if (System.Web.HttpContext.Current != null)
				strLogPath = System.Web.HttpContext.Current.Server.MapPath("log.txt");


			if (blnReadFromServer)
			{
				string strDebugMessage = String.Format("Reading entities from server (Type: {0}, Filter: ({1}), Timestamp {2})...",
					_name, filter, timestamp);


				//Sport.Common.Tools.WriteToLog(strDebugMessage, strLogPath);

				System.Diagnostics.Debug.WriteLine(strDebugMessage);

				DateTime time1 = DateTime.Now;

				Sport.Data.SportServices.DataService ds = new Sport.Data.SportServices.DataService();
				ds.CookieContainer = Sport.Core.Session.Cookies;
				//bool blnSuccess = false;
				int triesCount = 0;
				/*
				while (triesCount < 5)
				{
					if (blnSuccess)
						break;
					try
					{
						ds.SetClientVersion(Sport.Core.Data.CurrentVersion);
						blnSuccess = true;
					}
					catch
					{
						blnSuccess = false;
						System.Threading.Thread.Sleep(1000);
					}
					triesCount++;
				}
				*/

				if (Sport.Core.Session.IsLogActive)
					Sport.Common.Tools.WriteToLog(String.Format("DataService: SetClientVersion called with {0}. Tries count: {1}", Sport.Core.Data.CurrentVersion, triesCount));

				string entName = Name;
				Sport.Data.SportServices.FilterField[] arrFields = filter.ToFilterFields(this);

				Sport.Data.SportServices.Entity[] sourceEntities = null;
				bool blnKeepTrying = true;
				Sport.Core.Data.TemporaryAuthorization = true;
				triesCount = 0;
				deleted = null;
				while (blnKeepTrying)
				{
					if (sourceEntities != null)
						break;
					if (triesCount > 5)
						break;
					try
					{
						// The data service will read only entities with timestamp
						// larger than the given and will return the maximum timestamp
						// read
						sourceEntities = ds.GetEntities(entName, arrFields,
							ref timestamp, out deleted);
					}
					catch (Exception ex)
					{
						sourceEntities = null;
						if (ex.Message.ToLower().IndexOf("the underlying connection was closed") >= 0)
						{
							blnKeepTrying = true;
							triesCount++;
							System.Diagnostics.Debug.WriteLine("connection temporary " +
								"closed, sleeping before trying again. (retry #" + triesCount + ")");
							System.Threading.Thread.Sleep(500);
						}
						else
						{
							blnKeepTrying = false;
							throw ex;
						}
					}
				}

				if (Sport.Core.Session.IsLogActive)
				{
					string strLogText = "DataService: GetEntities called. Entity name: " + entName + ". ";
					strLogText += "Filter fields: ";
					if (arrFields != null)
					{
						strLogText += arrFields.Length.ToString() + " (";
						for (int i = 0; i < arrFields.Length; i++)
						{
							Sport.Data.SportServices.FilterField filterField = arrFields[i];
							strLogText += "{" + filterField.Field + ", " + filterField.Value + ", " + filterField.Not + "}";
							if (i < (arrFields.Length - 1))
							{
								strLogText += ", ";
							}
						}
						strLogText += ")";
					}
					else
					{
						strLogText += "N/A";
					}
					strLogText += ". Tries count: " + triesCount + ". ";
					strLogText += "Result entities: " + ((sourceEntities != null) ? sourceEntities.Length.ToString() : "N/A");
					Sport.Common.Tools.WriteToLog(strLogText);
				}

				DateTime time2 = DateTime.Now;

				System.Diagnostics.Debug.WriteLine("read took: " +
					Sport.Common.Tools.DateDiffMiliSeconds(time1, time2) + " miliseconds.");

				System.Diagnostics.Debug.WriteLine(
					String.Format("Reading result: Entities: {0},  Deleted: {1}, Timestamp {2}.",
					(sourceEntities == null) ? 0 : sourceEntities.Length, (deleted == null) ? 0 : deleted.Length, timestamp));

				int sourceLengh = (sourceEntities == null) ? 0 : sourceEntities.Length;
				Entity[] entities = new Entity[sourceLengh];
				for (int n = 0; n < entities.Length; n++)
				{
					entities[n] = new Entity(this, sourceEntities[n].Fields);
				}

				if (Utils.IsRunningLocal())
				{
					tblCache[key] = entities;
					tblTimes[key] = DateTime.Now;
					tblStamp[key] = timestamp;
					tblDeleted[key] = deleted;
				}
				else
				{
					return entities;
				}
			}
			else
			{
				System.Diagnostics.Debug.WriteLine(
					String.Format("Taking entities from the cache (Type: {0}, Filter: ({1}), Timestamp {2})",
					_name, filter, timestamp));
			}

			deleted = (int[])tblDeleted[key];
			timestamp = (decimal)tblStamp[key];
			return (Entity[])tblCache[key];
		}

		// Returns the EntityArray that equals to the given filter
		// or that contains the given filter
		private EntityArray FindEntityArray(EntityFilter filter)
		{
			// Lookup for exact array
			EntityArray array = (EntityArray)entityArrays[filter];

			if (array != null)
				return array;

			// Looking for contaning array
			IDictionaryEnumerator e = entityArrays.GetEnumerator();
			while (e.MoveNext())
			{
				if (filter.Compare((EntityFilter)e.Key) ==
					EntityFilter.Differences.Containing)
					return (EntityArray)e.Value;
			}

			return null;
		}

		// Finds the EntityArray that fit for the given filter
		// and if not exist, creates a new array
		public EntityArray GetEntityArray(EntityFilter filter)
		{
			EntityFilter arrayFilter = filter == null ? EntityFilter.Empty :
				filter.ToArrayFilter();

			EntityArray array = FindEntityArray(arrayFilter);

			if (Sport.Core.Session.Connected)
			{
				// Entities not found in memory, creating a new array
				if ((array == null) || (array.Count == 0))
					array = CreateEntityArray(arrayFilter);
				else
					RefreshEntityArray(array);

				if (this.Name == "charge" && array.Entities.Length > 0)
				{
					Entity[] entities = array.Entities;
					ArrayList result = new ArrayList();
					int[] charges = new int[entities.Length];
					for (int i = 0; i < entities.Length; i++)
						charges[i] = entities[i].Id;
					AdvancedServices.AdvancedService service = new AdvancedServices.AdvancedService();
					int[] differentSeason = service.GetDifferentSeasonCharges(charges, Sport.Core.Session.Season);
					if (differentSeason != null && differentSeason.Length > 0)
					{
						System.Diagnostics.Debug.WriteLine("count: " + array.Entities.Length);
						System.Diagnostics.Debug.WriteLine("different season: " + String.Join(", ", Sport.Common.Tools.ToStringArray(differentSeason)));
						ArrayList arrDifferentSeason = new ArrayList(differentSeason);
						foreach (Entity entity in entities)
							if (arrDifferentSeason.IndexOf(entity.Id) < 0)
								result.Add(entity);
						entities = (Entity[])result.ToArray(typeof(Entity));
						array.Entities = entities;
						System.Diagnostics.Debug.WriteLine("count: " + array.Entities.Length);
					}
				}
			}

			//System.Diagnostics.Debug.WriteLine("get entity array: count: " + array.Entities.Length);
			return array;
		}

		// Removes all EntityArrays that equals to the given filter,
		// contains the given filter or contained by the given filter
		private void ResetEntityArrays(EntityFilter filter)
		{
			lock (this)
			{
				// Looking for contaning array
				IDictionaryEnumerator e = entityArrays.GetEnumerator();
				while (e.MoveNext())
				{
					if (filter.Compare((EntityFilter)e.Key) !=
						EntityFilter.Differences.Equal)
						((EntityArray)e.Value).Reset();
				}
			}
		}

		// Removes the given EntityArray from the arrays list and
		// removes all array's entities lookup
		private void RemoveArray(EntityArray array)
		{
			for (int n = 0; n < array.Count; n++)
			{
				entityLookup.Remove(array[n].Id);
			}

			// The array might remove entities that are used by other
			// arrays, so reinserting them
			foreach (EntityArray a in entityArrays.Values)
			{
				if (a != array && array.Filter.Compare(a.Filter) != EntityFilter.Differences.NotEqual)
				{
					foreach (Entity e in a.Entities)
					{
						entityLookup[e.Id] = e;
					}
				}
			}

			array.Dispose();

			entityArrays.Remove(array.Filter);
		}

		// Creates an EntityArray and adds it to the EntityType
		// Remove all EntityArrays contained in the filter
		private EntityArray CreateEntityArray(EntityFilter filter)
		{
			if (!Sport.Core.Session.Connected)
				throw new EntityException("Cannot create entity array on disconnected session");

			lock (this)
			{
				// Removing all contained arrays
				IDictionaryEnumerator e = entityArrays.GetEnumerator();
				ArrayList toBeRemoved = new ArrayList();
				while (e.MoveNext())
				{
					if (filter.Compare((EntityFilter)e.Key) == EntityFilter.Differences.Contained)
						toBeRemoved.Add(e.Value);
				}

				// Removing arrays and entities from lookup
				foreach (EntityArray array in toBeRemoved)
				{
					RemoveArray(array);
				}

				// Reading entities from server
				decimal timestamp = 0; // no timestamp check, read all entities
				int[] deleted;
				Entity[] entities = ReadEntities(filter, ref timestamp, out deleted);

				Entity old;
				// Adding entities to lookup
				for (int n = 0; n < entities.Length; n++)
				{
					Entity en = entities[n];

					old = entityLookup[en.Id] as Entity;

					if (old == null)
					{
						entityLookup[en.Id] = en;
					}
					else
					{
						// The entity might exist from another array
						// Refreshing old entity with new values
						old.Refresh(en);
						// Setting the entity to point to the entity in the lookup
						entities[n] = old;
					}
				}

				EntityArray ea = new EntityArray(this, filter, entities, timestamp);
				entityArrays[filter] = ea;

				SerializeArrays();

				return ea;
			}
		}

		// Reads all new entities from server
		private void RefreshEntityArray(EntityArray array)
		{
			if (!Sport.Core.Session.Connected)
				throw new EntityException("Cannot refresh entity array on disconnected session");

			lock (this)
			{
				if (DateTime.Now - array.ReadTime < new TimeSpan(0, _refreshRate, 0))
					return;

				// Reading entities from server
				decimal timestamp = array.Timestamp; // comparing with array timestamp
				int[] deleted;
				Entity[] entities = ReadEntities(array.Filter, ref timestamp, out deleted);

				array.Refresh(timestamp);

				bool serialize = false;

				if (entities.Length > 0)
				{
					System.Collections.ArrayList adds = new ArrayList();
					Entity old;
					// Updating entities lookup
					for (int n = 0; n < entities.Length; n++)
					{
						Entity entity = entities[n];

						old = entityLookup[entity.Id] as Entity;
						if (old == null)
						{
							entityLookup[entity.Id] = entity;
						}
						else
						{
							old.Refresh(entity);
							entity = old;
							entities[n] = entity;
						}

						if (array.IndexOf(entity) == -1)
							adds.Add(entity);
					}

					if (adds.Count > 0)
					{
						array.Add((Entity[])adds.ToArray(typeof(Entity)));
					}

					serialize = true;
				}

				if (deleted != null)
				{
					foreach (int entityId in deleted)
					{
						array.Remove(entityId);
					}
					serialize = true;
				}

				if (serialize)
					SerializeArrays();
			}
		}

		/// <summary>
		/// GetEntities reads an array of entities from the
		/// server according to the given filter
		/// </summary>
		public virtual Entity[] GetEntities(EntityFilter filter)
		{
			if (filter == null)
				filter = EntityFilter.Empty;

			if (entitiesLogFilePath.Length > 0)
			{
				try
				{
					SportServices.FilterField[] fields = filter.ToFilterFields(this);
					if (fields == null)
						fields = new SportServices.FilterField[] {};
					string lineToAdd = string.Format("[{0}] {1} ({2})", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), this.Name,
										string.Join(", ", fields.ToList().ConvertAll(f => string.Format("{0}={1}", f.Field, f.Value))));
					File.AppendAllLines(entitiesLogFilePath, new string[] { lineToAdd });
				}
				catch
				{
				}
			}

			EntityArray array = GetEntityArray(filter);

			if (array == null)
				return null;

			return array.GetEntities(filter);
		}

		/// <summary>
		/// try to delete the dat file of that entity.
		/// </summary>
		public bool DeleteDatFile()
		{
			if (!Utils.IsRunningLocal())
				return false;

			//try to close:
			try
			{
				serializeStream.Close();
			}
			catch { }

			//try to rewrite:
			try
			{
				serializeStream = new System.IO.FileStream(
					_name + ".dat", System.IO.FileMode.Create);
				return true;
			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.WriteLine("failed to delete data file: " + e.Message);
				System.Diagnostics.Debug.WriteLine(e.StackTrace);
				return false;
			}
		}

		/// <summary>
		/// Reset removes arrays of entities from the memory
		/// according to the given filter, making them be
		/// reread next time get entities is called
		/// </summary>
		public virtual void Reset(EntityFilter filter)
		{
			if (filter == null)
			{
				filter = EntityFilter.Empty;
			}

			ResetEntityArrays(filter);
		}

		public void ClearMemoryCache()
		{
			entityLookup.Clear();
		}

		// StoreEntity stores the given entityEdit in the server
		private EntityResult StoreEntity(EntityEdit entityEdit)
		{
			if (!Sport.Core.Session.Connected)
				throw new EntityException("Cannot store entity on disconnected session");

			try
			{
				//initialize data service:
				Sport.Data.SportServices.DataService ds = new Sport.Data.SportServices.DataService();
				//ds.SetClientVersion(Sport.Core.Data.CurrentVersion);
				ds.CookieContainer = Sport.Core.Session.Cookies;

				//store current date:
				if (entityEdit.EntityType.DateLastModified != null)
					entityEdit.Fields[entityEdit.EntityType.DateLastModified.Index] = DateTime.Now;

				string entityName = entityEdit.EntityType.Name;
				object[] values = entityEdit.Fields;
				//check if new entity or existing entity:
				if (entityEdit.Entity == null)
				{
					//new entity, insert new to the database:
					if (!ds.InsertEntity(entityName, ref values))
						return EntityResult.Failed;
				}
				else
				{
					//existing entity, update its values:
					if (!ds.UpdateEntity(entityName, values))
						return EntityResult.Failed;
				}

				for (int n = 0; n < values.Length; n++)
					entityEdit.Fields[n] = values[n];
			}
			catch (Exception e)
			{
				return new EntityMessageResult(EntityResultCode.Failed, e.Message);
			}

			return EntityResult.Ok;
		}

		// DeleteEntity removes the given entity from the server
		private bool DeleteEntity(Entity entity)
		{
			if (!Sport.Core.Session.Connected)
				throw new EntityException("Cannot delete entity on disconnected session");

			Sport.Data.SportServices.DataService ds = new Sport.Data.SportServices.DataService();
			ds.CookieContainer = Sport.Core.Session.Cookies;
			//ds.SetClientVersion(Sport.Core.Data.CurrentVersion);
			string strName = this.Name;
			int id = (int)entity.Id;
			if (ds.DeleteEntity(strName, id) > 0)
			{
				return true;
			}

			return false;
		}

		// Returns an EntityList of entities according to the given filter
		public virtual EntityList CreateEntityList(EntityFilter filter)
		{
			if (filter == null)
				filter = EntityFilter.Empty;

			return new EntityList(this, filter);
		}

		// CanEditField returns whether the given field can be edited
		// in the given entity
		public bool CanEditField(int field, Entity entity)
		{
			// First checking field definition
			if (!Fields[field].CanEdit)
				return false;

			// ... then calling delegate
			if (FieldEdit != null)
			{
				foreach (EntityFieldEdit efe in FieldEdit.GetInvocationList())
				{
					// if should not edit returning false...
					if (!efe(field, entity))
						return false;
					//... otherwise checking next delegates
				}
			}

			return true;
		}

		public delegate bool EntityFieldEdit(int field, Entity entity);
		public EntityFieldEdit FieldEdit;

		// OnValueChange is called by the type's fields when their value
		// is changed
		public void OnValueChange(EntityEdit entityEdit, EntityField entityField)
		{
			if (ValueChanged != null)
				ValueChanged(entityEdit, entityField.Index);
		}

		public delegate void EntityValueChange(EntityEdit entityEdit, int field);
		public EntityValueChange ValueChanged;

		// OnNewEntity is called by when a new EntityEdit is
		// created.
		public void OnNewEntity(EntityEdit entityEdit)
		{
			// Default behavior, call delegate
			if (NewEntity != null)
				NewEntity(entityEdit);
		}

		public delegate void EntityNewEntity(EntityEdit entityEdit);
		public EntityNewEntity NewEntity;

		// CheckEntity is called when an Entity is checked before
		// saving.
		public EntityResult CheckEntity(Entity entity)
		{
			// Calling delegates
			if (EntityCheck != null)
			{
				foreach (EntityEntityCheck eec in EntityCheck.GetInvocationList())
				{
					EntityResult er = eec(entity);
					// if entity check failed returing result...
					if (!er.Succeeded)
						return er;
					//... otherwise checking next delegates
				}
			}

			return EntityResult.Ok;
		}

		public delegate EntityResult EntityEntityCheck(Entity entity);
		public EntityEntityCheck EntityCheck;

		// New is called to create an EntityEdit in order to create
		// a new entity
		public EntityEdit New()
		{
			return new EntityEdit(this);
		}

		// Save is called to store an entityEdit changes
		public EntityResult Save(EntityEdit entityEdit)
		{
			if (!Sport.Core.Session.Connected)
				return new EntityResult(EntityResultCode.NotConnected);

			EntityResult result = entityEdit.Check();

			if (result == EntityResultCode.NoChange)
				return result;
			if (result == EntityResultCode.Ok)
			{
				// Calling delegated checks
				result = CheckEntity(entityEdit);

				// Storing entity in server
				if (result.Succeeded)
					result = StoreEntity(entityEdit);

				if (result.Succeeded)
				{
					// If entityEdit is for a new entity...
					if (entityEdit.Entity == null)
					{
						// ... create a new entity in entityEdit ...
						entityEdit.CreateEntity();
						// ... and adding it to the type
						Add(entityEdit.Entity);
					}
					else
					{
						// ... otherwise 
						// reposition in correct arrays
						IDictionaryEnumerator e = entityArrays.GetEnumerator();
						while (e.MoveNext())
						{
							EntityFilter filter = (EntityFilter)e.Key;
							// if was filtered by array
							if (filter.Filtered(entityEdit.Entity))
							{
								// and not any more
								if (!filter.Filtered(entityEdit))
								{
									// add to array
									((EntityArray)e.Value).Add(entityEdit.Entity);
								}
							}
							else // (was not filtered by array)
							{
								// and is filtered now
								if (filter.Filtered(entityEdit))
								{
									// remove from array
									((EntityArray)e.Value).Remove(entityEdit.Entity.Id);
								}
							}
						}

						OnUpdate(entityEdit); // calling before value update to enable comparison with old values

						// ... and copy values to the changed entity.
						for (int n = 0; n < _dataFields; n++)
						{
							entityEdit.Entity.Fields[n] = entityEdit.Fields[n];
						}

					}

					OnStore(entityEdit.Entity);

					SerializeArrays();
				}
			}

			return result;
		}

		// Delete is called to remove an entity
		public delegate string DeleteEventHandler(object sender);
		public event DeleteEventHandler BeforeDelete;
		public bool Delete(Entity entity)
		{
			//can delete?
			string strErrMsg = "";
			if (this.BeforeDelete != null)
			{
				strErrMsg = this.BeforeDelete(entity);
			}
			if (strErrMsg.Length > 0)
			{
				System.Windows.Forms.MessageBox.Show("לא ניתן למחוק:\n" + strErrMsg,
					"מחיקת נתונים", System.Windows.Forms.MessageBoxButtons.OK,
					System.Windows.Forms.MessageBoxIcon.Error,
					System.Windows.Forms.MessageBoxDefaultButton.Button1,
					System.Windows.Forms.MessageBoxOptions.RightAlign |
					System.Windows.Forms.MessageBoxOptions.RtlReading);
				return false;
			}

			// Removing entity from server, if not connected can't delete
			if (!Sport.Core.Session.Connected || !DeleteEntity(entity))
				return false;

			Remove(entity);

			SerializeArrays();

			return true;
		}

		// Event handler for entity add
		public event EntityEventHandler EntityAdded;
		// Adds an entity to the list
		public void Add(Entity e)
		{
			entityLookup[e.Id] = e;

			// Raises an entity added event
			if (EntityAdded != null)
				EntityAdded(this, new EntityEventArgs(e));
		}

		// Event handler for entity remove
		public event EntityEventHandler EntityRemoved;
		// Removes an entity from the list
		public void Remove(Entity e)
		{

			// Raises an entity removed event
			if (EntityRemoved != null)
				EntityRemoved(this, new EntityEventArgs(e));

			entityLookup.Remove(e.Id);
		}

		/// <summary>
		/// EntityUpdated is called when an entity is saved,
		/// before the new values are set into the stored entity.
		/// </summary>
		public event EntityEventHandler EntityUpdated;
		// Calls updated event
		public void OnUpdate(EntityEdit e)
		{
			// Raises an entity update event
			if (EntityUpdated != null)
				EntityUpdated(this, new EntityEventArgs(e));
		}

		/// <summary>
		/// EntityStored is called after an entity was stored.
		/// </summary>
		public event EntityEventHandler EntityStored;
		// Calls stored event
		public void OnStore(Entity e)
		{
			if (EntityStored != null)
				EntityStored(this, new EntityEventArgs(e));
		}

		public static Entity GetEntityFromField(EntityType type, Entity entity, int field)
		{
			if (entity == null)
				return null;

			object id = entity.EntityType.Fields[field].GetValue(entity);
			if (id == null)
				return null;

			return type.Lookup((int)id);
		}

		#region Serialization

		/// <summary>
		/// Serialization stores all EntityType's EntityArrays to a file
		/// named as the entity name (_name) .dat in the "Cache" subdirectory
		/// 
		/// The file is read on entity type creation and on each read
		/// from the server and stored on every change
		/// </summary>

		private System.IO.FileStream serializeStream = null;

		private bool OpenSerializationStream()
		{
			if (!Utils.IsRunningLocal())
				return false;

			if (serializeStream != null)
				return true;

			try
			{
				serializeStream = Utils.GetCacheFile(_cachePath.Replace("@season", Session.Season.ToString()), _name + ".dat");
			}
			catch (Exception exp)
			{
				serializeStream = null;
				System.Diagnostics.Debug.WriteLine("Failed to read entities of type '" + _name + "': " + exp.Message);
				return false;
			}

			return true;
		}

		private void DeserializeArrays()
		{
			if ((Utils.IsRunningLocal()) && (OpenSerializationStream()))
			{
				serializeStream.Seek(0, System.IO.SeekOrigin.End);

				if (serializeStream.Position == 0)
				{
					entityArrays = new Hashtable();
				}
				else
				{
					BinaryFormatter formatter = new BinaryFormatter();
					serializeStream.Position = 0;

					formatter.Context = new StreamingContext(StreamingContextStates.All, this);
					try
					{
						entityArrays = (Hashtable)formatter.Deserialize(serializeStream);
					}
					catch (Exception e)
					{
						System.Diagnostics.Debug.WriteLine("failed to deserialize file for entity " + _name + ": " + e.Message);
						System.Diagnostics.Debug.WriteLine(e.StackTrace);

						try
						{
							serializeStream = Utils.GetCacheFile(_cachePath.Replace("@season", Session.Season.ToString()), _name + ".dat");
						}
						catch (Exception fileException)
						{
							System.Diagnostics.Debug.WriteLine("failed to delete data file: " + fileException.Message);
						}
						entityArrays = new Hashtable();
						return;
					}

					foreach (EntityArray array in entityArrays.Values)
					{
						for (int n = 0; n < array.Entities.Length; n++)
						{
							Entity entity = array.Entities[n];
							Entity o = entityLookup[entity.Id] as Entity;
							if (o == null)
							{
								entityLookup[entity.Id] = entity;
							}
							else
							{
								array.Entities[n] = o;
							}
						}
					}
				}
			}
			else
			{
				entityArrays = new Hashtable();
			}
		}

		private void SerializeArrays()
		{
			if (OpenSerializationStream())
			{
				BinaryFormatter formatter = new BinaryFormatter();
				serializeStream.SetLength(0);
				formatter.Serialize(serializeStream, entityArrays);
			}
		}

		#endregion

		private void SeasonParameter_ValueChanged(object sender, EventArgs e)
		{
			this.serializeStream = null;
			this.entityArrays.Clear();
		}
	}
}
