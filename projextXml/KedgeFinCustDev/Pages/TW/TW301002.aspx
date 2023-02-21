<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="TW301002.aspx.cs" Inherits="Page_TW301002" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/ListView.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="Fin.TWNWHTMaint"
        PrimaryView="WHTTrans"
        >
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phL" runat="Server">
	<px:PXGrid SyncPosition="True" ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="Primary" AllowAutoHide="false">
		<Levels>
			<px:PXGridLevel DataMember="WHTTrans">
			    <Columns>
				<px:PXGridColumn CommitChanges="True" DataField="BranchID" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="BranchID_description" Width="220" />
				<px:PXGridColumn DataField="TranType" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="DocType" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="RefNbr" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="BatchNbr" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="TranDate" Width="90" ></px:PXGridColumn>
				<px:PXGridColumn DataField="PayAmt" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="PaymentDate" Width="90" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Jurisdiction" Width="70" CommitChanges="True" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Jurisdiction_SegmentValue_descr" Width="220" ></px:PXGridColumn>
				<px:PXGridColumn DataField="TaxationAgreements" Width="70" CommitChanges="True" ></px:PXGridColumn>
				<px:PXGridColumn DataField="TaxationAgreements_SegmentValue_descr" Width="220" ></px:PXGridColumn>
				<px:PXGridColumn DataField="PersonalID" Width="120" ></px:PXGridColumn>
				<px:PXGridColumn DataField="PropertyID" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="TypeOfIn" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="WHTFmtCode" Width="70" CommitChanges="True" ></px:PXGridColumn>
				<px:PXGridColumn DataField="WHTFmtCode_SegmentValue_descr" Width="220" ></px:PXGridColumn>
				<px:PXGridColumn DataField="WHTFmtSub" Width="70" CommitChanges="True" ></px:PXGridColumn>
				<px:PXGridColumn DataField="WHTFmtSub_SegmentValue_descr" Width="220" ></px:PXGridColumn>
				<px:PXGridColumn DataField="PayeeName" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="PayeeAddr" Width="220" ></px:PXGridColumn>
				<px:PXGridColumn DataField="GNHI2Pct" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="GNHI2Code" Width="70" CommitChanges="True" ></px:PXGridColumn>
				<px:PXGridColumn DataField="GNHI2Code_description" Width="220" ></px:PXGridColumn>
				<px:PXGridColumn DataField="GNHI2Amt" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="WHTTaxPct" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="WHTAmt" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="NetAmt" Width="100" ></px:PXGridColumn></Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
	
		<Mode AllowUpload="True" />
		<Mode InitNewRow="True" /></px:PXGrid>
</asp:Content>