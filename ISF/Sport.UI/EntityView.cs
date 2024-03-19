using System;
using Sport.Data;

namespace Sport.UI
{
	/// <summary>
	/// EntityFieldView stores displaying information of an EntityField 
	/// </summary>
	public class EntityFieldView : IVisualizerField
	{
		private string _name;
		public string Name
		{
			get { return _name; }
			set 
			{ 
				if (_name != value)
				{
					_name = value; 
					OnChange();
				}
			}
		}

		private System.Drawing.Size _size;
		public System.Drawing.Size Size
		{
			get { return _size; }
			set 
			{ 
				if (_size != value)
				{
					_size = value; 
					OnChange();
				}
			}
		}

		public int Width
		{
			get { return _size.Width; }
			set
			{
				if (_size.Width != value)
				{
					_size.Width = value;
					OnChange();
				}
			}
		}

		public int Height
		{
			get { return _size.Height; }
			set
			{
				if (_size.Height != value)
				{
					_size.Height = value;
					OnChange();
				}
			}
		}

		private Sport.UI.Controls.GenericItemType _genericItemType;
		public Sport.UI.Controls.GenericItemType GenericItemType
		{
			get { return _genericItemType; }
			set 
			{ 
				if (_genericItemType != value)
				{
					_genericItemType = value; 
					OnChange();
				}
			}
		}

		private object[] _values;
		public object[] Values
		{
			get { return _values; }
			set 
			{ 
				if (_values != value)
				{
					_values = value; 
					OnChange();
				}
			}
		}

		private EntityField _entityField;
		public EntityField EntityField
		{
			get { return _entityField; }
		}

		private bool _canEdit;
		public bool CanEdit
		{
			get { return _canEdit; }
			set
			{
				if (_entityField.CanEdit && _canEdit != value)
				{
					_canEdit = value;
					OnChange();
				}
			}
		}

		private bool _mustExist;
		public bool MustExist
		{
			get { return _mustExist; }
			set
			{
				if (!_entityField.MustExist && _mustExist != value)
				{
					_mustExist = value;
					OnChange();
				}
			}
		}

		public EntityFieldView(EntityField entityField)
		{
			_entityField = entityField;
			_name = "Field " + _entityField.Index.ToString();

			Size = new System.Drawing.Size(1, 0);
			_genericItemType = Sport.UI.Controls.GenericItemType.Text;
			_values = null;

			if (_entityField is EntityEntityField ||
				_entityField is LookupEntityField)
			{
				_genericItemType = Sport.UI.Controls.GenericItemType.Selection;
				if (_entityField is LookupEntityField)
				{
					_values = ((LookupEntityField) _entityField).LookupType.Items;
				}
			}
			else if (_entityField is LetterNumberEntityField)
			{
				_genericItemType = Sport.UI.Controls.GenericItemType.Text;
				_values = Sport.UI.Controls.GenericItem.TextValues(
					new Sport.UI.Controls.LetterNumberController(((LetterNumberEntityField)_entityField).MinValue,
					((LetterNumberEntityField)_entityField).MaxValue), true);
			}
			else if (_entityField is NumberEntityField)
			{
				NumberEntityField nef = _entityField as NumberEntityField;
				_genericItemType = Sport.UI.Controls.GenericItemType.Number;
				_values = Sport.UI.Controls.GenericItem.NumberValues(
					nef.MinValue, nef.MaxValue, nef.Scale, nef.Precision);
			}
			else if (_entityField is DateTimeEntityField)
			{
				_genericItemType = Sport.UI.Controls.GenericItemType.DateTime;
				_values = new object[] { ((DateTimeEntityField) _entityField).Format };
			}
			else if (_entityField is TextEntityField)
			{
				if (((TextEntityField) _entityField).Multiline)
				{
					_genericItemType = Sport.UI.Controls.GenericItemType.Memo;
				}
				else
				{
					_genericItemType = Sport.UI.Controls.GenericItemType.Text;
				}
			}

			_canEdit = _entityField.CanEdit;
			_mustExist = _entityField.MustExist;
		}

		public event EventHandler Changed;

		protected void OnChange()
		{
			if (Changed != null)
				Changed(this, EventArgs.Empty);
		}

		public override string ToString()
		{
			return _name;
		}

		#region IVisualizerField Members

		public int Field
		{
			get { return _entityField.Index; }
		}

		public string Title
		{
			get { return _name; }
		}

		public int DefaultWidth
		{
			get { return _size.Width; }
		}

		public System.Drawing.StringAlignment Alignment
		{
			get { return System.Drawing.StringAlignment.Near; }
		}

		public System.Collections.IComparer Comparer
		{
			get { return new EntityFieldComparer(_entityField); }
		}

		#endregion
	}

	/// <summary>
	/// EntityView handles the display of an entity
	/// </summary>
	public class EntityView : IVisualizer
	{
		private EntityType _entityType;
		public EntityType EntityType
		{
			get { return _entityType; }
		}

		private EntityFieldView[] _fields;
		public EntityFieldView[] Fields
		{
			get { return _fields; }
		}

		public EntityFieldView this[int field]
		{
			get { return _fields[field]; }
		}

		private string _name;
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		private string _pluralName;
		public string PluralName
		{
			get { return _pluralName; }
			set { _pluralName = value; }
		}

