using System;

namespace Sportsman.Producer
{
	public class MatchPartsResultForm : System.Windows.Forms.Form
	{
		private int titleHeight;
		private int partWidth;
		private int partHeight = 22;
		private int gameWidth;
		private int gameHeight;

		private int baseWidth;
		private System.Windows.Forms.Panel panelGames;
		private System.Windows.Forms.Panel panelTitles;
		private System.Windows.Forms.Panel panelParts;
		private int baseHeight;
		private Sport.Rulesets.Rules.GameStructure _gameStructure;
		private Sport.Championships.Match _match;

		private Sport.Championships.GameResult _gameResult;
		public Sport.Championships.GameResult GameResult
		{
			get { return _gameResult; }
		}
		private double teamAScore;
		public double TeamAScore
		{
			get { return teamAScore; }
		}

		private double teamBScore;
		public double TeamBScore
		{
			get { return teamBScore; }
		}

		private Sport.UI.Controls.TextControl resultControl;

		private void SetTitle()
		{
			string strTeamName_A = _match.GetTeamAName();
			string strTeamName_B = _match.GetTeamBName();
			
			//labelTitle.Text = _match.GroupTeamA.Name + " (" + teamAScore.ToString() + ") -\n" + 
			//	_match.GroupTeamB.Name + " (" + teamBScore.ToString() + ")";
			
			labelTitle.Text = strTeamName_A + " (" + teamAScore.ToString() + ") -\n" + 
				strTeamName_B + " (" + teamBScore.ToString() + ")";
		}

		private void SetGameTitle(int game, int teamASets, int teamBSets)
		{
			foreach (System.Windows.Forms.Label label in panelGames.Controls)
			{
				if ((int) label.Tag == game)
				{
					label.Text = "משחקון " + (game + 1).ToString() + 
						"\n(" + teamASets + " - " + teamBSets + ")";
					return ;
				}
			}
		}

		private void CalculateMatchResult()
		{
			teamAScore = 0;
			teamBScore = 0;

			if (_gameStructure.GameType == Sport.Rulesets.Rules.GameType.Points)
			{
				if (_gameStructure.GameExtension > 1)
				{
					for (int game = 0; game < _gameResult.Games; game++)
					{
						int teamASets = 0;
						int teamBSets = 0;
						for (int part = 0; part < _gameResult[game].Count; part++)
						{
							int[] result = _gameResult[game][part];
							if (result[0] > result[1])
								teamASets++;
							else if (result[0] < result[1])
								teamBSets++;
						}

						SetGameTitle(game, teamASets, teamBSets);

						if (teamASets > teamBSets)
							teamAScore++;
						else if (teamASets < teamBSets)
							teamBScore++;
					}
				}
				else
				{
					for (int part = 0; part < _gameResult[0].Count; part++)
					{
						int[] result = _gameResult[0][part];
						if (result[0] > result[1])
							teamAScore++;
						else if (result[0] < result[1])
							teamBScore++;
					}
				}
			}
			else
			{
				for (int part = 0; part < _gameResult[0].Count; part++)
				{
					teamAScore += _gameResult[0][part][0];
					teamBScore += _gameResult[0][part][1];
				}
			}

			SetTitle();
		}


