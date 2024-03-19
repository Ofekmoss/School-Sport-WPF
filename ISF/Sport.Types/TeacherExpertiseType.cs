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
				new LookupItem((int) TeacherExpertise.None, "��� �����"),
				new LookupItem((int) TeacherExpertise.Teacher, "����	"),
				new LookupItem((int) TeacherExpertise.Coach, "����"),
				new LookupItem((int) TeacherExpertise.Guide, "�����"),
				new LookupItem((int) TeacherExpertise.TeacherAndCoach, "���� �����"),
				new LookupItem((int) TeacherExpertise.TeacherAndGuide, "���� ������")
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
