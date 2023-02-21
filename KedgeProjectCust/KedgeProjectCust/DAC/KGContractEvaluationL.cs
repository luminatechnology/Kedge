using System;
using PX.Data;
using PX.Objects.FS;
using PX.Objects.AP;
using PX.Objects.GL;
namespace Kedge.DAC
{
    [Serializable]
    public class KGContractEvaluationL : IBqlTable
    {
        #region ContractEvaluationQuestID
        [PXDBIdentity(IsKey = true)]
        [PXUIField(DisplayName = "Contract Evaluation Quest ID")]
        public virtual int? ContractEvaluationQuestID { get; set; }
        public abstract class contractEvaluationQuestID : PX.Data.BQL.BqlInt.Field<contractEvaluationQuestID> { }
        #endregion

        #region ContractEvaluationID
        [PXDBInt()]
        [PXUIField(DisplayName = "Contract Evaluation ID")]
        [PXDBDefault(typeof(KGContractEvaluation.contractEvaluationID))]
        [PXParent(typeof(Select<KGContractEvaluation,
                                Where<KGContractEvaluation.contractEvaluationID, Equal<Current<KGContractEvaluationL.contractEvaluationID>>>>))]
        public virtual int? ContractEvaluationID { get; set; }
        public abstract class contractEvaluationID : PX.Data.BQL.BqlInt.Field<contractEvaluationID> { }
        #endregion

        #region QuestSeq
        [PXDBInt()]
        [PXUIField(DisplayName = "Question Seq", IsReadOnly = true)]
        public virtual int? QuestSeq { get; set; }
        public abstract class questSeq : PX.Data.BQL.BqlInt.Field<questSeq> { }
        #endregion

        #region Quest
        [PXDBString(500, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Question", IsReadOnly = true)]
        public virtual string Quest { get; set; }
        public abstract class quest : PX.Data.BQL.BqlString.Field<quest> { }
        #endregion

        #region Score
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Score")]
        public virtual decimal? Score { get; set; }
        public abstract class score : PX.Data.BQL.BqlDecimal.Field<score> { }
        #endregion

        #region WeightScore
        [PXDBInt()]
        [PXUIField(DisplayName = "Weight Score", IsReadOnly = true)]
        public virtual int? WeightScore { get; set; }
        public abstract class weightScore : PX.Data.BQL.BqlInt.Field<weightScore> { }
        #endregion

        #region FinalScore
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Final Score", Enabled = false)]
        [PXFormula(typeof(Div<Mult<score, weightScore>, SharedClasses.decimal_100>))]
        public virtual decimal? FinalScore { get; set; }
        public abstract class finalScore : PX.Data.BQL.BqlDecimal.Field<finalScore> { }
        #endregion

        #region Remark
        [PXDBString(500, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Remark")]
        public virtual string Remark { get; set; }
        public abstract class remark : PX.Data.BQL.BqlString.Field<remark> { }
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