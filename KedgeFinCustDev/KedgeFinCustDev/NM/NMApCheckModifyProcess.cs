using System;
using PX.Data;
using NM.DAC;
using PX.Objects.CR;
using PX.Objects.EP;
using System.Collections;
using PX.Objects.AR;
using PX.Objects.PM;
using PX.Data.ReferentialIntegrity.Attributes;
using RC.Util;
using NM.Util;
using PX.Objects.PO;
using PX.Objects.AP;
using Kedge.DAC;
using PX.Objects.GL;
using PX.Objects.CS;
using KG.Util;

namespace NM
{
    /**
     * ===2021/03/31 Mantis : 0011988 === Althea
     * details只顯示狀態為PendingRefund
     * 
     * ===2021/04/27 Mantis: 0012024 === Althea
     * Add Filter and Shows on Detail (KGBillPayment.UsrTrComfirmBy/ UsrTrComfirmDate/ UsrTrConfirmID/ UsrTrPaymentType )
     * 
     * ===2021-04-29 : 12024====Alton
     * 修正KGBillpayment資料遺失問題
     * 
     * ===2021/07/09 :0012134 & 0012135 === Althea
     * 撤退票時檢查對應的batchNbr是否有值,若有 產生一張反向的GL,記錄BatchNbr在對應的反向欄位
     * 
     * ==2021/07/16 :Tiffany === Althea
     * ADD PPM 的 Reverse
     * 
     * ===2021/08/24 :0012209 === Althea
     * Add if(ppm appayment voided) create GL For Reverse 
     * 
     * ===2021/08/24 :0012208 === Althea
     * Update KGBillPayment : Add UsrIsTrConfirm = false
    **/
    public class NMApCheckModifyProcess : PXGraph<NMApCheckModifyProcess>
    {
        //權限設定開關
        public NMApCheckModifyProcess()
        {
            if (!RCFeaturesSetUtil.IsActive(this, RCFeaturesSetProperties.NOTES_PAYABLE))
            {
                RCFeaturesSetUtil.BackToHomePage();
            }

        }

        #region ToolBar Action
        public PXCancel<NMApCheckModifyFilter> Cancel;

        #region 撤票
        public PXAction<NMApCheckModifyFilter> WithdrawProcessAction;
        [PXButton(CommitChanges = true, Tooltip = "撤票可勾選多筆支票")]
        [PXUIField(DisplayName = "撤/退票")]
        protected IEnumerable withdrawProcessAction(PXAdapter adapter)
        {
            NMPayableCheck payableCheck = CheckDetails.Current;
            NMApCheckModifyFilter filter = CheckFilters.Current;
            if (payableCheck != null)
            {
                checkRequired(filter);

                //異動應付票據, 異動日期yyyy/mm/dd, 異動筆數 XX筆, 異動金額 xxxxx元, 請確認!
                string dialogMsg = "異動應付票據\r\n";
                dialogMsg += "異動日期:{0}\r\n";
                dialogMsg += "異動筆數:{1}筆\r\n";
                dialogMsg += "異動現金額:{2:#,0.##}元";
                dialogMsg = String.Format(
                    dialogMsg,
                    filter.ModifyDate?.ToString("yyyy/MM/dd"),
                    filter.SelectedCount,
                    filter.SelectedOriCuryAmount);
                if (CheckFilters.Ask("確認異動資訊", dialogMsg, MessageButtons.YesNo) == WebDialogResult.Yes)
                {
                    ModifyMethod(NMStringList.NMAPCheckStatus.INVALID);
                }
                    
            }
            return adapter.Get();
        }
        #endregion

        /*#region 退票
        public PXAction<NMApCheckModifyFilter> ReturnProcessAction;
        [PXButton(CommitChanges = true, Tooltip = "退票可勾選多筆支票")]
        [PXUIField(DisplayName = "退票")]
        protected IEnumerable returnProcessAction(PXAdapter adapter)
        {
            NMPayableCheck payableCheck = CheckDetails.Current;
            NMApCheckModifyFilter filter = CheckFilters.Current;
            if (payableCheck != null)
            {
                checkRequired(filter);
                ModifyMethod(NMStringList.NMAPCheckStatus.CANCEL);
            }
            return adapter.Get();
        }
        #endregion*/

