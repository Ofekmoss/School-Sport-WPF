using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Sport.Data;
using Sport.Types;

namespace Sportsman.Forms
{
	/// <summary>
	/// Summary description for CategorySelectionDialog.
	/// </summary>
	public class CategorySelectionDialog : System.Windows.Forms.Form
	{
		private bool lastSet;

		private int _category;
		public int Category { get { return _category; } }


		private System.Windows.Forms.Button tbOk;
		private System.Windows.Forms.Button tbCancel;
		private System.Windows.Forms.Label labelBoys;
		private System.Windows.Forms.Label labelGirls;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public CategorySelectionDialog(int category)
		{
			_category = category;
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			MouseMove += new MouseEventHandler(CategorySelectionDialog_MouseMove);
			MouseUp += new MouseEventHandler(CategorySelectionDialog_MouseUp);

			int l = 95;
			int t = 0;
			for (int s = 1 /* boys */; s <= 2 /* girls */; s++)
			{
				for (int g = 0; g < 12; g++)
				{
					Label label = new Label();
					label.TabIndex = g + (s * 12);
					label.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
					label.Size = new System.Drawing.Size(20, 20);
					label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
					label.Location = new System.Drawing.Point(l, t);
					label.Text = CategoryTypeLookup.Grades[g];
					label.Tag = CategoryTypeLookup.ToCategory((Sex) s, g);
					if ((_category & (int) label.Tag) == 0)
					{
						label.BackColor = System.Drawing.SystemColors.Window;
						label.ForeColor = System.Drawing.SystemColors.WindowText;
					}
					else
					{
						label.BackColor = System.Drawing.SystemColors.Highlight;
						label.ForeColor = System.Drawing.SystemColors.HighlightText;
					}

					label.MouseDown += new MouseEventHandler(GradeMouseDown);
					Controls.Add(label);
					l -= 19;
					if (g == 5 || g == 11)
					{
						t += 19;
						l = 95;
					}
				}
			}
		}

		private void GradeMouseDown(object sender, MouseEventArgs e)
		{
			if (sender is Label)
			{
				Label label = (Label) sender;

				if ((_category & (int) label.Tag) == 0)
				{
					_category |= (int) label.Tag;
					label.BackColor = SystemColors.Highlight;
					label.ForeColor = SystemColors.HighlightText;
					lastSet = true;
				}
				else
				{
					_category &= ~(int) label.Tag;
					label.BackColor = SystemColors.Window;
					label.ForeColor = SystemColors.WindowText;
					lastSet = false;
				}

				Capture = true;
			}
		}

