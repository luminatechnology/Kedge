<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="GV303001.aspx.cs" Inherits="Page_GV303001" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="RCGV.GV.GVApArTaxFreeSalesEntry"
        PrimaryView="TaxFilter"
        >
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="TaxFilter" Width="100%" Height="" AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartRow="True"></px:PXLayoutRule>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule1" StartColumn="True" ></px:PXLayoutRule>
			<px:PXSelector runat="server" ID="CstPXSelector4" DataField="RegistrationCD" CommitChanges="True" ></px:PXSelector>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule2" StartColumn="True" ></px:PXLayoutRule>
			<px:PXNumberEdit runat="server" ID="CstPXNumberEdit5" DataField="DeclareYear" CommitChanges="True" ></px:PXNumberEdit></Template>
	
		<AutoSize MinHeight="50" />
		<AutoSize Enabled="True" />
		<AutoSize Container="Window" /></px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXGrid SyncPosition="True" ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="Details" AllowAutoHide="false">
		<Levels>
			<px:PXGridLevel DataMember="MediaDeclarations">
			    <Columns>
				<px:PXGridColumn DataField="RegistrationCD" Width="108" ></px:PXGridColumn>
				<px:PXGridColumn DataField="DeclareYear" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Period" Width="70" />
				<px:PXGridColumn DataField="DeclareBatchNbr" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="ARSalesAmt" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="ARSalesAmtByZero" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn CommitChanges="True" DataField="TaxFreeSalse" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="ARSalesAmtBySP" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="APTaxAmt" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="APDiscountTax" Width="100" ></px:PXGridColumn></Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
		<ActionBar >
		
			<Actions>
				<AddNew Enabled="False" /></Actions>
			<Actions>
				<Delete Enabled="False" /></Actions></ActionBar>
	
		<Mode AllowDelete="False" ></Mode></px:PXGrid>
</asp:Content>