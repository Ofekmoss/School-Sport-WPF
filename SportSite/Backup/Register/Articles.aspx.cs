using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI.HtmlControls;
using SportSite.Common;
using SportSite.Core;

namespace SportSite.NewRegister
{
	public partial class Articles : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			(this.Master as SportSite.NewRegister.Register).RequiresAdminAccess = true;
			AssignRegions();
			if (!HandleArticleActions())
			{
				tblArticles.AdditionalButtons = new string[] { "btnSetHomepageArticles" };
				LoadAllArticles();
			}
		}

		#region Article Actions
		private bool HandleArticleActions()
		{
			if (this.IsPostBack)
			{
				var loggedInUser = (this.Master as SportSite.NewRegister.Register).LoggedInUser;
				if (loggedInUser.Id == Core.UserData.Empty.Id)
					throw new Exception("Permission Denied");

				bool isArticleDeleted = hidDeleteArticle.Value == "yes";
				if (isArticleDeleted)
				{
					//delete
					int articleId = Int32.Parse(hidArticleId.Value);
					DeleteArticle(articleId, loggedInUser);
				}
				else
				{
					//edit
					AddOrEditArticle(loggedInUser);
				}
				(this.Master as SportSite.NewRegister.Register).OutsideFormVisible = true;
				metRedirect.Visible = true;
				cssArticleSubmitted.Visible = true;
				FindRelevantStylesheet(isArticleDeleted).Visible = true;
				return true;
			}

			WebSiteServices.ArticleData articleData;
			Page.ClientScript.RegisterClientScriptInclude(this.Page.GetType(), "main", "Articles.js");
			Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "attachment_valid_extensions", "var _validAttachmentExtensions = [" +
				string.Join(", ", SportSite.Core.AttachmentManager.AllowedExtensions.ConvertAll(extensionData =>
				{
					return SportSite.Common.Tools.BuildJsonString(new KeyValuePair<string, string>("Description", extensionData.Description),
						new KeyValuePair<string, string>("Extensions", "[" + string.Join(", ", extensionData.Extensions.ToList().ConvertAll(x =>
							string.Format("'{0}'", x))) + "]"));
				})) + "]; ", true);
			mainCssLink.Visible = true;
			if (TryGetArticleFromRequest(out articleData))
			{
				bool isNew = articleData.ID < 0;
				(this.Master as SportSite.NewRegister.Register).OutsideFormVisible = true;
				tblArticles.Visible = false;
				pnlAddOrEdit.Visible = true;
				
				lbPanelTitle.InnerHtml = isNew ? "הוספת כתבה חדשה" : "עריכת כתבה קיימת";
				hidUploadToken.Value = Guid.NewGuid().ToString("N");
				if (isNew)
				{
					txtAuthor.Value = (this.Master as Register).LoggedInUser.Name;
				}
				else
				{
					AssignArticleData(articleData);
					pnlDeleteArticle.Visible = true;
				}
				return true;
			}
			return false;
		}

		private void DeleteArticle(int articleId, Core.UserData loggedInUser)
		{
			using (WebSiteServices.WebsiteService service = new WebSiteServices.WebsiteService())
			{
				service.DeleteArticle(articleId, loggedInUser.Login, loggedInUser.Password);
			}
		}

		private void AddOrEditArticle(Core.UserData loggedInUser)
		{
			WebSiteServices.ArticleData articleData;
			if (!TryGetArticle(hidArticleId.Value.GetDefaultValueIfEmpty("-1"), out articleData))
				throw new Exception("זיהוי כתבה שגוי");

			int articleId = articleData.ID;
			bool isNewArticle = articleId < 0;
			string uploadToken = hidUploadToken.Value;
			articleData.Caption = txtCaption.Value;
			articleData.SubCaption = txtSubCaption.Value;
			articleData.AuthorName = txtAuthor.Value;
			articleData.IsHotLink = chkHotLink.Checked;
			articleData.IsPrimary = rbArticle_Main.Checked;
			articleData.IsSub = rbArticle_Sub.Checked;
			articleData.Contents = txtArticleContents.Value;
			articleData.ArticleRegion = GetArticleRegionFromRequest();
			articleData.IsClubsArticle = false;
			articleData.Links = GetArticleLinksFromRequest().ConvertAll(l => new WebSiteServices.LinkData
			{
				URL = l.Key, 
				Text = l.Value
			}).ToArray();
			articleData.Images = ApplyArticleImagesFromRequest(articleId, uploadToken, articleData.Images);
			articleData.Attachments = ApplyArticleAttachmentsFromRequest(articleId, uploadToken, articleData.Attachments, loggedInUser);
			using (WebSiteServices.WebsiteService service = new WebSiteServices.WebsiteService())
			{
				service.UpdateArticle(articleData, loggedInUser.Login, loggedInUser.Password, loggedInUser.Id);
			}
		}
		#endregion

		#region Load Articles
		private void LoadAllArticles()
		{
			List<PagedTableData.TableRow> rows = new List<PagedTableData.TableRow>();
			using (WebSiteServices.WebsiteService service = new WebSiteServices.WebsiteService())
			{
				rows.AddRange(service.GetArticles(-1).ToList().ConvertAll(articleData =>
				{
					DateTime submitDate = articleData.Time;
					string submittedBy = articleData.User;
					string editUrl = "?edit=" + articleData.ID;
					return new PagedTableData.TableRow
					{
						Cells = (new PagedTableData.TableCell[]
						{
							new PagedTableData.TableCell { Contents = articleData.ID.ToString(), Value = articleData.ID }, 
							new PagedTableData.TableCell { Contents = articleData.Caption }, 
							new PagedTableData.TableCell { Contents = submitDate.ToString("dd/MM/yyyy"), Value = submitDate.Ticks }, 
							new PagedTableData.TableCell { Contents = articleData.AuthorName }, 
							new PagedTableData.TableCell { Contents = submittedBy }
						}).ToList(),
						TargetUrl = editUrl
					};
				}));
			}
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
		}
		#endregion

		#region Assign Data
		private void AssignRegions()
		{
			List<Sport.Data.Entity> allRegions = Sport.Entities.Region.Type.GetEntities(null).ToList();
			allRegions.Sort((r1, r2) => r1.Id.CompareTo(r2.Id));
			Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "article_regions", "var _articleRegions = [" +
				string.Join(", ", allRegions.ConvertAll(r => Tools.BuildJsonString(new KeyValuePair<string, string>("Id", r.Id.ToString()),
					new KeyValuePair<string, string>("Name", r.Name)))) + "]; ", true);
		}

		private void AssignRadioButton(HtmlInputRadioButton button, HtmlGenericControl label, bool isChecked)
		{
			button.Checked = isChecked;
			if (isChecked)
				label.Attributes["class"] = "btn btn-default active";
		}

		private void AssignArticleData(WebSiteServices.ArticleData articleData)
		{
			hidArticleId.Value = articleData.ID.ToString();
			txtCaption.Value = articleData.Caption;
			txtSubCaption.Value = articleData.SubCaption;
			txtAuthor.Value = articleData.AuthorName;
			chkHotLink.Checked = articleData.IsHotLink;
			AssignRadioButton(rbArticle_Main, lblArticle_Main, articleData.IsPrimary);
			AssignRadioButton(rbArticle_Sub, lblArticle_Sub, articleData.IsSub);
			if (articleData.ArticleRegion > 0)
			{
				chkRegionalArticle.Checked = true;
				chkRegionalArticle.Attributes["data-region"] = articleData.ArticleRegion.ToString();
			}
			txtArticleContents.InnerHtml = articleData.Contents;
			if (articleData.Images != null && articleData.Images.Length > 0)
				AssignArticleImages(articleData.Images);
			if (articleData.Links != null)
			{
				List<WebSiteServices.LinkData> validLinks = articleData.Links.ToList().FindAll(l => (l.URL + "").Length > 0);
				if (validLinks.Count > 0)
					AssignArticleLinks(validLinks);
			}
			if (articleData.Attachments != null)
			{
				List<WebSiteServices.AttachmentData> validAttachments = articleData.Attachments.ToList().FindAll(a => a.ID > 0);
				if (validAttachments.Count > 0)
					AssignArticleAttachments(validAttachments);
			}
		}

		private void AssignArticleLinks(List<WebSiteServices.LinkData> links)
		{
			Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "article_links", "var _articleLinks = [" +
				string.Join(", ", links.ConvertAll(link => BuildJSON(link.Text, link.URL, -1))) + "]; ", true);
		}

		private void AssignArticleAttachments(List<WebSiteServices.AttachmentData> attachments)
		{
			Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "article_attachments", "var _articleAttachments = [" +
				string.Join(", ", attachments.ConvertAll(attachment => BuildJSON(attachment.Description,
					Core.AttachmentManager.GetRelativePath(attachment.fileName), attachment.ID))) + "]; ", true);
		}

		private void AssignArticleImages(string[] imageNames)
		{
			Image[] imageControls = new Image[] { imgFirstPicture, imgSecondPicture, imgThirdPicture, imgFourthPicture };
			for (int i = 0; i < imageControls.Length; i++)
			{
				string currentImageName = (i < imageNames.Length) ? imageNames[i] : "";
				if (currentImageName.Length > 0)
				{
					string currentImageRelativePath = "~/" + SportSite.Common.Data.ArticlesImagesFolder + "/" + currentImageName;
					if (File.Exists(Server.MapPath(currentImageRelativePath)))
						imageControls[i].ImageUrl = currentImageRelativePath;
				}
			}
		}
		#endregion

		#region Helpers
		protected WebSiteServices.AttachmentData[] ApplyArticleAttachmentsFromRequest(int articleId, string uploadToken, 
			WebSiteServices.AttachmentData[] existingAttachments, SportSite.Core.UserData loggedInUser)
		{
			List<WebSiteServices.AttachmentData> articleAttachments = new List<WebSiteServices.AttachmentData>();
			if (existingAttachments != null)
				articleAttachments.AddRange(existingAttachments);
			List<ArticleAttachmentData> requestAttachments = GetArticleAttachmentsFromRequest(articleId, uploadToken, loggedInUser);
			List<int> attachmentsToDelete = attachmentsToRemove.Value.Split('|').ToList().ConvertAll(rawValue => rawValue.ToIntOrDefault(0)).FindAll(a => a > 0).Distinct().ToList();
			attachmentsToDelete.ForEach(attachmentId => articleAttachments.RemoveAll(attachmentData => attachmentData.ID.Equals(attachmentId)));
			requestAttachments.ForEach(requestAttachment =>
			{
				WebSiteServices.AttachmentData existingAttachmentData = articleAttachments.Find(a => a.ID.Equals(requestAttachment.AttachmentId));
				if (existingAttachmentData != null)
				{
					existingAttachmentData.Description = requestAttachment.Description;
				}
				else
				{
					if (requestAttachment.WebAttachment != null)
					{
						articleAttachments.Add(requestAttachment.WebAttachment);
					}
				}
			});
			return articleAttachments.ToArray();
		}

		protected string[] ApplyArticleImagesFromRequest(int articleId, string uploadToken, string[] existingImages)
		{
			List<string> requestImages = GetArticleImagesFromRequest(articleId, uploadToken);
			List<string> imagesToDelete = imagesToRemove.Value.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).ToList();
			List<string> articleImages = new List<string>();
			if (imagesToDelete.Count > 0 && existingImages != null && existingImages.Length > 0)
			{
				Dictionary<string, bool> deleteMapping = new Dictionary<string, bool>();
				imagesToDelete.ForEach(imgName =>
				{
					string key = imgName.ToLower();
					if (!deleteMapping.ContainsKey(key))
						deleteMapping.Add(key, true);
				});
				for (int i = 0; i < existingImages.Length; i++)
				{
					string existingImage = existingImages[i].ToLower();
					if (deleteMapping.ContainsKey(existingImage))
						existingImages[i] = "";
				}
			}
			
			if (existingImages == null || existingImages.Length == 0)
			{
				articleImages.AddRange(requestImages);
			}
			else
			{
				articleImages.AddRange(existingImages);
				for (int i = 0; i < requestImages.Count; i++)
				{
					string requestImage = requestImages[i];
					if (requestImage.Length > 0)
					{
						if (i >= existingImages.Length)
							articleImages.Add(requestImage);
						else
							articleImages[i] = requestImage;
					}
				}
			}
			return articleImages.FindAll(imgName => imgName.Length > 0).ToArray();
		}

		protected int GetArticleRegionFromRequest()
		{
			return chkRegionalArticle.Checked ? Int32.Parse(Request.Form["ddlArticleRegion"]) : -1;
		}

		protected List<ArticleAttachmentData> GetArticleAttachmentsFromRequest(int articleId, string uploadToken, UserData loggedInUser)
		{
			List<Dictionary<string, string>> allItems = SportSite.Common.Tools.GetRequestCollection("ArticleAttachmentDescription",
				"ArticleAttachmentId", "ArticleAttachmentToken");

			var articleAttachments = allItems.ConvertAll(curItem => new ArticleAttachmentData
			{
				Description = curItem["ArticleAttachmentDescription"],
				AttachmentId = curItem["ArticleAttachmentId"].ToIntOrDefault(0),
				FileToken = curItem["ArticleAttachmentToken"]
			});

			string errorMsg, attachmentFilePath;
			articleAttachments.FindAll(a => a.AttachmentId == 0 && a.FileToken.Length > 0).ForEach(curAttachment =>
			{
				if (UploadManager.TryGetFilePath(uploadToken, curAttachment.FileToken, out errorMsg, out attachmentFilePath))
				{
					if (curAttachment.Description.Length == 0)
						curAttachment.Description = System.IO.Path.GetFileName(attachmentFilePath);
					WebSiteServices.AttachmentData webAttachment = AttachmentManager.AddAttachment(articleId, attachmentFilePath,
						curAttachment.Description, loggedInUser);
					if (webAttachment != null)
					{
						curAttachment.WebAttachment = webAttachment;
						curAttachment.AttachmentId = webAttachment.ID;
					}
				}
			});
			return articleAttachments;
		}

		protected List<string> GetArticleImagesFromRequest(int articleId, string uploadToken)
		{
			string[] imageTokens = hidArticleImages.Value.Split(',');
			string existingImagePath, errorMsg;
			return imageTokens.ToList().ConvertAll(imageToken =>
			{
				if (imageToken.Length > 0 && UploadManager.TryGetFilePath(uploadToken, imageToken, out errorMsg, out existingImagePath))
					return SportSite.Common.Tools.GetArticleImage(articleId, existingImagePath);
				return "";
			});
		}

		protected List<KeyValuePair<string, string>> GetArticleLinksFromRequest()
		{
			List<Dictionary<string, string>> allItems = SportSite.Common.Tools.GetRequestCollection("ArticleLinkUrl", "ArticleLinkDescription");
			List<KeyValuePair<string, string>> links = allItems.ConvertAll(curItem =>
			{
				string url = curItem["ArticleLinkUrl"];
				string description = curItem["ArticleLinkDescription"].GetDefaultValueIfEmpty(url);
				return new KeyValuePair<string, string>(url, description);
			});
			return links.FindAll(l => l.Key.Length > 0);
		}

		private System.Web.UI.HtmlControls.HtmlGenericControl FindRelevantStylesheet(bool isArticleDeleted)
		{
			if (isArticleDeleted)
				return cssArticleDeleted;
			bool edited = (hidArticleId.Value.Length > 0) && (hidArticleId.Value != "new");
			return (edited) ? cssArticleEdited : cssArticleAdded;
		}

		private bool TryGetArticle(string rawArticleId, out WebSiteServices.ArticleData articleData)
		{
			articleData = null;
			int articleId;
			if (Int32.TryParse(rawArticleId, out articleId))
			{
				if (articleId > 0)
				{
					using (WebSiteServices.WebsiteService service = new WebSiteServices.WebsiteService())
					{
						articleData = service.GetArticleData(articleId);
					}
				}
				else
				{
					articleData = new WebSiteServices.ArticleData { ID = -1 };
				}
				return articleData.ID == articleId;
			}
			return false;
		}

		private bool TryGetArticleFromRequest(out WebSiteServices.ArticleData articleData)
		{
			string rawArticleId = Request.QueryString["edit"] + "";
			if (rawArticleId.Equals("new", StringComparison.CurrentCultureIgnoreCase))
				rawArticleId = "-1";
			return TryGetArticle(rawArticleId, out articleData);
		}

		private string BuildJSON(string description, string url, int id)
		{
			List<KeyValuePair<string, string>> pairs = new List<KeyValuePair<string, string>>();
			pairs.Add(new KeyValuePair<string, string>("Description", description));
			pairs.Add(new KeyValuePair<string, string>("Url", url));
			if (id > 0)
				pairs.Add(new KeyValuePair<string, string>("Id", id.ToString()));
			return SportSite.Common.Tools.BuildJsonString(pairs.ToArray());
		}
		#endregion

		public class ArticleAttachmentData
		{
			public string Description { get; set; }
			public int AttachmentId { get; set; }
			public string FileToken { get; set; }
			public WebSiteServices.AttachmentData WebAttachment { get; set; }
		}

		#region not in use
		private List<PagedTableData.TableRow> BuildDummyArticles()
		{
			List<PagedTableData.TableRow> rows = new List<PagedTableData.TableRow>();
			int count = Mir.Common.RandomHelper.GetInt(1500, 7120);
			for (int i = 0; i < count; i++)
			{
				string contents = Mir.Common.RandomHelper.GetLoremIpsum(Mir.Common.RandomHelper.GetInt(2, 7));
				DateTime date = Mir.Common.RandomHelper.GetDateTime(2013, 1, 1, 2016, 5, 1);
				string submittedBy = Mir.Common.RandomHelper.GetString(Mir.Common.RandomHelper.GetInt(3, 10), false, true);
				rows.Add(new PagedTableData.TableRow
				{
					Cells = (new PagedTableData.TableCell[]
						{
							new PagedTableData.TableCell { Contents = contents }, 
							new PagedTableData.TableCell { Contents = date.ToString("dd/MM/yyyy"), Value = date.Ticks }, 
							new PagedTableData.TableCell { Contents = submittedBy }
						}).ToList(),
					TargetUrl = "?edit=" + Mir.Common.RandomHelper.GetInt(1, 2500)
				});
			}
			return rows;
		}
		#endregion
	}
}