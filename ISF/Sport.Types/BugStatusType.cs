using System;
using Sport.Data;

namespace Sport.Types
{
	public enum BugStatus
	{
		New,
		Fixed,
		Checked,
		InProcess,
		Old
	}

	#region Bug Status Lookup
	public class BugStatusLookup : LookupType
	{
		public static LookupItem[] statuses = new LookupItem[]
			{ 
				new LookupItem((int) BugStatus.New, "חדש"),
				new LookupItem((int) BugStatus.Fixed, "תוקן"),
				new LookupItem((int) BugStatus.Checked, "ניתנה תשובה"),
				new LookupItem((int) BugStatus.InProcess, "בטיפול"),
				new LookupItem((int) BugStatus.Old, "ארכיון")
			};

		public override string Lookup(int id)
		{
			return statuses[id].Text;
		}

		public override LookupItem this[int id]
		{
			get { return statuses[id]; }
		}

		public override LookupItem[] Items
		{
			get
			{
				return statuses;
			}
		}
	}
	#endregion
}
