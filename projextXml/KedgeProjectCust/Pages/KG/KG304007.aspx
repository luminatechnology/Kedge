<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="KG304007.aspx.cs" Inherits="Page_KG304007" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormView.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="Kedge.KGMonthlyInspectFileAllEntry"
        PrimaryView="KGMonthlyInspectTicketLFiles"
        >
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="KGMonthlyInspectTicketLFiles" Width="100%"  AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule runat="server" ID="PXLayoutRule1" StartRow="True" ></px:PXLayoutRule>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule1" StartColumn="True" ></px:PXLayoutRule>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit3" DataField="CreatedByID_Creator_Username" ></px:PXTextEdit>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit7" DataField="CheckItem" />
            <px:PXDropDown CommitChanges="True" runat="server" ID="PXDropDown1" DataField="ViewTestResult"></px:PXDropDown>
            <px:PXTextEdit runat="server" ID="CstPXTextEdit4" DataField="Remark" TextMode="MultiLine" Height="100px" ></px:PXTextEdit>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule2" StartColumn="True" ></px:PXLayoutRule>
            <px:PXImageUploader  runat="server" DataField="ImageUrl" TabIndex="1" ShowComment="true"  AllowNoImage="false" ID ="CstPXTextEdit6" Height="320px" Width="430px"  ></px:PXImageUploader>
		</Template>
		<AutoSize Container="Window" Enabled="True" MinHeight="200" ></AutoSize>
	</px:PXFormView>
</asp:Content>

