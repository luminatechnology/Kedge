<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormTab.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="KG505004.aspx.cs" Inherits="Page_KG505004" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormTab.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="Kedge.KGWriteOffProcess"
        PrimaryView="Filters"
        >
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="Filters" Width="100%" Height="200px" AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartRow="True"></px:PXLayoutRule>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule1" StartColumn="True" ></px:PXLayoutRule>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule3" StartGroup="True" ></px:PXLayoutRule>
			<px:PXDropDown runat="server" ID="CstPXDropDown16" DataField="Target" CommitChanges="True" ></px:PXDropDown>
			<px:PXSelector runat="server" ID="CstPXSelector8" DataField="RefNbr" CommitChanges="True" ></px:PXSelector>
			<px:PXSelector runat="server" ID="CstPXSelector4" DataField="AccConfrimNbr" CommitChanges="True" ></px:PXSelector>
			<px:PXSelector runat="server" ID="CstPXSelector5" DataField="ContractID" CommitChanges="True" ></px:PXSelector>
			<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit7" DataField="PaymentDate" CommitChanges="True" ></px:PXDateTimeEdit>
			<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit6" DataField="DueDate" CommitChanges="True" ></px:PXDateTimeEdit>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule2" StartColumn="True" ></px:PXLayoutRule>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule9" StartGroup="True" ></px:PXLayoutRule>
			<px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector10" DataField="TrConfirmBy" ></px:PXSelector>
			<px:PXDateTimeEdit CommitChanges="True" runat="server" ID="CstPXDateTimeEdit11" DataField="TrConfirmDate" ></px:PXDateTimeEdit>
			<px:PXNumberEdit CommitChanges="True" runat="server" ID="CstPXNumberEdit12" DataField="TrConfirmID" ></px:PXNumberEdit>
			<px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector13" DataField="TrPaymentType" ></px:PXSelector>
			<px:PXDropDown CommitChanges="True" runat="server" ID="CstPXDropDown14" DataField="PaymentMethod" ></px:PXDropDown></Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXTab ID="tab" runat="server" Width="100%" Height="150px" DataSourceID="ds" AllowAutoHide="false">
		<Items>
			<px:PXTabItem Text="INV">
				<Template>
					<px:PXGrid runat="server" ID="CstPXGrid19" SkinID="details" Height="250" Width="100%" SyncPosition="True">
						<Levels>
							<px:PXGridLevel DataMember="INVViews">
								<Columns>
									<px:PXGridColumn AllowCheckAll="True" CommitChanges="True" Type="CheckBox" DataField="Selected" Width="60" ></px:PXGridColumn>
									<px:PXGridColumn DataField="APInvoice__DocType" Width="70" ></px:PXGridColumn>
									<px:PXGridColumn DataField="RefNbr" Width="140" ></px:PXGridColumn>
									<px:PXGridColumn DataField="APRegister__UsrAccConfirmNbr" Width="140" ></px:PXGridColumn>
									<px:PXGridColumn DataField="APRegister__ProjectID" Width="70" ></px:PXGridColumn>
									<px:PXGridColumn DataField="APRegister__ProjectID_description" Width="280" ></px:PXGridColumn>
									<px:PXGridColumn DataField="APInvoice__DueDate" Width="90" ></px:PXGridColumn>
									<px:PXGridColumn DataField="VendorID" Width="140" ></px:PXGridColumn>
									<px:PXGridColumn DataField="VendorID_description" Width="280" ></px:PXGridColumn>
									<px:PXGridColumn DataField="PaymentDate" Width="90" ></px:PXGridColumn>
									<px:PXGridColumn DataField="PaymentMethod" Width="70" ></px:PXGridColumn>
									<px:PXGridColumn DataField="NMBankAccount__BankAccountCD" Width="180" ></px:PXGridColumn>
									<px:PXGridColumn DataField="NMBankAccount__BankShortName" Width="140" />
									<px:PXGridColumn DataField="PaymentAmount" Width="100" ></px:PXGridColumn>
									<px:PXGridColumn DataField="PaymentPct" Width="100" ></px:PXGridColumn>
									<px:PXGridColumn DataField="IsPostageFree" Width="60" ></px:PXGridColumn>
									<px:PXGridColumn DataField="PostageAmt" Width="100" ></px:PXGridColumn>
									<px:PXGridColumn DataField="ActPayAmt" Width="100" ></px:PXGridColumn>
									<px:PXGridColumn DataField="UsrTrPaymentType" Width="70" ></px:PXGridColumn>
									<px:PXGridColumn DataField="UsrTrPaymentType_description" Width="220" ></px:PXGridColumn>
									<px:PXGridColumn DataField="UsrTrConfirmID" Width="70" ></px:PXGridColumn>
									<px:PXGridColumn DataField="UsrTrConfirmDate" Width="90" ></px:PXGridColumn>
									<px:PXGridColumn DataField="UsrTrConfirmBy" Width="220" ></px:PXGridColumn>
									<px:PXGridColumn Type="CheckBox" DataField="UsrIsWriteOff" Width="60" ></px:PXGridColumn></Columns></px:PXGridLevel></Levels>
						<ActionBar>
							<Actions>
								<AddNew ToolBarVisible="False" />
								<Delete ToolBarVisible="False" /></Actions></ActionBar></px:PXGrid></Template>
			</px:PXTabItem>
			<px:PXTabItem Text="ADR">
				<Template>
					<px:PXGrid SkinID="details" runat="server" ID="CstPXGrid18" Height="250" Width="100%" SyncPosition="True">
						<Levels>
							<px:PXGridLevel DataMember="ADRViews">
								<Columns>
									<px:PXGridColumn AllowCheckAll="True" CommitChanges="True" Type="CheckBox" DataField="Selected" Width="60" ></px:PXGridColumn>
									<px:PXGridColumn DataField="OrigRefNbr" Width="140" ></px:PXGridColumn>
									<px:PXGridColumn DataField="RefNbr" Width="140" ></px:PXGridColumn>
									<px:PXGridColumn DataField="UsrAccConfirmNbr" Width="140" ></px:PXGridColumn>
									<px:PXGridColumn DataField="VendorID" Width="140" ></px:PXGridColumn>
									<px:PXGridColumn DataField="VendorID_BAccountR_acctName" Width="280" ></px:PXGridColumn>
									<px:PXGridColumn DataField="ProjectID" Width="70" ></px:PXGridColumn>
									<px:PXGridColumn DataField="ProjectID_description" Width="280" ></px:PXGridColumn>
									<px:PXGridColumn DataField="APInvoice__DueDate" Width="90" />
									<px:PXGridColumn DataField="CuryOrigDocAmt" Width="100" ></px:PXGridColumn>
									<px:PXGridColumn DataField="UsrIsDeductionDoc" Width="60" ></px:PXGridColumn></Columns></px:PXGridLevel></Levels>
						<ActionBar>
							<Actions>
								<AddNew ToolBarVisible="False" ></AddNew></Actions></ActionBar>
						<ActionBar>
							<Actions>
								<Delete ToolBarVisible="False" ></Delete></Actions></ActionBar></px:PXGrid></Template>
			</px:PXTabItem>
		</Items>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" />
	</px:PXTab>
</asp:Content>