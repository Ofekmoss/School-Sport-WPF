using System;

namespace Sport.Data
{
	#region EntityEdit

	/// <summary>
	/// EntityEdit enable changing the values of an entity's
	/// fields and to create a new entity.
	/// The EntityEdit is constructed when a new entity needs
	/// to be created or an existing entity needs to be changed.
	/// </summary>
	public class EntityEdit : Entity
	{
		// The entity being edited or null for new entity
		private Entity		_entity;
		public Entity Entity
		{
			get { return _entity; }
		}

		public new int Id
		{
			get
			{
				return base.Id;
			}
			set
			{
				if (_type.EntityId != null)
				{
					_type.EntityId.SetId(this, value);
				}
			}
		}

		// EntityEdit constructor for edit on an existing entity
		// Receives the entity to be changed
		public EntityEdit(Entity entity)
			: base(entity.EntityType)
		{
			_entity = entity;
			// Copying the current values of the entity fields
			Array.Copy(_entity.Fields, _fields, _entity.Fields.Length);
		}

		// EntityEdit constructor for a new entity
		// Receives the new entity's type
		public EntityEdit(EntityType type)
			: base(type)
		{
			type.OnNewEntity(this);
		}

		// Called after the new entity was stored,
		// EntityEdit needs to replicate itself to its Entity
		// property
		public void CreateEntity()
		{
			_entity = new Entity(_type, _fields);
		}

		// Called to check entity edit before saving
		// if checks if fields are changed and if must-exist
		// fields exist
		public EntityResult Check()
		{
			bool changed = _entity == null;
			// Checking that all needed fields are set
			// and that there was a change
			for (int n = 0; n < _type.DataFields; n++)
			{
				EntityField field = _type.Fields[n];

				if (!changed)
				{
					if (!field.Equals(this, field.GetValue(_entity)))
						changed = true;
				}

				if (field.MustExist && field.IsEmpty(this))
				{
					return new EntityFieldResult(EntityResultCode.FieldMustExist, field);
				}
			}

			// If not changed nothing needs to be done
			if (!changed)
				return new EntityResult(EntityResultCode.NoChange);

			return EntityResult.Ok;
		}

		// Called when the changes made need to stored
		public EntityResult Save()
		{
			// The storing of the data is done by the
			// EntityType
			return _type.Save(this);
		}

		#region Field Change Events

		public class FieldEventArgs : EventArgs
		{
			private EntityField _field;
			public EntityField EntityField
			{
				get { return _field; }
			}

			public FieldEventArgs(EntityField field)
			{
				_field = field;
			}
		}

		public delegate void FieldEventHandler(object sender, FieldEventArgs e);

		public event FieldEventHandler FieldChanged;

		internal void OnValueChange(EntityField field)
		{
			if (FieldChanged != null)
				FieldChanged(this, new FieldEventArgs(field));

			
			EntityType.OnValueChange(this, field);
		}

		#endregion
	}

	#endregion
}
