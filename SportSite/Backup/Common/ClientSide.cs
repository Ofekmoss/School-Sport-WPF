using System;
using System.Collections;

namespace SportSite.Common
{
	public enum ModalResults
	{
		Undefined=0,
		OK=1,
		Cancel=2
	}

	/// <summary>
	/// Summary description for ClientSide.
	/// </summary>
	public class ClientSide
	{
		private System.Web.UI.Page _page=null;
		private System.Collections.ArrayList _onloadCollection=null;
		private System.Web.UI.WebControls.Literal _onloadContainer=null;
		private ArrayList _arrFlashMovies=new ArrayList();
		private readonly string NO_ERRORS="-0-";
		
		/// <summary>
		/// the page associated with the client side.
		/// </summary>
		public ClientSide(System.Web.UI.Page page, 
			System.Web.UI.WebControls.Literal literal)
		{
			_page = page;
			_onloadContainer = literal;
			
			string strCode =  "<script type=\"text/javascript\" src=\""+Common.Data.AppPath+"/SportSite.js\"></script>\n";
			strCode +=  "<script type=\"text/javascript\" src=\""+Common.Data.AppPath+"/Common/Common.js\"></script>\n";
			strCode += "<link rel=\"stylesheet\" type=\"text/css\" href=\""+Common.Data.AppPath+"/SportSite.css\" />";
			if (!_page.ClientScript.IsClientScriptBlockRegistered("global"))
				_page.ClientScript.RegisterClientScriptBlock(_page.GetType(), "global", strCode, false);
			
			AddOnloadCommand("SetDefaultFocus()", true);
			AddOnloadCommand("ApplyMaxRows()", true);
			AddOnloadCommand("AssignButtonPanels()", true);
			AddOnloadCommand("FillVerticalGap()", true);
			AddOnloadCommand("AutoFitCombos()", true);
			AddOnloadCommand("ApplyTableRowColors()", true);
		}
		
		public ArrayList RegisteredFlashMovies
		{
			get {return _arrFlashMovies;}
		}
		
		public System.Web.UI.WebControls.Literal OnloadContainer
		{
			get {return _onloadContainer;}
			set {_onloadContainer = value;}
		}
		
		/// <summary>
		/// return the string to be used for onload event.
		/// </summary>
		/// <returns></returns>
		public string OnloadString()
		{
			if ((_onloadCollection == null)||(_onloadCollection.Count == 0))
				return "";
			System.Text.StringBuilder result=new System.Text.StringBuilder();
			result.Append("<script type=\"text/javascript\">\n");
			result.Append("window.onload = function WindowLoad() \n");
			result.Append("{\n");
			for (int i=0; i<_onloadCollection.Count; i++)
			{
				string strToAdd="";
				string strCode=_onloadCollection[i].ToString();
				if (strCode.StartsWith(NO_ERRORS))
				{
					strToAdd = strCode.Substring(NO_ERRORS.Length);
				}
				else
				{
					strToAdd += "try {"+strCode+"} catch(e) {if (typeof AddError != 'undefined') "+
						"AddError('the following code generated an error: "+
						strCode.Replace("'", "\\'")+"<br />error: '+e.description+'"+
						"<br />key: onload');}";
				}
				result.Append("\t"+strToAdd+"\n");
			}
			result.Append("} \n");
			result.Append("</script>\n");
			return result.ToString();
		}
		
		public void RegisterJavascript(string key, string strCode, bool blnRawScript)
		{
			if (_page == null)
				throw new System.Exception("register client side code failed, no page defined.");
			
			if (_page.ClientScript.IsClientScriptBlockRegistered(key))
				return;

			string strToAdd="";
			if (blnRawScript)
			{
				strToAdd = strCode;
			}
			else
			{
				strToAdd  = "<script type=\"text/javascript\">";
				strToAdd  += "try {"+strCode+"} catch(e) {if (typeof AddError != 'undefined') "+
					"AddError('the following code generated an error: "+
					strCode.Replace("'", "\\'")+"<br />error: '+e.description+'"+
					"<br />key: "+key.Replace("'", "\\'")+"');}";
				strToAdd += "</script>";
			}
			_page.ClientScript.RegisterClientScriptBlock(_page.GetType(), key, strToAdd, false);
		}
		
		public void RegisterJavascript(string key, string strCode)
		{
			RegisterJavascript(key, strCode, false);
		}

		public void AddOnloadCommand(string strCommand, bool catchErrors, bool insertOnTop)
		{
			if (_onloadContainer == null)
			{
				System.Diagnostics.Debug.WriteLine("add onload command: can't add, no container");
				return;
				//throw new Exception("add onload command: can't add, no container");
			}
			if (_onloadCollection == null)
				_onloadCollection = new ArrayList();
			if (_onloadCollection.IndexOf(strCommand) >= 0)
				return;
			string strToAdd=strCommand;
			if (!catchErrors)
				strToAdd = NO_ERRORS+strToAdd;
			if (insertOnTop)
				_onloadCollection.Insert(0, strToAdd);
			else
				_onloadCollection.Add(strToAdd);
			_onloadContainer.Text = OnloadString();
		}
		
