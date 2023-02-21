<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="KG104000.aspx.cs" Inherits="Page_KG104000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="Kedge.KGMonthlyInspectionTemplate"
        PrimaryView="MonthlyTemplateH"
        >
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="MonthlyTemplateH" Width="100%" Height="" AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartRow="True"></px:PXLayoutRule>
			<px:PXLayoutRule ControlSize="M" LabelsWidth="M" runat="server" ID="CstPXLayoutRule3" StartColumn="True" ></px:PXLayoutRule>
			<px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector13" DataField="TemplateCD" ></px:PXSelector>
			<px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector25" DataField="SegmentCD" ></px:PXSelector>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit7" DataField="Description" ></px:PXTextEdit>
	        <px:PXSelector AllowEdit="False" runat="server" ID="CstPXSelector15" DataField="CreatedByID" ></px:PXSelector>
			<px:PXLayoutRule ControlSize="M" ColumnWidth="" runat="server" ID="CstPXLayoutRule4" StartColumn="True" ></px:PXLayoutRule>
			<px:PXNumberEdit CommitChanges="True" runat="server" ID="CstPXNumberEdit10" DataField="Version" AllowNull="True" ></px:PXNumberEdit>
            <px:PXDropDown runat="server" ID="CstPXDropDown14" DataField="Status" ></px:PXDropDown>
			<px:PXCheckBox CommitChanges="True" runat="server" ID="CstPXCheckBox27" DataField="Hold" ></px:PXCheckBox>
			
			<px:PXDateTimeEdit Size="" Enabled="False" runat="server" ID="CstPXDateTimeEdit16" DataField="CreatedDateTime" ></px:PXDateTimeEdit></Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXTab runat="server" ID="CstPXTab22">
		<Items>
			<px:PXTabItem Text="Monthly Second Category" >
				<Template>
					<px:PXGrid DataSourceID="ds" Width="100%" SkinID="Details" runat="server" ID="GridS" SyncPosition="true" MatrixMode="true" >
						<Levels>
							<px:PXGridLevel DataMember="MonthlyTemplateS" >
								<Columns>
									<px:PXGridColumn DataField="SegmentCD" Width="140" CommitChanges="True" ></px:PXGridColumn>
									<px:PXGridColumn DataField="SegmentDesc" Width="280" ></px:PXGridColumn>
									<px:PXGridColumn DataField="ScoreSetup" Width="100" ></px:PXGridColumn>
								</Columns>

							</px:PXGridLevel>

						</Levels>
						<AutoSize Container="Window" Enabled="True" MinHeight="150" >

						</AutoSize>
                        <Mode AllowUpload ="true" ></Mode>
					</px:PXGrid></Template></px:PXTabItem>
			<px:PXTabItem Text="Monthly Inspection Inventory" >
				<Template>
					<px:PXGrid DataSourceID="ds" Width="100%" runat="server" ID="GridL" SkinID="Details" SyncPosition="true" MatrixMode="true"  >
						<Levels>
							<px:PXGridLevel DataMember="MonthlyTemplateL" >
								<Columns >
									<px:PXGridColumn CommitChanges="True" DataField="SegmentCD" Width="140"   ></px:PXGridColumn>
									<px:PXGridColumn DataField="CheckItem" Width="180" ></px:PXGridColumn>
									<px:PXGridColumn DataField="CheckPointDesc" Width="180" ></px:PXGridColumn>
									<px:PXGridColumn CommitChanges="True" DataField="MaxNoMissing" Width="200" ></px:PXGridColumn></Columns>
							</px:PXGridLevel>

						</Levels>
						<AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
                        <Mode InitNewRow ="true"   AllowUpload ="true"  ></Mode>
					</px:PXGrid></Template></px:PXTabItem></Items></px:PXTab></asp:Content>
