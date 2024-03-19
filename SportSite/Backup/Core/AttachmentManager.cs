using System;
using System.Linq;
using System.Collections;
using System.IO;

using SportSite.Common;
using System.Collections.Generic;

namespace SportSite.Core
{
	public struct AttachmentData
	{
		public int ID;
		public string fileName;
		public long fileSize;
		public string description;
		public DateTime TimeSent;
		public string sender;
		public AttachmentManager.FileType fileType;
		public static AttachmentData Empty;

		static AttachmentData()
		{
			Empty = new AttachmentData();
			Empty.ID = -1;
		}

		public AttachmentData(WebSiteServices.AttachmentData data,
			System.Web.HttpServerUtility Server)
		{
			this.ID = data.ID;
			this.fileName = data.fileName;
			this.fileSize = -1;
			this.description = data.Description;
			this.TimeSent = data.Time;
			this.sender = data.SentBy;
			this.fileType = AttachmentManager.FileType.Unknown;
			if ((data.fileName != null) && (data.fileName.Length > 0))
			{
				string strPath = Server.MapPath(
					AttachmentManager.GetRelativePath(data.fileName));
				if (File.Exists(strPath))
				{
					FileInfo objFile = new FileInfo(strPath);
					this.fileSize = objFile.Length;
					this.fileType = AttachmentManager.GetPathType(data.fileName);
				}
			}
		}
	}

	/// <summary>
	/// Provides collection of static methods to deal with article attachments.
	/// </summary>
	public class AttachmentManager
	{
		public enum FileType
		{
			Unknown = 0,
			Word = 1,
			Excel,
			PDF,
			Picture, 
			PowerPoint
		}

		public static int TypeToInt(FileType type)
		{
			return ((int)type) - 1;
		}



		#region extension management
		public class ExtensionData
		{
			public string[] Extensions { get; set; }
			public string Description { get; set; }
			public string IconPath { get; set; }
			public FileType Type { get; set; }

			public ExtensionData(string description, string icon, FileType fileType, params string[] extensions)
			{
				this.Description = description;
				this.IconPath = Data.AppPath + "/" + Data.AttachmentsFolderName + "/Icons" + "/" + icon;
				this.Type = fileType;
				this.Extensions = new string[extensions.Length];
				for (int i = 0; i < extensions.Length; i++)
					this.Extensions[i] = extensions[i];
			}
		}

		private static readonly ExtensionData[] _allowedExtensions = new ExtensionData[] {
			new ExtensionData("מסמך וורד", "word.jpg", FileType.Word, "doc", "rtf", "docx"), 
			new ExtensionData("גיליון אקסל", "excel.jpg", FileType.Excel, "xls", "xlsx"), 
			new ExtensionData("PDF document", "PDF.gif", FileType.PDF, "pdf"), 
			new ExtensionData("קובץ תמונה", "picture.jpg", FileType.Picture, "jpg", "jpeg", "bmp", "gif", "png", "svg")
		};

		public static List<ExtensionData> AllowedExtensions
		{
			get
			{
				return _allowedExtensions.ToList();
			}
		}

		public static FileType GetPathType(string filePath)
		{
			if (filePath == null || filePath.Length < 2)
				return FileType.Unknown;

			int lastDotIndex = filePath.LastIndexOf(".");
			if (lastDotIndex < 0)
				return FileType.Unknown;

			string strFileExtension = filePath.Substring(lastDotIndex + 1).ToLower();
			ExtensionData matchingExtension = _allowedExtensions.ToList().Find(
				ed => Array.Exists<string>(ed.Extensions, x => x.Equals(strFileExtension, StringComparison.CurrentCultureIgnoreCase)));
			return matchingExtension == null ? FileType.Unknown : matchingExtension.Type;
		}
		#endregion

		public static AttachmentData[] GetArticleAttachments(
			System.Web.HttpServerUtility Server, int articleID)
		{
			WebSiteServices.WebsiteService websiteService =
				new WebSiteServices.WebsiteService();
			WebSiteServices.ArticleData article =
				websiteService.GetArticleData(articleID);
			if (article.ID < 0)
				return null;
			ArrayList attachments = new ArrayList();
			foreach (WebSiteServices.AttachmentData data in article.Attachments)
			{
				if (GetPathType(data.fileName) == FileType.Unknown)
					continue;

				string strPath = Server.MapPath(GetRelativePath(data.fileName));
				if (File.Exists(strPath))
				{
					AttachmentData attachment = new AttachmentData(data, Server);
					//FileInfo objFile=new FileInfo(strPath);
					//attachment.fileSize = objFile.Length;
					//attachment.fileType = GetPathType(data.fileName);
					attachments.Add(attachment);
				}
			}

			return (AttachmentData[])attachments.ToArray(typeof(AttachmentData));
		}

