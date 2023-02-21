using System;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.IN;

namespace Kedge.DAC
{
    /**
     * === 2021/03/19 Mantis: 0011991 === Althea
     * ADD Fields
     * LaborInsInventoryID int (勞保費工料編號)
     * HealthInsInventoryID int (健保費工料編號)
     * PensionInventoryID int (退休金工料編號)
     * 
     * ===2021/06/15 Mantis: 0012092 ===Althea
     * Add Fields:
     * DiscountInventoryID int (會計票貼)
     * 
     * ===2021/09/28 Mantis: 0012247 === Althea
     * Add Fields:
     * KGRePurchaseInventoryID int (附買回扣款)
     **/
    public class DayofMonth
    {

        public static readonly int[] Values =
        {
            1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31
        };
        public static readonly string[] Labels =
        {
            "1","2","3","4","5","6","7","8","9","10","11","12","13","14","15","16","17","18","19","20","21","22","23","24","25","26","27","28","29","30","31"
        };
        public class ListAttribute : PXIntListAttribute
        {
            public ListAttribute() : base(Values, Labels) { }
        }
    }


    [Serializable]
    public class KGSetUp : IBqlTable
    {
        #region SetupID
        [PXDBInt()]
        [PXUIField(DisplayName = "Setup ID")]
        [PXDefault(1)]
        public virtual int? SetupID { get; set; }
        public abstract class setupID : PX.Data.BQL.BqlInt.Field<setupID> { }
        #endregion

        #region Numbering Setting

