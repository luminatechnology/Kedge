using System;
using PX.Data;
using PX.Objects.PM;

namespace PX.Objects.AP
{
    /**
     * ===2021/03/24 Add Mantis:0011992=== Althea
     * Add UsrValuationType :"L"-"勞保費", "H"-"健保費", "N"="退休金"
     * ===2021/06/18 Add Mantis:0012096===Althea
     * Add UsrValuationType: "C"-"票貼"
     * ===2021/11/18 Add Mantis:0012267 === althea
     * Add Fields UsrTmpPaymentReleased deciaml(19,6) for Fin TempPayment 
     * ===2022/05/09 Mantis [0012321]===Jeff
     * Add two fields to store the source value and manual typing by the user.
    **/
    public class APTranExt : PXCacheExtension<PX.Objects.AP.APTran>
    {
        #region UsrValuationID
        [PXDBInt]
        [PXUIField(DisplayName = "ValuationID", Enabled = false)]
        public virtual int? UsrValuationID { get; set; }
        public abstract class usrValuationID : IBqlField { }
        #endregion

        #region UsrValuationType
        [PXDBString(1, IsFixed = true, IsUnicode = true)]
        [PXDefault("B")]
        [ValuationTypeStringList]
        [PXUIField(DisplayName = "ValuationType", Enabled = false)]
        public virtual string UsrValuationType { get; set; }
        public abstract class usrValuationType : PX.Data.BQL.BqlString.Field<usrValuationType> { }
        #endregion

        #region UsrSegPricingFlag
        [PXDBBool]
        [PXUIField(DisplayName = "Segment Pricing Flag")]

        public virtual bool? UsrSegPricingFlag { get; set; }
        public abstract class usrSegPricingFlag : PX.Data.BQL.BqlBool.Field<usrSegPricingFlag> { }
        #endregion

        #region UsrAccumulateQty
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Accumulate Qty", Enabled = false)]
        public virtual Decimal? UsrAccumulateQty { get; set; }
        public abstract class usrAccumulateQty : IBqlField { }
        #endregion

        #region UsrTotalAccumulateQty
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Total Accumulate Qty", Enabled = false)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXFormula(typeof(Add<usrAccumulateQty, PX.Objects.AP.APTran.qty>))]
        public virtual Decimal? UsrTotalAccumulateQty { get; set; }
        public abstract class usrTotalAccumulateQty : IBqlField { }
        #endregion

        #region UsrAccumulateAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Accumulate Amt", Enabled = false)]
        public virtual Decimal? UsrAccumulateAmt { get; set; }
        public abstract class usrAccumulateAmt : IBqlField { }
        #endregion

        #region UsrTotalAccumulateAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Total Accumulate Amt", Enabled = false)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXFormula(typeof(Add<usrAccumulateAmt, PX.Objects.AP.APTran.curyTranAmt>))]
        public virtual Decimal? UsrTotalAccumulateAmt { get; set; }
        public abstract class usrTotalAccumulateAmt : IBqlField { }
        #endregion

        #region UsrOrigDocType
        [PXDBString(50, IsUnicode = true)]
        [PXUIField(DisplayName = "OrigDocType", Enabled = false)]
        public virtual string UsrOrigDocType { get; set; }
        public abstract class usrOrigDocType : IBqlField { }
        #endregion

        #region UsrOrigRefNbr
        [PXDBString(50, IsUnicode = true)]
        [PXUIField(DisplayName = "OrigRefNbr", Enabled = false)]
        public virtual string UsrOrigRefNbr { get; set; }
        public abstract class usrOrigRefNbr : IBqlField { }
        #endregion

