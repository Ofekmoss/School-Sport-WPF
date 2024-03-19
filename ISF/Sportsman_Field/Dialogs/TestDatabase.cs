using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using LocalDatabaseManager;

namespace Sportsman_Field
{
	/// <summary>
	/// Summary description for TestDatabase.
	/// </summary>
	public class TestDatabase : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox tbTableName;
		private System.Windows.Forms.ListBox lbFields;
		private System.Windows.Forms.TextBox tbFieldName;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox cbFieldType;
		private System.Windows.Forms.Button btnAddField;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button btnClearFields;
		private System.Windows.Forms.Button btnCreateTable;
		private System.Windows.Forms.Button btnDropTable;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public TestDatabase()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
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
			this.label1 = new System.Windows.Forms.Label();
			this.tbTableName = new System.Windows.Forms.TextBox();
			this.lbFields = new System.Windows.Forms.ListBox();
			this.tbFieldName = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.cbFieldType = new System.Windows.Forms.ComboBox();
			this.btnAddField = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.btnClearFields = new System.Windows.Forms.Button();
			this.btnCreateTable = new System.Windows.Forms.Button();
			this.btnDropTable = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(8, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(66, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "table name: ";
			// 
			// tbTableName
			// 
			this.tbTableName.Location = new System.Drawing.Point(88, 16);
			this.tbTableName.Name = "tbTableName";
			this.tbTableName.TabIndex = 1;
			this.tbTableName.Text = "";
			// 
			// lbFields
			// 
			this.lbFields.Location = new System.Drawing.Point(224, 8);
			this.lbFields.Name = "lbFields";
			this.lbFields.Size = new System.Drawing.Size(120, 134);
			this.lbFields.TabIndex = 2;
			// 
			// tbFieldName
			// 
			this.tbFieldName.Location = new System.Drawing.Point(88, 16);
			this.tbFieldName.Name = "tbFieldName";
			this.tbFieldName.TabIndex = 4;
			this.tbFieldName.Text = "";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(8, 16);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(63, 16);
			this.label2.TabIndex = 3;
			this.label2.Text = "field name: ";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(8, 56);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(53, 16);
			this.label3.TabIndex = 5;
			this.label3.Text = "field type:";
			// 
			// cbFieldType
			// 
			this.cbFieldType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbFieldType.Location = new System.Drawing.Point(72, 56);
			this.cbFieldType.Name = "cbFieldType";
			this.cbFieldType.Size = new System.Drawing.Size(121, 21);
			this.cbFieldType.TabIndex = 6;
			// 
			// btnAddField
			// 
			this.btnAddField.Location = new System.Drawing.Point(8, 88);
			this.btnAddField.Name = "btnAddField";
			this.btnAddField.TabIndex = 7;
			this.btnAddField.Text = "Add Field";
			this.btnAddField.Click += new System.EventHandler(this.btnAddField_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.cbFieldType);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.tbFieldName);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.btnAddField);
			this.groupBox1.Location = new System.Drawing.Point(8, 48);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(208, 120);
			this.groupBox1.TabIndex = 8;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Fields";
			// 
			// btnClearFields
			// 
			this.btnClearFields.Location = new System.Drawing.Point(224, 144);
			this.btnClearFields.Name = "btnClearFields";
			this.btnClearFields.TabIndex = 9;
			this.btnClearFields.Text = "Clear Fields";
			this.btnClearFields.Click += new System.EventHandler(this.btnClearFields_Click);
			// 
			// btnCreateTable
			// 
			this.btnCreateTable.Location = new System.Drawing.Point(16, 240);
			this.btnCreateTable.Name = "btnCreateTable";
			this.btnCreateTable.Size = new System.Drawing.Size(80, 23);
			this.btnCreateTable.TabIndex = 10;
			this.btnCreateTable.Text = "Create Table";
			this.btnCreateTable.Click += new System.EventHandler(this.btnCreateTable_Click);
			// 
			// btnDropTable
			// 
			this.btnDropTable.Location = new System.Drawing.Point(120, 240);
			this.btnDropTable.Name = "btnDropTable";
			this.btnDropTable.Size = new System.Drawing.Size(80, 23);
			this.btnDropTable.TabIndex = 11;
			this.btnDropTable.Text = "Drop Table";
			this.btnDropTable.Click += new System.EventHandler(this.btnDropTable_Click);
			// 
			// TestDatabase
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(352, 273);
			this.Controls.Add(this.btnDropTable);
			this.Controls.Add(this.btnCreateTable);
			this.Controls.Add(this.btnClearFields);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.lbFields);
			this.Controls.Add(this.tbTableName);
			this.Controls.Add(this.label1);
			this.Name = "TestDatabase";
			this.Text = "TestDatabase";
			this.Load += new System.EventHandler(this.TestDatabase_Load);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void TestDatabase_Load(object sender, System.EventArgs e)
		{
			cbFieldType.Items.Add(FieldType.Text);
			cbFieldType.Items.Add(FieldType.Integer);
			cbFieldType.Items.Add(FieldType.DateTime);
		}

		private void btnAddField_Click(object sender, System.EventArgs e)
		{
			Field field=new Field();
			field.Name = tbFieldName.Text;
			field.Type = (FieldType) cbFieldType.SelectedItem;
			lbFields.Items.Add(field);
		}

		private void btnClearFields_Click(object sender, System.EventArgs e)
		{
			lbFields.Items.Clear();
		}

		private void btnDropTable_Click(object sender, System.EventArgs e)
		{
			try
			{
				LocalDatabaseManager.LocalDatabase.DropTable(tbTableName.Text);
				MessageBox.Show(this, "Table Dropped Successfully", "Success", 
					MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			catch (Exception err)
			{
				MessageBox.Show(this, "Failed to drop table:\n"+err.Message, "Error", 
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void btnCreateTable_Click(object sender, System.EventArgs e)
		{
			Field[] fields=new Field[lbFields.Items.Count];
			for (int i=0; i<lbFields.Items.Count; i++)
			{
				Field field=(Field) lbFields.Items[i];
				fields[i] = field;
			}
			try
			{
				LocalDatabaseManager.LocalDatabase.CreateTable(tbTableName.Text, fields);
				MessageBox.Show(this, "Table Created Successfully", "Success", 
					MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			catch (Exception err)
			{
				MessageBox.Show(this, "Failed to create table:\n"+err.Message, "Error", 
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}
