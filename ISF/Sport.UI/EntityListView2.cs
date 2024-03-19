using System;
using Sport.Data;

namespace Sport.UI
{
	/// <summary>
	/// EntityListView handles display of an EntityList using an EntityView
	/// </summary>
	public class EntityListView2 : IDisposable, Sport.UI.Controls.IGridSource
	{
		#region Constructor

		public EntityListView2(EntityView entityView)
		{
			if (entityView == null)
				throw new ArgumentNullException("entityView", "EntityListView must have an entity view");

			_entityView = entityView;

			_entityQuery = new EntityQuery(entityView.EntityType);
			_entityQuery.Changed += new EventHandler(QueryChanged);
			_autoQuery = true;

			SetEntityList(null);
		}

		#endregion

		#region Properties

		// Stores the entity view
		private EntityView _entityView;
		public EntityView EntityView
		{
			get { return _entityView; }
		}

		// Returns the entity type
		public EntityType EntityType
		{
			get { return _entityView.EntityType; }
		}

		// Stores the EntityList
		private EntityList	_entityList;
		public EntityList EntityList
		{
			get { return _entityList; }
		}

		// Stores the EntityQuery
		private EntityQuery _entityQuery;
		public EntityQuery EntityQuery
		{
			get { return _entityQuery; }
		}

		// Returns the query's parameters
		public EntityQuery.ParameterCollection Parameters
		{
			get { return _entityQuery.Parameters; }
		}

		private bool _autoQuery;
		public bool AutoQuery
		{
			get { return _autoQuery; }
			set { _autoQuery = value; }
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
				//string strLogPath="D:\\Sportsman\\FinalCheck.log";
				//Sport.Core.LogFiles.AppendToLogFile(strLogPath, "getting Count of EntityListView2...");
				if (_entityList == null)
					return 0;
				//Sport.Core.LogFiles.AppendToLogFile(strLogPath, "calling Count of EntityList...");
				int count = _entityList.Count;
				//Sport.Core.LogFiles.AppendToLogFile(strLogPath, "count: "+count);

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
			
			System.Windows.Forms.Cursor c = System.Windows.Forms.Cursor.Current;
			System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
			
			int id = -1;
			int index = CurrentIndex;
			
			if (_entityList != null)
			{
				// Storing current's id in order to reset current
				if (_entityList.Current != null)
					id = _entityList.Current.Id;

				_entityList.EntityAdded -= new EntityListEventHandler(EntityListChanged);
				_entityList.EntityRemoved -= new EntityListEventHandler(EntityListChanged);
				_entityList.CurrentChanged -= new EntityListEventHandler(EntityListCurrentChanged);
				_entityList.EntityChanged -= new EntityChangeEventHandler(EntityListEntityChanged);
				_entityList.Sorting -= new EventHandler(EntityListSorting);
				_entityList.Sorted -= new EventHandler(EntityListSorted);
				
				_entityList.Dispose();
			}
			
			_entityList = entityList;

			if (_entityList != null)
			{
				if (id != -1)
					_entityList.Current = EntityType.Lookup(id);
				
				_entityList.EntityAdded += new EntityListEventHandler(EntityListChanged);
				_entityList.EntityRemoved += new EntityListEventHandler(EntityListChanged);
				_entityList.CurrentChanged += new EntityListEventHandler(EntityListCurrentChanged);
				_entityList.EntityChanged += new EntityChangeEventHandler(EntityListEntityChanged);
				_entityList.Sorting += new EventHandler(EntityListSorting);
				_entityList.Sorted += new EventHandler(EntityListSorted);
			}
			
			OnListChange();
			
			//Sport.UI.MessageBox.Show("current change..");
			
			if (CurrentIndex != index)
				OnCurrentChange(Current);
			
			//Sport.UI.MessageBox.Show("setting sort..");
			SetSort();
			
			System.Windows.Forms.Cursor.Current = c;
			
			//Sport.UI.MessageBox.Show("done!");
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
		public void Requery()
		{
			//Sport.UI.MessageBox.Show("reading 2");
			if (this.ShowWaitDialog)
				Sport.UI.Dialogs.WaitForm.ShowWait("טוען נתונים אנא המתן...");
			
			SetEntityList(_entityQuery.GetEntityList());
			
			if (this.ShowWaitDialog)
				Sport.UI.Dialogs.WaitForm.HideWait();
		}

		#endregion

		#region Events

		// Called when the current entity in the view changes
		public event EntityEventHandler			CurrentChanged;
		// Called when the size of the list or the entire list change
		public event EventHandler				ListChanged;
		// Called when an entity operation occurs (edit, new, delete, save, cancel)
		public event EntityChangeEventHandler	EntityChanged;
		// Called when a field value is changed
		public event FieldEditEventHandler		ValueChanged;

		private void OnCurrentChange(Entity entity)
		{
			_entityView.OnSelectEntity(entity);

			if (CurrentChanged != null)
			{
				this.ShowWaitDialog = false;
				CurrentChanged(this, new EntityEventArgs(Current));
				this.ShowWaitDialog = true;
			}
		}

		private void OnListChange()
		{
			if (ListChanged != null)
				ListChanged(this, EventArgs.Empty);
		}

		private void OnEntityChange(EntityChangeEventArgs e)
		{
			switch (e.Operation)
			{
				case (EntityChangeOperation.New):
					_entityView.OnNewEntity(e.Entity as EntityEdit);
					break;
				case (EntityChangeOperation.Edit):
					_entityView.OnEditEntity(e.Entity as EntityEdit);
					break;
				case (EntityChangeOperation.Save):
					_entityView.OnSaveEntity(e.Entity);
					break;
				case (EntityChangeOperation.Cancel):
					_entityView.OnCancelEntity(e.Entity);
					break;
			}

			if (EntityChanged != null)
				EntityChanged(this, e);
		}

		// Called when a ViewItem changes the value of a field
		//private void OnValueChange(EntityEdit entityEdit, EntityField entityField)
		//{
		//	_entityView.OnValueChange(entityEdit, entityField);

//			if (ValueChanged != null)
				//ValueChanged(this, new FieldEditEventArgs(entityEdit, entityField));
		//}

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
			OnCurrentChange(e.Entity);
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
			GetSort();
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
							sortFields[n] = new EntitySortField(EntityType.Fields[_sort[n] - Int32.MinValue], EntitySortDirection.Descending);
						else
							sortFields[n] = new EntitySortField(EntityType.Fields[_sort[n]], EntitySortDirection.Ascending);
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
			OnCurrentChange(Current);
		}

