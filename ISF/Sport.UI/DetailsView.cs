using System;
using Sport.Data;
using Sport.UI.Controls;

namespace Sport.UI
{
	/// <summary>
	/// This is the details screen. All details screens should
	/// inherit this class.
	/// </summary>
	public class DetailsView : View, ICommandTarget
	{
		private System.ComponentModel.Container components = null;

		#region Properties

		protected EntityView _entityView;
		public EntityView EntityView
		{
			get { return _entityView; }
		}

		protected Sport.Data.Entity _entity;
		public Sport.Data.Entity Entity
		{
			get { return _entity; }
			set { SetEntity(value); }
		}

		#endregion

		#region Initialization

		#region Controls

		protected System.Windows.Forms.Panel panelTop;
		protected Sport.UI.Controls.GenericPanel searchBar;
		protected Sport.UI.Controls.RightTabControl tcDetailTabs;
		protected Sport.UI.Controls.ThemeButton tbSave;
		protected Sport.UI.Controls.ThemeButton tbCancel;
		protected Sport.UI.Controls.ThemeButton tbNew;
		protected Sport.UI.Controls.ThemeButton tbDelete;

		#endregion

		private void searchBar_LayoutChanged(object sender, EventArgs e)
		{
			if (searchBar.Height < tbSave.Height)
				panelTop.Height = tbSave.Height;
			else
				panelTop.Height = searchBar.Height;
		}

		#region Constructor

		public DetailsView(EntityView entityView)
		{
			_entityView = entityView;

			pages = new PageCollection(this);
			searchers = new SearcherCollection(this);

			components = new System.ComponentModel.Container();

			SuspendLayout();

			searchBar = new Sport.UI.Controls.GenericPanel();
			panelTop = new System.Windows.Forms.Panel();
			tcDetailTabs = new Sport.UI.Controls.RightTabControl();
			tbSave = new ThemeButton();
			tbCancel = new ThemeButton();
			tbNew = new ThemeButton();
			tbDelete = new ThemeButton();

			panelTop.Height = 40;
			panelTop.Dock = System.Windows.Forms.DockStyle.Top;
			panelTop.Controls.Add(tbCancel);
			panelTop.Controls.Add(tbSave);
			panelTop.Controls.Add(tbDelete);
			panelTop.Controls.Add(tbNew);
			panelTop.Controls.Add(searchBar);

			System.Windows.Forms.ImageList whiteImages = Sport.Resources.ImageLists.CreateWhiteImageList(components);

			//
			// tbSave
			//
			tbSave.AutoSize = true;
			tbSave.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			tbSave.Hue = 120F;
			tbSave.Saturation = 0.5F;
			tbSave.ImageList = whiteImages;
			tbSave.ImageIndex = (int)Sport.Resources.Images.Save;
			tbSave.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			tbSave.ImageSize = new System.Drawing.Size(10, 10);
			tbSave.Text = "שמור";
			tbSave.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Top;
			tbSave.Left = 0;
			tbSave.Click += new EventHandler(SaveClicked);

			//
			// tbCancel
			//
			tbCancel.AutoSize = true;
			tbCancel.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Top;
			tbCancel.Left = tbSave.Right;
			tbCancel.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			tbCancel.Hue = 0F;
			tbCancel.Saturation = 0.7F;
			tbCancel.ImageList = whiteImages;
			tbCancel.ImageIndex = (int)Sport.Resources.Images.Undo;
			tbCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			tbCancel.ImageSize = new System.Drawing.Size(10, 10);
			tbCancel.Text = "בטל";
			tbCancel.Click += new EventHandler(CancelClicked);

			//
			// tbNew
			//
			tbNew.AutoSize = true;
			tbNew.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			tbNew.Hue = 220F;
			tbNew.Saturation = 0.9F;
			tbNew.ImageList = whiteImages;
			tbNew.ImageIndex = (int)Sport.Resources.Images.New;
			tbNew.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			tbNew.ImageSize = new System.Drawing.Size(10, 10);
			tbNew.Text = "חדש";
			tbNew.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Top;
			tbNew.Left = 0;
			tbNew.Click += new EventHandler(NewClicked);

			//
			// tbDelete
			//
			tbDelete.AutoSize = true;
			tbDelete.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Top;
			tbDelete.Left = tbNew.Right;
			tbDelete.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			tbDelete.Hue = 0F;
			tbDelete.Saturation = 0.9F;
			tbDelete.ImageList = whiteImages;
			tbDelete.ImageIndex = (int)Sport.Resources.Images.X;
			tbDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			tbDelete.ImageSize = new System.Drawing.Size(10, 10);
			tbDelete.Text = "מחק";
			tbDelete.Click += new EventHandler(DeleteClicked);

			searchBar.Left = tbCancel.Right;
			searchBar.AutoSize = true;
			searchBar.ItemsLayout = new Sport.UI.Controls.GenericDefaultLayout();
			searchBar.Width = (panelTop.Width - (tbCancel.Width + tbSave.Width));
			searchBar.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
			searchBar.LayoutChanged += new EventHandler(searchBar_LayoutChanged);

			tcDetailTabs.Dock = System.Windows.Forms.DockStyle.Fill;

			this.Controls.Add(tcDetailTabs);
			this.Controls.Add(panelTop);

			ResumeLayout(false);

			SetControlsState();
		}

