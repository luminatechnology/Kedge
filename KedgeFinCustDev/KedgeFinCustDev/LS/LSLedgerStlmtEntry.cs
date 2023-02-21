using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.IN;
using PX.Objects.GL;
using KedgeFinCustDev.LS.DAC;

namespace KedgeFinCustDev.LS
{
    public class LSLedgerStlmtEntry : PXGraph<LSLedgerStlmtEntry>
    {
        #region Constant String Classes & Properties
        public const string steldAmtExceedRmngAmt = "Settle Amount Can't Be Greater Than Remaining Amount.";

        public const string ZZ_UOM = "ZZ";
        public const string YY_UOM = "YY";

        public class ZZUOM : PX.Data.BQL.BqlString.Constant<ZZUOM>
        {
            public ZZUOM() : base(ZZ_UOM) { }
        }

        public class YYUOM : PX.Data.BQL.BqlString.Constant<YYUOM>
        {
            public YYUOM() : base(YY_UOM) { }
        }
        #endregion

        #region Selects & Features
        public PXCancel<LedgerTranFilter> Cancel;
        public PXFilter<LedgerTranFilter> Filter;
        public SelectFrom<LSLedgerSettlement>.View LedgerStlmt;
        public SelectFrom<GLTran>.InnerJoin<Ledger>.On<Ledger.ledgerID.IsEqual<GLTran.ledgerID>
                                                       .And<Ledger.balanceType.IsEqual<LedgerBalanceType.actual>>>
                                 .Where<GLTran.accountID.IsEqual<LedgerTranFilter.stlmtAcctID.FromCurrent>
                                        .And<GLTran.branchID.IsEqual<LedgerTranFilter.branchID.FromCurrent>>
                                            .And<GLTran.released.IsEqual<True>>
                                                .And<GLTran.posted.IsEqual<True>
                                                     .And<Where<GLTran.uOM.IsNotEqual<ZZUOM>.Or<GLTran.uOM.IsNull>>>
                                                          .And<Where2<Where<Current<LedgerTranFilter.stlmtAcctType>, Equal<AccountType.asset>,
                                                                            And<GLTran.curyDebitAmt, Greater<PX.Objects.CS.decimal0>>>,
                                                                      Or<Current<LedgerTranFilter.stlmtAcctType>, Equal<AccountType.liability>,
                                                                         And<GLTran.curyCreditAmt, Greater<PX.Objects.CS.decimal0>>>>>>>.View GLTranDebit;
        public SelectFrom<GLTran>.InnerJoin<Ledger>.On<Ledger.ledgerID.IsEqual<GLTran.ledgerID>
                                                       .And<Ledger.balanceType.IsEqual<LedgerBalanceType.actual>>>
                                 .Where<GLTran.accountID.IsEqual<LedgerTranFilter.stlmtAcctID.FromCurrent>
                                        .And<GLTran.branchID.IsEqual<LedgerTranFilter.branchID.FromCurrent>>
                                            .And<GLTran.released.IsEqual<True>>
                                                .And<GLTran.posted.IsEqual<True>
                                                     .And<Where<GLTran.uOM.IsNotEqual<ZZUOM>.Or<GLTran.uOM.IsNull>>>
                                                          .And<Where2<Where<Current<LedgerTranFilter.stlmtAcctType>, Equal<AccountType.asset>,
                                                                            And<GLTran.curyCreditAmt, Greater<PX.Objects.CS.decimal0>>>,
                                                                      Or<Current<LedgerTranFilter.stlmtAcctType>, Equal<AccountType.liability>,
                                                                         And<GLTran.curyDebitAmt, Greater<PX.Objects.CS.decimal0>>>>>>>.View GLTranCredit;
        #endregion

