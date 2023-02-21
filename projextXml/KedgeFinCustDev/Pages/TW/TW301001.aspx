<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="TW301001.aspx.cs" Inherits="Page_TW301001" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="Fin.TWWHTProcess"
        PrimaryView="Filters"
        >
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="Filters" Width="100%" Height="100px" AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartRow="True"></px:PXLayoutRule>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule1" StartColumn="True" />
			<px:PXSelector CommitChanges="True" runat="server" ID="CstPXSelector3" DataField="PersonalID" ></px:PXSelector>
			<px:PXNumberEdit CommitChanges="True" runat="server" ID="CstPXNumberEdit6" DataField="DeclareYearFrom" ></px:PXNumberEdit>
			<px:PXNumberEdit CommitChanges="True" runat="server" ID="CstPXNumberEdit5" DataField="DeclareMonthFrom" ></px:PXNumberEdit>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule2" StartColumn="True" />
			<px:PXTextEdit runat="server" ID="CstPXTextEdit4" DataField="PayeeName" />
			<px:PXNumberEdit CommitChanges="True" runat="server" ID="CstPXNumberEdit8" DataField="DeclareYearTo" ></px:PXNumberEdit>
			<px:PXNumberEdit CommitChanges="True" runat="server" ID="CstPXNumberEdit7" DataField="DeclareMonthTo" ></px:PXNumberEdit></Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXGrid SyncPosition="True" ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="Inquire" AllowAutoHide="false">
		<Levels>
			<px:PXGridLevel DataMember="WHTView">
			    <Columns>
				<px:PXGridColumn Type="CheckBox" DataField="Selected" Width="60" CommitChanges="True" AllowCheckAll="True" ></px:PXGridColumn>
				<px:PXGridColumn DataField="TranType" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn LinkCommand="viewPersonalID" DataField="PersonalID" Width="120" ></px:PXGridColumn>
				<px:PXGridColumn DataField="PayeeName" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="PayAmt" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="WHTAmt" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="NetAmt" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="GNHI2Amt" Width="100" ></px:PXGridColumn></Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
		<ActionBar >
		</ActionBar>
	</px:PXGrid>
</asp:Content>