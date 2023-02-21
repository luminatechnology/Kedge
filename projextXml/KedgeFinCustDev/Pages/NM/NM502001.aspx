<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="NM502001.aspx.cs" Inherits="Page_NM502001" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="NM.NMApCheckConfirmProcess"
        PrimaryView="MasterView"
        >
		<CallbackCommands>
                     <px:PXDSCallbackCommand CommitChanges="True" Name="confirm" Visible="True" ></px:PXDSCallbackCommand>
		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="MasterView" Width="100%" Height="" AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartRow="True"></px:PXLayoutRule>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule44" StartColumn="True" ColumnWidth="L" ControlSize="XM" ></px:PXLayoutRule>
			<px:PXSegmentMask CommitChanges="True" runat="server" ID="CstPXSegmentMask61" DataField="CashierID" ></px:PXSegmentMask>
			<px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector68" DataField="VendorID" ></px:PXSelector>
			<px:PXNumberEdit CommitChanges="True" runat="server" ID="CstPXNumberEdit60" DataField="ProjectStartPeriod" ></px:PXNumberEdit>
			<px:PXDateTimeEdit CommitChanges="True" runat="server" ID="CstPXDateTimeEdit71" DataField="CheckStartDate" ></px:PXDateTimeEdit>
			<px:PXSegmentMask CommitChanges="True" runat="server" ID="CstPXSegmentMask63" DataField="ProjectID" ></px:PXSegmentMask>
			<px:PXSelector runat="server" ID="CstPXSelector83" DataField="UsrTrPaymentType" CommitChanges="True" ></px:PXSelector>
			<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit86" DataField="DueDate" CommitChanges="True" />
			<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit84" DataField="UsrTrConfirmDate" CommitChanges="True" ></px:PXDateTimeEdit>
			
			<px:PXLayoutRule ColumnWidth="L" ControlSize="XM" runat="server" ID="CstPXLayoutRule45" StartColumn="True" ></px:PXLayoutRule>
			<px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector73" DataField="BankAccountID" ></px:PXSelector>
			<px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector70" DataField="VendorLocationID" ></px:PXSelector>
			<px:PXNumberEdit CommitChanges="True" runat="server" ID="CstPXNumberEdit66" DataField="ProjectEndPeriod" ></px:PXNumberEdit>
			<px:PXDateTimeEdit CommitChanges="True" runat="server" ID="CstPXDateTimeEdit72" DataField="CheckEndDate" ></px:PXDateTimeEdit>
			<px:PXDropDown runat="server" ID="CstPXDropDown79" DataField="Status" CommitChanges="True" ></px:PXDropDown>
			<px:PXNumberEdit runat="server" ID="CstPXNumberEdit82" DataField="UsrTrConfirmID" CommitChanges="True" ></px:PXNumberEdit>
			<px:PXSelector runat="server" ID="CstPXSelector85" DataField="UsrTrConfirmBy" CommitChanges="True" ></px:PXSelector>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule74" StartColumn="True" ></px:PXLayoutRule>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule75" StartGroup="True" GroupCaption="Select Info" ></px:PXLayoutRule>
			<px:PXNumberEdit runat="server" ID="CstPXNumberEdit77" DataField="SelectedCount" ></px:PXNumberEdit>
			<px:PXNumberEdit runat="server" ID="CstPXNumberEdit78" DataField="SelectedOriCuryAmount" ></px:PXNumberEdit></Template>
	
		<AutoSize Container="Parent" ></AutoSize></px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXGrid SyncPosition="True" PageSize="100" ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="Details" AllowAutoHide="false">
		<Levels>
			<px:PXGridLevel DataMember="DetailsView">
			    <Columns>
				<px:PXGridColumn CommitChanges="True" AllowCheckAll="True" Type="CheckBox" DataField="Selected" Width="60" ></px:PXGridColumn>
				<px:PXGridColumn DataField="PayableCheckCD" Width="180" LinkCommand="ViewCheck" ></px:PXGridColumn>
				<px:PXGridColumn DataField="APInvBatchNbr" Width="280" LinkCommand="viewAPReleaseGL" ></px:PXGridColumn>
				<px:PXGridColumn DataField="BranchID" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CheckDate" Width="90" ></px:PXGridColumn>
				<px:PXGridColumn DataField="EtdDepositDate" Width="90" ></px:PXGridColumn>
				<px:PXGridColumn DataField="APDueDate" Width="90" />
				<px:PXGridColumn LinkCommand="viewVendor" DataField="VendorID" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="VendorID_description" Width="280" ></px:PXGridColumn>
				<px:PXGridColumn DataField="VendorLocationID" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="ProjectID" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="ProjectID_description" Width="220" ></px:PXGridColumn>
				<px:PXGridColumn DataField="ProjectPeriod" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn CommitChanges="True" DataField="BankAccountID" Width="180" ></px:PXGridColumn>
				<px:PXGridColumn DataField="BookNbr" Width="180" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CuryID" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="OriCuryAmount" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="PayableCashierID_description" Width="220" ></px:PXGridColumn>
				<px:PXGridColumn DataField="UsrTrPaymentType" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="UsrTrConfirmID" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="UsrTrConfirmBy" Width="220" ></px:PXGridColumn>
				<px:PXGridColumn DataField="UsrTrConfirmDate" Width="90" ></px:PXGridColumn></Columns>
                                <RowTemplate>
                                    <px:PXSelector runat="server" ID="PXSelector_BookNbr" DataField="BookNbr" AutoRefresh="True" ></px:PXSelector>
                                </RowTemplate>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
		<ActionBar >
		
			<Actions>
				<AddNew ToolBarVisible="False" Enabled="False" MenuVisible="False" ></AddNew>
						<Delete ToolBarVisible="False" MenuVisible="False" ></Delete>
						<Delete Enabled="False" ></Delete></Actions>
			<Actions>
				<Delete MenuVisible="False" ></Delete></Actions></ActionBar>
	</px:PXGrid>
</asp:Content>