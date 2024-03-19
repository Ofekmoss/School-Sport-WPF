using System;
using Sport.Data;

namespace Sport.Types
{
	public enum FunctionaryType
	{
		UNDEFINED=0,
		Coordinator=1,
		Referee,
		ClubChairman,
		SupervisorTeacher,
		SchoolManager,
		SportDepartmentManager,
		SportDepartment,
		FacilitySupervisor,
		Coach
	}

	/// <summary>
	/// Summary description for SchoolSupervisionLookup.
	/// </summary>
	public class FunctionaryTypeLookup : LookupType
	{
		public static LookupItem[] types = new LookupItem[]
			{ 
				new LookupItem((int) FunctionaryType.Coordinator, "רכז"),
				new LookupItem((int) FunctionaryType.Referee, "שופט"),
				new LookupItem((int) FunctionaryType.ClubChairman, "יו\"ר הנהלת מועדון"),
				new LookupItem((int) FunctionaryType.SupervisorTeacher, "מורה אחראי"),
				new LookupItem((int) FunctionaryType.SchoolManager, "מנהל בית ספר"),
				new LookupItem((int) FunctionaryType.SportDepartmentManager, "מנהל מחלקת ספורט"),
				new LookupItem((int) FunctionaryType.SportDepartment, "מחלקת ספורט"),
				new LookupItem((int) FunctionaryType.FacilitySupervisor, "אחראי מתקן"),
				new LookupItem((int) FunctionaryType.Coach, "מאמן")
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
