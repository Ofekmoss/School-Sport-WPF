using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;
using Sport.Common;
using System.IO;

namespace Sport.UI.Controls
{
	[Flags]
	public enum GridDrawState
	{
		Normal = 0,
		Selected = 1,
		Focused = 2
	}

	public enum GridGroupingStyle
	{
		Grid,
		List
	}

	#region IGridSource

	/// <summary>
	/// IGridSource serves as a source of data for the grid.
	/// The grid uses the IGridSource to retrieve cells
	/// values and to update cells.
	/// The grid data source should be implemented for data 
	/// reading and writing.
	/// </summary>
	public interface IGridSource : IDisposable
	{
		void SetGrid(Grid grid);

		// Read operations
		int GetRowCount();
		int GetFieldCount(int row);
		int GetGroup(int row);
		string GetText(int row, int field);
		Style GetStyle(int row, int field, GridDrawState state);
		string GetTip(int row);

		// Sorting
		int[] GetSort(int group);
		void Sort(int group, int[] columns);

		// Update operations
		Control Edit(int row, int field);
		void EditEnded(Control control);
	}

	#endregion

	#region GridDrawStyle Class

	public class Style : ICloneable
	{
		private System.Drawing.Brush _background;
		/// <summary>
		/// Gets or sets the brush used for background
		/// </summary>
		public System.Drawing.Brush Background
		{
			get { return _background; }
			set
			{
				_background = value;
				OnChange();
			}
		}

		private System.Drawing.Brush _foreground;
		/// <summary>
		/// Gets or sets the brush used for text color
		/// </summary>
		public System.Drawing.Brush Foreground
		{
			get { return _foreground; }
			set
			{
				_foreground = value;
				OnChange();
			}

		}

		private System.Drawing.Font _font;
		/// <summary>
		/// Gets or sets the font used
		/// </summary>
		public System.Drawing.Font Font
		{
			get { return _font; }
			set
			{
				_font = value;
				OnChange();
			}
		}

		public event EventHandler StyleChanged;

		private void OnChange()
		{
			if (StyleChanged != null)
				StyleChanged(this, EventArgs.Empty);
		}

		public Style()
		{
		}

		public Style(System.Drawing.Brush background,
			System.Drawing.Brush foreground, System.Drawing.Font font)
		{
			_background = background;
			_foreground = foreground;
			_font = font;
		}

		#region ICloneable Members

		public object Clone()
		{
			Style style = new Style();
			style._background = _background;
			style._foreground = _foreground;
			style._font = _font;
			return style;
		}

		#endregion
	}

	public class GridDrawStyle : ICloneable
	{
		private Style _normal;
		public Style Normal
		{
			get { return _normal; }
		}

		private Style _group;
		public Style Group
		{
			get { return _group; }
		}

		private Style _title;
		public Style Title
		{
			get { return _title; }
		}

		private Style _selectedTitle;
		public Style SelectedTitle
		{
			get { return _selectedTitle; }
		}

		private Style _selected;
		public Style Selected
		{
			get { return _selected; }
		}

		private Style _unfocused;
		public Style Unfocused
		{
			get { return _unfocused; }
		}

		private Style _unfocusedSelected;
		public Style UnfocusedSelected
		{
			get { return _unfocusedSelected; }
		}

		public GridDrawStyle()
		{
			_normal = new Style();
			_normal.StyleChanged += new EventHandler(StyleStyleChanged);
			_group = new Style();
			_group.StyleChanged += new EventHandler(StyleStyleChanged);
			_title = new Style();
			_title.StyleChanged += new EventHandler(StyleStyleChanged);
			_selectedTitle = new Style();
			_selectedTitle.StyleChanged += new EventHandler(StyleStyleChanged);
			_selected = new Style();
			_selected.StyleChanged += new EventHandler(StyleStyleChanged);
			_unfocused = new Style();
			_unfocused.StyleChanged += new EventHandler(StyleStyleChanged);
			_unfocusedSelected = new Style();
			_unfocusedSelected.StyleChanged += new EventHandler(StyleStyleChanged);
		}

		public event EventHandler StyleChanged;

		private void StyleStyleChanged(object sender, EventArgs e)
		{
			if (StyleChanged != null)
				StyleChanged(this, EventArgs.Empty);
		}

		#region ICloneable Members

		public object Clone()
		{
			GridDrawStyle style = new GridDrawStyle();
			style._normal = (Style)_normal.Clone();
			style._selected = (Style)_selected.Clone();
			style._unfocused = (Style)_unfocused.Clone();
			style._unfocusedSelected = (Style)_unfocusedSelected.Clone();

			return style;
		}

		#endregion
	}

	#endregion

	#region Grid Class
	/// <summary>
	/// Grid Control
	/// </summary>
	public class Grid : ControlWrapper
	{
		#region Grid Columns

		#region DrawCellEventArgs Class

		public class DrawCellEventArgs : EventArgs
		{
			private GridColumn _column;
			public GridColumn Column
			{
				get { return _column; }
			}

			private int _row;
			public int Row
			{
				get { return _row; }
			}

			private Graphics _graphics;
			public Graphics Graphics
			{
				get { return _graphics; }
			}

			private Rectangle _rectangle;
			public Rectangle Rectangle
			{
				get { return _rectangle; }
			}

			private Style _style;
			public Style Style
			{
				get { return _style; }
			}

			private GridDrawState _state;
			public GridDrawState State
			{
				get { return _state; }
			}

			private bool _handled;
			public bool Handled
			{
				get { return _handled; }
				set { _handled = value; }
			}

			public DrawCellEventArgs(GridColumn column, int row,
				Graphics graphics, Rectangle rectangle, Style style, GridDrawState state)
			{
				_column = column;
				_row = row;
				_graphics = graphics;
				_rectangle = rectangle;
				_style = style;
				_state = state;
				_handled = false;
			}
		}

		#endregion

		// Delegate that receives DrawCellEventArgs
		public delegate void DrawCellEventHandler(object sender, DrawCellEventArgs e);

		#region CellMouseEventArgs Class

		public class CellMouseEventArgs : EventArgs
		{
			private GridColumn _column;
			public GridColumn Column
			{
				get { return _column; }
			}

			private int _row;
			public int Row
			{
				get { return _row; }
			}

			private Rectangle _bounds;
			public Rectangle Bounds
			{
				get { return _bounds; }
			}

			private Size _offset;
			public Size Offset
			{
				get { return _offset; }
			}

			public CellMouseEventArgs(GridColumn column, int row, Rectangle bounds, Size offset)
			{
				_column = column;
				_row = row;
				_bounds = bounds;
				_offset = offset;
			}
		}

		#endregion

		// Delegate that receives CellMouseEventArgs
		public delegate void CellMouseEventHandler(object sender, CellMouseEventArgs e);

		#region GridColumn Class

		/// <summary>
		/// GridColumn
		/// </summary>
		public sealed class GridColumn : GeneralCollection.CollectionItem
		{
			private string _title;
			public string Title
			{
				get { return _title; }
				set { _title = value; }
			}

			private int _field;
			public int Field
			{
				get { return _field; }
				set { _field = value; }
			}

			private int _width;
			public int Width
			{
				get { return _width; }
				set { _width = value <= 0 ? 1 : value; }
			}

			private bool _visible;
			public bool Visible
			{
				get { return _visible; }
				set
				{
					if (_visible != value)
					{
						_visible = value;
						if (Owner != null)
						{
							GridGroup gg = (GridGroup)Owner;
							if (gg != null)
								gg.OnVisibleChange();
						}
					}
				}
			}

			private StringAlignment _alignment;
			public StringAlignment Alignment
			{
				get { return _alignment; }
				set { _alignment = value; }
			}

			public event DrawCellEventHandler DrawCell;

			internal bool OnDrawCell(int row, Graphics graphics,
				Rectangle rectangle, Style style, GridDrawState state)
			{
				if (DrawCell != null)
				{
					DrawCellEventArgs e = new DrawCellEventArgs(this, row, graphics,
						rectangle, style, state);
					DrawCell(this, e);

					return e.Handled;
				}

				return false;
			}

			public event CellMouseEventHandler MouseDown;

			internal void OnMouseDown(int row, Rectangle bounds, Size offset)
			{
				if (MouseDown != null)
				{
					MouseDown(this, new CellMouseEventArgs(this, row, bounds, offset));
				}
			}

			public GridColumn(int field, string title, int width, StringAlignment alignment)
			{
				_field = field;
				_title = title;
				_width = width <= 0 ? 1 : width;
				_alignment = alignment;
				_visible = true;
			}

			public GridColumn(int field, string title, int width)
				: this(field, title, width, StringAlignment.Near)
			{
			}

			public GridColumn(int field, string title)
				: this(field, title, 100)
			{
			}

			public GridColumn(int field)
				: this(field, null)
			{
			}

			public GridColumn()
				: this(-1)
			{
			}

			public override string ToString()
			{
				return _title;
			}
		}

		#endregion

		#region GridColumnCollection Class

		/// <summary>
		/// GridColumnCollection inherits GeneralCollection
		/// to use GridColumn.
		/// </summary>
		[ListBindable(false), Editor("System.ComponentModel.Design.CollectionEditor,System.Design", typeof(UITypeEditor))]
		public class GridColumnCollection : GeneralCollection
		{
			public GridColumnCollection(GridGroup group)
				: base(group)
			{
			}

			public GridColumn this[int index]
			{
				get { return (GridColumn)GetItem(index); }
				set { SetItem(index, value); }
			}

			public void Insert(int index, GridColumn value)
			{
				InsertItem(index, value);
			}

			public void Insert(int index, int column, string title, int width)
			{
				InsertItem(index, new GridColumn(column, title, width));
			}

			public void Remove(GridColumn value)
			{
				RemoveItem(value);
			}

			public bool Contains(GridColumn value)
			{
				return base.Contains(value);
			}

			public int IndexOf(GridColumn value)
			{
				return base.IndexOf(value);
			}

			public int Add(GridColumn value)
			{
				return AddItem(value);
			}

			public int Add(int field, string title, int width)
			{
				return AddItem(new GridColumn(field, title, width));
			}

			public int Add(int field, string title, int width, StringAlignment alignment)
			{
				return AddItem(new GridColumn(field, title, width, alignment));
			}

			public int VisibleCount
			{
				get
				{
					int c = 0;
					foreach (GridColumn gc in this)
						if (gc.Visible)
							c++;
					return c;
				}
			}
		}

		#endregion

		#endregion

		#region Grid Groups

		private GridGroupingStyle _groupingStyle = GridGroupingStyle.Grid;
		public GridGroupingStyle GroupingStyle
		{
			get { return _groupingStyle; }
			set
			{
				if (_groupingStyle != value)
				{
					if (_groupingStyle == GridGroupingStyle.Grid)
						headerheight = headerheight / groups.Count;
					else
						headerheight = headerheight * groups.Count;
					_groupingStyle = value;
					Refresh();
				}
			}
		}

		#region GridGroup Class

		/// <summary>
		/// GridGroup
		/// </summary>
		public sealed class GridGroup : GeneralCollection.CollectionItem
		{
			private int _rowheight;
			public int RowHeight
			{
				get { return _rowheight; }
				set { _rowheight = value; }
			}
			private int _lastVisible;
			public int LastVisible
			{
				get { return _lastVisible; }
			}

			private GridColumnCollection _columns;
			public GridColumnCollection Columns
			{
				get { return _columns; }
			}

			public GridGroup()
			{
				_rowheight = 20;
				_lastVisible = -1;
				_columns = new GridColumnCollection(this);
				_columns.Changed += new CollectionEventHandler(ColumnChanged);
			}

			internal void OnVisibleChange()
			{
				_lastVisible = -1;
				for (int c = 0; c < _columns.Count; c++)
				{
					if (_columns[c].Visible)
						_lastVisible = c;
				}

				((Grid)Owner).OnVisibleChange();
			}

			private void ColumnChanged(object sender, CollectionEventArgs e)
			{
				if (e.New != null)
				{
					if (((GridColumn)e.New).Visible)
					{
						if (e.Index > _lastVisible)
							_lastVisible = e.Index;
						else if (e.Index < _lastVisible)
							_lastVisible++;
					}
				}
				if (e.Old != null)
				{
					if (((GridColumn)e.Old).Visible && e.Index < _lastVisible)
						_lastVisible--;
				}

				((Grid)Owner).groupColumnsWidth = null;
			}
		}

		#endregion

		#region GridGroupCollection Class

		/// <summary>
		/// GridGroupCollection inherits GeneralCollection
		/// to use GridGroup.
		/// </summary>
		[ListBindable(false)]
		public class GridGroupCollection : GeneralCollection
		{
			public GridGroupCollection(Grid grid)
				: base(grid)
			{
			}

			public GridGroup this[int index]
			{
				get { return (GridGroup)GetItem(index); }
			}

			public void Insert(int index, GridGroup value)
			{
				InsertItem(index, value);
			}

			public void Insert(int index)
			{
				InsertItem(index, new GridGroup());
			}

			public void Remove(GridGroup value)
			{
				RemoveItem(value);
			}

			public bool Contains(GridGroup value)
			{
				return base.Contains(value);
			}

			public int IndexOf(GridGroup value)
			{
				return base.IndexOf(value);
			}

			public int Add(GridGroup value)
			{
				return AddItem(value);
			}

			public int Add()
			{
				return AddItem(new GridGroup());
			}
		}

		#endregion

		#endregion

		#region Grid Selection

		#region GridSelection Class

		/// <summary>
		/// GridSelection inherit RangeArray to 
		/// form up the selection of rows in the grid.
		/// </summary>
		public class GridSelection : RangeArray
		{
			// Owner grid
			private Grid _owner;

			// _rows stores an array of indexes of the rows
			// that are selected
			// _rows is created when Rows get is called
			// and destroyed when the selection changes.
			private int[] _rows;
			// Returning the indexes of the selected rows
			public int[] Rows
			{
				get
				{
					if (_rows == null)
					{
						int i = 0;
						_rows = new int[Size];

						foreach (Range range in this)
						{
							for (int n = range.First; n <= range.Last; n++)
							{
								_rows[i] = n;
								i++;
							}
						}
					}

					return _rows;
				}
			}