		public void AddOnloadCommand(string strCommand, bool catchErrors)
		{
			AddOnloadCommand(strCommand, catchErrors, false);
		}
		
		/// <summary>
		/// makes the given control be brighter or darker when mouse is over it by 
		/// changing its background color using javascript code. given color is the 
		/// original background color, howMuchLight can be also negative to make it darker.
		/// </summary>
		public void MakeBrighter(System.Web.UI.WebControls.WebControl control, 
			string color, int howMuchLight)
		{
			control.Attributes.Add("onmouseover", "PutMoreLight(this, '"+color+"', "+howMuchLight.ToString()+");");
			control.Attributes.Add("onmouseout", "RestoreColor(this)");
		}

		/// <summary>
		/// registers all the relevant code for adding more light when mouse
		/// over specific controls in the page.
		/// </summary>
		public void RegisterAddLight(System.Web.UI.Page page)
		{
			if (page.ClientScript.IsClientScriptBlockRegistered("AddMoreLight"))
				return;
			
			System.Text.StringBuilder builder=new System.Text.StringBuilder();
			builder.Append("<script type=\"text/javascript\" "+
				"src=\""+Data.AppPath+"/Common/AddLight.js\"></script>\n");
			
			string strCode=builder.ToString();
			page.ClientScript.RegisterClientScriptBlock(page.GetType(), "AddMoreLight", strCode, false);
		} //end function RegisterAddLight
		
		public void RegisterFlashMovie(string strContainerID, 
			string strMovieURL, int width, int height, string strFlashVars, string bgColor)
		{
			if (_page == null)
			{
				System.Diagnostics.Debug.WriteLine("Register Flash: no page.");
				return;
				//throw new Exception("Register Flash: no page.");
			}
			
			if (!_page.ClientScript.IsClientScriptBlockRegistered("SwfObject"))
			{
				string strHTML="<script type=\"text/javascript\" src=\""+Common.Data.AppPath+"/Common/swfobject.js\"></script>\n";
				_page.ClientScript.RegisterClientScriptBlock(_page.GetType(), "SwfObject", strHTML, false);
			}
			
			_arrFlashMovies.Add(strMovieURL);
			AddOnloadCommand("RegisterFlashMovie(\""+strMovieURL+"\", "+width+", "+height+", \""+strContainerID+"\", \""+strFlashVars.Replace("\"", Common.Data.FlashTitleQuote)+"\", \""+bgColor+"\")", true, true);
		} //end function RegisterAddLight
		
		public void RegisterFlashMovie(string strContainerID, 
			string strMovieURL, int width, int height, string strFlashVars)
		{
			RegisterFlashMovie(strContainerID, strMovieURL, width, height, strFlashVars, "#ffffff");
		}
		
		public void RegisterDebugArea(System.Web.UI.Page page)
		{
			System.Text.StringBuilder builder=new System.Text.StringBuilder();
			builder.Append("<script type=\"text/javascript\" language=\"javascript\">\n");
			builder.Append("var _buffer=\"\";\n");
			builder.Append("function PageKeyUp(event) {\n");
			builder.Append("	if (typeof event == \"undefined\")\n");
			builder.Append("		event = window.event; //yeah, IE.\n");
			builder.Append("	var keyCode=event.keyCode;\n");
			builder.Append("	if (typeof keyCode == \"undefined\")\n");
			builder.Append("		keyCode =  event.charCode;\n");
			builder.Append("	if (keyCode == 8) {\n");
			builder.Append("		_buffer = \"\";\n");
			builder.Append("		return false;\n");
			builder.Append("	}\n");
			builder.Append("	var myChar=String.fromCharCode(keyCode);\n");
			builder.Append("	_buffer += myChar;\n");
			builder.Append("	if (_buffer.toLowerCase() == \"debug\") {\n");
			builder.Append("		var element=document.getElementById(\"debugArea\");\n");
			builder.Append("		if (element)\n");
			builder.Append("			element.style.display = \"\";\n");
			builder.Append("	}\n");
			builder.Append("	if (_buffer.length >= \"debug\".length)\n");
			builder.Append("		_buffer = \"\";\n");
			builder.Append("}\n");
			builder.Append("document.onkeyup=PageKeyUp;\n");
			builder.Append("</script>\n");
			
			string strCode=builder.ToString();
			if (!page.ClientScript.IsClientScriptBlockRegistered("DebugArea"))
				page.ClientScript.RegisterClientScriptBlock(page.GetType(), "DebugArea", strCode, false);
		}
	}
}
