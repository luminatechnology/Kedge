<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="GV301003.aspx.cs" Inherits="Page_GV301003" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="RCGV.GVAPInvoiceInq" PrimaryView="gvApGuiInvoices">
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" 
		Width="100%" DataMember="gvApGuiInvoiceFilters" TabIndex="1100">
		<Template>
			<px:PXLayoutRule runat="server" LabelsWidth="M" StartRow="True" ControlSize="XM" StartColumn="True">
			</px:PXLayoutRule>
			<px:PXNumberEdit ID="edGuiInvoiceNbrForm" runat="server" AlreadyLocalized="False" DataField="GuiInvoiceNbrForm" DefaultLocale="" Size="XM">
			</px:PXNumberEdit>
			<px:PXDateTimeEdit ID="edDateFrom" runat="server" AlreadyLocalized="False" DataField="DateFrom" Size="XM">
			</px:PXDateTimeEdit>
			<px:PXSelector ID="edSeller" runat="server" DataField="Seller" Size="XM">
			</px:PXSelector>
			<px:PXDropDown ID="edGuiType" runat="server" DataField="GuiType" Size="XM">
			</px:PXDropDown>
			<px:PXLayoutRule runat="server" ControlSize="XM" LabelsWidth="M" StartColumn="True">
			</px:PXLayoutRule>
			<px:PXNumberEdit ID="edGuiInvoiceNbrTo" runat="server" AlreadyLocalized="False" DataField="GuiInvoiceNbrTo" DefaultLocale="" Size="XM">
			</px:PXNumberEdit>
			<px:PXDateTimeEdit ID="edDateTo" runat="server" AlreadyLocalized="False" DataField="DateTo" Size="XM">
			</px:PXDateTimeEdit>
			<px:PXSelector ID="edSellerUniformNumber" runat="server" DataField="SellerUniformNumber" Size="XM">
			</px:PXSelector>
		</Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXGrid ID="grid" runat="server" DataSourceID="ds" Style="z-index: 100" 
		Width="100%" Height="150px" SkinID="Inquire" TabIndex="4500">
<EmptyMsg ComboAddMessage="No records found.
Try to change filter or modify parameters above to see records here." NamedComboMessage="No records found as &#39;{0}&#39;.
Try to change filter or modify parameters above to see records here." NamedComboAddMessage="No records found as &#39;{0}&#39;.
Try to change filter or modify parameters above to see records here." FilteredMessage="No records found.
Try to change filter to see records here." FilteredAddMessage="No records found.
Try to change filter to see records here." NamedFilteredMessage="No records found as &#39;{0}&#39;.
Try to change filter to see records here." NamedFilteredAddMessage="No records found as &#39;{0}&#39;.
Try to change filter to see records here." AnonFilteredMessage="No records found.
Try to change filter to see records here." AnonFilteredAddMessage="No records found.
Try to change filter to see records here."></EmptyMsg>
		<Levels>
			<px:PXGridLevel DataMember="gvApGuiInvoices">
				<Columns>
					<px:PXGridColumn DataField="GuiInvoiceNbr" Width="160px">
					</px:PXGridColumn>
					<px:PXGridColumn DataField="InvoiceDate" Width="90px">
					</px:PXGridColumn>
					<px:PXGridColumn DataField="Seller" Width="200px">
					</px:PXGridColumn>
					<px:PXGridColumn DataField="SellerUniformNumber">
					</px:PXGridColumn>
					<px:PXGridColumn DataField="GuiType">
					</px:PXGridColumn>
					<px:PXGridColumn DataField="SalesAmt" TextAlign="Right" Width="100px">
					</px:PXGridColumn>
					<px:PXGridColumn DataField="TaxAmt" TextAlign="Right" Width="100px">
					</px:PXGridColumn>
				</Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" />
		<ActionBar>
			<CustomItems>
				<px:PXToolBarButton AlreadyLocalized="False" SuppressHtmlEncoding="False" Text="AddInvoice">
					<AutoCallBack Command="AddInvoice">
					</AutoCallBack>
				</px:PXToolBarButton>
			</CustomItems>
		</ActionBar>
	</px:PXGrid>
</asp:Content>
