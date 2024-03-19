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
	/// Summary description for SendToFriend.
	/// </summary>
	public class SendToFriend : System.Web.UI.Page
	{
		private void Page_Load(object sender, System.EventArgs e)
		{
			//add hitlog data:
			Common.Tools.AddHitLog(WebSiteServices.WebSitePage.SendToFriend, this.Request);
			
			//get data:
			string strName=Common.Tools.CStrDef(Request.Form["name"], "").Trim();
			string strEmail=Common.Tools.CStrDef(Request.Form["email"], "").Trim();
			string strFriendEmail=Common.Tools.CStrDef(Request.Form["friend_email"], "").Trim();
			string strExtraText=Common.Tools.CStrDef(Request.Form["extra_text"], "").Trim();
			string strSendWhat=Common.Tools.CStrDef(Request.Form["SendWhat"], "").Trim();
			string strURL=Common.Tools.CStrDef(Request.Form["URL"], "").Trim();
			string strContents=Common.Tools.CStrDef(Request.Form["Contents"], "").Trim();
			
			//verify:
			VerifyData(strName, "��� �� �����");
			VerifyData(strEmail, "��� ������ �����");
			VerifyData(strFriendEmail, "��� ������ �� ���");
			VerifyData(strSendWhat, "���� ���� �����");
			VerifyData(strURL, "��� �����");
			VerifyData(strContents, "��� ���� �����");
			if ((strSendWhat != "reference")&&(strSendWhat != "html"))
				ShowMessage("���� ����� �����", true);
			
			//build email body:
			System.Text.StringBuilder sb=new System.Text.StringBuilder();
			sb.Append("<div align=\"right\" dir=\"rtl\">");
			sb.Append("����,<br />����� �� ����� ���� �� ��� "+strName+" ("+strEmail+")<br />");
			sb.Append("���� ���� ������� ������ ���� ���� ������ ���� �� ����� "+strName+", ");
			sb.Append("���� "+((strSendWhat == "reference")?"����� �����":"����� ������ �����")+".<br />");
			if (strExtraText.Length > 0)
				sb.Append("��� �� "+strName+" ���� ������: "+strExtraText+"<br />");
			if (strSendWhat == "reference")
			{
				sb.Append("�����:<br /><a href=\""+strURL+"\">"+strURL+"</a><br />");
			}
			else
			{
				System.IO.StreamReader reader=new System.IO.StreamReader(Server.MapPath("SportSite.css"));
				sb.Append("<style type=\"text/css\">");
				sb.Append(reader.ReadToEnd());
				sb.Append("</style>");
				reader.Close();
				sb.Append("<hr />"+strContents);
			}
			sb.Append("</div>");
			
			//subject:
			string strSubject="����� ���� ���� ���� ������� ������ ���� ����";
			
			//send:
			Sport.Common.Tools.sendEmail(strEmail, strFriendEmail, strSubject, sb.ToString());
			
			ShowMessage("����, ������ ���� ������", true);
		}
		
		private void VerifyData(string data, string msg)
		{
			if (data.Length == 0)
				ShowMessage(msg, true);
		}

		private void ShowMessage(string strMessage, bool terminate)
		{
			Response.Write("<script type=\"text/javascript\">");
			Response.Write(" alert(\""+strMessage.Replace("\"", "\\\"")+"\");");
			if (terminate)
				Response.Write(" window.close();");
			Response.Write("</script>");
			if (terminate)
				Response.End();
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
