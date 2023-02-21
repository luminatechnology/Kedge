using System;
using PX.Data;
using Kedge.DAC;
using PX.Data.BQL;
using PX.Objects.CT;
using PX.Objects.GL;
using PX.Objects.CR;
using PX.Objects.PM;
using PX.Objects.GL.FinPeriods.TableDefinition;
using System.Collections.Generic;
using PX.Objects.CA;
using Branch = PX.Data.PXAccess.Branch;

namespace Kedge
{
    public class KGPercentCompleteProcess : PXGraph<KGPercentCompleteProcess>
    {

        public KGPercentCompleteProcess()
        {
            PXUIFieldAttribute.SetEnabled(MonthView.Cache, null, false);
            PXUIFieldAttribute.SetEnabled(YearView.Cache, null, false);
            SetLabel(true);
            SetLabel(false);
        }

        #region view
        public PXFilter<KG505002Filter> MasterView;
        //public PXSelect<KG505002Month, Where<KG505002Month.cumProfitThis, GreaterEqual<Zero>>> MonthView;
        //public PXSelect<KG505002Year, Where<KG505002Year.cumProfitThis, GreaterEqual<Zero>>> YearView;
        public PXSelect<KG505002Month> MonthView;
        public PXSelect<KG505002Year> YearView;
        #endregion

        #region Param
        public const int P_Active = 1;
        public const string P_Status = "D";


        public class pActive : BqlInt.Constant<pActive>
        {
            public pActive() : base(P_Active) { }
        }
        public class pStatus : BqlString.Constant<pStatus>
        {
            public pStatus() : base(P_Status) { }
        }
        #endregion

        #region Button

