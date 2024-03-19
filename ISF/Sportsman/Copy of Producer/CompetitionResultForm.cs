using System;

namespace Sportsman.Producer
{
	/// <summary>
	/// Summary description for CompetitionResultForm.
	/// </summary>
	public class CompetitionResultForm : System.Windows.Forms.Form,
		 Sport.UI.Controls.IGridSource
	{
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Label labelNumber;
		private System.Windows.Forms.Label labelTeam;
		private System.Windows.Forms.Label labelResult;
		private Sport.UI.Controls.TextControl tcNumber;
		private System.Windows.Forms.TextBox tbTeam;
		private Sport.UI.Controls.TextItemsControl ticResult;
		private System.Windows.Forms.Label labelPlayer;
		private System.Windows.Forms.TextBox tbPlayer;
		private Sport.UI.Controls.Grid gridResults;
		
		private int _minCompetitorCompetitions = 0;
		private int _maxCompetitorCompetitions = 0;
		private int _minCompetitionTeamCompetitors = 0;
		
		private CompetitionChampionshipEditorView parentView = null;
		private System.Windows.Forms.Button btnInputResults;
		private System.Windows.Forms.GroupBox gbResults;
		private System.Windows.Forms.Button btnApplyResults;
		private System.Windows.Forms.Panel pnlCurrentResult;
		private System.Windows.Forms.Label label1;
		private int _maxCompetitionTeamCompetitors=0;
		
		private int[] _customResults=null;
		private System.Windows.Forms.Label lbCurrentResult;
		private int _customResultIndex=-1;
		
		public CompetitionChampionshipEditorView ParentView
		{
			get { return parentView; }
			set { parentView = value; }
		}

		#region designer code
		private void InitializeComponent()
		{
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.gridResults = new Sport.UI.Controls.Grid();
			this.tcNumber = new Sport.UI.Controls.TextControl();
			this.tbTeam = new System.Windows.Forms.TextBox();
			this.ticResult = new Sport.UI.Controls.TextItemsControl();
			this.labelNumber = new System.Windows.Forms.Label();
			this.labelTeam = new System.Windows.Forms.Label();
			this.labelPlayer = new System.Windows.Forms.Label();
			this.labelResult = new System.Windows.Forms.Label();
			this.tbPlayer = new System.Windows.Forms.TextBox();
			this.btnInputResults = new System.Windows.Forms.Button();
			this.gbResults = new System.Windows.Forms.GroupBox();
			this.btnApplyResults = new System.Windows.Forms.Button();
			this.pnlCurrentResult = new System.Windows.Forms.Panel();
			this.lbCurrentResult = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.gbResults.SuspendLayout();
			this.pnlCurrentResult.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnOK.Location = new System.Drawing.Point(8, 440);
			this.btnOK.Name = "btnOK";
			this.btnOK.TabIndex = 3;
			this.btnOK.TabStop = false;
			this.btnOK.Text = "אישור";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnCancel.Location = new System.Drawing.Point(88, 440);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.TabIndex = 4;
			this.btnCancel.TabStop = false;
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
			this.gridResults.ExpandOnDoubleClick = true;
			this.gridResults.GroupingStyle = Sport.UI.Controls.GridGroupingStyle.Grid;
			this.gridResults.HeaderHeight = 17;
			this.gridResults.HorizontalLines = true;
			this.gridResults.Location = new System.Drawing.Point(8, 56);
			this.gridResults.Name = "gridResults";
			this.gridResults.SelectedIndex = -1;
			this.gridResults.SelectionMode = System.Windows.Forms.SelectionMode.One;
			this.gridResults.SelectOnSpace = false;
			this.gridResults.ShowCheckBoxes = false;
			this.gridResults.ShowRowNumber = false;
			this.gridResults.Size = new System.Drawing.Size(502, 355);
			this.gridResults.TabIndex = 26;
			this.gridResults.TabStop = false;
			this.gridResults.VerticalLines = true;
			this.gridResults.VisibleRow = 0;
			// 
			// tcNumber
			// 
			this.tcNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.tcNumber.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.tcNumber.Controller = null;
			this.tcNumber.Location = new System.Drawing.Point(456, 24);
			this.tcNumber.Name = "tcNumber";
			this.tcNumber.ReadOnly = false;
			this.tcNumber.SelectionLength = 0;
			this.tcNumber.SelectionStart = 0;
			this.tcNumber.ShowSpin = false;
			this.tcNumber.Size = new System.Drawing.Size(56, 21);
			this.tcNumber.TabIndex = 27;
			this.tcNumber.Value = "";
			// 
			// tbTeam
			// 
			this.tbTeam.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.tbTeam.Location = new System.Drawing.Point(272, 24);
			this.tbTeam.Name = "tbTeam";
			this.tbTeam.ReadOnly = true;
			this.tbTeam.Size = new System.Drawing.Size(176, 21);
			this.tbTeam.TabIndex = 28;
			this.tbTeam.TabStop = false;
			this.tbTeam.Text = "";
			// 
			// ticResult
			// 
			this.ticResult.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.ticResult.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.ticResult.Items = new Sport.UI.Controls.TextItemsControl.TextItem[0];
			this.ticResult.Location = new System.Drawing.Point(8, 24);
			this.ticResult.Name = "ticResult";
			this.ticResult.ReadOnly = true;
			this.ticResult.SelectionLength = 0;
			this.ticResult.SelectionStart = 0;
			this.ticResult.ShowSpin = true;
			this.ticResult.Size = new System.Drawing.Size(104, 20);
			this.ticResult.TabIndex = 29;
			// 
			// labelNumber
			// 
			this.labelNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelNumber.Location = new System.Drawing.Point(472, 8);
			this.labelNumber.Name = "labelNumber";
			this.labelNumber.Size = new System.Drawing.Size(40, 16);
			this.labelNumber.TabIndex = 30;
			this.labelNumber.Text = "מספר:";
			// 
			// labelTeam
			// 
			this.labelTeam.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelTeam.Location = new System.Drawing.Point(400, 8);
			this.labelTeam.Name = "labelTeam";
			this.labelTeam.Size = new System.Drawing.Size(48, 16);
			this.labelTeam.TabIndex = 31;
			this.labelTeam.Text = "קבוצה:";
			// 
			// labelPlayer
			// 
			this.labelPlayer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelPlayer.Location = new System.Drawing.Point(208, 8);
			this.labelPlayer.Name = "labelPlayer";
			this.labelPlayer.Size = new System.Drawing.Size(56, 16);
			this.labelPlayer.TabIndex = 32;
			this.labelPlayer.Text = "שחקן:";
			// 
			// labelResult
			// 
			this.labelResult.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelResult.Location = new System.Drawing.Point(64, 8);
			this.labelResult.Name = "labelResult";
			this.labelResult.Size = new System.Drawing.Size(48, 16);
			this.labelResult.TabIndex = 33;
			this.labelResult.Text = "תוצאה:";
			// 
			// tbPlayer
			// 
			this.tbPlayer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.tbPlayer.Location = new System.Drawing.Point(120, 24);
			this.tbPlayer.Name = "tbPlayer";
			this.tbPlayer.ReadOnly = true;
			this.tbPlayer.Size = new System.Drawing.Size(144, 21);
			this.tbPlayer.TabIndex = 34;
			this.tbPlayer.TabStop = false;
			this.tbPlayer.Text = "";
			// 
			// btnInputResults
			// 
			this.btnInputResults.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnInputResults.Location = new System.Drawing.Point(95, 18);
			this.btnInputResults.Name = "btnInputResults";
			this.btnInputResults.Size = new System.Drawing.Size(72, 23);
			this.btnInputResults.TabIndex = 35;
			this.btnInputResults.Text = "קליטה";
			this.btnInputResults.Click += new System.EventHandler(this.btnInputResults_Click);
			// 
			// gbResults
			// 
			this.gbResults.Controls.Add(this.btnApplyResults);
			this.gbResults.Controls.Add(this.btnInputResults);
			this.gbResults.Location = new System.Drawing.Point(335, 416);
			this.gbResults.Name = "gbResults";
			this.gbResults.Size = new System.Drawing.Size(170, 48);
			this.gbResults.TabIndex = 36;
			this.gbResults.TabStop = false;
			this.gbResults.Text = "תוצאות";
			this.gbResults.Visible = false;
			// 
			// btnApplyResults
			// 
			this.btnApplyResults.Enabled = false;
			this.btnApplyResults.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnApplyResults.Location = new System.Drawing.Point(10, 18);
			this.btnApplyResults.Name = "btnApplyResults";
			this.btnApplyResults.Size = new System.Drawing.Size(72, 23);
			this.btnApplyResults.TabIndex = 36;
			this.btnApplyResults.Text = "שיוך";
			this.btnApplyResults.Click += new System.EventHandler(this.btnApplyResults_Click);
			// 
			// pnlCurrentResult
			// 
			this.pnlCurrentResult.Controls.Add(this.lbCurrentResult);
			this.pnlCurrentResult.Controls.Add(this.label1);
			this.pnlCurrentResult.Location = new System.Drawing.Point(184, 416);
			this.pnlCurrentResult.Name = "pnlCurrentResult";
			this.pnlCurrentResult.Size = new System.Drawing.Size(136, 48);
			this.pnlCurrentResult.TabIndex = 37;
			this.pnlCurrentResult.Visible = false;
			// 
			// lbCurrentResult
			// 
			this.lbCurrentResult.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.lbCurrentResult.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbCurrentResult.ForeColor = System.Drawing.Color.Blue;
			this.lbCurrentResult.Location = new System.Drawing.Point(0, 23);
			this.lbCurrentResult.Name = "lbCurrentResult";
			this.lbCurrentResult.Size = new System.Drawing.Size(136, 25);
			this.lbCurrentResult.TabIndex = 1;
			this.lbCurrentResult.Text = "00:00";
			this.lbCurrentResult.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(32, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "שיוך נוכחי:";
			// 
			// CompetitionResultForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.ClientSize = new System.Drawing.Size(520, 472);
			this.Controls.Add(this.pnlCurrentResult);
			this.Controls.Add(this.gbResults);
			this.Controls.Add(this.tbPlayer);
			this.Controls.Add(this.tbTeam);
			this.Controls.Add(this.ticResult);
			this.Controls.Add(this.labelResult);
			this.Controls.Add(this.labelPlayer);
			this.Controls.Add(this.labelTeam);
			this.Controls.Add(this.labelNumber);
			this.Controls.Add(this.tcNumber);
			this.Controls.Add(this.gridResults);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.Name = "CompetitionResultForm";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "הכנסת תוצאות";
			this.Load += new System.EventHandler(this.CompetitionResultForm_Load);
			this.Closed += new System.EventHandler(this.CompetitionResultForm_Closed);
			this.Activated += new System.EventHandler(this.CompetitionResultForm_Activated);
			this.gbResults.ResumeLayout(false);
			this.pnlCurrentResult.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion
		
		private Sport.Championships.Competition _competition = null;
		private Sport.Championships.Heat _heat = null;

		private CompetitionResultForm(Sport.Championships.Competition competition,
			Sport.Championships.Heat heat)
		{
			_competition = competition;
			_heat = heat;

			InitializeComponent();

			tcNumber.Controller = new Sport.UI.Controls.NumberController(0, 99999, 4, 0);
			tcNumber.LostFocus += new EventHandler(tcNumber_LostFocus);
			tcNumber.KeyPress += new System.Windows.Forms.KeyPressEventHandler(tcNumber_KeyPress);
			tcNumber.TextChanged += new EventHandler(tcNumber_TextChanged);
			tcNumber.GotFocus += new EventHandler(tcNumber_GotFocus);

			ticResult.SetFormatterItems(competition.ResultType.ValueFormatter);
			ticResult.KeyPress += new System.Windows.Forms.KeyPressEventHandler(ticResult_KeyPress);
			ticResult.GotFocus += new EventHandler(ticResult_GotFocus);
			ticResult.LostFocus += new EventHandler(ticResult_LostFocus);

			gridResults.Groups.Clear();
			gridResults.Groups[0].Columns.Add(0, "מספר", 20);
			gridResults.Groups[0].Columns.Add(1, "קבוצה", 40);
			gridResults.Groups[0].Columns.Add(2, "שחקן", 40);
			gridResults.Groups[0].Columns.Add(3, "תוצאה", 40);

			ReadResults();

			gridResults.Source = this;

			gridResults.SelectionChanged += new EventHandler(gridResults_SelectionChanged);
			
			Sport.Rulesets.Rules.CompetitorCompetitions rule1=
				(Sport.Rulesets.Rules.CompetitorCompetitions)
				_competition.GetRule(typeof(Sport.Rulesets.Rules.CompetitorCompetitions), 
				typeof(Sport.Rulesets.Rules.CompetitorCompetitionsRule));
			if (rule1 == null)
			{
				_minCompetitorCompetitions = 0;
				_maxCompetitorCompetitions = 99;
				Sport.UI.MessageBox.Warn("לא מוגדר חוק 'מקצועות למשתתף', בדיקת תקנון לא פעילה", "קביעת תוצאות תחרות");
			}
			else
			{
				_minCompetitorCompetitions = rule1.Minimum;
				_maxCompetitorCompetitions = rule1.Maximum;
			}
			
			Sport.Rulesets.Rules.CompetitionTeamCompetitors rule2=
				(Sport.Rulesets.Rules.CompetitionTeamCompetitors)
				_competition.GetRule(typeof(Sport.Rulesets.Rules.CompetitionTeamCompetitors), 
				typeof(Sport.Rulesets.Rules.CompetitionTeamCompetitorsRule));
			if (rule2 == null)
			{
				_minCompetitionTeamCompetitors = 0;
				_maxCompetitionTeamCompetitors = 999;
				Sport.UI.MessageBox.Warn("לא מוגדר חוק 'משתתפי קבוצה במקצוע' עבור תחרות זו, בדיקת תקנון לא פעילה", "קביעת תוצאות תחרות");
			}
			else
			{
				_minCompetitionTeamCompetitors = rule2.Minimum;
				_maxCompetitionTeamCompetitors = rule2.Maximum;
			}
		}

		public CompetitionResultForm(Sport.Championships.Competition competition)
			: this(competition, null)
		{
		}

		public CompetitionResultForm(Sport.Championships.Heat heat)
			: this(heat.Competition, heat)
		{
		}

		private void btnOK_Click(object sender, System.EventArgs e)
		{
			if (SetResults())
			{
				DialogResult = System.Windows.Forms.DialogResult.OK;
				if (parentView != null)
				{
					parentView.Rebuild();
				}
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
			//Enter pressed?
			if (e.KeyChar != (char) System.Windows.Forms.Keys.Enter)
				return;
			
			//got anything?
			if (tcNumber.Text.Length == 0)
				return;
			
			//get shirt number:
			int playerNumber = (int) (double) tcNumber.Value;
			
			//try to find and select proper player:
			string strMessage=SelectPlayer(playerNumber);
			
			//successful?
			if (strMessage.Length == 0)
			{
				//current result?
				if (_customResultIndex >= 0)
				{
					int curSeconds=_customResults[_customResultIndex];
					SetPlayerResult(curSeconds*1000);
					_customResultIndex++;
					if (_customResultIndex >= _customResults.Length)
					{
						pnlCurrentResult.Visible = false;
						Sport.Core.Configuration.WriteString(
							"General", "InputResultSeconds", "");
						Sport.Core.Configuration.WriteString(
							"General", "InputResultResults", "");
						_customResultIndex = -1;
					}
					else
					{
						ApplyCurrentResult();
						tcNumber.Focus();
					}
				}
				else
				{
					//player has been found or added.
					ticResult.Focus();
				}
				return;
			}
			
			//something was wrong...
			tcNumber.SelectionStart = 0;
			tcNumber.SelectionLength = tcNumber.Text.Length;
			
			//show alert:
			Sport.UI.MessageBox.Error(strMessage, "קביעת תוצאות תחרות");
		}

		private void tcNumber_LostFocus(object sender, EventArgs e)
		{
			if (selectedRow == -1)
			{
				if (tcNumber.Text.Length != 0)
				{
					SelectPlayer((int) (double) tcNumber.Value);
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
			if (!selectingPlayer)
			{
				tbTeam.Text = null;
				tbPlayer.Text = null;
				selectedRow = -1;
				ticResult.ReadOnly = true;
			}
		}

        private void ticResult_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			if (e.KeyChar == (char) System.Windows.Forms.Keys.Enter)
			{
				tcNumber.Focus();
			}
		}

		private void ticResult_GotFocus(object sender, EventArgs e)
		{
			ticResult.SelectionStart = 0;
		}

		private void ticResult_LostFocus(object sender, EventArgs e)
		{
			if (selectedRow != -1)
			{
				SetPlayerResult(ticResult.GetValue(_competition.ResultType.ValueFormatter));
			}
		}


		private void gridResults_SelectionChanged(object sender, EventArgs e)
		{
			if (gridResults.Selection.Size == 1 && !selectingPlayer)
			{
				selectedRow = gridResults.Selection.Rows[0];

				selectingPlayer = true;

				tcNumber.Value = ((Row) results[selectedRow]).PlayerNumber;
				tbTeam.Text = ((Row) results[selectedRow]).TeamName;
				tbPlayer.Text = ((Row) results[selectedRow]).PlayerName;
				int curResult=((Row) results[selectedRow]).Result;
				ticResult.SetValue(_competition.ResultType.ValueFormatter, 
					curResult);
				
				selectingPlayer = false;
			}
		}

		private int selectedRow = -1;

		private void SetPlayerResult(int result)
		{
			if (selectedRow != -1)
			{
				((Row) results[selectedRow]).Result = result;
				gridResults.Refresh();
			}
		}

		private bool selectingPlayer = false;

		private string SelectPlayer(int playerNumber)
		{
			//search for player with given shirt number in the grid:
			selectedRow = -1;
			for (int n = 0; n < results.Count && selectedRow == -1; n++)
			{
				if (((Row) results[n]).PlayerNumber == playerNumber)
					selectedRow = n;
			}
			
			//player not here?
			if (selectedRow == -1)
			{
				//look for the player in the whole group:
				Sport.Championships.CompetitionPlayer player =
					_competition.Group.Players[playerNumber];
				
				//found the player?
				if (player != null)
				{
					//get competitor competitions and team competitors:
					int competitorCompetitions=
						_competition.Group.GetCompetitorCompetitions(playerNumber);
					int teamCompetitors=
						_competition.Group.GetTeamCompetitionCompetitors(player.CompetitionTeam, _competition.Index);
					
					//maybe this player is already playing in maximum competitions?
					if (competitorCompetitions >= _maxCompetitorCompetitions)
						return "מתמודד זה כבר משתתף בכמות מקסימלית של מקצועות המותרת לפי התקנון";
					
					//maybe team already have maximum competitors?
					if (teamCompetitors >= _maxCompetitionTeamCompetitors)
						return "בית הספר כבר משתף את מירב המתמודדים המותרים לפי התקנון";
					
					selectedRow = results.Add(new Row(playerNumber, player.CompetitionTeam.Name,
						player.PlayerEntity == null ? null : player.PlayerEntity.Name, -1));
					gridResults.RefreshSource();
				}
				else
				{
					//maybe offline?
					Sport.Entities.OfflineTeam[] arrOfflineTeams = 
						_competition.Group.GetOfflineTeams();
					bool blnOffline = false;
					foreach (Sport.Entities.OfflineTeam offlineTeam in arrOfflineTeams)
					{
						int pNumFrom = offlineTeam.PlayerNumberFrom;
						int pNumTo = offlineTeam.PlayerNumberTo;
						if ((playerNumber >= pNumFrom)&&(playerNumber <= pNumTo))
						{
							blnOffline = true;
							break;
						}
					}
					if (blnOffline)
						return "על מנת להוסיף תוצאה לקבוצה לא מקוונת אנא הוסף שחקן";
				}
			}
			
			//got anything?
			if (selectedRow >= 0)
			{
				//raise global flag:
				selectingPlayer = true;
				
				//select player and scroll to its row
				gridResults.SelectRow(selectedRow);
				gridResults.ScrollToRow(selectedRow);
				
				//set player and team text:
				tbTeam.Text = ((Row) results[selectedRow]).TeamName;
				tbPlayer.Text = ((Row) results[selectedRow]).PlayerName;
				
				//set result:
				ticResult.SetValue(_competition.ResultType.ValueFormatter, 
					((Row) results[selectedRow]).Result);
				
				//let user change the result:
				ticResult.ReadOnly = false;
				
				//lower global flag:
				selectingPlayer = false;
				
				//done.
				return "";
			} //end if player is in this grid.
			
			//getting here means player does not exist.
			return "מספר חזה לא מזוהה";
		} //end function SelectPlayer
	
		private class Row
		{
			public int		PlayerNumber;
			public string	TeamName;
			public string	PlayerName;
			public int		Result;
			public Row(int playerNumber, string teamName, string playerName, int result)
			{
				PlayerNumber = playerNumber;
                TeamName = teamName;
				PlayerName = playerName;
				Result = result;
			}
		}

		private System.Collections.ArrayList	results;

		private void ReadResults()
		{
			results = new System.Collections.ArrayList();
			if (_competition == null)
				return;
			System.Collections.ArrayList arrCompetitors = 
				new System.Collections.ArrayList();
			if (_competition.Competitors != null)
				arrCompetitors.AddRange(_competition.Competitors);
			arrCompetitors.AddRange(_competition.GetOfflinePlayers());
			if (arrCompetitors.Count == 0)
				return;
			//System.Collections.Hashtable tblOfflineTeams = 
			//	new System.Collections.Hashtable();
			foreach (object oComp in arrCompetitors)
			{
				int curShirtNumber = -1;
				string curPlayerName = null;
				string curTeamName = null;
				int curResult = 0;
				if (oComp is Sport.Championships.Competitor)
				{
					Sport.Championships.Competitor competitor = 
						(Sport.Championships.Competitor) oComp;
					if (_heat == null || competitor.Heat == _heat.Index)
					{
						curShirtNumber = competitor.PlayerNumber;
						if ((competitor.Player != null)&&(competitor.Player.PlayerEntity != null))
							curPlayerName = competitor.Player.PlayerEntity.Name;
						if ((competitor.Team >= 0)&&(competitor.Team < _competition.Group.Teams.Count))
						{
							Sport.Championships.CompetitionTeam compTeam = 
								_competition.Group.Teams[competitor.Team];
							if (compTeam != null)
							{
								curTeamName = compTeam.Name;
							}
						}
						curResult = competitor.Result;
					}
				}
				else if(oComp is Sport.Entities.OfflinePlayer)
				{
					Sport.Entities.OfflinePlayer oPlayer = 
						(Sport.Entities.OfflinePlayer) oComp;
					curShirtNumber = oPlayer.ShirtNumber;
					curPlayerName = oPlayer.ToString();
					if (oPlayer.Team != null)
						curTeamName = oPlayer.Team.TeamName();
					else if (oPlayer.OfflineTeam != null)
					{
						//int ID = oPlayer.OfflineTeam.OfflineID;
						//Sport.Entities.OfflineTeam oTeam = tblOfflineTeams[
						curTeamName = oPlayer.OfflineTeam.ToString();
					}
					curResult = oPlayer.Result;
				}
				if (curShirtNumber < 0)
					continue;
				results.Add(new Row(curShirtNumber, curTeamName, curPlayerName, 
					curResult));
			}
		}

		private bool SetResults()
		{
			Sport.Championships.CompetitorResult[] competitorsResults = new Sport.Championships.CompetitorResult[results.Count];
			for (int n = 0; n < results.Count; n++)
			{
				competitorsResults[n] = new Sport.Championships.CompetitorResult(
					((Row) results[n]).PlayerNumber, ((Row) results[n]).Result);
			}

			return _competition.SetResults(_heat == null ? -1 : _heat.Index, competitorsResults);
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
			return 0;
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
				switch (field)
				{
					case (0):
						return ((Row) results[row]).PlayerNumber.ToString();
					case (1):
						return ((Row) results[row]).TeamName;
					case (2):
						return ((Row) results[row]).PlayerName;
					case (3):
						return _competition.ResultType == null ? null :
							_competition.ResultType.FormatResult(((Row) results[row]).Result);
				}
			}

			return null;
		}

		public int GetFieldCount(int row)
		{
			return 4;
		}

		#endregion

		private void CompetitionResultForm_Load(object sender, System.EventArgs e)
		{
			this.Text = "הכנסת תוצאות" + " - " + _competition.Group.Phase.Championship.Name + " - " + _competition.Name;
			if ((_competition.GeneralData != null)&&(_competition.GeneralData.ScoreIsRank))
				gbResults.Visible = true;
		}

		private InputResultsForm resultForm = null;
		private void btnInputResults_Click(object sender, System.EventArgs e)
		{
			if (!ResultFormOpened())
			{
				resultForm = new InputResultsForm();
			}
			else
			{
			}
			resultForm.ParentResultForm = this;
			ShowResultForm();
		}

		public void SetCustomResults(int[] customResults)
		{
			_customResults = customResults;
			if ((_customResults != null)&&(_customResults.Length > 0))
			{
				btnApplyResults.Enabled = true;
				btnApplyResults.Focus();
			}
			else
			{
				btnApplyResults.Enabled = false;
			}
		}

		private bool ResultFormOpened()
		{
			return (resultForm != null && !resultForm.IsDisposed && resultForm.DialogResult == System.Windows.Forms.DialogResult.None);
		}

		private void ShowResultForm()
		{
			resultForm.WindowState = System.Windows.Forms.FormWindowState.Normal;
			resultForm.Show();
			resultForm.Focus();
		}

		private void btnApplyResults_Click(object sender, System.EventArgs e)
		{
			//got anything?
			if ((_customResults == null)||(_customResults.Length == 0))
				return;
			pnlCurrentResult.Visible = true;
			btnApplyResults.Enabled = false;
			_customResultIndex = 0;
			ApplyCurrentResult();
			tcNumber.Focus();
		}
		
		private void ApplyCurrentResult()
		{
			int seconds=_customResults[_customResultIndex];
			int minutes=(int) (((double) seconds)/((double) 60));
			seconds %= 60;
			lbCurrentResult.Text = minutes.ToString().PadLeft(2, '0')+":"+
				seconds.ToString().PadLeft(2, '0');
		}

		private void CompetitionResultForm_Closed(object sender, System.EventArgs e)
		{
			if (ResultFormOpened())
				resultForm.Close();
			
			if (parentView != null)
				Sport.UI.ViewManager.SelectedView = parentView;
		}

		private void CompetitionResultForm_Activated(object sender, System.EventArgs e)
		{
			if (ResultFormOpened())
				ShowResultForm();
		}
	}
}
