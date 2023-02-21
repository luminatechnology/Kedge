<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="KG104002.aspx.cs" Inherits="Page_KG104002" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="Kedge.KGSafetyHealthInspectionTemplate"
        PrimaryView="SafetyHealthTemplateH"
        >
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="SafetyHealthTemplateH" Width="100%" Height="" AllowAutoHide="false"  >
		<Template>
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartRow="True"></px:PXLayoutRule>
			<px:PXLayoutRule ControlSize="M" LabelsWidth="M" runat="server" ID="CstPXLayoutRule1" StartColumn="True" ></px:PXLayoutRule>
			<px:PXSelector runat="server" ID="CstPXTextEdit6" DataField="TemplateCD" CommitChanges="True" ></px:PXSelector >
			<px:PXSelector runat="server" ID="CstPXSelector20" DataField="SegmentCD" CommitChanges="True" ></px:PXSelector>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit3" DataField="Description" ></px:PXTextEdit>
			<px:PXDateTimeEdit LabelWidth="200px" Size="M" Enabled="False" runat="server" ID="CstPXDateTimeEdit19" DataField="CreatedDateTime" ></px:PXDateTimeEdit>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule2" StartColumn="True" ></px:PXLayoutRule>
			<px:PXNumberEdit Enabled="False" runat="server" ID="CstPXNumberEdit9" DataField="Version" ></px:PXNumberEdit>
			<px:PXDropDown Enabled="False" runat="server" ID="CstPXTextEdit8" DataField="Status" ></px:PXDropDown >
			<px:PXCheckBox CommitChanges="True" runat="server" ID="CstPXCheckBox17" DataField="Hold" ></px:PXCheckBox>
			<px:PXSelector runat="server" ID="CstPXSelector18" DataField="CreatedByID" ></px:PXSelector></Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXTab runat="server" ID="CstPXTab22">
		<Items>
			<px:PXTabItem Text="Safety Second Category" >
				<Template>
					<px:PXGrid DataSourceID="ds" Width="100%" SkinID="Details" runat="server" ID="CstPXGrid26" SyncPosition="true" MatrixMode="true" >
						<Levels>
							<px:PXGridLevel DataMember="SafetyHealthTemplateS" >
								<Columns>
									<px:PXGridColumn DataField="SegmentCD" Width="140" CommitChanges="True" ></px:PXGridColumn>
									<px:PXGridColumn DataField="SegmentDesc" Width="280" ></px:PXGridColumn>
									<px:PXGridColumn DataField="ScoreSetup" Width="100" ></px:PXGridColumn></Columns></px:PXGridLevel></Levels>
						<AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
						<Mode AllowUpload="True" /></px:PXGrid></Template></px:PXTabItem>
			<px:PXTabItem Text="SafetyInspection Inventory" >
				<Template>
					<px:PXGrid DataSourceID="ds" Width="100%" runat="server" ID="CstPXGrid24" SkinID="Details" SyncPosition="true" MatrixMode="true">
						<Levels>
							<px:PXGridLevel DataMember="SafetyHealthTemplateL" >
								<Columns>
									<px:PXGridColumn DataField="SegmentCD" Width="140" ></px:PXGridColumn>
									<px:PXGridColumn DataField="CheckItem" Width="70" ></px:PXGridColumn>
									<px:PXGridColumn DataField="RiskType" Width="70" />
									<px:PXGridColumn DataField="RiskCategory" Width="70" /></Columns></px:PXGridLevel></Levels>
						<AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
<Mode InitNewRow ="true"   AllowUpload ="true"  /></px:PXGrid></Template></px:PXTabItem></Items></px:PXTab></asp:Content>