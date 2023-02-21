<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="GV301001.aspx.cs" Inherits="Page_GV301001" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">

	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" PrimaryView="Invoice" TypeName="RCGV.GV.GVApInvoiceEntry">
		<CallbackCommands>
			 <px:PXDSCallbackCommand PopupCommand="addVoid" PopupPanel="PanelAddVoid" PostData="Page" PopupCheckSave="True" CommitChanges="True" Name="AddVoid" Visible="True" ></px:PXDSCallbackCommand>

			<px:PXDSCallbackCommand CommitChanges="true" Name="AddItem" Visible="False" />
		</CallbackCommands>
	</px:PXDataSource>
    <px:PXSmartPanel ID="pnlVoidInvoice" runat="server" AcceptButtonID="PXButtonOK" AutoReload="true" CancelButtonID="PXButtonCancel"
          Caption="Void GVApInvoice" CaptionVisible="True" DesignView="Content" HideAfterAction="false" Key="VoidInvoicePanel"
          LoadOnDemand="true" DefaultControlID="edTextVoidReason">
		<px:PXFormView ID="frmMyCommand" runat="server" SkinID="Transparent" DataMember="VoidInvoicePanel"	DataSourceID="ds" EmailingGraph="">
			<ContentStyle BackColor="Transparent" BorderStyle="None">
            </ContentStyle>
            <Template>
				<px:PXLayoutRule runat="server" ControlSize="M" LabelsWidth="M" StartColumn="True" />
				<px:PXTextEdit runat="server" ID="edTextVoidReason" CommitChanges="true" DataField="VoidReason" TextMode="MultiLine" Height="60px" LabelWidth="70px" />
				<px:PXLayoutRule runat="server" StartRow="True" ></px:PXLayoutRule>
				<px:PXPanel ID="PXPanel1" runat="server" SkinID="Buttons">
                  <px:PXButton ID="OK" runat="server" DialogResult="OK" Text="OK" ></px:PXButton>
                  <px:PXButton ID="Cancel" runat="server" DialogResult="Cancel" Text="Cancel" ></px:PXButton>
              </px:PXPanel>
			</Template>
		</px:PXFormView>
    </px:PXSmartPanel>
</asp:Content>

