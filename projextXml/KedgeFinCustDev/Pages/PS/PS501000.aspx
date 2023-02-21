<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="PS501000.aspx.cs" Inherits="Page_PS501000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/ListView.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="PS.PSPaymentSlipProcess"
        PrimaryView="DetailsView"
        >
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phL" runat="Server">
	<px:PXGrid NoteIndicator="False" AllowSearch="True" FastFilterFields="DocType,RefNbr,Status,DocDate,DocDesc,ContractID,ContractID_description,CustomerVendorCD,CustomerVendorName,EmployeeID,DepartmentID" SyncPosition="True" ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="Primary" AllowAutoHide="false">
		<Levels>
			<px:PXGridLevel DataMember="DetailsView">
			    <Columns>
				<px:PXGridColumn Type="CheckBox" AllowCheckAll="True" DataField="Selected" Width="60" ></px:PXGridColumn>
				<px:PXGridColumn DataField="DocType" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn LinkCommand="viewPaymentSlip" DataField="RefNbr" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Status" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="DocDate" Width="90" ></px:PXGridColumn>
				<px:PXGridColumn DataField="DocBal" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="DocDesc" Width="280" ></px:PXGridColumn>
				<px:PXGridColumn DataField="ContractID" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="ContractID_description" Width="220" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CustomerVendorCD" Width="90" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CustomerVendorName" Width="150" ></px:PXGridColumn>
				<px:PXGridColumn DataField="EmployeeID" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="DepartmentID" Width="120" ></px:PXGridColumn></Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
		<ActionBar >
		
			<Actions>
				<AddNew Enabled="False" ></AddNew>
						<NoteShow Enabled="False" ></NoteShow></Actions>
			<Actions>
				<Delete Enabled="False" ></Delete></Actions></ActionBar>
	</px:PXGrid>
</asp:Content>