using System;
using System.Linq;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using SportSite.Controls;
using SportSite.Common;
using SportSite.Core;
using System.Collections.Generic;
using System.Configuration;
using Sport.Types;
using ISF.DataLayer;

namespace SportSite
{
	//school
	//640086
	//640086

	//444455
	//444455

	//limnaa

	//208580670
	//208513705

	//441279
	//441279

	//51596252
	//60027554

	//http://www.schoolsport.co.il/Download.aspx?v=D066C738-110F-43D9-8C5A-22B0F7EA669C

	/// <summary>
	/// Main.aspx - main page of Israel Sport Association website.
	/// </summary>
	public class Main : System.Web.UI.Page
	{
		protected SportSite.Controls.MainView IsfMainView;
		protected System.Web.UI.WebControls.Literal lbStyle;
		protected SportSite.Controls.HebDateTime IsfHebDateTime;
		protected System.Web.UI.WebControls.Table IsfMainBody;
		protected SportSite.Controls.SideNavBar SideNavBar;
		protected HtmlMeta mtTitle;
		protected HtmlMeta mtDescription;
		protected HtmlLink mtPageThumb;

		#region Initialization
		private void Page_Load(object sender, System.EventArgs e)
		{
			//ZooZoo?
			if (Request.ServerVariables["SERVER_NAME"].IndexOf("zoozoo.org.il", StringComparison.CurrentCultureIgnoreCase) >= 0)
			{
				Server.Transfer("/ZooZoo/default.aspx", true);
				return;
			}

			if (Request.QueryString["ViewCacheStore"] != null)
			{
				Response.Write(CacheStore.Instance.ToString());
				Response.End();
				return;
			}

			//add hitlog data:
			Common.Tools.AddHitLog(WebSiteServices.WebSitePage.Main, this.Request);

			//season
			if (Sport.Core.Session.Season < 50)
			{
				Sport.Core.Session.Season = Sport.Entities.Season.GetOpenSeason();
			}

			//need to redirect?
			/*
			if ((Request.QueryString.Count == 0)&&(Request.Form.Count == 0))
			{
				if (Request.ServerVariables["SERVER_NAME"].ToLower().EndsWith(".org.il"))
				{
					Response.Write("<script type=\"text/javascript\">");
					Response.Write(" window.onload = WindowLoad;");
					Response.Write(" function WindowLoad(event) {");
					Response.Write("   var objImage = document.getElementById(\"JHWC\");");
					Response.Write("   objImage.height = (document.body.clientHeight-30);");
					Response.Write("");
					Response.Write(" }");
					Response.Write("</script>");
					Response.Write("<center>");
					Response.Write("<a href=\"JHWC.aspx\"><img id=\"JHWC\" src=\""+
						"Images/World_Basketball_Championships.JPG\" border=\"0\" "+
						"style=\"border: 1px solid black;\"/></a>");
					Response.Write("</center>");
					Response.End();
				}
			}
			*/

			//hide caption:
			IsfMainView.SetPageCaption("");

			//get action:
			SportSiteAction action = Tools.StrToAction(Request.QueryString["action"]);

			//form?
			if ((action == SportSiteAction.Undefined) && (Request.Form["action"] != null))
				action = Tools.StrToAction(Request.Form["action"]);

			//add the style:
			AddStyle();

			//set active link:
			SideNavBar.Links.MakeLinkActive(NavBarLink.MainPage);

			//decide what we have to do...
			DecideAction(action);

			//add javascript:
			Page.ClientScript.RegisterClientScriptInclude("ToolTip_V1", Data.AppPath + "/Common/ToolTip_V1.js");
		}

		public void Page_PreRender(object sender, System.EventArgs e)
		{

		}

		private void DecideAction(SportSiteAction action)
		{
			if (Session["ActionToPerform"] != null)
			{
				switch ((SportSiteAction)Session["ActionToPerform"])
				{
					case SportSiteAction.ShowPollResults:
						ShowPollResults();
						break;
				}
				Session["ActionToPerform"] = null;
				return;
			}

			switch (action)
			{
				//case SportSiteAction.CheckNewData:
				//	CheckNewData();
				//	break;
				case SportSiteAction.GoIsfHome:
					Response.Redirect(Data.InternationalIsfWebsite, true);
					break;
				case SportSiteAction.NationalChampionships:
					DisplayChampionships(NavBarLink.NationalChamps);
					break;
				case SportSiteAction.OtherChampionships:
					DisplayChampionships(NavBarLink.OtherChamps);
					break;
				case SportSiteAction.ClubChampionships:
					DisplayChampionships(NavBarLink.ClubChamps);
					break;
				case SportSiteAction.Register:
					Response.Redirect("Register.aspx");
					break;
				case SportSiteAction.ViewPictureGallery:
					Response.Redirect("Gallery.aspx");
					break;
				case SportSiteAction.RelevantLinks:
					RelevantLinks();
					break;
				case SportSiteAction.ShowArticle:
					if (Request.QueryString["id"] == "all")
					{
						ShowAllArticles();
					}
					else
					{
						int articleID = Tools.CIntDef(Request.QueryString["id"], -1);
						if (articleID >= 0)
							ShowArticle(articleID);
					}
					break;
				case SportSiteAction.ViewCalendar:
					ViewEventCalendar();
					break;
				case SportSiteAction.ViewDynamicPage:
					ViewDymaicPage();
					break;
				case SportSiteAction.ViewNews:
					int newsID = Tools.CIntDef(Request.QueryString["id"], -1);
					if (newsID >= 0)
						ShowFlashNews(newsID);
					break;
				case SportSiteAction.WriteToUs:
					WriteToUs();
					break;
				case SportSiteAction.ShowPollResults:
					ShowPollResults();
					break;
				case SportSiteAction.PracticeCampRegister:
					RegisterPracticeCamp();
					break;
				case SportSiteAction.TeacherCourseRegister:
					RegisterTeacherCourse();
					break;
				default:
					//add sub articles:
					AddViewText(BuildSubArticles());
					IsfMainView.ShowLogoOne();
					break;
			}
		}

		private void AddStyle()
		{
			lbStyle.Text = "";
		}
		#endregion

		#region basic action handlers
		#region Event Calendar
		private void ViewEventCalendar()
		{
			SideNavBar.Links.MakeLinkActive(NavBarLink.Calendar);
			IsfMainView.SetPageCaption("לוח אירועים");

			//add control:
			IsfMainView.AddContents("<div class=\"OrdinaryArticlePanel\" id=\"CalendarArticlePanel\">");
			SportSite.Controls.Article objArticle = (SportSite.Controls.Article)
				this.LoadControl("Controls/Article.ascx");
			objArticle.Type = ArticleType.Main;
			objArticle.ExternalArticle = true;
			IsfMainView.AddContents(objArticle);
			IsfMainView.AddContents("</div><br />");

			IsfMainView.AddContents("<div id=\"CalendarBody\">");

			SportSite.Controls.EventCalendar objCalendar =
				new SportSite.Controls.EventCalendar();

			objCalendar.CellAddLight = 30;

			IsfMainView.AddContents(objCalendar);
			IsfMainView.AddContents("</div>" + Tools.BuildVisitorActionsPanel("CalendarBody", "לוח אירועים", IsfMainView.clientSide));
		}
		#endregion

		#region registration actions
		private void RegisterPracticeCamp()
		{
			int id = Core.PracticeCamp.GetDynamicPageID(this.Server);
			if (id >= 0)
				Response.Redirect(string.Format("{0}?action={1}&id={2}&register=1", this.Request.FilePath, SportSiteAction.ViewDynamicPage, id), true);
		}

		private void RegisterTeacherCourse()
		{
			int id = TeacherCourseManager.GetDynamicPageID(this.Server);
			if (id >= 0)
			{
				string strURL = string.Format("{0}?action={1}&id={2}&register=1", this.Request.FilePath, SportSiteAction.ViewDynamicPage, id);
				if (this.Request.QueryString["coach"] == "1")
					strURL += "&coach=1";
				Response.Redirect(strURL, true);
			}
		}
		#endregion

		#region Dynamic Page
		private void ViewDymaicPage()
		{
			WebSiteServices.PageData page = null;

			//initialize service:
			WebSiteServices.WebsiteService websiteService =
				new WebSiteServices.WebsiteService();

			//get desired page:
			NavBarLink navLink = Tools.StrToNavBarLink(Request.QueryString["page"]);
			Data.DynamicLinkData linkData = Data.DynamicLinkData.Empty;
			Data.DynamicLinkData objParentLink = Data.DynamicLinkData.Empty;
			string strParentName = "";
			int pageID = -1;
			if (Request.QueryString["page"] == null)
				pageID = Tools.CIntDef(Request.QueryString["id"], -1);

			string cacheKey = "DynamicPage_";
			cacheKey += (pageID >= 0) ? pageID.ToString() : navLink.ToString();
			foreach (string qsKey in Request.QueryString.Keys)
			{
				if (qsKey.StartsWith("W", StringComparison.CurrentCultureIgnoreCase) && qsKey.Length > 1 && char.IsDigit(qsKey[1]))
				{
					cacheKey += "_" + qsKey + "_" + Request.QueryString[qsKey];
					break;
				}
			}
			object data;
			if (CacheStore.Instance.Get(cacheKey, out data))
			{
				page = (WebSiteServices.PageData)data;
			}
			else
			{
				if (pageID >= 0)
				{
					page = websiteService.GetPageData(pageID);
					linkData = Data.FindLinkByCaption(page.Caption);
				}
				else
				{
					//find the proper dynamic link:
					int linkIndex = -1;
					for (int i = 0; i < Data.DynamicLinks.Length; i++)
					{
						if (Data.DynamicLinks[i].Link == navLink)
						{
							linkIndex = i;
							break;
						}
					}

					//abort if invalid:
					if (linkIndex >= 0)
					{
						//get dynamic link:
						linkData = Data.DynamicLinks[linkIndex];

						//maybe nested?
						int tmp = 1;
						strParentName = "";
						objParentLink = linkData;
						if ((linkData.Children != null) && (linkData.Children.Length > 0))
							strParentName = linkData.Text;
						while ((linkData.Children != null) && (Request.QueryString["w" + tmp] != null))
						{
							int W = Tools.CIntDef(Request.QueryString["w" + tmp], -1) - 1;
							if ((W < 0) || (W >= linkData.Children.Length))
								break;
							linkData = linkData.Children[W];
							tmp++;
						}

						//get page data from database:
						page = websiteService.FindPageData(linkData.Text, (int)linkData.Link);
					}
				}
				if (page != null && page.ID >= 0)
					CacheStore.Instance.Update(cacheKey, page, 5);
			}

			if (page == null || page.ID < 0)
			{
				AddViewError("עמוד לא חוקי");
				return;
			}

			//text file?
			if (!System.IO.Directory.Exists(Server.MapPath("Public/DynamicPages")))
				System.IO.Directory.CreateDirectory(Server.MapPath("Public/DynamicPages"));
			string strFilePath = Server.MapPath("Public/DynamicPages/" + page.ID + ".txt");
			if (System.IO.File.Exists(strFilePath))
			{
				page.Contents = String.Join("\n", Sport.Common.Tools.ReadTextFile(strFilePath, true));
			}

			//change active link:
			SideNavBar.Links.MakeLinkActive(linkData.Link, linkData.Text);

			//caption:
			string strCaption = (strParentName.Length == 0) ? (page.Caption) : (strParentName);
			IsfMainView.SetPageCaption(strCaption);

			//initialize html container:
			System.Text.StringBuilder sb = new System.Text.StringBuilder();

			//details?
			if (strParentName.Length > 0)
			{
				string strLogo = Tools.GetPageLogo(objParentLink, this.Server);
				sb.Append(Tools.BuildDetailsTable(strLogo, linkData.Text,
					this.Server, this.Page, -1));
			}

			//practice camp?
			bool blnPracticeCamp = (page.ID == Core.PracticeCamp.GetDynamicPageID(this.Server));
			if (blnPracticeCamp)
			{
				if (Page.IsPostBack || Request.QueryString["register"] == "1")
				{
					sb.Append(PracticeCampRegister());
					IsfMainView.AddContents(sb.ToString());
					return;
				}
			}

			//teacher course?
			bool blnTeacherCourse = (page.ID == TeacherCourseManager.GetDynamicPageID(this.Server));
			if (blnTeacherCourse)
			{
				if (Page.IsPostBack || Request.QueryString["register"] == "1")
				{
					sb.Append(TeacherCourseRegister());
					IsfMainView.AddContents(sb.ToString());
					return;
				}
			}

			//information:
			sb.Append("<div id=\"DynamicPageBody\">");
			sb.Append(Tools.ToHTML(page.Contents) + "<br />");

			//author:
			if ((page.AuthorName != null) && (page.AuthorName.Length > 0))
			{
				sb.Append("<div class=\"DynamicPageAuthor\">");
				sb.Append(page.AuthorName + "<br />" + page.AuthorTitle);
				sb.Append("</div>");
			}
			sb.Append("</div>");

			//add links, if any exist:
			sb.Append(BuildLinksHTML(page.Links, "DynamicPageLinks"));

			//FAQ?
			if (Core.FAQ.DynamicPage == page.ID)
			{
				string strJS = "<script type=\"text/javascript\">";
				strJS += " var _add_question_texts=new Array(\"שם:\", " +
					"\"אימייל שלי:\", " +
					"\"נושא:\", " +
					"\"שאלה:\");";
				strJS += " var _strSubject = \"הוספת שאלה חדשה\";";
				strJS += " var _add_question_title = \"שאלות ותשובות\";";
				strJS += "</script>";
				Page.ClientScript.RegisterClientScriptBlock(Page.GetType(), "AddQuestionScript", strJS, false);
				sb.Append("<button type=\"button\" onclick=\"AddQuestion();\" >" +
					"<b>הוספת שאלה</b></button><br />");
			}

			//Practice Camp?
			if (blnPracticeCamp)
				sb.Append("<button type=\"submit\">רישום למחנה אימון</button>");

			//Teacher Course?
			if (blnTeacherCourse)
				sb.Append("<button type=\"submit\">רישום להשתלמות ארצית</button>");

			//action panel:
			string strTitle = "";
			if (strParentName.Length > 0)
				strTitle += strParentName + " - ";
			strTitle += page.Caption;
			sb.Append(Tools.BuildVisitorActionsPanel("DynamicPageBody", strTitle, IsfMainView.clientSide));

			//attachments:
			if (!Sport.Common.Tools.IsArrayEmpty(page.Attachments))
			{
				IsfMainView.clientSide.RegisterAddLight(this.Page);
				sb.Append("<hr />");
				sb.Append(Tools.BuildAttachmentsTable(page.Attachments, this.Server));
				sb.Append("<br />");
			}

			//add as pure HTML to the page:
			IsfMainView.AddContents(sb.ToString());

			//FAQ page?
			if (Core.FAQ.DynamicPage == page.ID)
			{
				//get all questions:
				Core.FAQ.Data[] arrQuestions = Core.FAQ.GetAllData();

				//display:
				AddQuestionsTable(arrQuestions);
			}
		}