<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" 
		Width="100%" DataMember="Invoice" TabIndex="5300">
		<Template>
			<px:PXLayoutRule runat="server" ControlSize="M" LabelsWidth="S" StartColumn="True" StartRow="True">
			</px:PXLayoutRule>
			<px:PXSelector ID="edGuiInvoiceNbr" runat="server" CommitChanges="True" DataField="GuiInvoiceNbr">
			</px:PXSelector>
			<px:PXSelector ID="edRegistrationCD" runat="server" CommitChanges="True" DataField="RegistrationCD">
			</px:PXSelector>
			<px:PXDropDown ID="edTaxCode" runat="server" CommitChanges="True" DataField="TaxCode">
			</px:PXDropDown>
			<px:PXDropDown ID="edGuiType" runat="server" DataField="GuiType">
            </px:PXDropDown>
			<px:PXDateTimeEdit ID="edVoidDate" runat="server" AlreadyLocalized="False" DataField="VoidDate">
			</px:PXDateTimeEdit>

			
            <px:PXNumberEdit ID="edSalesAmt" runat="server" AlreadyLocalized="False" DataField="SalesAmt">
			</px:PXNumberEdit>
			<px:PXLayoutRule runat="server" ColumnSpan="2">
			</px:PXLayoutRule>
			<px:PXTextEdit ID="edRemark" runat="server" CommitChanges="True" AlreadyLocalized="False" DataField="Remark">
			</px:PXTextEdit>
			<px:PXLayoutRule runat="server" ControlSize="M" LabelsWidth="SM" StartColumn="True">
			</px:PXLayoutRule>
			<px:PXSegmentMask ID="edVendor" runat="server" DataField="Vendor" CommitChanges="True">
            </px:PXSegmentMask>
			<px:PXDateTimeEdit ID="edInvoiceDate" runat="server" CommitChanges="True" AlreadyLocalized="False" DataField="InvoiceDate">
			</px:PXDateTimeEdit>
			<px:PXDropDown ID="edDeductionCode" runat="server" DataField="DeductionCode">
			</px:PXDropDown>
			<px:PXNumberEdit ID="edGroupCnt" runat="server" CommitChanges="True" AlreadyLocalized="False" DataField="GroupCnt">
			</px:PXNumberEdit>

			<px:PXTextEdit ID="edVoidReason" runat="server" AlreadyLocalized="False" DataField="VoidReason">
			</px:PXTextEdit>

			<px:PXNumberEdit ID="edTaxAmt" runat="server" AlreadyLocalized="False" DataField="TaxAmt">
			</px:PXNumberEdit>
			<px:PXLayoutRule runat="server" ControlSize="M" LabelsWidth="S" StartColumn="True">
			</px:PXLayoutRule>
			<px:PXTextEdit ID="edVendorUniformNumber" runat="server" AlreadyLocalized="False" DataField="VendorUniformNumber">
			</px:PXTextEdit>
			<px:PXDropDown ID="edVoucherCategory" runat="server" DataField="VoucherCategory">
			</px:PXDropDown>
            <px:PXDropDown runat="server" ID="CstPXDropDown1" DataField="InvoiceType" />
			<px:PXDropDown ID="edGroupRemark" runat="server" DataField="GroupRemark">
			</px:PXDropDown>
			<px:PXNumberEdit ID="edPrintCnt" runat="server" AlreadyLocalized="False" DataField="PrintCnt">
			</px:PXNumberEdit>
			<px:PXNumberEdit ID="edTotalAmt" runat="server" AlreadyLocalized="False" DataField="TotalAmt" DefaultLocale="">
            </px:PXNumberEdit>
			<px:PXDropDown ID="edStatus" runat="server" CommitChanges="True" DataField="Status">
			</px:PXDropDown>
			
		</Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXGrid ID="grid" runat="server" DataSourceID="ds" Style="z-index: 100" 
		Width="100%" Height="150px" SkinID="Details" TabIndex="300">