		#endregion

		#endregion

		#region DetailsView Operations


		/// <summary>
		/// Should be called whenever the searchers are changed.
		/// </summary>
		private void Search(int field, object val)
		{
			Sport.Data.EntityFilter filter = new Sport.Data.EntityFilter(field, val);
			Sport.Data.Entity[] returnedEntities = _entityView.EntityType.GetEntities(filter);
			if (returnedEntities.Length > 0)
			{
				SetEntity(returnedEntities[0]);
			}
			else
			{
				SetEntity(null);
			}
		}

		private void ChangeValue(int field, object val)
		{
			if (setting)
				return;

			if (!Editing)
			{
				Edit();
			}

			EntityEdit entityEdit = _entity as EntityEdit;
			EntityField entityField = _entityView.EntityType.Fields[field];

			entityField.SetValue(entityEdit, val);

			_entityView.OnValueChange(entityEdit, entityField);
		}

		#endregion

		#region View Operations

		#region Open

		/// <summary>
		/// This should be overriden by the children.
		/// </summary>
		public override void Open()
		{
			if (State["id"] != null)
			{
				try
				{
					SetEntity(_entityView.EntityType.Lookup(Int32.Parse(State["id"])));
				}
				catch (Exception ex)
				{
					Sport.UI.MessageBox.Warn("אזהרה: אירעה שגיאה פנימית בעת טעינת נתונים. נתוני השגיאה:\n" + ex.Message, "שגיאה כללית");
					State["id"] = null;
				}
			}
			else
			{
				SetEntity(null);
				if (searchers.Count > 0)
					searchers[0]._genericItem.Control.Focus();
			}

			ReadConfiguration();
		}

		#endregion

		#region Closing

		public override bool Closing()
		{

			if (_entity is Sport.Data.EntityEdit)
			{
				Sport.UI.MessageBoxReturnType result = Sport.UI.MessageBox.AskYesNoCancel("האם לשמור את השינויים לפני סגירת החלון?", Sport.UI.MessageBoxReturnType.Cancel);
				switch (result)
				{
					case Sport.UI.MessageBoxReturnType.Cancel:
						return false;
					case Sport.UI.MessageBoxReturnType.No:
						return true;
					case Sport.UI.MessageBoxReturnType.Yes:
						Sport.Data.EntityResult saveResult = ((Sport.Data.EntityEdit)_entity).Save();
						if (!saveResult.Succeeded)
							throw new Exception("Unable to save details! " + saveResult.ResultCode.ToString() + "\n" + saveResult.GetMessage());
						return true;
					default:
						return false;
				}
			}
			return true;
		}

		#endregion

		#endregion

		#region Entity Operations

		public bool Editing
		{
			get { return _entity is EntityEdit; }
		}

		private bool setting = false;

		private void SetEntity(Entity entity)
		{
			setting = true;
			_entity = entity;
			_entityView.OnSelectEntity(_entity);
			ReadEntity();
			SetControlsState();
			string title = "פרטי " + _entityView.Name;
			if ((_entity is EntityEdit) && ((EntityEdit)_entity).Entity == null)
			{
				title += " - חדש";
			}
			else if (_entity != null)
			{
				title += " - " + _entity.Name;
			}
			Title = title;
			setting = false;
		}

		/// <summary>
		/// Refreshes the values on the page items to the current entity
		/// </summary>
		private void ReadEntity()
		{
			foreach (Page page in Pages)
			{
				page.ReadEntity(_entity);
			}
		}

		private void Edit()
		{
			if (_entity != null)
			{
				EntityEdit entityEdit = _entity.Edit();
				_entity = entityEdit;
				_entityView.OnEditEntity(entityEdit);
				SetControlsState();
			}
		}

		private bool Save()
		{
			if (_entity is Sport.Data.EntityEdit)
			{
				Sport.Data.EntityResult result = ((Sport.Data.EntityEdit)_entity).Save();

				if (!result.Succeeded)
				{
					if (result is Sport.Data.EntityMessageResult)
					{
						Sport.UI.MessageBox.Show(((Sport.Data.EntityMessageResult)result).Message,
							System.Windows.Forms.MessageBoxIcon.Stop);
						return false;
					}
					else if (result.ResultCode == Sport.Data.EntityResultCode.FieldMustExist)
					{
						Sport.UI.MessageBox.Show("יש להכניס ערך בשדה '" + EntityView.Fields[((Sport.Data.EntityFieldResult)result).Field.Index].Name + "'",
							System.Windows.Forms.MessageBoxIcon.Information);
						return false;
					}

					Sport.UI.MessageBox.Show("שגיאה בשמירת נתונים:\n" + result.ToString());
					return false;
				}

				SetEntity(((Sport.Data.EntityEdit)_entity).Entity);

				_entityView.OnSaveEntity(_entity);
			}

			SetControlsState();

			return true;
		}

