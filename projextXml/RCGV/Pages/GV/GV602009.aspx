<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="GV602009.aspx.cs" Inherits="Page_GV602009" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormView.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="RCGV.GV.GVMediaDeclaration" PrimaryView="ARAPINVfilter">
		<CallbackCommands></CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="ARAPINVfilter" Width="100%" AllowAutoHide="false" Height="90px">
		<Template>
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartRow="True"></px:PXLayoutRule>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule3" StartColumn="True" ControlSize="M" ></px:PXLayoutRule>
			<px:PXSelector runat="server" ID="CstPXSelector7" DataField="RegistrationCD" ></px:PXSelector>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit1" DataField="DeclareBatchNbr" CommitChanges="true"></px:PXTextEdit>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule13" StartColumn="True" ></px:PXLayoutRule>
			<px:PXGroupBox runat="server" ID="CstGroupBox11" DataField="GvType" Caption="GvType" RenderStyle="Simple">
				<Template>
					<px:PXLayoutRule runat="server" ID="CstPXLayoutRule1" StartColumn="True" ></px:PXLayoutRule>
					<px:PXLabel runat="server" ID="CstLabel2" Text="Gv Type" ></px:PXLabel>
					<px:PXLayoutRule runat="server" ID="CstPXLayoutRule12" StartColumn="True" ></px:PXLayoutRule>
					<px:PXRadioButton runat="server" ID="CstRadioButton15" Text="I/O" Value="IO" ></px:PXRadioButton>
					<px:PXRadioButton runat="server" ID="CstRadioButton16" Text="I" Value="I" ></px:PXRadioButton>
					<px:PXRadioButton runat="server" ID="CstRadioButton17" Text="O" Value="O" ></px:PXRadioButton>
				</Template>
			</px:PXGroupBox>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule4" StartColumn="True" LabelsWidth="150px" ></px:PXLayoutRule>
			<px:PXNumberEdit runat="server" ID="CstPXNumberEdit6" DataField="DeclareYearFrom" CommitChanges="true"></px:PXNumberEdit>
			<px:PXDropDown CommitChanges="True" runat="server" ID="CstPXDropDown4" DataField="Period" ></px:PXDropDown></Template>
	</px:PXFormView>
	<px:PXGrid DataSourceID="ds" SkinID="Details" Width="100%" SyncPosition="True" runat="server" ID="CstPXGrid2">
		<Levels>
			<px:PXGridLevel DataMember="DataViews" >
				<Columns>
					<px:PXGridColumn CommitChanges="True" DataField="Selected" Width="40" Type="CheckBox" AllowCheckAll="True" TextAlign="Center" ></px:PXGridColumn>
					<px:PXGridColumn DataField="GvType" Width="70" ></px:PXGridColumn>
					<px:PXGridColumn LinkCommand="ViewGuiInvoice" DataField="GuiInvoiceNbr" Width="140" ></px:PXGridColumn>
					<px:PXGridColumn LinkCommand="ViewAllowance" DataField="GuiCmInvoiceNbr" Width="140" ></px:PXGridColumn>
					<px:PXGridColumn DataField="GuiType" Width="70" ></px:PXGridColumn>
					<px:PXGridColumn DataField="InvoiceDate" Width="90" ></px:PXGridColumn>
					<px:PXGridColumn DataField="SalesAmt" Width="100" ></px:PXGridColumn>
					<px:PXGridColumn DataField="TaxAmt" Width="100" ></px:PXGridColumn>
					<px:PXGridColumn DataField="TotalAmt" Width="100" ></px:PXGridColumn>
					<px:PXGridColumn DataField="DeclareBatchNbr" Width="140" ></px:PXGridColumn></Columns></px:PXGridLevel></Levels>
		<AutoSize MinHeight="300" Container="Window" Enabled="true" ></AutoSize>
		<ActionBar>
			<Actions>
				<AddNew Enabled="False" ></AddNew></Actions></ActionBar>
		<ActionBar>
			<Actions>
				<Delete Enabled="False" ></Delete></Actions></ActionBar>
		<ActionBar>
			<Actions>
				<Delete MenuVisible="False" ></Delete></Actions></ActionBar>
		<ActionBar>
			<Actions>
				<AddNew MenuVisible="False" ></AddNew></Actions></ActionBar></px:PXGrid></asp:Content>