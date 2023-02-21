<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="NM301000.aspx.cs" Inherits="Page_NM301000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormView.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="NM.NMArCheckEntry"
        PrimaryView="Checks"
        >
		<CallbackCommands>
                       <px:PXDSCallbackCommand Visible="False" Name="CurrencyView" />
		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="Checks" Width="100%" AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule26" StartRow="True" ></px:PXLayoutRule>
			<px:PXLayoutRule ControlSize="XM" runat="server" ID="CstPXLayoutRule27" StartColumn="True" ></px:PXLayoutRule>
			<px:PXSegmentMask runat="server" ID="CstPXSegmentMask52" DataField="CollEmployeeID" ></px:PXSegmentMask>
			<px:PXSegmentMask runat="server" ID="CstPXSegmentMask40" DataField="CheckCashierID" ></px:PXSegmentMask>
			<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit34" DataField="CheckProcessDate" ></px:PXDateTimeEdit>
			<px:PXMaskEdit runat="server" ID="CstPXMaskEdit29" DataField="CheckNbr" ></px:PXMaskEdit>
			<px:PXSelector runat="server" ID="CstPXSelector31" DataField="OriBankCode" ></px:PXSelector>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit30" DataField="OriBankAccount" ></px:PXTextEdit>
			<px:PXDropDown runat="server" ID="CstPXDropDown32" DataField="Status" ></px:PXDropDown>
			<px:PXDateTimeEdit CommitChanges="True" runat="server" ID="CstPXDateTimeEdit36" DataField="CheckDate" ></px:PXDateTimeEdit>
			<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit35" DataField="DueDate" ></px:PXDateTimeEdit>
			<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit33" DataField="EtdDepositDate" ></px:PXDateTimeEdit>
			<px:PXLabel runat="server" ID="CstLabel82" ></px:PXLabel>					
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule38" StartColumn="False" ColumnSpan="2" ></px:PXLayoutRule>
			<px:PXTextEdit TextMode="MultiLine" Height="50px" runat="server" ID="CstPXTextEdit39" DataField="Description" ></px:PXTextEdit>
			<px:PXLayoutRule ControlSize="XM" runat="server" ID="CstPXLayoutRule28" StartColumn="True" ></px:PXLayoutRule>
			<pxa:PXCurrencyRate ID="edCury" runat="server"  DataField="CuryID"  DataMember="_Currency_"  DataSourceID="ds"  RateTypeView="_NMReceivableCheck_CurrencyInfo_" ></pxa:PXCurrencyRate>
			<px:PXNumberEdit CommitChanges="True" runat="server" ID="CstPXNumberEdit44" DataField="OriCuryAmount" ></px:PXNumberEdit>
			<px:PXSegmentMask CommitChanges="True" runat="server" ID="CstPXSegmentMask45" DataField="ProjectID" ></px:PXSegmentMask>
			<px:PXNumberEdit runat="server" ID="CstPXNumberEdit46" DataField="ProjectPeriod" ></px:PXNumberEdit>
			<px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector79" DataField="CustomerID" AllowEdit="True" ></px:PXSelector>
			<px:PXSelector AutoRefresh="True" runat="server" ID="CstPXSelector73" DataField="CustomerLocationID" ></px:PXSelector>
			<px:PXCheckBox CommitChanges="True" runat="server" ID="CstPXCheckBox48" DataField="IsByElse" ></px:PXCheckBox>
			<px:PXTextEdit CommitChanges="True" runat="server" ID="CstPXTextEdit47" DataField="CheckIssuer" ></px:PXTextEdit>
			<px:PXTextEdit Enabled="False" runat="server" ID="CstPXTextEdit71" DataField="ReceiptNbr" >
				<LinkCommand Target="ds" Command="ViewPsPaymentSlip" ></LinkCommand></px:PXTextEdit>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit80" DataField="PayRefNbr" Enabled="False">
				<LinkCommand Target="ds" Command="ViewArPayment" ></LinkCommand></px:PXTextEdit>
			<px:PXTextEdit CommitChanges="True" Enabled="False" runat="server" ID="CstPXTextEdit81" DataField="ARPaymentBatchNbr" >
				<LinkCommand Target="ds" Command="ViewARGL" ></LinkCommand></px:PXTextEdit>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule53" StartRow="True" ></px:PXLayoutRule>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule54" StartColumn="True" ></px:PXLayoutRule>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule57" StartGroup="True" GroupCaption="Collection Information" ></px:PXLayoutRule>
			<px:PXSelector runat="server" ID="CstPXSelector60" DataField="CollBankAccountID" ></px:PXSelector>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit72" DataField="BankAccount" ></px:PXTextEdit>
			<px:PXSegmentMask runat="server" ID="CstPXSegmentMask62" DataField="CollCashierID" ></px:PXSegmentMask>
			<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit61" DataField="CollCheckDate" ></px:PXDateTimeEdit>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit74" DataField="CollBatchNbr" Enabled="False" >
				<LinkCommand Target="ds" Command="ViewCollBatchNbr" ></LinkCommand></px:PXTextEdit>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule55" StartColumn="True" ></px:PXLayoutRule>
			<px:PXLayoutRule GroupCaption="Cash Information" runat="server" ID="CstPXLayoutRule58" StartGroup="True" ></px:PXLayoutRule>
			<px:PXSegmentMask runat="server" ID="CstPXSegmentMask65" DataField="CashCashierID" ></px:PXSegmentMask>
			<px:PXDateTimeEdit Enabled="True" runat="server" ID="CstPXDateTimeEdit64" DataField="DepositDate" ></px:PXDateTimeEdit>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit77" DataField="CashBatchNbr" Enabled="False" >
				<LinkCommand Target="ds" Command="ViewCashBatchNbr" ></LinkCommand></px:PXTextEdit>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule56" StartColumn="True" ></px:PXLayoutRule>
			<px:PXLayoutRule GroupCaption="Modify Information" runat="server" ID="CstPXLayoutRule59" StartGroup="True" ></px:PXLayoutRule>
			<px:PXSegmentMask runat="server" ID="CstPXSegmentMask68" DataField="ModifyCashierID" ></px:PXSegmentMask>
			<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit67" DataField="ReverseDate" ></px:PXDateTimeEdit>
			<px:PXTextEdit Enabled="False" runat="server" ID="CstPXTextEdit75" DataField="CollReverseBatchNbr" >
				<LinkCommand Target="ds" Command="ViewCollReverseBatchNbr" ></LinkCommand></px:PXTextEdit>
			<px:PXTextEdit Enabled="False" runat="server" ID="CstPXTextEdit76" DataField="CashReverseBatchNbr" >
				<LinkCommand Target="ds" Command="ViewCashReverseBatchNbr" ></LinkCommand></px:PXTextEdit>
			<px:PXTextEdit Height="100px" runat="server" ID="CstPXTextEdit66" DataField="Reason" ></px:PXTextEdit></Template>
		<AutoSize Container="Window" Enabled="True" MinHeight="200" ></AutoSize>
	</px:PXFormView>
</asp:Content>