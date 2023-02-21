using PX.Data;
using System;

namespace PX.Objects.GL
{
    public class GLTranExt : PXCacheExtension<PX.Objects.GL.GLTran>
    {
        #region UsrBillPaymentID
        [PXDBInt()]
        [PXUIField(DisplayName = "Bill Payment ID", Visible = false)]
        public virtual int? UsrBillPaymentID { get; set; }
        public abstract class usrBillPaymentID : PX.Data.BQL.BqlInt.Field<usrBillPaymentID> { }
        #endregion

        #region UsrPaymentDate
        [PXDBDate]
        [PXUIField(DisplayName = "Payment Date")]

        public virtual DateTime? UsrPaymentDate { get; set; }
        public abstract class usrPaymentDate : PX.Data.BQL.BqlInt.Field<usrPaymentDate> { }
        #endregion

        #region Unbound Fields

        #region UsrRmngCreditAmt
        [PXDecimal()]
        [PXUIField(DisplayName = "Remaining Crdit Amt", Enabled = false)]
        public virtual decimal? UsrRmngCreditAmt { get; set; }
        public abstract class usrRmngCreditAmt : PX.Data.BQL.BqlDecimal.Field<usrRmngCreditAmt> { }
        #endregion

        #region UsrRmngDebitAmt
        [PXDecimal()]
        [PXUIField(DisplayName = "Remaining Debit Amt", Enabled = false)]
        public virtual decimal? UsrRmngDebitAmt { get; set; }
        public abstract class usrRmngDebitAmt : PX.Data.BQL.BqlDecimal.Field<usrRmngDebitAmt> { }
        #endregion

        #region UsrSetldCreditAmt
        [PXDecimal()]
        [PXUIField(DisplayName = "Settle Credit Amt")]
        public virtual decimal? UsrSetldCreditAmt { get; set; }
        public abstract class usrSetldCreditAmt : PX.Data.BQL.BqlDecimal.Field<usrSetldCreditAmt> { }
        #endregion

        #region UsrSetldDebitAmt
        [PXDecimal()]
        [PXUIField(DisplayName = "Settle Debit Amt")]
        public virtual decimal? UsrSetldDebitAmt { get; set; }
        public abstract class usrSetldDebitAmt : PX.Data.BQL.BqlDecimal.Field<usrSetldDebitAmt> { }
        #endregion

        #endregion
    }
}