        #endregion

        #region  Select and Filter
        public PXFilter<NMApCheckModifyFilter> CheckFilters;

        //2021/03/31 Modify
        //更改為只顯示待撤退
        //2021/04/27 Modify
        //LeftJoin KGBillPayment for Show 4 Fields: UsrTrComfirmBy/ UsrTrComfirmDate/ UsrTrConfirmID/ UsrTrPaymentType 
        public PXSelectJoin<NMPayableCheck,
            LeftJoin<KGBillPayment,On<KGBillPayment.billPaymentID,Equal<NMPayableCheck.billPaymentID>>>,
            Where<NMPayableCheck.status, Equal<NMStringList.NMAPCheckStatus.pendingrefund>>> CheckDetails;
        /*
        //只有已託收和已收票可以顯示在畫面
        public PXSelect<NMPayableCheck,
            Where<NMPayableCheck.status, Equal<NMStringList.NMAPCheckStatus.unconfirm>,
                Or<NMPayableCheck.status, Equal<NMStringList.NMAPCheckStatus.confirm>,
                    Or<NMPayableCheck.status,Equal<NMStringList.NMAPCheckStatus.cash>>>>> CheckDetails;
        */
        protected virtual IEnumerable checkDetails()
        {
            NMApCheckModifyFilter filter = CheckFilters.Current;
            PXSelectJoin<NMPayableCheck,
            LeftJoin<KGBillPayment, On<KGBillPayment.billPaymentID, Equal<NMPayableCheck.billPaymentID>>>,
            Where<NMPayableCheck.status, Equal<NMStringList.NMAPCheckStatus.pendingrefund>>> query =
            new PXSelectJoin<NMPayableCheck,
            LeftJoin<KGBillPayment, On<KGBillPayment.billPaymentID, Equal<NMPayableCheck.billPaymentID>>>,
            Where<NMPayableCheck.status, Equal<NMStringList.NMAPCheckStatus.pendingrefund>>>(this);

            //query

            if (filter.PayableCashierID != null)
                query.WhereAnd<Where<NMPayableCheck.payableCashierID,
                    Equal<Current<NMApCheckModifyFilter.payableCashierID>>>>();
            if (filter.CashCashierID != null)
                query.WhereAnd<Where<NMPayableCheck.cashCashierID,
                    Equal<Current<NMApCheckModifyFilter.cashCashierID>>>>();
            if (filter.SendCashierID != null)
                query.WhereAnd<Where<NMPayableCheck.sendCashierID,
                    Equal<Current<NMApCheckModifyFilter.sendCashierID>>>>();
            if (filter.BankAccountID != null)
                query.WhereAnd<Where<NMPayableCheck.bankAccountID,
                    Equal<Current<NMApCheckModifyFilter.bankAccountID>>>>();
            if (filter.CheckNbrFrom != null)
                query.WhereAnd<Where<NMPayableCheck.checkNbr,
                    GreaterEqual<Current<NMApCheckModifyFilter.checkNbrFrom>>>>();
            if (filter.CheckNbrTo != null)
                query.WhereAnd<Where<NMPayableCheck.checkNbr,
                    LessEqual<Current<NMApCheckModifyFilter.checkNbrTo>>>>();
            if (filter.CheckDateFrom != null)
                query.WhereAnd<Where<NMPayableCheck.checkDate,
                    GreaterEqual<Current<NMApCheckModifyFilter.checkDateFrom>>>>();
            if (filter.CheckDateTo != null)
                query.WhereAnd<Where<NMPayableCheck.checkDate,
                    LessEqual<Current<NMApCheckModifyFilter.checkDateTo>>>>();
            if (filter.DueDateFrom != null)
                query.WhereAnd<Where<NMPayableCheck.dueDate,
                    GreaterEqual<Current<NMApCheckModifyFilter.dueDateFrom>>>>();
            if (filter.DueDateTo != null)
                query.WhereAnd<Where<NMPayableCheck.dueDate,
                    LessEqual<Current<NMApCheckModifyFilter.dueDateTo>>>>();
            if (filter.DepositDateFrom != null)
                query.WhereAnd<Where<NMPayableCheck.depositDate,
                    GreaterEqual<Current<NMApCheckModifyFilter.depositDateFrom>>>>();
            if (filter.DepositDateTo != null)
                query.WhereAnd<Where<NMPayableCheck.depositDate,
                    LessEqual<Current<NMApCheckModifyFilter.depositDateTo>>>>();
            if (filter.VendorID != null)
                query.WhereAnd<Where<NMPayableCheck.vendorID,
                    Equal<Current<NMApCheckModifyFilter.vendorID>>>>();
            if (filter.VendorLocationID != null)
                query.WhereAnd<Where<NMPayableCheck.vendorLocationID,
                    Equal<Current<NMApCheckModifyFilter.vendorLocationID>>>>();
            if (filter.ProjectID != null)
                query.WhereAnd<Where<NMPayableCheck.projectID,
                    Equal<Current<NMApCheckModifyFilter.projectID>>>>();
            if (filter.ProjectPeriod != null)
                query.WhereAnd<Where<NMPayableCheck.projectPeriod,
                    Equal<Current<NMApCheckModifyFilter.projectPeriod>>>>();
            //2021/04/27 Add 
            if (filter.TrConfirmBy != null)
                query.WhereAnd<Where<KGBillPaymentExt.usrTrConfirmBy,
                    Equal<Current<NMApCheckModifyFilter.trConfirmBy>>>>();
            if (filter.TrConfirmDate != null)
                query.WhereAnd<Where<KGBillPaymentExt.usrTrConfirmDate,
                    Equal<Current<NMApCheckModifyFilter.trConfirmDate>>>>();
            if (filter.TrConfirmID != null)
                query.WhereAnd<Where<KGBillPaymentExt.usrTrConfirmID,
                    Equal<Current<NMApCheckModifyFilter.trConfirmID>>>>();
            if (filter.TrPaymentType != null)
                query.WhereAnd<Where<KGBillPaymentExt.usrTrPaymentType,
                    Equal<Current<NMApCheckModifyFilter.trPaymentType>>>>();

            foreach (PXResult<NMPayableCheck, KGBillPayment> item in query.Select())
            {
                //NMPayableCheck check = (NMPayableCheck)item;
                //if (check.RefNbr != null)
                //{
                //    APInvoice invoice = GetInvoice(check.RefNbr);
                //    if (invoice != null)
                //        check.APInvBatchNbr = invoice.BatchNbr??null;
                //}
                yield return item;
            }

            //return query.Select();
        }

