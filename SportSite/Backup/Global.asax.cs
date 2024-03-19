using System;
using System.Collections;
using System.ComponentModel;
using System.Web;
using System.Web.SessionState;
using System.Net;

namespace SportSite 
{
	/// <summary>
	/// Summary description for Global.
	/// </summary>
	public class Global : System.Web.HttpApplication
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		public Global()
		{
			InitializeComponent();
		}	
		
		protected void Application_Start(Object sender, EventArgs e)
		{
			//Application["ini_file_path"] = Server.MapPath(Common.Data.INI_FILE_NAME);
		}
 
		protected void Session_Start(Object sender, EventArgs e)
		{

		}

		protected void Application_BeginRequest(Object sender, EventArgs e)
		{

		}

		protected void Application_EndRequest(Object sender, EventArgs e)
		{

		}

		protected void Application_AuthenticateRequest(Object sender, EventArgs e)
		{

		}

		protected void Application_Error(Object sender, EventArgs e)
		{

		}

		protected void Session_End(Object sender, EventArgs e)
		{

		}

		protected void Application_End(Object sender, EventArgs e)
		{

		}

		protected void Application_PreRequestHandlerExecute(Object sender, EventArgs e)
		{
			if (HttpContext.Current.Request.Url.ToString().IndexOf("/SwimCup", StringComparison.CurrentCultureIgnoreCase) >= 0)
			{
				//Server.Transfer("http://62.219.14.227/$sitepreview/isfisrael.org/");
				//Response.Status = "301 Moved Permanently";
				//Response.AddHeader("Location", currentUrl);
				string url = "http://62.219.14.227/$sitepreview/isfisrael.org/";
				string rawHTML = (new WebClient()).DownloadString(url);
				rawHTML = rawHTML.Replace("type=\"text/css\" href=\"", "type=\"text/css\" href=\"" + url);
				rawHTML = rawHTML.Replace("src=\"", "src=\"" + url);
				Response.Clear();
				Response.Write(rawHTML);
				Response.End();
			}
		}
			
		#region Web Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.components = new System.ComponentModel.Container();
		}
		#endregion
	}
}

