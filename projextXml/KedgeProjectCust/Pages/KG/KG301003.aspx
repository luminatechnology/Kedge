<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="KG301003.aspx.cs" Inherits="Page_KG301003" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="Kedge.KGDailyRenterEntry"
        PrimaryView="DailyRenterM">
	<CallbackCommands>
        </CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView SyncPosition="True" AllowCollapse="True" ID="form" runat="server" DataSourceID="ds" DataMember="DailyRenterM" Width="100%" Height="" AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartRow="True"></px:PXLayoutRule>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule1" StartColumn="True" ></px:PXLayoutRule>
			<px:PXSelector Size="M" runat="server" ID="CstPXSelector6" DataField="DailyRenterCD" ></px:PXSelector>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule2" StartRow="True" ></px:PXLayoutRule>
			<px:PXLayoutRule ControlSize="M" runat="server" ID="CstPXLayoutRule3" StartColumn="True" ></px:PXLayoutRule>
			<px:PXSelector Size="" Width="" CommitChanges="True" runat="server" ID="CstPXSelector5" DataField="ContractID" ></px:PXSelector>
			<px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector7" DataField="OrderNbr" ></px:PXSelector>
			<px:PXSelector runat="server" ID="CstPXSelector8" DataField="LineNbr" ></px:PXSelector>
			<px:PXNumberEdit runat="server" ID="CstPXNumberEdit9" DataField="ReqQty" ></px:PXNumberEdit>
			<px:PXNumberEdit runat="server" ID="CstPXNumberEdit10" DataField="ReqSelfQty" ></px:PXNumberEdit>
			<px:PXNumberEdit runat="server" ID="CstPXNumberEdit11" DataField="ReqInsteadQty" ></px:PXNumberEdit>
			<px:PXSelector runat="server" ID="CstPXSelector12" DataField="CreatedByID" ></px:PXSelector>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule4" StartColumn="True" ></px:PXLayoutRule>
			<px:PXDateTimeEdit CommitChanges="True" runat="server" ID="CstPXDateTimeEdit13" DataField="WorkDate" ></px:PXDateTimeEdit>
			<px:PXSelector Enabled="False" runat="server" ID="CstPXSelector14" DataField="VendorID" ></px:PXSelector>
			<px:PXDropDown Enabled="False" runat="server" ID="CstPXDropDown15" DataField="Status" ></px:PXDropDown>
			<px:PXCheckBox CommitChanges="True" runat="server" ID="CstPXCheckBox16" DataField="Hold" ></px:PXCheckBox>
			<px:PXTextEdit TextMode="MultiLine" Height="100px" runat="server" ID="CstPXTextEdit17" DataField="ReqWorkContent" ></px:PXTextEdit></Template>	
		<AutoSize Container="Window" ></AutoSize>
		<AutoSize Enabled="True" ></AutoSize></px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXGrid SyncPosition="True" ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="" SkinID="Details" AllowAutoHide="false">
		<Levels>
			<px:PXGridLevel DataMember="DailyRenterVendorM">
			    <Columns>
				<px:PXGridColumn DataField="VendorID" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="InsteadQty" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="WorkContent" Width="70" ></px:PXGridColumn></Columns>
			
				<RowTemplate>
					<px:PXSelector runat="server" ID="CstPXSelector19" DataField="VendorID" DataKeyNames="Vendor" /></RowTemplate></px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True"></AutoSize>
		<ActionBar >
		</ActionBar>
	</px:PXGrid>
</asp:Content>