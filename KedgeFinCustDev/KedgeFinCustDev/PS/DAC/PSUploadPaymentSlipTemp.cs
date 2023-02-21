using System;
using KG.Util;
using NM.DAC;
using NM.Util;
using PX.Data;
using PX.Objects.CA;
using PX.Objects.CR;
using PX.Objects.CT;
using PX.Objects.IN;
using PX.Objects.PO;
using static CC.Util.CCList;
using static PS.Util.PSStringList;

namespace PS.DAC
{
    [Serializable]
    [PXCacheName("PSUploadPaymentSlipTemp")]
    public class PSUploadPaymentSlipTemp : IBqlTable
    {
        #region TempSeq
        [PXDBIdentity(IsKey = true)]
        public virtual int? TempSeq { get; set; }
        public abstract class tempSeq : PX.Data.BQL.BqlInt.Field<tempSeq> { }
        #endregion

        #region DocType
        [PXDBString(25, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Doc Type")]
        [PSDocType]
        public virtual string DocType { get; set; }
        public abstract class docType : PX.Data.BQL.BqlString.Field<docType> { }
        #endregion

        #region RefNbr
        [PXDBString(25, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Ref Nbr")]
        public virtual string RefNbr { get; set; }
        public abstract class refNbr : PX.Data.BQL.BqlString.Field<refNbr> { }
        #endregion

        #region DocDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Doc Date")]
        public virtual DateTime? DocDate { get; set; }
        public abstract class docDate : PX.Data.BQL.BqlDateTime.Field<docDate> { }
        #endregion

        #region TargetType
        [PXDBString(1, IsFixed = true, InputMask = "")]
        [PXUIField(DisplayName = "Target Type")]
        [PSTargetType]
        public virtual string TargetType { get; set; }
        public abstract class targetType : PX.Data.BQL.BqlString.Field<targetType> { }
        #endregion

        #region PayerID
        [PXDBInt()]
        [PXUIField(DisplayName = "Payer ID")]
        [PXSelector(
            typeof(Search<BAccount.bAccountID>),
            typeof(BAccount.acctCD),
            typeof(BAccount.acctName),
            SubstituteKey = typeof(BAccount.acctCD),
            DescriptionField = typeof(BAccount.acctName)
            )]
        public virtual int? PayerID { get; set; }
        public abstract class payerID : PX.Data.BQL.BqlInt.Field<payerID> { }
        #endregion

        #region PayerLocationID
        [PXDBInt()]
        [PXUIField(DisplayName = "Payer Location ID")]
        [PXSelector(
            typeof(Search<Location.locationID, Where<Location.bAccountID, Equal<Current<payerID>>>>),
            typeof(Location.locationCD),
            typeof(Location.descr),
            SubstituteKey = typeof(Location.locationCD),
            DescriptionField = typeof(Location.descr)
            )]
        public virtual int? PayerLocationID { get; set; }
        public abstract class payerLocationID : PX.Data.BQL.BqlInt.Field<payerLocationID> { }
        #endregion

        #region EmployeeID
        [PXDBInt()]
        [PXUIField(DisplayName = "Employee ID")]
        [PXSelector(
            typeof(Search<BAccount.bAccountID>),
            typeof(BAccount.acctCD),
            typeof(BAccount.acctName),
            SubstituteKey = typeof(BAccount.acctCD),
            DescriptionField = typeof(BAccount.acctName)
            )]
        public virtual int? EmployeeID { get; set; }
        public abstract class employeeID : PX.Data.BQL.BqlInt.Field<employeeID> { }
        #endregion

        #region ContractID
        [PXDBInt()]
        [PXUIField(DisplayName = "Project ID")]
        [PXSelector(
            typeof(Search<Contract.contractID>),
            typeof(Contract.contractCD),
            typeof(Contract.description),
            SubstituteKey = typeof(Contract.contractCD),
            DescriptionField = typeof(Contract.description)
            )]
        public virtual int? ContractID { get; set; }
        public abstract class contractID : PX.Data.BQL.BqlInt.Field<contractID> { }
        #endregion

        #region DocDesc
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Doc Desc")]
        public virtual string DocDesc { get; set; }
        public abstract class docDesc : PX.Data.BQL.BqlString.Field<docDesc> { }
        #endregion

        #region LineDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Line Date")]
        public virtual DateTime? LineDate { get; set; }
        public abstract class lineDate : PX.Data.BQL.BqlDateTime.Field<lineDate> { }
        #endregion

        #region LinePayNbr
        [PXDBString(25, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Line Pay Nbr")]
        public virtual string LinePayNbr { get; set; }
        public abstract class linePayNbr : PX.Data.BQL.BqlString.Field<linePayNbr> { }
        #endregion

        #region LinePayMehtod
        [PXDBString(20, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Line Pay Mehtod")]
        [PXSelector(typeof(Search<PaymentMethod.paymentMethodID,
            Where<True, Equal<True>>>),
            typeof(PaymentMethod.paymentMethodID))]
        public virtual string LinePayMehtod { get; set; }
        public abstract class linePayMehtod : PX.Data.BQL.BqlString.Field<linePayMehtod> { }
        #endregion

        #region LineInvID
        //[PXDBInt()]
        //[PXUIField(DisplayName = "Line Inv ID")]
        [Inventory(DisplayName = "Line Inv ID")]
        public virtual int? LineInvID { get; set; }
        public abstract class lineInvID : PX.Data.BQL.BqlInt.Field<lineInvID> { }
        #endregion

        #region LineDesc
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Line Desc")]
        public virtual string LineDesc { get; set; }
        public abstract class lineDesc : PX.Data.BQL.BqlString.Field<lineDesc> { }
        #endregion

        #region LineQty
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Line Qty")]
        public virtual Decimal? LineQty { get; set; }
        public abstract class lineQty : PX.Data.BQL.BqlDecimal.Field<lineQty> { }
        #endregion

        #region LineCost
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Line Cost")]
        public virtual Decimal? LineCost { get; set; }
        public abstract class lineCost : PX.Data.BQL.BqlDecimal.Field<lineCost> { }
        #endregion

        #region LineIssues
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Line Issues")]
        public virtual string LineIssues { get; set; }
        public abstract class lineIssues : PX.Data.BQL.BqlString.Field<lineIssues> { }
        #endregion

        #region IssueBankCode
        [PXDBString(7, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Issue Bank Code")]
        public virtual string IssueBankCode { get; set; }
        public abstract class issueBankCode : PX.Data.BQL.BqlString.Field<issueBankCode> { }
        #endregion

        #region IssueBankAccount
        [PXDBString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Issue Bank Account")]
        public virtual string IssueBankAccount { get; set; }
        public abstract class issueBankAccount : PX.Data.BQL.BqlString.Field<issueBankAccount> { }
        #endregion

        #region PaymentCategory
        [PXDBString(3, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Payment Category")]
        [PSPaymentCategory()]
        public virtual string PaymentCategory { get; set; }
        public abstract class paymentCategory : PX.Data.BQL.BqlString.Field<paymentCategory> { }
        #endregion

        #region BankAccountID
        [PXDBInt()]
        [PXUIField(DisplayName = "Bank Account ID")]
        [NMBankAccount(
                    SubstituteKey = typeof(NMBankAccount.bankAccountCD),
                    DescriptionField = typeof(NMBankAccount.bankName)
        )]
        [PXRestrictor(typeof(Where<NMBankAccount.paymentMethodID, Equal<Current<linePayMehtod>>,
            Or<Current<linePayMehtod>, Equal<KGConst.cash>>
            >), "PaymentMethod Not Found", typeof(NMBankAccount.paymentMethodID))]
        public virtual int? BankAccountID { get; set; }
        public abstract class bankAccountID : PX.Data.BQL.BqlInt.Field<bankAccountID> { }
        #endregion

        #region ActualDueDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Actual Due Date")]
        public virtual DateTime? ActualDueDate { get; set; }
        public abstract class actualDueDate : PX.Data.BQL.BqlDateTime.Field<actualDueDate> { }
        #endregion

        #region CheckDueDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Check Due Date")]
        public virtual DateTime? CheckDueDate { get; set; }
        public abstract class checkDueDate : PX.Data.BQL.BqlDateTime.Field<checkDueDate> { }
        #endregion

        #region EtdDepositDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Etd Deposit Date")]
        public virtual DateTime? EtdDepositDate { get; set; }
        public abstract class etdDepositDate : PX.Data.BQL.BqlDateTime.Field<etdDepositDate> { }
        #endregion

        #region ProcessDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Process Date")]
        public virtual DateTime? ProcessDate { get; set; }
        public abstract class processDate : PX.Data.BQL.BqlDateTime.Field<processDate> { }
        #endregion

        #region GuarType
        [PXDBString(1, IsFixed = true)]
        [PXUIField(DisplayName = "Guar Type")]
        [GuarTypeList]
        public virtual string GuarType { get; set; }
        public abstract class guarType : PX.Data.BQL.BqlString.Field<guarType> { }
        #endregion

        #region GuarClass
        [PXDBString(2, IsFixed = true)]
        [PXUIField(DisplayName = "Guar Class")]
        [GuarClassList]
        public virtual string GuarClass { get; set; }
        public abstract class guarClass : PX.Data.BQL.BqlString.Field<guarClass> { }
        #endregion

        #region POOrderType
        [PXDBString(2, IsFixed = true, IsUnicode = true)]
        [PXUIField(DisplayName = "POOrderType")]
        public virtual string POOrderType { get; set; }
        public abstract class pOOrderType : PX.Data.BQL.BqlString.Field<pOOrderType> { }
        #endregion

        #region PONbr
        [PXDBString(15, IsUnicode = true)]
        [PXUIField(DisplayName = "PONbr")]
        [PXSelector(typeof(Search<POOrder.orderNbr,
            Where<POOrder.vendorID, Equal<Current<payerID>>,
                And<POOrder.projectID, Equal<Current<contractID>>>>>),
            typeof(POOrder.orderType),
            typeof(POOrder.orderNbr),
            typeof(POOrder.orderDesc),
            typeof(POOrder.orderDate),
            DescriptionField = typeof(POOrder.orderDesc))]
        public virtual string PONbr { get; set; }
        public abstract class pONbr : PX.Data.BQL.BqlString.Field<pONbr> { }
        #endregion

        #region IssueDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Issue Date")]
        public virtual DateTime? IssueDate { get; set; }
        public abstract class issueDate : PX.Data.BQL.BqlDateTime.Field<issueDate> { }
        #endregion

        #region AuthDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Auth Date")]
        public virtual DateTime? AuthDate { get; set; }
        public abstract class authDate : PX.Data.BQL.BqlDateTime.Field<authDate> { }
        #endregion

        #region Error
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Error")]
        public virtual string Error { get; set; }
        public abstract class error : PX.Data.BQL.BqlString.Field<error> { }
        #endregion
    }
}