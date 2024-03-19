using System;
using Sport.Common;
using Sport.Data;

namespace Sport.UI
{
	/// <summary>
	/// EntityEventArgs adds an entity to the event arguments.
	/// </summary>
	public class FieldEditEventArgs : System.EventArgs
	{
		private EntityEdit	_entityEdit;
		private EntityField	_entityField;

		public EntityEdit	EntityEdit	{ get { return _entityEdit; } }
		public EntityField	EntityField { get { return _entityField; } }

		public FieldEditEventArgs(EntityEdit entityEdit, EntityField entityField)
		{
			_entityEdit = entityEdit;
			_entityField = entityField;
		}
	}

	// Delegate that receives EntityEventArgs
	public delegate void FieldEditEventHandler(object sender, FieldEditEventArgs e);

	public class EntityListView : IDisposable
	{
		private string _entityName="";
		public string EntityName
		{
			get { return _entityName; }
		}

		#region View Items

		#region View Item Events

		/// <summary>
		/// The view item events are used by the view items
		/// to receive notification on changes in the view
		/// </summary>

		private delegate void ViewItemHandler(ViewItem source);
		private delegate void ViewItemOperationHandler(ViewItem source, EntityChangeOperation operation);
		private delegate void ViewItemFieldHandler(ViewItem source, EntityField field);
		private delegate void ViewItemViewFieldHandler(ViewItem source, Field field, FieldChangeType changeType);

		// ViewItemListChanged is called when the list is replaced
		// or its size changes
		private event ViewItemHandler ViewItemListChanged;
		// ViewItemCurrentChanged is called when the current entity
		// in the view is changed
		private event ViewItemHandler ViewItemCurrentChanged;
		// ViewItemEntityChanged is called when an entity's operation 
		// occurs
		private event ViewItemOperationHandler ViewItemEntityChanged;
		// ViewItemFieldChanged is called when a field value is changed
		private event ViewItemFieldHandler ViewItemFieldChanged;
		// ViewItemViewFieldChanged is called when a view field is changed
		private event ViewItemViewFieldHandler ViewItemViewFieldChanged;

		private void OnViewItemListChange(ViewItem source)
		{
			if (ViewItemListChanged != null)
				ViewItemListChanged(source);
		}

		private void OnViewItemCurrentChange(ViewItem source)
		{
			if (ViewItemCurrentChanged != null)
				ViewItemCurrentChanged(source);
		}

		private void OnViewItemEntityChange(ViewItem source, EntityChangeOperation operation)
		{
			if (ViewItemEntityChanged != null)
				ViewItemEntityChanged(source, operation);
		}

		private void OnViewItemFieldChange(ViewItem source, EntityField field)
		{
			if (ViewItemFieldChanged != null)
				ViewItemFieldChanged(source, field);
		}

		private void OnViewItemViewFieldChange(ViewItem source, Field field, FieldChangeType changeType)
		{
			if (ViewItemViewFieldChanged != null)
				ViewItemViewFieldChanged(source, field, changeType);
		}

		#endregion

		#region ViewItem Class

		/// <summary>
		/// ViewItem is a single item in a view the should act on
		/// entity change and value change in the view
		/// </summary>
		private abstract class ViewItem : GeneralCollection.CollectionItem
		{
			// Gets the distinct value of the view item
			public abstract object Id { get; }
			// Called when the item is removed from the EntityListView
			public abstract void Remove();
		}

		#endregion

		#region GenericItemViewItem Class

		/// <summary>
		/// GenericItemViewItem implements a ViewItem that connects
		/// a view field with a generic item.
		/// </summary>
		private class GenericItemViewItem : ViewItem
		{
			private Sport.UI.Controls.GenericItem _genericItem;
			public Sport.UI.Controls.GenericItem GenericItem
			{
				get { return _genericItem; }
			}

			private EntityField _field;
			public EntityField Field
			{
				get { return _field; }
			}

			public GenericItemViewItem(Sport.UI.Controls.GenericItem genericItem, EntityField field)
			{
				_genericItem = genericItem;
				_field = field;
				_genericItem.ReadOnly = !_field.CanEdit;
				_genericItem.Nullable = !_field.MustExist;
				_genericItem.ValueChanged += new EventHandler(GenericValueChanged);
			}

