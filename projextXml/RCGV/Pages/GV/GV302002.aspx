<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormTab.master" AutoEventWireup="true"
    ValidateRequest="false" CodeFile="GV302002.aspx.cs" Inherits="Page_GV302002" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormTab.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" PrimaryView="gvArGuiInvoices" TypeName="RCGV.GV.GVArGuiCmInvoiceMaint">
        <CallbackCommands>
            <px:PXDSCallbackCommand PopupCommand="addVoid" PopupPanel="PanelAddVoid" PostData="Page" PopupCheckSave="True" CommitChanges="True" Name="AddVoid" Visible="True"></px:PXDSCallbackCommand>
	<px:PXDSCallbackCommand Visible="False" Name="selectGuiInvoiceItem" ></px:PXDSCallbackCommand>
	<px:PXDSCallbackCommand Visible="False" Name="AddGuiInvoice" ></px:PXDSCallbackCommand></CallbackCommands>
    </px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
    <px:PXFormView ID="PXFormView1" runat="server" DataSourceID="ds" Style="z-index: 100"
        Width="100%" DataMember="gvArGuiInvoices" TabIndex="1100">
        <Template>
            <px:PXLayoutRule runat="server" StartRow="True"></px:PXLayoutRule>
            <px:PXLayoutRule runat="server" ID="CstPXLayoutRule1" StartColumn="True"></px:PXLayoutRule>
            <px:PXSelector runat="server" ID="CstPXSelector9" DataField="GuiCmInvoiceNbr"></px:PXSelector>
            <px:PXDateTimeEdit CommitChanges="True" runat="server" ID="CstPXDateTimeEdit8" DataField="InvoiceDate"></px:PXDateTimeEdit>
	<px:PXNumberEdit runat="server" ID="CstPXNumberEdit41" DataField="DeclareYear" />
            <px:PXSegmentMask CommitChanges="True" runat="server" ID="CstPXSegmentMask7" DataField="CustomerID"></px:PXSegmentMask>
            <px:PXTextEdit CommitChanges="True" runat="server" ID="CstPXTextEdit6" DataField="CustUniformNumber"></px:PXTextEdit>
            <px:PXDropDown runat="server" ID="CstPXDropDown5" DataField="TaxCode"></px:PXDropDown>
            <px:PXSelector AutoRefresh="True" runat="server" ID="CstPXSelector31" DataField="BatchNbr"></px:PXSelector>
            <px:PXLayoutRule runat="server" ID="CstLayoutRule29" ColumnSpan="3"></px:PXLayoutRule>
            <px:PXTextEdit runat="server" ID="CstPXTextEdit4" DataField="Remark"></px:PXTextEdit>
            <px:PXLayoutRule ColumnWidth="L" LabelsWidth="S" runat="server" ID="CstPXLayoutRule2" StartColumn="True"></px:PXLayoutRule>
            <px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector14" DataField="RegistrationCD"></px:PXSelector>
            <px:PXDropDown runat="server" ID="CstPXDropDown13" DataField="GuiType"></px:PXDropDown>
	<px:PXNumberEdit runat="server" ID="CstPXNumberEdit42" DataField="DeclareMonth" />
            <px:PXTextEdit runat="server" ID="CstPXTextEdit12" DataField="CustName"></px:PXTextEdit>
            <px:PXDropDown runat="server" ID="CstPXDropDown11" DataField="Status"></px:PXDropDown>
            <px:PXCheckBox CommitChanges="True" runat="server" ID="CstPXCheckBox10" DataField="Hold"></px:PXCheckBox>
            <px:PXLayoutRule runat="server" ID="CstPXLayoutRule3" StartColumn="True"></px:PXLayoutRule>
            <px:PXNumberEdit runat="server" ID="CstPXNumberEdit16" DataField="SalesAmt"></px:PXNumberEdit>
            <px:PXNumberEdit runat="server" ID="CstPXNumberEdit17" DataField="TaxAmt"></px:PXNumberEdit>
            <px:PXNumberEdit runat="server" ID="CstPXNumberEdit15" DataField="TotalAmt"></px:PXNumberEdit></Template>
    </px:PXFormView>
    <px:PXTab runat="server" ID="CstPXTab18" Width="100%">
        <Items>
            <px:PXTabItem Text="Gui Invoice">
                <Template>
                    <px:PXGrid SkinID="Details" runat="server" ID="CstPXGrid19" SyncPosition="True" Width="100%">
                        <Levels>
                            <px:PXGridLevel DataMember="gvArGuiInvoiceLines">
                                <Columns>
                                    <px:PXGridColumn DataField="ArGuiInvoiceNbr" Width="180"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="InvSalesAmt" Width="100"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="Balance" Width="100"></px:PXGridColumn>
                                    <px:PXGridColumn CommitChanges="True" DataField="SalesAmt" Width="100"></px:PXGridColumn>
                                    <px:PXGridColumn CommitChanges="True" DataField="TaxAmt" Width="100"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="ItemDesc" Width="280"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="BatchNbr" Width="70"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="RefNbr" Width="70"></px:PXGridColumn>
                                </Columns>
                            </px:PXGridLevel>
                        </Levels>
                        <Mode InitNewRow="True"></Mode>
                        <AutoSize Enabled="True" Container="Window"></AutoSize>
                        <ActionBar>
                            <CustomItems>
                                <px:PXToolBarButton Key="cmSelectGuiInvoiceItem" DisplayStyle="Text">
                                    <AutoCallBack Target="ds" Command="SelectGuiInvoiceItem" ></AutoCallBack>
                                </px:PXToolBarButton>
                            </CustomItems>

                        
	<Actions>
		<AddNew Enabled="False" /></Actions></ActionBar>
                    </px:PXGrid>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Other Information">
                <Template>
                    <px:PXFormView Width="100%" runat="server" ID="CstFormView20" DataMember="gvArGuiInvoices">
                        <Template>
                            <px:PXLayoutRule runat="server" ID="CstPXLayoutRule21" StartRow="True"></px:PXLayoutRule>
                            <px:PXLayoutRule runat="server" ID="CstPXLayoutRule22" StartColumn="True"></px:PXLayoutRule>
                            <px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit25" DataField="ConfirmDate"></px:PXDateTimeEdit>
                            <px:PXTextEdit runat="server" ID="CstPXTextEdit24" DataField="ConfirmBy"></px:PXTextEdit>
                            <px:PXLayoutRule runat="server" ID="CstPXLayoutRule23" StartColumn="True"></px:PXLayoutRule>
                            <px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit28" DataField="VoidDate"></px:PXDateTimeEdit>
                            <px:PXTextEdit runat="server" ID="CstPXTextEdit27" DataField="VoidBy"></px:PXTextEdit>
                            <px:PXTextEdit runat="server" ID="CstPXTextEdit26" DataField="VoidReason"></px:PXTextEdit>
                        </Template>
                        <AutoSize Enabled="True" Container="Window"></AutoSize>
                    </px:PXFormView>
                </Template>
            </px:PXTabItem>
        </Items>
        <AutoSize Container="Window"></AutoSize>
    </px:PXTab>

    <px:PXSmartPanel runat="server" ID="PanelAddVoid" AutoCallBack-Behavior-CommitChanges="True" AutoCallBack-Behavior-PostData="Page" AlreadyLocalized="False" DesignView="Content" Height="100px" Width="100px" CreateOnDemand="True" AutoRepaint="True" CaptionVisible="True" Caption="Void Description" Key="addFilter" LoadOnDemand="True">
        <px:PXFormView runat="server" ID="CstFormView16" DataMember="addFilter" DataSourceID="ds">
            <Template>
                <px:PXTextEdit runat="server" ID="CstPXTextEdit26" DataField="VoidDesc" CommitChanges="True" TextMode="MultiLine" Height="60px" LabelWidth="70px"></px:PXTextEdit>
            </Template>
        </px:PXFormView>
        <px:PXPanel runat="server" ID="CstPanel10" SkinID="Buttons">
            <px:PXButton runat="server" ID="CstButton24" Text="OK" CommandName="Addok" SyncVisible="false" CommandSourceID="ds" DialogResult="OK"></px:PXButton>
        </px:PXPanel>
    </px:PXSmartPanel>

    <px:PXSmartPanel AutoReload="True" AutoRepaint="True" CancelButtonID="PXButtonCancel" Key="GvArInvoiceItems" CaptionVisible="True" Height="396px" Width="910px" Caption="GV Ar Gui Invoice Item" runat="server" ID="CstSmartPanel35">
        <px:PXGrid Height="240px" SyncPosition="True" NoteIndicator="False" FilesIndicator="False" SkinID="Inquire" Width="100%" DataSourceID="ds" runat="server" ID="CstPXGrid37">
            <Levels>
                <px:PXGridLevel DataMember="GvArInvoiceItems">
                    <Columns>
                        <px:PXGridColumn CommitChanges="True" Type="CheckBox" AllowCheckAll="True" DataField="Selected" Width="60"></px:PXGridColumn>
                        <px:PXGridColumn DataField="GuiInvoiceNbr" Width="70" ></px:PXGridColumn>
                        <px:PXGridColumn DataField="InvoiceType" Width="70" ></px:PXGridColumn>
                        <px:PXGridColumn DataField="Remark" Width="70" ></px:PXGridColumn>
                        <px:PXGridColumn DataField="SalesAmt" Width="100" ></px:PXGridColumn>
                        <px:PXGridColumn DataField="TaxAmt" Width="100" ></px:PXGridColumn>
                        <px:PXGridColumn DataField="ItemDesc" Width="70" ></px:PXGridColumn>
                        <px:PXGridColumn DataField="RefNbr" Width="70" ></px:PXGridColumn>
                        <px:PXGridColumn DataField="BatchNbr" Width="70" ></px:PXGridColumn>
                    </Columns>
                </px:PXGridLevel>
            </Levels>
        
	<AutoSize Enabled="True" ></AutoSize>
	<Mode AllowAddNew="False" />
	<Mode AllowUpdate="False" />
	<Mode AllowDelete="False" /></px:PXGrid>
        <px:PXPanel runat="server" ID="CstPanel36">
	<px:PXButton runat="server" ID="CstButton40" Text="Add" >
		<AutoCallBack Command="AddGuiInvoice" />
		<AutoCallBack Target="ds" /></px:PXButton>
            <px:PXButton runat="server" ID="PXButtonAddAndClose" Text="Add &amp; Close" CommandName="AddAndClose" SyncVisible="false" CommandSourceID="ds" DialogResult="OK"></px:PXButton>
            <px:PXButton runat="server" ID="PXButtonCancel" Text="Cancel" CommandName="Close" SyncVisible="false" CommandSourceID="ds" DialogResult="Cancel"></px:PXButton></px:PXPanel>
    </px:PXSmartPanel>
</asp:Content>
