using System;
using PX.Data;

namespace NM.DAC
{
    [Serializable]
    [PXCacheName("NMApPaymentProcessLog")]
    public class NMApPaymentProcessLog : IBqlTable
    {
        #region LogID
        [PXDBIdentity(IsKey = true)]
        public virtual int? LogID { get; set; }
        public abstract class logID : PX.Data.BQL.BqlInt.Field<logID> { }
        #endregion

        #region RefNbr
        [PXDBString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "RefNbr")]
        public virtual string RefNbr { get; set; }
        public abstract class refNbr : PX.Data.BQL.BqlString.Field<refNbr> { }
        #endregion

        #region BillPaymentID
        [PXDBInt()]
        [PXUIField(DisplayName = "Bill Payment ID")]
        public virtual int? BillPaymentID { get; set; }
        public abstract class billPaymentID : PX.Data.BQL.BqlInt.Field<billPaymentID> { }
        #endregion

        #region Message
        [PXDBString(400, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Message")]
        public virtual string Message { get; set; }
        public abstract class message : PX.Data.BQL.BqlString.Field<message> { }
        #endregion

        #region CreatedDateTime
        [PXDBCreatedDateTime()]
        public virtual DateTime? CreatedDateTime { get; set; }
        public abstract class createdDateTime : PX.Data.BQL.BqlDateTime.Field<createdDateTime> { }
        #endregion
    }
}