			// Constructor
			public GridSelection(Grid owner)
			{
				_owner = owner;
			}


			// Set sets the selection to the fiven
			// from and to rows
			public void Set(int from, int to)
			{
				// First clearing selection
				Clear();
				// Then selecting range
				Add(from, to);
			}

			protected override void OnRangeChange()
			{
				_rows = null;
				base.OnRangeChange();
			}
		}

		#endregion

		#region Selection Events

		public class SelectionEventArgs
		{
			private int _from;
			private int _to;
			private bool _add;

			public int From { get { return _from; } }
			public int To { get { return _to; } }
			public bool Add { get { return _add; } }

			public bool Continue;

			public SelectionEventArgs(int from, int to, bool add)
			{
				_from = from;
				_to = to;
				_add = add;
				Continue = true;
			}
		}

		public delegate void SelectionEventHandler(object sender, SelectionEventArgs e);

		public event SelectionEventHandler SelectionChanging;
		public event System.EventHandler SelectionChanged;

		private bool OnSelectionChanging(int from, int to, bool add)
		{
			switch (_selectionMode)
			{
				case (SelectionMode.None):
					return false;
				case (SelectionMode.One):
					if (from != to)
						return false;
					if (add && selection.Size != 0)
						return false;
					break;
			}

			if (SelectionChanging != null)
			{
				SelectionEventArgs e = new SelectionEventArgs(from, to, add);

				SelectionChanging(this, e);

				return e.Continue;
			}

			return true;
		}

		private void OnSelectionChanged()
		{
			if (SelectionChanged != null)
				SelectionChanged(this, EventArgs.Empty);
		}

		#endregion

		#region Selection Properties

		private SelectionMode _selectionMode = SelectionMode.MultiExtended;
		public SelectionMode SelectionMode
		{
			get { return _selectionMode; }
			set { _selectionMode = value; }
		}

		// Grid's selection stored in a GridSelection class
		private GridSelection selection = null;
		public GridSelection Selection
		{
			get { return selection; }
		}

		// selectionStart stores the start position
		// from which to select if a range selection will
		// be preformed (selecting with shift)
		private int selectionStart = -1;

		// Grid selected index
		private int selectedIndex = -1;
		public int SelectedIndex
		{
			get
			{
				if (selectedIndex >= GetRowCount())
				{
					selectedIndex = -1;
					selectionStart = -1;
				}
				return selectedIndex;
			}
			set
			{
				if (value != selectedIndex)
				{
					selectedIndex = value;
					Refresh();
				}
			}
		}

		#endregion

		#region Selection Functions

		// SelectRows adds or sets the selecion of the grid
		private bool SelectRows(int from, int to, bool add)
		{
			if (!OnSelectionChanging(from, to, add))
				return false;

			// Row count might change in OnSelectionChanging
			int rows = source.GetRowCount();
			if (to >= rows)
				return false;

			// If not adding selection...
			if (!add)
				//... clearing selection first
				selection.Clear();

			// adding the given selection range
			if (from < to)
				selection.Add(from, to);
			else
				selection.Add(to, from);

			// Notifing of selection change
			OnSelectionChanged();
			Refresh();
			return true;
		}

		// Single row selection
		private bool SelectRow(int row, bool select, bool add)
		{
			if (select != selection[row])
			{
				if (!OnSelectionChanging(row, row, add))
					return false;

				// If not adding selection...
				if (!add)
					//... clearing selection first
					selection.Clear();

				selection[row] = select;

				// Notifing of selection change
				OnSelectionChanged();
				Refresh();
			}

			return true;
		}

		private bool selecting = false;
		private void SelectStart(int row, bool ctrl, bool shift)
		{
			CancelEdit();
			if (_selectionMode == SelectionMode.None)
				return;

			int lastIndex = selectedIndex;
			selectedIndex = row;
			if (_selectionMode == SelectionMode.One)
			{
				selecting = SelectRow(row, true, false);
			}
			else if (_selectionMode == SelectionMode.MultiSimple)
			{
				selecting = SelectRow(row, !selection[row], true);
			}
			else
			{
				if (shift && selectionStart != -1)
				{
					selecting = SelectRows(selectionStart, row, ctrl);
				}
				else if (ctrl)
				{
					selectionStart = row;
					selecting = SelectRow(row, !selection[row], true);
					selecting = false;
				}
				else
				{
					selectionStart = row;
					selecting = SelectRows(row, row, false);
				}
			}

			if (!selecting)
			{
				selectedIndex = lastIndex;
				Refresh();
			}
		}

		private void SelectMove(int row, bool ctrl, bool shift)
		{
			if (_selectionMode == SelectionMode.One)
			{
				selectedIndex = row;
				SelectRow(row, true, false);
			}
			else if (_selectionMode == SelectionMode.MultiExtended)
			{
				selectedIndex = row;
				if (selectionStart == -1)
					selectionStart = row;
				SelectRows(selectionStart, row, ctrl);
			}
		}

		private void SelectEnd()
		{
			if (selecting)
			{
				selecting = false;
				StopAutoScroll();
			}
		}

		private void StopAutoScroll()
		{
			scrollDirection = 0;
			autoScrollTimer.Stop();
		}

		// ClearSelection clears the selection in the grid
		private bool ClearSelection()
		{
			CancelEdit();

			if (!OnSelectionChanging(-1, -1, false))
				return false;

			selectedIndex = -1;
			selectionStart = -1;
			selection.Clear();
			// Notifing of selection change
			OnSelectionChanged();
			Refresh();
			return true;
		}


		public void SelectRow(int row)
		{
			if (row == -1)
				selection.Clear();
			else
				selection.Set(row, row);

			selectedIndex = row;
			selectionStart = row;
			OnSelectionChanged();
			Refresh();
		}

		/// <summary>
		/// Check selection match source
		/// </summary>
		public void RefreshSelection()
		{
			if (source != null)
			{
				if (selection.RangeCount > 0)
				{
					int rowCount = source.GetRowCount();
					if (selection.RangeAt(selection.RangeCount - 1).Last >= rowCount)
					{
						if (selectedIndex >= rowCount)
							selectedIndex = -1;
						if (selectionStart >= rowCount)
							selectionStart = -1;
						selection.Remove(rowCount, Int32.MaxValue);
						OnSelectionChanged();
					}
				}
			}
			else
			{
				if (selection.Size > 0)
				{
					selectedIndex = -1;
					selectionStart = -1;
					selection.Clear();
					// Notifing of selection change
					OnSelectionChanged();
				}
			}
		}

		#endregion

		#endregion

		private GridGroupCollection groups;

		public GridGroupCollection Groups
		{
			get { return groups; }
		}

		private void GroupChanged(object sender, CollectionEventArgs e)
		{
			if (e.EventType == CollectionEventType.Insert)
			{
				if (groups.Count > 1 && _groupingStyle == GridGroupingStyle.Grid)
					headerheight += (headerheight / (groups.Count - 1));
				((GridGroup)e.New).RowHeight = defaultRowHeight;
			}
			else if (e.EventType == CollectionEventType.Remove)
			{
				if (groups.Count == 0)
				{
					groups.Add();
				}
				else if (_groupingStyle == GridGroupingStyle.Grid)
				{
					headerheight -= (headerheight / (groups.Count + 1));
				}
			}
		}

		private void OnVisibleChange()
		{
			groupColumnsWidth = null;
		}

		public GridColumnCollection Columns
		{
			get { return groups[0].Columns; }
		}

		public int GetFieldColumn(int field, int group)
		{
			for (int c = 0; c < groups[group].Columns.Count; c++)
			{
				if (groups[group].Columns[c].Field == field)
					return c;
			}

			return -1;
		}

		public int GetFieldColumn(int field)
		{
			return GetFieldColumn(field, 0);
		}

		#region Constructor

		public Grid()
		{
			groups = new GridGroupCollection(this);
			groups.Add();
			groups.Changed += new CollectionEventHandler(GroupChanged);

			expandedRows = new RangeArray();
			expandedRows.Changed += new EventHandler(ExpandedRowsChanged);
			rowsIndexes = new ArrayList();

			_expandOnDoubleClick = true;

			checkedRows = new RangeArray();

			_showRowNumber = false;
			_showCheckBoxes = false;

			selection = new GridSelection(this);

			source = new GridDefaultSource();
			source.SetGrid(this);

			scroll = new VScrollBar();
			scroll.Visible = false;
			scroll.Scroll += new ScrollEventHandler(ScrollScroll);
			scroll.ValueChanged += new EventHandler(ScrollValueChanged);
			Controls.Add(scroll);
			scrollVisible = false;

			Size = new Size(150, 100);

			headerheight = Font.Height + 4;
			defaultRowHeight = Font.Height + 8;
			groups[0].RowHeight = defaultRowHeight;

			SetStyle(ControlStyles.ResizeRedraw, true);

			_style = new GridDrawStyle();
			ResetGridStyle();
			_style.StyleChanged += new EventHandler(GridStyleChanged);

			_row = 0;
			_visibleRow = 0;

			autoScrollTimer = new Timer();
			autoScrollTimer.Tick += new EventHandler(ScrollTick);
			autoScrollTimer.Interval = 10;

			toolTip = new ToolTip();
			toolTip.InitialDelay = 1000;
			toolTip.AutoPopDelay = 3000;

			ResetGrid();
		}

		#endregion

		#region Hit Test

		/// <summary>
		/// Hit test implementation.
		/// The HitTest function receives control coordinates
		/// and returns where those hit on the control
		/// </summary>


		public enum HitTestType
		{
			None,		// Space between last row and control bounds
			// or ident header
			Border,		// On the border of the control
			Header,		// Inside a header column
			HeaderSize,	// Between the bottom side of the header to the top size of the
			// first row
			Expand,		// On the expand button
			CheckBox,	// On the check box
			Row,		// On ident space outside the expand button or check box
			Cell,		// Inside a cell
			ColumnSize,	// Between two header columns
			RowSize		// Between two rows
		}

		private sealed class HitTestInfo
		{
			private HitTestType _type;
			public HitTestType HitTestType
			{
				get { return _type; }
			}

			private Point _cell;
			public Point Cell
			{
				get { return _cell; }
			}
			public int Column
			{
				get { return _cell.X; }
			}
			public int Row
			{
				get { return _cell.Y; }
			}

			private Rectangle _bounds;
			// The bounds of the hitted item
			public Rectangle Bounds
			{
				get { return _bounds; }
			}
			private Size _offset;
			// The offset from the point of hit to the bounds
			// upper left corner
			public Size Offset
			{
				get { return _offset; }
			}

			public HitTestInfo(HitTestType type, Point cell,
				Rectangle bounds, Size offset)
			{
				_type = type;
				_cell = cell;
				_bounds = bounds;
				_offset = offset;

				//				Debug.WriteLine(this);
			}

			public HitTestInfo(HitTestType type, Point cell)
				: this(type, cell, new Rectangle(0, 0, 0, 0), new Size(0, 0))
			{
			}

			public HitTestInfo(HitTestType type, Rectangle bounds, Size offset)
				: this(type, new Point(-1, -1), bounds, offset)
			{
			}

			public HitTestInfo(HitTestType type)
				: this(type, new Point(-1, -1))
			{
			}

			public HitTestInfo()
				: this(HitTestType.None)
			{
			}

			public override string ToString()
			{
				return _type.ToString() + " " + _cell.ToString() + " " +
					_bounds.ToString() + " " + _offset.ToString();
			}

		}

		private HitTestInfo HitTest(Point position)
		{
			return HitTest(position.X, position.Y);
		}

