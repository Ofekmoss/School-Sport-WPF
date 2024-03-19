using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Web;
using System.Web.Services;
using System.Data.SqlClient;
using Sport.Core;
using System.Collections.Generic;
using ISF.DataLayer;
using Sport.Common;
using System.Linq;
using System.Text;

namespace SportServices
{
	/// <summary>
	/// Provides variety of services which are website specific, such as articles, 
	/// web site pages or attachments. all services which only give information
	/// are public and free, all services which update information are protected.
	/// </summary>
	public class WebsiteService : System.Web.Services.WebService
	{
		#region static readonly members
		public static readonly char DefaultSeperator = ',';
		public static readonly char LinkSeperator = ((char)236);
		public static readonly char LinkTextSeperator = ((char)237);
		#endregion

		#region structs and enumerations
		public struct MatchData
		{
			public int Category;
			public int Phase;
			public int Group;
			public int Round;
			public int Cycle;
			public int Match;
			public int TeamA;
			public int TeamB;
			public DateTime Time;
			public double ScoreA;
			public double ScoreB;
			public int Sender;
			public int Outcome;
			public string PartsResult;
		}

		public struct PendingCompetitorData
		{
			public int ID;
			public int Player;
			public int ChampionshipCategory;
			public int Competition;
			public int Heat;
			public int Team;
			public string Phase;
			public string Group;
		}

		public struct AttachmentData
		{
			public int ID;
			public string fileName;
			public string Description;
			public DateTime Time;
			public string SentBy;

			public override string ToString()
			{
				return ID.ToString();
			}
		}

		public struct LinkData
		{
			public string URL;
			public string Text;
			public override string ToString()
			{
				string result = URL;
				if ((Text != null) && (Text.Length > 0))
				{
					result += LinkTextSeperator + Text;
				}
				return result;
			}

		}

		public struct FlashNewsData
		{
			public int ID;
			public DateTime Time;
			public string User;
			public string Contents;
			public LinkData[] Links;
		}

		public class ArticleData
		{
			public int ID;
			public DateTime Time;
			public string User;
			public string Contents;
			public string[] Images;
			public string File;
			public string Caption;
			public string SubCaption;
			public bool IsPrimary;
			public bool IsSub;
			public bool IsHotLink;
			public LinkData[] Links;
			public AttachmentData[] Attachments;
			public int ArticleRegion;
			public bool IsClubsArticle;
			public string AuthorName;
			public string[] ImageDescriptions;
			public DateTime LastModified = DateTime.MinValue;
		}

		public struct PageData
		{
			public int ID;
			public string Caption;
			public string Contents;
			public string AuthorName;
			public string AuthorTitle;
			public AttachmentData[] Attachments;
			public int Index;
			public LinkData[] Links;
		}

		public struct RegionData
		{
			public int ID;
			public string Name;
			public string Email;
		}

		public struct PollResult
		{
			public int ID;
			public string visitorIp;
			public string sessionId;
		}

		public struct PollAnswerData
		{
			public static PollAnswerData Empty;

			static PollAnswerData()
			{
				Empty = new PollAnswerData();
				Empty.ID = -1;
				Empty.pollId = -1;
				Empty.answer = null;
				Empty.results = null;
			}

			public int ID;
			public int pollId;
			public string answer;
			public PollResult[] results;
		}

		public class PollData
		{
			public static PollData Empty;

			private PollData()
			{
				this.ID = -1;
			}

			static PollData()
			{
				Empty = new PollData();
				Empty.ID = -1;
				Empty.question = null;
				Empty.creationDate = DateTime.MinValue;
				Empty.experationDate = DateTime.MinValue;
				Empty.possibleAnswers = null;
				Empty.creator = -1;
			}

			public int ID;
			public string question;
			public PollAnswerData[] possibleAnswers;
			public DateTime creationDate;
			public DateTime experationDate;
			public int creator;

			public PollData(SimpleRow dataRow)
			{
				this.ID = (int)dataRow["POLL_ID"];
				this.question = dataRow["POLL_QUESTION"].ToString();
				this.creationDate = (DateTime)dataRow["POLL_SUBMIT_DATE"];
				this.experationDate = (DateTime)dataRow["POLL_EXPIRE_DATE"];
				this.possibleAnswers = this.getPollAnswerDataArray(this.ID);
				this.creator = dataRow.GetIntOrDefault("CREATOR", -1);
			}

			public override int GetHashCode()
			{
				return this.ID.GetHashCode();
			}

			public override bool Equals(object obj)
			{
				if (obj is PollData)
					return this.ID.Equals(((PollData)obj).ID);
				return false;
			}

			/// <summary>
			/// Returns the array of answers related to a specific poll (given by the ID)
			/// If the ID = -1, then an array of all possible answers to all polls is returned.
			/// </summary>
			/// <param name="ID">The ID of the poll</param>
			/// <returns></returns>
			private PollAnswerData[] getPollAnswerDataArray(int ID)
			{
				List<PollAnswerData> answers = new List<PollAnswerData>();

				string strSQL = "SELECT ANSWER_ID,POLL_ID,ANSWER " + 
					"FROM WEBSITE_POLLS_POSSIBLE_ANSWERS " + 
					"WHERE ARCHIVE_DATE IS NULL";
				List<SimpleParameter> parameters = new List<SimpleParameter>();
				if (ID >= 0)
				{
					strSQL += " AND POLL_ID = @1";
					parameters.Add(new SimpleParameter("@1", ID));
				}
				SimpleTable table = DB.Instance.GetDataBySQL(strSQL, parameters.ToArray());
				table.Rows.ForEach(row =>
				{
					PollAnswerData currentAnswerData = new PollAnswerData();
					currentAnswerData.ID = (int)row["ANSWER_ID"];
					currentAnswerData.pollId = (int)row["POLL_ID"];
					currentAnswerData.answer = row["ANSWER"].ToString();
					currentAnswerData.results = new PollResult[] { };
					answers.Add(currentAnswerData);
				});
				
				strSQL = "SELECT RESULT_ID,ANSWER_ID,VISITOR_IP,SESSION_ID " + 
					"FROM WEBSITE_POLLS_RESULTS " + 
					"WHERE ANSWER_ID = @1";
				SimpleParameter answerParam = new SimpleParameter("@1", -1);
				for (int i = 0; i < answers.Count; i++)
				{
					PollAnswerData answer = answers[i];
					List<PollResult> results = new List<PollResult>();
					answerParam.Value = answer.ID;
					table = DB.Instance.GetDataBySQL(strSQL, answerParam);
					table.Rows.ForEach(row =>
					{
						PollResult currentResult = new PollResult();
						currentResult.ID = (int)row["RESULT_ID"];
						currentResult.visitorIp = row["VISITOR_IP"].ToString();
						currentResult.sessionId = row["SESSION_ID"].ToString();
						results.Add(currentResult);
					});
					answer.results = results.ToArray();
					answers[i] = answer;
				};
				
				return answers.ToArray();
			}
		}

		public struct BannerData
		{
			public int ID;
			public string url_150_150;
			public string url_760_80;
			public string description;
		}

		public struct EventData
		{
			public int ID;
			public DateTime Date;
			public string Description;
			public string URL;
		}

		public class ImageData
		{
			public int ID = -1;
			public string PictureName = "";
			public string OriginalName = "";
			public string GroupName = "";
			public string Description = "";
			public int ViewCount = 0;
			public string SubGroup = "";
		}

		public class ArticleCommentData
		{
			public int ID = -1;
			public int Article = -1;
			public int Number = 0;
			public string VisitorIP = "";
			public string VisitorName = "";
			public string VisitorEmail = "";
			public string Caption = "";
			public string Contents = "";
			public DateTime DatePosted = DateTime.MinValue;
			public bool Deleted = false;
		}

		public enum WebSitePage
		{
			Unknown = 0,
			Main = 1,
			Register = 2,
			Results = 3,
			PlayerSelectionDialog = 5,
			SchoolSelectionDialog = 6,
			Gallery = 7,
			SendToFriend = 8,
			AddComment = 9
		}
		#endregion

		#region construction
		public WebsiteService()
		{
			//CODEGEN: This call is required by the ASP.NET Web Services Designer
			InitializeComponent();
		}
		#endregion

		#region Component Designer generated code

		//Required by the Web Services Designer 
		private IContainer components = null;

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#endregion

		#region FlashNews
		#region Get
		/// <summary>
		/// returns list of all flash news available for today.
		/// </summary>
		[WebMethod(EnableSession = true)]
		public FlashNewsData[] GetCurrentNews()
		{
			return GetFlashNews(-1, DateTime.Today, DateTime.Today);
		}

		[WebMethod]
		public FlashNewsData[] GetNewsInRange(DateTime start, DateTime end)
		{
			return GetFlashNews(-1, start, end);
		}

		/// <summary>
		/// returns the desired flashnews data.
		/// </summary>
		[WebMethod(EnableSession = true)]
		public FlashNewsData GetFlashnewsData(int id)
		{
			FlashNewsData[] arrNews = GetFlashNews(id, DateTime.MinValue, DateTime.MinValue);
			FlashNewsData result = new FlashNewsData();
			result.ID = -1;
			if ((arrNews != null) && (arrNews.Length > 0))
				result = arrNews[0];
			return result;
		}

		private FlashNewsData[] GetFlashNews(int id, DateTime start, DateTime end)
		{
			//build sql statement and parameters:
			string strSQL = "SELECT n.FLASH_NEWS_ID, n.SUBMISSION_TIME, " +
				"n.NEWS_DESCRIPTION, n.FLASH_NEWS_LINKS, u.USER_FIRST_NAME, " +
				"u.USER_LAST_NAME " +
				"FROM (ISF_FLASH_NEWS n INNER JOIN USERS u ON n.SUBMITTED_BY=u.USER_ID)";

			//filters:
			List<string> filters = new List<string>();
			List<SimpleParameter> parameters = new List<SimpleParameter>();
			if (id >= 0)
			{
				filters.Add("n.FLASH_NEWS_ID=@id");
				parameters.Add(new SimpleParameter("@id", id));
			}
			if (start.Year > 1900)
			{
				filters.Add("n.SUBMISSION_TIME>=@start");
				parameters.Add(new SimpleParameter("@start", Sport.Common.Tools.SetTime(start, 0, 0, 0)));
			}
			if (end.Year > 1900)
			{
				filters.Add("n.SUBMISSION_TIME<=@end");
				parameters.Add(new SimpleParameter("@end", Sport.Common.Tools.SetTime(end, 23, 59, 59)));
			}
			if (filters.Count > 0)
			{
				strSQL += " WHERE ";
				for (int i = 0; i < filters.Count; i++)
				{
					strSQL += filters[i];
					if (i < (filters.Count - 1))
						strSQL += " AND ";
				}
			}
			strSQL += " ORDER BY n.SUBMISSION_TIME DESC";

			//read results:
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL, parameters.ToArray());
			List<FlashNewsData> result = new List<FlashNewsData>();
			table.Rows.ForEach(row =>
			{
				//add current record.
				FlashNewsData data = new FlashNewsData();
				data.ID = row.GetIntOrDefault("FLASH_NEWS_ID", -1);
				data.Time = (DateTime)row["SUBMISSION_TIME"];
				data.User = row["USER_FIRST_NAME"].ToString() + " " +
					row["USER_LAST_NAME"].ToString();
				data.Contents = row["NEWS_DESCRIPTION"].ToString();
				data.Links = StringToLinks(row["FLASH_NEWS_LINKS"].ToString());
				result.Add(data);
			});

			//done.
			return result.ToArray();
		} //end function GetFlashNews
		#endregion

		/*
			if (Session[SessionService.SessionKey_UserID] == null)
				throw new Exception("can't update news: session expired.");
		*/

		#region Update
		/// <summary>
		/// inserts new flash news record or updates existing flash news.
		/// </summary>
		[WebMethod]
		public int UpdateFlashNews(FlashNewsData data,
			string username, string password, int userid)
		{
			//verify authorized user:
			VerifyAuthorizedUser(username, password, true, "update news");

			//build sql statement and parameters:
			string strSQL = "";
			List<SimpleParameter> parameters = new List<SimpleParameter>();
			parameters.Add(new SimpleParameter("@id", data.ID));
			parameters.Add(new SimpleParameter("@nd", data.Contents));
			parameters.Add(new SimpleParameter("@sb", userid));
			if (data.ID < 0)
			{
				//insert
				strSQL = "INSERT INTO ISF_FLASH_NEWS (";
				strSQL += "NEWS_DESCRIPTION, SUBMITTED_BY, FLASH_NEWS_LINKS";
				if (data.Time.Year > 1900)
					strSQL += ", SUBMISSION_TIME";
				strSQL += ") VALUES (@nd, @sb, @fnl";
				if (data.Time.Year > 1900)
				{
					strSQL += ", @time";
					parameters.Add(new SimpleParameter("@time", data.Time));
				}
				strSQL += ")";
			}
			else
			{
				//update
				strSQL = "UPDATE ISF_FLASH_NEWS SET NEWS_DESCRIPTION=@nd, ";
				strSQL += "SUBMITTED_BY=@sb, FLASH_NEWS_LINKS=@fnl ";
				strSQL += "WHERE FLASH_NEWS_ID=@id";
			}

			//flashnews links:
			string[] arrLinks = Sport.Common.Tools.ToStringArray(data.Links);
			if (Sport.Common.Tools.IsArrayEmpty(arrLinks))
				parameters.Add(new SimpleParameter("@fnl", System.DBNull.Value));
			else
				parameters.Add(new SimpleParameter("@fnl", string.Join(LinkSeperator.ToString(), arrLinks)));

			//execute and return result:
			return DB.Instance.Execute(strSQL, parameters.ToArray());
		}
		#endregion

		#region Delete
		/// <summary>
		/// delete given record.
		/// </summary>
		[WebMethod]
		public int DeleteFlashNews(int id, string username, string password)
		{
			//verify authorized user:
			VerifyAuthorizedUser(username, password, true, "delete news");

			//build sql statement:
			string strSQL = "DELETE FROM ISF_FLASH_NEWS WHERE FLASH_NEWS_ID=@id";

			//execute and return result:
			return DB.Instance.Execute(strSQL, new SimpleParameter("@id", id));
		}
		#endregion
		#endregion

		#region Polls

		#region Init

		public enum PollReturnFilter
		{
			All = 0,
			Latest,
			Oldest
		}

		#endregion

		#region Get

		/// <summary>
		/// Returns the specified poll.
		/// </summary>
		/// <param name="ID">The poll ID</param>
		/// <returns>Returns the poll as PollData</returns>
		[WebMethod]
		public PollData getPoll(int ID)
		{
			PollData result = PollData.Empty;
			string strSQL = "SELECT POLL_ID,POLL_QUESTION,POLL_SUBMIT_DATE,POLL_EXPIRE_DATE,CREATOR FROM WEBSITE_POLLS " + 
				"WHERE POLL_ID = @1 AND ARCHIVE_DATE IS NULL";
			SimpleRow row;
			if (DB.Instance.GetSingleRow(strSQL, out row,
				new SimpleParameter("@1", ID)))
			{
				result = new PollData(row);
			}
			
			return result;
		}