			public override void OnOwnerChange(object oo, object no)
			{
				if (oo != null)
				{
					((EntityListView)oo).ViewItemEntityChanged -= new ViewItemOperationHandler(EntityChanged);
					((EntityListView)oo).ViewItemCurrentChanged -= new ViewItemHandler(CurrentChanged);
					((EntityListView)oo).ViewItemViewFieldChanged -= new ViewItemViewFieldHandler(ViewFieldChanged);
					((EntityListView)oo).ViewItemFieldChanged -= new ViewItemFieldHandler(FieldChanged);
				}
				if (no != null)
				{
					((EntityListView)no).ViewItemEntityChanged += new ViewItemOperationHandler(EntityChanged);
					((EntityListView)no).ViewItemCurrentChanged += new ViewItemHandler(CurrentChanged);
					((EntityListView)no).ViewItemViewFieldChanged += new ViewItemViewFieldHandler(ViewFieldChanged);
					((EntityListView)no).ViewItemFieldChanged += new ViewItemFieldHandler(FieldChanged);
				}

				base.OnOwnerChange (oo, no);
			}


			public override object Id
			{
				get { return _genericItem; }
			}

			public override void Remove()
			{
				_genericItem.ValueChanged -= new EventHandler(GenericValueChanged);
			}

			private bool setting = false;
			private void SetGenericItem()
			{
				setting = true;
				if (Owner != null)
				{
					try
					{
						Entity entity = ((EntityListView) Owner).Current;
						_genericItem.Value = (entity == null)?null:_field.GetValueItem(entity);
					}
					catch (Exception e)
					{
						System.Diagnostics.Debug.WriteLine("failed to set generic item: "+e.Message);
						_genericItem.Value = null;
					}
				}
				setting = false;
			}

			
			private void GenericValueChanged(object sender, EventArgs e)
			{
				if (!setting)
				{
					object value=_genericItem.Value;
					EntityListView listView=(EntityListView) Owner;
					listView.SetFieldValue(this, _field, value);
					/*
					if ((value == null)||(value is Sport.Data.Entity)||(value is Sport.Data.EntityBase)||
						(value is Int32)||(value is Double)||
						(value is Sport.Data.LookupItem)||(value is String)||
						(value is Sport.Types.DateSetType))
					{
						listView.SetFieldValue(this, _field, value);
					}
					else
					{
						if (value is Sport.Data.Entity[])
						{
							Sport.Data.Entity[] arrEntities=
								(Sport.Data.Entity[]) value;
							//change current value:
							if (arrEntities.Length > 0)
								listView.SetFieldValue(this, _field, arrEntities[0].Id);
							//and add all the rest:
							for (int i=1; i<arrEntities.Length; i++)
							{
								listView.EntityList.Add(arrEntities[i]);
							}
						}
						else
						{
							System.Windows.Forms.MessageBox.Show("invalid value type: "+
								value.GetType().Name);
						}
					}
					*/
				}
			}

			private void EntityChanged(ViewItem source, EntityChangeOperation operation)
			{
				if (source == this)
					return ;
				if (operation == EntityChangeOperation.Cancel)
					SetGenericItem();
			}

			private void CurrentChanged(ViewItem source)
			{
				if (source != this)
					SetGenericItem();
			}

			private void ViewFieldChanged(ViewItem source, Field field, FieldChangeType changeType)
			{
				if (source == this)
					return ;

				if (field.EntityField.Equals(_field))
				{
					switch (changeType)
					{
						case (FieldChangeType.Values):
							_genericItem.Values = field.Values;
							break;
						case (FieldChangeType.GenericItemType):
							_genericItem.ItemType = field.GenericItemType;
							if (_genericItem.ItemType == Controls.GenericItemType.Number)
							{
								((_genericItem.Control as Sport.UI.Controls.TextControl).Controller 
									as Sport.UI.Controls.NumberController).Max = 
									(field.EntityField as Sport.Data.NumberEntityField).MaxValue;
							}
							break;
						case (FieldChangeType.CanEdit):
							_genericItem.ReadOnly = !field.CanEdit;
							break;
						case (FieldChangeType.MustExist):
							_genericItem.Nullable = !field.MustExist;
							break;
					}
				}
			}

