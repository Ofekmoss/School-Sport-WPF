using System;
using System.Windows.Forms;
using Sport.Common;

namespace Sport.UI.Controls
{
	/// <summary>
	/// GenericItemType defines the type of control in use
	/// for a GenericItem
	/// </summary>
	public enum GenericItemType
	{
		Text,			// Uses a TextBox control, values: 0 - ITextController
		Memo,			// Uses a multiline TextBox control
		Check,			// Uses a CheckBox control
		Radio,			// Uses a RadioButton control
		DateTime,		// Uses a DateTimePicker control, values: 0 - format
		Number,			// Uses a NumericUpDown control, values: 0 - min, 1 - max, 2 - scale, 3 - precision
		Selection,		// Uses a ComboBox control, values: items
		Button,			// Uses a ButtonBox control, values: 0 - selector
		Grid			// Uses a GridControl control, values: columns
	}

	/// <summary>
	/// IGenericContainer interface defines the functions needed
	/// for a control that uses GenericItem items
	/// </summary>
	public interface IGenericContainer
	{
		// Called by the GenericItem to determin the border needed
		// for the control
		BorderStyle GenericItemBorder { get; }

		// Called by GenericItem when the given control need to
		// be removed from the container
		void RemoveControl(GenericItem item, Control control);
		// Called by GenericItem when the given control need to
		// be added to the container
		void AddControl(GenericItem item, Control control);
		// Called by GenericItem when the value of the item
		// changes
		void OnValueChange(GenericItem item, object ov, object nv);
		// Called by GenericItem when a change in the item might
		// need a reposition of the item
		void ResetItems();
	}

	#region GenericItem Class

	/// <summary>
	/// GenericItem stores a control and its label to be
	/// used on IGenericContainer implementing controls.
	/// Each generic item is created with a given type by which
	/// the item's control is created. The GenericItem class
	/// gives a unified interface for different types of
	/// controls.
	/// </summary>
	public class GenericItem : GeneralCollection.CollectionItem
	{
		public IGenericContainer Container
		{
			get { return (IGenericContainer) base.Owner; }
		}
		/// <summary>
		/// Overrides the CollectionItem OnOwnerChange to call 
		/// previous owner to remove the GenericItem controls and
		/// to call the new owner to add the GenericItem controls.
		/// </summary>
		public override void OnOwnerChange(object oo, object no)
		{
			IGenericContainer o = oo as IGenericContainer;
			if (o != null)
			{
				if (label != null)
					o.RemoveControl(this, label);
				o.RemoveControl(this, control);
				o.ResetItems();
			}

			if (no != null)
			{
				IGenericContainer n = no as IGenericContainer;
				if (n == null)
					throw new ArgumentException("Owner of generic item must implement IGenericContainer");

				if (label != null)
					n.AddControl(this, label);
				BorderStyle = n.GenericItemBorder;
				n.AddControl(this, control);
				n.ResetItems();
			}

			base.OnOwnerChange (oo, no);
		}

		#region Control Operations

		public System.Windows.Forms.RightToLeft RightToLeft
		{
			get { return control.RightToLeft; }
			set { control.RightToLeft = value; }
		}

		public bool Focus()
		{
			return control.Focus();
		}

		public bool Enabled
		{
			get { return control.Enabled; }
			set { control.Enabled = value; }
		}

