using System;
using PX.Data;
using PX.Objects.CM;
using PX.Objects.PO;
using PX.Objects.GL;

namespace Kedge.DAC
{

    [Serializable]
    public class KGContractPricingRule : IBqlTable
    {

        #region PricingRuleID
        [PXDBIdentity]
        [PXUIField(DisplayName = "Pricing Rule ID")]
        public virtual int? PricingRuleID { get; set; }
        public abstract class pricingRuleID : IBqlField { }
        #endregion

        #region OrderType
        public abstract class orderType : PX.Data.IBqlField
        {
        }
        protected String _OrderType;
        [PXDBString(2, IsKey = true, IsFixed = true)]
        [PXDBDefault(typeof(POOrder.orderType))]
        [PXUIField(DisplayName = "Order Type", Visibility = PXUIVisibility.Visible, Visible = false)]
        public virtual String OrderType
        {
            get
            {
                return this._OrderType;
            }
            set
            {
                this._OrderType = value;
            }
        }
        #endregion

        #region OrderNbr
        public abstract class orderNbr : PX.Data.IBqlField
        {
        }
        protected String _OrderNbr;

        [PXDBString(15, IsUnicode = true, IsKey = true, InputMask = "")]
        [PXDBDefault(typeof(POOrder.orderNbr))]
        [PXParent(typeof(Select<POOrder, 
            Where<POOrder.orderType, Equal<Current<orderType>>, 
            And<POOrder.orderNbr, Equal<Current<orderNbr>>>>>))]
        [PXUIField(DisplayName = "Order Nbr.", Visibility = PXUIVisibility.Invisible, Visible = false)]
        public virtual String OrderNbr
        {
            get
            {
                return this._OrderNbr;
            }
            set
            {
                this._OrderNbr = value;
            }
        }
        #endregion

        #region PaymentMethod
        [PXString(10)]
        [PXUnboundDefault("採數量計算法")]
        [PXUIField(DisplayName = "Payment Method", Enabled = false)]
        public virtual string PaymentMethod { get; set; }
        public abstract class paymentMethod : IBqlField { }
        #endregion

        #region PriceCalculationMethod
        [PXString(30)]
        [PXUnboundDefault("本期複價=本期計價數量 * 契約單價")]
        [PXUIField(DisplayName = "Price Calculation Method", Enabled = false)]
        public virtual string PriceCalculationMethod { get; set; }
        public abstract class priceCalculationMethod : IBqlField { }
        #endregion

        #region IsPrepayments
        [PXDBBool()]
        [PXUIField(DisplayName = "Is Prepayments")]
        public virtual bool? IsPrepayments { get; set; }
        public abstract class isPrepayments : IBqlField { }
        #endregion

        #region PrepaymentRatioUntaxed
        [PXDBDecimal(6)]
        [PXDefault(TypeCode.Decimal, "0.000000")]
        [PXUIField(DisplayName = "Prepayment Ratio Untaxed",Enabled =false)]
        public virtual Decimal? PrepaymentRatioUntaxed { get; set; }
        public abstract class prepaymentRatioUntaxed : IBqlField { }
        #endregion

        #region PrepaymentsAmtUntaxed
        [PXDBDecimal(2)]
        [PXUIField(DisplayName = "Prepayments Amt Untaxed",Enabled =false)]
        public virtual Decimal? PrepaymentsAmtUntaxed { get; set; }
        public abstract class prepaymentsAmtUntaxed : IBqlField { }
        #endregion

        #region DeductionRatioPeriod
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Deduction Ratio Period")]
        [PXDefault(TypeCode.Decimal, "100.00")]
        public virtual Decimal? DeductionRatioPeriod { get; set; }
        public abstract class deductionRatioPeriod : IBqlField { }
        #endregion

        #region EstimatedAmtPeriod
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Estimated Amt Period", Visibility = PXUIVisibility.Invisible, Visible = false)]
        public virtual Decimal? EstimatedAmtPeriod { get; set; }
        public abstract class estimatedAmtPeriod : IBqlField { }
        #endregion

        #region IsPerformanceGuarantee
        [PXDBBool()]
        [PXUIField(DisplayName = "Is Performance Guarantee")]
        public virtual bool? IsPerformanceGuarantee { get; set; }
        public abstract class isPerformanceGuarantee : IBqlField { }
        #endregion

