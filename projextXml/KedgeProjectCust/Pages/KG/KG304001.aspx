<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormTab.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="KG304001.aspx.cs" Inherits="Page_KG304001" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormTab.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="Kedge.KGSelfInspectionEntry"
        PrimaryView="SelfInspection"
        >
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="SelfInspection" Width="100%" Height="" AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartRow="True"></px:PXLayoutRule>
			<px:PXLayoutRule ControlSize="XM" LabelsWidth="SM" runat="server" ID="CstPXLayoutRule9" StartColumn="True" ></px:PXLayoutRule>
			<px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector1" DataField="SelfInspectionCD" ></px:PXSelector>
			<px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector2" DataField="ProjectID" ></px:PXSelector>
			<px:PXSelector FilterByAllFields="True" CommitChanges="True" runat="server" ID="CstPXSelector3" DataField="TemplateHeaderID" ></px:PXSelector>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit7" DataField="SegmentCD" ></px:PXTextEdit>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit6" DataField="SegmentDesc" ></px:PXTextEdit>
			<px:PXNumberEdit runat="server" ID="CstPXNumberEdit8" DataField="Version" ></px:PXNumberEdit>
			<px:PXDateTimeEdit Enabled="False" Size="M" runat="server" ID="CstPXDateTimeEdit11" DataField="CreatedDateTime" ></px:PXDateTimeEdit>
            <px:PXSelector runat="server" ID="CstPXSelector12" DataField="CreatedByID" ></px:PXSelector>
			<px:PXLayoutRule ControlSize="L" LabelsWidth="S" runat="server" ID="CstPXLayoutRule6" StartColumn="True" ></px:PXLayoutRule>
			<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit10" DataField="CheckDate" ></px:PXDateTimeEdit>
			<px:PXSelector runat="server" ID="CstPXSelector14" DataField="InspectByID" ></px:PXSelector>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit5" DataField="CheckPosition" ></px:PXTextEdit>
			<px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector9" DataField="OrderNbr" AutoRefresh="True" ></px:PXSelector>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit26" DataField="VendorName" ></px:PXTextEdit>
			<px:PXCheckBox CommitChanges="True" runat="server" ID="CstPXCheckBox13" DataField="Hold" ></px:PXCheckBox>
			<px:PXDropDown runat="server" ID="CstPXDropDown10" DataField="Status" ></px:PXDropDown>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit39" DataField="Remark" ></px:PXTextEdit></Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXTab ID="tab" runat="server" Width="100%" Height="150px" DataSourceID="ds" AllowAutoHide="false">
		<Items>
			<px:PXTabItem Text="SelfInspectionLine">
				<Template>
					<px:PXGrid SyncPosition="True" SkinID="Details" Height="250px" Width="100%" runat="server" ID="CstPXGrid1">
						<Levels>
							<px:PXGridLevel DataMember="SelfInspectionLine" >
								<Columns>
									<px:PXGridColumn DataField="CheckItem" Width="250" ></px:PXGridColumn>
									<px:PXGridColumn CommitChanges="True" DataField="TestResult" Width="100" ></px:PXGridColumn>
									<px:PXGridColumn DataField="ActualInspectionDesc" Width="120" ></px:PXGridColumn></Columns></px:PXGridLevel></Levels>
                        <Mode InitNewRow="True" />
					</px:PXGrid></Template>
			</px:PXTabItem>
			<px:PXTabItem Text="SelfInspectionReview">
				<Template>
					<px:PXGrid SkinID="Details" Width="100%" Height="250px" runat="server" ID="CstPXGrid2">
						<Levels>
							<px:PXGridLevel DataMember="SelfInspectionReview" >
								<Columns>
									<px:PXGridColumn DataField="CheckItem" Width="250" ></px:PXGridColumn>
									<px:PXGridColumn DataField="TestResult" Width="140" ></px:PXGridColumn>
									<px:PXGridColumn CommitChanges="True" DataField="ReviewResult" Width="70" ></px:PXGridColumn>
									<px:PXGridColumn DataField="MissingProcessing" Width="70" ></px:PXGridColumn>
									<px:PXGridColumn DataField="CompleteDate" Width="90" ></px:PXGridColumn>
									<px:PXGridColumn DataField="Remark" Width="70" ></px:PXGridColumn></Columns></px:PXGridLevel></Levels>
						<Mode AllowAddNew="False" />
						<Mode AllowDelete="False" /></px:PXGrid></Template>
			</px:PXTabItem>
		</Items>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" />
	</px:PXTab>
</asp:Content>