		/// <summary>
		/// Called when the values of the GenericItem changes.
		/// The GenericItem will set the values to the control
		/// according to its type.
		/// </summary>
		private void ResetValues()
		{
			if (control == null)
				return ;

			switch (_itemType)
			{
				case (GenericItemType.Text):
					if (_values == null || _values.Length == 0)
					{
						((TextControl) control).Controller = null;
					}
					else
					{
						((TextControl) control).Controller = _values[0] as ITextController;
						if (_values.Length > 1)
						{
							((TextControl) control).ShowSpin = (bool) _values[1];
						}
					}
					break;
				case (GenericItemType.DateTime):
					if (_values != null && !_readOnly)
					{
						if (_values.Length > 0)
						{
							((DateTimePicker) control).Format = DateTimePickerFormat.Custom;
							((DateTimePicker) control).CustomFormat = _values[0] as string;
						}
					}
					break;
				case (GenericItemType.Number):
					if (_values == null)
					{
						((TextControl) control).Controller = new NumberController();
					}
					else
					{
						double min = 0, max = double.MaxValue;
						byte scale = byte.MaxValue, precision = 0;

						if (_values.Length > 0)
						{
							min = Sport.Common.Tools.ToDoubleFast(_values[0], true);
							if (_values.Length > 1)
							{
								max = Sport.Common.Tools.ToDoubleFast(_values[1], true);
								if (_values.Length > 2)
								{
									try
									{
										//for some reason integer can't be converted
										scale = (byte) _values[2];
									}
									catch (Exception e)
									{
										System.Diagnostics.Debug.WriteLine("failed to convert "+_values[2].ToString()+" into byte.\n");
										System.Diagnostics.Debug.WriteLine(e.Message+"\n"+e.StackTrace);
										scale = byte.MaxValue;
									}

									if (_values.Length > 3)
									{
										try
										{
											precision = (byte) _values[3];
										}
										catch (Exception e)
										{
											System.Diagnostics.Debug.WriteLine("failed to convert "+_values[3].ToString()+" into byte.\n");
											System.Diagnostics.Debug.WriteLine(e.Message+"\n"+e.StackTrace);
											precision = 0;
										}
									}
								}
							}
						}

						((TextControl) control).Controller = new NumberController(min, max, scale, precision);
					}
					break;
				case (GenericItemType.Selection):
					if (!_readOnly)
					{
						((NullComboBox) control).Items.Clear();
						if (_values != null)
						{
							((NullComboBox) control).Items.AddRange(_values);
							if (_nullable)
								((NullComboBox) control).Items.Add(NullComboBox.Null);
							ResetValue();
						}
					}
					break;
				case (GenericItemType.Button):
					if (_values != null && _values.Length > 0 && !_readOnly)
					{
						((ButtonBox) control).ValueSelector = (ButtonBox.SelectValue) _values[0];
					}
					break;
				case (GenericItemType.Grid):
					GridControl.Column[] columns = _values as GridControl.Column[];
					((GridControl) control).SetColumns(columns);
					break;
			}
		}

		/// <summary>
		/// Called when the value of the GenericItem changes.
		/// The GenericItem will set the value to the control
		/// according to its type.
		/// </summary>
		private void ResetValue()
		{
			if (control == null)
				return ;

			switch (_itemType)
			{
				case (GenericItemType.Text):
					((TextControl) control).Value = _value;
					break;
				case (GenericItemType.Memo):
					if (_value == null)
						((TextBox) control).Text = null;
					else
						((TextBox) control).Lines = _value.ToString().Split(new char[] {'\n'});
					break;
				case (GenericItemType.Check):
					if (_value == null)
						_value = false;
					((CheckBox) control).Checked = (bool) _value;
					break;
				case (GenericItemType.Radio):
					if (_value == null)
						_value = false;
					((RadioButton) control).Checked = (bool) _value;
					break;
				case (GenericItemType.DateTime):
					if (_readOnly)
					{
						if (_value == null)
						{
							((TextBox) control).Text = null;
						}
						else if (_values != null && _values.Length > 0)
						{
							((TextBox) control).Text = ((DateTime) _value).ToString(_values[0] as string);
						}
						else
						{
							((TextBox) control).Text = ((DateTime) _value).ToString();
						}
					}
					else
					{
						if (_value == null)
						{
							((DateTimePicker) control).Checked = false;
						}
						else
						{
							((DateTimePicker) control).Checked = true;
							((DateTimePicker) control).Value = (DateTime) _value;
						}
					}
					break;
				case (GenericItemType.Number):
					((TextControl) control).Value = _value;
					// Value type might be changed if not double
					_value = ((TextControl) control).Value;
					break;
				case (GenericItemType.Selection):
					if (_readOnly)
					{
						((TextBox) control).Text = _value == null ? null :
							_value.ToString();
					}
					else
					{
						((NullComboBox) control).SelectedItem = _value;
					}
					break;
				case (GenericItemType.Button):
					if (_readOnly)
					{
						((TextBox) control).Text = _value == null ? null :
							_value.ToString();
					}
					else
					{
						((ButtonBox) control).Value = _value;
					}
					break;
				case (GenericItemType.Grid):
					((GridControl) control).Grid.Source = _value as IGridSource;
					break;
			}
		}

