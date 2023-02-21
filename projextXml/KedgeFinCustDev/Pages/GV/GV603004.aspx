<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="GV603004.aspx.cs" Inherits="Page_GV603004" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormView.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="KedgeFinCustDev.FIN.Graph.APARTopNReport"
        PrimaryView="Filter"
        >
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="Filter" Width="100%" AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartRow="True"></px:PXLayoutRule>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule1" StartColumn="True" ></px:PXLayoutRule>
			<px:PXLayoutRule runat="server" ID="CstLayoutRule3" ColumnSpan="2" ></px:PXLayoutRule>
			<px:PXSelector runat="server" ID="CstPXSelector4" DataField="BranchID" ></px:PXSelector>
			<px:PXNumberEdit runat="server" ID="CstPXNumberEdit11" DataField="InvYear" />
			<px:PXLabel runat="server" ID="CstLabel12" />
			<px:PXNumberEdit runat="server" ID="CstPXNumberEdit9" DataField="Top" ></px:PXNumberEdit>
			<px:PXDropDown runat="server" ID="CstPXDropDown10" DataField="Type" ></px:PXDropDown>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule2" StartColumn="True" ></px:PXLayoutRule>
			<px:PXNumberEdit runat="server" ID="CstPXNumberEdit7" DataField="InvMonthFrom" ></px:PXNumberEdit>
			<px:PXNumberEdit runat="server" ID="CstPXNumberEdit8" DataField="InvMonthTo" ></px:PXNumberEdit></Template>
		<AutoSize Container="Window" Enabled="True" MinHeight="200" ></AutoSize>
	</px:PXFormView>
</asp:Content>