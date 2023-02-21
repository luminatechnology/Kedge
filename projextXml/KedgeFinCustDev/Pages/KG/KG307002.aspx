<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="KG307002.aspx.cs" Inherits="Page_KG307002" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="Kedge.KGBudApproveLevelEntry"
        PrimaryView="BudApproveLevels"
        >
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="Filters" Width="100%" Height="100px" AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartRow="True"></px:PXLayoutRule>
			<px:PXLayoutRule ControlSize="XM" runat="server" ID="CstPXLayoutRule1" StartColumn="True" ></px:PXLayoutRule>
			<px:PXSelector runat="server" ID="CstPXSelector4" DataField="Branch" />
			<px:PXNumberEdit CommitChanges="True" runat="server" ID="CstPXNumberEdit5" DataField="BudgetYear" ></px:PXNumberEdit></Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXGrid SyncPosition="True" RepaintColumns="True" ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="Details" AllowAutoHide="false">
		<Levels>
			<px:PXGridLevel DataMember="BudApproveLevels">
			    <Columns>
				<px:PXGridColumn CommitChanges="True" DataField="ApprovalLevelID" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="BillingType" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn CommitChanges="True" DataField="GroupNo" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="GroupNo_description" Width="220" ></px:PXGridColumn>
				<px:PXGridColumn CommitChanges="True" DataField="SubGroupNo" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn CommitChanges="True" DataField="EPDocType" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn CommitChanges="True" DataField="InventoryID" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="InventoryID_description" Width="280" ></px:PXGridColumn>
				<px:PXGridColumn CommitChanges="True" DataField="CreditAccountID" Width="120" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CreditAccountID_Account_description" Width="220" ></px:PXGridColumn>
				<px:PXGridColumn CommitChanges="True" DataField="CreditSubID" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CreditSubID_description" Width="280" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Remark" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn Type="CheckBox" DataField="Level1" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Level1Amt" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn Type="CheckBox" DataField="Level2" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Level2Amt" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn Type="CheckBox" DataField="Level3" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Level3Amt" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn Type="CheckBox" DataField="Level4" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Level4Amt" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn Type="CheckBox" DataField="Level5" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Level5Amt" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn Type="CheckBox" DataField="Level6" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Level6Amt" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn Type="CheckBox" DataField="Level7" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Level7Amt" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn Type="CheckBox" DataField="Level8" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Level8Amt" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn Type="CheckBox" DataField="Level9" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Level9Amt" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn Type="CheckBox" DataField="Level10" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Level10Amt" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn Type="CheckBox" DataField="Level11" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Level11Amt" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn Type="CheckBox" DataField="Level12" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Level12Amt" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn Type="CheckBox" DataField="Level13" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Level13Amt" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn Type="CheckBox" DataField="Level14" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Level14Amt" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn Type="CheckBox" DataField="Level15" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Level15Amt" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn Type="CheckBox" DataField="Level16" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Level16Amt" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn Type="CheckBox" DataField="Level17" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Level17Amt" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn Type="CheckBox" DataField="Level18" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Level18Amt" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn Type="CheckBox" DataField="Level19" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Level19Amt" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn Type="CheckBox" DataField="Level20" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Level20Amt" Width="100" ></px:PXGridColumn></Columns>
			
				<RowTemplate ></RowTemplate></px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
		<ActionBar >
		</ActionBar>
	
		<Mode AllowUpload="True" /></px:PXGrid>
</asp:Content>