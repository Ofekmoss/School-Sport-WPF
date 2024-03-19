using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sport.Common;
using SportSite.Common;
using System.IO;
using System.Text;

namespace SportSite.Core
{
	public class ZooZooManager
	{
		#region private properties
		private static ZooZooManager instance;
		private IniFile configManager;
		private DateTime configLastRefresh = DateTime.MinValue;
		private readonly List<string> validGalleryFileTypes = new List<string>(new string[] { ".jpg", ".gif", ".png", ".bmp" });
		private readonly string pollFormTemplateFile = Data.AppPath + "/ZooZoo/ZooZooPollFormTemplate.txt";
		private readonly string eventsFormTemplateFile = Data.AppPath + "/ZooZoo/ZooZooEventsFormTemplate.txt";
		private readonly string contactUsTemplateFile = Data.AppPath + "/ZooZoo/ContactUsTemplate.txt";
		private readonly string logFolderName = Data.AppPath + "/ZooZoo/Log";
		private readonly string logBrowsersFileName = Data.AppPath + "/ZooZoo/Log/Browsers.txt";
		private readonly string logResolutionsFileName = Data.AppPath + "/ZooZoo/Log/Resolutions.txt";
		private readonly string logCurrentLogFileName = Data.AppPath + "/ZooZoo/Log/Current.txt";
		private readonly string weeklyGamePictureFolder = Data.AppPath + "/ZooZoo/Images/WeeklyGame";
		private readonly string wroteOnUsPictureFolder = Data.AppPath + "/ZooZoo/Images/WroteOnUs";
		private string logFolderPath = string.Empty;
		private string logFilePath = string.Empty;
		private int logFileLines = 0;
		private object logLocker = new object();
		private Dictionary<string, int> browsersMapping = new Dictionary<string, int>();
		private Dictionary<string, int> resolutionsMapping = new Dictionary<string, int>();
		#endregion

		#region constructors
		static ZooZooManager()
		{
			//make sure to have root path when starting in sub folder
			instance = new ZooZooManager(HttpContext.Current.Server.MapPath(SportSite.Common.Data.AppPath + "/ZooZoo.config"));
		}

		private ZooZooManager(string iniPath)
		{
			configManager = new IniFile(iniPath);
		}
		#endregion

		#region instance
		public static ZooZooManager Instance
		{
			get { return instance; }
		}
		#endregion

		#region articles
		public int[] Articles
		{
			get
			{
				return TryGetConfigValue("Articles", "ID").SplitToNumbers(',');
			}
			set
			{
				TryWriteConfigValue("Articles", "ID", 
					string.Join(",", value.ToList().ConvertAll(id => { return id.ToString(); }).ToArray()));
			}
		}

		public SportSite.WebSiteServices.ArticleData[] GetRawArticles(int count)
		{
			List<SportSite.WebSiteServices.ArticleData> articles = new List<WebSiteServices.ArticleData>();
			SportSite.WebSiteServices.WebsiteService service = new SportSite.WebSiteServices.WebsiteService();

			//take into account that sometimes the articles are not valid anymore..
			int[] arrZooZooArticles = this.Articles;
			SportSite.WebSiteServices.ArticleData currentArticle;
			for (int i = 0; i < arrZooZooArticles.Length; i++)
			{
				//got enough?
				if (articles.Count >= count)
					break;

				//check current article
				int zooZooArticleId = arrZooZooArticles[i];
				currentArticle = service.GetArticleData(zooZooArticleId);

				//valid?
				if (currentArticle != null && currentArticle.ID == zooZooArticleId)
					articles.Add(currentArticle);
			}
			
			articles.Sort((a1, a2) => a2.Time.CompareTo(a1.Time));
			return articles.ToArray();
		}

		public bool IsZooZooArticle(int id)
		{
			return (Articles.ToList().IndexOf(id) >= 0);
		}

		public void AddZooZooArticle(int id)
		{
			if (!IsZooZooArticle(id))
			{
				List<int> articles = new List<int>(this.Articles);
				articles.Add(id);
				this.Articles = articles.ToArray();
			}
		}

		public void RemoveZooZooArticle(int id)
		{
			if (IsZooZooArticle(id))
			{
				List<int> articles = new List<int>(this.Articles);
				articles.Remove(id);
				this.Articles = articles.ToArray();
			}
		}
		#endregion

