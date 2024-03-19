using System;
using System.Linq;
using System.Collections;
using SportSite.Common;
using System.Collections.Generic;

namespace SportSite.Core
{
	/// <summary>
	/// provides static methods to manage links.
	/// </summary>
	public class LinkManager
	{
		public static readonly char GROUP_NAME_SEPERATOR = '|';

		public struct LinkData
		{
			public string URL;
			public string Text;
			public static LinkData Empty;

			static LinkData()
			{
				Empty = new LinkData();
				Empty.URL = "";
				Empty.Text = "";
			}
		}

		public struct LinkGroupData
		{
			public string Caption;
			public LinkData[] Links;
			public static LinkGroupData Empty;

			static LinkGroupData()
			{
				Empty = new LinkGroupData();
				Empty.Caption = "";
				Empty.Links = null;
			}
		}

		public static LinkGroupData[] GetLinkGroups(System.Web.HttpServerUtility Server)
		{
			//initialize ini file reader:
			Sport.Common.IniFile iniFile =
				new Sport.Common.IniFile(Server.MapPath(Data.INI_FILE_NAME));

			//read groups count:
			int groupsCount = Tools.CIntDef(
				iniFile.ReadValue("Links", "GroupCount"), 0);

			//initialize collection:
			ArrayList result = new ArrayList();

			//read groups:
			for (int groupIndex = 0; groupIndex < groupsCount; groupIndex++)
			{
				//get name of the section:
				string strGroupSection = "LinkGroup_" + groupIndex;

				//read current group name:
				string strGroupName = Tools.CStrDef(
					iniFile.ReadValue(strGroupSection, "Name"), "");

				//maybe it's empty?
				if (strGroupName.Length == 0)
					continue;

				//create new group:
				LinkGroupData linkGroup = new LinkGroupData();
				linkGroup.Caption = strGroupName;

				//read links count:
				int linksCount = Tools.CIntDef(
					iniFile.ReadValue(strGroupSection, "LinksCount"), 0);

				//initialize collection:
				ArrayList links = new ArrayList();

				//read links:
				for (int linkIndex = 0; linkIndex < linksCount; linkIndex++)
				{
					//get name of the section:
					string strLinkSection = strGroupSection + "_" + linkIndex;

					//read current link URL and text:
					string strLinkText = Tools.CStrDef(
						iniFile.ReadValue(strLinkSection, "Text"), "");
					string strLinkURL = Tools.CStrDef(
						iniFile.ReadValue(strLinkSection, "URL"), "");

					//maybe it's empty?
					if ((strLinkText.Length == 0) || (strLinkURL.Length == 0))
						continue;

					//create new link data:
					LinkData link = new LinkData();

					//assign data:
					link.URL = strLinkURL;
					link.Text = strLinkText;

					//add to collection:
					links.Add(link);
				} //end loop over the links

				//apply links and add to result collection:
				linkGroup.Links = (LinkData[])links.ToArray(typeof(LinkData));
				result.Add(linkGroup);
			} //end loop over the groups

			//done, return array:
			return (LinkGroupData[])result.ToArray(typeof(LinkGroupData));
		} //end function GetLinkGroups

		public static void UpdateLinkGroups(LinkGroupData[] groups,
			System.Web.HttpServerUtility Server)
		{
			//initialize ini file writer:
			Sport.Common.IniFile iniFile =
				new Sport.Common.IniFile(Server.MapPath(Data.INI_FILE_NAME));

			//write groups count:
			iniFile.WriteValue("Links", "GroupCount", groups.Length.ToString());

			//iterate over groups:
			for (int groupIndex = 0; groupIndex < groups.Length; groupIndex++)
			{
				//get current group:
				LinkGroupData group = groups[groupIndex];

				//get name of the section:
				string strGroupSection = "LinkGroup_" + groupIndex;

				//write current group name:
				iniFile.WriteValue(strGroupSection, "Name", group.Caption);

				//write links count:
				iniFile.WriteValue(strGroupSection, "LinksCount",
					(group.Links == null) ? "0" : group.Links.Length.ToString());

				//iterate over links:
				if (group.Links != null)
				{
					for (int linkIndex = 0; linkIndex < group.Links.Length; linkIndex++)
					{
						//get current link:
						LinkData link = group.Links[linkIndex];

						//get name of the section:
						string strLinkSection = strGroupSection + "_" + linkIndex;

						//write current link URL and text:
						iniFile.WriteValue(strLinkSection, "Text", link.Text);
						iniFile.WriteValue(strLinkSection, "URL", link.URL);
					} //end loop over the links
				}
			} //end loop over the groups
		} //end function UpdateLinkGroups

