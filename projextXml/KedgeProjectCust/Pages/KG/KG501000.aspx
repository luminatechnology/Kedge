<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="KG501000.aspx.cs" Inherits="Page_KG501000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/ListView.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width=""
        TypeName="Kedge.KGRenterConfirmProcess"
        PrimaryView="RenterFilter"
        >
		<CallbackCommands>
<px:PXDSCallbackCommand CommitChanges="True" Name="sendApprovalAction" Visible="False" ></px:PXDSCallbackCommand>
<px:PXDSCallbackCommand CommitChanges="True" Name="vendorConfirmAction" Visible="False" ></px:PXDSCallbackCommand>
<px:PXDSCallbackCommand CommitChanges="True" Name="unSignAction" Visible="False" ></px:PXDSCallbackCommand>
<px:PXDSCallbackCommand CommitChanges="True" Name="unConfirmAction" Visible="False" ></px:PXDSCallbackCommand>
		</CallbackCommands>
	</px:PXDataSource>
	<px:PXFormView Width="100%" DataMember="RenterFilter" SkinID="" runat="server" ID="CstFormView1" >
		<Template>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule2" StartColumn="True" ></px:PXLayoutRule>
			<px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector4" DataField="ContractID" ></px:PXSelector>
			<px:PXSegmentMask CommitChanges="True" runat="server" ID="CstPXSegmentMask12" DataField="VendorID" ></px:PXSegmentMask>
			<px:PXDateTimeEdit CommitChanges="True" runat="server" ID="CstPXDateTimeEdit7" DataField="WorkDateFrom" ></px:PXDateTimeEdit>
			<px:PXSelector CommitChanges="" runat="server" ID="CstPXSelector15" DataField="CreatedByID" ></px:PXSelector>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule3" StartColumn="True" ></px:PXLayoutRule>
			<px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector11" DataField="OrderNbr" ></px:PXSelector>
			<px:PXDropDown CommitChanges="True" runat="server" ID="CstPXDropDown10" DataField="Status" ></px:PXDropDown>
			<px:PXDateTimeEdit CommitChanges="True" runat="server" ID="CstPXDateTimeEdit8" DataField="WorkDateTo" ></px:PXDateTimeEdit></Template>
		</px:PXFormView></asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phL" runat="Server">
	<px:PXGrid PageSize="12" MatrixMode="True" SyncPosition="True" ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="Details" AllowAutoHide="false">
		<Levels>
			<px:PXGridLevel DataMember="RenterVendors">
			    <Columns>
				<px:PXGridColumn CommitChanges="True" DataField="Selected" Width="60" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CreatedByID_Creator_Username" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Status" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn LinkCommand="ViewRenter" DataField="DailyRenterCD" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="WorkDate" Width="90" ></px:PXGridColumn>
				<px:PXGridColumn DataField="ContractID" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="OrderNbr" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="VendorID" Width="220" ></px:PXGridColumn>
				<px:PXGridColumn DataField="InsteadQty" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn CommitChanges="True" DataField="ConfirmQty" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Amount" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="WorkContent" Width="70" ></px:PXGridColumn></Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
		<ActionBar >
		
			<CustomItems>
						<px:PXToolBarButton Text="Print &amp; Sign">
							<AutoCallBack Command="sendApprovalAction" Target="ds" ></AutoCallBack></px:PXToolBarButton>
				<px:PXToolBarButton Text="VendorConfirm">
					<AutoCallBack Command="vendorConfirmAction" Target="ds" ></AutoCallBack></px:PXToolBarButton>
						<px:PXToolBarButton Text="UnSign" >
							<AutoCallBack Target="ds" ></AutoCallBack>
							<AutoCallBack Command="unSignAction" ></AutoCallBack></px:PXToolBarButton>
						<px:PXToolBarButton Text="VendorUnConfirm">
							<AutoCallBack Target="ds" Command="unConfirmAction" ></AutoCallBack></px:PXToolBarButton></CustomItems>
			<Actions>
				<AddNew Enabled="False" ></AddNew></Actions>
			<Actions>
				<Delete Enabled="False" ></Delete></Actions></ActionBar>
	</px:PXGrid>
</asp:Content>