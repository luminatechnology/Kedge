<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="KG306001.aspx.cs" Inherits="Page_KG306001" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
  <px:PXDataSource PageLoadBehavior="PopulateSavedValues" ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="Kedge.KGOwnerRevenueAllocEntry" PrimaryView="FilterView" >
    <CallbackCommands>
    </CallbackCommands> 
    <DataTrees>
      <px:PXTreeDataMember TreeView="Tasks" TreeKeys="TaskID" ></px:PXTreeDataMember></DataTrees></px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
  <px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="FilterView" Width="100%" Height="50px" AllowAutoHide="false">
    <Template>
	<px:PXLayoutRule runat="server" ID="PXLayoutRule1" StartRow="True" ></px:PXLayoutRule>
	<px:PXLayoutRule runat="server" ID="CstPXLayoutRule1" StartColumn="True" ControlSize="M" ></px:PXLayoutRule>
	<px:PXSegmentMask runat="server" ID="CstPXSegmentMask4" DataField="ProjectID" CommitChanges="True" ></px:PXSegmentMask>
	<px:PXLayoutRule runat="server" ID="CstPXLayoutRule2" StartColumn="True" LabelsWidth="SM" ></px:PXLayoutRule>
	<px:PXNumberEdit runat="server" DataField="TotalAmtByProject" ID="CstPXNumberEdit5" ></px:PXNumberEdit>
	<px:PXLayoutRule runat="server" ID="CstPXLayoutRule3" StartColumn="True" LabelsWidth="SM" ></px:PXLayoutRule>
	<px:PXNumberEdit runat="server" DataField="TotalAmtByTask" ID="CstPXNumberEdit6" ></px:PXNumberEdit>
    </Template>
  </px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
  <px:PXSplitContainer runat="server" ID="sp2" SplitterPosition="300">
    <Template1>
      <px:PXTreeView Caption="Project Tasks" runat="server" ID="tree" DataSourceID="ds" Height="500px" PopulateOnDemand="True" AllowCollapse="true" ExpandDepth="1" AutoRepaint="true" ShowRootNode="False" SyncPosition="True">
        <AutoSize Enabled="True" MinHeight="300"></AutoSize>
        <AutoCallBack Target="CurrentTask" ActiveBehavior="true" Command="Refresh">
          <Behavior RepaintControlsIDs="form,grid" ></Behavior></AutoCallBack>
        <DataBindings>
          <px:PXTreeItemBinding DataMember="Tasks" TextField="Description" ValueField="TaskID" ImageUrlField="Icon" ></px:PXTreeItemBinding></DataBindings>
        <ToolBarItems>
          <px:PXToolBarButton Tooltip="Reload Tree" ImageKey="Refresh">
            <AutoCallBack Command="Refresh" Target="tree" ></AutoCallBack></px:PXToolBarButton></ToolBarItems></px:PXTreeView>
    </Template1>
    <Template2>
      <px:PXGrid runat="server" ID="grid" SyncPosition="true" Height="400px" SkinID="Details" Width="100%" Caption="Revenue Allocation"
 CaptionVisible="True" AutoAdjustColumns="False" 
