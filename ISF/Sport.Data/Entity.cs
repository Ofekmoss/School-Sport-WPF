using System;
using System.Collections;
using System.IO;
using Sport.Core;
using System.Diagnostics;
using System.Web;

namespace Sport.Data
{
	/// <summary>
	/// Entity holds the data of a single entity.
	/// The entity stores the entity type and the
	/// values of its fields according to its type.
	/// </summary>
	[Serializable]
	public class Entity : System.Runtime.Serialization.ISerializable, IComparable
	{
		#region Properties

		// The type of the entity
		protected EntityType _type;

		public EntityType EntityType
		{
			get { return _type; }
		}

		// The data fields of the entity
		protected object[] _fields;

		public object[] Fields
		{
			get { return _fields; }
		}

		// Each entity type stores an EntityField for
		// the entity's Id, calling this property
		// returns the value of the id field of the entity
		public int Id
		{
			get
			{
				if (_type.EntityId == null)
					throw new EntityException("no entity id");

				return _type.EntityId.GetId(this);
			}
		}

		// Each entity type stores an EntityField for
		// the entity's Name, calling this property
		// returns the displayed name of the entity
		// as implemented by its name field
		public string Name
		{
			get
			{
				if (_type == null)
					return null;
				if (_type.NameField == null)
					return null;
				return _type.NameField.GetText(this);
			}
		}

		#endregion

		#region String Representation

		// Overrinding ToString to return the entity's name
		public override string ToString()
		{
			return Name;
		}

		// Gets field text
		public string GetText(int field)
		{
			return _type.Fields[field].GetText(this);
		}

		#endregion

		#region Constructors

		// Entity constructor, receives the type and field
		// values for the entity
		public Entity(EntityType type, object[] fields)
		{
			if (fields.Length < type.DataFields) //type.DataFields != fields.Length
				throw new EntityException("Fields don't match entity type: '" + type.Name + "'");

			_type = type;
			_fields = fields;
		}

		// Entity constructor for inheriting EntityEdit
		protected Entity(EntityType type)
		{
			_type = type;
			_fields = new object[_type.DataFields];
		}

		#endregion

		#region Edit Operations

		// The entity cannot be edited itself,
		// in order to edit constructing an EntityEdit
		// for this entity
		public EntityEdit Edit()
		{
			return new EntityEdit(this);
		}

		// Delete calls EntityType.Delete to delete
		// itself
		public bool Delete()
		{
			return _type.Delete(this);
		}


		public void Refresh(Entity ne)
		{
			if (ne.Id != Id)
				throw new EntityException("New entity's id don't match old entity's id");
			_fields = ne._fields;
		}

		#endregion

		#region Object Members

		public override bool Equals(object obj)
		{
			if (this == obj)
				return true;
			if (obj is Entity)
			{
				return _type == ((Entity)obj)._type &&
					Id == ((Entity)obj).Id;
			}
			if (obj is EntityBase)
			{
				return Equals(((EntityBase)obj).Entity);
			}

			return false;
		}

		public override int GetHashCode()
		{
			return Id;
		}

		#endregion

		#region ISerializable Members

		protected Entity(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			_type = context.Context as EntityType;
			_fields = (object[])info.GetValue("fields", typeof(object[]));
		}

		public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			info.AddValue("fields", _fields);
		}

		#endregion

