<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="KG506000.aspx.cs" Inherits="Page_KG506000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/ListView.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="Kedge.KGRequisitionsUploadTempEntry"
        PrimaryView="DetailsView"
        >
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phL" runat="Server">
	<px:PXGrid ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="Primary" AllowAutoHide="false">
		<Levels>
			<px:PXGridLevel DataMember="DetailsView">
			    <Columns>
				<px:PXGridColumn Type="CheckBox" AllowCheckAll="True" DataField="IsActive" Width="60" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Type" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="RefNbr" Width="120" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Usercd" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="ProjectID" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Date" Width="90" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Description" Width="280" ></px:PXGridColumn>
				<px:PXGridColumn DataField="TaskID" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CostCodeID" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="InventoryID" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="InventoryDesc" Width="280" ></px:PXGridColumn>
				<px:PXGridColumn DataField="AccountGroupID" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Uom" Width="72" ></px:PXGridColumn>
				<px:PXGridColumn CommitChanges="True" DataField="OrderQty" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn CommitChanges="True" DataField="UnitCost" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="SubTotalCost" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="VendorCD" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Area" Width="120" />
				<px:PXGridColumn DataField="DefRetainagePct" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="PaymentCalculateMethod" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="PerformanceGuaranteePct" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="WarrantyYear" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="PaymentPeriod1" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="PaymentPct1" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="PaymentPeriod2" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="PaymentPct2" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="ErrorMsg" Width="280" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CountByRefNbr" Width="70" ></px:PXGridColumn></Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
		<ActionBar >
		
			<Actions>
				<Upload Enabled="True" /></Actions></ActionBar>
	
		<Mode AllowUpload="True" ></Mode></px:PXGrid>
</asp:Content>