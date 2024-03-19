using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SportSite.Common;
using SportSite.Core;
using System.IO;

namespace SportSite.NewRegister
{
	public partial class TestZone : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			//lblArticle_Sub.Attributes["class"] = "btn btn-default active";
			//rbArticle_Sub.Checked = true;

			UserData loggedInUser;
			if (TryGetLoggedInUser(out loggedInUser))
				imgLoggedIn.Visible = true;

			tblArticles.AdditionalButtons = new string[] { "btnSetHomepageArticles" };
			List<PagedTableData.TableRow> rows = BuildDummyArticles();
			tblArticles.SetTableData(new PagedTableData
			{
				Columns = (new PagedTableData.TableColumn[]
				{
					new PagedTableData.TableColumn { Caption = "זיהוי פנימי", Name="Id" }, 
					new PagedTableData.TableColumn { Caption = "כותרת הכתבה", Name="Title" }, 
					new PagedTableData.TableColumn { Caption = "תאריך שליחה", Name="Date" }, 
					new PagedTableData.TableColumn { Caption = "מאת", Name="Author" }, 
					new PagedTableData.TableColumn { Caption = "נשלחה על ידי", Name="SubmittedBy" }
				}).ToList(),
				DefaultSort = new PagedTableData.TableSort
				{
					ColumnName = "Date",
					Direction = SortDirection.Descending
				},
				Rows = rows,
				PageSize = 20
			});

