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
     * PaymentMethod ��q��(�쥻�N����)/ §��/�{��/�¦�
     * �q�ת��޿����
     * §��/�{��/�¦� : usrIsWriteOff <> True
     * 
     * ===2021/07/02 : 0012108 === Althea
     * TT NMID Selector Add Log
     * 1. �I�ڤ覡��TT��
        KGBillPayment.PaymentMethod = '�״�' �B TT�Ȧ�^�X��log�D �^�Ǧ��\ ��

        2a.�I�ڤ覡���{���P�¦��A�B���p���o�� �|���I�ڪ�
        KGBillPayment.PaymentMethod = '�{��','�¦�'

        AND KGBillPayment.Usriswriteoff <> True
        AND APRegister.Status = Open
        AND APInvoice.DocType = INV

        2b.�I�ڤ覡���{���P�¦��A�B���w�I�ڪ� �|���I�ڪ�
        KGBillPayment.PaymentMethod = '�{��','�¦�'

        AND KGBillPayment.Usriswriteoff <> True
        AND APRegister.Status = Balanced
        AND APInvoice.DocType = PPM

        3a. �I�ڤ覡���D�M��§���A�B���p�� �|���I�ڪ�
        KGBillPayment.PaymentMethod = '§��'
        AND APRegister.ProjectID = PMSetup.NonProjectCode

        AND KGBillPayment.Usriswriteoff <> True
        AND APRegister.Status = Open
        AND APInvoice.DocType = INV


        3b.�I�ڤ覡���D�M��§���A�B���w�I�� �|���I�ڪ�
        KGBillPayment.PaymentMethod = '§��'
        AND APRegister.ProjectID = PMSetup.NonProjectCode

        AND KGBillPayment.Usriswriteoff <> True
        AND APRegister.Status = Balanced
        AND APInvoice.DocType = PPM
     *
     * ===2021/07/07 :0012108 === Althea
     * TT NMID Fix Log
     * 1. �I�ڤ覡��TT��
        KGBillPayment.PaymentMethod = '�״�' �B TT�Ȧ�^�X��log�D �^�Ǧ��\ ��

        2.�I�ڤ覡���{���P�¦��A�B���p���ιw�I�� �|���I�ڪ�
        KGBillPayment.PaymentMethod = '�{��','�¦�'

        AND KGBillPayment.Usriswriteoff <> True
        AND APRegister.Status = Open
        AND APInvoice.DocType = INV OR PPM

        3.�I�ڤ覡���D�M��§���A�B���p���ιw�I�� �|���I�ڪ�
        KGBillPayment.PaymentMethod = '§��'
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