<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="KG601001.aspx.cs" Inherits="Page_KG601001" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="Kedge.KGDailyMaterialReport"
        PrimaryView="MasterView"
        >
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="MasterView" Width="100%" Height="" AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartRow="True"></px:PXLayoutRule>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule1" StartColumn="True" ></px:PXLayoutRule>
			<px:PXSegmentMask CommitChanges="True" runat="server" ID="CstPXSegmentMask4" DataField="ProjectID" ></px:PXSegmentMask>
			<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit5" DataField="StageOneDate" ></px:PXDateTimeEdit>
			<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit7" DataField="StageTwoDate" ></px:PXDateTimeEdit>
			<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit6" DataField="StageThreeDate" ></px:PXDateTimeEdit>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule2" StartColumn="True" ></px:PXLayoutRule>
			<px:PXDateTimeEdit CommitChanges="True" runat="server" ID="CstPXDateTimeEdit3" DataField="ExecutionTime" ></px:PXDateTimeEdit>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit8" DataField="StageOneDesc" ></px:PXTextEdit>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit10" DataField="StageTwoDesc" ></px:PXTextEdit>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit9" DataField="StageThreeDesc" ></px:PXTextEdit></Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXGrid RepaintColumns="True" ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="Details" AllowAutoHide="false">
		<Levels>
			<px:PXGridLevel DataMember="DetailsView">
			    <Columns>
				<px:PXGridColumn DataField="InventoryCD" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="InventoryDesc" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="BudgetAmount" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="SOneQty" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="SOneAmt" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="SOnePerBudget" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="SOneIncAmount" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="STwoQty" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="STwoAmt" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="STwoPerBudget" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="STwoIncAmount" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="SThreeQty" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="SThreeAmt" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="SThreePerBudget" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="SThreeIncAmount" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="NowQty" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="NowAmt" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="NowPerBudget" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="NowIncAmount" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="RePerBudget" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="RemainingCost" Width="100" ></px:PXGridColumn></Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
		<ActionBar >
		</ActionBar>
	</px:PXGrid>
</asp:Content>
