using System;
using Sport.Data;

namespace Sport.Types
{
	public enum SchoolSupervisionType
	{
		State=1,
		Religious,
		Other
	}
	
	/// <summary>
	/// Summary description for SchoolSupervisionLookup.
	/// </summary>
	public class SchoolSupervisionLookup : LookupType
	{
		public static LookupItem[] types = new LookupItem[]
			{ 
				new LookupItem((int) SchoolSupervisionType.State, "ממלכתי"),
				new LookupItem((int) SchoolSupervisionType.Religious, "ממ\"ד"),
				new LookupItem((int) SchoolSupervisionType.Other, "אחר"),
		};
		
		public override LookupItem this[int id]
		{
			get
			{
				if (!IsLegal(id))
					throw new ArgumentException("Unknown school supervision type: "+id.ToString());
				return types[id - 1];
			}
		}
		
		public override string Lookup(int id)
		{
			return this[id].Text;
		}
		
		private bool IsLegal(int id)
		{
			switch (id)
			{
				case (int) SchoolSupervisionType.State:
				case (int) SchoolSupervisionType.Religious:
				case (int) SchoolSupervisionType.Other:
					return true;
			}

			return false;
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
