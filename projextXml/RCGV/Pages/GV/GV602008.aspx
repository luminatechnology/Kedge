<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="GV602008.aspx.cs" Inherits="Page_GV602008" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormView.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="RCGV.GV.GVGuiZeroDocDataFileReport" 
       PrimaryView="Filter"  >
		<CallbackCommands>
			<px:PXDSCallbackCommand Visible="True" CommitChanges="True" Name="Exportxt" PopupCommand="exporttxt" ></px:PXDSCallbackCommand>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" DataMember="Filter">
		<Template>
			<px:PXLayoutRule runat="server" StartRow="True"/>
            <px:PXNumberEdit runat="server" ID="CstPXNumberEdit6" DataField="DeclareYear" ></px:PXNumberEdit>
            <px:PXNumberEdit runat="server" ID="CstPXNumberEdit4" DataField="DeclareMonth" ></px:PXNumberEdit>
			
		</Template>
		<AutoSize Container="Window" Enabled="True" MinHeight="200" />
	</px:PXFormView>
</asp:Content>
