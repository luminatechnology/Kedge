using System;
using PX.Data;
using PX.Data.BQL;
using PX.Objects.PM;
using PX.Objects.AR;

using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.CS;
using PX.Objects.EP;
namespace Kedge.DAC
{
    public static class KGMonthInspectionTicketStatuses
    {

        public const string HoldName = "Hold";
        public const string OpenName = "Open";

        public const string CloseName = "Close";
        public const string Hold = "H";
        public const string Open = "N";

        public const string Close = "C";

        public class open : BqlString.Constant<open>
        {
            public open() : base(Open) { }
        }

        public class close : BqlString.Constant<close>
        {
            public close() : base(Close) { }
        }
        public class hold : BqlString.Constant<hold>
        {
            public hold() : base(Hold) { }
        }
        public class ShortListAttribute : PXStringListAttribute
        {
            public ShortListAttribute()
                : base(
                    new string[] { Hold, Open, Close },
                    new string[] { HoldName, OpenName, CloseName })
            {
            }
        }
    }


    [Serializable]
    public class KGMonthlyInspectTicket : IBqlTable
    {
        #region MonthlyInspectTicketID
        [PXDBIdentity]
        [PXUIField(DisplayName = "Monthly Inspect Ticket ID")]
        public virtual int? MonthlyInspectTicketID { get; set; }
        public abstract class monthlyInspectTicketID : PX.Data.BQL.BqlInt.Field<monthlyInspectTicketID> { }
        #endregion


        #region KGMonthlyInspectTicketCD
        [PXDBString(50, IsUnicode = true, InputMask = ">CCCCCCCCCC", IsKey = true)]
        [PXUIField(DisplayName = "Monthly Inspect TicketCD")]
        [PXSelector(typeof(Search<KGMonthlyInspectTicket.monthlyInspectTicketCD>),
            typeof(KGMonthlyInspectTicket.monthlyInspectTicketCD), 
            typeof(KGMonthlyInspectTicket.contractID),
            SubstituteKey = typeof(KGMonthlyInspectTicket.monthlyInspectTicketCD))]
        [AutoNumber(typeof(KGSetUp.kGMonthlyInspectTicketNumbering), typeof(AccessInfo.businessDate))]
        public virtual string MonthlyInspectTicketCD  { get; set; }
        public abstract class monthlyInspectTicketCD : PX.Data.BQL.BqlString.Field<monthlyInspectTicketCD> { }
        #endregion
        /*
        #region MonthlyInspectionCD
        [PXDBString(50, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Monthly Inspection CD")]
        [PXDefault()]
        [PXSelector(typeof(Search<KGMonthlyInspection.monthlyInspectionCD,
            Where<KGMonthlyInspection.projectID, Equal<Current<KGMonthlyInspectTicket.contractID>>
                ,And<KGMonthlyInspection.status,Equal<KGMonthInspectionStatuses.hold>>>>),
            typeof(KGMonthlyInspection.monthlyInspectionCD),
            typeof(KGMonthlyInspection.checkYear), typeof(KGMonthlyInspection.checkMonth))]
        public virtual string MonthlyInspectionCD { get; set; }
        public abstract class monthlyInspectionCD : PX.Data.BQL.BqlString.Field<monthlyInspectionCD> { }
        #endregion*/
        
        #region MonthlyInspectionID
        [PXDBInt()]
        [PXUIField(DisplayName = "Monthly Inspection CD")]
        [PXDefault()]
        [PXSelector(typeof(Search<KGMonthlyInspection.monthlyInspectionID,
            Where<KGMonthlyInspection.projectID, Equal<Current<KGMonthlyInspectTicket.contractID>>
                ,And<KGMonthlyInspection.status,Equal<KGMonthInspectionTicketStatuses.hold>>>>),
            typeof(KGMonthlyInspection.monthlyInspectionCD),
            typeof(KGMonthlyInspection.checkYear), typeof(KGMonthlyInspection.checkMonth),SubstituteKey = typeof(KGMonthlyInspection.monthlyInspectionCD))]
        public virtual int? MonthlyInspectionID { get; set; }
        public abstract class monthlyInspectionID : PX.Data.BQL.BqlInt.Field<monthlyInspectionID> { }
        #endregion


        #region ContractID
        //[PXDBInt()]
        [ProjectBaseExt()]
        [PXUIField(DisplayName = "Project",Required =true)]
        [PXForeignReference(typeof(Field<contractID>.IsRelatedTo<PMProject.contractID>))]
        public virtual int? ContractID { get; set; }
        public abstract class contractID : PX.Data.BQL.BqlInt.Field<contractID> { }
        #endregion

        #region CheckDate
        [PXDBDate()]
        [PXDefault(typeof(AccessInfo.businessDate))]
        [PXUIField(DisplayName = "Check Date")]
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
        [PXDefault(KGMonthInspectionTicketStatuses.Hold)]
        [KGMonthInspectionTicketStatuses.ShortList()]
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

        #region SegmentCD
        [PXString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "月查核類別",Enabled =false)]
        
        [PXSelector(typeof(Search2<SegmentValue.value,
            InnerJoin<Segment, On<Segment.segmentID, Equal<SegmentValue.segmentID>,
                And<Segment.dimensionID, Equal<SegmentValue.dimensionID>>>>,
            Where<Segment.dimensionID, Equal<KGInspectionConstant.kgmninsc>,
                And<Segment.segmentID, Equal<KGInspectionConstant.segmentIDPart1>>>>),
            typeof(SegmentValue.value),
            typeof(SegmentValue.descr),DescriptionField = typeof(SegmentValue.descr)
        )]
        /*
        [PXDBScalar(
            typeof(Search<KGMonthlyInspection.segmentCD,
            Where<KGMonthlyInspection.monthlyInspectionCD, Equal<KGMonthlyInspectTicket.monthlyInspectionCD>>>)
        )]*/
        public virtual string SegmentCD { get; set; }
        public abstract class segmentCD : IBqlField { }
        #endregion
        /*
        #region SegmentDesc
        [PXString(256, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Monthly Segment Desc", Enabled = false)]

        public virtual string SegmentDesc { get; set; }
        public abstract class segmentDesc : IBqlField { }*/


        #region CheckYear
        [PXInt()]
        [PXUIField(DisplayName = "CheckYear",Enabled =false)]
        [RecentYear.List]
        [PXDBScalar(
            typeof(Search<KGMonthlyInspection.checkYear,
            Where<KGMonthlyInspectTicket.monthlyInspectionID, Equal<KGMonthlyInspection.monthlyInspectionID>>>)
        )]
        public virtual int? CheckYear { get; set; }
        public abstract class checkYear : IBqlField { }
        #endregion
        #region CheckMonth
        [PXInt()]
        [PXUIField(DisplayName = "CheckMonth",Enabled =false)]
        [MonthOfYear.List]
        /*
        [PXDBScalar(
            typeof(Search<KGMonthlyInspection.checkMonth,
            Where<KGMonthlyInspectTicket.monthlyInspectionCD, Equal<KGMonthlyInspection.monthlyInspectionCD>>>)
        )]*/
        public virtual int? CheckMonth { get; set; }
        public abstract class checkMonth : IBqlField { }
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