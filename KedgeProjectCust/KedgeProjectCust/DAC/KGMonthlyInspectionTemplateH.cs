using System;
using PX.Data;
using PX.Data.BQL;
using PX.Objects.CS;
using PX.Objects.CS;
using PX.Objects.GL;

namespace Kedge.DAC
{
    [Serializable]
    public class KGMonthlyInspectionTemplateH : IBqlTable
    {

        #region TemplateHeaderID
        [PXDBIdentity]
        [PXUIField(DisplayName = "Monthly Template Header ID")]
        public virtual int? TemplateHeaderID { get; set; }
        public abstract class templateHeaderID : IBqlField { }
        #endregion

        #region TemplateCD
        [PXDefault()]
        [PXDBString(50, IsKey = true, IsUnicode = true, InputMask = ">CCCCCCCCCC")]
        [PXUIField(DisplayName = "Monthly Template CD", Required = true)]
        [PXSelector(typeof(Search<KGMonthlyInspectionTemplateH.templateCD>),
                    typeof(KGMonthlyInspectionTemplateH.templateCD),
                    typeof(KGMonthlyInspectionTemplateH.segmentCD)
        )]
        [AutoNumber(typeof(Search<KGSetUp.kGMonthlyInspectionTempNumbering>), typeof(AccessInfo.businessDate))]
        public virtual string TemplateCD { get; set; }
        public abstract class templateCD : IBqlField { }
        #endregion

        #region SegmentCD
        //[PXDimension(KGMonthlyInspectionTemplateH.DimensionName)]
        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "月查核類別", Required = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXSelector(typeof(Search2<SegmentValue.value,
            InnerJoin<Segment, On<Segment.segmentID, Equal<SegmentValue.segmentID>,
                And<Segment.dimensionID, Equal<SegmentValue.dimensionID>>>>,
            Where<Segment.dimensionID, Equal<KGInspectionConstant.kgmninsc>,
                And<Segment.segmentID, Equal<KGInspectionConstant.segmentIDPart1>,
                    And<SegmentValue.active,Equal<True>>>>>),
            typeof(SegmentValue.value),
            typeof(SegmentValue.descr),
            DescriptionField = typeof(SegmentValue.descr)
        )]
        public virtual string SegmentCD { get; set; }
        public abstract class segmentCD : IBqlField { }
        #endregion

        
        #region SegmentDesc
        [PXString(256, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Monthly Segment Desc", Enabled = false)]
        [PXDBScalar(typeof(Search<SegmentValue.descr,
                Where<SegmentValue.value, Equal<KGMonthlyInspectionTemplateH.segmentCD>,
                    And<SegmentValue.dimensionID, Equal<KGInspectionConstant.kgmninsc>>>>
                ))]
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
        
        public virtual int? Version { get; set; }
        public abstract class version : IBqlField { }
        #endregion

        
        #region Status
        [PXDBString(1, IsFixed = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Status", Enabled = false)]
        [PXDefault(KGMonthInspectionStatuses.Hold)]
        [KGMonthInspectionStatuses.ShortList()]

        public virtual string Status { get; set; }
        public abstract class status : PX.Data.BQL.BqlString.Field<status> { }
        #endregion

        #region Hold
        [PXDBBool()]
        [PXUIField(DisplayName = "Hold", Visibility = PXUIVisibility.Visible)]
        [PXDefault(true)]
        public virtual Boolean? Hold { get; set; }
        public abstract class hold : IBqlField { }
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

        /*#region BranchID
        public abstract class branchID : PX.Data.BQL.BqlInt.Field<branchID> { }
        protected Int32? _BranchID;

        /// <summary>
        /// Identifier of the <see cref="PX.Objects.GL.Branch">Branch</see>, to which the document belongs.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="PX.Objects.GL.Branch.BranchID">Branch.BranchID</see> field.
        /// </value>
        [PX.Objects.GL.Branch()]
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