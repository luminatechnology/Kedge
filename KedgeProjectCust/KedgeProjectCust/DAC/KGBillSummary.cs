using System;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.GL;
namespace Kedge.DAC
{
    /**
     * ===20201/04/06 Mantis:0011992 === Althea
     * Add WhtAmt/HliAmt/Gnhi2Amt/LbpAmt/LbiAmt ReadOnly
     * 
     * ====2021-06-30 :12118 ====Alton
     * 計價匯總, 請將全部金額欄位預設為0, 以避免沒有資料, db 資料為 null, 做累計或是加總時會有錯. 造成許入Agentflow中介table資料有錯.
     * 
     **/
    [Serializable]
    public class KGBillSummary : IBqlTable
    {
        #region BillSummaryID
        [PXDBIdentity()]
        [PXUIField(DisplayName = "Bill Summary ID")]
        public virtual int? BillSummaryID { get; set; }
        public abstract class billSummaryID : PX.Data.BQL.BqlInt.Field<billSummaryID> { }
        #endregion

        #region RefNbr
        [PXDBString(15, IsUnicode = true, InputMask = "", IsKey = true)]
        [PXUIField(DisplayName = "Reference Nbr.", Visibility = PXUIVisibility.Visible, Visible = false)]
        [PXDBLiteDefault(typeof(APInvoice.refNbr))]
        [PXParent(typeof(Select<APRegister, Where<APRegister.docType, Equal<Current<docType>>, And<APRegister.refNbr, Equal<Current<refNbr>>>>>))]
        public virtual string RefNbr { get; set; }
        public abstract class refNbr : PX.Data.BQL.BqlString.Field<refNbr> { }
        #endregion

        #region DocType
        [PXDBString(3, IsFixed = true, InputMask = "", IsKey = true)]
        [PXUIField(DisplayName = "Doc Type")]
        [PXDBLiteDefault(typeof(APInvoice.docType))]
        public virtual string DocType { get; set; }
        public abstract class docType : PX.Data.BQL.BqlString.Field<docType> { }
        #endregion

        #region BillStartDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Bill Start Date")]
        public virtual DateTime? BillStartDate { get; set; }
        public abstract class billStartDate : IBqlField { }
        #endregion

        #region BillEndDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Bill End Date")]
        public virtual DateTime? BillEndDate { get; set; }
        public abstract class billEndDate : IBqlField { }
        #endregion

        #region PoValuationAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Po Valuation Amt")]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? PoValuationAmt { get; set; }
        public abstract class poValuationAmt : PX.Data.BQL.BqlDecimal.Field<poValuationAmt> { }
        #endregion

        #region PoCumulativeAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Po Cumulative Amt")]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? PoCumulativeAmt { get; set; }
        public abstract class poCumulativeAmt : PX.Data.BQL.BqlDecimal.Field<poCumulativeAmt> { }
        #endregion

        #region PoTotalAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Po Total Amt", Enabled = false)]
        public virtual Decimal? PoTotalAmt { get; set; }
        public abstract class poTotalAmt : PX.Data.BQL.BqlDecimal.Field<poTotalAmt> { }
        #endregion

        #region PoValuationPercent
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Po Valuation Percent")]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? PoValuationPercent { get; set; }
        public abstract class poValuationPercent : PX.Data.BQL.BqlDecimal.Field<poValuationPercent> { }
        #endregion

        #region PrepaymentAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Prepayment Amt")]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? PrepaymentAmt { get; set; }
        public abstract class prepaymentAmt : PX.Data.BQL.BqlDecimal.Field<prepaymentAmt> { }
        #endregion

        #region PrepaymentCumAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Prepayment Cumulative Amt", Enabled = false)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? PrepaymentCumAmt { get; set; }
        public abstract class prepaymentCumAmt : PX.Data.BQL.BqlDecimal.Field<prepaymentCumAmt> { }
        #endregion

        #region PrepaymentTotalAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Prepayment Total Amt", Enabled = false)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? PrepaymentTotalAmt { get; set; }
        public abstract class prepaymentTotalAmt : PX.Data.BQL.BqlDecimal.Field<prepaymentTotalAmt> { }
        #endregion

        #region PrepaymentTaxAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Prepayment Tax Amt")]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? PrepaymentTaxAmt { get; set; }
        public abstract class prepaymentTaxAmt : PX.Data.BQL.BqlDecimal.Field<prepaymentTaxAmt> { }
        #endregion


