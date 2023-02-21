using System;
using PX.Data;
using PX.Objects.GL;

namespace Kedge.DAC
{
    [Serializable]
    [PXCacheName("KGBudGroup")]
    public class KGBudGroup : IBqlTable
    {
        #region BudGroupID
        [PXDBIdentity(IsKey = true)]
        public virtual int? BudGroupID { get; set; }
        public abstract class budGroupID : PX.Data.BQL.BqlInt.Field<budGroupID> { }
        #endregion

        #region BudGroupNO
        [PXDBInt()]
        [PXUIField(DisplayName = "Bud Group NO")]
        public virtual int? BudGroupNO { get; set; }
        public abstract class budGroupNO : PX.Data.BQL.BqlInt.Field<budGroupNO> { }
        #endregion

        #region BranchID
        [PXDBInt()]
        [PXSelector(typeof(Branch.branchID), SubstituteKey = typeof(Branch.branchCD))]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        [PXUIField(DisplayName = "Branch", Required = true)]
        public virtual int? BranchID { get; set; }
        public abstract class branchID : PX.Data.BQL.BqlInt.Field<branchID> { }
        #endregion

        #region BudGroupName
        [PXDBString(60, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Bud Group Name")]
        public virtual string BudGroupName { get; set; }
        public abstract class budGroupName : PX.Data.BQL.BqlString.Field<budGroupName> { }
        #endregion

        #region IsTravelExpense
        [PXDBBool()]
        [PXUIField(DisplayName = "Is Travel Expense")]
        [PXDefault(typeof(False), PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual bool? IsTravelExpense { get; set; }
        public abstract class isTravelExpense : PX.Data.BQL.BqlBool.Field<isTravelExpense> { }
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

        #region NoteId
        [PXNote()]
        public virtual Guid? NoteId { get; set; }
        public abstract class noteId : PX.Data.BQL.BqlGuid.Field<noteId> { }
        #endregion

        #region Tstamp
        [PXDBTimestamp()]
        [PXUIField(DisplayName = "Tstamp")]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
        #endregion
    }
}