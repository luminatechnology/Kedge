<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="GV106000.aspx.cs" Inherits="Page_GV106000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/ListView.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="RCGV.GV.GVPeriodMaint"
        PrimaryView="GVPeriods"
        >
        <CallbackCommands>

        </CallbackCommands>
    </px:PXDataSource>

<px:PXSmartPanel ID="pnlPeriod" runat="server" AcceptButtonID="PXButtonOK" AutoReload="true" CancelButtonID="PXButtonCancel"
  Caption="Period" CaptionVisible="True" DesignView="Content" HideAfterAction="false" Key="PeriodSettings"
  LoadOnDemand="true" Height="190px" Width="360px" DefaultControlID="edRegistration">
  <px:PXFormView ID="PXFormView6" runat="server" CaptionVisible="False" DataMember="PeriodSettings" Width="100%"
      DefaultControlID="edRegistration" DataSourceID="ds" TabIndex="1400">
      <ContentStyle BackColor="Transparent" BorderStyle="None">
      </ContentStyle>
      <Template>
          <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="M" ></px:PXLayoutRule>
          <px:PXSelector Size="M" runat="server" ID="edRegistration" DataField="RegistrationCD" CommitChanges="True" Width="130px" ></px:PXSelector>
	<px:PXMaskEdit Size="S" CommitChanges="True" runat="server" ID="CstPXMaskEdit11" DataField="PeriodYear" ></px:PXMaskEdit>
          <px:PXLayoutRule runat="server" StartRow="True" ></px:PXLayoutRule>
          <px:PXPanel ID="PXPanel1" runat="server" SkinID="Buttons">
              <px:PXButton ID="OK" runat="server" DialogResult="OK" Text="OK" ></px:PXButton>
              <px:PXButton ID="Cancel" runat="server" DialogResult="Cancel" Text="Cancel" ></px:PXButton>
          </px:PXPanel></Template>
  </px:PXFormView>
</px:PXSmartPanel>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phL" runat="Server">
    <px:PXGrid ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="Primary" AllowAutoHide="False" TabIndex="200">
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
            <px:PXGridLevel DataMember="GVPeriods">
                <Columns>
                <px:PXGridColumn CommitChanges="" DataField="RegistrationCD" Width="120" ></px:PXGridColumn>
	<px:PXGridColumn DataField="RegistrationCD_description" Width="70" ></px:PXGridColumn>
                <px:PXGridColumn CommitChanges="True" DataField="PeriodYear" Width="70" ></px:PXGridColumn>
                <px:PXGridColumn CommitChanges="True" DataField="PeriodMonth" Width="50" ></px:PXGridColumn>
	<px:PXGridColumn Type="CheckBox" DataField="InActive" Width="60" ></px:PXGridColumn>
	<px:PXGridColumn Type="CheckBox" DataField="OutActive" Width="60" ></px:PXGridColumn></Columns>
            
	<RowTemplate>
		<px:PXCheckBox runat="server" ID="CstPXCheckBox1" DataField="InActive" AlreadyLocalized="False" ></px:PXCheckBox>
		<px:PXCheckBox runat="server" ID="CstPXCheckBox2" DataField="OutActive" AlreadyLocalized="False" ></px:PXCheckBox>
		<px:PXNumberEdit runat="server" ID="CstPXNumberEdit3" DataField="PeriodMonth" AlreadyLocalized="False" DefaultLocale="" ></px:PXNumberEdit>
		<px:PXNumberEdit runat="server" ID="CstPXNumberEdit4" DataField="PeriodYear" AlreadyLocalized="False" DefaultLocale="" ></px:PXNumberEdit>
		<px:PXSelector runat="server" ID="CstPXSelector5" DataField="RegistrationCD" ></px:PXSelector>
		<px:PXTextEdit runat="server" ID="CstPXTextEdit6" DataField="GovUniformNumber" AlreadyLocalized="False" DefaultLocale="" ></px:PXTextEdit>
		<px:PXTextEdit runat="server" ID="CstPXTextEdit7" DataField="RegistrationCD_GVRegistration_govUniformNumber" AlreadyLocalized="False" DefaultLocale="" ></px:PXTextEdit>
		<px:PXTextEdit runat="server" ID="CstPXTextEdit8" DataField="RegistrationCD_description" AlreadyLocalized="False" DefaultLocale="" ></px:PXTextEdit></RowTemplate></px:PXGridLevel>
        </Levels>
        <AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
        <ActionBar >
            <Actions>
                <AddNew Enabled="False" />
                <Delete Enabled="False" />
            </Actions>
        </ActionBar>
    
        <Mode AllowUpdate="True" AllowDelete="True" AllowAddNew="True" ></Mode>
        <Mode AllowUpdate="True" ></Mode>
        <Mode AllowDelete="True" ></Mode></px:PXGrid>
</asp:Content>