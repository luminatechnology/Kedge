using System;
using PX.Data;
using PX.Objects.CS;
using PX.Objects.FS;
using PX.Objects.CS;
using PX.Objects.GL;

namespace Kedge.DAC
{
    [Serializable]
    public class KGMonthlyInspectionTemplateS : IBqlTable
    {

        #region TemplateHeaderID
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Monthly Template Header ID")]
        [PXDBDefault(typeof(KGMonthlyInspectionTemplateH.templateHeaderID))]
        [PXParent(typeof(Select<KGMonthlyInspectionTemplateH,
                    Where<KGMonthlyInspectionTemplateH.templateHeaderID,
                    Equal<Current<KGMonthlyInspectionTemplateS.templateHeaderID>>>>))]
        public virtual int? TemplateHeaderID { get; set; }
        public abstract class templateHeaderID : IBqlField { }
        #endregion
        #region SegmentCD
        //[PXDimension(KGMonthlyInspectionTemplateH.DimensionName)]
        [PXDBString(30, IsUnicode = true, InputMask = "", IsKey = true)]
        [PXUIField(DisplayName = "月查核類別", Required = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        /*
        [PXSelector(typeof(Search2<SegmentValue.value,
            InnerJoin<Segment, On<Segment.segmentID, Equal<SegmentValue.segmentID>,
                And<Segment.dimensionID, Equal<SegmentValue.dimensionID>>>>,
            Where<Segment.dimensionID, Equal<KGInspectionConstant.kgmninsc>,
                And<Segment.segmentID, Equal<KGInspectionConstant.segmentIDPart2>,
                    And<SegmentValue.active, Equal<True>>>>>),
            typeof(SegmentValue.value),
            typeof(SegmentValue.descr), DescriptionField= typeof(SegmentValue.descr)
        )]*/
        [PXSelectorWithCustomOrderBy(typeof(Search2<SegmentValue.value,
            InnerJoin<Segment, On<Segment.segmentID, Equal<SegmentValue.segmentID>,
                And<Segment.dimensionID, Equal<SegmentValue.dimensionID>>>>,
            Where<Segment.dimensionID, Equal<KGInspectionConstant.kgmninsc>,
                And<Segment.segmentID, Equal<KGInspectionConstant.segmentIDPart2>,
                    And<SegmentValue.active, Equal<True>>>>,
            OrderBy<Asc<SegmentValue.value>>>),
            typeof(SegmentValue.value),
            typeof(SegmentValue.descr), DescriptionField = typeof(SegmentValue.descr))]

        public virtual string SegmentCD { get; set; }
        public abstract class segmentCD : IBqlField { }

        #endregion

        #region SegmentDesc
        [PXDBString(256, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Monthly Segment Desc", Enabled = false)]
        [PXDefault(
            typeof(Search<SegmentValue.descr,
                Where<SegmentValue.value, Equal<Current<KGMonthlyInspectionTemplateS.segmentCD>>,
                    And<SegmentValue.dimensionID, Equal<KGInspectionConstant.kgmninsc>>>>
                )
        )]
        public virtual string SegmentDesc { get; set; }
        public abstract class segmentDesc : IBqlField { }
        #endregion

        #region ScoreSetup
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Score Setup", Required = true)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        public virtual Decimal? ScoreSetup { get; set; }
        public abstract class scoreSetup : IBqlField { }
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
		[Branch(typeof(KGMonthlyInspectionTemplateH.branchID))]
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