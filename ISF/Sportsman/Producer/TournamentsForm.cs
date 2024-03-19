using System;

namespace Sportsman.Producer
{
	public class TournamentsForm : System.Windows.Forms.Form,
		Sport.UI.Controls.IGridSource
	{
		private System.Windows.Forms.Button btnOK;
		private Sport.UI.Controls.Grid gridMatches;
		private Sport.UI.Controls.ThemeButton tbNextTournament;
		private Sport.UI.Controls.ThemeButton tbPrevTournament;
		private System.Windows.Forms.Label labelTournament;
		private Sport.UI.Controls.TextControl tcNumber;
		private System.Windows.Forms.Label labelTime;
		private System.Windows.Forms.DateTimePicker dtpTime;
		private Sport.UI.Controls.NullComboBox cbCourt;
		private System.Windows.Forms.Label labelCourt;
		private System.Windows.Forms.Label labelFacility;
		private Sport.UI.Controls.NullComboBox cbFacility;
		private System.Windows.Forms.Button btnCancel;

		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(TournamentsForm));
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.gridMatches = new Sport.UI.Controls.Grid();
			this.tbNextTournament = new Sport.UI.Controls.ThemeButton();
			this.tbPrevTournament = new Sport.UI.Controls.ThemeButton();
			this.labelTournament = new System.Windows.Forms.Label();
			this.tcNumber = new Sport.UI.Controls.TextControl();
			this.labelTime = new System.Windows.Forms.Label();
			this.cbCourt = new Sport.UI.Controls.NullComboBox();
			this.dtpTime = new System.Windows.Forms.DateTimePicker();
			this.labelCourt = new System.Windows.Forms.Label();
			this.labelFacility = new System.Windows.Forms.Label();
			this.cbFacility = new Sport.UI.Controls.NullComboBox();
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
			// gridMatches
			// 
			this.gridMatches.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.gridMatches.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.gridMatches.Editable = true;
			this.gridMatches.GroupingStyle = Sport.UI.Controls.GridGroupingStyle.Grid;
			this.gridMatches.HorizontalLines = true;
			this.gridMatches.Location = new System.Drawing.Point(8, 56);
			this.gridMatches.Name = "gridMatches";
			this.gridMatches.SelectedIndex = -1;
			this.gridMatches.SelectionMode = System.Windows.Forms.SelectionMode.One;
			this.gridMatches.ShowCheckBoxes = false;
			this.gridMatches.ShowRowNumber = false;
			this.gridMatches.Size = new System.Drawing.Size(678, 376);
			this.gridMatches.TabIndex = 26;
			this.gridMatches.TabStop = false;
			this.gridMatches.VerticalLines = true;
			this.gridMatches.VisibleRow = 0;
			// 
			// tbNextTournament
			// 
			this.tbNextTournament.Alignment = System.Drawing.StringAlignment.Center;
			this.tbNextTournament.AutoSize = true;
			this.tbNextTournament.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbNextTournament.Hue = 200F;
			this.tbNextTournament.Image = ((System.Drawing.Image)(resources.GetObject("tbNextTournament.Image")));
			this.tbNextTournament.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbNextTournament.ImageList = null;
			this.tbNextTournament.ImageSize = new System.Drawing.Size(12, 12);
			this.tbNextTournament.Location = new System.Drawing.Point(8, 32);
			this.tbNextTournament.Name = "tbNextTournament";
			this.tbNextTournament.Saturation = 0.5F;
			this.tbNextTournament.Size = new System.Drawing.Size(49, 17);
			this.tbNextTournament.TabIndex = 0;
			this.tbNextTournament.Text = "הבא";
			this.tbNextTournament.Transparent = System.Drawing.Color.Black;
			this.tbNextTournament.Click += new System.EventHandler(this.tbNextTournament_Click);
			// 
			// tbPrevTournament
			// 
			this.tbPrevTournament.Alignment = System.Drawing.StringAlignment.Center;
			this.tbPrevTournament.AutoSize = true;
			this.tbPrevTournament.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbPrevTournament.Hue = 200F;
			this.tbPrevTournament.Image = ((System.Drawing.Image)(resources.GetObject("tbPrevTournament.Image")));
			this.tbPrevTournament.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbPrevTournament.ImageList = null;
			this.tbPrevTournament.ImageSize = new System.Drawing.Size(12, 12);
			this.tbPrevTournament.Location = new System.Drawing.Point(64, 32);
			this.tbPrevTournament.Name = "tbPrevTournament";
			this.tbPrevTournament.Saturation = 0.5F;
			this.tbPrevTournament.Size = new System.Drawing.Size(52, 17);
			this.tbPrevTournament.TabIndex = 1;
			this.tbPrevTournament.Text = "קודם";
			this.tbPrevTournament.Transparent = System.Drawing.Color.Black;
			this.tbPrevTournament.Click += new System.EventHandler(this.tbPrevTournament_Click);
			// 
			// labelTournament
			// 
			this.labelTournament.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelTournament.Location = new System.Drawing.Point(638, 11);
			this.labelTournament.Name = "labelTournament";
			this.labelTournament.Size = new System.Drawing.Size(48, 16);
			this.labelTournament.TabIndex = 28;
			this.labelTournament.Text = "טורניר:";
			// 
			// tcNumber
			// 
			this.tcNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.tcNumber.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.tcNumber.Controller = null;
			this.tcNumber.Location = new System.Drawing.Point(569, 6);
			this.tcNumber.Name = "tcNumber";
			this.tcNumber.ReadOnly = false;
			this.tcNumber.SelectionLength = 0;
			this.tcNumber.SelectionStart = 0;
			this.tcNumber.ShowSpin = true;
			this.tcNumber.Size = new System.Drawing.Size(64, 21);
			this.tcNumber.TabIndex = 29;
			this.tcNumber.Value = "";
			this.tcNumber.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tcNumber_KeyPress);
			// 
			// labelTime
			// 
			this.labelTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelTime.Location = new System.Drawing.Point(646, 35);
			this.labelTime.Name = "labelTime";
			this.labelTime.Size = new System.Drawing.Size(40, 16);
			this.labelTime.TabIndex = 30;
			this.labelTime.Text = "מועד:";
			// 
			// cbCourt
			// 
			this.cbCourt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cbCourt.Location = new System.Drawing.Point(248, 30);
			this.cbCourt.Name = "cbCourt";
			this.cbCourt.Size = new System.Drawing.Size(168, 22);
			this.cbCourt.Sorted = true;
			this.cbCourt.TabIndex = 32;
			this.cbCourt.SelectedIndexChanged += new System.EventHandler(this.cbCourt_SelectedIndexChanged);
			// 
			// dtpTime
			// 
			this.dtpTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.dtpTime.CustomFormat = "dd/MM/yyyy HH:mm";
			this.dtpTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dtpTime.Location = new System.Drawing.Point(497, 30);
			this.dtpTime.Name = "dtpTime";
			this.dtpTime.ShowCheckBox = true;
			this.dtpTime.Size = new System.Drawing.Size(136, 21);
			this.dtpTime.TabIndex = 33;
			this.dtpTime.ValueChanged += new System.EventHandler(this.dtpTime_ValueChanged);
			// 
			// labelCourt
			// 
			this.labelCourt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelCourt.Location = new System.Drawing.Point(424, 35);
			this.labelCourt.Name = "labelCourt";
			this.labelCourt.Size = new System.Drawing.Size(40, 16);
			this.labelCourt.TabIndex = 34;
			this.labelCourt.Text = "מגרש:";
			// 
			// labelFacility
			// 
			this.labelFacility.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelFacility.Location = new System.Drawing.Point(424, 11);
			this.labelFacility.Name = "labelFacility";
			this.labelFacility.Size = new System.Drawing.Size(40, 16);
			this.labelFacility.TabIndex = 36;
			this.labelFacility.Text = "מתקן:";
			// 
			// cbFacility
			// 
			this.cbFacility.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cbFacility.Location = new System.Drawing.Point(248, 6);
			this.cbFacility.Name = "cbFacility";
			this.cbFacility.Size = new System.Drawing.Size(168, 22);
			this.cbFacility.Sorted = true;
			this.cbFacility.TabIndex = 35;
			this.cbFacility.SelectedIndexChanged += new System.EventHandler(this.cbFacility_SelectedIndexChanged);
			// 
			// TournamentsForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.ClientSize = new System.Drawing.Size(696, 472);
			this.Controls.Add(this.labelFacility);
			this.Controls.Add(this.cbFacility);
			this.Controls.Add(this.labelCourt);
			this.Controls.Add(this.dtpTime);
			this.Controls.Add(this.cbCourt);
			this.Controls.Add(this.labelTime);
			this.Controls.Add(this.tbNextTournament);
			this.Controls.Add(this.tbPrevTournament);
			this.Controls.Add(this.tcNumber);
			this.Controls.Add(this.labelTournament);
			this.Controls.Add(this.gridMatches);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Name = "TournamentsForm";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "הכנסת תוצאות";
			this.ResumeLayout(false);

		}
	

		private Sport.Championships.MatchGroup _group = null;

		public TournamentsForm(Sport.Championships.MatchGroup group)
		{
			_group = group;

			InitializeComponent();

			AddFacilities();

			tcNumber.Controller = new Sport.UI.Controls.NumberController(1, 999);
			tcNumber.LostFocus += new EventHandler(tcNumber_LostFocus);

			if (group.Phase.Championship.Editing)
			{
				btnCancel.Visible = false;
				btnOK.Text = "סגור";
			}
			else
			{
				_group.EditGroup();
			}



			gridMatches.GroupingStyle = Sport.UI.Controls.GridGroupingStyle.List;
			gridMatches.Groups.Clear();
			gridMatches.Groups.Add();
			gridMatches.Groups.Add();
			gridMatches.Groups[0].Columns.Add(0, "סיבוב - מחזור", 1);
			gridMatches.Groups[1].Columns.Add(0, "טורניר", 1);
			gridMatches.Groups[2].Columns.Add(0, "מספר", 20);
			gridMatches.Groups[2].Columns.Add(1, "קבוצה מארחת", 80);
			gridMatches.Groups[2].Columns.Add(2, "קבוצה אורחת", 80);
			gridMatches.Groups[2].Columns.Add(3, "מועד", 60);
			gridMatches.Groups[2].Columns.Add(4, "מתקן", 60);

			Rebuild();

			gridMatches.Source = this;

			gridMatches.SelectionChanged += new EventHandler(gridMatches_SelectionChanged);
			gridMatches.DoubleClick += new EventHandler(gridMatches_DoubleClick);

			Text = _group.Name + " - טורנירים";

			_cycle = null;
			_tournament = null;
			_match = null;

			OnSelectionChanged();
		}

		protected override void OnClosed(EventArgs e)
		{
			if (_group.Editing)
				_group.CancelGroup();

			base.OnClosed (e);
		}



		private void btnOK_Click(object sender, System.EventArgs e)
		{
			if (_group.Editing)
			{
				//
				// Resorting matches in cycles according to tournament and time
				//
				System.Collections.IComparer comparer = new Sport.Championships.MatchTournamentComparer();
				Sport.Championships.Match[] matches = new Sport.Championships.Match[0];
				int count = 0;
				foreach (Sport.Championships.Round round in _group.Rounds)
				{
					foreach (Sport.Championships.Cycle cycle in round.Cycles)
					{
						count = cycle.Matches.Count;
						if (count > matches.Length)
							matches = new Sport.Championships.Match[count];
						for (int n = count - 1; n >= 0; n--)
						{
							matches[n] = cycle.Matches[n];
							cycle.Matches.RemoveAt(n);
						}
                        
						Array.Sort(matches, 0, count, comparer);

						for (int n = 0; n < count; n++)
							cycle.Matches.Add(matches[n]);
					}
				}

				if (_group.SaveGroup())
				{
					DialogResult = System.Windows.Forms.DialogResult.OK;
					Close();
				}
				else
				{
					Sport.UI.MessageBox.Show("כשלון בשמירת בית");
				}
			}
			else
			{
				DialogResult = System.Windows.Forms.DialogResult.OK;
				Close();
			}
		}

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			_group.CancelGroup();
			DialogResult = System.Windows.Forms.DialogResult.Cancel;
			Close();
		}

		private Sport.Championships.Match		_match;
		private Sport.Championships.Tournament	_tournament;
		private Sport.Championships.Cycle		_cycle;

		private void gridMatches_SelectionChanged(object sender, EventArgs e)
		{
			ResetSelection();
		}

		private void ResetSelection()
		{
			_cycle = null;
			_tournament = null;
			_match = null;

			if (gridMatches.Selection.Size == 1)
			{
				int row = gridMatches.Selection.Rows[0];
				_cycle = _rows[row] as Sport.Championships.Cycle;
				if (_cycle == null)
				{
					_tournament = _rows[row] as Sport.Championships.Tournament;
					if (_tournament == null)
					{
						NoTournament nt = _rows[row] as NoTournament;
						if (nt != null)
						{
							_cycle = nt.Cycle;
						}
						else
						{
							_match = (Sport.Championships.Match) _rows[row];
							_tournament = _match.Tournament == -1 ? null : _match.Cycle.Tournaments[_match.Tournament];
							_cycle = _match.Cycle;
						}
					}
					else
					{
						_cycle = _tournament.Cycle;
					}
				}
			}

			OnSelectionChanged();
		}

		private void gridMatches_DoubleClick(object sender, EventArgs e)
		{
		}

		private bool setting = false;
		private void SetTimeFacility(DateTime time, Sport.Entities.Facility facility,
			Sport.Entities.Court court)
		{
			setting = true;

			if (time == DateTime.MinValue)
			{
				dtpTime.Checked = false;
			}
			else
			{
				dtpTime.Checked = true;
				dtpTime.Value = time;
			}

			cbFacility.SelectedItem = facility;
			cbCourt.SelectedItem = court;
			
			setting = false;
		}

		private void AddFacilities()
		{
			Sport.UI.Dialogs.WaitForm.ShowWait("טוען מתקנים אנא המתן...");

			Sport.Entities.Championship cc = 
				_group.Phase.Championship.ChampionshipCategory.Championship;

			//add the facilities:
			cbFacility.Items.Add(Sport.UI.Controls.NullComboBox.Null);
			cbFacility.Items.AddRange(cc.GetFacilities());

			Sport.UI.Dialogs.WaitForm.HideWait();
		}

		private void SetCourts()
		{
			cbCourt.Items.Clear();

			Sport.Entities.Facility facility = cbFacility.SelectedItem as Sport.Entities.Facility;
			if (facility != null)
			{
				Sport.Championships.MatchChampionship champ = _group.Phase.Championship;
				Sport.Entities.Sport sport = champ.ChampionshipCategory.Championship.Sport;
				cbCourt.Items.AddRange(facility.GetCourts(sport));
			}

			cbCourt.Items.Add(Sport.UI.Controls.NullComboBox.Null);
		}

		private void OnSelectionChanged()
		{
			tbNextTournament.Enabled = _match != null && 
				(_match.Tournament < _match.Cycle.Tournaments.Count - 1 || _cycle.GetNextCycle() != null);
			tbPrevTournament.Enabled = _match != null && 
				(_match.Tournament > -1 || _cycle.GetPrevCycle() != null);

			if (_tournament == null)
			{
				tcNumber.Enabled = false;
				tcNumber.Value = null;

				if (_match != null)
				{
					SetTimeFacility(_match.Time, _match.Facility, _match.Court);
				}
			}
			else
			{
				tcNumber.Enabled = true;
				tcNumber.Value = _tournament.Number;
				
				if (_match == null)
				{
					SetTimeFacility(_tournament.Time, _tournament.Facility, _tournament.Court);
				}
				else
				{
					SetTimeFacility(_match.Time, _match.Facility, _match.Court);
				}
			}

			dtpTime.Enabled = _match != null || _tournament != null;
			cbFacility.Enabled = _match != null || _tournament != null;
			cbCourt.Enabled = _match != null || _tournament != null;
		}

		#region IGridSource Members

		private class NoTournament
		{
			private Sport.Championships.Cycle _cycle;
			public Sport.Championships.Cycle Cycle
			{
				get { return _cycle; }
			}

			public NoTournament(Sport.Championships.Cycle cycle)
			{
				_cycle = cycle;
			}
		}

		private object[] _rows;

		private void Rebuild()
		{
			int vr = gridMatches.VisibleRow;
			System.Collections.ArrayList rows = new System.Collections.ArrayList();

			foreach (Sport.Championships.Round round in _group.Rounds)
			{
				foreach (Sport.Championships.Cycle cycle in round.Cycles)
				{
					rows.Add(cycle);

					foreach (Sport.Championships.Match match in cycle.Matches)
					{
						rows.Add(match);
					}
				}
			}

			// Adding tournaments
			int start = 0;
			int end;
			int tournament;
			System.Collections.IComparer comparer = new Sport.Championships.MatchTournamentComparer();
			Sport.Championships.Cycle currentCycle = null;

			while (start < rows.Count)
			{
				while (start < rows.Count && (rows[start] is Sport.Championships.Cycle))
				{
					currentCycle = (Sport.Championships.Cycle) rows[start];
					start++;
				}

				if (start < rows.Count)
				{
					end = start;
					while (end < rows.Count && rows[end] is Sport.Championships.Match)
						end++;

					rows.Sort(start, end - start, comparer);

					tournament = -999;

					while (start < end)
					{
						Sport.Championships.Match match = (Sport.Championships.Match) rows[start];
						if (tournament == match.Tournament)
						{
							start++;
						}
						else
						{
							tournament = match.Tournament;
							if (tournament == -1)
								rows.Insert(start, new NoTournament(currentCycle));
							else
								rows.Insert(start, currentCycle.Tournaments[tournament]);
							start += 2;
							end++;
						}
					}
				}
			}

			_rows = rows.ToArray();

			if (_rows.Length > 0)
				gridMatches.ExpandedRows.Add(0, _rows.Length - 1);
			gridMatches.RefreshSource();

			gridMatches.VisibleRow = vr;
		}

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
			if (_rows[row] is Sport.Championships.Cycle)
				return 0;
			if (_rows[row] is Sport.Championships.Tournament ||
				_rows[row] is NoTournament)
				return 1;
			return 2;
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
			Sport.Championships.Cycle cycle = _rows[row] as Sport.Championships.Cycle;
			if (cycle != null)
			{
				return cycle.Round.Name + " - " + cycle.Name;
			}

			Sport.Championships.Tournament tournament = _rows[row] as Sport.Championships.Tournament;
			if (tournament != null)
			{
				string stour = "טורניר " + tournament.Number.ToString();
				if (tournament.Time != DateTime.MinValue)
				{
					stour += " - " + tournament.Time.ToString("g");
				}
				if (tournament.Facility != null)
				{
					stour += " - " + tournament.Facility.Name;
				}
				if (tournament.Court != null)
				{
					stour += " - " + tournament.Court.Name;
				}
				return stour;
			}

			if (_rows[row] is NoTournament)
			{
				return "לא בטורניר";
			}

			Sport.Championships.Match match = (Sport.Championships.Match) _rows[row];

			switch (field)
			{
				case (0):
					return match.Number.ToString();
				case (1):
					return match.GetTeamAName();
				case (2):
					return match.GetTeamBName();
				case (3):
					if (match.Time == DateTime.MinValue)
						return null;
					return match.Time.ToString("g");
				case (4):
					if (match.Court != null)
					{
						return match.Facility.Name + " - " + match.Court.Name;
					}
					if (match.Facility != null)
					{
						return match.Facility.Name;
					}
					return null;
			}

			return null;
		}

		public int GetFieldCount(int row)
		{
			if (_rows[row] is Sport.Championships.Match)
				return 3;
			return 1;
		}

		#endregion


		private void tbNextTournament_Click(object sender, System.EventArgs e)
		{
			if (_match != null)
			{
				int tournament = _match.Tournament;
				int matchCount = 0;
				if (tournament >= 0)
					matchCount = _cycle.Tournaments[tournament].GetMatchCount();

				// Checking if in last tournament
				if (tournament != -1 && _match.Tournament == _cycle.Tournaments.Count - 1)
				{
					// Checking if last match in tournament
					if (matchCount == 1)
					{
						Sport.Championships.Cycle cycle = _cycle.GetNextCycle();
						if (cycle == null)
							return ;

						_cycle.Matches.RemoveAt(_match.Index);
						cycle.Matches.Insert(0, _match);
						_match.Tournament = -1;
					}
					else
					{
						// Moving match to a new tournament
						_match.Tournament = _cycle.Tournaments.Add(new Sport.Championships.Tournament(_cycle.Round.Group.GetMaxTournamentNumber() + 1));
					}
				}
				else
				{
					_match.Tournament++;
				}

				if (matchCount == 1)
					_cycle.Tournaments.RemoveAt(tournament);
				
				Rebuild();
				int row = Array.IndexOf(_rows, _match);
				gridMatches.SelectRow(row);
				gridMatches.ScrollToRow(row);
			}
		}

		private void tbPrevTournament_Click(object sender, System.EventArgs e)
		{
			if (_match != null)
			{
				int tournament = _match.Tournament;
				int matchCount = 0;
				if (tournament >= 0)
					matchCount = _cycle.Tournaments[tournament].GetMatchCount();

				// Checking if in no tournament
				if (tournament == -1)
				{
					Sport.Championships.Cycle cycle = _cycle.GetPrevCycle();
					if (cycle == null)
						return ;

					_cycle.Matches.RemoveAt(_match.Index);
					cycle.Matches.Add(_match);
					_match.Tournament = cycle.Tournaments.Count - 1;
				}
				else
				{
					_match.Tournament--;
				}

				if (matchCount == 1)
					_cycle.Tournaments.RemoveAt(tournament);
				
				Rebuild();
				int row = Array.IndexOf(_rows, _match);
				gridMatches.SelectRow(row);
				gridMatches.ScrollToRow(row);
			}
		}

		private void tcNumber_LostFocus(object sender, EventArgs e)
		{
			if (_tournament != null)
			{
				ResetTournamentNumber();
			}
		}

		private void ResetTournamentNumber()
		{
			int tn = (int) (double) tcNumber.Value;
			if (tn != _tournament.Number)
			{
				if (_cycle.Round.Group.GetTournamentByNumber(tn) == null)
				{
					_tournament.Number = tn;
					gridMatches.Refresh();
				}
				else
				{
					tcNumber.Value = _tournament.Number;
				}
			}
		}

		private void tcNumber_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			if (e.KeyChar == 13) // enter
			{
				ResetTournamentNumber();
			}
		}

		private DateTime GetTime()
		{
			if (dtpTime.Checked)
				return dtpTime.Value;

			return DateTime.MinValue;
		}

		private void dtpTime_ValueChanged(object sender, System.EventArgs e)
		{
			if (!setting)
			{
				if (_match != null)
				{
					_match.Time = GetTime();
				}
				else if (_tournament != null)
				{
					_tournament.Time = GetTime();
				}

				gridMatches.Refresh();
			}
		}

		private void cbFacility_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			SetCourts();

			if (!setting)
			{
				if (_match != null)
				{
					_match.Facility = cbFacility.SelectedItem as Sport.Entities.Facility;
					_match.Court = cbCourt.SelectedItem as Sport.Entities.Court;
				}
				else if (_tournament != null)
				{
					_tournament.Facility = cbFacility.SelectedItem as Sport.Entities.Facility;
					_tournament.Court = cbCourt.SelectedItem as Sport.Entities.Court;
				}

				gridMatches.Refresh();
			}
		}

		private void cbCourt_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (!setting)
			{
				if (_match != null)
				{
					_match.Court = cbCourt.SelectedItem as Sport.Entities.Court;
				}
				else if (_tournament != null)
				{
					_tournament.Court = cbCourt.SelectedItem as Sport.Entities.Court;
				}

				gridMatches.Refresh();
			}
		}
	}
}
