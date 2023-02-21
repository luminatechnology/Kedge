using System;
using PX.Data;
using PX.Objects.CR;
using PX.Objects.CM;
using PX.Objects.EP;
using PX.Objects.PM;
using PX.Objects.GL;
using PX.Data.ReferentialIntegrity.Attributes;
using NM.Util;
using PX.Objects.CS;
using PX.Objects.AP;
using PX.Objects.PO;
using PX.Objects.CA;
using Microsoft.WindowsAzure.ActiveDirectory;
using PX.SM;
using static NM.Util.NMStringList;
using Kedge.DAC;

namespace NM.DAC
{
    /**
     *===2021/07/09 : 0012134 ===Althea
     *Add APPaymentReverseBacthNbr & WriteoffPostageReverse
     **/
    [Serializable]
    public class NMPayableCheck : IBqlTable
    {

        #region SetUp
        public abstract class setup : IBqlField
        { }
        [PXString()]
        //CS201010
        [PXDefault("PAYCHECK", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXSelector(typeof(Numbering.numberingID))]
        [PXUIField(DisplayName = "Setup")]
        public virtual string Setup { get; set; }
        #endregion

        #region Selected
        [PXBool]
        [PXUIField(DisplayName = "Selected")]
        public virtual bool? Selected { get; set; }
        public abstract class selected : PX.Data.BQL.BqlBool.Field<selected> { }
        #endregion

        #region PayableCheckID
        [PXDBIdentity()]
        [PXUIField(DisplayName = "Payable Check ID")]
        public virtual int? PayableCheckID { get; set; }
        public abstract class payableCheckID : PX.Data.BQL.BqlInt.Field<payableCheckID> { }
        #endregion

        #region PayableCheckCD
        [PXDBString(50, InputMask = "", IsKey = true)]
        [PXUIField(DisplayName = "Payable Check CD", Required = true)]
        [PXSelector(typeof(Search<payableCheckCD>), typeof(payableCheckCD), typeof(description))]
        [AutoNumber(typeof(Search<Numbering.numberingID, Where<Numbering.numberingID, Equal<Current<setup>>>>), typeof(AccessInfo.businessDate))]
        public virtual string PayableCheckCD { get; set; }
        public abstract class payableCheckCD : PX.Data.BQL.BqlString.Field<payableCheckCD> { }
        #endregion

        #region Status
        [PXDBInt()]
        [PXUIField(DisplayName = "Status", IsReadOnly = true)]
        [PXDefault(1)]
        [NMStringList.NMAPCheckStatus()]
        public virtual int? Status { get; set; }
        public abstract class status : PX.Data.BQL.BqlInt.Field<status> { }
        #endregion

        #region BranchID
        [PXDBInt()]
        [PXUIField(DisplayName = "Branch ID")]
        //[PXDefault(typeof(AccessInfo.branchID))]
        [PXDefault(typeof(Search<PX.Objects.GL.Branch.branchID, Where<PX.Objects.GL.Branch.active, Equal<True>, And<PX.Objects.GL.Branch.branchID, Equal<Current<AccessInfo.branchID>>>>>))]
        [PXDimensionSelector("BIZACCT", typeof(Search<PX.Objects.GL.Branch.branchID, Where<PX.Objects.GL.Branch.active, Equal<True>, And<MatchWithBranch<PX.Objects.GL.Branch.branchID>>>>), typeof(PX.Objects.GL.Branch.branchCD), DescriptionField = typeof(PX.Objects.GL.Branch.acctName))]
        public virtual int? BranchID { get; set; }
        public abstract class branchID : PX.Data.BQL.BqlInt.Field<branchID> { }
        #endregion

        #region PayableCashierID
        [PXDBInt()]
        [PXUIField(DisplayName = "Payable Cashier ID", Required = true)]
        [PXDefault(typeof(Search<EPEmployee.bAccountID,
            Where<EPEmployee.userID, Equal<Current<AccessInfo.userID>>>>),
            PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXEPEmployeeSelector]
        public virtual int? PayableCashierID { get; set; }
        public abstract class payableCashierID : PX.Data.BQL.BqlInt.Field<payableCashierID> { }
        #endregion

        #region SendCashierID
        [PXDBInt()]
        [PXUIField(DisplayName = "Send Cashier ID")]
        [PXEPEmployeeSelector]
        public virtual int? SendCashierID { get; set; }
        public abstract class sendCashierID : PX.Data.BQL.BqlInt.Field<sendCashierID> { }
        #endregion

        #region CashCashierID
        [PXDBInt()]
        [PXUIField(DisplayName = "Cash Cashier ID")]
        [PXEPEmployeeSelector]
        public virtual int? CashCashierID { get; set; }
        public abstract class cashCashierID : PX.Data.BQL.BqlInt.Field<cashCashierID> { }
        #endregion

        #region ModifyCashierID
        [PXDBInt()]
        [PXUIField(DisplayName = "Modify Cashier ID")]
        [PXEPEmployeeSelector]
        public virtual int? ModifyCashierID { get; set; }
        public abstract class modifyCashierID : PX.Data.BQL.BqlInt.Field<modifyCashierID> { }
        #endregion

        #region CheckDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Check Date", Required = true)]
        [PXDefault(typeof(AccessInfo.businessDate), PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual DateTime? CheckDate { get; set; }
        public abstract class checkDate : PX.Data.BQL.BqlDateTime.Field<checkDate> { }
        #endregion

        #region DueDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Due Date", Required = true)]
        [PXDefault(typeof(AccessInfo.businessDate), PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual DateTime? DueDate { get; set; }
        public abstract class dueDate : PX.Data.BQL.BqlDateTime.Field<dueDate> { }
        #endregion

        #region EtdDepositDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Etd Deposit Date", Required = true)]
        [PXDefault(typeof(AccessInfo.businessDate), PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual DateTime? EtdDepositDate { get; set; }
        public abstract class etdDepositDate : PX.Data.BQL.BqlDateTime.Field<etdDepositDate> { }
        #endregion

        #region DepositDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Deposit Date")]
        public virtual DateTime? DepositDate { get; set; }
        public abstract class depositDate : PX.Data.BQL.BqlDateTime.Field<depositDate> { }
        #endregion

        #region BankAccountID
        [PXDBInt()]
        [PXUIField(DisplayName = "Bank Account ID", Required = true)]
        [NMBankAccount(
                    SubstituteKey = typeof(NMBankAccount.bankAccountCD),
                    DescriptionField = typeof(NMBankAccount.bankNameByIssueBank)
        )]
        [PXRestrictor(typeof(Where<NMBankAccount.paymentMethodID, Equal<word.check>>), "銀行{0}的付款方式須為CHECK", typeof(NMBankAccount.bankAccountCD))]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual int? BankAccountID { get; set; }
        public abstract class bankAccountID : PX.Data.BQL.BqlInt.Field<bankAccountID> { }
        #endregion

        #region BookNbr
        [PXDBString(50, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Book Nbr")]
        [PXSelector(
            typeof(Search<NMCheckBook.bookCD,
                Where<NMCheckBook.bankAccountID, Equal<Current<NMPayableCheck.bankAccountID>>,
                    And<IsNull<NMCheckBook.bookUsage, NMBookUsage.check>, Equal<NMBookUsage.check>>
                    >>),
            typeof(NMCheckBook.bookCD),
            typeof(NMCheckBook.startCheckNbr),
            typeof(NMCheckBook.currentCheckNbr),
            typeof(NMCheckBook.endCheckNbr),
            typeof(NMCheckBook.startDate),
            typeof(NMCheckBook.description),
            DescriptionField = typeof(NMCheckBook.description)
            )]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        //[PXUIRequired(typeof(Where<isIssuedByBank,NotEqual<True>>))]
        [PXUIEnabled(typeof(Where<isIssuedByBank, NotEqual<True>>))]
        public virtual string BookNbr { get; set; }
        public abstract class bookNbr : PX.Data.BQL.BqlString.Field<bookNbr> { }
        #endregion

        #region CheckNbr
        [PXDBString(12, IsFixed = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Check Nbr", IsReadOnly = true)]
        public virtual string CheckNbr { get; set; }
        public abstract class checkNbr : PX.Data.BQL.BqlString.Field<checkNbr> { }
        #endregion

        #region CuryID
        [PXDBString()]
        [PXUIField(DisplayName = "CuryID", Required = true, Enabled = false)]
        [PXDefault(typeof(Search<PX.Objects.GL.Company.baseCuryID>), PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXSelector(typeof(Currency.curyID))]
        public virtual string CuryID { get; set; }
        public abstract class curyID : PX.Data.BQL.BqlString.Field<curyID> { }
        #endregion

        #region CuryInfoID
        [PXDBLong()]
        [CurrencyInfo()]
        public virtual Int64? CuryInfoID { get; set; }
        public abstract class curyInfoID : PX.Data.BQL.BqlLong.Field<curyInfoID> { }
        protected Int64? _CuryInfoID;
        #endregion

        #region OriCuryAmount
        [PXDBCurrency(typeof(curyInfoID), typeof(baseCuryAmount))]
        [PXUIField(DisplayName = "Ori Cury Amount", Required = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual Decimal? OriCuryAmount { get; set; }
        public abstract class oriCuryAmount : PX.Data.BQL.BqlDecimal.Field<oriCuryAmount> { }
        #endregion

        #region BaseCuryAmount
        [PXDBBaseCury()]
        [PXUIField(DisplayName = "Base Cury Amount", IsReadOnly = true)]
        public virtual Decimal? BaseCuryAmount { get; set; }
        public abstract class baseCuryAmount : PX.Data.BQL.BqlDecimal.Field<baseCuryAmount> { }
        #endregion

        #region SourceCode
        [PXDBInt()]
        [PXUIField(DisplayName = "Source Code", IsReadOnly = true)]
        [PXDefault(TypeCode.String, "1")]
        [NMStringList.NMAPSourceCode]
        public virtual int? SourceCode { get; set; }
        public abstract class sourceCode : PX.Data.BQL.BqlInt.Field<sourceCode> { }
        #endregion

        #region VendorID
        [PXDBInt()]
        [PXUIField(DisplayName = "Vendor ID", Required = true)]
        [PXSelector(typeof(Search<BAccount2.bAccountID, Where<BAccount2.status, Equal<EPConst.active>,
            And<Where<BAccount2.type, Equal<BAccountType.vendorType>,
                Or<BAccount2.type, Equal<BAccountType.combinedType>,
                Or<BAccount2.type, Equal<BAccountType.employeeType>>>>>>>),
            typeof(BAccount2.acctCD),
            typeof(BAccount2.acctName),
            typeof(BAccount2.status),
            typeof(BAccount2.defAddressID),
            typeof(BAccount2.defContactID),
            typeof(BAccount2.defLocationID),
            SubstituteKey = typeof(BAccount2.acctCD), DescriptionField = typeof(BAccount2.acctName))]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual int? VendorID { get; set; }
        public abstract class vendorID : PX.Data.BQL.BqlInt.Field<vendorID> { }
        #endregion

        #region VendorLocationID
        [PXDBInt()]
        [PXUIField(DisplayName = "Vendor Location ID"
            //, Required = true
            )]
        [PXSelector(typeof(Search<Location.locationID,
            Where<Location.bAccountID, Equal<Current<vendorID>>>>),
            typeof(Location.locationCD),
            typeof(Location.descr),
            SubstituteKey = typeof(Location.locationCD))]
        //[PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual int? VendorLocationID { get; set; }
        public abstract class vendorLocationID : PX.Data.BQL.BqlInt.Field<vendorLocationID> { }
        #endregion

        #region Title
        [PXDBString(50, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Title", Required = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual string Title { get; set; }
        public abstract class title : PX.Data.BQL.BqlString.Field<title> { }
        #endregion

        #region ProjectID
        [ProjectBase()]
        [PXRestrictor(typeof(Where<PMProject.isCancelled, Equal<False>>), PX.Objects.PM.Messages.CancelledContract, typeof(PMProject.contractCD))]
        [PXRestrictor(typeof(Where<PMProject.visibleInPO, Equal<True>, Or<PMProject.nonProject, Equal<True>>>), PX.Objects.PM.Messages.ProjectInvisibleInModule, typeof(PMProject.contractCD))]
        //[PXRestrictor(typeof(Where<PMProject.defaultBranchID, Equal<Current<AccessInfo.branchID>>, Or<PMProject.defaultBranchID, IsNull>>), "Branch Not Found.", typeof(PMProject.contractCD))]
        [PXForeignReference(typeof(Field<projectID>.IsRelatedTo<PMProject.contractID>))]
        [PXUIField(DisplayName = "Project ID", Required = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [ProjectDefault()]
        public virtual int? ProjectID { get; set; }
        public abstract class projectID : PX.Data.BQL.BqlInt.Field<projectID> { }
        #endregion

        #region ProjectPeriod
        [PXDBInt()]
        [PXDefault(TypeCode.Int32, "0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Project Period")]
        public virtual int? ProjectPeriod { get; set; }
        public abstract class projectPeriod : PX.Data.BQL.BqlInt.Field<projectPeriod> { }
        #endregion

        #region Description
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Description")]
        public virtual string Description { get; set; }
        public abstract class description : PX.Data.BQL.BqlString.Field<description> { }
        #endregion

        #region PaymentDocNo
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Payment Doc No")]
        public virtual string PaymentDocNo { get; set; }
        public abstract class paymentDocNo : PX.Data.BQL.BqlString.Field<paymentDocNo> { }
        #endregion

        #region PaymentDocDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Payment Doc Date")]
        public virtual DateTime? PaymentDocDate { get; set; }
        public abstract class paymentDocDate : PX.Data.BQL.BqlDateTime.Field<paymentDocDate> { }
        #endregion

        #region DeliverMethod
        [PXDBInt()]
        [PXUIField(DisplayName = "Deliver Method")]
        //[PXDefault(TypeCode.Int32, "3")]
        [NMStringList.NMAPDeliverMethod]
        public virtual int? DeliverMethod { get; set; }
        public abstract class deliverMethod : PX.Data.BQL.BqlInt.Field<deliverMethod> { }
        #endregion

        #region Receiver
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Receiver")]
        public virtual string Receiver { get; set; }
        public abstract class receiver : PX.Data.BQL.BqlString.Field<receiver> { }
        #endregion

        #region DeliverDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Deliver Date")]
        //[PXDefault(typeof(AccessInfo.businessDate))]
        public virtual DateTime? DeliverDate { get; set; }
        public abstract class deliverDate : PX.Data.BQL.BqlDateTime.Field<deliverDate> { }
        #endregion

        #region ModifyDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Modify Date")]
        public virtual DateTime? ModifyDate { get; set; }
        public abstract class modifyDate : PX.Data.BQL.BqlDateTime.Field<modifyDate> { }
        #endregion

        #region ModifyReason
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Modify Reason")]
        public virtual string ModifyReason { get; set; }
        public abstract class modifyReason : PX.Data.BQL.BqlString.Field<modifyReason> { }
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
        public virtual DateTime? LastModifiedDateTime { get; set; }
        public abstract class lastModifiedDateTime : PX.Data.BQL.BqlDateTime.Field<lastModifiedDateTime> { }
        #endregion

        #region NoteID
        [PXNote()]
        [PXUIField(DisplayName = "NoteID")]
        public virtual Guid? NoteID { get; set; }
        public abstract class noteID : PX.Data.BQL.BqlGuid.Field<noteID> { }
        #endregion

        #region Tstamp
        [PXDBTimestamp()]
        [PXUIField(DisplayName = "Tstamp")]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
        #endregion

        #region RefNbr
        [PXDBString(15, InputMask = "")]
        [PXUIField(DisplayName = "RefN br")]
        public virtual string RefNbr { get; set; }
        public abstract class refNbr : PX.Data.BQL.BqlString.Field<refNbr> { }
        #endregion

        #region PrintCount
        [PXDBInt()]
        [PXUIField(DisplayName = "Print Count", IsReadOnly = true)]
        [PXDefault(0, PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual int? PrintCount { get; set; }
        public abstract class printCount : PX.Data.BQL.BqlInt.Field<printCount> { }
        #endregion

        #region PrintDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Print Date", IsReadOnly = true)]
        public virtual DateTime? PrintDate { get; set; }
        public abstract class printDate : PX.Data.BQL.BqlInt.Field<printDate> { }
        #endregion

        #region PrintUser
        [PXDBGuid()]
        [PXSelector(typeof(Search<Users.pKID>),
            typeof(Users.username),
            typeof(Users.fullName),
            SubstituteKey = typeof(Users.username))]
        [PXUIField(DisplayName = "PrintUser", IsReadOnly = true)]
        public virtual Guid? PrintUser { get; set; }
        public abstract class printUser : PX.Data.BQL.BqlGuid.Field<printUser> { }
        #endregion

        #region ConfirmBatchNbr
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Confirm Batch Nbr")]
        public virtual string ConfirmBatchNbr { get; set; }
        public abstract class confirmBatchNbr : PX.Data.BQL.BqlString.Field<confirmBatchNbr> { }
        #endregion        

        #region ConfirmReverseBatchNbr
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Confirm Reverse Batch Nbr")]
        public virtual string ConfirmReverseBatchNbr { get; set; }
        public abstract class confirmReverseBatchNbr : PX.Data.BQL.BqlString.Field<confirmReverseBatchNbr> { }
        #endregion

        #region CashBatchNbr
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Cash Batch Nbr")]
        public virtual string CashBatchNbr { get; set; }
        public abstract class cashBatchNbr : PX.Data.BQL.BqlString.Field<cashBatchNbr> { }
        #endregion

        #region CashReverseBatchNbr
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Cash Reverse Batch Nbr")]
        public virtual string CashReverseBatchNbr { get; set; }
        public abstract class cashReverseBatchNbr : PX.Data.BQL.BqlString.Field<cashReverseBatchNbr> { }
        #endregion

        #region PaymentBatchNbr
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Payment Batch Nbr")]
        //[PXUIVisible(typeof(Where<confirmBatchNbr, IsNotNull>))]
        public virtual string PaymentBatchNbr { get; set; }
        public abstract class paymentBatchNbr : PX.Data.BQL.BqlString.Field<paymentBatchNbr> { }
        #endregion

        #region PaymentReverseBatchNbr
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Payment Reverse Batch Nbr")]
        //[PXUIVisible(typeof(Where<cashReverseBatchNbr, IsNotNull>))]
        public virtual string PaymentReverseBatchNbr { get; set; }
        public abstract class paymentReverseBatchNbr : PX.Data.BQL.BqlString.Field<paymentReverseBatchNbr> { }
        #endregion

        #region BillPaymentID
        [PXDBInt()]
        public virtual int? BillPaymentID { get; set; }
        public abstract class billPaymentID : PX.Data.BQL.BqlInt.Field<billPaymentID> { }
        #endregion

        #region IsIssuedByBank
        [PXDBBool()]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Is Issued By Bank")]
        public virtual bool? IsIssuedByBank { get; set; }
        public abstract class isIssuedByBank : PX.Data.BQL.BqlBool.Field<isIssuedByBank> { }
        #endregion

        //Althea ADD For Print Report
        #region PrintBatchNbr
        [PXDBString(255)]
        [PXUIField(DisplayName = "PrintBatchNbr")]
        public virtual string PrintBatchNbr { get; set; }
        public abstract class printBatchNbr : IBqlField { }
        #endregion

        #region PrintChineseAmt
        [PXDBString(255, IsUnicode = true)]
        [PXUIField(DisplayName = "PrintChineseAmt")]
        public virtual string PrintChineseAmt { get; set; }
        public abstract class printChineseAmt : IBqlField { }
        #endregion

        #region FeedbackStatus
        [PXDBString(5)]
        [PXUIField(DisplayName = "Feedback Status")]
        [NMApTnBankCheckStatus]
        public virtual string FeedbackStatus { get; set; }
        public abstract class feedbackStatus : IBqlField { }
        #endregion

        #region ConfirmAPPaymentBatchNbr
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Confirm APPayment Batch Nbr")]
        //[PXUIVisible(typeof(Where<confirmBatchNbr, IsNotNull>))]
        public virtual string ConfirmAPPaymentBatchNbr { get; set; }
        public abstract class confirmAPPaymentBatchNbr : PX.Data.BQL.BqlString.Field<confirmAPPaymentBatchNbr> { }
        #endregion

        #region ConfirmWriteoffPostageBatchNbr 
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Confirm Writeoff Postage Batch Nbr ")]
        //[PXUIVisible(typeof(Where<confirmBatchNbr, IsNotNull>))]
        public virtual string ConfirmWriteoffPostageBatchNbr { get; set; }
        public abstract class confirmWriteoffPostageBatchNbr : PX.Data.BQL.BqlString.Field<confirmWriteoffPostageBatchNbr> { }
        #endregion

        #region ConfirmPPMBatchNbr
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Confirm PPM Batch Nbr")]
        //[PXUIVisible(typeof(Where<confirmBatchNbr, IsNotNull>))]
        public virtual string ConfirmPPMBatchNbr { get; set; }
        public abstract class confirmPPMBatchNbr : PX.Data.BQL.BqlString.Field<confirmPPMBatchNbr> { }
        #endregion


        #region APPaymentReverseBatchNbr
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "APPayment Reverse BatchNbr")]
        public virtual string APPaymentReverseBatchNbr { get; set; }
        public abstract class aPPaymentReverseBatchNbr : PX.Data.BQL.BqlString.Field<aPPaymentReverseBatchNbr> { }
        #endregion

        #region WriteoffPostageReverseBatchNbr
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Writeoff Postage Reverse BatchNbr")]
        public virtual string WriteoffPostageReverseBatchNbr { get; set; }
        public abstract class writeoffPostageReverseBatchNbr : PX.Data.BQL.BqlString.Field<writeoffPostageReverseBatchNbr> { }
        #endregion

        #region PPMReverseBatchNbr
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "PPM Reverse BatchNbr")]
        public virtual string PPMReverseBatchNbr { get; set; }
        public abstract class pPMReverseBatchNbr : PX.Data.BQL.BqlString.Field<pPMReverseBatchNbr> { }
        #endregion

        #region IsManualCheckNbr
        [PXDBBool()]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Is Manual Check Nbr", IsReadOnly = true)]
        public virtual bool? IsManualCheckNbr { get; set; }
        public abstract class isManualCheckNbr : PX.Data.BQL.BqlBool.Field<isManualCheckNbr> { }
        #endregion

        #region unbound

        #region CashAccountID
        [PXInt()]
        [PXUIField(DisplayName = "Cash Account ID", IsReadOnly = true)]
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

        #region PaymentMethodID
        [PXString()]
        [PXUIField(DisplayName = "Payment Method ID", IsReadOnly = true)]
        [PXSelector(typeof(Search<PaymentMethodAccount.paymentMethodID,
            Where<PaymentMethodAccount.cashAccountID, Equal<Current<cashAccountID>>>>),
            typeof(PaymentMethodAccount.paymentMethodID))]
        public virtual string PaymentMethodID { get; set; }
        public abstract class paymentMethodID : PX.Data.BQL.BqlString.Field<paymentMethodID> { }
        #endregion

        #region APInvBatchNbr
        [PXString(255, IsUnicode = true)]
        [PXUIField(DisplayName = "APInvBatchNbr")]
        [PXSelector(typeof(Search<BatchExt.usrAccConfirmNbr, Where<Batch.module, Equal<BatchModule.moduleGL>>>))]
        [PXUnboundDefault(typeof(Search<APRegisterFinExt.usrAccConfirmNbr,
            Where<APRegister.refNbr, Equal<Current<refNbr>>>>))]
        //[PXSelector(typeof(Search<Batch.batchNbr>))]
        //[PXUnboundDefault(typeof(Search<APInvoice.batchNbr,
        //    Where<APInvoice.refNbr, Equal<Current<refNbr>>>>))]
        //[PXDBScalar(typeof(Search<APInvoice.batchNbr,
        //    Where<APInvoice.refNbr, Equal<refNbr>>>))]
        public virtual string APInvBatchNbr { get; set; }
        public abstract class aPInvBatchNbr : PX.Data.BQL.BqlString.Field<aPInvBatchNbr> { }
        #endregion

        #region ManualCheckNbr
        [PXString(12, IsFixed = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Check Nbr", Required = true)]
        [PXSelector(typeof(Search<NMPayableCheck.checkNbr,
            Where<NMPayableCheck.status, In3<NMStringList.NMAPCheckStatus.cancel, NMStringList.NMAPCheckStatus.invalid>,
                And<NMPayableCheck.bankAccountID, Equal<Current<bankAccountID>>>>>))]
        public virtual string ManualCheckNbr { get; set; }
        public abstract class manualCheckNbr : PX.Data.BQL.BqlString.Field<manualCheckNbr> { }
        #endregion

        #region ShowManual
        [PXBool()]
        [PXUnboundDefault(false)]
        public virtual bool? ShowManual { get; set; }
        public abstract class showManual : PX.Data.BQL.BqlBool.Field<showManual> { }
        #endregion

        #endregion
    }

}