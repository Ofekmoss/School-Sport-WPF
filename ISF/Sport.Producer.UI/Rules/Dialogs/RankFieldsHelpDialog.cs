using System;
using Sport.Rulesets.Rules;
using Sport.Rulesets;
using System.Drawing;
using System.Windows.Forms;

namespace Sport.Producer.UI.Rules.Dialogs
{
	public class RankFieldsHelpDialog : System.Windows.Forms.Form
	{
		private class Field
		{
			private string _name;
			public string Name
			{
				get { return _name; }
			}
			private string _description;
			public string Description
			{
				get { return _description; }
			}
			public Field(string name, string description)
			{
				_name = name;
				_description = description;
			}
			public override string ToString()
			{
				return _name + " - " + _description;
			}

		}

		public RankFieldsHelpDialog(Sport.Types.SportType sportType)
		{
			InitializeComponent();

			lbFields.Items.Add(new Field("R", "מיקום"));
			lbFields.Items.Add(new Field("S", "ניקוד"));

			if ((sportType & Sport.Types.SportType.Match) != 0)
			{
				lbFields.Items.Add(new Field("G", "משחקים"));
				lbFields.Items.Add(new Field("P", "נק'/מערכות/שערי זכות"));
				lbFields.Items.Add(new Field("C", "נק'/מערכות/שערי חובה"));
				//lbFields.Items.Add(new Field("M", "מערכות זכות"));
				//lbFields.Items.Add(new Field("N", "מערכות חובה"));
				lbFields.Items.Add(new Field("T", "נקודות קטנות זכות"));
				lbFields.Items.Add(new Field("Y", "נקודות קטנות חובה"));
				lbFields.Items.Add(new Field("W", "נצחונות"));
				lbFields.Items.Add(new Field("L", "הפסדים"));
				lbFields.Items.Add(new Field("E", "נצחונות טכניים"));
				lbFields.Items.Add(new Field("F", "הפסדים טכניים"));
				lbFields.Items.Add(new Field("D", "תיקו"));
			}
			if ((sportType & Sport.Types.SportType.Competition) != 0)
			{
				lbFields.Items.Add(new Field("C", "משתתפים"));
				lbFields.Items.Add(new Field("Cn", "משתתפים למונה"));
				lbFields.Items.Add(new Field("Sn", "ניקוד למונה"));
			}
		}

		private System.Windows.Forms.Button btnClose;
		public System.Windows.Forms.TextBox tbField;
		public System.Windows.Forms.Button button2;
		private System.Windows.Forms.ListBox lbFields;
		private System.Windows.Forms.Button btnAddField;
		public System.Windows.Forms.Button btnAdd;
		public System.Windows.Forms.Button btnDiv;
		public System.Windows.Forms.Button btnSub;
		public System.Windows.Forms.Button btnMul;
		public System.Windows.Forms.Button btnOpenBracet;
		public System.Windows.Forms.Button btnCloseBarcet;
		private System.Windows.Forms.Panel panel;
	