		/// <summary>
		/// Called when the value of the control changes.
		/// The GenericItem will set the value member of according
		/// to the control's value.
		/// </summary>
		private void GetValue()
		{
			if (control == null || _readOnly)
				return ;

			switch (_itemType)
			{
				case (GenericItemType.Text):
					_value = ((TextControl) control).Value;
					break;
				case (GenericItemType.Memo):
					_value = ((TextBox) control).Text;
					break;
				case (GenericItemType.Check):
					_value = ((CheckBox) control).Checked;
					break;
				case (GenericItemType.Radio):
					_value = ((RadioButton) control).Checked;
					break;
				case (GenericItemType.DateTime):
					if (((DateTimePicker) control).Checked)
						_value = ((DateTimePicker) control).Value;
					else
						_value = null;
					break;
				case (GenericItemType.Number):
					_value = ((TextControl) control).Value;
					break;
				case (GenericItemType.Selection):
					_value = ((NullComboBox) control).SelectedItem;
					break;
				case (GenericItemType.Button):
					_value = ((ButtonBox) control).Value;
					break;
				case (GenericItemType.Grid):
					_value = ((GridControl) control).Grid.Source;
					break;
			}
		}

		/// <summary>
		/// Called when the type of control is set.
		/// The GenericItem will recreate the control according
		/// to its type.
		/// </summary>
		private void ResetControl(System.Drawing.Size controlSize)
		{
			IGenericContainer owner = Owner as IGenericContainer;
			BorderStyle border = BorderStyle;
			if (owner != null)
				border = owner.GenericItemBorder;

			Control ctrl = null;
			switch (_itemType)
			{
				case (GenericItemType.Text):
				case (GenericItemType.Number):
					ctrl = new TextControl();
					((TextControl) ctrl).BorderStyle = border;
					((TextControl) ctrl).ReadOnly = _readOnly;
					((TextControl) ctrl).ShowSpin = _itemType == GenericItemType.Number;
					((TextControl) ctrl).TextChanged += new EventHandler(GenericItemChanged);
					break;
				case (GenericItemType.Memo):
					ctrl = new TextBox();
					((TextBox) ctrl).BorderStyle = border;
					((TextBox) ctrl).ReadOnly = _readOnly;
					((TextBox) ctrl).AutoSize = false;
					((TextBox) ctrl).Multiline = true;
					((TextBox) ctrl).TextChanged += new EventHandler(GenericItemChanged);
					break;
				case (GenericItemType.Check):
					ctrl = new CheckBox();
					((CheckBox) ctrl).Text = _title;
					((CheckBox) ctrl).CheckedChanged += new EventHandler(GenericItemChanged);
					break;
				case (GenericItemType.Radio):
					ctrl = new RadioButton();
					((RadioButton) ctrl).Text = _title;
					((RadioButton) ctrl).CheckedChanged += new EventHandler(GenericItemChanged);
					break;
				case (GenericItemType.DateTime):
					if (_readOnly)
					{
						ctrl = new TextBox();
						((TextBox) ctrl).BorderStyle = border;
						((TextBox) ctrl).ReadOnly = true;
					}
					else
					{
						ctrl = new DateTimePicker();
						((DateTimePicker) ctrl).ShowCheckBox = _nullable;
						((DateTimePicker) ctrl).ValueChanged += new EventHandler(GenericItemChanged);
					}
					break;
				case (GenericItemType.Selection):
					if (_readOnly)
					{
						ctrl = new TextBox();
						((TextBox) ctrl).BorderStyle = border;
						((TextBox) ctrl).ReadOnly = true;
					}
					else
					{
						ctrl = new NullComboBox();
						((NullComboBox) ctrl).SelectedIndexChanged += new EventHandler(GenericItemChanged);
					}
					break;
				case (GenericItemType.Button):
					if (_readOnly)
					{
						ctrl = new TextBox();
						((TextBox) ctrl).BorderStyle = border;
						((TextBox) ctrl).ReadOnly = true;
					}
					else
					{
						ctrl = new ButtonBox();
						((ButtonBox) ctrl).BorderStyle = border;
						((ButtonBox) ctrl).ValueChanged +=new EventHandler(GenericItemChanged);
					}
					break;
				case (GenericItemType.Grid):
					ctrl = new GridControl();
					((GridControl) ctrl).BorderStyle = border;
					((GridControl) ctrl).Grid.Editable = !_readOnly;
					break;
			}

			if (control != null)
			{
				if (owner != null)
					owner.RemoveControl(this, control);

				if (ctrl != null)
				{
					ctrl.Location = control.Location;
					ctrl.Size = control.Size;
					ctrl.TabIndex = control.TabIndex;
				}
			}

			if (controlSize != System.Drawing.Size.Empty)
			{
				if (controlSize.Width > 0 && controlSize.Height > 0)
					ctrl.Size = controlSize;
				else if (controlSize.Width > 0)
					ctrl.Width = controlSize.Width;
				else if (controlSize.Height > 0)
					ctrl.Height = controlSize.Height;
			}

			control = ctrl;

			if (control != null)
			{
				ResetValues();
				ResetValue();

				if (owner != null)
					owner.AddControl(this, control);
			}
		}

