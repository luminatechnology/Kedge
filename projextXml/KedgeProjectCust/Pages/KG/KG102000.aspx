<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="KG102000.aspx.cs" Inherits="Page_KG102000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="Kedge.KGContractCategoryMaint"
        PrimaryView="KGContractCategorys"
        >
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView SkinID="" SyncPosition="True" ID="form" runat="server" DataSourceID="ds" DataMember="KGContractCategorys" Width="100%" Height="" AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartRow="True"></px:PXLayoutRule>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule9" StartColumn="True" />
			<px:PXSelector runat="server" ID="CstPXSelector3" DataField="ContractCategoryCD" ></px:PXSelector>
			<px:PXDropDown runat="server" ID="CstPXDropDown5" DataField="ContractType" ></px:PXDropDown>
			<px:PXTextEdit Width="200px" TextMode="MultiLine" Height="100px" runat="server" ID="CstPXTextEdit4" DataField="ContractDesc" ></px:PXTextEdit>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule11" StartColumn="True" />
			<px:PXTextEdit LabelWidth="50px" runat="server" ID="CstPXTextEdit12" DataField="Remind" Size="XM" ></px:PXTextEdit></Template>
	
		<AutoSize Container="Window" ></AutoSize>
		<AutoSize Enabled="True" ></AutoSize></px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXGrid FilesIndicator="True" ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="Details" AllowAutoHide="false">
		<Levels>
			<px:PXGridLevel DataMember="KGContractTags">
			    <Columns>
				<px:PXGridColumn Type="CheckBox" DataField="Active" Width="60" CommitChanges="True" AllowCheckAll="True" ></px:PXGridColumn>
				<px:PXGridColumn CommitChanges="True" DataField="Tagcd" Width="180" ></px:PXGridColumn>
				<px:PXGridColumn CommitChanges="True" DataField="TagDesc" Width="200" ></px:PXGridColumn>
				<px:PXGridColumn CommitChanges="True" DataField="TagContent" Width="300" ></px:PXGridColumn></Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
		<ActionBar >
		</ActionBar>
	
		<OnChangeCommand Enabled="True" ></OnChangeCommand>
		<Mode AllowUpload="True" /></px:PXGrid>
</asp:Content>
