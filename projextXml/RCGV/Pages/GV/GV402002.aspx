<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="GV402002.aspx.cs" Inherits="Page_GV402002" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" PrimaryView="gVArGuiCmInvoiceFilters" TypeName="RCGV.GV.GVArGuiCmInvoiceInq">
	<CallbackCommands>
<px:PXDSCallbackCommand Name="ViewGVArGuiCmInvoice" Visible="False"
                        DependOnGrid="grid">
</px:PXDSCallbackCommand>
</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" 
		Width="100%" DataMember="gVArGuiCmInvoiceFilters" TabIndex="1100">
		<Template>
			<px:PXLayoutRule runat="server" ControlSize="XM" StartColumn="True" StartRow="True" LabelsWidth="SM">
			</px:PXLayoutRule>
			<px:PXSelector ID="edRegistrationCD" runat="server" DataField="RegistrationCD" CommitChanges="True" Size="XM">
			</px:PXSelector>
			<px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector1" DataField="GuiCmInvoiceNbrFrom" ></px:PXSelector>
			<px:PXDateTimeEdit ID="edCMInvoiceDateFrom" runat="server" AlreadyLocalized="False" DataField="CMInvoiceDateFrom" CommitChanges="True" DefaultLocale="" Size="XM">
			</px:PXDateTimeEdit>
			<px:PXLayoutRule runat="server" ControlSize="XM" StartColumn="True" LabelsWidth="SM">
			</px:PXLayoutRule>
			<px:PXSegmentMask ID="edCustomerID" runat="server" CommitChanges="True" DataField="CustomerID" Size="XM">
			</px:PXSegmentMask>
			<px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector2" DataField="GuiCmInvoiceNbrTo" ></px:PXSelector>
			<px:PXDateTimeEdit ID="edCMInvoiceDateTo" runat="server" AlreadyLocalized="False" DataField="CMInvoiceDateTo" CommitChanges="True" DefaultLocale="" Size="XM">
			</px:PXDateTimeEdit></Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXGrid ID="grid" runat="server" DataSourceID="ds" Style="z-index: 100" 
		Width="100%" Height="150px" SkinID="Details" TabIndex="1300">
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
			<px:PXGridLevel DataMember="gvArGuiCmInvoices">
				<Columns>
					<px:PXGridColumn DataField="GuiCmInvoiceNbr" Width="160px" LinkCommand="ViewGVArGuiCmInvoice">
					</px:PXGridColumn>
					<px:PXGridColumn DataField="InvoiceDate" Width="90px">
					</px:PXGridColumn>
					<px:PXGridColumn DataField="Customer" Width="200px">
					</px:PXGridColumn>
					<px:PXGridColumn DataField="TaxCode">
					</px:PXGridColumn>
					<px:PXGridColumn DataField="SalesAmt" TextAlign="Right" Width="100px">
					</px:PXGridColumn>
					<px:PXGridColumn DataField="TaxAmt" TextAlign="Right" Width="100px">
					</px:PXGridColumn>
				</Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
		<ActionBar>
			<Actions>
				<AddNew Enabled="False" ></AddNew>
				<Delete Enabled="False" ></Delete>
			</Actions>
			<CustomItems></CustomItems>
		</ActionBar>
	</px:PXGrid>
</asp:Content>
