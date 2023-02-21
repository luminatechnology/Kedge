using System;
using PX.Data;
using PX.Objects.RQ;
using Kedge.DAC;
using PX.Objects.GL;
using PX.Objects.CS;

namespace Kedge.DAC
{
      [Serializable]
      public class KGFlowRequisitionsDetail : IBqlTable
      {
        #region Setup
        public abstract class setup : IBqlField
        { }
        //CS201010
        [PXString()]
        [PXDefault("KGFLOWRED")]
        [PXUIField(DisplayName = "Setup")]
        public virtual string Setup { get; set; }
        #endregion

        #region RedUID
        [PXDBString(40, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Reduid")]
        //¦³°ÝÃD@@
        [AutoNumber(typeof(setup), typeof(AccessInfo.businessDate))]
        public virtual string RedUID { get; set; }
        public abstract class redUID : PX.Data.BQL.BqlString.Field<redUID> { }
        #endregion

        #region RedID

        [PXDBIdentity(IsKey = true)]
        public virtual int? RedID { get; set; }
        public abstract class redID : IBqlField { }
        #endregion

        #region ReqUID
        [PXDBString(40, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Requid")]
        [PXDBDefault(typeof(KGFlowRequisitions.reqUID))]
        [PXParent(typeof(Select<KGFlowRequisitions,
                            Where<KGFlowRequisitions.reqUID,
                            Equal<Current<KGFlowRequisitionsDetail.reqUID>>>>))]
        public virtual string ReqUID { get; set; }
        public abstract class reqUID : PX.Data.BQL.BqlString.Field<reqUID> { }
        #endregion

        #region PccesCode
        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Pcces Code")]
        public virtual string PccesCode { get; set; }
        public abstract class pccesCode : PX.Data.BQL.BqlString.Field<pccesCode> { }
        #endregion

        #region ItemName
        [PXDBString(256, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Item Name")]
        public virtual string ItemName { get; set; }
        public abstract class itemName : PX.Data.BQL.BqlString.Field<itemName> { }
        #endregion

        #region ItemQty
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Item Qty")]
        public virtual Decimal? ItemQty { get; set; }
        public abstract class itemQty : PX.Data.BQL.BqlDecimal.Field<itemQty> { }
        #endregion

        #region ItemUnit
        [PXDBString(6, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Item Unit")]
        public virtual string ItemUnit { get; set; }
        public abstract class itemUnit : PX.Data.BQL.BqlString.Field<itemUnit> { }
        #endregion

        #region ItemCost
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Item Cost")]
        public virtual Decimal? ItemCost { get; set; }
        public abstract class itemCost : PX.Data.BQL.BqlDecimal.Field<itemCost> { }
        #endregion

        #region ItemAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Item Amt")]
        public virtual Decimal? ItemAmt { get; set; }
        public abstract class itemAmt : PX.Data.BQL.BqlDecimal.Field<itemAmt> { }
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
        [Branch(typeof(RQRequest.branchID))]
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