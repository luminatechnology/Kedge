<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="NM200001.aspx.cs" Inherits="Page_NM200001" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormView.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource SkinID="Primary" ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="NM.NMCheckBookMaint"
        PrimaryView="CheckBooks"
        >
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phL" runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="CheckBooks" Width="100%" AllowAutoHide="false">
    
	<Template>
		<px:PXLayoutRule runat="server" ID="CstPXLayoutRule2" StartRow="True" ></px:PXLayoutRule>
		<px:PXLayoutRule ColumnWidth="XL" runat="server" ID="CstPXLayoutRule3" StartColumn="True" ></px:PXLayoutRule>
		<px:PXSelector runat="server" ID="CstPXSelector15" DataField="BookCD" CommitChanges="True" ></px:PXSelector>
		<px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector5" DataField="BankAccountID" ></px:PXSelector>
		<px:PXMaskEdit runat="server" ID="CstPXMaskEdit10" DataField="StartCheckNbr" ></px:PXMaskEdit>
		<px:PXDropDown runat="server" ID="CstPXDropDown18" DataField="BookUsage" />
		<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit7" DataField="StartDate" ></px:PXDateTimeEdit>
		<px:PXLayoutRule runat="server" ID="CstPXLayoutRule17" Merge="False" ColumnSpan="2" ></px:PXLayoutRule>
		<px:PXTextEdit runat="server" ID="CstPXTextEdit11" DataField="Description" ></px:PXTextEdit>
		<px:PXLayoutRule ColumnWidth="XL" runat="server" ID="CstPXLayoutRule4" StartColumn="True" ></px:PXLayoutRule>
		<px:PXMaskEdit runat="server" ID="CstPXMaskEdit12" DataField="CheckWord" ></px:PXMaskEdit>
		<px:PXTextEdit runat="server" ID="CstPXTextEdit16" DataField="BankAccount" ></px:PXTextEdit>
		<px:PXMaskEdit runat="server" ID="CstPXMaskEdit9" DataField="EndCheckNbr" ></px:PXMaskEdit>
		<px:PXMaskEdit runat="server" ID="CstPXMaskEdit8" DataField="CurrentCheckNbr" ></px:PXMaskEdit></Template>
	<AutoSize Container="Window" ></AutoSize>
	<AutoSize Enabled="True" ></AutoSize></px:PXFormView></asp:Content>