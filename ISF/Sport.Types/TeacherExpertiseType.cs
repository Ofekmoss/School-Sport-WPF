using System;
using Sport.Data;

namespace Sport.Types
{
	public enum TeacherExpertise
	{
		None=0,
		Teacher,
		Coach,
		Guide,
		TeacherAndCoach,
		TeacherAndGuide
	}
	
	public class TeacherExpertiseLookup : LookupType
	{
		public static LookupItem[] types = new LookupItem[]
			{ 
				new LookupItem((int) TeacherExpertise.None, "ììà äëùøä"),
				new LookupItem((int) TeacherExpertise.Teacher, "îåøä	"),
				new LookupItem((int) TeacherExpertise.Coach, "îàîï"),
				new LookupItem((int) TeacherExpertise.Guide, "îãøéê"),
				new LookupItem((int) TeacherExpertise.TeacherAndCoach, "îåøä åîğäì"),
				new LookupItem((int) TeacherExpertise.TeacherAndGuide, "îåøä åîãøéê")
			};
		
		public override LookupItem this[int id]
		{
			get
			{
				if (!IsLegal(id))
					return new LookupItem(-1, "ìà éãåò");
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
			int count = Enum.GetValues(typeof(TeacherExpertise)).Length;
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