		public static AttachmentData[] GetChampionshipAttachments(
			System.Web.HttpServerUtility Server, int champCategoryID)
		{
			WebSiteServices.WebsiteService websiteService =
				new WebSiteServices.WebsiteService();
			WebSiteServices.AttachmentData[] attachments =
				websiteService.GetChampionshipAttachments(champCategoryID);
			ArrayList result = new ArrayList();
			foreach (WebSiteServices.AttachmentData attachment in attachments)
			{
				if (GetPathType(attachment.fileName) == FileType.Unknown)
					continue;

				string strPath = Server.MapPath(GetRelativePath(attachment.fileName));
				if (File.Exists(strPath))
				{
					AttachmentData attachmentData = new AttachmentData(attachment, Server);
					result.Add(attachmentData);
				}
			}

			return (AttachmentData[])result.ToArray(typeof(AttachmentData));
		}

		/// <summary>
		/// add the given attachment with given description to the dababase.
		/// also, it would save the file in its original name plus article ID.
		/// check if same file already exists.
		/// </summary>
		public static WebSiteServices.AttachmentData AddAttachment(int parentID,
			System.Web.HttpPostedFile objFile, string existingFilePath, string description,
			System.Web.HttpServerUtility Server, Core.UserData user)
		{
			//extract actual file name:
			string exisingFileName = (objFile == null) ? Path.GetFileName(existingFilePath) : objFile.FileName;

			//sanity check
			if (GetPathType(exisingFileName) == FileType.Unknown)
				throw new Exception("Invalid attachment extension");

			//initialize result:
			WebSiteServices.AttachmentData attachmentData = new WebSiteServices.AttachmentData();
			attachmentData.ID = -1;

			//initialize service:
			WebSiteServices.WebsiteService websiteService = new WebSiteServices.WebsiteService();

			//add authorization:
			websiteService.CookieContainer = Sport.Core.Session.Cookies;

			//build actual file name:
			string strFileName = Tools.AddToFilename(exisingFileName, "_" + parentID);

			//build folder name:
			string strFolderName = Data.AppPath + "/" + Data.AttachmentsFolderName;

			//build the full relative path:
			string strRelativePath = strFolderName + "/" + strFileName;

			//get actual path:
			string strFilePath = Server.MapPath(strRelativePath);

			//save to disk or copy exiting file:
			if (objFile == null)
			{
				File.Copy(existingFilePath, strFilePath, true);
			}
			else
			{
				objFile.SaveAs(strFilePath);
			}

			//check if same file already exists:
			DirectoryInfo objFolder = new DirectoryInfo(Server.MapPath(strFolderName));
			FileInfo[] files = objFolder.GetFiles();
			string strExistingFile = "";
			foreach (FileInfo file in files)
			{
				if (file.Name.ToLower() != strFileName.ToLower())
				{
					if (Sport.Common.Tools.FilesEqual(strFilePath, file.FullName))
					{
						strExistingFile = file.Name;
						break;
					}
				}
			}

			if (strExistingFile.Length > 0)
			{
				//found file which is exact copy of the file to upload, look for the attachment having this file:
				int existingAttachmentID = websiteService.FindAttachment(strExistingFile);

				//exists?
				if (existingAttachmentID >= 0)
				{
					File.Delete(strFilePath);
					return websiteService.GetAttachmentData(existingAttachmentID);
				}
			}

			//same file does not exist, apply data:
			attachmentData.Description = description;
			attachmentData.fileName = strFileName;

			//add to database:
			int attachmentID = websiteService.UpdateAttachment(attachmentData, user.Login, user.Password, user.Id);
			return websiteService.GetAttachmentData(attachmentID);
		}

		public static WebSiteServices.AttachmentData AddAttachment(int parentID, System.Web.HttpPostedFile objFile, 
			string description,	System.Web.HttpServerUtility Server, Core.UserData user)
		{
			return AddAttachment(parentID, objFile, null, description, Server, user);
		}

		public static WebSiteServices.AttachmentData AddAttachment(int parentID, string existingFilePath, string description, Core.UserData user)
		{
			return AddAttachment(parentID, null, existingFilePath, description, System.Web.HttpContext.Current.Server, user);
		}

		#region general methods
		public static string GetIcon(FileType type)
		{
			ExtensionData matchingData = _allowedExtensions.ToList().Find(ed => ed.Type == type);
			return (matchingData == null) ? "" : matchingData.IconPath;
		}

		public static string GetRelativePath(string attachmentName)
		{
			string result = Data.AppPath + "/" + Data.AttachmentsFolderName;
			result += "/" + attachmentName;
			return result;
		}

