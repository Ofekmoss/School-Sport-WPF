using System;

namespace Sportsman.Producer
{
	public class PhaseBuildForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label labelPattern;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private Sport.UI.Controls.Grid gridTeams;
		private Sport.UI.Controls.ThemeButton tbDown;
		private Sport.UI.Controls.ThemeButton tbUp;
		private Sport.UI.Controls.ThemeButton tbSwitch;
		private Sport.UI.Controls.NullComboBox cbPattern;
		
		private Sport.UI.Controls.Grid	gridGroups;
		private PhaseGroupsGridSource	sourceGroups;
		
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(PhaseBuildForm));
			this.labelPattern = new System.Windows.Forms.Label();
			this.cbPattern = new Sport.UI.Controls.NullComboBox();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.gridGroups = new Sport.UI.Controls.Grid();
			this.gridTeams = new Sport.UI.Controls.Grid();
			this.tbDown = new Sport.UI.Controls.ThemeButton();
			this.tbUp = new Sport.UI.Controls.ThemeButton();
			this.tbSwitch = new Sport.UI.Controls.ThemeButton();
			this.SuspendLayout();
			// 
			// labelPattern
			// 
			this.labelPattern.Location = new System.Drawing.Point(320, 16);
			this.labelPattern.Name = "labelPattern";
			this.labelPattern.Size = new System.Drawing.Size(80, 16);
			this.labelPattern.TabIndex = 0;
			this.labelPattern.Text = "תבנית שלבים:";
			// 
			// cbPattern
			// 
			this.cbPattern.Location = new System.Drawing.Point(134, 10);
			this.cbPattern.Name = "cbPattern";
			this.cbPattern.Size = new System.Drawing.Size(176, 22);
			this.cbPattern.Sorted = true;
			this.cbPattern.TabIndex = 1;
			this.cbPattern.SelectedIndexChanged += new System.EventHandler(this.cbPattern_SelectedIndexChanged);
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnOK.Enabled = false;
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
			// gridGroups
			// 
			this.gridGroups.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left)));
			this.gridGroups.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.gridGroups.Editable = true;
			this.gridGroups.ExpandOnDoubleClick = true;
			this.gridGroups.GroupingStyle = Sport.UI.Controls.GridGroupingStyle.Grid;
			this.gridGroups.HeaderHeight = 17;
			this.gridGroups.HorizontalLines = true;
			this.gridGroups.Location = new System.Drawing.Point(8, 98);
			this.gridGroups.Name = "gridGroups";
			this.gridGroups.SelectedIndex = -1;
			this.gridGroups.SelectionMode = System.Windows.Forms.SelectionMode.One;
			this.gridGroups.ShowCheckBoxes = false;
			this.gridGroups.ShowRowNumber = false;
			this.gridGroups.Size = new System.Drawing.Size(200, 250);
			this.gridGroups.TabIndex = 25;
			this.gridGroups.VerticalLines = true;
			this.gridGroups.VisibleRow = 0;
			// 
			// gridTeams
			// 
			this.gridTeams.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.gridTeams.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.gridTeams.Editable = true;
			this.gridTeams.ExpandOnDoubleClick = true;
			this.gridTeams.GroupingStyle = Sport.UI.Controls.GridGroupingStyle.Grid;
			this.gridTeams.HeaderHeight = 17;
			this.gridTeams.HorizontalLines = true;
			this.gridTeams.Location = new System.Drawing.Point(216, 48);
			this.gridTeams.Name = "gridTeams";
			this.gridTeams.SelectedIndex = -1;
			this.gridTeams.SelectionMode = System.Windows.Forms.SelectionMode.One;
			this.gridTeams.ShowCheckBoxes = false;
			this.gridTeams.ShowRowNumber = false;
			this.gridTeams.Size = new System.Drawing.Size(200, 380);
			this.gridTeams.TabIndex = 26;
			this.gridTeams.VerticalLines = true;
			this.gridTeams.VisibleRow = 0;
			this.gridTeams.SelectionChanged += new System.EventHandler(this.gridTeams_SelectionChanged);
			// 
			// tbDown
			// 
			this.tbDown.Alignment = System.Drawing.StringAlignment.Center;
			this.tbDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.tbDown.AutoSize = true;
			this.tbDown.Enabled = false;
			this.tbDown.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbDown.Hue = 200F;
			this.tbDown.Image = ((System.Drawing.Image)(resources.GetObject("tbDown.Image")));
			this.tbDown.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbDown.ImageList = null;
			this.tbDown.ImageSize = new System.Drawing.Size(12, 12);
			this.tbDown.Location = new System.Drawing.Point(312, 440);
			this.tbDown.Name = "tbDown";
			this.tbDown.Saturation = 0.5F;
			this.tbDown.Size = new System.Drawing.Size(51, 17);
			this.tbDown.TabIndex = 27;
			this.tbDown.Text = "הורד";
			this.tbDown.Transparent = System.Drawing.Color.Black;
			this.tbDown.Click += new System.EventHandler(this.tbDown_Click);
			// 
			// tbUp
			// 
			this.tbUp.Alignment = System.Drawing.StringAlignment.Center;
			this.tbUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.tbUp.AutoSize = true;
			this.tbUp.Enabled = false;
			this.tbUp.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbUp.Hue = 200F;
			this.tbUp.Image = ((System.Drawing.Image)(resources.GetObject("tbUp.Image")));
			this.tbUp.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbUp.ImageList = null;
			this.tbUp.ImageSize = new System.Drawing.Size(12, 12);
			this.tbUp.Location = new System.Drawing.Point(368, 440);
			this.tbUp.Name = "tbUp";
			this.tbUp.Saturation = 0.5F;
			this.tbUp.Size = new System.Drawing.Size(47, 17);
			this.tbUp.TabIndex = 28;
			this.tbUp.Text = "עלה";
			this.tbUp.Transparent = System.Drawing.Color.Black;
			this.tbUp.Click += new System.EventHandler(this.tbUp_Click);
			// 
			// tbSwitch
			// 
			this.tbSwitch.Alignment = System.Drawing.StringAlignment.Center;
			this.tbSwitch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.tbSwitch.AutoSize = true;
			this.tbSwitch.Enabled = false;
			this.tbSwitch.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbSwitch.Hue = 200F;
			this.tbSwitch.Image = ((System.Drawing.Image)(resources.GetObject("tbSwitch.Image")));
			this.tbSwitch.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbSwitch.ImageList = null;
			this.tbSwitch.ImageSize = new System.Drawing.Size(12, 12);
			this.tbSwitch.Location = new System.Drawing.Point(216, 440);
			this.tbSwitch.Name = "tbSwitch";
			this.tbSwitch.Saturation = 0.5F;
			this.tbSwitch.Size = new System.Drawing.Size(56, 17);
			this.tbSwitch.TabIndex = 29;
			this.tbSwitch.Text = "החלף";
			this.tbSwitch.Transparent = System.Drawing.Color.Black;
			this.tbSwitch.Click += new System.EventHandler(this.tbSwitch_Click);
			// 
			// PhaseBuildForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.ClientSize = new System.Drawing.Size(426, 472);
			this.Controls.Add(this.tbSwitch);
			this.Controls.Add(this.tbDown);
			this.Controls.Add(this.tbUp);
			this.Controls.Add(this.gridTeams);
			this.Controls.Add(this.gridGroups);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.cbPattern);
			this.Controls.Add(this.labelPattern);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "PhaseBuildForm";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.ResumeLayout(false);

		}
	
		private Sport.Championships.MatchChampionship _championship;
		public Sport.Championships.MatchChampionship Championship
		{
			get { return _championship; }
		}

		private Sport.Producer.PhasePattern _phasePattern;
		public Sport.Producer.PhasePattern PhasePattern
		{
			get { return _phasePattern; }
		}

		public PhaseBuildForm(Sport.Championships.MatchChampionship championship, int phase)
		{
			InitializeComponent();

			groupsTeam = -1;
			teamsTeam = -1;
			tbDown.Enabled = false;
			tbUp.Enabled = false;
			tbSwitch.Enabled = false;

			_phasePattern = null;

			Sport.Entities.Team[] teams = new Sport.Entities.Team[championship.Teams.Count];
			for (int t = 0; t < championship.Teams.Count; t++)
			{
				teams[t] = championship.Teams[t];
			}

			_championship = new Sport.Championships.MatchChampionship(null, teams);
			_championship.Edit();

			gridTeams.Columns.Add(0, "מיקום", 20);
			gridTeams.Columns.Add(1, "קבוצה", 80);
			gridTeams.Source = new TeamsGridSource(_championship);

			sourceGroups = new PhaseGroupsGridSource();
			sourceGroups.SelectionChanged += new EventHandler(GroupSourceSelectionChanged);
			
			gridGroups.Columns.Add(0, "בתים", 1);
			gridGroups.Source = sourceGroups;
			
			cbPattern.Text = "בחר תבנית";
			
			Sport.Data.Entity[] patterns = Sport.Entities.PhasePattern.Type.GetEntities(null);

			foreach (Sport.Data.Entity entity in patterns)
			{
				Sport.Entities.PhasePattern pattern = new Sport.Entities.PhasePattern(entity);
				if (pattern.GetRangeArray()[teams.Length])
					cbPattern.Items.Add(entity);
			}
		}

		public static bool BuildChampionship(Sport.Championships.MatchChampionship championship, int phase)
		{
			PhaseBuildForm pbf = new PhaseBuildForm(championship, phase);

			if (pbf.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				// Resetting teams order
				championship.Teams.Clear();
				foreach (Sport.Entities.Team team in pbf.Championship.Teams)
					championship.Teams.Add(team);

				// Building championship
				pbf.PhasePattern.BuildChampionship(championship);
				return true;
			}
			
			return false;
		}

		public static bool BuildChampionship(Sport.Championships.MatchChampionship championship)
		{
			return BuildChampionship(championship, -1);
		}

		private void btnOK_Click(object sender, System.EventArgs e)
		{
			DialogResult = System.Windows.Forms.DialogResult.OK;
			Close();
		}

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			DialogResult = System.Windows.Forms.DialogResult.Cancel;
			Close();
		}

		private void Rebuild()
		{
			if (_phasePattern != null)
			{
				if (_phasePattern.BuildChampionship(_championship))
				{
					if (_championship.Phases.Count > 0)
						sourceGroups.Phase = _championship.Phases[0];
					else
						sourceGroups.Phase = null;

					btnOK.Enabled = true;
				}
				else
				{
					Sport.UI.MessageBox.Show("כשלון בהפעלת תבנית שלבים", System.Windows.Forms.MessageBoxIcon.Error);
					sourceGroups.Phase = null;
					btnOK.Enabled = false;
				}
			}
			else
			{
				btnOK.Enabled = false;
			}
		}

		private void tbUp_Click(object sender, System.EventArgs e)
		{
			int index = gridTeams.SelectedIndex;
			if (index > 0)
			{
				gridTeams.Selection.Set(index - 1, index - 1);
				((TeamsGridSource) gridTeams.Source).SwitchTeams(index, index - 1);
				gridTeams.SelectedIndex = index - 1;
				Rebuild();
				tbUp.Enabled = index > 1;
			}
		}

		private void tbDown_Click(object sender, System.EventArgs e)
		{
			int index = gridTeams.SelectedIndex;
			if (index >= 0 && index < _championship.Teams.Count - 1)
			{
				gridTeams.Selection.Set(index + 1, index + 1);
				((TeamsGridSource) gridTeams.Source).SwitchTeams(index + 1, index);
				gridTeams.SelectedIndex = index + 1;
				Rebuild();
				tbDown.Enabled = index < _championship.Teams.Count - 2;
			}
		}

		private void tbSwitch_Click(object sender, System.EventArgs e)
		{
			if (groupsTeam != -1 && teamsTeam != -1 && groupsTeam != teamsTeam)
			{
				((TeamsGridSource) gridTeams.Source).SwitchTeams(groupsTeam, teamsTeam);
				Rebuild();
			}			
		}


		private void cbPattern_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			Sport.Data.Entity entity = cbPattern.SelectedItem as Sport.Data.Entity;
			if (entity == null)
			{
				_phasePattern = null;
			}
			else
			{
				_phasePattern = new Sport.Producer.PhasePattern();
				if (!_phasePattern.Load(entity.Id))
					_phasePattern = null;
				Rebuild();
			}
		}

		private int groupsTeam;
		private int teamsTeam;

		private void gridTeams_SelectionChanged(object sender, System.EventArgs e)
		{
			teamsTeam = gridTeams.SelectedIndex;
			tbUp.Enabled = teamsTeam > 0;
			tbDown.Enabled = teamsTeam >= 0 && teamsTeam < _championship.Teams.Count - 1;
			tbSwitch.Enabled = groupsTeam != -1 && teamsTeam != -1 && groupsTeam != teamsTeam;
		}

		private void GroupSourceSelectionChanged(object sender, EventArgs e)
		{
			groupsTeam = -1;
			if (sourceGroups.SelectedTeam != null)
			{
                   groupsTeam = ((TeamsGridSource) gridTeams.Source).GetTeamIndex(
					   sourceGroups.SelectedTeam.Id);
			}

			tbSwitch.Enabled = groupsTeam != -1 && teamsTeam != -1 && groupsTeam != teamsTeam;
		}
	}

	public class TeamsGridSource : Sport.UI.Controls.IGridSource
	{
		private Sport.UI.Controls.Grid _grid;
		private Sport.Championships.MatchChampionship _championship;

		public TeamsGridSource(Sport.Championships.MatchChampionship championship)
		{
			_championship = championship;
		}

		public void SwitchTeams(int team1, int team2)
		{
			Sport.Entities.Team team = _championship.Teams[team1];
			_championship.Teams[team1] = _championship.Teams[team2];
			_championship.Teams[team2] = team;
			_grid.RefreshSource();
		}

		public int GetTeamIndex(int teamId)
		{
			for (int t = 0; t < _championship.Teams.Count; t++)
			{
				if (_championship.Teams[t].Id == teamId)
					return t;
			}

			return -1;
		}

		#region IGridSource Members

		public void SetGrid(Sport.UI.Controls.Grid grid)
		{
			_grid = grid;
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
			return _championship.Teams.Count;
		}

		public int[] GetSort(int group)
		{
			return null;
		}

		public string GetText(int row, int field)
		{
			switch (field)
			{
				case (0):
					return (row + 1).ToString();
				case (1):
					return _championship.Teams[row].Name;
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
