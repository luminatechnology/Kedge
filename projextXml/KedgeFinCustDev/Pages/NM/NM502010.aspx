<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="NM502010.aspx.cs" Inherits="Page_NM502010" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/ListView.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="NM.NMApPaymentProcess"
        PrimaryView="ProcessDatas"
        >
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phL" runat="Server">
	<px:PXGrid ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="Primary" AllowAutoHide="false">
		<Levels>
			<px:PXGridLevel DataMember="ProcessDatas">
			    <Columns>
				<px:PXGridColumn DataField="Selected" Width="60" Type="CheckBox" AllowCheckAll="True" ></px:PXGridColumn>
				<px:PXGridColumn DataField="BillPaymentID" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="RefNbr" Width="140" />
				<px:PXGridColumn DataField="LineNbr" Width="70" />
				<px:PXGridColumn DataField="PaymentDate" Width="90" />
				<px:PXGridColumn DataField="PaymentAmount" Width="100" /></Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" />
		<ActionBar >
		</ActionBar>
	</px:PXGrid>
</asp:Content>