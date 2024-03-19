using System;
using System.Linq;
using System.Collections;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Text;
using ISF.DataLayer;
using SportSite.Core;
using System.Web;

namespace SportSite.Common
{
	/// <summary>
	/// define possible action for website navigation.
	/// </summary>
	public enum SportSiteAction
	{
		Undefined = -1,
		Register = 0,
		Logout,
		AddTeam,
		ShowOrdersBasket,
		CommitPendingTeams,
		CommitPendingPlayers,
		ShowTeamDetails,
		AddPlayers,
		DeletePendingTeams,
		DeletePendingPlayers,
		ChangePendingNumber,
		MarkMessageRead,
		ShowMessages,
		ReloadEntities,
		DownloadUpdates,
		PerformDownload,
		PlayerCardClick,
		ShowPlayerDetails,
		ShowArticle,
		UpdateFlashNews,
		UpdateArticles,
		CheckNewData,
		ViewNews,
		GeneralChampionships,
		ClubChampionships,
		OtherChampionships,
		ViewCalendar,
		WriteToUs,
		ViewPictureGallery,
		GoIsfHome,
		RelevantLinks,
		ViewHotLink,
		UpdateLinks,
		UpdatePages,
		ViewDynamicPage,
		UpdatePolls,
		UpdateImageGallery,
		UpdatePermanentChamps,
		UpdateBanners,
		UpdateEvents,
		NationalChampionships,
		ClubRegister,
		UpdateChampionshipAttachments,
		ShowPollResults,
		UpdateChampLogo,
		EditUnconfirmedPlayers,
		UpdateSponsors,
		UpdateSpecialOffer,
		UpdateTopBanner,
		UpdateMainAdvertisement,
		UpdateSecondaryAdvertisement,
		ShowEventsReport,
		UpdateSmallAdvertisement,
		ShowFunctionaryList,
		ShowInstantMessage,
		ResetSubArticles,
		AssignCompetitionPlayers,
		EditFAQ,
		SetMatchResults,
		PracticeCampRegister,
		UpdateClubRegisterAttachment,
		EditUnconfirmedTeams,
		GeneralChampRegister,
		TeacherCourseRegister,
		TeacherCourseEdit,
		EditZooZooAboutPage,
		EditZooZooEventGallery,
		EditZooZooPolls, 
		EditZooZooEvents,
		EditZooZooStreetGames,
		EditZooZooWeeklyGame,
		EditZooZooContactUs,
		EditZooZooWroteOnUs,
		UploadStudentPicture,
		DisplayStudentPictures,
		FindStudentById, 
		NewInterface
	}

	public enum NavBarLink
	{
		MainPage = 0,
		AdminTools,
		InfoPage,
		Registration,
		YSDT, //YoungSportsmanDevelopmentTeam
		PermanentChamp1,
		PermanentChamp2,
		PermanentChamp3,
		PermanentChamp4,
		PermanentChamp5,
		NationalChamps,
		ClubChamps,
		OtherChamps,
		Calendar,
		WriteUs,
		Publications,
		PictureGallery,
		Commities,
		Takanon,
		IsfHome,
		RelevantLinks,
		SportBoard,
		Archieve,
		SportBasket,
		HotLink,
		ClubRegister
	}

	public enum SportSiteErrors
	{
		AddPollDatabaseError = 0,
		AddPollTooManyCharacters,
		AddPollNotEnoughCharacters,
		AddPollInvalidExpirationDate,
		AddAnswerDatabaseError,
		AddAnswerTooManyCharacters,
		AddAnswerNotEnoughCharacters,
		EditAnswerDatabaseError,
		EditAnswerTooManyCharacters,
		EditAnswerNotEnoughCharacters
	}

	public enum HandlePollEvents
	{
		EditPoll = 0,
		EditAnswer,
		ListPolls,
		DoNothing
	}

	public enum ZooZooPage
	{
		None = 0,
		Homepage,
		About,
		Events,
		StreetGames,
		WeeklyGame,
		Newspaper,
		Championships,
		Twits,
		ContactUs,
		QuickPollVote,
		QuickPollPeek,
		Articles,
		EventGallery
	}

	public static class SportSiteExtensions
	{
		public static bool IsValid(this WebSiteServices.ArticleData article)
		{
			return (article != null) && (article.ID > 0) && ((article.Caption + "").Trim().Length > 0);
		}
	}

	/// <summary>
	/// Summary description for Data.
	/// </summary>
	public class Data
	{
		public static int _currentSeason = -1;
		public static int GetCurrentSeason()
		{
			if (_currentSeason < 0)
				_currentSeason = Sport.Entities.Season.GetOpenSeason();
			return _currentSeason;
		}

