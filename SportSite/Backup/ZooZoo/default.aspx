<%@ Page Language="C#" EnableViewState="false" ValidateRequest="false"  %>
<script language="C#" runat="server">
	#region code behind
	private readonly int LOG_FILE_LINES = 25000;
	private List<Control> arrHomepageControls = new List<Control>();
	private string currentFolderVirtualPath = string.Empty;
	private string action = string.Empty;
	private bool blnHomePage = true, blnShowSideBanner = true;
	private List<string> pageIcons = new List<string>();
	private string pageCaption = "עמוד הבית";
	private int selectedPageIndex = 0;
	
	private ListItem[] arrFooterTopLinks = new ListItem[] { new ListItem("עמוד הבית", "Main"), 
		new ListItem("אירועים", "Events"), new ListItem("משחק השבוע", "WeeklyGame"), new ListItem("כתבו עלינו", "Article/All"), 
		new ListItem("תחרויות", "Championships"), new ListItem("ציוצים", "Twits"), new ListItem("צור קשר", "ContactUs") };

	private ListItem[] arrFooterBottomLinks = new ListItem[] { new ListItem("פרטיות", "Privacy"), 
		new ListItem("אבטחת מידע", "Security"), new ListItem("תנאי שימוש", "Terms") };

	public void Page_Init(object sender, EventArgs e)
	{
		arrHomepageControls.AddRange(new Control[] { pnlHomepageArticles, pnlHomepageBottomContent, pnlHomepageEventGallery, 
			pnlHomepageMiddleBanner, pnlHomepageQuickPoll, pnlSportMoviesCompetition, pnlZooZooBlog });
	}
	
	public void Page_Load(object sender, EventArgs e)
	{
		CheckForVote();
		CheckContactUs();
		
		bool blnChromeBrowser = string.Equals(Request.Browser.Browser, "Chrome", StringComparison.CurrentCultureIgnoreCase);
		if (!blnChromeBrowser)
			HtmlTag.Attributes["dir"] = "rtl";
		
	
		WriteJavaScriptVariable("_isChrome", blnChromeBrowser);

		if (!string.IsNullOrEmpty(Request.QueryString["article"]))
		{
			HandleArticles(Request.QueryString["article"]);
			return;
		}		
		
		string strQS = Request.ServerVariables["QUERY_STRING"] + "";
		if (strQS.Length > 0 && !strQS.Contains('&'))
		{
			action = strQS;
			if (action.Contains("="))
				action = action.Split('=')[0];
		}

		switch (action.ToLower())
		{
			case "about":
				ShowAboutContents();
				break;
			case "events":
			case "streetgames":				
				ShowEvents();
				break;
			case "weeklygame":
				ShowWeeklyGames();
				break;
			case "newspaper":
				ShowNewsPaper();
				break;
			case "championships":
				ShowChampionships();
				break;
			case "twits":
				ShowTwits();
				break;
			case "contactus":
				ShowContactUs();
				break;
			case "eventgallery":
				ShowEventGallery();
				break;
		}

		//Response.Write(GetSubFolderURL());
	}

	public string GetSubFolderURL()
	{
		string url = "http";
		if (string.Equals(Request.ServerVariables["HTTPS"], "ON", StringComparison.CurrentCultureIgnoreCase))
			url += "s";
		url += "://";
		url += Request.ServerVariables["SERVER_NAME"];
		int port;
		if (Int32.TryParse(Request.ServerVariables["SERVER_PORT"], out port) && port != 80)
			url += ":" + port;
		url += Request.ServerVariables["SCRIPT_NAME"];
		return url.Substring(0, url.LastIndexOf("/") + 1);
	}
	
	public void Page_PreRender(object sender, EventArgs e)
	{
		arrFooterTopLinks.ToList().ForEach(item => { item.Value = MapActionKey(item.Value); });		
		rptFooterTopLinks.DataSource = arrFooterTopLinks;
		rptFooterTopLinks.DataBind();

		arrFooterBottomLinks.ToList().ForEach(item => { item.Value = MapActionKey(item.Value); });
		rptFooterBottomLinks.DataSource = arrFooterBottomLinks;
		rptFooterBottomLinks.DataBind();		
		
		string title = "זוזו";
		if (pageCaption.Length > 0)
			title += " - " + pageCaption;
		this.Title = title;

		if (pageIcons.Count > 0)
		{
			List<string> mappedIcons = new List<string>();
			pageIcons.ForEach(icon => mappedIcons.Add(MapZooZooImageFile(icon)));
			rptPageIcons.DataSource = mappedIcons;
			rptPageIcons.DataBind();
			pnlPageIcon.Visible = true;
		}
		
		if (blnHomePage)
		{
			/*
			SportSite.WebSiteServices.ArticleData article = GetPrimaryArticle();
			if (article != null)
			{
				hgcPrimaryArticleCaption.InnerHtml = article.Caption;
				hgcPrimaryArticleSubCaption.InnerHtml = article.SubCaption;
				lnkPrimaryArticleFull.HRef = MapActionKey("Article/" + article.ID);
				lnkPrimaryArticleFull.Visible = true;
				if (article.Images != null && article.Images.Length > 0)
				{
					string imagePath = SportSite.Common.Tools.CheckAndCreateThumbnail(SportSite.Common.Data.AppPath + "/Images/Articles/" + article.Images[0], 400, 0, this.Server, false);
					imgPrimaryArticleBg.Src = imagePath;
					imgPrimaryArticleBg.Visible = true;
				}
			}
			*/

			SportSite.Core.ZooZooManager.Instance.Log(SportSite.Common.ZooZooPage.Homepage, LOG_FILE_LINES);

            List<string> arrEventGalleryImages = new List<string>();
            SportSite.Core.ZooZooManager.Instance.EventGalleryImageAlbums.ForEach(album => arrEventGalleryImages.AddRange(album.Images));
            
            BindZooZooRepeater(rptQuickPoll, SportSite.Core.ZooZooManager.Instance.GetActivePoll().Answers, new RepeaterItemEventHandler(rptQuickPoll_ItemDataBound), pnlHomepageQuickPoll);
            BindZooZooRepeater(rptEventGalleryImages, RandomizeList(arrEventGalleryImages), 
                new RepeaterItemEventHandler(rptEventGalleryImages_ItemDataBound), pnlHomepageEventGallery);

			BindZooZooRepeater(rptHomepageEvents, GetEventsOrStreetGames(false, true), new RepeaterItemEventHandler(rptHomepageEvents_ItemDataBound), pnlHomepageEvents);
            BindZooZooRepeater(rptHomepageStreetGames, GetEventsOrStreetGames(true, true), new RepeaterItemEventHandler(rptHomepageEvents_ItemDataBound), pnlHomepageStreetGames);

			HandlePrimaryAndSubArticles();
		}
		else
		{
			arrHomepageControls.ForEach(c => { c.Visible = false; });
			if (blnShowSideBanner)
				pnlSideBanner.Visible = true;
		}

		WriteJavaScriptVariable("_isHomePage", blnHomePage);
	}

    void BindZooZooRepeater(Repeater repeater, IList list, RepeaterItemEventHandler handler, HtmlGenericControl container)
    {
        if (list.Count > 0)
        {
            repeater.ItemDataBound += handler;
            repeater.DataSource = list;
            repeater.DataBind();
        }
        else
        {
            container.Visible = false;
        }
    }

	private void HandlePrimaryAndSubArticles()
	{
		SportSite.WebSiteServices.ArticleData[] articles = SportSite.Core.ZooZooManager.Instance.GetRawArticles(2);
		SportSite.WebSiteServices.ArticleData primaryArticle = (articles.Length > 0) ? articles[0] : null;
		SportSite.WebSiteServices.ArticleData subArticle = (articles.Length > 1) ? articles[1] : null;
		SetArticleContents(primaryArticle, pnlPrimaryArticle, lblPrimaryArticleMainCaption, lblPrimaryArticleSubCaption, imgPrimaryArticleIcon, lnkPrimaryArticleFull);
		SetArticleContents(subArticle, pnlSubArticle, lblSubArticleMainCaption, lblSubyArticleSubCaption, imgSubArticleIcon, lnkSubArticleFull);
	}

	private void SetArticleContents(SportSite.WebSiteServices.ArticleData article, HtmlGenericControl container, 
		HtmlGenericControl mainCaptionLabel, HtmlGenericControl subCaptionLabel, HtmlImage image, HtmlAnchor link)
	{
		if (article == null)
		{
			container.Visible = false;
		}
		else
		{
			mainCaptionLabel.InnerHtml = article.Caption;
			subCaptionLabel.InnerHtml = article.SubCaption;
			link.HRef = MapActionKey("Article/" + article.ID);
			if (article.Images != null && article.Images.Length > 0)
				image.Src = SportSite.Common.Data.AppPath + "/" + SportSite.Common.Data.ArticlesImagesFolder + "/" + article.Images[0];
			else
				image.Visible = false;
		}
	}
	
	private void CheckForVote()
	{
		if (Request.Form["action"] == "vote" && !string.IsNullOrEmpty(Request.Form["answer"]))
		{
			int answer;
			if (Int32.TryParse(Request.Form["answer"], out answer) && answer >= 0)
			{
				SportSite.Common.ZooZooPoll activePoll = SportSite.Core.ZooZooManager.Instance.GetActivePoll();
				if (answer > 0 && activePoll.Id > 0 && answer <= activePoll.Answers.Count)
				{
					activePoll.UpdateVotes(answer, activePoll.GetVotes(answer) + 1);
					SportSite.Core.ZooZooManager.Instance.UpdatePoll(activePoll);
					activePoll = SportSite.Core.ZooZooManager.Instance.GetActivePoll();

					SportSite.Core.ZooZooManager.Instance.Log(SportSite.Common.ZooZooPage.QuickPollVote, LOG_FILE_LINES);
				}
				else
				{
					SportSite.Core.ZooZooManager.Instance.Log(SportSite.Common.ZooZooPage.QuickPollPeek, LOG_FILE_LINES);
				}
				Response.Clear();
				Response.Write(SportSite.Core.ZooZooManager.Instance.GeneratePollVoteFormFromTemplate(activePoll, PollVotedQuestionTemplate.InnerHtml, PollVotedAnswerTemplate.InnerHtml, 100));
				Response.End();
			}
		}
	}

	private void CheckContactUs()
	{
		if (Request.Form["action"] == "contact" && !string.IsNullOrEmpty(Request.Form["name"]))
		{
			string strName = SportSite.Common.Tools.GetSafeRequestValue("name");
			string strEmail = SportSite.Common.Tools.GetSafeRequestValue("email");
			string strSubject = SportSite.Common.Tools.GetSafeRequestValue("subject");
			string strMessage = GetSafeRequestValueWithLines("message").Replace("&lt;P&gt", "").Replace("&lt;/P&gt", "");
			if (strName.Length > 0 && (strSubject.Length > 0 || strMessage.Length > 0))
			{
				Response.Clear();
				string strError = SportSite.Core.ZooZooManager.Instance.SendContactUsMessage(strName, strEmail, strSubject, strMessage);
				if (strError.Length == 0)
				{
					Response.Write("OK");
				}
				else
				{
					Response.Write(strError);
				}
				Response.End();
			}
		}
	}

	private string GetSafeRequestValueWithLines(string key)
	{
		string[] arrLines = (this.Request.Form[key] + "").Replace("<BR>", "\n").Replace("<br />", "\n").Replace("\r", "").Split('\n');
		for (int i = 0; i < arrLines.Length; i++)
			arrLines[i] = SportSite.Common.Tools.PreventXSS(arrLines[i].Trim());
		return string.Join("<br />", arrLines);
	}
		
    void rptHomepageEvents_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            SportSite.Common.ZooZooEvent eve = (SportSite.Common.ZooZooEvent)e.Item.DataItem;
            Label label;
            if (eve.Title.Length > 0)
            {
                label = e.Item.FindControl("lbEventTitle") as Label;
                if (label != null)
                    label.Text = eve.Title + ", ";
            }
            if (eve.Date.Year > 1900)
            {
                label = e.Item.FindControl("lbEventDate") as Label;
                if (label != null)
                    label.Text = "ב" + eve.HebDayOfWeek + " " + eve.Date.ToString("dd/MM/yyyy");
            }
        }
    }
    
	void rptQuickPoll_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		HtmlGenericControl control;
		switch (e.Item.ItemType)
		{
			case ListItemType.Header:
				control = (HtmlGenericControl)e.Item.FindControl("PollQuestion");
				if (control != null)
					control.InnerHtml = (rptQuickPoll.DataSource as List<SportSite.Common.ZooZooPoll.Answer>)[0].Parent.Question;
				break;
			case ListItemType.Item:
			case ListItemType.AlternatingItem:
				control = (HtmlGenericControl)e.Item.FindControl("AnswerText");
				if (control != null)
					control.InnerHtml = ((SportSite.Common.ZooZooPoll.Answer)e.Item.DataItem).Text;
				control = (HtmlGenericControl)e.Item.FindControl("AnswerContainer");
				if (control != null)
					control.Attributes["answer_id"] = ((SportSite.Common.ZooZooPoll.Answer)e.Item.DataItem).Id.ToString();
				break;				
		}
	}

    void rptEventGalleryImages_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            HtmlImage image = (HtmlImage)e.Item.FindControl("GalleryImage");
            if (image != null)
                image.Src = (string)e.Item.DataItem;
        }
    }

	protected void ShowTwits()
	{
		SportSite.Core.ZooZooManager.Instance.Log(SportSite.Common.ZooZooPage.Twits, LOG_FILE_LINES);
		
		pageCaption = "ציוצים";
		pageIcons.Add("twits_icon.gif");
		selectedPageIndex = 6;
		blnHomePage = false;
		blnShowSideBanner = false;
		pnlTwitsPageContent.Visible = true;
	}
	
	protected void ShowChampionships()
	{
		SportSite.Core.ZooZooManager.Instance.Log(SportSite.Common.ZooZooPage.Championships, LOG_FILE_LINES);
		
		pageCaption = "תחרויות";
		selectedPageIndex = 5;
		blnHomePage = false;
		pnlChampionshipsContent.Visible = true;
		litChampionships.Text = "<h3>העמוד בבניה, בקרו בקרוב כדי לראות תחרויות</h3>";
	}
	
	protected void ShowEvents()
	{
		bool blnStreetGames = action.Equals("streetgames");
		string properCaseActionKey = blnStreetGames ? "StreetGames" : "Events";

		SportSite.Core.ZooZooManager.Instance.Log(blnStreetGames ? SportSite.Common.ZooZooPage.StreetGames : SportSite.Common.ZooZooPage.Events, LOG_FILE_LINES);
		
		DateTime date = GetEventDateFromUrl(), now = DateTime.Now, dtNextMonth = date.AddMonths(1), dtPrevMonth = date.AddMonths(-1);
		List<SportSite.Common.ZooZooEvent> arrEvents = GetEventsOrStreetGames(blnStreetGames, false);
		Dictionary<int, List<SportSite.Common.ZooZooEvent>> arrMonthlyEvents = new Dictionary<int, List<SportSite.Common.ZooZooEvent>>();

		pageCaption = blnStreetGames ? "משחקי רחוב" : "אירועים";
		selectedPageIndex = blnStreetGames ? -1 : 2;
		blnShowSideBanner = false;
		blnHomePage = false;
		pnlEventsContent.Visible = true;
		pageIcons.Add("stars.gif");
		pageIcons.Add(blnStreetGames ? "liloo.gif" : "air_baloon.gif");
		imgEventsOrStreetGamesCaption.Src = MapZooZooImageFile(action.ToLower() + "_caption.gif");
		imgEventsOrStreetGamesCaption.Visible = true;

		if (date.Year != now.Year || date.Month != now.Month)
			lnkResetCalendarDate.HRef = MapActionKey(properCaseActionKey);

		arrEvents.FindAll(eve => eve.Date.Year == date.Year && eve.Date.Month == date.Month).ForEach(eve =>
		{
			int key = eve.Date.Day;
			if (!arrMonthlyEvents.ContainsKey(key))
				arrMonthlyEvents.Add(key, new List<SportSite.Common.ZooZooEvent>());
			arrMonthlyEvents[key].Add(eve);
		});

		GenerateClientSideEventArray(arrMonthlyEvents);
		
		pnlEventCalendarDate.InnerHtml = GetMonthAndYearCaption(date, true);
		lnkEventCalendarNextMonth.HRef = BuildEventCalendarNextPrevUrl(dtNextMonth, properCaseActionKey);
		lnkEventCalendarNextMonth.Title = GetMonthAndYearCaption(dtNextMonth);
		lnkEventCalendarPrevMonth.HRef = BuildEventCalendarNextPrevUrl(dtPrevMonth, properCaseActionKey);
		lnkEventCalendarPrevMonth.Title = GetMonthAndYearCaption(dtPrevMonth);
		lnkEventCalendarNavigation.HRef = MapActionKey(properCaseActionKey + "/$month$year");

		ddlSelectMonth.Items.Clear();
		for (int i = 1; i <= 12; i++)
			ddlSelectMonth.Items.Add(new ListItem(Sport.Common.Tools.HebMonthName(i), i.ToString()));
		ddlSelectMonth.SelectedIndex = date.Month - 1;
		ddlSelectYear.Items.Clear();
		for (int i = date.Year - 5; i <= date.Year + 5; i++)
			ddlSelectYear.Items.Add(i.ToString());
		ddlSelectYear.SelectedIndex = 5;

		int cellWidth = 86, headerHeight = 38, cellHeight = 91, cellPadding = 2, curLeft = 0;		
		string[] arrHebHeaders = new string[] { "שבת", "ו", "ה", "ד", "ג", "ב", "א" };
		List<EventCalendarHeaderData> arrHeaderData = new List<EventCalendarHeaderData>();
		for (int i = 0; i < arrHebHeaders.Length; i++)
		{
			arrHeaderData.Add(new EventCalendarHeaderData(arrHebHeaders[i], curLeft));
			if (i < (arrHebHeaders.Length - 1))
				curLeft += (cellWidth + cellPadding);
		}

		rptEventCalendarHeaders.ItemDataBound += new RepeaterItemEventHandler(rptEventCalendarHeaders_ItemDataBound);
		rptEventCalendarHeaders.DataSource = arrHeaderData;
		rptEventCalendarHeaders.DataBind();

		List<EventCalendarDayData> arrDayData = new List<EventCalendarDayData>();
		string strDay, extraClass, strDescrption, firstHebWeekDay = GetFirstHebrewDayOfWeek(date); ;		
		int curTop = headerHeight + cellPadding, totalLeft = curLeft, day = 1, totalDaysCount = GetDaysInMonth(date);
		bool blnRangeIsActive = false, blnCurrentMonth = (date.Month.Equals(now.Month) && date.Year.Equals(now.Year));
		for (int row = 0; row < 6; row++)
		{
			if (row >= 5 && !blnRangeIsActive)
				break;
			for (int weekDay = 0; weekDay < 7; weekDay++)
			{
				if (row == 0 && firstHebWeekDay.Equals(arrHebHeaders[6 - weekDay].Substring(0, 1)))
					blnRangeIsActive = true;
				if (day > totalDaysCount)
					blnRangeIsActive = false;
				strDay = string.Empty;
				extraClass = string.Empty;
				strDescrption = string.Empty;
				if (blnRangeIsActive)
				{
					extraClass = (blnCurrentMonth && day.Equals(date.Day)) ? " eventcalendar_cell_today" : (arrMonthlyEvents.ContainsKey(day) ? " eventcalendar_cell_nonempty" : "");
					if (arrMonthlyEvents.ContainsKey(day))
						strDescrption = "<span class=\"ec_inner_text\">" + arrMonthlyEvents[day][0].Description + "</span>";
					strDay = day.ToString();
					day++;
				}
				arrDayData.Add(new EventCalendarDayData { Left = curLeft, Top = curTop, Day = strDay, Description = strDescrption, ExtraClass = extraClass });				
				curLeft -= (cellWidth + cellPadding);
			}
			curLeft = totalLeft;
			curTop += cellHeight + cellPadding;
		}
		rptEventCalendarDays.DataSource = arrDayData;
		rptEventCalendarDays.DataBind();
	}

	void ShowWeeklyGames()
	{
		SportSite.Core.ZooZooManager.Instance.Log(SportSite.Common.ZooZooPage.WeeklyGame, LOG_FILE_LINES);
		
		pageCaption = "משחק השבוע";
		selectedPageIndex = 3;
		blnHomePage = false;
		blnShowSideBanner = true;
		pnlWeeklyGamePageContents.Visible = true;

		string strPicName = SportSite.Core.ZooZooManager.Instance.WeeklyGamePicture;
		if (strPicName.Length > 0)
		{
			imgWeeklyGame.Src = strPicName;
			imgWeeklyGame.Visible = true;
		}
	}

	void ShowNewsPaper()
	{
		SportSite.Core.ZooZooManager.Instance.Log(SportSite.Common.ZooZooPage.Newspaper, LOG_FILE_LINES);
		
		pageCaption = "כתבו עלינו";
		selectedPageIndex = 4;
		blnHomePage = false;
		blnShowSideBanner = true;
		pnlNewspaperPageContents.Visible = true;

		string strPicName = SportSite.Core.ZooZooManager.Instance.WroteOnUsPicture;
		if (strPicName.Length > 0)
		{
			imgWroteOnUs.Src = strPicName;
			imgWroteOnUs.Visible = true;
		}		
	}

	void ShowContactUs()
	{
		SportSite.Core.ZooZooManager.Instance.Log(SportSite.Common.ZooZooPage.ContactUs, LOG_FILE_LINES);

		pageCaption = "צור קשר";
		selectedPageIndex = 7;
		pageIcons.Add("contactus_icon.gif");
		blnHomePage = false;
		blnShowSideBanner = false;
		pnlContactUs.Visible = true;
	}

	void ShowEventGallery()
	{
		SportSite.Core.ZooZooManager.Instance.Log(SportSite.Common.ZooZooPage.EventGallery, LOG_FILE_LINES);

		pageCaption = "גלריית אירועים";
		pageIcons.Add("event_gallery_icon.gif");
		blnHomePage = false;
		blnShowSideBanner = false;
		pnlEventGallery.Visible = true;
	}
			
	void rptEventCalendarHeaders_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
		{
			HtmlGenericControl control = (HtmlGenericControl)e.Item.FindControl("ecHeader");
			if (control != null)
			{
				EventCalendarHeaderData data = (EventCalendarHeaderData)e.Item.DataItem;
				control.Style["left"] = data.Left + "px";
				control.InnerHtml = data.Text;
			}
		}
	}
	
	protected void ShowAboutContents()
	{
		SportSite.Core.ZooZooManager.Instance.Log(SportSite.Common.ZooZooPage.About, LOG_FILE_LINES);
		
		pageCaption = "אודות";
		pageIcons.Add("about_page_icon.gif");
		selectedPageIndex = 1;
		blnHomePage = false;
		pnlAboutPageContents.Visible = true;
		litAbout.Text = SportSite.Core.ZooZooManager.Instance.AboutText;
	}
	
	protected void HandleArticles(string rawID)
	{
		blnHomePage = false;
		pnlArticlePageContents.Visible = true;
		//selectedPageIndex = 4;

		SportSite.Core.ZooZooManager.Instance.Log(SportSite.Common.ZooZooPage.Articles, LOG_FILE_LINES);
		
		SportSite.WebSiteServices.ArticleData article = null;
		if (!rawID.Equals("all", StringComparison.CurrentCultureIgnoreCase))
		{
			int id;
			if (Int32.TryParse(rawID, out id) && id > 0 && SportSite.Core.ZooZooManager.Instance.IsZooZooArticle(id))
			{
				//Response.Redirect("http://www.schoolsport.co.il/Main.aspx?action=ShowArticle&id=" + id, true);
				SportSite.WebSiteServices.WebsiteService service = new SportSite.WebSiteServices.WebsiteService();
				article = service.GetArticleData(id);
			}
		}

		if (article != null && article.ID > 0)
		{
			ShowSingleArticle(article);
		}
		else
		{
			ShowAllArticles();
		}
	}

	protected void ShowSingleArticle(SportSite.WebSiteServices.ArticleData article)
	{
		pnlSingleArticle.Visible = true;
		pageIcons.Add("article_page_icon.gif");
		//blnShowSideBanner = false;
		pageCaption = article.Caption;
		hgcArticleCaption.InnerHtml = article.Caption;
		hgcArticleSubCaption.InnerHtml = article.SubCaption;
		litArticleSubmissionDate.Text = article.Time.ToString("dd/MM/yyyy");
		litArticleSubmissionTime.Text = article.Time.ToString("HH:mm");
		litArticleBody.Text = SportSite.Common.Tools.KeepOriginalFormat(article.Contents);

		//images
		if (article.Images != null && article.Images.Length > 0)
		{
			int maxImagesWidth = 300;
			pnlArticleBodyImages.InnerHtml = BuildArticleImagesHTML(article, maxImagesWidth);

			imgArticleMainImage.Src = MapArticleImage(article.Images[0]);
			if (article.ImageDescriptions != null && article.ImageDescriptions.Length > 0)
			{
				imgArticleMainImage.Alt = article.ImageDescriptions[0];
				imgArticleMainImage.Attributes["title"] = article.ImageDescriptions[0];
			}
		}
		else
		{
			imgArticleMainImage.Src = "/images/default_article_image_new.JPG";
		}
		
		//attachments
		if (article.Attachments != null && article.Attachments.Length > 0)
		{
			List<SportSite.Common.ZooZooAttachment> arrAttachments = article.Attachments.ToList().ConvertAll(data =>
			{
				SportSite.Core.AttachmentData coreData = new SportSite.Core.AttachmentData(data, this.Server);
				return new SportSite.Common.ZooZooAttachment(coreData);
			});
			rptArticleAttachments.DataSource = arrAttachments;
			rptArticleAttachments.ItemDataBound += new RepeaterItemEventHandler(rptArticleAttachments_ItemDataBound);
			rptArticleAttachments.DataBind();
		}
		else
		{
			rptArticleAttachments.Visible = false;
		}
	}

	void rptArticleAttachments_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
		{
			Image image = e.Item.FindControl("imgArticleIcon") as Image;
			if (image != null)
			{
				SportSite.Common.ZooZooAttachment attachment = (SportSite.Common.ZooZooAttachment)e.Item.DataItem;
				if (attachment.IconUrl.Length > 0)
				{
					image.ImageUrl = attachment.IconUrl;
					image.ToolTip = attachment.FileType;
				}
				else
				{
					image.Visible = false;
				}
			}
		}
	}
	
	protected void ShowAllArticles()
	{
		pnlAllArticles.Visible = true;
		pageCaption = "כתבות";
		List<SportSite.Common.ZooZooArticle> articles = new List<SportSite.Common.ZooZooArticle>();
		SportSite.WebSiteServices.WebsiteService service = new SportSite.WebSiteServices.WebsiteService();
		SportSite.Core.ZooZooManager.Instance.Articles.ToList().ForEach(id =>
		{
			SportSite.WebSiteServices.ArticleData article = service.GetArticleData(id);
			if (article != null && article.ID == id)
				articles.Add(new SportSite.Common.ZooZooArticle(article));
		});

		articles.Sort((a1, a2) => { return a2.SubmissionDate.CompareTo(a1.SubmissionDate); });
		articles.ForEach(a => { a.Url = MapActionKey("Article/" + a.ID); });
		
		rptArticles.DataSource = articles;
		rptArticles.DataBind();		
	}

	#region helpers
	public void WriteJavaScriptVariable(string name, string value)
	{
		string key = "js_var_" + name;
		if (!Page.ClientScript.IsClientScriptBlockRegistered(this.GetType(), key))
			Page.ClientScript.RegisterClientScriptBlock(this.GetType(), key, string.Format("var {0} = {1}; ", name, value), true);
	}

	public string GetFirstHebrewDayOfWeek(DateTime date)
	{
		System.Globalization.CultureInfo hebCulture = System.Globalization.CultureInfo.CreateSpecificCulture("he-IL");
		hebCulture.DateTimeFormat.Calendar = new System.Globalization.HebrewCalendar();
		date = new DateTime(date.Year, date.Month, 1);
		return date.ToString("ddd", hebCulture);
	}

	public int GetDaysInMonth(DateTime date)
	{
		DateTime dtMonthStart = new DateTime(date.Year, date.Month, 1);
		DateTime dtMonthEnd = dtMonthStart.AddMonths(1);
		return (int)(dtMonthEnd - dtMonthStart).TotalDays;
	}

	public string BuildEventCalendarNextPrevUrl(DateTime date, string actionKey)
	{
		return MapActionKey(string.Format("{0}/{1}{2}", actionKey, date.Month.ToString().PadLeft(2, '0'), date.Year));		
	}
	
	public void WriteJavaScriptVariable(string name, bool value)
	{
		WriteJavaScriptVariable(name, value.ToString().ToLower());
	}
	
	public SportSite.WebSiteServices.ArticleData GetPrimaryArticle()
	{
		List<SportSite.WebSiteServices.ArticleData> arrZooZooArticles = new List<SportSite.WebSiteServices.ArticleData>();
		SportSite.WebSiteServices.WebsiteService service = new SportSite.WebSiteServices.WebsiteService();
		SportSite.Core.ZooZooManager.Instance.Articles.ToList().ForEach(id =>
		{
			SportSite.WebSiteServices.ArticleData article = service.GetArticleData(id);
			if (article != null && article.ID == id)
				arrZooZooArticles.Add(article);
		});

		if (arrZooZooArticles.Count > 0)
		{
			arrZooZooArticles.Sort((a1, a2) => { return a2.Time.CompareTo(a1.Time); });
			return arrZooZooArticles[0];
		}

		return null;
	}

	public string BuildArticleImagesHTML(SportSite.WebSiteServices.ArticleData article, int maxWidth)
	{
		StringBuilder sb = new StringBuilder();
		for (int i = 0; i < article.Images.Length; i++)
		{
			string imageName = article.Images[i];
			sb.AppendFormat("<img src=\"{0}\" max_width=\"{1}\" alt=\"תמונת כתבה\" title=\"מעבר לתמונה בגודל מלא\" style=\"cursor: pointer;\" onclick=\"window.open(this.src, '_blank').focus();\" />", 
				MapArticleImage(imageName), maxWidth);
			if (article.ImageDescriptions != null && i < article.ImageDescriptions.Length && !string.IsNullOrEmpty(article.ImageDescriptions[i]))
				sb.Append("<br />").Append(article.ImageDescriptions[i]);
			sb.Append("<br /><br />");
		}
		return sb.ToString();
	}

	public string MapArticleImage(string imageName)
	{
		return string.Format("{0}/Images/Articles/{1}", SportSite.Common.Data.AppPath, imageName);
	}
	
	public string CollectionToString(NameValueCollection collection, string separator)
	{
		List<string> data = new List<string>();
		foreach (string key in collection.Keys)
			data.Add(key + ": " + collection[key]);
		return string.Join(separator, data.ToArray());
	}

	public string BuildActionLink(string text, string actionKey, int index, bool disabled)
	{
		string retVal = string.Format("<div class=\"ActionLinkPanel ActionLinkPanel{0}_{1}\"><a href=\"{2}\"",
			((index == selectedPageIndex) ? "_active" : ""), index, MapActionKey(actionKey));
		if (disabled)
			retVal += " onclick=\"return false;\"";
		retVal += string.Format(">{0}</a></div><div class=\"link_separator\"></div>", text);
		return retVal;
	}

	public string BuildActionLink(string text, string actionKey, int index)
	{
		return BuildActionLink(text, actionKey, index, false);
	}

	public string MapActionKey(string key)
	{
		if (Request.ServerVariables["Script_Name"].IndexOf("/ZooZoo/", StringComparison.CurrentCultureIgnoreCase) >= 0)
			return "?" + key.Replace("/", "=").ToLower(); //ParseActionParams(key).ToLower();
		
		if (currentFolderVirtualPath.Length == 0)
		{
			List<string> temp = new List<string>(Request.ServerVariables["Script_Name"].Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries));
			temp.RemoveAt(temp.Count - 1);
			currentFolderVirtualPath = "/" + string.Join("/", temp.ToArray());
		}

		string retVal = currentFolderVirtualPath;
		if (retVal.EndsWith("/"))
			retVal = retVal.Substring(0, retVal.Length - 1);
		retVal += "/" + key + "/";
		return retVal;
	}

    public List<string> RandomizeList(List<string> list, Random rnd)
    {
        list = list.OrderBy(x => rnd.Next()).ToList();
        return list;     
    }

    public List<string> RandomizeList(List<string> list)
    {
        return RandomizeList(list, new Random());
    }

	public void GenerateClientSideEventArray(Dictionary<int, List<SportSite.Common.ZooZooEvent>> events)
	{
		StringBuilder sb = new StringBuilder();
		sb.Append("var arrEventCalendarData = { ");
		List<string> items = new List<string>();
		events.Keys.ToList().ForEach(day =>
		{
			items.Add(string.Format("\"{0}\": [\"{1}\"]", day.ToString(), string.Join("\", \"", events[day].ConvertAll(eve => SportSite.Common.Tools.KeepOriginalFormat(eve.Description).Replace("\"", "\\\"")).ToArray())));
		});
		sb.Append(string.Join(", ", items.ToArray()));
		sb.Append(" }; ");
		this.ClientScript.RegisterClientScriptBlock(this.Page.GetType(), "events_array", sb.ToString(), true);
	}

	public List<SportSite.Common.ZooZooEvent> GetEventsOrStreetGames(bool blnStreetGames, bool blnOnlyNew)
	{
		List<SportSite.Common.ZooZooEvent> events = SportSite.Core.ZooZooManager.Instance.GetEventsOrStreetGames(blnStreetGames).ToList();
		if (blnOnlyNew)
		{
			DateTime today = DateTime.Now.Date;
			events.RemoveAll(eve => eve.Date < today);
		}
		return events;
	}
	
	public DateTime GetEventDateFromUrl()
	{
		string rawValue = Request.QueryString[action.ToLower()] + "";
		if (rawValue.Length == 6)
		{
			string strMonth = rawValue.Substring(0, 2);
			string strYear = rawValue.Substring(2);
			int month, year;
			if (Int32.TryParse(strMonth, out month) && Int32.TryParse(strYear, out year) && month > 0 && month <= 31 && year > 1900)
				return new DateTime(year, month, DateTime.Now.Day);
		}
		return DateTime.Now;
	}

	public string GetMonthAndYearCaption(DateTime date, bool makeLink)
	{
		string strHtml = makeLink ? "<a class=\"makefancyzoom\" href=\"#select_month\">" : "";
		strHtml += Sport.Common.Tools.HebMonthName(date.Month);
		strHtml += makeLink ? "</a>" : "";
		strHtml += makeLink ? "&nbsp;&nbsp;&nbsp;" : " ";
		strHtml += makeLink ? "<a class=\"makefancyzoom\" href=\"#select_year\">" : "";
		strHtml += date.Year.ToString();
		strHtml += makeLink ? "</a>" : "";
		return strHtml;
	}

	public string GetMonthAndYearCaption(DateTime date)
	{
		return GetMonthAndYearCaption(date, false);
	}
	
	public string ParseActionParams(string actionKey)
	{
		if (actionKey.IndexOf("/") >= 0)
			return actionKey.Replace("/", "=");
		
		string retVal = string.Empty;
		bool blnDigitFound = false;
		for (int i = 0; i < actionKey.Length; i++)
		{
			char c = actionKey[i];
			if (!blnDigitFound && char.IsDigit(c))
			{
				retVal += "=";
				blnDigitFound = true;
			}
			retVal += c;
		}
		return retVal;
	}

	public string MapZooZooImageFile(string fileName)
	{
		return SportSite.Common.Data.AppPath + "/ZooZoo/Images/" + fileName;
	}
	#endregion

	/*
	public void SaveRemoteFile(object sender, EventArgs e)
	{
		string url = TextBox1.Text;
		string error;
		SportSite.Common.Tools.SaveRemoteUrl(url, Server.MapPath("/ZooZoo"), out error);
		if (error.Length > 0)
			TextBox1.Text = "error: " + error;
		else
			TextBox1.Text = "success";
	}
	*/

	public class EventCalendarHeaderData
	{
		public string Text { get; set; }
		public int Left { get; set; }

		public EventCalendarHeaderData()
		{
			this.Text = string.Empty;
			this.Left = 0;
		}
		
		public EventCalendarHeaderData(string text, int left)
			: this()
		{
			this.Text = text;
			this.Left = left;
		}
	}

	public class EventCalendarDayData
	{
		public int Left { get; set; }
		public int Top { get; set; }
		public string Day { get; set; }
		public string ExtraClass { get; set; }
		public string Description { get; set; }

		public EventCalendarDayData()
		{
			this.Left = 0;
			this.Top = 0;
			this.Day = string.Empty;
			this.ExtraClass = string.Empty;
			this.Description = string.Empty;
		}
	}
	#endregion
