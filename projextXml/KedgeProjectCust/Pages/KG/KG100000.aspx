<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="KG100000.aspx.cs" Inherits="Page_KG100000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormView.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="Kedge.KGSetting"
        PrimaryView="setup"
        >
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXTab Width="100%" DataMember="setup" runat="server" ID="CstPXTab66">
		<Items>
			<px:PXTabItem Text="General">
				<Template>
					<px:PXFormView Width="100%" runat="server" ID="CstFormView67" DataMember="setup">
						<Template>
							<px:PXLayoutRule ControlSize="L" LabelsWidth="XM" runat="server" ID="CstPXLayoutRule68" StartGroup="True" GroupCaption="Numbering Setting" ></px:PXLayoutRule>
							<px:PXSelector  runat="server" ID="CstPXSelector69" DataField="KGDailyRenterNumbering" ></px:PXSelector>
							<px:PXSelector  runat="server" ID="CstPXSelector70" DataField="KGDailyReportNumbering" ></px:PXSelector>
							<px:PXSelector  runat="server" ID="CstPXSelector71" DataField="KGMonthlyInspectionNumbering" ></px:PXSelector>
							<px:PXSelector  runat="server" ID="CstPXSelector72" DataField="KGMonthlyInspectionTempNumbering" ></px:PXSelector>
							<px:PXSelector  runat="server" ID="CstPXSelector83" DataField="KGMonthlyInspectTicketNumbering" ></px:PXSelector>
							<px:PXSelector  runat="server" ID="CstPXSelector73" DataField="KGSafetyHealthInspectionNumbering" ></px:PXSelector>
							<px:PXSelector  runat="server" ID="CstPXSelector78" DataField="KGSafetyHealthInspectTicketNumbering" ></px:PXSelector>
							<px:PXSelector  runat="server" ID="CstPXSelector74" DataField="KGSafetyHealthInspectionTempNumbering" ></px:PXSelector>
							<px:PXSelector  runat="server" ID="CstPXSelector75" DataField="KGSelfInspectionNumbering" ></px:PXSelector>
							<px:PXSelector  runat="server" ID="CstPXSelector76" DataField="KGSelfInspectionTempNumbering" ></px:PXSelector>
							<px:PXSelector  runat="server" ID="CstPXSelector77" DataField="KGValuationNumbering" ></px:PXSelector>
							<px:PXSelector  runat="server" ID="CstPXSelector105" DataField="KGEvaluationNumbering" ></px:PXSelector>
                            <px:PXSelector  runat="server" ID="CstPXSelector112" DataField="KGContractEvalNumbering" ></px:PXSelector>
							<px:PXLayoutRule runat="server" ID="CstPXLayoutRule78" StartGroup="True" GroupCaption="Other Setting" ></px:PXLayoutRule>
							<px:PXSelector  runat="server" ID="CstPXSelector79" DataField="KGAdditionInventoryID" ></px:PXSelector>
							<px:PXSelector  runat="server" ID="CstPXSelector80" DataField="KGDeductionInventoryID" ></px:PXSelector>
							<px:PXSelector  runat="server" ID="CstPXSelector99" DataField="KGDeductionTaxFreeInventoryID" ></px:PXSelector>
							<px:PXSelector runat="server" ID="CstPXSelector127" DataField="KGFognWorkerInventoryID" ></px:PXSelector>
							<px:PXSelector runat="server" ID="CstPXSelector126" DataField="DiscountInventoryID" ></px:PXSelector>
							<px:PXSelector runat="server" ID="CstPXSelector128" DataField="KGRePurchaseInventoryID" />
							<px:PXSelector  runat="server" ID="CstPXSelector81" DataField="KGManageInventoryID" ></px:PXSelector>
							<px:PXSelector  runat="server" ID="CstPXSelector82" DataField="KGRetainageReturnInventoryID" ></px:PXSelector>
							<px:PXSelector  runat="server" ID="CstPXSelector107" DataField="LaborInsInventoryID" ></px:PXSelector>
							<px:PXSelector  runat="server" ID="CstPXSelector108" DataField="HealthInsInventoryID" ></px:PXSelector>
							<px:PXSelector  runat="server" ID="CstPXSelector109" DataField="PensionInventoryID" ></px:PXSelector>
                            <px:PXSelector  runat="server" ID="CstPXSelector104" DataField="KGIndividualTaxInventoryID" ></px:PXSelector>
                            <px:PXSelector  runat="server" ID="CstPXSelector106" DataField="KGSupplementaryTaxInventoryID" ></px:PXSelector>
							<px:PXNumberEdit runat="server" ID="CstPXNumberEdit108" DataField="KGSupplPremiumRate" ></px:PXNumberEdit>
							<px:PXNumberEdit runat="server" ID="CstPXNumberEdit83" DataField="KGManageFeeRate" ></px:PXNumberEdit>
							<px:PXNumberEdit runat="server" ID="CstPXNumberEdit84" DataField="KGManageFeeTaxRate" ></px:PXNumberEdit>
							<px:PXNumberEdit runat="server" ID="CstPXNumberEdit104" DataField="KGDailyRenterDateLimit" ></px:PXNumberEdit>
							<px:PXDropDown Size="S" runat="server" ID="CstPXDateTimeEdit107" DataField="KGBillStartDate" ></px:PXDropDown>
							<px:PXDropDown Size="S" runat="server" ID="CstPXDateTimeEdit106" DataField="KGBillEndDate" ></px:PXDropDown>
							<px:PXNumberEdit runat="server" ID="CstPXNumberEdit120" DataField="BillAdjustAmtLimit" ></px:PXNumberEdit>
							<px:PXNumberEdit runat="server" ID="CstPXNumberEdit121" DataField="KGDefEvaluationPct" ></px:PXNumberEdit>
							<px:PXNumberEdit runat="server" ID="CstPXNumberEdit122" DataField="KGDefSecEvaluationPct" ></px:PXNumberEdit>
							<px:PXLayoutRule runat="server" ID="CstPXLayoutRule123" StartGroup="True" GroupCaption="PS Setting" ></px:PXLayoutRule>
							<px:PXSegmentMask runat="server" ID="CstPXSegmentMask125" DataField="DefVendorCustID" ></px:PXSegmentMask>
							<px:PXSegmentMask runat="server" ID="CstPXSegmentMask124" DataField="DefEmployeeCustID" ></px:PXSegmentMask></Template></px:PXFormView></Template></px:PXTabItem>
			<px:PXTabItem Text="FINConnectionSetup" >
				<Template>
					<px:PXFormView Width="100%" runat="server" ID="CstFormView85" DataMember="setup">
						<Template>
							<px:PXLayoutRule runat="server" ID="CstPXLayoutRule100" StartGroup="True" GroupCaption="PaymentDateSetting" ></px:PXLayoutRule>
							<px:PXLayoutRule GroupCaption="Payment Date Setting" runat="server" ID="CstPXLayoutRule113" StartGroup="True" ></px:PXLayoutRule>
							<px:PXNumberEdit runat="server" ID="CstPXNumberEdit116" DataField="PaymentDayFrom" ></px:PXNumberEdit>
							<px:PXNumberEdit runat="server" ID="CstPXNumberEdit117" DataField="PaymentDayTo" ></px:PXNumberEdit>
							<px:PXNumberEdit runat="server" ID="CstPXNumberEdit115" DataField="PaymentDateMid" ></px:PXNumberEdit>
							<px:PXNumberEdit runat="server" ID="CstPXNumberEdit114" DataField="PaymentDateEnd" ></px:PXNumberEdit>                       
                            <px:PXLayoutRule runat="server" ID="PXLayoutRule1" StartGroup="True" GroupCaption="FINConnectionSetup" ></px:PXLayoutRule>
							<px:PXGrid runat="server" ID="CstPXGrid109" DataSourceID="ds" SkinID="Details"  Width="100%">
								<Levels>
									<px:PXGridLevel DataMember="FinIntegrationSetup">
										<Columns>                                          
											<px:PXGridColumn CommitChanges="True" DataField="ContractID" Width="70" ></px:PXGridColumn>
											<px:PXGridColumn DataField="ContractID_description" Width="220" ></px:PXGridColumn>
											<px:PXGridColumn DataField="ConnectSite" Width="70" ></px:PXGridColumn></Columns></px:PXGridLevel></Levels>
