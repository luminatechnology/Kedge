<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="GV103000.aspx.cs" Inherits="Page_AT103000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" TypeName="RCGV.GV.GVRegistrationMaint" PrimaryView="Registrations" SuspendUnloading="False">
		<CallbackCommands>
			<px:PXDSCallbackCommand Name="Delete" Visible="False">
			</px:PXDSCallbackCommand>
		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" 
		Width="100%" DataMember="Registrations" TabIndex="5500">
		<Template>
			<px:PXLayoutRule runat="server" StartRow="True" ControlSize="M" LabelsWidth="M" StartColumn="True"></px:PXLayoutRule>
			<px:PXSelector ID="edRegistrationCD" runat="server" DataField="RegistrationCD" CommitChanges="True">
			</px:PXSelector>
			<px:PXTextEdit ID="edSiteNameChinese" runat="server" AlreadyLocalized="False" DataField="SiteNameChinese" CommitChanges="True">
			</px:PXTextEdit>
			<px:PXTextEdit ID="edTaxPayer" runat="server" AlreadyLocalized="False" DataField="TaxPayer" CommitChanges="True">
			</px:PXTextEdit>
			<px:PXLayoutRule runat="server" ColumnSpan="2">
			</px:PXLayoutRule>
			<px:PXTextEdit ID="edSiteAddress" runat="server" AlreadyLocalized="False" DataField="SiteAddress" DefaultLocale="" CommitChanges="True">
			</px:PXTextEdit>
			<px:PXMaskEdit ID="edSiteTelephone" runat="server" AlreadyLocalized="False" DataField="SiteTelephone" DefaultLocale="" CommitChanges="True">
			</px:PXMaskEdit>
			<px:PXDropDown ID="edDeclarationMethod" runat="server" DataField="DeclarationMethod" CommitChanges="True">
			</px:PXDropDown>
			<px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector1" DataField="ParentRegistrationCD" ></px:PXSelector>
			<px:PXLayoutRule runat="server" ColumnSpan="2">
			</px:PXLayoutRule>
			<px:PXTextEdit ID="edQrCodeSeedString" runat="server" AlreadyLocalized="False" DataField="QrCodeSeedString" DefaultLocale="" CommitChanges="True">
			</px:PXTextEdit>
			<px:PXCheckBox runat="server" ID="CstPXCheckBox2" DataField="IsActive" ></px:PXCheckBox>
			<px:PXLayoutRule runat="server" StartColumn="True" ControlSize="M" LabelsWidth="M">
			</px:PXLayoutRule>
			<px:PXMaskEdit ID="edGovUniformNumber" runat="server" AlreadyLocalized="False" DataField="GovUniformNumber" DefaultLocale="" CommitChanges="True">
			</px:PXMaskEdit>
			<px:PXTextEdit ID="edSiteNameEnglish" runat="server" AlreadyLocalized="False" DataField="SiteNameEnglish" DefaultLocale="">
			</px:PXTextEdit>
			<px:PXDropDown ID="edTaxCityCode" runat="server" DataField="TaxCityCode" CommitChanges="True">
			</px:PXDropDown>
			<px:PXSelector runat="server" ID="CstPXSelector3" DataField="TaxAuthority" />
			<px:PXDropDown ID="edDeclarationPayCode" runat="server" DataField="DeclarationPayCode" CommitChanges="True">
			</px:PXDropDown>
			<px:PXDropDown runat="server" ID="CstPXDropDown2" DataField="SpecialTaxType" CommitChanges="True" ></px:PXDropDown></Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXGrid ID="grid" runat="server" DataSourceID="ds" Style="z-index: 100" 
		Width="100%" Height="150px" SkinID="Details" TabIndex="3700">
<EmptyMsg ComboAddMessage="No records found.
Try to change filter or modify parameters above to see records here." NamedComboMessage="No records found as &#39;{0}&#39;.
Try to change filter or modify parameters above to see records here." NamedComboAddMessage="No records found as &#39;{0}&#39;.
Try to change filter or modify parameters above to see records here." FilteredMessage="No records found.
Try to change filter to see records here." FilteredAddMessage="No records found.
Try to change filter to see records here." NamedFilteredMessage="No records found as &#39;{0}&#39;.
Try to change filter to see records here." NamedFilteredAddMessage="No records found as &#39;{0}&#39;.
Try to change filter to see records here." AnonFilteredMessage="No records found.
Try to change filter to see records here." AnonFilteredAddMessage="No records found.
Try to change filter to see records here."></EmptyMsg>
		<Levels>
			<px:PXGridLevel DataMember="baccounts">
				<RowTemplate>
					<px:PXSelector ID="edBAccountID" runat="server" AutoRefresh="True" CommitChanges="True" DataField="BAccountID">
					</px:PXSelector>
					<px:PXTextEdit ID="edBAccount__AcctName" runat="server" AlreadyLocalized="False" DataField="BAccount__AcctName" DefaultLocale="">
					</px:PXTextEdit>
				</RowTemplate>
				<Columns>
					<px:PXGridColumn DataField="BAccountID" CommitChanges="True">
					</px:PXGridColumn>
					<px:PXGridColumn DataField="BAccount__AcctName" CommitChanges="True">
					</px:PXGridColumn>
				</Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" />
	</px:PXGrid>
</asp:Content>
