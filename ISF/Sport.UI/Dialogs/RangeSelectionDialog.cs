using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Sport.Common;

namespace Sport.UI.Dialogs
{
	/// <summary>
	/// Summary description for RangeSelectionDialog.
	/// </summary>
	public class RangeSelectionDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button btnAdd;
		private System.Windows.Forms.Button tbCancel;
		private System.Windows.Forms.Button tbOk;
		private System.Windows.Forms.Button btnRemove;
		private System.Windows.Forms.ListBox lbRanges;
		private System.Windows.Forms.TextBox tbRange;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public RangeSelectionDialog(RangeArray ranges)
		{
			if (ranges == null)
				_ranges = new RangeArray();
			else
				_ranges = (RangeArray) ranges.Clone();
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			SetRanges();
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
			this.lbRanges = new System.Windows.Forms.ListBox();
			this.tbRange = new System.Windows.Forms.TextBox();
			this.btnAdd = new System.Windows.Forms.Button();
			this.tbCancel = new System.Windows.Forms.Button();
			this.tbOk = new System.Windows.Forms.Button();
			this.btnRemove = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// lbRanges
			// 
			this.lbRanges.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lbRanges.Location = new System.Drawing.Point(0, 19);
			this.lbRanges.Name = "lbRanges";
			this.lbRanges.Size = new System.Drawing.Size(136, 171);
			this.lbRanges.TabIndex = 3;
			this.lbRanges.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lbRanges_MouseDown);
			this.lbRanges.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lbRanges_MouseUp);
			this.lbRanges.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lbRanges_MouseMove);
			this.lbRanges.SelectedIndexChanged += new System.EventHandler(this.lbRanges_SelectedIndexChanged);
			// 
			// tbRange
			// 
			this.tbRange.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tbRange.Location = new System.Drawing.Point(38, 0);
			this.tbRange.Name = "tbRange";
			this.tbRange.Size = new System.Drawing.Size(98, 20);
			this.tbRange.TabIndex = 0;
			this.tbRange.Text = "";
			this.tbRange.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbRange_KeyDown);
			// 
			// btnAdd
			// 
			this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnAdd.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnAdd.Location = new System.Drawing.Point(0, 0);
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.Size = new System.Drawing.Size(20, 20);
			this.btnAdd.TabIndex = 2;
			this.btnAdd.Text = "+";
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			// 
			// tbCancel
			// 
			this.tbCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.tbCancel.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.tbCancel.Location = new System.Drawing.Point(68, 189);
			this.tbCancel.Name = "tbCancel";
			this.tbCancel.Size = new System.Drawing.Size(68, 20);
			this.tbCancel.TabIndex = 4;
			this.tbCancel.Text = "ביטול";
			this.tbCancel.Click += new System.EventHandler(this.tbCancel_Click);
			// 
			// tbOk
			// 
			this.tbOk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.tbOk.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.tbOk.Location = new System.Drawing.Point(0, 189);
			this.tbOk.Name = "tbOk";
			this.tbOk.Size = new System.Drawing.Size(68, 20);
			this.tbOk.TabIndex = 5;
			this.tbOk.Text = "אישור";
			this.tbOk.Click += new System.EventHandler(this.tbOk_Click);
			// 
			// btnRemove
			// 
			this.btnRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnRemove.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnRemove.Location = new System.Drawing.Point(19, 0);
			this.btnRemove.Name = "btnRemove";
			this.btnRemove.Size = new System.Drawing.Size(20, 20);
			this.btnRemove.TabIndex = 3;
			this.btnRemove.Text = "-";
			this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
			// 
			// RangeSelectionDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(136, 209);
			this.ControlBox = false;
			this.Controls.Add(this.btnRemove);
			this.Controls.Add(this.tbOk);
			this.Controls.Add(this.tbCancel);
			this.Controls.Add(this.btnAdd);
			this.Controls.Add(this.tbRange);
			this.Controls.Add(this.lbRanges);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "RangeSelectionDialog";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "בחר טווח";
			this.ResumeLayout(false);

		}
		#endregion


		private RangeArray _ranges;
		public RangeArray Ranges
		{
			get { return _ranges; }
		}

		private void SetRanges()
		{
			lbRanges.Items.Clear();

			foreach (RangeArray.Range range in _ranges)
				lbRanges.Items.Add(range);
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

		private void btnAdd_Click(object sender, System.EventArgs e)
		{
			try
			{
				RangeArray.Range range = RangeArray.Range.Parse(tbRange.Text);

				if (range != null)
				{
					_ranges.Add(range);
					tbRange.Text = null;
					SetRanges();
				}
			}
			catch
			{
				MessageBox.Show("ערך טווח אינו חוקי, יש להכניס ערך בפורמט 'מ-עד'", 
					MessageBoxIcon.Information);
			}
		}

		private void btnRemove_Click(object sender, System.EventArgs e)
		{
			try
			{
				RangeArray.Range range = RangeArray.Range.Parse(tbRange.Text);

				if (range != null)
				{
					_ranges.Remove(range);
					tbRange.Text = null;
					SetRanges();
				}
			}
			catch
			{
				MessageBox.Show("ערך טווח אינו חוקי, יש להכניס ערך בפורמט 'מ-עד'", MessageBoxIcon.Information);
			}
		}

		private void lbRanges_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			tbRange.Text = lbRanges.SelectedItem.ToString();
		}



		bool buttonDown = false;
		Point buttonPoint;

		protected override void OnLostFocus(EventArgs e)
		{
			buttonDown = false;
			base.OnLostFocus (e);
		}

		private void lbRanges_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (buttonDown)
			{
				Point pt = PointToScreen(new Point(e.X, e.Y));
				Location = new Point(pt.X - buttonPoint.X, pt.Y - buttonPoint.Y);
			}
		
		}

		private void lbRanges_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				buttonDown = true;
				buttonPoint = new Point(e.X, e.Y);
			}
		}

		private void lbRanges_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			buttonDown = false;
		}

		private void tbRange_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (!e.Shift && ! e.Alt)
			{
				if (e.Control)
				{
					if (e.KeyCode == Keys.Enter)
					{
						btnAdd_Click(this, EventArgs.Empty);
					}
					else if (e.KeyCode == Keys.Delete)
					{
						btnRemove_Click(this, EventArgs.Empty);
					}
				}
				else
				{
					if (e.KeyCode == Keys.Down)
					{
						int index = lbRanges.SelectedIndex;
						if (index < 0)
							index = -1;
						if (index < lbRanges.Items.Count - 1)
							lbRanges.SelectedIndex = index + 1;
					}
					else if (e.KeyCode == Keys.Up)
					{
						int index = lbRanges.SelectedIndex;
						if (index < 0)
							index = 1;
						if (index > 0 && lbRanges.Items.Count > 0)
							lbRanges.SelectedIndex = index - 1;
					}
					else if (e.KeyCode == Keys.Enter)
					{
						tbOk_Click(this, EventArgs.Empty);
					}
					else if (e.KeyCode == Keys.Escape)
					{
						tbCancel_Click(this, EventArgs.Empty);
					}
				}
			}
		}

	
		public static bool EditRangeArray(ref RangeArray ra)
		{
			RangeSelectionDialog rsd = new RangeSelectionDialog(ra);

            rsd.Location = new Point(
				MousePosition.X - rsd.Width / 2, 
				MousePosition.Y - rsd.Height / 2);

			if (rsd.ShowDialog() == DialogResult.OK)
			{
				if (rsd.Ranges.RangeCount == 0)
					ra = null;
				else
					ra = (RangeArray) rsd.Ranges.Clone();

				return true;
			}

			return false;
		}

		public static object ValueSelector(Sport.UI.Controls.ButtonBox buttonBox, object value)
		{
			RangeArray ra = (RangeArray) value;
			if (EditRangeArray(ref ra))
                return ra;
			return value;
		}
	}
}
