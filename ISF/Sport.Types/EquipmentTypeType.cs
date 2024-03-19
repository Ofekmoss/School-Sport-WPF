using System;
using Sport.Data;

/*	EQUIPMENT_TYPE_TYPE			int NOT NULL, --0 General/1 Region/2 Sport/3 Championship/4 Category */

namespace Sport.Types
{

	public enum EquipmentSeperationType
	{
		General = 0,
		Regions,
		Sports,
		Championships,
		Categories
	}

	public class EquipmentTypeLookup : LookupType
	{
		private static LookupItem[] types = new LookupItem[]  
		{ 
			new LookupItem((int) EquipmentSeperationType.General, "����"),
			new LookupItem((int) EquipmentSeperationType.Regions, "������"), 
			new LookupItem((int) EquipmentSeperationType.Sports, "���� �����"), 
			new LookupItem((int) EquipmentSeperationType.Championships, "��������"), 
			new LookupItem((int) EquipmentSeperationType.Categories, "�������� ������")
		};
		
		public override string Lookup(int id)
		{
			LookupItem item=this[id];
			return (item == null)?"":item.Text;
		}

		public override LookupItem this[int id]
		{
			get
			{
				LookupItem[] items=this.Items;
				foreach (LookupItem item in items)
				{
					if (item.Id == id)
						return item;
				}
				return null;
			}
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
