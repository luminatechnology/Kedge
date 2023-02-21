<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="GV104000.aspx.cs" Inherits="Page_GV104000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/ListView.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" PrimaryView="guiWords" SuspendUnloading="False" TypeName="RCGV.GV.GVGuiWordMaint" >
		<CallbackCommands>
			<px:PXDSCallbackCommand Name="Insert" Visible="False">
			</px:PXDSCallbackCommand>
			<px:PXDSCallbackCommand Name="Delete" Visible="False">
			</px:PXDSCallbackCommand>
			<px:PXDSCallbackCommand Name="CopyPaste" Visible="False">
			</px:PXDSCallbackCommand>
			<px:PXDSCallbackCommand Name="First" Visible="False">
			</px:PXDSCallbackCommand>
			<px:PXDSCallbackCommand Name="Previous" Visible="False">
			</px:PXDSCallbackCommand>
			<px:PXDSCallbackCommand Name="Next" Visible="False">
			</px:PXDSCallbackCommand>
			<px:PXDSCallbackCommand Name="Last" Visible="False">
			</px:PXDSCallbackCommand>
		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phL" runat="Server">
	<px:PXGrid ID="grid" runat="server" Height="400px" Width="100%" Style="z-index: 100; margin-right: 0px;"
		AllowPaging="True" AllowSearch="True" AdjustPageSize="Auto" DataSourceID="ds" TabIndex="2100" SkinID="Primary" SyncPosition="True" MatrixMode="True">
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
			<px:PXGridLevel DataMember="guiWords" DataKeyNames="RegistrationCD,DeclareYear,GuiWordID">
				<RowTemplate>
					<px:PXNumberEdit ID="edDeclareYear" runat="server" AlreadyLocalized="False" DataField="DeclareYear" DefaultLocale="" Size="XM" CommitChanges="True">
					</px:PXNumberEdit>
					<px:PXDropDown ID="edDeclarePeriod" runat="server" DataField="DeclarePeriod" Size="XM" CommitChanges="True">
					</px:PXDropDown>
					<px:PXTextEdit ID="edGuiWordCD" runat="server" AlreadyLocalized="False" DataField="GuiWordCD" Size="XM" CommitChanges="True">
					</px:PXTextEdit>
					<px:PXDropDown ID="edGuiType" runat="server" DataField="GuiType" Size="XM" CommitChanges="True">
					</px:PXDropDown>
					<px:PXCheckBox ID="edIsEInvoice" runat="server" AlreadyLocalized="False" DataField="IsEInvoice" Text="IsEInvoice">
					</px:PXCheckBox>
				</RowTemplate>
				<Columns>
					<px:PXGridColumn DataField="DeclareYear" TextAlign="Left" CommitChanges="True">
					</px:PXGridColumn>
					<px:PXGridColumn DataField="GuiWordCD" Width="100px" CommitChanges="True">
					</px:PXGridColumn>
					<px:PXGridColumn DataField="DeclarePeriod" CommitChanges="True">
					</px:PXGridColumn></Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="200" />
		<ActionBar>
			<Actions>
				<Delete MenuVisible="False" />
			</Actions>
		</ActionBar>
		<Mode AllowFormEdit="True" AllowUpload="True" />
	</px:PXGrid>
</asp:Content>