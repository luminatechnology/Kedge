using System;
using System.Collections;
using System.Collections.Generic;
using NM.DAC;
using PX.Data;
using PX.Objects.CA;
using PX.Objects.GL;
using PX.Objects.CS;
using static NM.Util.NMStringList;
using KG.Util;

namespace NM
{
    public class NMSettlementReport : PXGraph<NMSettlementReport>
    {
        #region Constants
        public const string Userdefined_CATYPE = "AttributeCATYPE";
        public const string CATYPE_REAL = "REAL";
        #endregion

        #region Views
        public PXFilter<ParamTable> MasterView;
        public PXSelect<NMSettlementLog, Where<NMSettlementLog.branchID, Equal<Current<ParamTable.filterBranchID>>,
                                               And<NMSettlementLog.settlementDate, GreaterEqual<Current<ParamTable.filterDateFrom>>,
                                                   And<NMSettlementLog.settlementDate, LessEqual<Current<ParamTable.filterDateTo>>>>>> DetailsView;
        public PXSelect<NMDailyReportTmp, Where<NMDailyReportTmp.settlementID, Equal<Current<NMSettlementLog.settlementID>>>> ReportTmp;
        #endregion

        #region Actions
        #region 結算
        public PXAction<ParamTable> SettlementBtn;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "結算")]
        protected IEnumerable settlementBtn(PXAdapter adapter)
        {
            ParamTable param = MasterView.Current;
            if (!CheckNull<ParamTable.settlementBranchID>(MasterView.Cache, param, param.SettlementBranchID)) return adapter.Get();
            if (!CheckNull<ParamTable.settlementDateFrom>(MasterView.Cache, param, param.SettlementDateFrom)) return adapter.Get();
            if (!CheckNull<ParamTable.settlementDateTo>(MasterView.Cache, param, param.SettlementDateTo)) return adapter.Get();
            PXLongOperation.StartOperation(this, DoSettlement);
            return adapter.Get();
        }
        #endregion

        #region 收支日報表
        public PXAction<ParamTable> ReportBtn;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "收支日報表")]
        protected IEnumerable reportBtn(PXAdapter adapter)
        {
            ParamTable param = MasterView.Current;

            if (!CheckNull<ParamTable.reportBranchID>(MasterView.Cache, param, param.ReportBranchID)) return adapter.Get();
            if (!CheckNull<ParamTable.reportDate>(MasterView.Cache, param, param.ReportDate)) return adapter.Get();

            Dictionary<string, string> patams = new Dictionary<string, string>();

            patams["BranchID"]                     = ((Branch)PXSelectorAttribute.Select<ParamTable.reportBranchID>(MasterView.Cache, param))?.BranchCD;
            patams["SettlementDateFrom"]           = param.ReportDate?.ToString("yyyy/MM/dd");
            patams["SettlementDateTo"]             = param.ReportDate?.ToString("yyyy/MM/dd");
            patams[nameof(ParamTable.DisplayZero)] = param?.DisplayZero.ToString();

            throw new PXReportRequiredException(patams, "NM603001", "Report");
        }
        #endregion
        #endregion

        #region Event
        public void _(Events.FieldDefaulting<ParamTable, ParamTable.filterDateFrom> e)
        {
            if (e.Row == null) return;
            e.NewValue = this.Accessinfo.BusinessDate?.AddDays(-7);
        }

        public void _(Events.FieldDefaulting<ParamTable, ParamTable.settlementDateFrom> e)
        {
            if (e.Row == null) return;
            e.NewValue = this.Accessinfo.BusinessDate?.AddDays(-7);
        }
        #endregion

        #region Methods
        private void DoSettlement()
        {
            using (PXTransactionScope ts = new PXTransactionScope())
            {
                ParamTable param = MasterView.Current;
                DateTime dateB = (DateTime)param.SettlementDateFrom;
                DateTime dateE = (DateTime)param.SettlementDateTo;
                while (dateB <= dateE)
                {
                    DeleteOldData(param.SettlementBranchID, dateB);
                    CreateSettlementLog(param.SettlementBranchID, dateB);
                    dateB = dateB.AddDays(1);
                }
                Persist();
                ts.Complete();
            }
        }

        private void DeleteOldData(int? branchID, DateTime? settlementDate)
        {
            NMSettlementLog log = GetNMSettlementLog(branchID, settlementDate);
            if (log == null) return;
            DetailsView.Delete(log);
        }

        private void CreateSettlementLog(int? branchID, DateTime settlementDate)
        {
            NMSettlementLog log = (NMSettlementLog)DetailsView.Cache.CreateInstance();
            log = DetailsView.Insert(log);
            log.BranchID = branchID;
            log.SettlementDate = settlementDate;
            log.SettledBy = this.Accessinfo.UserID;
            log = DetailsView.Update(log);
            CreateDailyReportTmp(log);
        }

        private void CreateDailyReportTmp(NMSettlementLog log)
        {
            CreateDailyReportTmpByA(log);
            CreateDailyReportTmpByC(log);
        }

        /// <summary>
        /// 現金帳戶
        /// </summary>
        /// <param name="log"></param>
        private void CreateDailyReportTmpByA(NMSettlementLog log)
        {
            PXCache cashAccountCache = new PXCache<CashAccount>(this);
            PXCache bankAccountCache = new PXCache<NMBankAccount>(this);
            int seq = 0;
            //取得所有CashAccount
            foreach (CashAccount ca in GetCashAccountByBranch(log.BranchID))
            {
                #region 基本資訊 By CashAccount
                PXStringState _caTypeState = (PXStringState)cashAccountCache.GetValueExt(ca, Userdefined_CATYPE);

                string caType = (string)_caTypeState?.Value ?? CATYPE_REAL;
                
                //排除REAL以外資料
                if (caType != CATYPE_REAL) continue;

                NMDailyReportTmp tmp = (NMDailyReportTmp)ReportTmp.Cache.CreateInstance();

                tmp.SettlementID     = log.SettlementID;
                tmp.SettledType      = NMSettledType.CashAccount;
                tmp.Seq              = seq++;
                tmp.CashAccountID    = ca.CashAccountID;
                tmp.AccountID        = ca.AccountID;
                tmp.Subid            = ca.SubID;
                tmp.BranchID         = ca.BranchID;
                tmp.CuryDebitAmt     = tmp.CuryCreditAmt = 0m;
                tmp.CuryDebitCnt     = tmp.CuryCreditCnt = 0;
                // Added a new field according to mantis [0012346] (A)
                tmp.CashAccountAlias = ca.ExtRefNbr;

                NMBankAccount ba = GetBankAccount(ca.CashAccountID);
                if (ba != null)
                {
                    tmp.BankShortName    = ba?.BankShortName;
                    // Add new field as one of sorting conditions per Matis [0012288] #4
                    tmp.BankCode         = ba?.BankCode;
                    tmp.BankAccount      = ba?.BankAccount;
                    tmp.AccountTypeDesc  = (PXSelectorAttribute.Select<NMBankAccount.accountType>(bankAccountCache, ba) as SegmentValue)?.Descr;
                    tmp.RestrictedAmt    = ba?.RestrictedAmount ?? 0;
                    tmp.NMSettlementDate = ba?.SettlementDate;
                }
                else
                {
                    //2021-08-19 當對應不上BankAccount時寫入CashAccountCD
                    tmp.BankShortName = ca.CashAccountCD;
                }
                #endregion

                #region By Batch
                foreach (GLTran glTran in GetGLTranInBatch(log.BranchID, log.SettlementDate, ca.AccountID, ca.SubID))
                {
                    if ((glTran.CuryCreditAmt ?? 0) != 0)
                    {
                        tmp.CuryCreditAmt += (glTran.CuryCreditAmt ?? 0);
                        tmp.CuryCreditCnt += 1;
                    }
                    if ((glTran.CuryDebitAmt ?? 0) != 0)
                    {
                        tmp.CuryDebitAmt += (glTran.CuryDebitAmt ?? 0);
                        tmp.CuryDebitCnt += 1;
                    }
                }
                #endregion

                #region BegBalance
                decimal creditAmt = 0, debitAmt = 0;

                foreach (GLTran glTran in GetPreGLTranInBatch(log.BranchID, log.SettlementDate, ca.AccountID, ca.SubID))
                {
                    creditAmt += (glTran.CuryCreditAmt ?? 0);
                    debitAmt += (glTran.CuryDebitAmt ?? 0);
                }

                tmp.BegBalance = debitAmt - creditAmt;
                #endregion

                ReportTmp.Insert(tmp);
            }
        }

        /// <summary>
        /// Unused now.
        /// 現金帳戶帳戶類型
        /// </summary>
        /// <param name="log"></param>
        //private void CreateDailyReportTmpByB(NMSettlementLog log) { }

        /// <summary>
        /// 其他
        /// </summary>
        /// <param name="log"></param>
        private void CreateDailyReportTmpByC(NMSettlementLog log)
        {
            // Remove two types per Mantis [0012288] #2
            // Added one more type according to mantis [0012358]
            string[] ClassIDs = { /*"應收帳款", "應付帳款",*/ "應收票據", "應付票據", "應收保證票", "應付保證票", "定期存款" };
            string[] accounts = { "1117", "2102", "1413", "2303", "1106" };

            int seq = 0;
            foreach (string classID in ClassIDs)
            {
                bool isBillPayable = classID.Equals("應付票據");

                NMDailyReportTmp tmp = (NMDailyReportTmp)ReportTmp.Cache.CreateInstance();

                tmp.SettlementID  = log.SettlementID;
                tmp.SettledType   = NMSettledType.Other;
                tmp.Seq           = seq++;
                tmp.CashAccountID = 0;
                tmp.BankShortName = classID;
                tmp.BranchID      = log.BranchID;
                tmp.CuryDebitAmt  = tmp.CuryCreditAmt = 0m;
                tmp.CuryDebitCnt  = tmp.CuryCreditCnt = 0;

                #region By Batch
                // Change the retrieval data logic per Mantis [0012288] #3
                foreach (GLTran glTran in GetGLTranByAcctID(tmp.BranchID, accounts[tmp.Seq.Value], log.SettlementDate, isBillPayable))
                {
                    if ((glTran.CuryCreditAmt ?? 0) != 0)
                    {
                        tmp.CuryCreditAmt += (glTran.CuryCreditAmt ?? 0);
                        tmp.CuryCreditCnt += 1;
                    }
                    if ((glTran.CuryDebitAmt ?? 0) != 0)
                    {
                        tmp.CuryDebitAmt += (glTran.CuryDebitAmt ?? 0);
                        tmp.CuryDebitCnt += 1;
                    }
                }
                #endregion

                #region BegBalance
                decimal creditAmt = 0, debitAmt = 0;
                // Change the retrieval data logic per Mantis [0012288] #3
                foreach (GLTran glTran in GetGLTranByAcctID(tmp.BranchID, accounts[tmp.Seq.Value], log.SettlementDate, isBillPayable, true))
                {
                    creditAmt += (glTran.CuryCreditAmt ?? 0);
                    debitAmt  += (glTran.CuryDebitAmt ?? 0);
                }
                tmp.BegBalance = Math.Abs(debitAmt - creditAmt);
                #endregion

                ReportTmp.Insert(tmp);
            }
        }

        private bool CheckNull<Field>(PXCache cache, Object row, object newValue) where Field : PX.Data.IBqlField
        {
            if (newValue == null)
            {
                SetError<Field>(cache, row, newValue, "請輸入" + typeof(Field).Name);
                return false;
            }
            return true;
        }

        private void SetError<Field>(PXCache cache, Object row, object newValue, String errorMsg) where Field : PX.Data.IBqlField
        {
            cache.RaiseExceptionHandling<Field>(row, newValue,
                  new PXSetPropertyException(errorMsg, PXErrorLevel.RowError));
        }
        #endregion

        #region BQL
        //public PXResultset<GLTran> GetGLTranByClassID(string classID, DateTime? settlementDate)
        //{
        //    return PXSelectJoin<GLTran,
        //        InnerJoin<Batch,
        //            On<Batch.module, Equal<GLTran.module>,
        //            And<Batch.batchNbr, Equal<GLTran.batchNbr>,
        //            And<Batch.branchID, Equal<GLTran.branchID>>>>,
        //        InnerJoin<Account,
        //            On<Account.accountID, Equal<GLTran.accountID>>
        //            >>,
        //        Where<Batch.released, Equal<True>,
        //            And<Batch.posted, Equal<True>,
        //            And<Account.accountClassID, Equal<Required<Account.accountClassID>>,
        //            And<Batch.dateEntered, Equal<Required<Batch.dateEntered>>>>>>
        //        >.Select(this, classID, settlementDate);
        //}

        /// <summary>
        /// 應收票據: 1117% (會有1117與1117.01) 應付票據: 2102% 與 2117 應收保證票: 1413 應付保證票: 2303
        /// </summary>
        public class FixedAcct_2117 : PX.Data.BQL.BqlString.Constant<FixedAcct_2117>
        {
            public FixedAcct_2117() : base("2117") { }
        }

        public PXResultset<GLTran> GetGLTranByAcctID(int? branchID, string accountCD, DateTime? settlementDate, bool isBillPayable = false, bool calcBegBalance = false)
        {
            PXSelectBase<GLTran> select = new PXSelectJoin<GLTran, InnerJoin<Batch, On<Batch.module, Equal<GLTran.module>,
                                                                             And<Batch.batchNbr, Equal<GLTran.batchNbr>,
                                                                                 And<Batch.branchID, Equal<GLTran.branchID>>>>,
                                                                             InnerJoin<Account, On<Account.accountID, Equal<GLTran.accountID>>>>,
                                                                   Where<Batch.released, Equal<True>,
                                                                         And<Batch.posted, Equal<True>,
                                                                             And<GLTran.branchID, Equal<Required<GLTran.branchID>>,
                                                                                 And<Where2<Where<BatchExt.usrStageCode, IsNull>,
                                                                                            Or<Where2<Where<BatchExt.usrStageCode,
                                                                                            NotIn3<GLStageCode.nMPConfirmP2, GLStageCode.nMPConfirmP3, GLStageCode.nMPConfirmP4,
                                                                                                   GLStageCode.nMPVoidPB, GLStageCode.nMPVoidPC, GLStageCode.nMPVoidPD, GLStageCode.nMPVoidPE>>,
                                                                                            And<BatchExt.usrStageCode, NotIn3<GLStageCode.nMRPaymentReleaseR1, GLStageCode.nMRReceiveR2,
                                                                                                                              GLStageCode.nMRVoidRB, GLStageCode.nMRVoidRC, GLStageCode.nMRVoidRE,
                                                                                                                              GLStageCode.nMRPaymentReleaseR5, GLStageCode.nMPaymentVoidRF, GLStageCode.ePDepositM1>>>
                                                           >>>>>>>(this);

            // Redefine the BQL query for the specified account according to mantis [0012358].
            /**
            if (accountCD == "1106")
            {
                
                select = new PXSelectJoin<GLTran, InnerJoin<Batch, On<Batch.module, Equal<GLTran.module>,
                                                                      And<Batch.batchNbr, Equal<GLTran.batchNbr>,
                                                                          And<Batch.branchID, Equal<GLTran.branchID>>>>,
                                                            InnerJoin<Account, On<Account.accountID, Equal<GLTran.accountID>>>>,
                                                  Where<Batch.released, Equal<True>,
                                                        And<Batch.posted, Equal<True>,
                                                            And<GLTran.branchID, Equal<Required<GLTran.branchID>>,
                                                                And<Where2<Where<BatchExt.usrStageCode, IsNull>,
                                                                           Or<BatchExt.usrStageCode, NotEqual<GLStageCode.ePDepositM1>>>>>>>>(this);
                
            }**/

            if (isBillPayable == true)
            {
                select.WhereAnd<Where<Account.accountCD, StartsWith<Required<Account.accountCD>>,
                                      Or<Account.accountCD, Equal<FixedAcct_2117>>>>();
            }
            else
            {
                select.WhereAnd<Where<Account.accountCD, StartsWith<Required<Account.accountCD>>>>();
            }

            if (calcBegBalance == false)
            {
                select.WhereAnd<Where<Batch.dateEntered, Equal<Required<Batch.dateEntered>>>>();
            }
            else
            {
                select.WhereAnd<Where<Batch.dateEntered, Less<Required<Batch.dateEntered>>>>();
            }

            return select.Select(branchID, accountCD, settlementDate);
        }

        //public PXResultset<GLTran> GetPreGLTranByClassID(string classID, DateTime? settlementDate)
        //{
        //    return PXSelectJoin<GLTran,
        //        InnerJoin<Batch,
        //            On<Batch.module, Equal<GLTran.module>,
        //            And<Batch.batchNbr, Equal<GLTran.batchNbr>,
        //            And<Batch.branchID, Equal<GLTran.branchID>>>>,
        //        InnerJoin<Account,
        //            On<Account.accountID, Equal<GLTran.accountID>>
        //            >>,
        //        Where<Batch.released, Equal<True>,
        //            And<Batch.posted, Equal<True>,
        //            And<Account.accountClassID, Equal<Required<Account.accountClassID>>,
        //            And<Batch.dateEntered, Less<Required<Batch.dateEntered>>>>>>
        //        >.Select(this, classID, settlementDate);
        //}

        public PXResultset<CashAccount> GetCashAccountByBranch(int? branchID)
        {
            return PXSelect<CashAccount,
                Where<CashAccount.branchID, Equal<Required<CashAccount.branchID>>
                >>.Select(this, branchID);
        }

        public PXResultset<GLTran> GetGLTranInBatch(int? branchID, DateTime? settlementDate, int? accountID, int? subID)
        {
            return PXSelectJoin<GLTran, InnerJoin<Batch, On<Batch.module, Equal<GLTran.module>,
                                                            And<Batch.branchID, Equal<GLTran.branchID>,
                                                                And<Batch.batchNbr, Equal<GLTran.batchNbr>>>>>,
                                Where<Batch.released, Equal<True>,
                                And<Batch.posted, Equal<True>,
                                And<Batch.branchID, Equal<Required<Batch.branchID>>,
                                And<Batch.dateEntered, Equal<Required<Batch.dateEntered>>,
                                And<GLTran.accountID, Equal<Required<GLTran.accountID>>,
                                And<GLTran.subID, Equal<Required<GLTran.subID>>,
                                And<Where2<Where<BatchExt.usrStageCode, IsNull>,
                                           Or<Where2<Where<BatchExt.usrStageCode, 
                                           NotIn3<GLStageCode.nMPConfirmP2, GLStageCode.nMPConfirmP3, GLStageCode.nMPConfirmP4,
                                          GLStageCode.nMPVoidPB, GLStageCode.nMPVoidPC, GLStageCode.nMPVoidPD, GLStageCode.nMPVoidPE>>,
                                            And<BatchExt.usrStageCode, NotIn3<GLStageCode.nMRPaymentReleaseR1, GLStageCode.nMRReceiveR2,
                                          GLStageCode.nMRVoidRB, GLStageCode.nMRVoidRC, GLStageCode.nMRVoidRE,
                                           GLStageCode.nMRPaymentReleaseR5, GLStageCode.nMPaymentVoidRF>>>
                                           >>>>>>>>>>
                                .Select(this, branchID, settlementDate, accountID, subID);
        }

        public PXResultset<GLTran> GetPreGLTranInBatch(int? branchID, DateTime? settlementDate, int? accountID, int? subID)
        {
            return PXSelectJoin<GLTran,
                InnerJoin<Batch,
                    On<Batch.module, Equal<GLTran.module>,
                    And<Batch.branchID, Equal<GLTran.branchID>,
                    And<Batch.batchNbr, Equal<GLTran.batchNbr>>>
                >>,
                Where<Batch.released, Equal<True>,
                And<Batch.posted, Equal<True>,
                And<Batch.branchID, Equal<Required<Batch.branchID>>,
                And<Batch.dateEntered, Less<Required<Batch.dateEntered>>,
                And<GLTran.accountID, Equal<Required<GLTran.accountID>>,
                And<GLTran.subID, Equal<Required<GLTran.subID>>,
                And<Where2<Where<BatchExt.usrStageCode, IsNull>,
                                           Or<Where2<Where<BatchExt.usrStageCode,
                                           NotIn3<GLStageCode.nMPConfirmP2, GLStageCode.nMPConfirmP3, GLStageCode.nMPConfirmP4,
                                          GLStageCode.nMPVoidPB, GLStageCode.nMPVoidPC, GLStageCode.nMPVoidPD, GLStageCode.nMPVoidPE>>,
                                            And<BatchExt.usrStageCode, NotIn3<GLStageCode.nMRPaymentReleaseR1, GLStageCode.nMRReceiveR2,
                                          GLStageCode.nMRVoidRB, GLStageCode.nMRVoidRC, GLStageCode.nMRVoidRE, 
                                          GLStageCode.nMRPaymentReleaseR5, GLStageCode.nMPaymentVoidRF
                                          >>>
                                           >>>>>>>>>>
                .Select(this, branchID, settlementDate, accountID, subID);
        }

        public NMSettlementLog GetNMSettlementLog(int? branchID, DateTime? settlementDate)
        {
            return PXSelect<NMSettlementLog,
                Where<NMSettlementLog.branchID, Equal<Required<NMSettlementLog.branchID>>,
                And<NMSettlementLog.settlementDate, Equal<Required<NMSettlementLog.settlementDate>>
                >>>.Select(this, branchID, settlementDate);
        }

        public NMBankAccount GetBankAccount(int? cashAccountID)
        {
            return PXSelect<NMBankAccount,
                Where<NMBankAccount.cashAccountID, Equal<Required<NMBankAccount.cashAccountID>>
                >>.Select(this, cashAccountID);
        }
        #endregion

        #region Table
        [Serializable]
        public class ParamTable : IBqlTable
        {
            #region 計算記錄條件
            #region FilterBranchID
            [PXInt()]
            [PXUIField(DisplayName = "Branch ID")]
            [PXDimensionSelector("BIZACCT", typeof(Search<Branch.branchID, Where<Branch.active, Equal<True>, And<MatchWithBranch<Branch.branchID>>>>), typeof(Branch.branchCD), DescriptionField = typeof(Branch.acctName))]
            [PXUnboundDefault(typeof(Current<AccessInfo.branchID>))]
            public virtual int? FilterBranchID { get; set; }
            public abstract class filterBranchID : PX.Data.BQL.BqlInt.Field<filterBranchID> { }
            #endregion

            #region FilterDateFrom
            [PXDate()]
            [PXUIField(DisplayName = "Settlement Date From")]
            [PXUnboundDefault(typeof(Current<AccessInfo.businessDate>))]
            public virtual DateTime? FilterDateFrom { get; set; }
            public abstract class filterDateFrom : PX.Data.BQL.BqlDateTime.Field<filterDateFrom> { }
            #endregion

            #region FilterDateTo
            [PXDate()]
            [PXUIField(DisplayName = "Settlement Date To")]
            [PXUnboundDefault(typeof(Current<AccessInfo.businessDate>))]
            public virtual DateTime? FilterDateTo { get; set; }
            public abstract class filterDateTo : PX.Data.BQL.BqlDateTime.Field<filterDateTo> { }
            #endregion
            #endregion

            #region 結算資料
            #region SettlementBranchID
            [PXInt()]
            [PXUIField(DisplayName = "Branch ID")]
            [PXDimensionSelector("BIZACCT", typeof(Search<Branch.branchID, Where<Branch.active, Equal<True>, And<MatchWithBranch<Branch.branchID>>>>), typeof(Branch.branchCD), DescriptionField = typeof(Branch.acctName))]
            [PXUnboundDefault(typeof(Current<AccessInfo.branchID>))]
            public virtual int? SettlementBranchID { get; set; }
            public abstract class settlementBranchID : PX.Data.BQL.BqlInt.Field<settlementBranchID> { }
            #endregion

            #region SettlementDateFrom
            [PXDate()]
            [PXUIField(DisplayName = "Settlement Date From")]
            [PXUnboundDefault(typeof(Current<AccessInfo.businessDate>))]
            public virtual DateTime? SettlementDateFrom { get; set; }
            public abstract class settlementDateFrom : PX.Data.BQL.BqlDateTime.Field<settlementDateFrom> { }
            #endregion

            #region SettlementDateTo
            [PXDate()]
            [PXUIField(DisplayName = "Settlement Date To")]
            [PXUnboundDefault(typeof(Current<AccessInfo.businessDate>))]
            public virtual DateTime? SettlementDateTo { get; set; }
            public abstract class settlementDateTo : PX.Data.BQL.BqlDateTime.Field<settlementDateTo> { }
            #endregion
            #endregion

            #region 收支日報表資料
            #region ReportBranchID
            [PXInt()]
            [PXUIField(DisplayName = "Branch ID")]
            [PXDimensionSelector("BIZACCT", typeof(Search<Branch.branchID, Where<Branch.active, Equal<True>, And<MatchWithBranch<Branch.branchID>>>>), typeof(Branch.branchCD), DescriptionField = typeof(Branch.acctName))]
            [PXUnboundDefault(typeof(Current<AccessInfo.branchID>))]
            public virtual int? ReportBranchID { get; set; }
            public abstract class reportBranchID : PX.Data.BQL.BqlInt.Field<reportBranchID> { }
            #endregion

            #region ReportDate
            [PXDate()]
            [PXUIField(DisplayName = "Settlement Date")]
            [PXUnboundDefault(typeof(Current<AccessInfo.businessDate>))]
            public virtual DateTime? ReportDate { get; set; }
            public abstract class reportDate : PX.Data.BQL.BqlDateTime.Field<reportDate> { }
            #endregion

            #region DisplayZero
            [PXDBBool()]
            [PXUIField(DisplayName = "Display Zero")]
            [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
            public virtual bool? DisplayZero { get; set; }
            public abstract class displayZero : PX.Data.BQL.BqlBool.Field<displayZero> { }
            #endregion
            #endregion
        }
        #endregion
    }
}