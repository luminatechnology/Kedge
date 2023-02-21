using System;
using PX.Data;

namespace Kedge.Coms.DAC
{
    [Serializable]
    [PXCacheName("KGComsVoucherItem")]
    public class KGComsVoucherItem : IBqlTable
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

        #region ItemID
        [PXDBIdentity(IsKey = true)]
        public virtual int? ItemID { get; set; }
        public abstract class itemID : PX.Data.BQL.BqlInt.Field<itemID> { }
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

        #region ItemNo
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Item No")]
        public virtual Decimal? ItemNo { get; set; }
        public abstract class itemNo : PX.Data.BQL.BqlDecimal.Field<itemNo> { }
        #endregion

        #region AccountNo
        [PXDBString(8, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Account No")]
        public virtual string AccountNo { get; set; }
        public abstract class accountNo : PX.Data.BQL.BqlString.Field<accountNo> { }
        #endregion

        #region Project
        [PXDBString(4, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Project")]
        public virtual string Project { get; set; }
        public abstract class project : PX.Data.BQL.BqlString.Field<project> { }
        #endregion

        #region Digest
        [PXDBString(200, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Digest")]
        public virtual string Digest { get; set; }
        public abstract class digest : PX.Data.BQL.BqlString.Field<digest> { }
        #endregion

        #region Cd
        [PXDBString(1, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Cd")]
        public virtual string Cd { get; set; }
        public abstract class cd : PX.Data.BQL.BqlString.Field<cd> { }
        #endregion

        #region Amount
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Amount")]
        public virtual Decimal? Amount { get; set; }
        public abstract class amount : PX.Data.BQL.BqlDecimal.Field<amount> { }
        #endregion

        #region Operator
        [PXDBString(12, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Operator")]
        public virtual string Operator { get; set; }
        public abstract class _operator : PX.Data.BQL.BqlString.Field<_operator> { }
        #endregion

        #region Org
        [PXDBString(4, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Org")]
        public virtual string Org { get; set; }
        public abstract class org : PX.Data.BQL.BqlString.Field<org> { }
        #endregion

        #region UniformNo
        [PXDBString(40, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Uniform No")]
        public virtual string UniformNo { get; set; }
        public abstract class uniformNo : PX.Data.BQL.BqlString.Field<uniformNo> { }
        #endregion

        #region DateDue
        [PXDBDate()]
        [PXUIField(DisplayName = "Date Due")]
        public virtual DateTime? DateDue { get; set; }
        public abstract class dateDue : PX.Data.BQL.BqlDateTime.Field<dateDue> { }
        #endregion

        #region AccUid
        [PXDBString(22, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Acc Uid")]
        public virtual string AccUid { get; set; }
        public abstract class accUid : PX.Data.BQL.BqlString.Field<accUid> { }
        #endregion

        #region NameAbbr
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Name Abbr")]
        public virtual string NameAbbr { get; set; }
        public abstract class nameAbbr : PX.Data.BQL.BqlString.Field<nameAbbr> { }
        #endregion

        #region Nametw
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Nametw")]
        public virtual string Nametw { get; set; }
        public abstract class nametw : PX.Data.BQL.BqlString.Field<nametw> { }
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
        [PXFormula(null, typeof(SumCalc<KGComsBatch.voucherItemCount>))]
        public virtual int? Num { get; set; }
        public abstract class num : PX.Data.BQL.BqlInt.Field<num> { }
        #endregion
        #endregion
    }
}