<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="PS301000.aspx.cs" Inherits="Page_PS301000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/ListView.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="PS.PSUploadPaymentSlip"
        PrimaryView="DetailsView"
        >
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phL" runat="Server">
	<px:PXGrid SyncPosition="True" ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="Primary" AllowAutoHide="false">
		<Levels>
			<px:PXGridLevel DataMember="DetailsView">
			    <Columns>
				<px:PXGridColumn DataField="Error" Width="280" ></px:PXGridColumn>
				<px:PXGridColumn DataField="DocType" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="RefNbr" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="DocDate" Width="90" ></px:PXGridColumn>
				<px:PXGridColumn DataField="TargetType" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn CommitChanges="True" DataField="PayerID" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="PayerLocationID" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="EmployeeID" Width="140" />
				<px:PXGridColumn DataField="ContractID" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="DocDesc" Width="280" ></px:PXGridColumn>
				<px:PXGridColumn DataField="PaymentCategory" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="LineDate" Width="90" ></px:PXGridColumn>
				<px:PXGridColumn DataField="LinePayNbr" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="LinePayMehtod" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="LineInvID" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="LineDesc" Width="280" ></px:PXGridColumn>
				<px:PXGridColumn DataField="LineQty" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="LineCost" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="LineIssues" Width="280" ></px:PXGridColumn>
				<px:PXGridColumn DataField="IssueBankCode" Width="84" ></px:PXGridColumn>
				<px:PXGridColumn DataField="IssueBankAccount" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="BankAccountID" Width="180" ></px:PXGridColumn>
				<px:PXGridColumn DataField="ActualDueDate" Width="90" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CheckDueDate" Width="90" ></px:PXGridColumn>
				<px:PXGridColumn DataField="ProcessDate" Width="90" ></px:PXGridColumn>
				<px:PXGridColumn DataField="IssueDate" Width="90" ></px:PXGridColumn>
				<px:PXGridColumn DataField="PONbr" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="GuarType" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="GuarClass" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="EtdDepositDate" Width="90" ></px:PXGridColumn>
				<px:PXGridColumn DataField="AuthDate" Width="90" ></px:PXGridColumn></Columns>
			
				<RowTemplate>
					<px:PXSelector runat="server" ID="CstPXSelector1" DataField="PayerLocationID" AutoRefresh="True" ></px:PXSelector></RowTemplate></px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
		<ActionBar >
		
			<Actions>
				<Upload Enabled="True" ></Upload></Actions></ActionBar>
	
		<Mode AllowUpload="True" AllowUpdate="True" ></Mode></px:PXGrid>
</asp:Content>