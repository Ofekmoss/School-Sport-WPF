using System;

namespace Sportsman.Producer
{
	/// <summary>
	/// Summary description for GameBoardCopyForm.
	/// </summary>
	public class GameBoardCopyForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.GroupBox gbSource;
		private Sport.UI.Controls.NullComboBox cbSourceBoard;
		private System.Windows.Forms.Label labelSourceRound;
		private Sport.UI.Controls.NullComboBox cbSourceRound;
		private System.Windows.Forms.Label labelSourceCycle;
		private System.Windows.Forms.GroupBox gbTarget;
		private Sport.UI.Controls.NullComboBox cbSourceCycle;
		private System.Windows.Forms.Label labelTargetCycle;
		private Sport.UI.Controls.NullComboBox cbTargetCycle;
		private System.Windows.Forms.Label labelTargetRound;
		private Sport.UI.Controls.NullComboBox cbTargetRound;
		private Sport.UI.Controls.NullComboBox cbTargetBoard;
		private System.Windows.Forms.Label labelTargetBoard;
		private System.Windows.Forms.Label labelTargetRange;
		private Sport.UI.Controls.NullComboBox cbTargetRange;
		private System.Windows.Forms.Label labelSourceRange;
		private Sport.UI.Controls.NullComboBox cbSourceRange;
		private System.Windows.Forms.Label labelSourceBoard;
	
