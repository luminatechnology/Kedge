<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="KG503000.aspx.cs" Inherits="Page_KG503000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="Kedge.KGValuationConfirmProcess"
        PrimaryView="ValuationConfirmFilter"
        >
		<CallbackCommands>
<px:PXDSCallbackCommand CommitChanges="True" Name="sendValuationApprovalAction" Visible="False" ></px:PXDSCallbackCommand>
<px:PXDSCallbackCommand CommitChanges="True" Name="valuationVendorConfirmAction" Visible="False" ></px:PXDSCallbackCommand>
<px:PXDSCallbackCommand CommitChanges="True" Name="unSignValuationAction" Visible="False" ></px:PXDSCallbackCommand>
<px:PXDSCallbackCommand CommitChanges="True" Name="unConfirmValuationAction" Visible="False" ></px:PXDSCallbackCommand>

<px:PXDSCallbackCommand CommitChanges="True" Name="viewValuationConfirm" Visible="False" DependOnGrid="True"></px:PXDSCallbackCommand></CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="ValuationConfirmFilter" Width="100%" Height="130px" AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartRow="True"></px:PXLayoutRule>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule7" StartColumn="True" ></px:PXLayoutRule>
			<px:PXSelector runat="server" ID="CstPXSelector1" DataField="ContractID" CommitChanges="True" ></px:PXSelector>
			<px:PXSegmentMask CommitChanges="True" runat="server" ID="CstPXSegmentMask6" DataField="VendorID" ></px:PXSegmentMask>
			<px:PXDateTimeEdit CommitChanges="True" runat="server" ID="CstPXDateTimeEdit4" DataField="ValuationDateFrom" ></px:PXDateTimeEdit>
			<px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector10" DataField="CreatedByID" ></px:PXSelector>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule8" StartColumn="True" ></px:PXLayoutRule>
			<px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector2" DataField="OrderNbr" ></px:PXSelector>
			<px:PXDropDown CommitChanges="True" runat="server" ID="CstPXDropDown3" DataField="Status" ></px:PXDropDown>
			<px:PXDateTimeEdit CommitChanges="True" runat="server" ID="CstPXDateTimeEdit5" DataField="ValuationDateTo" ></px:PXDateTimeEdit></Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXGrid PageSize="12" MatrixMode="True" SyncPosition="True" ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="Details" AllowAutoHide="false">
		<Levels>
			<px:PXGridLevel DataMember="ValuationDetails">
			    <Columns>
				<px:PXGridColumn Type="CheckBox" DataField="Selected" Width="60" CommitChanges="True" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CreatedByID" Width="220" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Status" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn LinkCommand="viewValuationConfirm" DataField="ValuationID" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="ValuationDescr" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="ValuationDate" Width="90" ></px:PXGridColumn>
				<px:PXGridColumn DataField="ContractCD" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="ContractDescr" Width="70" />
				<px:PXGridColumn DataField="OrderNbr" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="VendorID" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="VendorAcctName" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="InventoryID" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="InventoryID_description" Width="280" ></px:PXGridColumn>
				<px:PXGridColumn DataField="PricingType" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Qty" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="UnitPrice" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Amount" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="TaxAmt" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="TotalAmt" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="ManageFeeAmt" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="ManageFeeTaxAmt" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="ManageFeeTotalAmt" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="APInvoiceNbr" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="APInvoiceDate" Width="90" ></px:PXGridColumn></Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
		<ActionBar >
		
			<CustomItems>
				<px:PXToolBarButton Text="Print &amp; Sign">
					<AutoCallBack Command="sendValuationApprovalAction" Target="ds" ></AutoCallBack></px:PXToolBarButton>
				<px:PXToolBarButton Text="Vendor Confirm" >
					<AutoCallBack Target="ds" Command="valuationVendorConfirmAction" ></AutoCallBack></px:PXToolBarButton>
				<px:PXToolBarButton Text="UnSign" >
					<AutoCallBack Target="ds" Command="unSignValuationAction" ></AutoCallBack></px:PXToolBarButton>
				<px:PXToolBarButton Text="Vendor UnConfirm" >
					<AutoCallBack Command="unConfirmValuationAction" Target="ds" ></AutoCallBack></px:PXToolBarButton></CustomItems>
			<Actions>
				<AddNew ToolBarVisible="False" ></AddNew></Actions>
			<Actions>
				<Delete ToolBarVisible="False" ></Delete></Actions></ActionBar>
	</px:PXGrid>
</asp:Content>