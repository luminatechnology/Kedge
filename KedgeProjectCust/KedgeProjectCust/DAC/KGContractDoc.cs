using System;
using PX.Data;
using PX.Objects.PM;
using PX.Objects.PO;
using PX.Objects.GL;

namespace Kedge.DAC
{
    [Serializable]
    public class KGContractDoc : IBqlTable
    {

        #region ContractDocID
        [PXDBIdentity()]
        [PXUIField(DisplayName = "Contract Doc ID")]
        public virtual int? ContractDocID { get; set; }
        public abstract class contractDocID : IBqlField { }
        #endregion

        #region ContractDocCD
        /*[PXDBString(50, IsUnicode = true, InputMask = "", IsKey = true)]
        [PXSelector(typeof(contractDocCD))]
        [PXUIField(DisplayName = "Contract Doc CD", Required = true)]
        public virtual string ContractDocCD { get; set; }
        public abstract class contractDocCD : IBqlField { }
        */
        #endregion

        #region OrderClass
        [PXDBString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "OrderClass", IsReadOnly = true)]
        [PXDefault("A", PersistingCheck = PXPersistingCheck.Nothing)]
        [ReqClass.List]
        public virtual string OrderClass { get; set; }
        public abstract class orderClass : PX.Data.BQL.BqlString.Field<orderClass> { }
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

        #region ContractCategoryID
        [PXDBInt()]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXUIField(DisplayName = "Contract Category ID", Required = true)]
        [PXSelector(typeof(KGContractCategory.contractCategoryID),
        typeof(KGContractCategory.contractCategoryCD),
        typeof(KGContractCategory.contractType),
        SubstituteKey = typeof(KGContractCategory.contractCategoryID),
        DescriptionField = typeof(KGContractCategory.contractCategoryCD)
    )]
        public virtual int? ContractCategoryID { get; set; }
        public abstract class contractCategoryID : IBqlField { }
        #endregion

        #region ContractDesc
        [PXDBString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Contract Desc")]
        public virtual string ContractDesc { get; set; }
        public abstract class contractDesc : IBqlField { }
        #endregion

        #region ContractDocDate
        [PXDBDate()]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXUIField(DisplayName = "Contract Doc Date", Required = true)]
        public virtual DateTime? ContractDocDate { get; set; }
        public abstract class contractDocDate : IBqlField { }
        #endregion

        #region ExpectStartDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Expect Start Date")]
        public virtual DateTime? ExpectStartDate { get; set; }
        public abstract class expectStartDate : IBqlField { }
        #endregion

        #region ExpectEndDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Expect End Date")]
        public virtual DateTime? ExpectEndDate { get; set; }
        public abstract class expectEndDate : IBqlField { }
        #endregion

        #region ActualStartDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Actual Start Date", Required = false, Visibility = PXUIVisibility.Invisible, Visible = false)]
        public virtual DateTime? ActualStartDate { get; set; }
        public abstract class actualStartDate : IBqlField { }
        #endregion

        #region ExpectDays
        [PXDBInt()]
        [PXUIField(DisplayName = "Expect Days", Required = false, Visibility = PXUIVisibility.Invisible, Visible = false)]
        public virtual int? ExpectDays { get; set; }
        public abstract class expectDays : IBqlField { }
        #endregion

        #region RemainingDays
        [PXDBInt()]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Remaining Days", Required = false, Visibility = PXUIVisibility.Invisible, Visible = false)]
        public virtual int? RemainingDays { get; set; }
        public abstract class remainingDays : IBqlField { }
        #endregion

        #region EvaluationScore
        [PXDBInt(MinValue = 0 ,MaxValue =100)]
        //[PXDefault(90, PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXDefault(typeof(KGSetUp.kGDefEvaluationPct))]
        [PXUIField(DisplayName = "Evaluation Score", Required = true)]
        public virtual int? EvaluationScore { get; set; }
        public abstract class evaluationScore : IBqlField { }
        #endregion

        #region SecEvaluationScore
        [PXDBInt(MinValue =1, MaxValue =100)]
        [PXDefault(typeof(KGSetUp.kGDefSecEvaluationPct))]
        [PXUIField(DisplayName = "Second Evaluation Score", Required = true)]
        public virtual int? SecEvaluationScore { get; set; }
        public abstract class secEvaluationScore : IBqlField { }
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

        //2019/07/11 ADD 工地地址
        #region SiteAddress
        [PXDBString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Site Address")]
        public virtual string SiteAddress { get; set; }
        public abstract class siteAddress : IBqlField { }
        #endregion

        #region EvaluationID
        [PXDBInt()]
        [PXUIField(DisplayName = "Evaluation ID")]
        [PXDefault(typeof(KGSetUp.kGDefEvaluationID))]
        [PXSelector(typeof(Search<KGVendorEvaluation.evaluationID,
                                  Where<KGVendorEvaluation.hold, Equal<False>>,
                                  OrderBy<Asc<KGVendorEvaluation.evaluationCD>>>),
                    typeof(KGVendorEvaluation.evaluationCD),
                    typeof(KGVendorEvaluation.evaluationName),
                    SubstituteKey = typeof(KGVendorEvaluation.evaluationCD),
                    DescriptionField = typeof(KGVendorEvaluation.evaluationName))]
        public virtual int? EvaluationID { get; set; }
        public abstract class evaluationID : IBqlField { }
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

        #region unbound

        #region TotalAmt
        //契約工程總價(未稅)
        [PXDecimal()]
        [PXUIField(DisplayName = "Total Amount", IsReadOnly = true)]
        public virtual decimal? TotalAmt { get; set; }
        public abstract class totalAmt : IBqlField { }

        #endregion

        #region TotalTaxAmt
        //契約工程總價(營業稅)
        [PXDecimal()]
        [PXUIField(DisplayName = "Total Tax Amount", IsReadOnly = true)]
        public virtual decimal? TotalTaxAmt { get; set; }
        public abstract class totalTaxAmt : IBqlField { }
        #endregion

        #endregion
    }
}