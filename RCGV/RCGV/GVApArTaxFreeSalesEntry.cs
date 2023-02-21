using System;
using System.Collections.Generic;
using PX.Data;
using PX.Data.BQL;
using PX.Objects.GL;
using RCGV.GV.Attribute.Selector;
using RCGV.GV.DAC;

namespace RCGV.GV
{
    public class GVApArTaxFreeSalesEntry : PXGraph<GVApArTaxFreeSalesEntry>
    {

        public PXSave<GVApArTaxFilter> Save;
        public PXCancel<GVApArTaxFilter> Cancel;

        #region View
        public PXFilter<GVApArTaxFilter> TaxFilter;
        public PXSelect<GVApArGuiTaxFreeSalesView,
                 Where<GVApArGuiTaxFreeSalesView.declareYear, Equal<Current<GVApArTaxFilter.declareYear>>,
                 And<GVApArGuiTaxFreeSalesView.registrationCD, Equal<Current<GVApArTaxFilter.registrationCD>>
                 >>> MediaDeclarations;
        public PXSelect<GVApArGuiTaxFreeSales> TaxFreeSales;
        #endregion

        #region Action
        public PXAction<GVApArTaxFilter> reportSalesTax;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "營業稅調整計算表", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = true)]
        protected virtual void ReportSalesTax()
        {
            var currFilter = TaxFilter.Current;
            throw new PXReportRequiredException(new Dictionary<string, string>()
            {
                ["Period"] = "12",
                ["RegistrationCD"] = currFilter.RegistrationCD,
                ["DeclareYear"] = currFilter.DeclareYear.ToString()
            }, "GV604001", PXBaseRedirectException.WindowMode.NewWindow, "營業稅調整計算表");
        }
        #endregion

        #region Event
        protected virtual void _(Events.FieldUpdated<GVApArGuiTaxFreeSalesView, GVApArGuiTaxFreeSalesView.taxFreeSalse> e)
        {
            var row = e.Row;
            if (row == null) return;
            GVApArGuiTaxFreeSales data = GetTaxFreeSales(e.Row.DeclareYear, e.Row.Period, e.Row.DeclareBatchNbr, e.Row.RegistrationCD);
            if (data == null)
            {
                data = new GVApArGuiTaxFreeSales()
                {
                    DeclareYear = e.Row.DeclareYear,
                    Period = e.Row.Period,
                    DeclareBatchNbr = e.Row.DeclareBatchNbr,
                    RegistrationCD = e.Row.RegistrationCD,
                    TaxFreeSalse = e.Row.TaxFreeSalse
                };
                TaxFreeSales.Insert(data);
            }
            else
            {
                data.TaxFreeSalse = e.Row.TaxFreeSalse;
                TaxFreeSales.Update(data);
            }
        }
        protected virtual void _(Events.RowPersisting<GVApArGuiTaxFreeSalesView> e)
        {
            e.Cancel = true;
        }

        #endregion

        #region BQL

        protected GVApArGuiTaxFreeSales GetTaxFreeSales(int? declareYear, int? period, string declareBatchNbr, string registrationCD)
        {
            return PXSelect<GVApArGuiTaxFreeSales,
                Where<GVApArGuiTaxFreeSales.declareYear, Equal<Required<GVApArGuiTaxFreeSales.declareYear>>,
                And<GVApArGuiTaxFreeSales.period, Equal<Required<GVApArGuiTaxFreeSales.period>>,
                And<GVApArGuiTaxFreeSales.declareBatchNbr, Equal<Required<GVApArGuiTaxFreeSales.declareBatchNbr>>,
                And<GVApArGuiTaxFreeSales.registrationCD, Equal<Required<GVApArGuiTaxFreeSales.registrationCD>>>>>>>
              .Select(this, declareYear, period, declareBatchNbr, registrationCD);
        }
        #endregion

        #region Table
        [Serializable]
        public class GVApArTaxFilter : IBqlTable
        {
            #region DeclareYear
            [PXInt()]
            [PXUIField(DisplayName = "Declare Year", Required = true)]
            [PXUnboundDefault(typeof(DatePart<DatePart.year, Current<AccessInfo.businessDate>>))]
            public virtual int? DeclareYear { get; set; }
            public abstract class declareYear : PX.Data.BQL.BqlInt.Field<declareYear> { }
            #endregion

            #region RegistrationCD
            [PXString(IsUnicode = true, InputMask = "")]
            [PXUIField(DisplayName = "Registration CD", Required = true)]
            [RegistrationCDAttribute()]
            [PXUnboundDefault(typeof(
                Search2<GVRegistration.registrationCD,
                    InnerJoin<GVRegistrationBranch, On<GVRegistration.registrationID, Equal<GVRegistrationBranch.registrationID>>,
                    InnerJoin<Branch, On<Branch.bAccountID, Equal<GVRegistrationBranch.bAccountID>>>>,
                    Where<Branch.branchID, Equal<Current<AccessInfo.branchID>>>>))]
            public virtual string RegistrationCD { get; set; }
            public abstract class registrationCD : PX.Data.BQL.BqlString.Field<registrationCD> { }
            #endregion
        }

        [Serializable]
        [PXCacheName("GV Gui TaxFreeSales View")]
        [PXProjection(typeof(
            Select2<APARGui401DataV,
            LeftJoin<GVApArGuiTaxFreeSales,
                On<APARGui401DataV.declareYear, Equal<GVApArGuiTaxFreeSales.declareYear>,
                And<APARGui401DataV.period, Equal<GVApArGuiTaxFreeSales.period>,
                And<APARGui401DataV.declareBatchNbr, Equal<GVApArGuiTaxFreeSales.declareBatchNbr>>>>>,
            Where<APARGui401DataV.declareBatchNbr, IsNotNull>
            >
            )
            , Persistent = true
            )]
        public class GVApArGuiTaxFreeSalesView : APARGui401DataV
        {
            #region TaxFreeSalse
            [PXDBDecimal(0, BqlField = typeof(GVApArGuiTaxFreeSales.taxFreeSalse))]
            [PXUIField(DisplayName = "Tax Free Salse")]
            public virtual Decimal? TaxFreeSalse { get; set; }
            public abstract class taxFreeSalse : PX.Data.BQL.BqlDecimal.Field<taxFreeSalse> { }
            #endregion
        }
        #endregion

    }
}