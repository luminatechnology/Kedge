using System;
using PX.Data;
using PX.Objects.CS;
using PX.Objects.AP;
using PX.Objects.GL;
namespace Kedge.DAC
{
  [Serializable]
  public class KGFlowSubAccVendorEvaluateDetail : IBqlTable
  {
        #region Setup
        public abstract class setup : IBqlField
        { }
        [PXString()]
        //CS201010
        [PXDefault("KGFLOWEVD")]
        [PXUIField(DisplayName = "Setup")]
        public virtual string Setup { get; set; }
        #endregion

        #region EvaluateDetailID
        [PXUIField(DisplayName = "Evaluate Detail ID")]
        [PXDBIdentity(IsKey = true)]
        public virtual int? EvaluateDetailID { get; set; }
        public abstract class evaluateDetailID : PX.Data.BQL.BqlInt.Field<evaluateDetailID> { }
        #endregion

        #region EvaluateDetailUID
        [PXDBString(40, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Evaluate Detail UID")]
        [AutoNumber(typeof(setup), typeof(AccessInfo.businessDate))]
        public virtual string EvaluateDetailUID { get; set; }
        public abstract class evaluateDetailUID : PX.Data.BQL.BqlString.Field<evaluateDetailUID> { }
        #endregion

        #region AccUID
        [PXDBString(40, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "AccUID")]
        [PXDBDefault(typeof(KGFlowSubAccVendorEvaluate.accUID))]
        
        public virtual string AccUID { get; set; }
        public abstract class accUID : PX.Data.BQL.BqlString.Field<accUID> { }
        #endregion

        #region EvaluateUID
        [PXDBString(40, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Evaluate UID")]
        [PXParent(typeof(Select<KGFlowSubAccVendorEvaluate,
                            Where<KGFlowSubAccVendorEvaluate.evaluateUID,
                            Equal<Current<KGFlowSubAccVendorEvaluateDetail.evaluateUID>>>>))]
        [PXDBDefault(typeof(KGFlowSubAccVendorEvaluate.evaluateUID))]
        public virtual string EvaluateUID { get; set; }
        public abstract class evaluateUID : PX.Data.BQL.BqlString.Field<evaluateUID> { }
        #endregion

        #region QuestionNo
        [PXDBInt()]
        [PXUIField(DisplayName = "Question No")]
        public virtual int? QuestionNo { get; set; }
        public abstract class questionNo : PX.Data.BQL.BqlInt.Field<questionNo> { }
        #endregion

        #region Content
        [PXDBString(256, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Content")]
        public virtual string Content { get; set; }
        public abstract class content : PX.Data.BQL.BqlString.Field<content> { }
        #endregion

        #region Notes
        [PXDBString(512, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Notes")]
        public virtual string Notes { get; set; }
        public abstract class notes : PX.Data.BQL.BqlString.Field<notes> { }
        #endregion

        #region Score
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Score")]
        public virtual Decimal? Score { get; set; }
        public abstract class score : PX.Data.BQL.BqlDecimal.Field<score> { }
        #endregion

        #region Memo
        [PXDBString(512, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Memo")]
        public virtual string Memo { get; set; }
        public abstract class memo : PX.Data.BQL.BqlString.Field<memo> { }
        #endregion

        #region Weight
        [PXDBInt()]
        [PXUIField(DisplayName = "Weight")]
        public virtual int? Weight { get; set; }
        public abstract class weight : PX.Data.BQL.BqlInt.Field<weight> { }
        #endregion

        #region WeightScore
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Weight Score")]
        public virtual Decimal? WeightScore { get; set; }
        public abstract class weightScore : PX.Data.BQL.BqlDecimal.Field<weightScore> { }
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
		[Branch(typeof(APRegister.branchID))]
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