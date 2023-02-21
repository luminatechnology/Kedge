<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="KG505005.aspx.cs" Inherits="Page_KG505005" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/ListView.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="Kedge.KGBillWriteoffProcess"
        PrimaryView="INVViews"
        >
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phL" runat="Server">
	<px:PXGrid ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="Primary" AllowAutoHide="false">
		<Levels>
			<px:PXGridLevel DataMember="INVViews">
			    <Columns>
				<px:PXGridColumn CommitChanges="True" Type="CheckBox" AllowCheckAll="True" DataField="Selected" Width="60" ></px:PXGridColumn>
				<px:PXGridColumn DataField="RefNbr" Width="140" />
				<px:PXGridColumn DataField="APRegister__UsrAccConfirmNbr" Width="140" />
				<px:PXGridColumn DataField="APRegister__ProjectID" Width="70" />
				<px:PXGridColumn DataField="APRegister__ProjectID_description" Width="280" />
				<px:PXGridColumn DataField="APInvoice__DueDate" Width="90" />
				<px:PXGridColumn DataField="PaymentDate" Width="90" />
				<px:PXGridColumn DataField="PaymentMethod" Width="70" />
				<px:PXGridColumn DataField="NMBankAccount__BankAccountCD" Width="180" />
				<px:PXGridColumn DataField="NMBankAccount__BankShortName" Width="140" />
				<px:PXGridColumn DataField="PaymentAmount" Width="100" />
				<px:PXGridColumn DataField="PaymentPct" Width="100" />
				<px:PXGridColumn DataField="UsrTrPaymentType" Width="70" />
				<px:PXGridColumn DataField="UsrTrPaymentType_description" Width="220" />
				<px:PXGridColumn DataField="UsrTrConfirmID" Width="70" />
				<px:PXGridColumn DataField="UsrTrConfirmDate" Width="90" />
				<px:PXGridColumn DataField="UsrTrConfirmBy" Width="220" />
				<px:PXGridColumn Type="CheckBox" DataField="UsrIsWriteOff" Width="60" ></px:PXGridColumn></Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" />
		<ActionBar >
		</ActionBar>
	</px:PXGrid>
</asp:Content>