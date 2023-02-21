<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="NM509999.aspx.cs" Inherits="Page_NM509999" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/ListView.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="NM.NMUploadVendorLocation"
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
				<px:PXGridColumn DataField="Error" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="VendorID" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="VendorLocationCD" Width="70" />
				<px:PXGridColumn DataField="VendorLocationName" Width="280" ></px:PXGridColumn>
				<px:PXGridColumn DataField="PaymentMethod" Width="180" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CashAccount" Width="120" ></px:PXGridColumn>
				<px:PXGridColumn DataField="PaymentSameAsDefault" Width="60" ></px:PXGridColumn>
				<px:PXGridColumn DataField="BankID" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="TaxRegistrationID" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="BankName" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="BankBranchID" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="BankBranchName" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="BankAccount" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="BankAccountName" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Category" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CategoryMappingID" Width="280" ></px:PXGridColumn></Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
		<ActionBar >
		
			<Actions>
				<Upload Enabled="True" ></Upload></Actions></ActionBar>
	
		<Mode AllowUpload="True" /></px:PXGrid>
</asp:Content>