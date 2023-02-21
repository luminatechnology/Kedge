using System;
using PX.Data;
using NM.DAC;
using System.Collections;
using PX.Objects.PM;
using PX.Objects.CR;
using PX.Objects.EP;
using NM.Util;
using PX.Objects.AP;
using PX.Objects.PO;
using RC.Util;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.GL;
using Kedge.DAC;
using PX.Objects.CS;

namespace NM
{
    /**
     * ===2021/04/27 Mantis: 0012024 === Althea
     * Add Filter and Shows on Detail (KGBillPayment.UsrTrComfirmBy/ UsrTrComfirmDate/ UsrTrConfirmID/ UsrTrPaymentType )
     * 
     * ===2021-04-29 : 12024====Alton
     * 修正KGBillpayment資料遺失問題
     **/
    public class NMApCheckSendProcess : PXGraph<NMApCheckSendProcess>
    {

        //權限設定開關
        public NMApCheckSendProcess()
        {
            if (!RCFeaturesSetUtil.IsActive(this, RCFeaturesSetProperties.NOTES_PAYABLE))
            {
                RCFeaturesSetUtil.BackToHomePage();
            }

        }

        public PXCancel<NMAPCheckSendFilter> Cancel;

        public PXFilter<NMAPCheckSendFilter> CheckFilters;

        //2021/04/27 Modify
        //LeftJoin KGBillPayment for Show 4 Fields: UsrTrComfirmBy/ UsrTrComfirmDate/ UsrTrConfirmID/ UsrTrPaymentType 
        public PXSelectJoin<NMPayableCheck,
            InnerJoin<KGBillPayment, On<KGBillPayment.billPaymentID,Equal<NMPayableCheck.billPaymentID>>>,
            Where<NMPayableCheck.status, Equal<NMStringList.NMAPCheckStatus.confirm>,
                And<NMPayableCheck.sendCashierID, IsNull,
                    And<NMPayableCheck.depositDate, IsNull>>>> CheckDetails;

