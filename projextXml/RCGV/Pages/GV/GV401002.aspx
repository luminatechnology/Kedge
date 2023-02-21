<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="GV401002.aspx.cs" Inherits="Page_GV401002" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" PrimaryView="AddItemMasterView" TypeName="RCGV.GV.GVApCmInvoiceInq">
<CallbackCommands>
<px:PXDSCallbackCommand Name="ViewGVApGuiCmInvoice" Visible="False"
                        DependOnGrid="grid">
</px:PXDSCallbackCommand>
		</CallbackCommands>		
			
</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" 
		Width="100%" DataMember="AddItemMasterView" TabIndex="1300">
		<Template>
			<px:PXLayoutRule runat="server" StartRow="True" ControlSize="XM" LabelsWidth="M" StartColumn="True"></px:PXLayoutRule>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule1" StartColumn="True" ControlSize="M" LabelsWidth="SM" />
			<px:PXSelector ID="edRegistrationCD" runat="server" CommitChanges="True" DataField="RegistrationCD">
			</px:PXSelector>
			<px:PXSelector ID="edGuiCmInvoiceNbrFrom" runat="server" CommitChanges="True" DataField="GuiCmInvoiceNbrFrom">
			</px:PXSelector>
			<px:PXDateTimeEdit ID="edAPInvDateFrom" runat="server" AlreadyLocalized="False" CommitChanges="True" DataField="APInvDateFrom" DefaultLocale="">
			</px:PXDateTimeEdit>
			<px:PXSegmentMask ID="edVendorID" runat="server" CommitChanges="True" DataField="VendorID">
			</px:PXSegmentMask>
			<px:PXLayoutRule runat="server" ControlSize="M" LabelsWidth="SM" StartColumn="True">
			</px:PXLayoutRule>
			<px:PXTextEdit ID="edGovUniformNumber" runat="server" AlreadyLocalized="False" DataField="GovUniformNumber" DefaultLocale="">
			</px:PXTextEdit>
			<px:PXSelector ID="edGuiCmInvoiceNbrTo" runat="server" CommitChanges="True" DataField="GuiCmInvoiceNbrTo">
			</px:PXSelector>
			<px:PXDateTimeEdit ID="edAPInvDateTo" runat="server" AlreadyLocalized="False" CommitChanges="True" DataField="APInvDateTo" DefaultLocale="">
			</px:PXDateTimeEdit></Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXGrid ID="grid" runat="server" DataSourceID="ds" Style="z-index: 100" 
		Width="100%" Height="150px" SkinID="Details" TabIndex="300">
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
			<px:PXGridLevel DataKeyNames="GuiCmInvoiceID,GuiCmInvoiceNbr" DataMember="AddItemDetailView">
				<RowTemplate>
					<px:PXSelector ID="edGuiCmInvoiceNbr" runat="server" AllowEdit="True" DataField="GuiCmInvoiceNbr">
					</px:PXSelector>
				</RowTemplate>
				<Columns>
					<px:PXGridColumn CommitChanges="True" LinkCommand="ViewGVApGuiCmInvoice" DataField="GuiCmInvoiceNbr">
					</px:PXGridColumn>
					<px:PXGridColumn DataField="InvoiceDate" Width="90px">
					</px:PXGridColumn>
					<px:PXGridColumn DataField="SellerID" Width="140" />
					<px:PXGridColumn DataField="SellerUniformNumber">
					</px:PXGridColumn>
					<px:PXGridColumn DataField="TaxCode">
					</px:PXGridColumn>
					<px:PXGridColumn DataField="SalesAmt" TextAlign="Right" Width="100px">
					</px:PXGridColumn>
					<px:PXGridColumn DataField="TaxAmt" TextAlign="Right" Width="100px">
					</px:PXGridColumn></Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" />

	</px:PXGrid>
</asp:Content>