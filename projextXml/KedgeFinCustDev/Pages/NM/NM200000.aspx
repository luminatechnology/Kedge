<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="NM200000.aspx.cs" Inherits="Page_NM200000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormView.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="NM.NMBankAccountMaint"
        PrimaryView="BankAccounts"
        >
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="BankAccounts" Width="100%" AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartRow="True"></px:PXLayoutRule>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule24" StartColumn="True" ColumnWidth="XL" ></px:PXLayoutRule>
			<px:PXSelector runat="server" ID="CstPXSelector25" DataField="BankAccountCD" ></px:PXSelector>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit27" DataField="BankName" ></px:PXTextEdit>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit29" DataField="BankShortName" ></px:PXTextEdit>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit26" DataField="BankAddress" ></px:PXTextEdit>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit28" DataField="BankPhone" ></px:PXTextEdit>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit30" DataField="Contactor" ></px:PXTextEdit>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule49" Merge="True" ></px:PXLayoutRule>
			<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit43" DataField="SettlementDate" ></px:PXDateTimeEdit>
			<px:PXCheckBox runat="server" ID="CstPXCheckBox48" DataField="IsSettlement" CommitChanges="True" ></px:PXCheckBox>
			<px:PXLayoutRule runat="server" ID="CstLayoutRule50" ></px:PXLayoutRule>
			<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit44" DataField="ActivationDate" ></px:PXDateTimeEdit>
			<px:PXLabel runat="server" ID="CstLabel53" Text="" />
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule39" StartColumn="False" ColumnSpan="2" ></px:PXLayoutRule>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit31" DataField="Description" ></px:PXTextEdit>
			<px:PXLayoutRule ColumnWidth="XL" runat="server" ID="CstPXLayoutRule14" StartColumn="True" ></px:PXLayoutRule>
			<px:PXSelector runat="server" ID="CstPXSelector45" DataField="AccountType" ></px:PXSelector>
			<px:PXSelector runat="server" ID="CstPXSelector34" DataField="BankCode" ></px:PXSelector>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit33" DataField="BankAccount" ></px:PXTextEdit>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit42" DataField="AccountName" ></px:PXTextEdit>
			<px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector35" DataField="CashAccountID" ></px:PXSelector>
			<px:PXCheckBox runat="server" ID="CstPXCheckBox51" DataField="EnableIssueByBank" CommitChanges="True" ></px:PXCheckBox>
			<px:PXSelector AutoRefresh="True" runat="server" ID="CstPXSelector41" DataField="PaymentMethodID" ></px:PXSelector>
			<px:PXSelector runat="server" ID="CstPXSelector36" DataField="CuryID" ></px:PXSelector>
			<px:PXNumberEdit runat="server" ID="CstPXNumberEdit52" DataField="RestrictedAmount" /></Template>
		<AutoSize Container="Window" Enabled="True" MinHeight="200" ></AutoSize>
	</px:PXFormView>
</asp:Content>