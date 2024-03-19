using System;
using Sport.Data;

namespace Sport.Types
{
	public enum SchoolSector
	{
		Jewish=1,
		Arab,
		Unknown,
		Druse,
		Bedouin,
		Other
	}
	
	/// <summary>
	/// Summary description for SchoolSectorLookup.
	/// </summary>
	public class SchoolSectorLookup : LookupType
	{
		public static LookupItem[] types = new LookupItem[]
			{ 
				new LookupItem((int) SchoolSector.Jewish, "יהודי"),
				new LookupItem((int) SchoolSector.Arab, "ערבי"),
				new LookupItem((int) SchoolSector.Unknown, "???"),
				new LookupItem((int) SchoolSector.Druse, "דרוזי"),
				new LookupItem((int) SchoolSector.Bedouin, "בדואי"),
				new LookupItem((int) SchoolSector.Other, "אחר")
	};
		
		public override LookupItem this[int id]
		{
			get
			{
				if (!IsLegal(id))
					throw new ArgumentException("Unknown school sector: "+id.ToString());
				return types[id - 1];
			}
		}
		
		public override string Lookup(int id)
		{
			return this[id].Text;
		}
		
		private bool IsLegal(int id)
		{
			switch (id)
			{
				case (int) SchoolSector.Jewish:
				case (int) SchoolSector.Arab:
				case (int) SchoolSector.Druse:
				case (int) SchoolSector.Bedouin:
				case (int) SchoolSector.Other:
					return true;
			}

			return false;
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
