using System;
using PX.Data;
using RCGV.GV.Util;
using PX.Objects.AP;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.CR;
using RCGV.GV.Attribute.Selector;
using PX.Objects.CS;
using PX.Objects.GL;
using Branch = PX.Objects.GL.Branch;
using PX.SM;

namespace RCGV.GV.DAC
{
    /***
     * ====2021-03-15 : 11980 ====Alton
     * 新增一個欄位 Hold, bits
     * 新增 VendorName, nvarchar(255)
     * 新增 VoidDate, datetime
     * 新增 VoidReason, nvarchar(255)
     * 移除下列欄位
     * InvVoidDate
     * InvVoidReason
     * CmVoidDate
     * CmVoidReason
     * CmVoidPerson
     * CmVoidConfirmDate
     * CmVoidConfirmPerson
     * 
     * ====2021-03-23:11997 ====Alton
     * vendorID 改為非必填
     * VendorUniformNumber & VendorName 改為必填
     * 
     * **/

    [Serializable]
    [PXCacheName("GVApGuiCmInvoice")]
    public class GVApGuiCmInvoice : IBqlTable
    {
        #region SetUp
        public abstract class setup : IBqlField
        { }
        [PXString()]
        //CS201010
        [PXDefault("GVAPCMNBR", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXSelector(typeof(Numbering.numberingID))]
        [PXUIField(DisplayName = "Setup")]
        public virtual string Setup { get; set; }
        #endregion

        #region GuiCmInvoiceID
        [PXDBIdentity()]
        public virtual int? GuiCmInvoiceID { get; set; }
        public abstract class guiCmInvoiceID : PX.Data.BQL.BqlInt.Field<guiCmInvoiceID> { }
        #endregion

        #region GuiCmInvoiceNbr
        [PXDBString(15, IsUnicode = true, InputMask = "", IsKey = true)]
        [PXUIField(DisplayName = "Gui Cm Invoice Nbr")]
        [AutoNumber(typeof(Search<Numbering.numberingID, Where<Numbering.numberingID, Equal<Current<setup>>>>), typeof(AccessInfo.businessDate))]
        [PXSelector(
                typeof(Search<guiCmInvoiceNbr>),
                typeof(guiCmInvoiceNbr),
                typeof(registrationCD),
                typeof(guiType))]
        public virtual string GuiCmInvoiceNbr { get; set; }
        public abstract class guiCmInvoiceNbr : PX.Data.BQL.BqlString.Field<guiCmInvoiceNbr> { }
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
        [PXUIField(DisplayName = "Invoice Date", Required = true)]
        [PXDefault(typeof(AccessInfo.businessDate), PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual DateTime? InvoiceDate { get; set; }
        public abstract class invoiceDate : PX.Data.BQL.BqlDateTime.Field<invoiceDate> { }
        #endregion

        #region DeclareYear
        [PXDBInt()]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXUIField(DisplayName = "Declare Year", Required = true, Visibility = PXUIVisibility.Visible)]
        public virtual int? DeclareYear { get; set; }
        public abstract class declareYear : PX.Data.BQL.BqlInt.Field<declareYear> { }
        #endregion

        #region DeclareMonth
        [PXDBInt()]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXUIField(DisplayName = "Declare Month", Required = true)]
        [GVList.GVGuiDeclareMonth]
        public virtual int? DeclareMonth { get; set; }
        public abstract class declareMonth : PX.Data.BQL.BqlInt.Field<declareMonth> { }
        #endregion

        #region VendorUniformNumber
        [PXDBString(8, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Vendor Uniform Number", Required = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual string VendorUniformNumber { get; set; }
        public abstract class vendorUniformNumber : PX.Data.BQL.BqlString.Field<vendorUniformNumber> { }
        #endregion

        #region VendorID
        [VendorActive(
                Visibility = PXUIVisibility.SelectorVisible,
                DescriptionField = typeof(Vendor.acctName),
                CacheGlobal = true,
                Filterable = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXForeignReference(typeof(Field<vendorID>.IsRelatedTo<BAccount.bAccountID>))]
        [PXUIField(DisplayName = "Vendor")]
        public virtual int? VendorID { get; set; }
        public abstract class vendorID : PX.Data.BQL.BqlInt.Field<vendorID> { }
        #endregion

        #region GuiType
        [PXDBString(2, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Gui Type", Visibility = PXUIVisibility.Visible, Required = true)]
        [GVList.GVGuiType.APCmInvoice]
        public virtual string GuiType { get; set; }
        public abstract class guiType : PX.Data.BQL.BqlString.Field<guiType> { }
        #endregion

        #region TaxCode
        [PXDBString(1, IsUnicode = true, IsFixed = true)]
        [PXDefault(GVList.GVTaxCode.TAXABLE)]
        [PXUIField(DisplayName = "Tax Code")]
        [GVList.GVTaxCode]
        public virtual string TaxCode { get; set; }
        public abstract class taxCode : PX.Data.BQL.BqlString.Field<taxCode> { }
        #endregion

        #region SalesAmt
        [PXDBDecimal(2)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "SalesAmt", Enabled = false)]
        public virtual Decimal? SalesAmt { get; set; }
        public abstract class salesAmt : PX.Data.BQL.BqlDecimal.Field<salesAmt> { }
        #endregion

        #region TaxRate
        [PXDBDecimal(2)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Tax Rate", Enabled = false)]
        public virtual Decimal? TaxRate { get; set; }
        public abstract class taxRate : PX.Data.BQL.BqlDecimal.Field<taxRate> { }
        #endregion

        #region TaxAmt
        [PXDBDecimal(2)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "TaxAmt", Enabled = false)]
        public virtual Decimal? TaxAmt { get; set; }
        public abstract class taxAmt : PX.Data.BQL.BqlDecimal.Field<taxAmt> { }
        #endregion

        #region TotalAmt
        [PXDBDecimal(2)]
        [PXUIField(DisplayName = "Total Amt", Enabled = false)]
        public virtual Decimal? TotalAmt { get; set; }
        public abstract class totalAmt : PX.Data.BQL.BqlDecimal.Field<totalAmt> { }
        #endregion

        #region ApInvoiceNbr
        [PXDBString(40, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Ap Invoice Nbr")]
        public virtual string ApInvoiceNbr { get; set; }
        public abstract class apInvoiceNbr : PX.Data.BQL.BqlString.Field<apInvoiceNbr> { }
        #endregion

        #region Remark
        [PXDBString(240, IsUnicode = true, InputMask = "")]
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

        #region ConfirmPerson
        [PXDBGuid()]
        [PXUIField(DisplayName = "Confirm Person")]
        [PXSelector(
            typeof(Search<Users.pKID>),
            DescriptionField = typeof(Users.username),
            SubstituteKey = typeof(Users.username)
            )]
        public virtual Guid? ConfirmPerson { get; set; }
        public abstract class confirmPerson : PX.Data.BQL.BqlGuid.Field<confirmPerson> { }
        #endregion

        #region PrintCount
        [PXDBInt()]
        [PXUIField(DisplayName = "Print Count")]
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

        #region Status
        [PXDBString(1, IsUnicode = true, IsFixed = true)]
        [PXDefault(GVList.GVStatus.HOLD)]
        [PXUIField(DisplayName = "Status", Enabled = false)]
        [GVList.GVStatus]
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

        #region VendorName
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Vendor Name", Required = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual string VendorName { get; set; }
        public abstract class vendorName : PX.Data.BQL.BqlString.Field<vendorName> { }
        #endregion

        #region VoidDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Void Date")]
        public virtual DateTime? VoidDate { get; set; }
        public abstract class voidDate : PX.Data.BQL.BqlDateTime.Field<voidDate> { }
        #endregion

        #region VoidReason
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Void Reason")]
        public virtual string VoidReason { get; set; }
        public abstract class voidReason : PX.Data.BQL.BqlString.Field<voidReason> { }
        #endregion

        #region VendorAddress
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Vendor Address")]
        [PXDefault(typeof(Search2<Address.addressLine1,
            InnerJoin<BAccount,On<BAccount.defAddressID,Equal<Address.addressID>>>,
            Where<BAccount.bAccountID,Equal<Current2<vendorID>>>>) , PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual string VendorAddress { get; set; }
        public abstract class vendorAddress : PX.Data.BQL.BqlString.Field<vendorAddress> { }
        #endregion

        #region AccConfirmNbr
        [PXDBString(14, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Acc Confirm Nbr")]
        public virtual string AccConfirmNbr { get; set; }
        public abstract class accConfirmNbr : PX.Data.BQL.BqlString.Field<accConfirmNbr> { }
        #endregion

        #region DeclareBatchNbr
        [PXDBString(20, IsUnicode = true)]
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
    }
}

