using System;
using Sport.Rulesets.Rules;
using Sport.Rulesets;
using System.Drawing;
using System.Windows.Forms;
using Sport.Producer.UI.Rules.Dialogs;

namespace Sport.Producer.UI.Rules.Dialogs
{
	public class RankingTablesDialog : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.TextBox tbTitle;
		private System.Windows.Forms.TextBox tbValue;
		private System.Windows.Forms.Label labelTitle;
		private System.Windows.Forms.Label labelValue;
		private System.Windows.Forms.Button btnUpdate;
		private System.Windows.Forms.Button btnValueHelp;
		private System.Windows.Forms.ListBox lbRankFields;
		private System.Windows.Forms.Button btnDown;
		private System.Windows.Forms.Button btnUp;
		private System.Windows.Forms.Panel panelRankTable;
		private System.Windows.Forms.Button btnRemoveTable;
		private System.Windows.Forms.Button btnAddTable;
		private System.Windows.Forms.Label labelTable;
		private System.Windows.Forms.Button btnRename;
		private System.Windows.Forms.ComboBox cbRankingTables;
		private System.Windows.Forms.Button btnSetAsDefault;
		private System.Windows.Forms.Label labelDefaultTable;

		private Sport.Types.SportType _sportType;

		public RankingTablesDialog(RankingTables rankingTables, Sport.Types.SportType sportType)
		{
			_sportType = sportType;
			if (rankingTables == null)
				_rankingTables = new RankingTables();
			else
				_rankingTables = (RankingTables) rankingTables.Clone();
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			SetTables();

			SetButtonState();
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
			this.btnRename = new System.Windows.Forms.Button();
			this.btnRemoveTable = new System.Windows.Forms.Button();
			this.btnAddTable = new System.Windows.Forms.Button();
			this.cbRankingTables = new System.Windows.Forms.ComboBox();
			this.labelTable = new System.Windows.Forms.Label();
			this.panelRankTable = new System.Windows.Forms.Panel();
			this.btnDown = new System.Windows.Forms.Button();
			this.btnUp = new System.Windows.Forms.Button();
			this.btnValueHelp = new System.Windows.Forms.Button();
			this.btnUpdate = new System.Windows.Forms.Button();
			this.labelValue = new System.Windows.Forms.Label();
			this.labelTitle = new System.Windows.Forms.Label();
			this.tbValue = new System.Windows.Forms.TextBox();
			this.tbTitle = new System.Windows.Forms.TextBox();
			this.btnRemove = new System.Windows.Forms.Button();
			this.btnAdd = new System.Windows.Forms.Button();
			this.lbRankFields = new System.Windows.Forms.ListBox();
			this.tbOk = new System.Windows.Forms.Button();
			this.tbCancel = new System.Windows.Forms.Button();
			this.btnSetAsDefault = new System.Windows.Forms.Button();
			this.labelDefaultTable = new System.Windows.Forms.Label();
			this.panel.SuspendLayout();
			this.panelRankTable.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel
			// 
			this.panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel.Controls.Add(this.labelDefaultTable);
			this.panel.Controls.Add(this.btnSetAsDefault);
			this.panel.Controls.Add(this.btnRename);
			this.panel.Controls.Add(this.btnRemoveTable);
			this.panel.Controls.Add(this.btnAddTable);
			this.panel.Controls.Add(this.cbRankingTables);
			this.panel.Controls.Add(this.labelTable);
			this.panel.Controls.Add(this.panelRankTable);
			this.panel.Controls.Add(this.tbOk);
			this.panel.Controls.Add(this.tbCancel);
			this.panel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.panel.Location = new System.Drawing.Point(0, 0);
			this.panel.Name = "panel";
			this.panel.Size = new System.Drawing.Size(264, 266);
			this.panel.TabIndex = 7;
			this.panel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelMouseUp);
			this.panel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelMouseMove);
			this.panel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelMouseDown);
			// 
			// btnRename
			// 
			this.btnRename.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnRename.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnRename.Location = new System.Drawing.Point(7, 29);
			this.btnRename.Name = "btnRename";
			this.btnRename.Size = new System.Drawing.Size(52, 21);
			this.btnRename.TabIndex = 32;
			this.btnRename.Text = "שנה שם";
			this.btnRename.Click += new System.EventHandler(this.btnRename_Click);
			// 
			// btnRemoveTable
			// 
			this.btnRemoveTable.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnRemoveTable.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnRemoveTable.Location = new System.Drawing.Point(27, 5);
			this.btnRemoveTable.Name = "btnRemoveTable";
			this.btnRemoveTable.Size = new System.Drawing.Size(21, 21);
			this.btnRemoveTable.TabIndex = 31;
			this.btnRemoveTable.Text = "-";
			this.btnRemoveTable.Click += new System.EventHandler(this.btnRemoveTable_Click);
			// 
			// btnAddTable
			// 
			this.btnAddTable.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnAddTable.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnAddTable.Location = new System.Drawing.Point(7, 5);
			this.btnAddTable.Name = "btnAddTable";
			this.btnAddTable.Size = new System.Drawing.Size(21, 21);
			this.btnAddTable.TabIndex = 30;
			this.btnAddTable.Text = "+";
			this.btnAddTable.Click += new System.EventHandler(this.btnAddTable_Click);
			// 
			// cbRankingTables
			// 
			this.cbRankingTables.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbRankingTables.Location = new System.Drawing.Point(56, 5);
			this.cbRankingTables.Name = "cbRankingTables";
			this.cbRankingTables.Size = new System.Drawing.Size(152, 21);
			this.cbRankingTables.TabIndex = 29;
			this.cbRankingTables.SelectedIndexChanged += new System.EventHandler(this.cbRankingTables_SelectedIndexChanged);
			// 
			// labelTable
			// 
			this.labelTable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelTable.Location = new System.Drawing.Point(216, 11);
			this.labelTable.Name = "labelTable";
			this.labelTable.Size = new System.Drawing.Size(40, 16);
			this.labelTable.TabIndex = 28;
			this.labelTable.Text = "טבלה:";
			// 
			// panelRankTable
			// 
			this.panelRankTable.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.panelRankTable.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panelRankTable.Controls.Add(this.btnDown);
			this.panelRankTable.Controls.Add(this.btnUp);
			this.panelRankTable.Controls.Add(this.btnValueHelp);
			this.panelRankTable.Controls.Add(this.btnUpdate);
			this.panelRankTable.Controls.Add(this.labelValue);
			this.panelRankTable.Controls.Add(this.labelTitle);
			this.panelRankTable.Controls.Add(this.tbValue);
			this.panelRankTable.Controls.Add(this.tbTitle);
			this.panelRankTable.Controls.Add(this.btnRemove);
			this.panelRankTable.Controls.Add(this.btnAdd);
			this.panelRankTable.Controls.Add(this.lbRankFields);
			this.panelRankTable.Location = new System.Drawing.Point(7, 52);
			this.panelRankTable.Name = "panelRankTable";
			this.panelRankTable.Size = new System.Drawing.Size(249, 184);
			this.panelRankTable.TabIndex = 23;
			// 
			// btnDown
			// 
			this.btnDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnDown.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnDown.Location = new System.Drawing.Point(160, 159);
			this.btnDown.Name = "btnDown";
			this.btnDown.Size = new System.Drawing.Size(40, 20);
			this.btnDown.TabIndex = 22;
			this.btnDown.Text = "הורד";
			this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
			// 
			// btnUp
			// 
			this.btnUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnUp.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnUp.Location = new System.Drawing.Point(201, 159);
			this.btnUp.Name = "btnUp";
			this.btnUp.Size = new System.Drawing.Size(40, 20);
			this.btnUp.TabIndex = 21;
			this.btnUp.Text = "עלה";
			this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
			// 
			// btnValueHelp
			// 
			this.btnValueHelp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnValueHelp.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnValueHelp.Location = new System.Drawing.Point(49, 26);
			this.btnValueHelp.Name = "btnValueHelp";
			this.btnValueHelp.Size = new System.Drawing.Size(21, 21);
			this.btnValueHelp.TabIndex = 20;
			this.btnValueHelp.Text = "?";
			this.btnValueHelp.Click += new System.EventHandler(this.btnValueHelp_Click);
			// 
			// btnUpdate
			// 
			this.btnUpdate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnUpdate.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnUpdate.Location = new System.Drawing.Point(4, 26);
			this.btnUpdate.Name = "btnUpdate";
			this.btnUpdate.Size = new System.Drawing.Size(41, 21);
			this.btnUpdate.TabIndex = 19;
			this.btnUpdate.Text = "עדכן";
			this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
			// 
			// labelValue
			// 
			this.labelValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelValue.Location = new System.Drawing.Point(209, 31);
			this.labelValue.Name = "labelValue";
			this.labelValue.Size = new System.Drawing.Size(32, 16);
			this.labelValue.TabIndex = 18;
			this.labelValue.Text = "ערך:";
			// 
			// labelTitle
			// 
			this.labelTitle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelTitle.Location = new System.Drawing.Point(209, 9);
			this.labelTitle.Name = "labelTitle";
			this.labelTitle.Size = new System.Drawing.Size(32, 16);
			this.labelTitle.TabIndex = 17;
			this.labelTitle.Text = "שם:";
			// 
			// tbValue
			// 
			this.tbValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.tbValue.AutoSize = false;
			this.tbValue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tbValue.Location = new System.Drawing.Point(71, 26);
			this.tbValue.Name = "tbValue";
			this.tbValue.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.tbValue.Size = new System.Drawing.Size(131, 21);
			this.tbValue.TabIndex = 16;
			this.tbValue.Text = "";
			this.tbValue.TextChanged += new System.EventHandler(this.tbValue_TextChanged);
			// 
			// tbTitle
			// 
			this.tbTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.tbTitle.AutoSize = false;
			this.tbTitle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tbTitle.Location = new System.Drawing.Point(49, 4);
			this.tbTitle.Name = "tbTitle";
			this.tbTitle.Size = new System.Drawing.Size(153, 21);
			this.tbTitle.TabIndex = 15;
			this.tbTitle.Text = "";
			this.tbTitle.TextChanged += new System.EventHandler(this.tbTitle_TextChanged);
			// 
			// btnRemove
			// 
			this.btnRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnRemove.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnRemove.Location = new System.Drawing.Point(24, 4);
			this.btnRemove.Name = "btnRemove";
			this.btnRemove.Size = new System.Drawing.Size(21, 21);
			this.btnRemove.TabIndex = 8;
			this.btnRemove.Text = "-";
			this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
			// 
			// btnAdd
			// 
			this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnAdd.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnAdd.Location = new System.Drawing.Point(4, 4);
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.Size = new System.Drawing.Size(21, 21);
			this.btnAdd.TabIndex = 7;
			this.btnAdd.Text = "+";
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			// 
			// lbRankFields
			// 
			this.lbRankFields.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.lbRankFields.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lbRankFields.Location = new System.Drawing.Point(4, 48);
			this.lbRankFields.Name = "lbRankFields";
			this.lbRankFields.Size = new System.Drawing.Size(237, 106);
			this.lbRankFields.TabIndex = 9;
			this.lbRankFields.SelectedIndexChanged += new System.EventHandler(this.SelectedFieldChanged);
			// 
			// tbOk
			// 
			this.tbOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.tbOk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.tbOk.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.tbOk.Location = new System.Drawing.Point(7, 240);
			this.tbOk.Name = "tbOk";
			this.tbOk.Size = new System.Drawing.Size(68, 20);
			this.tbOk.TabIndex = 11;
			this.tbOk.Text = "אישור";
			this.tbOk.Click += new System.EventHandler(this.tbOk_Click);
			// 
			// tbCancel
			// 
			this.tbCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.tbCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.tbCancel.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.tbCancel.Location = new System.Drawing.Point(79, 240);
			this.tbCancel.Name = "tbCancel";
			this.tbCancel.Size = new System.Drawing.Size(68, 20);
			this.tbCancel.TabIndex = 10;
			this.tbCancel.Text = "ביטול";
			this.tbCancel.Click += new System.EventHandler(this.tbCancel_Click);
			// 
			// btnSetAsDefault
			// 
			this.btnSetAsDefault.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnSetAsDefault.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnSetAsDefault.Location = new System.Drawing.Point(62, 29);
			this.btnSetAsDefault.Name = "btnSetAsDefault";
			this.btnSetAsDefault.Size = new System.Drawing.Size(96, 21);
			this.btnSetAsDefault.TabIndex = 33;
			this.btnSetAsDefault.Text = "קבע כברירת מחדל";
			this.btnSetAsDefault.Click += new System.EventHandler(this.btnSetAsDefault_Click);
			// 
			// labelDefaultTable
			// 
			this.labelDefaultTable.Font = new System.Drawing.Font("Arial", 8.25F);
			this.labelDefaultTable.Location = new System.Drawing.Point(184, 32);
			this.labelDefaultTable.Name = "labelDefaultTable";
			this.labelDefaultTable.Size = new System.Drawing.Size(72, 16);
			this.labelDefaultTable.TabIndex = 34;
			this.labelDefaultTable.Text = "ברירת מחדל";
			this.labelDefaultTable.Visible = false;
			// 
			// RankingTablesDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(264, 266);
			this.ControlBox = false;
			this.Controls.Add(this.panel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "RankingTablesDialog";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "שדות";
			this.panel.ResumeLayout(false);
			this.panelRankTable.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private System.Windows.Forms.Button btnRemove;
		private System.Windows.Forms.Button tbOk;
		private System.Windows.Forms.Button tbCancel;
		private System.Windows.Forms.Button btnAdd;
		private System.Windows.Forms.Panel panel;


		private RankingTables _rankingTables;
		public RankingTables RankingTables
		{
			get { return _rankingTables; }
		}

		private void SetTables()
		{
			cbRankingTables.Items.Clear();

			foreach (RankingTable rankingTable in _rankingTables.Tables)
			{
				cbRankingTables.Items.Add(rankingTable);
			}
		}

		private void SetFields()
		{
			lbRankFields.Items.Clear();

			RankingTable rankingTable = cbRankingTables.SelectedItem as RankingTable;

			if (rankingTable != null)
			{
				foreach (RankField field in rankingTable.Fields)
				{
					lbRankFields.Items.Add(field);
				}
			}
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
			RankingTable rankingTable = cbRankingTables.SelectedItem as RankingTable;
			
			if (rankingTable != null)
			{
				if (tbTitle.Text.Length > 0 && tbValue.Text.Length > 0)
				{
					RankField rf = new RankField(tbTitle.Text, tbValue.Text);
					rankingTable.Fields.Add(rf);
					lbRankFields.Items.Add(rf);
					SetFields();
				}

				SetButtonState();
			}
		}

		private void btnRemove_Click(object sender, System.EventArgs e)
		{
			RankingTable rankingTable = cbRankingTables.SelectedItem as RankingTable;
			
			if (rankingTable != null)
			{
				RankField rf = lbRankFields.SelectedItem as RankField;

				if (rf != null)
				{
					rankingTable.Fields.Remove(rf);
					lbRankFields.Items.Remove(rf);
				}

				SetButtonState();
			}
		}

		private void btnUpdate_Click(object sender, System.EventArgs e)
		{
			RankingTable rankingTable = cbRankingTables.SelectedItem as RankingTable;
			
			if (rankingTable != null)
			{
				RankField rf = lbRankFields.SelectedItem as RankField;

				if (rf != null && tbTitle.Text.Length > 0 && tbValue.Text.Length > 0)
				{
					rf.Title = tbTitle.Text;
					rf.Value = tbValue.Text;
					int si = lbRankFields.SelectedIndex;
					SetFields();
					lbRankFields.SelectedIndex = si;
				}

				SetButtonState();
			}
		}


		private void SetButtonState()
		{
			RankingTable rankingTable = cbRankingTables.SelectedItem as RankingTable;
			btnRemoveTable.Enabled = rankingTable != null;
			btnRename.Enabled = rankingTable != null;
			btnSetAsDefault.Enabled = rankingTable != null && _rankingTables.DefaultRankingTable != rankingTable;
			labelDefaultTable.Visible = rankingTable != null && _rankingTables.DefaultRankingTable == rankingTable;
			btnRemove.Enabled = rankingTable != null && (lbRankFields.SelectedIndex != -1);
			btnAdd.Enabled = rankingTable != null && ((tbTitle.Text.Length > 0)&&(tbValue.Text.Length > 0));
			btnUpdate.Enabled = rankingTable != null && (btnRemove.Enabled && btnAdd.Enabled);
			btnUp.Enabled = rankingTable != null && (lbRankFields.SelectedIndex > 0);
			btnDown.Enabled = rankingTable != null && ((lbRankFields.SelectedIndex >= 0)&&(lbRankFields.SelectedIndex < lbRankFields.Items.Count-1));
		}

		private void SelectedFieldChanged(object sender, EventArgs e)
		{
			RankField rf = lbRankFields.SelectedItem as RankField;
			if (rf != null)
			{
				tbTitle.Text = rf.Title;
				tbValue.Text = rf.Value;
			}

			SetButtonState();
		}

		bool buttonDown = false;
		Point buttonPoint;

		protected override void OnLostFocus(EventArgs e)
		{
			buttonDown = false;
			base.OnLostFocus (e);
		}

		private void panelMouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (buttonDown)
			{
				Point pt = PointToScreen(new Point(e.X, e.Y));
				Location = new Point(pt.X - buttonPoint.X, pt.Y - buttonPoint.Y);
			}
		
		}

		private void panelMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				buttonDown = true;
				buttonPoint = new Point(e.X, e.Y);
			}
		}

		private void panelMouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			buttonDown = false;
		}


	
		public static bool EditRankingTables(ref RankingTables rts, Sport.Types.SportType sportType)
		{
			RankingTablesDialog rtd = new RankingTablesDialog(rts, sportType);

			if (rtd.ShowDialog() == DialogResult.OK)
			{
				rts = (RankingTables) rtd.RankingTables.Clone();

				return true;
			}

			return false;
		}

		public static object ValueSelector(Sport.UI.Controls.ButtonBox buttonBox, object value)
		{
			RankingTables rts = value as RankingTables;
			if (EditRankingTables(ref rts, (Sport.Types.SportType) buttonBox.Tag))
				return rts;
			return value;
		}

		private void tbTitle_TextChanged(object sender, System.EventArgs e)
		{
			SetButtonState();
		}

		private void tbValue_TextChanged(object sender, System.EventArgs e)
		{
			SetButtonState();
		}

		private void btnValueHelp_Click(object sender, System.EventArgs e)
		{
			RankFieldsHelpDialog rfhd = new RankFieldsHelpDialog(_sportType);
			if (rfhd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				tbValue.Text += "{" + rfhd.FieldValue + "}";
			}
		}

		private void btnDown_Click(object sender, System.EventArgs e)
		{
			int selIndex=lbRankFields.SelectedIndex;
			if ((selIndex < 0)||(selIndex >= lbRankFields.Items.Count-1))
				return;
			SwitchFields(selIndex, selIndex+1);
			SetFields();
			lbRankFields.SelectedIndex = selIndex+1;
		}

		private void btnUp_Click(object sender, System.EventArgs e)
		{
			int selIndex=lbRankFields.SelectedIndex;
			if (selIndex <= 0)
				return;
			SwitchFields(selIndex, selIndex-1);
			SetFields();
			lbRankFields.SelectedIndex = selIndex-1;
		}

		private void SwitchFields(int index1, int index2)
		{
			RankingTable rankingTable = cbRankingTables.SelectedItem as RankingTable;
			if (rankingTable != null)
			{
				RankField fldTemp=rankingTable.Fields[index1];
				rankingTable.Fields[index1] = rankingTable.Fields[index2];
				rankingTable.Fields[index2] = fldTemp;
			}
		}

		private void btnAddTable_Click(object sender, System.EventArgs e)
		{
			string name = "";
			if (Sport.UI.Dialogs.TextEditDialog.EditText("הכנס שם טבלה", ref name))
			{
				RankingTable rankingTable = new RankingTable();
				rankingTable.Name = name;
				_rankingTables.Tables.Add(rankingTable);
				cbRankingTables.Items.Add(rankingTable);
				cbRankingTables.SelectedItem = rankingTable;
			}
		}

		private void btnRemoveTable_Click(object sender, System.EventArgs e)
		{
			RankingTable rankingTable = cbRankingTables.SelectedItem as RankingTable;
			if (rankingTable != null)
			{
				cbRankingTables.Items.Remove(rankingTable);
				_rankingTables.Tables.Remove(rankingTable);
				SetFields();
				SetButtonState();
			}
		}

		private void btnRename_Click(object sender, System.EventArgs e)
		{
			RankingTable rankingTable = cbRankingTables.SelectedItem as RankingTable;
			if (rankingTable != null)
			{
				string name = rankingTable.Name;
				if (Sport.UI.Dialogs.TextEditDialog.EditText("הכנס שם טבלה", ref name))
				{
					rankingTable.Name = name;
				}
			}
		}

		private void btnSetAsDefault_Click(object sender, System.EventArgs e)
		{
			RankingTable rankingTable = cbRankingTables.SelectedItem as RankingTable;
			if (rankingTable != null)
			{
				_rankingTables.DefaultRankingTable = rankingTable;
				SetButtonState();
			}
		}

		private void cbRankingTables_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			RankingTable rankingTable = cbRankingTables.SelectedItem as RankingTable;
			SetFields();
			SetButtonState();
		}
	}
}
