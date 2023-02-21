using System;
using System.Collections;
using Kedge.DAC;
using NM.DAC;
using NM.Util;
using PX.Data;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.AP;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.EP;
using PX.Objects.GL;
using PX.Objects.PM;
using PX.Objects.PO;
using RC.Util;
using KG.Util;

namespace NM
{
    /**
     * ===2021/04/27 Mantis: 0012024 ===Althea
     * Add Filter and Shows on Detail (KGBillPayment.UsrTrComfirmBy/ UsrTrComfirmDate/ UsrTrConfirmID/ UsrTrPaymentType )
     * 
     * ===2021-04-29 : 12024====Alton
     * 修正KGBillpayment資料遺失問題
     **/
    public class NMApCheckCashProcess : PXGraph<NMApCheckCashProcess>
    {
        //權限設定開關
        public NMApCheckCashProcess()
        {
            if (!RCFeaturesSetUtil.IsActive(this, RCFeaturesSetProperties.NOTES_PAYABLE))
            {
                RCFeaturesSetUtil.BackToHomePage();
            }

        }

        public PXCancel<NMAPCheckCashFilter> Cancel;

        public PXFilter<NMAPCheckCashFilter> CheckFilters;

        #region View
        //2021/04/27 Modify
        //LeftJoin KGBillPayment for Show 4 Fields: UsrTrComfirmBy/ UsrTrComfirmDate/ UsrTrConfirmID/ UsrTrPaymentType 
        public PXSelectJoin<NMPayableCheck,
            LeftJoin<KGBillPayment, On<KGBillPayment.billPaymentID, Equal<NMPayableCheck.billPaymentID>>>,
              Where<NMPayableCheck.status, Equal<NMStringList.NMAPCheckStatus.confirm>,
                  And2<Where<
                    NMPayableCheck.payableCashierID, Equal<Current2<NMAPCheckCashFilter.payableCashierID>>,
                    Or<Current2<NMAPCheckCashFilter.payableCashierID>, IsNull>>,
                  And2<Where<
                    NMPayableCheck.bankAccountID, Equal<Current2<NMAPCheckCashFilter.bankAccountID>>,
                    Or<Current2<NMAPCheckCashFilter.bankAccountID>, IsNull>>,
                  And2<Where<
                    NMPayableCheck.checkNbr, GreaterEqual<Current2<NMAPCheckCashFilter.checkNbrFrom>>,
                    Or<Current2<NMAPCheckCashFilter.checkNbrFrom>, IsNull>>,
                  And2<Where<
                    NMPayableCheck.checkNbr, LessEqual<Current2<NMAPCheckCashFilter.checkNbrTo>>,
                    Or<Current2<NMAPCheckCashFilter.checkNbrTo>, IsNull>>,
                  And2<Where<
                    NMPayableCheck.checkDate, GreaterEqual<Current2<NMAPCheckCashFilter.checkDateFrom>>,
                    Or<Current2<NMAPCheckCashFilter.checkDateFrom>, IsNull>>,
                  And2<Where<
                    NMPayableCheck.checkDate, LessEqual<Current2<NMAPCheckCashFilter.checkDateTo>>,
                    Or<Current2<NMAPCheckCashFilter.checkDateTo>, IsNull>>,
                  And2<Where<
                    NMPayableCheck.dueDate, GreaterEqual<Current2<NMAPCheckCashFilter.dueDateFrom>>,
                    Or<Current2<NMAPCheckCashFilter.dueDateFrom>, IsNull>>,
                  And2<Where<
                    NMPayableCheck.dueDate, LessEqual<Current2<NMAPCheckCashFilter.dueDateTo>>,
                    Or<Current2<NMAPCheckCashFilter.dueDateTo>, IsNull>>,
                  And2<Where<
                    NMPayableCheck.etdDepositDate, GreaterEqual<Current2<NMAPCheckCashFilter.etdDepositDateFrom>>,
                    Or<Current2<NMAPCheckCashFilter.etdDepositDateFrom>, IsNull>>,
                  And2<Where<
                    NMPayableCheck.etdDepositDate, LessEqual<Current2<NMAPCheckCashFilter.etdDepositDateTo>>,
                    Or<Current2<NMAPCheckCashFilter.etdDepositDateTo>, IsNull>>,
                  And2<Where<
                    NMPayableCheck.vendorID, Equal<Current2<NMAPCheckCashFilter.vendorID>>,
                    Or<Current2<NMAPCheckCashFilter.vendorID>, IsNull>>,
                  And2<Where<
                    NMPayableCheck.vendorLocationID, Equal<Current2<NMAPCheckCashFilter.vendorLocationID>>,
                    Or<Current2<NMAPCheckCashFilter.vendorLocationID>, IsNull>>,
                  And2<Where<
                    NMPayableCheck.projectID, Equal<Current2<NMAPCheckCashFilter.projectID>>,
                    Or<Current2<NMAPCheckCashFilter.projectID>, IsNull>>,
                  And2<Where<
                    NMPayableCheck.projectPeriod, Equal<Current2<NMAPCheckCashFilter.projectPeriod>>,
                    Or<Current2<NMAPCheckCashFilter.projectPeriod>, IsNull>>,
                  And2<Where<
                    KGBillPaymentExt.usrTrConfirmBy, Equal<Current2<NMAPCheckCashFilter.trConfirmBy>>,
                    Or<Current2<NMAPCheckCashFilter.trConfirmBy>, IsNull>>,
                  And2<Where<
                    KGBillPaymentExt.usrTrConfirmDate, Equal<Current2<NMAPCheckCashFilter.trConfirmDate>>,
                    Or<Current2<NMAPCheckCashFilter.trConfirmDate>, IsNull>>,
                  And2<Where<
                      KGBillPaymentExt.usrTrConfirmID, Equal<Current2<NMAPCheckCashFilter.trConfirmID>>,
                      Or<Current2<NMAPCheckCashFilter.trConfirmID>, IsNull>>,
                  And<Where<
                    KGBillPaymentExt.usrTrPaymentType, Equal<Current2<NMAPCheckCashFilter.trPaymntType>>,
                    Or<Current2<NMAPCheckCashFilter.trPaymntType>, IsNull>>
                      >>>>>>>>>>>>>>>>>>>> CheckDetails;
        #endregion

