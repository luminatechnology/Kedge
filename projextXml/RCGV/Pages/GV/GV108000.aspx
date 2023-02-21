<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="GV108000.aspx.cs" Inherits="Page_GV108000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True"
        TypeName="RCGV.GV.GVLookupCodeMaint"
        PrimaryView="CodeMaster" SuspendUnloading="False"
        >
  </px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="CodeMaster" Width="100%" Height="100px" AllowAutoHide="false">
    <Template>
      <px:PXLayoutRule LabelsWidth="L" ID="PXLayoutRule1" runat="server" StartRow="True"></px:PXLayoutRule>
      <px:PXSelector runat="server" ID="CstPXSelector3" DataField="LookupCodeType" />
      <px:PXTextEdit runat="server" ID="CstPXTextEdit4" DataField="LookupCodeTypeDesc" /></Template>
  </px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
    <px:PXGrid SyncPosition="True" MarkRequired="Dynamic" ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="Details">
    <Levels>
      <px:PXGridLevel DataMember="CodeDetails">
          <Columns>
        <px:PXGridColumn DataField="LookupCode" Width="200" CommitChanges="True"></px:PXGridColumn>
        <px:PXGridColumn DataField="LookupCodeValue" Width="200" ></px:PXGridColumn>
        <px:PXGridColumn DataField="IsActive" Width="100" CommitChanges="True" AllowNull="False" Type="CheckBox" ></px:PXGridColumn>
       </Columns>
      
        <RowTemplate>
		  <px:PXTextEdit Width="200px" Size="" runat="server" ID="CstPXTextEdit7" DataField="LookupCode" CommitChanges="True"></px:PXTextEdit>
          <px:PXTextEdit Width="200px" Size="" runat="server" ID="CstPXTextEdit9" DataField="LookupCodeValue" ></px:PXTextEdit>
          <px:PXCheckBox CommitChanges="True" SuppressLabel="True" ID="chkHold" runat="server" DataField="IsActive" />
        </RowTemplate>

      </px:PXGridLevel>
    </Levels>
    <AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
    <Mode AllowFormEdit="True" AllowUpload="True"></Mode></px:PXGrid>
</asp:Content>
