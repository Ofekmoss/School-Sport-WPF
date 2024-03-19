using System;
using System.Collections;
using System.Drawing;
using Sport.Common;

namespace Sport.UI
{
	public interface IVisualizerField
	{
		int				Field { get; }
		string			Title { get; }
		int				DefaultWidth { get; }
		StringAlignment	Alignment { get; }
		IComparer		Comparer { get; }
	}

	public interface IVisualizer
	{
		int					GetFieldCount();
		IVisualizerField	GetField(int field);
		IVisualizerField[]	GetFields();
		string				GetText(object o, int field);
	}

	public class VisualizerField : Sport.Common.GeneralCollection.CollectionItem, IVisualizerField
	{
		public VisualizerField(string title, int defaultWidth, 
			StringAlignment alignment, IComparer comparer)
		{
			_field = -1;
			_title = title;
			_defaultWidth = defaultWidth;
			_alignment = alignment;
			_comparer = comparer;
		}

		public VisualizerField(string title, int defaultWidth,
			StringAlignment alignment)
			: this(title, defaultWidth, alignment, null)
		{
		}

		public VisualizerField(string title, int defaultWidth)
			: this(title, defaultWidth, StringAlignment.Near, null)
		{
		}

		public VisualizerField(string title, IComparer comparer)
			: this(title, 100, StringAlignment.Near, comparer)
		{
		}

		public VisualizerField(string title)
			: this(title, 100, StringAlignment.Near, null)
		{
		}

		public override void OnOwnerChange(object oo, object no)
		{
			if (oo != null)
			{
				Visualizer.VisualizerFieldCollection collection = ((Visualizer) oo).Fields;
				for (int n = _field; n < collection.Count; n++)
				{
					((VisualizerField) collection[n])._field--;
				}
			}

			base.OnOwnerChange (oo, no);

			if (no == null)
			{
				_field = -1;
			}
			else
			{
				Visualizer.VisualizerFieldCollection collection = ((Visualizer) no).Fields;
				_field = collection.IndexOf(this);
				for (int n = _field + 1; n < collection.Count; n++)
				{
					((VisualizerField) collection[n])._field++;
				}
			}
		}

		private int				_field;
		private string			_title;
		private int				_defaultWidth;
		private StringAlignment _alignment; 
		private IComparer		_comparer;

		#region IVisualizerField Members

		public int Field
		{
			get { return _field; }
		}

		public string Title
		{
			get { return _title; }
		}

		public int DefaultWidth
		{
			get { return _defaultWidth; }
		}

		public System.Drawing.StringAlignment Alignment
		{
			get { return _alignment; }
		}

		public IComparer Comparer
		{
			get { return _comparer; }
		}

		#endregion

		public override string ToString()
		{
			return _title;
		}
	}

	public class Visualizer : IVisualizer
	{
		#region VisualizerFieldCollection

		public class VisualizerFieldCollection : Sport.Common.GeneralCollection
		{
			public VisualizerFieldCollection(Visualizer visualizer)
				: base(visualizer)
			{
			}

			public VisualizerField this[int index]
			{
				get { return (VisualizerField) GetItem(index); }
				set { SetItem(index, value); }
			}

			public void Insert(int index, VisualizerField value)
			{
				InsertItem(index, value);
			}

			public void Remove(VisualizerField value)
			{
				RemoveItem(value);
			}

			public bool Contains(VisualizerField value)
			{
				return base.Contains(value);
			}

			public int IndexOf(VisualizerField value)
			{
				return base.IndexOf(value);
			}

			public int Add(VisualizerField value)
			{
				return AddItem(value);
			}
		}

		#endregion

		public Visualizer()
		{
			_fields = new VisualizerFieldCollection(this);
		}

		private VisualizerFieldCollection _fields;

		public VisualizerFieldCollection Fields
		{
			get { return _fields; }
		}

		#region IVisualizer Members

		public int GetFieldCount()
		{
			return _fields.Count;
		}

		public IVisualizerField GetField(int field)
		{
			return _fields[field];
		}

		public IVisualizerField[] GetFields()
		{
			IVisualizerField[] fields = new IVisualizerField[_fields.Count];
			_fields.CopyTo(fields, 0);
			return fields;
		}

		public virtual string GetText(object o, int field)
		{
			return null;
		}

		#endregion
	}

	public class TableDesign
	{
		private IVisualizer _visualizer;
		public IVisualizer Visualizer
		{
			get { return _visualizer; }
		}

		private ColumnCollection _columns;
		public ColumnCollection Columns
		{
			get { return _columns; }
		}

		public TableDesign(IVisualizer visualizer)
		{
			_visualizer = visualizer;
			_columns = new ColumnCollection(this);
		}

		#region Column

		public class Column : Sport.Common.GeneralCollection.CollectionItem
		{
			private int _field;
			public int Field
			{
				get { return _field; }
			}

			private string _title;
			public string Title
			{
				get { return _title; }
				set { _title = value; }
			}

			private int _width;
			public int Width
			{
				get { return _width; }
				set { _width = value; }
			}

			private StringAlignment _alignment; 
			public StringAlignment Alignment
			{
				get { return _alignment; }
				set { _alignment = value; }
			}

			private IComparer _comparer;
			public IComparer Comparer
			{
				get { return _comparer; }
				set { _comparer = value; }
			}

			public Column(int field)
			{
				_field = field;
			}

			public override void OnOwnerChange(object oo, object no)
			{
				base.OnOwnerChange (oo, no);

				if (no != null)
				{
					TableDesign tableDesign = (TableDesign) no;

					if (_field >= tableDesign._visualizer.GetFieldCount())
						throw new Exception("Visualizer does not contain the field");

					IVisualizerField visualizerField = tableDesign._visualizer.GetField(_field);

					if (_title == null)
						_title = visualizerField.Title;
					if (_width == 0)
						_width = visualizerField.DefaultWidth;
					if (_comparer == null)
						_comparer = visualizerField.Comparer;
					_alignment = visualizerField.Alignment;
				}
			}

		}

		#endregion

		#region ColumnCollection

		public class ColumnCollection : Sport.Common.GeneralCollection
		{
			public ColumnCollection(TableDesign tableDesign)
				: base(tableDesign)
			{
			}

			public Column this[int index]
			{
				get { return (Column) GetItem(index); }
				set { SetItem(index, value); }
			}

			public void Insert(int index, Column value)
			{
				InsertItem(index, value);
			}

			public void Remove(Column value)
			{
				RemoveItem(value);
			}

			public bool Contains(Column value)
			{
				return base.Contains(value);
			}

			public int IndexOf(Column value)
			{
				return base.IndexOf(value);
			}

			public int Add(Column value)
			{
				return AddItem(value);
			}
		}

		#endregion
	}
}
