using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.GL;
using PX.Objects.GL.FinPeriods;
using KedgeFinCustDev.FIN.DAC;

namespace KedgeFinCustDev.FIN.Graph
{
    public class KGAcctMovementProc : PXGraph<KGAcctMovementProc>
    {
        #region Selects & Feature
        public PXCancel<KGProjHistoryFilter> Cancel;
        public PXFilter<KGProjHistoryFilter> Filter;
        public PXFilteredProcessing<KGProjHistoryTran, KGProjHistoryFilter, Where<KGProjHistoryTran.finYear, Equal<Optional<KGProjHistoryFilter.year>>>> ProjHistTran;
        public SelectFrom<KGProjHistoryByPeriod>.Where<KGProjHistoryByPeriod.finYear.IsEqual<KGProjHistoryFilter.year.FromCurrent>>.View ProjHistByPeriod;
        #endregion

        #region Ctor
        public KGAcctMovementProc()
        {
            KGProjHistoryFilter filter = Filter.Current;

            ProjHistTran.SetProcessVisible(false);
            ProjHistTran.SetProcessAllCaption("Generate Movement Data");
            ProjHistTran.SetProcessDelegate(delegate (List<KGProjHistoryTran> list)
                                            {
                                                GenerateProjHistory(list, filter);
                                            });
        }
        #endregion

        #region Delegate Data View
        /// <summary>
        /// Since the data view will have more than 10,000 records in processing, the maximum rows is specified after discussion with YJ.
        /// </summary>
        public IEnumerable projHistTran()
        {
            PXView view = new PXView(this, true, ProjHistTran.View.BqlSelect);

            int startRow = PXView.StartRow;
            int totalRow = 0;

            var lists = view.Select(PXView.Currents, PXView.Parameters, PXView.Searches, PXView.SortColumns, PXView.Descendings, PXView.Filters, ref startRow, 100, ref totalRow);

            PXView.StartRow = 0;

            return lists;
        }
        #endregion

        #region Event Handers
        protected virtual void _(Events.FieldUpdated<KGProjHistoryFilter.year> e)
        {
            string year = (string)e.NewValue;

            if (ProjHistTran.Select().Count <= 0 && !string.IsNullOrEmpty(year))
            {
                ///<remarks> Since there is no data for each year at the beginning, that will impact the processing feature doesn't work. </remarks>
                // Acuminator disable once PX1043 SavingChangesInEventHandlers [Justification]
                InsertInitializedData(year);
            }
        }
        #endregion

        #region Static Method
        public static void GenerateProjHistory(List<KGProjHistoryTran> list, KGProjHistoryFilter filter)
        {
            KGAcctMovementProc graph = CreateInstance<KGAcctMovementProc>();

            graph.DeleteAllData(filter.Year);

            // Since the delete script is executed directly, the cache will not be refreshed and the cache needs to be cleared manually.
            graph.ProjHistByPeriod.Cache.Clear();
            graph.ProjHistTran.Cache.Clear();

            graph.GenerateProjHistory(filter.Year);
        }
        #endregion

