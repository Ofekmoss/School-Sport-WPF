using System;
using Sport.Common;
using System.Windows.Forms;
using Sport.UI.Display;
using Sport.UI.Controls;

namespace Sport.UI
{
	/// <summary>
	/// ITableView is used in places both TableView and TableView2 should be used
	/// </summary>
	public interface ITableView
	{
		bool Editable { get; set; }

        bool DetailsBarEnabled { get; set; }
		bool ToolBarEnabled { get; set; }

		SelectionMode SelectionMode { get; set; }

		bool Editing { get; }

		bool Edit();
		bool Save();
		void Cancel();
		void New();
		void Delete();

		void SetEntityCheck(Sport.Data.Entity entity, bool chk);

		bool EntitiesSelection { get; set; }
		Sport.Data.Entity[] SelectedEntities { get; set; }
		event EventHandler SelectedEntitiesChanged;

		Sport.Data.Entity Current { get; set; }
	}

	/// <summary>
	/// TableView inherits View to implement a view that contains
	/// a grid and a filter, search and details bar.
	/// The TableView uses an EntityListView instance to display and
	/// edit the entities in an EntityList.
	/// Table based views will inherit from the TableView and will
	/// set the EntityListView and the filter, search and 
	/// details collections implemented by TableView.
	/// </summary>
	public class TableView2 : Sport.UI.View, ICommandTarget, ITableView
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
		private bool canDelete=true;

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
			public Filter()
			{
				_parameters = new ParameterCollection(this);
			}

			protected object _value;
			public virtual object Value
			{
				get { return _value; }
				set { _value = value; }
			}
			
			protected Sport.UI.Controls.GenericItem _genericItem;
			public Sport.UI.Controls.GenericItem GenericItem
			{
				get { return _genericItem; }
			}

			#region ParameterCollection

			public class ParameterCollection : System.Collections.IEnumerable
			{
				private Sport.Data.EntityQuery.Parameter[] _parameters;

				private Filter _filter;

				public ParameterCollection(Filter filter)
				{
					_filter = filter;
					_parameters = new Sport.Data.EntityQuery.Parameter[0];
				}

				public void Add(Sport.Data.EntityQuery.Parameter parameter)
				{
					Sport.Data.EntityQuery.Parameter[] parameters = 
						new Sport.Data.EntityQuery.Parameter[_parameters.Length + 1];

					_parameters.CopyTo(parameters, 0);

					parameters[_parameters.Length] = parameter;
					_parameters = parameters;
                    
					parameter.Value = _filter.Value == null ? Sport.Data.EntityQuery.Parameter.NoValue : _filter.Value;
				}

				public void Remove(Sport.Data.EntityQuery.Parameter parameter)
				{
					int index = Array.IndexOf(_parameters, parameter);

					if (index >= 0)
					{
						Sport.Data.EntityQuery.Parameter[] parameters = 
							new Sport.Data.EntityQuery.Parameter[_parameters.Length - 1];

						Array.Copy(_parameters, parameters, index);
						Array.Copy(_parameters, index + 1, parameters, index, parameters.Length - index);
						_parameters = parameters;
					}
				}

				public void Set(Sport.Data.EntityQuery.Parameter parameter)
				{
					_parameters = new Sport.Data.EntityQuery.Parameter[1] { parameter };

					parameter.Value = _filter.Value == null ? Sport.Data.EntityQuery.Parameter.NoValue : _filter.Value;
				}

				#region IEnumerable Members

				public System.Collections.IEnumerator GetEnumerator()
				{
					return _parameters.GetEnumerator();
				}

				#endregion
			}

			#endregion

			private ParameterCollection _parameters;
			public ParameterCollection Parameters
			{
				get { return _parameters; }
			}

			public event EventHandler FilterChanged;
			public event EventHandler FilterChanging;

			protected void OnFilterChange()
			{
				object value = _value == null ? Sport.Data.EntityQuery.Parameter.NoValue : _value;
				
				if (FilterChanging != null)
					FilterChanging(this, EventArgs.Empty);
				
				foreach (Sport.Data.EntityQuery.Parameter parameter in _parameters)
					parameter.Value = value;
				
				if (FilterChanged != null)
					FilterChanged(this, EventArgs.Empty);
			}

