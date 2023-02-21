<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="NM501002.aspx.cs" Inherits="Page_NM501002" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="NM.NMArCheckModifyProcess"
        PrimaryView="CheckFilters"
        >
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="CheckFilters" Width="100%" Height="210px" AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartRow="True"></px:PXLayoutRule>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule7" StartColumn="True" ></px:PXLayoutRule>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule9" StartGroup="True" GroupCaption="Check Filter" ></px:PXLayoutRule>
			<px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector27" DataField="CheckNbr" ></px:PXSelector>
			<px:PXSegmentMask CommitChanges="True" runat="server" ID="CstPXSegmentMask28" DataField="ProjectID" ></px:PXSegmentMask>
			<px:PXSegmentMask CommitChanges="True" runat="server" ID="CstPXSegmentMask18" DataField="CustomerID" ></px:PXSegmentMask>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule11" StartColumn="True" ></px:PXLayoutRule>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule12" StartGroup="True" GroupCaption="Data" ></px:PXLayoutRule>
			<px:PXSegmentMask runat="server" ID="CstPXSegmentMask24" DataField="ModifyCashierID" ></px:PXSegmentMask>
			<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit26" DataField="ReverseDate" ></px:PXDateTimeEdit>
			<px:PXTextEdit Height="100px" TextMode="MultiLine" runat="server" ID="CstPXTextEdit25" DataField="Reason" ></px:PXTextEdit>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule3" StartColumn="True" ></px:PXLayoutRule>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule4" StartGroup="True" GroupCaption="Selected Cal" ></px:PXLayoutRule>
			<px:PXNumberEdit runat="server" ID="CstPXNumberEdit30" DataField="SelectedCount" ></px:PXNumberEdit>
			<px:PXNumberEdit runat="server" ID="CstPXNumberEdit31" DataField="SelectedOriCuryAmount" ></px:PXNumberEdit></Template>
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
	<px:PXGridColumn DataField="OriBankCode" Width="70" ></px:PXGridColumn>
	<px:PXGridColumn DataField="OriBankCode_description" Width="180" ></px:PXGridColumn>
        <px:PXGridColumn DataField="CheckCashierID" Width="140" ></px:PXGridColumn>
	<px:PXGridColumn DataField="CheckDate" Width="90" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CollBankAccountID" Width="180" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CollCashierID" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CollCashierID_description" Width="220" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CollCheckDate" Width="90" ></px:PXGridColumn>
	<px:PXGridColumn DataField="EtdDepositDate" Width="90" ></px:PXGridColumn>
	<px:PXGridColumn DataField="CuryID" Width="70" ></px:PXGridColumn>
	<px:PXGridColumn DataField="OriCuryAmount" Width="100" ></px:PXGridColumn>
	<px:PXGridColumn DataField="BaseCuryAmount" Width="100" ></px:PXGridColumn>
	<px:PXGridColumn DataField="CustomerID" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CustomerID_Customer_acctName" Width="220" ></px:PXGridColumn>
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