		private void Cancel()
		{
			if (_entity is Sport.Data.EntityEdit)
			{
				SetEntity(((Sport.Data.EntityEdit)_entity).Entity);
				_entityView.OnCancelEntity(_entity);
			}
		}

		private void New()
		{
			EntityEdit entityEdit = _entityView.EntityType.New();

			_entityView.OnNewEntity(entityEdit);

			SetEntity(entityEdit);
		}

		private void Delete()
		{
			if (_entity != null)
			{
				if (_entityView.OnDeleteEntity(_entity))
				{
					if (_entity.Delete())
					{
						SetEntity(null);
					}
				}
			}
		}


		#endregion

		#region Events

		#region Button Events

		private void SetControlsState()
		{
			if (_entity is EntityEdit)
			{
				tbSave.Visible = true;
				tbCancel.Visible = true;
				tbNew.Visible = false;
				tbDelete.Visible = false;
				searchBar.Enabled = false;
			}
			else
			{
				tbNew.Visible = true;
				tbDelete.Visible = true;
				tbSave.Visible = false;
				tbCancel.Visible = false;

				bool canEdit = true;
				for (int p = 0; p < pages.Count && canEdit; p++)
					canEdit = pages[p].CanEditOwner;

				tbDelete.Enabled = canEdit && _entity != null;
				tbNew.Enabled = canEdit;
				searchBar.Enabled = true;
			}
		}

		private void SaveClicked(object sender, EventArgs e)
		{
			Save();
		}

		private void CancelClicked(object sender, EventArgs e)
		{
			Cancel();
		}

		private void NewClicked(object sender, EventArgs e)
		{
			New();
		}

		private void DeleteClicked(object sender, EventArgs e)
		{
			Delete();
		}

		#endregion


		#endregion

		#region Pages

		#region Page

		public abstract class Page : Sport.Common.GeneralCollection.CollectionItem
		{
			public DetailsView View
			{
				get { return (DetailsView)Owner; }
			}

			private string _title;
			public string Title
			{
				get { return _title; }
				set
				{
					_title = value;
					if (tabPage != null)
						tabPage.Text = _title;
				}
			}

			#region Constructors

			public Page()
			{
				tabPage = null;
			}

			public Page(string title)
				: this()
			{
				_title = title;
			}

			#endregion

			internal protected abstract void ReadEntity(Entity entity);

			internal abstract void ReadConfiguration();

			private System.Windows.Forms.TabPage tabPage;

			protected virtual System.Windows.Forms.TabPage CreateTabPage()
			{
				System.Windows.Forms.TabPage tp = new System.Windows.Forms.TabPage();
				tp.Text = _title;

				return tp;
			}

			public override void OnOwnerChange(object oo, object no)
			{
				if (oo != null)
				{
					DetailsView old = (DetailsView)oo;
					old.tcDetailTabs.TabPages.Remove(tabPage);
				}

				base.OnOwnerChange(oo, no);

				if (no != null)
				{
					if (tabPage == null)
						tabPage = CreateTabPage();

					View.tcDetailTabs.TabPages.Add(tabPage);
				}
			}

			public virtual bool CanEditOwner
			{
				get { return true; }
			}
		}

		#endregion

		#region FieldsPage

		public class FieldsPage : Page
		{
			private int[] _fields;
			public int[] Fields
			{
				get { return _fields; }
				set
				{
					ClearFields();
					_fields = value;
					if (View != null)
					{
						SetGenericItems();
					}
				}
			}

			private GenericItem[] _genericItems;

			public FieldsPage()
			{
				_fields = null;
			}

			public FieldsPage(string title)
				: base(title)
			{
				_fields = null;
			}

			public FieldsPage(string title, int[] fields)
				: base(title)
			{
				_fields = fields;
			}

			public override void OnOwnerChange(object oo, object no)
			{
				if (no == null)
				{
					_genericItems = null;
				}
				else
				{
					SetGenericItems();
				}

				base.OnOwnerChange(oo, no);
			}

			private void ClearFields()
			{
				if (_fields != null)
				{
					EntityView entityView = View.EntityView;

					for (int f = 0; f < _fields.Length; f++)
					{
						entityView.Fields[_fields[f]].Changed -= new EventHandler(EntityFieldChanged);
					}
				}
			}

