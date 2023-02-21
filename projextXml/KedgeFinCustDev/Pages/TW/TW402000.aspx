<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="TW402000.aspx.cs" Inherits="Page_TW402000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/ListView.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="Fin.Graph.TWNWHTInquiry"
        PrimaryView="WHTTran"
        >
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phL" runat="Server">
	<px:PXGrid AdjustPageSize="Auto" AllowPaging="True" SyncPosition="True" ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="100%" SkinID="Primary" AllowAutoHide="false">
		<Levels>
			<px:PXGridLevel DataMember="WHTTran">
			    <Columns>
				<px:PXGridColumn DataField="DocType" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="RefNbr" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="BatchNbr" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="TranDate" Width="90" ></px:PXGridColumn>
				<px:PXGridColumn DataField="PaymDate" Width="90" ></px:PXGridColumn>
				<px:PXGridColumn DataField="PersonalID" Width="120" ></px:PXGridColumn>
				<px:PXGridColumn DataField="PropertyID" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="TypeOfIn" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="WHTFmtCode" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="WHTFmtSub" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="PayeeName" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="PayeeAddr" Width="220" ></px:PXGridColumn>
				<px:PXGridColumn DataField="GNHI2Pct" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="GNHI2Code" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="GNHI2Amt" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="WHTTaxPct" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="WHTAmt" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="NetAmt" Width="100" ></px:PXGridColumn></Columns>
			
				<RowTemplate>
					<px:PXSelector runat="server" ID="CstPXSelector1" DataField="RefNbr" AllowEdit="True" ></px:PXSelector></RowTemplate></px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
		<ActionBar >
		</ActionBar>
	
		<Mode AllowAddNew="False" AllowDelete="False" ></Mode></px:PXGrid>
</asp:Content>