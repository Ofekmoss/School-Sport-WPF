using System;
using Sport.Common;
using System.Windows.Forms;
using Sport.UI.Display;
using Sport.UI.Controls;

namespace Sport.UI
{
	/// <summary>
	/// TableView inherits View to implement a view that contains
	/// a grid and a filter, search and details bar.
	/// The TableView uses an EntityListView instance to display and
	/// edit the entities in an EntityList.
	/// Table based views will inherit from the TableView and will
	/// set the EntityListView and the filter, search and 
	/// details collections implemented by TableView.
	/// </summary>
	public class TableView : Sport.UI.View, Sport.UI.Controls.IGridSource, 
		ICommandTarget, ITableView
	{
		private System.ComponentModel.IContainer components = null;

		#region Control Bars General

		#region States

		// State and Enable members
		private bool filterBarState = true;
		private bool toolBarState = true;
		private bool detailsBarState = true;
		private bool searchBarState = true;

		private bool filterBarEnabled = true;
		private bool toolBarEnabled = true;
		private bool detailsBarEnabled = true;
		private bool searchBarEnabled = false;

		private bool canInsert=true;
		private bool canDelete = true;

		#endregion

		#region Controls

		// Bar Controls
		private Sport.UI.Controls.GenericPanel	filterBar;
		protected Sport.UI.Controls.RightToolBar	toolBar;
		private System.Windows.Forms.Panel		detailsBar;
		private Sport.UI.Controls.GenericPanel	searchBar;

		#endregion

		#region Bar Reset

		private void SetFilterBar()
		{
			StateItem si = StateManager.States["filterbar"];
			filterBar.Visible = si.Checked;
			if (si.Visible)
			{
				tbbFilter.Pushed = filterBar.Visible;
				tbbFilter.Visible = true;
			}
			else
			{
				tbbFilter.Visible = false;
			}
		}

		private void SetToolBar()
		{
			toolBar.Visible = StateManager.States["toolbar"].Checked;
		}

		private void SetDetailsBar()
		{
			detailsBar.Visible = StateManager.States["detailsbar"].Checked;
		}

		private void SetSearchBar()
		{
			StateItem si = StateManager.States["searchbar"];
			searchBar.Visible = si.Checked;
			if (si.Visible)
			{
				tbbSearch.Pushed = searchBar.Visible;
				tbbSearch.Visible = true;
			}
			else
			{
				tbbSearch.Visible = false;
			}
		}

		#endregion

		#region Enabled Properties

		public bool FilterBarEnabled
		{
			get { return filterBarEnabled; }
			set
			{
				filterBarEnabled = value;

				if (Status == ViewStatus.Activated)
				{
					StateItem si = StateManager.States["toolbar"];

					si.Visible = filterBarEnabled;
					si.Checked = filterBarState && filterBarEnabled;
				}
			}
		}

		public bool ToolBarEnabled
		{
			get { return toolBarEnabled; }
			set
			{
				toolBarEnabled = value;

				if (Status == ViewStatus.Activated)
				{
					StateItem si = StateManager.States["toolbar"];

					si.Visible = toolBarEnabled;
					si.Checked = toolBarState && toolBarEnabled;
				}
			}
		}

		public bool DetailsBarEnabled
		{
			get { return detailsBarEnabled; }
			set
			{
				detailsBarEnabled = value;

				if (Status == ViewStatus.Activated)
				{
					StateItem si = StateManager.States["detailsbar"];

					si.Visible = detailsBarEnabled;
					si.Checked = detailsBarState && detailsBarEnabled;
				}
			}
		}

		public bool SearchBarEnabled
		{
			get { return searchBarEnabled; }
			set
			{
				searchBarEnabled = value;

				if (Status == ViewStatus.Activated)
				{
					StateItem si = StateManager.States["searchbar"];

					si.Visible = searchBarEnabled;
					si.Checked = searchBarState && searchBarEnabled;
				}
			}
		}

		private bool moreButtonEnabled = true;
		public bool MoreButtonEnabled
		{
			get { return moreButtonEnabled; }
			set
			{
				if (moreButtonEnabled != value)
				{
					moreButtonEnabled = value;
					tbMore.Visible = moreButtonEnabled;

					if (moreButtonEnabled)
					{
						tbSave.Left = tbMore.Right + 2;
					}
					else
					{
						tbSave.Left = 4;
					}

					tbCancel.Left = tbSave.Right + 2;
				}
			}
		}

		public bool CanInsert
		{
			get {return canInsert;}
			set
			{
				canInsert = value;
				tbbNew.Visible = canInsert;
			}
		}

		public bool CanDelete
		{
			get { return canDelete; }
			set
			{
				canDelete = value;
				tbbDelete.Visible = canDelete;
			}
		}

		#endregion

		#region State Change Events

		private void FilterBarStateChanged(object sender, System.EventArgs e)
		{
			SetFilterBar();
		}

		private void ToolBarStateChanged(object sender, System.EventArgs e)
		{
			SetToolBar();
		}

		private void DetailsBarStateChanged(object sender, System.EventArgs e)
		{
			SetDetailsBar();
		}

		private void SearchBarStateChanged(object sender, System.EventArgs e)
		{
			SetSearchBar();
		}

		#endregion

		#endregion

		#region Tool Bar

		private System.Windows.Forms.ToolBarButton tbbNew;
		private System.Windows.Forms.ToolBarButton tbbDelete;
		private System.Windows.Forms.ToolBarButton tbbFilter;
		private System.Windows.Forms.ToolBarButton tbbSearch;
		private System.Windows.Forms.ToolBarButton tbbCustom;
		private System.Windows.Forms.ToolBarButton tbbPrint;

		private void ToolBarButtonClicked(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			if (e.Button == tbbFilter)
			{
				StateItem si = StateManager.States["filterbar"];
				if (si != null)
					si.Checked = !si.Checked;
			}
			else if (e.Button == tbbSearch)
			{
				StateItem si = StateManager.States["searchbar"];
				if (si != null)
					si.Checked = !si.Checked;
			}
			else if (e.Button == tbbNew)
			{
				New();
			}
			else if (e.Button == tbbDelete)
			{
				Delete();
			}
			else if (e.Button == tbbCustom)
			{
				CustomizeView();
			}
			else if (e.Button == tbbPrint)
			{
				PrintPreview();
			}
		}
		
		private void ToolBarButtonClick(ToolBarButton button)
		{
			ToolBarButtonClickEventArgs e=new ToolBarButtonClickEventArgs(button);
			ToolBarButtonClicked(this, e);
		}
		#endregion

		#region Filter Bar

		#region Filter Class

		public class Filter : GeneralCollection.CollectionItem
		{
			protected object _value;
			public virtual object Value
			{
				get { return _value; }
				set { _value = value; }
			}
			
			protected bool _stopEvents=false;
			/// <summary>
			/// set StopEvents to true to cancel any events for this filter.
			/// </summary>
			public virtual bool StopEvents
			{
				get { return _stopEvents; }
				set { _stopEvents = value; }
			}

			protected Sport.UI.Controls.GenericItem _genericItem;
			public Sport.UI.Controls.GenericItem GenericItem
			{
				get { return _genericItem; }
			}

			public event EventHandler FilterChanged;

			protected void OnFilterChange()
			{
				if ((FilterChanged != null)&&(_stopEvents == false))
					FilterChanged(this, EventArgs.Empty);
			}

			public override void OnOwnerChange(object oo, object no)
			{
				if (oo != null)
				{
					((TableView) oo).filterBar.Items.Remove(_genericItem);
				}
				if (no != null)
				{
					((TableView) no).filterBar.Items.Add(_genericItem);
				}

				base.OnOwnerChange (oo, no);
			}

		}

		public class ComboBoxFilter : Filter
		{
			private string _noValue;
			// Sets and gets the current item selected by the filter
			public override object Value
			{
				get
				{
					return _value;
				}
				set
				{
					((Sport.UI.Controls.NullComboBox) _genericItem.Control).SelectedItem = value;
				}
			}

			// Set the possible selections in the filter
			public void SetValues(object[] values)
			{
				_genericItem.Values = values;
				_value = _genericItem.Value;
			}

			// Construct the filter with the given caption, possible values,
			// selected value and given width
			public ComboBoxFilter(string title, object[] values, object value, string noValue, int width)
			{
				_genericItem = new Sport.UI.Controls.GenericItem(title, 
					Sport.UI.Controls.GenericItemType.Selection, 
					value, values, new System.Drawing.Size(width, 0));
				Sport.UI.Controls.NullComboBox cb = (Sport.UI.Controls.NullComboBox) _genericItem.Control;
				_noValue = noValue;
				if (_noValue != null)
				{
					cb.Text = _noValue;
					_genericItem.Nullable = true;
				}
				else
				{
					_genericItem.Nullable = false;
				}
				cb.SelectedItem = value;
				_value = cb.SelectedItem;
				cb.SelectedIndexChanged += new System.EventHandler(IndexChanged);
				if (width > 0)
					cb.Width = width;
			}

			// Construct the filter with the given caption, possible values
			// and selected value.
			public ComboBoxFilter(string title, object[] values, object value, string noValue)
				: this(title, values, value, noValue, 100)
			{
			}
	
			// Receives combo box event of item change
			private void IndexChanged(object sender, EventArgs e)
			{
				object v = ((Sport.UI.Controls.NullComboBox) _genericItem.Control).SelectedItem;
				if (_value != v)
				{
					// If the selected item in the combo box has changed
					// firing FilterChanged event.
					_value = v;
					OnFilterChange();
				}
			}
		}

		public class ButtonBoxFilter : Filter
		{
			// Sets and gets the current item selected by the filter
			public override object Value
			{
				get
				{
					return _value;
				}
				set
				{
					_value = value;
					((Sport.UI.Controls.ButtonBox) _genericItem.Control).Value = _value;
				}
			}

			// Construct the filter with the given caption, value selector,
			// current value and given width
			public ButtonBoxFilter(string title, Sport.UI.Controls.ButtonBox.SelectValue valueSelector, 
				object value, string nullText, int width)
			{
				_genericItem = new Sport.UI.Controls.GenericItem(title, 
					Sport.UI.Controls.GenericItemType.Button, value, 
					null, new System.Drawing.Size(width, 0));
				Sport.UI.Controls.ButtonBox bb = (Sport.UI.Controls.ButtonBox) _genericItem.Control;
				_value = value;
				//bb.Value = value;
				bb.Text = nullText;
				bb.ValueChanged += new System.EventHandler(ValueChanged);
				bb.ValueSelector = valueSelector;
				if (width > 0)
					bb.Width = width;
				//layoutItem = new LayoutPanel.LayoutItem(caption, bb);
			}

			// Construct the filter with the given caption, value selector
			// and current value
			public ButtonBoxFilter(string title, Sport.UI.Controls.ButtonBox.SelectValue valueSelector, 
				object value)
				: this(title, valueSelector, value, null, 100)
			{
			}
	
			public ButtonBoxFilter(string title, Sport.UI.Controls.ButtonBox.SelectValue valueSelector, 
				object value, string nullText)
				: this(title, valueSelector, value, nullText, 100)
			{
			}

			// Receives combo box event of item change
			private void ValueChanged(object sender, EventArgs e)
			{
				_value = ((Sport.UI.Controls.ButtonBox) _genericItem.Control).Value;
				OnFilterChange();
			}
		}

		#endregion

		#region FilterCollection Class
        
		public class FilterCollection : GeneralCollection
		{
			public FilterCollection(TableView owner)
				: base(owner)
			{
			}

			public Filter this[int index]
			{
				get { return (Filter) GetItem(index); }
				set { SetItem(index, value); }
			}

			public void Insert(int index, Filter value)
			{
				InsertItem(index, value);
			}

			public void Remove(Filter value)
			{
				RemoveItem(value);
			}

			public bool Contains(Filter value)
			{
				return base.Contains(value);
			}

			public int IndexOf(Filter value)
			{
				return base.IndexOf(value);
			}

			public int Add(Filter value)
			{
				return AddItem(value);
			}
		}

		#endregion
		
		private FilterCollection filters;
		public FilterCollection Filters
		{
			get { return filters; }
		}

		public void FilterView()
		{
			if (filterBar.Visible && filters.Count > 0)
			{
				int current = -1;
				for (int n = 0; n < filters.Count && current == -1; n++)
				{
					if (filters[n].GenericItem.Control.ContainsFocus)
						current = n;
				}
				if (current == filters.Count - 1)
					current = 0;
				else
					current++;

				filters[current].GenericItem.Control.Focus();
			}
		}

		#endregion

		#region Search Bar

		#region Searcher Class

		public class Searcher : GeneralCollection.CollectionItem
		{
			internal Sport.Data.EntityField			_field;
			internal Sport.UI.Controls.GenericItem	_genericItem;

			/// <summary>
			/// solve bug with TextControl and TextBox casting.....
			/// </summary>
			private TextBox GetTextBox()
			{
				Control control=_genericItem.Control;
				if (control is TextBox)
				{
					return (TextBox) control;
				}
				else
				{
					if (control is TextControl)
					{
						return (TextBox) (control as TextControl);
					}
					else
					{
						throw new Exception("General Error: expected TextBox or TextControl but got "+
							control.GetType().Name);
					}
				}
			}

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

			bool locked = false;

			// Construct the searcher with the given title, 
			// search field and given width
			public Searcher(string title, Sport.Data.EntityField field, int width)
			{
				if (field == null)
					throw new ArgumentNullException("field", "Field must not be null");

				_field = field;

				_genericItem = new Sport.UI.Controls.GenericItem(title,
					Sport.UI.Controls.GenericItemType.Text, new System.Drawing.Size(width, 0));
				Sport.UI.Controls.TextControl tc = (Sport.UI.Controls.TextControl) _genericItem.Control;
				tc.TextChanged += new System.EventHandler(TextChanged);
				tc.KeyDown += new KeyEventHandler(TextKeyDown);
				tc.GotFocus += new System.EventHandler(TextFocus);
			}

			// Construct the searcher with the given caption, 
			// search field
			public Searcher(string title, Sport.Data.EntityField field)
				: this(title, field, 0)
			{
			}
	
			// Sets text in control
			internal void ResetText()
			{
				if (!locked)
				{
					locked = true;
					TextBox tb = GetTextBox(); //((TextBox) _genericItem.Control);
					string strText= "";
					Sport.Data.Entity entity = ((TableView) Owner).Current;
					if (entity != null)
						strText = _field.GetText(entity);
					tb.Text = strText;
					locked = false;
				}
			}

			internal void Clear()
			{
				if (!locked)
				{
					locked = true;
					GetTextBox().Text = "";
					locked = false;
				}
			}

			// Receives text box event of text change
			private void TextChanged(object sender, EventArgs e)
			{
				if (!locked)
				{
					locked = true;
					TextBox tb = GetTextBox(); //(TextBox) _genericItem.Control;
					/*
					if (_genericItem.Control is TextBox)
					{
						tb = (TextBox) _genericItem.Control;
					}
					else
					{
						if (_genericItem.Control is TextControl)
						{
							tb = (_genericItem.Control as TextControl).TxtBox;
						}
						else
						{
							throw new Exception("General Error: expected TextBox or TextControl but got "+
									_genericItem.Control.GetType().Name);
						}
					}
					*/
					
					string s = tb.Text;

					TableView view = ((TableView) Owner);

					foreach (Searcher searcher in view.Searchers)
					{
						if (searcher != this)
							searcher.Clear();
					}

					Sport.Data.EntityList entityList = view._listView.EntityList;
					if (entityList != null)
					{
						if (!entityList.Search(_field, s))
						{
							Sport.Data.Entity entity = ((TableView) Owner).Current;
							if (entity != null)
							{
								string full = _field.GetText(entity);
								if (full == null)
									full = "";
								tb.Text = full;
								int eq = Sport.Common.Tools.MatchStrings(s, full);
								tb.Select(eq, full.Length - eq);
							}
							else
							{
								tb.Text = "";
							}
						}
					}
					else
					{
						tb.Text = "";
					}
					locked = false;
				}
			}

			private void TextKeyDown(object sender, KeyEventArgs e)
			{
				if (e.KeyCode == Keys.Back)
				{
					TextBox tb = GetTextBox(); //((TextBox) _genericItem.Control);
					int start = tb.SelectionStart;
					if (start > 0)
						tb.Text = tb.Text.Substring(0, start - 1);
				}
			}

			private void TextFocus(object sender, EventArgs e)
			{
				GetTextBox().SelectAll(); //((TextBox) _genericItem.Control).SelectAll();
			}

			public override void OnOwnerChange(object oo, object no)
			{
				if (oo != null)
				{
					((TableView) oo).searchBar.Items.Remove(_genericItem);
				}
				if (no != null)
				{
					((TableView) no).searchBar.Items.Add(_genericItem);
				}
				base.OnOwnerChange (oo, no);
			}

		}

		#endregion

		#region SearcherCollection Class
        
		public class SearcherCollection : GeneralCollection
		{
			public SearcherCollection(TableView owner)
				: base(owner)
			{
			}

			public Searcher this[int index]
			{
				get { return (Searcher) GetItem(index); }
				set { SetItem(index, value); }
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

		public void SearchView()
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

		#endregion

		#region Grid Control

		private Sport.UI.Controls.Grid _grid;

		public bool EntitiesSelection
		{
			get { return _grid.ShowCheckBoxes; }
			set { _grid.ShowCheckBoxes = value; }
		}
		
		/// <summary>
		/// SelectedEntities returns entities of all checked rows
		/// </summary>
		public Sport.Data.Entity[] SelectedEntities
		{
			get
			{
				if (!EntitiesSelection)
					return null;

				Sport.Data.Entity[] result = new Sport.Data.Entity[_grid.CheckedRows.Size];
				int i = 0;
				foreach (RangeArray.Range range in _grid.CheckedRows)
				{
					for (int n = range.First; n <= range.Last; n++)
					{
						result[i] = EntityListView[n];
						i++;
					}
				}

				return result;
			}

			set
			{
				if (EntitiesSelection && _listView.EntityList != null)
				{
					_grid.CheckedRows.Clear();
					if (value != null)
					{
						for (int n = 0; n < value.Length; n++)
						{
							int index = _listView.EntityList.IndexOf(value[n]);
							if (index >= 0)
								_grid.CheckedRows[index] = true;
						}
					}
				}
			}
		}

		public Sport.Data.Entity[] GetMarkedEntities()
		{
			if (_grid.Selection == null || _grid.Selection.Size < 1)
				return null;

			Sport.Data.Entity[] entities = new Sport.Data.Entity[_grid.Selection.Rows.Length];
			for (int i = 0; i < _grid.Selection.Rows.Length; i++)
				entities[i] = this.EntityListView[_grid.Selection.Rows[i]];
			
			return entities;
		}

		public void SetEntityCheck(Sport.Data.Entity entity, bool chk)
		{
			int index = _listView.EntityList.IndexOf(entity);
			if (index >= 0)
				_grid.CheckedRows[index] = chk;
		}

		public event EventHandler SelectedEntitiesChanged;

		private void GridRowChecked(object sender, Sport.UI.Controls.Grid.CheckEventArgs e)
		{
			if (SelectedEntitiesChanged != null)
				SelectedEntitiesChanged(this, EventArgs.Empty);
		}

		public bool Editable
		{
			get { return _grid.Editable; }
			set
			{
				_grid.Editable = value;
				if (value == false)
				{
					this.detailsBar.Enabled = false;
					/*
					foreach (Control control in this.detailsBar.Controls)
					{
						control.Enabled = false;
					}
					*/
				}
			}
		}

		public SelectionMode SelectionMode
		{
			get { return _grid.SelectionMode; }
			set { _grid.SelectionMode = value; }
		}

		#endregion

		#region Table Items

		#region TableItem Class

		public class TableItem : GeneralCollection.CollectionItem
		{
			private string _title;
			public string Title
			{
				get { return _title; }
			}

			private int _width;
			public int Width
			{
				get { return _width; }
			}

			private int _height;
			public int Height
			{
				get { return _height; }
			}

			private int _field;
			public int Field
			{
				get { return _field; }
			}

			// Both table and details item
			public TableItem(int field, string title, int width, int height)
			{
				_title = title;
				_field = field;
				_width = width;
				_height = height;
				tableIndex = -1;
				detailIndex = -1;
			}

			public TableItem(int field, string title, int width)
				: this(field, title, width, 0)
			{
			}

			private DetailItem _detail;
			public DetailItem Detail
			{
				get { return _detail; }
			}

			// Details item only
			public TableItem(string title, DetailItem detail)
			{
				_title = title;
				_detail = detail;
				tableIndex = -1;
				detailIndex = -1;
			}

			public override string ToString()
			{
				return _title;
			}

			internal int tableIndex;
			internal int detailIndex;
		}

		#endregion

		#region TableItemCollection Class
        
		public class TableItemCollection : GeneralCollection
		{
			public TableItemCollection(TableView owner)
				: base(owner)
			{
			}

			public TableItem this[int index]
			{
				get
				{ return (TableItem) GetItem(index); }
				set { SetItem(index, value); }
			}

			public int GetFieldIndex(int field)
			{
				for (int n = 0; n < Count; n++)
				{
					if (this[n].Field == field)
						return n;
				}
				return -1;
			}

			public TableItem GetFieldItem(int field)
			{
				int index = GetFieldIndex(field);
				if (index >= 0)
					return this[index];
				return null;
			}

			public void Insert(int index, TableItem value)
			{
				InsertItem(index, value);
				this[index].tableIndex = Count;
			}

			// Both table and details item
			public void Insert(int index, int field, string title, int width, int height)
			{
				InsertItem(index, new TableItem(field, title, width, height));
				this[index].tableIndex = Count;
			}

			public void Insert(int index, int field, string title, int width)
			{
				InsertItem(index, new TableItem(field, title, width));
				this[index].tableIndex = Count;
			}
            
			// Details only item
			public void Insert(int index, string title, DetailItem detail)
			{
				InsertItem(index, new TableItem(title, detail));
				this[index].tableIndex = Count;
			}

			public void Remove(TableItem value)
			{
				RemoveItem(value);
			}

			public bool Contains(TableItem value)
			{
				return base.Contains(value);
			}

			public int IndexOf(TableItem value)
			{
				return base.IndexOf(value);
			}

			public int Add(TableItem value)
			{
				int index = AddItem(value);
				this[index].tableIndex = Count;
				return index;
			}

			// Both table and details item
			public int Add(int field, string title, int width, int height)
			{
				int index = AddItem(new TableItem(field, title, width, height));
				this[index].tableIndex = Count;
				return index;
			}

			public int Add(int field, string title, int width)
			{
				int index = AddItem(new TableItem(field, title, width));
				this[index].tableIndex = Count;
				return index;
			}
            
			// Details only item
			public int Add(string title, DetailItem detail)
			{
				int index = AddItem(new TableItem(title, detail));
				this[index].tableIndex = Count;
				return index;
			}
		}

		#endregion
		
		private TableItemCollection items;
		public TableItemCollection Items
		{
			get { return items; }
		}

		private void GridColumnMoved(object sender, Sport.UI.Controls.Grid.ColumnMoveEventArgs e)
		{
			for (int n = 0; n < items.Count; n++)
			{
				if (items[n].tableIndex == e.Column1)
					items[n].tableIndex = e.Column2;
				else if (items[n].tableIndex == e.Column2)
					items[n].tableIndex = e.Column1;
			}
			int w = _columnsWidth[e.Column1];
			_columnsWidth[e.Column1] = _columnsWidth[e.Column2];
			_columnsWidth[e.Column2] = w;

			theColumns = null;
			SaveConfiguration();
		}

		private int[] theColumns;
		private void BuildColumns()
		{
			System.Collections.SortedList sl = new System.Collections.SortedList();
			for (int n = 0; n < items.Count; n++)
			{
				if (items[n].tableIndex != -1)
				{
					sl.Add(items[n].tableIndex, n);
				}
			}
			theColumns = new int[sl.Count];
			sl.Values.CopyTo(theColumns, 0);
		}
		private void ResetColumns()
		{
			if (_grid == null || _grid.Columns == null || items == null)
				return;
			
			_grid.Columns.Clear();
			for (int n = 0; n < items.Count; n++)
			{
				if (items[n] != null)
					items[n].tableIndex = -1;
			}

			if (theColumns == null)
				return ;
			for (int n = 0; n < theColumns.Length; n++)
			{
				TableItem item = items[theColumns[n]];
				if (item == null)
					continue;
				if (item.Detail != null)
					throw new ArgumentException("Table item can only be set as detail");
				else
					_grid.Columns.Add(item.Field, item.Title, item.Width);

				item.tableIndex = n;
			}

			_grid.Refresh();

			RebuildColumnsWidth();
		}

		public int[] Columns
		{
			get
			{
				if (theColumns == null)
					BuildColumns();
				return theColumns;
			}
			set
			{
				theColumns = value;
				ResetColumns();
			}
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
			_columnsWidth = new int[_grid.Columns.Count];
			for (int n = 0; n < _grid.Columns.Count; n++)
			{
				_columnsWidth[n] = _grid.GetColumnWidth(0, n);
			}
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
			if (_columnsWidth != null && _grid.Columns.Count == _columnsWidth.Length)
			{
				_grid.SetColumnsWidth(0, _columnsWidth);

				_grid.Refresh();
			}
		}


		private int[] theDetails;
		private void BuildDetails()
		{
			System.Collections.SortedList sl = new System.Collections.SortedList();
			for (int n = 0; n < items.Count; n++)
			{
				if (items[n].detailIndex != -1)
				{
					sl.Add(items[n].detailIndex, n);
				}
			}
			theDetails = new int[sl.Count];
			sl.Values.CopyTo(theDetails, 0);
		}
		private void ResetDetails()
		{
			details.Clear();
			for (int n = 0; n < items.Count; n++)
			{
				items[n].detailIndex = -1;
			}
			if (theDetails == null)
				return ;
			for (int n = 0; n < theDetails.Length; n++)
			{
				TableItem item = items[theDetails[n]];
				if (item == null)
					continue;
				if (item.Detail != null)
				{
					details.Add(item.Detail);
				}
				else
				{
					if (item.Height > 0)
						details.Add(item.Field, item.Title + ":", new System.Drawing.Size(item.Width, item.Height));
					else
						details.Add(item.Field, item.Title + ":", item.Width);
				}

				item.detailIndex = n;
			}

			ResetDetailSize();
		}

		public int[] Details
		{
			get
			{
				if (theDetails == null)
					BuildDetails();
				return theDetails;
			}
			set
			{
				theDetails = value;
				ResetDetails();
			}
		}

		private Sport.Data.Entity[] storedSelectedEntities = null;
		private void StoreSelectedEntities()
		{
			storedSelectedEntities = null;
			if (EntitiesSelection)
				storedSelectedEntities = SelectedEntities;
		}

		private void ResetSelectedEntities()
		{
			if (storedSelectedEntities != null)
			{
				SelectedEntities = storedSelectedEntities;
				storedSelectedEntities = null;
			}
		}

		private void ListViewSorting(object sender, EventArgs e)
		{
			// If in entities selection, storing selected entity
			// for placing checks after sort
			StoreSelectedEntities();
		}

		private void ListViewSorted(object sender, EventArgs e)
		{
			ResetSelectedEntities();

			theSort = null;

			_grid.Refresh();
		}
		
		private void ListViewSortChanged(object sender, EventArgs e)
		{
			SaveConfiguration();
		}


		private int[] theSort;
		private void BuildSort()
		{
			int[] fields = _listView.Sort;
			System.Collections.ArrayList al = new System.Collections.ArrayList();
			if (fields != null)
			{
				int field;
				bool asc;
				for (int n = 0; n < fields.Length; n++)
				{
					if (fields[n] < 0)
					{
						asc = false;
						field = fields[n] - Int32.MinValue;
					}
					else
					{
						asc = true;
						field = fields[n];
					}
					int index = items.GetFieldIndex(field);
					if (index >= 0)
					{
						if (asc)
							al.Add(index);
						else
							al.Add(index + Int32.MinValue);
					}
				}
			}
			theSort = (int[]) al.ToArray(typeof(int));
		}
		private void ResetSort()
		{
			System.Collections.ArrayList al = new System.Collections.ArrayList();
			int index;
			bool asc;
			for (int n = 0; n < theSort.Length; n++)
			{
				if (theSort[n] < 0)
				{
					asc = false;
					index = theSort[n] - Int32.MinValue;
				}
				else
				{
					asc = true;
					index = theSort[n];
				}
				int field = items[index].Field;
				if (field >= 0)
				{
					if (asc)
						al.Add(field);
					else
						al.Add(field + Int32.MinValue);
				}
			}

			_listView.Sort = (int[]) al.ToArray(typeof(int));

			_grid.Refresh();
		}

		public int[] Sort
		{
			get
			{
				if (theSort == null)
					BuildSort();
				return theSort;
			}
			set
			{
				theSort = value;
				ResetSort();
			}
		}

		private void CustomizeView()
		{
			Dialogs.CustomizeForm cf = new Sport.UI.Dialogs.CustomizeForm(this);
			cf.ShowDialog();
		}

		#endregion

		#region Details Bar

		#region DetailItem Class

		public class DetailItem : GeneralCollection.CollectionItem
		{
			protected Sport.UI.Controls.GenericItem _genericItem;
			public Sport.UI.Controls.GenericItem GenericItem
			{
				get { return _genericItem; }
			}

			private string _title;
			private System.Drawing.Size _controlSize;

			public DetailItem(string title, System.Drawing.Size controlSize)
			{
				_title = title;
				_controlSize = controlSize;
			}

			protected virtual void CreateGenericItem(string title, System.Drawing.Size controlSize)
			{
			}

			public override void OnOwnerChange(object oo, object no)
			{
				if (_genericItem != null)
				{
					_title = _genericItem.Title;

					if (oo != null)
					{
						((TableView)oo).gpDetails.Items.Remove(_genericItem);
						Remove(((TableView)oo).EntityListView);
					}
				}
				if (no != null)
				{
					CreateGenericItem(_title, _controlSize);
					((TableView)no).gpDetails.Items.Add(_genericItem);
					Add(((TableView)no).EntityListView);
				}

				base.OnOwnerChange (oo, no);
			}

			public virtual void Add(EntityListView view)
			{
			}

			public virtual void Remove(EntityListView view)
			{
			}
		}

		public class FieldDetailItem : DetailItem
		{
			private int _field;
			public int Field
			{
				get { return _field; }
			}

			public FieldDetailItem(string title, int field, System.Drawing.Size controlSize)
				: base(title, controlSize)
			{
				_field = field;
			}

			protected override void CreateGenericItem(string title, System.Drawing.Size controlSize)
			{
				Sport.UI.EntityListView.Field field = ((TableView)Owner).Fields[_field];
				_genericItem = new Sport.UI.Controls.GenericItem(title, 
					field.GenericItemType, null, field.Values, controlSize);
			}

			public override void Add(EntityListView view)
			{
				view.Items.Add(_genericItem, _field);
				base.Add (view);
			}

			public override void Remove(EntityListView view)
			{
				view.Items.Remove(_genericItem);
				base.Remove (view);
			}
		}

		#region GridDetailItem Class

		public delegate void IndexChangeEventHandler(object sender, int change);
		
		public class GridDetailItem : DetailItem
		{
			public event IndexChangeEventHandler IndexChanged;
			
			private int				_idField;
			private EntityListView	_view;

			private bool			_autoInsert;
			public bool AutoInsert
			{
				get { return _autoInsert; }
				set { _autoInsert = value; }
			}

			private bool			_autoDelete;
			public bool AutoDelete
			{
				get { return _autoDelete; }
				set { _autoDelete = value; }
			}

			private Sport.UI.Controls.GridControl	gridControl;

			public Sport.UI.Controls.Grid.GridColumnCollection Columns
			{
				get { return gridControl.Grid.Columns; }
			}

			public EntityListView EntityListView
			{
				get { return _view; }
			}

			public GridDetailItem(string title, string entityType, int idField, System.Drawing.Size controlSize)
				: this(title, Sport.Data.EntityType.GetEntityType(entityType), idField, controlSize)
			{
			}

			public GridDetailItem(string title, Sport.Data.EntityType entityType, int idField, System.Drawing.Size controlSize)
				: base(title, controlSize)
			{
				_idField = idField;
				_view = new EntityListView(entityType);

				_genericItem = new Sport.UI.Controls.GenericItem(title, 
					Sport.UI.Controls.GenericItemType.Grid, controlSize);

				gridControl = _genericItem.Control as Sport.UI.Controls.GridControl;

				if (gridControl == null)
					throw new Exception("Failed to create a grid control generic item");
				
				_view.Items.Add(gridControl.Grid);
				
				_view.ListChanged += new EventHandler(ViewListChanged);
				_view.CurrentChanged += new Sport.Data.EntityEventHandler(ViewCurrentChanged);
				_view.EntityChanged += new Sport.Data.EntityChangeEventHandler(ViewEntityChanged);
				
				int i=0;
				
				i = gridControl.Buttons.Add(Sport.Resources.Images.Plus, "חדש");
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
				
				i = gridControl.Buttons.Add(Sport.Resources.Images.Up, "העלה");
				gridControl.Buttons[i].Enabled = false;
				gridControl.Buttons[i].Click += new EventHandler(UpClicked);
				
				i = gridControl.Buttons.Add(Sport.Resources.Images.Down, "הורד");
				gridControl.Buttons[i].Enabled = false;
				gridControl.Buttons[i].Click += new EventHandler(DownClicked);
				
				_autoInsert = true;
				_autoDelete = true;
			}

			public Sport.UI.Controls.Grid Grid
			{
				get { return ((Sport.UI.Controls.GridControl)_genericItem.Control).Grid; }
			}

			public bool Editable
			{
				get { return Grid.Editable; }
				set 
				{ 
					Grid.Editable = value; 
					ResetButtonState();
				}
			}

			public override void Add(EntityListView view)
			{
				view.Items.Add(_view, _idField);

				base.Add (view);
			}

			public override void Remove(EntityListView view)
			{
				view.Items.Remove(_view);

				base.Remove (view);
			}

			public event EventHandler NewClick;

			private void NewClicked(object sender, EventArgs e)
			{
				if (_autoInsert)
				{
					Sport.Data.Entity entity = ((TableView) Owner).Current;
					if (entity != null && _view.New())
					{
						_view.EntityType.Fields[_idField].SetValue(
							_view.EntityEdit, entity.Id);
					}
				}
				else
				{
					if (NewClick != null)
						NewClick(this, EventArgs.Empty);
				}
			}

			public event EventHandler DeleteClick;

			private void DeleteClicked(object sender, EventArgs e)
			{
				if (_autoDelete)
				{
					_view.Delete();
				}
				else
				{
					if (DeleteClick != null)
						DeleteClick(this, EventArgs.Empty);
				}
			}

			private void SaveClicked(object sender, EventArgs e)
			{
				_view.Save();
			}
			
			private void UpClicked(object sender, EventArgs e)
			{
				if (this.IndexChanged != null)
					IndexChanged(sender, -1);
			}
			
			private void DownClicked(object sender, EventArgs e)
			{
				if (this.IndexChanged != null)
					IndexChanged(sender, 1);
			}
			
			private void CancelClicked(object sender, EventArgs e)
			{
				_view.Cancel();
			}

			private void ResetButtonState()
			{
				gridControl.Enabled = _view.EntityList != null;
				if (!Editable)
				{
					// Plus button
					gridControl.Buttons[0].Enabled = false;
					// Minus button
					gridControl.Buttons[1].Enabled = false;
					// Save button
					gridControl.Buttons[2].Enabled = false;
					// Undo button
					gridControl.Buttons[3].Enabled = false;
					// Up button
					gridControl.Buttons[4].Enabled = false;
					// Down button
					gridControl.Buttons[5].Enabled = false;
				}
				else
				{
					// Plus button
					gridControl.Buttons[0].Enabled = !_view.Editing; // && gridControl.Enabled; //could not find any way to solve this otherwise.
					// Minus button
					gridControl.Buttons[1].Enabled = !_view.Editing && (_view.Current != null);
					// Save button
					gridControl.Buttons[2].Enabled = _view.Editing;
					// Undo button
					gridControl.Buttons[3].Enabled = _view.Editing;
					// Up button
					gridControl.Buttons[4].Enabled = !_view.Editing && (_view.Current != null);
					// Down button
					gridControl.Buttons[5].Enabled = !_view.Editing && (_view.Current != null);
				}
			}

			private void ViewCurrentChanged(object sender, Sport.Data.EntityEventArgs e)
			{
				ResetButtonState();
			}

			private void ViewEntityChanged(object sender, Sport.Data.EntityChangeEventArgs e)
			{
				ResetButtonState();
			}

			private void ViewListChanged(object sender, EventArgs e)
			{
				ResetButtonState();
			}
		}

		#endregion

		#endregion

		#region DetailItemCollection Class
        
		public class DetailItemCollection : GeneralCollection
		{
			public DetailItemCollection(TableView owner)
				: base(owner)
			{
			}

			public DetailItem this[int index]
			{
				get { return (DetailItem) GetItem(index); }
				set { SetItem(index, value); }
			}

			public void Insert(int index, DetailItem value)
			{
				InsertItem(index, value);
			}

			public void Remove(DetailItem value)
			{
				RemoveItem(value);
			}

			public bool Contains(DetailItem value)
			{
				return base.Contains(value);
			}

			public int IndexOf(DetailItem value)
			{
				return base.IndexOf(value);
			}

			public int Add(DetailItem value)
			{
				return AddItem(value);
			}

			public int Add(int field, string title, int width)
			{
				return Add(new FieldDetailItem(title, field, new System.Drawing.Size(width, 0)));
			}

			public int Add(int field, string title, System.Drawing.Size controlSize)
			{
				return Add(new FieldDetailItem(title, field, controlSize));
			}
		}

		#endregion
		
		private DetailItemCollection details;

		private System.Windows.Forms.Label		labelCaption;
		private Sport.UI.Controls.GenericPanel	gpDetails;
		private Sport.UI.Controls.ThemeButton	tbMore;
		private Sport.UI.Controls.ThemeButton	tbSave;
		private Sport.UI.Controls.ThemeButton	tbCancel;

		private void ResetDetailSize()
		{
			gpDetails.Width = labelCaption.Width;
			detailsBar.Height = labelCaption.Height + 
				gpDetails.ItemsHeight + tbMore.Height + 16;
			tbMore.Top = detailsBar.Height - tbMore.Height - 4;
			tbSave.Top = tbMore.Top;
			tbCancel.Top = tbMore.Top;
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize (e);

			ResetDetailSize();
		}

		#endregion

		#region  TableView's EntityListview

		private void ClearViewItems()
		{
			if (_listView != null)
			{
				//_listView.Items.Remove(_grid);
				foreach (DetailItem item in details)
				{
					item.Remove(_listView);
				}
			}
		}

		private void SetViewItems()
		{
			if (_listView != null)
			{
				//_listView.Items.Add(_grid);
				foreach (DetailItem item in details)
				{
					item.Add(_listView);
				}
			}
		}

		private EntityListView _listView;
		public EntityListView EntityListView
		{
			get { return _listView; }
			set
			{
				if (_listView != value)
				{
					ClearViewItems();

					if (_listView != null)
						_listView.Dispose();

					_listView = value;

					SetViewItems();

					_listView.ListChanged += new EventHandler(ListViewListChanged);
					_listView.ListChanging += new EventHandler(ListViewListChanging);
					_listView.CurrentChanged += new Sport.Data.EntityEventHandler(ListViewCurrentChanged);
					_listView.EntityChanged += new Sport.Data.EntityChangeEventHandler(ListViewEntityChanged);
					_listView.ValueChanged += new FieldEditEventHandler(ListViewValueChanged);
					_listView.Sorting += new EventHandler(ListViewSorting);
					_listView.Sorted += new EventHandler(ListViewSorted);
					_listView.SortChanged += new EventHandler(ListViewSortChanged);

					SetEnableState();
				}
			}
		}

		public bool Editing
		{
			get { return _listView.Editing; }
		}

		public Sport.Data.Entity Current
		{
			get 
			{ 
				return _listView == null ? null : _listView.Current; 
			}
			set
			{
				if (_listView != null)
					_listView.Current = value;
			}
		}

		public int CurrentIndex
		{
			get
			{
				return _listView == null ? -1 : _listView.CurrentIndex;
			}
			set
			{
				if (_listView != null)
					_listView.CurrentIndex = value;
			}
		}

		public EntityListView.FieldCollection Fields
		{
			get
			{
				return _listView == null ? null : _listView.Fields;
			}
		}


		#endregion

		#region Constructor

		public TableView()
		{
			// Collections creation
			items = new TableItemCollection(this);
			filters = new FilterCollection(this);
			searchers = new SearcherCollection(this);
			details = new DetailItemCollection(this);

			components = new System.ComponentModel.Container();

			SuspendLayout();

			// Controls creation
			toolBar = new Sport.UI.Controls.RightToolBar();
			filterBar = new Sport.UI.Controls.GenericPanel();
			searchBar = new Sport.UI.Controls.GenericPanel();
			_grid = new Sport.UI.Controls.Grid();
			detailsBar = new System.Windows.Forms.Panel();

			//
			// toolBar
			//
			tbbNew		= new System.Windows.Forms.ToolBarButton();
			tbbNew.ImageIndex = (int) Sport.Resources.ColorImages.New;
			tbbNew.Text = "חדש";
			tbbDelete	= new System.Windows.Forms.ToolBarButton();
			tbbDelete.ImageIndex = (int) Sport.Resources.ColorImages.Delete;
			tbbDelete.Text = "מחק";
			tbbFilter	= new System.Windows.Forms.ToolBarButton();
			tbbFilter.ImageIndex = (int) Sport.Resources.ColorImages.Cut;
			tbbFilter.Text = "חיתוך";
			tbbSearch	= new System.Windows.Forms.ToolBarButton();
			tbbSearch.ImageIndex = (int) Sport.Resources.ColorImages.Find;
			tbbSearch.Text = "חיפוש";
			tbbCustom	= new System.Windows.Forms.ToolBarButton();
			tbbCustom.ImageIndex = (int) Sport.Resources.ColorImages.Custom;
			tbbCustom.Text = "התאמה";
			tbbPrint	= new System.Windows.Forms.ToolBarButton();
			tbbPrint.ImageIndex = (int) Sport.Resources.ColorImages.Print;
			tbbPrint.Text = "הדפסה";

			toolBar.ImageList = Sport.Resources.ImageLists.CreateColorImageList(components);
			toolBar.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
																				  tbbNew,
																				  tbbDelete,
																				  tbbFilter,
																				  tbbSearch,
																				  tbbCustom,
																				  tbbPrint
																			  });
			toolBar.Name = "toolBar";
			toolBar.ShowToolTips = true;
			toolBar.Size = new System.Drawing.Size(0, 28);
			toolBar.TextAlign = System.Windows.Forms.ToolBarTextAlign.Right;
			toolBar.TabIndex = 0;
			toolBar.Dock = System.Windows.Forms.DockStyle.Top;
			toolBar.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(ToolBarButtonClicked);

			//
			// filterBar
			//
			filterBar.ItemsLayout = new Sport.UI.Controls.GenericDefaultLayout();
			filterBar.AutoSize = true;
			filterBar.Dock = System.Windows.Forms.DockStyle.Top;
			filterBar.Location = new System.Drawing.Point(0, 28);
			filterBar.Name = "filterBar";
			filterBar.TabIndex = 1;

			//
			// searchBar
			//
			searchBar.ItemsLayout = new Sport.UI.Controls.GenericDefaultLayout();
			searchBar.AutoSize = true;
			searchBar.Dock = System.Windows.Forms.DockStyle.Top;
			searchBar.Location = new System.Drawing.Point(0, 56);
			searchBar.Name = "searchBar";
			searchBar.TabIndex = 2;

			//
			// _grid
			//
			_grid.Dock = System.Windows.Forms.DockStyle.Fill;
			_grid.TabIndex = 3;
			_grid.ShowRowNumber = true;
			_grid.Source = this;
			_grid.ColumnMoved += new Sport.UI.Controls.Grid.ColumnMoveEventHandler(GridColumnMoved);
			_grid.ColumnResized += new Sport.UI.Controls.Grid.ColumnEventHandler(GridColumnResized);
			_grid.HeaderResized += new EventHandler(GridHeaderResized);
			_grid.RowResized += new Sport.UI.Controls.Grid.GroupEventHandler(GridRowResized);
			_grid.MouseUp += new MouseEventHandler(GridMouseUp);
			_grid.SelectionChanging += new Sport.UI.Controls.Grid.SelectionEventHandler(GridSelectionChanging);
			_grid.SelectionChanged += new EventHandler(GridSelectionChanged);
			_grid.RowChecked += new Sport.UI.Controls.Grid.CheckEventHandler(GridRowChecked);


			//
			// detailsBar
			//
			detailsBar.Dock = System.Windows.Forms.DockStyle.Bottom;
			detailsBar.Location = new System.Drawing.Point(0, 0);
			detailsBar.Name = "detailsBar";
			detailsBar.Size = new System.Drawing.Size(100, 100);
			detailsBar.TabIndex = 4;

			// labelCaption
			labelCaption = new System.Windows.Forms.Label();
			labelCaption.BackColor = System.Drawing.SystemColors.Highlight;
			labelCaption.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			labelCaption.ForeColor = System.Drawing.SystemColors.HighlightText;
			labelCaption.Location = new System.Drawing.Point(4, 4);
			labelCaption.Name = "detailsBarCaption";
			labelCaption.Size = new System.Drawing.Size(92, 20);
			labelCaption.TabIndex = 0;
			labelCaption.Text = "פרטים";
			labelCaption.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right |
				System.Windows.Forms.AnchorStyles.Top;
			labelCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

			// gpDetails
			gpDetails = new Sport.UI.Controls.GenericPanel();
			gpDetails.ItemsLayout = new Sport.UI.Controls.GenericDefaultLayout();
			gpDetails.AutoSize = true;
			gpDetails.Anchor = System.Windows.Forms.AnchorStyles.Left | 
				System.Windows.Forms.AnchorStyles.Top;
			gpDetails.Location = new System.Drawing.Point(4, 28);
			gpDetails.Size = new System.Drawing.Size(92, 48);
			gpDetails.TabIndex = 0;

			System.Windows.Forms.ImageList whiteImages = Sport.Resources.ImageLists.CreateWhiteImageList(components);

			// tbMore
			tbMore = new Sport.UI.Controls.ThemeButton();
			tbMore.Alignment = System.Drawing.StringAlignment.Center;
			tbMore.AutoSize = true;
			tbMore.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			tbMore.Hue = 220F;
			tbMore.Saturation = 0.9F;
			tbMore.ImageList = whiteImages;
			tbMore.ImageIndex = (int) Sport.Resources.Images.DLeft;
			tbMore.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			tbMore.ImageSize = new System.Drawing.Size(10, 10);
			tbMore.Location = new System.Drawing.Point(4, 80);
			tbMore.Anchor = System.Windows.Forms.AnchorStyles.Left;// | System.Windows.Forms.AnchorStyles.Bottom;
			tbMore.Name = "tbMore";
			tbMore.Text = "פרטים";
			tbMore.Click += new EventHandler(MoreClicked);

			// tbSave
			tbSave = new Sport.UI.Controls.ThemeButton();
			tbSave.Alignment = System.Drawing.StringAlignment.Center;
			tbSave.AutoSize = true;
			tbSave.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			tbSave.Hue = 120F;
			tbSave.Saturation = 0.5F;
			tbSave.ImageList = whiteImages;
			tbSave.ImageIndex = (int) Sport.Resources.Images.Save;
			tbSave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			tbSave.ImageSize = new System.Drawing.Size(10, 10);
			tbSave.Location = new System.Drawing.Point(tbMore.Right + 2, 80);
			tbSave.Anchor = System.Windows.Forms.AnchorStyles.Left;// | System.Windows.Forms.AnchorStyles.Bottom;
			tbSave.Name = "tbSave";
			tbSave.Text = "שמור";
			tbSave.Enabled = false;
			tbSave.Click += new EventHandler(SaveClicked);

			// tbCancel
			tbCancel = new Sport.UI.Controls.ThemeButton();
			tbCancel.Alignment = System.Drawing.StringAlignment.Center;
			tbCancel.AutoSize = true;
			tbCancel.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			tbCancel.Hue = 0F;
			tbCancel.Saturation = 0.7F;
			tbCancel.ImageList = whiteImages;
			tbCancel.ImageIndex = (int) Sport.Resources.Images.Undo;
			tbCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			tbCancel.ImageSize = new System.Drawing.Size(10, 10);
			tbCancel.Location = new System.Drawing.Point(tbSave.Right + 2, 80);
			tbCancel.Anchor = System.Windows.Forms.AnchorStyles.Left;// | System.Windows.Forms.AnchorStyles.Bottom;
			tbCancel.Name = "tbCancel";
			tbCancel.Text = "בטל";
			tbCancel.Enabled = false;
			tbCancel.Click += new EventHandler(CancelClicked);

			detailsBar.Controls.Add(labelCaption);
			detailsBar.Controls.Add(gpDetails);
			detailsBar.Controls.Add(tbMore);
			detailsBar.Controls.Add(tbSave);
			detailsBar.Controls.Add(tbCancel);

			//
			// TableView
			//
			Controls.Add(_grid);
			Controls.Add(searchBar);
			Controls.Add(filterBar);
			Controls.Add(toolBar);
			Controls.Add(detailsBar);

			ResumeLayout(false);

			SetEnableState();
		}

		#endregion

		#region View Operations

		public override void Open()
		{
			ReadConfiguration();

			//add user action:
			/*
			try
			{
				SessionServices.SessionService _service=
					new SessionServices.SessionService();
				_service.CookieContainer = Sport.Core.Session.Cookies;
				string description="Class:"+this.GetType().Name+this.State.ToString();
				_service.AddUserAction(SessionServices.Action.TableView_Opened, 
					description, Sport.Core.Data.CurrentVersion);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("failed to add Open View action: "+ex.Message);
			}
			*/
			
			base.Open ();
		}

		public override void Close()
		{
			Searchers.Clear();
			Filters.Clear();
			_grid.Columns.Clear();

			base.Close();
		}

		// Activate is called when the user opens or selects the view.
		public override void Activate()
		{
			// Reconnecting to filter and details state.
			// Each view is storing a different state for
			// tool bar, filter bar and details bar.

			StateItem si = StateManager.States["toolbar"];
			if (si != null)
			{
				si.Visible = toolBarEnabled;
				si.Checked = toolBarState && toolBarEnabled;
				si.StateChanged += new System.EventHandler(ToolBarStateChanged);
				SetToolBar();
			}
			si = StateManager.States["filterbar"];
			if (si != null)
			{
				si.Visible = filterBarEnabled;
				si.Checked = filterBarState && filterBarEnabled;
				tbbFilter.Pushed = si.Checked;
				si.StateChanged += new System.EventHandler(FilterBarStateChanged);
				SetFilterBar();
			}
			si = StateManager.States["detailsbar"];
			if (si != null)
			{
				si.Visible = detailsBarEnabled;
				si.Checked = detailsBarState && detailsBarEnabled;
				si.StateChanged += new System.EventHandler(DetailsBarStateChanged);
				SetDetailsBar();
			}
			si = StateManager.States["searchbar"];
			if (si != null)
			{
				si.Visible = SearchBarEnabled;
				si.Checked = searchBarState && searchBarEnabled;
				tbbSearch.Pushed = si.Checked;
				si.StateChanged += new System.EventHandler(SearchBarStateChanged);
				SetSearchBar();
			}

			base.Activate ();
		}

		// Deactivate is called when the user closes the view
		// or selects a different view.
		public override void Deactivate()
		{
			base.Deactivate ();

			// Disconnecting from filter and details state.
			// Each view is storing a different state for
			// filter bar and details bar.

			StateItem si = StateManager.States["toolbar"];
			if (si != null)
			{
				toolBarState = si.Checked;
				si.StateChanged -= new System.EventHandler(ToolBarStateChanged);
			}
			si = StateManager.States["filterbar"];
			if (si != null)
			{
				filterBarState = si.Checked;
				si.StateChanged -= new System.EventHandler(FilterBarStateChanged);
			}
			si = StateManager.States["detailsbar"];
			if (si != null)
			{
				detailsBarState = si.Checked;
				si.StateChanged -= new System.EventHandler(DetailsBarStateChanged);
			}
			si = StateManager.States["searchbar"];
			if (si != null)
			{
				searchBarState = si.Checked;
				si.StateChanged -= new System.EventHandler(SearchBarStateChanged);
			}
		}

		#endregion

		protected override void Dispose(bool disposing)
		{
			// If the view wasn't opened (e.g. selection dialog that
			// was not used), _listView will be null
			if (_listView != null)
				_listView.Dispose();

			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		private void SetEnableState()
		{
			if (_listView == null)
			{
				tbSave.Enabled = false;
				tbCancel.Enabled = false;
				tbMore.Enabled = false;
				tbbNew.Enabled = false;
				tbbDelete.Enabled = false;
				gpDetails.Enabled = false;
			}
			else
			{
				tbSave.Enabled = _listView.Editing;
				tbCancel.Enabled = tbSave.Enabled;
				tbbNew.Enabled = !_listView.Editing;
				tbbDelete.Enabled = (!_listView.Editing) && 
					((Current != null)||(_grid.Selection.Size > 1));
				gpDetails.Enabled = Current != null;
				tbMore.Enabled = gpDetails.Enabled;
			}
		}

		private void ListViewEntityChanged(object sender, Sport.Data.EntityChangeEventArgs e)
		{
			if (e.Operation == Sport.Data.EntityChangeOperation.Save)
				OnSaveEntity(e.Entity);
			else if (e.Operation == Sport.Data.EntityChangeOperation.Cancel)
				OnCancelEntity(e.Entity);
			SetEnableState();
		}

		private void ListViewListChanging(object sender, EventArgs e)
		{
			StoreSelectedEntities();
		}

		private void ListViewListChanged(object sender, EventArgs e)
		{
			_grid.RefreshSource();
			ResetSelectedEntities();
		}

		private void ListViewCurrentChanged(object sender, Sport.Data.EntityEventArgs e)
		{
			SetEnableState();
			SetGridSelection();
			if (Current == null)
				labelCaption.Text = "פרטים";
			else
				labelCaption.Text = "פרטים - " + Current.Name;
			OnSelectEntity(Current);
		}

		private void ListViewValueChanged(object sender, FieldEditEventArgs e)
		{
			OnValueChange(e.EntityEdit, e.EntityField);
			_grid.Refresh();
		}

		private void SaveClicked(object sender, EventArgs e)
		{
			if (!tbSave.Enabled)
				return;
			Save();
		}

		private void MoreClicked(object sender, EventArgs e)
		{
			if (!tbMore.Enabled)
				return;
			OpenDetails(Current);
		}

		private void CancelClicked(object sender, EventArgs e)
		{
			if (!tbCancel.Enabled)
				return;
			Cancel();
		}

		#region IGridSource

		void Sport.UI.Controls.IGridSource.SetGrid(Sport.UI.Controls.Grid grid)
		{
		}

		#region Read operations
		int Sport.UI.Controls.IGridSource.GetRowCount()
		{
			if (_listView == null)
				return 0;
			return _listView.Count;
		}

		int Sport.UI.Controls.IGridSource.GetFieldCount(int row)
		{
			if (_listView == null)
				return 0;
			return _listView.EntityType.Fields.Length;
		}

		int Sport.UI.Controls.IGridSource.GetGroup(int row)
		{
			return 0;
		}

		string Sport.UI.Controls.IGridSource.GetText(int row, int field)
		{
			Sport.Data.EntityField entityField = _listView.EntityType.Fields[field];
			Sport.Data.Entity entity = _listView[row];
			return entityField.GetText(entity);

		}

		Sport.UI.Controls.Style Sport.UI.Controls.IGridSource.GetStyle(int row, int field, Sport.UI.Controls.GridDrawState state)
		{
			return GetGridStyle(row, field, state);
		}

		string Sport.UI.Controls.IGridSource.GetTip(int row)
		{
			return GetGridTip(row);
		}

		#endregion

		#region Sorting

		int[] Sport.UI.Controls.IGridSource.GetSort(int group)
		{
			return _listView.Sort;
		}

		void Sport.UI.Controls.IGridSource.Sort(int group, int[] columns)
		{
			_listView.Sort = columns;
			_grid.Refresh();
		}

		#endregion

		#region Update operations

		private Sport.UI.Controls.GenericItem editGenericItem = null;

		Control Sport.UI.Controls.IGridSource.Edit(int row, int field)
		{
			if (editGenericItem != null)
			{
				_listView.Items.Remove(editGenericItem);
				editGenericItem = null;
			}

			if (_listView.Fields[field].CanEdit)
			{
				Sport.Data.EntityField entityField = _listView.EntityType.Fields[field];
				Sport.Data.Entity entity = _listView[row];
						
				EntityListView.Field viewField = _listView.Fields[field];

				object value=null;
				try
				{
					value = entityField.GetValueItem(entity);
				}
				catch
				{}
				editGenericItem = new Sport.UI.Controls.GenericItem(
					viewField.GenericItemType, value,
					viewField.Values);
				_listView.Items.Add(editGenericItem, field);

				return editGenericItem.Control;
			}
                    
			return null;
		}

		void Sport.UI.Controls.IGridSource.EditEnded(Control control)
		{
			if (editGenericItem == null || control != editGenericItem.Control)
				throw new ArgumentException("Given control is not edited control");
			_listView.Items.Remove(editGenericItem);
			editGenericItem = null;
		}

		#endregion
        
		#endregion

		#region Grid Events

		private void GridSelectionChanging(object sender, Sport.UI.Controls.Grid.SelectionEventArgs e)
		{
			if (_listView.Editing)
			{
				if (!Save())
					e.Continue = false;
			}
		}

		private bool inGridSelection = false;

		private void SetGridSelection()
		{
			if (!inGridSelection)
			{
				int index = _listView.CurrentIndex;
				if (index < 0)
				{
					_grid.Selection.Clear();
					_grid.SelectedIndex = -1;
				}
				else
				{
					_grid.Selection.Set(index, index);
					_grid.SelectedIndex = index;
					_grid.ScrollToRow(index);
				}
			}
		}

		private void GridSelectionChanged(object sender, EventArgs e)
		{
			inGridSelection = true;

			int rows = _grid.Selection.Size;
			if (rows == 1)
			{
				_listView.CurrentIndex = _grid.Selection.Rows[0];
			}
			else
			{
				_listView.CurrentIndex = -1;
			}

			inGridSelection = false;
		}

		#endregion

		#region Grid Style Overridables

		protected virtual Sport.UI.Controls.Style GetGridStyle(int row, int field, Sport.UI.Controls.GridDrawState state)
		{
			return null;
		}

		protected virtual string GetGridTip(int row)
		{
			return null;
		}

		#endregion

		#region Context Menu

		public enum SelectionType
		{
			None,
			Single,
			Multiple
		}

		public virtual MenuItem[] GetContextMenu(SelectionType selectionType)
		{
			return null;
		}

		private void GridMouseUp(object sender, MouseEventArgs e)
		{
			// Grid was right clicked, lets show menu
			if (e.Button == MouseButtons.Right)
			{
				int count = _grid.Selection.Size;
				
				//check if current logged in user has permissions over this view...
				//PermissionType userPermission=CurrentUserPermission();
				MenuItem[] menuItems = null;
				/*
				if (userPermission == PermissionType.Full)
				{
				*/
				SelectionType selType;
				switch (count)
				{
					case 0:
						selType = SelectionType.None;
						break;
					case 1:
						selType = SelectionType.Single;
						break;
					default:
						selType = SelectionType.Multiple;
						break;
				}
				menuItems = GetContextMenu(selType);
				//}
				
				if (menuItems != null)
				{
					Sport.UI.Controls.RightContextMenu cm = new Sport.UI.Controls.RightContextMenu(menuItems);
					cm.RightToLeft = RightToLeft.Yes;
					cm.Show((Control) sender, new System.Drawing.Point(e.X, e.Y));
				}
			}
		}

		#endregion

		#region Entity Operations

		public bool Edit()
		{
			if (_listView == null)
				return false;

			return _listView.Edit();
		}

		public bool Save()
		{
			_grid.CancelEdit();

			Sport.Data.EntityResult result = _listView.Save();

			if (result.Succeeded)
				return true;

			if (result is Sport.Data.EntityMessageResult)
			{
				Sport.UI.MessageBox.Show(((Sport.Data.EntityMessageResult) result).Message,
					System.Windows.Forms.MessageBoxIcon.Stop);
				return false;
			}
			else if (result == Sport.Data.EntityResultCode.FieldMustExist)
			{
				TableItem item = items.GetFieldItem(((Sport.Data.EntityFieldResult) result).Field.Index);
				if (item != null)
				{
					Sport.UI.MessageBox.Show("יש להכניס ערך בשדה '" + item.Title + "'",
						System.Windows.Forms.MessageBoxIcon.Information);
					return false;
				}
			}

			Sport.UI.MessageBox.Show("שגיאה בשמירת נתונים:\n" + result.ToString());
			return false;
		}

		protected virtual void OpenDetails(Sport.Data.Entity entity)
		{
		}

		public void Cancel()
		{
			_listView.Cancel();
		}

		protected virtual void NewEntity()
		{
			if (_listView.New())
			{
				OnNewEntity(_listView.EntityEdit);
				_listView.RefreshCurrent();
				SetEnableState();
			}
		}

		protected virtual void DeleteEntity()
		{
			_listView.Delete();
		}

		public void New()
		{
			if (canInsert == false)
				throw new Exception("can't add new Entity: view not in insert mode.");
			
			NewEntity();
		}

		public void Delete()
		{
			int selCount = _grid.Selection.Size;
			if (selCount > 1)
			{
				//get entities to delete:
				System.Collections.ArrayList arrToDelete=new System.Collections.ArrayList();
				//System.Collections.ArrayList arrIndices=new System.Collections.ArrayList();
				for (int row=0; row<_grid.Selection.Rows.Length; row++)
				{
					int index=_grid.Selection.Rows[row];
					Sport.Data.Entity entity = this.EntityListView[index];
					if (entity != null)
					{
						arrToDelete.Add(entity);
						//arrIndices.Add(index);
					}
				}

				//delete one by one...
				for (int i=0; i<arrToDelete.Count; i++)
				{
					Sport.Data.Entity entity=(Sport.Data.Entity) arrToDelete[i];
					Current = entity;
					if (OnDeleteEntity(entity))
					{
						DeleteEntity();
					}
					/*
					for (int j=i+1; j<arrIndices.Count; j++)
					{
						int index = (int) arrIndices[j];
						_grid.Selection.Add(index);
					}
					*/
				}
			}
			else
			{
				Sport.Data.Entity entity = Current;
				if (entity != null)
				{
					if (OnDeleteEntity(entity))
						DeleteEntity();
				}
			}
		}

		#endregion

		#region Configuration

		public void SaveConfiguration()
		{
			if (configurationRead)
			{
				Sport.Core.Configuration view = GetConfiguration();
				Sport.Core.Configuration table = view.GetConfiguration("Table", "table");
				Sport.Core.Configuration columns = table.GetConfiguration("Columns");
				columns.Value = Sport.Common.Tools.ArrayToString(Columns, ",");
				Sport.Core.Configuration columnsWidth = table.GetConfiguration("ColumnsWidth");
				columnsWidth.Value = Sport.Common.Tools.ArrayToString(ColumnsWidth, ",");
				Sport.Core.Configuration headerHeight = table.GetConfiguration("HeaderHeight");
				headerHeight.Value = _grid.HeaderHeight.ToString();
				Sport.Core.Configuration rowHeight = table.GetConfiguration("RowHeight");
				rowHeight.Value = _grid.Groups[0].RowHeight.ToString();
				Sport.Core.Configuration sort = table.GetConfiguration("Sort");
				sort.Value = Sport.Common.Tools.ArrayToString(Sort, ",");
				Sport.Core.Configuration details = view.GetConfiguration("Details");
				details.Value = Sport.Common.Tools.ArrayToString(Details, ",");
				Sport.Core.Configuration.Save();
			}
		}

		private bool configurationRead = false;
		public void ReadConfiguration()
		{
			if (State[SelectionDialog] == "1")
				return;
			
			Sport.Core.Configuration view = GetConfiguration();
			Sport.Core.Configuration table = view.GetConfiguration("Table", "table");
			Sport.Core.Configuration columns = table.GetConfiguration("Columns");
			int[] arr = Sport.Common.Tools.ToIntArray(columns.Value, ',');
			if (arr != null)
			{
				int[] oldColumns=Columns;
				try
				{
					Columns = arr;
				}
				catch
				{
					System.Diagnostics.Debug.WriteLine("failed to reset columns for the view "+this.ViewName+": \n"+
						String.Join(",", Sport.Common.Tools.ToStringArray(arr)));
					Columns = oldColumns;
				}
			}
			Sport.Core.Configuration columnsWidth = table.GetConfiguration("ColumnsWidth");
			arr = Sport.Common.Tools.ToIntArray(columnsWidth.Value, ',');
			if (arr != null)
			{
				int[] oldColWidths=ColumnsWidth;
				try
				{
					ColumnsWidth = arr;
				}
				catch
				{
					System.Diagnostics.Debug.WriteLine("failed to reset column widths for the view "+this.ViewName+": \n"+
						String.Join(",", Sport.Common.Tools.ToStringArray(arr)));
					ColumnsWidth = oldColWidths;
				}
			}
			Sport.Core.Configuration headerHeight = table.GetConfiguration("HeaderHeight");
			int val;
			if (Sport.Common.Tools.ToInt(headerHeight.Value, out val))
				_grid.HeaderHeight = val;
			Sport.Core.Configuration rowHeight = table.GetConfiguration("RowHeight");
			if (Sport.Common.Tools.ToInt(rowHeight.Value, out val))
				_grid.Groups[0].RowHeight = val;
			Sport.Core.Configuration sort = table.GetConfiguration("Sort");
			arr = Sport.Common.Tools.ToIntArray(sort.Value, ',');
			if (arr != null)
			{
				int[] oldSort=Sort;
				try
				{
					Sort = arr;
				}
				catch
				{
					System.Diagnostics.Debug.WriteLine("failed to reset sort columns for the view "+this.ViewName+": \n"+
						String.Join(",", Sport.Common.Tools.ToStringArray(arr)));
					Sort = oldSort;
				}
			}
			Sport.Core.Configuration details = view.GetConfiguration("Details");
			arr = Sport.Common.Tools.ToIntArray(details.Value, ',');
			if (arr != null)
			{
				int[] oldDetails=Details;
				try
				{
					Details = arr;
				}
				catch
				{
					System.Diagnostics.Debug.WriteLine("failed to reset detail columns for the view "+this.ViewName+": \n"+
						String.Join(",", Sport.Common.Tools.ToStringArray(arr)));
					Details = oldDetails;
				}
			}

			configurationRead = true;
		}

		#endregion

		/*
		/// <summary>
		/// returns permission of current logged in user over this view.
		/// </summary>
		private PermissionType CurrentUserPermission()
		{
			return Sport.Entities.User.ViewPermission(this.GetType().Name);
		}
		*/

		#region TableView Entity Operation Overridables

		/// <summary>
		/// OnSelectEntity is called when an entity is selected.
		/// TableView implementing class can override this method
		/// to set view fields settings.
		/// </summary>
		protected virtual void OnSelectEntity(Sport.Data.Entity entity)
		{
		}

		/// <summary>
		/// OnValueChange is called when a field value is changed.
		/// TableView implementing class can override this method
		/// to set view fields settings.
		/// </summary>
		protected virtual void OnValueChange(Sport.Data.EntityEdit entityEdit, Sport.Data.EntityField entityField)
		{
		}

		/// <summary>
		/// OnNewEntity is called when a new entity is created.
		/// TableView implementing class can override this method
		/// to set values for the new entity.
		/// </summary>
		protected virtual void OnNewEntity(Sport.Data.EntityEdit entityEdit)
		{
		}

		/// <summary>
		/// OnNewEntity is called when an entity is about to be deleted.
		/// TableView implementing class can override this method
		/// to ask the user for entity delete and to return whether
		/// or not the entity should be deleted.
		/// </summary>
		protected virtual bool OnDeleteEntity(Sport.Data.Entity entity)
		{
			return false;
		}

		/// <summary>
		/// OnSaveEntity is called when an edited entity was saved.
		/// TableView implementing class can override this method
		/// to take actions after the entity was saved.
		/// </summary>
		protected virtual void OnSaveEntity(Sport.Data.Entity entity)
		{
		}

		/// <summary>
		/// OnCancelEntity is called when an edited entity change was
		/// canceled.
		/// TableView implementing class can override this method
		/// to take actions after the change was canceled
		/// </summary>
		protected virtual void OnCancelEntity(Sport.Data.Entity entity)
		{
		}

		#endregion

		#region Printing

		private Sport.Documents.DocumentBuilder CreateDocumentBuilder(System.Drawing.Printing.PrinterSettings settings)
		{
			Cursor cur = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;

			Sport.Documents.DocumentBuilder db = new Sport.Documents.DocumentBuilder(Title);

			db.Direction = Sport.Documents.Direction.Right;
			db.SetSettings(settings);

			db.Direction = Sport.Documents.Direction.Right;
			db.Font = new System.Drawing.Font("Tahoma", 16, System.Drawing.GraphicsUnit.Pixel);
			db.Sections.Add(CreatePrintSection(db));

			Cursor.Current = cur;

			return db;
		}
		
		public Sport.Documents.Section CreatePrintSection(Sport.Documents.DocumentBuilder db)
		{
			System.Drawing.Rectangle margins = db.DefaultMargins;

			System.Drawing.Font fontHeader = new System.Drawing.Font("Tahoma", 
				20, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			
			System.Drawing.Font fontTitle = new System.Drawing.Font("Tahoma", 
				22, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);

			Sport.Documents.Section section = new Sport.Documents.Section();

			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(TableView));

			// Image
			System.Drawing.Image objImage = (System.Drawing.Image)
				resources.GetObject("Logo-new.bmp");
			Sport.Documents.ImageItem ii = new Sport.Documents.ImageItem(new System.Drawing.Rectangle(
				margins.Right - 200, 0, 200, 40), objImage); //margins.Top
			ii.ImagePosition = Documents.ImagePosition.Normal;
			section.Items.Add(ii);

			Sport.Documents.TextItem ti;

			// Date
			DateTime now = DateTime.Now;
			System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("he-IL");
			System.Globalization.DateTimeFormatInfo dtfi = ci.DateTimeFormat;
			ti = new Sport.Documents.TextItem(now.ToString("dd בMMMM yyyy", dtfi));
			ti.Bounds = new System.Drawing.Rectangle(margins.Left, margins.Top + 20, 170, 20);
			ti.Alignment = Sport.Documents.TextAlignment.Far;
			section.Items.Add(ti);
			dtfi.Calendar = new System.Globalization.HebrewCalendar();
			ti = new Sport.Documents.TextItem(now.ToString("dd בMMMM yyyy", dtfi));
			ti.Bounds = new System.Drawing.Rectangle(margins.Left, margins.Top, 170, 20);
			ti.Alignment = Sport.Documents.TextAlignment.Far;
			section.Items.Add(ti);
			
			// Title
			ti = new Sport.Documents.FieldTextItem("{" + ((int) Sport.Documents.TextField.Title).ToString() + "}");
			ti.Font = fontTitle;
			ti.ForeColor = System.Drawing.Color.Blue;
			ti.Bounds = new System.Drawing.Rectangle(margins.Right - (margins.Width - 160), margins.Top + 50, margins.Width - 160, 25);
			section.Items.Add(ti);
			
			// Filters
			int top = margins.Top + 90;
			foreach (Filter filter in filters)
			{
				if (filter.Value != null)
				{
					ti = new Sport.Documents.TextItem(filter.GenericItem.Title + " " +
						filter.Value.ToString());
					ti.Bounds = new System.Drawing.Rectangle(margins.Left + 150, top, margins.Width - 150, 30);
					ti.Font = fontHeader;
					section.Items.Add(ti);
					top += 30;
				}
			}

			// Table
			Sport.Documents.TableItem tableItem = new Sport.Documents.TableItem(new System.Drawing.Rectangle(margins.Left, top + 10,
				margins.Width, margins.Bottom - top - 10));

			//tableItem.Border = System.Drawing.SystemPens.WindowFrame;

			int[] fields = new int[_grid.Columns.VisibleCount];

			Sport.Documents.TableItem.TableCell[] header = new Sport.Documents.TableItem.TableCell[_grid.Columns.VisibleCount + 1];
			header[0] = new Sport.Documents.TableItem.TableCell();
			header[0].Border = System.Drawing.SystemPens.WindowFrame;
			Sport.Documents.TableItem.TableColumn tableColumn = new Sport.Documents.TableItem.TableColumn();
			tableColumn.Width = _grid.GetColumnWidth(0, -1);
			tableItem.Columns.Add(tableColumn);

			int n = 0;
			for (int c = 0; c < _grid.Columns.Count; c++)
			{
				Sport.UI.Controls.Grid.GridColumn column = _grid.Columns[c];
				if (column.Visible)
				{
					tableColumn = new Sport.Documents.TableItem.TableColumn();
					header[n + 1] = new Sport.Documents.TableItem.TableCell(column.Title);
					//header[n + 1].InnerCells = 3;
					header[n + 1].Border = System.Drawing.SystemPens.WindowFrame;
					fields[n] = column.Field;
					tableColumn.Alignment = (Sport.Documents.TextAlignment) column.Alignment;
					tableColumn.Width = _grid.GetColumnWidth(0, c);
					tableColumn.Border = System.Drawing.SystemPens.ControlDark;
					tableItem.Columns.Add(tableColumn);
					n++;
				}
			}

			Sport.Documents.TableItem.TableRow row = new Sport.Documents.TableItem.TableRow(header);

			row.BackColor = System.Drawing.Color.SkyBlue;
			row.Font = new System.Drawing.Font("Tahoma", 12, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			//tb.HeaderColumnBorder = System.Drawing.SystemPens.WindowFrame;
			row.Alignment = Sport.Documents.TextAlignment.Center;
			row.LineAlignment = Sport.Documents.TextAlignment.Center;
			//tb.RowBorder = System.Drawing.SystemPens.WindowFrame;

			tableItem.Rows.Add(row);

			string[] record;
			for (int r = 0; r < _listView.Count; r++)
			{
				record = new string[fields.Length + 1];
				record[0] = (r + 1).ToString();
				for (int f = 0; f < fields.Length; f++)
				{
					record[f + 1] = _listView[r].GetText(fields[f]);
				}

				tableItem.Rows.Add(record);
			}
			
			section.Items.Add(tableItem);
			
			// Page Number
			ti = new Sport.Documents.FieldTextItem("עמוד {" + ((int) Sport.Documents.TextField.Page).ToString() +
				"} מתוך {" + ((int) Sport.Documents.TextField.PageCount).ToString() + "}");
			ti.Bounds = new System.Drawing.Rectangle(margins.Left, margins.Bottom + 20, 150, 20);
			ti.Alignment = Sport.Documents.TextAlignment.Far;
			section.Items.Add(ti);

			return section;
		}

		private void PrintPreview()
		{
			System.Drawing.Printing.PrinterSettings ps;
			Sport.UI.Dialogs.PrintSettingsForm settingsForm;
			if (!Sport.UI.Helpers.GetPrinterSettings(out ps, out settingsForm))
				return;
			if (settingsForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				Sport.Documents.DocumentBuilder db = CreateDocumentBuilder(ps);
				Sport.Documents.Document document = db.CreateDocument();

				if (settingsForm.ShowPreview)
				{
					Sport.UI.Dialogs.PrintForm printForm = new Sport.UI.Dialogs.PrintForm(document, ps);
		
					if (!printForm.Canceled)
						printForm.ShowDialog();

					printForm.Dispose();
				}
				else
				{
					System.Drawing.Printing.PrintDocument pd = document.CreatePrintDocument(ps);
					pd.PrintController = new PrintControllerWithPageForm(pd.PrintController, 0);
					pd.Print();
				}
			}
		}

		#endregion

		#region ICommandTarget Members

		public virtual bool ExecuteCommand(string command)
		{
			if (command == Sport.Core.Data.KeyCommands.MarkItem)
			{
				if (EntitiesSelection && _grid.Selection.Size > 0)
				{
					int[] rows = _grid.Selection.Rows;
					bool mark = false;
					for (int n = 0; n < rows.Length && !mark; n++)
					{
						if (!_grid.CheckedRows[rows[n]])
							mark = true;
					}
					for (int n = 0; n < rows.Length; n++)
						_grid.CheckedRows[rows[n]] = mark;
					_grid.Refresh();
				}
			}
			else if (command == Sport.Core.Data.KeyCommands.EditItem)
			{
				_grid.Edit();
			}
			else if (command == Sport.Core.Data.KeyCommands.FilterTable)
			{
				FilterView();
			}
			else if (command == Sport.Core.Data.KeyCommands.SearchTable)
			{
				SearchView();
			}
			else if (command == Sport.Core.Data.KeyCommands.Print)
			{
				ToolBarButtonClick(tbbPrint);
			}
			else if (command == Sport.Core.Data.KeyCommands.CustomizeTable)
			{
				ToolBarButtonClick(tbbCustom);
			}
			else if (command == Sport.Core.Data.KeyCommands.DeleteItem)
			{
				ToolBarButtonClick(tbbDelete);
			}
			else if (command == Sport.Core.Data.KeyCommands.NewItem)
			{
				ToolBarButtonClick(tbbNew);
			}
			else if (command == Sport.Core.Data.KeyCommands.ItemDetails)
			{
				MoreClicked(this, EventArgs.Empty);
			}
			else if (command == Sport.Core.Data.KeyCommands.SaveItem)
			{
				if (!Editing)
					return false;
				SaveClicked(this, EventArgs.Empty);
			}
			else if (command == Sport.Core.Data.KeyCommands.Cancel)
			{
				if (!Editing)
					return false;
				CancelClicked(this, EventArgs.Empty);
			}
			else
			{
				return false;
			}

			return true;
		}

		#endregion

		public Sport.Data.Entity[] GetSelectedEntities()
		{
			int selCount = _grid.Selection.Size;
			System.Collections.ArrayList arrEntities = new System.Collections.ArrayList();
			if (selCount > 1)
			{
				for (int row = 0; row < _grid.Selection.Rows.Length; row++)
				{
					int index = _grid.Selection.Rows[row];
					Sport.Data.Entity entity = this.EntityListView[index];
					if (entity != null)
					{
						arrEntities.Add(entity);
					}
				}
			}
			return (Sport.Data.Entity[])arrEntities.ToArray(typeof(Sport.Data.Entity));
		}
	}
}
