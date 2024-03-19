using System;
using System.Drawing;
using Sport.Documents;

namespace Sportsman.Producer
{
	public class PlayerCardItem : PageItem
	{
		private static Font			headerFont;
		private static Font			labelFont;
		private static Font			captionFont;
		private static StringFormat labelFormat;
		private static StringFormat nameFormat;
		private static SolidBrush	overAgeBrush=new SolidBrush(Color.FromArgb(255, 164, 164));
		
		static PlayerCardItem()
		{
			headerFont = new Font("Tahoma", 16, System.Drawing.GraphicsUnit.Pixel);
			labelFont = new Font("Tahoma", 12, System.Drawing.GraphicsUnit.Pixel);
			captionFont = new Font("Tahoma", 12, FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			labelFormat = new StringFormat(StringFormatFlags.DirectionRightToLeft);
			nameFormat = new StringFormat(StringFormatFlags.DirectionRightToLeft);
			nameFormat.LineAlignment = StringAlignment.Center;
			nameFormat.Alignment = StringAlignment.Far;
		}

		private int _rightMargin=0;
		
		public PlayerCardItem(Rectangle bounds, Sport.Entities.Player player,
			DateTime issueDate, System.Drawing.Image picture, int rightMargin)
			: base(bounds)
		{
			Direction = Direction.Right;
			_player = player;
			_issueDate = issueDate;
			_picture = picture;
			_rightMargin = rightMargin;
		}

		public PlayerCardItem(Rectangle bounds, Sport.Entities.Player player)
			: this(bounds, player, DateTime.MinValue, null, 0)
		{
		}

		public PlayerCardItem(Rectangle bounds)
			: this(null)
		{
		}

		public PlayerCardItem(PlayerCardItem playerCard)
			: this(playerCard.Bounds, playerCard._player, playerCard._issueDate,
			playerCard._picture, 0)
		{
		}

		#region Player Properties

		private Sport.Entities.Player _player;
		public Sport.Entities.Player Player
		{
			get { return _player; }
			set { _player = value; }
		}

		private DateTime _issueDate;
		public DateTime IssueDate
		{
			get { return _issueDate; }
			set { _issueDate = value; }
		}

		private System.Drawing.Image _picture;
		public System.Drawing.Image Picture
		{
			get { return _picture; }
			set { _picture = value; }
		}

		#endregion

		public override void PaintItem(Graphics g)
		{
			base.PaintItem (g);

			int labelHeight=15;		//height of label
			int headerHeight=48;//20;	//height of header
			int teamNameGap=12;//25;		//vertical gap between team name and school symbol
			int labelWidth=260;//150;		//maximum width of the labels.
			int captionWidth=80;	//maximum width of label caption.
			int seperatorWidth=20;	//width of seperator between label and caption
			int labelVerticalGap=3; //vertical gap between each label
			int pictureWidth=100; //cardTotalWidth-captionWidth-labelWidth-seperatorWidth;
			int pictureHeight=100; //cardTotalHeight-headerHeight-teamNameGap-headerHeight;


			int left = Bounds.Left;
			int top = Bounds.Top;
			
			//background if needed
			if (_player.IsOverAge())
				g.FillRectangle(overAgeBrush, Bounds);
			
			// Team name
			g.DrawString(_player.Team.TeamName(), headerFont, ForeBrush,
				new Rectangle(left, top, labelWidth, headerHeight), nameFormat);

			top += headerHeight + teamNameGap;
			// School symbol
			g.DrawString(_player.Team.School.Symbol, headerFont, ForeBrush,
				new Rectangle(left, top, labelWidth, headerHeight));

			top += headerHeight;
			// Picture if exists
			if (_picture != null)
			{
				Size picSize=Sport.Common.Tools.SmartResize(_picture, 
					Math.Max(pictureWidth, pictureHeight));
				g.DrawImage(_picture, left, 
					top + 10, picSize.Width, picSize.Height);
			}
            
			string seperator=" : ";
			string[] captions = new string[] {"ענף/קבוצה", "שם", "ת.ז.", 
				"ת. לידה", "כיתה", "ת. הוצאה"};
			string[] labels = new string[] {
											   _player.Team.Championship.Sport.Name,
											   _player.Name, 
											   _player.Student.IdNumber, 
											   _player.Student.BirthDate.ToString("dd/MM/yy"),
											   new Sport.Types.GradeTypeLookup(true).Lookup(_player.Student.Grade),
											   _issueDate.ToString("dd/MM/yy")
										   };
			
			left -= _rightMargin;
			for (int i = 0; i < captions.Length; i++)
			{
				//label
				g.DrawString(labels[i], labelFont, ForeBrush,
					new Rectangle(left, top, labelWidth, labelHeight), labelFormat);

				//seperator
				g.DrawString(seperator, captionFont, ForeBrush,
					new Rectangle(left + labelWidth, top, seperatorWidth, labelHeight), labelFormat);

				//caption
				g.DrawString(captions[i], captionFont, ForeBrush,
					new Rectangle(left + labelWidth + seperatorWidth, top, captionWidth, labelHeight), labelFormat);

				top += labelHeight + labelVerticalGap;
			}
            
		}
	}
	