		public int CompareTo(object obj)
		{
			if (obj == null)
				return -1;
			if (!(obj is Entity))
				return -2;
			return this.Id.CompareTo((obj as Entity).Id);
		}
	}

	public abstract class OfflineEntity
	{
		public static readonly string FolderPath = "";
		public static readonly string FileName = "offlinedata.xml";
		public int OfflineID;

		protected abstract string GetOfflineType();
		protected abstract OfflineField[] GetFields();
		protected abstract OfflineEntity GetExistingEntity();
		protected abstract void Load(OfflineField[] fields);
		//protected abstract void Reset();

		static OfflineEntity()
		{
			if (Utils.IsRunningLocal())
			{
				FolderPath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
			}
			else
			{
				FolderPath = HttpContext.Current.Server.MapPath(".");
			}
		}

		protected OfflineEntity()
		{
			this.OfflineID = -1;
		}

		private bool Save(bool delete)
		{
			string type = this.GetOfflineType();
			if ((type == null) || (type.Length == 0))
				throw new Exception("can't save offline entity: no type defined");
			string strFilePath = FolderPath + Path.DirectorySeparatorChar + FileName;
			OfflineField[] fields = this.GetFields();
			if ((fields == null) || (fields.Length == 0))
				throw new Exception("can't save offline entity of type " + type + ": no fields");
			Sport.Common.IniFile ini = new Sport.Common.IniFile(strFilePath);
			int count = Sport.Common.Tools.CIntDef(ini.ReadValue(
				type, "count"), 0);
			int ID = this.OfflineID;
			OfflineEntity existingEntity = null;
			if (ID < 0)
			{
				ID = count;
				existingEntity = this.GetExistingEntity();
				if (existingEntity != null)
				{
					ID = existingEntity.OfflineID;
				}
				else
				{
					count++;
					ini.WriteValue(type, "count", count.ToString());
				}
			}
			string strSection = type + "_" + ID;
			foreach (OfflineField field in fields)
			{
				string curValue = field.Value;
				if ((delete == true) && (existingEntity == null))
				{
					curValue = "";
				}
				ini.WriteValue(strSection, field.Name, curValue);
			}
			return (existingEntity == null);
		}

		public bool Save()
		{
			return Save(false);
		}

		public void Delete()
		{
			Save(true);
		}

		public static OfflineEntity[] LoadAllEntities(System.Type entType)
		{
			OfflineEntity entity = (OfflineEntity)
				System.Activator.CreateInstance(entType);
			string type = entity.GetOfflineType();
			if ((type == null) || (type.Length == 0))
				throw new Exception("can't load all entities: no type defined");
			string strFilePath = FolderPath + Path.DirectorySeparatorChar + FileName;
			OfflineField[] fields = entity.GetFields();
			if ((fields == null) || (fields.Length == 0))
				throw new Exception("can't load all entities of type " + type + ": no fields");
			Sport.Common.IniFile ini = new Sport.Common.IniFile(strFilePath);
			int count = Sport.Common.Tools.CIntDef(ini.ReadValue(
				type, "count"), -1);
			ArrayList entities = new ArrayList();
			for (int i = 0; i <= count; i++)
			{
				string strSection = type + "_" + i;
				string strValue = ini.ReadValue(strSection, fields[0].Name);
				if ((strValue != null) && (strValue.Length > 0))
				{
					fields[0].Value = strValue;
					for (int j = 1; j < fields.Length; j++)
					{
						strValue = Sport.Common.Tools.CStrDef(
							ini.ReadValue(strSection, fields[j].Name), "");
						fields[j].Value = strValue;
					}
					OfflineEntity curEntity = (OfflineEntity)
						System.Activator.CreateInstance(entType);
					curEntity.OfflineID = i;
					curEntity.Load(fields);
					entities.Add(curEntity);
				}
			}
			return (OfflineEntity[])entities.ToArray(typeof(OfflineEntity));
		}

		public static OfflineEntity LoadEntity(System.Type entType, int ID)
		{
			OfflineEntity entity = (OfflineEntity)
				System.Activator.CreateInstance(entType);
			string type = entity.GetOfflineType();
			if ((type == null) || (type.Length == 0))
				throw new Exception("can't load entity: no type defined");
			string strFilePath = FolderPath + Path.DirectorySeparatorChar + FileName;
			OfflineField[] fields = entity.GetFields();
			if ((fields == null) || (fields.Length == 0))
				throw new Exception("can't load entity of type " + type + ": no fields");
			Sport.Common.IniFile ini = new Sport.Common.IniFile(strFilePath);
			string strSection = type + "_" + ID;
			string strValue = ini.ReadValue(strSection, fields[0].Name);
			if ((strValue == null) || (strValue.Length == 0))
				return null;
			fields[0].Value = strValue;
			for (int j = 1; j < fields.Length; j++)
			{
				strValue = Sport.Common.Tools.CStrDef(
					ini.ReadValue(strSection, fields[j].Name), "");
				fields[j].Value = strValue;
			}
			OfflineEntity result = (OfflineEntity)
					System.Activator.CreateInstance(entType);
			result.OfflineID = ID;
			result.Load(fields);
			return result;
		}

		protected class OfflineField
		{
			public string Name = "";
			public string Value = "";
			public OfflineField(string name, string value)
			{
				this.Name = name;
				this.Value = value;
			}
		}
	}
}
