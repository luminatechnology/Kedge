<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="GV102000.aspx.cs" Inherits="Page_GV102000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/ListView.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True"
        TypeName="RCGV.GV.GVGuiTypeMaint"
        PrimaryView="CodeMaster" SuspendUnloading="False" >
		<CallbackCommands>
            <px:PXDSCallbackCommand Name="Save" CommitChanges="True" ></px:PXDSCallbackCommand>
        </CallbackCommands>
  </px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phL" runat="Server">
	<px:PXGrid ID="grid" runat="server" DataSourceID="ds"  Width ="100%" Height="150px" SkinID="Primary" AllowAutoHide="False" SyncPosition="True" TabIndex="900" MatrixMode="True">
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
      <px:PXGridLevel DataMember="CodeMaster">
          <Columns>
        <px:PXGridColumn DataField="GuiTypeCD" Width="70" CommitChanges="True"></px:PXGridColumn>
        <px:PXGridColumn DataField="GvType" Width="70" CommitChanges="True" ></px:PXGridColumn>
        <px:PXGridColumn DataField="GuiTypeDesc" Width="200" ></px:PXGridColumn>
	<px:PXGridColumn Type="CheckBox" DataField="IsActive" Width="60" ></px:PXGridColumn></Columns>
      
        <RowTemplate>
          <px:PXTextEdit Width="200px" runat="server" ID="CstPXTextEdit3"  DataField ="GuiTypeCD" AlreadyLocalized="False" CommitChanges="True" ></px:PXTextEdit>
          <px:PXDropDown Width="200px" runat="server" ID="CstPXDropDown8" DataField="GvType" CommitChanges="True" ></px:PXDropDown>
           <px:PXTextEdit Width="400px" runat="server" ID="CstPXTextEdit9" DataField="GuiTypeDesc" AlreadyLocalized="False" ></px:PXTextEdit>
          <px:PXDateTimeEdit Width="200px" runat="server" ID="CstPXDateTimeEdit1"   CommitChanges="True" DataField="EffectiveDate" AlreadyLocalized="False" ></px:PXDateTimeEdit>
          <px:PXDateTimeEdit Width="200px" runat="server" ID="CstPXDateTimeEdit2"  CommitChanges="True" DataField="ExpirationDate" AlreadyLocalized="False" ></px:PXDateTimeEdit>
          <px:PXCheckBox runat="server" ID="edIsDeclare" DataField="IsDeclare" AlreadyLocalized="False" Text="Is Declare" ></px:PXCheckBox>
                </RowTemplate></px:PXGridLevel>
    </Levels>
    <AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
    <ActionBar >
    
	<Actions>
		<Upload Enabled="True" ></Upload></Actions></ActionBar>
  
    <Mode AllowUpload="True" AllowFormEdit="True" InitNewRow="true" ></Mode></px:PXGrid>
</asp:Content>