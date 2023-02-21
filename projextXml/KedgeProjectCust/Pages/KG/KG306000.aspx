<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="KG306000.aspx.cs" Inherits="Page_KG306000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource PageLoadBehavior="PopulateSavedValues"  ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="Kedge.KGOwnerRevenueEntry"
        PrimaryView="FilterView" >
		<CallbackCommands>
                     <px:PXDSCallbackCommand CommitChanges="True" Name="Save" />
                </CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="FilterView" Width="100%" Height="50px" AllowAutoHide="false" >
		<Template>
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartRow="True"></px:PXLayoutRule>
                        <px:PXLayoutRule runat="server" ID="CstPXLayoutRule5" StartColumn="True" />
			<px:PXSegmentMask CommitChanges="True" runat="server" ID="CstPXSegmentMask6" DataField="ProjectID" ></px:PXSegmentMask>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule3" StartColumn="True" ></px:PXLayoutRule>
			<px:PXNumberEdit runat="server" ID="CstPXNumberEdit4" DataField="Total" ></px:PXNumberEdit>
                </Template>		
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXGrid AdjustPageSize="Auto" AllowPaging="True" SyncPosition="" ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="" SkinID="Details" AllowAutoHide="false">
		<Levels>
			<px:PXGridLevel DataMember="OwnerRevenue">
			    <Columns>
				<px:PXGridColumn DataField="ProjectID" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="ProjectID_description" Width="220" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CostCodeID" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CostCodeDesc" Width="280" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Qty" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Uom" Width="72" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Rate" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Amount" Width="100" ></px:PXGridColumn></Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
		<ActionBar >
		</ActionBar>	
		<Mode InitNewRow="True" AllowUpload="True" ></Mode>
		<CallbackCommands>
			<Refresh RepaintControlsIDs="form" ></Refresh></CallbackCommands></px:PXGrid>
</asp:Content>