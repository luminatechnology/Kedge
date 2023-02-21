using System;
using NM.DAC;
using NM.Util;
using PX.Data;
using PX.Data.ReferentialIntegrity.Attributes;
using CS = PX.Objects.CS;
using PX.Objects.EP;
using PX.Objects.AR;
using PX.Objects.CR;
using PX.Objects.AP;
using PX.Objects.PO;
using PX.Objects.PM;
using static CC.Util.CCList;

namespace CC.DAC
{
    /**
     * ===2021/10/05 :0012252 ===Althea
     * Add CCPostageAmt decimal(18,6)
     * When Update : GuarAmt = GuarAmt - CCPostageAmt
     **/
    [Serializable]
    public class CCPayableCheck : IBqlTable
    {
        #region SetUp
        [PXString()]
        //CS201010
        [PXDefault("CCPAYABLE", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXSelector(typeof(CS.Numbering.numberingID))]
        [PXUIField(DisplayName = "Setup")]
        public virtual string Setup { get; set; }
        public abstract class setup : IBqlField { }
        #endregion

        #region GuarPayableID
        [PXDBIdentity()]
        public virtual int? GuarPayableID { get; set; }
        public abstract class guarPayableID : PX.Data.BQL.BqlInt.Field<guarPayableID> { }
        #endregion

        #region GuarPayableCD
        [PXDBString(15, InputMask = "", IsKey = true)]
        [PXUIField(DisplayName = "Guar Payable CD")]
        [PXSelector(typeof(Search<guarPayableCD>), typeof(guarPayableCD))]
        [CS.AutoNumber(typeof(Search<CS.Numbering.numberingID,
            Where<CS.Numbering.numberingID, Equal<Current<setup>>>>), typeof(AccessInfo.businessDate))]
        public virtual string GuarPayableCD { get; set; }
        public abstract class guarPayableCD : PX.Data.BQL.BqlString.Field<guarPayableCD> { }
        #endregion

        #region BranchID
        [PXDBInt()]
        [PXDefault(typeof(AccessInfo.branchID))]
        [PXUIField(DisplayName = "Branch ID")]
        public virtual int? BranchID { get; set; }
        public abstract class branchID : PX.Data.BQL.BqlInt.Field<branchID> { }
        #endregion

        #region DocDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Doc Date")]
        [PXDefault(typeof(AccessInfo.businessDate))]
        public virtual DateTime? DocDate { get; set; }
        public abstract class docDate : PX.Data.BQL.BqlDateTime.Field<docDate> { }
        #endregion

        #region Hold
        [PXDBBool()]
        [PXDefault(true, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Hold")]
        public virtual bool? Hold { get; set; }
        public abstract class hold : PX.Data.BQL.BqlBool.Field<hold> { }
        #endregion

        #region Status
        [PXDBString(1, IsFixed = true, InputMask = "")]
        [PXUIField(DisplayName = "Status", IsReadOnly = true)]
        [CCPayableStatus]
        [PXDefault(CCPayableStatus.OnHold)]
        public virtual string Status { get; set; }
        public abstract class status : PX.Data.BQL.BqlString.Field<status> { }
        #endregion

        #region IssueDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Issue Date")]
        [PXDefault(typeof(AccessInfo.businessDate), PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual DateTime? IssueDate { get; set; }
        public abstract class issueDate : PX.Data.BQL.BqlDateTime.Field<issueDate> { }
        #endregion

        #region BankAccountID
        [PXDBInt()]
        [PXUIField(DisplayName = "Bank Account ID")]
        [NMBankAccount(
                    SubstituteKey = typeof(NMBankAccount.bankAccountCD),
                    DescriptionField = typeof(NMBankAccount.bankName)
        )]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual int? BankAccountID { get; set; }
        public abstract class bankAccountID : PX.Data.BQL.BqlInt.Field<bankAccountID> { }
        #endregion

        #region BankCode
        [PXString(7, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Ori Bank Code", IsReadOnly = true)]
        [NMBankCode(DescriptionField = typeof(CS.SegmentValue.descr))]
        [PXUnboundDefault(typeof(Search<NMBankAccount.bankCode,
            Where<NMBankAccount.bankAccountID, Equal<Current<bankAccountID>>>>))]
        public virtual string BankCode { get; set; }
        public abstract class bankCode : PX.Data.BQL.BqlString.Field<bankCode> { }
        #endregion

        #region TargetType
        [PXDBString()]
        [PXUIField(DisplayName = "Target Type")]
        [CCTargetType()]
        [PXDefault(CCTargetType.Customer)]
        public virtual string TargetType { get; set; }
        public abstract class targetType : PX.Data.BQL.BqlString.Field<targetType> { }
        #endregion

        #region CustomerID
        [PXUIField(DisplayName = "Customer ID")]
        [CustomerActive(DescriptionField = typeof(Customer.acctName))]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIRequired(typeof(Where<targetType, Equal<CCTargetType.customer>>))]
        [PXUIVisible(typeof(Where<targetType, Equal<CCTargetType.customer>>))]
        [PXUIEnabled(typeof(Where<projectID, Equal<Zero>, And<projectID, IsNotNull>>))]
        public virtual int? CustomerID { get; set; }
        public abstract class customerID : PX.Data.BQL.BqlInt.Field<customerID> { }
        #endregion

        #region CustomerLocationID
        [CS.LocationID(typeof(Where<Location.bAccountID, Equal<Current2<customerID>>>), DescriptionField = typeof(Location.descr))]
        [PXFormula(typeof(Switch<Case<Where<Current2<customerID>, IsNull>, Null>, Selector<customerID, Customer.defLocationID>>))]
        [PXUIRequired(typeof(Where<targetType, Equal<CCTargetType.customer>>))]
        [PXUIVisible(typeof(Where<targetType, Equal<CCTargetType.customer>>))]
        [PXUIEnabled(typeof(Where<Current2<customerID>, IsNotNull>))]
        [PXDefault(typeof(Search<Customer.defLocationID, Where<Customer.bAccountID, Equal<Current<customerID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual int? CustomerLocationID { get; set; }
        public abstract class customerLocationID : PX.Data.BQL.BqlInt.Field<customerLocationID> { }
        #endregion

        #region VendorID
        [PXUIField(DisplayName = "Vendor ID")]
        [PXUIRequired(typeof(Where<targetType, Equal<CCTargetType.vendor>>))]
        [PXUIVisible(typeof(Where<targetType, Equal<CCTargetType.vendor>>))]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [POVendor(DescriptionField = typeof(Vendor.acctName))]
        public virtual int? VendorID { get; set; }
        public abstract class vendorID : PX.Data.BQL.BqlInt.Field<vendorID> { }
        #endregion

        #region VendorLocationID
        [PXUIRequired(typeof(Where<targetType, Equal<CCTargetType.vendor>>))]
        [PXUIVisible(typeof(Where<targetType, Equal<CCTargetType.vendor>>))]
        [CS.LocationID(typeof(Where<Location.bAccountID, Equal<Current2<vendorID>>>), DescriptionField = typeof(Location.descr))]
        [PXFormula(typeof(Switch<Case<Where<Current2<vendorID>, IsNull>, Null>, Selector<vendorID, Vendor.defLocationID>>))]
        [PXUIEnabled(typeof(Where<Current2<vendorID>, IsNotNull>))]
        [PXDefault(typeof(Search<Vendor.defLocationID, Where<Vendor.bAccountID, Equal<Current<vendorID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual int? VendorLocationID { get; set; }
        public abstract class vendorLocationID : PX.Data.BQL.BqlInt.Field<vendorLocationID> { }
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

        #region GuarType
        [PXDBString(1, IsFixed = true, InputMask = "")]
        [PXUIField(DisplayName = "Guar Type", Required = true)]
        [PXDefault(GuarTypeList.Other, PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [GuarTypeList]
        public virtual string GuarType { get; set; }
        public abstract class guarType : PX.Data.BQL.BqlString.Field<guarType> { }
        #endregion

        #region GuarClass
        [PXDBString(2, IsFixed = true, InputMask = "")]
        [PXUIField(DisplayName = "Guar Class", Required = true)]
        [PXDefault(GuarClassList.CommercialPaper, PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [GuarClassList]

        public virtual string GuarClass { get; set; }
        public abstract class guarClass : PX.Data.BQL.BqlString.Field<guarClass> { }
        #endregion

        #region GuarNbr
        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Guar Nbr")]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIEnabled(typeof(Where<guarClass, NotEqual<GuarClassList.commercialPaper>,
            Or<guarClass, NotEqual<GuarClassList.cashiersCheck>>>))]
        public virtual string GuarNbr { get; set; }
        public abstract class guarNbr : PX.Data.BQL.BqlString.Field<guarNbr> { }
        #endregion

        #region GuarTitle
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Guar Title", Required = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual string GuarTitle { get; set; }
        public abstract class guarTitle : PX.Data.BQL.BqlString.Field<guarTitle> { }
        #endregion

        #region GuarAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Guar Amt", Required = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXFormula(typeof(Sub<guarAmt,ccPostageAmt>))]
        public virtual Decimal? GuarAmt { get; set; }
        public abstract class guarAmt : PX.Data.BQL.BqlDecimal.Field<guarAmt> { }
        #endregion

        #region DueDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Due Date")]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual DateTime? DueDate { get; set; }
        public abstract class dueDate : PX.Data.BQL.BqlDateTime.Field<dueDate> { }
        #endregion

        #region DepID
        [PXDBString()]
        [PXUIField(DisplayName = "Dep ID", Required = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXSelector(typeof(EPDepartment.departmentID), DescriptionField = typeof(EPDepartment.description))]
        public virtual string DepID { get; set; }
        public abstract class depID : PX.Data.BQL.BqlString.Field<depID> { }
        #endregion

        #region ContractorID
        [PXDBInt()]
        [PXUIField(DisplayName = "Contractor ID", Required = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXEPEmployeeSelector]
        public virtual int? ContractorID { get; set; }
        public abstract class contractorID : PX.Data.BQL.BqlInt.Field<contractorID> { }
        #endregion

        #region CashierID
        [PXDBInt()]
        [PXUIField(DisplayName = "Cashier ID", Required = true)]
        [PXDefault(typeof(Search<EPEmployee.bAccountID,
            Where<EPEmployee.userID, Equal<Current<AccessInfo.userID>>>>),
            PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXEPEmployeeSelector]
        public virtual int? CashierID { get; set; }
        public abstract class cashierID : PX.Data.BQL.BqlInt.Field<cashierID> { }
        #endregion

        #region GuarReleaseDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Guar Release Date", IsReadOnly = true)]
        public virtual DateTime? GuarReleaseDate { get; set; }
        public abstract class guarReleaseDate : PX.Data.BQL.BqlDateTime.Field<guarReleaseDate> { }
        #endregion

        #region GuarReleaseNbr
        [PXDBString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Guar Release Nbr", IsReadOnly = true)]
        public virtual string GuarReleaseNbr { get; set; }
        public abstract class guarReleaseNbr : PX.Data.BQL.BqlString.Field<guarReleaseNbr> { }
        #endregion

        /*#region GuarReturnDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Guar Return Date", IsReadOnly = true)]
        public virtual DateTime? GuarReturnDate { get; set; }
        public abstract class guarReturnDate : PX.Data.BQL.BqlDateTime.Field<guarReturnDate> { }
        #endregion*/

        #region GuarVoidDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Guar Void Date", IsReadOnly = true)]
        public virtual DateTime? GuarVoidDate { get; set; }
        public abstract class guarVoidDate : PX.Data.BQL.BqlDateTime.Field<guarVoidDate> { }
        #endregion

        #region GuarVoidNbr
        [PXDBString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Guar Void Nbr", IsReadOnly = true)]
        public virtual string GuarVoidNbr { get; set; }
        public abstract class guarVoidNbr : PX.Data.BQL.BqlString.Field<guarVoidNbr> { }
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

        #region Description
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Description", Required = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual string Description { get; set; }
        public abstract class description : PX.Data.BQL.BqlString.Field<description> { }
        #endregion

        #region AuthDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Auth Date")]
        public virtual DateTime? AuthDate { get; set; }
        public abstract class authDate : PX.Data.BQL.BqlDateTime.Field<authDate> { }
        #endregion

        //2020/11/26 ADD 
        #region POOrderType
        [PXDBString(2, IsFixed = true, IsUnicode = true)]
        [PXUIField(DisplayName = "POOrderType")]
        public virtual string POOrderType { get; set; }
        public abstract class pOOrderType : PX.Data.BQL.BqlString.Field<pOOrderType> { }
        #endregion

        #region PONbr
        [PXDBString(15, IsFixed = true, IsUnicode = true)]
        [PXUIField(DisplayName = "PONbr")]
        [PXUIEnabled(typeof(Where<vendorID, IsNotNull>))]
        [PXSelector(typeof(Search<POOrder.orderNbr,
            Where<POOrder.vendorID, Equal<Current<vendorID>>,
                And<POOrder.projectID, Equal<Current<projectID>>>>>),
            typeof(POOrder.orderType),
            typeof(POOrder.orderNbr),
            typeof(POOrder.orderDesc),
            typeof(POOrder.orderDate),
            DescriptionField = typeof(POOrder.orderDesc))]
        public virtual string PONbr { get; set; }
        public abstract class pONbr : PX.Data.BQL.BqlString.Field<pONbr> { }
        #endregion

        //2021/01/05 Add
        #region BankAccount

        #endregion

        #region BookCD
        [PXDBString(50, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Book CD")]
        [PXSelector(
            typeof(Search<NMCheckBook.bookCD,
                Where<NMCheckBook.bankAccountID, Equal<Current<bankAccountID>>,
                    And<NMCheckBook.bookUsage, Equal<NMStringList.NMBookUsage.cashierscheck>>>>),
            typeof(NMCheckBook.bookCD),
            typeof(NMCheckBook.startCheckNbr),
            typeof(NMCheckBook.currentCheckNbr),
            typeof(NMCheckBook.endCheckNbr),
            typeof(NMCheckBook.startDate),
            typeof(NMCheckBook.description),
            DescriptionField = typeof(NMCheckBook.description)
            )]
        [PXDefault(typeof(Search<NMCheckBook.bookCD,
                Where<NMCheckBook.bankAccountID, Equal<Current<bankAccountID>>,
                    And<NMCheckBook.bookUsage, Equal<NMStringList.NMBookUsage.cashierscheck>>>>)
            ,PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIRequired(typeof(Where<guarClass, Equal<GuarClassList.cashiersCheck>>))]
        [PXUIVisible(typeof(Where<guarClass, Equal<GuarClassList.cashiersCheck>>))]
        public virtual string BookCD { get; set; }
        public abstract class bookCD : PX.Data.BQL.BqlString.Field<bookCD> { }
        #endregion

        #region unboun
        #region CustomerVendorCD
        [PXString(IsUnicode = true)]
        [PXUIField(DisplayName = "Customer / Vendor", Enabled = false)]
        [PXUnboundDefault(typeof(Search<BAccount2.acctCD,
            Where<BAccount2.bAccountID,Equal<IsNull<Current2<customerID>,Current2<vendorID>>>>>))]
        [PXDBScalar(typeof(Search<BAccount2.acctCD,
            Where<BAccount2.bAccountID, Equal<IsNull<customerID, vendorID>>>>))]
        public virtual string CustomerVendorCD { get; set; }
        public abstract class customerVendorCD : PX.Data.BQL.BqlString.Field<customerVendorCD> { }
        #endregion

        #region CustomerVendorName
        [PXString(IsUnicode = true)]
        [PXUIField(DisplayName = "Customer / Vendor Name", Enabled = false)]
        [PXUnboundDefault(typeof(Search<BAccount2.acctName,
            Where<BAccount2.bAccountID, Equal<IsNull<Current2<customerID>, Current2<vendorID>>>>>))]
        [PXDBScalar(typeof(Search<BAccount2.acctName,
            Where<BAccount2.bAccountID, Equal<IsNull<customerID, vendorID>>>>))]
        public virtual string CustomerVendorName { get; set; }
        public abstract class customerVendorName : PX.Data.BQL.BqlString.Field<customerVendorName> { }
        #endregion

        #endregion

        //2021/10/05 Add Mantis: 0012252
        //2021/10/08 visible Mantis: 0012252
        //會計:喔沒有那是工程師請款的時候忘記殺掉 我們作帳的時候會忽略
        #region CCPostageAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "CCPostageAmt",Visible = false)]
        [PXDefault(TypeCode.Decimal, "0.0",PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual decimal? CCPostageAmt { get; set; }
        public abstract class ccPostageAmt : PX.Data.BQL.BqlDecimal.Field<ccPostageAmt> { }
        #endregion

    }
}