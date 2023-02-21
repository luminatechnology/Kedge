using System;
using PX.Data;
using PX.Objects.CS;

namespace Kedge.DAC
{
    [Serializable]
    public class KGSafetyHealthInspectionTemplateH : IBqlTable
    {
        #region TemplateHeaderID
        [PXDBIdentity]
        [PXUIField(DisplayName = "Safety Health Template Header ID")]
        public virtual int? TemplateHeaderID { get; set; }
        public abstract class templateHeaderID : PX.Data.BQL.BqlInt.Field<templateHeaderID> { }
        #endregion

        #region TemplateCD
        [PXDBString(50, IsKey = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Safety Health Template CD")]
        [PXSelector(typeof(Search<templateCD>),
                    typeof(templateCD),
                    typeof(segmentCD)
        )]
        [AutoNumber(typeof(Search<KGSetUp.kGSafetyHealthInspectionTempNumbering>), typeof(AccessInfo.businessDate))]
        public virtual string TemplateCD { get; set; }
        public abstract class templateCD : PX.Data.BQL.BqlString.Field<templateCD> { }
        #endregion

        #region SegmentCD
        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Safety Health Segment CD")]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXSelector(typeof(Search2<SegmentValue.value,
            InnerJoin<Segment, On<Segment.segmentID, Equal<SegmentValue.segmentID>,
                And<Segment.dimensionID, Equal<SegmentValue.dimensionID>>>>,
            Where<Segment.dimensionID, Equal<KGInspectionConstant.kgshinsc>,
                And<Segment.segmentID, Equal<KGInspectionConstant.shin_segmentIDPart1>,
                And<SegmentValue.active,Equal<True>>>>>),
            typeof(SegmentValue.value),
            typeof(SegmentValue.descr),
            DescriptionField =typeof(SegmentValue.descr)
        )]
        public virtual string SegmentCD { get; set; }
        public abstract class segmentCD : PX.Data.BQL.BqlString.Field<segmentCD> { }
        #endregion

        //2019/11/13 ®³±¼
       /*#region SegmentDesc
        [PXString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Safety Health Segment Desc")]      
        public virtual string SegmentDesc { get; set; }
        public abstract class segmentDesc : PX.Data.BQL.BqlString.Field<segmentDesc> { }
        #endregion*/

        #region Description
        [PXDBString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Safety Health Description")]
        public virtual string Description { get; set; }
        public abstract class description : PX.Data.BQL.BqlString.Field<description> { }
        #endregion

        #region Version
        [PXDBInt()]
        [PXUIField(DisplayName = "Safety Health Version")]
        public virtual int? Version { get; set; }
        public abstract class version : PX.Data.BQL.BqlInt.Field<version> { }
        #endregion

        #region Hold
        [PXDBBool()]
        [PXUIField(DisplayName = "Hold")]
        [PXDefault(true)]
        public virtual bool? Hold { get; set; }
        public abstract class hold : PX.Data.BQL.BqlBool.Field<hold> { }
        #endregion

        #region Status
        [PXDBString(1, IsFixed = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Status")]
        [PXDefault("H")]
        [PXStringList(
            new string[]
            {
                "H","N",
                "C"
            },
            new string[]
            {
                "Hold","Open",
                "Close"
            }
        )]
        public virtual string Status { get; set; }
        public abstract class status : PX.Data.BQL.BqlString.Field<status> { }
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

        #region SegmentDesc
        [PXString(256, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Safety Health Segment Desc", Enabled = false)]
        [PXDBScalar(typeof(Search<SegmentValue.descr,
                Where<SegmentValue.value, Equal<KGSafetyHealthInspectionTemplateH.segmentCD>,
                    And<SegmentValue.dimensionID, Equal<KGInspectionConstant.kgshinsc>>>>
                ))]
        public virtual string SegmentDesc { get; set; }
        public abstract class segmentDesc : IBqlField { }
        #endregion 
    }
}