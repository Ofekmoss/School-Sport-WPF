<%@ Page Language="C#" %>
<script language="C#" runat="server">
	public void Page_Load(object sender, EventArgs e)
	{
		Response.Status = "404 Not Found";

		bool blnRedirect;
		string strRequestedUrl = GetRequestedUrl(out blnRedirect);
		if (strRequestedUrl.EndsWith("/gameresult", StringComparison.CurrentCultureIgnoreCase))
		{
			Response.Redirect("https://94ed2e7b77ecd78cd82115b33d377a2e8f49a55e.googledrive.com/host/0B_bCrBSyqlSXTUtCZXktWktER0U/index.html", true);
			return;
		}
		if (strRequestedUrl.IndexOf("/ZooZoo", StringComparison.CurrentCultureIgnoreCase) >= 0 ||
			strRequestedUrl.IndexOf("ZooZoo.org.il", StringComparison.CurrentCultureIgnoreCase) >= 0)
		{
			string strURL = "/ZooZoo/default.aspx" + GetZooZooQueryString(strRequestedUrl);
			if (blnRedirect)
				Response.Redirect(strURL, true);
			else
				Server.Transfer(strURL, false);
			return;
		}
		
		Label1.Text = "<br />(" + Server.HtmlEncode(strRequestedUrl) + ")";
	}

	string GetZooZooQueryString(string rawURL)
	{
		string[] parts = rawURL.ToLower().Split('/');
		int index = -1;
		if (rawURL.IndexOf("ZooZoo.org.il", StringComparison.CurrentCultureIgnoreCase) >= 0)
			index = Array.IndexOf<string>(parts, Request.ServerVariables["SERVER_NAME"].ToLower());
		else
			index = Array.IndexOf<string>(parts, "zoozoo");			
		string querystring = string.Empty;
		if (index >= 0 && index < (parts.Length - 1))
		{
			string key = parts[index + 1];
			switch (key)
			{
				case "article":
				case "weeklypicture":
					int id = -1;
					if (index < (parts.Length - 2))
					{
						if (!Int32.TryParse(parts[index + 2], out id))
						{
							id = -1;
						}
					}
					querystring = "?" + key + "=" + ((id < 0) ? "all" : id.ToString());
					//Response.Write("qs: " + querystring);
					//Response.End();
					break;
				case "events":
				case "streetgames":
					string range = string.Empty;
					if (index < (parts.Length - 2))
						range = parts[index + 2];
					querystring = "?" + key + "=" + range;
					break;
				default:
					if (IsEnglishWord(key))
						querystring = "?" + key;
					break;
			}
		}
		return querystring;
	}

	bool IsEnglishWord(string str)
	{
		str = str.ToLower();
		for (int i = 0; i < str.Length; i++)
		{
			char c = str[i];
			if (c < 'a' || c > 'z')
				return false;
		}
		return true;
	}

	string GetRequestedUrl(out bool blnRedirect)
	{
		blnRedirect = false;
		if (!string.IsNullOrEmpty(Request.QueryString["aspxerrorpath"]))
		{
			string url = Request.QueryString["aspxerrorpath"];
			blnRedirect = true;
			return Request.ServerVariables["SERVER_NAME"] + url;
		}
		List<string> parts = new List<string>(Request.ServerVariables["QUERY_STRING"].Split(';'));
		parts.RemoveAt(0);
		return string.Join(";", parts.ToArray());
	}
</script>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
	<meta http-equiv="Content-Type" content="text/html; charset=windows-1255" />
    <title>404 - העמוד לא נמצא</title>
	<style type="text/css">
		body * { text-align: right; direction: rtl; }
	</style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
		שגיאה 404 - העמוד המבוקש לא קיים באתר זה
		<asp:Label ID="Label1" runat="server" />
    </div>
    </form>
</body>
</html>