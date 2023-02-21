<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="GV302001.aspx.cs" Inherits="Page_GV302001" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
  <px:PXDataSource ToolTip="" ID="ds" runat="server" Visible="True" Width="100%" PrimaryView="gvArGuiInvoices" TypeName="RCGV.GV.GVArInvoiceEntry">
    <CallbackCommands>
      <px:PXDSCallbackCommand PopupCommand="addVoid" PopupPanel="PanelAddVoid" PostData="Page" PopupCheckSave="True" CommitChanges="True" Name="AddVoid" Visible="True" ></px:PXDSCallbackCommand>
	<px:PXDSCallbackCommand CommitChanges="True" Name="AddItem" Visible="False" ></px:PXDSCallbackCommand></CallbackCommands>  
  </px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
  <px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" 
    Width="100%" DataMember="gvArGuiInvoices" TabIndex="6400" CaptionVisible="False">
    <Template>
      <px:PXLayoutRule runat="server" StartColumn="True">
      </px:PXLayoutRule>
	<px:PXSelector Size="SM" runat="server" ID="CstPXSelector2" DataField="GuiInvoiceNbr" ></px:PXSelector>
      <px:PXSelector ID="edRegistrationCD" runat="server" DataField="RegistrationCD" Size="SM" TextAlign="Left" CommitChanges="True">
      </px:PXSelector>
      <px:PXDateTimeEdit ID="edInvoiceDate" runat="server" AlreadyLocalized="False" DataField="InvoiceDate" Size="SM" TextAlign="Left" DefaultLocale="">
      </px:PXDateTimeEdit>
      <px:PXSegmentMask ID="edCustomerID" runat="server" DataField="CustomerID" CommitChanges="True" Size="SM" TextAlign="Left">
      </px:PXSegmentMask>
      <px:PXTextEdit ID="edCustomerAcctName" runat="server" AlreadyLocalized="False" DataField="CustomerAcctName" Size="SM" TextAlign="Left" DefaultLocale="">
      </px:PXTextEdit>
      <px:PXTextEdit ID="edCustUniformNumber" runat="server" AlreadyLocalized="False" DataField="CustUniformNumber" Size="SM" TextAlign="Left" DefaultLocale="">
      </px:PXTextEdit>
      <px:PXLayoutRule runat="server" ColumnSpan="2">
      </px:PXLayoutRule>
      <px:PXTextEdit ID="edCustAddress" runat="server" AlreadyLocalized="False" DataField="CustAddress" TextAlign="Left" DefaultLocale="">
      </px:PXTextEdit>
      <px:PXLayoutRule runat="server" ColumnSpan="3">
      </px:PXLayoutRule>
      <px:PXTextEdit ID="edRemark" runat="server" AlreadyLocalized="False" DataField="Remark" TextAlign="Left" DefaultLocale="">
      </px:PXTextEdit>
      <px:PXLayoutRule runat="server" StartColumn="True">
      </px:PXLayoutRule>
      <px:PXSelector AutoRefresh="True" ID="edGuiBookCD" runat="server" CommitChanges="True" DataField="GuiBookCD" Size="S">
      </px:PXSelector>
      <px:PXSelector ID="edGuiWordCD" runat="server" DataField="GuiWordCD" Size="S">
      </px:PXSelector>
      <px:PXTextEdit Size="S" runat="server" ID="CstPXTextEdit28" DataField="CurrentNum" Enabled="False" ></px:PXTextEdit>
      <px:PXDateTimeEdit Size="S" runat="server" ID="CstPXDateTimeEdit27" DataField="CurrentGuiDate" Enabled="False" ></px:PXDateTimeEdit>
      <px:PXDropDown ID="edStatus" runat="server" DataField="Status" Size="S">
      </px:PXDropDown>
      <px:PXCheckBox ID="edDonateRemark" runat="server" AlreadyLocalized="False" DataField="DonateRemark" Text="DonateRemark" >
      </px:PXCheckBox>
      <px:PXLayoutRule runat="server" StartColumn="True">
      </px:PXLayoutRule>
      <px:PXDropDown CommitChanges="True" ID="edEgvType" runat="server" DataField="EgvType" Size="S">
      </px:PXDropDown>
      <px:PXDropDown ID="edTaxCode" runat="server" DataField="TaxCode" CommitChanges="True" Size="S">
      </px:PXDropDown>
      <px:PXDateTimeEdit ID="edVoidDate" runat="server" AlreadyLocalized="False" DataField="VoidDate" CommitChanges="True" Size="S" DefaultLocale="">
      </px:PXDateTimeEdit>
      <px:PXTextEdit ID="edVoidDesc" runat="server" AlreadyLocalized="False" DataField="VoidDesc" CommitChanges="True" Size="S" TextAlign="Left" DefaultLocale="">
      </px:PXTextEdit>
      <px:PXNumberEdit ID="edSalesAmt" runat="server" AlreadyLocalized="False" DataField="SalesAmt" Size="S" TextAlign="Left" CommitChanges="True" Enabled="False">
      </px:PXNumberEdit>
      <px:PXNumberEdit ID="edTaxAmt" runat="server" AlreadyLocalized="False" DataField="TaxAmt" CommitChanges="True" Enabled="False" Size="S" TextAlign="Left" DefaultLocale="">
      </px:PXNumberEdit>
      <px:PXNumberEdit ID="edTotalAmt" runat="server" AlreadyLocalized="False" CommitChanges="True" DataField="TotalAmt" Enabled="False" Size="S" TextAlign="Left" DefaultLocale="">
      </px:PXNumberEdit></Template>
  </px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
  <px:PXGrid ID="grid" runat="server" DataSourceID="ds" Style="z-index: 100" 
    Width="100%" Height="150px" SkinID="Details" TabIndex="6600">
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
      <px:PXGridLevel DataKeyNames="GuiInvoiceDetailID" DataMember="gvArGuiInvoiceDetails">
        <Columns>
          <px:PXGridColumn DataField="LineNumber" TextAlign="Right" CommitChanges="True">
          </px:PXGridColumn>
          <px:PXGridColumn DataField="ArInvoiceNbr" Width="160px" CommitChanges="True">
          </px:PXGridColumn>
          <px:PXGridColumn DataField="ItemDesc" Width="200px">
          </px:PXGridColumn>
          <px:PXGridColumn DataField="Quantity" TextAlign="Right" Width="100px" CommitChanges="True">
          </px:PXGridColumn>
	<px:PXGridColumn DataField="Unit" Width="70" CommitChanges="True" MatrixMode="True" ></px:PXGridColumn>
          <px:PXGridColumn DataField="UnitPrice" TextAlign="Right" Width="100px" CommitChanges="True">
          </px:PXGridColumn>
          <px:PXGridColumn DataField="SalesAmt" TextAlign="Right" Width="100px" CommitChanges="True">
          </px:PXGridColumn>
          <px:PXGridColumn CommitChanges="True" DataField="TaxAmt" TextAlign="Right" Width="100px">
          </px:PXGridColumn>
          <px:PXGridColumn DataField="Remark" Width="200px">
          </px:PXGridColumn></Columns>
      </px:PXGridLevel>
    </Levels>
    <AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
    <AutoCallBack>
      <Behavior CommitChanges="True" ></Behavior>
    </AutoCallBack>
  
	<ActionBar>
		<CustomItems>
			<px:PXToolBarButton Text="Add Item">
				<AutoCallBack Command="AddItem" Target="ds" /></px:PXToolBarButton></CustomItems></ActionBar></px:PXGrid>
