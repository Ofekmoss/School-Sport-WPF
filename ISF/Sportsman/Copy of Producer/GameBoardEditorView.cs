using System;

namespace Sportsman.Producer
{
	[Sport.UI.ViewPermissions(Sport.Types.Permission.Sports)]
	public class GameBoardEditorView : Sport.UI.View
	{
		private Sport.UI.Controls.GenericPanel		boardSizePanel;

		private System.Windows.Forms.Label			labelTeams;
		private System.Windows.Forms.ListBox		lbTeams;
		private Sport.UI.Controls.ThemeButton		tbAddRange;
		private Sport.UI.Controls.ThemeButton		tbCancel;
		private Sport.UI.Controls.ThemeButton		tbSave;
		private System.Windows.Forms.Label			labelMatches;
		private Sport.UI.Controls.BoxGrid			grid;
		private System.Windows.Forms.Label			labelTournament;
		private Sport.UI.Controls.BoxGrid			gridTournament;
		private Sport.UI.Controls.ThemeButton		tbRemoveRange;
		private Sport.UI.Controls.ThemeButton tbCopy;

		//private Sport.Entities.PermissionServices.PermissionType _permType;

		public GameBoardEditorView()
		{
			InitializeComponent();

			boardSizePanel.ItemsLayout = new Sport.UI.Controls.GenericDefaultLayout();

			boardSizePanel.Items.Add("סיבובים:",
				Sport.UI.Controls.GenericItemType.Number, 1, 
				new object[] { 1d, 100d }, new System.Drawing.Size(40, 0));
			boardSizePanel.Items.Add("מחזורים:",
				Sport.UI.Controls.GenericItemType.Number, 1, 
				new object[] { 1d, 100d }, new System.Drawing.Size(40, 0));
			boardSizePanel.Items.Add("התמודדויות:",
				Sport.UI.Controls.GenericItemType.Number, 1, 
				new object[] { 1d, 100d }, new System.Drawing.Size(40, 0));
			boardSizePanel.Items.Add("קבוצות:",
				Sport.UI.Controls.GenericItemType.Number, 1, 
				new object[] { 2d, 100d }, new System.Drawing.Size(40, 0));

			boardSizePanel.Items[0].ValueChanged += new EventHandler(RoundsChanged);
			boardSizePanel.Items[1].ValueChanged += new EventHandler(CyclesChanged);
			boardSizePanel.Items[2].ValueChanged += new EventHandler(MatchesChanged);
			boardSizePanel.Items[3].ValueChanged += new EventHandler(GamesChanged);

			grid.BoxClick += new Sport.UI.Controls.BoxGrid.BoxEventHandler(MatchClick);
			grid.PartClick += new Sport.UI.Controls.BoxGrid.BoxEventHandler(RoundClick);

			gridTournament.HeaderSize = new System.Drawing.Size(0, gridTournament.HeaderSize.Height);
			gridTournament.BoxClick += new Sport.UI.Controls.BoxGrid.BoxEventHandler(GameClick);

			gameBoard = new Sport.Producer.GameBoard();
			gameBoard.Changed += new EventHandler(GameBoardChanged);
		}

		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(GameBoardEditorView));
			this.boardSizePanel = new Sport.UI.Controls.GenericPanel();
			this.labelTeams = new System.Windows.Forms.Label();
			this.lbTeams = new System.Windows.Forms.ListBox();
			this.tbAddRange = new Sport.UI.Controls.ThemeButton();
			this.tbRemoveRange = new Sport.UI.Controls.ThemeButton();
			this.tbCancel = new Sport.UI.Controls.ThemeButton();
			this.tbSave = new Sport.UI.Controls.ThemeButton();
			this.labelMatches = new System.Windows.Forms.Label();
			this.grid = new Sport.UI.Controls.BoxGrid();
			this.labelTournament = new System.Windows.Forms.Label();
			this.gridTournament = new Sport.UI.Controls.BoxGrid();
			this.tbCopy = new Sport.UI.Controls.ThemeButton();
			this.SuspendLayout();
			// 
			// boardSizePanel
			// 
			this.boardSizePanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.boardSizePanel.AutoSize = false;
			this.boardSizePanel.ItemsLayout = null;
			this.boardSizePanel.Location = new System.Drawing.Point(16, 336);
			this.boardSizePanel.Name = "boardSizePanel";
			this.boardSizePanel.Size = new System.Drawing.Size(562, 32);
			this.boardSizePanel.TabIndex = 1;
			// 
			// labelTeams
			// 
			this.labelTeams.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelTeams.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.labelTeams.Location = new System.Drawing.Point(582, 16);
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
			this.lbTeams.Location = new System.Drawing.Point(582, 40);
			this.lbTeams.Name = "lbTeams";
			this.lbTeams.Size = new System.Drawing.Size(120, 329);
			this.lbTeams.TabIndex = 3;
			this.lbTeams.DoubleClick += new System.EventHandler(this.lbTeams_DoubleClick);
			this.lbTeams.SelectedIndexChanged += new System.EventHandler(this.TeamsChanged);
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
			// labelMatches
			// 
			this.labelMatches.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.labelMatches.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.labelMatches.Location = new System.Drawing.Point(148, 16);
			this.labelMatches.Name = "labelMatches";
			this.labelMatches.Size = new System.Drawing.Size(426, 23);
			this.labelMatches.TabIndex = 8;
			this.labelMatches.Text = "התמודדויות";
			this.labelMatches.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// grid
			// 
			this.grid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.grid.BoxList = null;
			this.grid.BoxSize = new System.Drawing.Size(50, 100);
			this.grid.Direction = Sport.UI.Controls.BoxGridDirection.Horizontal;
			this.grid.HeaderSize = new System.Drawing.Size(50, 20);
			this.grid.Line = 0;
			this.grid.Location = new System.Drawing.Point(148, 40);
			this.grid.Name = "grid";
			this.grid.Part = 0;
			this.grid.ScrollDirection = Sport.UI.Controls.BoxGridDirection.Vertical;
			this.grid.Size = new System.Drawing.Size(426, 292);
			this.grid.TabIndex = 9;
			this.grid.Title = null;
			// 
			// labelTournament
			// 
			this.labelTournament.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.labelTournament.Location = new System.Drawing.Point(16, 16);
			this.labelTournament.Name = "labelTournament";
			this.labelTournament.Size = new System.Drawing.Size(128, 23);
			this.labelTournament.TabIndex = 10;
			this.labelTournament.Text = "מבנה התמודדות";
			this.labelTournament.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// gridTournament
			// 
			this.gridTournament.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left)));
			this.gridTournament.BoxList = null;
			this.gridTournament.BoxSize = new System.Drawing.Size(50, 20);
			this.gridTournament.Direction = Sport.UI.Controls.BoxGridDirection.Vertical;
			this.gridTournament.HeaderSize = new System.Drawing.Size(50, 20);
			this.gridTournament.Line = 0;
			this.gridTournament.Location = new System.Drawing.Point(16, 40);
			this.gridTournament.Name = "gridTournament";
			this.gridTournament.Part = 0;
			this.gridTournament.ScrollDirection = Sport.UI.Controls.BoxGridDirection.Vertical;
			this.gridTournament.Size = new System.Drawing.Size(128, 292);
			this.gridTournament.TabIndex = 11;
			this.gridTournament.Title = null;
			// 
			// tbCopy
			// 
			this.tbCopy.Alignment = System.Drawing.StringAlignment.Center;
			this.tbCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.tbCopy.AutoSize = true;
			this.tbCopy.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbCopy.Hue = 200F;
			this.tbCopy.Image = ((System.Drawing.Image)(resources.GetObject("tbCopy.Image")));
			this.tbCopy.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbCopy.ImageList = null;
			this.tbCopy.ImageSize = new System.Drawing.Size(12, 12);
			this.tbCopy.Location = new System.Drawing.Point(152, 376);
			this.tbCopy.Name = "tbCopy";
			this.tbCopy.Saturation = 0.5F;
			this.tbCopy.Size = new System.Drawing.Size(57, 17);
			this.tbCopy.TabIndex = 19;
			this.tbCopy.Text = "העתק";
			this.tbCopy.Transparent = System.Drawing.Color.Black;
			this.tbCopy.Visible = false;
			this.tbCopy.Click += new System.EventHandler(this.tbCopy_Click);
			// 
			// GameBoardEditorView
			// 
			this.Controls.Add(this.tbCopy);
			this.Controls.Add(this.gridTournament);
			this.Controls.Add(this.labelTournament);
			this.Controls.Add(this.labelMatches);
			this.Controls.Add(this.tbCancel);
			this.Controls.Add(this.tbSave);
			this.Controls.Add(this.boardSizePanel);
			this.Controls.Add(this.labelTeams);
			this.Controls.Add(this.lbTeams);
			this.Controls.Add(this.tbAddRange);
			this.Controls.Add(this.tbRemoveRange);
			this.Controls.Add(this.grid);
			this.Name = "GameBoardEditorView";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.Size = new System.Drawing.Size(721, 413);
			this.ResumeLayout(false);

		}

		private Sport.Entities.GameBoard	entityGameBoard;
		private Sport.Producer.GameBoard	gameBoard;
		private GameBoardBoxList gameList = null;
		private TournamentBoxList tournamentList = null;

		public override void Open()
		{

			/*
			if (Sport.Entities.User.ViewPermission(this.GetType().Name) == 
				Sport.Entities.PermissionServices.PermissionType.None)
			{
				throw new Exception("Can't edit game boards: Permission Denied");
			}
			*/
			
			string bid = State["board"];

			Title = "לוח משחקים";

			if (bid != null)
			{
				int board = Int32.Parse(bid);

				Sport.Data.Entity entity = Sport.Entities.GameBoard.Type.Lookup(board);

				if (entity != null)
				{
					entityGameBoard = new Sport.Entities.GameBoard(entity);

					Title = "לוח משחקים - " + entity.Name;

					if (gameBoard.Load(entity.Id))
					{
						foreach (Sport.Producer.GameBoardItem item in gameBoard)
						{
							lbTeams.Items.Add(item);
						}
					}
				}
				else
					entityGameBoard = null;
			}

			tbSave.Enabled = false;
			tbCancel.Enabled = false;

			/*
			_permType = Sport.Entities.User.ViewPermission(this.GetType().Name);
			if (_permType == 
				Sport.Entities.PermissionServices.PermissionType.ReadOnly)
			{
				this.tbAddRange.Visible = false;
				this.tbRemoveRange.Visible = false;
				this.tbSave.Visible = false;
			}
			*/

			base.Open ();
		}

		public override void Close()
		{
			SelectTeams(null);
			lbTeams.Items.Clear();
			base.Close ();
		}


		private void tbAddRange_Click(object sender, System.EventArgs e)
		{
			string text = "";
			if (gameBoard == null)
				return;
			
			if (Sport.UI.Dialogs.TextEditDialog.EditText("בחר תחום קבוצות", ref text))
			{
				Sport.Common.RangeArray.Range range = Sport.Common.RangeArray.Range.Parse(text);
                
				if (range != null)
				{
					if (range.First < 1)
					{
						System.Windows.Forms.MessageBox.Show("תחום קבוצות חייב להיות גדול מ 0");
					}
					else if (!gameBoard.Fit(range.First, range.Last))
					{
						System.Windows.Forms.MessageBox.Show("תחום קבוצות מתנגש עם תחום אחר");
					}
					else
					{
						Sport.Producer.GameBoardItem item = 
							new Sport.Producer.GameBoardItem(range.First, range.Last);
						int n = gameBoard.Add(item);
						lbTeams.Items.Insert(n, gameBoard[n]);
					}
				}
			}
		}

		private void tbRemoveRange_Click(object sender, System.EventArgs e)
		{
			int index = lbTeams.SelectedIndex;
			if (index >= 0)
			{
				Sport.Producer.GameBoardItem item = (Sport.Producer.GameBoardItem) lbTeams.Items[index];
				gameBoard.Remove(item);
				lbTeams.Items.RemoveAt(index);
				SelectTeams(null);
			}
		}

		private void RoundsChanged(object sender, EventArgs e)
		{
			if (gameList != null)
			{
				gameList.Rounds = (int) (double) boardSizePanel.Items[0].Value;
				ResetBoxSize();
			}
		}

		private void CyclesChanged(object sender, EventArgs e)
		{
			if (gameList != null)
				gameList.Cycles = (int) (double) boardSizePanel.Items[1].Value;
		}

		private void MatchesChanged(object sender, EventArgs e)
		{
			if (gameList != null)
			{
				gameList.Matches = (int) (double) boardSizePanel.Items[2].Value;
				ResetBoxSize();
			}
		}

		private void GamesChanged(object sender, EventArgs e)
		{
			if (tournamentList != null)
			{
				tournamentList.MatchTeams = (int) (double) boardSizePanel.Items[3].Value;
			}
		}


		private void ResetBoxSize()
		{
			grid.BoxSize = new System.Drawing.Size(50, 
				gameList.Matches * gameList.Rounds * 15);
		}

		private void RoundClick(object sender, Sport.UI.Controls.BoxGrid.BoxEventArgs e)
		{
			if (e.Part != -1)
				gameList.EditRoundName(e.Part);
		}

		private void MatchClick(object sender, Sport.UI.Controls.BoxGrid.BoxEventArgs e)
		{
			if (e.Part == -1)
				gameList.EditCycleName(e.Box);
			else if (tournamentList.MatchTeams > 0)
				gameList.EditMatch(e.Part, e.Box, e.SubPart);
		}

		private void GameClick(object sender, Sport.UI.Controls.BoxGrid.BoxEventArgs e)
		{
			if (e.Part != -1)
			{
				if (tournamentList.MatchTeams > 0)
					tournamentList.EditGame(e.Box);
			}
		}

		private void SelectTeams(Sport.Producer.GameBoardItem item)
		{
			if (item == null)
			{
				grid.BoxList = null;
				boardSizePanel.Enabled = false;
			}
			else
			{
				// Setting gameList to null so
				// that the Value change in the
				// NumericUpDowns won't cause plan change
				gameList = null;
				tournamentList = null;
				boardSizePanel.Items[0].Value = item.Rounds;
				boardSizePanel.Items[1].Value = item.Cycles;
				boardSizePanel.Items[2].Value = item.Matches;
				boardSizePanel.Items[3].Value = item.MatchTeams;
				boardSizePanel.Enabled = true;
				gameList = new GameBoardBoxList(item);
				tournamentList = new TournamentBoxList(item);
				grid.BoxList = gameList;
				gridTournament.BoxList = tournamentList;
				ResetBoxSize();
			}
		}

		private void TeamsChanged(object sender, EventArgs e)
		{
			SelectTeams(lbTeams.SelectedItem as Sport.Producer.GameBoardItem);
		}

		private void tbSave_Click(object sender, System.EventArgs e)
		{
			if (gameBoard.Save(entityGameBoard.Id))
			{
				tbCancel.Enabled = false;
				tbSave.Enabled = false;
			}
		}

		private void tbCancel_Click(object sender, System.EventArgs e)
		{
			lbTeams.Items.Clear();
			SelectTeams(null);
			if (gameBoard.Load(entityGameBoard.Id))
			{
				foreach (Sport.Producer.GameBoardItem item in gameBoard)
				{
					lbTeams.Items.Add(item);
				}
			}

			tbCancel.Enabled = false;
			tbSave.Enabled = false;
		}

		private void GameBoardChanged(object sender, EventArgs e)
		{
			tbCancel.Enabled = true;
			tbSave.Enabled = true;
		}

		private void lbTeams_DoubleClick(object sender, System.EventArgs e)
		{
			int index = lbTeams.SelectedIndex;
			if (index != -1)
			{
				Sport.Producer.GameBoardItem gameBoardItem = 
					(Sport.Producer.GameBoardItem) lbTeams.Items[index];
				string text = gameBoardItem.Min.ToString() + "-" + gameBoardItem.Max.ToString();
				if (Sport.UI.Dialogs.TextEditDialog.EditText("בחר תחום קבוצות", ref text))
				{
					Sport.Common.RangeArray.Range range = Sport.Common.RangeArray.Range.Parse(text);
                
					if (range.First < 1)
					{
						System.Windows.Forms.MessageBox.Show("תחום קבוצות חייב להיות גדול מ 0");
					}
					else if ((index > 0 && gameBoard[index - 1].Max >= range.First) ||
						(index < gameBoard.Count - 1 && gameBoard[index + 1].Min <= range.Last))
					{
						System.Windows.Forms.MessageBox.Show("תחום קבוצות מתנגש עם תחום אחר");
					}
					else
					{
						if (range.First > gameBoardItem.Max)
						{
							gameBoardItem.Max = range.Last;
							gameBoardItem.Min = range.First;
						}
						else
						{
							gameBoardItem.Min = range.First;
							gameBoardItem.Max = range.Last;
						}

						lbTeams.Items.RemoveAt(index);
						lbTeams.Items.Insert(index, gameBoardItem);
						lbTeams.SelectedIndex = index;

					}
				}
			}
		}

		private void tbCopy_Click(object sender, System.EventArgs e)
		{
			Sport.Producer.GameBoardItem gameBoardItem = lbTeams.SelectedItem as
				Sport.Producer.GameBoardItem;
			GameBoardCopyForm gbcf = new GameBoardCopyForm(gameBoardItem);
			if (gbcf.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
			}
		}
	}

	public class GameBoardBoxList : Sport.UI.Controls.IBoxList
	{
		private Sport.Producer.GameBoardItem _gameBoardItem;
		public Sport.Producer.GameBoardItem GameBoardItem
		{
			get { return _gameBoardItem; }
		}

		public GameBoardBoxList(Sport.Producer.GameBoardItem gameBoardItem)
		{
			_gameBoardItem = gameBoardItem;
		}


		public int Rounds
		{
			get { return _gameBoardItem.Rounds; }
			set
			{
				_gameBoardItem.Rounds = value;
				if (ListChanged != null)
					ListChanged(this, EventArgs.Empty);
			}
		}

		public int Cycles
		{
			get { return _gameBoardItem.Cycles; }
			set 
			{
				_gameBoardItem.Cycles = value;
				if (ListChanged != null)
					ListChanged(this, EventArgs.Empty);
			}
		}

		public int Matches
		{
			get { return _gameBoardItem.Matches; }
			set 
			{ 
				_gameBoardItem.Matches = value;
				if (ListChanged != null)
					ListChanged(this, EventArgs.Empty);
			}
		}

		public void EditCycleName(int cycle)
		{
			string name = _gameBoardItem.GetCycleName(cycle);

			if (Sport.UI.Dialogs.TextEditDialog.EditText("עריכת שם מחזור", ref name))
			{
				_gameBoardItem.SetCycleName(cycle, name);
				if (ValueChanged != null)
					ValueChanged(this, EventArgs.Empty);
			}
		}

		public void EditRoundName(int round)
		{
			string name = _gameBoardItem.GetRoundName(round);

			if (Sport.UI.Dialogs.TextEditDialog.EditText("עריכת שם סיבוב", ref name))
			{
				_gameBoardItem.SetRoundName(round, name);
				if (ValueChanged != null)
					ValueChanged(this, EventArgs.Empty);
			}
		}

		public void EditMatch(int round, int cycle, int matchNumber)
		{
			/*
			string strTypeName=typeof(GameBoardEditorView).Name;
			if (Sport.Entities.User.ViewPermission(strTypeName) != 
				Sport.Entities.PermissionServices.PermissionType.Full)
			{
				return;
			}
			*/
			
			Sport.Producer.GameBoardItem.Match match = _gameBoardItem[round, cycle, matchNumber];
            
			string title =
				_gameBoardItem.GetRoundName(round) + " " +
				_gameBoardItem.GetCycleName(cycle) + " התמודדות " + (matchNumber + 1).ToString();

			Sport.UI.Dialogs.GenericEditDialog ged = 
				new Sport.UI.Dialogs.GenericEditDialog(title);

			Sport.Common.LetterNumber tn = new Sport.Common.LetterNumber(1, new Sport.Common.HebrewNumberFormat());
			for (int n = 0; n < _gameBoardItem.MatchTeams; n++)
			{
				int team = match == null || match.Teams == null || n >= match.Teams.Length ? 0 : match.Teams[n];
				ged.Items.Add("קבוצה " + tn.ToString() + ":", Sport.UI.Controls.GenericItemType.Number, team,
					new object[] { 0d, (double) _gameBoardItem.Max });
				tn.Number++;
			}
				
			if (ged.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				System.Collections.ArrayList al = new System.Collections.ArrayList();
				for (int n = 0; n < _gameBoardItem.MatchTeams; n++)
				{
					int team = (int) (double) ged.Items[n].Value;
					if (team != 0)
						al.Add(team);
				}

				if (al.Count == 0)
				{
					_gameBoardItem[round, cycle, matchNumber] = null;
				}
				else
				{
					_gameBoardItem.SetMatch(round, cycle, matchNumber, (int[]) al.ToArray(typeof(int)));
				}

				if (ValueChanged != null)
					ValueChanged(this, EventArgs.Empty);
			}
		}


		#region IBoxList Members

		public event EventHandler ListChanged;

		public string GetBoxTitle(int box)
		{
			return _gameBoardItem.GetCycleName(box);
		}

		public event EventHandler ValueChanged;

		public string GetPartTitle(int part)
		{
			return _gameBoardItem.GetRoundName(part);
		}

		public int GetPartCount()
		{
			return _gameBoardItem.Rounds;
		}

		public int GetPartSize(int part)
		{
			return 0;
		}

		public object GetBoxItem(int box, int part, int subpart)
		{
			return _gameBoardItem.GetMatch(part, box, subpart);
		}

		public int GetBoxCount()
		{
			return _gameBoardItem.Cycles;
		}

		public int GetSubPartCount(int part)
		{
			return _gameBoardItem.Matches;
		}

		#endregion
	}

	public class TournamentBoxList : Sport.UI.Controls.IBoxList
	{
		private Sport.Producer.GameBoardItem _gameBoardItem;
		public Sport.Producer.GameBoardItem GameBoardItem
		{
			get { return _gameBoardItem; }
		}

		public TournamentBoxList(Sport.Producer.GameBoardItem gameBoardItem)
		{
			_gameBoardItem = gameBoardItem;
		}


		public int MatchTeams
		{
			get { return _gameBoardItem.MatchTeams; }
			set
			{
				_gameBoardItem.MatchTeams = value;
				if (ListChanged != null)
					ListChanged(this, EventArgs.Empty);
			}
		}

		public void EditGame(int number)
		{
			/*
			string strTypeName=typeof(GameBoardEditorView).Name;
			if (Sport.Entities.User.ViewPermission(strTypeName) != 
				Sport.Entities.PermissionServices.PermissionType.Full)
			{
				return;
			}
			*/
			
			Sport.Producer.GameBoardItem.Game game = _gameBoardItem.GetGame(number);
            
			string title = "משחק " + (number + 1).ToString();

			Sport.UI.Dialogs.GenericEditDialog ged = 
				new Sport.UI.Dialogs.GenericEditDialog(title);

			int teamA = game == null ? 0 : game.TeamA;
			int teamB = game == null ? 0 : game.TeamB;

			ged.Items.Add("קבוצה א:", Sport.UI.Controls.GenericItemType.Number, teamA,
				new object[] { 0d, (double) _gameBoardItem.MatchTeams });
			ged.Items.Add("קבוצה ב:", Sport.UI.Controls.GenericItemType.Number, teamB,
				new object[] { 0d, (double) _gameBoardItem.MatchTeams });
				
			if (ged.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				teamA = (int) (double) ged.Items[0].Value;
				teamB = (int) (double) ged.Items[1].Value;

				if (teamA == 0 || teamB == 0)
					game = null;
				else
					game = new Sport.Producer.GameBoardItem.Game(teamA, teamB);
				_gameBoardItem.SetGame(number, game);

				if (ValueChanged != null)
					ValueChanged(this, EventArgs.Empty);
			}
		}


		#region IBoxList Members

		public event EventHandler ListChanged;

		public string GetBoxTitle(int box)
		{
			return null;
		}

		public event EventHandler ValueChanged;

		public string GetPartTitle(int part)
		{
			return "משחקים";
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
			return _gameBoardItem.GetGame(box);
		}

		public int GetBoxCount()
		{
			return _gameBoardItem.GameCount + 1;
		}

		public int GetSubPartCount(int part)
		{
			return 1;
		}

		#endregion
	}
}
