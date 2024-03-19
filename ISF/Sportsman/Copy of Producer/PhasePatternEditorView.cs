using System;

namespace Sportsman.Producer
{
	[Sport.UI.ViewPermissions(Sport.Types.Permission.Sports)]
	public class PhasePatternEditorView : Sport.UI.View
	{
		private System.Windows.Forms.Label			labelTeams;
		private System.Windows.Forms.ListBox		lbTeams;
		private Sport.UI.Controls.ThemeButton		tbAddRange;
		private Sport.UI.Controls.ThemeButton		tbCancel;
		private Sport.UI.Controls.ThemeButton		tbSave;
		private Sport.UI.Controls.CaptionBar phases;
		private System.Windows.Forms.Label labelPhaseName;
		private System.Windows.Forms.TextBox tbPhaseName;
		private Sport.UI.Controls.ThemeButton tbAddPhase;
		private Sport.UI.Controls.ThemeButton tbRemovePhase;
		private Sport.UI.Controls.BoxGrid gridGroups;
		private System.Windows.Forms.Label labelGroups;
		private System.Windows.Forms.Label labelResult;
		private Sport.UI.Controls.BoxGrid gridResult;
		private Sport.UI.Controls.GenericPanel groupCountPanel;
		private Sport.UI.Controls.ThemeButton		tbRemoveRange;

		private GroupsBoxList groupsList = null;
		private Sport.UI.Controls.ThemeButton tbDivGroups;
		private Sport.UI.Controls.ThemeButton tbClearGroups;
		private Sport.UI.Controls.ThemeButton tbDivResult;
		private Sport.UI.Controls.ThemeButton tbClearResult;
		private ResultBoxList resultList = null;

		//private Sport.Entities.PermissionServices.PermissionType _permType;