        #region Delegate Data View
        public IEnumerable gLTranDebit()
        {
            LedgerTranFilter filter = Filter.Current;

            PXView view = new PXView(this, false, GLTranDebit.View.BqlSelect);

            List<PXResult<LSLedgerSettlement>> settlements = SelectFrom<LSLedgerSettlement>.Where<LSLedgerSettlement.branchID.IsEqual<@P.AsInt>
                                                                                                  .And<LSLedgerSettlement.accountID.IsEqual<@P.AsInt>>>
                                                                                           .AggregateTo<GroupBy<LSLedgerSettlement.module,
                                                                                                                GroupBy<LSLedgerSettlement.batchNbr,
                                                                                                                        GroupBy<LSLedgerSettlement.lineNbr,
                                                                                                                                Sum<LSLedgerSettlement.settledDebitAmt,
                                                                                                                                    Sum<LSLedgerSettlement.settledCreditAmt>>>>>>
                                                                                           .View.Select(this, filter?.BranchID, filter?.StlmtAcctID).ToList<PXResult<LSLedgerSettlement>>();

            foreach (GLTran row in view.SelectMulti().RowCast<GLTran>())
            {
                LSLedgerSettlement settlement = settlements.Find(f => f.GetItem<LSLedgerSettlement>().Module == row.Module && 
                                                                      f.GetItem<LSLedgerSettlement>().BatchNbr == row.BatchNbr && 
                                                                      f.GetItem<LSLedgerSettlement>().LineNbr == row.LineNbr);

                GLTranExt tranExt = row.GetExtension<GLTranExt>();

                if (filter.StlmtAcctType == AccountType.Asset)
                {
                    tranExt.UsrRmngDebitAmt = row.DebitAmt - (settlement?.SettledDebitAmt ?? 0m);
                    tranExt.UsrSetldDebitAmt = tranExt.UsrRmngDebitAmt == 0m ? row.DebitAmt : (tranExt.UsrSetldDebitAmt < tranExt.UsrRmngDebitAmt) && tranExt.UsrSetldDebitAmt > 0m ? tranExt.UsrSetldDebitAmt : tranExt.UsrRmngDebitAmt;
                }
                else
                {
                    tranExt.UsrRmngCreditAmt = row.CreditAmt - (settlement?.SettledCreditAmt ?? 0m);
                    tranExt.UsrSetldCreditAmt = tranExt.UsrRmngCreditAmt == 0m ? row.CreditAmt : (tranExt.UsrSetldCreditAmt < tranExt.UsrRmngCreditAmt) && tranExt.UsrSetldCreditAmt > 0m ? tranExt.UsrSetldCreditAmt : tranExt.UsrRmngCreditAmt;
                }

                if ((tranExt.UsrRmngCreditAmt ?? 0m) != 0m || (tranExt.UsrRmngDebitAmt ?? 0m) != 0m)
                {
                    yield return row;
                }
            }
        }

