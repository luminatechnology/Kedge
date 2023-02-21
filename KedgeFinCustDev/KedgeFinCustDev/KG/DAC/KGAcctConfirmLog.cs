using System;
using PX.Data;

namespace Kedge.DAC
{
    [Serializable]
    [PXCacheName("KGAcctConfirmLog")]
    public class KGAcctConfirmLog : IBqlTable
    {
        #region LogID
        [PXDBIdentity()]
        public virtual int? LogID { get; set; }
        public abstract class logID : PX.Data.BQL.BqlInt.Field<logID> { }
        #endregion

        #region Module
        [PXDBString(4, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Module")]
        public virtual string Module { get; set; }
        public abstract class module : PX.Data.BQL.BqlString.Field<module> { }
        #endregion

        #region RefNbr
        [PXDBString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Ref Nbr")]
        public virtual string RefNbr { get; set; }
        public abstract class refNbr : PX.Data.BQL.BqlString.Field<refNbr> { }
        #endregion

        #region DocType
        [PXDBString(3, IsFixed = true, InputMask = "")]
        [PXUIField(DisplayName = "Doc Type")]
        public virtual string DocType { get; set; }
        public abstract class docType : PX.Data.BQL.BqlString.Field<docType> { }
        #endregion

        #region ConfirmBy
        [PXDBGuid()]
        [PXUIField(DisplayName = "Confirm By")]
        public virtual Guid? ConfirmBy { get; set; }
        public abstract class confirmBy : PX.Data.BQL.BqlGuid.Field<confirmBy> { }
        #endregion

        #region ConfirmDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Confirm Date")]
        public virtual DateTime? ConfirmDate { get; set; }
        public abstract class confirmDate : PX.Data.BQL.BqlDateTime.Field<confirmDate> { }
        #endregion

        #region OrigAccConfirmNbr
        [PXDBString(14, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Orig Acc Confirm Nbr")]
        public virtual string OrigAccConfirmNbr { get; set; }
        public abstract class origAccConfirmNbr : PX.Data.BQL.BqlString.Field<origAccConfirmNbr> { }
        #endregion

        #region NewAccConfirmNbr
        [PXDBString(14, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "New Acc Confirm Nbr")]
        public virtual string NewAccConfirmNbr { get; set; }
        public abstract class newAccConfirmNbr : PX.Data.BQL.BqlString.Field<newAccConfirmNbr> { }
        #endregion

        #region Tstamp
        [PXDBTimestamp()]
        [PXUIField(DisplayName = "Tstamp")]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
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