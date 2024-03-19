<%@ Page Language="C#" AutoEventWireup="true" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Cache Store</title>
</head>
<body>
<script language="C#" runat="server">
	void Page_Load(object sender, System.EventArgs e)
	{
		if (Request.QueryString["key"] != null)
		{
			pnlContents.InnerHtml = GetKeyValue(Request.QueryString["key"]);
		}
		else
		{
			pnlContents.InnerHtml = SportSite.CacheStore.Instance.ToString();
		}
	}

	string GetKeyValue(string key)
	{
		object data;
		if (SportSite.CacheStore.Instance.Get(key, out data))
		{
			string value = "";
			data.GetType().GetProperties().ToList().ForEach(p =>
			{
				object propVal = p.GetValue(data, null);
				value += p.Name + " = " + propVal;
				if (propVal != null && propVal.GetType().IsArray)
				{
					value += " (" + ((System.Array)propVal).Length + ")";
				}
				value += "<br />";
			});
			
			/*
			if (data is SportSite.WebSiteServices.ArticleData)
			{
				SportSite.WebSiteServices.ArticleData article = (SportSite.WebSiteServices.ArticleData)data;
			}
			else
			{
				value = data.ToString();
			}
			*/
			
			return value;
		}
		return "";
	}
</script>
    <form id="form1" runat="server">
    <div id="pnlContents" runat="server">
    
    </div>
    </form>
</body>
</html>
