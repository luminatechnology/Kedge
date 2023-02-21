<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="NM502005.aspx.cs" Inherits="Page_NM502005" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="NM.NMApCheckBankProcess"
        PrimaryView="MasterView"
        >
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="MasterView" Width="100%" Height="" AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartRow="True"></px:PXLayoutRule>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule44" StartColumn="True" ColumnWidth="L" ControlSize="XM" ></px:PXLayoutRule>
			<px:PXSegmentMask CommitChanges="True" runat="server" ID="CstPXSegmentMask61" DataField="CashierID" ></px:PXSegmentMask>
			<px:PXNumberEdit CommitChanges="True" runat="server" ID="CstPXNumberEdit60" DataField="ProjectStartPeriod" ></px:PXNumberEdit>
			<px:PXDateTimeEdit CommitChanges="True" runat="server" ID="CstPXDateTimeEdit71" DataField="CheckStartDate" ></px:PXDateTimeEdit>
			<px:PXSegmentMask CommitChanges="True" runat="server" ID="CstPXSegmentMask63" DataField="ProjectID" ></px:PXSegmentMask>
			<px:PXSelector runat="server" ID="CstPXSelector8" DataField="UsrTrPaymentType" CommitChanges="True" ></px:PXSelector>
			<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit12" DataField="DueDate" CommitChanges="True" ></px:PXDateTimeEdit>
			<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit9" DataField="UsrTrConfirmDate" CommitChanges="True" ></px:PXDateTimeEdit>
			
			<px:PXLayoutRule ColumnWidth="L" ControlSize="XM" runat="server" ID="CstPXLayoutRule45" StartColumn="True" ></px:PXLayoutRule>
			<px:PXSelector Visible="False" CommitChanges="True" runat="server" ID="CstPXSelector69" DataField="BankCode" ></px:PXSelector>
			<px:PXSelector Visible="False" CommitChanges="True" runat="server" ID="CstPXSelector70" DataField="VendorLocationID" ></px:PXSelector>
			<px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector68" DataField="VendorID" ></px:PXSelector>
			<px:PXNumberEdit CommitChanges="True" runat="server" ID="CstPXNumberEdit66" DataField="ProjectEndPeriod" ></px:PXNumberEdit>
			<px:PXDateTimeEdit CommitChanges="True" runat="server" ID="CstPXDateTimeEdit72" DataField="CheckEndDate" ></px:PXDateTimeEdit>
			<px:PXDropDown runat="server" ID="CstPXDropDown7" DataField="CheckStatus" CommitChanges="True" ></px:PXDropDown>
			<px:PXNumberEdit runat="server" ID="CstPXNumberEdit10" DataField="UsrTrConfirmID" CommitChanges="True" ></px:PXNumberEdit>
			<px:PXSelector runat="server" ID="CstPXSelector11" DataField="UsrTrConfirmBy" CommitChanges="True" ></px:PXSelector>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule1" StartColumn="True" ></px:PXLayoutRule>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule3" StartGroup="True" GroupCaption="Select Info" ></px:PXLayoutRule>
			<px:PXNumberEdit runat="server" ID="CstPXNumberEdit5" DataField="SelectedCount" ></px:PXNumberEdit>
			<px:PXNumberEdit runat="server" ID="CstPXNumberEdit6" DataField="SelectedOriCuryAmount" ></px:PXNumberEdit></Template>
	
		<AutoSize Container="Parent" ></AutoSize>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXGrid SyncPosition="True" PageSize="100" ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="Details" AllowAutoHide="false">
		<Levels>
			<px:PXGridLevel DataMember="DetailsView">
			    <Columns>
			        <px:PXGridColumn CommitChanges="True" AllowCheckAll="True" Type="CheckBox" DataField="Selected" Width="60" ></px:PXGridColumn>
				<px:PXGridColumn DataField="APInvBatchNbr" Width="280" LinkCommand="viewAPReleaseGL" ></px:PXGridColumn>
				<px:PXGridColumn DataField="PayableCheckCD" Width="180" LinkCommand="ViewCheck" ></px:PXGridColumn>
				<px:PXGridColumn DataField="NMBatchNbr" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="BranchID" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CheckDate" Width="90" ></px:PXGridColumn>
				<px:PXGridColumn DataField="EtdDepositDate" Width="90" ></px:PXGridColumn>
				<px:PXGridColumn DataField="APDueDate" Width="90" />
				<px:PXGridColumn DataField="VendorID" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="VendorID_description" Width="280" ></px:PXGridColumn>
				<px:PXGridColumn DataField="VendorLocationID" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="ProjectID" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="ProjectID_description" Width="220" ></px:PXGridColumn>
				<px:PXGridColumn DataField="ProjectPeriod" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="BankAccountID" Width="180" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CuryID" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="OriCuryAmount" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="PayableCashierID_description" Width="220" ></px:PXGridColumn>
				<px:PXGridColumn DataField="UsrTrConfirmBy" Width="220" ></px:PXGridColumn>
				<px:PXGridColumn DataField="UsrTrConfirmDate" Width="90" ></px:PXGridColumn>
				<px:PXGridColumn DataField="UsrTrConfirmID" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="UsrTrPaymentType" Width="70" ></px:PXGridColumn></Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
		<ActionBar >
		
			<Actions>
				<AddNew Enabled="False" ></AddNew></Actions>
			<Actions>
				<AddNew MenuVisible="False" ></AddNew></Actions>
			<Actions>
				<Delete Enabled="False" ></Delete></Actions>
			<Actions>
				<Delete MenuVisible="False" ></Delete></Actions></ActionBar>
	
		<Mode AllowAddNew="False" ></Mode></px:PXGrid>
</asp:Content>