		private void SetReadOnly()
		{
			if (control != null)
			{
				switch (_itemType)
				{
					case (GenericItemType.Text):
					case (GenericItemType.Number):
						((TextControl) control).ReadOnly = _readOnly;
						break;
					case (GenericItemType.Memo):
						((TextBox) control).ReadOnly = _readOnly;
						break;
					case (GenericItemType.Check):
					case (GenericItemType.Radio):
						control.Enabled = !_readOnly;
						break;
					case (GenericItemType.DateTime):
						ResetControl(System.Drawing.Size.Empty);
						break;
					case (GenericItemType.Selection):
						ResetControl(System.Drawing.Size.Empty);
						break;
					case (GenericItemType.Button):
						ResetControl(System.Drawing.Size.Empty);
						break;
					case (GenericItemType.Grid):
						((GridControl) control).Grid.Editable = !_readOnly;
						break;
				}
			}
		}

		private void SetNullable()
		{
			if (control != null && !_readOnly)
			{
				switch (_itemType)
				{
					case (GenericItemType.DateTime):
						((DateTimePicker) control).ShowCheckBox = _nullable;
						break;
					case (GenericItemType.Selection):
						if (_nullable)
						{
							((NullComboBox) control).Items.Add(NullComboBox.Null);
							if (((NullComboBox) control).SelectedItem == null)
								((NullComboBox) control).SelectedItem = NullComboBox.Null;
						}
						else
						{
							object selectedItem = ((NullComboBox) control).SelectedItem;
							((NullComboBox) control).Items.Remove(NullComboBox.Null);
							((NullComboBox) control).SelectedItem = selectedItem;
						}
						break;
				}
			}
		}

		#endregion

		public event EventHandler ValueChanged;

		/// <summary>
		/// OnValueChange is called when the value of the control changes.
		/// </summary>
		private void OnValueChange(object ov, object nv)
		{
			IGenericContainer owner = Owner as IGenericContainer;
			if (owner != null)
				owner.OnValueChange(this, ov, nv);

			if (ValueChanged != null)
				ValueChanged(this, EventArgs.Empty);
		}

		private void GenericItemChanged(object sender, EventArgs e)
		{
			object old = _value;
			GetValue();
			if (old != _value)
				OnValueChange(old, _value);
		}

		#region Properties

		#region Controls Properties

		private Control control;
		public Control Control
		{
			get {return control;}
			/*
			get {
				//solve bug with TextControl and TextBox...
				if (control is TextControl)
				{
					return ((TextBox) (control as TextControl));
				}
				else
				{
					return control;
				}
			}
			*/
		}

		private Label label;
		public Label Label
		{
			get { return label; }
		}

		#endregion Controls Properties
        
		#region Type and Value Properties

		private bool _readOnly;
		/// <summary>
		/// Gets or sets whether the control is readonly
		/// </summary>
		public bool ReadOnly
		{
			get { return _readOnly; }
			set
			{
				if (_readOnly != value)
				{
					_readOnly = value;
					SetReadOnly();
				}
			}
		}

