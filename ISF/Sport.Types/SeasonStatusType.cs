using System;
using Sport.Data;

namespace Sport.Types
{
	public enum SeasonStatus
	{
		New=0,
		Opened,
		Closed
	}

	public class SeasonStatusType : LookupType
	{
		public static LookupItem[] types = new LookupItem[]
			{ 
				new LookupItem((int) SeasonStatus.New, "òåğä çãùä"),
				new LookupItem((int) SeasonStatus.Opened, "òåğä ôúåçä"),
				new LookupItem((int) SeasonStatus.Closed, "òåğä ñâåøä")
		};
		
		public override string Lookup(int id)
		{
			if (!IsLegal(id))
				throw new ArgumentException("Unknown season status: "+id.ToString());
			return types[id].Text;
		}

		public override LookupItem this[int id]
		{
			get
			{
				if (!IsLegal(id))
					throw new ArgumentException("Unknown season status: "+id.ToString());
				return types[id];
			}
		}
		
		private bool IsLegal(int id)
		{
			return (id >= 0)&&(id < types.Length);
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
