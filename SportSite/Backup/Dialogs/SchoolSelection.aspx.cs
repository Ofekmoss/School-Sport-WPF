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
using Sport.Data;

namespace SportSite.Dialogs
{
	/// <summary>
	/// this page allows the user to select one school.
	/// uses javascript.
	/// </summary>
	public class SchoolSelection : System.Web.UI.Page
	{
		public Common.ClientSide clientSide=null;
		protected System.Web.UI.WebControls.Label Label1;
		protected System.Web.UI.WebControls.DropDownList ddlCities;
		protected System.Web.UI.WebControls.Label Label2;
		protected System.Web.UI.WebControls.Panel PnlSchools;
		protected System.Web.UI.WebControls.ListBox lbSchools;
		protected System.Web.UI.WebControls.Label Label3;
		protected System.Web.UI.WebControls.TextBox txtSymbol;
		protected System.Web.UI.WebControls.DropDownList ddlRegions;
		protected System.Web.UI.HtmlControls.HtmlForm Form1;
		protected System.Web.UI.HtmlControls.HtmlGenericControl PageTitle;
		
		private void Page_Load(object sender, System.EventArgs e)
		{
			clientSide = new Common.ClientSide(this.Page, null);
			
			//add hitlog data:
			Common.Tools.AddHitLog(WebSiteServices.WebSitePage.SchoolSelectionDialog, 
				this.Request);
			
			int i; //loop iterator
			
			//add general code:
			clientSide.RegisterAddLight(this);

			ddlRegions.Attributes.Add("onclick", "innerAction=true;");
			Sport.Core.Session.Cookies = (System.Net.CookieContainer) Session["cookies"];

			//define string builder to hold client side code:
			System.Text.StringBuilder builder=new System.Text.StringBuilder();
			builder.Append("<script type=\"text/javascript\" language=\"javascript\">\n");
			builder.Append("function FindSchool(event, objSymbol) {\n");
			builder.Append("   var symbol = objSymbol.value;\n");
			builder.Append("   var objCombo=objSymbol.form.elements[\""+lbSchools.UniqueID+"\"];\n");
			builder.Append("   if (event.keyCode == 40) {\n");
			builder.Append("      if (objCombo.selectedIndex < (objCombo.options.length-1)) {\n");
			builder.Append("         objCombo.selectedIndex = objCombo.selectedIndex+1;\n");
			builder.Append("         objCombo.onchange();\n");
			builder.Append("      }\n");
			builder.Append("      return false;\n");
			builder.Append("   }\n");
			builder.Append("   if (event.keyCode == 38) {\n");
			builder.Append("      if (objCombo.selectedIndex > 0) {\n");
			builder.Append("         objCombo.selectedIndex = objCombo.selectedIndex-1;\n");
			builder.Append("         objCombo.onchange();\n");
			builder.Append("      }\n");
			builder.Append("      return false;\n");
			builder.Append("   }\n");
			builder.Append("   if (symbol.length == 0) {\n");
			builder.Append("      objCombo.selectedIndex = -1;\n");
			builder.Append("      return true;\n");
			builder.Append("   }\n");
			builder.Append("   for (var i=0; i<objCombo.options.length; i++) {\n");
			builder.Append("      if (objCombo.options[i].text.substr(0, symbol.length) == symbol) {\n");
			builder.Append("         objCombo.selectedIndex = i;\n");
			builder.Append("         return true;\n");
			builder.Append("      }\n");
			builder.Append("   }\n");
			builder.Append("   objCombo.selectedIndex = -1;\n");
			builder.Append("   return true;\n");
			builder.Append("}\n");
			builder.Append("</script>\n");
			builder.Append("\n");
			Page.ClientScript.RegisterClientScriptBlock(Page.GetType(), "FindSchool", builder.ToString(), false);
			
			builder=new System.Text.StringBuilder();
			builder.Append("<script type=\"text/javascript\" language=\"javascript\">\n");
			builder.Append("var ActiveControl=0;\n");
			builder.Append("window.onload=function WindowLoad(event) {\n");
			builder.Append("   if (document.forms[0].elements['txtSymbol']) {\n");
			builder.Append("       document.forms[0].elements['txtSymbol'].focus();\n");
			builder.Append("       ActiveControl = document.forms[0].elements['"+txtSymbol.UniqueID+"'];");
			builder.Append("   }\n");
			builder.Append("   else {\n");
			builder.Append("      ActiveControl = document.forms[0].elements['"+ddlRegions.UniqueID+"'];\n");
			builder.Append("   }");
			builder.Append("   var totalWidth=100;\n");
			//builder.Append("   totalWidth += GetClientWidth(document.forms[0].elements[\""+ddlRegions.UniqueID+"\"]);\n");
			builder.Append("   if (document.forms[0].elements[\""+lbSchools.UniqueID+"\"])\n");
			builder.Append("      totalWidth += GetClientWidth(document.forms[0].elements[\""+lbSchools.UniqueID+"\"]);\n");
			builder.Append("   else");
			builder.Append("      totalWidth += GetClientWidth(document.forms[0].elements[\""+ddlRegions.UniqueID+"\"]);\n");
			builder.Append("   var totalHeight=130; \n");
			builder.Append("   totalHeight += GetClientHeight(document.forms[0].elements[\""+ddlRegions.UniqueID+"\"]);\n");
			builder.Append("   totalHeight += GetClientHeight(document.forms[0].elements[\""+ddlCities.UniqueID+"\"]);\n");
			builder.Append("   totalHeight += GetClientHeight(document.forms[0].elements[\""+txtSymbol.UniqueID+"\"]);\n");
			builder.Append("   totalHeight += GetClientHeight(document.forms[0].elements[\""+lbSchools.UniqueID+"\"]);\n");
			builder.Append("   if (document.forms[0].elements[\""+lbSchools.UniqueID+"\"])\n");
			builder.Append("      totalHeight += 30;\n");
			builder.Append("   window.resizeTo(totalWidth, totalHeight);\n");
			builder.Append("   var windowLeft=window.screenLeft;\n");
			builder.Append("   if (typeof windowLeft == \"undefined\")\n");
			builder.Append("      windowLeft = window.screenX;\n");
			builder.Append("   var windowTop=window.screenTop;\n");
			builder.Append("   if (typeof windowTop == \"undefined\")\n");
			builder.Append("      windowTop = window.screenY;\n");
			builder.Append("   if ((totalHeight+windowTop) >= (screen.height))\n");
			builder.Append("      window.moveBy(0, screen.height-(totalHeight+windowTop));\n");
			//builder.Append("   var objImage=document.images[\"LogoImage\"];\n");
			//builder.Append("   objImage.src = \""+SportSite.Common.Data.AppPath+"/Images/logo_big_light.gif\";\n");
			//builder.Append("   objImage.width = document.body.scrollWidth;\n");
			//builder.Append("   objImage.height = document.body.scrollHeight;\n");
			//builder.Append("   objImage.style.display = \"inline\";\n");
			//builder.Append("   objImage.style.zIndex = 99;\n");
			builder.Append("}\n");
			builder.Append("\n");
			builder.Append("function GetClientHeight(element) {\n");
			builder.Append("   return (element)?element.clientHeight:0;\n");
			builder.Append("}\n");
			builder.Append("\n");
			builder.Append("function GetClientWidth(element) {\n");
			builder.Append("   return (element)?element.clientWidth:0;\n");
			builder.Append("}\n");
			builder.Append("function GetSelectedText(objCombo) {\n");
			builder.Append("   return objCombo.options[objCombo.selectedIndex].text;\n");
			builder.Append("}\n");
			builder.Append("</script>\n");
			builder.Append("\n");
			Page.ClientScript.RegisterClientScriptBlock(Page.GetType(), "OnLoad", builder.ToString(), false);

			builder=new System.Text.StringBuilder();
			builder.Append("<script type=\"text/javascript\" language=\"javascript\">\n");
			builder.Append("var ModalResult="+((int) SportSite.Common.ModalResults.Undefined).ToString()+";\n");
			builder.Append("var SelectedSchoolName='';\n");
			builder.Append("var SelectedSchoolID=-1;\n");
			builder.Append("function ConfirmClicked(sender) {\n");
			builder.Append("   sender.disabled = true;\n");
			builder.Append("   sender.form.elements[\"BtnCancel\"].disabled = true;\n");
			builder.Append("   var objCombo=sender.form.elements[\""+lbSchools.UniqueID+"\"];\n");
			builder.Append("   var strText=GetSelectedText(objCombo);\n");
			builder.Append("   var arrTmp=strText.split(' ');\n");
			builder.Append("   SelectedSchoolName = PartialJoin(arrTmp, 1, arrTmp.length-1, ' '); \n");
			builder.Append("   SelectedSchoolID = objCombo.value; \n");
			builder.Append("   ModalResult = "+((int) SportSite.Common.ModalResults.OK).ToString()+";\n");
			builder.Append("}\n");
			builder.Append("function CancelClicked(sender) {\n");
			builder.Append("   sender.disabled = true;\n");
			builder.Append("   sender.form.elements[\"BtnConfirm\"].disabled = true;\n");
			builder.Append("   ModalResult = "+((int) SportSite.Common.ModalResults.Cancel).ToString()+";\n");
			builder.Append("}\n");
			builder.Append("function SchoolChanged(event, sender) {\n");
			builder.Append("   sender.form.elements['"+txtSymbol.UniqueID+"'].value = sender.options[sender.selectedIndex].text.split(' ')[0];\n");
			builder.Append("   sender.form.elements[\"BtnConfirm\"].disabled = false;\n");
			builder.Append("\n");
			builder.Append("}\n");
			builder.Append("function PartialJoin(arr, startIndex, endIndex, delimeter) {\n");
			builder.Append("   var result='';\n");
			builder.Append("   for (var i=startIndex; i<=endIndex; i++) {\n");
			builder.Append("      result += (arr[i]+'');\n");
			builder.Append("      if (i < endIndex)\n");
			builder.Append("         result += delimeter;\n");
			builder.Append("   }\n");
			builder.Append("   return result;\n");
			builder.Append("}\n");
			builder.Append("</script>\n");
			builder.Append("\n");
			Page.ClientScript.RegisterClientScriptBlock(Page.GetType(), "General", builder.ToString(), false);
			
			txtSymbol.Attributes.Add("onkeydown", "FindSchool(event, this);");
			lbSchools.Attributes.Add("onchange", "SchoolChanged(event, this);");
			
			ddlRegions.SelectedIndexChanged += new EventHandler(RegionChanged);
			ddlCities.SelectedIndexChanged += new EventHandler(CityChanged);

			if (!Page.IsPostBack)
			{
				//get list of all regions:
				EntityType regionType=Sport.Entities.Region.Type;
				Entity[] regions=regionType.GetEntities(null);
			
				//fill regions list:
				ddlRegions.Items.Add(new ListItem("   <בחר מחוז>   ", ""));
				for (i=0; i<regions.Length; i++)
				{
					ddlRegions.Items.Add(new ListItem(regions[i].Name, regions[i].Id.ToString()));
				}
				SportSite.Common.Tools.SortByText(ddlRegions);
				ddlCities.Items.Add(new ListItem("   <בחר יישוב>   ", ""));
			}
			
			if ((Request.QueryString["region"] != null)&&(!Page.IsPostBack))
			{
				for (i=0; i<ddlRegions.Items.Count; i++)
				{
					if (ddlRegions.Items[i].Value == Request.QueryString["region"])
					{
						ddlRegions.SelectedIndex = i;
						RegionChanged(sender, System.EventArgs.Empty);
						break;
					}
				}
			}
			
			//MainPanel.Controls.Add(SportSite.Common.FastControls.TextLabel(selectedCity.ToString()+"<br />"));
		}

