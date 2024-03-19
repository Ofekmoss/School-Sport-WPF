<%@ language="C#" %>
<%@ Import Namespace="System.Net" %>
<script runat="server">
void Application_Start(Object Sender, EventArgs e)
{
	Application["Hits"] = 0;
	Application["Sessions"] = 0;
	Application["TerminatedSessions"] = 0;
}

//The BeginRequest event is fired for every hit to every page in the site
void Application_BeginRequest(Object Sender, EventArgs e)
{
	Application.Lock();
	try
	{
		Application["Hits"] = (int) Application["Hits"] + 1;
	}
	catch
	{}
	Application.UnLock();
}

void Session_Start(Object Sender, EventArgs e)
{
	Application.Lock();
	try
	{
		Application["Sessions"] = (int) Application["Sessions"] + 1;
	}
	catch
	{}
	Application.UnLock();
}

void Session_End(Object Sender, EventArgs e)
{
	Application.Lock();
	try
	{
		Application["TerminatedSessions"] = (int) Application["TerminatedSessions"] + 1;
	}
	catch
	{}
	Application.UnLock();
}

void Application_End(Object Sender, EventArgs e)
{
	//Write out our statistics to a log file
	//...code omitted...
}

void Application_Error(object sender, EventArgs e)
{
	//get reference to the source of the exception chain
	Exception ex = Server.GetLastError().GetBaseException();
	
	string strPageUrl = Request.ServerVariables["Script_Name"] + "?" + Request.QueryString;
	string strErrorMessage = ex.Message;
	if (strErrorMessage.IndexOf("File does not exist", StringComparison.CurrentCultureIgnoreCase) >= 0)
	{
		strPageUrl = Request.ServerVariables["Script_Name"].Replace("/", "");
		if (strPageUrl.Equals("gameresult", StringComparison.CurrentCultureIgnoreCase))
		{
			Response.Redirect("https://94ed2e7b77ecd78cd82115b33d377a2e8f49a55e.googledrive.com/host/0B_bCrBSyqlSXTUtCZXktWktER0U/index.html", true);
		}
		return;
	}
	
	string strStackTrace = (ex.StackTrace + "").Replace("\n", "<br />");
	string strErrorSource = ex.Source;
	//string strUserIP = Request.ServerVariables["REMOTE_HOST"];
	string strSessionID = "";
	try
	{
		strSessionID = Session.SessionID;
	}
	catch
	{
	}
	string now = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
	string strLogMessage = string.Format("[{0}] [{1}] [{2}] [{3}] [{4}] [{5}]", now, strPageUrl, strErrorMessage, strStackTrace, 
		strErrorSource, strSessionID);
	
	string strLogFilePath = Server.MapPath("/ExceptionLog.txt");
	string strLogFilePath2 = Server.MapPath("/ExceptionLog_Prev.txt");
	
	
	//create if does not exist yet
	if (!System.IO.File.Exists(strLogFilePath))
		System.IO.File.WriteAllText(strLogFilePath, "");
	
	//copy to backup file if exceeded the size limit:
	if ((new System.IO.FileInfo(strLogFilePath)).Length > 102400)
	{
		try
		{
			System.IO.File.Copy(strLogFilePath, strLogFilePath2, true);
			System.IO.File.WriteAllText(strLogFilePath, "");
		}
		catch
		{
		}
	}
	
	System.IO.File.AppendAllText(strLogFilePath, strLogMessage + Environment.NewLine);
}

protected void Application_PreRequestHandlerExecute(Object sender, EventArgs e)
{
	if (HttpContext.Current.Request.Url.ToString().IndexOf("/SwimCup", StringComparison.CurrentCultureIgnoreCase) >= 0)
	{
		//Server.Transfer("http://62.219.14.227/$sitepreview/isfisrael.org/");
		//Response.Status = "301 Moved Permanently";
		//Response.AddHeader("Location", currentUrl);
		string localURL = HttpContext.Current.Request.ServerVariables["Script_Name"].Replace("/SwimCup", "");
		string strQS = Request.ServerVariables["QUERY_STRING"]  + "";
		if (strQS.Length > 0)
			localURL += "?" + strQS;
		if (localURL.EndsWith("/"))
			localURL = localURL.Substring(0, localURL.Length - 1);
		string baseURL = "http://62.219.14.227/$sitepreview/isfisrael.org/";
		string externalURL = baseURL + localURL;
		string rawHTML = "";
		using (WebClient wc = new WebClient())
		{
			wc.Encoding = UTF8Encoding.GetEncoding("utf-8");
			rawHTML = wc.DownloadString(externalURL);
		}
		rawHTML = rawHTML.Replace("type=\"text/css\" href=\"", "type=\"text/css\" href=\"" + baseURL);
		rawHTML = rawHTML.Replace("src=\"", "src=\"" + baseURL);
		Response.Clear();
		Response.Write(rawHTML);
		Response.End();
	}
}
</script>