		public struct DynamicLinkData
		{
			public NavBarLink Link;
			public string Text;
			public DynamicLinkData[] Children;

			public static DynamicLinkData Empty;

			static DynamicLinkData()
			{
				Empty = new DynamicLinkData();
				Empty.Link = NavBarLink.YSDT;
			}

			public DynamicLinkData(NavBarLink link, string text,
				DynamicLinkData[] children)
			{
				this.Link = link;
				this.Text = text;
				this.Children = children;
			}

			public override string ToString()
			{
				return this.Text;
			}

			public bool Contains(DynamicLinkData linkData)
			{
				if (this.Children == null)
					return (this.Text == linkData.Text) && (this.Link == linkData.Link);

				for (int i = 0; i < this.Children.Length; i++)
				{
					if ((this.Children[i].Text == linkData.Text) &&
						(this.Children[i].Link == linkData.Link))
					{
						return true;
					}
					if (Children[i].Contains(linkData))
						return true;
				}
				return false;
			}

			public bool Contains(WebSiteServices.PageData pageData)
			{
				return Contains(new DynamicLinkData((NavBarLink)pageData.Index,
					pageData.Caption, null));
			}
		}

		public static readonly string INI_FILE_NAME = "SportSite.ini";
		public static readonly string ImageGalleryFolder = "ImageGallery";
		public static readonly string AppPath = System.Configuration.ConfigurationManager.AppSettings["Path"];
		public static readonly string ArticlesFolderName = "Articles";
		public static readonly string AttachmentsFolderName = "Attachments";

		public static readonly int CheckNewDataInterval = 60; //seconds
		public static readonly string NEW_LINE = (((char)13).ToString()) + (((char)10).ToString());
		public static readonly int FlashTitlesVersion = 3;

		public static readonly string FlashTitleQuote = "{amp}quot;";

		public static readonly System.Drawing.Size ArticleSmallImage = new System.Drawing.Size(148, 116);
		public static readonly System.Drawing.Size ArticleBigImage = new System.Drawing.Size(258, 172);

		/* public static string[] PermanentChampNames=new string[] 
			{"ליגת על כדורסל תלמידים", "ליגת על כדורסל תלמידות", "ליגת על כדורעף תלמידים", 
			"ליגת על כדורעף תלמידות", "כדורגל י'-י\"ב תלמידות"}; */

		public static readonly int MaximumSubArticles = (5 - 1);
		public static readonly string ArticlesImagesFolder = "Images/Articles";
		public static readonly int MaximumArticleImages = 4;

		public static readonly string CaptionsFontFamily = "Cahit";

		public static readonly string ThumbnailBaseFolder = "Thumbnails";
		public static readonly string DynamicImagesFolder = "DynamicImages";

		public static readonly string InternationalIsfWebsite = "http://www.isfsports.org/"; //"http://www.isfsports.org/sports/home.asp";

		public static readonly string LinkSuffix = ""; //"&raquo;"; //the small <<

		public static string[] ChampMemuItemsSections =
			new string[] { "games", "ranking", "teams", "attachments" };

		public static string[] MatchChampMemuItemsText =
			new string[] { "תכנית משחקים", "טבלאות דירוג", "חלוקה לבתים", "חוברת / חוזר אליפות" };

		public static string[] CompetitionChampMenuItemsText =
			new string[] { "דו\"חות סיכום", "טבלת דירוג כללי", "חלוקה לבתים", "חוברת / חוזר אליפות" };

		public static DynamicLinkData[] DynamicLinks = null;

		private static string _galleryGroupsOrder = "";
		public static string GetGalleryGroupsOrder(System.Web.HttpServerUtility Server)
		{
			if ((_galleryGroupsOrder == null) || (_galleryGroupsOrder.Length == 0))
				RebuildGalleryGroups(Server);
			return _galleryGroupsOrder;
		}
		public static void RebuildGalleryGroups(System.Web.HttpServerUtility Server)
		{
			_galleryGroupsOrder = Common.Tools.ReadIniValue("ImageGallery",
				"GroupsOrder", Server);
		}

