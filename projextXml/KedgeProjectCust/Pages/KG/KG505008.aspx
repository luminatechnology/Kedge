<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="KG505008.aspx.cs" Inherits="Page_KG505008" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/ListView.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="Kedge.KGCreatePORetainageProc" PrimaryView="Retainage">
		<CallbackCommands>
		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phL" runat="Server">
	<px:PXGrid ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="PrimaryInquire" AllowAutoHide="false" SyncPosition="true">
		<Levels>
			<px:PXGridLevel DataMember="Retainage">
			    <Columns>
			        <px:PXGridColumn DataField="Selected" AllowCheckAll="true" TextAlign="Center" Type="CheckBox" Width="30px"></px:PXGridColumn>
					<px:PXGridColumn DataField="OrderDate"></px:PXGridColumn>
					<px:PXGridColumn DataField="OrderType"></px:PXGridColumn>
					<px:PXGridColumn DataField="OrderNbr"></px:PXGridColumn>
					<px:PXGridColumn DataField="ProjBranchID"></px:PXGridColumn>
					<px:PXGridColumn DataField="ContractCD"></px:PXGridColumn>
					<px:PXGridColumn DataField="RetainageTotal"></px:PXGridColumn>
					<px:PXGridColumn DataField="RetainageReleased"></px:PXGridColumn>
					<px:PXGridColumn DataField="RetainageRemaining"></px:PXGridColumn>
			    </Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" />
		<ActionBar PagerVisible="Bottom" >
			<PagerSettings Mode="NumericCompact" />
		</ActionBar>
	</px:PXGrid>
</asp:Content>