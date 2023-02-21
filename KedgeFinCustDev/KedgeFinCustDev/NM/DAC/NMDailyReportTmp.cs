using System;
using PX.Data;
using static NM.Util.NMStringList;

namespace NM.DAC
{
    [Serializable]
    [PXCacheName("NMDailyReportTmp")]
    public class NMDailyReportTmp : IBqlTable
    {
        #region BranchID
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Branch ID")]
        public virtual int? BranchID { get; set; }
        public abstract class branchID : PX.Data.BQL.BqlInt.Field<branchID> { }
        #endregion

        #region SettlementID
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Settlement ID")]
        [PXDBDefault(typeof(NMSettlementLog.settlementID))]
        [PXParent(typeof(Select<NMSettlementLog,
                            Where<NMSettlementLog.settlementID,
                            Equal<Current<settlementID>>>>))]
        public virtual int? SettlementID { get; set; }
        public abstract class settlementID : PX.Data.BQL.BqlInt.Field<settlementID> { }
        #endregion

        #region Seq
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Seq")]
        public virtual int? Seq { get; set; }
        public abstract class seq : PX.Data.BQL.BqlInt.Field<seq> { }
        #endregion

        #region SettledType
        [PXDBString(1, IsFixed = true, InputMask = "",IsKey = true)]
        [PXUIField(DisplayName = "Settled Type")]
        [NMSettledType]
        public virtual string SettledType { get; set; }
        public abstract class settledType : PX.Data.BQL.BqlString.Field<settledType> { }
        #endregion

        #region CashAccountID
        [PXDBInt()]
        [PXUIField(DisplayName = "Cash Account ID")]
        public virtual int? CashAccountID { get; set; }
        public abstract class cashAccountID : PX.Data.BQL.BqlInt.Field<cashAccountID> { }
        #endregion

        #region BankShortName
        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Bank Short Name")]
        public virtual string BankShortName { get; set; }
        public abstract class bankShortName : PX.Data.BQL.BqlString.Field<bankShortName> { }
        #endregion

        #region BankCode
        [PXDBString(7, IsFixed = true, IsUnicode = true)]
        [PXUIField(DisplayName = "Bank Code")]
        [Util.NMBankCode]
        public virtual string BankCode { get; set; }
        public abstract class bankCode : PX.Data.BQL.BqlString.Field<bankCode> { }
        #endregion

        #region BankAccount
        [PXDBString(50, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Bank Account")]
        public virtual string BankAccount { get; set; }
        public abstract class bankAccount : PX.Data.BQL.BqlString.Field<bankAccount> { }
        #endregion

        #region AccountTypeDesc
        [PXDBString(60, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Account Type Desc")]
        public virtual string AccountTypeDesc { get; set; }
        public abstract class accountTypeDesc : PX.Data.BQL.BqlString.Field<accountTypeDesc> { }
        #endregion

        #region NMSettlementDate
        [PXDBDate()]
        [PXUIField(DisplayName = "NMSettlement Date")]
        public virtual DateTime? NMSettlementDate { get; set; }
        public abstract class nMSettlementDate : PX.Data.BQL.BqlDateTime.Field<nMSettlementDate> { }
        #endregion

        #region AccountID
        [PXDBInt()]
        [PXUIField(DisplayName = "Account ID")]
        public virtual int? AccountID { get; set; }
        public abstract class accountID : PX.Data.BQL.BqlInt.Field<accountID> { }
        #endregion

        #region Subid
        [PXDBInt()]
        [PXUIField(DisplayName = "Subid")]
        public virtual int? Subid { get; set; }
        public abstract class subid : PX.Data.BQL.BqlInt.Field<subid> { }
        #endregion

        #region BegBalance
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Beg Balance")]
        public virtual Decimal? BegBalance { get; set; }
        public abstract class begBalance : PX.Data.BQL.BqlDecimal.Field<begBalance> { }
        #endregion

        #region CuryDebitAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Cury Debit Amt")]
        public virtual Decimal? CuryDebitAmt { get; set; }
        public abstract class curyDebitAmt : PX.Data.BQL.BqlDecimal.Field<curyDebitAmt> { }
        #endregion

        #region CuryDebitCnt
        [PXDBInt()]
        [PXUIField(DisplayName = "Cury Debit Cnt")]
        public virtual int? CuryDebitCnt { get; set; }
        public abstract class curyDebitCnt : PX.Data.BQL.BqlInt.Field<curyDebitCnt> { }
        #endregion

        #region CuryCreditAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Cury Credit Amt")]
        public virtual Decimal? CuryCreditAmt { get; set; }
        public abstract class curyCreditAmt : PX.Data.BQL.BqlDecimal.Field<curyCreditAmt> { }
        #endregion

        #region CuryCreditCnt
        [PXDBInt()]
        [PXUIField(DisplayName = "Cury Credit Cnt")]
        public virtual int? CuryCreditCnt { get; set; }
        public abstract class curyCreditCnt : PX.Data.BQL.BqlInt.Field<curyCreditCnt> { }
        #endregion

        #region RestrictedAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Restricted Amt")]
        public virtual Decimal? RestrictedAmt { get; set; }
        public abstract class restrictedAmt : PX.Data.BQL.BqlDecimal.Field<restrictedAmt> { }
        #endregion

        #region CashAccountAlias
        [PXDBString(40, IsUnicode = true)]
        [PXUIField(DisplayName = "Cash Account Alias")]
        public virtual string CashAccountAlias { get; set; }
        public abstract class cashAccountAlias : PX.Data.BQL.BqlString.Field<cashAccountAlias> { }
        #endregion
    }
}