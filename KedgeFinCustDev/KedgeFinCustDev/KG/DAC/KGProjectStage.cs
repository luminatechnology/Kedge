using System;
using PX.Data;
using PX.Objects.PM;

namespace Kedge.DAC
{
    [Serializable]
    public class KGProjectStage : IBqlTable
    {
        #region ProjectStageID
        [PXDBIdentity()]
        [PXUIField(DisplayName = "Project Stage ID")]
        public virtual int? ProjectStageID { get; set; }
        public abstract class projectStageID : PX.Data.BQL.BqlInt.Field<projectStageID> { }
        #endregion

        #region ProjectID
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Project ID")]
        [PXDBDefault(typeof(PMProject.contractID))]
        [PXParent(typeof(Select<PMProject,
                                    Where<PMProject.contractID, Equal<Current<projectID>>>>))]
        public virtual int? ProjectID { get; set; }
        public abstract class projectID : PX.Data.BQL.BqlInt.Field<projectID> { }
        #endregion

        #region StageOneDate
        [PXDBDate()]
        [PXUIField(DisplayName = "StageOneDate")]
        public virtual DateTime? StageOneDate { get; set; }
        public abstract class stageOneDate : PX.Data.BQL.BqlDateTime.Field<stageOneDate> { }
        #endregion

        #region StageOneDesc
        [PXDBString(60, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "StageOne")]
        [PXDefault("Stage One")]
        public virtual string StageOneDesc { get; set; }
        public abstract class stageOneDesc : PX.Data.BQL.BqlString.Field<stageOneDesc> { }
        #endregion

        #region StageTwoDate
        [PXDBDate()]
        [PXUIField(DisplayName = "StageTwoDate")]

        public virtual DateTime? StageTwoDate { get; set; }
        public abstract class stageTwoDate : PX.Data.BQL.BqlDateTime.Field<stageTwoDate> { }
        #endregion

        #region StageTwoDesc
        [PXDBString(60, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "StageTwo")]
        [PXDefault("Stage Two")]
        public virtual string StageTwoDesc { get; set; }
        public abstract class stageTwoDesc : PX.Data.BQL.BqlString.Field<stageTwoDesc> { }
        #endregion

        #region StageThreeDate
        [PXDBDate()]
        [PXUIField(DisplayName = "StageThreeDate")]
        public virtual DateTime? StageThreeDate { get; set; }
        public abstract class stageThreeDate : PX.Data.BQL.BqlDateTime.Field<stageThreeDate> { }
        #endregion

        #region StageThreeDesc
        [PXDBString(60, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "StageThree")]
        [PXDefault("Stage Three")]
        public virtual string StageThreeDesc { get; set; }
        public abstract class stageThreeDesc : PX.Data.BQL.BqlString.Field<stageThreeDesc> { }
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

        #region NoteId
        [PXNote()]
        [PXUIField(DisplayName = "NoteId")]
        public virtual Guid? NoteId { get; set; }
        public abstract class noteId : PX.Data.BQL.BqlGuid.Field<noteId> { }
        #endregion

        #region Tstamp
        [PXDBTimestamp()]
        [PXUIField(DisplayName = "Tstamp")]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
        #endregion
    }
}