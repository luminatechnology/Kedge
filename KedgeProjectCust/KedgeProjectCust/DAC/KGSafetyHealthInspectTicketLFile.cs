using System;
using PX.Data;

namespace Kedge.DAC
{
    [Serializable]
    public class KGSafetyHealthInspectTicketLFile : IBqlTable
    {
        #region Fileid
        [PXDBIdentity(IsKey = true)]
        [PXUIField(DisplayName = "Fileid")]
        public virtual int? Fileid { get; set; }
        public abstract class fileid : PX.Data.BQL.BqlInt.Field<fileid> { }
        #endregion

        #region SafetyHealthInspectTicketLineID
        [PXDBInt()]
        [PXUIField(DisplayName = "Safety Health Inspect Ticket Line ID")]
        [PXDBDefault(typeof(KGSafetyHealthInspectTicketL.safetyHealthInspectTicketLineID))]       
        public virtual int? SafetyHealthInspectTicketLineID { get; set; }
        public abstract class safetyHealthInspectTicketLineID : PX.Data.BQL.BqlInt.Field<safetyHealthInspectTicketLineID> { }
        #endregion

        #region SafetyHealthInspectTicketID
        [PXDBInt()]
        [PXUIField(DisplayName = "Safety Health Inspect Ticket ID")]
        [PXDBDefault(typeof(KGSafetyHealthInspectTicket.safetyHealthInspectTicketID))]
        [PXParent(typeof(Select<KGSafetyHealthInspectTicket,
                                Where<KGSafetyHealthInspectTicket.safetyHealthInspectTicketID,
                                Equal<Current<KGSafetyHealthInspectTicketL.safetyHealthInspectTicketID>>>>))]
        public virtual int? SafetyHealthInspectTicketID { get; set; }
        public abstract class safetyHealthInspectTicketID : PX.Data.BQL.BqlInt.Field<safetyHealthInspectTicketID> { }
        #endregion

        #region SafetyHealthInspectionLineID
        [PXDBInt()]
        [PXUIField(DisplayName = "Safety Health Inspection Line ID")]
        public virtual int? SafetyHealthInspectionLineID { get; set; }
        public abstract class safetyHealthInspectionLineID : PX.Data.BQL.BqlInt.Field<safetyHealthInspectionLineID> { }
        #endregion

        #region TestResult
        [PXDBString(1, IsFixed = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Test Result",Enabled =false)]
        [KGMonthInspectionResults.List]
        public virtual string TestResult { get; set; }
        public abstract class testResult : PX.Data.BQL.BqlString.Field<testResult> { }
        #endregion

        #region ReviseTestResult
        [PXString()]
        [PXUIField(DisplayName = "Revise Test Result")]
        [KGMonthInspectionResults.List]
        public virtual string ReviseTestResult { get; set; }
        public abstract class reviseTestResult : PX.Data.BQL.BqlString.Field<reviseTestResult> { }
        #endregion

        #region IssueDesc
        [PXDBString(1000, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Issue Desc")]
        public virtual string IssueDesc { get; set; }
        public abstract class issueDesc : PX.Data.BQL.BqlString.Field<issueDesc> { }
        #endregion

        #region ImprovementPlan
        [PXDBString(1000, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "ImprovementPlan")]
        public virtual string ImprovementPlan { get; set; }
        public abstract class improvementPlan : PX.Data.BQL.BqlString.Field<improvementPlan> { }
        #endregion

        #region ImageUrl
        [PXDBString(255, InputMask = "")]
        [PXUIField(DisplayName = "Image Url")]
        public virtual string ImageUrl { get; set; }
        public abstract class imageUrl : PX.Data.BQL.BqlString.Field<imageUrl> { }
        #endregion

        #region CheckItem
        [PXString(50, IsUnicode = true, InputMask = "")]
        [PXDBScalar(
            typeof(Search2<KGSafetyHealthInspectionTemplateL.checkItem, InnerJoin<KGSafetyHealthInspectionL,
                On<KGSafetyHealthInspectionL.templateLineID, Equal<KGSafetyHealthInspectionTemplateL.templateLineID>>,
                InnerJoin<KGSafetyHealthInspectTicketLFile, On<KGSafetyHealthInspectTicketLFile.safetyHealthInspectionLineID,
                    Equal<KGSafetyHealthInspectTicketLFile.safetyHealthInspectionLineID>>>>>)
        )]
        [PXUIField(DisplayName = "Check Item", IsReadOnly = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]

        public virtual string CheckItem { get; set; }
        public abstract class checkItem : IBqlField { }
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
        public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
        #endregion
    }
}