using System;
using Sport.Rulesets.Rules;
using Sport.Rulesets;
using System.Drawing;
using System.Windows.Forms;
using Sport.Producer.UI.Rules.Dialogs;

namespace Sport.Producer.UI.Rules.Dialogs
{
	/// <summary>
	/// Summary description for FunctionariesDialog.
	/// </summary>
	public class FunctionariesDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Panel panel;
		private System.Windows.Forms.Button btnDown;
		private System.Windows.Forms.Button btnUp;
		private System.Windows.Forms.Button btnUpdate;
		private System.Windows.Forms.Label labelValue;
		private System.Windows.Forms.Label labelTitle;
		private System.Windows.Forms.Button btnRemove;
		private System.Windows.Forms.Button tbOk;
		private System.Windows.Forms.Button tbCancel;
		private System.Windows.Forms.Button btnAdd;
		private System.Windows.Forms.TextBox tbDescription;
		private System.Windows.Forms.ListBox lbFunctionaries;
		private System.Windows.Forms.ComboBox cbTypes;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		
		private Sport.Types.SportType _sportType;
		
		private Functionaries _functionaries;
		public Functionaries Functionaries
		{
			get { return _functionaries; }
		}
		
		public FunctionariesDialog(Functionaries functionaries, Sport.Types.SportType sportType)
		{
			_sportType = sportType;
			if (functionaries == null)
				_functionaries = new Functionaries();
			else
				_functionaries = (Functionaries) functionaries.Clone();
			
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			SetFields();
			SetButtonState();
		}
		
		private void SetFields()
		{
			lbFunctionaries.Items.Clear();
			
			foreach (FunctionaryValue field in _functionaries.Fields)
			{
				lbFunctionaries.Items.Add(field);
			}
		}
		
