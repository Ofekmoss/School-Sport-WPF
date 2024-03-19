using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Web;

namespace SportSite.Core
{
	public class FileData
	{
		public static readonly string SECURE_FOLDER_PATH = "C:\\wwwnoah\\SecureDownload";

		public string Path { get; set; }
		public string ShortDescription { get; set; }
		public string FullDescription { get; set; }
		public bool PushToClient { get; set; }

		public FileData()
		{
			this.Path = string.Empty;
			this.ShortDescription = string.Empty;
			this.FullDescription = string.Empty;
		}

		public FileData(string fileName, string shortDescription, string fullDescription)
			: this()
		{
			this.Path = SECURE_FOLDER_PATH + System.IO.Path.DirectorySeparatorChar + fileName;
			this.ShortDescription = shortDescription;
			this.FullDescription = fullDescription;
		}
	}

	public struct LinkData
	{
		public string Url;
		public string Text;

		public LinkData(string url, string text)
		{
			Url = url;
			Text = text;
		}
	}

	/// <summary>
	/// Contain static method for dealing with downloads.
	/// </summary>
	public class DownloadManager
	{
		private static readonly double TMP_FOLDER_EXPIRE = 10; //minutes
		public static System.Web.HttpResponse Response = null;
		public static System.Web.SessionState.HttpSessionState Session = null;

		public static string LatestSportsmanInstall
		{
			get
			{
				string relativePath = "Install/sportsman";
				string rootPath = HttpContext.Current.Server.MapPath("~/" + relativePath);
				if (!Directory.Exists(rootPath))
					return "";
				
				List<string> versions = Directory.GetDirectories(rootPath).ToList();
				if (versions.Count == 0)
					return "";

				versions.Sort((v1, v2) =>
				{
					string f1 = Path.GetFileName(v1);
					string f2 = Path.GetFileName(v2);
					double d1, d2;
					if (double.TryParse(f1, out d1) && double.TryParse(f2, out d2) && d1 > 0 && d2 > 0)
						return d2.CompareTo(d1);
					else
						return f2.CompareTo(f1);
				});

				string latestVersionFolderPath = versions[0];
				List<string> files = SportSite.Common.Tools.GetFiles(latestVersionFolderPath, "*.msi", "*.exe").ToList();
				if (files.Count == 0)
					return "";
				return relativePath + "/" + Path.GetFileName(latestVersionFolderPath) + "/" + Path.GetFileName(files[0]);
			}
		}

		/// <summary>
		/// returns list of downloadable files.
		/// </summary>
		public static FileData[] GetFilesList()
		{
			List<FileData> result = new List<FileData>();
			string latestSportsmanInstall = DownloadManager.LatestSportsmanInstall;
			if (latestSportsmanInstall.Length > 0)
			{
				//HttpContext.Current.Server.MapPath("~/" + 
				result.Add(new FileData
				{
					Path =  latestSportsmanInstall,
					ShortDescription = "תוכנת ספורט גרסה " + latestSportsmanInstall.Split('/')[2], 
					FullDescription = "ההורדה אמורה להתחיל בתוך מספר שניות. באם לא מופיע כל דיאלוג נא ללחוץ על הקישור הבא: " + 
						"<a href=\"" + latestSportsmanInstall + "\">הורדה ישירה</a><br /><br />" + 
						"<span style=\"font-weight: bold;\">יש לאשר את דיאלוג האזהרה ולעקוב אחרי הוראות ההתקנה</span>",
					PushToClient = true
				});
			}
			string filePath = HttpContext.Current.Server.MapPath("SecureDownloadFilesList.txt");
			if (File.Exists(filePath))
			{
				string[] lines = File.ReadAllLines(filePath, System.Text.Encoding.GetEncoding("ISO-8859-8"));
				for (int i = 0; i < lines.Length; i += 3)
				{
					if ((i + 2) >= lines.Length)
						break;
					string currentFileName = lines[i];
					string currentFileShortDescription = currentFileShortDescription = lines[i + 1];
					string currentFileLongDescription = lines[i + 2];
					result.Add(new FileData(currentFileName, currentFileShortDescription, currentFileLongDescription));
				}
			}
			return result.ToArray();
		}