		#region IDisposable Members

		public void Dispose()
		{
			SetEntityList(null);
		}

		#endregion

		#region IGridSource Members

		public void SetGrid(Sport.UI.Controls.Grid grid)
		{
		}

		public int GetRowCount()
		{
			return Count;
		}

		public int GetFieldCount(int row)
		{
			return EntityType.Fields.Length;
		}

		public int GetGroup(int row)
		{
			return 0;
		}

		public string GetText(int row, int field)
		{
			return _entityView.GetText(_entityList[row], field);
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
			return Sort;
		}

		void Sport.UI.Controls.IGridSource.Sort(int group, int[] columns)
		{
			Sort = columns;
		}

		private Sport.UI.Controls.GenericItem gridEditGenericItem = null;

		public System.Windows.Forms.Control Edit(int row, int field)
		{
			if (gridEditGenericItem != null)
			{
				gridEditGenericItem.ValueChanged -= new EventHandler(GridEditItemValueChanged);
			}

			EntityFieldView efv = _entityView.Fields[field];
			if (!efv.CanEdit)
				return null;

			object genericItemValue = null;
			try
			{
				genericItemValue = efv.EntityField.GetValueItem(_entityList[row]);
			}
			catch
			{
				//sometimes it crash when season is incorrect
			}
			gridEditGenericItem = new Sport.UI.Controls.GenericItem(efv.GenericItemType, 
				genericItemValue, efv.Values, efv.Size);
			gridEditGenericItem.Tag = field;
			gridEditGenericItem.Nullable = !efv.MustExist;
			gridEditGenericItem.ValueChanged += new EventHandler(GridEditItemValueChanged);

			return gridEditGenericItem.Control;
		}

		private void GridEditItemValueChanged(object sender, EventArgs e)
		{
			ChangeValue(gridEditGenericItem, (int) gridEditGenericItem.Tag, gridEditGenericItem.Value);
		}

		public void EditEnded(System.Windows.Forms.Control control)
		{
			if (gridEditGenericItem == null || control != gridEditGenericItem.Control)
				throw new ArgumentException("Given control is not edited control");
			gridEditGenericItem.ValueChanged -= new EventHandler(GridEditItemValueChanged);
			gridEditGenericItem = null;
		}

		#endregion

		/// <summary>
		/// ChangeValue is called to set a value to a field
		/// of the current entity in the view
		/// </summary>
		public void ChangeValue(object changer, int field, object value)
		{
			if (_entityList != null)
			{
				if (_entityList.Edit())
				{
					EntityField entityField = _entityView.Fields[field].EntityField;
					entityField.SetValue(_entityList.EntityEdit, value);

					if (gridEditGenericItem != null && changer != gridEditGenericItem &&
						(int) gridEditGenericItem.Tag == field)
					{
						gridEditGenericItem.ValueChanged -= new EventHandler(GridEditItemValueChanged);
						gridEditGenericItem.Value = value;
						gridEditGenericItem.ValueChanged += new EventHandler(GridEditItemValueChanged);
					}

					if (ValueChanged != null)
						ValueChanged(changer, new FieldEditEventArgs(EntityEdit, entityField));

					//OnValueChange(_entityList.EntityEdit, field);
				}
			}
		}

		private void QueryChanged(object sender, EventArgs e)
		{
			if (_autoQuery)
			{
				Requery();
			}
		}
	}
}