<px:PXSmartPanel ID="PanelAddVoid" runat="server" AcceptButtonID="PXButtonOK" AutoReload="true" CancelButtonID="PXButtonCancel"
          Caption="Void GVArInvoice" CaptionVisible="True" DesignView="Content" HideAfterAction="false" Key="addFilter"
          LoadOnDemand="true" DefaultControlID="CstPXTextEdit8">
		<px:PXFormView ID="CstFormView16" runat="server" SkinID="Transparent" DataMember="addFilter" DataSourceID="ds">
			<ContentStyle BackColor="Transparent" BorderStyle="None"></ContentStyle>
			<Template>
				<px:PXTextEdit runat="server" ID="CstPXTextEdit8" DataField="VoidDesc" CommitChanges="true" TextMode="MultiLine"></px:PXTextEdit></Template></px:PXFormView>
		<px:PXPanel runat="server" ID="PXPanel1" SkinID="Buttons">
			<px:PXButton ID="PXButton1" runat="server" DialogResult="OK" Text="OK" ></px:PXButton>
                  <px:PXButton ID="PXButton2" runat="server" DialogResult="Cancel" Text="Cancel" ></px:PXButton></px:PXPanel></px:PXSmartPanel>
   
<px:PXSmartPanel Caption="Add AR Invoice" ID="PanelAddItem" runat="server" Key="addItemLookup" LoadOnDemand="True" Width="1000px" Height="500px"
       CaptionVisible="True" AutoRepaint="True" DesignView="Content" AlreadyLocalized="False" CreateOnDemand="True" TabIndex="4600" >
    <px:PXFormView DataMember="addItemFilter" runat="server" ID="CstFormView1" DataSourceID="ds" TabIndex="7300" >
      <Template>

        <px:PXLayoutRule runat="server" ControlSize="XM" StartColumn="True">
        </px:PXLayoutRule>
                    <px:PXLayoutRule runat="server" ColumnSpan="2">
              </px:PXLayoutRule>
                    <px:PXSelector runat="server" ID="edCustomertID" DataField="CustomerID"  Enabled ="false" CommitChanges ="true" ></px:PXSelector>
        <px:PXSelector runat="server" ID="edARInvNbrFrom" DataField="ARInvNbrFrom"  CommitChanges ="true"  ></px:PXSelector>
        <px:PXDateTimeEdit runat="server" ID="edARInvDateFrom" DataField="ARInvDateFrom" AlreadyLocalized="False" DefaultLocale="" CommitChanges ="true"  ></px:PXDateTimeEdit>
        
        <px:PXLayoutRule runat="server" StartColumn="True" ControlSize="XM" ></px:PXLayoutRule>
        <px:PXSelector runat="server" ID="edARInvNbrTo" DataField="ARInvNbrTo"  CommitChanges ="true"   ></px:PXSelector>
        <px:PXDateTimeEdit runat="server" ID="edARInvDateTo" DataField="ARInvDateTo" AlreadyLocalized="False" DefaultLocale="" CommitChanges ="true"  ></px:PXDateTimeEdit>
        

      </Template>

    </px:PXFormView>


      <px:PXGrid runat="server" ID="CstPXGrid16" DataSourceID="ds" TabIndex="1400"  Style="border-width: 1px 0px; top: 0px; left: 0px; height: 189px;"
        AutoAdjustColumns="True" Width="100%" SkinID="Inquire" AllowPaging="True" PageSize="10"  AllowSearch="True">
       <CallbackCommands>
                    <Refresh CommitChanges="true"></Refresh>
                </CallbackCommands>
                <ClientEvents AfterCellUpdate="UpdateItemSiteCell" />
                <ActionBar PagerVisible="False">
                    <PagerSettings Mode="NextPrevFirstLast" />
                </ActionBar>
      
      
      <EmptyMsg AnonFilteredAddMessage="No records found.
