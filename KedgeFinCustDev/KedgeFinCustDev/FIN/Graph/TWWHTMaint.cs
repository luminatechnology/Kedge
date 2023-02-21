using System;
using PX.Data;
using Fin.DAC;
using FIN.Util;
using System.Collections;
using RC.Util;
using PX.Objects.AP;
using PX.Objects.EP;

namespace Fin
{
    public class TWWHTMaint : PXGraph<TWWHTMaint>
    {
        #region View Select
        public PXSave<TWNWHTFilter> Save;
        public PXCancel<TWNWHTFilter> Cancel;
        public PXFilter<TWNWHTFilter> Filter;
        public PXSelect<TWNWHTTran, 
            Where<
                TWNWHTTran.personalID, Equal<Current2<TWNWHTFilter.personalID>>,
                    And2<Where<
                        DatePart<DatePart.year,TWNWHTTran.paymentDate>, 
                        GreaterEqual<Current2<TWNWHTFilter.declareYearFrom>>,
                          Or<Current2<TWNWHTFilter.declareYearFrom>, IsNull>>,
                    And2<Where<
                          DatePart<DatePart.year, TWNWHTTran.paymentDate>,
                        LessEqual<Current2<TWNWHTFilter.declareYearTo>>,
                          Or<Current2<TWNWHTFilter.declareYearTo>, IsNull>>,
                    And2<Where<
                        DatePart<DatePart.month, TWNWHTTran.paymentDate>,
                        GreaterEqual<Current2<TWNWHTFilter.declareMonthFrom>>,
                          Or<Current2<TWNWHTFilter.declareMonthFrom>, IsNull>>,
                    And<Where<
                          DatePart<DatePart.month, TWNWHTTran.paymentDate>,
                        LessEqual<Current2<TWNWHTFilter.declareMonthTo>>,
                          Or<Current2<TWNWHTFilter.declareMonthTo>, IsNull>>>>>>>> WHTView;

        #endregion

        #region HyperLink
        
        public PXAction<TWNWHTFilter> ViewAP;
        [PXButton(CommitChanges = true)]
        [PXUIField(Visible = false)]
        protected virtual void viewAP()
        {
            TWNWHTTran tran = WHTView.Current;
            APInvoiceEntry graph = PXGraph.CreateInstance<APInvoiceEntry>();
            graph.Document.Current = graph.Document.Search<APInvoice.refNbr>
                (tran.RefNbr,new object[] {tran.DocType });
            
            new HyperLinkUtil<APInvoiceEntry>(graph.Document.Current, true);
        }

        public PXAction<TWNWHTFilter> ViewEP;
        [PXButton(CommitChanges = true)]
        [PXUIField(Visible = false)]
        protected virtual void viewEP()
        {
            TWNWHTTran tran = WHTView.Current;
            ExpenseClaimEntry graph = PXGraph.CreateInstance<ExpenseClaimEntry>();
            graph.ExpenseClaim.Current = graph.ExpenseClaim.Search<EPExpenseClaim.refNbr>
                (tran.RefNbr);
            new HyperLinkUtil<APInvoiceEntry>(graph.ExpenseClaim.Current, true);
        }
        
        #endregion

        #region Event

