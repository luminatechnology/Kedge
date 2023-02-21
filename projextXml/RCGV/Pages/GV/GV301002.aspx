<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormTab.master" AutoEventWireup="true"
    ValidateRequest="false" CodeFile="GV301002.aspx.cs" Inherits="Page_GV301002" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormTab.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
	<px:PXDataSource SkinID="" ID="ds" runat="server" Visible="True" Width="100%" PrimaryView="GuiCmInvoice" TypeName="RCGV.GV.GVApCmInvoiceEntry">
		<CallbackCommands>
			<px:PXDSCallbackCommand Name="AddAPCMVoid" CommitChanges="True" PopupCheckSave="True" PopupCommand="addAPCMVoid" PopupPanel="PanelAddVoid" PostData="Page" Visible="True" ></px:PXDSCallbackCommand></CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" DataMember="GuiCmInvoice" TabIndex="3500">
		<Template>
			<px:PXLayoutRule runat="server" StartRow="True" ControlSize="M" LabelsWidth="SM" StartColumn="False"></px:PXLayoutRule>
			<px:PXLayoutRule LabelsWidth="S" ControlSize="SM" runat="server" ID="CstPXLayoutRule1" StartColumn="True" ></px:PXLayoutRule>
			<px:PXSelector runat="server" ID="CstPXSelector7" DataField="GuiCmInvoiceNbr" ></px:PXSelector>
			<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit6" DataField="InvoiceDate" CommitChanges="True" ></px:PXDateTimeEdit>
			<px:PXNumberEdit runat="server" ID="CstPXNumberEdit5" DataField="DeclareYear" ></px:PXNumberEdit>
			<px:PXSegmentMask runat="server" ID="CstPXSegmentMask4" DataField="VendorID" CommitChanges="True" ></px:PXSegmentMask>
			<px:PXTextEdit CommitChanges="True" runat="server" ID="CstPXTextEdit3" DataField="VendorUniformNumber" ></px:PXTextEdit>
			<px:PXDropDown runat="server" ID="CstPXDropDown2" DataField="TaxCode" CommitChanges="True" ></px:PXDropDown>
			<px:PXSelector runat="server" ID="CstPXSelector1" DataField="AccConfirmNbr" AutoRefresh="True" />
			<px:PXLayoutRule runat="server" ID="CstLayoutRule37" ColumnSpan="3" ></px:PXLayoutRule>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit36" DataField="VendorAddress" ></px:PXTextEdit>
			<px:PXLayoutRule runat="server" ID="CstLayoutRule17" ColumnSpan="3" ></px:PXLayoutRule>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit1" DataField="Remark" ></px:PXTextEdit>
			<px:PXLayoutRule runat="server" ControlSize="SM" LabelsWidth="S" StartColumn="True">
			</px:PXLayoutRule>
			<px:PXSelector runat="server" ID="CstPXSelector13" DataField="RegistrationCD" ></px:PXSelector>
			<px:PXDropDown runat="server" ID="CstPXDropDown12" DataField="GuiType" ></px:PXDropDown>
			<px:PXDropDown runat="server" ID="CstPXDropDown32" DataField="DeclareMonth" ></px:PXDropDown>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit10" DataField="VendorName" ></px:PXTextEdit>
			<px:PXDropDown runat="server" ID="CstPXDropDown9" DataField="Status" ></px:PXDropDown>
			<px:PXCheckBox runat="server" ID="CstPXCheckBox31" DataField="Hold" CommitChanges="True" ></px:PXCheckBox>
			<px:PXLayoutRule runat="server" ControlSize="SM" LabelsWidth="S" StartColumn="True">
			</px:PXLayoutRule>
			<px:PXNumberEdit runat="server" ID="CstPXNumberEdit16" DataField="SalesAmt" ></px:PXNumberEdit>
			<px:PXNumberEdit runat="server" ID="CstPXNumberEdit15" DataField="TaxAmt" ></px:PXNumberEdit>
			<px:PXNumberEdit runat="server" ID="CstPXNumberEdit14" DataField="TotalAmt" ></px:PXNumberEdit></Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
	<px:PXTab ID="tab" runat="server" Width="100%" Height="150px" DataSourceID="ds">
		<Items>
			<px:PXTabItem Text="Gui Invoice" >
				<Template>
					<px:PXGrid Height="280px" Width="100%" SkinID="Details" runat="server" ID="CstPXGrid30" SyncPosition="True">
						<Levels>
							<px:PXGridLevel DataMember="GuiCmInvoiceLine" >
								<Columns>
									<px:PXGridColumn CommitChanges="True" DataField="ApGuiInvoiceNbr" Width="180" ></px:PXGridColumn>
									<px:PXGridColumn DataField="RefNbr" Width="70" ></px:PXGridColumn>
									<px:PXGridColumn DataField="InvSalesAmt" Width="100" ></px:PXGridColumn>
                                    <px:PXGridColumn DataField="InvTaxAmt" Width="90" ></px:PXGridColumn>
                                    <px:PXGridColumn DataField="Balance" Width="100" ></px:PXGridColumn>
                                    <px:PXGridColumn DataField="TaxBalance" Width="90" ></px:PXGridColumn>
									<px:PXGridColumn CommitChanges="True" DataField="SalesAmt" Width="100" ></px:PXGridColumn>
									<px:PXGridColumn CommitChanges="True" DataField="TaxAmt" Width="100" ></px:PXGridColumn>
									<px:PXGridColumn DataField="ItemDesc" Width="300" ></px:PXGridColumn></Columns>
								<RowTemplate>
									<px:PXSelector runat="server" ID="CstPXSelector34" DataField="ApGuiInvoiceNbr" AutoRefresh="True" ></px:PXSelector></RowTemplate></px:PXGridLevel></Levels>
						<Mode InitNewRow="True" ></Mode>
						<AutoSize Enabled="True" MinHeight="150" Container="Window" ></AutoSize>
						</px:PXGrid></Template></px:PXTabItem>
			
			<px:PXTabItem Text="Other Information" >
				<Template>
					<px:PXFormView runat="server" ID="CstFormView19" DataMember="GuiCmInvoice" Width="100%" >
						<Template>
							<px:PXLayoutRule runat="server" ID="CstPXLayoutRule20" StartRow="True" ></px:PXLayoutRule>
							<px:PXLayoutRule runat="server" ID="CstPXLayoutRule21" StartColumn="True" ></px:PXLayoutRule>
							<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit25" DataField="ConfirmDate" ></px:PXDateTimeEdit>
							<px:PXTextEdit runat="server" ID="CstPXTextEdit24" DataField="ConfirmPerson" ></px:PXTextEdit>
							<px:PXLayoutRule runat="server" ID="CstPXLayoutRule22" StartColumn="True" ></px:PXLayoutRule>
							<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit27" DataField="VoidDate" ></px:PXDateTimeEdit>
							<px:PXTextEdit runat="server" ID="CstPXTextEdit26" DataField="VoidReason" ></px:PXTextEdit>
							<px:PXLayoutRule runat="server" ID="CstPXLayoutRule23" StartColumn="True" ></px:PXLayoutRule>
							<px:PXNumberEdit runat="server" ID="CstPXNumberEdit28" DataField="PrintCount" ></px:PXNumberEdit></Template>
						<AutoSize Enabled="True" Container="Window" ></AutoSize></px:PXFormView></Template></px:PXTabItem></Items>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
	</px:PXTab>