        #region PrepaymentWithTaxAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Prepayment With Tax Amt", Enabled = false)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? PrepaymentWithTaxAmt { get; set; }
        public abstract class prepaymentWithTaxAmt : PX.Data.BQL.BqlDecimal.Field<prepaymentWithTaxAmt> { }
        #endregion

        #region PrepaymentDuctAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Prepayment Duct Amt")]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? PrepaymentDuctAmt { get; set; }
        public abstract class prepaymentDuctAmt : PX.Data.BQL.BqlDecimal.Field<prepaymentDuctAmt> { }
        #endregion

        #region PrepaymentDuctCumAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Prepayment Duct Cumulative Amt", Enabled = false)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? PrepaymentDuctCumAmt { get; set; }
        public abstract class prepaymentDuctCumAmt : PX.Data.BQL.BqlDecimal.Field<prepaymentDuctCumAmt> { }
        #endregion

        #region PrepaymentDuctTotalAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Prepayment Duct Total Amt", Enabled = false)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? PrepaymentDuctTotalAmt { get; set; }
        public abstract class prepaymentDuctTotalAmt : PX.Data.BQL.BqlDecimal.Field<prepaymentDuctTotalAmt> { }
        #endregion

        #region PrepaymentDuctTaxAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Prepayment Duct Tax Amt", Enabled = false)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? PrepaymentDuctTaxAmt { get; set; }
        public abstract class prepaymentDuctTaxAmt : PX.Data.BQL.BqlDecimal.Field<prepaymentDuctTaxAmt> { }
        #endregion

        #region PrepaymentDuctWithTaxAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Prepayment Duct With  Tax Amt", Enabled = false)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? PrepaymentDuctWithTaxAmt { get; set; }
        public abstract class prepaymentDuctWithTaxAmt : PX.Data.BQL.BqlDecimal.Field<prepaymentDuctWithTaxAmt> { }
        #endregion

        #region AdditionAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Addition Amt", Enabled = false)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? AdditionAmt { get; set; }
        public abstract class additionAmt : PX.Data.BQL.BqlDecimal.Field<additionAmt> { }
        #endregion

        #region AdditionCumAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Addition Cumulative Amt", Enabled = false)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? AdditionCumAmt { get; set; }
        public abstract class additionCumAmt : PX.Data.BQL.BqlDecimal.Field<additionCumAmt> { }
        #endregion

        #region AdditionTotalAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Addition Total Amt", Enabled = false)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? AdditionTotalAmt { get; set; }
        public abstract class additionTotalAmt : PX.Data.BQL.BqlDecimal.Field<additionTotalAmt> { }
        #endregion

        #region AdditionTaxAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Addition Tax Amt", Enabled = false)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? AdditionTaxAmt { get; set; }
        public abstract class additionTaxAmt : PX.Data.BQL.BqlDecimal.Field<additionTaxAmt> { }
        #endregion

        #region AdditionWithTaxAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Addition With Tax Amt", Enabled = false)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? AdditionWithTaxAmt { get; set; }
        public abstract class additionWithTaxAmt : PX.Data.BQL.BqlDecimal.Field<additionWithTaxAmt> { }
        #endregion

        #region DeductionAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Deduction Amt")]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? DeductionAmt { get; set; }
        public abstract class deductionAmt : PX.Data.BQL.BqlDecimal.Field<deductionAmt> { }
        #endregion

        #region DeductionTaxAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Deduction Tax Amt")]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? DeductionTaxAmt { get; set; }
        public abstract class deductionTaxAmt : PX.Data.BQL.BqlDecimal.Field<deductionTaxAmt> { }
        #endregion

        #region DeductionWithTaxAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Deduction With Tax Amt", Enabled = false)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? DeductionWithTaxAmt { get; set; }
        public abstract class deductionWithTaxAmt : PX.Data.BQL.BqlDecimal.Field<deductionWithTaxAmt> { }
        #endregion

        #region DeductionWithTaxCumAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Deduction With Tax Cumulative Amt", Enabled = false)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? DeductionWithTaxCumAmt { get; set; }
        public abstract class deductionWithTaxCumAmt : PX.Data.BQL.BqlDecimal.Field<deductionWithTaxCumAmt> { }
        #endregion

        #region DeductionWithTaxTotalAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Deduction With Tax Total Amt", Enabled = false)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? DeductionWithTaxTotalAmt { get; set; }
        public abstract class deductionWithTaxTotalAmt : PX.Data.BQL.BqlDecimal.Field<deductionWithTaxTotalAmt> { }
        #endregion

        #region StdDeductionAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "STD Deduction Amt")]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? StdDeductionAmt { get; set; }
        public abstract class stdDeductionAmt : PX.Data.BQL.BqlDecimal.Field<stdDeductionAmt> { }
        #endregion

        #region InsDeductionAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "INS Deduction Amt")]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? InsDeductionAmt { get; set; }
        public abstract class insDeductionAmt : PX.Data.BQL.BqlDecimal.Field<insDeductionAmt> { }
        #endregion

        #region OriPOTotalAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Ori PO Total Amt")]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? OriPOTotalAmt { get; set; }
        public abstract class oriPOTotalAmt : PX.Data.BQL.BqlDecimal.Field<oriPOTotalAmt> { }
        #endregion

        #region RetentionAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Retention Amt")]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? RetentionAmt { get; set; }
        public abstract class retentionAmt : PX.Data.BQL.BqlDecimal.Field<retentionAmt> { }
        #endregion

        #region RetentionTaxAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Retention Tax Amt")]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? RetentionTaxAmt { get; set; }
        public abstract class retentionTaxAmt : PX.Data.BQL.BqlDecimal.Field<retentionTaxAmt> { }
        #endregion

        #region RetentionWithTaxAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Retention With Tax  Amt", Enabled = false)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? RetentionWithTaxAmt { get; set; }
        public abstract class retentionWithTaxAmt : PX.Data.BQL.BqlDecimal.Field<retentionWithTaxAmt> { }
        #endregion

        #region RetentionWithTaxCumAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Retention With Tax Cumulative Amt", Enabled = false)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? RetentionWithTaxCumAmt { get; set; }
        public abstract class retentionWithTaxCumAmt : PX.Data.BQL.BqlDecimal.Field<retentionWithTaxCumAmt> { }
        #endregion

        #region RetentionWithTaxTotalAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Retention With Tax Total Amt", Enabled = false)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? RetentionWithTaxTotalAmt { get; set; }
        public abstract class retentionWithTaxTotalAmt : PX.Data.BQL.BqlDecimal.Field<retentionWithTaxTotalAmt> { }
        #endregion

        #region RetentionReturnAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Retention Return Amt")]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? RetentionReturnAmt { get; set; }
        public abstract class retentionReturnAmt : PX.Data.BQL.BqlDecimal.Field<retentionReturnAmt> { }
        #endregion

        #region RetentionReturnTaxAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Retention Return Tax Amt")]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? RetentionReturnTaxAmt { get; set; }
        public abstract class retentionReturnTaxAmt : PX.Data.BQL.BqlDecimal.Field<retentionReturnTaxAmt> { }
        #endregion

        #region RetentionReturnWithTaxAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Retention With Tax Return Amt", Enabled = false)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? RetentionReturnWithTaxAmt { get; set; }
        public abstract class retentionReturnWithTaxAmt : PX.Data.BQL.BqlDecimal.Field<retentionReturnWithTaxAmt> { }
        #endregion

        #region RetentionReturnWithTaxCumAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Retention With Tax Return Cumulative Amt", Enabled = false)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? RetentionReturnWithTaxCumAmt { get; set; }
        public abstract class retentionReturnWithTaxCumAmt : PX.Data.BQL.BqlDecimal.Field<retentionReturnWithTaxCumAmt> { }
        #endregion

        #region RetentionReturnWithTaxTotalAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Retention With Tax Return Total Amt", Enabled = false)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? RetentionReturnWithTaxTotalAmt { get; set; }
        public abstract class retentionReturnWithTaxTotalAmt : PX.Data.BQL.BqlDecimal.Field<retentionReturnWithTaxTotalAmt> { }
        #endregion

        #region TaxAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Tax Amt", Enabled = false)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? TaxAmt { get; set; }
        public abstract class taxAmt : PX.Data.BQL.BqlDecimal.Field<taxAmt> { }
        #endregion

        #region TotalAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Total Amt")]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? TotalAmt { get; set; }
        public abstract class totalAmt : PX.Data.BQL.BqlDecimal.Field<totalAmt> { }
        #endregion

