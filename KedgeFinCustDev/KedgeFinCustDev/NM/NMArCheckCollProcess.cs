using System;
using PX.Data;
using NM.DAC;
using PX.Objects.CR;
using PX.Objects.EP;
using System.Collections;
using PX.Objects.AR;
using RC.Util;
using NM.Util;
using PX.Objects.PM;
using PS.DAC;
using KG.Util;

namespace NM
{
    public class NMArCheckCollProcess : PXGraph<NMArCheckCollProcess>
    {
        //權限設定開關
        public NMArCheckCollProcess()
        {
            if (!RCFeaturesSetUtil.IsActive(this, RCFeaturesSetProperties.NOTES_RECEIVABLE))
            {
                RCFeaturesSetUtil.BackToHomePage();
            }

        }

        #region ToolBar Action
        //public PXSave<NMCollectionCheckFilter> Save;
        public PXCancel<NMCollectionCheckFilter> Cancel;

        #region 託收
        public PXAction<NMCollectionCheckFilter> CollectionProcessAction;
        [PXButton(CommitChanges = true, Tooltip = "託收可勾選多筆支票")]
        [PXUIField(DisplayName = "託收")]
        protected IEnumerable collectionProcessAction(PXAdapter adapter)
        {
            NMCollectionCheckFilter filter = CheckFilters.Current;
            PXResultset<NMReceivableCheck> set =
                PXSelect<NMReceivableCheck,
            Where<NMReceivableCheck.status, Equal<NMStringList.NMARCheckStatus.receive>,
            And<NMReceivableCheck.selected, Equal<True>>>>.Select(this);
            if (set.Count != 0)
            {
                //2020/11/06 CollBankAccountID改為非必填
                if (filter.CollCashierID == null || filter.CollCheckDate == null)
                {
                    string errormsg = "請填妥以下欄位!";
                    /*if(filter.CollBankAccountID == null)
                    {
                        CheckFilters.Cache.RaiseExceptionHandling<NMCollectionCheckFilter.collBankAccountID>(
                                       filter, filter.CollBankAccountID,
                                               new PXSetPropertyException("此欄位為必填!", PXErrorLevel.Error));
                        //errormsg = errormsg + "託收銀行/";
                    }*/
                    if (filter.CollCashierID == null)
                    {
                        CheckFilters.Cache.RaiseExceptionHandling<NMCollectionCheckFilter.collCashierID>(
                                       filter, filter.CollCashierID,
                                               new PXSetPropertyException("此欄位為必填!", PXErrorLevel.Error));
                        //errormsg = errormsg + "託收作業人/";
                    }
                    if (filter.CollCheckDate == null)
                    {
                        CheckFilters.Cache.RaiseExceptionHandling<NMCollectionCheckFilter.collCheckDate>(
                                       filter, filter.CollCheckDate,
                                               new PXSetPropertyException("此欄位為必填!", PXErrorLevel.Error));
                        //errormsg = errormsg + "託收日期";
                    }


                    throw new Exception(errormsg);
                }
                collectionMethod();
            }
            else
            {
                throw new Exception("請勾選最少一筆資料!");
            }
            return adapter.Get();
        }


        #endregion

        #endregion

        #region Select and Filter
        public PXFilter<NMCollectionCheckFilter> CheckFilters;

        //只有已收票的單子可以顯示在畫面
        public PXSelect<NMReceivableCheck,
            Where<NMReceivableCheck.status, Equal<NMStringList.NMARCheckStatus.receive>>> CheckDetails;