        #region Methods
        public virtual void GenerateProjHistory(string year)
        {
            #region Create KGProjHistoryTran
            List<KGProjHistoryTran> trans = SelectFrom<KGProjHistoryTran>.Where<KGProjHistoryTran.finYear.IsEqual<@P.AsString>>.View.Select(this, Convert.ToInt32(year) - 1).RowCast<KGProjHistoryTran>().ToList();
            // These key fields have history, but no transactions during the current year.
            for (int i = 0; i < trans.Count; i++)
            {
                KGProjHistoryTran histTran = trans[i];

                histTran.FinYear     = year;
                histTran.FinPeriodID = $"{year}{histTran.PeriodNbr}";
                histTran.NoteID      = null;

                histTran = ProjHistTran.Insert(histTran);

                GLTran tran = SelectFrom<GLTran>.Where<GLTran.accountID.IsEqual<@P.AsInt>
                                                      .And<GLTran.branchID.IsEqual<@P.AsInt>
                                                           .And<GLTran.projectID.IsEqual<@P.AsInt>
                                                                .And<GLTran.ledgerID.IsEqual<@P.AsInt>
                                                                     .And<GLTran.subID.IsEqual<@P.AsInt>
                                                                          .And<GLTran.finPeriodID.IsEqual<@P.AsString>>>>>>>
                                                .AggregateTo<GroupBy<GLTran.accountID,
                                                                    GroupBy<GLTran.branchID,
                                                                            GroupBy<GLTran.projectID,
                                                                                    GroupBy<GLTran.ledgerID,
                                                                                            GroupBy<GLTran.subID,
                                                                                                    GroupBy<GLTran.finPeriodID,
                                                                                                            Sum<GLTran.debitAmt,
                                                                                                                Sum<GLTran.creditAmt>>>>>>>>>
                                                .View.Select(this, histTran.AccountID, histTran.BranchID, histTran.ProjectID, histTran.LedgerID, histTran.SubID, histTran.FinPeriodID);

                histTran.FinPtdCredit = tran?.CreditAmt;
                histTran.FinPtdDebit  = tran?.DebitAmt;

                ProjHistTran.Update(histTran);
            }

            foreach (PXResult<GLTran, Account> res in SelectFrom<GLTran>.LeftJoin<Account>.On<Account.accountID.IsEqual<GLTran.accountID>>
                                                                        .Where<GLTran.finPeriodID.Contains<@P.AsString>
                                                                                .And<Account.type.IsIn<AccountType.asset, AccountType.liability, AccountType.expense, AccountType.income>
                                                                                    .And<GLTran.posted.IsEqual<True>>>>
                                                                        .AggregateTo<GroupBy<GLTran.accountID,
                                                                                            GroupBy<GLTran.branchID,
                                                                                                    GroupBy<GLTran.projectID,
                                                                                                            GroupBy<GLTran.ledgerID,
                                                                                                                    GroupBy<GLTran.subID,
                                                                                                                            GroupBy<GLTran.finPeriodID,
                                                                                                                                    Sum<GLTran.debitAmt,
                                                                                                                                        Sum<GLTran.creditAmt>>>>>>>>>
                                                                        .View.Select(this, year))
            {
                GLTran tran = res;
                Account acct = res;

                foreach (MasterFinPeriod master in SelectFrom<MasterFinPeriod>.Where<MasterFinPeriod.finYear.IsEqual<@P.AsString>>.View.Select(this, year))
                {
                    KGProjHistoryTran histTran = new KGProjHistoryTran()
                    {
                        AccountID = tran.AccountID,
                        BranchID = tran.BranchID,
                        ProjectID = tran.ProjectID,
                        LedgerID = tran.LedgerID,
                        SubID = tran.SubID,
                        FinPeriodID = master.FinPeriodID,
                        FinYear = master.FinYear,
                        PeriodNbr = master.PeriodNbr,
                        AcctType = acct.Type
                    };

                    histTran = ProjHistTran.Cache.Locate(histTran) == null ? ProjHistTran.Insert(histTran) : ProjHistTran.Cache.Locate(histTran) as KGProjHistoryTran;

                    if (master.FinPeriodID == tran.FinPeriodID)
                    {
                        histTran.FinPtdCredit = tran.CreditAmt;
                        histTran.FinPtdDebit = tran.DebitAmt;

                        ProjHistTran.Update(histTran);
                    }
                }
            }

            this.Actions.PressSave();
            #endregion

            #region Create KGProjHistoryByPeriod
            decimal? ytdBalance = 0m, calcBalance = 0m;

            Dictionary<string, decimal?> dic = new Dictionary<string, decimal?>();

            // Get the ending balance at the end of last year's month.
            List<KGProjHistoryByPeriod> periods = SelectFrom<KGProjHistoryByPeriod>.Where<KGProjHistoryByPeriod.finPeriodID.IsEqual<@P.AsString>>
                                                                                   .AggregateTo<GroupBy<KGProjHistoryByPeriod.accountID,
                                                                                                        GroupBy<KGProjHistoryByPeriod.branchID,
                                                                                                                GroupBy<KGProjHistoryByPeriod.projectID,
                                                                                                                        GroupBy<KGProjHistoryByPeriod.ledgerID,
                                                                                                                                GroupBy<KGProjHistoryByPeriod.subID,
                                                                                                                                        Sum<KGProjHistoryByPeriod.finYtdBalance>>>>>>>
                                                                                    .View.Select(this, $"{Convert.ToInt32(year) - 1}12").RowCast<KGProjHistoryByPeriod>().ToList();

            foreach (KGProjHistoryTran histTran in SelectFrom<KGProjHistoryTran>.Where<KGProjHistoryTran.finYear.IsEqual<@P.AsString>>.View.Select(this, year))
            {
                KGProjHistoryByPeriod histPeriod = new KGProjHistoryByPeriod()
                {
                    AccountID = histTran.AccountID, BranchID = histTran.BranchID, ProjectID   = histTran.ProjectID,
                    LedgerID  = histTran.LedgerID,  SubID    = histTran.SubID,    FinPeriodID = histTran.FinPeriodID, 
                    FinYear   = histTran.FinYear,   AcctType = histTran.AcctType
                };

                histPeriod = ProjHistByPeriod.Insert(histPeriod);

                // Debug by specifying conditions.
                //if (histPeriod.AccountID == 729 && histPeriod.ProjectID == 179) { ytdBalance = 0; }

                bool isJanuary = histTran.FinPeriodID.Contains($"{histTran.FinYear}01");

                // Calculate the balance for each month of the year.
                calcBalance = histTran.AcctType == AccountType.Asset ? (histTran.FinPtdDebit ?? 0m) - (histTran.FinPtdCredit ?? 0m) : (histTran.FinPtdCredit ?? 0m) - (histTran.FinPtdDebit ?? 0m);

                ytdBalance = isJanuary ? calcBalance : ytdBalance + calcBalance;

                // Only the first month of each year is required to accumulate the ending balance of the previous year.
                if (histTran.AcctType.IsIn(AccountType.Asset, AccountType.Liability) && ytdBalance == calcBalance && isJanuary)
                {
                    // Find the corresponding record using the key field.
                    var lastYearRec = periods.Find(f => f.AccountID == histPeriod.AccountID && f.BranchID == histPeriod.BranchID && f.ProjectID == histPeriod.ProjectID && 
                                                        f.LedgerID == histPeriod.LedgerID && f.SubID == histPeriod.SubID);

                    // Current year beginning balance + last year ending balance
                    ytdBalance += (lastYearRec?.FinYtdBalance ?? 0m);
                }

                histPeriod.FinYtdBalance = ytdBalance;

                ProjHistByPeriod.Update(histPeriod);
            }

            int? retEarnAccountID = SelectFrom<GLSetup>.View.SelectSingleBound(this, null).TopFirst?.RetEarnAccountID;

            foreach (KGProjHistoryByPeriod row in ProjHistByPeriod.Cache.Inserted.RowCast<KGProjHistoryByPeriod>().Where(w => w.AccountID == retEarnAccountID))
            {
                decimal? totalBalance = ProjHistByPeriod.Cache.Inserted.RowCast<KGProjHistoryByPeriod>().Where(w => w.AccountID != retEarnAccountID && w.FinPeriodID == row.FinPeriodID &&
                                                                                                               w.AcctType != AccountType.Asset)
                                                                                                        .GroupBy(g => new { g.FinPeriodID }).Select(s => s.Sum(x => x.FinYtdBalance)).FirstOrDefault().GetValueOrDefault();

                decimal? assetBalance = ProjHistByPeriod.Cache.Inserted.RowCast<KGProjHistoryByPeriod>().Where(w => w.AccountID != retEarnAccountID && w.FinPeriodID == row.FinPeriodID && 
                                                                                                               w.AcctType == AccountType.Asset)
                                                                                                        .GroupBy(g => new { g.FinPeriodID }).Select(s => s.Sum(x => x.FinYtdBalance)).FirstOrDefault().GetValueOrDefault();
                row.FinYtdBalance = 0 - totalBalance + assetBalance;

                ProjHistByPeriod.Update(row);
            }

            this.Actions.PressSave();
            #endregion
        }

