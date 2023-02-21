using System;
using PX.Data;

namespace Kedge.Coms.DAC
{
    [Serializable]
    [PXCacheName("KGComsInvoice")]
    public class KGComsInvoice : IBqlTable
    {
        #region BatchID
        [PXDBInt()]
        [PXDBDefault(typeof(KGComsBatch.batchID))]
        [PXParent(typeof(Select<KGComsBatch,
                            Where<KGComsBatch.batchID,
                            Equal<Current<batchID>>>>))]
        public virtual int? BatchID { get; set; }
        public abstract class batchID : PX.Data.BQL.BqlInt.Field<batchID> { }
        #endregion

        #region InvoiceID
        [PXDBIdentity(IsKey = true)]
        public virtual int? InvoiceID { get; set; }
        public abstract class invoiceID : PX.Data.BQL.BqlInt.Field<invoiceID> { }
        #endregion

        #region VoucherKey
        [PXDBString(11, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Voucher Key")]
        public virtual string VoucherKey { get; set; }
        public abstract class voucherKey : PX.Data.BQL.BqlString.Field<voucherKey> { }
        #endregion

        #region VoucherNo
        [PXDBString(11, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Voucher No")]
        public virtual string VoucherNo { get; set; }
        public abstract class voucherNo : PX.Data.BQL.BqlString.Field<voucherNo> { }
        #endregion

        #region InvoiceNo
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Invoice No")]
        public virtual string InvoiceNo { get; set; }
        public abstract class invoiceNo : PX.Data.BQL.BqlString.Field<invoiceNo> { }
        #endregion

        #region InvoiceDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Invoice Date")]
        public virtual DateTime? InvoiceDate { get; set; }
        public abstract class invoiceDate : PX.Data.BQL.BqlDateTime.Field<invoiceDate> { }
        #endregion

        #region UniformYy
        [PXDBString(3, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Uniform Yy")]
        public virtual string UniformYy { get; set; }
        public abstract class uniformYy : PX.Data.BQL.BqlString.Field<uniformYy> { }
        #endregion

        #region UniformMm
        [PXDBString(2, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Uniform Mm")]
        public virtual string UniformMm { get; set; }
        public abstract class uniformMm : PX.Data.BQL.BqlString.Field<uniformMm> { }
        #endregion

        #region Amount
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Amount")]
        public virtual Decimal? Amount { get; set; }
        public abstract class amount : PX.Data.BQL.BqlDecimal.Field<amount> { }
        #endregion

        #region TaxCode
        [PXDBString(1, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Tax Code")]
        public virtual string TaxCode { get; set; }
        public abstract class taxCode : PX.Data.BQL.BqlString.Field<taxCode> { }
        #endregion

        #region TaxAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Tax Amt")]
        public virtual Decimal? TaxAmt { get; set; }
        public abstract class taxAmt : PX.Data.BQL.BqlDecimal.Field<taxAmt> { }
        #endregion

        #region ClassCode
        [PXDBString(1, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Class Code")]
        public virtual string ClassCode { get; set; }
        public abstract class classCode : PX.Data.BQL.BqlString.Field<classCode> { }
        #endregion

        #region CompanyName
        [PXDBString(3, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Company Name")]
        public virtual string CompanyName { get; set; }
        public abstract class companyName : PX.Data.BQL.BqlString.Field<companyName> { }
        #endregion

        #region ItemNo
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Item No")]
        public virtual Decimal? ItemNo { get; set; }
        public abstract class itemNo : PX.Data.BQL.BqlDecimal.Field<itemNo> { }
        #endregion

        #region UniformNo
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Uniform No")]
        public virtual string UniformNo { get; set; }
        public abstract class uniformNo : PX.Data.BQL.BqlString.Field<uniformNo> { }
        #endregion

        #region IvoKind
        [PXDBString(1, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Ivo Kind")]
        public virtual string IvoKind { get; set; }
        public abstract class ivoKind : PX.Data.BQL.BqlString.Field<ivoKind> { }
        #endregion

        #region BelongYy
        [PXDBString(3, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Belong Yy")]
        public virtual string BelongYy { get; set; }
        public abstract class belongYy : PX.Data.BQL.BqlString.Field<belongYy> { }
        #endregion

        #region BelongMm
        [PXDBString(2, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Belong Mm")]
        public virtual string BelongMm { get; set; }
        public abstract class belongMm : PX.Data.BQL.BqlString.Field<belongMm> { }
        #endregion

        #region Project
        [PXDBString(4, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Project")]
        public virtual string Project { get; set; }
        public abstract class project : PX.Data.BQL.BqlString.Field<project> { }
        #endregion

        #region Accuid
        [PXDBString(22, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Accuid")]
        public virtual string Accuid { get; set; }
        public abstract class accuid : PX.Data.BQL.BqlString.Field<accuid> { }
        #endregion

        #region ErrorMsg
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Error Msg")]
        public virtual string ErrorMsg { get; set; }
        public abstract class errorMsg : PX.Data.BQL.BqlString.Field<errorMsg> { }
        #endregion

        #region unbound
        #region Num
        [PXInt]
        [PXUnboundDefault(1)]
        [PXFormula(null, typeof(SumCalc<KGComsBatch.invoiceCount>))]
        public virtual int? Num { get; set; }
        public abstract class num : PX.Data.BQL.BqlInt.Field<num> { }
        #endregion
        #endregion
    }
}