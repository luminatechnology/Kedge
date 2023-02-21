using System;
using PX.Data;
using PX.Objects.CS;

namespace Kedge.DAC
{

    [Serializable]
    public class KGSelfInspectionTemplateH : IBqlTable
    {
        public const string DimensionName = "KGSFINSC";

        #region TemplateHeaderID
        [PXDBIdentity]
        [PXUIField(DisplayName = "Template Header ID")]
        public virtual int? TemplateHeaderID { get; set; }
        public abstract class templateHeaderID : IBqlField { }
        #endregion

        #region TemplateCD
        [PXDBString(50, IsKey = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Template CD", Required = true)]
        [PXSelector(typeof(KGSelfInspectionTemplateH.templateCD),
                    typeof(KGSelfInspectionTemplateH.templateCD),
                    typeof(KGSelfInspectionTemplateH.segmentCD),
                    typeof(KGSelfInspectionTemplateH.description),
                    typeof(KGSelfInspectionTemplateH.segmentDesc),
                    typeof(KGSelfInspectionTemplateH.version)
        )]
        [AutoNumber(typeof(Search<KGSetUp.kGSelfInspectionTempNumbering>), typeof(AccessInfo.businessDate))]
        public virtual string TemplateCD { get; set; }
        public abstract class templateCD : IBqlField { }
        #endregion

        #region SegmentCD
        //[PXDimensionSelector(segmentCD.DimensionName, typeof(Search<KGSelfInspectionTemplateH.segmentCD>), typeof(KGSelfInspectionTemplateH.segmentCD))]
        //[PXDimensionSelector(KGSetUp.kGSelfInspectionSegmentKey, typeof(Search<KGSelfInspectionTemplateH.segmentCD>), typeof(KGSelfInspectionTemplateH.segmentCD))]
        [PXDimension(KGSelfInspectionTemplateH.DimensionName)]
        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXUIField(DisplayName = "Segment CD", Required = true)]
        public virtual string SegmentCD { get; set; }
        public abstract class segmentCD : IBqlField { }
        #endregion

        #region SegmentDesc
        [PXDBString(256, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Segment Desc", Enabled = false)]
        public virtual string SegmentDesc { get; set; }
        public abstract class segmentDesc : IBqlField { }
        #endregion

        #region Description
        [PXDBString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Description")]
        public virtual string Description { get; set; }
        public abstract class description : IBqlField { }
        #endregion

        #region Version
        [PXDBInt()]
        [PXUIField(DisplayName = "Version", Enabled = false, Required = true)]
        [PXDefault(1)]
        public virtual int? Version { get; set; }
        public abstract class version : IBqlField { }
        #endregion

        #region Status
        [PXDBString(1, IsFixed = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Status", Enabled = false, Required = true)]
        [PXDefault(KGSelfInspectionStatuses.Hold)]
        [KGSelfInspectionStatuses.ListAttribute4Template]
        public virtual string Status { get; set; }
        public abstract class status : IBqlField { }
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

        #region NoteID
        [PXNote()]
        [PXUIField(DisplayName = "NoteID")]
        public virtual Guid? NoteID { get; set; }
        public abstract class noteID : IBqlField { }
        #endregion

        #region Tstamp
        [PXDBTimestamp()]
        [PXUIField(DisplayName = "Tstamp")]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : IBqlField { }
        #endregion

        #region Hold
        [PXDBBool()]
        [PXUIField(DisplayName = "Hold", Required = true)]
        [PXDefault(true)]
        public virtual bool? Hold { get; set; }
        public abstract class hold : IBqlField { }
        #endregion

    }

  
}

