using PX.Data;

namespace PX.Objects.AP
{
    public class APPaymentExt : PXCacheExtension<APPayment>
    {
        #region UsrBillPaymentID
        [PXDBInt()]
        [PXUIField(DisplayName = "Bill Payment ID", Visible = false)]
        public virtual int? UsrBillPaymentID { get; set; }
        public abstract class usrBillPaymentID : PX.Data.BQL.BqlInt.Field<usrBillPaymentID> { }
        #endregion

        #region UsrTTUploadKey
        [PXDBString()]
        [PXUIField(DisplayName = "TTUploadKey", Visible = false)]
        public virtual string UsrTTUploadKey { get; set; }
        public abstract class usrTTUploadKey : PX.Data.BQL.BqlString.Field<usrTTUploadKey> { }
        #endregion
    }
}
