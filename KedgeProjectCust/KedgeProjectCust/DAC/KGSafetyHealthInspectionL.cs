using System;
using PX.Data;
using PX.Objects.CS;

namespace Kedge.DAC
{
    [Serializable]
    public class KGSafetyHealthInspectionL : IBqlTable
    {

        #region Selected
        [PXBool]
        [PXUIField(DisplayName = "Selected")]
        public virtual bool? Selected { get; set; }
        public abstract class selected : IBqlField { }
        #endregion

        #region SafetyHealthInspectionLineID
        [PXDBIdentity(IsKey = true)]
        [PXUIField(DisplayName = "Safety Health Inspection Line ID")]
        public virtual int? SafetyHealthInspectionLineID { get; set; }
        public abstract class safetyHealthInspectionLineID : IBqlField { }
        #endregion

        #region SafetyHealthInspectionID
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Safety Health Inspection ID")]
        [PXDBDefault(typeof(KGSafetyHealthInspection.safetyHealthInspectionID))]
        [PXParent(typeof(Select<KGSafetyHealthInspection,
                    Where<KGSafetyHealthInspection.safetyHealthInspectionID,
                    Equal<Current<KGSafetyHealthInspectionL.safetyHealthInspectionID>>>>))]
        public virtual int? SafetyHealthInspectionID { get; set; }
        public abstract class safetyHealthInspectionID : IBqlField { }
        #endregion

        #region TemplateLineID
        [PXDBInt()]
        [PXUIField(DisplayName = "Safety Health Template Line ID")]
        public virtual int? TemplateLineID { get; set; }
        public abstract class templateLineID : PX.Data.BQL.BqlInt.Field<templateLineID> { }
        #endregion

        #region SegmentCD
        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Safety Health Segment CD", Enabled = false)]
        public virtual string SegmentCD { get; set; }
        public abstract class segmentCD : PX.Data.BQL.BqlString.Field<segmentCD> { }
        #endregion

        #region SegmentDesc
        [PXString(IsUnicode = true)]
        [PXUIField(DisplayName = "Safety Health Segment Desc", Enabled = false)]
        public virtual string SegmentDesc { get; set; }
        public abstract class segmentDesc : IBqlField { }
        #endregion

        #region IsChecked
        [PXDBBool()]
        [PXUIField(DisplayName = "Is Checked")]
        [PXDefault(true)]
        public virtual bool? IsChecked { get; set; }
        public abstract class isChecked : IBqlField { }
        #endregion

        #region CategoryCD
        [PXDBString(50, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Safety Health Category CD", IsReadOnly = true)]
        [PXStringList()]
        public virtual string CategoryCD { get; set; }
        public abstract class categoryCD : IBqlField { }
        #endregion

        #region Remark
        [PXDBString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Remark")]
        public virtual string Remark { get; set; }
        public abstract class remark : IBqlField { }
        #endregion

        #region Deduction
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Safety Health Deduction", Enabled = false)]
        public virtual Decimal? Deduction { get; set; }
        public abstract class deduction : IBqlField { }
        #endregion

        //2019/10/01 Delete
        /*#region Score
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Score", Enabled = false)]
        public virtual Decimal? Score { get; set; }
        public abstract class score : IBqlField { }
        #endregion*/

        //2019/10/01 Delete
        /* #region ScoreSetupLine
         [PXDecimal()]
         [PXUIField(DisplayName = "Safety Health Score Setup Line", Enabled = false)]
         public virtual Decimal? ScoreSetupLine { get; set; }
         public abstract class scoreSetupLine : IBqlField { }
         #endregion*/

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

        /* #region NoteID
         [PXNote()]
         [PXUIField(DisplayName = "NoteID")]
         public virtual Guid? NoteID { get; set; }
         public abstract class noteID : IBqlField { }
         #endregion*/

        #region Tstamp
        [PXDBTimestamp()]
        [PXUIField(DisplayName = "Tstamp")]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : IBqlField { }
        #endregion

        #region LastRemark
        [PXDBString(IsUnicode = true)]
        [PXUIField(DisplayName = "LastRemark", Enabled = false)]
        public virtual string LastRemark { get; set; }
        public abstract class lastRemark : PX.Data.BQL.BqlString.Field<lastRemark> { }
        #endregion

        #region CheckItem
        [PXString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Check Item", Enabled = false)]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXDBScalar(
            typeof(Search<KGSafetyHealthInspectionTemplateL.checkItem,
                Where<KGSafetyHealthInspectionTemplateL.templateLineID,
                    Equal<KGSafetyHealthInspectionL.templateLineID>>>
                )
        )]
        public virtual string CheckItem { get; set; }
        public abstract class checkItem : IBqlField { }
        #endregion

        //2020/03/09 Delete
        /*#region CheckPointDesc
        [PXString(50, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Check Point Desc", Enabled = false)]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXDBScalar(
            typeof(Search<KGSafetyHealthInspectionTemplateL.checkPointDesc,
                Where<KGSafetyHealthInspectionTemplateL.templateLineID,
                    Equal<KGSafetyHealthInspectionL.templateLineID>>>
                )
        )]
        public virtual string CheckPointDesc { get; set; }
        public abstract class checkPointDesc : IBqlField { }
        #endregion*/

        //2019/11/21 ADD
        #region ImprovementPlan
        [PXDBString(IsUnicode = true)]
        [PXUIField(DisplayName = "Improvement Plan")]
        public virtual string ImprovementPlan { get; set; }
        public abstract class improvementPlan : PX.Data.BQL.BqlString.Field<improvementPlan> { }
        #endregion
    }
}