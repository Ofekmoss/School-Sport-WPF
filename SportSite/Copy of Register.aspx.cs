using System;
using System.IO;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using SportSite.Controls;
using SportSite.Common;
using SportSite.Core;
using Sport.Entities;
using Sport.Data;
using System.Globalization;
//using System.Reflection.Emit

namespace SportSite
{
	/// <summary>
	/// Summary description for Register.
	/// </summary>
	public class Register : System.Web.UI.Page
	{
		private bool _loginExecuted=false;
		
		#region Private Members
		private Core.UserManager userManager;
		private UserData _user=UserData.Empty;
		private System.Collections.Specialized.NameValueCollection SportData=new System.Collections.Specialized.NameValueCollection();
		private readonly string PlayerToAddString="PlayersToAdd";
		private readonly string SelectedStudentString="SelectedStudent";
		private readonly string SelectedSportString="SelectedSport";
		private PlayerCardService.PlayerCardService _playerCardService=null;
		protected SportSite.Controls.MainView IsfMainView;
		protected System.Web.UI.WebControls.Literal lbStyle;
		protected System.Web.UI.WebControls.Label errorsLabel;
		protected System.Web.UI.WebControls.Label successLabel;
		protected System.Web.UI.HtmlControls.HtmlInputHidden userSchoolID;
		protected SportSite.Controls.HebDateTime IsfHebDateTime;
		protected System.Web.UI.WebControls.Table IsfMainBody;
		protected SportSite.Controls.SideNavBar SideNavBar;
		protected SportSite.Controls.LinkBox LeftNavBar;
		protected SportSite.Controls.LinkBox OrdersBasket;
		protected System.Web.UI.HtmlControls.HtmlInputHidden TeamID;
		protected System.Web.UI.HtmlControls.HtmlInputHidden MessageID;
		protected System.Web.UI.HtmlControls.HtmlInputFile ArticleImage;
		private string _pageCaption="כלי ניהול";
		private LinkBox.Link[] _basketLinks=new LinkBox.Link[3+1];
		
		// Service
		WebSiteServices.WebsiteService websiteService;

		// Create Poll and Answer Pages
		protected Label newPollQuestionLabel,newAnswerQuestionLabel;
		protected Label newPollDateLabel;
		protected TextBox newPollExpirationTime;
		protected TextBox newPollTextbox,newAnswerTextbox;
		protected Button newPollOkButton,newAnswerOkButton;
		protected int newPollId,newAnswerPollId;

		#endregion

		#region Dynamically Created Controls
		private SportSite.Controls.TableView TeamsTableView=null;
		private SportSite.Controls.TableView PlayersTableView=null;
		private SportSite.Controls.TableView MessageTableView=null;
		private System.Web.UI.WebControls.Panel PnlUserLogin=null;
		private System.Web.UI.WebControls.ImageButton btnAddTeam=null;
		private System.Web.UI.HtmlControls.HtmlInputText edUsername=null;
		protected System.Web.UI.HtmlControls.HtmlInputHidden pending_players_team;
		private System.Web.UI.WebControls.TextBox edUserPassword=null;
		#endregion

		#region Initialization
		public void Dummy()
		{
			Response.Write("dummy function called<br />");
		}
		
		private void Page_Load(object sender, System.EventArgs e)
		{
			/*
			string s=Server.MapPath("testfile.txt");
			Tools.CreateTextFile(s, "hello world\r\nhow are you?\r\nשלום עולם", true);
			string[] lines=Tools.ReadTextFile(s, true);
			if (lines.Length > 2)
				Response.Write(lines[2]+"<br />");
			*/
			
			//Response.Write("path: "+Server.MapPath("/ISF/SportSite/Images/Sponsors")+"<br />");
			//Response.End();
			
			//show instant message?
			CheckInstantMessage();
			
			//check AJAX:
			CheckAJAX();
			
			//check display players register form:
			CheckPlayersRegisterForm();
			
			//clear previous data:
			IsfMainView.ClearContents();

			//Initialize Web Controls
			newPollQuestionLabel = new Label();
			newPollDateLabel = new Label();
			newPollExpirationTime = new TextBox();
			newPollTextbox = new TextBox();
			newPollOkButton = new Button();
			newAnswerQuestionLabel = new Label();
			newAnswerTextbox = new TextBox();
			newAnswerOkButton = new Button();
			newPollId = -1;
			newAnswerPollId = -1;
			
			//add hitlog data:
			Common.Tools.AddHitLog(WebSiteServices.WebSitePage.Register, this.Request);
			
			//client side:
			IsfMainView.clientSide.RegisterAddLight(this.Page);
			
			//maybe need to preview date...
			CheckDatePreview();

			//maybe only need to delete image?
			CheckDeleteImage();
			
			//maybe only need to delete attachment?
			CheckDeleteAttachment();
			
			errorsLabel = new Label();
			successLabel = new Label();
			errorsLabel.Text = "";
			successLabel.Text = "";
			IsfMainView.AddContents(errorsLabel);
			IsfMainView.AddContents(successLabel);
			
			//Dummy();
			//System.Reflection.MethodInfo info=this.GetType().GetMethod("Dummy");
			//info.Invoke(this, null);
			
			AddStyle();
			CreateAdminControls();
			VerifyUser(null, null);
			Sport.Core.Session.Cookies = (System.Net.CookieContainer) Session["cookies"];
			SetSportData();
			UserLogin();
			IsfMainView.clientSide.AddOnloadCommand("IterateTables();", true);
			
			//maybe only need to delete link?
			CheckDeleteLink();
			
			//reset entities?
			if ((_user.Login != null)&&
				(Request.QueryString["action"] == SportSiteAction.ReloadEntities.ToString()))
			{
				ReloadEntities();
				SetSportData();
			}

			SetSideMenu();
			
			//DebugTools.PrintRequestCollections(this.Page);

			//check querystring
			if (Request.QueryString["action"] != null)
			{
				string strAction = Request.QueryString["action"].ToString();
				if (strAction == SportSiteAction.Logout.ToString())
					UserLogout();
			}
			
			//maybe user selected championship?
			if ((_user.Login != null)&&(SportData["category"] != null))
			{
				DisplayCategoryTeams(Int32.Parse(SportData["category"]));
			}
			
			//check form action
			if (_user.Login != null)
			{
				//get instant message:
				Sport.Entities.InstantMessage objMessage=
					Tools.GetPendingMessage(_user.Id);

				//got anything?
				if (objMessage != null)
				{
					//show message:
					IsfMainView.clientSide.AddOnloadCommand(
						"OpenDialog('Register.aspx?action="+
						Common.SportSiteAction.ShowInstantMessage+
						"&id="+objMessage.Id+"&nnn="+
						(new Random()).Next(1000, 999999)+"', 500, 300)", false, true);
					
					//mark as read:
					DataServices.DataService dataService=
						new DataServices.DataService();
					dataService.MarkMessageRead(_user.Login, _user.Password, objMessage.Id);
				}
				
				//reset sub articles?
				CheckResetSubArticles();
				
				//decide what method to execute:
				DecideAction();
			}
			
			//display orders basket:
			SetOrdersBasket();

			//add javascript:
			string strCode="<script type=\"text/javascript\" src=\""+Common.Data.AppPath+"/Common/ToolTip_V1.js\"></script>\n";
			this.Page.RegisterClientScriptBlock("ToolTip_V1", strCode);
			strCode="<script type=\"text/javascript\" src=\"Register.js\"></script>\n";
			this.Page.RegisterClientScriptBlock("Register_Js", strCode);
		}
		
		public void Page_Unload(object sender, System.EventArgs e)
		{
			Sport.Core.Session.Cookies = null;
		}

		public void Page_PreRender(object sender, System.EventArgs e)
		{
			if ((MessageTableView != null)&&(MessageTableView.RowsCount == 0))
				IsfMainView.clientSide.AddOnloadCommand("document.getElementById(\"MessageClickTip\").style.display = \"none\";", true);
			
			if (errorsLabel.Text.Length > 0)
				errorsLabel.Visible = true;
			
			PnlUserLogin.Visible = _user.Equals(UserData.Empty);
			IsfMainView.ExtraDetails.Visible = !PnlUserLogin.Visible;
			if (_user.Equals(UserData.Empty))
			{
				IsfMainView.SetPageCaption("רישום משתמש");
				IsfMainView.AddContents(Tools.MultiString("<br />", 15));
			}
			else
			{
				IsfMainView.ExtraDetails.Text = BuildExtraDetailsHTML();
				IsfMainView.SetPageCaption(_pageCaption);
			}
		}
		
		private void DecideAction()
		{
			//get data:
			string strFormAction=Tools.CStrDef(Request.Form["action"], "");
			string strQuerystringAction=Tools.CStrDef(Request.QueryString["action"], "");
			
			//add team?
			if (strFormAction == SportSiteAction.AddTeam.ToString())
			{
				AddTeam();
				return;
			}
			
			//add players?
			if (strQuerystringAction == SportSiteAction.AddPlayers.ToString())
			{
				AddPlayers();
				return;
			}
			
			//update champ logo?
			if (strQuerystringAction == SportSiteAction.UpdateChampLogo.ToString())
			{
				UpdateChampLogo();
				return;
			}

			//commit pending data?
			if (strFormAction == SportSiteAction.CommitPendingTeams.ToString())
			{
				CommitPendingTeams();
				return;
			}
			
			//commit pending data?
			if (strFormAction == SportSiteAction.CommitPendingPlayers.ToString())
			{
				CommitPendingPlayers();
				return;
			}
			
			//remove pending teams?
			if (strFormAction == SportSiteAction.DeletePendingTeams.ToString())
			{
				RemovePendingTeams();
				return;
			}
				
			//remove pending teams?
			if (strFormAction == SportSiteAction.DeletePendingPlayers.ToString())
			{
				RemovePendingPlayers();
				return;
			}
			
			//change pending number?
			if (strFormAction == SportSiteAction.ChangePendingNumber.ToString())
			{
				ChangePendingNumber();
				return;
			}
			
			//assign competitor players?
			if ((strFormAction == SportSiteAction.AssignCompetitionPlayers.ToString())||
				(strQuerystringAction == SportSiteAction.AssignCompetitionPlayers.ToString()))
			{
				AssignCompetitionPlayers();
				return;
			}
			
			//change player card?
			if (strFormAction == SportSiteAction.PlayerCardClick.ToString())
			{
				ChangePlayerCardStatus();
				return;
			}
			
			//show orders basket?
			if ((strQuerystringAction == SportSiteAction.ShowOrdersBasket.ToString())&&
				(strFormAction != SportSiteAction.CommitPendingTeams.ToString()) &&
				(strFormAction != SportSiteAction.CommitPendingPlayers.ToString()))
			{
				ShowOrdersBasket();
				return;
			}
			
			//show team details?
			if (strFormAction == SportSiteAction.ShowTeamDetails.ToString())
			{
				ShowTeamDetails();
				return;
			}
				
			//show player details?
			if (strFormAction == SportSiteAction.ShowPlayerDetails.ToString())
			{
				ShowPlayersDetails();
				return;
			}
			if ((strFormAction == SportSiteAction.PlayerCardClick.ToString())&&
				(strQuerystringAction != SportSiteAction.ShowOrdersBasket.ToString()))
			{
				ShowPlayersDetails();
				return;
			}
			
			//mark message read?
			if (strFormAction == SportSiteAction.MarkMessageRead.ToString())
			{
				MarkMessageRead();
				DisplayMessages(false);
				return;
			}
			
			//show all messages?
			if (strQuerystringAction == SportSiteAction.ShowMessages.ToString())
			{
				DisplayMessages(false);
				return;
			}
			
			//club register?
			if (strQuerystringAction == SportSiteAction.ClubRegister.ToString())
			{
				ClubRegister();
				return;
			}
			
			//events report?
			if (strQuerystringAction == SportSiteAction.ShowEventsReport.ToString())
			{
				ShowEventsReport();
				return;
			}
			
			//functionaty list?
			if (strQuerystringAction == SportSiteAction.ShowFunctionaryList.ToString())
			{
				ShowFunctionariesList();
				return;
			}
			
			//edit FAQ?
			if (strQuerystringAction == SportSiteAction.EditFAQ.ToString())
			{
				StartEditFAQ();
				return;
			}
			
			//download updates?
			if (strQuerystringAction == SportSiteAction.DownloadUpdates.ToString())
			{
				DownloadUpdates();
				return;
			}
			
			//edit flash news?
			if (strQuerystringAction == SportSiteAction.UpdateFlashNews.ToString())
			{
				EditFlashNews();
				return;
			}

			//edit polls?
			if (strQuerystringAction == SportSiteAction.UpdatePolls.ToString())
			{
				EditPolls();
				return;
			}

			//edit gallery?
			if (strQuerystringAction == SportSiteAction.UpdateImageGallery.ToString())
			{
				EditImageGallery();
				return;
			}
			
			//edit articles?
			if (strQuerystringAction == SportSiteAction.UpdateArticles.ToString())
			{
				EditArticles();
				return;
			}
			
			//edit links?
			if (strQuerystringAction == SportSiteAction.UpdateLinks.ToString())
			{
				EditLinks();
				return;
			}
			
			//edit pages?
			if (strQuerystringAction == SportSiteAction.UpdatePages.ToString())
			{
				EditPages();
				return;
			}

			//edit permanent championships?
			if (strQuerystringAction == SportSiteAction.UpdatePermanentChamps.ToString())
			{
				EditPermanentChamps();
				return;
			}
			
			//edit championship attachments?
			if (strQuerystringAction == SportSiteAction.UpdateChampionshipAttachments.ToString())
			{
				EditChampionshipAttachments();
				return;
			}
			
			//edit uncorirmed teams?
			if (strQuerystringAction == SportSiteAction.EditUnconfirmedPlayers.ToString())
			{
				EditUnconfirmedPlayers();
				return;
			}
			
			//update sponsors list?
			if (strQuerystringAction == SportSiteAction.UpdateSponsors.ToString())
			{
				UpdateSponsorsList();
				return;
			}
			
			//update special offers?
			if (strQuerystringAction == SportSiteAction.UpdateSpecialOffer.ToString())
			{
				UpdateSpecialOffer();
				return;
			}
			
			//update top banner?
			if (strQuerystringAction == SportSiteAction.UpdateTopBanner.ToString())
			{
				UpdateTopBanner();
				return;
			}
			
			//update main advertisement?
			if (strQuerystringAction == SportSiteAction.UpdateMainAdvertisement.ToString())
			{
				UpdateMainAdvertisement();
				return;
			}
			
			//update secondary advertisement?
			if (strQuerystringAction == SportSiteAction.UpdateSecondaryAdvertisement.ToString())
			{
				UpdateSecondaryAdvertisement();
				return;
			}
			
			//update secondary advertisement?
			if (strQuerystringAction == SportSiteAction.UpdateSmallAdvertisement.ToString())
			{
				UpdateSmallAdvertisement();
				return;
			}
			
			//edit events?
			if (strQuerystringAction == SportSiteAction.UpdateEvents.ToString())
			{
				EditEvents();
				return;
			}

			//add user action:
			if ((Request.QueryString.Count > 0)||(Request.Form.Count > 0))
			{
				try
				{
					SessionService.SessionService _service=
						new SessionService.SessionService();
					_service.CookieContainer = (System.Net.CookieContainer) 
						Session["cookies"];
					if (Request.QueryString.Count > 0)
					{
						string description="QueryString: ";
						foreach (string key in Request.QueryString.Keys)
						{
							description += key+"="+Request.QueryString[key]+"&";
						}
						description=description.Substring(0, description.Length-1);
						_service.AddUserAction(SessionService.Action.Register_Page_Load, 
							description, Sport.Core.Data.CurrentVersion);
					}
					if (Request.Form.Count > 0)
					{
						string description="Form: ";
						foreach (string key in Request.Form.Keys)
						{
							description += key+"="+Request.Form[key]+"&";
						}
						description=description.Substring(0, description.Length-1);
						_service.AddUserAction(SessionService.Action.Register_Page_Load, 
							description, Sport.Core.Data.CurrentVersion);
					}
				}
				catch (Exception ex)
				{
					System.Diagnostics.Debug.WriteLine("failed to add Register Page Load action: "+ex.Message);
				}
			}
		}

		/// <summary>
		/// read the querystring or form data, populates the global SportData collection
		/// </summary>
		private void SetSportData()
		{
			//first populate the one on one values:
			SportData["sport"] = Request.QueryString["sport"];
			SportData["champ"] = Request.QueryString["championship"];
			SportData["category"] = Request.QueryString["category"];
			SportData["TeamID"] = Request.Form["TeamID"];
			if (Request.QueryString["team"] != null)
				SportData["TeamID"] = Request.QueryString["team"];
			SportData["PlayerID"] = Request.Form["PlayerID"];
			SportData["MessageID"] = Request.Form["MessageID"];
			if (Request.QueryString[userSchoolID.UniqueID] != null)
				Session["user_school"] = Request.QueryString[userSchoolID.UniqueID];

			SportData["user_school"] = Tools.CStrDef(Session["user_school"], "");
			
			if (Sport.Core.Session.Cookies == null)
				return;

			//check if need to extract database information:
			if (SportData["category"] != null)
			{
				int categoryID=Tools.CIntDef(SportData["category"], -1);
				if (categoryID < 0)
				{
					AddViewError("מזהה קטגוריה לא חוקי: "+SportData["category"]);
					SportData["category"] = null;
				}
				else
				{
					Entity category=null;
					try 
					{
						category = Sport.Entities.ChampionshipCategory.Type.Lookup(categoryID);
					}
					catch (Exception e)
					{
						if (e.Message.ToLower() != "failed to load requested entity")
							throw e;
					}
					if (category == null)
					{
						AddViewError("קטגוריה לא קיימת במאגר הנתונים: "+SportData["category"]);
						SportData["category"] = null;
					}
					else
					{
						SportData["champ"] = category.Fields[(int) Sport.Entities.ChampionshipCategory.Fields.Championship].ToString();
					}
				}
			} //end if category data not null.
			
			if (SportData["champ"] != null)
			{
				//verify that selected championship is valid:
				int champID=Tools.CIntDef(SportData["champ"], -1);
				if (champID < 0)
				{
					AddViewError("מזהה אליפות לא חוקי: "+SportData["champ"]);
					SportData["champ"] = null;
				}
				else
				{
					Entity champ=null;
					try 
					{
						champ = Sport.Entities.Championship.Type.Lookup(champID);
					}
					catch (Exception e)
					{
						if (e.Message.ToLower() != "failed to load requested entity")
							throw e;
					}
					if (champ == null)
					{
						AddViewError("אליפות לא קיימת במאגר הנתונים: "+SportData["champ"]);
						SportData["champ"] = null;
					}
					else
					{
						//check if user can view this championship.
						//first, build proper entity:
						Sport.Entities.Championship championship = 
							new Sport.Entities.Championship(champ);
						//extract sport:
						int sportID=championship.Sport.Id;
						//get array of legal championships for that sport:
						Entity[] sportChamps=GetSportChampionships(sportID);
						//check if the championship exists in the array:
						bool exists=false;
						for (int i=0; i<sportChamps.Length; i++)
						{
							if (sportChamps[i].Id == championship.Id)
							{
								exists = true;
								break;
							}
						}
						
						if (exists == false)
						{
							AddViewError("שגיאה כללית: אינך מורשה עבור אליפות זו: "+SportData["champ"]);
							SportData["champ"] = null;
							SportData["category"] = null;
						}
						else
						{
							SportData["sport"] = sportID.ToString(); //championship.Sport.Id.ToString(); //champ.Fields[(int) Sport.Entities.Championship.Fields.Sport].ToString();
						}
					}
				} //end if legal championship id (greater than zero)
			} //end if championship data not null.
			
			//check if sport selected without championship:
			if ((SportData["sport"] != null)&&(SportData["champ"] == null))
			{
				//verify that we have at least one valid championship for the sport:
				Entity[] champs=GetSportChampionships(Int32.Parse(SportData["sport"]));
				if (champs.Length == 0)
				{
					AddViewError("שגיאה כללית: אין אליפויות עבור ענף ספורט זה: "+SportData["sport"]);
					SportData["sport"] = null;
					SportData["category"] = null;
				}
			}
		} //end function SetSportData

		#region Side Menu
		/// <summary>
		/// set navigation menu links.
		/// </summary>
		private void SetSideMenu()
		{
			SideNavBar.Links.Clear();
			
			//define general properties:
			SideNavBar.BkColor = SportSite.Common.Style.NavBarBgColor;
			LeftNavBar.BkColor = SportSite.Common.Style.LeftNavBarBgColor;
			SideNavBar.IsHebrew = true;
			LeftNavBar.IsHebrew = true;
			
			//define the various link styles
			LinkBox.LinkStyle rightNavbarStyle=new SportSite.Controls.LinkBox.LinkStyle("");
			rightNavbarStyle.AddLight = 60;
			rightNavbarStyle.CssClass = SportSite.Common.Style.NavBarLinkCss;
			rightNavbarStyle.hAlign = HorizontalAlign.Right;
			rightNavbarStyle.Height = 18;
			rightNavbarStyle.fontSize = 12;
			SideNavBar.DefaultLinkStyle = rightNavbarStyle;
			
			//copy first and change part of the properties:
			LinkBox.LinkStyle leftNavbarStyle=rightNavbarStyle;
			leftNavbarStyle.fontSize = 14;
			leftNavbarStyle.AddLight = 30;
			leftNavbarStyle.CssClass = SportSite.Common.Style.LeftNavBarLinkCss;
			LeftNavBar.DefaultLinkStyle = leftNavbarStyle;
			
			//copy and change some properties:
			LinkBox.LinkStyle leftNavbarNoLight=leftNavbarStyle;
			leftNavbarNoLight.AddLight = 0;
			
			//caption:
			LinkBox.LinkStyle captionStyle=leftNavbarNoLight;
			captionStyle.fontSize = 18;
			captionStyle.CssClass = SportSite.Common.Style.NavBarCaptionCss;
			captionStyle.bgColor = SportSite.Common.Style.LeftNavBarBgColor;
			captionStyle.hAlign = HorizontalAlign.Center;
			
			//extra details:
			LinkBox.LinkStyle extraDetailStyle=new SportSite.Controls.LinkBox.LinkStyle("");
			extraDetailStyle.AddLight = 0;
			extraDetailStyle.bgColor = Color.Empty;
			extraDetailStyle.CssClass = SportSite.Common.Style.ExtraDetailsCss;
			extraDetailStyle.fontSize = 0;
			extraDetailStyle.hAlign = HorizontalAlign.Right;
			OrdersBasket.DefaultLinkStyle = extraDetailStyle;
			
			//functionary list link:
			LinkBox.Link funcListLink = new LinkBox.Link("?action="+SportSiteAction.ShowFunctionaryList, 
				"רשימת בעלי תפקידים", LinkBox.LinkIndicator.Red);
			
			//FAQ link:
			LinkBox.Link faqLink = new LinkBox.Link("?action="+SportSiteAction.EditFAQ, 
				"פורום שאלות ותשובות", LinkBox.LinkIndicator.Red);
			
			//links
			LeftNavBar.Links += new LinkBox.Link(Data.AppPath+"/Main.aspx", "חזרה לראשי", LinkBox.LinkIndicator.Blue);
			
			//add the basket menus:
			_basketLinks[0] = new LinkBox.Link("", "סל הזמנות", captionStyle);
			_basketLinks[1] = new LinkBox.Link("", "אין קבוצות");
			_basketLinks[2] = new LinkBox.Link("", "אין שחקנים");
			_basketLinks[3] = new LinkBox.Link(
				"?action="+SportSiteAction.ShowOrdersBasket.ToString(), 
				"אישור שחקנים וקבוצות", LinkBox.LinkIndicator.Blue);
			_basketLinks[3].IsHotLink = true;
			foreach (LinkBox.Link basketLink in _basketLinks)
				LeftNavBar.Links += basketLink;
			
			//set basket menu teams and players:
			SetBasketData();
			
			//logout and messages - only if user is logged in...
			if (_user.Equals(UserData.Empty) == false)
			{
				//add options for authorized users only:
				int userid=_user.Id;
				
				//caption
				LeftNavBar.Links += new LinkBox.Link("", "כלים", LinkBox.LinkIndicator.None);
				
				//logout
				LeftNavBar.Links += new LinkBox.Link("?action="+SportSiteAction.Logout, "החלפת משתמש", LinkBox.LinkIndicator.Red);
				
				//messages
				LeftNavBar.Links += new LinkBox.Link("?action="+SportSiteAction.ShowMessages, "הודעות מערכת", LinkBox.LinkIndicator.Red);
				
				//school manager?
				if ((_user.Type == (int) Sport.Types.UserType.External)&&
					(_user.Permissions == 0))
				{
					//club register
					LeftNavBar.Links += new LinkBox.Link("?action="+SportSiteAction.ClubRegister, "רישום מועדון", LinkBox.LinkIndicator.StrongRed);
					
					//events report
					LeftNavBar.Links += new LinkBox.Link("?action="+SportSiteAction.ShowEventsReport, "דו\"ח אירועים", LinkBox.LinkIndicator.StrongRed);
					
					//functionary list
					LeftNavBar.Links += funcListLink;
				}
				
				//internal users only:
				if (_user.Type == (int) Sport.Types.UserType.Internal)
				{
					//edit:
					LeftNavBar.Links += BuildEditLinks(userid, leftNavbarStyle);
					
					//general:
					LinkBox.Link objGeneralLinks=
						BuildGeneralLinks(userid, leftNavbarStyle);
					objGeneralLinks.Links += funcListLink;
					objGeneralLinks.Links += faqLink;
					LeftNavBar.Links += objGeneralLinks;
					
					//download:
					LinkBox.Link downloadLink=new LinkBox.Link("?action="+SportSiteAction.DownloadUpdates, "הורדת עדכונים", LinkBox.LinkIndicator.Red);
					FileData[] arrFiles=DownloadManager.GetFilesList();
					for (int fileIndex=0; fileIndex<arrFiles.Length; fileIndex++)
					{
						downloadLink.Links += new LinkBox.Link("?action="+SportSiteAction.DownloadUpdates.ToString()+
							"&fileIndex="+fileIndex.ToString(), arrFiles[fileIndex].ShortDescription, 
							leftNavbarStyle);
					}
					LeftNavBar.Links += downloadLink;
				}
			}
			
			//exit if user not logged in:
			if (_user.Equals(UserData.Empty))
				return;
			
			//add caption:
			LinkBox.Link sportLinks=new SportSite.Controls.LinkBox.Link("", "ענפי ספורט", captionStyle);
			SideNavBar.Links += sportLinks;
			
			//get list of sports types:
			EntityType sportType=EntityType.GetEntityType(Sport.Entities.Sport.TypeName);
			Entity[] sports=sportType.GetEntities(null);
			
			//iterate through the sports, look for proper championships to display:
			for (int sportIndex=0; sportIndex<sports.Length; sportIndex++)
			{
				//get current sport:
				Entity sport=sports[sportIndex];
				
				//get its championships:
				Entity[] championships=GetSportChampionships(sport.Id);
				
				//check if it has any championships:
				if (championships.Length == 0)
					continue;
				
				//create link:
				LinkBox.Link curLink=new LinkBox.Link("?sport="+sport.Id, sport.Name, rightNavbarStyle, LinkBox.LinkIndicator.Blue);
				
				//add nested links.
				//create child menu:
				for (int champIndex=0; champIndex<championships.Length; champIndex++)
				{
					Entity champ=championships[champIndex];
					
					//create new link:
					string strChampName=champ.Name;
					int champRegion=Tools.CIntDef(
						champ.Fields[(int) Sport.Entities.Championship.Fields.Region], 
						-1);
					if (champRegion == Sport.Entities.Region.CentralRegion)
					{
						strChampName += " - ארצי";
					}
					else
					{
						if (_user.Region.ID == Sport.Entities.Region.CentralRegion)
						{
							Sport.Entities.Region region=new Sport.Entities.Region(champRegion);
							strChampName += " - "+region.Name;
						}
					}
					LinkBox.Link champLink=new LinkBox.Link("?championship="+champ.Id, strChampName, rightNavbarStyle);
					
					//championship logo:
					if (_user.Type == (int) Sport.Types.UserType.Internal)
						champLink.Links += new LinkBox.Link("?action="+SportSiteAction.UpdateChampLogo+"&championship="+champ.Id, "[לוגו אליפות]");
					
					//add nested links.
					//get list of proper categories:
					EntityType categoryType=Sport.Entities.ChampionshipCategory.Type;
					EntityFilter categoryFilter=new EntityFilter((int) Sport.Entities.ChampionshipCategory.Fields.Championship, champ.Id);
					//read from database or memory:
					Entity[] categories=categoryType.GetEntities(categoryFilter);
					for (int categoryIndex=0; categoryIndex<categories.Length; categoryIndex++)
					{
						Entity category=categories[categoryIndex];
						//add new link:
						champLink.Links += new LinkBox.Link("?category="+category.Id, category.Name, LinkBox.LinkStyle.ChangeFontSize(rightNavbarStyle, 10));
					}
					
					//add link to the sports link:
					curLink.Links += champLink;
				}
			
				//add the link:
				SideNavBar.Links += curLink;
			} //end loop over sports
			
			foreach (LinkBox.Link link in SideNavBar.Links)
			{
				if (CheckActiveLink(link))
					break;
			}
		} //end function SetSideMenu
		
		private void SetBasketData()
		{
			//initialize local variables:
			int teamsCount=0;
			int playersCount=0;
			
			//can show basket only if user has defined school:
			if ((_user.Equals(UserData.Empty) == false)&&(_user.School.ID >= 0))
			{
				//get list of pending teams and players and store the amounts:
				SportSite.RegistrationService.RegistrationService regService=new SportSite.RegistrationService.RegistrationService();
				regService.CookieContainer = (System.Net.CookieContainer) Session["cookies"];
				SportSite.RegistrationService.TeamData[] arrPendingTeams=regService.GetPendingTeams(_user.School.ID);
				SportSite.RegistrationService.PlayerData[] arrPendingPlayers=regService.GetPendingPlayers(_user.School.ID, -1);
				teamsCount = arrPendingTeams.Length;
				playersCount = arrPendingPlayers.Length;
			}
			
			//hide?
			if ((teamsCount == 0)&&(playersCount == 0))
			{
				foreach (LinkBox.Link basketLink in _basketLinks)
					basketLink.Visible = false;
				return;
			}
			
			//show first and last links:
			_basketLinks[0].Visible = true;
			_basketLinks[3].Visible = true;

			//teams:
			_basketLinks[1].Text = Sport.Common.Tools.BuildOneOrMany("קבוצה", "קבוצות", 
				teamsCount, false);
			
			//players
			_basketLinks[2].Text = Sport.Common.Tools.BuildOneOrMany("שחקן", "שחקנים", 
				playersCount, true);
			
			//visibility:
			_basketLinks[1].Visible = (teamsCount > 0);
			_basketLinks[2].Visible = (playersCount > 0);
		} //end function SetBasketData
		
		private LinkBox.Link BuildEditLinks(int userid, LinkBox.LinkStyle style)
		{
			ArrayList arrLinks=new ArrayList();
			
			//edit flash news:
			if (Tools.IsAuthorized_News(userid))
				arrLinks.Add(new LinkBox.Link("?action="+SportSiteAction.UpdateFlashNews, "עריכת מבזקים", style));

			//edit polls:
			if (Tools.IsAuthorized_Polls(userid))
				arrLinks.Add(new LinkBox.Link("?action="+SportSiteAction.UpdatePolls, "עריכת סקרים", style));

			//edit gallery - SHOULD ADD PERMISSIONS
			arrLinks.Add(new LinkBox.Link("?action="+SportSiteAction.UpdateImageGallery, "עריכת גלריית תמונות", style));

			//edit articles:
			if (Tools.IsAuthorized_Articles(userid))
				arrLinks.Add(new LinkBox.Link("?action="+SportSiteAction.UpdateArticles, "עריכת כתבות", style));

			//edit links:
			if (Tools.IsAuthorized_Articles(userid))
				arrLinks.Add(new LinkBox.Link("?action="+SportSiteAction.UpdateLinks, "עריכת קישורים", style));
			
			//edit events:
			arrLinks.Add(new LinkBox.Link("?action="+SportSiteAction.UpdateEvents, "עריכת אירועים", style));
			
			//edit sponsors:
			arrLinks.Add(new LinkBox.Link("?action="+SportSiteAction.UpdateSponsors, "עריכת נותני חסות", style));
			
			//banners
			LinkBox.Link objBannerLinks=new LinkBox.Link("", "עריכת באנרים", style);
			objBannerLinks.Links += new LinkBox.Link("?action="+SportSiteAction.UpdateSpecialOffer, "קליק חודשי", style);
			objBannerLinks.Links += BuildTopBannerLinks();
			//objBannerLinks.Links += new LinkBox.Link("?action="+SportSiteAction.UpdateTopBanner+"&index=JHWC", "English Banner", style);
			objBannerLinks.Links += new LinkBox.Link("?action="+SportSiteAction.UpdateMainAdvertisement, "פרסומת ראשית", style);
			objBannerLinks.Links += new LinkBox.Link("?action="+SportSiteAction.UpdateSecondaryAdvertisement, "פרסומת משנית", style);
			objBannerLinks.Links += new LinkBox.Link("?action="+SportSiteAction.UpdateSmallAdvertisement, "באנר צד קטן", style);
			arrLinks.Add(objBannerLinks);
			
			if (arrLinks.Count == 0)
				return LinkBox.Link.Empty;
			
			if (arrLinks.Count == 1)
				return (LinkBox.Link) arrLinks[0];

			LinkBox.Link result=new LinkBox.Link("", "תפריט עריכה", style, LinkBox.LinkIndicator.Red);
			result.Links += (LinkBox.Link[]) arrLinks.ToArray(typeof(LinkBox.Link));
			return result;
		}
		
		private LinkBox.Link BuildTopBannerLinks()
		{
			string strBaseLink="?action="+SportSiteAction.UpdateTopBanner+"&index=%i";
			LinkBox.Link result=new LinkBox.Link(strBaseLink.Replace("%i", "0"), "באנר עליון");
			string[] arrDataFiles=Banner.GetTopBannerDataFiles(this.Page, this.Server);
			if (arrDataFiles.Length > 1)
			{
				result.Url = "";
				for (int i=1; i<=arrDataFiles.Length; i++)
				{
					result.Links += new LinkBox.Link(strBaseLink.Replace("%i", i.ToString()), 
						Sport.Common.Tools.IntToHebrew(i, false));
				}
			}
			return result;
		}
		
		private LinkBox.Link BuildGeneralLinks(int userid, LinkBox.LinkStyle style)
		{
			ArrayList arrLinks=new ArrayList();
			
			//edit pages:
			if (Tools.IsAuthorized_Pages(userid))
				arrLinks.Add(new LinkBox.Link("?action="+SportSiteAction.UpdatePages, "שינוי תוכן עמודים", style));
			
			//edit permanent championships:
			if (Tools.IsAuthorized_PermanentChamps(userid))
				arrLinks.Add(new LinkBox.Link("?action="+SportSiteAction.UpdatePermanentChamps, "הגדרת ליגות קבועות", style));
			
			//edit championship attachments:
			arrLinks.Add(new LinkBox.Link("?action="+SportSiteAction.UpdateChampionshipAttachments, "ניהול קבצים של אליפויות", style));
			
			//unconfirmed teams:
			arrLinks.Add(new LinkBox.Link("?action="+SportSiteAction.EditUnconfirmedPlayers, "ניהול שחקנים לא מאושרים", style));
			
			if (arrLinks.Count == 0)
				return LinkBox.Link.Empty;
			
			if (arrLinks.Count == 1)
				return (LinkBox.Link) arrLinks[0];
			
			LinkBox.Link result=new LinkBox.Link("", "שונות", style, LinkBox.LinkIndicator.Red);
			result.Links += (LinkBox.Link[]) arrLinks.ToArray(typeof(LinkBox.Link));
			return result;
		}

		#endregion
		
		private bool CheckActiveLink(LinkBox.Link link)
		{
			string strURL=link.Url;
			bool result=false;
			string sportID=Tools.CStrDef(SportData["sport"], "-999");
			string champID=Tools.CStrDef(SportData["champ"], "-999");
			string categoryID=Tools.CStrDef(SportData["category"], "-999");
			if ((strURL.IndexOf("sport="+sportID)>=0)||
				(strURL.IndexOf("championship="+champID)>=0)||
				(strURL.IndexOf("category="+categoryID)>=0))
			{
				SideNavBar.Links.ChangeBgColor(link, Common.Style.ActiveLinkColor);
				result = true;
			}
			if (link.Links != null)
			{
				foreach (LinkBox.Link childLink in link.Links)
				{
					if (CheckActiveLink(childLink))
						result = true;
				}
			}
			return result;
		}

		/// <summary>
		/// returns array of Championship entities for given sports type.
		/// </summary>
		private Entity[] GetSportChampionships(int sportID, int regionID, 
			bool blnUseStatus)
		{
			EntityType champType=Sport.Entities.Championship.Type;
			EntityField champStatusField=champType.Fields[(int) Sport.Entities.Championship.Fields.Status];
			ArrayList result=new ArrayList();
			
			//filter:
			EntityFilter filter=Tools.GetChampionshipDefaultFilter();
			filter.Add(new EntityFilterField(
				(int) Sport.Entities.Championship.Fields.Sport, sportID));
			result.AddRange(Sport.Entities.Championship.Type.GetEntities(filter));
			
			//central user can see all championships (?)
			if (regionID != Sport.Entities.Region.CentralRegion)
			{
				//check region:
				ArrayList arrToRemove=new ArrayList();
				foreach (Entity ent in result)
				{
					//get current championship:
					Sport.Entities.Championship champ = new Sport.Entities.Championship(ent);
					
					if (champ.Region.IsNationalRegion())
						continue;
					
					//get its regions:
					ChampionshipRegion[] regions=champ.GetRegions();
					
					//verify the the user's region is listed for this championship:
					bool exists=false;
					for (int i=0; i<regions.Length; i++)
					{
						if (regions[i].Region.Id == regionID)
						{
							exists = true;
							break;
						}
					}
					
					//clear the championship if it's not defined for user's region:
					if (exists == false)
						arrToRemove.Add(ent);
				} //end loop over championships to be returned
				
				//remove
				foreach (Entity objToRemove in arrToRemove)
					result.Remove(objToRemove);
			} //end if central user.
			
			//remove null values from the array:
			//result = (Entity[]) Sport.Common.Tools.RemoveNullValues(result, typeof(Entity));
			
			//return proper entities:
			return (Entity[]) result.ToArray(typeof(Entity));
		}
		
		private Entity[] GetSportChampionships(int sportID)
		{
			return GetSportChampionships(sportID, _user.Region.ID, true);
		}
		
		private void SetOrdersBasket()
		{
			//define general properties:
			OrdersBasket.BkColor = SideNavBar.BkColor;
			OrdersBasket.IsHebrew = true;
			
			//clear previous links:
			OrdersBasket.Links.Clear();
			
			//define the various link styles
			LinkBox.LinkStyle captionStyle=new SportSite.Controls.LinkBox.LinkStyle("");
			captionStyle.AddLight = 0;
			captionStyle.fontSize = 18;
			captionStyle.CssClass = SportSite.Common.Style.NavBarCaptionCss;
			captionStyle.bgColor = SportSite.Common.Style.LeftNavBarBgColor;
			captionStyle.hAlign = HorizontalAlign.Center;
			captionStyle.Height = 22;
			
			//default style:
			LinkBox.LinkStyle ordersBasketStyle=new SportSite.Controls.LinkBox.LinkStyle("");
			ordersBasketStyle.AddLight = 0;
			ordersBasketStyle.CssClass = SportSite.Common.Style.NavBarLinkCss;
			ordersBasketStyle.hAlign = HorizontalAlign.Right;
			ordersBasketStyle.Height = 22;
			ordersBasketStyle.fontSize = 12;
			OrdersBasket.DefaultLinkStyle = ordersBasketStyle;
		} //end function SetOrdersBasket

		private void AddStyle()
		{
			lbStyle.Text = "";
		}
		#endregion
		
		#region reset sub articles
		private void CheckResetSubArticles()
		{
			if (Request.QueryString["action"] != Common.SportSiteAction.ResetSubArticles.ToString())
				return;
			
			WebSiteServices.WebsiteService service = 
				new WebSiteServices.WebsiteService();
			WebSiteServices.ArticleData[] arrSubArticles = service.GetSubArticles();
			if ((arrSubArticles == null)||(arrSubArticles.Length < 2))
			{
				AddErrorMessage("not enough sub articles!");
				return;
			}
			
			for (int i=1; i<arrSubArticles.Length; i++)
			{
				arrSubArticles[i].IsSub = false;
				service.UpdateArticle(arrSubArticles[i], 
					_user.Login, _user.Password, _user.Id);
			}
			
			AddViewSuccessMsg("כתבות משניות אופסו בהצלחה");
			EditArticles();
		}
		#endregion
		
		#region instant messages
		private void CheckInstantMessage()
		{
			//got any request?
			if ((Request.QueryString["action"] != Common.SportSiteAction.ShowInstantMessage.ToString())&&
				(Request.QueryString["action"] != "mark_message_read"))
			{
				return;
			}
			
			//logged in?
			if (Session[UserManager.SessionKey] == null)
			{
				Response.Write("You are not logged in!");
				Response.End();
			}
			
			//get message ID:
			string strMessageID=Request.QueryString["id"];
			int messageID=Tools.CIntDef(strMessageID, -1);
			
			//valid?
			if (messageID < 0)
			{
				Response.Write("invalid message ID!");
				Response.End();
			}
			
			//get current user:
			UserData objUser=((UserData) Session[UserManager.SessionKey]);
			int userID=objUser.Id;
			
			//get message data:
			InstantMessage objMessage=null;
			try
			{
				objMessage = new InstantMessage(messageID);
			}
			catch
			{}
			
			//got anything?
			if ((objMessage == null)||(objMessage.Id < 0))
			{
				Response.Write("invalid message!");
				Response.End();
			}
			
			//check if the message has been sent here:
			//Response.Write("recipient: " +objMessage.Recipient.Id+"<br />");
			//Response.Write("user: " +userID+"<br />");
			if (objMessage.Recipient.Id != userID)
			{
				Response.Write("this messagzsdfdfdfe wasn't sent to you!!!");
				Response.End();
			}
			
			//decide proper action:
			if (Request.Form["action"] == "mark_message_read")
			{
				//mark message read?
				if (Request.Form["message_read"] == "1")
				{
					DataServices.DataService service=new DataServices.DataService();
					service.MarkMessageRead(objUser.Login, objUser.Password, messageID);
				}
				
				//close the window:
				Response.Write("success");
			}
			else
			{
				//output proper HTML:
				Response.Write(Tools.GetInstantMessageHTML(objMessage, this.Server, 
					this.Request));
			}
			
			//done.
			Response.End();
		} //end function CheckInstantMessage
		#endregion
		
		#region AJAX code
		private void CheckAJAX()
		{
			//got any request?
			if (Request.QueryString["ajax"] != "1")
				return;
			
			int region=Tools.CIntDef(Request.QueryString["region"], -1);
			
			//maybe sport?
			if (Request.QueryString["sport"] != null)
			{
				//get sport:
				int sport=Tools.CIntDef(Request.QueryString["sport"], -1);
				if (sport >= 0)
				{
					SendEntitiesResponse(GetSportChampionships(sport, region, false));
				}
				Response.End();
				return;
			}
			
			//maybe championship?
			if (Request.QueryString["champ"] != null)
			{
				//get sport:
				int champ=Tools.CIntDef(Request.QueryString["champ"], -1);
				if (champ >= 0)
				{
					EntityFilter filter=new EntityFilter(
						(int) Sport.Entities.ChampionshipCategory.Fields.Championship, champ);
					SendEntitiesResponse(Sport.Entities.ChampionshipCategory.Type.GetEntities(filter));
				}
				Response.End();
				return;
			}
		}

		private void SendEntitiesResponse(Entity[] entities)
		{
			System.Text.StringBuilder sb=new System.Text.StringBuilder();
			for (int i=0; i<entities.Length; i++)
			{
				sb.Append(entities[i].Id.ToString()+"|"+entities[i].Name);
				if (i < (entities.Length-1))
					sb.Append("*");
			}
			Response.Write(sb.ToString());
		}
		#endregion
		
		#region User Login
		/// <summary>
		/// verify that user is logged in to the system.
		/// </summary>
		private void UserLogin()
		{
			//already executed?
			if (_loginExecuted)
				return;
			else
				_loginExecuted = true;
			
			//get data from session if possible:
			if (Session[UserManager.SessionKey] is UserData)
			{
				IsfMainView.ClearContents();
				_user = (UserData) Session[UserManager.SessionKey];
			}
			
			//check if null, meaning user not logged in or session expired
			if (_user.Equals(UserData.Empty))
			{
				HideAll();
				return;
			}
			
			//getting here means user is logged in.
			AddViewText("<div dir=\"rtl\">", 0, true);
			AddViewText("ברוך הבא "+_user.Name+".<br />", 1, true);
			
			if (_user.Type == ((int) Sport.Types.UserType.Internal))
			{
				WebSiteServices.WebsiteService service = 
					new WebSiteServices.WebsiteService();
				WebSiteServices.PendingCompetitorData[] arrTeamsData = 
					service.GetPendingCompetitorsTeamsData();
				if (arrTeamsData != null)
				{
					ArrayList arrLinks = new ArrayList();
					foreach (WebSiteServices.PendingCompetitorData teamData in arrTeamsData)
					{
						int teamID = teamData.Team;
						string strPhaseName = teamData.Phase;
						string strGroupName = teamData.Group;
						Sport.Entities.Team team = null;
						try
						{
							team = new Sport.Entities.Team(teamID);
						}
						catch {}
						if (team == null)
							continue;
						Sport.Entities.Championship championship = team.Championship;
						Sport.Entities.Region region = championship.Region;
						Sport.Entities.User supervisor = region.Coordinator;
						bool blnShowLink = false;
						if ((supervisor != null)&&(_user.Id == supervisor.Id))
							blnShowLink = true;
						if (!blnShowLink)
						{
							Sport.Entities.User champSupervisor = championship.Supervisor;
							if (champSupervisor != null)
								if (champSupervisor.Id == _user.Id)
									blnShowLink = true;
						}
						if (blnShowLink)
						{
							Sport.Entities.ChampionshipCategory category = team.Category;
							int categoryID = category.Id;
							string strChampName = championship.Name+" "+category.Name;
							string strCurLink = "<a href=\"?action="+
								SportSiteAction.AssignCompetitionPlayers.ToString()+
								"&category="+categoryID+"&team="+teamID+
								"&phase="+strPhaseName+"&group="+strGroupName+
								"\">"+team.TeamName()+" ("+strChampName+" שלב "+
								strPhaseName.Replace("שלב", "")+" בית "+
								strGroupName.Replace("בית", "")+")</a>";
							arrLinks.Add(strCurLink);
						}
					}
					if (arrLinks.Count > 0)
					{
						string strHTML = "<div style=\"border: 1px solid black;\">";
						strHTML += "חלוקת מתמודדים מחכה לאישור עבור הקבוצות הבאות:<br />";
						for (int i=0; i<arrLinks.Count; i++)
						{
							strHTML += arrLinks[i].ToString();
							if (i < (arrLinks.Count-1))
								strHTML += "<br />";
						}
						strHTML += "</div>";
						IsfMainView.AddContents(strHTML);
					}
				}
			}
			
			/*
			if (_user.Type == (int) Sport.Types.UserType.Internal)
			{
				string strTempDownloadHTML="<h1 style=\"border: 1px solid black;\">";
				strTempDownloadHTML += "במידה ואתם מקבלים שגיאה בעת הפעלת התוכנה, "+
					"באפשרותכם להוריד ידנית את הגרסה המלאה או להעתיק את "+
					"<a href=\"temp/Sport.Core.zip\" "+
					"style=\"text-decoration: underline;\">הקובץ הזה</a> "+
					"לספרייה C:\\Sportsman ולפתוח את קובץ הזיפ שם. עמכם הסליחה!";
				strTempDownloadHTML += "</h1>";
				AddViewText(strTempDownloadHTML);
			}
			*/
			
			//define Response for download manager:
			SportSite.Core.DownloadManager.Response = this.Response;
			
			//add javascript:
			AddGeneralJavascript();
			
			//initialize service:
			_playerCardService = new PlayerCardService.PlayerCardService();
			_playerCardService.CookieContainer = (System.Net.CookieContainer) Session["cookies"];
			
			//set season:
			Sport.Core.Session.Season = GetLastSeason();
			
			if ((_user.Region.ID == Sport.Entities.Region.CentralRegion)&&
				(Tools.CStrDef(SportData["user_school"], "").Length == 0))
				AddViewText("בחר בית ספר עבורו ברצונך לרשום קבוצה.<br />", true);
			
			//get orders:
			string strOrders=DecideOrders();
			
			if (strOrders.Length > 0)
				AddViewText(strOrders+"<br />", true);

			AddViewText("</div>", true);

			//add messages table view if there is no action:
			if (Request.QueryString.Keys.Count == 0)
				DisplayMessages(true);
		} //end function UserLogin
		
		/// <summary>
		/// log out the user and redirect to the login page again.
		/// </summary>
		private void UserLogout()
		{
			Session.Abandon();
			Response.Redirect("Register.aspx", true);
		}
		
		private void VerifyUser(object sender, ImageClickEventArgs e)
		{
			//get user input:
			string strUsername=Tools.CStrDef(Request.Form[edUsername.UniqueID], "");
			string strPassword=Tools.CStrDef(Request.Form[edUserPassword.UniqueID], "");
			
			//check if any input given:
			if (strUsername.Length == 0)
				return;
			
			//verify those details with the user manager:
			if (userManager.VerifyUser(strUsername, strPassword) == false)
			{
				//user failed to login.
				AddViewError("זיהוי משתמש או סיסמא שגויים");
				return;
			}
			
			//getting here means user is ok.
			_user = (UserData) Session[UserManager.SessionKey];
			UserLogin();
		} //end function VerifyUser
		#endregion

		#region General Stuff
		private string BuildExtraDetailsHTML()
		{
			System.Text.StringBuilder sb=new System.Text.StringBuilder();
			sb.Append("<div align=\"center\"><br />");
			sb.Append("<table class=\""+Common.Style.ExtraDetailsCss+"\">");
			bool[] arrBoldText=new bool[] {false, true};
			sb.Append("<tr><td style=\"width: 10%; height: 5px;\">&nbsp;</td><td style=\"width: 100%;\">&nbsp;</td></tr>");
			sb.Append(Tools.BuildTableRow(new string[] {"מחוז:", _user.Region.Name}, arrBoldText, false));
			if (_user.Region.ID == Sport.Entities.Region.CentralRegion)
			{
				//check if user selected school:
				if (Tools.CStrDef(SportData["user_school"], "").Length > 0)
				{
					int schoolID=Int32.Parse(SportData["user_school"]);
					Entity school=Sport.Entities.School.Type.Lookup(schoolID);
					_user.School.ID = school.Id;
					_user.School.Name = school.Name;
					Session[UserManager.SessionKey] = _user;
					userSchoolID.Value = school.Id.ToString();
					SetBasketData();
				}
				else
				{
					if (_user.School.ID > 0)
						SportData["user_school"] = _user.School.ID.ToString();
				}
				sb.Append(Tools.BuildTableRow(new string[] {"בי\"ס:", "<span id=\"UserSchool\">"+_user.School.Name+"</span>"}, arrBoldText, false));
				sb.Append("<tr><td colspan=\"2\" style=\"text-align: center;\">");
				sb.Append(Tools.BuildSubmitButton("SelectSchool(event);", "Images/btn_small_choose.gif"));
				sb.Append("</td></tr>");
				AddSelectSchoolJavascript();
			}
			else
			{
				if (_user.School.ID > 0)
				{
					sb.Append(Tools.BuildTableRow(new string[] {"בי\"ס:", _user.School.Name}, arrBoldText, false));
					this.userSchoolID.Value = _user.School.ID.ToString();
					SportData["user_school"] = _user.School.ID.ToString();
				}
			}
			sb.Append("</table><br /><br /><br /></div>");
			return sb.ToString();
		}
		
		/// <summary>
		/// decides what orders to write according to the selections made by user.
		/// </summary>
		private string DecideOrders()
		{
			//check if user selected sports type:
			if (SportData["sport"] == null) 
				return "אנא בחר ענף ספורט מהתפריט.";
			
			//check if user selected championship:
			if (SportData["champ"] == null) 
				return "אנא בחר אליפות עבור ענף ספורט זה.";
			
			//check if user selected category:
			if (SportData["category"] == null) 
				return "אנא בחר קטגורית אליפות עבורה ברצונך לרשום קבוצה.";
			
			//check if can add team:
			Entity champ=Sport.Entities.Championship.Type.Lookup(Int32.Parse(SportData["champ"]));
			Championship championship=new Championship(champ.Id);
			int champStatus=(int) champ.Fields[(int) Sport.Entities.Championship.Fields.Status];
			int teamRegisterStatus=(int) Sport.Types.ChampionshipType.TeamRegister;
			if (champStatus != teamRegisterStatus)
				return "<span style=\"color: red\">אליפות לא במצב רישום קבוצות, לא ניתן להוסיף קבוצה.</span>";
			if ((!championship.Region.IsNationalRegion())&&(Tools.CIntDef(champ.Fields[(int)Sport.Entities.Championship.Fields.IsOpen], -1) == 0))
				return "<span style=\"color: red\">אליפות זו סגורה, לא ניתן להוסיף קבוצה.</span>";
			if (_user.School.ID <= 0)
				return "<span style=\"color: red\">לא הוגדר בית ספר, לא ניתן להוסיף קבוצה.</span>";
			
			//can add team.
			return "<span style=\"color: blue\">לחץ על כפתור הוספת קבוצה על מנת לרשום קבוצה חדשה.</span>";
		} //end function DecideOrders

		/// <summary>
		/// finds the latest not closed season in the database.
		/// </summary>
		/// <returns></returns>
		private int GetLastSeason()
		{
			//define filter for not closed seasons:
			Sport.Data.EntityFilterField seasonStatus = new Sport.Data.EntityFilterField(
				(int) Sport.Entities.Season.Fields.Status, Sport.Types.SeasonStatus.Closed, true);

			//grab seasons:
			Sport.Data.Entity[] seasons = Sport.Entities.Season.Type.GetEntities(
				new Sport.Data.EntityFilter(seasonStatus));
			
			//iterate over seasons and find the maximum:
			int maxSeason=0;
			foreach (Sport.Data.Entity season in seasons)
			{
				if (season.Id > maxSeason)
					maxSeason = season.Id;
			}
			
			return maxSeason;
		}
		
		private void ReloadEntities()
		{
			//reset all relevant entities:
			Sport.Entities.Championship.Type.Reset(null);
			Sport.Entities.ChampionshipCategory.Type.Reset(null);
			Sport.Entities.Message.Type.Reset(null);
			Sport.Entities.Player.Type.Reset(null);
			Sport.Entities.Sport.Type.Reset(null);
			Sport.Entities.Team.Type.Reset(null);
		}
		
		private void DownloadUpdates()
		{
			//check for file index:
			if (Request.QueryString["fileIndex"] == null)
			{
				AddViewText("אנא בחר קובץ להורדה.");
				return;
			}
			
			//getting here means there is file index to download:
			int fileIndex=Tools.CIntDef(Request.QueryString["fileIndex"], -1);
			FileData[] arrFiles=DownloadManager.GetFilesList();
			if ((fileIndex < 0)||(fileIndex >= arrFiles.Length))
			{
				AddErrorMessage("אינדקס קובץ שגוי: "+fileIndex);
				return;
			}
			
			//display details:
			AddViewText(arrFiles[fileIndex].FullDescription+"<br />", 14, Color.Blue, -1);

			//check if need to perform actual download:
			if (Request.Form["action"] == SportSiteAction.PerformDownload.ToString())
			{
				AddViewText("הורדה מתבצעת...<br />");
				DownloadManager.Session = this.Session;
				DownloadManager.DownloadFile(fileIndex, Server.MapPath("temp"), "temp");
			}
			else
			{
				AddViewText("<input type=\"button\" value=\"הורד\" "+
					"onclick=\"this.form.elements['action'].value = '"+
					SportSiteAction.PerformDownload.ToString()+"'; "+
					"this.form.submit();\" /><br />");
			}
			
			//download the file:
			//SportSite.Core.DownloadManager.DownloadFile(0, Server.MapPath("temp"), "temp");
		} //end function DownloadUpdates
		
		private void CreateAdminControls()
		{
			if (PnlUserLogin != null)
				return;
			
			PnlUserLogin=new Panel();
			PnlUserLogin.CssClass = "UserLoginPanel";
			Table objTable=new Table();
			PnlUserLogin.Attributes["dir"] = "rtl";
			objTable.Attributes["dir"] = "rtl";
			objTable.BackColor = Color.White;
			TableRow objRow;
			TableCell objCell;
			objTable.HorizontalAlign = HorizontalAlign.Right;
			
			//first row - username
			objRow = new TableRow();
			objCell = new TableCell();
			objCell.Attributes["dir"] = "rtl";
			objCell.HorizontalAlign = HorizontalAlign.Right;
			objCell.Text = "שם משתמש:";
			objRow.Cells.Add(objCell);
			//----------------
			objCell = new TableCell();
			objCell.HorizontalAlign = HorizontalAlign.Right;
			edUsername = new HtmlInputText("text");
			edUsername.ID = "edUserName";
			edUsername.MaxLength = 20;
			edUsername.Attributes.Add("default", "1");
			edUsername.Attributes["dir"] = "ltr";
			objCell.Controls.Add(edUsername);
			objRow.Cells.Add(objCell);
			objTable.Rows.Add(objRow);
			
			//second row - password
			objRow = new TableRow();
			objCell = new TableCell();
			objCell.Attributes["dir"] = "rtl";
			objCell.HorizontalAlign = HorizontalAlign.Right;
			objCell.Text = "סיסמא:";
			objRow.Cells.Add(objCell);
			//----------------
			objCell = new TableCell();
			objCell.HorizontalAlign = HorizontalAlign.Right;
			edUserPassword = new TextBox();
			edUserPassword.MaxLength = 20;
			edUserPassword.TextMode = TextBoxMode.Password;
			edUserPassword.Attributes["dir"] = "ltr";
			objCell.Controls.Add(edUserPassword);
			objRow.Cells.Add(objCell);
			objTable.Rows.Add(objRow);
			
			//third row - submit button
			objRow = new TableRow();
			objCell = new TableCell();
			objCell.HorizontalAlign = HorizontalAlign.Center;
			objCell.ColumnSpan = 2;
			objCell.Text = Tools.BuildSubmitButton(null);
			objRow.Cells.Add(objCell);
			objTable.Rows.Add(objRow);
			
			//add hidden:
			PnlUserLogin.Controls.Add(objTable);
			
			//add into main container:
			IsfMainView.AddContents(PnlUserLogin);
			
			//general controls
			btnAddTeam=new ImageButton();
			btnAddTeam.ImageUrl = SportSite.Common.Data.AppPath+"/Images/AddTeam.gif";
			btnAddTeam.ID = "BtnAddTeam";
			//btnAddTeam.Click += new ImageClickEventHandler(this.AddTeamClicked);
		} //end function CreateAdminControls

		private void HideAll()
		{
			IsfMainView.ExtraDetails.Visible = false;
		}
		#endregion
		
		#region Javascript Coding
		private void AddGeneralJavascript()
		{
			System.Text.StringBuilder builder=new System.Text.StringBuilder();
			builder.Append("<script type=\"text/javascript\" language=\"javascript\">");
			builder.Append("var selectSchoolDlg=0; ");
			builder.Append("var timer1=0; ");
			builder.Append("var selectPlayersDlg=0; ");
			builder.Append("var timer2=0; ");
			builder.Append("function WindowFocus(event) {");
			builder.Append("   if ((typeof selectSchoolDlg != \"undefined\")&&(selectSchoolDlg)) {");
			builder.Append("      try {");
			builder.Append("         selectSchoolDlg.focus();");
			builder.Append("      }");
			builder.Append("      catch (e) {");
			builder.Append("         selectSchoolDlg = 0;");
			builder.Append("         clearTimeout(timer1);");
			builder.Append("         return false;");
			builder.Append("      }");
			builder.Append("      return true;");
			builder.Append("   }");
			builder.Append("   if ((typeof selectPlayersDlg != \"undefined\")&&(selectPlayersDlg)) {");
			builder.Append("      try {");
			builder.Append("         selectPlayersDlg.focus();");
			builder.Append("      }");
			builder.Append("      catch (e) {");
			builder.Append("         selectPlayersDlg = 0;");
			builder.Append("         clearTimeout(timer2);");
			builder.Append("         return false;");
			builder.Append("      }");
			builder.Append("      return true;");
			builder.Append("   }");
			builder.Append("} ");
			builder.Append("function WindowUnload(event) {");
			builder.Append("   if ((typeof selectSchoolDlg != \"undefined\")&&(selectSchoolDlg)) {");
			builder.Append("      try {");
			builder.Append("         selectSchoolDlg.close();");
			builder.Append("      }");
			builder.Append("      catch (e) {");
			builder.Append("      }");
			builder.Append("   }");
			builder.Append("   if ((typeof selectPlayersDlg != \"undefined\")&&(selectPlayersDlg)) {");
			builder.Append("      try {");
			builder.Append("         selectPlayersDlg.close();");
			builder.Append("      }");
			builder.Append("      catch (e) {");
			builder.Append("      }");
			builder.Append("   }");
			builder.Append("} ");
			builder.Append("window.onfocus = WindowFocus; ");
			builder.Append("window.onunload = WindowUnload; ");
			builder.Append("");
			builder.Append("</script>");
			Page.RegisterClientScriptBlock("GeneralCode", builder.ToString());
			
			builder=new System.Text.StringBuilder();
			builder.Append("<script type=\"text/javascript\" language=\"javascript\">");
			builder.Append("var String_Grid=new Array(); ");
			builder.Append("String_Grid[\"confirmDeletePendingTeams\"] =  \"?קבוצות אלו יימחקו מסל הרישום. האם להמשיך\"; ");
			builder.Append("String_Grid[\"confirmDeletePendingPlayers\"] =  \"?שחקנים אלו יימחקו מסל הרישום. האם להמשיך\"; ");
			builder.Append("String_Grid[\"ErrorNoValue\"] =  \"אנא הכנס ערך\"; ");
			builder.Append("String_Grid[\"ErrorOutOfRange\"] =  \"מספר לא חוקי\"; ");
			builder.Append("</script>");
			Page.RegisterClientScriptBlock("String_Grid", builder.ToString());
		}
		
		/// <summary>
		/// add all javascript code to invoke the school selection dialog and
		/// get its result.
		/// </summary>
		private void AddSelectSchoolJavascript()
		{
			string regionID="";
			string cityID="";
			//extract region and city from school, if defined:
			if (Tools.CStrDef(SportData["user_school"], "").Length > 0)
			{
				Entity school=Sport.Entities.School.Type.Lookup(Int32.Parse(SportData["user_school"]));
				regionID = school.Fields[(int) Sport.Entities.School.Fields.Region].ToString();
				cityID = school.Fields[(int) Sport.Entities.School.Fields.City].ToString();
			}
			
			//build querystring to pass to the dialog:
			string queryString="";
			if (regionID.Length > 0)
			{
				queryString += "?region="+regionID+"&city="+cityID;
			}

			//much text, better to use the fast string builder.
			System.Text.StringBuilder builder=new System.Text.StringBuilder();
			builder.Append("<script type=\"text/javascript\" language=\"javascript\">");
			builder.Append("function SelectSchool(event) {");
			builder.Append("   var cursorLeft=event.x;");
			builder.Append("   if (typeof cursorLeft == \"undefined\")");
			builder.Append("      cursorLeft = event.pageX;");
			builder.Append("   var cursorTop=event.y;");
			builder.Append("   if (typeof cursorTop == \"undefined\")");
			builder.Append("      cursorTop = event.pageY;");
			builder.Append("   selectSchoolDlg = window.open('");
			builder.Append(SportSite.Common.Data.AppPath+"/Dialogs/SchoolSelection.aspx"+queryString+"'");
			builder.Append(", 'SelectSchool', 'left='+(cursorLeft+10)+',top='+(cursorTop+10)+',toolbar=no,scrollbars=no,addressbar=no');");
			builder.Append("   selectSchoolDlg.focus();");
			builder.Append("   timer1 = setTimeout(\"Timer()\", 500);");
			builder.Append("} ");
			builder.Append("function Timer(event) {");
			builder.Append("   if (selectSchoolDlg) {");
			builder.Append("      try {");
			builder.Append("         var modalResult=selectSchoolDlg.ModalResult;");
			builder.Append("      }");
			builder.Append("      catch (e) {");
			builder.Append("         selectSchoolDlg = 0;");
			builder.Append("         clearTimeout(timer1);");
			builder.Append("         return false;");
			builder.Append("      }");
			builder.Append("      if (typeof modalResult != \"undefined\") {");
			builder.Append("         switch (modalResult) {");
			builder.Append("            case "+((int) SportSite.Common.ModalResults.OK).ToString()+": ");
			builder.Append("               var schoolName=selectSchoolDlg.SelectedSchoolName;");
			builder.Append("               var schoolID=selectSchoolDlg.SelectedSchoolID;");
			builder.Append("               var container=document.getElementById(\"UserSchool\");");
			builder.Append("               container.innerHTML = '<b>'+schoolName+'</b>';");
			builder.Append("               document.forms[0].elements['"+userSchoolID.UniqueID+"'].value = schoolID;");
			builder.Append("               selectSchoolDlg.close();");
			builder.Append("               document.location = \"?"+userSchoolID.UniqueID+"=\"+schoolID;");
			//builder.Append("               document.forms[0].submit();");
			builder.Append("               break;");
			builder.Append("            case "+((int) SportSite.Common.ModalResults.Cancel).ToString()+": ");
			builder.Append("               selectSchoolDlg.close();");
			builder.Append("               break;");
			builder.Append("         }");
			builder.Append("      }");
			builder.Append("   }");
			builder.Append("   timer1 = setTimeout(\"Timer()\", 200);");
			builder.Append("} ");
			builder.Append("</script>");
			Page.RegisterClientScriptBlock("SelectSchool", builder.ToString());
		}
		
		private void AddSelectPlayersJavascript()
		{
			//add only if we have valid team ID
			string teamID=Tools.CStrDef(SportData["TeamID"], "");
			if (teamID.Length == 0) 
			{
				return;
			}
			
			//get info:
			Entity team=Sport.Entities.Team.Type.Lookup(Int32.Parse(teamID));
			string schoolID=team.Fields[(int) Sport.Entities.Team.Fields.School].ToString();
			string categoryID=team.Fields[(int) Sport.Entities.Team.Fields.Category].ToString();
			//build querystring to pass to the dialog:
			string queryString="?school="+schoolID+"&category="+categoryID+"&team="+teamID;
			
			//much text, better to use the fast string builder.
			System.Text.StringBuilder builder=new System.Text.StringBuilder();
			builder.Append("<script type=\"text/javascript\" language=\"javascript\">");
			builder.Append("function SelectPlayers(event) {");
			builder.Append("   var cursorLeft=event.x;");
			builder.Append("   if (typeof cursorLeft == \"undefined\")");
			builder.Append("      cursorLeft = event.pageX;");
			builder.Append("   var cursorTop=event.y;");
			builder.Append("   if (typeof cursorTop == \"undefined\")");
			builder.Append("      cursorTop = event.pageY;");
			builder.Append("   selectPlayersDlg = window.open('");
			builder.Append(SportSite.Common.Data.AppPath+"/Dialogs/PlayerSelection.aspx"+queryString+"'");
			builder.Append(", 'SelectPlayer', 'left='+(cursorLeft+10)+',top='+(cursorTop-300)+',toolbar=no,scrollbars=no,addressbar=no,resizable=yes');");
			builder.Append("   selectPlayersDlg.focus();");
			builder.Append("   timer2 = setTimeout(\"Timer2()\", 500);");
			builder.Append("} ");
			builder.Append("function Timer2(event) {");
			builder.Append("   if (selectPlayersDlg) {");
			builder.Append("      try {");
			builder.Append("         var modalResult=selectPlayersDlg.ModalResult;");
			builder.Append("      }");
			builder.Append("      catch (e) {");
			builder.Append("         selectPlayersDlg = 0;");
			builder.Append("         clearTimeout(timer2);");
			builder.Append("         return false;");
			builder.Append("      }");
			builder.Append("      if (typeof modalResult != \"undefined\") {");
			builder.Append("         switch (modalResult) {");
			builder.Append("            case "+((int) SportSite.Common.ModalResults.OK).ToString()+": ");
			builder.Append("               var arrStudents=selectPlayersDlg.SelectedPlayers;");
			builder.Append("               var strStudents = ArrayJoin(arrStudents, ',', true);");
			builder.Append("               selectPlayersDlg.close();");
			builder.Append("               document.location = \"?category="+Request.QueryString["category"]+"&action="+SportSiteAction.AddPlayers.ToString()+"&"+PlayerToAddString+"=\"+strStudents+\"&team="+SportData["TeamID"]+"\";");
			builder.Append("               break;");
			builder.Append("            case "+((int) SportSite.Common.ModalResults.Cancel).ToString()+": ");
			builder.Append("               selectPlayersDlg.close();");
			builder.Append("               break;");
			builder.Append("         }");
			builder.Append("      }");
			builder.Append("   }");
			builder.Append("   timer2 = setTimeout(\"Timer2()\", 200);");
			builder.Append("} ");
			builder.Append("</script>");
			Page.RegisterClientScriptBlock("SelectPlayer", builder.ToString());
		}
		#endregion

		#region messages
		private void MarkMessageRead()
		{
			//get message id:
			int messageID=Tools.CIntDef(Request.Form["MessageID"], -1);
			
			if (messageID <= 0)
			{
				AddErrorMessage("לא ניתן לסמן הודעה כנקראה, זיהוי הודעה לא חוקי: "+Request.Form["MessageID"]);
				return;
			}
			
			//create instance of the serive:
			SportSite.RegistrationService.RegistrationService regService=new SportSite.RegistrationService.RegistrationService();
			regService.CookieContainer = (System.Net.CookieContainer) Session["cookies"];

			//mark the message as read:
			regService.MarkMessageRead(messageID);
			
			ArrayList arrMarked=(ArrayList) Session["marked_messages"];
			if (arrMarked == null)
				arrMarked = new ArrayList();
			arrMarked.Add(messageID);
			Session["marked_messages"] = arrMarked;

			Sport.Entities.Message.Type.Reset(null);
		}
		
		/// <summary>
		/// load the grid with all new messages for logged in user, or all messages if desired.
		/// </summary>
		private void DisplayMessages(bool newMessagesOnly)
		{
			if (Tools.CStrDef(_user.Login, "").Length == 0)
				throw new Exception("DisplayMessages: user not logged in or session expired.");
			
			//add the table view.
			Panel objPanel=new Panel();
			
			//initialize TableView component as Message table view:
			MessageTableView = new TableView();
			MessageTableView.EntityTypeName = Sport.Entities.Message.TypeName;
			MessageTableView.EntitiesToIgnore = (ArrayList) Session["marked_messages"];
			MessageTableView.TableViewCaption = (newMessagesOnly)?"הודעות חדשות":"הודעות מערכת";
			MessageTableView.NoValuesText = (newMessagesOnly)?"אין הודעות חדשות עבורך":"אין הודעות מערכת";
			MessageTableView.GridViewHeight = 250;
			
			//define row click command to mark message as read:
			string rowCommand="document.forms[0].elements['action'].value = '";
			rowCommand += SportSiteAction.MarkMessageRead.ToString()+"'; ";
			rowCommand +="document.forms[0].elements['MessageID'].value = '%id'; ";
			rowCommand += "document.forms[0].submit();";
			MessageTableView.RowClickCommand = rowCommand;
			MessageTableView.RowTooltip = "לחץ כדי לסמן הודעה כנקראה";
			
			//define view fields, add the status field only if not new messages only:
			if (!newMessagesOnly)
			{
				MessageTableView.ViewFields += new ViewField((int) Sport.Entities.Message.Fields.Status, "סטטוס");
			}
			MessageTableView.ViewFields += new ViewField((int) Sport.Entities.Message.Fields.Text, "פרטים");
			MessageTableView.ViewFields += new ViewField((int) Sport.Entities.Message.Fields.TimeSent, "זמן שליחת הודעה");
			
			//define desired filter:
			EntityType messageType=Sport.Entities.Message.Type;
			MessageTableView.Filter = new EntityFilter(new EntityFilterField(
				(int) Sport.Entities.Message.Fields.User, _user.Id));
			if (newMessagesOnly)
			{
				MessageTableView.Filter.Add(new EntityFilterField(
					(int) Sport.Entities.Message.Fields.Status, (int) Sport.Types.MessageStatus.New));
			}
			
			objPanel.Controls.Add(MessageTableView);
			string strHtml="<span id=\"MessageClickTip\" dir=\"rtl\">לחץ על הודעה על מנת לסמנה כנקראה.</span><br />";
			IsfMainView.AddContents(strHtml);
			IsfMainView.AddContents(objPanel);
			IsfMainView.AddContents("<hr />");
		} //end function DisplayMessages
		#endregion

		#region Registration Methods
		#region events report
		private void ShowEventsReport()
		{
			_pageCaption = "דו\"ח אירועים";
			
			int schoolID=_user.School.ID;
			if (schoolID < 0)
			{
				AddErrorMessage("לא מוגדר בית ספר עבורך, לא ניתן להשתמש בעמוד זה");
				return;
			}
			
			System.Text.StringBuilder strJS=new System.Text.StringBuilder();
			strJS.Append("<script type=\"text/javascript\">");
			strJS.Append(" var _serverURL=\""+Tools.GetFullURL(this.Request)+"\";");
			strJS.Append(" var _region=\""+_user.Region.ID+"\";");
			strJS.Append("</script>");
			strJS.Append("<script type=\"text/javascript\" src=\""+Data.AppPath+
				"/Common/Shadow_AJAX.js\"></script>");
			strJS.Append("<script type=\"text/javascript\" src=\""+Data.AppPath+
				"/Common/VisitorActions.js\"></script>");
			strJS.Append("<script type=\"text/javascript\">");
			strJS.Append(" var _caption_print=\"הדפס\";");
			strJS.Append(" var _caption_cancel=\"ביטול\";");
			strJS.Append("</script>");
			Page.RegisterClientScriptBlock("event_reports_js", strJS.ToString());
			
			//got filter data?
			if (Request.Form["start_year"] != null)
			{
				Session["er_start"] = GetDateSelectionDef("start", DateTime.Now);
				Session["er_end"] = GetDateSelectionDef("end", DateTime.Now);
				Session["er_sport"] = Tools.CIntDef(Request.Form["sport"], -1);
				Session["er_champ"] = Tools.CIntDef(Request.Form["championship"], -1);
				Session["er_category"] = Tools.CIntDef(Request.Form["category"], -1);
				Session["er_facility"] = Tools.CIntDef(Request.Form["facility"], -1);
			}
			
			//define filters:
			DateTime start=Sport.Common.Tools.CDateTimeDef(Session["er_start"], DateTime.Now);
			DateTime end=Sport.Common.Tools.CDateTimeDef(Session["er_end"], DateTime.Now);
			int region=_user.Region.ID;
			int sport=Tools.CIntDef(Session["er_sport"], -1);
			int championship=Tools.CIntDef(Session["er_champ"], -1);
			int category=Tools.CIntDef(Session["er_category"], -1);
			int facility=Tools.CIntDef(Session["er_facility"], -1);
			int school=_user.School.ID;
			
			//read all events for given region, sport and championship in given time range:
			start = Sport.Common.Tools.SetTime(start, 0, 0, 0);
			end = Sport.Common.Tools.SetTime(end, 23, 59, 59);
			Sport.Championships.Event[] events = null;
			string strErrorMessage="";
			try
			{
				events = Sport.Championships.Events.GetEventsInRange(start, end, region, sport, championship, 
					category, facility, -1, school);
			}
			catch (Exception ex)
			{
				strErrorMessage = ex.Message+"<!-- stack trace: "+ex.StackTrace+" -->";
			}
			
			//build HTML:
			System.Text.StringBuilder sb=new System.Text.StringBuilder();
			sb.Append("<div dir=\"rtl\" align=\"right\">");
			if (strErrorMessage.Length > 0)
				sb.Append("שגיאה בעת קריאת נתונים: "+strErrorMessage+"<br />");
			sb.Append("<fieldset style=\"width: 500px;\"><legend>חיתוך</legend>");
			sb.Append("תאריך התחלה: "+BuildDateSelection("start", start.Day, start.Month, start.Year)+"<br />");
			sb.Append("תאריך סיום: "+BuildDateSelection("end", end.Day, end.Month, end.Year)+"<br />");
			sb.Append("ענף ספורט: "+Tools.GetSportsCombo(sport)+"<br />");
			sb.Append("אליפות: "+Tools.GetChampionshipCombo(sport, championship)+"<br />");
			sb.Append("קטגוריה: "+Tools.GetChampCategoryCombo(championship, category)+"<br />");
			sb.Append(Tools.BuildSubmitButton(""));
			sb.Append("</fieldset>");
			if (Page.IsPostBack)
			{
				sb.Append(Tools.BuildPageSubCaption("משחקים אשר נמצאו עבור חיתוך זה:", this.Server, IsfMainView.clientSide));
				if (events != null)
				{
					ArrayList arrTemp=new ArrayList(events);
					arrTemp.Sort(new Sport.Championships.Events.EventDateComparer(false));
					events = (Sport.Championships.Event[])
						arrTemp.ToArray(typeof(Sport.Championships.Event));
					sb.Append(BuildEventReportsTable(events));
				}
				else
				{
					sb.Append("לא נמצאו משחקים בטווח זמן נבחר");
				}
			}
			sb.Append("<br /><br />");
			sb.Append("</div>");
			
			//done.
			IsfMainView.AddContents(sb.ToString());
		}
		
		private string BuildEventReportsTable(Sport.Championships.Event[] events)
		{
			//captions:
			string[] captions=new string[] {"#", "תאריך", "יום", "שעה", "אליפות", 
				"קטגוריה", "שלב", "בית", "קבוצות מתמודדות", "מקום"};
			
			//initialize hash table for the week days:
			Hashtable hebWeekDays=new Hashtable();
			
			//iterate over events, add one row for each event:
			ArrayList rows=new ArrayList();
			int row=0;
			foreach (Sport.Championships.Event curEvent in events)
			{
				//get data for the current event:
				DateTime curDate=curEvent.Date;
				string strCurDate="";
				string strCurDay="";
				string strCurHour="";
				if (curDate.Year > 1900)
				{
					DateTime date=Tools.GetOnlyDate(curDate);
					if (hebWeekDays[date] == null)
						hebWeekDays[date] = Sport.Common.Tools.GetHebDayOfWeek(date);
					strCurDate = curDate.ToString("dd/MM/yyyy");
					strCurHour = curDate.ToString("HH:mm");
					strCurDay = hebWeekDays[date].ToString();
				}
				
				//build array of cells for the current event:
				ArrayList arrCells=new ArrayList();
				arrCells.Add((row+1).ToString());
				arrCells.Add(strCurDate);
				arrCells.Add(strCurDay);
				arrCells.Add(strCurHour);
				arrCells.Add(curEvent.ChampCategory.Championship.Name);
				arrCells.Add(curEvent.ChampCategory.Name);
				arrCells.Add(curEvent.PhaseName);
				arrCells.Add(curEvent.GroupName);
				arrCells.Add(curEvent.GetTeamsOrCompetition());
				arrCells.Add(curEvent.GetPlace());
				//arrCells.Add(curEvent.Supervisor);
				//arrCells.Add(curEvent.Referee);
				rows.Add((string[]) arrCells.ToArray(typeof(string)));
				row++;
			}
			string strBasicStyle="<style type=\"text/css\" media=\"%media\">"+
				"div.ShowPrintOnly {display: %display;}</style>";
			string strStyleHTML=strBasicStyle.Replace("%media", "screen").
				Replace("%display", "none");
			strStyleHTML += strBasicStyle.Replace("%media", "print").
				Replace("%display", "block");
			Page.RegisterClientScriptBlock("game_report_style", strStyleHTML);
			System.Text.StringBuilder sb=new System.Text.StringBuilder();
			sb.Append("<div id=\"EventReportsPanel\">");
			sb.Append("<div class=\"ShowPrintOnly\">");
			sb.Append("<h2>דו\"ח אירועים</h2>");
			sb.Append("<span id=\"EventReportsDetails\"></span>");
			sb.Append("</div>");
			IsfMainView.clientSide.AddOnloadCommand("ApplyReportDetails()", true);
			sb.Append(Tools.BuildChampTable(captions, rows));
			sb.Append("</div>");
			sb.Append("<a href=\"javascript: void(0);\" "+
				"onclick=\"PrintElement('EventReportsPanel', 'דו~ח אירועים');\">");
			sb.Append("<img src=\""+Common.Data.AppPath+"/Images/btn_print.jpg\" "+
				"alt=\"Print Button\" /></a>");
			sb.Append("<img src=\""+Common.Data.AppPath+"/Images/small_seperator.jpg\" />");
			sb.Append("<br />");
			return sb.ToString();
		}
		#endregion
		
		#region club register
		/// <summary>
		/// let school manager register as club and order teams.
		/// </summary>
		private void ClubRegister()
		{
			_pageCaption = "רישום מועדון";
			
			int schoolID=_user.School.ID;
			if (schoolID < 0)
			{
				AddErrorMessage("לא מוגדר בית ספר עבורך, לא ניתן להשתמש בעמוד זה");
				return;
			}
			
			Sport.Entities.School school=new School(schoolID);
			string strSchoolName=school.Entity.Fields[(int) Sport.Entities.School.Fields.Name].ToString();
			string strSchoolManager=Tools.CStrDef(school.ManagerName, "");
			string strSchoolSymbol=school.Symbol;
			string strSchoolAddress=school.Address;
			string strSchoolCity="";
			if (school.City != null)
				strSchoolCity = school.City.Name;
			string strSchoolZipCode=school.ZipCode;
			string strSchoolPhone=school.Phone;
			string strSchoolFax=school.Fax;
			string strSchoolEmail=school.Email;
			Sport.Entities.Functionary coordinator=school.GetCoordinator();
			string strCoordinatorName=(coordinator == null)?"":coordinator.Name;
			string strCoordinatorAddress=(coordinator == null)?"":coordinator.Address;
			string strCoordinatorZipCode=(coordinator == null)?"":coordinator.ZipCode;
			string strCoordinatorPhone=(coordinator == null)?"":coordinator.Phone;
			string strCoordinatorFax=(coordinator == null)?"":coordinator.Fax;
			string strCoordinatorCellPhone=(coordinator == null)?"":coordinator.CellPhone;
			string strCoordinatorEmail=(coordinator == null)?"":coordinator.Email;
			System.Text.StringBuilder sb=new System.Text.StringBuilder();
			sb.Append("<div dir=\"rtl\">");
			
			//maybe got order?
			int clubRegisterStep=Tools.CIntDef(Request.Form["club_register"], 0);
			if (clubRegisterStep > 0)
			{
				bool canContinue=false;
				sb.Append(PerformClubRegister(clubRegisterStep, school, coordinator, 
					ref canContinue));
				if (canContinue == false)
				{
					sb.Append("</div>");
					AddViewText(sb.ToString(), true);
					return;
				}
			}
			
			string[] arrLines=Tools.ReadTextFile(Server.MapPath("ClubRegister.txt"), true);
			string strHTML="<br />";
			if (arrLines != null)
				strHTML += String.Join("<br />", arrLines)+"<br />";
			sb.Append(strHTML);
			sb.Append("<div id=\"club_details_form\">");
			sb.Append(Tools.BuildCaption("טופס הרשמת מועדון ספורט בית-ספרי"));
			
			sb.Append("<input type=\"hidden\" name=\"club_register\" value=\"1\" />");
			sb.Append("<p><i>"+((school.ClubStatus == 0)?"הרשמה ראשונה":"חידוש הרשמה")+"</i></p>");
			
			sb.Append("<p>");
			sb.Append("<fieldset style=\"width: 400px;\"><legend>פרטי בית הספר</legend>");
			sb.Append(Tools.BuildHtmlTable(0, new ArrayList(
				new object[] {
					new object[] {"<u>שם בית הספר:</u>", strSchoolName},
					new object[] {"<u>שם מנהל בית הספר:</u>", Common.FastControls.InputBox("school_manager", strSchoolManager, 30)}, 
					new object[] {"<u>מספר סימול בית ספר:</u>", "<b>"+strSchoolSymbol+"</b>"}, 
					new object[] {"<u>כתובת בית הספר:</u>", strSchoolAddress}, 
					new object[] {"<u>עיר:</u>", strSchoolCity}, 
					new object[] {"<u>מיקוד:</u>", strSchoolZipCode}, 
					new object[] {"<u>טלפון:</u>", Common.FastControls.InputBox("school_phone", strSchoolPhone, 11)}, 
					new object[] {"<u>פקס:</u>", Common.FastControls.InputBox("school_fax", strSchoolFax, 11)}, 
					new object[] {"<u>אימייל:</u>", Common.FastControls.InputBox("school_email", strSchoolEmail, 30)}
				 }), "right"));
			sb.Append("</fieldset>");
			sb.Append("</p>");
			
			sb.Append("<p>");
			sb.Append("<fieldset style=\"width: 400px;\"><legend>רכז מועדון בית ספרי</legend>");
			sb.Append(Tools.BuildHtmlTable(0, new ArrayList(
				new object[] {
					 new object[] {"<u>שם רכז מועדון הבית ספרי:</u>", Common.FastControls.InputBox("coordinator_name", strCoordinatorName, 50, 20)},
					 new object[] {"<u>כתובת פרטית:</u>", Common.FastControls.InputBox("coordinator_address", strCoordinatorAddress, 70, 30)}, 
					 new object[] {"<u>מיקוד:</u>", Common.FastControls.InputBox("coordinator_zipcode", strCoordinatorZipCode, 15, 5)}, 
					 new object[] {"<u>טלפון:</u>", Common.FastControls.InputBox("coordinator_phone", strCoordinatorPhone, 15, 11)}, 
					 new object[] {"<u>פקס:</u>", Common.FastControls.InputBox("coordinator_fax", strCoordinatorFax, 15, 11)}, 
					 new object[] {"<u>נייד:</u>", Common.FastControls.InputBox("coordinator_cellphone", strCoordinatorCellPhone, 15, 11)}, 
					 new object[] {"<u>אימייל:</u>", Common.FastControls.InputBox("coordinator_email", strCoordinatorEmail, 100, 30)}
				 }), "right"));
			sb.Append("</fieldset>");
			sb.Append("</p>");
			
			sb.Append("<p>הרינו מבקשים בזאת להקים מועדון ספורט בית ספרי ולהפעילו "+
				"במסגרת התאחדות הספורט לבתי-הספר בישראל.</p>");
			
			sb.Append("<p>אנו מצהירים בזאת על התחייבותנו לפעול על פי עקרונות ותקנון "+
				"התאחדות הספורט לבתי הספר בישראל כפי שהם יעודכנו מעת לעת ויופיעו "+
				"במסמך המידע ועקרונות הפעולה.</p>");
			sb.Append("<div class=\"printable\">"+Tools.GetSchoolSignatureHtml()+"</div>");
			sb.Append("</div>");
			sb.Append(Tools.BuildVisitorActionsPanel("club_details_form", 
				this.Server, IsfMainView.clientSide));
			
			sb.Append("<div id=\"team_register_form\">");
			sb.Append(Tools.BuildCaption("טופס הרשמת קבוצות תחרותיות "));
			
			sb.Append("<p>למדנו את העקרונות שנקבעו להפעלת המועדון ובהתאם לכך אנו מבקשים "+
				"לרשום את הקבוצות הבאות למסגרת התחרותית במועדוני הספורט הבית ספריים "+
				"של התאחדות הספורט לבתיה\"ס.</p>");
			
			sb.Append(BuildClubsChampionshipsTable());
			sb.Append("<div class=\"printable\">"+Tools.GetSchoolSignatureHtml()+"</div>");
			sb.Append("</div>");
			sb.Append(Tools.BuildVisitorActionsPanel("team_register_form", 
				this.Server, IsfMainView.clientSide));
			
			sb.Append("<br />"+Tools.BuildSubmitButton(null)+"<br />");
			//<button type=\"submit\">שלח בקשת רישום מועדון</button>
			
			sb.Append("</div>");
			AddViewText(sb.ToString(), true);
		}
		
		private string PerformClubRegister(int registerStep, Sport.Entities.School school, 
			Sport.Entities.Functionary coordinator, ref bool canContinue)
		{
			//build the result string:
			System.Text.StringBuilder sb=new System.Text.StringBuilder();
			
			//unless there will be something wrong, user can't continue:
			canContinue = false;
			
			//compare current school details with new school details:
			ArrayList arrHTML=new ArrayList();
			Sport.Data.EntityEdit entEdit=school.Entity.Edit();
			arrHTML.AddRange(CheckClubRegisterChange(registerStep, school, entEdit, 
						"שם מנהל/ת בית ספר", "school_manager", 
						Sport.Entities.School.Fields.ManagerName));
			arrHTML.AddRange(CheckClubRegisterChange(registerStep, school, entEdit, 
				"טלפון בית ספר", "school_phone", 
				Sport.Entities.School.Fields.Phone));
			arrHTML.AddRange(CheckClubRegisterChange(registerStep, school, entEdit, 
				"פקס בית ספר", "school_fax", 
				Sport.Entities.School.Fields.Fax));
			arrHTML.AddRange(CheckClubRegisterChange(registerStep, school, entEdit, 
				"אימייל בית ספר", "school_email", 
				Sport.Entities.School.Fields.Email));
			
			//comapre current coordinator details with new details:
			arrHTML.AddRange(CheckClubRegisterChange(registerStep, school, coordinator, 
				new string[] {"שם", "כתובת","טלפון", "פקס", "מיקוד", "אימייל", "טלפון נייד"}, 
				new string[] {"coordinator_name", "coordinator_address", "coordinator_phone", 
					"coordinator_fax", "coordinator_zipcode", 
					"coordinator_email", "coordinator_cellphone"
				 }, 
				new Sport.Entities.Functionary.Fields[] {Sport.Entities.Functionary.Fields.Name, 
					Sport.Entities.Functionary.Fields.Address, Sport.Entities.Functionary.Fields.Phone, 
					Sport.Entities.Functionary.Fields.Fax, Sport.Entities.Functionary.Fields.ZipCode, 
					Sport.Entities.Functionary.Fields.Email, Sport.Entities.Functionary.Fields.CellPhone
				}));
			
			//anything different?
			int diffCount=arrHTML.Count;
			if (registerStep == 1)
				diffCount = diffCount-11;
			if (diffCount > 0)
			{
				switch (registerStep)
				{
					case 1:
						sb.Append("<u>אנא אשר את השינויים הבאים בפרטי בית הספר:</u><br />");
						break;
					case 2:
						System.Net.CookieContainer originalCookies=Sport.Core.Session.Cookies;
						Sport.Core.Session.Cookies = (System.Net.CookieContainer) Session["cookies"];
						entEdit.Save();
						Sport.Core.Session.Cookies = originalCookies;
						break;
				}
			}
			for (int i=0; i<arrHTML.Count; i++)
				sb.Append(arrHTML[i]);
			
			//check ordered teams:
			NameValueCollection arrTeamAmounts=GetClubOrderedTeams();
			diffCount += arrTeamAmounts.Count;
			if (arrTeamAmounts.Count > 0)
			{
				string strAllCategories="";
				
				//build the string for all categories:
				for (int i=0; i<arrTeamAmounts.AllKeys.Length; i++)
				{
					string strCategoryID=arrTeamAmounts.AllKeys[i];
					int categoryID = Int32.Parse(strCategoryID);
					int amount=Int32.Parse(arrTeamAmounts[strCategoryID]);
					if (registerStep == 1)
					{
						sb.Append("<input type=\"hidden\" name=\"amount_"+
							strCategoryID+"\" value=\""+amount+"\" />");
					}
					strAllCategories += strCategoryID+"|"+amount.ToString();
					if (i < (arrTeamAmounts.AllKeys.Length-1))
						strAllCategories += ",";
				}
				
				//get the team names without actually adding them if not needed yet
				SportData["category"] = strAllCategories;
				string[] arrTeamNames=AddTeam(true, (registerStep == 2));
				
				//reset:
				SportData["category"] = null;
				
				//commit pending teams?
				if (registerStep == 2)
				{
					//initialize service:
					SportSite.RegistrationService.RegistrationService regService=new SportSite.RegistrationService.RegistrationService();
					regService.CookieContainer = (System.Net.CookieContainer) Session["cookies"];
					
					//commit data:
					if (regService.CommitPendingTeams(school.Id) == true)
					{
						sb.Append("<u>הקבוצות הבאות הוזמנו בהצלחה:</u><br />");
					}
					else
					{
						sb.Append("כישלון בהזמנת קבוצות, אנא פנה אל רכז המחוז<br />");
						arrTeamNames = new string[] {};
					}
				}
				else
				{
					if (registerStep == 1)
						sb.Append("<u>אנא אשר את הזמנת הקבוצות הבאות:</u><br />");
				}
				
				if (arrTeamNames.Length > 0)
					sb.Append(String.Join("<br />", arrTeamNames)+"<br />");
			} //end if arrTeamAmounts contain items.
			
			//alert of the charge:
			double totalCharge=0;
			if (registerStep == 1)
			{
				arrHTML.Clear();
				
				//club register charge:
				if (school.ClubStatus == 0)
				{
					double curCharge=0;
					Sport.Entities.Product product=null;
					try
					{
						product = new Sport.Entities.Product(
							(int) Sport.Entities.Product.BasicProducts.ClubRegister);
					}
					catch {}
					if (product != null)
						curCharge = product.Price;
					arrHTML.Add("<u>רישום מועדון חדש:</u> "+curCharge+" ש\"ח<br />");
					totalCharge += curCharge;
				}
				
				//team register charges:
				if (arrTeamAmounts.Count > 0)
				{
					foreach (string strCategoryID in arrTeamAmounts.AllKeys)
					{
						int categoryID = Int32.Parse(strCategoryID);
						int amount=Int32.Parse(arrTeamAmounts[strCategoryID]);
						ChampionshipCategory category=null;
						try
						{
							category = new ChampionshipCategory(categoryID);
						}
						catch {}
						if (category == null)
							continue;
						double curCharge=(category.RegistrationPrice*amount);
						if (curCharge > 0)
						{
							arrHTML.Add("רישום "+
								Sport.Common.Tools.BuildOneOrMany("קבוצה", "קבוצות", amount, false)+
								" ל"+category.Championship.Name+": "+curCharge+" ש\"ח<br />");
						}
						totalCharge += curCharge;
					}
				}
			}
			
			//any charge present?
			if (totalCharge > 0)
			{
				sb.Append("<br /><div style=\"border: 3px solid black; width: 450px;\">");
				sb.Append("<u><b>דמי רישום</b></u><br />");
				for (int i=0; i<arrHTML.Count; i++)
					sb.Append(arrHTML[i]);
				sb.Append("<u>סה\"כ דמי רישום:</u> <b>"+totalCharge+" ש\"ח</b>");
				sb.Append("</div><br />");
			}
			
			if (diffCount == 0)
			{
				sb.Append("אין שינוי פרטים ולא הוזמנו קבוצות<br /><br />");
				canContinue = true;
			}
			
			if ((canContinue == false)&&(registerStep == 1))
			{
				sb.Append("<input type=\"hidden\" name=\"club_register\" value=\"2\" />");
				sb.Append("<button type=\"submit\">אישור סופי</button>");
			}
			
			return sb.ToString();
		}
		
		private string[] CheckClubRegisterChange(int registerStep, 
			Sport.Entities.School school, Sport.Data.EntityEdit entEdit, 
			string strCaption, string strRequestKey, Sport.Entities.School.Fields schoolField)
		{
			ArrayList result=new ArrayList();
			
			string strOldValue=Tools.CStrDef(school.Entity.Fields[(int) schoolField], "");
			string strNewValue=Tools.CStrDef(Request.Form[strRequestKey], "");
			
			if (registerStep == 1)
				result.Add("<input type=\"hidden\" name=\""+strRequestKey+"\" value=\""+strNewValue+"\" />");
			
			if (strOldValue != strNewValue)
			{
				switch (registerStep)
				{
					case 1:
						result.Add(strCaption+" קודם: "+strOldValue+"<br />");
						result.Add(strCaption+" חדש: "+strNewValue+"<br /><br />");
						break;
					case 2:
						entEdit.Fields[(int) schoolField] = strNewValue;
						result.Add(strCaption+" שונה בהצלחה<br /><br />");
						break;
				}
			}
			return (string[]) result.ToArray(typeof(string));
		}
		
		private string[] CheckClubRegisterChange(int registerStep, 
			Sport.Entities.School school, Sport.Entities.Functionary coordinator, 
			string[] arrCaptions, string[] arrRequestKeys, 
			Sport.Entities.Functionary.Fields[] coordinatorFields)
		{
			ArrayList result=new ArrayList();
			int changedCount=0;
			
			for (int i=0; i<arrCaptions.Length; i++)
			{
				string strCaption=arrCaptions[i]+" רכז מועדון";
				string strRequestKey=arrRequestKeys[i];
				Sport.Entities.Functionary.Fields coordinatorField=coordinatorFields[i];
				
				string strOldValue=(coordinator == null)?"":Tools.CStrDef(coordinator.Entity.Fields[(int) coordinatorField], "");
				string strNewValue=Tools.CStrDef(Request.Form[strRequestKey], "");
				
				if (registerStep == 1)
					result.Add("<input type=\"hidden\" name=\""+strRequestKey+"\" value=\""+strNewValue+"\" />");
			
				if (strOldValue != strNewValue)
				{
					if (registerStep == 1)
					{
						result.Add(strCaption+" קודם: "+strOldValue+"<br />");
						result.Add(strCaption+" חדש: "+strNewValue+"<br /><br />");
					}
					changedCount++;
				}
			} //end loop over arrays
			
			if ((registerStep == 2)&&(changedCount > 0))
			{
				school.UpdateCoordinatorData(Request.Form[arrRequestKeys[0]], 
					Request.Form[arrRequestKeys[1]], Request.Form[arrRequestKeys[2]], 
					Request.Form[arrRequestKeys[3]], Request.Form[arrRequestKeys[4]], 
					Request.Form[arrRequestKeys[5]], Request.Form[arrRequestKeys[6]]);
				result.Add("פרטי רכז מועדון עודכנו בהצלחה<br /><br />");
			}
			
			return (string[]) result.ToArray(typeof(string));
		}
		
		public NameValueCollection GetClubOrderedTeams()
		{
			//initialize result collection:
			NameValueCollection result=new NameValueCollection();
			
			//iterate over all form elements, look for the orders:
			foreach (string key in Request.Form.Keys)
			{
				//got amount?
				if (!key.StartsWith("amount_"))
					continue;
				
				//split by underscore:
				string[] arrTemp=key.Split(new char[] {'_'});
				
				//format must be amount_[category id]
				if (arrTemp.Length != 2)
					continue;
				
				//read the category id:
				int catID=Tools.CIntDef(arrTemp[1], -1);
				
				//check that valid:
				if (catID < 0)
					continue;
				
				//check that it's valid championship category:
				Sport.Entities.ChampionshipCategory category=null;
				try
				{
					category = new Sport.Entities.ChampionshipCategory(catID);
				}
				catch {}
				if (category == null)
					continue;
				
				//read the requested amount:
				int amount=Tools.CIntDef(Request.Form[key], -1);
				if ((amount > 0)&&(amount < 100))
				{
					//order the requested amount of teams to the requested championship
					result[catID.ToString()] = amount.ToString();
				}
			}
			
			return result;
		}
		
		/// <summary>
		/// return the HTML for the table holding the club-only championships
		/// from which school manager can select and order teams.
		/// </summary>
		private string BuildClubsChampionshipsTable()
		{
			System.Text.StringBuilder sb=new System.Text.StringBuilder();
			
			//build the filter for the championships: only in the manager's region,
			//only for clubs and only team register status.
			EntityFilter filter=new EntityFilter();
			filter.Add(new EntityFilterField((int) Sport.Entities.Championship.Fields.Region, _user.Region.ID));
			filter.Add(new EntityFilterField((int) Sport.Entities.Championship.Fields.Status, (int) Sport.Types.ChampionshipType.TeamRegister));
			filter.Add(new EntityFilterField((int) Sport.Entities.Championship.Fields.IsClubs, 1));
			
			//read entities from database:
			Entity[] champs=Sport.Entities.Championship.Type.GetEntities(filter);
			
			//make array of championship categories
			ArrayList arrAllCategories=new ArrayList();
			foreach (Entity champEnt in champs)
			{
				//make championship entity:
				Sport.Entities.Championship champ=new Sport.Entities.Championship(champEnt);
				
				//extract categories:
				Sport.Entities.ChampionshipCategory[] categories=champ.GetCategories();
				
				//iterate and add to array:
				foreach (Sport.Entities.ChampionshipCategory curCategory in categories)
					arrAllCategories.Add(curCategory);
			}
			
			//all the championship categories available to select from:
			ArrayList arrChampCategories=new ArrayList();
			ArrayList arrCatSorted=new ArrayList();
			
			//define possible grades to select from: ז', ז'-ח', ט', י'-י"ב
			//object[] arrPossibleGrades=new object[] {new int[] {6}, new int[] {6, 7}, 
			//											new int[] {8}, new int[] {9, 11}};
			
			//the table headers - available grades will be known only at run time.
			ArrayList arrCaptions=new ArrayList();
			ArrayList arrCellsWidth=new ArrayList();
			
			//list of "valid" categories: (for each grade range: boys and girls)
			ArrayList arrClubCategoriesData=new ArrayList();
			
			//list of the available sports and whether it's boys or girls.
			//defines the table rows.
			ArrayList arrSports=new ArrayList();
			
			//list of sports and grades available:
			ArrayList arrAvailableSportsGrades=new ArrayList();
			
			//amount of matching categories found: can be 0 to the size of arrPossibleGrades
			int catCount=0;
			
			//add the first two headers:
			arrCaptions.Add("&nbsp;");
			arrCaptions.Add("&nbsp;");
			arrCellsWidth.Add("5%");
			arrCellsWidth.Add("13%");
			
			//build the valid categories list according to the available categories:
			foreach (Sport.Entities.ChampionshipCategory champCategory in arrAllCategories)
			{
				//already exists?
				bool blnExists=false;
				foreach (CategoryData catData in arrClubCategoriesData)
				{
					if (catData.category == champCategory.Category)
					{
						blnExists = true;
						break;
					}
				}
				
				//add only if does not exist
				if (!blnExists)
				{
					string strCatName=champCategory.Name;
					for (int i=Sport.Types.CategoryTypeLookup.Sexes.Length-1; i>=0; i--)
					{
						string strCurSex=Sport.Types.CategoryTypeLookup.Sexes[i];
						if (strCurSex != null)
							strCatName = strCatName.Replace(strCurSex, "");
					}
					CategoryData data=new CategoryData();
					data.category = champCategory.Category;
					data.strNoSex = strCatName;
					arrClubCategoriesData.Add(data);
				}
			}
			
			//sort by grade:
			arrClubCategoriesData.Sort(new CategoryGradeComparer());
			
			//iterate over the championships and extract their categories:
			ArrayList arrRemarks=new ArrayList();
			ArrayList arrSportCategories=new ArrayList();
			foreach (Sport.Entities.ChampionshipCategory champCat in arrAllCategories)
			{
				//debug only..
				//Response.Write("category ID: "+champCat.Id+", name: "+champCat.Name);
				
				//get category i.e. range of grades and sex
				int category=champCat.Category;
				
				//Response.Write(", value: "+category);
				
				//look if this category is valid:
				int index=CategoryData.IndexOfCategory(arrClubCategoriesData, category);
				if (index >= 0)
				{
					//the category is valid, add to list:
					arrChampCategories.Add(champCat);
					
					//get only the grades of this category and add to headers:
					string strCatName=(arrClubCategoriesData[index] as CategoryData).strNoSex;
					//Response.Write(" (valid: "+strCatName+")<br />");
					
					/*
					int iCurIndex=arrCategoriesNoSex.IndexOf(strCatName);
					if (iCurIndex < 0)
					{
						iCurIndex = catCount;
						catCount++;
						arrCategoriesNoSex.Add(strCatName);
					}
					*/
					
					//build the sports text:
					string strSport=champCat.Championship.Sport.Name+"<br />";
					strSport += (champCat.Name.IndexOf("תלמידים") >= 0)?"תלמידים":"תלמידות";
					
					CategorySportData catSportData=new CategorySportData();
					catSportData.strNoSex = strCatName;
					catSportData.strSport = strSport;
					catSportData.category = champCat;
					arrSportCategories.Add(catSportData);
					
					//add only if not there yet
					if (arrSports.IndexOf(strSport) < 0)
					{
						arrSports.Add(strSport);
						arrRemarks.Add(champCat.Championship.Remarks);
					}
					
					/*
					//add to available sports and grades list:
					//Response.Write("category: "+champCat.Name+", index: "+noSexIndex+" ("+index+")<br />");
					arrAvailableSportsGrades.Add(strSport+"\n"+iCurIndex);
					//Response.Write(strSport+"<br />"+iCurIndex+"<br /><br />");
					arrCatSorted.Add(champCat);
					*/
				} //end if index is equal or greater than zero.
			} //end loop over championship categories
			
			//got anything?
			if (arrChampCategories.Count == 0)
				return "אין כרגע אליפויות מועדונים במאגר הנתונים";
			
			//sort and add
			arrSportCategories.Sort(new CategorySportComparer());
			string strLastNoSex="";
			for (int i=0; i<arrSportCategories.Count; i++)
			{
				CategorySportData curCatSportData=(CategorySportData) arrSportCategories[i];
				string strCurNoSex=curCatSportData.strNoSex;
				if (strCurNoSex != strLastNoSex)
				{
					arrCaptions.Add(strCurNoSex);
					catCount++;
				}
				arrAvailableSportsGrades.Add(curCatSportData.strSport+"\n"+(catCount-1));
				arrCatSorted.Add(curCatSportData.category);
				strLastNoSex = strCurNoSex;
			}
			//arrCaptions.AddRange(arrCategoriesNoSex);
			
			//add the cells width:
			int cellWidth=(int) (((double) 60)/((double) catCount));
			for (int col=0; col<catCount; col++)
				arrCellsWidth.Add(cellWidth+"%");
			arrCellsWidth.Add("7%");
			arrCellsWidth.Add("15%");
			
			//sort sports by their name:
			//arrSports.Sort();
			
			//add last two headers:
			arrCaptions.Add("סה\"כ");
			arrCaptions.Add("הערות");
			
			//build the table HTML:
			sb.Append("<table class=\"centered_table\" border=\"1\" width=\"100%\" ");
			sb.Append("cellpadding=\"0\" cellspacing=\"0\" auto_calc_sum=\"1\">");
			
			//add table headers:
			sb.Append(Tools.BuildTableRow(arrCaptions, true, arrCellsWidth));
			
			//colors list:
			string[] arrColors=new string[arrCaptions.Count];
			arrColors[0] = arrColors[1] = arrColors[arrColors.Length-1] = arrColors[arrColors.Length-2] = "";
			
			//iterate over the sports and add row for each:
			for (int row=0; row<arrSports.Count; row++)
			{
				//get current sport:
				string strCurSport=arrSports[row].ToString();
				
				//initialize cells list:
				ArrayList arrCells=new ArrayList();
				
				//first - row index
				arrCells.Add((row+1).ToString());
				
				//second - the sport field and sex
				arrCells.Add(strCurSport);
				
				//add textbox for each category available for order
				for (int col=0; col<catCount; col++)
				{
					//available?
					string strCellText="";
					int index = arrAvailableSportsGrades.IndexOf(strCurSport+"\n"+col);
					if (index >= 0)
					{
						Sport.Entities.ChampionshipCategory champCat=
							(Sport.Entities.ChampionshipCategory) arrCatSorted[index];
						int categoryID=champCat.Id;
						strCellText = "<input type=\"text\" name=\"amount_"+categoryID+"\" "+
							"size=\"2\" maxlength=\"2\" style=\"width: 2em;\" />";
						arrColors[col+2] = "";
					}
					else
					{
						strCellText = "";
						arrColors[col+2] = "black";
						//Tools.BuildFilledSquare("black");
					}
					arrCells.Add(strCellText);
				}
				
				//total selection cell:
				arrCells.Add("<span id=\"total_amount_"+(row+1)+"\">0</span>");
				
				//remarks cell:
				arrCells.Add(arrRemarks[row]);
				
				//add current row
				sb.Append(Tools.BuildTableRow(arrCells, false, arrColors));
			} //end loop over available sports
			
			sb.Append("</table>");
			return sb.ToString();
		}
		#endregion
		
		#region Team Registration
		/// <summary>
		/// takes the data from the form collection and add team to pending teams
		/// database table. team index is automatically calculated, taking the 
		/// maximum index and adding one to that index.
		/// </summary>
		private string[] AddTeam(bool silent, bool reallyAdd)
		{
			ArrayList result=new ArrayList();
			
			//assign variables:
			int userid=_user.Id;
			string strAllCategories=Tools.CStrDef(SportData["category"], "");
			string[] arrCategories=strAllCategories.Split(new char[] {','});
			int schoolID=_user.School.ID; //Int32.Parse(SportData["user_school"]);
			
			//can add only for specific school - check if any school defined:
			if (schoolID <= 0)
			{
				if (!silent)
					AddErrorMessage("לא ניתן להוסיף קבוצה: אין בית ספר מוגדר.");
				return null;
			}
			
			Sport.Entities.School school=new Sport.Entities.School(schoolID);
			
			//check that we have categories to add teams to:
			if (arrCategories.Length == 0)
			{
				if (!silent)
					AddErrorMessage("לא נבחרו אליפויות");
				return null;
			}
			
			//initialize service:
			SportSite.RegistrationService.RegistrationService regService=new SportSite.RegistrationService.RegistrationService();
			regService.CookieContainer = (System.Net.CookieContainer) Session["cookies"];
			
			//get list of pending teams for the school:
			SportSite.RegistrationService.TeamData[] arrPendingTeams=regService.GetPendingTeams(schoolID);
			
			//iterate over the categories:
			int successCount=0;
			//Sport.Types.CategoryTypeLookup lookup=new Sport.Types.CategoryTypeLookup();
			for (int i=0; i<arrCategories.Length; i++)
			{
				//split current value:
				string[] arrTemp=arrCategories[i].Split(new char[] {'|'});
				
				//get current category ID:
				int categoryID = Int32.Parse(arrTemp[0]);
				
				//get entity:
				Sport.Entities.ChampionshipCategory category=null;
				try
				{
					category = new ChampionshipCategory(categoryID);
				}
				catch {}
				if ((category == null)||(category.Id < 0)||(category.Championship == null))
					continue;
				
				//get amount of teams to register:
				int amount=1;
				if (arrTemp.Length > 1)
				{
					amount = Int32.Parse(arrTemp[1]);
				}
				
				//get maximum team index (number) and add one to that number:
				int teamIndex=FindMaxTeamIndex(categoryID, schoolID)+1;
				
				//how many pending teams for this championship category we have?
				int pendingTeamsCount=0;
				
				//iterate over the pending teams:
				foreach (SportSite.RegistrationService.TeamData curTeam in arrPendingTeams)
				{
					//check if current team is registered for the same category:
					if (curTeam.championship_category_id == categoryID)
					{
						//increase counter:
						pendingTeamsCount++;
					}
				}
				//add the amount of pending teams to the index:
				teamIndex += pendingTeamsCount;
				
				//add pending team(s):
				for (int j=1; j<=amount; j++)
				{
					//add one team:
					int teamID=0;
					if (reallyAdd)
					{
						teamID=regService.RegisterTeam(userid, schoolID, categoryID, teamIndex);
					}
					
					//success?
					if (teamID > 0)
						successCount++;
					
					string strCurTeam=category.Championship.Name+" "+category.Name+" - "+
						school.Name;
					if (teamIndex >= 1)
					{
						strCurTeam += " "+Sport.Common.Tools.ToHebLetter(teamIndex);
						if ((teamIndex < 10)||((teamIndex % 10) == 0))
							strCurTeam += "'";
					}
					result.Add(strCurTeam);
					
					//increase index:
					teamIndex++;
				}
			} //end loop over categories
			
			if (successCount > 0)
			{
				//success:
				string strTemp="קבוצה נוספה";
				if (successCount > 1)
					strTemp = successCount+" קבוצות נוספו";
				if (!silent)
					AddViewText("<span class=\"BoldBlueText\">"+strTemp+" לסל ההזמנות.</span><br />", true);
				SetBasketData();
			}
			else
			{
				//failure!
				if (!silent)
					AddErrorMessage("הוספת קבוצה נכשלה");
			}
			
			return (string[]) result.ToArray(typeof(string));
		} //end function AddTeam
		
		private string[] AddTeam()
		{
			return AddTeam(false, true);
		}
		/// <summary>
		/// remove given teams from pending teams table.
		/// </summary>
		private void RemovePendingTeams()
		{
			//verify we have value:
			if (Request.Form["selected_teams"] == null)
				return;
			
			string[] arrTeams=Tools.SplitWithEscape(Request.Form["selected_teams"], ',');
			SportSite.RegistrationService.RegistrationService regService=new SportSite.RegistrationService.RegistrationService();
			regService.CookieContainer = (System.Net.CookieContainer) Session["cookies"];
			
			//loop over the selected teams and remove:
			for (int i=0; i<arrTeams.Length; i++)
			{
				//remove current pending team from database:
				int pendingTeamID=Int32.Parse(arrTeams[i]);
				regService.RemovePendingTeam(pendingTeamID);
			}

			//done.
			AddViewSuccessMsg("קבוצות נמחקו ממאגר הנתונים.");
		} //end function RemovePendingTeams
		
		/// <summary>
		/// show the pending teams and players.
		/// </summary>
		private void ShowPendingTeams(bool centralUser, int userSchoolId)
		{
			TableRow objRow;
			
			//build headers:
			string[] arrHeaders=new string[] {"קטגוריית אליפות", "שם אליפות", 
												 "ענף ספורט", "אינדקס קבוצה"};
			if (centralUser) 
			{
				arrHeaders = Tools.AddToArray(arrHeaders, "שם בית ספר");
			}
			arrHeaders = Tools.AddToArray(arrHeaders, "&nbsp;");
			
			//decide amount of columns:
			int colsCount=arrHeaders.Length;
			
			//get list of pending teams:
			SportSite.RegistrationService.RegistrationService regService = 
				new SportSite.RegistrationService.RegistrationService();
			regService.CookieContainer = (System.Net.CookieContainer) Session["cookies"];
			SportSite.RegistrationService.TeamData[] arrPendingTeams = 
				regService.GetPendingTeams(_user.School.ID);
			
			//exit if there are no pending teams:
			if (arrPendingTeams.Length == 0)
				return;
			
			//build html table:
			Table pendingTeamsTable=BuildPendingTable();
			
			//caption:
			string strCaption="קבוצות ממתינות לאישור עבור בית הספר "+_user.School.Name;
			pendingTeamsTable.Rows.Add(PendingCaptionRow(strCaption, colsCount));
			
			//add headers:
			pendingTeamsTable.Rows.Add(PendingHeadersRow(arrHeaders, "selected_teams"));
			
			//data:
			int rowsCount=0;
			for (int i=0; i<arrPendingTeams.Length; i++)
			{
				//initialize objects:
				SportSite.RegistrationService.TeamData pendingTeam=arrPendingTeams[i];
				objRow = new TableRow();
				
				//get information:
				int categoryID=pendingTeam.championship_category_id;
				Entity category=null;
				try
				{
					category = Sport.Entities.ChampionshipCategory.Type.Lookup(categoryID);
				}
				catch
				{
					category = null;
				}
				
				//got anything?
				if (category == null)
					continue;
				
				//get championship:
				int champID=(int) category.Fields[(int) Sport.Entities.ChampionshipCategory.Fields.Championship];
				Entity champ=Sport.Entities.Championship.Type.Lookup(champID);
				
				//get sport:
				int sportID=(int) champ.Fields[(int) Sport.Entities.Championship.Fields.Sport];
				Entity sport=Sport.Entities.Sport.Type.Lookup(sportID);
				
				//add the information:
				objRow.BackColor = SportSite.Common.Style.PendingTeamsColors[rowsCount % SportSite.Common.Style.PendingTeamsColors.Length];
				string css = SportSite.Common.Style.PendingTeamCss;
				
				//championship category:
				objRow.Cells.Add(FastControls.HebTableCell(category.Name, css));
				
				//championship:
				objRow.Cells.Add(FastControls.HebTableCell(champ.Name, css));
				
				//sport:
				objRow.Cells.Add(FastControls.HebTableCell(sport.Name, css));
				
				//team index:
				objRow.Cells.Add(PendingNumberCell(pendingTeam.team_index, css, "team_index", 
					rowsCount, objRow.BackColor, pendingTeam.id));
				
				//school, if central user:
				if (centralUser)
				{
					int schoolID=pendingTeam.school_id;
					Entity school=Sport.Entities.School.Type.Lookup(schoolID);
					objRow.Cells.Add(FastControls.HebTableCell(school.Name, css));
				}
				objRow.Cells.Add(BuildCheckboxCell("selected_teams", 
					arrPendingTeams[i].id.ToString()));

				pendingTeamsTable.Rows.Add(objRow);
				rowsCount++;
			} //end loop over pending teams
			
			//add action buttons, currently delete button:
			Button btnDeleteTeams=new Button();
			btnDeleteTeams.ToolTip = "לחץ כדי למחוק קבוצות מסומנות";
			btnDeleteTeams.Font.Size = new FontUnit(10);
			btnDeleteTeams.Text = "[מחיקת קבוצות מסומנות]";
			objRow=new TableRow();
			TableCell objCell=new TableCell();
			objCell.HorizontalAlign = HorizontalAlign.Center;
			objCell.ColumnSpan = colsCount;
			objCell.Controls.Add(btnDeleteTeams);
			objRow.Cells.Add(objCell);
			objRow.Attributes["id"] = "ActionsRow";
			objRow.Style.Add("display", "none");
			pendingTeamsTable.Rows.Add(objRow);
			btnDeleteTeams.Attributes.Add("onclick", "this.form.elements['action'].value = '"+SportSiteAction.DeletePendingTeams.ToString()+"'; return DeleteTeamsClick(this);");
			
			IsfMainView.AddContents(pendingTeamsTable);
			
			//add the confirmation button only if the user is the manager:
			if (_user.School.ID == userSchoolId)
			{
				Button btnCommitTeams=new Button();
				btnCommitTeams.ToolTip = "לחץ כדי לרשום קבוצות אלו";
				btnCommitTeams.Text = "אישור סופי - רישום קבוצות";
				btnCommitTeams.Attributes.Add("RealName", "btnCommitTeams");
				btnCommitTeams.Style["text-align"] = "center";
				btnCommitTeams.Attributes.Add("onclick", "this.form.elements['action'].value = '"+SportSiteAction.CommitPendingTeams.ToString()+"';");
				IsfMainView.AddContents(btnCommitTeams);
			}
		} //end function ShowPendingTeams
		
		/// <summary>
		/// returns the maximum team index for team with given category and school,
		/// returns 0 in such team does not exist.
		/// </summary>
		private int FindMaxTeamIndex(int category_id, int school_id)
		{
			EntityType teamType=Sport.Entities.Team.Type;
			EntityFilter teamFilter=new EntityFilter((int) Sport.Entities.Team.Fields.Category, category_id);
			teamFilter.Add(new EntityFilterField((int) Sport.Entities.Team.Fields.School, school_id));
			Entity[] teams=teamType.GetEntities(teamFilter);
			int maxIndex=0;
			for (int i=0; i<teams.Length; i++)
			{
				Entity team=teams[i];
				int currentIndex=Common.Tools.CIntDef(team.Fields[(int) Sport.Entities.Team.Fields.Index], -1);
				if (currentIndex > maxIndex)
				{
					maxIndex = currentIndex;
				}
			}
			return maxIndex;
		} //end function FindMaxTeamIndex
		
		/// <summary>
		/// display details for one specific team, as given in the Form collection.
		/// </summary>
		private void ShowTeamDetails()
		{
			//grab form data:
			string teamID=SportData["TeamID"];
			if (teamID == null)
				return;
			
			//extract database information:
			Sport.Entities.Team team=new Sport.Entities.Team(Int32.Parse(teamID));
			if (!team.IsValid())
			{
				SportData["TeamID"] = null;
				return;
			}
			
			Sport.Entities.School school=team.School;
			Sport.Entities.ChampionshipCategory category=team.Category;
			Sport.Entities.Championship champ=category.Championship;
			Sport.Entities.Sport sport=champ.Sport;
			bool teamConfirmed=(team.Status == Sport.Types.TeamStatusType.Confirmed);
			int champStatus=(int) champ.Status;
			
			//hide all other elements:
			if (Request.Form["action"] == SportSiteAction.ShowTeamDetails.ToString())
				IsfMainView.ClearContents();
			
			//display team details:
			IsfMainView.AddContents("<span class=\"SubCaption\">נתוני קבוצה</span>");
			Label objLabel=FastControls.HebTextLabel("<p class=\"HorizontalLine\" ");
			if (SportSite.Common.Style.DefaultFontFamily.Length > 0)
				objLabel.Text += "style=\"font-size: 12px; font-family: "+SportSite.Common.Style.DefaultFontFamily+"\"";
			objLabel.Text += ">";
			objLabel.Text += "בית ספר: "+school.Name+"<br />";
			objLabel.Text += "סמל בית ספר: "+school.Symbol+"<br />";
			objLabel.Text += "אינדקס קבוצה: "+team.Index+"<br />";
			objLabel.Text += "ענף ספורט: "+sport.Name+"<br />";
			objLabel.Text += "אליפות: "+champ.Name+"<br />";
			objLabel.Text += "קטגוריה: "+category.Name;
			//check if can register new players:
			if (teamConfirmed == false)
				objLabel.Text += "<br />(קבוצה לא עברה אישור, לא ניתן להוסיף שחקנים בשלב זה)";
			if ((teamConfirmed)&&(champStatus != ((int) Sport.Types.ChampionshipType.PlayerRegister)))
				objLabel.Text += "<br />(לא ניתן להוסיף שחקנים, אליפות לא בשלב רישום שחקנים)";
			objLabel.Text += "</p>";
			IsfMainView.AddContents(objLabel);
			
			//initialize TableView component as Player table view:
			PlayersTableView=new TableView();
			PlayersTableView.EntityTypeName = Sport.Entities.Player.TypeName;
			PlayersTableView.TableViewCaption = "שחקנים רשומים עבור הקבוצה "+team.Name;
			PlayersTableView.NoValuesText = "אין שחקנים רשומים עבור קבוצה זו";
			PlayersTableView.GridViewHeight = 300;
			
			//define row click command to display the player details:
			string rowCommand="document.forms[0].elements['action'].value = '";
			rowCommand += SportSiteAction.ShowPlayerDetails.ToString()+"'; ";
			rowCommand +="document.forms[0].elements['PlayerID'].value = '%id'; ";
			rowCommand += "document.forms[0].submit();";
			PlayersTableView.RowClickCommand = rowCommand;
			PlayersTableView.RowTooltip = "לחץ כדי לעבור אל מסך פרטי שחקן";
			
			//define view fields:
			PlayersTableView.ViewFields += new ViewField((int) Sport.Entities.Player.Fields.Status, "סטטוס", ViewFieldType.PlayerStatusType);
			PlayersTableView.ViewFields += new ViewField((int) Sport.Entities.Player.Fields.Number, "מספר שחקן");
			PlayersTableView.ViewFields += new ViewField((int) Sport.Entities.Player.Fields.Grade, "כיתה");
			PlayersTableView.ViewFields += new ViewField((int) Sport.Entities.Player.Fields.LastName, "שם משפחה");
			PlayersTableView.ViewFields += new ViewField((int) Sport.Entities.Player.Fields.FirstName, "שם פרטי");
			
			//define desired filter: players registered for the given team.
			EntityType playerType=EntityType.GetEntityType(Sport.Entities.Player.TypeName);
			PlayersTableView.Filter = new EntityFilter(new EntityFilterField((int) Sport.Entities.Player.Fields.Team, teamID));
			
			string strHTML="<button type=\"button\" onclick=\"ShowPlayersRegisterForm('"+team.Id+"');\">";
			strHTML += "טופס רישום קבוצתי</button><br />";
			IsfMainView.AddContents(strHTML);
			
			//add to the main panel:
			IsfMainView.AddContents(PlayersTableView);
			
			//let user add players if possible:
			if ((teamConfirmed)&&(champStatus == ((int) Sport.Types.ChampionshipType.PlayerRegister)))
			{
				IsfMainView.AddContents("<br /><br />");
				ImageButton btnAddPlayers=new ImageButton();
				btnAddPlayers.ID = "BtnAddPlayers";
				btnAddPlayers.ImageUrl = Common.Data.AppPath+"/Images/AddPlayers.gif";
				btnAddPlayers.AlternateText = "הוספת שחקנים";
				btnAddPlayers.Attributes.Add("onclick", "SelectPlayers(event); return false;");
				btnAddPlayers.Style["text-align"] = "center";
				IsfMainView.AddContents(btnAddPlayers);
				IsfMainView.AddContents("<br />");
			}
			
			//divide players to competitions?
			/*
			if (team.Championship.Sport.SportType == Sport.Types.SportType.Competition)
			{
				Sport.Entities.Player[] arrTeamPlayers = team.GetPlayers();
				int confirmedPlayersCount = 0;
				foreach (Sport.Entities.Player curTeamPlayer in arrTeamPlayers)
					if (curTeamPlayer.Status == ((int) Sport.Types.PlayerStatusType.Confirmed))
						confirmedPlayersCount++;
				if (confirmedPlayersCount > 0)
				{
					//SportSiteAction
				}
			}
			*/
			
			//add javascript code:
			AddSelectPlayersJavascript();
			TeamID.Attributes["value"] = Tools.CStrDef(SportData["TeamID"], "");
		} //end function ShowTeamDetails
		
		/// <summary>
		/// load the grid with all registered teams for given category.
		/// category defines the championship, and championship defines the sports type.
		/// allow user to change registered teams or add new teams for that category.
		/// </summary>
		private void DisplayCategoryTeams(int categoryID)
		{
			if (categoryID < 0)
			{
				throw new Exception("DisplayTeam: invalid argument "+categoryID.ToString());
			}
			
			//find the championship:
			Entity category=Sport.Entities.ChampionshipCategory.Type.Lookup(categoryID);
			int champID=(int) category.Fields[(int) Sport.Entities.ChampionshipCategory.Fields.Championship];
			
			//got any data?
			if (Request.Form["ApplyLinkAmitBB"] != null)
			{
				bool blnExists=(Request.Form["LinkAmitBB"] == "1");
				Tools.SetLinkAmitBB(categoryID, blnExists, this.Server, this.Page);
			}
			
			//AmitBB link:
			string strHTML="";
			if (_user.Type == ((int) Sport.Types.UserType.Internal))
			{
				strHTML += "<fieldset style=\"width: 300px;\"><legend>אתר עמית לכדורסל</legend>";
				strHTML += "<input type=\"hidden\" name=\"ApplyLinkAmitBB\" value=\"1\" />";
				strHTML += "<input type=\"checkbox\" name=\"LinkAmitBB\" value=\"1\"";
				if (Tools.IsCategoryLinkedAmitBB(categoryID, this.Server, this.Page))
					strHTML += " checked=\"checked\"";
				strHTML += " />&nbsp;";
				strHTML += "האם להציג קישור לאתר עמית לכדורסל?<br />";
				strHTML += Tools.BuildSubmitButton("");
				strHTML += "</fieldset><br />";
				IsfMainView.AddContents(strHTML);
			}
			
			//check if can add team:
			bool canAddTeam=true;
			Entity champ=Sport.Entities.Championship.Type.Lookup(champID);
			Championship championship=new Championship(champ.Id);
			int champStatus=(int) champ.Fields[(int) Sport.Entities.Championship.Fields.Status];
			int teamRegisterStatus=(int) Sport.Types.ChampionshipType.TeamRegister;
			canAddTeam = ((_user.School.ID > 0)&&(champStatus == teamRegisterStatus)&&((championship.Region.IsNationalRegion())||(Tools.CIntDef(champ.Fields[(int)Sport.Entities.Championship.Fields.IsOpen], -1) != 0)));
			
			//add the table view.
			Panel objPanel=new Panel();
			
			//initialize TableView component as Team table view:
			TeamsTableView=new TableView();
			TeamsTableView.EntityTypeName = Sport.Entities.Team.TypeName;
			TeamsTableView.TableViewCaption = "קבוצות רשומות עבור האליפות "+Tools.GetEntityName(Sport.Entities.Championship.TypeName, champID);
			TeamsTableView.NoValuesText = "אין קבוצות רשומות עבור אליפות זו ";
			TeamsTableView.GridViewHeight = 300;
			
			//define row click command to display the team details:
			string rowCommand="document.forms[0].elements['action'].value = '";
			rowCommand += SportSiteAction.ShowTeamDetails.ToString()+"'; ";
			rowCommand +="document.forms[0].elements['TeamID'].value = '%id'; ";
			rowCommand += "document.forms[0].submit();";
			TeamsTableView.RowClickCommand = rowCommand;
			TeamsTableView.RowTooltip = "לחץ כדי לעבור אל מסך פירוט קבוצה";

			//define view fields:
			TeamsTableView.ViewFields += new ViewField((int) Sport.Entities.Team.Fields.Status, "סטטוס קבוצה", ViewFieldType.TeamStatusType);
			TeamsTableView.ViewFields += new ViewField((int) Sport.Entities.Team.Fields.Supervisor, "אחראי קבוצה", ViewFieldType.Default);
			TeamsTableView.ViewFields += new ViewField((int) Sport.Entities.Team.Fields.Name, "שם קבוצה", ViewFieldType.TeamName);
			//define desired filter: only teams which belong to the user's school, or
			//all teams in case of central user.
			//main filter is the championship category.
			EntityType teamType=EntityType.GetEntityType(Sport.Entities.Team.TypeName);
			TeamsTableView.Filter = new EntityFilter(new EntityFilterField((int) Sport.Entities.Team.Fields.Category, categoryID));
			if (_user.Region.ID != Sport.Entities.Region.CentralRegion)
				TeamsTableView.Filter.Add(new EntityFilterField((int) Sport.Entities.Team.Fields.School, _user.School.ID));
			objPanel.Controls.Add(TeamsTableView);
			
			//add the Add Team panel controls as well:
			IsfMainView.AddContents("לחץ על קבוצה על מנת לעבור למסך רישום שחקנים.");
			IsfMainView.AddContents("<br />");
			
			//assign competition players?
			int schoolID = _user.School.ID;
			if (championship.Sport.SportType == Sport.Types.SportType.Competition)
			{
				ChampionshipCategory champCategory = 
					new ChampionshipCategory(categoryID);
				Sport.Entities.Team[] arrTeams = champCategory.GetTeams();
				bool blnConfirmed = false;
				foreach (Sport.Entities.Team team in arrTeams)
				{
					if (team.School == null)
						continue;
					if (team.Status == Sport.Types.TeamStatusType.Confirmed)
					{
						if ((schoolID < 0)||(schoolID == team.School.Id))
						{
							blnConfirmed = true;
							break;
						}
					}
				}
				if (blnConfirmed)
				{
					string strText = "";
					if (_user.Type == ((int) Sport.Types.UserType.Internal))
						strText += "אישור ";
					strText += "חלוקת שחקנים למקצועות";
					strHTML = "<button type=\"button\" "+
						"onclick=\"this.form.elements['action'].value = '"+
						SportSiteAction.AssignCompetitionPlayers.ToString()+"'; "+
						"this.form.submit();\">"+strText+"</button><br />";
					IsfMainView.AddContents(strHTML);
				}
			}
			
			IsfMainView.AddContents(objPanel);
			IsfMainView.AddContents("<hr />");
			
			//add only if school is defined, championship is in team register status 
			//and chamionship is open for external users:
			if (canAddTeam)
			{
				btnAddTeam.Style["text-align"] = "left";
				btnAddTeam.Attributes.Add("onclick", "this.form.elements['action'].value = '"+SportSiteAction.AddTeam.ToString()+"';");
				IsfMainView.AddContents(btnAddTeam);
			}
		} //end function DisplayChampTeams
		#endregion

		#region Player Registration
		/// <summary>
		/// takes the data from the form collection and add players to the pending 
		/// players database table. player numbers are automatically calculated, taking 
		/// the maximum index and adding one to that index for each player.
		/// </summary>
		private void AddPlayers()
		{
			int i;
			
			//get list of players to add from the form collection:
			string strPlayersID=Tools.CStrDef(Request.QueryString[PlayerToAddString], "");
			string strTeamID=Tools.CStrDef(SportData["TeamID"], "");
			
			//split into students:
			string[] arrPlayersID=Tools.SplitWithEscape(strPlayersID, ',');
			
			//check that it's not empty:
			if (arrPlayersID.Length == 0)
			{
				AddErrorMessage("שגיאה: לא נבחרו שחקנים להוספה");
				ShowTeamDetails();
				return;
			}
			
			//verify we have team:
			if (strTeamID.Length == 0)
			{
				AddErrorMessage("שגיאה: לא הוגדרה קבוצה עבורה יש להוסיף שחקנים");
				ShowTeamDetails();
				return;
			}
			
			//get school:
			Sport.Entities.Team team=null;
			team = new Sport.Entities.Team(Int32.Parse(strTeamID));
			if (!team.IsValid())
			{
				AddErrorMessage("שגיאה: טעינת קבוצה נכשלה");
				ShowTeamDetails();
				return;
			}
			
			int schoolID=team.School.Id;
			
			//initialize service:
			SportSite.RegistrationService.RegistrationService regService=new SportSite.RegistrationService.RegistrationService();
			regService.CookieContainer = (System.Net.CookieContainer) Session["cookies"];
			
			//find maximum player number for this team:
			int maxPlayerNumber=FindMaxPlayerNumber(Int32.Parse(strTeamID));
			
			//get list of pending players:
			RegistrationService.PlayerData[] arrPendingPlayers=
				regService.GetPendingPlayers(schoolID, Int32.Parse(strTeamID));
			
			//check if the maximum player number in pending table is bigger:
			for (i=0; i<arrPendingPlayers.Length; i++)
			{
				if (arrPendingPlayers[i].player_number > maxPlayerNumber)
					maxPlayerNumber = arrPendingPlayers[i].player_number;
			}
			
			//increase the number:
			maxPlayerNumber++;
			
			//build array of PlayerData to be added to pending players table.
			RegistrationService.PlayerData[] arrPlayerData=
				new SportSite.RegistrationService.PlayerData[arrPlayersID.Length];
			
			//populate the array with the proper details:
			for (i=0; i<arrPlayersID.Length; i++)
			{
				arrPlayerData[i] = new SportSite.RegistrationService.PlayerData();
				arrPlayerData[i].player_number = maxPlayerNumber+i;
				arrPlayerData[i].student_id = Int32.Parse(arrPlayersID[i]);
				arrPlayerData[i].team_id = Int32.Parse(strTeamID);
				arrPlayerData[i].user_id = _user.Id;
			}
			
			//send players data:
			regService.RegisterPlayers(_user.Id, arrPlayerData);
			
			//clear previous items:
			IsfMainView.ClearContents();
			
			//done.
			AddViewText(arrPlayerData.Length.ToString()+" שחקנים נוספו לסל ההזמנות.<br />");
			SetBasketData();
			
			ShowTeamDetails();
		} //end function AddPlayers
		
		/// <summary>
		/// remove given players from pending players table.
		/// </summary>
		private void RemovePendingPlayers()
		{
			//verify we have value:
			if (Request.Form["selected_players"] == null)
				return;
			
			string[] arrPlayers=Tools.SplitWithEscape(Request.Form["selected_players"], ',');
			SportSite.RegistrationService.RegistrationService regService=new SportSite.RegistrationService.RegistrationService();
			regService.CookieContainer = (System.Net.CookieContainer) Session["cookies"];
			
			//loop over the selected teams and remove:
			for (int i=0; i<arrPlayers.Length; i++)
			{
				//remove current pending team from database:
				int pendingPlayerID=Int32.Parse(arrPlayers[i]);
				regService.RemovePendingPlayer(pendingPlayerID);
			}
			
			//done.
			AddViewSuccessMsg("שחקנים נמחקו ממאגר הנתונים.");
		} //end function RemovePendingPlayers
		
		private void ShowPendingPlayers(bool centralUser, int userSchoolId)
		{
			TableRow objRow;
			TableCell objCell;
			
			string[] arrHeaders=new string[] {"שם קבוצה", "כרטיס", "ת\"ז", "שם שחקן", "מס' חולצה"};
			arrHeaders = Tools.AddToArray(arrHeaders, "&nbsp;");
			
			//decide amount of columns:
			int colsCount=arrHeaders.Length;
			
			//get list of pending players
			SportSite.RegistrationService.RegistrationService regService = 
				new SportSite.RegistrationService.RegistrationService();
			regService.CookieContainer = (System.Net.CookieContainer) Session["cookies"];
			SportSite.RegistrationService.PlayerData[] arrPendingPlayers = 
				regService.GetPendingPlayers(_user.School.ID, -1);
			
			//exit if there are no pending teams:
			if (arrPendingPlayers.Length == 0)
				return;
			
			//build html table:
			Table pendingPlayersTable=BuildPendingTable();
			
			//caption:
			string strCaption="שחקנים הממתינים לאישור עבור בית הספר "+_user.School.Name;
			pendingPlayersTable.Rows.Add(PendingCaptionRow(strCaption, colsCount));
			
			//headers:
			pendingPlayersTable.Rows.Add(PendingHeadersRow(arrHeaders, "selected_players"));
			
			//data:
			int lastTeamID=-1;
			string css = SportSite.Common.Style.PendingTeamCss;
			for (int i=0; i<arrPendingPlayers.Length; i++)
			{
				//initialize objects:
				SportSite.RegistrationService.PlayerData pendingPlayer=arrPendingPlayers[i];
				objRow = new TableRow();
				
				//get information:
				int teamID=pendingPlayer.team_id;
				Sport.Entities.Team team=null;
				team = new Sport.Entities.Team(teamID);
				if (!team.IsValid())
				{
					continue;
				}
				
				int studentID=pendingPlayer.student_id;
				Sport.Entities.Student student=new Sport.Entities.Student(studentID);
				if (!student.IsValid())
				{
					continue;
				}
				
				//new team?
				string strHTML="";
				if (lastTeamID != teamID)
				{
					if (lastTeamID > 0)
					{
						if (_user.School.ID == userSchoolId)
						{
							Button objButton=new Button();
							objButton.Text = "טופס רישום קבוצתי עבור קבוצה זו";
							objButton.Attributes.Add("onclick", 
								"this.form.target = '_blank'; "+
								"this.form.elements['action'].value = '"+
								SportSiteAction.CommitPendingPlayers.ToString()+"'; "+
								"this.form.elements['pending_players_team'].value = '"+
								lastTeamID+"';");
							objCell = FastControls.HebTableCell("", css);
							objCell.Attributes["colspan"] = arrHeaders.Length.ToString();
							objCell.Style["text-align"] = "center";
							objCell.Controls.Add(objButton);
							objRow.Cells.Add(objCell);
							pendingPlayersTable.Rows.Add(objRow);
							objRow = new TableRow();
						}
					}
					objCell = FastControls.HebTableCell("", css);
					objCell.Attributes["colspan"] = arrHeaders.Length.ToString();
					objCell.Style["text-align"] = "right";
					strHTML = team.Championship.Name+" "+team.Category.Name;
					objCell.Controls.Add(new System.Web.UI.LiteralControl(strHTML));
					objRow.Cells.Add(objCell);
					pendingPlayersTable.Rows.Add(objRow);
					objRow = new TableRow();
				}
				
				int categoryID=team.Category.Id;
				int champID=team.Championship.Id;
				int sportID=team.Championship.Sport.Id;
				string sportName=team.Championship.Sport.Name;
				string categoryName=team.Category.Name;
				string champName=team.Championship.Name;
				
				//add the information:
				objRow.BackColor = SportSite.Common.Style.PendingPlayersColors[i % SportSite.Common.Style.PendingPlayersColors.Length];
				
				//build team html:
				strHTML = team.Name;
				objCell = FastControls.HebTableCell("", css);
				objCell.Controls.Add(new System.Web.UI.LiteralControl(strHTML));
				objRow.Cells.Add(objCell);
				
				//player card:
				objRow.Cells.Add(PlayerCardCell(studentID, sportID, objRow.BackColor));
				
				//player ID number:
				objRow.Cells.Add(FastControls.HebTableCell(student.IdNumber, css));
				
				//player name:
				objRow.Cells.Add(FastControls.HebTableCell(student.Name, css));

				//player number:
				objRow.Cells.Add(PendingNumberCell(pendingPlayer.player_number, css, "player_number", 
					i, objRow.BackColor, pendingPlayer.id));
				
				objRow.Cells.Add(BuildCheckboxCell("selected_players", 
					arrPendingPlayers[i].id.ToString()));
				pendingPlayersTable.Rows.Add(objRow);
				
				lastTeamID = teamID;
			} //end loop over pending teams
			
			if (lastTeamID > 0)
			{
				if (_user.School.ID == userSchoolId)
				{
					Button objButton=new Button();
					objButton.Text = "טופס רישום קבוצתי עבור קבוצה זו";
					objButton.Attributes.Add("onclick", 
						"this.form.target = '_blank'; "+
						"this.form.elements['action'].value = '"+
						SportSiteAction.CommitPendingPlayers.ToString()+"'; "+
						"this.form.elements['pending_players_team'].value = '"+
						lastTeamID+"';");
					objCell = FastControls.HebTableCell("", css);
					objCell.Attributes["colspan"] = arrHeaders.Length.ToString();
					objCell.Style["text-align"] = "center";
					objCell.Controls.Add(objButton);
					objRow = new TableRow();
					objRow.Cells.Add(objCell);
					pendingPlayersTable.Rows.Add(objRow);
				}
			}
			IsfMainView.AddContents(FastControls.LineBreak(2));
			
			//add action buttons, currently delete button:
			Button btnDeletePlayers=new Button();
			btnDeletePlayers.ToolTip = "לחץ כדי למחוק שחקנים מסומנים";
			btnDeletePlayers.Font.Size = new FontUnit(10);
			btnDeletePlayers.Text = "[מחיקת שחקנים מסומנים]";
			objRow=new TableRow();
			objCell=new TableCell();
			objCell.HorizontalAlign = HorizontalAlign.Center;
			objCell.ColumnSpan = colsCount;
			objCell.Controls.Add(btnDeletePlayers);
			objRow.Cells.Add(objCell);
			objRow.Attributes["id"] = "ActionsRow";
			objRow.Style.Add("display", "none");
			pendingPlayersTable.Rows.Add(objRow);
			btnDeletePlayers.Attributes.Add("onclick", "this.form.elements['action'].value = '"+SportSiteAction.DeletePendingPlayers.ToString()+"'; return DeletePlayersClick(this);");
			
			IsfMainView.AddContents(pendingPlayersTable);
		} //end function ShowPendingPlayers
		
		/// <summary>
		/// returns the maximum player number in given team.
		/// </summary>
		private int FindMaxPlayerNumber(int teamid)
		{
			EntityType playerType=Sport.Entities.Player.Type;
			EntityFilter playerFilter=new EntityFilter((int) Sport.Entities.Player.Fields.Team, teamid);
			Entity[] players=playerType.GetEntities(playerFilter);
			int maxNumber=0;
			for (int i=0; i<players.Length; i++)
			{
				Entity player=players[i];
				int currentNumber=Tools.CIntDef(player.Fields[(int) Sport.Entities.Player.Fields.Number], 0);
				if (currentNumber > maxNumber)
				{
					maxNumber = currentNumber;
				}
			}
			return maxNumber;
		} //end function FindMaxPlayerNumber
		
		/// <summary>
		/// display details for one specific player, as given in the Form collection.
		/// </summary>
		private void ShowPlayersDetails()
		{
			//grab form data:
			string playerID=SportData["PlayerID"];
			if (playerID == null)
			{
				AddErrorMessage("לא הוגדר שחקן");
				return;
			}
			
			//extract database information:
			Sport.Entities.Player player=new Sport.Entities.Player(Int32.Parse(playerID));
			if (!player.IsValid())
			{
				AddErrorMessage("שחקן לא מוגדר");
				SportData["PlayerID"] = null;
				return;
			}
			Sport.Entities.Team team=player.Team;
			
			//clear previous items:
			IsfMainView.ClearContents();
			
			//display player details:
			_pageCaption = "נתוני שחקן";
			Sport.Types.GradeTypeLookup gradesLookup=new Sport.Types.GradeTypeLookup(true);
			Label objLabel=FastControls.HebTextLabel("<p align=\"right\">");
			System.Text.StringBuilder builder=new System.Text.StringBuilder();
			builder.Append("קבוצה: <a href=\"about:blank\" ");
			builder.Append("onclick=\"document.forms[0].elements['action'].value=");
			builder.Append("'"+SportSiteAction.ShowTeamDetails.ToString()+"'; ");
			builder.Append("document.forms[0].elements['TeamID'].value = '"+team.Id+"'; ");
			builder.Append("document.forms[0].submit(); return false;\" ");
			builder.Append("title=\"לחץ כדי לעבור אל מסך פירוט קבוצה\" ");
			builder.Append(">"+team.Name+"</a><br />");
			builder.Append("שם פרטי: "+player.Student.FirstName+"<br />");
			builder.Append("שם משפחה: "+player.Student.LastName+"<br />");
			builder.Append("מספר שחקן: "+player.Number+"<br />");
			builder.Append("כיתה: "+gradesLookup.Lookup(player.Student.Grade)+"<br />");
			builder.Append("תאריך לידה: "+player.Student.BirthDate.ToShortDateString()+"<br />");
			builder.Append("סטטוס שחקן: "+player.StatusText+"<br />");
			builder.Append("כרטיס שחקן: "+PlayerCardCheckbox(player.Student.Id, 
				team.Championship.Sport.Id).Text);
			builder.Append("");
			objLabel.Text += builder.ToString();
			objLabel.Text += "</p>";
			IsfMainView.AddContents(objLabel);
		} //end function ShowPlayersDetails
		#endregion
		
		#region Assign Competition Players
		private void AssignCompetitionPlayers()
		{
			IsfMainView.ClearContents();
			_pageCaption = "חלוקת שחקנים למקצועות";
			int categoryID = Tools.CIntDef(Request.QueryString["category"], -1);
			int selectedTeamID = Tools.CIntDef(Request.QueryString["team"], -1);
			Sport.Entities.ChampionshipCategory champCategory = null;
			if (categoryID >= 0)
			{
				try
				{
					champCategory = new ChampionshipCategory(categoryID);
				}
				catch
				{
				}
			}
			if (champCategory == null)
			{
				AddErrorMessage("זיהוי אליפות לא חוקי: "+categoryID);
				return;
			}
			
			IsfMainView.AddContents("<h2>"+champCategory.Championship.Name+" "+
				champCategory.Name+"</h2>");
			
			Sport.Championships.Championship championship = null;
			string strGeneralError = "";
			try
			{
				championship = 
					Sport.Championships.Championship.GetChampionship(categoryID);
			}
			catch (Exception ex)
			{
				strGeneralError = "<!-- message: "+ex.Message+"\nstack: "+ex.StackTrace+" -->";
			}
			if (championship == null)
			{
				AddErrorMessage("כשלון בעת טעינת נתוני אליפות"+strGeneralError);
				return;
			}
			
			if (!(championship is Sport.Championships.CompetitionChampionship))
			{
				AddErrorMessage("אליפות לא מסוג תחרות");
				return;
			}
			
			if (Request.Form["assign_competitors"] == "1")
			{
				UpdateCompetitionCompetitors();
				return;
			}
			
			IsfMainView.clientSide.AddOnloadCommand("document.forms[0].elements[\""+
				"action\"].value = \""+SportSiteAction.AssignCompetitionPlayers.ToString()+
				"\";", false, false);
			
			int schoolID = _user.School.ID;
			int phaseIndex = -1;
			int groupIndex = -1;
			if (Request.QueryString["phase"] != null)
			{
				string strSelectedPhase = Request.QueryString["phase"];
				for (int i=0; i<championship.Phases.Count; i++)
				{
					if (strSelectedPhase == championship.Phases[i].Name)
					{
						phaseIndex = i;
						string strSelectedGroup = Request.QueryString["group"];
						if (championship.Phases[i].Groups != null)
						{
							for (int j=0; j<championship.Phases[i].Groups.Count; j++)
							{
								if (championship.Phases[i].Groups[j].Name == strSelectedGroup)
								{
									groupIndex = j;
									break;
								}
							}
						}
						break;
					}
				}
			}
			if (phaseIndex < 0)
				phaseIndex = Tools.CIntDef(Request.Form["phase"], -1);
			if (groupIndex < 0)
				groupIndex = Tools.CIntDef(Request.Form["group"], -1);
			
			ArrayList arrPhasePool = new ArrayList();
			ArrayList arrGroupPool = new ArrayList();
			foreach (Sport.Championships.Phase phase in championship.Phases)
			{
				if (phase.Groups != null)
				{
					foreach (Sport.Championships.CompetitionGroup group in phase.Groups)
					{
						if ((group.Competitions != null)&&(group.Teams != null))
						{
							if (group.Competitions.Count == 0)
								continue;
							foreach (Sport.Championships.CompetitionTeam compTeam
										 in group.Teams)
							{
								if (compTeam.TeamEntity == null)
									continue;
								if ((schoolID < 0)||(compTeam.TeamEntity.School.Id == schoolID))
								{
									if (arrPhasePool.IndexOf(phase) < 0)
										arrPhasePool.Add(phase);
									if (phaseIndex == phase.Index)
										if (arrGroupPool.IndexOf(group) < 0)
											arrGroupPool.Add(group);
									break;
								}
							}
						}
					}
				}
			}
			
			if (arrPhasePool.Count == 0)
			{
				AddErrorMessage("באליפות זו אין תחרויות או שקבוצתך לא רשומה לשום בית");
				return;
			}
			
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			if ((phaseIndex < 0)||(phaseIndex >= arrPhasePool.Count))
			{
				sb.Append("בחר שלב אליפות: <select name=\"phase\">");
				foreach (Sport.Championships.Phase phase in arrPhasePool)
				{
					int curIndex = phase.Index;
					sb.Append("<option value=\""+curIndex+"\"");
					if (curIndex == phaseIndex)
						sb.Append(" selected=\"selected\"");
					sb.Append(">"+phase.Name+"</option>");
				}
				sb.Append("</select><br />");
				sb.Append("<button type=\"submit\">שלח</button>");
				IsfMainView.AddContents(sb.ToString());
				return;
			}
			
			Sport.Championships.Phase selectedPhase = 
				(arrPhasePool[phaseIndex] as Sport.Championships.Phase);
			
			sb.Append(FastControls.HiddenInputHtml("phase", phaseIndex.ToString()));
			sb.Append("שלב: "+selectedPhase.Name+"<br />");
			
			if ((groupIndex < 0)||(groupIndex >= arrGroupPool.Count))
			{
				sb.Append("בחר בית אליפות: <select name=\"group\">");
				foreach (Sport.Championships.CompetitionGroup group in arrGroupPool)
				{
					int curIndex = group.Index;
					sb.Append("<option value=\""+curIndex+"\"");
					if (curIndex == groupIndex)
						sb.Append(" selected=\"selected\"");
					sb.Append(">"+group.Name+"</option>");
				}
				sb.Append("</select><br />");
				sb.Append("<button type=\"submit\" onclick=\"ShowWait(this, 'אנא המתן'"+
					", 30);\">שלח</button>");
				IsfMainView.AddContents(sb.ToString());
				return;
			}
			
			Sport.Championships.CompetitionGroup selectedGroup = 
				(selectedPhase.Groups[groupIndex] as Sport.Championships.CompetitionGroup);
			
			sb.Append(FastControls.HiddenInputHtml("group", groupIndex.ToString()));
			sb.Append("בית: "+selectedGroup.Name+"<br />");
			
			sb.Append(FastControls.HiddenInputHtml("phase_name", selectedPhase.Name));
			sb.Append(FastControls.HiddenInputHtml("group_name", selectedGroup.Name));
			
			Sport.Championships.CompetitionGroup compGroup = (Sport.Championships.CompetitionGroup)
				championship.Phases[phaseIndex].Groups[groupIndex];
			Hashtable tblCompetitionCompetitors = new Hashtable();
			Hashtable tblTeamPlayers = new Hashtable();
			bool blnAdmin = (_user.Type == ((int) Sport.Types.UserType.Internal));
			if (blnAdmin)
				IsfMainView.clientSide.AddOnloadCommand("_admin = 1;", false, true);
			foreach (Sport.Championships.Competition competition in compGroup.Competitions)
			{
				if (tblCompetitionCompetitors[competition] == null)
					tblCompetitionCompetitors[competition] = new ArrayList();
				if (blnAdmin)
					continue;
				foreach (Sport.Championships.Competitor competitor in competition.Competitors)
				{
					if (competitor.Player == null)
						continue;
					if (competitor.Player.CompetitionTeam == null)
						continue;
					if (competitor.Player.CompetitionTeam.TeamEntity == null)
						continue;
					Sport.Entities.School compSchool = 
						competitor.Player.CompetitionTeam.TeamEntity.School;
					if (compSchool == null)
						continue;
					if ((schoolID < 0)||(compSchool.Id == schoolID))
						(tblCompetitionCompetitors[competition] as ArrayList).Add(competitor);
				}
			}
			foreach (Sport.Championships.CompetitionTeam compTeam in compGroup.Teams)
			{
				Sport.Entities.Team curTeam = compTeam.TeamEntity;
				if (curTeam == null)
					continue;
				if ((selectedTeamID >= 0)&&(selectedTeamID != curTeam.Id))
					continue;
				if ((schoolID < 0)||(compTeam.TeamEntity.School.Id == schoolID))
				{
					if (tblTeamPlayers[curTeam] == null)
						tblTeamPlayers[curTeam] = new ArrayList();
					(tblTeamPlayers[curTeam] as ArrayList).AddRange(curTeam.GetPlayers());
				}
			}
			
			WebSiteServices.WebsiteService service = 
				new WebSiteServices.WebsiteService();
			
			sb.Append(FastControls.HiddenInputHtml("assign_competitors", "1"));
			sb.Append("<table id=\"CompetitionCompetitors\" border=\"0\" cellspacing=\"0\">");
			sb.Append("<tr>");
			sb.Append("<th>שחקנים</th>");
			foreach (Sport.Championships.Competition competition in tblCompetitionCompetitors.Keys)
				sb.Append("<th>"+competition.Name+"</th>");
			sb.Append("</tr>");
			foreach (Sport.Entities.Team team in tblTeamPlayers.Keys)
			{
				WebSiteServices.PendingCompetitorData[] arrPendingCompetitors = 
					service.GetPendingCompetitors(team.Id, selectedPhase.Name);
				Hashtable tblPendingCompetitors = new Hashtable();
				if (arrPendingCompetitors != null)
				{
					foreach (WebSiteServices.PendingCompetitorData data in arrPendingCompetitors)
					{
						string strKey = data.Player+"_"+data.Competition;
						tblPendingCompetitors[strKey] = data.Heat;
					}
				}
				
				sb.Append("<tr>");
				sb.Append("<td colspan=\""+(tblCompetitionCompetitors.Count+1)+"\" "+
					" align=\"center\">"+team.TeamName()+"</td>");
				sb.Append("</tr>");
				ArrayList arrTeamPlayers = (ArrayList) tblTeamPlayers[team];
				foreach (Sport.Entities.Player curPlayer in arrTeamPlayers)
				{
					if (curPlayer.Student == null)
						continue;
					/*
					if (blnAdmin)
					{
						int count = 0;
						foreach (Sport.Championships.Competition competition in tblCompetitionCompetitors.Keys)
						{
							string strCurKey = curPlayer.Id+"_"+competition.Index;
							if (tblPendingCompetitors[strCurKey] != null)
								count++;
						}
						if (count == 0)
							continue;
					}
					*/
					sb.Append("<tr>");
					sb.Append("<td>");
					sb.Append(curPlayer.Student.FirstName+" "+curPlayer.Student.LastName);
					sb.Append("</td>");
					foreach (Sport.Championships.Competition competition in tblCompetitionCompetitors.Keys)
					{
						string strCheckboxName = "p_"+curPlayer.Id+"_c_"+
							competition.Index+"_t_"+team.Id;
						string strCurKey = curPlayer.Id+"_"+competition.Index;
						object oCompHeat = tblPendingCompetitors[strCurKey];
						sb.Append("<td>");
						if ((blnAdmin)&&(oCompHeat == null))
						{
							sb.Append("&nbsp;");
							sb.Append("</td>");
							continue;
						}
						sb.Append("<input type=\"checkbox\" name=\""+
							strCheckboxName+"\" value=\"1\"");
						ArrayList arrCompetitors = (ArrayList) tblCompetitionCompetitors[competition];
						Sport.Championships.Competitor curPlayerCompetitor = null;
						if (!blnAdmin)
						{
							foreach (Sport.Championships.Competitor competitor in arrCompetitors)
							{
								if (competitor.PlayerNumber == curPlayer.Number)
								{
									curPlayerCompetitor = competitor;
									break;
								}
								if (competitor.Player == null)
									continue;
								if (competitor.Player.PlayerEntity == null)
									continue;
								if (competitor.Player.PlayerEntity.Id == curPlayer.Id)
								{
									curPlayerCompetitor = competitor;
									break;
								}
							}
						}
						int compHeat = -1;
						if (oCompHeat != null)
							compHeat = (int) oCompHeat;
						if ((curPlayerCompetitor != null)||(oCompHeat != null))
						{
							sb.Append(" checked=\"checked\"");
							if ((schoolID >= 0)&&(curPlayerCompetitor != null))
								sb.Append(" disabled=\"disabled\"");
						}
						string strCheckboxText = "משתתף?";
						if (tblCompetitionCompetitors.Count > 5)
							strCheckboxText = "";
						sb.Append(" />&nbsp;"+strCheckboxText+"<br />");
						if (competition.Heats != null)
						{
							string strRadioName = strCheckboxName+"_heat";
							foreach (Sport.Championships.Heat heat in competition.Heats)
							{
								sb.Append("<input type=\"radio\" name=\""+
									strRadioName+"\" value=\""+heat.Index+"\"");
								if ((compHeat == heat.Index)||
									((curPlayerCompetitor != null)&&(curPlayerCompetitor.Heat == heat.Index)))
								{
									sb.Append(" checked=\"checked\"");
									if ((!blnAdmin)&&(curPlayerCompetitor != null))
										sb.Append(" disabled=\"disabled\"");
								}
								strCheckboxText = "מקצה ";
								if (tblCompetitionCompetitors.Count > 5)
									strCheckboxText = "";
								sb.Append(" />&nbsp;"+strCheckboxText+(heat.Index+1)+"<br />");
							}
						}
						sb.Append("</td>");
					}
					sb.Append("</tr>");
				}
			}
			sb.Append("</table>"+Tools.MultiString("<br />", 2));
			int iDelay = (blnAdmin)?15:10;
			string strButtonText = (blnAdmin)?"אישור סופי":"בצע חלוקה";
			sb.Append("<center><button style=\"font-weight: bold;\" "+
				"type=\"submit\" onclick=\"ShowWait(this, 'אנא המתן'"+
				", "+iDelay+");\">"+strButtonText+"</button></center>");
			IsfMainView.AddContents(sb.ToString());
			IsfMainView.clientSide.AddOnloadCommand("AssignCompetitorsCheckboxes();", false, false);
		}
		
		private void UpdateCompetitionCompetitors()
		{
			//get selected phase and group:
			string strPhaseName = Request.Form["phase_name"];
			string strGroupName = Request.Form["group_name"];
			
			//get user school:
			int schoolID = _user.School.ID;
			string strSchoolName = _user.School.Name;
			
			//got selected team?
			if (Request.QueryString["team"] != null)
			{
				Sport.Entities.Team selectedTeam = null;
				Sport.Entities.School selectedSchool = null;
				try
				{
					selectedTeam = new Sport.Entities.Team(
						Tools.CIntDef(Request.QueryString["team"], -1));
				}
				catch {}
				if (selectedTeam != null)
					selectedSchool = selectedTeam.School;
				if (selectedSchool == null)
				{
					AddErrorMessage("זיהוי קבוצה שגוי");
					return;
				}
				schoolID = selectedSchool.Id;
				strSchoolName = selectedSchool.Name;
			}
			
			//initialize pending competitors array:
			ArrayList arrPendingCompetitors = new ArrayList();
			
			//initialize teams table:
			Hashtable tblTeamPlayers = new Hashtable();
			
			//iterate through request collection, look for competitors.
			Hashtable tblCompetitionCompetitors = new Hashtable();
			foreach  (string key in Request.Form)
			{
				//player?
				if (!key.StartsWith("p_"))
					continue;
				
				//heat?
				if (key.EndsWith("_heat"))
					continue;
				
				//split:
				string[] arrTemp = key.Split(new char[] {'_'});
				
				//got enough items?
				if (arrTemp.Length <= 5)
					continue;
				
				//get current information:
				int playerID = Common.Tools.CIntDef(arrTemp[1], -1);
				int competition = Common.Tools.CIntDef(arrTemp[3], -1);
				int teamID = Common.Tools.CIntDef(arrTemp[5], -1);
				int heat = Common.Tools.CIntDef(Request.Form[key+"_heat"], -1);
				
				//valid?
				if ((playerID < 0)||(competition < 0)||(teamID < 0))
					continue;
				
				//need to create team data?
				Sport.Entities.Team team = null;
				if (tblTeamPlayers[teamID] == null)
				{
					tblTeamPlayers[teamID] = new ArrayList();
					try
					{
						team = new Sport.Entities.Team(teamID);
					}
					catch
					{}
					if ((team != null)&&(team.School.Id == schoolID))
						(tblTeamPlayers[teamID] as ArrayList).AddRange(team.GetPlayers());
				}
				
				//get players:
				ArrayList arrPlayers = (ArrayList) tblTeamPlayers[teamID];
				
				//got anything?
				if ((arrPlayers == null)||(arrPlayers.Count == 0))
					continue;
				
				//player belong to this team?
				bool blnPlayerExists = false;
				foreach (Sport.Entities.Player player in arrPlayers)
				{
					if (player.Id == playerID)
					{
						blnPlayerExists = true;
						break;
					}
				}
				if (!blnPlayerExists)
					continue;
				
				//build current data:
				WebSiteServices.PendingCompetitorData data = 
					new WebSiteServices.PendingCompetitorData();
				data.Player = playerID;
				data.Competition = competition;
				data.Heat = heat;
				data.Phase = strPhaseName;
				data.Group = strGroupName;
				
				//need to create?
				if (tblCompetitionCompetitors[teamID] == null)
					tblCompetitionCompetitors[teamID] = new ArrayList();
				
				//add to list.
				(tblCompetitionCompetitors[teamID] as ArrayList).Add(data);
			} //end loop over form elements
			

			//System.Text.StringBuilder sb = new System.Text.StringBuilder();
			
			//initialize web service:
			WebSiteServices.WebsiteService service = new WebSiteServices.WebsiteService();
			
			//need to load championship?
			bool blnAdmin = (_user.Type == ((int) Sport.Types.UserType.Internal));
			Sport.Championships.Championship championship = null;
			Sport.Championships.CompetitionGroup group = null;
			if (blnAdmin)
			{
				int categoryID = Tools.CIntDef(Request.QueryString["category"], -1);
				championship = (Sport.Championships.CompetitionChampionship)
					Sport.Championships.Championship.GetChampionship(categoryID);
				championship.Edit();
				foreach (Sport.Championships.CompetitionPhase curPhase in championship.Phases)
				{
					if (curPhase.Name == strPhaseName)
					{
						foreach (Sport.Championships.CompetitionGroup curGroup in curPhase.Groups)
						{
							if (curGroup.Name == strGroupName)
							{
								group = curGroup;
								break;
							}
						}
						break;
					}
				}
			}
			
			//iterate over the team competitors and add each.
			foreach (int curTeam in tblCompetitionCompetitors.Keys)
			{
				//get competitors:
				ArrayList arrCompetitors = (ArrayList) tblCompetitionCompetitors[curTeam];
				
				Sport.Entities.Team team = null;
				Sport.Entities.User supervisor = null;
				
				if (blnAdmin)
				{
					team = new Sport.Entities.Team(curTeam);
					ArrayList arrTeamPlayers = (ArrayList) tblTeamPlayers[curTeam];
					Hashtable tblPlayers = new Hashtable();
					foreach (Sport.Entities.Player curPlayer in arrTeamPlayers)
						tblPlayers[curPlayer.Id] = curPlayer;
					ArrayList arrSuccessMessages = new ArrayList();
					foreach (WebSiteServices.PendingCompetitorData compData in arrCompetitors)
					{
						int playerID = compData.Player;
						if (tblPlayers[playerID] == null)
							continue;
						Sport.Entities.Player player = (Sport.Entities.Player) tblPlayers[playerID];
						int shirtNumber = player.Number;
						Sport.Championships.Competition competition = 
							group.Competitions[compData.Competition];
						bool blnExists = false;
						int teamIndex = -1;
						foreach (Sport.Championships.Competitor competitor in competition.Competitors)
						{
							if (competitor.PlayerNumber == shirtNumber)
							{
								blnExists = true;
								teamIndex = competitor.Team;
								break;
							}
						}
						if (blnExists)
						{
							AddErrorMessage("השחקן "+player.FirstName+" "+player.LastName+
								" כבר רשום לתחרות '"+competition.Name+"'");
							continue;
						}
						Sport.Championships.Competitor newCompetitor = 
							new Sport.Championships.Competitor(shirtNumber);
						newCompetitor.Team = teamIndex;
						newCompetitor.Heat = compData.Heat;
						competition.Competitors.Add(newCompetitor);
						arrSuccessMessages.Add("השחקן "+player.FirstName+" "+
							player.LastName+" נרשם בהצלחה לתחרות '"+competition.Name+"'");
					}
					if (arrSuccessMessages.Count > 0)
					{
						string strErrorMessage = championship.Save();
						if (strErrorMessage.Length > 0)
						{
							AddErrorMessage("שגיאה בעת שמירת אליפות: "+strErrorMessage);
						}
						else
						{
							foreach (string strSuccessMessage in arrSuccessMessages)
								AddSuccessMessage(strSuccessMessage);
							supervisor = team.Supervisor;
							if (supervisor != null)
							{
								Sport.Data.EntityEdit message = 
									Sport.Entities.InstantMessage.Type.New();
								string strContents = "הקצאת מתמודדים עבור האליפות "+
									"'"+championship.Name+"' אושרה על ידי התאחדות הספורט";
								message.Fields[(int) InstantMessage.Fields.Sender] = _user.Id;
								message.Fields[(int) InstantMessage.Fields.Recipient] = supervisor.Id;
								message.Fields[(int) InstantMessage.Fields.DateSent] = DateTime.Now;
								message.Fields[(int) InstantMessage.Fields.LastModified] = DateTime.Now;
								message.Fields[(int) InstantMessage.Fields.Contents] = strContents;
								message.Save();
							}
							service.AddPendingCompetitors(curTeam, null);
						}
					}
					else
					{
						AddErrorMessage("אין הקצאות מתמודדים");
					}
					break;
				}
				
				//add:
				service.AddPendingCompetitors(curTeam, (WebSiteServices.PendingCompetitorData[])
					arrCompetitors.ToArray(typeof(WebSiteServices.PendingCompetitorData)));
				
				//send message:
				team = ((tblTeamPlayers[curTeam] as ArrayList)[0] as Sport.Entities.Player).Team;
				Sport.Entities.Championship champEnt = team.Championship;
				supervisor = champEnt.Supervisor;
				if (supervisor == null)
					supervisor = champEnt.Region.Coordinator;
				string strChampName = champEnt.Name+" "+team.Category.Name;
				if (supervisor != null)
				{
					Sport.Data.EntityEdit message = 
						Sport.Entities.InstantMessage.Type.New();
					string strContents = "בית הספר '"+strSchoolName+"' שלח בקשה "+
						"לחלוקת משתתפים למקצועות באליפות '"+strChampName+"'\n"+
						"כדי לאשר את החלוקה יש להתחבר לאתר דרך כלי הניהול.";
					message.Fields[(int) InstantMessage.Fields.Sender] = _user.Id;
					message.Fields[(int) InstantMessage.Fields.Recipient] = supervisor.Id;
					message.Fields[(int) InstantMessage.Fields.DateSent] = DateTime.Now;
					message.Fields[(int) InstantMessage.Fields.LastModified] = DateTime.Now;
					message.Fields[(int) InstantMessage.Fields.Contents] = strContents;
					message.Save();
				}
				
				AddSuccessMessage("נשלחה בקשה להקצאת "+arrCompetitors.Count+" מתמודדים לאליפות "+
					"'"+strChampName+"' עבור הקבוצה '"+team.TeamName()+"'<br />");
			} //end loop over teams
		} //end function UpdateCompetitionCompetitors
		#endregion
		
		#region General Methods
		#region general
		/// <summary>
		/// change given team index or player number.
		/// </summary>
		private void ChangePendingNumber()
		{
			//first, get entity id to change index of number:
			int entID=Tools.CIntDef(Request.Form["changed_entity_id"], -1);
			
			//abort if invalid id:
			if (entID <= 0)
			{
				AddErrorMessage("לא ניתן לשנות מספר: זיהוי בלתי חוקי");
				return;
			}

			//check if it's team or player and get new number:
			bool isTeamIndex=true;
			string number=Request.Form["team_index_new_"+entID.ToString()];
			if (number == null)
			{
				isTeamIndex = false;
				number=Request.Form["player_number_new_"+entID.ToString()];
				if (number == null)
				{
					AddErrorMessage("לא ניתן לשנות מספר: לא התקבל מספר חדש");
					return;
				}
				isTeamIndex = false;
			}

			//initialize service:
			SportSite.RegistrationService.RegistrationService regService=new SportSite.RegistrationService.RegistrationService();
			regService.CookieContainer = (System.Net.CookieContainer) Session["cookies"];
			
			//call proper methods:
			if (isTeamIndex)
			{
				regService.UpdatePendingTeam(entID, Int32.Parse(number));
			}
			else
			{
				regService.UpdatePendingPlayer(entID, Int32.Parse(number));
			}

			string msg="";
			msg += (isTeamIndex)?"אינדקס קבוצה":"מספר שחקן";
			msg += " שונה בהצלחה";
			
			successLabel.Text += msg+"<br />";
			successLabel.Visible = true;
		} //end function ChangePendingNumber
		
		/// <summary>
		/// display full details of the logged-in user's orders basket and let
		/// him Commit all the pending teams and players to the database.
		/// </summary>
		private void ShowOrdersBasket()
		{
			//exit if there is no school defined:
			if (_user.School.ID <= 0)
				return;
			
			//get actual school:
			//Sport.Entities.User user=new Sport.Entities.User(_user.Id);
			websiteService = new WebSiteServices.WebsiteService();
			WebSiteServices.UserData user=websiteService.GetUserData(_user.Id);
			int userSchoolId=user.UserSchool; //(user.School == null)?-1:user.School.Id;
			
			//central user?
			bool centralUser=(_user.Region.ID == Sport.Entities.Region.CentralRegion);
			
			//add hidden field:
			IsfMainView.AddContents(FastControls.HiddenInput("changed_entity_id", ""));
			
			//check if manager:
			if (_user.School.ID != userSchoolId)
				IsfMainView.AddContents(FastControls.HebTextLabel("(רק מנהל בית הספר רשאי לאשר סופית את הרישום)"));
			
			//show teams:
			ShowPendingTeams(centralUser, userSchoolId);
			
			//show players:
			ShowPendingPlayers(centralUser, userSchoolId);
		} //end function ShowOrdersBasket
		
		/// <summary>
		/// commit all pending teams for the logged in user to the database.
		/// </summary>
		private void CommitPendingTeams()
		{
			//initialize service:
			SportSite.RegistrationService.RegistrationService regService=new SportSite.RegistrationService.RegistrationService();
			regService.CookieContainer = (System.Net.CookieContainer) Session["cookies"];
			
			//can commit only if this user is defined in the database as manager of the school!
			//Sport.Entities.User user=new Sport.Entities.User(_user.Id);
			websiteService = new WebSiteServices.WebsiteService();
			WebSiteServices.UserData user=websiteService.GetUserData(_user.Id);
			Sport.Entities.School school = new Sport.Entities.School(user.UserSchool);
			if ((school == null)||(school.Id != _user.School.ID))
			{
				AddErrorMessage("רק מנהל בית הספר רשאי לאשר את רישום הקבוצות!");
				return;
			}
			
			//commit data:
			if (regService.CommitPendingTeams(_user.School.ID) == true)
			{
				AddViewSuccessMsg("קבוצות נשלחו בהצלחה למאגר הנתונים.");
			}
			else
			{
				AddErrorMessage("שגיאה בעת שליחת קבוצות חדשות אל מאגר הנתונים.");
			}
			
			//reload basket:
			SetOrdersBasket();
			
			//reload entities:
			Sport.Entities.Team.Type.Reset(null);
			Sport.Entities.Player.Type.Reset(null);
		} //end function CommitPendingData
		
		/// <summary>
		/// commit all pending players for the logged in user to the database.
		/// </summary>
		private void CommitPendingPlayers()
		{
			//hide all controls:
			foreach (Control control in this.Controls)
				control.Visible = false;
			
			//initialize HTML string:
			System.Text.StringBuilder sb=new System.Text.StringBuilder();
			
			//get players team:
			int teamID=Tools.CIntDef(Request.Form["pending_players_team"], -1);
			if (teamID < 0)
			{
				Response.Write("missing or invalid team ID");
				return;
			}
			
			//build team:
			Sport.Entities.Team team=new Sport.Entities.Team(teamID);
			if ((team == null)||(team.Id < 0))
			{
				Response.Write("invalid team ID: "+teamID);
				return;
			}
			
			//can commit only if this user is defined in the database as manager of the school!
			//Sport.Entities.User user=new Sport.Entities.User(_user.Id);
			websiteService = new WebSiteServices.WebsiteService();
			WebSiteServices.UserData user=websiteService.GetUserData(_user.Id);
			Sport.Entities.School school = new Sport.Entities.School(user.UserSchool);
			if ((school == null)||(school.Id != _user.School.ID))
			{
				Response.Write("רק מנהל בית הספר רשאי לאשר את רישום הקבוצות!");
				return;
			}
			
			//initialize service:
			SportSite.RegistrationService.RegistrationService regService=new SportSite.RegistrationService.RegistrationService();
			regService.CookieContainer = (System.Net.CookieContainer) Session["cookies"];
			
			//get pending players:
			RegistrationService.PlayerData[] arrPlayers=
				regService.GetPendingPlayers(_user.School.ID, teamID);
			
			//got anything?
			if ((arrPlayers == null)||(arrPlayers.Length == 0))
			{
				Response.Write("אין שחקנים עבור קבוצה זו");
				return;
			}
			
			//confirmed?
			if (Request.Form["final_confirm"] == "1")
			{
				regService.CommitPendingPlayers(_user.School.ID, teamID);
				Response.Write("<center>");
				Response.Write("<div align=\"right\" dir=\"rtl\">");
				Response.Write("<span style=\"font-family: Arial; font-size: 18px; font-weight: bold;\">"+
					"רישום "+arrPlayers.Length+" שחקנים בוצע בהצלחה.</span><br />");
				Response.Write("<button onclick=\"window.opener.document.location.reload(); window.close();\">חזרה למסך ראשי</button>");
				Response.Write("</div>");
				Response.Write("</center>");
				return;
			}
			
			//page:
			int curPage=Tools.CIntDef(Request.Form["page"], 1);
			
			//players registration form:
			string strHTML=Tools.BuildPlayersRegisterForm(team, arrPlayers, this.Server, 
				Request.Form["action"], 
				Request.ServerVariables["Script_Name"], curPage, this.Session);
			
			//add to HTML:
			sb.Append(strHTML);
			
			//reload basket:
			SetOrdersBasket();
			
			this.Controls.Add(new LiteralControl(sb.ToString()));
			
			//reload entities:
			Sport.Entities.Team.Type.Reset(null);
			Sport.Entities.Player.Type.Reset(null);
		} //end function CommitPendingData
		#endregion
		
		#region building elements
		/// <summary>
		/// build the standard table for pending teams/players data.
		/// </summary>
		/// <returns></returns>
		private Table BuildPendingTable()
		{
			Table result=new Table();
			result.BorderWidth = new Unit((double) 1);
			result.BorderColor = SportSite.Common.Style.NavBarBgColor;
			result.BorderStyle = BorderStyle.Ridge;
			result.GridLines = GridLines.Both;
			result.Width = new Unit(100, UnitType.Percentage);
			return result;
		}

		/// <summary>
		/// build the caption row for the pending teams/players data.
		/// </summary>
		private TableRow PendingCaptionRow(string captionText, int colsCount)
		{
			TableRow result=new TableRow();
			TableCell objCell=new TableCell();
			objCell.Text = captionText;
			objCell.HorizontalAlign = HorizontalAlign.Center;
			objCell.CssClass = SportSite.Common.Style.TableViewCaptionCss;
			objCell.ColumnSpan = colsCount;
			result.Cells.Add(objCell);
			return result;
		}

		/// <summary>
		/// build the headers row for teams/players pending data.
		/// </summary>
		private TableRow PendingHeadersRow(string[] arrHeadersText, string checkboxBindName)
		{
			TableRow result = new TableRow();
			for (int i=0; i<arrHeadersText.Length; i++)
			{
				TableHeaderCell headerCell;
				if ((arrHeadersText[i] == "&nbsp;")&&(checkboxBindName == "selected_players"))
				{
					headerCell = new TableHeaderCell();
					string strHtml="<input type=\"checkbox\" ";
					strHtml += "onclick=\"BindCheckBoxes(this, '"+checkboxBindName+"');\" />";
					headerCell.Controls.Add(new System.Web.UI.LiteralControl(strHtml));
				}
				else
				{
					headerCell= FastControls.HebHeaderCell(arrHeadersText[i]);
				}
				result.Cells.Add(headerCell);
			}
			return result;
		}
		
		/// <summary>
		/// build table cell for pending player number or for pending team index.
		/// use the given index/number and given css and enable the user to change
		/// the number by clicking it, using client side code.
		/// </summary>
		private TableCell PendingNumberCell(int number, string css, string cellType, 
			int cellIndex, System.Drawing.Color bgColor, int entityId)
		{
			TableCell result=FastControls.HebTableCell("", css);
			result.Wrap = false;
			IsfMainView.clientSide.MakeBrighter(result, Sport.Common.Tools.ColorToHex(bgColor), 50);
			
			Label objLabel=new Label();
			string textBoxName=cellType+"_new_"+entityId.ToString();
			string id=cellType+"_"+cellIndex.ToString();
			objLabel.Style.Add("cursor", "pointer");
			objLabel.ToolTip = "לחץ כדי לשנות ערך זה";
			objLabel.Attributes.Add("onclick", "ShowChangeNumber('"+id+"', '"+textBoxName+"');");
			objLabel.Attributes["id"]=id;
			objLabel.Text = number.ToString();
			result.Controls.Add(objLabel);
			
			objLabel=new Label();
			objLabel.Style.Add("display", "none");
			objLabel.Attributes["id"]=id+"_change";

			string strHtml="<input type=\"text\" size=\"1\" maxlength=\"4\" ";
			strHtml += "onkeypress=\"return DigitOnly(event);\" ";
			strHtml += "name=\""+textBoxName+"\" value=\""+number.ToString()+"\" /><br />";
			strHtml += "<input type=\"button\" value=\"אישור\" title=\"אישור שינוי מספר\" ";
			strHtml += "onclick=\"if (!ConfirmNumeric(this.form.elements['"+textBoxName+"'])) return false; ";
			strHtml += "this.form.elements['action'].value = ";
			strHtml += "'"+SportSiteAction.ChangePendingNumber.ToString()+"'; ";
			strHtml += " this.form.elements['changed_entity_id'].value = ";
			strHtml += "'"+entityId.ToString()+"'; this.form.submit();\" /> ";
			strHtml += "<input type=\"button\" value=\"ביטול\" title=\"ביטול שינוי מספר\" ";
			strHtml += "onclick=\"CancelChangeNumber('"+id+"');\" />";
			objLabel.Text = strHtml;
			result.Controls.Add(objLabel);
			
			return result;
		}

		/// <summary>
		/// build cell with checkbox used to "select" current row for various actions.
		/// </summary>
		private System.Web.UI.WebControls.TableCell BuildCheckboxCell(string name, string value)
		{
			TableCell result=new TableCell();
			result.HorizontalAlign = HorizontalAlign.Center;
			string strHtml="<input type=\"checkbox\" name=\""+name+"\" ";
			strHtml += "value=\""+value+"\" onclick=\"CellClick(event, this);\" />";
			result.Controls.Add(new LiteralControl(strHtml));
			return result;
		}

		/// <summary>
		/// build cell with checkbox used to set player card status upon clicking.
		/// </summary>
		private System.Web.UI.WebControls.TableCell PlayerCardCell(int studentID, 
			int sportID, Color bgColor)
		{
			//initialize table cell:
			TableCell result=new TableCell();

			//add light upon mouse over:
			IsfMainView.clientSide.MakeBrighter(result, Sport.Common.Tools.ColorToHex(bgColor), 60);
			result.HorizontalAlign = HorizontalAlign.Center;

			//add checkbox:
			result.Controls.Add(PlayerCardCheckbox(studentID, sportID));
			return result;
		}

		/// <summary>
		/// build checkbox used to set player card status upon clicking.
		/// </summary>
		private System.Web.UI.LiteralControl PlayerCardCheckbox(int studentID, int sportID)
		{
			//build checkbox html:
			string strHtml="<input type=\"checkbox\" name=\"PlayerCard_"+studentID+"\" ";
			strHtml += "value=\"1\" onclick=\"this.form.elements['action'].value=";
			strHtml += "'"+SportSiteAction.PlayerCardClick.ToString()+"'; ";
			strHtml += "this.form.elements['"+SelectedStudentString+"'].value=";
			strHtml += "'"+studentID.ToString()+"'; ";
			strHtml += "this.form.elements['"+SelectedSportString+"'].value=";
			strHtml += "'"+sportID.ToString()+"'; ";
			strHtml += "this.form.submit();\" style=\"cursor: pointer;\" title=";
			strHtml += "\"לחץ כדי לשנות סטטוס מדבקת שחקן עבור תלמיד זה\" ";
			System.DateTime date=_playerCardService.CardIssueDate(studentID, sportID);
			bool cardExist=(date.Year > 1900);
			if (cardExist)
			{
				strHtml += "checked=\"1\"";
			}
			strHtml += " />";
			return new LiteralControl(strHtml);
		}
		#endregion
		
		#region player cards
		/// <summary>
		/// change status of player card for specific student and sport
		/// </summary>
		private void ChangePlayerCardStatus()
		{
			//user must be logged in!
			if (_user.Login == null)
			{
				AddErrorMessage("אינך מורשה לבצע פעולה זו");
				return;
			}

			//get affected student and sport:
			int studentID=Tools.CIntDef(Request.Form[SelectedStudentString], -1);
			int sportID=Tools.CIntDef(Request.Form[SelectedSportString], -1);

			//verify valid numbers:
			if (studentID < 0)
			{
				AddErrorMessage("זיהוי תלמיד שגוי");
				return;
			}
			if (sportID < 0)
			{
				AddErrorMessage("ענף ספורט שגוי");
				return;
			}

			//get card status:
			int cardStatus=Tools.CIntDef(Request.Form["PlayerCard_"+studentID], 0);
			
			//decide whether to remove player card or issue new card:
			try
			{
				Sport.Entities.Student student= new Sport.Entities.Student(studentID);
				if (cardStatus == 0)
				{
					//remove player card:
					int result=_playerCardService.RemovePlayerCard(studentID, sportID);
					if (result == 1)
					{
						AddViewSuccessMsg("כרטיס שחקן נמחק בהצלחה ממאגר הנתונים עבור "+student.Name);
					}
					else
					{
						AddErrorMessage("לא קיים כרטיס שחקן עבור "+student.Name);
					}
				}
				else
				{
					//issue card:
					int[] students=new int[1] {studentID};
					_playerCardService.IssuePlayerCards(students, sportID);
					AddViewSuccessMsg("כרטיס שחקן נוסף בהצלחה עבור "+student.Name);
				}
			}
			catch (Exception e)
			{
				AddErrorMessage("שגיאה בעת הוספת/הסרת כרטיס שחקן: "+e.Message);
			}
		} //end function ChangePlayerCardStatus
		#endregion
		#endregion
		#endregion

		#region Website Management
		#region Flash News
		private void EditFlashNews()
		{
			//verify user has permissions!
			if (!Tools.IsAuthorized_News(_user.Id))
				throw new Exception("You are not authorized to use this page!");
			
			_pageCaption = "עריכת מבזקי חדשות";
			WebSiteServices.WebsiteService websiteService=
				new WebSiteServices.WebsiteService();
			websiteService.CookieContainer = Sport.Core.Session.Cookies;
			if (Tools.CIntDef(Request.QueryString["id"], -1) >= 0)
			{
				int newsID=Tools.CIntDef(Request.QueryString["id"], -1);
				WebSiteServices.FlashNewsData newsData=
					websiteService.GetFlashnewsData(newsID);
				if (newsData.ID < 0)
				{
					AddErrorMessage("זיהוי מבזק שגוי");
				}
				else
				{
					//delete?
					if (Request.QueryString["delete"] == "1")
					{
						if (Request.Form["Confirm"] == "1")
						{
							websiteService.DeleteFlashNews(newsData.ID, 
								_user.Login, _user.Password);
							AddSuccessMessage("מבזק נמחק בהצלחה ממאגר הנתונים");
						}
						else
						{
							if (Request.Form["Confirm"] == "0")
							{
								Response.Redirect("Register.aspx?action="+SportSiteAction.UpdateFlashNews);
							}
							else
							{
								AddViewText(
									Tools.BuildConfirmDeleteHtml("מבזק זה"), true);
								return;
							}
						}
					}
					else
					{
						//update?
						if (Request.Form["Update"] == "1")
						{
							ApplyFlashNewsData(newsData);
							websiteService.UpdateFlashNews(newsData, 
								_user.Login, _user.Password, _user.Id);
							AddSuccessMessage("מבזק עודכן בהצלחה");
						}
						else
						{
							AddViewText(BuildNewsTextarea(newsData));
							AddViewText("<input type=\"hidden\" name=\"Update\" "+
								"value=\"1\" />");
							return;
						}
					}
				}
			}
			if (Request.QueryString["id"] == "new")
			{

				WebSiteServices.FlashNewsData flashNewsData=
					new WebSiteServices.FlashNewsData();
				flashNewsData.ID = -1;
				if (Request.Form["FlashNews"] != null)
				{
					ApplyFlashNewsData(flashNewsData);
					
					//add new record...
					websiteService.UpdateFlashNews(flashNewsData, 
						_user.Login, _user.Password, _user.Id);
					
					AddSuccessMessage("מבזק נוסף בהצלחה");
				}
				else
				{
					AddViewText(BuildNewsTextarea(flashNewsData));
					return;
				}
			}
			WebSiteServices.FlashNewsData[] arrNews=
				websiteService.GetCurrentNews();
			AddViewText("מבזקים קיימים:<br />");
			AddViewText(BuildNewsTable(arrNews)+"<br /><br />");
			AddViewText("[<a href=\"?action="+SportSiteAction.UpdateFlashNews+
				"&id=new\" title=\"הוספת מבזק חדשות\">"+
				"הוסף מבזק</a>]");
		} //end function EditFlashNews
		
		/// <summary>
		/// reads all information from the request collection into the flash news data.
		/// </summary>
		private void ApplyFlashNewsData(WebSiteServices.FlashNewsData flashNews)
		{
			string contents=Tools.CStrDef(Request.Form["FlashNews"], "");
			
			//contents:
			flashNews.Contents = contents;
			
			//links:
			flashNews.Links = BuildLinksList(flashNews.Links);
		} //end function ApplyFlashNewsData
		
		private string BuildNewsTable(WebSiteServices.FlashNewsData[] news)
		{
			System.Text.StringBuilder sb=new System.Text.StringBuilder();
			sb.Append("<table id=\"TblFlashNews\" border=\"1\">");
			sb.Append("<tr>");
			sb.Append("<th>שעת שליחה</th>");
			sb.Append("<th>נשלח ע\"י</th>");
			sb.Append("<th>תוכן מבזק</th>");
			sb.Append("<th>&nbsp;</th>");
			sb.Append("</tr>");
			for (int i=0; i<news.Length; i++)
			{
				string strLink="<a href=\"?action="+SportSiteAction.UpdateFlashNews;
				strLink += "&id="+news[i].ID+"\" title=\"ערוך מבזק זה\">";
				sb.Append("<tr>");
				sb.Append("<td>"+strLink+news[i].Time.Hour.ToString().PadLeft(2, '0'));
				sb.Append(":"+news[i].Time.Minute.ToString().PadLeft(2, '0'));
				sb.Append("</a></td>");
				sb.Append("<td>"+strLink+news[i].User+"</a></td>");
				sb.Append("<td title=\""+news[i].Contents.Replace("\"", "&quot;"));
				sb.Append("\">"+strLink);
				sb.Append(Common.Tools.TruncateString(news[i].Contents, 20));
				sb.Append("</a></td>");
				sb.Append("<td><a href=\"?action="+SportSiteAction.UpdateFlashNews);
				sb.Append("&id="+news[i].ID+"&delete=1\" style=\"color: red;\">");
				sb.Append("[מחק מבזק זה]</a></td>");
				sb.Append("</tr>");
			}
			sb.Append("</table>");
			sb.Append("");
			return sb.ToString();
		} //end function BuildNewsTable
		
		private string BuildNewsTextarea(WebSiteServices.FlashNewsData flashNewsData)
		{
			System.Text.StringBuilder sb=new System.Text.StringBuilder();
			string strContents=flashNewsData.Contents;
			
			//contents:
			sb.Append("תוכן המבזק:<br />");
			sb.Append("<textarea id=\"TxtFlashNews\" name=\"FlashNews\" ");
			sb.Append("rows=\"15\" cols=\"60\" dir=\"rtl\">"+strContents);
			sb.Append("</textarea><br />");
			
			//links:
			if ((flashNewsData.Links != null)&&(flashNewsData.Links.Length > 0)&&
				(flashNewsData.Links[0].URL != null)&&(flashNewsData.Links[0].URL.Length > 0))
			{
				sb.Append("<iframe src=\"about:blank\" name=\"DeleteLinkFrame\" ");
				sb.Append("id=\"DeleteLinkFrame\"></iframe>");
				sb.Append("קישורים:<br />");
				for (int i=0; i<flashNewsData.Links.Length; i++)
					sb.Append(BuildFlashNewsLink(flashNewsData, i)+"<br />");
			}
			sb.Append(BuildFlashNewsLink(flashNewsData, -1)+"<br />");
			
			//submit button:
			sb.Append(Tools.BuildSubmitButton(null));
			sb.Append("<script type=\"text/javascript\">");
			sb.Append("document.getElementById(\"TxtFlashNews\").focus();");
			sb.Append("</script>");
			sb.Append("<br /><br /><br />");
			return sb.ToString();
		} //end function BuildNewsTextarea
		
		private string BuildFlashNewsLink(WebSiteServices.FlashNewsData flashNewsData, 
			int linkIndex)
		{
			return BuildLinkHTML(flashNewsData.Links, linkIndex, "flashnews", flashNewsData.ID);
		} //end function BuildFlashNewsLink
		#endregion

		#region Polls		
		private void AnswerOkButton_Command(object sender, CommandEventArgs e)
		{
			string pollAnswer = Tools.CStrDef(newAnswerTextbox.Text,"");

			SportSiteErrors returnError = SportSiteErrors.AddAnswerDatabaseError;

			// Not enough characters in answer
			if (pollAnswer.Length <= 0)
			{
				switch (e.CommandName)
				{
					case "NewAnswer":
						returnError = SportSiteErrors.AddAnswerNotEnoughCharacters;
						break;
					case "EditAnswer":
						returnError = SportSiteErrors.EditAnswerNotEnoughCharacters;
						break;
				}
				Response.Redirect("Register.aspx?action="+SportSiteAction.UpdatePolls+
					"&id=new&success=0&pollid=" + newAnswerPollId + "&error="+((int) returnError));
				return;
			}

			// Too many characters in question
			if (pollAnswer.Length > 1024)
			{
				switch (e.CommandName)
				{
					case "NewAnswer":
						returnError = SportSiteErrors.AddAnswerTooManyCharacters;
						break;
					case "EditAnswer":
						returnError = SportSiteErrors.EditAnswerTooManyCharacters;
						break;
				}
				Response.Redirect("Register.aspx?action="+SportSiteAction.UpdatePolls+
					"&id=new&success=0&pollid=" + newAnswerPollId + "&error="+((int) returnError));
				return;
			}
			
			int success = -1;
			if (e.CommandName == "NewAnswer")
			{
				success = websiteService.addAnswerToPoll(newAnswerPollId, pollAnswer,
					_user.Login, _user.Password);
			}
			else
			{
				if (e.CommandName == "EditAnswer")
				{
					success = websiteService.editAnswer(
						Tools.CIntDef(e.CommandArgument,-1), pollAnswer, 
						_user.Login, _user.Password);
				}
			}

			if (success >= 0)
			{
				switch (e.CommandName)
				{
					case "NewAnswer":
						returnError = SportSiteErrors.AddAnswerDatabaseError;
						Response.Redirect("Register.aspx?action="+SportSiteAction.UpdatePolls+
							"&id=new&success=1&pollid=" + newAnswerPollId + "&answer=1");
						break;
					case "EditAnswer":
						returnError = SportSiteErrors.EditAnswerDatabaseError;
						Response.Redirect("Register.aspx?action="+SportSiteAction.UpdatePolls+
							"&id=" + e.CommandArgument + "&edit=1&success=1&pollid=" + newAnswerPollId + "&answer=1");
						break;
				}
				
			}
			else
			{
				switch (e.CommandName)
				{
					case "NewAnswer":
						returnError = SportSiteErrors.AddAnswerDatabaseError;
						break;
					case "EditAnswer":
						returnError = SportSiteErrors.EditAnswerDatabaseError;
						break;
				}
				Response.Redirect("Register.aspx?action="+SportSiteAction.UpdatePolls+
					"&id=new&success=0&pollid=" + newAnswerPollId + "&answer=1&error="+((int) returnError));
			}
			

		}

		private void newPollOkButton_Click(object sender, EventArgs e)
		{
			int daysUntilExpiration = Tools.CIntDef(newPollExpirationTime.Text,-1);
			string pollQuestion = Tools.CStrDef(newPollTextbox.Text,"");
			DateTime expirationDate = DateTime.Today.AddDays(daysUntilExpiration);
			
			// Invalid expiration date
			if (daysUntilExpiration <= 0)
			{
				Response.Redirect("Register.aspx?action="+SportSiteAction.UpdatePolls+
					"&id=new&success=0&error="+((int) SportSiteErrors.AddPollInvalidExpirationDate));
				return;
			}
			// Not enough characters in question
			if (pollQuestion.Length <= 0)
			{
				Response.Redirect("Register.aspx?action="+SportSiteAction.UpdatePolls+
					"&id=new&success=0&error="+((int) SportSiteErrors.AddPollNotEnoughCharacters));
				return;
			}
			// Too many characters in question
			if (pollQuestion.Length > 1024)
			{
				Response.Redirect("Register.aspx?action="+SportSiteAction.UpdatePolls+
					"&id=new&success=0&error="+((int) SportSiteErrors.AddPollTooManyCharacters));
				return;
			}


			int success = websiteService.addPoll(pollQuestion, expirationDate, 
				_user.Login, _user.Password);
			if (success >= 0)
			{
				Response.Redirect("Register.aspx?action="+SportSiteAction.UpdatePolls+
					"&id=new&success=1");
			}
			else
			{
				Response.Redirect("Register.aspx?action="+SportSiteAction.UpdatePolls+
					"&id=new&success=0&error="+((int) SportSiteErrors.AddPollDatabaseError));
			}
		}

		/// <summary>
		/// Creates the actual table for the answers page
		/// </summary>
		/// <param name="existingTable"></param>
		/// <param name="pollId">Poll to get answers to</param>
		private void createAnswersTable(Table existingTable,int pollId)
		{
			// Creating the table of answers
			existingTable.Attributes.Add("border","1");
			TableRow currentRow;
			TableCell currentCell;
			
			// Create topic
			currentRow = new TableRow();
			currentRow.CssClass = "StdHeader";

			currentCell = new TableCell();
			currentCell.Attributes.Add("dir","rtl");
			currentRow.Cells.Add(currentCell);
			currentCell = new TableCell();
			currentCell.Attributes.Add("dir","rtl");
			currentRow.Cells.Add(currentCell);
			currentCell = new TableCell();
			currentCell.Text = "מספר מצביעים";
			currentCell.Attributes.Add("dir","rtl");
			currentRow.Cells.Add(currentCell);
			currentCell = new TableCell();
			currentCell.Text = "תשובות אפשריות";
			currentCell.Attributes.Add("dir","rtl");
			currentRow.Cells.Add(currentCell);

			existingTable.Rows.Add(currentRow);

			WebSiteServices.PollData currentPoll = websiteService.getPoll(pollId);
			if (currentPoll.ID < 0)
			{
				// ERROR
				return;
			}

			WebSiteServices.PollAnswerData[] pollAnswers = currentPoll.possibleAnswers;
			for (int i = 0;i < pollAnswers.Length;i++)
			{
				WebSiteServices.PollAnswerData currentAnswer = pollAnswers[i];
				
				currentRow = new TableRow();
				currentRow.CssClass = "ArialBold12";

				// Delete answer
				currentCell = new TableCell();
				if (websiteService.isCurrentUserAuthorizedToUpdate(currentPoll.creator, 
					_user.Login, _user.Password))
					currentCell.Text = "<a href=\"?action="+SportSiteAction.UpdatePolls+
						"&id="+ currentAnswer.ID +"&delete=1&answer=1&pollid=" + currentPoll.ID + "\" title=\"[מחק תשובה זו]\" style=\"color: red;\">"+
						"[מחק תשובה זו]</a>";
				else
					currentCell.Text = "[מחק תשובה זו]";	
				currentCell.Attributes.Add("dir","rtl");
				currentRow.Cells.Add(currentCell);

				// Edit poll
				currentCell = new TableCell();
				if (websiteService.isCurrentUserAuthorizedToUpdate(currentPoll.creator, 
					_user.Login, _user.Password))
					currentCell.Text = "<a href=\"?action="+SportSiteAction.UpdatePolls+
						"&id="+ currentAnswer.ID +"&pollid=" + currentAnswer.pollId + "&edit=1&answer=1\" title=\"[ערוך תשובה זו]\">"+
						"[ערוך תשובה זו]</a>";
				else
					currentCell.Text = "[ערוך תשובה זו]";	
				currentCell.Attributes.Add("dir","rtl");
				currentRow.Cells.Add(currentCell);
				
				currentCell = new TableCell();
				currentCell.Text = currentAnswer.results.Length.ToString();
				currentCell.Attributes.Add("dir","rtl");
				currentRow.Cells.Add(currentCell);

				currentCell = new TableCell();
				currentCell.Text = currentAnswer.answer;
				currentCell.Attributes.Add("dir","rtl");
				currentRow.Cells.Add(currentCell);

				existingTable.Rows.Add(currentRow);
			}
		}
		
		/// <summary>
		/// Creates the actual table for the polls page
		/// </summary>
		/// <param name="existingTable">The table to fill</param>
		private void createPollsTable(Table existingTable)
		{
			// Creating the table of existing polls
			existingTable.Attributes.Add("border","1");
			TableRow currentRow;
			TableCell currentCell;
			
			// Create topic
			currentRow = new TableRow();
			currentRow.CssClass = "StdHeader";
			
			currentCell = new TableCell();
			currentCell.Attributes.Add("dir","rtl");
			currentRow.Cells.Add(currentCell);
			currentCell = new TableCell();
			currentCell.Attributes.Add("dir","rtl");
			currentRow.Cells.Add(currentCell);
			currentCell = new TableCell();
			currentCell.Text = "תאריך תפוגה";
			currentCell.Attributes.Add("dir","rtl");
			currentRow.Cells.Add(currentCell);
			currentCell = new TableCell();
			currentCell.Text = "תאריך יצירה";
			currentCell.Attributes.Add("dir","rtl");
			currentRow.Cells.Add(currentCell);
			currentCell = new TableCell();
			currentCell.Text = "שאלת הסקר";
			currentCell.Attributes.Add("dir","rtl");
			currentRow.Cells.Add(currentCell);

			existingTable.Rows.Add(currentRow);


			WebSiteServices.WebsiteService service = new WebSiteServices.WebsiteService();
			service.CookieContainer = Sport.Core.Session.Cookies;
			WebSiteServices.PollData[] pollsArray = service.getPollsByFilter(WebSiteServices.PollReturnFilter.All);
			
			for (int i = 0;i < pollsArray.Length;i++)
			{
				WebSiteServices.PollData currentPoll = pollsArray[i];
				
				currentRow = new TableRow();
				currentRow.CssClass = "ArialBold12";

				// Delete poll
				currentCell = new TableCell();
				if (service.isCurrentUserAuthorizedToUpdate(currentPoll.creator, 
					_user.Login, _user.Password))
					currentCell.Text = "<a href=\"?action="+SportSiteAction.UpdatePolls+
						"&id="+ currentPoll.ID +"&delete=1\" title=\"[מחק סקר זה]\" style=\"color: red;\">"+
						"[מחק סקר זה]</a>";
				else
					currentCell.Text = "[מחק סקר זה]";	
				currentCell.Attributes.Add("dir","rtl");
				currentCell.Width = Unit.Percentage(20);
				currentRow.Cells.Add(currentCell);

				// Edit poll
				currentCell = new TableCell();
				if (service.isCurrentUserAuthorizedToUpdate(currentPoll.creator, 
					_user.Login, _user.Password))
					currentCell.Text = "<a href=\"?action="+SportSiteAction.UpdatePolls+
						"&id="+ currentPoll.ID +"&edit=1\" title=\"[ערוך סקר זה]\">"+
						"[ערוך סקר זה]</a>";
				else
					currentCell.Text = "[ערוך סקר זה]";	
				currentCell.Attributes.Add("dir","rtl");
				currentCell.Width = Unit.Percentage(20);
				currentRow.Cells.Add(currentCell);
				
				currentCell = new TableCell();
				currentCell.Text = currentPoll.experationDate.ToShortDateString();
				if (currentPoll.experationDate.Ticks < DateTime.Today.Ticks)
					currentCell.ForeColor = Color.Magenta;
				currentCell.Attributes.Add("dir","rtl");
				currentRow.Cells.Add(currentCell);
				currentCell = new TableCell();
				currentCell.Text = currentPoll.creationDate.ToShortDateString();
				currentCell.Attributes.Add("dir","rtl");
				currentRow.Cells.Add(currentCell);
				currentCell = new TableCell();
				currentCell.Text = currentPoll.question.ToString();
				currentCell.Attributes.Add("dir","rtl");
				currentRow.Cells.Add(currentCell);
				
				existingTable.Rows.Add(currentRow);
			}
		}

		/// <summary>
		/// Activates any input received by the EditPolls method
		/// </summary>
		private HandlePollEvents HandleEditPollsInput()
		{
			// Initialize Web service
			websiteService = new WebSiteServices.WebsiteService();
			websiteService.CookieContainer = Sport.Core.Session.Cookies;
			// Check if there are id specific queries.
			if (Tools.CIntDef(Request.QueryString["id"], -1) >= 0)
			{
				int currentId = Tools.CIntDef(Request.QueryString["id"], -1);
				// Check if delete.
				if (Request.QueryString["delete"] == "1")
				{
					// Show confirmation dialog
					if (Request.Form["Confirm"] == "1")
					{
						if (Request.QueryString["answer"] == "1")
						{
							int deleteResult = websiteService.deleteAnswer(currentId, 
								_user.Login, _user.Password);
							if (deleteResult >= 0)
								AddSuccessMessage("תשובה נמחקה בהצלחה");
							else
								AddErrorMessage("אינך רשאי למחוק תשובה זו");
							newPollId = Tools.CIntDef(Request.QueryString["pollid"],-1);
							return HandlePollEvents.EditPoll;
						}
						else
						{
							int deleteResult = websiteService.deletePoll(currentId, 
								_user.Login, _user.Password);
							if (deleteResult >= 0)
								AddSuccessMessage("סקר נמחק בהצלחה");
							else
								AddErrorMessage("אינך רשאי למחוק סקר זה");
							return HandlePollEvents.ListPolls;
						}
					}
					else
					{
						if (Request.Form["Confirm"] == "0")
							return HandlePollEvents.ListPolls;
						else
						{
							if (Request.QueryString["answer"] == "1")
								AddViewText("האם אתה בטוח שברצונך למחוק תשובה זו?<br />");
							else
								AddViewText("האם אתה בטוח שברצונך למחוק סקר זה?<br />");
							
							AddViewText("<input type=\"hidden\" name=\"Confirm\" />");
							AddViewText("<button onclick=\"this.form.Confirm.value='1'; "+
								"this.form.submit();\">אישור</button>");
							AddViewText(Tools.MultiString("&nbsp;", 20));
							AddViewText("<button onclick=\"this.form.Confirm.value='0'; "+
								"this.form.submit();\">ביטול</button>");
							return HandlePollEvents.DoNothing;
						}
					}
				}
				// check if edit
				else if (Request.QueryString["edit"] == "1")
				{
					if (Request.QueryString["answer"] == "1")
					{
						if (Request.QueryString["success"] == "1")
						{
							newPollId = Tools.CIntDef(Request.QueryString["pollid"],-1);
							return HandlePollEvents.EditPoll;
						}
						else if (Request.QueryString["success"] == "0")
						{
						}
						else
						{
							newAnswerTextbox = new TextBox();
							newAnswerTextbox.Attributes["dir"] = "rtl";
							newAnswerPollId = Tools.CIntDef(Request.QueryString["pollid"],-1);
							return HandlePollEvents.EditAnswer;
						}
					}
					else
					{
						newPollId = currentId;
						return HandlePollEvents.EditPoll;
					}
				}
			}
			// Check if create new poll
			if (Request.QueryString["id"] == "new")
			{
				if (Request.QueryString["success"] == "1")
				{
					if (Request.QueryString["answer"] == "1")
					{
						AddSuccessMessage("תשובה נוספה בהצלחה");
						newPollId = Tools.CIntDef(Request.QueryString["pollid"],-1);
						return HandlePollEvents.EditPoll;
					}
					else
					{
						AddSuccessMessage("סקר נוסף בהצלחה");
						return HandlePollEvents.ListPolls;
					}
				}

				else
				{
					if (Request.QueryString["success"] == "0")
					{
						int errorNumber = Tools.CIntDef(Request.QueryString["error"],-1);
						string errorMessage;
						SportSiteErrors possibleErrors = (SportSiteErrors)errorNumber;
						switch (possibleErrors)
						{
							case SportSiteErrors.AddPollInvalidExpirationDate:
								errorMessage = "תאריך תפוגה אינו תקין. אנא הכניסו מספר חיובי בלבד.";
								break;
							case SportSiteErrors.AddPollTooManyCharacters:
								errorMessage = "שאלת הסקר ארוכה מדי. אורך מקסימאלי הינו 1024 תווים.";
								break;
							case SportSiteErrors.AddAnswerTooManyCharacters:
								errorMessage = "תשובת הסקר ארוכה מדי. אורך מקסימאלי הינו 1024 תווים.";
								break;
							case SportSiteErrors.EditAnswerTooManyCharacters:
								errorMessage = "תשובת הסקר ארוכה מדי. אורך מקסימאלי הינו 1024 תווים.";
								break;
							case SportSiteErrors.AddPollNotEnoughCharacters:
								errorMessage = "שאלת הסקר קצרה מדי. אנא הכניסו לפחות תו אחד.";
								break;
							case SportSiteErrors.AddAnswerNotEnoughCharacters:
								errorMessage = "תשובת הסקר קצרה מדי. אנא הכניסו לפחות תו אחד.";
								break;
							case SportSiteErrors.EditAnswerNotEnoughCharacters:
								errorMessage = "תשובת הסקר קצרה מדי. אנא הכניסו לפחות תו אחד.";
								break;
							case SportSiteErrors.AddPollDatabaseError:
								errorMessage = "שגיאה במסד הנתונים. אנא נסו שנית בעוד מספר דקות.";
								break;
							case SportSiteErrors.AddAnswerDatabaseError:
								errorMessage = "שגיאה במסד הנתונים. אנא נסו שנית בעוד מספר דקות.";
								break;
							case SportSiteErrors.EditAnswerDatabaseError:
								errorMessage = "שגיאה במסד הנתונים. אנא נסו שנית בעוד מספר דקות.";
								break;
							default:
								errorMessage = "שגיאה לא ידועה";
								break;
						}

						if (Request.QueryString["answer"] == "1")
						{
							AddErrorMessage("תשובה לא התווספה. סיבה:" + errorMessage);
							newPollId = Tools.CIntDef(Request.QueryString["pollid"],-1);
							return HandlePollEvents.EditPoll;
						}
						else
						{
							AddErrorMessage("סקר לא התווסף. סיבה:" + errorMessage);
							return HandlePollEvents.ListPolls;
						}
					}
					else
					{
						if (Request.QueryString["answer"] == "1")
						{
							newAnswerPollId = Tools.CIntDef(Request.QueryString["pollid"],-1);
							BuildNewAnswerForm();
						}
						else
							BuildNewPollForm();
						return HandlePollEvents.DoNothing;
					}
				}
			}
			return HandlePollEvents.ListPolls;
		}

		private void BuildNewAnswerForm()
		{
			newAnswerOkButton.Text = "הוסף תשובה";
			newAnswerOkButton.CommandName = "NewAnswer";
			newAnswerOkButton.Command += new CommandEventHandler(AnswerOkButton_Command);
			//newAnswerOkButton.Click +=new EventHandler(newAnswerOkButton_Click);

			newAnswerQuestionLabel.Attributes.Add("dir","rtl");
			newAnswerQuestionLabel.Text = "נא להזין תשובה:<br />";
					
			newAnswerTextbox.TextMode = TextBoxMode.MultiLine;
			newAnswerTextbox.Attributes["dir"] = "rtl";
			newAnswerTextbox.ID = "PollAnswer";

			IsfMainView.AddContents(newAnswerQuestionLabel);
			IsfMainView.AddContents(newAnswerTextbox);
			IsfMainView.AddContents(newAnswerOkButton);
		}

		private void BuildNewPollForm()
		{
			newPollOkButton.Text = "הוסף סקר";
			newPollOkButton.Click += new EventHandler(newPollOkButton_Click);

			newPollQuestionLabel.Attributes.Add("dir","rtl");
			newPollQuestionLabel.Text = "נא להזין שאלה:<br />";
			newPollDateLabel.Attributes.Add("dir","rtl");
			newPollDateLabel.Text = "<br/>נא להזין מספר ימים עד לתפוגת הסקר:<br />";
			
			newPollExpirationTime.Attributes["dir"] = "rtl";

			newPollTextbox.TextMode = TextBoxMode.MultiLine;
			newPollTextbox.Attributes["dir"] = "rtl";
			newPollTextbox.ID = "PollQuestion";

			IsfMainView.AddContents(newPollQuestionLabel);
			IsfMainView.AddContents(newPollTextbox);
			IsfMainView.AddContents(newPollDateLabel);
			IsfMainView.AddContents(newPollExpirationTime);
			IsfMainView.AddContents(newPollOkButton);
		}

		private void EditPolls()
		{
			//verify user has permissions!
			if (!Tools.IsAuthorized_Polls(_user.Id))
				throw new Exception("You are not authorized to use this page!");
			
			// Initializations

			// Creating controls
			Label topic = new Label();
			Label existingLabel = new Label();
			Table existingTable = new Table();
			Label addLabel = new Label();
			
			// Adding controls to page.
			IsfMainView.AddContents(topic);
			// Adding text to the topic labels
			string topicStr = "";
			string existingLabelStr = "";
			string addLabelStr = "";
			
			HandlePollEvents eventRecieved = HandleEditPollsInput();
			switch (eventRecieved)
			{
				case HandlePollEvents.DoNothing:
					return;
				case HandlePollEvents.EditAnswer:
					topicStr = "עריכת תשובה";
					IsfMainView.AddContents(topic);
					IsfMainView.AddContents(newAnswerTextbox);
					Button editAnswerOk = new Button();
					editAnswerOk.Text = "אישור";
					editAnswerOk.CommandName = "EditAnswer";
					editAnswerOk.CommandArgument = Tools.CStrDef(Request.QueryString["id"],"");
					editAnswerOk.Command += new CommandEventHandler(AnswerOkButton_Command);
					AddViewText("<br />");
					IsfMainView.AddContents(editAnswerOk);
					return;
				case HandlePollEvents.EditPoll:
					topicStr = "עריכת סקר";
					existingLabelStr = "תשובות קיימות:";

					// Create the actual table
					createAnswersTable(existingTable,newPollId);

					addLabelStr = "[<a href=\"?action="+SportSiteAction.UpdatePolls+
						"&id=new&answer=1&pollid=" + newPollId + "\" title=\"הוספת תשובה חדשה\">"+
						"הוסף תשובה חדשה</a>]<br /><br /><a href=\"?action=" + 
						SportSiteAction.UpdatePolls + "\" style=\"\">חזור לרשימת הסקרים</a>";
					break;
				case HandlePollEvents.ListPolls:
					topicStr = "עריכת סקרים";
					existingLabelStr = "סקרים קיימים:";

					// Create the actual table
					createPollsTable(existingTable);

					addLabelStr = "[<a href=\"?action="+SportSiteAction.UpdatePolls+
						"&id=new\" title=\"הוספת סקר חדש\" style=\"\">"+
						"הוסף סקר חדש</a>]";
					break;
				
				default:
					break;
			}
					
			topic.Text = "";
			_pageCaption = topicStr;

			existingLabel.Text = existingLabelStr;
			existingLabel.Attributes.Add("dir","rtl");
			
			// Add new poll option:
			addLabel.CssClass = "ArialBold12";
			addLabel.Text = addLabelStr;
			
			IsfMainView.AddContents(existingLabel);
			IsfMainView.AddContents(existingTable);
			AddViewText("<br /><br />");
			IsfMainView.AddContents(addLabel);
		}//end function EditPolls
		#endregion
		
		#region Articles
		private void EditArticles()
		{
			//TODO: verify user has permissions!
			if (!Tools.IsAuthorized_Articles(_user.Id))
				throw new Exception("You are not authorized to use this page!");
			
			_pageCaption = "עריכת כתבות";
			
			WebSiteServices.WebsiteService websiteService=
				new WebSiteServices.WebsiteService();
			
			websiteService.CookieContainer = Sport.Core.Session.Cookies;
			if (Tools.CIntDef(Request.QueryString["id"], -1) >= 0)
			{
				int articleID=Tools.CIntDef(Request.QueryString["id"], -1);
				WebSiteServices.ArticleData articleData=
					websiteService.GetArticleData(articleID);
				if (articleData.ID < 0)
				{
					AddErrorMessage("זיהוי כתבה שגוי");
				}
				else
				{
					//delete?
					if (Request.QueryString["delete"] == "1")
					{
						if (Request.Form["Confirm"] == "1")
						{
							websiteService.DeleteArticle(articleData.ID, 
								_user.Login, _user.Password);
							AddSuccessMessage("כתבה נמחקה בהצלחה ממאגר הנתונים");
						}
						else
						{
							if (Request.Form["Confirm"] == "0")
							{
								Response.Redirect("Register.aspx?action="+SportSiteAction.UpdateArticles);
							}
							else
							{
								AddViewText(
									Tools.BuildConfirmDeleteHtml("כתבה זו"), true);
								return;
							}
						}
					}
					else
					{
						//update?
						if (Request.Form["Update"] == "1")
						{
							ApplyArticleData(articleData);
							websiteService.UpdateArticle(articleData, 
								_user.Login, _user.Password, _user.Id);
							AddSuccessMessage("כתבה עודכנה בהצלחה");
						}
						else
						{
							AddViewText(BuildArticleElements(articleData));
							return;
						}
					}
				}
			}
			if (Request.QueryString["id"] == "new")
			{
				WebSiteServices.ArticleData articleData=
					new WebSiteServices.ArticleData();
				articleData.ID = -1;
				articleData.ArticleRegion = -1;
				if (Request.Form["Caption"] != null)
				{
					ApplyArticleData(articleData);
					
					//add new record...
					websiteService.UpdateArticle(articleData, 
						_user.Login, _user.Password, _user.Id);
					
					//apply ID:
					articleData.ID = websiteService.GetLatestArticleID();
					AddSuccessMessage("כתבה נוספה בהצלחה");
				}
				else
				{
					AddViewText(BuildArticleElements(articleData));
					return;
				}
			}
			WebSiteServices.ArticleData[] arrAllArticles=
				websiteService.GetArticles(-1);
			int subArticlesCount = 0;
			foreach (WebSiteServices.ArticleData curArticleData in arrAllArticles)
				subArticlesCount += (curArticleData.IsSub)?1:0;
			string strAddArticleHTML = "[<a href=\"?action="+SportSiteAction.UpdateArticles+
				"&id=new\" title=\"הוספת כתבה\" style=\"\">"+
				"הוסף כתבה</a>]";
			string strResetSubArticles = "[<a href=\"?action="+SportSiteAction.ResetSubArticles+
				"\" onclick=\"return confirm('"+
				"האם לאפס את כל הכתבות המשניות?"+
				"');\">איפוס כתבות משניות</a>]";
			System.Text.StringBuilder sb=new System.Text.StringBuilder();
			sb.Append(strAddArticleHTML);
			if (subArticlesCount > 1)
				sb.Append("&nbsp;&nbsp;&nbsp;"+strResetSubArticles);
			sb.Append("<br />");
			ArrayList arrArticles=new ArrayList();
			string strRange=Tools.CStrDef(Request.QueryString["range"], "");
			if (strRange.Length >= 5)
			{
				int year=Tools.CIntDef(strRange.Substring(strRange.Length-4, 4), 0);
				int month=Tools.CIntDef(strRange.Substring(0, strRange.Length-4), 0);
				if ((year >= 2000)&&(month >= 1)&&(month <= 12))
				{
					string strSubCaption="כתבות "+
						SportSite.Controls.HebDateTime.HebMonthName(month)+" "+
						year.ToString();
					sb.Append(Tools.BuildPageSubCaption(strSubCaption, 
						this.Server, IsfMainView.clientSide));
					foreach (WebSiteServices.ArticleData curArticle in arrAllArticles)
					{
						string strKey=curArticle.Time.Month.ToString()+curArticle.Time.Year.ToString();
						if ((strRange == "all")||(strKey == strRange))
							arrArticles.Add(curArticle);
					}
				}
			}
			if (arrArticles.Count > 0)
			{
				sb.Append("כתבות קיימות:<br />");
				sb.Append(BuildArticlesTable((WebSiteServices.ArticleData[]) 
					arrArticles.ToArray(typeof(WebSiteServices.ArticleData)))+
					"<br /><br />");
				sb.Append(strAddArticleHTML+"<br /><br />");
			}
			
			sb.Append("בחר טווח תאריכים:<br />");
			sb.Append("<ul>");
			Hashtable tblArticlesCount=new Hashtable();
			foreach (WebSiteServices.ArticleData curArticle in arrAllArticles)
			{
				string strKey=curArticle.Time.Month.ToString()+curArticle.Time.Year.ToString();
				tblArticlesCount[strKey] = 
					Tools.CIntDef(tblArticlesCount[strKey], 0)+1;
			}
			string strLastKey="";
			foreach (WebSiteServices.ArticleData curArticle in arrAllArticles)
			{
				string strCurKey=curArticle.Time.Month.ToString()+curArticle.Time.Year.ToString();
				if (strCurKey != strLastKey)
				{
					int year=Int32.Parse(strCurKey.Substring(strCurKey.Length-4, 4));
					int month=Int32.Parse(strCurKey.Substring(0, strCurKey.Length-4), 0);
					sb.Append("<li><a href=\"?action="+SportSiteAction.UpdateArticles+
						"&range="+strCurKey+"\">"+
						SportSite.Controls.HebDateTime.HebMonthName(month)+" "+
						year.ToString()+" ("+
						Sport.Common.Tools.BuildOneOrMany("כתבה", "כתבות", 
						(int) tblArticlesCount[strCurKey], false) +")</a></li>");
				}
				strLastKey = strCurKey;
			}
			sb.Append("</ul><br /><br />");
			AddViewText(sb.ToString());
		} //end function EditArticles
		
		private string BuildArticlesTable(WebSiteServices.ArticleData[] articles)
		{
			System.Text.StringBuilder sb=new System.Text.StringBuilder();
			sb.Append("<table id=\"TblArticles\" border=\"1\">");
			sb.Append("<tr>");
			sb.Append("<th>תאריך שליחה</th>");
			sb.Append("<th>נשלח ע\"י</th>");
			sb.Append("<th>תמונות</th>");
			sb.Append("<th>כותרת</th>");
			sb.Append("<th>כותרת משנה</th>");
			sb.Append("<th>תוכן</th>");
			sb.Append("<th>&nbsp;</th>");
			sb.Append("</tr>");
			for (int i=0; i<articles.Length; i++)
			{
				string strLink="<a href=\"?action="+SportSiteAction.UpdateArticles;
				string strContents=articles[i].Contents;
				strLink += "&id="+articles[i].ID+"\" title=\"ערוך כתבה זו\">";
				sb.Append("<tr");
				Color bgColor=Color.Empty;
				if (articles[i].IsPrimary)
					bgColor = Common.Style.PrimaryArticleBgcolor;
				if (articles[i].IsSub)
					bgColor = Common.Style.SubArticleBgcolor;
				if (articles[i].IsHotLink)
					bgColor = Common.Style.HotArticleBgcolor;
				if (bgColor != Color.Empty)
					sb.Append(" style=\"background-color: "+Sport.Common.Tools.ColorToHex(bgColor)+"\"");
				sb.Append(">");
				sb.Append("<td>"+strLink+articles[i].Time.ToString("dd/MM/yyyy<br />HH:mm:ss"));
				sb.Append("</a></td>");
				sb.Append("<td>"+strLink+articles[i].User+"</a></td>");
				sb.Append("<td>");
				sb.Append((articles[i].Images == null)?"0":articles[i].Images.Length.ToString());
				sb.Append("</td>");
				sb.Append("<td>"+strLink+articles[i].Caption+"</a></td>");
				sb.Append("<td title=\""+articles[i].SubCaption.Replace("\"", "&quot;"));
				sb.Append("\">"+strLink);
				sb.Append(Common.Tools.TruncateString(articles[i].SubCaption, 50));
				sb.Append("</a></td>");
				sb.Append("<td title=\""+strContents.Replace("\"", "&quot;"));
				sb.Append("\">"+strLink);
				sb.Append(Common.Tools.TruncateString(strContents, 150));
				sb.Append("</a></td>");
				sb.Append("<td><a href=\"?action="+SportSiteAction.UpdateArticles);
				sb.Append("&id="+articles[i].ID+"&delete=1\" style=\"color: red;\">");
				sb.Append("[מחק כתבה זו]</a></td>");
				sb.Append("</tr>");
			}
			sb.Append("</table>");

			//legend:
			string strSubSpan="style=\"border: 1px solid black;";
			strSubSpan += " background-color: %bg; width: 60px; height: 20px;\"";
			sb.Append("<br /><span dir=\"rtl\">");
			sb.Append("<span "+strSubSpan.Replace("%bg", 
				Sport.Common.Tools.ColorToHex(Common.Style.HotArticleBgcolor))+">");
			sb.Append("&nbsp;");
			sb.Append("</span>&nbsp;קישור חם");
			sb.Append(Tools.MultiString("&nbsp;", 5));
			sb.Append("<span "+strSubSpan.Replace("%bg", 
				Sport.Common.Tools.ColorToHex(Common.Style.PrimaryArticleBgcolor))+">");
			sb.Append("&nbsp;");
			sb.Append("</span>&nbsp;כתבה ראשית");
			sb.Append(Tools.MultiString("&nbsp;", 5));
			sb.Append("<span "+strSubSpan.Replace("%bg", 
				Sport.Common.Tools.ColorToHex(Common.Style.SubArticleBgcolor))+">");
			sb.Append("&nbsp;");
			sb.Append("</span>&nbsp;כתבה משנית");
			sb.Append("</span>");
			sb.Append("");
			return sb.ToString();
		} //end function BuildArticlesTable
		
		private string BuildArticleElements(WebSiteServices.ArticleData articleData)
		{
			//initialize html:
			System.Text.StringBuilder sb=new System.Text.StringBuilder();
			sb.Append("<div dir=\"rtl\" align=\"right\">");
			
			//comments:
			if (EditArticleComments(sb, articleData.ID))
				return sb.ToString();
			
			ArticleImage = new System.Web.UI.HtmlControls.HtmlInputFile();
			ArticleImage.Style["display"] = "none";
			IsfMainView.AddContents(ArticleImage);
			
			//hidden flag:
			sb.Append("<input type=\"hidden\" name=\"Update\" value=\"1\" />");
			
			//Caption
			sb.Append("כותרת הכתבה: <input type=\"text\" name=\"Caption\" ");
			sb.Append("size=\"50\" maxlength=\"100\" onkeyup=\"CaptionKeyUp(this);\"");
			if (articleData.Caption != null)
				sb.Append(" value=\""+articleData.Caption.Replace("\"", "&quot;")+"\"");
			sb.Append(" /><br />");
			
			//preview:
			this.RegisterClientScriptBlock("article_caption_preview", 
				"<script type=\"text/javascript\">"+
				"function GetCaptionFlash() {"+
				" return \""+Common.Data.AppPath+"/Flash/red_title_v"+Common.Data.FlashTitlesVersion+".swf\";"+
				"}"+
				"</script>");
			sb.Append("תצוגה מקדימה: <div id=\"CaptionPreviewPanel\"></div>");
			
			//Sub Caption
			sb.Append("כותרת משנה: <textarea name=\"SubCaption\" rows=\"2\" ");
			sb.Append("cols=\"45\">");
			if (articleData.SubCaption != null)
				sb.Append(articleData.SubCaption);
			sb.Append("</textarea><br />");
			
			//Author Name:
			sb.Append("מאת: <input type=\"text\" name=\"AuthorName\" ");
			sb.Append("size=\"30\" maxlength=\"50\"");
			if (articleData.AuthorName != null)
				sb.Append(" value=\""+articleData.AuthorName+"\"");
			sb.Append(" /><br />");
			
			//hot link?
			sb.Append("<span style=\"font-weight: bold;\">");
			sb.Append("קישור חם? <input type=\"checkbox\" name=\"IsHotLink\" ");
			if (articleData.IsHotLink)
				sb.Append("checked=\"checked\" ");
			sb.Append("value=\"1\" /><br />"); //onclick=\"IsHotLinkClicked(this);\"
			sb.Append("</span>");
			
			//primary article?
			sb.Append("כתבה ראשית? <input type=\"checkbox\" name=\"IsPrimary\" ");
			if (articleData.IsPrimary)
				sb.Append("checked=\"checked\" ");
			if (articleData.IsSub)
				sb.Append("disabled=\"disabled\" ");
			sb.Append("value=\"1\" onclick=\"IsPrimaryClicked(this);\" /><br />");
			
			//primary article?
			sb.Append("כתבה משנית? <input type=\"checkbox\" name=\"IsSub\" ");
			if (articleData.IsSub)
				sb.Append("checked=\"checked\" ");
			if (articleData.IsPrimary)
				sb.Append("disabled=\"disabled\" ");
			sb.Append("value=\"1\" onclick=\"IsSubClicked(this);\" /><br />");
			
			//regional article?
			sb.Append("כתבה מחוזית? <input type=\"checkbox\" name=\"IsRegional\" ");
			if (articleData.ArticleRegion >= 0)
				sb.Append("checked=\"checked\" ");
			sb.Append("value=\"1\" />&nbsp;&nbsp;&nbsp;");
			sb.Append("מחוז: <select name=\"ArticleRegion\">");
			Sport.Data.Entity[] regionEnts=Sport.Entities.Region.Type.GetEntities(null);
			foreach (Sport.Data.Entity regionEnt in regionEnts)
			{
				Sport.Entities.Region region=new Sport.Entities.Region(regionEnt);
				if ((region.Name.IndexOf("שומרון") >= 0)||(region.Name.IndexOf("עזה") >= 0))
					continue;
				if (!region.IsNationalRegion())
				{
					sb.Append(Tools.BuildOption(region.Id.ToString(), region.Name, 
						(region.Id == articleData.ArticleRegion)));
				}
			}
			sb.Append("</select>&nbsp;&nbsp;&nbsp;");
			sb.Append(Tools.BuildRadioButton("IsClubArticle", "1", articleData.IsClubsArticle));
			sb.Append("מועדוני ספורט&nbsp;&nbsp;&nbsp;");
			sb.Append(Tools.BuildRadioButton("IsClubArticle", "0", !articleData.IsClubsArticle));
			sb.Append("אירועי ספורט<br />");
			
			//Images:
			if (articleData.ID >= 0)
			{
				string strFrameName="DeleteImageFrame";
				sb.Append("<iframe src=\"about:blank\" name=\""+strFrameName+"\" ");
				sb.Append("id=\""+strFrameName+"\" width=\"0\" height=\"0\">");
				sb.Append("</iframe><table border=\"0\"><tr>");
				sb.Append("<td align=\"right\">תמונות:</td>");
				if (Sport.Common.Tools.IsArrayEmpty(articleData.Images))
				{
					sb.Append("<td align=\"center\">---</td>");
				}
				else
				{
					Tools.Page = this.Page;
					for (int i=0; i<articleData.Images.Length; i++)
					{
						string strDescription="";
						if ((articleData.ImageDescriptions != null)&&(i < articleData.ImageDescriptions.Length))
							strDescription = Tools.CStrDef(articleData.ImageDescriptions[i], "");
						sb.Append("<td align=\"center\">");
						sb.Append("<span id=\"image_container_"+i+"\">");
						sb.Append(Tools.BuildArticleImage(articleData.Images[i], "---", 
							100, 100, this.Server, "article_image_"+i, 
							IsfMainView.clientSide)+"<br />");
						sb.Append("תיאור: <input type=\"text\" name=\"ImageDescription_"+i+"\" "+
							"value=\""+strDescription+"\" /><br />");
						if ((strDescription != null)&&(strDescription.IndexOf("\"") >= 0))
						{
							IsfMainView.clientSide.AddOnloadCommand("document.forms[0].elements[\"\"].value = "+
								"\""+strDescription.Replace("\"", "\\\"")+"\";", false, false);
						}
						sb.Append("<a href=\"?action=delimage&id="+articleData.ID);
						sb.Append("&index="+i+"\" target=\""+strFrameName+"\" ");
						sb.Append("style=\"\" ");
						sb.Append("onclick=\"return confirm('האם למחוק תמונה זו?');\"");
						sb.Append(">[מחק תמונה]</a>");
						sb.Append("</span></td>");
					}
				}
				sb.Append("</tr></table>");
			}
			sb.Append("<div id=\"new_article_image\">הוסף תמונה: ");
			sb.Append(FastControls.InputFile("NewImage", false)+"<br />");
			sb.Append("תיאור חדש: <input type=\"text\" name=\"NewImageDescription\" />");
			sb.Append("</div>");

			//Contents:
			string strContents="";
			try
			{
				strContents = Tools.GetArticleContents(articleData, this.Server);
			}
			catch (Exception ex)
			{
				AddViewText("<span style=\"display: none;\">"+ex.Message+"</span>");
				strContents = articleData.Contents;
			}
			sb.Append("תוכן הכתבה:<br />");
			sb.Append("<textarea id=\"TxtArticleContents\" name=\"ArticleContents\" ");
			sb.Append("rows=\"35\" cols=\"60\" dir=\"rtl\">");
			if (articleData.Contents != null)
				sb.Append(strContents);
			sb.Append("</textarea><br />");
			
			//comments:
			if (articleData.ID >= 0)
			{
				WebSiteServices.WebsiteService service=new WebSiteServices.WebsiteService();
				WebSiteServices.ArticleCommentData[] comments=
					service.GetArticleComments(articleData.ID);
				int commentsCount=comments.Length;
				sb.Append("<div>");
				if (commentsCount == 0)
				{
					sb.Append("לכתבה זו אין תגובות");
				}
				else
				{
					sb.Append("<a href=\"?action="+SportSiteAction.UpdateArticles+"&id="+
						articleData.ID+"&editcomments=1\">לכתבה זו "+
						Sport.Common.Tools.BuildOneOrMany("תגובה", "תגובות", 
						commentsCount, false)+" (לחץ לעריכת התגובות)</a>");
				}
				sb.Append("</div>");
			}

			//links:
			if ((articleData.Links != null)&&(articleData.Links.Length > 0)&&
				(articleData.Links[0].URL != null)&&(articleData.Links[0].URL.Length > 0))
			{
				sb.Append("<iframe src=\"about:blank\" name=\"DeleteLinkFrame\" ");
				sb.Append("id=\"DeleteLinkFrame\"></iframe>");
				sb.Append("קישורים:<br />");
				for (int i=0; i<articleData.Links.Length; i++)
					sb.Append(BuildArticleLink(articleData, i)+"<br />");
			}
			sb.Append(BuildArticleLink(articleData, -1)+"<br />");

			//attachments:
			if ((articleData.Attachments != null)&&(articleData.Attachments.Length > 0)&&
				(articleData.Attachments[0].ID >= 0))
			{
				sb.Append(BuildDeleteAttachmentFrame());
				sb.Append("קבצים מצורפים:<br />");
				Core.AttachmentData[] attachments=
					AttachmentManager.GetArticleAttachments(Server, articleData.ID);
				for (int i=0; i<articleData.Attachments.Length; i++)
				{
					int curID=articleData.Attachments[i].ID;
					int index=-1;
					for (int j=0; j<attachments.Length; j++)
					{
						if (attachments[j].ID == curID)
						{
							index = j;
							break;
						}
					}
					if (index >= 0)
					{
						sb.Append(BuildArticleAttachment(articleData, i, 
							attachments[index])+"<br />");
					}
				}
			}
			sb.Append(BuildArticleAttachment(
				articleData, -1, AttachmentData.Empty)+"<br />");
			
			//Submit and focus:
			sb.Append(Tools.BuildSubmitButton(null)+"<br /><br />");
			sb.Append("<script type=\"text/javascript\">");
			sb.Append("if (document.forms[0])");
			sb.Append("   if (document.forms[0].Caption)");
			sb.Append("      document.forms[0].Caption.focus();");
			sb.Append("</script>");
			sb.Append("</div>");
			return sb.ToString();
		} //end function BuildArticleElements
		
		private bool EditArticleComments(System.Text.StringBuilder sb, int articleID)
		{
			//need to edit?
			if ((Request.QueryString["editcomments"] != "1")||(articleID < 0))
				return false;
			
			//initialize service:
			WebSiteServices.WebsiteService service=new WebSiteServices.WebsiteService();
			
			//get all comments:
			WebSiteServices.ArticleCommentData[] comments=
				service.GetArticleComments(articleID);
			
			//need to change?
			if (Request.Form["EditCommentsActive"] == "1")
			{
				//get deleted comments
				ArrayList arrDeletedComments=new ArrayList(
					Tools.CStrDef(Request.Form["DeletedComments"], "").Split(new char[] {','}));
				
				//iterate over comments:
				foreach (WebSiteServices.ArticleCommentData comment in comments)
				{
					//get ID:
					int ID=comment.ID;
					
					//get data for the current comment:
					bool blnDelete=(arrDeletedComments.IndexOf(ID.ToString()) >= 0);
					string strAuthor=Tools.CStrDef(Request.Form["Author_"+ID], "");
					string strEmail=Tools.CStrDef(Request.Form["Email_"+ID], "");
					string strSubject=Tools.CStrDef(Request.Form["Subject_"+ID], "");
					string strContents=Tools.CStrDef(Request.Form["Contents_"+ID], "");
					
					//any difference?
					if ((blnDelete != comment.Deleted)||(strAuthor != comment.VisitorName)||
						(strEmail != comment.VisitorEmail)||(strSubject != comment.Caption)||
						(strContents != comment.Contents))
					{
						service.CookieContainer = (System.Net.CookieContainer) 
							Session["cookies"];
						comment.Deleted = blnDelete;
						comment.VisitorName = strAuthor;
						comment.VisitorEmail = strEmail;
						comment.Caption = strSubject;
						comment.Contents = strContents;
						service.UpdateArticleComment(comment, 
							_user.Login, _user.Password);
					}
				}
			}
			
			//add proper HTML.
			sb.Append("<input type=\"hidden\" name=\"EditCommentsActive\" value=\"1\" />");
			sb.Append("<table id=\"ArticleCommentsTable\" border=\"1\">");
			sb.Append("<tr>");
			sb.Append("<th>תגובה מחוקה?</th>");
			sb.Append("<th>זמן כתיבת התגובה</th>");
			sb.Append("<th>שם כותב התגובה</th>");
			sb.Append("<th>אימייל כותב התגובה</th>");
			sb.Append("<th>נושא התגובה</th>");
			sb.Append("<th>תוכן התגובה</th>");
			sb.Append("</tr>");
			for (int i=0; i<comments.Length; i++)
			{
				WebSiteServices.ArticleCommentData comment=comments[i];
				int ID=comment.ID;
				sb.Append("<tr");
				if (comment.Deleted)
					sb.Append(" style=\"background-color: red;\"");
				sb.Append(">");
				sb.Append("<td><input type=\"checkbox\" name=\"DeletedComments\" "+
					"value=\""+ID+"\"");
				if (comment.Deleted)
					sb.Append(" checked=\"checked\"");
				sb.Append(" /></td>");
				sb.Append("<td>"+comment.DatePosted.ToString("dd/MM/yyyy HH:mm:ss")+"</td>");
				sb.Append("<td><input type=\"text\" size=\"7\" name=\"Author_"+ID+"\" "+
					"value=\""+comment.VisitorName+"\" /></td>");
				sb.Append("<td><input type=\"text\" size=\"7\" name=\"Email_"+ID+"\" "+
					"value=\""+comment.VisitorEmail+"\" /></td>");
				sb.Append("<td><input type=\"text\" size=\"10\" name=\"Subject_"+ID+"\" "+
					"value=\""+comment.Caption+"\" /></td>");
				sb.Append("<td><textarea style=\"width: 108px;\" name=\"Contents_"+ID+"\" "+
					"rows=\"3\">"+comment.Contents+"</textarea></td>");
				sb.Append("</tr>");
			}
			sb.Append("</table><br />");
			sb.Append(Tools.BuildSubmitButton("")+"<br /><br />");
			sb.Append("</div>");
			
			//done.
			return true;
		}
		
		private string BuildDeleteAttachmentFrame()
		{
			string frameName="DeleteAttachmentFrame";
			string result="<iframe src=\"about:blank\" name=\""+frameName+"\" ";
			result += "id=\""+frameName+"\" width=\"0\" height=\"0\"></iframe>";
			return result;
		}
		
		private string BuildArticleLink(WebSiteServices.ArticleData article, 
			int linkIndex)
		{
			return BuildLinkHTML(article.Links, linkIndex, "article", article.ID);
		} //end function BuildArticleLink
		
		private string BuildArticleAttachment(WebSiteServices.ArticleData article, 
			int attachmentIndex, Core.AttachmentData attachment)
		{
			return BuildAttachment(article.ID, "article", attachmentIndex, attachment);
		}
		
		private string BuildAttachment(int parentID, string strParentName, 
			int attachmentIndex, Core.AttachmentData attachment)
		{
			System.Text.StringBuilder sb=new System.Text.StringBuilder();
			string strIndex="_new";
			if (attachment.ID >= 0)
				strIndex = "_"+attachment.ID.ToString();
			sb.Append("<span id=\""+strParentName+"_attachment"+strIndex+"\">");
			string strHebIndex="חדש";
			if (attachmentIndex >= 0)
				strHebIndex = Sport.Common.Tools.IntToHebrew(attachmentIndex+1, false);

			//fieldset caption:
			sb.Append("<fieldset style=\"width: "+Common.Style.AttachmentFieldsetWidth);
			sb.Append("px; background-color: "+Sport.Common.Tools.ColorToHex(
				Common.Style.AttachmentFieldsetBgColor)+";\">");
			sb.Append("<legend>קובץ מצורף "+strHebIndex+"</legend>");
			
			//attachment description:
			sb.Append("תיאור: ");
			sb.Append("<input type=\"text\" name=\"Attachment_Text"+strIndex+"\" ");
			sb.Append("maxlength=\"255\" dir=\"rtr\" value=\""+attachment.description);
			sb.Append("\" />");
			
			//delete?
			if (attachmentIndex >= 0)
			{
				sb.Append(Tools.MultiString("&nbsp;", 7)+"<a href=\"");
				sb.Append("?action=deleteattachment&id="+attachment.ID);
				sb.Append("&"+strParentName+"="+parentID+"\" id=\"DeleteAttachment_");
				sb.Append(attachment.ID+"\" style=\"\" target=\"");
				sb.Append("DeleteAttachmentFrame\" onclick=\"");
				sb.Append("return confirm('?האם לבטל קובץ זה מהכתבה');\">");
				sb.Append("[בטל קובץ זה]</a>");
			}
			
			//attachment file:
			sb.Append("<br />שם קובץ: <span style=\"font-weight: bold;\" dir=\"ltr\">");
			if (attachmentIndex < 0)
				sb.Append(FastControls.InputFile("NewAttachment", true));
			else
				sb.Append(attachment.fileName);
			sb.Append("</span>");
			
			//attachment preview:
			if (attachmentIndex >= 0)
			{
				sb.Append("<br />"+Core.AttachmentManager.BuildAttachmentHtml(
					attachment, "AttachmentPreview"+strIndex));
			}
			
			//done.
			sb.Append("</fieldset></span>");
			return sb.ToString();
		} //end function BuildArticleAttachment
		
		/// <summary>
		/// reads all information from the request collection into the article data.
		/// </summary>
		private void ApplyArticleData(WebSiteServices.ArticleData article)
		{
			string contents=Tools.CStrDef(Request.Form["ArticleContents"], "");
			string caption=Tools.CStrDef(Request.Form["Caption"], "---");
			string subCaption=Tools.CStrDef(Request.Form["SubCaption"], "---");
			string linkText=Tools.CStrDef(Request.Form["LinkText_new"], "");
			string linkURL=Tools.CStrDef(Request.Form["LinkUrl_new"], "");
			string authorName=Tools.CStrDef(Request.Form["AuthorName"], "");
			int articleRegion=Tools.CIntDef(Request.Form["ArticleRegion"], -1);
			bool isPrimary=(Request.Form["IsPrimary"] == "1");
			bool isSub=(Request.Form["IsSub"] == "1");
			bool isHotLink=(Request.Form["IsHotLink"] == "1");
			bool isRegional=(Request.Form["IsRegional"] == "1");
			bool isClubArticle=(Request.Form["IsClubArticle"] == "1");
			
			//apply captions:
			article.Caption = caption;
			article.SubCaption = subCaption;
			
			//hot link?
			article.IsHotLink = isHotLink;
			
			//primary article or sub article?
			if ((isPrimary)&&(isSub))
			{
				AddErrorMessage("כתבה לא יכולה להיות ראשית ומשנית ביחד");
			}
			else
			{
				//apply primary or sub status:
				article.IsPrimary = isPrimary;
				article.IsSub = isSub;
			}

			//regional article?
			article.ArticleRegion = (isRegional)?articleRegion:-1;
			article.IsClubsArticle = isClubArticle;
			
			//author name:
			article.AuthorName = authorName;
			
			//image descriptions?
			if (article.Images != null)
			{
				for (int i=0; i<article.Images.Length; i++)
				{
					article.ImageDescriptions[i] = Tools.CStrDef(
						Request.Form["ImageDescription_"+i], "");
				}
			}
			
			//apply image:
			System.Web.HttpPostedFile objImageFile=Request.Files["NewImage"];
			if ((objImageFile != null)&&(objImageFile.ContentLength > 0))
			{
				//verify image:
				if (Tools.IsImageFile(objImageFile.FileName) == false)
				{
					string strHtml="<script type=\"text/javascript\">";
					strHtml += "alert(\"invalid image format!\");</script>";
					Page.RegisterClientScriptBlock("InvalidFormatAlert", strHtml);
				}
				else
				{
					//rebuild the images array for the article
					ArrayList arrImages=new ArrayList();
					ArrayList arrDescriptions=new ArrayList();
					if (article.Images != null)
					{
						for (int j=0; j<article.Images.Length; j++)
						{
							if ((j+1) >= Data.MaximumArticleImages)
								break;
							if ((article.Images[j] == null)||(article.Images[j].Length == 0))
								break;
							arrImages.Add(article.Images[j]);
							if (j < article.ImageDescriptions.Length)
								arrDescriptions.Add(article.ImageDescriptions[j]);
						}
					}

					//build proper file name for the new image: (prevent cache)
					int randNum=(new Random()).Next(1, 999999);
					string strExtension=Tools.GetFileExtension(objImageFile.FileName).ToLower();
					string strImageName="article_"+article.ID+"_"+randNum+"."+strExtension;
					while (System.IO.File.Exists(Server.MapPath(Data.AppPath+"/"+Data.ArticlesImagesFolder+"/"+strImageName)))
					{
						randNum++;
						strImageName = "article_"+article.ID+"_"+randNum+"."+strExtension;
					}
					
					//add the new image to collection and save the image file:
					arrImages.Add(strImageName);
					arrDescriptions.Add(Tools.CStrDef(Request.Form["NewImageDescription"], ""));
					
					//build path:
					strImageName = Data.AppPath+"/"+Data.ArticlesImagesFolder+"/"+strImageName;
					string strImagePath=Server.MapPath(strImageName);
					
					//save:
					objImageFile.SaveAs(strImagePath);
					
					//apply new collections:
					article.Images = (string[]) arrImages.ToArray(typeof(string));
					article.ImageDescriptions = (string[]) arrDescriptions.ToArray(typeof(string));
					
					//build thumbnails:
					string strSmallThumb=Tools.CheckAndCreateThumbnail(strImageName, Common.Data.ArticleSmallImage.Width, Common.Data.ArticleSmallImage.Height, this.Server);
					string strBigThumb=Tools.CheckAndCreateThumbnail(strImageName, Common.Data.ArticleBigImage.Width, Common.Data.ArticleBigImage.Height, this.Server);
				} //end if valid image
			} //end if new image not null
			
			//links:
			article.Links = BuildLinksList(article.Links);
			
			//attachments:
			if (article.Attachments != null)
			{
				for (int attIndex=0; attIndex<article.Attachments.Length; attIndex++)
				{
					//attachment description:
					WebSiteServices.AttachmentData curAttachment=
						article.Attachments[attIndex];
					string strTemp="Attachment_Text_"+curAttachment.ID;
					article.Attachments[attIndex].Description = 
						Tools.CStrDef(Request.Form[strTemp], "");
				}
			}
			
			//new attachment?
			CheckNewAttachment(article.ID, ref article.Attachments);
			
			//update direct article contents if 1024 characters or less:
			if (contents.Length <= 1024)
			{
				article.Contents = contents;
				article.File = "";
				return;
			}

			//contents are too big for database only.... store in text file:
			string strFileName="article_"+article.ID+".txt";
			string filePath=Server.MapPath(
				Common.Data.ArticlesFolderName+"/"+strFileName);
			try
			{
				System.IO.StreamWriter objWriter=
					new System.IO.StreamWriter(filePath);
				objWriter.Write(contents);
				objWriter.Close();
				article.File = strFileName;
				article.Contents = contents.Substring(0, 256);

			}
			catch (Exception ex)
			{
				AddViewText("<span style=\"display: none;\">error while writing: "+
					filePath+": "+ex.Message+"</span>");
				article.File = "";
				article.Contents = contents.Substring(0, 1024);
			}
		} //end function ApplyArticleData
		
		/// <summary>
		/// delete image (from article) if asked to.
		/// </summary>
		private void CheckDeleteImage()
		{
			//check action:
			if (Request.QueryString["action"] != "delimage")
				return;
			
			//check permissions:
			VerifyAuthorizedUser("articles");
			
			//get article ID and image index:
			int articleID=Tools.CIntDef(Request["id"], -1);
			int imageIndex=Tools.CIntDef(Request["index"], -1);

			//verify legal value:
			if ((articleID < 0)||(imageIndex < 0))
				throw new Exception("invalid article or image index!");
			
			//initialize service:
			WebSiteServices.WebsiteService websiteService=
				new WebSiteServices.WebsiteService();

			//load article:
			WebSiteServices.ArticleData article=
				websiteService.GetArticleData(articleID);

			//verify legal article:
			if (article.ID < 0)
				throw new Exception("invalid article ID!");
			
			//verify valid index:
			if ((Sport.Common.Tools.IsArrayEmpty(article.Images))||
				(imageIndex >= article.Images.Length)||
				(article.Images[imageIndex].Length == 0))
			{
				throw new Exception("invalid image index!");
			}
			
			//set cookies:
			Sport.Core.Session.Cookies = (System.Net.CookieContainer) Session["cookies"];
			websiteService.CookieContainer = Sport.Core.Session.Cookies;

			//rebuild images array, adding all except for image to delete:
			ArrayList arrImages=new ArrayList();
			for (int i=0; i<article.Images.Length; i++)
			{
				if (i != imageIndex)
					arrImages.Add(article.Images[i]);
			}

			//apply new array:
			article.Images = (string[]) arrImages.ToArray(typeof(string));
			
			//try to update the new value:
			try
			{
				websiteService.UpdateArticle(article, _user.Login, _user.Password, 
					_user.Id);
				string containerID="image_container_"+imageIndex;
				Response.Write("<script type=\"text/javascript\">");
				Response.Write(" parent.alert(\"תמונה נמחקה בהצלחה ממאגר הנתונים\");");
				Response.Write(" parent.document.getElementById(\""+containerID+"\").style.display = 'none';");
				Response.Write(" parent.document.getElementById(\"new_article_image\").style.display = '';");
				Response.Write("</script>");
			}
			catch (Exception ex)
			{
				Response.Write("<script type=\"text/javascript\">");
				Response.Write(" parent.alert(\"error deleting image: "+ex.Message+"\");");
				Response.Write("</script>");
			}
			
			//done.
			Sport.Core.Session.Cookies = null;
			Response.End();
		} //end function CheckDeleteImage
		
		/// <summary>
		/// delete link (from article) if asked to.
		/// </summary>
		private void CheckDeleteLink()
		{
			//check action:
			if (Request.QueryString["action"] != "deletelink")
				return;
			
			//check permissions:
			if (_user.Id < 0)
				throw new Exception("you are not authorized to use this page.");
			
			//get ID of parent - article, flashnews or page.
			int articleID=Tools.CIntDef(Request["article_id"], -1);
			int flashNewsID=Tools.CIntDef(Request["flashnews_id"], -1);
			int pageID=Tools.CIntDef(Request["page_link_id"], -1);
			
			//get link index.
			int linkIndex=Tools.CIntDef(Request["index"], -1);
			
			//verify legal value:
			if (((articleID < 0)&&(flashNewsID < 0)&&(pageID < 0))||(linkIndex < 0))
				throw new Exception("not enough details!");
			
			//initialize service:
			WebSiteServices.WebsiteService websiteService=
				new WebSiteServices.WebsiteService();
			
			//get data from the database:
			WebSiteServices.ArticleData article=new WebSiteServices.ArticleData();
			WebSiteServices.FlashNewsData flashNews=new WebSiteServices.FlashNewsData();
			WebSiteServices.PageData page=new WebSiteServices.PageData();
			article.ID = -1;
			flashNews.ID = -1;
			page.ID = -1;
			if (articleID >= 0)
				article = websiteService.GetArticleData(articleID);
			if (flashNewsID >= 0)
				flashNews = websiteService.GetFlashnewsData(flashNewsID);
			if (pageID >= 0)
				page = websiteService.GetPageData(pageID);
			
			//verify legal parent:
			if ((article.ID < 0)&&(flashNews.ID < 0)&&(page.ID < 0))
				throw new Exception("invalid ID!");
			
			//get links:
			WebSiteServices.LinkData[] links=null;
			if (article.ID >= 0)
			{
				links = article.Links;
			}
			else
			{
				if (flashNews.ID >= 0)
					links = flashNews.Links;
				else
					links = page.Links;
			}
			
			//verify valid index:
			if ((Sport.Common.Tools.IsArrayEmpty(links))||
				(linkIndex >= links.Length))
			{
				throw new Exception("invalid link index!");
			}
			
			//set cookies:
			Sport.Core.Session.Cookies = (System.Net.CookieContainer) Session["cookies"];
			websiteService.CookieContainer = Sport.Core.Session.Cookies;
			
			//rebuild links array, adding all except for link to be deleted:
			ArrayList arrLinks=new ArrayList();
			for (int i=0; i<links.Length; i++)
			{
				if (i != linkIndex)
					arrLinks.Add(links[i]);
			}
			
			//apply new array:
			links = (WebSiteServices.LinkData[]) 
				arrLinks.ToArray(typeof(WebSiteServices.LinkData));
			if (article.ID >= 0)
			{
				article.Links = links;
			}
			else
			{
				if (flashNews.ID >= 0)
					flashNews.Links = links;
				else
					page.Links = links;
			}
			
			//try to update the new value:
			try
			{
				string strCaption="";
				if (article.ID >= 0)
				{
					websiteService.UpdateArticle(article, _user.Login, _user.Password, 
						_user.Id);
					strCaption = "article";
				}
				else
				{
					if (flashNews.ID >= 0)
					{
						websiteService.UpdateFlashNews(flashNews, 
							_user.Login, _user.Password, _user.Id);
						strCaption = "flashnews";
					}
					else
					{
						websiteService.UpdatePage(page, _user.Login, _user.Password, 
							_user.Id);
						strCaption = "page_link";
					}
				}

				string containerID=strCaption+"_link_"+linkIndex;
				Response.Write("<script type=\"text/javascript\">");
				Response.Write(" parent.alert(\"קישור נמחק בהצלחה ממאגר הנתונים\");");
				Response.Write(" parent.document.getElementById(\""+containerID+"\").style.display = 'none';");
				Response.Write(" for (var i="+(linkIndex+1)+"; i<99; i++) {");
				Response.Write("    var objLink=parent.document.getElementById(\"DeleteLink_\"+i);");
				Response.Write("    if (!objLink)");
				Response.Write("       break;");
				Response.Write("    var strUrl=objLink.href+\"\";");
				Response.Write("    var arrTemp=strUrl.split(\"=\");");
				Response.Write("    var index=parseInt(arrTemp[arrTemp.length-1]);");
				Response.Write("    arrTemp[arrTemp.length-1] = (index-1)+\"\";");
				Response.Write("    strUrl=\"\";");
				Response.Write("    for (var j=0; j<arrTemp.length; j++) {");
				Response.Write("       strUrl += arrTemp[j];");
				Response.Write("       if (j < (arrTemp.length-1))");
				Response.Write("          strUrl += \"=\";");
				Response.Write("    }");
				Response.Write("    objLink.href = strUrl;");
				Response.Write(" }");
				Response.Write(" ");
				Response.Write("</script>");
			}
			catch (Exception ex)
			{
				Response.Write("<script type=\"text/javascript\">");
				Response.Write(" parent.alert(\"error deleting link: "+ex.Message+"\");");
				Response.Write("</script>");
			}
			
			//done.
			Sport.Core.Session.Cookies = null;
			Response.End();
		} //end function CheckDeleteLink
		
		/// <summary>
		/// delete attachment (from article or page) if asked to.
		/// </summary>
		private void CheckDeleteAttachment()
		{
			//check action:
			if (Request.QueryString["action"] != "deleteattachment")
				return;
			
			//check permissions:
			if (Request.QueryString["article"] != null)
				VerifyAuthorizedUser("articles");
			else if (Request.QueryString["page"] != null)
				VerifyAuthorizedUser("pages");
			else if (Request.QueryString["championship"] != null)
				VerifyAuthorizedUser("pages");
			else
				throw new Exception("general error: no article or page.");
			
			//get article and page ID numbers:
			int articleID=Tools.CIntDef(Request.QueryString["article"], -1);
			int pageID=Tools.CIntDef(Request.QueryString["page"], -1);
			int championshipID=Tools.CIntDef(Request.QueryString["championship"], -1);
			
			//get attachment ID:
			int attachmentID=Tools.CIntDef(Request.QueryString["id"], -1);
			
			//verify legal value:
			if (attachmentID < 0)
				throw new Exception("invalid attachment!");
			if ((articleID < 0)&&(pageID < 0)&&(championshipID < 0))
				throw new Exception("invalid article or page!");
			
			//initialize service:
			WebSiteServices.WebsiteService websiteService=
				new WebSiteServices.WebsiteService();
			
			if (championshipID >= 0)
			{
				//remove from championship
				websiteService.RemoveChampionshipAttachment(championshipID, attachmentID);
				string containerID="championship_attachment_"+attachmentID;
				Response.Write("<script type=\"text/javascript\">");
				Response.Write(" parent.alert(\"קובץ מצורף הוסר בהצלחה מהאליפות\");");
				Response.Write(" parent.document.getElementById(\""+containerID+"\").style.display = 'none';");
				Response.Write("</script>");
				
				//done.
				Response.End();
				return;
			}
			
			//load article or page:
			WebSiteServices.ArticleData article=new WebSiteServices.ArticleData();
			WebSiteServices.PageData page=new WebSiteServices.PageData();
			article.ID = -1;
			page.ID = -1;
			if (articleID >= 0)
				article = websiteService.GetArticleData(articleID);
			else
				page = websiteService.GetPageData(pageID);
				
			//verify legal article or page:
			if ((article.ID < 0)&&(page.ID < 0))
				throw new Exception("invalid article or page ID!");
			
			//search for attachment in the article or page:
			int index=-1;
			WebSiteServices.AttachmentData[] attachments=
				(article.ID >= 0)?article.Attachments:page.Attachments;
			if (attachments != null)
			{
				for (int i=0; i<attachments.Length; i++)
				{
					if (attachments[i].ID == attachmentID)
					{
						index = i;
						break;
					}
				}
			}
			
			//verify valid index:
			if (index < 0)
				throw new Exception("attachment does not belong to that article or page!");
			
			//rebuild attachments array, adding all except for deleted attachment:
			ArrayList attList=new ArrayList();
			foreach (WebSiteServices.AttachmentData attachment in attachments)
			{
				if (attachment.ID != attachmentID)
					attList.Add(attachment);
			}
			
			//apply new array:
			if (article.ID >= 0)
			{
				article.Attachments = (WebSiteServices.AttachmentData[]) 
					attList.ToArray(typeof(WebSiteServices.AttachmentData));
			}
			else
			{
				page.Attachments = (WebSiteServices.AttachmentData[]) 
					attList.ToArray(typeof(WebSiteServices.AttachmentData));
			}
			
			//try to update the new value:
			try
			{
				string containerID="";
				if (article.ID >= 0)
				{
					websiteService.UpdateArticle(article, _user.Login, _user.Password, 
						_user.Id);
					containerID="article_attachment_"+attachmentID;
				}
				else
				{
					websiteService.UpdatePage(page, _user.Login, _user.Password, _user.Id);
					containerID="page_attachment_"+attachmentID;
				}
				Response.Write("<script type=\"text/javascript\">");
				Response.Write(" parent.alert(\"קובץ מצורף הוסר בהצלחה מהכתבה\");");
				Response.Write(" parent.document.getElementById(\""+containerID+"\").style.display = 'none';");
				Response.Write("</script>");
			}
			catch (Exception ex)
			{
				Response.Write("<script type=\"text/javascript\">");
				Response.Write(" parent.alert(\"error deleting attachment: "+ex.Message+"\");");
				Response.Write("</script>");
			}
			
			//done.
			Response.End();
		} //end function CheckDeleteAttachment
		
		/// <summary>
		/// throw exception if user is not logged in or have no
		/// permissions to edit articles.
		/// </summary>
		private void VerifyAuthorizedUser(string authorizedForWhat)
		{
			//logged in?
			if (Session[UserManager.SessionKey] == null)
				throw new Exception("session expired");

			//correct data?
			if (!(Session[UserManager.SessionKey] is UserData))
				throw new Exception("invalid user data!");
			
			//get user data:
			UserData curUser=(UserData) Session[UserManager.SessionKey];

			//correct ID?
			if (curUser.Id < 0)
				throw new Exception("invalid user!");
			
			//maybe no permissions.....
			if (authorizedForWhat == "articles")
			{
				if (!Tools.IsAuthorized_Articles(curUser.Id))
					throw new Exception("You are not authorized to use this page!");
			}
			else
			{
				if (authorizedForWhat == "pages")
				{
					if (!Tools.IsAuthorized_Pages(curUser.Id))
						throw new Exception("You are not authorized to use this page!");
				}
			}
		} //end function VerifyAuthorizedUser
		#endregion
		
		#region Links
		private void EditLinks()
		{
			//verify user has permissions!
			if (!Tools.IsAuthorized_Articles(_user.Id))
				throw new Exception("You are not authorized to use this page!");
			
			_pageCaption = "עריכת קישורים";

			//apply user data:
            ApplyLinkData();

			//get all the links:
			LinkManager.LinkGroupData[] linkGroups=LinkManager.GetLinkGroups(Server);
			
			//display link groups
			AddViewText("<input type=\"hidden\" name=\"UpdateLinks\" value=\"1\" />"+
				"קבוצות קישורים: "+
				Tools.MakeHtmlBold("(על מנת למחוק השאר ערך ריק)")+"<br />");
			ArticleImage = new System.Web.UI.HtmlControls.HtmlInputFile();
			ArticleImage.Style["display"] = "none";
			IsfMainView.AddContents(ArticleImage);
			for (int groupIndex=0; groupIndex<linkGroups.Length; groupIndex++)
			{
				AddViewText(BuildLinkGroup(linkGroups[groupIndex], groupIndex)+"<br />");
			}

			//new group
			AddViewText(BuildLinkGroup(LinkManager.LinkGroupData.Empty, -1)+"<br />");

			//done.
			AddViewText("<br />"+Tools.BuildSubmitButton(null)+"<br /><br /><br />");
		} //end function EditLinks
		
		private void ApplyLinkData()
		{
			//maybe first visit?
			if (Request.Form["UpdateLinks"] != "1")
				return;
			
			//get all the link groups:
			ArrayList arrLinkGroups=new ArrayList(LinkManager.GetLinkGroups(Server));
			
			//initialize flag:
			bool changed=false;

			//check existing data:
			for (int i=0; i<arrLinkGroups.Count; i++)
			{
				//links order:
				string strOrder=Tools.CStrDef(Request.Form["LinksOrder_"+i], "");
				if (strOrder.Length > 0)
				{
					Tools.WriteIniValue("Links", "GroupOrder_"+i, strOrder, this.Server);
				}
				
				//get current group:
				LinkManager.LinkGroupData groupData=
					(LinkManager.LinkGroupData) arrLinkGroups[i];

				//update caption?
				string curCaption=Tools.CStrDef(Request.Form["GroupCaption_"+i], "");
				if (curCaption != groupData.Caption)
				{
					//raise boolean flag and apply new caption:
					changed = true;
					groupData.Caption = curCaption;
				}

				//group links:
				ArrayList links=new ArrayList(groupData.Links);
				for (int j=0; j<links.Count; j++)
				{
					//get current link:
					LinkManager.LinkData linkData=(LinkManager.LinkData) links[j];

					//update URL or text?
					string curText=Tools.CStrDef(Request.Form["LinkText_"+i+"_"+j], "");
					string curURL=Tools.CStrDef(Request.Form["LinkUrl_"+i+"_"+j], "");
					if ((curText != linkData.Text)||(curURL != linkData.URL))
					{
						//raise boolean flag and apply new data:
						changed = true;
						linkData.Text = curText;
						linkData.URL = curURL;
					}
					
					//logo?
					if (curURL.Length > 0)
					{
						HttpPostedFile objImageFile=Request.Files["LinkLogo_"+i+"_"+j];
						if ((objImageFile != null)&&(objImageFile.ContentLength > 0))
						{
							Core.LinkManager.SaveLinkLogo(curURL, objImageFile, this.Server, 
								i.ToString(), j.ToString(), this.Page, curText);
						}
					}
					
					//apply changes:
					links[j] = linkData;
				} //end loop over the links of the current group
				
				//new link?
				string strNewText=Tools.CStrDef(Request.Form["LinkText_"+i+"_new"], "");
				string strNewURL=Tools.CStrDef(Request.Form["LinkURL_"+i+"_new"], "");
				HttpPostedFile objNewImageFile=Request.Files["LinkLogo_"+i+"_new"];
				if ((strNewText.Length*strNewURL.Length) > 0)
				{
					//raise boolean flag:
					changed = true;

					//build new link:
					LinkManager.LinkData link=new LinkManager.LinkData();

					//apply new data:
					link.Text = strNewText;
					link.URL = strNewURL;
					
					//add to array:
					links.Add(link);
					
					//logo?
					if ((objNewImageFile != null)&&(objNewImageFile.ContentLength > 0))
					{
						Core.LinkManager.SaveLinkLogo(strNewURL, objNewImageFile, this.Server, 
							i.ToString(), "new", this.Page, strNewText);
					}
				} //end if new link data exists

				//apply array in the group data:
				groupData.Links = (LinkManager.LinkData[]) 
					links.ToArray(typeof(LinkManager.LinkData));

				arrLinkGroups[i] = groupData;
			} //end loop over all the groups
			
			//new group?
			string strNewCaption=Tools.CStrDef(Request.Form["GroupCaption_new"], "");
			if (strNewCaption.Length > 0)
			{
				//raise boolean flag:
				changed = true;

				//build new group and apply caption:
				LinkManager.LinkGroupData groupData=new LinkManager.LinkGroupData();
				groupData.Caption = strNewCaption;

				//new link?
				string strLinkUrl=Tools.CStrDef(Request.Form["LinkUrl_new_new"], "");
				string strLinkText=Tools.CStrDef(Request.Form["LinkText_new_new"], "");
				if ((strLinkUrl.Length*strLinkText.Length) > 0)
				{
					//build new link:
					LinkManager.LinkData link=new LinkManager.LinkData();
					link.URL = strLinkUrl;
					link.Text = strLinkText;

					//add to group:
					groupData.Links = new LinkManager.LinkData[] {link};
				}

				//add to array:
				arrLinkGroups.Add(groupData);
			}
			
			//update link groups?
			if (changed == true)
			{
				LinkManager.UpdateLinkGroups((LinkManager.LinkGroupData[]) 
					arrLinkGroups.ToArray(typeof(LinkManager.LinkGroupData)), Server);
			}
		} //end function ApplyLinkData
		
		private string BuildLinkGroup(LinkManager.LinkGroupData group, int groupIndex)
		{
			System.Text.StringBuilder sb=new System.Text.StringBuilder();
			
			string strIndex="_new";
			if (groupIndex >= 0)
				strIndex = "_"+groupIndex.ToString();
			sb.Append("<span id=\"link_group"+strIndex+"\">");
			string strHebIndex="חדשה";
			if (groupIndex >= 0)
				strHebIndex = Sport.Common.Tools.IntToHebrew(groupIndex+1, true);
			
			//fieldset caption:
			sb.Append("<fieldset>");
			sb.Append("<legend>קבוצה "+strHebIndex+"</legend>");
			
			//group caption:
			sb.Append("כותרת: ");
			sb.Append("<input type=\"text\" name=\"GroupCaption"+strIndex+"\" ");
			sb.Append("maxlength=\"128\" size=\"15\" dir=\"rtl\" default=\"1\" ");
			sb.Append("value=\""+group.Caption+"\" /><br /><br />");
			
			//links...
			sb.Append("קישורים: <br />");
			ArrayList arrTexts=new ArrayList();
			if (group.Links != null)
			{
				for (int linkIndex=0; linkIndex<group.Links.Length; linkIndex++)
				{
					Core.LinkManager.LinkData curLink=group.Links[linkIndex];
					sb.Append(BuildLinkHtml(curLink, linkIndex, groupIndex)+"<br />");
					arrTexts.Add(curLink.Text);
				}
			}
			
			//new link:
			sb.Append(BuildLinkHtml(LinkManager.LinkData.Empty, -1, groupIndex));
			
			//links order:
			if ((groupIndex >= 0)&&(arrTexts.Count > 1))
			{
				int[] order=Tools.ReadIniOrder("Links", "GroupOrder"+strIndex, 
					arrTexts.Count, this.Server);
				arrTexts = Tools.ReArrangeArray(arrTexts, order);
				sb.Append("<br />סדר הופעה של הקישורים:<br />");
				sb.Append(Tools.BuildUpDownCombo("LinksOrder"+strIndex, 
					"RelevantLinks"+strIndex, 
					Sport.Common.Tools.ToStringArray(order), 
					(string[]) arrTexts.ToArray(typeof(string)), ",", 
					"קישור", true)+"<br /><br />");
			}

			
			//done.
			sb.Append("</fieldset></span>");
			return sb.ToString();
		} //end function BuildLinkGroup
		
		private string BuildLinkHtml(LinkManager.LinkData link, int linkIndex, 
			int groupIndex)
		{
			System.Text.StringBuilder sb=new System.Text.StringBuilder();
			
			string strIndex="_"+((groupIndex >= 0)?groupIndex.ToString():"new")+"_";
			strIndex += (linkIndex < 0)?"new":linkIndex.ToString();
			sb.Append("<span id=\"link"+strIndex+"\">");
			string strHebIndex="חדש";
			if (linkIndex >= 0)
				strHebIndex = Sport.Common.Tools.IntToHebrew(linkIndex+1, false);
			
			//fieldset caption:
			sb.Append("<fieldset style=\"width: "+(Common.Style.LinkFieldsetWidth-50));
			sb.Append("px; background-color: "+Sport.Common.Tools.ColorToHex(
				Common.Style.LinkFieldsetBgColor)+";\">");
			sb.Append("<legend>קישור "+strHebIndex+"</legend>");
			
			//link URL
			string strKeyUp="onkeyup=\"ApplyLinkPreview(this, ' "+Data.LinkSuffix+"');\"";
			sb.Append("כתובת: ");
			sb.Append("<input type=\"text\" name=\"LinkUrl"+strIndex+"\" ");
			sb.Append("maxlength=\"128\" size=\"50\" dir=\"ltr\" ");
			sb.Append("value=\""+link.URL+"\" "+strKeyUp+" />");
			
			//link logo:
			sb.Append("<br />לוגו: "+Core.LinkManager.GetLinkLogoHtml(link.URL, this.Server, "<br />"));
			sb.Append("תמונה חדשה: <input type=\"file\" name=\"LinkLogo"+strIndex+"\" />");
			
			//link text:
			sb.Append("<br />טקסט: ");
			sb.Append("<input type=\"text\" name=\"LinkText"+strIndex+"\" ");
			sb.Append("maxlength=\"50\" value=\""+link.Text.Replace("\"", "&quot;"));
			sb.Append("\" dir=\"rtl\" "+strKeyUp+" />");
			
			//link preview:
			sb.Append("<br /><a id=\"LinkPreview"+strIndex+"\" ");
			sb.Append("class=\"DocumentLink\" target=\"_blank\"");
			if ((link.URL != null)&&(link.URL.Length > 0))
				sb.Append(" href=\""+link.URL+"\"");
			sb.Append(">");
			if ((link.Text != null)&&(link.Text.Length > 0))
				sb.Append(link.Text+" "+Data.LinkSuffix);
			sb.Append("</a>");
			
			//done.
			sb.Append("</fieldset></span>");
			return sb.ToString();
		} //end function BuildLinkHtml
		#endregion
		
		#region Pages
		private void EditPages()
		{
			//verify user has permissions!
			if (!Tools.IsAuthorized_Pages(_user.Id))
				throw new Exception("You are not authorized to use this page!");
			
			//put initial data:
			_pageCaption = "עריכת תוכן עמודי אינטרנט";
			WebSiteServices.WebsiteService websiteService=
				new WebSiteServices.WebsiteService();
			websiteService.CookieContainer = Sport.Core.Session.Cookies;
			
			//need to edit?
			if (Request.QueryString["id"] != null)
			{
				if (Request.QueryString["id"] == "ClubRegister")
				{
					UpdatePageData(Request.QueryString["id"]);
					return;
				}
				else
				{
					int pageID=Tools.CIntDef(Request.QueryString["id"], -1);
					if (pageID < 0)
					{
						AddErrorMessage("זיהוי עמוד שגוי");
					}
					else
					{
						WebSiteServices.PageData page=websiteService.GetPageData(pageID);
						if (page.ID < 0)
						{
							AddErrorMessage("עמוד מבוקש לא קיים במאגר הנתונים");
						}
						else
						{
							UpdatePageData(page);
							return;
						}
					}
				}
			}
			
			//get all pages:
			WebSiteServices.PageData[] arrPages=PageManager.GetDynamicPages(_user);
			
			//display on screen:
			AddViewText("רשימת העמודים הדינאמיים:<br />");
			AddViewText(BuildPagesTable(arrPages), true);
		} //end function EditPages

		private void UpdatePageData(WebSiteServices.PageData page, string strTextFileName)
		{
			//initialize html container:
			System.Text.StringBuilder sb=new System.Text.StringBuilder();
			sb.Append("<div id=\"PageData_"+page.ID+"\" dir=\"rtl\">");
			
			//general stuff:
			if (page.ID >= 0)
			{
				ArticleImage = new System.Web.UI.HtmlControls.HtmlInputFile();
				ArticleImage.Style["display"] = "none";
				IsfMainView.AddContents(ArticleImage);
			}
			IsfMainView.clientSide.AddOnloadCommand("IterateTextAreas();", true);

			//find ancestors of the given page:
			Data.DynamicLinkData[] ancestors=null;
			string strPageContents=null;
			string strFilePath=null;
			if (strTextFileName != null)
				strFilePath = Server.MapPath(strTextFileName+".txt");
			if (page.ID >= 0)
			{
				if (!Directory.Exists(Server.MapPath("Public/DynamicPages")))
					System.IO.Directory.CreateDirectory(Server.MapPath("Public/DynamicPages"));
				strFilePath = Server.MapPath("Public/DynamicPages/"+page.ID+".txt");
				ancestors = PageManager.GetPageAncestors(page);
				//print ancestors path:
				sb.Append("<h3>");
				for (int i=0; i<ancestors.Length; i++)
				{
					sb.Append(ancestors[i].Text);
					if (i < (ancestors.Length-1))
						sb.Append(" > ");//&#8656; &larr; stupid IE won't display it.
				}
				sb.Append("</h3>");
				strPageContents = page.Contents;
				if (File.Exists(strFilePath))
					strPageContents = String.Join("\n", Tools.ReadTextFile(strFilePath, true));
			}
			else
			{
				string[] arrLines=Tools.ReadTextFile(strFilePath, true);
				if (arrLines != null)
					strPageContents = String.Join("\n", arrLines);
			}
			
			//maybe user sent new data...
			if (Request.Form["PageContents"] != null)
			{
				//update contents:
				strPageContents = Tools.CStrDef(Request.Form["PageContents"], "");
				if (page.ID < 0)
				{
					if (strFilePath != null)
					{
						Tools.CreateTextFile(strFilePath, strPageContents, true);
					}
				}
				else
				{
					if (strPageContents.Length <= 1024)
					{
						page.Contents = strPageContents;
						if (File.Exists(strFilePath))
							File.Delete(strFilePath);
					}
					else
					{
						page.Contents = "";
						Tools.CreateTextFile(strFilePath, strPageContents, true);
					}
					
					//update author name and author title:
					page.AuthorName = Tools.CStrDef(Request["PageAuthor"], "");
					page.AuthorTitle = Tools.CStrDef(Request["AuthorTitle"], "");
					
					//links:
					page.Links = BuildLinksList(page.Links);
					
					//update page attachments:
					if (page.Attachments != null)
					{
						foreach (WebSiteServices.AttachmentData attachment in 
							page.Attachments)
						{
							attachment.Description = Tools.CStrDef(
								Request.Form["Attachment_Text_"+attachment.ID], "");
						}
					}
					
					//new attachment?
					CheckNewAttachment(page.ID, ref page.Attachments);
					
					//new order?
					if (Request.Form["AttachmentsOrder"] != null)
					{
						string strNewOrder=Tools.CStrDef(Request.Form["AttachmentsOrder"], "");
						ArrayList arrPageAttachments=new ArrayList();
						string[] arrNewOrder=strNewOrder.Split(new char[] {','});
						for (int i=0; i<arrNewOrder.Length; i++)
						{
							int curAttachmentID=Tools.CIntDef(arrNewOrder[i], -1);
							if (curAttachmentID < 0)
								continue;
							foreach (WebSiteServices.AttachmentData attData in page.Attachments)
							{
								if (attData.ID == curAttachmentID)
								{
									if (arrPageAttachments.IndexOf(attData) < 0)
										arrPageAttachments.Add(attData);
									break;
								}
							}
						}
						if (arrPageAttachments.Count == page.Attachments.Length)
						{
							page.Attachments = (WebSiteServices.AttachmentData[])
								arrPageAttachments.ToArray(typeof(WebSiteServices.AttachmentData));
						}
					}
					
					//apply new data:
					WebSiteServices.WebsiteService websiteService=
						new WebSiteServices.WebsiteService();
					websiteService.CookieContainer = Sport.Core.Session.Cookies;
					websiteService.UpdatePage(page, _user.Login, _user.Password, _user.Id);
					
					//read again:
					page = websiteService.GetPageData(page.ID);
					
					//FAQ page?
					if (Request.Form["FAQ"] == "1")
						Core.FAQ.DynamicPage = page.ID;
				}
				AddSuccessMessage("<br />תוכן עמוד עודכן בהצלחה<br />");
			}

			//page contents:
			sb.Append("תוכן:<br />");
			sb.Append("<textarea name=\"PageContents\" cols=\"60\" rows=\"20\"");
			if (page.Index == ((int) NavBarLink.JHWC))
				sb.Append(" dir=\"ltr\"");
			sb.Append(">");
			if (strPageContents != null)
				sb.Append(strPageContents);
			sb.Append("</textarea><br />");
			
			if (page.ID >= 0)
			{
				//page author:
				sb.Append("שם נושא דברים: <input name=\"PageAuthor\"");
				if (page.AuthorName != null)
					sb.Append(" value=\""+Server.HtmlEncode(page.AuthorName)+"\"");
				sb.Append(" /><br />");
			
				//author title:
				sb.Append("תואר נושא הדברים: <input name=\"AuthorTitle\"");
				if (page.AuthorTitle != null)
					sb.Append(" value=\""+Server.HtmlEncode(page.AuthorTitle)+"\"");
				sb.Append(" /><br />");
				
				//FAQ page?
				if (page.ID >= 0)
				{
					sb.Append("<input type=\"checkbox\" name=\"FAQ\" value=\"1\" ");
					if (Core.FAQ.DynamicPage == page.ID)
					{
						sb.Append("checked=\"checked\" ");
					}
					sb.Append("/>&nbsp;פורום שאלות ותשובות?<br />");
				}
				
				//links:
				if ((page.Links != null)&&(page.Links.Length > 0)&&
					(page.Links[0].URL != null)&&(page.Links[0].URL.Length > 0))
				{
					sb.Append("<iframe src=\"about:blank\" name=\"DeleteLinkFrame\" ");
					sb.Append("id=\"DeleteLinkFrame\"></iframe>");
					sb.Append("קישורים:<br />");
					for (int i=0; i<page.Links.Length; i++)
						sb.Append(BuildLinkHTML(page.Links, i, "page_link", page.ID)+"<br />");
				}
				sb.Append(BuildLinkHTML(page.Links, -1, "page_link", page.ID)+"<br />");
				
				//attachments:
				sb.Append("קבצים מצורפים:<br />");
				if (page.Attachments != null)
				{
					sb.Append(BuildDeleteAttachmentFrame());
					for (int attIndex=0; attIndex<page.Attachments.Length; attIndex++)
						sb.Append(BuildPageAttachment(page, attIndex)+"<br />");
				}

				//new attachment:
				sb.Append(BuildPageAttachment(page, -1)+"<br />");
				
				if ((page.Attachments != null)&&(page.Attachments.Length > 1))
				{
					string[] arrValues=new string[page.Attachments.Length];
					string[] arrTexts=new string[page.Attachments.Length];
					for (int attIndex=0; attIndex<page.Attachments.Length; attIndex++)
					{
						WebSiteServices.AttachmentData curData=page.Attachments[attIndex];
						arrValues[attIndex] = curData.ID.ToString();
						arrTexts[attIndex] = curData.Description;
					}
					sb.Append("סדר הופעה של קבצים מצורפים:<br />");
					sb.Append(Tools.BuildUpDownCombo("AttachmentsOrder", 
						"PageAttachments", arrValues, arrTexts, 
						",", "קובץ", true)+"<br /><br />");
				}
			}
			
			//submit:
			sb.Append(Tools.BuildSubmitButton("")+"<br />");
			
			//link for going back to list of pages:
			sb.Append("<br /><a href=\"?action="+SportSiteAction.UpdatePages);
			sb.Append("&nnn="+Tools.MakeRandom(1, 999999)+"\" ");
			sb.Append("style=\"\">");
			sb.Append("[חזרה לרשימת עמודים]</a>");
			
			//add as pure html to the page:
			sb.Append("</div>");
			AddViewText(sb.ToString(), true);
		}
		
		private void UpdatePageData(WebSiteServices.PageData page)
		{
			UpdatePageData(page, null);
		}
		
		private void UpdatePageData(string strTextFileName)
		{
			WebSiteServices.PageData page=new WebSiteServices.PageData();
			page.ID = -1;
			UpdatePageData(page, strTextFileName);
		}
		
		private string BuildPagesTable(WebSiteServices.PageData[] pages)
		{
			System.Text.StringBuilder sb=new System.Text.StringBuilder();
			ArticleImage = new System.Web.UI.HtmlControls.HtmlInputFile();
			ArticleImage.Style["display"] = "none";
			IsfMainView.AddContents(ArticleImage);
			sb.Append("<div id=\"TblDynamicPages\" dir=\"rtl\">");
			sb.Append("<ul>");
			foreach (Data.DynamicLinkData linkData in Data.DynamicLinks)
				sb.Append(BuildDynamicLinkHtml(linkData, pages));
			sb.Append("<li><a href=\"?action="+SportSiteAction.UpdatePages);
			sb.Append("&id=ClubRegister\" style=\"\">רישום מועדון</a></li>");
			sb.Append("</ul>");
			sb.Append("</div>");
			sb.Append("");
			return sb.ToString();
		}

		private string BuildDynamicLinkHtml(Data.DynamicLinkData linkData, 
			WebSiteServices.PageData[] pages)
		{
			System.Text.StringBuilder sb=new System.Text.StringBuilder();
			sb.Append("<li>");
			if ((linkData.Children == null)||(linkData.Children.Length == 0))
			{
				WebSiteServices.PageData pageData=PageManager.FindPage(pages, linkData);
				if (pageData.ID >= 0)
				{
					sb.Append("<a href=\"?action="+SportSiteAction.UpdatePages);
					sb.Append("&id="+pageData.ID+"\" style=\"\">");
				}
			}
			sb.Append(linkData.Text);
			if ((linkData.Children == null)||(linkData.Children.Length == 0))
			{
				sb.Append("</a>");
			}
			else
			{
				sb.Append("<ul>");
				sb.Append("<li>");
				sb.Append(BuildPageLogo(linkData));
				sb.Append("</li>");
				for (int i=0; i<linkData.Children.Length; i++)
					sb.Append(BuildDynamicLinkHtml(linkData.Children[i], pages));
				sb.Append("</ul>");
			}
			sb.Append("</li>");
			return sb.ToString();
		}
		
		private string BuildPageLogo(Common.Data.DynamicLinkData linkData)
		{
			//initialize result string:
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			
			//build path for the folder:
			string strFolderName = Common.Data.AppPath+"/Images/Logos";
			string strFolderPath = Server.MapPath(strFolderName);
			
			//build file name:
			string strPageText = Tools.MakeValidFileName(linkData.Text);
			
			//create if does not exist:
			if (!System.IO.Directory.Exists(strFolderPath))
				System.IO.Directory.CreateDirectory(strFolderPath);
			
			//add proper path:
			strFolderName += "/page_"+strPageText;
			strFolderPath = Server.MapPath(strFolderName);
			
			//need to create?
			if (!System.IO.Directory.Exists(strFolderPath))
				System.IO.Directory.CreateDirectory(strFolderPath);
			
			//got new logo?
			System.Web.HttpPostedFile objPageLogo = 
				Request.Files["logo_"+strPageText];
			string[] arrFileNames=null;
			if ((objPageLogo != null)&&(objPageLogo.ContentLength > 0))
			{
				arrFileNames = System.IO.Directory.GetFiles(strFolderPath);
				foreach (string strFileName in arrFileNames)
					System.IO.File.Delete(strFileName);
				string strImageName = strFolderName+"/"+
					System.IO.Path.GetFileName(objPageLogo.FileName);
				objPageLogo.SaveAs(Server.MapPath(strImageName));
			}
				
			//search for existing logo...
			string strLogoImage="";
			arrFileNames = System.IO.Directory.GetFiles(strFolderPath);
			if ((arrFileNames != null)&&(arrFileNames.Length > 0))
				strLogoImage = strFolderName+"/"+System.IO.Path.GetFileName(arrFileNames[0]);
				
			//exisiting logo:
			sb.Append("לוגו קיים: ");
			if (strLogoImage.Length > 0)
			{
				strLogoImage = Tools.CheckAndCreateThumbnail(
					strLogoImage, 67, 80, this.Server);
				sb.Append("<img src=\""+strLogoImage+"\" />");
			}
			sb.Append("<br />");
			
			//new logo:
			sb.Append("<input type=\"file\" name=\"logo_"+strPageText+"\" /><br />");
			sb.Append("<button type=\"submit\">שלח לוגו חדש</button>");
			
			//done.
			return sb.ToString();
		}
		
		private string BuildPageAttachment(WebSiteServices.PageData page, 
			int attachmentIndex)
		{
			System.Text.StringBuilder sb=new System.Text.StringBuilder();
			
			WebSiteServices.AttachmentData attachment=
				new WebSiteServices.AttachmentData();
			attachment.ID = -1;
			if (attachmentIndex >= 0)
				attachment=page.Attachments[attachmentIndex];
			
			string strIndex="_";
			strIndex += (attachment.ID >= 0)?attachment.ID.ToString():"new";
			
			string strHebIndex="חדש";
			if (attachmentIndex >= 0)
				strHebIndex = Sport.Common.Tools.IntToHebrew(attachmentIndex+1, false);
			
			sb.Append("<span id=\"page_attachment"+strIndex+"\">");
			
			//fieldset caption:
			sb.Append("<fieldset style=\"width: "+Common.Style.AttachmentFieldsetWidth);
			sb.Append("px; background-color: "+Sport.Common.Tools.ColorToHex(
				Common.Style.AttachmentFieldsetBgColor)+";\">");
			sb.Append("<legend>קובץ מצורף "+strHebIndex+"</legend>");
			
			//attachment description:
			sb.Append("תיאור: ");
			sb.Append("<input type=\"text\" name=\"Attachment_Text"+strIndex+"\" ");
			sb.Append("maxlength=\"255\" dir=\"rtl\" value=\""+Server.HtmlEncode(attachment.Description));
			sb.Append("\" />");
			
			//delete?
			if (attachmentIndex >= 0)
			{
				sb.Append(Tools.MultiString("&nbsp;", 7)+"<a href=\"");
				sb.Append("?action=deleteattachment&id="+attachment.ID);
				sb.Append("&page="+page.ID+"\" id=\"DeleteAttachment_");
				sb.Append(attachment.ID+"\" style=\"\" target=\"");
				sb.Append("DeleteAttachmentFrame\" onclick=\"");
				sb.Append("return confirm('?האם לבטל קובץ זה מהעמוד');\">");
				sb.Append("[בטל קובץ זה]</a>");
			}
			
			//attachment file:
			sb.Append("<br />שם קובץ: <span style=\"font-weight: bold;\" dir=\"ltr\">");
			if (attachmentIndex < 0)
				sb.Append(FastControls.InputFile("NewAttachment", true));
			else
				sb.Append(attachment.fileName);
			sb.Append("</span>");
			
			//attachment preview:
			if (attachmentIndex >= 0)
			{
				sb.Append("<br />"+Core.AttachmentManager.BuildAttachmentHtml(
					attachment, "AttachmentPreview"+strIndex, Server));
			}
			
			//done.
			sb.Append("</fieldset></span>");
			return sb.ToString();
		} //end function BuildPageAttachment
		#endregion
		
		#region Attachments
		private void CheckNewAttachment(int parentID, 
			ref WebSiteServices.AttachmentData[] attachments)
		{
			//get file pointer:
			System.Web.HttpPostedFile objAttachmentFile=Request.Files["NewAttachment"];
			
			//got something?
			if ((objAttachmentFile != null)&&(objAttachmentFile.ContentLength > 0))
			{
				//verify file type:
				if (AttachmentManager.GetPathType(objAttachmentFile.FileName) == 
					AttachmentManager.FileType.Unknown)
				{
					string strHtml="<script type=\"text/javascript\">";
					strHtml += "alert(\"invalid attachment format!\");</script>";
					Page.RegisterClientScriptBlock("InvalidAttFormatAlert", strHtml);
				}
				else
				{
					//rebuild the attachments array for the article
					ArrayList attList=new ArrayList(attachments);
					
					//add the attachment, or get existing one if exists:
					WebSiteServices.AttachmentData attachment=
						AttachmentManager.AddAttachment(parentID, objAttachmentFile, 
						Request.Form["Attachment_Text_new"], Server, _user);
					if (attachment.fileName != Tools.AddToFilename(
						objAttachmentFile.FileName, "_"+parentID))
					{
						AddErrorMessage("קובץ מצורף כבר קיים ניתן לשנות תיאור על ידי עריכת הנתונים החדשים");
					}
					attList.Add(attachment);
					
					attachments = (WebSiteServices.AttachmentData[]) 
						attList.ToArray(typeof(WebSiteServices.AttachmentData));
				} //end if valid image
			} //end if new image not null
		} //end function CheckNewAttachment
		#endregion
		
		#region championship attachments
		private void EditChampionshipAttachments()
		{
			//verify user has permissions!
			if (_user.Id < 0)
				throw new Exception("You are not authorized to use this page!");
			
			ArticleImage = new System.Web.UI.HtmlControls.HtmlInputFile();
			ArticleImage.Style["display"] = "none";
			IsfMainView.AddContents(ArticleImage);
			
			System.Text.StringBuilder sb=new System.Text.StringBuilder();
			_pageCaption = "ניהול קבצים של אליפויות";
			
			//got anything?
			Sport.Entities.ChampionshipCategory category=null;
			int categoryID=Tools.CIntDef(Request.Form["CategoryID_1"], -1);
			if (categoryID > 0)
			{
				try
				{
					category = new Sport.Entities.ChampionshipCategory(categoryID);
				}
				catch
				{}
				if ((category != null)&&(category.Id < 0))
					category = null;
			}
			if (category != null)
			{
				WebSiteServices.WebsiteService websiteService=
					new WebSiteServices.WebsiteService();
				websiteService.CookieContainer = Sport.Core.Session.Cookies;
				
				//update existing attachments descriptions
				WebSiteServices.AttachmentData[] champAttachments=
					websiteService.GetChampionshipAttachments(category.Id);
				foreach (WebSiteServices.AttachmentData curAttachment in champAttachments)
				{
					if (Request.Form["Attachment_Text_"+curAttachment.ID] != null)
					{
						string strOldText=curAttachment.Description;
						string strNewText=Tools.CStrDef(Request.Form["Attachment_Text_"+curAttachment.ID], "");
						if (strOldText != strNewText)
						{
							curAttachment.Description = strNewText;
							websiteService.UpdateAttachment(curAttachment, 
								_user.Login, _user.Password, _user.Id);
						}
					}
				}

				//need to add attachment?
				System.Web.HttpPostedFile objAttachmentFile=Request.Files["NewAttachment"];
				
				//got something?
				if ((objAttachmentFile != null)&&(objAttachmentFile.ContentLength > 0))
				{
					//verify file type:
					if (AttachmentManager.GetPathType(objAttachmentFile.FileName) == 
						AttachmentManager.FileType.Unknown)
					{
						string strHtml="<script type=\"text/javascript\">";
						strHtml += "alert(\"invalid attachment format!\");</script>";
						Page.RegisterClientScriptBlock("InvalidAttFormatAlert", strHtml);
					}
					else
					{
						//add the attachment, or get existing one if exists:
						WebSiteServices.AttachmentData attachment=
							AttachmentManager.AddAttachment(category.Id, 
							objAttachmentFile, Request.Form["Attachment_Text_new"], 
							Server, _user);
						if (attachment.fileName != Tools.AddToFilename(
							objAttachmentFile.FileName, "_"+category.Id))
						{
							AddErrorMessage("קובץ מצורף כבר קיים ניתן לשנות תיאור על ידי עריכת הנתונים החדשים");
						}
						
						//add to championship:
						websiteService.AddChampionshipAttachment(category.Id, attachment.ID);
					} //end if valid image
				} //end if new image not null
			}
			
			//build html...
			string strPanelName="ChampAttachmentsPanel";
			sb.Append("<script type=\"text/javascript\" src=\"BrowseChampionship.js\"></script>\n");
			sb.Append(BuildChooseChampionshipHTML(strPanelName, true));
			sb.Append("<div dir=\"rtl\" id=\""+strPanelName+"\" "+
				"style=\"font-size: 12px; font-weight: bold;\">");
			sb.Append("אליפות: ");
			sb.Append("<input type=\"text\" name=\"ChampionshipName_1\" ");
			sb.Append("size=\"30\" readonly=\"readonly\" value=\"");
			if (category != null)
			{
				sb.Append(category.Championship.Name.Replace("\"", "&quot;")+" "+
					category.Name.Replace("\"", "&quot;"));
			}
			sb.Append("\" dir=\"rtl\" />&nbsp;&nbsp;&nbsp;");
			sb.Append("<input type=\"button\" name=\"btnChooseChamp_1\" ");
			sb.Append("value=\"...\" onclick=\"ChooseChampCategory(event, this);\" />");
			sb.Append(Tools.MultiString("&nbsp;", 3));
			sb.Append("<a href=\"javascript:ClearChamp(1);\">[מחק]</a><br />");
			sb.Append("<input type=\"hidden\" name=\"CategoryID_1\" ");
			sb.Append("value=\"");
			if (category != null)
				sb.Append(category.Id.ToString());
			sb.Append("\" />");
			if (category != null)
			{
				sb.Append(BuildDeleteAttachmentFrame());
				sb.Append("קבצים מצורפים:<br />");
				Core.AttachmentData[] attachments=
					AttachmentManager.GetChampionshipAttachments(Server, category.Id);
				for (int i=0; i<attachments.Length; i++)
				{
					sb.Append(BuildAttachment(category.Id, "championship", i, 
						attachments[i])+"<br />");
				}
				sb.Append(BuildAttachment(
					category.Id, "championship", -1, AttachmentData.Empty)+"<br />");
			}
			sb.Append(Tools.BuildSubmitButton(null));
			sb.Append("</div>");
			
			//done.
			AddViewText(sb.ToString(), true);
		} //end function EditChampionshipAttachments
		#endregion
		
		#region unconfirmed teams
		private void EditUnconfirmedPlayers()
		{
			//verify user has permissions!
			if (_user.Id < 0)
				throw new Exception("You are not authorized to use this page!");
			
			System.Text.StringBuilder sb=new System.Text.StringBuilder();
			sb.Append("<div dir=\"right\" dir=\"rtl\">");
			_pageCaption = "ניהול שחקנים לא מאושרים";
			
			IsfMainView.ClearContents();
			
			//check if got any data:
			if (ApplyUnconfirmedPlayersData())
			{
				sb.Append("</div>");
				IsfMainView.AddContents(sb.ToString());
				return;
			}
			
			//get list of unconfirmed teams:
			EntityFilter filter=new EntityFilter((int) Sport.Entities.Player.Fields.Status, 
				(int) Sport.Types.PlayerStatusType.Registered);
			Entity[] entities=Sport.Entities.Player.Type.GetEntities(filter);
			ArrayList arrNonConfirmedPlayers=new ArrayList();
			foreach (Entity entity in entities)
			{
				Player curPlayer=null;
				try
				{
					curPlayer = new Player(entity);
				}
				catch {}
				if ((curPlayer != null)&&(curPlayer.Id >= 0)&&(curPlayer.Team != null)&&
					(curPlayer.Team.Championship != null)&&
					(curPlayer.Team.Championship.Region != null)&&
					(curPlayer.Team.Championship.Region.Id == _user.Region.ID))
				{
					arrNonConfirmedPlayers.Add(curPlayer);
				}
			}
			
			//got anything?
			if (arrNonConfirmedPlayers.Count == 0)
			{
				sb.Append("אין שחקנים לא מאושרים במחוז שלך</div><br /><br />");
				IsfMainView.AddContents(sb.ToString());
				return;
			}
			
			//sort by championship:
			arrNonConfirmedPlayers.Sort(new PlayerTeamComparer());
			
			//build HTML:
			sb.Append("רשימת שחקנים הממתינים לאישור:<br />");
			sb.Append("<table border=\"1\">");
			sb.Append("<tr>");
			sb.Append("<th><input type=\"checkbox\" id=\"master_checkbox\" onclick=\"TogglePlayersCheckboxes(this);\" /></th>");
			sb.Append("<th>שם שחקן</th>");
			sb.Append("<th>שם קבוצה</th>");
			sb.Append("<th>תאריך רישום</th>");
			sb.Append("</tr>");
			int lastChampID=-1;
			for (int i=0; i<arrNonConfirmedPlayers.Count; i++)
			{
				Sport.Entities.Player player=(Sport.Entities.Player) arrNonConfirmedPlayers[i];
				int curChampID=player.Team.Category.Id;
				if (curChampID != lastChampID)
				{
					sb.Append("<tr>");
					sb.Append("<td><input type=\"checkbox\" id=\"toggle_champ_"+curChampID+"\" "+
						"onclick=\"TogglePlayersCheckboxes(this);\" /></td>");
					sb.Append("<td colspan=\"3\" align=\"right\" style=\"font-weight: bold;\">"+
						player.Team.Championship.Name+" "+player.Team.Category.Name+"</td>");
					sb.Append("</tr>");
					lastChampID = curChampID;
				}
				sb.Append("<tr>");
				sb.Append("<td><input type=\"checkbox\" id=\"player_"+curChampID+"_"+player.Id+"\" "+
					"name=\"UnconfirmedPlayers\" value=\""+player.Id+"\" /></td>");
				sb.Append("<td>"+player.FirstName+" "+player.LastName+"</td>");
				sb.Append("<td>"+player.Team.TeamName()+"</td>");
				sb.Append("<td>"+player.RegisterDate.ToString("dd/MM/yyyy HH:mm:ss")+"</td>");
				sb.Append("</tr>");
			}
			sb.Append("</table><br />");
			
			//submit button:
			sb.Append("<button type=\"submit\">אשר שחקנים מסומנים</button>");
			
			//done.
			sb.Append("</div>");
			IsfMainView.AddContents(sb.ToString());
		} //end function EditUnconfirmedPlayers
		
		private bool ApplyUnconfirmedPlayersData()
		{
			//get list of unconfirmed teams to be confirmed:
			string strUnconfirmedPlayers=Tools.CStrDef(Request.Form["UnconfirmedPlayers"], "");
			
			//got anything?
			if (strUnconfirmedPlayers.Length == 0)
				return false;
			
			//split into seperate items:
			string[] arrPlayersID=strUnconfirmedPlayers.Split(new char[] {','});
			System.Text.StringBuilder sb=new System.Text.StringBuilder();
			
			//build list of teams:
			ArrayList arrPlayers=new ArrayList();
			foreach (string playerID in arrPlayersID)
			{
				Player player=null;
				try
				{
					player = new Player(Tools.CIntDef(playerID, -1));
				}
				catch {}
				if ((player != null)&&(player.Id >= 0))
					arrPlayers.Add(player);
			}
			
			//confirmed?
			if (Request.Form["confirm_apply_players"] == "1")
			{
				foreach (Player player in arrPlayers)
				{
					Sport.Data.EntityEdit entEdit=player.Entity.Edit();
					entEdit.Fields[(int) Sport.Entities.Player.Fields.Status] = (int) Sport.Types.PlayerStatusType.Confirmed;
					System.Net.CookieContainer originalCookies=Sport.Core.Session.Cookies;
					Sport.Core.Session.Cookies = (System.Net.CookieContainer) Session["cookies"];
					entEdit.Save();
					Sport.Core.Session.Cookies = originalCookies;
				}
				return false;
			}
			
			//build HTML:
			sb.Append("<input type=\"hidden\" name=\"UnconfirmedPlayers\" value=\""+strUnconfirmedPlayers+"\" />");
			sb.Append("<input type=\"hidden\" name=\"confirm_apply_players\" value=\"1\" />");
			sb.Append("השחקנים הבאים יהיו מאושרים:<br />");
			sb.Append("<ul>");
			foreach (Player player in arrPlayers)
			{
				sb.Append("<li>");
				sb.Append(player.Name+" ("+player.Team.TeamName()+", "+player.Team.Championship.Name+" "+player.Team.Category.Name+")");
				sb.Append("</li>");
			}
			sb.Append("</ul><br /><br />");
			sb.Append("<button type=\"submit\">אישור סופי</button>");
			
			//done.
			IsfMainView.AddContents(sb.ToString());
			return true;
		}
		#endregion
		
		#region sponsors list
		private void UpdateSponsorsList()
		{
			//verify user has permissions!
			if (_user.Id < 0)
				throw new Exception("You are not authorized to use this page!");
			
			System.Text.StringBuilder sb=new System.Text.StringBuilder();
			sb.Append("<div dir=\"right\" dir=\"rtl\">");
			_pageCaption = "ניהול נותני חסות";
			
			IsfMainView.ClearContents();
			
			ArticleImage = new System.Web.UI.HtmlControls.HtmlInputFile();
			ArticleImage.Style["display"] = "none";
			IsfMainView.AddContents(ArticleImage);
			
			//check if got any data:
			if (ApplySponsorsData())
			{
				sb.Append("</div>");
				IsfMainView.AddContents(sb.ToString());
				return;
			}
			
			//get list of sponsors:
			Core.SponsorManager.SetServer(this.Server);
			Core.SponsorData[] arrSponsors=Core.SponsorManager.GetSponsors();
			
			//build HTML:
			sb.Append("(סדר התצוגה מימין לשמאל)<br />");
			sb.Append("<input type=\"hidden\" name=\"UpdateSponsorsList\" value=\"1\" />");
			sb.Append("<ol>");
			for (int i=0; i<arrSponsors.Length; i++)
			{
				Core.SponsorData sponsor=arrSponsors[i];
				sb.Append("<li>");
				sb.Append("תמונה:<br /><img src=\""+sponsor.ImageFileName+"\" /><br />");
				sb.Append("קישור: <input type=\"text\" dir=\"ltr\" name=\"sponsor_url_"+(i+1)+"\" "+
					"value=\""+sponsor.URL+"\" /><br />");
				sb.Append("החלף תמונה: <input type=\"file\" name=\"sponsor_image_"+(i+1)+"\" /><br />");
				sb.Append("<input type=\"checkbox\" name=\"delete_image_"+(i+1)+"\" "+
					"value=\"1\" />סמן למחיקה");
				sb.Append("</li>");
			}
			sb.Append("<li>");
			sb.Append("<u>נותן חסות חדש</u><br />");
			sb.Append("קישור: <input type=\"text\" name=\"sponsor_url_new\" /><br />");
			sb.Append("תמונה: <input type=\"file\" name=\"sponsor_image_new\" />");
			sb.Append("</li>");
			sb.Append("</ol>");
			
			//submit button:
			sb.Append(Tools.BuildSubmitButton(null));
			
			//done.
			sb.Append("</div>");
			IsfMainView.AddContents(sb.ToString());
		}
		
		private bool ApplySponsorsData()
		{
			//got anything?
			if (Request.Form["UpdateSponsorsList"] != "1")
				return false;
			
			//get current sponsors:
			Core.SponsorManager.SetServer(this.Server);
			Core.SponsorData[] arrSponsors=Core.SponsorManager.GetSponsors();
			
			//iterate over sponsors:
			ArrayList sponsorsList=new ArrayList(arrSponsors);
			bool blnDeleted=false;
			for (int i=0; i<arrSponsors.Length; i++)
			{
				//get current sponsor:
				Core.SponsorData curSponsor=arrSponsors[i];
				
				//have to delete?
				if (Request.Form["delete_image_"+(i+1)] == "1")
				{
					//delete:
					System.IO.File.Delete(Server.MapPath(curSponsor.ImageFileName));
					
					//raise flag:
					blnDeleted = true;
					
					//remove:
					for (int k=0; k<sponsorsList.Count; k++)
					{
						if ((sponsorsList[k] as Core.SponsorData).ImageFileName == curSponsor.ImageFileName)
						{
							sponsorsList.RemoveAt(k);
							break;
						}
					}
					
					//cancel pending actions:
					continue;
				}
				
				//update URL if needed:
				bool blnChanged=false;
				if (Request.Form["sponsor_url_"+(i+1)] != curSponsor.URL)
				{
					curSponsor.URL = Request.Form["sponsor_url_"+(i+1)];
					blnChanged = true;
				}

				//update image if needed:
				HttpPostedFile objFile=Request.Files["sponsor_image_"+(i+1)];
				if ((objFile != null)&&(objFile.ContentLength > 0))
				{
					string strOldImageName=curSponsor.ImageFileName;
					File.Delete(Server.MapPath(strOldImageName));
					string strFileName=Core.SponsorManager.FolderName+"/"+Path.GetFileName(objFile.FileName);
					objFile.SaveAs(Server.MapPath(strFileName));
					curSponsor.ImageFileName = strFileName;
					blnChanged = true;
				}
				
				//changed?
				if (blnChanged)
					Core.SponsorManager.UpdateSponsor(curSponsor, i);
			} //end loop over existing sponsors
			
			//deleted any?
			if (blnDeleted)
			{
				for (int i=0; i<sponsorsList.Count; i++)
				{
					Core.SponsorData sponsor=(Core.SponsorData) sponsorsList[i];
					string strCurPath=Server.MapPath(sponsor.ImageFileName);
					string strNewPath=Server.MapPath(Core.SponsorManager.FolderName+"/"+
						(i+1)+"_"+Path.GetFileName(strCurPath).Substring(2));
					if (strCurPath != strNewPath)
						File.Move(strCurPath, strNewPath);
				}
			}
			
			//new sponsor?
			HttpPostedFile objNewImage=Request.Files["sponsor_image_new"];
			if ((objNewImage != null)&&(objNewImage.ContentLength > 0))
			{
				string strFileName=Core.SponsorManager.FolderName+"/"+Path.GetFileName(objNewImage.FileName);
				objNewImage.SaveAs(Server.MapPath(strFileName));
				Core.SponsorData data=new Core.SponsorData();
				data.ImageFileName = strFileName;
				data.URL = Tools.CStrDef(Request.Form["sponsor_url_new"], "");
				Core.SponsorManager.UpdateSponsor(data, sponsorsList.Count);
			}

			//update cache:
			Cache["sponsors"] = Core.SponsorManager.GetSponsors();
			
			//done.
			return false;
		} //end function ApplySponsorsData
		#endregion
		
		#region special offer
		private void UpdateSpecialOffer()
		{
			//verify user has permissions!
			if (_user.Id < 0)
				throw new Exception("You are not authorized to use this page!");
			
			System.Text.StringBuilder sb=new System.Text.StringBuilder();
			sb.Append("<div dir=\"right\" dir=\"rtl\">");
			_pageCaption = "עריכת קליק חודשי";
			
			IsfMainView.ClearContents();
			
			//file path:
			string strTextFilePath=Server.MapPath(Common.Data.AppPath+"/puredataBox.txt");
			
			//check if got any data:
			string strSpecialOfferData=Request.Form["SpecialOfferData"];
			if (strSpecialOfferData != null)
			{
				string[] arrSpecialOfferLines=strSpecialOfferData.Split(new char[] {'\n'});
				System.Text.StringBuilder builder=new System.Text.StringBuilder();
				for (int i=0; i<arrSpecialOfferLines.Length; i++)
					builder.Append("&phrase"+(i+1)+"="+arrSpecialOfferLines[i]+"\n");
				builder.Append("\n"+"&NumberSentences="+arrSpecialOfferLines.Length);
				Tools.CreateTextFile(strTextFilePath, builder.ToString(), false); //true);
			}
			
			//get file data:
			strSpecialOfferData = "";
			string[] arrLines=Tools.ReadTextFile(strTextFilePath, false); //true);
			for (int j=0; j<arrLines.Length-1; j++)
			{
				string strCurLine=arrLines[j];
				string[] arrTemp=strCurLine.Split(new char[] {'='});
				strSpecialOfferData += arrTemp[arrTemp.Length-1];
				if (j < (arrLines.Length-2))
					strSpecialOfferData += "\n";
			}
			
			//build HTML:
			sb.Append("<u>שורות אשר יופיעו בבאנר הקליק החודשי:</u><br />");
			sb.Append("<textarea name=\"SpecialOfferData\" "+
				"cols=\"30\" rows=\"5\">"+strSpecialOfferData+
				"</textarea><br />");
			
			//submit button:
			sb.Append(Tools.BuildSubmitButton(null));
			
			//done.
			sb.Append("<br /><br /><br /></div>");
			IsfMainView.AddContents(sb.ToString());
		}
		#endregion
		
		#region top banner
		private void UpdateTopBanner()
		{
			//verify user has permissions!
			if (_user.Id < 0)
				throw new Exception("You are not authorized to use this page!");
			
			System.Text.StringBuilder sb=new System.Text.StringBuilder();
			sb.Append("<div dir=\"right\" dir=\"rtl\">");
			
			//URL
			string strBaseURL=Common.Data.AppPath+"/Register.aspx?action="+
					SportSiteAction.UpdateTopBanner+"&index=%index";
			
			//banner index:
			int index=Tools.CIntDef(Request.QueryString["index"], 0);
			
			if (Request.QueryString["index"] == "JHWC")
			{
				//string strFilePath=Server.MapPath(Banner.TopBannerDataFolder+"/data_JHWC.txt");
				//StreamWriter objWriter=File.CreateText(strFilePath);
			}
			
			//get files list:
			string[] arrDataFiles=SportSite.Controls.Banner.GetTopBannerDataFiles(
				this.Page, this.Server);
			
			//new?
			if (Request.QueryString["new"] == "1")
			{
				//first?
				if (arrDataFiles.Length == 0)
				{
					string strSourceFile=Server.MapPath(Common.Data.AppPath+"/puredata.txt");
					string strDestFile=Server.MapPath(Banner.TopBannerDataFolder+"/data_1.txt");
					System.IO.File.Copy(strSourceFile, strDestFile, true);
					Banner.RebuildTopBannerDataFilesList(this.Page, this.Server);
					arrDataFiles = Banner.GetTopBannerDataFiles(this.Page, this.Server);
				}
				index = arrDataFiles.Length+1;
				string strFilePath=Server.MapPath(Banner.TopBannerDataFolder+"/data_"+index+".txt");
				StreamWriter objWriter=File.CreateText(strFilePath);
				objWriter.Close();
				Banner.RebuildTopBannerDataFilesList(this.Page, this.Server);
				Response.Redirect(strBaseURL.Replace("%index", index.ToString()), true);
			} //end if need to add new banner
			
			//delete?
			if (Request.QueryString["delete"] == "1")
			{
				string strFilePath=Server.MapPath(Banner.TopBannerDataFolder+"/data_"+index+".txt");
				if (System.IO.File.Exists(strFilePath))
				{
					File.Delete(strFilePath);
					Banner.RebuildTopBannerDataFilesList(this.Page, this.Server);
					arrDataFiles = Banner.GetTopBannerDataFiles(this.Page, this.Server);
					index = (arrDataFiles.Length == 0)?0:1;
					Response.Redirect(strBaseURL.Replace("%index", index.ToString()), true);
				}
			} //end if need to delete banner
			
			//caption:
			string strPageCaption="עריכת באנר עליון";
			if (index > 0)
			{
				strPageCaption += " "+Sport.Common.Tools.IntToHebrew(index, false);
			}
			_pageCaption = strPageCaption;
			
			IsfMainView.ClearContents();
			
			//file path:
			string strTextFilePath=Server.MapPath(Common.Data.AppPath+"/puredata.txt");
			if ((index > 0)&&(arrDataFiles != null)&&((index-1) < arrDataFiles.Length))
			{
				strTextFilePath = arrDataFiles[index-1];
			}
			
			//check if got any data:
			string strFirstLine=Request.Form["BannerFirstLine"];
			string strSecondLine=Request.Form["BannerSecondLine"];
			string strThirdLine=Request.Form["BannerThirdLine"];
			int bannerType=Tools.CIntDef(Request.Form["BannerType"], -1);
			if (strFirstLine != null)
				CreateTopBannerDataFile(strTextFilePath, strFirstLine, strSecondLine, strThirdLine, bannerType);
			
			//get file data:
			//Response.Write(Tools.DiagnoseTextFile(strTextFilePath, "<br />"));
			//Response.Write("<br />");
			strFirstLine = "";
			strSecondLine = "";
			strThirdLine = "";
			bannerType = -1;
			string[] arrLines=Tools.ReadTextFile(strTextFilePath, false);;
			for (int j=0; j<arrLines.Length; j++)
			{
				string strCurLine=arrLines[j];
				string[] arrTemp=strCurLine.Split(new char[] {'='});
				string strCurValue=arrTemp[arrTemp.Length-1];
				switch (j)
				{
					case 0:
						strFirstLine = strCurValue;
						break;
					case 1:
						string[] arrTemp2=strCurValue.Replace("<br>", "\n").Split(new char[] {'\n'});
						strSecondLine = arrTemp2[0];
						if (arrTemp2.Length > 1)
							strThirdLine = arrTemp2[1];
						break;
					case 2:
						bannerType = Tools.CIntDef(strCurValue, -1);
						break;
				}
			}
			
			//build HTML:
			sb.Append("<u>שורות אשר יופיעו בבאנר עליון:</u><br />");
			sb.Append("שורה ראשונה: <input type=\"text\" name=\"BannerFirstLine\" "+
				"value=\""+strFirstLine+"\" /><br />");
			sb.Append("שורה שנייה: <input type=\"text\" name=\"BannerSecondLine\" "+
				"value=\""+strSecondLine+"\" /><br />");
			sb.Append("שורה שלישית: <input type=\"text\" name=\"BannerThirdLine\" "+
				"value=\""+strThirdLine+"\" /><br />");
			sb.Append("<u>סוג באנר:</u><br />");
			string[] arrBannerTypeNames=new string[] {"כדורסל", "כדורעף", "טלויזיה"};
			for (int i=0; i<arrBannerTypeNames.Length; i++)
			{
				int curType=(i+1);
				sb.Append("<input type=\"radio\" name=\"BannerType\" "+
					"value=\""+curType+"\"");
				if (curType == bannerType)
					sb.Append(" checked=\"checked\"");
				sb.Append(" />"+arrBannerTypeNames[i]+"<br />");
			}
			
			//submit button:
			sb.Append(Tools.BuildSubmitButton(null));
			
			//more banners..
			string strBaseLink="[<a href=\"?action="+SportSiteAction.UpdateTopBanner+
				"%qs\"%onclick>%text</a>]";
			sb.Append("<br /><br />");
			sb.Append(strBaseLink.Replace("%qs", "&new=1").Replace("%text", "הוסף באנר").Replace("%onclick", ""));
			sb.Append(Tools.MultiString("&nbsp;", 5));
			if (index > 0)
			{
				sb.Append(strBaseLink.Replace("%qs", "&delete=1&index="+index).Replace(
					"%text", "מחק באנר זה").Replace("%onclick", " "+
					"onclick=\"return confirm('האם למחוק באנר זה?');\""));
			}
			
			//done.
			sb.Append("<br /></div>");
			IsfMainView.AddContents(sb.ToString());
		}
		
		private void CreateTopBannerDataFile(string strFilePath, string strFirstLine, 
			string strSecondLine, string strThirdLine, int bannerType)
		{
			string strText="";
			strText += "&phrase1="+strFirstLine+Common.Data.NEW_LINE;
			strText += "&phrase2="+strSecondLine;
			if ((strThirdLine != null)&&(strThirdLine.Length > 0))
				strText += "<br>"+strThirdLine;
			strText += Common.Data.NEW_LINE+"";
			strText += "&bannerType="+bannerType;
			string strLink=Tools.GetFlashLink(strFilePath);
			if (strLink.Length > 0)
				strText += Common.Data.NEW_LINE+"link="+strLink;
			Tools.CreateTextFile(strFilePath, strText, false);
		}
		#endregion
		
		#region advertisements
		private void UpdateAdvertisement(SportSite.Controls.BannerType type)
		{
			//verify user has permissions!
			if (_user.Id < 0)
				throw new Exception("You are not authorized to use this page!");
			
			//define ini section and key..
			string strSectionName="";
			string strKeyName="FlashMovie";
			
			System.Text.StringBuilder sb=new System.Text.StringBuilder();
			sb.Append("<div dir=\"right\" dir=\"rtl\">");
			string strCaption="";
			ArrayList arrFileNames=null;
			switch (type)
			{
				case SportSite.Controls.BannerType.Advertisement_Main:
					strCaption = "ראשית";
					strSectionName = "MainAdvertisement";
					arrFileNames = new ArrayList(
						Banner.GetMainAdvertisementFlash(this.Server));
					break;
				case SportSite.Controls.BannerType.Advertisement_Secondary:
					strCaption = "משנית";
					strSectionName = "SecondaryAdvertisement";
					arrFileNames = new ArrayList(
						Banner.GetSubAdvertisementFlash(this.Server));
					break;
				case SportSite.Controls.BannerType.Advertisement_Small:
					strCaption = "צד קטנה";
					strSectionName = "SmallAdvertisement";
					strKeyName = "FileName";
					arrFileNames = new ArrayList();
					string strSmallBannerFileName = 
						Banner.GetSmallAdvertisementFile(this.Server);
					arrFileNames.Add(strSmallBannerFileName);
					break;
			}
			_pageCaption = "עריכת פרסומת "+strCaption;
			
			IsfMainView.ClearContents();
			
			ArticleImage = new System.Web.UI.HtmlControls.HtmlInputFile();
			ArticleImage.Style["display"] = "none";
			IsfMainView.AddContents(ArticleImage);
			
			//check flash folder:
			string strFolderName=Data.AppPath+"/Flash/Advertisements";
			string strFolderPath=Server.MapPath(strFolderName);
			if (!Directory.Exists(strFolderPath))
				Directory.CreateDirectory(strFolderPath);
			
			//check if got any data:
			bool blnChanged=false;
			if (Page.IsPostBack)
			{
				//got files?
				for (int i=0; i<Request.Files.Count; i++)
				{
					//get current key
					string strKey=Request.Files.Keys[i];
					HttpPostedFile objFile=null;
					
					//small advertisement or flash file?
					if ((strKey.ToLower() == "small_banner_file")||
						(strKey.ToLower().StartsWith("flashfile")))
					{
						//get file:
						objFile = Request.Files[i];
					}
					
					//got anything?
					if ((objFile == null)||(objFile.ContentLength == 0))
						continue;
					
					//original file name:
					string strOriginalName=objFile.FileName;
					
					//get extension:
					string strExtension=Path.GetExtension(strOriginalName);
					
					//get index:
					int index=-1;
					string strIndex="";
					if (type != BannerType.Advertisement_Small)
					{
						string[] arrTemp=strKey.Split(new char[] {'_'});
						index = Tools.CIntDef(arrTemp[1], -1);
						strIndex = "חדש";
						if (index >= 0)
							strIndex=Sport.Common.Tools.IntToHebrew(index+1, false);
						strIndex = "קובץ "+strIndex+": ";
						
						//flash indeed?
						if (strExtension.ToLower() != ".swf")
						{
							sb.Append("<span style=\"color: red; font-weight: bold;\">"+
								strIndex+"ניתן להעלות קבצי פלאש בלבד</span><br />");
							continue;
						}
					}
					else
					{
						index = 0;
						if ((!Tools.IsImageFile(strOriginalName))&&
							(strExtension.ToLower() != ".swf"))
						{
							sb.Append("<span style=\"color: red; font-weight: bold;\">"+
								"ניתן להעלות קבצי פלאש או תמונה בלבד</span><br />");
							continue;
						}
					}
					
					//generate unique name:
					string strTempFilePath=Server.MapPath(strFolderName+"/tmp_"+
						Tools.CurrentMilliSeconds()+strExtension.ToLower());
					
					//save as temporary file:
					objFile.SaveAs(strTempFilePath);
					
					//initialize name:
					string strFileName="";
					string strFilePath="";
					
					//already exists?
					string strExistingFile=Sport.Common.Tools.GetEqualFile(
						strTempFilePath, strFolderPath);
					if (strExistingFile.Length > 0)
					{
						string strExistingName=Path.GetFileName(strExistingFile);
						bool exists=false;
						foreach (string strCurFileName in arrFileNames)
						{
							if (strCurFileName.ToLower() == strExistingName.ToLower())
							{
								sb.Append("<span style=\"color: red; font-weight: bold;\">"+
									strIndex+"קובץ זהה כבר קיים</span><br />");
								File.Delete(strTempFilePath);
								exists = true;
							}
						}
						if (exists)
							continue;
						strFileName = strExistingName;
						if (type == BannerType.Advertisement_Small)
							arrFileNames[0] = strFileName;
					}
					
					//find proper name:
					if (strFileName.Length == 0)
					{
						strFileName = Path.GetFileName(strOriginalName);
						strFilePath = Server.MapPath(strFolderName+"/"+strFileName);
						int x=1;
						while (File.Exists(strFilePath))
						{
							strFileName = Tools.AddToFilename(strFilePath, "_"+x);
							strFilePath = Server.MapPath(strFolderName+"/"+strFileName);
							x++;
						}
					}
					strFilePath = Server.MapPath(strFolderName+"/"+strFileName);
					
					//copy temporary file to new location and remove:
					File.Copy(strTempFilePath, strFilePath, true);
					File.Delete(strTempFilePath);
					
					//new?
					if (index == -1)
					{
						arrFileNames.Add(strFileName);
					}
					else
					{
						if (index < arrFileNames.Count)
							arrFileNames[index] = strFileName;
					}
					
					//raise flag:
					blnChanged = true;
				} //end loop over files
				
				//remove any files?
				ArrayList arrToRemove=new ArrayList(
					Sport.Common.Tools.SplitNoBlank(Request.Form["RemoveFile"], ','));
				for (int i=0; i<arrToRemove.Count; i++)
				{
					int curIndex=Tools.CIntDef(arrToRemove[i], -1);
					if ((curIndex >= 0)&&(curIndex < arrFileNames.Count))
						arrToRemove[i] = arrFileNames[curIndex];
				}
				for (int i=0; i<arrToRemove.Count; i++)
				{
					object o=arrToRemove[i];
					if (arrFileNames.IndexOf(o) > 0)
					{
						arrFileNames.Remove(o);
						blnChanged = true;
					}
				}
				
				//links
				if (type == BannerType.Advertisement_Small)
				{
					string strURL=Tools.CStrDef(Request.Form["small_banner_link"], "");
					Tools.WriteIniValue(strSectionName, "URL", strURL, this.Server);
					blnChanged = true;
				}
				else
				{
					foreach (string strCurFileName in arrFileNames)
					{
						string strValidFileName=strCurFileName.Replace(" ", "_");
						string strLinkFile=Tools.CStrDef(
							Request.Form["linkfile_"+strValidFileName], "");
						if (strLinkFile.Length > 0)
						{
							string strLink=Tools.CStrDef(
								Request.Form["link_"+strValidFileName], "");
							if (!strLinkFile.ToLower().EndsWith(".txt"))
							{
								sb.Append("<span style=\"color: red; font-weight: bold;\">"+
									"קובץ קישור: אפשר רק סיומת טקסט</span><br />");
								continue;
							}
							StreamWriter objWriter=new StreamWriter(Server.MapPath(strLinkFile), false);
							objWriter.WriteLine("link="+strLink);
							objWriter.Close();
						}
					}
				}
			} //end if page was posted
			
			//any changes were made?
			if (blnChanged)
			{
				//apply in the ini file and rebuild the banner data:
				Tools.WriteIniValue(strSectionName, strKeyName, 
					String.Join("|", (string[]) arrFileNames.ToArray(typeof(string))), 
					this.Server);
				switch (type)
				{
					case SportSite.Controls.BannerType.Advertisement_Main:
						Banner.RebuildMainAdvertisement(this.Server);
						Application["MainAdvertisementCount"] = arrFileNames.Count;
						break;
					case SportSite.Controls.BannerType.Advertisement_Secondary:
						Banner.RebuildSubAdvertisement(this.Server);
						Application["SubAdvertisementCount"] = arrFileNames.Count;
						break;
					case SportSite.Controls.BannerType.Advertisement_Small:
						Banner.RebuildSmallAdvertisement(this.Server);
						break;
				}
			}
			
			//build HTML:
			for (int j=0; j<arrFileNames.Count; j++)
			{
				string strCurName=arrFileNames[j].ToString();
				sb.Append(BuildBannerFlashPanel(strFolderName, strCurName, j, type));
				sb.Append("<br />");
			}
			if (type != BannerType.Advertisement_Small)
				sb.Append(BuildBannerFlashPanel("", "", -1, type)+"<br />");
			
			//javascript:
			string strJS="<script type=\"text/javascript\">";
			strJS += " var _adConfirmRemoveMsg=\"האם למחוק %c קבצים?\";";
			strJS += "</script>";
			Page.RegisterClientScriptBlock("advertisement_heb", strJS);
			
			//submit button:
			sb.Append(Tools.BuildSubmitButton("return ValidateAdvertisements(this)"));
			
			//done.
			sb.Append("<br /><br /></div>");
			IsfMainView.AddContents(sb.ToString());
		}
		
		private string BuildBannerFlashPanel(string strFolderName, 
			string strFileName, int index, BannerType type)
		{
			System.Text.StringBuilder builder=new System.Text.StringBuilder();
			if (type != BannerType.Advertisement_Small)
			{
				string strIndex="חדש";
				if (index >= 0)
					strIndex = Sport.Common.Tools.IntToHebrew(index+1, false);
				builder.Append("<fieldset style=\"width: 505px;\">"+
					"<legend>קובץ "+strIndex+"</legend>");
			}
			string strMovie=strFolderName+"/"+strFileName;
			string strContainer="AdvertisementMovie_Register_"+(index+1);
			if (index >= 0)
			{
				builder.Append("באנר קיים:<br />");
				if (strMovie.ToLower().EndsWith(".swf"))
				{
					IsfMainView.clientSide.RegisterFlashMovie(strContainer, strMovie, 500, 65, "", "#ffffff");
					builder.Append("<div id=\""+strContainer+"\"></div>");
				}
				else
				{
					string strURL=Tools.CStrDef(
						Banner.GetSmallAdvertisementLink(this.Server), "");
					builder.Append(Banner.BuildImageBanner(
						strMovie, strURL, this.Server));
				}
				string strValidFileName=strFileName.Replace(" ", "_");
				if (type != BannerType.Advertisement_Small)
				{
					builder.Append("קובץ טקסט שמכיל קישור: <input type=\"text\" "+
						"name=\"linkfile_"+strValidFileName+"\" /><br />");
				}
				string strElementName=(type == BannerType.Advertisement_Small)?
					"small_banner_link":"link_"+strValidFileName;
				builder.Append("קישור: <input type=\"text\" "+
					"name=\""+strElementName+"\"");
				if (type == BannerType.Advertisement_Small)
				{
					string strURL=Banner.GetSmallAdvertisementLink(this.Server);
					builder.Append(" value=\""+strURL+"\"");
				}
				builder.Append(" /><br />");
				if ((index > 0)||(type == BannerType.Advertisement_Small))
				{
					builder.Append("<input type=\"checkbox\" name=\"RemoveFile\" "+
						"value=\""+index+"\" />מחק קובץ זה<br />");
				}
			} //end if index is greater or equal to zero.
			
			builder.Append("העלה קובץ חדש: <input type=\"file\" "+
				"name=\"FlashFile_"+index+"\" />");
			
			if (type != BannerType.Advertisement_Small)
				builder.Append("</fieldset>");
			
			return builder.ToString();
		}
		
		private void UpdateMainAdvertisement()
		{
			UpdateAdvertisement(SportSite.Controls.BannerType.Advertisement_Main);
		}
		
		private void UpdateSecondaryAdvertisement()
		{
			UpdateAdvertisement(SportSite.Controls.BannerType.Advertisement_Secondary);
		}
		
		private void UpdateSmallAdvertisement()
		{
			UpdateAdvertisement(SportSite.Controls.BannerType.Advertisement_Small);
		}
		#endregion
		
		#region championship logo
		private void UpdateChampLogo()
		{
			IsfMainView.SetPageCaption("עדכון לוגו אליפות");
			System.Text.StringBuilder sb=new System.Text.StringBuilder();
			sb.Append("<div dir=\"rtl\" align=\"right\">");
			int champID=Tools.CIntDef(Request.QueryString["championship"], -1);
			Championship champ=null;
			if (champID >= 0)
			{
				try
				{
					champ = new Championship(champID);
				}
				catch {}
			}
			if (champ == null)
			{
				sb.Append("זיהוי אליפות שגוי!");
			}
			else
			{
				ArticleImage = new System.Web.UI.HtmlControls.HtmlInputFile();
				ArticleImage.Style["display"] = "none";
				IsfMainView.AddContents(ArticleImage);
				sb.Append("אליפות: "+champ.Name+"<br />");
				string strFolderName=Common.Data.AppPath+"/Images/Logos";
				string strFolderPath=Server.MapPath(strFolderName);
				if (!System.IO.Directory.Exists(strFolderPath))
					System.IO.Directory.CreateDirectory(strFolderPath);
				strFolderName += "/champ_"+champ.Id;
				strFolderPath = Server.MapPath(strFolderName);
				if (!System.IO.Directory.Exists(strFolderPath))
					System.IO.Directory.CreateDirectory(strFolderPath);
				
				//got new logo?
				System.Web.HttpPostedFile objChampLogo=Request.Files["ChampLogo"];
				string[] arrFileNames=null;
				if ((objChampLogo != null)&&(objChampLogo.ContentLength > 0))
				{
					arrFileNames = System.IO.Directory.GetFiles(strFolderPath);
					foreach (string strFileName in arrFileNames)
						System.IO.File.Delete(strFileName);
					string strImageName=strFolderName+"/"+System.IO.Path.GetFileName(objChampLogo.FileName);
					objChampLogo.SaveAs(Server.MapPath(strImageName));
				}
				
				//search for existing logo...
				string strLogoImage="";
				arrFileNames = System.IO.Directory.GetFiles(strFolderPath);
				if ((arrFileNames != null)&&(arrFileNames.Length > 0))
					strLogoImage = strFolderName+"/"+System.IO.Path.GetFileName(arrFileNames[0]);
				
				//exisiting logo:
				sb.Append("תמונה קיימת: ");
				if (strLogoImage.Length > 0)
					sb.Append("<img src=\""+strLogoImage+"\" />");
				sb.Append("<br />");
				
				sb.Append("לוגו חדש: <input type=\"file\" name=\"ChampLogo\" /><br /><br />");
				sb.Append(Tools.BuildSubmitButton(null));
			}
			sb.Append("</div>");
			IsfMainView.AddContents(sb.ToString());
		}
		#endregion
		
		#region Permanent Championships
		private void EditPermanentChamps()
		{
			//verify user has permissions!
			if (!Tools.IsAuthorized_PermanentChamps(_user.Id))
				throw new Exception("You are not authorized to use this page!");
			
			//put initial data:
			_pageCaption = "הגדרת ליגות קבועות";
			
			//need to update?
			for (int linkIndex=0; linkIndex<LinkManager.PermanentChampsCount; linkIndex++)
			{
				string strChampName=Request.Form["DisplayName_"+(linkIndex+1)];
				int categoryID=Tools.CIntDef(Request.Form["CategoryID_"+(linkIndex+1)], -1);
				if (strChampName != null)
				{
					LinkManager.SetPermanentChampionship(
						Server, linkIndex, strChampName, categoryID);
				}
			}

			//get all permanent championships data:
			LinkManager.LinkData[] arrPermanentChamps=
				LinkManager.GetPermanentChampionships(Server);
			
			//build html...
			string strPanelName="PermanentChampionshipsPanel";
			System.Text.StringBuilder strJS=new System.Text.StringBuilder();
			System.Text.StringBuilder strHTML=new System.Text.StringBuilder();
			strJS.Append("<script type=\"text/javascript\" src=\"BrowseChampionship.js\"></script>\n");
			strHTML.Append(BuildChooseChampionshipHTML(strPanelName, true));
			
			//iterate over the permanent championships and add proper html...
			strHTML.Append("<div dir=\"rtl\" id=\""+strPanelName+"\" "+
				"style=\"font-size: 12px; font-weight: bold;\">");
			for (int i=0; i<arrPermanentChamps.Length; i++)
				strHTML.Append(BuildPermanentChampHtml(arrPermanentChamps, i)+"<br />");
			strHTML.Append("<br />"+Tools.BuildSubmitButton(null));
			strHTML.Append("</div>");
			
			//done.
			AddViewText(strJS.ToString(), true);
			AddViewText(strHTML.ToString(), true);
		} //end function EditPermanentChamps
		
		private string BuildChooseChampionshipHTML(string parentID, bool showCategory)
		{
			//initialize service:
			DataServices.DataService _service=new DataServices.DataService();
			_service.CookieContainer = Sport.Core.Session.Cookies;
			
			//get latest season:
			int season=_service.GetLatestSeason();
			
			//get all available championship categories from database:
			int[] arrCategories=_service.GetChampionshipCategoriesBySeason(season);
			
			//build lists...
			ArrayList regions=new ArrayList();
			ArrayList championships=new ArrayList();
			ArrayList categories=new ArrayList();
			foreach (int categoryID in arrCategories)
			{
				Sport.Entities.ChampionshipCategory category=
					new ChampionshipCategory(categoryID);
				Sport.Entities.Championship championship=category.Championship;
				Sport.Entities.Region region=championship.Region;
				if (regions.IndexOf(region) == -1)
					regions.Add(region);
				if (championships.IndexOf(championship) == -1)
					championships.Add(championship);
				categories.Add(category);
			}
			
			//build client side arrays and list boxes
			System.Text.StringBuilder strJS=new System.Text.StringBuilder();
			System.Text.StringBuilder strHTML=new System.Text.StringBuilder();
			strJS.Append("<script type=\"text/javascript\">");
			strJS.Append(" _parentPanelID = \""+parentID+"\";\n");
			if (!showCategory)
				strJS.Append(" _showCategory = false;\n");
			foreach (Sport.Entities.Region region in regions)
			{
				int regionID=region.Id;
				string v="[\""+regionID+"\"]";
				string vn=v+"[\"name\"]";
				string vc=v+"[\"champs\"]";
				strJS.Append(" _rd"+v+" = new Array();");
				strJS.Append(" _rd"+vn+" = \""+region.Name.Replace("\"", "\\\"")+"\";");
				strJS.Append(" _rd"+vc+" = new Array();");
				foreach (Sport.Entities.Championship championship in championships)
				{
					if (championship.Region.Id == region.Id)
					{
						int champID=championship.Id;
						string vv=vc+"[\""+champID+"\"]";
						string vvn=vv+"[\"name\"]";
						string vvc=vv+"[\"categories\"]";
						strJS.Append(" _rd"+vv+" = new Array();");
						strJS.Append(" _rd"+vvn+" = \""+championship.Name.Replace("\"", "\\\"")+"\";");
						strJS.Append(" _rd"+vvc+" = new Array();");
						foreach (Sport.Entities.ChampionshipCategory category in categories)
						{
							if (category.Championship.Id == championship.Id)
							{
								int categoryID=category.Id;
								string vvv=vvc+"[\""+categoryID+"\"]";
								strJS.Append(" _rd"+vvv+" = \""+category.Name.Replace("\"", "\\\"")+"\";");
							} //end if same championship
						} //end loop over categories
					} //end if same region
				} //end loop over championships
			} //end loop over the regions
			strJS.Append("</script>");

			strHTML.Append(strJS.ToString());
			strHTML.Append("\n");
			strHTML.Append("<div id=\"ChooseCategory\" dir=\"rtl\" style=\"width: 250px; ");
			strHTML.Append("border: 2px solid blue; text-align: center; display: none; ");
			strHTML.Append("font-weight: bold; background-color: yellow;\">");
			strHTML.Append("<h3><u>בחירת אליפות</u></h3>");
			strHTML.Append("בחר מחוז:<br />");
			strHTML.Append("<select id=\"ChampRegion\" size=\"5\" onchange=\"RegionChanged(this);\">");
			foreach (Sport.Entities.Region region in regions)
				strHTML.Append("<option value=\""+region.Id+"\">"+region.Name+"</option>");
			strHTML.Append("</select><br /><br />");
			strHTML.Append("בחר אליפות:<br />");
			strHTML.Append("<select id=\"ChampChampionship\" size=\"3\" onchange=\"ChampionshipChanged(this);\">");
			strHTML.Append("</select><br /><br />");
			if (showCategory)
			{
				strHTML.Append("בחר קטגורית אליפות:<br />");
				strHTML.Append("<select id=\"ChampCategory\" size=\"3\" onchange=\"CategoryChanged(this);\">");
				strHTML.Append("</select><br />");
			}
			strHTML.Append("<input type=\"button\" value=\"הצב ערך זה\" id=\"btnSetChamp\" ");
			strHTML.Append("disabled=\"disabled\" onclick=\"SetChampionship();\" />");
			strHTML.Append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input type=\"button\" ");
			strHTML.Append("value=\"ביטול\" onclick=\"CancelSetChampionship();\" />");
			strHTML.Append("</div>");
			
			return strHTML.ToString();
		}

		private string BuildPermanentChampHtml(LinkManager.LinkData[] links, int index)
		{
			System.Text.StringBuilder sb=new System.Text.StringBuilder();
			
			LinkManager.LinkData linkData=links[index];
			string strIndex=(index+1).ToString();
			sb.Append("<span id=\"PermanentChamp_"+strIndex+"\">");
			string strHebIndex=Sport.Common.Tools.IntToHebrew(index+1, true);
			string strColor=
				Sport.Common.Tools.ColorToHex(Common.Style.PermanentChampBgColor);
			
			//get actual category:
			Sport.Entities.ChampionshipCategory category=null;
			if ((linkData.URL != null)&&(linkData.URL.Length > 0))
			{
				string[] arrTmp1=linkData.URL.Split(new char[] {'?'});
				string[] arrTmp2=arrTmp1[1].Split(new char[] {'&'});
				foreach (string tempKey in arrTmp2)
				{
					string[] arrTmp3=tempKey.Split(new char[] {'='});
					if (arrTmp3[0] == "category")
					{
						try
						{
							category = new ChampionshipCategory(Int32.Parse(arrTmp3[1]));
						}
						catch
						{
							category = null;
						}
						if ((category.Championship == null)||(category.Championship.Id < 0))
							category = null;
						break;
					}
				}
			}
			
			//fieldset caption:
			sb.Append("<fieldset style=\"width: 380px; background-color: "+strColor);
			sb.Append(";\"><legend>ליגה קבועה "+strHebIndex+"</legend>");
			
			//championship display name:
			sb.Append("שם אליפות לתצוגה: ");
			sb.Append("<input type=\"text\" name=\"DisplayName_"+strIndex+"\" ");
			sb.Append("maxlength=\"128\" size=\"30\" dir=\"rtr\" ");
			sb.Append("value=\""+linkData.Text.Replace("\"", "&quot;")+"\" />");
			
			//championship actual name:
			sb.Append("<br />שם אליפות אמיתי: ");
			sb.Append("<input type=\"text\" name=\"ChampionshipName_"+strIndex+"\" ");
			sb.Append("size=\"30\" readonly=\"readonly\" value=\"");
			if (category != null)
				sb.Append(category.Championship.Name.Replace("\"", "&quot;"));
			sb.Append("\" dir=\"rtl\" />&nbsp;&nbsp;&nbsp;");
			
			//choose championship:
			sb.Append("<input type=\"button\" name=\"btnChooseChamp_"+strIndex+"\" ");
			sb.Append("value=\"...\" onclick=\"ChooseChampCategory(event, this);\" />");
			
			//clear championship:
			sb.Append(Tools.MultiString("&nbsp;", 3));
			sb.Append("<a href=\"javascript:ClearChamp("+strIndex+");\">[מחק]</a>");
			
			//category ID:
			sb.Append("<input type=\"hidden\" name=\"CategoryID_"+strIndex+"\" ");
			sb.Append("value=\"");
			if (category != null)
				sb.Append(category.Id.ToString());
			sb.Append("\" />");
			
			//done.
			sb.Append("</fieldset></span>");
			return sb.ToString();
		} //end function BuildLinkHtml
		#endregion
		
		#region Events
		private void AddEventRange(WebSiteServices.WebsiteService service, 
			string strDescription, string strURL, DateTime dtEventDate, int range)
		{
			//valid?
			if ((range <= 0)||(range > 365))
				return;
			if ((strDescription == null)||(strDescription.Length == 0))
				return;
			if (dtEventDate.Year < 1900)
				return;
			
			//set hour:
			DateTime dtCurDate=Sport.Common.Tools.SetTime(dtEventDate, 7, 0, 0);

			//get existing events:
			DateTime dtStart=dtCurDate;
			DateTime dtEnd=dtStart+new TimeSpan(range, 0, 0, 0);
			WebSiteServices.EventData[] arrEvents=service.GetEvents(dtStart, dtEnd);
			for (int i=0; i<range; i++)
			{
				//get date string:
				string strCurDate=dtCurDate.ToString("dd/MM/yyyy");
				
				//exists?
				bool blnExists=false;
				foreach (WebSiteServices.EventData curEvent in arrEvents)
				{
					if ((curEvent.Description.ToLower() == strDescription.ToLower())&&
						(Sport.Common.Tools.IsSameDate(curEvent.Date, dtCurDate)))
					{
						blnExists = true;
						break;
					}
				}
				if (blnExists)
				{
					AddErrorMessage("אירוע זה כבר קיים בתאריך "+strCurDate);
				}
				else
				{
					//add new record...
					WebSiteServices.EventData eventData=
						new WebSiteServices.EventData();
					
					//apply ID for new event:
					eventData.ID = -1;
					
					//apply date:
					eventData.Date = dtCurDate;
					
					//apply description and URL...
					eventData.Description = strDescription;
					eventData.URL =  Tools.CStrDef(strURL, "");
					
					//add to database:
					service.UpdateEventData(eventData, 
						_user.Login, _user.Password);
					
					//done for current day
					AddSuccessMessage("אירוע נוסף בהצלחה בתאריך "+strCurDate);
				} //end if does not exist already
				dtCurDate += new TimeSpan(1, 0, 0, 0);
			} //end loop over days in range
		} //end function AddEventRange
		
		private void EditEvents()
		{
			//verify user has permissions!
			/* if (!Tools.IsAuthorized_News(_user.Id))
				throw new Exception("You are not authorized to use this page!"); */
			
			_pageCaption = "עריכת אירועים";
			WebSiteServices.WebsiteService websiteService=
				new WebSiteServices.WebsiteService();
			websiteService.CookieContainer = Sport.Core.Session.Cookies;
			int eventID=Tools.CIntDef(Request.QueryString["id"], -1);
			string strEventDescription = Tools.CStrDef(Request.Form["description"], "");
			string strEventURL = Tools.CStrDef(Request.Form["URL"], "");
			int range = Tools.CIntDef(Request.Form["range"], 1);
			DateTime dtEventDate = Tools.GenerateDateTimeDef(this.Request, "event", 
				DateTime.MinValue);
			if (eventID >= 0)
			{
				WebSiteServices.EventData eventData=
					websiteService.GetEventData(eventID);
				if (eventData.ID < 0)
				{
					AddErrorMessage("זיהוי אירוע שגוי");
				}
				else
				{
					//delete?
					if (Request.QueryString["delete"] == "1")
					{
						if (Request.Form["Confirm"] == "1")
						{
							websiteService.DeleteEventData(eventData.ID, 
								_user.Login, _user.Password);
							AddSuccessMessage("אירוע נמחק בהצלחה ממאגר הנתונים");
						}
						else
						{
							if (Request.Form["Confirm"] == "0")
							{
								Response.Redirect("Register.aspx?action="+SportSiteAction.UpdateEvents);
							}
							else
							{
								AddViewText(
									Tools.BuildConfirmDeleteHtml("אירוע זה"), true);
								return;
							}
						}
					}
					else
					{
						//update?
						if (Request.Form["Update"] == "1")
						{
							
							eventData.Description = strEventDescription;
							eventData.URL = strEventURL;
							if (dtEventDate.Year < 1900)
							{
								AddErrorMessage("תאריך אירוע שגוי");
								AddViewText(BuildEventHTML(eventData), true);
								AddViewText("<input type=\"hidden\" name=\"Update\" "+
									"value=\"1\" />");
								return;
							}
							eventData.Date = dtEventDate;
							websiteService.UpdateEventData(eventData, 
								_user.Login, _user.Password);
							AddSuccessMessage("נתוני האירוע עודכנו בהצלחה");
							if (range > 1)
							{
								dtEventDate += new TimeSpan(1, 0, 0, 0);
								AddEventRange(websiteService, strEventDescription, 
									strEventURL, dtEventDate, range-1);
							}
						}
						else
						{
							AddViewText(BuildEventHTML(eventData), true);
							AddViewText("<input type=\"hidden\" name=\"Update\" "+
								"value=\"1\" />");
							return;
						}
					}
				}
			}
			
			if (Request.QueryString["id"] == "new")
			{
				WebSiteServices.EventData eventData=new WebSiteServices.EventData();
				eventData.ID = -1;
				eventData.Date = DateTime.Now;
				if (strEventDescription.Length > 0)
				{
					//add new record...
					AddEventRange(websiteService, strEventDescription, 
						strEventURL, dtEventDate, range);
				}
				else
				{
					AddViewText(BuildEventHTML(eventData), true);
					return;
				}
			}
			
			WebSiteServices.EventData[] arrEvents=
				websiteService.GetEvents(DateTime.MinValue, DateTime.MinValue);
			
			if (Request.QueryString["updatenews"] == "1")
			{
				DateTime lastEventDay=DateTime.MinValue;
				WebSiteServices.FlashNewsData[] arrCurrentNews=null;
				int newCount=0;
				DateTime today=Sport.Common.Tools.SetTime(DateTime.Now, 0, 0, 0);
				foreach (WebSiteServices.EventData eventData in arrEvents)
				{
					DateTime curEventDay=eventData.Date;
					if (curEventDay < today)
					{
						lastEventDay = curEventDay;
						continue;
					}
					if ((curEventDay.Year != lastEventDay.Year)||
						(curEventDay.Month != lastEventDay.Month)||
						(curEventDay.Day != lastEventDay.Day))
					{
						arrCurrentNews = websiteService.GetNewsInRange(
							curEventDay, curEventDay);
					}
					bool blnExists=false;
					if (arrCurrentNews != null)
					{
						foreach (WebSiteServices.FlashNewsData curNews in arrCurrentNews)
						{
							if (curNews.Contents == eventData.Description)
							{
								blnExists = true;
								break;
							}
						}
					}
					
					if (!blnExists)
					{
						WebSiteServices.FlashNewsData flashNews=
							new WebSiteServices.FlashNewsData();
						flashNews.ID = -1;
						flashNews.Time = Sport.Common.Tools.SetTime(
							curEventDay, 7, 0, 0);
						flashNews.Contents = eventData.Description;
						if ((eventData.URL != null)&&(eventData.URL.Length > 0))
						{
							flashNews.Links = new WebSiteServices.LinkData[1];
							flashNews.Links[0] = new WebSiteServices.LinkData();
							flashNews.Links[0].Text = flashNews.Links[0].URL = eventData.URL;
						}
						websiteService.UpdateFlashNews(flashNews, 
							_user.Login, _user.Password, _user.Id);
						newCount++;
					}
					lastEventDay = curEventDay;
				} //end loop over existing events
				
				if (newCount > 0)
				{
					AddViewText("<span style=\"color: blue;\">"+
						Tools.HebrewCount(newCount, true)+" מבזקים נוספו בהצלחה"+
						"</span><br /><br />");
				}
			} //end if user asked to update flashnews
			
			AddViewText("אירועים קיימים:<br />");
			string strNewHTML="[<a href=\"?action="+SportSiteAction.UpdateEvents+
				"&id=new\" title=\"הוספת אירוע\" style=\"\">"+
				"הוסף אירוע</a>]<br />";
			AddViewText(strNewHTML);
			AddViewText(BuildEventsTable(arrEvents)+"<br /><br />");
			AddViewText(strNewHTML+"<br /><br />");
			AddViewText("[<a href=\"?action="+SportSiteAction.UpdateEvents+
				"&updatenews=1\">עדכון מבזקים</a>]<br /><br />");
		} //end function EditEvents
		
		private string BuildEventsTable(WebSiteServices.EventData[] events)
		{
			System.Text.StringBuilder sb=new System.Text.StringBuilder();
			bool blnShowOld=(Request.QueryString["old"] == "1");
			
			sb.Append("<table id=\"TblEvents\" border=\"1\">");
			sb.Append("<tr>");
			sb.Append("<th>תאריך</th>");
			sb.Append("<th>תיאור</th>");
			sb.Append("<th>קישור</th>");
			sb.Append("<th>&nbsp;</th>");
			sb.Append("</tr>");
			DateTime now=Sport.Common.Tools.SetTime(DateTime.Now, 0, 0, 0);
			for (int i=0; i<events.Length; i++)
			{
				WebSiteServices.EventData curEvent=events[i];
				int curEventID=curEvent.ID;
				string strURL=curEvent.URL;
				string curDescription=curEvent.Description;
				DateTime curEventTime=curEvent.Date;
				if ((blnShowOld == false)&&(curEventTime < now))
					continue;
				string strLink="<a style=\"\" href=\"?action="+SportSiteAction.UpdateEvents;
				strLink += "&id="+curEventID+"\" title=\"%title\">";
				if (strURL == null)
					strURL = "";
				sb.Append("<tr>");
				sb.Append("<td>"+strLink.Replace("%title", "ערוך אירוע זה"));
				sb.Append(events[i].Date.ToString("dd/MM/yyyy")+"</a></td>");
				sb.Append("<td>"+strLink.Replace("%title", "ערוך אירוע זה"));
				sb.Append(Tools.TruncateString(curDescription, 30)+"</a></td>");
				sb.Append("<td>");
				if (strURL.Length > 0)
				{
					sb.Append(strLink.Replace("%title", strURL));
					sb.Append(Tools.TruncateString(strURL, 30)+"</a>");
				}
				else
				{
					sb.Append("&nbsp;");
				}
				sb.Append("</td>");
				sb.Append("<td><a href=\"?action="+SportSiteAction.UpdateEvents);
				sb.Append("&id="+curEventID+"&delete=1\" style=\"color: red;\">");
				sb.Append("[מחק אירוע זה]</a></td>");
				sb.Append("</tr>");
			}
			sb.Append("</table>");
			if (!blnShowOld)
			{
				sb.Append("<br />[<a href=\"?action="+SportSiteAction.UpdateEvents+
					"&old=1\">הצג את כל האירועים</a>]");
			}
			sb.Append("");
			return sb.ToString();
		} //end function BuildEventsTable
		
		private string BuildEventHTML(WebSiteServices.EventData data)
		{
			System.Text.StringBuilder sb=new System.Text.StringBuilder();
			
			int day=data.Date.Day;
			int month=data.Date.Month;
			int year=data.Date.Year;
			
			sb.Append("<div align=\"right\" dir=\"rtl\">");
			
			sb.Append("תאריך האירוע: ");
			sb.Append(BuildDateSelection("event", day, month, year));
			sb.Append("<br />");

			sb.Append("תיאור האירוע: <br />");
			sb.Append("<textarea name=\"description\" rows=\"10\" cols=\"45\" dir=\"rtl\">");
			sb.Append(data.Description+"</textarea><br />");
			
			sb.Append("קישור:<br /><input type=\"text\" name=\"URL\" size=\"50\" value=\""+data.URL+"\" />&nbsp;");
			sb.Append("<a style=\"\" href=\"javascript:OpenPreview('URL');\">[תצוגה מקדימה]</a>");
			sb.Append("<br /><br />");
			
			sb.Append("טווח ימים: <input type=\"text\" name=\"range\" "+
				"size=\"3\" maxlength=\"3\" value=\"1\" />");
			sb.Append("<br /><br />");
			
			sb.Append(Tools.BuildSubmitButton(null));
			sb.Append("</div>");
			
			return sb.ToString();
		}
		
		private int _dateSelectionCount=0;
		private string BuildDateSelection(string strCaption, 
			int selectedDay, int selectedMonth, int selectedYear)
		{
			_dateSelectionCount++;
			
			System.Text.StringBuilder sb=new System.Text.StringBuilder();
			DateTime now=DateTime.Now;
			
			if (selectedDay <= 0)
				selectedDay = now.Day;
			if (selectedMonth <= 0)
				selectedMonth = now.Month;
			if (selectedYear < 1900)
				selectedYear = now.Year;
			
			//year:
			int yearStart=now.Year;
			if (now.Month < 9)
				yearStart -= 1;
			sb.Append("<select name=\""+strCaption+"_year\" onchange=\"DatePreview(this, '"+strCaption+"');\">");
			sb.Append(Tools.BuildOptionsRange(yearStart, yearStart+1, selectedYear));
			sb.Append("</select>");
			sb.Append("&nbsp;/&nbsp;");
			
			//month:
			sb.Append("<select name=\""+strCaption+"_month\" onchange=\"DatePreview(this, '"+strCaption+"');\">");
			sb.Append(Tools.BuildOptionsRange(1, 12, selectedMonth));
			sb.Append("</select>");
			sb.Append("&nbsp;/&nbsp;");
			
			//day:
			sb.Append("<select name=\""+strCaption+"_day\" onchange=\"DatePreview(this, '"+strCaption+"');\">");
			sb.Append(Tools.BuildOptionsRange(1, 31, selectedDay));
			sb.Append("</select>");
			sb.Append(Tools.MultiString("&nbsp;", 5));
			
			//preview:
			sb.Append("<span dir=\"rtl\" style=\"font-size: 12px; font-weight: bold;\" id=\""+strCaption+"_date_preview\"></span>");
			sb.Append("<iframe name=\"frm_date_preview\" id=\"frm_date_preview_"+strCaption+"\" style=\"display: none;\"></iframe>");
			
			IsfMainView.clientSide.AddOnloadCommand("DatePreview(document.forms[0].elements[\""+strCaption+"_year\"], '"+strCaption+"');", true);
			return sb.ToString();
		}
		
		private DateTime GetDateSelectionDef(string strCaption, DateTime defValue)
		{
			DateTime result=defValue;
			int day=Tools.CIntDef(Request.Form[strCaption+"_day"], -1);
			int month=Tools.CIntDef(Request.Form[strCaption+"_month"], -1);
			int year=Tools.CIntDef(Request.Form[strCaption+"_year"], -1);
			if ((day >= 1)&&(day <= 31)&&(month >= 1)&&(month <= 12)&&(year >= 2005))
			{
				try
				{
					result = new DateTime(year, month, day);
				}
				catch
				{
					result = defValue;
				}
			}
			return result;
		}

		private void CheckDatePreview()
		{
			if (Request.QueryString["action"] != "check_date_preview")
				return;

			int day=Int32.Parse(Request.QueryString["day"]);
			int month=Int32.Parse(Request.QueryString["month"]);
			int year=Int32.Parse(Request.QueryString["year"]);

			DateTime date=DateTime.MinValue;
			string strDate="";
			try
			{
				date = new DateTime(year, month, day);
			}
			catch
			{}

			if (date.Year < 1900)
			{
				strDate = "תאריך שגוי!";
			}
			else
			{
				//strDate = Tools.HebrewCount(date.Day, true)+" ב"+HebDateTime.HebMonthName(date.Month)+" "+
				//	Tools.HebrewCount(date.Year, false);
				strDate = SportSite.Controls.HebDateTime.HebDates.JewishDate(date, "%hwd %hmd ב%y");
			}
			
			string containerID=Request.QueryString["caption"]+"_date_preview";
			Response.Write("<script type=\"text/javascript\">");
			Response.Write(" var objContainer=parent.document.getElementById(\""+containerID+"\");");
			Response.Write(" objContainer.style.color = \""+((date.Year<1900)?"red":"blue")+"\";");
			Response.Write(" objContainer.innerHTML = \""+strDate.Replace("\"", "\\\"")+"\";");
			Response.Write("</script>");
			
			Response.End();
		}
		#endregion
		
		#region players register form
		private void CheckPlayersRegisterForm()
		{
			if (Request.QueryString["action"] != "ShowPlayersRegisterForm")
				return;

			int teamID=Tools.CIntDef(Request.QueryString["team"], -1);
			if (teamID < 0)
				throw new Exception("invalid team ID!");
			
			Team team=null;
			try
			{
				team = new Team(teamID);
			}
			catch {}
			if ((team == null)||(team.Id < 0))
				throw new Exception("invlid team!");
			
			Sport.Entities.Player[] arrPlayers=team.GetPlayers();
			int curPage=Tools.CIntDef(Request.Form["page"], 1);
			string strHTML=Tools.BuildPlayersRegisterForm(team, arrPlayers, this.Server, 
				curPage, this.Session);
			Response.Write(strHTML);
			
			Response.End();
		}
		#endregion

		#region Links
		private string BuildLinkHTML(WebSiteServices.LinkData[] links, int linkIndex, 
			string strCaption, int parentID)
		{
			System.Text.StringBuilder sb=new System.Text.StringBuilder();
			WebSiteServices.LinkData link=new WebSiteServices.LinkData();
			if (linkIndex >= 0)
			{
				link = links[linkIndex];
			}
			else
			{
				link.Text = "";
				link.URL = "";
			}
			string strIndex="_new";
			if (linkIndex >= 0)
				strIndex = "_"+linkIndex.ToString();
			sb.Append("<span id=\""+strCaption+"_link"+strIndex+"\">");
			string strHebIndex="חדש";
			if (linkIndex >= 0)
				strHebIndex = Sport.Common.Tools.IntToHebrew(linkIndex+1, false);

			//fieldset caption:
			sb.Append("<fieldset style=\"width: "+Common.Style.LinkFieldsetWidth);
			sb.Append("px; background-color: "+Sport.Common.Tools.ColorToHex(
				Common.Style.LinkFieldsetBgColor)+";\">");
			sb.Append("<legend>קישור "+strHebIndex+"</legend>");
			
			//link URL
			sb.Append("כתובת: ");
			sb.Append("<input type=\"text\" name=\"LinkUrl"+strIndex+"\" ");
			sb.Append("maxlength=\"128\" size=\"50\" dir=\"ltr\" ");
			sb.Append("value=\""+link.URL+"\" onkeyup=\"ApplyLinkPreview(this);\" />");
			
			//delete?
			if (linkIndex >= 0)
			{
				sb.Append(Tools.MultiString("&nbsp;", 5)+"<a href=\"");
				sb.Append("?action=deletelink&"+strCaption+"_id="+parentID+"&index=");
				sb.Append(linkIndex+"\" id=\"DeleteLink_"+linkIndex+"\" ");
				sb.Append("style=\"\" target=\"DeleteLinkFrame\" ");
				sb.Append("onclick=\"return confirm('?האם למחוק קישור זה');\">");
				sb.Append("[מחק קישור]</a>");
			}
			
			//link text:
			sb.Append("<br />טקסט: ");
			sb.Append("<input type=\"text\" name=\"LinkText"+strIndex+"\" ");
			sb.Append("maxlength=\"50\" value=\""+link.Text+"\" ");
			sb.Append("dir=\"rtl\" onkeyup=\"ApplyLinkPreview(this);\" />");
			
			//link preview:
			sb.Append(Tools.MultiString("&nbsp;", 5)+"<a id=\"LinkPreview"+strIndex+"\" ");
			sb.Append("style=\"\" target=\"_blank\"");
			if ((link.URL != null)&&(link.URL.Length > 0))
				sb.Append(" href=\""+link.URL+"\"");
			sb.Append(">");
			if ((link.Text != null)&&(link.Text.Length > 0))
				sb.Append(link.Text);
			sb.Append("</a>");
			sb.Append("</fieldset></span>");
			
			//done.
			return sb.ToString();
		} //end function BuildLinkHTML
		#endregion
		
		#region image gallery
		private void EditImageGallery()
		{
			//verify user has permissions!
			if (_user.Id < 0)
				throw new Exception("You are not authorized to use this page!");
			
			//need to create folder?
			if (!Directory.Exists(Server.MapPath(Common.Data.ImageGalleryFolder)))
				Directory.CreateDirectory(Server.MapPath(Common.Data.ImageGalleryFolder));
			
			ArticleImage = new System.Web.UI.HtmlControls.HtmlInputFile();
			ArticleImage.Style["display"] = "none";
			IsfMainView.AddContents(ArticleImage);
			
			//initialize html string builder
			System.Text.StringBuilder sb=new System.Text.StringBuilder();
			_pageCaption = "עריכת גלריית תמונות";
			
			//build html...
			sb.Append("<div dir=\"rtl\" id=\"EditGalleryPanel\" "+
				"style=\"font-size: 12px; font-weight: bold;\">");
			
			//need to update specific image?
			if (CheckUpdateGalleryImage(sb))
				return;
			
			//need to change order?
			if (Request.Form["GroupsOrder"] != null)
			{
				string strGroupsOrder=Tools.CStrDef(Request.Form["GroupsOrder"], "");
				if (strGroupsOrder.Length > 0)
				{
					Common.Tools.WriteIniValue("ImageGallery", "GroupsOrder", 
						strGroupsOrder, Server);
					Common.Data.RebuildGalleryGroups(this.Server);
				}
			}
			
			//need to delete image from gallery?
			//CheckDeleteGalleryImage(sb);
			
			//got new image?
			string strGroupName="";
			string strSubGroup="";
			CheckNewGalleryImage(sb, ref strGroupName, ref strSubGroup);
			
			//new picture
			string[] arrGroups=null;
			sb.Append("<fieldset style=\"width: 450px;\"><legend>תמונה חדשה</legend>");
			sb.Append("קובץ תמונה: ");
			sb.Append(FastControls.InputFile("NewImage", false)+"<br />");
			sb.Append("תיאור:&nbsp;&nbsp;&nbsp;&nbsp;");
			sb.Append(FastControls.InputBox("ImageDescription", 
				Request.Form["ImageDescription"], 255, 40)+"<br />");
			sb.Append(BuildGalleryGroupsHTML(strGroupName, strSubGroup, ref arrGroups));
			sb.Append("<br />");
			sb.Append(Tools.BuildSubmitButton(null));
			sb.Append("</fieldset>");
			
			//groups order
			string[] arrValues=null;
			if (arrGroups != null)
			{
				string strOrder=Common.Data.GetGalleryGroupsOrder(this.Server);
				if ((strOrder != null)&&(strOrder.Length > 0))
				{
					//get and fix the values:
					string[] arrOrder=Sport.Common.Tools.SplitNoBlank(strOrder, '|');
					arrValues = Tools.FixNumericArray(arrOrder, arrGroups.Length);
				}
				else
				{
					arrValues = new string[arrGroups.Length];
					for (int j=0; j<arrGroups.Length; j++)
						arrValues[j] = j.ToString();
				}
				//Response.Write("groups: "+String.Join(", ", arrGroups)+"<br />");
				//Response.Write("values: "+String.Join(", ", arrValues)+"<br />");
			}
			sb.Append("<fieldset style=\"width: 450px;\"><legend>סדר הופעת הנושאים</legend>");
			sb.Append(Tools.BuildUpDownCombo("GroupsOrder", "GalleryGroups", 
				arrValues, arrGroups, "|", "נושא", true)+"<br />");
			sb.Append(Tools.BuildSubmitButton(null));
			sb.Append("</fieldset>");
			
			//existing pictures
			AddGalleryImages(sb);
			
			//done.
			sb.Append("</div>");
			AddViewText(sb.ToString(), true);
		} //end function EditImageGallery
		
		/// <summary>
		/// check if user asked to delete gallery image.
		/// </summary>
		private void DeleteGalleryImage(System.Text.StringBuilder sb, int imageID, 
			WebSiteServices.WebsiteService service)
		{
			//error message:
			string strBaseError="<span style=\"color: red;\">%text</span><br />";
			
			//get image data:
			WebSiteServices.ImageData image=service.GetGalleryImage(imageID);
			
			//valid?
			if ((image == null)||(image.ID < 0))
			{
				sb.Append(strBaseError.Replace("%text", "זיהוי תמונה שגוי: "+imageID));
				return;
			}
			
			//delete from disk:
			try
			{
				string strBasePath=Server.MapPath(Data.ImageGalleryFolder+"/%image");
				System.IO.File.Delete(strBasePath.Replace("%image", image.PictureName));
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("failed to delete gallery images: "+ex.Message);
				System.Diagnostics.Debug.WriteLine("picture: "+image.PictureName);
			}
			
			//delete from database:
			service.DeleteGallreyImage(imageID);
			
			//done.
			sb.Append("<span style=\"color: blue;\">התמונה \""+image.Description+"\" "+
				"נמחקה בהצלחה מגלריית התמונות</span><br />");
		} //end function CheckDeleteGalleryImage
		
		/// <summary>
		/// check if user asked to edit gallery image and if so, let him edit.
		/// </summary>
		private bool CheckUpdateGalleryImage(System.Text.StringBuilder sb)
		{
			//user asked to edit?
			if (Request.QueryString["edit"] != "1")
				return false;
			
			//maybe new image?
			if ((Request.Files["NewImage"] != null)&&(Request.Files["NewImage"].ContentLength > 0))
				return false;
			
			//define base error message:
			string strBaseError="<span style=\"color: red; font-weight: bold;\">%text</span><br />";
			
			//read the desired image:
			int imageID=Tools.CIntDef(Request.QueryString["id"], -1);
			if (imageID < 0)
			{
				sb.Append(strBaseError.Replace("%text", "זיהוי תמונה שגוי"));
				return false;
			}
			
			//initialize service:
			WebSiteServices.WebsiteService websiteService=
				new WebSiteServices.WebsiteService();
			
			//get image data:
			WebSiteServices.ImageData image=websiteService.GetGalleryImage(imageID);
			
			//valid?
			if ((image == null)||(image.ID < 0))
			{
				sb.Append(strBaseError.Replace("%text", "תמונה שגויה"));
				return false;
			}
			
			//need to edit?
			if (Request.Form["ImageDescription"] != null)
			{
				string strDescription=Tools.CStrDef(Request.Form["ImageDescription"], "");
				if (strDescription.Length == 0)
				{
					sb.Append(strBaseError.Replace("%text", "תיאור תמונה לא יכול להיות ריק"));
				}
				else
				{
					image.Description = strDescription;
					image.GroupName = GetRequestDataDef("GroupNameCombo", "GroupNameText");
					image.SubGroup = GetRequestDataDef("SubGroupCombo", "SubGroupText");
					websiteService.UpdateGalleryImage(image);
					sb.Append("<span style=\"font-weight: bold;\">"+
						"נתוני תמונה עודכנו בהצלחה</span><br />");
					return false;
				}
			} //end if need to edit
			
			//add HTML.
			sb.Append("<div align=\"right\">"+Tools.BuildThumbnailHTML(image, this.Server)+"</div>");
			sb.Append("תיאור:&nbsp;&nbsp;&nbsp;&nbsp;");
			sb.Append(FastControls.InputBox("ImageDescription", 
				image.Description, 255, 40)+"<br />");
			sb.Append(BuildGalleryGroupsHTML(image.GroupName, image.SubGroup)+"<br />");
			sb.Append(Tools.BuildSubmitButton(null));
			
			//done.
			sb.Append("</div>");
			AddViewText(sb.ToString(), true);
			return true;
		} //end function CheckUpdateGalleryImage

		private void AddGalleryImages(System.Text.StringBuilder sb)
		{
			//define table width:
			int iTableWidth=500;
			
			//calculate details:
			int picsPerRow=3;
			int iCellWidth=(int) ((((double) 100)/((double) picsPerRow)));
			
			//initialize service:
			WebSiteServices.WebsiteService websiteService=
				new WebSiteServices.WebsiteService();
			
			//get desired group:
			int iSelGroup=Tools.CIntDef(Request.QueryString["group"], -1);
			
			//get existing images:
			WebSiteServices.ImageData[] arrImages=
				websiteService.GetAllGalleryImages();
			
			//got anything?
			if (arrImages == null)
				return;
			
			//declare groups table
			Hashtable tblGroups=new Hashtable();
			
			//iterate over the gallery images, add to table and check updates
			foreach (WebSiteServices.ImageData image in arrImages)
			{
				//get form data:
				string strNewDescription=Tools.CStrDef(Request.Form["description_"+image.ID], "");
				bool blnDelete=(Request.Form["delete_"+image.ID] == "1");
				
				//delete?
				if (blnDelete)
				{
					DeleteGalleryImage(sb, image.ID, websiteService);
					continue;
				}
				
				//update?
				if ((strNewDescription.Length > 0)&&(strNewDescription != image.Description))
				{
					image.Description = strNewDescription;
					websiteService.UpdateGalleryImage(image);
				}
				
				//add to groups table:
				string strCurGroup=image.GroupName;
				tblGroups[strCurGroup] = Tools.CIntDef(tblGroups[strCurGroup], 0)+1;
			}
			
			//valid?
			if (iSelGroup >= tblGroups.Keys.Count)
				iSelGroup = -1;
			
			//add HTML.
			if (iSelGroup < 0)
			{
				sb.Append("<h3>בחר נושא לעריכה:</h3>");
				sb.Append("<ul>");
				int index=0;
				foreach (string strCurGroup in tblGroups.Keys)
				{
					sb.Append("<li>");
					sb.Append("<a href=\"?action="+SportSiteAction.UpdateImageGallery+
						"&group="+index.ToString()+"\">"+strCurGroup+" ("+
						Sport.Common.Tools.BuildOneOrMany("תמונה", "תמונות", 
						(int) tblGroups[strCurGroup], false)+")</a>");
					sb.Append("</li>");
					index++;
				}
				sb.Append("</ul>");
			}
			else
			{
				int index=0;
				int picIndex=0;
				string strSelGroup="";
				foreach (string strCurGroup in tblGroups.Keys)
				{
					strSelGroup = strCurGroup;
					if (index == iSelGroup)
					{
						break;
					}
					index++;
				}

				sb.Append(Tools.BuildPageSubCaption(strSelGroup, 
					this.Server, IsfMainView.clientSide));
				sb.Append("<center>");
				sb.Append("<table cellspacing=\"0\" cellpadding=\"0\" "+
					"style=\"width: "+iTableWidth+"px; background-color: #D2D2FF;\">");
				string strBaseCell="<td alt_color=\"#ffd2d2\" style=\"width: "+
					iCellWidth+"%; border: 1px solid black;\">%text</td>";
				int count=0;
				while (picIndex<arrImages.Length)
				{
					WebSiteServices.ImageData curImage=arrImages[picIndex];
					if (curImage.GroupName == strSelGroup)
					{
						if (Request.Form["delete_"+curImage.ID] != "1")
						{
							if ((count == 0)||((count % picsPerRow) == 0))
							{
								if (count > 0)
									sb.Append("</tr>");
								sb.Append("<tr>");
							}
							sb.Append(strBaseCell.Replace("%text", 
								BuildGalleryImageHTML(curImage)));
							count++;
						}
					}
					picIndex++;
				}
				if ((count > 0)&&((count % picsPerRow) != 0))
				{
					for (int i=count; i<picsPerRow; i++)
						sb.Append(strBaseCell.Replace("%text", "&nbsp;"));
					sb.Append("</tr>");
				}
				sb.Append("</table><br />");
				string strConfirmDelete="סומנו למחיקה %num תמונות. האם למחוק תמונות אלו?";
				Page.RegisterClientScriptBlock("confirm_gallery_delete", 
					"<script type=\"text/javascript\">var _confirmDeleteImage=\""+strConfirmDelete+"\";</script>");
				sb.Append(Tools.BuildSubmitButton("return CheckDeleteImages(this);", ""));
				sb.Append("</center>");
			}
		}
		
		private string BuildGalleryImageHTML(WebSiteServices.ImageData image)
		{
			System.Text.StringBuilder result=new System.Text.StringBuilder();
			string strGroupName=Tools.CStrDef(image.GroupName, "");
			if (strGroupName.Length == 0)
				strGroupName = "לא שייך לקבוצה";
			//string strConfirmMessage="האם למחוק תמונה זו?";
			result.Append("<div style=\"text-align: center; background-color: #B9B9B9;\">"+
				strGroupName+"</div>");
			result.Append("<div style=\"text-align: center; background-color: #D2D2D2;\">"+
				image.SubGroup+"</div>");
			result.Append("<div>"+Tools.BuildThumbnailHTML(image, this.Server, true)+"</div>");
			result.Append("<div>מספר צפיות: "+image.ViewCount+"</div>");
			result.Append("<div style=\"text-align: center; background-color: #ffffff;\">"+
				"<a href=\"?action="+Common.SportSiteAction.UpdateImageGallery.ToString()+
				"&edit=1&id="+image.ID+"\">[שינוי קבוצה או נושא]</a></div>");
			result.Append("<div style=\"text-align: center; background-color: #ffffff;\">");
			result.Append("<input type=\"checkbox\" name=\"delete_"+image.ID+"\" "+
				"value=\"1\" onclick=\"DeleteImageClick(this);\" />"+
				"סמן למחיקה");
			result.Append("</div>");
			return result.ToString();
		}
		
		private void CheckNewGalleryImage(System.Text.StringBuilder sb, 
			ref string groupName, ref string subGroupName)
		{
			//prefix:
			string strZipPrefix="tmp_zip_";
			
			//previous folders?
			string[] arrFolders=Directory.GetDirectories(Server.MapPath("temp"));
			foreach (string strCurFolderPath in arrFolders)
			{
				if (Path.GetFileName(strCurFolderPath).ToLower().StartsWith(strZipPrefix))
					Directory.Delete(strCurFolderPath, true);
			}
			
			//get posted file:
			HttpPostedFile objImageFile=Request.Files["NewImage"];
			string strBaseError="<span style=\"color: red; font-weight: bold;\">%error</span><br />";
			
			//initialize service:
			WebSiteServices.WebsiteService websiteService=
				new WebSiteServices.WebsiteService();
			
			//got anything?
			if ((objImageFile != null)&&(objImageFile.ContentLength > 0))
			{
				//read user data:
				string strDescription=Tools.CStrDef(Request.Form["ImageDescription"], "");
				string strGroupName = GetRequestDataDef("GroupNameCombo", "GroupNameText");
				string strSubGroup = GetRequestDataDef("SubGroupCombo", "SubGroupText");
				ArrayList arrImages=new ArrayList();
				string strErrorMessage="";
				string strTempPath="";
				bool blnZip=false;
				string strOriginalName="";
				
				//apply:
				groupName= strGroupName;
				subGroupName = strSubGroup;
				
				//zip file?
				if (Path.GetExtension(objImageFile.FileName).ToLower() == ".zip")
				{
					blnZip = true;
					string strZipPath=Server.MapPath("temp/temp_zip_file.zip");
					objImageFile.SaveAs(strZipPath);
					string strFolderPath=Server.MapPath("temp/"+strZipPrefix+Tools.CurrentMilliSeconds());
					if (!Directory.Exists(strFolderPath))
						Directory.CreateDirectory(strFolderPath);
					Server.ScriptTimeout = 1800;
					Common.ZipFiles.ExtractTo(strZipPath, strFolderPath, this.Server);
					File.Delete(strZipPath);
					string[] arrFiles=Directory.GetFiles(strFolderPath);
					foreach (string strCurrentFile in arrFiles)
					{
						if (Tools.IsValidImage(strCurrentFile))
							arrImages.Add(strCurrentFile);
					}
					strTempPath = strFolderPath;
					if (arrImages.Count == 0)
						strErrorMessage = "לא נמצאו תמונות";
				}
				else
				{
					//valid image?
					if (Tools.IsValidImage(objImageFile.InputStream))
					{
						strTempPath = Server.MapPath("temp/tmp_file_"+
							Tools.CurrentMilliSeconds()+
							Path.GetExtension(objImageFile.FileName));
						objImageFile.SaveAs(strTempPath);
						arrImages.Add(strTempPath);
						strOriginalName = objImageFile.FileName;
					}
					else
					{
						strErrorMessage = "קובץ תמונה לא חוקי - ניתן להעלות תמונות בלבד";
					}
				}
				
				//check description:
				if ((blnZip == false)&&(strDescription.Length == 0))
					strErrorMessage = "יש להוסיף תיאור לתמונה";
				
				//error?
				if (strErrorMessage.Length > 0)
				{
					sb.Append(strBaseError.Replace("%error", strErrorMessage));
					if (strTempPath.Length > 0)
					{
						if (blnZip)
						{
							//Directory.Delete(strTempPath, true);
						}
						else
						{
							File.Delete(strTempPath);
						}
					}
					return;
				}
				
				//save the image(s) to disk and try to make thumbnail.
				for (int i=0; i<arrImages.Count; i++)
				{
					string strImageName="";
					string strCurPath=arrImages[i].ToString();
					SaveGalleryImage(strCurPath, ref strImageName);
					
					/*
					//picture already exists in the same group?
					WebSiteServices.ImageData[] arrGroupImages=
						websiteService.GetImagesByGroup(strGroupName, strSubGroup);
					if (arrGroupImages != null)
					{
						foreach (WebSiteServices.ImageData curGroupImage in arrGroupImages)
						{
							string strCurPictureName=curGroupImage.PictureName;
							if (strCurPictureName.ToLower() != strImageName.ToLower())
							{
								string strBaseFolderPath=Server.MapPath(Data.ImageGalleryFolder+"/%file");
								string strPath1=strBaseFolderPath.Replace("%file", strCurPictureName);
								string strPath2=strBaseFolderPath.Replace("%file", strImageName);
								if (Sport.Common.Tools.FilesEqual(strPath1, strPath2))
								{
									sb.Append(strBaseError.Replace("%error", "תמונה זו כבר קיימת בקבוצה"));
									try
									{
										System.IO.File.Delete(Server.MapPath(Data.ImageGalleryFolder+"/"+strImageName));
									}
									catch {}
									return;
								}
							}
						}
					}
					*/
					
					//exists?
					if (strGroupName.Length > 0)
					{
						object[] arrExistingGroups=websiteService.GetGalleryGroups();
						bool blnExists=false;
						foreach (object objExistingGroup in arrExistingGroups)
						{
							string[] arrCurGroup=(string[]) objExistingGroup;
							string curGroupName=arrCurGroup[0];
							if (curGroupName == groupName)
							{
								blnExists = true;
								break;
							}
						}
						if (!blnExists)
						{
							string strCurOrder=Common.Data.GetGalleryGroupsOrder(this.Server);
							if (strCurOrder.Length > 0)
							{
								Common.Tools.WriteIniValue("ImageGallery", "GroupsOrder", 
									strCurOrder+"|"+arrExistingGroups.Length, Server);
								Common.Data.RebuildGalleryGroups(this.Server);
							}
						}
					}
					
					//save to database.
					WebSiteServices.ImageData imageData=new WebSiteServices.ImageData();
					imageData.ID = -1;
					imageData.OriginalName = Path.GetFileName((blnZip)?strCurPath:strOriginalName);
					imageData.PictureName = strImageName;
					imageData.Description = strDescription;
					imageData.GroupName = strGroupName;
					imageData.SubGroup = strSubGroup;
					websiteService.UpdateGalleryImage(imageData);
				}
				
				//delete temp files:
				if (strTempPath.Length > 0)
				{
					if (blnZip)
					{
						//Directory.Delete(strTempPath, true);
					}
					else
					{
						File.Delete(strTempPath);
					}
				}
				
				//done.
				sb.Append(Sport.Common.Tools.BuildOneOrMany("תמונה", "תמונות", 
					arrImages.Count, false)+"נוספו בהצלחה לגלריית התמונות");
			} //end if got posted image
		} //end function CheckNewGalleryImage
		
		/// <summary>
		/// save the image to disk and creates thumbnail image as well.
		/// </summary>
		private void SaveGalleryImage(string strExistingPath, ref string strImageName)
		{
			//find available file name.
			int x=1;
			string strBaseName="Image_%num"+Path.GetExtension(strExistingPath);
			string strBasePath=Server.MapPath(Data.ImageGalleryFolder+"/"+strBaseName);
			string strImagePath=strBasePath.Replace("%num", x.ToString());
			while (File.Exists(strImagePath))
			{
				x += 1;
				strImagePath = strBasePath.Replace("%num", x.ToString());
			}
			strImageName = strBaseName.Replace("%num", x.ToString());
			
			//save:
			File.Copy(strExistingPath, strImagePath, true);
			//objImageFile.SaveAs(strImagePath);
		} //end function SaveGalleryImage
		
		private string GetRequestDataDef(string key1, string key2)
		{
			string result=Tools.CStrDef(Request.Form[key1], "");
			if (result.Length == 0)
			{
				result = Tools.CStrDef(Request.Form[key2], "");
			}
			return result;
		}

		private string BuildGalleryGroupsHTML(string strGroupName, string strSubGroup, 
			ref string[] arrGalleryGroups)
		{
			strGroupName = Tools.CStrDef(strGroupName, "");
			System.Text.StringBuilder sb=new System.Text.StringBuilder();
			System.Text.StringBuilder strJS=new System.Text.StringBuilder();
			strJS.Append("<script type=\"text/javascript\">");
			strJS.Append(" var _arrSubGroups=new Array();");
			sb.Append("נושא:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"+
				"<select name=\"GroupNameCombo\" id=\"GroupNameCombo\" "+
				"onchange=\"GalleryGroupChanged(this);\">");
			sb.Append(Tools.BuildOption("", "נושא חדש", (strGroupName.Length == 0)));
			WebSiteServices.WebsiteService websiteService=
				new WebSiteServices.WebsiteService();
			object[] arrGroups=websiteService.GetGalleryGroups();
			if (arrGroups != null)
			{
				arrGalleryGroups = new string[arrGroups.Length];
				for (int i=0; i<arrGroups.Length; i++)
				{
					string[] arrSubGroups=(string[]) arrGroups[i];

					string curGroup=arrSubGroups[0];
					arrGalleryGroups[i] = curGroup;
					strJS.Append(" _arrSubGroups["+i+"] = new Array();");
					for (int j=1; j<arrSubGroups.Length; j++)
					{
						string sub=arrSubGroups[j].Replace("\"", "\\\"");
						strJS.Append(" _arrSubGroups["+i+"]["+(j-1)+"] = \""+sub+"\";");
					}
					sb.Append(Tools.BuildOption(curGroup, (curGroup == strGroupName)));
				}
				ArrayList arrTemp=new ArrayList(arrGalleryGroups);
				Tools.SortGalleryGroups(arrTemp, this.Server);
				arrGalleryGroups = (string[]) arrTemp.ToArray(typeof(string));
			}
			sb.Append("</select>&nbsp;"+FastControls.InputBox("GroupNameText", "")+"<br />");
			strJS.Append("</script>");
			this.RegisterClientScriptBlock("GalleryGroupArray", strJS.ToString());
			sb.Append("קבוצה:&nbsp;&nbsp;&nbsp;"+
				"<select name=\"SubGroupCombo\" id=\"SubGroupCombo\">");
			sb.Append(Tools.BuildOption("", "קבוצה חדשה", (strSubGroup.Length == 0)));
			sb.Append("</select>&nbsp;"+FastControls.InputBox("SubGroupText", ""));
			IsfMainView.clientSide.AddOnloadCommand("SelectSubGroup(\""+
				strSubGroup.Replace("\"", "\\\"")+"\")", true, true);
			return sb.ToString();
		}
		
		private string BuildGalleryGroupsHTML(string strGroupName, string strSubGroup)
		{
			string[] dummy=null;
			string result=BuildGalleryGroupsHTML(strGroupName, strSubGroup, ref dummy);
			return result;
		}
		
		private bool VerifyNumberInRange(int num, int min, int max, 
			System.Text.StringBuilder sb, string strCaption)
		{
			if (!Tools.InRange(num, min, max))
			{
				sb.Append("<span style=\"color: red; font-weight: bold;\">ערך שגוי עבור "+
					strCaption+" (טווח ערכים: "+min+"-"+max+")</span><br />");
				return false;
			}
			return true;
		}
		#endregion
		
		#region functionaries
		private void ShowFunctionariesList()
		{
			//page caption:
			_pageCaption = "רשימת בעלי תפקידים";
			
			//clear previous data:
			IsfMainView.ClearContents();
			
			//initialize TableView component as Team table view:
			TableView objView=new TableView();
			objView.EntitiesRead += new TableView.EntitiesReadHandler(SortFunctionaryList);
			objView.EntityTypeName = Sport.Entities.Functionary.TypeName;
			objView.TableViewCaption = "רשימת בעלי תפקידים מחוזית";
			objView.NoValuesText = "אין בעלי תפקידים במחוז";
			objView.GridViewHeight = 350;
			
			//define view fields:
			objView.ViewFields += new ViewField((int) Sport.Entities.Functionary.Fields.Type, "סוג");
			objView.ViewFields += new ViewField((int) Sport.Entities.Functionary.Fields.Name, "שם");
			objView.ViewFields += new ViewField((int) Sport.Entities.Functionary.Fields.School, "בית ספר");
			objView.ViewFields += new ViewField((int) Sport.Entities.Functionary.Fields.Address, "כתובת");
			objView.ViewFields += new ViewField((int) Sport.Entities.Functionary.Fields.Phone, "טלפון");
			objView.ViewFields += new ViewField((int) Sport.Entities.Functionary.Fields.CellPhone, "סלולרי");
			objView.ViewFields += new ViewField((int) Sport.Entities.Functionary.Fields.Email, "אימייל", ViewFieldType.Email);
			
			//define desired filter: 
			int regionID=_user.Region.ID;
			EntityFilter filter=new EntityFilter(
				(int) Sport.Entities.Functionary.Fields.Region, regionID);
			objView.Filter = filter;
			
			//done.
			IsfMainView.AddContents(objView);
		}
		
		private void SortFunctionaryList(ArrayList entities)
		{
			entities.Sort(new FunctionaryComparer());
		}
		#endregion
		
		#region FAQ
		private void StartEditFAQ()
		{
			//page caption:
			_pageCaption = "פורום שאלות ותשובות";
			
			//clear previous data:
			IsfMainView.ClearContents();
			
			//initialize output string:
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			
			//iterate over form data look for something to update:
			char[] questionSplit = new char[] {'_'};
			foreach (string key in Request.Form)
			{
				//check current key
				if (key == "supervisor")
				{
					//update supervisor:
					if (Request.Form["supervisor"] != null)
					{
						int supervisorID = Tools.CIntDef(Request.Form["supervisor"], -1);
						if ((supervisorID >= 0)&&(supervisorID != Core.FAQ.Supervisor))
						{
							Sport.Entities.User user = null;
							try
							{
								user = new User(supervisorID);
							}
							catch
							{}
							if (user == null)
								AddErrorMessage("זיהוי אחראי לא מוגדר");
							else if (user.UserType != Sport.Types.UserType.Internal)
								AddErrorMessage("אחראי חייב להיות משתמש התאחדות");
							else
								Core.FAQ.Supervisor = supervisorID;
						}
					}
				}
				else if (key.StartsWith("question_"))
				{
					//update question, answer and asker.
					string[] arrTemp = key.Split(questionSplit);
					if (!Sport.Common.Tools.IsInteger(arrTemp[1]))
						continue;
					int curIndex = Int32.Parse(arrTemp[1]);
					string strQuestion = Request.Form[key];
					string strAnswer = Tools.CStrDef(Request.Form["answer_"+curIndex], "");
					string strAsker = Tools.CStrDef(Request.Form["asker_"+curIndex], "");
					FAQ.Data curData = Core.FAQ.GetSingleData(curIndex);
					string strOriginalQuestion = curData.Question;
					string strOriginalAnswer = curData.Answer;
					string strOriginalAsker = curData.Asker;
					if ((strOriginalQuestion == null)||(strOriginalQuestion.Length == 0))
					{
						AddErrorMessage("אינדקס שאלה לא חוקי: "+curIndex);
						continue;
					}
					if ((strOriginalQuestion != strQuestion)||
						(strOriginalAnswer != strAnswer)||
						(strOriginalAsker != strAsker))
					{
						Core.FAQ.UpdateData(strOriginalQuestion, 
							strQuestion, strAnswer, strAsker);
					}
				}
			}
			
			//supervisor:
			sb.Append("אחראי פורום: <select name=\"supervisor\">");
			EntityFilter filter = new EntityFilter(
				(int) Sport.Entities.User.Fields.UserType, (int) Sport.Types.UserType.Internal);
			Sport.Data.Entity[] arrUsers = Sport.Entities.User.Type.GetEntities(filter);
			int supervisor = Core.FAQ.Supervisor;
			foreach (Sport.Data.Entity entUser in arrUsers)
			{
				int curUserID = entUser.Id;
				sb.Append("<option value=\""+curUserID+"\"");
				if (curUserID == supervisor)
					sb.Append(" selected=\"selected\"");
				sb.Append(">"+entUser.Name+"</option>");
			}
			sb.Append("</select><br />");
			
			sb.Append("<u>רשימת שאלות ותשובות:</u> (כדי למחוק שאלה השאר ערך ריק בתיבת הטקסט)<br />");
			Core.FAQ.Data[] arrData = Core.FAQ.GetAllData();
			if (arrData != null)
			{
				string strBaseTextarea = "<textarea name=\"%name\" "+
					"rows=\"%rows\" cols=\"50\">%value</textarea>";
				string strQuestionTextarea = strBaseTextarea.Replace("%rows", "3");
				string strAnswerTextarea = strBaseTextarea.Replace("%rows", "6");
				ArrayList arrIndices = new ArrayList();
				ArrayList arrQuestions = new ArrayList();
				for (int i=0; i<arrData.Length; i++)
				{
					Core.FAQ.Data curData = arrData[i];
					int curIndex = curData.Index;
					string curQuestion = Tools.CStrDef(curData.Question, "");
					string curAnswer = Tools.CStrDef(curData.Answer, "");
					string curAsker = Tools.CStrDef(curData.Asker, "").Replace("\"", "&quot;");
					sb.Append("<fieldset style=\"width: 350px;\">");
					sb.Append("<legend>שאלה "+Sport.Common.Tools.IntToHebrew(i+1, true)+"</legend>");
					sb.Append("שאלה: "+strQuestionTextarea.Replace("%name", "question_"+curIndex).Replace("%value", curQuestion)+"<br />");
					sb.Append("שם השואל: <input type=\"text\" name=\"asker_"+curIndex+"\" value=\""+curAsker+"\" /><br />");
					sb.Append("תשובה: "+strAnswerTextarea.Replace("%name", "answer_"+curIndex).Replace("%value", curAnswer)+"<br />");
					sb.Append("</fieldset><br />");
					arrIndices.Add(curIndex);
					arrQuestions.Add(Tools.CStrDef(curQuestion, "", 60));
				}
				sb.Append("סדר הופעת השאלות:<br />");
				sb.Append(Tools.BuildUpDownCombo("QuestionOrder", "AllQuestions", 
					arrIndices, arrQuestions, ",", "שאלה", false)+"<br /><br />");
			}
			
			//done.
			sb.Append(Tools.BuildSubmitButton(""));
			IsfMainView.AddContents(sb.ToString());
		}
		#endregion
		#endregion

		#region view methods
		/// <summary>
		/// adds text to the view, i.e. the main panel and return the added object.
		/// </summary>
		private void AddViewText(string text, int index, bool blnPureHtml)
		{
			IsfMainView.AddContents(text);
		}

		private void AddViewText(string text, int index)
		{
			AddViewText(text, index, false);
		}

		private void AddViewText(string text, bool blnPureHtml)
		{
			AddViewText(text, -1, blnPureHtml);
		}

		private void AddViewText(string text)
		{
			AddViewText(text, -1);
		}

		/// <summary>
		/// add given text into the main view with given color and given size (pixels)
		/// </summary>
		private void AddViewText(string text, int fontSize, Color fontColor, int index)
		{
			//return null;
			
			//create new label control and apply given attributes:
			Label objLabel=new Label();
			objLabel.Text = text;

			objLabel.Attributes.Add("dir", "rtl");
			objLabel.Font.Size = FontUnit.Point(fontSize);
			objLabel.ForeColor = fontColor;
			if (SportSite.Common.Style.DefaultFontFamily.Length > 0)
				objLabel.Style.Add("font-family", SportSite.Common.Style.DefaultFontFamily);
			
			//add the label to the panel containder:
			IsfMainView.AddContents(objLabel);
		}

		private object AddViewError(string strMessage)
		{
			//object result=AddViewText(strMessage, 14, Color.Red, 0);
			errorsLabel.Text += strMessage+"<br />";
			return errorsLabel;
		}

		private void AddSuccessMessage(string strMessage)
		{
			AddViewText("<span style=\"font-weight: bold; "+
				"font-style: italic;\">"+strMessage+"</span><br />");
		}

		private void AddErrorMessage(string strMessage)
		{
			AddViewText("<span style=\"color: red; font-weight: bold; "+
				"font-style: italic;\">"+strMessage+"</span><br />");
		}

		private object AddViewSuccessMsg(string strMessage)
		{
			successLabel.Text += strMessage+"<br />";
			successLabel.Visible = true;
			return successLabel;
		}
		#endregion
		
		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			userManager = new UserManager(this.Session);
			//ClientSide.Page = this.Page;
			
			this.lbStyle = IsfMainView.lbStyle;
			this.successLabel = IsfMainView.successLabel;
			this.IsfHebDateTime = IsfMainView.IsfHebDateTime;
			this.SideNavBar = IsfMainView.SideNavBar;
			this.LeftNavBar = IsfMainView.LeftNavBar;
			this.OrdersBasket = IsfMainView.OrdersBasket;
			
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
			this.Unload += new System.EventHandler(this.Page_Unload);
			this.Load += new System.EventHandler(this.Page_Load);
			this.PreRender += new System.EventHandler(this.Page_PreRender);

		}
		#endregion
		
		#region category sport data
		private class CategorySportData
		{
			public string strNoSex="";
			public string strSport="";
			public Sport.Entities.ChampionshipCategory category=null;
		}
		#endregion
		
		#region category sport comparer
		private class CategorySportComparer : IComparer
		{
			public int Compare(object x, object y)
			{
				//valid?
				if ((x is CategorySportData)&&(y is CategorySportData))
				{
					//make the objects championship categories:
					CategorySportData cat1=(CategorySportData) x;
					CategorySportData cat2=(CategorySportData) y;
					
					//compare by category
					return cat1.strNoSex.CompareTo(cat2.strNoSex);
				}
				return 0;
			}
		}
		#endregion
		
		#region category data
		private class CategoryData
		{
			public static int IndexOfCategory(ArrayList arrData, int category)
			{
				for (int i=0; i<arrData.Count; i++)
				{
					CategoryData data=(CategoryData) arrData[i];
					if (data.category == category)
						return i;
				}
				return -1;
			}
			
			public int category=-1;
			public string strNoSex="";
		}
		#endregion
		
		#region links
		private WebSiteServices.LinkData[] BuildLinksList(
			WebSiteServices.LinkData[] links)
		{
			ArrayList result=new ArrayList();
			
			if (links != null)
			{
				for (int linkIndex=0; linkIndex<links.Length; linkIndex++)
				{
					//text
					links[linkIndex].Text = 
						Tools.CStrDef(Request.Form["LinkText_"+linkIndex], "");
					//URL
					links[linkIndex].URL = 
						Tools.CStrDef(Request.Form["LinkUrl_"+linkIndex], "");
				}
			}
			
			//build result array:
			if (links != null)
				result.AddRange(links);
			
			//new link?
			string strNewLinkText=Tools.CStrDef(Request.Form["LinkText_new"], "");
			string strNewLinkURL=Tools.CStrDef(Request.Form["LinkUrl_new"], "");
			if ((strNewLinkText.Length > 0)&&(strNewLinkURL.Length > 0))
			{
				WebSiteServices.LinkData link=new WebSiteServices.LinkData();
				link.Text = strNewLinkText;
				link.URL = strNewLinkURL;
				result.Add(link);
			}
			if (result.Count == 0)
				return null;
			return (WebSiteServices.LinkData[]) 
				result.ToArray(typeof(WebSiteServices.LinkData));
		}
		#endregion
		
		#region category comparer
		private class CategoryGradeComparer : IComparer
		{
			public int Compare(object x, object y)
			{
				//valid?
				if ((x is CategoryData)&&(y is CategoryData))
				{
					//make the objects championship categories:
					CategoryData cat1=(CategoryData) x;
					CategoryData cat2=(CategoryData) y;
					
					//compare by category
					return cat1.strNoSex.CompareTo(cat2.strNoSex);
				}
				return 0;
			}
		}
		#endregion
		
		#region team championship comparer
		private class PlayerTeamComparer : IComparer
		{
			public int Compare(object x, object y)
			{
				Sport.Entities.Player p1=(Sport.Entities.Player) x;
				Sport.Entities.Player p2=(Sport.Entities.Player) y;
				if (p1.Team.Id == p2.Team.Id)
				{
					if (p1.Team.Championship.Id == p2.Team.Championship.Id)
						return p1.Team.Category.Id.CompareTo(p2.Team.Category.Id);
					return p1.Team.Championship.Id.CompareTo(p2.Team.Championship.Id);
				}
				return p1.Team.Id.CompareTo(p2.Team.Id);
			}
		}
		#endregion
		
		#region functionary comparer
		private class FunctionaryComparer : IComparer
		{
			public int Compare(object x, object y)
			{
				//get entities
				Entity e1=(Entity) x;
				Entity e2=(Entity) y;
				
				//get functionaries
				Functionary f1=new Functionary(e1);
				Functionary f2=new Functionary(e2);
				
				//same type?
				if (f1.FunctionaryType == f2.FunctionaryType)
					return f1.Name.CompareTo(f2.Name);
				
				//sort by type:
				return f1.FunctionaryType.CompareTo(f2.FunctionaryType);
			}
		}
		#endregion
	}
}
