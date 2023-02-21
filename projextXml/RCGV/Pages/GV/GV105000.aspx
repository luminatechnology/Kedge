<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="GV105000.aspx.cs" Inherits="Page_GV105000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="RCGV.GV.GVGuiNumberMaint" PrimaryView="CodeMaster">
        <CallbackCommands>
            <px:PXDSCallbackCommand Name="InsertRow" CommitChanges="True" Visible ="False" />
        </CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" 
		Width="100%" DataMember="CodeMaster" TabIndex="1700">
		<Template>
			<px:PXLayoutRule runat="server" StartRow="True" StartColumn="True"/>

			<px:PXSelector ID="edRegistrationCD" runat="server" DataField="RegistrationCD" AutoRefresh="True" CommitChanges="True">
			</px:PXSelector>

            <px:PXSelector ID="edGuiWordID" runat="server" DataField="GuiWordID" CommitChanges="True" AutoRefresh="True" >
			</px:PXSelector>

		
			<px:PXMaskEdit ID="edApplyStartNumber" runat="server" AlreadyLocalized="False" DataField="ApplyStartNumber" CommitChanges="True" EmptyChar="0">
			</px:PXMaskEdit>
            <px:PXCheckBox CommitChanges="True" runat="server" ID="CstPXCheckBox20" DataField="Hold" AlreadyLocalized="False" ></px:PXCheckBox>

			<px:PXLayoutRule runat="server" StartColumn="True">
			</px:PXLayoutRule>
			

			<px:PXNumberEdit ID="edDeclareYear" runat="server" AlreadyLocalized="False" CommitChanges="True" DataField="DeclareYear">
			</px:PXNumberEdit>


			<px:PXDropDown ID="edDeclarePeriod" runat="server" DataField="DeclarePeriod" AlreadyLocalized="False" CommitChanges="True" Enabled = "False">
			</px:PXDropDown>

		
			<px:PXMaskEdit ID="edApplyEndNumber" runat="server" AlreadyLocalized="False" CommitChanges="True" DataField="ApplyEndNumber" EmptyChar="0">
			</px:PXMaskEdit>
		</Template>
		<CallbackCommands>
<%--			<Refresh CommitChanges="True" />
			<AddNew CommitChanges="True" />--%>
		</CallbackCommands>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXGrid ID="grid" runat="server" DataSourceID="ds" Style="z-index: 100"
		Width="100%" Height="150px" SkinID="Details" TabIndex="900" SyncPosition="True" AutoAdjustColumns="True" MatrixMode="False">
		<ActionBar PagerVisible="False">
              <PagerSettings Mode="NextPrevFirstLast" />
         </ActionBar>
        <Levels>
			<px:PXGridLevel DataKeyNames="GuiNumberID" DataMember="CodeDetails">
				<RowTemplate>
					<px:PXNumberEdit ID="edLineID" runat="server" AlreadyLocalized="False" DataField="LineID" >
					</px:PXNumberEdit>
					<px:PXTextEdit ID="edStartNumber" runat="server" AlreadyLocalized="False" DataField="StartNumber" >
					</px:PXTextEdit>
					<px:PXMaskEdit ID="edEndNumber" runat="server" AlreadyLocalized="False" DataField="EndNumber" CommitChanges="True" EmptyChar="0" DisableSpellcheck="True" >
                       <%--<AutoCallBack Enabled="true" Target="form" Command="Refresh">
                       </AutoCallBack>--%>
					</px:PXMaskEdit>
				</RowTemplate>
				<Columns>
					<px:PXGridColumn DataField="LineNbr" TextAlign="Right"  >
					</px:PXGridColumn>
					<px:PXGridColumn DataField="StartNumber"  >
					</px:PXGridColumn>
					<px:PXGridColumn DataField="EndNumber" CommitChanges="True"  >
                    </px:PXGridColumn>
				</Columns>
			</px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" />
<%--		<CallbackCommands>
			<InitRow CommitChanges="True" />
		</CallbackCommands>--%>
<%--        <AutoCallBack Enabled="true" Target="form" Command="Refresh">			
		</AutoCallBack>--%>
<%--		<OnChangeCommand>
			<Behavior CommitChanges="True" />
		</OnChangeCommand>--%>
        <%-- 
        <ActionBar >
            
        </ActionBar>--%>
		<Mode AllowFormEdit="True" InitNewRow="True" AllowAddNew="True" AllowDelete="True" /> 
        <ActionBar PagerVisible="False">
            <Actions>
                <AddNew MenuVisible ="False" ToolBarVisible="False" />
            </Actions>
            <PagerSettings Mode="NextPrevFirstLast" />
            <CustomItems>
                <px:PXToolBarButton Text="Insert" Key="cmdADI" >
                    <AutoCallBack Command="InsertRow" Target="ds">
                        <Behavior CommitChanges="True"   PostData="Page" />
                    </AutoCallBack>
                </px:PXToolBarButton>
            </CustomItems>
        </ActionBar>
        <%-- 
        <ActionBar >
			<CustomItems>
				<px:PXToolBarButton Text="Insert">
				    <AutoCallBack Command="InsertRow" Target="ds" />
				</px:PXToolBarButton>
			</CustomItems>
		</ActionBar>--%>
	</px:PXGrid>
</asp:Content>