		static Data()
		{
			/* new DynamicLinkData(NavBarLink.Publications, "פרסומים", 
			new DynamicLinkData[] 
			{
				new DynamicLinkData(NavBarLink.Publications, "מגזין הכל בספורט", null), 
				new DynamicLinkData(NavBarLink.Publications, "מגזין פיוז", null)}), */
			DynamicLinks = new DynamicLinkData[] {
				new DynamicLinkData(NavBarLink.InfoPage, "דף מידע ההתאחדות", null), 
				new DynamicLinkData(NavBarLink.YSDT, "היחידה לספורטאים צעירים", 
				new DynamicLinkData[] 
				{
					new DynamicLinkData(NavBarLink.YSDT, "כללי", null), 
					new DynamicLinkData(NavBarLink.YSDT, "השכלה תנועתית ספורטיבית", null), 
					new DynamicLinkData(NavBarLink.YSDT, "מחנות אימון", null), 
					new DynamicLinkData(NavBarLink.YSDT, "השתלמויות", null), 
					new DynamicLinkData(NavBarLink.YSDT, "חומר מקצועי", null), 
					new DynamicLinkData(NavBarLink.YSDT, "עתודת זהב", null), 
					new DynamicLinkData(NavBarLink.YSDT, "עתודה צעירה", null), 
					new DynamicLinkData(NavBarLink.YSDT, "שאלות ותשובות", null), 
					new DynamicLinkData(NavBarLink.YSDT, "אליפויות , ליגות ותחרויות", 
				    new DynamicLinkData[] 
    				{
                        new DynamicLinkData(NavBarLink.YSDT, "מרכזי מצוינות כדורסל", null), 
                        new DynamicLinkData(NavBarLink.YSDT, "מרכזי מצויינות כדורעף", null), 
                        new DynamicLinkData(NavBarLink.YSDT, "מרכזי מצויינות א\"ק", null), 
                        new DynamicLinkData(NavBarLink.YSDT, "הפנינג לצעירים", null), 
                        new DynamicLinkData(NavBarLink.YSDT, "אליפות היחידה", null)
                    })
                }), 
				new DynamicLinkData(NavBarLink.Publications, "מגזין הכל בספורט", null), 
				new DynamicLinkData(NavBarLink.Commities, "ועדות", null), 
				new DynamicLinkData(NavBarLink.Takanon, "תקנון אירועי ספורט וחינוך גופני", null), 
				new DynamicLinkData(NavBarLink.SportBoard, "לוח אירועי הספורט הארציים", null), 
				new DynamicLinkData(NavBarLink.Archieve, "ארכיון", null), 
				new DynamicLinkData(NavBarLink.SportBasket, "טפסים", null)
				//,new DynamicLinkData(NavBarLink.ClubRegister, "רישום מועדון", null)

/*
						new DynamicLinkData(NavBarLink.YSDT, "אליפויות , ליגות ותחרויות", null), 
						new DynamicLinkData(NavBarLink.YSDT, "עתודת זהב", null), 
						new DynamicLinkData(NavBarLink.YSDT, "עתודה צעירה", null), 
						new DynamicLinkData(NavBarLink.YSDT, "שאלות ותשובות", null), 
                        new DynamicLinkData(NavBarLink.YSDT, "ליגת מרכזי מצוינות בכדורסל", null), 
                        new DynamicLinkData(NavBarLink.YSDT, "ליגת מרכזי מצויינות בכדורעף", null), 
                        new DynamicLinkData(NavBarLink.YSDT, "ליגת מרכזי מצויינות בא\"ק", null), 
                        new DynamicLinkData(NavBarLink.YSDT, "הפנינג לצעירים", null), 
                        new DynamicLinkData(NavBarLink.YSDT, "אליפות היחידה", null)
*/
			};
		}

		public static DynamicLinkData FindLinkByCaption(string caption)
		{
			return FindLinkByCaption(caption, DynamicLinks);
		}

		private static DynamicLinkData FindLinkByCaption(string caption, DynamicLinkData[] links)
		{
			if (links == null || links.Length == 0)
				return DynamicLinkData.Empty;

			foreach (DynamicLinkData link in links)
			{
				if (link.Text == caption)
					return link;

				DynamicLinkData result = FindLinkByCaption(caption, link.Children);
				if (result.Text != null)
					return result;
			}

			return DynamicLinkData.Empty;
		}

		public static int DynamicIndex(NavBarLink link)
		{
			switch (link)
			{
				case NavBarLink.InfoPage:
					return 0;
				case NavBarLink.YSDT:
					return 1;
				case NavBarLink.Publications:
					return 2;
				case NavBarLink.Commities:
					return 3;
				case NavBarLink.Takanon:
					return 4;
				case NavBarLink.SportBoard:
					return 5;
				case NavBarLink.Archieve:
					return 6;
				case NavBarLink.SportBasket:
					return 7;
				default:
					return -1;
			}
		}
	} //end class Data