		private void AddQuestionsTable(Core.FAQ.Data[] questions)
		{
			//got something?
			if ((questions == null) || (questions.Length == 0))
				return;

			//build comments data:
			ArrayList arrComments = new ArrayList();
			for (int i = 0; i < questions.Length; i++)
			{
				//get current question:
				Core.FAQ.Data question = questions[i];

				//get answer:
				string answer = question.Answer;

				//got anything?
				if ((answer == null) || (answer.Length == 0))
					continue;

				//build current comment:
				WebSiteServices.ArticleCommentData comment =
					new WebSiteServices.ArticleCommentData();
				comment.Article = -1;
				comment.Caption = "ש: " + question.Question;
				comment.Contents = "ת: " + question.Answer + "<br /><br />";
				comment.DatePosted = DateTime.MinValue;
				comment.Deleted = false;
				comment.ID = -1;
				comment.VisitorEmail = "";
				comment.VisitorIP = "";
				comment.VisitorName = question.Asker;

				//add to list:
				arrComments.Add(comment);
			} //end loop over questions

			//apply index:
			for (int i = 0; i < arrComments.Count; i++)
				(arrComments[i] as WebSiteServices.ArticleCommentData).Number = (i + 1);

			//build array:
			WebSiteServices.ArticleCommentData[] comments = (WebSiteServices.ArticleCommentData[])
				arrComments.ToArray(typeof(WebSiteServices.ArticleCommentData));

			//done.
			AddCommentsTable(comments, "שאלות ותשובות", false);
		}
		#endregion

		#region Articles
		private void ShowAllArticles()
		{
			//caption:
			IsfMainView.SetPageCaption("כתבות");
			
			object data;
			int[] articleIDs = null;
			string strSubCaption = "";
			int groupIndex = Tools.CIntDef(Request.QueryString["index"], 0);
			string[] arrSubCaptions = null;
			List<int[]> arrAllIDs = null;
			if (CacheStore.Instance.Get("Main_Article_IDs", out data))
			{
				arrAllIDs = (List<int[]>)data;
				if (groupIndex >= arrAllIDs.Count)
					groupIndex = 0;
				articleIDs = arrAllIDs[groupIndex];
				if (CacheStore.Instance.Get("Main_Article_Subcaptions", out data))
					arrSubCaptions = (string[])data;
				if (arrSubCaptions != null && groupIndex < arrSubCaptions.Length)
					strSubCaption = arrSubCaptions[groupIndex];
			}
			else
			{
				//initialize service:
				WebSiteServices.WebsiteService websiteService = new WebSiteServices.WebsiteService();

				//get all articles:
				List<WebSiteServices.ArticleData> articles = new List<WebSiteServices.ArticleData>(websiteService.GetArticles(-1));

				//remove ZooZoo articles
				List<int> arrZooZooArticles = new List<int>(ZooZooManager.Instance.Articles);
				articles.RemoveAll(a => { return arrZooZooArticles.IndexOf(a.ID) >= 0; });

				//make groups:
				List<WebSiteServices.ArticleData[]> arrGroups = Tools.BuildArticleGroups(articles.ToArray());

				//make sure it's valid:
				if (groupIndex >= arrGroups.Count)
					groupIndex = 0;

				arrAllIDs = arrGroups.ConvertAll(g => g.ToList().ConvertAll(a => a.ID).ToArray());
				arrSubCaptions = arrGroups.ConvertAll(g => Tools.BuildArticlesGroupCaption(g)).ToArray();

				articleIDs = arrAllIDs[groupIndex];
				strSubCaption = arrSubCaptions[groupIndex];

				//store:
				CacheStore.Instance.Update("Main_Article_IDs", arrAllIDs, 5);
				CacheStore.Instance.Update("Main_Article_Subcaptions", arrSubCaptions, 5);
			}

			//output sub caption:
			IsfMainView.AddContents(Tools.BuildPageSubCaption(strSubCaption, this.Server, IsfMainView.clientSide));

			//iterate over articles:
			for (int index = 0; index < articleIDs.Length; index++)
			{
				//get current article:
				int curArticleID = articleIDs[index];

				//add control:
				IsfMainView.AddContents("<div class=\"OrdinaryArticlePanel\">");
				SportSite.Controls.Article objArticle = (SportSite.Controls.Article)
					this.LoadControl("Controls/Article.ascx");
				objArticle.Type = ArticleType.Ordinary;
				objArticle.ArticleID = curArticleID;
				IsfMainView.AddContents(objArticle);
				IsfMainView.AddContents("</div><br />");
			}

			//history:
			if (arrSubCaptions != null && arrSubCaptions.Length > 0)
			{
				string strPageUrl = this.Page.Request.FilePath;
				IsfMainView.AddContents(Tools.BuildPageSubCaption("ארכיון כתבות", this.Server, IsfMainView.clientSide));
				System.Text.StringBuilder sb = new System.Text.StringBuilder();
				string strBaseLink = string.Format("<a href=\"{0}?action={1}&id=all&index=%index\" style=\"color: #CC0000;\">%text</a>", strPageUrl, SportSiteAction.ShowArticle.ToString());
				sb.Append("<ul id=\"ArticlesHistoryPanel\">");
				for (int i = 0; i < arrSubCaptions.Length; i++)
				{
					if (i == groupIndex)
						continue;
					string strCurText = arrSubCaptions[i];
					strCurText += Tools.MultiString(Server.HtmlEncode(">"), 2);
					sb.Append("<li>");
					sb.Append(strBaseLink.Replace("%index", i.ToString()).Replace("%text", strCurText));
					sb.Append("</li>");
				}
				sb.Append("</ul>");
				IsfMainView.AddContents(sb.ToString());
			}
		} //end function ShowAllArticles

		private void ShowArticle(int articleID)
		{
			if (articleID < 0)
			{
				AddViewError("זיהוי כתבה שגוי");
				return;
			}

			object data;
			WebSiteServices.ArticleData article = null;
			string rawCommentsHTML = "";
			string cacheKeyArticle = "ArticleData_" + articleID;
			string cacheKeyComments = "ArticleComments_" + articleID;
			if (CacheStore.Instance.Get(cacheKeyArticle, out data))
			{
				article = (WebSiteServices.ArticleData)data;
				if (CacheStore.Instance.Get(cacheKeyComments, out data))
					rawCommentsHTML = data.ToString();
			}
			else
			{
				WebSiteServices.WebsiteService websiteService = new WebSiteServices.WebsiteService();
				article = websiteService.GetArticleData(articleID);
				if (article != null && article.ID >= 0)
				{
					CacheStore.Instance.Update(cacheKeyArticle, article, 5);
					WebSiteServices.ArticleCommentData[] comments = websiteService.GetArticleComments(articleID);
					rawCommentsHTML = CommentsToString(comments, true);
					CacheStore.Instance.Update(cacheKeyComments, rawCommentsHTML, 5);
				}
				else
				{
					article = null;
				}
			}

			if (article == null)
			{
				AddViewError("זיהוי כתבה שגוי");
				return;
			}

			AddViewText(BuildArticleHtml(article));
			AddCommentsTable(rawCommentsHTML, "תגובות");

			this.Page.Title = article.Caption;
			mtDescription.Content = article.SubCaption;

			mtTitle.Content = article.Caption;
			mtTitle.Visible = true;

			//thumbnails for facebook
			string strThumbnailUrl = string.Empty;
			if (!Sport.Common.Tools.IsArrayEmpty(article.Images))
			{
				strThumbnailUrl += string.Format("{0}/{1}/{2}", Data.AppPath, Data.ArticlesImagesFolder, article.Images[0]);
			}
			else
			{
				strThumbnailUrl += string.Format("{0}/Images/default_article_image_new.JPG", Data.AppPath);
			}
			strThumbnailUrl = Tools.CheckAndCreateThumbnail(strThumbnailUrl, 130, 110, Server, true);
			mtPageThumb.Href = Tools.MapVirtualUrl(strThumbnailUrl).ToLower();
			mtPageThumb.Visible = true;
		}

		private void AddCommentsTable(string contents, string caption)
		{
			if (string.IsNullOrEmpty(contents))
				return;

			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append(Tools.BuildPageSubCaption(caption, this.Server, "#f9f9ef",
				IsfMainView.clientSide));

			sb.Append(contents);

			IsfMainView.AddExtraContents(contents);
		}

		private void AddCommentsTable(WebSiteServices.ArticleCommentData[] comments, 
			string caption, bool blnHide)
		{
			AddCommentsTable(CommentsToString(comments, blnHide), caption);
		}

		private string CommentsToString(WebSiteServices.ArticleCommentData[] comments, bool blnHide)
		{
			if (comments == null || comments.Length == 0)
				return "";

			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append("<table id=\"ArticleCommentsPanel\" cellspacing=\"0\" cellpadding=\"0\">");
			string strBaseLink = "<a href=\"javascript: void(0);\" " +
				"onclick=\"CommentClicked(this, '%container'); return false;\">";
			sb.Append("<tr>");
			sb.Append("<td style=\"width: 20px;\">&nbsp;</td>");
			sb.Append("<td style=\"width: 480px;\">&nbsp;</td>");
			sb.Append("</tr>");
			int counter = 0;
			foreach (WebSiteServices.ArticleCommentData comment in comments)
			{
				if (comment.Deleted)
					continue;
				string strSubject = comment.Caption;
				string strAuthor = comment.VisitorName;
				string strDatePosted = "";
				if (comment.DatePosted.Year > 1900)
					strDatePosted = comment.DatePosted.ToString("dd.MM.yyyy");
				string strContents = comment.Contents.Replace("\n", "<br />");
				string strContainerID = "comment_contents_" + comment.ID;
				if (strContents.Length == 0)
					strSubject += " (לת)";
				sb.Append("<tr>");
				sb.Append("<td>" + (counter + 1) +".</td>"); //comment.Number
				sb.Append("<td>");
				if (strContents.Length > 0)
					sb.Append(strBaseLink.Replace("%container", strContainerID));
				sb.Append(strSubject);
				string strExtra = "";
				if ((strAuthor.Length > 0) || (strDatePosted.Length > 0))
				{
					if (strDatePosted.Length > 0)
						strExtra += "<br />";
					strExtra += " (";
					if (strAuthor.Length > 0)
						strExtra += strAuthor;
					if (strDatePosted.Length > 0)
					{
						if (strAuthor.Length > 0)
							strExtra += ", ";
						strExtra += strDatePosted;
					}
					strExtra += ")";
				}
				sb.Append(strExtra);
				if (strContents.Length > 0)
					sb.Append("</a>");
				if (strDatePosted.Length > 0)
					sb.Append("<br />");
				sb.Append("<div style=\"height: 3px;\">&nbsp;</div>");
				if (strContents.Length > 0)
				{
					sb.Append("<div id=\"" + strContainerID + "\"");
					if (blnHide)
						sb.Append("style=\"display: none;\"");
					sb.Append(">");
					sb.Append(strContents);
					sb.Append("</div>");
				}
				sb.Append("</td>");
				sb.Append("</tr>");
				counter++;
			}
			sb.Append("</table>");
			return sb.ToString();
		}
		#endregion

