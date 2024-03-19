namespace SportSite.Controls
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Text;
	using System.Xml;
	using System.Collections.Generic;
	using SportSite.Core;
	using SportSite.Common;
	using System.IO;

	/// <summary>
	///		Summary description for MainView.
	/// </summary>
	public class MainView : System.Web.UI.UserControl
	{
		public enum AlternativeContentPanel
		{
			AdminDashboard = 1,
			SchoolDashboard
		}

		public Common.ClientSide clientSide = null;
		public System.Web.UI.WebControls.Literal lbStyle;
		public System.Web.UI.WebControls.Literal lbOnloadJS;
		public System.Web.UI.WebControls.Label ErrorsLabel;
		public System.Web.UI.WebControls.Label successLabel;
		public System.Web.UI.WebControls.Label lbPoweredBy;
		public SportSite.Controls.HebDateTime IsfHebDateTime;
		public SportSite.Controls.SideNavBar SideNavBar;
		public SportSite.Controls.LinkBox LeftNavBar;
		public System.Web.UI.WebControls.Literal ExtraDetails;
		public SportSite.Controls.LinkBox OrdersBasket;
		public SportSite.Controls.Banner TopBanner;
		public SportSite.Controls.Banner MiddleBanner;
		//public SportSite.Controls.Banner HotSaleBanner;
		public SportSite.Controls.Banner SmallBanner;
		public SportSite.Controls.Banner BottomBanner;
		public SportSite.Controls.Article IsfMainArticle;
		public SportSite.Controls.Article IsfSubArticle;
		public SportSite.Controls.Article IsfExtraArticle;
		//public SportSite.Controls.OnlinePoll IsfOnlinePoll;
		//public SportSite.Controls.FlashNews IsfFlashNews;
		public SportSite.Controls.Sponsors IsfSponsors;
		protected System.Web.UI.HtmlControls.HtmlAnchor TopLogoLink;
		protected System.Web.UI.HtmlControls.HtmlImage TopLogoImage;
		protected System.Web.UI.HtmlControls.HtmlGenericControl PageCaption;
		protected System.Web.UI.HtmlControls.HtmlGenericControl PageCaptionWrapper;
		protected System.Web.UI.HtmlControls.HtmlGenericControl pnlLoggedInUser;
		protected System.Web.UI.WebControls.Panel PageContentsPanel;
		protected System.Web.UI.WebControls.Panel AdminDashboardPanel;
		protected System.Web.UI.WebControls.Panel SchoolDashboardPanel;
		protected System.Web.UI.WebControls.Panel ExtraContentsPanel;
		protected System.Web.UI.HtmlControls.HtmlAnchor MoreArticlesLink;
		protected System.Web.UI.HtmlControls.HtmlImage MoreArticlesImage;
		protected System.Web.UI.HtmlControls.HtmlImage imgLogoOne;

		private string _pageCaption = string.Empty;
		private int pageContentsOffset = 0;
		public int PageContentsOffset
		{
			get { return pageContentsOffset; }
			set { pageContentsOffset = value; }
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			CheckTopBanner();
			CheckAdvertisementBanners();

			if (SportSite.Common.Style.DatePanelCss.Length > 0)
				IsfHebDateTime.Attributes.Add("class", SportSite.Common.Style.DatePanelCss);

			TopLogoLink.HRef = Common.Data.AppPath + "/";
			TopLogoImage.Src = Common.Data.AppPath + "/Images/logo.gif";

			MoreArticlesLink.HRef = Common.Data.AppPath + "/Main.aspx?action=" + Common.SportSiteAction.ShowArticle + "&id=all";
			MoreArticlesImage.Src = Common.Data.AppPath + "/Images/more_articles.gif";

			ExtraContentsPanel.Style["display"] = "none";

			TopBanner.Type = BannerType.IsfGeneral;
			MiddleBanner.Type = BannerType.Advertisement_Main;
			//HotSaleBanner.Type = BannerType.IsfSpecialOffer;
			SmallBanner.Type = BannerType.Advertisement_Small;
			BottomBanner.Type = BannerType.Advertisement_Secondary;

			IsfMainArticle.Type = ArticleType.Main;
			IsfSubArticle.Type = ArticleType.Sub;
			IsfExtraArticle.Type = ArticleType.Sub;

			IsfSubArticle.Index = 0;

			int subArticlesCount = 0;
			object data;
			if (CacheStore.Instance.Get("ArticleCount_Sub", out data))
			{
				subArticlesCount = (int)data;
			}
			else
			{
				WebSiteServices.WebsiteService service = new WebSiteServices.WebsiteService();
				subArticlesCount = service.GetArticlesCount(false, true);
				CacheStore.Instance.Update("ArticleCount_Sub", subArticlesCount, 5);
			}

			if (subArticlesCount > 1)
			{
				IsfExtraArticle.Index = 1;
				IsfExtraArticle.Visible = true;
				clientSide.AddOnloadCommand("MoveContentsDown();", false, true);
			}

			string strBaseLink = "<a href=\"http://www.mir.co.il\" target=\"_blank\">%text</a>";
			string strHTML = "<div id=\"MirLogoPanel\">";
			strHTML += strBaseLink.Replace("%text", "<img src=\"" +
				Common.Tools.CheckAndCreateThumbnail(
				Common.Data.AppPath + "/Images/MIRlogo.jpg", 30, 25, this.Server) + "\" " +
				"id=\"MirLogoImage\" align=\"middle\" />");
			strHTML += "&nbsp;&nbsp;&nbsp;";
			strHTML += strBaseLink.Replace("%text", "Powered by MIR");
			strHTML += "</div>";
			lbPoweredBy.Text = strHTML;

			//JHWC_Link.ImageUrl = Common.Data.AppPath+"/Images/JHWC.PNG";
			//JHWC_Link.NavigateUrl = Common.Data.AppPath+"/JHWC.aspx";

			//SetPageCaption("");
			AddJavascript();
		}

		public void ShowAlternativeContent(AlternativeContentPanel panel)
		{
			switch (panel)
			{
				case AlternativeContentPanel.AdminDashboard:
					AdminDashboardPanel.Visible = true;
					break;
				case AlternativeContentPanel.SchoolDashboard:
					SchoolDashboardPanel.Visible = true;
					break;
			}
		}

		public void Page_PreRender(object sender, System.EventArgs e)
		{
			if (PageContentsOffset != 0)
			{
				PageContentsPanel.Style["left"] = PageContentsOffset + "px";
			}
		}

		private void CheckLoginPanel()
		{
			//user login panel
			if (Session[UserManager.SessionKey] != null && Session[UserManager.SessionKey] is UserData && _pageCaption.Length == 0)
			{
				UserData user = (UserData)Session[UserManager.SessionKey];
				if (!string.IsNullOrEmpty(user.Login))
				{
					bool blnExternalUser = (user.Type == (int)Sport.Types.UserType.External);
					string text = string.IsNullOrEmpty(user.Name) ? user.Login : user.Name;
					text += string.Format(" [<a href=\"{0}/Register.aspx{1}\">{2}</a>]", Data.AppPath,
						(blnExternalUser ? "?action=" + SportSiteAction.GeneralChampRegister : ""),
						(blnExternalUser ? "רישום קבוצה או שחקנים" : "אינדקס ניהול אתר"));
					text += string.Format(" [<a href=\"{0}/Register.aspx?action={1}\">{2}</a>]", Data.AppPath, SportSiteAction.Logout.ToString(),
						((CookieManager.Read(UserManager.RememberMeKey).Length > 0) ? "שכח אותי" : "החלפת משתמש"));
					pnlLoggedInUser.InnerHtml = text;
					pnlLoggedInUser.Visible = true;
				}
			}
		}

		private void UpdateApplicationIndex(string key, int seconds)
		{
			string strKeyLastChange = key + "LastChange";
			string strKeyIndex = key + "Index";
			string strKeyCount = key + "Count";
			if (Application[strKeyLastChange] == null)
				Application[strKeyLastChange] = DateTime.Now;
			DateTime dtLastChange = (DateTime)Application[strKeyLastChange];
			if ((DateTime.Now - dtLastChange).TotalSeconds >= seconds)
			{
				int curIndex = Common.Tools.CIntDef(Application[strKeyIndex], 0);
				int count = Common.Tools.CIntDef(Application[strKeyCount], 0);
				curIndex++;
				if (curIndex >= count)
					curIndex = 0;
				Application[strKeyIndex] = curIndex;
				Application[strKeyLastChange] = DateTime.Now;
			}
		}

		private void CheckAdvertisementBanners()
		{
			int mainCount = Common.Tools.CIntDef(Application["MainAdvertisementCount"], 0);
			if (mainCount <= 0)
				Application["MainAdvertisementCount"] = Banner.GetMainAdvertisementFlash(this.Server).Length;
			int subCount = Common.Tools.CIntDef(Application["SubAdvertisementCount"], 0);
			if (subCount <= 0)
				Application["SubAdvertisementCount"] = Banner.GetSubAdvertisementFlash(this.Server).Length;
			UpdateApplicationIndex("MainAdvertisement", 60);
			UpdateApplicationIndex("SubAdvertisement", 60);
		}

		private void CheckTopBanner()
		{
			if (Application["TopBannerLastChange"] == null)
				Application["TopBannerLastChange"] = DateTime.Now;
			DateTime dtLastChange = (DateTime)Application["TopBannerLastChange"];
			if ((DateTime.Now - dtLastChange).TotalSeconds >= 60)
			{
				Core.TopBannerManager.Data[] arrBanners = Core.TopBannerManager.GetAllBanners();
				if (arrBanners.Length > 0)
				{
					int curIndex = Common.Tools.CIntDef(Application["TopBannerIndex"], -1);
					curIndex++;
					if (curIndex >= arrBanners.Length)
						curIndex = 0;
					Core.TopBannerManager.Data bannerData = arrBanners[curIndex];
					string strBannerId = null;
					switch (bannerData.BannerType)
					{
						case (int)Core.TopBannerManager.TopBannerType.Basketball:
							strBannerId = "0";
							break;
						case (int)Core.TopBannerManager.TopBannerType.Volleyball:
							strBannerId = "1";
							break;
						case (int)Core.TopBannerManager.TopBannerType.BeachVolleyball:
							strBannerId = "2";
							break;
						case (int)Core.TopBannerManager.TopBannerType.ZooZoo:
							strBannerId = "3";
							break;
						case (int)Core.TopBannerManager.TopBannerType.Handball:
							strBannerId = "4";
							break;
					}

					if (strBannerId != null)
					{
						string strFileName = "sportschool.xml"; //"puredata.txt";
						string strFilePath = Server.MapPath(Common.Data.AppPath + "/" + strFileName);
						string strTargetDestination = Server.MapPath(Common.Data.AppPath + "/Flash/" + strFileName);
						try
						{
							XmlDocument document = new XmlDocument();
							document.Load(strFilePath);
							document.DocumentElement.Attributes["DATE"].Value = DateTime.Now.ToString("dd/MM/yyyy");
							document.DocumentElement.Attributes["TIME"].Value = DateTime.Now.ToString("HH:mm:ss");
							List<XmlNode> arrBannerNodes = new List<XmlNode>();
							foreach (XmlNode node in document.DocumentElement.GetElementsByTagName("Banner"))
								arrBannerNodes.Add(node);

							XmlNode bannerNode = arrBannerNodes.Find(node => node.Attributes["BID"].Value.Equals(strBannerId));
							if (bannerNode != null)
							{
								bannerNode.Attributes["Line1"].Value = bannerData.FirstLine;
								bannerNode.Attributes["Line2"].Value = bannerData.SecondLine;
								bannerNode.Attributes["Line3"].Value = bannerData.ThirdLine;
							}

							document.Save(strFilePath);
						}
						catch
						{ }

						try
						{
							File.Copy(strFilePath, strTargetDestination, true);
						}
						catch
						{ }
					}

					/*
					else
					{
						StringBuilder strFileData = new StringBuilder();
						strFileData.Append("&phrase1=" + bannerData.FirstLine + Common.Data.NEW_LINE);
						strFileData.Append("&phrase2=" + bannerData.SecondLine);
						string strThirdLine = bannerData.ThirdLine;
						if ((strThirdLine != null) && (strThirdLine.Length > 0))
							strFileData.Append("<br>" + strThirdLine);
						strFileData.Append(Common.Data.NEW_LINE + "");
						strFileData.Append("&bannerType=" + (bannerData.BannerType + 1).ToString());
						Common.Tools.CreateTextFile(strFilePath, strFileData.ToString(), false);
					}
					*/
					Application["TopBannerIndex"] = curIndex;
					Application["TopBannerType"] = bannerData.BannerType;
				}
				Application["TopBannerLastChange"] = DateTime.Now;
			}
		}

		private void AddJavascript()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append("<script type=\"text/javascript\">");
			sb.Append("   _rootPath = \"" + Common.Data.AppPath + "\";");
			sb.Append("</script>");
			Page.ClientScript.RegisterClientScriptBlock(Page.GetType(), "fill_gap", sb.ToString(), false);
		}

		public void AddExtraContents(string strHtml)
		{
			if ((strHtml != null) && (strHtml.Length > 0))
			{
				Literal literal = new Literal();
				literal.Text = strHtml;
				ExtraContentsPanel.Controls.Add(literal);
			}
		}

		public void AddContents(string strHtml)
		{
			if ((strHtml != null) && (strHtml.Length > 0))
			{
				Literal literal = new Literal();
				literal.Text = strHtml;
				PageContentsPanel.Controls.Add(literal);
			}
		}

		public void AddContents(System.Web.UI.Control control)
		{
			PageContentsPanel.Controls.Add(control);
		}

		public void AddSubCaption(string strText)
		{
			AddContents(Common.Tools.BuildPageSubCaption(strText, this.Server, this.clientSide));
		}

		public void ShowLogoOne()
		{
			imgLogoOne.Visible = true;
		}

		public void SetPageCaption(string caption)
		{
			if ((caption == null) || (caption.Length == 0))
			{
				PageCaption.InnerHtml = "";
				PageCaptionWrapper.Style["visibility"] = "hidden";
				return;
			}
			//string strImageName=Common.Tools.BuildPageCaptionImage(caption, this.Server);
			//PageCaption.InnerHtml = "<img src=\""+strImageName+"\" />";
			string strContainerID = "PageCaptionContainer";
			PageCaption.InnerHtml = "<div id=\"" + strContainerID + "\"></div>";
			clientSide.RegisterFlashMovie(strContainerID, Common.Data.AppPath + "/Flash/red_title_v" + Common.Data.FlashTitlesVersion + ".swf", 380, 30, "txt=" + caption, "#ffffff");
			PageCaptionWrapper.Style["visibility"] = "visible";

			_pageCaption = caption;
		}

		public void ClearContents()
		{
			PageContentsPanel.Controls.Clear();
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
			this.PreRender += new System.EventHandler(this.MainView_PreRender);
			this.Unload += new EventHandler(Page_Unload);
			this.Init += new EventHandler(Page_Init);
			this.PreRender += new System.EventHandler(this.Page_PreRender);
		}
		#endregion

		private void MainView_PreRender(object sender, EventArgs e)
		{
			if (PageContentsPanel.Controls.Count == 0)
			{
				PageContentsPanel.Style["display"] = "none";
			}
			else
			{
				string[] arrInvisiblePanels = new string[] {
					"MainArticlePanel", "SubArticlePanel", "MoreArticlesLinkPanel", 
					"MiddleBannerPanel", "SportFlowersPanel", "BottomBannerPanel"};

				System.Text.StringBuilder strHTML = new System.Text.StringBuilder();
				strHTML.Append("<style type=\"text/css\">");
				for (int i = 0; i < arrInvisiblePanels.Length; i++)
				{
					strHTML.Append(" #" + arrInvisiblePanels[i] + " {display: none;}\n");
				}
				strHTML.Append("</style>");
				Page.ClientScript.RegisterClientScriptBlock(Page.GetType(), "hide_panels", strHTML.ToString(), false);
				IsfMainArticle.Visible = false;
				IsfSubArticle.Visible = false;
				MiddleBanner.Visible = false;
				//IsfOnlinePoll.Visible = false;
				//IsfFlashNews.Visible = false;
				//HotSaleBanner.Visible = false;
				BottomBanner.Visible = false;

				strHTML = new System.Text.StringBuilder();
				strHTML.Append("<div class=\"BottomInnerPanel\">&nbsp;</div>");
				AddContents(strHTML.ToString());

				if (this.Attributes["dir"] == "ltr")
				{
					PageContentsPanel.Style["text-align"] = "left";
					PageContentsPanel.Attributes["align"] = "left";
					PageContentsPanel.Attributes["dir"] = "ltr";
				}
			}

			CheckLoginPanel();
		}

		private void Page_Unload(object sender, EventArgs e)
		{

		}

		private void Page_Init(object sender, EventArgs e)
		{
			this.clientSide = new Common.ClientSide(this.Page, lbOnloadJS);

			if (CookieManager.Read(UserManager.RememberMeKey).Length > 0 && Session[UserManager.SessionKey] == null && CookieManager.Read("rmp").Length > 0)
			{
				UserManager manager = new UserManager(this.Session);
				if (!manager.VerifyUser(CookieManager.Read(UserManager.RememberMeKey), Sport.Common.Crypto.Decode(CookieManager.Read("rmp"))))
					UserManager.RemoveRememberMeCookiesFromBrowser();
			}
		}
	}
}