		public MatchPartsResultForm(Sport.Championships.Match match, Sport.Championships.GameResult gameResult)
		{
			_match = match;
			teamAScore = _match.TeamAScore;
			teamBScore = _match.TeamBScore;

			_gameResult = (Sport.Championships.GameResult) gameResult.Clone();

			_gameStructure = null;
			object rule=null;
			if (Sport.Core.Session.Connected)
			{
				rule = _match.Cycle.Round.Group.Phase.Championship.ChampionshipCategory.GetRule(
					typeof(Sport.Rulesets.Rules.GameStructure));
			}
			else
			{
				rule = Sport.Rulesets.Ruleset.LoadOfflineRule(
					typeof(Sport.Rulesets.Rules.GameStructureRule), 
					_match.Cycle.Round.Group.Phase.Championship.CategoryID, -1);
			}
			if (rule != null)
				_gameStructure = (Sport.Rulesets.Rules.GameStructure) rule;
			
			InitializeComponent();

			resultControl = new Sport.UI.Controls.TextControl();
			resultControl.Controller = new Sport.UI.Controls.NumberController(0, 1000, 3, 0);
			resultControl.Visible = false;
			resultControl.TabStop = false;
			resultControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			resultControl.ShowSpin = true;
			resultControl.TextChanged += new EventHandler(ResultControlTextChanged);
			resultControl.KeyDown += new System.Windows.Forms.KeyEventHandler(resultControl_KeyDown);

			labelParts.Text = _gameStructure.GameType == Sport.Rulesets.Rules.GameType.Duration ?
				"חלק" : "מערכה";

			titleHeight = labelParts.Height;
			gameWidth = labelParts.Width;
			gameHeight = partHeight * 2;
			partWidth = panelTitles.Width;
			baseWidth = Width - partWidth + 1;
			baseHeight = Height - panelGames.Height;

			SetParts();

			CalculateMatchResult();

			Edit(0, 0, 0);
		}

		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOk;
		private System.Windows.Forms.Label labelTitle;
		private System.Windows.Forms.Label labelParts;
		private System.Windows.Forms.Panel panel;
	