<EmptyMsg ComboAddMessage="No records found.
Try to change filter or modify parameters above to see records here." NamedComboMessage="No records found as &#39;{0}&#39;.
Try to change filter or modify parameters above to see records here." NamedComboAddMessage="No records found as &#39;{0}&#39;.
Try to change filter or modify parameters above to see records here." FilteredMessage="No records found.
Try to change filter to see records here." FilteredAddMessage="No records found.
Try to change filter to see records here." NamedFilteredMessage="No records found as &#39;{0}&#39;.
Try to change filter to see records here." NamedFilteredAddMessage="No records found as &#39;{0}&#39;.
Try to change filter to see records here." AnonFilteredMessage="No records found.
Try to change filter to see records here." AnonFilteredAddMessage="No records found.
Try to change filter to see records here."></EmptyMsg>
		<Levels>
			<px:PXGridLevel DataMember="InvoiceDetails">
				<RowTemplate>
					<px:PXLayoutRule runat="server" ControlSize="L" LabelsWidth="M" StartRow="True">
					</px:PXLayoutRule>
					<px:PXNumberEdit ID="edLineNumber" CommitChanges="true" runat="server" AlreadyLocalized="False" DataField="LineNumber" DefaultLocale="">
                    </px:PXNumberEdit>
					<px:PXTextEdit ID="edApInvoiceNbr" runat="server" CommitChanges="true" AlreadyLocalized="False" DataField="ApInvoiceNbr">
					</px:PXTextEdit>
                    <px:PXSegmentMask ID="edVendor" runat="server" DataField="Vendor" CommitChanges="True"> </px:PXSegmentMask>
					<px:PXTextEdit ID="edItemDesc" runat="server" AlreadyLocalized="False" DataField="ItemDesc" DefaultLocale="">
                    </px:PXTextEdit>
					<px:PXNumberEdit ID="edQuantity" runat="server" CommitChanges="True" AlreadyLocalized="False" DataField="Quantity" DefaultLocale="">
					</px:PXNumberEdit>
					<px:PXTextEdit ID="edUnit" runat="server" CommitChanges="true" AlreadyLocalized="False" DataField="Unit" DefaultLocale="">
					</px:PXTextEdit>
					<px:PXNumberEdit ID="edUnitPrice" runat="server" CommitChanges="True" AlreadyLocalized="False" DataField="UnitPrice" DefaultLocale="">
					</px:PXNumberEdit>
					<px:PXNumberEdit ID="edSalesAmt" runat="server" AlreadyLocalized="False" DataField="SalesAmt" DefaultLocale="">
					</px:PXNumberEdit>
					<px:PXNumberEdit ID="edTaxAmt" runat="server"  CommitChanges="True"  AlreadyLocalized="False" DataField="TaxAmt" DefaultLocale="">
					</px:PXNumberEdit>
					<px:PXNumberEdit ID="edRemark" runat="server" AlreadyLocalized="False" DataField="Remark" DefaultLocale="">
					</px:PXNumberEdit>
				</RowTemplate>
				<Columns>
					<px:PXGridColumn DataField="LineNumber" CommitChanges="True" TextAlign="Right">
					</px:PXGridColumn>
					<px:PXGridColumn DataField="APInvoiceNbr" CommitChanges="true" Width="160px">
					</px:PXGridColumn>
                    
                    <px:PXGridColumn DataField="Vendor"  Width="160px">
					</px:PXGridColumn>

					<px:PXGridColumn DataField="ItemDesc" Width="200px">
					</px:PXGridColumn>
					<px:PXGridColumn DataField="Quantity" CommitChanges="True" TextAlign="Right" Width="100px">
					</px:PXGridColumn>
					<px:PXGridColumn DataField="Unit" CommitChanges="true">
					</px:PXGridColumn>
					<px:PXGridColumn DataField="UnitPrice" CommitChanges="True" TextAlign="Right" Width="100px">
					</px:PXGridColumn>
					<px:PXGridColumn DataField="SalesAmt" TextAlign="Right" Width="100px">
					</px:PXGridColumn>
					<px:PXGridColumn DataField="TaxAmt"  CommitChanges="True"  TextAlign="Right" Width="100px">
					</px:PXGridColumn>
				    <px:PXGridColumn DataField="Remark" TextAlign="Right" Width="100px">
                    </px:PXGridColumn>
				</Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" />
        <ActionBar>
			<CustomItems>
				<px:PXToolBarButton Text="Add Item" CommandSourceID="ds" CommandName="AddItem" Enabled="true"></px:PXToolBarButton>
			</CustomItems>
		</ActionBar>
		<Mode AllowFormEdit="True" AllowAddNew="True" AllowDelete="True" />
	</px:PXGrid>
</asp:Content>

<asp:Content ID="Content1" runat="server" contentplaceholderid="phDialogs">
		<px:PXSmartPanel ID="pnlAddItemMasterView" runat="server" AcceptButtonID="PXButtonOK" AutoReload="true" 
		CancelButtonID="PXButtonCancel" Caption="Add AP Invoic" CaptionVisible="True" 
		DesignView="Content" HideAfterAction="false" Key="AddItemMasterView"
  LoadOnDemand="true" >
		<px:PXFormView DataMember="AddItemMasterView" runat="server" ID="CstFormView2" DataSourceID="ds" TabIndex="3900" Width="100%" >
			<Template>
                <px:PXLayoutRule runat="server" StartRow="True" StartColumn="True"/>
                <px:PXLayoutRule runat="server" ColumnSpan="2">
			    </px:PXLayoutRule>
                <px:PXSegmentMask ID="edVendorID" StartColumn="True" ControlSize="M" LabelsWidth="S" runat="server" DataField="VendorID" CommitChanges="True">
				</px:PXSegmentMask>
                <px:PXSelector ID="edAPInvNbrFrom" runat="server" CommitChanges="True" DataField="APInvNbrFrom">
				</px:PXSelector>
				<px:PXDateTimeEdit ID="edAPInvDateFrom" runat="server" AlreadyLocalized="False" CommitChanges="True" DataField="APInvDateFrom">
				</px:PXDateTimeEdit>
				<px:PXLayoutRule runat="server" StartColumn="True" ControlSize="M" LabelsWidth="S">
				</px:PXLayoutRule>
				<px:PXSelector ID="edAPInvNbrTo" runat="server" CommitChanges="True" DataField="APInvNbrTo">
				</px:PXSelector>
				<px:PXDateTimeEdit runat="server" ID="edAPInvDateTo" DataField="APInvDateTo" AlreadyLocalized="False" CommitChanges="True" ></px:PXDateTimeEdit>
				</Template></px:PXFormView>
		<px:PXGrid NoteIndicator="False" ExportNotes="False" FilesIndicator="False" runat="server" ID="CstPXGrid15" DataSourceID="ds" Height="150px" TabIndex="300">
			<EmptyMsg AnonFilteredAddMessage="No records found.
