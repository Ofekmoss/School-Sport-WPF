using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Sport.UI;

namespace Sport.UI.Dialogs
{
	/// <summary>
	/// Summary description for CustomizeForm2.
	/// </summary>
	public class CustomizeForm2 : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox gbTable;
		private System.Windows.Forms.Button btnTableFields;
		private System.Windows.Forms.Label labelTableFields;
		private System.Windows.Forms.Label labelSort;
		private System.Windows.Forms.Button btnSort;
		private System.Windows.Forms.GroupBox gbDetails;
		private System.Windows.Forms.Label labelDetailFields;
		private System.Windows.Forms.Button btnDetailFields;
		private System.Windows.Forms.Button btnOK;

		private Sport.UI.TableView2 _view;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public CustomizeForm2(TableView2 view)
		{
			_view = view;
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			if (!_view.DetailsBarEnabled)
			{
				Height -= gbDetails.Height + 8;
				gbDetails.Visible = false;
			}

			SetLabels();
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
			this.gbTable = new System.Windows.Forms.GroupBox();
			this.labelSort = new System.Windows.Forms.Label();
			this.btnSort = new System.Windows.Forms.Button();
			this.labelTableFields = new System.Windows.Forms.Label();
			this.btnTableFields = new System.Windows.Forms.Button();
			this.gbDetails = new System.Windows.Forms.GroupBox();
			this.labelDetailFields = new System.Windows.Forms.Label();
			this.btnDetailFields = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.gbTable.SuspendLayout();
			this.gbDetails.SuspendLayout();
			this.SuspendLayout();
			// 
			// gbTable
			// 
			this.gbTable.Controls.Add(this.labelSort);
			this.gbTable.Controls.Add(this.btnSort);
			this.gbTable.Controls.Add(this.labelTableFields);
			this.gbTable.Controls.Add(this.btnTableFields);
			this.gbTable.Location = new System.Drawing.Point(8, 8);
			this.gbTable.Name = "gbTable";
			this.gbTable.Size = new System.Drawing.Size(384, 136);
			this.gbTable.TabIndex = 0;
			this.gbTable.TabStop = false;
			this.gbTable.Text = "טבלה";
			// 
			// labelSort
			// 
			this.labelSort.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.labelSort.Location = new System.Drawing.Point(8, 80);
			this.labelSort.Name = "labelSort";
			this.labelSort.Size = new System.Drawing.Size(280, 40);
			this.labelSort.TabIndex = 3;
			this.labelSort.Text = "label1";
			// 
			// btnSort
			// 
			this.btnSort.Location = new System.Drawing.Point(296, 88);
			this.btnSort.Name = "btnSort";
			this.btnSort.TabIndex = 2;
			this.btnSort.Text = "מיון...";
			this.btnSort.Click += new System.EventHandler(this.btnSort_Click);
			// 
			// labelTableFields
			// 
			this.labelTableFields.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.labelTableFields.Location = new System.Drawing.Point(8, 24);
			this.labelTableFields.Name = "labelTableFields";
			this.labelTableFields.Size = new System.Drawing.Size(280, 40);
			this.labelTableFields.TabIndex = 1;
			this.labelTableFields.Text = "label1";
			// 
			// btnTableFields
			// 
			this.btnTableFields.Location = new System.Drawing.Point(296, 32);
			this.btnTableFields.Name = "btnTableFields";
			this.btnTableFields.TabIndex = 1;
			this.btnTableFields.Text = "שדות...";
			this.btnTableFields.Click += new System.EventHandler(this.btnTableFields_Click);
			// 
			// gbDetails
			// 
			this.gbDetails.Controls.Add(this.labelDetailFields);
			this.gbDetails.Controls.Add(this.btnDetailFields);
			this.gbDetails.Location = new System.Drawing.Point(8, 152);
			this.gbDetails.Name = "gbDetails";
			this.gbDetails.Size = new System.Drawing.Size(384, 80);
			this.gbDetails.TabIndex = 1;
			this.gbDetails.TabStop = false;
			this.gbDetails.Text = "פרטים";
			// 
			// labelDetailFields
			// 
			this.labelDetailFields.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.labelDetailFields.Location = new System.Drawing.Point(8, 24);
			this.labelDetailFields.Name = "labelDetailFields";
			this.labelDetailFields.Size = new System.Drawing.Size(280, 40);
			this.labelDetailFields.TabIndex = 1;
			this.labelDetailFields.Text = "label1";
			// 
			// btnDetailFields
			// 
			this.btnDetailFields.Location = new System.Drawing.Point(296, 32);
			this.btnDetailFields.Name = "btnDetailFields";
			this.btnDetailFields.TabIndex = 3;
			this.btnDetailFields.Text = "שדות...";
			this.btnDetailFields.Click += new System.EventHandler(this.btnDetailFields_Click);
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(8, 242);
			this.btnOK.Name = "btnOK";
			this.btnOK.TabIndex = 0;
			this.btnOK.Text = "סגור";
			// 
			// CustomizeForm2
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.ClientSize = new System.Drawing.Size(400, 272);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.gbDetails);
			this.Controls.Add(this.gbTable);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "CustomizeForm2";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "התאמה";
			this.gbTable.ResumeLayout(false);
			this.gbDetails.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void SetLabels()
		{
			EntityView entityView = _view.EntityView;
			string[] titles;

			// Columns
			int[] items = _view.Columns;
			if (items != null && items.Length > 0)
			{
				titles = new string[items.Length];
				for (int n = 0; n < items.Length; n++)
				{
					titles[n] = entityView[items[n]].Title;
				}
				labelTableFields.Text = String.Join(", ", titles);
			}
			else
			{
				labelTableFields.Text = "(אין)";
			}

			// Details
			items = _view.Details;
			if (items != null && items.Length > 0)
			{
				titles = new string[items.Length];
				for (int n = 0; n < items.Length; n++)
				{
					titles[n] = entityView[items[n]].Title;
				}
				labelDetailFields.Text = String.Join(", ", titles);
			}
			else
			{
				labelDetailFields.Text = "(אין)";
			}

			// Sort
			items = _view.Sort;
			if (items != null && items.Length > 0)
			{
				titles = new string[items.Length];
				for (int n = 0; n < items.Length; n++)
				{
					if (items[n] < 0)
					{
						titles[n] = entityView[items[n] - Int32.MinValue].Title + " (יורד)";
					}
					else
					{
						titles[n] = entityView[items[n]].Title + " (עולה)";
					}
				}

				labelSort.Text = String.Join(", ", titles);
			}
			else
			{
				labelSort.Text = "(אין)";
			}
				
		}