		#region "about" text
		public string AboutText
		{
			get
			{
				string text = TryGetConfigValue("About", "RawText");
				if (text.Length > 0)
				{
					text = HttpContext.Current.Server.HtmlDecode(text);
					while (text.IndexOf("&nbsp;&nbsp;") >= 0)
						text = text.Replace("&nbsp;&nbsp;", "&nbsp;");
					text = SportSite.Common.Tools.KeepOriginalFormat(text).Replace("<p>&nbsp;</p><br />", "").Replace("</p><br />", "</p>").Replace("<li><br />", "<li>");
				}
				return text;
			}
			set
			{
				TryWriteConfigValue("About", "RawText", HttpContext.Current.Server.HtmlEncode(value));
			}
		}
		#endregion

		#region Event Gallery
		public readonly string EventGalleryFolderName = Data.AppPath + "/ZooZoo/Images/EventGallery";
		private List<ZooZooEventGalleryAlbum> eventGalleryImageAlbums = new List<ZooZooEventGalleryAlbum>();
		public List<ZooZooEventGalleryAlbum> EventGalleryImageAlbums
		{
			get
			{
				if (eventGalleryImageAlbums.Count == 0)
					ReloadEventGalleryImages();
				return eventGalleryImageAlbums;
			}
		}

		public Dictionary<string, string> UploadEventGalleryImages(int albumId)
		{
			Dictionary<string, string> errors = new Dictionary<string, string>();
			HttpRequest request = HttpContext.Current.Request;
			HttpServerUtility server = HttpContext.Current.Server;
			for (int i = 0; i < request.Files.Count; i++)
			{
				HttpPostedFile oCurFile = request.Files[i];
				string fileName = Path.GetFileName(oCurFile.FileName);
				if (Common.Tools.IsValidImage(oCurFile.InputStream))
				{
					oCurFile.SaveAs(MapEventGalleryImageName(fileName));
					//configManager.WriteValue("EventGalleryText", fileName, server.HtmlEncode(request.Form["EventGalleryText_" + i] + ""));
				}
				else
				{
					errors.Add(fileName, "קובץ תמונה לא חוקי");
				}
			}
			return errors;
		}

		public int AddEventGalleryAlbum(string title, string description, out string strError)
		{
			return AddOrUpdateEventGalleryAlbum(0, title, description, out strError);
		}

		public bool UpdateEventGalleryAlbum(int albumId, string title, string description, out string strError)
		{
			strError = string.Empty;
			return albumId > 0 && AddOrUpdateEventGalleryAlbum(albumId, title, description, out strError) == albumId;
		}

		public bool DeleteEventGalleryImage(int albumId, string pictureName)
		{
			ZooZooEventGalleryAlbum existingAlbum = this.EventGalleryImageAlbums.Find(album => album.Id.Equals(albumId));
			if (existingAlbum != null)
			{
				string nonFullPicName = pictureName.Substring(pictureName.LastIndexOf("/") + 1);
				List<int> arrExistingAlbumImages = GetEventGalleryConfigValue(albumId, "Images").SplitToNumbers(',').ToList();
				int existingImageID = arrExistingAlbumImages.DefaultIfEmpty(0).FirstOrDefault(id =>
				{
					return TryGetConfigValue("EventGallery", "Image_" + id, false).Equals(nonFullPicName, StringComparison.CurrentCultureIgnoreCase);
				});
				if (existingImageID > 0)
				{
					arrExistingAlbumImages.Remove(existingImageID);
					SetEventGalleryConfigValue(albumId, "Images", string.Join(",", arrExistingAlbumImages.ToArray()));
					TryWriteConfigValue("EventGallery", "Image_" + existingImageID, "");
					File.Delete(HttpContext.Current.Server.MapPath(pictureName));
					return true;
				}
			}
			return false;
		}

		public bool DeleteEventGalleryAlbum(int albumId)
		{
			List<ZooZooEventGalleryAlbum> existingAlbums = this.EventGalleryImageAlbums;
			if (existingAlbums.Exists(album => album.Id.Equals(albumId)))
			{
				List<string> arrAlbumsID = existingAlbums.ConvertAll(album => album.Id.ToString());
				arrAlbumsID.Remove(albumId.ToString());
				TryWriteConfigValue("EventGallery", "Albums", string.Join(",", arrAlbumsID.ToArray()));
				ReloadEventGalleryImages();
				return true;
			}

			return false;
		}

