using System;
using PX.Data;
using PX.Objects.AR;
using RCGV.GV.Util;
using PX.Objects.CS;
using Branch = PX.Objects.GL.Branch;
using RCGV.GV.Attribute.Selector;
using PX.SM;
using PX.Objects.CT;

namespace RCGV.GV.DAC
{

    [Serializable]
    [PXCacheName("GVArGuiCmInvoice")]
    public class GVArGuiCmInvoice : IBqlTable
    {
        #region SetUp
        [PXString()]
        //CS201010
        [PXDefault("GVARCMNBR", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXSelector(typeof(Numbering.numberingID))]
        public virtual string Setup { get; set; }
        public abstract class setup : IBqlField { }
        #endregion

        #region GuiCmInvoiceID
        [PXDBIdentity]
        public virtual int? GuiCmInvoiceID { get; set; }
        public abstract class guiCmInvoiceID : PX.Data.BQL.BqlInt.Field<guiCmInvoiceID> { }
        #endregion

        #region GuiCmInvoiceNbr
        [PXSelector(
          typeof(Search<guiCmInvoiceNbr>),
          typeof(guiCmInvoiceNbr),
          typeof(customerID),
          typeof(registrationCD)
        )]
        [AutoNumber(typeof(Search<Numbering.numberingID, Where<Numbering.numberingID, Equal<Current<setup>>>>), typeof(AccessInfo.businessDate))]
        [PXDBString(40, IsUnicode = true, IsKey = true, InputMask = "")]
        [PXUIField(DisplayName = "Gui Cm Invoice Nbr")]
        public virtual string GuiCmInvoiceNbr { get; set; }
        public abstract class guiCmInvoiceNbr : PX.Data.BQL.BqlString.Field<guiCmInvoiceNbr> { }
        #endregion

        #region RegistrationCD
        [PXDBString(9, IsUnicode = true)]
        [PXUIField(DisplayName = "RegistrationCD", Required = true)]
        [RegistrationCD()]
        [PXDefault(typeof(
            Search2<GVRegistration.registrationCD,
                InnerJoin<GVRegistrationBranch, On<GVRegistration.registrationID, Equal<GVRegistrationBranch.registrationID>>,
                InnerJoin<Branch, On<Branch.bAccountID, Equal<GVRegistrationBranch.bAccountID>>>>,
                Where<Branch.branchID, Equal<Current<AccessInfo.branchID>>>>),
            PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual string RegistrationCD { get; set; }
        public abstract class registrationCD : PX.Data.BQL.BqlString.Field<registrationCD> { }
        #endregion

        #region InvoiceDate
        [PXDBDate()]
        [PXDefault(typeof(AccessInfo.businessDate), PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXUIField(DisplayName = "InvoiceDate", Required = true)]
        public virtual DateTime? InvoiceDate { get; set; }
        public abstract class invoiceDate : PX.Data.BQL.BqlDateTime.Field<invoiceDate> { }
        #endregion

        #region DeclareYear
        [PXDBInt()]
        [PXDefault(2018, PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXUIField(DisplayName = "DeclareYear", Required = true)]
        public virtual int? DeclareYear { get; set; }
        public abstract class declareYear : PX.Data.BQL.BqlInt.Field<declareYear> { }
        #endregion

        #region DeclareMonth
        [PXDBInt()]
        [PXDefault(1, PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXUIField(DisplayName = "DeclareMonth", Required = true)]
        public virtual int? DeclareMonth { get; set; }
        public abstract class declareMonth : PX.Data.BQL.BqlInt.Field<declareMonth> { }
        #endregion

        #region CustomerID
        [CustomerActive(Visibility = PXUIVisibility.SelectorVisible, DescriptionField = typeof(Customer.acctName), Filterable = true, TabOrder = 2)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXUIField(DisplayName = "Customer ID", Required = true)]
        public virtual int? CustomerID { get; set; }
        public abstract class customerID : PX.Data.BQL.BqlInt.Field<customerID> { }
        #endregion

        #region CustName
        [PXDBString(240, IsUnicode = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXUIField(DisplayName = "Cust Name", Required = true)]
        public virtual string CustName { get; set; }
        public abstract class custName : PX.Data.BQL.BqlString.Field<custName> { }
        #endregion

        #region CustUniformNumber
        [PXDBString(8, IsUnicode = true)]
        [PXUIField(DisplayName = "Cust Uniform Number")]
        public virtual string CustUniformNumber { get; set; }
        public abstract class custUniformNumber : PX.Data.BQL.BqlString.Field<custUniformNumber> { }
        #endregion

        #region GuiType
        [PXDBString(2, IsFixed = true, IsUnicode = true)]
        [PXUIField(DisplayName = "Gui Type", Required = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [GVList.GVGuiType.ARCmInvoice]
        public virtual string GuiType { get; set; }
        public abstract class guiType : PX.Data.BQL.BqlString.Field<guiType> { }
        #endregion

        #region TaxCode
        [PXDBString(1, IsFixed = true, IsUnicode = true)]
        [PXUIField(DisplayName = "TaxCode", Required = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [GVList.GVTaxCode]
        public virtual string TaxCode { get; set; }
        public abstract class taxCode : PX.Data.BQL.BqlString.Field<taxCode> { }
        #endregion

        #region SalesAmt
        [PXDBDecimal(2)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "SalesAmt")]
        public virtual Decimal? SalesAmt { get; set; }
        public abstract class salesAmt : PX.Data.BQL.BqlDecimal.Field<salesAmt> { }
        #endregion

        #region TaxAmt
        [PXDBDecimal(2)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "TaxAmt")]
        public virtual Decimal? TaxAmt { get; set; }
        public abstract class taxAmt : PX.Data.BQL.BqlDecimal.Field<taxAmt> { }
        #endregion

        #region Remark
        [PXDBString(240, IsUnicode = true)]
        [PXUIField(DisplayName = "Remark")]
        public virtual string Remark { get; set; }
        public abstract class remark : PX.Data.BQL.BqlString.Field<remark> { }
        #endregion

        #region ConfirmDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Confirm Date")]
        public virtual DateTime? ConfirmDate { get; set; }
        public abstract class confirmDate : PX.Data.BQL.BqlDateTime.Field<confirmDate> { }
        #endregion

        #region ConfirmBy
        [PXDBGuid()]
        [PXUIField(DisplayName = "Confirm By")]
        [PXSelector(
            typeof(Search<Users.pKID>),
            DescriptionField = typeof(Users.username),
            SubstituteKey = typeof(Users.username)
            )]
        public virtual Guid? ConfirmBy { get; set; }
        public abstract class confirmBy : PX.Data.BQL.BqlGuid.Field<confirmBy> { }
        #endregion

        #region VoidDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Void Date")]
        public virtual DateTime? VoidDate { get; set; }
        public abstract class voidDate : PX.Data.BQL.BqlDateTime.Field<voidDate> { }
        #endregion

        #region VoidReason
        [PXDBString(240, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Void Reason")]
        public virtual string VoidReason { get; set; }
        public abstract class voidReason : PX.Data.BQL.BqlString.Field<voidReason> { }
        #endregion

        #region VoidBy
        [PXDBGuid()]
        [PXUIField(DisplayName = "Void By")]
        [PXSelector(
            typeof(Search<Users.pKID>),
            DescriptionField = typeof(Users.username),
            SubstituteKey = typeof(Users.username)
            )]
        public virtual Guid? VoidBy { get; set; }
        public abstract class voidBy : PX.Data.BQL.BqlGuid.Field<voidBy> { }
        #endregion

        #region PrintCnt
        [PXDBInt()]
        [PXUIField(DisplayName = "Print Cnt")]
        [PXDefault(0, PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual int? PrintCnt { get; set; }
        public abstract class printCnt : PX.Data.BQL.BqlInt.Field<printCnt> { }
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
        public virtual Guid? NoteID { get; set; }
        public abstract class noteID : PX.Data.BQL.BqlGuid.Field<noteID> { }
        #endregion

        #region Tstamp
        [PXDBTimestamp()]
        [PXUIField(DisplayName = "Tstamp")]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
        #endregion

        #region DeclarePeriod
        [PXDBString(1, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Declare Period")]
        public virtual string DeclarePeriod { get; set; }
        public abstract class declarePeriod : PX.Data.BQL.BqlString.Field<declarePeriod> { }
        #endregion

        #region Status
        [PXDBString(1, IsUnicode = true)]
        [PXUIField(DisplayName = "Status", Enabled = false)]
        [GVList.GVStatus]
        [PXDefault(GVList.GVStatus.HOLD)]
        public virtual string Status { get; set; }
        public abstract class status : PX.Data.BQL.BqlString.Field<status> { }
        #endregion

        #region Hold
        [PXDBBool()]
        [PXUIField(DisplayName = "Hold")]
        [PXDefault(true, PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual bool? Hold { get; set; }
        public abstract class hold : PX.Data.BQL.BqlBool.Field<hold> { }
        #endregion

        #region TotalAmt
        [PXDBDecimal(2)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "TotalAmt")]
        [PXFormula(typeof(Add<salesAmt, taxAmt>))]
        public virtual Decimal? TotalAmt { get; set; }
        public abstract class totalAmt : PX.Data.BQL.BqlDecimal.Field<totalAmt> { }
        #endregion

        #region DeclareBatchNbr
        [PXDBString(20, IsUnicode = true)]
        [PXUIField(DisplayName = "Declare Batch Nbr")]
        public virtual string DeclareBatchNbr { get; set; }
        public abstract class declareBatchNbr : PX.Data.BQL.BqlString.Field<declareBatchNbr> { }
        #endregion

        #region BatchNbr
        [PXDBString(15, IsUnicode = true)]
        [PXUIField(DisplayName = "Batch Nbr")]
        [PXSelector(
            typeof(Search2<ARInvoice.batchNbr,
                InnerJoin<Contract, On<ARInvoice.projectID, Equal<Contract.contractID>>>,
                Where<ARInvoice.docType, In3<ARDocType.invoice, ARDocType.creditMemo>,
                    And<ARInvoice.status, In3<ARDocStatus.balanced, ARDocStatus.open, ARDocStatus.closed>,
                    And<ARInvoice.batchNbr, IsNotNull,
                    And<ARInvoice.customerID,Equal<Current<customerID>>>>>>>),
            typeof(ARInvoice.batchNbr),
            typeof(ARInvoice.refNbr),
            typeof(Contract.contractCD),
            typeof(ARInvoice.customerID),
            typeof(ARInvoice.customerID_Customer_acctName)
            )]
        public virtual string BatchNbr { get; set; }
        public abstract class batchNbr : PX.Data.BQL.BqlString.Field<batchNbr> { }
        #endregion

        #region DeclaredByID
        [PXDBGuid()]
        [PXUIField(DisplayName = "Declared On")]
        public virtual Guid? DeclaredByID { get; set; }
        public abstract class declaredByID : PX.Data.BQL.BqlGuid.Field<declaredByID> { }
        #endregion

        #region DeclaredDateTime
        [PXDBDateAndTime(UseTimeZone = true)]
        [PXUIField(DisplayName = "Declared Date Time")]
        public virtual DateTime? DeclaredDateTime { get; set; }
        public abstract class declaredDateTime : PX.Data.BQL.BqlDateTime.Field<declaredDateTime> { }
        #endregion
    }
}
