using System;
using Sport.Data;

namespace Sport.Types
{
	[Flags]
	public enum Permission
	{
		Administration,
		Sports,
		Championships,
		Accounts,
		General
	}

	public class PermissionTypeLookup : LookupType
	{
		public static readonly string[] Permissions = 
			{ "מערכת", "ספורט", "אליפויות", "חשבונות", "כללי" };

		private static string PermissionsToString(int permissions)
		{
			if (permissions == 0)
				return "ללא";

			string result = "";

			for (int n = 0; n < Permissions.Length; n++)
			{
				if ((permissions & 0x1) == 0x1)
				{
					if (result.Length != 0)
						result += ", ";
					result += Permissions[n];
				}

				permissions >>= 1;
			}

			return result;
		}

		public static string ToString(int permissions)
		{
			return PermissionsToString(permissions);
		}

		public static bool Contains(int permissions, Permission permission)
		{
			int perm = 1 << (int) permission;
			return ((permissions & perm) == perm);
		}

		public static int Add(int permissions, Permission permission)
		{
			return permissions | (1 << (int) permission);
		}

		public static int Remove(int permissions, Permission permission)
		{			
			return permissions & ~(1 << (int) permission);
		}

		public static LookupItem ToLookupItem(int permissions)
		{
			return new LookupItem(permissions, ToString(permissions));
		}

		public override LookupItem this[int permissions]
		{
			get
			{
				return ToLookupItem(permissions);
			}
		}

		public override string Lookup(int permissions)
		{
			return ToString(permissions);
		}
	}
}
