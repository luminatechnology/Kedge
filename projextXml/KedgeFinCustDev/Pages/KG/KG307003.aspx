<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="KG307003.aspx.cs" Inherits="Page_KG307003" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/ListView.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="KedgeFinCustDev.KGBudGroupEntry"
        PrimaryView="BudGroup"
        >
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phL" runat="Server">
	<px:PXGrid ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="Primary" AllowAutoHide="false">
		<Levels>
			<px:PXGridLevel DataMember="BudGroup">
			    <Columns>
				<px:PXGridColumn DataField="BranchID" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="BudGroupNO" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="BudGroupName" Width="220" ></px:PXGridColumn>
				<px:PXGridColumn Type="CheckBox" DataField="IsTravelExpense" Width="60" ></px:PXGridColumn></Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
		<ActionBar >
		</ActionBar>
	
		<Mode AllowUpload="True" /></px:PXGrid>
</asp:Content>
