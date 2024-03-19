using System;
using Sport.Data;

namespace Sport.Types
{
	/// <summary>
	/// used to represent boolean entity type.
	/// </summary>
	public class BooleanTypeLookup : LookupType
	{
		private LookupItem[] values;

		public BooleanTypeLookup(string trueString, string falseString)
		{
			values = new LookupItem[2];
			values[0] = new LookupItem(0, falseString);
			values[1] = new LookupItem(1, trueString);
		}

		public BooleanTypeLookup()
			: this("כן", "לא")
		{
		}

		public override LookupItem this[int id]
		{
			get
			{
				if (id < 0 || id >= values.Length)
					return null;
				return values[id];
			}
		}

		public override string Lookup(int id)
		{
			if (id < 0 || id >= values.Length)
				return "";
			return values[id].Text;
		}

		public override LookupItem[] Items
		{
			get
			{
				return values;
			}
		}
	}
}