Try to change filter to see records here." AnonFilteredMessage="No records found.
Try to change filter to see records here." ComboAddMessage="No records found.
Try to change filter or modify parameters above to see records here." FilteredAddMessage="No records found.
Try to change filter to see records here." FilteredMessage="No records found.
Try to change filter to see records here." NamedComboAddMessage="No records found as '{0}'.
Try to change filter or modify parameters above to see records here." NamedComboMessage="No records found as '{0}'.
Try to change filter or modify parameters above to see records here." NamedFilteredAddMessage="No records found as '{0}'.
Try to change filter to see records here." NamedFilteredMessage="No records found as '{0}'.
Try to change filter to see records here." />
      <Levels>
        <px:PXGridLevel DataMember="addItemLookup" DataKeyNames="DocType,RefNbr">
          <Columns>
            <px:PXGridColumn AllowNull="False" DataField="Selected" TextAlign="Center" Type="CheckBox" Width="40px" AutoCallBack="true"
                            AllowCheckAll="true" CommitChanges="true" AllowUpdate="true" />
            <px:PXGridColumn DataField="RefNbr">
            </px:PXGridColumn>
            <px:PXGridColumn DataField="DocDate" Width="90px">
            </px:PXGridColumn>
            <px:PXGridColumn DataField="CustomerID" Width="120px">
            </px:PXGridColumn>
            <px:PXGridColumn DataField="CuryID">
            </px:PXGridColumn>
            <px:PXGridColumn DataField="CuryVatTaxableTotal" TextAlign="Right" Width="100px">
            </px:PXGridColumn>
            <px:PXGridColumn DataField="CuryTaxTotal" TextAlign="Right" Width="100px">
            </px:PXGridColumn>
          </Columns>
          <Mode AllowAddNew="false" AllowDelete="false"  AllowUpdate="false"  AllowUpload="false" />
        </px:PXGridLevel>
      </Levels>
    </px:PXGrid>
    <px:PXPanel ID="PXPanel6" runat="server" SkinID="Buttons">
            <px:PXButton ID="PXButton5" runat="server" CommandName="AddItems" CommandSourceID="ds" Text="Add" SyncVisible="false" />
            <px:PXButton ID="PXButton4" runat="server" Text="Add & Close" DialogResult="OK" />
            <px:PXButton ID="PXButton6" runat="server" DialogResult="Cancel" Text="Cancel" />
        </px:PXPanel>

  </px:PXSmartPanel></asp:Content>