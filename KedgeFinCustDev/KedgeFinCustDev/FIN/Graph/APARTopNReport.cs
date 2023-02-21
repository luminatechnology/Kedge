using System;
using KedgeFinCustDev.FIN.DAC;
using PX.Data;
using PX.Objects.GL;
using System.Collections.Generic;
using PX.Common;
using static PX.Data.PXBaseRedirectException;

namespace KedgeFinCustDev.FIN.Graph
{
    public class APARTopNReport : PXGraph<APARTopNReport>
    {

        public PXCancel<TopNFilter> Cancel;

        #region View
        public PXFilter<TopNFilter> Filter;
        public PXSelect<APARTopNTemp> DetailsView;
        #endregion

        #region Button
        public PXAction<TopNFilter> ProcessAction;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "執行報表")]
        protected void processAction()
        {
            if (!Validate()) return;
            DateTime businessDate = (DateTime)Accessinfo.BusinessDate;
            DeleteOldData(businessDate, 7);
            TopNFilter filter = Filter.Current;
            string batchNbr = GetBatchNbr(this);
            DateTime dateFrom = new DateTime(filter.InvYear ?? 0, filter.InvMonthFrom ?? 0, 1);
            DateTime dateTo = new DateTime(filter.InvYear ?? 0, filter.InvMonthTo ?? 0, 1).AddMonths(1).AddDays(-1);

            #region Get BulidTotal
            string format = "{0}{1:00}";
            string finPeriodIDFrom = String.Format(format, filter.InvYear, filter.InvMonthFrom);
            string finPeriodIDTo = String.Format(format, filter.InvYear, filter.InvMonthTo);
            string likeAccountCD = "AP".Equals(filter.Type) ? "5104%" : "4104%";
            //GLTran sum = SumGLTran(filter.BranchID, finPeriodIDFrom, finPeriodIDTo, likeAccountCD);
            //decimal bulidTotal = (sum.CuryDebitAmt ?? 0m) - (sum.CuryCreditAmt ?? 0m);
            APARTopNV sum = GetSumDataByYear(filter.BranchID, filter.Type, filter.InvYear);
            decimal bulidTotal = (sum.SalesAmt ?? 0) - (sum.CmAmt ?? 0);
            #endregion

            using (PXTransactionScope ts = new PXTransactionScope())
            {
                PXResultset<APARTopNV> datas = GetData(filter.BranchID, dateFrom, dateTo, filter.Type);
                List<APARTopNTemp> list = new List<APARTopNTemp>();
                foreach (APARTopNV data in datas)
                {
                    list.Add(new APARTopNTemp()
                    {
                        BatchNbr = batchNbr,
                        UniformNumber = data.CustUniformNumber,
                        Name = data.CustName,
                        Amount = data.SalesAmt - data.CmAmt,
                        BuildTotalAmt = bulidTotal,
                        Type = data.Type
                    });
                }
                //降慕
                list.Sort((x, y) => -(x.Amount ?? 0m).CompareTo(y.Amount ?? 0m));
                int seq = 1;
                foreach (var item in list) {
                    if (seq > (filter.Top ?? 10)) break;
                    item.Seq = seq++;
                    DetailsView.Insert(item);
                }

                Persist();
                ts.Complete();
            }

            //產生ReportData
            //傳入報表參數
            Dictionary<string, string> patams = new Dictionary<string, string>();
            patams["BatchNbr"] = batchNbr;
            throw new PXReportRequiredException(parameters: patams, reportID: "GV603005", newWindow: WindowMode.New, "Report");
        }
        #endregion

        #region Event
        //protected virtual void _(Events.FieldDefaulting<TopNFilter, TopNFilter.invYearFrom> e)
        //{
        //    e.NewValue = Accessinfo.BusinessDate?.Year;
        //}

        protected virtual void _(Events.FieldDefaulting<TopNFilter, TopNFilter.invYear> e)
        {
            e.NewValue = Accessinfo.BusinessDate?.Year;
        }

        protected virtual void _(Events.FieldDefaulting<TopNFilter, TopNFilter.invMonthFrom> e)
        {
            e.NewValue = Accessinfo.BusinessDate?.Month;
        }

        protected virtual void _(Events.FieldDefaulting<TopNFilter, TopNFilter.invMonthTo> e)
        {
            e.NewValue = Accessinfo.BusinessDate?.Month;
        }
        #endregion

        #region Method
        private bool Validate()
        {
            TopNFilter filter = Filter.Current;
            bool flag = true;
            if (filter.BranchID == null) flag = SetError<TopNFilter.branchID>(filter, filter.BranchID, "請輸入");
            //if (filter.InvYearFrom == null) flag = SetError<TopNFilter.invYearFrom>(filter, filter.InvYearFrom, "請輸入");
            if (filter.InvYear == null) flag = SetError<TopNFilter.invYear>(filter, filter.InvYear, "請輸入");
            if (filter.InvMonthFrom == null) flag = SetError<TopNFilter.invMonthFrom>(filter, filter.InvMonthFrom, "請輸入");
            if (filter.InvMonthTo == null) flag = SetError<TopNFilter.invMonthTo>(filter, filter.InvMonthTo, "請輸入");
            if (filter.Top == null) flag = SetError<TopNFilter.top>(filter, filter.Top, "請輸入");
            if (filter.Type == null) flag = SetError<TopNFilter.type>(filter, filter.Type, "請輸入");
            return flag;
        }

        private bool SetError<Field>(object row, object newValue, String errorMsg, PXErrorLevel errorLevel = PXErrorLevel.Error) where Field : PX.Data.IBqlField
        {
            Filter.Cache.RaiseExceptionHandling<Field>(row, newValue,
                  new PXSetPropertyException(errorMsg, errorLevel));
            return false;
        }

        private readonly object lockObj = new object();
        private string GetBatchNbr(APARTopNReport graph)
        {
            lock (lockObj)
            {
                var sysDate = PXTimeZoneInfo.Now;
                string batchNbr;
                while (true)
                {
                    batchNbr = sysDate.ToString("yyyyMMddHHmmss");
                    APARTopNTemp temp = PXSelect<APARTopNTemp, Where<APARTopNTemp.batchNbr, Equal<Required<APARTopNTemp.batchNbr>>>>.Select(new PXGraph(), batchNbr);
                    if (temp == null) break;
                }
                return batchNbr;
            }
        }

        /**
         * <summary>
         * 刪除舊資料
         * </summary>
         * <param name="date">當前時間</param>
         * <param name="day">預刪除幾天前的資料</param>
         */
        private void DeleteOldData(DateTime date, int day)
        {
            DateTime deadLine = date.AddDays(-day);
            PXResultset<APARTopNTemp> set = PXSelect<APARTopNTemp,
                Where<APARTopNTemp.createdDateTime, LessEqual<Required<APARTopNTemp.createdDateTime>>>>
                .Select(this, deadLine);
            foreach (APARTopNTemp old in set)
            {
                DetailsView.Delete(old);
            }
            base.Persist();
        }
        #endregion

        #region BQL
        private PXResultset<APARTopNV> GetData(int? branchID, DateTime dateFrom, DateTime dateTo, string type)
        {
            return PXSelectGroupBy<APARTopNV, Where<APARTopNV.branchID, Equal<Required<APARTopNV.branchID>>,
                And<APARTopNV.invDate, GreaterEqual<Required<APARTopNV.invDate>>,
                And<APARTopNV.invDate, LessEqual<Required<APARTopNV.invDate>>,
                And<APARTopNV.type, Equal<Required<APARTopNV.type>>>>>>,
                Aggregate<
                    GroupBy<APARTopNV.custUniformNumber,
                    GroupBy<APARTopNV.custName,
                    Sum<APARTopNV.salesAmt,
                    Sum<APARTopNV.cmAmt>>>>>
                >.Select(this, branchID, dateFrom, dateTo, type);
        }

        private PXResultset<APARTopNV> GetSumDataByYear(int? branchID, string type, int? year)
        {
            return PXSelectGroupBy<APARTopNV, Where<APARTopNV.branchID, Equal<Required<APARTopNV.branchID>>,
                And<DatePart<DatePart.year, APARTopNV.invDate>, Equal<Required<TopNFilter.invYear>>,
                And<APARTopNV.type, Equal<Required<APARTopNV.type>>>>>,
                Aggregate<
                    Sum<APARTopNV.salesAmt,
                    Sum<APARTopNV.cmAmt>>>
                >.Select(this, branchID, year, type);
        }

        private GLTran SumGLTran(int? branchID, string finPeriodIDFrom, string finPeriodIDTo, string likeAccountCD)
        {
            return PXSelectJoinGroupBy<GLTran,
                InnerJoin<Batch, On<GLTran.batchNbr, Equal<Batch.batchNbr>,
                    And<GLTran.ledgerID, Equal<Batch.ledgerID>,
                    And<GLTran.module, Equal<Batch.module>>>>,
                InnerJoin<Account, On<GLTran.accountID, Equal<Account.accountID>>,
                InnerJoin<Ledger, On<GLTran.ledgerID, Equal<Ledger.ledgerID>>>>>,
                Where<GLTran.released, Equal<True>,
                    And<GLTran.posted, Equal<True>,
                    And<Batch.branchID, Equal<Required<GLTran.branchID>>,
                    And<Batch.finPeriodID, GreaterEqual<Required<Batch.finPeriodID>>,
                    And<Batch.finPeriodID, LessEqual<Required<Batch.finPeriodID>>,
                    And<Account.accountCD, Like<Required<Account.accountCD>>,
                    And<Ledger.ledgerCD, Equal<Required<Ledger.ledgerCD>>
                >>>>>>>,
                Aggregate<Sum<GLTran.curyDebitAmt, Sum<GLTran.curyCreditAmt>>>
                >.Select(this, branchID, finPeriodIDFrom, finPeriodIDTo, likeAccountCD, "ACTUAL");

        }
        #endregion

        #region Table
        [Serializable]
        [PXCacheName("TopNFilter")]
        public class TopNFilter : IBqlTable
        {
            #region BranchID
            [PXInt()]
            [PXUIField(DisplayName = "公司別", Required = true)]
            [PXSelector(typeof(Search<Branch.branchID, Where<Branch.active, Equal<True>>>),
                            typeof(Branch.branchCD),
                            typeof(Branch.acctName),
                          SubstituteKey = typeof(Branch.branchCD),
                          DescriptionField = typeof(Branch.acctName))]
            [PXUnboundDefault(typeof(AccessInfo.branchID))]
            public virtual int? BranchID { get; set; }
            public abstract class branchID : PX.Data.BQL.BqlInt.Field<branchID> { }
            #endregion

            //#region InvYearFrom
            //[PXInt(MinValue = 1911)]
            //[PXUIField(DisplayName = "發票輸入年(起)", Required = true)]
            //[PXUnboundDefault]
            //public virtual int? InvYearFrom { get; set; }
            //public abstract class invYearFrom : PX.Data.BQL.BqlInt.Field<invYearFrom> { }
            //#endregion

            #region InvYear
            [PXInt(MinValue = 1911)]
            [PXUIField(DisplayName = "發票輸入年", Required = true)]
            [PXUnboundDefault]
            public virtual int? InvYear { get; set; }
            public abstract class invYear : PX.Data.BQL.BqlInt.Field<invYear> { }
            #endregion

            #region InvMonthFrom
            [PXInt(MinValue = 1, MaxValue = 12)]
            [PXUIField(DisplayName = "發票輸入月(起)", Required = true)]
            [PXUnboundDefault]
            public virtual int? InvMonthFrom { get; set; }
            public abstract class invMonthFrom : PX.Data.BQL.BqlInt.Field<invMonthFrom> { }
            #endregion

            #region InvMonthTo
            [PXInt(MinValue = 1, MaxValue = 12)]
            [PXUIField(DisplayName = "發票輸入月(訖)", Required = true)]
            [PXUnboundDefault]
            public virtual int? InvMonthTo { get; set; }
            public abstract class invMonthTo : PX.Data.BQL.BqlInt.Field<invMonthTo> { }
            #endregion

            #region Top
            [PXInt(MinValue = 1)]
            [PXUIField(DisplayName = "排名數", Required = true)]
            [PXUnboundDefault(10)]
            public virtual int? Top { get; set; }
            public abstract class top : PX.Data.BQL.BqlInt.Field<top> { }
            #endregion

            #region Type
            [PXString(IsUnicode = true)]
            [PXUIField(DisplayName = "類型", Required = true)]
            [PXUnboundDefault("AP")]
            [PXStringList(
                new String[] { "AP", "AR" },
                new String[] { "進項", "銷項" }
                )]
            public virtual string Type { get; set; }
            public abstract class type : PX.Data.BQL.BqlString.Field<type> { }
            #endregion
        }

        #endregion
    }
}