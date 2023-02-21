<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="KG505007.aspx.cs" Inherits="Page_KG505007" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="KedgeProjectCust.KGRefundRetainageProc" PrimaryView="Filter">
		<CallbackCommands>
		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="Filter" Width="100%" Height="70px" AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartRow="True"></px:PXLayoutRule>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule1" StartColumn="True" />
			<px:PXSegmentMask runat="server" ID="CstPXSegmentMask4" DataField="ProjectID" CommitChanges="true" />
			<px:PXSelector runat="server" ID="CstPXSelector5" DataField="UsrPONbr" CommitChanges="true" />
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule2" StartColumn="True" />
			<px:PXSegmentMask runat="server" ID="CstPXSegmentMask6" DataField="VendorID" CommitChanges="true" />
		</Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXGrid ID="grid" runat="server" DataSourceID="ds" Width="100%" SkinID="Inquire" AllowAutoHide="false" SyncPosition="true" AdjustPageSize="None" NoteIndicator="false" FilesIndicator="false">
		<Levels>
			<px:PXGridLevel DataMember="RefundRetainage">
			    <Columns>
				<px:PXGridColumn DataField="Selected" Width="60" Type="CheckBox" TextAlign="Center" AllowCheckAll="true" ></px:PXGridColumn>
				<px:PXGridColumn DataField="RefNbr" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="UsrPONbr" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="DocDate" Width="90" ></px:PXGridColumn>
				<px:PXGridColumn DataField="VendorID" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="VendorID_BAccountR_acctName" Width="280" ></px:PXGridColumn>
				<px:PXGridColumn DataField="ProjectID" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="ProjectID_description" Width="280" ></px:PXGridColumn>
				<px:PXGridColumn DataField="UsrRetainageAmt" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="UsrRetainageReleased" Width="130" ></px:PXGridColumn>
				<px:PXGridColumn DataField="UsrRetainageUnreleasedAmt" Width="150" ></px:PXGridColumn></Columns>			
				<RowTemplate>
					<px:PXSegmentMask runat="server" ID="CstPXSegmentMask9" DataField="ProjectID" AllowEdit="True" ></px:PXSegmentMask>
					<px:PXSelector AllowEdit="True" runat="server" ID="CstPXSelector10" DataField="RefNbr" ></px:PXSelector>
					<px:PXSelector AllowEdit="True" runat="server" ID="PXSelector11" DataField="UsrPONbr" ></px:PXSelector>
					<px:PXSegmentMask AllowEdit="True" runat="server" ID="CstPXSegmentMask11" DataField="VendorID" ></px:PXSegmentMask>
				</RowTemplate>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" />
		<ActionBar PagerVisible="Bottom">
			<PagerSettings Mode="NumericCompact" />
		</ActionBar>
	</px:PXGrid>
</asp:Content>