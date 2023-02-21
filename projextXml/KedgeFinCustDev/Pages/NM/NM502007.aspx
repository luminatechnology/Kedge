<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="NM502007.aspx.cs" Inherits="Page_NM502007" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="NM.NMApTeleTransProcess"
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
			<px:PXLayoutRule ColumnWidth="L" ControlSize="" LabelsWidth="" runat="server" ID="CstPXLayoutRule1" StartColumn="True" ></px:PXLayoutRule>
			<px:PXLayoutRule GroupCaption="Insert Data" runat="server" ID="CstPXLayoutRule14" StartGroup="True" ></px:PXLayoutRule>
			<px:PXSelector runat="server" ID="CstPXSelector35" DataField="BankAccountID" CommitChanges="True" ></px:PXSelector>
			<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit44" DataField="PaymentDate" CommitChanges="True" />
			<px:PXLayoutRule ColumnWidth="L" ControlSize="" runat="server" ID="CstPXLayoutRule2" StartColumn="True" ></px:PXLayoutRule>
			<px:PXLayoutRule GroupCaption="Filters" runat="server" ID="CstPXLayoutRule27" StartGroup="True" ></px:PXLayoutRule>
			<px:PXSegmentMask runat="server" ID="CstPXSegmentMask32" DataField="ProjectID" CommitChanges="True" ></px:PXSegmentMask>
			<px:PXSelector runat="server" ID="CstPXSelector34" DataField="VendorID" CommitChanges="True" ></px:PXSelector>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule36" Merge="True" ></px:PXLayoutRule>
			<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit31" DataField="PayDate" CommitChanges="True" ></px:PXDateTimeEdit>
			<px:PXCheckBox runat="server" ID="CstPXCheckBox30" DataField="IsCheckIssue" CommitChanges="True" ></px:PXCheckBox>
			<px:PXLayoutRule runat="server" ID="CstLayoutRule37" ></px:PXLayoutRule>
			<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit29" DataField="DueDate" CommitChanges="True" ></px:PXDateTimeEdit>
			<px:PXSelector runat="server" ID="CstPXSelector41" DataField="UsrTrPaymentType" CommitChanges="True" ></px:PXSelector>
			<px:PXNumberEdit runat="server" ID="CstPXNumberEdit40" DataField="UsrTrConfirmID" CommitChanges="True" ></px:PXNumberEdit>
			<px:PXDateTimeEdit CommitChanges="True" runat="server" ID="CstPXDateTimeEdit43" DataField="UsrTrConfirmDate" ></px:PXDateTimeEdit>
			<px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector42" DataField="UsrTrConfirmBy" ></px:PXSelector>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule8" StartColumn="True" ></px:PXLayoutRule>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule9" StartGroup="True" GroupCaption="Select Info" ></px:PXLayoutRule>
			<px:PXNumberEdit runat="server" ID="CstPXNumberEdit11" DataField="SelectedCount" ></px:PXNumberEdit>
			<px:PXNumberEdit runat="server" ID="CstPXNumberEdit12" DataField="SelectedOriCuryAmount" ></px:PXNumberEdit></Template>
	
		<AutoSize Container="Parent" ></AutoSize></px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXGrid PageSize="100" SyncPosition="True" ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="Details" AllowAutoHide="false">
		<Levels>
			<px:PXGridLevel DataMember="DetailsView">
			    <Columns>
				<px:PXGridColumn CommitChanges="True" Type="CheckBox" DataField="Selected" Width="60" AllowCheckAll="True" ></px:PXGridColumn>
				<px:PXGridColumn DataField="NMBatchNbr" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="BranchID" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="ProjectID" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="ProjectID_PMProject_description" Width="280" />
				<px:PXGridColumn DataField="RefNbr" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="UsrAccConfirmNbr" Width="70" LinkCommand="ViewBatch" ></px:PXGridColumn>
				<px:PXGridColumn DataField="PaymentDate" Width="90" ></px:PXGridColumn>
				<px:PXGridColumn DataField="DueDate" Width="90" ></px:PXGridColumn>
				<px:PXGridColumn DataField="APVendorID" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="APVendorLocationID" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="VendorID" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="VendorID_BAccount2_acctName" Width="280" ></px:PXGridColumn>
				<px:PXGridColumn CommitChanges="True" DataField="VendorLocationID" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="ProjectPeriod" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="OriCuryAmount" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="PaymentMethodID" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CashAccountID" Width="120" ></px:PXGridColumn>
				<px:PXGridColumn CommitChanges="True" DataField="BankAccountID" Width="180" ></px:PXGridColumn>
				<px:PXGridColumn DataField="AcctName" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="BankAcct" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="BankName" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="BankNbr" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="BranchNam" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="BranchNbr" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Charge" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="UsrTrPaymentType" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="UsrTrConfirmID" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="UsrTrConfirmBy" Width="220" ></px:PXGridColumn>
				<px:PXGridColumn DataField="UsrTrConfirmDate" Width="90" ></px:PXGridColumn></Columns>
                                <RowTemplate>
                                    <px:PXSelector runat="server" ID="PXSelector_VendorLocationID" DataField="VendorLocationID" AutoRefresh="True" ></px:PXSelector>
                                    <px:PXSelector runat="server" ID="PXSelector_BankAccountID" DataField="BankAccountID" AutoRefresh="True" ></px:PXSelector>
                                </RowTemplate>
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
	</px:PXGrid>
</asp:Content>