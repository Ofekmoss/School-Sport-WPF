using System;
using System.Linq;
using System.Collections;
using System.Windows.Forms;
using Sport.UI;
using Sport.UI.Controls;
using Sport.Data;
using Sportsman.Core;

namespace Sportsman.Producer
{
	public class PlayerCardBuildForm : System.Windows.Forms.Form
	{
		#region components

		private System.Windows.Forms.ListBox cardsList;
		private System.Windows.Forms.ListBox stickersList;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button btnToCards;
		private System.Windows.Forms.Button btnToStickers;
		private System.Windows.Forms.Button btnRemoveCards;
		private Sport.UI.Controls.ThemeButton tbPrintCards;
		private System.Windows.Forms.Button btnRemoveStickers;
		private Sport.UI.Controls.ThemeButton tbPrintStickers;
		private System.Windows.Forms.Button btnClose;
		//private Sport.UI.Controls.Grid cardsGrid;
		//private Sport.UI.Controls.Grid stickersGrid;
		//private Sport.UI.Controls.Grid playersGrid;
		private System.Windows.Forms.ListBox playersList;
		#endregion

		private int _teamId;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		private Sport.Entities.Player[] _players; //players for which to print cards.

		/// <summary>
		/// default constructor: initialize all components and build them.
		/// </summary>
		public PlayerCardBuildForm(int teamId)
		{
			Sport.Entities.Team team = null;
			_teamId = teamId;
			InitializeComponent();

			//get selected team:
			team = new Sport.Entities.Team(_teamId);
			if (!team.IsValid())
			{
				Sport.UI.MessageBox.Warn("לא ניתן להדפיס כרטיסי שחקן: קבוצה לא חוקית", "שגיאת מערכת");
				this.DialogResult = DialogResult.Abort;
			}

			//fill players grid.
			//get players for given team.
			//first, define filter:
			EntityFilter filter = new EntityFilter(
				new EntityFilterField((int)Sport.Entities.Player.Fields.Team, team.Id));
			//get players list:
			Entity[] players = Sport.Entities.Player.Type.GetEntities(filter);

			//initialize service:
			PlayerCardServices.PlayerCardService service = new PlayerCardServices.PlayerCardService();
			service.CookieContainer = Sport.Core.Session.Cookies;

			//iterate through the players and add to the proper grid:
			for (int i = 0; i < players.Length; i++)
			{
				//get current player:
				Sport.Entities.Player player = null;
				try
				{
					player = new Sport.Entities.Player(players[i]);
				}
				catch
				{
					player = null;
				}
				if ((player == null) || (player.Id < 0))
					continue;

				//check if already has player card:
				Sport.Entities.Student student = null;
				try
				{
					student = player.Student;
				}
				catch { }
				if ((student == null) || (student.Id < 0))
					continue;
				
				//define custom name:
				string playerName = student.FirstName + " " + student.LastName;

				//DateTime date = service.CardIssueDate(student.Id, player.Team.Championship.Sport.Id);
				//bool cardExists = (date.Year > 1900);

				//update grid value:
				PlayerItem item = new PlayerItem(playerName, player.IdNumber, player.Id);
				if (player.GotSticker == 1)
				{
					playersList.Items.Add(item);
				}
				else
				{
					cardsList.Items.Add(item);
				}
			}

			this.Text = "כרטיסי שחקן - " + team.Name;
		} //end default constructor