		public void ReloadEventGalleryImages()
		{
			eventGalleryImageAlbums.Clear();
			HttpServerUtility server = HttpContext.Current.Server;
			List<string> fileNames = Directory.GetFiles(server.MapPath(EventGalleryFolderName)).ToList().FindAll(filePath =>
			{
				return validGalleryFileTypes.IndexOf(Path.GetExtension(filePath).ToLower()) >= 0;
			}).ConvertAll(filePath => Path.GetFileName(filePath));

			TryGetConfigValue("EventGallery", "Albums", true).SplitRemoveBlank(',').ToList().ConvertAll(item => Int32.Parse(item)).ForEach(albumId =>
			{
				string description = GetEventGalleryConfigValue(albumId, "Description");
				string title = GetEventGalleryConfigValue(albumId, "Title");
				int coverImageId = Int32.Parse("0" + GetEventGalleryConfigValue(albumId, "Cover"));
				ZooZooEventGalleryAlbum album = new ZooZooEventGalleryAlbum { Id = albumId, Title = title, Description = description };
				GetEventGalleryConfigValue(albumId, "Images").SplitToNumbers(',').ToList().ForEach(id =>
				{
					string curImageName = TryGetConfigValue("EventGallery", "Image_" + id, false);
					if (curImageName.Length > 0 && fileNames.IndexOf(curImageName) >= 0)
					{
						string fullName = EventGalleryFolderName + "/" + curImageName;
						album.Images.Add(fullName);
						fileNames.Remove(curImageName);

						if (id.Equals(coverImageId))
							album.CoverImage = fullName;
					}
				});

				if (album.CoverImage.Length == 0 && album.Images.Count > 0)
					album.CoverImage = album.Images[0];

				eventGalleryImageAlbums.Add(album);
			});

			if (fileNames.Count > 0)
			{
				//albumless images..
				ZooZooEventGalleryAlbum album = new ZooZooEventGalleryAlbum { Id = 0, Description = "ללא אלבום" };
				fileNames.ForEach(fileName =>
				{
					string fullName = EventGalleryFolderName + "/" + fileName;
					album.Images.Add(fullName);
				});
				eventGalleryImageAlbums.Add(album);
			}
		}

		protected int AddOrUpdateEventGalleryAlbum(int albumId, string title, string description, out string strError)
		{
			strError = string.Empty;

			if (string.IsNullOrEmpty(title) || title.Trim().Length == 0)
			{
				strError = "כותרת אלבום לא יכולה להיות ריקה";
				return 0;
			}

			//already exists with this title?
			List<ZooZooEventGalleryAlbum> existingAlbums = this.EventGalleryImageAlbums;
			if (existingAlbums.Exists(album => album.Id != albumId && album.Title.Equals(title, StringComparison.CurrentCultureIgnoreCase)))
			{
				strError = "אלבום בעל כותרת זהה כבר קיים";
				return 0;
			}

			if (albumId < 1)
			{
				//add new album
				albumId = existingAlbums.Count > 0 ? existingAlbums.Max(album => album.Id) + 1 : 1;
				List<string> arrAlbumsID = existingAlbums.ConvertAll(album => album.Id.ToString());
				arrAlbumsID.Add(albumId.ToString());
				TryWriteConfigValue("EventGallery", "Albums", string.Join(",", arrAlbumsID.ToArray()));
				SetEventGalleryConfigValue(albumId, "Images", "");
				SetEventGalleryConfigValue(albumId, "Cover", "");
			}
			else
			{
				//make sure album exists
				if (!existingAlbums.Exists(album => album.Id.Equals(albumId)))
				{
					strError = "אלבום בעל זיהוי זה לא נמצא";
					return 0;
				}
			}

			//update existing values
			SetEventGalleryConfigValue(albumId, "Title", title);
			SetEventGalleryConfigValue(albumId, "Description", description);
			ReloadEventGalleryImages();

			return albumId;
		}

		protected string GetEventGalleryConfigValue(int albumId, string caption)
		{
			return TryGetConfigValue("EventGallery", ZooZooEventGalleryAlbum.FormatConfigKey(albumId, caption), false);
		}

		protected bool SetEventGalleryConfigValue(int albumId, string caption, string value)
		{
			return TryWriteConfigValue("EventGallery", ZooZooEventGalleryAlbum.FormatConfigKey(albumId, caption), value);
		}

		protected string MapEventGalleryImageName(string fileName)
		{
			HttpServerUtility server = HttpContext.Current.Server;
			string virtPath = EventGalleryFolderName + "/" + fileName;
			return server.MapPath(virtPath);
		}
		#endregion

		#region Polls
		public static readonly int PollMaxAnswers = 5;
		public static readonly string PollEditHiddenFieldName = "ZooZooPollEditId";
		public ZooZooPoll[] GetPolls(bool includeDeleted)
		{
			List<ZooZooPoll> polls = new List<ZooZooPoll>();
			List<int> arrPollsIdValues = GetExistingIdValues("Polls");
			if (arrPollsIdValues.Count > 0)
			{
				arrPollsIdValues.ForEach(pollId =>
				{
					string rawPollData = TryGetConfigValue("Polls", "Poll_" + pollId, false);
					if (rawPollData.Length > 0)
					{
						ZooZooPoll poll = ZooZooPoll.FromString(rawPollData);
						if (poll != null && !poll.Equals(ZooZooPoll.Empty) && !polls.Contains(poll))
							polls.Add(poll);
					}
				});
			}

			if (!includeDeleted)
				polls.RemoveAll(p => { return p.Deleted; });

			return polls.ToArray();
		}

