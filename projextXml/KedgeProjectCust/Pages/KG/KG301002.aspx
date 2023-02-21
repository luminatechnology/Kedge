<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="KG301002.aspx.cs" Inherits="Page_KG301002" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormView.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="Kedge.KGDailyRenterActualEntry"
        PrimaryView="DailyRenters"
        >
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="DailyRenters" Width="100%" AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartRow="True"></px:PXLayoutRule>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule1" StartColumn="True" ></px:PXLayoutRule>
			<px:PXSelector Size="SM" runat="server" ID="CstPXSelector2" DataField="ContractID" Enabled="false" ></px:PXSelector>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule3" StartRow="True" ></px:PXLayoutRule>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule4" StartColumn="True" ></px:PXLayoutRule>
			<px:PXSelector runat="server" ID="CstPXSelector32" DataField="DailyRenterCD" ></px:PXSelector>
			<px:PXSelector CommitChanges="True" Required="True" runat="server" ID="CstPXSelector18" Enabled="false"  DataField="OrderNbr"  ></px:PXSelector>
			<px:PXSelector Required="True" runat="server" ID="CstPXSelector19" DataField="LineNbr" Enabled="false" ></px:PXSelector>
			<px:PXNumberEdit Enabled="False" runat="server" ID="CstPXNumberEdit15" DataField="ReqQty" ></px:PXNumberEdit>
			<px:PXNumberEdit Enabled="False" runat="server" ID="CstPXNumberEdit16" DataField="ReqSelfQty" ></px:PXNumberEdit>
			<px:PXNumberEdit Enabled="False" runat="server" ID="CstPXNumberEdit14" DataField="ReqInsteadQty" ></px:PXNumberEdit>
			<px:PXTextEdit Enabled="False" runat="server" ID="CstPXTextEdit21" DataField="ReqWorkContent" TextMode="MultiLine" Height="100px" Required="false"></px:PXTextEdit>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit33" DataField="CreatedByID_Creator_displayName" ></px:PXTextEdit>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule5" StartColumn="True" ></px:PXLayoutRule>
			<px:PXDateTimeEdit Enabled="False" runat="server" ID="CstPXDateTimeEdit7" DataField="WorkDate" ></px:PXDateTimeEdit>
			<px:PXSelector Required="True" runat="server" ID="PXSelector1" DataField="VendorID" Enabled="False"></px:PXSelector>
			<px:PXDropDown Enabled="False" Size="" runat="server" ID="CstPXTextEdit2" DataField="Status" ></px:PXDropDown>
			<px:PXNumberEdit Enabled="False" CommitChanges="True" runat="server" ID="CstPXNumberEdit19" DataField="ActQty" ></px:PXNumberEdit>
			<px:PXNumberEdit Enabled="False" CommitChanges="True" runat="server" ID="CstPXNumberEdit20" DataField="ActSelfQty"  ></px:PXNumberEdit>

			<px:PXNumberEdit Enabled="False" runat="server" ID="CstPXNumberEdit18" DataField="ActInsteadQty" ></px:PXNumberEdit>
			<px:PXTextEdit CommitChanges="True" Enabled="" runat="server" ID="CstPXTextEdit22" DataField="ActWorkContent" TextMode="MultiLine" Height="100px" Required="true" ></px:PXTextEdit>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule11" StartRow="True" ></px:PXLayoutRule>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule12" StartColumn="True" ></px:PXLayoutRule>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule13" StartColumn="True" ></px:PXLayoutRule></Template>
		<AutoSize Container="Window" Enabled="True" MinHeight="200" ></AutoSize>
	</px:PXFormView>
	<px:PXFormView runat="server" ID="RenterVendors">
		<Template>
			<px:PXLayoutRule GroupCaption="Apply Vendor" runat="server" ID="CstPXLayoutRule26" StartGroup="True" ></px:PXLayoutRule>
			<px:PXGrid SkinID="Details" Width="100%" runat="server" ID="CstPXGrid28" SyncPosition ="true" MatrixMode ="true">
				<Levels>
					<px:PXGridLevel DataMember="DailyRenterReqVendors" >
						<Columns>
							<px:PXGridColumn Type="CheckBox" DataField="Selected" Width="60" ></px:PXGridColumn>

                            <px:PXGridColumn DataField="VendorID" Width="140" ></px:PXGridColumn>
							<px:PXGridColumn DataField="InsteadQty" Width="100" ></px:PXGridColumn>
							<px:PXGridColumn DataField="WorkContent" Width="70" ></px:PXGridColumn></Columns>
						<RowTemplate>
	
                            <px:PXNumberEdit runat="server" ID="PXNumberEdit1" DataField="InsteadQty" ></px:PXNumberEdit>
                            <px:PXTextEdit runat="server" ID="PXTextEdit1" DataField="WorkContent" TextMode="MultiLine" Height="100px" Required="true" />
						</RowTemplate>
                        <Mode  InitNewRow="true" AllowUpload ="false"  />

					</px:PXGridLevel>

				</Levels>
				<AutoSize Container="Window"  Enabled="True" ></AutoSize>
	        </px:PXGrid>
			<px:PXLayoutRule GroupCaption="Actual Vendor" runat="server" ID="CstPXLayoutRule27" StartGroup="True" ></px:PXLayoutRule>
			<px:PXGrid SkinID="Details" Width="100%" runat="server" ID="CstPXGrid29" SyncPosition ="true" MatrixMode ="true">
				<Levels>
					<px:PXGridLevel DataMember="DailyRenterActVendors" >
						<Columns>
							<px:PXGridColumn Type="CheckBox" DataField="Selected" Width="60" ></px:PXGridColumn>
							<px:PXGridColumn DataField="OrderNbr" Required="True" Width="140" CommitChanges="true"  ></px:PXGridColumn>
                            <px:PXGridColumn DataField="VendorID" Width="140" ></px:PXGridColumn>
							<px:PXGridColumn CommitChanges="True" DataField="InsteadQty" Required="True" Width="100" ></px:PXGridColumn>
							<px:PXGridColumn DataField="WorkContent" Required="True" Width="70" ></px:PXGridColumn></Columns>
						<RowTemplate>
                            <px:PXSelector runat="server" ID="PXSelector2" DataField="OrderNbr" CommitChanges="true" AutoRefresh="True" />
							<px:PXSelector runat="server" ID="CstPXSelector33" DataField="LineNbr" CommitChanges="true" AutoRefresh="True" />
                            <px:PXNumberEdit runat="server" ID="CstPXNumberEdit19" DataField="InsteadQty" ></px:PXNumberEdit>
                            <px:PXTextEdit runat="server" ID="CstPXTextEdit22" DataField="WorkContent" TextMode="MultiLine" Height="100px" Required="true" />
						</RowTemplate>


                        <Mode  InitNewRow="true" AllowUpload ="true"   />
					</px:PXGridLevel>

				</Levels>
				<AutoSize Container="Window"  Enabled="True" ></AutoSize>

			</px:PXGrid>

		</Template>

	</px:PXFormView>

</asp:Content>

