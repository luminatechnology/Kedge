<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="GV401001.aspx.cs" Inherits="Page_GV401001" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="RCGV.GV.GVApInvoiceInq" PrimaryView="Master">
<CallbackCommands>
<px:PXDSCallbackCommand Name="ViewGVApGuiInvoice" Visible="False"
                        DependOnGrid="grid">
</px:PXDSCallbackCommand>
		</CallbackCommands>		
			
</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" 
		Width="100%" DataMember="Master" TabIndex="2900">
		<Template>
			<px:PXLayoutRule runat="server" StartRow="True" ControlSize="M" LabelsWidth="SM" StartColumn="True"/>
			<px:PXSelector ID="edInvoiceNbrFrom" runat="server" CommitChanges="True" DataField="InvoiceNbrFrom">
			</px:PXSelector>
			<px:PXDateTimeEdit ID="edDateFrom" runat="server" AlreadyLocalized="False" DataField="DateFrom" CommitChanges="True">
			</px:PXDateTimeEdit>
			<px:PXSegmentMask ID="edVendorID" runat="server" DataField="VendorID" CommitChanges="True">
			</px:PXSegmentMask>
			<px:PXLayoutRule runat="server" ControlSize="M" LabelsWidth="SM" StartColumn="True">
			</px:PXLayoutRule>
			<px:PXSelector ID="edInvoiceNbrTo" runat="server" CommitChanges="True" DataField="InvoiceNbrTo">
			</px:PXSelector>
			<px:PXDateTimeEdit ID="edDateTo" runat="server" AlreadyLocalized="False" DataField="DateTo" CommitChanges="True">
			</px:PXDateTimeEdit>
			<px:PXDropDown ID="edGuiType" runat="server" DataField="GuiType" CommitChanges="True">
			</px:PXDropDown>
		</Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXGrid ID="grid" runat="server" DataSourceID="ds" Style="z-index: 100" 
		Width="100%" Height="150px" SkinID="Inquire" AdjustPageSize="Auto" AllowPaging="True" TabIndex="900">
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
			<px:PXGridLevel DataMember="Details">
				<RowTemplate>
					<px:PXSegmentMask ID="edGuiInvoiceNbr" runat="server" AllowEdit="True" DataField="GuiInvoiceNbr">
					</px:PXSegmentMask>
				</RowTemplate>
				<Columns>
					<px:PXGridColumn CommitChanges="True" LinkCommand="ViewGVApGuiInvoice" DataField="GuiInvoiceNbr" Width="160px">
					</px:PXGridColumn>
					<px:PXGridColumn DataField="InvoiceDate" Width="90px">
					</px:PXGridColumn>
					<px:PXGridColumn DataField="DeclareYear" TextAlign="Right">
					</px:PXGridColumn>
					<px:PXGridColumn DataField="DeclareMonth" TextAlign="Right">
					</px:PXGridColumn>
					<px:PXGridColumn DataField="Vendor" Width="140" />
					<px:PXGridColumn DataField="TaxCode">
					</px:PXGridColumn>
					<px:PXGridColumn DataField="SalesAmt" TextAlign="Right" Width="100px">
					</px:PXGridColumn>
					<px:PXGridColumn DataField="TaxAmt" TextAlign="Right" Width="100px">
					</px:PXGridColumn>
					<px:PXGridColumn DataField="RegistrationCD">
					</px:PXGridColumn></Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
	</px:PXGrid>
</asp:Content>