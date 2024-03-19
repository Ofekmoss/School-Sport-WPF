<%@ Page language="c#" Codebehind="WebForm1.aspx.cs" AutoEventWireup="false" Inherits="SportSite.WebForm1" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>WebForm1</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<script type="text/javascript" src="Common/swfobject.js"></script>
	</HEAD>
	<body MS_POSITIONING="GridLayout" bgcolor="yellow">
		<form id="Form1" method="post" runat="server">
			<asp:LinkButton OnClick="ButtonClick" ID="MyButton" Text="click me" Runat="server"></asp:LinkButton>
			<input type="file" runat="server" id="File1">
			<asp:Image ID="Image1" Runat="server"></asp:Image>
			<div id="MyFlashPanel"></div>
			<script type="text/javascript">
				var so = new SWFObject("bigPicNew.swf", "movie_1", "258", "172", "7", "#ffffff");
				so.addVariable("picUrl", "Images/default_article_image_new.JPG");
				so.write("MyFlashPanel");
			</script>
			<br /><br /><br />
			<%
				foreach (string key in Application.Keys)
					Response.Write("Application(\""+key+"\") = "+Application[key].ToString()+"<br />");
			%>
			<asp:label runat="server" id="Label2"></asp:label>
			<asp:TextBox ID="Text1" Runat="server"></asp:TextBox>
			<button type="submit">Submit</button><br /><br />
			<a href="WebForm1.aspx">click</a>
			<br />
			<asp:HyperLink ImageUrl="Images/btn_print.jpg" NavigateUrl="page2.aspx" Runat="server">Go</asp:HyperLink>
		</form>
	</body>
</HTML>
