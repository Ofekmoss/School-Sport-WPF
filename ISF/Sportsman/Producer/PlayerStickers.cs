using System;
using System.Drawing;
using Sport.Documents;

namespace Sportsman.Producer
{
	#region PlayerStickerItem
	public class PlayerStickerItem : PageItem
	{
		private static Font			labelFont;
		private static Font			captionFont;
		private static StringFormat labelFormat;
		private Sport.Entities.Player _player;
		private Sport.Entities.Functionary _functionary;
		private static SolidBrush	overAgeBrush=new SolidBrush(Color.FromArgb(255, 164, 164));
		
		static PlayerStickerItem()
		{
			labelFont = new Font("Tahoma", 12, System.Drawing.GraphicsUnit.Pixel);
			captionFont = new Font("Tahoma", 12, FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			
			labelFormat = new StringFormat(StringFormatFlags.DirectionRightToLeft);
			labelFormat.Alignment = StringAlignment.Near;
		}

		public PlayerStickerItem(Rectangle bounds, Sport.Entities.Player player, Sport.Entities.Functionary functionary)
			: base(bounds)
		{
			Direction = Direction.Right;
			_player = player;
			_functionary = functionary;
		}

		public PlayerStickerItem(PlayerStickerItem playerSticker)
			: this(playerSticker.Bounds, playerSticker._player, playerSticker._functionary)
		{
		}
		
		public Sport.Entities.Player Player
		{
			get { return _player; }
			set { _player = value; }
		}
		
		public Sport.Entities.Functionary Functionary
		{
			get { return _functionary; }
			set { _functionary = value; }
		}
		
		public override void PaintItem(Graphics g)
		{
			base.PaintItem (g);
			
			//local data:
			int labelHeight=20;
			string strFirstLineData = null;
			string strSecondLineData = null;
			string strLastLineData = null;
			if (_player != null)
			{
				strFirstLineData = _player.Student.LastName+" "+_player.Student.FirstName;
				strSecondLineData = _player.Student.IdNumber;
				strLastLineData = _player.Team.Championship.Season.Name;
			}
			else if (_functionary != null)
			{
				strFirstLineData = _functionary.Name;
				strSecondLineData = _functionary.Address;
			}
			
			//sanity check
			if (strFirstLineData == null)
				return;
			
			//background if needed
			if (_player != null && _player.IsOverAge())
				g.FillRectangle(overAgeBrush, Bounds);
			
			//First Line (player name or functionary name):
			g.DrawString(strFirstLineData, captionFont, ForeBrush, 
				new Rectangle(Bounds.Left, Bounds.Top, Bounds.Width, labelHeight), labelFormat);
			
			//Second Line (Player ID or functionary address):
			g.DrawString(strSecondLineData, labelFont, ForeBrush, 
				new Rectangle(Bounds.Left, Bounds.Top+labelHeight, Bounds.Width, labelHeight), labelFormat);
			
			//Last Line (Season, if exist):
			g.DrawString(strLastLineData, labelFont, ForeBrush, 
				new Rectangle(Bounds.Left, Bounds.Bottom-labelHeight, Bounds.Width, labelHeight), labelFormat);
		} //end function PaintItem
	} //end class PlayerStickerItem
	#endregion
	
	#region PlayerStickersObject
	public class PlayerStickersObject : PageItem, ISectionObject
	{
		private readonly int STICKERS_PER_ROW=3;
		private StickerCollection _stickers;
		private int _currentSticker;
		
		public PlayerStickersObject(Rectangle bounds)
			: base(bounds)
		{
			_stickers = new StickerCollection(this);
		}
		
		public StickerCollection Stickers
		{
			get { return _stickers; }
		}
		
		#region ISectionObject Members
		public bool SavePage(DocumentBuilder builder, Page page)
		{
			int topMargin=Sport.Common.Tools.CIntDef(
				Sport.Core.Configuration.ReadString("PlayerStickerMargins", 
				"TopMargin"), 37);
			int leftMargin=Sport.Common.Tools.CIntDef(
				Sport.Core.Configuration.ReadString("PlayerStickerMargins", 
				"SideMargin"), 50);
			int stickerWidth=290;	//total width of each sticker. (3 like those in row)
			int stickerHeight=75;	//total height of each sticker.
			int verticalGap=15;		//gap between each row of stickers.
			int top = Bounds.Top+topMargin;
			int left = 0;
			
			while (((top+stickerHeight) <= Bounds.Bottom)&&(_currentSticker < _stickers.Count))
			{
				left = Bounds.Right-stickerWidth-leftMargin;
				for (int i=1; i<=STICKERS_PER_ROW; i++)
				{
					if (_currentSticker >= _stickers.Count)
						break;
					
					PlayerStickersObject.Sticker sticker = _stickers[_currentSticker];
					
					page.Items.Add(new PlayerStickerItem(
						new Rectangle(left, top, stickerWidth, stickerHeight), 
						sticker.Player, sticker.Functionary));
					
					left -= stickerWidth;
					_currentSticker++;
				}
				top += stickerHeight+verticalGap;
			} //end loop over cards
			
			return (_currentSticker<_stickers.Count);
		} //end function SavePage
		
		public void InitializeSave(DocumentBuilder builder)
		{
			_currentSticker = 0;
		}
		
		public void FinalizeSave(DocumentBuilder builder)
		{
		}
		#endregion
		
		#region Sticker
		public class Sticker
		{
			private Sport.Entities.Player _player = null;
			private Sport.Entities.Functionary _functionary = null;
			
			public Sticker(Sport.Entities.Player player)
			{
				_player = player;
			}
			
			public Sticker(Sport.Entities.Functionary functionary)
			{
				_functionary = functionary;
			}
			
			public Sport.Entities.Player Player
			{
				get { return _player; }
				set { _player = value; }
			}
			
			public Sport.Entities.Functionary Functionary
			{
				get { return _functionary; }
				set { _functionary = value; }
			}
		} //end class Sticker
		#endregion
		
		#region StickerCollection
		/// <summary>
		/// collection of Stickers to be printed.
		/// </summary>
		public class StickerCollection : Sport.Common.GeneralCollection
		{
			public StickerCollection(PlayerStickersObject stickersObject)
				: base(stickersObject)
			{
			}
			
			public Sticker this[int index]
			{
				get { return (Sticker) GetItem(index); }
				set { SetItem(index, value); }
			}

			public void Insert(int index, Sticker value)
			{
				InsertItem(index, value);
			}
			
			public void Remove(Sticker value)
			{
				RemoveItem(value);
			}
			
			public bool Contains(Sticker value)
			{
				return base.Contains(value);
			}
			
			public int IndexOf(Sticker value)
			{
				return base.IndexOf(value);
			}
			
			public int Add(Sticker value)
			{
				return AddItem(value);
			}
		} //end class StickerCollection
		#endregion
	} //end class PlayerStickersObject
	#endregion
}
