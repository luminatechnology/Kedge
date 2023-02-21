using System;
using PX.Data;
using PX.Objects.PO;
using PX.Objects.GL;
namespace Kedge.DAC
{
    [Serializable]
    public class KGContractSegPricing : IBqlTable
    {
        #region Selected
        [PXBool]
        [PXUIField(DisplayName = "Selected")]
        public virtual bool? Selected { get; set; }
        public abstract class selected : IBqlField { }
        #endregion

        #region SegPricingID
        [PXDBIdentity()]
        [PXDefault()]
        [PXUIField(DisplayName = "Seg Pricing ID")]
        public virtual int? SegPricingID { get; set; }
        public abstract class segPricingID : IBqlField { }
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
        [PXParent(typeof(Select<POOrder, Where<POOrder.orderType, Equal<Current<KGContractSegPricing.orderType>>, And<POOrder.orderNbr, Equal<Current<KGContractSegPricing.orderNbr>>>>>))]
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

        #region LineNbr
        [PXDBInt(IsKey =true)]
        [PXUIField(DisplayName = "Line Nbr")]
        public virtual int? LineNbr { get; set; }
        public abstract class lineNbr : IBqlField { }
        #endregion

        #region SortOrder
        [PXDBInt(IsKey =true)]
        [PXUIField(DisplayName = "Sort Order", Required = true)]
        public virtual int? SortOrder { get; set; }
        public abstract class sortOrder : IBqlField { }
        #endregion

        #region SegmentName
        [PXDBString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "階段計價名稱", Required = true)]
        public virtual string SegmentName { get; set; }
        public abstract class segmentName : IBqlField { }
        #endregion

        #region SegmentPercent
        [PXDBDecimal()]
        [PXUIField(DisplayName = "計價比率(%)", Required = true)]
        public virtual Decimal? SegmentPercent { get; set; }
        public abstract class segmentPercent : IBqlField { }
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