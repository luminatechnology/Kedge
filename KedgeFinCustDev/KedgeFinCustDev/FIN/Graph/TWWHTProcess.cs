using System;
using PX.Data;
using Fin.DAC;
using FIN.Util;
using RC.Util;

namespace Fin
{
    public class TWWHTProcess : PXGraph<TWWHTProcess>
    {
        #region Select View
        public PXFilter<TWNWHTProcessFilter> Filters;

        public PXSelect<TWNWHTTran,
            Where<TWNWHTTran.tranType,Equal<FinStringList.TranType.wht>,
                And2<Where<TWNWHTTran.personalID, Equal<Current2<TWNWHTProcessFilter.personalID>>,
                    Or<Current2<TWNWHTProcessFilter.personalID>,IsNull>>,
                    And2<Where<
                        DatePart<DatePart.year, TWNWHTTran.paymentDate>,
                        GreaterEqual<Current2<TWNWHTProcessFilter.declareYearFrom>>,
                          Or<Current2<TWNWHTProcessFilter.declareYearFrom>, IsNull>>,
                    And2<Where<
                          DatePart<DatePart.year, TWNWHTTran.paymentDate>,
                        LessEqual<Current2<TWNWHTProcessFilter.declareYearTo>>,
                          Or<Current2<TWNWHTProcessFilter.declareYearTo>, IsNull>>,
                    And2<Where<
                        DatePart<DatePart.month, TWNWHTTran.paymentDate>,
                        GreaterEqual<Current2<TWNWHTProcessFilter.declareMonthFrom>>,
                          Or<Current2<TWNWHTProcessFilter.declareMonthFrom>, IsNull>>,
                    And<Where<
                          DatePart<DatePart.month, TWNWHTTran.paymentDate>,
                        LessEqual<Current2<TWNWHTProcessFilter.declareMonthTo>>,
                          Or<Current2<TWNWHTProcessFilter.declareMonthTo>, IsNull>>>>>>>>> WHTView;

        #endregion

        #region Hyper Link
        public PXAction<TWNWHTProcessFilter> ViewPersonalID;
        [PXButton(CommitChanges = true)]
        [PXUIField(Visible = false)]
        protected virtual void viewPersonalID()
        {
            TWNWHTTran tran = WHTView.Current;
            TWWHTMaint graph = PXGraph.CreateInstance<TWWHTMaint>();
            TWNWHTFilter filter = graph.Filter.Current;
            filter.PersonalID = tran.PersonalID;
            graph.Filter.Current =graph.Filter.Update(filter);

            new HyperLinkUtil<TWWHTMaint>(graph.Filter.Current, true);
        }

        #endregion

        #region Events 
        protected virtual void _(Events.RowSelected<TWNWHTProcessFilter> e)
        {
            TWNWHTProcessFilter filter = e.Row;
            if (filter == null) return;
            setDetailsUI();
        }
        protected virtual void _(Events.FieldUpdated<TWNWHTProcessFilter, TWNWHTProcessFilter.personalID> e)
        {
            TWNWHTProcessFilter filter = e.Row;
            if (filter == null) return;
            if (filter.PersonalID != null)
            {
                TWNWHTTran tran = getTWNWHTTran(filter.PersonalID);
                filter.PayeeName = tran?.PayeeName;
            }
        }
        #endregion

        #region Method 
        private void setDetailsUI()
        {
            TWNWHTTran tran = WHTView.Current;
            PXUIFieldAttribute.SetEnabled(WHTView.Cache, null, false);
            PXUIFieldAttribute.SetEnabled<TWNWHTTran.selected>(WHTView.Cache, null, true);

        }
        #endregion

        #region Select Method 
        private TWNWHTTran getTWNWHTTran(string PersonalID)
        {
            return PXSelect<TWNWHTTran,
                Where<TWNWHTTran.personalID, Equal<Required<TWNWHTTran.personalID>>>>
                .Select(this, PersonalID);
        }
        #endregion
    }
    #region Filter Table
    [Serializable]
    public class TWNWHTProcessFilter : IBqlTable
    {
        #region PersonalID
        [PXString(10, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Personal Tax ID")]
        [PXSelector(typeof(Search4<TWNWHTTran.personalID,
            Where<TWNWHTTran.personalID, IsNotNull>,
            Aggregate<GroupBy<TWNWHTTran.personalID>>>),
            typeof(TWNWHTTran.personalID),
            typeof(TWNWHTTran.payeeName))]

        public virtual string PersonalID { get; set; }
        public abstract class personalID : PX.Data.BQL.BqlString.Field<personalID> { }
        #endregion

        #region PayeeName
        [PXString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Payee Name", IsReadOnly = true)]
        [PXUnboundDefault(typeof(Search<TWNWHTTran.payeeName,
            Where<TWNWHTTran.personalID, Equal<Current<personalID>>>>))]
        public virtual string PayeeName { get; set; }
        public abstract class payeeName : PX.Data.BQL.BqlString.Field<payeeName> { }
        #endregion

        #region TotalWHTAmt 代扣稅總額
        [PXDecimal(0)]
        [PXUIField(DisplayName = "Total WHT Amount", IsReadOnly = true)]
        [PXUnboundDefault]
        public virtual decimal? TotalWHTAmt { get; set; }
        public abstract class totalWHTAmt : PX.Data.BQL.BqlDecimal.Field<totalWHTAmt> { }
        #endregion

        #region TotalGNHI2Amt 代扣二代健保總額
        [PXDecimal(0)]
        [PXUIField(DisplayName = "Total 2GNHI Amount", IsReadOnly = true)]
        [PXUnboundDefault]
        public virtual decimal? TotalGNHI2Amt { get; set; }
        public abstract class totalGNHI2Amt : PX.Data.BQL.BqlDecimal.Field<totalGNHI2Amt> { }
        #endregion

        #region DeclareYearFrom
        [PXInt()]
        [PXUIField(DisplayName = "Declare Year From")]
        [PXDefault]
        public virtual int? DeclareYearFrom { get; set; }
        public abstract class declareYearFrom : PX.Data.BQL.BqlInt.Field<declareYearFrom> { }
        #endregion

        #region DeclareYearTo
        [PXInt()]
        [PXUIField(DisplayName = "Declare Year To")]
        [PXDefault]
        public virtual int? DeclareYearTo { get; set; }
        public abstract class declareYearTo : PX.Data.BQL.BqlInt.Field<declareYearTo> { }
        #endregion

        #region DeclareMonthFrom
        [PXInt()]
        [PXUIField(DisplayName = "Declare Month From")]
        [PXDefault]
        public virtual int? DeclareMonthFrom { get; set; }
        public abstract class declareMonthFrom : PX.Data.BQL.BqlInt.Field<declareMonthFrom> { }
        #endregion

        #region DeclareMonthTo
        [PXInt()]
        [PXUIField(DisplayName = "Declare Month To")]
        [PXDefault()]
        public virtual int? DeclareMonthTo { get; set; }
        public abstract class declareMonthTo : PX.Data.BQL.BqlInt.Field<declareMonthTo> { }
        #endregion
    }
    #endregion
}