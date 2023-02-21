using System;
using PX.Data;
using PX.Objects.AR;
using RCGV.GV.Util;
using RCGV.GV.Attribute.Selector;
using PX.Objects.CS;
using Branch = PX.Objects.GL.Branch;
using PX.SM;

namespace RCGV.GV.DAC
{
    /**
     * ====2021-03-26: 12004 ====
     * 1.欄位調整(GVArGuiInvoice)
     *   CustomerAcctName --> CustName
     *   Currency --> 移除
     *   LinkGuiInvoiceNbr --> 移除
     *   VoidPerson --> VoidBy
     *   VoidDesc --> VoidReason
     * **/
    [Serializable]
    [PXCacheName("GVArGuiInvoice")]
    public class GVArGuiInvoice : IBqlTable
    {
        #region SetUp
        [PXString()]
        //CS201010
        [PXDefault("GVARINVCD", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXSelector(typeof(Numbering.numberingID))]
        public virtual string Setup { get; set; }
        public abstract class setup : IBqlField { }
        #endregion

        #region GuiInvoiceCD
        [PXDBString(15, IsUnicode = true, InputMask = "", IsKey = true)]
        [PXUIField(DisplayName = "Gui Invoice CD")]
        [AutoNumber(typeof(Search<Numbering.numberingID, Where<Numbering.numberingID, Equal<Current<setup>>>>), typeof(AccessInfo.businessDate))]
        [PXSelector(
                typeof(Search<guiInvoiceCD>),
                typeof(guiInvoiceCD),
                typeof(registrationCD),
                typeof(guiType))]
        public virtual string GuiInvoiceCD { get; set; }
        public abstract class guiInvoiceCD : PX.Data.BQL.BqlString.Field<guiInvoiceCD> { }
        #endregion

        #region GuiInvoiceID
        [PXDBIdentity]
        public virtual int? GuiInvoiceID { get; set; }
        public abstract class guiInvoiceID : PX.Data.BQL.BqlInt.Field<guiInvoiceID> { }
        #endregion

        #region GuiInvoiceNbr
        [PXDBString(10, IsUnicode = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Gui Invoice Nbr")]
        public virtual string GuiInvoiceNbr { get; set; }
        public abstract class guiInvoiceNbr : PX.Data.BQL.BqlString.Field<guiInvoiceNbr> { }
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
        [PXUIField(DisplayName = "Invoice Date", Required = true)]
        public virtual DateTime? InvoiceDate { get; set; }
        public abstract class invoiceDate : PX.Data.BQL.BqlDateTime.Field<invoiceDate> { }
        #endregion

        #region CustomerID
        [CustomerActive(Visibility = PXUIVisibility.SelectorVisible, DescriptionField = typeof(Customer.acctName), Filterable = true)]
        [PXUIField(DisplayName = "Customer ID", Required = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual int? CustomerID { get; set; }
        public abstract class customerID : PX.Data.BQL.BqlInt.Field<customerID> { }
        #endregion

        #region CustName
        [PXDBString(60, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Customer Name")]
        public virtual string CustName { get; set; }
        public abstract class custName : PX.Data.BQL.BqlString.Field<custName> { }
        #endregion

        #region CustUniformNumber
        [PXDBString(8, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Cust Uniform Number", Required = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual string CustUniformNumber { get; set; }
        public abstract class custUniformNumber : PX.Data.BQL.BqlString.Field<custUniformNumber> { }
        #endregion

        #region CustAddress
        [PXDBString(240, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Cust Address")]
        public virtual string CustAddress { get; set; }
        public abstract class custAddress : PX.Data.BQL.BqlString.Field<custAddress> { }
        #endregion

        #region InvoiceType
        [PXDBString(1, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "InvoiceType", Required = true)]
        [PXDefault(GVList.GVGuiInvoiceType.INVOICE, PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [GVList.GVGuiInvoiceType]
        public virtual string InvoiceType { get; set; }
        public abstract class invoiceType : PX.Data.BQL.BqlString.Field<invoiceType> { }
        #endregion

        #region Remark
        [PXDBString(240, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Remark")]
        public virtual string Remark { get; set; }
        public abstract class remark : PX.Data.BQL.BqlString.Field<remark> { }
        #endregion

        #region GuiType
        [PXDBString(2, IsUnicode = true, InputMask = "")]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXUIField(DisplayName = "Gui Type", Required = true)]
        [GVList.GVGuiType.ARInvoice]
        public virtual string GuiType { get; set; }
        public abstract class guiType : PX.Data.BQL.BqlString.Field<guiType> { }
        #endregion

        #region DeclareYear
        [PXDBInt()]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXUIField(DisplayName = "Declare Year", Required = true)]
        public virtual int? DeclareYear { get; set; }
        public abstract class declareYear : PX.Data.BQL.BqlInt.Field<declareYear> { }
        #endregion

        #region DeclareMonth
        [PXDBInt()]
        [PXDefault(1, PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXUIField(DisplayName = "Declare Month", Required = true)]
        public virtual int? DeclareMonth { get; set; }
        public abstract class declareMonth : PX.Data.BQL.BqlInt.Field<declareMonth> { }
        #endregion

        #region GuiBookID
        [PXDBInt()]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIRequired(typeof(Where<isHistorical,NotEqual<True>>))]
        [PXUIField(DisplayName = "Gui Book ID", Required = true)]
        [PXSelector(
               typeof(Search<GVGuiBook_GVPeriod.guiBookID,
                   Where<GVGuiBook_GVPeriod.registrationCD, Equal<Current<registrationCD>>,
                    //And<GVPeriod.outActive, Equal<True>,
                    And2<Where<GVGuiBook_GVPeriod.currentNum, NotEqual<GVGuiBook_GVPeriod.endNum>, Or<GVGuiBook_GVPeriod.currentNum, IsNull>>,
                    And<GVGuiBook_GVPeriod.startMonth, LessEqual<DatePart<DatePart.month, Current<invoiceDate>>>,
                    And<GVGuiBook_GVPeriod.endMonth, GreaterEqual<DatePart<DatePart.month, Current<invoiceDate>>>,
                    And<GVGuiBook_GVPeriod.declareYear, Equal<DatePart<DatePart.year, Current<invoiceDate>>>
                    >>>>>>),
                typeof(GVGuiBook_GVPeriod.guiBookCD),
                typeof(GVGuiBook_GVPeriod.declareYear),
                typeof(GVGuiBook_GVPeriod.startMonth),
                typeof(GVGuiBook_GVPeriod.endMonth),
                typeof(GVGuiBook_GVPeriod.currentNum),
                typeof(GVGuiBook_GVPeriod.endNum),
               SubstituteKey = typeof(GVGuiBook_GVPeriod.guiBookCD)
            )]
        public virtual int? GuiBookID { get; set; }
        public abstract class guiBookID : PX.Data.BQL.BqlInt.Field<guiBookID> { }
        #endregion

        #region EgvType
        [PXDBString(2, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Egv Type")]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [GVList.GVEgvType]
        public virtual string EgvType { get; set; }
        public abstract class egvType : PX.Data.BQL.BqlString.Field<egvType> { }
        #endregion

        #region VoidDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Void Date")]
        public virtual DateTime? VoidDate { get; set; }
        public abstract class voidDate : PX.Data.BQL.BqlDateTime.Field<voidDate> { }
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

        #region VoidReason
        [PXDBString(240, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Void Reason")]
        public virtual string VoidReason { get; set; }
        public abstract class voidReason : PX.Data.BQL.BqlString.Field<voidReason> { }
        #endregion

        #region TaxCode
        [PXDBString(1, IsUnicode = true, InputMask = "")]
        [PXDefault(GVList.GVTaxCode.TAXABLE, PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [GVList.GVTaxCode]
        [PXUIField(DisplayName = "Tax Code", Required = true)]
        public virtual string TaxCode { get; set; }
        public abstract class taxCode : PX.Data.BQL.BqlString.Field<taxCode> { }
        #endregion

        #region SalesAmt
        [PXDBDecimal(2, MinValue = 0)]
        [PXDefault(TypeCode.Decimal, "0.00")]
        [PXUIField(DisplayName = "SalesAmt", Enabled = false)]
        public virtual Decimal? SalesAmt { get; set; }
        public abstract class salesAmt : PX.Data.BQL.BqlDecimal.Field<salesAmt> { }
        #endregion

        #region TaxAmt
        [PXDBDecimal(2)]
        [PXDefault(TypeCode.Decimal, "0.00")]
        [PXUIField(DisplayName = "TaxAmt", Enabled = false)]
        public virtual Decimal? TaxAmt { get; set; }
        public abstract class taxAmt : PX.Data.BQL.BqlDecimal.Field<taxAmt> { }
        #endregion

        #region TotalAmt
        [PXDBDecimal(2)]
        [PXDefault(TypeCode.Decimal, "0.00")]
        [PXUIField(DisplayName = "TotalAmt", Enabled = false)]
        [PXFormula(typeof(Add<salesAmt, taxAmt>))]
        public virtual Decimal? TotalAmt { get; set; }
        public abstract class totalAmt : PX.Data.BQL.BqlDecimal.Field<totalAmt> { }
        #endregion

        #region RandonNumber
        [PXDBString(4, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Randon Number")]
        public virtual string RandonNumber { get; set; }
        public abstract class randonNumber : PX.Data.BQL.BqlString.Field<randonNumber> { }
        #endregion

        #region QrcodeStr1
        [PXDBString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Qrcode Str1")]
        public virtual string QrcodeStr1 { get; set; }
        public abstract class qrcodeStr1 : PX.Data.BQL.BqlString.Field<qrcodeStr1> { }
        #endregion

        #region QrcodeStr2
        [PXDBString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Qrcode Str2")]
        public virtual string QrcodeStr2 { get; set; }
        public abstract class qrcodeStr2 : PX.Data.BQL.BqlString.Field<qrcodeStr2> { }
        #endregion

        #region DonateRemark
        [PXDBBool()]
        [PXUIField(DisplayName = "Donate Remark")]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual bool? DonateRemark { get; set; }
        public abstract class donateRemark : PX.Data.BQL.BqlBool.Field<donateRemark> { }
        #endregion

        #region Status
        [PXDBString(1, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Status", Enabled = false)]
        [PXDefault(GVList.GVStatus.HOLD, PersistingCheck = PXPersistingCheck.Nothing)]
        [GVList.GVStatus]
        public virtual string Status { get; set; }
        public abstract class status : PX.Data.BQL.BqlString.Field<status> { }
        #endregion

        #region TurnkeySendDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Turnkey Send Date")]
        public virtual DateTime? TurnkeySendDate { get; set; }
        public abstract class turnkeySendDate : PX.Data.BQL.BqlDateTime.Field<turnkeySendDate> { }
        #endregion

        #region PrintCount
        [PXDBInt()]
        [PXDefault(0, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "PrintCount", Enabled = false)]
        public virtual int? PrintCount { get; set; }
        public abstract class printCount : PX.Data.BQL.BqlInt.Field<printCount> { }
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

        #region GuiWordCD
        [PXDBString(2, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Gui Word CD")]
        public virtual string GuiWordCD { get; set; }
        public abstract class guiWordCD : PX.Data.BQL.BqlString.Field<guiWordCD> { }
        #endregion

        #region DeclareBatchNbr
        [PXDBString(20, IsUnicode = true)]
        [PXUIField(DisplayName = "Declare Batch Nbr")]
        public virtual string DeclareBatchNbr { get; set; }
        public abstract class declareBatchNbr : PX.Data.BQL.BqlString.Field<declareBatchNbr> { }
        #endregion

        #region IsHistorical
        [PXDBBool()]
        [PXUIField(DisplayName = "Is Historical")]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual bool? IsHistorical { get; set; }
        public abstract class isHistorical : PX.Data.BQL.BqlBool.Field<isHistorical> { }
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

        #region unbound
        //#region CurrentGuiDate
        //[PXDate]
        //[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        //[PXUIField(DisplayName = "Current Gui Date", Enabled = false)]
        //public virtual DateTime? CurrentGuiDate { get; set; }
        //public abstract class currentGuiDate : PX.Data.BQL.BqlString.Field<currentGuiDate> { }
        //#endregion

        //#region CurrentNum
        //[PXString(8)]
        //[PXUIField(DisplayName = "Current Number", Enabled = false)]
        //public virtual string CurrentNum { get; set; }
        //public abstract class currentNum : PX.Data.BQL.BqlString.Field<currentNum> { }
        //#endregion
        #endregion
    }
}



