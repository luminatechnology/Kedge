<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="KG507001.aspx.cs" Inherits="Page_KG507001" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="KedgeFinCustDev.FIN.Graph.KGAcctMovementProc" PrimaryView="Filter">
		<CallbackCommands>
		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
	<px:PXFormView ID="form" runat="server" Width="100%" Height="45px" DataMember="Filter"  NoteIndicator="True" FilesIndicator="True" ActivityIndicator="True" ActivityField="NoteActivity">
		<Template>
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartRow="True" LabelsWidth="SM" ControlSize="S" />
			<px:PXSelector ID="edYear" runat="server" DataField="Year" AutoRefresh="true" CommitChanges="true"></px:PXSelector>
		</Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
    <px:PXSplitContainer runat="server" ID="sp1" SkinID="Transparent" Height="100%" PositionInPercent="true" >
		<AutoSize Enabled="true" Container="Window" />
		<Template1>
			<px:PXGrid ID="grid1" runat="server" DataSourceID="ds" Width="100%" SkinID="Inquire" BorderStyle="None" AdjustPageSize="Auto" FilesIndicator="false" SyncPosition="true" MatrixMode="true" >
				<Levels>
					<px:PXGridLevel DataMember="ProjHistTran" SortOrder="FinPeriodID,AccountID,ProjectID" >
						<RowTemplate>
							<px:PXSegmentMask runat="server" ID="CstPXSegmentMask12" DataField="AccountID" AllowEdit="True" />
							<px:PXSegmentMask AllowEdit="True" runat="server" ID="CstPXSegmentMask13" DataField="BranchID" ></px:PXSegmentMask>
							<px:PXSelector AllowEdit="True" runat="server" ID="CstPXSelector14" DataField="LedgerID" ></px:PXSelector>
							<px:PXSegmentMask AllowEdit="True" runat="server" ID="CstPXSegmentMask15" DataField="ProjectID" ></px:PXSegmentMask>
							<px:PXSelector AllowEdit="True" runat="server" ID="CstPXSelector16" DataField="SubID" ></px:PXSelector></RowTemplate>
						<Columns>
							<px:PXGridColumn DataField="AccountID" ></px:PXGridColumn>
							<px:PXGridColumn DataField="AccountID_Account_description" Width="220" />
							<px:PXGridColumn DataField="BranchID" ></px:PXGridColumn>
							<px:PXGridColumn DataField="BranchID_Branch_acctName" Width="220" />
							<px:PXGridColumn DataField="ProjectID" ></px:PXGridColumn>
							<px:PXGridColumn DataField="ProjectID_description" Width="280" />
							<px:PXGridColumn DataField="LedgerID" ></px:PXGridColumn>
							<px:PXGridColumn DataField="LedgerID_description" Width="220" />
							<px:PXGridColumn DataField="SubID" ></px:PXGridColumn>
							<px:PXGridColumn DataField="SubID_description" Width="280" />
							<px:PXGridColumn DataField="FinPeriodID" ></px:PXGridColumn>
							<px:PXGridColumn DataField="FinPtdDebit" ></px:PXGridColumn>
							<px:PXGridColumn DataField="FinPtdCredit" ></px:PXGridColumn>
						</Columns>
					</px:PXGridLevel>
				</Levels>
				<AutoSize Enabled="True" MinHeight="150" />
				<Mode AllowAddNew="False" AllowDelete="False" AllowUpdate="False" />
				<ActionBar PagerVisible="Bottom">
					<PagerSettings Mode="NumericCompact" />
				</ActionBar>
			</px:PXGrid>
		</Template1>
		<Template2>
			<px:PXGrid ID="grid2" runat="server" DataSourceID="ds" Width="100%" SkinID="Inquire" BorderStyle="None" AdjustPageSize="Auto" FilesIndicator="false" SyncPosition="true" MatrixMode="true">
				<Levels>
					<px:PXGridLevel DataMember="ProjHistByPeriod" SortOrder="FinPeriodID,AccountID,ProjectID" >
						<RowTemplate>
							<px:PXSegmentMask runat="server" ID="CstPXSegmentMask17" DataField="AccountID" AllowEdit="True" />
							<px:PXSegmentMask AllowEdit="True" runat="server" ID="CstPXSegmentMask18" DataField="BranchID" ></px:PXSegmentMask>
							<px:PXSelector AllowEdit="True" runat="server" ID="CstPXSelector19" DataField="LedgerID" ></px:PXSelector>
							<px:PXSegmentMask AllowEdit="True" runat="server" ID="CstPXSegmentMask20" DataField="ProjectID" ></px:PXSegmentMask>
							<px:PXSelector AllowEdit="True" runat="server" ID="CstPXSelector21" DataField="SubID" ></px:PXSelector></RowTemplate>
						<Columns>
							<px:PXGridColumn DataField="AccountID" ></px:PXGridColumn>
							<px:PXGridColumn DataField="AccountID_Account_description" Width="220" />
							<px:PXGridColumn DataField="BranchID" ></px:PXGridColumn>
							<px:PXGridColumn DataField="BranchID_Branch_acctName" Width="220" />
							<px:PXGridColumn DataField="ProjectID" ></px:PXGridColumn>
							<px:PXGridColumn DataField="ProjectID_description" Width="280" />
							<px:PXGridColumn DataField="LedgerID" ></px:PXGridColumn>
							<px:PXGridColumn DataField="LedgerID_description" Width="220" />
							<px:PXGridColumn DataField="SubID" ></px:PXGridColumn>
							<px:PXGridColumn DataField="SubID_description" Width="280" />
							<px:PXGridColumn DataField="FinPeriodID" ></px:PXGridColumn>
							<px:PXGridColumn DataField="FinYtdBalance" ></px:PXGridColumn></Columns>
					</px:PXGridLevel>
				</Levels>
				<AutoSize Enabled="True" MinHeight="150" />
				<Mode AllowAddNew="False" AllowDelete="False" AllowUpdate="False" />
				<ActionBar PagerVisible="Bottom">
					<PagerSettings Mode="NumericCompact" />
				</ActionBar>
			</px:PXGrid>
		</Template2>
	</px:PXSplitContainer>
</asp:Content>