        #region IsWarranty
        [PXDBBool()]
        [PXUIField(DisplayName = "Is Warranty")]
        [PXDefault(false)]
        public virtual bool? IsWarranty { get; set; }
        public abstract class isWarranty : IBqlField { }
        #endregion

        #region WarrantyRatioUntaxed
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Warranty Ratio Untaxed",Enabled =false)]
        public virtual Decimal? WarrantyRatioUntaxed { get; set; }
        public abstract class warrantyRatioUntaxed : IBqlField { }
        #endregion

        #region WarrantyCalcuMethod
        [PXDBString(1, IsFixed = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Warranty Calcu Method", Enabled = false)]
        [PXStringList(
            new string[]
            {
        "F",
        "O"
            },
            new string[]
            {
        "依結算金額",
        "依原始合約金額"
        })]
        public virtual string WarrantyCalcuMethod { get; set; }
        public abstract class warrantyCalcuMethod : IBqlField { }
        #endregion

        //2019/07/09改成WarrantyYear(decimal(5,1))
        #region WarrantyYear
        [PXDBDecimal(1)]
        [PXUIField(DisplayName = "Warranty Year", Enabled = false)]
        public virtual Decimal? WarrantyYear { get; set; }
        public abstract class warrantyYear : IBqlField { }
        #endregion

        #region WarrantyGuaranteeCashRatio
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Warranty Guarantee Cash Ratio", Enabled = false)]
        public virtual Decimal? WarrantyGuaranteeCashRatio { get; set; }
        public abstract class warrantyGuaranteeCashRatio : IBqlField { }
        #endregion

        #region WarrantyGuaranteeTicketRatio
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Warranty Guarantee Ticket Ratio", Enabled = false)]
        public virtual Decimal? WarrantyGuaranteeTicketRatio { get; set; }
        public abstract class warrantyGuaranteeTicketRatio : IBqlField { }
        #endregion

        #region NoteID
        [PXNote()]
        [PXUIField(DisplayName = "NoteID")]
        public virtual Guid? NoteID { get; set; }
        public abstract class noteID : IBqlField { }
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

        #region Tstamp
        [PXDBTimestamp()]
        [PXUIField(DisplayName = "Tstamp")]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : IBqlField { }
        #endregion

        #region RetainageApply
        [PXDBBool()]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Null)]
        [PXUIField(DisplayName = "應用保留款")]
        public virtual bool? RetainageApply { get; set; }
        public abstract class retainageApply : IBqlField { }
        #endregion

        #region DefRetainagePct
        [PXDBDecimal(0, MinValue = 0, MaxValue = 100)]
        [PXDefault(TypeCode.Decimal, "0")]
        [PXUIField(DisplayName = "保留款百分比")]
        public virtual Decimal? DefRetainagePct { get; set; }
        public abstract class defRetainagePct : IBqlField { }
        #endregion

        //2019/07/04 ADD RadioButton
        //2019/09/19 不需要RadioButton
        /*#region WarrantyCalUntaxedRB
        [PXDBString(1)]
        [PXUIField(DisplayName = "")]
        [PXStringList(new string[] {"A","R" },
            new string[] {"","" })]
        [PXDefault("R")]
        public virtual string WarrantyCalUntaxedRB { get; set; }
        public abstract class warrantyCalUntaxedRB : IBqlField { }
        #endregion*/

        //2019/09/19 拿掉
        /*#region IsWarrantyPaymentMethod
        [PXDBBool()]
        [PXUIField(DisplayName = "履約保證轉保固保證")]
        public virtual bool? IsWarrantyPaymentMethod { get; set; }
        public abstract class isWarrantyPaymentMethod : IBqlField { }
        #endregion*/

        //2019/07/05 拿掉
        /*#region WarrantyDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Warranty Date")]
        public virtual DateTime? WarrantyDate { get; set; }
        public abstract class warrantyDate : IBqlField { }
        #endregion*/

        //2019/09/19 拿掉
        /*#region FixedAmtUntaxed
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Fixed Amt Untaxed",Enabled =false)]
        public virtual Decimal? FixedAmtUntaxed { get; set; }
        public abstract class fixedAmtUntaxed : IBqlField { }
        #endregion*/

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
		[Branch(typeof(POOrder.branchID))]
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
    }

}