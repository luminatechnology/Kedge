<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="KG303000.aspx.cs" Inherits="Page_KG303000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormView.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="Kedge.KGValuationEntry"
        PrimaryView="Valuations"
        >
		<CallbackCommands>
			<px:PXDSCallbackCommand Visible="True" Name="Confirm" CommitChanges="True" PopupCommandTarget="ds" PopupCommand="confirmAction" ></px:PXDSCallbackCommand>
<px:PXDSCallbackCommand Visible="False" Name="ViewDailyRenterCD" CommitChanges="True" PopupCommandTarget="ds" PopupCommand="viewDailyRenterCD" ></px:PXDSCallbackCommand></CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="Valuations" Width="100%" AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule9" StartRow="True" ></px:PXLayoutRule>
			<px:PXLayoutRule LabelsWidth="S" ControlSize="M" runat="server" ID="CstPXLayoutRule6" StartColumn="True" ></px:PXLayoutRule>
			<px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector116" DataField="ValuationCD" ></px:PXSelector>
			<px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector182" DataField="ContractID" ></px:PXSelector>
			<px:PXDropDown CommitChanges="True" runat="server" ID="CstPXDropDown99" DataField="ValuationType" ></px:PXDropDown>
			<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit4" DataField="ValuationDate" ></px:PXDateTimeEdit>
			<px:PXDropDown CommitChanges="True" runat="server" ID="CstPXDropDown137" DataField="Uom" ></px:PXDropDown>
			<px:PXTextEdit Height="50" runat="server" ID="CstPXTextEdit145" DataField="Description" TextMode="MultiLine" ></px:PXTextEdit>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule147" Merge="True" />
			<px:PXCheckBox Size="S" CommitChanges="True" runat="server" ID="CstPXCheckBox106" DataField="IsFreeManageFeeAmt" ></px:PXCheckBox>
			<px:PXCheckBox CommitChanges="True" runat="server" ID="CstPXCheckBox146" DataField="IsTaxFree" ></px:PXCheckBox>
			<px:PXLayoutRule runat="server" ID="CstLayoutRule148" />
			<px:PXLayoutRule LabelsWidth="S" ControlSize="S" runat="server" ID="CstPXLayoutRule118" StartColumn="True" ></px:PXLayoutRule>
			<px:PXSelector runat="server" ID="CstPXSelector140" DataField="DailyRenterCD" AllowEdit="True" Enabled="False" ></px:PXSelector>
			<px:PXNumberEdit Size="S" runat="server" ID="CstPXNumberEdit133" DataField="Qty" CommitChanges="True" ></px:PXNumberEdit>
			<px:PXNumberEdit Size="S" CommitChanges="True" runat="server" ID="CstPXNumberEdit21" DataField="UnitPrice" ></px:PXNumberEdit>
			<px:PXNumberEdit Size="S" CommitChanges="True" runat="server" ID="CstPXNumberEdit15" DataField="Amount" ></px:PXNumberEdit>
			<px:PXNumberEdit Size="S" CommitChanges="True" runat="server" ID="CstPXNumberEdit107" DataField="TaxAmt" ></px:PXNumberEdit>
			<px:PXNumberEdit Size="S" CommitChanges="True" runat="server" ID="CstPXNumberEdit111" DataField="TotalAmt" ></px:PXNumberEdit>
			<px:PXNumberEdit Size="S" CommitChanges="True" runat="server" ID="CstPXNumberEdit14" DataField="AdditionAmt" ></px:PXNumberEdit>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule128" ColumnSpan="3" ></px:PXLayoutRule>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit3" DataField="ValuationContent" ></px:PXTextEdit>
			<px:PXLayoutRule LabelsWidth="S" ControlSize="SM" runat="server" ID="CstPXLayoutRule101" StartColumn="True" ></px:PXLayoutRule>
			<px:PXSelector runat="server" ID="CstPXSelector103" DataField="CreatedByID" ></px:PXSelector>
			<px:PXCheckBox Size="XS" CommitChanges="True" runat="server" ID="CstPXCheckBox98" DataField="Hold" ></px:PXCheckBox>
			<px:PXDropDown Size="" LabelWidth="" runat="server" ID="CstPXDropDown187" DataField="Status" ></px:PXDropDown>
			<px:PXNumberEdit Size="SM" CommitChanges="True" runat="server" ID="CstPXNumberEdit109" DataField="ManageFeeAmt" ></px:PXNumberEdit>
			<px:PXNumberEdit Size="SM" CommitChanges="True" runat="server" ID="CstPXNumberEdit100" DataField="ManageFeeTaxAmt" ></px:PXNumberEdit>
			<px:PXNumberEdit Size="SM" CommitChanges="True" runat="server" ID="CstPXNumberEdit112" DataField="ManageFeeTotalAmt" ></px:PXNumberEdit>
			<px:PXNumberEdit Size="SM" CommitChanges="True" runat="server" ID="CstPXNumberEdit16" DataField="DeductionAmt" ></px:PXNumberEdit>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule57" StartRow="True" ></px:PXLayoutRule>
			<px:PXLayoutRule ControlSize="M" runat="server" ID="CstPXLayoutRule58" StartColumn="True" ></px:PXLayoutRule>
			<px:PXFormView AllowCollapse="False" DataMember="AdditionDetails" runat="server" ID="CstFormView79">
				<Template>
					<px:PXLayoutRule ControlSize="XM" GroupCaption="Addition Contract" runat="server" ID="CstPXLayoutRule80" StartGroup="True" ></px:PXLayoutRule>
					<px:PXSelector AutoRefresh="True" CommitChanges="True" runat="server" ID="CstPXSelector168" DataField="OrderNbr" ></px:PXSelector>
					<px:PXTextEdit runat="server" ID="CstPXTextEdit121" DataField="Vendor" ></px:PXTextEdit>
					<px:PXTextEdit runat="server" ID="CstPXTextEdit132" DataField="InventoryID" ></px:PXTextEdit>
					<px:PXTextEdit runat="server" ID="CstPXTextEdit119" DataField="InvDesc" ></px:PXTextEdit>
					<px:PXTextEdit Enabled="False" runat="server" ID="CstPXTextEdit83" DataField="APInvoiceNbr" ></px:PXTextEdit>
					<px:PXDateTimeEdit Enabled="False" runat="server" ID="CstPXDateTimeEdit82" DataField="APInvoiceDate" ></px:PXDateTimeEdit></Template></px:PXFormView>
			<px:PXLayoutRule ControlSize="M" runat="server" ID="CstPXLayoutRule59" StartColumn="True" ></px:PXLayoutRule>
			<px:PXFormView AllowCollapse="False" DataMember="DeductionDetails" runat="server" ID="CstFormView70">
				<Template>
					<px:PXLayoutRule ControlSize="XM" GroupCaption="Deduction Contract" runat="server" ID="CstPXLayoutRule71" StartGroup="True" ></px:PXLayoutRule>
					<px:PXSelector AutoRefresh="True" CommitChanges="True" runat="server" ID="CstPXSelector171" DataField="OrderNbr" ></px:PXSelector>
					<px:PXTextEdit runat="server" ID="CstPXTextEdit124" DataField="Vendor" ></px:PXTextEdit>
					<px:PXTextEdit runat="server" ID="CstPXTextEdit131" DataField="InventoryID" ></px:PXTextEdit>
					<px:PXTextEdit runat="server" ID="CstPXTextEdit122" DataField="InvDesc" ></px:PXTextEdit>
					<px:PXTextEdit Enabled="False" runat="server" ID="CstPXTextEdit74" DataField="APInvoiceNbr" ></px:PXTextEdit>
					<px:PXDateTimeEdit Enabled="False" runat="server" ID="CstPXDateTimeEdit73" DataField="APInvoiceDate" ></px:PXDateTimeEdit>
					<px:PXNumberEdit runat="server" ID="CstPXNumberEdit140" DataField="UnBilledTotalAmt" ></px:PXNumberEdit></Template></px:PXFormView></Template>
		<AutoSize Container="Window" Enabled="True" MinHeight="200" ></AutoSize>
	</px:PXFormView></asp:Content>