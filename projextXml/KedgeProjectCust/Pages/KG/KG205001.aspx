<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="KG205001.aspx.cs" Inherits="Page_KG205001" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="Kedge.KGVoucherMaint"
        PrimaryView="VoucherH"
        >
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="VoucherH" Width="100%" Height="140px" AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartRow="True"></px:PXLayoutRule>
			<px:PXLayoutRule ControlSize="XM" runat="server" ID="CstPXLayoutRule3" StartColumn="True" ></px:PXLayoutRule>
			<px:PXTextEdit CommitChanges="True" Enabled="False" runat="server" ID="RefNbrID" DataField="RefNbr" ></px:PXTextEdit>
			<px:PXDropDown Enabled="False" runat="server" ID="CstPXDropDown15" DataField="VendorType" ></px:PXDropDown>
			<px:PXSelector Enabled="False" runat="server" ID="CstPXSelector21" DataField="VendorCD" ></px:PXSelector>
			<px:PXSelector Enabled="False" runat="server" ID="CstPXSelector20" DataField="ContractCD" ></px:PXSelector>
			<px:PXTextEdit Enabled="False" runat="server" ID="CstPXTextEdit8" DataField="ContractDesc" ></px:PXTextEdit>
			<px:PXLayoutRule ControlSize="S" runat="server" ID="CstPXLayoutRule4" StartColumn="True" ></px:PXLayoutRule>
			<px:PXDateTimeEdit Enabled="False" TimeMode="False" runat="server" ID="CstPXDateTimeEdit19" DataField="VoucherDate" ></px:PXDateTimeEdit>
			<px:PXTextEdit Enabled="False" runat="server" ID="CstPXTextEdit10" DataField="VoucherKey" ></px:PXTextEdit>
			<px:PXTextEdit Enabled="False" runat="server" ID="CstPXTextEdit11" DataField="VoucherNo" ></px:PXTextEdit>
			<px:PXDropDown Enabled="False" runat="server" ID="CstPXDropDown14" DataField="Status" ></px:PXDropDown>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule16" StartColumn="True" ></px:PXLayoutRule>
			<px:PXNumberEdit runat="server" ID="CstPXNumberEdit17" DataField="CAmt" ></px:PXNumberEdit>
			<px:PXNumberEdit runat="server" ID="CstPXNumberEdit18" DataField="DAmt" ></px:PXNumberEdit></Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXGrid ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="Details" AllowAutoHide="false">
		<Levels>
			<px:PXGridLevel DataMember="VoucherL">
			    <Columns>
				<px:PXGridColumn DataField="ItemNo" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="AccountCD" Width="120" ></px:PXGridColumn>
				<px:PXGridColumn DataField="AccountDesc" Width="220" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Cd" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Amt" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="DueDate" Width="90" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Digest" Width="280" ></px:PXGridColumn>
				<px:PXGridColumn DataField="ContractCD" Width="140" />
				<px:PXGridColumn DataField="VendorCD" Width="140" /></Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
		<ActionBar >
		
	    </ActionBar>
	</px:PXGrid>
</asp:Content>