		private void ResetLabels()
		{
			foreach (Control control in Controls)
			{
				if (control.Tag is int)
				{
					if (((int) control.Tag & _category) == 0)
					{
						control.BackColor = SystemColors.Window;
						control.ForeColor = SystemColors.WindowText;
					}
					else
					{
						control.BackColor = SystemColors.Highlight;
						control.ForeColor = SystemColors.HighlightText;
					}
				}
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
			this.tbOk = new System.Windows.Forms.Button();
			this.tbCancel = new System.Windows.Forms.Button();
			this.labelBoys = new System.Windows.Forms.Label();
			this.labelGirls = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// tbOk
			// 
			this.tbOk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.tbOk.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.tbOk.Location = new System.Drawing.Point(0, 76);
			this.tbOk.Name = "tbOk";
			this.tbOk.Size = new System.Drawing.Size(84, 20);
			this.tbOk.TabIndex = 6;
			this.tbOk.Text = "אישור";
			this.tbOk.Click += new System.EventHandler(this.tbOk_Click);
			// 
			// tbCancel
			// 
			this.tbCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.tbCancel.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.tbCancel.Location = new System.Drawing.Point(83, 76);
			this.tbCancel.Name = "tbCancel";
			this.tbCancel.Size = new System.Drawing.Size(84, 20);
			this.tbCancel.TabIndex = 5;
			this.tbCancel.Text = "ביטול";
			this.tbCancel.Click += new System.EventHandler(this.tbCancel_Click);
			// 
			// labelBoys
			// 
			this.labelBoys.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.labelBoys.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.labelBoys.Location = new System.Drawing.Point(114, 0);
			this.labelBoys.Name = "labelBoys";
			this.labelBoys.Size = new System.Drawing.Size(53, 39);
			this.labelBoys.TabIndex = 22;
			this.labelBoys.Text = "תלמידים";
			this.labelBoys.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.labelBoys.Click += new System.EventHandler(this.labelBoys_Click);
			// 
			// labelGirls
			// 
			this.labelGirls.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.labelGirls.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.labelGirls.Location = new System.Drawing.Point(114, 38);
			this.labelGirls.Name = "labelGirls";
			this.labelGirls.Size = new System.Drawing.Size(53, 39);
			this.labelGirls.TabIndex = 23;
			this.labelGirls.Text = "תלמידות";
			this.labelGirls.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.labelGirls.Click += new System.EventHandler(this.labelGirls_Click);
			// 
			// CategorySelectionDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(167, 96);
			this.Controls.Add(this.labelGirls);
			this.Controls.Add(this.labelBoys);
			this.Controls.Add(this.tbOk);
			this.Controls.Add(this.tbCancel);
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "CategorySelectionDialog";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "CategorySelectionDialog";
			this.ResumeLayout(false);

		}
		#endregion


		private void tbCancel_Click(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}

		private void tbOk_Click(object sender, System.EventArgs e)
		{
			if ((_category & CategoryTypeLookup.All) == CategoryTypeLookup.All)
				_category = -1;
			DialogResult = DialogResult.OK;
			Close();
		}

		public static object ValueSelector(Sport.UI.Controls.ButtonBox buttonBox, object value)
		{
			int category;
			if (value == null)
				category = 0;
			else if (value is int)
				category = (int) value;
			else if (value is LookupItem)
				category = ((LookupItem) value).Id;
			else
				return value;

			if (EditCategory(ref category))
			{
				if (value is int)
					return category;
				if (category == 0)
					return null;
				return CategoryTypeLookup.ToLookupItem(category);
			}

			return value;
		}

		private void labelBoys_Click(object sender, System.EventArgs e)
		{
			if ((_category & 0xFFF) == 0xFFF)
			{
				_category &= 0xFFF0000;
			}
			else
			{
				_category |= 0xFFF;
			}

			ResetLabels();
		}

		private void labelGirls_Click(object sender, System.EventArgs e)
		{
			if ((_category & 0xFFF0000) == 0xFFF0000)
			{
				_category &= 0xFFF;
			}
			else
			{
				_category |= 0xFFF0000;
			}

			ResetLabels();
		}

		private void CategorySelectionDialog_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				Label label = GetChildAtPoint(new System.Drawing.Point(e.X, e.Y)) as
					Label;
				if (label != null && label.Tag != null)
				{
					bool s = (_category & (int) label.Tag) != 0;

					if (s != lastSet)
					{
						if (lastSet)
						{
							_category |= (int) label.Tag;
							label.BackColor = SystemColors.Highlight;
							label.ForeColor = SystemColors.HighlightText;
						}
						else
						{
							_category &= ~(int) label.Tag;
							label.BackColor = SystemColors.Window;
							label.ForeColor = SystemColors.WindowText;
						}
					}
				}
			}
		}

		private void CategorySelectionDialog_MouseUp(object sender, MouseEventArgs e)
		{
			Capture = false;
		}

		public static bool EditCategory(ref int category)
		{
			CategorySelectionDialog csd = new CategorySelectionDialog(category);

			csd.Location = new Point(
				MousePosition.X - csd.Width / 2, 
				MousePosition.Y - csd.Height / 2);

			if (csd.ShowDialog() == DialogResult.OK)
			{
				category = csd.Category;

				return true;
			}

			return false;
		}
	}
}
