<%@ Page Language="C#" MasterPageFile="~/MasterPages/ListView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="KG105001.aspx.cs" Inherits="Page_KG105001" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/ListView.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="Kedge.KGVoucherDigestSetting"
        PrimaryView="PostageSetups"
        >
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phL" runat="Server">
	<px:PXTab Width="100%" DataMember="PostageSetups" runat="server" ID="CstPXTab1">
		<Items>
			<px:PXTabItem Text="Accounting Setting">
				<Template>
					<px:PXLayoutRule runat="server" ID="CstPXLayoutRule14" StartGroup="True" GroupCaption="WebService WSDL Setting" ></px:PXLayoutRule>
					<px:PXFormView Width="100%" DataMember="PostageSetups" runat="server" ID="CstFormView2" >
						<Template>
							<px:PXLayoutRule runat="server" ID="CstPXLayoutRule26" StartColumn="True" ></px:PXLayoutRule>
							<px:PXSelector Size="XM"  runat="server" ID="CstPXSelector12" DataField="KGPostageAccountID" ></px:PXSelector>
							<px:PXSelector runat="server" ID="CstPXSelector37" DataField="KGTTFeeAccountID" Size="XM"  ></px:PXSelector>
							<px:PXSelector  Size="XM" runat="server" ID="CstPXSelector11" DataField="KGCheckAccountID" ></px:PXSelector>
							<px:PXSelector Size="XM"  runat="server" ID="CstPXSelector10" DataField="KGCashAccountID" ></px:PXSelector>
							<px:PXSelector runat="server" ID="CstPXSelector32" DataField="KGTTAccountID" Size="XM"  ></px:PXSelector>
							<px:PXSelector runat="server" ID="CstPXSelector33" DataField="KGVenCouponAccountID" Size="XM"  ></px:PXSelector>
							<px:PXSelector runat="server" ID="CstPXSelector31" DataField="KGEmpCouponAccountID" Size="XM"  ></px:PXSelector>
							<px:PXSelector runat="server" ID="CstPXSelector40" DataField="KGAuthAccountID" ></px:PXSelector>
							<px:PXSelector runat="server" ID="CstPXSelector42" DataField="KGTempWriteoffAccountID" />
							<px:PXLayoutRule runat="server" ID="CstPXLayoutRule27" StartColumn="True" ></px:PXLayoutRule>
							<px:PXSelector runat="server" ID="CstPXSelector30" DataField="KGPostageSubID"  ></px:PXSelector>
							<px:PXSelector runat="server" ID="CstPXSelector39" DataField="KGTTFeeSubID"  ></px:PXSelector>
							<px:PXSelector runat="server" ID="CstPXSelector29" DataField="KGCheckSubID"  ></px:PXSelector>
							<px:PXSelector runat="server" ID="CstPXSelector28" DataField="KGCashSubID"  ></px:PXSelector>
							<px:PXSelector runat="server" ID="CstPXSelector35" DataField="KGTTSubID"  ></px:PXSelector>
							<px:PXSelector runat="server" ID="CstPXSelector36" DataField="KGVenCouponSubID"  ></px:PXSelector>
							<px:PXSelector runat="server" ID="CstPXSelector34" DataField="KGEmpCouponSubID"  ></px:PXSelector>
							<px:PXSelector runat="server" ID="CstPXSelector41" DataField="KGAuthSubID" ></px:PXSelector>
							<px:PXSelector runat="server" ID="CstPXSelector43" DataField="KGTempWriteoffSubID" /></Template>
						<AutoSize Container="Window" ></AutoSize></px:PXFormView>
					<px:PXLayoutRule runat="server" ID="CstPXLayoutRule16" StartGroup="True" GroupCaption="Postage Settimg" ></px:PXLayoutRule>
					<px:PXFormView Width="100%" runat="server" ID="CstFormView17" DataMember="PostageSetups">
						<Template>
							<px:PXLayoutRule runat="server" ID="CstPXLayoutRule18" StartRow="True" ></px:PXLayoutRule>
							<px:PXLayoutRule runat="server" ID="CstPXLayoutRule19" StartColumn="True" ></px:PXLayoutRule>
							<px:PXNumberEdit runat="server" ID="CstPXNumberEdit23" DataField="CostIntervalEnd" ></px:PXNumberEdit>
							<px:PXNumberEdit runat="server" ID="CstPXNumberEdit25" DataField="CostInterval" ></px:PXNumberEdit>
							<px:PXNumberEdit runat="server" ID="CstPXNumberEdit24" DataField="LongTerm" ></px:PXNumberEdit>
							<px:PXLayoutRule runat="server" ID="CstPXLayoutRule20" StartColumn="True" ></px:PXLayoutRule>
							<px:PXNumberEdit runat="server" ID="CstPXNumberEdit21" DataField="FirstPostCost" ></px:PXNumberEdit>
							<px:PXNumberEdit runat="server" ID="CstPXNumberEdit22" DataField="CostIntervalPostCost" ></px:PXNumberEdit></Template></px:PXFormView></Template></px:PXTabItem>
			<px:PXTabItem Text="Voucher Digest Setting">
				<Template>
					<px:PXGrid  SkinID="Details" Width="100%" PageSize="30" runat="server" ID="CstPXGrid3">
						<Levels>
							<px:PXGridLevel DataMember="VoucherDigestSetups" >
								<Columns>
									<px:PXGridColumn DataField="AccountID" Width="120" ></px:PXGridColumn>
									<px:PXGridColumn DataField="AccountCD" Width="70" />
									<px:PXGridColumn DataField="OracleAccountCD" Width="120" ></px:PXGridColumn>
									<px:PXGridColumn DataField="OracleDigestRule" Width="280" ></px:PXGridColumn>
									<px:PXGridColumn DataField="IsProjectCode" Width="60" Type="CheckBox" ></px:PXGridColumn></Columns></px:PXGridLevel></Levels>
						<AutoSize Container="Window" ></AutoSize></px:PXGrid></Template></px:PXTabItem></Items>
		<AutoSize Container="Window" /></px:PXTab></asp:Content>