		public ZooZooPoll GetActivePoll()
		{
			return this.GetPolls(false).ToList().DefaultIfEmpty(ZooZooPoll.Empty).Max();
		}

		public int AddPoll(ZooZooPoll poll)
		{
			//get existing polls:
			List<int> arrPollsIdValues = GetExistingIdValues("Polls");
			//List<ZooZooPoll> existingPolls = this.GetPolls(true).ToList();

			//new poll ID should be the maximum of existing poll plus one
			poll.Id = ((arrPollsIdValues.Count > 0) ? arrPollsIdValues.Max() : 0) + 1;
			//poll.Id = ((existingPolls.Count > 0) ? existingPolls.Max(p => p.Id) : 0) + 1;

			//add to list:
			arrPollsIdValues.Add(poll.Id);

			//write to config..
			TryWriteConfigValue("Polls", "ID", string.Join(",", arrPollsIdValues.ConvertAll(id => id.ToString()).ToArray()));
			TryWriteConfigValue(poll);

			return poll.Id;
		}

		public bool UpdatePoll(ZooZooPoll poll)
		{
			//ignore if poll does not exist
			if (!this.GetPolls(true).ToList().Exists(p => { return p.Id.Equals(poll.Id); }))
				return false;

			//write to config..
			TryWriteConfigValue(poll);

			return true;
		}

		public string GeneratePollForm(ZooZooPoll poll)
		{
			string strHTML = File.ReadAllText(HttpContext.Current.Server.MapPath(pollFormTemplateFile)) + "";
			string id = (poll.Id < 1) ? "new" : poll.Id.ToString();
			string strValueFormat = "$ZooZooPoll_" + id + "_{0}";
			strHTML = strHTML.Replace("$id", id);
			strHTML = strHTML.Replace(string.Format(strValueFormat, "Question"), poll.Question);
			for (int i = 1; i <= PollMaxAnswers; i++)
			{
				ZooZooPoll.Answer answer = poll.Answers.GetById(i);
				strHTML = strHTML.Replace(string.Format(strValueFormat, "Answer" + i), answer.Text);
				strHTML = strHTML.Replace(string.Format(strValueFormat, "Votes" + i), answer.Votes.ToString());
			}

			strHTML = strHTML.Replace(string.Format(strValueFormat, "Deleted"), poll.Deleted ? "checked=\"checked\"" : "");
			strHTML = strHTML.Replace("$HideWhenNew", poll.Id < 1 ? "style=\"display: none;\"" : "");
			return strHTML;
		}

		public string GeneratePollVoteFormFromTemplate(ZooZooPoll poll, string questionTemplate, string answerTemplate, int barTotalWidth)
		{
			string strQuestionHTML = questionTemplate.Replace("$question", poll.Question);
			StringBuilder strAnswersHTML = new StringBuilder();
			int totalVotes = poll.Answers.Sum(answer => answer.Votes);
			double barPixelsPerPercent = ((double)barTotalWidth / 100);
			int percentageCounter = 0;
			poll.Answers.ForEach(answer =>
			{
				int curVotePercentage = (totalVotes > 0) ? (int)(((answer.Votes / (double)totalVotes) * 100) + 0.5) : 0;
				if ((curVotePercentage + percentageCounter) > 100)
					curVotePercentage = (100 - percentageCounter);
				int curFullPixels = (int)((double)curVotePercentage * barPixelsPerPercent);
				string curAnswerHTML = answerTemplate.Replace("$answer", answer.Text);
				curAnswerHTML = curAnswerHTML.Replace("$votes_percentage", curVotePercentage.ToString() + "%");
				curAnswerHTML = curAnswerHTML.Replace("$bar_full_pixels", curFullPixels.ToString());
				curAnswerHTML = curAnswerHTML.Replace("$bar_empty_pixels", (barTotalWidth - curFullPixels).ToString());
				curAnswerHTML = curAnswerHTML.Replace("$vote_text_class", curVotePercentage < 100 ? "VoteText" : "VoteTextSmall");
				strAnswersHTML.Append(curAnswerHTML);
				if (percentageCounter < 100)
					percentageCounter += curVotePercentage;
			});
			return strQuestionHTML + strAnswersHTML.ToString();
		}

