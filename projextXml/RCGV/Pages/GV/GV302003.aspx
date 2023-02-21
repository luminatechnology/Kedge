<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="GV302003.aspx.cs" Inherits="Page_GV302003" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
  <px:PXDataSource ID="ds" runat="server" Visible="True" PrimaryView="gvArGuiZeroDocs" SuspendUnloading="False" TypeName="RCGV.GV.GVArGuiZeroDocEntry" >
  </px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
  <px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" 
    Width="100%" DataMember="gvArGuiZeroDocs" TabIndex="1100">
    <Template>
      <px:PXLayoutRule runat="server" StartRow="True" ControlSize="SM" StartColumn="True"></px:PXLayoutRule>
	<px:PXLayoutRule ControlSize="SM" runat="server" ID="CstPXLayoutRule2" StartColumn="True" ></px:PXLayoutRule>
      <px:PXSelector AutoRefresh="True" CommitChanges="True" Size="SM" ID="edZeroDocCD" runat="server" DataField="ZeroDocCD">
      </px:PXSelector>
      <px:PXTextEdit CommitChanges="True" Size="SM" ID="edDocNbr" runat="server" AlreadyLocalized="False" DataField="DocNbr">
      </px:PXTextEdit>
      <px:PXNumberEdit Size="SM" ID="edDeclareYear" runat="server" AlreadyLocalized="False" DataField="DeclareYear">
      </px:PXNumberEdit>
      <px:PXSelector Size="SM" ID="edCustomerID" runat="server" DataField="CustomerID" CommitChanges="True">
      </px:PXSelector>
      <px:PXNumberEdit Size="SM" ID="edQuantity" runat="server" AlreadyLocalized="False" DataField="Quantity">
      </px:PXNumberEdit>
      <px:PXNumberEdit Size="SM" ID="edOriSalesAmt" runat="server" AlreadyLocalized="False" DataField="OriSalesAmt">
      </px:PXNumberEdit>
      <px:PXLayoutRule runat="server" ControlSize="SM" StartColumn="True">
      </px:PXLayoutRule>
      <px:PXSelector CommitChanges="True" Size="SM" ID="edRegistrationCD" runat="server" DataField="RegistrationCD">
      </px:PXSelector>
      <px:PXDateTimeEdit Size="SM" ID="edDocDate" runat="server" AlreadyLocalized="False" DataField="DocDate">
      </px:PXDateTimeEdit>
      <px:PXNumberEdit Size="SM" ID="edDeclareMonth" runat="server" AlreadyLocalized="False" DataField="DeclareMonth">
      </px:PXNumberEdit>
      <px:PXTextEdit Size="SM" ID="edCustUniformNumber" runat="server" AlreadyLocalized="False" DataField="CustUniformNumber" CommitChanges="True">
      </px:PXTextEdit>
      <px:PXNumberEdit Size="SM" ID="edSalesAmt" runat="server" AlreadyLocalized="False" DataField="SalesAmt">
      </px:PXNumberEdit>
	<px:PXLayoutRule runat="server" ID="CstPXLayoutRule1" ColumnSpan="2" StartColumn="False" />
	<px:PXTextEdit runat="server" ID="CstPXTextEdit1" DataField="Remark" ></px:PXTextEdit>
      <px:PXLayoutRule runat="server" StartColumn="True" ControlSize="SM">
      </px:PXLayoutRule>
      <px:PXDropDown Size="SM" ID="edStatus" runat="server" AlreadyLocalized="False" DataField="Status" Enabled="False">
      </px:PXDropDown>
      <px:PXDropDown Size="SM" ID="edDocType" runat="server" DataField="DocType">
      </px:PXDropDown>
      <px:PXDropDown Size="SM" ID="edSalesType" runat="server" DataField="SalesType">
      </px:PXDropDown>
      <px:PXDropDown Size="SM" ID="edDataType" runat="server" DataField="DataType">
      </px:PXDropDown>
      <px:PXTextEdit Size="SM" ID="edOriCurrency" runat="server" AlreadyLocalized="False" DataField="OriCurrency">
      </px:PXTextEdit></Template>
  </px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
  <px:PXGrid SyncPosition="True" ID="grid" runat="server" DataSourceID="ds" Style="z-index: 100" 
    Width="100%" Height="150px" SkinID="Details" TabIndex="900">
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
      <px:PXGridLevel DataMember="gvArGuiZeroDocLines">
        <Columns>
	<px:PXGridColumn CommitChanges="True" DataField="GuiInvoiceNbr" Width="160" ></px:PXGridColumn>
          <px:PXGridColumn DataField="InvoiceDate" Width="90px">
          </px:PXGridColumn>
          <px:PXGridColumn DataField="Customer" Width="200px">
          </px:PXGridColumn>
          <px:PXGridColumn DataField="SalesAmt" TextAlign="Right" Width="100px">
          </px:PXGridColumn></Columns>
      
        <RowTemplate></RowTemplate></px:PXGridLevel>
    </Levels>
    <AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
  </px:PXGrid>
</asp:Content>