		private void InitializeComponent()
		{
			this.panel = new System.Windows.Forms.Panel();
			this.button2 = new System.Windows.Forms.Button();
			this.btnCloseBarcet = new System.Windows.Forms.Button();
			this.btnOpenBracet = new System.Windows.Forms.Button();
			this.btnMul = new System.Windows.Forms.Button();
			this.btnSub = new System.Windows.Forms.Button();
			this.btnDiv = new System.Windows.Forms.Button();
			this.btnAdd = new System.Windows.Forms.Button();
			this.tbField = new System.Windows.Forms.TextBox();
			this.btnAddField = new System.Windows.Forms.Button();
			this.lbFields = new System.Windows.Forms.ListBox();
			this.btnClose = new System.Windows.Forms.Button();
			this.panel.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel
			// 
			this.panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel.Controls.Add(this.button2);
			this.panel.Controls.Add(this.btnCloseBarcet);
			this.panel.Controls.Add(this.btnOpenBracet);
			this.panel.Controls.Add(this.btnMul);
			this.panel.Controls.Add(this.btnSub);
			this.panel.Controls.Add(this.btnDiv);
			this.panel.Controls.Add(this.btnAdd);
			this.panel.Controls.Add(this.tbField);
			this.panel.Controls.Add(this.btnAddField);
			this.panel.Controls.Add(this.lbFields);
			this.panel.Controls.Add(this.btnClose);
			this.panel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel.Location = new System.Drawing.Point(0, 0);
			this.panel.Name = "panel";
			this.panel.Size = new System.Drawing.Size(160, 196);
			this.panel.TabIndex = 0;
			// 
			// button2
			// 
			this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.button2.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.button2.Location = new System.Drawing.Point(125, 144);
			this.button2.Name = "button2";
			this.button2.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.button2.Size = new System.Drawing.Size(21, 21);
			this.button2.TabIndex = 17;
			this.button2.Text = "<";
			this.button2.Click += new System.EventHandler(this.DelClick);
			// 
			// btnCloseBarcet
			// 
			this.btnCloseBarcet.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnCloseBarcet.Location = new System.Drawing.Point(126, 120);
			this.btnCloseBarcet.Name = "btnCloseBarcet";
			this.btnCloseBarcet.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.btnCloseBarcet.Size = new System.Drawing.Size(20, 20);
			this.btnCloseBarcet.TabIndex = 16;
			this.btnCloseBarcet.Text = ")";
			this.btnCloseBarcet.Click += new System.EventHandler(this.ButtonClick);
			// 
			// btnOpenBracet
			// 
			this.btnOpenBracet.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnOpenBracet.Location = new System.Drawing.Point(102, 120);
			this.btnOpenBracet.Name = "btnOpenBracet";
			this.btnOpenBracet.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.btnOpenBracet.Size = new System.Drawing.Size(20, 20);
			this.btnOpenBracet.TabIndex = 15;
			this.btnOpenBracet.Text = "(";
			this.btnOpenBracet.Click += new System.EventHandler(this.ButtonClick);
			// 
			// btnMul
			// 
			this.btnMul.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnMul.Location = new System.Drawing.Point(54, 120);
			this.btnMul.Name = "btnMul";
			this.btnMul.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.btnMul.Size = new System.Drawing.Size(20, 20);
			this.btnMul.TabIndex = 14;
			this.btnMul.Tag = "";
			this.btnMul.Text = "*";
			this.btnMul.Click += new System.EventHandler(this.ButtonClick);
			// 
			// btnSub
			// 
			this.btnSub.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnSub.Location = new System.Drawing.Point(30, 120);
			this.btnSub.Name = "btnSub";
			this.btnSub.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.btnSub.Size = new System.Drawing.Size(20, 20);
			this.btnSub.TabIndex = 13;
			this.btnSub.Tag = "";
			this.btnSub.Text = "-";
			this.btnSub.Click += new System.EventHandler(this.ButtonClick);
			// 
			// btnDiv
			// 
			this.btnDiv.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnDiv.Location = new System.Drawing.Point(78, 120);
			this.btnDiv.Name = "btnDiv";
			this.btnDiv.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.btnDiv.Size = new System.Drawing.Size(20, 20);
			this.btnDiv.TabIndex = 12;
			this.btnDiv.Tag = "";
			this.btnDiv.Text = "/";
			this.btnDiv.Click += new System.EventHandler(this.ButtonClick);
			// 
			// btnAdd
			// 
			this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnAdd.Location = new System.Drawing.Point(6, 120);
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.btnAdd.Size = new System.Drawing.Size(20, 20);
			this.btnAdd.TabIndex = 10;
			this.btnAdd.Tag = "";
			this.btnAdd.Text = "+";
			this.btnAdd.Click += new System.EventHandler(this.ButtonClick);
			// 
			// tbField
			// 
			this.tbField.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tbField.Location = new System.Drawing.Point(6, 144);
			this.tbField.Name = "tbField";
			this.tbField.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.tbField.Size = new System.Drawing.Size(120, 21);
			this.tbField.TabIndex = 9;
			this.tbField.Text = "";
			this.tbField.TextChanged += new System.EventHandler(this.tbField_TextChanged);
			// 
			// btnAddField
			// 
			this.btnAddField.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnAddField.Enabled = false;
			this.btnAddField.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnAddField.Location = new System.Drawing.Point(62, 168);
			this.btnAddField.Name = "btnAddField";
			this.btnAddField.Size = new System.Drawing.Size(54, 21);
			this.btnAddField.TabIndex = 8;
			this.btnAddField.Text = "הוסף";
			this.btnAddField.Click += new System.EventHandler(this.btnAddField_Click);
			// 
			// lbFields
			// 
			this.lbFields.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lbFields.Location = new System.Drawing.Point(6, 8);
			this.lbFields.Name = "lbFields";
			this.lbFields.Size = new System.Drawing.Size(140, 106);
			this.lbFields.TabIndex = 7;
			this.lbFields.SelectedIndexChanged += new System.EventHandler(this.lbFields_SelectedIndexChanged);
			// 
			// btnClose
			// 
			this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnClose.Location = new System.Drawing.Point(6, 168);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(54, 21);
			this.btnClose.TabIndex = 6;
			this.btnClose.Text = "סגור";
			// 
			// RankFieldsHelpDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.ClientSize = new System.Drawing.Size(160, 196);
			this.Controls.Add(this.panel);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "RankFieldsHelpDialog";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.panel.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		public string FieldValue
		{
			get { return tbField.Text; }
		}

		private void ButtonClick(object sender, System.EventArgs e)
		{
			Button btn = sender as Button;
			if (btn != null)
			{
				tbField.Text += btn.Text;
				tbField.SelectionStart = tbField.Text.Length;
			}
		}

		private void DelClick(object sender, System.EventArgs e)
		{
			if (tbField.Text.Length > 0)
			{
				tbField.Text = tbField.Text.Substring(0, tbField.Text.Length - 1);
				tbField.SelectionStart = tbField.Text.Length;
			}
		}

		private void lbFields_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			Field field = lbFields.SelectedItem as Field;
			if (field != null)
			{
				tbField.Text += field.Name;
				tbField.SelectionStart = tbField.Text.Length;
			}
		}

		private void tbField_TextChanged(object sender, System.EventArgs e)
		{
			btnAddField.Enabled = tbField.Text.Length > 0;
		}

		private void btnAddField_Click(object sender, System.EventArgs e)
		{
		
		}
	}
}
