using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using System.Text;

namespace SportSite.Controls
{
	public partial class TabControl : System.Web.UI.UserControl
	{
		protected List<Tab> tabs = new List<Tab>();

		public void AddTab(string strTabCaption, string strAjaxURL, string strDisabledText)
		{
			Tab existingTab = tabs.Find(delegate(Tab t) { return (t.Caption == strTabCaption && t.AjaxUrl == strAjaxURL); });
			if (existingTab == null)
				tabs.Add(new Tab(strTabCaption, strAjaxURL, strDisabledText));
		}

		protected string loadingText = null;
		public string LoadingText
		{
			get { return loadingText; }
			set { loadingText = value; }
		}

		protected string elementsToPost = null;
		public string ElementsToPost
		{
			get { return elementsToPost; }
			set { elementsToPost = value; }
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			SetTabs();
		}

		public void SetTabs()
		{
			MainTabTable.Rows[0].Cells.Clear();
			if (tabs.Count > 0)
			{
				Page.ClientScript.RegisterClientScriptInclude(this.GetType(), "tab_control", "/Common/TabControl.js");
				StringBuilder strJS = new StringBuilder();
				strJS.Append(String.Format("var _contentCellId = \"{0}\"; ", ContentCell.ClientID));
				strJS.Append(String.Format("var _mainTableId = \"{0}\"; ", MainTabTable.ClientID));
				if (!String.IsNullOrEmpty(this.LoadingText))
					strJS.Append(String.Format("var _loadingText = \"{0}\"; ", this.LoadingText));
				if (!String.IsNullOrEmpty(this.ElementsToPost))
					strJS.Append(String.Format("var _elementsToPost = \"{0}\"; ", this.ElementsToPost));
				
				HtmlTableRow header = MainTabTable.Rows[0];
				HtmlTableCell cell;
				int cellWidth = (int)(((100 - (tabs.Count - 1))) / tabs.Count);
				int totalWidth = 0;
				strJS.Append("var arrTabAjax = new Array(); ");
				strJS.Append("var arrDisabledTabText = new Array(); ");
				strJS.Append("var arrTabStatus = new Array(); ");
				for (int i = 0; i < tabs.Count; i++)
				{
					Tab curTab = tabs[i];
					int curWidth = cellWidth;
					bool blnLastRow = (i == (tabs.Count - 1));
					if (blnLastRow)
						curWidth = (100 - totalWidth);
					cell = new HtmlTableCell();
					cell.Attributes["class"] = (i == 0) ? "tab_selected_header" : "tab_header";
					cell.Attributes["onclick"] = String.Format("ChangeTab({0});", i.ToString());
					cell.Style["width"] = curWidth + "%";
					cell.InnerHtml = curTab.Caption;
					header.Cells.Add(cell);
					totalWidth += curWidth;
					if (!blnLastRow)
					{
						cell = new HtmlTableCell();
						cell.Attributes["class"] = "tab_seperator";
						cell.InnerHtml = "&nbsp;";
						header.Cells.Add(cell);
						totalWidth += 1;
					}
					strJS.Append(String.Format("arrTabAjax[arrTabAjax.length] = \"{0}\"; ", curTab.AjaxUrl));
					strJS.Append(String.Format("arrDisabledTabText[arrDisabledTabText.length] = \"{0}\"; ", curTab.DisabledText.Replace("\"", "\\\"")));
					strJS.Append("arrTabStatus[arrTabStatus.length] = true; ");
				}
				
				Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "tab_script", strJS.ToString(), true);
				MainTabTable.Rows[1].Cells[0].ColSpan = (tabs.Count * 2) - 1;
			}
		}

		public class Tab
		{
			private string caption = null;
			public string Caption
			{
				get { return caption; }
				set { caption = value; }
			}

			private string ajaxUrl = null;
			public string AjaxUrl
			{
				get { return ajaxUrl; }
				set { ajaxUrl = value; }
			}

			private string disabledText = null;
			public string DisabledText
			{
				get { return disabledText; }
				set { disabledText = value; }
			}

			public Tab(string strCaption, string strAjaxUrl, string strDisabledText)
			{
				this.Caption = strCaption;
				this.AjaxUrl = strAjaxUrl;
				this.DisabledText = strDisabledText;
			}
		}
	}
}