        public IEnumerable gLTranCredit()
        {
            LedgerTranFilter filter = Filter.Current;

            PXView view = new PXView(this, false, GLTranCredit.View.BqlSelect);

            List<PXResult<LSLedgerSettlement>> settlements = SelectFrom<LSLedgerSettlement>.Where<LSLedgerSettlement.branchID.IsEqual<@P.AsInt>
                                                                                                  .And<LSLedgerSettlement.accountID.IsEqual<@P.AsInt>>>
                                                                                           .AggregateTo<GroupBy<LSLedgerSettlement.module,
                                                                                                                GroupBy<LSLedgerSettlement.batchNbr,
                                                                                                                        GroupBy<LSLedgerSettlement.lineNbr,
                                                                                                                                Sum<LSLedgerSettlement.settledCreditAmt,
                                                                                                                                    Sum<LSLedgerSettlement.settledDebitAmt>>>>>>
                                                                                           .View.Select(this, filter?.BranchID, filter?.StlmtAcctID).ToList<PXResult<LSLedgerSettlement>>();

            foreach (GLTran row in view.SelectMulti().RowCast<GLTran>())
            {
                LSLedgerSettlement settlement = settlements.Find(f => f.GetItem<LSLedgerSettlement>().Module == row.Module && 
                                                                      f.GetItem<LSLedgerSettlement>().BatchNbr == row.BatchNbr && 
                                                                      f.GetItem<LSLedgerSettlement>().LineNbr == row.LineNbr);

                GLTranExt tranExt = row.GetExtension<GLTranExt>();

                if (filter.StlmtAcctType == AccountType.Asset)
                {
                    tranExt.UsrRmngCreditAmt = row.CreditAmt - (settlement?.SettledCreditAmt ?? 0m);
                    tranExt.UsrSetldCreditAmt = tranExt.UsrRmngCreditAmt == 0m ? row.CreditAmt : (tranExt.UsrSetldCreditAmt < tranExt.UsrRmngCreditAmt) && tranExt.UsrSetldCreditAmt > 0m ? tranExt.UsrSetldCreditAmt : tranExt.UsrRmngCreditAmt;
                }
                else
                {
                    tranExt.UsrRmngDebitAmt = row.DebitAmt - (settlement?.SettledDebitAmt ?? 0m);
                    tranExt.UsrSetldDebitAmt = tranExt.UsrRmngDebitAmt == 0m ? row.DebitAmt : (tranExt.UsrSetldDebitAmt < tranExt.UsrRmngDebitAmt) && tranExt.UsrSetldDebitAmt > 0m ? tranExt.UsrSetldDebitAmt : tranExt.UsrRmngDebitAmt;
                }

                if ((tranExt.UsrRmngCreditAmt ?? 0m) != 0m || (tranExt.UsrRmngDebitAmt ?? 0m) != 0m)
                {
                    yield return row;
                }
            }
        }
        #endregion

        #region Actions
        public PXAction<LedgerTranFilter> Match;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Match", MapEnableRights = PXCacheRights.Select)]
        public IEnumerable match(PXAdapter adapter)
        {
            PXLongOperation.StartOperation(this, delegate ()
            {
                CreateLedgerSettlement();
            });

            return adapter.Get();
        }

