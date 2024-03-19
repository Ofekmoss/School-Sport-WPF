using System;
using System.IO;
using System.Collections;

namespace SportSite.Core
{
	public class SponsorData
	{
		public string ImageFileName="";
		public string URL="";
	}
	
	public class SponsorManager
	{
		public static string FolderName="";
		private static System.Web.HttpServerUtility Server=null;

		static SponsorManager()
		{
			FolderName = Common.Data.AppPath+"/Images/Sponsors";
		}

		public static void SetServer(System.Web.HttpServerUtility server)
		{
			Server = server;
		}

		public static SponsorData[] GetSponsors()
		{
			if (Server == null)
				throw new Exception("Get Sponsors: no Server defined.");
			ArrayList arrValidExtensions=new ArrayList(new string[] {".bmp", ".jpg", 
											".jpeg", ".gif", ".png"});
			string strFolderPath=Server.MapPath(FolderName);
			string[] arrFilesPath=Directory.GetFiles(strFolderPath);
			ArrayList result=new ArrayList();
			foreach (string strFilePath in arrFilesPath)
			{
				string strCurExtension=Path.GetExtension(strFilePath).ToLower();
				if (arrValidExtensions.IndexOf(strCurExtension) >= 0)
				{
					SponsorData curData=new SponsorData();
					curData.ImageFileName = FolderName+"/"+Path.GetFileName(strFilePath);
					string strLinkFilePath=Server.MapPath(FolderName+"/"+Path.GetFileNameWithoutExtension(strFilePath)+".txt");
					string strURL="";
					if (File.Exists(strLinkFilePath))
					{
						string[] arrLines = Sport.Common.Tools.ReadTextFile(strLinkFilePath, false);
						if (arrLines != null && arrLines.Length > 0)
							strURL = arrLines[0];
					}
					curData.URL = strURL;
					result.Add(curData);
				}
			}
			return (SponsorData[]) result.ToArray(typeof(SponsorData));
		}
		
		public static void UpdateSponsor(SponsorData sponsor, int index)
		{
			if (Server == null)
				throw new Exception("Update Sponsor: no Server defined.");
			string strLinkFilePath=Server.MapPath(FolderName+"/"+Path.GetFileNameWithoutExtension(Server.MapPath(sponsor.ImageFileName))+".txt");
			StreamWriter objWriter=File.CreateText(strLinkFilePath);
			objWriter.Write(sponsor.URL);
			objWriter.Close();
			//File.Move(Server.MapPath(sponsor.ImageFileName), 
			//	Server.MapPath(FolderName+"/"+(index+1)+"_"+
			//	Path.GetFileName(Server.MapPath(sponsor.ImageFileName))));
		}
	}
}
