<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormTab.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="KG506001.aspx.cs" Inherits="Page_KG506001" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormTab.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="Kedge.Coms.KGComsUploadEntry"
        PrimaryView="MasterView"
        >
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="MasterView" Width="100%" Height="100px" AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartRow="True"></px:PXLayoutRule>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule1" StartColumn="True" ></px:PXLayoutRule>
			<px:PXNumberEdit runat="server" ID="CstPXNumberEdit11" DataField="UnBatchID" ></px:PXNumberEdit>
			<px:PXSelector runat="server" ID="CstPXSelector7" DataField="CreatedByID" ></px:PXSelector>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit12" DataField="Desc" />
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule2" StartColumn="True" ></px:PXLayoutRule>
			<px:PXNumberEdit runat="server" ID="CstPXNumberEdit8" DataField="VoucherCount" ></px:PXNumberEdit>
			<px:PXNumberEdit runat="server" ID="CstPXNumberEdit9" DataField="VoucherItemCount" ></px:PXNumberEdit>
			<px:PXNumberEdit runat="server" ID="CstPXNumberEdit10" DataField="InvoiceCount" ></px:PXNumberEdit></Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXTab ID="tab" runat="server" Width="100%" Height="150px" DataSourceID="ds" AllowAutoHide="false">
		<Items>
			<px:PXTabItem Text="Voucher">
				<Template>
					<px:PXGrid DataSourceID="ds" Width="100%" SyncPosition="True" Height="" SkinID="DetailsInTab" runat="server" ID="CstPXGrid3">
						<Levels>
							<px:PXGridLevel DataMember="Vouchers" >
								<Columns>
									<px:PXGridColumn DataField="ErrorMsg" Width="280" ></px:PXGridColumn>
									<px:PXGridColumn DataField="UniformNo" Width="180" ></px:PXGridColumn>
									<px:PXGridColumn DataField="SubCode" Width="140" ></px:PXGridColumn>
									<px:PXGridColumn DataField="Project" Width="70" ></px:PXGridColumn>
									<px:PXGridColumn DataField="Operator" Width="140" ></px:PXGridColumn>
									<px:PXGridColumn DataField="DrAmt" Width="100" ></px:PXGridColumn>
									<px:PXGridColumn DataField="CrAmt" Width="100" ></px:PXGridColumn>
									<px:PXGridColumn DataField="CompanyName" Width="70" ></px:PXGridColumn>
									<px:PXGridColumn DataField="Accuid" Width="140" ></px:PXGridColumn>
									<px:PXGridColumn DataField="VoucherNo" Width="140" ></px:PXGridColumn>
									<px:PXGridColumn DataField="VoucherDate" Width="84" ></px:PXGridColumn>
									<px:PXGridColumn DataField="CreateDate" Width="90" ></px:PXGridColumn>
									<px:PXGridColumn DataField="VoucherKey" Width="140" ></px:PXGridColumn></Columns></px:PXGridLevel></Levels>
						<Mode AllowUpload="True" ></Mode>
						<ActionBar>
							<Actions>
								<AddNew Enabled="False" ></AddNew></Actions></ActionBar>
						<AutoSize Enabled="True" MinHeight="150" Container="Window" ></AutoSize></px:PXGrid></Template>
			</px:PXTabItem>
			<px:PXTabItem Text="Voucher Item">
				<Template>
					<px:PXGrid DataSourceID="ds" SyncPosition="True" Width="100%" SkinID="DetailsInTab" runat="server" ID="CstPXGrid4">
						<Levels>
							<px:PXGridLevel DataMember="VoucherItems" >
								<Columns>
									<px:PXGridColumn DataField="ErrorMsg" Width="280" ></px:PXGridColumn>
									<px:PXGridColumn DataField="VoucherNo" Width="140" ></px:PXGridColumn>
									<px:PXGridColumn DataField="VoucherKey" Width="140" ></px:PXGridColumn>
									<px:PXGridColumn DataField="UniformNo" Width="180" ></px:PXGridColumn>
									<px:PXGridColumn DataField="Project" Width="70" ></px:PXGridColumn>
									<px:PXGridColumn DataField="Org" Width="70" ></px:PXGridColumn>
									<px:PXGridColumn DataField="Operator" Width="140" ></px:PXGridColumn>
									<px:PXGridColumn DataField="Nametw" Width="280" ></px:PXGridColumn>
									<px:PXGridColumn DataField="NameAbbr" Width="280" ></px:PXGridColumn>
									<px:PXGridColumn DataField="ItemNo" Width="100" ></px:PXGridColumn>
									<px:PXGridColumn DataField="Digest" Width="280" ></px:PXGridColumn>
									<px:PXGridColumn DataField="DateDue" Width="90" ></px:PXGridColumn>
									<px:PXGridColumn DataField="CompanyName" Width="70" ></px:PXGridColumn>
									<px:PXGridColumn DataField="Cd" Width="70" ></px:PXGridColumn>
									<px:PXGridColumn DataField="Amount" Width="100" ></px:PXGridColumn>
									<px:PXGridColumn DataField="AccUid" Width="140" ></px:PXGridColumn>
									<px:PXGridColumn DataField="AccountNo" Width="96" ></px:PXGridColumn></Columns></px:PXGridLevel></Levels>
						<ActionBar>
							<Actions>
								<AddNew Enabled="False" ></AddNew></Actions></ActionBar>
						<Mode AllowUpload="True" ></Mode>
						<AutoSize Enabled="True" ></AutoSize>
						<AutoSize MinHeight="150" ></AutoSize></px:PXGrid></Template>
			</px:PXTabItem>
			<px:PXTabItem Text="Invoice" >
				<Template>
					<px:PXGrid DataSourceID="ds" SyncPosition="True" Width="100%" SkinID="DetailsInTab" runat="server" ID="CstPXGrid5">
						<Levels>
							<px:PXGridLevel DataMember="Invoices" >
								<Columns>
									<px:PXGridColumn DataField="ErrorMsg" Width="280" ></px:PXGridColumn>
									<px:PXGridColumn DataField="VoucherNo" Width="140" ></px:PXGridColumn>
									<px:PXGridColumn DataField="VoucherKey" Width="140" ></px:PXGridColumn>
									<px:PXGridColumn DataField="UniformYy" Width="70" ></px:PXGridColumn>
									<px:PXGridColumn DataField="UniformNo" Width="120" ></px:PXGridColumn>
									<px:PXGridColumn DataField="UniformMm" Width="70" ></px:PXGridColumn>
									<px:PXGridColumn DataField="TaxCode" Width="70" ></px:PXGridColumn>
									<px:PXGridColumn DataField="TaxAmt" Width="100" ></px:PXGridColumn>
									<px:PXGridColumn DataField="Project" Width="70" ></px:PXGridColumn>
									<px:PXGridColumn DataField="IvoKind" Width="70" ></px:PXGridColumn>
									<px:PXGridColumn DataField="ItemNo" Width="100" ></px:PXGridColumn>
									<px:PXGridColumn DataField="InvoiceNo" Width="120" ></px:PXGridColumn>
									<px:PXGridColumn DataField="InvoiceDate" Width="90" ></px:PXGridColumn>
									<px:PXGridColumn DataField="CompanyName" Width="70" ></px:PXGridColumn>
									<px:PXGridColumn DataField="ClassCode" Width="70" ></px:PXGridColumn>
									<px:PXGridColumn DataField="BelongYy" Width="70" ></px:PXGridColumn>
									<px:PXGridColumn DataField="BelongMm" Width="70" ></px:PXGridColumn>
									<px:PXGridColumn DataField="Amount" Width="100" ></px:PXGridColumn>
									<px:PXGridColumn DataField="Accuid" Width="140" ></px:PXGridColumn></Columns></px:PXGridLevel></Levels>
						<ActionBar>
							<Actions>
								<AddNew Enabled="False" ></AddNew></Actions></ActionBar>
						<Mode AllowUpload="True" ></Mode>
						<AutoSize Enabled="True" ></AutoSize>
						<AutoSize MinHeight="150" ></AutoSize></px:PXGrid></Template></px:PXTabItem></Items>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
	</px:PXTab>
</asp:Content>