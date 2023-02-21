<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="RC100000.aspx.cs" Inherits="Page_RC100000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="RC.RCFeaturesSetEntry"
        PrimaryView="Setings">
        <CallbackCommands>
        </CallbackCommands>
    </px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="Setings" Width="100%" AllowAutoHide="false">
        <Template>
	<px:PXLayoutRule runat="server" ID="CstPXLayoutRule29" StartRow="True" ></px:PXLayoutRule>
	<px:PXDropDown runat="server" ID="CstPXDropDown30" DataField="Status" ></px:PXDropDown>
	</Template>
        <AutoSize Container="Window" Enabled="True" MinHeight="200" ></AutoSize>
    </px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server"></asp:Content>