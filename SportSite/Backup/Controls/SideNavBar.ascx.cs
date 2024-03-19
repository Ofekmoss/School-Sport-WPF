using System;
using System.Linq;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Collections;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Sport.Common;
using SportSite.Common;
using ISF.DataLayer;
namespace SportSite.Controls
{
	/// <summary>
	///		Summary description for SideNavBar.
	/// </summary>
	public class SideNavBar : LinkBox
	{
		private bool _checkActiveLink = false;
		public bool CheckActiveLinkHighlight
		{
			get { return _checkActiveLink; }
			set { _checkActiveLink = value; }
		}

		public SideNavBar()
		{
			//this.Links += new Link("hello.html", "hello");

			//style
			this.BkColor = SportSite.Common.Style.NavBarBgColor;
			this.IsHebrew = true;
			//this.Width = 120;
			LinkBox.LinkStyle style1 = new LinkBox.LinkStyle("");
			style1.AddLight = Common.Style.NavBarAddLight; //-80;
			style1.CssClass = SportSite.Common.Style.NavBarLinkCss;
			style1.hAlign = HorizontalAlign.Right;
			style1.Height = 18;
			style1.fontSize = 12;
			this.DefaultLinkStyle = style1;

			BuildLinks();
		}

		private void BuildLinks()
		{
			/* Links */
			string strPageUrl = Data.AppPath + "/Main.aspx";

			//get regions list:
			object data;
			Sport.Common.SimpleData[] regions = null;
			if (CacheStore.Instance.Get("RegionData_All", out data))
			{
				regions = (Sport.Common.SimpleData[])data;
			}
			else
			{
				//regions = DB.Instance.GetRegionsData();
				DataServices1.DataService service = new DataServices1.DataService();
				regions = service.GetRegionsData().ToList().ConvertAll(dsr => new Sport.Common.SimpleData
				{
					ID = dsr.ID, 
					Name = dsr.Name
				}).ToArray();
				CacheStore.Instance.Update("RegionData_All", regions, 5);
			}


			//main page:
			this.Links += new LinkBox.Link(strPageUrl, "עמוד ראשי", LinkBox.LinkIndicator.Blue);

			//hot link?
			string cacheKey = "ArticleLimitedData_Hot";
			LimitedArticleData hotArticle = null;
			if (CacheStore.Instance.Get(cacheKey, out data))
			{
				hotArticle = (LimitedArticleData)data;
			}
			else
			{
				WebSiteServices.WebsiteService websiteService = new WebSiteServices.WebsiteService();
				WebSiteServices.ArticleData article = websiteService.GetHotArticle();
				if (article != null && article.ID >= 0)
				{
					hotArticle = new LimitedArticleData(article);
					CacheStore.Instance.Update(cacheKey, hotArticle, 5);
				}
			}

			if (hotArticle != null)
			{
				//get link URL
				string strURL = strPageUrl + "?action=" + SportSiteAction.ShowArticle.ToString();
				strURL += "&id=" + hotArticle.ArticleId;

				//get link text:
				string strText = hotArticle.Caption;

				//add hot link...
				LinkBox.Link hotLink = new LinkBox.Link(strURL, strText, LinkBox.LinkIndicator.StrongRed);
				hotLink.IsHotLink = true;
				this.Links += hotLink;
			}

			//sport flowers:
			this.Links += new LinkBox.Link("https://www.schoolsport.co.il/", "פרחי ספורט", LinkBox.LinkIndicator.Blue);

			//admin tools:
			this.Links += new LinkBox.Link(Data.AppPath + "/Register.aspx", "רישום קבוצות או שחקנים", LinkBox.LinkIndicator.Blue);

			//info page:
			this.Links += BuildDynamicLink(strPageUrl, NavBarLink.InfoPage);

			//Registration:
			this.Links += BuildRegistrationLink(strPageUrl);

			//Young Sportsman Development Team:
			this.Links += BuildYsdtLink(strPageUrl);

			//permanent championships:
			int permanentChampCount = Core.LinkManager.GetPermanentChampionships().Length;
			for (int i = 0; i < permanentChampCount; i++)
			{
				this.Links += new LinkBox.Link("permanentchamp" + i, "", LinkBox.LinkIndicator.Red);
			}

			//national champs:
			this.Links += BuildChampLink(strPageUrl, SportSiteAction.NationalChampionships, regions);

			//club championships:
			this.Links += BuildChampLink(strPageUrl, SportSiteAction.ClubChampionships, regions);

			//other championships:
			this.Links += BuildChampLink(strPageUrl, SportSiteAction.OtherChampionships, regions);

			//calendar:
			this.Links += new LinkBox.Link(strPageUrl + "?action=" + SportSiteAction.ViewCalendar, "לוח אירועים חודשי", LinkBox.LinkIndicator.Yellow);

			//write to us:
			this.Links += new LinkBox.Link(strPageUrl + "?action=" + SportSiteAction.WriteToUs, "כתבו לנו", LinkBox.LinkIndicator.Yellow);

			//publications:
			this.Links += BuildDynamicLink(strPageUrl, NavBarLink.Publications); //BuildPublicationsLink(strPageUrl);

			//picture gallery:
			this.Links += new LinkBox.Link(strPageUrl + "?action=" + SportSiteAction.ViewPictureGallery, "גלריית תמונות", LinkBox.LinkIndicator.Yellow);

			//commities:
			this.Links += BuildDynamicLink(strPageUrl, NavBarLink.Commities);

			//takanon:
			this.Links += BuildDynamicLink(strPageUrl, NavBarLink.Takanon);

			//ISF home page:
			this.Links += new LinkBox.Link(strPageUrl + "?action=" + SportSiteAction.GoIsfHome, "ISF - דף הבית", LinkBox.LinkIndicator.StrongRed);

			//relevant links:
			this.Links += new LinkBox.Link(strPageUrl + "?action=" + SportSiteAction.RelevantLinks, "קישורים רלוונטיים", LinkBox.LinkIndicator.StrongRed);

			//sports board:
			this.Links += BuildDynamicLink(strPageUrl, NavBarLink.SportBoard);

			//archive:
			this.Links += BuildDynamicLink(strPageUrl, NavBarLink.Archieve);

			//sports basket:
			this.Links += BuildDynamicLink(strPageUrl, NavBarLink.SportBasket);
		}