        #region Button
        #region 兌現
        public PXAction<NMAPCheckCashFilter> CashProcessAction;
        [PXButton(CommitChanges = true, Tooltip = "兌現可勾選多筆支票")]
        [PXUIField(DisplayName = "兌現")]
        protected IEnumerable cashProcessAction(PXAdapter adapter)
        {
            NMAPCheckCashFilter filter = CheckFilters.Current;
            NMPayableCheck payableCheck = CheckDetails.Current;
            if (payableCheck != null)
            {
                if (filter.CashCashierID == null || filter.DepositDate == null)
                {
                    string errormsg = "請填妥以下資料:";
                    if (filter.CashCashierID == null)
                    {
                        errormsg = errormsg + "兌現作業人/";
                    }
                    if (filter.DepositDate == null)
                    {
                        errormsg = errormsg + "兌現日期";
                    }
                    throw new Exception(errormsg);
                }

            }
            //兌現應付票據, 兌現日期yyyy/mm/dd, 兌現筆數 XX筆, 兌現金額 xxxxx元, 請確認!
            string dialogMsg = "兌現應付票據\r\n";
            dialogMsg += "兌現日期:{0}\r\n";
            dialogMsg += "兌現筆數:{1}筆\r\n";
            dialogMsg += "兌現金額:{2:#,0.##}元";
            dialogMsg = String.Format(
                dialogMsg,
                filter.DepositDate?.ToString("yyyy/MM/dd"),
                filter.SelectedCount,
                filter.SelectedOriCuryAmount);
            if (CheckFilters.Ask("確認兌現資訊", dialogMsg, MessageButtons.YesNo) == WebDialogResult.Yes)
            {
                cashMethod();
            }
            return adapter.Get();
        }


        #endregion
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
            NMAPCheckCashFilter header = CheckFilters.Current;
            if (row == null) return;

            decimal? selectbaseAmount = 0;
            decimal? selectoriAmount = 0;
            int? selectcount = 0;

            PXResultset<NMPayableCheck> set =
                PXSelect<NMPayableCheck,
              Where<NMPayableCheck.status, Equal<NMStringList.NMAPCheckStatus.confirm>,
              And<NMPayableCheck.selected, Equal<True>>>>.Select(this);
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

