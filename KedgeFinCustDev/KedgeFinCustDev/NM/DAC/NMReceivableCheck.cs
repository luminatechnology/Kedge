using System;
using PX.Data;
using PX.Data.BQL;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.AR;
using PX.Objects.CM;
using PX.Objects.CS;
using PX.Objects.EP;
using PX.Objects.PM;
using NM.Util;
using PX.Objects.CR;
using PS.DAC;

namespace NM.DAC
{
    /**
     * ===2021/08/10 :0012191 === Althea
     * Add ARPaymentBatchNbr
     * 
     **/
    [Serializable]
    public class NMReceivableCheck : IBqlTable
    {
        #region Selected
        [PXBool]
        [PXUIField(DisplayName = "Selected")]
        public virtual bool? Selected { get; set; }
        public abstract class selected : PX.Data.BQL.BqlBool.Field<selected> { }
        #endregion

        #region ReceivableCheckID
        [PXDBIdentity(IsKey = true)]
        [PXUIField(DisplayName = "Receivable Check ID")]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual int? ReceivableCheckID { get; set; }
        public abstract class receivableCheckID : PX.Data.BQL.BqlInt.Field<receivableCheckID> { }
        #endregion

        #region OriBankCode
        [PXDBString(7, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Ori Bank Code", Required =true)]
        [NMBankCode(DescriptionField = typeof(SegmentValue.descr))]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual string OriBankCode { get; set; }
        public abstract class oriBankCode : PX.Data.BQL.BqlString.Field<oriBankCode> { }
        #endregion

        #region CheckNbr
        [PXDBString(15, IsFixed = true, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
        [PXUIField(DisplayName = "Check Nbr",Required =true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual string CheckNbr { get; set; }
        public abstract class checkNbr : PX.Data.BQL.BqlString.Field<checkNbr> { }
        #endregion

        #region OriBankAccount
        [PXDBString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Ori Bank Account")]
        public virtual string OriBankAccount { get; set; }
        public abstract class oriBankAccount : PX.Data.BQL.BqlString.Field<oriBankAccount> { }
        #endregion      

        #region Status
        [PXDBInt()]
        [PXUIField(DisplayName = "Status", IsReadOnly = true)]
        [PXDefault(1)]
        [NMStringList.NMARCheckStatus]
        public virtual int? Status { get; set; }
        public abstract class status : PX.Data.BQL.BqlInt.Field<status> { }
        #endregion

        #region BranchID
        [PXDBInt()]
        [PXUIField(DisplayName = "Branch ID")]
        [PXDefault(typeof(AccessInfo.branchID))]
        public virtual int? BranchID { get; set; }
        public abstract class branchID : PX.Data.BQL.BqlInt.Field<branchID> { }
        #endregion

        #region CheckCashierID
        [PXDBInt()]
        [PXUIField(DisplayName = "Check Cashier ID",Required =true)]
        [PXEPEmployeeSelector]
        [PXDefault(typeof(Search<EPEmployee.bAccountID,
            Where<EPEmployee.userID, Equal<Current<AccessInfo.userID>>>>),
            PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual int? CheckCashierID { get; set; }
        public abstract class checkCashierID : PX.Data.BQL.BqlInt.Field<checkCashierID> { }
        #endregion

        #region CheckProcessDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Check Process Date", Required = true)]
        [PXDefault(typeof(AccessInfo.businessDate),
            PersistingCheck = PXPersistingCheck.NullOrBlank)]

        public virtual DateTime? CheckProcessDate { get; set; }
        public abstract class checkProcessDate : PX.Data.BQL.BqlDateTime.Field<checkProcessDate> { }

        #endregion

        #region CheckDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Check Date",Required =true)]
        [PXDefault(typeof(AccessInfo.businessDate),
            PersistingCheck =PXPersistingCheck.NullOrBlank)]

        public virtual DateTime? CheckDate { get; set; }
        public abstract class checkDate : PX.Data.BQL.BqlDateTime.Field<checkDate> { }
        #endregion

        #region DueDate
        [PXDBDate()]
        [PXUIField(DisplayName = " Due Date")]
        [PXDefault(typeof(AccessInfo.businessDate),
            PersistingCheck =PXPersistingCheck.Nothing)]

        public virtual DateTime? DueDate { get; set; }
        public abstract class dueDate : PX.Data.BQL.BqlDateTime.Field<dueDate> { }
        #endregion

        #region CheckInDays
        [PXDBInt()]
        [PXUIField(DisplayName = "Check In Days")]
       // [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual int? CheckInDays { get; set; }
        public abstract class checkInDays : PX.Data.BQL.BqlInt.Field<checkInDays> { }
        #endregion

        #region EtdDepositDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Etd Deposit Date", Required =true)]
        [PXDefault(typeof(AccessInfo.businessDate),
            PersistingCheck =PXPersistingCheck.NullOrBlank)]
        public virtual DateTime? EtdDepositDate { get; set; }
        public abstract class etdDepositDate : PX.Data.BQL.BqlDateTime.Field<etdDepositDate> { }
        #endregion

        #region CuryID
        [PXDBString(5, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "CuryID",Required =true,Enabled = false)]
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
        [PXUIField(DisplayName = "Ori Cury Amount",Required =true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual Decimal? OriCuryAmount { get; set; }
        public abstract class oriCuryAmount : PX.Data.BQL.BqlDecimal.Field<oriCuryAmount> { }
        #endregion

        #region BaseCuryAmount
        [PXDBBaseCury()]
        [PXUIField(DisplayName = "Base Cury Amount",Required =true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual Decimal? BaseCuryAmount { get; set; }
        public abstract class baseCuryAmount : PX.Data.BQL.BqlDecimal.Field<baseCuryAmount> { }
        #endregion

        #region CustomerID
        //[CustomerActive(Visibility = PXUIVisibility.SelectorVisible, DescriptionField = typeof(Customer.acctName), Filterable = true, TabOrder = 2)]
        //2020/10/21 Althea Modify to have AllowEdit
        [PXDBInt()]
        [PXUIField(DisplayName ="Customer")]
        //[PXSelector(typeof(Search<Customer.bAccountID,
        //    Where<Customer.status,Equal<Customer.status.active>>>),
        //    typeof(Customer.acctCD),
        //    typeof(Customer.acctName),
        //    SubstituteKey =typeof(Customer.acctCD),
        //    DescriptionField = typeof(Customer.acctName))]
        [PXSelector(typeof(Search<BAccountR.bAccountID,
            Where<BAccountR.type, In3<BAccountType.customerType, 
                                     BAccountType.vendorType, 
                                     BAccountType.employeeType, 
                                     BAccountType.combinedType>,
                And<BAccountR.status, Equal<BAccountR.status.active>>>>),
            typeof(BAccountR.acctCD),
            typeof(BAccountR.acctName),
            SubstituteKey = typeof(BAccountR.acctCD),
            DescriptionField = typeof(BAccountR.acctName))]
        public virtual int? CustomerID { get; set; }
        public abstract class customerID : PX.Data.BQL.BqlInt.Field<customerID> { }
        #endregion

        #region CustomerLocationID
        [PXDBInt()]
        [PXUIField(DisplayName = "Customer Location ID")]
        //[PXSelector(typeof(Search<LocationExtAddress.locationID,
        //    Where<LocationExtAddress.bAccountID, Equal<Current<customerID>>>>),
        //    typeof(LocationExtAddress.locationCD),
        //    typeof(LocationExtAddress.addressLine1),
        //    typeof(LocationExtAddress.addressLine2),
        //    SubstituteKey = typeof(LocationExtAddress.locationCD))]
        [PXSelector(typeof(Search<Location.locationID,
            Where<Location.bAccountID, Equal<Current<customerID>>>>),
            typeof(Location.locationCD),
            typeof(Location.descr),
            SubstituteKey = typeof(Location.locationCD))]
        //[PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual int? CustomerLocationID { get; set; }
        public abstract class customerLocationID : PX.Data.BQL.BqlInt.Field<customerLocationID> { }
        #endregion

        #region CollEmployeeID
        [PXDBInt()]
        [PXUIField(DisplayName = "Coll Employee ID")]
        [PXEPEmployeeSelector]
        [PXDefault(typeof(Search<EPEmployee.bAccountID,
            Where<EPEmployee.userID, Equal<Current<AccessInfo.userID>>>>))]
        public virtual int? CollEmployeeID { get; set; }
        public abstract class collEmployeeID : PX.Data.BQL.BqlInt.Field<collEmployeeID> { }
        #endregion

        #region IsByElse
        [PXDBBool()]
        [PXUIField(DisplayName = "Is By Else")]
        [PXDefault(false)]
        public virtual bool? IsByElse { get; set; }
        public abstract class isByElse : PX.Data.BQL.BqlBool.Field<isByElse> { }
        #endregion

        #region CheckIssuer
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Check Issuer")]
        public virtual string CheckIssuer { get; set; }
        public abstract class checkIssuer : PX.Data.BQL.BqlString.Field<checkIssuer> { }
        #endregion

        #region ProjectID
        [PXUIField(DisplayName = "Project ID", Required = true)]
        [PXRestrictor(typeof(Where<PMProject.isCancelled, Equal<False>>), PX.Objects.PM.Messages.CancelledContract, typeof(PMProject.contractCD))]
        [PXRestrictor(typeof(Where<PMProject.visibleInPO, Equal<True>, Or<PMProject.nonProject, Equal<True>>>), PX.Objects.PM.Messages.ProjectInvisibleInModule, typeof(PMProject.contractCD))]
        [PXRestrictor(typeof(Where<PMProject.defaultBranchID, Equal<Current<AccessInfo.branchID>>, Or<PMProject.defaultBranchID, IsNull>>), "Branch Not Found.", typeof(PMProject.contractCD))]
        [ProjectBaseAttribute()]
        [PXForeignReference(typeof(Field<projectID>.IsRelatedTo<PMProject.contractID>))]
        [ProjectDefault()]
        public virtual int? ProjectID { get; set; }
        public abstract class projectID : PX.Data.BQL.BqlInt.Field<projectID> { }
        #endregion

        #region ProjectPeriod
        [PXDBInt()]
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

        #region ARAccountID
        [PXDBInt()]
        [PXUIField(DisplayName = "ARAccount ID")]
        public virtual int? ARAccountID { get; set; }
        public abstract class aRAccountID : PX.Data.BQL.BqlInt.Field<aRAccountID> { }
        #endregion

        #region ARSubaccountID
        [PXDBInt()]
        [PXUIField(DisplayName = "ARSubaccount ID")]
        public virtual int? ARSubaccountID { get; set; }
        public abstract class aRSubaccountID : PX.Data.BQL.BqlInt.Field<aRSubaccountID> { }
        #endregion

        #region Module
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Module")]
        public virtual string Module { get; set; }
        public abstract class module : PX.Data.BQL.BqlString.Field<module> { }
        #endregion

        #region Arrefnbr
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Arrefnbr")]
        public virtual string Arrefnbr { get; set; }
        public abstract class arrefnbr : PX.Data.BQL.BqlString.Field<arrefnbr> { }
        #endregion

        #region PayRefNbr
        [PXDBString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Pay Ref Nbr")]
        public virtual string PayRefNbr { get; set; }
        public abstract class payRefNbr : PX.Data.BQL.BqlString.Field<payRefNbr> { }
        #endregion

        #region ReceiptNbr
        [PXString(15, IsFixed = true, IsUnicode = true)]
        [PXUIField(DisplayName = "Receipt Nbr",IsReadOnly =true)]
        [PXUnboundDefault(typeof(Search2<PSPaymentSlip.refNbr,
            InnerJoin<PSPaymentSlipDetails, On<PSPaymentSlipDetails.refNbr, Equal<PSPaymentSlip.refNbr>>>,
            Where<PSPaymentSlipDetails.arPaymentRefNbr, Equal<Current<payRefNbr>>>
            >))]
        public virtual string ReceiptNbr { get; set; }
        public abstract class receiptNbr : PX.Data.BQL.BqlString.Field<receiptNbr> { }
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

        #region EtdCollBankAccountID
        [PXDBInt()]
        [PXUIField(DisplayName = "Etd Coll Bank Account ID")]
        [NMBankAccount()]
        public virtual int? EtdCollBankAccountID { get; set; }
        public abstract class etdCollBankAccountID : PX.Data.BQL.BqlInt.Field<etdCollBankAccountID> { }
        #endregion

        //Check For Receive
        #region Receive Check
        #region RecBatchNbr
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "ReceiveBatchNbr")]
        public virtual string RecBatchNbr { get; set; }
        public abstract class recBatchNbr : PX.Data.BQL.BqlString.Field<recBatchNbr> { }
        #endregion

        #region RecReverseBatchNbr
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Receive Reverse Batch Nbr")]
        public virtual string RecReverseBatchNbr { get; set; }
        public abstract class recReverseBatchNbr : PX.Data.BQL.BqlString.Field<recReverseBatchNbr> { }
        #endregion
        #endregion

        //Check For Collection
        #region Collection Check

        #region CollBankAccountID
        [PXDBInt()]
        [PXUIField(DisplayName = "Coll Bank Account ID")]
        [NMBankAccount()]
        public virtual int? CollBankAccountID { get; set; }
        public abstract class collBankAccountID : PX.Data.BQL.BqlInt.Field<collBankAccountID> { }
        #endregion

        #region BankAccount
        [PXString(50, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Bank Account",IsReadOnly =true)]
       
        public virtual string BankAccount { get; set; }
        public abstract class bankAccount : PX.Data.BQL.BqlString.Field<bankAccount> { }
        #endregion

        #region CollCashierID
        [PXDBInt()]
        [PXUIField(DisplayName = "Coll Cashier ID")]
        [PXEPEmployeeSelector]       
        public virtual int? CollCashierID { get; set; }
        public abstract class collCashierID : PX.Data.BQL.BqlInt.Field<collCashierID> { }
        #endregion

        #region CollCheckDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Coll Check Date",Required =true)]
        
        public virtual DateTime? CollCheckDate { get; set; }
        public abstract class collCheckDate : PX.Data.BQL.BqlDateTime.Field<collCheckDate> { }
        #endregion


        #region CollBatchNbr
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Coll Batch Nbr")]
        public virtual string CollBatchNbr { get; set; }
        public abstract class collBatchNbr : PX.Data.BQL.BqlString.Field<collBatchNbr> { }
        #endregion

        #region CollReverseBatchNbr
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Coll Reverse Batch Nbr")]
        public virtual string CollReverseBatchNbr { get; set; }
        public abstract class collReverseBatchNbr : PX.Data.BQL.BqlString.Field<collReverseBatchNbr> { }
        #endregion

        #endregion


        //Check For Cash
        #region Cash Cehck     

        #region CashCashierID
        [PXDBInt()]
        [PXUIField(DisplayName = "Cash Cashier ID")]
        [PXEPEmployeeSelector]
        //[PXDefault(typeof(Search<EPEmployee.bAccountID,
        //    Where<EPEmployee.userID, Equal<Current<AccessInfo.userID>>>>))]
        public virtual int? CashCashierID { get; set; }
        public abstract class cashCashierID : PX.Data.BQL.BqlInt.Field<cashCashierID> { }
        #endregion

        #region DepositDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Deposit Date")]
        //[PXDefault(typeof(AccessInfo.businessDate))]

        public virtual DateTime? DepositDate { get; set; }
        public abstract class depositDate : PX.Data.BQL.BqlDateTime.Field<depositDate> { }
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

        #endregion


        //Check For Modify
        #region Modify Check

        #region ModifyCashierID
        [PXDBInt()]
        [PXUIField(DisplayName = "Modify Cashier ID")]
        [PXEPEmployeeSelector]
        //[PXDefault(typeof(Search<EPEmployee.bAccountID,
        //    Where<EPEmployee.userID, Equal<Current<AccessInfo.userID>>>>))]
        public virtual int? ModifyCashierID { get; set; }
        public abstract class modifyCashierID : PX.Data.BQL.BqlInt.Field<modifyCashierID> { }
        #endregion

        #region ReverseDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Reverse Date")]
        //[PXDefault(typeof(AccessInfo.businessDate))]
        public virtual DateTime? ReverseDate { get; set; }
        public abstract class reverseDate : PX.Data.BQL.BqlDateTime.Field<reverseDate> { }
        #endregion

        #region Reason
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Reason")]
        public virtual string Reason { get; set; }
        public abstract class reason : PX.Data.BQL.BqlString.Field<reason> { }
        #endregion

        #endregion

        //2021/08/10 Add Mantis: 0012191 By Althea
        #region ARPaymentBatchNbr
        [PXString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "ARPaymentBatchNbr")]
        [PXUnboundDefault(typeof(Search<ARPayment.batchNbr,
            Where<ARPayment.refNbr,Equal<Current<payRefNbr>>>>),
            PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual string ARPaymentBatchNbr { get; set; }
        public abstract class arPaymentBatchNbr : PX.Data.BQL.BqlString.Field<arPaymentBatchNbr> { }
        #endregion
    }



}