		private HitTestInfo HitTest(int x, int y)
		{
			Point rp = ToGridPoint(x, y);
			int rx = rp.X;
			int ry = rp.Y;

			if (rx < 0 || ry < 0 || rx > rectGrid.Width ||
				ry > rectGrid.Height)
			{
				return new HitTestInfo(HitTestType.Border);
			}

			if (groupColumnsWidth == null)
				CalculateColumnWidth();

			// Now x and y are rectGrid relative and left oriented

			// If point of header...
			if (ry < headerheight + sizeheight)
			{
				// ... if on header line
				if (ry > headerheight - sizeheight)
				{
					// ... it is header sizing
					return new HitTestInfo(HitTestType.HeaderSize,
						new Rectangle(0, 0, rectGrid.Width, headerheight),
						new Size(rx, ry));
				}

				int gh, group;
				if (_groupingStyle == GridGroupingStyle.Grid)
				{
					// Getting height of one group in header
					gh = headerheight / groups.Count;
					// Getting the group in y position
					group = ry / gh;
					if (group >= groups.Count)
						group = groups.Count - 1;
				}
				else
				{
					gh = headerheight;
					group = groups.Count - 1;
				}
				// Getting ident size of the group
				int ident = GetGroupIdent(group);

				// If x on ident ...
				if (rx < ident)
				{
					// ... hit test is none
					return new HitTestInfo(HitTestType.None);
				}
				else
				{
					// ... otherwise finding the column
					int width = rectGrid.Width - ident;
					GridGroup g = groups[group];
					int left = ident;

					Rectangle columnRect;

					if (_showRowNumber || _showCheckBoxes)
					{
						if (rx < left + sizewidth && rx > left - sizewidth)
						{
							columnRect = new Rectangle(left - rowNumberIdent,
									gh * group, rowNumberIdent, group == groups.Count - 1 ? headerheight - (gh * group) : gh);
							//... this is row number column sizing
							return new HitTestInfo(HitTestType.ColumnSize,
								new Point(-1, group),
								columnRect,
								new Size(rx - columnRect.Left, y - columnRect.Y));
						}
					}

					float[] columnsWidth = groupColumnsWidth[group];
					int columnWidth;

					for (int column = 0; column < g.Columns.Count - 1; column++)
					{
						GridColumn c = g.Columns[column];
						if (c.Visible)
						{
							// Getting column width
							columnWidth = (int)((float)width * columnsWidth[column]);
							// If x on column width...
							if (rx < left + columnWidth + sizewidth)
							{
								columnRect = new Rectangle(left, gh * group,
									columnWidth, group == groups.Count - 1 ? headerheight - (gh * group) : gh);
								// ... if between columns ...
								if (rx > left + columnWidth - sizewidth)
								{
									//... this is column sizing
									return new HitTestInfo(HitTestType.ColumnSize,
										new Point(column, group),
										columnRect,
										new Size(rx - left, y - columnRect.Y));
								}

								// ... otherwise on header
								return new HitTestInfo(HitTestType.Header,
									new Point(column, group), columnRect,
									new Size(rx - left, y - columnRect.Y));
							}
							left += columnWidth;
						}
					}

					columnRect = new Rectangle(left, gh * group,
						rectGrid.Width - left,
						group == groups.Count - 1 ? headerheight - (gh * group) : gh);
					// ... didn't find the column so it is the last visible
					// column
					return new HitTestInfo(HitTestType.Header,
						new Point(g.LastVisible, group), columnRect,
						new Size(rx - left, y - columnRect.Y));
				}
			}
			else
			{
				// Getting the row and group according to original y
				int row, group;
				int oy = y;
				if (GetRowAt(ref oy, out row, out group))
				{
					int height = groups[group].RowHeight;
					int top = y - oy - rectGrid.Y;
					// Checking for row sizing
					if (oy > height - sizeheight)
					{
						return new HitTestInfo(HitTestType.RowSize,
							new Point(-1, row),
							new Rectangle(0, top, rectGrid.Width, height),
							new Size(rx, oy));
					}

					int ident;

					// Checking for list grouping style expand box
					if (_groupingStyle == GridGroupingStyle.List &&
						group < groups.Count)
					{
						int off = (height - 8) / 2;
						ident = (group * 12) + 4;
						if (rx >= ident && rx <= ident + 8 && oy >= off && oy <= off + 8)
						{
							return new HitTestInfo(HitTestType.Expand, new Point(-1, row));
						}
					}

					ident = GetGroupIdent(group);

					// If x is on ident ...
					if (rx < ident)
					{
						int m = height / 2;
						if (_showCheckBoxes || group < groups.Count - 1)
						{
							if (rx < 13 && oy > m - 6 && oy < m + 6)
							{
								return new HitTestInfo(
									_showCheckBoxes ? HitTestType.CheckBox : HitTestType.Expand,
									new Point(-1, row));
							}
						}
						return new HitTestInfo(HitTestType.Row,
							new Point(-1, row),
							new Rectangle(0, top, rectGrid.Width, height),
							new Size(rx, oy));
					}
					else
					{
						// ... otherwise finding the column
						int width = rectGrid.Width - ident;
						GridGroup g = groups[group];
						int left = ident;
						float[] columnsWidth = groupColumnsWidth[group];
						int columnWidth;

						for (int column = 0; column < g.Columns.Count - 1; column++)
						{
							GridColumn c = g.Columns[column];
							if (c.Visible)
							{
								// Getting column width
								columnWidth = (int)((float)width * columnsWidth[column]);
								// If x on column width...
								if (rx < left + columnWidth)
								{
									return new HitTestInfo(HitTestType.Cell,
										new Point(column, row),
										new Rectangle(left, top, columnWidth, height),
										new Size(rx - left, oy));
								}
								left += columnWidth;
							}
						}

						// ... didn't find the column so it is the last visible
						// column
						return new HitTestInfo(HitTestType.Cell,
							new Point(g.LastVisible, row),
							new Rectangle(left, top, rectGrid.Width - left, height),
							new Size(rx - left, oy));
					}
				}
			}

			// returns 'none'
			return new HitTestInfo();
		}

		private bool GetRowAt(ref int y, out int row, out int group)
		{
			if (_borderStyle == BorderStyle.Fixed3D)
				y -= 2;
			else if (_borderStyle == BorderStyle.FixedSingle)
				y -= 1;

			row = -1;
			group = -1;

			y -= headerheight;
			if (y < 0)
				return false;

			int bottom = rectGrid.Height - headerheight;

			int rows = source.GetRowCount();
			int r = _row;
			int g;
			int lastGroup = 0;
			bool skipgroup = false;

			while (y < bottom && r < rows)
			{
				g = source.GetGroup(r);
				if (!skipgroup || g <= lastGroup)
				{
					if (y < groups[g].RowHeight)
					{
						row = r;
						group = g;
						return true;
					}

					y -= groups[g].RowHeight;
					skipgroup = !expandedRows[r];
					lastGroup = g;
				}
				r++;
			}

			row = 1;

			return false;
		}

		#endregion

		#region Column Resize

		public class ColumnEventArgs
		{
			private int _column;

			public int Column { get { return _column; } }

			public ColumnEventArgs(int column)
			{
				_column = column;
			}
		}

		public delegate void ColumnEventHandler(object sender, ColumnEventArgs e);

		public event ColumnEventHandler ColumnResized;

		private static readonly int minimumColumnWidth = 10;

		private class ColumnResize
		{
			public readonly int Group;
			public readonly int Column;
			public readonly int Start;
			public readonly int Offset;

			public ColumnResize(int group, int column,
				int start, int offset)
			{
				Group = group;
				Column = column;
				Start = start;
				Offset = offset;
			}
		}

		private ColumnResize columnResizing = null;

		private void ResizeColumnStart(int group, int column,
			int start, int offset)
		{
			if (columnResizing == null)
			{
				if (column == -1)
				{
					columnResizing = new ColumnResize(group, -1, start, offset - rowNumberIdent);
				}
				else
				{
					int ident = GetGroupIdent(group);
					int width = rectGrid.Width - ident;
					float[] columnsWidth = groupColumnsWidth[group];
					int columnWidth = (int)((float)width * columnsWidth[column]);
					columnResizing = new ColumnResize(group, column, start, offset - columnWidth);
				}
				Cursor.Current = Cursors.VSplit;
			}
		}

		private void ResizeColumnMove(Point moveTo)
		{
			Cursor.Current = Cursors.VSplit;

			int x = moveTo.X;

			if (x < columnResizing.Start + minimumColumnWidth)
				x = columnResizing.Start + minimumColumnWidth;

			SetColumnWidth(columnResizing.Group, columnResizing.Column, x - columnResizing.Start - columnResizing.Offset);

			if (editControl != null)
			{
				ResetEditControl();
			}

			Refresh();
		}

		private void ResizeColumnEnd()
		{
			if (columnResizing != null)
			{
				int column = columnResizing.Column;
				columnResizing = null;
				if (ColumnResized != null)
					ColumnResized(this, new ColumnEventArgs(column));
			}
		}

		#endregion

		#region Header Resize

		public event EventHandler HeaderResized;

		private static readonly int minimumHeaderHeight = 10;

		private class HeaderResize
		{
			public readonly int Offset;

			public HeaderResize(int offset)
			{
				Offset = offset;
			}
		}

		private HeaderResize headerResizing = null;

		private void ResizeHeaderStart(int offset)
		{
			if (headerResizing == null)
			{
				headerResizing = new HeaderResize(offset - headerheight);
				Cursor.Current = Cursors.HSplit;
			}
		}

		private void ResizeHeaderMove(Point moveTo)
		{
			Cursor.Current = Cursors.HSplit;

			int newHeight = moveTo.Y - headerResizing.Offset;
			if (newHeight < minimumHeaderHeight)
				newHeight = minimumHeaderHeight;
			if (newHeight > rectGrid.Height / 2)
				newHeight = rectGrid.Height / 2;

			headerheight = newHeight;

			Refresh();
		}

		private void ResizeHeaderEnd()
		{
			headerResizing = null;
			if (HeaderResized != null)
				HeaderResized(this, EventArgs.Empty);
		}

		#endregion

		#region Row Resize

		public class GroupEventArgs
		{
			private int _group;

			public int Group { get { return _group; } }

			public GroupEventArgs(int group)
			{
				_group = group;
			}
		}

		public delegate void GroupEventHandler(object sender, GroupEventArgs e);

		public event GroupEventHandler RowResized;

		private static readonly int minimumRowHeight = 10;

		private class RowResize
		{
			public readonly int Group;
			public readonly int Start;
			public readonly int Offset;

			public readonly int Row;
			public readonly bool Expanded;
			public readonly bool HasChildren;

			public int RowHeight;

			public RowResize(int group, int start, int offset,
				int row, bool expanded, bool hasChildren)
			{
				Group = group;
				Start = start;
				Offset = offset;
				Row = row;
				Expanded = expanded;
				HasChildren = hasChildren;
			}
		}

		private RowResize rowResizing = null;

		private void ResizeRowStart(int row, int start, int offset)
		{
			if (rowResizing == null)
			{
				if (editControl != null)
					editControl.Visible = false;

				int group = source.GetGroup(row);
				int rowheight = groups[group].RowHeight;
				bool hc = false;
				if (row < source.GetRowCount() - 1)
				{
					if (source.GetGroup(row + 1) > group)
						hc = true;
				}

				rowResizing = new RowResize(group, start,
					offset - rowheight, row, expandedRows[row], hc);
				rowResizing.RowHeight = groups[group].RowHeight;
				Cursor.Current = Cursors.HSplit;
			}
		}

		private void ResizeRowMove(Point moveTo)
		{
			Cursor.Current = Cursors.HSplit;

			rowResizing.RowHeight = moveTo.Y - rowResizing.Offset - rowResizing.Start;
			if (rowResizing.RowHeight < minimumRowHeight)
				rowResizing.RowHeight = minimumRowHeight;
			if (rowResizing.RowHeight > rectGrid.Height / 2)
				rowResizing.RowHeight = rectGrid.Height / 2;

			Graphics g = Graphics.FromImage(bitMap);
			DrawStruct ds = new DrawStruct(g, rectGrid.Size);
			ds.Group = rowResizing.Group;
			ds.Row = rowResizing.Row;
			ds.Rectangle = new Rectangle(0, 0, rectGrid.Width, rowResizing.RowHeight);
			ds.State = GridDrawState.Focused;
			if (selection[ds.Row])
			{
				if (ds.Group < groups.Count - 1 && _groupingStyle == GridGroupingStyle.List)
					ds.Style = _style.SelectedTitle;
				else
					ds.Style = _style.Selected;
				ds.State |= GridDrawState.Selected;
			}
			else
			{
				if (ds.Group == groups.Count - 1)
					ds.Style = _style.Normal;
				else if (_groupingStyle == GridGroupingStyle.Grid)
					ds.Style = _style.Group;
				else
					ds.Style = _style.Title;
			}

			if (ds.Group == groups.Count - 1 || _groupingStyle == GridGroupingStyle.Grid)
				DrawRow(ds, rowResizing.Expanded, rowResizing.HasChildren);
			else
				DrawListHeader(ds, rowResizing.Expanded, rowResizing.HasChildren);

			Graphics cg = CreateGraphics();
			cg.DrawImage(bitMap,
				new Rectangle(rectGrid.X, rectGrid.Y + rowResizing.Start, rectGrid.Width, rowResizing.RowHeight),
				new Rectangle(0, 0, rectGrid.Width, rowResizing.RowHeight),
				GraphicsUnit.Pixel);
			cg.Dispose();

			Invalidate(new Rectangle(rectGrid.X, rectGrid.Y + rowResizing.Start + rowResizing.RowHeight,
				rectGrid.Width, rectGrid.Bottom - (rowResizing.Start + rowResizing.RowHeight)));
		}

		private void ResizeRowEnd()
		{
			if (rowResizing != null)
			{
				int group = rowResizing.Group;
				groups[rowResizing.Group].RowHeight = rowResizing.RowHeight;
				if (editControl != null)
					ResetEditControl();
				rowResizing = null;

				if (RowResized != null)
					RowResized(this, new GroupEventArgs(group));
			}
		}

		#endregion

		#region Column Move

		public class ColumnMoveEventArgs
		{
			private int _column1;
			private int _column2;

			public int Column1 { get { return _column1; } }
			public int Column2 { get { return _column2; } }

			public bool Continue;

			public ColumnMoveEventArgs(int column1, int column2)
			{
				_column1 = column1;
				_column2 = column2;
			}
		}

		public delegate void ColumnMoveEventHandler(object sender, ColumnMoveEventArgs e);

		public event ColumnMoveEventHandler ColumnMoved;

		private class ColumnMove
		{
			public readonly int Group;
			public readonly int Offset;

			public int Column;
			public bool Moved;

			public ColumnMove(int group, int column, int offset)
			{
				Group = group;
				Column = column;
				Offset = offset;
				Moved = false;
			}
		}

		private ColumnMove columnMoving = null;

		private void MoveColumnStart(int group, int column,
			int offset)
		{
			if (columnMoving == null)
			{
				columnMoving = new ColumnMove(group, column, offset);
			}
		}

		private void ReplaceColumns(int group, int column1, int column2)
		{
			// Removing column from the array will delete the widthes
			// and recalculate using default sizes
			// So storing the widthes aside
			float[][] tempWidth = groupColumnsWidth;

			GridColumn c = groups[group].Columns[column1];
			groups[group].Columns[column1] = groups[group].Columns[column2];
			groups[group].Columns[column2] = c;

			groupColumnsWidth = tempWidth;

			if (groupColumnsWidth == null)
			{
				CalculateColumnWidth();
			}
			else
			{
				float cw = groupColumnsWidth[group][column1];
				groupColumnsWidth[group][column1] = groupColumnsWidth[group][column2];
				groupColumnsWidth[group][column2] = cw;
			}

			if (ColumnMoved != null)
				ColumnMoved(this, new ColumnMoveEventArgs(column1, column2));

			Refresh();
		}

