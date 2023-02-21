using System;
using PX.Data;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.AP;
namespace Kedge.DAC
{
      [Serializable]
      public class KGFlowSubAccMfq : IBqlTable
      {
        #region Setup
        public abstract class setup : IBqlField
        { }
        [PXString()]
        //CS201010
        [PXDefault("KGFLOWMFQ")]
        [PXUIField(DisplayName = "Setup")]
        public virtual string Setup { get; set; }
        #endregion
        #region MfqID
        [PXDBIdentity(IsKey = true)]
        [PXUIField(DisplayName = "MfqID")]
        public virtual int? MfqID { get; set; }
        public abstract class mfqID : PX.Data.BQL.BqlInt.Field<mfqID> { }
        #endregion

        #region MfqUID
        [PXDBString(40, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "MfqUID")]
        [AutoNumber(typeof(setup), typeof(AccessInfo.businessDate))]
        public virtual string MfqUID { get; set; }
        public abstract class mfqUID : PX.Data.BQL.BqlString.Field<mfqUID> { }
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

        #region ItemName
        [PXDBString(256, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Item Name")]
        public virtual string ItemName { get; set; }
        public abstract class itemName : PX.Data.BQL.BqlString.Field<itemName> { }
        #endregion

        #region PccesCode
        [PXDBString(30, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Pcces Code")]
        public virtual string PccesCode { get; set; }
        public abstract class pccesCode : PX.Data.BQL.BqlString.Field<pccesCode> { }
        #endregion

        #region ItemUnit
        [PXDBString(6, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Item Unit")]
        public virtual string ItemUnit { get; set; }
        public abstract class itemUnit : PX.Data.BQL.BqlString.Field<itemUnit> { }
        #endregion

        #region SubItemQty
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Sub Item Qty")]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? SubItemQty { get; set; }
        public abstract class subItemQty : PX.Data.BQL.BqlDecimal.Field<subItemQty> { }
        #endregion

        #region SubItemCost
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Sub Item Cost")]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? SubItemCost { get; set; }
        public abstract class subItemCost : PX.Data.BQL.BqlDecimal.Field<subItemCost> { }
        #endregion

        #region SubItemAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Sub Item Amt")]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? SubItemAmt { get; set; }
        public abstract class subItemAmt : PX.Data.BQL.BqlDecimal.Field<subItemAmt> { }
        #endregion

        #region AddAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Add Amt")]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? AddAmt { get; set; }
        public abstract class addAmt : PX.Data.BQL.BqlDecimal.Field<addAmt> { }
        #endregion

        #region PreQty
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Pre Qty")]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? PreQty { get; set; }
        public abstract class preQty : PX.Data.BQL.BqlDecimal.Field<preQty> { }
        #endregion

        #region PreAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Pre Amt")]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? PreAmt { get; set; }
        public abstract class preAmt : PX.Data.BQL.BqlDecimal.Field<preAmt> { }
        #endregion

        #region ItemQty
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Item Qty")]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? ItemQty { get; set; }
        public abstract class itemQty : PX.Data.BQL.BqlDecimal.Field<itemQty> { }
        #endregion

        #region ItemCost
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Item Cost")]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? ItemCost { get; set; }
        public abstract class itemCost : PX.Data.BQL.BqlDecimal.Field<itemCost> { }
        #endregion

        #region ItemAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Item Amt")]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? ItemAmt { get; set; }
        public abstract class itemAmt : PX.Data.BQL.BqlDecimal.Field<itemAmt> { }
        #endregion

        #region AccQty
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Acc Qty")]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? AccQty { get; set; }
        public abstract class accQty : PX.Data.BQL.BqlDecimal.Field<accQty> { }
        #endregion

        #region AccAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Acc Amt")]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? AccAmt { get; set; }
        public abstract class accAmt : PX.Data.BQL.BqlDecimal.Field<accAmt> { }
        #endregion

        #region Percentage
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Percentage")]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? Percentage { get; set; }
        public abstract class percentage : PX.Data.BQL.BqlDecimal.Field<percentage> { }
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
        public abstract class noteID : PX.Data.BQL.BqlGuid.Field<noteID>
        {
        }
        protected Guid? _NoteID;

        /// <summary>
        /// Identifier of the <see cref="PX.Data.Note">Note</see> object, associated with the line.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="PX.Data.Note.NoteID">Note.NoteID</see> field. 
        /// </value>
	    [PXNote()]
        public virtual Guid? NoteID
        {
            get
            {
                return this._NoteID;
            }
            set
            {
                this._NoteID = value;
            }
        }
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