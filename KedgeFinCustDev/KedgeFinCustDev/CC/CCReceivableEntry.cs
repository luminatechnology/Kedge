using System;
using PX.Data;
using CC.DAC;
using CC.Util;
using RC.Util;
using PX.Objects.PO;
using System.Collections;
using PX.Objects.GL;
using PX.Objects.CS;
using PS.DAC;
using PX.Objects.EP;

namespace CC
{
    /***
     * ====2021-03-10:11973====Alton
     * add Transaction
     * 
     *  ====2021-03-10:11967====Alton
     *  1.add CCReceivableCheckPostponeLog 展延日期紀錄
     *  2.第一次展延：IsPostpone = True 且存檔後，將PostponeDate寫入紀錄，且將DueDate 與 IsPostpone設為Readyonly
     *  2.第N次展延：PostponeDate被異動，將PostponeDate寫入紀錄
     *  
     *  ===2021/03/31:119988 === Althea
     * Add Action : ModifyAction
     * 當狀態為PendingModify時可以使用
     * 按下按鈕更改此張票據狀態 = balanced
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
     *   ====2021-05-20=====Louis
     *   SetUI() --> 新增 status為 AppliedRTN時, 可以執行ReturnBtn
     *   
     * ====2021-07-14:12153====Alton
     * 工商本票取號提前至PS提交時執行
     * 
     * ====2021-10-05:12253====Alton
     * 1.請在CCReceivableCheck新增一欄位 CCPostageAmt,decimal(18,6),非必填,預設=0
     *  - 請放在「合約編號」欄位下方
     *  - 當此欄金額異動時，記得將 CCReceivableCheck.GuarAmt的金額減少
     * 2.CC應收保證票「核可」時，如CCReceivableCheck.CCPostageAmt >0,請在核可傳票中多產生一筆借方的分錄：
     *   科目＝KGPostageSetup.KGPostageAccountID
     *   子科目＝KGPostageSetup.KGPostageSubID
     * 
     * **/
    public class CCReceivableEntry : PXGraph<CCReceivableEntry, CCReceivableCheck>
    {
        public CCReceivableEntry()
        {
            if (!RCFeaturesSetUtil.IsActive(this, RCFeaturesSetProperties.CC_RECEIVABLE_CHECCK))
            {
                RCFeaturesSetUtil.BackToHomePage();
            }
            this.ActionMenu.MenuAutoOpen = true;
            this.ActionMenu.AddMenuAction(this.ReleaseBtn);
            this.ActionMenu.AddMenuAction(this.ReturnBtn);
            this.ActionMenu.AddMenuAction(this.AccountReceiveBtn);
        }

        #region View
        public PXSelect<CCReceivableCheck> ReceivableChecks;
        public PXSelect<CCReceivableCheckPostponeLog,
            Where<CCReceivableCheckPostponeLog.guarReceivableID, Equal<Current<CCReceivableCheck.guarReceviableID>>>> PostponeLogs;

        #endregion

        #region Button
        #region Menu
        public PXAction<CCReceivableCheck> ActionMenu;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Actions")]
        protected void actionMenu() { }
        #endregion

