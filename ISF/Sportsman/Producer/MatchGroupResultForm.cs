using System;

namespace Sportsman.Producer
{
	/// <summary>
	/// Summary description for MatchGroupResultForm.
	/// </summary>
	public class MatchGroupResultForm : System.Windows.Forms.Form,
		Sport.UI.Controls.IGridSource, Sport.UI.ICommandTarget
	{
		#region private members
		private Sport.UI.Controls.ThemeButton btnOK;
		private Sport.UI.Controls.ThemeButton btnCancel;
		private System.Windows.Forms.Label labelNumber;
		private Sport.UI.Controls.TextControl tcNumber;
		private System.Windows.Forms.TextBox tbTeamA;
		private Sport.UI.Controls.TextControl tcResultA;
		private System.Windows.Forms.Label labelTeamA;
		private System.Windows.Forms.Label labelResultA;
		private System.Windows.Forms.TextBox tbTeamB;
		private Sport.UI.Controls.TextControl tcResultB;
		private System.Windows.Forms.Label labelResultB;
		private System.Windows.Forms.Label labelTeamB;
		private Sport.UI.Controls.Grid gridResults;
		private Sport.Championships.MatchGroup _group = null;
		private int _round=-1;
		private int _cycle=-1;
		private bool enableParts = false;
		#endregion
		
		#region initialize component
		private void InitializeComponent()
		{
			this.btnOK = new Sport.UI.Controls.ThemeButton();
			this.btnCancel = new Sport.UI.Controls.ThemeButton();
			this.gridResults = new Sport.UI.Controls.Grid();
			this.tcNumber = new Sport.UI.Controls.TextControl();
			this.tbTeamA = new System.Windows.Forms.TextBox();
			this.tcResultA = new Sport.UI.Controls.TextControl();
			this.labelNumber = new System.Windows.Forms.Label();
			this.labelTeamA = new System.Windows.Forms.Label();
			this.labelResultA = new System.Windows.Forms.Label();
			this.tbTeamB = new System.Windows.Forms.TextBox();
			this.tcResultB = new Sport.UI.Controls.TextControl();
			this.labelResultB = new System.Windows.Forms.Label();
			this.labelTeamB = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnOK.Location = new System.Drawing.Point(8, 440);
			this.btnOK.Size = new System.Drawing.Size(75, 30);
			this.btnOK.Hue = 300;
			this.btnOK.Saturation = 0.1f;
			this.btnOK.Name = "btnOK";
			this.btnOK.TabStop = true;
			this.btnOK.TabIndex = 3;
			this.btnOK.Text = "אישור";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnCancel.Location = new System.Drawing.Point(88, 440);
			this.btnCancel.Size = new System.Drawing.Size(75, 30);
			this.btnCancel.Hue = 300;
			this.btnCancel.Saturation = 0.1f;
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.TabStop = true;
			this.btnCancel.TabIndex = 4;
			this.btnCancel.Text = "ביטול";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// gridResults
			// 
			this.gridResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.gridResults.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.gridResults.Editable = true;
			this.gridResults.GroupingStyle = Sport.UI.Controls.GridGroupingStyle.Grid;
			this.gridResults.HorizontalLines = true;
			this.gridResults.Location = new System.Drawing.Point(8, 56);
			this.gridResults.Name = "gridResults";
			this.gridResults.SelectedIndex = -1;
			this.gridResults.SelectionMode = System.Windows.Forms.SelectionMode.One;
			this.gridResults.ShowCheckBoxes = false;
			this.gridResults.ShowRowNumber = false;
			this.gridResults.Size = new System.Drawing.Size(582, 376);
			this.gridResults.TabIndex = 26;
			this.gridResults.TabStop = false;
			this.gridResults.VerticalLines = true;
			// 
			// tcNumber
			// 
			this.tcNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.tcNumber.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.tcNumber.Controller = null;
			this.tcNumber.Location = new System.Drawing.Point(536, 24);
			this.tcNumber.Name = "tcNumber";
			this.tcNumber.ReadOnly = false;
			this.tcNumber.SelectionLength = 0;
			this.tcNumber.SelectionStart = 0;
			this.tcNumber.ShowSpin = false;
			this.tcNumber.Size = new System.Drawing.Size(56, 21);
			this.tcNumber.TabIndex = 27;
			this.tcNumber.Value = "";
			// 
			// tbTeamA
			// 
			this.tbTeamA.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.tbTeamA.Location = new System.Drawing.Point(352, 24);
			this.tbTeamA.Name = "tbTeamA";
			this.tbTeamA.ReadOnly = true;
			this.tbTeamA.Size = new System.Drawing.Size(176, 21);
			this.tbTeamA.TabIndex = 28;
			this.tbTeamA.TabStop = false;
			this.tbTeamA.Text = "";
			// 
			// tcResultA
			// 
			this.tcResultA.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.tcResultA.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.tcResultA.Location = new System.Drawing.Point(272, 24);
			this.tcResultA.Name = "tcResultA";
			this.tcResultA.ReadOnly = true;
			this.tcResultA.SelectionLength = 0;
			this.tcResultA.SelectionStart = 0;
			this.tcResultA.ShowSpin = true;
			this.tcResultA.Size = new System.Drawing.Size(72, 20);
			this.tcResultA.TabIndex = 29;
			// 
			// labelNumber
			// 
			this.labelNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelNumber.Location = new System.Drawing.Point(552, 8);
			this.labelNumber.Name = "labelNumber";
			this.labelNumber.Size = new System.Drawing.Size(40, 16);
			this.labelNumber.TabIndex = 30;
			this.labelNumber.Text = "מספר:";
			// 
			// labelTeamA
			// 
			this.labelTeamA.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelTeamA.Location = new System.Drawing.Point(424, 8);
			this.labelTeamA.Name = "labelTeamA";
			this.labelTeamA.Size = new System.Drawing.Size(104, 16);
			this.labelTeamA.TabIndex = 31;
			this.labelTeamA.Text = "קבוצה א':";
			// 
			// labelResultA
			// 
			this.labelResultA.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelResultA.Location = new System.Drawing.Point(296, 8);
			this.labelResultA.Name = "labelResultA";
			this.labelResultA.Size = new System.Drawing.Size(48, 16);
			this.labelResultA.TabIndex = 33;
			this.labelResultA.Text = "תוצאה:";
			// 
			// tbTeamB
			// 
			this.tbTeamB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.tbTeamB.Location = new System.Drawing.Point(88, 24);
			this.tbTeamB.Name = "tbTeamB";
			this.tbTeamB.ReadOnly = true;
			this.tbTeamB.Size = new System.Drawing.Size(176, 21);
			this.tbTeamB.TabIndex = 34;
			this.tbTeamB.TabStop = false;
			this.tbTeamB.Text = "";
			// 
			// tcResultB
			// 
			this.tcResultB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.tcResultB.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.tcResultB.Location = new System.Drawing.Point(8, 24);
			this.tcResultB.Name = "tcResultB";
			this.tcResultB.ReadOnly = true;
			this.tcResultB.SelectionLength = 0;
			this.tcResultB.SelectionStart = 0;
			this.tcResultB.ShowSpin = true;
			this.tcResultB.Size = new System.Drawing.Size(72, 20);
			this.tcResultB.TabIndex = 35;
			// 
			// labelResultB
			// 
			this.labelResultB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelResultB.Location = new System.Drawing.Point(32, 8);
			this.labelResultB.Name = "labelResultB";
			this.labelResultB.Size = new System.Drawing.Size(48, 16);
			this.labelResultB.TabIndex = 37;
			this.labelResultB.Text = "תוצאה:";
			// 
			// labelTeamB
			// 
			this.labelTeamB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelTeamB.Location = new System.Drawing.Point(160, 8);
			this.labelTeamB.Name = "labelTeamB";
			this.labelTeamB.Size = new System.Drawing.Size(104, 16);
			this.labelTeamB.TabIndex = 36;
			this.labelTeamB.Text = "קבוצה ב':";
			// 
			// MatchGroupResultForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.ClientSize = new System.Drawing.Size(600, 472);
			this.Controls.Add(this.tbTeamB);
			this.Controls.Add(this.tcResultB);
			this.Controls.Add(this.labelResultB);
			this.Controls.Add(this.labelTeamB);
			this.Controls.Add(this.tbTeamA);
			this.Controls.Add(this.tcResultA);
			this.Controls.Add(this.labelResultA);
			this.Controls.Add(this.labelTeamA);
			this.Controls.Add(this.labelNumber);
			this.Controls.Add(this.tcNumber);
			this.Controls.Add(this.gridResults);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Name = "MatchGroupResultForm";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "הכנסת תוצאות";
			this.ResumeLayout(false);

		}
		#endregion
		
		public MatchGroupResultForm(Sport.Championships.MatchGroup group, 
			int round, int cycle)
		{
			_group = group;
			_round = round;
			_cycle = cycle;
			
			object rule=null;
			if (Sport.Core.Session.Connected)
			{
				rule = group.Phase.Championship.ChampionshipCategory.GetRule(
					typeof(Sport.Rulesets.Rules.GameStructure));
			}
			else
			{
				rule = Sport.Rulesets.Ruleset.LoadOfflineRule(
					typeof(Sport.Rulesets.Rules.GameStructureRule), 
					group.Phase.Championship.CategoryID, -1);
			}
			enableParts = (rule != null);
			
			InitializeComponent();
			
			tcNumber.Controller = new Sport.UI.Controls.NumberController(0, 9999, 4, 0);
			tcNumber.LostFocus += new EventHandler(tcNumber_LostFocus);
			tcNumber.KeyPress += new System.Windows.Forms.KeyPressEventHandler(tcNumber_KeyPress);
			tcNumber.TextChanged += new EventHandler(tcNumber_TextChanged);
			tcNumber.GotFocus += new EventHandler(tcNumber_GotFocus);

			tcResultA.Controller = new Sport.UI.Controls.NumberController(0, 1000, 4, 0);
			tcResultA.KeyPress += new System.Windows.Forms.KeyPressEventHandler(tcResultA_KeyPress);
			tcResultA.GotFocus += new EventHandler(tcResultA_GotFocus);
			tcResultA.LostFocus += new EventHandler(tcResultA_LostFocus);

			tcResultB.Controller = new Sport.UI.Controls.NumberController(0, 1000, 4, 0);
			tcResultB.KeyPress += new System.Windows.Forms.KeyPressEventHandler(tcResultB_KeyPress);
			tcResultB.GotFocus += new EventHandler(tcResultB_GotFocus);
			tcResultB.LostFocus += new EventHandler(tcResultB_LostFocus);

			gridResults.GroupingStyle = Sport.UI.Controls.GridGroupingStyle.List;
			gridResults.Groups.Clear();
			gridResults.Groups.Add();
			gridResults.Groups[0].Columns.Add(0, "סיבוב - מחזור", 1);
			gridResults.Groups[1].Columns.Add(0, "מספר", 20);
			gridResults.Groups[1].Columns.Add(1, "קבוצה א'", 40);
			gridResults.Groups[1].Columns.Add(2, "קבוצה ב'", 40);
			gridResults.Groups[1].Columns.Add(3, "תוצאה", 40);

			ReadResults();

			gridResults.Source = this;

			gridResults.SelectionChanged += new EventHandler(gridResults_SelectionChanged);
			gridResults.DoubleClick += new EventHandler(gridResults_DoubleClick);
			
			string strCaption="הכנסת תוצאות - " + _group.Name;
			if (_round >= 0)
			{
				strCaption += " "+_group.Rounds[_round].Name;
				if (_cycle >= 0)
					strCaption += " "+_group.Rounds[_round].Cycles[_cycle].Name;
			}
			Text = strCaption;
		}

		private void btnOK_Click(object sender, System.EventArgs e)
		{
			if (SetResults())
			{
				DialogResult = System.Windows.Forms.DialogResult.OK;
				Close();
			}
			else
			{
				Sport.UI.MessageBox.Show("כשלון בשמירת תוצאות");
			}
		}

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			DialogResult = System.Windows.Forms.DialogResult.Cancel;
			Close();
		}

