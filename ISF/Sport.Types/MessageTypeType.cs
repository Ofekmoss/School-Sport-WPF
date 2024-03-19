using System;
using Sport.Data;

namespace Sport.Types
{
	public enum MessageType
	{
		General=0,
		CanRegisterTeams,
		CanRegisterPlayers,
		TeamRegistered,
		PlayersRegistered,
		TeamStatusChanged,
		PlayerStatusChanged,
		NewClubRegister
	}
	
	/// <summary>
	/// Summary description for MessageTypeType.
	/// </summary>
	public class MessageTypeLookup : LookupType
	{
		public static LookupItem[] types = new LookupItem[]
			{ 
				new LookupItem((int) MessageType.General, "הודעה כללית"),
				new LookupItem((int) MessageType.CanRegisterTeams, "רישום קבוצות נפתח"),
				new LookupItem((int) MessageType.CanRegisterPlayers, "רישום שחקנים נפתח"),
				new LookupItem((int) MessageType.TeamRegistered, "קבוצה נרשמה"),
				new LookupItem((int) MessageType.PlayersRegistered, "שחקנים נרשמו"),
				new LookupItem((int) MessageType.TeamStatusChanged, "שונה סטטוס קבוצה"),
				new LookupItem((int) MessageType.PlayerStatusChanged, "שונה סטטוס שחקנים"),
				new LookupItem((int) MessageType.NewClubRegister, "רישום מועדון חדש")
			};
		
		public override string Lookup(int id)
		{
			if (!IsLegal(id))
				throw new ArgumentException("Unknown message type: "+id.ToString());
			return types[id].Text;
		}
		
		public override LookupItem this[int id]
		{
			get
			{
				if (!IsLegal(id))
					throw new ArgumentException("Unknown message type: "+id.ToString());
				return types[id];
			}
		}

		private bool IsLegal(int id)
		{
			bool result=false;
			switch (id)
			{
				case (int) MessageType.General:
				case (int) MessageType.CanRegisterTeams:
				case (int) MessageType.CanRegisterPlayers:
				case (int) MessageType.TeamRegistered:
				case (int) MessageType.PlayersRegistered:
				case (int) MessageType.TeamStatusChanged:
				case (int) MessageType.PlayerStatusChanged:
				case (int) MessageType.NewClubRegister:
					result = true;
					break;
			}
			return result;
		}

		public override LookupItem[] Items
		{
			get
			{
				return types;
			}
		}
	} //end class MessageTypeLookup
}
