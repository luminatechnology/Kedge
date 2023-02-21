using System;
using PX.Data;
using PX.Objects.CS;
using PX.Objects.GL;

namespace Kedge.DAC
{
  [Serializable]
  public class KGMonthlyInspectionSign : IBqlTable
  {
    #region MonthInspectionSignID
    [PXDBIdentity(IsKey = true)]
    [PXUIField(DisplayName = "Month Inspection Sign ID")]
    public virtual int? MonthInspectionSignID { get; set; }
    public abstract class monthInspectionSignID : PX.Data.BQL.BqlInt.Field<monthInspectionSignID> { }
    #endregion

    #region MonthlyInspectionID
    [PXDBInt()]
    [PXUIField(DisplayName = "Monthly Inspection ID")]
    [PXDBDefault(typeof(KGMonthlyInspection.monthlyInspectionID))]
    [PXParent(typeof(Select<KGMonthlyInspection,
                        Where<KGMonthlyInspection.monthlyInspectionID,
                            Equal<Current<KGMonthlyInspectionL.monthlyInspectionID>>>>))]
    public virtual int? MonthlyInspectionID { get; set; }
    public abstract class monthlyInspectionID : PX.Data.BQL.BqlInt.Field<monthlyInspectionID> { }
    #endregion

    #region SignBy
    [PXDBString(60, IsUnicode = true, InputMask = "")]
    [PXUIField(DisplayName = "Sign By")]
    public virtual string SignBy { get; set; }
    public abstract class signBy : PX.Data.BQL.BqlString.Field<signBy> { }
        #endregion
    #region SeqNo
    [PXDBInt()]
    [PXUIField(DisplayName = "SeqNo")]
  
    public virtual int? SeqNo { get; set; }
    public abstract class seqNo : PX.Data.BQL.BqlInt.Field<seqNo> { }
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
    }
}