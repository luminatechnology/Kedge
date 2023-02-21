using System;
using PX.Data;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.CS;
using PX.Objects.CR;
using PX.Objects.CT;
using PX.Objects.AR;
using PX.Objects.AP;
using PX.Objects.EP;
using PX.Objects.PM;
using TX = PX.Objects.TX;
using PXEP = PX.Objects.EP;
using GL = PX.Objects.GL;
using PX.Data.EP;
using PX.Objects.PO;
using PX.Objects.CA;
using NM.Util;
using NM.DAC;
using static PS.Util.PSStringList;
using static CC.Util.CCList;
using KG.Util;
using CC.DAC;

namespace PS.DAC
{
    /**
     * =====2021-02-05:11916=====
     * 1.PSPaymentSlip.DocType='OTH'時，不需填寫「繳款單分類」及「銀行帳號簡碼」
     * 2.PSPaymentSlip.DocType='RGU'時： GuarPayableCD 的LOV內只能挑選到 CCPayableCheck.Status=Release 的應付保證票
     * 
     * ====2021-05-06====Alton
     * DocType = apRtnGuarCheck 時 DueDate不要必填
     * 
     * ====2021-05-31====Alton
     * 1. PS301010 繳款單明細 針對 "一般繳款" 與 "其他" 須改回 Radio Button模式
     * a. 選項是: 客戶、供應商、員工
     *    對應要顯示 可以選擇 客戶、供應商、員工
     *    (把畫面上 EmployeeID 請求者 拉過來給員工用)
     * 
     *    註: 針對"應收保證票" 與"繳回應付保證票" 仍然維持 客戶+供應商only
     * 
     * b. 畫面上要放出來顯示 CreateBy (ID + Desc)
     * 
     * ====2021-07-08:12140====Alton
     * PS301000繳款單 doctype= 應收保證票 時，表身到期日修改為非必填
     * ActualDueDate 實際到期日改非必填
     * 
     *  ====2021-10-05:12253====Alton
     * 請在PSPaymentSlipDetails新增一欄位 CCPostageAmt,decimal(18,6),非必填,預設=0
     * --當此欄金額異動時，記得將PSPaymentSlip.DocBal的金額減少
     * 
     * ====2021-11-03:12262====
     * DocType=繳回應付保證票時，請在 繳款單明細 及 文件號碼的LOV 中，皆放上當初 應付保證票 核可時的傳票號碼（CCPayableCheck.GuarReleaseNbr)
     * 
     * **/
    [Serializable]
    public class PSPaymentSlipDetails : IBqlTable
    {
        #region PaymentSlipDetailID
        [PXDBIdentity(IsKey = true)]
        [PXUIField(DisplayName = "Payment Slip Detail ID")]
        public virtual int? PaymentSlipDetailID { get; set; }
        public abstract class paymentSlipDetailID : PX.Data.BQL.BqlInt.Field<paymentSlipDetailID> { }
        #endregion

        #region BranchID
        [Branch(typeof(PSPaymentSlip.branchID))]
        public virtual int? BranchID { get; set; }
        public abstract class branchID : PX.Data.BQL.BqlInt.Field<branchID> { }
        #endregion