			private void FieldChanged(ViewItem source, EntityField field)
			{
				if (source != this && _field.Equals(field))
				{
					SetGenericItem();
				}
			}
		}

		#endregion

		#region GridViewItem Class

		/// <summary>
		/// GridViewItem implements a ViewItem that connects a Grid 
		/// with the entity list
		/// </summary>
		private class GridViewItem : ViewItem
		{
			#region GridSource

			public class GridSource : Sport.UI.Controls.IGridSource
			{
				private Sport.UI.Controls.Grid _grid;
				private EntityListView _view;

				public GridSource(EntityListView view)
				{
					_view = view;
				}

				#region IGridSource Members

				public void SetGrid(Sport.UI.Controls.Grid grid)
				{
					_grid = grid;
				}

				public int GetRowCount()
				{
					return _view.Count;
				}

				public int GetFieldCount(int row)
				{
					return _view.EntityType.Fields.Length;
				}

				public int GetGroup(int row)
				{
					return 0;
				}

				public string GetText(int row, int field)
				{
					string result="-שגיאה-";
					try
					{
						EntityField entityField = _view.EntityType.Fields[field];
						Entity entity = _view[row];
						result = entityField.GetText(entity);
					}
					catch (Exception e)
					{
						System.Diagnostics.Debug.WriteLine("failed to get grid text. row: "+row+" field: "+field);
						System.Diagnostics.Debug.WriteLine("error: "+e.Message);
						System.Diagnostics.Debug.WriteLine(e.StackTrace);
					}
					return result;
				}

				public Sport.UI.Controls.Style GetStyle(int row, int field, Sport.UI.Controls.GridDrawState state)
				{
					return null;
				}

				public string GetTip(int row)
				{
					return null;
				}

				public int[] GetSort(int group)
				{
					return _view.Sort;
				}

				public void Sort(int group, int[] columns)
				{
					_view.Sort = columns;
					_grid.Refresh();
				}

				private Sport.UI.Controls.GenericItem genericItem = null;

				public System.Windows.Forms.Control Edit(int row, int field)
				{
					if (genericItem != null)
					{
						_view.Items.Remove(genericItem);
						genericItem = null;
					}

					if (_view.Fields[field].CanEdit)
					{
						EntityField entityField = _view.EntityType.Fields[field];
						Entity entity = _view[row];
						
						EntityListView.Field viewField = _view.Fields[field];
						genericItem = new Sport.UI.Controls.GenericItem(
							viewField.GenericItemType, entityField.GetValueItem(entity),
							viewField.Values);
						_view.Items.Add(genericItem, field);

						return genericItem.Control;
					}
                    
					return null;
				}

				public void EditEnded(System.Windows.Forms.Control control)
				{
					if (genericItem == null || control != genericItem.Control)
						throw new ArgumentException("Given control is not edited control");
					_view.Items.Remove(genericItem);
					genericItem = null;
				}

				#endregion

				#region IDisposable Members

				public void Dispose()
				{
					// TODO:  Add GridSource.Dispose implementation
				}

				#endregion

			}

			#endregion

			private Sport.UI.Controls.Grid _grid;
			public Sport.UI.Controls.Grid Grid
			{
				get { return _grid; }
			}

			public GridViewItem(Sport.UI.Controls.Grid grid)
			{
				if (grid == null)
					throw new ArgumentNullException("grid", "Grid must have a value");

				_grid = grid;
				_grid.SelectionChanging += new Sport.UI.Controls.Grid.SelectionEventHandler(GridSelectionChanging);
				_grid.SelectionChanged += new EventHandler(GridSelectionChanged);
			}