<AutoSize Container="Window" Enabled="True" ></AutoSize></px:PXGrid>
							</Template></px:PXFormView></Template></px:PXTabItem>
			<px:PXTabItem Text="Project Approve">
				<Template>
					<px:PXGrid runat="server" ID="CstPXGrid108" SkinID="Details" Width="100%">
						<Levels>
							<px:PXGridLevel DataMember="ApproveSetup">
								<Columns>
									<px:PXGridColumn DataField="ContractID" Width="70" CommitChanges="True" ></px:PXGridColumn>
									<px:PXGridColumn DataField="ContractID_description" Width="220" ></px:PXGridColumn>
									<px:PXGridColumn DataField="ApprovalSite" Width="70" ></px:PXGridColumn>
									<px:PXGridColumn DataField="IsAutoApproved" Width="60" Type="CheckBox" ></px:PXGridColumn></Columns></px:PXGridLevel></Levels>
						<AutoSize Container="Window" Enabled="True" ></AutoSize></px:PXGrid></Template></px:PXTabItem>
			<px:PXTabItem Text="Template Setup">
				<Template>
					<px:PXGrid runat="server" ID="CstPXGrid119" SkinID="Details" Width="100%">
						<Levels>
							<px:PXGridLevel DataMember="InspectionFileTemplates" >
								<Columns>
									<px:PXGridColumn DataField="InspectionType" Width="220" />
									<px:PXGridColumn DataField="Description" Width="220" /></Columns></px:PXGridLevel></Levels>
						<AutoSize Container="Window" Enabled="True"/></px:PXGrid></Template></px:PXTabItem></Items></px:PXTab></asp:Content>