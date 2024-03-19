using System;

namespace Sport.Core
{
	/// <summary>
	/// Summary description for Permissions.
	/// </summary>
	public class PermissionsManager
	{
		private static string[] _arrPublicParams=null;

		static PermissionsManager()
		{
			_arrPublicParams=new string[] {"Sport.UI.Display.RestateCommand", "Sportsman.Commands.AboutCommand", 
							"Sportsman.Commands.CloseCommand", "Sportsman.Commands.DeleteDatFilesCommand", "Sportsman.Forms.KeyboardForm", "calendarbar"};
		}

		public static bool IsSuperUser(int userID)
		{
			return (userID == 1)||(userID == 109)||(userID == 110)||(userID == 113);
		}

		public static bool IsPublicParameter(string strParameter)
		{
			if ((strParameter == null)||(strParameter.Length == 0))
				return true;
			
			for (int i=0; i<_arrPublicParams.Length; i++)
			{
				string curParam=_arrPublicParams[i];
				if (strParameter.ToLower().IndexOf(curParam.ToLower()) >= 0)
					return true;
			}
			return false;
		}
	}
}