			public override void OnOwnerChange(object oo, object no)
			{
				if (oo != null)
				{
					((EntityListView)oo).ViewItemListChanged -= new ViewItemHandler(ListChanged);
					((EntityListView)oo).ViewItemEntityChanged -= new ViewItemOperationHandler(EntityChanged);
					((EntityListView)oo).ViewItemCurrentChanged -= new ViewItemHandler(CurrentChanged);
					((EntityListView)oo).ViewItemFieldChanged -= new ViewItemFieldHandler(FieldChanged);
					_grid.Source = null;
				}
				if (no != null)
				{
					((EntityListView)no).ViewItemListChanged += new ViewItemHandler(ListChanged);
					((EntityListView)no).ViewItemEntityChanged += new ViewItemOperationHandler(EntityChanged);
					((EntityListView)no).ViewItemCurrentChanged += new ViewItemHandler(CurrentChanged);
					((EntityListView)no).ViewItemFieldChanged += new ViewItemFieldHandler(FieldChanged);
					_grid.Source = new GridSource((EntityListView) no);
				}

				base.OnOwnerChange (oo, no);
			}


			public override object Id
			{
				get { return _grid; }
			}

			public override void Remove()
			{
				_grid.SelectionChanged -= new EventHandler(GridSelectionChanged);
			}

			private void GridSelectionChanged(object sender, EventArgs e)
			{
				if (_grid.Selection.Size == 1)
				{
					((EntityListView) Owner).SetEntity(this, _grid.Selection.Rows[0]);
				}
				else
				{
					((EntityListView) Owner).SetEntity(this, -1);
				}
			}

			private void ListChanged(ViewItem source)
			{
				if (source != this)
				{
					_grid.RefreshSource();
					_grid.Refresh();
				}
			}

			private void EntityChanged(ViewItem source, EntityChangeOperation operation)
			{
				if (source == this)
					return ;
				if (operation == Sport.Data.EntityChangeOperation.Cancel ||
					operation == Sport.Data.EntityChangeOperation.Save)
					_grid.CancelEdit();
			}


			private void CurrentChanged(ViewItem source)
			{
				if (source == this)
					return ;

				int index = ((EntityListView) Owner).CurrentIndex;
				_grid.Selection.Clear();
				if (index != -1)
				{
					_grid.Selection[index] = true;
					_grid.SelectedIndex = index;
					_grid.ScrollToRow(index);
				}
				_grid.Refresh();
			}

			private void FieldChanged(ViewItem source, EntityField field)
			{
				_grid.Refresh();
			}

			private void GridSelectionChanging(object sender, Sport.UI.Controls.Grid.SelectionEventArgs e)
			{
				if ((Owner != null)&&(((EntityListView) Owner).Editing))
				{
					System.Diagnostics.Debug.WriteLine("Editing");
					if (!((EntityListView) Owner).Save().Succeeded)
						e.Continue = false;
				}
			}
		}

		#endregion

		#region EntityListViewItem Class

		/// <summary>
		/// EntityListViewItem implements a ViewItem that connects an
		/// EntityListView
		/// </summary>
		private class EntityListViewItem : ViewItem
		{
			private EntityListView _view;
			public EntityListView EntityListView
			{
				get { return _view; }
			}

			private int _idField;
			public int IdField
			{
				get { return _idField; }
			}

			public EntityListViewItem(EntityListView view, int idField)
			{
				_view = view;
				_idField = idField;
			}

			public override void OnOwnerChange(object oo, object no)
			{
				if (oo != null)
				{
					((EntityListView)oo).ViewItemCurrentChanged -= new ViewItemHandler(CurrentChanged);
				}
				if (no != null)
				{
					((EntityListView)no).ViewItemCurrentChanged += new ViewItemHandler(CurrentChanged);
				}

				base.OnOwnerChange (oo, no);
			}


			public override object Id
			{
				get { return _view; }
			}

			public override void Remove()
			{
			}

			private void CurrentChanged(ViewItem source)
			{
				if (source == this)
					return ;

				Entity entity = ((EntityListView) Owner).Current;

				if (entity == null)
				{
					_view.Clear();
				}
				else
				{
					_view.ShowWaitDialog = false;
					_view.Read(new EntityFilter(_idField, entity.Id));
					_view.ShowWaitDialog = true;
				}
			}
		}

