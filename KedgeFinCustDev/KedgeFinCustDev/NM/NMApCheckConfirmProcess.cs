using System;
using PX.Data;
using NM.DAC;
using NM.Util;
using PX.Objects.EP;
using PX.Objects.CR;
using PX.Objects.PM;
using PX.Objects.GL;
using PX.Data.ReferentialIntegrity.Attributes;
using RC.Util;
using Kedge.DAC;
using System.Collections;
using PX.Objects.AP;
using KG.Util;
using static NM.Util.NMStringList;
using PX.Objects.CS;


/**
 * ====2021-04-29:12025 ====Alton
 * 1.GRID與Filter加入 UsrTrPaymntType & UsrTrConfirmID
 * 2.BatchNbr的顯示改為UsrAccConfirmNbr
 * 
 * ====2021-05-20====Alton
 * 追APInvoice加到期日條件
 * 
 * ====2021-06-01:12071====Alton
 * 	1.產生 APPayment時, APPayment.CuryOrigDocAmt(實付金額)以及APAdjust.CuryAdjgAmt(支付金額), 原來是寫入應付票據的OriCuryAmount, 請改為OriCuryAmount+ KGBillPayment.PostageAmt(郵資費)
 * 	---->改在NMApCheckEntry
 * 	
 * ====2021-07-08:12132====Alton
 * APPayment過帳時產生 APPayment的GL傳票 & 郵資費傳票
 * ====2021-07-09:12132====Alton
 * 將傳票號碼回寫NMPayableCheck
 * 
 * **/
namespace NM
{
    public class NMApCheckConfirmProcess : PXGraph<NMApCheckConfirmProcess>
    {

        public NMApCheckConfirmProcess()
        {
            if (!RCFeaturesSetUtil.IsActive(this, RCFeaturesSetProperties.NOTES_PAYABLE))
            {
                RCFeaturesSetUtil.BackToHomePage();
            }
        }

        #region View
        public PXFilter<ParamTable> MasterView;
        public PXSelect<ViewTable,
            Where<ViewTable.status, Equal<Current2<ParamTable.status>>,
                And2<Where<
                    ViewTable.payableCashierID, Equal<Current2<ParamTable.cashierID>>,
                    Or<Current2<ParamTable.cashierID>, IsNull>>,
                And2<Where<
                    ViewTable.bankAccountID, Equal<Current2<ParamTable.bankAccountID>>,
                    Or<Current2<ParamTable.bankAccountID>, IsNull>>,
                And2<Where<
                    ViewTable.vendorID, Equal<Current2<ParamTable.vendorID>>,
                    Or<Current2<ParamTable.vendorID>, IsNull>>,
                And2<Where<
                    ViewTable.vendorLocationID, Equal<Current2<ParamTable.vendorLocationID>>,
                    Or<Current2<ParamTable.vendorLocationID>, IsNull>>,
                And2<Where<
                    ViewTable.projectPeriod, GreaterEqual<Current2<ParamTable.projectStartPeriod>>,
                    Or<Current2<ParamTable.projectStartPeriod>, IsNull>>,
                And2<Where<
                    ViewTable.projectPeriod, LessEqual<Current2<ParamTable.projectEndPeriod>>,
                    Or<Current2<ParamTable.projectEndPeriod>, IsNull>>,
                And2<Where<
                    ViewTable.checkDate, GreaterEqual<Current2<ParamTable.checkStartDate>>,
                    Or<Current2<ParamTable.checkStartDate>, IsNull>>,
                And2<Where<
                    ViewTable.apDueDate, Equal<Current2<ParamTable.dueDate>>,
                    Or<Current2<ParamTable.dueDate>, IsNull>>,
                And2<Where<
                    ViewTable.checkDate, LessEqual<Current2<ParamTable.checkEndDate>>,
                    Or<Current2<ParamTable.checkEndDate>, IsNull>>,
                And2<Where<ViewTable.projectID, Equal<Current2<ParamTable.projectID>>,
                    Or<Current2<ParamTable.projectID>, IsNull>>,
                And2<Where<Current2<ParamTable.usrTrConfirmBy>, IsNull,
                    Or<ViewTable.usrTrConfirmBy, Equal<Current2<ParamTable.usrTrConfirmBy>>>>,
                And2<Where<Current2<ParamTable.usrTrConfirmDate>, IsNull,
                    Or<ViewTable.usrTrConfirmDate, Equal<Current2<ParamTable.usrTrConfirmDate>>>>,
                And2<Where<Current2<ParamTable.usrTrConfirmID>, IsNull,
                    Or<ViewTable.usrTrConfirmID, Equal<Current2<ParamTable.usrTrConfirmID>>>>,
                And<Where<Current2<ParamTable.usrTrPaymentType>, IsNull,
                    Or<ViewTable.usrTrPaymentType, Equal<Current2<ParamTable.usrTrPaymentType>>>>
                    >>>>>>>>>>>>>>>
            > DetailsView;
        #endregion
        //public PXCancel<ParamTable> Cancel;