		#region Flash News
		private void ShowFlashNews(int newsID)
		{
			WebSiteServices.WebsiteService websiteService =
				new WebSiteServices.WebsiteService();
			WebSiteServices.FlashNewsData news =
				websiteService.GetFlashnewsData(newsID);
			if (news.ID < 0)
			{
				AddViewError("זיהוי מבזק חדשות שגוי");
				return;
			}

			AddViewText(BuildFlashnewsHtml(news));
		}
		#endregion

		#region Practice Camp
		private string PracticeCampRegister()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();

			//sb.Append("<p><center><img src=\"images/practice_camp_header2.PNG\" /></center></p>");
			sb.Append("<h1>טופס הרשמה למחנה אימון</h1>");

			List<int> arrIgnoredPracticeCamps = (Tools.ReadIniValue("General", "IgnoredPracticeCamps", Server) + "").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList().ConvertAll(s => Int32.Parse(s)).ToList();

			bool blnAdminAccess = false;
			if (!string.IsNullOrEmpty(Request.Form["HidePracticeCamps"]) &&
				Tools.IsCurrentUserLoggedIn() &&
				Tools.GetLoggedInUserType() == UserType.Internal)
			{
				blnAdminAccess = true;
				List<Sport.Data.Entity> arrEntities = Sport.Entities.PracticeCamp.Type.GetEntities(null).ToList();
				if (arrEntities.Count == 0)
				{
					sb.Append("אין מחנות אימון פתוחים");
				}
				else
				{
					if (Request.Form["HiddenPracticeCamps"] != null)
					{
						arrIgnoredPracticeCamps.Clear();
						Request.Form["HiddenPracticeCamps"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(rawId =>
						{
							int campId;
							if (Int32.TryParse(rawId, out campId) && arrIgnoredPracticeCamps.IndexOf(campId) < 0 && arrEntities.Exists(e => e.Id.Equals(campId)))
								arrIgnoredPracticeCamps.Add(campId);
						});
						Tools.WriteIniValue("General", "IgnoredPracticeCamps", string.Join(",", arrIgnoredPracticeCamps), Server);
						sb.Append("<p style=\"color: blue; font-weight: bold;\">מחנות אימונים עודכנו בהצלחה</p>");
						blnAdminAccess = false;
					}
					else
					{
						sb.Append("<p>מחנה אימון מסומן יהיה מוסתר:</p>");
						sb.Append("<input type=\"hidden\" name=\"HiddenPracticeCamps\" value=\"\" />");
						arrEntities.ForEach(entity =>
						{
							if (Tools.IsValidEntity(entity))
							{
								sb.Append("<p><input type=\"checkbox\" name=\"HiddenPracticeCamps\" value=\"").Append(entity.Id).Append("\"");
								if (arrIgnoredPracticeCamps.IndexOf(entity.Id) >= 0)
									sb.Append(" checked=\"checked\"");
								sb.Append(" />").Append(entity.Name).Append("</p>");
							}
						});
						sb.Append("<p><input type=\"submit\" name=\"HidePracticeCamps\" value=\"עדכן נתונים\" /></p>");
					}
				}
			}

			if (!blnAdminAccess && Request.Form["FullName"] != null)
			{
				string strErrMsg = CreatePracticeCampParticipantData();
				if (strErrMsg.Length > 0)
				{
					sb.Append("<span style=\"color: red;\"><b>" + strErrMsg + "</b></span><br />");
				}
				else
				{
					if (Request.Form["final_confirm"] == "1")
					{
						sb.Append("בקשתך נקלטה במערכת ותטופל בהקדם, תודה!");
						return sb.ToString();
					}
					else
					{
						string strPracticeCampForm = String.Join("\n",
							Sport.Common.Tools.ReadTextFile(Server.MapPath("PracticeCampForm.txt"), true));
						sb.Append(Tools.OutputHiddenValues(new string[] { "PracticeCamp", "FullName", "gender", 
							"Email", "Address", "HomePhone", "CellPhone", "Birthday", "SchoolName", 
							"Coach", "HMO" }));
						Sport.Entities.PracticeCamp camp = new Sport.Entities.PracticeCamp(Int32.Parse(Request.Form["PracticeCamp"]));
						strPracticeCampForm = strPracticeCampForm.Replace("@Sport", camp.Sport.Name);
						strPracticeCampForm = strPracticeCampForm.Replace("@Name", Request.Form["FullName"]);
						strPracticeCampForm = strPracticeCampForm.Replace("@Address", Request.Form["Address"]);
						strPracticeCampForm = strPracticeCampForm.Replace("@HomePhone", Common.Tools.CStrN(Request.Form["HomePhone"], "_________________"));
						strPracticeCampForm = strPracticeCampForm.Replace("@CellPhone", Common.Tools.CStrN(Request.Form["CellPhone"], "_________________"));
						strPracticeCampForm = strPracticeCampForm.Replace("@BirthDay", Common.Tools.CStrN(Request.Form["Birthday"], "_________________"));
						strPracticeCampForm = strPracticeCampForm.Replace("@Email", Common.Tools.CStrN(Request.Form["Email"], "_________________"));
						strPracticeCampForm = strPracticeCampForm.Replace("@School", Common.Tools.CStrN(Request.Form["SchoolName"], "_________________"));
						strPracticeCampForm = strPracticeCampForm.Replace("@Coach", Common.Tools.CStrN(Request.Form["Coach"], "_________________"));
						strPracticeCampForm = strPracticeCampForm.Replace("@HMO", Common.Tools.CStrN(Request.Form["HMO"], "_________________"));
						strPracticeCampForm = strPracticeCampForm.Replace("@CampDates", camp.DateStart.ToString("dd/MM/yyyy") + " עד " + camp.DateFinish.ToString("dd/MM/yyyy"));
						sb.Append("<input type=\"hidden\" name=\"final_confirm\" value=\"1\" />");
						sb.Append("<div id=\"PracticeCampForm\" style=\"border: 1px solid black;\">");
						sb.Append(strPracticeCampForm);
						sb.Append("</div>");
						sb.Append(Tools.BuildVisitorActionsPanel("PracticeCampForm", IsfMainView.clientSide));
						sb.Append("<button type=\"submit\" id=\"btnConfirm\"><b>אישור סופי</b></button>");
						return sb.ToString();
					}
				}
			}

			if (!blnAdminAccess)
			{
				int campCount = 0;
				string strComboHTML  = Tools.CreateEntitiesCombo("בחר מחנה אימון רצוי", "PracticeCamp", Sport.Entities.PracticeCamp.Type, arrIgnoredPracticeCamps, ref campCount);
				if (campCount == 0)
				{
					sb.Append("<p style=\"color: red; font-weight: bold;\">אין כרגע מחנות אימון פתוחים להרשמה</p>");
				}
				else
				{
					string ballKey = SportSite.Common.Tools.AddBall();
					string ballImageSrc = "balls.aspx?key=" + ballKey;
					sb.AppendFormat("<input type=\"hidden\" name=\"ball_key\" value=\"{0}\" />", ballKey);
					sb.Append(strComboHTML);
					sb.Append("<p>שם פרטי ושם משפחה: <input type=\"text\" size=\"30\" name=\"FullName\" value=\"").Append(Request.Form["FullName"]).Append("\" />&nbsp;&nbsp;&nbsp;");
					sb.Append("מין: <input type=\"radio\" name=\"gender\" value=\"M\" /> זכר" +
						"<input type=\"radio\" name=\"gender\" value=\"F\" /> נקבה</p>");
					sb.Append("<p>דואר אלקטרוני: <input type=\"text\" size=\"25\" name=\"Email\" value=\"").Append(Request.Form["Email"]).Append("\" /></p>");
					sb.Append("<p>כתובת: <input type=\"text\" size=\"30\" name=\"Address\" value=\"").Append(Request.Form["Address"]).Append("\" /></p>");
					sb.Append("<p>טלפון בית: <input type=\"text\" size=\"10\" name=\"HomePhone\" value=\"").Append(Request.Form["HomePhone"]).Append("\" />&nbsp;&nbsp;&nbsp;" +
						"טלפון נייד: <input type=\"text\" size=\"10\" name=\"CellPhone\" value=\"").Append(Request.Form["CellPhone"]).Append("\" /></p>");
					sb.Append("<p>תאריך לידה: <input type=\"text\" size=\"10\" name=\"Birthday\" value=\"").Append(Request.Form["Birthday"]).Append("\" /></p>");
					sb.Append("<p>שם בית ספר: <input type=\"text\" size=\"25\" name=\"SchoolName\" value=\"").Append(Request.Form["SchoolName"]).Append("\" /></p>");
					sb.Append("<p>שם המאמן/ת: <input type=\"text\" size=\"25\" name=\"Coach\" value=\"").Append(Request.Form["Coach"]).Append("\" /></p>");
					sb.Append("<p>קופת חולים: <input type=\"text\" size=\"20\" name=\"HMO\" value=\"").Append(Request.Form["HMO"]).Append("\" /></p>");
					sb.Append("<p>כמה כדורים יש למטה? <input type=\"text\" size=\"20\" name=\"Balls\" /><br /><img src=\"").Append(ballImageSrc).Append("\" /></p>");
					sb.Append("<p><center><input type=\"image\" src=\"Images/big_send_button.jpg\" /></center></p>");
				}

				if (Tools.IsCurrentUserLoggedIn() && Tools.GetLoggedInUserType() == UserType.Internal)
					sb.Append("<p><input type=\"submit\" name=\"HidePracticeCamps\" value=\"הסתרת מחנות אימון\" /></p>");
			}

			sb.Append("");

			return sb.ToString();
		}

