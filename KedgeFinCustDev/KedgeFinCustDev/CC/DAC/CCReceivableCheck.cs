using System;
using PX.Data;
using static CC.Util.CCList;
using CS = PX.Objects.CS;
using PX.Objects.AR;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.PO;
using PX.Objects.AP;
using PX.Objects.EP;
using PX.Objects.PM;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.GL;
using NM.Util;

/**
 * ====2021-07-08:12140====Alton
 * 因應第一條的修改, CC應收保證票的到期日也請修改為非必填
 * 
 * ====2021-10-05:12253====Alton
 * 請在CCReceivableCheck新增一欄位 CCPostageAmt,decimal(18,6),非必填,預設=0
 * - 請放在「合約編號」欄位下方
 * - 當此欄金額異動時，記得將 CCReceivableCheck.GuarAmt的金額減少
 * 
 * ====2021-10-08:12253====Alton
 * 請協助將CCReceivable的CCPostageAmt在畫面上隱藏
 * 
 * ===2022-03-30=== Jeff
 * Added the key function.
 * **/
namespace CC.DAC
{
    [Serializable]
    public class CCReceivableCheck : IBqlTable
    {
        #region Keys
        public class UK : PrimaryKeyOf<CCReceivableCheck>.By<guarReceviableCD>
        {
            public static CCReceivableCheck Find(PXGraph graph, string guarReceviableCD) => FindBy(graph, guarReceviableCD);
        }
        #endregion

