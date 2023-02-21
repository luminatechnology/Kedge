<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="KG304006.aspx.cs" Inherits="Page_KG304006" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormView.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="Kedge.KGSafetyHealthInspectTicketLFileEntry"
        PrimaryView="SafetyTickectFiles"
        >
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="SafetyTickectFiles" Width="100%" AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule runat="server" ID="PXLayoutRule1" StartRow="True" ></px:PXLayoutRule>
			<px:PXLayoutRule LabelsWidth="SM" runat="server" ID="CstPXLayoutRule1" StartColumn="True" ></px:PXLayoutRule>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit1" DataField="CreatedByID_Creator_Username" ></px:PXTextEdit>
			<px:PXSelector runat="server" ID="CstPXSelector5" DataField="CheckItem" />
			<px:PXTextEdit runat="server" ID="CstPXTextEdit3" DataField="ViewCategoryCD" ></px:PXTextEdit>
			<px:PXSelector runat="server" ID="CstPXSelector4" DataField="ReviseCheckItem" AutoRefresh="true"></px:PXSelector>
            <px:PXDropDown runat="server" ID="CstPXDropDown5" DataField="ReviseCategoryCD" ></px:PXDropDown>
            <px:PXCheckBox runat="server" ID="CstPXCheckBox6" DataField="IsDelete"></px:PXCheckBox>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit7" DataField="Remark" TextMode="MultiLine" Height="100px"></px:PXTextEdit>
			<px:PXTextEdit Height="100px" TextMode="MultiLine" runat="server" ID="CstPXTextEdit4" DataField="ImprovementPlan" ></px:PXTextEdit>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule2" StartColumn="True" ></px:PXLayoutRule>
            <px:PXImageUploader runat="server" DataField="ImageUrl"  ShowComment="true"   DataMember="SafetyTickectFiles" AllowNoImage="false" ID ="CstPXImageUploader8" Height="320px" Width="430px"  ></px:PXImageUploader></Template>
		<AutoSize Container="Window" Enabled="True" MinHeight="200" ></AutoSize>
	</px:PXFormView>
</asp:Content>