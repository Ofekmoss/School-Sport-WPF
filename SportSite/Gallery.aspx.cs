using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using SportSite.Common;
using System.IO;

namespace SportSite
{
	/// <summary>
	/// Summary description for Gallery.
	/// </summary>
	public class Gallery : System.Web.UI.Page
	{
		protected SportSite.Controls.MainView IsfMainView;
		
		private void Page_Load(object sender, System.EventArgs e)
		{
			//add hitlog data:
			Common.Tools.AddHitLog(WebSiteServices.WebSitePage.Gallery, this.Request);
			
			//set active link:
			IsfMainView.SideNavBar.Links.MakeLinkActive(NavBarLink.PictureGallery);
			
			//initialize group and sub group
			string strGroup="";
			string strSubGroup="";
			
			//get image ID from querystring:
			int imageID=Tools.CIntDef(Request.QueryString["i"], -1);

			//got anything?
			if (imageID >= 0)
			{
				//initialize service:
				WebSiteServices.WebsiteService websiteService=
					new WebSiteServices.WebsiteService();
				
				//get image from database:
				WebSiteServices.ImageData image=websiteService.GetGalleryImage(imageID);
				
				//got anything?
				if ((image != null)&&(image.ID >= 0))
				{
					strGroup = image.GroupName;
					strSubGroup = image.SubGroup;
				}
			}
			
			//caption.
			string strCaption=(strGroup.Length == 0)?"גלריות":strGroup;
			IsfMainView.SetPageCaption(strCaption);
			
			//need to display single image?
			CheckDisplayImage();
			
			//show the gallery.
			ShowGallery(strGroup, strSubGroup);
		}
		
		private WebSiteServices.ImageData[] GetGroupsSorted(
			WebSiteServices.WebsiteService websiteService)
		{
			ArrayList arrAllData=new ArrayList(websiteService.GetAllGalleryImages());
			ArrayList result=new ArrayList();
			string strOrder=Common.Data.GetGalleryGroupsOrder(this.Server);
			if ((strOrder == null)||(strOrder.Length == 0))
				return (WebSiteServices.ImageData[]) arrAllData.ToArray(typeof(WebSiteServices.ImageData));
			string[] arrOrder=Sport.Common.Tools.SplitNoBlank(strOrder, '|');
			int[] arrDataGroups=new int[arrAllData.Count];
			int curGroupIndex=-1;
			string strLastGroup=null;
			for (int i=0; i<arrAllData.Count; i++)
			{
				WebSiteServices.ImageData curImage=(WebSiteServices.ImageData) 
					arrAllData[i];
				string strCurGroup=curImage.GroupName;
				if ((strLastGroup == null)||(strCurGroup != strLastGroup))
					curGroupIndex++;
				arrDataGroups[i] = curGroupIndex;
				strLastGroup = strCurGroup;
			}
			string[] arrIndices=Tools.FixNumericArray(arrOrder, curGroupIndex+1);
			for (int i=0; i<arrIndices.Length; i++)
			{
				int index=Tools.CIntDef(arrIndices[i], -1);
				if ((index < 0)||(index >= arrDataGroups.Length))
					continue;
				for (int j=0; j<arrDataGroups.Length; j++)
				{
					if (arrDataGroups[j] == index)
						result.Add(arrAllData[j]);
				}
			}
			return (WebSiteServices.ImageData[]) result.ToArray(typeof(WebSiteServices.ImageData));
		}
		