		public ZooZooPoll GetZooZooPollFromRequest(int id)
		{
			HttpRequest request = HttpContext.Current.Request;
			string format = "ZooZooPoll_" + ((id < 1) ? "new" : id.ToString()) + "_{0}";
			ZooZooPoll poll = new ZooZooPoll(request.Form[string.Format(format, "Question")] + "");
			for (int i = 1; i <= ZooZooManager.PollMaxAnswers; i++)
			{
				string strCurAnswerText = request.Form[string.Format(format, "Answer" + i)] + "";
				if (strCurAnswerText.Length > 0)
					poll.AddAnswer(strCurAnswerText);
			}
			if (id > 0)
				poll.Deleted = (request.Form[string.Format(format, "Delete")] + "").Equals("1");
			return poll;
		}
		#endregion

		#region Events / Street Games
		public static readonly string EventEditHiddenFieldName = "ZooZooEventEditId";
		public ZooZooEvent[] GetEventsOrStreetGames(bool includeDeleted, bool blnStreetGames)
		{
			string section = (blnStreetGames) ? "StreetGames" : "Events";
			List<ZooZooEvent> events = new List<ZooZooEvent>();
			List<int> arrIdValues = GetExistingIdValues(section);
			if (arrIdValues.Count > 0)
			{
				arrIdValues.ForEach(id =>
				{
					string rawData = TryGetConfigValue(section, "Event_" + id, false);
					if (rawData.Length > 0)
					{
						ZooZooEvent eve = ZooZooEvent.FromString(rawData);
						if (eve != null && !eve.Equals(ZooZooEvent.Empty) && !events.Contains(eve))
							events.Add(eve);
					}
				});
			}

			if (!includeDeleted)
				events.RemoveAll(e => { return e.Deleted; });

			events.Sort((e1, e2) => e1.Date.CompareTo(e2.Date));

			return events.ToArray();
		}

		public ZooZooEvent[] GetEventsOrStreetGames(bool blnStreetGames)
		{
			return GetEventsOrStreetGames(false, blnStreetGames);
		}

		public int AddEventOrStreetGame(ZooZooEvent eve, bool blnStreetGames)
		{
			string section = (blnStreetGames) ? "StreetGames" : "Events";

			//get existing polls:
			List<int> arrEventsIdValues = GetExistingIdValues(section);

			//new poll ID should be the maximum of existing poll plus one
			eve.Id = ((arrEventsIdValues.Count > 0) ? arrEventsIdValues.Max() : 0) + 1;

			//add to list:
			arrEventsIdValues.Add(eve.Id);

			//write to config..
			TryWriteConfigValue(section, "ID", string.Join(",", arrEventsIdValues.ConvertAll(id => id.ToString()).ToArray()));
			TryWriteConfigValue(eve, section);

			return eve.Id;
		}

		public bool UpdateEventOrStreetGame(ZooZooEvent eve, bool blnStreetGames)
		{
			//ignore if poll does not exist
			if (!this.GetEventsOrStreetGames(true, blnStreetGames).ToList().Exists(e => { return e.Id.Equals(eve.Id); }))
				return false;

			//write to config..
			TryWriteConfigValue(eve, (blnStreetGames) ? "StreetGames" : "Events");

			return true;
		}

		public string GenerateEventOrStreetGameForm(ZooZooEvent eve, bool blnStreetGames)
		{
			string strHTML = File.ReadAllText(HttpContext.Current.Server.MapPath(eventsFormTemplateFile)) + "";
			string id = (eve.Id < 1) ? "new" : eve.Id.ToString();
			string strValueFormat = "$ZooZooEvent_" + id + "_{0}";
			strHTML = strHTML.Replace("$id", id);
			strHTML = strHTML.Replace("$title", blnStreetGames ? "משחק רחוב" : "אירוע");
			DateTime date = (eve.Date.Year < 1900) ? DateTime.Now : eve.Date;
			strHTML = strHTML.Replace(string.Format(strValueFormat, "Day"), date.Day.ToString());
			strHTML = strHTML.Replace(string.Format(strValueFormat, "Month"), date.Month.ToString());
			strHTML = strHTML.Replace(string.Format(strValueFormat, "Year"), date.Year.ToString());
			strHTML = strHTML.Replace(string.Format(strValueFormat, "Description"), eve.Description.Replace("<br />", "\n"));
			strHTML = strHTML.Replace(string.Format(strValueFormat, "Title"), eve.Title);
			strHTML = strHTML.Replace(string.Format(strValueFormat, "Deleted"), eve.Deleted ? "checked=\"checked\"" : "");
			strHTML = strHTML.Replace("$HideWhenNew", eve.Id < 1 ? "style=\"display: none;\"" : "");
			return strHTML;
		}

