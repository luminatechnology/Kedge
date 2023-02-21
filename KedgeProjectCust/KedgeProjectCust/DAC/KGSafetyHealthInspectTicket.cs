using System;
using PX.Data;
using PX.Data.BQL;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.CS;
using PX.Objects.EP;
using PX.Objects.PM;

namespace Kedge.DAC
{
    [Serializable]
    public class KGSafetyHealthInspectTicket : IBqlTable
    {
        #region SafetyHealthInspectTicketID
        [PXDBIdentity()]
        [PXUIField(DisplayName = "Safety Health Inspect Ticket ID")]
        public virtual int? SafetyHealthInspectTicketID { get; set; }
        public abstract class safetyHealthInspectTicketID : PX.Data.BQL.BqlInt.Field<safetyHealthInspectTicketID> { }
        #endregion

        #region SafetyHealthInspectTicketCD
        [PXDBString(50, IsUnicode = true, InputMask = "",IsKey =true)]
        [PXUIField(DisplayName = "Safety Health Inspect Ticket CD")]
        [PXSelector(typeof(Search<KGSafetyHealthInspectTicket.safetyHealthInspectTicketCD>),
                    typeof(KGSafetyHealthInspectTicket.safetyHealthInspectTicketCD),
                    typeof(KGSafetyHealthInspectTicket.checkDate)
                   )]
        [AutoNumber(typeof(Search<KGSetUp.kGSafetyHealthInspectTicketNumbering>), typeof(AccessInfo.businessDate))]
        public virtual string SafetyHealthInspectTicketCD { get; set; }
        public abstract class safetyHealthInspectTicketCD : PX.Data.BQL.BqlString.Field<safetyHealthInspectTicketCD> { }
        #endregion

        #region SafetyHealthInspectionID
        [PXDBInt()]
        [PXUIField(DisplayName = "Safety Health Inspection ID",Required =true)]
        [PXSelector(typeof(Search2<KGSafetyHealthInspection.safetyHealthInspectionID,
            InnerJoin<KGSafetyHealthInspectionTemplateH,On<KGSafetyHealthInspectionTemplateH.templateHeaderID,Equal<KGSafetyHealthInspection.templateHeaderID>>>,
            Where<KGSafetyHealthInspection.projectID, Equal<Current<projectID>>,
                And<KGSafetyHealthInspection.status,Equal<pStatusHold>>>>),
            typeof(KGSafetyHealthInspection.safetyHealthInspectionCD), 
            typeof(KGSafetyHealthInspectionTemplateH.segmentCD),
            typeof(KGSafetyHealthInspectionTemplateH.segmentDesc),
             typeof(KGSafetyHealthInspection.checkDate),
            SubstituteKey = typeof(KGSafetyHealthInspection.safetyHealthInspectionCD))]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public virtual int? SafetyHealthInspectionID { get; set; }
        public abstract class safetyHealthInspectionID : PX.Data.BQL.BqlInt.Field<safetyHealthInspectionID> { }
        #endregion

        #region ProjectID
        //[PXDBInt()]
        [PXUIField(DisplayName = "Project ID",Required =true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.Null)]
        [ProjectBase()]
        [PXForeignReference(typeof(Field<projectID>.IsRelatedTo<PMProject.contractID>))]
        public virtual int? ProjectID { get; set; }
        public abstract class projectID : PX.Data.BQL.BqlInt.Field<projectID> { }
        #endregion

        #region CheckDate
        [PXDBDate()]
        [PXDefault(typeof(AccessInfo.businessDate), PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXUIField(DisplayName = "Check Date", Required = true)]
        public virtual DateTime? CheckDate { get; set; }
        public abstract class checkDate : PX.Data.BQL.BqlDateTime.Field<checkDate> { }
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
        [PXUIField(DisplayName = "Status",Enabled =false)]
        [PXDefault("H")]
        [PXStringList(
                new string[]{"H","N","C"},
                new string[]{"Hold","Open","Close"})]
        public virtual string Status { get; set; }
        public abstract class status : PX.Data.BQL.BqlString.Field<status> { }
        #endregion

        #region InspectByID
        [PXDBInt()]
        [PXUIField(DisplayName = "InspectByID", Required = true)]
        [PXDefault(
            typeof(Search<EPEmployee.bAccountID, Where<EPEmployee.userID, Equal<Current<AccessInfo.userID>>>>)
            , PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXEPEmployeeSelector]
        public virtual int? InspectByID { get; set; }
        public abstract class inspectByID : IBqlField { }
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
        [PXDBGuid()]
        [PXUIField(DisplayName = "NoteID")]
        public virtual Guid? NoteID { get; set; }
        public abstract class noteID : PX.Data.BQL.BqlGuid.Field<noteID> { }
        #endregion

        #region Tstamp
        [PXDBTimestamp()]
        [PXUIField(DisplayName = "Tstamp")]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
        #endregion

        #region SegmentCD
        [PXString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Â¾¦w½ÃÃþ§O", Enabled = false)]
        [PXSelector(typeof(Search2<SegmentValue.value,
            InnerJoin<Segment, On<Segment.segmentID, Equal<SegmentValue.segmentID>,
                And<Segment.dimensionID, Equal<SegmentValue.dimensionID>>>>,
            Where<Segment.dimensionID, Equal<KGInspectionConstant.kgshinsc>,
                And<Segment.segmentID, Equal<KGInspectionConstant.segmentIDPart1>>>>),
            typeof(SegmentValue.value),
            typeof(SegmentValue.descr), DescriptionField = typeof(SegmentValue.descr)
        )]
        [PXDBScalar(
            typeof(Search2<KGSafetyHealthInspectionTemplateH.segmentCD,
                InnerJoin<KGSafetyHealthInspection,On<KGSafetyHealthInspection.templateHeaderID,Equal<KGSafetyHealthInspectionTemplateH.templateHeaderID>>>,
            Where<KGSafetyHealthInspection.safetyHealthInspectionID, Equal<KGSafetyHealthInspectTicket.safetyHealthInspectionID>>>)
        )]
        public virtual string SegmentCD { get; set; }
        public abstract class segmentCD : IBqlField { }
        #endregion

        #region Param
        #region pStatus
        public const string PStatusHold = "H";
        public const string PStatusOpen = "N";
        public const string PStatusClose = "C";

        public class pStatusHold : BqlString.Constant<pStatusHold>
        {
            public pStatusHold() : base(PStatusHold) { }
        }
        public class pStatusOpen : BqlString.Constant<pStatusOpen>
        {
            public pStatusOpen() : base(PStatusOpen) { }
        }
        public class pStatusClose : BqlString.Constant<pStatusClose>
        {
            public pStatusClose() : base(PStatusClose) { }
        }
        #endregion
        #endregion
    }
}