        public void cashMethod()
        {
            NMAPCheckCashFilter filter = CheckFilters.Current;
            PXLongOperation.StartOperation(this, delegate ()
            {
                foreach (NMPayableCheck check in CheckDetails.Select())
                {
                    if (check.Selected == true)
                    {
                        check.Status = NMStringList.NMAPCheckStatus.CASH;
                        //以下資訊更新欄位
                        check.CashCashierID = filter.CashCashierID;
                        check.DepositDate = filter.DepositDate;
                        //20220418 by louis NMVoucherUtil.CreateAPVoucher()新增一個參數紀錄傳票產生的時機
                        check.CashBatchNbr = NMVoucherUtil.CreateAPVoucher(NMStringList.NMAPVoucher.CASH, check, GLStageCode.NMPCashP5);
                    }
                    CheckDetails.Update(check);
                }
                base.Persist();
            });
        }


        #endregion

        #region Select Method
        private APInvoice GetInvoice(string RefNbr)
        {
            return PXSelect<APInvoice,
                Where<APInvoice.refNbr, Equal<Required<APInvoice.refNbr>>>>
                .Select(this, RefNbr);
        }
        #endregion

        #region Filter DAC
        [Serializable]
        public class NMAPCheckCashFilter : IBqlTable
        {
            //For Search

            //PayableCashier
            #region PayableCashierID
            [PXInt()]
            [PXUIField(DisplayName = "Payable Cashier ID")]
            [PXEPEmployeeSelector]
            public virtual int? PayableCashierID { get; set; }
            public abstract class payableCashierID : PX.Data.BQL.BqlInt.Field<payableCashierID> { }
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

            //EtdDeposit Date
            #region EtdDepositDateFrom
            [PXDate()]
            [PXUIField(DisplayName = "Etd Deposit Date From")]
            [PXUnboundDefault(typeof(AccessInfo.businessDate))]
            public virtual DateTime? EtdDepositDateFrom { get; set; }
            public abstract class etdDepositDateFrom : PX.Data.BQL.BqlDateTime.Field<etdDepositDateFrom> { }
            #endregion

            #region EtdDepositDateTo
            [PXDate()]
            [PXUIField(DisplayName = "Etd Deposit Date To")]
            [PXUnboundDefault(typeof(AccessInfo.businessDate))]
            public virtual DateTime? EtdDepositDateTo { get; set; }
            public abstract class etdDepositDateTo : PX.Data.BQL.BqlDateTime.Field<etdDepositDateTo> { }
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
            [NMBankAccount(SubstituteKey = typeof(NMBankAccount.bankAccountCD))]
            public virtual int? BankAccountID { get; set; }
            public abstract class bankAccountID : PX.Data.BQL.BqlInt.Field<bankAccountID> { }
            #endregion

            //2021/04/27 Add
            #region PaymntType
            [PXString(1)]
            [PXUIField(DisplayName = "UsrTrPaymntType")]
            [PXSelector(typeof(Search<SegmentValue.value,
                           Where<SegmentValue.active, Equal<True>,
                               And<SegmentValue.dimensionID, Equal<NMSegmentKey.nmTrPaymentType>,
                                   And<SegmentValue.segmentID, Equal<NMSegmentKey.segmentIDPart1>>>>>),
                   typeof(SegmentValue.value),
                   typeof(SegmentValue.descr),
                DescriptionField = typeof(SegmentValue.descr))]

            public virtual string TrPaymntType { get; set; }
            public abstract class trPaymntType : PX.Data.BQL.BqlString.Field<trPaymntType> { }
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


            //For Insert  Data
            #region CashCashierID
            [PXInt()]
            [PXUIField(DisplayName = "Cash Cashier ID", Required = true)]
            [PXEPEmployeeSelector]
            [PXDefault(typeof(Search<EPEmployee.bAccountID,
            Where<EPEmployee.userID, Equal<Current<AccessInfo.userID>>>>),
                PersistingCheck = PXPersistingCheck.NullOrBlank)]
            public virtual int? CashCashierID { get; set; }
            public abstract class cashCashierID : PX.Data.BQL.BqlInt.Field<cashCashierID> { }
            #endregion

            #region DepositDate
            [PXDate()]
            [PXUIField(DisplayName = "Deposit Date", Required = true)]
            [PXDefault(typeof(AccessInfo.businessDate),
                PersistingCheck = PXPersistingCheck.NullOrBlank)]
            public virtual DateTime? DepositDate { get; set; }
            public abstract class depositDate : PX.Data.BQL.BqlDateTime.Field<depositDate> { }
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
        #endregion

    }
}