		public ZooZooEvent GetEventOrStreetGameFromRequest(int id)
		{
			HttpRequest request = HttpContext.Current.Request;
			string format = "ZooZooEvent_" + ((id < 1) ? "new" : id.ToString()) + "_{0}";
			ZooZooEvent eve = new ZooZooEvent();
			DateTime date;
			if (TryParseDateTime(request.Form[string.Format(format, "Day")] + "", request.Form[string.Format(format, "Month")] + "", request.Form[string.Format(format, "Year")] + "", out date))
				eve.Date = date;
			eve.Title = request.Form[string.Format(format, "Title")] + "";
			eve.Description = request.Form[string.Format(format, "Description")] + "";
			if (id > 0)
				eve.Deleted = (request.Form[string.Format(format, "Delete")] + "").Equals("1");
			return eve;
		}
		#endregion

		#region Weekly Game
		public string WeeklyGamePicture
		{
			get
			{
				string currentPicture = TryGetConfigValue("WeeklyGame", "CurrentPicture", true);
				return currentPicture.Length > 0 ? string.Format("{0}/{1}", weeklyGamePictureFolder, currentPicture) : string.Empty;
			}
		}

		public string UpdateWeeklyGamePicture(HttpPostedFile oPictureFile)
		{
			string fileName = "ZooZooWeeklyGame" + Path.GetExtension(oPictureFile.FileName);
			try
			{
				string filePath = HttpContext.Current.Server.MapPath(string.Format("{0}/{1}", weeklyGamePictureFolder, fileName));
				oPictureFile.SaveAs(filePath);
			}
			catch (Exception ex)
			{
				return "שגיאה כללית בעת שמירת נתונים:<br />" + ex.ToString().Replace("\n", "<br />");
			}
			TryWriteConfigValue("WeeklyGame", "CurrentPicture", fileName);
			return string.Empty;
		}

		public void DeleteWeeklyGamePicture()
		{
			TryWriteConfigValue("WeeklyGame", "CurrentPicture", "");
		}
		#endregion

		#region Wrote On Us
		public string WroteOnUsPicture
		{
			get
			{
				string currentPicture = TryGetConfigValue("WroteOnUs", "CurrentPicture", true);
				return currentPicture.Length > 0 ? string.Format("{0}/{1}", wroteOnUsPictureFolder, currentPicture) : string.Empty;
			}
		}

		public string UpdateWroteOnUsPicture(HttpPostedFile oPictureFile)
		{
			string fileName = "WroteOnUs" + Path.GetExtension(oPictureFile.FileName);
			try
			{
				string filePath = HttpContext.Current.Server.MapPath(string.Format("{0}/{1}", wroteOnUsPictureFolder, fileName));
				oPictureFile.SaveAs(filePath);
			}
			catch (Exception ex)
			{
				return "שגיאה כללית בעת שמירת נתונים:<br />" + ex.ToString().Replace("\n", "<br />");
			}
			TryWriteConfigValue("WroteOnUs", "CurrentPicture", fileName);
			return string.Empty;
		}

		public void DeleteWroteOnUsPicture()
		{
			TryWriteConfigValue("WroteOnUs", "CurrentPicture", "");
		}
		#endregion

		#region Contact Us
		public string[] ContactUsRecepients
		{
			get
			{
				return TryGetConfigValue("ContactUs", "Recepients", true).SplitRemoveBlank(',');
			}
			set
			{
				TryWriteConfigValue("ContactUs", "Recepients", string.Join(",", value));
			}
		}

		public string ContactUsMailSubject
		{
			get
			{
				string subject = TryGetConfigValue("ContactUs", "Subject", false) + "";
				return subject.Length > 0 ? subject : "טופס יצירת קשר מאתר זוזו";
			}
			set
			{
				TryWriteConfigValue("ContactUs", "Subject", value);
			}
		}

		public string SendContactUsMessage(string name, string email, string subject, string message)
		{
			string filePath = HttpContext.Current.Server.MapPath(contactUsTemplateFile);
			if (!File.Exists(filePath))
				return "Template file '" + contactUsTemplateFile + "' does not exist!";

			string[] arrSendTo = this.ContactUsRecepients;
			if (arrSendTo.Length == 0)
				return "Nobody to send to, aborting";

			DateTime now = DateTime.Now;
			string strHTML = File.ReadAllText(filePath);
			strHTML = strHTML.Replace("$date", now.ToString("dd/MM/yyyy"));
			strHTML = strHTML.Replace("$time", now.ToString("HH:mm:ss"));
			strHTML = strHTML.Replace("$name", name);
			strHTML = strHTML.Replace("$email", email);
			strHTML = strHTML.Replace("$subject", subject);
			strHTML = strHTML.Replace("$message", message);

			string strSentFrom = string.Format("{0}<{1}>", name, (email.Length == 0) ? "noreply@zoozoo.org.il" : email);
			try
			{
				SportSite.Common.Tools.SendEmail(strSentFrom, string.Join(";", arrSendTo), this.ContactUsMailSubject, strHTML);
			}
			catch (Exception ex)
			{
				return "Error while sending: " + ex.ToString();
			}

			return string.Empty;
		}
		#endregion