        #region SetUp
        [PXString()]
        //CS201010
        [PXDefault("CCRECEIVE", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXSelector(typeof(CS.Numbering.numberingID))]
        [PXUIField(DisplayName = "Setup")]
        public virtual string Setup { get; set; }
        public abstract class setup : IBqlField { }
        #endregion

        #region GuarReceviableID
        [PXDBIdentity()]
        [PXUIField(DisplayName = "Guar Receviable ID")]
        public virtual int? GuarReceviableID { get; set; }
        public abstract class guarReceviableID : PX.Data.BQL.BqlInt.Field<guarReceviableID> { }
        #endregion

        #region GuarReceviableCD
        [PXDBString(15, InputMask = "", IsKey = true)]
        [PXUIField(DisplayName = "Guar Receviable CD")]
        [PXSelector(typeof(Search<guarReceviableCD>), typeof(guarReceviableCD))]
        [CS.AutoNumber(typeof(Search<CS.Numbering.numberingID,
            Where<CS.Numbering.numberingID, Equal<Current<setup>>>>), typeof(AccessInfo.businessDate))]
        public virtual string GuarReceviableCD { get; set; }
        public abstract class guarReceviableCD : PX.Data.BQL.BqlString.Field<guarReceviableCD> { }
        #endregion

        #region BranchID
        [PXDBInt()]
        [PXUIField(DisplayName = "Branch ID")]
        [PXDefault(typeof(Search<Branch.branchID,
            Where<Branch.active, Equal<True>,
                And<Branch.branchID, Equal<Current<AccessInfo.branchID>>>>>))]
        [PXDimensionSelector("BIZACCT",
            typeof(Search<Branch.branchID,
            Where<Branch.active, Equal<True>,
                And<MatchWithBranch<Branch.branchID>>>>),
            typeof(Branch.branchCD),
            DescriptionField = typeof(Branch.acctName))]
        public virtual int? BranchID { get; set; }
        public abstract class branchID : PX.Data.BQL.BqlInt.Field<branchID> { }
        #endregion

        #region DocDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Doc Date", Required = true)]
        [PXDefault(typeof(AccessInfo.businessDate), PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual DateTime? DocDate { get; set; }
        public abstract class docDate : PX.Data.BQL.BqlDateTime.Field<docDate> { }
        #endregion

        #region Status
        [PXDBString(1, IsFixed = true, InputMask = "")]
        [PXUIField(DisplayName = "Status", IsReadOnly = true)]
        [CCReceivableStatus]
        [PXDefault(CCReceivableStatus.Balanced)]
        public virtual string Status { get; set; }
        public abstract class status : PX.Data.BQL.BqlString.Field<status> { }
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

        #region TargetType
        [PXDBString()]
        [PXUIField(DisplayName = "Target Type")]
        [CCTargetType()]
        [PXDefault(CCTargetType.Customer)]
        public virtual string TargetType { get; set; }
        public abstract class targetType : PX.Data.BQL.BqlString.Field<targetType> { }
        #endregion

        #region CustomerID
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [CustomerActive(DescriptionField = typeof(Customer.acctName))]
        [PXUIVisible(typeof(Where<targetType, Equal<CCTargetType.customer>>))]

        public virtual int? CustomerID { get; set; }
        public abstract class customerID : PX.Data.BQL.BqlInt.Field<customerID> { }
        #endregion

        #region CustomerLocationID
        [PXDefault(typeof(Search<Customer.defLocationID, Where<Customer.bAccountID, Equal<Current<customerID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
        [LocationID(typeof(Where<Location.bAccountID, Equal<Current2<customerID>>>), DescriptionField = typeof(Location.descr))]
        [PXUIEnabled(typeof(Where<Current2<customerID>, IsNotNull>))]
        [PXFormula(typeof(Switch<Case<Where<Current2<customerID>, IsNull>, Null>, Selector<customerID, Customer.defLocationID>>))]
        [PXUIVisible(typeof(Where<targetType, Equal<CCTargetType.customer>>))]

        public virtual int? CustomerLocationID { get; set; }
        public abstract class customerLocationID : PX.Data.BQL.BqlInt.Field<customerLocationID> { }
        #endregion

        #region VendorID
        [PXUIField(DisplayName = "Vendor ID")]
        [PXUIVisible(typeof(Where<targetType, Equal<CCTargetType.vendor>>))]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [POVendor(DescriptionField = typeof(Vendor.acctName))]

        public virtual int? VendorID { get; set; }
        public abstract class vendorID : PX.Data.BQL.BqlInt.Field<vendorID> { }
        #endregion

        #region VendorLocationID
        [PXDefault(typeof(Search<Vendor.defLocationID, Where<Vendor.bAccountID, Equal<Current<vendorID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
        [LocationID(typeof(Where<Location.bAccountID, Equal<Current2<vendorID>>>), DescriptionField = typeof(Location.descr))]
        [PXFormula(typeof(Switch<Case<Where<Current2<vendorID>, IsNull>, Null>, Selector<vendorID, Vendor.defLocationID>>))]
        [PXUIVisible(typeof(Where<targetType, Equal<CCTargetType.vendor>>))]
        [PXUIEnabled(typeof(Where<Current2<vendorID>, IsNotNull>))]

        public virtual int? VendorLocationID { get; set; }
        public abstract class vendorLocationID : PX.Data.BQL.BqlInt.Field<vendorLocationID> { }
        #endregion

        #region ProjectID
        [ProjectBase()]
        [PXRestrictor(typeof(Where<PMProject.isCancelled, Equal<False>>), PX.Objects.PM.Messages.CancelledContract, typeof(PMProject.contractCD))]
        [PXRestrictor(typeof(Where<PMProject.visibleInPO, Equal<True>, Or<PMProject.nonProject, Equal<True>>>), PX.Objects.PM.Messages.ProjectInvisibleInModule, typeof(PMProject.contractCD))]
        [PXRestrictor(typeof(Where<PMProject.defaultBranchID, Equal<Current<AccessInfo.branchID>>, Or<PMProject.defaultBranchID, IsNull>>), "Branch Not Found.", typeof(PMProject.contractCD))]
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
        [PXDefault(GuarClassList.PromissoryNote, PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [GuarClassList]
        public virtual string GuarClass { get; set; }
        public abstract class guarClass : PX.Data.BQL.BqlString.Field<guarClass> { }
        #endregion

        #region GuarNbr
        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "GuarNbr")]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIRequired(typeof(Where<Current<guarClass>, NotEqual<GuarClassList.commercialPaper>>))]
        public virtual string GuarNbr { get; set; }
        public abstract class guarNbr : PX.Data.BQL.BqlString.Field<guarNbr> { }
        #endregion

        #region GuarAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "GuarAmt", Required = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
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

        #region ContractorID
        [PXDBInt()]
        [PXUIField(DisplayName = "Contractor ID")]
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

        #region GuarReceiveDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Guar Receive Date", IsReadOnly = true)]
        public virtual DateTime? GuarReceiveDate { get; set; }
        public abstract class guarReceiveDate : PX.Data.BQL.BqlDateTime.Field<guarReceiveDate> { }
        #endregion

        #region GuarReturnDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Guar Return Date", IsReadOnly = true)]
        public virtual DateTime? GuarReturnDate { get; set; }
        public abstract class guarReturnDate : PX.Data.BQL.BqlDateTime.Field<guarReturnDate> { }
        #endregion

        #region BatchNbr
        [PXDBString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Batch Nbr", IsReadOnly = true)]
        public virtual string BatchNbr { get; set; }
        public abstract class batchNbr : PX.Data.BQL.BqlString.Field<batchNbr> { }
        #endregion

        #region ReturnBatchNbr
        [PXDBString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Return Batch Nbr", IsReadOnly = true)]
        public virtual string ReturnBatchNbr { get; set; }
        public abstract class returnBatchNbr : PX.Data.BQL.BqlString.Field<returnBatchNbr> { }
        #endregion

        #region Description
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Description", Required = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual string Description { get; set; }
        public abstract class description : PX.Data.BQL.BqlString.Field<description> { }
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

        //2020/10/09 Add
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

        #region IsPostpone
        [PXDBBool()]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Is Postpone")]
        public virtual bool? IsPostpone { get; set; }
        public abstract class isPostpone : PX.Data.BQL.BqlBool.Field<isPostpone> { }
        #endregion

        #region PostDueDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Post Due Date")]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIRequired(typeof(Where<IsNull<isPostpone, False>, Equal<True>>))]
        public virtual DateTime? PostDueDate { get; set; }
        public abstract class postDueDate : PX.Data.BQL.BqlDateTime.Field<postDueDate> { }
        #endregion

        #region CheckIssuer
        [PXDBString(255, IsUnicode = true)]
        [PXUIField(DisplayName = "Check Issuer")]
        public virtual string CheckIssuer { get; set; }
        public abstract class checkIssuer : PX.Data.BQL.BqlString.Field<checkIssuer> { }
        #endregion


        #region OriBankCode - 原支票銀行代碼
        [PXDBString()]
        [PXUIField(DisplayName = "Ori Bank Code")]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [NMBankCode]
        public virtual string OriBankCode { get; set; }
        public abstract class oriBankCode : PX.Data.BQL.BqlString.Field<oriBankCode> { }
        #endregion

        #region OriBankAccount - 原支票銀行帳戶
        [PXDBString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Ori Bank Account")]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual string OriBankAccount { get; set; }
        public abstract class oriBankAccount : PX.Data.BQL.BqlString.Field<oriBankAccount> { }
        #endregion

        #region CCPostageAmt
        [PXDBDecimal]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "CC Postage Amt", Visible = false)]
        public virtual Decimal? CCPostageAmt { get; set; }
        public abstract class ccPostageAmt : PX.Data.BQL.BqlDecimal.Field<ccPostageAmt> { }
        #endregion

        #region unbound
        #region CustomerVendorCD
        [PXString(IsUnicode = true)]
        [PXUIField(DisplayName = "Customer / Vendor", Enabled = false)]
        [PXUnboundDefault(typeof(Search<BAccount2.acctCD,
            Where<BAccount2.bAccountID, Equal<IsNull<Current2<customerID>, Current2<vendorID>>>>>))]
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
    }
}