        #region KGDailyRenterNumbering
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "KGDaily Renter Numbering")]
        [PXSelector(typeof(Numbering.numberingID))]
        public virtual string KGDailyRenterNumbering { get; set; }
        public abstract class kGDailyRenterNumbering : IBqlField { }
        #endregion

        #region KGDailyReportNumbering
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "KGDaily Report Numbering")]
        [PXSelector(typeof(Numbering.numberingID))]
        public virtual string KGDailyReportNumbering { get; set; }
        public abstract class kGDailyReportNumbering : IBqlField { }
        #endregion

        #region KGValuationNumbering
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "KGValuation Numbering")]
        [PXSelector(typeof(Numbering.numberingID))]
        public virtual string KGValuationNumbering { get; set; }
        public abstract class kGValuationNumbering : IBqlField { }
        #endregion

        #region KGSelfInspectionTempNumbering
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "KGSelf Inspection Temp Numbering")]
        [PXSelector(typeof(Numbering.numberingID))]
        public virtual string KGSelfInspectionTempNumbering { get; set; }
        public abstract class kGSelfInspectionTempNumbering : IBqlField { }
        #endregion

        #region KGSelfInspectionNumbering
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "KGSelf Inspection Numbering")]
        [PXSelector(typeof(Numbering.numberingID))]
        public virtual string KGSelfInspectionNumbering { get; set; }
        public abstract class kGSelfInspectionNumbering : IBqlField { }
        #endregion

        #region KGMonthlyInspectionTempNumbering
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "KGMonthly Inspection Temp Numbering")]
        [PXSelector(typeof(Numbering.numberingID))]
        public virtual string KGMonthlyInspectionTempNumbering { get; set; }
        public abstract class kGMonthlyInspectionTempNumbering : IBqlField { }
        #endregion

        #region KGMonthlyInspectionNumbering
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "KGMonthly Inspection Numbering")]
        [PXSelector(typeof(Numbering.numberingID))]
        public virtual string KGMonthlyInspectionNumbering { get; set; }
        public abstract class kGMonthlyInspectionNumbering : IBqlField { }
        #endregion

        #region KGSafetyHealthInspectionTempNumbering
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXSelector(typeof(Numbering.numberingID))]
        [PXUIField(DisplayName = "KGSafety Health Inspection Temp Numbering")]
        public virtual string KGSafetyHealthInspectionTempNumbering { get; set; }
        public abstract class kGSafetyHealthInspectionTempNumbering : IBqlField { }
        #endregion

        #region KGSafetyHealthInspectionNumbering
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXSelector(typeof(Numbering.numberingID))]
        [PXUIField(DisplayName = "KGSafety Health Inspection Numbering")]
        public virtual string KGSafetyHealthInspectionNumbering { get; set; }
        public abstract class kGSafetyHealthInspectionNumbering : IBqlField { }
        #endregion

        #region KGEvaluationNumbering
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXSelector(typeof(Numbering.numberingID))]
        [PXUIField(DisplayName = "KG Vendor Evaluation Numbering")]
        public virtual string KGEvaluationNumbering { get; set; }
        public abstract class kGEvaluationNumbering : IBqlField { }
        #endregion

        //2019/09/16 ADD
        #region KGSafetyHealthInspectTicketNumbering
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXSelector(typeof(Numbering.numberingID))]
        [PXUIField(DisplayName = "KG Safety Health Inspect Ticket Numbering")]
        public virtual string KGSafetyHealthInspectTicketNumbering { get; set; }
        public abstract class kGSafetyHealthInspectTicketNumbering : IBqlField { }
        #endregion

        #region KGMonthlyInspectTicketNumbering
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXSelector(typeof(Numbering.numberingID))]
        [PXUIField(DisplayName = "KG Monthly Inspect Ticket Numbering")]
        public virtual string KGMonthlyInspectTicketNumbering { get; set; }
        public abstract class kGMonthlyInspectTicketNumbering : IBqlField { }
        #endregion

        #region KGContractEvalNumbering
        [PXDBString(10, IsUnicode = true)]
        [PXSelector(typeof(Numbering.numberingID),
                    DescriptionField = typeof(Numbering.descr))]
        [PXUIField(DisplayName = "Constract Evaluation Numbering")]
        public virtual string KGContractEvalNumbering { get; set; }
        public abstract class kGContractEvalNumbering : IBqlField { }
        #endregion

        #endregion

        #region Other Setting

        #region KGManageFeeRate
        [PXDBDecimal()]
        [PXUIField(DisplayName = "KGManage Fee Rate")]
        public virtual Decimal? KGManageFeeRate { get; set; }
        public abstract class kGManageFeeRate : IBqlField { }
        #endregion

        #region KGManageFeeTaxRate
        [PXDBDecimal()]
        [PXUIField(DisplayName = "KGManage Fee Tax Rate")]
        public virtual Decimal? KGManageFeeTaxRate { get; set; }
        public abstract class kGManageFeeTaxRate : IBqlField { }
        #endregion

        #region KGAdditionInventoryID
        [PXDBInt()]
        [PXUIField(DisplayName = "KGAdditionInventoryID")]
        [PXSelector(typeof(Search<InventoryItem.inventoryID>),
            typeof(InventoryItem.inventoryID),
            typeof(InventoryItem.inventoryCD),
            typeof(InventoryItem.descr),
            typeof(InventoryItem.itemClassID),
            typeof(InventoryItem.itemStatus),
            typeof(InventoryItem.itemType),
            SubstituteKey = typeof(InventoryItem.inventoryCD),
            DescriptionField = typeof(InventoryItem.descr))]
        public virtual int? KGAdditionInventoryID { get; set; }
        public abstract class kGAdditionInventoryID : IBqlField { }
        #endregion

        #region KGDeductionInventoryID
        [PXDBInt()]
        [PXSelector(typeof(Search<InventoryItem.inventoryID>),
            typeof(InventoryItem.inventoryID),
            typeof(InventoryItem.inventoryCD),
            typeof(InventoryItem.descr),
            typeof(InventoryItem.itemClassID),
            typeof(InventoryItem.itemStatus),
            typeof(InventoryItem.itemType),
            SubstituteKey = typeof(InventoryItem.inventoryCD),
            DescriptionField = typeof(InventoryItem.descr))]
        [PXUIField(DisplayName = "KGDeductionInventoryID")]
        public virtual int? KGDeductionInventoryID { get; set; }
        public abstract class kGDeductionInventoryID : IBqlField { }
        #endregion

        //2019/06/26新增
        #region KGDeductionTaxFreeInventoryID
        [PXDBInt()]
        [PXSelector(typeof(Search<InventoryItem.inventoryID>),
            typeof(InventoryItem.inventoryID),
            typeof(InventoryItem.inventoryCD),
            typeof(InventoryItem.descr),
            typeof(InventoryItem.itemClassID),
            typeof(InventoryItem.itemStatus),
            typeof(InventoryItem.itemType),
            SubstituteKey = typeof(InventoryItem.inventoryCD),
            DescriptionField = typeof(InventoryItem.descr))]
        [PXUIField(DisplayName = "KGDeductionTaxFreeInventoryID")]
        public virtual int? KGDeductionTaxFreeInventoryID { get; set; }
        public abstract class kGDeductionTaxFreeInventoryID : IBqlField { }
        #endregion

        #region KGRetainageReturnInventoryID
        [PXDBInt()]
        [PXSelector(typeof(Search<InventoryItem.inventoryID>),
                typeof(InventoryItem.inventoryID),
                typeof(InventoryItem.inventoryCD),
                typeof(InventoryItem.descr),
                typeof(InventoryItem.itemClassID),
                typeof(InventoryItem.itemStatus),
                typeof(InventoryItem.itemType),
                SubstituteKey = typeof(InventoryItem.inventoryCD),
                DescriptionField = typeof(InventoryItem.descr))]
        [PXUIField(DisplayName = "KGRetainageReturnInventoryID")]
        public virtual int? KGRetainageReturnInventoryID { get; set; }
        public abstract class kGRetainageReturnInventoryID : IBqlField { }
        #endregion

        // Add 2019/09/10
        #region KGSupplPremiumRate
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Supplementary Preminum Rate")]
        public virtual Decimal? KGSupplPremiumRate { get; set; }
        public abstract class kGSupplPremiumRate : IBqlField { }
        #endregion

        #region KGManageInventoryID
        [PXDBInt()]
        [PXSelector(typeof(Search<InventoryItem.inventoryID>),
            typeof(InventoryItem.inventoryID),
            typeof(InventoryItem.inventoryCD),
            typeof(InventoryItem.descr),
            typeof(InventoryItem.itemClassID),
            typeof(InventoryItem.itemStatus),
            typeof(InventoryItem.itemType),
            SubstituteKey = typeof(InventoryItem.inventoryCD),
            DescriptionField = typeof(InventoryItem.descr))]
        [PXUIField(DisplayName = "KGManageInventoryID")]
        public virtual int? KGManageInventoryID { get; set; }
        public abstract class kGManageInventoryID : IBqlField { }
        #endregion

        #region KGIndividualTaxInventoryID
        [PXDBInt()]
        [PXSelector(typeof(Search<InventoryItem.inventoryID>),
                    typeof(InventoryItem.inventoryID),
                    typeof(InventoryItem.inventoryCD),
                    typeof(InventoryItem.descr),
                    typeof(InventoryItem.itemClassID),
                    typeof(InventoryItem.itemStatus),
                    typeof(InventoryItem.itemType),
                    SubstituteKey = typeof(InventoryItem.inventoryCD),
                    DescriptionField = typeof(InventoryItem.descr))]
        [PXUIField(DisplayName = "Withholding Tax")]
        public virtual int? KGIndividualTaxInventoryID { get; set; }
        public abstract class kGIndividualTaxInventoryID : IBqlField { }
        #endregion

        #region KGSupplementaryTaxInventoryID
        [PXDBInt()]
        [PXSelector(typeof(Search<InventoryItem.inventoryID>),
                    typeof(InventoryItem.inventoryID),
                    typeof(InventoryItem.inventoryCD),
                    typeof(InventoryItem.descr),
                    typeof(InventoryItem.itemClassID),
                    typeof(InventoryItem.itemStatus),
                    typeof(InventoryItem.itemType),
                    SubstituteKey = typeof(InventoryItem.inventoryCD),
                    DescriptionField = typeof(InventoryItem.descr))]
        [PXUIField(DisplayName = "Supplementary Premium")]
        public virtual int? KGSupplementaryTaxInventoryID { get; set; }
        public abstract class kGSupplementaryTaxInventoryID : IBqlField { }
        #endregion

        #region KGDefEvaluationID
        [PXDBInt()]
        [PXSelector(typeof(Search<KGVendorEvaluation.evaluationID>),
                    typeof(KGVendorEvaluation.evaluationCD),
                    typeof(KGVendorEvaluation.evaluationName),
                    SubstituteKey = typeof(KGVendorEvaluation.evaluationCD),
                    DescriptionField = typeof(KGVendorEvaluation.evaluationName))]
        [PXUIField(DisplayName = "Default Evaluation ID")]
        public virtual int? KGDefEvaluationID { get; set; }
        public abstract class kGDefEvaluationID : IBqlField { }
        #endregion

        #region KGDailyRenterDateLimit
        [PXDBInt()]
        [PXUIField(DisplayName = "KGDailyRenterDateLimit")]
        public virtual int? KGDailyRenterDateLimit { get; set; }
        public abstract class kGDailyRenterDateLimit : IBqlField { }
        #endregion

        #region KGBillStartDate
        [PXDBInt]
        [PXUIField(DisplayName = "KGBillStartDate")]
        [DayofMonth.List]
        public virtual int? KGBillStartDate { get; set; }
        public abstract class kGBillStartDate : PX.Data.BQL.BqlInt.Field<kGBillStartDate> { }
        #endregion

        #region KGBillEndDate
        [PXDBInt]
        [PXUIField(DisplayName = "KGBillEndDate")]
        [DayofMonth.List]
        public virtual int? KGBillEndDate { get; set; }
        public abstract class kGBillEndDate : PX.Data.BQL.BqlInt.Field<kGBillEndDate> { }
        #endregion

        #region KGDefEvaluationPct
        [PXDBInt()]
        [PXUIField(DisplayName = "Default Evaluation %")]
        public virtual int? KGDefEvaluationPct { get; set; }
        public abstract class kGDefEvaluationPct : IBqlField{ }
        #endregion

        #region KGDefSecEvaluationPct
        [PXDBInt()]
        [PXUIField(DisplayName = "Default Second Evaluation %")]
        public virtual int? KGDefSecEvaluationPct { get; set; }
        public abstract class kGDefSecEvaluationPct : IBqlField { }
        #endregion

        #region BillAdjustAmtLimit
        [PXDBDecimal()]
        [PXUIField(DisplayName = "BillAdjustAmtLimit")]
        public virtual Decimal? BillAdjustAmtLimit { get; set; }
        public abstract class billAdjustAmtLimit : IBqlField { }
        #endregion

        #endregion

        #region WorkFlow
        //2019/06/26把Numbering欄位拿掉
        /*
        #region KGRequestNumbering
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "KG Request Numbering")]
        [PXSelector(typeof(Numbering.numberingID))]
        public virtual string KGRequestNumbering { get; set; }
        public abstract class kGRequestNumbering : IBqlField { }
        #endregion

        #region KGRQRequisitionNumbering
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "KGRQ Requisition Numbering")]
        [PXSelector(typeof(Numbering.numberingID))]
        public virtual string KGRQRequisitionNumbering { get; set; }
        public abstract class kGRQRequisitionNumbering : IBqlField { }
        #endregion

        #region KGPOOrderNumbering
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "KGPO Order Numbering")]
        [PXSelector(typeof(Numbering.numberingID))]
        public virtual string KGPOOrderNumbering { get; set; }
        public abstract class kGPOOrderNumbering : IBqlField { }
        #endregion

        #region KGAPInvoiceNumbering
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "KGAP Invoice Numbering")]
        [PXSelector(typeof(Numbering.numberingID))]
        public virtual string KGAPInvoiceNumbering { get; set; }
        public abstract class kGAPInvoiceNumbering : IBqlField { }
        #endregion

        #region KGPMChangeOrderNumbering
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "KGPM Change Order Numbering")]
        [PXSelector(typeof(Numbering.numberingID))]
        public virtual string KGPMChangeOrderNumbering { get; set; }
        public abstract class kGPMChangeOrderNumbering : IBqlField { }
        #endregion*/

        #region KGRequestWSDL
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "KGRequestWSDL")]
        public virtual string KGRequestWSDL { get; set; }
        public abstract class kGRequestWSDL : IBqlField { }
        #endregion

        #region KGRQRequisitionWSDL
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "KGRQRequisitionWSDL")]
        public virtual string KGRQRequisitionWSDL { get; set; }
        public abstract class kGRQRequisitionWSDL : IBqlField { }
        #endregion

        #region KGPOOrderWSDL
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "KGPOOrderWSDL")]
        public virtual string KGPOOrderWSDL { get; set; }
        public abstract class kGPOOrderWSDL : IBqlField { }
        #endregion

        #region KGAPInvoiceWSDL
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "KGAPInvoiceWSDL")]
        public virtual string KGAPInvoiceWSDL { get; set; }
        public abstract class kGAPInvoiceWSDL : IBqlField { }
        #endregion

        #region KGPMChangeOrderWSDL
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "KGPMChangeOrderWSDL")]
        public virtual string KGPMChangeOrderWSDL { get; set; }
        public abstract class kGPMChangeOrderWSDL : IBqlField { }
        #endregion

        #region KGFinWSDL
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "KGFinWSDL")]
        public virtual string KGFinWSDL { get; set; }
        public abstract class kGFinWSDL : IBqlField { }
        #endregion
        #endregion

        //2019/07/04 ADD
        //2019/10/24 Move to KGPostageSetup
       /* #region Account Setting

        #region KGCashAccountID
        [PXDBInt()]
        [PXSelector(typeof(Search<Account.accountID,
            Where<Account.active,Equal<True>>>),
            typeof(Account.accountCD),
            typeof(Account.description),
            typeof(Account.active),
            SubstituteKey = typeof(Account.accountCD),
            DescriptionField = typeof(Account.description))]
        [PXUIField(DisplayName = "KGCashAccountID")]
        public virtual int? KGCashAccountID { get; set; }
        public abstract class kGCashAccountID : IBqlField { }
        #endregion

        #region KGCheckAccountID
        [PXDBInt()]
        [PXSelector(typeof(Search<Account.accountID,
            Where<Account.active, Equal<True>>>),
            typeof(Account.accountCD),
            typeof(Account.description),
            typeof(Account.active),
            SubstituteKey = typeof(Account.accountCD),
            DescriptionField = typeof(Account.description))]
        [PXUIField(DisplayName = "KGCheckAccountID")]
        public virtual int? KGCheckAccountID { get; set; }
        public abstract class kGCheckAccountID : IBqlField { }
        #endregion

        #region KGPostageAccountID
        [PXDBInt()]
        [PXSelector(typeof(Search<Account.accountID,
            Where<Account.active, Equal<True>>>),
            typeof(Account.accountCD),
            typeof(Account.description),
            typeof(Account.active),
            SubstituteKey = typeof(Account.accountCD),
            DescriptionField = typeof(Account.description))]
        [PXUIField(DisplayName = "KGPostageAccountID")]
        public virtual int? KGPostageAccountID { get; set; }
        public abstract class kGPostageAccountID : IBqlField { }
        #endregion

        #endregion*/

        //2019/10/23 ADD
        #region Payment Date

        #region PaymentDayFrom
        [PXDBInt(MinValue = 1, MaxValue = 31)]
        [PXUIField(DisplayName = "PaymentDay From")]
        public virtual int? PaymentDayFrom { get; set; }
        public abstract class paymentDayFrom : IBqlField { }
        #endregion

        #region PaymentDayTo
        [PXDBInt(MinValue = 1, MaxValue = 31)]
        [PXUIField(DisplayName = "PaymentDay To")]
        public virtual int? PaymentDayTo { get; set; }
        public abstract class paymentDayTo : IBqlField { }
        #endregion

        #region PaymentDateMid
        [PXDBInt(MinValue = 1, MaxValue = 31)]
        [PXUIField(DisplayName = "PaymentDate Mid")]
        public virtual int? PaymentDateMid { get; set; }
        public abstract class paymentDateMid : IBqlField { }
        #endregion

        #region PaymentDateEnd
        [PXDBInt( MinValue = 1, MaxValue = 31)]
        [PXUIField(DisplayName = "PaymentDate End")]
        public virtual int? PaymentDateEnd { get; set; }
        public abstract class paymentDateEnd : IBqlField { }
        #endregion

        #endregion

        #region CreatedByID
        [PXDBCreatedByID()]
        public virtual Guid? CreatedByID { get; set; }
        public abstract class createdByID : IBqlField { }
        #endregion

        #region CreatedByScreenID
        [PXDBCreatedByScreenID()]
        public virtual string CreatedByScreenID { get; set; }
        public abstract class createdByScreenID : IBqlField { }
        #endregion

        #region CreatedDateTime
        [PXDBCreatedDateTime()]
        public virtual DateTime? CreatedDateTime { get; set; }
        public abstract class createdDateTime : IBqlField { }
        #endregion

        #region LastModifiedByID
        [PXDBLastModifiedByID()]
        public virtual Guid? LastModifiedByID { get; set; }
        public abstract class lastModifiedByID : IBqlField { }
        #endregion

        #region LastModifiedByScreenID
        [PXDBLastModifiedByScreenID()]
        public virtual string LastModifiedByScreenID { get; set; }
        public abstract class lastModifiedByScreenID : IBqlField { }
        #endregion

        #region LastModifiedDateTime
        [PXDBLastModifiedDateTime()]
        public virtual DateTime? LastModifiedDateTime { get; set; }
        public abstract class lastModifiedDateTime : IBqlField { }
        #endregion

        #region NoteID
        [PXNote()]
        [PXUIField(DisplayName = "NoteID")]
        public virtual Guid? NoteID { get; set; }
        public abstract class noteID : IBqlField { }
        #endregion

        #region Tstamp
        [PXDBTimestamp()]
        [PXUIField(DisplayName = "Tstamp")]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : IBqlField { }
        #endregion

        /*#region BranchID
        public abstract class branchID : PX.Data.BQL.BqlInt.Field<branchID>
        {
        }
        protected Int32? _BranchID;

        /// <summary>
        /// Identifier of the <see cref="PX.Objects.GL.Branch">Branch</see>, to which the transaction belongs.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="PX.Objects.GL.Branch.BranchID">Branch.BranchID</see> field.
        /// </value>
		[Branch()]
        public virtual Int32? BranchID
        {
            get
            {
                return this._BranchID;
            }
            set
            {
                this._BranchID = value;
            }
        }
        #endregion*/

        //2021/03/19 Althea add
        #region Without Holding Tax Setting
        #region LaborInsInventoryID
        [PXDBInt()]
        [PXSelector(typeof(Search<InventoryItem.inventoryID>),
            typeof(InventoryItem.inventoryID),
            typeof(InventoryItem.inventoryCD),
            typeof(InventoryItem.descr),
            typeof(InventoryItem.itemClassID),
            typeof(InventoryItem.itemStatus),
            typeof(InventoryItem.itemType),
            SubstituteKey = typeof(InventoryItem.inventoryCD),
            DescriptionField = typeof(InventoryItem.descr))]
        [PXUIField(DisplayName = "LaborInsInventoryID")]
        public virtual int? LaborInsInventoryID { get; set; }
        public abstract class laborInsInventoryID : IBqlField { }
        #endregion

        #region HealthInsInventoryID
        [PXDBInt()]
        [PXSelector(typeof(Search<InventoryItem.inventoryID>),
            typeof(InventoryItem.inventoryID),
            typeof(InventoryItem.inventoryCD),
            typeof(InventoryItem.descr),
            typeof(InventoryItem.itemClassID),
            typeof(InventoryItem.itemStatus),
            typeof(InventoryItem.itemType),
            SubstituteKey = typeof(InventoryItem.inventoryCD),
            DescriptionField = typeof(InventoryItem.descr))]
        [PXUIField(DisplayName = "HealthInsInventoryID")]
        public virtual int? HealthInsInventoryID { get; set; }
        public abstract class healthInsInventoryID : IBqlField { }
        #endregion

        #region PensionInventoryID
        [PXDBInt()]
        [PXSelector(typeof(Search<InventoryItem.inventoryID>),
            typeof(InventoryItem.inventoryID),
            typeof(InventoryItem.inventoryCD),
            typeof(InventoryItem.descr),
            typeof(InventoryItem.itemClassID),
            typeof(InventoryItem.itemStatus),
            typeof(InventoryItem.itemType),
            SubstituteKey = typeof(InventoryItem.inventoryCD),
            DescriptionField = typeof(InventoryItem.descr))]
        [PXUIField(DisplayName = "PensionInventoryID")]
        public virtual int? PensionInventoryID { get; set; }
        public abstract class pensionInventoryID : IBqlField { }
        #endregion
        #endregion

        //2021-05-31 Alton add
        #region PS-一般請款預設客戶
        #region DefVendorCustID
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [CustomerActive(DisplayName = "DefVendorCustID", DescriptionField = typeof(Customer.acctName))]
        public virtual int? DefVendorCustID { get; set; }
        public abstract class defVendorCustID : PX.Data.BQL.BqlInt.Field<defVendorCustID> { }
        #endregion

        #region DefEmployeeCustID
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [CustomerActive(DisplayName = "DefEmployeeCustID", DescriptionField = typeof(Customer.acctName))]
        public virtual int? DefEmployeeCustID { get; set; }
        public abstract class defEmployeeCustID : PX.Data.BQL.BqlInt.Field<defEmployeeCustID> { }
        #endregion
        #endregion

        //2021/06/15 Althea add
        #region DiscountInventoryID
        [PXDBInt()]
        [PXUIField(DisplayName = "DiscountInventoryID")]
        [PXSelector(typeof(Search<InventoryItem.inventoryID>),
            typeof(InventoryItem.inventoryID),
            typeof(InventoryItem.inventoryCD),
            typeof(InventoryItem.descr),
            typeof(InventoryItem.itemClassID),
            typeof(InventoryItem.itemStatus),
            typeof(InventoryItem.itemType),
            SubstituteKey = typeof(InventoryItem.inventoryCD),
            DescriptionField = typeof(InventoryItem.descr))]
        public virtual int? DiscountInventoryID { get; set; }
        public abstract class discountInventoryID : IBqlField { }
        #endregion

        //2021/07/19 Althea Add
        #region KGFognWorkerInventoryID
        [PXDBInt()]
        [PXUIField(DisplayName = "KGFognWorkerInventoryID")]
        [PXSelector(typeof(Search<InventoryItem.inventoryID>),
            typeof(InventoryItem.inventoryID),
            typeof(InventoryItem.inventoryCD),
            typeof(InventoryItem.descr),
            typeof(InventoryItem.itemClassID),
            typeof(InventoryItem.itemStatus),
            typeof(InventoryItem.itemType),
            SubstituteKey = typeof(InventoryItem.inventoryCD),
            DescriptionField = typeof(InventoryItem.descr))]
        public virtual int? KGFognWorkerInventoryID { get; set; }
        public abstract class kGFognWorkerInventoryID : IBqlField { }
        #endregion

        //2021/09/28 Althea Add
        #region KGRePurchaseInventoryID
        [PXDBInt()]
        [PXUIField(DisplayName = "RePurchaseInventoryID")]
        [PXSelector(typeof(Search<InventoryItem.inventoryID>),
            typeof(InventoryItem.inventoryID),
            typeof(InventoryItem.inventoryCD),
            typeof(InventoryItem.descr),
            typeof(InventoryItem.itemClassID),
            typeof(InventoryItem.itemStatus),
            typeof(InventoryItem.itemType),
            SubstituteKey = typeof(InventoryItem.inventoryCD),
            DescriptionField = typeof(InventoryItem.descr))]
        public virtual int? KGRePurchaseInventoryID { get; set; }
        public abstract class kGRePurchaseInventoryID : IBqlField { }
        #endregion

    }
}