			private void SetGenericItems()
			{
				if (_genericItems != null)
				{
					foreach (GenericItem item in _genericItems)
					{
						item.ValueChanged -= new EventHandler(ItemValueChanged);
					}
				}


				if (_fields == null)
				{
					_fields = new int[0];
					_genericItems = new GenericItem[0];
				}
				else
				{
					_genericItems = new GenericItem[_fields.Length];

					EntityView entityView = View.EntityView;

					for (int f = 0; f < _fields.Length; f++)
					{
						EntityFieldView efv = entityView.Fields[_fields[f]];
						efv.Changed += new EventHandler(EntityFieldChanged);
						_genericItems[f] = new GenericItem(efv.Name + ":",
							efv.GenericItemType, null, efv.Values, efv.Size);
						_genericItems[f].ReadOnly = !efv.CanEdit;
						_genericItems[f].Nullable = !efv.MustExist;
						_genericItems[f].ValueChanged += new EventHandler(ItemValueChanged);
					}
				}
			}

			private bool reading = false;

			internal protected override void ReadEntity(Entity entity)
			{
				reading = true;

				if (entity == null)
				{
					for (int f = 0; f < _fields.Length; f++)
					{
						_genericItems[f].Enabled = false;
					}
				}
				else
				{
					for (int f = 0; f < _fields.Length; f++)
					{
						EntityFieldView efv = View.EntityView.Fields[_fields[f]];
						if (efv.Values != _genericItems[f].Values)
							_genericItems[f].Values = efv.Values;
						_genericItems[f].Enabled = true;
						_genericItems[f].Value = efv.EntityField.GetValueItem(entity);
					}
				}

				reading = false;
			}

			protected override System.Windows.Forms.TabPage CreateTabPage()
			{
				System.Windows.Forms.TabPage tp = base.CreateTabPage();

				GenericPanel panel = new GenericPanel();
				panel.ItemsLayout = new Sport.UI.Controls.GenericDetailsLayout();
				panel.Dock = System.Windows.Forms.DockStyle.Fill;

				foreach (GenericItem item in _genericItems)
				{
					panel.Items.Add(item);
				}

				tp.Controls.Add(panel);

				return tp;
			}

			private void ItemValueChanged(object sender, EventArgs e)
			{
				if (!reading)
				{
					int index = Array.IndexOf(_genericItems, sender);
					if (index >= 0)
					{
						View.ChangeValue(_fields[index], _genericItems[index].Value);
					}
				}
			}

			private void EntityFieldChanged(object sender, EventArgs e)
			{
				EntityFieldView efv = sender as EntityFieldView;

				int index = Array.IndexOf(_fields, efv.EntityField.Index);
				if (index >= 0)
				{
					if (_genericItems[index].ItemType != efv.GenericItemType)
						_genericItems[index].ItemType = efv.GenericItemType;
					if (_genericItems[index].Title != efv.Name + ":")
						_genericItems[index].Title = efv.Name + ":";
					if (_genericItems[index].Values != efv.Values)
						_genericItems[index].Values = efv.Values;
					if (_genericItems[index].ReadOnly != !efv.CanEdit)
						_genericItems[index].ReadOnly = !efv.CanEdit;
					if (_genericItems[index].Nullable != !efv.MustExist)
						_genericItems[index].Nullable = !efv.MustExist;
				}
			}

			internal override void ReadConfiguration()
			{

			}

		}

		#endregion

		#region EntitiesPage

		public class EntitiesPage : Page
		{
			private Entity _entity;

			private int _idField;
			public int IdField
			{
				get { return _idField; }
			}

			private EntityView _entityView;
			public EntityView EntityView
			{
				get { return _entityView; }
			}

			private EntityListView2 _listView;

			protected Sport.UI.Controls.GridControl gridControl;

			#region Table Columns

			private int[] _columns;
			public int[] Columns
			{
				get
				{
					return _columns;
				}
				set
				{
					_columns = value;
					ResetColumns();
				}
			}

			public int[] Sort
			{
				get
				{
					return _listView.Sort;
				}
				set
				{
					_listView.Sort = value;
				}
			}

			private void GridColumnMoved(object sender, Sport.UI.Controls.Grid.ColumnMoveEventArgs e)
			{
				int f = _columns[e.Column1];
				_columns[e.Column1] = _columns[e.Column2];
				_columns[e.Column2] = f;
				f = _columnsWidth[e.Column1];
				_columnsWidth[e.Column1] = _columnsWidth[e.Column2];
				_columnsWidth[e.Column2] = f;

				SaveConfiguration();
			}

			private void GridColumnResized(object sender, Sport.UI.Controls.Grid.ColumnEventArgs e)
			{
				RebuildColumnsWidth();

				SaveConfiguration();
			}

			private void GridHeaderResized(object sender, EventArgs e)
			{
				SaveConfiguration();
			}