		#region Initialize Component
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PlayerCardBuildForm));
			this.playersList = new System.Windows.Forms.ListBox();
			this.cardsList = new System.Windows.Forms.ListBox();
			this.stickersList = new System.Windows.Forms.ListBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.btnToCards = new System.Windows.Forms.Button();
			this.btnToStickers = new System.Windows.Forms.Button();
			this.btnRemoveCards = new System.Windows.Forms.Button();
			this.tbPrintCards = new Sport.UI.Controls.ThemeButton();
			this.tbPrintStickers = new Sport.UI.Controls.ThemeButton();
			this.btnRemoveStickers = new System.Windows.Forms.Button();
			this.btnClose = new System.Windows.Forms.Button();
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// playersList
			// 
			this.playersList.ItemHeight = 17;
			this.playersList.Location = new System.Drawing.Point(302, 28);
			this.playersList.Name = "playersList";
			this.playersList.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.playersList.Size = new System.Drawing.Size(210, 395);
			this.playersList.TabIndex = 0;
			this.playersList.SelectedIndexChanged += new System.EventHandler(this.cardsList_SelectedIndexChanged);
			this.playersList.DoubleClick += new System.EventHandler(this.playersList_DoubleClick);
			// 
			// cardsList
			// 
			this.cardsList.ItemHeight = 17;
			this.cardsList.Location = new System.Drawing.Point(605, 28);
			this.cardsList.Name = "cardsList";
			this.cardsList.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.cardsList.Size = new System.Drawing.Size(210, 395);
			this.cardsList.TabIndex = 3;
			this.cardsList.SelectedIndexChanged += new System.EventHandler(this.cardsList_SelectedIndexChanged);
			this.cardsList.DoubleClick += new System.EventHandler(this.cardsList_DoubleClick);
			// 
			// stickersList
			// 
			this.stickersList.ItemHeight = 17;
			this.stickersList.Location = new System.Drawing.Point(14, 28);
			this.stickersList.Name = "stickersList";
			this.stickersList.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.stickersList.Size = new System.Drawing.Size(210, 395);
			this.stickersList.TabIndex = 4;
			this.stickersList.SelectedIndexChanged += new System.EventHandler(this.cardsList_SelectedIndexChanged);
			this.stickersList.DoubleClick += new System.EventHandler(this.stickersList_DoubleClick);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(14, 5);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(210, 22);
			this.label1.TabIndex = 5;
			this.label1.Text = "מדבקות";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(605, 7);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(210, 20);
			this.label2.TabIndex = 6;
			this.label2.Text = "כרטיסי שחקן";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(302, 5);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(210, 22);
			this.label3.TabIndex = 7;
			this.label3.Text = "מאגר שחקנים";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// btnToCards
			// 
			this.btnToCards.Enabled = false;
			this.btnToCards.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.btnToCards.Location = new System.Drawing.Point(525, 58);
			this.btnToCards.Name = "btnToCards";
			this.btnToCards.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.btnToCards.Size = new System.Drawing.Size(70, 31);
			this.btnToCards.TabIndex = 8;
			this.btnToCards.Text = ">>";
			this.btnToCards.Click += new System.EventHandler(this.btnToCards_Click);
			// 
			// btnToStickers
			// 
			this.btnToStickers.Enabled = false;
			this.btnToStickers.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.btnToStickers.Location = new System.Drawing.Point(227, 58);
			this.btnToStickers.Name = "btnToStickers";
			this.btnToStickers.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.btnToStickers.Size = new System.Drawing.Size(70, 31);
			this.btnToStickers.TabIndex = 9;
			this.btnToStickers.Text = "<<";
			this.btnToStickers.Click += new System.EventHandler(this.btnToStickers_Click);
			// 
			// btnRemoveCards
			// 
			this.btnRemoveCards.Enabled = false;
			this.btnRemoveCards.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.btnRemoveCards.Location = new System.Drawing.Point(525, 158);
			this.btnRemoveCards.Name = "btnRemoveCards";
			this.btnRemoveCards.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.btnRemoveCards.Size = new System.Drawing.Size(70, 30);
			this.btnRemoveCards.TabIndex = 10;
			this.btnRemoveCards.Text = "<<";
			this.btnRemoveCards.Click += new System.EventHandler(this.btnRemoveCards_Click);
			// 
			// tbPrintCards
			// 
			this.tbPrintCards.Alignment = System.Drawing.StringAlignment.Center;
			this.tbPrintCards.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbPrintCards.Hue = 220F;
			this.tbPrintCards.Image = ((System.Drawing.Image)(resources.GetObject("tbPrintCards.Image")));
			this.tbPrintCards.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbPrintCards.ImageList = null;
			this.tbPrintCards.ImageSize = new System.Drawing.Size(12, 12);
			this.tbPrintCards.Location = new System.Drawing.Point(605, 447);
			this.tbPrintCards.Name = "tbPrintCards";
			this.tbPrintCards.Saturation = 0.9F;
			this.tbPrintCards.Size = new System.Drawing.Size(56, 17);
			this.tbPrintCards.TabIndex = 12;
			this.tbPrintCards.Text = "הדפס";
			this.tbPrintCards.Transparent = System.Drawing.Color.Black;
			this.tbPrintCards.Click += new System.EventHandler(this.tbPrintCards_Click);
			// 
			// tbPrintStickers
			// 
			this.tbPrintStickers.Alignment = System.Drawing.StringAlignment.Center;
			this.tbPrintStickers.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbPrintStickers.Hue = 220F;
			this.tbPrintStickers.Image = ((System.Drawing.Image)(resources.GetObject("tbPrintStickers.Image")));
			this.tbPrintStickers.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbPrintStickers.ImageList = null;
			this.tbPrintStickers.ImageSize = new System.Drawing.Size(12, 12);
			this.tbPrintStickers.Location = new System.Drawing.Point(11, 447);
			this.tbPrintStickers.Name = "tbPrintStickers";
			this.tbPrintStickers.Saturation = 0.9F;
			this.tbPrintStickers.Size = new System.Drawing.Size(56, 17);
			this.tbPrintStickers.TabIndex = 13;
			this.tbPrintStickers.Text = "הדפס";
			this.tbPrintStickers.Transparent = System.Drawing.Color.Black;
			this.tbPrintStickers.Click += new System.EventHandler(this.tbPrintStickers_Click);
			// 
			// btnRemoveStickers
			// 
			this.btnRemoveStickers.Enabled = false;
			this.btnRemoveStickers.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.btnRemoveStickers.Location = new System.Drawing.Point(227, 158);
			this.btnRemoveStickers.Name = "btnRemoveStickers";
			this.btnRemoveStickers.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.btnRemoveStickers.Size = new System.Drawing.Size(70, 30);
			this.btnRemoveStickers.TabIndex = 14;
			this.btnRemoveStickers.Text = ">>";
			this.btnRemoveStickers.Click += new System.EventHandler(this.btnRemoveStickers_Click);
			// 
			// btnClose
			// 
			this.btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnClose.Location = new System.Drawing.Point(11, 476);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(105, 28);
			this.btnClose.TabIndex = 15;
			this.btnClose.Text = "סגור";
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(739, 447);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(67, 28);
			this.button1.TabIndex = 16;
			this.button1.Text = "שוליים";
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(146, 447);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(67, 28);
			this.button2.TabIndex = 17;
			this.button2.Text = "שוליים";
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// PlayerCardBuildForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(7, 17);
			this.ClientSize = new System.Drawing.Size(930, 565);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.btnClose);
			this.Controls.Add(this.btnRemoveStickers);
			this.Controls.Add(this.tbPrintStickers);
			this.Controls.Add(this.tbPrintCards);
			this.Controls.Add(this.btnRemoveCards);
			this.Controls.Add(this.btnToStickers);
			this.Controls.Add(this.btnToCards);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.stickersList);
			this.Controls.Add(this.cardsList);
			this.Controls.Add(this.playersList);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "PlayerCardBuildForm";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Load += new System.EventHandler(this.PlayerCardBuildForm_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		#region Printing
		private Sport.Documents.DocumentBuilder CreatePlayerCardsDocument(
			System.Drawing.Printing.PrinterSettings settings)
		{
			//abort if no players were selected for card printing:
			if (cardsList.Items.Count == 0)
				return null;

			//get selected players.
			_players = GetComboPlayers(cardsList);

			//set wait cursor:
			Cursor cur = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;

			//initialize print document:
			Sport.Documents.DocumentBuilder db = new Sport.Documents.DocumentBuilder("הדפסת כרטיסי שחקן");
			db.SetSettings(settings);

			//UPDATE 08/01/2018 - Yahav - force fixed size
			//db.DefaultPageSize = new System.Drawing.Size(358, 234);

			//get document size:
			System.Drawing.Size size = db.DefaultPageSize;

			//set printing direction and font:
			db.Direction = Sport.Documents.Direction.Right;
			db.Font = new System.Drawing.Font("Tahoma", 16, System.Drawing.GraphicsUnit.Pixel);

			//initialize section:
			Sport.Documents.Section section = new Sport.Documents.Section();

			DateTime issueDate = DateTime.Now;
			PlayerCardsObject2 pco = new PlayerCardsObject2(new System.Drawing.Rectangle(8, 8, size.Width - 8, size.Height - 8));

			for (int i = 0; i < _players.Length; i++)
			{
				pco.Cards.Add(new PlayerCardsObject2.PlayerCard(_players[i], issueDate,
					Sport.Entities.Student.StudentPicture(_players[i].Student.IdNumber)));
			}

			section.Items.Add(pco);

			//add section to the document:
			db.Sections.Add(section);

			//restore cursor:
			Cursor.Current = cur;

			//done.
			return db;
		} //end function CreatePlayerCardsDocument

		private Sport.Documents.DocumentBuilder CreateStickersDocument(
			System.Drawing.Printing.PrinterSettings settings)
		{
			//abort if no players were selected for card printing:
			if (stickersList.Items.Count == 0)
				return null;

			//get selected players.
			Sport.Entities.Player[] players = GetComboPlayers(stickersList);

			//set wait cursor:
			Cursor cur = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;

			//initialize print document:
			Sport.Documents.DocumentBuilder db = new Sport.Documents.DocumentBuilder("הדפסת מדבקות שחקן");
			db.SetSettings(settings);

			//get document size:
			System.Drawing.Size size = db.DefaultPageSize;

			//set printing direction and font:
			db.Direction = Sport.Documents.Direction.Right;
			db.Font = new System.Drawing.Font("Tahoma", 12, System.Drawing.GraphicsUnit.Pixel);

			//initialize section:
			Sport.Documents.Section section = new Sport.Documents.Section();

			//initialize stickers object:
			PlayerStickersObject pso = new PlayerStickersObject(new System.Drawing.Rectangle(8, 8, size.Width - 8, size.Height - 8));

			//add stickers:
			foreach (Sport.Entities.Player player in players)
				pso.Stickers.Add(new PlayerStickersObject.Sticker(player));

			//add the stickers to the section:
			section.Items.Add(pso);

			//add section to the document:
			db.Sections.Add(section);

			//restore cursor:
			Cursor.Current = cur;

			//done.
			return db;
		} //end function CreateStickersDocument
		#endregion

		private Sport.Entities.Player[] GetComboPlayers(ListBox combo)
		{
			ArrayList result = new ArrayList();
			for (int index = 0; index < combo.Items.Count; index++)
			{
				//get current item:
				Sport.Entities.Player player = new Sport.Entities.Player(
					(combo.Items[index] as PlayerItem).playerID);

				//valid?
				if (player.IsValid())
					result.Add(player);
			}
			return (Sport.Entities.Player[])
				result.ToArray(typeof(Sport.Entities.Player));
		}

		private void tbPrintCards_Click(object sender, System.EventArgs e)
		{
			//get selected team:
			Sport.Entities.Team team = null;
			System.Drawing.Printing.PrinterSettings ps;
			Sport.UI.Dialogs.PrintSettingsForm settingsForm;
			team = new Sport.Entities.Team(_teamId);
			if (!team.IsValid())
			{
				return;
			}

			//abort if no players were selected:
			if (cardsList.Items.Count == 0)
				return;

			if (!Sport.UI.Helpers.GetPrinterSettings(out ps, out settingsForm, 8.56, 5.398))
				return;
			
			//ask user to choose the settings:
			if (settingsForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				//user confirmed settings, initialize document builder:
				Sport.Documents.DocumentBuilder db = CreatePlayerCardsDocument(ps);

				//check if user actually printed the document:
				bool printed = false;

				//show preview if user asked:
				if (settingsForm.ShowPreview)
				{
					//build the preview, let user cancel:
					Sport.UI.Dialogs.PrintForm printForm = new Sport.UI.Dialogs.PrintForm(db.CreateDocument(), ps);

					//show the preview if not canceled:
					if (!printForm.Canceled)
					{
						if (printForm.ShowDialog() == DialogResult.OK)
							printed = true;
					}

					//done with preview.
					printForm.Dispose();
				}
				else
				{
					//user don't want preview, print directly:
					System.Drawing.Printing.PrintDocument pd = db.CreateDocument().CreatePrintDocument(ps);
					pd.PrintController = new PrintControllerWithPageForm(pd.PrintController, 0);
					pd.Print();
					printed = true;
				}

				//add to database if printed:
				if (printed)
				{
					PlayerCardServices.PlayerCardService service = new PlayerCardServices.PlayerCardService();
					service.CookieContainer = Sport.Core.Session.Cookies;
					int[] students = GetStudents();
					int sport = team.Championship.Sport.Id;
					try
					{
						service.IssuePlayerCards(students, sport);
					}
					catch (Exception ex)
					{
						Mir.Common.Logger.Instance.WriteLog(Mir.Common.LogType.Warning, "PlayerCards", "Error in IssuePlayerCards: " + ex.ToString());
					}
				}
			}
		}  //end function tbPrintCards_Click

		private int[] GetStudents()
		{
			int[] result = new int[_players.Length];
			for (int i = 0; i < _players.Length; i++)
			{
				result[i] = _players[i].Student.Id;
			}
			return result;
		}

		/// <summary>
		/// move selected items from source to target list box
		/// </summary>
		private void MoveComboSelection(ListBox source, ListBox target)
		{
			ArrayList arrToRemove = new ArrayList();

			//source.SelectedIndices.OfT
			source.SelectedIndices.OfType<int>().ToList().ForEach(selectedIndex =>
			{
				//get selected item:
				object item = source.Items[selectedIndex];

				//add to target list box if not in there yet
				if (target.Items.IndexOf(item) < 0)
					target.Items.Add(item);

				//add to the list of items to be removed:
				arrToRemove.Add(item);
			});

			//remove selected items from source list box
			for (int i = 0; i < arrToRemove.Count; i++)
				source.Items.Remove(arrToRemove[i]);
		}

		private void btnToCards_Click(object sender, System.EventArgs e)
		{
			if (!btnToCards.Enabled)
				return;
			MoveComboSelection(playersList, cardsList);
		}

		private void btnToStickers_Click(object sender, System.EventArgs e)
		{
			if (!btnToStickers.Enabled)
				return;
			MoveComboSelection(playersList, stickersList);
		}

		private void btnRemoveCards_Click(object sender, System.EventArgs e)
		{
			if (!btnRemoveCards.Enabled)
				return;
			MoveComboSelection(cardsList, playersList);
		}

		private void btnRemoveStickers_Click(object sender, System.EventArgs e)
		{
			if (!btnRemoveStickers.Enabled)
				return;
			MoveComboSelection(stickersList, playersList);
		}

		private void PlayerCardBuildForm_Load(object sender, EventArgs e)
		{

		}

		private void tbPrintStickers_Click(object sender, System.EventArgs e)
		{
			//get selected team:
			Sport.Entities.Team team = null;
			System.Drawing.Printing.PrinterSettings ps;
			Sport.UI.Dialogs.PrintSettingsForm settingsForm;
			team = new Sport.Entities.Team(_teamId);
			if (!team.IsValid())
			{
				return;
			}

			//abort if no players were selected:
			if (stickersList.Items.Count == 0)
				return;

			if (!Sport.UI.Helpers.GetPrinterSettings(out ps, out settingsForm))
				return;

			//ask user to choose the settings:
			settingsForm.Landscape = false;
			if (settingsForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				//user confirmed settings, initialize document builder:
				Sport.Documents.DocumentBuilder db = CreateStickersDocument(ps);

				//show preview if user asked:
				if (settingsForm.ShowPreview)
				{
					//build the preview, let user cancel:
					Sport.UI.Dialogs.PrintForm printForm = new Sport.UI.Dialogs.PrintForm(db.CreateDocument(), ps);

					//show the preview if not canceled:
					if (!printForm.Canceled)
						printForm.ShowDialog();

					//done with preview.
					printForm.Dispose();
				}
				else
				{
					//user don't want preview, print directly:
					System.Drawing.Printing.PrintDocument pd = db.CreateDocument().CreatePrintDocument(ps);
					pd.PrintController = new PrintControllerWithPageForm(pd.PrintController, 0);
					pd.Print();
				}
			}
		}

		private void cardsList_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			btnToCards.Enabled = btnToStickers.Enabled = (playersList.SelectedIndex >= 0);
			btnRemoveCards.Enabled = (cardsList.SelectedIndex >= 0);
			btnRemoveStickers.Enabled = (stickersList.SelectedIndex >= 0);
		}

		private void cardsList_DoubleClick(object sender, System.EventArgs e)
		{
			btnRemoveCards_Click(sender, e);
		}

		private void stickersList_DoubleClick(object sender, System.EventArgs e)
		{
			btnRemoveStickers_Click(sender, e);
		}

		private void playersList_DoubleClick(object sender, System.EventArgs e)
		{
			if (cardsList.Items.Count > 0)
			{
				btnToCards_Click(sender, e);
			}
			else
			{
				btnToStickers_Click(sender, e);
			}
		}

		private void button1_Click(object sender, System.EventArgs e)
		{
			Core.Tools.InputMargins("PlayerCardMargins", 0, 0);
		}

		private void button2_Click(object sender, System.EventArgs e)
		{
			Core.Tools.InputMargins("PlayerStickerMargins", 37, 50);
		}

		private class PlayerItem
		{
			public string playerName;
			public string IdNumber;
			public int playerID;

			public PlayerItem(string name, string number, int id)
			{
				this.playerName = name;
				this.IdNumber = number;
				this.playerID = id;
			}

			public override string ToString()
			{
				return playerName + " " + IdNumber;
			}

			public override bool Equals(object obj)
			{
				if (!(obj is PlayerItem))
					return false;
				return ((obj as PlayerItem).playerID == this.playerID);
			}

			public override int GetHashCode()
			{
				return this.playerID;
			}

		} //end class PlayerItem
	} //end class PlayerCardBuildForm
}
