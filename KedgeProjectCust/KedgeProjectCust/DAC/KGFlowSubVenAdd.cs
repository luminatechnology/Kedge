using System;
using PX.Data;
using PX.Objects.CS;
using PX.Objects.AP;
using PX.Objects.GL;

namespace Kedge.DAC
{
      [Serializable]
      public class KGFlowSubVenAdd : IBqlTable
      {
        #region Setup
        public abstract class setup : IBqlField
        { }
        [PXString()]
        //CS201010
        [PXDefault("KGFLOWVEN")]
        [PXUIField(DisplayName = "Setup")]
        public virtual string Setup { get; set; }
        #endregion

        #region VenID
        [PXDBIdentity(IsKey = true)]
        [PXUIField(DisplayName = "VenID")]
        public virtual int? VenID { get; set; }
        public abstract class venID : PX.Data.BQL.BqlInt.Field<venID> { }
        #endregion

        #region VenUID
        [PXDBString(40, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "VenUID")]
        [AutoNumber(typeof(setup), typeof(AccessInfo.businessDate))]
        public virtual string VenUID { get; set; }
        public abstract class venUID : PX.Data.BQL.BqlString.Field<venUID> { }
        #endregion

        #region AccUID
        [PXDBString(40, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "AccUID")]
        [PXDBDefault(typeof(KGFlowSubAcc.accUID))]
        [PXParent(typeof(Select<KGFlowSubAcc,
                            Where<KGFlowSubAcc.accUID,
                            Equal<Current<KGFlowSubAccVenDed.accUID>>>>))]
        public virtual string AccUID { get; set; }
        public abstract class accUID : PX.Data.BQL.BqlString.Field<accUID> { }
        #endregion

        #region VenName
        [PXDBString(200, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Ven Name")]
        public virtual string VenName { get; set; }
        public abstract class venName : PX.Data.BQL.BqlString.Field<venName> { }
        #endregion

        #region VenUnit
        [PXDBString(20, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Ven Unit")]
        public virtual string VenUnit { get; set; }
        public abstract class venUnit : PX.Data.BQL.BqlString.Field<venUnit> { }
        #endregion

        #region AddSubCode
        [PXDBString(64, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Add Sub Code")]
        public virtual string AddSubCode { get; set; }
        public abstract class addSubCode : PX.Data.BQL.BqlString.Field<addSubCode> { }
        #endregion

        #region AddNum
        [PXDBInt()]
        [PXUIField(DisplayName = "Add Num")]
        public virtual int? AddNum { get; set; }
        public abstract class addNum : PX.Data.BQL.BqlInt.Field<addNum> { }
        #endregion

        #region AddValue
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Add Value")]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Decimal? AddValue { get; set; }
        public abstract class addValue : PX.Data.BQL.BqlDecimal.Field<addValue> { }
        #endregion

        #region VenDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Ven Date")]
        public virtual DateTime? VenDate { get; set; }
        public abstract class venDate : PX.Data.BQL.BqlDateTime.Field<venDate> { }
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