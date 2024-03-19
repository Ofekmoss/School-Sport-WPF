<%@ Register TagPrefix="ISF" TagName="MainView" Src="Controls/MainView.ascx" %>
<%@ Page language="c#" Codebehind="Gallery.aspx.cs" AutoEventWireup="false" Inherits="SportSite.Gallery" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" > 
<html>
  <head>
	<title>התאחדות הספורט לבתי הספר בישראל</title>
    <meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1" />
    <meta name="CODE_LANGUAGE" Content="C#" />
    <meta name=vs_defaultClientScript content="JavaScript" />
    <meta name=vs_targetSchema content="http://schemas.microsoft.com/intellisense/ie5" />
	<meta http-equiv="Content-Type" content="text/html; charset=windows-1255" />
	<meta name="Author" content="Yahav Braverman M.I.R" />
	<meta name="Description" content="גלריית תמונות של אתר התאחדות הספורט לבתי ספר" />	
	<meta name="Keywords" content="גלרייה, גלריה, תמונות" />		
  </head>
  <body MS_POSITIONING="GridLayout">
    <form id="Form1" method="post" runat="server">
		<ISF:MainView id="IsfMainView" Runat="server"></ISF:MainView>
     </form>
  </body>
</html>