		#endregion

		#region ViewItemCollection Class

		public class ViewItemCollection : GeneralCollection
		{
			public ViewItemCollection(EntityListView owner)
				: base(owner)
			{
			}

			protected override void RemoveItem(int index)
			{
				((ViewItem) GetItem(index)).Remove();

				base.RemoveItem (index);
			}

			private new int IndexOf(object item)
			{
				for (int i = 0; i < Count; i++)
				{
					if (((ViewItem) GetItem(i)).Id == item)
						return i;
				}
				
				return -1;
			}

			#region GenericItem

			public void Add(Sport.UI.Controls.GenericItem genericItem, int field)
			{
				Remove(genericItem);
				AddItem(new GenericItemViewItem(genericItem, ((EntityListView) Owner).EntityType.Fields[field]));
			}

			public void Add(Sport.UI.Controls.GenericItem genericItem, EntityField field)
			{
				if (!field.Type.Equals(((EntityListView) Owner).EntityType))
					throw new ArgumentException("Given field does not belong to view's entity type");

				Remove(genericItem);
				AddItem(new GenericItemViewItem(genericItem, field));
			}

			public void Remove(Sport.UI.Controls.GenericItem genericItem)
			{
				int i = IndexOf(genericItem);
				if (i != -1)
					RemoveItem(i);
			}

			public bool Contains(Sport.UI.Controls.GenericItem genericItem)
			{
				return IndexOf(genericItem) != -1;
			}

			#endregion

			#region Grid

			public void Add(Sport.UI.Controls.Grid grid)
			{
				Remove(grid);
				AddItem(new GridViewItem(grid));
			}

			public void Remove(Sport.UI.Controls.Grid grid)
			{
				int i = IndexOf(grid);
				if (i != -1)
					RemoveItem(i);
			}

			public bool Contains(Sport.UI.Controls.Grid grid)
			{
				return IndexOf(grid) != -1;
			}

			#endregion

			#region EntityListView

			public void Add(EntityListView view, int idField)
			{
				Remove(view);
				AddItem(new EntityListViewItem(view, idField));
			}

			public void Remove(EntityListView view)
			{
				int i = IndexOf(view);
				if (i != -1)
					RemoveItem(i);
			}

			public bool Contains(EntityListView view)
			{
				return IndexOf(view) != -1;
			}

			#endregion
		}

		#endregion

		private ViewItemCollection _items;
		public ViewItemCollection Items
		{
			get { return _items; }
		}

		/// <summary>
		/// SetEntity is called when the current entiy in the view
		/// changes
		/// </summary>
		private void SetEntity(ViewItem source, int index)
		{
			if (_entityList != null)
			{
				if (_entityList.CurrentIndex != index)
				{
					_entityList.CurrentIndex = index;
					OnViewItemCurrentChange(null);
				}
			}
		}

		/// <summary>
		/// SetFieldValue is called to set a value to a field
		/// of the current entity in the view
		/// </summary>
		private void SetFieldValue(ViewItem source, EntityField field, object value)
		{
			if (_entityList != null)
			{
				if (_entityList.Edit())
				{
					field.SetValue(_entityList.EntityEdit, value);
					OnViewItemFieldChange(source, field);
					OnValueChange(_entityList.EntityEdit, field);
				}
			}
		}

		#endregion

		#region View Fields

		private enum FieldChangeType
		{
			GenericItemType,
			Values,
			CanEdit,
			MustExist
		};

		private void OnFieldChange(FieldChangeType changeType, int field)
		{
			OnViewItemViewFieldChange(null, _fields[field], changeType);
		}

		public class Field
		{
			private Sport.UI.Controls.GenericItemType _genericItemType;
			public Sport.UI.Controls.GenericItemType GenericItemType
			{
				get { return _genericItemType; }
				set 
				{ 
					if (_genericItemType != value)
					{
						_genericItemType = value;
						_listView.OnFieldChange(FieldChangeType.GenericItemType, _field);
					}
				}
			}

