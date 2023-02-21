<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="GV402001.aspx.cs" Inherits="Page_GV402001" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" PrimaryView="gVArGuiInvoiceFilters" TypeName="RCGV.GV.GVArInvoiceInq">
		
		<CallbackCommands>
<px:PXDSCallbackCommand Name="ViewGVArGuiInvoice" Visible="False"
                        DependOnGrid="grid">
</px:PXDSCallbackCommand>
		</CallbackCommands>		
		
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100; margin-top: 0px;" 
		Width="100%" DataMember="gVArGuiInvoiceFilters" TabIndex="1100">
		<Template>
			<px:PXLayoutRule runat="server" StartRow="True" ControlSize="XM" StartColumn="True" LabelsWidth="SM"></px:PXLayoutRule>
			<px:PXSelector CommitChanges="True" runat="server" ID="edRegistrationCD" DataField="RegistrationCD" ></px:PXSelector>
			<px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector1" DataField="GuiInvoiceNbrFrom" ></px:PXSelector>
			<px:PXDateTimeEdit ID="edDateFrom" runat="server" AlreadyLocalized="False" DataField="DateFrom" Size="XM" CommitChanges="True">
			</px:PXDateTimeEdit>
			<px:PXDropDown ID="edGuiType" runat="server" DataField="GuiType" Size="XM" CommitChanges="True">
			</px:PXDropDown>
			<px:PXLayoutRule runat="server" StartColumn="True" ControlSize="XM" LabelsWidth="SM">
			</px:PXLayoutRule>
			<px:PXSelector ID="edCustomerID" runat="server" DataField="CustomerID" Size="XM" CommitChanges="True">
			</px:PXSelector>
			<px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector2" DataField="GuiInvoiceNbrTo" ></px:PXSelector>
			<px:PXDateTimeEdit ID="edDateTo" runat="server" AlreadyLocalized="False" DataField="DateTo" Size="XM" CommitChanges="True">
			</px:PXDateTimeEdit></Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXGrid ID="grid" runat="server" DataSourceID="ds" Style="z-index: 100" 
		Width="100%" Height="150px" SkinID="Inquire" TabIndex="12400">
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
			<px:PXGridLevel DataMember="gvArGuiInvoices" DataKeyNames="GuiInvoiceID">
				<Columns>
					<px:PXGridColumn DataField="GuiInvoiceNbr" Width="160px" CommitChanges="True" LinkCommand="ViewGVArGuiInvoice">
					</px:PXGridColumn>
					<px:PXGridColumn DataField="CustomerID" Width="120px">
					</px:PXGridColumn>
					<px:PXGridColumn DataField="InvoiceDate" Width="90px">
					</px:PXGridColumn>
					<px:PXGridColumn DataField="CustUniformNumber">
					</px:PXGridColumn>
					<px:PXGridColumn DataField="InvoiceType">
					</px:PXGridColumn>
					<px:PXGridColumn DataField="SalesAmt" TextAlign="Right" Width="100px">
					</px:PXGridColumn>
					<px:PXGridColumn DataField="TaxAmt" TextAlign="Right" Width="100px">
					</px:PXGridColumn>
					<px:PXGridColumn DataField="TotalAmt" TextAlign="Right" Width="100px">
					</px:PXGridColumn>
					<px:PXGridColumn DataField="GuiType">
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
