<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="TW505000.aspx.cs" Inherits="Page_TW505000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="Fin.Graph.TWNGenWHTFile"
        PrimaryView="Filter"
        >
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="Filter" Width="100%" Height="150px" AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule1" StartRow="True" ></px:PXLayoutRule>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule4" StartColumn="True" ></px:PXLayoutRule>
			<px:PXPanel Caption="Filter" runat="server" ID="CstPanel18">
				<px:PXLayoutRule runat="server" ID="CstPXLayoutRule19" StartColumn="True" ></px:PXLayoutRule>
				<px:PXSelector runat="server" ID="CstPXSelector21" DataField="BranchID" CommitChanges="True" ></px:PXSelector>
				<px:PXNumberEdit runat="server" ID="CstPXNumberEdit34" DataField="WithHoldingYearFrom" CommitChanges="True" ></px:PXNumberEdit>
				<px:PXDropDown runat="server" ID="CstPXDropDown26" DataField="PayMonthFrom" CommitChanges="True" ></px:PXDropDown>
				<px:PXLayoutRule runat="server" ID="CstPXLayoutRule20" StartColumn="True" ></px:PXLayoutRule>
				<px:PXTextEdit runat="server" ID="CstPXTextEdit36" DataField="TaxRegistrationID" />
				<px:PXNumberEdit runat="server" ID="CstPXNumberEdit35" DataField="WithHoldingYearTo" CommitChanges="True" ></px:PXNumberEdit>
				<px:PXDropDown runat="server" ID="CstPXDropDown28" DataField="PayMonthTo" CommitChanges="True" ></px:PXDropDown></px:PXPanel>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule29" StartColumn="True" ></px:PXLayoutRule>
			<px:PXPanel runat="server" ID="CstPanel30" Caption="Count">
				<px:PXNumberEdit runat="server" ID="CstPXNumberEdit31" DataField="SelectedCount" ></px:PXNumberEdit>
				<px:PXNumberEdit runat="server" ID="CstPXNumberEdit33" DataField="TotalWHTAmt" ></px:PXNumberEdit>
				<px:PXNumberEdit runat="server" ID="CstPXNumberEdit37" DataField="TotalGNHI2Amt" />
				<px:PXNumberEdit runat="server" ID="CstPXNumberEdit32" DataField="TotalPayAmt" ></px:PXNumberEdit></px:PXPanel></Template>
	</px:PXFormView></asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXGrid SyncPosition="True" AutoAdjustColumns="True" ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="" SkinID="Inquire" AllowAutoHide="false">
		<Levels>
			<px:PXGridLevel DataMember="WHTTranProc">
			    <Columns>
				<px:PXGridColumn DataField="WHTFmtCode" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="PersonalID" Width="120" ></px:PXGridColumn>
				<px:PXGridColumn DataField="PropertyID" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="PaymentDate" Width="90" ></px:PXGridColumn>
				<px:PXGridColumn DataField="TypeOfIn" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="PayeeName" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="PayeeAddr" Width="220" ></px:PXGridColumn>
				<px:PXGridColumn DataField="WHTAmt" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="GNHI2Amt" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="NetAmt" Width="100" ></px:PXGridColumn></Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
		<ActionBar >
		</ActionBar>
	</px:PXGrid>
</asp:Content>