		/// <summary>
		/// Returns an array of polls selected according to the given filter
		/// </summary>
		/// <returns>Array of PollData objects</returns>
		[WebMethod]
		public PollData[] getPollsByFilter(PollReturnFilter filter)
		{
			List<PollData> polls = new List<PollData>();
			string strSQL = "SELECT POLL_ID,POLL_QUESTION,POLL_SUBMIT_DATE,POLL_EXPIRE_DATE,CREATOR " + 
				"FROM WEBSITE_POLLS WHERE ARCHIVE_DATE IS NULL";
			switch (filter)
			{
				case PollReturnFilter.Latest:
					strSQL += " AND (POLL_EXPIRE_DATE >= GETDATE() OR POLL_EXPIRE_DATE IS NULL) ";
					strSQL += "ORDER BY POLL_SUBMIT_DATE DESC";
					break;
				case PollReturnFilter.Oldest:
					strSQL += " AND (POLL_EXPIRE_DATE >= GETDATE() OR POLL_EXPIRE_DATE IS NULL) ";
					strSQL += "ORDER BY POLL_SUBMIT_DATE ASC";
					break;
			}

			SimpleTable table = DB.Instance.GetDataBySQL(strSQL);
			foreach (SimpleRow row in table.Rows)
			{
				polls.Add(new PollData(row));
				if (filter != PollReturnFilter.All)
					break;
			};

			return polls.ToArray();
		}

		#endregion

		#region Set

		/// <summary>
		/// Changes the requested answer to the new value.
		/// </summary>
		/// <param name="answerID">The requested answer</param>
		/// <param name="newAnswer">The new value</param>
		/// <returns></returns>
		[WebMethod]
		public int editAnswer(int answerID, string newAnswer,
			string username, string password)
		{
			VerifyAuthorizedUser(username, password, true, "edit poll answer");

			string strSQL = "UPDATE WEBSITE_POLLS_POSSIBLE_ANSWERS " + 
				"SET ANSWER = @1 WHERE ANSWER_ID = @2";
			return DB.Instance.Execute(strSQL,
				new SimpleParameter("@1", newAnswer),
				new SimpleParameter("@2", answerID));
		}

		/// <summary> 
		/// Inserts a poll into the database.
		/// </summary>
		/// <param name="pollQuestion">The question to be presented as the topic of discussion</param>
		/// <param name="expirationDate">Specifies the exact time in which 
		/// this poll will be removed from the web page</param>
		/// <returns></returns>
		[WebMethod(EnableSession = true)]
		public int addPoll(string pollQuestion, DateTime expirationDate,
			string username, string password)
		{
			VerifyAuthorizedUser(username, password, true, "add poll");

			string strSQL = "INSERT INTO WEBSITE_POLLS (POLL_QUESTION, POLL_EXPIRE_DATE) " + 
				"VALUES (@1, @2)";
			return DB.Instance.Execute(strSQL,
				new SimpleParameter("@1", pollQuestion),
				new SimpleParameter("@2", expirationDate));
		}

		/// <summary>
		/// Adds an answer to an existing poll.
		/// This answer will be presented as a possible answer within the poll
		/// </summary>
		/// <param name="pollID">The ID of the poll</param>
		/// <param name="pollAnswer">The actual answer</param>
		/// <returns></returns>
		[WebMethod]
		public int addAnswerToPoll(int pollID, string pollAnswer,
			string username, string password)
		{
			VerifyAuthorizedUser(username, password, true, "add poll answer");

			string strSQL = "INSERT INTO WEBSITE_POLLS_POSSIBLE_ANSWERS (POLL_ID, ANSWER) " + 
				"VALUES (@1, @2)";
			return DB.Instance.Execute(strSQL,
				new SimpleParameter("@1", pollID),
				new SimpleParameter("@2", pollAnswer));
		}

		#endregion

		#region Delete

		[WebMethod]
		public int deleteAnswer(int ID, string username, string password)
		{
			VerifyAuthorizedUser(username, password, true, "delete poll answer");

			PollAnswerData deletedAnswer = getAnswer(ID);
			if (deletedAnswer.Equals(PollAnswerData.Empty))
				return -1;

			PollData ownerPoll = getPoll(deletedAnswer.pollId);
			if (!isCurrentUserAuthorizedToUpdate(ownerPoll.creator, username, password))
				return -1;

			string strSQL = "UPDATE WEBSITE_POLLS_POSSIBLE_ANSWERS " + 
				"SET ARCHIVE_DATE = @1 " + 
				"WHERE ANSWER_ID = @2";
			return DB.Instance.Execute(strSQL,
				new SimpleParameter("@1", DateTime.Now),
				new SimpleParameter("@2", ID));
		}

		/// <summary>
		/// Attempts to delete a poll
		/// </summary>
		/// <param name="ID">The ID of the poll</param>
		/// <returns>Returns -1 if unsuccessful</returns>
		[WebMethod]
		public int deletePoll(int ID, string username, string password)
		{
			VerifyAuthorizedUser(username, password, true, "delete poll");

			PollData deletedPoll = getPoll(ID);
			if (deletedPoll.Equals(PollData.Empty))
				return -1;

			string strSQL = "UPDATE WEBSITE_POLLS " + 
				"SET ARCHIVE_DATE = @1 " + 
				"WHERE POLL_ID = @2";
			return DB.Instance.Execute(strSQL,
				new SimpleParameter("@1", DateTime.Now),
				new SimpleParameter("@2", ID));
		}

		#endregion

		#region Misc

		/// <summary>
		/// This method should be called by the poll control.
		/// The current user is voting for the specified answer.
		/// </summary>
		/// <param name="pollID"></param>
		/// <param name="answerID"></param>
		/// <param name="userIP"></param>
		/// <returns>Returns the result ID if successful, or -1 if not.</returns>
		[WebMethod(EnableSession = true)]
		public int vote(int pollID, int answerID, string userIP)
		{
			// Set a new result
			string strSQL = "INSERT INTO WEBSITE_POLLS_RESULTS (ANSWER_ID, VISITOR_IP, SESSION_ID) " + 
				"VALUES (@1, @2, @3)";
			DB.Instance.Execute(strSQL,
				new SimpleParameter("@1", answerID),
				new SimpleParameter("@2", userIP.ToString()),
				new SimpleParameter("@3", "")); //Session.SessionID

			return DB.Instance.GetMaxValue("WEBSITE_POLLS_RESULTS", "RESULT_ID");
		}

		/// <summary>
		/// Determines if the calling user may update a poll
		/// created by the specified creator
		/// </summary>
		/// <param name="creatorId">The ID of the creator</param>
		/// <returns>true if user is authorized, or false if not</returns>
		[WebMethod]
		public bool isCurrentUserAuthorizedToUpdate(int creatorId,
			string username, string password)
		{
			return true;
			/*
			VerifyAuthorizedUser(username, password, true, "check user authorized");
			
			int currentUserId = (int)(Session[SessionService.SessionKey_UserID]);
			if ((creatorId != currentUserId) && 
				(Common.GetUserPermissions(creatorId) >= Common.GetUserPermissions(currentUserId)))
				return false;
			else
				return true;
			*/
		}

		#endregion

		#region Private

		private PollAnswerData getAnswer(int ID)
		{
			PollAnswerData answerData = PollAnswerData.Empty;

			string strSQL = "SELECT ANSWER_ID,POLL_ID,ANSWER " + 
				"FROM WEBSITE_POLLS_POSSIBLE_ANSWERS " + 
				"WHERE ANSWER_ID = @1";
			SimpleRow dataRow;
			SimpleParameter answerParam = new SimpleParameter("@1", ID);
			if (DB.Instance.GetSingleRow(strSQL, out dataRow, answerParam))
			{
				answerData = new PollAnswerData();
				answerData.ID = (int)dataRow["ANSWER_ID"];
				answerData.pollId = (int)dataRow["POLL_ID"];
				answerData.answer = dataRow["ANSWER"].ToString();
			}

			if (!answerData.Equals(PollAnswerData.Empty))
			{
				strSQL = "SELECT RESULT_ID,ANSWER_ID,VISITOR_IP,SESSION_ID " + 
					"FROM WEBSITE_POLLS_RESULTS WHERE ANSWER_ID = @1";
				SimpleTable table = DB.Instance.GetDataBySQL(strSQL, answerParam);
				List<PollResult> results = new List<PollResult>();
				table.Rows.ForEach(row =>
				{
					PollResult currentResult = new PollResult();
					currentResult.ID = (int)row["RESULT_ID"];
					currentResult.visitorIp = row["VISITOR_IP"].ToString();
					currentResult.sessionId = row["SESSION_ID"].ToString();
					results.Add(currentResult);
				});
				answerData.results = results.ToArray();
			}
			
			return answerData;
		}
		#endregion

		#endregion

		#region Articles
		#region Get
		#region web methods
		/// <summary>
		/// returns list of desired amount of last articles, sorted by date.
		/// </summary>
		[WebMethod(EnableSession = true)]
		public ArticleData[] GetArticles(int articlesCount)
		{
			ArticleData[] articles = GetArticle(-1, false, false, false);
			if (articlesCount < 0)
				articlesCount = articles.Length;
			List<ArticleData> result = new List<ArticleData>();
			for (int i = 0; i < articlesCount; i++)
				result.Add(articles[i]);
			return result.ToArray();
		}

		[WebMethod]
		public ArticleData GetPrimaryArticle()
		{
			ArticleData[] articles = GetArticle(-1, true);
			if (articles.Length > 0)
				return articles[0];

			ArticleData result = new ArticleData();
			result.ID = -1;
			return result;
		}

		[WebMethod]
		public ArticleData[] GetSubArticles()
		{
			return GetArticle(-1, false, true, false);
		}

		[WebMethod]
		public int GetArticlesCount(bool primary, bool sub)
		{
			//build sql statement:
			string strSQL = "SELECT COUNT(ARTICLE_ID) FROM ISF_ARTICLES";
			List<string> conditions = new List<string>();
			if (primary)
				conditions.Add("IS_PRIMARY_ARTICLE=1");
			if (sub)
				conditions.Add("IS_SUB_ARTICLE=1");
			if (conditions.Count > 0)
			{
				strSQL += " WHERE ";
				for (int i = 0; i < conditions.Count; i++)
				{
					strSQL += conditions[i].ToString() + " ";
					if (i < (conditions.Count - 1))
						strSQL += "AND ";
				}
			}

			//read results:
			return (int)DB.Instance.ExecuteScalar(strSQL, 0);
		}

		/// <summary>
		/// returns ID of the most recent flashnews.
		/// </summary>
		[WebMethod(EnableSession = true)]
		public int GetLatestNewsID()
		{
			return DB.Instance.GetMaxValue("ISF_FLASH_NEWS", "FLASH_NEWS_ID");
		}

		/// <summary>
		/// returns ID of the most recent article.
		/// </summary>
		[WebMethod(EnableSession = true)]
		public int GetLatestArticleID()
		{
			return DB.Instance.GetMaxValue("ISF_ARTICLES", "ARTICLE_ID", "DATE_DELETED IS NULL");
		}

		/// <summary>
		/// returns the desired article data.
		/// </summary>
		[WebMethod(EnableSession = true)]
		public ArticleData GetArticleData(int id)
		{
			ArticleData[] articles = GetArticle(id);
			if (articles.Length > 0)
				return articles[0];

			ArticleData result = new ArticleData();
			result.ID = -1;
			return result;
		}

		/// <summary>
		/// returns the article which is defined as hot link, if exists.
		/// </summary>
		[WebMethod]
		public ArticleData GetHotArticle()
		{
			//initialize result article:
			ArticleData result = new ArticleData();
			result.ID = -1;

			//check if hot article is defined:
			string hotArticleID = Common.ReadIniValue("HotArticle", this.Server);
			if (hotArticleID.Length > 0)
			{
				//get articles data:
				ArticleData[] articles = GetArticle(Int32.Parse(hotArticleID), false, false, false);

				//check if we got anything:
				if (articles.Length > 0)
				{
					result = articles[0];
					result.IsHotLink = true;
				}
			}

			//done.
			return result;
		}
		#endregion

		#region private methods
		private Dictionary<string, string> GetImageDescriptions(List<string> imageNames, bool getName)
		{
			Dictionary<string, string> imageDescriptions = new Dictionary<string, string>();
			List<SimpleParameter> parameters = new List<SimpleParameter>();
			if (imageNames.Count > 0)
			{
				StringBuilder sb = new StringBuilder();
				sb.Append("SELECT IMAGE_DESCRIPTION_ID, IMAGE_NAME, IMAGE_DESCRIPTION FROM WEBSITE_IMAGE_DESCRIPTIONS WHERE ");
				for (int i = 0; i < imageNames.Count; i++)
				{
					if (i > 0)
						sb.Append(" OR ");
					string pName = "@image" + i;
					sb.Append("IMAGE_NAME=").Append(pName);
					parameters.Add(new SimpleParameter(pName, imageNames[i]));
				}

				SimpleTable table = DB.Instance.GetDataBySQL(sb.ToString(), parameters.ToArray());
				table.Rows.ForEach(row =>
				{
					string key = row["IMAGE_NAME"].ToString();
					string field = getName ? "IMAGE_DESCRIPTION" : "IMAGE_DESCRIPTION_ID";
					if (!imageDescriptions.ContainsKey(key))
						imageDescriptions.Add(key, row[field].ToString());
				});
			}
			return imageDescriptions;
		}

		private ArticleData[] GetArticle(int articleID,
			bool primaryArticle, bool subArticle, bool fullDetails)
		{
			if (primaryArticle || subArticle || articleID < 0)
				fullDetails = false;

			//build sql statement:
			string strSQL = "SELECT a.ARTICLE_ID, a.SUBMISSION_TIME, a.ARTICLE_DESCRIPTION, a.ARTICLE_FILE_NAME, a.ARTICLE_IMAGE_NAME, " + 
							"	a.ARTICLE_CAPTION, a.ARTICLE_SUB_CAPTION, a.AUTHOR_NAME, u.USER_FIRST_NAME, u.USER_LAST_NAME, " + 
							"	a.IS_PRIMARY_ARTICLE, a.IS_SUB_ARTICLE, a.ARTICLE_LINKS, a.ARTICLE_ATTACHMENTS, a.DATE_LAST_MODIFIED " + 
							"FROM (ISF_ARTICLES a INNER JOIN USERS u ON a.SUBMITTED_BY=u.USER_ID) ";
			List<string> conditions = new List<string>();
			List<SimpleParameter> parameters = new List<SimpleParameter>();
			conditions.Add("a.DATE_DELETED IS NULL");
			if (articleID >= 0)
			{
				conditions.Add("ARTICLE_ID=@id");
				parameters.Add(new SimpleParameter("@id", articleID));
			}
			if (primaryArticle)
				conditions.Add("IS_PRIMARY_ARTICLE=1");
			if (subArticle)
				conditions.Add("IS_SUB_ARTICLE=1");

			strSQL += "WHERE " + string.Join(" AND ", conditions) + " ORDER BY SUBMISSION_TIME DESC";

			//read results:
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL, parameters.ToArray());
			List<ArticleData> arrArticles = new List<ArticleData>();
			table.Rows.ForEach(row =>
			{
				//read current record:
				ArticleData data = new ArticleData();
				List<AttachmentData> attachments = new List<AttachmentData>();
				data.ID = row.GetIntOrDefault("ARTICLE_ID", -1);
				data.Time = (DateTime)row["SUBMISSION_TIME"];
				data.Contents = row["ARTICLE_DESCRIPTION"].ToString();
				string strImages = row["ARTICLE_IMAGE_NAME"].ToString();
				data.Images = Sport.Common.Tools.SplitNoBlank(strImages, DefaultSeperator);
				data.Caption = row["ARTICLE_CAPTION"].ToString();
				data.SubCaption = row["ARTICLE_SUB_CAPTION"].ToString();
				data.IsPrimary = (row["IS_PRIMARY_ARTICLE"].ToString() == "1");
				data.IsSub = (row["IS_SUB_ARTICLE"].ToString() == "1");
				data.LastModified = (DateTime)row["DATE_LAST_MODIFIED"];
				data.User = row["USER_FIRST_NAME"] + " " + row["USER_LAST_NAME"];
				data.File = row["ARTICLE_FILE_NAME"].ToString();
				data.AuthorName = row["AUTHOR_NAME"].ToString();
				data.Links = StringToLinks(row["ARTICLE_LINKS"] + "");
				data.IsHotLink = (Common.ReadIniValue("HotArticle", Server) == data.ID.ToString());
				if (fullDetails)
					attachments.AddRange(GetAttachment(row["ARTICLE_ATTACHMENTS"] + ""));
				data.Attachments = attachments.ToArray();
				arrArticles.Add(data);
			});

