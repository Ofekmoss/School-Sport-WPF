using System;
using Sport.Rulesets.Rules;
using Sport.Rulesets;
using System.Drawing;
using System.Windows.Forms;
using Sport.Producer.UI.Rules.Dialogs;

namespace Sport.Producer.UI.Rules.Dialogs
{
	public class TeamScoreCountersDialog : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.TextBox tbName;
		private System.Windows.Forms.NumericUpDown nudResults;
		private System.Windows.Forms.Label labelName;
		private System.Windows.Forms.Label labelResults;
		private System.Windows.Forms.Button btnUpdate;
		private System.Windows.Forms.ListBox lbTeamScoreCounters;
		private System.Windows.Forms.Button btnDown;
		private System.Windows.Forms.Button btnUp;

		public TeamScoreCountersDialog(TeamScoreCounters teamScoreCounters)
		{
			if (teamScoreCounters == null)
				_teamScoreCounters = new TeamScoreCounters();
			else
				_teamScoreCounters = (TeamScoreCounters) teamScoreCounters.Clone();
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			SetCounters();

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
			this.btnDown = new System.Windows.Forms.Button();
			this.btnUp = new System.Windows.Forms.Button();
			this.btnUpdate = new System.Windows.Forms.Button();
			this.labelResults = new System.Windows.Forms.Label();
			this.labelName = new System.Windows.Forms.Label();
			this.nudResults = new System.Windows.Forms.NumericUpDown();
			this.tbName = new System.Windows.Forms.TextBox();
			this.btnRemove = new System.Windows.Forms.Button();
			this.tbOk = new System.Windows.Forms.Button();
			this.tbCancel = new System.Windows.Forms.Button();
			this.btnAdd = new System.Windows.Forms.Button();
			this.lbTeamScoreCounters = new System.Windows.Forms.ListBox();
			this.panel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudResults)).BeginInit();
			this.SuspendLayout();
			// 
			// panel
			// 
			this.panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel.Controls.Add(this.btnDown);
			this.panel.Controls.Add(this.btnUp);
			this.panel.Controls.Add(this.btnUpdate);
			this.panel.Controls.Add(this.labelResults);
			this.panel.Controls.Add(this.labelName);
			this.panel.Controls.Add(this.nudResults);
			this.panel.Controls.Add(this.tbName);
			this.panel.Controls.Add(this.btnRemove);
			this.panel.Controls.Add(this.tbOk);
			this.panel.Controls.Add(this.tbCancel);
			this.panel.Controls.Add(this.btnAdd);
			this.panel.Controls.Add(this.lbTeamScoreCounters);
			this.panel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.panel.Location = new System.Drawing.Point(0, 0);
			this.panel.Name = "panel";
			this.panel.Size = new System.Drawing.Size(232, 256);
			this.panel.TabIndex = 7;
			this.panel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelMouseUp);
			this.panel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelMouseMove);
			this.panel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelMouseDown);
			// 
			// btnDown
			// 
			this.btnDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnDown.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnDown.Location = new System.Drawing.Point(143, 230);
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
			this.btnUp.Location = new System.Drawing.Point(184, 230);
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
			// labelResults
			// 
			this.labelResults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelResults.Location = new System.Drawing.Point(184, 35);
			this.labelResults.Name = "labelResults";
			this.labelResults.Size = new System.Drawing.Size(40, 16);
			this.labelResults.TabIndex = 18;
			this.labelResults.Text = "כמות:";
			// 
			// labelName
			// 
			this.labelName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelName.Location = new System.Drawing.Point(192, 13);
			this.labelName.Name = "labelName";
			this.labelName.Size = new System.Drawing.Size(32, 16);
			this.labelName.TabIndex = 17;
			this.labelName.Text = "שם:";
			// 
			// nudResults
			// 
			this.nudResults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.nudResults.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.nudResults.Location = new System.Drawing.Point(56, 30);
			this.nudResults.Minimum = new System.Decimal(new int[] {
																	   1,
																	   0,
																	   0,
																	   0});
			this.nudResults.Name = "nudResults";
			this.nudResults.TabIndex = 16;
			this.nudResults.Value = new System.Decimal(new int[] {
																	 1,
																	 0,
																	 0,
																	 0});
			this.nudResults.TextChanged += new System.EventHandler(this.nudResults_TextChanged);
			// 
			// tbName
			// 
			this.tbName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.tbName.AutoSize = false;
			this.tbName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tbName.Location = new System.Drawing.Point(56, 8);
			this.tbName.Name = "tbName";
			this.tbName.Size = new System.Drawing.Size(120, 21);
			this.tbName.TabIndex = 15;
			this.tbName.Text = "";
			this.tbName.TextChanged += new System.EventHandler(this.tbName_TextChanged);
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
			this.tbOk.Size = new System.Drawing.Size(64, 20);
			this.tbOk.TabIndex = 11;
			this.tbOk.Text = "אישור";
			this.tbOk.Click += new System.EventHandler(this.tbOk_Click);
			// 
			// tbCancel
			// 
			this.tbCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.tbCancel.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.tbCancel.Location = new System.Drawing.Point(76, 230);
			this.tbCancel.Name = "tbCancel";
			this.tbCancel.Size = new System.Drawing.Size(64, 20);
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
			// lbTeamScoreCounters
			// 
			this.lbTeamScoreCounters.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.lbTeamScoreCounters.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lbTeamScoreCounters.Location = new System.Drawing.Point(8, 52);
			this.lbTeamScoreCounters.Name = "lbTeamScoreCounters";
			this.lbTeamScoreCounters.Size = new System.Drawing.Size(216, 171);
			this.lbTeamScoreCounters.TabIndex = 9;
			this.lbTeamScoreCounters.SelectedIndexChanged += new System.EventHandler(this.SelectedCounterChanged);
			// 
			// TeamScoreCountersDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(232, 256);
			this.ControlBox = false;
			this.Controls.Add(this.panel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "TeamScoreCountersDialog";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "שדות";
			this.panel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.nudResults)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		private System.Windows.Forms.Button btnRemove;
		private System.Windows.Forms.Button tbOk;
		private System.Windows.Forms.Button tbCancel;
		private System.Windows.Forms.Button btnAdd;
		private System.Windows.Forms.Panel panel;


		private TeamScoreCounters _teamScoreCounters;
		public TeamScoreCounters TeamScoreCounters
		{
			get { return _teamScoreCounters; }
		}

		private void SetCounters()
		{
			lbTeamScoreCounters.Items.Clear();
			
			foreach (TeamScoreCounters.Counter counter in _teamScoreCounters.Counters)
			{
				lbTeamScoreCounters.Items.Add(counter);
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
			if (tbName.Text.Length > 0 && nudResults.Value > 0)
			{
				TeamScoreCounters.Counter c = new TeamScoreCounters.Counter(tbName.Text, (int) nudResults.Value);
				_teamScoreCounters.Counters.Add(c);
				lbTeamScoreCounters.Items.Add(c);
				SetCounters();
			}

			SetButtonState();
		}

		private void btnRemove_Click(object sender, System.EventArgs e)
		{
			TeamScoreCounters.Counter c = lbTeamScoreCounters.SelectedItem as TeamScoreCounters.Counter;

			if (c != null)
			{
				_teamScoreCounters.Counters.Remove(c);
				lbTeamScoreCounters.Items.Remove(c);
			}

			SetButtonState();
		}

		private void btnUpdate_Click(object sender, System.EventArgs e)
		{
			TeamScoreCounters.Counter c = lbTeamScoreCounters.SelectedItem as TeamScoreCounters.Counter;

			if (c != null && tbName.Text.Length > 0 && nudResults.Value > 0)
			{
				c.Name = tbName.Text;
				c.Results = (int) nudResults.Value;
				int si = lbTeamScoreCounters.SelectedIndex;
				SetCounters();
				lbTeamScoreCounters.SelectedIndex = si;
			}

			SetButtonState();
		}


		private void SetButtonState()
		{
			btnRemove.Enabled = (lbTeamScoreCounters.SelectedIndex != -1);
			btnAdd.Enabled = ((tbName.Text.Length > 0)&&(nudResults.Value > 0));
			btnUpdate.Enabled = (btnRemove.Enabled && btnAdd.Enabled);
			btnUp.Enabled = (lbTeamScoreCounters.SelectedIndex > 0);
			btnDown.Enabled = ((lbTeamScoreCounters.SelectedIndex >= 0)&&(lbTeamScoreCounters.SelectedIndex < lbTeamScoreCounters.Items.Count-1));
		}

		private void SelectedCounterChanged(object sender, EventArgs e)
		{
			TeamScoreCounters.Counter c = lbTeamScoreCounters.SelectedItem as TeamScoreCounters.Counter;
			if (c != null)
			{
				tbName.Text = c.Name;
				nudResults.Value = c.Results;
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


	
		public static bool EditTeamScoreCounters(ref TeamScoreCounters tsc)
		{
			TeamScoreCountersDialog tscd = new TeamScoreCountersDialog(tsc);

			if (tscd.ShowDialog() == DialogResult.OK)
			{
				tsc = (TeamScoreCounters) tscd.TeamScoreCounters.Clone();

				return true;
			}

			return false;
		}

		public static object ValueSelector(Sport.UI.Controls.ButtonBox buttonBox, object value)
		{
			TeamScoreCounters tsc = value as TeamScoreCounters;
			if (EditTeamScoreCounters(ref tsc))
				return tsc;
			return value;
		}

		private void tbName_TextChanged(object sender, System.EventArgs e)
		{
			SetButtonState();
		}

		private void nudResults_TextChanged(object sender, System.EventArgs e)
		{
			SetButtonState();
		}

		private void btnDown_Click(object sender, System.EventArgs e)
		{
			int selIndex=lbTeamScoreCounters.SelectedIndex;
			if ((selIndex < 0)||(selIndex >= lbTeamScoreCounters.Items.Count-1))
				return;
			SwitchCounters(selIndex, selIndex+1);
			SetCounters();
			lbTeamScoreCounters.SelectedIndex = selIndex+1;
		}

		private void btnUp_Click(object sender, System.EventArgs e)
		{
			int selIndex=lbTeamScoreCounters.SelectedIndex;
			if (selIndex <= 0)
				return;
			SwitchCounters(selIndex, selIndex-1);
			SetCounters();
			lbTeamScoreCounters.SelectedIndex = selIndex-1;
		}

		private void SwitchCounters(int index1, int index2)
		{
			TeamScoreCounters.Counter c = _teamScoreCounters.Counters[index1];
			_teamScoreCounters.Counters[index1] = _teamScoreCounters.Counters[index2];
			_teamScoreCounters.Counters[index2] = c;
		}
	}
}
