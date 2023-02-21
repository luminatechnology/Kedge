using System;
using EP.Util;
using PX.Data;
using PX.Objects.CR;
using Kedge.DAC;
using PX.Objects.AP;
using NM;
using NM.Util;
using static EP.Util.EPStringList;
using PX.Objects.EP;

namespace EP.DAC
{
    /**
     * ===2021/06/24 : 0012108 ===Althea
     * NMID Selector Modify Filter:
     * PaymentMethod 抓電匯(原本就有的)/ 禮券/現金/授扣
     * 電匯的邏輯照舊
     * 禮券/現金/授扣 : usrIsWriteOff <> True
     * 
     * ===2021/07/02 : 0012108 === Althea
     * TT NMID Selector Add Log
     * 1. 付款方式為TT者
        KGBillPayment.PaymentMethod = '匯款' 且 TT銀行回饋檔log非 回傳成功 者

        2a.付款方式為現金與授扣，且為計價發票 尚未付款者
        KGBillPayment.PaymentMethod = '現金','授扣'

        AND KGBillPayment.Usriswriteoff <> True
        AND APRegister.Status = Open
        AND APInvoice.DocType = INV

        2b.付款方式為現金與授扣，且為預付款者 尚未付款者
        KGBillPayment.PaymentMethod = '現金','授扣'

        AND KGBillPayment.Usriswriteoff <> True
        AND APRegister.Status = Balanced
        AND APInvoice.DocType = PPM

        3a. 付款方式為非專案禮卷，且為計價 尚未付款者
        KGBillPayment.PaymentMethod = '禮券'
        AND APRegister.ProjectID = PMSetup.NonProjectCode

        AND KGBillPayment.Usriswriteoff <> True
        AND APRegister.Status = Open
        AND APInvoice.DocType = INV


        3b.付款方式為非專案禮卷，且為預付款 尚未付款者
        KGBillPayment.PaymentMethod = '禮券'
        AND APRegister.ProjectID = PMSetup.NonProjectCode

        AND KGBillPayment.Usriswriteoff <> True
        AND APRegister.Status = Balanced
        AND APInvoice.DocType = PPM
     *
     * ===2021/07/07 :0012108 === Althea
     * TT NMID Fix Log
     * 1. 付款方式為TT者
        KGBillPayment.PaymentMethod = '匯款' 且 TT銀行回饋檔log非 回傳成功 者

        2.付款方式為現金與授扣，且為計價或預付款 尚未付款者
        KGBillPayment.PaymentMethod = '現金','授扣'

        AND KGBillPayment.Usriswriteoff <> True
        AND APRegister.Status = Open
        AND APInvoice.DocType = INV OR PPM

        3.付款方式為非專案禮卷，且為計價或預付款 尚未付款者
        KGBillPayment.PaymentMethod = '禮券'
        AND APRegister.ProjectID = PMSetup.NonProjectCode

        AND KGBillPayment.Usriswriteoff <> True
        AND APRegister.Status = Open
        AND APInvoice.DocType =( INV OR PPM)
     *
     **/
    [Serializable]
    [PXCacheName("EPPaymentModiReqTT")]
    public class EPPaymentModiReqTT : EPPaymentModiReq
    {
        #region PaymentType
        [PXDBString(3, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Payment Type")]
        [PXDefault(EPStringList.PaymentType.TT)]
        public override string PaymentType { get; set; }
        #endregion

        #region NMID
        [PXDBInt()]
        [PXUIField(DisplayName = "RefNbr",Required =true)]
        [PXSelector(typeof(Search2<KGBillPayment.billPaymentID,
            LeftJoin<NMApTeleTransLog,On<NMApTeleTransLog.billPaymentID,Equal<KGBillPayment.billPaymentID>,
                And<NMApTeleTransLog.isNew,Equal<True>,
                And<NMApTeleTransLog.status,NotEqual<NMStringList.NMApTtLogStatus.feedBackSuccess>>>>,
            InnerJoin<APRegister,On<APRegister.refNbr,Equal<KGBillPayment.refNbr>>,
            LeftJoin<BAccount2,On<BAccount2.bAccountID,Equal<APRegister.vendorID>>>>>,
            Where<KGBillPayment.paymentMethod,Equal<PaymentMethod.wireTransfer>,
                Or2<Where<KGBillPayment.paymentMethod,In3<PaymentMethod.auth,PaymentMethod.cash>,
                        And2<Where<KGBillPaymentExt.usrIsWriteOff,NotEqual<True>,Or<KGBillPaymentExt.usrIsWriteOff,IsNull>>,
                        And<APRegister.docType,In3<APDocType.invoice, APDocType.prepayment>,
                        And<APRegister.status,Equal<APDocStatus.open>>>>>,
                Or<Where<KGBillPayment.paymentMethod, Equal<PaymentMethod.giftCertificate>,
                        And2<Where<KGBillPaymentExt.usrIsWriteOff, NotEqual<True>, Or<KGBillPaymentExt.usrIsWriteOff, IsNull>>,
                        And<APRegister.projectID, Equal<Zero>,
                        And<APRegister.docType, In3<APDocType.invoice,APDocType.prepayment>,
                        And<APRegister.status, Equal<APDocStatus.open>>>>>>>>>>),
            typeof(APRegister.docType),
            typeof(KGBillPayment.refNbr),
            typeof(APRegisterFinExt.usrAccConfirmNbr),
            typeof(KGBillPayment.paymentMethod),
            typeof(KGBillPayment.paymentAmount),
            typeof(BAccount2.acctCD),
            typeof(BAccount2.acctName),
            typeof(KGBillPayment.paymentDate),
            SubstituteKey = typeof(KGBillPayment.refNbr))]
        [PXDefault(PersistingCheck = PXPersistingCheck.NullOrBlank)]
        public override int? NMID { get; set; }
        #endregion

        #region Amt
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Amt", IsReadOnly = true)]
        [PXFormula(null, typeof(SumCalc<EPExpenseClaimExt.usrAmtTT>))]
        public override decimal? Amt { get; set; }
        #endregion
    }
}