</script>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html id="HtmlTag" runat="server" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<meta http-equiv="Content-Type" content="text/html; charset=windows-1255" />
	<script type="text/javascript" src="/ZooZoo/jquery-1.4.3.min.js"></script>
	<script type="text/javascript" src="/ZooZoo/jquery.color.js"></script>
	<script type="text/javascript" src="/Common/s3Slider.js"></script>
	<script type="text/javascript" src="/Common/swfobject.js"></script>
	<script type="text/javascript" src="/Common/fancyzoom.js"></script>
	<script type="text/javascript" src="/Common/gritter/js/jquery.gritter.js"></script>
	<link rel="stylesheet" href="/Common/gritter/css/jquery.gritter.css" />
		<!-- script type="text/javascript" src="/ZooZoo/jquery.animation-fix.js"></script -->
	<script type="text/javascript" src="/ZooZoo/ZooZoo.js"></script>
	<link rel="Stylesheet" href="/ZooZoo/ZooZoo.css" media="all" />
	<script src="/Common/orbit/jquery.orbit.min.js" type="text/javascript"></script>
	<link rel="stylesheet" href="/Common/orbit/orbit.css" />
</head>
<body> <!--  onload="WindowLoad(event);" -->
    <form id="form1" runat="server">
	<div style="text-align: center; padding: 10px 10px 10px 10px;">
	<center>
		<div id="top_banner" class="homepage_banner"></div>
	    <div id="MainPanel" class="MainPanel">
			<div id="LogoPlaceHolder">
				<a href="<%=MapActionKey("Main") %>"><img src="<%=MapZooZooImageFile("Logo_Main_2.gif")%>" alt="לוגו" title="עמוד הבית" /></a>
			</div>
			<div id="RightSideBar_Top" class="RightSideBar">
				<%=BuildActionLink("אודות", "About", 1) %>
				<%=BuildActionLink("אירועים", "Events", 2) %>
				<%=BuildActionLink("משחק השבוע", "WeeklyGame", 3) %>
				<%=BuildActionLink("כתבו עלינו", "Newspaper", 4) %>
				<%=BuildActionLink("תחרויות", "Championships", 5, true) %>
				<%=BuildActionLink("ציוצים", "Twits", 6) %>
				<%=BuildActionLink("צור קשר", "ContactUs", 7) %>
				<div runat="server" id="pnlSideBanner" visible="false">
					<div id="side_banner" class="side_banner" style="margin-top: 20px;"></div>
				</div>
				<div runat="server" id="pnlPageIcon" class="page_icon" visible="false">
					<asp:Repeater ID="rptPageIcons" runat="server">
						<ItemTemplate><div class="icon_image_placeholder"><img alt="page icon" title="" align="middle" src="<%# Container.DataItem %>" /></div></ItemTemplate>
					</asp:Repeater>
				</div>
			</div>
			<div id="BodyContentsPanel_Top" class="BodyContentsPanel">
				<div class="hp_middle_content" style="margin-bottom: 20px;">
					<!-- Sport Movies Competition -->
					<div runat="server" id="pnlSportMoviesCompetition">
						<div class="SportMoviesCaption"></div>
						<a href="http://www.flix.co.il/tapuz/channel.asp?c=386" target="_blank"><img src="/ZooZoo/images/tv_play.gif" border="0" alt="טלויזיה" title="מעבר לתחרות סרטי ספורט" /></a>
					</div>
					<!-- End Sport Movies Competition -->
					<asp:Panel ID="pnlChampionshipsContent" runat="server" Visible="false">
						<h1>תחרויות</h1>
						<asp:Literal ID="litChampionships" runat="server"></asp:Literal>
					</asp:Panel>
					<asp:Panel ID="pnlEventsContent" runat="server" Visible="false">
						<!-- Events page -->
                        <div id="ec_inner_text_box" style="display: none;"></div>
						<div><a id="lnkResetCalendarDate" runat="server"><img id="imgEventsOrStreetGamesCaption" runat="server" visible="false" alt="caption" title="" /></a></div>
						<div class="eventcalendar_placeholder">
							<div id="pnlEventCalendarDate" runat="server" class="eventcalendar_date_placeholder"></div>
							<a id="lnkEventCalendarNextMonth" runat="server" class="eventcalendar_next_month"></a>
							<a id="lnkEventCalendarPrevMonth" runat="server" class="eventcalendar_prev_month"></a>
							<div class="eventcalendar_inner_contents">
								<asp:Repeater ID="rptEventCalendarHeaders" runat="server">
									<ItemTemplate><div id="ecHeader" runat="server" class="eventcalendar_header"></div></ItemTemplate>
								</asp:Repeater>
								<asp:Repeater ID="rptEventCalendarDays" runat="server">
									<ItemTemplate><div class="eventcalendar_cell<%# DataBinder.Eval(Container.DataItem, "ExtraClass")%>" style="left: <%# DataBinder.Eval(Container.DataItem, "Left")%>px; top: <%# DataBinder.Eval(Container.DataItem, "Top")%>px;"><span class="ec_date"><%# DataBinder.Eval(Container.DataItem, "Day")%></span><br /><%# DataBinder.Eval(Container.DataItem, "Description")%></div></ItemTemplate>
								</asp:Repeater>
							</div>
						</div>
					</asp:Panel>
					<asp:Panel ID="pnlAboutPageContents" runat="server" Visible="false">
						<!-- About page -->
                        <div><img src="/ZooZoo/images/about_caption_2.gif" alt="about caption" title="" /></div>
                        <div class="AboutPlaceholder">
                            <div class="AboutInnerContents">
                                <asp:Literal ID="litAbout" runat="server"></asp:Literal>
                            </div>
							<img src="<%=MapZooZooImageFile("about_inner_background.jpg")%>" class="AboutBackgroundImage" alt="background" title="" onerror="this.style.display = 'none';" />
                        </div>
					</asp:Panel>
					<asp:Panel ID="pnlWeeklyGamePageContents" runat="server" Visible="false">
						<!-- Weekly Game page -->
						<div><img src="/ZooZoo/images/weeklygame_caption.gif" alt="weeklygame caption" title="" /></div>
						<div class="weeklygame_placeholder">
							<div class="weeklygame_contents"><img id="imgWeeklyGame" runat="server" visible="false" class="WeeklyGameImage"  /></div>
						</div>
						<div style="text-align: right; margin-top: 25px; margin-right: 25px;" align="right"><img src="/ZooZoo/Images/weeklygame_icon.gif" alt="icon" title="" /></div>
					</asp:Panel>
					<asp:Panel ID="pnlNewspaperPageContents" runat="server" Visible="false">
						<!-- Wrote On Us page -->
						<div><img src="/ZooZoo/images/newspaper_caption.gif" alt="newspaper caption" title="" /></div>
						<div class="newspaper_placeholder">
							<div class="newspaper_contents"><img id="imgWroteOnUs" runat="server" visible="false" class="WroteOnUsImage"  /></div>
						</div>
						<div style="text-align: right; margin-top: 25px; margin-right: 25px;" align="right"><img src="/ZooZoo/Images/newspaper_icon.gif" alt="icon" title="" /></div>
					</asp:Panel>
					<asp:Panel ID="pnlTwitsPageContent" runat="server" Visible="false">
						<!-- Twits page -->
						<div><img src="/ZooZoo/images/twits_caption.gif" alt="twits caption" title="" /></div>
						<div class="twits_placeholder">
							<div class="twits_contents">עמוד זה בבנייה, בקרו שוב בקרוב!</div>
						</div>
					</asp:Panel>
					<asp:Panel ID="pnlContactUs" runat="server" Visible="false">
						<!-- Contact Us page -->
						<div><img src="/ZooZoo/images/contactus_caption.gif" alt="contact us caption" title="" /></div>
						<div class="contactus_placeholder">
							<div class="contactus_inner_div contactus_input" id="contactus_input_name" contenteditable="true"></div>
							<div class="contactus_inner_div contactus_input" id="contactus_input_email" contenteditable="true"></div>
							<div class="contactus_inner_div contactus_input" id="contactus_input_subject" contenteditable="true"></div>
							<div class="contactus_inner_div contactus_input" id="contactus_input_message" contenteditable="true"></div>
							<div class="contactus_inner_div" id="contactus_submit_button"></div>
						</div>
					</asp:Panel>
					<asp:Panel ID="pnlEventGallery" runat="server" Visible="false">
						<!-- Event Gallery page -->
						<div style="margin-bottom: 20px;"><img src="/ZooZoo/images/event_gallery_caption.gif" alt="event gallery caption" title="" /></div>

						<div style="width: 685px;">
							<div class="EventGalleryAlbumCoverAndTitle EventGalleryAlbumCoverAndTitle_Blue">
								<div class="EventGalleryAlbumTitle EventGalleryAlbumTitle_Blue">כותרת אלבום כחול ראשון</div>
								<div class="EventGalleryAlbumCover EventGalleryAlbumCover_Blue">
									<img src="/ZooZoo/Images/EventGallery/IMG_4327.JPG" />
									<div class="EventGalleryAlbumCoverDescription">תיאור של אלבום כחול ראשון מגיע כאן, יכול להיות שתי שורות</div>
								</div>
							</div>
							<div class="EventGalleryAlbumCoverAndTitle EventGalleryAlbumCoverAndTitle_Yellow">
								<div class="EventGalleryAlbumTitle EventGalleryAlbumTitle_Yellow">כותרת אלבום צהוב ראשון</div>
								<div class="EventGalleryAlbumCover EventGalleryAlbumCover_Yellow">
									<img src="/ZooZoo/Images/EventGallery/IMG_4327.JPG" />
									<div class="EventGalleryAlbumCoverDescription">תיאור של אלבום צהוב ראשון מגיע כאן</div>
								</div>
							</div>
							<div class="EventGalleryAlbumCoverAndTitle EventGalleryAlbumCoverAndTitle_Red">
								<div class="EventGalleryAlbumTitle EventGalleryAlbumTitle_Red">כותרת אלבום אדום ראשון</div>
								<div class="EventGalleryAlbumCover EventGalleryAlbumCover_Red">
									<img src="/ZooZoo/Images/EventGallery/IMG_4327.JPG" />
									<div class="EventGalleryAlbumCoverDescription">תיאור של אלבום אדום ראשון מגיע כאן אפשר עד שתי שורות</div>
								</div>
							</div>
							<div style="clear: both;"></div>

							<div class="EventGalleryAlbumCoverAndTitle EventGalleryAlbumCoverAndTitle_Blue">
								<div class="EventGalleryAlbumTitle EventGalleryAlbumTitle_Blue">כותרת אלבום כחול שני</div>
								<div class="EventGalleryAlbumCover EventGalleryAlbumCover_Blue">
									<img src="/ZooZoo/Images/EventGallery/IMG_4327.JPG" />
									<div class="EventGalleryAlbumCoverDescription">תיאור של אלבום כחול שני מגיע כאן, יכול להיות שתי שורות</div>
								</div>
							</div>
							<div class="EventGalleryAlbumCoverAndTitle EventGalleryAlbumCoverAndTitle_Yellow">
								<div class="EventGalleryAlbumTitle EventGalleryAlbumTitle_Yellow">כותרת אלבום צהוב שני</div>
								<div class="EventGalleryAlbumCover EventGalleryAlbumCover_Yellow">
									<img src="/ZooZoo/Images/EventGallery/IMG_4327.JPG" />
									<div class="EventGalleryAlbumCoverDescription">תיאור של אלבום צהוב שני מגיע כאן</div>
								</div>
							</div>
							<div class="EventGalleryAlbumCoverAndTitle EventGalleryAlbumCoverAndTitle_Red">
								<div class="EventGalleryAlbumTitle EventGalleryAlbumTitle_Red">כותרת אלבום אדום שני</div>
								<div class="EventGalleryAlbumCover EventGalleryAlbumCover_Red">
									<img src="/ZooZoo/Images/EventGallery/IMG_4327.JPG" />
									<div class="EventGalleryAlbumCoverDescription">תיאור של אלבום אדום שני מגיע כאן אפשר עד שתי שורות</div>
								</div>
							</div>
							<div style="clear: both;"></div>

							<div class="EventGalleryAlbumCoverAndTitle EventGalleryAlbumCoverAndTitle_Blue">
								<div class="EventGalleryAlbumTitle EventGalleryAlbumTitle_Blue">כותרת אלבום כחול שלישי</div>
								<div class="EventGalleryAlbumCover EventGalleryAlbumCover_Blue">
									<img src="/ZooZoo/Images/EventGallery/IMG_4327.JPG" />
									<div class="EventGalleryAlbumCoverDescription">תיאור של אלבום כחול שלישי מגיע כאן, יכול להיות שתי שורות</div>
								</div>
							</div>
							<div class="EventGalleryAlbumCoverAndTitle EventGalleryAlbumCoverAndTitle_Yellow">
								<div class="EventGalleryAlbumTitle EventGalleryAlbumTitle_Yellow">כותרת אלבום צהוב שלישי</div>
								<div class="EventGalleryAlbumCover EventGalleryAlbumCover_Yellow">
									<img src="/ZooZoo/Images/EventGallery/IMG_4327.JPG" />
									<div class="EventGalleryAlbumCoverDescription">תיאור של אלבום צהוב שלישי מגיע כאן</div>
								</div>
							</div>
							<div class="EventGalleryAlbumCoverAndTitle EventGalleryAlbumCoverAndTitle_Red">
								<div class="EventGalleryAlbumTitle EventGalleryAlbumTitle_Red">כותרת אלבום אדום שלישי</div>
								<div class="EventGalleryAlbumCover EventGalleryAlbumCover_Red">
									<img src="/ZooZoo/Images/EventGallery/IMG_4327.JPG" />
									<div class="EventGalleryAlbumCoverDescription">תיאור של אלבום אדום שלישי מגיע כאן אפשר עד שתי שורות</div>
								</div>
							</div>
							<div style="clear: both;"></div>
						</div>
					</asp:Panel>

					<asp:Panel ID="pnlArticlePageContents" runat="server" Visible="false">
						<!-- Articles page -->
						<div style="margin-bottom: 10px;"><img src="/ZooZoo/images/articles_caption.gif" alt="articles caption" title="" /></div>
						<asp:Panel ID="pnlAllArticles" runat="server" Visible="false">
							<div class="ArticlePlaceholder_All">
								<div class="ArticleContents_All">
									<asp:Repeater ID="rptArticles" runat="server">
										<ItemTemplate>
											<h2><a href="<%# DataBinder.Eval(Container.DataItem, "Url")%>" title="מעבר לכתבה מלאה"><%# DataBinder.Eval(Container.DataItem, "Caption")%></a></h2>
											<h3><%# DataBinder.Eval(Container.DataItem, "SubCaption")%></h3>
											<div class="ArticleBody"><%# DataBinder.Eval(Container.DataItem, "BodyFormatted")%></div>
											<div style="font-style: italic;">פורסם <%# DataBinder.Eval(Container.DataItem, "SubmissionDateFormatted")%></div>
										</ItemTemplate>
										<SeparatorTemplate><div class="ArticleSeparator"></div></SeparatorTemplate>
									</asp:Repeater>
								</div>
							</div>
						</asp:Panel>
						<asp:Panel ID="pnlSingleArticle" runat="server" Visible="false">
							<div class="ArticlePlaceholder_Single">
								<div class="ArticleContents_Single">
									<div id="hgcArticleCaption" class="ArticleCaption" runat="server"></div>
									<!-- <div style="font-style: italic;">פורסם בתאריך <asp:Literal ID="litArticleSubmissionDate" runat="server" /> בשעה <asp:Literal ID="litArticleSubmissionTime" runat="server" /></div> -->
									<div id="hgcArticleSubCaption" class="ArticleSubCaption" runat="server"></div>
									<div class="ArticleBody">
										<div><asp:Literal ID="litArticleBody" runat="server" /></div>
										<div id="pnlArticleBodyImages" runat="server" class="ArticleBodyImages"></div>
										<div class="ArticleAttachment">
											<asp:Repeater ID="rptArticleAttachments" runat="server">
												<HeaderTemplate><hr /></HeaderTemplate>
												<ItemTemplate><p><asp:Image id="imgArticleIcon" runat="server" AlternateText="סוג קובץ" BorderWidth="0" CssClass="ArticleAttachmentIcon" /><a href="<%# DataBinder.Eval(Container.DataItem, "FileUrl")%>" title="<%# DataBinder.Eval(Container.DataItem, "Title")%>" target="_blank"><%# DataBinder.Eval(Container.DataItem, "FileName")%></a></p></ItemTemplate>
											</asp:Repeater>
										</div>
									</div>
								</div>
								<img id="imgArticleMainImage" runat="server" class="ArticleImage_Single" />
							</div>
						</asp:Panel>
					</asp:Panel>
				</div> <!-- right div -->
				<div runat="server" id="pnlZooZooBlog" class="hp_left_content" style="display: none;">
					<div>
						<!-- <div class="BlogCaption"></div> -->
						<div id="BlogPlaceholder">
							<img id="BlogMainImage" src="/ZooZoo/images/superstar.jpg" alt="בלוג השבוע" title="" />
							<div class="BlogMainCaption">כל שבוע בלוג חדש מכוכב חדש כנסו כנסו</div>
							<div class="BlogEnterLink" onclick="window.location.href = '<%=MapActionKey("Blog") %>';" title="כניסה לבלוג ערוץ הילדים">&nbsp;</div>
							<!-- <div class="BlogSubCaption">כותרת משנה: תיאור קצר של הכתבה, אולי תמצית. מתפרס על פני כמה שורות ואם יש יותר מידי טקסט הוא יהיה מוסתר</div> -->
						</div>
					</div>
				</div> <!-- left div -->
				<div style="clear: both;"></div>
			</div>
			<div style="clear: both;"></div>
			<div runat="server" id="pnlHomepageMiddleBanner">
				<div id="hp_middle_banner" class="homepage_banner" style="margin-bottom: 20px;"></div>
			</div>
			<div id="RightSideBar_Bottom" class="RightSideBar">
				<div runat="server" id="pnlHomepageQuickPoll" class="QuickPollPlaceholder">
					<div class="PollInternalContents">
						<asp:Repeater ID="rptQuickPoll" runat="server">
							<HeaderTemplate><div class="PollQuestion" id="PollQuestion" runat="server"></div></HeaderTemplate>
							<ItemTemplate>
								<div class="PollAnswerContainer" id="AnswerContainer" runat="server">
									<div style="float: right;" class="PollAnswerBullet_Empty"></div>
									<div style="float: right; margin-right: 5px;" class="PollAnswerText" id="AnswerText" runat="server"></div>
									<div style="clear: both;"></div>
								</div>
							</ItemTemplate>
						</asp:Repeater>
					</div>
					<div class="PollSubmitButton"></div>
					<div class="PollDummyVote" onclick="SendPollVote('0');"></div>
				</div>
			</div>
			<div id="BodyContentsPanel_Bottom" class="BodyContentsPanel">
				<div runat="server" id="pnlHomepageBottomContent" class="hp_middle_content">
					<div id="pnlHomepageEvents" runat="server" class="EventsPlaceholder">
						<div id="EventsWrapper">
							<div id="EventsContents">
                                <asp:Repeater ID="rptHomepageEvents" runat="server">
                                    <ItemTemplate>
                                        <div class="EventMainCaption"><asp:Label ID="lbEventTitle" runat="server" /><asp:Label ID="lbEventDate" runat="server" /></div>
    								    <div class="EventInnerContents"><%# DataBinder.Eval(Container.DataItem, "Description")%></div>
                                    </ItemTemplate>
                                </asp:Repeater>
							</div>
						</div>
						<a class="hp_all_events" href="<%=MapActionKey("Events")%>" title="לכל האירועים"></a>
					</div>
					<div id="pnlHomepageStreetGames" runat="server" class="StreetGamesPlaceholder">
						<div id="StreetGamesWrapper">
							<div id="StreetGamesContents">
                                <asp:Repeater ID="rptHomepageStreetGames" runat="server">
                                    <ItemTemplate>
                                        <div class="EventMainCaption"><asp:Label ID="lbEventTitle" runat="server" /><asp:Label ID="lbEventDate" runat="server" /></div>
    								    <div class="EventInnerContents"><%# DataBinder.Eval(Container.DataItem, "Description")%></div>
                                    </ItemTemplate>
                                </asp:Repeater>
                                <!--
								<div class="EventMainCaption">משחק רחוב ראשון</div>
								<div class="EventInnerContents">ים יבשה - מזכיר קצת את הרצל אמר , על הריצפה, חול, מדרכה  היו מצירים קו שמיצדו האחד היה הים, ומצידו השני היתה היבשה ועל השחקנים היה לקפוץ מצד אל צד לקול הוראותיו של מנהל המשחק!</div>
								<div class="EventMainCaption">משחק רחוב שני</div>
								<div class="EventInnerContents">כן לא שחור לבן- משחק בו מנהל המשחק שואל את השחקנים כל מיני שאלות גם מכשילות ועל השחקנים לענות מבלי להשתמש במילים כן לא שחור ולבן , שזה נשמע אולי קל אבל זה ממש לא פשוט, המנהל מנסה לבלבלך ולמשוך אותך להשתמש במילים האסורות</div>
								<div class="EventMainCaption">משחק רחוב שלישי</div>
								<div class="EventInnerContents">חבל- הגירסה הישנה של הגומי שני ילדים מסובבים חבל, ועל שאר הילדים לקפוץ מעל החבל תוך כדי שירת שירים שונים  כמו אדון מוזלה נולד בתמוזלה אדון מוז נולד בתמוז</div>
                                -->
							</div>
						</div>
						<a class="hp_all_streetgames" href="<%=MapActionKey("StreetGames")%>" title="לכל משחקי הרחוב"></a>
					</div>
				</div>
				<div runat="server" id="pnlHomepageEventGallery" class="hp_left_content">
					<div id="EventGalleryPlaceholder">
						<div id="EventGallerySlider">
							<asp:Repeater ID="rptEventGalleryImages" runat="server">
								<ItemTemplate><a href="#" target="_blank" onclick="GalleryImageClick(this);"><img id="GalleryImage" runat="server" class="EventGallerySliderImage" alt="תמונה" title="" /></a></ItemTemplate>
								<FooterTemplate><div class="clear EventGallerySliderImage"></div></FooterTemplate>
							</asp:Repeater>
						</div>
						<div class="EventGalleryDescription"></div>
						<a class="EventGalleryHomepageTitle" href="<%=MapActionKey("EventGallery") %>"></a>
					</div>
				</div>
			</div>
			<div style="clear: both;"></div>
			<div id="pnlHomepageArticles" class="ArticlesPlaceholder" runat="server" align="right">
				<div id="pnlPrimaryArticle" runat="server" class="HP_ArticleContainer HP_FirstArticleContainer">
					<div class="HP_ArticleContents">
						<div id="lblPrimaryArticleMainCaption" runat="server" class="HP_ArticleMainCaption"></div>
						<div id="lblPrimaryArticleSubCaption" runat="server" class="HP_ArticleSubCaption"></div>
					</div>
					<div class="HP_ArticleEnterButton"><a id="lnkPrimaryArticleFull" runat="server"><img src="/ZooZoo/images/enter_button_2.gif" alt="כניסה לכתבה" title="" /></a></div>
					<div class="HP_ArticleImage"><img id="imgPrimaryArticleIcon" runat="server" alt="כתבה ראשונה" title="" /></div>
				</div>
				<div id="pnlSubArticle" runat="server" class="HP_ArticleContainer HP_SecondArticleContainer">
					<div class="HP_ArticleContents">
						<div id="lblSubArticleMainCaption" runat="server" class="HP_ArticleMainCaption"></div>
						<div id="lblSubyArticleSubCaption" runat="server" class="HP_ArticleSubCaption"></div>
					</div>
					<div class="HP_ArticleEnterButton"><a id="lnkSubArticleFull" runat="server"><img src="/ZooZoo/images/enter_button_2.gif" alt="כניסה לכתבה" title="" /></a></div>
					<div class="HP_ArticleImage"><img id="imgSubArticleIcon" runat="server" alt="כתבה שנייה" title="" /></div>
				</div>
			</div>
			<!--
				<h1>ברוכים הבאים לפרויקט זוזו!</h1>
				<h2>האתר כרגע בבניה</h2>
				<h3>בקרוב יופיע כאן תוכן, נשמח לראותכם שוב!</h3>
				<img src="Images/article_1.jpg" />
			-->
		</div>
		<center>
		<div class="footer_placeholder">
			<div class="footer_ISF_logo"><img src="<%=MapZooZooImageFile("footer_logo_ISF_2.gif")%>" alt="לוגו התאחדות" title="" /></div>
			<div class="footer_ISF_text"><nobr>התאחדות הספורט לבתי הספר בישראל</nobr></div>
			<div class="footer_top_line">
				<asp:Repeater ID="rptFooterTopLinks" runat="server">
					<ItemTemplate><a href="<%# DataBinder.Eval(Container.DataItem, "Value")%>"><%# DataBinder.Eval(Container.DataItem, "Text")%></a></ItemTemplate>
					<SeparatorTemplate><img src="<%=MapZooZooImageFile("footer_separator_2.gif")%>" alt="" title="" align="middle" /></SeparatorTemplate>
				</asp:Repeater>
			</div>
			<div class="footer_bottom_line">
				<asp:Repeater ID="rptFooterBottomLinks" runat="server">
					<ItemTemplate><a href="<%# DataBinder.Eval(Container.DataItem, "Value")%>"><%# DataBinder.Eval(Container.DataItem, "Text")%></a></ItemTemplate>
					<SeparatorTemplate><img src="<%=MapZooZooImageFile("footer_separator_2.gif")%>" alt="" title="" align="middle" /></SeparatorTemplate>
				</asp:Repeater>
			</div>
			<div class="footer_credit_logo"><a href="http://www.kapaim.co.il/"><img src="<%=MapZooZooImageFile("footer_logo_credit_2.gif")%>" alt="לוגו כפיים" title="לאתר כפיים" /></a></div>
		</div>
		</center>
		<!-- developing stuff
			<asp:Label ID="Label1" runat="server" />
		-->
	</center>
	</div>
	
	<div id="PollVotedQuestionTemplate" runat="server" visible="false">
		<div class="PollQuestionVoted">$question</div>
	</div>
	<div id="PollVotedAnswerTemplate" runat="server" visible="false">
		<div class="PollVotedPlaceholder">
			<div class="PollAnswerText">$answer</div>
			<div class="PollVotesPlaceholder">
				<div class="$vote_text_class">$votes_percentage</div>
				<div class="VoteBar">
					<div class="Bar_Full" style="width: $bar_full_pixelspx;"></div>
					<div class="Bar_Empty"  style="width: $bar_empty_pixelspx;"></div>
					<div style="clear: both;"></div>
				</div>
				<div style="clear: both;"></div>
			</div>
		</div>
	</div>

	<div id="select_month" style="display: none;">
		<select id="ddlSelectMonth" runat="server" class="select_month_ddl" onchange="EventCalendarDateChosen();"></select>
	</div>
	<div id="select_year" style="display: none;">
		<select id="ddlSelectYear" runat="server"  class="select_year_ddl" onchange="EventCalendarDateChosen();"></select>
	</div>
	<a id="lnkEventCalendarNavigation" runat="server" class="event_calendar_navigate" style="display: none;">dummy</a>

    </form>
</body>
</html>
