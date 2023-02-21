using System;
using PX.Data;
using PX.Objects.CT;
using PX.Objects.PM;
using PX.Objects.GL;

namespace Kedge.DAC
{
  [Serializable]
  public class KGDailyTask : IBqlTable
  {
    
        #region Selected
        public abstract class selected : IBqlField
        { }
        [PXBool]
        [PXUIField(DisplayName = "Selected")]
        public virtual bool? Selected { get; set; }
        #endregion
    
        #region DailyTaskID
        [PXDBIdentity(IsKey = true)]
        [PXUIField(DisplayName = "Daily Task ID")]
        public virtual int? DailyTaskID { get; set; }
        public abstract class dailyTaskID : IBqlField { }
        #endregion

        #region DailyReportID
        [PXCheckUnique(Where = typeof(Where<dailyTaskID, Equal<Current<dailyTaskID>>>))]
        //[PXDBInt(IsKey = true)]
        [PXDBInt()]
        [PXUIField(DisplayName = "Daily Report ID")]
        [PXDBDefault(typeof(KGDailyReport.dailyReportID))]
        [PXParent(typeof(Select<KGDailyReport,
                                Where<KGDailyReport.dailyReportID,
                                Equal<Current<KGDailyTask.dailyReportID>>>>))]
        public virtual int? DailyReportID { get; set; }
        public abstract class dailyReportID : IBqlField { }
        #endregion

        #region ProjectTaskID
        //[PXDBInt(IsKey = true)]
        [PXDBInt()]
        [PXUIField(DisplayName = "Project Task ID", Enabled = false)]
        [PXDimensionSelector("PROTASK", typeof(Search<PMTask.taskID,
                    Where<PMTask.projectID, Equal<Current<KGDailyReport.contractID>>>>), typeof(PMTask.taskCD), typeof(PMTask.description),
                typeof(PMTask.status), typeof(PMTask.isDefault), DescriptionField = typeof(PMTask.description))]
        [PXCheckUnique(Where = typeof(Where<dailyReportID, Equal<Current<dailyReportID>>>))]
        public virtual int? ProjectTaskID { get; set; }
        public abstract class projectTaskID : IBqlField { }
        #endregion
        /*
        #region ScheduleStartDate
        [PXDate()]
        [PXUIField(DisplayName = "Schedule Start Date", Enabled = false)]
        public virtual DateTime? ScheduleStartDate { get; set; }
        public abstract class scheduleStartDate : IBqlField { }
        #endregion

        #region ScheduleEndDate
        [PXDate()]
        [PXUIField(DisplayName = "Schedule End Date", Enabled = false)]
        public virtual DateTime? ScheduleEndDate { get; set; }
        public abstract class scheduleEndDate : IBqlField { }
        #endregion*/

        #region SchedulePercent
        [PXDBDecimal(2)]
        [PXUIField(DisplayName = "Schedule Percent", Enabled = false)]
        public virtual Decimal? SchedulePercent { get; set; }
        public abstract class schedulePercent : IBqlField { }
        #endregion

        #region ActualStartDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Actual Start Date", Enabled = false)]
        public virtual DateTime? ActualStartDate { get; set; }
        public abstract class actualStartDate : IBqlField { }
        #endregion
      
        #region ActualEndDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Actual End Date", Enabled = false)]
        public virtual DateTime? ActualEndDate { get; set; }
        public abstract class actualEndDate : IBqlField { }
        #endregion

        #region ActualPercent
        [PXDBDecimal(2)]
        [PXUIField(DisplayName = "Actual Percent", Enabled = false)]
        public virtual Decimal? ActualPercent { get; set; }
        public abstract class actualPercent : IBqlField { }
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
		[Branch(typeof(KGDailyReport.branchID))]
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