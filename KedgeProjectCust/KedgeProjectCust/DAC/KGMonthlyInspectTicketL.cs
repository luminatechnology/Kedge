using System;
using PX.Data;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.CS;
using PX.Objects.FS;
using PX.Objects.GL;
namespace Kedge.DAC
{
    [Serializable]
    public class KGMonthlyInspectTicketLUpdateAll : KGMonthlyInspectTicketL
    {

        #region MonthlyInspectionID
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Monthly Inspection CD", TabOrder = 1)]
        public new virtual int? MonthlyInspectionID { get; set; }
        public new abstract class monthlyInspectionID : PX.Data.BQL.BqlInt.Field<monthlyInspectionID> { }
        #endregion

        #region MonthlyInspectTicketLineID
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Monthly Inspect Ticket Line ID")]
        public new virtual int? MonthlyInspectTicketLineID { get; set; }
        public new abstract class monthlyInspectTicketLineID : PX.Data.BQL.BqlInt.Field<monthlyInspectTicketLineID> { }
        #endregion

        #region MonthlyInspectionLineID
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Monthly Inspection Line ID", TabOrder = 1)]
        public new virtual int? MonthlyInspectionLineID { get; set; }
        public new abstract class monthlyInspectionLineID : PX.Data.BQL.BqlInt.Field<monthlyInspectionLineID> { }
        #endregion

        #region MonthlyInspectTicketID
        [PXDBInt()]
        [PXUIField(DisplayName = "Monthly Inspect Ticket ID")]
        //[PXDBDefault(typeof(KGMonthlyInspectTicketL.monthlyInspectTicketID))]
        public new virtual int? MonthlyInspectTicketID { get; set; }
        public new abstract class monthlyInspectTicketID : PX.Data.BQL.BqlInt.Field<monthlyInspectTicketID> { }
        #endregion

