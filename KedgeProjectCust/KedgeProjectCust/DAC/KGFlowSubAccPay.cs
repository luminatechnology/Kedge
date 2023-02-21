using System;
using PX.Data;
using PX.Objects.CS;
using PX.Objects.AP;
using PX.Objects.GL;
namespace Kedge.DAC
{
  [Serializable]
  public class KGFlowSubAccPay : IBqlTable
  {
        //
        #region Setup
        public abstract class setup : IBqlField
        { }
        //CS201010
        [PXString()]
        [PXDefault("KGFLOWSAP")]
        [PXUIField(DisplayName = "Setup")]
        public virtual string Setup { get; set; }
        #endregion

        #region SapID
        [PXDBIdentity(IsKey = true)]
        [PXUIField(DisplayName = "Sapid")]
        public virtual int? SapID { get; set; }
        public abstract class sapID : PX.Data.BQL.BqlInt.Field<sapID> { }
        #endregion

        #region SapUIDd
        [AutoNumber(typeof(setup), typeof(AccessInfo.businessDate))]
        [PXDBString(40, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "SapUID")]
        public virtual string SapUID { get; set; }
        public abstract class sapUID : PX.Data.BQL.BqlString.Field<sapUID> { }
        #endregion

        #region AccUID
        [PXDBString(40, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "AccUID")]
        [PXDBDefault(typeof(KGFlowSubAcc.accUID))]
        [PXParent(typeof(Select<KGFlowSubAcc,
                            Where<KGFlowSubAcc.accUID,
                            Equal<Current<KGFlowSubAccMfq.accUID>>>>))]
        public virtual string AccUID { get; set; }
        public abstract class accUID : PX.Data.BQL.BqlString.Field<accUID> { }
        #endregion

        #region CostName
        [PXDBString(50, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Cost Name")]
        public virtual string CostName { get; set; }
        public abstract class costName : PX.Data.BQL.BqlString.Field<costName> { }
        #endregion

        #region PayName
        [PXDBString(200, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Pay Name")]
        public virtual string PayName { get; set; }
        public abstract class payName : PX.Data.BQL.BqlString.Field<payName> { }
        #endregion

        #region PayAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Pay Amt")]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? PayAmt { get; set; }
        public abstract class payAmt : PX.Data.BQL.BqlDecimal.Field<payAmt> { }
        #endregion

        #region Title
        [PXDBString(60, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Title")]
        public virtual string Title { get; set; }
        public abstract class title : PX.Data.BQL.BqlString.Field<title> { }
        #endregion

        #region CashDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Cash Date")]
        public virtual DateTime? CashDate { get; set; }
        public abstract class cashDate : PX.Data.BQL.BqlDateTime.Field<cashDate> { }
        #endregion

        #region AccMemo
        [PXDBString(IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Acc Memo")]
        public virtual string AccMemo { get; set; }
        public abstract class accMemo : PX.Data.BQL.BqlString.Field<accMemo> { }
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
		[Branch(typeof(APRegister.branchID))]
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