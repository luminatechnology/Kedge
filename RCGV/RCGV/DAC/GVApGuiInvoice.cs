using System;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.GL;
using Branch = PX.Objects.GL.Branch;
using PX.Data.EP;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Data.Licensing;
using RCGV.GV.Attribute.Selector;
using RCGV.GV.Attribute.DropDown;
using PX.SM;
using static RCGV.GV.Util.GVList;
using GVList = RCGV.GV.Util.GVList;

namespace RCGV.GV.DAC
{
    /**
     * ===2021-02-19:11955 === Alton
     * 6.GVApGuiInvoice新增一個欄位VendorName, nvarchar(255)-->直接加在RCGV那包
     * 
     * ===2021-03-11:11974 === Alton
     * 1.GVApGuiInvoice 新增一個欄位 Hold, bits, 預設0
     * 
     * ===2021-03-17:11974 === Alton
     * 1.表頭必填欄位
     *   RegistrationCD
     *   InvoiceDate
     *   GuiType
     *   DeclareYear
     *   DeclareMonth
     *   VendorName
     *   VendorUniformNumber
     *   InvoiceType
     *   TaxCode
     * 2.欄位預設值
     *   GVApGuiInvoice.RegistrationCD -->從使用者登入的BranchID, 從GvRegistrationbranch取的RegistrationCD
     *   GVApGuiInvoice.InvoiceDate 預設BusinessDate
     *   GVApGuiInvoice.GuiType 可否不要讓使用者挑到 23/24/29 ?
     * ===2021-03-18 :口頭討論===Alton
     * 1.GuiInvoiceNbr 移除isKey & Lov
     * 2.GuiInvoiceID IsKey = true
     * 
     * ===2021-03-22:11995 ====Alton
     * GVApGuiInvoice.Hold 預設為 True, Status預設為擱置
     * 
     * ====2021-06-17:12097====Alton
     * 1. 請在GVApGuiInvoice多開一個欄位VendorAddress, nvarchar(255), 允許Null=False
     * 
     * ===2022/03/10===Jeff
     * Since users need to upload excel but DAC PK is identity field, it will impact the column must exist in source excel file to avoid existing record only being inserted instead of updated.
     * **/
    [PXPrimaryGraph(typeof(GVApInvoiceMaint))]
    [SerializableAttribute()]
    [PXCacheName("GVApGuiInvoice")]
    public class GVApGuiInvoice : PX.Data.IBqlTable
    {
        #region GuiInvoiceID
        [PXDBIdentity(IsKey = true)]
        [PXUIField(Visible = false)]
        public virtual int? GuiInvoiceID { get; set; }
        public abstract class guiInvoiceID : PX.Data.BQL.BqlInt.Field<guiInvoiceID> { }
        #endregion

        #region DocType
        [PXDBString(3, IsFixed = true)]
        [APDocType.List()]
        [PXUIField(DisplayName = "Type", Visibility = PXUIVisibility.SelectorVisible, Enabled = true, TabOrder = 0)]
        public virtual string DocType { get; set; }
        public abstract class docType : PX.Data.BQL.BqlString.Field<docType> { }
        #endregion

        #region RefNbr
        [PXDBString(15, IsUnicode = true)]
        [PXUIField(DisplayName = "Reference Nbr.")]
        public virtual string RefNbr { get; set; }
        public abstract class refNbr : PX.Data.BQL.BqlString.Field<refNbr> { }
        #endregion

        #region LineNbr
        [PXDBInt()]
        [PXUIField(DisplayName = "Line Nbr.", Visibility = PXUIVisibility.Visible, Visible = false)]
        public virtual int? LineNbr { get; set; }
        public abstract class lineNbr : PX.Data.BQL.BqlInt.Field<lineNbr> { }
        #endregion

