<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormTab.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="KG301000.aspx.cs" Inherits="Page_KG301000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormTab.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="Kedge.KGDailyReportEntry"
        PrimaryView="Dailys"
        >
		<CallbackCommands>
                <px:PXDSCallbackCommand CommitChanges="True" Name="AddItem" Visible="False" />
		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="Dailys" Width="100%"  AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartRow="True"></px:PXLayoutRule>
			<px:PXLayoutRule ColumnWidth ="300px" runat="server" ID="CstPXLayoutRule1" StartColumn="True" ></px:PXLayoutRule>
            <px:PXSelector runat="server" ID="edDailyReportCDSel" DataField="DailyReportCD"  />
            <px:PXLayoutRule ColumnWidth ="300px" ID="PXLayoutRule3" runat="server" StartRow="True"></px:PXLayoutRule>
            
			<px:PXSelector runat="server" ID="CstPXSelector3" DataField="ContractID"  CommitChanges ="true" />
			<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit6" DataField="WorkDate"  CommitChanges ="true"  />
            <px:PXTextEdit runat="server" ID="CstPXTextEdit26" DataField="CreatedByID_Creator_displayName" ></px:PXTextEdit>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule2" StartColumn="True" ></px:PXLayoutRule>
            <px:PXDropDown runat="server" ID="CstPXDropDown4" DataField="WeatherAM" />
			
			<px:PXDropDown runat="server" ID="CstPXDropDown5" DataField="WeatherPM" />
            <px:PXTextEdit runat="server" ID="CstLastModifiedByID" DataField="LastModifiedByID_Modifier_Username" ></px:PXTextEdit>
		</Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXTab ID="tab" runat="server" Width="100%" DataSourceID="ds" AllowAutoHide="false">
		<Items>
			<px:PXTabItem Text="Site Notes">
				<Template>
					<px:PXFormView DataMember="DailyNotes" runat="server" ID="CstFormView7" >
						<Template>
							<px:PXRichTextEdit runat="server" EncodeInstructions="False" AllowAttached="True" AllowSearch="True" AllowMacros="True" AllowSourceMode="True" DataField="Remark" AlreadyLocalized="False" ID="edRemark" TabIndex="9999" Style='width:100%;'>
								<AutoSize Enabled="True" ></AutoSize></px:PXRichTextEdit></Template></px:PXFormView></Template>
			</px:PXTabItem>
			<px:PXTabItem Text="Daily Schedule" >
				<Template>
					<px:PXLayoutRule runat="server" ID="CstPXLayoutRule8" StartGroup="True" GroupCaption="Schedule Task" ></px:PXLayoutRule>
					<px:PXGrid SkinID="Details" Width="100%" runat="server" ID="taskGrid" DataSourceID="ds" SyncPosition="True" Height ="300px">
						<Levels>
							<px:PXGridLevel DataMember="DailyTasks" >
								<Columns>

									<px:PXGridColumn DataField="ProjectTaskID" Width="70" CommitChanges="True"  AutoCallBack="True"  ></px:PXGridColumn>
									<px:PXGridColumn DataField="ProjectTaskID_description" Width="280" ></px:PXGridColumn>
									<px:PXGridColumn DataField="PMTask__StartDate" Width="90" ></px:PXGridColumn>
									<px:PXGridColumn DataField="PMTask__EndDate" Width="90" ></px:PXGridColumn>
									<px:PXGridColumn DataField="ActualStartDate" Width="90" ></px:PXGridColumn>
									<px:PXGridColumn DataField="ActualEndDate" Width="90" ></px:PXGridColumn>
									<px:PXGridColumn DataField="ActualPercent" Width="100" CommitChanges="True"></px:PXGridColumn>
									<px:PXGridColumn DataField="SchedulePercent" Width="100" ></px:PXGridColumn>
									<px:PXGridColumn DataField="LastModifiedByID_Modifier_displayName" Width="70" ></px:PXGridColumn></Columns></px:PXGridLevel></Levels>
						
                        <Mode InitNewRow="True" AllowAddNew="false" AllowUpload ="true"   />
                         <AutoCallBack Target="processGrid" Command="Refresh" />
                        <AutoSize Container="Window" ></AutoSize>
                        <AutoSize Enabled="True" MinHeight="300" ></AutoSize>
                        <ActionBar PagerVisible="False">
							<CustomItems>
								<px:PXToolBarButton Text="Add Schedule Task" Key="cmdADI" AlreadyLocalized="False" SuppressHtmlEncoding="False" >
									<AutoCallBack Command="AddItem" Target="ds">
										<Behavior CommitChanges="True"   PostData="Page" />
									</AutoCallBack>
								</px:PXToolBarButton>
							</CustomItems>
						</ActionBar>

					</px:PXGrid>
					<px:PXLayoutRule runat="server" ID="CstPXLayoutRule9" StartGroup="True" GroupCaption="Daily Progress" ></px:PXLayoutRule>
					<px:PXGrid SkinID="Details" Width="100%" runat="server" ID="processGrid" DataSourceID="ds" SyncPosition="True" Height ="600px" AllowPaging="True" PageSize="12">
                        <CallbackCommands>
                                    <Refresh SelectControlsIDs="taskGrid" />
                        </CallbackCommands>
						<Levels>
							<px:PXGridLevel DataMember="DailyProgress" DataKeyNames="DailyScheduleID" >
								<Columns>
									<px:PXGridColumn DataField="ProjectTaskID" Width="70" CommitChanges="True" AllowUpdate="false" MatrixMode ="true"  ></px:PXGridColumn>
									<px:PXGridColumn DataField="ProjectTaskID_description" Width="280" />
									<px:PXGridColumn DataField="CostCodeID" Width="70" CommitChanges="True" MatrixMode ="true" ></px:PXGridColumn>
									<px:PXGridColumn DataField="InventoryID" Width="70" ></px:PXGridColumn>
									<px:PXGridColumn DataField="InventoryDesc" Width="70" ></px:PXGridColumn>
									<px:PXGridColumn DataField="Qty" Width="100" ></px:PXGridColumn>
									<px:PXGridColumn DataField="ChangeOrderQty" Width="100" ></px:PXGridColumn>
									<px:PXGridColumn DataField="RevisedQty" Width="100" ></px:PXGridColumn>
									<px:PXGridColumn DataField="ActualQty" Width="100" CommitChanges="True" ></px:PXGridColumn>
									<px:PXGridColumn DataField="AccumulateQty" Width="100" CommitChanges="True"></px:PXGridColumn>
                                    <px:PXGridColumn DataField="TotalAccumulateQty" Width="100" CommitChanges="True"></px:PXGridColumn>
									<px:PXGridColumn DataField="LastModifiedByID_Modifier_displayName" Width="70" ></px:PXGridColumn></Columns></px:PXGridLevel></Levels>
						<Mode InitNewRow="True" AllowAddNew="false" AllowDelete ="false" AllowUpload ="true" />
                       
                        <AutoSize Container="Window" ></AutoSize>
						<AutoSize Enabled="True" MinHeight="600" ></AutoSize></px:PXGrid></Template>
			</px:PXTabItem>
			<px:PXTabItem Text="Human Materials" >
				<Template>
					<px:PXGrid runat="server" ID="CstPXGrid16" SkinID="Details" Width="100%" SyncPosition="true">
						<Levels>
							<px:PXGridLevel DataMember="DailyMaterials" >
								<Columns>
									<px:PXGridColumn Type="CheckBox" DataField="Selected" Width="60" ></px:PXGridColumn>
									<px:PXGridColumn DataField="MaterialID" Width="70" CommitChanges="true" ></px:PXGridColumn>
                                    <px:PXGridColumn DataField="UOM" Width="70" ></px:PXGridColumn>
									<px:PXGridColumn DataField="MaterialID_description" Width="280" ></px:PXGridColumn>
									<px:PXGridColumn DataField="Qty" Width="100" ></px:PXGridColumn></Columns></px:PXGridLevel></Levels>
						<AutoSize Container="Window" ></AutoSize>
						<AutoSize Enabled="True" ></AutoSize></px:PXGrid></Template></px:PXTabItem></Items>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
	</px:PXTab>


