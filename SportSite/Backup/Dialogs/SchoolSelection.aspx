<%@ Page language="c#" Codebehind="SchoolSelection.aspx.cs" AutoEventWireup="false" Inherits="SportSite.Dialogs.SchoolSelection" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title runat="server" id="PageTitle">בחירת בית ספר</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<meta http-equiv="Content-Type" content="text/html; charset=windows-1255">
		<style type="text/css">
			.MainDiv {direction: rtl; }
		</style>
	</HEAD>
	<body>
		<div class="MainDiv">
			<form id="Form1" method="post" runat="server" onsubmit="return false;">
				<asp:label id="Label1" runat="server">מחוז: </asp:label>&nbsp;
				<asp:dropdownlist id="ddlRegions" runat="server" Width="102px" AutoPostBack="True"></asp:dropdownlist><br />
				<asp:label id="Label2" runat="server">יישוב: </asp:label>&nbsp;
				<asp:dropdownlist id="ddlCities" runat="server" Width="102px" AutoPostBack="True"></asp:dropdownlist><br />
				<asp:panel id="PnlSchools" runat="server">
<asp:label id="Label3" runat="server">סמל בית ספר: </asp:label>&nbsp; 
<asp:textbox id="txtSymbol" runat="server" Width="105px" MaxLength="7"></asp:textbox><br />
<asp:listbox id="lbSchools" runat="server" Rows="20"></asp:listbox>
			</asp:panel><br />
				<input type="button" name="BtnConfirm" disabled value="אישור" onclick="ConfirmClicked(this);">&nbsp;&nbsp;&nbsp;
				<input type="button" name="BtnCancel" value="ביטול" onclick="CancelClicked(this);">
			</form>
		</div>
	</body>
</HTML>
