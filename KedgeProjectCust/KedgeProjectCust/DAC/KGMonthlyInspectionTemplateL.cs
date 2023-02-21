using System;
using PX.Data;
using PX.Objects.IN;
using Kedge.DAC;
using PX.Objects.CS;
using System.Collections;
using System.Collections.Generic;
using PX.Objects.CS;
using PX.Objects.GL;

namespace Kedge.DAC
{


    public class TemplateSegmentCDAttribute : PXStringListAttribute, IPXRowSelectedSubscriber
    {
        public TemplateSegmentCDAttribute()
            : base()
        {
     

        }
        public void RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            KGMonthlyInspectionTemplateL row = e.Row as KGMonthlyInspectionTemplateL;
            if (row == null) return;
            if (typeof(KGMonthlyInspectionTemplate).IsInstanceOfType(sender.Graph)) {
                KGMonthlyInspectionTemplate template = sender.Graph as KGMonthlyInspectionTemplate;
                HashSet<String> hashSet = new HashSet<String>();
                foreach (KGMonthlyInspectionTemplateS templateS in template.MonthlyTemplateS.Select())
                {
                    hashSet.Add(templateS.SegmentCD);
                }
                PXGraph graph = new PXGraph();
                PXResultset<SegmentValue> set = PXSelectJoin<SegmentValue,
                    InnerJoin<Segment, On<Segment.segmentID, Equal<SegmentValue.segmentID>,
                    And<Segment.dimensionID, Equal<SegmentValue.dimensionID>>>>,
                    Where<Segment.dimensionID, Equal<KGInspectionConstant.kgmninsc>,
                    And<Segment.segmentID, Equal<KGInspectionConstant.segmentIDPart2>>>>.Select(graph);
                List<String> key = new List<String>();
                List<String> value = new List<String>();
                foreach (SegmentValue segmentValue in set)
                {
                    if (hashSet.Contains(segmentValue.Value))
                    {
                        key.Add(segmentValue.Value);
                        value.Add(segmentValue.Value + "-" + segmentValue.Descr);
                    }
                }
                PXStringListAttribute.SetList<KGMonthlyInspectionTemplateL.segmentCD>(sender, row, key.ToArray(), value.ToArray());
            }
            else
            {
                HashSet<String> hashSet = new HashSet<String>();

                PXGraph graph = new PXGraph();
                PXResultset<SegmentValue> set = PXSelectJoin<SegmentValue,
                    InnerJoin<Segment, On<Segment.segmentID, Equal<SegmentValue.segmentID>,
                    And<Segment.dimensionID, Equal<SegmentValue.dimensionID>>>>,
                    Where<Segment.dimensionID, Equal<KGInspectionConstant.kgmninsc>,
                    And<Segment.segmentID, Equal<KGInspectionConstant.segmentIDPart2>>>>.Select(graph);
                List<String> key = new List<String>();
                List<String> value = new List<String>();
                foreach (SegmentValue segmentValue in set)
                {
                   key.Add(segmentValue.Value);
                }
                PXStringListAttribute.SetList<KGMonthlyInspectionTemplateL.segmentCD>(sender, row, key.ToArray(), value.ToArray());
            }
        }
    
    }


    [Serializable]
    public class KGMonthlyInspectionTemplateL : IBqlTable
    {

        #region TemplateHeaderID
        [PXDBInt()]
        [PXUIField(DisplayName = "Monthly Template Header ID")]
        [PXDBDefault(typeof(KGMonthlyInspectionTemplateH.templateHeaderID))]
        [PXParent(typeof(Select<KGMonthlyInspectionTemplateH,
                    Where<KGMonthlyInspectionTemplateH.templateHeaderID,
                    Equal<Current<templateHeaderID>>>>))]
        public virtual int? TemplateHeaderID { get; set; }
        public abstract class templateHeaderID : IBqlField { }
        #endregion


        #region TemplateLineID
        [PXDBIdentity(IsKey = true)]
        [PXUIField(DisplayName = "Template Line ID")]
        public virtual int? TemplateLineID { get; set; }
        public abstract class templateLineID : IBqlField { }
        #endregion


        #region SegmentCD
        //[PXDimension(KGMonthlyInspectionTemplateH.DimensionName)]
        [PXDBString(30, IsUnicode = true, InputMask = "", IsKey = true)]
        [PXUIField(DisplayName = "月查核類別", Required = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [TemplateSegmentCD]
        public virtual string SegmentCD { get; set; }
        public abstract class segmentCD : IBqlField { }
        #endregion
        #region SegmentDescr
        //[PXDimension(KGMonthlyInspectionTemplateH.DimensionName)]
        [PXString()]
        [PXUIField(DisplayName = "月查核類別說明", IsReadOnly = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXDBScalar(
               typeof(Search2<SegmentValue.descr,
            InnerJoin<Segment, On<Segment.segmentID, Equal<SegmentValue.segmentID>,
                And<Segment.dimensionID, Equal<SegmentValue.dimensionID>>>>,
            Where<Segment.dimensionID, Equal<KGInspectionConstant.kgmninsc>,
                And<Segment.segmentID, Equal<KGInspectionConstant.segmentIDPart2>,
                And<SegmentValue.value,Equal<KGMonthlyInspectionTemplateL.segmentCD>>>>>))]
        public virtual string SegmentDescr { get; set; }
        public abstract class segmentDescr : IBqlField { }
        #endregion
        #region SegmentCDRead
        //[PXDimension(KGMonthlyInspectionTemplateH.DimensionName)]
        [PXString(30)]
        [PXUIField(DisplayName = "月查核類別", Required = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXFormula(typeof(segmentCD))]
        public virtual string SegmentCDRead { get; set; }
        public abstract class segmentCDRead : IBqlField { }

        #endregion

        #region CheckItem
        [PXDBString(50, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Check Item", Required = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual string CheckItem { get; set; }
        public abstract class checkItem : IBqlField { }
        #endregion

        #region CheckPointDesc
        [PXDBString(50, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Check Point Desc", Required = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual string CheckPointDesc { get; set; }
        public abstract class checkPointDesc : IBqlField { }
        #endregion
        


        #region MaxNoMissing
        [PXDBInt()]
        [PXDefault(0, PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXUIField(DisplayName = "Max No Missing", Required = true)]
        public virtual int? MaxNoMissing { get; set; }
        public abstract class maxNoMissing : IBqlField { }
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