		public PhasePatternEditorView()
		{
			InitializeComponent();

			groupCountPanel.ItemsLayout = new Sport.UI.Controls.GenericDefaultLayout();

			groupCountPanel.Items.Add("בתים:",
				Sport.UI.Controls.GenericItemType.Number, 1, 
				new object[] { 1d, 1000d }, new System.Drawing.Size(50, 0));
			groupCountPanel.Items.Add("לוח משחקים:",
				Sport.UI.Controls.GenericItemType.Selection, null,
				Sport.Entities.GameBoard.Type.GetEntities(null), new System.Drawing.Size(220, 0));

			groupCountPanel.Items[0].ValueChanged += new EventHandler(GroupCountChanged);
			groupCountPanel.Items[1].ValueChanged += new EventHandler(GameBoardChanged);

			phasePattern = new Sport.Producer.PhasePattern();
			phasePattern.Changed += new EventHandler(PhasePatternChaged);

			gridGroups.HeaderSize = new System.Drawing.Size(0, 15);
			gridGroups.BoxClick += new Sport.UI.Controls.BoxGrid.BoxEventHandler(GroupClicked);

			gridResult.Direction = Sport.UI.Controls.BoxGridDirection.Vertical;
			gridResult.ScrollDirection = Sport.UI.Controls.BoxGridDirection.Vertical;
			gridResult.BoxSize = new System.Drawing.Size(70, 15);
			gridResult.HeaderSize = new System.Drawing.Size(30, 15);
			gridResult.BoxClick += new Sport.UI.Controls.BoxGrid.BoxEventHandler(ResultClicked);
		}

		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(PhasePatternEditorView));
			this.labelTeams = new System.Windows.Forms.Label();
			this.lbTeams = new System.Windows.Forms.ListBox();
			this.tbAddRange = new Sport.UI.Controls.ThemeButton();
			this.tbRemoveRange = new Sport.UI.Controls.ThemeButton();
			this.tbCancel = new Sport.UI.Controls.ThemeButton();
			this.tbSave = new Sport.UI.Controls.ThemeButton();
			this.phases = new Sport.UI.Controls.CaptionBar();
			this.labelPhaseName = new System.Windows.Forms.Label();
			this.tbPhaseName = new System.Windows.Forms.TextBox();
			this.tbAddPhase = new Sport.UI.Controls.ThemeButton();
			this.tbRemovePhase = new Sport.UI.Controls.ThemeButton();
			this.gridGroups = new Sport.UI.Controls.BoxGrid();
			this.groupCountPanel = new Sport.UI.Controls.GenericPanel();
			this.labelGroups = new System.Windows.Forms.Label();
			this.gridResult = new Sport.UI.Controls.BoxGrid();
			this.labelResult = new System.Windows.Forms.Label();
			this.tbDivGroups = new Sport.UI.Controls.ThemeButton();
			this.tbClearGroups = new Sport.UI.Controls.ThemeButton();
			this.tbDivResult = new Sport.UI.Controls.ThemeButton();
			this.tbClearResult = new Sport.UI.Controls.ThemeButton();
			this.SuspendLayout();
			// 
			// labelTeams
			// 
			this.labelTeams.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelTeams.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.labelTeams.Location = new System.Drawing.Point(582, 64);
			this.labelTeams.Name = "labelTeams";
			this.labelTeams.Size = new System.Drawing.Size(120, 23);
			this.labelTeams.TabIndex = 2;
			this.labelTeams.Text = "קבוצות";
			this.labelTeams.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lbTeams
			// 
			this.lbTeams.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.lbTeams.Location = new System.Drawing.Point(582, 88);
			this.lbTeams.Name = "lbTeams";
			this.lbTeams.Size = new System.Drawing.Size(120, 277);
			this.lbTeams.TabIndex = 3;
			this.lbTeams.DoubleClick += new System.EventHandler(this.lbTeams_DoubleClick);
			this.lbTeams.SelectedIndexChanged += new System.EventHandler(this.lbTeams_SelectedIndexChanged);
			// 
			// tbAddRange
			// 
			this.tbAddRange.Alignment = System.Drawing.StringAlignment.Center;
			this.tbAddRange.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.tbAddRange.AutoSize = true;
			this.tbAddRange.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbAddRange.Hue = 220F;
			this.tbAddRange.Image = ((System.Drawing.Image)(resources.GetObject("tbAddRange.Image")));
			this.tbAddRange.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbAddRange.ImageList = null;
			this.tbAddRange.ImageSize = new System.Drawing.Size(12, 12);
			this.tbAddRange.Location = new System.Drawing.Point(649, 375);
			this.tbAddRange.Name = "tbAddRange";
			this.tbAddRange.Saturation = 0.9F;
			this.tbAddRange.Size = new System.Drawing.Size(53, 17);
			this.tbAddRange.TabIndex = 4;
			this.tbAddRange.Text = "הוסף";
			this.tbAddRange.Transparent = System.Drawing.Color.Black;
			this.tbAddRange.Click += new System.EventHandler(this.tbAddRange_Click);
			// 
			// tbRemoveRange
			// 
			this.tbRemoveRange.Alignment = System.Drawing.StringAlignment.Center;
			this.tbRemoveRange.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.tbRemoveRange.AutoSize = true;
			this.tbRemoveRange.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbRemoveRange.Hue = 0F;
			this.tbRemoveRange.Image = ((System.Drawing.Image)(resources.GetObject("tbRemoveRange.Image")));
			this.tbRemoveRange.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbRemoveRange.ImageList = null;
			this.tbRemoveRange.ImageSize = new System.Drawing.Size(12, 12);
			this.tbRemoveRange.Location = new System.Drawing.Point(582, 375);
			this.tbRemoveRange.Name = "tbRemoveRange";
			this.tbRemoveRange.Saturation = 0.9F;
			this.tbRemoveRange.Size = new System.Drawing.Size(49, 17);
			this.tbRemoveRange.TabIndex = 5;
			this.tbRemoveRange.Text = "מחק";
			this.tbRemoveRange.Transparent = System.Drawing.Color.Black;
			this.tbRemoveRange.Click += new System.EventHandler(this.tbRemoveRange_Click);
			// 
			// tbCancel
			// 
			this.tbCancel.Alignment = System.Drawing.StringAlignment.Center;
			this.tbCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.tbCancel.AutoSize = true;
			this.tbCancel.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbCancel.Hue = 0F;
			this.tbCancel.Image = ((System.Drawing.Image)(resources.GetObject("tbCancel.Image")));
			this.tbCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbCancel.ImageList = null;
			this.tbCancel.ImageSize = new System.Drawing.Size(12, 12);
			this.tbCancel.Location = new System.Drawing.Point(72, 375);
			this.tbCancel.Name = "tbCancel";
			this.tbCancel.Saturation = 0.7F;
			this.tbCancel.Size = new System.Drawing.Size(48, 17);
			this.tbCancel.TabIndex = 6;
			this.tbCancel.Text = "בטל";
			this.tbCancel.Transparent = System.Drawing.Color.Black;
			this.tbCancel.Click += new System.EventHandler(this.tbCancel_Click);
			// 
			// tbSave
			// 
			this.tbSave.Alignment = System.Drawing.StringAlignment.Center;
			this.tbSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.tbSave.AutoSize = true;
			this.tbSave.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbSave.Hue = 120F;
			this.tbSave.Image = ((System.Drawing.Image)(resources.GetObject("tbSave.Image")));
			this.tbSave.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbSave.ImageList = null;
			this.tbSave.ImageSize = new System.Drawing.Size(12, 12);
			this.tbSave.Location = new System.Drawing.Point(16, 375);
			this.tbSave.Name = "tbSave";
			this.tbSave.Saturation = 0.5F;
			this.tbSave.Size = new System.Drawing.Size(53, 17);
			this.tbSave.TabIndex = 7;
			this.tbSave.Text = "שמור";
			this.tbSave.Transparent = System.Drawing.Color.Black;
			this.tbSave.Click += new System.EventHandler(this.tbSave_Click);
			// 
			// phases
			// 
			this.phases.Appearance = Sport.UI.Controls.CaptionBarAppearance.Buttons;
			this.phases.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.phases.Dock = System.Windows.Forms.DockStyle.Top;
			this.phases.Location = new System.Drawing.Point(0, 0);
			this.phases.Name = "phases";
			this.phases.SelectedIndex = -1;
			this.phases.SelectedItem = null;
			this.phases.Size = new System.Drawing.Size(721, 19);
			this.phases.TabIndex = 8;
			this.phases.SelectionChanged += new System.EventHandler(this.phases_SelectionChanged);
			// 
			// labelPhaseName
			// 
			this.labelPhaseName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelPhaseName.Location = new System.Drawing.Point(653, 32);
			this.labelPhaseName.Name = "labelPhaseName";
			this.labelPhaseName.Size = new System.Drawing.Size(48, 16);
			this.labelPhaseName.TabIndex = 9;
			this.labelPhaseName.Text = "שם שלב:";
			this.labelPhaseName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tbPhaseName
			// 
			this.tbPhaseName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.tbPhaseName.Location = new System.Drawing.Point(464, 28);
			this.tbPhaseName.Name = "tbPhaseName";
			this.tbPhaseName.Size = new System.Drawing.Size(176, 20);
			this.tbPhaseName.TabIndex = 10;
			this.tbPhaseName.Text = "";
			this.tbPhaseName.TextChanged += new System.EventHandler(this.tbPhaseName_TextChanged);
			// 
			// tbAddPhase
			// 
			this.tbAddPhase.Alignment = System.Drawing.StringAlignment.Center;
			this.tbAddPhase.AutoSize = true;
			this.tbAddPhase.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbAddPhase.Hue = 220F;
			this.tbAddPhase.Image = ((System.Drawing.Image)(resources.GetObject("tbAddPhase.Image")));
			this.tbAddPhase.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbAddPhase.ImageList = null;
			this.tbAddPhase.ImageSize = new System.Drawing.Size(12, 12);
			this.tbAddPhase.Location = new System.Drawing.Point(8, 32);
			this.tbAddPhase.Name = "tbAddPhase";
			this.tbAddPhase.Saturation = 0.9F;
			this.tbAddPhase.Size = new System.Drawing.Size(81, 17);
			this.tbAddPhase.TabIndex = 11;
			this.tbAddPhase.Text = "הוסף שלב";
			this.tbAddPhase.Transparent = System.Drawing.Color.Black;
			this.tbAddPhase.Click += new System.EventHandler(this.tbAddPhase_Click);
			// 
			// tbRemovePhase
			// 
			this.tbRemovePhase.Alignment = System.Drawing.StringAlignment.Center;
			this.tbRemovePhase.AutoSize = true;
			this.tbRemovePhase.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbRemovePhase.Hue = 0F;
			this.tbRemovePhase.Image = ((System.Drawing.Image)(resources.GetObject("tbRemovePhase.Image")));
			this.tbRemovePhase.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbRemovePhase.ImageList = null;
			this.tbRemovePhase.ImageSize = new System.Drawing.Size(12, 12);
			this.tbRemovePhase.Location = new System.Drawing.Point(96, 32);
			this.tbRemovePhase.Name = "tbRemovePhase";
			this.tbRemovePhase.Saturation = 0.9F;
			this.tbRemovePhase.Size = new System.Drawing.Size(77, 17);
			this.tbRemovePhase.TabIndex = 12;
			this.tbRemovePhase.Text = "מחק שלב";
			this.tbRemovePhase.Transparent = System.Drawing.Color.Black;
			this.tbRemovePhase.Click += new System.EventHandler(this.tbRemovePhase_Click);
			// 
			// gridGroups
			// 
			this.gridGroups.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.gridGroups.BoxList = null;
			this.gridGroups.BoxSize = new System.Drawing.Size(50, 100);
			this.gridGroups.Direction = Sport.UI.Controls.BoxGridDirection.Horizontal;
			this.gridGroups.HeaderSize = new System.Drawing.Size(50, 20);
			this.gridGroups.Line = 0;
			this.gridGroups.Location = new System.Drawing.Point(176, 88);
			this.gridGroups.Name = "gridGroups";
			this.gridGroups.Part = 0;
			this.gridGroups.ScrollDirection = Sport.UI.Controls.BoxGridDirection.Vertical;
			this.gridGroups.Size = new System.Drawing.Size(389, 260);
			this.gridGroups.TabIndex = 13;
			this.gridGroups.Title = null;
			// 
			// groupCountPanel
			// 
			this.groupCountPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.groupCountPanel.AutoSize = false;
			this.groupCountPanel.ItemsLayout = null;
			this.groupCountPanel.Location = new System.Drawing.Point(136, 362);
			this.groupCountPanel.Name = "groupCountPanel";
			this.groupCountPanel.Size = new System.Drawing.Size(429, 50);
			this.groupCountPanel.TabIndex = 14;
			// 
			// labelGroups
			// 
			this.labelGroups.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.labelGroups.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.labelGroups.Location = new System.Drawing.Point(176, 64);
			this.labelGroups.Name = "labelGroups";
			this.labelGroups.Size = new System.Drawing.Size(389, 23);
			this.labelGroups.TabIndex = 15;
			this.labelGroups.Text = "בתים";
			this.labelGroups.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// gridResult
			// 
			this.gridResult.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left)));
			this.gridResult.BoxList = null;
			this.gridResult.BoxSize = new System.Drawing.Size(50, 100);
			this.gridResult.Direction = Sport.UI.Controls.BoxGridDirection.Horizontal;
			this.gridResult.HeaderSize = new System.Drawing.Size(50, 20);
			this.gridResult.Line = 0;
			this.gridResult.Location = new System.Drawing.Point(8, 88);
			this.gridResult.Name = "gridResult";
			this.gridResult.Part = 0;
			this.gridResult.ScrollDirection = Sport.UI.Controls.BoxGridDirection.Vertical;
			this.gridResult.Size = new System.Drawing.Size(160, 260);
			this.gridResult.TabIndex = 16;
			this.gridResult.Title = null;
			// 
			// labelResult
			// 
			this.labelResult.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.labelResult.Location = new System.Drawing.Point(8, 64);
			this.labelResult.Name = "labelResult";
			this.labelResult.Size = new System.Drawing.Size(160, 23);
			this.labelResult.TabIndex = 17;
			this.labelResult.Text = "תוצאה";
			this.labelResult.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// tbDivGroups
			// 
			this.tbDivGroups.Alignment = System.Drawing.StringAlignment.Center;
			this.tbDivGroups.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.tbDivGroups.AutoSize = true;
			this.tbDivGroups.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbDivGroups.Hue = 200F;
			this.tbDivGroups.Image = ((System.Drawing.Image)(resources.GetObject("tbDivGroups.Image")));
			this.tbDivGroups.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbDivGroups.ImageList = null;
			this.tbDivGroups.ImageSize = new System.Drawing.Size(12, 12);
			this.tbDivGroups.Location = new System.Drawing.Point(469, 348);
			this.tbDivGroups.Name = "tbDivGroups";
			this.tbDivGroups.Saturation = 0.5F;
			this.tbDivGroups.Size = new System.Drawing.Size(47, 17);
			this.tbDivGroups.TabIndex = 18;
			this.tbDivGroups.Text = "חלק";
			this.tbDivGroups.Transparent = System.Drawing.Color.Black;
			this.tbDivGroups.Click += new System.EventHandler(this.tbDivGroups_Click);
			// 
			// tbClearGroups
			// 
			this.tbClearGroups.Alignment = System.Drawing.StringAlignment.Center;
			this.tbClearGroups.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.tbClearGroups.AutoSize = true;
			this.tbClearGroups.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbClearGroups.Hue = 40F;
			this.tbClearGroups.Image = ((System.Drawing.Image)(resources.GetObject("tbClearGroups.Image")));
			this.tbClearGroups.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbClearGroups.ImageList = null;
			this.tbClearGroups.ImageSize = new System.Drawing.Size(12, 12);
			this.tbClearGroups.Location = new System.Drawing.Point(517, 348);
			this.tbClearGroups.Name = "tbClearGroups";
			this.tbClearGroups.Saturation = 0.7F;
			this.tbClearGroups.Size = new System.Drawing.Size(46, 17);
			this.tbClearGroups.TabIndex = 19;
			this.tbClearGroups.Text = "נקה";
			this.tbClearGroups.Transparent = System.Drawing.Color.Black;
			this.tbClearGroups.Click += new System.EventHandler(this.tbClearGroups_Click);
			// 
			// tbDivResult
			// 
			this.tbDivResult.Alignment = System.Drawing.StringAlignment.Center;
			this.tbDivResult.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.tbDivResult.AutoSize = true;
			this.tbDivResult.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbDivResult.Hue = 200F;
			this.tbDivResult.Image = ((System.Drawing.Image)(resources.GetObject("tbDivResult.Image")));
			this.tbDivResult.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbDivResult.ImageList = null;
			this.tbDivResult.ImageSize = new System.Drawing.Size(12, 12);
			this.tbDivResult.Location = new System.Drawing.Point(72, 348);
			this.tbDivResult.Name = "tbDivResult";
			this.tbDivResult.Saturation = 0.5F;
			this.tbDivResult.Size = new System.Drawing.Size(47, 17);
			this.tbDivResult.TabIndex = 20;
			this.tbDivResult.Text = "חלק";
			this.tbDivResult.Transparent = System.Drawing.Color.Black;
			this.tbDivResult.Click += new System.EventHandler(this.tbDivResult_Click);
			// 
			// tbClearResult
			// 
			this.tbClearResult.Alignment = System.Drawing.StringAlignment.Center;
			this.tbClearResult.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.tbClearResult.AutoSize = true;
			this.tbClearResult.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbClearResult.Hue = 40F;
			this.tbClearResult.Image = ((System.Drawing.Image)(resources.GetObject("tbClearResult.Image")));
			this.tbClearResult.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbClearResult.ImageList = null;
			this.tbClearResult.ImageSize = new System.Drawing.Size(12, 12);
			this.tbClearResult.Location = new System.Drawing.Point(120, 348);
			this.tbClearResult.Name = "tbClearResult";
			this.tbClearResult.Saturation = 0.7F;
			this.tbClearResult.Size = new System.Drawing.Size(46, 17);
			this.tbClearResult.TabIndex = 21;
			this.tbClearResult.Text = "נקה";
			this.tbClearResult.Transparent = System.Drawing.Color.Black;
			this.tbClearResult.Click += new System.EventHandler(this.tbClearResult_Click);
			// 
			// PhasePatternEditorView
			// 
			this.Controls.Add(this.tbDivResult);
			this.Controls.Add(this.tbClearResult);
			this.Controls.Add(this.tbDivGroups);
			this.Controls.Add(this.tbClearGroups);
			this.Controls.Add(this.labelResult);
			this.Controls.Add(this.gridResult);
			this.Controls.Add(this.labelGroups);
			this.Controls.Add(this.gridGroups);
			this.Controls.Add(this.groupCountPanel);
			this.Controls.Add(this.tbAddPhase);
			this.Controls.Add(this.tbRemovePhase);
			this.Controls.Add(this.tbPhaseName);
			this.Controls.Add(this.labelPhaseName);
			this.Controls.Add(this.phases);
			this.Controls.Add(this.tbCancel);
			this.Controls.Add(this.tbSave);
			this.Controls.Add(this.labelTeams);
			this.Controls.Add(this.lbTeams);
			this.Controls.Add(this.tbAddRange);
			this.Controls.Add(this.tbRemoveRange);
			this.Name = "PhasePatternEditorView";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.Size = new System.Drawing.Size(721, 413);
			this.ResumeLayout(false);

		}

		private void ResetGroupsBoxSize()
		{
			if (groupsList != null)
			{
				gridGroups.BoxSize = new System.Drawing.Size(50, 
					(groupsList.MaxGroupTeams + 1) * 15);
			}
		}


		private Sport.Entities.PhasePattern	entityPhasePattern;
		private Sport.Producer.PhasePattern				phasePattern;
		private Sport.Producer.PhasePattern.Phase		currentPhase;
		private Sport.Producer.PhasePattern.PhaseItem	currentPhaseItem;

		public override void Open()
		{
			/*
			if (Sport.Entities.User.ViewPermission(this.GetType().Name) == 
				Sport.Entities.PermissionServices.PermissionType.None)
			{
				throw new Exception("Can't edit phase patterns: Permission Denied");
			}
			*/
			
			string pid = State["pattern"];

			Title = "תבנית שלבים";

			currentPhase = null;

			if (pid != null)
			{
				int pattern = Int32.Parse(pid);

				Sport.Data.Entity entity = Sport.Entities.PhasePattern.Type.Lookup(pattern);

				if (entity != null)
				{
					entityPhasePattern = new Sport.Entities.PhasePattern(entity);

					Title = "תבנית שלבים - " + entity.Name;

					if (phasePattern.Load(entity.Id))
					{
						foreach (Sport.Producer.PhasePattern.Phase phase in phasePattern.Phases)
						{
							phases.Items.Add(phase.Name);
						}
					}
				}
				else
					entityPhasePattern = null;
			}

			tbSave.Enabled = false;
			tbCancel.Enabled = false;

			SelectPhase(null);

			/*
			_permType = Sport.Entities.User.ViewPermission(this.GetType().Name);
			if (_permType == 
				Sport.Entities.PermissionServices.PermissionType.ReadOnly)
			{
				this.tbAddPhase.Visible = false;
				this.tbAddRange.Visible = false;
				this.tbClearGroups.Visible = false;
				this.tbClearResult.Visible = false;
				this.tbDivGroups.Visible = false;
				this.tbDivResult.Visible = false;
				this.tbRemovePhase.Visible = false;
				this.tbRemoveRange.Visible = false;
				this.tbSave.Visible = false;
			}
			*/

			base.Open ();
		}

		private void SelectPhase(Sport.Producer.PhasePattern.Phase phase)
		{
			lbTeams.Items.Clear();

			int itemIndex = -1;
			if (currentPhase != null && currentPhaseItem != null)
			{
				itemIndex = currentPhase.IndexOf(currentPhaseItem);
			}

			currentPhase = null;

			if (phase == null)
			{
				tbPhaseName.Text = null;
				tbPhaseName.Enabled = false;
				tbAddRange.Enabled = false;
				tbRemoveRange.Enabled = false;
				itemIndex = -1;
			}
			else
			{
				tbPhaseName.Enabled = true;
				tbAddRange.Enabled = true;
				tbRemoveRange.Enabled = true;

				foreach (Sport.Producer.PhasePattern.PhaseItem item in phase)
				{
					lbTeams.Items.Add(item);
				}

				if (phase.Count - 1 > itemIndex)
					itemIndex = phase.Count - 1;

				tbPhaseName.Text = phase.Name;
			}

			currentPhase = phase;
			
			SelectPhaseItem(itemIndex == -1 ? null : currentPhase[itemIndex]);
		}

		private void SelectPhaseItem(Sport.Producer.PhasePattern.PhaseItem phaseItem)
		{
			if (currentPhaseItem != phaseItem)
			{
				currentPhaseItem = null;

				groupsList = null;
				resultList = null;

				if (phaseItem == null)
				{
					groupCountPanel.Enabled = false;
				}
				else
				{
					groupCountPanel.Enabled = true;
					if (phaseItem.Groups.Count > 0)
						groupCountPanel.Items[0].Value = phaseItem.Groups.Count;

					if (phaseItem.GameBoard != null)
						groupCountPanel.Items[1].Value = phaseItem.GameBoard.Entity;
					else
						groupCountPanel.Items[1].Value = null;

					groupsList = new GroupsBoxList(phaseItem);
					resultList = new ResultBoxList(phaseItem);
				}

				currentPhaseItem = phaseItem;

				lbTeams.SelectedItem = currentPhaseItem;
				gridGroups.BoxList = groupsList;
				gridResult.BoxList = resultList;
				ResetGroupsBoxSize();
			}
		}

		public override void Close()
		{
			SelectPhase(null);
			phases.Items.Clear();
			base.Close ();
		}

		private void PhasePatternChaged(object sender, EventArgs e)
		{
			tbCancel.Enabled = true;
			tbSave.Enabled = true;
			gridGroups.Refresh();
			gridResult.Refresh();
		}

		private void tbAddPhase_Click(object sender, System.EventArgs e)
		{
			/*
			if (_permType !=
				Sport.Entities.PermissionServices.PermissionType.Full)
			{
				return;
			}
			*/
			
			Sport.UI.Dialogs.GenericEditDialog ged = 
				new Sport.UI.Dialogs.GenericEditDialog("בחר שלב");

			ged.Items.Add(Sport.UI.Controls.GenericItemType.Number, 
				phasePattern.Phases.Count + 1, new object[] { 1d, (double) phasePattern.Phases.Count + 1 });

			if (ged.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				int index = (int) (double) ged.Items[0].Value;
				Sport.Producer.PhasePattern.Phase phase = 
					new Sport.Producer.PhasePattern.Phase();
				phase.Name = "שלב " + index.ToString();

				phasePattern.Phases.Insert(index - 1, phase);

				phases.Items.Insert(index - 1, phase.Name);
				phases.SelectedIndex = index - 1;
				SelectPhase(phase);
			}
		}

		private void tbRemovePhase_Click(object sender, System.EventArgs e)
		{
			if (currentPhase != null)
			{
				int index = phasePattern.Phases.IndexOf(currentPhase);
				if (index >= 0)
				{
					phasePattern.Phases.RemoveAt(index);
					phases.Items.RemoveAt(index);
					phases.SelectedIndex = -1;
					SelectPhase(null);
				}
			}
		}

		private void tbPhaseName_TextChanged(object sender, System.EventArgs e)
		{
			if (currentPhase != null)
			{
				currentPhase.Name = tbPhaseName.Text;
				phases.SelectedItem.Text = tbPhaseName.Text;
			}
		}

		private void phases_SelectionChanged(object sender, System.EventArgs e)
		{
			int index = phases.SelectedIndex;
			if (index < 0)
			{
				SelectPhase(null);
			}
			else
			{
				SelectPhase(phasePattern.Phases[index]);
			}
		}

		private void GroupCountChanged(object sender, EventArgs e)
		{
			if (groupsList != null)
			{
				groupsList.Groups = (int) (double) groupCountPanel.Items[0].Value;
			}
		}

		private void lbTeams_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			Sport.Producer.PhasePattern.PhaseItem phaseItem =
				lbTeams.SelectedItem as Sport.Producer.PhasePattern.PhaseItem;
			SelectPhaseItem(phaseItem);
		
		}

		private void tbAddRange_Click(object sender, System.EventArgs e)
		{
			if (currentPhase != null)
			{
				string text = "";
				if (Sport.UI.Dialogs.TextEditDialog.EditText("בחר תחום קבוצות", ref text))
				{
					Sport.Common.RangeArray.Range range = Sport.Common.RangeArray.Range.Parse(text);
	                
					if (range.First < 1)
					{
						System.Windows.Forms.MessageBox.Show("תחום קבוצות חייב להיות גדול מ 0");
					}
					else if (!currentPhase.Fit(range.First, range.Last))
					{
						System.Windows.Forms.MessageBox.Show("תחום קבוצות מתנגש עם תחום אחר");
					}
					else
					{
						Sport.Producer.PhasePattern.PhaseItem item = 
							new Sport.Producer.PhasePattern.PhaseItem(range.First, range.Last);
						int n = currentPhase.Add(item);
						lbTeams.Items.Insert(n, currentPhase[n]);
						Sport.Producer.PhasePattern.Group group = new Sport.Producer.PhasePattern.Group();
						group.Name = "א";
						currentPhase[n].Groups.Add(group);
					}
				}
			}
		}

		private void tbRemoveRange_Click(object sender, System.EventArgs e)
		{
			if (currentPhase != null)
			{
				int index = lbTeams.SelectedIndex;
				if (index >= 0)
				{
					Sport.Producer.PhasePattern.PhaseItem item = (Sport.Producer.PhasePattern.PhaseItem) lbTeams.Items[index];
					currentPhase.Remove(item);
					lbTeams.Items.RemoveAt(index);
					SelectPhaseItem(null);
				}
			}
		}

		private void tbSave_Click(object sender, System.EventArgs e)
		{
			if (phasePattern.Save(entityPhasePattern.Id))
			{
				tbSave.Enabled = false;
				tbCancel.Enabled = false;
			}
		}

		private void tbCancel_Click(object sender, System.EventArgs e)
		{
			SelectPhase(null);
			phases.Items.Clear();
			phases.SelectedIndex = -1;
			if (phasePattern.Load(entityPhasePattern.Id))
			{
				foreach (Sport.Producer.PhasePattern.Phase phase in phasePattern.Phases)
				{
					phases.Items.Add(phase.Name);
				}
			}

			tbSave.Enabled = false;
			tbCancel.Enabled = false;
		}

		private void GroupClicked(object sender, Sport.UI.Controls.BoxGrid.BoxEventArgs e)
		{
			if (e.SubPart == -1)
			{
				groupsList.EditGroupName(e.Box);
			}
			else
			{
				groupsList.EditGroupPosition(e.Box, e.SubPart);
			}

			ResetGroupsBoxSize();
		}

		private void ResultClicked(object sender, Sport.UI.Controls.BoxGrid.BoxEventArgs e)
		{
			resultList.EditPosition(e.Box);
		}

		private void tbClearGroups_Click(object sender, System.EventArgs e)
		{
			if (currentPhaseItem != null)
			{
				groupsList.Clear();
				ResetGroupsBoxSize();
			}
		}

		private void tbClearResult_Click(object sender, System.EventArgs e)
		{
			if (currentPhaseItem != null)
			{
				resultList.Clear();
			}
		}

		private void tbDivGroups_Click(object sender, System.EventArgs e)
		{
			/*
			if (_permType !=
				Sport.Entities.PermissionServices.PermissionType.Full)
			{
				return;
			}
			*/
			
			if (currentPhaseItem != null)
			{
				Sport.UI.Dialogs.GenericEditDialog ged = new Sport.UI.Dialogs.GenericEditDialog("בחר צורת חלוקה");
				Sport.Data.LookupItem[] items = 
					new Sport.Data.LookupItem[] {
													new Sport.Data.LookupItem((int) Sport.Producer.PhasePattern.GenerateType.Position, "מלא מיקום"),
													new Sport.Data.LookupItem((int) Sport.Producer.PhasePattern.GenerateType.Group, "מלא בית")
												};

				ged.Items.Add(new Sport.UI.Controls.GenericItem(Sport.UI.Controls.GenericItemType.Selection, null, items));
				ged.Items[0].Nullable = false;

				if (ged.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					Sport.Data.LookupItem item = ged.Items[0].Value as Sport.Data.LookupItem;

					if (item != null)
					{
						groupsList.Generate((Sport.Producer.PhasePattern.GenerateType) item.Id);
						ResetGroupsBoxSize();
					}
				}
			}
		}

		private void tbDivResult_Click(object sender, System.EventArgs e)
		{
			/*
			if (_permType !=
				Sport.Entities.PermissionServices.PermissionType.Full)
			{
				return;
			}
			*/
			
			if (currentPhaseItem != null)
			{
				Sport.UI.Dialogs.GenericEditDialog ged = new Sport.UI.Dialogs.GenericEditDialog("בחר צורת חלוקה");
				Sport.Data.LookupItem[] items = 
					new Sport.Data.LookupItem[] {
													new Sport.Data.LookupItem((int) Sport.Producer.PhasePattern.GenerateType.Position, "מלא מיקום"),
													new Sport.Data.LookupItem((int) Sport.Producer.PhasePattern.GenerateType.Group, "מלא בית")
												};

				ged.Items.Add(new Sport.UI.Controls.GenericItem(Sport.UI.Controls.GenericItemType.Selection, null, items));
				ged.Items[0].Nullable = false;

				if (ged.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					Sport.Data.LookupItem item = ged.Items[0].Value as Sport.Data.LookupItem;

					if (item != null)
					{
						resultList.Generate((Sport.Producer.PhasePattern.GenerateType) item.Id);
					}
				}
			}
		}

		private void GameBoardChanged(object sender, EventArgs e)
		{
			if (currentPhaseItem != null)
			{
				Sport.Data.Entity entity = groupCountPanel.Items[1].Value as Sport.Data.Entity;
				if (entity == null)
					currentPhaseItem.GameBoard = null;
				else
					currentPhaseItem.GameBoard = new Sport.Entities.GameBoard(entity);
			}
		}

		private void lbTeams_DoubleClick(object sender, System.EventArgs e)
		{
			if (currentPhase != null)
			{
				int index = lbTeams.SelectedIndex;
				Sport.Producer.PhasePattern.PhaseItem item = 
					(Sport.Producer.PhasePattern.PhaseItem) lbTeams.Items[index];
				string text = item.Min.ToString() + "-" + item.Max.ToString();
				if (Sport.UI.Dialogs.TextEditDialog.EditText("בחר תחום קבוצות", ref text))
				{
					Sport.Common.RangeArray.Range range = Sport.Common.RangeArray.Range.Parse(text);
	                
					if (range.First < 1)
					{
						System.Windows.Forms.MessageBox.Show("תחום קבוצות חייב להיות גדול מ 0");
					}
					else if ((index > 0 && currentPhase[index - 1].Max >= range.First) ||
						(index < currentPhase.Count - 1 && currentPhase[index + 1].Min <= range.Last))
					{
						System.Windows.Forms.MessageBox.Show("תחום קבוצות מתנגש עם תחום אחר");
					}
					else
					{
						if (range.First > item.Max)
						{
							item.Max = range.Last;
							item.Min = range.First;
						}
						else
						{
							item.Min = range.First;
							item.Max = range.Last;
						}

						lbTeams.Items.RemoveAt(index);
						lbTeams.Items.Insert(index, item);
						lbTeams.SelectedIndex = index;
					}
				}
			}
		}

	}

	public class GroupsBoxList : Sport.UI.Controls.IBoxList
	{
		private Sport.Producer.PhasePattern.PhaseItem _phaseItem;
		public Sport.Producer.PhasePattern.PhaseItem PhaseItem
		{
			get { return _phaseItem; }
		}

		public GroupsBoxList(Sport.Producer.PhasePattern.PhaseItem phaseItem)
		{
			_phaseItem = phaseItem;
			CalculateMaxGroupTeams();
		}

		public int Groups
		{
			get { return _phaseItem.Groups.Count; }
			set
			{
				while (value < _phaseItem.Groups.Count)
					_phaseItem.Groups.RemoveAt(_phaseItem.Groups.Count - 1);

				while (value > _phaseItem.Groups.Count)
				{
					Sport.Producer.PhasePattern.Group group = new Sport.Producer.PhasePattern.Group();
					group.Name = Sport.Common.LetterNumber.ToString(_phaseItem.Groups.Count + 1);
					_phaseItem.Groups.Add(group);
				}

				if (ListChanged != null)
					ListChanged(this, EventArgs.Empty);
			}
		}

		public void Clear()
		{
			foreach (Sport.Producer.PhasePattern.Group group in _phaseItem.Groups)
			{
				while (group.Size > 0)
					group.RemoveAt(0);
			}
			CalculateMaxGroupTeams();
		}

		public void Generate(Sport.Producer.PhasePattern.GenerateType type)
		{
			_phaseItem.GenerateGroups(type);
			CalculateMaxGroupTeams();
		}

		public void EditGroupName(int group)
		{
			string name = _phaseItem.Groups[group].Name;

			if (Sport.UI.Dialogs.TextEditDialog.EditText("עריכת שם בית", ref name))
			{
				_phaseItem.Groups[group].Name = name;
				if (ValueChanged != null)
					ValueChanged(this, EventArgs.Empty);
			}
		}

		public void EditGroupPosition(int group, int position)
		{
			/*
			string strTypeName=typeof(PhasePatternEditorView).Name;
			if (Sport.Entities.User.ViewPermission(strTypeName) != 
				Sport.Entities.PermissionServices.PermissionType.Full)
			{
				return;
			}
			*/
			
			bool[] positions = new bool[_phaseItem.Max];
			int count = _phaseItem.Max;

			foreach (Sport.Producer.PhasePattern.Group g in _phaseItem.Groups)
			{
				for (int n = 0; n < g.Size; n++)
				{
					positions[g[n] - 1] = true;
					count--;
				}
			}

			Sport.Producer.PhasePattern.Group gr = _phaseItem.Groups[group];
			if (gr.Size > position)
			{
				count++;
			}
			object[] teams = new object[count];
			object team = null;
			int i = 0;
			if (gr.Size > position)
			{
				teams[0] = gr[position];
				team = teams[0];
				i++;
			}
			for (int n = 0; n < _phaseItem.Max; n++)
			{
				if (!positions[n])
				{
					teams[i] = n + 1;
					i++;
				}
			}

			int target = gr.Size > position ? position : gr.Size;

			string title = _phaseItem.Groups[group].Name + " - מיקום " +
				(target + 1).ToString();

			Sport.UI.Dialogs.GenericEditDialog ged = 
				new Sport.UI.Dialogs.GenericEditDialog(title);

			ged.Items.Add(Sport.UI.Controls.GenericItemType.Selection,
				team, teams);

			if (ged.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				team = ged.Items[0].Value;
				if (team == null)
				{
					if (target < gr.Size)
						gr.RemoveAt(target);
				}
				else
				{
					if (target == gr.Size)
						gr.Add((int) team);
					else
						gr[target] = (int) team;
				}

				CalculateMaxGroupTeams();

				if (ValueChanged != null)
					ValueChanged(this, EventArgs.Empty);
			}
		}

		private int maxGroupTeams;
		public int MaxGroupTeams
		{
			get { return maxGroupTeams; }
		}
		private void CalculateMaxGroupTeams()
		{
			maxGroupTeams = 0;
			foreach (Sport.Producer.PhasePattern.Group group in _phaseItem.Groups)
			{
				if (group.Size > maxGroupTeams)
					maxGroupTeams = group.Size;
			}
		}

		#region IBoxList Members

		public event EventHandler ListChanged;

		public string GetBoxTitle(int box)
		{
			return _phaseItem.Groups[box].Name;
		}

		public event EventHandler ValueChanged;

		public string GetPartTitle(int part)
		{
			return null;
		}

		public int GetPartCount()
		{
			return 1;
		}

		public int GetPartSize(int part)
		{
			return 0;
		}

		public object GetBoxItem(int box, int part, int subpart)
		{
			if (subpart >= _phaseItem.Groups[box].Size)
				return null;
			return _phaseItem.Groups[box][subpart];
		}

		public int GetBoxCount()
		{
			return _phaseItem.Groups.Count;
		}

		public int GetSubPartCount(int part)
		{
			return maxGroupTeams + 1;
		}

		#endregion
	}

	public class ResultBoxList : Sport.UI.Controls.IBoxList
	{
		private Sport.UI.Dialogs.GenericEditDialog positionDialog;
		private int editPosition;
		
		private void PositionGroupChanged(object sender, EventArgs e)
		{
			RefreshPositions();
		}

		public void Clear()
		{
			for (int n = 0; n < _phaseItem.Max; n++)
				_phaseItem[n] = null;

			if (ValueChanged != null)
				ValueChanged(this, EventArgs.Empty);
		}

		public void Generate(Sport.Producer.PhasePattern.GenerateType type)
		{
			_phaseItem.GenerateResult(type);

			if (ValueChanged != null)
				ValueChanged(this, EventArgs.Empty);
		}

		private void RefreshGroups(Sport.Producer.PhasePattern.Group group)
		{
			int count = _phaseItem.Groups.Count;
			int[] groupSet = new int [count];
			int gi;

			for (int n = 0; n < _phaseItem.Max; n++)
			{
				if (_phaseItem[n] != null)
				{
					gi = _phaseItem.Groups.IndexOf(_phaseItem[n].Group);

					if (gi != -1)
					{
						groupSet[gi]++;
						if (groupSet[gi] == _phaseItem[n].Group.Size && _phaseItem[n].Group != group)
						{
							count--;
						}
					}
				}
			}

			for (int i = 0; i < _phaseItem.Groups.Count; i++)
			{
				if (_phaseItem.Groups[i].Size == 0)
					count--;
			}

			object[] groups = new object[count];
			gi = 0;
			for (int i = 0; i < _phaseItem.Groups.Count; i++)
			{
				if (groupSet[i] < _phaseItem.Groups[i].Size || _phaseItem.Groups[i] == group)
				{
					groups[gi] = _phaseItem.Groups[i];
				}
			}

			positionDialog.Items[0].Values = groups;
			positionDialog.Items[0].Value = group;
		}

		private void RefreshPositions()
		{
			Sport.Producer.PhasePattern.Group group = positionDialog.Items[0].Value as Sport.Producer.PhasePattern.Group;
			if (group == null)
			{
				positionDialog.Items[1].Values = null;
			}
			else
			{
				int gi = _phaseItem.Groups.IndexOf(group);
				bool[] positions = new bool[group.Size];
				int count = group.Size;

				for (int n = 0; n < _phaseItem.Max; n++)
				{
					if (_phaseItem[n] != null && _phaseItem[n].Group == group)
					{
						positions[_phaseItem[n].Position] = true;
						count--;
					}
				}

				if (_phaseItem[editPosition] != null && _phaseItem[editPosition].Group == group)
				{
					count++;
				}

				object[] pos = new object[count];
				object sel = null;				
				int i = 0;
				if (_phaseItem[editPosition] != null && _phaseItem[editPosition].Group == group)
				{
					pos[0] = _phaseItem[editPosition].Position + 1;
					sel = pos[0];
					i++;
				}

				for (int p = 0; p < group.Size; p++)
				{
					if (!positions[p])
					{
						pos[i] = p + 1;
						i++;
					}
				}

				positionDialog.Items[1].Values = pos;
				positionDialog.Items[1].Value = sel;
			}
		}


		private Sport.Producer.PhasePattern.PhaseItem _phaseItem;
		public Sport.Producer.PhasePattern.PhaseItem PhaseItem
		{
			get { return _phaseItem; }
		}

		public ResultBoxList(Sport.Producer.PhasePattern.PhaseItem phaseItem)
		{
			_phaseItem = phaseItem;

			positionDialog = new Sport.UI.Dialogs.GenericEditDialog(null);
			positionDialog.Items.Add("בית:", Sport.UI.Controls.GenericItemType.Selection);
			positionDialog.Items.Add("מיקום:", Sport.UI.Controls.GenericItemType.Selection);

			positionDialog.Items[0].ValueChanged += new EventHandler(PositionGroupChanged);
		}

		public void EditPosition(int position)
		{
			/*
			string strTypeName=typeof(PhasePatternEditorView).Name;
			if (Sport.Entities.User.ViewPermission(strTypeName) != 
				Sport.Entities.PermissionServices.PermissionType.Full)
			{
				return;
			}
			*/
			
			editPosition = position;
			positionDialog.Title = "מיקום " + (position + 1).ToString();

			Sport.Producer.PhasePattern.Team team =
				_phaseItem[position];

			RefreshGroups(team == null ? null : team.Group);

			RefreshPositions();

			if (positionDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				Sport.Producer.PhasePattern.Group group = 
					positionDialog.Items[0].Value as Sport.Producer.PhasePattern.Group;
				object pos = positionDialog.Items[1].Value;

				if (group != null && pos is int)
				{
					_phaseItem[editPosition] = new Sport.Producer.PhasePattern.Team(group, (int) pos - 1);
				}
				else
				{
					_phaseItem[editPosition] = null;
				}

				if (ValueChanged != null)
					ValueChanged(this, EventArgs.Empty);
			}
		}

		#region IBoxList Members

		public event EventHandler ListChanged;

		public string GetBoxTitle(int box)
		{
			return (box + 1).ToString();
		}

		// ListChanged event gives warning, so...
		private void justtouseunusedlistchanged()
		{
			EventHandler h = ListChanged;
		}

		public event EventHandler ValueChanged;

		public string GetPartTitle(int part)
		{
			return "קבוצה";
		}

		public int GetPartCount()
		{
			return 1;
		}

		public int GetPartSize(int part)
		{
			return 0;
		}

		public object GetBoxItem(int box, int part, int subpart)
		{
			Sport.Producer.PhasePattern.Team team = _phaseItem[box];
			if (team == null)
				return null;
			return team.Group.Name + " - " +
				(team.Position + 1).ToString();
		}

		public int GetBoxCount()
		{
			return _phaseItem.Max;
		}

		public int GetSubPartCount(int part)
		{
			return 1;
		}

		#endregion
	}
}