        #region HyperLink
        public PXAction<ViewTable> ViewCheck;
        [PXButton(CommitChanges = true)]
        [PXUIField(Visible = false)]
        protected virtual void viewCheck()
        {
            NMPayableCheck check = (NMPayableCheck)PXSelectorAttribute.Select<ViewTable.payableCheckCD>(DetailsView.Cache, DetailsView.Current);
            new HyperLinkUtil<NMApCheckEntry>(check, true);
        }

        #region APReleaseGLBatchNbr
        public PXAction<ViewTable> ViewAPReleaseGL;
        [PXButton(CommitChanges = true)]
        [PXUIField(Visible = false)]
        protected virtual void viewAPReleaseGL()
        {
            Batch batch = (Batch)PXSelectorAttribute.Select<ViewTable.aPInvBatchNbr>(DetailsView.Cache, DetailsView.Current);
            new HyperLinkUtil<JournalEntry>(batch, true);
        }
        #endregion

        #region Vendor
        public PXAction<ViewTable> ViewVendor;
        [PXButton(CommitChanges = true)]
        [PXUIField(Visible = false)]
        protected virtual void viewVendor()
        {
            BAccount2 vendor = (BAccount2)PXSelectorAttribute.Select<ViewTable.vendorID>(DetailsView.Cache, DetailsView.Current);
            VendorMaint graph = PXGraph.CreateInstance<VendorMaint>();
            graph.BAccount.Current = graph.BAccount.Search<BAccount.bAccountID>(vendor.BAccountID);
            if (graph.BAccount.Current != null)
            {
                throw new PXRedirectRequiredException(graph, "VendorMaint")
                {
                    Mode = PXBaseRedirectException.WindowMode.NewWindow
                };
            }
        }
        #endregion

        #endregion

        #region Event
        protected virtual void ViewTable_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            ViewTable row = (ViewTable)e.Row;
            if (row == null) return;
            setReadOnly();
        }
        #endregion



