using System;
using PX.Data;
using PX.Objects.IN;
using PX.Objects.CS;
using PX.Data.BQL;
using PX.Objects.GL;

namespace Kedge.DAC
{
    [Serializable]
    public class KGMonthlyInspectionL : IBqlTable
    {
        #region Selected
        [PXBool]
        [PXUIField(DisplayName = "Selected")]
        public virtual bool? Selected { get; set; }
        public abstract class selected : IBqlField { }
        #endregion

        #region MonthlyInspectionID
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Monthly Inspection ID", Enabled = false, Required = true)]
        [PXDBDefault(typeof(KGMonthlyInspection.monthlyInspectionID))]
        [PXParent(typeof(Select<KGMonthlyInspection,
                            Where<KGMonthlyInspection.monthlyInspectionID,
                              Equal<Current<KGMonthlyInspectionL.monthlyInspectionID>>>>))]
        public virtual int? MonthlyInspectionID { get; set; }
        public abstract class monthlyInspectionID : IBqlField { }
        #endregion

        #region SegmentCD
        //[PXDimension(KGMonthlyInspectionTemplateH.DimensionName)]
        [PXDBString(30, IsKey = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "月查核類別", Required = true, IsReadOnly = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXSelector(typeof(Search2<SegmentValue.value,
            InnerJoin<Segment, On<Segment.segmentID, Equal<SegmentValue.segmentID>,
                And<Segment.dimensionID, Equal<SegmentValue.dimensionID>>>>,
            Where<Segment.dimensionID, Equal<KGInspectionConstant.kgmninsc>,
                And<Segment.segmentID, Equal<KGInspectionConstant.segmentIDPart2>>>>),
            typeof(SegmentValue.value),
            typeof(SegmentValue.descr), DescriptionField = typeof(SegmentValue.descr)
        )]
        public virtual string SegmentCD { get; set; }
        public abstract class segmentCD : IBqlField { }

        #endregion

        #region MonthlyInspectionLineID
        [PXDBIdentity(IsKey = true)]
        [PXUIField(DisplayName = "Monthly Inspection Line ID")]
        public virtual int? MonthlyInspectionLineID { get; set; }
        public abstract class monthlyInspectionLineID : IBqlField { }
        #endregion

        #region TemplateLineID
        [PXDBInt()]
        [PXUIField(DisplayName = "Template Line ID")]
        public virtual int? TemplateLineID { get; set; }
        public abstract class templateLineID : IBqlField { }
        #endregion

        #region CheckItem
        [PXString(50, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Check Item",Enabled =false)]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXDBScalar(
            typeof(Search<KGMonthlyInspectionTemplateL.checkItem,
                Where<KGMonthlyInspectionTemplateL.templateLineID, Equal<KGMonthlyInspectionL.templateLineID>>>
                )
        )]
        public virtual string CheckItem { get; set; }
        public abstract class checkItem : IBqlField { }
        #endregion

        #region CheckPointDesc
        [PXString(50, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Check Point Desc", Enabled = false)]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXDBScalar(
            typeof(Search<KGMonthlyInspectionTemplateL.checkPointDesc,
                Where<KGMonthlyInspectionTemplateL.templateLineID, Equal<KGMonthlyInspectionL.templateLineID>>>
                )
        )]
        public virtual string CheckPointDesc { get; set; }
        public abstract class checkPointDesc : IBqlField { }
        #endregion



        /*
        #region InvDesc
        [PXString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Inv Descr", Enabled = false, IsReadOnly = true)]
        [PXDBScalar(
            typeof(Search<InventoryItem.descr,
                Where<InventoryItem.inventoryID, Equal<KGMonthlyInspectionL.inventoryID>>>
                )
        )]
        public virtual string InvDesc { get; set; }
        public abstract class invDesc : IBqlField { }
        #endregion*/

        #region IsChecked
        [PXDBBool()]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Is Checked", Required = true)]
        public virtual bool? IsChecked { get; set; }
        public abstract class isChecked : IBqlField { }
        #endregion

        #region Remark
        [PXDBString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Remark")]
        public virtual string Remark { get; set; }
        public abstract class remark : IBqlField { }
        #endregion

        #region LastMonthRemark
        [PXDBString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "LastMonthRemark",Enabled =false)]
        public virtual string LastMonthRemark { get; set; }
        public abstract class lastMonthRemark : IBqlField { }
        #endregion

        #region MissingNum
        [PXDBInt()]
        [PXDefault(0)]
        [PXUIField(DisplayName = "Missing Num", Required = true)]
        public virtual int? MissingNum { get; set; }
        public abstract class missingNum : IBqlField { }
        #endregion

        #region EveryMissingBuckle
        [PXDecimal()]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Every Missing Buckle", Enabled = false, IsReadOnly = true)]
        public virtual Decimal? EveryMissingBuckle { get; set; }
        public abstract class everyMissingBuckle : IBqlField { }
        #endregion

        #region Deduction
        [PXDBDecimal()]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Deduction", Enabled = false, IsReadOnly = true)]
        //[PXFormula(typeof(Mult<KGMonthlyInspectionL.missingNum, KGMonthlyInspectionL.everyMissingBuckle>))]
        public virtual Decimal? Deduction { get; set; }
        public abstract class deduction : IBqlField { }
        #endregion

        #region ScoreSetupLine
        [PXDecimal()]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Score Setup Line", Enabled = false, IsReadOnly = true, Required = true)]
        public virtual Decimal? ScoreSetupLine { get; set; }
        public abstract class scoreSetupLine : IBqlField { }
        #endregion

        #region Score
        [PXDBDecimal()]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Score", Enabled = false, IsReadOnly = true)]
        //[PXFormula(null, typeof(SumCalc<KGMonthlyInspection.finalScore>))]
        [PXFormula(typeof(Sub<KGMonthlyInspectionL.scoreSetupLine, KGMonthlyInspectionL.deduction>))]
        /*[PXFormula(typeof(Sub<KGMonthlyInspectionL.scoreSetupLine, KGMonthlyInspectionL.deduction>),
            typeof(SumCalc<KGMonthlyInspection.finalScore>))]*/
        public virtual Decimal? Score { get; set; }
        public abstract class score : IBqlField { }
        #endregion

        #region TestResult
        [PXDBString(1, IsFixed = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Test Result")]
        [KGMonthInspectionResults.List]
        public virtual string TestResult { get; set; }
        public abstract class testResult : IBqlField { }
        #endregion

        #region MaxNoMissing
        [PXInt()]
        [PXUIField(DisplayName = "Max No Missing", Enabled = false, Required = true, IsReadOnly = true)]
        [PXDBScalar(
            typeof(Search<KGMonthlyInspectionTemplateL.maxNoMissing,
            Where<KGMonthlyInspectionTemplateL.templateLineID, Equal<KGMonthlyInspectionL.templateLineID>>>))]
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
		[Branch(typeof(KGMonthlyInspection.branchID))]
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

        #region RiskType
        [PXString(3, IsUnicode = true)]
        [PXUIField(DisplayName = "RiskType")]
        
        public virtual string RiskType { get; set; }
        public abstract class riskType : IBqlField { }
        #endregion

        #region RiskCategory
        [PXString(1, IsUnicode = true)]
        [PXUIField(DisplayName = "RiskCategory")]

        public virtual string RiskCategory { get; set; }
        public abstract class riskCategory : IBqlField { }
        #endregion
    }

    #region KGMonthInspectionResults
    public static class KGMonthInspectionResults
    {

        public const string QualifiedName = "檢查合格";
        public const string DefectName = "有缺失需改正";
        public const string ObservationName = "觀察事項";
        public const string Qualified = "1";
        public const string Defect = "2";
        public const string Observation = "3";

        public class qualified : BqlString.Constant<qualified>
        {
            public qualified() : base(Qualified) { }
        }

        public class defect : BqlString.Constant<defect>
        {
            public defect() : base(Defect) { }
        }
        public class noItem : BqlString.Constant<noItem>
        {
            public noItem() : base(ObservationName) { }
        }
        public class ListAttribute : PXStringListAttribute
        {
            public ListAttribute()
                : base(
                    new string[] { Qualified, Defect, Observation },
                    new string[] { QualifiedName, DefectName, ObservationName })
            { }
        }

        public class ListSimpleAttribute : PXStringListAttribute
        {
            public ListSimpleAttribute()
                : base(
                new string[] { Qualified, Defect, Observation },
                new string[] { QualifiedName, DefectName, ObservationName })
            { }
        }
    }
    #endregion
}