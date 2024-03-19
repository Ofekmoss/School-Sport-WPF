using System;
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

namespace SportSite
{
	//541110
	//kash
	
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
		
		#region Initialization
		private void Page_Load(object sender, System.EventArgs e)
		{
			//add hitlog data:
			Common.Tools.AddHitLog(WebSiteServices.WebSitePage.Main, this.Request);
			
			//need to redirect?
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
			
			//hide caption:
			IsfMainView.SetPageCaption("");
			
			//get action:
			SportSiteAction action=Tools.StrToAction(Request.QueryString["action"]);
			
			//add the style:
			AddStyle();
			
			//set active link:
			SideNavBar.Links.MakeLinkActive(NavBarLink.MainPage);

			//decide what we have to do...
			DecideAction(action);
			
			//add javascript:
			string strCode="<script type=\"text/javascript\" src=\""+Data.AppPath+"/Common/ToolTip_V1.js\"></script>\n";
			this.Page.RegisterClientScriptBlock("ToolTip_V1", strCode);
			
			//AddAutoRefresh();
		}

		public void Page_PreRender(object sender, System.EventArgs e)
		{
			
		}

		private void DecideAction(SportSiteAction action)
		{
			if (Session["ActionToPerform"] != null)
			{
				switch ((SportSiteAction) Session["ActionToPerform"])
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
				case SportSiteAction.CheckNewData:
					CheckNewData();
					break;
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
					/* string url="http://forums.devshed.com/c-programming-42/open-file-from-net-313908.html";
					System.Uri uri=new Uri(url);
					System.Net.WebRequest objRequest=System.Net.WebRequest.CreateDefault(uri);
					System.Net.WebResponse objResponse=objRequest.GetResponse();
					Response.Write("length: "+objResponse.ContentLength+"<br />"); */
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
						int articleID=Tools.CIntDef(Request.QueryString["id"], -1);
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
					int newsID=Tools.CIntDef(Request.QueryString["id"], -1);
					if (newsID >= 0)
						ShowFlashNews(newsID);
					break;
				case SportSiteAction.WriteToUs:
					WriteToUs();
					break;
				case SportSiteAction.ShowPollResults:
					ShowPollResults();
					break;
				default:
					//add sub articles:
					AddViewText(BuildSubArticles());
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
			SportSite.Controls.Article objArticle=(SportSite.Controls.Article)
				this.LoadControl("Controls/Article.ascx");
			objArticle.Type = ArticleType.Main;
			objArticle.ExternalArticle = true;
			IsfMainView.AddContents(objArticle);
			IsfMainView.AddContents("</div><br />");
			
			IsfMainView.AddContents("<div id=\"CalendarBody\">");
			
			SportSite.Controls.EventCalendar objCalendar=
				new SportSite.Controls.EventCalendar();
			
			objCalendar.CellAddLight = 30;
			
			IsfMainView.AddContents(objCalendar);
			IsfMainView.AddContents("</div>"+Tools.BuildVisitorActionsPanel(
				"CalendarBody", this.Server, "לוח אירועים", IsfMainView.clientSide));
		}
		#endregion
		
		#region Dynamic Page
		private void ViewDymaicPage()
		{
			//get desired page:
			NavBarLink navLink=Tools.StrToNavBarLink(Request.QueryString["page"]);
			
			//find the proper dynamic link:
			int linkIndex=-1;
			for (int i=0; i<Data.DynamicLinks.Length; i++)
			{
				if (Data.DynamicLinks[i].Link == navLink)
				{
					linkIndex = i;
					break;
				}
			}
			
			//abort if invalid:
			if (linkIndex < 0)
			{
				AddViewError("עמוד לא חוקי");
				return;
			}
			
			//get dynamic link:
			Data.DynamicLinkData linkData=Data.DynamicLinks[linkIndex];
			
			//maybe nested?
			int tmp=1;
			string strParentName = "";
			Common.Data.DynamicLinkData objParentLink = linkData;
			if ((linkData.Children != null)&&(linkData.Children.Length > 0))
				strParentName = linkData.Text;
			while ((linkData.Children != null)&&(Request.QueryString["w"+tmp] != null))
			{
				int W=Tools.CIntDef(Request.QueryString["w"+tmp], -1)-1;
				if ((W < 0)||(W >= linkData.Children.Length))
					break;
				linkData = linkData.Children[W];
				tmp++;
			}
			
			//initialize service:
			WebSiteServices.WebsiteService websiteService=
				new WebSiteServices.WebsiteService();
			
			//get page data from database:
			WebSiteServices.PageData page=
				websiteService.FindPageData(linkData.Text, (int) linkData.Link);
			
			//text file?
			if (!System.IO.Directory.Exists(Server.MapPath("Public/DynamicPages")))
				System.IO.Directory.CreateDirectory(Server.MapPath("Public/DynamicPages"));
			string strFilePath=Server.MapPath("Public/DynamicPages/"+page.ID+".txt");
			if (System.IO.File.Exists(strFilePath))
			{
				page.Contents = String.Join("\n", 
					Tools.ReadTextFile(strFilePath, true));
			}
			
			//change active link:
			SideNavBar.Links.MakeLinkActive(linkData.Link, linkData.Text);
			
			//caption:
			string strCaption = (strParentName.Length == 0)?(page.Caption):(strParentName);
			IsfMainView.SetPageCaption(strCaption);
			
			//initialize html container:
			System.Text.StringBuilder sb=new System.Text.StringBuilder();
			sb.Append("<div id=\"DynamicPageBody\">");
			
			//details?
			if (strParentName.Length > 0)
			{
				string strLogo = Tools.GetPageLogo(objParentLink, this.Server);
				sb.Append(Tools.BuildDetailsTable(strLogo, linkData.Text, 
					this.Server, this.Page, -1));
			}
			
			//information:
			sb.Append(Tools.ToHTML(page.Contents));
			sb.Append("<br />");
			
			//author:
			if ((page.AuthorName != null)&&(page.AuthorName.Length > 0))
			{
				sb.Append("<div class=\"DynamicPageAuthor\">");
				sb.Append(page.AuthorName+"<br />"+page.AuthorTitle);
				sb.Append("</div>");
			}
			sb.Append("</div>");
			
			//add links, if any exist:
			sb.Append(BuildLinksHTML(page.Links, "DynamicPageLinks"));
			
			//FAQ?
			if (Core.FAQ.DynamicPage == page.ID)
			{
				string strJS = "<script type=\"text/javascript\">";
				strJS += " var _add_question_texts=new Array(\"שם:\", "+
					"\"אימייל שלי:\", "+
					"\"נושא:\", "+
					"\"שאלה:\");";
				strJS += " var _strSubject = \"הוספת שאלה חדשה\";";
				strJS += " var _add_question_title = \"שאלות ותשובות\";";
				strJS += "</script>";
				Page.RegisterClientScriptBlock("AddQuestionScript", strJS);
				sb.Append("<button type=\"button\" onclick=\"AddQuestion();\" >"+
					"<b>הוספת שאלה</b></button><br />");
			}
			
			//action panel:
			string strTitle = "";
			if (strParentName.Length > 0)
				strTitle += strParentName+" - ";
			strTitle += page.Caption;
			sb.Append(Tools.BuildVisitorActionsPanel("DynamicPageBody", 
				this.Server, strTitle, IsfMainView.clientSide));
			
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
		}
		#endregion
		
		#region Articles
		private void ShowAllArticles()
		{
			//caption:
			IsfMainView.SetPageCaption("כתבות");
			
			//initialize service:
			WebSiteServices.WebsiteService websiteService=
				new WebSiteServices.WebsiteService();
			
			//get all articles:
			WebSiteServices.ArticleData[] articles=
				websiteService.GetArticles(-1);
			
			//make groups:
			ArrayList arrGroups=Tools.BuildArticleGroups(articles);
			
			//get index:
			int groupIndex=Tools.CIntDef(Request.QueryString["index"], 0);
			
			//make sure it's valid:
			if ((groupIndex < 0)||(groupIndex >= arrGroups.Count))
				groupIndex = 0;
			
			//get proper group of articles:
			articles = (WebSiteServices.ArticleData[]) arrGroups[groupIndex];
			
			//output sub caption:
			string strSubCaption=Tools.BuildArticlesGroupCaption(articles);
			IsfMainView.AddContents(Tools.BuildPageSubCaption(strSubCaption, this.Server, IsfMainView.clientSide));
			
			//iterate over articles:
			for (int index=0; index<articles.Length; index++)
			{
				//get current article:
				WebSiteServices.ArticleData article=articles[index];
				
				//add control:
				IsfMainView.AddContents("<div class=\"OrdinaryArticlePanel\">");
				SportSite.Controls.Article objArticle=(SportSite.Controls.Article)
					this.LoadControl("Controls/Article.ascx");
				objArticle.Type = ArticleType.Ordinary;
				objArticle.ArticleID = article.ID;
				IsfMainView.AddContents(objArticle);
				IsfMainView.AddContents("</div><br />");
			}
			
			//history:
			IsfMainView.AddContents(Tools.BuildPageSubCaption("ארכיון כתבות", this.Server, IsfMainView.clientSide));
			System.Text.StringBuilder sb=new System.Text.StringBuilder();
			string strBaseLink="<a href=\"?action="+SportSiteAction.ShowArticle+"&id=all"+
				"&index=%index\" style=\"color: #CC0000;\">%text</a>";
			sb.Append("<ul id=\"ArticlesHistoryPanel\">");
			for (int i=0; i<arrGroups.Count; i++)
			{
				if (i == groupIndex)
					continue;
				string strCurText=Tools.BuildArticlesGroupCaption((WebSiteServices.ArticleData[]) arrGroups[i]);
				strCurText += Tools.MultiString(Server.HtmlEncode(">"), 2);
				sb.Append("<li>");
				sb.Append(strBaseLink.Replace("%index", i.ToString()).Replace("%text", strCurText));
				sb.Append("</li>");
			}
			sb.Append("</ul>");
			IsfMainView.AddContents(sb.ToString());
		} //end function ShowAllArticles
		
		private void ShowArticle(int articleID)
		{
			WebSiteServices.WebsiteService websiteService=
				new WebSiteServices.WebsiteService();
			WebSiteServices.ArticleData article=
				websiteService.GetArticleData(articleID);
			if (article.ID < 0)
			{
				AddViewError("זיהוי כתבה שגוי");
				return;
			}
			
			AddViewText(BuildArticleHtml(article));
			
			WebSiteServices.ArticleCommentData[] comments=
				websiteService.GetArticleComments(articleID);
			if (comments.Length > 0)
			{
				System.Text.StringBuilder sb=new System.Text.StringBuilder();
				sb.Append(Tools.BuildPageSubCaption("תגובות", this.Server, "#f9f9ef", IsfMainView.clientSide));
				sb.Append("<table id=\"ArticleCommentsPanel\" cellspacing=\"0\" cellpadding=\"0\">");
				string strBaseLink="<a href=\"javascript: void(0);\" "+
					"onclick=\"CommentClicked(this, '%container'); return false;\">";
				sb.Append("<tr>");
				sb.Append("<td style=\"width: 20px;\">&nbsp;</td>");
				sb.Append("<td style=\"width: 480px;\">&nbsp;</td>");
				sb.Append("</tr>");
				foreach (WebSiteServices.ArticleCommentData comment in comments)
				{
					if (comment.Deleted)
						continue;
					string strSubject=comment.Caption;
					string strAuthor=comment.VisitorName;
					string strDatePosted=comment.DatePosted.ToString("dd.MM.yyyy");
					string strContents=comment.Contents.Replace("\n", "<br />");
					string strContainerID="comment_contents_"+comment.ID;
					if (strContents.Length == 0)
						strSubject += " (לת)";
					sb.Append("<tr>");
					sb.Append("<td>"+comment.Number+".</td>");
					sb.Append("<td>");
					if (strContents.Length > 0)
						sb.Append(strBaseLink.Replace("%container", strContainerID));
					sb.Append(strSubject+"<br />");
					sb.Append("("+strAuthor+", "+strDatePosted+")");
					if (strContents.Length > 0)
						sb.Append("</a>");
					sb.Append("<br /><div style=\"height: 3px;\">&nbsp;</div>");
					if (strContents.Length > 0)
					{
						sb.Append("<div id=\""+strContainerID+"\" style=\"display: none;\">");
						sb.Append(strContents);
						sb.Append("</div>");
					}
					sb.Append("</td>");
					sb.Append("</tr>");
				}
				sb.Append("</table>");
				IsfMainView.AddExtraContents(sb.ToString());
			}
		}
		#endregion
		
		#region Flash News
		private void ShowFlashNews(int newsID)
		{
			WebSiteServices.WebsiteService websiteService=
				new WebSiteServices.WebsiteService();
			WebSiteServices.FlashNewsData news=
				websiteService.GetFlashnewsData(newsID);
			if (news.ID < 0)
			{
				AddViewError("זיהוי מבזק חדשות שגוי");
				return;
			}
			
			AddViewText(BuildFlashnewsHtml(news));
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
			
			TableRow row=null;
			TableCell cell=null;
			
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
			string strImageName="Images/big_send_button.jpg";
			button.Attributes["src"] = strImageName;
			button.ID = "sendButton";
			System.Drawing.Bitmap bitmap=new Bitmap(Server.MapPath(strImageName));
			button.Style["width"] = bitmap.Width+"px";
			button.Style["height"] = bitmap.Height+"px";
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
			TableRow row=null;
			TableCell cell=null;
			
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
				cell.Style["height"] = gap+"px";
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
			
			int regionID = Tools.CIntDef(_sendableRegions.SelectedValue,-1);
			string senderName =Tools.CStrDef(_userName.Text,"");
			string senderEmail =Tools.CStrDef(_userEmail.Text,"");
			string senderComment =Tools.CStrDef(_userComments.Text,"");
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
				Sport.Common.Tools.sendEmail("משתמש אתר<dummy@dummy.com>",recipient,"הערות",messageBody);
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
			LinkManager.LinkGroupData[] arrGroups=LinkManager.GetLinkGroups(Server);

			//initialize html container:
			System.Text.StringBuilder sb=new System.Text.StringBuilder();
			
			//page caption:
			IsfMainView.SetPageCaption("קישורים רלוונטים");
			
			//build groups html:
			sb.Append("<div id=\"RelevantLinks\">");
			int groupIndex=0;
			foreach (LinkManager.LinkGroupData groupData in arrGroups)
			{
				//caption
				sb.Append(Tools.BuildPageSubCaption(groupData.Caption, this.Server, IsfMainView.clientSide));
				
				//links:
				if ((groupData.Links != null)&&(groupData.Links.Length > 0))
				{
					ArrayList arrLinks=new ArrayList(groupData.Links);
					int[] order=Tools.ReadIniOrder("Links", "GroupOrder_"+groupIndex, 
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
			SportSite.Controls.OnlinePoll objPoll=(SportSite.Controls.OnlinePoll)
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
			//get desired region:
			int regionID=Tools.CIntDef(Request.QueryString["r"], -1);

			//abort if invalid:
			if (regionID < 0)
			{
				AddViewError("זיהוי מחוז שגוי");
				return;
			}

			//get name of the region:
			DataServices.DataService _service=new DataServices.DataService();
			DataServices.RegionData regionData=_service.GetRegionData(regionID);
			if (regionData.ID != regionID)
			{
				AddViewError("מחוז לא חוקי");
				return;
			}
			string regionName=regionData.Name;
			if (regionData.ID == Sport.Entities.Region.CentralRegion)
				regionName = "ארצי";
			
			//change active link:
			SideNavBar.Links.MakeLinkActive(champType, regionName);
			
			//initialize html container:
			System.Text.StringBuilder sb=new System.Text.StringBuilder();
			
			//caption:
			string strCaption="N/A";
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
			IsfMainView.SetPageCaption(strCaption+" - "+regionName);
			
			//got article?
			WebSiteServices.WebsiteService websiteService=
				new WebSiteServices.WebsiteService();
			WebSiteServices.ArticleData article=websiteService.GetRegionArticles(
				regionID, (champType == NavBarLink.ClubChamps));
			if ((article != null)&&(article.ID >= 0))
			{
				//add control:
				IsfMainView.AddContents("<div class=\"OrdinaryArticlePanel\">");
				SportSite.Controls.Article objArticle=(SportSite.Controls.Article)
					this.LoadControl("Controls/Article.ascx");
				objArticle.Type = ArticleType.Ordinary;
				objArticle.ArticleID = article.ID;
				objArticle.ExternalArticle = true;
				IsfMainView.AddContents(objArticle);
				IsfMainView.AddContents("</div><br />");
			}
			
			//get all proper championships.
			Sport.Data.EntityFilter filter = Tools.GetChampionshipDefaultFilter();
			filter.Add(new Sport.Data.EntityFilterField(
				(int) Sport.Entities.Championship.Fields.Region, regionID));
			if (champType != NavBarLink.NationalChamps)
			{
				int isClubs=(champType == NavBarLink.ClubChamps)?1:0;
				filter.Add(new Sport.Data.EntityFilterField(
					(int) Sport.Entities.Championship.Fields.IsClubs, isClubs));
			}
			Sport.Data.Entity[] arrChamps=Sport.Entities.Championship.Type.GetEntities(filter);
			
			//convert all to real championships:
			ArrayList champs=new ArrayList();
			foreach (Sport.Data.Entity champEnt in arrChamps)
			{
				Sport.Entities.Championship champ=null;
				try
				{
					champ = new Sport.Entities.Championship(champEnt);
				}
				catch (Exception ex)
				{
					System.Diagnostics.Debug.WriteLine("failed to load championship: "+ex.Message);
					System.Diagnostics.Debug.WriteLine("failed to load championship: "+ex.StackTrace);
				}
				if (champ != null)
					champs.Add(champ);
			}

			//sort by sport:
			champs.Sort(new ChampSportComparer());
			
			//iterate over championships and add proper HTML:
			sb.Append("<div id=\"OtherChampionships\">");
			int lastSportID=-1;
			//string strBaseChampLink="<a href=\"Results.aspx?championship=%champ\" class=\"PageChampLink\">%text</a>";
			string strBaseCategoryLink="<a id=\"%id\" href=\"Results.aspx?category=%category&section=%section\" class=\"PageCatLink\">%text</a>";
			foreach (Sport.Entities.Championship champ in champs)
			{
				sb.Append("<a name=\"champ_"+champ.Id+"\">");
				if (champ.Sport.Id != lastSportID)
				{
					if (lastSportID >= 0)
						sb.Append("<br /><br />");
					sb.Append(Tools.BuildPageSubCaption(champ.Sport.Name, this.Server, IsfMainView.clientSide));
				}
				//string strChampLink=strBaseChampLink.Replace("%champ", champ.Id.ToString());
				//sb.Append(strChampLink.Replace("%text", champ.Name)+"<br />");
				Sport.Entities.ChampionshipCategory[] categories=champ.GetCategories();
				if (categories.Length > 0)
				{
					sb.Append("<ul>");
					sb.Append(champ.Name);
					foreach (Sport.Entities.ChampionshipCategory category in categories)
					{
						string strLinkID="cat_link_"+category.Id;
						string strContainerID="cat_container_"+category.Id;
						string strCategoryLink=strBaseCategoryLink.Replace("%category", category.Id.ToString());
						bool blnCompetition = (category.Championship.Sport.SportType == Sport.Types.SportType.Competition);
						string[] arrItemsText = (blnCompetition)?
							Data.CompetitionChampMenuItemsText:Data.MatchChampMemuItemsText;
						strCategoryLink = strCategoryLink.Replace("%id", strLinkID);
						sb.Append("<li>");
						sb.Append(strCategoryLink.Replace("%text", category.Name));
						sb.Append("<ul id=\""+strContainerID+"\" style=\"list-style-type: none;\">");
						for (int j=0; j<Data.ChampMemuItemsSections.Length; j++)
						{
							sb.Append("<li>"+strCategoryLink.Replace("%section", 
								Data.ChampMemuItemsSections[j]).Replace("%text", 
								arrItemsText[j])+"</li>");
						}
						sb.Append("</ul>");
						sb.Append("<script type=\"text/javascript\">");
						sb.Append(" var objContainer=document.getElementById(\""+strContainerID+"\");");
						sb.Append(" var objLink=document.getElementById(\""+strLinkID+"\");");
						sb.Append(" if (objContainer)");
						sb.Append("   objContainer.style.display = \"none\";");
						sb.Append(" if (objLink) {");
						sb.Append("   objLink.href = \"javascript:void(0);\";");
						sb.Append("   objLink.onclick = new Function(\"ToggleVisibility('"+strContainerID+"');\");");
						sb.Append(" }");
						sb.Append("</script>");
						if (category.Id.ToString() == Request.QueryString["category"])
							IsfMainView.clientSide.AddOnloadCommand("ToggleVisibility('"+strContainerID+"')", true);
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
			System.Text.StringBuilder sb=new System.Text.StringBuilder();
			
			//got anything?
			if ((links != null)&&(links.Length > 0))
			{
				//links are available, add each link as new line.
				sb.Append("<hr />");
				sb.Append("<div id=\""+strCaption+"Links\" dir=\"rtl\" ");
				sb.Append("style=\"font-family: Arial; font-size: 14px;\">");
				sb.Append("קישורים: <br />");
				for (int i=0; i<links.Length; i++)
				{
					//get link data:
					WebSiteServices.LinkData link=links[i];
					string strText=Tools.CStrDef(link.Text, "");
					string strURL=Tools.CStrDef(link.URL, "");
					if ((strText.Length > 0)&&(strURL.Length > 0))
					{
						sb.Append(Tools.BuildLink(
							strURL, strText, "_blank", "blue", null)+"<br />");
					}
				}
				sb.Append("</div>");
			}
			return sb.ToString();
		}
		
		private void CheckNewData()
		{
			int newsID=Tools.CIntDef(Request.QueryString["news"], -1);
			int articleID=Tools.CIntDef(Request.QueryString["article"], -1);
			WebSiteServices.WebsiteService websiteService=
				new WebSiteServices.WebsiteService();
			int lastNewsID=websiteService.GetLatestNewsID();
			int lastArticleID=websiteService.GetLatestArticleID();
			if ((lastNewsID > newsID)||(lastArticleID > articleID))
			{
				System.Text.StringBuilder sb=new System.Text.StringBuilder();
				sb.Append("<script language=\"javascript\" type=\"text/javascript\">");
				sb.Append("  parent.location.reload();");
				sb.Append("</script>");
				//Page.RegisterClientScriptBlock("MyCode1", sb.ToString());
				Response.Write(sb.ToString());
				Response.Flush();
			}
			Response.End();
		}
		
		/// <summary>
		/// build the proper HTML for given article.
		/// </summary>
		private string BuildArticleHtml(WebSiteServices.ArticleData article)
		{
			//initialize sting builder to hold the HTML string:
			System.Text.StringBuilder sb=new System.Text.StringBuilder();
			
			//get article contents:
			string strContents="";
			try
			{
				strContents = Tools.GetArticleContents(article, this.Server);
			}
			catch (Exception ex)
			{
				AddViewText("<span style=\"display: none;\">"+ex.Message+"</span>");
				strContents = article.Contents;
			}
			
			//set caption:
			IsfMainView.SetPageCaption(article.Caption);
			
			//make the contents html friendly:
			strContents = strContents.Replace("\n", "<br />");
			
			//build article HTML.
			sb.Append("<div id=\"ArticleContents\">");
			
			//add article sub caption and send time to the html:
			sb.Append("<div class=\"ArticleSubCaption\">"+article.SubCaption+"<br />");
			sb.Append("<span class=\"ArticleDetails\">");
			sb.Append("פורסם: "+article.Time.ToString("dd/MM/yyyy HH:mm"));
			if ((article.AuthorName != null)&&(article.AuthorName.Length > 0))
			{
				sb.Append(" על ידי "+article.AuthorName);
			}
			if (article.LastModified.Year >= DateTime.Now.Year)
			{
				sb.Append(Tools.MultiString("&nbsp;", 6)+"עדכון אחרון: "+
					article.LastModified.ToString("dd/MM/yyyy HH:mm"));
			}
			sb.Append("</span></div><br />");
			
			//add images, if any exist:
			if (Sport.Common.Tools.IsArrayEmpty(article.Images) == false)
			{
				//iterate over images and add each as new paragraph.
				sb.Append("<div id=\"ArticleFlashes\" style=\"float: left;\">");
				for (int i=0; i<article.Images.Length; i++)
				{
					string strDescription="";
					if (i < article.ImageDescriptions.Length)
						strDescription = article.ImageDescriptions[i];
					Tools.Page = this.Page;
					sb.Append(Tools.BuildArticleImage(article.Images[i], 
						"", 200, 200, this.Server, null, "left", strDescription, 
						IsfMainView.clientSide));
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
			sb.Append(Tools.BuildVisitorActionsPanel("ArticleContents", 
				this.Server, article.Caption, article.ID, IsfMainView.clientSide));
			
			//add attachments, if any exist:
			if ((article.Attachments != null)&&(article.Attachments.Length > 0))
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
			System.Text.StringBuilder sb=new System.Text.StringBuilder();
			
			//verify given data:
			if (newsData.ID < 0)
				return "";
			
			//initialize service:
			WebSiteServices.WebsiteService websiteService=
				new WebSiteServices.WebsiteService();
			
			//get all today's news:
			WebSiteServices.FlashNewsData[] arrNews=
				websiteService.GetCurrentNews();
			
			//find the index of the given data in the array:
			int index=-1;
			for (int i=0; i<arrNews.Length; i++)
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
			string strContents=newsData.Contents.Replace("\n", "<br />");
			sb.Append("<div id=\"FlashNewsData\">");
			sb.Append("<div dir=\"rtl\" style=\"font-family: Arial; font-size: 14px; ");
			sb.Append("font-weight: bold;\">"+newsData.Time.ToString("HH:mm")+"</div>");
			sb.Append("<div dir=\"rtl\" style=\"font-family: Arial; font-size: 14px;\">");
			sb.Append(strContents+"</div>");
			
			//add links, if any exist:
			sb.Append(BuildLinksHTML(newsData.Links, "FlashNews"));
			
			//next and previous flash news:
			sb.Append("<br /><div dir=\"rtl\">");
			string strLink="<a class=\"FlashNewsLink\" href=\""+Data.AppPath+"/?action=";
			strLink += SportSiteAction.ViewNews+"&id=[%id]\">";
			if (index > 0)
			{
				sb.Append(strLink.Replace("[%id]", arrNews[index-1].ID.ToString()));
				sb.Append("מבזק חדשות הבא&raquo;</a>");
			}
			if (index < (arrNews.Length-1))
			{
				sb.Append(Tools.MultiString("&nbsp;", 25));
				sb.Append(strLink.Replace("[%id]", arrNews[index+1].ID.ToString()));
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
			int LinksPerRow=(3+1);
			int rowsToDisplay=2;
			System.Text.StringBuilder result=new System.Text.StringBuilder();
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
			ArrayList arrLinksWithLogo=new ArrayList();
			ArrayList arrLogosHtml=new ArrayList();
			bool blnShowAll=(Request.QueryString["all"] == "1");
			foreach (LinkManager.LinkData link in links)
			{
				string strLogoHtml=LinkManager.GetLinkLogoHtml(link.URL, this.Server, "");
				if (strLogoHtml.Length > 0)
				{
					arrLinksWithLogo.Add(link);
					arrLogosHtml.Add(strLogoHtml);
				}
			}
			int linksCount=arrLinksWithLogo.Count;
			while ((linksCount % LinksPerRow) != 0)
				linksCount++;
			int rowCount=0;
			for (int i=0; i<linksCount; i+=LinksPerRow)
			{
				if (i > 0)
				{
					result.Append("<tr><td colspan=\"7\" style=\"height: 15px;\">"+
						"&nbsp;</td></tr>");
					if ((!blnShowAll)&&(rowCount >= rowsToDisplay))
					{
						result.Append("<tr><td colspan=\"7\" align=\"right\">"+
							"<a href=\"?action="+SportSiteAction.RelevantLinks+
							"&all=1\"><img src=\""+Common.Data.AppPath+
							"/Images/more_links.gif\" /></a></td></tr>");
						break;
					}
				}
				result.Append("<tr>");
				for (int j=0; j<LinksPerRow; j++)
				{
					string curURL="";
					string curText="";
					if ((i+j) < arrLinksWithLogo.Count)
					{
						LinkManager.LinkData curLink=(LinkManager.LinkData) arrLinksWithLogo[i+j];
						curURL = curLink.URL;
						curText = "<a href=\""+curURL+"\">"+curLink.Text+"</a>";
					}
					string curHTML="&nbsp;";
					if (curURL.Length > 0)
						curHTML = arrLogosHtml[i+j].ToString()+curText;
					result.Append("<td align=\"center\">"+curHTML+"</td>");
					if (j < (LinksPerRow-1))
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
			if ((strIP == "10.1.1.4")||(strIP == "localhost")||(strIP == "127.0.0.1"))
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
			string strHTML="<span dir=\"rtl\" style=\"color: red; font-weight: bold;\">";
			strHTML += strMessage+"</span>";
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
			#region IComparer Members
			public int Compare(object x, object y)
			{
				if ((x is Sport.Entities.Championship)&&(y is Sport.Entities.Championship))
				{
					Sport.Entities.Championship c1=(Sport.Entities.Championship) x;
					Sport.Entities.Championship c2=(Sport.Entities.Championship) y;
					return (c1.Sport.Id.CompareTo(c2.Sport.Id));
				}
				return 0;
			}
			#endregion
		}
		#endregion
	}
}
