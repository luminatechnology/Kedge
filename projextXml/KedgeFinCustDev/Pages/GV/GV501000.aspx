<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="GV501000.aspx.cs" Inherits="Page_GV501000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="GV.GVApGuiInvoiceConfirmProcess"
        PrimaryView="MasterView"
        >
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="MasterView" Width="100%" Height="100px" AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartRow="True"></px:PXLayoutRule>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule1" StartColumn="True" ></px:PXLayoutRule>
			<px:PXDateTimeEdit CommitChanges="True" runat="server" ID="CstPXDateTimeEdit3" DataField="InvoiceDateFrom" ></px:PXDateTimeEdit>
			<px:PXNumberEdit CommitChanges="True" runat="server" ID="CstPXNumberEdit5" DataField="DeclareYear" ></px:PXNumberEdit>
			<px:PXSegmentMask CommitChanges="True" runat="server" ID="CstPXSegmentMask7" DataField="ProjectID" ></px:PXSegmentMask>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule2" StartColumn="True" ></px:PXLayoutRule>
			<px:PXDateTimeEdit CommitChanges="True" runat="server" ID="CstPXDateTimeEdit4" DataField="InvoiceDateTo" ></px:PXDateTimeEdit>
			<px:PXDropDown CommitChanges="True" runat="server" ID="CstPXDropDown6" DataField="DeclareMonth" ></px:PXDropDown>
			<px:PXSegmentMask runat="server" ID="CstPXSegmentMask8" DataField="VendorID" CommitChanges="True" /></Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXGrid AllowSearch="True" SyncPosition="True" ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="Details" AllowAutoHide="false">
		<Levels>
			<px:PXGridLevel DataMember="DetailsView">
			    <Columns>
				<px:PXGridColumn AllowCheckAll="True" Type="CheckBox" DataField="Selected" Width="60" ></px:PXGridColumn>
				<px:PXGridColumn DataField="UsrAccConfirmNbr" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn LinkCommand="ViewAPInvoice" DataField="RefNbr" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn LinkCommand="viewGuiInvoice" DataField="GuiInvoiceNbr" Width="120" ></px:PXGridColumn>
				<px:PXGridColumn DataField="InvoiceDate" Width="90" ></px:PXGridColumn>
				<px:PXGridColumn DataField="TaxCode" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="InvoiceType" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="GuiType" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="SalesAmt" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="TaxAmt" Width="100" ></px:PXGridColumn></Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
		<ActionBar >
		</ActionBar>
	</px:PXGrid>
</asp:Content>