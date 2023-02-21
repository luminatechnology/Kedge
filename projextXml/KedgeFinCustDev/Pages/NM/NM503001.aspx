<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="NM503001.aspx.cs" Inherits="Page_NM503001" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="NM.NMSettlementReport" PrimaryView="MasterView">
		<CallbackCommands>
		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="MasterView" Width="100%" Height="140px" AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartRow="True"></px:PXLayoutRule>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule1" StartColumn="True" ></px:PXLayoutRule>
			<px:PXGroupBox Caption="Filter" RenderStyle="Fieldset"  runat="server" ID="CstGroupBox4">
				<Template>
					<px:PXLayoutRule runat="server" ID="CstPXLayoutRule7" StartRow="True" ></px:PXLayoutRule>
					<px:PXSelector CommitChanges="True" runat="server" ID="CstPXNumberEdit8" DataField="FilterBranchID" ></px:PXSelector>
					<px:PXDateTimeEdit CommitChanges="True" runat="server" ID="CstPXDateTimeEdit9" DataField="FilterDateFrom" ></px:PXDateTimeEdit>
					<px:PXDateTimeEdit CommitChanges="True" runat="server" ID="CstPXDateTimeEdit10" DataField="FilterDateTo" ></px:PXDateTimeEdit></Template>
				</px:PXGroupBox>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule2" StartColumn="True" ></px:PXLayoutRule>
			<px:PXGroupBox Caption="Settlement Data" RenderStyle="Fieldset" runat="server" ID="CstGroupBox5">
				<Template>
					<px:PXLayoutRule runat="server" ID="CstPXLayoutRule11" StartRow="True" ></px:PXLayoutRule>
					<px:PXSelector runat="server" ID="CstPXNumberEdit13" DataField="SettlementBranchID" ></px:PXSelector>
					<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit14" DataField="SettlementDateFrom" ></px:PXDateTimeEdit>
					<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit15" DataField="SettlementDateTo" ></px:PXDateTimeEdit></Template></px:PXGroupBox>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule3" StartColumn="True" ></px:PXLayoutRule>
			<px:PXGroupBox Caption="Report Data" RenderStyle="Fieldset" runat="server" ID="CstGroupBox6">
				<Template>
					<px:PXLayoutRule runat="server" ID="CstPXLayoutRule12" StartRow="True" ></px:PXLayoutRule>
					<px:PXSelector runat="server" ID="CstPXNumberEdit18" DataField="ReportBranchID" ></px:PXSelector>
					<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit19" DataField="ReportDate" ></px:PXDateTimeEdit>
					<px:PXCheckBox runat="server" ID="CstPXCheckBox1" DataField="DisplayZero"></px:PXCheckBox>
				</Template></px:PXGroupBox></Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXGrid ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="Details" AllowAutoHide="false">
		<Levels>
			<px:PXGridLevel DataMember="DetailsView">
			    <Columns>
				<px:PXGridColumn DataField="SettlementDate" Width="90" ></px:PXGridColumn>
				<px:PXGridColumn DataField="SettledBy" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="LastModifiedDateTime" Width="90" ></px:PXGridColumn></Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
		<ActionBar >		
			<Actions>
				<AddNew MenuVisible="False" ></AddNew></Actions>
			<Actions>
				<AddNew Enabled="False" ></AddNew></Actions>
			<Actions>
				<Delete Enabled="False" ></Delete></Actions>
			<Actions>
				<Delete MenuVisible="False" ></Delete></Actions></ActionBar>	
		<Mode AllowAddNew="False" AllowUpdate="False" ></Mode></px:PXGrid>
</asp:Content>