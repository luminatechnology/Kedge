<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="NM502009.aspx.cs" Inherits="Page_NM502009" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="NM.NMTrConfirmProcess" PrimaryView="Filters">
		<CallbackCommands>
                        <px:PXDSCallbackCommand Name="toggleSource" Visible="false" DependOnGrid="grid" />
		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView AllowCollapse="True" ID="form" runat="server" DataSourceID="ds" DataMember="Filters" Width="100%" Height="270px" AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartRow="True"></px:PXLayoutRule>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule47" StartColumn="True" ></px:PXLayoutRule>
			<px:PXPanel runat="server" ID="CstPanel50" Caption="Filters">
				<px:PXLayoutRule runat="server" ID="CstPXLayoutRule51" StartColumn="True" ></px:PXLayoutRule>
				<px:PXDateTimeEdit CommitChanges="True" runat="server" ID="CstPXDateTimeEdit55" DataField="DueDateFrom" ></px:PXDateTimeEdit>
				<px:PXDateTimeEdit CommitChanges="True" runat="server" ID="CstPXDateTimeEdit56" DataField="PayDate" ></px:PXDateTimeEdit>
				<px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector54" DataField="ContractID" ></px:PXSelector>
				<px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector61" DataField="TrConfirmBy" ></px:PXSelector>
				<px:PXTextEdit CommitChanges="True" runat="server" ID="CstPXTextEdit53" DataField="AccConfirmNbrFrom" ></px:PXTextEdit>
				<px:PXLayoutRule runat="server" ID="CstPXLayoutRule52" StartColumn="True" ></px:PXLayoutRule>
				<px:PXDateTimeEdit CommitChanges="True" runat="server" ID="CstPXDateTimeEdit59" DataField="DueDateTo" ></px:PXDateTimeEdit>
				<px:PXDateTimeEdit CommitChanges="True" runat="server" ID="CstPXDateTimeEdit58" DataField="ConfirmDate" ></px:PXDateTimeEdit>
				<px:PXSegmentMask CommitChanges="True" runat="server" ID="CstPXSegmentMask62" DataField="Vendor" ></px:PXSegmentMask>
				<px:PXDropDown CommitChanges="True" runat="server" ID="CstPXDropDown60" DataField="Status" ></px:PXDropDown>
				<px:PXTextEdit CommitChanges="True" runat="server" ID="CstPXTextEdit57" DataField="AccConfirmNbrTo" ></px:PXTextEdit></px:PXPanel>
			<px:PXPanel Caption="Count" runat="server" ID="CstPanel38">
				<px:PXLayoutRule runat="server" ID="CstPXLayoutRule39" StartColumn="True" ></px:PXLayoutRule>
				<px:PXNumberEdit CommitChanges="True" runat="server" ID="CstPXNumberEdit40" DataField="TTCount" ></px:PXNumberEdit>
				<px:PXNumberEdit CommitChanges="True" runat="server" ID="CstPXNumberEdit44" DataField="CheckCount" ></px:PXNumberEdit>
				<px:PXNumberEdit runat="server" ID="CstPXNumberEdit69" DataField="AuthCount" ></px:PXNumberEdit>
				<px:PXNumberEdit runat="server" ID="CstPXNumberEdit67" DataField="GiftCount" ></px:PXNumberEdit>
				<px:PXNumberEdit CommitChanges="True" runat="server" ID="CstPXNumberEdit42" DataField="CashCount" ></px:PXNumberEdit>
				<px:PXLayoutRule runat="server" ID="CstPXLayoutRule46" StartColumn="True" ></px:PXLayoutRule>
				<px:PXNumberEdit runat="server" ID="CstPXNumberEdit41" DataField="TTCountTotalAmt" ></px:PXNumberEdit>
				<px:PXNumberEdit runat="server" ID="CstPXNumberEdit45" DataField="CheckCountTotalAmt" ></px:PXNumberEdit>
				<px:PXNumberEdit runat="server" ID="CstPXNumberEdit70" DataField="AuthCountTotalAmt" ></px:PXNumberEdit>
				<px:PXNumberEdit runat="server" ID="CstPXNumberEdit68" DataField="GiftCountTotalAmt" ></px:PXNumberEdit>
				<px:PXNumberEdit runat="server" ID="CstPXNumberEdit43" DataField="CashCountTotalAmt" ></px:PXNumberEdit></px:PXPanel>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule48" StartColumn="True" ></px:PXLayoutRule>
			<px:PXPanel Caption="Insert TrConfirm Data" runat="server" ID="CstPanel63">
				<px:PXLayoutRule runat="server" ID="CstPXLayoutRule64" StartColumn="True" ></px:PXLayoutRule>
				<px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector66" DataField="TrPaymentType" ></px:PXSelector>
				<px:PXNumberEdit CommitChanges="True" runat="server" ID="CstPXNumberEdit65" DataField="TrConfirmID" ></px:PXNumberEdit></px:PXPanel>
			<px:PXPanel runat="server" ID="CstPanel71" Caption="Insert BillPayment Data">
				<px:PXLayoutRule runat="server" ID="CstPXLayoutRule72" StartColumn="True" />
				<px:PXDropDown CommitChanges="True" runat="server" ID="CstPXDropDown74" DataField="PaymentMethod" ></px:PXDropDown>
				<px:PXSelector AutoRefresh="True" CommitChanges="True" runat="server" ID="CstPXSelector73" DataField="BankAccountID" ></px:PXSelector></px:PXPanel></Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXGrid PageSize="100" SyncPosition="True" ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="Details" AllowAutoHide="false">
		<Levels>
			<px:PXGridLevel DataMember="Details">
			    <Columns>
				<px:PXGridColumn DataField="ISSelected" Width="60" CommitChanges="True" Type="CheckBox" AllowCheckAll="True" ></px:PXGridColumn>
				<px:PXGridColumn LinkCommand="ViewGL" DataField="UsrAccConfirmNbr" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="RefNbr" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="BranchID" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="BranchID_description" Width="220" ></px:PXGridColumn>
				<px:PXGridColumn DataField="DueDate" Width="90" ></px:PXGridColumn>
				<px:PXGridColumn DataField="ProjectID" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="ProjectID_description" Width="280" ></px:PXGridColumn>
				<px:PXGridColumn DataField="BankShortName" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="BankAccountID" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn Type="CheckBox" DataField="EnableIssueByBank" Width="60" ></px:PXGridColumn>
				<px:PXGridColumn DataField="LocationID" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="LocationDescr" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="PostalCode" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="AddrLine1" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="VendorID" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="VendorID_description" Width="280" ></px:PXGridColumn>
				<px:PXGridColumn DataField="AcctName" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="BankAcct" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="BankName" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="BankNbr" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="BranchName" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="BranchNbr" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CategoryID" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Category" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn CommitChanges="True" DataField="PaymentMethod" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="PaymentDate" Width="90" ></px:PXGridColumn>
				<px:PXGridColumn DataField="ActPayAmt" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="PaymentPct" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="PaymentPeriod" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CheckDueType" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CheckTitle" Width="280" ></px:PXGridColumn>
				<px:PXGridColumn CommitChanges="True" DataField="UsrTrPaymentType" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="UsrTrPaymentType_description" Width="220" ></px:PXGridColumn>
				<px:PXGridColumn DataField="UsrTrConfirmID" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="UsrTrConfirmDate" Width="90" ></px:PXGridColumn>
				<px:PXGridColumn DataField="UsrTrConfirmBy" Width="220" ></px:PXGridColumn>
				<px:PXGridColumn DataField="UsrIsAlertVendor" Width="60" Type="CheckBox" ></px:PXGridColumn></Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
		<ActionBar>		
			<Actions>
				<AddNew ToolBarVisible="False" ></AddNew>
				<Delete Enabled="false" />
			</Actions>
                         <CustomItems>
                        <px:PXToolBarButton>
                            <AutoCallBack Command="toggleSource" Target="ds" />
                        </px:PXToolBarButton>
                    </CustomItems>
		</ActionBar>
		<Mode AllowAddNew="False" AllowDelete="False" ></Mode></px:PXGrid>
</asp:Content>