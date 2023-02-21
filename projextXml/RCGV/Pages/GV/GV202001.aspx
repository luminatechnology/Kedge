<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="GV202001.aspx.cs" Inherits="Page_GV202001" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ToolTip="" ID="ds" runat="server" Visible="True" Width="100%" PrimaryView="gvArGuiInvoices" TypeName="RCGV.GV.GVArInvoiceEntry">
        <CallbackCommands>
            <px:PXDSCallbackCommand PopupCommand="addVoid" PopupPanel="PanelAddVoid" PostData="Page" PopupCheckSave="True" CommitChanges="True" Name="AddVoid" Visible="True"></px:PXDSCallbackCommand>
        </CallbackCommands>
    </px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100"
        Width="100%" DataMember="gvArGuiInvoices" TabIndex="6400" CaptionVisible="False">
        <Template>
            <px:PXLayoutRule runat="server" StartRow="True" ></px:PXLayoutRule>
	<px:PXLayoutRule runat="server" ID="CstPXLayoutRule1" StartColumn="True" ></px:PXLayoutRule>
	<px:PXSelector runat="server" ID="CstPXSelector25" DataField="GuiInvoiceCD" ></px:PXSelector>
	<px:PXDateTimeEdit CommitChanges="True" runat="server" ID="CstPXDateTimeEdit8" DataField="InvoiceDate" ></px:PXDateTimeEdit>
	<px:PXSegmentMask CommitChanges="True" runat="server" ID="CstPXSegmentMask7" DataField="CustomerID" ></px:PXSegmentMask>
	<px:PXTextEdit runat="server" ID="CstPXTextEdit6" DataField="CustName" ></px:PXTextEdit>
	<px:PXTextEdit runat="server" ID="CstPXTextEdit5" DataField="CustUniformNumber" ></px:PXTextEdit>
	<px:PXTextEdit runat="server" ID="CstPXTextEdit4" DataField="CustAddress" ></px:PXTextEdit>
	<px:PXDropDown runat="server" ID="CstPXDropDown11" DataField="Status" ></px:PXDropDown>
	<px:PXLayoutRule runat="server" ID="CstLayoutRule41" ColumnSpan="3" ></px:PXLayoutRule>
	<px:PXTextEdit runat="server" ID="CstPXTextEdit40" DataField="Remark" ></px:PXTextEdit>
	<px:PXLayoutRule runat="server" ID="CstPXLayoutRule2" StartColumn="True" ></px:PXLayoutRule>
	<px:PXSelector runat="server" ID="CstPXSelector13" DataField="RegistrationCD" ></px:PXSelector>
	<px:PXDropDown CommitChanges="True" runat="server" ID="CstPXDropDown26" DataField="TaxCode" ></px:PXDropDown>
	<px:PXCheckBox runat="server" ID="CstPXCheckBox42" DataField="IsHistorical" CommitChanges="True" ></px:PXCheckBox>
	<px:PXSelector AutoRefresh="True" CommitChanges="True" runat="server" ID="CstPXSelector12" DataField="GuiBookID" ></px:PXSelector>
	<px:PXDropDown runat="server" ID="CstPXDropDown43" DataField="GuiType" ></px:PXDropDown>
	<px:PXTextEdit runat="server" ID="CstPXTextEdit9" DataField="GuiInvoiceNbr" ></px:PXTextEdit >
	<px:PXLayoutRule runat="server" ID="CstPXLayoutRule3" StartColumn="True" ></px:PXLayoutRule>
	<px:PXNumberEdit CommitChanges="True" runat="server" ID="CstPXNumberEdit16" DataField="SalesAmt" ></px:PXNumberEdit>
	<px:PXNumberEdit CommitChanges="True" runat="server" ID="CstPXNumberEdit15" DataField="TaxAmt" ></px:PXNumberEdit>
	<px:PXNumberEdit runat="server" ID="CstPXNumberEdit14" DataField="TotalAmt" ></px:PXNumberEdit></Template>
    </px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
   <px:PXSmartPanel ID="PanelAddVoid" runat="server" AcceptButtonID="PXButtonOK" AutoReload="true" CancelButtonID="PXButtonCancel"
  Caption="Void GVArInvoice" CaptionVisible="True" DesignView="Content" HideAfterAction="false" Key="addFilter"
  LoadOnDemand="true" DefaultControlID="CstPXTextEdit26">
        <px:PXFormView SkinID="Transparent" Width="" runat="server" ID="CstFormView16" DataMember="addFilter" DataSourceID="ds">
            <Template>
	<px:PXLayoutRule runat="server" ID="CstPXLayoutRule35" StartRow="True" />
                <px:PXTextEdit runat="server" ID="CstPXTextEdit26" DataField="VoidReason" CommitChanges="True" TextMode="MultiLine" Height="60px" LabelWidth="70px" ></px:PXTextEdit>
	<px:PXLayoutRule runat="server" ID="CstPXLayoutRule36" StartRow="True" />
	<px:PXPanel runat="server" ID="CstPanel37" SkinID="Buttons">
		<px:PXButton runat="server" ID="CstButton38" DialogResult="OK" Text="OK" />
		<px:PXButton runat="server" ID="CstButton39" DialogResult="Cancel" Text="Cancel" /></px:PXPanel></Template>
        </px:PXFormView></px:PXSmartPanel>
	<px:PXTab Width="100%" runat="server" ID="CstPXTab17">
		<Items>
			<px:PXTabItem Text="Details" >
				<Template>
					<px:PXGrid SkinID="Details" SyncPosition="True" runat="server" ID="CstPXGrid18" Width="100%">
						<Levels>
							<px:PXGridLevel DataMember="gvArGuiInvoiceDetails" >
								<Columns>
									<px:PXGridColumn CommitChanges="True" DataField="ARRefNbr" Width="140" ></px:PXGridColumn>
									<px:PXGridColumn DataField="ARCurLineAmt" Width="100" ></px:PXGridColumn>
									<px:PXGridColumn DataField="CRMAmt" Width="100" />
									<px:PXGridColumn DataField="ItemDesc" Width="280" ></px:PXGridColumn>
									<px:PXGridColumn CommitChanges="True" DataField="Qty" Width="100" ></px:PXGridColumn>
									<px:PXGridColumn DataField="Uom" Width="72" ></px:PXGridColumn>
									<px:PXGridColumn CommitChanges="True" DataField="UnitPrice" Width="100" ></px:PXGridColumn>
									<px:PXGridColumn CommitChanges="True" DataField="SalesAmt" Width="100" ></px:PXGridColumn>
									<px:PXGridColumn CommitChanges="True" DataField="TaxAmt" Width="100" ></px:PXGridColumn></Columns>
                                              <RowTemplate>
                                    <px:PXSelector runat="server" ID="T1_ARRefNbr" DataField="ARRefNbr" AutoRefresh="True" ></px:PXSelector>
                                 </RowTemplate>