		#region Log
		public void ReloadBrowsersMapping()
		{
			browsersMapping = MapLogDataFile(HttpContext.Current.Server.MapPath(logBrowsersFileName));
		}

		public void ReloadResolutionsMapping()
		{
			resolutionsMapping = MapLogDataFile(HttpContext.Current.Server.MapPath(logResolutionsFileName));
		}

		public string LogFilePath(int maxLogFileLines)
		{
			if (logFolderPath.Length == 0)
			{
				logFolderPath = HttpContext.Current.Server.MapPath(logFolderName);
				if (!Directory.Exists(logFolderPath))
					Directory.CreateDirectory(logFolderPath);
			}
			if (logFilePath.Length == 0)
			{
				//read from file
				string currentLogFileName = HttpContext.Current.Server.MapPath(logCurrentLogFileName);
				lock (logLocker)
				{
					try
					{
						logFilePath = File.ReadAllText(currentLogFileName);
					}
					catch
					{ }
				}
				if (string.IsNullOrEmpty(logFilePath) || !File.Exists(logFilePath))
					logFilePath = GenerateNewLogFilePath();
			}

			if (this.LogFileLines >= maxLogFileLines)
				logFilePath = GenerateNewLogFilePath();

			return logFilePath;
		}

		public int LogFileLines
		{
			get
			{
				if (logFileLines <= 0)
				{
					lock (logLocker)
					{
						try
						{
							logFileLines = File.ReadAllLines(logFilePath).Length;
						}
						catch
						{ }
					}
				}

				return logFileLines;
			}
		}

		public void Log(ZooZooPage page, int maxLogFileLines)
		{
			string filePath = this.LogFilePath(maxLogFileLines);

			//browser and resolution
			int browserLogId = GetBrowserId();
			//int resolutionLogId = GetResolutionId();

			//string line = string.Format("{0}|{1}|{2}|{3}", this.GenerateTimeStamp(), (int)page, browserLogId, resolutionLogId);
			string line = string.Format("{0}|{1}|{2}", this.GenerateTimeStamp(), (int)page, browserLogId);
			bool success = false;
			lock (logLocker)
			{
				try
				{
					File.AppendAllLines(filePath, new string[] { line });
					success = true;
				}
				catch
				{ }
			}

			if (success)
				logFileLines = this.LogFileLines + 1;
		}
		#endregion

		#region private helpers/methods
		private void KeepConfigUpdated()
		{
			if (configLastRefresh.Year < 1900 || (DateTime.Now - configLastRefresh).TotalSeconds >= 60)
			{
				configManager = new IniFile(configManager.FileName);
				configLastRefresh = DateTime.Now;
			}
		}

		private string GenerateNewLogFilePath()
		{
			string strNewPath = Path.Combine(logFolderPath, GenerateTimeStamp("_") + ".txt");
			string currentLogFileName = HttpContext.Current.Server.MapPath(logCurrentLogFileName);
			lock (logLocker)
			{
				try
				{
					File.WriteAllText(currentLogFileName, strNewPath);
				}
				catch
				{ }
			}
			logFileLines = 0;
			return strNewPath;
		}

		private int GetBrowserId()
		{
			if (browsersMapping.Count == 0)
				ReloadBrowsersMapping();

			string browser = string.Empty;
			try
			{
				HttpBrowserCapabilities capabilities = HttpContext.Current.Request.Browser;
				browser = string.Format("{0} {1}.{2}", capabilities.Browser, capabilities.MajorVersion, capabilities.MinorVersionString);
			}
			catch
			{ }

			if (browser.Length > 0)
			{
				if (browsersMapping.ContainsKey(browser))
					return browsersMapping[browser];

				int id = browsersMapping.Values.ToList().DefaultIfEmpty(0).Max() + 1;
				string line = string.Format("{0}|{1}", id, browser);
				string filePath = HttpContext.Current.Server.MapPath(logBrowsersFileName);
				lock (logLocker)
				{
					try
					{
						File.AppendAllLines(filePath, new string[] { line });
					}
					catch
					{ }
				}
				ReloadBrowsersMapping();
				return id;
			}

			return 0;
		}

