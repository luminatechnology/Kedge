<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="NM502002.aspx.cs" Inherits="Page_NM502002" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="NM.NMApCheckSendProcess"
        PrimaryView="CheckFilters"
        >
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="CheckFilters" Width="100%" Height="250px" AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule43" StartRow="True" ></px:PXLayoutRule>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule44" StartColumn="True" ></px:PXLayoutRule>
			<px:PXLayoutRule GroupCaption="Filters" runat="server" ID="CstPXLayoutRule47" StartGroup="True" ></px:PXLayoutRule>
			<px:PXPanel runat="server" ID="CstPanel48">
				<px:PXLayoutRule runat="server" ID="CstPXLayoutRule49" StartColumn="True" ></px:PXLayoutRule>
				<px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector52" DataField="BankAccountID" ></px:PXSelector>
				<px:PXTextEdit runat="server" ID="CstPXTextEdit75" DataField="CheckNbrFrom" CommitChanges="True" ></px:PXTextEdit>
				<px:PXDateTimeEdit CommitChanges="True" runat="server" ID="CstPXDateTimeEdit53" DataField="CheckDateFrom" ></px:PXDateTimeEdit>
				<px:PXDateTimeEdit CommitChanges="True" runat="server" ID="CstPXDateTimeEdit72" DataField="EtdDepositDateFrom" ></px:PXDateTimeEdit>
				<px:PXSegmentMask CommitChanges="True" runat="server" ID="CstPXSegmentMask56" DataField="ProjectID" ></px:PXSegmentMask>
				<px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector57" DataField="VendorID" ></px:PXSelector>
				<px:PXLayoutRule runat="server" ID="CstPXLayoutRule50" StartColumn="True" ></px:PXLayoutRule>
				<px:PXSegmentMask CommitChanges="True" runat="server" ID="CstPXSegmentMask74" DataField="PayableCashierID" ></px:PXSegmentMask>
				<px:PXTextEdit CommitChanges="True" runat="server" ID="CstPXTextEdit76" DataField="CheckNbrTo" ></px:PXTextEdit>
				<px:PXDateTimeEdit CommitChanges="True" runat="server" ID="CstPXDateTimeEdit59" DataField="CheckDateTo" ></px:PXDateTimeEdit>
				<px:PXDateTimeEdit CommitChanges="True" runat="server" ID="CstPXDateTimeEdit73" DataField="EtdDepositDateTo" ></px:PXDateTimeEdit>
				<px:PXNumberEdit CommitChanges="True" runat="server" ID="CstPXNumberEdit62" DataField="ProjectPeriod" ></px:PXNumberEdit>
				<px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector77" DataField="VendorLocationID" ></px:PXSelector>
				<px:PXLayoutRule runat="server" ID="CstPXLayoutRule78" StartColumn="True" ></px:PXLayoutRule>
				<px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector79" DataField="TrConfirmBy" ></px:PXSelector>
				<px:PXDateTimeEdit CommitChanges="True" runat="server" ID="CstPXDateTimeEdit80" DataField="TrConfirmDate" ></px:PXDateTimeEdit>
				<px:PXNumberEdit CommitChanges="True" runat="server" ID="CstPXNumberEdit81" DataField="TrConfirmID" ></px:PXNumberEdit>
				<px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector82" DataField="TrPaymentType" ></px:PXSelector></px:PXPanel>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule45" StartColumn="True" ></px:PXLayoutRule>
			<px:PXLayoutRule GroupCaption="Insert Data" runat="server" ID="CstPXLayoutRule51" StartGroup="True" ></px:PXLayoutRule>
			<px:PXSegmentMask runat="server" ID="CstPXSegmentMask66" DataField="SendCashierID" ></px:PXSegmentMask>
			<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit64" DataField="DeliverDate" ></px:PXDateTimeEdit>
			<px:PXDropDown runat="server" ID="CstPXDropDown65" DataField="DeliverMethod" ></px:PXDropDown>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule67" StartColumn="True" ></px:PXLayoutRule>
			<px:PXLayoutRule GroupCaption="Select Cal" runat="server" ID="CstPXLayoutRule68" StartGroup="True" ></px:PXLayoutRule>
			<px:PXNumberEdit runat="server" ID="CstPXNumberEdit70" DataField="SelectedCount" ></px:PXNumberEdit>
			<px:PXNumberEdit runat="server" ID="CstPXNumberEdit71" DataField="SelectedOriCuryAmount" ></px:PXNumberEdit></Template>
	
		<AutoSize Container="Parent" ></AutoSize></px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXGrid SyncPosition="True" ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="Details" AllowAutoHide="false">
		<Levels>
			<px:PXGridLevel DataMember="CheckDetails">
			    <Columns>
				<px:PXGridColumn AllowCheckAll="True" CommitChanges="True" Type="CheckBox" DataField="Selected" Width="60" ></px:PXGridColumn>
				<px:PXGridColumn CommitChanges="True" LinkCommand="ViewPayableCD" DataField="PayableCheckCD" Width="180" ></px:PXGridColumn>
				<px:PXGridColumn LinkCommand="ViewAPReleaseGL" CommitChanges="True" DataField="APInvBatchNbr" Width="280" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Status" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CheckNbr" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="BankAccountID" Width="180" ></px:PXGridColumn>
				<px:PXGridColumn DataField="BookNbr" Width="180" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CheckDate" Width="90" ></px:PXGridColumn>
				<px:PXGridColumn DataField="EtdDepositDate" Width="90" ></px:PXGridColumn>
				<px:PXGridColumn DataField="VendorID" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="VendorID_description" Width="280" ></px:PXGridColumn>
				<px:PXGridColumn DataField="VendorLocationID" Width="180" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Receiver" Width="280" ></px:PXGridColumn>
				<px:PXGridColumn DataField="ProjectID" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="ProjectID_description" Width="220" ></px:PXGridColumn>
				<px:PXGridColumn DataField="ProjectPeriod" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CuryID" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="BaseCuryAmount" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="OriCuryAmount" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="KGBillPayment__UsrTrConfirmBy" Width="220" />
				<px:PXGridColumn DataField="KGBillPayment__UsrTrConfirmDate" Width="90" />
				<px:PXGridColumn DataField="KGBillPayment__UsrTrConfirmID" Width="70" />
				<px:PXGridColumn DataField="KGBillPayment__UsrTrPaymentType" Width="70" />
				<px:PXGridColumn DataField="KGBillPayment__UsrTrPaymentType_description" Width="220" /></Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
		<ActionBar >
		
			<Actions>
				<AddNew ToolBarVisible="False" ></AddNew></Actions>
			<Actions>
				<Delete ToolBarVisible="False" ></Delete></Actions></ActionBar>
	</px:PXGrid>
</asp:Content>