<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="KG505001.aspx.cs" Inherits="Page_KG505001" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="Kedge.KGVoucherProcess"
        PrimaryView="Filters"
        >
		<CallbackCommands>
<px:PXDSCallbackCommand CommitChanges="True" Name="voucherPreviewAction" Visible="False" ></px:PXDSCallbackCommand>
<px:PXDSCallbackCommand CommitChanges="True" Name="aPInvoicePreviewAction" Visible="False" ></px:PXDSCallbackCommand>
<px:PXDSCallbackCommand CommitChanges="True" Name="viewVoucherAction" Visible="False" ></px:PXDSCallbackCommand>
		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="Filters" Width="100%" Height="170px" AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartRow="True"></px:PXLayoutRule>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule7" StartColumn="True" ></px:PXLayoutRule>
			<px:PXPanel RenderStyle="Fieldset" Caption="Filter" runat="server" ID="CstPanel11">
				<px:PXLayoutRule runat="server" ID="CstPXLayoutRule12" StartColumn="True" ></px:PXLayoutRule>
				<px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector14" DataField="ContractID" ></px:PXSelector>
				<px:PXDateTimeEdit CommitChanges="True" runat="server" ID="CstPXDateTimeEdit15" DataField="DueDateFrom" ></px:PXDateTimeEdit>
				<px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector16" DataField="DocType" ></px:PXSelector>
				<px:PXDropDown CommitChanges="True" runat="server" ID="CstPXDropDown17" DataField="UsrIsConfirm" ></px:PXDropDown>
				<px:PXTextEdit runat="server" ID="CstPXTextEdit27" DataField="UsrVoucherNoFrom" CommitChanges="True" />
				<px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector26" DataField="UsrConfirmBy" ></px:PXSelector>
				<px:PXLayoutRule runat="server" ID="CstPXLayoutRule13" StartColumn="True" ></px:PXLayoutRule>
				<px:PXSegmentMask CommitChanges="True" runat="server" ID="CstPXSegmentMask18" DataField="Vendor" ></px:PXSegmentMask>
				<px:PXDateTimeEdit CommitChanges="True" runat="server" ID="CstPXDateTimeEdit19" DataField="DueDateTo" ></px:PXDateTimeEdit>
				<px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector20" DataField="RefNbr" ></px:PXSelector>
				<px:PXSelector runat="server" ID="CstPXSelector25" DataField="PONbr" CommitChanges="True" ></px:PXSelector>
				<px:PXTextEdit runat="server" ID="CstPXTextEdit28" DataField="UsrVoucherNoTo" CommitChanges="True" /></px:PXPanel>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule24" StartColumn="True" />
			<px:PXPanel Caption="Voucher Data" RenderStyle="Fieldset" runat="server" ID="CstPanel21">
				<px:PXLayoutRule runat="server" ID="CstPXLayoutRule22" StartColumn="True" ></px:PXLayoutRule>
				<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit23" DataField="VoucherDate" ></px:PXDateTimeEdit></px:PXPanel></Template>
	
		<AutoSize Enabled="False" ></AutoSize></px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXGrid PageSize="50" SyncPosition="True" ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="Details" AllowAutoHide="false">
		<Levels>
			<px:PXGridLevel DataMember="Details">
			    <Columns>
				<px:PXGridColumn AllowCheckAll="True" Type="CheckBox" CommitChanges="True" DataField="Selected" Width="60" ></px:PXGridColumn>
				<px:PXGridColumn DataField="DocType" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn LinkCommand="ViewAP" DataField="RefNbr" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn LinkCommand="viewEP" CommitChanges="True" DataField="UsrEPRefNbr" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="UsrEPRefNbr_EPExpenseClaim_docDesc" Width="280" />
				<px:PXGridColumn DataField="UsrAccConfirmNbr" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn LinkCommand="ViewAPADR" DataField="UsrDeductionRefNbr" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="DocDate" Width="90" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Status" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn Type="CheckBox" DataField="UsrIsConfirm" Width="60" ></px:PXGridColumn>
				<px:PXGridColumn DataField="UsrConfirmBy" Width="220" ></px:PXGridColumn>
				<px:PXGridColumn DataField="UsrConfirmDate" Width="90" ></px:PXGridColumn>
				<px:PXGridColumn DataField="UsrVoucherNo" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="UsrPONbr" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="UsrOrderDesc" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="VendorID" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="VendorID_BAccountR_acctName" Width="280" ></px:PXGridColumn>
				<px:PXGridColumn DataField="ProjectID" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="ProjectID_description" Width="220" ></px:PXGridColumn>
				<px:PXGridColumn DataField="PayAccountID" Width="120" ></px:PXGridColumn>
				<px:PXGridColumn DataField="PayAccountID_description" Width="220" ></px:PXGridColumn>
				<px:PXGridColumn DataField="PayDate" Width="90" ></px:PXGridColumn>
				<px:PXGridColumn DataField="OrigDocAmt" Width="100" ></px:PXGridColumn></Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
		<ActionBar >
		
			<Actions>
				<Delete Enabled="False" ></Delete>
						<AddNew Enabled="False" ></AddNew></Actions>
			<CustomItems>
                                <px:PXToolBarButton Text="Voucher Preview" >
					<AutoCallBack Command="viewVoucherAction" Target="ds" ></AutoCallBack>
				</px:PXToolBarButton></CustomItems>
			</ActionBar>
	</px:PXGrid>
	<px:PXSmartPanel ID="pnMakeSure" runat="server" CaptionVisible="True" Caption="Make Sure"
		Style="position: static" LoadOnDemand="True" Key="VoucherFilters" AutoCallBack-Target="frmMyCommand"
		AutoCallBack-Command="Refresh" DesignView="Content">
		<px:PXFormView ID="frmMyCommand" runat="server" SkinID="Transparent" DataMember="VoucherFilters" DataSourceID="ds" >
			<Template>
				<px:PXLayoutRule runat="server" ControlSize="M" LabelsWidth="M" StartColumn="True" ></px:PXLayoutRule>
				
				<px:PXPanel ID="PXPanel1" runat="server" SkinID="Buttons">
					<px:PXButton ID="btnMyCommandOK" runat="server" DialogResult="OK" Text="OK">
						<AutoCallBack Command="Save" Target="frmMyCommand" ></AutoCallBack>
					</px:PXButton>
					<px:PXButton ID="btnMyCommandCancel" runat="server" DialogResult="Cancel" Text="Cancel" ></px:PXButton>
				</px:PXPanel>
			</Template>
		</px:PXFormView>
	</px:PXSmartPanel></asp:Content>