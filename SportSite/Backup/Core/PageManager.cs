using System;
using System.Collections;
using SportSite.Common;

namespace SportSite.Core
{
	/// <summary>
	/// provides static methods to manage web site pages.
	/// </summary>
	public class PageManager
	{
		/// <summary>
		/// returns list of all available pages having dynamic title and contents.
		/// </summary>
		public static WebSiteServices.PageData[] GetDynamicPages(Core.UserData user)
		{
			ArrayList pages=new ArrayList();

			//iterate over all the dynamic links and add them one by one...
			for (int i=0; i<Data.DynamicLinks.Length; i++)
				pages.AddRange(GetDynamicPages(Data.DynamicLinks[i], user));
			
			return (WebSiteServices.PageData[])
				pages.ToArray(typeof(WebSiteServices.PageData));
		}
		
		private static WebSiteServices.PageData[] GetDynamicPages
			(Data.DynamicLinkData linkData, Core.UserData user)
		{
			ArrayList result=new ArrayList();

			WebSiteServices.WebsiteService websiteService=
				new WebSiteServices.WebsiteService();
			websiteService.CookieContainer = Sport.Core.Session.Cookies;
			
			//get current link:
			if ((linkData.Children == null)||(linkData.Children.Length == 0))
			{
				//get all pages having this index and caption:
				WebSiteServices.PageData pageData=
					websiteService.FindPageData(linkData.Text, (int) linkData.Link);
				
				//check if we need to add to database:
				if (pageData.ID < 0)
				{
					//add the data to database and read again:
					pageData.Caption = linkData.Text;
					pageData.Index = (int) linkData.Link;
					websiteService.UpdatePage(pageData, user.Login, user.Password, user.Id);
					pageData = websiteService.FindPageData(
						linkData.Text, (int) linkData.Link);
				}
				
				//add to result:
				result.Add(pageData);
			} //end if no children

			//add the child links as well...
			if (linkData.Children != null)
			{
				for (int i=0; i<linkData.Children.Length; i++)
				{
					result.AddRange(GetDynamicPages(linkData.Children[i], user));
				}
			}
			
			//done.
			return (WebSiteServices.PageData[])
				result.ToArray(typeof(WebSiteServices.PageData));
		}

		/// <summary>
		/// return the proper page data having the same Index and Text/Caption as
		/// the given page.
		/// </summary>
		public static WebSiteServices.PageData FindPage(
			WebSiteServices.PageData[] pages, Data.DynamicLinkData linkData)
		{
			//build the default return value of "empty" page:
			WebSiteServices.PageData result=new WebSiteServices.PageData();
			result.ID = -1;

			//search for the page having the same index and caption:
			foreach (WebSiteServices.PageData page in pages)
			{
				if ((page.Caption == linkData.Text)&&
					(page.Index == (int) linkData.Link))
				{
					result = page;
					break;
				}
			}

			//done.
			return result;
		} //end function FindPage

		public static Data.DynamicLinkData[] GetPageAncestors(
			WebSiteServices.PageData pageData)
		{
			ArrayList arrLinks=new ArrayList();
			//look for ancestor:
			foreach (Data.DynamicLinkData linkData in Data.DynamicLinks)
			{
				if (linkData.Contains(pageData))
				{
					arrLinks.AddRange(GetPageAncestors(pageData, linkData));
					arrLinks.Reverse();
					break;
				}
			}
			return (Data.DynamicLinkData[])
				arrLinks.ToArray(typeof(Data.DynamicLinkData));
		}

		private static Data.DynamicLinkData[] GetPageAncestors(
			WebSiteServices.PageData pageData, Data.DynamicLinkData linkData)
		{
			ArrayList arrLinks=new ArrayList();
			
			arrLinks.Add(linkData);
			if (linkData.Children != null)
			{
				for (int i=0; i<linkData.Children.Length; i++)
					if (linkData.Children[i].Contains(pageData))
						arrLinks.AddRange(GetPageAncestors(pageData, linkData.Children[i]));
			}
			
			return (Data.DynamicLinkData[])
				arrLinks.ToArray(typeof(Data.DynamicLinkData));
		}
	}
}
