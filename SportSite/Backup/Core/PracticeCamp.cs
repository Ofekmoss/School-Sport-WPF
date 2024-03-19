using System;

namespace SportSite.Core
{
	public class PracticeCamp
	{
		public static int GetDynamicPageID(System.Web.HttpServerUtility Server)
		{
			string strValue = Common.Tools.ReadIniValue("PracticeCamp", "DynamicPage", Server);
			return Common.Tools.CIntDef(strValue, -1);
		}
		
		public static void SetDynamicPageID(int pageID, System.Web.HttpServerUtility Server)
		{
			Common.Tools.WriteIniValue("PracticeCamp", "DynamicPage", pageID.ToString(), Server);
		}
	}
}