		/// <summary>
		/// force download of given file, uses given temp folder.
		/// </summary>
		public static bool DownloadFile(string filePath, string tempFolderPath,
			string tempFolderName)
		{
			//verify we're in secure area:
			if (!filePath.ToLower().StartsWith(FileData.SECURE_FOLDER_PATH.ToLower()))
				throw new Exception("Given file is not under secure area: " + filePath);

			//verify temp folder exists:
			if (!Directory.Exists(tempFolderPath))
			{
				try
				{
					Directory.CreateDirectory(tempFolderPath);
				}
				catch (Exception e)
				{
					throw new Exception("Can't download file: failed to create temp folder: " + e.Message);
				}
			}

			//verify file valid:
			if (!File.Exists(filePath))
				throw new Exception("Can't download file " + filePath + ": file does not exist");

			//verify Response valid:
			if (Response == null)
				throw new Exception("Can't download file: no valid Response.");

			//clean temp folder: (format: [file name]_[user name]_[ticks]
			string[] arrFolderNames = System.IO.Directory.GetDirectories(tempFolderPath);
			int i;
			for (i = 0; i < arrFolderNames.Length; i++)
			{
				//get current folder:
				string folderName = Path.GetFileName(arrFolderNames[i]);

				//split into parts:
				string[] arrTmp = SportSite.Common.Tools.SplitWithEscape(folderName, '_');

				//check if valid folder format:
				if (arrTmp.Length >= 2)
				{
					//get creation ticks:
					long ticks = Sport.Common.Tools.CLngDef(arrTmp[arrTmp.Length - 1], (long)-1);
					if (ticks >= 0)
					{
						System.DateTime folderTime = new System.DateTime(ticks);
						//delete if old enough...
						if (folderTime.AddMinutes(TMP_FOLDER_EXPIRE) < DateTime.Now)
						{
							//Response.Write(folderName+": "+folderTime.ToString()+"<br />");
							try
							{
								//remove directory with all its files:
								Directory.Delete(arrFolderNames[i], true);
							}
							catch (Exception e)
							{
								Response.Write("Warning: failed to remove old temporary " +
									"folder " + arrFolderNames[i] + ": " + e.Message + "<br />");
							}
						}
					} //end if valid ticks value
				} //end if valid format
			} //end loop over the sub folders

			//build temp folder name:
			string strName = Path.GetFileName(filePath) + "_" + DateTime.Now.Ticks.ToString();
			string strPath = tempFolderPath + "\\" + strName;

			//create temp folder:
			Directory.CreateDirectory(strPath);

			//copy the file to temp directory:
			File.Copy(filePath, strPath + "\\" + Path.GetFileName(filePath), true);

			//add user action:
			if (Session != null)
			{
				try
				{
					SessionService.SessionService _service =
						new SessionService.SessionService();
					_service.CookieContainer = (System.Net.CookieContainer)
						Session["cookies"];
					string description = "File: " + filePath;
					_service.AddUserAction(SessionService.Action.Update_Downloaded,
						description, Sport.Core.Data.CurrentVersion);
				}
				catch (Exception ex)
				{
					System.Diagnostics.Debug.WriteLine("failed to add Update Downloaded action: " + ex.Message);
				}
			}

			//redirect user to the new file:
			Response.Redirect(tempFolderName + "/" + strName + "/" + Path.GetFileName(filePath), false);
			return true;
		}

		/// <summary>
		/// download file with given index
		/// </summary>
		public static bool DownloadFile(int fileIndex, string tempFolderPath,
			string tempFolderName)
		{
			FileData[] files = GetFilesList();
			if ((fileIndex < 0) || (fileIndex >= files.Length))
				throw new Exception("Can't download file: illegal index: " + fileIndex.ToString());

			//use static method:
			return DownloadFile(files[fileIndex].Path, tempFolderPath, tempFolderName);
		}

		/*
		public static void DownloadDocument(int docIndex)
		{
			if ((docIndex < 0)||(docIndex >= _documents.Length))
				throw new Exception("Can't download document: invalid index.");
			
			if (Response == null)
				throw new Exception("Can't download document: no Response available.");
			
			string strURL=System.Configuration.ConfigurationSettings.AppSettings["Path"];
			strURL += "/"+DownloadManager.DOCUMENTS_FOLDER_NAME+"/"+_documents[docIndex].Url;
			Response.Redirect(strURL);
		}
		*/
	}
}