	public class LimitedArticleData
	{
		public int ArticleId { get; set; }
		public string Caption { get; set; }
		public string SubCaption { get; set; }
		public string FirstImage { get; set; }

		public LimitedArticleData()
		{
			this.ArticleId = -1;
			this.Caption = "";
			this.SubCaption = "";
			this.FirstImage = "";
		}

		public LimitedArticleData(WebSiteServices.ArticleData article)
			: this()
		{
			if (article != null && article.ID >= 0)
			{
				this.ArticleId = article.ID;
				this.Caption = article.Caption;
				this.SubCaption = article.SubCaption;
				if (article.Images != null && article.Images.Length > 0 && article.Images[0].Length > 0)
				{
					string strImage = Data.AppPath + "/" + Data.ArticlesImagesFolder + "/" + article.Images[0];
					if (System.IO.File.Exists(HttpContext.Current.Server.MapPath(strImage)))
						this.FirstImage = strImage;
				}
			}
		}
	}

	/// <summary>
	/// represents data of html link, <a></a> tag - text and url of the link.
	/// </summary>
	public class LinkData
	{
		private string _url;
		private string _text;

		/// <summary>
		/// create new instance of the link data with given url and text.
		/// </summary>
		public LinkData(string url, string text)
		{
			URL = url;
			Text = text;
		}

		/// <summary>
		/// creates empty instance link data.
		/// </summary>
		public LinkData()
		{
			URL = "";
			Text = "";
		}

		/// <summary>
		/// get or set url of the link - where it points to.
		/// </summary>
		public string URL
		{
			get { return _url; }
			set { _url = value; }
		}

		/// <summary>
		/// get or set text of the link - text which when clicked redirects the user.
		/// </summary>
		public string Text
		{
			get { return _text; }
			set { _text = value; }
		}
	} //end class LinkData

	/// <summary>
	/// collection of menu items and sub menus.
	/// </summary>
	public class Menu
	{
		private ArrayList _items;
		private ArrayList _menus;
		private bool _visible = true;

		/// <summary>
		/// create new empty menu.
		/// </summary>
		public Menu()
		{
			_items = new ArrayList();
			_menus = new ArrayList();
			_menus.Add(this);
		}

		/// <summary>
		/// get all menu items and sub menus.
		/// </summary>
		public ArrayList Items
		{
			get { return _items; }
		}

		public ArrayList Menus
		{
			get { return _menus; }
			set { _menus = value; }
		}

		public bool Visible
		{
			get
			{
				return _visible;
			}
			set
			{
				_visible = value;
				foreach (object obj in this.Items)
				{
					if (obj is Menu)
					{
						Menu menu = (Menu)obj;
						menu.Visible = _visible;
					}
					if (obj is MenuItem)
					{
						MenuItem item = (MenuItem)obj;
						item.Visible = _visible;
					}
				}
			}
		}

		/// <summary>
		/// returns menu item with same command as given item, or null if does not exist.
		/// </summary>
		public MenuItem FindItem(MenuItem item)
		{
			for (int i = 0; i < _items.Count; i++)
			{
				if (_items[i] is Menu)
				{
					Menu menu = (Menu)_items[i];
					MenuItem menuItem = menu.FindItem(item);
					if (menuItem != null)
						return menuItem;
					continue;
				}
				if (_items[i] is MenuItem)
				{
					MenuItem menuItem = (MenuItem)_items[i];
					if (menuItem.Equals(item))
						return menuItem;
				}
			}
			return null;
		}

		/// <summary>
		/// returns menu item with same command as given item, or null if does not exist.
		/// </summary>
		public MenuItem FindItem(LinkButton control)
		{
			for (int i = 0; i < _items.Count; i++)
			{
				if (_items[i] is Menu)
				{
					Menu menu = (Menu)_items[i];
					MenuItem menuItem = menu.FindItem(control);
					if (menuItem != null)
						return menuItem;
					continue;
				}
				if (_items[i] is MenuItem)
				{
					MenuItem menuItem = (MenuItem)_items[i];
					if (menuItem.Control == control)
						return menuItem;
				}
			}
			return null;
		}

		/// <summary>
		/// returns menu item with given command name and argument if exist, null otherwise.
		/// </summary>
		public MenuItem FindItem(string commandName, string commandArg)
		{
			return FindItem(new MenuItem(commandName, commandArg, ""));
		}