		private LinkBox.Link BuildDynamicLink(string pageURL, NavBarLink link)
		{
			string strURL = pageURL + "?action=" + SportSiteAction.ViewDynamicPage;
			strURL += "&page=" + link.ToString();
			string strText = Data.DynamicLinks[Data.DynamicIndex(link)].Text;
			LinkBox.LinkIndicator indicator = LinkBox.LinkIndicator.StrongRed;
			switch (link)
			{
				case NavBarLink.InfoPage:
					indicator = LinkBox.LinkIndicator.Blue;
					break;
				case NavBarLink.Publications:
					indicator = LinkBox.LinkIndicator.Yellow;
					break;
			}
			LinkBox.Link result = new LinkBox.Link(strURL, strText, indicator);
			return result;
		}

		private LinkBox.Link BuildChampLink(string pageURL, SportSiteAction action,
			Sport.Common.SimpleData[] regions)
		{
			string caption = "N/A";
			switch (action)
			{
				case SportSiteAction.ClubChampionships:
					caption = "מועדוני ספורט";
					break;
				case SportSiteAction.OtherChampionships:
					caption = "אירועי ספורט";
					break;
				case SportSiteAction.NationalChampionships:
					caption = "אליפויות ארציות";
					break;
			}
			string linkURL = pageURL + "?action=" + action;
			string[] regionsToIgnore = new string[] { "חבל עזה", "יהודה ושומרון" };
			LinkBox.Link result = new LinkBox.Link(pageURL, caption, LinkBox.LinkIndicator.Red);
			//result.Style = LinkStyle.CopyLinkStyle(this.DefaultLinkStyle);
			LinkBox.LinkStyle style1 = new LinkBox.LinkStyle("");
			//style1.AddLight = Common.Style.NavBarAddLight; //-80;
			style1.CssClass = SportSite.Common.Style.NavBarLinkCss;
			//style1.hAlign = HorizontalAlign.Right;
			//style1.Height = 18;
			//style1.fontSize = 12;
			result.Style = style1;
			for (int i = 0; i < regions.Length; i++)
			{
				Sport.Entities.Region region = null;
				try
				{
					region = new Sport.Entities.Region(regions[i].ID);
				}
				catch { }

				if (region == null)
					continue;

				string strName = regions[i].Name;
				if (region.Id == Sport.Entities.Region.CentralRegion)
					strName = "ארצי";

				if (Sport.Common.Tools.InArray(regionsToIgnore, strName) >= 0)
					continue;

				if (action == SportSiteAction.NationalChampionships)
				{
					if (!region.IsNationalRegion())
						continue;
				}
				else
				{
					if (region.IsNationalRegion())
						continue;
				}

				result.Links += new LinkBox.Link(linkURL + "&r=" + regions[i].ID,
					strName, style1, LinkBox.LinkIndicator.Red);
			}
			if (result.Links.Count == 1)
			{
				result.Url = result.Links[0].Url;
				result.Links.Clear();
			}
			return result;
		}