		private void btnTableFields_Click(object sender, System.EventArgs e)
		{
			EntityView entityView = _view.EntityView;
			
			ArrayList source = new ArrayList();
			source.AddRange(entityView.Fields);
			ArrayList target = new ArrayList();
			int[] items = _view.Columns;
			for (int n = 0; n < items.Length; n++)
			{
				target.Add(entityView[items[n]]);
				source.Remove(entityView[items[n]]);
			}
			ChooseItemsForm cif = new ChooseItemsForm("בחר שדות לטבלה",
				"שדות פנויים:", "שדות בטבלה:",
				source.ToArray(), target.ToArray());

			if (cif.ShowDialog() == DialogResult.OK)
			{
				object[] ti = cif.Target;
				items = new int[ti.Length];
				for (int n = 0; n < ti.Length; n++)
				{
					items[n] = ((EntityFieldView) ti[n]).Field;
				}
				
				_view.Columns = items;

				_view.SaveConfiguration();

				SetLabels();
			}
		}

		private void btnDetailFields_Click(object sender, System.EventArgs e)
		{
			EntityView entityView = _view.EntityView;

			ArrayList source = new ArrayList();
			source.AddRange(entityView.Fields);
			ArrayList target = new ArrayList();
			int[] items = _view.Details;
			for (int n = 0; n < items.Length; n++)
			{
				target.Add(entityView[items[n]]);
				source.Remove(entityView[items[n]]);
			}
			ChooseItemsForm cif = new ChooseItemsForm("בחר שדות לפרטים",
				"שדות פנויים:", "שדות בפרטים:",
				source.ToArray(), target.ToArray());

			if (cif.ShowDialog() == DialogResult.OK)
			{
				object[] ti = cif.Target;
				items = new int[ti.Length];
				for (int n = 0; n < ti.Length; n++)
				{
					items[n] = ((EntityFieldView) ti[n]).Field;
				}
				int index = _view.CurrentIndex;
				_view.Current = null;

				_view.Details = items;
				_view.CurrentIndex = index;

				_view.SaveConfiguration();

				SetLabels();
			}
		}

		private void btnSort_Click(object sender, System.EventArgs e)
		{
			EntityView entityView = _view.EntityView;
			if ((entityView == null)||(entityView.Fields == null))
				return;
			
			System.Collections.ArrayList al = new ArrayList();
			int[] indexes = new int[entityView.Fields.Length];
			for (int n = 0; n < entityView.Fields.Length; n++)
			{
				indexes[n] = al.Add(entityView[n]);
			}

			int[] viewSort = _view.Sort;
			if (viewSort == null)
				return;
			
			int[] sort = new int[viewSort.Length];
			for (int n = 0; n < viewSort.Length; n++)
			{
				if (viewSort[n] < 0)
					sort[n] = indexes[viewSort[n] - Int32.MinValue] + Int32.MinValue;
				else
					sort[n] = indexes[viewSort[n]];
			}

			SortForm sf = new SortForm(
				al.ToArray(), sort);
			if (sf.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				sort = sf.Sort;
				if (sort != null)
				{
					viewSort = new int[sort.Length];
					for (int n = 0; n < sort.Length; n++)
					{
						if (sort[n] < 0)
						{
							EntityFieldView efv = (EntityFieldView) al[sort[n] - Int32.MinValue];
							if (efv != null)
								viewSort[n] = efv.Field + Int32.MinValue;
						}
						else
						{
							EntityFieldView efv = (EntityFieldView) al[sort[n]];
							if (efv != null)
								viewSort[n] = efv.Field;
						}
					}
					
					_view.Sort = viewSort;
					_view.SaveConfiguration();
					SetLabels();
				}
			}
		}
	} //end class CustomizeForm2
}