		private void Page_Unload(object sender, System.EventArgs e)
		{
			Sport.Core.Session.Cookies = null;
		}

		private void Page_PreRender(object sender, System.EventArgs e)
		{
			if (ddlRegions.SelectedIndex == 0)
			{
				ddlCities.Items.Clear();
				Label2.Visible = false;
				ddlCities.Visible = false;
			}
			else
			{
				Label2.Visible = true;
				ddlCities.Visible = true;
			}
			
			if ((ddlCities.Items.Count < 2)||(lbSchools.Items.Count == 0))
			{
				lbSchools.Items.Clear();
				PnlSchools.Visible = false;
			}
			else
			{
				PnlSchools.Visible = true;
			}
			if ((lbSchools.Items.Count > 0)&&(lbSchools.Items.Count < 20))
			{
				lbSchools.Rows = lbSchools.Items.Count;
				if (lbSchools.Rows < 2)
					lbSchools.Rows = 2;
			}
			else
			{
				lbSchools.Rows = 20;
			}
		}
		
		private void RegionChanged(object sender, System.EventArgs args)
		{
			string title="בחירת בית ספר";
			if (ddlRegions.SelectedIndex > 0)
			{
				//get selected region:
				int region=Int32.Parse(ddlRegions.SelectedValue);
				int i;
				
				//get list of cities:
				EntityType cityType=Sport.Entities.City.Type;
				Entity[] cities=cityType.GetEntities(new EntityFilter((int) Sport.Entities.City.Fields.Region, region));
				
				//fill cities list:
				ddlCities.Items.Clear();
				ddlCities.Items.Add(new ListItem("   <כל היישובים>   ", ""));
				for (i=0; i<cities.Length; i++)
				{
					ddlCities.Items.Add(new ListItem(cities[i].Name, cities[i].Id.ToString()));
				}
				
				SportSite.Common.Tools.SortByText(ddlCities);
				title += " - "+ddlRegions.SelectedItem.Text;
				
				for (i=0; i<ddlCities.Items.Count; i++)
				{
					if (ddlCities.Items[i].Value == Request.QueryString["city"])
					{
						ddlCities.SelectedIndex = i;
						break;
					}
				}
				CityChanged(sender, args);
			}
			else
			{
				ddlCities.Items.Clear();
				ddlCities.Items.Add(new ListItem("   <בחר יישוב>   ", ""));
			}
			PageTitle.InnerText = title;
		}

