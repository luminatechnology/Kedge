using System;
using PX.Data;
using PX.Objects.CT;
using PX.Objects.PM;
using PX.Objects.GL;

namespace Kedge.DAC
{
  [Serializable]
  public class KGDailyNote : IBqlTable
  {
    #region Selected
    public abstract class selected : IBqlField
    { }
    [PXBool]
    [PXUIField(DisplayName = "Selected")]
    public virtual bool? Selected { get; set; }
    #endregion
  
    #region DailyNoteID
    [PXDBIdentity(IsKey = true)]
    [PXUIField(DisplayName = "Daily Note ID")]
    public virtual int? DailyNoteID { get; set; }
    public abstract class dailyNoteID : IBqlField { }
    #endregion

    #region DailyReportID
    [PXDBInt()]
    [PXUIField(DisplayName = "Daily Report ID")]
    [PXDBDefault(typeof(KGDailyReport.dailyReportID))]
    [PXParent(typeof(Select<KGDailyReport,
                        Where<KGDailyReport.dailyReportID,
                        Equal<Current<KGDailyNote.dailyReportID>>>>))]
    public virtual int? DailyReportID { get; set; }
    public abstract class dailyReportID : IBqlField { }
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