		private int GetResolutionId()
		{
			if (resolutionsMapping.Count == 0)
				ReloadResolutionsMapping();

			string resolution = string.Empty;
			try
			{
				HttpBrowserCapabilities capabilities = HttpContext.Current.Request.Browser;
				resolution = string.Format("{0}x{1}", capabilities.ScreenPixelsWidth, capabilities.ScreenPixelsHeight);
			}
			catch
			{ }

			if (resolution.Length > 0)
			{
				if (resolutionsMapping.ContainsKey(resolution))
					return resolutionsMapping[resolution];

				int id = resolutionsMapping.Values.ToList().DefaultIfEmpty(0).Max() + 1;
				string line = string.Format("{0}|{1}", id, resolution);
				string filePath = HttpContext.Current.Server.MapPath(logResolutionsFileName);
				lock (logLocker)
				{
					try
					{
						File.AppendAllLines(filePath, new string[] { line });
					}
					catch
					{ }
				}
				ReloadResolutionsMapping();
				return id;
			}

			return 0;
		}

		private Dictionary<string, int> MapLogDataFile(string filePath)
		{
			Dictionary<string, int> map = new Dictionary<string, int>();
			string[] lines = null;
			lock (logLocker)
			{
				try
				{
					lines = File.ReadAllLines(filePath);
				}
				catch
				{ }
			}
			if (lines != null && lines.Length > 0)
			{
				lines.ToList().ConvertAll(line => line.SplitRemoveBlank('|')).ForEach(parts =>
				{
					if (parts.Length == 2)
					{
						string value = parts[1];
						if (value.Length > 0)
						{
							int id;
							if (Int32.TryParse(parts[0], out id) && id > 0 && !map.ContainsKey(value))
								map.Add(value, id);
						}
					}
				});
			}
			return map;
		}

		private string GenerateTimeStamp(string separator)
		{
			return DateTime.Now.ToString(string.Format("dd{0}MM{0}yyyy{0}HH{0}mm{0}ss", separator));
		}

		private string GenerateTimeStamp()
		{
			return GenerateTimeStamp(string.Empty);
		}

		private bool TryParseDateTime(string day, string month, string year, out DateTime date)
		{
			date = DateTime.MinValue;
			int dayVal, monthVal, yearVal;
			if (Int32.TryParse(day, out dayVal) && Int32.TryParse(month, out monthVal) && Int32.TryParse(year, out yearVal))
			{
				try
				{
					date = new DateTime(yearVal, monthVal, dayVal);
					return true;
				}
				catch
				{ }
			}
			return false;
		}

		private List<int> GetExistingIdValues(string section)
		{
			string rawValue = TryGetConfigValue(section, "ID");
			return rawValue.SplitRemoveBlank(',').ToList().ConvertAll(id => { return Int32.Parse(id); });
		}

		public int GetElementToEditId(List<ZooZooElement> existingElements, string hiddenFieldName)
		{
			string strElementId = HttpContext.Current.Request.Form[hiddenFieldName] + "";
			if (strElementId.Length > 0)
			{
				int elementId;
				if (Int32.TryParse(strElementId, out elementId) && elementId > 0 && existingElements.Exists(e => e.Id.Equals(elementId)))
					return elementId;
			}
			return -1;
		}

		private string TryGetConfigValue(string section, string key, bool keepUpdated)
		{
			string value = string.Empty;
			if (keepUpdated)
				KeepConfigUpdated();
			try
			{
				value = configManager.ReadValue(section, key) + "";
			}
			catch
			{ }
			return value;
		}

		private string TryGetConfigValue(string section, string key)
		{
			return TryGetConfigValue(section, key, true);
		}

		private string[] TryGetConfigValues(string section, string[] keys)
		{
			string[] values = new string[keys.Length];
			for (int i = 0; i < keys.Length; i++)
				values[i] = TryGetConfigValue(section, keys[i], (i == 0));
			return values;
		}

		private bool TryWriteConfigValue(string section, string key, string value)
		{
			bool success = false;
			try
			{
				configManager.WriteValue(section, key, value);
				success = true;
			}
			catch
			{ }
			return success;
		}

		private bool TryWriteConfigValue(ZooZooPoll poll)
		{
			return TryWriteConfigValue("Polls", "Poll_" + poll.Id, poll.ToString());
		}

		private bool TryWriteConfigValue(ZooZooEvent eve, string section)
		{
			return TryWriteConfigValue(section, "Event_" + eve.Id, eve.ToString());
		}
		#endregion
	}
}