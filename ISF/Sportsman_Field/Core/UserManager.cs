using System;

namespace Sportsman_Field.Core
{
	public class User
	{
		public int ID;
		public string Name;
		public string Region;
	}

	/// <summary>
	/// Summary description for CurrentUser.
	/// </summary>
	public class UserManager
	{
		public static User CurrentUser=null;


	}
}
