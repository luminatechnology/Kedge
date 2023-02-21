using System;
using PX.Data;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.CS;
using PX.Objects.CT;
using PX.Objects.IN;
using PX.Objects.PM;

namespace Kedge.DAC
{
    /**
     * ===2021/06/15 :0012093 ===Althea
     * ValuationType Add:
     * "3", 票貼
     * 
     * ===2021/07/19 :0012158 === Althea
     * ValuationType Add:
     * "4", 外勞租用扣款
     * 
     * ===2021/09/28 :0012247 === Althea
     * ValuationType Add:
     * "5", 附買回扣款
     * 
     * ===2021/11/05 : 0012264 === Althea
     * ValuationType Change name : "4", 外勞租用扣款 -->外勞管理費
     **/
    [Serializable]
    public class KGValuation : IBqlTable
    {
        #region ValuationID
        [PXDBIdentity(IsKey = true)]
        [PXUIField(DisplayName = "Valuation ID")]
        public virtual int? ValuationID { get; set; }
        public abstract class valuationID : IBqlField { }
        #endregion

        #region ValuationCD
        [PXDBString(30, IsKey = true, IsUnicode = true, InputMask = ">CCCCCCCC")]
        [PXUIField(DisplayName = "Valuation CD", Required = true)]
        [PXSelector(
        typeof(Search<KGValuation.valuationCD>),
        typeof(KGValuation.valuationCD),
        typeof(KGValuation.valuationType),
        typeof(KGValuation.valuationDate),
            typeof(KGValuation.contractID),
            typeof(KGValuation.dailyRenterCD),
            typeof(KGValuation.createdByID))]
        //,
        //SubstituteKey = typeof(KGValuation.valuationCD))]
        [AutoNumber(typeof(KGSetUp.kGValuationNumbering), typeof(AccessInfo.businessDate))]

        public virtual string ValuationCD { get; set; }
        public abstract class valuationCD : IBqlField { }
        #endregion

        #region ContractID
        [PXUIField(DisplayName = "Usr Contract ID", Required = true)]
        [PXRestrictor(typeof(Where<PMProject.isCancelled, Equal<False>>), PX.Objects.PM.Messages.CancelledContract, typeof(PMProject.contractCD))]
        [PXRestrictor(typeof(Where<PMProject.visibleInPO, Equal<True>, Or<PMProject.nonProject, Equal<True>>>), PX.Objects.PM.Messages.ProjectInvisibleInModule, typeof(PMProject.contractCD))]
        [PXRestrictor(typeof(Where<PMProject.defaultBranchID, Equal<Current<AccessInfo.branchID>>>), "Branch Not Found.", typeof(PMProject.contractCD))]
        [ProjectBaseAttribute()]
        [PXForeignReference(typeof(Field<contractID>.IsRelatedTo<PMProject.contractID>))]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual int? ContractID { get; set; }
        public abstract class contractID : IBqlField { }
        #endregion

        #region ValuationType
        [PXDBString(1, IsFixed = true, IsUnicode = true)]
        [PXUIField(DisplayName = "Valuation Type", Required = true, Enabled = false)]
        [KGValuationTypeStringList]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual string ValuationType { get; set; }
        public abstract class valuationType : IBqlField { }
        #endregion

        #region ValuationDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Valuation Date", Required = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual DateTime? ValuationDate { get; set; }
        public abstract class valuationDate : IBqlField { }
        #endregion

        #region ValuationContent
        [PXDBString(IsUnicode = true)]
        [PXUIField(DisplayName = "Valuation Content", Enabled = false)]
        public virtual string ValuationContent { get; set; }
        public abstract class valuationContent : IBqlField { }
        #endregion

