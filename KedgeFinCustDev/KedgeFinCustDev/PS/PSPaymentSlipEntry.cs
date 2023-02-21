using System;
using PX.Data;
using PS.DAC;
using PX.Objects.IN;
using static PS.Util.PSStringList;
using PX.Objects.AR;
using PX.Objects.CA;
using System.Collections;
using NM.DAC;
using System.Text.RegularExpressions;
using PX.Objects.CT;
using RC.Util;
using PX.Web.UI;
using CC;
using CC.DAC;
using PX.Objects.PO;
using System.Collections.Generic;
using PX.Objects.EP;
using NM.Util;
using PX.Objects.PM;
using static CC.Util.CCList;
using PX.Objects.CR;
using Kedge.DAC;

namespace PS
{
    /**
    * =====2021-02-05:11916===== Alton
    * 1.PSPaymentSlip.DocType='OTH'時，不需填寫「繳款單分類」及「銀行帳號簡碼」
    * 2.PSPaymentSlip.DocType='RGU'時：
    *   - GuarPayableCD 的LOV內只能挑選到 CCPayableCheck.Status=Release 的應付保證票
    *   - 「提交」時，請update CCPayableCheck.Status=AppliedRTN
    *   - 「暫時擱置」時，請將 CCPayableCheck.Status從AppliedRTN變回Release
    *   - 「過帳」時，請update CCPayableCheck.Status= Return
    * 
    * =====2021-02-23:11916===== Alton
    *  "移除"「過帳」時，請update CCPayableCheck.Status= Return
    * Return的動作會請資金到CC的畫面作業
    * 
    * ====2021-05-18:12043====Alton
    * 1.應收保證票新增下列欄位
    *   CheckIssuer nvarchar(255)
    *   OriBankCode nvarchar(7)
    *   OriBankAccount nvarchar(15)
    * 2.請將 PSPaymentSlipDetails.CheckIssuer
    *       PSPaymentSlipDetails.OriBankCode
    *       PSPaymentSlipDetails.OriBankAccount
    *    寫入對應的欄位
    * 3.請將新增的三個欄位放到ContractorID上面, 依序排下來OriBankCode, OriBankAccount, CheckIssuer
    * 4.這三個欄位先 readonly
    * 
    * ====2021-05-19:口頭====Alton
    * 將以下欄位對調Tab(包含控制)
    * 發票人checkIssuer
    * 開票銀行oriBankCode
    * 開票銀行帳號oriBankAccount
    * 
    * ====2021-05-21:12051====Alton
    * 1.一般繳款繳款單過帳時, 原來是一個明細產生一個APPayment, 請調整為依照明細的付款方式+BankAccountID Group by, 相同付款方式+BankAccountID 的放在同一張ARPayment上
    * 2.原會將PaymentRefNbr放到ARPayment的ExtRefNbr, 因為會有多張, 請先不用放
    * 
    * ====2021-05-26:12039====Alton
    * DocType='GUR'時，請將表頭的customer/vendor name 帶到 PSPayementslipDetail. CheckIssuer,
    * 
    * ====2021-05-26:12038====Alton
    * PSPaymentSlipDetails. ActualDueDate 預設值＝PSPaymentSlipDetails.DueDate
    * 
    * ====2021-05-31:12059====Alton
    * 1. PS301010 繳款單明細 針對 "一般繳款" 與 "其他" 須改回 Radio Button模式
    *    a. 選項是: 客戶、供應商、員工
    *       對應要顯示 可以選擇 客戶、供應商、員工
    *       (把畫面上 EmployeeID 請求者 拉過來給員工用)
    *    
    *       註: 針對"應收保證票" 與"繳回應付保證票" 仍然維持 客戶+供應商only
    * 
    *    b. 畫面上要放出來顯示 CreateBy (ID + Desc)
    * 
    * 2. AR Pay的PS Tab 上的請求者 須判斷 RadioButton 是否選為員工
    *       Yes --> 抓EmployeeID
    *       No --> 抓Create By
    *  
    *    AR Pay的 Grid 放出 供應商 欄位
    * 
    * 
    * 3. KG100000 KG偏好設定須新增一個Group 'PS繳款單設定'
    *    放兩個欄位 分別是 '供應商代表客戶' 與 '員工代表客戶'
    * 
    *    PS301010 過帳產生 ARPay時
    *    Radio Button = 客戶 則 copy PS上的客戶
    *    Radio Button = 供應商、員工 則 copy KG100000 分別設定的 代表客戶
    * 
    *    請協助看一下 如果沒設定代表客戶，直接拋立AR Pay的錯誤訊息是不是一般IT人看得懂的
    *    看不懂的話麻煩做個檢查: "尚未設定供應商、員工應收代表客戶，請於KG偏好設定指定"
    * 
    * 
    * 4. PS to CCR
    *    原本抓EmplyeeID 的經辦人 & 作業人，須改抓 Create By
    *    註: PS to CCP Return 目前本來就不會去更新 經辦人 & 作業人，故不做處理
    *    
    * ====2021-07-08: 12140====Alton
    * DocType=應收保證票時, 請將 資金收款資料tab內的「實際到期日」與「到期日」的連動拔掉 , 並請預設為空
    * ====2021-07-12:12146====Alton
    * 選定專案合約後, 預帶 專案內預設客戶 的連動拔掉
    * 
    * ====2021-07-14:12153====Alton
    * 工商本票取號提前至PS提交時執行
    * 
    * ====2021-10-05:12253====Alton
    * 1.請在PSPaymentSlipDetails新增一欄位 CCPostageAmt,decimal(18,6),非必填,預設=0
    *  -當此欄金額異動時，記得將PSPaymentSlip.DocBal的金額減少
    * 2.PS過帳後請將PSPaymentSlipDetails.CCPostageAmt的值寫入CCReceivableCheck.CCPostageAmt內
    * **/
    public class PSPaymentSlipEntry : PXGraph<PSPaymentSlipEntry, PSPaymentSlip>
    {

        private const String CHECK = "CHECK";

        public PSPaymentSlipEntry()
        {
            if (!RCFeaturesSetUtil.IsActive(this, RCFeaturesSetProperties.PAYMENT_SLIP))
            {
                RCFeaturesSetUtil.BackToHomePage();
            }
            this.ActionMenu.MenuAutoOpen = true;

            this.ActionMenu.AddMenuAction(this.Release);
            this.ActionMenu.AddMenuAction(this.Submit);
            this.ActionMenu.AddMenuAction(this.PutOnHold);
            this.ActionMenu.AddMenuAction(this.VoidBtn);
        }