		private void MoveColumnMove(Point moveTo)
		{
			if (columnMoving == null)
				return;

			columnMoving.Moved = true;

			int x = moveTo.X - columnMoving.Offset;
			int ident = GetGroupIdent(columnMoving.Group);
			int width = rectGrid.Width - ident;
			int left = ident;
			float[] columnsWidth = groupColumnsWidth[columnMoving.Group];
			GridGroup group = groups[columnMoving.Group];
			int column = 0;
			while (column < group.Columns.Count)
			{
				if (group.Columns[column].Visible)
				{
					if (x <= left)
					{
						if (column != columnMoving.Column)
						{
							ReplaceColumns(columnMoving.Group, columnMoving.Column, column);
							columnMoving.Column = column;
						}

						break;
					}

					left += (int)((float)width * columnsWidth[column]);
				}

				column++;
			}

			int gh, top;
			if (_groupingStyle == GridGroupingStyle.Grid)
			{
				gh = headerheight / groups.Count;
				top = gh * columnMoving.Group;
			}
			else
			{
				gh = headerheight;
				top = 0;
			}
			int columnWidth = (int)((float)width * columnsWidth[columnMoving.Column]);
			Rectangle rect = CreateRect(x, top, columnWidth,
				columnMoving.Group == groups.Count - 1 ? headerheight - top : gh);

			rect.X += rectGrid.X;
			rect.Y += rectGrid.Y;

			StringFormat sf;
			if (RightToLeft == RightToLeft.Yes)
				sf = new StringFormat(StringFormatFlags.DirectionRightToLeft);
			else
				sf = new StringFormat();
			sf.LineAlignment = StringAlignment.Center;
			sf.Alignment = StringAlignment.Center;

			Region region = new Region(new Rectangle(0, 0, Size.Width, Size.Height));
			region.Exclude(rect);

			Graphics g = CreateGraphics();
			DrawButton(g, rect, groups[columnMoving.Group].Columns[columnMoving.Column].Title,
				sf);
			g.Dispose();

			Invalidate(region);
		}

		private void MoveColumnEnd()
		{
			columnMoving = null;
		}

		#endregion

		#region Mouse Events

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);

			// Focusing on grid
			if (!Focused)
				Focus();