		private LinkBox.Link BuildYsdtLink(string pageURL, Data.DynamicLinkData[] links, int childIndex,
			int[] levelIndices)
		{
			string linkURL = pageURL + "?action=" + SportSiteAction.ViewDynamicPage;
			linkURL += "&page=" + NavBarLink.YSDT;
			if (levelIndices.Length > 0)
				for (int i = 0; i < levelIndices.Length; i++)
					linkURL += "&w" + (i + 1) + "=" + (levelIndices[i] + 1);
			Data.DynamicLinkData parent = links[childIndex];
			Data.DynamicLinkData[] arrChildren = parent.Children;
			string caption = parent.Text;
			LinkBox.Link result = new LinkBox.Link(linkURL, caption, LinkBox.LinkIndicator.Yellow);
			LinkBox.LinkStyle style1 = new LinkBox.LinkStyle("");
			style1.AddLight = Common.Style.NavBarAddLight; //-80;
			style1.CssClass = SportSite.Common.Style.NavBarLinkCss;
			style1.hAlign = HorizontalAlign.Right;
			style1.Height = 18;
			style1.fontSize = 12;
			result.Style = style1;
			if (arrChildren != null)
			{
				int[] arrIndices = new int[levelIndices.Length + 1];
				for (int i = 0; i < levelIndices.Length; i++)
					arrIndices[i] = levelIndices[i];
				for (int i = 0; i < arrChildren.Length; i++)
				{
					arrIndices[levelIndices.Length] = i;
					result.Links += BuildYsdtLink(pageURL, arrChildren, i, arrIndices);
				}
			}
			return result;
		}

		private LinkBox.Link BuildYsdtLink(string pageURL)
		{
			return BuildYsdtLink(pageURL, Data.DynamicLinks, Data.DynamicIndex(NavBarLink.YSDT),
				new int[] { });
		}

		private LinkBox.Link BuildRegistrationLink(string pageURL)
		{
			LinkBox.Link result = new LinkBox.Link("", "הרשמה", LinkBox.LinkIndicator.Blue);
			result.Links += new LinkBox.Link(pageURL + "?action=" + SportSiteAction.PracticeCampRegister,
				"הרשמה למחנה אימון", LinkIndicator.Blue);
			result.Links += new LinkBox.Link(pageURL + "?action=" + SportSiteAction.TeacherCourseRegister,
				"הרשמה להשתלמויות", LinkIndicator.Blue);
			//result.Links += new LinkBox.Link(pageURL + "?action=" + SportSiteAction.TeacherCourseRegister + "&coach=1",
			//	"הרשמה להשתלמות מאמנים", LinkIndicator.Blue);
			return result;
		}

		/*
		private LinkBox.Link BuildPublicationsLink(string pageURL)
		{
			string caption=Data.DynamicLinks[Data.DynamicIndex(NavBarLink.Publications)].Text;
			string linkURL=pageURL+"?action="+SportSiteAction.ViewDynamicPage;
			linkURL += "&page="+NavBarLink.Publications;
			LinkBox.Link result=new LinkBox.Link(pageURL, caption, LinkBox.LinkIndicator.Yellow);
			return result;
		}
		*/

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			//base.OnInit(e);
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

		protected override void Page_Load(object sender, EventArgs e)
		{
			//permanent championships:
			Core.LinkManager.LinkData[] arrLinks =
				Core.LinkManager.GetPermanentChampionships();
			for (int i = 0; i < arrLinks.Length; i++)
			{
				string linkID = "permanentchamp" + i;
				string strText = Common.Tools.CStrDef(arrLinks[i].Text, "");
				LinkBox.Link link = this.Links[linkID];
				if (link != null)
				{
					link.Text = strText;
					if (strText.Length == 0)
						link.Visible = false;
					link.Url = Common.Tools.CStrDef(arrLinks[i].URL, "");
					if ((link.Visible) && (link.Url.Length > 0))
					{
						string strBaseURL = link.Url + "&section=%s";
						for (int j = 0; j < Data.ChampMemuItemsSections.Length; j++)
						{
							link.Links += new Link(
								strBaseURL.Replace("%s", Data.ChampMemuItemsSections[j]),
								Data.MatchChampMemuItemsText[j], link.Style);
						}
					}
					//this.Links[linkID] = link;
				}
			}

			if (_checkActiveLink)
			{
				foreach (LinkBox.Link link in this.Links)
				{
					if (CheckActiveLink(link))
						break;
				}
			}

			base.Page_Load(sender, e);
		}

		private bool CheckActiveLink(LinkBox.Link link)
		{
			string strURL = link.Url;
			bool result = false;
			string sportID = Common.Tools.CStrDef(Request.QueryString["sport"], "-999");
			string champID = Common.Tools.CStrDef(Request.QueryString["championship"], "-999");
			string categoryID = Common.Tools.CStrDef(Request.QueryString["category"], "-999");
			string strSection = Common.Tools.CStrDef(Request.QueryString["section"], "-999");
			if ((strURL.IndexOf("sport=" + sportID) >= 0) ||
				(strURL.IndexOf("championship=" + champID) >= 0) ||
				(strURL.IndexOf("category=" + categoryID) >= 0))
			{
				if (strURL.IndexOf("category=" + categoryID) >= 0)
				{
					if (strURL.IndexOf("section=" + strSection) >= 0)
						link.IsActive = true;
				}
				else
				{
					link.IsActive = true;
				}
				result = true;
			}
			if (link.Links != null)
			{
				foreach (LinkBox.Link childLink in link.Links)
				{
					if (CheckActiveLink(childLink))
						result = true;
				}
			}
			return result;
		}
	}
}
