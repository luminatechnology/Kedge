<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="KG303001.aspx.cs" Inherits="Page_KG303001" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="Kedge.KGVenEvaluationEntry"
        PrimaryView="VendorEvaluation"
        >
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="VendorEvaluation" Width="100%" Height="100px" AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartRow="True"></px:PXLayoutRule>
            <px:PXLayoutRule ControlSize="M" LabelsWidth="M" runat="server" ID="CstPXLayoutRule2" StartColumn="True" ></px:PXLayoutRule>
			<px:PXSelector runat="server" ID="CstPXSelector2" DataField="EvaluationCD" />
            <px:PXDropDown runat="server" ID="CstPXDropDown3" DataField="Status" />
            <px:PXLayoutRule ControlSize="M" LabelsWidth="M" runat="server" ID="PXLayoutRule4" StartColumn="True" ></px:PXLayoutRule>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit5" DataField="EvaluationName" />
			<px:PXCheckBox runat="server" ID="CstPXCheckBox6" DataField="Hold" CommitChanges="True" /></Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXGrid ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="Details" AllowAutoHide="false">
		<Levels>
			<px:PXGridLevel DataMember="VendorEvaluationQuest">
			    <Columns>
				<px:PXGridColumn DataField="QuestSeq" Width="85" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Quest" Width="280" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Score" Width="70" ></px:PXGridColumn></Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
		<ActionBar >
		
			<Actions>
				<Upload Enabled="True" ></Upload></Actions></ActionBar>
	
		<Mode AllowUpload="True" /></px:PXGrid>
</asp:Content>
