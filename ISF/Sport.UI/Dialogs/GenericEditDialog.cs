using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Sport.Common;
using Sport.UI.Controls;

namespace Sport.UI.Dialogs
{
	/// <summary>
	/// Summary description for GenericEditDialog.
	/// </summary>
	public class GenericEditDialog : System.Windows.Forms.Form,
		IGenericContainer
	{
		private bool _confirmable;
		public bool Confirmable
		{
			get { return _confirmable; }
			set
			{
				if (_confirmable != value)
				{
					_confirmable = value;
					tbOk.Enabled = _confirmable;
				}
			}
		}

		public string Title
		{
			get { return lbTitle.Text; }
			set { lbTitle.Text = value; }
		}

		private GenericItemCollection _items;
		public GenericItemCollection Items
		{
			get { return _items; }
		}

		private System.Windows.Forms.Panel panel;
		private System.Windows.Forms.Button tbOk;
		private System.Windows.Forms.Button tbCancel;

		private System.ComponentModel.Container components = null;
		internal System.Windows.Forms.Panel panelItems;
		private System.Windows.Forms.Label lbTitle;

		public GenericEditDialog(string title)
		{
			_items = new GenericItemCollection(this);
			_items.Changed += new CollectionEventHandler(ItemsChanged);

			InitializeComponent();

			lbTitle.Text = title;
			this.CancelButton = tbCancel;
			_confirmable = true;
		}

		public GenericEditDialog(string title, GenericItem[] items)
			: this(title)
		{
			foreach (GenericItem item in items)
			{
				_items.Add(item);
			}
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
			this.panel = new System.Windows.Forms.Panel();
			this.lbTitle = new System.Windows.Forms.Label();
			this.tbOk = new System.Windows.Forms.Button();
			this.tbCancel = new System.Windows.Forms.Button();
			this.panelItems = new System.Windows.Forms.Panel();
			this.panel.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel
			// 
			this.panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel.Controls.Add(this.panelItems);
			this.panel.Controls.Add(this.lbTitle);
			this.panel.Controls.Add(this.tbOk);
			this.panel.Controls.Add(this.tbCancel);
			this.panel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel.Location = new System.Drawing.Point(0, 0);
			this.panel.Name = "panel";
			this.panel.Size = new System.Drawing.Size(232, 88);
			this.panel.TabIndex = 9;
			// 
			// lbTitle
			// 
			this.lbTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.lbTitle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lbTitle.Location = new System.Drawing.Point(4, 5);
			this.lbTitle.Name = "lbTitle";
			this.lbTitle.Size = new System.Drawing.Size(222, 16);
			this.lbTitle.TabIndex = 12;
			this.lbTitle.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lbTitle_MouseUp);
			this.lbTitle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lbTitle_MouseMove);
			this.lbTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lbTitle_MouseDown);
			// 
			// tbOk
			// 
			this.tbOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.tbOk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.tbOk.Location = new System.Drawing.Point(4, 60);
			this.tbOk.Name = "tbOk";
			this.tbOk.Size = new System.Drawing.Size(68, 20);
			this.tbOk.TabIndex = 10;
			this.tbOk.Text = "אישור";
			this.tbOk.Click += new System.EventHandler(this.tbOk_Click);
			// 
			// tbCancel
			// 
			this.tbCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.tbCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.tbCancel.Location = new System.Drawing.Point(71, 60);
			this.tbCancel.Name = "tbCancel";
			this.tbCancel.Size = new System.Drawing.Size(68, 20);
			this.tbCancel.TabIndex = 11;
			this.tbCancel.Text = "ביטול";
			this.tbCancel.Click += new System.EventHandler(this.tbCancel_Click);
			// 
			// panelItems
			// 
			this.panelItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.panelItems.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panelItems.Location = new System.Drawing.Point(4, 20);
			this.panelItems.Name = "panelItems";
			this.panelItems.Size = new System.Drawing.Size(222, 36);
			this.panelItems.TabIndex = 13;
			// 
			// GenericEditDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 13);
			this.ClientSize = new System.Drawing.Size(232, 88);
			this.Controls.Add(this.panel);
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "GenericEditDialog";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "GenericEditDialog";
			this.panel.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		public void SetAcceptButton()
		{
			this.AcceptButton = tbOk;
			tbOk.Focus();
		}

		private void tbOk_Click(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.OK;
			Close();
		}

		private void tbCancel_Click(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}

		private void itemKeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (!e.Shift && !e.Alt && !e.Control)
			{
				if (e.KeyCode == Keys.Enter)
				{
					tbOk_Click(this, EventArgs.Empty);
				}
				else if (e.KeyCode == Keys.Escape)
				{
					tbCancel_Click(this, EventArgs.Empty);
				}
			}
		}

		private void ItemsChanged(object sender, CollectionEventArgs e)
		{
			if (e.Old != null)
			{
				GenericItem item = (GenericItem) e.Old;
				item.Control.KeyDown -= new KeyEventHandler(itemKeyDown);
			}
			if (e.New != null)
			{
				GenericItem item = (GenericItem) e.New;
				item.Control.TabIndex = e.Index;
				item.Control.KeyDown += new KeyEventHandler(itemKeyDown);
			}
		}

		private object hotSpot;

		private void lbTitle_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			System.Drawing.Point pt = lbTitle.PointToScreen(new System.Drawing.Point(e.X, e.Y));
			hotSpot = new System.Drawing.Point(pt.X - Left, pt.Y - Top);
		}

		private void lbTitle_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (hotSpot != null)
			{
				System.Drawing.Point pt = lbTitle.PointToScreen(new System.Drawing.Point(e.X, e.Y));
				System.Drawing.Point hs = (System.Drawing.Point) hotSpot;

				Location = new System.Drawing.Point(pt.X - hs.X, pt.Y - hs.Y);
			}
		}

		private void lbTitle_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			hotSpot = null;
		}

		protected override void OnLostFocus(EventArgs e)
		{
			hotSpot = null;
			base.OnLostFocus (e);
		}

		public event EventHandler ValueChanged;

		#region IGenericContainer Members

		public BorderStyle GenericItemBorder
		{
			get { return BorderStyle.FixedSingle; }
		}

		public void RemoveControl(GenericItem item, Control control)
		{
			panelItems.Controls.Remove(control);
		}

		public void AddControl(GenericItem item, Control control)
		{
			panelItems.Controls.Add(control);
			if (control is NullComboBox)
			{
				NullComboBox objCombo = (NullComboBox)control;
				string strLongestItem = "";
				for (int i = 0; i < objCombo.Items.Count; i++)
				{
					object objCurItem = objCombo.Items[i];
					if (objCurItem != null)
					{
						string strCurItem = objCurItem.ToString().Trim();
						if (strCurItem.Length > strLongestItem.Length)
							strLongestItem = strCurItem;
					}
				}

				if (strLongestItem.Length > 0)
				{
					Label label = new Label();
					label.AutoSize = true;
					label.Font = control.Font;
					label.Text = strLongestItem;
					this.Controls.Add(label);
					int width = label.Width;
					this.Controls.Remove(label);
					if (width  > objCombo.Width)
					{
						int offset = (width - objCombo.Width);
						this.Width = this.Width + offset;
					}
				}
			}
		}

		public void OnValueChange(GenericItem item, object ov, object nv)
		{
			if (ValueChanged != null)
				ValueChanged(this, EventArgs.Empty);
		}

		public void ResetItems()
		{
			int labelWidth = 0;
			//int height = lbTitle.Bottom + (Bottom - tbOk.Top) + 4;
			int height = 0;

			foreach (GenericItem item in _items)
			{
				if (item.Label != null && item.Label.Width > labelWidth)
					labelWidth = item.Label.Width;
				height += item.Control.Height;
			}

			Height = height + panelItems.Top + (Height - panelItems.Bottom);

			labelWidth += 4;

			int controlWidth = lbTitle.Width - labelWidth;
			int top = -1;

			if (RightToLeft == RightToLeft.Yes)
			{
				foreach (GenericItem item in _items)
				{
					item.Control.Location = new System.Drawing.Point(-1, top);
					if (item.Label == null)
					{
						item.Control.Width = panelItems.Width;
					}
					else
					{
						item.Control.Width = controlWidth;
						item.Label.Location = new System.Drawing.Point(
							panelItems.Width - item.Label.Width, 
							top + item.Control.Height - item.Label.Height - 1);
					}
					top += item.Control.Height - 1;
				}
			}
			else
			{
				foreach (GenericItem item in _items)
				{
					if (item.Label == null)
					{
						item.Control.Width = panelItems.Width;
					}
					else
					{
						item.Control.Width = controlWidth;
						item.Label.Location = new System.Drawing.Point(0,
							top + item.Control.Height - item.Label.Height - 1);
					}
					item.Control.Location = new System.Drawing.Point(panelItems.Width - item.Control.Width, top);
					top += item.Control.Height;
				}
			}
		}

		#endregion
	}
}