		private string CreatePracticeCampParticipantData()
		{
			int practiceCampId = Tools.CIntDef(Request.Form["PracticeCamp"], -1);
			if (practiceCampId < 0)
				return "זיהוי מחנה אימון שגוי";

			Sport.Entities.PracticeCamp practiceCamp = null;
			try
			{
				practiceCamp = new Sport.Entities.PracticeCamp(practiceCampId);
			}
			catch
			{ }
			if (practiceCamp == null)
				return "מחנה אימון זה לא קיים";

			string strFullName = Tools.CStrDef(Request.Form["FullName"], "").Trim();
			if (strFullName.Length == 0)
				return "יש להזין שם";

			string strEmail = Tools.CStrDef(Request.Form["Email"], "").Trim();
			if (strEmail.Length == 0)
				return "יש להזין אימייל";

			string strAddress = Tools.CStrDef(Request.Form["Address"], "").Trim();
			if (strAddress.Length == 0)
				return "יש להזין כתובת";

			string strHomePhone = Tools.CStrDef(Request.Form["HomePhone"], "").Trim();
			string strCellPhone = Tools.CStrDef(Request.Form["CellPhone"], "").Trim();
			if (strHomePhone.Length == 0 && strCellPhone.Length == 0)
			{
				return "יש להזין לפחות מספר טלפון אחד";
			}

			if (Request.Form["final_confirm"] != "1")
			{
				string ballKey = Request.Form["ball_key"] + "";
				int ballsCount = Tools.CIntDef(Request.Form["Balls"], 0);
				if (ballKey.Length == 0 || ballsCount <= 0 || Tools.GetBallValue(ballKey) != ballsCount)
				{
					return "מספר כדורים לא הוזן או שגוי";
				}
			}

			if (Request.Form["final_confirm"] == "1")
			{
				Sport.Types.Sex gender = (Request.Form["gender"] == "M") ? Sport.Types.Sex.Boys : Sport.Types.Sex.Girls;
				string strBirthday = Tools.CStrDef(Request.Form["Birthday"], "").Trim();
				string strSchoolName = Tools.CStrDef(Request.Form["SchoolName"], "").Trim();
				string strCoachName = Tools.CStrDef(Request.Form["Coach"], "").Trim();
				string strHMO = Tools.CStrDef(Request.Form["HMO"], "").Trim();

				string strRemarks = "";
				if (strCoachName.Length > 0)
					strRemarks += "מאמן: " + strCoachName;
				if (strHMO.Length > 0)
				{
					if (strRemarks.Length > 0)
						strRemarks += ", ";
					strRemarks += "קופת חולים: " + strHMO;
				}

				Sport.Data.EntityEdit participant =
					Sport.Entities.PracticeCampParticipant.Type.New();
				participant.Fields[(int)Sport.Entities.PracticeCampParticipant.Fields.IsConfirmed] = 0;
				participant.Fields[(int)Sport.Entities.PracticeCampParticipant.Fields.PracticeCamp] = practiceCampId;
				participant.Fields[(int)Sport.Entities.PracticeCampParticipant.Fields.ParticipantName] = strFullName;
				participant.Fields[(int)Sport.Entities.PracticeCampParticipant.Fields.SexType] = (int)gender;
				participant.Fields[(int)Sport.Entities.PracticeCampParticipant.Fields.ParticipantAddress] = strAddress;
				participant.Fields[(int)Sport.Entities.PracticeCampParticipant.Fields.ParticipantPhone] = strHomePhone;
				participant.Fields[(int)Sport.Entities.PracticeCampParticipant.Fields.ParticipantCellPhone] = strCellPhone;
				participant.Fields[(int)Sport.Entities.PracticeCampParticipant.Fields.ParticipantBirthday] = strBirthday;
				participant.Fields[(int)Sport.Entities.PracticeCampParticipant.Fields.ParticipantSchool] = strSchoolName;
				participant.Fields[(int)Sport.Entities.PracticeCampParticipant.Fields.Remarks] = strRemarks;
				participant.Fields[(int)Sport.Entities.PracticeCampParticipant.Fields.Email] = strEmail;
				try
				{
					Sport.Data.EntityResult result = participant.Save();
					if (!result.Succeeded)
						return "כשלון בשמירת נתונים אנא נסה שוב מאוחר יותר<!-- message: " + result.GetMessage() + " -->";
				}
				catch (Exception ex)
				{
					return "שגיאה בעת שמירת נתונים אנא נסה שנית מאוחר יותר<!-- error: " + ex.Message + " -->";
				}

				//send email
				try
				{
					Tools.SendEmail("אתר התאחדות הספורט לבתי ספר<noreply@schoolsport.co.il>", strEmail, "רישום למחנה אימון", "<b>הרשמתך למחנה אימון נקלטה במערכת הרישום של היחידה לטיפוח ספורטאים צעירים</b>");
				}
				catch
				{
				}
			}

			return "";
		}
		#endregion

		#region Teacher Course
		private string TeacherCourseRegister()
		{
			/*
			System.Xml.XmlReader reader = System.Xml.XmlReader.Create("Sample.xml");
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			while (reader.Read())
			{

				sb.Append(reader.ReadOuterXml());
			}
			reader.Close();
			*/

			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			bool blnCoachRegister = (this.Request.QueryString["coach"] == "1");
			Dictionary<int, string> sportMapping;

			Sport.Entities.Season season = new Sport.Entities.Season(Sport.Core.Session.Season);
			sb.Append("<div id=\"TeacherCourseForm\">");
			string strCaption = (blnCoachRegister) ? "טופס רישום להשתלמות מאמנים" : TeacherCourseManager.Caption;
			sb.Append("<center><h2>").Append(strCaption).Append("<br />לשנה\"ל ").Append(season.Name).Append("</h2></center>");

			string strTeacherCourseForm = String.Join("\n",
				Sport.Common.Tools.ReadTextFile(Server.MapPath("TeacherCourseForm.txt"), true));

			string[] arrSportOptions = GetSportOptions(new string[] { Request.Form["sport_1"] + "", Request.Form["sport_2"] + "" }, out sportMapping);
			strTeacherCourseForm = strTeacherCourseForm.Replace("@sport1", arrSportOptions[0]);
			strTeacherCourseForm = strTeacherCourseForm.Replace("@sport2", arrSportOptions[1]);

			strTeacherCourseForm = strTeacherCourseForm.Replace("@exp_title", (blnCoachRegister) ? "ההכשרה המבוקשת" : "השתלמות ארצית");

			Sport.Types.TeacherExpertiseLookup teacherExpertiseLookup = new Sport.Types.TeacherExpertiseLookup();
			for (int i = 1; i <= 6; i++)
				strTeacherCourseForm = strTeacherCourseForm.Replace("@ev" + i, (i - 1).ToString()).Replace("@et" + i, teacherExpertiseLookup.Lookup(i - 1));

			Sport.Types.CourseAgeRangeLookup courseAgeRangeLookup = new Sport.Types.CourseAgeRangeLookup();
			for (int i = 1; i <= 8; i++)
				strTeacherCourseForm = strTeacherCourseForm.Replace("@arv" + i, i.ToString()).Replace("@art" + i, courseAgeRangeLookup.Lookup(i));

			KeyValuePair<string, string>[] arrCourses = new KeyValuePair<string, string>[6];
			int c = 0;
			string strCurCourse;
			if (blnCoachRegister)
			{
				string strRawValue = ConfigurationManager.AppSettings["CoachTrainingData"];
				if (!string.IsNullOrEmpty(strRawValue))
				{
					IsfMainView.clientSide.AddOnloadCommand("HideBlankRadioButtons();", true);
					string[] arrHourCourses = strRawValue.Split('|');
					foreach (string strRawHourCourse in arrHourCourses)
					{
						string[] arrCourseData = strRawHourCourse.Split(',');
						if (arrCourseData.Length > 1)
						{
							string strCurHours = arrCourseData[0];
							for (int i = 1; i < arrCourseData.Length; i++)
							{
								if (c >= arrCourses.Length)
									break;
								strCurCourse = string.Format("{0} שעות {1}", strCurHours, arrCourseData[i]);
								arrCourses[c] = new KeyValuePair<string, string>(strCurHours + "_" + Server.HtmlEncode(arrCourseData[i]), strCurCourse);
								c++;
							}
							c++;
						}
					}
				}
			}
			else
			{
				TeacherCourseManager.BuildTeacherCourses(ref arrCourses);
			}

			int maxCoursesToShow = blnCoachRegister ? 999 : TeacherCourseManager.GetMaxCoursesToShow(this.Server);
			for (int i = 0; i < arrCourses.Length; i++)
			{
				KeyValuePair<string, string> curValuePair = arrCourses[i];
				if (i >= maxCoursesToShow || string.IsNullOrEmpty(curValuePair.Key))
				{
					strTeacherCourseForm = strTeacherCourseForm.Replace("@cv" + (i + 1), "\" style=\"display: none;").Replace("@ct" + (i + 1), "");
				}
				else
				{
					strTeacherCourseForm = strTeacherCourseForm.Replace("@cv" + (i + 1), curValuePair.Key).Replace("@ct" + (i + 1), curValuePair.Value);
				}
			}

			//DataServices.DataService service = new DataServices.DataService();
			int[] arrCourseSports = TeacherCourseManager.GetCourseSports(this.Server); //service.GetTeacherCourseSports();
			System.Text.StringBuilder strCourseSportHtml = new System.Text.StringBuilder();
			foreach (int sportId in arrCourseSports)
			{
				if (sportMapping.ContainsKey(sportId))
				{
					strCourseSportHtml.AppendFormat("<input type=\"radio\" name=\"course_sport\" value=\"{0}\"{1} />", sportId,
						(Request.Form["course_sport"] == sportId.ToString()) ? " checked=\"checked\"" : "");
					strCourseSportHtml.Append(sportMapping[sportId]).Append("&nbsp;&nbsp;&nbsp;");
				}
			}
			strTeacherCourseForm = strTeacherCourseForm.Replace("@course_sport_selection", strCourseSportHtml.ToString());

			List<string> arrHiddenElements = new List<string>(TeacherCourseManager.HiddenElements);
			Dictionary<string, string> captions = TeacherCourseManager.ElementCaptions;
			captions.Keys.ToList().ForEach(key =>
			{
				strTeacherCourseForm = strTeacherCourseForm.Replace("@" + key + "_display", ((arrHiddenElements.IndexOf(key) >= 0) ? " style=\"display: none;\"" : ""));
				strTeacherCourseForm = strTeacherCourseForm.Replace("@" + key + "_caption", captions[key]);
			});

			bool blnValid = false;
			Hashtable tblCourseDataValues = new Hashtable();
			if (Request.Form.Count > 0)
			{
				blnValid = CreateTeacherCourseData(ref strTeacherCourseForm, tblCourseDataValues, blnCoachRegister);
				if (Request.Form["final_confirm"] != "1")
				{
					System.Text.StringBuilder strJS = new System.Text.StringBuilder();
					strJS.Append("var _arrValues = new Array(); ");
					foreach (string key in Request.Form.Keys)
					{
						if (!key.StartsWith("_"))
						{
							strJS.Append("_arrValues[\"").Append(key).Append("\"] = \"").Append(
								Request.Form[key].Replace("\"", "\\\"")).Append("\"; ");
						}
					}
					this.ClientScript.RegisterClientScriptBlock(this.GetType(),
						"fill_values", strJS.ToString(), true);
					IsfMainView.clientSide.AddOnloadCommand("FillValues();", true);
				}
			}
			else
			{
				IsfMainView.clientSide.AddOnloadCommand("document.forms[0].elements[\"lname\"].focus();", true);
			}

			ShowTeacherCourseData(ref strTeacherCourseForm, "lname", tblCourseDataValues[(int)Sport.Entities.TeacherCourse.Fields.LastName]);
			ShowTeacherCourseData(ref strTeacherCourseForm, "fname", tblCourseDataValues[(int)Sport.Entities.TeacherCourse.Fields.FirstName]);
			ShowTeacherCourseData(ref strTeacherCourseForm, "id", tblCourseDataValues[(int)Sport.Entities.TeacherCourse.Fields.IdNumber]);
			ShowTeacherCourseData(ref strTeacherCourseForm, "bday", tblCourseDataValues[(int)Sport.Entities.TeacherCourse.Fields.BirthDay]);
			ShowTeacherCourseData(ref strTeacherCourseForm, "address", tblCourseDataValues[(int)Sport.Entities.TeacherCourse.Fields.Address]);
			ShowTeacherCourseData(ref strTeacherCourseForm, "city", tblCourseDataValues[(int)Sport.Entities.TeacherCourse.Fields.CityName]);
			ShowTeacherCourseData(ref strTeacherCourseForm, "zip", tblCourseDataValues[(int)Sport.Entities.TeacherCourse.Fields.ZipCode]);
			ShowTeacherCourseData(ref strTeacherCourseForm, "email", tblCourseDataValues[(int)Sport.Entities.TeacherCourse.Fields.Email]);
			ShowTeacherCourseData(ref strTeacherCourseForm, "home_phone", tblCourseDataValues[(int)Sport.Entities.TeacherCourse.Fields.HomePhone]);
			ShowTeacherCourseData(ref strTeacherCourseForm, "fax", tblCourseDataValues[(int)Sport.Entities.TeacherCourse.Fields.FaxNumber]);
			ShowTeacherCourseData(ref strTeacherCourseForm, "cell_phone", tblCourseDataValues[(int)Sport.Entities.TeacherCourse.Fields.CellPhone]);
			ShowTeacherCourseData(ref strTeacherCourseForm, "gender", tblCourseDataValues[(int)Sport.Entities.TeacherCourse.Fields.Gender]);
			ShowTeacherCourseData(ref strTeacherCourseForm, "school_name", tblCourseDataValues[(int)Sport.Entities.TeacherCourse.Fields.SchoolName]);
			ShowTeacherCourseData(ref strTeacherCourseForm, "school_city", tblCourseDataValues[(int)Sport.Entities.TeacherCourse.Fields.SchoolCityName]);
			ShowTeacherCourseData(ref strTeacherCourseForm, "school_address", tblCourseDataValues[(int)Sport.Entities.TeacherCourse.Fields.SchoolAddress]);
			ShowTeacherCourseData(ref strTeacherCourseForm, "sport_1", tblCourseDataValues[(int)Sport.Entities.TeacherCourse.Fields.FirstSport]);
			ShowTeacherCourseData(ref strTeacherCourseForm, "sport_2", tblCourseDataValues[(int)Sport.Entities.TeacherCourse.Fields.SecondSport]);
			ShowTeacherCourseData(ref strTeacherCourseForm, "veteranship", tblCourseDataValues[(int)Sport.Entities.TeacherCourse.Fields.Veteranship]);
			ShowTeacherCourseData(ref strTeacherCourseForm, "expertise", tblCourseDataValues[(int)Sport.Entities.TeacherCourse.Fields.Expertise]);
			ShowTeacherCourseData(ref strTeacherCourseForm, "age_range", tblCourseDataValues[(int)Sport.Entities.TeacherCourse.Fields.TeamAgeRange]);
			ShowTeacherCourseData(ref strTeacherCourseForm, "course", tblCourseDataValues[(int)Sport.Entities.TeacherCourse.Fields.CourseHoliday]);
			ShowTeacherCourseData(ref strTeacherCourseForm, "course_sport", tblCourseDataValues[(int)Sport.Entities.TeacherCourse.Fields.CourseSport]);

			sb.Append(strTeacherCourseForm);
			sb.Append("</div>");
			if (Request.Form["final_confirm"] != "1")
			{
				if (Request.Form.Count == 0 || !blnValid)
				{
					sb.Append("<p><center><input type=\"image\" src=\"Images/big_send_button.jpg\" /></center></p>");
				}
				else
				{
					sb.Append(Tools.BuildVisitorActionsPanel("TeacherCourseForm", IsfMainView.clientSide));
					sb.Append("<br />");
					sb.Append("<input type=\"hidden\" name=\"final_confirm\" value=\"1\" />");
					sb.Append("<button type=\"submit\" id=\"btnConfirm\"><b>אישור סופי</b></button>");
					sb.Append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;");
					sb.Append("<button type=\"button\" id=\"btnCancel\" onclick=\"history.go(-1);\">שינוי פרטים</button>");
				}
			}
			sb.Append("");

			return sb.ToString();
		}

