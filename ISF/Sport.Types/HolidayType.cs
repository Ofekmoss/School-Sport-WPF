using System;
using Sport.Data;

namespace Sport.Types
{
	public enum HolidayType
	{
		Passover = 0, 
		Sukkut,
		Hanukka
	}
	public class HolidayTypeLookup : LookupType
	{
		public static LookupItem[] types = new LookupItem[]
			{ 
				new LookupItem((int) HolidayType.Passover, "���"),
				new LookupItem((int) HolidayType.Sukkut, "�����"),
				new LookupItem((int) HolidayType.Hanukka, "�����")
			};
		
		public override LookupItem this[int id]
		{
			get
			{
				if (!IsLegal(id))
					return new LookupItem(-1, "�� ����");
				//throw new ArgumentException("Unknown functionary type: "+id.ToString());
				return types[id];
			}
		}
		
		public override string Lookup(int id)
		{
			return this[id].Text;
		}
		
		private bool IsLegal(int id)
		{
			int count = Enum.GetValues(typeof(HolidayType)).Length;
			return ((id >= 0)&&(id < count));
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
