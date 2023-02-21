using System;
using PX.Data;
using NM.DAC;
using PX.Objects.CR;
using PX.Objects.EP;
using System.Collections;
using RC.Util;
using NM.Util;
using PX.Objects.AR;
using KG.Util;

namespace NM
{
    public class NMArCheckCashProcess : PXGraph<NMArCheckCashProcess>
    {
        //權限設定開關
        public NMArCheckCashProcess()
        {
            if (!RCFeaturesSetUtil.IsActive(this, RCFeaturesSetProperties.NOTES_RECEIVABLE))
            {
                RCFeaturesSetUtil.BackToHomePage();
            }

        }

        #region ToolBar Action
        //public PXSave<NMCashCheckFilter> Save;
        public PXCancel<NMCashCheckFilter> Cancel;

        #region 兌現
        public PXAction<NMCashCheckFilter> CashProcessAction;
        [PXButton(CommitChanges = true, Tooltip = "兌現可勾選多筆支票")]
        [PXUIField(DisplayName = "兌現")]
        protected IEnumerable cashProcessAction(PXAdapter adapter)
        {
            NMReceivableCheck receivableCheck = CheckDetails.Current;
            NMCashCheckFilter filter = CheckFilters.Current;
            PXResultset<NMReceivableCheck> set =
                PXSelect<NMReceivableCheck,
            Where<NMReceivableCheck.status, Equal<NMStringList.NMARCheckStatus.collection>,
            And<NMReceivableCheck.selected, Equal<True>>>>.Select(this);
            if (set.Count != 0)
            {
                if (filter.CashCashierID == null || filter.DepositDate == null )
                {
                    string errormsg = "請填妥以下欄位!";
                    if (filter.CashCashierID == null)
                    {
                        CheckFilters.Cache.RaiseExceptionHandling<NMCashCheckFilter.cashCashierID>(
                                       filter, filter.CashCashierID,
                                               new PXSetPropertyException("此欄位為必填!", PXErrorLevel.Error));
                        //errormsg = errormsg + "兌現作業人/";
                    }
                    if (filter.DepositDate == null)
                    {
                        CheckFilters.Cache.RaiseExceptionHandling<NMCashCheckFilter.depositDate>(
                                       filter, filter.DepositDate,
                                               new PXSetPropertyException("此欄位為必填!", PXErrorLevel.Error));
                        //errormsg = errormsg + "兌現日期/";
                    }

                    throw new Exception(errormsg);
                }
               
            }
            else
            {
                throw new Exception("請勾選最少一筆資料!");
            }
            cashMethod();
            return adapter.Get();
        }


        #endregion

        #endregion

        #region Select and Filter
        public PXFilter<NMCashCheckFilter> CheckFilters;

        //只有已託收的單子可以顯示畫面
        public PXSelect<NMReceivableCheck,
            Where<NMReceivableCheck.status, Equal<NMStringList.NMARCheckStatus.collection>>> CheckDetails;

