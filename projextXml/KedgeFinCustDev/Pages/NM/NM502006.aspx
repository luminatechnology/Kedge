<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="NM502006.aspx.cs" Inherits="Page_NM502006" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="NM.NMPrintableProcess"
        PrimaryView="Filters"
        >
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="Filters" Width="100%" Height="200px" AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartRow="True"></px:PXLayoutRule>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule1" StartColumn="True" ></px:PXLayoutRule>
			<px:PXSelector runat="server" ID="CstPXSelector3" DataField="BankAccountID" CommitChanges="True" ></px:PXSelector>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit4" DataField="CheckNbrFrom" CommitChanges="True" ></px:PXTextEdit>
			<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit5" DataField="CheckDateFrom" CommitChanges="True" ></px:PXDateTimeEdit>
			<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit6" DataField="EtdDepositDateFrom" CommitChanges="True" ></px:PXDateTimeEdit>
			<px:PXSegmentMask runat="server" ID="CstPXSegmentMask7" DataField="ProjectID" CommitChanges="True" ></px:PXSegmentMask>
			<px:PXSegmentMask runat="server" ID="CstPXSegmentMask8" DataField="VendorID" CommitChanges="True" ></px:PXSegmentMask>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule2" StartColumn="True" ></px:PXLayoutRule>
			<px:PXSegmentMask runat="server" ID="CstPXSegmentMask9" DataField="PayableCashierID" CommitChanges="True" ></px:PXSegmentMask>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit10" DataField="CheckNbrTo" CommitChanges="True" ></px:PXTextEdit>
			<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit11" DataField="CheckDateTo" CommitChanges="True" ></px:PXDateTimeEdit>
			<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit12" DataField="EtdDepositDateTo" CommitChanges="True" ></px:PXDateTimeEdit>
			<px:PXNumberEdit runat="server" ID="CstPXNumberEdit14" DataField="ProjectPeriod" CommitChanges="True" ></px:PXNumberEdit>
			<px:PXSelector runat="server" ID="CstPXSelector13" DataField="VendorLocationID" CommitChanges="True" ></px:PXSelector></Template>
	
		<AutoSize Container="Parent" /></px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXGrid SyncPosition="True" MatrixMode="True" ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="Details" AllowAutoHide="false">
		<Levels>
			<px:PXGridLevel DataMember="DetailViews">
			    <Columns>
				<px:PXGridColumn AllowCheckAll="True" Type="CheckBox" CommitChanges="True" DataField="Selected" Width="60" ></px:PXGridColumn>
				<px:PXGridColumn DataField="PayableCheckCD" Width="180" CommitChanges="True" LinkCommand="ViewPayableCD" ></px:PXGridColumn>
				<px:PXGridColumn DataField="APInvBatchNbr" Width="280" CommitChanges="True" LinkCommand="ViewAPReleaseGL" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Status" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CheckNbr" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="BankAccountID" Width="180" ></px:PXGridColumn>
				<px:PXGridColumn DataField="BookNbr" Width="180" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CheckDate" Width="90" ></px:PXGridColumn>
				<px:PXGridColumn DataField="EtdDepositDate" Width="90" ></px:PXGridColumn>
				<px:PXGridColumn DataField="VendorID" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="VendorID_description" Width="280" ></px:PXGridColumn>
				<px:PXGridColumn DataField="VendorLocationID" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Receiver" Width="280" ></px:PXGridColumn>
				<px:PXGridColumn DataField="ProjectID" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="ProjectID_description" Width="220" ></px:PXGridColumn>
				<px:PXGridColumn DataField="ProjectPeriod" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CuryID" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="BaseCuryAmount" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="OriCuryAmount" Width="100" ></px:PXGridColumn></Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
		<ActionBar >
		</ActionBar>
	</px:PXGrid>
</asp:Content>