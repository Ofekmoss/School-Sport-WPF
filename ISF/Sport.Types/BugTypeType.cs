using System;
using Sport.Data;

namespace Sport.Types
{
	public enum BugType
	{
		Error,
		Remark,
		Question
	}
	
	public class BugTypeLookup : LookupType
	{
		public static LookupItem[] types = new LookupItem[]
			{ 
				new LookupItem((int) BugType.Error, "דוח שגיאה"),
				new LookupItem((int) BugType.Remark, "הערה כללית"),
				new LookupItem((int) BugType.Question, "שאלה כללית")
			};

		public override string Lookup(int id)
		{
			return types[id].Text;
		}

		public override LookupItem this[int id]
		{
			get { return types[id]; }
		}

		public override LookupItem[] Items
		{
			get
			{
				return types;
			}
		}
	}
}
