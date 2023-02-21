<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="KG300001.aspx.cs" Inherits="Page_KG300001" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/ListView.master" %>



<asp:Content ID="Content1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True"
        TypeName="Kedge.KGVendorPriceEntry"
        PrimaryView="VendorPrices"  >
		<CallbackCommands>
            <px:PXDSCallbackCommand Name="Save" CommitChanges="True" ></px:PXDSCallbackCommand>
			<px:PXDSCallbackCommand Visible="True" Name="processDelete" CommitChanges="True" ></px:PXDSCallbackCommand></CallbackCommands>
  </px:PXDataSource>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="phL" runat="Server">
	<px:PXGrid ID="PXGrid1" runat="server" DataSourceID="ds" AllowSearch="true"  AllowPaging="true" PageSize="30" Width ="100%" Height="150px" SkinID="Primary" AllowAutoHide="False" SyncPosition="True" TabIndex="900" MatrixMode="True">
    <Levels>
      <px:PXGridLevel DataMember="VendorPrices">
	<Columns>
		<px:PXGridColumn Type="CheckBox" CommitChanges="True" DataField="Selected" Width="60" ></px:PXGridColumn>
		<px:PXGridColumn DataField="InventoryID" Width="70" ></px:PXGridColumn>
      
		<px:PXGridColumn DataField="AreaCode" Width="70" ></px:PXGridColumn>
		<px:PXGridColumn DataField="VendorID" Width="70" ></px:PXGridColumn>
		<px:PXGridColumn DataField="VendorID_description" Width="220" ></px:PXGridColumn>
		<px:PXGridColumn DataField="ContactName" Width="250" ></px:PXGridColumn>
		<px:PXGridColumn DataField="ContactPhone" Width="220" ></px:PXGridColumn>
		<px:PXGridColumn DataField="EffectiveDate" Width="90" ></px:PXGridColumn>
		<px:PXGridColumn DataField="ExpirationDate" Width="90" ></px:PXGridColumn>
		<px:PXGridColumn DataField="Item" Width="280" ></px:PXGridColumn>
        <px:PXGridColumn DataField="Uom" Width="70" ></px:PXGridColumn>
		<px:PXGridColumn DataField="UnitPrice" Width="70" ></px:PXGridColumn>
		<px:PXGridColumn DataField="Remark" Width="280" ></px:PXGridColumn></Columns></px:PXGridLevel>
    </Levels>
    <AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
    <ActionBar DefaultAction="viewDetails" PagerVisible="Bottom"><PagerSettings Mode="NumericCompact" />
    
	
	<Actions>
		<Delete ToolBarVisible="False" ></Delete></Actions></ActionBar>
  
    <Mode AllowDelete="False" InitNewRow="True" AllowFormEdit="true"   AllowUpload  ="true" ></Mode></px:PXGrid>
</asp:Content>