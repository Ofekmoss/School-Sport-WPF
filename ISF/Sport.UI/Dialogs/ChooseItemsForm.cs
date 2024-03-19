using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Sport.UI.Dialogs
{
	/// <summary>
	/// Summary description for ChooseItemsForm.
	/// </summary>
	public class ChooseItemsForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.ListBox listSource;
		private System.Windows.Forms.ListBox listTarget;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Label labelSource;
		private System.Windows.Forms.Label labelTarget;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.ImageList imageList;
		private System.Windows.Forms.Button btnAdd;
		private System.Windows.Forms.Button btnRemove;
		private System.Windows.Forms.Button btnDown;
		private System.Windows.Forms.Button btnUp;
		private System.Windows.Forms.Button btnAddAll;
		private System.Windows.Forms.Button btnRemoveAll;
		private System.ComponentModel.IContainer components;

		public object[] Source
		{
			get 
			{ 
				object[] items = new object[listSource.Items.Count];
				listSource.Items.CopyTo(items, 0);
				return items;
			}
			set
			{
				for (int n = 0; n < value.Length; n++)
				{
					listSource.Items.Add(value[n]);
				}
			}
		}

		public object[] Target
		{
			get 
			{ 
				object[] items = new object[listTarget.Items.Count];
				listTarget.Items.CopyTo(items, 0);
				return items;
			}
			set
			{
				for (int n = 0; n < value.Length; n++)
				{
					if (value[n] != null)
						listTarget.Items.Add(value[n]);
				}
			}
		}

		public ChooseItemsForm(string title, string sourceTitle,
			string targetTitle, object[] source, object[] target)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			Text = title;
			labelSource.Text = sourceTitle;
			labelTarget.Text = targetTitle;

			Source = source;
			Target = target;

			SetButtonsState();
		}

		private void SetButtonsState()
		{
			btnAddAll.Enabled = listSource.Items.Count > 0;
			btnRemoveAll.Enabled = listTarget.Items.Count > 0;
			btnAdd.Enabled = listSource.SelectedItems.Count > 0;

			int index = listTarget.SelectedIndex;
			btnRemove.Enabled = listTarget.SelectedItems.Count > 0;
			btnUp.Enabled = index > 0;
			btnDown.Enabled = index < listTarget.Items.Count - 1;
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
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ChooseItemsForm));
			this.listSource = new System.Windows.Forms.ListBox();
			this.listTarget = new System.Windows.Forms.ListBox();
			this.btnOK = new System.Windows.Forms.Button();
			this.labelSource = new System.Windows.Forms.Label();
			this.labelTarget = new System.Windows.Forms.Label();
			this.btnCancel = new System.Windows.Forms.Button();
			this.imageList = new System.Windows.Forms.ImageList(this.components);
			this.btnAdd = new System.Windows.Forms.Button();
			this.btnRemove = new System.Windows.Forms.Button();
			this.btnDown = new System.Windows.Forms.Button();
			this.btnUp = new System.Windows.Forms.Button();
			this.btnAddAll = new System.Windows.Forms.Button();
			this.btnRemoveAll = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// listSource
			// 
			this.listSource.Location = new System.Drawing.Point(240, 32);
			this.listSource.Name = "listSource";
			this.listSource.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.listSource.Size = new System.Drawing.Size(168, 264);
			this.listSource.Sorted = true;
			this.listSource.TabIndex = 0;
			this.listSource.DoubleClick += new System.EventHandler(this.listSource_DoubleClick);
			this.listSource.SelectedIndexChanged += new System.EventHandler(this.listSource_SelectedIndexChanged);
			// 
			// listTarget
			// 
			this.listTarget.Location = new System.Drawing.Point(8, 32);
			this.listTarget.Name = "listTarget";
			this.listTarget.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.listTarget.Size = new System.Drawing.Size(168, 264);
			this.listTarget.TabIndex = 1;
			this.listTarget.DoubleClick += new System.EventHandler(this.listTarget_DoubleClick);
			this.listTarget.SelectedIndexChanged += new System.EventHandler(this.listTarget_SelectedIndexChanged);
			// 
			// btnOK
			// 
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(8, 304);
			this.btnOK.Name = "btnOK";
			this.btnOK.TabIndex = 2;
			this.btnOK.Text = "אישור";
			// 
			// labelSource
			// 
			this.labelSource.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelSource.Location = new System.Drawing.Point(232, 16);
			this.labelSource.Name = "labelSource";
			this.labelSource.Size = new System.Drawing.Size(176, 16);
			this.labelSource.TabIndex = 3;
			this.labelSource.Text = "label1";
			// 
			// labelTarget
			// 
			this.labelTarget.Location = new System.Drawing.Point(8, 16);
			this.labelTarget.Name = "labelTarget";
			this.labelTarget.Size = new System.Drawing.Size(168, 16);
			this.labelTarget.TabIndex = 4;
			this.labelTarget.Text = "label2";
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(96, 304);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.TabIndex = 5;
			this.btnCancel.Text = "ביטול";
			// 
			// imageList
			// 
			this.imageList.ImageSize = new System.Drawing.Size(12, 12);
			this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
			this.imageList.TransparentColor = System.Drawing.Color.White;
			// 
			// btnAdd
			// 
			this.btnAdd.ImageIndex = 0;
			this.btnAdd.ImageList = this.imageList;
			this.btnAdd.Location = new System.Drawing.Point(192, 64);
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.Size = new System.Drawing.Size(32, 24);
			this.btnAdd.TabIndex = 6;
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			// 
			// btnRemove
			// 
			this.btnRemove.ImageIndex = 1;
			this.btnRemove.ImageList = this.imageList;
			this.btnRemove.Location = new System.Drawing.Point(192, 152);
			this.btnRemove.Name = "btnRemove";
			this.btnRemove.Size = new System.Drawing.Size(32, 24);
			this.btnRemove.TabIndex = 7;
			this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
			// 
			// btnDown
			// 
			this.btnDown.ImageIndex = 3;
			this.btnDown.ImageList = this.imageList;
			this.btnDown.Location = new System.Drawing.Point(184, 272);
			this.btnDown.Name = "btnDown";
			this.btnDown.Size = new System.Drawing.Size(24, 24);
			this.btnDown.TabIndex = 9;
			this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
			// 
			// btnUp
			// 
			this.btnUp.ImageIndex = 2;
			this.btnUp.ImageList = this.imageList;
			this.btnUp.Location = new System.Drawing.Point(184, 240);
			this.btnUp.Name = "btnUp";
			this.btnUp.Size = new System.Drawing.Size(24, 24);
			this.btnUp.TabIndex = 8;
			this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
			// 
			// btnAddAll
			// 
			this.btnAddAll.ImageIndex = 4;
			this.btnAddAll.ImageList = this.imageList;
			this.btnAddAll.Location = new System.Drawing.Point(192, 96);
			this.btnAddAll.Name = "btnAddAll";
			this.btnAddAll.Size = new System.Drawing.Size(32, 24);
			this.btnAddAll.TabIndex = 10;
			this.btnAddAll.Click += new System.EventHandler(this.btnAddAll_Click);
			// 
			// btnRemoveAll
			// 
			this.btnRemoveAll.ImageIndex = 5;
			this.btnRemoveAll.ImageList = this.imageList;
			this.btnRemoveAll.Location = new System.Drawing.Point(192, 184);
			this.btnRemoveAll.Name = "btnRemoveAll";
			this.btnRemoveAll.Size = new System.Drawing.Size(32, 24);
			this.btnRemoveAll.TabIndex = 11;
			this.btnRemoveAll.Click += new System.EventHandler(this.btnRemoveAll_Click);
			// 
			// ChooseItemsForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.ClientSize = new System.Drawing.Size(418, 336);
			this.Controls.Add(this.btnRemoveAll);
			this.Controls.Add(this.btnAddAll);
			this.Controls.Add(this.btnDown);
			this.Controls.Add(this.btnUp);
			this.Controls.Add(this.btnRemove);
			this.Controls.Add(this.btnAdd);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.labelTarget);
			this.Controls.Add(this.labelSource);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.listTarget);
			this.Controls.Add(this.listSource);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "ChooseItemsForm";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "בחר...";
			this.ResumeLayout(false);

		}
		#endregion

		private void btnAdd_Click(object sender, System.EventArgs e)
		{
			object[] items = new object[listSource.SelectedItems.Count];
			listSource.SelectedItems.CopyTo(items, 0);
			for (int n = 0; n < items.Length; n++)
			{
				listTarget.Items.Add(items[n]);
				listSource.Items.Remove(items[n]);
			}
			SetButtonsState();
		}

		private void btnAddAll_Click(object sender, System.EventArgs e)
		{
			for (int n = 0; n < listSource.Items.Count; n++)
			{
				listTarget.Items.Add(listSource.Items[n]);
			}
			listSource.Items.Clear();
			SetButtonsState();
		}

		private void btnRemove_Click(object sender, System.EventArgs e)
		{
			object[] items = new object[listTarget.SelectedItems.Count];
			listTarget.SelectedItems.CopyTo(items, 0);
			for (int n = 0; n < items.Length; n++)
			{
				listSource.Items.Add(items[n]);
				listTarget.Items.Remove(items[n]);
			}

			SetButtonsState();
		}

		private void btnRemoveAll_Click(object sender, System.EventArgs e)
		{
			for (int n = 0; n < listTarget.Items.Count; n++)
			{
				listSource.Items.Add(listTarget.Items[n]);
			}
			listTarget.Items.Clear();
			SetButtonsState();
		}

		private void btnUp_Click(object sender, System.EventArgs e)
		{
			int index = listTarget.SelectedIndex;
			if (index > 0)
			{
				object item = listTarget.Items[index];
				listTarget.Items.RemoveAt(index);
				listTarget.Items.Insert(index - 1, item);
				listTarget.SelectedIndex = index - 1;
			}
			SetButtonsState();
		}

		private void btnDown_Click(object sender, System.EventArgs e)
		{
			int index = listTarget.SelectedIndex;
			if ((index >= 0)&&(index < listTarget.Items.Count - 1))
			{
				object item = listTarget.Items[index];
				listTarget.Items.RemoveAt(index);
				listTarget.Items.Insert(index + 1, item);
				listTarget.SelectedIndex = index + 1;
			}
			SetButtonsState();
		}

		private void listTarget_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			SetButtonsState();
		}

		private void listSource_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			SetButtonsState();
		}

		private void listSource_DoubleClick(object sender, System.EventArgs e)
		{
			int index = listSource.SelectedIndex;
			if (index != -1)
			{
				listTarget.Items.Add(listSource.Items[index]);
				listSource.Items.RemoveAt(index);
			}
			SetButtonsState();
		}

		private void listTarget_DoubleClick(object sender, System.EventArgs e)
		{
			int index = listTarget.SelectedIndex;
			if (index != -1)
			{
				listSource.Items.Add(listTarget.Items[index]);
				listTarget.Items.RemoveAt(index);
			}
			SetButtonsState();
		}
	}
}
