namespace SportSite.Controls
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	/// <summary>
	///		Summary description for Advertisement.
	/// </summary>
	public class Advertisement : System.Web.UI.UserControl
	{
		protected System.Web.UI.WebControls.Panel AdvertisementPanel;
		protected System.Web.UI.HtmlControls.HtmlGenericControl AdvertisementFrame;

		public static readonly string DEFAULT_150_150=Common.Data.AppPath+
			"/Images/Banners/default_150_150.gif";
		
		public static readonly string DEFAULT_760_80=Common.Data.AppPath+
			"/Images/Banners/default_760_80.gif";
		
		private void Page_Load(object sender, System.EventArgs e)
		{
			int panelWidth=Common.Tools.CIntDef(this.Attributes["width"], 0);
			int panelHeight=Common.Tools.CIntDef(this.Attributes["height"], 0);
			if ((panelWidth > 0)&&(panelHeight > 0))
			{
				AdvertisementPanel.Width = new Unit(panelWidth+"px");
				AdvertisementPanel.Height = new Unit(panelHeight+"px");
				AdvertisementFrame.Attributes["width"] = panelWidth.ToString();
				AdvertisementFrame.Attributes["height"] = panelHeight.ToString();
				if (this.Attributes["ad_type"] != null)
					ApplyFrameSettings(Common.Tools.CIntDef(this.Attributes["ad_type"], 0));

				WebSiteServices.WebsiteService websiteService=
					new WebSiteServices.WebsiteService();
				WebSiteServices.BannerData[] banners=websiteService.GetBanners();
				string strURL="about:blank";
				int randNum=-1;
				if ((banners != null)&&(banners.Length > 0))
					randNum = (new Random()).Next(0, banners.Length);
				if ((panelWidth == 150)&&(panelHeight == 150))
				{
					if (randNum < 0)
					{
						strURL = DEFAULT_150_150;
					}
					else
					{
						strURL = banners[randNum].url_150_150;
						if ((strURL == null)||(strURL.Length < 3))
							strURL = DEFAULT_150_150;
					}
				}
				else
				{
					if ((panelWidth == 760)&&(panelHeight == 80))
					{
						if (randNum < 0)
						{
							strURL = DEFAULT_760_80;
						}
						else
						{
							strURL = banners[randNum].url_760_80;
							if ((strURL == null)||(strURL.Length < 3))
								strURL = DEFAULT_760_80;
						}
					}
				}
				
				AdvertisementFrame.Attributes["src"] = strURL;
			}
			//AdvertisementFrame.Attributes["src"] = "http://www.google.co.uk";
			//AdvertisementFrame.Attributes["width"] = "250";
			//AdvertisementFrame.Attributes["height"] = "150";
		}

		private void ApplyFrameSettings(int type)
		{
			switch (type)
			{
				case 1:
					AdvertisementFrame.Attributes["marginwidth"] = "0";
					AdvertisementFrame.Attributes["marginheight"] = "0";
					AdvertisementFrame.Attributes["frameborder"] = "no";
					AdvertisementFrame.Attributes["border"] = "0";
					AdvertisementFrame.Attributes["scrolling"] = "no";
					break;
				case 2:
					AdvertisementFrame.Attributes["marginwidth"] = "0";
					AdvertisementFrame.Attributes["marginheight"] = "0";
					AdvertisementFrame.Attributes["scrolling"] = "no";
					AdvertisementFrame.Attributes["frameborder"] = "no";
					AdvertisementFrame.Style["border"] = "3px solid #F7F3F7";
					break;
			}
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
