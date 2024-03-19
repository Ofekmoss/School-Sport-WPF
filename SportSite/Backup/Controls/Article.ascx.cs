namespace SportSite.Controls
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Configuration;
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using SportSite.Common;

	public enum ArticleType
	{
		Unknown = -1,
		Main = 0,
		Sub,
		Ordinary
	}

	/// <summary>
	///		Summary description for Article.
	/// </summary>
	public class Article : System.Web.UI.UserControl
	{
		protected System.Web.UI.WebControls.Label ArticleContents;
		protected System.Web.UI.HtmlControls.HtmlAnchor ArticleCaptionLink;
		protected System.Web.UI.HtmlControls.HtmlTable ArticleMainTable;
		protected System.Web.UI.WebControls.Literal ArticleFlashPanel;
		private ArticleType _type = ArticleType.Unknown;
		private int _articleID = -1;
		private int _index = 0;
		private bool _externalArticle = false;
		private Controls.MainView IsfMainView = null;

		public ClientSide DirectClientSide { get; set; }
		public string OverrideLink { get; set; }

		private static int FlashCount = 0;

		public ArticleType Type
		{
			get { return _type; }
			set { _type = value; }
		}

		public int Index
		{
			get { return _index; }
			set { _index = value; }
		}

		public int ArticleID
		{
			get { return _articleID; }
			set { _articleID = value; }
		}

		public bool ExternalArticle
		{
			get { return _externalArticle; }
			set { _externalArticle = value; }
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			IsfMainView = (SportSite.Controls.MainView)
				Common.Tools.FindControlByType(this, "MainView");
		} //end function Page_Load

		private string BuildFlashHTML(string strImageURL, int articleID)
		{
			string strMovie = Common.Data.AppPath + "/Flash/";
			switch (_type)
			{
				case ArticleType.Main:
				case ArticleType.Ordinary:
					strMovie += "small_pic_blue_new.swf";
					break;
				case ArticleType.Sub:
					if (this.Index > 0)
						strMovie += "small_pic_blue_new.swf";
					else
						strMovie += "small_pic_red_new.swf";
					break;
			}
			if (Article.FlashCount > 1000)
				Article.FlashCount = 0;
			Article.FlashCount++;
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			string strContainerID = "ArticleImageFlashSmall_" + Article.FlashCount;
			sb.Append("<div id=\"" + strContainerID + "\" class=\"ArticlePicFlashSmall\"></div>");
			string strColor = (_externalArticle) ? "#C9E0F0" : "#ffffff";
			ClientSide clientSide = (this.DirectClientSide == null) ? IsfMainView.clientSide : this.DirectClientSide;
			string targetUrl = string.IsNullOrEmpty(this.OverrideLink) ? Common.Data.AppPath + "/Article.aspx?id=" + articleID : this.OverrideLink;
			string flashVars = "picUrl=" + strImageURL + "&picURL=" + targetUrl;
			clientSide.RegisterFlashMovie(strContainerID, strMovie,
				Common.Data.ArticleSmallImage.Width, Common.Data.ArticleSmallImage.Height, flashVars, strColor);
			return sb.ToString();
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
			this.PreRender += new EventHandler(Page_PreRender);
		}
		#endregion

		private void Page_PreRender(object sender, EventArgs e)
		{
			string strArticleCaption = "";
			string strArticleContents = "";
			string strArticleImage = Data.AppPath + "/Images/default_article_image_new.JPG";
			string strURL = "";
			int articleID = -1;

			//valid?
			if (Type != ArticleType.Unknown)
			{
				//load proper article:
				string cacheKey = "ArticleLimitedData_";
				switch (Type)
				{
					case ArticleType.Main:
						cacheKey += "Primary";
						break;
					case ArticleType.Sub:
						if (this.Index >= 0)
							cacheKey += "Sub_" + this.Index;
						break;
					case ArticleType.Ordinary:
						if (_articleID >= 0)
							cacheKey += _articleID.ToString();
						break;
				}

				LimitedArticleData limitedArticle = null;
				if (!cacheKey.EndsWith("_"))
				{
					object data;
					if (CacheStore.Instance.Get(cacheKey, out data))
					{
						limitedArticle = (LimitedArticleData)data;
					}
					else
					{
						WebSiteServices.ArticleData article = SportSite.Core.ArticleManager.GetActualArticle(this.Type, _articleID, this.Index);
						if (article != null && article.ID >= 0)
						{
							limitedArticle = new LimitedArticleData(article);
							CacheStore.Instance.Update(cacheKey, limitedArticle, 5);
						}
					}
				}

				if (limitedArticle != null)
				{
					//show article data:
					articleID = limitedArticle.ArticleId;
					strURL = Data.AppPath + "/Main.aspx?action=" +
						SportSiteAction.ShowArticle + "&id=" + articleID;
					strArticleCaption = limitedArticle.Caption;
					strArticleContents = limitedArticle.SubCaption;
					ArticleCaptionLink.HRef = strURL;
					if (limitedArticle.FirstImage.Length > 0)
						strArticleImage = limitedArticle.FirstImage;
				}
			}

			strArticleImage = Tools.CheckAndCreateThumbnail(strArticleImage, Common.Data.ArticleSmallImage.Width, Common.Data.ArticleSmallImage.Height, this.Server);

			if (_externalArticle)
			{
				ArticleMainTable.Style["background-color"] = "#C9E0F0";
				ArticleCaptionLink.Attributes["class"] = "ArticleCaptionLinkBlue";
				ArticleContents.CssClass = "ArticleContentsBlue";
			}

			ArticleCaptionLink.InnerHtml = strArticleCaption;
			ArticleContents.Text = strArticleContents;
			ArticleFlashPanel.Text = BuildFlashHTML(strArticleImage, articleID);
		}
	}
}