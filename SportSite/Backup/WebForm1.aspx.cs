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
	/// Summary description for WebForm1.
	/// </summary>
	public class WebForm1 : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.LinkButton MyButton;
		protected System.Web.UI.HtmlControls.HtmlInputFile File1;
		protected System.Web.UI.WebControls.Image Image1;
		protected System.Web.UI.WebControls.TextBox Text1;
		protected System.Web.UI.WebControls.Label Label2;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			Response.Write("referer: "+Request.UrlReferrer+"<br />");
			
			//System.Drawing.Image image=null;
			string strImageName="test1.gif";
			//image = Common.Tools.BuildTextGif("hello world", "chaimt", Color.Black, Color.Transparent, 25, true, 600, 300, true);
			//image.Save(Server.MapPath(strImageName));
			Image1.ImageUrl = strImageName;
			bool result=(Request.Form.Count > 0);
			foreach (string key in Request.Form.Keys)
			{
				if (key.StartsWith("__"))
					continue;
				string strSessionKey="frm_"+key;
				string strSessionValue="";
				string strFormValue="";
				if (Session[strSessionKey] != null)
					strSessionValue = Session[strSessionKey].ToString();
				if (Request.Form[key] != null)
					strFormValue = Request.Form[key];
				if (strSessionValue != strFormValue)
				{
					result = false;
					break;
				}
			}
			foreach (string key in Request.Form.Keys)
				Session["frm_"+key] = Request.Form[key];
			Label2.Text = result.ToString();

			HtmlTable t1=new HtmlTable();
			HtmlTableRow row=new HtmlTableRow();
			HtmlTableCell cell=new HtmlTableCell();
			cell.InnerText = "aaa";
			row.Cells.Add(cell);
			cell = new HtmlTableCell();
			cell.InnerText = "bbb";
			row.Cells.Add(cell);
			cell = new HtmlTableCell();
			cell.InnerText = "ccc";
			row.Cells.Add(cell);
			t1.Rows.Add(row);
			this.Controls.Add(t1);
			//HtmlTable t2=new HtmlTable();
			//this.Controls.Add(t2);
		}
		
		public void ButtonClick(object sender, System.EventArgs e)
		{
			Response.Write("hello");
			/*
			TextBox MyTextBox;
			if (Page.IsPostBack)
			{
				string strJS="<script type=\"text/javascript\">";
				strJS += "window.onload = WindowLoad; ";
				strJS += "function WindowLoad(event) {";
				strJS += "   document.getElementById(\"" + MyTextBox.UniqueID + "\").focus();";
				strJS += "}";
				strJS += "</script>";
				Page.RegisterClientScriptBlock("onload", strJS);
			}
			*/
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