		private string[] GetSportOptions(string[] arrSelValues, out Dictionary<int, string> sportMapping)
		{
			List<Sport.Data.Entity> sports = new List<Sport.Data.Entity>(Sport.Entities.Sport.Type.GetEntities(null));
			List<int> arrIncludedSportIds = new List<int>();
			sportMapping = new Dictionary<int, string>();
			int[] arrIniSports = TeacherCourseManager.GetExpertiseSports(this.Server);
			if (arrIniSports.Length > 0)
			{
				arrIncludedSportIds.AddRange(arrIniSports);
			}
			else
			{
				sports.Sort(delegate(Sport.Data.Entity ent1, Sport.Data.Entity ent2)
				{
					return ent1.Name.CompareTo(ent2.Name);
				});
				arrIncludedSportIds.AddRange(sports.ConvertAll<int>(delegate(Sport.Data.Entity entity) { return entity.Id; }).ToArray());
			}

			foreach (Sport.Data.Entity entSport in sports)
				sportMapping.Add(entSport.Id, entSport.Name);

			string[] retValue = new string[arrSelValues.Length];
			for (int i = 0; i < arrSelValues.Length; i++)
			{
				string strSelValue = arrSelValues[i];
				System.Text.StringBuilder sb = new System.Text.StringBuilder();
				sb.Append("<option value=\"\">      &nbsp;     </option>");
				foreach (int sportId in arrIncludedSportIds)
				{
					if (sportMapping.ContainsKey(sportId))
					{
						sb.AppendFormat("<option value=\"{0}\"{1}>{2}</option>", sportId,
							(sportId.ToString().Equals(strSelValue) ? " selected=\"selected\"" : ""),
							sportMapping[sportId]);
					}
				}
				retValue[i] = sb.ToString();
			}

			return retValue;
		}

		private bool CreateTeacherCourseData(ref string strTeacherCourseData, Hashtable tblCourseDataValues, bool blnCoachRegister)
		{
			List<string> arrMissingElements = new List<string>();
			string strFirstName = null, strLastName = null, strIdNumber = null;
			string strBirthDay = null, strAddress = null, strCity = null;
			string strZipCode = null, strEmail = null, strHomePhone = null;
			string strFaxNumber = null, strCellPhone = null, strSchoolName = null;
			string strSchoolCity = null, strSchoolAddress = null, strVeteranship = null;
			string strCoachCourseType = null;
			int gender = (int)Sport.Types.Sex.None;
			int sport1 = -1, sport2 = -1, courseSport = -1;
			int expertise = (int)Sport.Types.TeacherExpertise.None;
			int ageRange = (int)Sport.Types.CourseAgeRange.None;
			int courseHoliday = -1;
			int courseYear = -1;
			int coachCourseHours = -1;

			strFirstName = (string)GetTeacherCourseFormValue("fname", true, arrMissingElements);
			strLastName = (string)GetTeacherCourseFormValue("lname");
			strIdNumber = (string)GetTeacherCourseFormValue("id", true, arrMissingElements);
			strBirthDay = (string)GetTeacherCourseFormValue("b_day");
			strAddress = (string)GetTeacherCourseFormValue("address");
			strCity = (string)GetTeacherCourseFormValue("city");
			strZipCode = (string)GetTeacherCourseFormValue("zip_code");
			strEmail = (string)GetTeacherCourseFormValue("email");
			strHomePhone = (string)GetTeacherCourseFormValue("home_phone", true, arrMissingElements);
			strFaxNumber = (string)GetTeacherCourseFormValue("fax_number");
			strCellPhone = (string)GetTeacherCourseFormValue("cell_phone");
			gender = (int)GetTeacherCourseFormValue("gender", true, arrMissingElements);
			strSchoolName = (string)GetTeacherCourseFormValue("school_name", true, arrMissingElements);
			strSchoolCity = (string)GetTeacherCourseFormValue("school_city");
			strSchoolAddress = (string)GetTeacherCourseFormValue("school_address");
			sport1 = (int)GetTeacherCourseFormValue("sport_1", true, arrMissingElements);
			sport2 = (int)GetTeacherCourseFormValue("sport_2");
			strVeteranship = (string)GetTeacherCourseFormValue("veteranship");
			expertise = (int)GetTeacherCourseFormValue("expertise", true, arrMissingElements);
			ageRange = (int)GetTeacherCourseFormValue("age_range", true, arrMissingElements);
			if (blnCoachRegister)
			{
				coachCourseHours = (int)GetTeacherCourseFormValue("course_h", true, arrMissingElements);
				strCoachCourseType = (string)GetTeacherCourseFormValue("course_c", true, arrMissingElements);
			}
			else
			{
				courseHoliday = (int)GetTeacherCourseFormValue("course_h", true, arrMissingElements);
				courseYear = (int)GetTeacherCourseFormValue("course_y", true, arrMissingElements);
			}
			courseSport = (int)GetTeacherCourseFormValue("course_sport", true, arrMissingElements);

			//remove hidden elements
			List<string> hiddenElements = new List<string>(TeacherCourseManager.HiddenElements);
			arrMissingElements.RemoveAll(key => { return hiddenElements.IndexOf(key) >= 0; });

			if (arrMissingElements.Count > 0)
			{
				foreach (string strKey in arrMissingElements)
					IsfMainView.clientSide.AddOnloadCommand("MarkInvalidValue(\"" + strKey + "\");", true);
				return false;
			}

			if (Request.Form["final_confirm"] == "1")
			{
				Sport.Data.EntityEdit teacherCourse = Sport.Entities.TeacherCourse.Type.New();
				teacherCourse.Fields[(int)Sport.Entities.TeacherCourse.Fields.FirstName] = string.IsNullOrEmpty(strFirstName) ? " " : strFirstName;
				teacherCourse.Fields[(int)Sport.Entities.TeacherCourse.Fields.LastName] = strLastName;
				teacherCourse.Fields[(int)Sport.Entities.TeacherCourse.Fields.IdNumber] = string.IsNullOrEmpty(strIdNumber) ? " " : strIdNumber;
				teacherCourse.Fields[(int)Sport.Entities.TeacherCourse.Fields.BirthDay] = strBirthDay;
				teacherCourse.Fields[(int)Sport.Entities.TeacherCourse.Fields.Address] = strAddress;
				teacherCourse.Fields[(int)Sport.Entities.TeacherCourse.Fields.CityName] = strCity;
				teacherCourse.Fields[(int)Sport.Entities.TeacherCourse.Fields.ZipCode] = strZipCode;
				teacherCourse.Fields[(int)Sport.Entities.TeacherCourse.Fields.Email] = strEmail;
				teacherCourse.Fields[(int)Sport.Entities.TeacherCourse.Fields.HomePhone] = string.IsNullOrEmpty(strHomePhone) ? " " : strHomePhone;
				teacherCourse.Fields[(int)Sport.Entities.TeacherCourse.Fields.FaxNumber] = strFaxNumber;
				teacherCourse.Fields[(int)Sport.Entities.TeacherCourse.Fields.CellPhone] = strCellPhone;
				teacherCourse.Fields[(int)Sport.Entities.TeacherCourse.Fields.Gender] = gender;
				teacherCourse.Fields[(int)Sport.Entities.TeacherCourse.Fields.SchoolName] = string.IsNullOrEmpty(strSchoolName) ? " " : strSchoolName;
				teacherCourse.Fields[(int)Sport.Entities.TeacherCourse.Fields.SchoolCityName] = strSchoolCity;
				teacherCourse.Fields[(int)Sport.Entities.TeacherCourse.Fields.SchoolAddress] = strSchoolAddress;
				if (sport1 >= 0)
					teacherCourse.Fields[(int)Sport.Entities.TeacherCourse.Fields.FirstSport] = sport1;
				if (sport2 >= 0)
					teacherCourse.Fields[(int)Sport.Entities.TeacherCourse.Fields.SecondSport] = sport2;
				teacherCourse.Fields[(int)Sport.Entities.TeacherCourse.Fields.Veteranship] = (String.IsNullOrEmpty(strVeteranship)) ? 0 : Common.Tools.CIntDef(strVeteranship, 0);
				teacherCourse.Fields[(int)Sport.Entities.TeacherCourse.Fields.Expertise] = expertise;
				teacherCourse.Fields[(int)Sport.Entities.TeacherCourse.Fields.TeamAgeRange] = ageRange;
				if (blnCoachRegister)
				{
					teacherCourse.Fields[(int)Sport.Entities.TeacherCourse.Fields.CoachTrainingHours] = coachCourseHours;
					teacherCourse.Fields[(int)Sport.Entities.TeacherCourse.Fields.CoachTrainingType] = strCoachCourseType;
				}
				else
				{
					teacherCourse.Fields[(int)Sport.Entities.TeacherCourse.Fields.CourseHoliday] = courseHoliday;
					teacherCourse.Fields[(int)Sport.Entities.TeacherCourse.Fields.CourseYear] = courseYear;
				}
				teacherCourse.Fields[(int)Sport.Entities.TeacherCourse.Fields.CourseSport] = courseSport;
				teacherCourse.Fields[(int)Sport.Entities.TeacherCourse.Fields.IsConfirmed] = 0;

				string strMessage = "";
				try
				{
					Sport.Data.EntityResult result = teacherCourse.Save();
					if (!result.Succeeded)
						strMessage = "<b><font color=\"red\">כשלון בשמירת נתונים אנא נסה שוב מאוחר יותר</font></b><!-- message: " + result.GetMessage() + " -->";
					else
						strMessage = "<b><font color=\"blue\">בקשת הרישום נשלחה בהצלחה, תודה!</font></b>";
				}
				catch (Exception ex)
				{
					strMessage = "<b><font color=\"red\">שגיאה בעת שמירת נתונים אנא נסה שנית מאוחר יותר</font></b><!-- error: " + ex.Message + " -->";
				}

				strTeacherCourseData = strMessage;
				return false;
			}
			else
			{
				tblCourseDataValues[(int)Sport.Entities.TeacherCourse.Fields.LastName] = Common.Tools.CStrDef(strLastName, "");
				tblCourseDataValues[(int)Sport.Entities.TeacherCourse.Fields.FirstName] = Common.Tools.CStrDef(strFirstName, "");
				tblCourseDataValues[(int)Sport.Entities.TeacherCourse.Fields.IdNumber] =
					"<td colspan=\"9\">" + strIdNumber + "</td>";
				tblCourseDataValues[(int)Sport.Entities.TeacherCourse.Fields.BirthDay] = Common.Tools.CStrDef(strBirthDay, "");
				tblCourseDataValues[(int)Sport.Entities.TeacherCourse.Fields.Address] = Common.Tools.CStrDef(strAddress, "");
				tblCourseDataValues[(int)Sport.Entities.TeacherCourse.Fields.CityName] = Common.Tools.CStrDef(strCity, "");
				tblCourseDataValues[(int)Sport.Entities.TeacherCourse.Fields.ZipCode] = Common.Tools.CStrDef(strZipCode, ""); ;
				tblCourseDataValues[(int)Sport.Entities.TeacherCourse.Fields.Email] = Common.Tools.CStrDef(strEmail, "");
				tblCourseDataValues[(int)Sport.Entities.TeacherCourse.Fields.HomePhone] = Common.Tools.CStrDef(strHomePhone, "");
				tblCourseDataValues[(int)Sport.Entities.TeacherCourse.Fields.FaxNumber] = Common.Tools.CStrDef(strFaxNumber, "");
				tblCourseDataValues[(int)Sport.Entities.TeacherCourse.Fields.CellPhone] = Common.Tools.CStrDef(strCellPhone, "");
				tblCourseDataValues[(int)Sport.Entities.TeacherCourse.Fields.Gender] =
					(gender == (int)Sport.Types.Sex.Boys) ? "גבר" : "אישה";
				tblCourseDataValues[(int)Sport.Entities.TeacherCourse.Fields.SchoolName] = Common.Tools.CStrDef(strSchoolName, "");
				tblCourseDataValues[(int)Sport.Entities.TeacherCourse.Fields.SchoolCityName] = Common.Tools.CStrDef(strSchoolCity, "");
				tblCourseDataValues[(int)Sport.Entities.TeacherCourse.Fields.SchoolAddress] = Common.Tools.CStrDef(strSchoolAddress, "");
				tblCourseDataValues[(int)Sport.Entities.TeacherCourse.Fields.FirstSport] = Common.Tools.CStrDef(
					Common.Tools.GetEntityName(Sport.Entities.Sport.TypeName, sport1), "");
				tblCourseDataValues[(int)Sport.Entities.TeacherCourse.Fields.SecondSport] = Common.Tools.CStrDef(
					Common.Tools.GetEntityName(Sport.Entities.Sport.TypeName, sport2), "");
				tblCourseDataValues[(int)Sport.Entities.TeacherCourse.Fields.Veteranship] = Common.Tools.CStrDef(strVeteranship, "");
				tblCourseDataValues[(int)Sport.Entities.TeacherCourse.Fields.Expertise] =
					"<td colspan=\"6\">" + (new Sport.Types.TeacherExpertiseLookup()).Lookup(expertise) + "</td>";
				tblCourseDataValues[(int)Sport.Entities.TeacherCourse.Fields.TeamAgeRange] =
					"<tr><td colspan=\"4\">" + (new Sport.Types.CourseAgeRangeLookup()).Lookup(ageRange) + "</td></tr>";

				if (blnCoachRegister)
				{
					tblCourseDataValues[(int)Sport.Entities.TeacherCourse.Fields.CourseHoliday] =
						"<td colspan=\"6\">" + coachCourseHours + " שעות " + strCoachCourseType + "</td>";
				}
				else
				{
					tblCourseDataValues[(int)Sport.Entities.TeacherCourse.Fields.CourseHoliday] =
						"<td colspan=\"6\">" + (new Sport.Types.HolidayTypeLookup()).Lookup(courseHoliday) + " " +
						courseYear + "</td>";
				}

				tblCourseDataValues[(int)Sport.Entities.TeacherCourse.Fields.CourseSport] =
					Common.Tools.CStrDef(Common.Tools.GetEntityName(Sport.Entities.Sport.TypeName, courseSport), "");
			}

			return true;
		}

