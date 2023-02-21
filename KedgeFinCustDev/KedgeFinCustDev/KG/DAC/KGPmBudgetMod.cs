using System;
using PX.Data;
using PX.Objects.PM;

namespace Kedge.DAC
{
    [Serializable]
    public class KGPmBudgetMod : IBqlTable
    {
        #region PmBudgetModID
        [PXDBIdentity(IsKey = true)]
        [PXUIField(DisplayName = "Pm Budget Mod ID")]
        public virtual int? PmBudgetModID { get; set; }
        public abstract class pmBudgetModID : PX.Data.BQL.BqlInt.Field<pmBudgetModID> { }
        #endregion

        #region ProjectID
        [PXDBInt()]
        [PXDBDefault(typeof(PMProject.contractID))]
        [PXParent(typeof(Select<PMProject,
                            Where<PMProject.contractID,
                            Equal<Current<projectID>>>>))]
        public virtual int? ProjectID { get; set; }
        public abstract class projectID : PX.Data.BQL.BqlInt.Field<projectID> { }
        #endregion

        #region ModifyClass
        [PXDBString(1, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Modify Class")]
        public virtual string ModifyClass { get; set; }
        public abstract class modifyClass : PX.Data.BQL.BqlString.Field<modifyClass> { }
        #endregion

        #region ApprovedDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Approved Date")]
        public virtual DateTime? ApprovedDate { get; set; }
        public abstract class approvedDate : PX.Data.BQL.BqlDateTime.Field<approvedDate> { }
        #endregion

        #region Amount
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Amount")]
        public virtual Decimal? Amount { get; set; }
        public abstract class amount : PX.Data.BQL.BqlDecimal.Field<amount> { }
        #endregion

        #region Remark
        [PXDBString(240, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Remark")]
        public virtual string Remark { get; set; }
        public abstract class remark : PX.Data.BQL.BqlString.Field<remark> { }
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
        [PXUIField(DisplayName = "Created Date Time")]
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
        [PXUIField(DisplayName = "Last Modified Date Time")]
        public virtual DateTime? LastModifiedDateTime { get; set; }
        public abstract class lastModifiedDateTime : PX.Data.BQL.BqlDateTime.Field<lastModifiedDateTime> { }
        #endregion

        #region NoteId
        [PXNote()]
        [PXUIField(DisplayName = "NoteId")]
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