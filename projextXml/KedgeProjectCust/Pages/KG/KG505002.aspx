<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormTab.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="KG505002.aspx.cs" Inherits="Page_KG505002" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormTab.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="Kedge.KGPercentCompleteProcess"
        PrimaryView="MasterView"
        >
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="MasterView" Width="100%" Height="100px" AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule runat="server" ID="PXLayoutRule1" StartRow="True" ></px:PXLayoutRule>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule3" StartColumn="True" ></px:PXLayoutRule>
			<px:PXSelector runat="server" ID="CstPXSelector1" DataField="BranchID" CommitChanges="True" ></px:PXSelector>
			<px:PXMultiSelector runat="server" ID="CstPXSelector2" DataField="ContractCD" ></px:PXMultiSelector>
			<px:PXSelector runat="server" ID="CstPXSelector7" DataField="Period" AutoRefresh="true"/></Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXTab ID="tab" runat="server" Width="100%" Height="150px" DataSourceID="ds" AllowAutoHide="false">
		<Items>
			<px:PXTabItem Text="Monthly accumulation">
				<Template>
					<px:PXGrid runat="server" ID="CstPXGrid4" RepaintColumns="true">
						<Levels>
							<px:PXGridLevel DataMember="MonthView" >
                                <Columns>
                                    <px:PXGridColumn DataField="ContractCD" Width="140" />
									<px:PXGridColumn DataField="ContractDesc" Width="220" />
									<px:PXGridColumn DataField="VendorName" Width="220" />
                                    <px:PXGridColumn DataField="CumIncomeThis" Width="100" />
									<px:PXGridColumn DataField="CumIncomeLast" Width="100" />
                                    <px:PXGridColumn DataField="RecIncomeThis" Width="100" />
                                    <px:PXGridColumn DataField="CumCostThis" Width="100" />
									<px:PXGridColumn DataField="CumCostLast" Width="100" />
                                    <px:PXGridColumn DataField="RecCostThis" Width="100" />
                                    <px:PXGridColumn DataField="CumProfitThis" Width="100" />
									<px:PXGridColumn DataField="CumProfitLast" Width="100" />
									<px:PXGridColumn DataField="RecProfitThis" Width="100" />
									<px:PXGridColumn DataField="ProfitRate" Width="100" />
									<px:PXGridColumn DataField="IncomeAmt" Width="100" />
									<px:PXGridColumn DataField="ProjectProgrees" Width="100" />
									<px:PXGridColumn DataField="ExpenseAmt" Width="100" />
									<px:PXGridColumn DataField="Profit" Width="100" />
								</Columns>
							</px:PXGridLevel></Levels>
                        <AutoSize Container="Window" Enabled="True" MinHeight="150" /></px:PXGrid></Template>
			</px:PXTabItem>
			<px:PXTabItem Text="Annual accumulation">
				<Template>
					<px:PXGrid runat="server" ID="CstPXGrid5" RepaintColumns="true">
						<Levels>
							<px:PXGridLevel DataMember="YearView" >
								<Columns>
                                    <px:PXGridColumn DataField="ContractCD" Width="140" />
									<px:PXGridColumn DataField="ContractDesc" Width="220" />
									<px:PXGridColumn DataField="VendorName" Width="220" />
                                    <px:PXGridColumn DataField="CumIncomeThis" Width="100" />
									<px:PXGridColumn DataField="CumIncomeLast" Width="100" />
                                    <px:PXGridColumn DataField="RecIncomeThis" Width="100" />
                                    <px:PXGridColumn DataField="CumCostThis" Width="100" />
									<px:PXGridColumn DataField="CumCostLast" Width="100" />
                                    <px:PXGridColumn DataField="RecCostThis" Width="100" />
                                    <px:PXGridColumn DataField="CumProfitThis" Width="100" />
									<px:PXGridColumn DataField="CumProfitLast" Width="100" />
									<px:PXGridColumn DataField="RecProfitThis" Width="100" />
									<px:PXGridColumn DataField="ProfitRate" Width="100" />
									<px:PXGridColumn DataField="IncomeAmt" Width="100" />
									<px:PXGridColumn DataField="ProjectProgrees" Width="100" />
									<px:PXGridColumn DataField="ExpenseAmt" Width="100" />
									<px:PXGridColumn DataField="Profit" Width="100" />
								</Columns></px:PXGridLevel></Levels>
                        <AutoSize Container="Window" Enabled="True" MinHeight="150" />
					</px:PXGrid></Template>
			</px:PXTabItem>
		</Items>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" />
	</px:PXTab>
</asp:Content>