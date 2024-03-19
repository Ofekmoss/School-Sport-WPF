using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace SportSite
{
	public partial class TestFlash : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			string strURL1 = String.Format("{0}/Flash/437x74_basket13.swf", Common.Data.AppPath);
			string strURL2 = String.Format("{0}/Flash/437x74_kaduraf2.swf", Common.Data.AppPath);
			string strURL3 = String.Format("{0}/Flash/437x74_kaduraf4.swf", Common.Data.AppPath);
			int width = 437;
			int height = 74;
			Page.ClientScript.RegisterClientScriptInclude("common", String.Format("{0}/Common/Common.js", Common.Data.AppPath));
			Page.ClientScript.RegisterClientScriptInclude("swfobject", String.Format("{0}/Common/swfobject.js", Common.Data.AppPath));
			Page.ClientScript.RegisterStartupScript(this.GetType(), "flash1", String.Format("RegisterFlashMovie(\"{0}\", {1}, {2}, \"{3}\", \"\", \"#ffffff\"); ", strURL1, width, height, "MyFlashDiv1"), true);
			Page.ClientScript.RegisterStartupScript(this.GetType(), "flash2", String.Format("RegisterFlashMovie(\"{0}\", {1}, {2}, \"{3}\", \"\", \"#ffffff\"); ", strURL2, width, height, "MyFlashDiv2"), true);
			Page.ClientScript.RegisterStartupScript(this.GetType(), "flash3", String.Format("RegisterFlashMovie(\"{0}\", {1}, {2}, \"{3}\", \"\", \"#ffffff\"); ", strURL3, width, height, "MyFlashDiv3"), true);
			//			string strCode =  "<script type=\"text/javascript\" src=\""++"/SportSite.js\"></script>\n";
		}
	}
}
