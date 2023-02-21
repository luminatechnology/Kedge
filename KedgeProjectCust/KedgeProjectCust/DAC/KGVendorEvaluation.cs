using System;
using PX.Data;
using PX.Objects.CS;

namespace Kedge.DAC
{
    [Serializable]
    public class KGVendorEvaluation : IBqlTable
    {
        #region EvaluationID
        [PXDBIdentity(IsKey = true)]
        [PXUIField(DisplayName = "Evaluation ID")]
        public virtual int? EvaluationID { get; set; }
        public abstract class evaluationID : PX.Data.BQL.BqlInt.Field<evaluationID> { }
        #endregion

        #region EvaluationCD
        [PXDBString(40, IsUnicode = true, InputMask = "", IsKey = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXSelector(typeof(evaluationCD),
                    typeof(evaluationCD),
                    typeof(evaluationName))]
        [AutoNumber(typeof(Search<KGSetUp.kGEvaluationNumbering>), typeof(AccessInfo.businessDate))]
        [PXUIField(DisplayName = "Evaluation CD", Required = true)]
        public virtual string EvaluationCD { get; set; }
        public abstract class evaluationCD : PX.Data.BQL.BqlString.Field<evaluationCD> { }
        #endregion

        #region EvaluationName
        [PXDBString(240, IsUnicode = true, InputMask = "")]
        [PXDefault(PersistingCheck= PXPersistingCheck.NullOrBlank)]
        [PXUIField(DisplayName = "Evaluation Name", Required = true)]
        public virtual string EvaluationName { get; set; }
        public abstract class evaluationName : PX.Data.BQL.BqlString.Field<evaluationName> { }
        #endregion

        #region Status
        [PXDBString(1, IsFixed = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Status", Enabled = false)]
        [PXDefault("H")]
        [PXStringList(
            new string[] {"H","N"
            },
            new string[] {"Hold","Open"
            })]
        public virtual string Status { get; set; }
        public abstract class status : PX.Data.BQL.BqlString.Field<status> { }
        #endregion

        #region Hold
        [PXDBBool()]
        [PXUIField(DisplayName = "Hold", Enabled = true)]
        [PXDefault(true)]
        public virtual bool? Hold { get; set; }
        public abstract class hold : IBqlField { }
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