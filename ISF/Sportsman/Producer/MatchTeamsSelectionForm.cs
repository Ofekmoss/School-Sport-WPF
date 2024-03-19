using System;

namespace Sportsman.Producer
{
	/// <summary>
	/// Summary description for MatchTeamsSelectionForm.
	/// </summary>
	public class MatchTeamsSelectionForm : System.Windows.Forms.Form
	{
		private readonly string GroupTeams = "בית";
		private readonly string WiningTeams = "מנצחת משחק";
		private readonly string LosingTeams = "מפסידת משחק";
		private readonly string WinningAndLosingTeams = "מנצחת או מפסידת משחק";
		private readonly string CustomTeams = "טקסט חופשי";
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.TextBox tbTeamA;
		private System.Windows.Forms.Label labelTeamA;
		private System.Windows.Forms.TextBox tbTeamB;
		private Sport.UI.Controls.TextControl tcResultB;
		private System.Windows.Forms.Label labelResultB;
		private System.Windows.Forms.ListBox lbTeams;
		private Sport.UI.Controls.ThemeButton tbSelectA;
		private Sport.UI.Controls.ThemeButton tbSelectB;
		private System.Windows.Forms.Label labelTeams;
		private System.Windows.Forms.ComboBox cbTeams;
		private System.Windows.Forms.Label labelTeamB;
		private bool _customTeamA = false;
		private bool _customTeamB = false;

		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(MatchTeamsSelectionForm));
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.tbTeamA = new System.Windows.Forms.TextBox();
			this.labelTeamA = new System.Windows.Forms.Label();
			this.tbTeamB = new System.Windows.Forms.TextBox();
			this.tcResultB = new Sport.UI.Controls.TextControl();
			this.labelResultB = new System.Windows.Forms.Label();
			this.labelTeamB = new System.Windows.Forms.Label();
			this.lbTeams = new System.Windows.Forms.ListBox();
			this.tbSelectA = new Sport.UI.Controls.ThemeButton();
			this.tbSelectB = new Sport.UI.Controls.ThemeButton();
			this.labelTeams = new System.Windows.Forms.Label();
			this.cbTeams = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnOK.Location = new System.Drawing.Point(8, 318);
			this.btnOK.Name = "btnOK";
			this.btnOK.TabIndex = 3;
			this.btnOK.TabStop = false;
			this.btnOK.Text = "אישור";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnCancel.Location = new System.Drawing.Point(88, 318);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.TabIndex = 4;
			this.btnCancel.TabStop = false;
			this.btnCancel.Text = "ביטול";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// tbTeamA
			// 
			this.tbTeamA.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.tbTeamA.Location = new System.Drawing.Point(224, 288);
			this.tbTeamA.Name = "tbTeamA";
			this.tbTeamA.ReadOnly = true;
			this.tbTeamA.Size = new System.Drawing.Size(208, 21);
			this.tbTeamA.TabIndex = 28;
			this.tbTeamA.TabStop = false;
			this.tbTeamA.Text = "";
			this.tbTeamA.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbTeamA_KeyPress);
			// 
			// labelTeamA
			// 
			this.labelTeamA.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelTeamA.Location = new System.Drawing.Point(328, 272);
			this.labelTeamA.Name = "labelTeamA";
			this.labelTeamA.Size = new System.Drawing.Size(104, 16);
			this.labelTeamA.TabIndex = 31;
			this.labelTeamA.Text = "קבוצה א\':";
			// 
			// tbTeamB
			// 
			this.tbTeamB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.tbTeamB.Location = new System.Drawing.Point(8, 288);
			this.tbTeamB.Name = "tbTeamB";
			this.tbTeamB.ReadOnly = true;
			this.tbTeamB.Size = new System.Drawing.Size(208, 21);
			this.tbTeamB.TabIndex = 34;
			this.tbTeamB.TabStop = false;
			this.tbTeamB.Text = "";
			this.tbTeamB.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbTeamB_KeyPress);
			// 
			// tcResultB
			// 
			this.tcResultB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.tcResultB.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.tcResultB.Controller = null;
			this.tcResultB.Location = new System.Drawing.Point(-152, 24);
			this.tcResultB.Name = "tcResultB";
			this.tcResultB.ReadOnly = true;
			this.tcResultB.SelectionLength = 0;
			this.tcResultB.SelectionStart = 0;
			this.tcResultB.ShowSpin = true;
			this.tcResultB.Size = new System.Drawing.Size(77, 20);
			this.tcResultB.TabIndex = 35;
			this.tcResultB.Value = "";
			// 
			// labelResultB
			// 
			this.labelResultB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelResultB.Location = new System.Drawing.Point(-128, 8);
			this.labelResultB.Name = "labelResultB";
			this.labelResultB.Size = new System.Drawing.Size(53, 16);
			this.labelResultB.TabIndex = 37;
			this.labelResultB.Text = "תוצאה:";
			// 
			// labelTeamB
			// 
			this.labelTeamB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelTeamB.Location = new System.Drawing.Point(112, 272);
			this.labelTeamB.Name = "labelTeamB";
			this.labelTeamB.Size = new System.Drawing.Size(104, 16);
			this.labelTeamB.TabIndex = 36;
			this.labelTeamB.Text = "קבוצה ב\':";
			// 
			// lbTeams
			// 
			this.lbTeams.Location = new System.Drawing.Point(8, 36);
			this.lbTeams.Name = "lbTeams";
			this.lbTeams.Size = new System.Drawing.Size(424, 225);
			this.lbTeams.TabIndex = 38;
			this.lbTeams.SelectedIndexChanged += new System.EventHandler(this.lbTeams_SelectedIndexChanged);
			// 
			// tbSelectA
			// 
			this.tbSelectA.Alignment = System.Drawing.StringAlignment.Center;
			this.tbSelectA.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.tbSelectA.AutoSize = true;
			this.tbSelectA.Enabled = false;
			this.tbSelectA.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbSelectA.Hue = 200F;
			this.tbSelectA.Image = ((System.Drawing.Image)(resources.GetObject("tbSelectA.Image")));
			this.tbSelectA.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbSelectA.ImageList = null;
			this.tbSelectA.ImageSize = new System.Drawing.Size(12, 12);
			this.tbSelectA.Location = new System.Drawing.Point(224, 268);
			this.tbSelectA.Name = "tbSelectA";
			this.tbSelectA.Saturation = 0.5F;
			this.tbSelectA.Size = new System.Drawing.Size(48, 17);
			this.tbSelectA.TabIndex = 39;
			this.tbSelectA.Text = "בחר";
			this.tbSelectA.Transparent = System.Drawing.Color.Black;
			this.tbSelectA.Click += new System.EventHandler(this.tbSelectA_Click);
			// 
			// tbSelectB
			// 
			this.tbSelectB.Alignment = System.Drawing.StringAlignment.Center;
			this.tbSelectB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.tbSelectB.AutoSize = true;
			this.tbSelectB.Enabled = false;
			this.tbSelectB.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbSelectB.Hue = 200F;
			this.tbSelectB.Image = ((System.Drawing.Image)(resources.GetObject("tbSelectB.Image")));
			this.tbSelectB.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbSelectB.ImageList = null;
			this.tbSelectB.ImageSize = new System.Drawing.Size(12, 12);
			this.tbSelectB.Location = new System.Drawing.Point(8, 268);
			this.tbSelectB.Name = "tbSelectB";
			this.tbSelectB.Saturation = 0.5F;
			this.tbSelectB.Size = new System.Drawing.Size(48, 17);
			this.tbSelectB.TabIndex = 40;
			this.tbSelectB.Text = "בחר";
			this.tbSelectB.Transparent = System.Drawing.Color.Black;
			this.tbSelectB.Click += new System.EventHandler(this.tbSelectB_Click);
			// 
			// labelTeams
			// 
			this.labelTeams.Location = new System.Drawing.Point(384, 16);
			this.labelTeams.Name = "labelTeams";
			this.labelTeams.Size = new System.Drawing.Size(48, 16);
			this.labelTeams.TabIndex = 41;
			this.labelTeams.Text = "קבוצות:";
			// 
			// cbTeams
			// 
			this.cbTeams.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbTeams.Location = new System.Drawing.Point(272, 8);
			this.cbTeams.Name = "cbTeams";
			this.cbTeams.Size = new System.Drawing.Size(112, 21);
			this.cbTeams.Sorted = true;
			this.cbTeams.TabIndex = 42;
			this.cbTeams.SelectedIndexChanged += new System.EventHandler(this.cbTeams_SelectedIndexChanged);
			// 
			// MatchTeamsSelectionForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.ClientSize = new System.Drawing.Size(440, 350);
			this.Controls.Add(this.cbTeams);
			this.Controls.Add(this.labelTeams);
			this.Controls.Add(this.tbSelectB);
			this.Controls.Add(this.tbSelectA);
			this.Controls.Add(this.tbTeamB);
			this.Controls.Add(this.tbTeamA);
			this.Controls.Add(this.lbTeams);
			this.Controls.Add(this.tcResultB);
			this.Controls.Add(this.labelResultB);
			this.Controls.Add(this.labelTeamB);
			this.Controls.Add(this.labelTeamA);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "MatchTeamsSelectionForm";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "בחירת קבוצות";
			this.Load += new System.EventHandler(this.MatchTeamsSelectionForm_Load);
			this.ResumeLayout(false);

		}


		private Sport.Championships.MatchGroup _group = null;
		private Sport.Championships.Match _match = null;
		public Sport.Championships.Match Match
		{
			get { return _match; }
		}

		public MatchTeamsSelectionForm(Sport.Championships.MatchGroup group, Sport.Championships.Match match)
		{
			_group = group;
			_match = match;

			InitializeComponent();

			cbTeams.Items.AddRange(new object[] { GroupTeams, WiningTeams, 
													LosingTeams, WinningAndLosingTeams, CustomTeams });

			cbTeams.SelectedItem = GroupTeams;

			if (_match != null)
			{
				tbSelectA.Tag = _match.GroupTeamA;
				tbSelectB.Tag = _match.GroupTeamB;

				int mn;
				bool w;

				if (tbSelectA.Tag == null && _match.RelativeTeamA != 0)
				{
					mn = _match.RelativeTeamA;
					if (mn < 0)
					{
						w = false;
						mn = -mn;
					}
					else
					{
						w = true;
					}
					tbSelectA.Tag = new RelativeTeam(_group.GetMatchByNumber(mn), w);
				}
				if (tbSelectB.Tag == null && _match.RelativeTeamB != 0)
				{
					mn = _match.RelativeTeamB;
					if (mn < 0)
					{
						w = false;
						mn = -mn;
					}
					else
					{
						w = true;
					}
					tbSelectB.Tag = new RelativeTeam(_group.GetMatchByNumber(mn), w);
				}
			}

			SetTeamNames();

			BuildTeamsList();
		}

		private void SetTeamNames()
		{
			tbTeamA.Text = tbSelectA.Tag == null ? null : tbSelectA.Tag.ToString();
			tbTeamB.Text = tbSelectB.Tag == null ? null : tbSelectB.Tag.ToString();
			if (_match != null)
			{
				if ((_match.CustomTeamA != null) && (_match.CustomTeamA.Length > 0))
					tbTeamA.Text = _match.CustomTeamA;
				if ((_match.CustomTeamB != null) && (_match.CustomTeamB.Length > 0))
					tbTeamB.Text = _match.CustomTeamB;
			}
		}

		private class RelativeTeam
		{
			private Sport.Championships.Match _match;
			public Sport.Championships.Match Match
			{
				get { return _match; }
			}

			private bool _winner;
			public bool Winner
			{
				get { return _winner; }
			}

			public RelativeTeam(Sport.Championships.Match match, bool winner)
			{
				_winner = winner;
				_match = match;
			}

			public override int GetHashCode()
			{
				return base.GetHashCode();
			}

			public override bool Equals(object obj)
			{
				RelativeTeam rt = obj as RelativeTeam;
				if (rt == null)
					return false;

				return rt._winner == _winner &&
					rt._match.Equals(_match);
			}


			public override string ToString()
			{
				string retVal = (_winner ? "מנצחת" : "מפסידת");
				retVal += " משחק";
				if (_match != null)
					retVal += " " + _match.Number.ToString();
				return retVal;
			}
		}

		private void SetButtons()
		{
			object item = lbTeams.SelectedItem;

			bool enable = item != null &&
				!item.Equals(tbSelectA.Tag) &&
				!item.Equals(tbSelectB.Tag);

			tbSelectA.Enabled = enable;
			tbSelectB.Enabled = enable;
		}

		private void CreateMatch()
		{
			if (_group == null || _group.Phase == null || _group.Phase.Championship == null)
				return;

			if (_match == null)
				_match = new Sport.Championships.Match(
					_group.Phase.Championship.GetMaxMatchNumber() + 1, -1, -1);
			else
				_match = new Sport.Championships.Match(-1, -1, _match);

			Sport.Championships.MatchTeam mt = tbSelectA.Tag as Sport.Championships.MatchTeam;
			RelativeTeam rt;
			if (mt != null)
			{
				_match.TeamA = mt.Index;
			}
			else
			{
				rt = tbSelectA.Tag as RelativeTeam;
				if (rt != null && rt.Match != null)
					_match.RelativeTeamA = rt.Match.Number * (rt.Winner ? 1 : -1);
			}

			mt = tbSelectB.Tag as Sport.Championships.MatchTeam;
			if (mt != null)
			{
				_match.TeamB = mt.Index;
			}
			else
			{
				rt = tbSelectB.Tag as RelativeTeam;
				if (rt != null)
					_match.RelativeTeamB = rt.Match.Number * (rt.Winner ? 1 : -1);
			}

			SetTeamNames();
			SetButtons();
		}

		private void BuildTeamsList()
		{
			string teams = cbTeams.SelectedItem as string;

			lbTeams.Items.Clear();

			tbTeamA.ReadOnly = (teams != CustomTeams);
			tbTeamB.ReadOnly = (teams != CustomTeams);

			_customTeamA = false;
			_customTeamB = false;

			if (teams == GroupTeams)
			{
				foreach (Sport.Championships.MatchTeam team in _group.Teams)
				{
					lbTeams.Items.Add(team);
				}
			}
			else
			{
				if (teams == WiningTeams || teams == LosingTeams || teams == WinningAndLosingTeams)
				{
					bool w = (teams == WiningTeams || teams == WinningAndLosingTeams);
					foreach (Sport.Championships.Round round in _group.Rounds)
						foreach (Sport.Championships.Cycle cycle in round.Cycles)
							foreach (Sport.Championships.Match match in cycle.Matches)
								if (!match.Equals(_match))
									lbTeams.Items.Add(new RelativeTeam(match, w));

					if (teams == WinningAndLosingTeams)
						foreach (Sport.Championships.Round round in _group.Rounds)
							foreach (Sport.Championships.Cycle cycle in round.Cycles)
								foreach (Sport.Championships.Match match in cycle.Matches)
									if (!match.Equals(_match))
										lbTeams.Items.Add(new RelativeTeam(match, false));
				}
				else
				{
					//tbTeamA.Text = "";
					//tbTeamB.Text = "";
					tbTeamA.Focus();
				}
			}
		}

		private void btnOK_Click(object sender, System.EventArgs e)
		{
			if (_match != null)
			{
				if (tbTeamA.Text.Length == 0)
					_customTeamA = false;
				if (tbTeamB.Text.Length == 0)
					_customTeamB = false;
				if (_customTeamA)
					tbSelectA.Tag = null;
				if (_customTeamB)
					tbSelectB.Tag = null;
				if (_customTeamA)
					_match.CustomTeamA = tbTeamA.Text;
				if (_customTeamB)
					_match.CustomTeamB = tbTeamB.Text;
				if ((_customTeamA) || (_customTeamB))
					CreateMatch();
			}
			DialogResult = System.Windows.Forms.DialogResult.OK;
			Close();
		}

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			_match = null;
			DialogResult = System.Windows.Forms.DialogResult.Cancel;
			Close();
		}

		private void cbTeams_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			BuildTeamsList();
		}

		private void lbTeams_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			SetButtons();
		}

		private void tbSelectA_Click(object sender, System.EventArgs e)
		{
			tbSelectA.Tag = lbTeams.SelectedItem;
			CreateMatch();
		}

		private void tbSelectB_Click(object sender, System.EventArgs e)
		{
			tbSelectB.Tag = lbTeams.SelectedItem;
			CreateMatch();
		}

		private void tbTeamB_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			_customTeamB = true;
		}

		private void MatchTeamsSelectionForm_Load(object sender, System.EventArgs e)
		{

		}

		private void tbTeamA_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			_customTeamA = true;
		}

	}
}

