<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="GV502002.aspx.cs" Inherits="Page_GV502002" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="GV.GVArGuiAllowanceInvoiceProcess"
        PrimaryView="MasterView"
        >
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="MasterView" Width="100%" Height="160px" AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartRow="True"></px:PXLayoutRule>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule1" StartColumn="True" ></px:PXLayoutRule>
			<px:PXGroupBox RenderStyle="Fieldset" Caption="Allowance Invoice Info" runat="server" ID="CstGroupBox3" >
				<Template>
					<px:PXLayoutRule runat="server" ID="CstPXLayoutRule14" StartColumn="True" ControlSize="L" ></px:PXLayoutRule>
					<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit17" DataField="InvoiceDate" CommitChanges="True" />
					<px:PXDropDown runat="server" ID="CstPXDropDown15" DataField="GuiType" ></px:PXDropDown>
					<px:PXCheckBox runat="server" ID="CstPXCheckBox16" DataField="AutoConfirm" ></px:PXCheckBox></Template></px:PXGroupBox>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule2" StartColumn="True" ></px:PXLayoutRule>
			<px:PXGroupBox RenderStyle="Fieldset" Caption="Filter" runat="server" ID="CstGroupBox4" >
				<Template>
					<px:PXLayoutRule runat="server" ID="CstPXLayoutRule6" StartColumn="True" ></px:PXLayoutRule>
					<px:PXTextEdit CommitChanges="True" runat="server" ID="CstPXTextEdit10" DataField="RefNbrFrom" ></px:PXTextEdit>
					<px:PXDateTimeEdit CommitChanges="True" runat="server" ID="CstPXDateTimeEdit9" DataField="DocDateFrom" ></px:PXDateTimeEdit>
					<px:PXSegmentMask CommitChanges="True" runat="server" ID="CstPXSegmentMask8" DataField="ProjectID" ></px:PXSegmentMask>
					<px:PXLayoutRule runat="server" ID="CstPXLayoutRule7" StartColumn="True" ></px:PXLayoutRule>
					<px:PXTextEdit CommitChanges="True" runat="server" ID="CstPXTextEdit13" DataField="RefNbrTo" ></px:PXTextEdit>
					<px:PXDateTimeEdit CommitChanges="True" runat="server" ID="CstPXDateTimeEdit12" DataField="DocDateTo" ></px:PXDateTimeEdit>
					<px:PXSegmentMask CommitChanges="True" runat="server" ID="CstPXSegmentMask11" DataField="CustomerID" ></px:PXSegmentMask></Template></px:PXGroupBox></Template>
	
		<AutoSize Container="Parent" ></AutoSize></px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXGrid SyncPosition="True" ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="Details" AllowAutoHide="false">
		<Levels>
			<px:PXGridLevel DataMember="DetailsView">
			    <Columns>
				<px:PXGridColumn Type="CheckBox" AllowCheckAll="True" DataField="Selected" Width="60" ></px:PXGridColumn>
				<px:PXGridColumn DataField="RefNbr" Width="140" LinkCommand="ViewInv" ></px:PXGridColumn>
				<px:PXGridColumn DataField="OrigRefNbr" Width="140" LinkCommand="ViewOriInv" ></px:PXGridColumn>
				<px:PXGridColumn DataField="DocDate" Width="90" ></px:PXGridColumn>
				<px:PXGridColumn DataField="ProjectID" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="ProjectID_description" Width="280" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CustomerID" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CustomerID_description" Width="280" ></px:PXGridColumn>
				<px:PXGridColumn DataField="DocDesc" Width="280" ></px:PXGridColumn>
				<px:PXGridColumn DataField="OrigInvoiceSalesAmt" Width="100" />
				<px:PXGridColumn DataField="CuryVatTaxableTotal" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CuryTaxTotal" Width="100" ></px:PXGridColumn></Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
		<ActionBar >
		
			<Actions>
				<AddNew Enabled="False" /></Actions>
			<Actions>
				<Delete Enabled="False" /></Actions></ActionBar>
	</px:PXGrid>
</asp:Content>