        protected virtual IEnumerable checkDetails()
        {
            NMCashCheckFilter filter = CheckFilters.Current;
            PXSelect<NMReceivableCheck,
            Where<NMReceivableCheck.status, Equal<NMStringList.NMARCheckStatus.collection>>> query =
            new PXSelect<NMReceivableCheck,
            Where<NMReceivableCheck.status, Equal<NMStringList.NMARCheckStatus.collection>>>(this);

            //query
            if (filter.CollBankAccountID != null)
                query.WhereAnd<Where<NMReceivableCheck.collBankAccountID,
                    Equal<Current<NMCashCheckFilter.collBankAccountID>>>>();
            if (filter.CollCashierID != null)
                query.WhereAnd<Where<NMReceivableCheck.collCashierID,
                    Equal<Current<NMCashCheckFilter.collCashierID>>>>();
            if (filter.EtdDepositDate != null)
                query.WhereAnd<Where<NMReceivableCheck.etdDepositDate,
                    LessEqual<Current<NMCashCheckFilter.etdDepositDate>>>>();

            return query.Select();
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
            NMCashCheckFilter header = CheckFilters.Current;
            if (row == null) return;

            decimal? selectbaseAmount = 0;
            decimal? selectoriAmount = 0;
            int? selectcount = 0;

            PXResultset<NMReceivableCheck> set =
                PXSelect<NMReceivableCheck,
                Where<NMReceivableCheck.status, Equal<NMStringList.NMARCheckStatus.collection>,
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
        #endregion

        #region Method
        public void setReadOnly()
        {
            NMReceivableCheck row = CheckDetails.Current;
            PXUIFieldAttribute.SetReadOnly(CheckDetails.Cache, row, true);
            PXUIFieldAttribute.SetReadOnly<NMReceivableCheck.selected>(CheckDetails.Cache, row, false);
            CheckDetails.AllowDelete = false;
            CheckDetails.AllowInsert = false;
        }

        public void cashMethod()
        {
            NMCashCheckFilter filter = CheckFilters.Current;

            PXLongOperation.StartOperation(this, delegate ()
            {
                using (PXTransactionScope ts = new PXTransactionScope())
                {
                    foreach (NMReceivableCheck check in CheckDetails.Select())
                    {
                        if (check.Selected == true)
                        {
                            //狀態改為已兌現
                            check.Status = 3;
                            //以下資訊更新欄位
                            check.CashCashierID = filter.CashCashierID;
                            check.DepositDate = filter.DepositDate;
                            check.CashBatchNbr = NMVoucherUtil.CreateARVoucher(NMStringList.NMARVoucher.CASH, check, GLStageCode.NMRCashR4);
                            //2020/09/21 Delete Mantis:11697
                            //releaseARPaymentMethod(check.PayRefNbr);
                        }
                        CheckDetails.Update(check);
                    }
                    base.Persist();
                    ts.Complete();
                }
            });
            
        }

        //2020/09/04 ADD 抓出對應的ARPayment單,將他過帳
        public void releaseARPaymentMethod(string PayRefNbr)
        {
            ARPaymentEntry entry = PXGraph.CreateInstance<ARPaymentEntry>();
            
            ARPayment payment = PXSelect<ARPayment,
                Where<ARPayment.refNbr, Equal<Required<NMReceivableCheck.payRefNbr>>>>
                .Select(this, PayRefNbr);

            entry.Document.Update(payment);
            entry.release.Press();

        }

        #endregion

        #region CacheAttached
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXUIField(DisplayName = "Customer Name")]
        protected virtual void _(Events.CacheAttached<BAccountR.acctName> e) { }
        #endregion

        #region Filter DAC
        [Serializable]
        public class NMCashCheckFilter : IBqlTable
        {
            //For Search
            #region CollBankAccountID
            [PXInt()]
            [PXUIField(DisplayName = "Coll Bank Account ID")]
            [NMBankAccount()]
            public virtual int? CollBankAccountID { get; set; }
            public abstract class collBankAccountID : PX.Data.BQL.BqlInt.Field<collBankAccountID> { }
            #endregion

            #region CollCashierID
            [PXInt()]
            [PXUIField(DisplayName = "Coll Cashier ID")]
            [PXEPEmployeeSelector]
            public virtual int? CollCashierID { get; set; }
            public abstract class collCashierID : PX.Data.BQL.BqlInt.Field<collCashierID> { }
            #endregion

            #region EtdDepositDate
            [PXDate()]
            [PXUIField(DisplayName = "Etd Deposit Date")]
            public virtual DateTime? EtdDepositDate { get; set; }
            public abstract class etdDepositDate : PX.Data.BQL.BqlDateTime.Field<etdDepositDate> { }
            #endregion

            //For Insert Cash Data

            #region CashCashierID
            [PXInt()]
            [PXUIField(DisplayName = "Cash Cashier ID",Required =true)]
            [PXEPEmployeeSelector]
            [PXDefault(typeof(Search<EPEmployee.bAccountID,
            Where<EPEmployee.userID, Equal<Current<AccessInfo.userID>>>>))]
            public virtual int? CashCashierID { get; set; }
            public abstract class cashCashierID : PX.Data.BQL.BqlInt.Field<cashCashierID> { }
            #endregion

            #region DepositDate
            [PXDate()]
            [PXUIField(DisplayName = "Deposit Date",Required =true)]
            [PXDefault(typeof(AccessInfo.businessDate))]
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