        public PXAction<LedgerTranFilter> ToggleSource;
        [PXButton()]
        [PXUIField(DisplayName = "Toggle Source Selection", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        public IEnumerable toggleSource(PXAdapter adapter)
        {
            bool hasSelected = !GLTranDebit.View.SelectMulti().ToList().RowCast<GLTran>().First().Selected.GetValueOrDefault();

            foreach (GLTran tran in GLTranDebit.View.SelectMulti().RowCast<GLTran>())
            {
                tran.Selected = hasSelected;

                GLTranDebit.Cache.Update(tran);
            }

            return adapter.Get();
        }

        public PXAction<LedgerTranFilter> ToggleSettlement;
        [PXButton()]
        [PXUIField(DisplayName = "Toggle Settlement Selection", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        public IEnumerable toggleSettlement(PXAdapter adapter)
        {
            bool hasSelected = !GLTranCredit.View.SelectMulti().ToList().RowCast<GLTran>().First().Selected.GetValueOrDefault();

            foreach (GLTran tran in GLTranCredit.View.SelectMulti().RowCast<GLTran>())
            {
                tran.Selected = hasSelected;

                GLTranCredit.Cache.Update(tran);
            }

            return adapter.Get();
        }
        #endregion

        #region Cache Attached
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXUIField(DisplayName = "Selected", Visible = true)]
        protected void _(Events.CacheAttached<GLTran.selected> e) { }

        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXUIField(DisplayName = "Line Nbr.", Visibility = PXUIVisibility.Visible, Visible = true, Enabled = false)]
        protected void _(Events.CacheAttached<GLTran.lineNbr> e) { }

        [PXRemoveBaseAttribute(typeof(PX.Objects.IN.InventoryAttribute))]
        [PXDBInt]
        [PXUIField(DisplayName = "Inventory ID")]
        [PXDimensionSelector(InventoryAttribute.DimensionName,
                             typeof(Search<InventoryItem.inventoryID>),
                             typeof(InventoryItem.inventoryCD))]
        protected void _(Events.CacheAttached<GLTran.inventoryID> e) { }

        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXUIField(DisplayName = "Batch Number", Visibility = PXUIVisibility.Visible, Visible = true)]
        protected void _(Events.CacheAttached<GLTran.batchNbr> e) { }

        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXUIField(DisplayName = "Customer/Vendor", Enabled = false, Visible = true)]
        protected void _(Events.CacheAttached<GLTran.referenceID> e) { }

        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXUIField(DisplayName = "Tran Cur. Debit Amt.", Enabled = false)]
        protected void _(Events.CacheAttached<GLTran.curyDebitAmt> e) { }

        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXUIField(DisplayName = "Tran Cur. Credit Amt.", Enabled = false)]
        protected void _(Events.CacheAttached<GLTran.curyCreditAmt> e) { }
        #endregion

        #region Event Handlers

        #region LedgerTranFilter
        protected virtual void _(Events.RowSelected<LedgerTranFilter> e)
        {
            Match.SetEnabled(e.Row.BalanceAmt == decimal.Zero);

            PXUIFieldAttribute.SetEnabled<GLTran.batchNbr>(GLTranDebit.Cache, null, false);
            PXUIFieldAttribute.SetEnabled<GLTran.branchID>(GLTranDebit.Cache, null, false);
            PXUIFieldAttribute.SetEnabled<GLTran.subID>(GLTranDebit.Cache, null, false);
            PXUIFieldAttribute.SetEnabled<GLTran.refNbr>(GLTranDebit.Cache, null, false);
            PXUIFieldAttribute.SetEnabled<GLTran.curyDebitAmt>(GLTranDebit.Cache, null, false);
            PXUIFieldAttribute.SetEnabled<GLTran.curyCreditAmt>(GLTranDebit.Cache, null, false);
            PXUIFieldAttribute.SetEnabled<GLTran.tranDesc>(GLTranDebit.Cache, null, false);
            PXUIFieldAttribute.SetEnabled<GLTran.projectID>(GLTranDebit.Cache, null, false);
            PXUIFieldAttribute.SetEnabled<GLTran.taskID>(GLTranDebit.Cache, null, false);
            PXUIFieldAttribute.SetEnabled<GLTran.costCodeID>(GLTranDebit.Cache, null, false);
        }

        protected virtual void _(Events.FieldUpdated<LedgerTranFilter.stlmtAcctID> e)
        {
            // Reset the unbound fields after changing the filter for the parameter Account.
            (e.Row as LedgerTranFilter).BalanceAmt = null;
        }
        #endregion

        #region GLTran
        protected virtual void _(Events.RowSelected<GLTran> e)
        {
            if (e.Row != null)
            {
                PXUIFieldAttribute.SetEnabled<GLTranExt.usrSetldDebitAmt>(e.Cache, e.Row, e.Row.DebitAmt != 0m);
                PXUIFieldAttribute.SetEnabled<GLTranExt.usrSetldCreditAmt>(e.Cache, e.Row, e.Row.CreditAmt != 0m);
            }

            ToggleSource.SetEnabled(e.Row != null);
            ToggleSettlement.SetEnabled(e.Row != null);
        }

        protected virtual void _(Events.FieldUpdated<GLTran.selected> e)
        {
            var row = e.Row as GLTran;

            if (row != null)
            {
                GLTranExt tranExt = PXCacheEx.GetExtension<GLTranExt>(row);

                if ((bool)e.NewValue == true)
                {
                    Filter.Current.BalanceAmt = (Filter.Current.BalanceAmt ?? 0m) + (tranExt.UsrSetldDebitAmt ?? 0m) - (tranExt.UsrSetldCreditAmt ?? 0m);
                }
                else
                {
                    Filter.Current.BalanceAmt = (Filter.Current.BalanceAmt ?? 0m) - (tranExt.UsrSetldDebitAmt ?? 0m) + (tranExt.UsrSetldCreditAmt ?? 0m);

                    //tranExt.UsrRmngDebitAmt = tranExt.UsrRmngCreditAmt = tranExt.UsrSetldDebitAmt = tranExt.UsrSetldCreditAmt = 0m;
                }
            }
        }

        protected virtual void _(Events.FieldUpdated<GLTranExt.usrSetldDebitAmt> e)
        {
            var row = e.Row as GLTran;

            decimal? calcAmt = 0m;

            foreach (GLTran tran in GLTranDebit.Cache.Updated)
            {
                if (tran.Selected == true)
                {
                    GLTranExt tranExt = tran.GetExtension<GLTranExt>();

                    ///<remarks> Because we need to update the unbound fields and check this field and then add verification in this event. </remarks>
                    if (tran.CuryDebitAmt > 0m && (decimal)e.NewValue > tranExt.UsrRmngDebitAmt &&
                        row.Module == tran.Module && row.BatchNbr == tran.BatchNbr && row.LineNbr == tran.LineNbr)
                    {
                        throw new PXSetPropertyException<GLTranExt.usrSetldDebitAmt>(steldAmtExceedRmngAmt);
                    }

                    calcAmt += (tranExt.UsrSetldDebitAmt ?? 0m) - (tranExt.UsrSetldCreditAmt ?? 0m);
                }
            }

            Filter.Current.BalanceAmt = calcAmt;
        }

        protected virtual void _(Events.FieldUpdated<GLTranExt.usrSetldCreditAmt> e)
        {
            var row = e.Row as GLTran;

            decimal? calcAmt = 0m;

            foreach (GLTran tran in GLTranCredit.Cache.Updated)
            {
                if (tran.Selected == true)
                {
                    GLTranExt tranExt = tran.GetExtension<GLTranExt>();

                    ///<remarks> Because we need to update the unbound fields and check this field and then add verification in this event. </remarks>
                    if (tran.CuryCreditAmt > 0m && (decimal)e.NewValue > tranExt.UsrRmngCreditAmt &&
                        row.Module == tran.Module && row.BatchNbr == tran.BatchNbr && row.LineNbr == tran.LineNbr)
                    {
                        throw new PXSetPropertyException<GLTranExt.usrSetldCreditAmt>(steldAmtExceedRmngAmt);
                    }

                    calcAmt += (tranExt.UsrSetldDebitAmt ?? 0m) - (tranExt.UsrSetldCreditAmt ?? 0m);
                }
            }

            Filter.Current.BalanceAmt = calcAmt;
        }
        #endregion

        #endregion

        #region Methods
        public virtual void CreateLedgerSettlement()
        {
            string stlmtNbr = DateTime.UtcNow.ToString("yyyyMMddhhmmss");

            foreach (GLTran tran in SelectFrom<GLTran>.InnerJoin<Ledger>.On<Ledger.ledgerID.IsEqual<GLTran.ledgerID>
                                                                            .And<Ledger.balanceType.IsEqual<LedgerBalanceType.actual>>>
                                                      .Where<GLTran.selected.IsEqual<True>
                                                             .And<GLTran.accountID.IsEqual<LedgerTranFilter.stlmtAcctID.FromCurrent>>
                                                                  .And<GLTran.branchID.IsEqual<LedgerTranFilter.branchID.FromCurrent>>
                                                                       .And<GLTran.released.IsEqual<True>>
                                                                            .And<GLTran.posted.IsEqual<True>>>.View.Select(this))
            {
                GLTranExt tranExt = PXCacheEx.GetExtension<GLTranExt>(tran);

                LSLedgerSettlement row = LedgerStlmt.Cache.CreateInstance() as LSLedgerSettlement;

                row.SettlementNbr = stlmtNbr;
                row.BranchID = tran.BranchID;
                row.LineNbr = tran.LineNbr;
                row.Module = tran.Module;
                row.BatchNbr = tran.BatchNbr;
                row.LedgerID = tran.LedgerID;
                row.AccountID = tran.AccountID;
                row.SubID = tran.SubID;
                row.OrigCreditAmt = tran.CreditAmt;
                row.OrigDebitAmt = tran.DebitAmt;
                row.SettledCreditAmt = tranExt.UsrSetldCreditAmt;
                row.SettledDebitAmt = tranExt.UsrSetldDebitAmt;
                row.TranDesc = tran.TranDesc;
                row.TranDate = tran.TranDate;
                row.RefNbr = tran.RefNbr;
                row.InventoryID = tran.InventoryID;
                row.ProjectID = tran.ProjectID;
                row.TaskID = tran.TaskID;
                row.CostCodeID = tran.CostCodeID;

                row = (LSLedgerSettlement)LedgerStlmt.Insert(row);

                GLTranDebit.Current = tran;

                decimal debit = tranExt.UsrRmngDebitAmt ?? 0m;
                decimal credit = tranExt.UsrRmngCreditAmt ?? 0m;

                UpdateGLTranUOM(GLTranDebit.Cache, (row.OrigCreditAmt + row.OrigDebitAmt == row.SettledCreditAmt + row.SettledDebitAmt || debit + credit == row.SettledCreditAmt + row.SettledDebitAmt) ? "ZZ" : "YY");
            }

            this.Actions.PressSave();
        }

        private LedgerStlmtKey GetKey(LSLedgerSettlement record)
        {
            return new LedgerStlmtKey(record.BranchID.Value, record.LineNbr.Value, record.Module, record.BatchNbr);
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// Update GLTran UOM to filter report (LS601000) data source.
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="uOM"></param>
        public static void UpdateGLTranUOM(PXCache cache, string uOM)
        {
            cache.SetValue<GLTran.uOM>(cache.Current, uOM);

            cache.Update(cache.Current);
        }
        #endregion
    }

    #region Filter DAC
    [Serializable]
    [PXCacheName("Ledger Transaction Filter")]
    public partial class LedgerTranFilter : IBqlTable
    {
        #region BranchID
        [Branch()]
        public virtual int? BranchID { get; set; }
        public abstract class branchID : PX.Data.BQL.BqlInt.Field<branchID> { }
        #endregion

        #region StlmtAcctID
        [PXDBInt()]
        [PXUIField(DisplayName = "Account", Visibility = PXUIVisibility.SelectorVisible)]
        [PXSelector(typeof(Search<LSSettlementAccount.accountID, Where<LSSettlementAccount.type, Equal<AccountType.asset>,
                                                                       Or<LSSettlementAccount.type, Equal<AccountType.liability>>>>),
                    typeof(LSSettlementAccount.accountID),
                    typeof(LSSettlementAccount.type),    
                    SubstituteKey = typeof(LSSettlementAccount.accountCD),
                    DescriptionField = typeof(LSSettlementAccount.description))]
        public virtual int? StlmtAcctID { get; set; }
        public abstract class stlmtAcctID : PX.Data.BQL.BqlInt.Field<stlmtAcctID> { }
        #endregion

        #region StlmtAcctType
        [PXDBString(1, IsUnicode = true, IsFixed = true)]
        [PXUIField(DisplayName = "Type", IsReadOnly = true)]
        [PXDefault(typeof(Search<Account.type, Where<Account.accountID, Equal<Current<stlmtAcctID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
        [PXFormula(typeof(Default<stlmtAcctID>))]
        public virtual string StlmtAcctType { get; set; }
        public abstract class stlmtAcctType : PX.Data.BQL.BqlString.Field<stlmtAcctType> { }
        #endregion

        #region BalanceAmt
        [PX.Objects.CM.PXDBBaseCury(typeof(GLTran.ledgerID))]
        [PXUIField(DisplayName = "Balance", IsReadOnly = true)]
        public virtual decimal? BalanceAmt { get; set; }
        public abstract class balanceAmt : PX.Data.BQL.BqlDecimal.Field<balanceAmt> { }
        #endregion
    }
    #endregion
}