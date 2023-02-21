using System;
using NM.Util;
using PX.Data;

namespace NM
{
    [Serializable]
    [PXCacheName("NMApTeleTransLog")]
    public class NMApTeleTransLog : IBqlTable
    {
        #region LogID
        [PXDBIdentity(IsKey = true)]
        [PXUIField(DisplayName = "LogID")]
        public virtual int? LogID { get; set; }
        public abstract class logID : PX.Data.BQL.BqlInt.Field<logID> { }
        #endregion

        #region BillPaymentID
        [PXDBInt()]
        [PXUIField(DisplayName = "Bill Payment ID")]
        public virtual int? BillPaymentID { get; set; }
        public abstract class billPaymentID : PX.Data.BQL.BqlInt.Field<billPaymentID> { }
        #endregion

        #region VendorLocationID
        [PXDBInt()]
        [PXUIField(DisplayName = "Vendor Location ID")]
        public virtual int? VendorLocationID { get; set; }
        public abstract class vendorLocationID : PX.Data.BQL.BqlInt.Field<vendorLocationID> { }
        #endregion

        #region BankAccountID
        [PXDBInt()]
        [PXUIField(DisplayName = "Bank Account ID")]
        public virtual int? BankAccountID { get; set; }
        public abstract class bankAccountID : PX.Data.BQL.BqlInt.Field<bankAccountID> { }
        #endregion

        #region TeleTransRefNbr
        [PXDBString(25, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Tele Trans Ref Nbr")]
        public virtual string TeleTransRefNbr { get; set; }
        public abstract class teleTransRefNbr : PX.Data.BQL.BqlString.Field<teleTransRefNbr> { }
        #endregion

        #region Type
        [PXDBInt()]
        [PXUIField(DisplayName = "Type")]
        [NMStringList.NMApTtLogType]
        public virtual int? Type { get; set; }
        public abstract class type : PX.Data.BQL.BqlInt.Field<type> { }
        #endregion

        #region CustNbr
        /// <summary>
        /// 用戶自訂序號<br></br>
        /// 台新電匯<br></br>
        /// </summary>
        [PXDBString(IsUnicode = true)]
        [PXUIField(DisplayName = "Cust Nbr")]
        public virtual string CustNbr { get; set; }
        public abstract class custNbr : PX.Data.BQL.BqlString.Field<custNbr> { }
        #endregion

        #region Status
        [PXDBInt()]
        [PXUIField(DisplayName = "Status")]
        [NMStringList.NMApTtLogStatus]
        public virtual int? Status { get; set; }
        public abstract class status : PX.Data.BQL.BqlInt.Field<status> { }
        #endregion

        #region ReturnMsg
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Return Msg")]
        public virtual string ReturnMsg { get; set; }
        public abstract class returnMsg : PX.Data.BQL.BqlString.Field<returnMsg> { }
        #endregion

        #region IsNew
        [PXDBBool()]
        [PXUIField(DisplayName = "Is New")]
        public virtual bool? IsNew { get; set; }
        public abstract class isNew : PX.Data.BQL.BqlBool.Field<isNew> { }
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