			if (this.IsPostBack)
			{
				bool isArticleDeleted = (hidDeleteArticle.Value == "yes");
				List<string> lines = new List<string>();
				if (isArticleDeleted)
				{
					lines.Add("Article is being deleted...");
				}
				else
				{
					string uploadToken = hidUploadToken.Value;
					string articleCaption = txtCaption.Value;
					string articleSubCaption = txtSubCaption.Value;
					string articleAuthor = txtAuthor.Value;
					bool isHotLink = chkHotLink.Checked;
					bool isPrimaryArticle = rbArticle_Main.Checked;
					bool isSubArticle = rbArticle_Sub.Checked;
					bool isRegionalArticle = chkRegionalArticle.Checked;
					string articleContents = txtArticleContents.Value;
					List<KeyValuePair<string, string>> articleLinks = GetArticleLinksFromRequest();
					List<string> articleImages = GetArticleImagesFromRequest(uploadToken);
					List<string> imagesToDelete = imagesToRemove.Value.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).ToList();
					List<ArticleAttachmentData> articleAttachments = GetArticleAttachmentsFromRequest(uploadToken);
					List<string> attachmentsToDelete = attachmentsToRemove.Value.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).ToList();
					lines.AddRange(new KeyValuePair<string, string>[] {
						new KeyValuePair<string, string>("Upload token", uploadToken), 
						new KeyValuePair<string, string>("Caption", articleCaption), 
						new KeyValuePair<string, string>("Sub caption", articleSubCaption), 
						new KeyValuePair<string, string>("Author", articleAuthor), 
						new KeyValuePair<string, string>("Hot link?", isHotLink.ToString()), 
						new KeyValuePair<string, string>("Primary?", isPrimaryArticle.ToString()), 
						new KeyValuePair<string, string>("Sub?", isSubArticle.ToString()), 
						new KeyValuePair<string, string>("Regional?", isRegionalArticle.ToString()), 
						new KeyValuePair<string, string>("Region", ddlArticleRegion.Value), 
						new KeyValuePair<string, string>("Contents", articleContents), 
						new KeyValuePair<string, string>("Links", string.Join("\n", articleLinks.ConvertAll(p => string.Format("[{0}]({1})", p.Key, p.Value)))), 
						new KeyValuePair<string, string>("Images", string.Join(", ", articleImages)), 
						new KeyValuePair<string, string>("Images to remove", string.Join(", ", imagesToDelete)), 
						new KeyValuePair<string, string>("Attachments", string.Join("\n", articleAttachments.ConvertAll(d => d.ToString()))), 
						new KeyValuePair<string, string>("Attachments to remove", string.Join(", ", attachmentsToDelete))
					}.ToList().ConvertAll(pair => string.Format("{0}{1} {2}", pair.Key, (pair.Key.EndsWith("?")) ? "" : ":", pair.Value)));
				}
				System.IO.File.WriteAllLines(Server.MapPath("TestZoneOutput.txt"), lines.ToArray(), System.Text.Encoding.GetEncoding("ISO-8859-8"));
				metRedirect.Visible = true;
				cssMain.Visible = false;
				cssArticleSubmitted.Visible = true;
				FindRelevantStylesheet(isArticleDeleted).Visible = true;
				return;
			}

			hidUploadToken.Value = Guid.NewGuid().ToString("N");
			Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "article_id", "var _articleId=" +
				Mir.Common.RandomHelper.GetInt(1, 9500) + "; ", true);

			Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "attachment_valid_extensions", "var _validAttachmentExtensions = [" +
				string.Join(", ", SportSite.Core.AttachmentManager.AllowedExtensions.ConvertAll(extensionData => {
					return SportSite.Common.Tools.BuildJsonString(new KeyValuePair<string, string>("Description", extensionData.Description),
						new KeyValuePair<string, string>("Extensions", "[" + string.Join(", ", extensionData.Extensions.ToList().ConvertAll(x =>
							string.Format("'{0}'", x))) + "]"));
				})) + "]; ", true);
			
			ddlArticleRegion.Items.Add(new ListItem("ירושלים", "1"));
			ddlArticleRegion.Items.Add(new ListItem("תל אביב", "2"));
			ddlArticleRegion.Items.Add(new ListItem("צפון", "3"));
			new List<Image>(new Image[] { imgFirstPicture, imgSecondPicture, imgThirdPicture, imgFourthPicture }).ForEach(img => AssignRandomPicture(img));

			List<ArticleLinkData> randomLinks = new List<ArticleLinkData>();
			var linkCount = Mir.Common.RandomHelper.GetInt(0, 6);
			for (int i = 1; i <= linkCount; i++)
			{
				randomLinks.Add(new ArticleLinkData
				{
					Description = Mir.Common.RandomHelper.GetLoremIpsum(Mir.Common.RandomHelper.GetInt(3, 10)), 
					Url = "http://www." + Mir.Common.RandomHelper.GetString(Mir.Common.RandomHelper.GetInt(3, 10), false, true) + 
						Mir.Common.RandomHelper.GetItem<string>(new string[] {".com", ".co.il", ".net"})
				});
			}
			Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "article_links", "var _articleLinks = [" +
				string.Join(", ", randomLinks) + "]; ", true);

			List<ArticleLinkData> randomAttachments = new List<ArticleLinkData>();
			var attachmentsCount = Mir.Common.RandomHelper.GetInt(0, 3);
			for (int i = 1; i <= attachmentsCount; i++)
			{
				randomAttachments.Add(GetRandomAttachment());
			}
			Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "article_attachments", "var _articleAttachments = [" +
				string.Join(", ", randomAttachments) + "]; ", true);
		}

		private List<PagedTableData.TableRow> BuildDummyArticles()
		{
			List<PagedTableData.TableRow> rows = new List<PagedTableData.TableRow>();
			string[] possibleArticleIdNumbers = File.ReadAllLines(Server.MapPath("existing_articles.txt"));
			List<string> randomIdNumbers = Mir.Common.RandomHelper.GetItems<string>(possibleArticleIdNumbers, 
				Mir.Common.RandomHelper.GetInt(100, 1200)).ToList();
			if (!randomIdNumbers.Contains("9002"))
				randomIdNumbers.Add("9002");
			if (!randomIdNumbers.Contains("9004"))
				randomIdNumbers.Add("9004");
			if (!randomIdNumbers.Contains("9006"))
				randomIdNumbers.Add("9006");
			for (int i = 0; i < randomIdNumbers.Count; i++)
			{
				int dummyId;
				if (!Int32.TryParse(randomIdNumbers[i], out dummyId))
					dummyId = Mir.Common.RandomHelper.GetInt(10, 10000);
				string dummyCaption = Mir.Common.RandomHelper.GetLoremIpsum(Mir.Common.RandomHelper.GetInt(1, 5));
				DateTime dummyDate = Mir.Common.RandomHelper.GetDateTime(2013, 1, 1, 2016, 5, 1);
				string dummyAuthor = Mir.Common.RandomHelper.GetString(Mir.Common.RandomHelper.GetInt(3, 10), false, true);
				string dummySubmittedBy = Mir.Common.RandomHelper.GetString(Mir.Common.RandomHelper.GetInt(3, 10), false, true);
				rows.Add(new PagedTableData.TableRow
				{
					Cells = (new PagedTableData.TableCell[]
						{
							new PagedTableData.TableCell { Contents = dummyId.ToString(), Value = dummyId }, 
							new PagedTableData.TableCell { Contents = dummyCaption }, 
							new PagedTableData.TableCell { Contents = dummyDate.ToString("dd/MM/yyyy"), Value = dummyDate.Ticks }, 
							new PagedTableData.TableCell { Contents = dummyAuthor }, 
							new PagedTableData.TableCell { Contents = dummySubmittedBy }
						}).ToList(),
					TargetUrl = "?edit=" + dummyId
				});
			}
			return rows;
		}

		protected List<ArticleAttachmentData> GetArticleAttachmentsFromRequest(string uploadToken)
		{
			List<Dictionary<string, string>> allItems = SportSite.Common.Tools.GetRequestCollection("ArticleAttachmentDescription",
				"ArticleAttachmentId", "ArticleAttachmentToken");

			var articleAttachments = allItems.ConvertAll(curItem => new ArticleAttachmentData
			{
				Description = curItem["ArticleAttachmentDescription"],
				AttachmentId = curItem["ArticleAttachmentId"].ToIntOrDefault(0),
				FileToken = curItem["ArticleAttachmentToken"]
			}); ;

			UserData loggedInUser;
			string errorMsg, attachmentFilePath;
			if (TryGetLoggedInUser(out loggedInUser))
			{
				articleAttachments.FindAll(a => a.AttachmentId == 0 && a.FileToken.Length > 0).ForEach(curAttachment =>
				{
					if (UploadManager.TryGetFilePath(uploadToken, curAttachment.FileToken, out errorMsg, out attachmentFilePath))
					{
						if (curAttachment.Description.Length == 0)
							curAttachment.Description = System.IO.Path.GetFileName(attachmentFilePath);
						WebSiteServices.AttachmentData webAttachment = AttachmentManager.AddAttachment(-1, attachmentFilePath,
							curAttachment.Description, loggedInUser);
						if (webAttachment != null)
							curAttachment.AttachmentId = webAttachment.ID;
					}
				});
			}

			return articleAttachments;
		}

		protected List<string> GetArticleImagesFromRequest(string uploadToken)
		{
			string[] imageTokens = (Request.Form["ArticleImages"] + "").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			string existingImagePath, errorMsg;
			return imageTokens.ToList().ConvertAll(imageToken =>
			{
				if (UploadManager.TryGetFilePath(uploadToken, imageToken, out errorMsg, out existingImagePath))
					return SportSite.Common.Tools.GetArticleImage(-1, existingImagePath);
				return "";
			});
		}

		protected List<KeyValuePair<string, string>> GetArticleLinksFromRequest()
		{
			List<Dictionary<string, string>> allItems = SportSite.Common.Tools.GetRequestCollection("ArticleLinkUrl", "ArticleLinkDescription");
			return allItems.ConvertAll(curItem => new KeyValuePair<string, string>(curItem["ArticleLinkUrl"], curItem["ArticleLinkDescription"]));
		}

		private bool TryGetLoggedInUser(out UserData userData)
		{
			userData = UserData.Empty;
			if (Session[UserManager.SessionKey] == null)
				return false;

			userData = (UserData)Session[UserManager.SessionKey];
			return userData.Id > 0;
		}

		private System.Web.UI.HtmlControls.HtmlGenericControl FindRelevantStylesheet(bool isArticleDeleted)
		{
			if (isArticleDeleted)
				return cssArticleDeleted;
			return (Request.QueryString["edited"] == "1") ? cssArticleEdited : cssArticleAdded;
		}

		private ArticleLinkData GetRandomAttachment()
		{
			string relativeFolderPath = "~/Attachments";
			string[] allAttachments = System.IO.Directory.GetFiles(Server.MapPath(relativeFolderPath));
			string randomAttachmentPath = Mir.Common.RandomHelper.GetItem<string>(allAttachments);
			return new ArticleLinkData
			{
				Description = System.IO.Path.GetFileName(randomAttachmentPath), 
				Url = "/Attachments/" + System.IO.Path.GetFileName(randomAttachmentPath), 
				Id = Array.IndexOf<string>(allAttachments, randomAttachmentPath) 
			};
		}

		private void AssignRandomPicture(Image imageControl)
		{
			if (Mir.Common.RandomHelper.Chance(20))
			{
				string relativeFolderPath = "~/Images/Articles";
				string[] allImages = System.IO.Directory.GetFiles(Server.MapPath(relativeFolderPath));
				string randomImagePath = Mir.Common.RandomHelper.GetItem<string>(allImages);
				string relativeImagePath = "~/Images/Articles/" + System.IO.Path.GetFileName(randomImagePath);
				imageControl.ImageUrl = relativeImagePath;
			}
		}

		private class ArticleLinkData
		{
			public string Description { get; set; }
			public string Url { get; set; }
			public int Id { get; set; }
			public override string ToString()
			{
				return SportSite.Common.Tools.BuildJsonString(new KeyValuePair<string, string>("Description", this.Description),
					new KeyValuePair<string, string>("Url", this.Url),
					new KeyValuePair<string, string>("Id", this.Id.ToString()));
			}
		}

		public class ArticleAttachmentData
		{
			public string Description { get; set; }
			public int AttachmentId { get; set; }
			public string FileToken { get; set; }

			public override string ToString()
			{
				return string.Format("Description: {0}, Id: {1}, Token: {2}", this.Description, this.AttachmentId, this.FileToken);
			}
		}
	}
}