        //2021/11/10 Mantis:0012266 add by althea
        #region UsrTmpPaymentReleased
        [PXDBDecimal]
        [PXUIField(DisplayName = "UsrTmpPaymentReleased")]
        [PXDefault(TypeCode.Decimal,"0.0",PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? UsrTmpPaymentReleased { get; set; }
        public abstract class usrTmpPaymentReleased : PX.Data.BQL.BqlDecimal.Field<usrTmpPaymentReleased> { }
        #endregion

        #region initFlag
        [PXBool]
        [PXDefault(true,PersistingCheck =PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "InitFlag", Enabled = false)]
        public virtual bool? InitFlag { get; set; }
        public abstract class initFlag : IBqlField { }
        #endregion

        #region UsrEPProjectID
        [PXUIField(DisplayName = "EPProject", IsReadOnly = true)]
        [ProjectBase()]
        public virtual int? UsrEPProjectID { get; set; }
        public abstract class usrEPProjectID : PX.Data.BQL.BqlInt.Field<usrEPProjectID> { }
        #endregion

        //2022/03/21 計價單保留款Tab
        #region UsrOrigRetainageDocType
        [PXDBString(3, IsUnicode = true)]
        [PXUIField(DisplayName = "Orig. Retainage Doc. Type", Enabled = false)]
        public virtual string UsrOrigRetainageDocType { get; set; }
        public abstract class usrOrigRetainageDocType : PX.Data.BQL.BqlString.Field<usrOrigRetainageDocType> { }
        #endregion

        #region UsrOrigRetainageRefNbr
        [PXDBString(50, IsUnicode = true)]
        [PXUIField(DisplayName = "Orig. Retainage Ref. Nbr.", Enabled = false)]
        public virtual string UsrOrigRetainageRefNbr { get; set; }
        public abstract class usrOrigRetainageRefNbr : PX.Data.BQL.BqlString.Field<usrOrigRetainageRefNbr> { }
        #endregion

        #region UsrOrigRetUnreleasedAmt
        [PXDBDecimal]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Orig. Retainage Unreleased Amt", IsReadOnly = true)]
        public virtual decimal? UsrOrigRetUnreleasedAmt { get; set; }
        public abstract class usrOrigRetUnreleasedAmt : PX.Data.BQL.BqlDecimal.Field<usrOrigRetUnreleasedAmt> { }
        #endregion

        #region UsrRetainagePct
        [PXDBInt(MinValue = 0, MaxValue = 100)]
        [PXDefault(100, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Retainage Percent")]
        public virtual int? UsrRetainagePct { get; set; }
        public abstract class usrRetainagePct : PX.Data.BQL.BqlInt.Field<usrRetainagePct> { }
        #endregion
    }

    #region ValuationTypeStringList
    public class ValuationTypeStringList : PXStringListAttribute
    {
        public ValuationTypeStringList() : base(Values, Labels) { }
        public static readonly string[] Values =
            {
            A, D, R, P, B, W, S, L, H, N,C
        };
        public static readonly string[] Labels =
        {
            "加款", "扣款", "保留款退回", "預付款", "計價",  "代扣稅", "健保補充",
                          "勞保費","健保費","退休金","票貼"
        };
        /// <summary>
        /// 加款
        /// </summary>
        public const string A = "A";
        /// <summary>
        /// 扣款
        /// </summary>
        public const string D = "D";
        /// <summary>
        /// 保留款退回
        /// </summary>
        public const string R = "R";
        /// <summary>
        /// 預付款
        /// </summary>
        public const string P = "P";
        /// <summary>
        /// 計價
        /// </summary>
        public const string B = "B";
        /// <summary>
        /// 代扣稅
        /// </summary>
        public const string W = "W";
        /// <summary>
        /// 健保補充
        /// </summary>
        public const string S = "S";
        /// <summary>
        /// 勞保費
        /// </summary>
        public const string L = "L";
        /// <summary>
        /// 健保費
        /// </summary>
        public const string H = "H";
        /// <summary>
        /// 退休金
        /// </summary>
        public const string N = "N";
        /// <summary>
        /// 票貼 
        /// </summary>
        /// 2021/06/18 Add 
        public const string C = "C";
        /// <summary>
        /// 代扣稅
        /// </summary>
        /// 2023/01/04 mantis:12386
        public const string WITH_TAX = "T";
        /// <summary>
        /// 二代健保
        /// </summary>
        /// 2023/01/04 mantis:12386 
        public const string SECOND = "2";

        /// <summary>
        /// 加款
        /// </summary>
        public class a : PX.Data.BQL.BqlString.Constant<a> { public a() : base(A) {; } }
        /// <summary>
        /// 扣款
        /// </summary>
        public class d : PX.Data.BQL.BqlString.Constant<d> { public d() : base(D) {; } }
        /// <summary>
        /// 保留款退回
        /// </summary>
        public class r : PX.Data.BQL.BqlString.Constant<r> { public r() : base(R) {; } }
        /// <summary>
        /// 預付款
        /// </summary>
        public class p : PX.Data.BQL.BqlString.Constant<p> { public p() : base(P) {; } }
        /// <summary>
        /// 計價
        /// </summary>
        public class b : PX.Data.BQL.BqlString.Constant<b> { public b() : base(B) {; } }
        /// <summary>
        /// 代扣稅
        /// </summary>
        public class w : PX.Data.BQL.BqlString.Constant<w> { public w() : base(W) {; } }
        /// <summary>
        /// 健保補充
        /// </summary>
        public class s : PX.Data.BQL.BqlString.Constant<s> { public s() : base(S) {; } }
        /// <summary>
        /// 勞保費
        /// </summary>
        public class l : PX.Data.BQL.BqlString.Constant<l> { public l() : base(L) {; } }
        /// <summary>
        /// 健保費
        /// </summary>
        public class h : PX.Data.BQL.BqlString.Constant<h> { public h() : base(H) {; } }
        /// <summary>
        /// 退休金
        /// </summary>
        public class n : PX.Data.BQL.BqlString.Constant<n> { public n() : base(N) {; } }
        /// <summary>
        /// 票貼
        /// </summary>
        public class c : PX.Data.BQL.BqlString.Constant<c> { public c() : base(C) {; } }

        /// <summary>
        /// 代扣稅
        /// </summary>
        /// 2023/01/04 mantis:12386
        public class withTax : PX.Data.BQL.BqlString.Constant<withTax> { public withTax() : base(WITH_TAX) {; } }
        /// <summary>
        /// 二代健保
        /// </summary>
        /// 2023/01/04 mantis:12386 
        public class second : PX.Data.BQL.BqlString.Constant<second> { public second() : base(SECOND) {; } }

    }
    #endregion
}