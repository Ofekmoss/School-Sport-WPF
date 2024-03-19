using System;
using Sport.Data;

namespace Sport.Types
{
	public enum FunctionaryStatus
	{
		UNDEFINED=0,
		Senior=1,
		Young,
		Other,
	}
	
	/// <summary>
	/// Summary description for FunctionaryStatusType.
	/// </summary>
	public class FunctionaryStatusTypeLookup : LookupType
	{
		public static LookupItem[] types = new LookupItem[]
			{ 
				new LookupItem((int) FunctionaryStatus.Senior, "בוגר"),
				new LookupItem((int) FunctionaryStatus.Young, "צעיר"),
				new LookupItem((int) FunctionaryStatus.Other, "אחר")
			};
		
		public override LookupItem this[int id]
		{
			get
			{
				if (!IsLegal(id))
					return new LookupItem(0, "לא ידוע");
				//throw new ArgumentException("Unknown functionary type: "+id.ToString());
				return types[id - 1];
			}
		}
		
		public override string Lookup(int id)
		{
			return this[id].Text;
		}
		
		private bool IsLegal(int id)
		{
			int count=Enum.GetValues(typeof(FunctionaryType)).Length-1;
			return ((id >= 1)&&(id <= count));
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