        private void InsertInitializedData(string year)
        {
            string screenIDWODot = this.Accessinfo.ScreenID.ToString().Replace(".", "");

            // Since the data view is delegated, checking for the existence of fields that define the PXSelector property will affect processing to determine the existence of the initial record.
            int? accountID = SelectFrom<Account>.OrderBy<Account.accountID.Asc>.View.SelectSingleBound(this, null).TopFirst?.AccountID;
            int? subID     = SelectFrom<Sub>.OrderBy<Sub.subID.Asc>.View.SelectSingleBound(this, null).TopFirst?.SubID;
            int? ledgerID  = SelectFrom<Ledger>.OrderBy<Ledger.ledgerID.Asc>.View.SelectSingleBound(this, null).TopFirst?.LedgerID;
            string finPer  = SelectFrom<MasterFinPeriod>.Where<MasterFinPeriod.finYear.IsEqual<@P.AsString>>.View.SelectSingleBound(this, null, year).TopFirst?.FinPeriodID;

            PXDatabase.Insert<KGProjHistoryTran>(new PXDataFieldAssign<KGProjHistoryTran.createdByID>(this.Accessinfo.UserID),
                                                 new PXDataFieldAssign<KGProjHistoryTran.createdByScreenID>(screenIDWODot),
                                                 new PXDataFieldAssign<KGProjHistoryTran.createdDateTime>(this.Accessinfo.BusinessDate),
                                                 new PXDataFieldAssign<KGProjHistoryTran.lastModifiedByID>(this.Accessinfo.UserID),
                                                 new PXDataFieldAssign<KGProjHistoryTran.lastModifiedByScreenID>(screenIDWODot),
                                                 new PXDataFieldAssign<KGProjHistoryTran.lastModifiedDateTime>(this.Accessinfo.BusinessDate),
                                                 new PXDataFieldAssign<KGProjHistoryTran.noteID>(Guid.NewGuid()),
                                                 new PXDataFieldAssign<KGProjHistoryTran.accountID>(accountID),
                                                 new PXDataFieldAssign<KGProjHistoryTran.branchID>(this.Accessinfo.BranchID),
                                                 new PXDataFieldAssign<KGProjHistoryTran.projectID>(0),
                                                 new PXDataFieldAssign<KGProjHistoryTran.ledgerID>(ledgerID),
                                                 new PXDataFieldAssign<KGProjHistoryTran.subID>(subID),
                                                 new PXDataFieldAssign<KGProjHistoryTran.finPeriodID>(finPer),
                                                 new PXDataFieldAssign<KGProjHistoryTran.finYear>(year));
        }

        private void DeleteAllData(string year)
        {
            PXDatabase.Delete<KGProjHistoryTran>(new PXDataFieldRestrict<KGProjHistoryTran.finYear>(year));
            PXDatabase.Delete<KGProjHistoryByPeriod>(new PXDataFieldRestrict<KGProjHistoryByPeriod.finYear>(year));
        }
        #endregion
    }

    #region Unbound DAC
    [PXHidden]
    public class KGProjHistoryFilter : PX.Data.IBqlTable
    {
        #region Year
        [PXString(4, IsFixed = true)]
        [PXUIField(DisplayName = "Financial Year", Visibility = PXUIVisibility.SelectorVisible)]
        [PXSelector(typeof(Search3<MasterFinYear.year, OrderBy<Desc<MasterFinYear.year>>>))]
        public virtual string Year { get; set; }
        public abstract class year : PX.Data.BQL.BqlString.Field<year> { }
        #endregion
    }
    #endregion
}