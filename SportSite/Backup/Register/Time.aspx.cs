using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SportSite.Core;

namespace SportSite.NewRegister
{
	public partial class Time : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			string message = "";
			if (Session[UserManager.SessionKey] == null)
			{
				message = "Not Authorized";
			}
			else
			{
				message = DateTime.Now.ToString("HH:mm");
			}
			Response.Clear();
			Response.Write(message);
			Response.End();
		}
	}
}