		private bool _nullable;
		/// <summary>
		/// Gets or sets whether the control value can be set to null
		/// </summary>
		public bool Nullable
		{
			get { return _nullable; }
			set
			{
				if (_nullable != value)
				{
					_nullable = value;
					SetNullable();
				}
			}
		}

		private GenericItemType _itemType;
		/// <summary>
		/// Gets or sets the type of the item.
		/// The type of the item defines the control that it holds.
		/// </summary>
		public GenericItemType ItemType
		{
			get { return _itemType; }
			set 
			{ 
				_itemType = value; 
				ResetControl(System.Drawing.Size.Empty);
			}
		}

		private System.Drawing.Size _controlSize;
		public System.Drawing.Size ControlSize
		{
			get { return _controlSize; }
			set { _controlSize = value; }
		}

		private object pVal = null;

		private object _value
		{
			get
			{
				if (pVal != null)
				{
					if ((pVal is int && ((int)pVal) == -1) || (pVal is long && ((long)pVal) == -1) || (pVal is double && ((double)pVal) == -1))
						pVal = null;
					if (pVal is DateTime && ((DateTime)pVal).Year < 1900)
						pVal = null;
				}
				return pVal;
			}
			set
			{
				pVal = value;
				if (pVal != null)
				{
					if ((pVal is int && ((int)pVal) == -1) || (pVal is long && ((long)pVal) == -1) || (pVal is double && ((double)pVal) == -1))
						pVal = null;
					if (pVal is DateTime && ((DateTime)pVal).Year < 1900)
						pVal = null;
				}
			}
		}

		/// <summary>
		/// Gets or sets the value of the item.
		/// The value will be set to the control according to
		/// its type.
		/// </summary>
		public object Value
		{
			get
			{
				return _value;
			}
			set 
			{ 
				_value = value;
				if ((_value is int && ((int)_value) == -1) || (_value is long && ((long)_value) == -1) || (_value is double && ((double)_value) == -1))
					_value = null;
				if (_value is DateTime && ((DateTime)_value).Year < 1900)
					_value = null;
				ResetValue();
			}
		}

		private object[] _values;
		/// <summary>
		/// Gets or sets the values of the item.
		/// The values will be set to the control according to
		/// its type.
		/// </summary>
		public object[] Values
		{
			get { return _values; }
			set 
			{ 
				_values = value; 
				ResetValues();
				GetValue();
			}
		}

		public int Add(object value)
		{
			if (_values == null)
			{
				_values = new object[1] { value };
				ResetValues();
				ResetValue();
				return 0;
			}

			object[] values = new object[_values.Length + 1];
			Array.Copy(_values, values, _values.Length);
			values[_values.Length] = value;
			_values = values;
			ResetValues();
			ResetValue();
			return _values.Length - 1;
		}

		public void AddRange(object[] values)
		{
			if (_values == null)
			{
				_values = new object[values.Length];
				Array.Copy(values, _values, 0);
			}
			else
			{
				object[] nvs = new object[_values.Length + values.Length];
				Array.Copy(_values, nvs, _values.Length);
				Array.Copy(values, 0, nvs, _values.Length, values.Length);
				_values = nvs;
			}
			ResetValues();
			ResetValue();
		}

		public void Insert(int index, object value)
		{
			if (_values == null)
			{
				_values = new object[1] { value };
			}
			else
			{
				object[] values = new object[_values.Length + 1];
				Array.Copy(_values, values, index);
				Array.Copy(_values, index, values, index + 1, _values.Length - index);
				values[index] = value;
				_values = values;
			}
			ResetValues();
			ResetValue();
		}

		public void RemoveAt(int index)
		{
			if (_values.Length == 1)
			{
				_values = null;
			}
			else
			{
				object[] values = new object[_values.Length - 1];
				Array.Copy(_values, values, index);
				Array.Copy(_values, index + 1, values, index, _values.Length - index - 1);
				_values = values;
			}
			ResetValues();
			ResetValue();
		}

		public void Remove(object value)
		{
			int index = Array.IndexOf(_values, value);
			if (index >= 0)
				RemoveAt(index);
		}

		#endregion

		#region Style Properties

