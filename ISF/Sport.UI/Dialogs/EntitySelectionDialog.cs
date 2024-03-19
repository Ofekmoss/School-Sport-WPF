using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Sport.Data;
using Sport.UI.Controls;

namespace Sport.UI
{
	/// <summary>
	/// Summary description for DialogForm.
	/// </summary>
	public class EntitySelectionDialog : DialogForm
	{
		private Entity _entity;
		public Entity Entity
		{
			get { return _entity; }
			set { _entity = value; }
		}

		/// <summary>
		/// selected entities in case of multiple selection.
		/// </summary>
		private Entity[] _entities = null;
		public Entity[] Entities
		{
			get { return _entities; }
			set { _entities = value; }
		}

		private bool _multiple;
		public bool Multiple
		{
			get { return _multiple; }
			set
			{
				if (_multiple != value)
				{
					_multiple = value;
					((ITableView) _view).EntitiesSelection = _multiple;
				}
			}
		}

		private System.Windows.Forms.CheckedListBox selectedItems;

		public EntitySelectionDialog(ITableView view, string ok, string cancel, bool multiple, Size clientSize)
			: base(view as View, new DialogResult[] { DialogResult.OK, DialogResult.Cancel }, new string[] { ok, cancel }, clientSize)
		{
			selectedItems = new System.Windows.Forms.CheckedListBox();
			selectedItems.Anchor = System.Windows.Forms.AnchorStyles.Top | 
				System.Windows.Forms.AnchorStyles.Bottom |
				System.Windows.Forms.AnchorStyles.Left;
			selectedItems.Location = new System.Drawing.Point(8, 8);
			selectedItems.Name = "selectedItems";
			selectedItems.Size = new System.Drawing.Size(100, 304);
			selectedItems.Visible = false;
			selectedItems.TabIndex = 2;
			selectedItems.ItemCheck += new ItemCheckEventHandler(SelectedItemsChecked);

			Controls.Add(selectedItems);

			Multiple = multiple;

			view.SelectionMode = SelectionMode.One;
			view.Editable = false;
			view.DetailsBarEnabled = false;
			view.ToolBarEnabled = false;
			((View) view).Opened += new System.EventHandler(ViewOpened);
			((View) view).Closed += new System.EventHandler(ViewClosed);

			view.SelectedEntitiesChanged += new EventHandler(SelectedEntitiesChanged);
		}

		public EntitySelectionDialog(ITableView view, string ok, string cancel, bool multiple)
			: this(view, ok, cancel, multiple, Size.Empty)
		{
		}

		public EntitySelectionDialog(ITableView view, string ok, string cancel, Size clientSize)
			: this(view, ok, cancel, false, clientSize)
		{
		}

		public EntitySelectionDialog(ITableView view, string ok, string cancel)
			: this(view, ok, cancel, false)
		{
		}

		public EntitySelectionDialog(ITableView view, bool multiple, Size clientSize)
			: this(view, "אישור", "ביטול", multiple, clientSize)
		{
		}

		public EntitySelectionDialog(ITableView view, bool multiple)
			: this(view, multiple, Size.Empty)
		{
		}

		public EntitySelectionDialog(ITableView view, Size clientSize)
			: this (view, "אישור", "ביטול", clientSize)
		{
		}

		public EntitySelectionDialog(ITableView view)
			: this (view, "אישור", "ביטול", Size.Empty)
		{
		}

		private void ViewOpened(object sender, EventArgs e)
		{
			ITableView view = (ITableView) _view;

			view.Current = _entity;

			if (_multiple)
			{
				view.SelectedEntities = _entities;
			}
		}

		private void ViewClosed(object sender, EventArgs e)
		{
			ITableView view=(ITableView) _view;
			_entity = view.Current;

			if (_multiple)
			{
				_entities = view.SelectedEntities;
			}
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing (e);

			if (DialogResult == DialogResult.OK && ((ITableView) _view).Editing)
			{
				if (!((ITableView) _view).Save())
					e.Cancel = true;
			}
		}

		private View previousView = null;

		public override bool ExecuteCommand(string command)
		{
			if (command == "ENTER")
			{
				this.DialogResult = DialogResult.OK;
				return true;
			}									   
			return base.ExecuteCommand (command);
		}


		protected override void OnDialogLoaded()
		{
			// The current view is deactivated
			previousView = ViewManager.SelectedView;
			if (previousView != null)
				previousView.Deactivate();

			base.OnDialogLoaded ();
		}


		protected override void OnDialogClosed()
		{
			base.OnDialogClosed ();

			// Reactivating last selected view
			if (previousView != null)
				previousView.Activate();
		}


		public object ValueSelector(ButtonBox buttonBox, object value)
		{
			Entity result = value as Entity;

			Entity = result;

			if (ShowDialog() == DialogResult.OK)
			{
				result = Entity;
			}

			return result;
		}

		private bool settingSelectedEntities = false;

		private void SelectedEntitiesChanged(object sender, EventArgs e)
		{
			settingSelectedEntities = true;
			selectedItems.Items.Clear();
			selectedItems.Items.AddRange(((ITableView) _view).SelectedEntities);
			for (int n = 0; n < selectedItems.Items.Count; n++)
				selectedItems.SetItemChecked(n, true);
			settingSelectedEntities = false;
		}

		private void SelectedItemsChecked(object sender, ItemCheckEventArgs e)
		{
			if (!settingSelectedEntities)
			{
				ITableView view = (ITableView) _view;
				view.SetEntityCheck((Entity) selectedItems.Items[e.Index], false);
			}
		}
	}
}