        #region Menu
        public PXAction<PSPaymentSlip> ActionMenu;

        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Action")]
        protected void actionMenu()
        {

        }
        #endregion

        #region Button
        public PXAction<PSPaymentSlip> Release;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Release")]
        protected IEnumerable release(PXAdapter adapter)
        {
            PSPaymentSlip ps = PaymentSlips.Current;
            PaymentSlips.Update(ps);
            foreach (PSPaymentSlipDetails item in PaymentSlipDetails.Select())
            {
                PaymentSlipDetails.Update(item);
            }
            base.Persist();
            PXLongOperation.StartOperation(this, delegate ()
            {
                using (PXTransactionScope ts = new PXTransactionScope())
                {
                    ps.Status = PSStatus.Released;
                    //
                    if (ps.DocType == PSDocType.Payment)
                    {
                        CreateARPayment(ps);
                    }
                    else
                    {
                        foreach (PSPaymentSlipDetails item in PaymentSlipDetails.Select())
                        {
                            switch (ps.DocType)
                            {
                                //case PSDocType.Payment:
                                //    item.ArPaymentRefNbr = CreateARPayment(item, ps);
                                //    break;
                                case PSDocType.ArGuarCheck:
                                    //2021-07-14 改由PS取號
                                    String guarNbr = "";
                                    item.GuarReceviableCD = CreateCCReceivable(item, ps, ref guarNbr);
                                    //item.PaymentRefNbr = guarNbr;
                                    break;
                                case PSDocType.ApRtnGuarCheck:
                                    UpdateCCPayableCheckStatus(CCPayableRtnStatus.Return);
                                    break;
                                default:
                                    break;
                            }
                            PaymentSlipDetails.Update(item);
                        }
                    }


                    PaymentSlips.Update(ps);
                    base.Persist();
                    ts.Complete();
                }
            });
            return adapter.Get();
        }

        public PXAction<PSPaymentSlip> Submit;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Submit")]
        protected void submit()
        {
            using (PXTransactionScope ts = new PXTransactionScope())
            {
                base.Persist();
                PSPaymentSlip ps = PaymentSlips.Current;
                ps.Status = PSStatus.Open;
                PaymentSlips.Update(ps);
                UpdateGuarNbrByCommercialPaper();
                base.Persist();
                UpdateCCPayableCheckStatus(CCPayableRtnStatus.AppliedRTN);
                ts.Complete();
            }
        }

        public PXAction<PSPaymentSlip> VoidBtn;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Void")]
        protected void voidBtn()
        {
            base.Persist();
            PSPaymentSlip ps = PaymentSlips.Current;
            ps.Status = PSStatus.Voided;
            ps.VoidedBy = this.Accessinfo.UserID;
            ps.VoidedDate = this.Accessinfo.BusinessDate;
            PaymentSlips.Update(ps);
            base.Persist();
        }

        public PXAction<PSPaymentSlip> PutOnHold;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Put On Hold")]
        protected void putOnHold()
        {
            using (PXTransactionScope ts = new PXTransactionScope())
            {
                base.Persist();
                PSPaymentSlip ps = PaymentSlips.Current;
                ps.Status = PSStatus.Hold;
                PaymentSlips.Update(ps);
                base.Persist();
                UpdateCCPayableCheckStatus(CCPayableRtnStatus.Release);
                ts.Complete();
            }
        }