        #region GuiInvoiceNbr
        [PXDBString(14, IsUnicode = true)]
        [PXDefault]
        [PXUIField(DisplayName = "GuiInvoiceNbr")]
        //[PXSelector(
        //    typeof(Search<GVApGuiInvoice.guiInvoiceNbr>),
        //    typeof(GVApGuiInvoice.guiInvoiceNbr),
        //    typeof(GVApGuiInvoice.registrationCD),
        //    typeof(GVApGuiInvoice.guiType))]
        public virtual string GuiInvoiceNbr { get; set; }
        public abstract class guiInvoiceNbr : PX.Data.BQL.BqlString.Field<guiInvoiceNbr> { }
        #endregion

        #region RegistrationCD
        [PXDBString(9, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Registration CD", Required = true)]
        [RegistrationCDAttribute()]
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

        #region DeclareYear
        [PXDBInt()]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXUIField(DisplayName = "Declare Year", Required = true)]
        public virtual int? DeclareYear { get; set; }
        public abstract class declareYear : PX.Data.BQL.BqlInt.Field<declareYear> { }
        #endregion

        #region DeclareMonth
        [PXDBInt()]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXUIField(DisplayName = "Declare Month", Required = true)]
        [GVGuiDeclareMonth]
        public virtual int? DeclareMonth { get; set; }
        public abstract class declareMonth : PX.Data.BQL.BqlInt.Field<declareMonth> { }
        #endregion

