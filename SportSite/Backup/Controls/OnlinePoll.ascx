<%@ Register TagPrefix="ISF" TagName="ProgressBar" Src="ProgressBar.ascx" %>
<%@ Control Language="c#" AutoEventWireup="false" Codebehind="OnlinePoll.ascx.cs" Inherits="SportSite.Controls.OnlinePoll" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<asp:Panel ID="pnOnlinePollPanel" Runat="server">
	<asp:Label ID="lbPollQuestion" Runat="server"></asp:Label><br />
	<asp:Table HorizontalAlign="Center" CellPadding="0" CellSpacing="0" ID="tbPollAnswers" Runat="server"></asp:Table>
	<asp:ImageButton ID="btnPollVote" Runat="server"></asp:ImageButton>
</asp:Panel>