			public void FieldChanged()
			{
				_listView.OnFieldChange(FieldChangeType.GenericItemType, _field);
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
						_listView.OnFieldChange(FieldChangeType.Values, _field);
					}
				}
			}

			private bool _canEdit;
			public bool CanEdit
			{
				get { return _canEdit; }
				set
				{
					if (_canEdit != value)
					{
						_canEdit = value;
						_listView.OnFieldChange(FieldChangeType.CanEdit, _field);
					}
				}
			}

			private bool _mustExist;
			public bool MustExist
			{
				get { return _mustExist; }
				set
				{
					if (_mustExist != value)
					{
						_mustExist = value;
						_listView.OnFieldChange(FieldChangeType.MustExist, _field);
					}
				}
			}

			private Sport.Data.EntityField _entityField;
			public Sport.Data.EntityField EntityField
			{
				get { return _entityField; }
			}

			private int _field;
			public int Index
			{
				get { return _field; }
			}

			private EntityListView _listView;
			public EntityListView EntityListView
			{
				get { return _listView; }
			}

			public Field(EntityListView listView, int field)
			{
				if (listView == null)
					throw new ArgumentNullException("listView", "EntityListView's Field must have an EntityListView");

				if (field < 0 || field >= listView.EntityType.Fields.Length)
					throw new ArgumentOutOfRangeException("field", "Given field out of EntityListView's fields range");

				_listView = listView;
				_field = field;
				_entityField = listView.EntityType.Fields[field];

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
				else
				{
					_genericItemType = Sport.UI.Controls.GenericItemType.Text;
				}

				_canEdit = _entityField.CanEdit;
				_mustExist = _entityField.MustExist;
			}
		}

		public class FieldCollection
		{
			private Field[] _fields;

			public FieldCollection(EntityListView listView)
			{
				if (listView == null)
					throw new ArgumentNullException("listView", "FieldCollection must have an EntityListView");

				_fields = new Field[listView.EntityType.Fields.Length];
				for (int f = 0; f < listView.EntityType.Fields.Length; f++)
				{
					_fields[f] = new Field(listView, f);
				}
			}

			public Field GetField(int field)
			{
				if (field < 0 ||  field >= _fields.Length)
					throw new ArgumentOutOfRangeException("field", "Field index out of range");

				return _fields[field];
			}

			public Field this[int field]
			{
				get { return GetField(field); }
			}
		}

		private FieldCollection _fields;
		public FieldCollection Fields
		{
			get { return _fields; }
		}

		#endregion

		#region Constructor

		public EntityListView(string entity)
			: this(EntityType.GetEntityType(entity))
		{
			_entityName = entity;
		}

		public EntityListView(EntityType entityType)
		{
			if (entityType == null)
				throw new ArgumentNullException("entityType", "EntityListView must have an entity type");

			_entityType = entityType;

			Clear();

			_items = new ViewItemCollection(this);
			_fields = new FieldCollection(this);
		}

		#endregion

		#region Properties

		// Stores the type of the EntityList
		private EntityType	_entityType;
		public EntityType EntityType
		{
			get { return _entityType; }
		}

		// Stores the EntityList
		private EntityList	_entityList;
		public EntityList EntityList
		{
			get { return _entityList; }
		}

		#endregion

		#region EntityList Properties
		
		/// <summary>
		/// Gets the amount of entities in the view
		/// </summary>
		public int Count
		{
			get
			{
				if (_entityList == null)
					return 0;
				int count = _entityList.Count;

				// When editing new entity, count include it
				if (_entityList.CurrentIndex == count)
					count++;

				return count;
			}
		}

		/// <summary>
		/// Gets the entity in the given index
		/// </summary>
		public Entity this[int index]
		{
			get
			{
				return _entityList == null ? null : _entityList[index];
			}
		}

		/// <summary>
		/// Gets the index of the current entity
		/// </summary>
		public int CurrentIndex
		{
			get
			{
				return _entityList == null ? -1 : _entityList.CurrentIndex;
			}
			set
			{
				if (_entityList != null)
					_entityList.CurrentIndex = value;
			}
		}

		/// <summary>
		/// Gets the current entity
		/// </summary>
		public Entity Current
		{
			get
			{
				return _entityList == null ? null : _entityList.Current;
			}
			set
			{
				if (_entityList != null)
					_entityList.Current = value;
			}
		}

		/// <summary>
		/// Gets whether the view is in edit mode
		/// </summary>
		public bool Editing
		{
			get 
			{
				return _entityList == null ? false : _entityList.Editing; 
			}
		}

		/// <summary>
		/// Gets the edited entity's EntityEdit
		/// </summary>
		public EntityEdit EntityEdit
		{
			get
			{
				return _entityList == null ? null : _entityList.EntityEdit;
			}
		}

		#endregion

		#region EntityList Setting

		/// <summary>
		/// SetEntityList is called to set the entity list in the view
		/// </summary>
		private void SetEntityList(EntityList entityList)
		{
			if (entityList == _entityList)
				return ;

			OnListChanging();

			System.Windows.Forms.Cursor c = System.Windows.Forms.Cursor.Current;
			System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;

			int id = -1;
			int index = CurrentIndex;
			if (_entityList != null)
			{
				// Storing current's id in order to reset current
				if (_entityList.Current != null)
					id = _entityList.Current.Id;

				_entityList.Dispose();
			}

			_entityList = entityList;

			if (_entityList != null)
			{
				if (id != -1)
					_entityList.Current = _entityType.Lookup(id);

				_entityList.EntityAdded += new EntityListEventHandler(EntityListChanged);
				_entityList.EntityRemoved += new EntityListEventHandler(EntityListChanged);
				_entityList.CurrentChanged += new EntityListEventHandler(EntityListCurrentChanged);
				_entityList.EntityChanged += new EntityChangeEventHandler(EntityListEntityChanged);
				_entityList.Sorting += new EventHandler(EntityListSorting);
				_entityList.Sorted += new EventHandler(EntityListSorted);
			}

			OnListChange();

			if (CurrentIndex != index)
				OnCurrentChange();

			SetSort();

			System.Windows.Forms.Cursor.Current = c;
		}

		/// <summary>
		/// show the wait dialog when reading or not
		/// </summary>
		private bool _showWaitDialog=true;
		public bool ShowWaitDialog
		{
			get { return _showWaitDialog; }
			set { _showWaitDialog = value; }
		}

		/// <summary>
		/// Read reads the entities of the given filter
		/// </summary>
		public void Read(EntityFilter filter)
		{
			if (this.ShowWaitDialog)
				Sport.UI.Dialogs.WaitForm.ShowWait("טוען נתונים אנא המתן...", true);
			
			SetEntityList(_entityType.CreateEntityList(filter));
			
			if (this.ShowWaitDialog)
				Sport.UI.Dialogs.WaitForm.HideWait();
		}

		/// <summary>
		/// Clear clears the entity list
		/// </summary>
		public void Clear()
		{
			SetEntityList(null);
		}

		#endregion

		#region Events

		// Called when the current entity in the view changes
		public event EntityEventHandler			CurrentChanged;
		// Called when the size of the list or the entire list change
		public event EventHandler				ListChanged;
		// Called before the list is set to a new list
		public event EventHandler				ListChanging;
		// Called when an entity operation occurs (edit, new, delete, save, cancel)
		public event EntityChangeEventHandler	EntityChanged;
		// Called when a field value is changed
		public event FieldEditEventHandler		ValueChanged;

		private void OnCurrentChange()
		{
			OnViewItemCurrentChange(null);

			if (CurrentChanged != null)
			{
				this.ShowWaitDialog = false;
				CurrentChanged(this, new EntityEventArgs(Current));
				this.ShowWaitDialog = true;
			}
		}

		private void OnListChange()
		{
			OnViewItemListChange(null);
			if (ListChanged != null)
				ListChanged(this, EventArgs.Empty);
		}

		private void OnListChanging()
		{
			if (ListChanging != null)
				ListChanging(this, EventArgs.Empty);
		}

		private void OnEntityChange(EntityChangeEventArgs e)
		{
			if (EntityChanged != null)
				EntityChanged(this, e);

			OnViewItemEntityChange(null, e.Operation);
		}

		// Called when a ViewItem changes the value of a field
		private void OnValueChange(EntityEdit entityEdit, EntityField entityField)
		{
			if (ValueChanged != null)
				ValueChanged(this, new FieldEditEventArgs(entityEdit, entityField));
		}

		private void OnEntityListSorting()
		{
			if (Sorting != null)
				Sorting(this, EventArgs.Empty);
		}

		private void OnEntityListSorted()
		{
			if (Sorted != null)
				Sorted(this, EventArgs.Empty);
		}

		#endregion

		#region EntityList Operations

		public bool Edit()
		{
			if (_entityList == null)
				return false;

			return _entityList.Edit();
		}

		public bool New()
		{
			if (_entityList == null)
				return false;

			return _entityList.New();
		}

		public bool Delete()
		{
			if (_entityList == null)
				return false;
			
			Sport.Data.Entity entity=_entityList.Current;
			if ((entity == null)||(entity.Id < 0))
				return false;
			
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
			
			if (objBase != null)
			{
				string strMessage=objBase.CanDelete();
				if (strMessage.Length > 0)
				{
					Sport.UI.MessageBox.Show(strMessage, "מחיקת נתונים", System.Windows.Forms.MessageBoxIcon.Warning);
					return false;
				}
			}
			
			return _entityList.Delete();
		}


		public EntityResult Save()
		{
			if (_entityList == null)
				return new EntityResult(EntityResultCode.NotEditing);

			return _entityList.Save();
		}

		public void Cancel()
		{
			if (_entityList != null)
				_entityList.Cancel();
		}

		private void EntityListCurrentChanged(object sender, EntityListEventArgs e)
		{
			OnCurrentChange();
		}

		private void EntityListChanged(object sender, EntityListEventArgs e)
		{
			OnListChange();
		}

		private void EntityListEntityChanged(object sender, EntityChangeEventArgs e)
		{
			if (e.Operation == EntityChangeOperation.New ||
				e.Operation == EntityChangeOperation.Cancel)
				OnListChange();

			OnEntityChange(e);
		}

		private void EntityListSorting(object sender, EventArgs e)
		{
			OnEntityListSorting();
		}

		private void EntityListSorted(object sender, EventArgs e)
		{
			OnEntityListSorted();
		}


		#endregion

		#region Sorting

		private int[] _sort;

		// Change the sort fields
		private void SetSort()
		{
			if (_entityList != null)
			{
				EntitySortField[] sortFields = null;
				if (_sort != null)
				{
					sortFields = new EntitySortField[_sort.Length];
					for (int n = 0; n < _sort.Length; n++)
					{
						if (_sort[n] < 0)
							sortFields[n] = new EntitySortField(_entityType.Fields[_sort[n] - Int32.MinValue], EntitySortDirection.Descending);
						else
							sortFields[n] = new EntitySortField(_entityType.Fields[_sort[n]], EntitySortDirection.Ascending);
					}

				}
				_entityList.Sort(sortFields);
			}
		}

		private void GetSort()
		{
			_sort = null;
			if (_entityList != null && _entityList.SortFields != null)
			{
				_sort = new int[_entityList.SortFields.Length];

				for (int n = 0; n < _entityList.SortFields.Length; n++)
				{
					if (_entityList.SortFields[n].Direction == EntitySortDirection.Ascending)
						_sort[n] = _entityList.SortFields[n].Field.Index;
					else
						_sort[n] = _entityList.SortFields[n].Field.Index + Int32.MinValue;
				}
			}
		}

		public event EventHandler Sorting;
		public event EventHandler Sorted;
		public event EventHandler SortChanged;

		public int[] Sort
		{
			get 
			{ 
				return _sort; 
			}
			set 
			{
				_sort = value;
				SetSort();
				if (SortChanged != null)
					SortChanged(this, EventArgs.Empty);
			}
		}

		#endregion

		public void RefreshCurrent()
		{
			OnCurrentChange();
		}

		#region IDisposable Members

		public void Dispose()
		{
			Clear();
		}

		#endregion
	}
}