        #region Vendor
        [VendorActive(
            DescriptionField = typeof(PX.Objects.AP.Vendor.acctName),
            CacheGlobal = true,
            Required = false,
            Filterable = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXForeignReference(typeof(Field<vendor>.IsRelatedTo<BAccount.bAccountID>))]
        [PXUIField(DisplayName = "Vendor")]
        public virtual int? Vendor { get; set; }
        public abstract class vendor : PX.Data.BQL.BqlInt.Field<vendor> { }
        #endregion

        #region VendorName
        [PXDBString(255, IsUnicode = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXUIField(DisplayName = "Vendor Name", Required = true)]
        public virtual string VendorName { get; set; }
        public abstract class vendorName : PX.Data.BQL.BqlString.Field<vendorName> { }
        #endregion

        #region VendorUniformNumber
        [PXDBString(8, IsUnicode = true)]
        [PXUIField(DisplayName = "VendorUniformNumber", IsReadOnly = true, Required = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIRequired(typeof(Where<Current<voucherCategory>, NotIn3<GVGuiVoucherCategory.customproxytax, GVGuiVoucherCategory.othercertificate>>))]
        public virtual string VendorUniformNumber { get; set; }
        public abstract class vendorUniformNumber : PX.Data.BQL.BqlString.Field<vendorUniformNumber> { }
        #endregion

        #region VendorAddress
        [PXDBString(255, IsUnicode = true)]
        [PXDefault(
            typeof(Search2<Address.addressLine1,
                InnerJoin<Vendor,On<Vendor.defAddressID,Equal<Address.addressID>>>,
                Where<Vendor.bAccountID,Equal<Current2<vendor>>>>),
            PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Vendor Address")]
        public virtual string VendorAddress { get; set; }
        public abstract class vendorAddress : PX.Data.BQL.BqlString.Field<vendorAddress> { }
        #endregion

        #region VoucherCategory
        [PXDBString(1, IsUnicode = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXUIField(DisplayName = "Voucher Category", Required = true)]
        [GVGuiVoucherCategory]
        public virtual string VoucherCategory { get; set; }
        public abstract class voucherCategory : PX.Data.BQL.BqlString.Field<voucherCategory> { }
        #endregion

        #region DeductionCode
        [PXDBString(1, IsUnicode = true)]
        [PXUIField(DisplayName = "Deduction Code")]
        [GVGuiDeductionCode]
        public virtual string DeductionCode { get; set; }
        public abstract class deductionCode : PX.Data.BQL.BqlString.Field<deductionCode> { }
        #endregion

        #region Remark
        [PXDBString(240, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Remark")]
        public virtual string Remark { get; set; }
        public abstract class remark : PX.Data.BQL.BqlString.Field<remark> { }
        #endregion

        #region GuiType
        [PXDBString(2)]
        [PXUIField(DisplayName = "Gui Type", Required = true)]
        [PXDefault(GVList.GVGuiType.AP.GuiType_21, PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [GVList.GVGuiType.APInvoice]
        public virtual string GuiType { get; set; }
        public abstract class guiType : PX.Data.BQL.BqlString.Field<guiType> { }

        #region GuiType Const
        private class guiTypeCD_23 : PX.Data.BQL.BqlString.Constant<guiTypeCD_23> { public guiTypeCD_23() : base("23") {; } }
        private class guiTypeCD_24 : PX.Data.BQL.BqlString.Constant<guiTypeCD_24> { public guiTypeCD_24() : base("24") {; } }
        private class guiTypeCD_29 : PX.Data.BQL.BqlString.Constant<guiTypeCD_29> { public guiTypeCD_29() : base("29") {; } }
        #endregion
        #endregion

        #region InvoiceType
        [PXDBString(1)]
        [PXUIField(DisplayName = "InvoiceType", Required = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [GVGuiInvoiceType]
        public virtual string InvoiceType { get; set; }
        public abstract class invoiceType : PX.Data.BQL.BqlString.Field<invoiceType> { }
        #endregion

        #region GroupRemark
        [PXDBString(1, IsUnicode = true)]
        [PXDefault(GVGuiGroupRemark.UNSUMMARY)]
        [PXUIField(DisplayName = "GroupRemark")]
        [GVGuiGroupRemark]
        public virtual string GroupRemark { get; set; }
        public abstract class groupRemark : PX.Data.BQL.BqlString.Field<groupRemark> { }
        #endregion

        #region GroupCnt
        [PXDBInt()]
        [PXDefault(0)]
        [PXUIField(DisplayName = "Group Count")]
        public virtual int? GroupCnt { get; set; }
        public abstract class groupCnt : PX.Data.BQL.BqlInt.Field<groupCnt> { }
        #endregion

        #region SalesAmt
        [PXDBDecimal(2)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "SalesAmt")]
        //[PXFormula(null, typeof(SumCalc<APInvoice.curyLineTotal>))]
        public virtual Decimal? SalesAmt { get; set; }
        public abstract class salesAmt : PX.Data.BQL.BqlDecimal.Field<salesAmt> { }
        #endregion

        #region TaxCode
        [PXDBString(1, IsUnicode = true)]
        [PXDefault(GVTaxCode.TAXABLE, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Tax Code", Required = true)]
        [GVTaxCode]
        public virtual string TaxCode { get; set; }
        public abstract class taxCode : PX.Data.BQL.BqlString.Field<taxCode> { }
        #endregion

        #region TaxRate
        [PXDBDecimal(4)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Tax Rate")]
        public virtual Decimal? TaxRate { get; set; }
        public abstract class taxRate : PX.Data.BQL.BqlDecimal.Field<taxRate> { }
        #endregion

        #region TaxAmt
        [PXDBDecimal(2)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "TaxAmt")]
        //[PXFormula(null, typeof(SumCalc<APInvoice.curyTaxTotal>))]
        public virtual Decimal? TaxAmt { get; set; }
        public abstract class taxAmt : PX.Data.BQL.BqlDecimal.Field<taxAmt> { }
        #endregion

        #region TotalAmt
        [PXDBDecimal(2)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Total Amt")]
        [PXFormula(typeof(Add<salesAmt, taxAmt>))]
        public virtual Decimal? TotalAmt { get; set; }
        public abstract class totalAmt : PX.Data.BQL.BqlDecimal.Field<totalAmt> { }
        #endregion

        #region ConfirmDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Confirm Date")]
        public virtual DateTime? ConfirmDate { get; set; }
        public abstract class confirmDate : PX.Data.BQL.BqlDateTime.Field<confirmDate> { }
        #endregion

        #region ConfirmPerson
        [PXDBGuid()]
        [PXSelector(
            typeof(Search<Users.pKID>),
            DescriptionField = typeof(Users.username),
            SubstituteKey = typeof(Users.username)
            )]
        [PXUIField(DisplayName = "Confirm Person")]
        public virtual Guid? ConfirmPerson { get; set; }
        public abstract class confirmPerson : PX.Data.BQL.BqlGuid.Field<confirmPerson> { }
        #endregion

        #region PrintCnt
        [PXDBInt()]
        [PXDefault(0)]
        [PXUIField(DisplayName = "PrintCnt", Enabled = false)]
        public virtual int? PrintCnt { get; set; }
        public abstract class printCnt : PX.Data.BQL.BqlInt.Field<printCnt> { }
        #endregion

        #region Status
        [PXDBString(1, IsUnicode = true)]
        [PXDefault(GVStatusAPInv_APInv.HOLD)]
        [PXUIField(DisplayName = "Status", Enabled = false)]
        [GVStatusAPInv_APInv]
        public virtual string Status { get; set; }
        public abstract class status : PX.Data.BQL.BqlString.Field<status> { }
        #endregion

        #region VoidDate
        [PXDBDate()]
        [PXDefault()]
        [PXUIField(DisplayName = "Void Date", Required = false, IsReadOnly = true)]
        [PXUIRequired(typeof(Where<GVApGuiInvoice.status, Equal<GVStatusAPInv_APInv.voidinvoice>>))]
        public virtual DateTime? VoidDate { get; set; }
        public abstract class voidDate : PX.Data.BQL.BqlDateTime.Field<voidDate> { }
        #endregion

        #region VoidReason
        [PXDBString(240, IsUnicode = true)]
        [PXDefault()]
        [PXUIField(DisplayName = "Void Reason", Required = false, IsReadOnly = true)]
        [PXUIRequired(typeof(Where<GVApGuiInvoice.status, Equal<GVStatusAPInv_APInv.voidinvoice>>))]
        public virtual string VoidReason { get; set; }
        public abstract class voidReason : PX.Data.BQL.BqlString.Field<voidReason> { }
        #endregion

        #region LatestLineNo
        [PXDBInt()]
        [PXDefault(0)]
        [PXUIField(DisplayName = "Latest Line No", IsReadOnly = true)]
        public virtual int? LatestLineNo { get; set; }
        public abstract class latestLineNo : PX.Data.BQL.BqlInt.Field<latestLineNo> { }
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

        #region EPRefNbr
        [PXDBString()]
        [PXUIField(DisplayName = "EPRefNbr")]
        public virtual string EPRefNbr { get; set; }
        public abstract class ePRefNbr : PX.Data.BQL.BqlString.Field<ePRefNbr> { }
        #endregion

        #region DeclareBatchNbr
        [PXDBString(20,IsUnicode = true)]
        [PXUIField(DisplayName = "Declare Batch Nbr")]
        public virtual string DeclareBatchNbr { get; set; }
        public abstract class declareBatchNbr : PX.Data.BQL.BqlString.Field<declareBatchNbr> { }
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

        //#region Hold
        //[PXDBBool()]
        //[PXDefault(true, PersistingCheck = PXPersistingCheck.Nothing)]
        //[PXUIField(DisplayName = "Hold")]
        //public virtual bool? Hold { get; set; }
        //public abstract class hold : PX.Data.BQL.BqlBool.Field<hold> { }
        //#endregion

        #region unbound
        #region APReleased
        [PXBool()]
        [PXUnboundDefault(
            typeof(Search<APInvoice.released,
                Where<APInvoice.refNbr, Equal<Current<refNbr>>,
                    And<APInvoice.docType, NotIn3<APDocType.voidCheck, APDocType.voidQuickCheck, APDocType.voidRefund>>>>)
            )]
        [PXUIField(DisplayName = "APReleased")]
        public virtual bool? APReleased { get; set; }
        public abstract class apReleased : PX.Data.BQL.BqlBool.Field<apReleased> { }
        #endregion
        #endregion
    }
}
