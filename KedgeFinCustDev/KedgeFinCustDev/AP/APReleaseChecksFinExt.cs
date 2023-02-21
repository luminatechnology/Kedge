using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using PX.Common;
using PX.Data;
using PX.Objects.GL;
using PX.Objects.CM;
using PX.Objects.CA;
using PX.Objects.CS;
using PX.Objects;
using PX.Objects.AP;
using NM.DAC;
using NM.Util;
using KG.Util;

namespace PX.Objects.AP
{
    public class APReleaseChecksFinExt : PXGraphExtension<APReleaseChecks>
    {
        #region Event Handlers
        protected virtual void ReleaseChecksFilter_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            ReleaseChecksFilter filter = e.Row as ReleaseChecksFilter;
            if (filter == null) return;
            string action = filter.Action;
            Base.APPaymentList.SetProcessDelegate(list => FinReleasePayments(list, action));
        }
        #endregion

        #region Method
        public void FinReleasePayments(List<APPayment> list, string Action)
        {
            using (PXTransactionScope ts = new PXTransactionScope())
            {
                APReleaseChecks.ReleasePayments(list, Action);
                OverrideAPRelease(list);
                ts.Complete();
            }
        }

        public void OverrideAPRelease(List<APPayment> list)
        {
            //Stage1. 原廠先過帳，且產生傳票
            foreach (APPayment apdoc in list)
            {
                if (apdoc == null) continue;
                if (apdoc.DocType != APDocType.Prepayment && apdoc.DocType != APDocType.Check) continue;
                bool isVoid = apdoc.DocType == APDocType.VoidCheck;
                if (apdoc.PaymentMethodID != "CHECK") continue;
                //20220418 by louis NMVoucherUtil.CreateAPVoucher()新增一個參數紀錄傳票產生的時機
                if (isVoid)
                {
                    NMPayableCheck payableCheck = GetPayableCheck(apdoc.RefNbr);
                    PXUpdate<
                        Set<NMPayableCheck.paymentReverseBatchNbr, Required<NMPayableCheck.paymentReverseBatchNbr>>,
                        NMPayableCheck,
                        Where<NMPayableCheck.payableCheckID, Equal<Required<NMPayableCheck.payableCheckID>>>>
                        .Update(Base, NMVoucherUtil.CreateAPVoucher(NMStringList.NMAPVoucher.PAYMENTREVERSE, payableCheck, GLStageCode.NMPVoidPB), 
                                payableCheck.PayableCheckID);
                }
                else
                {
                    NMPayableCheck payableCheck = GetPayableCheck(apdoc.RefNbr);
                    PXUpdate<
                        Set<NMPayableCheck.paymentBatchNbr, Required<NMPayableCheck.paymentBatchNbr>>,
                        NMPayableCheck,
                        Where<NMPayableCheck.payableCheckID, Equal<Required<NMPayableCheck.payableCheckID>>>>
                        .Update(Base, NMVoucherUtil.CreateAPVoucher(NMStringList.NMAPVoucher.PAYMENT, payableCheck, GLStageCode.NMPConfirmP2),
                        payableCheck.PayableCheckID);
                }


            }
        }

        public NMPayableCheck GetPayableCheck(string APPRefNbr)
        {
            return PXSelect<NMPayableCheck,
                Where<NMPayableCheck.paymentDocNo, Equal<Required<NMPayableCheck.paymentDocNo>>>>
                .Select(Base, APPRefNbr);
        }
        #endregion
    }
}