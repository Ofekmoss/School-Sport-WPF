namespace SportSite.Controls
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Configuration;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	/// <summary>
	///		Summary description for FlashNews.
	/// </summary>
	public class FlashNews : System.Web.UI.UserControl
	{
		protected System.Web.UI.HtmlControls.HtmlGenericControl NewsPanel;

		private void Page_Load(object sender, System.EventArgs e)
		{
			//load today's news:
			NewsPanel.InnerHtml = BuildNewsHtml();
		}

		private string BuildNewsHtml()
		{
			string strHTML = "";
			object data;
			if (CacheStore.Instance.Get("FlashNews_Raw", out data))
			{
				strHTML = data.ToString();
			}
			else
			{
				System.Text.StringBuilder sb = new System.Text.StringBuilder();
				WebSiteServices.WebsiteService websiteService = new WebSiteServices.WebsiteService();
				WebSiteServices.FlashNewsData[] arrNews = websiteService.GetCurrentNews();
				int newsCount = 0;
				sb.Append("<div dir=\"rtl\" align=\"right\">");
				foreach (WebSiteServices.FlashNewsData newsData in arrNews)
				{
					if (newsCount > 20)
						break;
					string strURL = ConfigurationManager.AppSettings["Path"] + "/?action=";
					strURL += Common.SportSiteAction.ViewNews + "&id=" + newsData.ID;
					sb.Append("<p>");
					sb.Append("<span class=\"FlashNewsTime\">");
					sb.Append(newsData.Time.Hour.ToString().PadLeft(2, '0') + ":");
					sb.Append(newsData.Time.Minute.ToString().PadLeft(2, '0'));
					sb.Append("</span><br />");
					sb.Append("<span dir=\"rtl\">");
					sb.Append("<a href=\"" + strURL + "\" class=\"FlashNewsContents\" ");
					sb.Append(">");
					sb.Append(Common.Tools.TruncateString(newsData.Contents, 50) + "</a>");
					sb.Append("</span><br />");
					sb.Append("</p><hr />");
					newsCount++;
				}
				sb.Append("</div>");
				strHTML = sb.ToString();
				CacheStore.Instance.Update("FlashNews_Raw", strHTML, 5);
			}

			return strHTML;
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion
	}
}
