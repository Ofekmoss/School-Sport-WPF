<%@ Page language="c#" Codebehind="Results.aspx.cs" AutoEventWireup="false" Inherits="SportSite.Results" %>
<%@ Register TagPrefix="ISF" TagName="MainView" Src="Controls/MainView.ascx" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" > 
<html>
  <head>
	<meta http-equiv="Content-Type" content="text/html; charset=windows-1255" />
	<meta name="Description" content="נתוני ליגות ואליפויות של אתר התאחדות הספורט של בתי הספר" />	
	<meta name="Keywords" content="תוצאות משחקים, לוחות משחקים, טבלאות דירוג" />
	<link rel="icon" href="favicon.ico" type="image/x-icon" />
	<link rel="shortcut icon" href="favicon.ico" />
	<title>התאחדות הספורט לבתי הספר בישראל - משחקים ותוצאות</title>
	<style type="text/css" media="screen">
		.printable {display: none;}
	</style>		
	<style type="text/css" media="print">
		.printable {display: block;}
	</style>	
  </head>
  <body MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
			<ISF:MainView id="IsfMainView" Runat="server"></ISF:MainView>
		</form>
  </body>
</html>
