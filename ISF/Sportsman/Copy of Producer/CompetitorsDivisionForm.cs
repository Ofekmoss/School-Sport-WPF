using System;
using Sport.Rulesets.Rules;

namespace Sportsman.Producer
{
	/// <summary>
	/// Summary description for CompetitorsDivisionForm.
	/// </summary>
	public class CompetitorsDivisionForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnCheckAll;
		private Sport.UI.Controls.Grid gridCompetitors;
		
		#region designer code
		private void InitializeComponent()
		{
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.gridCompetitors = new Sport.UI.Controls.Grid();
			this.btnCheckAll = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnOK.Location = new System.Drawing.Point(8, 440);
			this.btnOK.Name = "btnOK";
			this.btnOK.TabIndex = 3;
			this.btnOK.Text = "אישור";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnCancel.Location = new System.Drawing.Point(88, 440);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.TabIndex = 4;
			this.btnCancel.Text = "ביטול";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// gridCompetitors
			// 
			this.gridCompetitors.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.gridCompetitors.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.gridCompetitors.Editable = true;
			this.gridCompetitors.ExpandOnDoubleClick = true;
			this.gridCompetitors.GroupingStyle = Sport.UI.Controls.GridGroupingStyle.Grid;
			this.gridCompetitors.HeaderHeight = 17;
			this.gridCompetitors.HorizontalLines = true;
			this.gridCompetitors.Location = new System.Drawing.Point(8, 8);
			this.gridCompetitors.Name = "gridCompetitors";
			this.gridCompetitors.SelectedIndex = -1;
			this.gridCompetitors.SelectionMode = System.Windows.Forms.SelectionMode.One;
			this.gridCompetitors.SelectOnSpace = false;
			this.gridCompetitors.ShowCheckBoxes = false;
			this.gridCompetitors.ShowRowNumber = false;
			this.gridCompetitors.Size = new System.Drawing.Size(552, 424);
			this.gridCompetitors.TabIndex = 26;
			this.gridCompetitors.VerticalLines = true;
			this.gridCompetitors.VisibleRow = 0;
			// 
			// btnCheckAll
			// 
			this.btnCheckAll.Location = new System.Drawing.Point(440, 440);
			this.btnCheckAll.Name = "btnCheckAll";
			this.btnCheckAll.Size = new System.Drawing.Size(88, 23);
			this.btnCheckAll.TabIndex = 27;
			this.btnCheckAll.Text = "סמן את כולם";
			this.btnCheckAll.Click += new System.EventHandler(this.btnCheckAll_Click);
			// 
			// CompetitorsDivisionForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.ClientSize = new System.Drawing.Size(570, 472);
			this.Controls.Add(this.btnCheckAll);
			this.Controls.Add(this.gridCompetitors);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Name = "CompetitorsDivisionForm";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "חלוקת שחקנים";
			this.ResumeLayout(false);

		}
		#endregion
	
		public CompetitorsDivisionForm(Sport.Championships.CompetitionGroup group)
		{
			InitializeComponent();

			gridCompetitors.Source = new CompetitorsDivisionGridSource(group);
		}