			if (e.Button == MouseButtons.Left)
			{
				HitTestInfo hti = HitTest(e.X, e.Y);
				if (hti == null)
					return;
				switch (hti.HitTestType)
				{
					case (HitTestType.Expand):
						if (expandedRows != null)
							expandedRows[hti.Row] = !expandedRows[hti.Row];
						break;
					case (HitTestType.CheckBox):
						CheckRow(hti.Row);
						break;
					case (HitTestType.Row):
						SelectStart(hti.Row, (ModifierKeys & Keys.Control) != 0,
							(ModifierKeys & Keys.Shift) != 0);
						break;
					case (HitTestType.Cell):
						if (selection != null)
						{
							if (selection.Size == 1 && selection[hti.Row] && (ModifierKeys & Keys.Control) == 0)
							{
								Edit(hti.Row, hti.Column);
							}
							else
							{
								SelectStart(hti.Row, (ModifierKeys & Keys.Control) != 0,
									(ModifierKeys & Keys.Shift) != 0);
							}
						}
						if (source != null)
						{
							int group = source.GetGroup(hti.Row);
							if ((groups != null) && (groups[group] != null))
							{
								if (groups[group].Columns != null)
								{
									if (groups[group].Columns[hti.Column] != null)
									{
										groups[group].Columns[hti.Column].OnMouseDown(hti.Row,
											hti.Bounds, hti.Offset);
									}
								}
							}
						}
						break;
					case (HitTestType.ColumnSize):
						ResizeColumnStart(hti.Row, hti.Column,
							hti.Bounds.Location.X, hti.Offset.Width);
						break;
					case (HitTestType.HeaderSize):
						ResizeHeaderStart(hti.Offset.Height);
						break;
					case (HitTestType.Header):
						if (hti.Column != -1)
							MoveColumnStart(hti.Row, hti.Column, hti.Offset.Width);
						break;
					case (HitTestType.RowSize):
						ResizeRowStart(hti.Row, hti.Bounds.Y, hti.Offset.Height);
						break;
					case (HitTestType.None):
						ClearSelection();
						break;
				}
			}
			else if (e.Button == MouseButtons.Right)
			{
				HitTestInfo hti = HitTest(e.X, e.Y);

				switch (hti.HitTestType)
				{
					case (HitTestType.Row):
					case (HitTestType.Cell):
						if (selection != null)
						{
							if (!selection[hti.Row])
							{
								SelectStart(hti.Row, (ModifierKeys & Keys.Control) != 0,
									(ModifierKeys & Keys.Shift) != 0);
							}
						}
						break;
					default:
						ClearSelection();
						break;
				}
			}
		}

		private int lastTipRow = -1;
		private ToolTip toolTip = new ToolTip();
		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (selecting)
			{
				if (e.Button == MouseButtons.None)
				{
					ReleaseDrag();
					return;
				}
				int y = e.Y;
				int row;
				int group;
				if (GetRowAt(ref y, out row, out group))
				{
					StopAutoScroll();
					SelectMove(row, (ModifierKeys & Keys.Control) != 0, (ModifierKeys & Keys.Shift) != 0);
				}
				else
				{
					if (scrollVisible)
					{
						if (row == -1)
							scrollDirection = -1;
						else
							scrollDirection = 1;
						autoScrollTimer.Start();
					}
				}
			}
			else if (columnResizing != null)
			{
				ResizeColumnMove(ToGridPoint(e.X, e.Y));
			}
			else if (headerResizing != null)
			{
				ResizeHeaderMove(ToGridPoint(e.X, e.Y));
			}
			else if (rowResizing != null)
			{
				ResizeRowMove(ToGridPoint(e.X, e.Y));
			}
			else if (columnMoving != null)
			{
				MoveColumnMove(ToGridPoint(e.X, e.Y));
			}
			else
			{
				HitTestInfo hti = HitTest(e.X, e.Y);
				int row = -1;

				switch (hti.HitTestType)
				{
					case (HitTestType.ColumnSize):
						Cursor.Current = Cursors.VSplit;
						break;
					case (HitTestType.HeaderSize):
					case (HitTestType.RowSize):
						Cursor.Current = Cursors.HSplit;
						row = hti.Cell.Y;
						break;
					case (HitTestType.Cell):
					case (HitTestType.Row):
						row = hti.Cell.Y;
						break;
				}

				if (row != lastTipRow)
				{
					toolTip.SetToolTip(this, null);
					if (row != -1)
					{
						string tip = source.GetTip(row);
						if (tip != null)
							toolTip.SetToolTip(this, tip);
					}
					lastTipRow = row;
				}
			}


			base.OnMouseMove(e);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);

			if (e.Button == MouseButtons.Left)
			{
				// If column move start but not moved...
				if (columnMoving != null && !columnMoving.Moved)
				{
					//... it is a header click
					HeaderClick(columnMoving.Group, columnMoving.Column);
				}
			}

			ReleaseDrag();
		}

		// OnMouseWheel is called when the user moves the mouse wheel
		protected override void OnMouseWheel(MouseEventArgs e)
		{
			base.OnMouseWheel(e);

			if (!scrollVisible || editControl != null)
				return;

			// Scrolling the grid according to the mouse wheel

			int d = e.Delta / 120;

			int n = _visibleRow - d;

			if (n < 0)
				n = 0;
			else
			{
				int max = scroll.Maximum - scroll.LargeChange + 1;
				if (n > max)
					n = max;
			}

			ScrollTo(n);
		}

		protected override void OnDoubleClick(EventArgs e)
		{
			base.OnDoubleClick(e);

			if (_expandOnDoubleClick && selectedIndex != -1)
				expandedRows[selectedIndex] = !expandedRows[selectedIndex];
		}


		#endregion

		private void HeaderClick(int group, int column)
		{
			int[] sort = source.GetSort(group);
			if (sort != null && sort.Length == 1 &&
				groups[group].Columns[column].Field == sort[0])
				source.Sort(group, new int[] { groups[group].Columns[column].Field + Int32.MinValue });
			else
				source.Sort(group, new int[] { groups[group].Columns[column].Field });

			Refresh();
		}

		#region Edit

		private bool _editable = true;
		public bool Editable
		{
			get { return _editable; }
			set
			{
				if (_editable != value)
				{
					CancelEdit();
					_editable = value;
				}
			}
		}

		private bool CanEditRow()
		{
			return _editable && selectedIndex != -1 && selection.Size == 1;
		}

		private Control editControl = null;
		private Point editCell;
		public Point EditCell
		{
			get { return editCell; }
		}

		private void ResetEditControl()
		{
			if (editControl != null)
			{
				Rectangle bounds;
				editControl.Visible = GetCellBounds(editCell.Y, editCell.X, out bounds);
				editControl.Bounds = bounds;
			}
		}

		private void Edit(int row, int col)
		{
			if (_editable)
			{
				CancelEdit();
				if (source != null && groups != null)
				{
					int group = source.GetGroup(row);
					if (group >= 0 && group < groups.Count && groups[group] != null &&
						groups[group].Columns != null && groups[group].Columns[col] != null)
					{
						editControl = source.Edit(row, groups[group].Columns[col].Field);
						if (editControl != null)
						{
							editCell = new Point(col, row);
							Rectangle bounds;
							editControl.Visible = GetCellBounds(row, col, out bounds);
							editControl.Bounds = bounds;
							editControl.KeyDown += new KeyEventHandler(EditControlKeyDown);
							Controls.Add(editControl);
							Invalidate(new Rectangle(0, bounds.Top, Width, bounds.Height));
							editControl.Focus();
						}
					}
				}
			}
		}

		public void EditField(int row, int field)
		{
			CancelEdit();

			SelectRow(row);
			int column = GetFieldColumn(field);

			if (!RowInPage(row))
				ScrollToRow(row);

			Edit(row, column);
		}

		public void Edit()
		{
			// Checking if edit is enabled or already editing
			// and if a row is selected
			if (!CanEditRow() || editControl != null)
				return;

			// Trying to edit all columns
			int group = source.GetGroup(selectedIndex);
			for (int c = 0; c < groups[group].Columns.Count; c++)
			{
				Edit(selectedIndex, c);
				// If edit of column succeeded edit is done
				if (editControl != null)
					return;
			}
		}

		public void EditNext()
		{
			if (!CanEditRow())
				return;

			int row;
			int col;
			bool focused = false;

			if (editControl == null)
			{
				if (selectedIndex == -1)
					return;
				row = selectedIndex;
				col = 0;
			}
			else
			{
				focused = editControl.Focused;
				row = editCell.Y;
				col = editCell.X + 1;
			}

			EndEdit();

			int group = source.GetGroup(row);
			// Trying to edit the current row next columns
			for (int c = col; c < groups[group].Columns.Count; c++)
			{
				Edit(selectedIndex, c);
				// If edit of column succeeded edit is done
				if (editControl != null)
					return;
			}

			if (focused)
				Focus();
		}

		public void EditPrevious()
		{
			if (!CanEditRow())
				return;

			int row;
			int col;
			int group;
			bool focused = false;

			if (editControl == null)
			{
				if (selectedIndex == -1)
					return;
				row = selectedIndex;
				group = source.GetGroup(row);
				col = groups[group].Columns.Count - 1;
			}
			else
			{
				focused = editControl.Focused;
				row = editCell.Y;
				group = source.GetGroup(row);
				col = editCell.X - 1;
			}

			EndEdit();

			// Trying to edit the current row next columns
			for (int c = col; c >= 0; c--)
			{
				Edit(selectedIndex, c);
				// If edit of column succeeded edit is done
				if (editControl != null)
					return;
			}

			if (focused)
				Focus();
		}

		private void EndEdit()
		{
			editControl.KeyDown -= new KeyEventHandler(EditControlKeyDown);
			source.EditEnded(editControl);
			Controls.Remove(editControl);
			editControl = null;
		}

		public void CancelEdit()
		{
			if (editControl != null)
			{
				bool focused = editControl.Focused;
				EndEdit();
				if (focused)
					Focus();
			}
		}

		private void EditControlKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Handled)
				return;
			if (e.Modifiers == Keys.None)
			{
				if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
				{
					EditNext();
					e.Handled = true;
				}
				else if (e.KeyCode == Keys.Escape)
				{
					CancelEdit();
					e.Handled = true;
				}
			}
			else if (e.Modifiers == Keys.Shift)
			{
				if (e.KeyCode == Keys.Tab)
				{
					EditPrevious();
					e.Handled = true;
				}
			}
		}



		#endregion

		protected override void OnLostFocus(EventArgs e)
		{
			ReleaseDrag();
			base.OnLostFocus(e);
		}

		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus(e);
			if (selectedIndex == -1 && source.GetRowCount() > 0)
				selectedIndex = 0;
			Refresh();
		}

		private void ReleaseDrag()
		{
			SelectEnd();
			ResizeColumnEnd();
			ResizeHeaderEnd();
			ResizeRowEnd();
			MoveColumnEnd();
			Refresh();
		}


		#region Grid Source

		private IGridSource source;
		public IGridSource Source
		{
			get
			{
				return source;
			}
			set
			{
				if (source != value)
				{
					if (source != null)
						source.SetGrid(null);

					source = value;

					if (source == null)
						source = new GridDefaultSource();
					source.SetGrid(this);

					OnSourceChanged();
				}
			}
		}

		private void OnSourceChanged()
		{
			RefreshSource();
		}

		public void RefreshSource()
		{
			//Sport.UI.MessageBox.Show("cancel edit...");
			CancelEdit();
			_row = 0;
			_visibleRow = 0;
			//Sport.UI.MessageBox.Show("clearing rows...");
			checkedRows.Clear();
			//Sport.UI.MessageBox.Show("reset rows indices...");
			ResetRowsIndexes();
			//Sport.UI.MessageBox.Show("reset grid...");
			ResetGrid();
			//Sport.UI.MessageBox.Show("done!");
		}

		#endregion

		#region Grid Drawing

		private BorderStyle _borderStyle = BorderStyle.Fixed3D;
		public BorderStyle BorderStyle
		{
			get { return _borderStyle; }
			set
			{
				if (_borderStyle != value)
				{
					_borderStyle = value;
					ResetGrid();
				}
			}
		}

		private int rowNumberIdent = defaultRowNumberIdent;
		private bool _showRowNumber;
		public bool ShowRowNumber
		{
			get { return _showRowNumber; }
			set
			{
				// Resetting ident size
				if (value && !_showRowNumber)
				{
					if (_showCheckBoxes)
					{
						if (rowNumberIdent < defaultRowNumberIdent + checkBoxIdent)
							rowNumberIdent = defaultRowNumberIdent + checkBoxIdent;
					}
					else
					{
						if (rowNumberIdent < defaultRowNumberIdent)
							rowNumberIdent = defaultRowNumberIdent;
					}
				}
				_showRowNumber = value;
			}
		}

		private bool _showCheckBoxes;
		public bool ShowCheckBoxes
		{
			get { return _showCheckBoxes; }
			set
			{
				// Resetting ident size
				if (value && !_showCheckBoxes)
				{
					if (_showRowNumber)
					{
						if (rowNumberIdent < defaultRowNumberIdent + checkBoxIdent)
							rowNumberIdent = defaultRowNumberIdent + checkBoxIdent;
					}
					else
					{
						if (rowNumberIdent < checkBoxIdent)
							rowNumberIdent = checkBoxIdent;
					}
				}

				_showCheckBoxes = value;
			}
		}

		private bool _horizontalLines = true;
		public bool HorizontalLines
		{
			get { return _horizontalLines; }
			set
			{
				_horizontalLines = value;
				Refresh();
			}
		}

		private bool _verticalLines = true;
		public bool VerticalLines
		{
			get { return _verticalLines; }
			set
			{
				_verticalLines = value;
				Refresh();
			}
		}

		private GridDrawStyle _style;
		public GridDrawStyle Style
		{
			get { return _style; }
		}

		private bool settingstyle = false;
		private void GridStyleChanged(object sender, EventArgs e)
		{
			if (!settingstyle)
			{
				settingstyle = true;
				ResetGridStyle();

				Refresh();
				settingstyle = false;
			}
		}

		private void ResetGridStyle()
		{
			if (_style.Normal.Background == null)
				_style.Normal.Background = SystemBrushes.Window;
			if (_style.Normal.Foreground == null)
				_style.Normal.Foreground = SystemBrushes.WindowText;
			if (_style.Normal.Font == null)
				_style.Normal.Font = Font;
			if (_style.Group.Background == null)
				_style.Group.Background = SystemBrushes.Control;
			if (_style.Group.Foreground == null)
				_style.Group.Foreground = SystemBrushes.WindowText;
			if (_style.Group.Font == null)
				_style.Group.Font = Font;
			if (_style.Title.Background == null)
				_style.Title.Background = SystemBrushes.Window;
			if (_style.Title.Foreground == null)
				_style.Title.Foreground = SystemBrushes.WindowText;
			if (_style.Title.Font == null)
			{
				Font font = Font;
				_style.Title.Font = new Font(font.FontFamily, font.Size, font.Style | FontStyle.Bold | FontStyle.Underline, font.Unit);
			}
			if (_style.SelectedTitle.Background == null)
				_style.SelectedTitle.Background = SystemBrushes.Highlight;
			if (_style.SelectedTitle.Foreground == null)
				_style.SelectedTitle.Foreground = SystemBrushes.HighlightText;
			if (_style.SelectedTitle.Font == null)
			{
				Font font = Font;
				_style.SelectedTitle.Font = new Font(font.FontFamily, font.Size, font.Style | FontStyle.Bold | FontStyle.Underline, font.Unit);
			}
			if (_style.Selected.Background == null)
				_style.Selected.Background = SystemBrushes.Highlight;
			if (_style.Selected.Foreground == null)
				_style.Selected.Foreground = SystemBrushes.HighlightText;
			if (_style.Selected.Font == null)
				_style.Selected.Font = Font;
			if (_style.Unfocused.Background == null)
				_style.Unfocused.Background = SystemBrushes.Window;
			if (_style.Unfocused.Foreground == null)
				_style.Unfocused.Foreground = SystemBrushes.WindowText;
			if (_style.Unfocused.Font == null)
				_style.Unfocused.Font = Font;
			if (_style.UnfocusedSelected.Background == null)
				_style.UnfocusedSelected.Background = SystemBrushes.Control;
			if (_style.UnfocusedSelected.Foreground == null)
				_style.UnfocusedSelected.Foreground = SystemBrushes.WindowText;
			if (_style.UnfocusedSelected.Font == null)
				_style.UnfocusedSelected.Font = Font;
		}

		protected override void OnFontChanged(EventArgs e)
		{
			base.OnFontChanged(e);

			settingstyle = true;
			Font font = Font;
			_style.Normal.Font = font;
			_style.Group.Font = font;
			_style.Title.Font = new Font(font.FontFamily, font.Size, font.Style | FontStyle.Bold | FontStyle.Underline, font.Unit);
			_style.SelectedTitle.Font = new Font(font.FontFamily, font.Size, font.Style | FontStyle.Bold | FontStyle.Underline, font.Unit);
			_style.Selected.Font = font;
			_style.Unfocused.Font = font;
			_style.UnfocusedSelected.Font = font;
			settingstyle = false;
			Refresh();
		}


		private void GridUnfocusedStyleChanged(object sender, EventArgs e)
		{
			Refresh();
		}

		private Point ToGridPoint(int x, int y)
		{
			if (_borderStyle == BorderStyle.FixedSingle)
			{
				x--;
				y--;
			}
			else if (_borderStyle == BorderStyle.Fixed3D)
			{
				x -= 2;
				y -= 2;
			}

			if (RightToLeft == RightToLeft.Yes)
			{
				if (scrollVisible)
					x -= scroll.Width;
				x = rectGrid.Width - x;
			}

			return new Point(x, y);
		}

		private Rectangle CreateRect(int x, int y, int width, int height)
		{
			if (RightToLeft == RightToLeft.Yes)
			{
				return new Rectangle(rectGrid.Width - x - width, y, width, height);
			}

			return new Rectangle(x, y, width, height);
		}

		private Point[] CreatePoints(Point[] points)
		{
			if (RightToLeft == RightToLeft.Yes)
			{
				Point[] pts = new Point[points.Length];
				for (int p = 0; p < points.Length; p++)
				{
					pts[p] = new Point(rectGrid.Width - points[p].X - 1,
						points[p].Y);
				}

				return pts;
			}

			return points;
		}

		private Point CreatePoint(Point point)
		{
			if (RightToLeft == RightToLeft.Yes)
			{
				return new Point(rectGrid.Width - point.X - 1,
					point.Y);
			}

			return point;
		}

		private Point CreatePoint(int x, int y)
		{
			if (RightToLeft == RightToLeft.Yes)
			{
				return new Point(rectGrid.Width - x, y);
			}

			return new Point(x, y);
		}

		private void DrawHeaderBlank(DrawStruct ds,
			int x, int y, int width, int height)
		{
			ds.Graphics.FillRectangle(
				SystemBrushes.Control,
				CreateRect(x, y, width, height));
			ds.Graphics.DrawLine(
				SystemPens.ControlDark,
				CreatePoint(x, y + height - 1),
				CreatePoint(x + width, y + height - 1));
		}

		private void DrawButton(Graphics g, Rectangle rect, string text,
			StringFormat sf)
		{
			ControlPaint.DrawButton(g, rect, ButtonState.Normal);
			if (text != null)
			{
				g.DrawString(text, Font, SystemBrushes.WindowText, rect, sf);
			}
		}

		private void DrawHeaderControl(DrawStruct ds,
			int x, int y, int width, int height, string text,
			int sort)
		{
			Rectangle rect = CreateRect(x, y, width, height);
			DrawButton(ds.Graphics, rect, text, ds.StringFormat);
			if (sort > 0)
			{
				int t = y + (height / 2) - 2;
				t -= sort - 1;

				while (sort > 1)
				{
					ds.Graphics.DrawLine(SystemPens.ControlDark,
						CreatePoint(x + width - 10, t),
						CreatePoint(x + width - 4, t));
					sort--;
					t += 2;
				}

				FillPolygon(ds, SystemBrushes.ControlDark,
					new Point[] {
									new Point(x + width - 12, t),
									new Point(x + width - 5, t),
									new Point(x + width - 8, t + 4)
								});
			}
			else if (sort < 0)
			{
				int t = y + (height / 2) - 2;
				t += sort + 1;

				int tt = t + 5;

				while (sort < -1)
				{
					ds.Graphics.DrawLine(SystemPens.ControlDark,
						CreatePoint(x + width - 10, tt),
						CreatePoint(x + width - 4, tt));
					sort++;
					tt += 2;
				}

				FillPolygon(ds, SystemBrushes.ControlDark,
					new Point[] {
									new Point(x + width - 12, t + 4),
									new Point(x + width - 4, t + 4),
									new Point(x + width - 8, t - 1)
								});
			}
		}

		private void FillRectangle(DrawStruct ds,
			int x, int y, int width, int height, Brush brush)
		{
			//SolidBrush b=new SolidBrush(Color.Red);
			Rectangle rect = CreateRect(x, y, width, height);
			ds.Graphics.FillRectangle(brush, rect);
			//ds.Graphics.FillRectangle(b, rect);
		}

		private void FillPolygon(DrawStruct ds, Brush brush, Point[] points)
		{
			//SolidBrush b=new SolidBrush(Color.Red);
			Point[] pts = CreatePoints(points);
			ds.Graphics.FillPolygon(brush, pts);
			//ds.Graphics.FillPolygon(b, pts);
		}

		private void DrawHeaderColumns(DrawStruct ds)
		{
			Rectangle rect = ds.Rectangle;

			int ident = GetGroupIdent(ds.Group);
			if (ds.Group == groups.Count - 1)
			{
				DrawHeaderBlank(ds, 0, ds.Rectangle.Top, ident, ds.Rectangle.Height);
			}
			else
			{
				DrawHeaderControl(ds, 0, ds.Rectangle.Top, ident, ds.Rectangle.Height, "", 0);
			}

			int left = ident;

			int width = ds.Rectangle.Width - left;
			float[] columnsWidth = groupColumnsWidth[ds.Group];

			int[] sort = source.GetSort(ds.Group);
			int columnWidth;
			GridGroup group = groups[ds.Group];
			for (int column = 0; column < group.Columns.Count; column++)
			{
				if (group.Columns[column].Visible)
				{
					if (column == group.LastVisible)
					{
						columnWidth = ds.Rectangle.Right - left;
					}
					else
					{
						columnWidth = (int)((float)width * columnsWidth[column]);
					}
					int d = 0;
					if (sort != null)
					{
						for (int s = 0; s < sort.Length && d == 0; s++)
						{
							if (sort[s] == group.Columns[column].Field)
								d = s + 1;
							else if (sort[s] - Int32.MinValue == group.Columns[column].Field)
								d = -(s + 1);
						}
					}
					DrawHeaderControl(ds,
						left, ds.Rectangle.Top, columnWidth,
						ds.Rectangle.Height,
						group.Columns[column].Title, d);
					left += columnWidth;
				}
			}
		}

		private void DrawHeader(DrawStruct ds)
		{
			if (RightToLeft == RightToLeft.Yes)
				ds.StringFormat = new StringFormat(StringFormatFlags.DirectionRightToLeft);
			else
				ds.StringFormat = new StringFormat();

			ds.StringFormat.Alignment = StringAlignment.Center;
			ds.StringFormat.LineAlignment = StringAlignment.Center;

			if (groupColumnsWidth == null)
				CalculateColumnWidth();

			Rectangle rect = ds.Rectangle;

			ds.Graphics.FillRectangle(SystemBrushes.Control, rect);

			if (_groupingStyle == GridGroupingStyle.Grid)
			{
				int rh = rect.Height / groups.Count;

				if (rh > 0)
				{
					int top = rect.Top;
					for (int group = 0; group < groups.Count; group++)
					{
						if (group == groups.Count - 1)
						{
							rh = rect.Bottom - top;
						}

						ds.Group = group;
						ds.Rectangle = new Rectangle(rect.Left, top, rect.Width, rh);

						DrawHeaderColumns(ds);
						top += rh;
					}
				}
			}
			else
			{
				ds.Group = groups.Count - 1;
				DrawHeaderColumns(ds);
			}
		}

		private struct DrawStruct
		{
			public Graphics Graphics;
			public Size Size;

			public StringFormat StringFormat;

			public Rectangle Rectangle;
			public int Group;
			public int Row;
			public int Column;

			public GridDrawState State;
			public Style Style;

			public DrawStruct(Graphics g, Size s)
			{
				Graphics = g;
				Size = s;
				StringFormat = null;
				Rectangle = new Rectangle(0, 0, 0, 0);
				Group = 0;
				Column = 0;
				Row = 0;
				State = GridDrawState.Normal;
				Style = null;
			}
		}

		private void DrawGrid(DrawStruct ds)
		{
			try
			{
				ds.Rectangle = new Rectangle(0, 0, ds.Size.Width, headerheight);
				DrawHeader(ds);
				ds.Rectangle = new Rectangle(0, headerheight, ds.Size.Width, ds.Size.Height - headerheight);
				DrawRows(ds);
			}
			catch (Exception e)
			{
				ds.Graphics.FillRectangle(SystemBrushes.Window, ds.Rectangle);
				ds.Graphics.DrawString("An exception has occured during grid paint:\n  " + e.Message,
					Font, SystemBrushes.WindowText, ds.Rectangle);
				System.Diagnostics.Debug.WriteLine("failed to draw grid: " + e.Message);
				System.Diagnostics.Debug.WriteLine(e.StackTrace);
			}
		}

		private void DrawRowRoot(DrawStruct ds,
			int x, int y, int width, int height, bool expanded, bool hasChildren)
		{
			FillRectangle(ds, x, y, width, height, SystemBrushes.Control);
			if (hasChildren)
			{
				int m = y + (height / 2);
				int l = x - 1;
				if (!expanded)
				{
					FillPolygon(ds, SystemBrushes.ControlDarkDark,
						new Point[] {
										new Point(l + 3, m - 1),
										new Point(l + 6, m - 1),
										new Point(l + 6, m - 4),
										new Point(l + 9, m - 4),
										new Point(l + 9, m - 1),
										new Point(l + 12, m - 1),
										new Point(l + 12, m + 2),
										new Point(l + 9, m + 2),
										new Point(l + 9, m + 5),
										new Point(l + 6, m + 5),
										new Point(l + 6, m + 2),
										new Point(l + 3, m + 2)
									});
				}
				else
				{
					FillPolygon(ds, SystemBrushes.ControlDarkDark,
						new Point[] {
										new Point(l + 3, m - 1),
										new Point(l + 12, m - 1),
										new Point(l + 12, m + 2),
										new Point(l + 3, m + 2)
									});
				}
			}

			ds.Graphics.DrawLine(SystemPens.ControlDark,
				CreatePoint(new Point(x + width - 1, y)),
				CreatePoint(new Point(x + width - 1, y + height - 1)));
		}

		private void DrawRowColumn(DrawStruct ds)
		{
			string text = null;
			int field = -1;
			GridColumn column = null;

			Rectangle rect = CreateRect(ds.Rectangle.X,
				ds.Rectangle.Y, ds.Rectangle.Width, ds.Rectangle.Height);

			if (editControl != null && editControl.Visible &&
				editCell.X == ds.Column &&
				editCell.Y == ds.Row)
			{
				ds.Graphics.FillRectangle(SystemBrushes.Control, rect);
			}
			else
			{
				int styleField = field;
				int dsGroup = ds.Group;
				if (groups != null && ds.Group >= 0 && ds.Group < groups.Count)
				{
					GridGroup gridGroup = groups[dsGroup];
					if (gridGroup.Columns != null && ds.Column >= 0 && ds.Column < gridGroup.Columns.Count)
						styleField = gridGroup.Columns[ds.Column].Field;
				}
				Style style = source.GetStyle(ds.Row, styleField, ds.State);

				if (style != null && style.Background != null)
					ds.Graphics.FillRectangle(style.Background, rect);
				else
					ds.Graphics.FillRectangle(ds.Style.Background, rect);

				if (ds.Column == -1) // Row number column
				{
					if (_showRowNumber)
					{
						text = (ds.Row + 1).ToString();
					}

					if (_showCheckBoxes)
					{
						ButtonState state = checkedRows[ds.Row] ? ButtonState.Checked : ButtonState.Normal;
						if (RightToLeft == RightToLeft.Yes)
						{
							ControlPaint.DrawCheckBox(ds.Graphics, rect.Right - 14,
								rect.Top + (rect.Height - 12) / 2, 12, 12, state);
							rect.Width -= checkBoxIdent;
						}
						else
						{
							ControlPaint.DrawCheckBox(ds.Graphics, rect.Left + 2,
								rect.Top + (rect.Height - 12) / 2, 12, 12, state);
							rect.Width -= checkBoxIdent;
							rect.X += checkBoxIdent;
						}
					}
				}
				else
				{
					column = groups[ds.Group].Columns[ds.Column];

					if (!column.OnDrawCell(ds.Row, ds.Graphics, rect, ds.Style, ds.State))
					{
						field = column.Field;
						if (field >= 0)
						{
							text = source.GetText(ds.Row, field);
						}
					}
				}

				if (text != null)
				{
					StringFormat sf = new StringFormat();
					if (RightToLeft == RightToLeft.Yes)
						sf.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
					sf.LineAlignment = StringAlignment.Center;
					if (column != null)
						sf.Alignment = column.Alignment;
					Font font = style == null || style.Font == null ? ds.Style.Font : style.Font;
					Brush fg = style == null || style.Foreground == null ? ds.Style.Foreground : style.Foreground;
					ds.Graphics.DrawString(text, font, fg, rect, sf);
				}
			}
		}

		private void DrawRow(DrawStruct ds, bool expanded, bool hasChildren)
		{
			Rectangle rect = ds.Rectangle;
			int ident = GetGroupIdent(ds.Group);
			if (ds.Group == groups.Count - 1)
			{
				if (ident > 0)
				{
					FillRectangle(ds, rect.Left, rect.Top,
						ident, rect.Height, ds.Style.Background);
				}
			}
			else
			{
				DrawRowRoot(ds, rect.Left, rect.Top,
					ident, rect.Height, expanded, hasChildren);
			}

			int left = rect.Left + ident;
			int width = rect.Right - left;


			if (_showRowNumber || _showCheckBoxes)
			{
				ds.Column = -1; // number/check box column
				ds.Rectangle = new Rectangle(left - rowNumberIdent, rect.Top,
					rowNumberIdent, rect.Height);
				DrawRowColumn(ds);
			}

			GridGroup group = groups[ds.Group];
			float[] columnsWidth = groupColumnsWidth[ds.Group];
			int columnWidth;

			for (int column = 0; column < group.Columns.Count; column++)
			{
				GridColumn c = group.Columns[column];
				if (c.Visible)
				{
					if (column == group.LastVisible)
						columnWidth = rect.Right - left;
					else
						columnWidth = (int)((float)width * columnsWidth[column]);
					ds.Column = column;
					ds.Rectangle = new Rectangle(left, rect.Top, columnWidth, rect.Height);
					try
					{
						DrawRowColumn(ds);
					}
					catch (Exception e)
					{
						System.Diagnostics.Debug.WriteLine("warning: failed to draw row column. row: " + ds.Row);
						System.Diagnostics.Debug.WriteLine("error: " + e.Message);
						System.Diagnostics.Debug.WriteLine(e.StackTrace);
						//groups.Remove(group);
						//return;
					}
					left += columnWidth;
				}
			}

			if (_horizontalLines)
			{
				ds.Graphics.DrawLine(
					SystemPens.ControlLight,
					CreatePoint(rect.Left, rect.Bottom - 1),
					CreatePoint(rect.Right, rect.Bottom - 1));
			}

			if (ds.Row == selectedIndex && Focused)
			{
				ControlPaint.DrawFocusRectangle(ds.Graphics,
					new Rectangle(rect.X, rect.Y - 1, rect.Width, rect.Height + 1));
			}
		}

		private void DrawListHeader(DrawStruct ds, bool expanded, bool hasChildren)
		{
			Rectangle rect = CreateRect(ds.Rectangle.X,
				ds.Rectangle.Y, ds.Rectangle.Width, ds.Rectangle.Height);

			Style style = source.GetStyle(ds.Row, -1, ds.State);

			if (style != null && style.Background != null)
				ds.Graphics.FillRectangle(style.Background, rect);
			else
				ds.Graphics.FillRectangle(ds.Style.Background, rect);

			int ident = (ds.Group * 12) + 4;

			if (hasChildren)
			{
				Pen pen = new Pen(ds.Style.Foreground);
				int off = (ds.Rectangle.Height - 8) / 2;
				ds.Graphics.DrawRectangle(pen,
						CreateRect(ds.Rectangle.X + ident, ds.Rectangle.Y + off, 8, 8));

				ds.Graphics.DrawLine(pen,
					CreatePoint(new Point(ds.Rectangle.X + ident + 1, ds.Rectangle.Y + off + 4)),
					CreatePoint(new Point(ds.Rectangle.X + ident + 5, ds.Rectangle.Y + off + 4)));

				if (!expanded)
				{
					ds.Graphics.DrawLine(pen,
						CreatePoint(new Point(ds.Rectangle.X + ident + 3, ds.Rectangle.Y + off + 2)),
						CreatePoint(new Point(ds.Rectangle.X + ident + 3, ds.Rectangle.Y + off + 6)));
				}

				pen.Dispose();
			}

			GridGroup group = groups[ds.Group];
			string title = "";
			int field;
			string text;
			for (int column = 0; column < group.Columns.Count; column++)
			{
				GridColumn c = group.Columns[column];
				if (c.Visible)
				{
					field = c.Field;
					if (field >= 0)
					{
						text = source.GetText(ds.Row, field);
						if (text.Length > 0)
						{
							if (title.Length > 0)
								title += " - ";
							title += text;
						}
					}
				}
			}

			if (title.Length > 0)
			{
				ident += 12;
				Rectangle textRect = CreateRect(ds.Rectangle.X + ident,
					ds.Rectangle.Y, ds.Rectangle.Width - ident, ds.Rectangle.Height);

				StringFormat sf = new StringFormat();
				if (RightToLeft == RightToLeft.Yes)
					sf.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
				sf.LineAlignment = StringAlignment.Center;
				Font font = style == null || style.Font == null ? ds.Style.Font : style.Font;
				Brush fg = style == null || style.Foreground == null ? ds.Style.Foreground : style.Foreground;
				ds.Graphics.DrawString(title, font, fg, textRect, sf);
			}

			if (_horizontalLines)
			{
				ds.Graphics.DrawLine(
					SystemPens.ControlLight,
					CreatePoint(rect.Left, rect.Bottom - 1),
					CreatePoint(rect.Right, rect.Bottom - 1));
			}

			if (ds.Row == selectedIndex && Focused)
			{
				ControlPaint.DrawFocusRectangle(ds.Graphics,
					new Rectangle(rect.X, rect.Y - 1, rect.Width, rect.Height + 1));
			}
		}

		private void DrawRows(DrawStruct ds)
		{
			Rectangle rect = ds.Rectangle;
			int rowcount = source.GetRowCount();
			int top = rect.Top;
			int row = _row;

			GridDrawState state = GridDrawState.Normal;

			if (Focused)
				state |= GridDrawState.Focused;

			if (row < rowcount)
			{
				int nextGroup = source.GetGroup(row);
				int lastGroup = 0;
				bool skipGroup = false;
				while (top < rect.Bottom && row < rowcount)
				{
					ds.Group = nextGroup;
					nextGroup = row < rowcount - 1 ? source.GetGroup(row + 1) : -1;
					if (!skipGroup || ds.Group <= lastGroup)
					{
						ds.Row = row;
						ds.Rectangle = new Rectangle(rect.Left, top, rect.Width,
							groups[ds.Group].RowHeight);

						skipGroup = !expandedRows[row];

						if (selection[ds.Row])
						{
							ds.State = state | GridDrawState.Selected;
							if (ds.Group < groups.Count - 1 && _groupingStyle == GridGroupingStyle.List)
								ds.Style = _style.SelectedTitle;
							else if ((state & GridDrawState.Focused) != 0)
								ds.Style = _style.Selected;
							else
								ds.Style = _style.UnfocusedSelected;
						}
						else
						{
							ds.State = state;
							if (ds.Group == groups.Count - 1)
							{
								if ((state & GridDrawState.Focused) != 0)
									ds.Style = _style.Normal;
								else
									ds.Style = _style.Unfocused;
							}
							else
							{
								if (_groupingStyle == GridGroupingStyle.Grid)
									ds.Style = _style.Group;
								else
									ds.Style = _style.Title;
							}
						}

						if (_groupingStyle == GridGroupingStyle.List &&
							ds.Group < groups.Count - 1)
							DrawListHeader(ds, !skipGroup, nextGroup > ds.Group);
						else
							DrawRow(ds, !skipGroup, nextGroup > ds.Group);

						lastGroup = ds.Group;
						top += groups[ds.Group].RowHeight;
					}
					row++;
				}
			}

			if (top < rect.Bottom)
			{
				FillRectangle(ds, rect.X, top,
					rect.Width, rect.Bottom - top,
					SystemBrushes.Window);
			}
		}

		private Bitmap bitMap = null;
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			if (bitMap == null)
			{
				int width = rectGrid.Width;
				int height = rectGrid.Height;
				if (width <= 0)
					width = 1;
				if (height <= 0)
					height = 1;
				try
				{
					bitMap = new Bitmap(width, height, e.Graphics);
				}
				catch
				{
					// (Exception ex)
					//throw new Exception("error drawing: "+ex.Message+": width: "+rectGrid.Width+", height: "+rectGrid.Height+", graphics: "+e.Graphics.ToString());
				}
			}

			Graphics g = Graphics.FromImage(bitMap);

			if (_borderStyle == BorderStyle.Fixed3D)
			{
				ControlPaint.DrawBorder3D(e.Graphics, 0, 0, Width, Height,
					Border3DStyle.Sunken);
			}
			else if (_borderStyle == BorderStyle.FixedSingle)
			{
				e.Graphics.DrawRectangle(SystemPens.WindowFrame, 0, 0, Width - 1, Height - 1);
			}

			DrawGrid(new DrawStruct(g, rectGrid.Size));

			if (bitMap != null)
				e.Graphics.DrawImage(bitMap, rectGrid.Location);
		}

		protected override void OnPaintBackground(PaintEventArgs pevent)
		{
			// Not erasing background, just painting over
			//base.OnPaintBackground (pevent);
		}


		#endregion

		#region Grid Sizing

		private int defaultRowHeight = 20;
		private int headerheight = 20;
		private Rectangle rectGrid;
		private int sizewidth = 3;
		private int sizeheight = 2;

		private static readonly int defaultIdent = 15;
		private static readonly int defaultRowNumberIdent = 25;
		private static readonly int checkBoxIdent = 16;
		private float[][] groupColumnsWidth = null;

		public int HeaderHeight
		{
			get { return headerheight; }
			set
			{
				headerheight = value;
				Refresh();
			}
		}

		private void CalculateColumnWidth()
		{
			groupColumnsWidth = new float[groups.Count][];

			for (int group = 0; group < groups.Count; group++)
			{
				GridGroup g = groups[group];
				int vb = 0;
				int total = 0;
				foreach (GridColumn c in g.Columns)
				{
					if (c.Visible)
					{
						total += c.Width;
						vb++;
					}
				}

				groupColumnsWidth[group] = new float[g.Columns.Count];
				//int ident = group == groups.Count ?
				//	(group == 0 ? 0 : group * defaultIdent) : 
				//	(group + 1) * defaultIdent;
				//int width = rectGrid.Width - ident;
				float left = 1;
				for (int column = 0; column < g.Columns.Count; column++)
				{
					GridColumn c = g.Columns[column];
					if (c.Visible)
					{
						if (vb == 1)
						{
							groupColumnsWidth[group][column] = left;
						}
						else
						{
							groupColumnsWidth[group][column] = (float)c.Width / (float)total;
							left -= groupColumnsWidth[group][column];
						}
						vb--;
					}
					else
					{
						groupColumnsWidth[group][column] = 0;
					}
				}
			}
		}

		public int GetColumnWidth(int group, int column)
		{
			if (column == -1) // Row number column
			{
				return rowNumberIdent;
			}

			if (groupColumnsWidth == null)
				CalculateColumnWidth();

			int ident = GetGroupIdent(group);
			int total = rectGrid.Width - ident;
			float[] columnsWidth = groupColumnsWidth[group];
			return (int)((float)total * columnsWidth[column]);
		}

		public void SetColumnsWidth(int group, int[] widthes)
		{
			if (widthes.Length != groups[group].Columns.Count)
				throw new ArgumentOutOfRangeException("widthes", "Widthes count don't match columns count");

			int total = 0;
			for (int n = 0; n < widthes.Length; n++)
			{
				total += widthes[n];
			}

			float[] columnsWidth = groupColumnsWidth[group];
			float left = 1;
			for (int n = 0; n < columnsWidth.Length - 1; n++)
			{
				columnsWidth[n] = (float)widthes[n] / (float)total;
				left -= columnsWidth[n];
			}

			columnsWidth[columnsWidth.Length - 1] = left;
		}

		private void SetColumnWidth(int group, int column, int width)
		{
			if (column == -1)
			{
				int newWidth = width;
				if (newWidth < minimumColumnWidth)
					newWidth = minimumColumnWidth;
				int d = newWidth - rowNumberIdent;

				if (d > 0)
				{
					for (int g = 0; g < groups.Count; g++)
					{
						float[] columnsWidth = groupColumnsWidth[g];
						int ident = GetGroupIdent(g);
						int w = rectGrid.Width - ident - d;
						for (int c = 0; c < groups[group].Columns.Count; c++)
						{
							if ((int)(columnsWidth[c] * (float)w) < minimumColumnWidth)
							{
								w = (int)((float)minimumColumnWidth / columnsWidth[c]);
							}
						}

						d = rectGrid.Width - ident - w;
						if (d < 0)
							d = 0;
					}
				}

				rowNumberIdent += d;
			}
			else
			{
				float[] columnsWidth = groupColumnsWidth[group];
				int ident = GetGroupIdent(group);
				int w = rectGrid.Width - ident;
				float min = (float)minimumColumnWidth / (float)w;
				float newWidth = (float)(width) / (float)w;
				float d = newWidth - columnsWidth[column];

				if (d < 0)
				{
					columnsWidth[groups[group].LastVisible] -= d;
					columnsWidth[column] = newWidth;
				}
				else
				{
					for (int c = groups[group].Columns.Count - 1;
						c > column && d > 0; c--)
					{
						GridColumn gridColumn = groups[group].Columns[c];
						if (gridColumn.Visible)
						{
							if (columnsWidth[c] > min)
							{
								if (columnsWidth[c] > min + d)
								{
									columnsWidth[c] -= d;
									d = 0;
								}
								else
								{
									d -= columnsWidth[c] - min;
									columnsWidth[c] = min;
								}
							}
						}
					}

					columnsWidth[column] = newWidth - d;
				}
			}
		}

		private int GetGroupIdent(int group)
		{
			int rowIdent = _showRowNumber || _showCheckBoxes ? rowNumberIdent : 0;
			if (_groupingStyle == GridGroupingStyle.List)
				return rowIdent;
			if (group == groups.Count - 1)
			{
				return group * defaultIdent + rowIdent;
			}

			return (group + 1) * defaultIdent + rowIdent;
		}

		private void ResetBounds()
		{
			if (bitMap != null)
				bitMap.Dispose();

			bitMap = null;
			int borderWidth = 0;
			if (_borderStyle == BorderStyle.Fixed3D)
				borderWidth = 2;
			else if (_borderStyle == BorderStyle.FixedSingle)
				borderWidth = 1;

			if (scrollVisible)
			{
				scroll.Size = new Size(scroll.Width, Height - borderWidth * 2);
				if (RightToLeft == RightToLeft.Yes)
				{
					scroll.Location = new Point(borderWidth, borderWidth);
					rectGrid = new Rectangle(
						borderWidth + scroll.Width, borderWidth,
						Width - borderWidth * 2 - scroll.Width,
						Height - borderWidth * 2);
				}
				else
				{
					scroll.Location = new Point(
						Width - borderWidth - scroll.Width, borderWidth);
					rectGrid = new Rectangle(borderWidth, borderWidth,
						Width - borderWidth * 2 - scroll.Width,
						Height - borderWidth * 2);
				}
			}
			else
			{
				rectGrid = new Rectangle(borderWidth, borderWidth,
					Width - borderWidth * 2, Height - borderWidth * 2);
			}
		}

		private void ResetGrid()
		{
			string strLogPath = Path.GetDirectoryName(Application.ExecutablePath) +
				Path.DirectorySeparatorChar + "ResetGrid.log";

			// Calculating page size
			//Sport.Core.LogFiles.AppendToLogFile(strLogPath, "calculate page size...");
			CalculatePageSize();

			// Setting scroll
			//Sport.Core.LogFiles.AppendToLogFile(strLogPath, "set scroll...");
			ResetScroll();

			// Setting bounds
			//Sport.Core.LogFiles.AppendToLogFile(strLogPath, "reset bounds...");
			ResetBounds();

			// Setting edit control bounds
			if (editControl != null)
			{
				//Sport.Core.LogFiles.AppendToLogFile(strLogPath, "reset edit control...");
				ResetEditControl();
			}

			// Redrawing
			//Sport.Core.LogFiles.AppendToLogFile(strLogPath, "refresh...");
			Refresh();

			//Sport.Core.LogFiles.AppendToLogFile(strLogPath, "done!");
		}

		private bool GetCellBounds(int row, int col, out Rectangle bounds)
		{
			int group = source.GetGroup(row);
			GridGroup g = groups[group];
			int ident = GetGroupIdent(group);
			int total = rectGrid.Width - ident;
			float[] columnsWidth = groupColumnsWidth[group];
			int left = ident;
			for (int c = 0; c < col; c++)
			{
				if (g.Columns[c].Visible)
				{
					left += (int)(columnsWidth[c] * (float)total);
				}
			}
			int width;

			if (col == g.LastVisible)
				width = rectGrid.Width - left;
			else
				width = (int)(columnsWidth[col] * (float)total);

			int height = g.RowHeight - 1;

			if (row < _row)
			{
				bounds = CreateRect(left, 0, width, height);
				bounds.X += rectGrid.X;
				bounds.Y += rectGrid.Y;
				return false;
			}

			int top = headerheight;

			int lastGroup = group;
			bool skipgroup = false;
			int r = _row;
			while (r < row && top < rectGrid.Height)
			{
				group = source.GetGroup(r);
				if (!skipgroup || lastGroup >= group)
				{
					top += groups[group].RowHeight;
					lastGroup = group;
					skipgroup = !expandedRows[r];
				}
				r++;
			}

			bounds = CreateRect(left, top, width, height);
			bounds.X += rectGrid.X;
			bounds.Y += rectGrid.Y;

			return top < rectGrid.Height;
		}

		// OnResize is called when the grid is resized
		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);

			ResetGrid();
		}

		#endregion

		#region Scrolling

		private int _row;
		private int _visibleRow;
		private int _pageSize;
		private ScrollBar scroll;
		private bool scrollVisible;

		// Reset the scroll size and returns
		// whether the scroll visibility has changed
		private bool ResetScroll()
		{
			int count = GetRowCount();

			if (count > _pageSize)
			{
				scroll.Minimum = 0;
				scroll.Maximum = count - 1;
				scroll.Value = _visibleRow;
				scroll.LargeChange = _pageSize;
				scroll.Visible = true;
				scrollVisible = true;
			}
			else
			{
				_row = 0;
				_visibleRow = 0;
				scroll.Visible = false;
				scrollVisible = false;
			}

			return true;
		}

		private void ScrollScroll(object sender, ScrollEventArgs e)
		{
			switch (e.Type)
			{
				case (ScrollEventType.SmallIncrement):
				case (ScrollEventType.SmallDecrement):
				case (ScrollEventType.LargeIncrement):
				case (ScrollEventType.LargeDecrement):
				case (ScrollEventType.ThumbTrack):
				case (ScrollEventType.ThumbPosition):
					ScrollTo(e.NewValue);
					break;
			}
		}

		private void ScrollValueChanged(object sender, EventArgs e)
		{
			CalculatePageSize();

			if (editControl != null)
			{
				editControl.Visible = false;
				if (RowInPage(editCell.Y))
				{
					ResetEditControl();
					editControl.Visible = true;
				}
			}
		}


		private bool ScrollPrev()
		{
			int r = GetRowIndex(_visibleRow - 1);
			if (r != -1)
			{
				_row = r;
				_visibleRow--;
				scroll.Value = _visibleRow;

				return true;
			}

			return false;
		}

		private bool ScrollNext()
		{
			int r = GetRowIndex(_visibleRow + 1);
			if (r != -1)
			{
				_row = r;
				_visibleRow++;
				scroll.Value = _visibleRow;

				return true;
			}

			return false;
		}

		private void CalculatePageSize()
		{
			string strLogPath = Path.GetDirectoryName(Application.ExecutablePath) +
				Path.DirectorySeparatorChar + "CalculatePageSize.log";

			_pageSize = 0;

			int group;
			int h = rectGrid.Height - headerheight;
			int r = _visibleRow;
			//Sport.Core.LogFiles.AppendToLogFile(strLogPath, "getting row index for "+r);
			int i = GetRowIndex(r);
			//Sport.Core.LogFiles.AppendToLogFile(strLogPath, "index: "+i);
			int vv = 0;
			while (i >= 0)
			{
				if (vv > 1000)
					break;
				group = source.GetGroup(i);
				//Sport.Core.LogFiles.AppendToLogFile(strLogPath, 
				//	"group for index "+i+" is "+group);
				h -= groups[group].RowHeight;
				//Sport.Core.LogFiles.AppendToLogFile(strLogPath, 
				//	"current h: "+h);
				if (h >= 0)
				{
					_pageSize++;
					r++;
					i = GetRowIndex(r);
					//Sport.Core.LogFiles.AppendToLogFile(strLogPath, 
					//	"page size: "+_pageSize+", r: "+r+", index: "+i);
				}
				else
				{
					i = -1;
				}
				vv++;
			}

			/*
			 * causing problems...
			int v = _visibleRow;
			while (h > 0 && v > 0)
			{
				i = GetRowIndex(v - 1);
				group = source.GetGroup(i);
				h -= groups[group].RowHeight;
				if (h >= 0)
				{
					_pageSize++;
					v--;
				}
			}

			if (v != _visibleRow)
			{
				_visibleRow = v;
				scroll.Value = _visibleRow;
			}*/

			scroll.LargeChange = _pageSize;
		}

		// Timer used for automatic scrolling on cell selection dragging
		private Timer autoScrollTimer;
		private int scrollDirection = 0;

		private void ScrollTick(object sender, EventArgs e)
		{
			if (scrollDirection == -1)
			{
				if (_visibleRow > 0)
				{
					ScrollPrev();
					if (!SelectRows(selectionStart, _row, false))
					{
						StopAutoScroll();
					}
				}
				else
				{
					StopAutoScroll();
				}
			}
			else if (scrollDirection == 1)
			{
				if (_visibleRow <= scroll.Maximum - scroll.LargeChange)
				{
					ScrollNext();
					int row = _visibleRow + _pageSize;
					if (row >= rowsIndexes.Count)
						row = rowsIndexes.Count - 1;
					if (!SelectRows(selectionStart, (int)rowsIndexes[row], false))
					{
						StopAutoScroll();
					}
				}
				else
				{
					StopAutoScroll();
				}
			}
		}

		private void ScrollTo(int visibleRow)
		{
			if (visibleRow != _visibleRow)
			{
				int r = GetRowIndex(visibleRow);
				if (r != -1)
				{
					_row = r;
					_visibleRow = visibleRow;
					scroll.Value = _visibleRow;
				}

				Refresh();
			}
		}

		public int VisibleRow
		{
			get { return _visibleRow; }
			set { ScrollTo(value); }
		}

		private bool RowInPage(int row)
		{
			if (_row > row)
				return false;

			int lastRow = _visibleRow + _pageSize - 1;
			if (lastRow >= rowsIndexes.Count)
				lastRow = rowsIndexes.Count - 1;
			return GetRowIndex(lastRow) >= row;
		}

		public void ScrollToRow(int row)
		{
			if (row != _row)
			{
				if (row < _row)
				{
					while (!RowInPage(row) && ScrollPrev()) ;
				}
				else
				{
					while (!RowInPage(row) && ScrollNext()) ;
				}

				scroll.Value = _visibleRow;

				Refresh();
			}
		}

		#endregion

		#region Grid Input

		protected override bool IsInputKey(Keys keyData)
		{
			switch (keyData & (~Keys.Shift))
			{
				case (Keys.Down):
				case (Keys.Up):
				case (Keys.PageDown):
				case (Keys.PageUp):
				case (Keys.Home):
				case (Keys.End):
				case (Keys.Left):
				case (Keys.Right):
				case (Keys.Space):
					return true;
			}

			return base.IsInputKey(keyData);
		}

		private bool _selectOnSpace = false;
		public bool SelectOnSpace
		{
			get { return _selectOnSpace; }
			set { _selectOnSpace = value; }
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);

			if (e.Handled)
				return;

			int scrl = 0;

			int currentRow = GetIndexRow(selectedIndex);
			int newRow = currentRow;

			switch (e.KeyCode)
			{
				case (Keys.Down):
					newRow = currentRow + 1;
					break;
				case (Keys.Up):
					newRow = currentRow - 1;
					break;
				case (Keys.PageDown):
					newRow = currentRow + _pageSize;
					scrl = _pageSize;
					break;
				case (Keys.PageUp):
					newRow = currentRow - _pageSize;
					scrl = -_pageSize;
					break;
				case (Keys.Home):
					newRow = 0;
					break;
				case (Keys.End):
					newRow = rowsIndexes.Count - 1;
					break;
				case (Keys.Space):
					if (_selectOnSpace)
					{
						Edit();
					}
					else if (selectedIndex >= 0)
					{
						bool selected = selection[selectedIndex];
						if (_showCheckBoxes && selection.Size == 1 && selected)
						{
							CheckRow(selectedIndex);
							Refresh();
						}
						else if (SelectRow(selectedIndex, !selected, e.Control))
						{
							selectionStart = selectedIndex;
							Refresh();
						}
					}
					break;
				case (Keys.Left):
				case (Keys.Right):
					if (selectedIndex >= 0)
					{
						if (RightToLeft == RightToLeft.Yes)
						{
							expandedRows[selectedIndex] = e.KeyCode == Keys.Left;
						}
						else
						{
							expandedRows[selectedIndex] = e.KeyCode == Keys.Right;
						}
					}
					break;
			}

			if ((scrl != 0) && (rowsIndexes.Count > 0))
			{
				_visibleRow += scrl;
				if (_visibleRow < 0)
					_visibleRow = 0;
				if (_visibleRow > scroll.Maximum - scroll.LargeChange)
					_visibleRow = scroll.Maximum - scroll.LargeChange;
				if (_visibleRow < 0)
					_visibleRow = 0;
				if (_visibleRow >= rowsIndexes.Count)
					_visibleRow = rowsIndexes.Count - 1;
				_row = (int)rowsIndexes[_visibleRow];
				scroll.Value = _visibleRow;
			}

			if (newRow != currentRow)
			{
				if (newRow < 0)
					newRow = 0;
				if (newRow >= rowsIndexes.Count)
					newRow = rowsIndexes.Count - 1;

				int lastIndex = selectedIndex;
				selectedIndex = GetRowIndex(newRow);

				ScrollToRow(selectedIndex);

				if ((!e.Control && !e.Shift) || selectionStart == -1)
					selectionStart = selectedIndex;

				if (e.Shift || !e.Control)
				{
					if (!SelectRows(selectionStart, selectedIndex, false))
					{
						lastIndex = selectedIndex;
						Refresh();
					}
				}
				else // If not selecting rows need to refresh because scroll
					// might change
					Refresh();
			}
		}

		#endregion

		#region Expansion

		private bool resetRowsIndexes = true;
		private ArrayList rowsIndexes;
		private int GetRowIndex(int row)
		{
			string strLogPath = Path.GetDirectoryName(Application.ExecutablePath) +
				Path.DirectorySeparatorChar + "GetRowIndex.log";

			if (resetRowsIndexes)
			{
				//Sport.Core.LogFiles.AppendToLogFile(strLogPath, 
				//	"calculate rows indices (row "+row+")...");
				CalculateRowsIndexes();
				//Sport.Core.LogFiles.AppendToLogFile(strLogPath, 
				//	"done!");
			}

			if (row >= rowsIndexes.Count || row < 0)
				return -1;

			return (int)rowsIndexes[row];
		}
		private int GetIndexRow(int index)
		{
			if (resetRowsIndexes)
				CalculateRowsIndexes();

			for (int row = 0; row < rowsIndexes.Count; row++)
			{
				if ((int)rowsIndexes[row] > index)
					return -1;
				if ((int)rowsIndexes[row] == index)
					return row;
			}

			return -1;
		}
		private int GetRowCount()
		{
			if (resetRowsIndexes)
				CalculateRowsIndexes();
			return rowsIndexes.Count;
		}

		private void CalculateRowsIndexes()
		{
			string strLogPath = Path.GetDirectoryName(Application.ExecutablePath) +
				Path.DirectorySeparatorChar + "FinalCheck.log";

			rowsIndexes.Clear();
			//Sport.Core.LogFiles.AppendToLogFile(strLogPath, "getting rows count...");
			int rows = source.GetRowCount();
			//Sport.Core.LogFiles.AppendToLogFile(strLogPath, "count: "+rows);
			int row = 0;
			int lastGroup = 0;
			_visibleRow = 0;
			bool skipgroup = false;
			//Sport.Core.LogFiles.AppendToLogFile(strLogPath, "starting loop, rows: "+rows+"...");
			while (row < rows)
			{
				//Sport.Core.LogFiles.AppendToLogFile(strLogPath, 
				//	"getting group for row "+row+"...");
				int group = source.GetGroup(row);
				//Sport.Core.LogFiles.AppendToLogFile(strLogPath, "group: "+group);
				if (!skipgroup || lastGroup >= group)
				{
					lastGroup = group;
					skipgroup = !expandedRows[row];
					rowsIndexes.Add(row);
					if (row == _row)
						_visibleRow = rowsIndexes.Count - 1;
				}
				row++;
			}

			resetRowsIndexes = false;

			//			Debug.WriteLine("Visible row count: " + rowsIndexes.Count.ToString());
		}

		private void ResetRowsIndexes()
		{
			resetRowsIndexes = true;
		}

		private RangeArray expandedRows;
		public RangeArray ExpandedRows
		{
			get { return expandedRows; }
		}

		private bool _expandOnDoubleClick;
		public bool ExpandOnDoubleClick
		{
			get { return _expandOnDoubleClick; }
			set { _expandOnDoubleClick = value; }
		}

		private void ExpandedRowsChanged(object sender, EventArgs e)
		{
			CancelEdit();
			// Expansion changed, resetting rows indexes
			ResetRowsIndexes();
			if (ResetScroll())
			{
				ResetBounds();
			}
			Refresh();
		}

		#endregion

		#region Checked Rows

		public class CheckEventArgs
		{
			private int _row;
			private bool _checked;

			public int Row { get { return _row; } }
			public bool Checked { get { return _checked; } }

			public CheckEventArgs(int row, bool chk)
			{
				_row = row;
				_checked = chk;
			}
		}

		public delegate void CheckEventHandler(object sender, CheckEventArgs e);

		public event CheckEventHandler RowChecked;

		private RangeArray checkedRows;
		public RangeArray CheckedRows
		{
			get { return checkedRows; }
		}

		private void CheckRow(int row)
		{
			checkedRows[row] = !checkedRows[row];
			if (RowChecked != null)
				RowChecked(this, new CheckEventArgs(row, checkedRows[row]));
		}

		#endregion

		protected override void Dispose(bool disposing)
		{
			if (bitMap != null)
				bitMap.Dispose();

			base.Dispose(disposing);
		}

	}

	#endregion

	#region GridDefaultSource

	public class GridDefaultSource : IGridSource
	{
		#region GridRow Collection

		public class GridRow
		{
			public int Group;
			public object[] Fields;

			public GridRow(int group, object[] fields)
			{
				Group = group;
				Fields = fields;
			}
		}

		public class GridRowCollection : GeneralCollection
		{
			public GridRow this[int index]
			{
				get { return (GridRow)GetItem(index); }
			}

			public void Insert(int index, GridRow value)
			{
				InsertItem(index, value);
			}

			public void Insert(int index, int group, object[] fields)
			{
				InsertItem(index, new GridRow(group, fields));
			}

			public void Remove(GridRow value)
			{
				RemoveItem(value);
			}

			public bool Contains(GridRow value)
			{
				return base.Contains(value);
			}

			public int IndexOf(GridRow value)
			{
				return base.IndexOf(value);
			}

			public int Add(GridRow value)
			{
				return AddItem(value);
			}

			public int Add(int group, object[] fields)
			{
				return AddItem(new GridRow(group, fields));
			}
		}

		#endregion

		private Grid _grid;
		public Grid Grid
		{
			get { return _grid; }
		}

		private GridRowCollection _rows;
		public GridRowCollection Rows
		{
			get { return _rows; }
		}

		public GridDefaultSource()
		{
			_rows = new GridRowCollection();
			_rows.Changed += new CollectionEventHandler(RowsChanged);
			_grid = null;
		}

		#region IGridSource Members

		public void SetGrid(Grid grid)
		{
			_grid = grid;
		}

		#region Read operations

		public int GetRowCount()
		{
			return _rows.Count;
		}

		public int GetFieldCount(int row)
		{
			GridRow r = _rows[row];
			return r.Fields == null ? 0 : r.Fields.Length;
		}

		public int GetGroup(int row)
		{
			GridRow r = _rows[row];
			return r.Group;
		}

		public string GetText(int row, int field)
		{
			GridRow r = _rows[row];
			if (r.Fields != null && r.Fields.Length > field)
			{
				if (r.Fields[field] != null)
					return r.Fields[field].ToString();
			}
			return null;
		}

		public Style GetStyle(int row, int field, GridDrawState state)
		{
			return null;
		}

		public string GetTip(int row)
		{
			return null;
		}

		#endregion

		#region Sorting

		public int[] GetSort(int group)
		{
			return null;
		}

		public void Sort(int group, int[] columns)
		{
		}

		#endregion

		#region Update operations

		public Control Edit(int row, int field)
		{
			return null;
		}

		public void EditEnded(Control control)
		{
		}

		#endregion

		#endregion

		public void Dispose()
		{
			_rows.Clear();
		}

		private void RowsChanged(object sender, CollectionEventArgs e)
		{
			if (_grid != null)
				_grid.RefreshSource();
		}
	}

	#endregion

	#region GridControl

	/// <summary>
	/// GridControl is a panel that contains a grid control
	/// and a tool bar for grid operations
	/// </summary>
	public class GridControl : Panel
	{
		public class Column
		{
			public int Group;
			public string Title;
			public int Field;
			public int Width;
			public StringAlignment Alignment;

			public Column(int group, string title, int field, int width, StringAlignment alignment)
			{
				Group = group;
				Title = title;
				Field = field;
				Width = width;
				Alignment = alignment;
			}

			public Column(int group, string title, int field, int width)
				: this(group, title, field, width, StringAlignment.Near)
			{
			}

			public Column(int group, string title, int field)
				: this(group, title, field, 0, StringAlignment.Near)
			{
			}
		}

		public void SetColumns(Column[] columns)
		{
			grid.Groups.Clear();

			if (columns != null)
			{
				foreach (Column column in columns)
				{
					while (column.Group > grid.Groups.Count - 1)
						grid.Groups.Add();

					grid.Groups[column.Group].Columns.Add(
						new Grid.GridColumn(column.Field, column.Title, column.Width, column.Alignment));
				}
			}
		}

		private Grid grid;
		public Grid Grid
		{
			get { return grid; }
		}

		#region Grid Buttons

		public class Button : GeneralCollection.CollectionItem
		{
			private ToolBarButton toolBarButton;

			public Sport.Resources.Images Image
			{
				get
				{
					return (Sport.Resources.Images)toolBarButton.ImageIndex;
				}
				set
				{
					toolBarButton.ImageIndex = (int)value;
				}
			}

			public string ToolTipText
			{
				get { return toolBarButton.ToolTipText; }
				set { toolBarButton.ToolTipText = value; }
			}

			public bool Pushed
			{
				get { return toolBarButton.Pushed; }
				set { toolBarButton.Pushed = value; }
			}

			public bool Enabled
			{
				get { return toolBarButton.Enabled; }
				set { toolBarButton.Enabled = value; }
			}

			public event EventHandler Click;

			public Button(Sport.Resources.Images image, string toolTipText)
			{
				toolBarButton = new ToolBarButton();
				toolBarButton.ImageIndex = (int)image;
				toolBarButton.ToolTipText = toolTipText;
			}

			public Button(Sport.Resources.Images image)
				: this(image, null)
			{
			}

			public override void OnOwnerChange(object oo, object no)
			{
				if (oo != null)
				{
					((GridControl)oo).toolBar.Buttons.Remove(toolBarButton);
					((GridControl)oo).toolBar.ButtonClick -= new ToolBarButtonClickEventHandler(ToolBarButtonClicked);
				}
				if (no != null)
				{
					int index = ((GridControl)no).Buttons.IndexOf(this);
					if (index == -1)
						((GridControl)no).toolBar.Buttons.Add(toolBarButton);
					else
						((GridControl)no).toolBar.Buttons.Insert(index, toolBarButton);

					((GridControl)no).toolBar.ButtonClick += new ToolBarButtonClickEventHandler(ToolBarButtonClicked);
				}

				base.OnOwnerChange(oo, no);
			}

			private void OnClick()
			{
				if (Click != null)
					Click(this, EventArgs.Empty);
			}

			private void ToolBarButtonClicked(object sender, ToolBarButtonClickEventArgs e)
			{
				if (e.Button == toolBarButton)
				{
					OnClick();
				}
			}
		}

		public class ButtonCollection : GeneralCollection
		{
			public ButtonCollection(GridControl control)
				: base(control)
			{
			}

			public Button this[int index]
			{
				get { return (Button)GetItem(index); }
				set { SetItem(index, value); }
			}

			public void Insert(int index, Button value)
			{
				InsertItem(index, value);
			}

			public void Insert(int index, Sport.Resources.Images image)
			{
				Insert(index, new Button(image));
			}

			public void Insert(int index, Sport.Resources.Images image, string toolTipText)
			{
				Insert(index, new Button(image, toolTipText));
			}

			public void Remove(Button value)
			{
				RemoveItem(value);
			}

			public bool Contains(Button value)
			{
				return base.Contains(value);
			}

			public int IndexOf(Button value)
			{
				return base.IndexOf(value);
			}

			public int Add(Button value)
			{
				return AddItem(value);
			}

			public int Add(Sport.Resources.Images image)
			{
				return Add(new Button(image));
			}

			public int Add(Sport.Resources.Images image, string toolTipText)
			{
				return Add(new Button(image, toolTipText));
			}
		}

		private ButtonCollection _buttons;
		public ButtonCollection Buttons
		{
			get { return _buttons; }
		}

		#endregion

		private ToolBar toolBar;

		public GridControl()
		{
			BorderStyle = BorderStyle.Fixed3D;

			_buttons = new ButtonCollection(this);

			grid = new Grid();
			grid.Dock = DockStyle.Fill;

			toolBar = new ToolBar();
			toolBar.Appearance = System.Windows.Forms.ToolBarAppearance.Flat;
			toolBar.AutoSize = false;
			toolBar.ButtonSize = new Size(20, 20);
			toolBar.Divider = false;
			toolBar.Dock = DockStyle.Left;
			toolBar.ImageList = Sport.Resources.ImageLists.CreateBlackImageList(null);
			toolBar.Location = new Point(0, 0);
			toolBar.ShowToolTips = true;
			toolBar.Size = new System.Drawing.Size(20, 80);
			toolBar.ButtonClick += new ToolBarButtonClickEventHandler(ToolBarButtonClicked);

			Controls.Add(grid);
			Controls.Add(toolBar);
		}

		private void ToolBarButtonClicked(object sender, ToolBarButtonClickEventArgs e)
		{
		}
	}

	#endregion
}