        public PXAction<KG505002Filter> ProcessAction;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "���s���ͳ�����")]
        protected void processAction()
        {
            KGPercentCompleteProcess graph = PXGraph.CreateInstance<KGPercentCompleteProcess>();
            DateTime now = System.DateTime.Now;
            string batchID = graph.Accessinfo.UserName + now.ToString("yyyyMMddHHmmss");
            KG505002Filter filter = MasterView.Current;
            if (filter != null)
            {
                if (filter.BranchID == null)
                {
                    PXCache cache = MasterView.Cache;
                    String msg = "�����q���i���ŭ�!";
                    MasterView.Cache.RaiseExceptionHandling<KG505002Filter.branchID>(
                        filter, filter.BranchID, new PXSetPropertyException(msg));
                    return;
                }
                if (filter.Period == null)
                {
                    String msg = "�������i���ŭ�!";
                    MasterView.Cache.RaiseExceptionHandling<KG505002Filter.period>(
                        filter, filter.Period, new PXSetPropertyException(msg));
                    return;
                }

                //�L����R���¸��
                foreach (KG505002Month item in PXSelect<KG505002Month>.Select(this))
                {
                    MonthView.Delete(item);
                }
                foreach (KG505002Year item in PXSelect<KG505002Year>.Select(this))
                {
                    YearView.Delete(item);
                }
                int mSeq = 1;
                int ySeq = 1;
                //�}�l����template���
                if (filter.ContractCD == null)
                {
                    CreateTemplateData((int)filter.BranchID, 0, filter.Period, batchID, true, ref mSeq);
                    CreateTemplateData((int)filter.BranchID, 0, filter.Period, batchID, false, ref ySeq);
                }
                else
                {
                    foreach (String contractCD in filter.ContractCD.Split(';'))
                    {
                        Contract c = GetContractByCD(contractCD.Trim());
                        if (c == null) continue;
                        CreateTemplateData((int)filter.BranchID, c.ContractID, filter.Period, batchID, true, ref mSeq);
                        CreateTemplateData((int)filter.BranchID, c.ContractID, filter.Period, batchID, false, ref ySeq);
                    }
                }

                base.Persist();
            }

        }

        public PXAction<KG505002Filter> MonthReportAction;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "���ͤ�p����")]
        protected void monthReportAction()
        {
            CallReport(true);
        }

        public PXAction<KG505002Filter> YearReportAction;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "���ͦ~�ֳ���")]
        protected void yearReportAction()
        {
            CallReport(false);
        }
        #endregion

        #region Event
        public void _(Events.FieldDefaulting<KG505002Filter.beginDate> e)
        {
            KG505002Filter row = (KG505002Filter)e.Row;
            DateTime d1 = this.Accessinfo.BusinessDate.Value;
            DateTime d2 = new DateTime(d1.Year, d1.Month, 1).AddYears(-1);
            row.BeginDate = d2;
        }
        public void _(Events.FieldDefaulting<KG505002Filter.endDate> e)
        {
            KG505002Filter row = (KG505002Filter)e.Row;
            DateTime d1 = this.Accessinfo.BusinessDate.Value;
            DateTime d2 = new DateTime(d1.Year, d1.Month, 1).AddMonths(1);
            row.EndDate = d2;
        }
        public void _(Events.FieldDefaulting<KG505002Filter.period> e)
        {
            KG505002Filter row = (KG505002Filter)e.Row;
            row.Period = this.Accessinfo.BusinessDate.Value.Year.ToString() + this.Accessinfo.BusinessDate.Value.Month.ToString().PadLeft(2, '0');
        }

        #endregion

        #region Method

        private void CallReport(bool isMonth)
        {
            Dictionary<string, string> patams = new Dictionary<string, string>();
            String reportID = isMonth ? "KG605008" : "KG605009";
            throw new PXReportRequiredException(patams, reportID, PXBaseRedirectException.WindowMode.New, "Report");
            
        }
        /// <summary>
        /// 2020-07-23 [mantis:11630] Alton- �L���_��ɶ� 
        /// </summary>
        /// <param name="branchID"></param>
        /// <param name="contractID"></param>
        /// <param name="periodID"></param>
        /// <param name="batchID"></param>
        /// <param name="isMonth"></param>
        private void CreateTemplateData(int branchID, int? contractID, string periodID, string batchID, bool isMonth, ref int seq)
        {
            string lastPeriodID;
            String sYear = periodID.Substring(0, 4);
            String sMonth = periodID.Substring(4, 2);
            DateTime tStartDate;
            DateTime tEndDate;
            DateTime lStartDate;
            DateTime lEndDate;
            if (isMonth)
            {
                DateTime d = new DateTime(int.Parse(sYear), int.Parse(sMonth), 1).AddMonths(-1);
                //���o�W�Ӥ�
                lastPeriodID = d.Year.ToString() + d.Month.ToString().PadLeft(2, '0');
                FinPeriod fpLast = GetFinPeriod(branchID, lastPeriodID);
                FinPeriod fpThis = GetFinPeriod(branchID, periodID);
                tStartDate = (DateTime)fpThis.StartDate;
                tEndDate = (DateTime)fpThis.EndDate;
                lStartDate = (DateTime)fpLast.StartDate;
                lEndDate = (DateTime)fpLast.EndDate;
            }
            else
            {
                //���o�W�@�~
                lastPeriodID = (int.Parse(sYear) - 1).ToString() + "12";
                FinPeriod fpThis = GetFinPeriod(branchID, periodID);
                tStartDate = (DateTime)fpThis.StartDate;
                tEndDate = (DateTime)fpThis.EndDate;
                lStartDate = new DateTime((int.Parse(sYear) - 1), 1, 1);
                lEndDate = new DateTime(int.Parse(sYear), 1, 1);//�~�֭p��h�~�~��
            }

            String thisLabel = tEndDate.AddDays(-1).ToString("yyyy/MM/dd");
            String lastLabel = lEndDate.AddDays(-1).ToString("yyyy/MM/dd");
            DateTime? thisDate = DateTime.Parse(thisLabel);
            DateTime? lastDate = DateTime.Parse(lastLabel);
            IKG505002 temp;
            foreach (PXResult<GLTran, Branch, Contract, Account, BAccount> data in GetMainData(branchID, contractID, periodID))
            {
                if (!ValidatorData(data)) continue;

                int _contractID = (int)((Contract)data).ContractID;
                GLTran _data = GetMainData(branchID, _contractID, lastPeriodID);
                #region 20200907 add by Alton(mantis:11676)
                //�_���
                DateTime? finStartDate = getFinStartDate(_contractID);

                //�����O�_�w�}�l�p��
                bool isThisStartDate = finStartDate == null ? true : thisDate >= finStartDate;
                //�e���O�_�w�}�l�p��
                bool isLastStartDate = finStartDate == null ? true : lastDate >= finStartDate;
                #endregion

                if (isMonth)
                {
                    temp = new KG505002Month();
                }
                else
                {
                    temp = new KG505002Year();
                }
                //��template table��T
                temp.BatchID = batchID;
                temp.BatchSeq = seq++;
                temp.BranchID = branchID;
                temp.FinPeriodID = periodID;
                temp.LabelThis = thisLabel;
                temp.LabelLast = lastLabel;
                //�u�a��T
                temp.ContractCD = ((Contract)data).ContractCD;
                temp.ContractDesc = ((Contract)data).Description;
                temp.VendorName = ((BAccount)data).AcctName;

                //�X�����J
                decimal totalIncomeCuryAmount = GetTotalCuryAmount(_contractID, "I");
                //�X������
                decimal totalExpenseCuryAmount = GetTotalCuryAmount(_contractID, "E");

                //===����===
                //20200907 add by Alton isThisStartDate
                //20200917 edit by Alton �֭p���b�������GLTran�AdebitAmt-creditAmt
                //�֭p�ئb����
                decimal debitAmt = ((GLTran)data).DebitAmt ?? 0;
                decimal creditAmt = ((GLTran)data).CreditAmt ?? 0;
                decimal cumCost = isThisStartDate ? (debitAmt - creditAmt) : 0;

                //2020/09/08 add by Althea ���FinModsAmt 
                decimal? FinModsR = getFinModsAmt("R", _contractID, thisDate);
                decimal? FinModsC = getFinModsAmt("C", _contractID, thisDate);

                //�q���ܧ󦬤J�w��
                decimal totalIncomeModifyAmount = FinModsR ?? 0;//?? GetTotalModifAmount(_contractID, "I", tStartDate, tEndDate);
                //�q���ܧ󦨥��w��
                decimal totalExpenseModifyAmount = FinModsC ?? 0;//?? GetTotalModifAmount(_contractID, "E", tStartDate, tEndDate);
                //�X����(���|)
                decimal incomeAmt = totalIncomeCuryAmount + totalIncomeModifyAmount;
                //�w�p��J����
                decimal expenseAmt = totalExpenseCuryAmount + totalExpenseModifyAmount;
                //�u�i = �֭p�ئb����/�w����J����
                decimal projectProgrees = 0;
                if (expenseAmt > 0) projectProgrees = cumCost / expenseAmt;
                //�֭p���J = �X���� * �u�i 
                decimal cumIncome = incomeAmt * projectProgrees;

                //===�W��===
                //20200907 add by Alton isLastStartDate
                //20200917 edit by Alton �֭p���b�������GLTran�AdebitAmt-creditAmt
                //�֭p�ئb����
                decimal _debitAmt = _data?.DebitAmt ?? 0;
                decimal _creditAmt = _data?.CreditAmt ?? 0;
                decimal _cumCost = isLastStartDate ? (_debitAmt - _creditAmt) : 0;

                //2020/09/08 add by Althea ���FinModsAmt 
                FinModsR = getFinModsAmt("R", _contractID, lastDate);
                FinModsC = getFinModsAmt("C", _contractID, lastDate);

                //�q���ܧ󦬤J�w��
                decimal _totalIncomeModifyAmount = FinModsR ?? 0;//?? GetTotalModifAmount(_contractID, "I", lStartDate, lEndDate);
                //�q���ܧ󦨥��w��
                decimal _totalExpenseModifyAmount = FinModsC ?? 0;//?? GetTotalModifAmount(_contractID, "E", lStartDate, lEndDate);
                //�X����(���|)
                decimal _incomeAmt = totalIncomeCuryAmount + _totalIncomeModifyAmount;
                //�w�p��J����
                decimal _expenseAmt = totalExpenseCuryAmount + _totalExpenseModifyAmount;
                //�u�i = �֭p�ئb����/�w����J����
                decimal _projectProgrees = 0;
                if (_expenseAmt > 0) _projectProgrees = _cumCost / _expenseAmt;
                //�֭p���J = �X���� * �u�i 
                decimal _cumIncome = _incomeAmt * _projectProgrees;


                //�g�J
                temp.CumIncomeThis = Round(cumIncome, 0);
                temp.CumIncomeLast = Round(_cumIncome, 0);
                temp.RecIncomeThis = Round(cumIncome - _cumIncome, 0);

                temp.CumCostThis = Round(cumCost, 0);
                temp.CumCostLast = Round(_cumCost, 0);
                temp.RecCostThis = Round(cumCost - _cumCost, 0);

                //�֭p�Q�� = �֭p���J - �֭p�ئb����
                temp.CumProfitThis = Round(cumIncome - cumCost, 0);
                temp.CumProfitLast = Round(_cumIncome - _cumCost, 0);
                temp.RecProfitThis = temp.CumProfitThis - temp.CumProfitLast;
                //�X����
                temp.IncomeAmt = Round(incomeAmt, 0);
                //��J����
                temp.ExpenseAmt = Round(expenseAmt, 0);
                //�u�i(%)
                temp.ProjectProgrees = Round(projectProgrees * 100, 2);
                //�Q��
                temp.Profit = Round(incomeAmt - expenseAmt, 0);

                //�Q��v(%) = �Q�� / �X����
                temp.ProfitRate = 0;
                if (incomeAmt > 0) temp.ProfitRate = Round(temp.Profit / incomeAmt, 4) * 100;

                (isMonth ? MonthView.Cache : YearView.Cache).Insert(temp);

            }
            SetLabel(isMonth ? MonthView.Cache : YearView.Cache, thisLabel, lastLabel);
        }

        public virtual bool ValidatorData(PXResult<GLTran, Branch, Contract, Account, BAccount> data) {
            return true;
        }

        private void SetLabel(bool isMonth)
        {
            IKG505002 item;
            if (isMonth)
            {
                item = MonthView.Current as KG505002Month;
            }
            else
            {
                item = YearView.Current as KG505002Year;
            }
            if (item == null) return;
            String thisLabel = item.LabelThis;
            String lastLabel = item.LabelLast;
            SetLabel(isMonth ? MonthView.Cache : YearView.Cache, thisLabel, lastLabel);
        }

        private void SetLabel(PXCache cache, string thisDay, string lastDay)
        {
            string thisMonth = thisDay.Substring(0, thisDay.Length - 3);
            PXUIFieldAttribute.SetDisplayName(cache, "contractCD", "�u�a�O");
            PXUIFieldAttribute.SetDisplayName(cache, "contractDesc", "�u�a�W��");
            PXUIFieldAttribute.SetDisplayName(cache, "vendorName", "�~�D");
            PXUIFieldAttribute.SetDisplayName(cache, "cumIncomeLast", lastDay + "�ֿn���J");
            PXUIFieldAttribute.SetDisplayName(cache, "cumIncomeThis", thisDay + "�ֿn���J");
            PXUIFieldAttribute.SetDisplayName(cache, "recIncomeThis", thisMonth + "�i�{�禬");
            PXUIFieldAttribute.SetDisplayName(cache, "cumCostLast", lastDay + "�֭p�b�ئ���");
            PXUIFieldAttribute.SetDisplayName(cache, "cumCostThis", thisDay + "�֭p�b�ئ���");
            PXUIFieldAttribute.SetDisplayName(cache, "recCostThis", thisMonth + "�i�{����");
            PXUIFieldAttribute.SetDisplayName(cache, "cumProfitLast", lastDay + "�֭p�Q��");
            PXUIFieldAttribute.SetDisplayName(cache, "cumProfitThis", thisDay + "�֭p�Q��");
            PXUIFieldAttribute.SetDisplayName(cache, "recProfitThis", thisDay + "���{�Q��");
            PXUIFieldAttribute.SetDisplayName(cache, "profitRate", "�Q��v(%)");
            PXUIFieldAttribute.SetDisplayName(cache, "incomeAmt", "�X����(���|)");
            PXUIFieldAttribute.SetDisplayName(cache, "projectProgrees", "�u�i(%)");
            PXUIFieldAttribute.SetDisplayName(cache, "expenseAmt", "�w���`��J����");
            PXUIFieldAttribute.SetDisplayName(cache, "profit", "�Q��");
        }

        /// <summary>
        /// ���o�_���
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        public virtual DateTime? getFinStartDate(int? projectID)
        {
            return null;
        }

        /// <summary>
        /// ���oFinMods���B
        /// </summary>
        /// <param name="ModsType"></param>
        /// <returns></returns>
        public virtual decimal? getFinModsAmt(String ModsType, int? ProjectID, DateTime? date)
        {
            return null;
        }

        /**
         * <summary>
         * �|�ˤ��J
         * </summary>
         * **/
        private decimal Round(decimal? num, int decimals)
        {
            if (num == null)
            {
                return 0;
            }
            return Math.Round((decimal)num, decimals, MidpointRounding.AwayFromZero);
        }

        #region Query

        #region �p���į���D�w�p���N����}��T
        private PXResultset<Contract> GetContract(int? contractID)
        {
            return PXSelect<Contract,
                Where<Contract.contractID, Equal<IsNull<Optional<Contract.contractID>, Contract.contractID>>>,
                OrderBy<Asc<Contract.contractCD>>>.Select(this, contractID);
        }

        private BAccount GetBAccount(int? customerID)
        {
            return PXSelect<BAccount,
                Where<BAccount.bAccountID, Equal<Required<BAccount.bAccountID>>>>
                .Select(this, customerID);
        }

        private Branch GetBranch(int? branchID)
        {
            return PXSelect<Branch,
                Where<Branch.branchID, Equal<Required<Branch.branchID>>>>
                .Select(this, branchID);
        }

        private Account GetAccount(int? accountID)
        {
            return PXSelect<Account,
                Where<Account.accountID, Equal<Required<Account.accountID>>,
                    And<Account.accountCD, NotEqual<Required<Account.accountCD>>,
                    And<Account.accountCD, Like<Required<Account.accountCD>>>>
                >>
                .Select(this, accountID, "1131.600", "1131%");
        }

        private GLTran GetGLTran(int? projectID, int? branchID)
        {
            return PXSelectGroupBy<GLTran,
                Where<GLTran.projectID, Equal<Required<GLTran.projectID>>,
                    And<GLTran.branchID, Equal<Required<GLTran.branchID>>>
                >,
                Aggregate<
                    GroupBy<GLTran.ledgerID,
                    GroupBy<GLTran.branchID,
                    GroupBy<GLTran.accountID,
                    GroupBy<GLTran.subID>>>>
                    >
                >
                .Select(this, projectID, branchID);
        }

        private GLHistory GetGLHistory(int? ledgerID, int? branchID, int? accountID, int? subID, string finPeriodID)
        {
            return PXSelectGroupBy<GLHistory,
                Where<GLHistory.ledgerID, Equal<Required<GLHistory.ledgerID>>,
                    And<GLHistory.branchID, Equal<Required<GLHistory.branchID>>,
                    And<GLHistory.accountID, Equal<Required<GLHistory.accountID>>,
                    And<GLHistory.subID, Equal<Required<GLHistory.subID>>,
                    And<GLHistory.finPeriodID, Equal<Required<GLHistory.finPeriodID>>>>>>>,
                Aggregate<
                    Sum<GLHistory.curyFinBegBalance,
                    Sum<GLHistory.curyFinYtdBalance>
                >>
                >
                .Select(this, ledgerID, branchID, accountID, subID, finPeriodID);
        }
        #endregion

        /**
         * <summary>
         * ���o�D�n���<br/>
         *  Contract.contractCD<br/>
         *  Contract.description<br/>
         *  Branch.branchCD<br/>
         *  BAccount.acctName<br/>
         * --Sum--<br/>
         *  GLTran.debitAmt<br/>
         *  GLTran.creditAmt<br/>
         * </summary>
         * **/
        private PXResultset<GLTran> GetMainData(int branchID, int? contractID, string periodID)
        {
            return PXSelectJoinGroupBy<
               GLTran,
                   InnerJoin<Branch, On<GLTran.branchID, Equal<Branch.branchID>>,
                   InnerJoin<Contract, On<GLTran.projectID, Equal<Contract.contractID>>,
                   InnerJoin<Account, On<GLTran.accountID, Equal<Account.accountID>>,
                   InnerJoin<BAccount, On<Contract.customerID, Equal<BAccount.bAccountID>>,
                   InnerJoin<Ledger, On<Ledger.ledgerID, Equal<GLTran.ledgerID>>>>>>>,
                   Where2<
                        Where<Account.accountCD, NotEqual<Required<Account.accountCD>>,// <> '1131.600'
                           And<Account.accountCD, Like<Required<Account.accountCD>>,// like '1131%'
                           And<Branch.branchID, Equal<Required<Branch.branchID>>,// = pBranchID
                           And<GLTran.finPeriodID, LessEqual<Required<GLTran.finPeriodID>>,// <= pPeriodID
                           And<Ledger.balanceType, Equal<LedgerBalanceType.actual>,
                           And<GLTran.released, Equal<True>>>>>>>,
                        And<Where<Contract.contractID, Equal<Required<Contract.contractID>>,// = contractID or 0 > contractID
                            Or<Zero, Equal<Required<Contract.contractID>>>
                        >>
                   >,
                   Aggregate<
                       GroupBy<Contract.contractCD,
                       GroupBy<Branch.branchCD,
                       //GroupBy<Contract.description,
                       GroupBy<BAccount.acctName,
                           //Sum<GLHistory.curyFinBegBalance,
                           //Sum<GLHistory.curyFinYtdBalance>>>>>>>,
                           Sum<GLTran.debitAmt,
                           Sum<GLTran.creditAmt>>>>>>,
                   OrderBy<Asc<Contract.contractCD>>
               >.Select(this, "1131.600", "1131%", branchID, periodID, contractID, contractID);

            //List<PXResult<GLHistory, GLTran, PX.Data.PXAccess.Branch, Contract, Account, BAccount>> list = new List<PXResult<GLHistory, GLTran, PX.Data.PXAccess.Branch, Contract, Account, BAccount>>();
            //foreach (Contract c in GetContract(contractID)) {
            //    PX.Data.PXAccess.Branch b = GetBranch(branchID);
            //    BAccount ba = GetBAccount(c.CustomerID);
            //    GLTran gt = GetGLTran(c.ContractID,branchID);
            //    if (gt!=null && b!=null && ba!=null && gt!=null) {
            //        Account a = GetAccount(gt.AccountID);
            //        if (a!=null) {
            //            GLHistory gh = GetGLHistory(gt.LedgerID,branchID,gt.AccountID,gt.SubID,periodID);
            //            PXResult<GLHistory, GLTran, PX.Data.PXAccess.Branch, Contract, Account, BAccount> rs 
            //                = new PXResult<GLHistory, GLTran, PX.Data.PXAccess.Branch, Contract, Account, BAccount>
            //                (gh,gt,b,c,a,ba);
            //            list.Add(rs);
            //        }
            //    }

            //}
            //return list;
        }

        /**
         * <summary>
         * ���o�M�צ��J/�����w��<br/>
         * <paramref name="type"/> I/E(���J/����)<br/>
         *  Contract.contractCD<br/>
         * --Sum--<br/>
         *  PMBudget.curyAmount<br/>
         * </summary>
         * **/
        public virtual decimal GetTotalCuryAmount(int contractID, string type)
        {
            PMBudget pb = PXSelectJoinGroupBy<
                PMBudget,
                    InnerJoin<Contract, On<PMBudget.projectID, Equal<Contract.contractID>>>,
                    Where<PMBudget.type, Equal<Required<PMBudget.type>>,
                        And<PMBudget.projectID, Equal<Required<PMBudget.projectID>>>
                    >,
                    Aggregate<
                        GroupBy<Contract.contractCD,
                        Sum<PMBudget.curyAmount>>
                        >
                >.Select(this, type, contractID);

            if (pb == null || pb.CuryAmount == null)
            {
                return 0;
            }
            return (decimal)pb.CuryAmount;
        }

        /**
        * <summary>
        * ���o�q���ܧ󦬤J/�����w��<br/>
        * <paramref name="type"/> I/E(���J/����)<br/>
        *  Contract.contractCD<br/>
        * --Sum--<br/>
        *  PMChangeOrderBudget.amount<br/>
        * </summary>
        * **/
        private decimal GetTotalModifAmount(int contractID, string type, DateTime? pStartDate, DateTime? pEndDate)
        {
            PMChangeOrderBudget pcob = PXSelectJoinGroupBy<
                PMChangeOrderBudget,
                    InnerJoin<PMChangeOrder, On<PMChangeOrder.projectID, Equal<PMChangeOrderBudget.projectID>,
                        And<PMChangeOrder.refNbr, Equal<PMChangeOrderBudget.refNbr>>>,
                    InnerJoin<Contract, On<PMChangeOrderBudget.projectID, Equal<Contract.contractID>>>
                    >,
                    Where<PMChangeOrderBudget.type, Equal<Required<PMChangeOrderBudget.type>>,//='I'
                        And<PMChangeOrder.projectID, Equal<Required<PMChangeOrder.projectID>>,//=contractID
                        And<PMChangeOrder.status, Equal<Required<PMChangeOrder.status>>,//="C"
                        And<PMChangeOrder.classID, NotEqual<Required<PMChangeOrder.classID>>,//<>'�M�׵���'
                                                                                             //mark 20200723 Alton mantis 11630
                                                                                             // And<PMChangeOrder.date, GreaterEqual<Required<PMChangeOrder.date>>,// >= pStartDate 
                        And<PMChangeOrder.date, Less<Required<PMChangeOrder.date>>>>>>// < pEndDate
                    >,
                    Aggregate<
                        GroupBy<Contract.contractCD,
                        Sum<PMChangeOrderBudget.amount>>
                        >
                >.Select(this, type, contractID, "C", "�M�׵���",
                // pStartDate,
                pEndDate);

            if (pcob == null || pcob.Amount == null)
            {
                return 0;
            }
            return (decimal)pcob.Amount;
        }

        /**
         * <summary>
         * ���o������T
         * </summary>
         * **/
        private FinPeriod GetFinPeriod(int branchID, string finPeriodID)
        {
            return PXSelectJoin<FinPeriod,
                     InnerJoin<Branch, On<Branch.organizationID, Equal<FinPeriod.organizationID>>>,
                     Where<Branch.branchID, Equal<Required<Branch.branchID>>,
                     And<FinPeriod.finPeriodID, Equal<Required<FinPeriod.finPeriodID>>>>
                 >.Select(this, branchID, finPeriodID);
        }

        private Contract GetContractByCD(string contractCD)
        {
            return PXSelect<Contract, Where<Contract.contractCD, Equal<Required<Contract.contractCD>>>>
                .Select(new PXGraph(), contractCD);
        }
        #endregion

        #endregion





        #region filter
        [Serializable]
        public class KG505002Filter : IBqlTable
        {

            #region BranchID
            [PXInt]
            [PXUIField(DisplayName = "Branch ID", Required = true)]
            [PXUnboundDefault(typeof(AccessInfo.branchID))]
            [PXSelector(typeof(
                Search2<
                    Branch.branchID,
                    InnerJoin<BAccount, On<BAccount.bAccountID, Equal<Branch.bAccountID>>>,
                    Where<Branch.active, Equal<pActive>>,
                    OrderBy<Asc<Branch.branchCD>>>),
                typeof(Branch.branchCD),
                typeof(BAccount.acctName),
                SubstituteKey = typeof(Branch.branchCD),
                DescriptionField = typeof(BAccount.acctName)
                       )]
            public virtual int? BranchID { get; set; }
            public abstract class branchID : PX.Data.BQL.BqlInt.Field<branchID> { }
            #endregion

            #region ContractCD
            [PXString]
            [PXUIField(DisplayName = "Contract CD")]
            [PXSelector(typeof(
                Search<
                    Contract.contractCD,
                    Where<Contract.status, NotEqual<pStatus>,
                        And<Contract.defaultBranchID, Equal<Current<KG505002Filter.branchID>>>>,
                    OrderBy<
                        Asc<Contract.contractCD>>>
                ),
                typeof(Contract.contractCD),
                typeof(Contract.description),
                SubstituteKey = typeof(Contract.contractCD),
                DescriptionField = typeof(Contract.description),
                ValidateValue = false
                       )]
            public virtual String ContractCD { get; set; }
            public abstract class contractCD : PX.Data.BQL.BqlString.Field<contractCD> { }

            //[PXString]
            //[PXUIField(DisplayName = "Contract CD")]
            //[PXSelector(typeof(
            //   Search<
            //       Contract.contractCD,
            //       Where<Contract.status, NotEqual<pStatus>,
            //           And<Contract.defaultBranchID, Equal<Current<KG505002Filter.branchID>>>>,
            //       OrderBy<
            //           Asc<Contract.contractCD>>>
            //   ),
            //    typeof(Contract.contractCD),
            //    typeof(Contract.description),
            //    SubstituteKey = typeof(Contract.contractCD),
            //    DescriptionField = typeof(Contract.description),
            //    ValidateValue = false
            //          )]
            //public virtual String ContractID { get; set; }
            //public abstract class contractID : PX.Data.BQL.BqlString.Field<contractID> { }
            #endregion

            #region Period
            [PXString]
            [PX.Objects.GL.FinPeriodSelector(typeof(
                Search2<FinPeriod.finPeriodID,
                    InnerJoin<Branch, On<Branch.organizationID, Equal<FinPeriod.organizationID>>>,
                    Where<
                        FinPeriod.startDate, GreaterEqual<Current<KG505002Filter.beginDate>>,
                            And<FinPeriod.endDate, LessEqual<Current<KG505002Filter.endDate>>>>,
                    OrderBy<Desc<FinPeriod.finPeriodID>>>),
            typeof(AccessInfo.businessDate),
            branchSourceType: typeof(KG505002Filter.branchID),
            redefaultOrRevalidateOnOrganizationSourceUpdated: false)]
            //[PXSelector(typeof(
            //    Search2<FinPeriod.finPeriodID,
            //        InnerJoin<Branch, On<Branch.organizationID, Equal<FinPeriod.organizationID>>>,
            //        Where<Branch.branchID, Equal<Current<KG505002Filter.branchID>>,
            //            And<FinPeriod.startDate, GreaterEqual<Current<KG505002Filter.beginDate>>,
            //                And<FinPeriod.endDate, LessEqual<Current<KG505002Filter.endDate>>>>>,
            //        OrderBy<Desc<FinPeriod.finPeriodID>>>),
            //    typeof(FinPeriod.finPeriodID),
            //    typeof(FinPeriod.descr),
            //    typeof(FinPeriod.startDate),
            //    typeof(FinPeriod.endDate)
            //    )]
            [PXUIField(DisplayName = "Period", Required = true, Visibility = PXUIVisibility.Visible)]
            public virtual string Period { get; set; }
            public abstract class period : PX.Data.BQL.BqlString.Field<period> { }
            #endregion

            #region BeginDate
            [PXDate]
            public virtual DateTime? BeginDate { get; set; }
            public abstract class beginDate : PX.Data.BQL.BqlDateTime.Field<beginDate> { }
            #endregion

            #region EndDate
            [PXDate]
            public virtual DateTime? EndDate { get; set; }
            public abstract class endDate : PX.Data.BQL.BqlDateTime.Field<endDate> { }
            #endregion

        }
        #endregion
    }
}