<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="GV107000.aspx.cs" Inherits="Page_GV107000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormView.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="RCGV.GV.GVGuiBookMaint"
        PrimaryView="MasterView"
        >
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="MasterView" Width="100%" AllowAutoHide="False" TabIndex="1500">
		<Template>
			<px:PXLayoutRule ControlSize="XM" LabelsWidth="M" runat="server" StartRow="True" StartColumn="True"></px:PXLayoutRule>
			<px:PXSelector ID="edGuiBookCD" runat="server" CommitChanges="True" DataField="GuiBookCD">
			</px:PXSelector>
			<px:PXSelector ID="edRegistrationCD" runat="server" CommitChanges="True" DataField="RegistrationCD">
			</px:PXSelector>
			<px:PXDropDown ID="edDeclarePeriod" runat="server" DataField="DeclarePeriod">
            </px:PXDropDown>
			<px:PXNumberEdit ID="edDeclareYear" runat="server" AlreadyLocalized="False" CommitChanges="True" DataField="DeclareYear" DefaultLocale="">
			</px:PXNumberEdit>
			<px:PXLayoutRule runat="server" ColumnSpan="2">
			</px:PXLayoutRule>
			<px:PXTextEdit ID="edGuiNumberRange" runat="server" AlreadyLocalized="False" DataField="GuiNumberRange" DefaultLocale="">
			</px:PXTextEdit>
			<px:PXMaskEdit ID="edStartNum" runat="server" AlreadyLocalized="False" DataField="StartNum" DefaultLocale="">
			</px:PXMaskEdit>
			<px:PXNumberEdit ID="edStartMonth" runat="server" AlreadyLocalized="False" CommitChanges="True" DataField="StartMonth" DefaultLocale="">
            </px:PXNumberEdit>
			<px:PXMaskEdit ID="edCurrentNum" runat="server" AlreadyLocalized="False" DataField="CurrentNum" DefaultLocale="">
			</px:PXMaskEdit>
            <px:PXCheckBox CommitChanges="True" runat="server" ID="CstPXCheckBox20" DataField="Hold" AlreadyLocalized="False" ></px:PXCheckBox>
			<px:PXLayoutRule runat="server" ControlSize="XM" LabelsWidth="M" StartColumn="True">
			</px:PXLayoutRule>
			<px:PXTextEdit ID="edGuiBookDesc" runat="server" AlreadyLocalized="False" CommitChanges="True" DataField="GuiBookDesc" DefaultLocale="">
			</px:PXTextEdit>
			<px:PXTextEdit ID="edGovUniformNumber" runat="server" AlreadyLocalized="False" DataField="GovUniformNumber" DefaultLocale="">
			</px:PXTextEdit>
			<px:PXSelector ID="edGuiNumberDetailID" CommitChanges="True" runat="server" DataField="GuiNumberDetailID">
			</px:PXSelector>
			<px:PXDropDown runat="server" ID="CstPXDropDown1" DataField="GuiType" CommitChanges="True" />
			<px:PXMaskEdit ID="edEndNum" runat="server" AlreadyLocalized="False" CommitChanges="True" DataField="EndNum" DefaultLocale="">
			</px:PXMaskEdit>
			<px:PXNumberEdit ID="edEndMonth" runat="server" AlreadyLocalized="False" CommitChanges="True" DataField="EndMonth" DefaultLocale="">
            </px:PXNumberEdit>
			<px:PXDateTimeEdit ID="edCurrentGuiDate" runat="server" AlreadyLocalized="False" CommitChanges="True" DataField="CurrentGuiDate" DefaultLocale="">
			</px:PXDateTimeEdit></Template>
		<AutoSize Container="Window" Enabled="True" MinHeight="200" ></AutoSize>
	</px:PXFormView>
</asp:Content>

