using System;

namespace SportSite
{
	/// <summary>
	/// Summary description for JHWC.
	/// </summary>
	public class JHWC : System.Web.UI.Page
	{
		protected SportSite.Controls.MainView IsfMainView;
		
		private void Page_Load(object sender, System.EventArgs e)
		{
			//IsfMainView.SideNavBar.Visible = false;
			IsfMainView.IsfSponsors.Visible = false;
			IsfMainView.TopBanner.English = true;
			
			IsfMainView.SideNavBar.Links.Clear();
			IsfMainView.SideNavBar.Links += new SportSite.Controls.LinkBox.Link(
				Common.Data.AppPath+"/Main.aspx?sid="+Session.SessionID, "HEBREW SITE", SportSite.Controls.LinkBox.LinkIndicator.Blue);
			
			IsfMainView.SetPageCaption(" BASKETBALL CHAMPIONSHIP");
			IsfMainView.Attributes["dir"] = "ltr";
			
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			
			sb.Append("<div style=\"margin-left: 5px; margin-right: 5px; "+
				"text-align: left; width: 100%;\" dir=\"ltr\">");

			int index = -1; // Common.Data.DynamicIndex(Common.NavBarLink.JHWC);
			Common.Data.DynamicLinkData linkData = Common.Data.DynamicLinks[index];
			
			//initialize service:
			WebSiteServices.WebsiteService websiteService=
				new WebSiteServices.WebsiteService();
			
			//get page data from database:
			WebSiteServices.PageData page=
				websiteService.FindPageData(linkData.Text, (int) linkData.Link);
			
			//image:
			//sb.Append("<img src=\"Images/JHWC.PNG\" /><br />");
			
			//contents:
			sb.Append(page.Contents);
			
			//attachments:
			if (!Sport.Common.Tools.IsArrayEmpty(page.Attachments))
			{
				IsfMainView.clientSide.RegisterAddLight(this.Page);
				sb.Append("<hr />");
				sb.Append(Common.Tools.BuildAttachmentsTable(
					page.Attachments, this.Server, false));
				sb.Append("<br />");
			}
			
			/*
			sb.Append("<h1>THE FIRST WORLD BASKETBALL CHAMPIONSHIP FOR JEWISH SCHOOLS</h1>");
			sb.Append("<p>The first world basketball championship for Jewish high "+
				"school teams is coordinated by the Israeli Association of School "+
				"Sports, which is in charge of over 20,000 yearly sporting events "+
				"in which more than 500,000 children participate each year.</p>");
			sb.Append("<p>The championship is sponsored by the Israeli Ministries "+
				"of Education, Sports and Culture and Foreign Affairs, the Jewish "+
				"Agency, and the Municipalities of Eilat and Jerusalem.<br />"+
				"The sponsors have organized the Israeli leadership for the event ,"+
				"which is headed by Tal Brodie, the former University of Illinois "+
				"All-American, star of the US Maccabia team, and captain of Maccabi "+
				"Tel Aviv and the Israel national team.<br />"+
				"A parallel international board is currently being established.</p>");
			sb.Append("<p>This December, the first world championship for Jewish "+
				"high school teams will take place in Israel.<br />"+
				"16 teams of boys and 16 teams of girls from Australia, South "+
				"America, Russia, England, France, the United States, Canada, "+
				"Israel and additional countries will participate in the first "+
				"such event ever to take place.</p>");
			sb.Append("<p>The games will take place in Eilat between 04/12/2007 "+
				"and 07/12/2007 and in Jerusalem from Friday 07/12/2007 to "+
				"11/12/2007</p>");
			sb.Append("<p>Teams from throughout the Jewish Diaspora will be made up "+
				"of school organized by local community centers, federations, "+
				"synagogues and other agencies.<br />"+
				"The players must be in high school and 10th, 11th or 12th grade.<br />"+
				"In the first year of competition, teams will qualify on a first "+
				"come basis, depending upon their fulfilling of the basic requirements.<br />"+
				"These teams will be joined by 2-4 Israeli girls and 2-4 Israeli "+
				"boys teams, which have qualified in Israeli high school "+
				"competitions.</p>");
			sb.Append("<p style=\"text-align: center;\">"+
				"<img src=\"Images/JHWC_P1.PNG\" /></p>");
			sb.Append("<p>The tournament will last one week, the first section "+
				"in round robin rounds, then the quarter, semi and final games.<br />"+
				"In addition to the games, the participants will travel together "+
				"throughout Israel, participate in special events both social and "+
				"educational, and will train with Israeli and international "+
				"basketball professionals.</p>");
			sb.Append("<p style=\"text-align: center;\">"+
				"<img src=\"Images/JHWC_P2.PNG\" /></p>");
			sb.Append("<p>The initial championship games will be held in Eilat, "+
				"with the final stages of competition held in the central stadium "+
				"in Jerusalem.</p>");
			sb.Append("<p>This will be the first such championship competition "+
				"which will become an annual international event, involving "+
				"Jewish high school basket ball teams from all over the world.<br />"+
				"A winner's cup received by the champions will be displayed by the "+
				"team during the year of their reign.</p>");
			sb.Append("<p style=\"text-align: center;\">"+
				"<img src=\"Images/JHWC_P3.PNG\" /></p>");
			*/
			
			sb.Append("</div>");
			
			IsfMainView.AddContents(sb.ToString());
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