        #endregion

        #region Event
        protected virtual void NMPayableCheck_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            NMPayableCheck row = (NMPayableCheck)e.Row;
            if (row == null) return;
            setReadOnly();
        }

        protected virtual void NMPayableCheck_Selected_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            NMPayableCheck row = (NMPayableCheck)e.Row;
            NMApCheckModifyFilter header = CheckFilters.Current;
            if (row == null) return;

            decimal? selectbaseAmount = 0;
            decimal? selectoriAmount = 0;
            int? selectcount = 0;

            PXResultset<NMPayableCheck> set = 
                PXSelectJoin < NMPayableCheck,
                    LeftJoin< KGBillPayment, 
                        On < KGBillPayment.billPaymentID, Equal<NMPayableCheck.billPaymentID>>>,
                    Where <NMPayableCheck.selected, Equal<True>, 
                        And<NMPayableCheck.status, Equal<NMStringList.NMAPCheckStatus.pendingrefund>>>>
                .Select(this);
            foreach (NMPayableCheck payableCheck in set)
            {
                selectoriAmount = payableCheck.OriCuryAmount + selectoriAmount;
                selectbaseAmount = payableCheck.OriCuryAmount + selectbaseAmount;
            }
            selectcount = set.Count;

            header.SelectedOriCuryAmount = selectoriAmount;
            header.SelectedBaseCuryAmount = selectbaseAmount;
            header.SelectedCount = selectcount;



        }
        #endregion

        #region Method
        public void setReadOnly()
        {
            NMPayableCheck row = CheckDetails.Current;
            PXUIFieldAttribute.SetReadOnly(CheckDetails.Cache, row, true);
            PXUIFieldAttribute.SetReadOnly<NMPayableCheck.selected>(CheckDetails.Cache, row, false);
            CheckDetails.AllowDelete = false;
            CheckDetails.AllowInsert = false;
        }

        public void checkRequired(NMApCheckModifyFilter filter)
        {
            if (filter.ModifyCashierID == null || filter.ModifyDate == null ||
                    filter.ModifyReason == null)
            {
                string errormsg = "請填妥以下資料:";
                if (filter.ModifyCashierID == null)
                {
                    errormsg = errormsg + "異動作業人/";
                }
                if (filter.ModifyDate == null)
                {
                    errormsg = errormsg + "異動日期/";
                }
                if (filter.ModifyReason == null)
                {
                    errormsg = errormsg + "異動原因";
                }
                throw new Exception(errormsg);
            }
        }

        public void ModifyMethod(int ModifyStatus)
        {
            NMApCheckModifyFilter filter = CheckFilters.Current;
            PXLongOperation.StartOperation(this, delegate ()
            {
                using (PXTransactionScope ts = new PXTransactionScope())
                {
                    foreach (NMPayableCheck check in CheckDetails.Select())
                    {
                        if (check.Selected == true)
                        {
                            //以下資訊更新欄位
                            check.ModifyCashierID = filter.ModifyCashierID;
                            check.ModifyDate = filter.ModifyDate;
                            check.ModifyReason = filter.ModifyReason;
                            CheckDetails.Update(check);

                            #region 2021/07/08 Remove 
                            ///<Summary>Remove:
                            ///不用status做判斷產生反向GL 
                            ///也刪除ModifyType 因為退票action被拿掉了
                            ///</Summary>

                            /*
                            switch (check.Status)
                            {
                                case NMStringList.NMAPCheckStatus.CONFIRM:
                                    check.ConfirmReverseBatchNbr = NMVoucherUtil.CreateAPVoucher(NMStringList.NMAPVoucher.CONFIRM, check);
                                    CheckDetails.Update(check);
                                    break;

                                case NMStringList.NMAPCheckStatus.CASH:
                                    check.CashReverseBatchNbr = NMVoucherUtil.CreateAPVoucher(NMStringList.NMAPVoucher.CASHREVERSE, check);
                                    check.ConfirmReverseBatchNbr = NMVoucherUtil.CreateAPVoucher(NMStringList.NMAPVoucher.CONFIRMREVERSE, check);
                                    CheckDetails.Update(check);
                                    break;
                            }
                           

                            switch (ModifyStatus)
                            {
                                case NMStringList.NMAPCheckStatus.INVALID:
                                    //狀態改為已撤票
                                    check.Status = NMStringList.NMAPCheckStatus.INVALID;
                                    CheckDetails.Update(check);
                                    break;

                                //退票Action被拿掉了
                                case NMStringList.NMAPCheckStatus.CANCEL:
                                    //狀態改為已退票
                                    check.Status = NMStringList.NMAPCheckStatus.CANCEL;
                                    CheckDetails.Update(check);
                                    break;
                            }
                             */
                            #endregion

                            //2021/07/09 Add Mantis: 0012134 0012135
                            if (check.ConfirmBatchNbr != null || check.SourceCode == NMStringList.NMAPSourceCode.HISTORYCHECK)
                                //2021/09/23 Add Mantis: 0012237 
                                //20220418 by louis NMVoucherUtil.CreateAPVoucher()新增一個參數glStageCode紀錄傳票產生的時機

                                check.ConfirmReverseBatchNbr = NMVoucherUtil.CreateAPVoucher(NMStringList.NMAPVoucher.CONFIRMREVERSE, check, GLStageCode.NMPVoidPA);
                            if (check.CashBatchNbr != null)
                                check.CashReverseBatchNbr = NMVoucherUtil.CreateAPVoucher(NMStringList.NMAPVoucher.CASHREVERSE, check, GLStageCode.NMPVoidPE);
                            if (check.ConfirmAPPaymentBatchNbr != null)
                                check.APPaymentReverseBatchNbr = NMVoucherUtil.CreateAPVoucher(NMStringList.NMAPVoucher.APPAYMENTREVERSE, check, GLStageCode.NMPVoidPC);
                            if (check.ConfirmWriteoffPostageBatchNbr != null)
                                check.WriteoffPostageReverseBatchNbr = NMVoucherUtil.CreateAPVoucher(NMStringList.NMAPVoucher.WRITEOFFPOSTAGEREVERSE, check, GLStageCode.NMPVoidPD);
                            //2021/07/16 PPM 另外處裡
                            if (check.ConfirmPPMBatchNbr != null)
                                check.PPMReverseBatchNbr = NMVoucherUtil.CreateAPVoucher(NMStringList.NMAPVoucher.PPMREVERSE, check, GLStageCode.NMPVoidPC);

                            check.Status = NMStringList.NMAPCheckStatus.INVALID;
                            CheckDetails.Update(check);
                            base.Persist();
                            //2021/01/05 Mantis:0011833
                            VoidAPPaymant(check.PaymentDocNo);
                            //2021/08/24 Add :0012209 若為ppm作廢時要產生反向GL
                            //20220418 by louis NMVoucherUtil.CreateGLFromKGVoucher()新增一個參數glStageCode紀錄傳票產生的時機
                            APRegister register = GetAPRegister(check.RefNbr);
                            if (register.DocType == APDocType.Prepayment)
                                KGVoucherUtil.CreateGLFromKGVoucher(register, KGVoucherType.REVERSEGL, GLStageCode.NMPVoidPZ);

                            PXUpdate<
                                Set<KGBillPaymentExt.usrIsCheckIssued, False,
                                Set<KGBillPaymentExt.usrPayableCheckID, Null,
                                //2021/08/24 Add Mantis: 0012208
                                Set<KGBillPaymentExt.usrIsTrConfirm,False>>>,
                            KGBillPayment,
                            Where<KGBillPayment.billPaymentID, Equal<Required<KGBillPayment.billPaymentID>>>>
                            .Update(this, check.BillPaymentID);
                        }
                    }

                    ts.Complete();
                }
            });

        }

        public void VoidAPPaymant(string payRefnbr)
        {
            if (payRefnbr == null) return;
            APPaymentEntry entry = PXGraph.CreateInstance<APPaymentEntry>();
            entry.Document.Current = entry.Document.Search<APPayment.refNbr>(payRefnbr);
            APPayment apPayment = entry.Document.Current;
            if (apPayment == null) return;
            entry.voidCheck.Press();
            entry.Save.Press();
            //作廢後還要過帳
            apPayment = entry.Document.Current;
            apPayment.Hold = false;
            entry.Document.Update(apPayment);
            entry.release.Press();
        }

        private APRegister GetAPRegister(string RefNbr)
        {
            return PXSelect<APRegister,
                Where<APRegister.refNbr, Equal<Required<APRegister.refNbr>>>>
                .Select(this, RefNbr);
        }
        private APPayment GetAPPayment(string payRefNbr)
        {
            return PXSelect<APPayment,
                Where<APPayment.refNbr, Equal<Required<APPayment.refNbr>>>>
                .Select(this, payRefNbr);
        }
        #endregion

        #region HyperLink

        #region PayableCD
        public PXAction<NMPayableCheck> ViewPayableCD;
        [PXButton(CommitChanges = true)]
        [PXUIField(Visible = false)]
        protected virtual void viewPayableCD()
        {
            NMPayableCheck check = (NMPayableCheck)PXSelectorAttribute.Select<NMPayableCheck.payableCheckCD>
                (CheckDetails.Cache, CheckDetails.Current);
            new HyperLinkUtil<NMApCheckEntry>(check, true);
        }
        #endregion

        #region APReleaseGLBatchNbr
        public PXAction<NMPayableCheck> ViewAPReleaseGL;
        [PXButton(CommitChanges = true)]
        [PXUIField(Visible = false)]
        protected virtual void viewAPReleaseGL()
        {

            Batch batch = (Batch)PXSelectorAttribute.Select<NMPayableCheck.aPInvBatchNbr>
                (CheckDetails.Cache, CheckDetails.Current);
            new HyperLinkUtil<JournalEntry>(batch, true);
        }
        #endregion

        #endregion

        [Serializable]
        public class NMApCheckModifyFilter : IBqlTable
        {
            //For Search
            //Cashier
            #region PayableCashierID
            [PXInt()]
            [PXUIField(DisplayName = "Payable Cashier ID")]
            [PXEPEmployeeSelector]
            public virtual int? PayableCashierID { get; set; }
            public abstract class payableCashierID : PX.Data.BQL.BqlInt.Field<payableCashierID> { }
            #endregion

            #region SendCashierID
            [PXInt()]
            [PXUIField(DisplayName = "Send Cashier ID")]
            [PXEPEmployeeSelector]          
            public virtual int? SendCashierID { get; set; }
            public abstract class sendCashierID : PX.Data.BQL.BqlInt.Field<sendCashierID> { }
            #endregion

            #region CashCashierID
            [PXInt()]
            [PXUIField(DisplayName = "Cash Cashier ID")]
            [PXEPEmployeeSelector]
            public virtual int? CashCashierID { get; set; }
            public abstract class cashCashierID : PX.Data.BQL.BqlInt.Field<cashCashierID> { }
            #endregion

            //CheckNbr From To
            #region CheckNbrFrom
            [PXString(12, IsFixed = true, IsUnicode = true, InputMask = "")]
            [PXUIField(DisplayName = "Check Nbr From")]
            public virtual string CheckNbrFrom { get; set; }
            public abstract class checkNbrFrom : PX.Data.BQL.BqlString.Field<checkNbrFrom> { }
            #endregion

            #region CheckNbrFrom
            [PXString(12, IsFixed = true, IsUnicode = true, InputMask = "")]
            [PXUIField(DisplayName = "Check Nbr To")]
            public virtual string CheckNbrTo { get; set; }
            public abstract class checkNbrTo : PX.Data.BQL.BqlString.Field<checkNbrTo> { }
            #endregion

            //Check Date From To
            #region CheckDateFrom
            [PXDate()]
            [PXUIField(DisplayName = "Check Date From")]
            public virtual DateTime? CheckDateFrom { get; set; }
            public abstract class checkDateFrom : PX.Data.BQL.BqlDateTime.Field<checkDateFrom> { }
            #endregion

            #region CheckDateTo
            [PXDate()]
            [PXUIField(DisplayName = "Check Date To")]
            public virtual DateTime? CheckDateTo { get; set; }
            public abstract class checkDateTo : PX.Data.BQL.BqlDateTime.Field<checkDateTo> { }
            #endregion

            //Due Date
            #region DueDateFrom
            [PXDate()]
            [PXUIField(DisplayName = "Due Date From")]
            public virtual DateTime? DueDateFrom { get; set; }
            public abstract class dueDateFrom : PX.Data.BQL.BqlDateTime.Field<dueDateFrom> { }
            #endregion

            #region DueDateTo
            [PXDate()]
            [PXUIField(DisplayName = "Due Date To")]
            public virtual DateTime? DueDateTo { get; set; }
            public abstract class dueDateTo : PX.Data.BQL.BqlDateTime.Field<dueDateTo> { }
            #endregion

            //Deposit Date
            #region DepositDateFrom
            [PXDate()]
            [PXUIField(DisplayName = "Deposit Date From")]
            public virtual DateTime? DepositDateFrom { get; set; }
            public abstract class depositDateFrom : PX.Data.BQL.BqlDateTime.Field<depositDateFrom> { }
            #endregion

            #region DepositDateTo
            [PXDate()]
            [PXUIField(DisplayName = "Deposit Date To")]
            public virtual DateTime? DepositDateTo { get; set; }
            public abstract class depositDateTo : PX.Data.BQL.BqlDateTime.Field<depositDateTo> { }
            #endregion

            //Vendor ID
            #region VendorID
            [PXInt()]
            [PXUIField(DisplayName = "Vendor ID")]
            [POVendor(Visibility = PXUIVisibility.SelectorVisible,
            DescriptionField = typeof(Vendor.acctName),
            CacheGlobal = true, Filterable = true)]
            public virtual int? VendorID { get; set; }
            public abstract class vendorID : PX.Data.BQL.BqlInt.Field<vendorID> { }
            #endregion

            //VendorLocationID
            #region VendorLocationID
            [PXInt()]
            [PXUIField(DisplayName = "Vendor Location ID")]
            [PXSelector(typeof(Search<LocationExtAddress.locationID,
            Where<LocationExtAddress.bAccountID, Equal<Current<vendorID>>>>),
            typeof(LocationExtAddress.locationCD),
            SubstituteKey = typeof(LocationExtAddress.addressLine1))]
            public virtual int? VendorLocationID { get; set; }
            public abstract class vendorLocationID : PX.Data.BQL.BqlInt.Field<vendorLocationID> { }
            #endregion

            //ProjectID
            #region ProjectID
            [PXInt()]
            [PXUIField(DisplayName = "Project ID")]
            [ProjectBase()]
            [PXRestrictor(typeof(Where<PMProject.isCancelled, Equal<False>>), PX.Objects.PM.Messages.CancelledContract, typeof(PMProject.contractCD))]
            [PXRestrictor(typeof(Where<PMProject.visibleInPO, Equal<True>, Or<PMProject.nonProject, Equal<True>>>), PX.Objects.PM.Messages.ProjectInvisibleInModule, typeof(PMProject.contractCD))]
            [PXRestrictor(typeof(Where<PMProject.defaultBranchID, Equal<Current<AccessInfo.branchID>>, Or<PMProject.defaultBranchID, IsNull>>), "Branch Not Found.", typeof(PMProject.contractCD))]
            [PXForeignReference(typeof(Field<projectID>.IsRelatedTo<PMProject.contractID>))]
            public virtual int? ProjectID { get; set; }
            public abstract class projectID : PX.Data.BQL.BqlInt.Field<projectID> { }
            #endregion

            #region ProjectPeriod
            [PXInt()]
            [PXUIField(DisplayName = "Project Period")]
            public virtual int? ProjectPeriod { get; set; }
            public abstract class projectPeriod : PX.Data.BQL.BqlInt.Field<projectPeriod> { }
            #endregion

            //Bank Account ID
            #region BankAccountID
            [PXInt()]
            [PXUIField(DisplayName = "Bank Account ID")]
            [NMBankAccount()]
            public virtual int? BankAccountID { get; set; }
            public abstract class bankAccountID : PX.Data.BQL.BqlInt.Field<bankAccountID> { }
            #endregion

            //2021/04/27 Add
            #region PaymentType
            [PXString(1)]
            [PXUIField(DisplayName = "UsrTrPaymentType")]
            [PXSelector(typeof(Search<SegmentValue.value,
                           Where<SegmentValue.active, Equal<True>,
                               And<SegmentValue.dimensionID, Equal<NMSegmentKey.nmTrPaymentType>,
                                   And<SegmentValue.segmentID, Equal<NMSegmentKey.segmentIDPart1>>>>>),
                   typeof(SegmentValue.value),
                   typeof(SegmentValue.descr),
                DescriptionField = typeof(SegmentValue.descr))]

            public virtual string TrPaymentType { get; set; }
            public abstract class trPaymentType : PX.Data.BQL.BqlString.Field<trPaymentType> { }
            #endregion

            #region TrConfirmID
            [PXInt()]
            [PXUIField(DisplayName = "TrConfirmID")]
            public virtual int? TrConfirmID { get; set; }
            public abstract class trConfirmID : PX.Data.BQL.BqlInt.Field<trConfirmID> { }
            #endregion

            #region TrConfirmDate
            [PXDate()]
            [PXUIField(DisplayName = "TrConfirmDate")]
            public virtual DateTime? TrConfirmDate { get; set; }
            public abstract class trConfirmDate : IBqlField { }
            #endregion

            #region TrConfirmBy
            [PXGuid]
            [PXUIField(DisplayName = "TrConfirmBy")]
            [PXSelector(typeof(Search<PX.SM.Users.pKID>),
                    typeof(PX.SM.Users.username),
                    typeof(PX.SM.Users.firstName),
                    typeof(PX.SM.Users.fullName),
                    SubstituteKey = typeof(PX.SM.Users.username))]

            public virtual Guid? TrConfirmBy { get; set; }
            public abstract class trConfirmBy : PX.Data.BQL.BqlGuid.Field<trConfirmBy> { }
            #endregion


            //For Insert Modify Data
            #region ModifyCashierID
            [PXInt()]
            [PXUIField(DisplayName = "Modify Cashier ID",Required =true)]
            [PXEPEmployeeSelector]
            [PXDefault(typeof(Search<EPEmployee.bAccountID,
            Where<EPEmployee.userID, Equal<Current<AccessInfo.userID>>>>),
                PersistingCheck = PXPersistingCheck.NullOrBlank)]
            public virtual int? ModifyCashierID { get; set; }
            public abstract class modifyCashierID : PX.Data.BQL.BqlInt.Field<modifyCashierID> { }
            #endregion

            #region ModifyDate
            [PXDate()]
            [PXUIField(DisplayName = "Modify Date",Required =true)]
            [PXDefault(typeof(AccessInfo.businessDate),
                PersistingCheck =PXPersistingCheck.NullOrBlank)]
            public virtual DateTime? ModifyDate { get; set; }
            public abstract class modifyDate : PX.Data.BQL.BqlDateTime.Field<modifyDate> { }
            #endregion

            #region ModifyReason
            [PXString(255, IsUnicode = true, InputMask = "")]
            [PXUIField(DisplayName = "Modify Reason",Required =true)]
            [PXDefault(PersistingCheck =PXPersistingCheck.NullOrBlank)]
            public virtual string ModifyReason { get; set; }
            public abstract class modifyReason : PX.Data.BQL.BqlString.Field<modifyReason> { }
            #endregion

            //For Select Cal
            #region SelectedCount
            [PXInt()]
            [PXUIField(DisplayName = "Selected Count", IsReadOnly = true)]
            public virtual int? SelectedCount { get; set; }
            public abstract class selectedCount : PX.Data.BQL.BqlInt.Field<selectedCount> { }
            #endregion

            #region SelectedOriCuryAmount
            [PXDecimal()]
            [PXUIField(DisplayName = "Selected Ori Cury Amount", IsReadOnly = true)]
            public virtual Decimal? SelectedOriCuryAmount { get; set; }
            public abstract class selectedOriCuryAmount : PX.Data.BQL.BqlDecimal.Field<selectedOriCuryAmount> { }
            #endregion

            #region SelectedBaseCuryAmount
            [PXDecimal()]
            [PXUIField(DisplayName = "Selected Base Cury Amount", IsReadOnly = true)]
            public virtual Decimal? SelectedBaseCuryAmount { get; set; }
            public abstract class selectedBaseCuryAmount : PX.Data.BQL.BqlDecimal.Field<selectedBaseCuryAmount> { }
            #endregion
        }

    }
}