        protected virtual IEnumerable checkDetails()
        {
            NMAPCheckSendFilter filter = CheckFilters.Current;
            PXSelectJoin<NMPayableCheck,
            InnerJoin<KGBillPayment, On<KGBillPayment.billPaymentID, Equal<NMPayableCheck.billPaymentID>>>,
            Where<NMPayableCheck.status, Equal<NMStringList.NMAPCheckStatus.confirm>,
                And<NMPayableCheck.sendCashierID, IsNull,
                    And<NMPayableCheck.depositDate, IsNull>>>> query =
            new PXSelectJoin<NMPayableCheck,
            InnerJoin<KGBillPayment, On<KGBillPayment.billPaymentID, Equal<NMPayableCheck.billPaymentID>>>,
            Where<NMPayableCheck.status, Equal<NMStringList.NMAPCheckStatus.confirm>,
                And<NMPayableCheck.sendCashierID, IsNull,
                    And<NMPayableCheck.depositDate, IsNull>>>>(this);

            //query

            if (filter.PayableCashierID != null)
                query.WhereAnd<Where<NMPayableCheck.payableCashierID,
                    Equal<Current<NMAPCheckSendFilter.payableCashierID>>>>();
            if (filter.BankAccountID != null)
                query.WhereAnd<Where<NMPayableCheck.bankAccountID,
                    Equal<Current<NMAPCheckSendFilter.bankAccountID>>>>();
            if (filter.CheckNbrFrom != null)
                query.WhereAnd<Where<NMPayableCheck.checkNbr,
                    GreaterEqual<Current<NMAPCheckSendFilter.checkNbrFrom>>>>();
            if (filter.CheckNbrTo != null)
                query.WhereAnd<Where<NMPayableCheck.checkNbr,
                    LessEqual<Current<NMAPCheckSendFilter.checkNbrTo>>>>();
            if (filter.CheckDateFrom != null)
                query.WhereAnd<Where<NMPayableCheck.checkDate,
                    GreaterEqual<Current<NMAPCheckSendFilter.checkDateFrom>>>>();
            if (filter.CheckDateTo != null)
                query.WhereAnd<Where<NMPayableCheck.checkDate,
                    LessEqual<Current<NMAPCheckSendFilter.checkDateTo>>>>();
            if (filter.DueDateFrom != null)
                query.WhereAnd<Where<NMPayableCheck.dueDate,
                    GreaterEqual<Current<NMAPCheckSendFilter.dueDateFrom>>>>();
            if (filter.DueDateTo != null)
                query.WhereAnd<Where<NMPayableCheck.dueDate,
                    LessEqual<Current<NMAPCheckSendFilter.dueDateTo>>>>();
            if (filter.EtdDepositDateFrom != null)
                query.WhereAnd<Where<NMPayableCheck.etdDepositDate,
                    GreaterEqual<Current<NMAPCheckSendFilter.etdDepositDateFrom>>>>();
            if (filter.EtdDepositDateTo != null)
                query.WhereAnd<Where<NMPayableCheck.etdDepositDate,
                    LessEqual<Current<NMAPCheckSendFilter.etdDepositDateTo>>>>();
            if (filter.VendorID != null)
                query.WhereAnd<Where<NMPayableCheck.vendorID,
                    Equal<Current<NMAPCheckSendFilter.vendorID>>>>();
            if (filter.VendorLocationID != null)
                query.WhereAnd<Where<NMPayableCheck.vendorLocationID,
                    Equal<Current<NMAPCheckSendFilter.vendorLocationID>>>>();
            if (filter.ProjectID != null)
                query.WhereAnd<Where<NMPayableCheck.projectID,
                    Equal<Current<NMAPCheckSendFilter.projectID>>>>();
            if (filter.ProjectPeriod != null)
                query.WhereAnd<Where<NMPayableCheck.projectPeriod,
                    Equal<Current<NMAPCheckSendFilter.projectPeriod>>>>();
            if (filter.TrConfirmBy != null)
                query.WhereAnd<Where<KGBillPaymentExt.usrTrConfirmBy,
                    Equal<Current<NMAPCheckSendFilter.trConfirmBy>>>>();
            if (filter.TrConfirmDate != null)
                query.WhereAnd<Where<KGBillPaymentExt.usrTrConfirmDate,
                    Equal<Current<NMAPCheckSendFilter.trConfirmDate>>>>();
            if (filter.TrConfirmID != null)
                query.WhereAnd<Where<KGBillPaymentExt.usrTrConfirmID,
                    Equal<Current<NMAPCheckSendFilter.trConfirmID>>>>();
            if (filter.TrPaymentType != null)
                query.WhereAnd<Where<KGBillPaymentExt.usrTrPaymentType,
                    Equal<Current<NMAPCheckSendFilter.trPaymentType>>>>();

            //設Default
            foreach (PXResult<NMPayableCheck,KGBillPayment> item in query.Select())
            {
                NMPayableCheck payableCheck = (NMPayableCheck)item;
                //if (payableCheck.RefNbr != null)
                //{
                //    APInvoice invoice = GetInvoice(payableCheck.RefNbr);
                //    if (invoice != null)
                //        payableCheck.APInvBatchNbr = invoice.BatchNbr??null;
                //}
                if (payableCheck.Receiver == null)
                {
                    Contact contact = GetContact(payableCheck.VendorID);
                    if (contact != null)
                        payableCheck.Receiver = contact.Attention ?? "";
                }
                yield return item;
            }


        }