        #region OriTotalAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Ori Total Amt", Enabled = false)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? OriTotalAmt { get; set; }
        public abstract class oriTotalAmt : PX.Data.BQL.BqlDecimal.Field<oriTotalAmt> { }
        #endregion

        #region GvInvWithTaxAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "GV Invoice Amt", Enabled = false)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? GvInvWithTaxAmt { get; set; }
        public abstract class gvInvWithTaxAmt : PX.Data.BQL.BqlDecimal.Field<gvInvWithTaxAmt> { }
        #endregion

        #region NoteID
        [PXNote()]
        [PXUIField(DisplayName = "NoteID")]
        public virtual Guid? NoteID { get; set; }
        public abstract class noteID : IBqlField { }
        #endregion

        #region CreatedByID
        [PXDBCreatedByID()]
        public virtual Guid? CreatedByID { get; set; }
        public abstract class createdByID : IBqlField { }
        #endregion

        #region CreatedByScreenID
        [PXDBCreatedByScreenID()]
        public virtual string CreatedByScreenID { get; set; }
        public abstract class createdByScreenID : IBqlField { }
        #endregion

        #region CreatedDateTime
        [PXDBCreatedDateTime()]
        public virtual DateTime? CreatedDateTime { get; set; }
        public abstract class createdDateTime : IBqlField { }
        #endregion

        #region LastModifiedByID
        [PXDBLastModifiedByID()]
        public virtual Guid? LastModifiedByID { get; set; }
        public abstract class lastModifiedByID : IBqlField { }
        #endregion

        #region LastModifiedByScreenID
        [PXDBLastModifiedByScreenID()]
        public virtual string LastModifiedByScreenID { get; set; }
        public abstract class lastModifiedByScreenID : IBqlField { }
        #endregion

        #region LastModifiedDateTime
        [PXDBLastModifiedDateTime()]
        public virtual DateTime? LastModifiedDateTime { get; set; }
        public abstract class lastModifiedDateTime : IBqlField { }
        #endregion

        #region Tstamp
        [PXDBTimestamp()]
        [PXUIField(DisplayName = "Tstamp")]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : IBqlField { }
        #endregion

        //2021/03/31 Add manits: 0011992
        #region WhtAmt
        [PXDBDecimal]
        [PXUIField(DisplayName = "WhtAmt", IsReadOnly = true)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? WhtAmt { get; set; }
        public abstract class whtAmt : PX.Data.BQL.BqlDecimal.Field<whtAmt> { }
        #endregion

        #region HliAmt
        [PXDBDecimal]
        [PXUIField(DisplayName = "HliAmt", IsReadOnly = true)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]

        public virtual Decimal? HliAmt { get; set; }
        public abstract class hliAmt : PX.Data.BQL.BqlDecimal.Field<hliAmt> { }
        #endregion

        #region Gnhi2Amt
        [PXDBDecimal]
        [PXUIField(DisplayName = "Gnhi2Amt", IsReadOnly = true)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]

        public virtual Decimal? Gnhi2Amt { get; set; }
        public abstract class gnhi2Amt : PX.Data.BQL.BqlDecimal.Field<gnhi2Amt> { }
        #endregion

        #region LbiAmt
        [PXDBDecimal]
        [PXUIField(DisplayName = "LbiAmt", IsReadOnly = true)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]

        public virtual Decimal? LbiAmt { get; set; }
        public abstract class lbiAmt : PX.Data.BQL.BqlDecimal.Field<lbiAmt> { }
        #endregion

        #region LbpAmt
        [PXDBDecimal]
        [PXUIField(DisplayName = "LbpAmt", IsReadOnly = true)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]

        public virtual Decimal? LbpAmt { get; set; }
        public abstract class lbpAmt : PX.Data.BQL.BqlDecimal.Field<lbpAmt> { }
        #endregion

        //2021/06/17 Add Mantis: 0012096
        #region CheckDiscountAmt
        [PXDBDecimal]
        [PXUIField(DisplayName = "CheckDiscountAmt", IsReadOnly = true)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]

        public virtual Decimal? CheckDiscountAmt { get; set; }
        public abstract class checkDiscountAmt : PX.Data.BQL.BqlDecimal.Field<checkDiscountAmt> { }
        #endregion

    }
}