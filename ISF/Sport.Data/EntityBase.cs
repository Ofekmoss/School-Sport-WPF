using System;
using Mir.Common;

namespace Sport.Data
{
	/// <summary>
	/// EntityBase implement the basic operation on an entity.
	/// Inheriting class will extend the EntityBase for its
	/// EntityType
	/// </summary>
	public class EntityBase
	{
		#region Properties

		// Stores the Entity or EntityEdit operated by this EntityBase
		protected Entity _entity;
		public Entity Entity
		{
			get { return _entity; }
		}
		public EntityEdit EntityEdit
		{
			get { return _entity as EntityEdit; }
		}

		// Gets an sets the id of the entity
		public int Id
		{
			get 
			{ 
				return (_entity == null)?-1:_entity.Id; 
			}
			set
			{
				// If the EntityBase does not store an EntityEdit
				// the Id value cannot be set
				EntityEdit ee = EntityEdit;
				if (ee == null)
					throw new EntityException("Entity uneditable");
				ee.Id = value;
			}
		}

		// Gets the name of the entity
		public string Name
		{
			get
			{
				if (_entity == null)
					return "";
				return _entity.Name;
			}
		}

		// Gets and sets the given field
		public object this[int field]
		{
			get
			{
				return GetValue(field);
			}
			set
			{
				SetValue(field, value);
			}
		}

		#endregion
		
		#region Constructors
		// Contructs an EntityBase containing an Entity
		public EntityBase(Entity entity)
		{
			_entity = entity;
		}

		// Contructs an EntityBase containing an EntityEdit
		public EntityBase(EntityEdit entityEdit)
		{
			_entity = entityEdit;
		}

		// Contructs an EntityBase containing the Entity of
		// the given id in the give EntityType
		private static int _offlineEntityCounter=0;
		private static System.Collections.Hashtable _offlineEntities=
			new System.Collections.Hashtable();
		public EntityBase(EntityType type, int id)
		{
			if (id < 0)
			{
				System.Diagnostics.Debug.WriteLine("EntityBase: got negative ID ("+id+")");
				_entity = null;
				return;
			}
			
			//Sport.Common.Tools.WriteToLog("EntityBase called, type: " + type.Name + ", id: " + id + ", connected? " + Sport.Core.Session.Connected);

			if (!Sport.Core.Session.Connected)
			{
				if (_offlineEntities[type.Name] == null)
					_offlineEntities[type.Name] = new System.Collections.Hashtable();
				System.Collections.Hashtable tblEntities=
					(System.Collections.Hashtable) _offlineEntities[type.Name];
				if (tblEntities[id] != null)
				{
					_entity = (Entity) tblEntities[id];
					return;
				}
				
				System.Diagnostics.Debug.WriteLine("("+_offlineEntityCounter+") reading offline entity: "+type.Name+" with id "+id);
				DateTime date1=DateTime.Now;
				
				Sport.Common.IniFile ini=GetIniFile(type.Name);
				if (ini == null)
					return;
				_entity = null;
				int count=Sport.Common.Tools.CIntDef(
					ini.ReadValue("Fields", "Count"), -1);
				if (count > 0)
				{
					object[] fields=new object[count];
					for (int i=0; i<count; i++)
					{
						string strType=Sport.Common.Tools.CStrDef(
							ini.ReadValue("FieldTypes", "Field_"+i), "");
						if (strType.Length == 0)
							continue;
						System.Type fieldType=System.Type.GetType(strType);
						if (fieldType == null)
							continue;
						string strValue=ini.ReadValue("Entity_"+id, "Field_"+i);
						object value=null;
						if (strValue != null)
						{
							if (strType.IndexOf("DateTime") >= 0)
							{
								value = new DateTime(Sport.Common.Tools.CLngDef(strValue, 0));
							}
							else
							{
								if (fieldType.FullName != DBNull.Value.GetType().FullName)
								{
									try
									{
										value = System.Convert.ChangeType(strValue, fieldType);
									}
									catch (Exception ex)
									{
										System.Diagnostics.Debug.WriteLine("failed to read field "+i+" for entity "+type.Name+" with id "+id+": "+ex.Message);
										System.Diagnostics.Debug.WriteLine(ex.StackTrace);
									}
								}
							}
						}
						fields[i] = value;
					}
					_entity = new Entity(type, fields);
					tblEntities[id] = _entity;
				}
				
				DateTime date2=DateTime.Now;
				System.Diagnostics.Debug.WriteLine("done, reading took "+(date2-date1).TotalMilliseconds+" milli seconds. ["+((_entity == null)?0:_entity.Fields.Length)+"]");
				_offlineEntityCounter++;
				
				return;
			}
			
			try
			{
				_entity = type.Lookup(id);
			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.WriteLine("failed to create EntityBase: "+e.Message);
				System.Diagnostics.Debug.WriteLine("type: "+type.Name+", id: "+id.ToString());
				System.Diagnostics.Debug.WriteLine(e.StackTrace);
				_entity = null;
			}
		}

		#endregion

		#region Field Functions
        
