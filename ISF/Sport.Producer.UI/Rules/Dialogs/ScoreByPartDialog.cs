using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sport.Rulesets.Rules;
using System.Windows.Forms;

namespace Sport.Producer.UI.Rules.Dialogs
{
	public class ScoreByPartDialog : System.Windows.Forms.Form
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
		private System.Windows.Forms.TextBox tbResult;
		private System.Windows.Forms.ListBox lbScoreByPartItems;
		private System.Windows.Forms.TextBox tbScore;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ScoreByParts ScoreByPartsItem { get; private set; }

		public ScoreByPartDialog(ScoreByParts scoreByPartsItem)
		{
			this.ScoreByPartsItem = scoreByPartsItem == null ? new ScoreByParts() : (ScoreByParts)scoreByPartsItem.Clone();

			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			SetFields();
			SetButtonState();
		}

		private void SetFields()
		{
			lbScoreByPartItems.Items.Clear();

			foreach (ScoreByParts.Value scoreByPartValue in this.ScoreByPartsItem.Values)
			{
				lbScoreByPartItems.Items.Add(scoreByPartValue);
			}
		}

		private void SetButtonState()
		{
			btnRemove.Enabled = (lbScoreByPartItems.SelectedIndex != -1);
			btnAdd.Enabled = ((tbResult.Text.Length > 0) && (tbScore.Text.Length > 0));
			btnUpdate.Enabled = (btnRemove.Enabled && btnAdd.Enabled);
			btnUp.Enabled = (lbScoreByPartItems.SelectedIndex > 0);
			btnDown.Enabled = ((lbScoreByPartItems.SelectedIndex >= 0) && (lbScoreByPartItems.SelectedIndex < lbScoreByPartItems.Items.Count - 1));
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		public static bool EditScoreByPartItems(ref ScoreByParts scoreByPartsItem)
		{
			ScoreByPartDialog dialog = new ScoreByPartDialog(scoreByPartsItem);
			if (dialog.ShowDialog() == DialogResult.OK)
			{
				scoreByPartsItem = dialog.ScoreByPartsItem == null ? new ScoreByParts() : (ScoreByParts)dialog.ScoreByPartsItem.Clone();
				return true;
			}

			return false;
		}

		public static object ValueSelector(Sport.UI.Controls.ButtonBox buttonBox, object value)
		{
			ScoreByParts sbp = value as ScoreByParts;
			if (EditScoreByPartItems(ref sbp))
				return sbp;
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
			this.tbScore = new System.Windows.Forms.TextBox();
			this.btnDown = new System.Windows.Forms.Button();
			this.btnUp = new System.Windows.Forms.Button();
			this.btnUpdate = new System.Windows.Forms.Button();
			this.labelValue = new System.Windows.Forms.Label();
			this.labelTitle = new System.Windows.Forms.Label();
			this.tbResult = new System.Windows.Forms.TextBox();
			this.btnRemove = new System.Windows.Forms.Button();
			this.tbOk = new System.Windows.Forms.Button();
			this.tbCancel = new System.Windows.Forms.Button();
			this.btnAdd = new System.Windows.Forms.Button();
			this.lbScoreByPartItems = new System.Windows.Forms.ListBox();
			this.panel.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel
			// 
			this.panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel.Controls.Add(this.tbScore);
			this.panel.Controls.Add(this.btnDown);
			this.panel.Controls.Add(this.btnUp);
			this.panel.Controls.Add(this.btnUpdate);
			this.panel.Controls.Add(this.labelValue);
			this.panel.Controls.Add(this.labelTitle);
			this.panel.Controls.Add(this.tbResult);
			this.panel.Controls.Add(this.btnRemove);
			this.panel.Controls.Add(this.tbOk);
			this.panel.Controls.Add(this.tbCancel);
			this.panel.Controls.Add(this.btnAdd);
			this.panel.Controls.Add(this.lbScoreByPartItems);
			this.panel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.panel.Location = new System.Drawing.Point(0, 0);
			this.panel.Name = "panel";
			this.panel.Size = new System.Drawing.Size(272, 256);
			this.panel.TabIndex = 8;
			this.panel.Paint += new System.Windows.Forms.PaintEventHandler(this.panel_Paint);
			// 
			// tbScore
			// 
			this.tbScore.AutoSize = false;
			this.tbScore.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tbScore.Location = new System.Drawing.Point(56, 32);
			this.tbScore.Name = "tbScore";
			this.tbScore.Size = new System.Drawing.Size(160, 21);
			this.tbScore.TabIndex = 23;
			this.tbScore.Text = "";
			this.tbScore.TextChanged += new System.EventHandler(this.tbScore_TextChanged);
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
			this.labelValue.Text = "ניקוד:";
			// 
			// labelTitle
			// 
			this.labelTitle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelTitle.Location = new System.Drawing.Point(224, 13);
			this.labelTitle.Name = "labelTitle";
			this.labelTitle.Size = new System.Drawing.Size(40, 16);
			this.labelTitle.TabIndex = 17;
			this.labelTitle.Text = "תוצאה:";
			// 
			// tbResult
			// 
			this.tbResult.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.tbResult.AutoSize = false;
			this.tbResult.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tbResult.Location = new System.Drawing.Point(56, 8);
			this.tbResult.Name = "tbResult";
			this.tbResult.Size = new System.Drawing.Size(160, 21);
			this.tbResult.TabIndex = 15;
			this.tbResult.Text = "";
			this.tbResult.TextChanged += new System.EventHandler(this.tbResult_TextChanged);
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
			// lbScoreByPartItems
			// 
			this.lbScoreByPartItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
				| System.Windows.Forms.AnchorStyles.Left)
				| System.Windows.Forms.AnchorStyles.Right)));
			this.lbScoreByPartItems.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lbScoreByPartItems.Location = new System.Drawing.Point(8, 55);
			this.lbScoreByPartItems.Name = "lbScoreByPartItems";
			this.lbScoreByPartItems.Size = new System.Drawing.Size(256, 171);
			this.lbScoreByPartItems.TabIndex = 9;
			this.lbScoreByPartItems.SelectedIndexChanged += new System.EventHandler(this.lbScoreByPartItems_SelectedIndexChanged);
			// 
			// ScoreByPartDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(272, 256);
			this.ControlBox = false;
			this.Controls.Add(this.panel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "ScoreByPartDialog";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "ניקוד משחק לפי חלקים";
			this.Load += new System.EventHandler(this.ScoreByPartDialog_Load);
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
			int selIndex = lbScoreByPartItems.SelectedIndex;
			if (selIndex <= 0)
				return;
			SwitchFields(selIndex, selIndex - 1);
			SetFields();
			lbScoreByPartItems.SelectedIndex = selIndex - 1;
		}

		private void btnDown_Click(object sender, System.EventArgs e)
		{
			int selIndex = lbScoreByPartItems.SelectedIndex;
			if ((selIndex < 0) || (selIndex >= lbScoreByPartItems.Items.Count - 1))
				return;
			SwitchFields(selIndex, selIndex + 1);
			SetFields();
			lbScoreByPartItems.SelectedIndex = selIndex + 1;
		}

		private void SwitchFields(int index1, int index2)
		{
			ScoreByParts.Value fldTemp = this.ScoreByPartsItem.Values[index1];
			this.ScoreByPartsItem.Values[index1] = this.ScoreByPartsItem.Values[index2];
			this.ScoreByPartsItem.Values[index2] = fldTemp;
		}

		private void btnAdd_Click(object sender, System.EventArgs e)
		{
			if (tbResult.Text.Length > 0 && tbScore.Text.Length > 0)
			{
				ScoreByParts.Value scoreByPartsValue;
				string errorMsg;
				if (ScoreByParts.Value.TryCreate(tbScore.Text, tbResult.Text, out scoreByPartsValue, out errorMsg))
				{
					this.ScoreByPartsItem.AddValue(scoreByPartsValue);
					lbScoreByPartItems.Items.Add(scoreByPartsValue);
					SetFields();
				}
				else
				{
					Sport.UI.MessageBox.Error(errorMsg, "שגיאה");
				}
			}

			SetButtonState();
		}

		private void btnRemove_Click(object sender, System.EventArgs e)
		{
			ScoreByParts.Value sbp = lbScoreByPartItems.SelectedItem as ScoreByParts.Value;
			if (sbp != null)
			{
				this.ScoreByPartsItem.RemoveValue(sbp);
				lbScoreByPartItems.Items.Remove(sbp);
			}
			SetButtonState();
		}

		private void btnUpdate_Click(object sender, System.EventArgs e)
		{
			ScoreByParts.Value sbp = lbScoreByPartItems.SelectedItem as ScoreByParts.Value;
			if (sbp != null && tbResult.Text.Length > 0 && tbScore.Text.Length > 0)
			{
				string errorMsg;
				if (sbp.TryUpdate(tbScore.Text, tbResult.Text, out errorMsg))
				{
					int si = lbScoreByPartItems.SelectedIndex;
					SetFields();
					lbScoreByPartItems.SelectedIndex = si;
				}
				else
				{
					Sport.UI.MessageBox.Error(errorMsg, "שגיאה");
				}
			}

			SetButtonState();
		}

		private void ScoreByPartDialog_Load(object sender, System.EventArgs e)
		{

		}

		private void tbResult_TextChanged(object sender, System.EventArgs e)
		{
			SetButtonState();
		}

		private void tbScore_TextChanged(object sender, System.EventArgs e)
		{
			SetButtonState();
		}

		private void lbScoreByPartItems_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			ScoreByParts.Value value = lbScoreByPartItems.SelectedItem as ScoreByParts.Value;
			if (value != null)
			{
				tbScore.Text = string.Format("{0}-{1}", value.TeamA_PartScore, value.TeamB_PartScore);
				tbResult.Text = string.Format("{0}-{1}", value.TeamA_GameScore, value.TeamB_GameScore);
			}

			SetButtonState();
		}
	}
}