		private void ShowGallery(string strGroupName, string strSubGroupName)
		{
			//show group?
			bool blnGroup=(strGroupName.Length > 0);
			
			//initialize service:
			WebSiteServices.WebsiteService websiteService=
				new WebSiteServices.WebsiteService();
			
			//get images:
			WebSiteServices.ImageData[] images=null;
			if (blnGroup)
				images = websiteService.GetImagesByGroup(strGroupName, strSubGroupName);
			else
				images = GetGroupsSorted(websiteService);
			
			//got anything?
			if ((images == null)||(images.Length == 0))
				return;
			
			//initlaize html string:
			System.Text.StringBuilder sb=new System.Text.StringBuilder();
			sb.Append("<div dir=\"rtl\" align=\"right\">");
			
			//sub caption?
			if (strSubGroupName.Length > 0)
			{
				sb.Append(Tools.BuildPageSubCaption(strSubGroupName, 
					this.Server, IsfMainView.clientSide));
			}
			
			//constants:
			int thumbWidth=158;
			
			//calculate groups per row and table width
			int cellsPerRow=(int) (((double) 500)/((double) thumbWidth));
			int iTableWidth=(cellsPerRow*thumbWidth);
			
			//table spacing:
			int spacing=(blnGroup)?9:6;
			
			string strBaseCell="<td style=\"width: "+thumbWidth+"px;\">%text</td>";
			sb.Append("<center>");
			sb.Append("<table cellspacing=\""+spacing+"\" cellpadding=\""+spacing+"\" "+
				"style=\"width: "+iTableWidth+"px;\">");
			int picIndex=0;
			int rowCells=0;
			string strLastGroup=null;
			string strLastSubGroup=null;
			while (picIndex<images.Length)
			{
				WebSiteServices.ImageData curImage=images[picIndex];
				string strCurrentGroup=curImage.GroupName;
				string strCurrentSubGroup=curImage.SubGroup;
				if ((blnGroup)||(strCurrentGroup != strLastGroup)||
					(strCurrentSubGroup != strLastSubGroup))
				{
					if ((rowCells % cellsPerRow) == 0)
					{
						if (picIndex > 0)
							sb.Append("</tr>");
						sb.Append("<tr>");
						rowCells = 0;
					}
					string strCellHTML="";
					if ((blnGroup == false)&&(strCurrentGroup != strLastGroup))
					{
						sb.Append("</tr><td colspan=\""+cellsPerRow+"\">");
						sb.Append(Tools.BuildPageSubCaption(strCurrentGroup, this.Server, IsfMainView.clientSide));
						sb.Append("</td></tr><tr>");
						rowCells = 0;
					}
					if (blnGroup)
					{
						strCellHTML += Tools.BuildThumbnailHTML(curImage, this.Server);
					}
					else
					{
						strCellHTML += Tools.BuildThumbnailHTML(curImage, this.Server, 
							"Gallery.aspx?i="+curImage.ID, strCurrentSubGroup, 
							null, "צפה בגלרייה זו", false);
					}
					sb.Append(strBaseCell.Replace("%text", strCellHTML));
					rowCells++;
				} //end if need to show current image
				strLastGroup = strCurrentGroup;
				strLastSubGroup = strCurrentSubGroup;
				picIndex++;
			} //end loop over images.
			
			if ((rowCells > 0)&&((rowCells % cellsPerRow) != 0))
			{
				for (int i=rowCells; i<cellsPerRow; i++)
					sb.Append(strBaseCell.Replace("%text", "&nbsp;"));
				sb.Append("</tr>");
			}
			sb.Append("</table>");
			sb.Append("</center>");
			
			//done.
			sb.Append("</div>");
			AddHtml(sb.ToString());
		} //end function ShowGallery
		
		private void CheckDisplayImage()
		{
			//need to display anything?
			if (Request.QueryString["action"] != "view")
				return;
			
			//get image ID from querystring:
			int imageID=Tools.CIntDef(Request.QueryString["id"], -1);
			
			//valid?
			if (imageID < 0)
			{
				AddErrorMessage("זיהוי תמונה שגוי");
				return;
			}
			
			//initialize service:
			WebSiteServices.WebsiteService websiteService=
				new WebSiteServices.WebsiteService();
			
			//get image from database:
			WebSiteServices.ImageData image=websiteService.GetGalleryImage(imageID);
			
			//got anything?
			if ((image == null)||(image.ID < 0))
			{
				AddErrorMessage("תמונה שגויה");
				return;
			}
			
			//use the original name so that when user save the picture, it will
			//be saved under the meaningful name rather than internal name.
			//build path for copying the file.
			string strOriginalName=Data.ImageGalleryFolder+"/temp/"+image.OriginalName;
			string strPictureName=Data.ImageGalleryFolder+"/"+image.PictureName;
			string strPicturePath=Server.MapPath(strPictureName);
			string strTempPath=Server.MapPath(strOriginalName);
			bool blnCopySuccess=true;
			Gallery.RemoveTempFiles(this.Server);
			try
			{
				System.IO.File.Copy(strPicturePath, strTempPath, true);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("failed to copy original to temp: "+ex.Message);
				blnCopySuccess = false;
			}
			
			websiteService.AddImageView(imageID);
			string strURL=(blnCopySuccess)?strOriginalName:strPictureName;
			this.Controls.Clear();
			Response.Write("<html>");
			Response.Write("<head>");
			Response.Write("<title>"+image.Description+"</title>");
			Response.Write("</head>");
			Response.Write("<body>");
			Response.Write("<img src=\""+strURL+"\" border=\"0\" />");
			Response.Write("</body>");
			Response.Write("</html>");
			Response.Flush();
			Response.End();
		} //end function CheckDisplayImage
		
		private static void RemoveTempFiles(HttpServerUtility Server)
		{
			string strFolderName=Data.AppPath+"/"+Data.ImageGalleryFolder+"/temp";
			string strFolderPath=Server.MapPath(strFolderName);
			if (Directory.Exists(strFolderPath))
			{
				string[] arrFileNames=Directory.GetFiles(strFolderPath);
				foreach (string strFilePath in arrFileNames)
				{
					FileInfo info=new FileInfo(strFilePath);
					if ((DateTime.Now-info.LastAccessTime).TotalMinutes > 10)
						File.Delete(strFilePath);
				}
			}
		}
		
		private void AddHtml(string html)
		{
			IsfMainView.AddContents(html);
		}
		
		private void AddErrorMessage(string message)
		{
			AddHtml("<span style=\"color: red; font-weight: bold;\">"+message+"</span><br />");
		}
		
		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//ClientSide.Page = this.Page;
			
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.Load += new System.EventHandler(this.Page_Load);
		}
		#endregion
	}
}