		public static bool SaveLinkLogo(string linkURL, System.Web.HttpPostedFile objFile,
			System.Web.HttpServerUtility Server, string groupIndex, string linkIndex,
			System.Web.UI.Page Page, string linkText)
		{
			string strFolderName = Common.Data.AppPath + "/Images/Logos/Links";
			string strFolderPath = Server.MapPath(strFolderName);
			if (!System.IO.Directory.Exists(strFolderPath))
				System.IO.Directory.CreateDirectory(strFolderPath);
			strFolderName += "/" + Tools.MakeValidFileName(linkURL);
			strFolderPath = Server.MapPath(strFolderName);
			if (!System.IO.Directory.Exists(strFolderPath))
				System.IO.Directory.CreateDirectory(strFolderPath);
			string strImageName = strFolderName + "/" + System.IO.Path.GetFileName(objFile.FileName);
			string strImagePath = Server.MapPath(strImageName);
			objFile.SaveAs(strImagePath);
			System.Drawing.Bitmap bitmap = null;
			try
			{
				bitmap = new System.Drawing.Bitmap(strImagePath);
			}
			catch
			{
				bitmap = null;
			}
			if ((bitmap == null) || (bitmap.Width == 0))
			{
				Page.ClientScript.RegisterClientScriptBlock(Page.GetType(), "link_logo_error_" + groupIndex + "_" + linkIndex,
					"<script type=\"text/javascript\">alert(\"קובץ תמונה שגוי עבור הקישור '" + linkText + "'\");</script>", false);
				System.IO.File.Delete(strImagePath);
				return false;
			}
			string[] arrFileNames = System.IO.Directory.GetFiles(strFolderPath);
			foreach (string strFilePath in arrFileNames)
			{
				if (strFilePath != strImagePath)
					System.IO.File.Delete(strFilePath);
			}
			return true;
		}

		public static string GetLinkLogoHtml(string strLinkURL,
			System.Web.HttpServerUtility Server, string strDefaultHtml)
		{
			if ((strLinkURL == null) || (strLinkURL.Length == 0))
				return strDefaultHtml;
			string strFolderName = Data.AppPath + "/Images/Logos/Links/" +
				Tools.MakeValidFileName(strLinkURL);
			string strFolderPath = Server.MapPath(strFolderName);
			if (!System.IO.Directory.Exists(strFolderPath))
				return strDefaultHtml;
			string[] arrFileNames = System.IO.Directory.GetFiles(strFolderPath);
			if ((arrFileNames == null) || (arrFileNames.Length == 0))
				return strDefaultHtml;
			string strImageName = strFolderName + "/" + System.IO.Path.GetFileName(arrFileNames[0]);
			strImageName = Tools.CheckAndCreateThumbnail(strImageName, 104, 68, Server);
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append("<div class=\"LinkLogoContainer\">");
			sb.Append("<a href=\"" + strLinkURL + "\"><img class=\"LinkLogoImage\" " +
				"src=\"" + strImageName + "\" /></a>");
			sb.Append("</div>");
			return sb.ToString();
		}
		#region Permanent Championships
		public static LinkData[] GetPermanentChampionships()
		{
			List<LinkData> links = new List<LinkData>();
			string baseURL = Data.AppPath + "/Results.aspx?category=$cat";
			using (DataServices1.DataService service = new DataServices1.DataService())
			{
				links.AddRange(service.GetWebsitePermanentChampionships().ToList().ConvertAll(p =>
					new LinkData
					{
						Text = p.Title,
						URL = baseURL.Replace("$cat", p.ChampionshipCategoryId.ToString())
					}));
			}
			return links.ToArray();
		}
		#endregion
	} //end class LinkManager
}