DataSourceID="ds" AllowPaging="True" ActionsPosition="None" MatrixMode="true">
        <AutoSize Enabled="True" Container="Parent" MinHeight="200" ></AutoSize>
        <ActionBar>
          <Actions>
            <ExportExcel Enabled="True" ></ExportExcel></Actions>
          <CustomItems>
            <px:PXToolBarButton Tooltip="Delete node" ImageKey="Remove">
              <AutoCallBack Command="Delete" Target="ds" ></AutoCallBack>
              <ActionBar GroupIndex="0" Order="5" ></ActionBar></px:PXToolBarButton>
	<px:PXToolBarButton Text="Add Owner Revenue" AlreadyLocalized="False">
		<AutoCallBack Target="ds" Command="AddOwnerReve" ></AutoCallBack></px:PXToolBarButton></CustomItems></ActionBar>
        <AutoCallBack Target="formPanel" ActiveBehavior="True" Command="Refresh">
          <Behavior CommitChanges="True" RepaintControlsIDs="formPanel" BlockPage="True" ></Behavior></AutoCallBack>
          <CallbackCommands>
                    <Refresh CommitChanges="True" PostData="Page" RepaintControls="All" ></Refresh>
                </CallbackCommands>
        <Levels>
          <px:PXGridLevel DataMember="OwnerRevenueAlloc">
            <Columns>
              <px:PXGridColumn DataField="ProjectID" Width="70" ></px:PXGridColumn>
              <px:PXGridColumn DataField="ProjectID_description" Width="220" ></px:PXGridColumn>
              <px:PXGridColumn DataField="CostCodeID" Width="100" ></px:PXGridColumn>
	      <px:PXGridColumn DataField="CostCodeDesc" Width="280" ></px:PXGridColumn>
	      <px:PXGridColumn DataField="TaskID" Width="100" ></px:PXGridColumn>
	      <px:PXGridColumn DataField="TaskID_description" Width="280" ></px:PXGridColumn>
              <px:PXGridColumn DataField="Qty" Width="100" ></px:PXGridColumn>
              <px:PXGridColumn DataField="Uom" Width="72" ></px:PXGridColumn>
              <px:PXGridColumn DataField="Rate" Width="100" ></px:PXGridColumn>
              <px:PXGridColumn DataField="Amount" Width="100" ></px:PXGridColumn></Columns>
</px:PXGridLevel></Levels>
        <Parameters>
          <px:PXControlParam ControlID="tree" PropertyName="SelectedValue" Name="TaskID" ></px:PXControlParam></Parameters>
	<Mode AllowUpload="True" /></px:PXGrid></Template2>
<AutoSize Container="Window" Enabled="True" ></AutoSize>
</px:PXSplitContainer>
	<px:PXSmartPanel CaptionVisible="True" runat="server" ID="CstSmartPanel6" Caption="Add Owner Revenue" Height="400px" Key="OwnerRevenue" LoadOnDemand="True" Width="900px" AutoRepaint="True">
		<px:PXGrid DataSourceID="ds"  AdjustPageSize="None" AllowPaging="True" Height="" Width="100%" SkinID="Inquire" PageSize="40" SyncPosition="True" runat="server" ID="CstPXGrid7">
			<Levels>
				<px:PXGridLevel DataMember="OwnerRevenue" >
					<Columns>
						<px:PXGridColumn AllowCheckAll="True" Type="CheckBox" DataField="Selected" Width="60" ></px:PXGridColumn>
						<px:PXGridColumn DataField="ProjectID" Width="70" ></px:PXGridColumn>
						<px:PXGridColumn DataField="CostCodeID" Width="100" ></px:PXGridColumn>
						<px:PXGridColumn DataField="CostCodeDesc" Width="280" />
						<px:PXGridColumn DataField="Qty" Width="100" ></px:PXGridColumn>
						<px:PXGridColumn DataField="RemainQty" Width="100" ></px:PXGridColumn>
						<px:PXGridColumn DataField="Rate" Width="100" ></px:PXGridColumn>
						<px:PXGridColumn DataField="Uom" Width="72" ></px:PXGridColumn>
						<px:PXGridColumn DataField="Amount" Width="100" ></px:PXGridColumn></Columns></px:PXGridLevel></Levels>
			<AutoSize Container="Parent" Enabled="True" ></AutoSize>
			<Mode AllowUpdate="False" AllowAddNew="False" ></Mode></px:PXGrid>
		<px:PXPanel SkinID="Buttons" runat="server" ID="CstPanel8">
			<px:PXButton Text="Add" CommandSourceID="ds" CommandName="Add" runat="server" ID="CstButton9" ></px:PXButton>
			<px:PXButton DialogResult="OK" Text="Add &amp; Close" runat="server" ID="CstButton10" ></px:PXButton>
			<px:PXButton Text="Close" DialogResult="Cancel" runat="server" ID="CstButton11" ></px:PXButton></px:PXPanel></px:PXSmartPanel></asp:Content>