using System;
using PX.Data;

namespace CC.DAC
{
    [Serializable]
    [PXCacheName("CCReceivableCheckPostponeLog")]
    public class CCReceivableCheckPostponeLog : IBqlTable
    {
        #region GuarReceivableID
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Guar Receivable ID")]
        [PXDBDefault(typeof(CCReceivableCheck.guarReceviableID))]
        [PXParent(typeof(Select<CCReceivableCheck,
                                Where<CCReceivableCheck.guarReceviableID,
                                Equal<Current<guarReceivableID>>>>))]
        public virtual int? GuarReceivableID { get; set; }
        public abstract class guarReceivableID : PX.Data.BQL.BqlInt.Field<guarReceivableID> { }
        #endregion

        #region PostponeSeq
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Postpone Seq")]
        public virtual int? PostponeSeq { get; set; }
        public abstract class postponeSeq : PX.Data.BQL.BqlInt.Field<postponeSeq> { }
        #endregion

        #region PostponeDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Postpone Date")]
        public virtual DateTime? PostponeDate { get; set; }
        public abstract class postponeDate : PX.Data.BQL.BqlDateTime.Field<postponeDate> { }
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
    }
}