        public PXAction<PSPaymentSlip> PrintPaymentSlip;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "PrintPaymentSlip")]
        protected void printPaymentSlip()
        {
            PSPaymentSlip ps = PaymentSlips.Current;
            if (PaymentSlips.Cache.GetStatus(ps) == PXEntryStatus.Inserted) return;
            Dictionary<string, string> patams = new Dictionary<string, string>();
            patams["RefNbr"] = ps.RefNbr;
            throw new PXReportRequiredException(patams, "PS601000", "Report");
        }

        #endregion

        #region HyperLink
        public PXAction<PSPaymentSlipDetails> ViewInventoryItem;
        [PXButton(CommitChanges = true)]
        [PXUIField(Visible = false)]
        protected virtual void viewInventoryItem()
        {
            PSPaymentSlipDetails d = PaymentSlipDetails.Current;
            // Creating the instance of the graph
            NonStockItemMaint graph = PXGraph.CreateInstance<NonStockItemMaint>();
            // Setting the current product for the graph
            graph.Item.Current = graph.Item.Search<InventoryItem.inventoryID>(d.InventoryID);
            // If the product is found by its ID, throw an exception to open
            // a new window (tab) in the browser
            if (graph.Item.Current != null)
            {
                throw new PXRedirectRequiredException(graph, "Inventory Item")
                {
                    Mode = PXBaseRedirectException.WindowMode.NewWindow
                };
            }
        }

        public PXAction<PSPaymentSlipDetails> ViewArPayment;
        [PXButton(CommitChanges = true)]
        [PXUIField(Visible = false)]
        protected virtual void viewArPayment()
        {
            PSPaymentSlipDetails d = PaymentSlipDetails.Current;
            if (d.ArPaymentRefNbr == null) return;
            // Creating the instance of the graph
            ARPaymentEntry graph = PXGraph.CreateInstance<ARPaymentEntry>();
            // Setting the current product for the graph
            graph.Document.Current = graph.Document.Search<ARPayment.refNbr>(d.ArPaymentRefNbr, new object[] { ARDocType.Payment });
            // If the product is found by its ID, throw an exception to open
            // a new window (tab) in the browser
            if (graph.Document.Current != null)
            {
                throw new PXRedirectRequiredException(graph, "AR Payment")
                {
                    Mode = PXBaseRedirectException.WindowMode.NewWindow
                };
            }
        }

        public PXAction<PSPaymentSlipDetails> ViewCCReceivable;
        [PXButton(CommitChanges = true)]
        [PXUIField(Visible = false)]
        protected virtual void viewCCReceivable()
        {
            PSPaymentSlipDetails d = PaymentSlipDetails.Current;
            if (d.GuarReceviableCD == null) return;
            // Creating the instance of the graph
            CCReceivableEntry graph = PXGraph.CreateInstance<CCReceivableEntry>();
            // Setting the current product for the graph
            graph.ReceivableChecks.Current = graph.ReceivableChecks.Search<CCReceivableCheck.guarReceviableCD>(d.GuarReceviableCD);
            // If the product is found by its ID, throw an exception to open
            // a new window (tab) in the browser
            if (graph.ReceivableChecks.Current != null)
            {
                throw new PXRedirectRequiredException(graph, "CC Receivable Entry")
                {
                    Mode = PXBaseRedirectException.WindowMode.NewWindow
                };
            }
        }

        #endregion

        #region View
        public PXSelect<PSPaymentSlip> PaymentSlips;
        //[PXCopyPasteHiddenFields(
        //    typeof(PSPaymentSlipDetails.guarReceviableCD),
        //    typeof(PSPaymentSlipDetails.arPaymentRefNbr)
        //    )]
        public PXSelect<PSPaymentSlipDetails, Where<PSPaymentSlipDetails.refNbr, Equal<Current<PSPaymentSlip.refNbr>>>> PaymentSlipDetails;
        #endregion

        #region Event

        #region PSPaymentSlip
        protected virtual void _(Events.RowSelected<PSPaymentSlip> e)
        {
            PSPaymentSlip row = (PSPaymentSlip)e.Row;
            if (row == null) return;
            setColumnUI(row);
        }

        protected virtual void _(Events.RowPersisting<PSPaymentSlip> e)
        {
            PSPaymentSlip row = (PSPaymentSlip)e.Row;
            if (row == null || e.Cache.GetStatus(row) == PXEntryStatus.Deleted) return;
            if (row.DocType == PSDocType.Payment && row.TargetType != PSTargetType.Customer && row.Status == PSStatus.Hold)
            {
                KGSetUp setup = GetKGSetUp();
                if (setup == null || setup.DefEmployeeCustID == null || setup.DefVendorCustID == null)
                {
                    SetError<PSPaymentSlip.targetType>(e.Cache, row, row.TargetType, "尚未設定供應商、員工應收代表客戶，請於KG偏好設定指定", PXErrorLevel.RowError);
                }
                if (row.TargetType == PSTargetType.Vendor)
                    e.Cache.SetValueExt<PSPaymentSlip.customerID>(row, setup.DefVendorCustID);
                else if (row.TargetType == PSTargetType.Employee)
                    e.Cache.SetValueExt<PSPaymentSlip.customerID>(row, setup.DefEmployeeCustID);
            }
        }

        protected virtual void _(Events.FieldUpdated<PSPaymentSlip, PSPaymentSlip.employeeID> e)
        {
            PSPaymentSlip row = (PSPaymentSlip)e.Row;
            if (row == null) return;
            e.Cache.SetDefaultExt<PSPaymentSlip.departmentID>(row);
            e.Cache.SetDefaultExt<PSPaymentSlip.locationID>(row);
            e.Cache.SetDefaultExt<PSPaymentSlip.branchID>(row);
        }

        protected virtual void _(Events.FieldUpdated<PSPaymentSlip, PSPaymentSlip.docType> e)
        {
            PSPaymentSlip row = (PSPaymentSlip)e.Row;
            if (row == null) return;
            e.Cache.SetValueExt<PSPaymentSlip.targetType>(row, PSTargetType.Customer);
            foreach (PSPaymentSlipDetails item in PaymentSlipDetails.Select())
            {
                PaymentSlipDetails.Cache.SetValueExt<PSPaymentSlipDetails.guarClass>(item, null);
                PaymentSlipDetails.Cache.SetValueExt<PSPaymentSlipDetails.guarType>(item, null);
                PaymentSlipDetails.Cache.SetValueExt<PSPaymentSlipDetails.pONbr>(item, null);
                PaymentSlipDetails.Cache.SetValueExt<PSPaymentSlipDetails.pOOrderType>(item, null);
                PaymentSlipDetails.Cache.SetValueExt<PSPaymentSlipDetails.issueDate>(item, null);
                PaymentSlipDetails.Cache.SetValueExt<PSPaymentSlipDetails.authDate>(item, null);
                PaymentSlipDetails.Cache.SetValueExt<PSPaymentSlipDetails.paymentCategory>(item, null);
                PaymentSlipDetails.Cache.SetValueExt<PSPaymentSlipDetails.paymentMethodID>(item, null);
            }
        }

        protected virtual void _(Events.FieldUpdated<PSPaymentSlip, PSPaymentSlip.customerID> e)
        {
            PSPaymentSlip row = (PSPaymentSlip)e.Row;
            if (row == null) return;
            if (row.CustomerID == null) row.CustomerLocationID = null;
            foreach (PSPaymentSlipDetails d in PaymentSlipDetails.Select())
            {
                PaymentSlipDetails.Cache.SetValueExt<PSPaymentSlipDetails.customerID>(d, row.CustomerID);
                PaymentSlipDetails.Cache.SetValueExt<PSPaymentSlipDetails.customerLocationID>(d, row.CustomerLocationID);
                SetPaymentSlipDetailsCacheStatus(d);
            }
        }

        protected virtual void _(Events.FieldUpdated<PSPaymentSlip, PSPaymentSlip.contractID> e)
        {
            PSPaymentSlip row = (PSPaymentSlip)e.Row;
            if (row == null || row.ContractID == null || row.TargetType != PSTargetType.Customer) return;
            Contract c = (Contract)PXSelectorAttribute.Select<PSPaymentSlip.contractID>(e.Cache, row);
            //2021-07-12 mark :12146 選定專案合約後, 預帶 專案內預設客戶 的連動拔掉
            //e.Cache.SetValueExt<PSPaymentSlip.customerID>(row, c?.CustomerID);
            if (row.ContractID != 0 && String.IsNullOrEmpty(row.DocDesc))
            {
                e.Cache.SetValueExt<PSPaymentSlip.docDesc>(row, c?.Description);
            }
            foreach (PSPaymentSlipDetails d in PaymentSlipDetails.Select())
            {
                PaymentSlipDetails.Cache.SetValueExt<PSPaymentSlipDetails.contractID>(d, row.ContractID);
                SetPaymentSlipDetailsCacheStatus(d);
            }
        }

        #region 20200925 mark by alton 繳款單不維護供應商，將來user要輸入供應商資料就得去客戶主檔新增供應商資料
        protected virtual void _(Events.FieldUpdated<PSPaymentSlip, PSPaymentSlip.vendorID> e)
        {
            PSPaymentSlip row = (PSPaymentSlip)e.Row;
            if (row == null) return;
            if (row.VendorID == null) row.VendorLocationID = null;
            foreach (PSPaymentSlipDetails d in PaymentSlipDetails.Select())
            {
                PaymentSlipDetails.Cache.SetValueExt<PSPaymentSlipDetails.vendorID>(d, row.VendorID);
                PaymentSlipDetails.Cache.SetValueExt<PSPaymentSlipDetails.vendorLocationID>(d, row.VendorLocationID);
                SetPaymentSlipDetailsCacheStatus(d);
            }
        }

        protected virtual void _(Events.FieldUpdated<PSPaymentSlip, PSPaymentSlip.targetType> e)
        {
            PSPaymentSlip row = (PSPaymentSlip)e.Row;
            if (row == null) return;
            e.Cache.SetValueExt<PSPaymentSlip.vendorID>(row, null);
            e.Cache.SetValueExt<PSPaymentSlip.customerID>(row, null);
            e.Cache.SetValueExt<PSPaymentSlip.employeeID>(row, null);
            foreach (PSPaymentSlipDetails d in PaymentSlipDetails.Select())
            {
                PaymentSlipDetails.Cache.SetValueExt<PSPaymentSlipDetails.employeeID>(d, null);
                SetPaymentSlipDetailsCacheStatus(d);
            }
            switch (row.TargetType)
            {
                case PSTargetType.Customer:
                    //自動透過contractID帶客戶
                    e.Cache.SetValueExt<PSPaymentSlip.contractID>(row, row.ContractID);
                    break;
                case PSTargetType.Vendor:
                    break;
                case PSTargetType.Employee:
                    break;
            }

        }
        #endregion

        #endregion
        #region PSPaymentSlipDetails
        protected virtual void _(Events.FieldUpdated<PSPaymentSlipDetails, PSPaymentSlipDetails.ccPostageAmt> e)
        {
            PSPaymentSlipDetails row = e.Row;
            if (row == null) return;
            setDocBal();
        }

        protected virtual void _(Events.FieldDefaulting<PSPaymentSlipDetails, PSPaymentSlipDetails.checkIssuer> e)
        {
            PSPaymentSlipDetails row = e.Row;
            if (row == null) return;
            PSPaymentSlip master = PaymentSlips.Current;
            if (master.DocType == PSDocType.ArGuarCheck)
            {
                if (master.VendorID != null)
                {
                    BAccountR v = (BAccountR)PXSelectorAttribute.Select<PSPaymentSlip.vendorID>(PaymentSlips.Cache, master);
                    e.NewValue = v?.AcctName;
                }
                else
                {
                    BAccountR c = (BAccountR)PXSelectorAttribute.Select<PSPaymentSlip.customerID>(PaymentSlips.Cache, master);
                    e.NewValue = c?.AcctName;

                }
            }
        }

        protected virtual void _(Events.FieldUpdated<PSPaymentSlipDetails, PSPaymentSlipDetails.paymentRefNbr> e)
        {
            PSPaymentSlipDetails row = (PSPaymentSlipDetails)e.Row;
            if (row == null) return;
            PaymentSlipDetails.Cache.SetDefaultExt<PSPaymentSlipDetails.paymentRefNbrV>(row);
        }

        protected virtual void _(Events.FieldUpdated<PSPaymentSlipDetails, PSPaymentSlipDetails.slipDate> e)
        {
            PSPaymentSlipDetails row = (PSPaymentSlipDetails)e.Row;
            if (row == null) return;
            //PaymentSlipDetails.Cache.SetDefaultExt<PSPaymentSlipDetails.actualDueDate>(row);
            PaymentSlipDetails.Cache.SetDefaultExt<PSPaymentSlipDetails.processDate>(row);
            PaymentSlipDetails.Cache.SetDefaultExt<PSPaymentSlipDetails.etdDepositDate>(row);
            PaymentSlipDetails.Cache.SetDefaultExt<PSPaymentSlipDetails.checkDueDate>(row);
        }

        protected virtual void _(Events.FieldUpdated<PSPaymentSlipDetails, PSPaymentSlipDetails.dueDate> e)
        {
            PSPaymentSlipDetails row = (PSPaymentSlipDetails)e.Row;
            if (row == null) return;
            PaymentSlipDetails.Cache.SetDefaultExt<PSPaymentSlipDetails.actualDueDate>(row);
        }

        protected virtual void _(Events.FieldDefaulting<PSPaymentSlipDetails, PSPaymentSlipDetails.actualDueDate> e)
        {
            PSPaymentSlipDetails row = (PSPaymentSlipDetails)e.Row;
            if (row == null) return;
            //2021-05-26 edit By Alton :12038預設改dueDate
            //e.NewValue = row.SlipDate;
            PSPaymentSlip master = PaymentSlips.Current;
            //2021-07-08 應收保證票不帶預設
            if (master?.DocType != PSDocType.ArGuarCheck)
                e.NewValue = row.DueDate;
        }

        protected virtual void _(Events.FieldDefaulting<PSPaymentSlipDetails, PSPaymentSlipDetails.processDate> e)
        {
            PSPaymentSlipDetails row = (PSPaymentSlipDetails)e.Row;
            if (row == null) return;
            e.NewValue = row.SlipDate;
        }

        protected virtual void _(Events.FieldDefaulting<PSPaymentSlipDetails, PSPaymentSlipDetails.etdDepositDate> e)
        {
            PSPaymentSlipDetails row = (PSPaymentSlipDetails)e.Row;
            if (row == null) return;
            e.NewValue = row.PaymentMethodID == CHECK ? row.SlipDate : null;
        }

        protected virtual void _(Events.FieldDefaulting<PSPaymentSlipDetails, PSPaymentSlipDetails.checkDueDate> e)
        {
            PSPaymentSlipDetails row = (PSPaymentSlipDetails)e.Row;
            if (row == null) return;
            e.NewValue = row.PaymentMethodID == CHECK ? row.SlipDate : null;
        }

        protected virtual void _(Events.FieldUpdated<PSPaymentSlipDetails, PSPaymentSlipDetails.paymentMethodID> e)
        {
            PSPaymentSlipDetails row = (PSPaymentSlipDetails)e.Row;
            if (row == null) return;
            e.Cache.SetValueExt<PSPaymentSlipDetails.slipDate>(row, row.SlipDate);
        }

        protected virtual void _(Events.FieldUpdated<PSPaymentSlipDetails, PSPaymentSlipDetails.pONbr> e)
        {
            PSPaymentSlipDetails row = (PSPaymentSlipDetails)e.Row;
            if (row == null) return;
            POOrder order = (POOrder)PXSelectorAttribute.Select<PSPaymentSlipDetails.pONbr>(e.Cache, row);
            e.Cache.SetValueExt<PSPaymentSlipDetails.pOOrderType>(row, order?.OrderType);
        }

        //protected virtual void _(Events.FieldUpdated<PSPaymentSlipDetails, PSPaymentSlipDetails.inventoryID> e)
        //{
        //    PSPaymentSlipDetails row = (PSPaymentSlipDetails)e.Row;
        //    if (row == null) return;
        //    InventoryItem item = (InventoryItem)PXSelectorAttribute.Select<PSPaymentSlipDetails.inventoryID>(e.Cache, row);
        //    PaymentSlipDetails.Cache.SetValueExt<PSPaymentSlipDetails.tranDesc>(row, item?.Descr);
        //    //row.TranDesc = item?.Descr;
        //}

        protected virtual void _(Events.FieldUpdated<PSPaymentSlipDetails, PSPaymentSlipDetails.qty> e)
        {
            PSPaymentSlipDetails row = (PSPaymentSlipDetails)e.Row;
            if (row == null) return;
            setAmount(row);
        }

        protected virtual void _(Events.FieldUpdated<PSPaymentSlipDetails, PSPaymentSlipDetails.unitCost> e)
        {
            PSPaymentSlipDetails row = (PSPaymentSlipDetails)e.Row;
            if (row == null) return;
            setAmount(row);
        }

        protected virtual void _(Events.FieldUpdated<PSPaymentSlipDetails, PSPaymentSlipDetails.tranAmt> e)
        {
            PSPaymentSlipDetails row = (PSPaymentSlipDetails)e.Row;
            if (row == null) return;
            setDocBal();
        }

        //protected virtual void _(Events.FieldUpdated<PSPaymentSlipDetails, PSPaymentSlipDetails.paymentCategoryV> e)
        //{
        //    PSPaymentSlipDetails row = (PSPaymentSlipDetails)e.Row;
        //    if (row == null) return;
        //    //避免觸發FieldUpdated
        //    row.PaymentCategory = row.PaymentCategoryV;
        //}

        //protected virtual void _(Events.FieldUpdated<PSPaymentSlipDetails, PSPaymentSlipDetails.paymentCategory> e)
        //{
        //    PSPaymentSlipDetails row = (PSPaymentSlipDetails)e.Row;
        //    if (row == null) return;
        //    e.Cache.SetDefaultExt<PSPaymentSlipDetails.paymentCategoryV>(row);
        //}

        protected virtual void _(Events.FieldDefaulting<PSPaymentSlipDetails, PSPaymentSlipDetails.tranDesc> e)
        {
            PSPaymentSlip item = PaymentSlips.Current;
            PSPaymentSlipDetails row = (PSPaymentSlipDetails)e.Row;
            if (row == null) return;
            e.NewValue = item.DocDesc;
        }

        protected virtual void _(Events.FieldUpdated<PSPaymentSlipDetails, PSPaymentSlipDetails.guarPayableCD> e)
        {
            PSPaymentSlipDetails row = (PSPaymentSlipDetails)e.Row;
            if (row == null) return;
            CCPayableCheck cc = (CCPayableCheck)PXSelectorAttribute.Select<PSPaymentSlipDetails.guarPayableCD>(e.Cache, row);
            e.Cache.SetValueExt<PSPaymentSlipDetails.paymentRefNbr>(row, cc.GuarNbr);
            e.Cache.SetValueExt<PSPaymentSlipDetails.pONbr>(row, cc.PONbr);
            e.Cache.SetValueExt<PSPaymentSlipDetails.pOOrderType>(row, cc.POOrderType);
            e.Cache.SetValueExt<PSPaymentSlipDetails.issueDate>(row, cc.IssueDate);
            e.Cache.SetValueExt<PSPaymentSlipDetails.authDate>(row, cc.AuthDate);
            e.Cache.SetValueExt<PSPaymentSlipDetails.guarType>(row, cc.GuarType);
            e.Cache.SetValueExt<PSPaymentSlipDetails.guarClass>(row, cc.GuarClass);
            e.Cache.SetValueExt<PSPaymentSlipDetails.pOOrderType>(row, cc.POOrderType);
            e.Cache.SetValueExt<PSPaymentSlipDetails.pOOrderType>(row, cc.POOrderType);
            e.Cache.SetValueExt<PSPaymentSlipDetails.dueDate>(row, cc.DueDate);
            e.Cache.SetValueExt<PSPaymentSlipDetails.unitCost>(row, cc.GuarAmt);
        }

        protected virtual void _(Events.RowPersisting<PSPaymentSlipDetails> e)
        {
            PSPaymentSlipDetails row = (PSPaymentSlipDetails)e.Row;
            if (row == null) return;
            VerifyingCheckNbr(row);
        }

        protected virtual void _(Events.RowDeleted<PSPaymentSlipDetails> e)
        {
            PSPaymentSlipDetails row = (PSPaymentSlipDetails)e.Row;
            if (row == null) return;
            setDocBal();
        }


        #endregion

        #endregion
        #region Method

        private void UpdateGuarNbrByCommercialPaper()
        {
            foreach (PSPaymentSlipDetails item in PaymentSlipDetails.Select())
            {
                if (item.GuarClass == GuarClassList.CommercialPaper)
                {
                    item.PaymentRefNbr = CCReceivableEntry.GetGuarNbrByCommercialPaper(PaymentSlipDetails.Cache, item, this.Accessinfo.BusinessDate);
                    PaymentSlipDetails.Update(item);
                }
            }
        }

        private bool SetError<Field>(PXCache cache, object row, object newValue, String errorMsg, PXErrorLevel errorLevel) where Field : PX.Data.IBqlField
        {
            cache.RaiseExceptionHandling<Field>(row, newValue,
                  new PXSetPropertyException(errorMsg, errorLevel));
            return false;
        }

        public virtual void UpdateCCPayableCheckStatus(CCPayableRtnStatus statusEnum)
        {
            String status;
            switch (statusEnum)
            {
                case CCPayableRtnStatus.Release:
                    status = CCPayableStatus.Released;
                    break;
                case CCPayableRtnStatus.AppliedRTN:
                    status = CCPayableStatus.AppliedRTN;
                    break;
                case CCPayableRtnStatus.Return:
                    status = CCPayableStatus.Returned;
                    break;
                default:
                    return;
            }

            foreach (PSPaymentSlipDetails item in PaymentSlipDetails.Select())
            {
                UpdateCCPayableCheckStatus(item.GuarPayableCD, status);
            }
        }

        public virtual void setDocBal()
        {
            PSPaymentSlip row = PaymentSlips.Current;
            decimal amt = 0m;
            foreach (PSPaymentSlipDetails item in PaymentSlipDetails.Select())
            {
                amt += item.TranAmt ?? 0m;
                amt -= item.CCPostageAmt ?? 0m;
            }
            PaymentSlips.Cache.SetValueExt<PSPaymentSlip.docBal>(row, amt);
        }

        /// <summary>
        /// 當setValue的時候，明細Status莫名不會變成updated，在此修正
        /// </summary>
        /// <param name="item"></param>
        public void SetPaymentSlipDetailsCacheStatus(PSPaymentSlipDetails item)
        {
            if (PaymentSlipDetails.Cache.GetStatus(item) == PXEntryStatus.Notchanged)
                PaymentSlipDetails.Cache.SetStatus(item, PXEntryStatus.Updated);
        }

        public String VerifyingCheckNbr(PSPaymentSlipDetails row)
        {
            String error = null;
            if (row.PaymentMethodID == CHECK)
            {
                if (row.PaymentRefNbr == null || row.PaymentRefNbr.Trim() == "")
                {
                    error = "支票號碼不得為空!";
                }
                else
                {
                    //20201103 - Phil~不檢查格式
                    //Regex rgx = new Regex(@"^[A-Z0-9]{7,12}$");
                    //if (!rgx.IsMatch(row.PaymentRefNbr?.ToUpper()))
                    //{
                    //    error = "支票號碼格式錯誤，長度不得超過12";
                    //}
                    //else
                    //{
                    //支票號碼重複檢核(開票銀行帳號+支票號碼)
                    //Stage1. 用支票號碼取得ARPayment(類型=收款、付款方式=CHECK、狀態!=作廢)
                    //Stage2. 沒有則檢查對應繳款單的開票銀行帳號是否一樣，一樣則跳錯誤
                    PXResultset<ARPayment> rs = GetArPaymentByCheck(row.PaymentMethodID, row.PaymentRefNbr?.ToUpper());
                    foreach (ARPayment item in rs)
                    {
                        PSPaymentSlipDetails refPS = GetArRefSlip(item.RefNbr);
                        if (row.OriBankAccount != null && row.OriBankAccount == refPS?.OriBankAccount
                            && row.PaymentSlipDetailID != refPS.PaymentSlipDetailID)
                        {
                            error = "支票號碼已被登記(" + item.RefNbr + ")";
                            break;
                        }
                    }
                    //}
                }
                if (error != null)
                {
                    PaymentSlipDetails.Cache.RaiseExceptionHandling<PSPaymentSlipDetails.paymentRefNbr>(
                            row, row.PaymentRefNbr, new PXSetPropertyException(error, PXErrorLevel.Error));
                    throw new PXRowPersistedException("PaymentRefNbr", row, error);
                }
            }
            return error;
        }

        public String CreateCCReceivable(PSPaymentSlipDetails item, PSPaymentSlip header, ref String guarNbr)
        {
            CCReceivableEntry entry = PXGraph.CreateInstance<CCReceivableEntry>();
            PXCache cache = entry.ReceivableChecks.Cache;
            CCReceivableCheck check = (CCReceivableCheck)cache.CreateInstance();
            check = entry.ReceivableChecks.Insert(check);
            //20220420 by louis use PSPaymentSlipDetails.ProcessDate as DocDate
            cache.SetValueExt<CCReceivableCheck.docDate>(check, item.ProcessDate);
            //cache.SetValueExt<CCReceivableCheck.status>(check, null);//預設
            cache.SetValueExt<CCReceivableCheck.issueDate>(check, item.IssueDate);
            cache.SetValueExt<CCReceivableCheck.authDate>(check, item.AuthDate);
            cache.SetValueExt<CCReceivableCheck.targetType>(check, header.TargetType);
            cache.SetValueExt<CCReceivableCheck.projectID>(check, item.ContractID);
            cache.SetValueExt<CCReceivableCheck.branchID>(check, item.BranchID);
            cache.SetValueExt<CCReceivableCheck.guarType>(check, item.GuarType);
            cache.SetValueExt<CCReceivableCheck.guarClass>(check, item.GuarClass);
            cache.SetValueExt<CCReceivableCheck.description>(check, item.TranDesc);
            cache.SetValueExt<CCReceivableCheck.guarNbr>(check, item.PaymentRefNbr);
            cache.SetValueExt<CCReceivableCheck.guarAmt>(check, item.TranAmt);
            cache.SetValueExt<CCReceivableCheck.ccPostageAmt>(check, item.CCPostageAmt);
            cache.SetValueExt<CCReceivableCheck.dueDate>(check, item.DueDate);
            cache.SetValueExt<CCReceivableCheck.contractorID>(check, GetEmployeeByUser(this.Accessinfo.UserID));
            cache.SetValueExt<CCReceivableCheck.cashierID>(check, GetEmployeeByUser(item.CreatedByID ?? this.Accessinfo.UserID));

            cache.SetValueExt<CCReceivableCheck.customerID>(check, item.CustomerID);
            cache.SetValueExt<CCReceivableCheck.customerLocationID>(check, item.CustomerLocationID);

            //莫名其妙會找不到VendorID
            //cache.SetValueExt<CCReceivableCheck.vendorID>(check, item.VendorID);
            //cache.SetValueExt<CCReceivableCheck.vendorLocationID>(check, item.VendorLocationID);
            check.VendorID = item.VendorID;
            check.VendorLocationID = item.VendorLocationID;

            cache.SetValueExt<CCReceivableCheck.pONbr>(check, item.PONbr);
            cache.SetValueExt<CCReceivableCheck.pOOrderType>(check, item.POOrderType);
            //2021-05-18 add by Alton
            cache.SetValueExt<CCReceivableCheck.checkIssuer>(check, item.CheckIssuer);
            cache.SetValueExt<CCReceivableCheck.oriBankCode>(check, item.OriBankCode);
            cache.SetValueExt<CCReceivableCheck.oriBankAccount>(check, item.OriBankAccount);

            entry.Persist();
            guarNbr = entry.ReceivableChecks.Current.GuarNbr;
            return entry.ReceivableChecks.Current.GuarReceviableCD;
        }

        public void CreateARPayment(PSPaymentSlip header)
        {
            /**
            #region group by  PaymentMethodID && BankAccountID
            Dictionary<String, List<PSPaymentSlipDetails>> group = new Dictionary<String, List<PSPaymentSlipDetails>>();
            Dictionary<String, decimal> groupAmt = new Dictionary<String, decimal>();
            foreach (PSPaymentSlipDetails item in PaymentSlipDetails.Select())
            {
                string key = item.PaymentMethodID + item.BankAccountID;
                if (group.ContainsKey(key))
                {
                    group[key].Add(item);
                    groupAmt[key] += item.TranAmt ?? 0m;
                }
                else
                {
                    List<PSPaymentSlipDetails> data = new List<PSPaymentSlipDetails>();
                    data.Add(item);
                    group.Add(key, data);
                    groupAmt.Add(key, item.TranAmt ?? 0m);
                }
            }
            #endregion

            //CreateARPayment
            foreach (string key in group.Keys)
            {
                bool isFirst = true;
                string arPaymentRefNbr = null;
                foreach (PSPaymentSlipDetails item in group[key])
                {
                    if (isFirst)
                    {
                        arPaymentRefNbr = CreateARPayment(item, header, groupAmt[key]);
                        isFirst = false;
                    }
                    item.ArPaymentRefNbr = arPaymentRefNbr;
                    PaymentSlipDetails.Update(item);
                }
            }**/
            foreach (PSPaymentSlipDetails item in PaymentSlipDetails.Select()) {
                string arPaymentRefNbr = null;
                arPaymentRefNbr = CreateARPayment(item, header, item.TranAmt??0);
                item.ArPaymentRefNbr = arPaymentRefNbr;
                PaymentSlipDetails.Update(item);
            }
        }


        public String CreateARPayment(PSPaymentSlipDetails item, PSPaymentSlip header, decimal tranAmt)
        {
            ARPaymentEntry entry = PXGraph.CreateInstance<ARPaymentEntry>();
            ARPayment arPayment = (ARPayment)entry.Document.Cache.CreateInstance();
            //arPayment.CuryID = item.CuryID;
            //arPayment.CuryInfoID = item.CuryInfoID;
            arPayment = entry.Document.Insert(arPayment);

            entry.Document.Cache.SetValueExt<ARPayment.hold>(arPayment, false);
            entry.Document.Cache.SetValueExt<ARPayment.docType>(arPayment, ARDocType.Payment);
            entry.Document.Cache.SetValueExt<ARPayment.branchID>(arPayment, header.BranchID);
            entry.Document.Cache.SetValueExt<ARPayment.projectID>(arPayment, header.ContractID);
            if (header.ContractID != 0)
            {
                PMTask task = NMVoucherUtil.getPMTaskisDefault(this, header.ContractID);
                if (task == null)
                {
                    PMProject project = (PMProject)PXSelectorAttribute.Select<PSPaymentSlip.contractID>(PaymentSlips.Cache, header);
                    throw new Exception(String.Format("請至專案''{0}''預設任務!", project.ContractCD));
                }
                entry.Document.Cache.SetValueExt<ARPayment.taskID>(arPayment, task.TaskID);
            }

            //modifyby louis 20220311 adjDatee 改寫入款單明細的入帳日(ProcessDate)
            //entry.Document.Cache.SetValueExt<ARPayment.adjDate>(arPayment, header.DocDate);
            entry.Document.Cache.SetValueExt<ARPayment.adjDate>(arPayment, item.ProcessDate);
            entry.Document.Cache.SetValueExt<ARPayment.customerID>(arPayment, header.CustomerID);
            entry.Document.Cache.SetValueExt<ARPayment.customerLocationID>(arPayment, header.CustomerLocationID);
            entry.Document.Cache.SetValueExt<ARPayment.paymentMethodID>(arPayment, item.PaymentMethodID);
            NMBankAccount ba = (NMBankAccount)PXSelectorAttribute.Select<PSPaymentSlipDetails.bankAccountID>(PaymentSlipDetails.Cache, item);
            entry.Document.Cache.SetValueExt<ARPayment.cashAccountID>(arPayment, ba.CashAccountID);
            entry.Document.Cache.SetValueExt<ARPayment.docDesc>(arPayment, header.DocDesc);
            //entry.Document.Cache.SetValueExt<ARPayment.extRefNbr>(arPayment, item.PaymentRefNbr);//支票號碼
            entry.Document.Cache.SetValueExt<ARPayment.curyOrigDocAmt>(arPayment, tranAmt);

            arPayment = entry.Document.Update(arPayment);
            entry.Save.Press();
            arPayment = entry.Document.Current;
            return arPayment.RefNbr;

        }

        private void setAmount(PSPaymentSlipDetails row)
        {
            PaymentSlipDetails.Cache.SetValueExt<PSPaymentSlipDetails.tranAmt>(PaymentSlipDetails.Cache.Current, (row.UnitCost ?? 0) * (row.Qty ?? 0));
        }

        private void setColumnUI(PSPaymentSlip row)
        {
            bool isInsert = PaymentSlips.Cache.GetStatus(row) == PXEntryStatus.Inserted;
            Delete.SetEnabled(false);
            bool isOpen = row.Status == PSStatus.Open;
            bool isHold = row.Status == PSStatus.Hold;
            bool isRV = row.Status == PSStatus.Released || row.Status == PSStatus.Voided;
            bool isApRtnGuarCheck = row.DocType == PSDocType.ApRtnGuarCheck;
            bool isPayment = row.DocType == PSDocType.Payment;
            bool isOther = row.DocType == PSDocType.Other;
            PaymentSlipDetails.AllowDelete = isHold;
            PaymentSlipDetails.AllowInsert = isHold;

            #region 依據DocType
            PXUIFieldAttribute.SetEnabled<PSPaymentSlip.employeeID>(PaymentSlips.Cache, row, isPayment || isOther);
            //2022-12-28 Alton 當為保證票時顯示
            PXUIFieldAttribute.SetVisible<PSPaymentSlip.targetType>(PaymentSlips.Cache, row, !isPayment && !isOther);
            #endregion

            #region 依據狀態唯讀設定
            Release.SetEnabled(!isRV && isOpen);
            Submit.SetEnabled(!isRV && isHold);
            VoidBtn.SetEnabled(!isRV && !isInsert);
            PutOnHold.SetEnabled(isOpen);
            PrintPaymentSlip.SetEnabled(!isInsert);
            PXUIFieldAttribute.SetReadOnly(PaymentSlips.Cache, row, !isHold);
            PXUIFieldAttribute.SetReadOnly<PSPaymentSlip.refNbr>(PaymentSlips.Cache, row, false);
            #region Tab1-繳款單明細資料
            PXUIFieldAttribute.SetEnabled<PSPaymentSlipDetails.slipDate>(PaymentSlipDetails.Cache, null, isHold);
            PXUIFieldAttribute.SetEnabled<PSPaymentSlipDetails.paymentRefNbr>(PaymentSlipDetails.Cache, null, isHold);
            PXUIFieldAttribute.SetEnabled<PSPaymentSlipDetails.paymentMethodID>(PaymentSlipDetails.Cache, null, isHold);
            PXUIFieldAttribute.SetEnabled<PSPaymentSlipDetails.inventoryID>(PaymentSlipDetails.Cache, null, isHold);
            PXUIFieldAttribute.SetEnabled<PSPaymentSlipDetails.tranDesc>(PaymentSlipDetails.Cache, null, isHold);
            PXUIFieldAttribute.SetEnabled<PSPaymentSlipDetails.qty>(PaymentSlipDetails.Cache, null, isHold && !isApRtnGuarCheck);
            PXUIFieldAttribute.SetEnabled<PSPaymentSlipDetails.unitCost>(PaymentSlipDetails.Cache, null, isHold && !isApRtnGuarCheck);
            PXUIFieldAttribute.SetEnabled<PSPaymentSlipDetails.tranAmt>(PaymentSlipDetails.Cache, null, isHold && !isApRtnGuarCheck);
            PXUIFieldAttribute.SetEnabled<PSPaymentSlipDetails.contractID>(PaymentSlipDetails.Cache, null, isHold);
            PXUIFieldAttribute.SetEnabled<PSPaymentSlipDetails.dueDate>(PaymentSlipDetails.Cache, null, isHold && !isApRtnGuarCheck);
            PXUIFieldAttribute.SetEnabled<PSPaymentSlipDetails.authDate>(PaymentSlipDetails.Cache, null, isHold && !isApRtnGuarCheck);
            PXUIFieldAttribute.SetEnabled<PSPaymentSlipDetails.issueDate>(PaymentSlipDetails.Cache, null, isHold && !isApRtnGuarCheck);
            PXUIFieldAttribute.SetEnabled<PSPaymentSlipDetails.guarClass>(PaymentSlipDetails.Cache, null, isHold && !isApRtnGuarCheck);
            PXUIFieldAttribute.SetEnabled<PSPaymentSlipDetails.guarType>(PaymentSlipDetails.Cache, null, isHold && !isApRtnGuarCheck);
            PXUIFieldAttribute.SetEnabled<PSPaymentSlipDetails.guarPayableCD>(PaymentSlipDetails.Cache, null, isHold);
            PXUIFieldAttribute.SetEnabled<PSPaymentSlipDetails.pONbr>(PaymentSlipDetails.Cache, null, isHold && !isApRtnGuarCheck);
            PXUIFieldAttribute.SetEnabled<PSPaymentSlipDetails.checkIssuer>(PaymentSlipDetails.Cache, null, isHold);
            PXUIFieldAttribute.SetEnabled<PSPaymentSlipDetails.oriBankCode>(PaymentSlipDetails.Cache, null, isHold);
            PXUIFieldAttribute.SetEnabled<PSPaymentSlipDetails.oriBankAccount>(PaymentSlipDetails.Cache, null, isHold);
            #endregion
            #region Tab2-資金收款資料
            PXUIFieldAttribute.SetEnabled<PSPaymentSlipDetails.paymentCategory>(PaymentSlipDetails.Cache, null, isOpen);
            PXUIFieldAttribute.SetEnabled<PSPaymentSlipDetails.bankAccountID>(PaymentSlipDetails.Cache, null, isOpen);
            //PXUIFieldAttribute.SetEnabled<PSPaymentSlipDetails.actualDueDate>(PaymentSlipDetails.Cache, null, isOpen);
            PXUIFieldAttribute.SetEnabled<PSPaymentSlipDetails.checkDueDate>(PaymentSlipDetails.Cache, null, isOpen);
            PXUIFieldAttribute.SetEnabled<PSPaymentSlipDetails.etdDepositDate>(PaymentSlipDetails.Cache, null, isOpen);
            PXUIFieldAttribute.SetEnabled<PSPaymentSlipDetails.processDate>(PaymentSlipDetails.Cache, null, isOpen);
            #endregion

            PXUIFieldAttribute.SetEnabled<PSPaymentSlipDetails.guarReceviableCD>(PaymentSlipDetails.Cache, null, false);
            PXUIFieldAttribute.SetEnabled<PSPaymentSlipDetails.arPaymentRefNbr>(PaymentSlipDetails.Cache, null, false);

            #endregion
        }
        #endregion
        #region BQL
        /// <summary>
        /// 更新應付保證票狀態
        /// </summary>
        /// <param name="guarCD"></param>
        /// <param name="status"></param>
        public void UpdateCCPayableCheckStatus(string guarCD, string status)
        {
            PXUpdate<
                Set<CCPayableCheck.status, Required<CCPayableCheck.status>>,
                CCPayableCheck,
                Where<CCPayableCheck.guarPayableCD, Equal<Required<CCPayableCheck.guarPayableCD>>>>
                .Update(this, status, guarCD);
        }

        public int? GetEmployeeByUser(Guid userID)
        {
            EPEmployee emp = PXSelect<EPEmployee, Where<EPEmployee.userID, Equal<Required<EPEmployee.userID>>>>
                .Select(this, userID);
            return emp?.BAccountID;
        }

        public PaymentMethodAccount getPaymentMethodAccount(string paymentMethodID)
        {
            return PXSelect<PaymentMethodAccount,
                Where<PaymentMethodAccount.paymentMethodID, Equal<Required<PaymentMethodAccount.paymentMethodID>>>>
                .Select(this, paymentMethodID);
        }

        public PXResultset<ARPayment> GetArPaymentByCheck(string paymentMethodID, string checkNbr)
        {
            return PXSelect<ARPayment,
                Where<ARPayment.paymentMethodID, Equal<Required<ARPayment.paymentMethodID>>,
                And<ARPayment.extRefNbr, Equal<Required<ARPayment.extRefNbr>>,
                And<ARPayment.docType, Equal<ARDocType.payment>,
                And<ARPayment.status, NotEqual<ARDocStatus.voided>>>>>>
                .Select(this, paymentMethodID, checkNbr);
        }

        public PSPaymentSlipDetails GetArRefSlip(string refNbr)
        {
            return PXSelect<PSPaymentSlipDetails,
                Where<PSPaymentSlipDetails.arPaymentRefNbr, Equal<Required<PSPaymentSlipDetails.arPaymentRefNbr>>>>
                .Select(this, refNbr);
        }


        public Customer getCustomer(int? customerID)
        {
            return PXSelect<Customer,
                Where<Customer.bAccountID, Equal<Required<Customer.bAccountID>>>>
            .Select(new PXGraph(), customerID);
        }

        public KGSetUp GetKGSetUp()
        {
            return PXSelect<KGSetUp>.SelectWindowed(this, 0, 1);
        }
        #endregion


        #region Enum
        public enum CCPayableRtnStatus
        {
            Release, AppliedRTN, Return
        }

        #endregion
    }
}