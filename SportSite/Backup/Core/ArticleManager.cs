using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SportSite.Controls;
using SportSite.Common;

namespace SportSite.Core
{
	public static class ArticleManager
	{
		public static WebSiteServices.ArticleData GetActualArticle(ArticleType articleType, int articleId, int articleIndex)
		{
			WebSiteServices.ArticleData article = null;
			using (WebSiteServices.WebsiteService service = new WebSiteServices.WebsiteService())
			{
				switch (articleType)
				{
					case ArticleType.Main:
						article = service.GetPrimaryArticle();
						break;
					case ArticleType.Sub:
						WebSiteServices.ArticleData[] subArticles = service.GetSubArticles();
						if (subArticles != null)
						{
							for (int i = articleIndex - 1; i >= 0; i--)
							{
								if (i < subArticles.Length && !subArticles[i].IsValid())
									articleIndex++;
							}
							while (true)
							{
								if (articleIndex >= subArticles.Length)
									break;
								if (articleIndex >= 0 && subArticles[articleIndex].IsValid())
								{
									article = subArticles[articleIndex];
									break;
								}
								articleIndex++;
							}
						}
						break;
					case ArticleType.Ordinary:
						article = service.GetArticleData(articleId);
						break;
				}

				if (articleType != ArticleType.Ordinary)
				{
					if (!article.IsValid())
					{
						WebSiteServices.ArticleData[] recentArticles = service.GetArticles(20);
						if (recentArticles != null)
						{
							int i = 0;
							while (i < recentArticles.Length)
							{
								WebSiteServices.ArticleData currentArticle = recentArticles[i];
								if (currentArticle.IsValid())
								{
									article = currentArticle;
									break;
								}
								i++;
							}
						}
					}
				}
			}
			
			return article;
		}

		public static void SetPrimaryAndSecondaryArticles(Core.UserData loggedInUser, int primaryArticle, params int[] subArticles)
		{
			using (WebSiteServices.WebsiteService service = new WebSiteServices.WebsiteService())
			{
				service.SetPrimaryAndSecondaryArticles(primaryArticle, subArticles, loggedInUser.Login, loggedInUser.Password, loggedInUser.Id);
			}
		}
	}
}