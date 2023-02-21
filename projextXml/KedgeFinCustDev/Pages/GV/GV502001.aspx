<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="GV502001.aspx.cs" Inherits="Page_GV502001" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="GV.GVArGuiInvoiceProcess"
        PrimaryView="MasterView"
        >
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="MasterView" Width="100%" Height="160px" AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartRow="True"></px:PXLayoutRule>
			<px:PXLayoutRule ControlSize="" ColumnWidth="" runat="server" ID="CstPXLayoutRule1" StartColumn="True" ></px:PXLayoutRule>
			<px:PXGroupBox RenderStyle="Fieldset" Caption="Invoice Info" runat="server" ID="CstGroupBox3" >
				<Template>
					<px:PXLayoutRule runat="server" ID="CstPXLayoutRule14" StartColumn="True" ControlSize="M" ></px:PXLayoutRule>
					<px:PXDateTimeEdit CommitChanges="True" runat="server" ID="CstPXDateTimeEdit18" DataField="InvoiceDate" ></px:PXDateTimeEdit>
					<px:PXSelector AutoRefresh="True" runat="server" ID="CstPXSelector16" DataField="GuiBookID" CommitChanges="True" ></px:PXSelector>
					<px:PXCheckBox runat="server" ID="CstPXCheckBox17" DataField="AutoConfirm" CommitChanges="False" ></px:PXCheckBox></Template></px:PXGroupBox>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule2" StartColumn="True" ></px:PXLayoutRule>
			<px:PXGroupBox RenderStyle="Fieldset" Caption="Filter" runat="server" ID="CstGroupBox4" >
				<Template>
					<px:PXLayoutRule runat="server" ID="CstPXLayoutRule6" StartColumn="True" ></px:PXLayoutRule>
					<px:PXTextEdit CommitChanges="True" runat="server" ID="CstPXTextEdit8" DataField="RefNbrFrom" ></px:PXTextEdit>
					<px:PXDateTimeEdit CommitChanges="True" runat="server" ID="CstPXDateTimeEdit9" DataField="DocDateFrom" ></px:PXDateTimeEdit>
					<px:PXSegmentMask CommitChanges="True" runat="server" ID="CstPXSegmentMask10" DataField="ProjectID" ></px:PXSegmentMask>
					<px:PXLayoutRule runat="server" ID="CstPXLayoutRule7" StartColumn="True" ></px:PXLayoutRule>
					<px:PXTextEdit CommitChanges="True" runat="server" ID="CstPXTextEdit12" DataField="RefNbrTo" ></px:PXTextEdit>
					<px:PXDateTimeEdit CommitChanges="True" runat="server" ID="CstPXDateTimeEdit11" DataField="DocDateTo" ></px:PXDateTimeEdit>
					<px:PXSegmentMask CommitChanges="True" runat="server" ID="CstPXSegmentMask13" DataField="CustomerID" ></px:PXSegmentMask></Template></px:PXGroupBox></Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXGrid ExportNotes="False" SyncPosition="True" ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="Details" AllowAutoHide="false">
		<Levels>
			<px:PXGridLevel DataMember="DetailsView">
			    <Columns>
				<px:PXGridColumn AllowCheckAll="True" Type="CheckBox" DataField="Selected" Width="60" ></px:PXGridColumn>
				<px:PXGridColumn LinkCommand="viewInv" DataField="RefNbr" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="DocDate" Width="90" ></px:PXGridColumn>
				<px:PXGridColumn DataField="ProjectID" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="ProjectID_description" Width="280" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CustomerID" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CustomerID_BAccountR_acctName" Width="280" ></px:PXGridColumn>
				<px:PXGridColumn DataField="DocDesc" Width="280" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CuryVatTaxableTotal" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Balance" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CuryTaxTotal" Width="100" ></px:PXGridColumn></Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
		<ActionBar >
		
			<Actions>
				<AddNew ToolBarVisible="False" Enabled="False" MenuVisible="False" ></AddNew>
						<Delete ToolBarVisible="False" ></Delete></Actions>
			<Actions>
				<Delete MenuVisible="False" ></Delete></Actions></ActionBar>
	
		<Mode AllowAddNew="False" ></Mode>
		<Mode AllowDelete="False" ></Mode></px:PXGrid>
</asp:Content>