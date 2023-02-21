<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="TW201001.aspx.cs" Inherits="Page_TW201001" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="Fin.TWWHTMaint"
        PrimaryView="Filter"
        >
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="Filter" Width="100%" Height="155px" AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartRow="True"></px:PXLayoutRule>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule1" StartColumn="True" ></px:PXLayoutRule>
			<px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector15" DataField="PersonalID" ></px:PXSelector>
			<px:PXNumberEdit CommitChanges="True" runat="server" ID="CstPXNumberEdit7" DataField="DeclareYearFrom" ></px:PXNumberEdit>
			<px:PXNumberEdit CommitChanges="True" runat="server" ID="CstPXNumberEdit9" DataField="DeclareYearTo" ></px:PXNumberEdit>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule2" StartColumn="True" ></px:PXLayoutRule>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit5" DataField="PayeeName" ></px:PXTextEdit>
			<px:PXNumberEdit CommitChanges="True" runat="server" ID="CstPXNumberEdit6" DataField="DeclareMonthFrom" ></px:PXNumberEdit>
			<px:PXNumberEdit CommitChanges="True" runat="server" ID="CstPXNumberEdit8" DataField="DeclareMonthTo" ></px:PXNumberEdit>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule3" StartColumn="True" ></px:PXLayoutRule>
			<px:PXNumberEdit runat="server" ID="CstPXNumberEdit14" DataField="TotalWHTAmt" ></px:PXNumberEdit>
			<px:PXNumberEdit runat="server" ID="CstPXNumberEdit10" DataField="TotalGNHI2Amt" ></px:PXNumberEdit></Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXGrid SyncPosition="True" ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="Inquire" AllowAutoHide="false">
		<Levels>
			<px:PXGridLevel DataMember="WHTView">
			    <Columns>
				<px:PXGridColumn DataField="TranType" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn LinkCommand="viewEP" DataField="EPRefNbr" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn LinkCommand="viewAP" DataField="RefNbr" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="PayeeAddr" Width="220" ></px:PXGridColumn>
				<px:PXGridColumn DataField="PropertyID" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="TypeOfIn" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="TypeOfIn_CSAttributeDetail_description" Width="220" ></px:PXGridColumn>
				<px:PXGridColumn DataField="WHTFmtCode" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="WHTFmtCode_description" Width="220" ></px:PXGridColumn>
				<px:PXGridColumn DataField="WHTFmtSub" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="WHTFmtSub_description" Width="220" ></px:PXGridColumn>
				<px:PXGridColumn DataField="PayAmt" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="WHTAmt" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="NetAmt" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="PaymentDate" Width="90" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Jurisdiction" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Jurisdiction_description" Width="220" ></px:PXGridColumn>
				<px:PXGridColumn DataField="TaxationAgreements" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="TaxationAgreements_description" Width="220" ></px:PXGridColumn>
				<px:PXGridColumn DataField="GNHI2Amt" Width="100" ></px:PXGridColumn></Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
		<ActionBar >
		</ActionBar>
	
		<Mode InitNewRow="True" ></Mode></px:PXGrid>
</asp:Content>