        #region Filter
        protected void _(Events.FieldDefaulting<TWNWHTFilter,TWNWHTFilter.declareYearFrom> e)
        {
            TWNWHTFilter filter = e.Row;
            if (filter == null) return;
            DateTime today = (DateTime)base.Accessinfo.BusinessDate;
            int? Year = today.Year;
            filter.DeclareYearFrom = Year;
        }
        protected void _(Events.FieldDefaulting<TWNWHTFilter, TWNWHTFilter.declareYearTo> e)
        {
            TWNWHTFilter filter = e.Row;
            if (filter == null) return;
            DateTime today = (DateTime)base.Accessinfo.BusinessDate;
            int? Year = today.Year;
            filter.DeclareYearTo = Year;
        }
        protected void _(Events.FieldDefaulting<TWNWHTFilter, TWNWHTFilter.declareMonthFrom> e)
        {
            TWNWHTFilter filter = e.Row;
            if (filter == null) return;
            DateTime today = (DateTime)base.Accessinfo.BusinessDate;
            int? Month = today.Month;
            filter.DeclareMonthFrom = Month;
        }
        protected void _(Events.FieldDefaulting<TWNWHTFilter, TWNWHTFilter.declareMonthTo> e)
        {
            TWNWHTFilter filter = e.Row;
            if (filter == null) return;
            DateTime today = (DateTime)base.Accessinfo.BusinessDate;
            int? Month = today.Month;
            filter.DeclareMonthTo = Month;
        }
        protected void _(Events.FieldDefaulting<TWNWHTFilter,TWNWHTFilter.totalWHTAmt> e)
        {
            if (e.Row == null) return;
            TWNWHTFilter filter = e.Row as TWNWHTFilter;
            e.NewValue = CalAmt(filter, FinStringList.TranType.WHT,false);
        }
        protected void _(Events.FieldDefaulting<TWNWHTFilter,TWNWHTFilter.totalGNHI2Amt> e)
        {
            if (e.Row == null) return;
            TWNWHTFilter filter = e.Row as TWNWHTFilter;           
            e.NewValue = CalAmt(filter,FinStringList.TranType.GHI,false); 
        }
        protected void _(Events.FieldUpdated<TWNWHTFilter,TWNWHTFilter.personalID> e)
        {
            TWNWHTFilter filter = e.Row;
            if (filter == null) return;
            if (filter.PersonalID == null) filter.PayeeName = null;
            else
            {
                TWNWHTTran tran = getWHTTranByPersonalID(filter.PersonalID);
                filter.PayeeName = tran?.PayeeName;
            }

            Filter.Cache.SetDefaultExt<TWNWHTFilter.totalGNHI2Amt>(filter);
            Filter.Cache.SetDefaultExt<TWNWHTFilter.totalWHTAmt>(filter);
        }
        protected void _(Events.FieldUpdated<TWNWHTFilter,TWNWHTFilter.declareYearFrom> e)
        {
            if (e.Row == null) return;
            TWNWHTFilter filter = e.Row;
            Filter.Cache.SetDefaultExt<TWNWHTFilter.totalGNHI2Amt>(filter);
            Filter.Cache.SetDefaultExt<TWNWHTFilter.totalWHTAmt>(filter);
           // filter.TotalWHTAmt =CalAmt(filter, FinStringList.TranType.WHT,true); 
            //filter.TotalGNHI2Amt =CalAmt(filter, FinStringList.TranType.GHI,true);
        }
        protected void _(Events.FieldUpdated<TWNWHTFilter,TWNWHTFilter.declareYearTo> e)
        {
            if (e.Row == null) return;
            TWNWHTFilter filter = e.Row;
            Filter.Cache.SetDefaultExt<TWNWHTFilter.totalGNHI2Amt>(filter);
            Filter.Cache.SetDefaultExt<TWNWHTFilter.totalWHTAmt>(filter);
            //filter.TotalWHTAmt = CalAmt(filter, FinStringList.TranType.WHT, true);
            //filter.TotalGNHI2Amt = CalAmt(filter, FinStringList.TranType.GHI, true);
        }
        protected void _(Events.FieldUpdated<TWNWHTFilter,TWNWHTFilter.declareMonthFrom> e)
        {
            if (e.Row == null) return;
            TWNWHTFilter filter = e.Row;
            Filter.Cache.SetDefaultExt<TWNWHTFilter.totalGNHI2Amt>(filter);
            Filter.Cache.SetDefaultExt<TWNWHTFilter.totalWHTAmt>(filter);
            //filter.TotalWHTAmt = CalAmt(filter, FinStringList.TranType.WHT, true);
            //filter.TotalGNHI2Amt = CalAmt(filter, FinStringList.TranType.GHI, true);
        }
        protected void _(Events.FieldUpdated<TWNWHTFilter,TWNWHTFilter.declareMonthTo> e)
        {
            if (e.Row == null) return;
            TWNWHTFilter filter = e.Row ;
            Filter.Cache.SetDefaultExt<TWNWHTFilter.totalGNHI2Amt>(filter);
            Filter.Cache.SetDefaultExt<TWNWHTFilter.totalWHTAmt>(filter);
            //filter.TotalWHTAmt = CalAmt(filter, FinStringList.TranType.WHT, true);
            //filter.TotalGNHI2Amt = CalAmt(filter, FinStringList.TranType.GHI, true);
        }

        #endregion

        #region WHTTran
        protected void _(Events.RowSelected<TWNWHTTran> e)
        {
            if (e.Row == null) return;
            TWNWHTFilter filter = Filter.Current;
            setDetalUI();

        }

        #endregion

        #endregion

