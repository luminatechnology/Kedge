using System;
using PX.Data;
using PX.Data.BQL;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.PO;
using PX.Objects.GL;

namespace Kedge.DAC
{
    #region AccountAttribute
    public static class AccountTypeAttributes
    {
        public const string VE = "VE";

        public class ve : BqlString.Constant<ve>
        {
            public ve() : base(VE) { }
        }
    }
    public static class AccountStatusAttributes
    {
        public const string A = "A";

        public class a : BqlString.Constant<a>
        {
            public a() : base(A) { }
        }
    }
    #endregion

    [Serializable]
    public class KGContractRelatedVendor : IBqlTable
    {

        #region Selected
        [PXBool]
        [PXUIField(DisplayName = "Selected")]
        public virtual bool? Selected { get; set; }
        public abstract class selected : IBqlField { }
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
            Where<POOrder.orderType, Equal<Current<KGContractRelatedVendor.orderType>>,
                And<POOrder.orderNbr, Equal<Current<KGContractRelatedVendor.orderNbr>>>>>))]
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

        #region RelatedVendorID
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Related Vendor ID", Required = true)]
        [PXSelector(typeof(Search2<BAccount.bAccountID,
            InnerJoin<Location, On<Location.bAccountID, Equal<BAccount.bAccountID>>>,
            Where<BAccount.type, Equal<AccountTypeAttributes.ve>,
                And<BAccount.status, Equal<AccountStatusAttributes.a>,
                And<Location.isActive, Equal<True>>>>,
            OrderBy<Asc<BAccount.acctCD>>>),
            typeof(BAccount.bAccountID),
            typeof(BAccount.acctName),
            typeof(BAccount.taxRegistrationID),
            typeof(BAccount.status),
            typeof(BAccount.acctCD),
            SubstituteKey = typeof(BAccount.acctCD),
            DescriptionField = typeof(BAccount.acctName)
        )]
        public virtual int? RelatedVendorID { get; set; }
        public abstract class relatedVendorID : IBqlField { }
        #endregion

        #region RelatedType
        [PXDBString(1, IsFixed = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Related Type", Required = true)]
        [PXDefault()]
        [PXStringList(
          new string[]
          {
          "R",
          "G"
          },
          new string[]
          {
          "相關",
          "保証"
        })]
        public virtual string RelatedType { get; set; }
        public abstract class relatedType : IBqlField { }
        #endregion

        #region RelatedVendorName
        [PXString(60, IsUnicode = true)]
        [PXUIField(DisplayName = "Related Vendor Name", Enabled = false)]
        public virtual string RelatedVendorName { get; set; }
        public abstract class relatedVendorName : IBqlField { }
        #endregion

        #region RelatedVendorTaxRegistration
        [PXString(50, IsFixed = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Related Vendor Tax Registration", IsReadOnly = true, Enabled = false)]
        public virtual string RelatedVendorTaxRegistration { get; set; }
        public abstract class relatedVendorTaxRegistration : IBqlField { }
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