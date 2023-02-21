<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="NM502099.aspx.cs" Inherits="Page_NM502099" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/ListView.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="NM.NMUploadHistorical"
        PrimaryView="DetailsView"
        >
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phL" runat="Server">
	<px:PXGrid SyncPosition="True" ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="Primary" AllowAutoHide="false">
		<Levels>
			<px:PXGridLevel DataMember="DetailsView">
			    <Columns>
				<px:PXGridColumn DataField="Error" Width="280" ></px:PXGridColumn>
				<px:PXGridColumn DataField="PayableCheckCD" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CheckNbr" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CheckDate" Width="90" ></px:PXGridColumn>
				<px:PXGridColumn DataField="EtdDepositDate" Width="90" />
				<px:PXGridColumn DataField="BankAccountID" Width="180" ></px:PXGridColumn>
				<px:PXGridColumn DataField="OriCuryAmount" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn CommitChanges="True" DataField="VendorID" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="VendorLocationID" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CheckTitle" Width="280" ></px:PXGridColumn>
				<px:PXGridColumn DataField="PayableCashierID" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="ProjectID" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="ProjectPeriod" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Description" Width="280" ></px:PXGridColumn></Columns>
			
				<RowTemplate>
					<px:PXSelector runat="server" ID="CstPXSelector1" DataField="VendorLocationID" AutoRefresh="True" ></px:PXSelector></RowTemplate></px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
		<ActionBar >
		
			<Actions>
				<Upload Enabled="True" ></Upload></Actions></ActionBar>
	
		<Mode AllowUpdate="True" AllowUpload="True" ></Mode></px:PXGrid>
</asp:Content>