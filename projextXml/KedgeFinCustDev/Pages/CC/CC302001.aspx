<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="CC302001.aspx.cs" Inherits="Page_CC302001" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormView.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="CC.CCPayableEntry"
        PrimaryView="PayableChecks"
        >
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="PayableChecks" Width="100%" AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartRow="True"></px:PXLayoutRule>
			<px:PXLayoutRule ControlSize="XL" runat="server" ID="CstPXLayoutRule30" StartColumn="True" ></px:PXLayoutRule>
			<px:PXSelector runat="server" ID="CstPXSelector71" DataField="GuarPayableCD" ></px:PXSelector>
			<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit3" DataField="DocDate" ></px:PXDateTimeEdit>
			<%--<px:PXCheckBox CommitChanges="True" runat="server" ID="CstPXCheckBox77" DataField="Hold" ></px:PXCheckBox>--%>
			<px:PXDropDown runat="server" ID="CstPXDropDown63" DataField="Status" ></px:PXDropDown>
			<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit5" DataField="IssueDate" ></px:PXDateTimeEdit>
			<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit73" DataField="AuthDate" ></px:PXDateTimeEdit>
			<px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector6" DataField="BankAccountID" ></px:PXSelector>
			<px:PXSelector runat="server" ID="CstPXSelector58" DataField="BankCode" ></px:PXSelector>
			<px:PXGroupBox runat="server" ID="TargetType" DataField="TargetType" Caption="Customer/Vendor" CommitChanges="True" RenderStyle="Simple" >
				<Template>
					<px:PXLayoutRule runat="server" ID="CstPXLayoutRule72" StartColumn="True" ></px:PXLayoutRule>
					<px:PXRadioButton Checked="True" runat="server" ID="Customer" Text="Customer" Value="C" GroupName="TargetType" ></px:PXRadioButton>
					<px:PXLayoutRule runat="server" ID="CstLayoutRule11" StartColumn="True" ></px:PXLayoutRule>
					<px:PXRadioButton runat="server" ID="Vendor" Value="V" ></px:PXRadioButton></Template></px:PXGroupBox>
			<px:PXSegmentMask CommitChanges="True" runat="server" ID="CstPXSegmentMask64" DataField="CustomerID" ></px:PXSegmentMask>
			<px:PXSegmentMask runat="server" ID="CstPXSegmentMask65" DataField="CustomerLocationID" ></px:PXSegmentMask>
			<px:PXSegmentMask CommitChanges="True" runat="server" ID="CstPXSegmentMask66" DataField="VendorID" ></px:PXSegmentMask>
			<px:PXSegmentMask runat="server" ID="CstPXSegmentMask67" DataField="VendorLocationID" ></px:PXSegmentMask>
			<px:PXSegmentMask CommitChanges="True" runat="server" ID="CstProjectID" DataField="ProjectID" ></px:PXSegmentMask >
			<px:PXLayoutRule runat="server" ID="CstLayoutRule20" ColumnSpan="2" ></px:PXLayoutRule>
			<px:PXTextEdit TextMode="MultiLine" Height="90px" runat="server" ID="CstPXTextEdit57" DataField="Description" ></px:PXTextEdit>
			<px:PXLabel runat="server" ID="CstLabel60" ></px:PXLabel>
			<px:PXLayoutRule ControlSize="XL" runat="server" ID="CstPXLayoutRule31" StartColumn="True" ></px:PXLayoutRule>
			<px:PXDropDown runat="server" ID="CstPXDropDown61" DataField="GuarType" ></px:PXDropDown>
			<px:PXDropDown CommitChanges="True" runat="server" ID="CstPXDropDown62" DataField="GuarClass" ></px:PXDropDown>
			<px:PXSelector CommitChanges="True" AutoRefresh="True" runat="server" ID="CstPXSelector75" DataField="BookCD" ></px:PXSelector>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit23" DataField="GuarNbr" ></px:PXTextEdit>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit24" DataField="GuarTitle" ></px:PXTextEdit>
			<px:PXNumberEdit runat="server" ID="CstPXNumberEdit25" DataField="GuarAmt" ></px:PXNumberEdit>
			<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit26" DataField="DueDate" ></px:PXDateTimeEdit>
			<px:PXSegmentMask CommitChanges="True" runat="server" ID="CstPXSegmentMask70" DataField="ContractorID" ></px:PXSegmentMask>
			<px:PXSelector runat="server" ID="CstPXSelector68" DataField="DepID" ></px:PXSelector>
			<px:PXSegmentMask runat="server" ID="CstPXSegmentMask69" DataField="CashierID" ></px:PXSegmentMask>
			<px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector74" DataField="PONbr" ></px:PXSelector>
			<px:PXNumberEdit runat="server" ID="CstPXNumberEdit76" DataField="CCPostageAmt" />
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule1" StartRow="True" ></px:PXLayoutRule>
			<px:PXGroupBox RenderStyle="Fieldset" runat="server" ID="CstGroupBox49" Caption="Financial Details">
				<Template>
					<px:PXLayoutRule runat="server" ID="CstPXLayoutRule50" StartColumn="True" ></px:PXLayoutRule>
					<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit52" DataField="GuarReleaseDate" ></px:PXDateTimeEdit>
					<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit54" DataField="GuarVoidDate" ></px:PXDateTimeEdit>
					<px:PXLayoutRule runat="server" ID="CstPXLayoutRule51" StartColumn="True" ></px:PXLayoutRule>
					<px:PXTextEdit Enabled="False" runat="server" ID="CstPXTextEdit55" DataField="GuarReleaseNbr" >
						<LinkCommand Command="viewGLVoucherByRelease" ></LinkCommand>
						<LinkCommand Target="ds" ></LinkCommand></px:PXTextEdit>
					<px:PXTextEdit Enabled="False" runat="server" ID="CstPXTextEdit56" DataField="GuarVoidNbr" >
						<LinkCommand Target="ds" Command="viewGLVoucherByVoid" ></LinkCommand>
						<LinkCommand Target="ds" ></LinkCommand></px:PXTextEdit></Template></px:PXGroupBox></Template>
		<AutoSize Container="Window" Enabled="True" MinHeight="200" ></AutoSize>
	</px:PXFormView>
</asp:Content>