			public override void OnOwnerChange(object oo, object no)
			{
				if (oo != null)
				{
					((TableView2) oo).filterBar.Items.Remove(_genericItem);
				}
				if (no != null)
				{
					((TableView2) no).filterBar.Items.Add(_genericItem);
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

			private Sport.Data.EntityQuery _valuesQuery;
			public Sport.Data.EntityQuery ValuesQuery
			{
				get { return _valuesQuery; }
				set
				{
					if (_valuesQuery != value)
					{
						if (_valuesQuery != null)
						{
							_valuesQuery.Changed -= new EventHandler(QueryChanged);
						}

						_valuesQuery = value;
						
						if (_valuesQuery != null)
						{
							_valuesQuery.Changed += new EventHandler(QueryChanged);

							if (_valuesQuery.Prepared)
							{
								SetValues(_valuesQuery.GetEntities());
							}
							else
							{
								SetValues(null);
							}
						}
					}
				}
			}

			private void QueryChanged(object sender, EventArgs e)
			{
				RequeryValues();
			}

			public void RequeryValues()
			{
				if (_valuesQuery.Prepared)
				{
					SetValues(_valuesQuery.GetEntities());
				}
				else
				{
					SetValues(null);
				}
			}
			
			// Set the possible selections in the filter
			public void SetValues(object[] values)
			{
				_genericItem.Values = values;

				object value = _genericItem.Value;
				if (value != _value)
				{
					_value = value;
					OnFilterChange();
				}
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
				: this(title, values, value, noValue, 0)
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
				: this(title, valueSelector, value, null, 0)
			{
			}
	
			public ButtonBoxFilter(string title, Sport.UI.Controls.ButtonBox.SelectValue valueSelector, 
				object value, string nullText)
				: this(title, valueSelector, value, nullText, 0)
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
			public FilterCollection(TableView2 owner)
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
			internal Filter _filter;

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

			public Filter AffectedFilter
			{
				get { return _filter; }
				set { _filter = value; }
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
					Sport.Data.Entity entity = ((TableView2) Owner).Current;
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
					
					if (_filter != null)
					{
						if (_filter is ComboBoxFilter && _filter.GenericItem.Values != null)
						{
							string strEntityName = tb.Text;
							ComboBoxFilter cbf = (ComboBoxFilter) _filter;
							int curValue = (cbf.Value == null) ? -1 : (cbf.Value as Sport.Data.Entity).Id;
							if (strEntityName != null && strEntityName.Length > 0)
							{
								bool blnFound = false;
								foreach (Sport.Data.Entity entity in cbf.GenericItem.Values)
								{
									if (entity != null && entity.Name != null && entity.Id >= 0)
									{
										string strCurrentName = entity.Name;
										while (strCurrentName.IndexOf("  ") >= 0)
											strCurrentName = strCurrentName.Replace("  ", " ");
										if (strCurrentName.StartsWith(strEntityName))
										{
											blnFound = true;
											if (entity.Id != curValue)
											{
												cbf.Value = entity;
											}
											break;
										}
									}
								}
				
								if (!blnFound && strEntityName.Length >= 3)
								{
									foreach (Sport.Data.Entity entity in cbf.GenericItem.Values)
									{
										if (entity != null && entity.Name != null && entity.Id >= 0)
										{
											string strCurrentName = entity.Name;
											while (strCurrentName.IndexOf("  ") >= 0)
												strCurrentName = strCurrentName.Replace("  ", " ");
											if (strCurrentName.IndexOf(strEntityName) >= 0)
											{
												if (entity.Id != curValue)
												{
													cbf.Value = entity;
												}
												break;
											}
										}
									}
								}
							}
							else
							{
								cbf.Value = null;
							}
						}
					}
					else
					{
						TableView2 view = ((TableView2) Owner);

						foreach (Searcher searcher in view.Searchers)
						{
							if (searcher != this)
								searcher.Clear();
						}

						Sport.Data.EntityList entityList = view._listView.EntityList;
						string s = tb.Text;
						if (entityList != null)
						{
							if (!entityList.Search(_field, s))
							{
								Sport.Data.Entity entity = ((TableView2) Owner).Current;
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
					}
					
					locked = false;
				}
			}

			private void TextKeyDown(object sender, KeyEventArgs e)
			{
				if (_filter == null)
				{
					if (e.KeyCode == Keys.Back)
					{
						TextBox tb = GetTextBox(); //((TextBox) _genericItem.Control);
						int start = tb.SelectionStart;
						if (start > 0)
							tb.Text = tb.Text.Substring(0, start - 1);
					}
				}
			}

			private void TextFocus(object sender, EventArgs e)
			{
				if (_filter == null)
				{
					GetTextBox().SelectAll(); //((TextBox) _genericItem.Control).SelectAll();
				}
			}

			public override void OnOwnerChange(object oo, object no)
			{
				if (oo != null)
				{
					((TableView2) oo).searchBar.Items.Remove(_genericItem);
				}
				if (no != null)
				{
					((TableView2) no).searchBar.Items.Add(_genericItem);
				}
				base.OnOwnerChange (oo, no);
			}

		}

		#endregion

		#region SearcherCollection Class
        
		public class SearcherCollection : GeneralCollection
		{
			public SearcherCollection(TableView2 owner)
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

			public void Insert(int index, string title, Sport.Data.EntityField field)
			{
				InsertItem(index, new Searcher(title, field));
			}

			public void Insert(int index, string title, Sport.Data.EntityField field, int width)
			{
				InsertItem(index, new Searcher(title, field, width));
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

			public int Add(string title, Sport.Data.EntityField field)
			{
				return AddItem(new Searcher(title, field));
			}

			public int Add(string title, Sport.Data.EntityField field, int width)
			{
				return AddItem(new Searcher(title, field, width));
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
				if (EntitiesSelection)
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

		private void ResetColumns()
		{
			_grid.Columns.Clear();

			if (_columns == null)
				return ;

			for (int n = 0; n < _columns.Length; n++)
			{
				EntityFieldView efv = EntityView.Fields[_columns[n]];
				_grid.Columns.Add(_columns[n], efv.Name, efv.Width);
			}

			_grid.Refresh();

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

		#endregion

		#region Details Items

		private int[] _details;
		public int[] Details
		{
			get
			{
				if (_details == null)
					_details = new int[0];
				return _details;
			}
			set
			{
				_details = value;
				ResetDetails();
			}
		}

		private void ResetDetails()
		{
			details.Clear();

			if (_details == null)
				return ;

			for (int n = 0; n < _details.Length; n++)
			{
				/*if (item.Detail != null) // means some added a details that is not
					// a field detail
				{
				}
				else
				{*/
				EntityFieldView efv = EntityView[_details[n]];
				details.Add(efv.Field, efv.Name + ":", efv.Size);
				//}
			}

			ResetDetailSize();
		}

		#endregion

		#region Sorting

		private Sport.Data.Entity[] presortSelectedEntities = null;

		private void ListViewSorting(object sender, EventArgs e)
		{
			// If in entities selection, storing selected entity
			// for placing checks after sort
			presortSelectedEntities = null;
			if (EntitiesSelection)
				presortSelectedEntities = SelectedEntities;
		}

		private void ListViewSorted(object sender, EventArgs e)
		{
			if (presortSelectedEntities != null)
			{
				SelectedEntities = presortSelectedEntities;
				presortSelectedEntities = null;
			}

			_grid.Refresh();
		}

		private void ListViewSortChanged(object sender, EventArgs e)
		{
			SaveConfiguration();
		}

		public int[] Sort
		{
			get { return _listView.Sort; }
			set
			{
				_listView.Sort = value;
				_grid.Refresh();
			}
		}

		private void CustomizeView()
		{
			Dialogs.CustomizeForm2 cf = new Sport.UI.Dialogs.CustomizeForm2(this);
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
				if (_genericItem != null && oo != null)
				{
					((TableView2)oo).gpDetails.Items.Remove(_genericItem);
				}

				base.OnOwnerChange (oo, no);

				if (no != null)
				{
					if (_genericItem == null)
					{
						CreateGenericItem(_title, _controlSize);
					}

					((TableView2)no).gpDetails.Items.Add(_genericItem);
				}
			}

			public virtual void ReadEntity(Sport.Data.Entity entity, Sport.Data.EntityField entityField)
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
				EntityFieldView efv = ((TableView2)Owner).EntityView.Fields[_field];
				_genericItem = new Sport.UI.Controls.GenericItem(title, 
					efv.GenericItemType, null, efv.Values, controlSize);
				_genericItem.ReadOnly = !efv.CanEdit;
				_genericItem.Nullable = !efv.MustExist;
				_genericItem.Tag = _field;
				_genericItem.ValueChanged += new EventHandler(GenericItemValueChanged);
			}

			private bool reading = false;

			private void GenericItemValueChanged(object sender, EventArgs e)
			{
				if (!reading)
					((TableView2) Owner).ChangeValue(_genericItem, (int) _genericItem.Tag, _genericItem.Value);
			}

			public override void ReadEntity(Sport.Data.Entity entity, Sport.Data.EntityField entityField)
			{
				if (entityField != null && entityField.Index != _field)
					return ;

				reading = true;

				if (entity == null)
				{
					_genericItem.Value = null;
				}
				else
				{
					Sport.Data.EntityField ef = ((TableView2) Owner).EntityListView.EntityView.Fields[_field].EntityField;
					_genericItem.Value = ef.GetValueItem(entity);
				}

				reading = false;
			}
		}

		#region GridDetailItem Class

		public class GridDetailItem : DetailItem
		{
			private int				_idField;
			private EntityListView	_view;

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

			private void NewClicked(object sender, EventArgs e)
			{
				Sport.Data.Entity entity = ((TableView2) Owner).Current;
				if (entity != null && _view.New())
				{
					_view.EntityType.Fields[_idField].SetValue(
						_view.EntityEdit, entity.Id);
				}
			}

			private void DeleteClicked(object sender, EventArgs e)
			{
				_view.Delete();
			}

			private void SaveClicked(object sender, EventArgs e)
			{
				_view.Save();
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
				}
				else
				{
					// Plus button
					gridControl.Buttons[0].Enabled = !_view.Editing && gridControl.Enabled;
					// Minus button
					gridControl.Buttons[1].Enabled = !_view.Editing && (_view.Current != null);
					// Save button
					gridControl.Buttons[2].Enabled = _view.Editing;
					// Undo button
					gridControl.Buttons[3].Enabled = _view.Editing;
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

		/// <summary>
		/// ChangeValue is called to set a value to a field
		/// of the current entity in the view
		/// </summary>
		private void ChangeValue(object changer, int field, object value)
		{
			_listView.ChangeValue(changer, field, value);
		}

        private void ReadDetails(Sport.Data.Entity entity)
		{
			foreach (DetailItem item in details)
			{
				item.ReadEntity(entity, null);
			}
		}

		#region DetailItemCollection Class
        
		public class DetailItemCollection : GeneralCollection
		{
			public DetailItemCollection(TableView2 owner)
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

		private EntityListView2 _listView;
		public EntityListView2 EntityListView
		{
			get { return _listView; }
		}

        public EntityView EntityView
		{
			get { return _listView.EntityView; }
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

		#endregion

		#region Constructor

		public TableView2(EntityView entityView)
		{
			_listView = new EntityListView2(entityView);
			_listView.ListChanged += new EventHandler(ListViewListChanged);
			_listView.CurrentChanged += new Sport.Data.EntityEventHandler(ListViewCurrentChanged);
			_listView.EntityChanged += new Sport.Data.EntityChangeEventHandler(ListViewEntityChanged);
			_listView.ValueChanged += new FieldEditEventHandler(ListViewValueChanged);
			_listView.Sorting += new EventHandler(ListViewSorting);
			_listView.Sorted += new EventHandler(ListViewSorted);
			_listView.SortChanged += new EventHandler(ListViewSortChanged);

			// Collections creation
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
			_grid.Source = _listView;
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

			_autoTitle = true;
			RefreshTitle();
		}

		#endregion

		#region View Operations

		public override void Open()
		{
			ReadConfiguration();
			
			/*
			//check if current logged in user has permissions over this view...
			PermissionType userPermission=CurrentUserPermission();
			if (userPermission == PermissionType.None)
			{
				throw new Exception("Can't open view: Permission Denied");
			}
			if (userPermission == PermissionType.ReadOnly)
			{
				this.CanInsert = false;
				this.Editable = false;
				this.tbbDelete.Visible = false;
			}
			*/
			
			/*
			string strTypeName=typeof(Dialogs.PrintForm).Name;
			if (Sport.Entities.User.ViewPermission(strTypeName) ==
				Sport.Entities.PermissionServices.PermissionType.None)
			{
				this.tbbPrint.Enabled = false;
			}
			*/
			
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
			
			EntityListView.Requery();

			base.Open ();
			
			if (State["new_entity"] == "1")
				this.New();
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
			_autoTitle = false;
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
			if (e.Operation == Sport.Data.EntityChangeOperation.Cancel)
			{
				_grid.Refresh();
				ReadDetails(e.Entity);
			}

			SetEnableState();

			OnEntityChanged(e.Index, e.Operation, e.Entity);
		}

		private void ListViewListChanged(object sender, EventArgs e)
		{
			_grid.RefreshSource();

			if (_autoTitle)
			{
				//Sport.UI.MessageBox.Show("refresh title...");
				RefreshTitle();
			}
		}

		protected virtual void OnCurrentChanged(Sport.Data.Entity entity)
		{
		}

		protected virtual void OnEntityChanged(int index, 
			Sport.Data.EntityChangeOperation operation, Sport.Data.Entity entity)
		{
		}

		private void ListViewCurrentChanged(object sender, Sport.Data.EntityEventArgs e)
		{
			SetEnableState();
			SetGridSelection();
			Sport.Data.Entity current = Current;
			if (current == null)
				labelCaption.Text = "פרטים";
			else
				labelCaption.Text = "פרטים - " + current.Name;

			ReadDetails(current);

			OnCurrentChanged(current);
		}

		private void ListViewValueChanged(object sender, FieldEditEventArgs e)
		{
			_grid.Refresh();

			foreach (DetailItem item in details)
			{
				if (item.GenericItem != sender)
					item.ReadEntity(e.EntityEdit, e.EntityField);
			}
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

		#region Details View

		private Type _detailsView;
		public Type DetailsView
		{
			get { return _detailsView; }
			set { _detailsView = value; }
		}

		private void DetailsClick(object sender, EventArgs e)
		{
			if (_detailsView != null && Current != null)
			{
				OpenDetails(Current);
			}
		}

		#endregion

		#region Context Menu

		public enum SelectionType
		{
			None,
			Single,
			Multiple
		}

		public virtual void OnContextMenu(SelectionType selectionType, RightContextMenu menu)
		{
		}

		private void GridMouseUp(object sender, MouseEventArgs e)
		{
			// Grid was right clicked, lets show menu
			if (e.Button == MouseButtons.Right)
			{
				int count = _grid.Selection.Size;
				
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

				Sport.UI.Controls.RightContextMenu cm = new Sport.UI.Controls.RightContextMenu();
				if (selType == SelectionType.Single)
				{
					if (_detailsView != null)
					{
						cm.MenuItems.Add(new MenuItem("פתח", new System.EventHandler(DetailsClick)));
						cm.MenuItems.Add(new MenuItem("-"));
						cm.MenuItems[0].DefaultItem = true;
					}
				}

				OnContextMenu(selType, cm);
				
				if (cm.MenuItems.Count > 0)
				{
					// Removing separetor if it's last
					if (cm.MenuItems[cm.MenuItems.Count - 1].Text == "-")
						cm.MenuItems.RemoveAt(cm.MenuItems.Count - 1);
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
				Sport.UI.MessageBox.Show("יש להכניס ערך בשדה '" + EntityView.Fields[((Sport.Data.EntityFieldResult)result).Field.Index].Name + "'",
					System.Windows.Forms.MessageBoxIcon.Information);
				return false;
			}

			Sport.UI.MessageBox.Show("שגיאה בשמירת נתונים:\n" + result.ToString());
			return false;
		}

		protected virtual void OpenDetails(Sport.Data.Entity entity)
		{
			new OpenDialogCommand().Execute(_detailsView, "id=" + entity.Id);
		}

		public void Cancel()
		{
			_listView.Cancel();
		}

		protected virtual void NewEntity()
		{
			if (_listView.New())
			{
				//OnNewEntity(_listView.EntityEdit);
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
					if (EntityView.OnDeleteEntity(entity))
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
					if (EntityView.OnDeleteEntity(entity))
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
				Columns = arr;
			Sport.Core.Configuration columnsWidth = table.GetConfiguration("ColumnsWidth");
			arr = Sport.Common.Tools.ToIntArray(columnsWidth.Value, ',');
			if (arr != null)
				ColumnsWidth = arr;
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
				Sort = arr;
			Sport.Core.Configuration details = view.GetConfiguration("Details");
			arr = Sport.Common.Tools.ToIntArray(details.Value, ',');
			if (arr != null)
				Details = arr;

			configurationRead = true;
		}

		#endregion

		private bool _autoTitle;
		public bool AutoTitle
		{
			get { return _autoTitle; }
			set { _autoTitle = value; }
		}

		public new string Title
		{
			get { return base.Title; }
			set
			{
				base.Title = value;
				_autoTitle = false;
			}
		}

		private void RefreshTitle()
		{
			string title = EntityView.PluralName;

			foreach (Sport.Data.EntityQuery.Parameter parameter in _listView.EntityQuery.Parameters)
			{
				if (!parameter.Empty && parameter.Value != null)
				{
					title += " - " + parameter.Value.ToString();
				}
			}

			base.Title = title;
		}

		#region Printing

		private Sport.Documents.DocumentBuilder CreateDocumentBuilder(System.Drawing.Printing.PrinterSettings settings)
		{
			Cursor cur = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;

			Sport.Documents.DocumentBuilder db = new Sport.Documents.DocumentBuilder(Title);

			db.Direction = Sport.Documents.Direction.Right;
			db.SetSettings(settings);

			System.Drawing.Rectangle margins = db.DefaultMargins;
			
			db.Direction = Sport.Documents.Direction.Right;
			db.Font = new System.Drawing.Font("Tahoma", 16, System.Drawing.GraphicsUnit.Pixel);

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
				margins.Width, margins.Bottom - top - 30));

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
			bool blnAccountEntry = (this.EntityView.EntityType.Name == Sport.Entities.AccountEntry.TypeName);
			for (int r = 0; r < _listView.Count; r++)
			{
				record = new string[fields.Length + 1];
				record[0] = (r + 1).ToString();
				for (int f = 0; f < fields.Length; f++)
				{
					Sport.Data.Entity curEntity = _listView[r];
					int curField = fields[f];
					record[f + 1] = curEntity.GetText(curField);
					string strExtra = Sport.UI.EntityView.GetExtraText(curEntity, curField);
					if (strExtra != null && strExtra.Length > 0)
					{
						record[f + 1] += " - " + strExtra;
					}
				}

				tableItem.Rows.Add(record);
			}

			section.Items.Add(tableItem);
			
			//balance?
			if (this.EntityListView.EntityView.EntityType.Name == 
				Sport.Entities.AccountEntry.TypeName)
			{
				if (this.EntityListView.Count > 0)
				{
					double balance = 0;
					bool blnSameAccount = true;
					string strLastName = null;
					for (int i=0; i<this.EntityListView.Count; i++)
					{
						Sport.Entities.AccountEntry account = 
							new Sport.Entities.AccountEntry(this.EntityListView[i]);
						string strCurName = account.Account.Name;
						if (strLastName == null)
							strLastName = strCurName;
						if (strCurName != strLastName)
						{
							blnSameAccount = false;
							break;
						}
						balance += account.Sum;
					}
					if (blnSameAccount)
					{
						ti = new Sport.Documents.TextItem();
						ti.Text = "מאזן: "+
							Sport.Common.Tools.FixHebrewNegativeNumber(balance)+" ₪";
						ti.Font = fontTitle;
						ti.Bounds = new System.Drawing.Rectangle(
							margins.Left, margins.Bottom-15, margins.Width, 25);
						ti.Alignment = Sport.Documents.TextAlignment.Near;
						section.Items.Add(ti);
					}
				}
			}

			// Page Number
			ti = new Sport.Documents.FieldTextItem("עמוד {" + ((int) Sport.Documents.TextField.Page).ToString() +
				"} מתוך {" + ((int) Sport.Documents.TextField.PageCount).ToString() + "}");
			ti.Bounds = new System.Drawing.Rectangle(margins.Left, margins.Bottom + 20, 150, 20);
			ti.Alignment = Sport.Documents.TextAlignment.Far;
			section.Items.Add(ti);
            
			db.Sections.Add(section);

			Cursor.Current = cur;

			return db;
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
	}
}