</asp:Content>
<asp:Content ID="Content1" runat="server" contentplaceholderid="phDialogs">
	<px:PXSmartPanel ID="pnlVoidInvoice" runat="server" AcceptButtonID="PXButtonOK" AutoReload="true" CancelButtonID="PXButtonCancel"
          Caption="Void GVApInvoice" CaptionVisible="True" DesignView="Content" HideAfterAction="false" Key="VoidInvoicePanel"
          LoadOnDemand="true" DefaultControlID="CstPXTextEdit8">
		<px:PXFormView ID="frmMyCommand" runat="server" SkinID="Transparent" DataMember="VoidInvoicePanel"	DataSourceID="ds">
			<ContentStyle BackColor="Transparent" BorderStyle="None"></ContentStyle>
			<Template>
				<px:PXTextEdit LabelWidth="70px" CommitChanges="True" Height="60px" Width="" runat="server" ID="CstPXTextEdit33" DataField="VoidReason" TextMode="MultiLine" ></px:PXTextEdit></Template></px:PXFormView>
		<px:PXPanel runat="server" ID="CstPanel9" SkinID="Buttons">
			<px:PXButton ID="OK" runat="server" DialogResult="OK" Text="OK" ></px:PXButton>
                  <px:PXButton ID="Cancel" runat="server" DialogResult="Cancel" Text="Cancel" ></px:PXButton></px:PXPanel></px:PXSmartPanel></asp:Content>