			private void GridRowResized(object sender, Sport.UI.Controls.Grid.GroupEventArgs e)
			{
				SaveConfiguration();
			}

			private void ResetColumnsWidth()
			{
				if (_columnsWidth != null && gridControl.Grid.Columns.Count == _columnsWidth.Length)
				{
					gridControl.Grid.SetColumnsWidth(0, _columnsWidth);

					gridControl.Grid.Refresh();
				}
			}

			private void ListViewSorted(object sender, EventArgs e)
			{
				SaveConfiguration();
			}

			private void ResetColumns()
			{
				gridControl.Grid.Columns.Clear();

				if (_columns == null)
					return;

				for (int n = 0; n < _columns.Length; n++)
				{
					EntityFieldView efv = _entityView.Fields[_columns[n]];
					gridControl.Grid.Columns.Add(_columns[n], efv.Name, efv.Width);
				}

				gridControl.Grid.Refresh();

				RebuildColumnsWidth();
			}

			private int[] _columnsWidth;
			public int[] ColumnsWidth
			{
				get
				{
					if (_columnsWidth == null)
						RebuildColumnsWidth();
					return _columnsWidth;
				}
				set
				{
					_columnsWidth = value;
					ResetColumnsWidth();
				}
			}

			private void RebuildColumnsWidth()
			{
				_columnsWidth = new int[gridControl.Grid.Columns.Count];
				for (int n = 0; n < gridControl.Grid.Columns.Count; n++)
				{
					_columnsWidth[n] = gridControl.Grid.GetColumnWidth(0, n);
				}
			}

			private void ResetSort()
			{
			}

			#endregion

			public EntitiesPage(EntityView entityView, int idField)
				: base(entityView.PluralName)
			{
				_idField = idField;
				_entityView = entityView;

				_canInsert = true;
				_canDelete = true;

				_listView = new EntityListView2(entityView);
				_listView.ListChanged += new EventHandler(ViewListChanged);
				_listView.CurrentChanged += new Sport.Data.EntityEventHandler(ViewCurrentChanged);
				_listView.EntityChanged += new Sport.Data.EntityChangeEventHandler(ViewEntityChanged);
				_listView.Sorted += new EventHandler(ListViewSorted);

				_listView.EntityQuery.Parameters.Add(_idField);

				// Creating the grid control
				gridControl = new GridControl();
				gridControl.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;

				gridControl.Grid.Source = _listView;
				gridControl.Grid.SelectionChanging += new Sport.UI.Controls.Grid.SelectionEventHandler(GridSelectionChanging);
				gridControl.Grid.SelectionChanged += new EventHandler(GridSelectionChanged);

				int i = gridControl.Buttons.Add(Sport.Resources.Images.Plus, "חדש");
				gridControl.Buttons[i].Enabled = false;
				gridControl.Buttons[i].Click += new EventHandler(NewClicked);
				i = gridControl.Buttons.Add(Sport.Resources.Images.Minus, "מחק");
				gridControl.Buttons[i].Enabled = false;
				gridControl.Buttons[i].Click += new EventHandler(DeleteClicked);
				i = gridControl.Buttons.Add(Sport.Resources.Images.Save, "שמור");
				gridControl.Buttons[i].Enabled = false;
				gridControl.Buttons[i].Click += new EventHandler(SaveClicked);
				i = gridControl.Buttons.Add(Sport.Resources.Images.Undo, "בטל");
				gridControl.Buttons[i].Enabled = false;
				gridControl.Buttons[i].Click += new EventHandler(CancelClicked);

				gridControl.Grid.ColumnMoved += new Sport.UI.Controls.Grid.ColumnMoveEventHandler(GridColumnMoved);
				gridControl.Grid.ColumnResized += new Sport.UI.Controls.Grid.ColumnEventHandler(GridColumnResized);
				gridControl.Grid.HeaderResized += new EventHandler(GridHeaderResized);
				gridControl.Grid.RowResized += new Sport.UI.Controls.Grid.GroupEventHandler(GridRowResized);


				gridControl.Anchor = System.Windows.Forms.AnchorStyles.Left |
					System.Windows.Forms.AnchorStyles.Top |
					System.Windows.Forms.AnchorStyles.Right |
					System.Windows.Forms.AnchorStyles.Bottom;
			}

			private bool _canInsert;
			public bool CanInsert
			{
				get { return _canInsert; }
				set
				{
					_canInsert = value;
					ResetButtonState();
				}
			}

			private bool _canDelete;
			public bool CanDelete
			{
				get { return _canDelete; }
				set
				{
					_canDelete = value;
					ResetButtonState();
				}
			}

			internal protected override void ReadEntity(Entity entity)
			{
				_entity = entity;

				if (_entity == null)
					_listView.EntityQuery.Parameters[0].Value = null;
				else
					_listView.EntityQuery.Parameters[0].Value = _entity.Id;

				gridControl.Grid.RefreshSource();

				ResetButtonState();
			}

