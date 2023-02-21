using System;
using PX.Data;

namespace Kedge.DAC
{
    [Serializable]
    public class KGVendorEvaluationQuest : IBqlTable
    {
        #region QuestID
        [PXDBIdentity(IsKey =true)]
        [PXUIField(DisplayName = "Quest ID")]
        public virtual int? QuestID { get; set; }
        public abstract class questID : PX.Data.BQL.BqlInt.Field<questID> { }
        #endregion

        #region EvaluationID
        [PXDBInt(IsKey =true)]
        [PXDBDefault(typeof(KGVendorEvaluation.evaluationID))]
        [PXParent(typeof(Select<KGVendorEvaluation,
                        Where<KGVendorEvaluation.evaluationID,
                          Equal<Current<KGVendorEvaluationQuest.evaluationID>>>>))]
        [PXUIField(DisplayName = "Evaluation ID")]
        public virtual int? EvaluationID { get; set; }
        public abstract class evaluationID : PX.Data.BQL.BqlInt.Field<evaluationID> { }
        #endregion

        #region QuestSeq
        [PXDBInt()]
        [PXUIField(DisplayName = "Quest Seq" ,IsReadOnly =true)]
        public virtual int? QuestSeq { get; set; }
        public abstract class questSeq : PX.Data.BQL.BqlInt.Field<questSeq> { }
        #endregion

        #region Quest
        [PXDBString(500, IsUnicode = true, InputMask = "")]
        [PXDBDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXUIField(DisplayName = "Quest",Required =true)]
        public virtual string Quest { get; set; }
        public abstract class quest : PX.Data.BQL.BqlString.Field<quest> { }
        #endregion

        #region Score
        [PXDBInt()]
        [PXDefault(0)]
        [PXDBDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXUIField(DisplayName = "Score",Required =true)]
        public virtual int? Score { get; set; }
        public abstract class score : PX.Data.BQL.BqlInt.Field<score> { }
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
    }
}