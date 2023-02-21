using System;
using PX.Data;
using PX.Objects.GL;

namespace Kedge.DAC
{
    [Serializable]
    public class KGVoucherDigestSetup : IBqlTable
    {
        #region SetupID
        [PXDBInt()]
        [PXUIField(DisplayName = "Setup ID")]
        [PXDefault(1)]
        public virtual int? SetupID { get; set; }
        public abstract class setupID : PX.Data.BQL.BqlInt.Field<setupID> { }
        #endregion

        #region DigestID
        [PXDBIdentity()]
        [PXUIField(DisplayName = "Digest ID")]
        public virtual int? DigestID { get; set; }
        public abstract class digestID : PX.Data.BQL.BqlInt.Field<digestID> { }
        #endregion

        #region AccountID
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Account ID")]
        [PXSelector(typeof(Search<Account.accountID>),
            typeof(Account.accountCD),
            typeof(Account.description),
            DescriptionField = typeof(Account.description),
            SubstituteKey = typeof(Account.accountCD))]
        public virtual int? AccountID { get; set; }
        public abstract class accountID : PX.Data.BQL.BqlInt.Field<accountID> { }
        #endregion    

        #region AccountCD
        [PXString()]
        [PXUIField(DisplayName = "描述",IsReadOnly =true)]
      
        public virtual string AccountCD { get; set; }
        public abstract class accountCD : PX.Data.BQL.BqlInt.Field<accountCD> { }
        #endregion    


        #region OracleAccountCD
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Oracle Account CD")]
        public virtual string OracleAccountCD { get; set; }
        public abstract class oracleAccountCD : PX.Data.BQL.BqlString.Field<oracleAccountCD> { }
        #endregion

        #region OracleDigestRule
        [PXDBString(240, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Oracle Digest Rule")]
        public virtual string OracleDigestRule { get; set; }
        public abstract class oracleDigestRule : PX.Data.BQL.BqlString.Field<oracleDigestRule> { }
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

        #region Noteid
        [PXNote()]
        [PXUIField(DisplayName = "Noteid")]
        public virtual Guid? Noteid { get; set; }
        public abstract class noteid : PX.Data.BQL.BqlGuid.Field<noteid> { }
        #endregion

        #region Tstamp
        [PXDBTimestamp()]
        [PXUIField(DisplayName = "Tstamp")]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
        #endregion

        //2020/02/25 Add 傳票不帶專案代碼
        #region IsProjectCode
        [PXDBBool()]
        [PXUIField(DisplayName = "傳票不帶專案")]
        [PXDefault(false,PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual bool? IsProjectCode { get; set; }
        public abstract class isProjectCode : IBqlField { }
        #endregion
    }
}