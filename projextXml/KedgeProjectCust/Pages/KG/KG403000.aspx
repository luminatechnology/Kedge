<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="KG403000.aspx.cs" Inherits="Page_KG403000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="Kedge.KGValuationInq"
        PrimaryView="ValuationFilter"
        >
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="ValuationFilter" Width="100%" Height="" AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartRow="True"></px:PXLayoutRule>
			<px:PXLayoutRule LabelsWidth="S" ControlSize="SM" runat="server" ID="CstPXLayoutRule11" StartColumn="True" ></px:PXLayoutRule>
			<px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector1" DataField="ContractID" ></px:PXSelector>
			<px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector4" DataField="OrderNbrFrom" ></px:PXSelector>
			<px:PXDateTimeEdit CommitChanges="True" runat="server" ID="CstPXDateTimeEdit7" DataField="ValuationDateFrom" ></px:PXDateTimeEdit>
			<px:PXLayoutRule ControlSize="SM" LabelsWidth="S" runat="server" ID="CstPXLayoutRule12" StartColumn="True" ></px:PXLayoutRule>
			<px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector10" DataField="Vendor" ></px:PXSelector>
			<px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector5" DataField="OrderNbrTo" ></px:PXSelector>
			<px:PXDateTimeEdit CommitChanges="True" runat="server" ID="CstPXDateTimeEdit8" DataField="ValuationDateTo" ></px:PXDateTimeEdit>
			<px:PXLayoutRule LabelsWidth="S" ControlSize="S" runat="server" ID="CstPXLayoutRule1" StartColumn="True" ></px:PXLayoutRule>
			<px:PXDropDown CommitChanges="True" runat="server" ID="CstPXDropDown2" DataField="Status" ></px:PXDropDown>
			<px:PXDropDown CommitChanges="True" runat="server" ID="CstPXDropDown9" DataField="ValuationType" ></px:PXDropDown>
			<px:PXDropDown CommitChanges="True" runat="server" ID="CstPXDropDown6" DataField="PricingType" ></px:PXDropDown></Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXGrid  SyncPosition="True" ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="Inquire" AllowAutoHide="false">
		<Levels>
			<px:PXGridLevel DataMember="AllValuation">
			    <Columns>
				<px:PXGridColumn DataField="KGValuationDetail__Status" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn CommitChanges="True" LinkCommand="ViewValuation" DataField="ValuationCD" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Description" Width="70" />
				<px:PXGridColumn DataField="ContractID" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="ContractID_description" Width="220" ></px:PXGridColumn>
				<px:PXGridColumn DataField="DailyRenterCD" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="KGValuationDetail__PricingType" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="KGValuationDetail__OrderNbr" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="ValuationDate" Width="90" ></px:PXGridColumn>
				<px:PXGridColumn DataField="ValuationType" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="POOrder__VendorID" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="POOrder__VendorID_Vendor_acctName" Width="220" ></px:PXGridColumn>
				<px:PXGridColumn DataField="KGValuationDetail__InventoryID" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="KGValuationDetail__InventoryID_description" Width="280" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Uom" Width="72" ></px:PXGridColumn>
				<px:PXGridColumn DataField="KGValuationDetail__Qty" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="UnitPrice" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="KGValuationDetail__Amount" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="KGValuationDetail__TaxAmt" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="KGValuationDetail__TotalAmt" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="KGValuationDetail__ManageFeeAmt" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="KGValuationDetail__ManageFeeTaxAmt" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="KGValuationDetail__ManageFeeTotalAmt" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="KGValuationDetail__APInvoiceNbr" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="KGValuationDetail__APInvoiceDate" Width="90" ></px:PXGridColumn></Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
		<ActionBar >
		</ActionBar>
	</px:PXGrid>
</asp:Content>