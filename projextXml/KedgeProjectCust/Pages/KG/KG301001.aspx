<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="KG301001.aspx.cs" Inherits="Page_KG301001" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="Kedge.KGRenterApplyEntry"
        PrimaryView="DailyRenters"
        >
		<CallbackCommands></CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView Expanded="True" AllowCollapse="True" ID="form" runat="server" DataSourceID="ds" DataMember="DailyRenters" Width="100%" Height="" AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule ColumnSpan="1" runat="server" ID="CstPXLayoutRule10" StartRow="True" ></px:PXLayoutRule>
			<px:PXLayoutRule ColumnWidth="300px" ColumnSpan="1" runat="server" ID="CstPXLayoutRule11" StartColumn="True" ></px:PXLayoutRule>
			<px:PXSelector Required="True" runat="server" ID="CstPXSelector21" DataField="DailyRenterCD" ></px:PXSelector>
			<px:PXLayoutRule ColumnSpan="1" runat="server" ID="CstPXLayoutRule27" StartColumn="True" ></px:PXLayoutRule>
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartRow="True"></px:PXLayoutRule>
			<px:PXLayoutRule ColumnWidth="300px" runat="server" ID="CstPXLayoutRule1" StartColumn="True" ></px:PXLayoutRule>
			<px:PXSelector Width="" CommitChanges="True" Size="" runat="server" ID="CstPXSelector20" DataField="ContractID" Required="True" ></px:PXSelector>
			<px:PXSelector CommitChanges="True" Required="True" runat="server" ID="CstPXSelector18" DataField="OrderNbr" ></px:PXSelector>
			<px:PXSelector Required="True" runat="server" ID="CstPXSelector19" DataField="LineNbr" ></px:PXSelector>
			<px:PXNumberEdit Required="True" runat="server" ID="CstPXNumberEdit9" DataField="ReqQty" ></px:PXNumberEdit>
			<px:PXNumberEdit Required="True" runat="server" ID="CstPXNumberEdit10" DataField="ReqSelfQty" ></px:PXNumberEdit>
			<px:PXNumberEdit Required="True" runat="server" ID="CstPXNumberEdit7" DataField="ReqInsteadQty" ></px:PXNumberEdit>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit26" DataField="CreatedByID_Creator_displayName" ></px:PXTextEdit>
			<px:PXLayoutRule ColumnWidth="300px" runat="server" ID="CstPXLayoutRule2" StartColumn="True" ></px:PXLayoutRule>
			<px:PXDateTimeEdit Required="True" runat="server" ID="CstPXDateTimeEdit13" DataField="WorkDate" CommitChanges="True" ></px:PXDateTimeEdit>
            <px:PXSelector Required="True" runat="server" ID="PXSelector1" DataField="VendorID" Enabled="False"></px:PXSelector>
			<px:PXDropDown Enabled="False" Size="" runat="server" ID="CstPXTextEdit2" DataField="Status" ></px:PXDropDown>
			<px:PXTextEdit Required="True" runat="server" ID="CstPXTextEdit17" DataField="ReqWorkContent" TextMode="MultiLine" Height="100px"></px:PXTextEdit>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule7" StartColumn="True" ></px:PXLayoutRule>
            

			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule14" StartColumn="True" ></px:PXLayoutRule></Template>
	
		<AutoSize Enabled="True" Container="Window" ></AutoSize>
		<AutoSize Enabled="True" ></AutoSize></px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXGrid runat="server" ID="CstPXGrid1" SkinID="Details" Width="100%"  SyncPosition="true" MatrixMode="True">
		<Levels>
			<px:PXGridLevel DataMember="DailyRenterVendors" >
				<Columns>

                    <px:PXGridColumn DataField="VendorID" Width="140" ></px:PXGridColumn>
					<px:PXGridColumn Required="True" DataField="InsteadQty" Width="100" ></px:PXGridColumn>
					<px:PXGridColumn Required="True" DataField="WorkContent" Width="70" ></px:PXGridColumn></Columns>
                <Mode  InitNewRow="true" AllowUpload="true" ></Mode>
			
				<RowTemplate>
					<px:PXSelector runat="server" ID="CstPXSelector27" DataField="VendorID" ></px:PXSelector></RowTemplate></px:PXGridLevel></Levels>
		<AutoSize Container="Window" Enabled="True" ></AutoSize></px:PXGrid></asp:Content>
