<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="NM100000.aspx.cs" Inherits="Page_NM100000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormView.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="NM.NMSetting"
        PrimaryView="SetUps"
        >
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="SetUps" Width="100%" AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartRow="True"></px:PXLayoutRule>
			<px:PXLayoutRule ControlSize="L" LabelsWidth="SM" GroupCaption="AR Setting" runat="server" ID="CstPXLayoutRule1" StartGroup="True" ></px:PXLayoutRule>
			<px:PXSelector runat="server" ID="CstPXSelector10" DataField="ARNoteAccountID" ></px:PXSelector>
			<px:PXSelector runat="server" ID="CstPXSelector16" DataField="ARNoteSubID" />
			<px:PXSelector runat="server" ID="CstPXSelector8" DataField="ARComNoteAccountID" ></px:PXSelector>
			<px:PXSelector runat="server" ID="CstPXSelector14" DataField="ARComNoteSubID" />
			<px:PXSelector runat="server" ID="CstPXSelector19" DataField="GuarNoteAccountID" />
			<px:PXSelector runat="server" ID="CstPXSelector17" DataField="GuarNoteSubID" />
			<px:PXSelector runat="server" ID="CstPXSelector9" DataField="ARGuarAccountID" ></px:PXSelector>
			<px:PXSelector runat="server" ID="CstPXSelector15" DataField="ARGuarSubID" />
			<px:PXLayoutRule LabelsWidth="SM" ControlSize="L" GroupCaption="AP Setting" runat="server" ID="CstPXLayoutRule2" StartGroup="True" ></px:PXLayoutRule>
			<px:PXSelector runat="server" ID="CstPXSelector7" DataField="APNoteAccountID" ></px:PXSelector>
			<px:PXSelector runat="server" ID="CstPXSelector13" DataField="APNoteSubID" />
			<px:PXSelector runat="server" ID="CstPXSelector5" DataField="APComNoteAccountID" ></px:PXSelector>
			<px:PXSelector runat="server" ID="CstPXSelector11" DataField="APComNoteSubID" />
			<px:PXSelector runat="server" ID="CstPXSelector20" DataField="GuarTicketAccountID" />
			<px:PXSelector runat="server" ID="CstPXSelector18" DataField="GuarTicketSubID" />
			<px:PXSelector runat="server" ID="CstPXSelector6" DataField="APGuarAccountID" ></px:PXSelector>
			<px:PXSelector runat="server" ID="CstPXSelector12" DataField="APGuarSubID" /></Template>
		<AutoSize Container="Window" Enabled="True" MinHeight="200" ></AutoSize>
	</px:PXFormView>
</asp:Content>