        #region Button
        #region 寄領
        public PXAction<NMAPCheckSendFilter> SendProcessAction;
        [PXButton(CommitChanges = true, Tooltip = "寄領可勾選多筆支票")]
        [PXUIField(DisplayName = "寄領")]
        protected IEnumerable sendProcessAction(PXAdapter adapter)
        {
            NMAPCheckSendFilter filter = CheckFilters.Current;
            NMPayableCheck payableCheck = CheckDetails.Current;
            if (payableCheck != null)
            {
                if (filter.SendCashierID == null || filter.DeliverDate == null
                    || filter.DeliverMethod == null)
                {
                    string errormsg = "請填妥以下資料:";
                    if (filter.SendCashierID == null)
                    {
                        errormsg = errormsg + "寄送作業人/";
                    }
                    if (filter.DeliverMethod == null)
                    {
                        errormsg = errormsg + "寄送方式/";
                    }
                    if (filter.DeliverDate == null)
                    {
                        errormsg = errormsg + "寄送日期";
                    }
                    throw new Exception(errormsg);
                }

            }
            sendMethod();

            return adapter.Get();
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
            NMAPCheckSendFilter header = CheckFilters.Current;
            if (row == null) return;

            decimal? selectbaseAmount = 0;
            decimal? selectoriAmount = 0;
            int? selectcount = 0;

            PXResultset<NMPayableCheck> set =
                PXSelect<NMPayableCheck,
            Where<NMPayableCheck.status, Equal<NMStringList.NMAPCheckStatus.confirm>,
                And<NMPayableCheck.sendCashierID, IsNull,
                    And<NMPayableCheck.depositDate, IsNull,
                    And<NMPayableCheck.selected, Equal<True>>>>>>.Select(this);
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
            PXUIFieldAttribute.SetReadOnly<NMPayableCheck.receiver>(CheckDetails.Cache, row, false);
            CheckDetails.AllowDelete = false;
            CheckDetails.AllowInsert = false;
        }

        public void sendMethod()
        {
            NMAPCheckSendFilter filter = CheckFilters.Current;
            PXLongOperation.StartOperation(this, delegate ()
            {
                foreach (NMPayableCheck check in CheckDetails.Select())
                {
                    if (check.Selected == true)
                    {
                        //以下資訊更新欄位
                        check.SendCashierID = filter.SendCashierID;
                        check.DeliverDate = filter.DeliverDate;
                        check.DeliverMethod = filter.DeliverMethod;
                    }
                    CheckDetails.Update(check);
                }
                base.Persist();
            });
        }

        private Contact GetContact(int? vendorID)
        {
            return PXSelect<Contact,
                Where<Contact.bAccountID, Equal<Required<Contact.bAccountID>>>>
                .Select(this, vendorID);
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
    }




    [Serializable]
    public class NMAPCheckSendFilter : IBqlTable
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

        //Etd Deposit Date
        #region EtdDepositDateFrom
        [PXDate()]
        [PXUIField(DisplayName = "Etd Deposit Date From")]
        public virtual DateTime? EtdDepositDateFrom { get; set; }
        public abstract class etdDepositDateFrom : PX.Data.BQL.BqlDateTime.Field<etdDepositDateFrom> { }
        #endregion

        #region EtdDepositDateTo
        [PXDate()]
        [PXUIField(DisplayName = "Etd Deposit Date To")]
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


        //For Insert  Data
        #region SendCashierID
        [PXInt()]
        [PXUIField(DisplayName = "Send Cashier ID", Required = true)]
        [PXEPEmployeeSelector]
        [PXDefault(typeof(Search<EPEmployee.bAccountID,
        Where<EPEmployee.userID, Equal<Current<AccessInfo.userID>>>>),
            PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual int? SendCashierID { get; set; }
        public abstract class sendCashierID : PX.Data.BQL.BqlInt.Field<sendCashierID> { }
        #endregion

        #region DeliverMethod
        [PXInt()]
        [PXUIField(DisplayName = "Deliver Method", Required = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [NMStringList.NMAPDeliverMethod]
        public virtual int? DeliverMethod { get; set; }
        public abstract class deliverMethod : PX.Data.BQL.BqlInt.Field<deliverMethod> { }
        #endregion

        #region DeliverDate
        [PXDate()]
        [PXUIField(DisplayName = "Deliver Date", Required = true)]
        [PXDefault(typeof(AccessInfo.businessDate),
            PersistingCheck = PXPersistingCheck.NullOrBlank)]

        public virtual DateTime? DeliverDate { get; set; }
        public abstract class deliverDate : PX.Data.BQL.BqlDateTime.Field<deliverDate> { }
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
