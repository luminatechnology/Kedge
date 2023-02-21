<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="GV402003.aspx.cs" Inherits="Page_GV402003" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="RCGV.GV.GVArGuiZeroDocInq" PrimaryView="gVArGuiZeroDocFilters">
		<CallbackCommands>
			<px:PXDSCallbackCommand Name="ViewGVArGuiZeroDoc" Visible="False" DependOnGrid="grid">
			</px:PXDSCallbackCommand>
		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" 
		Width="100%" DataMember="gVArGuiZeroDocFilters" TabIndex="3900">
		<Template>
			<px:PXLayoutRule runat="server" StartRow="True" StartColumn="True"></px:PXLayoutRule>
			<px:PXLayoutRule runat="server" ColumnSpan="2">
			</px:PXLayoutRule>
			<px:PXSelector ID="edRegistrationCD" runat="server" DataField="RegistrationCD" CommitChanges="True">
			</px:PXSelector>
			<px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector1" DataField="ZeroDocCDFrom" ></px:PXSelector>
			<px:PXSelector ID="edDocNbr" runat="server" DataField="DocNbr" CommitChanges="True">
			</px:PXSelector>
			<px:PXDropDown ID="edSalesType" runat="server" DataField="SalesType" CommitChanges="True">
			</px:PXDropDown>
			<px:PXDateTimeEdit ID="edDocDateFrom" runat="server" AlreadyLocalized="False" DataField="DocDateFrom" CommitChanges="True">
			</px:PXDateTimeEdit>
			<px:PXLayoutRule runat="server" StartColumn="True">
			</px:PXLayoutRule>
			<px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector2" DataField="ZeroDocCDTo" ></px:PXSelector>
			<px:PXDropDown ID="edDocType" runat="server" DataField="DocType" CommitChanges="True">
			</px:PXDropDown>
			<px:PXDropDown ID="edDataType" runat="server" DataField="DataType" CommitChanges="True">
			</px:PXDropDown>
			<px:PXDateTimeEdit ID="edDocDateTo" runat="server" AlreadyLocalized="False" DataField="DocDateTo" CommitChanges="True">
			</px:PXDateTimeEdit></Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXGrid ID="grid" runat="server" DataSourceID="ds" Style="z-index: 100" 
		Width="100%" Height="150px" SkinID="Details" TabIndex="1300">
<EmptyMsg ComboAddMessage="No records found.
Try to change filter or modify parameters above to see records here." NamedComboMessage="No records found as &#39;{0}&#39;.
Try to change filter or modify parameters above to see records here." NamedComboAddMessage="No records found as &#39;{0}&#39;.
Try to change filter or modify parameters above to see records here." FilteredMessage="No records found.
Try to change filter to see records here." FilteredAddMessage="No records found.
Try to change filter to see records here." NamedFilteredMessage="No records found as &#39;{0}&#39;.
Try to change filter to see records here." NamedFilteredAddMessage="No records found as &#39;{0}&#39;.
Try to change filter to see records here." AnonFilteredMessage="No records found.
Try to change filter to see records here." AnonFilteredAddMessage="No records found.
Try to change filter to see records here."></EmptyMsg>
		<Levels>
			<px:PXGridLevel DataKeyNames="ZeroDocID,ZeroDocCD" DataMember="gvArGuiZeroDocs">
				<Columns>
					<px:PXGridColumn DataField="ZeroDocCD" LinkCommand="ViewGvArGuiZeroDoc">
					</px:PXGridColumn>
					<px:PXGridColumn DataField="DocNbr" Width="120px">
					</px:PXGridColumn>
					<px:PXGridColumn DataField="DocDate" Width="90px">
					</px:PXGridColumn>
					<px:PXGridColumn DataField="DocType">
					</px:PXGridColumn>
					<px:PXGridColumn DataField="SalesType">
					</px:PXGridColumn>
					<px:PXGridColumn DataField="DataType">
					</px:PXGridColumn>
					<px:PXGridColumn DataField="SalesAmt" TextAlign="Right" Width="100px">
					</px:PXGridColumn>
				</Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" />
	</px:PXGrid>
</asp:Content>
