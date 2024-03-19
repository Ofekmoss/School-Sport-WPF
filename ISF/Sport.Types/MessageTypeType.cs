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
				new LookupItem((int) MessageType.General, "����� �����"),
				new LookupItem((int) MessageType.CanRegisterTeams, "����� ������ ����"),
				new LookupItem((int) MessageType.CanRegisterPlayers, "����� ������ ����"),
				new LookupItem((int) MessageType.TeamRegistered, "����� �����"),
				new LookupItem((int) MessageType.PlayersRegistered, "������ �����"),
				new LookupItem((int) MessageType.TeamStatusChanged, "���� ����� �����"),
				new LookupItem((int) MessageType.PlayerStatusChanged, "���� ����� ������"),
				new LookupItem((int) MessageType.NewClubRegister, "����� ������ ���")
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
