using System;
using PX.Data;
using RCGV.GV.Attribute.Selector;

namespace RCGV.GV.DAC
{
    [Serializable]
    [PXCacheName("APARGui401DataV")]
    public class APARGui401DataV : IBqlTable
    {
        #region DeclareYear
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Declare Year", IsReadOnly = true)]
        public virtual int? DeclareYear { get; set; }
        public abstract class declareYear : PX.Data.BQL.BqlInt.Field<declareYear> { }
        #endregion

        #region Period
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Period")]
        public virtual int? Period { get; set; }
        public abstract class period : PX.Data.BQL.BqlInt.Field<period> { }
        #endregion

        #region DeclareBatchNbr
        [PXDBString(20, IsUnicode = true, InputMask = "", IsKey = true)]
        [PXUIField(DisplayName = "Declare Batch Nbr", IsReadOnly = true)]
        public virtual string DeclareBatchNbr { get; set; }
        public abstract class declareBatchNbr : PX.Data.BQL.BqlString.Field<declareBatchNbr> { }
        #endregion

        #region RegistrationCD
        [PXDBString(9, IsUnicode = true, InputMask = "", IsKey = true)]
        [PXUIField(DisplayName = "Registration CD", IsReadOnly = true)]
        [RegistrationCDAttribute()]
        public virtual string RegistrationCD { get; set; }
        public abstract class registrationCD : PX.Data.BQL.BqlString.Field<registrationCD> { }
        #endregion

        #region ARSalesAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "ARSales Amt", IsReadOnly = true)]
        public virtual Decimal? ARSalesAmt { get; set; }
        public abstract class aRSalesAmt : PX.Data.BQL.BqlDecimal.Field<aRSalesAmt> { }
        #endregion

        #region ARSalesAmtByZero
        [PXDBDecimal()]
        [PXUIField(DisplayName = "ARSales Amt By Zero", IsReadOnly = true)]
        public virtual Decimal? ARSalesAmtByZero { get; set; }
        public abstract class aRSalesAmtByZero : PX.Data.BQL.BqlDecimal.Field<aRSalesAmtByZero> { }
        #endregion

        #region ARSalesAmtBySP
        [PXDBDecimal()]
        [PXUIField(DisplayName = "AR Sales Amt By SP", IsReadOnly = true)]
        public virtual Decimal? ARSalesAmtBySP { get; set; }
        public abstract class aRSalesAmtBySP : PX.Data.BQL.BqlDecimal.Field<aRSalesAmtBySP> { }
        #endregion

        #region APTaxAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "AP Tax Amt", IsReadOnly = true)]
        public virtual Decimal? APTaxAmt { get; set; }
        public abstract class aPTaxAmt : PX.Data.BQL.BqlDecimal.Field<aPTaxAmt> { }
        #endregion

        #region APDiscountTax
        [PXDBDecimal()]
        [PXUIField(DisplayName = "AP Discount Tax", IsReadOnly = true)]
        public virtual Decimal? APDiscountTax { get; set; }
        public abstract class aPDiscountTax : PX.Data.BQL.BqlDecimal.Field<aPDiscountTax> { }
        #endregion

        #region APForeignTax
        [PXDBDecimal()]
        [PXUIField(DisplayName = "AP Foreign Tax", IsReadOnly = true)]
        public virtual Decimal? APForeignTax { get; set; }
        public abstract class aPForeignTax : PX.Data.BQL.BqlDecimal.Field<aPForeignTax> { }
        #endregion

        #region ARForeignTax
        [PXDBDecimal()]
        [PXUIField(DisplayName = "ARForeign Tax", IsReadOnly = true)]
        public virtual Decimal? ARForeignTax { get; set; }
        public abstract class aRForeignTax : PX.Data.BQL.BqlDecimal.Field<aRForeignTax> { }
        #endregion
    }
}