</asp:Content>

<asp:Content ID="DialogsContent" runat="server" contentplaceholderid="phDialogs">
    <px:PXSmartPanel runat="server" ID="CstPanelAddProjectCost" Caption="Add Process" CaptionVisible="True" LoadOnDemand="True"
        Height="400px" Width="600px" AutoRepaint="True" Key="pmTasks" DesignView="Content" CreateOnDemand="True" TabIndex="4600">
         <px:PXFormView DataMember="addItemFilter" runat="server" ID="CstFormView2" DataSourceID="ds" TabIndex="3900" Width="100%" >
			<Template>
                <px:PXLayoutRule runat="server" StartRow="True" StartColumn="True"></px:PXLayoutRule>
				<px:PXDateTimeEdit ID="edDateFrom" runat="server" AlreadyLocalized="False" CommitChanges="True" DataField="WorkDate" />
				<px:PXLayoutRule runat="server" StartColumn="True" ControlSize="M" LabelsWidth="S" />
                <px:PXLayoutRule runat="server" StartRow="True" StartColumn="True"></px:PXLayoutRule>
                <px:PXDateTimeEdit ID="PXDateTimeEdit1" runat="server" AlreadyLocalized="False" CommitChanges="True" DataField="DateFrom" Visible="false" />
				<px:PXLayoutRule runat="server" StartColumn="True" ControlSize="M" LabelsWidth="S" />
                <px:PXDateTimeEdit runat="server" ID="PXDateTimeEdit2" DataField="DateTo" AlreadyLocalized="False" CommitChanges="True" Visible="false"  ></px:PXDateTimeEdit>

			</Template>

         </px:PXFormView>
        <px:PXGrid runat="server" ID="processGrid" AllowPaging="True" AutoAdjustColumns="True" SkinID="Inquire"  Width="950px" Height="300px">
          <Levels>
                <px:PXGridLevel DataMember="pmTasks">
                      <Columns>
                        <px:PXGridColumn DataField="Selected" Width="60" Type="CheckBox" AllowUpdate="true" AllowCheckAll="True" />
                        <px:PXGridColumn DataField="TaskCD" Width="81px"  ></px:PXGridColumn>
                        <px:PXGridColumn DataField="Description" Width="200px" ></px:PXGridColumn>
                        <px:PXGridColumn DataField="Status" Width="81px"  ></px:PXGridColumn>
                        <px:PXGridColumn DataField="CompletedPercent" TextAlign="Right" Width="99px" ></px:PXGridColumn>
                        <px:PXGridColumn DataField="StartDate" Width="90px" ></px:PXGridColumn>
                        <px:PXGridColumn DataField="EndDate" Width="90px" ></px:PXGridColumn>
 
                        <px:PXGridColumn DataField="UsrActualStartDate" Width="90px" ></px:PXGridColumn>
                        <px:PXGridColumn DataField="UsrActualEndDate" Width="90px" ></px:PXGridColumn>
          
                      </Columns>

                </px:PXGridLevel>

          </Levels>
          <AutoSize Enabled="True" />
          <Mode AllowUpdate="False" AllowDelete="False" /></px:PXGrid>
        <px:PXPanel runat="server" ID="CstPanel5" SkinID="Buttons">
                <px:PXButton runat="server" ID="CstAddButton" Text="Add" CommandName="AddItems" CommandSourceID="ds" SyncVisible="false" />
                <px:PXButton runat="server" ID="CstButton7" Text="Add &amp; Close" DialogResult="OK" />
                <px:PXButton runat="server" ID="CstButton8" Text="Cancel" DialogResult="Cancel" />

        </px:PXPanel>

    </px:PXSmartPanel>
</asp:Content>