			protected override System.Windows.Forms.TabPage CreateTabPage()
			{
				System.Windows.Forms.TabPage tp = base.CreateTabPage();

				gridControl.Bounds = new System.Drawing.Rectangle(10, 10, tp.Bounds.Width - 20,
					tp.Bounds.Height - 20);

				tp.Controls.Add(gridControl);

				return tp;
			}

			#region Entity Operations

			private bool Save()
			{
				gridControl.Grid.CancelEdit();

				Sport.Data.EntityResult result = _listView.Save();

				if (result.Succeeded)
					return true;

				if (result is Sport.Data.EntityMessageResult)
				{
					Sport.UI.MessageBox.Show(((Sport.Data.EntityMessageResult)result).Message,
						System.Windows.Forms.MessageBoxIcon.Stop);
					return false;
				}
				else if (result == Sport.Data.EntityResultCode.FieldMustExist)
				{
					Sport.UI.MessageBox.Show("יש להכניס ערך בשדה '" + EntityView.Fields[((Sport.Data.EntityFieldResult)result).Field.Index].Name + "'",
						System.Windows.Forms.MessageBoxIcon.Information);
					return false;
				}

				Sport.UI.MessageBox.Show("שגיאה בשמירת נתונים:\n" + result.ToString());
				return false;
			}

			private void Cancel()
			{
				_listView.Cancel();
			}

			public event EventHandler NewClick;
			public event EntityEventHandler DeleteClick;

			private void NewClicked(object sender, EventArgs e)
			{
				if (NewClick != null)
				{
					NewClick(this, EventArgs.Empty);
					gridControl.Grid.RefreshSource();
				}
			}

			private void DeleteClicked(object sender, EventArgs e)
			{
				Entity current = _listView.Current;
				if (DeleteClick != null && current != null)
				{
					DeleteClick(this, new EntityEventArgs(current));
					gridControl.Grid.RefreshSource();
					ResetListSelection();
				}
				//_listView.Delete();
			}

			private void SaveClicked(object sender, EventArgs e)
			{
				Save();
			}

			private void CancelClicked(object sender, EventArgs e)
			{
				Cancel();
			}

			#endregion

			public override bool CanEditOwner
			{
				get { return !_listView.Editing; }
			}


			private void ResetButtonState()
			{
				gridControl.Enabled = _listView.EntityList != null;
				if (!gridControl.Grid.Editable || _entity == null)
				{
					// Plus button
					gridControl.Buttons[0].Enabled = false;
					// Minus button
					gridControl.Buttons[1].Enabled = false;
					// Save button
					gridControl.Buttons[2].Enabled = false;
					// Undo button
					gridControl.Buttons[3].Enabled = false;
				}
				else
				{
					// Plus button
					gridControl.Buttons[0].Enabled = _canInsert && NewClick != null && !_listView.Editing && gridControl.Enabled;
					// Minus button
					gridControl.Buttons[1].Enabled = _canDelete && DeleteClick != null && !_listView.Editing && (_listView.Current != null);
					// Save button
					gridControl.Buttons[2].Enabled = _listView.Editing;
					// Undo button
					gridControl.Buttons[3].Enabled = _listView.Editing;
				}
			}

			#region View Events

			private void ViewListChanged(object sender, EventArgs e)
			{
				//Sport.UI.MessageBox.Show("refreshing source...");
				gridControl.Grid.RefreshSource();
				//Sport.UI.MessageBox.Show("reset button state...");
				ResetButtonState();
				//Sport.UI.MessageBox.Show("done!");
			}

			private void ViewEntityChanged(object sender, EntityChangeEventArgs e)
			{
				ResetButtonState();

				View.SetControlsState();
			}

			private void ViewCurrentChanged(object sender, EntityEventArgs e)
			{
				ResetButtonState();
			}

			#endregion

			private void GridSelectionChanging(object sender, Sport.UI.Controls.Grid.SelectionEventArgs e)
			{
				if (_listView.Editing)
				{
					if (!Save())
						e.Continue = false;
				}
			}

			private void ResetListSelection()
			{
				int rows = gridControl.Grid.Selection.Size;
				if (rows == 1 && gridControl.Grid.Selection.Rows[0] < _listView.Count)
				{
					_listView.CurrentIndex = gridControl.Grid.Selection.Rows[0];
				}
				else
				{
					_listView.CurrentIndex = -1;
				}
			}

			private void GridSelectionChanged(object sender, EventArgs e)
			{
				ResetListSelection();
			}

