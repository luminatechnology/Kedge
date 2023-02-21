<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="KG104001.aspx.cs" Inherits="Page_KG104001" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="Kedge.KGSelfInspectionTemplate"
        PrimaryView="TemplateH"
        >
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="TemplateH" Width="100%" Height="" AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartRow="True"></px:PXLayoutRule>
			<px:PXLayoutRule ControlSize="M" LabelsWidth="M" runat="server" ID="CstPXLayoutRule3" StartColumn="True" ></px:PXLayoutRule>
			<px:PXSelector runat="server" ID="CstPXSelector7" DataField="TemplateCD" ></px:PXSelector>
			<px:PXSegmentMask CommitChanges="True" runat="server" ID="CstPXSegmentMask9" DataField="SegmentCD" ></px:PXSegmentMask>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit8" DataField="SegmentDesc" ></px:PXTextEdit>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit5" DataField="Description" ></px:PXTextEdit>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule4" StartColumn="True" ></px:PXLayoutRule>
			<px:PXNumberEdit runat="server" ID="CstPXNumberEdit2" DataField="Version" ></px:PXNumberEdit>
			<px:PXCheckBox CommitChanges="True" runat="server" ID="CstPXCheckBox13" DataField="Hold" ></px:PXCheckBox>
			<px:PXDropDown runat="server" ID="CstPXDropDown10" DataField="Status" ></px:PXDropDown>
			<px:PXSelector runat="server" ID="CstPXSelector11" DataField="CreatedByID" ></px:PXSelector>
			<px:PXDateTimeEdit Size="M" runat="server" ID="CstPXDateTimeEdit12" DataField="CreatedDateTime" Enabled="False" ></px:PXDateTimeEdit></Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXGrid ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="Details" AllowAutoHide="false">
		<Levels>
			<px:PXGridLevel DataMember="TemplateL">
			    <Columns>
				<px:PXGridColumn DataField="CheckItem" Width="100" ></px:PXGridColumn>
			    </Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
		<ActionBar >
		</ActionBar>
	
		<Mode AllowUpload="True" /></px:PXGrid>
</asp:Content>