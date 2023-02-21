using System;
using PX.Data;
using PX.Objects.CS;
using PX.Objects.AP;
using PX.Objects.GL;

namespace Kedge.DAC
{
  [Serializable]
  public class KGFlowSubAccVendorEvaluate : IBqlTable
  {
        #region Setup
        public abstract class setup : IBqlField
        { }
        [PXString()]
        //CS201010
        [PXDefault("KGFLOWEVA")]
        [PXUIField(DisplayName = "Setup")]
        public virtual string Setup { get; set; }
        #endregion
        #region EvaluateID
        [PXDBIdentity(IsKey = true)]
        [PXUIField(DisplayName = "Evaluate ID")]
        public virtual int? EvaluateID { get; set; }
        public abstract class evaluateID : PX.Data.BQL.BqlInt.Field<evaluateID> { }
        #endregion

        #region EvaluateUID
        [PXDBString(40, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Evaluate UID")]
        [AutoNumber(typeof(setup), typeof(AccessInfo.businessDate))]
        public virtual string EvaluateUID { get; set; }
        public abstract class evaluateUID : PX.Data.BQL.BqlString.Field<evaluateUID> { }
        #endregion

        #region Accuid
        [PXDBString(40, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "AccUID")]
        [PXDBDefault(typeof(KGFlowSubAcc.accUID))]
        [PXParent(typeof(Select<KGFlowSubAcc,
                            Where<KGFlowSubAcc.accUID,
                            Equal<Current<KGFlowSubAccDetail.accUID>>>>))]
        public virtual string AccUID { get; set; }
        public abstract class accUID : PX.Data.BQL.BqlString.Field<accUID> { }
        #endregion

        #region SubCode
        [PXDBString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Sub Code")]
        public virtual string SubCode { get; set; }
        public abstract class subCode : PX.Data.BQL.BqlString.Field<subCode> { }
        #endregion

        #region SubName
        [PXDBString(60, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Sub Name")]
        public virtual string SubName { get; set; }
        public abstract class subName : PX.Data.BQL.BqlString.Field<subName> { }
        #endregion

        #region Title
        [PXDBString(60, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Title")]
        public virtual string Title { get; set; }
        public abstract class title : PX.Data.BQL.BqlString.Field<title> { }
        #endregion

        #region EvaluationCode
        [PXDBString(22, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Evaluation Code")]
        public virtual string EvaluationCode { get; set; }
        public abstract class evaluationCode : PX.Data.BQL.BqlString.Field<evaluationCode> { }
        #endregion

        #region QuestionDocCode
        [PXDBString(20, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Question Doc Code")]
        public virtual string QuestionDocCode { get; set; }
        public abstract class questionDocCode : PX.Data.BQL.BqlString.Field<questionDocCode> { }
        #endregion

        #region TimePeriodName
        [PXDBString(200, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Time Period Name")]
        public virtual string TimePeriodName { get; set; }
        public abstract class timePeriodName : PX.Data.BQL.BqlString.Field<timePeriodName> { }
        #endregion

        #region VendorTypeName
        [PXDBString(200, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Vendor Type Name")]
        public virtual string VendorTypeName { get; set; }
        public abstract class vendorTypeName : PX.Data.BQL.BqlString.Field<vendorTypeName> { }
        #endregion

        #region EvaluateDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Evaluate Date")]
        public virtual DateTime? EvaluateDate { get; set; }
        public abstract class evaluateDate : PX.Data.BQL.BqlDateTime.Field<evaluateDate> { }
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