		private void CityChanged(object sender, System.EventArgs args)
		{
			EntityType schoolType=Sport.Entities.School.Type;
			Entity[] schools=null;
			int i;
			
			if (ddlCities.SelectedIndex > 0)
			{
				//get selected city:
				int city=Int32.Parse(ddlCities.SelectedValue);
				
				//get list of schools:
				schools=schoolType.GetEntities(new EntityFilter((int) Sport.Entities.School.Fields.City, city));
			}
			else
			{
				//get selected region:
				int region=Int32.Parse(ddlRegions.SelectedValue);
				
				//get list of schools:
				schools=schoolType.GetEntities(new EntityFilter((int) Sport.Entities.School.Fields.Region, region));
			}
			
			//fill schools list:
			lbSchools.Items.Clear();
			for (i=0; i<schools.Length; i++)
			{
				lbSchools.Items.Add(new ListItem(schools[i].Fields[(int) Sport.Entities.School.Fields.Symbol].ToString()+"   "+schools[i].Name, schools[i].Id.ToString()));
			}
			
			SportSite.Common.Tools.SortByText(lbSchools);
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
			this.PreRender += new System.EventHandler(this.Page_PreRender);
			this.Unload += new System.EventHandler(this.Page_Unload);
		}
		#endregion

		private void ListBox1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
		
		}
	}
}
