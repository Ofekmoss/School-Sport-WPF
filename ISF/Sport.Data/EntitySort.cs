using System;
using System.Collections;
using Sport.Common;

namespace Sport.Data
{
	// Enumeration of possion sorting directions
	public enum EntitySortDirection
	{
		Ascending,
		Descending
	}

	/// <summary>
	/// EntitySortField stores the field and direction of
	/// a single sort field.
	/// </summary>
	public class EntitySortField
	{
		private EntityField			_field;
		private EntitySortDirection	_direction;

		public EntityField Field { get { return _field; } }
		public EntitySortDirection Direction { get { return _direction; } }

		public EntitySortField(EntityField field, EntitySortDirection direction)
		{
			_field = field;
			_direction = direction;
		}
	}

	/// <summary>
	/// EntitySort implements the comparer for EntityList.
	/// EntitySort stores an array of sort fields and
	/// compares entities in the EntityList according
	/// to the sort field.
	/// </summary>
	public class EntitySort : IComparer
	{
		// Stores the owner list
		private EntityList _list;
		// Stores the sort fields
		private EntitySortField[] fields;

		public EntitySortField[] Fields
		{
			get { return fields; }
			set
			{
				fields = value;
				_list.Sort();
			}
		}

		public EntitySort(EntityList list)
		{
			_list = list;
			fields = null;
		}

		// Compare between two entities according to the sort fields
		public int Compare(object x, object y)
		{
			// both values must be entities
			if (x is Entity && y is Entity)
			{
				Entity ex = (Entity) x;
				Entity ey = (Entity) y;

				// Checking if there are fields in the sort
				if (fields != null)
				{
					// For each sort field in the sort
					foreach (EntitySortField esf in fields)
					{
						// comparing the values of the field of the two entities
						int r = esf.Field.Compare(ex, ey);
						if (r != 0)
						{
							// If values are not equal, then if the
							// sort is in ascending order returning the 
							// result of the field compare, and
							// if the order is descending retuning the 
							// opposite
							return esf.Direction == 
								EntitySortDirection.Ascending ? r : -r;
						}
					}
				}

				// Must return a consistent order
				// so if entities match by column we're
				// comparing the entities' ids.
				if (ex.Id > ey.Id)
					return 1;
				if (ex.Id < ey.Id)
					return -1;

				return 0;
			}

			throw new EntityException("Can only compare entities");
		}
	}

	public class EntityFieldComparer : IComparer
	{
		private EntityField _entityField;
		public EntityField EntityField
		{
			get { return _entityField; }
		}

		public EntityFieldComparer(EntityField entityField)
		{
			_entityField = entityField;
		}

		#region IComparer Members

		public int Compare(object x, object y)
		{
			return _entityField.Compare(x as Entity, y);
		}

		#endregion
	}
}
