using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SportSite.Common;
using SportSite.Controls;
using SportSite.Core;

namespace SportSite.NewRegister
{
	public partial class HomepageArticlesPreview : System.Web.UI.Page
	{
		public ClientSide clientSide;
		protected string PreviewSessionKey = "Homepage_Articles_Preview";

		protected int[] PreviewedArticles
		{
			get
			{
				List<int> articleIdNumbers = new List<int>();
				if (Session[PreviewSessionKey] != null)
				{
					articleIdNumbers.AddRange((int[])Session[PreviewSessionKey]);
				}
				return articleIdNumbers.ToArray();
			}
			set
			{
				Session[PreviewSessionKey] = value;
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			if (HandleAJAX())
			{
				Response.Clear();
				Response.Write("OK");
				Response.End();
				return;
			}

			clientSide = new Common.ClientSide(this.Page, lbOnloadJS);
			AssignArticles(new Controls.Article[] { IsfMainArticle, IsfSubArticle, IsfExtraArticle });
			
			Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "has_changes", "var _hasChanges=" + 
				HasPreviewChanges().ToString().ToLower() + "; ", true);
		}

		protected bool HasPreviewChanges()
		{
			int[] previewedArticles = this.PreviewedArticles;
			return (previewedArticles != null && previewedArticles.Length > 0);
		}

		protected void AssignArticles(Controls.Article[] articleControls)
		{
			int[] previewedArticles = this.PreviewedArticles;
			for (int i = 0; i < articleControls.Length; i++)
			{
				var articleControl = articleControls[i];
				articleControl.DirectClientSide = clientSide;
				AssignActualAricle(ref articleControl, i, previewedArticles);
			}
		}

		protected void AssignActualAricle(ref Controls.Article articleControl, int index, int[] previewedArticles)
		{
			int previewedArticleId = previewedArticles.GetItemOrDefault<int>(index, -1);
			if (previewedArticleId > 0)
			{
				articleControl.Type = ArticleType.Ordinary;
				articleControl.ArticleID = previewedArticleId;
			}
			else
			{
				articleControl.Type = (index == 0) ? ArticleType.Main : ArticleType.Sub;
				if (index > 0)
					articleControl.Index = index - 1;
			}
		}

		protected bool HandleAJAX()
		{
			string action = Request.Form["action"] + "";
			switch (action)
			{
				case "set":
					return SetArticle();
				case "reset":
					return ResetChanges();
				case "apply":
					return ApplyAllChanges();
			}

			return false;
		}

		protected bool SetArticle()
		{
			int articleId, articleType;
			if (Int32.TryParse(Request.Form["id"], out articleId) && Int32.TryParse(Request.Form["type"], out articleType))
			{
				if (articleId > 0 && articleType >= 1 && articleType <= 3)
				{
					List<int> previewedArticles = this.PreviewedArticles.ToList();
					while (previewedArticles.Count < 3)
						previewedArticles.Add(0);
					previewedArticles[articleType - 1] = articleId;
					this.PreviewedArticles = previewedArticles.ToArray();
					return true;
				}
			}
			return false;
		}

		protected bool ResetChanges()
		{
			this.PreviewedArticles = null;
			return true;
		}

		protected bool ApplyAllChanges()
		{
			if (Session[UserManager.SessionKey] == null)
				throw new Exception("Forbidden");

			UserData loggedInUser = (UserData)Session[UserManager.SessionKey];
			if (loggedInUser.Id < 0 || loggedInUser.Type != (int)Sport.Types.UserType.Internal)
				throw new Exception("Not Authorized");

			int[] previewedArticles = this.PreviewedArticles;
			if (previewedArticles.Length == 0)
				throw new Exception("Nothing to apply");

			int primaryArticleId = previewedArticles.GetItemOrDefault(0, -1);
			int secondaryArticleId = previewedArticles.GetItemOrDefault(1, -1);
			int extraArticleId = previewedArticles.GetItemOrDefault(2, -1);
			if (secondaryArticleId < 0 && extraArticleId < 0)
			{
				SportSite.Core.ArticleManager.SetPrimaryAndSecondaryArticles(loggedInUser, primaryArticleId);
			}
			else
			{
				if (secondaryArticleId <= 0)
					secondaryArticleId = ArticleManager.GetActualArticle(ArticleType.Sub, -1, 0).ID;
				if (extraArticleId <= 0)
					extraArticleId = ArticleManager.GetActualArticle(ArticleType.Sub, -1, 1).ID;
				SportSite.Core.ArticleManager.SetPrimaryAndSecondaryArticles(loggedInUser, primaryArticleId, secondaryArticleId, extraArticleId);
			}
			CacheStore.Instance.RemoveAll();
			return true;
		}
	}
}