Try to change filter to see records here." AnonFilteredMessage="No records found.
Try to change filter to see records here." ComboAddMessage="No records found.
Try to change filter or modify parameters above to see records here." FilteredAddMessage="No records found.
Try to change filter to see records here." FilteredMessage="No records found.
Try to change filter to see records here." NamedComboAddMessage="No records found as '{0}'.
Try to change filter or modify parameters above to see records here." NamedComboMessage="No records found as '{0}'.
Try to change filter or modify parameters above to see records here." NamedFilteredAddMessage="No records found as '{0}'.
Try to change filter to see records here." NamedFilteredMessage="No records found as '{0}'.
Try to change filter to see records here." ></EmptyMsg>
			<Levels>
				<px:PXGridLevel DataMember="AddItemDetailView">
					<RowTemplate>
						<px:PXCheckBox ID="edSelected" runat="server" AlreadyLocalized="False" DataField="Selected" Text="Selected">
						</px:PXCheckBox>
						<px:PXDropDown ID="edDocType" runat="server" DataField="DocType">
						</px:PXDropDown>
						<px:PXSelector ID="edRefNbr" runat="server" DataField="RefNbr">
						</px:PXSelector>
					</RowTemplate>
					<Columns>
						<px:PXGridColumn  AllowUpdate="true" AutoCallBack="true" DataField="Selected" TextAlign="Center" Type="CheckBox" Width="60px" CommitChanges="True" ></px:PXGridColumn>
						<px:PXGridColumn DataField="RefNbr">
						</px:PXGridColumn>
						<px:PXGridColumn DataField="APInvoice__DocDate" Width="90" />
						<px:PXGridColumn DataField="VendorID_Vendor_acctName" Width="200px">
						</px:PXGridColumn>
						<px:PXGridColumn DataField="CuryID">
						</px:PXGridColumn>
						<px:PXGridColumn DataField="APInvoice__CuryTaxTotal" Width="100"  ></px:PXGridColumn>
						<px:PXGridColumn DataField="APInvoice__CuryVatTaxableTotal" Width="100" ></px:PXGridColumn></Columns>
				</px:PXGridLevel>
			</Levels>
			<AutoSize Enabled="True" ></AutoSize>
		
			<Mode AllowUpload="False" AllowDelete="False" ></Mode></px:PXGrid>
		<px:PXPanel ContentLayout-ContentAlign="Right" runat="server" ID="Buttons">
			<px:PXButton ID="PXButton1" runat="server" CommandName="AddItemOK" CommandSourceID="ds" Text="Add" SyncVisible="false" ></px:PXButton>
			<px:PXButton runat="server" ID="CstButton4" DialogResult="OK" Text="Add &amp; Close" ></px:PXButton>
			<px:PXButton runat="server" ID="CstButton5" DialogResult="Cancel" Text="Cancel" ></px:PXButton></px:PXPanel></px:PXSmartPanel>
</asp:Content>