        #region RefNbr
        [PXDBString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Ref Nbr")]
        [PXDBDefault(typeof(PSPaymentSlip.refNbr))]
        [PXParent(typeof(Select<PSPaymentSlip,
                            Where<PSPaymentSlip.refNbr,
                            Equal<Current<refNbr>>>>))]
        public virtual string RefNbr { get; set; }
        public abstract class refNbr : PX.Data.BQL.BqlString.Field<refNbr> { }
        #endregion

        #region PaymentRefNbr
        [PXDBString(15, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
        [PXUIRequired(typeof(Where<paymentMethodID, Equal<PAYMETHOD_CHECK>,
            Or<Where<Current<PSPaymentSlip.docType>, Equal<PSDocType.arGuarCheck>,
            And<guarClass, NotEqual<GuarClassList.commercialPaper>>>>>))]
        [PXUIField(DisplayName = "Payment Ref Nbr")]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual string PaymentRefNbr { get; set; }
        public abstract class paymentRefNbr : PX.Data.BQL.BqlString.Field<paymentRefNbr> { }
        #endregion

        #region SlipDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Slip Date", Required = true)]
        [PXDefault(typeof(AccessInfo.businessDate), PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual DateTime? SlipDate { get; set; }
        public abstract class slipDate : PX.Data.BQL.BqlDateTime.Field<slipDate> { }
        #endregion

        #region InventoryID
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [Inventory(DisplayName = "Expense Item", Required = false)]
        //[PXRestrictor(typeof(Where<InventoryItem.itemType, Equal<INItemTypes.expenseItem>>), EP.Messages.InventoryItemIsNotAnExpenseType)]
        [PXForeignReference(typeof(Field<inventoryID>.IsRelatedTo<InventoryItem.inventoryID>))]
        public virtual int? InventoryID { get; set; }
        public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }
        #endregion

        #region Uom
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [INUnit(typeof(inventoryID), DisplayName = "UOM", Required = false)]
        [PXUIEnabled(typeof(Where<inventoryID, IsNotNull, And<FeatureInstalled<FeaturesSet.multipleUnitMeasure>>>))]
        [PXFormula(typeof(Switch<Case<Where<inventoryID, IsNull>, Null>, Selector<inventoryID, InventoryItem.purchaseUnit>>))]
        public virtual string Uom { get; set; }
        public abstract class uom : PX.Data.BQL.BqlString.Field<uom> { }
        #endregion

        #region Qty
        [PXDBQuantity]
        [PXDefault(TypeCode.Decimal, "1.0")]
        [PXUIField(DisplayName = "Quantity", Visibility = PXUIVisibility.Visible)]
        [PXUIVerify(typeof(Where<qty, NotEqual<decimal0>, Or<Selector<contractID, Contract.nonProject>, Equal<True>>>), PXErrorLevel.Error, PXEP.Messages.ValueShouldBeNonZero)]
        public virtual Decimal? Qty { get; set; }
        public abstract class qty : PX.Data.BQL.BqlDecimal.Field<qty> { }
        #endregion

        #region UnitCost
        [PXUIField(DisplayName = "Amount")]
        [PXDBPriceCost]
        [PXDefault(typeof(Search<INItemCost.lastCost, Where<INItemCost.inventoryID, Equal<Current<inventoryID>>>>))]
        public virtual Decimal? UnitCost { get; set; }
        public abstract class unitCost : PX.Data.BQL.BqlDecimal.Field<unitCost> { }
        #endregion

        #region TranAmt
        [PXDBDecimal(4)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Original Claim Amount", IsReadOnly = true)]
        public virtual Decimal? TranAmt { get; set; }
        public abstract class tranAmt : PX.Data.BQL.BqlDecimal.Field<tranAmt> { }
        #endregion

        #region TranDesc
        [PXDBString(256, IsUnicode = true)]
        [PXDefault(typeof(Current<PSPaymentSlip.docDesc>))]
        [PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.Visible)]
        public virtual string TranDesc { get; set; }
        public abstract class tranDesc : PX.Data.BQL.BqlString.Field<tranDesc> { }
        #endregion

        #region CustomerID
        [PXDefault(typeof(PSPaymentSlip.customerID), PersistingCheck = PXPersistingCheck.Nothing)]
        [CustomerActive(DescriptionField = typeof(Customer.acctName), Required = true)]
        [PXUIVisible(typeof(Where<PSPaymentSlip.targetType, Equal<PSTargetType.customer>>))]
        [PXUIEnabled(typeof(False))]
        public virtual int? CustomerID { get; set; }
        public abstract class customerID : PX.Data.BQL.BqlInt.Field<customerID> { }
        #endregion

        #region CustomerLocationID
        [PXDefault(typeof(PSPaymentSlip.customerLocationID), PersistingCheck = PXPersistingCheck.Nothing)]
        [LocationID(typeof(Where<Location.bAccountID, Equal<Current2<customerID>>>), DescriptionField = typeof(Location.descr), Required = true)]
        [PXUIVisible(typeof(Where<PSPaymentSlip.targetType, Equal<PSTargetType.customer>>))]
        [PXUIEnabled(typeof(False))]
        public virtual int? CustomerLocationID { get; set; }
        public abstract class customerLocationID : PX.Data.BQL.BqlInt.Field<customerLocationID> { }
        #endregion

        #region ContractID
        [ProjectBase()]
        [PXRestrictor(typeof(Where<PMProject.isCancelled, Equal<False>>), PX.Objects.PM.Messages.CancelledContract, typeof(PMProject.contractCD))]
        [PXRestrictor(typeof(Where<PMProject.visibleInPO, Equal<True>, Or<PMProject.nonProject, Equal<True>>>), PX.Objects.PM.Messages.ProjectInvisibleInModule, typeof(PMProject.contractCD))]
        [PXRestrictor(typeof(Where<PMProject.defaultBranchID, Equal<Current<AccessInfo.branchID>>, Or<PMProject.defaultBranchID, IsNull>>), "Branch Not Found.", typeof(PMProject.contractCD))]
        [PXForeignReference(typeof(Field<contractID>.IsRelatedTo<PMProject.contractID>))]
        [PXUIField(DisplayName = "Project/Contract", Required = true, IsReadOnly = true)]
        [PXDefault(typeof(PSPaymentSlip.contractID))]
        public virtual int? ContractID { get; set; }
        public abstract class contractID : PX.Data.BQL.BqlInt.Field<contractID> { }
        #endregion

        #region EmployeeID
        [PXDBInt]
        [PXUIField(DisplayName = "EmployeeID")]
        [PXEPEmployeeSelector]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        //[PXUIVisible(typeof(Where<Current<PSPaymentSlip.docType>,Equal<PSDocType.payment>>))]
        //2022-12-27 Alton mantis:12385 開放維護
        //[PXUIRequired(typeof(Where<Current<PSPaymentSlip.docType>, Equal<PSDocType.payment>>))]
        [PXUIVisible(typeof(Where<Current<PSPaymentSlip.docType>, Equal<PSDocType.payment>>))]
        public virtual int? EmployeeID { get; set; }
        public abstract class employeeID : PX.Data.BQL.BqlInt.Field<employeeID> { }
        #endregion

        #region 20200925 mark by alton 繳款單不維護供應商，將來user要輸入供應商資料就得去客戶主檔新增供應商資料
        #region VendorID
        [PXDefault(typeof(PSPaymentSlip.vendorID), PersistingCheck = PXPersistingCheck.Nothing)]
        [POVendor(DescriptionField = typeof(Vendor.acctName))]
        [PXUIVisible(typeof(Where<PSPaymentSlip.targetType, Equal<PSTargetType.vendor>>))]
        [PXUIEnabled(typeof(False))]
        public virtual int? VendorID { get; set; }
        public abstract class vendorID : PX.Data.BQL.BqlInt.Field<vendorID> { }
        #endregion

        #region VendorLocationID
        [PXUIField(DisplayName = "Vendor Location ID")]
        [PXDefault(typeof(PSPaymentSlip.vendorLocationID), PersistingCheck = PXPersistingCheck.Nothing)]
        [LocationID(typeof(Where<Location.bAccountID, Equal<Current2<vendorID>>>), DescriptionField = typeof(Location.descr))]
        [PXUIVisible(typeof(Where<PSPaymentSlip.targetType, Equal<PSTargetType.vendor>>))]
        [PXUIEnabled(typeof(False))]
        public virtual int? VendorLocationID { get; set; }
        public abstract class vendorLocationID : PX.Data.BQL.BqlInt.Field<vendorLocationID> { }
        #endregion
        #endregion

        #region PaymentMethodID
        [PXDBString()]
        [PXUIField(DisplayName = "Payment Method", Required = true)]
        [PXSelector(typeof(Search<PaymentMethod.paymentMethodID,
            Where<True, Equal<True>>>),
            typeof(PaymentMethod.paymentMethodID))]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIVisible(typeof(Where<Current<PSPaymentSlip.docType>, NotEqual<PSDocType.arGuarCheck>,
            And<Current<PSPaymentSlip.docType>, NotEqual<PSDocType.apRtnGuarCheck>>>))]
        [PXUIEnabled(typeof(Where<Current<PSPaymentSlip.docType>, NotEqual<PSDocType.arGuarCheck>,
            And<Current<PSPaymentSlip.docType>, NotEqual<PSDocType.apRtnGuarCheck>>>))]
        [PXUIRequired(typeof(Where<Current<PSPaymentSlip.docType>, NotEqual<PSDocType.arGuarCheck>,
            And<Current<PSPaymentSlip.docType>, NotEqual<PSDocType.apRtnGuarCheck>>>))]
        public virtual string PaymentMethodID { get; set; }
        public abstract class paymentMethodID : PX.Data.BQL.BqlString.Field<paymentMethodID> { }
        #endregion

        #region BankAccountID
        [PXDBInt()]
        [PXUIField(DisplayName = "Bank Account ID")]
        [PXUIRequired(typeof(Where<Current<PSPaymentSlip.status>, Equal<PSStatus.open>,
            And<Current<PSPaymentSlip.docType>,
                NotIn3<PSDocType.arGuarCheck, PSDocType.apRtnGuarCheck, PSDocType.other>>>))]
        [PXUIVisible(typeof(Where<Current<PSPaymentSlip.docType>,
                NotIn3<PSDocType.arGuarCheck, PSDocType.apRtnGuarCheck, PSDocType.other>>))]
        [PXUIEnabled(typeof(Where<Current<PSPaymentSlip.docType>,
                NotIn3<PSDocType.arGuarCheck, PSDocType.apRtnGuarCheck, PSDocType.other>>))]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [NMBankAccount(
                    SubstituteKey = typeof(NMBankAccount.bankAccountCD),
                    DescriptionField = typeof(NMBankAccount.bankName)
        )]
        [PXRestrictor(typeof(Where<NMBankAccount.paymentMethodID, Equal<Current<paymentMethodID>>,
            Or<Current<paymentMethodID>, Equal<KGConst.cash>>
            >), "PaymentMethod Not Found", typeof(NMBankAccount.paymentMethodID))]
        public virtual int? BankAccountID { get; set; }
        public abstract class bankAccountID : PX.Data.BQL.BqlInt.Field<bankAccountID> { }
        #endregion

        #region ActualDueDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Actual Due Date")]
        //[PXUIRequired(typeof(Where<Current<PSPaymentSlip.status>, Equal<PSStatus.open>>))]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual DateTime? ActualDueDate { get; set; }
        public abstract class actualDueDate : PX.Data.BQL.BqlDateTime.Field<actualDueDate> { }
        #endregion

        #region CheckDueDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Check Due Date")]
        [PXUIRequired(typeof(
            Where<Current<PSPaymentSlip.status>, Equal<PSStatus.open>,
                And<
                Where<paymentMethodID, Equal<PAYMETHOD_CHECK>>>>))]
        [PXUIVisible(typeof(Where<Current<PSPaymentSlip.docType>, NotEqual<PSDocType.arGuarCheck>,
            And<Current<PSPaymentSlip.docType>, NotEqual<PSDocType.apRtnGuarCheck>>>))]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual DateTime? CheckDueDate { get; set; }
        public abstract class checkDueDate : PX.Data.BQL.BqlDateTime.Field<checkDueDate> { }
        #endregion

        #region EtdDepositDate
        [PXDBDate()]
        [PXUIField(DisplayName = "EtdDeposit Date")]
        [PXUIRequired(typeof(Where<Current<PSPaymentSlip.status>, Equal<PSStatus.open>, And<paymentMethodID, Equal<PAYMETHOD_CHECK>>>))]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual DateTime? EtdDepositDate { get; set; }
        public abstract class etdDepositDate : PX.Data.BQL.BqlDateTime.Field<etdDepositDate> { }
        #endregion

        #region CheckIssuer
        [PXDBString(255, IsUnicode = true)]
        [PXUIField(DisplayName = "Check Issuer")]
        public virtual string CheckIssuer { get; set; }
        public abstract class checkIssuer : PX.Data.BQL.BqlString.Field<checkIssuer> { }
        #endregion

        #region ProcessDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Process Date")]
        [PXUIRequired(typeof(Where<Current<PSPaymentSlip.status>, Equal<PSStatus.open>>))]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual DateTime? ProcessDate { get; set; }
        public abstract class processDate : PX.Data.BQL.BqlDateTime.Field<processDate> { }
        #endregion

        #region OriBankCode - 原支票銀行代碼
        [PXDBString()]
        [PXUIField(DisplayName = "Ori Bank Code")]
        [PXUIRequired(typeof(Where<Current<PSPaymentSlip.status>, Equal<PSStatus.hold>, And<paymentMethodID, Equal<PAYMETHOD_CHECK>>>))]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [NMBankCode]
        public virtual string OriBankCode { get; set; }
        public abstract class oriBankCode : PX.Data.BQL.BqlString.Field<oriBankCode> { }
        #endregion

        #region OriBankAccount - 原支票銀行帳戶
        [PXDBString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Ori Bank Account")]
        [PXUIRequired(typeof(Where<Current<PSPaymentSlip.status>, Equal<PSStatus.hold>, And<paymentMethodID, Equal<PAYMETHOD_CHECK>>>))]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual string OriBankAccount { get; set; }
        public abstract class oriBankAccount : PX.Data.BQL.BqlString.Field<oriBankAccount> { }
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

        #region Tstamp
        [PXDBTimestamp()]
        [PXUIField(DisplayName = "Tstamp")]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
        #endregion

        #region NoteId
        [PXNote()]
        [PXUIField(DisplayName = "NoteID")]
        public virtual Guid? NoteID { get; set; }
        public abstract class noteID : PX.Data.BQL.BqlGuid.Field<noteID> { }
        #endregion

        #region PaymentCategory
        [PXDBString()]
        [PXUIField(DisplayName = "Payment Category")]
        [PXUIRequired(typeof(Where<Current<PSPaymentSlip.status>, Equal<PSStatus.open>,
            And<Current<PSPaymentSlip.docType>,
                NotIn3<PSDocType.arGuarCheck, PSDocType.apRtnGuarCheck, PSDocType.other>>>))]
        [PXUIVisible(typeof(Where<Current<PSPaymentSlip.docType>,
                NotIn3<PSDocType.arGuarCheck, PSDocType.apRtnGuarCheck, PSDocType.other>>))]
        [PXUIEnabled(typeof(Where<Current<PSPaymentSlip.docType>,
                NotIn3<PSDocType.arGuarCheck, PSDocType.apRtnGuarCheck, PSDocType.other>>))]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PSPaymentCategory()]
        public virtual string PaymentCategory { get; set; }
        public abstract class paymentCategory : PX.Data.BQL.BqlString.Field<paymentCategory> { }
        #endregion

        #region PaymentCategoryV
        //[PXString()]
        //[PXUIField(DisplayName = "Payment Category")]
        //[PXUIRequired(typeof(Where<Current<PSPaymentSlip.isGuarantee>, Equal<False>>))]
        //[PXUIVisible(typeof(Where<Current<PSPaymentSlip.isGuarantee>, Equal<False>>))]
        //[PXUIEnabled(typeof(Where<Current<PSPaymentSlip.isGuarantee>, Equal<False>>))]
        //[PXDefault(typeof(Current<paymentCategory>), PersistingCheck = PXPersistingCheck.Nothing)]
        //[PXFormula(typeof(Current<paymentCategory>))]
        //[PSPaymentCategory()]
        //public virtual string PaymentCategoryV { get; set; }
        //public abstract class paymentCategoryV : PX.Data.BQL.BqlString.Field<paymentCategoryV> { }
        #endregion

        #region ArPaymentRefNbr - AR302000 RefNbr
        [PXDBString()]
        [PXUIField(DisplayName = "Ar Payment RefNbr", IsReadOnly = true)]
        [PXUIVisible(typeof(Where<Current<PSPaymentSlip.docType>, NotEqual<PSDocType.arGuarCheck>>))]
        public virtual string ArPaymentRefNbr { get; set; }
        public abstract class arPaymentRefNbr : PX.Data.BQL.BqlString.Field<arPaymentRefNbr> { }
        #endregion

        #region CC保證票
        #region IssueDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Issue Date")]
        [PXUIVisible(typeof(Where<Current<PSPaymentSlip.docType>, Equal<PSDocType.arGuarCheck>,
            Or<Current<PSPaymentSlip.docType>, Equal<PSDocType.apRtnGuarCheck>>>))]
        [PXUIEnabled(typeof(Where<Current<PSPaymentSlip.docType>, Equal<PSDocType.arGuarCheck>,
            Or<Current<PSPaymentSlip.docType>, Equal<PSDocType.apRtnGuarCheck>>>))]
        public virtual DateTime? IssueDate { get; set; }
        public abstract class issueDate : PX.Data.BQL.BqlDateTime.Field<issueDate> { }
        #endregion

        #region AuthDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Auth Date")]
        [PXUIVisible(typeof(Where<Current<PSPaymentSlip.docType>, Equal<PSDocType.arGuarCheck>,
            Or<Current<PSPaymentSlip.docType>, Equal<PSDocType.apRtnGuarCheck>>>))]
        [PXUIEnabled(typeof(Where<Current<PSPaymentSlip.docType>, Equal<PSDocType.arGuarCheck>,
            Or<Current<PSPaymentSlip.docType>, Equal<PSDocType.apRtnGuarCheck>>>))]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual DateTime? AuthDate { get; set; }
        public abstract class authDate : PX.Data.BQL.BqlDateTime.Field<authDate> { }
        #endregion

        #region GuarType
        [PXDBString(1, IsFixed = true)]
        [PXUIField(DisplayName = "Guar Type")]
        [GuarTypeList]
        [PXUIVisible(typeof(Where<Current<PSPaymentSlip.docType>, Equal<PSDocType.arGuarCheck>,
            Or<Current<PSPaymentSlip.docType>, Equal<PSDocType.apRtnGuarCheck>>>))]
        [PXUIRequired(typeof(Where<Current<PSPaymentSlip.docType>, Equal<PSDocType.arGuarCheck>,
            Or<Current<PSPaymentSlip.docType>, Equal<PSDocType.apRtnGuarCheck>>>))]
        [PXUIEnabled(typeof(Where<Current<PSPaymentSlip.docType>, Equal<PSDocType.arGuarCheck>,
            Or<Current<PSPaymentSlip.docType>, Equal<PSDocType.apRtnGuarCheck>>>))]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual string GuarType { get; set; }
        public abstract class guarType : PX.Data.BQL.BqlString.Field<guarType> { }
        #endregion

        #region GuarClass
        [PXDBString(2, IsFixed = true)]
        [PXUIField(DisplayName = "Guar Class")]
        [GuarClassList]
        [PXUIVisible(typeof(Where<Current<PSPaymentSlip.docType>, Equal<PSDocType.arGuarCheck>,
            Or<Current<PSPaymentSlip.docType>, Equal<PSDocType.apRtnGuarCheck>>>))]
        [PXUIRequired(typeof(Where<Current<PSPaymentSlip.docType>, Equal<PSDocType.arGuarCheck>,
            Or<Current<PSPaymentSlip.docType>, Equal<PSDocType.apRtnGuarCheck>>>))]
        [PXUIEnabled(typeof(Where<Current<PSPaymentSlip.docType>, Equal<PSDocType.arGuarCheck>,
            Or<Current<PSPaymentSlip.docType>, Equal<PSDocType.apRtnGuarCheck>>>))]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual string GuarClass { get; set; }
        public abstract class guarClass : PX.Data.BQL.BqlString.Field<guarClass> { }
        #endregion

        #region POOrderType
        [PXDBString(2, IsFixed = true, IsUnicode = true)]
        [PXUIField(DisplayName = "POOrderType")]
        [PXUIVisible(typeof(Where<Current<PSPaymentSlip.docType>, Equal<PSDocType.arGuarCheck>,
            Or<Current<PSPaymentSlip.docType>, Equal<PSDocType.apRtnGuarCheck>>>))]
        public virtual string POOrderType { get; set; }
        public abstract class pOOrderType : PX.Data.BQL.BqlString.Field<pOOrderType> { }
        #endregion

        #region PONbr
        [PXDBString(15, IsUnicode = true)]
        [PXUIField(DisplayName = "PONbr")]
        [PXUIEnabled(typeof(Where<vendorID, IsNotNull>))]
        [PXSelector(typeof(Search<POOrder.orderNbr,
            Where<POOrder.vendorID, Equal<Current<vendorID>>,
                And<POOrder.projectID, Equal<Current<contractID>>>>>),
            typeof(POOrder.orderType),
            typeof(POOrder.orderNbr),
            typeof(POOrder.orderDesc),
            typeof(POOrder.orderDate),
            DescriptionField = typeof(POOrder.orderDesc))]
        [PXUIVisible(typeof(Where<Current<PSPaymentSlip.docType>, Equal<PSDocType.arGuarCheck>,
            Or<Current<PSPaymentSlip.docType>, Equal<PSDocType.apRtnGuarCheck>>>))]
        public virtual string PONbr { get; set; }
        public abstract class pONbr : PX.Data.BQL.BqlString.Field<pONbr> { }
        #endregion

        #region IsVoid
        [PXDBBool()]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Is Void", IsReadOnly = true)]
        public virtual bool? IsVoid { get; set; }
        public abstract class isVoid : PX.Data.BQL.BqlBool.Field<isVoid> { }
        #endregion

        #region DueDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Due Date")]
        [PXUIVisible(typeof(Where<Current<PSPaymentSlip.docType>, Equal<PSDocType.arGuarCheck>,
            Or<Current<PSPaymentSlip.docType>, Equal<PSDocType.apRtnGuarCheck>>>))]
        //[PXUIRequired(typeof(Where<Current<PSPaymentSlip.docType>, Equal<PSDocType.arGuarCheck>
        //    //,Or<Current<PSPaymentSlip.docType>, Equal<PSDocType.apRtnGuarCheck>>
        //    >))]
        [PXUIEnabled(typeof(Where<Current<PSPaymentSlip.docType>, Equal<PSDocType.arGuarCheck>,
            Or<Current<PSPaymentSlip.docType>, Equal<PSDocType.apRtnGuarCheck>>>))]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual DateTime? DueDate { get; set; }
        public abstract class dueDate : PX.Data.BQL.BqlDateTime.Field<dueDate> { }
        #endregion

        #region CCPostageAmt
        [PXDBDecimal]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "CC Postage Amt")]
        [PXUIVisible(typeof(Where<Current<PSPaymentSlip.docType>, Equal<PSDocType.arGuarCheck>,
            Or<Current<PSPaymentSlip.docType>, Equal<PSDocType.apRtnGuarCheck>>>))]
        [PXUIEnabled(typeof(Where<Current<PSPaymentSlip.docType>, Equal<PSDocType.arGuarCheck>,
            Or<Current<PSPaymentSlip.docType>, Equal<PSDocType.apRtnGuarCheck>>>))]
        public virtual Decimal? CCPostageAmt { get; set; }
        public abstract class ccPostageAmt : PX.Data.BQL.BqlDecimal.Field<ccPostageAmt> { }
        #endregion

        #region GuarReceviableCD - CC301001 RefNbr
        [PXDBString()]
        [PXUIField(DisplayName = "Guar Receviable CD", IsReadOnly = true)]
        [PXUIVisible(typeof(Where<Current<PSPaymentSlip.docType>, Equal<PSDocType.arGuarCheck>>))]
        public virtual string GuarReceviableCD { get; set; }
        public abstract class guarReceviableCD : PX.Data.BQL.BqlString.Field<guarReceviableCD> { }
        #endregion

        #region GuarPayableCD - CC302001 RefNbr
        [PXDBString()]
        [PXUIField(DisplayName = "Guar Payable CD")]
        [PXSelector(typeof(
            Search2<CCPayableCheck.guarPayableCD,
                LeftJoin<PMProject, On<CCPayableCheck.projectID, Equal<PMProject.contractID>>>,
                Where<True, Equal<True>,
                And2<Where2<
                    Where<Current<PSPaymentSlip.status>, Equal<PSStatus.hold>, And<CCPayableCheck.status, Equal<CCPayableStatus.released>>>,
                    Or2<Where<Current<PSPaymentSlip.status>, Equal<PSStatus.open>, And<CCPayableCheck.status, Equal<CCPayableStatus.appliedRTN>>>,
                    Or<Where<Current<PSPaymentSlip.status>, Equal<PSStatus.released>, And<CCPayableCheck.status, Equal<CCPayableStatus.returned>>>>>>,
                And2<Where<CCPayableCheck.vendorID, Equal<Current<PSPaymentSlip.vendorID>>,
                        Or<CCPayableCheck.customerID, Equal<Current<PSPaymentSlip.customerID>>>>,
                And<CCPayableCheck.projectID, Equal<Current<PSPaymentSlip.contractID>>>
            >>>>),
            typeof(CCPayableCheck.guarPayableCD),
            typeof(PMProject.contractCD),
            typeof(CCPayableCheck.customerVendorCD),
            typeof(CCPayableCheck.customerVendorName),
            typeof(CCPayableCheck.pONbr),
            typeof(CCPayableCheck.guarReleaseNbr),
            typeof(CCPayableCheck.description)
            )]

        [PXUIVisible(typeof(Where<Current<PSPaymentSlip.docType>, Equal<PSDocType.apRtnGuarCheck>>))]
        public virtual string GuarPayableCD { get; set; }
        public abstract class guarPayableCD : PX.Data.BQL.BqlString.Field<guarPayableCD> { }
        #endregion
        #endregion

        #region Unbound
        #region PaymentRefNbrV
        [PXString(IsUnicode = true)]
        [PXUIField(DisplayName = "Payment Ref Nbr", Enabled = false)]
        [PXUnboundDefault(typeof(Current<paymentRefNbr>))]
        public virtual string PaymentRefNbrV { get; set; }
        public abstract class paymentRefNbrV : PX.Data.BQL.BqlString.Field<paymentRefNbrV> { }
        #endregion
        #endregion

        #region const
        public class PAYMETHOD_CHECK : PX.Data.BQL.BqlString.Constant<PAYMETHOD_CHECK> { public PAYMETHOD_CHECK() : base("CHECK") {; } }
        #endregion

    }
}