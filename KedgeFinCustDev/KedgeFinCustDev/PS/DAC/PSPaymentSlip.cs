using System;
using PX.Data;
using PX.Objects.CM;
using PX.Objects.CS;
using PX.Objects.EP;
using PX.Objects.GL;
using PX.Objects.CR;
using PX.Objects.AP;
using PX.Objects.TX;
using PX.Objects.Common;
using TX = PX.Objects.TX;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.AR;
using PX.Objects.PO;
using PX.Objects.CT;
using PX.Objects.PM;
using static PS.Util.PSStringList;

namespace PS.DAC
{
    [Serializable]
    public class PSPaymentSlip : IBqlTable
    {
        #region SetUp
        public abstract class setup : IBqlField
        { }
        [PXString()]
        //CS201010
        [PXDefault("PSPAYSLIP", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXSelector(typeof(Numbering.numberingID))]
        [PXUIField(DisplayName = "Setup")]
        public virtual string Setup { get; set; }
        #endregion

        #region RefNbr
        [PXDBString(15, InputMask = "",IsKey = true)]
        [PXUIField(DisplayName = "Ref Nbr")]
        [PXSelector(typeof(Search<refNbr>), typeof(refNbr), typeof(docDesc))]
        [AutoNumber(typeof(Search<Numbering.numberingID, Where<Numbering.numberingID, Equal<Current<setup>>>>), typeof(AccessInfo.businessDate))]
        public virtual string RefNbr { get; set; }
        public abstract class refNbr : PX.Data.BQL.BqlString.Field<refNbr> { }
        #endregion

        #region BranchID
        [Branch(typeof(Search2<
                         Branch.branchID,
                         InnerJoin<EPEmployee,
                             On<Branch.bAccountID, Equal<EPEmployee.parentBAccountID>>>,
                         Where<EPEmployee.bAccountID, Equal<Current<employeeID>>>>))]
        public virtual int? BranchID { get; set; }
        public abstract class branchID : PX.Data.BQL.BqlInt.Field<branchID> { }
        #endregion

        #region EmployeeID
        [PXDBInt]
        [PXDefault(typeof(Search<EPEmployee.bAccountID, Where<EPEmployee.userID, Equal<Current<AccessInfo.userID>>>>),PersistingCheck =PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Payer", Visibility = PXUIVisibility.SelectorVisible)]
        [PXUIRequired(typeof(Where<targetType, Equal<PSTargetType.employee>>))]
        [PXUIVisible(typeof(Where<targetType, Equal<PSTargetType.employee>>))]
        [PXEPEmployeeSelector]
        public virtual int? EmployeeID { get; set; }
        public abstract class employeeID : PX.Data.BQL.BqlInt.Field<employeeID> { }
        #endregion

        #region DepartmentID
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXDefault(typeof(Search<EPEmployee.departmentID,
            Where<EPEmployee.userID, Equal<Current<AccessInfo.userID>>>>))]
        [PXSelector(typeof(EPDepartment.departmentID), DescriptionField = typeof(EPDepartment.description))]
        [PXUIField(DisplayName = "Department ID", Enabled = false, Visibility = PXUIVisibility.Visible)]
        public virtual string DepartmentID { get; set; }
        public abstract class departmentID : PX.Data.BQL.BqlString.Field<departmentID> { }
        #endregion

        #region LocationID
        [PXUIField(DisplayName = "Location ID")]
        [PXDefault(typeof(Search<EPEmployee.defLocationID, Where<EPEmployee.bAccountID, Equal<Current<employeeID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
        [LocationID(typeof(Where<Location.bAccountID, Equal<Current<employeeID>>>), Visibility = PXUIVisibility.SelectorVisible)]
        public virtual int? LocationID { get; set; }
        public abstract class locationID : PX.Data.BQL.BqlInt.Field<locationID> { }
        #endregion

        #region DocDate
        [PXDBDate]
        [PXDefault(typeof(AccessInfo.businessDate))]
        [PXUIField(DisplayName = "Date", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual DateTime? DocDate { get; set; }
        public abstract class docDate : PX.Data.BQL.BqlDateTime.Field<docDate> { }
        #endregion

        #region ApproveDate
        [PXDBDate]
        [PXUIField(DisplayName = "Approval Date", Enabled = false)]
        public virtual DateTime? ApproveDate { get; set; }
        public abstract class approveDate : PX.Data.BQL.BqlDateTime.Field<approveDate> { }
        #endregion

        #region DocDesc
        [PXDBString(Constants.TranDescLength, IsUnicode = true)]
        [PXDefault]
        [PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual string DocDesc { get; set; }
        public abstract class docDesc : PX.Data.BQL.BqlString.Field<docDesc> { }
        #endregion

        #region Hold
        [PXDBBool()]
        [PXDefault(true)]
        [PXUIField(DisplayName = "Hold")]
        public virtual bool? Hold { get; set; }
        public abstract class hold : PX.Data.BQL.BqlBool.Field<hold> { }
        #endregion

        #region Status
        [PXDBString(1, IsFixed = true, InputMask = "")]
        [PXUIField(DisplayName = "Status", Enabled = false)]
        [PXDefault(PSStatus.Hold)]
        [PSStatus]
        public virtual string Status { get; set; }
        public abstract class status : PX.Data.BQL.BqlString.Field<status> { }
        #endregion

        //#region CuryDocBal
        //[PXDBCurrency(typeof(curyInfoID), typeof(docBal))]
        //[PXDefault(TypeCode.Decimal, "0.0")]
        //[PXUIField(DisplayName = "Claim Total", Visibility = PXUIVisibility.SelectorVisible,Enabled =false)]
        //public virtual Decimal? CuryDocBal { get; set; }
        //public abstract class curyDocBal : PX.Data.BQL.BqlDecimal.Field<curyDocBal> { }
        //#endregion

        #region DocBal
        [PXDBDecimal(4)]
        [PXUIField(DisplayName = "Claim Total", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        public virtual Decimal? DocBal { get; set; }
        public abstract class docBal : PX.Data.BQL.BqlDecimal.Field<docBal> { }
        #endregion

        //#region CuryTaxTotal
        //[PXDBCurrency(typeof(curyInfoID), typeof(taxTotal))]
        //[PXUIField(DisplayName = "Tax Total", Visibility = PXUIVisibility.Visible, Enabled = false)]
        //[PXDefault(TypeCode.Decimal, "0.0")]
        //public virtual Decimal? CuryTaxTotal { get; set; }
        //public abstract class curyTaxTotal : PX.Data.BQL.BqlDecimal.Field<curyTaxTotal> { }
        //#endregion

        #region FinPeriodID
        [APOpenPeriod(null, typeof(branchID), masterFinPeriodIDType: typeof(tranPeriodID), ValidatePeriod = PeriodValidation.Nothing)]
        [PXFormula(typeof(Switch<Case<Where<hold, Equal<True>>, Null>, finPeriodID>))]
        [PXUIField(DisplayName = "Post to Period")]
        public virtual string FinPeriodID { get; set; }
        public abstract class finPeriodID : PX.Data.BQL.BqlString.Field<finPeriodID> { }
        #endregion

        #region TranPeriodID
        [PeriodID]
        public virtual string TranPeriodID { get; set; }
        public abstract class tranPeriodID : PX.Data.BQL.BqlString.Field<tranPeriodID> { }
        #endregion

        //#region CuryVatTaxableTotal
        //[PXDBCurrency(typeof(curyInfoID), typeof(vatTaxableTotal))]
        //[PXUIField(DisplayName = "VAT Taxable Total", Visibility = PXUIVisibility.Visible, Enabled = false)]
        //[PXDefault(TypeCode.Decimal, "0.0")]
        //public virtual Decimal? CuryVatTaxableTotal { get; set; }
        //public abstract class curyVatTaxableTotal : PX.Data.BQL.BqlDecimal.Field<curyVatTaxableTotal> { }
        //#endregion

        //#region CuryVatExemptTotal
        //[PXDBCurrency(typeof(curyInfoID), typeof(vatExemptTotal))]
        //[PXUIField(DisplayName = "VAT Exempt Total", Visibility = PXUIVisibility.Visible, Enabled = false)]
        //[PXDefault(TypeCode.Decimal, "0.0")]
        //public virtual Decimal? CuryVatExemptTotal { get; set; }
        //public abstract class curyVatExemptTotal : PX.Data.BQL.BqlDecimal.Field<curyVatExemptTotal> { }
        //#endregion

        #region CustomerID
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIRequired(typeof(Where<targetType, Equal<PSTargetType.customer>>))]
        [PXUIVisible(typeof(Where<targetType, Equal<PSTargetType.customer>>))]
        [CustomerActive(DisplayName = "Payer", DescriptionField = typeof(Customer.acctName))]
        //[PXUIEnabled(typeof(Where<contractID, Equal<Zero>, Or<contractID, IsNull>>))]
        public virtual int? CustomerID { get; set; }
        public abstract class customerID : PX.Data.BQL.BqlInt.Field<customerID> { }
        #endregion

        #region CustomerLocationID
        [PXDefault(typeof(Search<Customer.defLocationID, Where<Customer.bAccountID, Equal<Current<customerID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
        [LocationID(typeof(Where<Location.bAccountID, Equal<Current2<customerID>>>), DescriptionField = typeof(Location.descr))]
        [PXUIEnabled(typeof(Where<Current2<customerID>, IsNotNull>))]
        [PXFormula(typeof(Switch<Case<Where<Current2<customerID>, IsNull>, Null>, Selector<customerID, Customer.defLocationID>>))]
        [PXUIRequired(typeof(Where<targetType, Equal<PSTargetType.customer>>))]
        [PXUIVisible(typeof(Where<targetType, Equal<PSTargetType.customer>>))]
        public virtual int? CustomerLocationID { get; set; }
        public abstract class customerLocationID : PX.Data.BQL.BqlInt.Field<customerLocationID> { }
        #endregion


        #region 20200925 mark by alton 繳款單不維護供應商，將來user要輸入供應商資料就得去客戶主檔新增供應商資料

        #region TargetType
        [PXDBString()]
        [PXUIField(DisplayName = "Target Type")]
        [PSTargetType()]
        [PXDefault(PSTargetType.Customer, PersistingCheck = PXPersistingCheck.Nothing)]
        //[PXUIEnabled(typeof(
        //    Where<docType, In3<PSDocType.apRtnGuarCheck, PSDocType.arGuarCheck>>))]
        //[PXUIVisible(typeof(Where<docType, In3<PSDocType.apRtnGuarCheck, PSDocType.arGuarCheck>>))]
        public virtual string TargetType { get; set; }
        public abstract class targetType : PX.Data.BQL.BqlString.Field<targetType> { }
        #endregion


        #region VendorID
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIRequired(typeof(Where<targetType, Equal<PSTargetType.vendor>>))]
        [PXUIVisible(typeof(Where<targetType, Equal<PSTargetType.vendor>>))]
        [POVendor(DisplayName = "Payer",DescriptionField = typeof(Vendor.acctName))]
        public virtual int? VendorID { get; set; }
        public abstract class vendorID : PX.Data.BQL.BqlInt.Field<vendorID> { }
        #endregion

        #region VendorLocationID
        [PXDefault(typeof(Search<Vendor.defLocationID, Where<Vendor.bAccountID, Equal<Current<vendorID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
        [LocationID(typeof(Where<Location.bAccountID, Equal<Current2<vendorID>>>), DescriptionField = typeof(Location.descr))]
        [PXUIEnabled(typeof(Where<Current2<vendorID>, IsNotNull>))]
        [PXFormula(typeof(Switch<Case<Where<Current2<vendorID>, IsNull>, Null>, Selector<vendorID, Vendor.defLocationID>>))]
        [PXUIRequired(typeof(Where<targetType, Equal<PSTargetType.vendor>>))]
        [PXUIVisible(typeof(Where<targetType, Equal<PSTargetType.vendor>>))]
        public virtual int? VendorLocationID { get; set; }
        public abstract class vendorLocationID : PX.Data.BQL.BqlInt.Field<vendorLocationID> { }
        #endregion

        #endregion
        //#region FinancialYear
        //[PXDBString(4, IsUnicode = true, InputMask = "")]
        //[PXUIField(DisplayName = "Financial Year")]
        //public virtual string FinancialYear { get; set; }
        //public abstract class financialYear : PX.Data.BQL.BqlString.Field<financialYear> { }
        //#endregion

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
        [PXDBCreatedDateTime]
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
        [PXDBLastModifiedDateTime]
        public virtual DateTime? LastModifiedDateTime { get; set; }
        public abstract class lastModifiedDateTime : PX.Data.BQL.BqlDateTime.Field<lastModifiedDateTime> { }
        #endregion

        #region Tstamp
        [PXDBTimestamp()]
        [PXUIField(DisplayName = "Tstamp")]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
        #endregion

        #region NoteID
        [PXNote()]
        [PXUIField(DisplayName = "NoteID")]
        public virtual Guid? NoteID { get; set; }
        public abstract class noteID : PX.Data.BQL.BqlGuid.Field<noteID> { }
        #endregion

        #region ContractID
        [ProjectBase()]
        [PXRestrictor(typeof(Where<PMProject.isCancelled, Equal<False>>), PX.Objects.PM.Messages.CancelledContract, typeof(PMProject.contractCD))]
        [PXRestrictor(typeof(Where<PMProject.visibleInPO, Equal<True>, Or<PMProject.nonProject, Equal<True>>>), PX.Objects.PM.Messages.ProjectInvisibleInModule, typeof(PMProject.contractCD))]
        [PXRestrictor(typeof(Where<PMProject.defaultBranchID, Equal<Current<AccessInfo.branchID>>, Or<PMProject.defaultBranchID, IsNull>>), "Branch Not Found.", typeof(PMProject.contractCD))]
        [PXForeignReference(typeof(Field<contractID>.IsRelatedTo<PMProject.contractID>))]
        [PXUIField(DisplayName = "Project/Contract", Required = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [ProjectDefault()]
        public virtual int? ContractID { get; set; }
        public abstract class contractID : PX.Data.BQL.BqlInt.Field<contractID> { }
        #endregion

        #region VoidedBy
        [PXDBGuid()]
        public virtual Guid? VoidedBy { get; set; }
        public abstract class voidedBy : PX.Data.BQL.BqlGuid.Field<voidedBy> { }
        #endregion

        #region VoidedDate
        [PXDBDateAndTime()]
        public virtual DateTime? VoidedDate { get; set; }
        public abstract class voidedDate : PX.Data.BQL.BqlDateTime.Field<voidedDate> { }
        #endregion

        #region DocType
        [PXDBString(3, IsFixed = true, InputMask = "")]
        [PXDefault(PSDocType.Payment, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Doc Type")]
        [PSDocType]
        public virtual string DocType { get; set; }
        public abstract class docType : PX.Data.BQL.BqlString.Field<docType> { }
        #endregion
    }
}