<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="CC301001.aspx.cs" Inherits="Page_CC301001" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormView.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="CC.CCReceivableEntry"
        PrimaryView="ReceivableChecks"
        >
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="ReceivableChecks" Width="100%" AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartRow="True"></px:PXLayoutRule>
			<px:PXLayoutRule ControlSize="XL" runat="server" ID="CstPXLayoutRule11" StartColumn="True" ></px:PXLayoutRule>
			<px:PXSelector runat="server" ID="CstPXSelector55" DataField="GuarReceviableCD" ></px:PXSelector>
			<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit2" DataField="DocDate" ></px:PXDateTimeEdit>
			<px:PXDropDown runat="server" ID="CstPXDropDown56" DataField="Status" ></px:PXDropDown>
			<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit63" DataField="IssueDate" ></px:PXDateTimeEdit>
			<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit50" DataField="AuthDate" ></px:PXDateTimeEdit>
			<px:PXGroupBox Caption="Customer/Vendor" RenderStyle="Simple" runat="server" ID="TargetType" DataField="TargetType" CommitChanges="True">
				<Template>
					<px:PXLayoutRule runat="server" ID="CstPXLayoutRule67" StartColumn="True" ></px:PXLayoutRule>
					<px:PXRadioButton Text="Customer" runat="server" ID="Customer" Value="C" GroupName="TargetType" Checked="True" ></px:PXRadioButton>
					<px:PXLayoutRule runat="server" ID="CstLayoutRule42" StartColumn="True" ></px:PXLayoutRule>
					<px:PXRadioButton Text="Vendor" runat="server" ID="Vendor" Checked="False" Value="V" GroupName="TargetType" ></px:PXRadioButton></Template></px:PXGroupBox>
			<px:PXSegmentMask CommitChanges="True" runat="server" ID="CstPXSegmentMask61" DataField="VendorID" ></px:PXSegmentMask>
			<px:PXSegmentMask CommitChanges="True" runat="server" ID="CstPXSegmentMask62" DataField="VendorLocationID" ></px:PXSegmentMask>
			<px:PXSegmentMask CommitChanges="True" runat="server" ID="CstPXSegmentMask59" DataField="CustomerID" ></px:PXSegmentMask>
			<px:PXSegmentMask CommitChanges="True" runat="server" ID="CstPXSegmentMask60" DataField="CustomerLocationID" ></px:PXSegmentMask>
			<px:PXSegmentMask CommitChanges="True" runat="server" ID="CstPXNumberEdit10" DataField="ProjectID" ></px:PXSegmentMask >
			<px:PXSelector CommitChanges="True" AutoRefresh="True" runat="server" ID="CstPXSelector68" DataField="PONbr" ></px:PXSelector>
			<px:PXNumberEdit runat="server" ID="CstPXNumberEdit76" DataField="CCPostageAmt" CommitChanges="True" />
			<px:PXLayoutRule runat="server" ID="CstLayoutRule29" ColumnSpan="2" ></px:PXLayoutRule>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit64" DataField="Description" TextMode="MultiLine" Height="90px" ></px:PXTextEdit>
			<px:PXLayoutRule ControlSize="XL" runat="server" ID="CstPXLayoutRule12" StartColumn="True" ></px:PXLayoutRule>
			<px:PXDropDown runat="server" ID="CstPXDropDown58" DataField="GuarType" ></px:PXDropDown>
			<px:PXDropDown runat="server" ID="CstPXDropDown57" DataField="GuarClass" ></px:PXDropDown>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit15" DataField="GuarNbr" ></px:PXTextEdit>
			<px:PXNumberEdit runat="server" ID="CstPXNumberEdit16" DataField="GuarAmt" ></px:PXNumberEdit>
			<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit17" DataField="DueDate" ></px:PXDateTimeEdit>
			<px:PXCheckBox CommitChanges="True" runat="server" ID="CstPXCheckBox69" DataField="IsPostpone" ></px:PXCheckBox>
			<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit70" DataField="PostDueDate" ></px:PXDateTimeEdit>
			<px:PXSelector runat="server" ID="CstPXSelector73" DataField="OriBankCode" ></px:PXSelector>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit75" DataField="OriBankAccount" ></px:PXTextEdit>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit74" DataField="CheckIssuer" ></px:PXTextEdit>
			<px:PXSegmentMask runat="server" ID="CstPXNumberEdit18" DataField="ContractorID" ></px:PXSegmentMask >
			<px:PXSegmentMask runat="server" ID="CstPXNumberEdit19" DataField="CashierID" ></px:PXSegmentMask >
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule43" StartRow="True" ></px:PXLayoutRule>
                        <px:PXLayoutRule ControlSize="XL" runat="server" ID="CstPXLayoutRule47" StartColumn="True" ></px:PXLayoutRule>
			<px:PXGroupBox Caption="Financial Details" RenderStyle="Fieldset" runat="server" ID="CstGroupBox44">
				<Template>
					<px:PXLayoutRule ControlSize="SM" runat="server" ID="CstPXLayoutRule45" StartColumn="True" ></px:PXLayoutRule>
					<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit48" DataField="GuarReleaseDate" ></px:PXDateTimeEdit>
					<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit49" DataField="GuarReturnDate" ></px:PXDateTimeEdit>
					<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit47" DataField="GuarReceiveDate" ></px:PXDateTimeEdit>
					<px:PXLayoutRule ControlSize="SM" runat="server" ID="CstPXLayoutRule46" StartColumn="True" ></px:PXLayoutRule>
					<px:PXTextEdit Enabled="False" runat="server" ID="CstPXTextEdit52" DataField="BatchNbr" >
						<LinkCommand Command="viewGLVoucherByRelease" ></LinkCommand>
						<LinkCommand Target="ds" ></LinkCommand></px:PXTextEdit>
					<px:PXTextEdit Enabled="False" runat="server" ID="CstPXTextEdit53" DataField="ReturnBatchNbr" >
						<LinkCommand Command="viewGLVoucherByReturn" ></LinkCommand>
						<LinkCommand Target="ds" ></LinkCommand></px:PXTextEdit></Template></px:PXGroupBox>
                        <px:PXLayoutRule ControlSize="XL" runat="server" ID="CstPXLayoutRule48" StartColumn="True" ></px:PXLayoutRule>
			<px:PXGroupBox RenderStyle="Fieldset" runat="server" ID="CstGroupBox71" Caption="Postpone Log" >
				<Template>
					<px:PXGrid Height="300px" runat="server" ID="CstPXGrid72">
						<Levels>
							<px:PXGridLevel DataMember="PostponeLogs">
								<Columns>
									<px:PXGridColumn DataField="PostponeSeq" Width="70" ></px:PXGridColumn>
									<px:PXGridColumn DataField="PostponeDate" Width="90" ></px:PXGridColumn>
									<px:PXGridColumn DataField="CreatedByID_Creator_displayName" Width="70" ></px:PXGridColumn></Columns></px:PXGridLevel></Levels>
						<AutoSize MinHeight="150" ></AutoSize>
						<AutoSize Enabled="True" ></AutoSize></px:PXGrid></Template></px:PXGroupBox></Template>
		<AutoSize Container="Window" Enabled="True" MinHeight="200" ></AutoSize>
	</px:PXFormView></asp:Content>