using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Sport.UI.Dialogs
{
	/// <summary>
	/// Summary description for SortForm.
	/// </summary>
	public class SortForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private static readonly string ascending = "עולה";
		private static readonly string descending = "יורד";

		private int[] _sort;
	
		public int[] Sort
		{
			get { return _sort; }
		}

		private object[] _items;

		private System.Collections.ArrayList panels;

		public SortForm(object[] items, int[] sort)
		{
			_items = items;
			_sort = sort;

			panels = new ArrayList();
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			SetControls();
		}

		private void SetControls()
		{
			SuspendLayout();
			foreach (Control control in panels)
			{
				Controls.Remove(control);
			}
			panels.Clear();

			System.Collections.ArrayList al = new ArrayList();
			al.AddRange(_items);
			int top = 0;
			int index;
			bool asc;

			Sport.UI.Controls.GenericPanel panel;

			if (_sort != null && _sort.Length > 0)
			{
				for (int n = 0; n < _sort.Length - 1; n++)
				{
					if (_sort[n] < 0)
					{
						index = _sort[n] - Int32.MinValue;
						asc = false;
					}
					else
					{
						index = _sort[n];
						asc = true;
					}

					al.Remove(_items[index]);
					panel = new Sport.UI.Controls.GenericPanel();
					panel.ItemsLayout = new Sport.UI.Controls.GenericDefaultLayout();
					panel.Size = new Size(376, 32);
					panel.Location = new Point(8, top + 8);
					panel.Items.Add("שדה:", Sport.UI.Controls.GenericItemType.Selection,
						_items[index], new object[] { _items[index] }, new Size(192, 21));
					panel.Items.Add("כיוון:", Sport.UI.Controls.GenericItemType.Selection,
						asc ? ascending : descending, 
						new object[] { ascending, descending }, new Size(96, 21));
					((Sport.UI.Controls.NullComboBox) panel.Items[0].Control).Text = "ללא";
					panel.Items[0].Enabled = false;
					panel.Tag = n;
					panel.Items[0].ValueChanged += new EventHandler(SortFieldChanged);
					panel.Items[1].ValueChanged += new EventHandler(SortDirectionChanged);
					Controls.Add(panel);
					panels.Add(panel);

					top += 40;
				}

				if (_sort[_sort.Length - 1] < 0)
				{
					index = _sort[_sort.Length - 1] - Int32.MinValue;
					asc = false;
				}
				else
				{
					index = _sort[_sort.Length - 1];
					asc = true;
				}

				panel = new Sport.UI.Controls.GenericPanel();
				panel.ItemsLayout = new Sport.UI.Controls.GenericDefaultLayout();
				panel.Size = new Size(376, 32);
				panel.Location = new Point(8, top + 8);
				panel.Items.Add("שדה:", Sport.UI.Controls.GenericItemType.Selection,
					_items[index], al.ToArray(), new Size(192, 21));
				panel.Items.Add("כיוון:", Sport.UI.Controls.GenericItemType.Selection,
					asc ? ascending : descending, 
					new object[] { ascending, descending }, new Size(96, 21));
				((Sport.UI.Controls.NullComboBox) panel.Items[0].Control).Text = "ללא";
				panel.Tag = _sort.Length - 1;
				panel.Items[0].ValueChanged += new EventHandler(SortFieldChanged);
				panel.Items[1].ValueChanged += new EventHandler(SortDirectionChanged);
				Controls.Add(panel);
				panels.Add(panel);

				al.Remove(_items[index]);

				top += 40;
			}

			if (al.Count > 1)
			{
				panel = new Sport.UI.Controls.GenericPanel();
				panel.ItemsLayout = new Sport.UI.Controls.GenericDefaultLayout();
				panel.Size = new Size(376, 32);
				panel.Location = new Point(8, top + 8);
				panel.Items.Add("שדה:", Sport.UI.Controls.GenericItemType.Selection,
					Sport.UI.Controls.NullComboBox.Null, al.ToArray(), new Size(192, 21));
				panel.Items.Add("כיוון:", Sport.UI.Controls.GenericItemType.Selection,
					null, new object[] { ascending, descending }, new Size(96, 21));
				panel.Items[1].Enabled = false;
				((Sport.UI.Controls.NullComboBox) panel.Items[0].Control).Text = "ללא";
				panel.Tag = -1;
				panel.Items[0].ValueChanged += new EventHandler(SortFieldChanged);
				panel.Items[1].ValueChanged += new EventHandler(SortDirectionChanged);
				Controls.Add(panel);
				panels.Add(panel);
				top += 40;
			}

			Height = top + 72;

			ResumeLayout();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(8, 48);
			this.btnOK.Name = "btnOK";
			this.btnOK.TabIndex = 0;
			this.btnOK.Text = "אישור";
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(88, 48);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.TabIndex = 1;
			this.btnCancel.Text = "ביטול";
			// 
			// SortForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.ClientSize = new System.Drawing.Size(392, 78);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.Name = "SortForm";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "מיון";
			this.ResumeLayout(false);

		}
		#endregion

		private void SortFieldChanged(object sender, EventArgs e)
		{
			Sport.UI.Controls.GenericItem item = sender as Sport.UI.Controls.GenericItem;
			Sport.UI.Controls.GenericPanel panel = item.Container as Sport.UI.Controls.GenericPanel;
			int n = (int) panel.Tag;

			if (n == -1)
			{
				if (item.Value != null)
				{
					int index = Array.IndexOf(_items, item.Value);
					int[] sort = new int[_sort.Length + 1];
					Array.Copy(_sort, sort, _sort.Length);
					sort[_sort.Length] = index;
					_sort = sort;
					SetControls();
				}
			}
			else if (n == _sort.Length - 1)
			{
				if (item.Value == null)
				{
					int[] sort = new int[_sort.Length - 1];
					Array.Copy(_sort, sort, _sort.Length - 1);
					_sort = sort;
					SetControls();
				}
				else
				{
					if (n < panels.Count - 1)
					{
						Sport.UI.Controls.GenericPanel next = panels[n + 1] as Sport.UI.Controls.GenericPanel;
						next.Items[0].Values = item.Values;
						next.Items[0].Remove(item.Value);
					}

					int index = Array.IndexOf(_items, item.Value);
					if (_sort[n] < 0)
						_sort[n] = index + Int32.MinValue;
					else
						_sort[n] = index;
				}
			}
		}

		private void SortDirectionChanged(object sender, EventArgs e)
		{
			Sport.UI.Controls.GenericItem item = sender as Sport.UI.Controls.GenericItem;
			Control panel = item.Container as Control;
			int n = (int) panel.Tag;
			int index = _sort[n];
			if (_sort[n] < 0)
				index -= Int32.MinValue;


			if (item.Value == (object) descending)
				index += Int32.MinValue;
			_sort[n] = index;
		}
	}
}