			private void SaveConfiguration()
			{
				if (configurationRead)
				{
					Sport.Core.Configuration view = View.GetConfiguration();
					Sport.Core.Configuration page = view.GetConfiguration("EntitiesPage", _entityView.GetType().Name);
					Sport.Core.Configuration columns = page.GetConfiguration("Columns");
					columns.Value = Sport.Common.Tools.ArrayToString(Columns, ",");
					Sport.Core.Configuration columnsWidth = page.GetConfiguration("ColumnsWidth");
					columnsWidth.Value = Sport.Common.Tools.ArrayToString(ColumnsWidth, ",");
					Sport.Core.Configuration headerHeight = page.GetConfiguration("HeaderHeight");
					headerHeight.Value = gridControl.Grid.HeaderHeight.ToString();
					Sport.Core.Configuration rowHeight = page.GetConfiguration("RowHeight");
					rowHeight.Value = gridControl.Grid.Groups[0].RowHeight.ToString();
					Sport.Core.Configuration sort = page.GetConfiguration("Sort");
					sort.Value = Sport.Common.Tools.ArrayToString(Sort, ",");
					Sport.Core.Configuration.Save();
				}
			}

			private bool configurationRead = false;

			internal override void ReadConfiguration()
			{
				Sport.Core.Configuration view = View.GetConfiguration();
				Sport.Core.Configuration page = view.GetConfiguration("EntitiesPage", _entityView.GetType().Name);
				Sport.Core.Configuration columns = page.GetConfiguration("Columns");
				int[] arr = Sport.Common.Tools.ToIntArray(columns.Value, ',');
				if (arr != null)
					Columns = arr;
				Sport.Core.Configuration columnsWidth = page.GetConfiguration("ColumnsWidth");
				arr = Sport.Common.Tools.ToIntArray(columnsWidth.Value, ',');
				if (arr != null)
					ColumnsWidth = arr;
				Sport.Core.Configuration headerHeight = page.GetConfiguration("HeaderHeight");
				int val;
				if (Sport.Common.Tools.ToInt(headerHeight.Value, out val))
					gridControl.Grid.HeaderHeight = val;
				Sport.Core.Configuration rowHeight = page.GetConfiguration("RowHeight");
				if (Sport.Common.Tools.ToInt(rowHeight.Value, out val))
					gridControl.Grid.Groups[0].RowHeight = val;
				Sport.Core.Configuration sort = page.GetConfiguration("Sort");
				arr = Sport.Common.Tools.ToIntArray(sort.Value, ',');
				if (arr != null)
					Sort = arr;

				configurationRead = true;
			}
		}

		#endregion

		#region PageCollection

		public class PageCollection : Sport.Common.GeneralCollection
		{
			public PageCollection(DetailsView owner)
				: base(owner)
			{
			}

			public Page this[int page]
			{
				get { return (Page)GetItem(page); }
				set { SetItem(page, value); }
			}

			public void Insert(int index, Page value)
			{
				InsertItem(index, value);
			}

			public void Remove(Page value)
			{
				RemoveItem(value);
			}

			public bool Contains(Page value)
			{
				return base.Contains(value);
			}

			public int IndexOf(Page value)
			{
				return base.IndexOf(value);
			}

			public int Add(Page value)
			{
				return AddItem(value);
			}
		}

		#endregion

		private PageCollection pages;
		public PageCollection Pages
		{
			get { return pages; }
		}

		#endregion

		#region Configuration

		private void ReadConfiguration()
		{
			foreach (Page page in pages)
			{
				page.ReadConfiguration();
			}
		}

		#endregion

		#region Seachers

		#region Searcher Class

		public class Searcher : Sport.Common.GeneralCollection.CollectionItem
		{
			internal Sport.Data.EntityField _field;
			internal Sport.UI.Controls.GenericItem _genericItem;

			// The text of the label of the searcher
			public string Title
			{
				get { return _genericItem.Title; }
				set { _genericItem.Title = value; }
			}

			// The EntityField used in search
			public Sport.Data.EntityField Field
			{
				get { return _field; }
				set { _field = value; }
			}

			#region Constructors

			// Construct the searcher with the given title, 
			// search field and given width
			public Searcher(string title, Sport.Data.EntityField field, int width)
			{
				if (field == null)
					throw new ArgumentNullException("field", "Field must not be null");

				_field = field;

				_genericItem = new Sport.UI.Controls.GenericItem(title,
					Sport.UI.Controls.GenericItemType.Text, new System.Drawing.Size(width, 0));
				Sport.UI.Controls.TextControl tc = (Sport.UI.Controls.TextControl)_genericItem.Control;
				tc.KeyDown += new System.Windows.Forms.KeyEventHandler(TextKeyDown);
				tc.GotFocus += new EventHandler(TextGotFocus);
			}

			// Construct the searcher with the given caption, 
			// search field
			public Searcher(string title, Sport.Data.EntityField field)
				: this(title, field, 0)
			{
			}

			#endregion

