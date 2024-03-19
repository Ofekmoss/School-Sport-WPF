using System;
using Sport.Data;

namespace Sport.Types
{
	public enum MessageStatus
	{
		New=1,
		Read,
		Archieve
	}
	
	public class MessageStatusLookup : LookupType
	{
		public static LookupItem[] types = new LookupItem[]
			{ 
				new LookupItem((int) MessageStatus.New, "חדשה"),
				new LookupItem((int) MessageStatus.Read, "נקראה"),
				new LookupItem((int) MessageStatus.Archieve, "ארכיון")
		};
		
		public override string Lookup(int id)
		{
			if (!IsLegal(id))
				throw new ArgumentException("Unknown message status id: "+id.ToString());
			return types[id-1].Text;
		}
		
		public override LookupItem this[int id]
		{
			get
			{
				if (!IsLegal(id))
					throw new ArgumentException("Unknown message status id: "+id.ToString());
				return types[id - 1];
			}
		}

		private bool IsLegal(int id)
		{
			bool result=false;
			switch (id)
			{
				case (int) MessageStatus.New:
				case (int) MessageStatus.Read:
				case (int) MessageStatus.Archieve:
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
	} //end class MessageStatusLookup
}
