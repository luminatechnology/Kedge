using System;
using PX.Data;
using PX.Objects.CS;
using PX.Objects.AP;
using PX.Objects.GL;


namespace Kedge.DAC
{
      [Serializable]
      public class KGFlowSubAccInv : IBqlTable
      {
        #region Setup
        public abstract class setup : IBqlField
        { }
        [PXString()]
        //CS201010
        [PXDefault("KGFLOWINV")]
        [PXUIField(DisplayName = "Setup")]
        public virtual string Setup { get; set; }
        #endregion

        #region InvID
        [PXDBIdentity(IsKey = true)]
        [PXUIField(DisplayName = "InvID")]
        public virtual int? InvID { get; set; }
        public abstract class invID : PX.Data.BQL.BqlInt.Field<invID> { }
        #endregion

        #region InvUID
        [PXDBString(40, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "InvUID")]
        [AutoNumber(typeof(setup), typeof(AccessInfo.businessDate))]
        public virtual string InvUID { get; set; }
        public abstract class invUID : PX.Data.BQL.BqlString.Field<invUID> { }
        #endregion

        #region AccUID
        [PXDBString(40, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "AccUID")]
        [PXDBDefault(typeof(KGFlowSubAcc.accUID))]
        [PXParent(typeof(Select<KGFlowSubAcc,
                            Where<KGFlowSubAcc.accUID,
                            Equal<Current<KGFlowSubAccInv.accUID>>>>))]
        public virtual string AccUID { get; set; }
        public abstract class accUID : PX.Data.BQL.BqlString.Field<accUID> { }
        #endregion

        #region InvNo
        [PXDBString(14, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Inv No")]
        public virtual string InvNo { get; set; }
        public abstract class invNo : PX.Data.BQL.BqlString.Field<invNo> { }
        #endregion

        #region InvAmt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Inv Amt")]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? InvAmt { get; set; }
        public abstract class invAmt : PX.Data.BQL.BqlDecimal.Field<invAmt> { }
        #endregion

        #region InvTax
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Inv Tax")]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? InvTax { get; set; }
        public abstract class invTax : PX.Data.BQL.BqlDecimal.Field<invTax> { }
        #endregion

        #region InvTotal
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Inv Total")]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? InvTotal { get; set; }
        public abstract class invTotal : PX.Data.BQL.BqlDecimal.Field<invTotal> { }
        #endregion

        #region InvDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Inv Date")]
        public virtual DateTime? InvDate { get; set; }
        public abstract class invDate : PX.Data.BQL.BqlDateTime.Field<invDate> { }
        #endregion

        #region KindName
        [PXDBString(200, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Kind Name")]
        public virtual string KindName { get; set; }
        public abstract class kindName : PX.Data.BQL.BqlString.Field<kindName> { }
        #endregion

        #region InvoiceNo
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Invoice No")]
        public virtual string InvoiceNo { get; set; }
        public abstract class invoiceNo : PX.Data.BQL.BqlString.Field<invoiceNo> { }
        #endregion

        #region Memo
        [PXDBString(25, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Memo")]
        public virtual string Memo { get; set; }
        public abstract class memo : PX.Data.BQL.BqlString.Field<memo> { }
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