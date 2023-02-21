<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormTab.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="NM302000.aspx.cs" Inherits="Page_NM302000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormTab.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="NM.NMApCheckEntry"
        PrimaryView="PayableChecks"
        >
		<CallbackCommands>
                         <px:PXDSCallbackCommand Visible="False" Name="CurrencyView" />
		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="PayableChecks" Width="100%" Height="" AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartRow="True"></px:PXLayoutRule>
			<px:PXLayoutRule ControlSize="XM" ColumnWidth="XL" runat="server" ID="CstPXLayoutRule4" StartColumn="True" ></px:PXLayoutRule>
			<px:PXSelector runat="server" ID="CstPXSelector62" DataField="PayableCheckCD" ></px:PXSelector>
			<px:PXSegmentMask runat="server" ID="CstPXSegmentMask13" DataField="PayableCashierID" ></px:PXSegmentMask>
			<px:PXDropDown runat="server" ID="CstPXDropDown16" DataField="Status" ></px:PXDropDown>
			<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit9" DataField="CheckDate" ></px:PXDateTimeEdit>
			<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit12" DataField="EtdDepositDate" ></px:PXDateTimeEdit>
			<px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector64" DataField="VendorID" ></px:PXSelector>
			<px:PXSelector AutoRefresh="True" runat="server" ID="CstPXSelector70" DataField="VendorLocationID" ></px:PXSelector>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit17" DataField="Title" ></px:PXTextEdit>
			<px:PXSegmentMask runat="server" ID="CstPXSegmentMask14" DataField="ProjectID" ></px:PXSegmentMask>
			<px:PXNumberEdit runat="server" ID="CstPXNumberEdit15" DataField="ProjectPeriod" ></px:PXNumberEdit>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule42" Merge="False" ColumnSpan="2" ></px:PXLayoutRule>
			<px:PXTextEdit Height="50px" TextMode="MultiLine" runat="server" ID="CstPXTextEdit10" DataField="Description" ></px:PXTextEdit>
			<px:PXLayoutRule ControlSize="XM" ColumnWidth="XL" runat="server" ID="CstPXLayoutRule5" StartColumn="True" ></px:PXLayoutRule>
			<px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector20" DataField="BankAccountID" ></px:PXSelector>
			<px:PXSelector AutoRefresh="True" runat="server" ID="CstPXSelector60" DataField="BookNbr" ></px:PXSelector>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule112" Merge="True" />
			<px:PXTextEdit runat="server" ID="CstPXTextEdit23" DataField="CheckNbr" ></px:PXTextEdit>
			<px:PXSelector runat="server" ID="CstPXSelector113" DataField="ManualCheckNbr" CommitChanges="True" />
			<px:PXCheckBox runat="server" ID="CstPXCheckBox114" DataField="IsManualCheckNbr" />
			<px:PXLayoutRule runat="server" ID="CstLayoutRule111" />
			<pxa:PXCurrencyRate ID="edCury" runat="server"  DataField="CuryID"  DataMember="_Currency_"  DataSourceID="ds"  RateTypeView="_NMPayableCheck_CurrencyInfo_" ></pxa:PXCurrencyRate>
			<px:PXNumberEdit CommitChanges="True" runat="server" ID="CstPXNumberEdit25" DataField="OriCuryAmount" ></px:PXNumberEdit>
			<px:PXDropDown runat="server" ID="CstPXDropDown27" DataField="SourceCode" ></px:PXDropDown>
			<px:PXSelector runat="server" ID="CstPXSelector76" DataField="CashAccountID" ></px:PXSelector>
			<px:PXSelector runat="server" ID="CstPXSelector77" DataField="PaymentMethodID" ></px:PXSelector>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit78" DataField="RefNbr" Enabled="False" AllowEdit="True" >
				<LinkCommand Command="viewAPInvoice" Target="ds" ></LinkCommand></px:PXTextEdit>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit87" DataField="APInvBatchNbr" Enabled="False">
				<LinkCommand Target="ds" Command="viewAPReleaseGL" ></LinkCommand></px:PXTextEdit>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule65" StartColumn="True" ></px:PXLayoutRule>
			<px:PXLayoutRule GroupCaption="Print Information" runat="server" ID="CstPXLayoutRule66" StartGroup="True" ></px:PXLayoutRule>
			<px:PXNumberEdit runat="server" ID="CstPXNumberEdit72" DataField="PrintCount" ></px:PXNumberEdit>
			<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit75" DataField="PrintDate" ></px:PXDateTimeEdit>
			<px:PXSelector Size="S" runat="server" ID="CstPXSelector83" DataField="PrintUser" ></px:PXSelector>
			<px:PXLabel runat="server" ID="CstLabel103" Text="" ></px:PXLabel>
			<px:PXLayoutRule GroupCaption="Tr Confirm Info" runat="server" ID="CstPXLayoutRule88" StartGroup="True" ></px:PXLayoutRule>
			<px:PXFormView SkinID="Transparent" runat="server" ID="CstFormView97" DataMember="KGBillPaymentInfo" >
				<Template>
					<px:PXLayoutRule runat="server" ID="CstPXLayoutRule106" StartRow="True" ></px:PXLayoutRule>
					<px:PXSelector runat="server" ID="CstPXSelector110" DataField="UsrTrPaymentType" ></px:PXSelector>
					<px:PXNumberEdit runat="server" ID="CstPXNumberEdit109" DataField="UsrTrConfirmID" ></px:PXNumberEdit>
					<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit108" DataField="UsrTrConfirmDate" ></px:PXDateTimeEdit>
					<px:PXSelector runat="server" ID="CstPXSelector107" DataField="UsrTrConfirmBy" ></px:PXSelector></Template>
				</px:PXFormView></Template>
	
		<AutoSize Enabled="True" MinHeight="200" Container="Window" ></AutoSize></px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXTab ID="tab" runat="server" Width="100%" Height="150px" DataSourceID="ds" AllowAutoHide="false">
		<Items>
			<px:PXTabItem Text="Summary">
				<Template>
					<px:PXFormView Width="100%" runat="server" ID="CstFormView28" DataMember="PayableChecks" >
						<Template>
							<px:PXLayoutRule ColumnWidth="XL" ControlSize="XM" runat="server" ID="CstPXLayoutRule31" StartRow="True" ></px:PXLayoutRule>
							<px:PXLayoutRule runat="server" ID="CstPXLayoutRule43" StartColumn="True" ></px:PXLayoutRule>
							<px:PXLayoutRule runat="server" ID="CstPXLayoutRule71" StartGroup="True" GroupCaption="Payment Information" ></px:PXLayoutRule>
							<px:PXSegmentMask runat="server" ID="CstPXSegmentMask58" DataField="BranchID" ></px:PXSegmentMask>
							<px:PXTextEdit Enabled="False" AllowEdit="True" runat="server" ID="CstPXTextEdit35" DataField="PaymentDocNo" >
								<LinkCommand Command="viewAPPayment" Target="ds" ></LinkCommand></px:PXTextEdit>
							<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit34" DataField="PaymentDocDate" ></px:PXDateTimeEdit>
							<px:PXTextEdit runat="server" ID="CstPXTextEdit82" DataField="ConfirmBatchNbr" Enabled="False">
								<LinkCommand Command="ViewGLVoucherByConfirm" Target="ds" /></px:PXTextEdit>
							<px:PXLayoutRule runat="server" ID="CstPXLayoutRule44" StartColumn="True" ></px:PXLayoutRule>
							<px:PXLayoutRule runat="server" ID="CstPXLayoutRule48" StartGroup="True" GroupCaption="Cash Information" ></px:PXLayoutRule>
							<px:PXSegmentMask runat="server" ID="CstPXSegmentMask50" DataField="CashCashierID" ></px:PXSegmentMask>
							<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit51" DataField="DepositDate" ></px:PXDateTimeEdit>
							<px:PXTextEdit Enabled="False" runat="server" ID="CstPXTextEdit79" DataField="CashBatchNbr">
								<LinkCommand Target="ds" Command="viewGLVoucherByCash" Enabled="True" ></LinkCommand></px:PXTextEdit>
							<px:PXLayoutRule runat="server" ID="CstPXLayoutRule45" StartColumn="True" ></px:PXLayoutRule>
							<px:PXLayoutRule GroupCaption="Modify Information" runat="server" ID="CstPXLayoutRule49" StartGroup="True" ></px:PXLayoutRule>
							<px:PXSegmentMask runat="server" ID="CstPXSegmentMask52" DataField="ModifyCashierID" ></px:PXSegmentMask>
							<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit53" DataField="ModifyDate" ></px:PXDateTimeEdit>
							<px:PXTextEdit runat="server" ID="CstPXTextEdit55" DataField="ModifyReason" ></px:PXTextEdit>
							<px:PXTextEdit Enabled="False" runat="server" ID="CstPXTextEdit80" DataField="CashReverseBatchNbr">
								<LinkCommand Target="ds" Command="ViewGLVoucherByCashRe" Enabled="True" ></LinkCommand></px:PXTextEdit>
							<px:PXTextEdit runat="server" ID="CstPXTextEdit81" DataField="ConfirmReverseBatchNbr" Enabled="False">
								<LinkCommand Enabled="True" Command="ViewGLVoucherByConfirmRe" Target="ds" ></LinkCommand></px:PXTextEdit></Template>
						<AutoSize Container="Parent" Enabled="True" ></AutoSize>
						<AutoSize Container="Parent" ></AutoSize></px:PXFormView></Template>
			</px:PXTabItem>
			<px:PXTabItem Text="Deliver Method" >
				<Template>
					<px:PXFormView Width="100%" runat="server" ID="CstFormView36" DataMember="PayableChecks" >
						<Template>
							<px:PXLayoutRule ColumnWidth="XL" ControlSize="XM" runat="server" ID="CstPXLayoutRule37" StartRow="True" ></px:PXLayoutRule>
							<px:PXDropDown runat="server" ID="CstPXDropDown59" DataField="DeliverMethod" />
							<px:PXTextEdit runat="server" ID="CstPXTextEdit40" DataField="Receiver" ></px:PXTextEdit>
							<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit38" DataField="DeliverDate" ></px:PXDateTimeEdit>
							<px:PXSegmentMask runat="server" ID="CstPXSegmentMask41" DataField="SendCashierID" ></px:PXSegmentMask></Template>
						<AutoSize Container="Parent" ></AutoSize>
						<AutoSize Enabled="True" ></AutoSize></px:PXFormView></Template></px:PXTabItem></Items>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
	</px:PXTab>
</asp:Content>