using System;
using Sport.Rulesets.Rules;
using Sport.Rulesets;
using System.Drawing;
using System.Windows.Forms;

namespace Sport.Producer.UI.Rules
{
	/// <summary>
	/// Summary description for TeamRankingDialog.
	/// </summary>
	public class TeamRankingDialog : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private System.Collections.ArrayList methods;
		public static Rule CurrentRule=null;

		public TeamRankingDialog(TeamRanking teamRanking)
		{
			if (teamRanking == null)
				_teamRanking = new TeamRanking();
			else
				_teamRanking = (TeamRanking) teamRanking.Clone();
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			methods = new System.Collections.ArrayList();

			string pointsName="";
			if (TeamRankingDialog.CurrentRule != null)
			{
				int sportID=TeamRankingDialog.CurrentRule.Ruleset.Sport;
				Sport.Entities.Sport sport=
					new  Sport.Entities.Sport(sportID);
				pointsName = sport.PointsName;
			}
			
			foreach (RankingMethodValue methodValue in Enum.GetValues(typeof(RankingMethodValue)))
			{
				methods.Add(new RankingMethod(methodValue, true, pointsName));
				methods.Add(new RankingMethod(methodValue, false, pointsName));
			}

			SetMethods();
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
			this.cbRankingMethod = new System.Windows.Forms.ComboBox();
			this.btnRemove = new System.Windows.Forms.Button();
			this.tbOk = new System.Windows.Forms.Button();
			this.tbCancel = new System.Windows.Forms.Button();
			this.btnAdd = new System.Windows.Forms.Button();
			this.lbRankingMethods = new System.Windows.Forms.ListBox();
			this.panel.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel
			// 
			this.panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel.Controls.Add(this.btnDown);
			this.panel.Controls.Add(this.btnUp);
			this.panel.Controls.Add(this.cbRankingMethod);
			this.panel.Controls.Add(this.btnRemove);
			this.panel.Controls.Add(this.tbOk);
			this.panel.Controls.Add(this.tbCancel);
			this.panel.Controls.Add(this.btnAdd);
			this.panel.Controls.Add(this.lbRankingMethods);
			this.panel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel.Location = new System.Drawing.Point(0, 0);
			this.panel.Name = "panel";
			this.panel.Size = new System.Drawing.Size(296, 235);
			this.panel.TabIndex = 7;
			this.panel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelMouseUp);
			this.panel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelMouseMove);
			this.panel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelMouseDown);
			// 
			// btnDown
			// 
			this.btnDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnDown.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnDown.Location = new System.Drawing.Point(207, 208);
			this.btnDown.Name = "btnDown";
			this.btnDown.Size = new System.Drawing.Size(40, 20);
			this.btnDown.TabIndex = 14;
			this.btnDown.Text = "הורד";
			this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
			// 
			// btnUp
			// 
			this.btnUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnUp.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnUp.Location = new System.Drawing.Point(248, 208);
			this.btnUp.Name = "btnUp";
			this.btnUp.Size = new System.Drawing.Size(40, 20);
			this.btnUp.TabIndex = 13;
			this.btnUp.Text = "עלה";
			this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
			// 
			// cbRankingMethod
			// 
			this.cbRankingMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbRankingMethod.Location = new System.Drawing.Point(50, 8);
			this.cbRankingMethod.Name = "cbRankingMethod";
			this.cbRankingMethod.Size = new System.Drawing.Size(238, 21);
			this.cbRankingMethod.Sorted = true;
			this.cbRankingMethod.TabIndex = 12;
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
			this.tbOk.Location = new System.Drawing.Point(8, 208);
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
			this.tbCancel.Location = new System.Drawing.Point(80, 208);
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
			// lbRankingMethods
			// 
			this.lbRankingMethods.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lbRankingMethods.Location = new System.Drawing.Point(8, 32);
			this.lbRankingMethods.Name = "lbRankingMethods";
			this.lbRankingMethods.Size = new System.Drawing.Size(280, 171);
			this.lbRankingMethods.TabIndex = 9;
			this.lbRankingMethods.SelectedIndexChanged += new System.EventHandler(this.SelectedBreakerChanged);
			// 
			// TeamRankingDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(296, 235);
			this.ControlBox = false;
			this.Controls.Add(this.panel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "TeamRankingDialog";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "בחר טווח";
			this.panel.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private System.Windows.Forms.Button btnRemove;
		private System.Windows.Forms.Button tbOk;
		private System.Windows.Forms.Button tbCancel;
		private System.Windows.Forms.Button btnAdd;
		private System.Windows.Forms.Button btnDown;
		private System.Windows.Forms.Button btnUp;
		private System.Windows.Forms.Panel panel;
		private System.Windows.Forms.ComboBox cbRankingMethod;
		private System.Windows.Forms.ListBox lbRankingMethods;


		private TeamRanking _teamRanking;
		public TeamRanking TeamRanking
		{
			get { return _teamRanking; }
		}

		private void SetMethods()
		{
			lbRankingMethods.Items.Clear();
			
			foreach (RankingMethod method in _teamRanking.RankingMethods)
			{
				lbRankingMethods.Items.Add(method);
			}

			cbRankingMethod.Items.Clear();

			foreach (RankingMethod method in methods)
			{
				if (!_teamRanking.RankingMethods.Contains(method))
				{
					cbRankingMethod.Items.Add(method);
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
			RankingMethod method = cbRankingMethod.SelectedItem as RankingMethod;

			if (method != null)
			{
				_teamRanking.RankingMethods.Add(method);
				cbRankingMethod.Items.Remove(method);
				lbRankingMethods.Items.Add(method);
			}

			SetButtonState();
		}

		private void btnRemove_Click(object sender, System.EventArgs e)
		{
			RankingMethod method = lbRankingMethods.SelectedItem as RankingMethod;

			if (method != null)
			{
				_teamRanking.RankingMethods.Remove(method);
				cbRankingMethod.Items.Add(method);
				lbRankingMethods.Items.Remove(method);
			}

			SetButtonState();
		}

		private void btnDown_Click(object sender, EventArgs e)
		{
			int n = lbRankingMethods.SelectedIndex;
			if (n >= 0 && n < lbRankingMethods.Items.Count - 1)
			{
				RankingMethod rm = _teamRanking.RankingMethods[n + 1];
				_teamRanking.RankingMethods[n + 1] = _teamRanking.RankingMethods[n];
				_teamRanking.RankingMethods[n] = rm;

				SetMethods();

				lbRankingMethods.SelectedIndex = n + 1;
				SetButtonState();
			}
		}

		private void btnUp_Click(object sender, EventArgs e)
		{
			int n = lbRankingMethods.SelectedIndex;
			if (n > 0)
			{
				RankingMethod rm = _teamRanking.RankingMethods[n - 1];
				_teamRanking.RankingMethods[n - 1] = _teamRanking.RankingMethods[n];
				_teamRanking.RankingMethods[n] = rm;

				SetMethods();

				lbRankingMethods.SelectedIndex = n - 1;
				SetButtonState();
			}
		}

		private void SetButtonState()
		{
			int n = lbRankingMethods.SelectedIndex;
			if (n == -1)
			{
				btnUp.Enabled = false;
				btnDown.Enabled = false;
				btnRemove.Enabled = false;
			}
			else
			{
				btnRemove.Enabled = true;
				if (n > 0)
					btnUp.Enabled = true;
				if (n < lbRankingMethods.Items.Count - 1)
					btnDown.Enabled = true;
			}		
		}

		private void SelectedBreakerChanged(object sender, EventArgs e)
		{
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


	
		public static bool EditTeamRanking(ref TeamRanking tr)
		{
			TeamRankingDialog trd = new TeamRankingDialog(tr);

			if (trd.ShowDialog() == DialogResult.OK)
			{
				tr = (TeamRanking) trd.TeamRanking.Clone();

				return true;
			}

			return false;
		}

		public static object ValueSelector(Sport.UI.Controls.ButtonBox buttonBox, object value)
		{
			TeamRanking tr = value as TeamRanking;
			if (EditTeamRanking(ref tr))
				return tr;
			return value;
		}
	}

	public class TeamRankingRuleEditor : RuleTypeEditor
	{
		public TeamRankingRuleEditor()
		{
		}

		private Rule _rule;
		private RuleScope _scope;

		public override Control Edit(Rule rule, RuleScope scope)
		{
			_rule = rule;
			_scope = scope;

			Sport.UI.Controls.ButtonBox bb = new Sport.UI.Controls.ButtonBox();
			bb.Value = rule[scope];
			TeamRankingDialog.CurrentRule = rule;
			bb.ValueSelector = new Sport.UI.Controls.ButtonBox.SelectValue(TeamRankingDialog.ValueSelector);
			bb.ValueChanged += new EventHandler(TeamRankingChanged);

			return bb;
		}

		public override void EndEdit()
		{
			_rule = null;
		}

		private void TeamRankingChanged(object sender, EventArgs e)
		{
			Sport.UI.Controls.ButtonBox bb = sender as Sport.UI.Controls.ButtonBox;
			if (bb != null && _rule != null)
			{
				_rule.Set(_scope, bb.Value);
			}
		}
	}
}