		private void InitializeComponent()
		{
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.gbSource = new System.Windows.Forms.GroupBox();
			this.labelSourceRange = new System.Windows.Forms.Label();
			this.cbSourceRange = new Sport.UI.Controls.NullComboBox();
			this.labelSourceCycle = new System.Windows.Forms.Label();
			this.cbSourceCycle = new Sport.UI.Controls.NullComboBox();
			this.labelSourceRound = new System.Windows.Forms.Label();
			this.cbSourceRound = new Sport.UI.Controls.NullComboBox();
			this.cbSourceBoard = new Sport.UI.Controls.NullComboBox();
			this.labelSourceBoard = new System.Windows.Forms.Label();
			this.gbTarget = new System.Windows.Forms.GroupBox();
			this.labelTargetRange = new System.Windows.Forms.Label();
			this.cbTargetRange = new Sport.UI.Controls.NullComboBox();
			this.labelTargetCycle = new System.Windows.Forms.Label();
			this.cbTargetCycle = new Sport.UI.Controls.NullComboBox();
			this.labelTargetRound = new System.Windows.Forms.Label();
			this.cbTargetRound = new Sport.UI.Controls.NullComboBox();
			this.cbTargetBoard = new Sport.UI.Controls.NullComboBox();
			this.labelTargetBoard = new System.Windows.Forms.Label();
			this.gbSource.SuspendLayout();
			this.gbTarget.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(8, 240);
			this.btnOK.Name = "btnOK";
			this.btnOK.TabIndex = 0;
			this.btnOK.Text = "אישור";
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(96, 240);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.TabIndex = 1;
			this.btnCancel.Text = "ביטול";
			// 
			// gbSource
			// 
			this.gbSource.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.gbSource.Controls.Add(this.labelSourceRange);
			this.gbSource.Controls.Add(this.cbSourceRange);
			this.gbSource.Controls.Add(this.labelSourceCycle);
			this.gbSource.Controls.Add(this.cbSourceCycle);
			this.gbSource.Controls.Add(this.labelSourceRound);
			this.gbSource.Controls.Add(this.cbSourceRound);
			this.gbSource.Controls.Add(this.cbSourceBoard);
			this.gbSource.Controls.Add(this.labelSourceBoard);
			this.gbSource.Location = new System.Drawing.Point(8, 8);
			this.gbSource.Name = "gbSource";
			this.gbSource.Size = new System.Drawing.Size(418, 108);
			this.gbSource.TabIndex = 2;
			this.gbSource.TabStop = false;
			this.gbSource.Text = "מקור";
			// 
			// labelSourceRange
			// 
			this.labelSourceRange.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelSourceRange.Location = new System.Drawing.Point(336, 55);
			this.labelSourceRange.Name = "labelSourceRange";
			this.labelSourceRange.Size = new System.Drawing.Size(72, 16);
			this.labelSourceRange.TabIndex = 15;
			this.labelSourceRange.Text = "טווח קבוצות:";
			// 
			// cbSourceRange
			// 
			this.cbSourceRange.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cbSourceRange.Location = new System.Drawing.Point(224, 49);
			this.cbSourceRange.Name = "cbSourceRange";
			this.cbSourceRange.Size = new System.Drawing.Size(104, 22);
			this.cbSourceRange.Sorted = true;
			this.cbSourceRange.TabIndex = 14;
			// 
			// labelSourceCycle
			// 
			this.labelSourceCycle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelSourceCycle.Location = new System.Drawing.Point(360, 83);
			this.labelSourceCycle.Name = "labelSourceCycle";
			this.labelSourceCycle.Size = new System.Drawing.Size(48, 16);
			this.labelSourceCycle.TabIndex = 5;
			this.labelSourceCycle.Text = "מחזור:";
			// 
			// cbSourceCycle
			// 
			this.cbSourceCycle.Location = new System.Drawing.Point(200, 76);
			this.cbSourceCycle.Name = "cbSourceCycle";
			this.cbSourceCycle.Size = new System.Drawing.Size(128, 22);
			this.cbSourceCycle.Sorted = true;
			this.cbSourceCycle.TabIndex = 4;
			// 
			// labelSourceRound
			// 
			this.labelSourceRound.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelSourceRound.Location = new System.Drawing.Point(152, 55);
			this.labelSourceRound.Name = "labelSourceRound";
			this.labelSourceRound.Size = new System.Drawing.Size(48, 16);
			this.labelSourceRound.TabIndex = 3;
			this.labelSourceRound.Text = "סיבוב:";
			// 
			// cbSourceRound
			// 
			this.cbSourceRound.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cbSourceRound.Location = new System.Drawing.Point(8, 49);
			this.cbSourceRound.Name = "cbSourceRound";
			this.cbSourceRound.Size = new System.Drawing.Size(128, 22);
			this.cbSourceRound.Sorted = true;
			this.cbSourceRound.TabIndex = 2;
			// 
			// cbSourceBoard
			// 
			this.cbSourceBoard.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.cbSourceBoard.Location = new System.Drawing.Point(8, 21);
			this.cbSourceBoard.Name = "cbSourceBoard";
			this.cbSourceBoard.Size = new System.Drawing.Size(320, 22);
			this.cbSourceBoard.Sorted = true;
			this.cbSourceBoard.TabIndex = 0;
			this.cbSourceBoard.SelectedIndexChanged += new System.EventHandler(this.cbSourceBoard_SelectedIndexChanged);
			// 
			// labelSourceBoard
			// 
			this.labelSourceBoard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelSourceBoard.Location = new System.Drawing.Point(328, 26);
			this.labelSourceBoard.Name = "labelSourceBoard";
			this.labelSourceBoard.Size = new System.Drawing.Size(80, 16);
			this.labelSourceBoard.TabIndex = 1;
			this.labelSourceBoard.Text = "לוח משחקים:";
			// 
			// gbTarget
			// 
			this.gbTarget.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.gbTarget.Controls.Add(this.labelTargetRange);
			this.gbTarget.Controls.Add(this.cbTargetRange);
			this.gbTarget.Controls.Add(this.labelTargetCycle);
			this.gbTarget.Controls.Add(this.cbTargetCycle);
			this.gbTarget.Controls.Add(this.labelTargetRound);
			this.gbTarget.Controls.Add(this.cbTargetRound);
			this.gbTarget.Controls.Add(this.cbTargetBoard);
			this.gbTarget.Controls.Add(this.labelTargetBoard);
			this.gbTarget.Location = new System.Drawing.Point(8, 120);
			this.gbTarget.Name = "gbTarget";
			this.gbTarget.Size = new System.Drawing.Size(418, 108);
			this.gbTarget.TabIndex = 3;
			this.gbTarget.TabStop = false;
			this.gbTarget.Text = "יעד";
			// 
			// labelTargetRange
			// 
			this.labelTargetRange.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelTargetRange.Location = new System.Drawing.Point(336, 55);
			this.labelTargetRange.Name = "labelTargetRange";
			this.labelTargetRange.Size = new System.Drawing.Size(72, 16);
			this.labelTargetRange.TabIndex = 13;
			this.labelTargetRange.Text = "טווח קבוצות:";
			// 
			// cbTargetRange
			// 
			this.cbTargetRange.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cbTargetRange.Location = new System.Drawing.Point(224, 49);
			this.cbTargetRange.Name = "cbTargetRange";
			this.cbTargetRange.Size = new System.Drawing.Size(104, 22);
			this.cbTargetRange.Sorted = true;
			this.cbTargetRange.TabIndex = 12;
			// 
			// labelTargetCycle
			// 
			this.labelTargetCycle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelTargetCycle.Location = new System.Drawing.Point(360, 83);
			this.labelTargetCycle.Name = "labelTargetCycle";
			this.labelTargetCycle.Size = new System.Drawing.Size(48, 16);
			this.labelTargetCycle.TabIndex = 11;
			this.labelTargetCycle.Text = "מחזור:";
			// 
			// cbTargetCycle
			// 
			this.cbTargetCycle.Location = new System.Drawing.Point(200, 76);
			this.cbTargetCycle.Name = "cbTargetCycle";
			this.cbTargetCycle.Size = new System.Drawing.Size(128, 22);
			this.cbTargetCycle.Sorted = true;
			this.cbTargetCycle.TabIndex = 10;
			// 
			// labelTargetRound
			// 
			this.labelTargetRound.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelTargetRound.Location = new System.Drawing.Point(152, 55);
			this.labelTargetRound.Name = "labelTargetRound";
			this.labelTargetRound.Size = new System.Drawing.Size(48, 16);
			this.labelTargetRound.TabIndex = 9;
			this.labelTargetRound.Text = "סיבוב:";
			// 
			// cbTargetRound
			// 
			this.cbTargetRound.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cbTargetRound.Location = new System.Drawing.Point(8, 49);
			this.cbTargetRound.Name = "cbTargetRound";
			this.cbTargetRound.Size = new System.Drawing.Size(128, 22);
			this.cbTargetRound.Sorted = true;
			this.cbTargetRound.TabIndex = 8;
			// 
			// cbTargetBoard
			// 
			this.cbTargetBoard.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.cbTargetBoard.Location = new System.Drawing.Point(8, 21);
			this.cbTargetBoard.Name = "cbTargetBoard";
			this.cbTargetBoard.Size = new System.Drawing.Size(320, 22);
			this.cbTargetBoard.Sorted = true;
			this.cbTargetBoard.TabIndex = 6;
			this.cbTargetBoard.SelectedIndexChanged += new System.EventHandler(this.cbTargetBoard_SelectedIndexChanged);
			// 
			// labelTargetBoard
			// 
			this.labelTargetBoard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelTargetBoard.Location = new System.Drawing.Point(328, 26);
			this.labelTargetBoard.Name = "labelTargetBoard";
			this.labelTargetBoard.Size = new System.Drawing.Size(80, 16);
			this.labelTargetBoard.TabIndex = 7;
			this.labelTargetBoard.Text = "לוח משחקים:";
			// 
			// GameBoardCopyForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.ClientSize = new System.Drawing.Size(434, 272);
			this.Controls.Add(this.gbTarget);
			this.Controls.Add(this.gbSource);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "GameBoardCopyForm";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "העתקת התמודדויות";
			this.gbSource.ResumeLayout(false);
			this.gbTarget.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		private class BoardItem
		{
			private Sport.Data.Entity _entity;
			public Sport.Data.Entity Entity
			{
				get { return _entity; }
			}

			private Sport.Producer.GameBoard _board;
			public Sport.Producer.GameBoard Board
			{
				get
				{
					if (_board == null)
					{
						_board = new Sport.Producer.GameBoard();
						_board.Load(_entity.Id);
					}
					return _board;
				}
			}

			public BoardItem(Sport.Data.Entity entity)
			{
				_entity = entity;
			}

			public override string ToString()
			{
				return _entity.Name;
			}

			public override bool Equals(object obj)
			{
				if (obj is BoardItem)
					return _entity.Id == ((BoardItem) obj)._entity.Id;

				return _entity.Equals(obj);
			}

			public override int GetHashCode()
			{
				return _entity.Id;
			}
		}

		private BoardItem[] boardItems;
	
		public GameBoardCopyForm(Sport.Producer.GameBoardItem gameBoardItem)
		{
			InitializeComponent();

			Sport.Data.Entity[] boards = Sport.Entities.GameBoard.Type.GetEntities(null);

			int boardId = gameBoardItem == null ? -1 : gameBoardItem.GameBoard.Id;
			BoardItem selected = null;

			boardItems = new BoardItem[boards.Length];
			for (int n = 0; n < boards.Length; n++)
			{
				boardItems[n] = new BoardItem(boards[n]);
				if (boards[n].Id == boardId)
					selected = boardItems[n];
			}

			cbSourceBoard.Items.AddRange(boardItems);
			cbTargetBoard.Items.AddRange(boardItems);

			if (gameBoardItem != null)
			{
				Sport.Entities.GameBoard gameBoard = new Sport.Entities.GameBoard(gameBoardItem.GameBoard.Id);
				cbSourceBoard.SelectedItem = selected;
				cbTargetBoard.SelectedItem = selected;

				cbSourceRange.SelectedItem = gameBoardItem;
				cbTargetRange.SelectedItem = gameBoardItem;
			}
		}

		private void cbSourceBoard_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			BoardItem item = cbSourceBoard.SelectedItem as BoardItem;

			if (item != null)
			{
				cbSourceRange.Items.Clear();
				foreach (Sport.Producer.GameBoardItem gameBoardItem in item.Board)
				{
					cbSourceRange.Items.Add(gameBoardItem);
				}
			}
		}

		private void cbTargetBoard_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			BoardItem item = cbTargetBoard.SelectedItem as BoardItem;

			if (item != null)
			{
				cbTargetRange.Items.Clear();
				foreach (Sport.Producer.GameBoardItem gameBoardItem in item.Board)
				{
					cbTargetRange.Items.Add(gameBoardItem);
				}
			}
		}
	}
}