        protected virtual IEnumerable checkDetails()
        {
            NMCollectionCheckFilter filter = CheckFilters.Current;
            PXSelect<NMReceivableCheck,
            Where<NMReceivableCheck.status, Equal<NMStringList.NMARCheckStatus.receive>>> query =
            new PXSelect<NMReceivableCheck,
            Where<NMReceivableCheck.status, Equal<NMStringList.NMARCheckStatus.receive>>>(this);

            //query
            if (filter.CustomerID != null)
                query.WhereAnd<Where<NMReceivableCheck.customerID,
                    Equal<Current<NMCollectionCheckFilter.customerID>>>>();
            if (filter.CheckCashierID != null)
                query.WhereAnd<Where<NMReceivableCheck.checkCashierID,
                    Equal<Current<NMCollectionCheckFilter.checkCashierID>>>>();
            if (filter.ProjectID != null)
                query.WhereAnd<Where<NMReceivableCheck.projectID,
                    Equal<Current<NMCollectionCheckFilter.projectID>>>>();
            if (filter.DueDate != null)
                query.WhereAnd<Where<NMReceivableCheck.dueDate,
                    LessEqual<Current<NMCollectionCheckFilter.dueDate>>,
                    Or<NMReceivableCheck.dueDate, IsNull>>>();

            //設Default
            foreach (NMReceivableCheck check in query.Select())
            {
                if (filter.CollBankAccountID != null)
                {
                    check.CollBankAccountID = filter.CollBankAccountID;
                }
                else if (check.CollBankAccountID == null)
                {
                    //PSPaymentSlipDetails slipDetails = GetSlipDetails(check.PayRefNbr);
                    check.CollBankAccountID = check.EtdCollBankAccountID;
                }
                yield return check;
            }
            //return query.Select();
        }
        #endregion

        #region Event

        protected virtual void NMReceivableCheck_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            NMReceivableCheck row = (NMReceivableCheck)e.Row;
            if (row == null) return;
            setReadOnly();
        }

        protected virtual void NMReceivableCheck_Selected_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            NMReceivableCheck row = (NMReceivableCheck)e.Row;
            NMCollectionCheckFilter header = CheckFilters.Current;
            if (row == null) return;

            decimal? selectbaseAmount = 0;
            decimal? selectoriAmount = 0;
            int? selectcount = 0;

            PXResultset<NMReceivableCheck> set =
                PXSelect<NMReceivableCheck,
                Where<NMReceivableCheck.status, Equal<NMStringList.NMARCheckStatus.receive>,
                And<NMReceivableCheck.selected, Equal<True>>>>.Select(this);
            foreach (NMReceivableCheck receivableCheck in set)
            {
                selectoriAmount = receivableCheck.OriCuryAmount + selectoriAmount;
                selectbaseAmount = receivableCheck.OriCuryAmount + selectbaseAmount;
            }
            selectcount = set.Count;

