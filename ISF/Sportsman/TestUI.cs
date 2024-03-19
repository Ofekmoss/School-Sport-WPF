using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Sportsman
{
	/// <summary>
	/// Summary description for TestUI.
	/// </summary>
	public class TestUI : Sport.UI.View
	{
		public class MyGridSource : Sport.UI.Controls.IGridSource
		{
			Sport.UI.Controls.Style style = new Sport.UI.Controls.Style();
			private Sport.UI.Controls.Grid _grid;
			#region IGridSource Members

			public void SetGrid(Sport.UI.Controls.Grid grid)
			{

				style.Font = new Font("Tahoma", 20);
				_grid = grid;

				_grid.RefreshSource();
			}

			public void Sort(int group, int[] columns)
			{
				// TODO:  Add MyGridSource.Sort implementation
			}

			public Sport.UI.Controls.Style GetStyle(int row, int field, Sport.UI.Controls.GridDrawState state)
			{
				return null;
			}

			public string GetTip(int row)
			{
				// TODO:  Add MyGridSource.GetTip implementation
				return null;
			}

			public int GetGroup(int row)
			{
				// TODO:  Add MyGridSource.GetGroup implementation
				return 0;
			}

			public Control Edit(int row, int field)
			{
				TextBox tb = new TextBox();

				tb.Text = GetText(row, field);

				tb.TextChanged += new EventHandler(tb_TextChanged);

				return tb;
			}

			public void EditEnded(Control control)
			{
				// TODO:  Add MyGridSource.EditEnded implementation
			}

			public int GetRowCount()
			{
				return 3;
			}

			public int GetFieldCount(int row)
			{
				return 3;
			}

			public string GetText(int row, int field)
			{
				switch (field)
				{
					case (0):
						return row.ToString();
					case (2):
						return (row * 3).ToString();
				}

				return null;
			}

			public int[] GetSort(int group)
			{
				// TODO:  Add MyGridSource.GetSort implementation
				return null;
			}

			#endregion

			#region IDisposable Members

			public void Dispose()
			{
				// TODO:  Add MyGridSource.Dispose implementation
			}

			#endregion

			private void tb_TextChanged(object sender, EventArgs e)
			{

			}
		}

		private Sport.UI.Controls.ButtonBox button1;
		private Sport.UI.Controls.Grid grid;
		private Sport.UI.Controls.TextControl textBox1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public TestUI()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//textBox1.Controller = new Sport.UI.Controls.RegularExpressionController(
			//	"[a-zA-Z]{0,3}[0-9]*");

			textBox1.Controller = new Sport.UI.Controls.NumberController(-10, 10, 2, 5);
			textBox1.ShowSpin = true;
			textBox1.Value = 5;

			button1.ValueChanged += new EventHandler(button1_ValueChanged);
			button1.ValueSelector = new Sport.UI.Controls.ButtonBox.SelectValue(mySelectValue);

			grid.Source = new MyGridSource();

			grid.Columns.Add(2, "שם", 100);
			grid.Columns.Add(0, "ערך", 100);

			Sport.Common.IniFile iniFile=new Sport.Common.IniFile("test.ini");
			iniFile.WriteValue("general", "test", "hello world");

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		public object mySelectValue(Sport.UI.Controls.ButtonBox buttonBox, object value)
		{
			Sport.UI.Dialogs.GenericEditDialog ged = new Sport.UI.Dialogs.GenericEditDialog("בחר");

			ged.Items.Add(Sport.UI.Controls.GenericItemType.Text, value);

			if (ged.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				return ged.Items[0].Value;
			}

			return value;
		}

		private void button1_ValueChanged(object sender, EventArgs e)
		{

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
			this.button1 = new Sport.UI.Controls.ButtonBox();
			this.grid = new Sport.UI.Controls.Grid();
			this.textBox1 = new Sport.UI.Controls.TextControl();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.BackColor = System.Drawing.SystemColors.ActiveBorder;
			this.button1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.button1.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.button1.Location = new System.Drawing.Point(64, 48);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(104, 40);
			this.button1.TabIndex = 0;
			this.button1.Text = "button1";
			this.button1.Value = null;
			this.button1.ValueSelector = null;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// grid
			// 
			this.grid.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.grid.Editable = true;
			this.grid.ExpandOnDoubleClick = true;
			this.grid.GroupingStyle = Sport.UI.Controls.GridGroupingStyle.Grid;
			this.grid.HeaderHeight = 17;
			this.grid.HorizontalLines = true;
			this.grid.Location = new System.Drawing.Point(112, 104);
			this.grid.Name = "grid";
			this.grid.SelectedIndex = -1;
			this.grid.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.grid.ShowCheckBoxes = false;
			this.grid.ShowRowNumber = false;
			this.grid.Size = new System.Drawing.Size(440, 256);
			this.grid.TabIndex = 0;
			this.grid.VerticalLines = true;
			this.grid.VisibleRow = 0;
			// 
			// textBox1
			// 
			this.textBox1.BackColor = System.Drawing.SystemColors.InactiveBorder;
			this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.textBox1.Controller = null;
			this.textBox1.Location = new System.Drawing.Point(360, 48);
			this.textBox1.Name = "textBox1";
			this.textBox1.ReadOnly = false;
			this.textBox1.SelectionLength = 0;
			this.textBox1.SelectionStart = 0;
			this.textBox1.ShowSpin = false;
			this.textBox1.Size = new System.Drawing.Size(120, 20);
			this.textBox1.TabIndex = 1;
			this.textBox1.Text = "textBox1";
			this.textBox1.Value = "textBox1";
			// 
			// TestUI
			// 
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.grid);
			this.Controls.Add(this.button1);
			this.Name = "TestUI";
			this.Size = new System.Drawing.Size(786, 452);
			this.Text = "TestUI";
			this.LostFocus += new System.EventHandler(this.TestUI_LostFocus);
			this.ResumeLayout(false);

		}
		#endregion

		private void TestUI_Load(object sender, System.EventArgs e)
		{
		
		}

		private void button1_Click(object sender, System.EventArgs e)
		{
			Sport.Common.IniFile iniFile=new Sport.Common.IniFile("test.ini");
			Sport.UI.MessageBox.Show(iniFile.ReadValue("General", "test"));
		}

		private void TestUI_LostFocus(object sender, System.EventArgs e)
		{
		
		}

	}
}
