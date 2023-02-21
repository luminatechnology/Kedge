<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="NM502008.aspx.cs" Inherits="Page_NM502008" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="NM.NMApBankFeedbackProcess" PrimaryView="MasterView">
		<CallbackCommands>
		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
 <px:PXUploadDialog AllowedFileTypes=".txt" IgnoreSize="True" RenderComment="False" RenderLink="False" ID="pnlNewRev" runat="server" Height="70px" 
		Style="position: static" Width="450px" Caption="Import Text File(*.txt)"
		Key="UploadDialog" SessionKey="FeedbackSessionKey" 
		AutoSaveFile="false" RenderCheckIn="false" ></px:PXUploadDialog>
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="MasterView" Width="100%" Height="50px" AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartRow="True"></px:PXLayoutRule>
			<px:PXDropDown runat="server" ID="CstPXDropDown1" DataField="FeedbackType" Size="M" ></px:PXDropDown></Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXGrid ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="Details" AllowAutoHide="false">
		<Levels>
			<px:PXGridLevel DataMember="DetailsView">
			    <Columns>
				<px:PXGridColumn DataField="CustNbr" Width="70" />
				<px:PXGridColumn DataField="UploadKey" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Date" Width="90" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CheckNbr" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="TnTtStatus" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="TnBankCheckStatus" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="GtTtStatus" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Reason" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="ErrorMsg" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Amount" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="Receiver" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="RecBankCode" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="RecAccount" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="RemittanceNbr" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="TranTime" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="TransFee" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="TranDate" Width="90" ></px:PXGridColumn></Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
		<ActionBar >
			<Actions>
				<AddNew Enabled="False" ></AddNew></Actions>
			<Actions>
				<Delete Enabled="False" ></Delete></Actions>
			<Actions>
				<Refresh Enabled="False" ></Refresh></Actions></ActionBar>
	</px:PXGrid>
</asp:Content>