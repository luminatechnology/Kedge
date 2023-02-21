using System;
using Kedge.DAC;
using NM.DAC;
using PX.Data;
using PX.Objects.AP;

namespace Kedge
{
    public class KGBillWriteoffProcess : PXGraph<KGBillWriteoffProcess>
    {
        public PXCancel<KGBillPayment> Cancel;
        public PXProcessingJoin<KGBillPayment,
           InnerJoin<APRegister, On<APRegister.refNbr, Equal<KGBillPayment.refNbr>>,
           InnerJoin<APInvoice, On<APInvoice.refNbr, Equal<KGBillPayment.refNbr>>,
           InnerJoin<NMBankAccount, On<NMBankAccount.bankAccountID, Equal<KGBillPayment.bankAccountID>>>>>,
               Where<APRegisterFinExt.usrIsConfirm, Equal<True>,
                   And<APRegister.docType, In3<APDocType.invoice, APDocType.prepayment>,
                   And<APRegister.status, Equal<APDocStatus.open>,
                   //2021/07/21 Add Mantis: 0012166 
                   //And<KGBillPaymentExt.usrIsTrConfirm,Equal<True>,
                   And2<Where<KGBillPaymentExt.usrIsWriteOff, Equal<False>, Or<KGBillPaymentExt.usrIsWriteOff, IsNull>>,
                   And2<Where<KGBillPaymentExt.isPaymentHold, IsNull, 
                        Or<KGBillPaymentExt.isPaymentHold, Equal<False>>>,
                   And<Where<KGBillPaymentExt.usrIsTrConfirm, Equal<True>,
                        And<Where<KGBillPayment.paymentMethod, In3<PaymentMethod.cash, PaymentMethod.auth>,
                            Or<Where<KGBillPayment.paymentMethod, Equal<PaymentMethod.giftCertificate>,
                                And<APRegister.projectID, Equal<Zero>>>>>>>>>>>>>> INVViews;



        #region Constructor

        public KGBillWriteoffProcess()
        {

            INVViews.SetProcessDelegate(WriteoffPayment);
            INVViews.SetProcessCaption("¨R¾P¥I´Ú");
            INVViews.SetProcessAllCaption("¥þ³¡¨R¾P¥I´Ú");
            INVViews.SetSelected<KGBillPaymentExt.selected>();

        }
        public static void WriteoffPayment(KGBillPayment payment)
        {
            PXGraph.CreateInstance<KGWriteOffProcess>().INVWriteoff(payment);
        }
        #endregion

        #region Overridden Properties

        public override bool IsDirty => false;

        #endregion
    }
}