			private void TextKeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
			{
				if (e.KeyCode == System.Windows.Forms.Keys.Enter)
				{
					int index = _field.Index;
					object value = _genericItem.Value;
					if (value == null)
						return;
					string strValue = value.ToString();
					if (strValue.Length == 0)
						return;
					bool blnSearch = true;
					if (Sport.Common.Tools.IsInteger(strValue))
					{
						string strMax = Int32.MaxValue.ToString();
						if (strValue.Length > strMax.Length)
						{
							blnSearch = false;
						}
						else
						{
							if (Double.Parse(strValue) > Int32.MaxValue)
								blnSearch = false;
						}
					}
					if (blnSearch)
					{
						try
						{
							((DetailsView)Owner).Search(index, _field.ConvertValue(value));
						}
						catch (Exception ex)
						{
							System.Diagnostics.Debug.WriteLine("error while searching for " + strValue + ": " + ex.Message);
							System.Diagnostics.Debug.WriteLine(ex.StackTrace);
							Sport.UI.MessageBox.Error("שגיאה בעת חיפוש, אנא בדוק חוקיות הערך", "חיפוש מידע");
						}
					}

					Sport.UI.Controls.TextControl tc = (Sport.UI.Controls.TextControl)sender;
					tc.SelectionStart = 0;
					tc.SelectionLength = tc.Text.Length;
				}
			}

			private void TextGotFocus(object sender, EventArgs e)
			{
				Sport.UI.Controls.TextControl tc = (Sport.UI.Controls.TextControl)sender;
				tc.SelectionStart = 0;
				tc.SelectionLength = tc.Text.Length;
			}

			public override void OnOwnerChange(object oo, object no)
			{
				if (oo != null)
				{
					((DetailsView)oo).searchBar.Items.Remove(_genericItem);
				}

				base.OnOwnerChange(oo, no);

				if (no != null)
				{
					((DetailsView)no).searchBar.Items.Add(_genericItem);
				}
			}
		}

		#endregion

		#region SeacherCollection Class

		public class SearcherCollection : Sport.Common.GeneralCollection
		{
			public SearcherCollection(DetailsView owner)
				: base(owner)
			{
			}

			public Searcher this[int searcher]
			{
				get { return (Searcher)GetItem(searcher); }
				set { SetItem(searcher, value); }
			}

			public void Insert(int index, Searcher value)
			{
				InsertItem(index, value);
			}

			public void Remove(Searcher value)
			{
				RemoveItem(value);
			}

			public bool Contains(Searcher value)
			{
				return base.Contains(value);
			}

			public int IndexOf(Searcher value)
			{
				return base.IndexOf(value);
			}

			public int Add(Searcher value)
			{
				return AddItem(value);
			}
		}

		#endregion

		private SearcherCollection searchers;
		public SearcherCollection Searchers
		{
			get { return searchers; }
		}

		#endregion

		#region Dispose

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#endregion

		#region ICommandTarget Members

		public bool ExecuteCommand(string command)
		{
			if (command == Sport.Core.Data.KeyCommands.EditItem)
			{
				Edit();
			}
			else if (command == Sport.Core.Data.KeyCommands.SearchTable)
			{
				if (searchBar.Visible)
				{
					int current = -1;
					for (int n = 0; n < searchers.Count && current == -1; n++)
					{
						if (searchers[n]._genericItem.Control.ContainsFocus)
							current = n;
					}
					if (current == searchers.Count - 1)
						current = 0;
					else
						current++;

					searchers[current]._genericItem.Control.Focus();
				}
			}
			else if (command == Sport.Core.Data.KeyCommands.DeleteItem)
			{
				if (!Editing)
					Delete();
			}
			else if (command == Sport.Core.Data.KeyCommands.NewItem)
			{
				if (!Editing)
					New();
			}
			else if (command == Sport.Core.Data.KeyCommands.SaveItem)
			{
				if (Editing)
					Save();
			}
			else if (command == Sport.Core.Data.KeyCommands.Cancel)
			{
				if (Editing)
					Cancel();
				else
					return false;
			}
			else if (command == Sport.Core.Data.KeyCommands.NextView)
			{
				if (tcDetailTabs.TabPages.Count > 0)
				{
					if (tcDetailTabs.TabIndex == tcDetailTabs.TabPages.Count - 1)
						tcDetailTabs.TabIndex = 0;
					else
						tcDetailTabs.TabIndex++;
				}
			}
			else if (command == Sport.Core.Data.KeyCommands.PreviousView)
			{
				if (tcDetailTabs.TabPages.Count > 0)
				{
					if (tcDetailTabs.TabIndex == 0)
						tcDetailTabs.TabIndex = tcDetailTabs.TabPages.Count - 1;
					else
						tcDetailTabs.TabIndex--;
				}
			}
			else
			{
				return false;
			}

			return true;
		}

		#endregion
	}
}