		public EntityView(EntityType entityType)
		{
			_entityType = entityType;

			_fields = new EntityFieldView[_entityType.Fields.Length];
			for (int n = 0; n < _entityType.Fields.Length; n++)
			{
				_fields[n] = new EntityFieldView(_entityType.Fields[n]);
			}

			_name = _entityType.Name;
			_pluralName = _entityType.Name + "s";
		}

		/*
		// NewEntity is called when a new entity should be created
		public virtual bool New(ParameterList parameters, out bool succeeded)
		{
			succeeded = false;
			return false;
		}*/

		// DeleteEntity is called when an entity should be deleted
		/*public virtual bool DeleteEntity(Entity entity)
		{
			// Default behaviour, just delete the entity
			return entity.Delete();
		}*/

		// OnSelectEntity is called when an entity is selected
		public virtual void OnSelectEntity(Entity entity)
		{
		}

		// OnValueChange is called when a field value is set
		public virtual void OnValueChange(Sport.Data.EntityEdit entityEdit, Sport.Data.EntityField entityField)
		{
		}

		// OnEditEntity is called when an entity edit starts
		public virtual void OnEditEntity(EntityEdit entityEdit)
		{
		}

		// OnNewEntity is called when a new entity is created
		public virtual void OnNewEntity(EntityEdit entityEdit)
		{
		}

		// OnDeleteEntity is called before deleting an entity
		// If the function returns 'false' the entity will not be deleted
		public virtual bool OnDeleteEntity(Sport.Data.Entity entity)
		{
			Sport.Data.EntityBase objBase=null;
			string strTypeName = entity.EntityType.Name;
			if (strTypeName == Sport.Entities.Student.TypeName)
				objBase = new Sport.Entities.Student(entity);
			else if (strTypeName == Sport.Entities.School.TypeName)
				objBase = new Sport.Entities.School(entity);
			else if (strTypeName == Sport.Entities.ChampionshipCategory.TypeName)
				objBase = new Sport.Entities.ChampionshipCategory(entity);
			else if (strTypeName == Sport.Entities.SportField.TypeName)
				objBase = new Sport.Entities.SportField(entity);
			else if (strTypeName == Sport.Entities.Facility.TypeName)
				objBase = new Sport.Entities.Facility(entity);
			else if (strTypeName == Sport.Entities.Court.TypeName)
				objBase = new Sport.Entities.Court(entity);
			else if (strTypeName == Sport.Entities.PracticeCamp.TypeName)
				objBase = new Sport.Entities.PracticeCamp(entity);
			else if (strTypeName == Sport.Entities.Account.TypeName)
				objBase = new Sport.Entities.Account(entity);
			
			if (objBase != null)
			{
				string strMessage=objBase.CanDelete();
				if (strMessage.Length > 0)
				{
					Sport.UI.MessageBox.Show(strMessage, "מחיקת נתונים", System.Windows.Forms.MessageBoxIcon.Warning);
					return false;
				}
			}
			return Sport.UI.MessageBox.Ask("האם למחוק " + _name + " - '" + entity.Name + "'?", false);
		}

		// OnSaveEntity is called after an entity is saved
		public virtual void OnSaveEntity(Sport.Data.Entity entity)
		{
		}

		// OnCancelEntity is called after an entity edit is canceled
		public virtual void OnCancelEntity(Sport.Data.Entity entity)
		{
		}

		#region IVisualizer Members

		public int GetFieldCount()
		{
			return _entityType.Fields.Length;
		}

		public IVisualizerField GetField(int field)
		{
			return Fields[field];
		}

		public IVisualizerField[] GetFields()
		{
			IVisualizerField[] fields = new IVisualizerField[_entityType.Fields.Length];
			for (int n = 0; n < _entityType.Fields.Length; n++)
				fields[n] = GetField(n);
			return fields;
		}

		public string GetText(object o, int field)
		{
			EntityFieldView efv = Fields[field];
			if (o is Sport.Data.EntityBase)
			{
				string text = (o as Sport.Data.EntityBase).GetCustomText(field);
				if (text != null)
					return text;
				else
					return efv.EntityField.GetText(((Sport.Data.EntityBase) o).Entity);
			}
			else if (o is Sport.Data.Entity)
			{
				Sport.Data.Entity entity = (Sport.Data.Entity) o;
				if (entity != null)
				{
					string strText = efv.EntityField.GetText(entity);
					string strExtraText = EntityView.GetExtraText(entity, field);
					if (strExtraText != null && strExtraText.Length > 0)
					{
						strText += " - " + strExtraText;
					}
					return strText;
				}
			}
			return null;
		}

		#endregion

		public static string GetExtraText(Sport.Data.Entity entity, int field)
		{
			if (entity != null && entity.EntityType != null && 
				entity.EntityType.Name == Sport.Entities.AccountEntry.TypeName)
			{
				if (field == (int) Sport.Entities.AccountEntry.Fields.Description)
				{
					object objEntryType = entity.Fields[(int)Sport.Entities.AccountEntry.Fields.EntryType];
					if (objEntryType != null && (int) objEntryType == 1)
					{
						int chargeID = (entity.Id / 10);
						Sport.Entities.ChampionshipCategory category = null;
						try
						{
							Sport.Entities.Charge charge = new Sport.Entities.Charge(chargeID);
							category = charge.ChampionshipCategory;
						}
						catch
						{}
						if (category != null)
						{
							return category.Championship.Name + " " + category.Name;
						}
					}
				}
			}
			return null;
		}
	}
}
