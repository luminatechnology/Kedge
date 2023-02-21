<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="NM501000.aspx.cs" Inherits="Page_NM501000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
  <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="NM.NMArCheckCollProcess"
        PrimaryView="CheckFilters"
        >
    <CallbackCommands>

    </CallbackCommands>
  </px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
  <px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="CheckFilters" Width="100%" Height="160px" AllowAutoHide="false">
    <Template>
      <px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartRow="True"></px:PXLayoutRule>
	<px:PXLayoutRule LabelsWidth="100px" runat="server" ID="CstPXLayoutRule13" StartColumn="True" ></px:PXLayoutRule>
	<px:PXLayoutRule LabelsWidth="100px" GroupCaption="Filters" runat="server" ID="CstPXLayoutRule3" StartGroup="True" ></px:PXLayoutRule>
	<px:PXSegmentMask CommitChanges="True" runat="server" ID="CstPXSegmentMask14" DataField="CustomerID" ></px:PXSegmentMask>
	<px:PXSegmentMask CommitChanges="True" runat="server" ID="CstPXSegmentMask23" DataField="ProjectID" ></px:PXSegmentMask>
	<px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector21" DataField="CheckCashierID" ></px:PXSelector>
	<px:PXDateTimeEdit CommitChanges="True" runat="server" ID="CstPXDateTimeEdit22" DataField="DueDate" ></px:PXDateTimeEdit>
	<px:PXLayoutRule runat="server" ID="CstPXLayoutRule12" StartColumn="True" ></px:PXLayoutRule>
	<px:PXLayoutRule GroupCaption="Data" runat="server" ID="CstPXLayoutRule4" StartGroup="True" ></px:PXLayoutRule>
	<px:PXSegmentMask CommitChanges="True" LabelWidth="150px" runat="server" ID="CstPXSegmentMask10" DataField="CollCashierID" ></px:PXSegmentMask>
	<px:PXDateTimeEdit LabelWidth="150px" runat="server" ID="CstPXDateTimeEdit11" DataField="CollCheckDate" ></px:PXDateTimeEdit>
	<px:PXLayoutRule runat="server" ID="CstPXLayoutRule16" StartColumn="True" ></px:PXLayoutRule>
	<px:PXLayoutRule runat="server" ID="CstPXLayoutRule17" StartGroup="True" GroupCaption="Selected Cal" ></px:PXLayoutRule>
	<px:PXNumberEdit runat="server" ID="CstPXNumberEdit19" DataField="SelectedCount" ></px:PXNumberEdit>
	<px:PXNumberEdit runat="server" ID="CstPXNumberEdit20" DataField="SelectedOriCuryAmount" ></px:PXNumberEdit></Template>
  
	<AutoSize Container="Window" ></AutoSize></px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
  <px:PXGrid SyncPosition="True" ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="Details" AllowAutoHide="false">
    <Levels>
      <px:PXGridLevel DataMember="CheckDetails">
          <Columns>
	<px:PXGridColumn AllowCheckAll="True" Type="CheckBox" CommitChanges="True" DataField="Selected" Width="60" ></px:PXGridColumn>
	<px:PXGridColumn DataField="Status" Width="70" ></px:PXGridColumn>
	<px:PXGridColumn DataField="CheckNbr" Width="108" ></px:PXGridColumn>
	<px:PXGridColumn DataField="OriBankCode" Width="70" ></px:PXGridColumn>
	<px:PXGridColumn DataField="OriBankCode_description" Width="180" ></px:PXGridColumn>
	<px:PXGridColumn DataField="CollBankAccountID_NMBankAccount_bankName" Width="180" ></px:PXGridColumn>
	<px:PXGridColumn DataField="CollBankAccountID" Width="180" ></px:PXGridColumn>
	<px:PXGridColumn DataField="CheckDate" Width="90" ></px:PXGridColumn>
	<px:PXGridColumn DataField="EtdDepositDate" Width="90" ></px:PXGridColumn>
	<px:PXGridColumn DataField="DueDate" Width="90" ></px:PXGridColumn>
	<px:PXGridColumn DataField="CuryID" Width="70" ></px:PXGridColumn>
	<px:PXGridColumn DataField="OriCuryAmount" Width="100" ></px:PXGridColumn>
	<px:PXGridColumn DataField="BaseCuryAmount" Width="100" ></px:PXGridColumn>
	<px:PXGridColumn DataField="CustomerID" Width="140" ></px:PXGridColumn>
	<px:PXGridColumn DataField="CustomerID_Customer_acctName" Width="280" ></px:PXGridColumn>
	<px:PXGridColumn DataField="ProjectID" Width="70" ></px:PXGridColumn>
	<px:PXGridColumn DataField="ProjectID_description" Width="220" ></px:PXGridColumn>
	<px:PXGridColumn DataField="Description" Width="280" ></px:PXGridColumn></Columns>
      </px:PXGridLevel>
    </Levels>
    <AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
    <ActionBar >
    
	<Actions>
		<Delete ToolBarVisible="False" ></Delete></Actions>
	<Actions>
		<AddNew ToolBarVisible="False" ></AddNew></Actions></ActionBar>
  </px:PXGrid>
</asp:Content>