        #region Remark
        [PXDBString(1000, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Issue Desc")]
        [PXDefault("無資料")]
        public new virtual string Remark { get; set; }
        public new abstract class remark : PX.Data.BQL.BqlString.Field<remark> { }
        #endregion

    }


    [Serializable]
    public class KGMonthlyInspectTicketLUpdate : KGMonthlyInspectTicketL
    {

        #region TestResult
        [PXDBString(1, IsFixed = true, IsUnicode = true, InputMask = "", IsKey = true)]
        [PXUIField(DisplayName = "Test Result", TabOrder = 0)]
        [KGMonthInspectionResults.List]
        public new virtual string TestResult { get; set; }
        public new abstract class testResult : PX.Data.BQL.BqlString.Field<testResult> { }
        #endregion

        #region MonthlyInspectionID
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Monthly Inspection CD", TabOrder = 1)]
        public new virtual int? MonthlyInspectionID { get; set; }
        public new abstract class monthlyInspectionID : PX.Data.BQL.BqlInt.Field<monthlyInspectionID> { }
        #endregion

        #region MonthlyInspectTicketLineID
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Monthly Inspect Ticket Line ID")]
        public new virtual int? MonthlyInspectTicketLineID { get; set; }
        public new abstract class monthlyInspectTicketLineID : PX.Data.BQL.BqlInt.Field<monthlyInspectTicketLineID> { }
        #endregion

        #region MonthlyInspectionLineID
        [PXDBInt()]
        [PXUIField(DisplayName = "Monthly Inspection Line ID")]
        public new virtual int? MonthlyInspectionLineID { get; set; }
        public new abstract class monthlyInspectionLineID : PX.Data.BQL.BqlInt.Field<monthlyInspectionLineID> { }
        #endregion

        #region MonthlyInspectTicketID
        [PXDBInt()]
        [PXUIField(DisplayName = "Monthly Inspect Ticket ID")]
        //[PXDBDefault(typeof(KGMonthlyInspectTicketL.monthlyInspectTicketID))]
        public new virtual int? MonthlyInspectTicketID { get; set; }
        public new abstract class monthlyInspectTicketID : PX.Data.BQL.BqlInt.Field<monthlyInspectTicketID> { }
        #endregion


        #region Remark
        [PXDBString(1000, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Issue Desc")]
        [PXDefault("無資料")]
        public new virtual string Remark { get; set; }
        public new abstract class remark : PX.Data.BQL.BqlString.Field<remark> { }
        #endregion

    }


    [Serializable]
    public class KGMonthlyInspectTicketL : IBqlTable
    {
        #region MonthlyInspectTicketLineID
        [PXDBIdentity(IsKey = true)]
        [PXUIField(DisplayName = "Monthly Inspect Ticket Line ID")]
        public virtual int? MonthlyInspectTicketLineID { get; set; }
        public abstract class monthlyInspectTicketLineID : PX.Data.BQL.BqlInt.Field<monthlyInspectTicketLineID> { }
        #endregion


        #region MonthlyInspectTicketID
        [PXDBInt()]
        [PXUIField(DisplayName = "Monthly Inspect Ticket ID")]
        [PXDBDefault(typeof(KGMonthlyInspectTicket.monthlyInspectTicketID))]
        [PXParent(typeof(Select<KGMonthlyInspectTicket,
                            Where<KGMonthlyInspectTicket.monthlyInspectTicketID,
                                Equal<Current<KGMonthlyInspectTicketL.monthlyInspectTicketID>>>>))]
        public virtual int? MonthlyInspectTicketID { get; set; }
        public abstract class monthlyInspectTicketID : PX.Data.BQL.BqlInt.Field<monthlyInspectTicketID> { }
        #endregion


        #region MonthlyInspectionLineID
        [PXDBInt()]
        [PXUIField(DisplayName = "Monthly Inspection LineID")]
        public virtual int? MonthlyInspectionLineID { get; set; }
        public abstract class monthlyInspectionLineID : PX.Data.BQL.BqlInt.Field<monthlyInspectionLineID> { }
        #endregion
        /*
        [PXDBString(50, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Monthly Inspection CD")]
        [PXDBDefault(typeof(KGMonthlyInspectTicket.monthlyInspectionCD))]
        [PXSelector(typeof(Search<KGMonthlyInspection.monthlyInspectionCD>),
            typeof(KGMonthlyInspection.monthlyInspectionCD),
            typeof(KGMonthlyInspection.checkYear), typeof(KGMonthlyInspection.checkMonth))]
        public virtual string MonthlyInspectionCD { get; set; }
        public abstract class monthlyInspectionCD : PX.Data.BQL.BqlString.Field<monthlyInspectionCD> { }
        */

        #region MonthlyInspectionID
        [PXDBInt()]
        [PXUIField(DisplayName = "Monthly Inspection CD")]
        [PXDBDefault(typeof(KGMonthlyInspectTicket.monthlyInspectionID))]
        [PXSelector(typeof(Search<KGMonthlyInspection.monthlyInspectionID>),
            typeof(KGMonthlyInspection.monthlyInspectionCD),
            typeof(KGMonthlyInspection.checkYear), typeof(KGMonthlyInspection.checkMonth),
            SubstituteKey = typeof(KGMonthlyInspection.monthlyInspectionCD))]
        public virtual int? MonthlyInspectionID { get; set; }
        public abstract class monthlyInspectionID : PX.Data.BQL.BqlInt.Field<monthlyInspectionID> { }
        #endregion
        #region TemplateLineID
        [PXDBInt()]
        [PXUIField(DisplayName = "Template Line ID")]
        public virtual int? TemplateLineID { get; set; }
        public abstract class templateLineID : IBqlField { }
        #endregion

        #region TestResult
        [PXDBString(1, IsFixed = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Test Result")]
        [KGMonthInspectionResults.List]
        public virtual string TestResult { get; set; }
        public abstract class testResult : IBqlField { }
        #endregion

        #region Remark
        [PXDBString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Issue Desc")]
        public virtual string Remark { get; set; }
        public abstract class remark : PX.Data.BQL.BqlString.Field<remark> { }
        #endregion

        #region LastMonthRemark
        [PXDBString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Last Month Issue Desc", Enabled = false)]
        public virtual string LastMonthRemark { get; set; }
        public abstract class lastMonthRemark : IBqlField { }
        #endregion

        /*
        #region MissingNum
        [PXDBInt()]
        [PXDefault(0, PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXUIField(DisplayName = "Missing Num")]
        public virtual int? MissingNum { get; set; }
        public abstract class missingNum : PX.Data.BQL.BqlInt.Field<missingNum> { }
        #endregion*/

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
        //[PXDimension(KGMonthlyInspectionTemplateH.DimensionName)]
        [PXString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "月查核類別", IsReadOnly = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXSelector(typeof(Search2<SegmentValue.value,
            InnerJoin<Segment, On<Segment.segmentID, Equal<SegmentValue.segmentID>,
                And<Segment.dimensionID, Equal<SegmentValue.dimensionID>>>>,
            Where<Segment.dimensionID, Equal<KGInspectionConstant.kgmninsc>,
                And<Segment.segmentID, Equal<KGInspectionConstant.segmentIDPart2>>>>),
            typeof(SegmentValue.value),
            typeof(SegmentValue.descr), DescriptionField = typeof(SegmentValue.descr)
        )]
        [PXDBScalar(
            typeof(Search<KGMonthlyInspectionL.segmentCD,
            Where<KGMonthlyInspectionL.monthlyInspectionLineID, Equal<KGMonthlyInspectTicketL.monthlyInspectionLineID>>>)
        )]
        public virtual string SegmentCD { get; set; }
        public abstract class segmentCD : IBqlField { }

        #endregion
        #region CheckItem
        [PXDBString(50, IsUnicode = true, InputMask = "")]
        //PXSelector 只能對外鍵與subsuitKey
        /*
        [PXSelector(typeof(Search2<KGMonthlyInspectionTemplateL.checkItem,
            InnerJoin<KGMonthlyInspectionL, On<KGMonthlyInspectionL.templateLineID,
                Equal<KGMonthlyInspectionTemplateL.templateLineID>>>,
            Where<KGMonthlyInspectionL.monthlyInspectionID,
                Equal<Current<KGMonthlyInspectTicketL.monthlyInspectionID>>>,
            OrderBy<Asc<KGMonthlyInspectionTemplateL.templateLineID>>>),
            typeof(KGMonthlyInspectionTemplateL.segmentCDRead),
            typeof(KGMonthlyInspectionTemplateL.segmentDescr),
            typeof(KGMonthlyInspectionTemplateL.checkItem)
        )]*/
        [PXSelectorWithCustomOrderBy(typeof(Search2<KGMonthlyInspectionTemplateL.checkItem,
            InnerJoin<KGMonthlyInspectionL, On<KGMonthlyInspectionL.templateLineID,
                Equal<KGMonthlyInspectionTemplateL.templateLineID>>>,
            Where<KGMonthlyInspectionL.monthlyInspectionID,
                Equal<Current<KGMonthlyInspectTicketL.monthlyInspectionID>>>,
            OrderBy<Asc<KGMonthlyInspectionTemplateL.segmentCD>>>),
            typeof(KGMonthlyInspectionTemplateL.segmentCDRead),
            typeof(KGMonthlyInspectionTemplateL.segmentDescr),
            typeof(KGMonthlyInspectionTemplateL.checkItem))]

        [PXUIField(DisplayName = "Check Item")]
        public virtual string CheckItem { get; set; }
        public abstract class checkItem : IBqlField { }
        #endregion
        #region CheckPointDesc
        [PXString(50, IsUnicode = true, InputMask = "")]
        [PXDBScalar(
            typeof(Search2<KGMonthlyInspectionTemplateL.checkPointDesc, InnerJoin<KGMonthlyInspectionL,
                On<KGMonthlyInspectionL.templateLineID, Equal<KGMonthlyInspectionTemplateL.templateLineID>>,
                InnerJoin<KGMonthlyInspectTicketL, On<KGMonthlyInspectTicketL.monthlyInspectionLineID, Equal<KGMonthlyInspectionL.monthlyInspectionLineID>>>>
                , Where<KGMonthlyInspectionL.monthlyInspectionLineID, Equal<KGMonthlyInspectTicketL.monthlyInspectionLineID>>
                >)
        )]
        [PXUIField(DisplayName = "Check Point Desc", IsReadOnly = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual string CheckPointDesc { get; set; }
        public abstract class checkPointDesc : IBqlField { }
        #endregion

        #region ImageUrl
        [PXDBString(255)]
        [PXUIField(DisplayName = "Image")]
        public virtual string ImageUrl { get; set; }
        public abstract class imageUrl : PX.Data.BQL.BqlString.Field<imageUrl> { }
        #endregion

        #region ReviseTestResult
        [PXString(1)]
        [PXUIField(DisplayName = "ReviseTestResult")]
        [KGMonthInspectionResults.List]
        public virtual string ReviseTestResult { get; set; }
        public abstract class reviseTestResult : PX.Data.BQL.BqlString.Field<reviseTestResult> { }
        #endregion
        
        #region ReviseCheckItem
        [PXInt()]
        [PXUIField(DisplayName = "ReviseCheckItem")]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXSelector(typeof(Search2<KGMonthlyInspectionL.monthlyInspectionLineID,
            InnerJoin<KGMonthlyInspection, On<KGMonthlyInspection.monthlyInspectionID, 
                                            Equal<KGMonthlyInspectionL.monthlyInspectionID>>>
            , Where<KGMonthlyInspection.monthlyInspectionID, Equal<Current<KGMonthlyInspectTicketL.monthlyInspectionID>>>>),
            typeof(KGMonthlyInspectionL.checkItem),
            typeof(KGMonthlyInspectionL.checkPointDesc),
            SubstituteKey = typeof(KGMonthlyInspectionL.checkItem),
            DescriptionField = typeof(KGMonthlyInspectionL.checkPointDesc))]

        public virtual int? ReviseMonthlyInspectionLineID { get; set; }
        public abstract class reviseMonthlyInspectionLineID : PX.Data.BQL.BqlInt.Field<reviseMonthlyInspectionLineID> { }
        #endregion
        #region Delete
        [PXBool()]
        [PXUIField(DisplayName = "IsDelete", Visibility = PXUIVisibility.Visible)]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Boolean? IsDelete { get; set; }
        public abstract class isDelete : IBqlField { }
        #endregion

        #region ViewTestResult
        [PXString(1, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Test Result",Enabled =false)]
        [KGMonthInspectionResults.List]
        public virtual string ViewTestResult { get; set; }
        public abstract class viewTestResult : IBqlField { }
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
		[Branch(typeof(KGMonthlyInspectTicket.branchID))]
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