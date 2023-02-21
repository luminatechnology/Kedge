<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="GV201001.aspx.cs" Inherits="Page_GV201001" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="RCGV.GV.GVApInvoiceMaint" PrimaryView="Invoice">
        <CallbackCommands>
            <px:PXDSCallbackCommand PopupCommand="voidButton" PopupPanel="pnlVoidInvoice" PostData="Page" PopupCheckSave="True" CommitChanges="True" Name="AddVoid" Visible="True"></px:PXDSCallbackCommand>
        </CallbackCommands>
    </px:PXDataSource>
    <px:PXSmartPanel ID="pnlVoidInvoice" runat="server" AcceptButtonID="PXButtonOK" AutoReload="true" CancelButtonID="PXButtonCancel"
        Caption="Void GVApInvoice" CaptionVisible="True" DesignView="Content" HideAfterAction="false" Key="VoidInvoicePanel"
        LoadOnDemand="true" DefaultControlID="edTextVoidReason">
        <px:PXFormView ID="frmMyCommand" runat="server" SkinID="Transparent" DataMember="VoidInvoicePanel" DataSourceID="ds" EmailingGraph="">
            <ContentStyle BackColor="Transparent" BorderStyle="None">
            </ContentStyle>
            <Template>
                <px:PXLayoutRule runat="server" ControlSize="M" LabelsWidth="M" StartRow="True" ></px:PXLayoutRule>
                <px:PXTextEdit runat="server" ID="edTextVoidReason" CommitChanges="true" DataField="VoidReason" TextMode="MultiLine" Height="60px" LabelWidth="70px" ></px:PXTextEdit>
                <px:PXLayoutRule runat="server" StartRow="True"></px:PXLayoutRule>
                <px:PXPanel ID="PXPanel1" runat="server" SkinID="Buttons">
                    <px:PXButton ID="OK" runat="server" DialogResult="OK" Text="OK"></px:PXButton>
                    <px:PXButton ID="Cancel" runat="server" DialogResult="Cancel" Text="Cancel"></px:PXButton>
                </px:PXPanel>
            </Template>
        </px:PXFormView>
    </px:PXSmartPanel>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100"
        Width="100%" DataMember="Invoice" TabIndex="5300">
        <Template>
            <px:PXLayoutRule runat="server" StartRow="True"></px:PXLayoutRule>
            <px:PXLayoutRule runat="server" ID="CstPXLayoutRule4" StartColumn="True" ColumnWidth="M" LabelsWidth="S"></px:PXLayoutRule>
	<px:PXNumberEdit runat="server" ID="CstPXNumberEdit46" DataField="GuiInvoiceID" ></px:PXNumberEdit>
            <px:PXTextEdit runat="server" ID="CstPXTextEdit16" DataField="GuiInvoiceNbr"></px:PXTextEdit>
            <px:PXDateTimeEdit CommitChanges="True" runat="server" ID="CstPXDateTimeEdit15" DataField="InvoiceDate"></px:PXDateTimeEdit>
            <px:PXNumberEdit runat="server" ID="CstPXNumberEdit14" DataField="DeclareYear"></px:PXNumberEdit>
            <px:PXSegmentMask CommitChanges="True" runat="server" ID="CstPXSegmentMask13" DataField="Vendor"></px:PXSegmentMask>
            <px:PXTextEdit runat="server" ID="CstPXTextEdit12" DataField="VendorUniformNumber"></px:PXTextEdit>
            <px:PXDropDown CommitChanges="True" runat="server" ID="CstPXDropDown11" DataField="TaxCode"></px:PXDropDown>
            <px:PXDropDown runat="server" ID="CstPXDropDown9" DataField="GroupRemark"></px:PXDropDown>
            <px:PXNumberEdit runat="server" ID="CstPXNumberEdit19" DataField="GroupCnt"></px:PXNumberEdit>
            <px:PXLayoutRule runat="server" ID="CstPXLayoutRule17" ColumnSpan="3"></px:PXLayoutRule>
	<px:PXTextEdit runat="server" ID="CstPXTextEdit48" DataField="VendorAddress" ></px:PXTextEdit>
	<px:PXLayoutRule runat="server" ID="CstLayoutRule49" ColumnSpan="3" />
            <px:PXTextEdit runat="server" ID="CstPXTextEdit7" DataField="Remark"></px:PXTextEdit>
            <px:PXLayoutRule runat="server" ID="CstPXLayoutRule5" StartColumn="True" ColumnWidth="M" LabelsWidth="S"></px:PXLayoutRule>
            <px:PXSelector runat="server" ID="CstPXSelector26" DataField="RegistrationCD"></px:PXSelector>
            <px:PXDropDown runat="server" ID="CstPXDropDown25" DataField="GuiType"></px:PXDropDown>
            <px:PXDropDown runat="server" ID="CstPXDropDown44" DataField="DeclareMonth"></px:PXDropDown>
            <px:PXTextEdit runat="server" ID="CstPXTextEdit23" DataField="VendorName"></px:PXTextEdit>
            <px:PXDropDown CommitChanges="True" runat="server" ID="CstPXDropDown10" DataField="VoucherCategory"></px:PXDropDown>
            <px:PXDropDown runat="server" ID="CstPXDropDown20" DataField="DeductionCode"></px:PXDropDown>
            <px:PXDropDown runat="server" ID="CstPXDropDown8" DataField="Status"></px:PXDropDown>
            <px:PXLayoutRule runat="server" ID="CstPXLayoutRule6" StartColumn="True" ColumnWidth="M" LabelsWidth="S"></px:PXLayoutRule>
            <px:PXNumberEdit runat="server" ID="CstPXNumberEdit29" DataField="SalesAmt"></px:PXNumberEdit>
            <px:PXNumberEdit runat="server" ID="CstPXNumberEdit28" DataField="TaxAmt"></px:PXNumberEdit>
            <px:PXNumberEdit runat="server" ID="CstPXNumberEdit27" DataField="TotalAmt"></px:PXNumberEdit></Template>
    </px:PXFormView>
    <px:PXTab Width="100%" runat="server" ID="CstPXTab30">
        <Items>
            <px:PXTabItem Text="Detail">
                <Template>
                    <px:PXGrid Height="230" runat="server" ID="CstPXGrid31" SkinID="Details" Width="100%">
                        <Levels>
                            <px:PXGridLevel DataMember="InvoiceDetails">
                                <Columns>
                                    <px:PXGridColumn CommitChanges="True" DataField="APRefNbr" Width="180"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="ItemDesc" Width="280"></px:PXGridColumn>
                                    <px:PXGridColumn CommitChanges="True" DataField="Qty" Width="100"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="Uom" Width="72"></px:PXGridColumn>
                                    <px:PXGridColumn CommitChanges="True" DataField="UnitPrice" Width="100"></px:PXGridColumn>
                                    <px:PXGridColumn CommitChanges="True" DataField="SalesAmt" Width="100"></px:PXGridColumn>
                                    <px:PXGridColumn CommitChanges="True" DataField="TaxAmt" Width="100"></px:PXGridColumn>
                                </Columns>
                                <RowTemplate>
                                    <px:PXSelector runat="server" ID="T1_APRefNbr" DataField="APRefNbr" AutoRefresh="True" ></px:PXSelector>
                                </RowTemplate>
                            </px:PXGridLevel>
                        </Levels>
                    
	<Mode InitNewRow="True" /></px:PXGrid>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Other Information">
                <Template>
                    <px:PXFormView Width="100%" runat="server" ID="CstFormView32" DataMember="InvoiceInfo">
                        <Template>
                            <px:PXLayoutRule runat="server" ID="CstPXLayoutRule33" StartRow="True"></px:PXLayoutRule>
                            <px:PXLayoutRule runat="server" ID="CstPXLayoutRule34" StartColumn="True" ColumnWidth="M" LabelsWidth="S"></px:PXLayoutRule>
                            <px:PXTextEdit runat="server" ID="CstPXTextEdit37" DataField="EPRefNbr">
	<LinkCommand Command="ViewEPExpenseClaim" Target="ds" ></LinkCommand></px:PXTextEdit>
                            <px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit40" DataField="ConfirmDate"></px:PXDateTimeEdit>
                            <px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit42" DataField="VoidDate"></px:PXDateTimeEdit>
                            <px:PXLayoutRule runat="server" ID="CstPXLayoutRule35" StartColumn="True" ColumnWidth="M" LabelsWidth="S"></px:PXLayoutRule>
	<px:PXTextEdit runat="server" ID="CstPXTextEdit47" DataField="RefNbr">
		<LinkCommand Command="ViewAPInvoice" Target="ds" ></LinkCommand></px:PXTextEdit>
	<px:PXSelector runat="server" ID="CstPXSelector45" DataField="ConfirmPerson" ></px:PXSelector>
                            <px:PXTextEdit runat="server" ID="CstPXTextEdit41" DataField="VoidReason"></px:PXTextEdit>
                            <px:PXLayoutRule runat="server" ID="CstPXLayoutRule36" StartColumn="True" ColumnWidth="M" LabelsWidth="S"></px:PXLayoutRule>
                            <px:PXNumberEdit runat="server" ID="CstPXNumberEdit43" DataField="PrintCnt"></px:PXNumberEdit></Template>
                        <AutoSize Container="Window"></AutoSize>
                        <AutoSize Enabled="True"></AutoSize>
                    </px:PXFormView>
                </Template>
            </px:PXTabItem>
        </Items>
    </px:PXTab>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server"></asp:Content>