		private void ShowTeacherCourseData(ref string strTeacherCourseData, string key, object oValue)
		{
			string value = (oValue == null) ? null : oValue.ToString();
			if (value != null && value.Length == 0)
				value = "&nbsp;";

			strTeacherCourseData = strTeacherCourseData.Replace("@" + key + "_show", (value == null) ? "" : value);
			strTeacherCourseData = strTeacherCourseData.Replace("@" + key + "_style", (value == null) ? "" : " style=\"display: none;\"");
		}

		private object GetTeacherCourseFormValue(string key, bool required, List<string> arrMissingValues)
		{
			object oVariable = ReadTeacherCourseFormValue(key);
			if (arrMissingValues != null && required && IsValidValue(oVariable) == false)
			{
				arrMissingValues.Add(key);
			}
			return oVariable;
		}

		private bool IsValidValue(object oVariable)
		{
			if (oVariable is String && String.IsNullOrEmpty((string)oVariable))
				return false;

			if (oVariable is Int32 && ((int)oVariable) < 0)
				return false;

			return true;
		}

		private object GetTeacherCourseFormValue(string key)
		{
			return GetTeacherCourseFormValue(key, false, null);
		}

		private object ReadTeacherCourseFormValue(string key)
		{
			if (key == "id")
			{
				string retVal = "";
				for (int i = 1; i <= 9; i++)
				{
					string sCurDigit = Common.Tools.CStrDef(Request.Form["id_" + i], "");
					if (sCurDigit.Length == 1 && System.Char.IsDigit(sCurDigit[0]))
						retVal += sCurDigit;
				}
				return retVal;
			}

			if (key == "gender")
			{
				string strGender = Request.Form[key];
				if (String.IsNullOrEmpty(strGender))
					return (int)Sport.Types.Sex.None;
				else
					return (strGender == "M") ? (int)Sport.Types.Sex.Boys : (int)Sport.Types.Sex.Girls;
			}

			if (key.StartsWith("sport_") || key == "course_sport")
			{
				int sportId = Common.Tools.CIntDef(Request.Form[key], -1);
				if (sportId >= 0)
				{
					Sport.Entities.Sport sport = null;
					try
					{
						sport = new Sport.Entities.Sport(sportId);
					}
					catch { }
					if (sport == null)
						sportId = -1;
				}
				return sportId;
			}

			if (key == "expertise")
			{
				int expertise = Common.Tools.CIntDef(Request.Form[key], -1);
				if (expertise >= 0)
				{
					try
					{
						expertise = (int)((Sport.Types.TeacherExpertise)expertise);
					}
					catch
					{
						expertise = -1;
					}
				}
				else
				{
					expertise = (int)Sport.Types.TeacherExpertise.None;
				}
				return expertise;
			}

			if (key == "age_range")
			{
				int ageRange = Common.Tools.CIntDef(Request.Form[key], -1);
				if (ageRange >= 0)
				{
					try
					{
						ageRange = (int)((Sport.Types.CourseAgeRange)ageRange);
					}
					catch
					{
						ageRange = -1;
					}
				}
				else
				{
					ageRange = (int)Sport.Types.CourseAgeRange.None;
				}
				return ageRange;
			}

			if (key.StartsWith("course_"))
			{
				string strCourseData = Request.Form["course"];
				char cType = key[key.Length - 1];
				int retVal = -1;
				if (!String.IsNullOrEmpty(strCourseData) && strCourseData.Length >= 5)
				{
					string[] arrTemp = strCourseData.Split('_');
					string strValue = (cType == 'c') ? "" : "-1";
					if (arrTemp.Length == 2)
						strValue = (cType == 'h') ? arrTemp[0] : arrTemp[1];
					if (cType == 'c')
						return strValue;
					retVal = Common.Tools.CIntDef(strValue, -1);

				}

				if (cType == 'c')
					return retVal.ToString();
				else
					return retVal;
			}

			return Request.Form[key];
		}
		#endregion

		#region Write To Us
		private ListBox _sendableRegions;
		private TextBox _userName;
		private TextBox _userEmail;
		private TextBox _userComments;

		/// <summary>
		/// Activated by the "Comments" link.
		/// </summary>
		private void WriteToUs()
		{
			SideNavBar.Links.MakeLinkActive(NavBarLink.WriteUs);

			_sendableRegions = new ListBox();
			_userName = new TextBox();
			_userEmail = new TextBox();
			_userComments = new TextBox();

			_sendableRegions.ID = "sendableRegions";
			_userName.ID = "userName";
			_userEmail.ID = "userEmail";
			_userComments.ID = "userComments";

			_userComments.TextMode = TextBoxMode.MultiLine;
			_userComments.Rows = 7;

			IsfMainView.SetPageCaption("כתבו לנו");
			IsfMainView.AddContents(buildWriteToUsTable());
		}

		private Table buildWriteToUsTable()
		{
			Table result = new Table();
			result.ID = "writeToUsTable";
			result.CssClass = "WriteToUsTable";
			result.CellPadding = 2;
			result.CellSpacing = 3;
			result.HorizontalAlign = HorizontalAlign.Center;

			TableRow row = null;
			TableCell cell = null;

			_sendableRegions.SelectionMode = ListSelectionMode.Single;
			_sendableRegions.Rows = 1;

			WebSiteServices.WebsiteService service = new WebSiteServices.WebsiteService();
			WebSiteServices.RegionData[] regions = service.getSendableRegions();
			foreach (WebSiteServices.RegionData region in regions)
			{
				_sendableRegions.Items.Add(new ListItem(region.Name, region.ID.ToString()));
			}

			AddWriteToUsSection(result, "שלח ל:", _sendableRegions, 10);
			AddWriteToUsSection(result, "שם מלא:", _userName, 10);
			AddWriteToUsSection(result, "דואר אלקטרוני:", _userEmail, 15);
			AddWriteToUsSection(result, "רציתי להגיד ש:", _userComments, 10);
			row = new TableRow();
			cell = new TableCell();
			ImageButton button = new ImageButton();
			string strImageName = "Images/big_send_button.jpg";
			button.Attributes["src"] = strImageName;
			button.ID = "sendButton";
			System.Drawing.Bitmap bitmap = new Bitmap(Server.MapPath(strImageName));
			button.Style["width"] = bitmap.Width + "px";
			button.Style["height"] = bitmap.Height + "px";
			button.Style["border-style"] = "none";
			button.Click += new ImageClickEventHandler(handleWriteToUs);
			cell.HorizontalAlign = HorizontalAlign.Center;
			cell.Controls.Add(button);
			row.Cells.Add(cell);
			result.Rows.Add(row);

			_userName.Attributes["default"] = "1";
			return result;
		}

		private void AddWriteToUsSection(Table parent,
			string caption, Control control, int gap)
		{
			TableRow row = null;
			TableCell cell = null;

			row = new TableRow();
			cell = new TableCell();
			cell.Text = caption;
			row.Cells.Add(cell);
			parent.Rows.Add(row);

			row = new TableRow();
			cell = new TableCell();
			cell.Controls.Add(control);
			row.Cells.Add(cell);
			parent.Rows.Add(row);

			if (gap > 0)
			{
				row = new TableRow();
				cell = new TableCell();
				cell.Text = "&nbsp;";
				cell.Style["height"] = gap + "px";
				row.Cells.Add(cell);
				parent.Rows.Add(row);
			}
		}

		/// <summary>
		/// Returns the appropriate error message
		/// </summary>
		/// <returns></returns>
		private string handleWriteToUsData()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();

			int regionID = Tools.CIntDef(_sendableRegions.SelectedValue, -1);
			string senderName = Tools.CStrDef(_userName.Text, "");
			string senderEmail = Tools.CStrDef(_userEmail.Text, "");
			string senderComment = Tools.CStrDef(_userComments.Text, "");
			if (regionID < 0)
				sb.Append("<br />המחוז שנבחר אינו חוקי.");
			if (senderName.Length == 0)
				sb.Append("<br />לא הזנת שם.");
			if (senderEmail.Length == 0)
				sb.Append("<br />לא הזנת דואר אלקטרוני.");
			if (senderComment.Length == 0)
				sb.Append("<br />לא הזנת הערות.");
			if (sb.ToString().Length == 0)
			{
				WebSiteServices.WebsiteService service = new WebSiteServices.WebsiteService();
				string recipient = service.getRegionById(regionID).Email;
				string messageBody = "<font color=\"red\"><b>הודעה התקבלה מהאתר</b></font><br />";
				messageBody += "<b>מאת: </b>" + senderName + "<br />";
				messageBody += "<b>כתובת דואר אלקטרוני: </b>" + senderEmail + "<br />";
				messageBody += "<b>תוכן ההודעה: </b><br />" + senderComment;
				messageBody = "<div align=\"right\" dir=\"rtl\">" + messageBody + "</div>";
				Sport.Common.Tools.sendEmail("משתמש אתר<dummy@dummy.com>", recipient, "הערות", messageBody);
			}
			return sb.ToString();
		}


		private void handleWriteToUs(object sender, ImageClickEventArgs e)
		{
			string errorMessage = handleWriteToUsData();
			if (errorMessage == null || errorMessage.Length == 0)
			{
				AddViewText("<span dir=\"rtl\">הערותיך נרשמו. תודה.</span>");
				Tools.FindControlByID(this, "writeToUsTable").Visible = false;
			}
			else
				AddViewError("הערותיך לא נרשמו מהסיבות הבאות: " + errorMessage + "<br />");
		}
		#endregion