		public static string AttachmentHtml_FullRow(
			AttachmentData attachment, string strColor, int index, bool RTL)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			string strLink = BuildAttachmentLink(attachment.fileName, "");
			string strText = (Tools.CStrDef(attachment.description, "").Length == 0) ?
				attachment.fileName : attachment.description;
			string strJS = "onmouseover=\"PutMoreLight(this, '" + strColor + "', -70);\" ";
			strJS += "onmouseout=\"RestoreColor(this);\"";
			sb.Append("<tr style=\"background-color: " + strColor + ";\">");
			ArrayList arrCells = new ArrayList();
			string strCellHTML = "<td align=\"" + ((RTL) ? "right" : "left") + "\" " +
				"style=\"width: 80%;\" nowrap=\"nowrap\"><ul";
			strCellHTML += "><li";
			strCellHTML += ">" + strLink + strText + " " + Data.LinkSuffix + "</a>";
			strCellHTML += "</li></ul></td>";
			arrCells.Add(strCellHTML);
			strCellHTML = "<td align=\"" + ((RTL) ? "left" : "right") + "\" " +
				"style=\"width: 20%;\" nowrap=\"nowrap\">" +
				"<ul style=\"list-style-type: none;\"><li>";
			strCellHTML += strLink + BuildAttachmentIcon(attachment.fileType);
			strCellHTML += "&nbsp;" + ((RTL) ? "הורד " : "Download ");
			strCellHTML += GetFileTypeDescription(attachment.fileType, RTL) + "</a></li></ul>";
			strCellHTML += "</td>";
			arrCells.Add(strCellHTML);
			//if (!RTL)
			//	arrCells.Reverse();
			foreach (string strCurCellHTML in arrCells)
				sb.Append(strCurCellHTML);
			sb.Append("</tr>");
			return sb.ToString();
		}

		public static string AttachmentHtml_FullRow(
			AttachmentData attachment, string strColor, int index)
		{
			return AttachmentHtml_FullRow(attachment, strColor, index, true);
		}

		public static string GetFileTypeDescription(AttachmentManager.FileType type, string defValue)
		{
			switch (type)
			{
				case FileType.Word:
					return "מסמך וורד";
				case FileType.Excel:
					return "מסמך אקסל";
				case FileType.PDF:
					return "מסמך PDF";
				case FileType.Picture:
					return "קובץ תמונה";
				case FileType.PowerPoint:
					return "מצגת שקופיות";
			}
			return defValue;
		}

		private static string GetFileTypeDescription(AttachmentManager.FileType type,
			bool RTL)
		{
			if (!RTL)
				return "";
			return GetFileTypeDescription(type, "קובץ זה");
		}

		private static string GetFileTypeDescription(AttachmentManager.FileType type)
		{
			return GetFileTypeDescription(type, true);
		}

		public static string BuildClubRegisterAttachmentHtml(string strAttachmentName,
			System.Web.HttpServerUtility Server)
		{
			WebSiteServices.AttachmentData wData = new WebSiteServices.AttachmentData();
			wData.fileName = "ClubRegister/" + strAttachmentName;
			wData.ID = 999;
			wData.Time = DateTime.Now;
			AttachmentData data = new AttachmentData(wData, Server);
			return BuildAttachmentHtml(data);
		}

		public static string BuildAttachmentHtml(AttachmentData data, string ID)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			string strLink = BuildAttachmentLink(data.fileName, ID);
			sb.Append("<table dir=\"rtl\" border=\"0\" ");
			sb.Append("style=\"border: 1px solid black;\"><tr>");
			sb.Append("<td align=\"center\" valign=\"middle\">" + strLink);
			sb.Append(BuildAttachmentIcon(data.fileType) + "</a></td>");
			sb.Append("<td align=\"right\" valign=\"middle\">");
			sb.Append(strLink);
			if ((data.description != null) && (data.description.Length > 0))
			{
				sb.Append(data.description);
			}
			else
			{
				string strName = data.fileName;
				int index = strName.IndexOf("/");
				if (index >= 0)
					strName = strName.Substring(index + 1);
				sb.Append(strName);
			}
			//sb.Append(" ("+data.fileSize+" בייטים)");
			sb.Append("</a></td>");
			sb.Append("");
			sb.Append("</tr></table>");
			return sb.ToString();
		}

		public static string BuildAttachmentHtml(WebSiteServices.AttachmentData data,
			string ID, System.Web.HttpServerUtility Server)
		{
			return BuildAttachmentHtml(new AttachmentData(data, Server), ID);
		}

		public static string BuildAttachmentHtml(AttachmentData data)
		{
			return BuildAttachmentHtml(data, "");
		}

		private static string BuildAttachmentLink(string fileName, string ID)
		{
			string strLink = "<a href=\"" + GetRelativePath(fileName) + "\" ";
			if ((ID != null) && (ID.Length > 0))
				strLink += "id=\"" + ID + "\" ";
			strLink += "title=\"פתח קובץ זה\" target=\"_blank\">";
			return strLink;
		}

		private static string BuildAttachmentIcon(FileType fileType)
		{
			return "<image src=\"" + GetIcon(fileType) + "\" class=\"AttachmentIcon\" border=\"0\" />";
		}
		#endregion
	}
}
