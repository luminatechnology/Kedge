using System;
using PX.Data;
using PX.Objects.CS;

namespace Kedge.DAC
{
    [Serializable]
    public class KGSafetyHealthInspectionTemplateS : IBqlTable
    {
        #region TemplateHeaderID
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Safety Health Template Header ID")]
        [PXDBDefault(typeof(KGSafetyHealthInspectionTemplateH.templateHeaderID))]
        [PXParent(typeof(Select<KGSafetyHealthInspectionTemplateH,
                    Where<KGSafetyHealthInspectionTemplateH.templateHeaderID,
                    Equal<Current<templateHeaderID>>>>))]
        public virtual int? TemplateHeaderID { get; set; }
        public abstract class templateHeaderID : PX.Data.BQL.BqlInt.Field<templateHeaderID> { }
        #endregion

        #region SegmentCD
        [PXDBString(30, IsKey = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Safety Health Segment CD")]
        [PXSelector(typeof(Search2<SegmentValue.value,
            InnerJoin<Segment, On<Segment.segmentID, Equal<SegmentValue.segmentID>,
                And<Segment.dimensionID, Equal<SegmentValue.dimensionID>>>>,
            Where<Segment.dimensionID, Equal<KGInspectionConstant.kgshinsc>,
                And<Segment.segmentID, Equal<KGInspectionConstant.shin_segmentIDPart2>,
                    And<SegmentValue.active, Equal<True>>>>>),
            typeof(SegmentValue.value),
            typeof(SegmentValue.descr)
        )]
        public virtual string SegmentCD { get; set; }
        public abstract class segmentCD : PX.Data.BQL.BqlString.Field<segmentCD> { }
        #endregion

        #region SegmentDesc
        [PXString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Safety Health Segment Desc", Enabled =false)]     
        public virtual string SegmentDesc { get; set; }
        public abstract class segmentDesc : PX.Data.BQL.BqlString.Field<segmentDesc> { }
        #endregion

        #region ScoreSetup
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Score Setup")]
        [PXDefault(TypeCode.Decimal, "0.0")]
        public virtual Decimal? ScoreSetup { get; set; }
        public abstract class scoreSetup : PX.Data.BQL.BqlDecimal.Field<scoreSetup> { }
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

        #region Noteid
        [PXNote()]
        [PXUIField(DisplayName = "Noteid")]
        public virtual Guid? Noteid { get; set; }
        public abstract class noteid : PX.Data.BQL.BqlGuid.Field<noteid> { }
        #endregion

        #region Tstamp
        [PXDBTimestamp()]
        [PXUIField(DisplayName = "Tstamp")]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
        #endregion
    }
}