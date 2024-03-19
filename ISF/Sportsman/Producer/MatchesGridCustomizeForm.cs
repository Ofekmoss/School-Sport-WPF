using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Sport.UI.Dialogs;

namespace Sportsman.Producer
{
	/// <summary>
	/// Summary description for MatchesGridCustomize.
	/// </summary>
	public class MatchesGridCustomizeForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.GroupBox gbTable;
		private System.Windows.Forms.CheckBox cbShowGroup;
		private System.Windows.Forms.Button btnTableFields;
		private System.Windows.Forms.Label labelTableFields;
		private System.Windows.Forms.CheckBox cbGroupGroups;
		private System.Windows.Forms.CheckBox cbGroupRounds;
		private System.Windows.Forms.CheckBox cbGroupCycles;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.CheckBox cbGroupTournaments;

		private MatchesGridPanel matchesGridPanel;

		public MatchesGridCustomizeForm(MatchesGridPanel panel)
		{
			matchesGridPanel = panel;
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			RefreshCheckBoxes();

			RefreshLabel();
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
			this.btnOK = new System.Windows.Forms.Button();
			this.gbTable = new System.Windows.Forms.GroupBox();
			this.cbGroupCycles = new System.Windows.Forms.CheckBox();
			this.cbGroupRounds = new System.Windows.Forms.CheckBox();
			this.cbGroupGroups = new System.Windows.Forms.CheckBox();
			this.cbShowGroup = new System.Windows.Forms.CheckBox();
			this.labelTableFields = new System.Windows.Forms.Label();
			this.btnTableFields = new System.Windows.Forms.Button();
			this.cbGroupTournaments = new System.Windows.Forms.CheckBox();
			this.gbTable.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(8, 184);
			this.btnOK.Name = "btnOK";
			this.btnOK.TabIndex = 1;
			this.btnOK.Text = "סגור";
			// 
			// gbTable
			// 
			this.gbTable.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.gbTable.Controls.Add(this.cbGroupTournaments);
			this.gbTable.Controls.Add(this.cbGroupCycles);
			this.gbTable.Controls.Add(this.cbGroupRounds);
			this.gbTable.Controls.Add(this.cbGroupGroups);
			this.gbTable.Controls.Add(this.cbShowGroup);
			this.gbTable.Controls.Add(this.labelTableFields);
			this.gbTable.Controls.Add(this.btnTableFields);
			this.gbTable.Location = new System.Drawing.Point(8, 8);
			this.gbTable.Name = "gbTable";
			this.gbTable.Size = new System.Drawing.Size(384, 168);
			this.gbTable.TabIndex = 2;
			this.gbTable.TabStop = false;
			this.gbTable.Text = "טבלה";
			// 
			// cbGroupCycles
			// 
			this.cbGroupCycles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cbGroupCycles.Location = new System.Drawing.Point(152, 65);
			this.cbGroupCycles.Name = "cbGroupCycles";
			this.cbGroupCycles.Size = new System.Drawing.Size(120, 24);
			this.cbGroupCycles.TabIndex = 11;
			this.cbGroupCycles.Text = "חלק לפי מחזורים";
			this.cbGroupCycles.CheckedChanged += new System.EventHandler(this.cbGroupCycles_CheckedChanged);
			// 
			// cbGroupRounds
			// 
			this.cbGroupRounds.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cbGroupRounds.Location = new System.Drawing.Point(160, 41);
			this.cbGroupRounds.Name = "cbGroupRounds";
			this.cbGroupRounds.Size = new System.Drawing.Size(112, 24);
			this.cbGroupRounds.TabIndex = 10;
			this.cbGroupRounds.Text = "חלק לפי סיבובים";
			this.cbGroupRounds.CheckedChanged += new System.EventHandler(this.cbGroupRounds_CheckedChanged);
			// 
			// cbGroupGroups
			// 
			this.cbGroupGroups.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cbGroupGroups.Location = new System.Drawing.Point(168, 17);
			this.cbGroupGroups.Name = "cbGroupGroups";
			this.cbGroupGroups.TabIndex = 9;
			this.cbGroupGroups.Text = "חלק לפי בתים";
			this.cbGroupGroups.CheckedChanged += new System.EventHandler(this.cbGroupGroups_CheckedChanged);
			// 
			// cbShowGroup
			// 
			this.cbShowGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cbShowGroup.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.cbShowGroup.Location = new System.Drawing.Point(296, 19);
			this.cbShowGroup.Name = "cbShowGroup";
			this.cbShowGroup.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.cbShowGroup.Size = new System.Drawing.Size(80, 24);
			this.cbShowGroup.TabIndex = 8;
			this.cbShowGroup.Text = "הצג בתים";
			this.cbShowGroup.CheckedChanged += new System.EventHandler(this.cbShowGroup_CheckedChanged);
			// 
			// labelTableFields
			// 
			this.labelTableFields.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.labelTableFields.Location = new System.Drawing.Point(11, 120);
			this.labelTableFields.Name = "labelTableFields";
			this.labelTableFields.Size = new System.Drawing.Size(280, 40);
			this.labelTableFields.TabIndex = 7;
			this.labelTableFields.Text = "label1";
			// 
			// btnTableFields
			// 
			this.btnTableFields.Location = new System.Drawing.Point(299, 128);
			this.btnTableFields.Name = "btnTableFields";
			this.btnTableFields.TabIndex = 6;
			this.btnTableFields.Text = "שדות...";
			this.btnTableFields.Click += new System.EventHandler(this.btnTableFields_Click);
			// 
			// cbGroupTournaments
			// 
			this.cbGroupTournaments.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cbGroupTournaments.Location = new System.Drawing.Point(152, 88);
			this.cbGroupTournaments.Name = "cbGroupTournaments";
			this.cbGroupTournaments.Size = new System.Drawing.Size(120, 24);
			this.cbGroupTournaments.TabIndex = 12;
			this.cbGroupTournaments.Text = "חלק לפי טורנירים";
			this.cbGroupTournaments.CheckedChanged += new System.EventHandler(this.cbGroupTournaments_CheckedChanged);
			// 
			// MatchesGridCustomizeForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.ClientSize = new System.Drawing.Size(400, 216);
			this.Controls.Add(this.gbTable);
			this.Controls.Add(this.btnOK);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "MatchesGridCustomizeForm";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "משחקים - התאמה";
			this.gbTable.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void btnTableFields_Click(object sender, System.EventArgs e)
		{
			ArrayList source = new ArrayList();
			Sport.UI.IVisualizerField[] fields = matchesGridPanel.Visualizer.GetFields();
			foreach (Sport.UI.IVisualizerField field in fields)
			{
				source.Add(field);
			}
			ArrayList target = new ArrayList();
			foreach (MatchVisualizer.MatchField field in matchesGridPanel.Columns)
			{
				target.Add(fields[(int) field]);
				source.Remove(fields[(int) field]);
			}
			ChooseItemsForm cif = new ChooseItemsForm("בחר שדות לטבלה",
				"שדות פנויים:", "שדות בטבלה:",
				source.ToArray(), target.ToArray());

			if (cif.ShowDialog() == DialogResult.OK)
			{
				object[] ti = cif.Target;
				MatchVisualizer.MatchField[] columns = 
					new MatchVisualizer.MatchField[ti.Length];
				
				for (int n = 0; n < ti.Length; n++)
				{
					columns[n] = (MatchVisualizer.MatchField) ((Sport.UI.IVisualizerField) ti[n]).Field;
				}
				
				//store user choice:
				//matchesGridPanel.SaveSettings("CustomizeColumns", items);
				
				matchesGridPanel.Columns = columns;
				RefreshLabel();
			}
		}

		private void RefreshCheckBoxes()
		{
			cbShowGroup.Checked = matchesGridPanel.ShowGroups;

			if (cbShowGroup.Checked)
			{
				cbGroupGroups.Enabled = true;
				cbGroupGroups.Checked = matchesGridPanel.GroupGroups;
			}
			else
			{
				cbGroupGroups.Enabled = false;
				cbGroupGroups.Checked = false;
			}

			cbGroupRounds.Checked = matchesGridPanel.GroupRounds;
			cbGroupCycles.Checked = matchesGridPanel.GroupCycles;
			cbGroupTournaments.Checked = matchesGridPanel.GroupTournaments;
		}

		private void RefreshLabel()
		{
			string columns = "";

			foreach (MatchVisualizer.MatchField field in matchesGridPanel.Columns)
			{
				if (columns.Length > 0)
					columns += ", ";
				columns += matchesGridPanel.Visualizer.GetField((int) field).Title;
			}

			labelTableFields.Text = columns;
		}

		private void cbShowGroup_CheckedChanged(object sender, System.EventArgs e)
		{
			matchesGridPanel.ShowGroups = cbShowGroup.Checked;
			RefreshCheckBoxes();
		}

		private void cbGroupGroups_CheckedChanged(object sender, System.EventArgs e)
		{
			matchesGridPanel.GroupGroups = cbGroupGroups.Checked;
			RefreshCheckBoxes();
		}

		private void cbGroupRounds_CheckedChanged(object sender, System.EventArgs e)
		{
			matchesGridPanel.GroupRounds = cbGroupRounds.Checked;
			RefreshCheckBoxes();
		}

		private void cbGroupCycles_CheckedChanged(object sender, System.EventArgs e)
		{
			matchesGridPanel.GroupCycles = cbGroupCycles.Checked;
			RefreshCheckBoxes();
		}

		private void cbGroupTournaments_CheckedChanged(object sender, System.EventArgs e)
		{
			matchesGridPanel.GroupTournaments = cbGroupTournaments.Checked;
			RefreshCheckBoxes();
		}
	}
}
