<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="NM502000.aspx.cs" Inherits="Page_NM502000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="NM.NMApCheckProcess"
        PrimaryView="MasterView"
        >
		<CallbackCommands>
                      <px:PXDSCallbackCommand CommitChanges="True" Name="confirm" Visible="True" ></px:PXDSCallbackCommand>
		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="MasterView" Width="100%" Height="" AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartRow="True"></px:PXLayoutRule>
			<px:PXLayoutRule ColumnWidth="" ControlSize="" runat="server" ID="CstPXLayoutRule18" StartColumn="True" ></px:PXLayoutRule>
			<px:PXLayoutRule ColumnWidth="XM" ControlSize="L" GroupCaption="Filter" runat="server" ID="CstPXLayoutRule20" StartGroup="True" ></px:PXLayoutRule>
			<px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector45" DataField="BankAccountID" ></px:PXSelector>
			<px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector41" DataField="VendorID" ></px:PXSelector>
			<px:PXSegmentMask runat="server" ID="CstPXSegmentMask53" DataField="ProjectID" CommitChanges="True" ></px:PXSegmentMask>
			<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit54" DataField="EtdDepositDate" CommitChanges="True" ></px:PXDateTimeEdit>
			<px:PXDateTimeEdit CommitChanges="True" runat="server" ID="CstPXDateTimeEdit55" DataField="DueDate" ></px:PXDateTimeEdit>
			<px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector64" DataField="UsrTrPaymentType" ></px:PXSelector>
			<px:PXNumberEdit CommitChanges="True" runat="server" ID="CstPXNumberEdit63" DataField="UsrTrConfirmID" ></px:PXNumberEdit>
			<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit66" DataField="UsrTrConfirmDate" CommitChanges="True" ></px:PXDateTimeEdit>
			<px:PXSelector runat="server" ID="CstPXSelector65" DataField="UsrTrConfirmBy" CommitChanges="True" ></px:PXSelector>
			<px:PXLayoutRule ColumnWidth="" ControlSize="" runat="server" ID="CstPXLayoutRule19" StartColumn="True" ></px:PXLayoutRule>
			<px:PXLayoutRule ColumnWidth="XM" ControlSize="L" runat="server" ID="CstPXLayoutRule24" StartGroup="True" GroupCaption="Insert Data" ></px:PXLayoutRule>
			<px:PXDateTimeEdit CommitChanges="True" runat="server" ID="CstPXDateTimeEdit50" DataField="CheckDate" ></px:PXDateTimeEdit>
			<px:PXSegmentMask CommitChanges="True" runat="server" ID="CstPXSegmentMask49" DataField="CashierID" ></px:PXSegmentMask>
			<px:PXTextEdit CommitChanges="True" runat="server" ID="CstPXTextEdit46" DataField="Description" ></px:PXTextEdit>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule56" StartColumn="True" ></px:PXLayoutRule>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule57" StartGroup="True" GroupCaption="Select" ></px:PXLayoutRule>
			<px:PXNumberEdit runat="server" ID="CstPXNumberEdit59" DataField="SelectedCount" ></px:PXNumberEdit>
			<px:PXNumberEdit runat="server" ID="CstPXNumberEdit60" DataField="SelectedOriCuryAmount" ></px:PXNumberEdit></Template>
	
		<AutoSize Container="Parent" ></AutoSize></px:PXFormView>
	<px:PXGrid PageSize="100" SyncPosition="True" SkinID="Details" Height="150" Width="100%" runat="server" ID="CstPXGrid17">
		<Levels>
			<px:PXGridLevel DataMember="DetailView" >
				<Columns>
					<px:PXGridColumn DataField="RefNbr" Width="140" ></px:PXGridColumn>
					<px:PXGridColumn DataField="UsrAccConfirmNbr" Width="70" LinkCommand="ViewBatch" ></px:PXGridColumn>
					<px:PXGridColumn Type="CheckBox" DataField="Selected" Width="60" CommitChanges="True" AllowCheckAll="True" ></px:PXGridColumn>
					<px:PXGridColumn DataField="BranchID" Width="70" ></px:PXGridColumn>
					<px:PXGridColumn DataField="CheckDate" Width="90" ></px:PXGridColumn>
					<px:PXGridColumn DataField="DueDate" Width="90" ></px:PXGridColumn>
					<px:PXGridColumn DataField="EtdDepositDate" Width="90" ></px:PXGridColumn>
					<px:PXGridColumn LinkCommand="ViewAPVendor" DataField="APVendorID" Width="140" ></px:PXGridColumn>
					<px:PXGridColumn DataField="APVendorLocationID" Width="70" ></px:PXGridColumn>
					<px:PXGridColumn LinkCommand="viewVendor" DataField="VendorID" Width="140" ></px:PXGridColumn>
					<px:PXGridColumn DataField="VendorID_BAccount2_acctName" Width="280" ></px:PXGridColumn>
					<px:PXGridColumn CommitChanges="True" DataField="VendorLocationID" Width="70" ></px:PXGridColumn>
					<px:PXGridColumn DataField="ProjectID" Width="70" ></px:PXGridColumn>
					<px:PXGridColumn DataField="ProjectPeriod" Width="70" ></px:PXGridColumn>
					<px:PXGridColumn CommitChanges="True" DataField="BankAccountID" Width="180" ></px:PXGridColumn>
					<px:PXGridColumn Type="CheckBox" DataField="EnableIssueByBank" Width="60" ></px:PXGridColumn>
					<px:PXGridColumn DataField="BookNbr" Width="180" ></px:PXGridColumn>
					<px:PXGridColumn DataField="CuryID" Width="70" ></px:PXGridColumn>
					<px:PXGridColumn DataField="OriCuryAmount" Width="100" ></px:PXGridColumn>
					<px:PXGridColumn CommitChanges="True" DataField="CashAccountID" Width="120" ></px:PXGridColumn>
					<px:PXGridColumn DataField="UsrTrPaymentType" Width="70" ></px:PXGridColumn>
					<px:PXGridColumn DataField="UsrTrConfirmID" Width="70" ></px:PXGridColumn>
					<px:PXGridColumn DataField="UsrTrConfirmBy" Width="220" ></px:PXGridColumn>
					<px:PXGridColumn DataField="UsrTrConfirmDate" Width="90" ></px:PXGridColumn></Columns>
                                 <RowTemplate>
                                    <px:PXSelector runat="server" ID="PXSelector_VendorLocationID" DataField="VendorLocationID" AutoRefresh="True" ></px:PXSelector>
                                    <px:PXSelector runat="server" ID="PXSelector_BankAccountID" DataField="BankAccountID" AutoRefresh="True" ></px:PXSelector>
                                    <px:PXSelector runat="server" ID="PXSelector_BookNbr" DataField="BookNbr" AutoRefresh="True" ></px:PXSelector>
                                </RowTemplate>
</px:PXGridLevel></Levels>
		<AutoSize Enabled="True" MinHeight="150" Container="Window" ></AutoSize>

  <ActionBar >
    <Actions>
      <AddNew Enabled="False" ></AddNew></Actions>
    <Actions>
      <AddNew MenuVisible="False" ></AddNew></Actions>
    <Actions>
      <Delete Enabled="False" ></Delete></Actions>
    <Actions>
      <Delete MenuVisible="False" ></Delete></Actions>
  </ActionBar>
		<Mode AllowAddNew="False" ></Mode>
		<Mode AllowDelete="False" ></Mode></px:PXGrid></asp:Content>