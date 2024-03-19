<%@ Control Language="c#" AutoEventWireup="false" Codebehind="Article.ascx.cs" Inherits="SportSite.Controls.Article" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<table class="ArticleMainTable" cellspacing="0" cellpadding="0" id="ArticleMainTable" runat="server">
	<tr>
		<td align="right" style="width: 148px;">
			<asp:Literal ID="ArticleFlashPanel" Runat="server"></asp:Literal>
		</td>
		<td style="width: 20px;">&nbsp;</td>
		<td dir="rtl" align="right" style="vertical-align: top; width: 376px;">
			<p style="margin-top: 10px;">
				<a id="ArticleCaptionLink" class="ArticleCaptionLink" runat="server"></a>
			</p>
			<asp:Label ID="ArticleContents" CssClass="ArticleContents" Runat="server"></asp:Label>
		</td>
	</tr>
</table>