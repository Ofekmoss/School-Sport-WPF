<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PagedTable.ascx.cs" Inherits="SportSite.NewRegister.PagedTable" %>
<div class="panel panel-default table-view">
    <div class="panel-body paged-table-container">
		<input type="hidden" class="page-size" id="hidPageSize" runat="server" />
		<input type="hidden" class="singular-caption" id="hidSingularCaption" runat="server" />
		<input type="hidden" class="plural-caption" id="hidPluralCaption" runat="server" />
		<input type="hidden" class="caption-gender" id="hidCaptionGender" runat="server" />
		<input type="hidden" class="additional-buttons" id="hidAdditionalButtons" runat="server" />
        <div class="toolbar form-inline">
			<div class="pull-left" runat="server" id="pnlAddNew" visible="false">
				<button type="button" class="btn btn-success add-new" runat="server" id="btnAddNew">
					<span class="glyphicon glyphicon-plus"></span> <span id="lbAddNew" runat="server"></span>
				</button>
			</div>
			<div class="pull-left additional-buttons-placeholder" style="margin-left: 20px; display: none;">
			</div>
			<span style="font-size: 120%; margin-left: 5px;" class="label label-info records-count"></span>
            <input type="text" class="form-control search-term" placeholder="חיפוש" />
        </div>
        <table class="table table-striped table-bordered paged-table">
			<asp:Repeater id="rptTableCaptions" OnItemDataBound="TableCaptions_ItemDataBound" runat="server">
				<HeaderTemplate><tr></HeaderTemplate>
				<ItemTemplate>
					<th data-name="<%# DataBinder.Eval(Container.DataItem, "Name") %>">
						<asp:Label ID="lbCaption" Text='<%# DataBinder.Eval(Container.DataItem, "Caption") %>' Runat="server"/>
						<span class="pull-left descending-order" id="lbDescendingOrderIcon" runat="server" style="display: none;">▼</span>
						<span class="pull-left ascending-order" id="lbAscendingOrderIcon" runat="server" style="display: none;">▲</span>
					</th>
				</ItemTemplate>
				<FooterTemplate></tr></FooterTemplate>
			</asp:Repeater>
			<asp:Repeater id="rptTableRows" OnItemDataBound="TableRows_ItemDataBound" runat="server">
				<HeaderTemplate></HeaderTemplate>
				<ItemTemplate>
				<tr style="<%# DataBinder.Eval(Container.DataItem, "Style") %>" data-target-url="<%# DataBinder.Eval(Container.DataItem, "TargetUrl") %>">
					<asp:Repeater id="rptRowCells" OnItemDataBound="TableRowCells_ItemDataBound" runat="server">
						<HeaderTemplate></HeaderTemplate>
						<ItemTemplate>
							<td data-value="<%# DataBinder.Eval(Container.DataItem, "Value") %>">
								<%# DataBinder.Eval(Container.DataItem, "Contents") %>
							</td>
						</ItemTemplate>
						<FooterTemplate></FooterTemplate>
					</asp:Repeater>
				</tr>
				</ItemTemplate>
				<FooterTemplate></FooterTemplate>
			</asp:Repeater>
        </table>
		<div class="pull-right pages-placeholder">
			<button type="button" class="btn btn-info prev-page" data-page-jump="-1" disabled>&lt;</button>
			<button type="button" style="margin-left: 5px;" class="btn btn-default page" data-page-jump="0">1</button>
			<button type="button" style="margin-left: 5px;" class="btn btn-default page">2</button>
			<button type="button" style="margin-left: 5px;" class="btn btn-default page">3</button>
			<button type="button" style="margin-left: 5px;" class="btn btn-default page">4</button>
			<button type="button" style="margin-left: 5px;" class="btn btn-default page">5</button>
			<button type="button" class="btn btn-info next-page" disabled data-page-jump="1">&gt;</button>
		</div>
		<div class="pull-left page-size-placeholder" style="direction: ltr;">
			<button type="button" style="margin-left: 5px;" class="btn btn-default page-size">5</button>
			<button type="button" style="margin-left: 5px;" class="btn btn-default page-size">10</button>
			<button type="button" style="margin-left: 5px;" class="btn btn-default page-size">20</button>
			<button type="button" style="margin-left: 5px;" class="btn btn-default page-size">50</button>
			<button type="button" style="margin-left: 5px;" class="btn btn-default page-size">100</button>
		</div>
    </div>
</div>