		private void InitializeComponent()
		{
			this.panel = new System.Windows.Forms.Panel();
			this.panelParts = new System.Windows.Forms.Panel();
			this.panelTitles = new System.Windows.Forms.Panel();
			this.panelGames = new System.Windows.Forms.Panel();
			this.labelParts = new System.Windows.Forms.Label();
			this.labelTitle = new System.Windows.Forms.Label();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOk = new System.Windows.Forms.Button();
			this.panel.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel
			// 
			this.panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel.Controls.Add(this.panelParts);
			this.panel.Controls.Add(this.panelTitles);
			this.panel.Controls.Add(this.panelGames);
			this.panel.Controls.Add(this.labelParts);
			this.panel.Controls.Add(this.labelTitle);
			this.panel.Controls.Add(this.btnCancel);
			this.panel.Controls.Add(this.btnOk);
			this.panel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel.Location = new System.Drawing.Point(0, 0);
			this.panel.Name = "panel";
			this.panel.Size = new System.Drawing.Size(128, 104);
			this.panel.TabIndex = 0;
			// 
			// panelParts
			// 
			this.panelParts.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.panelParts.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panelParts.Location = new System.Drawing.Point(6, 55);
			this.panelParts.Name = "panelParts";
			this.panelParts.Size = new System.Drawing.Size(44, 16);
			this.panelParts.TabIndex = 13;
			// 
			// panelTitles
			// 
			this.panelTitles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.panelTitles.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panelTitles.Location = new System.Drawing.Point(6, 39);
			this.panelTitles.Name = "panelTitles";
			this.panelTitles.Size = new System.Drawing.Size(44, 17);
			this.panelTitles.TabIndex = 12;
			// 
			// panelGames
			// 
			this.panelGames.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.panelGames.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panelGames.Location = new System.Drawing.Point(49, 55);
			this.panelGames.Name = "panelGames";
			this.panelGames.Size = new System.Drawing.Size(72, 16);
			this.panelGames.TabIndex = 11;
			// 
			// labelParts
			// 
			this.labelParts.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelParts.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.labelParts.Location = new System.Drawing.Point(49, 39);
			this.labelParts.Name = "labelParts";
			this.labelParts.Size = new System.Drawing.Size(72, 17);
			this.labelParts.TabIndex = 10;
			this.labelParts.Text = "מערכה";
			// 
			// labelTitle
			// 
			this.labelTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.labelTitle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.labelTitle.Location = new System.Drawing.Point(6, 8);
			this.labelTitle.Name = "labelTitle";
			this.labelTitle.Size = new System.Drawing.Size(115, 32);
			this.labelTitle.TabIndex = 9;
			this.labelTitle.Text = "label1";
			this.labelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.labelTitle.MouseUp += new System.Windows.Forms.MouseEventHandler(this.labelTitle_MouseUp);
			this.labelTitle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.labelTitle_MouseMove);
			this.labelTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.labelTitle_MouseDown);
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnCancel.Location = new System.Drawing.Point(62, 76);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(54, 21);
			this.btnCancel.TabIndex = 8;
			this.btnCancel.TabStop = false;
			this.btnCancel.Text = "ביטול";
			// 
			// btnOk
			// 
			this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnOk.Location = new System.Drawing.Point(6, 76);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(54, 21);
			this.btnOk.TabIndex = 6;
			this.btnOk.TabStop = false;
			this.btnOk.Text = "אישור";
			// 
			// MatchPartsResultForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.ClientSize = new System.Drawing.Size(128, 104);
			this.Controls.Add(this.panel);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "MatchPartsResultForm";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.GotFocus += new EventHandler(MatchPartsResultForm_GotFocus);
			this.panel.ResumeLayout(false);
			this.ResumeLayout(false);

		}


		private System.Windows.Forms.Label CreateLabel(string text,
			int left, int top, int width, int height, object tag)
		{
			System.Windows.Forms.Label label = new System.Windows.Forms.Label();
			label.Text = text;
			label.Location = new System.Drawing.Point(left, top);
			label.Size = new System.Drawing.Size(width, height);
			label.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			label.Tag = tag;
			return label;
		}

		private System.Windows.Forms.Label CreatePart(int game, int part, int team,
			int left, int top, int width, int height)
		{
			System.Windows.Forms.Label label = CreateLabel(null,
				left, top, width, height, new int[] { game, part, team });
			label.Click += new EventHandler(GamePartClicked);
			return label;
		}

		private bool PartEnabled(int game, int part)
		{
			if (_gameStructure.GameType == Sport.Rulesets.Rules.GameType.Points)
			{
				if (game < _gameResult.Games)
				{
					return part <= _gameResult[game].Count;
				}
				else if (game == _gameResult.Games)
				{
					return part == 0;
				}
			}
			else
			{
				if (game == 0)
				{
					if (part < _gameStructure.SetPart)
						return true;
					if (part <= _gameResult[0].Count)
						return true;
				}
			}

			return false;
		}

		private void SetResults()
		{
			foreach (System.Windows.Forms.Control control in panelParts.Controls)
			{
				if (control is System.Windows.Forms.Label)
				{
					System.Windows.Forms.Label label = (System.Windows.Forms.Label) control;

					int[] p = (int[]) label.Tag;
					int game = p[0];
					int part = p[1];
					if (game < _gameResult.Games)
					{
						if (part < _gameResult[game].Count)
						{
							label.Text = _gameResult[game][part][p[2]].ToString();
						}
						else
						{
							label.Text = null;
						}
					}
					else
					{
						label.Text = null;
					}

					label.BackColor = PartEnabled(game, part) ? System.Drawing.SystemColors.Window :
						System.Drawing.SystemColors.Control;
				}
			}
		}

		private System.Windows.Forms.Label[,,] partLabels;

		private void SetParts()
		{
			System.Drawing.Point mid = new System.Drawing.Point(
				Left + (Width / 2), Top + (Height / 2));

			SuspendLayout();

			int parts = 1;
			int games = 1;

			if (_gameStructure.GameType == Sport.Rulesets.Rules.GameType.Duration)
			{
				parts = _gameResult[0].Count;
				if (parts < _gameStructure.SetPart)
				{
					parts = _gameStructure.SetPart;
				}
				if (parts < _gameStructure.GameExtension + _gameStructure.SetPart)
				{
					parts++; // Adding to enable setting of extensions result
				}
			}
			else
			{
				parts = _gameStructure.SetPart;
				games = _gameStructure.GameExtension;
			}

			Size = new System.Drawing.Size(
				baseWidth + (partWidth * parts),
				baseHeight + (games * gameHeight));

			panelGames.Controls.Clear();
			panelTitles.Controls.Clear();
			panelParts.Controls.Clear();

			// Adding titles
			int left = -1;
			for (int n = 0; n < parts; n++)
			{
				panelTitles.Controls.Add(
					CreateLabel((parts - n).ToString(), left, -1, partWidth + 1, titleHeight + 1, null));
				left += partWidth;
			}

			int top = -1;
			string s;
			for (int n = 0; n < games; n++)
			{
				s = _gameStructure.GameType == Sport.Rulesets.Rules.GameType.Duration ? 
					null : "משחקון " + (n + 1).ToString();
				panelGames.Controls.Add(
					CreateLabel(s, -1, top, gameWidth + 1, gameHeight + 1, n));
				top += gameHeight;
			}

			top = -1;
			partLabels = new System.Windows.Forms.Label[games, parts, 2]; 
			for (int g = 0; g < games; g++)
			{
				left = -1;
				for (int p = 0; p < parts; p++)
				{
					partLabels[g, parts - p - 1, 0] = CreatePart(g, parts - p - 1, 0, left, top, partWidth + 1, partHeight + 1);
					partLabels[g, parts - p - 1, 1] = CreatePart(g, parts - p - 1, 1, left, top + partHeight, partWidth + 1, partHeight + 1);
					panelParts.Controls.Add(partLabels[g, parts - p - 1, 0]);
					panelParts.Controls.Add(partLabels[g, parts - p - 1, 1]);

					left += partWidth;
				}

				top += partHeight * 2;
			}

			panelParts.Controls.Add(resultControl);

			SetResults();

			Location = new System.Drawing.Point(
				mid.X - (Width / 2), mid.Y - (Height / 2));

			ResumeLayout();
		}

		private System.Windows.Forms.Label editedLabel = null;

		private void GamePartClicked(object sender, EventArgs e)
		{
			System.Windows.Forms.Label label = sender as System.Windows.Forms.Label;
			int[] p = (int[]) label.Tag;
			Edit(p[0], p[1], p[2]);
		}

		private void Edit(int game, int part, int team)
		{
			if (PartEnabled(game, part))
			{
				System.Windows.Forms.Label label = partLabels[game, part, team];
				if (editedLabel != null)
					editedLabel.Visible = true;

				resultControl.Tag = null;

				bool reset = false;

				while (game >= _gameResult.Games)
				{
					_gameResult.AddGame();
					reset = true;
				}

				while (part >= _gameResult[game].Count)
				{
					_gameResult[game].Add(new int[] {0, 0});
					reset = true;
				}

				if (reset)
				{
					if (_gameStructure.GameType == Sport.Rulesets.Rules.GameType.Duration)
					{
						SetParts();
						label = partLabels[game, part, team];
					}
					else
					{
						SetResults();
					}
				}

				resultControl.Size = label.Size;
				resultControl.Location = label.Location;
				if (label.Text.Length == 0)
					resultControl.Value = 0;
				else
					resultControl.Value = Double.Parse(label.Text);
				label.Visible = false;

				resultControl.Tag = label.Tag;
				resultControl.Visible = true;
				resultControl.Focus();
				resultControl.SelectionStart = 0;
				resultControl.SelectionLength = resultControl.Text.Length;

				editedLabel = label;
			}
		}

		private object hotSpot;

		private void labelTitle_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			System.Drawing.Point pt = labelTitle.PointToScreen(new System.Drawing.Point(e.X, e.Y));
			hotSpot = new System.Drawing.Point(pt.X - Left, pt.Y - Top);
		}

		private void labelTitle_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (hotSpot != null)
			{
				System.Drawing.Point pt = labelTitle.PointToScreen(new System.Drawing.Point(e.X, e.Y));
				System.Drawing.Point hs = (System.Drawing.Point) hotSpot;

				Location = new System.Drawing.Point(pt.X - hs.X, pt.Y - hs.Y);
			}
		}

		private void labelTitle_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			hotSpot = null;
		}

		protected override void OnLostFocus(EventArgs e)
		{
			hotSpot = null;
			base.OnLostFocus (e);
		}

		private void ResultControlTextChanged(object sender, EventArgs e)
		{
			if (resultControl.Tag != null && resultControl.Value != null)
			{
				int[] p = (int[]) resultControl.Tag;
				int game = p[0];
				int part = p[1];
				if (game < _gameResult.Games)
				{
					_gameResult[game][part][p[2]] = (int) (double) resultControl.Value;

					SetResults();

					CalculateMatchResult();
				}
			}
		}

		private void resultControl_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (resultControl.Tag != null)
			{
				if (e.KeyCode == System.Windows.Forms.Keys.Delete)
				{
					int[] p = (int[]) resultControl.Tag;
					int game = p[0];
					int part = p[1];

					if (part == _gameResult[game].Count - 1)
					{
						_gameResult[game].RemoveAt(part);

						if (_gameResult[game].Count == 0 && game == _gameResult.Games - 1 &&
							game > 0)
						{
							_gameResult.GamesResults.RemoveAt(game);
						}

						resultControl.Tag = null;
						resultControl.Visible = false;

						if (editedLabel != null)
						{
							editedLabel.Visible = true;
							editedLabel = null;
						}
						
						if (_gameStructure.GameType == Sport.Rulesets.Rules.GameType.Duration)
							SetParts();
						else
							SetResults();
					}
					else
					{
						_gameResult[game][part] = new int[] { 0, 0 };
						SetResults();
					}
					CalculateMatchResult();

					if (part > 0)
						Edit(game, part - 1, 0);
					else if (game > 0)
						Edit(game - 1, 0, 0);
				}
				else if (e.KeyCode == System.Windows.Forms.Keys.Enter ||
					e.KeyCode == System.Windows.Forms.Keys.Tab)
				{
					int[] p = (int[]) resultControl.Tag;
					int game = p[0];
					int part = p[1];
					int team = p[2];

					if (team == 0 && !e.Control)
					{
						Edit(game, part, 1);
					}
					else
					{
						int parts = 1;
						int games = 1;

						if (_gameStructure.GameType == Sport.Rulesets.Rules.GameType.Duration)
						{
							parts = _gameResult[0].Count;
							if (parts < _gameStructure.SetPart)
							{
								parts = _gameStructure.SetPart;
							}
							if (parts < _gameStructure.GameExtension + _gameStructure.SetPart)
							{
								parts++; // Adding to enable setting of extensions result
							}
						}
						else
						{
							parts = _gameStructure.SetPart;
							games = _gameStructure.GameExtension;
						}

						if (e.Control)
						{
							if (game < games - 1)
							{
								Edit(game + 1, 0, 0);
							}
							else
							{
								DialogResult = System.Windows.Forms.DialogResult.OK;
								Close();
							}
						}
						else
						{
							if (part < parts - 1)
								Edit(game, part + 1, 0);
							else if (game < games - 1)
								Edit(game + 1, 0, 0);
							else
								btnOk.Focus();
						}
					}
				}
				else if (e.KeyCode == System.Windows.Forms.Keys.Escape)
				{
					DialogResult = System.Windows.Forms.DialogResult.Cancel;
					Close();
				}
			}
		}

		private void MatchPartsResultForm_GotFocus(object sender, EventArgs e)
		{
			if (resultControl.Visible)
				resultControl.Focus();
		}
	}
}
