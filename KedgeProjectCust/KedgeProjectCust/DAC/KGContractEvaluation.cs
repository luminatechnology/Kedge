using System;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.CS;
using PX.Objects.PO;
using PX.Objects.GL;
namespace Kedge.DAC
{

    /** =======20210324 Edit By Louis=====
     * modify  FinalScore to decmail
     * 
     * 
    **/



    [PXPrimaryGraph(typeof(KGContractEvalEntry))]
    [Serializable]
    public class KGContractEvaluation : IBqlTable
    {
        #region ContractEvaluationID
        [PXDBIdentity(IsKey = true)]
        [PXUIField(DisplayName = "Contract Evaluation ID")]       
        public virtual int? ContractEvaluationID { get; set; }
        public abstract class contractEvaluationID : PX.Data.BQL.BqlInt.Field<contractEvaluationID> { }
        #endregion

        #region ContractEvaluationCD
        [PXDBString(40, IsUnicode = true, InputMask = "", IsKey = true)]
        [PXUIField(DisplayName = "Contract Evaluation", Enabled = false)]
        [AutoNumber(typeof(KGSetUp.kGContractEvalNumbering), typeof(AccessInfo.businessDate))]
        [PXSelector(typeof(contractEvaluationCD))]
        public virtual string ContractEvaluationCD { get; set; }
        public abstract class contractEvaluationCD : PX.Data.BQL.BqlString.Field<contractEvaluationCD> { }
        #endregion

        #region EvaluationID
        [PXDBInt()]
        [PXUIField(DisplayName = "Evaluation ID")]
        [PXDefault(typeof(Select<KGContractDoc,
                                 Where<KGContractDoc.orderType, Equal<Current<orderType>>,
                                       And<KGContractDoc.orderNbr, Equal<Current<orderNbr>>>>>),
                   SourceField = typeof(KGContractDoc.evaluationID))]
        public virtual int? EvaluationID { get; set; }
        public abstract class evaluationID : PX.Data.BQL.BqlInt.Field<evaluationID> { }
        #endregion

        #region OrderType
        [PXDBString(2, IsFixed = true, InputMask = "")]
        [PXUIField(DisplayName = "Order Type")]
        public virtual string OrderType { get; set; }
        public abstract class orderType : PX.Data.BQL.BqlString.Field<orderType> { }
        #endregion

        #region OrderNbr
        [PXDBString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Order Nbr.")]
        [PXSelector(typeof(POOrder.orderNbr),
                    DescriptionField = typeof(POOrder.orderDesc))]
        public virtual string OrderNbr { get; set; }
        public abstract class orderNbr : PX.Data.BQL.BqlString.Field<orderNbr> { }
        #endregion

        #region APDocType
        [PXDBString(3, IsFixed = true, InputMask = "")]
        [PXUIField(DisplayName = "AP Doc Type")]
        [PXDBDefault(typeof(APInvoice.docType))]
       
        public virtual string APDocType { get; set; }
        public abstract class aPDocType : PX.Data.BQL.BqlString.Field<aPDocType> { }
        #endregion

        #region APRefNbr
        [PXDBString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "AP Ref Nbr.")]
        [PXDBDefault(typeof(APInvoice.refNbr))]
        [PXParent(typeof(Select<APInvoice,
                                Where<APInvoice.refNbr, Equal<Current<KGContractEvaluation.aPRefNbr>>,
                                     And<APInvoice.docType, Equal<Current<KGContractEvaluation.aPDocType>>>>>))]
      
        public virtual string APRefNbr { get; set; }
        public abstract class aPRefNbr : PX.Data.BQL.BqlString.Field<aPRefNbr> { }
        #endregion

        #region EvaluationDate
        [PXDBDate()]
        [PXDefault(typeof(AccessInfo.businessDate))]
        [PXUIField(DisplayName = "Evaluation Date")]
        public virtual DateTime? EvaluationDate { get; set; }
        public abstract class evaluationDate : IBqlField { }
        #endregion

        #region Score
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Total Score", Enabled = false)]
        public virtual decimal? Score { get; set; }
        public abstract class score : PX.Data.BQL.BqlDecimal.Field<score> { }
        #endregion

        #region WeightingScore
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Weighting Score")]
        public virtual decimal? WeightingScore { get; set; }
        public abstract class weightingScore : PX.Data.BQL.BqlDecimal.Field<weightingScore> { }
        #endregion

        #region VendorID
        [POVendor()]
        public virtual int? VendorID { get; set; }
        public abstract class vendorID : PX.Data.BQL.BqlInt.Field<vendorID> { }
        #endregion

        #region EvalPhase
        [PXDBInt()]
        [PXUIField(DisplayName = "Evaluation Phase",IsReadOnly =true)]
   
        public virtual int? EvalPhase { get; set; }
        public abstract class evalPhase : PX.Data.BQL.BqlInt.Field<evalPhase> { }
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

        #region Tstamp
        [PXDBTimestamp()]
        [PXUIField(DisplayName = "Tstamp")]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
        #endregion

        #region EvaluationName
        [PXString(240, IsUnicode = true)]
        [PXDBScalar(typeof(Search<KGVendorEvaluation.evaluationName,
                                  Where<KGVendorEvaluation.evaluationID, Equal<evaluationID>>>))]
        [PXUIField(DisplayName = "Evaluation Name", IsReadOnly = true)]
        public virtual string EvaluationName { get; set; }
        public abstract class evaluationName : PX.Data.BQL.BqlString.Field<evaluationName> { }
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
    }
}