		private void tcNumber_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			if (e.KeyChar == (char) System.Windows.Forms.Keys.Enter)
			{
				if (tcNumber.Text.Length != 0)
				{
					int matchNumber = (int) (double) tcNumber.Value;

					if (SelectMatch(matchNumber))
					{
						tcResultA.Focus();
					}
					else
					{
						tcNumber.SelectionStart = 0;
						tcNumber.SelectionLength = tcNumber.Text.Length;
						Sport.UI.MessageBox.Show("מספר משחק לא קיים");
					}
				}
			}
		}

		private void tcNumber_LostFocus(object sender, EventArgs e)
		{
			if (selectedRow == -1)
			{
				if (tcNumber.Text.Length != 0)
				{
					SelectMatch((int) (double) tcNumber.Value);
				}
			}
		}

		private void tcNumber_GotFocus(object sender, EventArgs e)
		{
			tcNumber.SelectionStart = 0;
			tcNumber.SelectionLength = tcNumber.Text.Length;
		}

		private void tcNumber_TextChanged(object sender, EventArgs e)
		{
			if (!selectingMatch)
			{
				selectedRow = -1;
				tbTeamA.Text = null;
				tbTeamB.Text = null;
				tcResultA.Value = null;
				tcResultB.Value = null;
				tcResultA.ReadOnly = true;
				tcResultB.ReadOnly = true;
			}
		}

		private void tcResultA_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			if (e.KeyChar == (char) System.Windows.Forms.Keys.Enter)
			{
				tcResultB.Focus();
			}
			else if (e.KeyChar == (char) System.Windows.Forms.Keys.LineFeed)
			{
				EditGameResult();
			}
		}

		private void tcResultA_GotFocus(object sender, EventArgs e)
		{
			tcResultA.SelectionStart = 0;
			tcResultA.SelectionLength = tcResultA.Text.Length;
		}

		private void tcResultA_LostFocus(object sender, EventArgs e)
		{
			if (selectedRow != -1)
			{
				object val = tcResultA.Value;
				SetMatchResultA(val == null ? -1 : (double) val);
			}
		}

		private void tcResultB_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			if (e.KeyChar == (char) System.Windows.Forms.Keys.Enter)
			{
				int row = selectedRow + 1;
				while (row < results.Count && !(results[row] is MatchRow))
				{
					if (!gridResults.ExpandedRows[row])
						gridResults.ExpandedRows[row] = true;
					row++;
				}
				if (row == results.Count)
				{
					btnOK.Focus();
				}
				else
				{
					tcNumber.Focus();
					gridResults.SelectRow(row);
				}
			}
			else if (e.KeyChar == (char) System.Windows.Forms.Keys.LineFeed)
			{
				EditGameResult();
			}
		}

		private void tcResultB_GotFocus(object sender, EventArgs e)
		{
			tcResultB.SelectionStart = 0;
			tcResultB.SelectionLength = tcResultB.Text.Length;
		}

		private void tcResultB_LostFocus(object sender, EventArgs e)
		{
			if (selectedRow != -1)
			{
				object val = tcResultB.Value;
				SetMatchResultB(val == null ? -1 : (double) val);
			}
		}

		private void gridResults_SelectionChanged(object sender, EventArgs e)
		{
			if (gridResults.Selection.Size == 1 && !selectingMatch)
			{
				selectedRow = gridResults.Selection.Rows[0];

				selectingMatch = true;

				if (results[selectedRow] is Sport.Championships.Cycle)
				{
					selectedRow = -1;
					tcNumber.Value = null;
					tbTeamA.Text = null;
					tbTeamB.Text = null;
					tcResultA.Value = null;
					tcResultB.Value = null;
					tcResultA.ReadOnly = true;
					tcResultB.ReadOnly = true;
				}
				else
				{
					tcNumber.Value = ((MatchRow) results[selectedRow]).MatchNumber;
					tbTeamA.Text = ((MatchRow) results[selectedRow]).TeamAName;
					tbTeamB.Text = ((MatchRow) results[selectedRow]).TeamBName;
					tcResultA.Value = ((MatchRow) results[selectedRow]).ResultA;
					tcResultB.Value = ((MatchRow) results[selectedRow]).ResultB;
					tcResultA.ReadOnly = false;
					tcResultB.ReadOnly = false;
				}

				selectingMatch = false;
			}
		}

		private void gridResults_DoubleClick(object sender, EventArgs e)
		{
			EditGameResult();
		}

		private int selectedRow = -1;

		private void SetMatchResultA(double result)
		{
			if (selectedRow != -1)
			{
				((MatchRow) results[selectedRow]).ResultA = result;
				gridResults.Refresh();
			}
		}

		private void SetMatchResultB(double result)
		{
			if (selectedRow != -1)
			{
				((MatchRow) results[selectedRow]).ResultB = result;
				gridResults.Refresh();
			}
		}

		private bool selectingMatch = false;

		private bool SelectMatch(int matchNumber)
		{
			selectedRow = -1;

			for (int n = 0; n < results.Count && selectedRow == -1; n++)
			{
				if (results[n] is MatchRow)
				{
					if (((MatchRow) results[n]).MatchNumber == matchNumber)
					{
						selectedRow = n;
					}
				}
			}

			if (selectedRow == -1)
			{
				return false;
			}

			selectingMatch = true;

			gridResults.SelectRow(selectedRow);
			gridResults.ScrollToRow(selectedRow);

			tbTeamA.Text = ((MatchRow) results[selectedRow]).TeamAName;
			tbTeamB.Text = ((MatchRow) results[selectedRow]).TeamBName;
			tcResultA.Value = ((MatchRow) results[selectedRow]).ResultA;
			tcResultB.Value = ((MatchRow) results[selectedRow]).ResultB;

			tcResultA.ReadOnly = false;
			tcResultB.ReadOnly = false;

			selectingMatch = false;

			return true;
		}

		private void EditGameResult()
		{
			if (selectedRow != -1 && enableParts)
			{
				MatchRow mr = results[selectedRow] as MatchRow;

				if (mr != null)
				{
					if (mr.GameResult == null)
					{
						if (Sport.Common.Tools.IsEmpty(mr.Match.PartsResult))
							mr.GameResult = new Sport.Championships.GameResult();
						else
							mr.GameResult = new Sport.Championships.GameResult(mr.Match.PartsResult);
					}

					MatchPartsResultForm mprf = new MatchPartsResultForm(mr.Match, mr.GameResult);

					if (mprf.ShowDialog() == System.Windows.Forms.DialogResult.OK)
					{
						mr.GameResult = (Sport.Championships.GameResult) mprf.GameResult.Clone();

						mr.ResultA = mprf.TeamAScore;
						mr.ResultB = mprf.TeamBScore;

						tcResultA.Value = mr.ResultA;
						tcResultB.Value = mr.ResultB;

						gridResults.Refresh();
					}
				}
			}
		}
	
		private class MatchRow
		{
			private Sport.Championships.Match _match;
			public Sport.Championships.Match Match
			{
				get { return _match; }
			}

			public int MatchNumber
			{
				get { return _match.Number; }
			}

			public string TeamAName
			{
				get
				{
					if (_match.GroupTeamA == null)
						return "";
					return _match.GroupTeamA.Name;
				}
			}

			public string TeamBName
			{
				get
				{
					if (_match.GroupTeamB == null)
						return "";
					return _match.GroupTeamB.Name;
				}
			}

			private void SetOutcome()
			{
				if (_resultA < 0 && _resultB < 0)
				{
					Outcome = Sport.Championships.MatchOutcome.None;
				}
				else if (_resultA > _resultB)
				{
					Outcome = Sport.Championships.MatchOutcome.WinA;
				}
				else if (_resultA == _resultB)
				{
					Outcome = Sport.Championships.MatchOutcome.Tie;
				}
				else
				{
					Outcome = Sport.Championships.MatchOutcome.WinB;
				}
			}

			public double _resultA;
			public double ResultA
			{
				get { return _resultA; }
				set
				{
					_resultA = value;
					SetOutcome();
				}
			}

			public double _resultB;
			public double ResultB
			{
				get { return _resultB; }
				set
				{
					_resultB = value;
					SetOutcome();
				}
			}

			public Sport.Championships.MatchOutcome Outcome;
			public Sport.Championships.GameResult GameResult;

			public string MatchScore
			{
				get
				{
					if (Outcome == Sport.Championships.MatchOutcome.None)
						return null;

					return (ResultB >= 0 ? ResultB.ToString() : null) + "-" + 
						(ResultA >= 0 ? ResultA.ToString() : null);
				}
			}

			public MatchRow(Sport.Championships.Match match)
			{
				_match = match;
				ResultA = _match.TeamAScore;
				ResultB = _match.TeamBScore;
				Outcome = _match.Outcome;
				GameResult = null;
			}
		}

		private System.Collections.ArrayList	results;
		private int								matchCount;

		private void ReadResults()
		{
			matchCount = 0;
			results = new System.Collections.ArrayList();

			foreach (Sport.Championships.Round round in _group.Rounds)
			{
				if ((_round >= 0)&&(_round != round.Index))
					continue;
				foreach (Sport.Championships.Cycle cycle in round.Cycles)
				{
					if ((_cycle >= 0)&&(_cycle != cycle.Index))
						continue;
					results.Add(cycle);
					foreach (Sport.Championships.Match match in cycle.Matches)
					{
						matchCount++;
						results.Add(new MatchRow(match));
					}
				}
			}

			if (results.Count > 0)
				gridResults.ExpandedRows.Add(0, results.Count - 1);
		}
		
		private bool SetResults()
		{
			Sport.Championships.MatchResult[] matchesResults = new Sport.Championships.MatchResult[matchCount];

			int index = 0;
			for (int n = 0; n < results.Count; n++)
			{
				MatchRow mr = results[n] as MatchRow;
				if (mr != null)
				{
					matchesResults[index] = new Sport.Championships.MatchResult(
						mr.MatchNumber, mr.Outcome, mr.ResultA, mr.ResultB, mr.GameResult);

					index++;
				}
			}

			return _group.SetResults(matchesResults);
		}

		#region IGridSource Members

		public void SetGrid(Sport.UI.Controls.Grid grid)
		{
		}

		public void Sort(int group, int[] columns)
		{
		}

		public Sport.UI.Controls.Style GetStyle(int row, int field, Sport.UI.Controls.GridDrawState state)
		{
			return null;
		}

		public string GetTip(int row)
		{
			return null;
		}

		public int GetGroup(int row)
		{
			if (results[row] is Sport.Championships.Cycle)
				return 0;
			return 1;
		}


		public System.Windows.Forms.Control Edit(int row, int field)
		{
			return null;
		}

		public void EditEnded(System.Windows.Forms.Control control)
		{
		}

		public int GetRowCount()
		{
			return results.Count;
		}

		public int[] GetSort(int group)
		{
			return null;
		}

		public string GetText(int row, int field)
		{
			if (row < results.Count)
			{
				if (results[row] is Sport.Championships.Cycle)
				{
					Sport.Championships.Cycle cycle = (Sport.Championships.Cycle) results[row];
					return cycle.Round.Name + " - " + cycle.Name;
				}
				else
				{
					switch (field)
					{
						case (0):
							return ((MatchRow) results[row]).MatchNumber.ToString();
						case (1):
							return ((MatchRow) results[row]).TeamAName;
						case (2):
							return ((MatchRow) results[row]).TeamBName;
						case (3):
							return ((MatchRow) results[row]).MatchScore;
					}
				}
			}

			return null;
		}

		public int GetFieldCount(int row)
		{
			return 4;
		}

		#endregion
		
		#region ICommandTarget Members
		public bool ExecuteCommand(string command)
		{
			if (command == Sport.Core.Data.KeyCommands.Cancel)
			{
				btnCancel_Click(null, EventArgs.Empty);
				return true;
			}
			
			/*
			if (command == "ENTER")
			{
				btnOK_Click(null, EventArgs.Empty);
				return true;
			}
			*/
			
			return false;
		}
		#endregion
	}
}