		#region Relevant Links
		private void RelevantLinks()
		{
			//highlight proper link in the side menu:
			SideNavBar.Links.MakeLinkActive(NavBarLink.RelevantLinks);

			//get all the links from link manager:
			LinkManager.LinkGroupData[] arrGroups = LinkManager.GetLinkGroups(Server);

			//initialize html container:
			System.Text.StringBuilder sb = new System.Text.StringBuilder();

			//page caption:
			IsfMainView.SetPageCaption("קישורים רלוונטים");

			//build groups html:
			sb.Append("<div id=\"RelevantLinks\">");
			int groupIndex = 0;
			foreach (LinkManager.LinkGroupData groupData in arrGroups)
			{
				//caption
				sb.Append(Tools.BuildPageSubCaption(groupData.Caption, this.Server, IsfMainView.clientSide));

				//links:
				if ((groupData.Links != null) && (groupData.Links.Length > 0))
				{
					ArrayList arrLinks = new ArrayList(groupData.Links);
					int[] order = Tools.ReadIniOrder("Links", "GroupOrder_" + groupIndex,
						arrLinks.Count, this.Server);
					arrLinks = Tools.ReArrangeArray(arrLinks, order);
					sb.Append(BuildLinksHtml((Core.LinkManager.LinkData[])
						arrLinks.ToArray(typeof(Core.LinkManager.LinkData))));
				}

				//new line...
				sb.Append("<br />");
				groupIndex++;
			}
			sb.Append("</div><br /><br />");

			//add the html:
			AddViewText(sb.ToString());

			//add control:
			Tools.LoadBanner(this, IsfMainView, BannerType.Advertisement_Secondary);
		}
		#endregion

		#region Show Poll Results
		private void ShowPollResults()
		{
			IsfMainView.SetPageCaption("תוצאות סקר");

			//add control:
			SportSite.Controls.OnlinePoll objPoll = (SportSite.Controls.OnlinePoll)
				this.LoadControl("Controls/OnlinePoll.ascx");
			objPoll.ShowResults = true;
			IsfMainView.AddContents(objPoll);
			IsfMainView.AddContents("<br /><br />");

			//add control:
			Tools.LoadBanner(this, IsfMainView, BannerType.Advertisement_Secondary);
		}
		#endregion

		#region Other Championships
		private void DisplayChampionships(NavBarLink champType)
		{
			//store selected year in session
			if (Request.Form["year"] != null)
			{
				int selSeason = Tools.CIntDef(Request.Form["year"], -1);
				if (selSeason > 0)
					Session["season"] = selSeason;
			}

			//get desired region:
			int regionID = Tools.CIntDef(Request.QueryString["r"], -1);

			//abort if invalid:
			if (regionID < 0)
			{
				AddViewError("זיהוי מחוז שגוי");
				return;
			}

			//get name of the region:
			DataServices1.DataService _service = new DataServices1.DataService();
			DataServices1.RegionData regionData = _service.GetRegionData(regionID);
			//Sport.Common.SimpleData regionData = DB.Instance.GetRegionData(regionID);
			if (regionData.ID != regionID)
			{
				AddViewError("מחוז לא חוקי");
				return;
			}
			string regionName = regionData.Name;
			if (regionData.ID == Sport.Entities.Region.CentralRegion)
				regionName = "ארצי";

			//change active link:
			SideNavBar.Links.MakeLinkActive(champType, regionName);

			//initialize html container:
			System.Text.StringBuilder sb = new System.Text.StringBuilder();

			//caption:
			string strCaption = "N/A";
			switch (champType)
			{
				case NavBarLink.OtherChamps:
					strCaption = "אירועי ספורט";
					break;
				case NavBarLink.ClubChamps:
					strCaption = "מועדוני ספורט";
					break;
				case NavBarLink.NationalChamps:
					strCaption = "אליפויות ארציות";
					break;
			}
			IsfMainView.SetPageCaption(strCaption + " - " + regionName);

			//got article?
			WebSiteServices.WebsiteService websiteService =
				new WebSiteServices.WebsiteService();
			WebSiteServices.ArticleData article = websiteService.GetRegionArticles(
				regionID, (champType == NavBarLink.ClubChamps));
			if ((article != null) && (article.ID >= 0))
			{
				//add control:
				IsfMainView.AddContents("<div class=\"OrdinaryArticlePanel\">");
				SportSite.Controls.Article objArticle = (SportSite.Controls.Article)
					this.LoadControl("Controls/Article.ascx");
				objArticle.Type = ArticleType.Ordinary;
				objArticle.ArticleID = article.ID;
				objArticle.ExternalArticle = true;
				IsfMainView.AddContents(objArticle);
				IsfMainView.AddContents("</div><br />");
			}

			Sport.Data.Entity[] arrSeasons = Sport.Entities.Season.Type.GetEntities(null);
			if (arrSeasons != null && arrSeasons.Length > 1)
			{
				System.Text.StringBuilder strHTML = new System.Text.StringBuilder();
				strHTML.Append("עונה: ").Append(Common.Tools.BuildAutoSubmitCombo("year"));
				int curSeason = Tools.CIntDef(Session["season"], -1);
				if (curSeason < 0)
					curSeason = Sport.Core.Session.Season;
				foreach (Sport.Data.Entity entSeason in arrSeasons)
				{
					strHTML.Append("<option value=\"").Append(entSeason.Id).Append("\"");
					if (entSeason.Id == curSeason)
						strHTML.Append(" selected=\"selected\"");
					strHTML.Append(">").Append(entSeason.Name).Append("</option>");
				}
				strHTML.Append("</select><br /><br />");
				IsfMainView.AddContents(strHTML.ToString());
			}

			//get all proper championships.
			Sport.Data.EntityFilter filter = Tools.GetChampionshipDefaultFilter(false);
			filter.Add(new Sport.Data.EntityFilterField(
				(int)Sport.Entities.Championship.Fields.Region, regionID));
			if (champType != NavBarLink.NationalChamps)
			{
				int isClubs = (champType == NavBarLink.ClubChamps) ? 1 : 0;
				filter.Add(new Sport.Data.EntityFilterField(
					(int)Sport.Entities.Championship.Fields.IsClubs, isClubs));
			}
			Sport.Data.Entity[] arrChamps = Sport.Entities.Championship.Type.GetEntities(filter);

			//convert all to real championships:
			ArrayList champs = new ArrayList();
			foreach (Sport.Data.Entity champEnt in arrChamps)
			{
				Sport.Entities.Championship champ = null;
				try
				{
					champ = new Sport.Entities.Championship(champEnt);
				}
				catch (Exception ex)
				{
					System.Diagnostics.Debug.WriteLine("failed to load championship: " + ex.Message);
					System.Diagnostics.Debug.WriteLine("failed to load championship: " + ex.StackTrace);
				}
				if (champ != null)
					champs.Add(champ);
			}

			//sort by sport:
			champs.Sort(new ChampSportComparer());

			//iterate over championships and add proper HTML:
			sb.Append("<div id=\"OtherChampionships\">");
			int lastSportID = -1;
			//string strBaseChampLink="<a href=\"Results.aspx?championship=%champ\" class=\"PageChampLink\">%text</a>";
			string strBaseCategoryLink = "<a id=\"%id\" href=\"Results.aspx?category=%category&section=%section\" class=\"PageCatLink\">%text</a>";
			foreach (Sport.Entities.Championship champ in champs)
			{
				sb.Append("<a name=\"champ_" + champ.Id + "\">");
				if (champ.Sport.Id != lastSportID)
				{
					if (lastSportID >= 0)
						sb.Append("<br /><br />");
					sb.Append(Tools.BuildPageSubCaption(champ.Sport.Name, this.Server, IsfMainView.clientSide));
				}
				//string strChampLink=strBaseChampLink.Replace("%champ", champ.Id.ToString());
				//sb.Append(strChampLink.Replace("%text", champ.Name)+"<br />");
				Sport.Entities.ChampionshipCategory[] categories = champ.GetCategories();
				if (categories.Length > 0)
				{
					sb.Append("<ul>");
					sb.Append(champ.Name);
					foreach (Sport.Entities.ChampionshipCategory category in categories)
					{
						string strLinkID = "cat_link_" + category.Id;
						string strContainerID = "cat_container_" + category.Id;
						string strCategoryLink = strBaseCategoryLink.Replace("%category", category.Id.ToString());
						bool blnCompetition = (category.Championship.Sport.SportType == Sport.Types.SportType.Competition);
						string[] arrItemsText = (blnCompetition) ?
							Data.CompetitionChampMenuItemsText : Data.MatchChampMemuItemsText;
						strCategoryLink = strCategoryLink.Replace("%id", strLinkID);
						sb.Append("<li>");
						sb.Append(strCategoryLink.Replace("%text", category.Name));
						sb.Append("<ul id=\"" + strContainerID + "\" style=\"list-style-type: none;\">");
						for (int j = 0; j < Data.ChampMemuItemsSections.Length; j++)
						{
							sb.Append("<li>" + strCategoryLink.Replace("%section",
								Data.ChampMemuItemsSections[j]).Replace("%text",
								arrItemsText[j]) + "</li>");
						}
						sb.Append("</ul>");
						sb.Append("<script type=\"text/javascript\">");
						sb.Append(" var objContainer=document.getElementById(\"" + strContainerID + "\");");
						sb.Append(" var objLink=document.getElementById(\"" + strLinkID + "\");");
						sb.Append(" if (objContainer)");
						sb.Append("   objContainer.style.display = \"none\";");
						sb.Append(" if (objLink) {");
						sb.Append("   objLink.href = \"javascript:void(0);\";");
						sb.Append("   objLink.onclick = new Function(\"ToggleVisibility('" + strContainerID + "');\");");
						sb.Append(" }");
						sb.Append("</script>");
						if (category.Id.ToString() == Request.QueryString["category"])
							IsfMainView.clientSide.AddOnloadCommand("ToggleVisibility('" + strContainerID + "')", true);
						sb.Append("</li>");
					}
					sb.Append("</ul>");
				}
				sb.Append("</a>");
				sb.Append("<br />");
				lastSportID = champ.Sport.Id;
			}
			sb.Append("</div><br />");

			//add as pure HTML to the page:
			AddViewText(sb.ToString());

			//add control:
			Tools.LoadBanner(this, IsfMainView, BannerType.Advertisement_Main);
		}
		#endregion
		#endregion

		#region helper functions
		private string BuildLinksHTML(WebSiteServices.LinkData[] links, string strCaption)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();

