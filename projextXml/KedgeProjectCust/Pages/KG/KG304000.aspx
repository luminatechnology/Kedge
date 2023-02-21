<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="KG304000.aspx.cs" Inherits="Page_KG304000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="Kedge.KGMonthlyInspectionEntry"
        PrimaryView="MonthlyInspectionH"
        >
        <CallbackCommands>
            <px:PXDSCallbackCommand CommitChanges="True" Name="FlawPhoto" Visible="False" />
		</CallbackCommands>
        <CallbackCommands>
            <px:PXDSCallbackCommand CommitChanges="True" Name="ObservedPhoto" Visible="False" />
		</CallbackCommands>
        <CallbackCommands>
            <px:PXDSCallbackCommand CommitChanges="True" Name="QualifiedPhoto" Visible="False" />
		</CallbackCommands>
        <CallbackCommands>
            <px:PXDSCallbackCommand CommitChanges="True" Name="ImportTicketResult" Visible="False" />
		</CallbackCommands>
        <CallbackCommands>
            <px:PXDSCallbackCommand CommitChanges="True" Name="ViewTicketFile" Visible="False" />
		</CallbackCommands>
        

	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="MonthlyInspectionH" Width="100%" AllowAutoHide="False" TabIndex="100">
		<Template>
			<px:PXLayoutRule LabelsWidth="SM" ControlSize="M" runat="server" StartRow="" StartColumn="True"></px:PXLayoutRule>
			<px:PXSegmentMask runat="server" ID="CstPXSegmentMask16" DataField="ProjectID" CommitChanges="True" ></px:PXSegmentMask>
			<px:PXSelector CommitChanges="True" runat="server" ID="CstPXTextEdit3" DataField="MonthlyInspectionCD"  ></px:PXSelector>
			<px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector17" DataField="TemplateHeaderID"  AutoRefresh ="true" ></px:PXSelector>
			<px:PXNumberEdit runat="server" ID="CstPXNumberEdit15" DataField="Version" AlreadyLocalized="False" DefaultLocale="" ></px:PXNumberEdit>
			
            <px:PXSelector CommitChanges="True" runat="server" ID="CstConstructionStage" DataField="ConstructionStage"  AutoRefresh ="true" ></px:PXSelector>
			<px:PXSelector CommitChanges="True" ID="edSystemManager" runat="server" AlreadyLocalized="False" DataField="SystemManager" DefaultLocale="">
            </px:PXSelector>
            <px:PXSelector CommitChanges="True" ID="edSiteManager" runat="server" AlreadyLocalized="False" DataField="SiteManager" DefaultLocale="">
            </px:PXSelector>
            <px:PXSelector runat="server" ID="CstPXSelector20" DataField="CreatedByID" ></px:PXSelector>
			<px:PXDateTimeEdit Size="M" Enabled="False" runat="server" ID="CstPXDateTimeEdit21" DataField="CreatedDateTime" AlreadyLocalized="False" DefaultLocale="" ></px:PXDateTimeEdit>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit5" DataField="Remark" AlreadyLocalized="False" DefaultLocale="" ></px:PXTextEdit>
            
            
            <px:PXLayoutRule ControlSize="M" LabelsWidth="SM" runat="server" StartColumn="True" ></px:PXLayoutRule>
			<px:PXDropDown CommitChanges="True" runat="server" ID="CstPXDropDown23" DataField="CheckYear" ></px:PXDropDown>
			<px:PXDropDown CommitChanges="True" runat="server" ID="CstPXDropDown22" DataField="CheckMonth" ></px:PXDropDown>
			<px:PXDateTimeEdit CommitChanges="True" runat="server" ID="CstPXDateTimeEdit7" DataField="CheckDate" AlreadyLocalized="False" DefaultLocale="" ></px:PXDateTimeEdit>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit10" DataField="OverallRating" AlreadyLocalized="False" DefaultLocale="" ></px:PXTextEdit>
			<px:PXDropDown runat="server" ID="CstPXDropDown19" DataField="Evaluation" ></px:PXDropDown>
			<px:PXTextEdit Width="350px"  runat="server" ID="CstPXTextEdit49" DataField="EvaluationDesc" />
			<px:PXNumberEdit CommitChanges="True" runat="server" ID="CstPXNumberEdit9" DataField="FinalScore" AlreadyLocalized="False" DefaultLocale="" ></px:PXNumberEdit>
			 <px:PXSelector CommitChanges="True" ID="edEquipmentSupervisor" runat="server"  DataField="EquipmentSupervisor" >
            </px:PXSelector>
            <px:PXDropDown CommitChanges="True" runat="server" ID="CstPXDropDown18" DataField="Status" ></px:PXDropDown>
			<px:PXCheckBox CommitChanges="True" runat="server" ID="CstPXCheckBox20" DataField="Hold" AlreadyLocalized="False" ></px:PXCheckBox></Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXTab runat="server" ID="CstPXTab22">
		<Items>
			<px:PXTabItem Text="Check Item" >
				<Template>
                    <px:PXGrid ID="MonthlyInspectionLGrid" runat="server" SyncPosition="True" DataSourceID="ds" Width="100%" Height="150px" SkinID="Details" AllowAutoHide="False" TabIndex="700">
		 <EmptyMsg AnonFilteredAddMessage="No records found.
