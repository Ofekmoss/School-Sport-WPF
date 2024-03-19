using System;
using Sport.Data;

namespace Sport.Types
{
	public enum UserType
	{
		Internal = 1,
		External
	}

	public class UserTypeLookup : LookupType
	{
		public static LookupItem[] types = new LookupItem[]
			{ 
				new LookupItem((int) UserType.Internal, "משתמש התאחדות"),
				new LookupItem((int) UserType.External, "משתמש חיצוני"),
		};

		public override string Lookup(int id)
		{
			if (!IsLegal(id))
				throw new ArgumentException("Unknown user type id: "+id.ToString());
			return types[id-1].Text;
		}

		public override LookupItem this[int id]
		{
			get
			{
				if (!IsLegal(id))
					throw new ArgumentException("Unknown user type id: "+id.ToString());
				return types[id - 1];
			}
		}

		private bool IsLegal(int id)
		{
			bool result=false;
			switch (id)
			{
				case (int) UserType.External:
				case (int) UserType.Internal:
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
	} //end class UserTypeLookup

}
