<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TabControl.ascx.cs" Inherits="SportSite.Controls.TabControl" %>
<style type="text/css">
.tab_control_table {width:100%; padding:10px;}
.tab_control_table .tab_header, .tab_control_table .tab_selected_header {cursor: pointer;}
.tab_control_table .tab_header, .tab_control_table .tab_selected_header, .ContentCell {text-align:center; border:solid 1px black; background-color:#DDEBF6;}
.ContentCell {border-top: none;}
.tab_control_table .tab_header {border-top:dashed 1px black; border-left:dashed 1px black; border-right:dashed 1px black;}
.tab_control_table .tab_selected_header {border-bottom: none;}
.tab_control_table .tab_seperator {width:1%; border-bottom: solid 1px black;}
</style>
<table id="MainTabTable" class="tab_control_table" cellpadding="0" cellspacing="0" runat="server">
<tr>
</tr>
<tr>
	<td id="ContentCell" class="ContentCell" style="padding:10px;" runat="server"></td>
</tr>
</table>
<iframe id="TabControlFrame" src="about:blank" width="0" height="0" style="display:none;"></iframe>