			//got anything?
			if ((links != null) && (links.Length > 0))
			{
				//links are available, add each link as new line.
				sb.Append("<hr />");
				sb.Append("<div id=\"" + strCaption + "Links\" dir=\"rtl\" ");
				sb.Append("style=\"font-family: Arial; font-size: 14px;\">");
				sb.Append("קישורים: <br />");
				for (int i = 0; i < links.Length; i++)
				{
					//get link data:
					WebSiteServices.LinkData link = links[i];
					string strText = Tools.CStrDef(link.Text, "");
					string strURL = Tools.CStrDef(link.URL, "");
					if ((strText.Length > 0) && (strURL.Length > 0))
					{
						sb.Append(Tools.BuildLink(
							strURL, strText, "_blank", "blue", null) + "<br />");
					}
				}
				sb.Append("</div>");
			}
			return sb.ToString();
		}

		/*
		private void CheckNewData()
		{
			int newsID = Tools.CIntDef(Request.QueryString["news"], -1);
			int articleID = Tools.CIntDef(Request.QueryString["article"], -1);
			WebSiteServices.WebsiteService websiteService =
				new WebSiteServices.WebsiteService();
			int lastNewsID = websiteService.GetLatestNewsID();
			int lastArticleID = websiteService.GetLatestArticleID();
			if ((lastNewsID > newsID) || (lastArticleID > articleID))
			{
				System.Text.StringBuilder sb = new System.Text.StringBuilder();
				sb.Append("<script language=\"javascript\" type=\"text/javascript\">");
				sb.Append("  parent.location.reload();");
				sb.Append("</script>");
				//Page.RegisterClientScriptBlock("MyCode1", sb.ToString());
				Response.Write(sb.ToString());
				Response.Flush();
			}
			Response.End();
		}
		*/

		/// <summary>
		/// build the proper HTML for given article.
		/// </summary>
		private string BuildArticleHtml(WebSiteServices.ArticleData article)
		{
			//initialize sting builder to hold the HTML string:
			System.Text.StringBuilder sb = new System.Text.StringBuilder();

			//get article contents:
			string strContents = "";
			try
			{
				strContents = Tools.GetArticleContents(article, this.Server);
			}
			catch (Exception ex)
			{
				AddViewText("<span style=\"display: none;\">" + ex.Message + "</span>");
				strContents = article.Contents;
			}

			//set caption:
			IsfMainView.SetPageCaption(article.Caption);

			//make the contents html friendly:
			strContents = strContents.Replace("\n", "<br />");

			//build article HTML.
			sb.Append("<div id=\"ArticleContents\">");

			//add article sub caption and send time to the html:
			sb.Append("<div class=\"ArticleSubCaption\">" + article.SubCaption + "<br />");
			sb.Append("<span class=\"ArticleDetails\">");
			sb.Append("פורסם: " + article.Time.ToString("dd/MM/yyyy HH:mm"));
			if ((article.AuthorName != null) && (article.AuthorName.Length > 0))
			{
				sb.Append(" על ידי " + article.AuthorName);
			}
			if (article.LastModified.Year >= DateTime.Now.Year)
			{
				sb.Append(Tools.MultiString("&nbsp;", 6) + "עדכון אחרון: " +
					article.LastModified.ToString("dd/MM/yyyy HH:mm"));
			}
			sb.Append("</span></div><br />");

			//add images, if any exist:
			if (Sport.Common.Tools.IsArrayEmpty(article.Images) == false)
			{
				//iterate over images and add each as new paragraph.
				sb.Append("<div id=\"ArticleFlashes\" style=\"float: left;\">");
				for (int i = 0; i < article.Images.Length; i++)
				{
					string strDescription = "";
					if (i < article.ImageDescriptions.Length)
						strDescription = article.ImageDescriptions[i];
					Tools.Page = this.Page;
					sb.Append(Tools.BuildArticleImage(article.Images[i],
						"", 200, 200, this.Server, null, "left", strDescription,
						IsfMainView.clientSide, string.Empty));
					sb.Append("<br />");
				}
				sb.Append("</div>");
			}

			//add article contents:
			sb.Append(strContents);

			//add links, if any exist:
			sb.Append(BuildLinksHTML(article.Links, "Article"));

			//done with contents.
			sb.Append("</div>");

			//allow edit?
			if (Tools.IsCurrentUserLoggedIn() && Tools.GetLoggedInUserType() == UserType.Internal)
				sb.Append("<div><a href=\"Register.aspx?action=").Append(SportSiteAction.UpdateArticles).Append("&id=").Append(article.ID).Append("\"><img src=\"images/btn_edit_article.gif\" border=\"0\" alt=\"edit\" /></a></div>");

			sb.Append(Tools.BuildVisitorActionsPanel("ArticleContents", article.Caption, article.ID, IsfMainView.clientSide));

			//add attachments, if any exist:
			if ((article.Attachments != null) && (article.Attachments.Length > 0))
			{
				//attachments are available, add each attachment as new line.
				sb.Append("<hr />");
				sb.Append(Tools.BuildAttachmentsTable(article.Attachments, this.Server));
				/*
				sb.Append("<div id=\"ArticleAttachments\" dir=\"rtl\" ");
				sb.Append("style=\"font-family: Arial; font-size: 14px;\">");
				sb.Append("קבצים מצורפים: <br />");
				foreach (WebSiteServices.AttachmentData attachment in article.Attachments)
				{
					//add proper html:
					sb.Append(AttachmentManager.BuildAttachmentHtml(
						attachment, "", this.Server)+"<br />");
				}
				sb.Append("</div>");
				*/
			}

			sb.Append("");
			return sb.ToString();
		}

		private string BuildFlashnewsHtml(WebSiteServices.FlashNewsData newsData)
		{
			//initialize html builder:
			System.Text.StringBuilder sb = new System.Text.StringBuilder();

			//verify given data:
			if (newsData.ID < 0)
				return "";

			//initialize service:
			WebSiteServices.WebsiteService websiteService =
				new WebSiteServices.WebsiteService();

			//get all today's news:
			WebSiteServices.FlashNewsData[] arrNews =
				websiteService.GetCurrentNews();

			//find the index of the given data in the array:
			int index = -1;
			for (int i = 0; i < arrNews.Length; i++)
			{
				if (arrNews[i].ID == newsData.ID)
				{
					index = i;
					break;
				}
			}

			//valid?
			if (index < 0)
				return "";

			//flash news contents:
			string strContents = newsData.Contents.Replace("\n", "<br />");
			sb.Append("<div id=\"FlashNewsData\">");
			sb.Append("<div dir=\"rtl\" style=\"font-family: Arial; font-size: 14px; ");
			sb.Append("font-weight: bold;\">" + newsData.Time.ToString("HH:mm") + "</div>");
			sb.Append("<div dir=\"rtl\" style=\"font-family: Arial; font-size: 14px;\">");
			sb.Append(strContents + "</div>");

			//add links, if any exist:
			sb.Append(BuildLinksHTML(newsData.Links, "FlashNews"));

			//next and previous flash news:
			sb.Append("<br /><div dir=\"rtl\">");
			string strLink = "<a class=\"FlashNewsLink\" href=\"" + Data.AppPath + "/?action=";
			strLink += SportSiteAction.ViewNews + "&id=[%id]\">";
			if (index > 0)
			{
				sb.Append(strLink.Replace("[%id]", arrNews[index - 1].ID.ToString()));
				sb.Append("מבזק חדשות הבא&raquo;</a>");
			}
			if (index < (arrNews.Length - 1))
			{
				sb.Append(Tools.MultiString("&nbsp;", 25));
				sb.Append(strLink.Replace("[%id]", arrNews[index + 1].ID.ToString()));
				sb.Append("&laquo;מבזק חדשות קודם</a>");
			}
			sb.Append("</div>");

			//done.
			sb.Append("</div>");
			return sb.ToString();
		} //end function BuildFlashnewsHtml

		private string BuildSubArticles()
		{
			return "";
		}

		private string BuildLinksHtml(LinkManager.LinkData[] links)
		{
			int LinksPerRow = (3 + 1);
			int rowsToDisplay = 2;
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			result.Append("<center>");
			result.Append("<table dir=\"rtl\" style=\"text-align: center;\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\">");
			result.Append("<tr>");
			result.Append("<td style=\"width: 106px;\">&nbsp;</td>");
			result.Append("<td style=\"width: 10px;\">&nbsp;</td>");
			result.Append("<td style=\"width: 106px;\">&nbsp;</td>");
			result.Append("<td style=\"width: 10px;\">&nbsp;</td>");
			result.Append("<td style=\"width: 106px;\">&nbsp;</td>");
			result.Append("<td style=\"width: 10px;\">&nbsp;</td>");
			result.Append("<td style=\"width: 106px;\">&nbsp;</td>");
			result.Append("</tr>");
			ArrayList arrLinksWithLogo = new ArrayList();
			ArrayList arrLogosHtml = new ArrayList();
			bool blnShowAll = (Request.QueryString["all"] == "1");
			foreach (LinkManager.LinkData link in links)
			{
				string strLogoHtml = LinkManager.GetLinkLogoHtml(link.URL, this.Server, "");
				if (strLogoHtml.Length > 0)
				{
					arrLinksWithLogo.Add(link);
					arrLogosHtml.Add(strLogoHtml);
				}
			}
			int linksCount = arrLinksWithLogo.Count;
			while ((linksCount % LinksPerRow) != 0)
				linksCount++;
			int rowCount = 0;
			string strPageUrl = this.Page.Request.FilePath;
			string strShowAllLinks = string.Format("<tr><td colspan=\"7\" align=\"right\"><a href=\"{0}?action={1}&all=1\"><img src=\"{2}/Images/more_links.gif\" /></a></td></tr>", strPageUrl, SportSiteAction.RelevantLinks.ToString(), Common.Data.AppPath);
			for (int i = 0; i < linksCount; i += LinksPerRow)
			{
				if (i > 0)
				{
					result.Append("<tr><td colspan=\"7\" style=\"height: 15px;\">&nbsp;</td></tr>");

					if ((!blnShowAll) && (rowCount >= rowsToDisplay))
					{
						result.Append(strShowAllLinks);
						break;
					}
				}
				result.Append("<tr>");
				for (int j = 0; j < LinksPerRow; j++)
				{
					string curURL = "";
					string curText = "";
					if ((i + j) < arrLinksWithLogo.Count)
					{
						LinkManager.LinkData curLink = (LinkManager.LinkData)arrLinksWithLogo[i + j];
						curURL = curLink.URL;
						curText = "<a href=\"" + curURL + "\">" + curLink.Text + "</a>";
					}
					string curHTML = "&nbsp;";
					if (curURL.Length > 0)
						curHTML = arrLogosHtml[i + j].ToString() + curText;
					result.Append("<td align=\"center\">" + curHTML + "</td>");
					if (j < (LinksPerRow - 1))
						result.Append("<td align=\"center\">&nbsp;</td>");
				}
				result.Append("</tr>");
				rowCount++;
			}
			result.Append("</table></center>");
			return result.ToString();
		}

		private void AddAutoRefresh()
		{
			/*
			//abort if it's me!!!
			string strIP=Request.ServerVariables["Remote_Host"];
			if ((strIP == "10.1.1.4")||(strIP == "213.8.193.147")||(strIP == "127.0.0.1"))
			{
				return;
			}
			System.Text.StringBuilder sb=new System.Text.StringBuilder();
			WebSiteServices.WebsiteService websiteService=
				new WebSiteServices.WebsiteService();
			int lastFlashNews=websiteService.GetLatestNewsID();
			int lastArticle=websiteService.GetLatestArticleID();
			string strURL=Data.AppPath+"/?action="+SportSiteAction.CheckNewData;
			strURL += "&news="+lastFlashNews+"&article="+lastArticle;
			sb.Append("<script language=\"javascript\" type=\"text/javascript\">");
			sb.Append("  var updateTimer=window.setInterval(\"CheckForUpdates();\", ");
			sb.Append(Data.CheckNewDataInterval*1000+");");
			sb.Append("  function CheckForUpdates() {");
			sb.Append("   var objFrame=0;");
			sb.Append("   if (window.frames)");
			sb.Append("      objFrame = window.frames[\"CheckUpdatesFrame\"];");
			sb.Append("   else");
			sb.Append("      objFrame = document.frames[\"CheckUpdatesFrame\"];");
			sb.Append("   objFrame.location = \""+strURL+"\";");
			//sb.Append("   alert(typeof objFrame);");
			sb.Append("  }");
			sb.Append("</script>");
			Page.RegisterStartupScript("AutoRefreshScript", sb.ToString());
			*/
		}
		#endregion

		#region global html functions
		/// <summary>
		/// adds text to the view, i.e. the main panel and return the added object.
		/// </summary>
		private void AddViewText(string text, int index)
		{
			IsfMainView.AddContents(text);
		}

		private void AddViewText(string text)
		{
			AddViewText(text, -1);
		}

		/// <summary>
		/// add given text into the main view with given color and given size (pixels)
		/// </summary>
		private void AddViewText(string text, int fontSize, Color fontColor, int index)
		{
			AddViewText(text, -1);
		}

		private void AddViewError(string strMessage)
		{
			string strHTML = "<span dir=\"rtl\" style=\"color: red; font-weight: bold;\">";
			strHTML += strMessage + "</span>";
			AddViewText(strHTML);
		}
		#endregion

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//userManager = new UserManager(this.Session);
			//ClientSide.Page = this.Page;

			this.lbStyle = IsfMainView.lbStyle;
			this.IsfHebDateTime = IsfMainView.IsfHebDateTime;
			this.SideNavBar = IsfMainView.SideNavBar;

			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Load += new System.EventHandler(this.Page_Load);
			this.PreRender += new System.EventHandler(this.Page_PreRender);
		}
		#endregion

		#region comparers
		private class ChampSportComparer : IComparer
		{
			public int Compare(object x, object y)
			{
				if ((x is Sport.Entities.Championship) && (y is Sport.Entities.Championship))
				{
					Sport.Entities.Championship c1 = (Sport.Entities.Championship)x;
					Sport.Entities.Championship c2 = (Sport.Entities.Championship)y;
					return (c1.Sport.Id.CompareTo(c2.Sport.Id));
				}
				return 0;
			}
		}

		private class EntityNameComparer : IComparer
		{
			public int Compare(object x, object y)
			{
				Sport.Data.Entity e1 = (Sport.Data.Entity)x;
				Sport.Data.Entity e2 = (Sport.Data.Entity)y;
				if (e1 == null && e2 == null)
					return 0;
				if (e1 == null)
					return 1;
				if (e2 == null)
					return -1;
				return e1.Name.CompareTo(e2.Name);
			}
		}
		#endregion

	}
}
