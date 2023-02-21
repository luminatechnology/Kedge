using System;
using PX.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Objects.GL;
namespace Kedge.DAC
{
    [Serializable]
    public class KGMonthlyInspectTicketLFileUpdate : KGMonthlyInspectTicketLFile
    {
        
        #region TestResult
        [PXDBString(1, IsFixed = true, IsUnicode = true, InputMask = "", IsKey = true)]
        [PXUIField(DisplayName = "Test Result", TabOrder = 0)]
        [KGMonthInspectionResults.List]
        public new virtual string TestResult { get; set; }
        public new abstract class testResult : PX.Data.BQL.BqlString.Field<testResult> { }
        #endregion

        #region MonthlyInspectionCD
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Monthly Inspection CD", TabOrder = 1)]
        public new virtual int? MonthlyInspectionID { get; set; }
        public new abstract class monthlyInspectionID : PX.Data.BQL.BqlInt.Field<monthlyInspectionID> { }
        #endregion

        #region MonthlyInspectTicketLineID
        [PXDBInt()]
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

        #region IssueDesc
        [PXDBString(1000, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Issue Desc")]
        [PXDefault("無資料")]
        public new virtual string IssueDesc { get; set; }
        public new abstract class issueDesc : PX.Data.BQL.BqlString.Field<issueDesc> { }
        #endregion


    }

    [Serializable]
    public class KGMonthlyInspectTicketLFileUpdateAll : KGMonthlyInspectTicketLFile
    {

        #region MonthlyInspectionCD
        [PXDBInt(IsKey =true)]
        [PXUIField(DisplayName = "Monthly Inspection CD", TabOrder = 0)]
        public new virtual int? MonthlyInspectionID { get; set; }
        public new abstract class monthlyInspectionID : PX.Data.BQL.BqlInt.Field<monthlyInspectionID> { }
        #endregion
        #region MonthlyInspectTicketLineID
        [PXDBInt()]
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

        #region IssueDesc
        [PXDBString(1000, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Issue Desc",Enabled =false)]
        [PXDefault("無資料")]
        public new virtual string IssueDesc { get; set; }
        public new abstract class issueDesc : PX.Data.BQL.BqlString.Field<issueDesc> { }
        #endregion
    }

    public class CheckItemAttribute : PXIntListAttribute, IPXRowSelectedSubscriber
    {
        //public  String type{ get; set; }
 
        public CheckItemAttribute()
            : base()
        {

            //this._AllowedLabels = GVLookUpCodeUtil.getValue(typeName);
            //this._AllowedValues = GVLookUpCodeUtil.getKey(typeName);

        }
        public void RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            Object row = e.Row ;
            PXGraph graph = new PXGraph();
            int? cd = null;
            if (typeof(KGMonthlyInspectTicketLFileUpdateAll).IsInstanceOfType(row))
            {
                KGMonthlyInspectTicketLFileUpdateAll updateAll = row as KGMonthlyInspectTicketLFileUpdateAll;

                cd = updateAll.MonthlyInspectionID;
            } else if (typeof(KGMonthlyInspectTicketLFileUpdate).IsInstanceOfType(row)) {
                KGMonthlyInspectTicketLFileUpdate update = row as KGMonthlyInspectTicketLFileUpdate;
                cd = update.MonthlyInspectionID;
            }
            PXResultset<KGMonthlyInspectionL> set = PXSelectJoin<KGMonthlyInspectionL,
            InnerJoin<KGMonthlyInspection, On<KGMonthlyInspection.monthlyInspectionID, Equal<KGMonthlyInspectionL.monthlyInspectionID>>>
            , Where<KGMonthlyInspection.monthlyInspectionID, Equal<Required<KGMonthlyInspectTicketLFile.monthlyInspectionID>>>>.Select(graph, cd);
            Dictionary<int, String> list = new Dictionary<int, String>();

            foreach (KGMonthlyInspectionL kgMonthlyInspectionL in set) {
                list.Add(kgMonthlyInspectionL.MonthlyInspectionLineID.Value, kgMonthlyInspectionL.CheckItem);
            }
            PXIntListAttribute.SetList<KGMonthlyInspectTicketLFile.reviseMonthlyInspectionLineID>(sender, row, list.Keys.ToArray(), list.Values.ToArray());
           
        }
    }

    [Serializable]
    public class KGMonthlyInspectTicketLFile : IBqlTable
    {
        #region FileID
        [PXDBIdentity(IsKey = true)]
        [PXUIField(DisplayName = "Fileid")]
        public virtual int? FileID { get; set; }
        public abstract class fileID : PX.Data.BQL.BqlInt.Field<fileID> { }
        #endregion

        #region MonthlyInspectTicketLineID
        [PXDBInt()]
        [PXUIField(DisplayName = "Monthly Inspect Ticket Line ID")]
        
        [PXParent(typeof(Select<KGMonthlyInspectTicketL,
                                Where<KGMonthlyInspectTicketL.monthlyInspectTicketLineID,
                                    Equal<Current<KGMonthlyInspectTicketLFile.monthlyInspectTicketLineID>>,
                                And<KGMonthlyInspectTicketL.monthlyInspectTicketID, 
                                    Equal<Current<KGMonthlyInspectTicketLFile.monthlyInspectTicketID>>
                                    ,And<KGMonthlyInspectTicketL.monthlyInspectionLineID,
                                        Equal<Current<KGMonthlyInspectTicketLFile.monthlyInspectionLineID>>>>>>))]
        [PXDBDefault(typeof(KGMonthlyInspectTicketL.monthlyInspectTicketLineID))]
        public virtual int? MonthlyInspectTicketLineID { get; set; }
        public abstract class monthlyInspectTicketLineID : PX.Data.BQL.BqlInt.Field<monthlyInspectTicketLineID> { }
        #endregion

        #region MonthlyInspectTicketID
        [PXDBInt()]
        [PXUIField(DisplayName = "Monthly Inspect Ticket ID")]
        [PXDBDefault(typeof(KGMonthlyInspectTicketL.monthlyInspectTicketID))]
        public virtual int? MonthlyInspectTicketID { get; set; }
        public abstract class monthlyInspectTicketID : PX.Data.BQL.BqlInt.Field<monthlyInspectTicketID> { }
        #endregion
        
        #region MonthlyInspectionLineID
        [PXDBInt()]
        [PXUIField(DisplayName = "Monthly Inspection Line ID")]
        //[PXDBDefault(typeof(KGMonthlyInspectTicketL.monthlyInspectionLineID))]
        public virtual int? MonthlyInspectionLineID { get; set; }
        public abstract class monthlyInspectionLineID : PX.Data.BQL.BqlInt.Field<monthlyInspectionLineID> { }
        #endregion
        #region TestResult
        [PXDBString(1, IsFixed = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Test Result")]
        [KGMonthInspectionResults.List]
        public virtual string TestResult { get; set; }
        public abstract class testResult : PX.Data.BQL.BqlString.Field<testResult> { }
        #endregion
        
        #region ReviseTestResult
        [PXString(1)]
        [PXUIField(DisplayName = "Revise Test Result")]
        [KGMonthInspectionResults.List]
        public virtual string ReviseTestResult { get; set; }
        public abstract class reviseTestResult : PX.Data.BQL.BqlString.Field<reviseTestResult> { }
        #endregion

        #region ReviseCheckItem
        [PXInt()]
        [PXUIField(DisplayName = "Revise Check Item")]
        [PXDefault( PersistingCheck = PXPersistingCheck.Nothing)]
        [PXSelector(typeof(Search2<KGMonthlyInspectionL.monthlyInspectionLineID,
            InnerJoin<KGMonthlyInspection,On<KGMonthlyInspection.monthlyInspectionID,Equal<KGMonthlyInspectionL.monthlyInspectionID>>>
            ,Where<KGMonthlyInspection.monthlyInspectionID,Equal<Current<KGMonthlyInspectTicketLFile.monthlyInspectionID>>>>),
            typeof(KGMonthlyInspectionL.checkItem),
            typeof(KGMonthlyInspectionL.checkPointDesc),
            SubstituteKey = typeof(KGMonthlyInspectionL.checkItem),
            DescriptionField = typeof(KGMonthlyInspectionL.checkPointDesc))]
        /*
        [PXSelector(typeof(Search2<KGMonthlyInspectionL.monthlyInspectionLineID,
            InnerJoin<KGMonthlyInspection, On<KGMonthlyInspection.monthlyInspectionID, Equal<KGMonthlyInspectionL.monthlyInspectionID>>>>),
            typeof(KGMonthlyInspectionL.checkItem),
            typeof(KGMonthlyInspectionL.checkPointDesc))]*/
        //[CheckItemAttribute()]
        //[PXIntList()]
        public virtual int? ReviseMonthlyInspectionLineID { get; set; }
        public abstract class reviseMonthlyInspectionLineID : PX.Data.BQL.BqlInt.Field<reviseMonthlyInspectionLineID> { }
        #endregion
        #region Delete
        [PXBool()]
        [PXUIField(DisplayName = "Is Delete", Visibility = PXUIVisibility.Visible)]
        [PXDefault(false,PersistingCheck =PXPersistingCheck.Nothing)]
        public virtual Boolean? IsDelete { get; set; }
        public abstract class isDelete : IBqlField { }
        #endregion

        #region MonthlyInspectionCD
        [PXDBInt()]
        [PXUIField(DisplayName = "Monthly Inspection CD")]
        [PXDBDefault(typeof(KGMonthlyInspectTicketL.monthlyInspectionID))]
        public  virtual int? MonthlyInspectionID { get; set; }
        public  abstract class monthlyInspectionID : PX.Data.BQL.BqlInt.Field<monthlyInspectionID> { }
        #endregion

        #region CheckItem
        [PXString(50, IsUnicode = true, InputMask = "")]
        /*
        [PXDBScalar(
            typeof(Search<KGMonthlyInspectTicketL.checkItem,
                Where<KGMonthlyInspectTicketL.monthlyInspectTicketLineID, Equal<Current<KGMonthlyInspectTicketLFile.monthlyInspectionLineID>>>>)
        )]*/
        /*
        [PXDBScalar(
            typeof(Search2<KGMonthlyInspectionTemplateL.checkItem, InnerJoin<KGMonthlyInspectionL,
                On<KGMonthlyInspectionL.templateLineID, Equal<KGMonthlyInspectionTemplateL.templateLineID>>,
                InnerJoin<KGMonthlyInspectTicketLFile, On<KGMonthlyInspectionL.monthlyInspectionLineID,
                    Equal<KGMonthlyInspectTicketLFile.monthlyInspectionLineID>>>>,
                Where<KGMonthlyInspectTicketLFile.monthlyInspectionLineID,
                    Equal<Current<KGMonthlyInspectTicketLFile.monthlyInspectionLineID>>>>)
        )]*/
        [PXUIField(DisplayName = "Check Item", IsReadOnly = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]

        public virtual string CheckItem { get; set; }
        public abstract class checkItem : IBqlField { }
        #endregion

        /*
        #region TemplateLineID
        [PXInt()]
        [PXUIField(DisplayName = "Template Line ID")]
        [PXDBScalar(
            typeof(Search<KGMonthlyInspectTicketL.templateLineID,
                Where<KGMonthlyInspectTicketL.monthlyInspectTicketLineID,
                    Equal<Current<KGMonthlyInspectTicketLFile.monthlyInspectionLineID>>>>)
        )]
        public virtual int? TemplateLineID { get; set; }
        public abstract class templateLineID : IBqlField { }
        #endregion*/

        #region IssueDesc
        [PXDBString(1000, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Issue Desc")]
        public virtual string IssueDesc { get; set; }
        public abstract class issueDesc : PX.Data.BQL.BqlString.Field<issueDesc> { }
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
        #region ImageUrl
        [PXDBString(255)]
        [PXUIField(DisplayName = "Image")]
        public virtual string ImageUrl { get; set; }
        public abstract class imageUrl : PX.Data.IBqlField { }
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