            header.SelectedOriCuryAmount = selectoriAmount;
            header.SelectedBaseCuryAmount = selectbaseAmount;
            header.SelectedCount = selectcount;



        }

        protected virtual void NMCollectionCheckFilter_CollBankAccountID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            NMCollectionCheckFilter row = (NMCollectionCheckFilter)e.Row;
            if (row == null) return;
            if (row.CollBankAccountID != null)
            {
                NMBankAccount bankAccount = getNMBank(row.CollBankAccountID);
                row.BankAccount = bankAccount.BankAccount;
            }
            else
                row.BankAccount = null;
        }
        #endregion



        #region Method
        public void setReadOnly()
        {
            NMReceivableCheck row = CheckDetails.Current;
            PXUIFieldAttribute.SetReadOnly(CheckDetails.Cache, row, true);
            PXUIFieldAttribute.SetReadOnly<NMReceivableCheck.selected>(CheckDetails.Cache, row, false);
            PXUIFieldAttribute.SetReadOnly<NMReceivableCheck.collBankAccountID>(CheckDetails.Cache, row, false);
            CheckDetails.AllowDelete = false;
            CheckDetails.AllowInsert = false;
        }

        public void collectionMethod()
        {
            PXLongOperation.StartOperation(this, delegate ()
             {
                 using (PXTransactionScope ts = new PXTransactionScope())
                 {

                     foreach (NMReceivableCheck check in CheckDetails.Select())
                     {
                         NMCollectionCheckFilter filter = CheckFilters.Current;
                         if (check.Selected == true)
                         {

                            //2020/11/06 Mantis:0011792
                            PSPaymentSlipDetails slipDetails = GetSlipDetails(check.PayRefNbr);

                            //狀態改為已託收
                            check.Status = 2;
                            //以下資訊更新欄位
                            //check.CollBankAccountID = slipDetails.BankAccountID;
                             check.CollCashierID = filter.CollCashierID;
                             check.CollCheckDate = filter.CollCheckDate;
                             check.CollBatchNbr = NMVoucherUtil.CreateARVoucher(NMStringList.NMARVoucher.COLLECTION, check, GLStageCode.NMRCollectR3);
                         }
                         CheckDetails.Update(check);
                         base.Persist();
                     }

                     ts.Complete();


                 }

             });

        }

        public PXSelect<ARPayment> ARPayments;
        public PXSelectJoin<ARPayment, LeftJoinSingleTable<Customer, On<Customer.bAccountID, Equal<ARPayment.customerID>>>,
            Where<ARPayment.docType, Equal<Optional<ARPayment.docType>>,
            And<Where<Customer.bAccountID, IsNull, Or<Match<Customer, Current<AccessInfo.userName>>>>>>> Document;

        public NMBankAccount getNMBank(int? BankAccountID)
        {
            return PXSelect<NMBankAccount,
                Where<NMBankAccount.bankAccountID, Equal<Required<NMBankAccount.bankAccountID>>>>
                .Select(this, BankAccountID);
        }

        public PSPaymentSlipDetails GetSlipDetails(string RefNbr)
        {
            return PXSelect<PSPaymentSlipDetails,
                            Where<PSPaymentSlipDetails.arPaymentRefNbr, Equal<Required<NMReceivableCheck.payRefNbr>>>>
                            .Select(this, RefNbr);
        }
        #endregion
    }

    [Serializable]
    public class NMCollectionCheckFilter : IBqlTable
    {
        //For Search
        #region CustomerID
        [PXInt()]
        [PXUIField(DisplayName = "Customer ID")]
        [CustomerActive(Visibility = PXUIVisibility.SelectorVisible,
            DescriptionField = typeof(Customer.acctName),
            Filterable = true, TabOrder = 2)]
        public virtual int? CustomerID { get; set; }
        public abstract class customerID : PX.Data.BQL.BqlInt.Field<customerID> { }
        #endregion

        #region CheckCashierID
        [PXInt()]
        [PXUIField(DisplayName = "Check Cashier ID")]
        [PXEPEmployeeSelector]
        public virtual int? CheckCashierID { get; set; }
        public abstract class checkCashierID : PX.Data.BQL.BqlInt.Field<checkCashierID> { }
        #endregion

        #region DueDate
        [PXDate()]
        [PXUIField(DisplayName = "Due Date")]

        public virtual DateTime? DueDate { get; set; }
        public abstract class dueDate : PX.Data.BQL.BqlDateTime.Field<dueDate> { }
        #endregion

        //ProjectID
        #region ProjectID
        [PXInt()]
        [PXUIField(DisplayName = "Project ID")]
        [ProjectBaseAttribute()]
        public virtual int? ProjectID { get; set; }
        public abstract class projectID : PX.Data.BQL.BqlInt.Field<projectID> { }
        #endregion

        //For Insert Collection Data
        #region CollBankAccountID
        [PXInt()]
        [PXUIField(DisplayName = "Coll Bank Account ID")]
        [NMBankAccount()]
        public virtual int? CollBankAccountID { get; set; }
        public abstract class collBankAccountID : PX.Data.BQL.BqlInt.Field<collBankAccountID> { }
        #endregion

        #region BankAccount
        [PXString(50, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Bank Account", IsReadOnly = true)]

        public virtual string BankAccount { get; set; }
        public abstract class bankAccount : PX.Data.BQL.BqlString.Field<bankAccount> { }
        #endregion

        #region CollCashierID
        [PXInt()]
        [PXUIField(DisplayName = "Coll Cashier ID", Required = true)]
        [PXEPEmployeeSelector]
        [PXDefault(typeof(Search<EPEmployee.bAccountID,
            Where<EPEmployee.userID, Equal<Current<AccessInfo.userID>>>>))]
        public virtual int? CollCashierID { get; set; }
        public abstract class collCashierID : PX.Data.BQL.BqlInt.Field<collCashierID> { }
        #endregion

        #region CollCheckDate
        [PXDate()]
        [PXUIField(DisplayName = "Coll Check Date", Required = true)]
        [PXDefault(typeof(AccessInfo.businessDate))]
        public virtual DateTime? CollCheckDate { get; set; }
        public abstract class collCheckDate : PX.Data.BQL.BqlDateTime.Field<collCheckDate> { }
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