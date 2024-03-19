using System;
using Sport.Data;

namespace Sport.Types
{
	public class SexTypeLookup : LookupType
	{
		private static LookupItem[] sexes = new LookupItem[]  
		{ 
			new LookupItem((int) Sex.Boys, "æ'"),
			new LookupItem((int) Sex.Girls, "ð'")
		};
		
		public override string Lookup(int id)
		{
			LookupItem item=this[id];
			return (item == null)?"":item.Text;
		}

		public override LookupItem this[int id]
		{
			get
			{
				LookupItem[] items=this.Items;
				foreach (LookupItem item in items)
				{
					if (item.Id == id)
						return item;
				}
				return null;
			}
		}

		public override LookupItem[] Items
		{
			get
			{
				return sexes;
			}
		}
	}
}