		public BorderStyle BorderStyle
		{
			get
			{
				if (control != null)
				{
					if (control is TextBox)
					{
						return ((TextBox) control).BorderStyle;
					}
					else if (control is NumericUpDown)
					{
						return ((NumericUpDown) control).BorderStyle;
					}
					else if (control is ButtonBox)
					{
						return ((ButtonBox) control).BorderStyle;
					}
					else if (control is GridControl)
					{
						return ((GridControl) control).BorderStyle;
					}
				}
				
				return BorderStyle.Fixed3D;
			}
			set
			{
				if (control != null)
				{
					if (control is TextBox)
					{
						((TextBox) control).BorderStyle = value;
					}
					else if (control is NumericUpDown)
					{
						((NumericUpDown) control).BorderStyle = value;
					}
					else if (control is ButtonBox)
					{
						((ButtonBox) control).BorderStyle = value;
					}
					else if (control is GridControl)
					{
						((GridControl) control).BorderStyle = value;
					}
				}
			}
		}

		private string _title;
		/// <summary>
		/// Gets or sets the title of the GenericItem.
		/// The title of the item is stored by its label control.
		/// </summary>
		public string Title
		{
			get { return _title; }
			set 
			{ 
				IGenericContainer owner = Owner as IGenericContainer;
				_title = value;

				if (_itemType == GenericItemType.Check ||
					_itemType == GenericItemType.Button)
				{
					Control.Text = _title;
				}
				else
				{
					if (label == null)
					{
						if (value != null)
						{
							CreateLabel();
							label.Text = value;
							if (owner != null)
								owner.AddControl(this, label);
						}
					}
					else
					{
						if (value == null)
						{
							if (owner != null)
								owner.RemoveControl(this, label);
							label = null;
						}
						else
						{
							label.Text = value;
						}
					}
				}

				if (owner != null)
					owner.ResetItems();
			}
		}

		#endregion

		private object tag;
		public object Tag
		{
			get { return tag; }
			set { tag = value; }
		}
		
		#endregion

		private void CreateLabel()
		{
			label = new Label();
			label.AutoSize = true;
			label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
		}

		#region Constuctors

		// Base constructor
		public GenericItem(string title, GenericItemType itemType, object value,
			object[] values, System.Drawing.Size controlSize)
		{
			_title = title;

			_controlSize = controlSize;

			// Setting members
			_readOnly = false;
			_nullable = true;
			_itemType = itemType;
			_value = value;
			_values = values;

			// Creating label if needed
			if (_itemType != GenericItemType.Check &&
				_itemType != GenericItemType.Radio &&
				_title != null)
			{
				CreateLabel();
				label.Text = _title;
			}


			// Creating control
			ResetControl(controlSize);
		}

		#region Contructors with title

		public GenericItem(string title, GenericItemType itemType, object value,
			object[] values)
			: this(title, itemType, value, values, System.Drawing.Size.Empty)
		{
		}

		public GenericItem(string title, GenericItemType itemType, object value)
			: this(title, itemType, value, null)
		{
		}

		public GenericItem(string title, GenericItemType itemType, System.Drawing.Size controlSize)
			: this(title, itemType, null, null, controlSize)
		{
		}

		public GenericItem(string title, GenericItemType itemType)
			: this(title, itemType, null, null)
		{
		}

		#endregion

		#region Contructors without title

		public GenericItem(GenericItemType itemType, object value, object[] values, System.Drawing.Size controlSize)
			: this(null, itemType, value, values, controlSize)
		{
		}

		public GenericItem(GenericItemType itemType, object value, object[] values)
			: this(null, itemType, value, values)
		{
		}

		public GenericItem(GenericItemType itemType, object value)
			: this(null, itemType, value)
		{
		}
		
		public GenericItem(GenericItemType itemType, System.Drawing.Size controlSize)
			: this(null, itemType, null, null, controlSize)
		{
		}

		public GenericItem(GenericItemType itemType)
			: this(null, itemType)
		{
		}

		#endregion

		#endregion

		// GenericItemType.DateTime values: 0 - format
		public static object[] DateTimeValues(string format)
		{
			return new object[] { format };
		}

		// GenericItemType.Number values: 0 - min, 1 - max, 2 - scale, 3 - precision
		public static object[] NumberValues(double min, double max, byte scale, byte precision)
		{
			return new object[] { min, max, scale, precision };
		}