        #region 核可
        public PXAction<CCReceivableCheck> ReleaseBtn;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "核可")]
        protected IEnumerable releaseBtn(PXAdapter adapter)
        {
            using (PXTransactionScope ts = new PXTransactionScope())
            {
                Persist();
                CCReceivableCheck item = ReceivableChecks.Current;
                item.Status = CCList.CCReceivableStatus.Released;
                item.GuarReleaseDate = this.Accessinfo.BusinessDate;
                item.BatchNbr = CCVoucherUtil.CreateARVoucher(CCList.CCARVoucher.RELEASE, item, true);
                ReceivableChecks.Update(item);
                Persist();
                ts.Complete();
            }

            return adapter.Get();
        }
        #endregion

        #region 核退
        public PXAction<CCReceivableCheck> ReturnBtn;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "核退")]
        protected IEnumerable returnBtn(PXAdapter adapter)
        {
            using (PXTransactionScope ts = new PXTransactionScope())
            {
                this.Persist();
                CCReceivableCheck item = ReceivableChecks.Current;
               

                // mantis [0012303] - #2
                //item.GuarReturnDate = this.Accessinfo.BusinessDate;
                //item.ReturnBatchNbr = CCVoucherUtil.CreateARVoucher(CCList.CCARVoucher.REVERSE, item);
                JournalEntry entry = CreateInstance<JournalEntry>();
                var batch = entry.BatchModule.Current = Batch.PK.Find(this, BatchModule.GL, item.ReturnBatchNbr);
                //2023-01-17 Alton KED-28: 應收保證票→退回（傳票Balance ＆ Release一起作業）。
                batch.Hold = false;
                batch.Status = PX.Objects.GL.BatchStatus.Balanced;
                entry.BatchModule.UpdateCurrent();

                entry.release.Press();

                //2023-01-17 Alton KED-28: 做完傳票Release後才改狀態
                item.Status = CCList.CCReceivableStatus.Returned;

                ReceivableChecks.Update(item);
                this.Persist();
                ts.Complete();
            }
            return adapter.Get();
        }
        #endregion

        #region 核退已領取
        public PXAction<CCReceivableCheck> AccountReceiveBtn;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "核退已領取")]
        protected IEnumerable accountReceiveBtn(PXAdapter adapter)
        {
            using (PXTransactionScope ts = new PXTransactionScope())
            {
                this.Persist();
                CCReceivableCheck item = ReceivableChecks.Current;
                item.Status = CCList.CCReceivableStatus.AccountReceived;
                item.GuarReceiveDate = this.Accessinfo.BusinessDate;
                ReceivableChecks.Update(item);
                this.Persist();
                ts.Complete();
            }
            return adapter.Get();
        }
        #endregion

        //2021/03/31 Add Mantis : 0011988 by Althea
        #region 完成修改
        public PXAction<CCReceivableCheck> ModifiedAction;
        [PXButton(CommitChanges = true, Tooltip = "")]
        [PXUIField(DisplayName = "完成修改")]
        protected IEnumerable modifiedAction(PXAdapter adapter)
        {
            CCReceivableCheck check = ReceivableChecks.Current;
            PXLongOperation.StartOperation(this, ModifiedMethod);
            return adapter.Get();
        }
        #endregion

        #endregion

        #region Hyper Link
        #region GL Voucher
        public PXAction<CCReceivableCheck> ViewGLVoucherByRelease;
        public PXAction<CCReceivableCheck> ViewGLVoucherByReturn;

        [PXButton(CommitChanges = true)]
        [PXUIField(Visible = false)]
        protected virtual void viewGLVoucherByRelease()
        {
            CCReceivableCheck check = ReceivableChecks.Current;
            if (check.BatchNbr == null) return;
            LinkVoucher(check.BatchNbr);
        }

        [PXButton(CommitChanges = true)]
        [PXUIField(Visible = false)]
        protected virtual void viewGLVoucherByReturn()
        {
            CCReceivableCheck check = ReceivableChecks.Current;
            if (check.ReturnBatchNbr == null) return;
            LinkVoucher(check.ReturnBatchNbr);
        }

        #endregion
        #endregion

        #region Event
        public override void Persist()
        {
            using (PXTransactionScope ts = new PXTransactionScope())
            {
                //紀錄延展日
                postponeLog(ReceivableChecks.Current);

                base.Persist();
                ts.Complete();
            }
            setUI(ReceivableChecks.Current);
        }

        protected virtual void _(Events.RowSelected<CCReceivableCheck> e)
        {
            CCReceivableCheck row = e.Row as CCReceivableCheck;
            if (row == null) return;
            setUI(row);
        }

        protected virtual void _(Events.RowPersisting<CCReceivableCheck> e)
        {
            CCReceivableCheck row = e.Row as CCReceivableCheck;
            if (row == null) return;
            CCReceivableCheck oriRow = (CCReceivableCheck)ReceivableChecks.Cache.GetOriginal(row);
            //2021-07-14 新增時不取號，由PS取號
            if ((oriRow != null && oriRow.GuarClass != row.GuarClass) && row.GuarClass == CCList.GuarClassList.CommercialPaper)
            {
                row.GuarNbr =
                    GetGuarNbrByCommercialPaper(ReceivableChecks.Cache, row, this.Accessinfo.BusinessDate);
            }
        }

        protected virtual void _(Events.FieldUpdated<CCReceivableCheck, CCReceivableCheck.targetType> e)
        {
            CCReceivableCheck row = e.Row as CCReceivableCheck;
            if (row == null) return;
            e.Cache.SetValueExt<CCReceivableCheck.customerID>(row, null);
            e.Cache.SetValueExt<CCReceivableCheck.vendorID>(row, null);
        }

        protected virtual void _(Events.FieldUpdated<CCReceivableCheck, CCReceivableCheck.customerID> e)
        {
            CCReceivableCheck row = e.Row as CCReceivableCheck;
            if (row == null) return;
            if (row.CustomerID == null) row.CustomerLocationID = null;
        }

        protected virtual void _(Events.FieldUpdated<CCReceivableCheck, CCReceivableCheck.vendorID> e)
        {
            CCReceivableCheck row = e.Row as CCReceivableCheck;
            if (row == null) return;
            if (row.VendorID == null) row.VendorLocationID = null;
        }

        //2020/10/30 Althea Add Mantis:0011759
        protected virtual void _(Events.FieldUpdated<CCReceivableCheck, CCReceivableCheck.pONbr> e)
        {
            CCReceivableCheck row = e.Row as CCReceivableCheck;
            if (row == null) return;
            if (row.PONbr != null)
            {
                POOrder order = GetPOOrder(row.PONbr);
                row.POOrderType = order.OrderType;
                //2021-01-19 Alton mark Mantis:11895
                //row.Description = order.OrderDesc;
            }
        }

        protected virtual void _(Events.FieldUpdated<CCReceivableCheck, CCReceivableCheck.isPostpone> e)
        {
            CCReceivableCheck row = e.Row as CCReceivableCheck;
            if (row == null) return;
            ReceivableChecks.Cache.SetDefaultExt<CCReceivableCheck.postDueDate>(row);
        }

        protected virtual void _(Events.FieldDefaulting<CCReceivableCheck, CCReceivableCheck.postDueDate> e)
        {
            CCReceivableCheck row = e.Row as CCReceivableCheck;
            if (row == null) return;
            e.NewValue = row.IsPostpone == true ? row.DueDate : null;
        }

        protected virtual void _(Events.FieldUpdated<CCReceivableCheck, CCReceivableCheck.ccPostageAmt> e)
        {
            CCReceivableCheck row = e.Row as CCReceivableCheck;
            if (row == null) return;
            decimal _old = (decimal?)e.OldValue ?? 0m;
            decimal _new = (decimal?)e.NewValue ?? 0m;
            decimal val = _new - _old;
            e.Cache.SetValueExt<CCReceivableCheck.guarAmt>(row, (row.GuarAmt??0m) - val);
        }
        #endregion

        #region Methods
        /// <summary>
        /// 取得工商本票號碼
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="data"></param>
        /// <param name="sysDate"></param>
        /// <returns></returns>
        public static string GetGuarNbrByCommercialPaper(PXCache cache, object data, DateTime? sysDate)
        {
            return AutoNumberAttribute.GetNextNumber(cache, data, "COMPAPER", sysDate);
        }

        /// <summary>
        /// According to Mantis [0012303] and Louis's suggestion to create a shard method.
        /// </summary>
        /// <param name="guarRecCD"></param>
        public static void CreateAppliedRTNVoucher(string guarRecCD, EPExpenseClaimDetails epDetail)
        {
            using (PXTransactionScope ts = new PXTransactionScope())
            {
                CCReceivableEntry graph = CreateInstance<CCReceivableEntry>();

                CCReceivableCheck row = CCReceivableCheck.UK.Find(graph, guarRecCD);
                EPExpenseClaimDetailsExt detailsExt = PXCache<EPExpenseClaimDetails>.GetExtension<EPExpenseClaimDetailsExt>(epDetail);
                //row.GuarReturnDate = graph.Accessinfo.BusinessDate;
                row.GuarReturnDate = detailsExt.UsrReturnDateCCR;
                row.ReturnBatchNbr = CCVoucherUtil.CreateARVoucher(CCList.CCARVoucher.REVERSE, row);
                row.Status = CCList.CCReceivableStatus.AppliedRTN;
                graph.ReceivableChecks.Cache.Update(row);
                // Mantis [0012302] - Comment [0025655] #3. Avoid the issue of "Another process has updated....".
                graph.SelectTimeStamp();
                graph.Save.Press();

                ts.Complete();
            }
        }

        private void postponeLog(CCReceivableCheck row)
        {
            if (row.IsPostpone != true) return;
            CCReceivableCheckPostponeLog lastLog = GetLastPostponeLog(row.GuarReceviableID);
            if (row.PostDueDate != lastLog?.PostponeDate)
            {
                CCReceivableCheckPostponeLog newLog = (CCReceivableCheckPostponeLog)PostponeLogs.Cache.CreateInstance();
                newLog.PostponeSeq = (lastLog?.PostponeSeq ?? 0) + 1;
                newLog.PostponeDate = row.PostDueDate;
                newLog = PostponeLogs.Insert(newLog);
                newLog = PostponeLogs.Update(newLog);
            }

        }

        private void setUI(CCReceivableCheck row)
        {
            Insert.SetEnabled(false);
            Delete.SetEnabled(false);
            bool isReturned = row.Status == CCList.CCReceivableStatus.Returned || row.Status == CCList.CCReceivableStatus.AccountReceived;
            bool isAccountReturned = row.Status == CCList.CCReceivableStatus.AccountReceived;
            bool isRelease = row.Status == CCList.CCReceivableStatus.Released;
            bool isAppliedRTN = row.Status == CCList.CCReceivableStatus.AppliedRTN;
            bool hasLog = GetLastPostponeLog(row.GuarReceviableID) != null;
            PXUIFieldAttribute.SetReadOnly(ReceivableChecks.Cache, row, true);
            PXUIFieldAttribute.SetReadOnly<CCReceivableCheck.isPostpone>(ReceivableChecks.Cache, row, isReturned || isAppliedRTN || hasLog);
            PXUIFieldAttribute.SetReadOnly<CCReceivableCheck.postDueDate>(ReceivableChecks.Cache, row, isReturned || isAppliedRTN);
            PXUIFieldAttribute.SetVisible<CCReceivableCheck.postDueDate>(ReceivableChecks.Cache, row, row.IsPostpone == true);

            PXUIFieldAttribute.SetReadOnly<CCReceivableCheck.guarReceviableCD>(ReceivableChecks.Cache, row, false);
            //PXUIFieldAttribute.SetReadOnly<CCReceivableCheck.issueDate>(ReceivableChecks.Cache, row, false);
            //PXUIFieldAttribute.SetReadOnly<CCReceivableCheck.authDate>(ReceivableChecks.Cache, row, false);
            PXUIFieldAttribute.SetReadOnly<CCReceivableCheck.dueDate>(ReceivableChecks.Cache, row, isAccountReturned || isAppliedRTN || hasLog);
            PXUIFieldAttribute.SetReadOnly<CCReceivableCheck.description>(ReceivableChecks.Cache, row, isAccountReturned || isAppliedRTN);
            PXUIFieldAttribute.SetReadOnly<CCReceivableCheck.ccPostageAmt>(ReceivableChecks.Cache, row, isReturned || isAppliedRTN || isRelease);

            #region CCReceivableCheckPostponeLog
            PXUIFieldAttribute.SetReadOnly(PostponeLogs.Cache, null, true);
            #endregion

            ReleaseBtn.SetEnabled(row.Status == CCList.CCReceivableStatus.Balanced);
            //add by louis 新增 status為 AppliedRTN時, 可以執行ReturnBtn
            //2023-01-17 Alton KED-28:僅限AppliedRTN可執行，否則會找不到對應傳票
            ReturnBtn.SetEnabled(row.Status == CCList.CCReceivableStatus.AppliedRTN);
            AccountReceiveBtn.SetEnabled(row.Status == CCList.CCReceivableStatus.Returned);
            ModifiedAction.SetEnabled(row.Status == CCList.CCReceivableStatus.PendingModify);
        }

        private void LinkVoucher(String batchNbr)
        {
            JournalEntry graph = PXGraph.CreateInstance<JournalEntry>();
            graph.BatchModule.Current = graph.BatchModule.Search<Batch.batchNbr>(batchNbr);
            if (graph.BatchModule.Current == null) return;
            throw new PXRedirectRequiredException(graph, "GL Voucher")
            {
                Mode = PXBaseRedirectException.WindowMode.NewWindow
            };
        }

        private void ModifiedMethod()
        {
            CCReceivableCheck check = ReceivableChecks.Current;
            check.Status = CCList.CCReceivableStatus.Balanced;
            ReceivableChecks.Update(check);
            base.Persist();
        }
        #endregion

        #region BQL
        public CCReceivableCheckPostponeLog GetLastPostponeLog(int? guarReceivableID)
        {
            return PXSelect<CCReceivableCheckPostponeLog,
                Where<CCReceivableCheckPostponeLog.guarReceivableID, Equal<Required<CCReceivableCheckPostponeLog.guarReceivableID>>>,
                OrderBy<Desc<CCReceivableCheckPostponeLog.postponeSeq>>>
                .Select(this, guarReceivableID);
        }
        public POOrder GetPOOrder(string POOrderNbr)
        {
            return PXSelect<POOrder,
                Where<POOrder.orderNbr, Equal<Required<CCReceivableCheck.pONbr>>>>
                .Select(this, POOrderNbr);
        }
        #endregion
    }
}