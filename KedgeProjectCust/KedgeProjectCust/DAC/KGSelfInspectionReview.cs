using System;
using PX.Data;

namespace Kedge.DAC
{
    [Serializable]
    public class KGSelfInspectionReview : IBqlTable
    {
        #region Selected
        [PXBool]
        [PXUIField(DisplayName = "Selected")]
        public virtual bool? Selected { get; set; }
        public abstract class selected : IBqlField { }
        #endregion

        #region SelfInspectionID
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Self Inspection ID")]
        [PXDBDefault(typeof(KGSelfInspection.selfInspectionID))]
        [PXParent(typeof(Select<KGSelfInspection,
                        Where<KGSelfInspection.selfInspectionID,
                          Equal<Current<KGSelfInspectionReview.selfInspectionID>>>>))]
        public virtual int? SelfInspectionID { get; set; }
        public abstract class selfInspectionID : IBqlField { }
        #endregion

        #region SelfInspectionReviewID
        [PXDBIdentity(IsKey = true)]
        [PXUIField(DisplayName = "Self Inspection Review ID")]
        public virtual int? SelfInspectionReviewID { get; set; }
        public abstract class selfInspectionReviewID : IBqlField { }
        #endregion

        #region TemplateLineID
        [PXDBInt]
        public virtual int? TemplateLineID { get; set; }
        public abstract class templateLineID : IBqlField { }
        #endregion

        #region CheckItem
        [PXString(IsUnicode = true, InputMask = "")]
        [PXDBScalar(
            typeof(Search<KGSelfInspectionTemplateL.checkItem,
            Where<KGSelfInspectionTemplateL.templateLineID, Equal<KGSelfInspectionReview.templateLineID>>>))]
        [PXUIField(DisplayName = "Check Item", IsReadOnly = true, Enabled = false)]
        public virtual string CheckItem { get; set; }
        public abstract class checkItem : IBqlField { }
        #endregion

        #region TestResult
        [PXDBString(1, IsFixed = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Test Result", IsReadOnly = true, Enabled = false)]
        [KGSelfInspectionTestResults.List]
        public virtual string TestResult { get; set; }
        public abstract class testResult : IBqlField { }
        #endregion

        #region ReviewResult
        [PXDBString(1, IsFixed = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Review Result")]
        [KGSelfInspectionReviewResults.List]
        public virtual string ReviewResult { get; set; }
        public abstract class reviewResult : IBqlField { }
        #endregion

        #region MissingProcessing
        [PXDBString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Missing Processing")]
        public virtual string MissingProcessing { get; set; }
        public abstract class missingProcessing : IBqlField { }
        #endregion

        #region CompleteDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Complete Date")]
        public virtual DateTime? CompleteDate { get; set; }
        public abstract class completeDate : IBqlField { }
        #endregion

        #region Remark
        [PXDBString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Remark")]
        public virtual string Remark { get; set; }
        public abstract class remark : IBqlField { }
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

    }
}