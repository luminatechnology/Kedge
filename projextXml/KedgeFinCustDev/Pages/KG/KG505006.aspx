<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="KG505006.aspx.cs" Inherits="Page_KG505006" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/ListView.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="Kedge.KGADRWriteoffProcess"
        PrimaryView="ADRViews"
        >
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phL" runat="Server">
	<px:PXGrid ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="Primary" AllowAutoHide="false">
		<Levels>
			<px:PXGridLevel DataMember="ADRViews">
			    <Columns>
				<px:PXGridColumn DataField="Selected" Width="60" CommitChanges="True" Type="CheckBox" AllowCheckAll="True" />
				<px:PXGridColumn DataField="OrigRefNbr" Width="140" />
				<px:PXGridColumn DataField="RefNbr" Width="140" />
				<px:PXGridColumn DataField="UsrAccConfirmNbr" Width="140" />
				<px:PXGridColumn DataField="VendorID" Width="140" />
				<px:PXGridColumn DataField="VendorID_description" Width="280" />
				<px:PXGridColumn DataField="ProjectID" Width="70" />
				<px:PXGridColumn DataField="ProjectID_description" Width="280" />
				<px:PXGridColumn DataField="APInvoice__DueDate" Width="90" />
				<px:PXGridColumn DataField="CuryOrigDocAmt" Width="100" />
				<px:PXGridColumn DataField="UsrIsDeductionDoc" Width="60" Type="CheckBox" /></Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" />
		<ActionBar >
		</ActionBar>
	</px:PXGrid>
</asp:Content>