<%@ Page language="c#" Codebehind="Article.aspx.cs" AutoEventWireup="false" Inherits="SportSite.Article" %>
<%@ Register Src="~/Controls/TabControl.ascx" TagPrefix="uc1" TagName="TabControl" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" > 

<html>
  <head>
    <title>Article</title>
    <meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
    <meta name="CODE_LANGUAGE" Content="C#">
    <meta name=vs_defaultClientScript content="JavaScript">
    <meta name=vs_targetSchema content="http://schemas.microsoft.com/intellisense/ie5">
  </head>
  <body MS_POSITIONING="GridLayout">
    <form id="Form1" method="post" runat="server">
		<uc1:TabControl ID="MyTabControl" TabCaptions="First Tab,Second Tab,Third Tab" runat="server" />
    </form>
  </body>
</html>
