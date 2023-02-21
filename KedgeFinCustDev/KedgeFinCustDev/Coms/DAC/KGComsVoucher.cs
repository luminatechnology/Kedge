using System;
using PX.Data;

namespace Kedge.Coms.DAC
{
    [Serializable]
    [PXCacheName("KGComsVoucher")]
    public class KGComsVoucher : IBqlTable
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

        #region VoucherID
        [PXDBIdentity(IsKey = true)]
        public virtual int? VoucherID { get; set; }
        public abstract class voucherID : PX.Data.BQL.BqlInt.Field<voucherID> { }
        #endregion

        #region CompanyName
        [PXDBString(3, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Company Name")]
        public virtual string CompanyName { get; set; }
        public abstract class companyName : PX.Data.BQL.BqlString.Field<companyName> { }
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

        #region VoucherDate
        [PXDBString(7, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Voucher Date")]
        public virtual string VoucherDate { get; set; }
        public abstract class voucherDate : PX.Data.BQL.BqlString.Field<voucherDate> { }
        #endregion

        #region Operator
        [PXDBString(12, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Operator")]
        public virtual string Operator { get; set; }
        public abstract class _operator : PX.Data.BQL.BqlString.Field<_operator> { }
        #endregion

        #region DrAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Dr Amt")]
        public virtual Decimal? DrAmt { get; set; }
        public abstract class drAmt : PX.Data.BQL.BqlDecimal.Field<drAmt> { }
        #endregion

        #region CrAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Cr Amt")]
        public virtual Decimal? CrAmt { get; set; }
        public abstract class crAmt : PX.Data.BQL.BqlDecimal.Field<crAmt> { }
        #endregion

        #region AccountantCheckUser
        [PXDBString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Accountant Check User")]
        public virtual string AccountantCheckUser { get; set; }
        public abstract class accountantCheckUser : PX.Data.BQL.BqlString.Field<accountantCheckUser> { }
        #endregion

        #region UniformNo
        [PXDBString(40, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Uniform No")]
        public virtual string UniformNo { get; set; }
        public abstract class uniformNo : PX.Data.BQL.BqlString.Field<uniformNo> { }
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

        #region SubCode
        [PXDBString(25, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Sub Code")]
        public virtual string SubCode { get; set; }
        public abstract class subCode : PX.Data.BQL.BqlString.Field<subCode> { }
        #endregion

        #region ErrorMsg
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Error Msg")]
        public virtual string ErrorMsg { get; set; }
        public abstract class errorMsg : PX.Data.BQL.BqlString.Field<errorMsg> { }
        #endregion

        #region CreateDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Create Date")]
        public virtual DateTime? CreateDate { get; set; }
        public abstract class createDate : PX.Data.BQL.BqlDateTime.Field<createDate> { }
        #endregion

        #region unbound
        #region Num
        [PXInt]
        [PXUnboundDefault(1)]
        [PXFormula(null,typeof(SumCalc<KGComsBatch.voucherCount>))]
        public virtual int? Num { get; set; }
        public abstract class num : PX.Data.BQL.BqlInt.Field<num> { }
        #endregion
        #endregion

        #region value
        public virtual decimal? SumAmt { get; set; }
        #endregion
    }
}