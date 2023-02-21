<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="KG304005.aspx.cs" Inherits="Page_KG304005" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="Kedge.KGSafetyHealthInspectTicketEntry"
        PrimaryView="SafetyTickets"
        >
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="SafetyTickets" Width="100%" Height="120px" AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule runat="server" ID="PXLayoutRule1" StartRow="True" ></px:PXLayoutRule>
			<px:PXLayoutRule LabelsWidth="SM" runat="server" ID="CstPXLayoutRule1" StartColumn="True" ></px:PXLayoutRule>
				<px:PXSelector runat="server" ID="PXSelector1" DataField="SafetyHealthInspectTicketCD" ></px:PXSelector>
				<px:PXSelector runat="server" ID="PXSelector2" DataField="ProjectID" CommitChanges="True"></px:PXSelector>
				<px:PXSelector runat="server" ID="PXSelector3" DataField="SafetyHealthInspectionID" AutoRefresh="true" CommitChanges="True"></px:PXSelector>
				<px:PXDropDown runat="server" ID="PXDropDown4" DataField="Status"></px:PXDropDown>
				<px:PXCheckBox runat="server" ID="PXCheckBox5" DataField="Hold" CommitChanges="True"></px:PXCheckBox>
           
	        <px:PXLayoutRule runat="server" ID="CstPXLayoutRule2" StartColumn="True" ></px:PXLayoutRule>
	            <px:PXDateTimeEdit runat="server" ID="PXDateTimeEdit6" DataField="CheckDate" ></px:PXDateTimeEdit>
				<px:PXSelector runat="server" ID="PXSelector8" DataField="InspectByID" ></px:PXSelector>
                <px:PXSelector runat="server" ID="PXSelector7" DataField="SegmentCD" ></px:PXSelector></Template>
         <AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
	</px:PXFormView>
</asp:Content>

<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
   
	<px:PXGrid NoteIndicator="False" runat="server" ID="grid" Height="300px" SkinID="Details" Width="100%" AllowAutoHide="false" DataSourceID="ds" SyncPosition ="true">
		
		<Levels>
			<px:PXGridLevel DataMember="SafetyTicketLs">
				<Columns >
					<px:PXGridColumn DataField="CheckItem" Width="180" ></px:PXGridColumn>
                    <px:PXGridColumn DataField="LastRemark" Width="250px" ></px:PXGridColumn>
                    <px:PXGridColumn DataField="CategoryCD"  Width="250px" CommitChanges="true"></px:PXGridColumn>
                    <px:PXGridColumn DataField="Remark" MatrixMode="true"  Width="250px"></px:PXGridColumn>
					<px:PXGridColumn DataField="ImprovementPlan" Width="70" ></px:PXGridColumn></Columns>

                    <RowTemplate>
					<px:PXSelector runat="server" ID="RowTempCheckItem" DataField="CheckItem" ></px:PXSelector></RowTemplate>
			</px:PXGridLevel>

		</Levels>
        <Mode  InitNewRow="true"></Mode>
		<AutoSize Enabled="True" Container="Parent" MinHeight="400" MinWidth="500" ></AutoSize>
	
		<ActionBar>
			<Actions>
				<NoteShow Enabled="False" ></NoteShow></Actions></ActionBar></px:PXGrid>

</asp:Content>