		// GenericItemType.Button values: 0 - selector
		public static object[] ButtonValues(ButtonBox.SelectValue valueSelector)
		{
			return new object[] { valueSelector };
		}

		// GenericItemType.Text values: 0 - ITextController
		public static object[] TextValues(ITextController textController, bool showSpin)
		{
			return new object[] { textController, showSpin };
		}
	}

	#endregion

	#region GenericItemCollection

	public class GenericItemCollection : GeneralCollection
	{
		public GenericItemCollection(IGenericContainer owner)
			: base(owner)
		{
		}

		public GenericItem this[int index]
		{
			get { return (GenericItem) GetItem(index); }
			set { SetItem(index, value); }
		}

		public void Insert(int index, GenericItem value)
		{
			InsertItem(index, value);
		}

		public void Insert(int index, string title, GenericItemType itemType, object value,
			object[] values, System.Drawing.Size controlSize)
		{
			InsertItem(index, new GenericItem(title, itemType, value, values, controlSize));
		}

		public void Insert(int index, string title, GenericItemType itemType, object value,
			object[] values)
		{
			InsertItem(index, new GenericItem(title, itemType, value, values));
		}

		public void Insert(int index, string title, GenericItemType itemType, object value)
		{
			InsertItem(index, new GenericItem(title, itemType, value));
		}

		public void Insert(int index, string title, GenericItemType itemType, System.Drawing.Size controlSize)
		{
			InsertItem(index, new GenericItem(title, itemType, controlSize));
		}

		public void Insert(int index, string title, GenericItemType itemType)
		{
			InsertItem(index, new GenericItem(title, itemType));
		}

		public void Insert(int index, GenericItemType itemType, object value,
			object[] values, System.Drawing.Size controlSize)
		{
			InsertItem(index, new GenericItem(itemType, value, values, controlSize));
		}

		public void Insert(int index, GenericItemType itemType, object value,
			object[] values)
		{
			InsertItem(index, new GenericItem(itemType, value, values));
		}

		public void Insert(int index, GenericItemType itemType, object value)
		{
			InsertItem(index, new GenericItem(itemType, value));
		}

		public void Insert(int index, GenericItemType itemType, System.Drawing.Size controlSize)
		{
			InsertItem(index, new GenericItem(itemType, controlSize));
		}

		public void Insert(int index, GenericItemType itemType)
		{
			InsertItem(index, new GenericItem(itemType));
		}

		public void Remove(GenericItem value)
		{
			RemoveItem(value);
		}

		public bool Contains(GenericItem value)
		{
			return base.Contains(value);
		}

		public int IndexOf(GenericItem value)
		{
			return base.IndexOf(value);
		}

		public int Add(GenericItem value)
		{
			return AddItem(value);
		}

		public int Add(string title, GenericItemType itemType, object value,
			object[] values, System.Drawing.Size controlSize)
		{
			return AddItem(new GenericItem(title, itemType, value, values, controlSize));
		}

		public int Add(string title, GenericItemType itemType, object value,
			object[] values)
		{
			return AddItem(new GenericItem(title, itemType, value, values));
		}

		public int Add(string title, GenericItemType itemType, object value)
		{
			return AddItem(new GenericItem(title, itemType, value));
		}

		public int Add(string title, GenericItemType itemType, System.Drawing.Size controlSize)
		{
			return AddItem(new GenericItem(title, itemType, controlSize));
		}

		public int Add(string title, GenericItemType itemType)
		{
			return AddItem(new GenericItem(title, itemType));
		}

		public int Add(GenericItemType itemType, object value,
			object[] values, System.Drawing.Size controlSize)
		{
			return AddItem(new GenericItem(itemType, value, values, controlSize));
		}

		public int Add(GenericItemType itemType, object value,
			object[] values)
		{
			return AddItem(new GenericItem(itemType, value, values));
		}

		public int Add(GenericItemType itemType, object value)
		{
			return AddItem(new GenericItem(itemType, value));
		}

		public int Add(GenericItemType itemType, System.Drawing.Size controlSize)
		{
			return AddItem(new GenericItem(itemType, controlSize));
		}

		public int Add(GenericItemType itemType)
		{
			return AddItem(new GenericItem(itemType));
		}
	}

	#endregion

}
