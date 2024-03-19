<%@ Register TagPrefix="ISF" TagName="LinkBox" Src="LinkBox.ascx" %>
<%@ Register TagPrefix="ISF" TagName="SideNavBar" Src="SideNavBar.ascx" %>
<%@ Register TagPrefix="ISF" TagName="HebDateTime" Src="HebDateTime.ascx" %>
<%@ Register TagPrefix="ISF" TagName="Banner" Src="Banner.ascx" %>
<%@ Register TagPrefix="ISF" TagName="Article" Src="Article.ascx" %>
<%@ Register TagPrefix="ISF" TagName="OnlinePoll" Src="OnlinePoll.ascx" %>
<%@ Register TagPrefix="ISF" TagName="FlashNews" Src="FlashNews.ascx" %>
<%@ Register TagPrefix="ISF" TagName="Sponsors" Src="Sponsors.ascx" %>
<%@ Control Language="c#" AutoEventWireup="false" Codebehind="MainView.ascx.cs" Inherits="SportSite.Controls.MainView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<asp:Literal id="lbStyle" runat="server"></asp:Literal>
<asp:Literal ID="lbOnloadJS" Runat="server"></asp:Literal>
<div align="center" id="MainViewPanel">
	<table id="MainViewTable" border="0" cellpadding="0" cellspacing="0" width="100%">
		<tr>
			<td id="LeftTableCell">&nbsp;</td>
			<td id="CenterTableCell">
				<div id="MainViewBody">
					<div id="DateTimePanel" style="width: 400px; padding-left: 10px;">
						<div style="float: right; width: 100px;"><a href="javascript:history.go(0);" onclick="this.style.behavior='url(#default#homepage)'; this.setHomePage('http://www.schoolsport.co.il');" class="makeHomePageLink">הפוך לעמוד הבית</a></div>
						<div style="float: left; width: 300px;"><ISF:HEBDATETIME id="IsfHebDateTime" dir="rtl" format="%hwd %hd, %d ב%hm %yyyy, <b>%h:%n</b>" Runat="server"></ISF:HEBDATETIME></div>
						<div style="clear: both;"></div>
						<div style="float: left; width: 400px; margin-top: 5px;"><span>התאחדות הספורט לבתיה"ס – ע"ר 580242220</span></div>
						<div style="clear: both;"></div>
					</div>
					<div id="TopLogoPanel">
						<a id="TopLogoLink" runat="server">
							<img id="TopLogoImage" runat="server" width="327" height="96" />
						</a>
					</div>
					<div id="TopBannerPanel">
						<ISF:Banner id="TopBanner" runat="server"></ISF:Banner>
						<div style="float: left;"><a href="http://xads.zedo.com/ads2/c?a=1014245;g=0;c=455000000;i=0;x=3840;n=455;s=0;k=http://www.one.co.il/"><img src="../Images/logo one.png" id="imgLogoOne" runat="server" alt="logo one" title="" visible="false" border="0" /></a></div>
						<div style="clear: both;"></div>
					</div>
					<div id="pnlLoggedInUser" runat="server" style="font-weight: bold; text-align: right; position: absolute; left: 25px; top: 117px; width: 437px; height: 20px;" visible="false"></div>
					<div id="PageCaptionWrapper" class="PageCaptionWrapper" runat="server">
						<div id="PageCaptionWrapper2">
							<div id="PageCaption" class="PageCaption" runat="server"></div>
						</div>
						<div id="PageCaptionExtra">&nbsp;</div>
					</div>
					<div id="NavBarPanel">
						<ISF:LinkBox id="LeftNavBar" style="vertical-align: top;" Runat="server"></ISF:LinkBox>
						<asp:Literal id="ExtraDetails" Runat="server"></asp:Literal>
						<ISF:SideNavBar id="SideNavBar" runat="server"></ISF:SideNavBar>
						<ISF:LinkBox id="OrdersBasket" style="vertical-align: top;" Runat="server"></ISF:LinkBox>
						<div id="SmallBannerPanel">
							<ISF:Banner id="SmallBanner" runat="server"></ISF:Banner>
							<br /><br />
							<div align="center">
								<ASP:Label ID="lbPoweredBy" Runat="server"></ASP:Label>
							</div>
						</div>
					</div>

					<asp:Panel id="AlternativeContentsPanel" Runat="server" ClientIDMode="Static" style="display: none;">
						<asp:Panel ID="AdminDashboardPanel" runat="server" Visible="false" CssClass="DashboardPanel">
							<div class="row">
								<div class="col-md-12">
									<button type="button" class="btn btn-lg btn-primary" data-href="Register.aspx?action=UpdateArticles&id=new">הוספת כתבה חדשה</button>
								</div>
							</div>
							<div class="row">
								<div class="col-md-12">
									<button type="button" class="btn btn-lg btn-primary" onclick="UpdateExistingArticle(this);">עדכון כתבה קיימת</button>
									<div id="ArticleSearchPanel">
										<div class="dismiss">X</div>
										חיפוש לפי כותרת או תוכן: <input type="text" class="form-control" id="txtSearchArticleContent" /><br />
										חיפוש לפי טווח תאריכים:
										<div class="row">
											<div class="col-md-5">
												<div class="input-group date form_date" data-date="" data-date-format="dd/mm/yyyy" data-link-field="dtp_SearchArticleTo" data-link-format="yyyy-mm-dd">
													<input id="SearchArticleTo" class="form-control" type="text" value="" placeholder="dd/MM/yyyy" />
												</div>
												<input type="hidden" id="dtp_SearchArticleTo" value="" />
											</div>
											<div class="col-md-2">
												עד
											</div>
											<div class="col-md-5">
												<div class="input-group date form_date" data-date="" data-date-format="dd/mm/yyyy" data-link-field="dtp_SearchArticleFrom" data-link-format="yyyy-mm-dd">
													<input id="SearchArticleFrom" class="form-control" type="text" value="" placeholder="dd/MM/yyyy" />
												</div>
												<input type="hidden" id="dtp_SearchArticleFrom" value="" />
											</div>
										</div>
									</div>
								</div>
							</div>
						</asp:Panel>
						<asp:Panel ID="SchoolDashboardPanel" runat="server" Visible="false" CssClass="DashboardPanel">
							<button type="button" class="btn btn-lg btn-primary">רישום מועדון</button>
						</asp:Panel>
					</asp:Panel>

					<asp:Panel id="PageContentsPanel" Runat="server" CssClass="PageContentsPanel" 
						ClientIDMode="Static"></asp:Panel>

					<asp:Panel id="ExtraContentsPanel" Runat="server" CssClass="ExtraContentsPanel"></asp:Panel>
					<div id="MainArticlePanel" class="ArticlePanel">
						<ISF:Article id="IsfMainArticle" runat="server"></ISF:Article>
					</div>
					<div id="SubArticlePanel" class="ArticlePanel">
						<ISF:Article id="IsfSubArticle" runat="server"></ISF:Article>
						<ISF:Article id="IsfExtraArticle" runat="server" Visible="False"></ISF:Article>
					</div>
					<div id="MoreArticlesLinkPanel">
						<a id="MoreArticlesLink" runat="server">
							<img id="MoreArticlesImage" width="96" height="18" runat="server" />
						</a>
					</div>
					<div id="MiddleBannerPanel">
						<ISF:Banner id="MiddleBanner" runat="server"></ISF:Banner>
					</div>
					<div id="SportFlowersPanel">
						<a href="https://www.schoolsport.co.il/"><img src="/Images/logo_sport_flowers.png" /></a>
					</div>
					<div id="BottomBannerPanel">
						<ISF:Banner id="BottomBanner" runat="server"></ISF:Banner>
					</div>
					<div id="FooterPanel">
						<ISF:Sponsors id="IsfSponsors" runat="server"></ISF:Sponsors>
						<div id="AllRightsPanel">&copy;&nbsp;כל הזכויות שמורות<br />&nbsp;&nbsp;התאחדות הספורט לבתי הספר בישראל 2016</div>
					</div>
				</div>
			</td>
			<td id="RightTableCell">&nbsp;</td>
		</tr>
	</table>	
</div>
<span id="lbShowError" title="client side script errors occured, click to display" onclick="ToggleVisibility('ErrorPanel');">
*<br />
</span>
<div id="ErrorPanel" style="DISPLAY: none;"></div>