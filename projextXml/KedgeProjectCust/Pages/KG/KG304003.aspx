<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="KG304003.aspx.cs" Inherits="Page_KG304003" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="Kedge.KGMonthlyInspectTicketEntry"
        PrimaryView="MonthlyTickets"
        >
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="MonthlyTickets" Width="100%" Height="" AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule runat="server" ID="PXLayoutRule1" StartRow="True" ></px:PXLayoutRule>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule1" StartColumn="True" ControlSize="L" ></px:PXLayoutRule>
			<px:PXSelector runat="server" ID="CstPXSelector9" DataField="MonthlyInspectTicketCD" />
            <px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector1" DataField="ContractID" ></px:PXSelector>
            <px:PXSelector CommitChanges="True" runat="server" ID="PXSelector1" DataField="MonthlyInspectionID" ></px:PXSelector>
            
            <px:PXDropDown CommitChanges="True" runat="server" ID="PXDropDown1" DataField="Status"></px:PXDropDown>
			<px:PXCheckBox runat="server" ID="CstPXCheckBox5" DataField="Hold" CommitChanges="True"></px:PXCheckBox>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule2" StartColumn="True" ControlSize="XM" ></px:PXLayoutRule>
			<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit3" DataField="CheckDate" ></px:PXDateTimeEdit>
            <px:PXDropDown CommitChanges="True" runat="server" ID="CstPXDropDown23" DataField="CheckYear" ></px:PXDropDown>
			<px:PXDropDown CommitChanges="True" runat="server" ID="CstPXDropDown22" DataField="CheckMonth" ></px:PXDropDown>
            <px:PXSelector CommitChanges="True" runat="server" ID="PXSelector2" DataField="SegmentCD" ></px:PXSelector>
			<px:PXSegmentMask runat="server" ID="CstPXSegmentMask8" DataField="InspectByID" ></px:PXSegmentMask></Template>

	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
   
	<px:PXGrid runat="server" ID="grid" Height="300px" SkinID="Details" Width="100%" AllowAutoHide="false" DataSourceID="ds" SyncPosition ="true">
		
		<Levels>
			<px:PXGridLevel DataMember="MonthlyTicketLs">
				<Columns >
                    <px:PXGridColumn DataField="CheckItem"  CommitChanges="true" Width="250px"  ></px:PXGridColumn>
                    <px:PXGridColumn DataField="LastMonthRemark" Width="250px" ></px:PXGridColumn>
                    <px:PXGridColumn DataField="TestResult"  Width="250px" CommitChanges="true">
                    </px:PXGridColumn>
                    <px:PXGridColumn DataField="Remark" MatrixMode="true"  Width="250px" CommitChanges="true">
                    </px:PXGridColumn>
				</Columns>
			
				<RowTemplate>
					<px:PXSelector runat="server" ID="CstPXSelector8" DataField="CheckItem" /></RowTemplate></px:PXGridLevel>

		</Levels>
        <Mode  InitNewRow="true"></Mode>
		<AutoSize Enabled="True" Container="Parent" MinHeight="400" MinWidth="500" ></AutoSize>
	</px:PXGrid>

</asp:Content>
