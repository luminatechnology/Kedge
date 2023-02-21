<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="KG304002.aspx.cs" Inherits="Page_KG304002" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="Kedge.KGSafetyHealthInspectionEntry"
        PrimaryView="SafetyHealthInspectionH"
        >
		<CallbackCommands>
            <px:PXDSCallbackCommand CommitChanges="True" Name="showPhoto" Visible="False" ></px:PXDSCallbackCommand>
            <px:PXDSCallbackCommand CommitChanges="True" Name="linePhoto" Visible="False" ></px:PXDSCallbackCommand>
            <px:PXDSCallbackCommand CommitChanges="True" Name="importResult" Visible="False" ></px:PXDSCallbackCommand>
		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="SafetyHealthInspectionH" Width="100%" Height="" AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartRow="True"></px:PXLayoutRule>
			<px:PXLayoutRule ControlSize="M" LabelsWidth="SM" runat="server" ID="CstPXLayoutRule1" StartColumn="True" ></px:PXLayoutRule>
                        <px:PXSelector runat="server" ID="CstPXNumberEdit3" DataField="ProjectID" ></px:PXSelector>
			            <px:PXSelector runat="server" ID="CstPXTextEdit4" DataField="SafetyHealthInspectionCD"  CommitChanges="True"></px:PXSelector>
			            <px:PXSelector runat="server" ID="CstPXNumberEdit5" DataField="TemplateHeaderID"  CommitChanges="True"></px:PXSelector>	
			            <px:PXNumberEdit runat="server" ID="CstPXNumberEdit15" DataField="Version" ></px:PXNumberEdit>	
                        <px:PXSelector runat="server" ID="CstPXSelector17" DataField="SiteManager" ></px:PXSelector>
                        <px:PXSelector runat="server" ID="CstPXSelector20" DataField="SafeHealthSupervisor" ></px:PXSelector>
                        <px:PXSelector runat="server" ID="CstPXSelector19" DataField="EquipmentSupervisor" ></px:PXSelector>
                        <px:PXSelector runat="server" ID="CstPXSelector16" DataField="CreatedByID" ></px:PXSelector>
			<px:PXLayoutRule runat="server" ID="CstLayoutRule29" ColumnSpan="2" ></px:PXLayoutRule>
			            <px:PXTextEdit runat="server" ID="CstPXTextEdit7" DataField="Remark" ></px:PXTextEdit>		
            <px:PXLayoutRule ControlSize="SM" LabelsWidth="SM" runat="server" ID="CstPXLayoutRule2" StartColumn="True" ></px:PXLayoutRule>
			            <px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit9" DataField="CheckDate" ></px:PXDateTimeEdit>	
                        <px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector14" DataField="InspectByID" AutoRefresh="True" ></px:PXSelector>
			            <px:PXTextEdit runat="server" ID="CstPXTextEdit10" DataField="Evaluation" ></px:PXTextEdit>
			            <px:PXNumberEdit runat="server" ID="CstPXNumberEdit11" DataField="FinalScore" ></px:PXNumberEdit>
                        <px:PXDropDown runat="server" ID="CstPXTextEdit13" DataField="Status" ></px:PXDropDown >
                        <px:PXCheckBox runat="server" ID="CstPXCheckBox5" DataField="Hold" CommitChanges="True"></px:PXCheckBox>
			<px:PXLabel runat="server" ID="CstLabel34" />
			            <px:PXDateTimeEdit Size="" LabelWidth="" runat="server" ID="CstPXDateTimeEdit17" DataField="CreatedDateTime" Enabled="False" ></px:PXDateTimeEdit>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule21" StartColumn="True" ></px:PXLayoutRule>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule22" StartGroup="True" GroupCaption="Photo View" ></px:PXLayoutRule>
			<px:PXFormView runat="server" ID="MasterFilter" DataMember="MasterFilter">
				<Template>
					<px:PXLayoutRule runat="server" ID="CstPXLayoutRule24" StartRow="True" ></px:PXLayoutRule>
					<px:PXLayoutRule runat="server" ID="CstPXLayoutRule25" StartColumn="True" LabelsWidth="S" ControlSize="S" ></px:PXLayoutRule>
					<px:PXDropDown runat="server" ID="CstPXDropDown26" DataField="CategoryCD" CommitChanges="true"></px:PXDropDown>
					<px:PXButton runat="server" ID="Photo" Text="Show Photo" >
                        <AutoCallBack Command="showPhoto" Target="ds" ></AutoCallBack>
					</px:PXButton>

				</Template></px:PXFormView></Template>	
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXGrid ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="Details" AllowAutoHide="false" SyncPosition="true" MatrixMode="true">
		<Levels>
			<px:PXGridLevel DataMember="SafetyHealthInspectionL">
			    <Columns>
				<px:PXGridColumn DataField="SegmentCD" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="SegmentDesc" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CheckItem" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Remark" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="ImprovementPlan" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn CommitChanges="True" DataField="IsChecked" Width="60"  Type="CheckBox" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CategoryCD" Width="180"  CommitChanges="True" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Deduction" Width="100" ></px:PXGridColumn>
                                <px:PXGridColumn DataField="LastRemark" Width="100" ></px:PXGridColumn></Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
		<ActionBar>
            
			<CustomItems>
                <px:PXToolBarButton Text="ShowPhoto" >
					<AutoCallBack Command="linePhoto" Target="ds" ></AutoCallBack></px:PXToolBarButton>
				<px:PXToolBarButton Text="ImportResult" >
					<AutoCallBack Command="importResult" Target="ds" ></AutoCallBack></px:PXToolBarButton>
            </CustomItems>

		
			<Actions>
				<AddNew Enabled="False" /></Actions>
			<Actions>
				<Delete Enabled="False" /></Actions></ActionBar>
	</px:PXGrid>
</asp:Content>