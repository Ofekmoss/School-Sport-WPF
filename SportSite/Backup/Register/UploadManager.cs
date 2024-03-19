using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace SportSite.NewRegister
{
	public static class UploadManager
	{
		public static readonly string UploadFolderName = "Uploads";
		public static readonly string RequestFileKey = "UploadedFile";
		public static readonly string RequestTokenKey = "Token";

		public static string UploadSingleFile()
		{
			Guid uploadToken = Guid.Empty;
			var context = HttpContext.Current;
			HttpPostedFile httpPostedFile = context.Request.Files[RequestFileKey];
			if (httpPostedFile == null || httpPostedFile.ContentLength == 0)
				throw new Exception("Missing or empty file");

			string uploadTokenRaw = HttpContext.Current.Request.Form[RequestTokenKey];
			if (string.IsNullOrEmpty(uploadTokenRaw) || !Guid.TryParseExact(uploadTokenRaw, "N", out uploadToken))
				throw new Exception("Missing or invalid upload token");

			string uploadFolderPath = context.Server.MapPath(UploadFolderName);
			if (!Directory.Exists(uploadFolderPath))
				throw new Exception("Upload folder does not exist");

			uploadFolderPath = Path.Combine(uploadFolderPath, uploadTokenRaw);
			if (!Directory.Exists(uploadFolderPath))
				Directory.CreateDirectory(uploadFolderPath);

			string fileToken = Guid.NewGuid().ToString("N");
			string subFolderPath = Path.Combine(uploadFolderPath, fileToken);
			Directory.CreateDirectory(subFolderPath);
			string fileSavePath = Path.Combine(subFolderPath, httpPostedFile.FileName);
			httpPostedFile.SaveAs(fileSavePath);
			return fileToken;
		}

		public static bool TryGetFilePath(string uploadToken, string fileToken, out string errorMsg, out string filePath)
		{
			filePath = "";
			errorMsg = "";
			string uploadFolderPath = HttpContext.Current.Server.MapPath(UploadFolderName);
			if (!Directory.Exists(uploadFolderPath))
			{
				errorMsg = "Upload folder does not exist";
				return false;
			}

			uploadFolderPath = Path.Combine(uploadFolderPath, uploadToken);
			if (!Directory.Exists(uploadFolderPath))
			{
				errorMsg = "Upload token is invalid or missing";
				return false;
			}

			string subFolderPath = Path.Combine(uploadFolderPath, fileToken);
			if (!Directory.Exists(subFolderPath))
			{
				errorMsg = "File token is invalid or missing";
				return false;
			}

			string[] files = Directory.GetFiles(subFolderPath);
			if (files == null || files.Length == 0)
			{
				errorMsg = "No files found";
				return false;
			}

			filePath = files[0];
			return true;
		}
	}
}