		private void SetButtonState()
		{
			btnRemove.Enabled = (lbFunctionaries.SelectedIndex != -1);
			btnAdd.Enabled = ((tbDescription.Text.Length > 0)&&(cbTypes.SelectedIndex >= 0));
			btnUpdate.Enabled = (btnRemove.Enabled && btnAdd.Enabled);
			btnUp.Enabled = (lbFunctionaries.SelectedIndex > 0);
			btnDown.Enabled = ((lbFunctionaries.SelectedIndex >= 0)&&(lbFunctionaries.SelectedIndex < lbFunctionaries.Items.Count-1));
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
		
		public static bool EditFunctionaries(ref Functionaries func, Sport.Types.SportType sportType)
		{
			FunctionariesDialog funcDialog = new FunctionariesDialog(func, sportType);

			if (funcDialog.ShowDialog() == DialogResult.OK)
			{
				func = (Functionaries) funcDialog.Functionaries.Clone();
				
				return true;
			}

			return false;
		}
		
		public static object ValueSelector(Sport.UI.Controls.ButtonBox buttonBox, object value)
		{
			Functionaries func = value as Functionaries;
			if (EditFunctionaries(ref func, (Sport.Types.SportType) buttonBox.Tag))
				return func;
			return value;
		}
		
		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.panel = new System.Windows.Forms.Panel();
			this.cbTypes = new System.Windows.Forms.ComboBox();
			this.btnDown = new System.Windows.Forms.Button();
			this.btnUp = new System.Windows.Forms.Button();
			this.btnUpdate = new System.Windows.Forms.Button();
			this.labelValue = new System.Windows.Forms.Label();
			this.labelTitle = new System.Windows.Forms.Label();
			this.tbDescription = new System.Windows.Forms.TextBox();
			this.btnRemove = new System.Windows.Forms.Button();
			this.tbOk = new System.Windows.Forms.Button();
			this.tbCancel = new System.Windows.Forms.Button();
			this.btnAdd = new System.Windows.Forms.Button();
			this.lbFunctionaries = new System.Windows.Forms.ListBox();
			this.panel.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel
			// 
			this.panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel.Controls.Add(this.cbTypes);
			this.panel.Controls.Add(this.btnDown);
			this.panel.Controls.Add(this.btnUp);
			this.panel.Controls.Add(this.btnUpdate);
			this.panel.Controls.Add(this.labelValue);
			this.panel.Controls.Add(this.labelTitle);
			this.panel.Controls.Add(this.tbDescription);
			this.panel.Controls.Add(this.btnRemove);
			this.panel.Controls.Add(this.tbOk);
			this.panel.Controls.Add(this.tbCancel);
			this.panel.Controls.Add(this.btnAdd);
			this.panel.Controls.Add(this.lbFunctionaries);
			this.panel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.panel.Location = new System.Drawing.Point(0, 0);
			this.panel.Name = "panel";
			this.panel.Size = new System.Drawing.Size(272, 256);
			this.panel.TabIndex = 8;
			this.panel.Paint += new System.Windows.Forms.PaintEventHandler(this.panel_Paint);
			// 
			// cbTypes
			// 
			this.cbTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbTypes.Location = new System.Drawing.Point(56, 32);
			this.cbTypes.Name = "cbTypes";
			this.cbTypes.Size = new System.Drawing.Size(160, 21);
			this.cbTypes.TabIndex = 23;
			this.cbTypes.TextChanged += new System.EventHandler(this.cbTypes_TextChanged);
			this.cbTypes.SelectedIndexChanged += new System.EventHandler(this.cbTypes_SelectedIndexChanged);
			// 
			// btnDown
			// 
			this.btnDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnDown.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnDown.Location = new System.Drawing.Point(175, 230);
			this.btnDown.Name = "btnDown";
			this.btnDown.Size = new System.Drawing.Size(40, 20);
			this.btnDown.TabIndex = 22;
			this.btnDown.Text = "הורד";
			this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
			// 
			// btnUp
			// 
			this.btnUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnUp.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnUp.Location = new System.Drawing.Point(216, 230);
			this.btnUp.Name = "btnUp";
			this.btnUp.Size = new System.Drawing.Size(40, 20);
			this.btnUp.TabIndex = 21;
			this.btnUp.Text = "עלה";
			this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
			// 
			// btnUpdate
			// 
			this.btnUpdate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnUpdate.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnUpdate.Location = new System.Drawing.Point(8, 30);
			this.btnUpdate.Name = "btnUpdate";
			this.btnUpdate.Size = new System.Drawing.Size(41, 21);
			this.btnUpdate.TabIndex = 19;
			this.btnUpdate.Text = "עדכן";
			this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
			// 
			// labelValue
			// 
			this.labelValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelValue.Location = new System.Drawing.Point(232, 35);
			this.labelValue.Name = "labelValue";
			this.labelValue.Size = new System.Drawing.Size(32, 16);
			this.labelValue.TabIndex = 18;
			this.labelValue.Text = "סוג:";
			// 
			// labelTitle
			// 
			this.labelTitle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelTitle.Location = new System.Drawing.Point(224, 13);
			this.labelTitle.Name = "labelTitle";
			this.labelTitle.Size = new System.Drawing.Size(40, 16);
			this.labelTitle.TabIndex = 17;
			this.labelTitle.Text = "תיאור:";
			// 
			// tbDescription
			// 
			this.tbDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.tbDescription.AutoSize = false;
			this.tbDescription.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tbDescription.Location = new System.Drawing.Point(56, 8);
			this.tbDescription.Name = "tbDescription";
			this.tbDescription.Size = new System.Drawing.Size(160, 21);
			this.tbDescription.TabIndex = 15;
			this.tbDescription.Text = "";
			this.tbDescription.TextChanged += new System.EventHandler(this.tbDescription_TextChanged);
			// 
			// btnRemove
			// 
			this.btnRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnRemove.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnRemove.Location = new System.Drawing.Point(28, 8);
			this.btnRemove.Name = "btnRemove";
			this.btnRemove.Size = new System.Drawing.Size(21, 21);
			this.btnRemove.TabIndex = 8;
			this.btnRemove.Text = "-";
			this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
			// 
			// tbOk
			// 
			this.tbOk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.tbOk.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.tbOk.Location = new System.Drawing.Point(8, 230);
			this.tbOk.Name = "tbOk";
			this.tbOk.Size = new System.Drawing.Size(68, 20);
			this.tbOk.TabIndex = 11;
			this.tbOk.Text = "אישור";
			this.tbOk.Click += new System.EventHandler(this.tbOk_Click);
			// 
			// tbCancel
			// 
			this.tbCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.tbCancel.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.tbCancel.Location = new System.Drawing.Point(80, 230);
			this.tbCancel.Name = "tbCancel";
			this.tbCancel.Size = new System.Drawing.Size(68, 20);
			this.tbCancel.TabIndex = 10;
			this.tbCancel.Text = "ביטול";
			this.tbCancel.Click += new System.EventHandler(this.tbCancel_Click);
			// 
			// btnAdd
			// 
			this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnAdd.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnAdd.Location = new System.Drawing.Point(8, 8);
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.Size = new System.Drawing.Size(21, 21);
			this.btnAdd.TabIndex = 7;
			this.btnAdd.Text = "+";
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			// 
			// lbFunctionaries
			// 
			this.lbFunctionaries.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.lbFunctionaries.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lbFunctionaries.Location = new System.Drawing.Point(8, 55);
			this.lbFunctionaries.Name = "lbFunctionaries";
			this.lbFunctionaries.Size = new System.Drawing.Size(256, 171);
			this.lbFunctionaries.TabIndex = 9;
			this.lbFunctionaries.SelectedIndexChanged += new System.EventHandler(this.lbFunctionaries_SelectedIndexChanged);
			// 
			// FunctionariesDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(272, 256);
			this.ControlBox = false;
			this.Controls.Add(this.panel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "FunctionariesDialog";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "בעלי תפקידים";
			this.Load += new System.EventHandler(this.FunctionariesDialog_Load);
			this.panel.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void tbOk_Click(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.OK;
			Close();
		}

		private void panel_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
		
		}

		private void tbCancel_Click(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}

		private void btnUp_Click(object sender, System.EventArgs e)
		{
			int selIndex=lbFunctionaries.SelectedIndex;
			if (selIndex <= 0)
				return;
			SwitchFields(selIndex, selIndex-1);
			SetFields();
			lbFunctionaries.SelectedIndex = selIndex-1;
		}

		private void btnDown_Click(object sender, System.EventArgs e)
		{
			int selIndex=lbFunctionaries.SelectedIndex;
			if ((selIndex < 0)||(selIndex >= lbFunctionaries.Items.Count-1))
				return;
			SwitchFields(selIndex, selIndex+1);
			SetFields();
			lbFunctionaries.SelectedIndex = selIndex+1;
		}
		
		private void SwitchFields(int index1, int index2)
		{
			FunctionaryValue fldTemp=_functionaries.Fields[index1];
			_functionaries.Fields[index1] = _functionaries.Fields[index2];
			_functionaries.Fields[index2] = fldTemp;
		}

		private void btnAdd_Click(object sender, System.EventArgs e)
		{
			int selIndex=cbTypes.SelectedIndex;
			if (tbDescription.Text.Length > 0 && selIndex >= 0)
			{
				string strDescription=tbDescription.Text;
				Sport.Types.FunctionaryType type=GetSelectedType();
				FunctionaryValue func = new FunctionaryValue(strDescription, type);
				_functionaries.Fields.Add(func);
				lbFunctionaries.Items.Add(func);
				SetFields();
			}
			
			SetButtonState();
		}

		private void btnRemove_Click(object sender, System.EventArgs e)
		{
			FunctionaryValue func = lbFunctionaries.SelectedItem as FunctionaryValue;

			if (func != null)
			{
				_functionaries.Fields.Remove(func);
				lbFunctionaries.Items.Remove(func);
			}
			
			SetButtonState();
		}

		private void btnUpdate_Click(object sender, System.EventArgs e)
		{
			FunctionaryValue func = lbFunctionaries.SelectedItem as FunctionaryValue;
			int selIndex=cbTypes.SelectedIndex;

			if (func != null && tbDescription.Text.Length > 0 && selIndex >= 0)
			{
				func.Description = tbDescription.Text;
				func.Type = GetSelectedType();
				int si=lbFunctionaries.SelectedIndex;
				SetFields();
				lbFunctionaries.SelectedIndex = si;
			}
			
			SetButtonState();
		}

		private void FunctionariesDialog_Load(object sender, System.EventArgs e)
		{
			System.Array values=Enum.GetValues(typeof(Sport.Types.FunctionaryType));
			for (int i=0; i<values.Length; i++)
				cbTypes.Items.Add(new FunctionaryTypeItem((Sport.Types.FunctionaryType) values.GetValue(i)));
		}
		
		private Sport.Types.FunctionaryType GetSelectedType()
		{
			return ((FunctionaryTypeItem) cbTypes.SelectedItem).Value;
		}

		private void tbDescription_TextChanged(object sender, System.EventArgs e)
		{
			SetButtonState();
		}

		private void cbTypes_TextChanged(object sender, System.EventArgs e)
		{
			SetButtonState();
		}

		private void cbTypes_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			SetButtonState();
		}

		private void lbFunctionaries_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			FunctionaryValue value = lbFunctionaries.SelectedItem as FunctionaryValue;
			if (value != null)
			{
				tbDescription.Text = value.Description;
				int index=-1;
				for (int i=0; i<cbTypes.Items.Count; i++)
				{
					if (value.Type == (cbTypes.Items[i] as FunctionaryTypeItem).Value)
					{
						index = i;
						break;
					}
				}
				cbTypes.SelectedIndex = index;
			}
			
			SetButtonState();
		}
		
		private class FunctionaryTypeItem
		{
			private Sport.Types.FunctionaryTypeLookup _lookup;
			public Sport.Types.FunctionaryType Value;
			
			public FunctionaryTypeItem(Sport.Types.FunctionaryType type)
			{
				Value = type;
				_lookup = new Sport.Types.FunctionaryTypeLookup();
			}
			
			public override string ToString()
			{
				return _lookup.Lookup((int) Value);
			}

		}
	}
}
