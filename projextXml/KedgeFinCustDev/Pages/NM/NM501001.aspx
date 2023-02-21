<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="NM501001.aspx.cs" Inherits="Page_NM501001" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="NM.NMArCheckCashProcess"
        PrimaryView="CheckFilters"
        >
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="CheckFilters" Width="100%" Height="" AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartRow="True"></px:PXLayoutRule>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule1" StartColumn="True" ></px:PXLayoutRule>
			<px:PXLayoutRule GroupCaption="Filter" runat="server" ID="CstPXLayoutRule3" StartGroup="True" ></px:PXLayoutRule>
			<px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector7" DataField="CollBankAccountID" ></px:PXSelector>
			<px:PXSegmentMask CommitChanges="True" runat="server" ID="CstPXSegmentMask8" DataField="CollCashierID" ></px:PXSegmentMask>
			<px:PXDateTimeEdit CommitChanges="True" runat="server" ID="CstPXDateTimeEdit10" DataField="EtdDepositDate" ></px:PXDateTimeEdit>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule2" StartColumn="True" ></px:PXLayoutRule>
			<px:PXLayoutRule GroupCaption="Data" runat="server" ID="CstPXLayoutRule4" StartGroup="True" ></px:PXLayoutRule>
			<px:PXSegmentMask runat="server" ID="CstPXSegmentMask5" DataField="CashCashierID" ></px:PXSegmentMask>
			<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit16" DataField="DepositDate" ></px:PXDateTimeEdit>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule11" StartColumn="True" ></px:PXLayoutRule>
			<px:PXLayoutRule GroupCaption="Selected Cal" runat="server" ID="CstPXLayoutRule12" StartGroup="True" ></px:PXLayoutRule>
			<px:PXNumberEdit runat="server" ID="CstPXNumberEdit14" DataField="SelectedCount" ></px:PXNumberEdit>
			<px:PXNumberEdit runat="server" ID="CstPXNumberEdit15" DataField="SelectedOriCuryAmount" ></px:PXNumberEdit></Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXGrid SyncPosition="True" ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="Details" AllowAutoHide="false">
		<Levels>
			<px:PXGridLevel DataMember="CheckDetails">
			    <Columns>
			        <px:PXGridColumn AllowCheckAll="True" Type="CheckBox" CommitChanges="True" DataField="Selected" Width="60" ></px:PXGridColumn>
	<px:PXGridColumn DataField="Status" Width="70" ></px:PXGridColumn>
	<px:PXGridColumn DataField="CheckNbr" Width="120" ></px:PXGridColumn>
	<px:PXGridColumn DataField="CollCashierID" Width="140" ></px:PXGridColumn>
	<px:PXGridColumn DataField="CollCheckDate" Width="90" ></px:PXGridColumn>
	<px:PXGridColumn DataField="CollBankAccountID" Width="70" ></px:PXGridColumn>
	<px:PXGridColumn DataField="OriBankCode" Width="70" ></px:PXGridColumn>
	<px:PXGridColumn DataField="OriBankCode_description" Width="180" ></px:PXGridColumn>
	<px:PXGridColumn DataField="CheckDate" Width="90" ></px:PXGridColumn>
	<px:PXGridColumn DataField="EtdDepositDate" Width="90" ></px:PXGridColumn>
	<px:PXGridColumn DataField="CuryID" Width="70" ></px:PXGridColumn>
	<px:PXGridColumn DataField="OriCuryAmount" Width="100" ></px:PXGridColumn>
	<px:PXGridColumn DataField="BaseCuryAmount" Width="100" ></px:PXGridColumn>
	<px:PXGridColumn DataField="CustomerID" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CustomerID_BAccountR_acctName" Width="220" ></px:PXGridColumn>
	<px:PXGridColumn DataField="ProjectID" Width="70" ></px:PXGridColumn>
	<px:PXGridColumn DataField="ProjectID_description" Width="220" ></px:PXGridColumn>	
	<px:PXGridColumn DataField="Description" Width="280" ></px:PXGridColumn></Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
		<ActionBar >
		
			<Actions>
				<AddNew ToolBarVisible="False" ></AddNew></Actions>
			<Actions>
				<Delete ToolBarVisible="False" ></Delete></Actions></ActionBar>
	</px:PXGrid>
</asp:Content>