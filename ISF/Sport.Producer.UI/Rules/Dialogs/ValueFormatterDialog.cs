using System;

namespace Sport.Producer.UI.Rules.Dialogs
{
	/// <summary>
	/// Summary description for ValueFormatterDialog.
	/// </summary>
	public class ValueFormatterDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Panel panel;
		private System.Windows.Forms.Button tbOk;
		private System.Windows.Forms.Button tbCancel;
		private System.Windows.Forms.TextBox edFormat;
		private System.Windows.Forms.Label lbTitle;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ValueFormatterDialog(string format)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			edFormat.Text = format;
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
			this.edFormat = new System.Windows.Forms.TextBox();
			this.tbCancel = new System.Windows.Forms.Button();
			this.tbOk = new System.Windows.Forms.Button();
			this.panel.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel
			// 
			this.panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel.Controls.Add(this.lbTitle);
			this.panel.Controls.Add(this.edFormat);
			this.panel.Controls.Add(this.tbCancel);
			this.panel.Controls.Add(this.tbOk);
			this.panel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel.Location = new System.Drawing.Point(0, 0);
			this.panel.Name = "panel";
			this.panel.Size = new System.Drawing.Size(232, 80);
			this.panel.TabIndex = 0;
			// 
			// lbTitle
			// 
			this.lbTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.lbTitle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lbTitle.Location = new System.Drawing.Point(3, 8);
			this.lbTitle.Name = "lbTitle";
			this.lbTitle.Size = new System.Drawing.Size(224, 16);
			this.lbTitle.TabIndex = 16;
			this.lbTitle.Text = "תבנית תוצאה";
			this.lbTitle.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lbTitle_MouseUp);
			this.lbTitle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lbTitle_MouseMove);
			this.lbTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lbTitle_MouseDown);
			// 
			// edFormat
			// 
			this.edFormat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.edFormat.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.edFormat.Location = new System.Drawing.Point(3, 26);
			this.edFormat.Name = "edFormat";
			this.edFormat.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.edFormat.Size = new System.Drawing.Size(224, 21);
			this.edFormat.TabIndex = 15;
			this.edFormat.Text = "";
			// 
			// tbCancel
			// 
			this.tbCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.tbCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.tbCancel.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.tbCancel.Location = new System.Drawing.Point(76, 54);
			this.tbCancel.Name = "tbCancel";
			this.tbCancel.Size = new System.Drawing.Size(68, 20);
			this.tbCancel.TabIndex = 12;
			this.tbCancel.Text = "ביטול";
			this.tbCancel.Click += new System.EventHandler(this.tbCancel_Click);
			// 
			// tbOk
			// 
			this.tbOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.tbOk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.tbOk.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.tbOk.Location = new System.Drawing.Point(3, 54);
			this.tbOk.Name = "tbOk";
			this.tbOk.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.tbOk.Size = new System.Drawing.Size(68, 20);
			this.tbOk.TabIndex = 13;
			this.tbOk.Text = "אישור";
			this.tbOk.Click += new System.EventHandler(this.tbOk_Click);
			// 
			// ValueFormatterDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.ClientSize = new System.Drawing.Size(232, 80);
			this.Controls.Add(this.panel);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "ValueFormatterDialog";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "תבנית תוצאה";
			this.panel.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

        public static bool EditFormat(ref string format)
		{
			ValueFormatterDialog vfd = new ValueFormatterDialog(format);
			if (vfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				format = vfd.edFormat.Text;
				return true;
			}
			
			return false;
		}		

		public static object ValueSelector(Sport.UI.Controls.ButtonBox buttonBox, object value)
		{
			Sport.Common.ValueFormatter vf = value as Sport.Common.ValueFormatter;
			string format = vf != null ? vf.Format : "";
			if (EditFormat(ref format))
			{
				return new Sport.Common.ValueFormatter(vf.Formatters, format);
			}

			return value;
		}

		private void tbCancel_Click(object sender, System.EventArgs e)
		{
			DialogResult = System.Windows.Forms.DialogResult.Cancel;
			Close();
		}

		private void tbOk_Click(object sender, System.EventArgs e)
		{
			DialogResult = System.Windows.Forms.DialogResult.OK;
			Close();
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
	}
}