		// Get the value of the given field
		protected object GetValue(int field)
		{
			if (_entity == null)
				return null;
			if (_entity.EntityType == null)
				return null;
			if (_entity.EntityType.Fields[field] == null)
				return null;
			object value = _entity.EntityType.Fields[field].GetValue(_entity);
			if ((value is int && ((int)value) == -1) || (value is long && ((long)value) == -1) || (value is double && ((double)value) == -1))
				value = null;
			if (value is DateTime && ((DateTime)value).Year < 1900)
				value = null;
			return value;
		}

		// Sets the value of the given field
		protected void SetValue(int field, object value)
		{
			// If the EntityBase does not store an EntityEdit
			// the field value cannot be set
			EntityEdit ee = EntityEdit;
			if (ee == null)
				throw new EntityException("Entity uneditable");

			_entity.EntityType.Fields[field].SetValue(ee, value);
		}

		// Get the Entity for the value of the given field
		protected Entity GetEntityValue(int field)
		{
			if ((_entity == null)||(_entity.EntityType == null))
				return null;
			
			EntityEntityField entityField = _entity.EntityType.Fields[field]
				as EntityEntityField;

			if (entityField == null)
				throw new EntityException("Field must be entity field");

			object id = GetValue(field);
			if (id == null)
				return null;

			if (entityField.EntityType == null)
				return null;

			int value = -1;
			if (id is Int32)
				value = (int)id;
			else
				Int32.TryParse(id.ToString(), out value);
			if (value < 0)
				return null;

			Entity result = null;
			try
			{
				result = entityField.EntityType.Lookup(value);
			}
			catch (Exception ex)
			{
				Logger.Instance.WriteLog(LogType.Error, "EntityBase", "Error getting value: " + ex.ToString());
				result = null;
			}
			return result;
		}

		#endregion

		#region Entity Operations

		public void Edit()
		{
			if (_entity is EntityEdit)
				return ;

			_entity = _entity.Edit();
		}

		public EntityResult Save(bool blnShowError)
		{
			if (_entity is EntityEdit)
			{
				EntityResult result = EntityEdit.Save();

				if (result.Succeeded)
				{
					_entity = EntityEdit.Entity;
				}
				else
				{
					if ((result.ResultCode == EntityResultCode.Failed)&&
						(result is EntityMessageResult))
					{
						if (blnShowError)
						{
							System.Windows.Forms.MessageBox.Show("שגיאה בעת שמירת נתונים:\n"+
								(result as EntityMessageResult).Message, "שמירת נתונים",
								System.Windows.Forms.MessageBoxButtons.OK,
								System.Windows.Forms.MessageBoxIcon.Error,
								System.Windows.Forms.MessageBoxDefaultButton.Button1,
								System.Windows.Forms.MessageBoxOptions.RightAlign|
								System.Windows.Forms.MessageBoxOptions.RtlReading);
						}
					}
				}

				return result;
			}

			return new EntityResult(EntityResultCode.NotEditing);
		}

		public EntityResult Save()
		{
			return Save(true);
		}

		public void Cancel()
		{
			if (_entity is EntityEdit && EntityEdit.Entity != null)
			{
				_entity = EntityEdit.Entity;
			}
		}

		#endregion
		
		#region export
		private Sport.Common.IniFile GetIniFile(string name)
		{
			Sport.Common.IniFile  ini=null;
			try
			{
				ini =  new Sport.Common.IniFile(
					Core.Session.GetSeasonCache(false) + System.IO.Path.DirectorySeparatorChar + name + "s.xml");
			}
			catch
			{}
			return ini;
		}
		
		public virtual void Export()
		{
			if (_entity == null)
				return;
			Sport.Common.IniFile ini=GetIniFile(_entity.EntityType.Name);
			if (ini == null)
				return;
			int count=_entity.Fields.Length;
			ini.WriteValue("Fields", "Count", count.ToString());
			string strSection="Entity_"+_entity.Id;
			for (int i=0; i<count; i++)
			{
				System.Type type=null;
				object value=_entity.Fields[i];
				if ((value != null)&&(!(value is System.DBNull)))
					type = value.GetType();
				if (value is DateTime)
					value = ((DateTime) value).Ticks;
				if (type != null)
				{
					ini.WriteValue("FieldTypes", "Field_"+i, type.FullName);
					ini.WriteValue(strSection, "Field_"+i, 
						Sport.Common.Tools.CStrDef(value, ""));
				}
			}
		}
		#endregion
		
		#region Object Members

		/// <summary>
		/// returns whether the current entity is valid or not
		/// </summary>
		public virtual bool IsValid()
		{
			return (_entity != null);
		}
		
		public virtual string CanDelete()
		{
			if (!this.IsValid())
				return "רשומה לא חוקית";
			return "";
		}
		
		public virtual string GetCustomText(int field)
		{
			return null;
		}

		public override string ToString()
		{
			return _entity.ToString();
		}

		public override bool Equals(object obj)
		{
			if ((obj == null)&&(_entity == null))
				return true;
			if ((obj == null)||(_entity == null))
				return false;
			if (obj is Entity)
				return _entity.Equals((Entity) obj);
			if (obj is EntityBase)
			{
				EntityBase ent=(EntityBase) obj;
				if (ent._entity == null)
				{
					return false;
				}
				return _entity.Equals(ent._entity);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return Id;
		}


		#endregion

	}
}
