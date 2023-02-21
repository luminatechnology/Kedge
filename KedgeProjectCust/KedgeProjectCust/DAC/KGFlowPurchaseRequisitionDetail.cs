using System;
using PX.Data;
using PX.Objects.GL;
namespace Kedge.DAC
{
    [Serializable]
    public class KGFlowPurchaseRequisitionDetail : IBqlTable
    {
        #region PprID
        [PXDBIdentity(IsKey = true)]
        public virtual int? PprID { get; set; }
        public abstract class pprID : PX.Data.BQL.BqlString.Field<pprID> { }
        #endregion
        
        #region PprUID
        [PXDBString(40, IsKey = true, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "PprUID")]
        public virtual string PprUID { get; set; }
        public abstract class pprUID : PX.Data.BQL.BqlString.Field<pprUID> { }
        #endregion

        #region PpmUID
        [PXDBString(40, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "PpmUID")]
        public virtual string PpmUID { get; set; }
        public abstract class ppmUID : PX.Data.BQL.BqlString.Field<ppmUID> { }
        #endregion

        #region ReqUID
        [PXDBString(40, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "ReqUID")]
        public virtual string ReqUID { get; set; }
        public abstract class reqUID : PX.Data.BQL.BqlString.Field<reqUID> { }
        #endregion

        #region PurchaseProgramNo
        [PXDBString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Purchase Program No")]
        public virtual string PurchaseProgramNo { get; set; }
        public abstract class purchaseProgramNo : PX.Data.BQL.BqlString.Field<purchaseProgramNo> { }
        #endregion

        #region RequisitionsNo
        [PXDBString(20, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Requisitions No")]
        public virtual string RequisitionsNo { get; set; }
        public abstract class requisitionsNo : PX.Data.BQL.BqlString.Field<requisitionsNo> { }
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
        /*
        #region BranchID
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
		[Branch()]
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