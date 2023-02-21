<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="GV501001.aspx.cs" Inherits="Page_GV501001" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="GV.GVApGuiAllowanceInvoiceProcess"
        PrimaryView="MasterView"
        >
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="MasterView" Width="100%" Height="180px" AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartRow="True"></px:PXLayoutRule>
                         <px:PXLayoutRule runat="server" ID="CstPXLayoutRule52" StartColumn="True" ></px:PXLayoutRule>
			<px:PXGroupBox RenderStyle="Fieldset" runat="server" ID="CstGroupBox80" Caption="Allowance Invoice Info">
				<Template>
					<px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector82" DataField="GuiType" ></px:PXSelector></Template></px:PXGroupBox>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule53" StartColumn="True" ></px:PXLayoutRule>
			<px:PXGroupBox RenderStyle="Fieldset" Caption="Filter" runat="server" ID="CstGroupBox81" >
				<Template>
					<px:PXLayoutRule runat="server" ID="CstPXLayoutRule83" StartColumn="True" ></px:PXLayoutRule>
					<px:PXTextEdit CommitChanges="True" runat="server" ID="CstPXTextEdit85" DataField="RefNbrFrom" ></px:PXTextEdit>
					<px:PXDateTimeEdit CommitChanges="True" runat="server" ID="CstPXDateTimeEdit87" DataField="DocDateFrom" ></px:PXDateTimeEdit>
					<px:PXSegmentMask CommitChanges="True" runat="server" ID="CstPXSegmentMask89" DataField="ProjectID" ></px:PXSegmentMask>
					<px:PXLayoutRule runat="server" ID="CstPXLayoutRule84" StartColumn="True" ></px:PXLayoutRule>
					<px:PXTextEdit CommitChanges="True" runat="server" ID="CstPXTextEdit86" DataField="RefNbrTo" ></px:PXTextEdit>
					<px:PXDateTimeEdit CommitChanges="True" runat="server" ID="CstPXDateTimeEdit88" DataField="DocDateTo" ></px:PXDateTimeEdit>
					<px:PXSegmentMask CommitChanges="True" runat="server" ID="CstPXSegmentMask90" DataField="VendorID" ></px:PXSegmentMask></Template></px:PXGroupBox></Template>
	
		<AutoSize Container="Parent" ></AutoSize></px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXGrid SyncPosition="True" ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="Details" AllowAutoHide="false">
		<Levels>
			<px:PXGridLevel DataMember="DetailsView">
			    <Columns>
				<px:PXGridColumn Type="CheckBox" AllowCheckAll="True" DataField="Selected" Width="60" ></px:PXGridColumn>
				<px:PXGridColumn DataField="UsrAccConfirmNbr" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn LinkCommand="viewDebitAdj" DataField="RefNbr" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn LinkCommand="viewInv" DataField="OriDecutionRefNbr" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="DocDate" Width="90" ></px:PXGridColumn>
				<px:PXGridColumn DataField="ProjectID" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="VendorID" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="GuiInvoiceNbr" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="TotalAmt" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CuryLineTotal" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CuryTaxTotal" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CuryOrigDocAmt" Width="100" ></px:PXGridColumn></Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
		<ActionBar >
		</ActionBar>
	</px:PXGrid>
</asp:Content>