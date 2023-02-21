<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="GV101000.aspx.cs" Inherits="Page_GV101000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/ListView.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" TypeName="RCGV.GV.GVOrderTypeMappingMaint" PrimaryView="OrderTypes" SuspendUnloading="False" >
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phL" runat="Server">
	<px:PXGrid ID="grid" runat="server" Height="400px" Width="100%" Style="z-index: 100"
		AllowPaging="True" AllowSearch="True" AdjustPageSize="Auto" DataSourceID="ds" SkinID="Primary" TabIndex="100" SyncPosition="True" MatrixMode="True">
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
			<px:PXGridLevel DataMember="OrderTypes" DataKeyNames="OrderTypeMappingID">
				<RowTemplate>
					<px:PXLayoutRule runat="server" StartRow="True" ControlSize="XM">
					</px:PXLayoutRule>
					<px:PXDropDown ID="edGvType" runat="server" CommitChanges="True" DataField="GvType">
					</px:PXDropDown>
					<px:PXDropDown ID="edOrderTypeCD" runat="server" DataField="OrderTypeCD" CommitChanges="True">
					</px:PXDropDown>
					<px:PXDropDown ID="edGuiSubType" runat="server" DataField="GuiSubType" CommitChanges="True">
					</px:PXDropDown>
					<px:PXDateTimeEdit ID="edEffectiveDate" runat="server" AlreadyLocalized="False" DataField="EffectiveDate">
					</px:PXDateTimeEdit>
					<px:PXDateTimeEdit ID="edExpirationDate" runat="server" AlreadyLocalized="False" DataField="ExpirationDate">
					</px:PXDateTimeEdit>
				</RowTemplate>
				<Columns>
					<px:PXGridColumn DataField="GvType" CommitChanges="True">
					</px:PXGridColumn>
					<px:PXGridColumn DataField="OrderTypeCD" CommitChanges="True">
					</px:PXGridColumn>
					<px:PXGridColumn DataField="GuiSubType" CommitChanges="True">
					</px:PXGridColumn>
					<px:PXGridColumn Type="CheckBox" DataField="IsActive" Width="60" ></px:PXGridColumn></Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="200" ></AutoSize>
		<Mode AllowFormEdit="True" AllowUpload="False" InitNewRow="true" ></Mode>
	</px:PXGrid>
</asp:Content>