Try to change filter to see records here." AnonFilteredMessage="No records found.
Try to change filter to see records here." ComboAddMessage="No records found.
Try to change filter or modify parameters above to see records here." FilteredAddMessage="No records found.
Try to change filter to see records here." FilteredMessage="No records found.
Try to change filter to see records here." NamedComboAddMessage="No records found as &#39;{0}&#39;.
Try to change filter or modify parameters above to see records here." NamedComboMessage="No records found as &#39;{0}&#39;.
Try to change filter or modify parameters above to see records here." NamedFilteredAddMessage="No records found as &#39;{0}&#39;.
Try to change filter to see records here." NamedFilteredMessage="No records found as &#39;{0}&#39;.
Try to change filter to see records here." ></EmptyMsg>
		<Levels>
			<px:PXGridLevel DataMember="MonthlyInspectionL" DataKeyNames="MonthlyInspectionID,SegmentCD,MonthlyInspectionLineID">
			    <Columns>
				<px:PXGridColumn CommitChanges="True" DataField="SegmentCD" Width="140px" ></px:PXGridColumn>
				<px:PXGridColumn DataField="SegmentCD_SegmentValue_descr" Width="220px" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CheckItem" Width="180px" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CheckPointDesc" Width="180px" ></px:PXGridColumn>
                <px:PXGridColumn CommitChanges="True" DataField="TestResult"  Width="200px"></px:PXGridColumn>

				<px:PXGridColumn DataField="Remark" Width="200px" ></px:PXGridColumn>

				<px:PXGridColumn DataField="LastMonthRemark" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn CommitChanges="True" Type="CheckBox" DataField="IsChecked" Width="60px" ></px:PXGridColumn>
				
				<px:PXGridColumn CommitChanges="True" DataField="MissingNum" ></px:PXGridColumn>
				<px:PXGridColumn CommitChanges="True" DataField="Deduction" ></px:PXGridColumn>
				<px:PXGridColumn CommitChanges="True" DataField="Score" ></px:PXGridColumn>
				<px:PXGridColumn DataField="ScoreSetupLine" Width="100px" ></px:PXGridColumn>
				<px:PXGridColumn DataField="MaxNoMissing" ></px:PXGridColumn>
				<px:PXGridColumn DataField="EveryMissingBuckle" Width="100px" ></px:PXGridColumn></Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
		<Mode AllowDelete="False" AllowAddNew="False" ></Mode>
        
						<ActionBar>
							<CustomItems>
								<px:PXToolBarButton Text="FlawPhoto" >
                                    <AutoCallBack Command="FlawPhoto" Target="ds">
									    <Behavior CommitChanges="True"   PostData="Page" ></Behavior>
							        </AutoCallBack>

								</px:PXToolBarButton>
								<px:PXToolBarButton Text="ObservedPhoto" >
                                    <AutoCallBack Command="ObservedPhoto" Target="ds">
									    <Behavior CommitChanges="True"   PostData="Page" ></Behavior>
							        </AutoCallBack>

								</px:PXToolBarButton>
								<px:PXToolBarButton Text="QualifiedPhoto" >
                                    <AutoCallBack Command="QualifiedPhoto" Target="ds">
									    <Behavior CommitChanges="True"   PostData="Page" ></Behavior>
							        </AutoCallBack>
								</px:PXToolBarButton>
								<px:PXToolBarButton Text="ImportTicketResult" >
                                    <AutoCallBack Command="ImportTicketResult" Target="ds">
									    <Behavior CommitChanges="True"   PostData="Page" ></Behavior>
							        </AutoCallBack>
                                </px:PXToolBarButton>
                                <px:PXToolBarButton Text="ViewTicketFile" >
                                    <AutoCallBack Command="ViewTicketFile" Target="ds">
									    <Behavior CommitChanges="True"   PostData="Page" ></Behavior>
							        </AutoCallBack>
                                </px:PXToolBarButton>
                                
							</CustomItems>

					</ActionBar>

		</px:PXGrid>
      </Template>

	</px:PXTabItem>
    <px:PXTabItem Text="Sign" >
		<Template>
            <px:PXGrid ID="KGMonthlyInspectionSignsGrid" runat="server" SyncPosition="True" DataSourceID="ds" Width="100%" Height="150px" SkinID="Details" AllowAutoHide="False" TabIndex="700">
		
		    <Levels>
			    <px:PXGridLevel DataMember="KGMonthlyInspectionSigns" >
			        <Columns>
				        <px:PXGridColumn DataField="SignBy" Width="220px" ></px:PXGridColumn>
			        </Columns>
			    </px:PXGridLevel>
		    </Levels>
		    <AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
		    <Mode AllowDelete="False" AllowAddNew="False" AllowUpdate="False" ></Mode>

		</px:PXGrid>

         </Template>
   </px:PXTabItem>
  </Items>
</px:PXTab>



	
</asp:Content>