        #region Action
        #region 確認
        public PXAction<ParamTable> Confirm;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "確認")]
        protected IEnumerable confirm(PXAdapter adapter)
        {
            PXLongOperation.StartOperation(this, ConfirmCheck);
            return adapter.Get();
        }
        #endregion
        #endregion

        #region Event
        protected virtual void _(Events.FieldUpdated<ViewTable.selected> e)
        {
            ViewTable row = (ViewTable)e.Row;
            if (row == null) return;
            SetSelectedInformation();
        }

        protected virtual void _(Events.FieldUpdated<ViewTable.bankAccountID> e)
        {
            ViewTable row = (ViewTable)e.Row;
            if (row == null) return;
            e.Cache.SetDefaultExt<ViewTable.bookNbr>(row);
        }

        protected virtual void _(Events.FieldDefaulting<ViewTable.bookNbr> e)
        {
            ViewTable row = (ViewTable)e.Row;
            if (row == null) return;
            if ((row.IsIssuedByBank ?? false) == false)
            {
                NMCheckBook ncb = GetCheckBook(row.BankAccountID);
                e.NewValue = ncb?.BookCD;
            }
            else
            {
                e.NewValue = null;
            }

        }
        #endregion

        #region method

        private void SetSelectedInformation()
        {
            ParamTable p = MasterView.Current;
            int count = 0;
            decimal oriAmt = 0m;
            decimal basAmt = 0m;
            foreach (NMPayableCheck item in DetailsView.Select())
            {
                if (item.Selected != true) continue;
                count += 1;
                oriAmt += item.OriCuryAmount ?? 0m;
                basAmt += item.BaseCuryAmount ?? 0m;
            }
            p.SelectedCount = count;
            p.SelectedOriCuryAmount = oriAmt;
            p.SelectedBaseCuryAmount = basAmt;

        }

        private void ConfirmCheck()
        {
            using (PXTransactionScope ts = new PXTransactionScope())
            {
                foreach (NMPayableCheck item in DetailsView.Select())
                {
                    if (item.Selected != true) continue;

                    //20210107 確認時才產生APPayment
                    NMPayableCheck _item = item;
                    if (!(item.IsManualCheckNbr ?? false))
                    {
                        _item.CheckNbr = NMCheckBookUtil.getCheckNbr(_item.BookNbr);
                    }

                    //2020/09/17 Althea Add Confirm GL
                    //20220418 by louis NMVoucherUtil.CreateAPVoucher()新增一個參數glStageCode紀錄傳票產生的時機
                    PXUpdate<Set<NMPayableCheck.status, Required<NMPayableCheck.status>,//status
                             Set<NMPayableCheck.checkNbr, Required<NMPayableCheck.checkNbr>,//checkNbr
                             Set<NMPayableCheck.bankAccountID, Required<NMPayableCheck.bankAccountID>,//BankAccountID
                             Set<NMPayableCheck.bookNbr, Required<NMPayableCheck.bookNbr>,//BookNbr
                             Set<NMPayableCheck.confirmBatchNbr, Required<NMPayableCheck.confirmBatchNbr>,//confirmBatchNbr
                             Set<NMPayableCheck.paymentDocNo, Required<NMPayableCheck.paymentDocNo>,//APpayment關聯
                             Set<NMPayableCheck.paymentDocDate, Required<NMPayableCheck.paymentDocDate>//APpayment關聯
                                                                                                       //Set<NMPayableCheck.lastModifiedByID, Current<AccessInfo.userID>,//WhoColumn
                                                                                                       //Set<NMPayableCheck.lastModifiedByScreenID, Current<AccessInfo.screenID>,//WhoColumn
                                                                                                       //Set<NMPayableCheck.lastModifiedDateTime, Current<AccessInfo.businessDate>>>>//WhoColumn
                             >>>>>>>,
                        NMPayableCheck,
                        Where<NMPayableCheck.payableCheckID, Equal<Required<NMPayableCheck.payableCheckID>>>
                        >.Update(this,
                        NMStringList.NMAPCheckStatus.CONFIRM, //status
                        _item.CheckNbr,//checkNbr
                        _item.BankAccountID,//BankAccountID
                        _item.BookNbr,//bookNbr
                        NMVoucherUtil.CreateAPVoucher(NMAPVoucher.CONFIRM, _item, GLStageCode.NMPConfirmP1), //confirmBathNbr
                        _item.PaymentDocNo, _item.PaymentDocDate,//APPayment關聯
                        item.PayableCheckID);

                    PXUpdate<
                        Set<KGBillPaymentExt.usrPayableCheckID, Required<KGBillPaymentExt.usrPayableCheckID>,
                        Set<KGBillPaymentExt.usrCreatePaymentType, NMApCreatePaymentType.check>>,
                         KGBillPayment,
                         Where<KGBillPayment.billPaymentID, Equal<Required<KGBillPayment.billPaymentID>>>>.Update(this,
                            item.PayableCheckID,
                            item.BillPaymentID
                         );

                    APInvoice invoice = GetAPInvoice(_item.RefNbr);
                    if (invoice.DocType == APDocType.Prepayment)
                    {
                        #region 產生PPM傳票
                        //20220418 by louis NMVoucherUtil.CreateAPVoucher()新增一個參數glStageCode紀錄傳票產生的時機
                        String confirmPPMBatchNbr = NMVoucherUtil.CreateAPVoucher(NMAPVoucher.PPM, _item, GLStageCode.NMPayableConfirm);
                        //add by alton 2021-07-16 :12133
                        PXUpdate<Set<NMPayableCheck.confirmPPMBatchNbr, Required<NMPayableCheck.confirmPPMBatchNbr>//confirmPPMBatchNbr
                                 >,
                            NMPayableCheck,
                            Where<NMPayableCheck.payableCheckID, Equal<Required<NMPayableCheck.payableCheckID>>>
                            >.Update(new PXGraph(),
                            confirmPPMBatchNbr,
                            item.PayableCheckID);
                        #endregion
                    }
                    else
                    {
                        #region 產生APPayment 傳票 & 郵資傳票
                        //20220418 by louis NMVoucherUtil.CreateAPVoucher()新增一個參數glStageCode紀錄傳票產生的時機
                        String confirmAPPaymentBatchNbr = NMVoucherUtil.CreateAPVoucher(NMAPVoucher.APPAYMENT, _item, GLStageCode.NMPConfirmP3);
                        //20221230 Alton 合併至P3不再產生P4
                        //String confirmWriteoffPostageBatchNbr = NMVoucherUtil.CreateAPVoucher(NMAPVoucher.WRITEOFFPOSTAGE, _item, GLStageCode.NMPConfirmP4);
                        String confirmWriteoffPostageBatchNbr = null;
                        //add by alton 2021-07-09 :12132
                        PXUpdate<Set<NMPayableCheck.confirmAPPaymentBatchNbr, Required<NMPayableCheck.confirmAPPaymentBatchNbr>,//confirmAPPaymentBatchNbr
                                 Set<NMPayableCheck.confirmWriteoffPostageBatchNbr, Required<NMPayableCheck.confirmWriteoffPostageBatchNbr>//confirmWriteoffPostageBatchNbr
                                 >>,
                            NMPayableCheck,
                            Where<NMPayableCheck.payableCheckID, Equal<Required<NMPayableCheck.payableCheckID>>>
                            >.Update(new PXGraph(),
                            confirmAPPaymentBatchNbr,
                            confirmWriteoffPostageBatchNbr,
                            item.PayableCheckID);
                        #endregion
                    }
                }
                ts.Complete();
            }
            DetailsView.Cache.Clear();
            DetailsView.Cache.ClearQueryCache();
            SetSelectedInformation();
        }

        private void setReadOnly()
        {
            ViewTable row = DetailsView.Current;
            var isReadOnly = (row.IsIssuedByBank ?? false) || (row.IsManualCheckNbr ?? false);
            PXUIFieldAttribute.SetReadOnly(DetailsView.Cache, row, true);
            PXUIFieldAttribute.SetReadOnly<ViewTable.selected>(DetailsView.Cache, row, false);
            PXUIFieldAttribute.SetReadOnly<ViewTable.bankAccountID>(DetailsView.Cache, row, isReadOnly);
            PXUIFieldAttribute.SetReadOnly<ViewTable.bookNbr>(DetailsView.Cache, row, isReadOnly);
            DetailsView.AllowDelete = false;
            DetailsView.AllowInsert = false;
        }


        #endregion

        #region BQL
        public APInvoice GetAPInvoice(string refNbr)
        {
            return PXSelect<APInvoice,
                Where<APInvoice.refNbr, Equal<Required<APInvoice.refNbr>>>>
                .Select(this, refNbr);
        }

        public NMCheckBook GetCheckBook(int? bankAccountID)
        {
            return PXSelect<NMCheckBook,
                Where<NMCheckBook.bankAccountID, Equal<Required<NMCheckBook.bankAccountID>>,
                    And<IsNull<NMCheckBook.bookUsage, NMBookUsage.check>, Equal<NMBookUsage.check>,
                    And<Where<NMCheckBook.currentCheckNbr, NotEqual<NMCheckBook.endCheckNbr>, Or<NMCheckBook.currentCheckNbr, IsNull>>>>>>
                .Select(this, bankAccountID);
        }
        #endregion

        #region Table
        [Serializable]
        [PXHidden]
        [PXProjection(typeof(
            Select2<NMPayableCheck,
            InnerJoin<NMBankAccount, On<NMBankAccount.bankAccountID, Equal<NMPayableCheck.bankAccountID>>,
            InnerJoin<KGBillPayment, On<KGBillPayment.billPaymentID, Equal<NMPayableCheck.billPaymentID>>,
            InnerJoin<APInvoice, On<APInvoice.refNbr, Equal<NMPayableCheck.refNbr>>>>>,
            Where<NMPayableCheck.status, Equal<NMStringList.NMAPCheckStatus.represent>,
                Or<Where<NMPayableCheck.status, Equal<NMStringList.NMAPCheckStatus.unconfirm>,
                    And<IsNull<NMBankAccount.enableIssueByBank, False>, Equal<False>>
               >>>>
            ), Persistent = false)]
        public class ViewTable : NMPayableCheck
        {
            #region BankAccountID
            [PXDBInt(BqlField = typeof(NMPayableCheck.bankAccountID))]
            [PXUIField(DisplayName = "Bank Account ID", Required = true)]
            [NMBankAccount(
                        SubstituteKey = typeof(NMBankAccount.bankAccountCD),
                        DescriptionField = typeof(NMBankAccount.bankName)
            )]
            [PXRestrictor(typeof(Where<NMBankAccount.paymentMethodID, Equal<KGConst.check>>), "Bank Account Not Found", typeof(NMBankAccount.enableIssueByBank))]
            [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
            public override int? BankAccountID { get; set; }
            #endregion

            #region PaymentMethodID
            [PXDBString(BqlField = typeof(NMBankAccount.paymentMethodID))]
            public override string PaymentMethodID { get; set; }
            #endregion

            #region CashAccountID
            [PXDBInt(BqlField = typeof(NMBankAccount.cashAccountID))]
            public override int? CashAccountID { get; set; }
            #endregion

            #region UsrTrPaymentType
            [PXDBString(BqlField = typeof(KGBillPaymentExt.usrTrPaymentType))]
            [PXUIField(DisplayName = "UsrTrPaymntType")]
            [PXSelector(typeof(Search<SegmentValue.value,
                           Where<SegmentValue.active, Equal<True>,
                               And<SegmentValue.dimensionID, Equal<NMSegmentKey.nmTrPaymentType>,
                                   And<SegmentValue.segmentID, Equal<NMSegmentKey.segmentIDPart1>>>>>),
                   typeof(SegmentValue.value),
                   typeof(SegmentValue.descr),
                DescriptionField = typeof(SegmentValue.descr))]

            public virtual string UsrTrPaymentType { get; set; }
            public abstract class usrTrPaymentType : PX.Data.BQL.BqlString.Field<usrTrPaymentType> { }
            #endregion

            #region UsrTrConfirmID
            [PXDBInt(BqlField = typeof(KGBillPaymentExt.usrTrConfirmID))]
            [PXUIField(DisplayName = "UsrTrConfirmID")]
            public virtual int? UsrTrConfirmID { get; set; }
            public abstract class usrTrConfirmID : PX.Data.BQL.BqlInt.Field<usrTrConfirmID> { }
            #endregion

            #region UsrTrConfirmDate
            [PXDBDate(BqlField = typeof(KGBillPaymentExt.usrTrConfirmDate))]
            [PXUIField(DisplayName = "UsrTrConfirmDate")]
            public virtual DateTime? UsrTrConfirmDate { get; set; }
            public abstract class usrTrConfirmDate : IBqlField { }
            #endregion

            #region UsrTrConfirmBy
            [PXDBGuid(BqlField = typeof(KGBillPaymentExt.usrTrConfirmBy))]
            [PXUIField(DisplayName = "UsrTrConfirmBy")]
            [PXSelector(typeof(Search<PX.SM.Users.pKID>),
                    typeof(PX.SM.Users.username),
                    typeof(PX.SM.Users.firstName),
                    typeof(PX.SM.Users.fullName),
                    SubstituteKey = typeof(PX.SM.Users.username))]

            public virtual Guid? UsrTrConfirmBy { get; set; }
            public abstract class usrTrConfirmBy : PX.Data.BQL.BqlGuid.Field<usrTrConfirmBy> { }
            #endregion

            #region DueDate
            [PXDBDate(BqlField = typeof(APInvoice.dueDate))]
            [PXUIField(DisplayName = "Due Date")]
            public virtual DateTime? APDueDate { get; set; }
            public abstract class apDueDate : PX.Data.BQL.BqlDateTime.Field<apDueDate> { }
            #endregion

        }

        [Serializable]
        public class ParamTable : IBqlTable
        {
            #region BranchID
            [PXInt()]
            [PXUIField(DisplayName = "Branch ID")]
            [PXDefault(
                typeof(Search<Branch.branchID,
                    Where<Branch.active, Equal<True>,
                        And<Branch.branchID, Equal<Current<AccessInfo.branchID>>>>>)
                , PersistingCheck = PXPersistingCheck.Nothing
                )]
            [PXDimensionSelector("BIZACCT", typeof(Search<Branch.branchID, Where<Branch.active, Equal<True>, And<MatchWithBranch<Branch.branchID>>>>), typeof(Branch.branchCD), DescriptionField = typeof(Branch.acctName))]
            public virtual int? BranchID { get; set; }
            public abstract class branchID : PX.Data.BQL.BqlInt.Field<branchID> { }
            #endregion

            #region Status
            [PXInt()]
            [PXUIField(DisplayName = "Status")]
            [PXIntList(
                new int[] { NMStringList.NMAPCheckStatus.UNCONFIRM, NMStringList.NMAPCheckStatus.REPRESENT },
                new String[] { "未確認", "已代開" }
                )]
            [PXUnboundDefault(typeof(NMStringList.NMAPCheckStatus.unconfirm))]
            public virtual int? Status { get; set; }
            public abstract class status : PX.Data.BQL.BqlInt.Field<status> { }
            #endregion

            #region CashierID
            [PXInt()]
            [PXUIField(DisplayName = "Cashier")]
            [PXEPEmployeeSelector]
            [PXDefault(typeof(Search<EPEmployee.bAccountID,
            Where<EPEmployee.userID, Equal<Current<AccessInfo.userID>>>>),
            PersistingCheck = PXPersistingCheck.Nothing)]
            public virtual int? CashierID { get; set; }
            public abstract class cashierID : PX.Data.BQL.BqlInt.Field<cashierID> { }
            #endregion

            #region CheckStartDate
            [PXDate()]
            [PXUIField(DisplayName = "Check Start Date")]
            public virtual DateTime? CheckStartDate { get; set; }
            public abstract class checkStartDate : PX.Data.BQL.BqlDateTime.Field<checkStartDate> { }
            #endregion

            #region CheckEndDate
            [PXDate()]
            [PXUIField(DisplayName = "Check End Date")]
            public virtual DateTime? CheckEndDate { get; set; }
            public abstract class checkEndDate : PX.Data.BQL.BqlDateTime.Field<checkEndDate> { }
            #endregion

            #region DeliverStartDate
            [PXDate()]
            [PXUIField(DisplayName = "Deliver Start Date")]
            public virtual DateTime? DeliverStartDate { get; set; }
            public abstract class deliverStartDate : PX.Data.BQL.BqlDateTime.Field<deliverStartDate> { }
            #endregion

            #region DeliverEndDate
            [PXDate()]
            [PXUIField(DisplayName = "Deliver End Date")]
            public virtual DateTime? DeliverEndDate { get; set; }
            public abstract class deliverEndDate : PX.Data.BQL.BqlDateTime.Field<deliverEndDate> { }
            #endregion

            #region VendorID
            [PXInt()]
            [PXUIField(DisplayName = "Vendor ID")]
            [PXSelector(typeof(Search<BAccount.bAccountID, Where<BAccount.status, Equal<EPConst.active>,
            And<Where<BAccount.type, Equal<BAccountType.vendorType>,
                Or<BAccount.type, Equal<BAccountType.combinedType>,
                Or<BAccount.type, Equal<BAccountType.employeeType>>>>>>>),
                        SubstituteKey = typeof(BAccount.acctCD),
                        DescriptionField = typeof(BAccount.acctName))]
            public virtual int? VendorID { get; set; }
            public abstract class vendorID : PX.Data.BQL.BqlInt.Field<vendorID> { }
            #endregion

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

            #region ProjectID
            //[PXInt()]
            [PXUIField(DisplayName = "Project ID")]
            [ProjectBase()]
            [PXRestrictor(typeof(Where<PMProject.isCancelled, Equal<False>>), PX.Objects.PM.Messages.CancelledContract, typeof(PMProject.contractCD))]
            [PXRestrictor(typeof(Where<PMProject.visibleInPO, Equal<True>, Or<PMProject.nonProject, Equal<True>>>), PX.Objects.PM.Messages.ProjectInvisibleInModule, typeof(PMProject.contractCD))]
            [PXRestrictor(typeof(Where<PMProject.defaultBranchID, Equal<Current<AccessInfo.branchID>>>), "Branch Not Found.", typeof(PMProject.contractCD))]
            [PXForeignReference(typeof(Field<projectID>.IsRelatedTo<PMProject.contractID>))]
            public virtual int? ProjectID { get; set; }
            public abstract class projectID : PX.Data.BQL.BqlInt.Field<projectID> { }
            #endregion

            #region ProjectPeriodStart
            [PXInt()]
            [PXUIField(DisplayName = "Project Start Period")]
            public virtual int? ProjectStartPeriod { get; set; }
            public abstract class projectStartPeriod : PX.Data.BQL.BqlInt.Field<projectStartPeriod> { }
            #endregion

            #region ProjectEndPeriod
            [PXInt()]
            [PXUIField(DisplayName = "Project End Period")]
            public virtual int? ProjectEndPeriod { get; set; }
            public abstract class projectEndPeriod : PX.Data.BQL.BqlInt.Field<projectEndPeriod> { }
            #endregion

            #region BankAccountID
            [PXInt()]
            [PXUIField(DisplayName = "Bank Account ID")]
            [NMBankAccount(
                        SubstituteKey = typeof(NMBankAccount.bankAccountCD),
                        DescriptionField = typeof(NMBankAccount.bankName)
            )]
            public virtual int? BankAccountID { get; set; }
            public abstract class bankAccountID : PX.Data.BQL.BqlInt.Field<bankAccountID> { }
            #endregion

            #region DueDate
            [PXDate()]
            [PXUIField(DisplayName = "Due Date")]
            [PXUnboundDefault(typeof(Current<AccessInfo.businessDate>), PersistingCheck = PXPersistingCheck.Nothing)]
            public virtual DateTime? DueDate { get; set; }
            public abstract class dueDate : PX.Data.BQL.BqlDateTime.Field<dueDate> { }
            #endregion

            #region UsrTrPaymentType
            [PXString()]
            [PXUIField(DisplayName = "UsrTrPaymntType")]
            [PXSelector(typeof(Search<SegmentValue.value,
                           Where<SegmentValue.active, Equal<True>,
                               And<SegmentValue.dimensionID, Equal<NMSegmentKey.nmTrPaymentType>,
                                   And<SegmentValue.segmentID, Equal<NMSegmentKey.segmentIDPart1>>>>>),
                   typeof(SegmentValue.value),
                   typeof(SegmentValue.descr),
                DescriptionField = typeof(SegmentValue.descr))]

            public virtual string UsrTrPaymentType { get; set; }
            public abstract class usrTrPaymentType : PX.Data.BQL.BqlString.Field<usrTrPaymentType> { }
            #endregion

            #region UsrTrConfirmID
            [PXInt()]
            [PXUIField(DisplayName = "UsrTrConfirmID")]
            public virtual int? UsrTrConfirmID { get; set; }
            public abstract class usrTrConfirmID : PX.Data.BQL.BqlInt.Field<usrTrConfirmID> { }
            #endregion

            #region UsrTrConfirmDate
            [PXDate()]
            [PXUIField(DisplayName = "UsrTrConfirmDate")]
            public virtual DateTime? UsrTrConfirmDate { get; set; }
            public abstract class usrTrConfirmDate : IBqlField { }
            #endregion

            #region UsrTrConfirmBy
            [PXGuid()]
            [PXUIField(DisplayName = "UsrTrConfirmBy")]
            [PXSelector(typeof(Search<PX.SM.Users.pKID>),
                    typeof(PX.SM.Users.username),
                    typeof(PX.SM.Users.firstName),
                    typeof(PX.SM.Users.fullName),
                    SubstituteKey = typeof(PX.SM.Users.username))]

            public virtual Guid? UsrTrConfirmBy { get; set; }
            public abstract class usrTrConfirmBy : PX.Data.BQL.BqlGuid.Field<usrTrConfirmBy> { }
            #endregion

            #region Selected Information

            #region SelectedCount
            [PXInt()]
            [PXUIField(DisplayName = "Selected Count", IsReadOnly = true)]
            [PXUnboundDefault(TypeCode.Int32, "0")]
            public virtual int? SelectedCount { get; set; }
            public abstract class selectedCount : PX.Data.BQL.BqlInt.Field<selectedCount> { }
            #endregion

            #region SelectedOriCuryAmount
            [PXDecimal(4)]
            [PXUIField(DisplayName = "Selected Ori Cury Amount", IsReadOnly = true)]
            [PXUnboundDefault(TypeCode.Decimal, "0.0")]
            public virtual Decimal? SelectedOriCuryAmount { get; set; }
            public abstract class selectedOriCuryAmount : PX.Data.BQL.BqlDecimal.Field<selectedOriCuryAmount> { }
            #endregion

            #region SelectedBaseCuryAmount
            [PXDecimal(4)]
            [PXUIField(DisplayName = "Selected Base Cury Amount", IsReadOnly = true)]
            [PXUnboundDefault(TypeCode.Decimal, "0.0")]
            public virtual Decimal? SelectedBaseCuryAmount { get; set; }
            public abstract class selectedBaseCuryAmount : PX.Data.BQL.BqlDecimal.Field<selectedBaseCuryAmount> { }
            #endregion

            #endregion
        }
        #endregion

    }
}