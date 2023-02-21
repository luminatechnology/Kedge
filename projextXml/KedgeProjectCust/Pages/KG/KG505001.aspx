<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="KG505001.aspx.cs" Inherits="Page_KG505001" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="Kedge.KGVoucherProcess"
        PrimaryView="VoucherFilters"
        >
		<CallbackCommands>
<px:PXDSCallbackCommand CommitChanges="True" Name="voucherPreviewAction" Visible="False" ></px:PXDSCallbackCommand>
<px:PXDSCallbackCommand CommitChanges="True" Name="aPInvoicePreviewAction" Visible="False" ></px:PXDSCallbackCommand>
		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="VoucherFilters" Width="100%" Height="170px" AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartRow="True"></px:PXLayoutRule>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule7" StartColumn="True" ></px:PXLayoutRule>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule17" StartGroup="True" GroupCaption="APInvoice Filter" ></px:PXLayoutRule>
			<px:PXFormView  runat="server" ID="CstFormView18" DataMember="VoucherFilters">
				<Template>
					<px:PXLayoutRule runat="server" ID="CstPXLayoutRule19" StartRow="True" ></px:PXLayoutRule>
					<px:PXLayoutRule runat="server" ID="CstPXLayoutRule20" StartColumn="True" ></px:PXLayoutRule>
					<px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector22" DataField="ContractID" ></px:PXSelector>
					<px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector23" DataField="DocType" ></px:PXSelector>
					<px:PXDateTimeEdit CommitChanges="True" runat="server" ID="CstPXDateTimeEdit24" DataField="DueDateFrom" ></px:PXDateTimeEdit>
					<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit2" DataField="VoucherDateFrom" CommitChanges="True" />
					<px:PXDropDown runat="server" ID="CstPXDropDown1" DataField="VoucherStatus" CommitChanges="True" />
					<px:PXLayoutRule runat="server" ID="CstPXLayoutRule21" StartColumn="True" ></px:PXLayoutRule>
					<px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector26" DataField="RefNbr" ></px:PXSelector>
					<px:PXSegmentMask CommitChanges="True" runat="server" ID="CstPXSegmentMask28" DataField="Vendor" ></px:PXSegmentMask>
					<px:PXDateTimeEdit CommitChanges="True" runat="server" ID="CstPXDateTimeEdit25" DataField="DueDateTo" ></px:PXDateTimeEdit>
					<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit3" DataField="VoucherDateTo" CommitChanges="True" />
					<px:PXSelector runat="server" ID="CstPXSelector36" DataField="PONbr" CommitChanges="True" ></px:PXSelector></Template></px:PXFormView>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule8" StartColumn="True" ></px:PXLayoutRule>
			<px:PXLayoutRule GroupCaption="Data" runat="server" ID="CstPXLayoutRule29" StartGroup="True" ></px:PXLayoutRule>
			<px:PXFormView DataMember="VoucherFilters" runat="server" ID="CstFormView30">
				<Template>
					<px:PXLayoutRule runat="server" ID="CstPXLayoutRule33" StartRow="True" ></px:PXLayoutRule>
					<px:PXLayoutRule runat="server" ID="CstPXLayoutRule34" StartColumn="True" ></px:PXLayoutRule>
					<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit35" DataField="VoucherDate" CommitChanges="True" ></px:PXDateTimeEdit></Template></px:PXFormView></Template>
	
		<AutoSize Enabled="False" ></AutoSize></px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXGrid SyncPosition="True" ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="Details" AllowAutoHide="false">
		<Levels>
			<px:PXGridLevel DataMember="AllVouchers">
			    <Columns>
				<px:PXGridColumn Type="CheckBox" CommitChanges="True" DataField="Selected" Width="60" ></px:PXGridColumn>
				<px:PXGridColumn DataField="APInvoice__ProjectID" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="APInvoice__ProjectID_description" Width="220" ></px:PXGridColumn>
				<px:PXGridColumn DataField="DocType" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="APInvoice__RefNbr" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="APInvoice__InvoiceNbr" Width="180" />
				<px:PXGridColumn DataField="APInvoice__DocDesc" Width="280" />
				<px:PXGridColumn DataField="KGVoucherH__VoucherNo" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="KGVoucherH__VoucherKey" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="KGVoucherH__VoucherDate" Width="90" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Status" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="BAccount__AcctCD" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="BAccount__AcctName" Width="220" ></px:PXGridColumn>
				<px:PXGridColumn DataField="APInvoice__UsrValuationPhase" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="PONbr" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="APInvoice__DueDate" Width="90" ></px:PXGridColumn>
				<px:PXGridColumn DataField="KGBillSummary__TotalAmt" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="KGBillSummary__PrepaymentAmt" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="KGBillSummary__PrepaymentDuctAmt" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="KGBillSummary__PoValuationAmt" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="KGBillSummary__RetentionAmt" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="KGBillSummary__RetentionReturnAmt" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="KGBillSummary__TaxAmt" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="KGBillSummary__DeductionAmt" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="KGBillSummary__AdditionAmt" Width="100" ></px:PXGridColumn></Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
		<ActionBar >
		
			<Actions>
				<Delete Enabled="False" ></Delete>
						<AddNew Enabled="False" ></AddNew></Actions>
			<CustomItems>
                                <px:PXToolBarButton Text="Voucher Preview" >
					<AutoCallBack Command="voucherPreviewAction" Target="ds" ></AutoCallBack>
				</px:PXToolBarButton>
                                <px:PXToolBarButton Text="APInovice Preview" >
					<AutoCallBack Command="aPInvoicePreviewAction" Target="ds" ></AutoCallBack>
				</px:PXToolBarButton></CustomItems>
			</ActionBar>
	</px:PXGrid>
	<px:PXSmartPanel ID="pnMakeSure" runat="server" CaptionVisible="True" Caption="Make Sure"
		Style="position: static" LoadOnDemand="True" Key="VoucherFilters" AutoCallBack-Target="frmMyCommand"
		AutoCallBack-Command="Refresh" DesignView="Content">
		<px:PXFormView ID="frmMyCommand" runat="server" SkinID="Transparent" DataMember="VoucherFilters" DataSourceID="ds" >
			<Template>
				<px:PXLayoutRule runat="server" ControlSize="M" LabelsWidth="M" StartColumn="True" />
				
				<px:PXPanel ID="PXPanel1" runat="server" SkinID="Buttons">
					<px:PXButton ID="btnMyCommandOK" runat="server" DialogResult="OK" Text="OK">
						<AutoCallBack Command="Save" Target="frmMyCommand" />
					</px:PXButton>
					<px:PXButton ID="btnMyCommandCancel" runat="server" DialogResult="Cancel" Text="Cancel" />
				</px:PXPanel>
			</Template>
		</px:PXFormView>
	</px:PXSmartPanel></asp:Content>