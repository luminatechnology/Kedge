<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="KG401000.aspx.cs" Inherits="Page_KG401000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="Kedge.KGDailyReportInq"
        PrimaryView="addItemFilter"
        >
		<CallbackCommands>
            <px:PXDSCallbackCommand Name="ViewKGReport" Visible="False" DependOnGrid="grid"></px:PXDSCallbackCommand>
		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView DataMember="addItemFilter" runat="server" ID="CstFormView2" DataSourceID="ds" TabIndex="3900" Width="100%" >
			<Template>
                <px:PXLayoutRule runat="server" StartRow="True" StartColumn="True"></px:PXLayoutRule>
				<px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector1" DataField="ContractID" ></px:PXSelector>

				<px:PXLayoutRule runat="server" StartColumn="True" ControlSize="M" LabelsWidth="S" ></px:PXLayoutRule>


                <px:PXLayoutRule runat="server" StartRow="True" StartColumn="True"></px:PXLayoutRule>
                    <px:PXDateTimeEdit ID="PXDateTimeEdit1" runat="server" AlreadyLocalized="False" CommitChanges="True" DataField="DateFrom" ></px:PXDateTimeEdit>

				<px:PXLayoutRule runat="server" StartColumn="True" ControlSize="M" LabelsWidth="S" ></px:PXLayoutRule>

				    <px:PXDateTimeEdit runat="server" ID="PXDateTimeEdit2" DataField="DateTo" AlreadyLocalized="False" CommitChanges="True" ></px:PXDateTimeEdit></Template>

    </px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXGrid  SyncPosition="True" ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="Inquire" AllowAutoHide="false">
		<Levels>
			<px:PXGridLevel DataMember="KGDailyReports">
			    <Columns>
			        <px:PXGridColumn LinkCommand="ViewKGReport"  DataField="DailyReportCD" Width="70" ></px:PXGridColumn>
				    <px:PXGridColumn DataField="ContractID" Width="70" ></px:PXGridColumn>
				    <px:PXGridColumn DataField="WorkDate" Width="70" ></px:PXGridColumn>
				    <px:PXGridColumn DataField="WeatherAM" Width="70" ></px:PXGridColumn>
                    <px:PXGridColumn DataField="WeatherPM" Width="70" ></px:PXGridColumn>
			    </Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" />
		<ActionBar >
		</ActionBar>
	</px:PXGrid>
</asp:Content>