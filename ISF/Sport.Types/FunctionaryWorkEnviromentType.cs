using System;
using Sport.Data;

namespace Sport.Types
{
	public enum FunctionaryWorkEnviroment
	{
		UNDEFINED=0,
		Club=1,
		Ordinary,
		Other,
	}
	
	/// <summary>
	/// Summary description for FunctionaryWorkEnviromentType.
	/// </summary>
	public class FunctionaryWorkEnviromentLookup : LookupType
	{
		public static LookupItem[] types = new LookupItem[]
			{ 
				new LookupItem((int) FunctionaryWorkEnviroment.Club, "��������"),
				new LookupItem((int) FunctionaryWorkEnviroment.Ordinary, "�����"),
				new LookupItem((int) FunctionaryWorkEnviroment.Other, "���")
			};
		
		public override LookupItem this[int id]
		{
			get
			{
				if (!IsLegal(id))
					return new LookupItem(0, "�� ����");
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
