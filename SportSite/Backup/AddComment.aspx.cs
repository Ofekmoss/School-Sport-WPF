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

namespace SportSite
{
	/// <summary>
	/// Summary description for AddComment.
	/// </summary>
	public class AddComment : System.Web.UI.Page
	{
		private void Page_Load(object sender, System.EventArgs e)
		{
			//add hitlog data:
			Common.Tools.AddHitLog(WebSiteServices.WebSitePage.AddComment, this.Request);
			
			//get data:
			int articleID = Common.Tools.CIntDef(Request.Form["article"], -1);
			bool blnFAQ = (Request.Form["FAQ"] == "1");
			string strName = Common.Tools.CStrDef(Request.Form["name"], "").Trim();
			string strEmail = Common.Tools.CStrDef(Request.Form["email"], "").Trim();
			string strSubject = Common.Tools.CStrDef(Request.Form["subject"], "").Trim();
			string strContents = Common.Tools.CStrDef(Request.Form["contents"], "").Trim();
			
			//FAQ question?
			if (blnFAQ)
			{
				//verify contents:
				VerifyData(strContents, "חסר תוכן השאלה");
				
				//get question:
				string strQuestion = strContents;
				
				//try to add question:
				string strError = Core.FAQ.AddQuestion(strQuestion, strName);
				
				//success?
				if (strError.Length > 0)
					ShowMessage("לא ניתן להוסיף שאלה: "+strError, true);
				
				//show message:
				ShowMessage("תודה, שאלה נוספה למאגר הנתונים ותופיע בהקדם בעמוד זה", 
					true, false);
				
				//done.
				return;
			}
			
			//verify valid ID:
			if (articleID < 0)
				ShowMessage("זיהוי כתבה שגוי", true);
			
			//verify given data:
			VerifyData(strName, "חסר שם השולח");
			VerifyData(strSubject, "חסר נושא התגובה");
			
			//initialize web service:
			WebSiteServices.WebsiteService service = 
				new WebSiteServices.WebsiteService();
			
			//build comment data:
			WebSiteServices.ArticleCommentData data = 
				new WebSiteServices.ArticleCommentData();
			data.ID = -1;
			data.Article = articleID;
			data.Caption = strSubject;
			data.Contents = strContents;
			data.VisitorName = strName;
			data.VisitorEmail = strEmail;
			data.VisitorIP = 
				Common.Tools.CStrDef(Request.ServerVariables["Remote_Host"], "");
			
			//update the database:
			service.UpdateArticleComment(data, "", "");

			//clear cache:
			CacheStore.Instance.Remove("ArticleData_" + articleID);
			CacheStore.Instance.Remove("ArticleComments_" + articleID);
			
			//done.
			ShowMessage("תודה, תגובתך נוספה בהצלחה", true, true);
		}
		
		private void VerifyData(string data, string msg)
		{
			if (data.Length == 0)
				ShowMessage(msg, true);
		}
		
		private void ShowMessage(string strMessage, bool terminate, bool reload)
		{
			Response.Write("<script type=\"text/javascript\">");
			Response.Write(" alert(\""+strMessage.Replace("\"", "\\\"")+"\");");
			Response.Write("</script>");
			if (reload)
			{
				Response.Write("<script type=\"text/javascript\">");
				Response.Write(" try {window.opener.location.reload();} catch (e) {}");
				Response.Write("</script>");
			}
			if (terminate)
			{
				Response.Write("<script type=\"text/javascript\">");
				Response.Write(" window.close();");
				Response.Write("</script>");
			}
			if (terminate)
				Response.End();
		}
		
		private void ShowMessage(string strMessage, bool terminate)
		{
			ShowMessage(strMessage, terminate, false);
		}
		
		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
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