        #region Description
        [PXDBString(IsUnicode = true)]
        [PXUIField(DisplayName = "Description", Required = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual string Description { get; set; }
        public abstract class description : IBqlField { }
        #endregion

        #region Qty
        //[PXDBDecimal(2)]
        [PXUIField(DisplayName = "Qty", Required = true, Enabled = false)]
        //20200909 小數點幾位改為抓系統的設定QTY
        [PXDBQuantity()]
        [PXDefault(TypeCode.Decimal, "0.00"
            , PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual Decimal? Qty { get; set; }
        public abstract class qty : IBqlField { }
        #endregion

        #region Uom
        [PXDBString(6, IsUnicode = true)]
        [PXUIField(DisplayName = "UOM", Required = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXStringList(new string[] { "工", "式" }, new string[] { "工", "式" })]
        public virtual string Uom { get; set; }
        public abstract class uom : IBqlField { }
        #endregion

        #region UnitPrice
        [PXDBDecimal(0)]
        [PXUIField(DisplayName = "Unit Price", Required = true, Enabled = false)]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual Decimal? UnitPrice { get; set; }
        public abstract class unitPrice : IBqlField { }
        #endregion

        #region Amount
        [PXDBDecimal(0)]
        [PXUIField(DisplayName = "Amount", Enabled = false)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        public virtual Decimal? Amount { get; set; }
        public abstract class amount : IBqlField { }
        #endregion

        #region TaxAmt
        [PXDBDecimal(0)]
        [PXUIField(DisplayName = "Tax Amt", Enabled = false)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        public virtual Decimal? TaxAmt { get; set; }
        public abstract class taxAmt : IBqlField { }
        #endregion

        #region TotalAmt
        [PXDBDecimal(0)]
        [PXUIField(DisplayName = "Total Amt", Enabled = false)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        public virtual Decimal? TotalAmt { get; set; }
        public abstract class totalAmt : IBqlField { }
        #endregion

        #region ManageFeeAmt
        [PXDBDecimal(0)]
        [PXUIField(DisplayName = "Manage Fee Amt", Enabled = false)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        public virtual Decimal? ManageFeeAmt { get; set; }
        public abstract class manageFeeAmt : IBqlField { }
        #endregion

        #region ManageFeeTaxAmt
        [PXDBDecimal(0)]
        [PXUIField(DisplayName = "Manage Fee Tax Amt", Enabled = false)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        public virtual Decimal? ManageFeeTaxAmt { get; set; }
        public abstract class manageFeeTaxAmt : IBqlField { }
        #endregion

        //2019/06/26加回欄位
        #region IsTaxFree
        [PXDBBool()]
        [PXUIField(DisplayName = "IsTaxFree")]
        [PXDefault(false)]
        public virtual bool? IsTaxFree { get; set; }
        public abstract class isTaxFree : IBqlField { }
        #endregion

        #region IsFreeManageFeeAmt
        [PXDBBool()]
        [PXUIField(DisplayName = "Is Free Manage Fee Amt", Required = true, Enabled = false)]
        [PXDefault(false)]
        public virtual bool? IsFreeManageFeeAmt { get; set; }
        public abstract class isFreeManageFeeAmt : IBqlField { }
        #endregion

        #region ManageFeeTotalAmt
        [PXDBDecimal(0)]
        [PXUIField(DisplayName = "Manage Fee Total Amt", Enabled = false)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        public virtual Decimal? ManageFeeTotalAmt { get; set; }
        public abstract class manageFeeTotalAmt : IBqlField { }
        #endregion

        #region AdditionAmt
        [PXDBDecimal(0)]
        [PXUIField(DisplayName = "Addition Amt", Enabled = false)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        public virtual Decimal? AdditionAmt { get; set; }
        public abstract class additionAmt : IBqlField { }
        #endregion

        #region DeductionAmt
        [PXDBDecimal(0)]
        [PXUIField(DisplayName = "Deduction Amt", Enabled = false)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        public virtual Decimal? DeductionAmt { get; set; }
        public abstract class deductionAmt : IBqlField { }
        #endregion

        #region Status
        [PXDBString(1, IsFixed = true, IsUnicode = true)]
        [PXUIField(DisplayName = "Status", Enabled = false, Required = true)]
        [PXDefault(KGValuationStatusStringList.O)]
        [KGValuationStatusStringList]
        public virtual string Status { get; set; }
        public abstract class status : IBqlField { }
        #endregion

        #region Hold
        [PXDBBool()]
        [PXUIField(DisplayName = "Hold", Required = true)]
        [PXDefault(true)]
        public virtual bool? Hold { get; set; }
        public abstract class hold : IBqlField { }
        #endregion

        #region Vendor
        [PXString(IsUnicode = true)]
        [PXUIField(DisplayName = "Vendor")]
        public virtual string Vendor { get; set; }
        public abstract class vendor : IBqlField { }
        #endregion

        #region DailyRenterCD
        [PXDBString(15, IsUnicode = true)]
        [PXUIField(DisplayName = "Daily Renter CD", Enabled = false)]
        [PXSelector(typeof(Search<KGDailyRenter.dailyRenterCD>),
            typeof(KGDailyRenter.dailyRenterCD))]
        public virtual string DailyRenterCD { get; set; }
        public abstract class dailyRenterCD : IBqlField { }
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

        #region NoteID
        [PXNote()]
        [PXUIField(DisplayName = "NoteID")]
        public virtual Guid? NoteID { get; set; }
        public abstract class noteID : IBqlField { }
        #endregion

        #region Tstamp
        [PXDBTimestamp()]
        [PXUIField(DisplayName = "Tstamp")]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : IBqlField { }
        #endregion

    }
    public class KGValuationStatusStringList : PXStringListAttribute
    {
        public KGValuationStatusStringList() : base(Values, Labels) { }
        public static readonly string[] Values =
         {
            O,
            P,
            C,
            S,
            V,
            B
        };
        public static readonly string[] Labels =
        {
            "Open", 
            "Pending Confirm", 
            "Confirm", 
            "Pending Sign", 
            "Vendor Confirm", 
            "Billed"
        };

        /// <summary>
        /// Open
        /// </summary>
        public const string O = "O";
        /// <summary>
        /// Pending Confirm
        /// </summary>
        public const string P = "P";
        /// <summary>
        /// Confirm
        /// </summary>
        public const string C = "C";
        /// <summary>
        /// Pending Sign
        /// </summary>
        public const string S = "S";
        /// <summary>
        /// Vendor Confirm
        /// </summary>
        public const string V = "V";
        /// <summary>
        /// Billed
        /// </summary>
        public const string B = "B";

        /// <summary>
        /// Open
        /// </summary>
        public class o : PX.Data.BQL.BqlString.Constant<o> { public o() : base(O) {; } }
        /// <summary>
        ///Pending Confirm
        /// </summary>
        public class p : PX.Data.BQL.BqlString.Constant<p> { public p() : base(P) {; } }
        /// <summary>
        /// Confirm
        /// </summary>
        public class c : PX.Data.BQL.BqlString.Constant<c> { public c() : base(C) {; } }
        //2021/06/015 Add Mantis: 0012093
        /// <summary>
        /// Pending Sign
        /// </summary>
        public class s : PX.Data.BQL.BqlString.Constant<s> { public s() : base(S) {; } }
        /// <summary>
        /// Vendor Confirm
        /// </summary>
        public class v : PX.Data.BQL.BqlString.Constant<v> { public v() : base(V) {; } }
        /// <summary>
        /// Billed
        /// </summary>
        public class b : PX.Data.BQL.BqlString.Constant<b> { public b() : base(B) {; } }

    }
    public class KGValuationTypeStringList :PXStringListAttribute
    {
        public KGValuationTypeStringList() : base(Values, Labels) { }
        public static readonly string[] Values =
         {
                _0,
                _1,
                _2,
                // 2021/06/15 Add Mantis: 0012093
                _3 ,
                //2021/07/19 Add Mantis: 0012158
                _4,
                //2021/09/28 Add Mantis: 0012247
                _5

                //2019/10/29移除:因為APInvocie已經有加Button了
                //"W",
                //"S"
        };
        public static readonly string[] Labels =
        {
                "代扣代付",
                "罰款",
                "一般扣款",
                // 2021/06/15 Add Mantis: 0012093
                "票貼" ,
                //2021/07/19 Add Mantis: 0012158
                //2021/11/05 Add Mantis: 0012264 更改名稱
                "外勞管理費",
                //2021/09/28 Add Mantis: 0012247
                "附買回扣款",

                //2019/10/29移除:因為APInvocie已經有加Button了
                //"代扣稅",
                //"健保補充"
        };

        /// <summary>
        /// 代扣代付
        /// </summary>
        public const string _0 = "0";
        /// <summary>
        /// 罰款
        /// </summary>
        public const string _1 = "1";
        /// <summary>
        /// 一般扣款
        /// </summary>
        public const string _2 = "2";

        //2021/06/15 Add Mantis: 0012093
        /// <summary>
        /// 票貼
        /// </summary>
        public const string _3 = "3";
        //2021/07/19 Add Mantis: 0012158
        /// <summary>
        /// 外勞租用扣款
        /// </summary>
        public const string _4 = "4";
        //2021/09/28 Add Mantis: 0012247
        /// <summary>
        /// 附買回扣款
        /// </summary>
        public const string _5 = "5";

        /// <summary>
        /// 代扣代付
        /// </summary>
        public class  zero: PX.Data.BQL.BqlString.Constant<zero> { public zero() : base(_0) {; } }
        /// <summary>
        /// 罰款
        /// </summary>
        public class one : PX.Data.BQL.BqlString.Constant<one> { public one() : base(_1) {; } }
        /// <summary>
        /// 一般扣款
        /// </summary>
        public class two : PX.Data.BQL.BqlString.Constant<two> { public two() : base(_2) {; } }
        //2021/06/15 Add Mantis: 0012093
        /// <summary>
        /// 會計票貼
        /// </summary>
        public class three : PX.Data.BQL.BqlString.Constant<three> { public three() : base(_3) {; } }
        //2021/07/19 Add Mantis: 0012158
        /// <summary>
        /// 外勞租用扣款
        /// </summary>
        public class four : PX.Data.BQL.BqlString.Constant<four> { public four() : base(_4) {; } }
        //2021/09/28 Add Mantis: 0012247
        /// <summary>
        /// 附買回扣款
        /// </summary>
        public class five : PX.Data.BQL.BqlString.Constant<five> { public five() : base(_5) {; } }

    }

}