			//iterate over articles, check if region articles.
			if (arrArticles.Count > 0 && fullDetails)
			{
				List<int> allIDs = new List<int>();
				List<string> allImages = new List<string>();
				arrArticles.ForEach(ar =>
				{
					allIDs.Add(ar.ID);
					if (ar.Images != null)
					{
						ar.Images.ToList().ForEach(img =>
						{
							if (!string.IsNullOrWhiteSpace(img))
							{
								if (allImages.IndexOf(img) < 0)
									allImages.Add(img);
							}
						});
					}
				});
				strSQL = "SELECT ARTICLE_ID, REGION_ID, REGION_TYPE FROM REGION_ARTICLES " +
					"WHERE ARTICLE_ID IN (" +
					string.Join(", ", allIDs.ToArray()) + ")";
				table = DB.Instance.GetDataBySQL(strSQL);
				Dictionary<int, SimpleData> regionArticles = new Dictionary<int, SimpleData>();
				table.Rows.ForEach(row =>
				{
					int key = (int)row["ARTICLE_ID"];
					if (!regionArticles.ContainsKey(key))
						regionArticles.Add(key, new SimpleData((int)row["REGION_ID"], row["REGION_TYPE"].ToString()));
				});

				Dictionary<string, string> regionImageDescriptions = GetImageDescriptions(allImages, true);
				foreach (ArticleData articleData in arrArticles)
				{
					articleData.ArticleRegion = -1;
					articleData.IsClubsArticle = false;
					if (regionArticles.ContainsKey(articleData.ID))
					{
						SimpleData regionData = regionArticles[articleData.ID];
						articleData.ArticleRegion = regionData.ID;
						int regionType = Int32.Parse(regionData.Name);
						articleData.IsClubsArticle = (regionType == 1) ? true : false;
					}
					
					//image descriptions
					if (articleData.Images != null && articleData.Images.Length > 0)
					{
						articleData.ImageDescriptions = new string[articleData.Images.Length];
						for (int i = 0; i < articleData.Images.Length; i++)
						{
							string key = articleData.Images[i];
							if (regionImageDescriptions.ContainsKey(key))
								articleData.ImageDescriptions[i] = regionImageDescriptions[key];
						}
					}
				}
			}

