<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="KG303002.aspx.cs" Inherits="Page_KG303002" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="Kedge.KGContractEvalEntry" PrimaryView="ContractEval">
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="ContractEval" Width="100%" >
		<Template>
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartRow="True"></px:PXLayoutRule>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule1" StartColumn="True" ></px:PXLayoutRule>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit9" DataField="ContractEvaluationCD" ></px:PXTextEdit>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit6" DataField="EvaluationName" ></px:PXTextEdit>
			<px:PXNumberEdit runat="server" ID="CstPXNumberEdit10" DataField="EvalPhase" />
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule2" StartColumn="True" ></px:PXLayoutRule>
			<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit4" DataField="EvaluationDate" ></px:PXDateTimeEdit>
			<px:PXNumberEdit runat="server" ID="CstPXNumberEdit5" DataField="Score" ></px:PXNumberEdit></Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXGrid AutoAdjustColumns="True" AdjustPageSize="None" AllowPaging="True" SyncPosition="True" ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="Details" AllowAutoHide="false">
		<Levels>
			<px:PXGridLevel DataMember="ContractEvalLine">
			    <Columns>
				<px:PXGridColumn DataField="QuestSeq" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Quest" Width="280" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Score" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="WeightScore" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="FinalScore" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Remark" Width="280" ></px:PXGridColumn></Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
		<ActionBar >
		</ActionBar>
	
		<CallbackCommands>
			<Refresh RepaintControlsIDs="form" /></CallbackCommands></px:PXGrid>
</asp:Content>