		/// <summary>
		/// add given item to the menu, or update item with the same command if exist.
		/// </summary>
		public void AddItem(MenuItem item, CommandEventHandler handler)
		{
			if (item == null)
				throw new ArgumentException("SportSite Menu: can't add null menu item");
			MenuItem itemToAdd = FindItem(item);
			if (itemToAdd == null)
			{
				itemToAdd = new MenuItem(item.Text);
				itemToAdd.Command += handler;
				_items.Add(itemToAdd);
			}
			itemToAdd.CommandName = item.CommandName;
			itemToAdd.CommandArgument = item.CommandArgument;
			itemToAdd.Text = item.Text;
		}

		public void AddItem(string commandName, string commandArg, string text,
			System.Web.UI.WebControls.CommandEventHandler handler)
		{
			AddItem(new MenuItem(commandName, commandArg, text), handler);
		}

		public void AddItem(string commandName, string commandArg, string text)
		{
			AddItem(commandName, commandArg, text, null);
		}

		/// <summary>
		/// add given menu as sub menu of the current menu.
		/// </summary>
		public void AddMenu(Menu menu)
		{
			_items.Add(menu);
			_menus.Add(menu);
		}

		/// <summary>
		/// remove given item, or item with the same command, from the menu if exists.
		/// </summary>
		public void RemoveItem(MenuItem item)
		{
			_items.Remove(item);
		}

		public void RemoveItem(string commandName, string commandArg)
		{
			RemoveItem(new MenuItem(commandName, commandArg, ""));
		}

		/// <summary>
		/// remove given sub menu.
		/// </summary>
		/// <param name="menu"></param>
		public void RemoveMenu(Menu menu)
		{
			_items.Remove(menu);
			_menus.Remove(menu);
		}

		/// <summary>
		/// return menu item or sub menu in given index or null if index invalid
		/// </summary>
		public object this[int index]
		{
			get
			{
				if ((index < 0) || (index >= _items.Count))
					return null;
				return _items[index];
			}
		}

		/// <summary>
		/// return menu item with given command name and argument, null if does not exist.
		/// </summary>
		public MenuItem this[string commandName, string commandArg]
		{
			get
			{
				return FindItem(commandName, commandArg);
			}
		}
	} //end class Menu

	/// <summary>
	/// represent menu item - link button with specific command name and argument.
	/// each link will store the command info in the global session state once 
	/// clicked and the calling page should check this info when loaded.
	/// </summary>
	public class MenuItem
	{
		/// <summary>
		/// the session key name - will store CommandEventArgs class once item clicked.
		/// </summary>
		public static readonly string SessionKey = "menu_item_command";

		/// <summary>
		/// string to be displayed in the browser status bar when mouse is over the item.
		/// </summary>
		private string _statusMessage;
		private bool _visible = true;
		private LinkButton _control;

		public delegate void StateChangedEventHandler(object sender);
		//public event StateChangedEventHandler StateChanged=null;

		/// <summary>
		/// construct menu item with given text.
		/// </summary>
		public MenuItem(string text)
		{
			_control = new LinkButton();
			_control.Text = text;
			_control.Command += new System.Web.UI.WebControls.CommandEventHandler(this.MenuItemClicked);
			//default status message is the item text:
			_statusMessage = text;
		}

		public MenuItem(string commandName, string commandArg, string text)
			: this(text)
		{
			_control.CommandName = commandName;
			_control.CommandArgument = commandArg;
		}

		/// <summary>
		/// get menu item control as link button.
		/// </summary>
		public LinkButton Control
		{
			get { return _control; }
		}

		public bool Visible
		{
			get
			{
				return _visible;
			}
			set
			{
				_visible = value;
				this.Control.Visible = _visible;
				/* if (this.StateChanged != null)
					this.StateChanged(this); */
			}
		}

		/// <summary>
		/// get or set staus message.
		/// </summary>
		public string StatusMessage
		{
			get { return _statusMessage; }
			set { _statusMessage = value; }
		}

		/// <summary>
		/// set or get item command name.
		/// </summary>
		public string CommandName
		{
			get { return _control.CommandName; }
			set { _control.CommandName = value; }
		}

		/// <summary>
		/// set or get item command argument.
		/// </summary>
		public string CommandArgument
		{
			get { return _control.CommandArgument; }
			set { _control.CommandArgument = value; }
		}

		/// <summary>
		/// set or get item text.
		/// </summary>
		public string Text
		{
			get { return _control.Text; }
			set { _control.Text = value; }
		}