        #region Method
        public void setDetalUI()
        {
            TWNWHTTran tran = WHTView.Current;
            PXUIFieldAttribute.SetEnabled<TWNWHTTran.refNbr>(WHTView.Cache, null, false);
            PXUIFieldAttribute.SetEnabled<TWNWHTTran.ePRefNbr>(WHTView.Cache, null, false);

            PXUIFieldAttribute.SetReadOnly<TWNWHTTran.payAmt>(WHTView.Cache, null, true);
            PXUIFieldAttribute.SetReadOnly<TWNWHTTran.payAmt>(WHTView.Cache, null, true);
            PXUIFieldAttribute.SetReadOnly<TWNWHTTran.wHTAmt>(WHTView.Cache, null, true);
            PXUIFieldAttribute.SetReadOnly<TWNWHTTran.netAmt>(WHTView.Cache, null, true);
            PXUIFieldAttribute.SetReadOnly<TWNWHTTran.gNHI2Amt>(WHTView.Cache, null, true);




        }
        public decimal? CalAmt(TWNWHTFilter filter, string TranType, bool hasSelected)
        {
            decimal? TotalWHTAmt = 0;
            decimal? TotalGhi2Amt = 0;
            if(hasSelected)
            {
                foreach (TWNWHTTran tran in WHTView.Select())
                {
                    TotalGhi2Amt = TotalGhi2Amt + tran.GNHI2Amt;
                    TotalWHTAmt = TotalWHTAmt + tran.WHTAmt;
                }
            }
            else
            {
                PXResultset<TWNWHTTran> transet = getWHTTran(filter);
                foreach (TWNWHTTran tran in transet)
                {
                    TotalGhi2Amt = TotalGhi2Amt + tran.GNHI2Amt;
                    TotalWHTAmt = TotalWHTAmt + tran.WHTAmt;
                }
            }
            

            switch (TranType)
            {
                case FinStringList.TranType.WHT:
                    return TotalWHTAmt;
                    break;
                case FinStringList.TranType.GHI:
                    return TotalGhi2Amt;
                    break;
            }
            return 0m;

        }

        #endregion

        #region Select Method
        private PXResultset<TWNWHTTran> getWHTTran(TWNWHTFilter filter)
        {
            return PXSelect<TWNWHTTran,
                Where<TWNWHTTran.personalID, Equal<Required<TWNWHTFilter.personalID>>,
                And<DatePart<DatePart.year, TWNWHTTran.paymentDate>,
                    GreaterEqual<Required<TWNWHTFilter.declareYearFrom>>,
                And<DatePart<DatePart.year, TWNWHTTran.paymentDate>,
                    LessEqual<Required<TWNWHTFilter.declareYearTo>>,
                And<DatePart<DatePart.month, TWNWHTTran.paymentDate>,
                    GreaterEqual<Required<TWNWHTFilter.declareYearFrom>>,
                And<DatePart<DatePart.month, TWNWHTTran.paymentDate>,
                    LessEqual<Required<TWNWHTFilter.declareYearTo>>>>>>>>
                .Select(this, filter.PersonalID, filter.DeclareYearFrom, filter.DeclareYearTo,
                            filter.DeclareMonthFrom, filter.DeclareMonthTo);

        }

        private TWNWHTTran getWHTTranByPersonalID(string PersonalID)
        {
            return PXSelect<TWNWHTTran,
                Where<TWNWHTTran.personalID, Equal<Required<TWNWHTTran.personalID>>>>
                .Select(this, PersonalID);
        }
        #endregion
    }


    #region Filter Table
    [Serializable]
    public class TWNWHTFilter : IBqlTable
    {
        #region PersonalID
        [PXString(10, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Personal Tax ID")]
        [PXSelector(typeof(Search4<TWNWHTTran.personalID,
            Where<TWNWHTTran.personalID,IsNotNull>,
            Aggregate<GroupBy<TWNWHTTran.personalID>>>))]
        
        //[PXDefault("A298300011")]
        public virtual string PersonalID { get; set; }
        public abstract class personalID : PX.Data.BQL.BqlString.Field<personalID> { }
        #endregion

        #region PayeeName
        [PXString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Payee Name", IsReadOnly = true)]
        [PXUnboundDefault(typeof(Search<TWNWHTTran.payeeName,
            Where<TWNWHTTran.personalID,Equal<Current<personalID>>>>))]      
        public virtual string PayeeName { get; set; }
        public abstract class payeeName : PX.Data.BQL.BqlString.Field<payeeName> { }
        #endregion

        #region TotalWHTAmt 代扣稅總額
        [PXDecimal(0)]
        [PXUIField(DisplayName = "Total WHT Amount",IsReadOnly =true)]
        [PXUnboundDefault]
        public virtual decimal? TotalWHTAmt { get; set; }
        public abstract class totalWHTAmt : PX.Data.BQL.BqlDecimal.Field<totalWHTAmt> { }
        #endregion

        #region TotalGNHI2Amt 代扣二代健保總額
        [PXDecimal(0)]
        [PXUIField(DisplayName = "Total 2GNHI Amount",IsReadOnly =true)]
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