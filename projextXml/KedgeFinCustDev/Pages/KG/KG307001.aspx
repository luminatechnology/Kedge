<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="KG307001.aspx.cs" Inherits="Page_KG307001" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormView.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="Kedge.KGBudApproveNameEntry"
        PrimaryView="BudApproveNames"
        >
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="BudApproveNames" Width="100%" AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartRow="True"></px:PXLayoutRule>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule2" StartColumn="True" ></px:PXLayoutRule>
			<px:PXSelector runat="server" ID="CstPXSelector91" DataField="Branch" CommitChanges="True" ></px:PXSelector>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule1" StartRow="True" ></px:PXLayoutRule>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule42" StartColumn="True" ></px:PXLayoutRule>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit93" DataField="Stage1Name" ></px:PXTextEdit>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit94" DataField="Stage2Name" ></px:PXTextEdit>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit95" DataField="Stage3Name" ></px:PXTextEdit>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit96" DataField="Stage4Name" ></px:PXTextEdit>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit97" DataField="Stage5Name" ></px:PXTextEdit>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit98" DataField="Stage6Name" ></px:PXTextEdit>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit99" DataField="Stage7Name" ></px:PXTextEdit>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit100" DataField="Stage8Name" ></px:PXTextEdit>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit101" DataField="Stage9Name" ></px:PXTextEdit>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit102" DataField="Stage10Name" ></px:PXTextEdit>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule92" StartColumn="True" ></px:PXLayoutRule>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit104" DataField="Stage11Name" ></px:PXTextEdit>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit105" DataField="Stage12Name" ></px:PXTextEdit>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit106" DataField="Stage13Name" ></px:PXTextEdit>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit107" DataField="Stage14Name" ></px:PXTextEdit>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit108" DataField="Stage15Name" ></px:PXTextEdit>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit109" DataField="Stage16Name" ></px:PXTextEdit>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit110" DataField="Stage17Name" ></px:PXTextEdit>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit113" DataField="Stage18Name" />
			<px:PXTextEdit runat="server" ID="CstPXTextEdit111" DataField="Stage19Name" ></px:PXTextEdit>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit112" DataField="Stage20Name" ></px:PXTextEdit></Template>
		<AutoSize Container="Window" Enabled="True" MinHeight="200" ></AutoSize>
	</px:PXFormView>
</asp:Content>