</px:PXGridLevel></Levels>
						<AutoSize Enabled="True" MinHeight="150" Container="Window" ></AutoSize>
						<Mode InitNewRow="True" ></Mode></px:PXGrid></Template></px:PXTabItem>
			<px:PXTabItem Text="Other Information" >
				<Template>
					<px:PXFormView runat="server" ID="CstFormView28" DataMember="gvArGuiInvoices" Width="100%" >
						<Template>
							<px:PXLayoutRule runat="server" ID="CstPXLayoutRule29" StartRow="True" ></px:PXLayoutRule>
							<px:PXLayoutRule runat="server" ID="CstPXLayoutRule34" StartColumn="True" ></px:PXLayoutRule>
							<px:PXTextEdit runat="server" ID="CstPXTextEdit33" DataField="VoidBy" ></px:PXTextEdit>
							<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit32" DataField="VoidDate" ></px:PXDateTimeEdit>
							<px:PXTextEdit runat="server" ID="CstPXTextEdit31" DataField="VoidReason" ></px:PXTextEdit>
							<px:PXLayoutRule runat="server" ID="CstPXLayoutRule30" StartColumn="True" ></px:PXLayoutRule></Template>
						<AutoSize Enabled="True" Container="Window" ></AutoSize></px:PXFormView></Template></px:PXTabItem></Items></px:PXTab></asp:Content>