		private void btnOK_Click(object sender, System.EventArgs e)
		{
			if 	(!((CompetitorsDivisionGridSource) gridCompetitors.Source).UpdateGroup())
				return;
			
			DialogResult = System.Windows.Forms.DialogResult.OK;
			Close();
		}

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			DialogResult = System.Windows.Forms.DialogResult.Cancel;
			Close();
		}

		private void btnCheckAll_Click(object sender, System.EventArgs e)
		{
			CompetitorsDivisionGridSource source=
				(CompetitorsDivisionGridSource) gridCompetitors.Source;
			source.CheckAllRows();
		}
	}

	public class CompetitorsDivisionGridSource : Sport.UI.Controls.IGridSource
	{
		private Sport.UI.Controls.Grid _grid;
		private Sport.Championships.CompetitionGroup _group;

		#region player row
		private struct PlayerRow
		{
			public Sport.Entities.Player	Player;
			public int[]					CompetitionsHeat;

			public PlayerRow(Sport.Entities.Player player, int[] competitionsHeat)
			{
				Player = player;
				CompetitionsHeat = competitionsHeat;
			}
			
			public int GetCompetitionsCount()
			{
				int result=0;
				for (int i=0; i<this.CompetitionsHeat.Length; i++)
				{
					if (this.CompetitionsHeat[i] >= 0)
						result++;
				}
				return result;
			}
		}
		#endregion

		private System.Drawing.StringFormat stringFormat;
		
		private Sport.UI.Controls.Style _tooLittleStyle;
		private Sport.UI.Controls.Style _tooMuchStyle;
		
		private int _minCompetitorCompetitions=0;
		private int _maxCompetitorCompetitions=0;
		
		private int[] _minCompetitionTeamCompetitors=null;
		private int[] _maxCompetitionTeamCompetitors=null;
		
		public CompetitorsDivisionGridSource(Sport.Championships.CompetitionGroup group)
		{
			_tooLittleStyle = new Sport.UI.Controls.Style();
			_tooMuchStyle = new Sport.UI.Controls.Style();
			
			_tooLittleStyle.Background = System.Drawing.Brushes.Orange;
			_tooMuchStyle.Background = System.Drawing.Brushes.Red;
			
			_group = group;
			
			if (_group.Competitions.Count > 0)
			{
				_minCompetitionTeamCompetitors = new int[_group.Competitions.Count];
				_maxCompetitionTeamCompetitors = new int[_group.Competitions.Count];

				string strInvalidCompetitions="";
				for (int c=0; c<_group.Competitions.Count; c++)
				{
					if (c == 0)
					{
						CompetitorCompetitions rule1=(CompetitorCompetitions)
							_group.Competitions[c].GetRule(typeof(CompetitorCompetitions), 
							typeof(Sport.Rulesets.Rules.CompetitorCompetitionsRule));
						if (rule1 == null)
						{
							_minCompetitorCompetitions = -1;
							_maxCompetitorCompetitions = -1;
							Sport.UI.MessageBox.Warn("לא מוגדר חוק 'מקצועות למשתתף', בדיקת תקנון לא פעילה", "חלוקת משתתפים לתחרויות");
						}
						else
						{
							_minCompetitorCompetitions = rule1.Minimum;
							_maxCompetitorCompetitions = rule1.Maximum;
						}
					}
					
					CompetitionTeamCompetitors rule2=(CompetitionTeamCompetitors)
						_group.Competitions[c].GetRule(typeof(CompetitionTeamCompetitors), 
						typeof(Sport.Rulesets.Rules.CompetitionTeamCompetitorsRule));
					if (rule2 == null)
					{
						_minCompetitionTeamCompetitors[c] = -1;
						_maxCompetitionTeamCompetitors[c] = -1;
						strInvalidCompetitions += _group.Competitions[c].Name+"\n";
					}
					else
					{
						_minCompetitionTeamCompetitors[c] = rule2.Minimum;
						_maxCompetitionTeamCompetitors[c] = rule2.Maximum;
					}
				} //end loop over group competition
				
				if (strInvalidCompetitions.Length > 0)
				{
					Sport.UI.MessageBox.Warn("עבור המקצועות הבאים "+
						"לא מוגדר חוק 'משתתפי קבוצה במקצוע':\n"+strInvalidCompetitions, "חלוקת משתתפים לתחרויות");
				}
			} //end if group has any competitions
			
			System.Collections.ArrayList al = new System.Collections.ArrayList();
			System.Collections.Hashtable ht = new System.Collections.Hashtable();

			int[] emptyCompetitions = new int[_group.Competitions.Count];
			for (int n = 0; n < _group.Competitions.Count; n++)
				emptyCompetitions[n] = -1;
			
			Sport.UI.Dialogs.WaitForm.ShowWait("טוען נתונים אנא המתן...");
			Sport.UI.Dialogs.WaitForm.SetProgress(0);
			
			double progress_Team=30;
			if (_group.Teams.Count > 0)
				progress_Team = (((double) 30)/((double) _group.Teams.Count));
			
			double current_Progress=0;
			
			// Adding all teams players to array
			foreach (Sport.Championships.Team team in _group.Teams)
			{
				al.Add(team.TeamEntity);
				Sport.Entities.Player[] players = team.TeamEntity.Players;
				
				foreach (Sport.Entities.Player player in players)
				{
					ht[player.Id] = al.Add(new PlayerRow(player, (int[]) emptyCompetitions.Clone()));
				}
				
				current_Progress += progress_Team;
				Sport.UI.Dialogs.WaitForm.SetProgress((int) current_Progress);
			}
			current_Progress = 30;
			Sport.UI.Dialogs.WaitForm.SetProgress((int) current_Progress);

			// Setting competitions in all players
			double progress_Competition=70;
			if (_group.Competitions.Count > 0)
				progress_Competition= (((double) 70)/((double) _group.Competitions.Count));
			foreach (Sport.Championships.Competition competition in _group.Competitions)
			{
				double progress_Competitor=progress_Competition;
				if (competition.Competitors.Count > 0)
					progress_Competitor= (progress_Competition/((double) competition.Competitors.Count));
				foreach (Sport.Championships.Competitor competitor in competition.Competitors)
				{
					if ((competitor.Player != null)&&(competitor.Player.PlayerEntity != null))
					{
						int index = (int) ht[competitor.Player.PlayerEntity.Id];
						((PlayerRow) al[index]).CompetitionsHeat[competition.Index] = competitor.Heat + 1;
					}
					current_Progress += progress_Competitor;
					Sport.UI.Dialogs.WaitForm.SetProgress((int) current_Progress);
				}
			}
			
			Sport.UI.Dialogs.WaitForm.SetProgress(100);
			Sport.UI.Dialogs.WaitForm.HideWait();
			_rows = al.ToArray();

			stringFormat = new System.Drawing.StringFormat(System.Drawing.StringFormatFlags.DirectionRightToLeft);
			stringFormat.Alignment = System.Drawing.StringAlignment.Near;
		}

		private void CompetitorsDivision_DrawCell(object sender, Sport.UI.Controls.Grid.DrawCellEventArgs e)
		{
			int competition = e.Column.Field - 2;
			int heats = _group.Competitions[competition].Heats.Count;
			int heat = ((PlayerRow) _rows[e.Row]).CompetitionsHeat[competition];

			System.Drawing.Rectangle rect = new System.Drawing.Rectangle(e.Rectangle.Location,
				new System.Drawing.Size(e.Rectangle.Width - 14, 14));
			e.Graphics.DrawString("משתתף", e.Style.Font, e.Style.Foreground, rect, stringFormat);
			System.Windows.Forms.ControlPaint.DrawCheckBox(e.Graphics,
				rect.Right + 1, rect.Top + 2, 12, 12, 
				heat == -1 ? System.Windows.Forms.ButtonState.Normal :
				System.Windows.Forms.ButtonState.Checked);
			for (int h = 0; h < heats; h++)
			{
				rect = new System.Drawing.Rectangle(rect.Left, rect.Bottom, rect.Width, 14);
                e.Graphics.DrawString("מקצה " + (h + 1).ToString(), e.Style.Font,
					e.Style.Foreground, rect, stringFormat); 
				System.Windows.Forms.ControlPaint.DrawRadioButton(e.Graphics,
					rect.Right + 1, rect.Top + 2, 12, 12,
					heat == h + 1 ? System.Windows.Forms.ButtonState.Checked :
					System.Windows.Forms.ButtonState.Normal);
			}
		}

		private void CompetitorsDivision_MouseDown(object sender, Sport.UI.Controls.Grid.CellMouseEventArgs e)
		{
			if (e.Offset.Width <= 13 &&
				e.Offset.Width >= 2)
			{
				int competition = e.Column.Field - 2;
				int heat = e.Offset.Height / 14;
				if (e.Offset.Height >= heat * 14 + 2 &&
					e.Offset.Height <= heat * 14 + 13 &&
					_group.Competitions[competition].Heats.Count >= heat)
				{
					int oldHeat = ((PlayerRow) _rows[e.Row]).CompetitionsHeat[competition];

					if (oldHeat != -1 && heat == 0)
						heat = -1;

					((PlayerRow) _rows[e.Row]).CompetitionsHeat[competition] = heat;
					_grid.Refresh();
				}
			}
		}
		
		public bool UpdateGroup()
		{
			if (_group.Competitions.Count == 0)
				return true;
			
			string strMessage="";
			
			Sport.UI.Dialogs.WaitForm.ShowWait("בודק עמידה בתקנון אנא המתן...", true);
			Sport.UI.Dialogs.WaitForm.SetProgress(0);
			
			System.Collections.ArrayList arrOutsideRangePlayers=
				new System.Collections.ArrayList();
			System.Collections.Hashtable tblOutsideRangeTeams=new System.Collections.Hashtable();
			int outsideRangeTeamsCount=0;
			double ratio=(((double) 100)/((double) _rows.Length));
			double currentProgress=0;
			for (int p = 0; p < _rows.Length; p++)
			{
				if (_rows[p] is PlayerRow)
				{
					PlayerRow pr = (PlayerRow) _rows[p];
					int curCompetitorCompetitions=pr.GetCompetitionsCount();
					if ((_minCompetitorCompetitions >= 0)&&
						((curCompetitorCompetitions < _minCompetitorCompetitions)||
						(curCompetitorCompetitions > _maxCompetitorCompetitions)))
					{
						arrOutsideRangePlayers.Add(pr.Player.FirstName+" "+
							pr.Player.LastName+" ("+pr.Player.Team.TeamName()+", "+
							curCompetitorCompetitions+" מקצועות)\n");
					}
				}
				else
				{
					if (_rows[p] is Sport.Entities.Team)
					{
						Sport.Entities.Team team=(Sport.Entities.Team) _rows[p];
						if ((p == _rows.Length-1)||(_rows[p+1] is Sport.Entities.Team))
						{
							currentProgress += ratio;
							Sport.UI.Dialogs.WaitForm.SetProgress((int) currentProgress);
							continue;
						}
						for (int c=0; c<_group.Competitions.Count; c++)
						{
							if (_minCompetitionTeamCompetitors[c] < 0)
								continue;
							int competionCount=GetTeamCompetitionCount(p, c);
							if ((competionCount < _minCompetitionTeamCompetitors[c])||
								(competionCount > _maxCompetitionTeamCompetitors[c]))
							{
								string strMsg=competionCount+" משתתפים במקצוע '"+
									_group.Competitions[c].Name+"'";
								if (tblOutsideRangeTeams[team] == null)
								{
									tblOutsideRangeTeams[team] = "";
									outsideRangeTeamsCount++;
								}
								tblOutsideRangeTeams[team] = 
									tblOutsideRangeTeams[team].ToString()+strMsg+", ";
							}
						}
					} //end if row is team row
				} //end if row is not player row
				
				currentProgress += ratio;
				Sport.UI.Dialogs.WaitForm.SetProgress((int) currentProgress);
			} //end loop over rows.
			
			Sport.UI.Dialogs.WaitForm.SetProgress(100);
			Sport.UI.Dialogs.WaitForm.HideWait();
			
			if (arrOutsideRangePlayers.Count > 0)
			{
				strMessage += "המתמודדים הבאים לא עומדים בחוק מקצועות למשתתף: "+
					"(טווח חוקי הוא "+_minCompetitorCompetitions+"-"+
					_maxCompetitorCompetitions+" מקצועות)\n";
				for (int i=0; i<arrOutsideRangePlayers.Count; i++)
				{
					if (i >= 5)
					{
						strMessage += "...\n";
						break;
					}
					strMessage += arrOutsideRangePlayers[i]+"\n";
				}
			}
			
			if (outsideRangeTeamsCount > 0)
			{
				strMessage += "הקבוצות הבאות לא עומדות בחוק משתתפי קבוצה למקצוע: \n";
				int count=0;
				foreach (Sport.Entities.Team curTeam in tblOutsideRangeTeams.Keys)
				{
					if (count >= 5)
					{
						strMessage += "...\n";
						break;
					}
					string strCompetitions=tblOutsideRangeTeams[curTeam].ToString();
					strMessage += curTeam.Name+": "+
						strCompetitions.Substring(0, strCompetitions.Length-2)+"\n";
					count++;
				}
				strMessage += "\n";
			}
			
			if (strMessage.Length > 0)
			{
				if (!Sport.UI.MessageBox.Ask("אזהרה!\n"+strMessage+"\nהאם להמשיך בכל זאת?", 
					System.Windows.Forms.MessageBoxIcon.Warning, false))
					return false;
			}
			
			Sport.UI.Dialogs.WaitForm.ShowWait("מוסיף מתמודדים אנא המתן...");
			
			System.Collections.Hashtable[] tblCompetitors=
				new System.Collections.Hashtable[_group.Competitions.Count];
			for (int c = 0; c < _group.Competitions.Count; c++)
			{
				Sport.Championships.Competition competition=_group.Competitions[c];
				tblCompetitors[c] = new System.Collections.Hashtable();
				int n = 0;
				while (n < competition.Competitors.Count)
				{
					if ((competition.Competitors[n].Player != null)&&
						(competition.Competitors[n].Player.PlayerEntity != null))
					{
						Sport.Championships.Competitor competitor=competition.Competitors[n];
						tblCompetitors[c][competitor.PlayerNumber] = competitor;
						competition.Competitors.Remove(competitor);
					}
					else
					{
						n++;
					}
				}
			}

			for (int p = 0; p < _rows.Length; p++)
			{
				if (_rows[p] is PlayerRow)
				{
					PlayerRow pr = (PlayerRow) _rows[p];
					for (int c = 0; c < _group.Competitions.Count; c++)
					{
						if (pr.CompetitionsHeat[c] >= 0)
						{
							int playerNumber=pr.Player.Number;
							Sport.Championships.Competitor competitor=
								(Sport.Championships.Competitor) tblCompetitors[c][playerNumber];
							if (competitor == null)
								competitor = new Sport.Championships.Competitor(playerNumber);
							competitor.Heat = pr.CompetitionsHeat[c] - 1;
							_group.Competitions[c].Competitors.Add(competitor);
						}
					}
				}
			}
			
			Sport.UI.Dialogs.WaitForm.HideWait();
			
			foreach (Sport.Championships.Competition competition in _group.Competitions)
			{
				competition.ResetCompetitorsPosition();
			}
			
			return true;
		}
		
		public void CheckAllRows()
		{
			for (int p = 0; p < _rows.Length; p++)
			{
				if (_rows[p] is PlayerRow)
				{
					PlayerRow pr = (PlayerRow) _rows[p];
					for (int c = 0; c < _group.Competitions.Count; c++)
					{
						if (pr.CompetitionsHeat[c] < 0)
							pr.CompetitionsHeat[c] = 0;
					}
				}
			}
			_grid.Refresh();
		}
		
		private int GetTeamCompetitionCount(int row, int c)
		{
			if (!(_rows[row] is Sport.Entities.Team))
				return 0;
			
			int result=0;
			
			for (int p=row+1; p<_rows.Length; p++)
			{
				if (_rows[p] is Sport.Entities.Team)
					break;
				if (_rows[p] is PlayerRow)
				{
					PlayerRow pr=(PlayerRow) _rows[p];
					if (pr.CompetitionsHeat[c] >= 0)
						result++;
				}
			}
			
			Sport.Entities.Team team=(Sport.Entities.Team) _rows[row];
			foreach (Sport.Championships.Competitor competitor in 
				_group.Competitions[c].Competitors)
			{
				if ((competitor.Player == null)||(competitor.Player.PlayerEntity == null))
				{
					int number=competitor.PlayerNumber;
					if ((number >= team.PlayerNumberFrom)&&
						(number <= team.PlayerNumberTo))
					{
						result++;
					}
				}
			}
			
			return result;
		}

		private object[] _rows;

		#region IGridSource Members

		public void SetGrid(Sport.UI.Controls.Grid grid)
		{
			_grid = grid;

			_grid.Groups.Clear();
			_grid.Groups.Add();
			_grid.Groups[0].Columns.Add(0, "קבוצה", 1);
			_grid.Groups[1].Columns.Add(0, "מספר", 20);
			_grid.Groups[1].Columns.Add(1, "שם", 120);

			int field = 2;
			int maxHeats = 0;

			foreach (Sport.Championships.Competition competition in _group.Competitions)
			{
				Sport.UI.Controls.Grid.GridColumn gridColumn = new 
					Sport.UI.Controls.Grid.GridColumn(field, competition.SportField.Name, 40);

				gridColumn.DrawCell += new Sport.UI.Controls.Grid.DrawCellEventHandler(CompetitorsDivision_DrawCell);
				gridColumn.MouseDown += new Sport.UI.Controls.Grid.CellMouseEventHandler(CompetitorsDivision_MouseDown);

				_grid.Groups[1].Columns.Add(gridColumn);

				if (competition.Heats.Count > maxHeats)
					maxHeats = competition.Heats.Count;

				field++;
			}

			_grid.Groups[1].RowHeight = (maxHeats + 1) * 14;

			_grid.ExpandedRows.Add(0, _rows.Length - 1);
		}

		public void Sort(int group, int[] columns)
		{
		}
		
		public Sport.UI.Controls.Style GetStyle(int row, int field, Sport.UI.Controls.GridDrawState state)
		{
			if (_rows[row] is Sport.Entities.Team)
			{
				bool blnTooMuch=false;
				bool blnTooLittle=false;
				for (int c=0; c<_group.Competitions.Count; c++)
				{
					if (_minCompetitionTeamCompetitors[c] < 0)
						continue;
					int competionCount=GetTeamCompetitionCount(row, c);
					if (competionCount < _minCompetitionTeamCompetitors[c])
						blnTooLittle = true;
					if (competionCount > _maxCompetitionTeamCompetitors[c])
						blnTooMuch = true;
				}
				if (blnTooMuch)
					return _tooMuchStyle;
				if (blnTooLittle)
					return _tooLittleStyle;
			}
			else
			{
				if (_rows[row] is PlayerRow)
				{
					PlayerRow pr=(PlayerRow) _rows[row];
					int count=pr.GetCompetitionsCount();
					if (count < _minCompetitorCompetitions)
						return _tooLittleStyle;
					if (count > _maxCompetitorCompetitions)
						return _tooMuchStyle;
				}
			}
			return null;
		}

		public string GetTip(int row)
		{
			return null;
		}

		public int GetGroup(int row)
		{
			if (_rows[row] is Sport.Entities.Team)
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
			return _rows.Length;
		}

		public int[] GetSort(int group)
		{
			return null;
		}

		public string GetText(int row, int field)
		{
			Sport.Entities.Team team = _rows[row] as Sport.Entities.Team;
			if (team != null)
			{
				return team.Name;
			}
			else
			{
				switch (field)
				{
					case (0):
						return ((PlayerRow) _rows[row]).Player.Number.ToString();
					case (1):
						return ((PlayerRow) _rows[row]).Player.Name;
				}
			}

			return null;
		}

		public int GetFieldCount(int row)
		{
			return 2;
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
		}

		#endregion
	}
}