		/// <summary>
		/// set css class of the menu item to given css class, returns previous class.
		/// </summary>
		public string SetCssClass(string cssClass)
		{
			string result = _control.CssClass;
			_control.CssClass = cssClass;
			return result;
		}

		/// <summary>
		/// add given attribute to the menu item, returns amount of attributes.
		/// </summary>
		public int AddAttribute(string key, string strValue)
		{
			_control.Attributes.Add(key, strValue);
			return _control.Attributes.Count;
		}

		public event CommandEventHandler Command;

		/// <summary>
		/// called once menu item is clicked.
		/// </summary>
		public void MenuItemClicked(object sender, CommandEventArgs args)
		{
			//store in session scope:
			//System.Web.HttpContext.Current.Session[MenuItem.SessionKey] = args;

			if (Command != null)
				Command(this, args);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is MenuItem))
			{
				throw new ArgumentException("SportSite Menu Item compare: must compare only menu items");
			}
			MenuItem item = (MenuItem)obj;
			return ((this._control.CommandName == item._control.CommandName) &&
				(this._control.CommandArgument == item._control.CommandArgument));
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}


	} //end class MenuItem

	/*
	public class SimpleData
	{
		public int ID { get; set; }
		public string Name { get; set; }

		public SimpleData()
		{
			this.ID = 0;
			this.Name = string.Empty;
		}

		public SimpleData(int id, string name)
			: this()
		{
			this.ID = id;
			this.Name = name;
		}
	}
	*/

	#region ZooZoo
	#region Articles
	public class ZooZooArticle
	{
		private readonly int maxBodyFormattedChars = 100;
		public int ID { get; set; }
		public string Caption { get; set; }
		public string SubCaption { get; set; }
		public DateTime SubmissionDate { get; set; }
		public string Body { get; set; }
		public string Url { get; set; }
		public string SubmissionDateFormatted { get { return Tools.GetHebrewTimeElpased(this.SubmissionDate); } }
		public string BodyFormatted
		{
			get
			{
				if (this.Body.Length <= maxBodyFormattedChars)
					return Tools.KeepOriginalFormat(this.Body);

				int index = maxBodyFormattedChars - 3;
				while (index >= 0)
				{
					char c = this.Body[index];
					if (char.IsWhiteSpace(c) || char.IsSeparator(c) || char.IsPunctuation(c))
						break;
					index--;
				}

				if (index <= 0)
					index = maxBodyFormattedChars - 3;

				return Tools.KeepOriginalFormat(this.Body.Substring(0, index)) + "...";
			}
		}

		public ZooZooArticle()
		{
			this.ID = 0;
			this.Caption = string.Empty;
			this.SubCaption = string.Empty;
			this.Body = string.Empty;
			this.SubmissionDate = DateTime.MinValue;
			this.Url = string.Empty;
		}

		public ZooZooArticle(SportSite.WebSiteServices.ArticleData article)
			: this()
		{
			this.ID = article.ID;
			this.Caption = article.Caption;
			this.SubCaption = article.SubCaption;
			this.Body = article.Contents;
			this.SubmissionDate = article.Time;
		}
	}
	#endregion

	#region base element
	public abstract class ZooZooElement : IComparable
	{
		public static readonly char MAIN_DELIMETER = '|';
		public int Id { get; set; }
		public bool Deleted { get; set; }

		protected ZooZooElement()
		{
			this.Id = 0;
			this.Deleted = false;
		}

		public override bool Equals(object obj)
		{
			return this.Id.Equals((obj as ZooZooElement).Id);
		}

		public override int GetHashCode()
		{
			return this.Id.GetHashCode();
		}

		public int CompareTo(object obj)
		{
			return this.Id.CompareTo((obj as ZooZooElement).Id);
		}
	}
	#endregion

	#region Poll
	public class ZooZooPoll : ZooZooElement
	{
		const string ANSWER_DELIMETER_OUTER = "***";
		const string ANSWER_DELIMETER_INNER = "+++";

		public string Question { get; set; }
		public List<Answer> Answers { get; set; }

		private static ZooZooPoll empty;
		static ZooZooPoll()
		{
			empty = new ZooZooPoll();
			empty.Id = -1;
		}
		public static ZooZooPoll Empty { get { return empty; } }

		public ZooZooPoll()
			: base()
		{
			this.Question = string.Empty;
			this.Answers = new List<Answer>();
		}

		public ZooZooPoll(string question)
			:this()
		{
			this.Question = question;
		}

		public override string ToString()
		{
			if (this.Equals(ZooZooPoll.Empty))
				return string.Empty;

			List<string> parts = new List<string>();
			parts.Add(this.Id.ToString());
			parts.Add(this.Question);
			parts.Add(string.Join(ANSWER_DELIMETER_OUTER, Answers.ConvertAll(answer => string.Join(ANSWER_DELIMETER_INNER, new string[] { answer.Id.ToString(), answer.Text, answer.Votes.ToString() })).ToArray()));
			parts.Add(this.Deleted.ToString());
			return string.Join(MAIN_DELIMETER.ToString(), parts.ToArray());
		}

		public bool AddAnswer(string text)
		{
			if (!string.IsNullOrEmpty(text) && !this.Answers.Exists(answer => answer.Text.Equals(text, StringComparison.CurrentCultureIgnoreCase)))
			{
				int answerId = this.Answers.Count + 1;
				this.Answers.Add(new Answer { Id = answerId, Text = text, Votes = 0 });
				return true;
			}
			return false;
		}

		public int GetVotes(int answerId)
		{
			Answer answer = this.Answers.GetById(answerId);
			return answer.Equals(Answer.Empty) ? 0 : answer.Votes;
		}

		public bool UpdateVotes(int answerId, int votes)
		{
			Answer answer = this.Answers.GetById(answerId);
			if (!answer.Equals(Answer.Empty))
			{
				answer.Votes = votes;
				this.Answers.SetById(answerId, answer);
				return true;
			}
			return false;
		}

		public static ZooZooPoll FromString(string data)
		{
			if (!string.IsNullOrEmpty(data))
			{
				string[] parts = data.Split(MAIN_DELIMETER);
				if (parts.Length > 3)
				{
					int id;
					if (Int32.TryParse(parts[0], out id) && id > 0)
					{
						ZooZooPoll poll = new ZooZooPoll(parts[1]);
						poll.Id = id;
						string[] answersData = parts[2].Split(new string[] { ANSWER_DELIMETER_OUTER }, StringSplitOptions.RemoveEmptyEntries);
						answersData.ToList().ForEach(answerData =>
						{
							string[] innerData = answerData.Split(new string[] { ANSWER_DELIMETER_INNER }, StringSplitOptions.RemoveEmptyEntries);
							if (innerData.Length >= 3)
							{
								int answerVotes;
								int answerId;
								if (Int32.TryParse(innerData[0], out answerId) && answerId > 0 && Int32.TryParse(innerData[2], out answerVotes) && answerVotes >= 0)
								{
									string answerText = innerData[1];
									if (answerText.Length > 0 && !poll.Answers.Exists(answer => (answer.Id.Equals(answerId) || answer.Text.Equals(answerText, StringComparison.CurrentCultureIgnoreCase))))
										poll.Answers.Add(new Answer { Id = answerId, Text = answerText, Votes = answerVotes, Parent = poll });
								}
							}
						});
						poll.Deleted = parts[3].Equals("true", StringComparison.CurrentCultureIgnoreCase);
						return poll;
					}
				}
			}
			
			return ZooZooPoll.Empty;
		}

		public struct Answer
		{
			public int Id;
			public string Text;
			public int Votes;

			public ZooZooPoll Parent;

			private static Answer empty;
			public static Answer Empty { get { return empty; } }
			static Answer()
			{
				empty = new Answer { Id = 0, Text = string.Empty, Votes = 0 };
			}
		}
	}
	#endregion

	#region Event
	public class ZooZooEvent : ZooZooElement
	{
		protected static readonly char DATE_DELIMETER = '-';

		public DateTime Date { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }

		private string hebDayOfWeek = string.Empty;
		public string HebDayOfWeek
		{
			get
			{
				if (hebDayOfWeek.Length == 0 && this.Date.Year > 1900)
				{
					System.Globalization.CultureInfo hebCulture = System.Globalization.CultureInfo.CreateSpecificCulture("he-IL");
					hebCulture.DateTimeFormat.Calendar = new System.Globalization.HebrewCalendar();
					hebDayOfWeek = this.Date.ToString("dddd", hebCulture);
				}
				return hebDayOfWeek;
			}
		}
		
		private static ZooZooEvent empty;
		static ZooZooEvent()
		{
			empty = new ZooZooEvent();
			empty.Id = -1;
		}
		public static ZooZooEvent Empty { get { return empty; } }

		public ZooZooEvent()
			: base()
		{
			this.Date = DateTime.MinValue;
			this.Description = string.Empty;
			this.Title = string.Empty;
		}

		public ZooZooEvent(DateTime date, string description, string title)
			: this()
		{
			this.Date = date;
			this.Description = description;
			this.Title = title;
		}

		public override string ToString()
		{
			if (this.Equals(Empty))
				return string.Empty;

			List<string> parts = new List<string>();
			parts.Add(this.Id.ToString());
			parts.Add(string.Join(DATE_DELIMETER.ToString(), new string[] { this.Date.Day.ToString(), this.Date.Month.ToString(), this.Date.Year.ToString() }));
			parts.Add(this.Description.Replace("\n", "<br />"));
			parts.Add(this.Title);
			parts.Add(this.Deleted.ToString());
			return string.Join(MAIN_DELIMETER.ToString(), parts.ToArray());
		}

		public static ZooZooEvent FromString(string data)
		{
			if (!string.IsNullOrEmpty(data))
			{
				string[] parts = data.Split(ZooZooElement.MAIN_DELIMETER);
				if (parts.Length >= 5)
				{
					int id;
					if (Int32.TryParse(parts[0], out id) && id > 0)
					{
						ZooZooEvent eve = new ZooZooEvent();
						eve.Id = id;
						string[] dateParts = parts[1].Split(DATE_DELIMETER);
						if (dateParts.Length == 3)
						{
							eve.Date = new DateTime(Int32.Parse(dateParts[2]), Int32.Parse(dateParts[1]), Int32.Parse(dateParts[0]));
							eve.Description = System.Web.HttpContext.Current.Server.HtmlDecode(parts[2]);
							eve.Title = parts[3];
							eve.Deleted = parts[4].Equals("true", StringComparison.CurrentCultureIgnoreCase);
							return eve;
						}
					}
				}
			}

			return ZooZooEvent.Empty;
		}
	}
	#endregion

	#region Event Gallery
	public class ZooZooEventGalleryAlbum
	{
		public int Id = 0;
		public List<string> Images = new List<string>();
		public string CoverImage = string.Empty;
		public string Title = string.Empty;
		public string Description = string.Empty;

		public static string FormatConfigKey(int albumId, string caption)
		{
			return string.Format("Album_{0}_{1}", albumId, caption);
		}

		/*
		private static ZooZooEventGalleryAlbum empty;
		public static ZooZooEventGalleryAlbum Empty { get { return empty; } }

		static ZooZooEventGalleryAlbum()
		{
			empty = new ZooZooEventGalleryAlbum { Images = new List<string>(), CoverImage = string
		}
		*/
	}  
	#endregion

	#region Attachment
	public class ZooZooAttachment
	{
		public string FileName { get; set; }
		public string FileUrl { get; set; }
		public string IconUrl { get; set; }
		public string FileType { get; set; }
		public int FileSize { get; set; }

		public string Title
		{
			get
			{
				if (this.FileSize > 0)
				{
					if (this.FileSize < 1024)
						return string.Format("{0} בייטים", this.FileSize);
					if (this.FileSize < 1024 * 1024)
						return string.Format("{0} קילובייטים", ((int)((this.FileSize / 1024) + 0.5)).ToString());
					return string.Format("{0} מגהבייטים", ((int)((this.FileSize / (1024 * 1024)) + 0.5)).ToString());
				}

				return string.Empty;
			}
		}

		public ZooZooAttachment()
		{
			this.FileName = string.Empty;
			this.FileUrl = string.Empty;
			this.IconUrl = string.Empty;
			this.FileType = string.Empty;
			this.FileSize = 0;
		}

		public ZooZooAttachment(AttachmentData data)
			: this()
		{
			this.FileName = string.IsNullOrEmpty(data.description) ? data.fileName : data.description;
			this.FileUrl = AttachmentManager.GetRelativePath(data.fileName);
			this.IconUrl = AttachmentManager.GetIcon(data.fileType);
			this.FileType = AttachmentManager.GetFileTypeDescription(data.fileType, "");
			this.FileSize = (int)data.fileSize;
		}
	}
	#endregion

	#region Extensions
	public static class ZooZooExtensions
	{
		public static ZooZooPoll.Answer GetById(this List<ZooZooPoll.Answer> answers, int id)
		{
			return answers.DefaultIfEmpty(ZooZooPoll.Answer.Empty).FirstOrDefault(answer => answer.Id.Equals(id));
		}

		public static void SetById(this List<ZooZooPoll.Answer> answers, int id, ZooZooPoll.Answer answer)
		{
			for (int i = 0; i < answers.Count; i++)
			{
				if (answers[i].Id == id)
				{
					answers[i] = answer;
					break;
				}
			}
		}
	}
	#endregion
	#endregion
}
