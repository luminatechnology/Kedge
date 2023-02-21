using System;
using NM.Util;
using PX.Data;
using PX.Data.BQL;
using PX.Objects.CA;
using PX.Objects.CM;
using PX.Objects.CS;
using PX.Objects.GL;

namespace NM.DAC
{
    [Serializable]
    public class NMBankAccount : IBqlTable
    {

        public const string NMBANKCONST = "NMBANK";

        public class nmbankconst : BqlString.Constant<nmbankconst> { public nmbankconst() : base(NMBANKCONST) { } }

        #region BankAccountID
        [PXDBIdentity()]
        [PXUIField(DisplayName = "Bank Account ID")]
        public virtual int? BankAccountID { get; set; }
        public abstract class bankAccountID : PX.Data.BQL.BqlInt.Field<bankAccountID> { }
        #endregion

        #region BankAccountCD
        [PXDBString(50, IsKey = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Bank Account CD", Required = true)]
        [PXSelector(typeof(Search<bankAccountCD>),
                    typeof(bankAccountCD),
                    typeof(bankShortName),
                    typeof(bankName),
                    typeof(description)
        )]
        public virtual string BankAccountCD { get; set; }
        public abstract class bankAccountCD : PX.Data.BQL.BqlString.Field<bankAccountCD> { }
        #endregion

        #region BankName
        [PXDBString(50, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Bank Name", Required = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual string BankName { get; set; }
        public abstract class bankName : PX.Data.BQL.BqlString.Field<bankName> { }
        #endregion

        #region BankShortName
        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Bank Short Name", Required = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual string BankShortName { get; set; }
        public abstract class bankShortName : PX.Data.BQL.BqlString.Field<bankShortName> { }
        #endregion

        #region BankAddress
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Bank Address")]
        public virtual string BankAddress { get; set; }
        public abstract class bankAddress : PX.Data.BQL.BqlString.Field<bankAddress> { }
        #endregion

        #region BankPhone
        [PXDBString(50, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Bank Phone")]
        public virtual string BankPhone { get; set; }
        public abstract class bankPhone : PX.Data.BQL.BqlString.Field<bankPhone> { }
        #endregion

        #region BankCode
        [PXDBString(7, IsFixed = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Bank Code", Required = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [NMBankCode]
        public virtual string BankCode { get; set; }
        public abstract class bankCode : PX.Data.BQL.BqlString.Field<bankCode> { }
        #endregion

        #region BankAccount
        [PXDBString(50, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Bank Account", Required = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual string BankAccount { get; set; }
        public abstract class bankAccount : PX.Data.BQL.BqlString.Field<bankAccount> { }
        #endregion

        #region BranchID    
        [PXDBInt()]
        [PXUIField(DisplayName = "Branch ID")]
        [PXDefault(typeof(AccessInfo.branchID))]
        public virtual int? BranchID { get; set; }
        public abstract class branchID : PX.Data.BQL.BqlInt.Field<branchID> { }
        #endregion

        #region Contactor
        [PXDBString(50, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Contactor")]
        public virtual string Contactor { get; set; }
        public abstract class contactor : PX.Data.BQL.BqlString.Field<contactor> { }
        #endregion

        #region CuryID
        [PXDBString(5, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Cury ID", Required = true, IsReadOnly = true)]
        //[PXDefault(typeof(Search<PX.Objects.GL.Company.baseCuryID>), PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXSelector(typeof(Currency.curyID))]
        public virtual string CuryID { get; set; }
        public abstract class curyID : PX.Data.BQL.BqlString.Field<curyID> { }
        #endregion

        #region CashAccountID
        [PXDBInt()]
        [PXUIField(DisplayName = "Cash Account ID", Required = true)]
        [PXSelector(typeof(Search<CashAccount.cashAccountID, Where<CashAccount.active, Equal<True>>>),
                    typeof(CashAccount.cashAccountCD),
                    typeof(CashAccount.descr),
                    typeof(CashAccount.accountID),
                    typeof(CashAccount.subID),
                    SubstituteKey = typeof(CashAccount.cashAccountCD),
                    DescriptionField = typeof(CashAccount.descr)
                   )]
        public virtual int? CashAccountID { get; set; }
        public abstract class cashAccountID : PX.Data.BQL.BqlInt.Field<cashAccountID> { }
        #endregion

        #region AccountID
        [PXDBInt()]
        [PXUIField(DisplayName = "Account ID")]
        public virtual int? AccountID { get; set; }
        public abstract class accountID : PX.Data.BQL.BqlInt.Field<accountID> { }
        #endregion

        #region SubID
        [PXDBInt()]
        [PXUIField(DisplayName = "Sub ID")]
        public virtual int? SubID { get; set; }
        public abstract class subID : PX.Data.BQL.BqlInt.Field<subID> { }
        #endregion

        #region AccountType
        [PXDBString(2, IsFixed = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Account Type", Required = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        //[NMSegmentKey()]
        //[PXRestrictor(typeof(Where<SegmentValue.dimensionID, Equal<NMSegmentKey.nMBankAccountType>>), "")]
        //[PXRestrictor(typeof(Where<SegmentValue.segmentID, Equal<NMSegmentKey.segmentIDPart1>>), "")]
        [PXSelector(typeof(Search<SegmentValue.value,
                       Where<SegmentValue.active, Equal<True>,
                           And<SegmentValue.dimensionID, Equal<NMSegmentKey.nMBankAccountType>,
                               And<SegmentValue.segmentID, Equal<NMSegmentKey.segmentIDPart1>>>>>),
               typeof(SegmentValue.value),
               typeof(SegmentValue.descr),
            DescriptionField = typeof(SegmentValue.descr))]
        public virtual string AccountType { get; set; }
        public abstract class accountType : PX.Data.BQL.BqlString.Field<accountType> { }
        #endregion

        #region AccountName
        [PXDBString(60, IsFixed = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Account Name", Required = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual string AccountName { get; set; }
        public abstract class accountName : PX.Data.BQL.BqlString.Field<accountName> { }
        #endregion

        #region Description
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Description")]
        public virtual string Description { get; set; }
        public abstract class description : PX.Data.BQL.BqlString.Field<description> { }
        #endregion

        #region IsSettlement
        [PXDBBool()]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Is Settlement", Required = true)]
        public virtual bool? IsSettlement { get; set; }
        public abstract class isSettlement : PX.Data.BQL.BqlBool.Field<isSettlement> { }
        #endregion

        #region ActivationDate
        [PXDBDate]
        [PXUIField(DisplayName = "Activation Date", Required = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual DateTime? ActivationDate { get; set; }
        public abstract class activationDate : PX.Data.BQL.BqlDateTime.Field<activationDate> { }
        #endregion

        #region SettlementDate
        [PXDBDate]
        [PXUIField(DisplayName = "Settlement Date", IsReadOnly = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual DateTime? SettlementDate { get; set; }
        public abstract class settlementDate : PX.Data.BQL.BqlDateTime.Field<settlementDate> { }
        #endregion

        #region CreatedByID
        [PXDBCreatedByID()]
        public virtual Guid? CreatedByID { get; set; }
        public abstract class createdByID : PX.Data.BQL.BqlGuid.Field<createdByID> { }
        #endregion

        #region CreatedByScreenID
        [PXDBCreatedByScreenID()]
        public virtual string CreatedByScreenID { get; set; }
        public abstract class createdByScreenID : PX.Data.BQL.BqlString.Field<createdByScreenID> { }
        #endregion

        #region CreatedDateTime
        [PXDBCreatedDateTime()]
        [PXUIField(DisplayName = "Created Date Time")]
        public virtual DateTime? CreatedDateTime { get; set; }
        public abstract class createdDateTime : PX.Data.BQL.BqlDateTime.Field<createdDateTime> { }
        #endregion

        #region LastModifiedByID
        [PXDBLastModifiedByID()]
        public virtual Guid? LastModifiedByID { get; set; }
        public abstract class lastModifiedByID : PX.Data.BQL.BqlGuid.Field<lastModifiedByID> { }
        #endregion

        #region LastModifiedByScreenID
        [PXDBLastModifiedByScreenID()]
        public virtual string LastModifiedByScreenID { get; set; }
        public abstract class lastModifiedByScreenID : PX.Data.BQL.BqlString.Field<lastModifiedByScreenID> { }
        #endregion

        #region LastModifiedDateTime
        [PXDBLastModifiedDateTime()]
        [PXUIField(DisplayName = "Last Modified Date Time")]
        public virtual DateTime? LastModifiedDateTime { get; set; }
        public abstract class lastModifiedDateTime : PX.Data.BQL.BqlDateTime.Field<lastModifiedDateTime> { }
        #endregion

        #region Noteid
        [PXNote()]
        [PXUIField(DisplayName = "Noteid")]
        public virtual Guid? Noteid { get; set; }
        public abstract class noteid : PX.Data.BQL.BqlGuid.Field<noteid> { }
        #endregion

        #region Tstamp
        [PXDBTimestamp()]
        [PXUIField(DisplayName = "Tstamp")]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
        #endregion

        #region PaymentMethodID
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Payment Method ID", Required = true)]
        [PXSelector(typeof(Search<PaymentMethodAccount.paymentMethodID,
            Where<PaymentMethodAccount.cashAccountID, Equal<Current<cashAccountID>>>>),
            typeof(PaymentMethodAccount.paymentMethodID))]
        public virtual string PaymentMethodID { get; set; }
        public abstract class paymentMethodID : PX.Data.BQL.BqlString.Field<paymentMethodID> { }
        #endregion

        #region EnableIssueByBank
        [PXDBBool()]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Enable Issue By Bank")]
        public virtual bool? EnableIssueByBank { get; set; }
        public abstract class enableIssueByBank : PX.Data.BQL.BqlBool.Field<enableIssueByBank> { }
        #endregion

        #region IsCheckPrintAble
        [PXDBBool()]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Is CheckPrintAble")]
        public virtual bool? IsCheckPrintAble { get; set; }
        public abstract class isCheckPrintAble : PX.Data.BQL.BqlBool.Field<isCheckPrintAble> { }
        #endregion

        #region RestrictedAmount
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Restricted Amount")]
        public virtual Decimal? RestrictedAmount { get; set; }
        public abstract class restrictedAmount : PX.Data.BQL.BqlDecimal.Field<restrictedAmount> { }
        #endregion

        #region unbound
        #region BankNameByIssueBank
        [PXString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Bank Name By IssueBank",IsReadOnly = true)]
        [PXUnboundDefault(typeof(Add<bankName,
            Switch<Case<Where<enableIssueByBank,Equal<True>>, issueBankInfo>, breakStr>>))]

        public virtual string BankNameByIssueBank { get; set; }
        public abstract class bankNameByIssueBank : PX.Data.BQL.BqlString.Field<bankNameByIssueBank> { }
        #endregion
        #endregion

        #region const

        public class issueBankInfo : PX.Data.BQL.BqlString.Constant<issueBankInfo> { public issueBankInfo() : base("(¥N¶}»È¦æ)") {; } }
        public class breakStr : PX.Data.BQL.BqlString.Constant<breakStr> { public breakStr() : base("") {; } }
        #endregion
    }

}