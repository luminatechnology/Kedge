<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="PS301010.aspx.cs" Inherits="Page_PS301010" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="PS.PSPaymentSlipEntry"
        PrimaryView="PaymentSlips">
        <CallbackCommands>
        </CallbackCommands>
    </px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="PaymentSlips" Width="100%" Height="" AllowAutoHide="false">
        <Template>
            <px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartRow="True"></px:PXLayoutRule>
            <px:PXLayoutRule LabelsWidth="S" ControlSize="XM" runat="server" ID="CstPXLayoutRule1" StartColumn="True"></px:PXLayoutRule>
	        <px:PXDropDown runat="server" ID="CstPXDropDown64" DataField="DocType" CommitChanges="True" ></px:PXDropDown>
            <px:PXSelector runat="server" ID="CstPXSelector23" DataField="RefNbr"></px:PXSelector>
            <px:PXDropDown runat="server" ID="CstPXDropDown24" DataField="Status"></px:PXDropDown>
            <px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit8" DataField="DocDate"></px:PXDateTimeEdit>
	<px:PXGroupBox RenderStyle="Simple" runat="server" ID="TargetType" CommitChanges="True" DataField="TargetType" >
		<Template>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule71" StartColumn="True" ></px:PXLayoutRule>
			<px:PXRadioButton runat="server" ID="Customer" Value="C" Checked="True" GroupName="TargetType" ></px:PXRadioButton>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule72" StartColumn="True" ></px:PXLayoutRule>
			<px:PXRadioButton runat="server" ID="Vendor" Value="V" Checked="False" GroupName="TargetType" ></px:PXRadioButton></Template></px:PXGroupBox>
            <px:PXSegmentMask CommitChanges="True" runat="server" ID="CstPXSegmentMask29" DataField="CustomerID" ToolTip="Please confirm your payer info already exist in Customer Profile"></px:PXSegmentMask>
            <px:PXSegmentMask runat="server" ID="CstPXSegmentMask30" DataField="CustomerLocationID"></px:PXSegmentMask>
            <px:PXSegmentMask CommitChanges="True" runat="server" ID="CstPXSegmentMask56" DataField="VendorID"></px:PXSegmentMask>
            <px:PXSegmentMask runat="server" ID="CstPXSegmentMask57" DataField="VendorLocationID"></px:PXSegmentMask>
	<px:PXSegmentMask CommitChanges="True" runat="server" ID="CstPXSegmentMask68" DataField="EmployeeID" ></px:PXSegmentMask>
            <px:PXLayoutRule runat="server" ID="CstPXLayoutRule37" StartColumn="False" ColumnSpan="2"></px:PXLayoutRule>
            <px:PXTextEdit CommitChanges="True" TextMode="MultiLine" Height="72px" runat="server" ID="CstPXTextEdit4" DataField="DocDesc"></px:PXTextEdit>
            <px:PXLayoutRule LabelsWidth="S" ControlSize="XM" runat="server" ID="CstPXLayoutRule2" StartColumn="True"></px:PXLayoutRule>
	<px:PXSelector runat="server" ID="CstPXSelector67" DataField="CreatedByID" ></px:PXSelector>
            <px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector28" DataField="DepartmentID"></px:PXSelector>
            <px:PXSegmentMask CommitChanges="True" runat="server" ID="CstPXSegmentMask51" DataField="ContractID"></px:PXSegmentMask>
            <px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit7" DataField="ApproveDate"></px:PXDateTimeEdit>
            <px:PXLayoutRule LabelsWidth="SM" ControlSize="XM" runat="server" ID="CstPXLayoutRule3" StartColumn="True"></px:PXLayoutRule>
            <px:PXNumberEdit runat="server" ID="CstPXNumberEdit55" DataField="DocBal"></px:PXNumberEdit></Template>

        <AutoSize Container="Parent"></AutoSize>
    </px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
    <px:PXTab DataMember="" runat="server" ID="CstPXTab31">
        <Items>
            <px:PXTabItem Text="TabItem1">
                <Template>
                    <px:PXGrid SyncPosition="True" ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="DetailsInTab" AllowAutoHide="false">
                        <Levels>

                            <px:PXGridLevel DataMember="PaymentSlipDetails">
                                <Columns>
                                    <px:PXGridColumn CommitChanges="True" DataField="GuarPayableCD" Width="120" ></px:PXGridColumn>
                                    <px:PXGridColumn CommitChanges="True" DataField="SlipDate" Width="90"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="PaymentRefNbr" Width="140"></px:PXGridColumn>
                                    <px:PXGridColumn MatrixMode="True" CommitChanges="True" DataField="PaymentMethodID" Width="70"></px:PXGridColumn>
                                    <px:PXGridColumn LinkCommand="viewInventoryItem" Type="NotSet" CommitChanges="True" DataField="InventoryID" Width="70"></px:PXGridColumn>
                                    <px:PXGridColumn CommitChanges="True" DataField="TranDesc" Width="280"></px:PXGridColumn>
                                    <px:PXGridColumn CommitChanges="True" DataField="Qty" Width="100"></px:PXGridColumn>
                                    <px:PXGridColumn CommitChanges="True" DataField="Uom" Width="72"></px:PXGridColumn>
                                    <px:PXGridColumn CommitChanges="True" DataField="UnitCost" Width="100"></px:PXGridColumn>
	<px:PXGridColumn DataField="CCPostageAmt" Width="100" CommitChanges="True" />
                                    <px:PXGridColumn CommitChanges="True" DataField="TranAmt" Width="100"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="DueDate" Width="90" ></px:PXGridColumn>
                                    <px:PXGridColumn DataField="GuarClass" Width="70"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="GuarType" Width="70"></px:PXGridColumn>
                                    <px:PXGridColumn CommitChanges="True" DataField="PONbr" Width="140"></px:PXGridColumn>
	<px:PXGridColumn CommitChanges="True" DataField="EmployeeID" Width="140" ></px:PXGridColumn>
	<px:PXGridColumn DataField="EmployeeID_description" Width="220" ></px:PXGridColumn>
	<px:PXGridColumn DataField="CheckIssuer" Width="280" ></px:PXGridColumn>
	<px:PXGridColumn DataField="OriBankCode" Width="70" ></px:PXGridColumn>
	<px:PXGridColumn DataField="OriBankAccount" Width="140" ></px:PXGridColumn>
                                    <px:PXGridColumn DataField="IssueDate" Width="90"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="AuthDate" Width="90"></px:PXGridColumn></Columns>
                                <RowTemplate>
                                    <px:PXSelector runat="server" ID="T1_PONbr" DataField="PONbr" AutoRefresh="True" ></px:PXSelector>
                                    <px:PXSelector runat="server" ID="T1_GuarPayableCD" DataField="GuarPayableCD" AutoRefresh="True" ></px:PXSelector>
                                </RowTemplate>
                            </px:PXGridLevel>
                        </Levels>
                        <AutoSize Container="Window" Enabled="True" MinHeight="150"></AutoSize>
                        <ActionBar>
                        </ActionBar>

                        <CallbackCommands>
                            <Refresh RepaintControls="All"></Refresh>
                        </CallbackCommands>
                        <CallbackCommands>
                            <Save PostData="Container"></Save>
                        </CallbackCommands>
                        <Mode InitNewRow="True" AllowFormEdit="False" AllowUpload="True" ></Mode>
                    </px:PXGrid>
                </Template>

            </px:PXTabItem>
            <px:PXTabItem Text="TabItem2">
                <Template>
                    <px:PXGrid MatrixMode="True" SyncPosition="True" Width="100%" Height="150px" runat="server" ID="CstPXGrid32" SkinID="DetailsInTab">
                        <Levels>
                            <px:PXGridLevel DataMember="PaymentSlipDetails">
                                <Columns>
                                    <px:PXGridColumn CommitChanges="True" DataField="PaymentCategory" Width="70"></px:PXGridColumn>
                                    <px:PXGridColumn CommitChanges="True" DataField="BankAccountID" Width="70"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="ActualDueDate" Width="90"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="CheckDueDate" Width="90"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="EtdDepositDate" Width="90"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="ProcessDate" Width="90"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="ArPaymentRefNbr" Width="100" LinkCommand="viewArPayment"></px:PXGridColumn>
	<px:PXGridColumn DataField="GuarReceviableCD" Width="100" LinkCommand="viewCCReceivable" ></px:PXGridColumn></Columns>
                                <RowTemplate>
                                    <px:PXSelector runat="server" ID="T2_BankAccountID" DataField="BankAccountID" AutoRefresh="True" ></px:PXSelector>
                                </RowTemplate>
                            </px:PXGridLevel>
                        </Levels>
                        <AutoSize Enabled="True" MinHeight="150" Container="Window" ></AutoSize>
                    
	<ActionBar>
		<Actions>
			<AddNew Enabled="False" ></AddNew>
			<Delete Enabled="False" ></Delete></Actions></ActionBar></px:PXGrid>
                </Template>
            </px:PXTabItem>
        </Items>
    </px:PXTab>
</asp:Content>