	public class PlayerCardsObject2 : PageItem, ISectionObject
	{
		private PlayerCardCollection _cards;
		private int currentCard;
		
		public PlayerCardsObject2(Rectangle bounds)
			: base(bounds)
		{
			_cards = new PlayerCardCollection(this);
		}
		
		public PlayerCardCollection Cards
		{
			get { return _cards; }
		}
		
		#region ISectionObject Members
		public bool SavePage(DocumentBuilder builder, Page page)
		{
			int topMargin=Sport.Common.Tools.CIntDef(
				Sport.Core.Configuration.ReadString("PlayerCardMargins", 
				"TopMargin"), 0);
			int rightMargin=Sport.Common.Tools.CIntDef(
				Sport.Core.Configuration.ReadString("PlayerCardMargins", 
				"SideMargin"), 0);
			int labelWidth = 260;		//maximum width of the labels.
			int seperatorWidth = 20;	//width of seperator between label and caption
			int captionWidth = 80;		//maximum width of label caption.
			int cardTotalWidth = labelWidth + seperatorWidth + captionWidth;
			int headerHeight = 48;		//height of header
			int teamNameGap = 12;		//vertical gap between team name and school symbol
			int labelHeight = 15;		//height of label
			int labelVerticalGap = 3;	//vertical gap between each label
			int cardTotalHeight = headerHeight + teamNameGap + headerHeight + 
				(6*labelHeight) + ((5)*labelVerticalGap);
			int horizontalGap = 28;		//gap between each student card.
			int verticalGap = 60;		//gap between each student card.
			
			int top = Bounds.Top+topMargin;
			int left = 0;
			while (top + cardTotalHeight <= Bounds.Bottom && currentCard < _cards.Count)
			{
				left = Bounds.Left;
				while (left + cardTotalWidth <= Bounds.Right && currentCard < _cards.Count)
				{
					page.Items.Add(new PlayerCardItem(
						new Rectangle(left, top, cardTotalWidth, cardTotalHeight), 
						_cards[currentCard].Player, 
						_cards[currentCard].IssueDate,
						_cards[currentCard].Picture, 
						rightMargin));

					left += cardTotalWidth + horizontalGap;
					currentCard++;
				}
				
				top += cardTotalHeight + verticalGap;
			} //end loop over cards
			
			return currentCard < _cards.Count;
		} //end function SavePage
		
		public void InitializeSave(DocumentBuilder builder)
		{
			currentCard = 0;
		}
		
		public void FinalizeSave(DocumentBuilder builder)
		{
		}
		#endregion
		
		#region PlayerCard
		public class PlayerCard
		{
			private Sport.Entities.Player _player;
			private DateTime _issueDate;
			private Image _picture;
			
			public PlayerCard(Sport.Entities.Player player, DateTime issueDate,
				Image picture)
			{
				_player = player;
				_issueDate = issueDate;
				_picture = picture;
			}
			
			public Sport.Entities.Player Player
			{
				get { return _player; }
				set { _player = value; }
			}
			
			public DateTime IssueDate
			{
				get { return _issueDate; }
				set { _issueDate = value; }
			}
			
			public Image Picture
			{
				get { return _picture; }
				set { _picture = value; }
			}
		}
		#endregion
		
		#region PlayerCardCollection
		/// <summary>
		/// collection of Player Cards to be printed.
		/// </summary>
		public class PlayerCardCollection : Sport.Common.GeneralCollection
		{
			public PlayerCardCollection(PlayerCardsObject2 playerCardsObject)
				: base(playerCardsObject)
			{
			}
			
			public PlayerCard this[int index]
			{
				get { return (PlayerCard) GetItem(index); }
				set { SetItem(index, value); }
			}

			public void Insert(int index, PlayerCard value)
			{
				InsertItem(index, value);
			}
			
			public void Remove(PlayerCard value)
			{
				RemoveItem(value);
			}
			
			public bool Contains(PlayerCard value)
			{
				return base.Contains(value);
			}
			
			public int IndexOf(PlayerCard value)
			{
				return base.IndexOf(value);
			}
			
			public int Add(PlayerCard value)
			{
				return AddItem(value);
			}
		}
		#endregion
	} //end class PlayerCardsObject2
}