			//done.
			return arrArticles.ToArray();
		}

		private ArticleData[] GetArticle(int articleID, bool primaryArticle)
		{
			return GetArticle(articleID, primaryArticle, false, true);
		}

		private ArticleData[] GetArticle(int articleID)
		{
			return GetArticle(articleID, false, false, true);
		}
		#endregion
		#endregion

		#region Update
		[WebMethod]
		public int SetPrimaryAndSecondaryArticles(int primaryArticleId, int[] secondaryArticleIds, string username, string password, int userid)
		{
			//verify authorized user:
			VerifyAuthorizedUser(username, password, true, "set primary and sub articles");

			string strSQL;
			if (primaryArticleId > 0)
			{
				strSQL = "UPDATE ISF_ARTICLES SET IS_PRIMARY_ARTICLE=0";
				DB.Instance.Execute(strSQL);

				strSQL = "UPDATE ISF_ARTICLES SET IS_PRIMARY_ARTICLE=1 WHERE ARTICLE_ID=@id";
				DB.Instance.Execute(strSQL, new SimpleParameter("@id", primaryArticleId));
			}

			if (secondaryArticleIds != null && secondaryArticleIds.Length > 0)
			{
				strSQL = "UPDATE ISF_ARTICLES SET IS_SUB_ARTICLE=0";
				DB.Instance.Execute(strSQL);
				for (int i = 0; i < secondaryArticleIds.Length; i++)
				{
					int secondaryArticleId = secondaryArticleIds[i];
					strSQL = "UPDATE ISF_ARTICLES SET IS_SUB_ARTICLE=1 WHERE ARTICLE_ID=@id";
					DB.Instance.Execute(strSQL, new SimpleParameter("@id", secondaryArticleId));
				}
			}

			return 0;
		}

		/// <summary>
		/// inserts new article or updates existing article data.
		/// </summary>
		[WebMethod]
		public int UpdateArticle(ArticleData data,
			string username, string password, int userid)
		{
			//need to check permissions?
			bool blnCheckPermissions = true;

			//maybe only change images or delete attachment?
			if (data.ID >= 0)
			{
				ArticleData curArticle = GetArticleData(data.ID);
				string strCurrentImages = "";
				if ((curArticle.Images != null) && (curArticle.Images.Length > 0))
					strCurrentImages = String.Join(DefaultSeperator.ToString(), curArticle.Images);
				string strNewImages = "";
				if ((data.Images != null) && (data.Images.Length > 0))
					strNewImages = String.Join(DefaultSeperator.ToString(), data.Images);
				if ((strNewImages != strCurrentImages) ||
					(!Sport.Common.Tools.SameArrays(curArticle.Attachments, data.Attachments)))
				{
					if ((curArticle.AuthorName == data.AuthorName) &&
						(curArticle.Caption == data.Caption) &&
						(curArticle.Contents == data.Contents) &&
						(curArticle.File == data.File) &&
						(curArticle.IsClubsArticle == data.IsClubsArticle) &&
						(curArticle.IsHotLink == data.IsHotLink) &&
						(curArticle.IsPrimary == data.IsPrimary) &&
						(curArticle.IsSub == data.IsSub) &&
						(curArticle.SubCaption == data.SubCaption) &&
						(curArticle.Time == data.Time) &&
						(curArticle.User == data.User))
					{
						blnCheckPermissions = false;
					}
				}
			}

			//verify authorized user:
			if (blnCheckPermissions)
				VerifyAuthorizedUser(username, password, true, "update article");

			//define variables:
			string strSQL = "";
			
			//update attachments description:
			if (data.Attachments != null)
			{
				foreach (AttachmentData attachment in data.Attachments)
				{
					_UpdateAttachment(attachment, username, password, userid,
						blnCheckPermissions);
				}
			}

			//remove current primary and sub flags:
			if (data.IsPrimary)
			{
				strSQL = "UPDATE ISF_ARTICLES SET IS_PRIMARY_ARTICLE=0";
				DB.Instance.Execute(strSQL);
			}
			
			//build sql statement for inserting new record or updating existing:
			List<SimpleParameter> parameters = new List<SimpleParameter>();
			string[] arrAttachments = Sport.Common.Tools.ToStringArray(data.Attachments);
			if (data.ID < 0)
			{
				//insert
				strSQL = "INSERT INTO ISF_ARTICLES (";
				strSQL += "ARTICLE_DESCRIPTION, ARTICLE_FILE_NAME, ARTICLE_IMAGE_NAME, ";
				strSQL += "ARTICLE_CAPTION, SUBMITTED_BY, ARTICLE_SUB_CAPTION, ";
				strSQL += "IS_PRIMARY_ARTICLE, IS_SUB_ARTICLE, ARTICLE_LINKS, ";
				strSQL += "ARTICLE_ATTACHMENTS, AUTHOR_NAME) VALUES (";
				strSQL += "@ad, @af, @ai, @ac, @sb, @asc, @ipa, @isa, @al, @aa, @an)";
			}
			else
			{
				//update
				strSQL = "UPDATE ISF_ARTICLES SET ARTICLE_DESCRIPTION=@ad, ";
				strSQL += "ARTICLE_FILE_NAME=@af, ARTICLE_CAPTION=@ac, ";
				strSQL += "ARTICLE_IMAGE_NAME=@ai, AUTHOR_NAME=@an, ";
				strSQL += "ARTICLE_SUB_CAPTION=@asc, IS_PRIMARY_ARTICLE=@ipa, ";
				strSQL += "ARTICLE_LINKS=@al";
				if (!Sport.Common.Tools.IsArrayEmpty(arrAttachments))
					strSQL += ", ARTICLE_ATTACHMENTS=@aa";
				strSQL += ", ";
				strSQL += "IS_SUB_ARTICLE=@isa, DATE_LAST_MODIFIED=GETDATE() ";
				strSQL += "WHERE ARTICLE_ID=@id";

				//article ID
				parameters.Add(new SimpleParameter("@id", data.ID));
			}

			//article contents
			parameters.Add(new SimpleParameter("@ad", Sport.Common.Tools.IIF((data.Contents != null), data.Contents, DBNull.Value)));

			//submitted by
			parameters.Add(new SimpleParameter("@sb", userid));

			//article file:
			if (data.File == null || data.File.Length == 0)
				parameters.Add(new SimpleParameter("@af", System.DBNull.Value));
			else
				parameters.Add(new SimpleParameter("@af", data.File));

			//article caption
			parameters.Add(new SimpleParameter("@ac", Sport.Common.Tools.IIF((data.Caption != null), data.Caption, DBNull.Value)));

			//article sub caption:
			parameters.Add(new SimpleParameter("@asc", Sport.Common.Tools.IIF((data.SubCaption != null), data.SubCaption, DBNull.Value)));

			//primary article?
			parameters.Add(new SimpleParameter("@ipa", Sport.Common.Tools.IIF(data.IsPrimary, 1, 0)));

			//sub article?
			parameters.Add(new SimpleParameter("@isa", Sport.Common.Tools.IIF(data.IsSub, 1, 0)));

			//article author:
			parameters.Add(new SimpleParameter("@an", Sport.Common.Tools.IIF((data.AuthorName != null), data.AuthorName, DBNull.Value)));

			//article images:
			if (Sport.Common.Tools.IsArrayEmpty(data.Images))
				parameters.Add(new SimpleParameter("@ai", System.DBNull.Value));
			else
				parameters.Add(new SimpleParameter("@ai", string.Join(DefaultSeperator.ToString(), data.Images)));

			//article links:
			string[] arrLinks = Sport.Common.Tools.ToStringArray(data.Links);
			if (Sport.Common.Tools.IsArrayEmpty(arrLinks))
				parameters.Add(new SimpleParameter("@al", System.DBNull.Value));
			else
				parameters.Add(new SimpleParameter("@al", string.Join(LinkSeperator.ToString(), arrLinks)));

			//article attachments:
			if (Sport.Common.Tools.IsArrayEmpty(arrAttachments))
			{
				if (data.ID < 0)
				{
					//null only when inserting new
					parameters.Add(new SimpleParameter("@aa", System.DBNull.Value));
				}
			}
			else
			{
				parameters.Add(new SimpleParameter("@aa", string.Join(DefaultSeperator.ToString(), arrAttachments)));
			}

			//execute the command and return result:
			int result = DB.Instance.Execute(strSQL, parameters.ToArray());

			//read new ID?
			if (data.ID < 0)
			{
				data.ID = DB.Instance.GetMaxValue("ISF_ARTICLES", "ARTICLE_ID");
			}

			//update hot article
			if (data.IsHotLink)
			{
				Common.WriteIniValue("HotArticle", data.ID.ToString(), Server);
			}
			else
			{
				//maybe need to clear value?
				if (data.ID.ToString() == Common.ReadIniValue("HotArticle", Server))
					Common.WriteIniValue("HotArticle", "", Server);
			}

			//region article?
			strSQL = "DELETE FROM REGION_ARTICLES WHERE ARTICLE_ID=@article";
			DB.Instance.Execute(strSQL, new SimpleParameter("@article", data.ID));
			
			//image descriptions:
			if (data.Images != null)
			{
				Dictionary<string, string> databaseImageDescriptions = GetImageDescriptions(data.Images.ToList(), false);
				for (int i = 0; i < data.Images.Length; i++)
				{
					parameters.Clear();
					string strCurImage = data.Images[i];
					string strCurDescription = "";
					if (data.ImageDescriptions != null && i < data.ImageDescriptions.Length)
						strCurDescription = data.ImageDescriptions[i] + "";
					if (databaseImageDescriptions.ContainsKey(strCurImage))
					{
						strSQL = "UPDATE WEBSITE_IMAGE_DESCRIPTIONS " +
							"SET IMAGE_DESCRIPTION=@description " +
							"WHERE IMAGE_DESCRIPTION_ID=@id";
						parameters.Add(new SimpleParameter("@id", Int32.Parse(databaseImageDescriptions[strCurImage])));
					}
					else
					{
						strSQL = "INSERT INTO WEBSITE_IMAGE_DESCRIPTIONS " +
							"(IMAGE_NAME, IMAGE_DESCRIPTION) " +
							"VALUES (@image, @description)";
						parameters.Add(new SimpleParameter("@image", strCurImage));
					}
					parameters.Add(new SimpleParameter("@description", strCurDescription));
					DB.Instance.Execute(strSQL, parameters.ToArray());
				}
			}
			
			//need to update?
			if (data.ArticleRegion >= 0)
				UpdateRegionArticle(data.ArticleRegion, data.IsClubsArticle, data.ID);

			//done.
			return result;
		}
		#endregion

		#region Delete
		/// <summary>
		/// delete given record.
		/// </summary>
		[WebMethod]
		public int DeleteArticle(int id, string username, string password)
		{
			//verify authorized user:
			VerifyAuthorizedUser(username, password, true, "delete article");

			//build sql statement:
			string strSQL = "UPDATE ISF_ARTICLES SET DATE_DELETED=@date " + 
				"WHERE ARTICLE_ID=@id";

			//execute:
			return DB.Instance.Execute(strSQL, new SimpleParameter("@id", id),
				new SimpleParameter("@date", DateTime.Now));
		}
		#endregion
		#endregion

		#region Hit Log
		#region Page Hit
		/// <summary>
		/// add record to the hit log table with given information.
		/// </summary>
		[WebMethod]
		public void Page_Hit(WebSitePage page, string strIP, string strQueryString,
			string strForm, string strUserAgent, string strReferer)
		{
			//TEMPORARY DISABLE
			/*
			//build visitor record:
			long visitorID = UpdateWebsiteVisitor(strIP, strUserAgent);
			if (visitorID < 0)
			{
				System.Diagnostics.Debug.WriteLine("can't add hit log record: visitor not defined.");
				return;
			}

			if (strReferer == null)
				strReferer = "";

			//maybe same action as last action in database?
			string strSQL = "SELECT HIT_LOG_ID, VISITOR_ID, PAGE_INDEX, " + 
				"QUERY_STRING_DATE, FORM_DATA, REFERER " + 
				"FROM WEBSITE_HIT_LOG WHERE HIT_LOG_ID=" + 
				"(SELECT MAX(HIT_LOG_ID) FROM WEBSITE_HIT_LOG)";

			//read results:
			SimpleRow row;
			long hitlogID = -1;
			if (DB.Instance.GetSingleRow(strSQL, out row))
			{
				int pageIndex = row.GetIntOrDefault("PAGE_INDEX", -1);
				string queryStringData = row["QUERY_STRING_DATE"].ToString();
				string formData = row["FORM_DATA"].ToString();
				string referer = row["REFERER"].ToString();
				if (Int64.Parse(row["VISITOR_ID"].ToString()) == visitorID &&
					pageIndex == (int)page && queryStringData == strQueryString &&
					formData == strForm && referer == strReferer)
				{
					hitlogID = Int64.Parse(row["HIT_LOG_ID"].ToString());
				}
			}
			
			//build proper SQL
			List<SimpleParameter> parameters = new List<SimpleParameter>();
			if (hitlogID < 0)
			{
				strSQL = "INSERT INTO WEBSITE_HIT_LOG " + 
					"(VISITOR_ID, PAGE_INDEX, QUERY_STRING_DATE, FORM_DATA, REFERER) " + 
					"VALUES (@vid, @pi, @qsd, @fd, @referer)";
				parameters.Add(new SimpleParameter("@vid", visitorID));
				parameters.Add(new SimpleParameter("@pi", (int)page));
				parameters.Add(new SimpleParameter("@qsd", 
					Sport.Common.Tools.IIF((strQueryString == null), DBNull.Value, strQueryString)));
				parameters.Add(new SimpleParameter("@fd", 
					Sport.Common.Tools.IIF((strForm == null), DBNull.Value, strForm));
				parameters.Add(new SimpleParameter("@referer", 
					Sport.Common.Tools.IIF((strReferer == null), DBNull.Value, strReferer));
			}
			else
			{
				strSQL = "UPDATE WEBSITE_HIT_LOG SET DATE_VISITED=@dv " + 
					"WHERE HIT_LOG_ID=@id";
				parameters.Add(new SimpleParameter("@dv", DateTime.Now));
				parameters.Add(new SimpleParameter("@id", hitlogID));
			}

			//execute:
			DB.Instance.Execute(strSQL, parameters.ToArray());
			*/
		}
		#endregion

		#region Update Visitor
		private long UpdateWebsiteVisitor(string strIP, string strUserAgent)
		{
			//validate data:
			if ((strIP == null) || (strIP.Length == 0) || (strIP.Length > 15))
				throw new Exception("invalid IP address: " + strIP);

			//got user agent?
			if (strUserAgent == null)
				strUserAgent = "";

			//truncate if needed:
			if (strUserAgent.Length > 150)
			{
				System.Diagnostics.Debug.WriteLine(
					"warning: user agent data truncated: " + strUserAgent);
				strUserAgent = strUserAgent.Substring(0, 150);
			}

			//build sql statement and parameters:
			string strSQL = "SELECT VISITOR_ID FROM WEBSITE_VISITORS " + 
				"WHERE IP_ADDRESS=@1 AND USER_AGENT=@2";
			SimpleParameter ipParam = new SimpleParameter("@1", strIP);
			SimpleParameter userAgentParam = new SimpleParameter("@2", strUserAgent);

			//read results:
			long visitorID = Int64.Parse(DB.Instance.ExecuteScalar(strSQL, -1, 
				ipParam, userAgentParam).ToString());
			
			//check if exists:
			if (visitorID < 0)
			{
				//does not exist, add new record:
				strSQL = "INSERT INTO WEBSITE_VISITORS (IP_ADDRESS, USER_AGENT) " +
					"VALUES (@1, @2)";
				DB.Instance.Execute(strSQL, ipParam, userAgentParam);
				
				//read id from database:
				visitorID = DB.Instance.GetMaxValue("WEBSITE_VISITORS", "VISITOR_ID");
			}
			
			//return ID:
			return visitorID;
		}
		#endregion

		#region Purge
		[WebMethod]
		public string PurgeOldHitLogData()
		{
			//TEMPORARY DISABLE
			/*
			//define days after which hit log data expire
			int EXPIRE_DAYS = 180;

			//initialize result string
			System.Text.StringBuilder result = new System.Text.StringBuilder();

			//build past date:
			DateTime dtPastDate = DateTime.Now.AddDays(-1 * EXPIRE_DAYS);
			result.Append("Purging hit log data older than " + dtPastDate.ToString("dd/MM/yyyy") + "...<br />");

			//get minimum date:
			string strSQL = "SELECT MIN(DATE_VISITED) AS MIN_DATE " +
				"FROM WEBSITE_HIT_LOG";
			DateTime dtMinDate = (DateTime)DB.Instance.ExecuteScalar(strSQL, DateTime.MinValue);
			
			//got anything?
			if (dtMinDate.Year < 1900)
			{
				result.Append("no hit log data found.<br />");
				return result.ToString();
			}
			else
			{
				result.Append("Purging records in the range " +
					dtMinDate.ToString("dd/MM/yyyy") + " - " +
					dtPastDate.ToString("dd/MM/yyyy") + "...<br />");
			}

			//select all hit log data in range to delete
			strSQL = "SELECT PAGE_INDEX, COUNT(HIT_LOG_ID) AS HIT_COUNT " + 
				"FROM WEBSITE_HIT_LOG " + 
				"WHERE DATE_VISITED<@past " + 
				"GROUP BY PAGE_INDEX";

			//read results:
			SimpleParameter pastParam = new SimpleParameter("@past", dtPastDate);
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL, pastParam);
			Dictionary<int, int> arrData = new Dictionary<int, int>();
			table.Rows.ForEach(row =>
			{
				int curPageIndex = row.GetIntOrDefault("PAGE_INDEX", (int)WebSitePage.Unknown);
				int curHitCount = (int)row["HIT_COUNT"];
				if (!arrData.ContainsKey(curPageIndex))
					arrData.Add(curPageIndex, curHitCount);
			});
			
			//remove old records
			strSQL = "DELETE FROM WEBSITE_HIT_LOG WHERE DATE_VISITED<@past";
			result.Append("Deleting all old records from database...<br />");
			DB.Instance.Execute(strSQL, pastParam);
			
			//initialize command:
			strSQL = "INSERT INTO WEBSITE_HIT_LOG_ARCHIVE (" +
				"PAGE_INDEX, DATE_START, DATE_END, HIT_COUNT) " +
				"VALUES (@page, @start, @end, @hitcount)";
			SimpleParameter pageParam = new SimpleParameter("@page", (int)WebSitePage.Unknown);
			SimpleParameter startDateParam = new SimpleParameter("@start", dtMinDate);
			SimpleParameter endDateParam = new SimpleParameter("@end", dtPastDate);
			SimpleParameter hitCountParam = new SimpleParameter("@hitcount", 0);

			//iterate over array, insert new archive record for each page index.
			int counter = 0;
			arrData.Keys.ToList().ForEach(pageIndex =>
			{
				//assign parameters:
				pageParam.Value = pageIndex;
				hitCountParam.Value = arrData[pageIndex];

				//execute
				result.Append("Now executing insert #" + (counter + 1) + "...<br />");
				DB.Instance.Execute(strSQL, pageParam, startDateParam, endDateParam, hitCountParam);
				counter++;
			});

			//done.
			return result.ToString();
			*/

			return "temporary disabled";
		}
		#endregion
		#endregion

		#region Attachments
		#region Get
		[WebMethod]
		public int FindAttachment(string fileName)
		{
			//build sql statement:
			string strSQL = "SELECT ATTACHMENT_ID FROM WEBSITE_ATTACHMENTS " + 
				"WHERE ATT_FILE_NAME=@1";

			//read results:
			return (int)DB.Instance.ExecuteScalar(strSQL, -1,
				new SimpleParameter("@1", fileName));
		}

		[WebMethod]
		public AttachmentData GetAttachmentData(int ID)
		{
			AttachmentData[] attachments = GetAttachment(ID, -1);
			if ((attachments == null) || (attachments.Length == 0))
			{
				AttachmentData result = new AttachmentData();
				result.ID = -1;
				return result;
			}

			return attachments[0];
		}

		private AttachmentData[] GetAttachment(int attachmentID, int userID)
		{
			//build sql statement:
			string strSQL = "SELECT a.ATTACHMENT_ID, a.ATT_FILE_NAME, " + 
				"a.ATT_DESCRPTION, a.SUBMIT_TIME, a.SENDER, " + 
				"u.USER_FIRST_NAME, u.USER_LAST_NAME " + 
				"FROM (WEBSITE_ATTACHMENTS a INNER JOIN USERS u ON a.SENDER=u.USER_ID)";

			//build conditions:
			List<string> conditions = new List<string>();
			List<SimpleParameter> parameters = new List<SimpleParameter>();
			if (attachmentID >= 0)
			{
				conditions.Add("ATTACHMENT_ID=@id");
				parameters.Add(new SimpleParameter("@id", attachmentID));
			}
			if (userID >= 0)
			{
				conditions.Add("SENDER=@sender");
				parameters.Add(new SimpleParameter("@sender", userID));
			}
			if (conditions.Count > 0)
				strSQL += " WHERE " + string.Join(" AND ", conditions);
			strSQL += " ORDER BY SUBMIT_TIME DESC";

			//read results:
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL, parameters.ToArray());
			List<AttachmentData> attachments = new List<AttachmentData>();
			table.Rows.ForEach(row =>
			{
				//read current record:
				AttachmentData data = new AttachmentData();
				data.ID = (int)row["ATTACHMENT_ID"];
				data.Time = (DateTime)row["SUBMIT_TIME"];
				data.SentBy = row["USER_FIRST_NAME"].ToString() + " " + row["USER_LAST_NAME"].ToString();
				data.fileName = row["ATT_FILE_NAME"].ToString();
				data.Description = row["ATT_DESCRPTION"].ToString();
				attachments.Add(data);
			});

			//return articles array:
			return attachments.ToArray();
		}

		/// <summary>
		/// get comma seperated list of attachments ID and returns their data.
		/// </summary>
		private AttachmentData[] GetAttachment(string strAttachments)
		{
			List<AttachmentData> result = new List<AttachmentData>();
			string[] arrAttachments = strAttachments.Split(new char[] { DefaultSeperator }, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < arrAttachments.Length; i++)
			{
				AttachmentData[] curAttachments = GetAttachment(Int32.Parse(arrAttachments[i]), -1);
				if (curAttachments.Length > 0)
					result.Add(curAttachments[0]);
			}
			return result.ToArray();
		}
		#endregion

		#region Update
		/// <summary>
		/// inserts new attachment or updates existing attachment data.
		/// </summary>
		[WebMethod]
		public int UpdateAttachment(AttachmentData attachment,
			string username, string password, int userid)
		{
			return _UpdateAttachment(attachment, username, password, userid, true);
		}

		private int _UpdateAttachment(AttachmentData attachment,
			string username, string password, int userid, bool checkPermissions)
		{
			//verify authorized user:
			if (checkPermissions)
				VerifyAuthorizedUser(username, password, true, "update attachment");

			//build sql statement for inserting new record or updating existing:
			string strSQL = "";
			List<SimpleParameter> parameters = new List<SimpleParameter>();
			parameters.Add(new SimpleParameter("@afn", attachment.fileName));
			parameters.Add(new SimpleParameter("@ad", Sport.Common.Tools.IIF(
				(attachment.Description == null), DBNull.Value, attachment.Description)));
			int result = attachment.ID;
			if (result < 0)
			{
				//insert
				strSQL = "INSERT INTO WEBSITE_ATTACHMENTS (ATT_FILE_NAME, ";
				strSQL += "ATT_DESCRPTION, SENDER) VALUES (@afn, @ad, @sender)";
				parameters.Add(new SimpleParameter("@sender", userid));
			}
			else
			{
				//update
				strSQL = "UPDATE WEBSITE_ATTACHMENTS SET ATT_FILE_NAME=@afn, ";
				strSQL += "ATT_DESCRPTION=@ad WHERE ATTACHMENT_ID=@id";
				parameters.Add(new SimpleParameter("@id", attachment.ID));
			}

			//execute:
			DB.Instance.Execute(strSQL, parameters.ToArray());

			if (result < 0)
			{
				//read the ID:
				result = DB.Instance.GetMaxValue("WEBSITE_ATTACHMENTS", "ATTACHMENT_ID");
			}

			return result;
		}
		#endregion

		#region Delete
		/// <summary>
		/// delete given record.
		/// </summary>
		[WebMethod]
		public int DeleteAttachment(int id, string username, string password)
		{
			//verify authorized user:
			//VerifyAuthorizedUser(username, password, true, "delete attachment");

			//build sql statement:
			string strSQL = "DELETE FROM WEBSITE_ATTACHMENTS WHERE ATTACHMENT_ID=@id";

			//execute and return the result:
			return DB.Instance.Execute(strSQL,
				new SimpleParameter("@id", id));
		}
		#endregion
		#endregion

		#region Pages
		#region Get
		[WebMethod]
		public PageData FindPageData(string pageCaption, int pageIndex)
		{
			PageData result = new PageData();
			result.ID = -1;
			PageData[] pages = GetPage(-1, null, pageIndex);
			if (pages != null)
			{
				for (int i = 0; i < pages.Length; i++)
				{
					if (pages[i].Caption == pageCaption)
						return pages[i];
				}
			}
			return result;
		}

		[WebMethod]
		public PageData GetPageData(int ID)
		{
			PageData[] pages = GetPage(ID, null, -1);
			if ((pages == null) || (pages.Length == 0))
			{
				PageData result = new PageData();
				result.ID = -1;
				return result;
			}

			return pages[0];
		}

		/// <summary>
		/// get all pages according to either ID, Title or Index in enumeration.
		/// </summary>
		private PageData[] GetPage(int pageID, string pageTitle, int pageIndex)
		{
			//build sql statement:
			string strSQL = "SELECT WEBSITE_PAGE_ID, PAGE_CAPTION, PAGE_CONTENTS, ";
			strSQL += "PAGE_AUTHOR, AUTHOR_TITLE, PAGE_ATTACHMENTS, PAGE_INDEX, ";
			strSQL += "PAGE_LINKS FROM WEBSITE_PAGES";
			List<SimpleParameter> parameters = new List<SimpleParameter>();
			if (pageID >= 0 || (pageTitle != null && pageTitle.Length > 0) ||
				pageIndex >= 0)
			{
				strSQL += " WHERE ";
				if (pageID >= 0)
				{
					strSQL += "WEBSITE_PAGE_ID=@id";
					parameters.Add(new SimpleParameter("@id", pageID));
				}
				else if ((pageTitle != null) && (pageTitle.Length > 0))
				{
					strSQL += "PAGE_CAPTION=@caption";
					parameters.Add(new SimpleParameter("@caption", pageTitle));
				}
				else if (pageIndex >= 0)
				{
					strSQL += "PAGE_INDEX=@index";
					parameters.Add(new SimpleParameter("@index", pageIndex));
				}
			}

			//read results:
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL, parameters.ToArray());
			List<PageData> pages = new List<PageData>();
			table.Rows.ForEach(row =>
			{
				//read current record:
				PageData data = new PageData();
				data.ID = (int)row["WEBSITE_PAGE_ID"];
				if (data.ID > 0)
				{
					data.Caption = row["PAGE_CAPTION"].ToString();
					data.Contents = row["PAGE_CONTENTS"].ToString();
					data.AuthorName = row["PAGE_AUTHOR"].ToString();
					data.AuthorTitle = row["AUTHOR_TITLE"].ToString();
					data.Index = row.GetIntOrDefault("PAGE_INDEX", -1);
					data.Attachments = GetAttachment(row["PAGE_ATTACHMENTS"].ToString());
					data.Links = StringToLinks(row["PAGE_LINKS"].ToString());
					pages.Add(data);
				}
			});

			//return pages array:
			return pages.ToArray();
		}
		#endregion

		#region Update
		/// <summary>
		/// inserts new page or updates existing page data.
		/// </summary>
		[WebMethod]
		public int UpdatePage(PageData page, string username, string password,
			int userid)
		{
			//need to check permissions?
			bool blnCheckPermissions = true;

			//maybe only change images or delete attachment?
			if (page.ID >= 0)
			{
				PageData curPage = GetPageData(page.ID);
				string strCurrentAttachments = "";
				if ((curPage.Attachments != null) && (curPage.Attachments.Length > 0))
				{
					for (int i = 0; i < curPage.Attachments.Length; i++)
					{
						strCurrentAttachments += curPage.Attachments[i].ID;
						if (i < (curPage.Attachments.Length - 1))
							strCurrentAttachments += DefaultSeperator.ToString();
					}
				}
				string strNewAttachments = "";
				if ((page.Attachments != null) && (page.Attachments.Length > 0))
				{
					for (int i = 0; i < page.Attachments.Length; i++)
					{
						strNewAttachments += page.Attachments[i].ID;
						if (i < (page.Attachments.Length - 1))
							strNewAttachments += DefaultSeperator.ToString();
					}
				}
				if (strNewAttachments != strCurrentAttachments)
				{
					if ((curPage.AuthorName == page.AuthorName) &&
						(curPage.AuthorTitle == page.AuthorTitle) &&
						(curPage.Caption == page.Caption) &&
						(curPage.Contents == page.Contents))
					{
						blnCheckPermissions = false;
					}
				}
			}

			//verify authorized user:
			if (blnCheckPermissions)
				VerifyAuthorizedUser(username, password, true, "update dynamic page");

			//update attachments description:
			if (page.Attachments != null)
			{
				foreach (AttachmentData attachment in page.Attachments)
					_UpdateAttachment(attachment, username, password, userid, false);
			}

			//build sql statement for inserting new record or updating existing:
			string strSQL = "";
			string[] arrLinks = Sport.Common.Tools.ToStringArray(page.Links);
			List<SimpleParameter> parameters = new List<SimpleParameter>();
			parameters.Add(new SimpleParameter("@caption",
				Sport.Common.Tools.IIF((page.Caption == null), DBNull.Value, page.Caption)));
			parameters.Add(new SimpleParameter("@contents",
				Sport.Common.Tools.IIF((page.Contents == null), DBNull.Value, page.Contents)));
			parameters.Add(new SimpleParameter("@authorname",
				Sport.Common.Tools.IIF((page.AuthorName == null), DBNull.Value, page.AuthorName)));
			parameters.Add(new SimpleParameter("@authortitle",
				Sport.Common.Tools.IIF((page.AuthorTitle == null), DBNull.Value, page.AuthorTitle)));
			if (page.Attachments == null || (page.Attachments.Length == 0))
			{
				parameters.Add(new SimpleParameter("@attachments", DBNull.Value));
			}
			else
			{
				string[] attachments = Sport.Common.Tools.ToStringArray(page.Attachments);
				parameters.Add(new SimpleParameter("@attachments", String.Join(
					DefaultSeperator.ToString(), attachments)));
			}
			parameters.Add(new SimpleParameter("@index", page.Index));
			if (Sport.Common.Tools.IsArrayEmpty(arrLinks))
				parameters.Add(new SimpleParameter("@links", System.DBNull.Value));
			else
				parameters.Add(new SimpleParameter("@links", String.Join(LinkSeperator.ToString(), arrLinks)));
			int result = page.ID;
			if (result < 0)
			{
				//insert
				strSQL = "INSERT INTO WEBSITE_PAGES (PAGE_CAPTION, PAGE_CONTENTS, ";
				strSQL += "PAGE_AUTHOR, AUTHOR_TITLE, PAGE_ATTACHMENTS, PAGE_INDEX, ";
				strSQL += "PAGE_LINKS) VALUES (@caption, @contents, @authorname, ";
				strSQL += "@authortitle, @attachments, @index, @links)";
			}
			else
			{
				//update
				strSQL = "UPDATE WEBSITE_PAGES SET PAGE_CAPTION=@caption, ";
				strSQL += "PAGE_CONTENTS=@contents, PAGE_AUTHOR=@authorname, ";
				strSQL += "AUTHOR_TITLE=@authortitle, PAGE_ATTACHMENTS=@attachments, ";
				strSQL += "PAGE_INDEX=@index, PAGE_LINKS=@links ";
				strSQL += "WHERE WEBSITE_PAGE_ID=@id";
				parameters.Add(new SimpleParameter("@id", page.ID));
			}

			//execute:
			DB.Instance.Execute(strSQL, parameters.ToArray());

			if (result < 0)
			{
				//read the ID:
				result = DB.Instance.GetMaxValue("WEBSITE_PAGES", "WEBSITE_PAGE_ID");
			}
			
			//done.
			return result;
		}
		#endregion

		#region Delete
		/// <summary>
		/// delete given record.
		/// </summary>
		[WebMethod]
		public int DeletePage(int id, string username, string password)
		{
			//verify authorized user:
			VerifyAuthorizedUser(username, password, true, "delete dynamic page");

			//build sql statement:
			string strSQL = "DELETE FROM WEBSITE_PAGES WHERE WEBSITE_PAGE_ID=@id";

			//execute command and return the result:
			return DB.Instance.Execute(strSQL,
				new SimpleParameter("@id", id));
		}
		#endregion
		#endregion

		#region Contact

		[WebMethod]
		public RegionData[] getSendableRegions()
		{
			return getSelectedRegions(-1);
		}

		[WebMethod]
		public RegionData getRegionById(int ID)
		{
			RegionData[] regions = getSelectedRegions(ID);
			RegionData result = new RegionData();
			if (regions.Length > 0)
				result = regions[0];
			else
				result.ID = -1;
			return result;
		}

		private RegionData[] getSelectedRegions(int ID)
		{
			string strSQL = "SELECT r.REGION_NAME,r.REGION_ID,u.USER_EMAIL " + 
				"FROM REGIONS r INNER JOIN USERS u ON r.COORDINATOR = u.USER_ID " + 
				"WHERE USER_EMAIL IS NOT NULL AND USER_EMAIL <> ''";
			List<SimpleParameter> parameters = new List<SimpleParameter>();
			if (ID >= 0)
			{
				strSQL += " AND r.REGION_ID = @1";
				parameters.Add(new SimpleParameter("@1", ID));
			}
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL, parameters.ToArray());
			List<RegionData> regions = new List<RegionData>();
			table.Rows.ForEach(row =>
			{
				string currentName = row["REGION_NAME"].ToString();
				string currentEmail = row["USER_EMAIL"].ToString();
				if (currentName.Length > 0 && currentEmail.Length > 0)
				{
					RegionData currentRegion = new RegionData();
					currentRegion.ID = (int)row["REGION_ID"];
					currentRegion.Name = currentName;
					currentRegion.Email = currentEmail;
					regions.Add(currentRegion);
				}
			});
			
			return regions.ToArray();
		}

		#endregion

		#region Banners
		#region Get
		/// <summary>
		/// returns list of all banners.
		/// </summary>
		[WebMethod]
		public BannerData[] GetBanners()
		{
			//build sql statement:
			string strSQL = "SELECT BANNER_ID, URL_150_150, URL_760_80, DESCRIPTION " + 
				"FROM WEBSITE_BANNERS WHERE DATE_DELETED IS NULL";

			//read results:
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL);
			List<BannerData> arrBanners = new List<BannerData>();
			table.Rows.ForEach(row =>
			{
				BannerData data = new BannerData();
				data.ID = (int)row["BANNER_ID"];
				data.url_150_150 = row["URL_150_150"].ToString();
				data.url_760_80 = row["URL_760_80"].ToString();
				data.description = row["DESCRIPTION"].ToString();
				arrBanners.Add(data);
			});
			
			return arrBanners.ToArray();
		}

		/// <summary>
		/// returns the desired banner data.
		/// </summary>
		[WebMethod]
		public BannerData GetBannerData(int id)
		{
			BannerData[] banners = GetBanners();
			BannerData result = new BannerData();
			result.ID = -1;
			result.url_150_150 = "";
			result.url_760_80 = "";
			foreach (BannerData banner in banners)
			{
				if (banner.ID == id)
				{
					result.ID = banner.ID;
					result.url_150_150 = banner.url_150_150;
					result.url_760_80 = banner.url_760_80;
					result.description = banner.description;
					break;
				}
			}
			return result;
		}
		#endregion

		#region Update
		/// <summary>
		/// inserts new banner or updates existing banner in the database.
		/// </summary>
		[WebMethod]
		public int UpdateBannerData(BannerData data, string username, string password)
		{
			//verify authorized user:
			VerifyAuthorizedUser(username, password, true, "update banner");

			//build sql statement:
			string strSQL = "";
			List<SimpleParameter> parameters = new List<SimpleParameter>();
			parameters.Add(new SimpleParameter("@1", data.url_150_150));
			parameters.Add(new SimpleParameter("@2", data.url_760_80));
			parameters.Add(new SimpleParameter("@3", data.description));
			if (data.ID < 0)
			{
				//insert
				strSQL = "INSERT INTO WEBSITE_BANNERS ";
				strSQL += "(URL_150_150, URL_760_80, DESCRIPTION) ";
				strSQL += "VALUES (@1, @2, @3)";
			}
			else
			{
				//update
				strSQL = "UPDATE WEBSITE_BANNERS SET URL_150_150=@1, ";
				strSQL += "URL_760_80=@2, DESCRIPTION=@3 WHERE BANNER_ID=@id";
				parameters.Add(new SimpleParameter("@id", data.ID));
			}


			//execute the command and return result:
			return DB.Instance.Execute(strSQL, parameters.ToArray());
		}
		#endregion

		#region Delete
		/// <summary>
		/// delete given record.
		/// </summary>
		[WebMethod]
		public int DeleteBannerData(int id, string username, string password)
		{
			//verify authorized user:
			VerifyAuthorizedUser(username, password, true, "delete banner");

			//build sql statement:
			string strSQL = "UPDATE WEBSITE_BANNERS SET DATE_DELETED=GETDATE() " + 
				"WHERE BANNER_ID=@id";

			//execute the command and return result:
			return DB.Instance.Execute(strSQL,
				new SimpleParameter("@id", id));
		}
		#endregion
		#endregion

		#region Events
		#region Get
		/// <summary>
		/// returns list of all banners.
		/// </summary>
		[WebMethod]
		public EventData[] GetEvents(DateTime start, DateTime end)
		{
			return _GetEvents(-1, start, end);
		}

		/// <summary>
		/// returns the desired banner data.
		/// </summary>
		[WebMethod]
		public EventData GetEventData(int id)
		{
			EventData[] events = _GetEvents(id, DateTime.MinValue, DateTime.MinValue);
			EventData result = new EventData();
			result.ID = -1;
			if (events.Length > 0)
				result = events[0];
			return result;
		}

		private EventData[] _GetEvents(int ID, DateTime start, DateTime end)
		{
			//build sql statement:
			string strSQL = "SELECT EVENT_ID, EVENT_DATE, DESCRIPTION, URL " + 
				"FROM WEBSITE_EVENTS WHERE DATE_DELETED IS NULL";

			//conditions:
			List<string> conditions = new List<string>();
			List<SimpleParameter> parameters = new List<SimpleParameter>();
			if (ID >= 0)
			{
				conditions.Add("EVENT_ID=@id");
				parameters.Add(new SimpleParameter("@id", ID));
			}
			if (start.Year > 1900)
			{
				conditions.Add("EVENT_DATE>=@start");
				parameters.Add(new SimpleParameter("@start", start));
			}
			if (end.Year > 1900)
			{
				conditions.Add("EVENT_DATE<=@end");
				parameters.Add(new SimpleParameter("@end", end));
			}

			if (conditions.Count > 0)
				strSQL += " AND " + string.Join(" AND ", conditions);
			
			strSQL += " ORDER BY EVENT_DATE ASC";

			//read results:
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL, parameters.ToArray());
			List<EventData> arrEvents = new List<EventData>();
			table.Rows.ForEach(row =>
			{
				EventData data = new EventData();
				data.ID = (int)row["EVENT_ID"];
				data.Date = (DateTime)row["EVENT_DATE"];
				data.Description = row["DESCRIPTION"].ToString();
				data.URL = row["URL"].ToString();
				arrEvents.Add(data);
			});
			
			return arrEvents.ToArray();
		}
		#endregion

		#region Update
		/// <summary>
		/// inserts new event or updates existing event in the database.
		/// </summary>
		[WebMethod]
		public int UpdateEventData(EventData data, string username, string password)
		{
			//verify authorized user:
			VerifyAuthorizedUser(username, password, true, "update Event");
			
			//build sql statement:
			string strSQL = "";
			List<SimpleParameter> parameters = new List<SimpleParameter>();
			parameters.Add(new SimpleParameter("@1", 
				Sport.Common.Tools.IIF((data.Date.Year > 1900), data.Date, DBNull.Value)));
			parameters.Add(new SimpleParameter("@2", 
				Sport.Common.Tools.IIF((data.Description != null), data.Description, DBNull.Value)));
			parameters.Add(new SimpleParameter("@3", 
				Sport.Common.Tools.IIF((data.URL != null), data.URL, DBNull.Value)));
			if (data.ID < 0)
			{
				//flash news
				FlashNewsData flashNews = new FlashNewsData();
				flashNews.ID = -1;
				flashNews.Time = data.Date;
				flashNews.Contents = data.Description;
				if (data.URL != null && data.URL.Length > 0)
				{
					flashNews.Links = new LinkData[1];
					flashNews.Links[0] = new LinkData();
					flashNews.Links[0].Text = flashNews.Links[0].URL = data.URL;
				}

				strSQL = "SELECT USER_ID FROM USERS " + 
					"WHERE USER_LOGIN=@login AND USER_PASSWORD=@password";
				SimpleTable table = DB.Instance.GetDataBySQL(strSQL, 
					new SimpleParameter("@login", username),
					new SimpleParameter("@password", password));
				if (table.Rows.Count > 0)
				{
					int userID = Int32.Parse(table.Rows[0]["USER_ID"].ToString());
					UpdateFlashNews(flashNews, username, password, userID);
				}

				//insert
				strSQL = "INSERT INTO WEBSITE_EVENTS ";
				strSQL += "(EVENT_DATE, DESCRIPTION, URL) ";
				strSQL += "VALUES (@1, @2, @3)";
			}
			else
			{
				//update
				strSQL = "UPDATE WEBSITE_EVENTS SET EVENT_DATE=@1, ";
				strSQL += "DESCRIPTION=@2, URL=@3 WHERE EVENT_ID=@id";
				parameters.Add(new SimpleParameter("@id", data.ID));
			}

			//execute the command and return result:
			return DB.Instance.Execute(strSQL, parameters.ToArray());
		}
		#endregion

		#region Delete
		/// <summary>
		/// delete given record.
		/// </summary>
		[WebMethod]
		public int DeleteEventData(int id, string username, string password)
		{
			//verify authorized user:
			VerifyAuthorizedUser(username, password, true, "delete Event");

			//build sql statement:
			string strSQL = "UPDATE WEBSITE_EVENTS SET DATE_DELETED=GETDATE() " + 
				"WHERE EVENT_ID=@id";

			//execute the command and return result:
			return DB.Instance.Execute(strSQL, new SimpleParameter("@id", id));
		}
		#endregion
		#endregion

		#region Championship Attachments
		#region get
		/// <summary>
		/// returns attachments for given championship
		/// </summary>
		[WebMethod(EnableSession = true)]
		public AttachmentData[] GetChampionshipAttachments(int champCategoryID)
		{
			//build sql statement:
			string strSQL = "SELECT ATTACHMENT_ID " + 
					"FROM CHAMPIONSHIP_ATTACHMENTS WHERE CHAMPIONSHIP_CATEGORY_ID=@1";

			//read results:
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL,
				new SimpleParameter("@1", champCategoryID));
			List<int> arrAttachmentsIDs = new List<int>();
			List<AttachmentData> attachments = new List<AttachmentData>();
			table.Rows.ForEach(row =>
			{
				arrAttachmentsIDs.Add((int)row["ATTACHMENT_ID"]);
			});
			
			//read attachments:
			arrAttachmentsIDs.ForEach(attachmentID =>
			{
				if (attachmentID > 0)
				{
					AttachmentData curAttachment = GetAttachmentData(attachmentID);
					if (curAttachment.ID >= 0)
						attachments.Add(curAttachment);
				}
			});

			//done.
			return attachments.ToArray();
		}
		#endregion

		#region add
		/// <summary>
		/// add given attachment to the given championship.
		/// </summary>
		[WebMethod(EnableSession = true)]
		public int AddChampionshipAttachment(int champCategoryID, int attachmentID)
		{
			//build sql statement:
			string strSQL = "INSERT INTO CHAMPIONSHIP_ATTACHMENTS " +
				"(ATTACHMENT_ID, CHAMPIONSHIP_CATEGORY_ID) VALUES (@att, @champ)";

			//execute the command and return result:
			return DB.Instance.Execute(strSQL, new SimpleParameter("@att", attachmentID),
				new SimpleParameter("@champ", champCategoryID));
		}
		#endregion

		#region remove
		/// <summary>
		/// remove attachment from championship
		/// </summary>
		[WebMethod(EnableSession = true)]
		public int RemoveChampionshipAttachment(int champCategoryID, int attachmentID)
		{
			//build sql statement:
			string strSQL = "DELETE FROM CHAMPIONSHIP_ATTACHMENTS " +
				"WHERE ATTACHMENT_ID=@att AND CHAMPIONSHIP_CATEGORY_ID=@champ";

			//execute the command and return result:
			return DB.Instance.Execute(strSQL, new SimpleParameter("@att", attachmentID),
				new SimpleParameter("@champ", champCategoryID));
		}
		#endregion
		#endregion

		#region Region Articles
		#region get
		/// <summary>
		/// returns the article for given region, if exists.
		/// </summary>
		[WebMethod(EnableSession = true)]
		public ArticleData GetRegionArticles(int regionID, bool blnClubs)
		{
			ArticleData result = new ArticleData();

			//build sql statement:
			string strSQL = "SELECT ARTICLE_ID " + 
				"FROM REGION_ARTICLES WHERE REGION_ID=@1 AND REGION_TYPE=@2";

			//read results:
			result.ID = Int32.Parse(DB.Instance.ExecuteScalar(strSQL, -1, 
				new SimpleParameter("@1", regionID), 
				new SimpleParameter("@2", (blnClubs) ? 1 : 0)).ToString());

			//got article?
			if (result.ID >= 0)
				result = GetArticleData(result.ID);

			//done.
			return result;
		}
		#endregion

		#region update
		/// <summary>
		/// add given article to the given region if does not exist yet, or change
		/// the current region article if exists.
		/// </summary>
		[WebMethod(EnableSession = true)]
		public int UpdateRegionArticle(int regionID, bool blnClubs, int articleID)
		{
			//already exists?
			string strSQL = "SELECT * " + 
				"FROM REGION_ARTICLES WHERE REGION_ID=@1 AND REGION_TYPE=@2";
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL,
				new SimpleParameter("@1", regionID),
				new SimpleParameter("@2", (blnClubs) ? 1 : 0));
			bool exists = (table.Rows.Count > 0);

			//build sql statement:
			if (exists)
			{
				//already exists, update.
				strSQL = "UPDATE REGION_ARTICLES SET ARTICLE_ID=@article ";
				strSQL += "WHERE REGION_ID=@region AND REGION_TYPE=@type";
			}
			else
			{
				//does not exist, add to database.
				strSQL = "INSERT INTO REGION_ARTICLES (REGION_ID, ARTICLE_ID, REGION_TYPE) ";
				strSQL += "VALUES (@region, @article, @type)";
				
			}

			//execute the command and return result:
			return DB.Instance.Execute(strSQL,
				new SimpleParameter("@region", regionID),
				new SimpleParameter("@type", (blnClubs) ? 1 : 0),
				new SimpleParameter("@article", articleID));
		}
		#endregion

		#region delete
		/// <summary>
		/// remove article from region.
		/// </summary>
		[WebMethod(EnableSession = true)]
		public int RemoveRegionArticle(int regionID, bool blnClubs)
		{
			//build sql statement:
			string strSQL = "DELETE FROM REGION_ARTICLES " +
				"WHERE REGION_ID=@region AND REGION_TYPE=@type";

			//execute the command and return result:
			return DB.Instance.Execute(strSQL,
				new SimpleParameter("@region", regionID),
				new SimpleParameter("@type", (blnClubs) ? 1 : 0));
		}
		#endregion
		#endregion

		#region image gallery
		#region get
		[WebMethod(EnableSession = true)]
		public ImageData[] GetAllGalleryImages()
		{
			return GetGalleryImages(-1, "", "");
		}

		[WebMethod(EnableSession = true)]
		public ImageData GetGalleryImage(int imageID)
		{
			ImageData result = null;
			ImageData[] arrImages = GetGalleryImages(imageID, "", "");
			if ((arrImages != null) && (arrImages.Length > 0))
				result = arrImages[0];
			return result;
		}

		[WebMethod(EnableSession = true)]
		public ImageData[] GetImagesByGroup(string strGroupName, string strSubGroup)
		{
			return GetGalleryImages(-1, strGroupName, strSubGroup);
		}

		[WebMethod(EnableSession = true)]
		public ArrayList GetGalleryGroups()
		{
			//build sql statement:
			string strSQL = "SELECT DISTINCT GROUP_NAME, SUB_GROUP " +
				"FROM WEBSITE_IMAGE_GALLERY ORDER BY GROUP_NAME ASC, SUB_GROUP ASC";

			//read results:
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL);
			ArrayList result = new ArrayList();
			ArrayList arrSubGroups = new ArrayList();
			string strLastGroup = null;
			table.Rows.ForEach(row =>
			{
				string strCurGroup = row["GROUP_NAME"].ToString();
				string strSubGroup = row["SUB_GROUP"].ToString();
				if (strLastGroup == null || strCurGroup != strLastGroup)
				{
					if (arrSubGroups.Count > 0)
						result.Add((string[])arrSubGroups.ToArray(typeof(string)));
					arrSubGroups.Clear();
					arrSubGroups.Add(strCurGroup);
				}
				if (strCurGroup.Length > 0 && strSubGroup.Length > 0)
					arrSubGroups.Add(strSubGroup);
				strLastGroup = strCurGroup;
			});

			if (arrSubGroups.Count > 0)
				result.Add((string[])arrSubGroups.ToArray(typeof(string)));
			
			//done.
			return result;
		} //end function GetGalleryGroups

		private ImageData[] GetGalleryImages(int imageID, string strGroupName,
			string strSubGroup)
		{
			//build sql statement:
			strGroupName = Common.ToStringDef(strGroupName, "");
			string strSQL = "SELECT IMAGE_ID, PICTURE_NAME, ORIGINAL_NAME, " +
				"THUMBNAIL_NAME, GROUP_NAME, SUB_GROUP, DESCRIPTION, VIEW_COUNT " +
				"FROM WEBSITE_IMAGE_GALLERY";
			List<string> filters = new List<string>();
			List<SimpleParameter> parameters = new List<SimpleParameter>();
			if (imageID >= 0)
			{
				filters.Add("IMAGE_ID=@image");
				parameters.Add(new SimpleParameter("@image", imageID));
			}
			if (strGroupName.Length > 0)
			{
				filters.Add("GROUP_NAME=@group");
				parameters.Add(new SimpleParameter("@group", strGroupName));
			}
			if (strSubGroup.Length > 0)
			{
				filters.Add("SUB_GROUP=@sub");
				parameters.Add(new SimpleParameter("@sub", strSubGroup));
			}
			if (filters.Count > 0)
				strSQL += " WHERE " + string.Join(" AND ", filters);
			strSQL += " ORDER BY GROUP_NAME ASC, SUB_GROUP ASC";

			//read results:
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL, parameters.ToArray());
			List<ImageData> images = new List<ImageData>();
			table.Rows.ForEach(row =>
			{
				ImageData data = new ImageData();
				data.ID = (int)row["IMAGE_ID"];
				data.PictureName = row["PICTURE_NAME"].ToString();
				data.OriginalName = row["ORIGINAL_NAME"].ToString();
				data.GroupName = row["GROUP_NAME"].ToString();
				data.SubGroup = row["SUB_GROUP"].ToString();
				data.Description = row["DESCRIPTION"].ToString();
				data.ViewCount = (int)row["VIEW_COUNT"];
				images.Add(data);
			});
			
			//done.
			return images.ToArray();
		}
		#endregion

		#region insert/update
		[WebMethod(EnableSession = true)]
		public int UpdateGalleryImage(ImageData data)
		{
			//build sql statement:
			string strSQL = "";
			List<SimpleParameter> parameters = new List<SimpleParameter>();
			parameters.Add(new SimpleParameter("@1", data.PictureName));
			parameters.Add(new SimpleParameter("@2", data.OriginalName));
			parameters.Add(new SimpleParameter("@3", ""));
			parameters.Add(new SimpleParameter("@4", data.GroupName));
			parameters.Add(new SimpleParameter("@5", data.Description));
			parameters.Add(new SimpleParameter("@6", data.SubGroup));
			if (data.ID < 0)
			{
				strSQL = "INSERT INTO WEBSITE_IMAGE_GALLERY (" +
					"PICTURE_NAME, ORIGINAL_NAME, THUMBNAIL_NAME, GROUP_NAME, " +
					"DESCRIPTION, SUB_GROUP)" +
					" VALUES (@1, @2, @3, @4, @5, @6)";
			}
			else
			{
				strSQL = "UPDATE WEBSITE_IMAGE_GALLERY " +
					"SET PICTURE_NAME=@1, ORIGINAL_NAME=@2, THUMBNAIL_NAME=@3, " +
					"GROUP_NAME=@4, DESCRIPTION=@5, SUB_GROUP=@6 WHERE IMAGE_ID=@id";
				parameters.Add(new SimpleParameter("@id", data.ID));
			}

			//execute the command and return result:
			return DB.Instance.Execute(strSQL, parameters.ToArray());
		}

		[WebMethod(EnableSession = true)]
		public int AddImageView(int imageID)
		{
			//build sql statement:
			string strSQL = "UPDATE WEBSITE_IMAGE_GALLERY " +
					"SET VIEW_COUNT=VIEW_COUNT+1 WHERE IMAGE_ID=@id";

			//execute the command and return result:
			return DB.Instance.Execute(strSQL, new SimpleParameter("@id", imageID));
		}
		#endregion

		#region delete
		[WebMethod(EnableSession = true)]
		public int DeleteGallreyImage(int imageID)
		{
			//build sql statement:
			string strSQL = "DELETE FROM WEBSITE_IMAGE_GALLERY " +
				"WHERE IMAGE_ID=@id";

			//execute the command and return result:
			return DB.Instance.Execute(strSQL, new SimpleParameter("@id", imageID));
		}
		#endregion
		#endregion

		#region Article Comments
		#region Get
		[WebMethod]
		public ArticleCommentData[] GetArticleComments(int articleID)
		{
			//build sql statement:
			string strSQL = "SELECT ARTICLE_COMMENT_ID, ARTICLE_ID, COMMENT_NUMBER, " +
				"VISITOR_IP, VISITOR_NAME, VISITOR_EMAIL, COMMENT_CAPTION, " +
				"COMMENT_DESCRIPTION, DATE_DELETED, SUBMISSION_TIME " +
				"FROM WEBSITE_ARTICLE_COMMENTS WHERE ARTICLE_ID=@article " +
				"ORDER BY COMMENT_NUMBER ASC";

			//read results:
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL,
				new SimpleParameter("@article", articleID));
			List<ArticleCommentData> comments = new List<ArticleCommentData>();
			table.Rows.ForEach(row =>
			{
				ArticleCommentData data = new ArticleCommentData();
				data.ID = (int)row["ARTICLE_COMMENT_ID"];
				data.Article = (int)row["ARTICLE_ID"];
				data.Number = (int)row["COMMENT_NUMBER"];
				data.VisitorIP = row["VISITOR_IP"].ToString();
				data.VisitorName = row["VISITOR_NAME"].ToString();
				data.VisitorEmail = row["VISITOR_EMAIL"].ToString();
				data.Caption = row["COMMENT_CAPTION"].ToString();
				data.Contents = row["COMMENT_DESCRIPTION"].ToString();
				data.DatePosted = (DateTime)row["SUBMISSION_TIME"];
				data.Deleted = (((DateTime)row["DATE_DELETED"]).Year > 1900);
				comments.Add(data);
			});
			
			//done.
			return comments.ToArray();
		}
		#endregion

		#region Update/Insert/Delete/UnDelete
		[WebMethod]
		public int UpdateArticleComment(ArticleCommentData data,
			string username, string password)
		{
			//build sql statement:
			string strSQL = "";
			List<SimpleParameter> parameters = new List<SimpleParameter>();
			SimpleParameter articleParam = new SimpleParameter("@article", data.Article);
			if (data.ID < 0)
			{
				strSQL = "SELECT MAX(COMMENT_NUMBER) FROM WEBSITE_ARTICLE_COMMENTS " +
					"WHERE ARTICLE_ID=@article";
				int maxNumber = Int32.Parse(DB.Instance.ExecuteScalar(strSQL, 0, articleParam).ToString());
				data.Number = maxNumber + 1;

				strSQL = "INSERT INTO WEBSITE_ARTICLE_COMMENTS (" +
					"ARTICLE_ID, COMMENT_NUMBER, VISITOR_IP, VISITOR_NAME, VISITOR_EMAIL, " +
					"COMMENT_CAPTION, COMMENT_DESCRIPTION)" +
					" VALUES (@article, @number, @IP, @name, @email, @caption, @contents)";
				parameters.Add(articleParam);
				parameters.Add(new SimpleParameter("@number", data.Number));
				parameters.Add(new SimpleParameter("@IP", data.VisitorIP));
			}
			else
			{
				//verify authorized user:
				VerifyAuthorizedUser(username, password, true, "edit article comment");

				strSQL = "UPDATE WEBSITE_ARTICLE_COMMENTS " +
					"SET VISITOR_NAME=@name, VISITOR_EMAIL=@email, COMMENT_CAPTION=@caption, " +
					"COMMENT_DESCRIPTION=@contents, DATE_DELETED=@deleted " +
					"WHERE ARTICLE_COMMENT_ID=@id";
				parameters.Add(new SimpleParameter("@id", data.ID));
				parameters.Add(new SimpleParameter("@deleted",
					Sport.Common.Tools.IIF(data.Deleted, DateTime.Now, DBNull.Value)));
			}
			parameters.Add(new SimpleParameter("@name", data.VisitorName));
			parameters.Add(new SimpleParameter("@email", data.VisitorEmail));
			parameters.Add(new SimpleParameter("@caption", data.Caption));
			parameters.Add(new SimpleParameter("@contents", data.Contents));

			//execute the command and return result:
			return DB.Instance.Execute(strSQL, parameters.ToArray());
		}
		#endregion
		#endregion

		#region Pending Competitors
		[WebMethod]
		public PendingCompetitorData[] GetPendingCompetitorsTeamsData()
		{
			//build sql statement:
			string strSQL = "SELECT DISTINCT TEAM_ID, PHASE_NAME, GROUP_NAME " +
				"FROM PENDING_COMPETITORS ORDER BY TEAM_ID, PHASE_NAME";

			//read results:
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL);
			List<PendingCompetitorData> result = new List<PendingCompetitorData>();
			table.Rows.ForEach(row =>
			{
				PendingCompetitorData data = new PendingCompetitorData();
				data.Team = (int)row["TEAM_ID"];
				data.Phase = row["PHASE_NAME"].ToString();
				data.Group = row["GROUP_NAME"].ToString();
				result.Add(data);
			});

			//done.
			return result.ToArray();
		}

		[WebMethod]
		public PendingCompetitorData[] GetPendingCompetitors(int teamID,
			string strPhase)
		{
			List<SimpleParameter> parameters = new List<SimpleParameter>();
			parameters.Add(new SimpleParameter("@team", teamID));

			//build sql statement:
			string strSQL = "SELECT PENDING_COMPETITOR_ID, TEAM_ID, PLAYER_ID, " +
				"COMPETITION_INDEX, HEAT_INDEX,  PHASE_NAME, GROUP_NAME " +
				"FROM PENDING_COMPETITORS WHERE TEAM_ID=@team ";
			if (!string.IsNullOrEmpty(strPhase))
			{
				strSQL += "AND PHASE_NAME=@phase ";
				parameters.Add(new SimpleParameter("@phase", strPhase));
			}
			strSQL += "ORDER BY HEAT_INDEX ASC";

			//read results:
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL, parameters.ToArray());
			List<PendingCompetitorData> result = new List<PendingCompetitorData>();
			table.Rows.ForEach(row =>
			{
				PendingCompetitorData data = new PendingCompetitorData();
				data.ID = (int)row["PENDING_COMPETITOR_ID"];
				data.Player = (int)row["PLAYER_ID"];
				data.Competition = (int)row["COMPETITION_INDEX"];
				data.Heat = (int)row["HEAT_INDEX"];
				data.Phase = row["PHASE_NAME"].ToString();
				data.Group = row["GROUP_NAME"].ToString();
				result.Add(data);
			});

			//done.
			return result.ToArray();
		}

		[WebMethod]
		public int AddPendingCompetitors(int teamID,
			PendingCompetitorData[] competitors)
		{
			//delete current competitors.

			//build SQL statement:
			string strSQL = "DELETE FROM PENDING_COMPETITORS " +
				"WHERE TEAM_ID=@team";

			//execute:
			DB.Instance.Execute(strSQL, new SimpleParameter("@team", teamID));

			//got anything?
			if (competitors == null || competitors.Length == 0)
				return 0;
			
			//insert new competitors.
			SimpleParameter teamParam = new SimpleParameter("@team", teamID);
			SimpleParameter playerParam = new SimpleParameter("@player", -1);
			SimpleParameter competitionParam = new SimpleParameter("@competition", -1);
			SimpleParameter heatParam = new SimpleParameter("@heat", -1);
			SimpleParameter phaseParam = new SimpleParameter("@phase", "");
			SimpleParameter groupParam = new SimpleParameter("@group", "");
			foreach (PendingCompetitorData competitor in competitors)
			{
				//build SQL statement:
				strSQL = "INSERT INTO PENDING_COMPETITORS " +
					"(TEAM_ID, PLAYER_ID, COMPETITION_INDEX, HEAT_INDEX, " + 
					", PHASE_NAME, GROUP_NAME) VALUES " + 
					"(@team, @player, @competition, @heat ,@phase, @group)";

				//apply parameters:
				playerParam.Value = competitor.Player;
				competitionParam.Value = competitor.Competition;
				heatParam.Value = DBNull.Value;
				if (competitor.Heat > 0)
					heatParam.Value = competitor.Heat;
				phaseParam.Value = competitor.Phase;
				groupParam.Value = competitor.Group;

				//execute the command:
				DB.Instance.Execute(strSQL, teamParam, playerParam,
					competitionParam, heatParam, phaseParam, groupParam);
			} //end loop over pending competitors

			//done.
			return 0;
		} //end function AddPendingCompetitors
		#endregion

		#region Pending Match Score
		#region Get
		[WebMethod]
		public WebsiteService.MatchData[] GetPendingMatchScores(int userid,
			string username, string password)
		{
			//authorized?
			VerifyAuthorizedUser(username, password, false, "get pending score");

			//build sql statement:
			List<SimpleParameter> parameters = new List<SimpleParameter>();
			string strSQL = "SELECT DISTINCT CHAMPIONSHIP_CATEGORY_ID, PHASE, " +
				"NGROUP, ROUND, CYCLE, MATCH, SCORE_A, SCORE_B, SENDER, PARTS_RESULT " +
				"FROM PENDING_MATCH_SCORES ";
			if (userid >= 0)
			{
				strSQL += "WHERE SENDER=@sender ";
				parameters.Add(new SimpleParameter("@sender", userid));
			}
			strSQL += "ORDER BY CHAMPIONSHIP_CATEGORY_ID ASC";

			//read results:
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL, parameters.ToArray());
			List<WebsiteService.MatchData> result = new List<MatchData>();
			table.Rows.ForEach(row =>
			{
				MatchData data = new MatchData();
				data.Category = (int)row["CHAMPIONSHIP_CATEGORY_ID"];
				data.Phase = (int)row["PHASE"];
				data.Group = (int)row["NGROUP"];
				data.Round = (int)row["ROUND"];
				data.Cycle = (int)row["CYCLE"];
				data.Match = (int)row["MATCH"];
				data.ScoreA = (int)row["SCORE_A"];
				data.ScoreB = (int)row["SCORE_B"];
				data.Sender = (int)row["SENDER"];
				data.PartsResult = row["PARTS_RESULT"].ToString();
				result.Add(data);
			});

			//done.
			return result.ToArray();
		}
		#endregion

		#region Update
		[WebMethod]
		public int UpdatePendingMatchScores(int userid,
			string username, string password, WebsiteService.MatchData[] matches)
		{
			//authorized?
			VerifyAuthorizedUser(username, password, false, "update pending score");

			//build sql statements and parameters:
			string strSelectSQL = "SELECT PENDING_MATCH_SCORE_ID " +
				"FROM PENDING_MATCH_SCORES " +
				"WHERE CHAMPIONSHIP_CATEGORY_ID=@category AND PHASE=@phase " +
				"AND NGROUP=@group AND ROUND=@round AND CYCLE=@cycle " +
				"AND MATCH=@match";
			string strUpdateSQL = "UPDATE PENDING_MATCH_SCORES " +
				"SET SCORE_A=@score1, SCORE_B=@score2, PARTS_RESULT=@pr " +
				"WHERE CHAMPIONSHIP_CATEGORY_ID=@category AND PHASE=@phase " +
				"AND NGROUP=@group AND ROUND=@round AND CYCLE=@cycle " +
				"AND MATCH=@match AND SENDER=@sender";
			string strInsertSQL = "INSERT INTO PENDING_MATCH_SCORES " +
				"(SCORE_A, SCORE_B, CHAMPIONSHIP_CATEGORY_ID, PHASE, NGROUP, " +
				"ROUND, CYCLE, MATCH, SENDER, PARTS_RESULT) VALUES (" +
				"@score1, @score2, @category, @phase, @group, @round, @cycle, " +
				"@match, @sender, @pr)";

			SimpleParameter score1Param = new SimpleParameter("@score1", DBNull.Value);
			SimpleParameter score2Param = new SimpleParameter("@score2", DBNull.Value);
			SimpleParameter categoryParam = new SimpleParameter("@category", DBNull.Value);
			SimpleParameter phaseParam = new SimpleParameter("@phase", DBNull.Value);
			SimpleParameter groupParam = new SimpleParameter("@group", DBNull.Value);
			SimpleParameter roundParam = new SimpleParameter("@round", DBNull.Value);
			SimpleParameter cycleParam = new SimpleParameter("@cycle", DBNull.Value);
			SimpleParameter matchParam = new SimpleParameter("@match", DBNull.Value);
			SimpleParameter senderParam = new SimpleParameter("@sender", userid);
			SimpleParameter prParam = new SimpleParameter("@pr", DBNull.Value);

			//iterate through matches:
			foreach (MatchData data in matches)
			{
				//apply parameter values:
				score1Param.Value = (int)data.ScoreA;
				score2Param.Value = (int)data.ScoreB;
				categoryParam.Value = data.Category;
				phaseParam.Value = data.Phase;
				groupParam.Value = data.Group;
				roundParam.Value = data.Round;
				cycleParam.Value = data.Cycle;
				matchParam.Value = data.Match;
				prParam.Value = data.PartsResult == null ? "" : data.PartsResult;

				//got existing match?
				SimpleTable table = DB.Instance.GetDataBySQL(strSelectSQL,
					categoryParam, phaseParam, groupParam, roundParam, 
					cycleParam, matchParam);
				bool blnExists = (table.Rows.Count > 0);
				
				//decide what SQL we need to execute.
				if (blnExists)
					DB.Instance.Execute(strUpdateSQL, score1Param, score2Param, 
						prParam, categoryParam, phaseParam, groupParam, roundParam, 
						cycleParam, matchParam, senderParam);
				else
					DB.Instance.Execute(strInsertSQL, score1Param, score2Param,
						categoryParam, phaseParam, groupParam, roundParam,
						cycleParam, matchParam, senderParam, prParam);
			}

			//done.
			return 0;
		}
		#endregion

		#region Commit
		[WebMethod]
		public int CommitPendingMatchScores(string username, string password,
			WebsiteService.MatchData[] matches)
		{
			//authorized?
			VerifyAuthorizedUser(username, password, true, "commit pending score");

			//build sql statements and parameters:
			string strUpdateSQL = "UPDATE CHAMPIONSHIP_MATCHES " +
				"SET TEAM_A_SCORE=@score1, TEAM_B_SCORE=@score2, RESULT=@outcome, PARTS_RESULT=@pr " +
				"WHERE CHAMPIONSHIP_CATEGORY_ID=@category AND PHASE=@phase " +
				"AND NGROUP=@group AND ROUND=@round AND CYCLE=@cycle " +
				"AND MATCH=@match";
			string strDeleteSQL = "DELETE FROM PENDING_MATCH_SCORES " +
				"WHERE CHAMPIONSHIP_CATEGORY_ID=@category AND PHASE=@phase " +
				"AND NGROUP=@group AND ROUND=@round AND CYCLE=@cycle " +
				"AND MATCH=@match";

			SimpleParameter score1Param = new SimpleParameter("@score1", DBNull.Value);
			SimpleParameter score2Param = new SimpleParameter("@score2", DBNull.Value);
			SimpleParameter outcomeParam = new SimpleParameter("@outcome", DBNull.Value);
			SimpleParameter categoryParam = new SimpleParameter("@category", DBNull.Value);
			SimpleParameter phaseParam = new SimpleParameter("@phase", DBNull.Value);
			SimpleParameter groupParam = new SimpleParameter("@group", DBNull.Value);
			SimpleParameter roundParam = new SimpleParameter("@round", DBNull.Value);
			SimpleParameter cycleParam = new SimpleParameter("@cycle", DBNull.Value);
			SimpleParameter matchParam = new SimpleParameter("@match", DBNull.Value);
			SimpleParameter prParam = new SimpleParameter("@pr", DBNull.Value);
			
			//iterate through matches:
			foreach (MatchData data in matches)
			{
				//got anything?
				if (data.ScoreA < 0 || data.ScoreB < 0)
					continue;

				//get score:
				double scoreA = data.ScoreA;
				double scoreB = data.ScoreB;

				//calculate match outcome:
				Sport.Championships.MatchOutcome outcome = Sport.Championships.MatchOutcome.None;
				if (scoreA > scoreB)
					outcome = Sport.Championships.MatchOutcome.WinA;
				else if (scoreB > scoreA)
					outcome = Sport.Championships.MatchOutcome.WinB;
				else
					outcome = Sport.Championships.MatchOutcome.Tie;

				//apply parameter values:
				score1Param.Value = (double)data.ScoreA;
				score2Param.Value = (double)data.ScoreB;
				outcomeParam.Value = (int)outcome;
				categoryParam.Value = data.Category;
				phaseParam.Value = data.Phase;
				groupParam.Value = data.Group;
				roundParam.Value = data.Round;
				cycleParam.Value = data.Cycle;
				matchParam.Value = data.Match;
				prParam.Value = data.PartsResult;

				DB.Instance.Execute(strUpdateSQL, score1Param, score2Param,
					outcomeParam, categoryParam, phaseParam, groupParam,
					roundParam, cycleParam, matchParam, prParam);

				DB.Instance.Execute(strDeleteSQL, categoryParam, phaseParam,
					groupParam, roundParam, cycleParam, matchParam);
			}

			//done.
			return 0;
		} //end function AddPendingCompetitors
		#endregion
		#endregion

		#region school matches
		[WebMethod]
		public WebsiteService.MatchData[] GetSchoolMatches(int schoolID)
		{
			//first, get supervisors for the given school.

			//build sql statement:
			string strSQL = "SELECT DISTINCT FUNCTIONARY_ID " +
				"FROM FUNCTIONARIES " +
				"WHERE SCHOOL_ID=@school " +
				"AND FUNCTIONARY_TYPE=@type " +
				"AND DATE_DELETED IS NULL";

			//read results:
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL,
				new SimpleParameter("@school", schoolID),
				new SimpleParameter("@type", (int)Sport.Types.FunctionaryType.Coordinator));
			List<int> arrFunctionaries = new List<int>();
			table.Rows.ForEach(row =>
			{
				arrFunctionaries.Add((int)row["FUNCTIONARY_ID"]);
			});

			//initialize result array:
			List<WebsiteService.MatchData> matches = new List<MatchData>();

			//iterate over functionaries and add matches
			if (arrFunctionaries.Count > 0)
			{
				//fitler:
				string strChampFilter = "%t1.CHAMPIONSHIP_CATEGORY_ID=%t2.CHAMPIONSHIP_CATEGORY_ID AND " +
					"%t1.PHASE=%t2.PHASE AND %t1.NGROUP=%t2.NGROUP";

				//build sql statement:
				strSQL = "" +
					"SELECT f.CHAMPIONSHIP_CATEGORY_ID, f.PHASE, f.NGROUP, f.ROUND, " +
					"	f.CYCLE, f.MATCH, t1.TEAM_ID AS TEAM_A, t2.TEAM_ID AS TEAM_B, " +
					"	m.[TIME], m.TEAM_A_SCORE, m.TEAM_B_SCORE, m.RESULT " +
					"FROM CHAMPIONSHIP_MATCH_FUNCTIONARIES f, " +
					"	CHAMPIONSHIP_MATCHES m, " +
					"	CHAMPIONSHIP_GROUP_TEAMS t1, " +
					"	CHAMPIONSHIP_GROUP_TEAMS t2, " + 
					"	CHAMPIONSHIP_CATEGORIES cc, " + 
					"	CHAMPIONSHIPS c " +
					"WHERE (" + strChampFilter.Replace("%t1", "f").Replace("%t2", "m") + " AND f.ROUND=m.ROUND AND f.CYCLE=m.CYCLE AND f.MATCH=m.MATCH) " +
					"AND (" + strChampFilter.Replace("%t1", "m").Replace("%t2", "t1") + " AND m.TEAM_A=t1.POSITION) " +
					"AND (" + strChampFilter.Replace("%t1", "m").Replace("%t2", "t2") + " AND m.TEAM_B=t2.POSITION) " +
					"AND f.CHAMPIONSHIP_CATEGORY_ID=cc.CHAMPIONSHIP_CATEGORY_ID " + 
					"AND cc.CHAMPIONSHIP_ID=c.CHAMPIONSHIP_ID " + 
					"AND (f.FUNCTIONARY_ID IN (" + string.Join(",", arrFunctionaries) + ")) " +
					"AND c.SEASON=(Select Max(SEASON) From SEASONS Where [STATUS]=1 And DATE_DELETED Is Null) " + 
					"AND (f.DATE_DELETED IS NULL) " +
					"ORDER BY f.CHAMPIONSHIP_CATEGORY_ID ASC, m.[TIME] ASC";

				//read results:
				table = DB.Instance.GetDataBySQL(strSQL);
				table.Rows.ForEach(row =>
				{
					int teamA = (int)row["TEAM_A"];
					int teamB = (int)row["TEAM_B"];
					if (teamA >= 0 && teamB >= 0)
					{
						MatchData data = new MatchData();
						data.Category = (int)row["CHAMPIONSHIP_CATEGORY_ID"];
						data.Phase = (int)row["PHASE"];
						data.Group = (int)row["NGROUP"];
						data.Round = (int)row["ROUND"];
						data.Cycle = (int)row["CYCLE"];
						data.Match = (int)row["MATCH"];
						data.TeamA = teamA;
						data.TeamB = teamB;
						data.Time = (DateTime)row["TIME"];
						data.ScoreA = Double.Parse(row["TEAM_A_SCORE"].ToString());
						data.ScoreB = Double.Parse(row["TEAM_B_SCORE"].ToString());
						data.Outcome = row.GetIntOrDefault("RESULT", (int)Sport.Championships.MatchOutcome.None);
						matches.Add(data);
					}
				});
			}

			//done.
			return matches.ToArray();
		}
		#endregion

		#region General Stuff
		#region user verification
		private void VerifyAuthorizedUser(string username, string password,
			bool blnFullAccess, string caption)
		{
			if (!Common.IsAuthorized(username, password, blnFullAccess))
				throw new Exception("can't " + caption + ": Not Authorized");
		}
		#endregion

		[WebMethod]
		public Sport.Data.SportDailyGames[] GetSportDailyGames()
		{
			string strSQL = "Select [REGION_ID], [SPORT_ID], [DAY], [CATEGORY] From WEBSITE_DAILY_SPORT_GAMES";
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL);
			List<Sport.Data.SportDailyGames> dailyGames = new List<Sport.Data.SportDailyGames>();
			table.Rows.ForEach(row =>
			{
				dailyGames.Add(new  Sport.Data.SportDailyGames
				{
					RegionId = (int)row["REGION_ID"],
					SportId = (int)row["SPORT_ID"],
					Day = (int)row["DAY"],
					Category = row["CATEGORY"] + ""
				});
			});
			return dailyGames.ToArray();
		}

		[WebMethod]
		public Sport.Data.FacilityHostDay[] GetFacilityHostDays()
		{
			string strSQL = "Select [REGION_ID], [ROW_INDEX], [DAY] From WEBSITE_FACILITY_HOST_DAYS";
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL);
			List<Sport.Data.FacilityHostDay> hostDays = new List<Sport.Data.FacilityHostDay>();
			table.Rows.ForEach(row =>
			{
				hostDays.Add(new Sport.Data.FacilityHostDay
				{
					RegionId = (int)row["REGION_ID"],
					Index = (int)row["ROW_INDEX"],
					Day = row["DAY"] + ""
				});
			});
			return hostDays.ToArray();
		}

		[WebMethod]
		public Sport.Data.FullStudentData[] GetTeamStudents(int teamId)
		{
			string strSQL = "Select Distinct A.STUDENT_ID, A.[Type], s.GRADE, s.LAST_NAME, s.FIRST_NAME, s.ID_NUMBER, s.SCHOOL_ID, s.BIRTH_DATE " +
				"From (Select STUDENT_ID, 'PLAYER' As [Type] From PLAYERS Where DATE_DELETED Is Null And TEAM_ID=@team " + 
				"	Union " +
				"	Select STUDENT_ID, 'PENDING' As [Type] From PENDING_PLAYERS Where DATE_DELETED Is Null And TEAM_ID=@team " + 
				"	Union " + 
				"	Select STUDENT_ID, 'SCHOOL' As [Type] From STUDENTS Where DATE_DELETED Is Null And SCHOOL_ID=(Select SCHOOL_ID From TEAMS Where TEAM_ID=@team) " + 
				") A Inner Join STUDENTS s ON A.STUDENT_ID=s.STUDENT_ID " + 
				"Where s.DATE_DELETED Is Null";
			Dictionary<string, Sport.Data.FullStudentType> typeMapping = new Dictionary<string, Sport.Data.FullStudentType>();
			typeMapping.Add("PLAYER", Sport.Data.FullStudentType.Player);
			typeMapping.Add("PENDING", Sport.Data.FullStudentType.Pending);
			typeMapping.Add("SCHOOL", Sport.Data.FullStudentType.School);
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL, new SimpleParameter("@team", teamId));
			List<Sport.Data.FullStudentData> students = new List<Sport.Data.FullStudentData>();
			table.Rows.ForEach(row =>
			{
				students.Add(new Sport.Data.FullStudentData
				{
					Id = (int)row["STUDENT_ID"], 
					Type = typeMapping[row["Type"].ToString()], 
					Grade = (int)row["GRADE"], 
					LastName = row["LAST_NAME"] + "", 
					FirstName = row["FIRST_NAME"] + "", 
					IdNumber = row["ID_NUMBER"] + "", 
					SchoolId = (int)row["SCHOOL_ID"], 
					BirthDate = Tools.CDateTimeDef(row["BIRTH_DATE"], DateTime.MinValue)
				});
			});
			return students.ToArray();
		}

		[WebMethod]
		public Sport.Data.FullTeamData GetFullTeamDetails(int teamId)
		{
			string strSQL = "Select t.TEAM_ID, t.[STATUS] As TeamStatus, IsNull(t.TEAM_INDEX, 0) As TeamIndex, t.REGISTRATION_DATE, t.SCHOOL_ID,  " + 
				"	s.SCHOOL_NAME, s.SYMBOL, s.CITY_ID, cit.CITY_NAME, cc.CHAMPIONSHIP_CATEGORY_ID, cc.CATEGORY, c.CHAMPIONSHIP_ID, " + 
				"	c.CHAMPIONSHIP_NAME, c.CHAMPIONSHIP_STATUS, sp.SPORT_ID, sp.SPORT_NAME, r.REGION_ID, r.REGION_NAME " + 
				"From TEAMS t Inner Join SCHOOLS s ON t.SCHOOL_ID=s.SCHOOL_ID " + 
				"	Inner Join CHAMPIONSHIP_CATEGORIES cc On t.CHAMPIONSHIP_CATEGORY_ID=cc.CHAMPIONSHIP_CATEGORY_ID " + 
				"	Inner Join CHAMPIONSHIPS c On cc.CHAMPIONSHIP_ID=c.CHAMPIONSHIP_ID " + 
				"	Inner Join SPORTS sp On c.SPORT_ID=sp.SPORT_ID " + 
				"	Left Join REGIONS r On c.REGION_ID=r.REGION_ID " + 
				"	Left Join CITIES cit On s.CITY_ID=cit.CITY_ID " + 
				"Where t.TEAM_ID=@team";
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL, new SimpleParameter("@team", teamId));
			Sport.Types.CategoryTypeLookup categoryLookup = new Sport.Types.CategoryTypeLookup();
			Sport.Types.GradeTypeLookup gradeLookup = new Sport.Types.GradeTypeLookup(true);
			List<PlayerData> players = new List<PlayerData>();
			Sport.Data.FullTeamData fullTeamData = new Sport.Data.FullTeamData
			{
				Id = -1
			};
			if (table.Rows.Count > 0)
			{
				SimpleRow row = table.Rows[0];
				int category = (int)row["CATEGORY"];
				fullTeamData.Id = teamId;
				string cityName = row["CITY_NAME"] + "";
				fullTeamData.TeamStatus = (int)row["TeamStatus"];
				fullTeamData.ChampionshipStatus = (int)row["CHAMPIONSHIP_STATUS"];
				fullTeamData.Index = (int)row["TeamIndex"];
				fullTeamData.RegistrationDate = (DateTime)row["REGISTRATION_DATE"];
				fullTeamData.School = new Sport.Data.SchoolData
				{
					Id = (int)row["SCHOOL_ID"],
					Name = row["SCHOOL_NAME"] + "", 
					Symbol = row["SYMBOL"] + ""
				};
				fullTeamData.City = cityName.Length > 0 ? row.ToSimpleData("CITY_ID", "CITY_NAME") : SimpleData.Empty;
				fullTeamData.Category = new Sport.Data.ChampionshipCategoryData
				{
					Id = (int)row["CHAMPIONSHIP_CATEGORY_ID"], 
					Category = category, 
					Name = categoryLookup.Lookup(category)
				};
				fullTeamData.Championship = row.ToSimpleData("CHAMPIONSHIP_ID", "CHAMPIONSHIP_NAME");
				fullTeamData.Sport = row.ToSimpleData("SPORT_ID", "SPORT_NAME");
				fullTeamData.Region = row.ToSimpleData("REGION_ID", "REGION_NAME");
				fullTeamData.ChampionshipStatus = (int)row["CHAMPIONSHIP_STATUS"];
			}

			if (fullTeamData.Id > 0)
			{
				strSQL = "Select p.PLAYER_ID, p.[STATUS], p.TEAM_NUMBER, p.STUDENT_ID, s.FIRST_NAME, s.LAST_NAME, s.GRADE " + 
					"From PLAYERS p Inner Join STUDENTS s On p.STUDENT_ID=s.STUDENT_ID " + 
					"Where p.TEAM_ID=@team";
				table = DB.Instance.GetDataBySQL(strSQL, new SimpleParameter("@team", teamId));
				fullTeamData.Players = new List<Sport.Data.PlayerData>();
				table.Rows.ForEach(row =>
				{
					fullTeamData.Players.Add(new Sport.Data.PlayerData
					{
						Id = (int)row["PLAYER_ID"],
						Status = (int)row["STATUS"],
						TeamNumber = (int)row["TEAM_NUMBER"],
						Student = new Sport.Data.StudentData
						{
							Id = (int)row["STUDENT_ID"],
							FirstName = row["FIRST_NAME"] + "",
							LastName = row["LAST_NAME"] + ""
						},
						Grade = row.ToSimpleData("GRADE", "", gradeLookup.Lookup((int)row["GRADE"]))
					});
				});
			}

			return fullTeamData;
		}

		[WebMethod]
		public Sport.Data.ChampionshipData[] GetChampionshipsBySeason(int season, int region)
		{
			string strSQL = "Select Distinct c.SPORT_ID, s.SPORT_NAME, c.CHAMPIONSHIP_ID, c.CHAMPIONSHIP_NAME, c.REGION_ID, " +
				"	r.REGION_NAME, cc.CHAMPIONSHIP_CATEGORY_ID, cc.CATEGORY " +
				"From CHAMPIONSHIPS c Inner Join REGIONS r On c.REGION_ID=r.REGION_ID " +
				"	Inner Join SPORTS s On c.SPORT_ID=s.SPORT_ID " +
				"	Left Join CHAMPIONSHIP_CATEGORIES cc On c.CHAMPIONSHIP_ID=cc.CHAMPIONSHIP_ID ";
			if (region >= 0)
				strSQL += "	Left Join CHAMPIONSHIP_REGIONS cr On c.CHAMPIONSHIP_ID=cr.CHAMPIONSHIP_ID ";
			strSQL += "Where c.SEASON=@season ";
			if (region >= 0)
				strSQL += "And cr.REGION_ID=@region ";
			strSQL += "Order By s.SPORT_NAME, c.CHAMPIONSHIP_ID, cc.CHAMPIONSHIP_CATEGORY_ID";
			List<SimpleParameter> parameters = new List<SimpleParameter>();
			parameters.Add(new SimpleParameter("@season", season));
			if (region >= 0)
				parameters.Add(new SimpleParameter("@region", region));
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL, parameters.ToArray());
			Sport.Types.CategoryTypeLookup categoryLookup = new Sport.Types.CategoryTypeLookup();
			Dictionary<int, Sport.Data.ChampionshipData> champMapping = new Dictionary<int, Sport.Data.ChampionshipData>();
			Dictionary<int, List<Sport.Data.ChampionshipCategoryData>> categoriesMapping = new Dictionary<int, List<Sport.Data.ChampionshipCategoryData>>();
			List<int> sortedChampionships = new List<int>();
			table.Rows.ForEach(row =>
			{
				int curChampId = (int)row["CHAMPIONSHIP_ID"];
				int rawCategory = (int)row["CATEGORY"];
				if (!champMapping.ContainsKey(curChampId))
				{
					champMapping.Add(curChampId, new Sport.Data.ChampionshipData
					{
						SportId = (int)row["SPORT_ID"],
						SportName = row["SPORT_NAME"] + "",
						ChampionshipId = curChampId,
						ChampionshipName = row["CHAMPIONSHIP_NAME"] + "",
						RegionId = (int)row["REGION_ID"],
						RegionName = row["REGION_NAME"] + ""
					});
					categoriesMapping.Add(curChampId, new List<Sport.Data.ChampionshipCategoryData>());
					sortedChampionships.Add(curChampId);
				}
				categoriesMapping[curChampId].Add(new Sport.Data.ChampionshipCategoryData
				{
					Id = (int)row["CHAMPIONSHIP_CATEGORY_ID"],
					Category = rawCategory,
					Name = categoryLookup.Lookup(rawCategory)
				});
			});

			List<Sport.Data.ChampionshipData> championships = sortedChampionships.ConvertAll(championshipId =>
			{
				Sport.Data.ChampionshipData curData = champMapping[championshipId];
				curData.Categories = categoriesMapping[championshipId].ToArray();
				return curData;
			});

			return championships.ToArray();
		}

		[WebMethod]
		public int MapExternalStudent(int studentId, int userId, string ipAddress)
		{
			string strSQL = "Insert Into [EXTERNAL_STUDENTS_MAPPING] ([STUDENT_ID], [USER_ID], [IP_ADDRESS], [DATE_ADDED]) " + 
				"Values (@student, @user, @ipaddress, @date)";
			return DB.Instance.Execute(strSQL, new SimpleParameter("@student", studentId),
				new SimpleParameter("@user", userId), new SimpleParameter("@ipaddress", ipAddress),
				new SimpleParameter("@date", DateTime.Now));
		}

		[WebMethod]
		public bool IsExternallyAddedStudent(int idNumber)
		{
			string strSQL = "SELECT CAST(" + 
				"CASE WHEN EXISTS("  + 
				"SELECT e.STUDENT_ID FROM EXTERNAL_STUDENTS_MAPPING e INNER JOIN STUDENTS s On e.STUDENT_ID=s.STUDENT_ID " + 
				"WHERE s.ID_NUMBER=@id " + 
				") THEN 1 " + 
				"ELSE 0 " + 
				"END " + 
				"AS INT)";
			return (DB.Instance.ExecuteScalar(strSQL, "0", new SimpleParameter("@id", idNumber)) + "").Equals("1");
		}

		/// <summary>
		/// parse the string to array of links.
		/// </summary>
		private LinkData[] StringToLinks(string strLinks)
		{
			//split by main delimeter:
			string[] arrLinks = Sport.Common.Tools.SplitNoBlank(strLinks, LinkSeperator);

			//initialize result array:
			LinkData[] result = new LinkData[arrLinks.Length];

			//iterate over the links, add each to result array.
			for (int i = 0; i < arrLinks.Length; i++)
			{
				//build current link:
				result[i] = new LinkData();

				//split by secondary delimeter:
				string[] arrLinkParts =
					Sport.Common.Tools.SplitNoBlank(arrLinks[i], LinkTextSeperator);

				//extract URL and assign to current link:
				string strURL = Sport.Common.Tools.MakeValidURL(arrLinkParts[0]);
				result[i].URL = strURL;

				//extract text, if exists:
				string strText = "";
				if (arrLinkParts.Length > 1)
				{
					strText = Common.ToStringDef(arrLinkParts[1], "");
				}

				//got anything? (if not, use URL as the text)
				if (strText.Length == 0)
					strText = strURL;

				//assign to current link:
				result[i].Text = strText;
			} //end loop over the links

			//done.
			return result;
		} //end function StringToLinks

		/// <summary>
		/// returns data for user with given ID. (entities are not stable)
		/// </summary>
		[WebMethod(EnableSession = true)]
		public UserData GetUserData(int userID)
		{
			UserData result = new UserData();

			//build sql statement:
			string strSQL = "SELECT * FROM USERS WHERE USER_ID=@1 " +
				"AND DATE_DELETED IS NULL";

			//read results:
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL, 
				new SimpleParameter("@1", userID));
			if (table.Rows.Count > 0)
			{
				SimpleRow row = table.Rows[0];
				result.Id = userID;
				string strFirstName = row["USER_FIRST_NAME"].ToString();
				string strLastName = row["USER_LAST_NAME"].ToString();
				result.Name = strFirstName;
				if (strLastName.Length > 0)
					result.Name += " " + strLastName;
				result.UserRegion = row.GetIntOrDefault("REGION_ID", -1);
				result.UserSchool = row.GetIntOrDefault("SCHOOL_ID", -1);
			}
			
			//done.
			return result;
		}
		#endregion
	}
}
