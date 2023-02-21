using System;
using System.Collections;
using System.Collections.Generic;
using PX.Data;
using NM.DAC;
using NM.Util;
using KG.Util;

namespace PX.Objects.AP
{
    public class APPaymentEntryFinExt : PXGraphExtension<APPaymentEntry>
    {
        #region Event Handlers

        #endregion

        #region Action

        #region Override Release
        public delegate IEnumerable ReleaseDelegate(PXAdapter adapter);
        [PXOverride]
        public IEnumerable Release(PXAdapter adapter, ReleaseDelegate baseMethod)
        {
            baseMethod(adapter);
            APPayment apdoc = Base.Document.Current;
            //List<APRegister> list = BaseRelease(adapter);
            /* foreach (APPayment apdoc in adapter.Get<APPayment>())
             {*/
            if (apdoc == null) return adapter.Get();
            if (apdoc.DocType != APDocType.Prepayment && apdoc.DocType != APDocType.Check && apdoc.DocType != APDocType.VoidCheck) return adapter.Get();
            if (apdoc.PaymentMethodID != "CHECK") return adapter.Get();
            //若為作廢的APPayment 則要多一張反轉傳票
            //20220418 by louis NMVoucherUtil.CreateAPVoucher()新增一個參數glStageCode紀錄傳票產生的時機
            if (apdoc.DocType == APDocType.VoidCheck)
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
                //20220414 by louis 新增判斷如果沒有對應的應付票據, 就不用更新傳票號碼回應付票據
                //20220418 by louis NMVoucherUtil.CreateAPVoucher()新增一個參數glStageCode紀錄傳票產生的時機
                if (payableCheck != null)
                {
                    PXUpdate<
                        Set<NMPayableCheck.paymentBatchNbr, Required<NMPayableCheck.paymentBatchNbr>>,
                        NMPayableCheck,
                        Where<NMPayableCheck.payableCheckID, Equal<Required<NMPayableCheck.payableCheckID>>>>
                        .Update(Base, NMVoucherUtil.CreateAPVoucher(NMStringList.NMAPVoucher.PAYMENT, payableCheck, GLStageCode.NMPConfirmP2), payableCheck.PayableCheckID);

                }

            }



            return adapter.Get();
        }
        #endregion

        #region Override Void
        public delegate IEnumerable VoidCheckDelegate(PXAdapter adapter);
        [PXOverride]
        public IEnumerable VoidCheck(PXAdapter adapter, VoidCheckDelegate baseMethod)
        {
            APPayment payment = Base.Document.Current;
            NMPayableCheck apCheck = GetPayableCheck(payment.RefNbr);
            //當NM應收票據為 已託收 or 已兌現 則不可作廢
            if (apCheck != null && (apCheck.Status == NMStringList.NMAPCheckStatus.UNCONFIRM ||
                apCheck.Status == NMStringList.NMAPCheckStatus.CONFIRM ||
                apCheck.Status == NMStringList.NMAPCheckStatus.CASH ||
                //apCheck.Status == NMStringList.NMAPCheckStatus.PRINT||
                apCheck.Status == NMStringList.NMAPCheckStatus.REPRESENT))
            {
                String msg = "請先處理應付票據!";
                throw new PXException(msg);
            }
            return baseMethod(adapter);
        }
        #endregion

        #endregion

        //2021-01-21 Add by Alton 防止別之程式呼叫原廠作廢時出錯
        protected virtual void APAdjust_AdjdCuryRate_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e, PXFieldVerifying InvokeBaseHandler)
        {
            if (e.NewValue != null && InvokeBaseHandler != null)
                InvokeBaseHandler(sender, e);
        }

        #region Method
        /// <summary>
        /// 應付票據過帳版本
        /// </summary>
        /// <param name="adapter"></param>
        public void OverrideAPRelease(PXAdapter adapter)
        {
            //Stage1. 原廠先過帳，且產生傳票
            using (PXTransactionScope ts = new PXTransactionScope())
            {
                List<APRegister> list = BaseRelease(adapter);
                foreach (APPayment apdoc in adapter.Get<APPayment>())
                {
                    if (apdoc == null) continue;
                    if (apdoc.DocType != APDocType.Prepayment && apdoc.DocType != APDocType.Check) continue;
                    if (apdoc.PaymentMethodID != "CHECK") continue;
                    //若為作廢的APPayment 則要多一張反轉傳票
                    //20220418 by louis NMVoucherUtil.CreateAPVoucher()新增一個參數glStageCode紀錄傳票產生的時機
                    if (apdoc.DocType == APDocType.VoidCheck)
                    {
                        NMPayableCheck payableCheck = GetPayableCheck(apdoc.RefNbr);
                        PXUpdate<
                            Set<NMPayableCheck.paymentReverseBatchNbr, Required<NMPayableCheck.paymentReverseBatchNbr>>,
                            NMPayableCheck,
                            Where<NMPayableCheck.payableCheckID, Equal<Required<NMPayableCheck.payableCheckID>>>>
                            .Update(Base, NMVoucherUtil.CreateAPVoucher(NMStringList.NMAPVoucher.PAYMENTREVERSE, payableCheck, GLStageCode.APPaymentVoid),
                             payableCheck.PayableCheckID);
                    }
                    else
                    {
                        NMPayableCheck payableCheck = GetPayableCheck(apdoc.RefNbr);
                        //20220418 by louis NMVoucherUtil.CreateAPVoucher()新增一個參數glStageCode紀錄傳票產生的時機
                        //20220414 by louis 新增判斷如果沒有對應的應付票據, 就不用更新傳票號碼回應付票據
                        if (payableCheck != null)
                        {
                            PXUpdate<
                            Set<NMPayableCheck.paymentBatchNbr, Required<NMPayableCheck.paymentBatchNbr>>,
                            NMPayableCheck,
                            Where<NMPayableCheck.payableCheckID, Equal<Required<NMPayableCheck.payableCheckID>>>>
                            .Update(Base, NMVoucherUtil.CreateAPVoucher(NMStringList.NMAPVoucher.PAYMENT, payableCheck, GLStageCode.NMPConfirmP2), payableCheck.PayableCheckID);

                        }

                    }

                }
                ts.Complete();
            }
        }
        /// <summary>
        /// 原廠APPaymentEntry Release
        /// </summary>
        /// <param name="adapter"></param>
        /// <returns></returns>
        public List<APRegister> BaseRelease(PXAdapter adapter)
        {
            PXCache cache = Base.Document.Cache;
            List<APRegister> list = new List<APRegister>();
            foreach (APPayment apdoc in adapter.Get<APPayment>())
            {
                if (apdoc.Status != APDocStatus.Balanced && apdoc.Status != APDocStatus.Printed && apdoc.Status != APDocStatus.Open)
                {
                    throw new PXException(Messages.Document_Status_Invalid);
                }
                if ((apdoc.DocType == APDocType.Check
                        || apdoc.DocType == APDocType.Prepayment && !Base.IsRequestPrepayment(apdoc)
                        || apdoc.DocType == APDocType.Refund
                        || apdoc.DocType == APDocType.VoidCheck)
                    && this.PaymentRefMustBeUnique && string.IsNullOrEmpty(apdoc.ExtRefNbr))
                {
                    cache.RaiseExceptionHandling<APPayment.extRefNbr>(apdoc, apdoc.ExtRefNbr,
                        new PXRowPersistingException(typeof(APPayment.extRefNbr).Name, null, ErrorMessages.FieldIsEmpty, typeof(APPayment.extRefNbr).Name));
                }
                cache.Update(apdoc);
                list.Add(apdoc);
            }

            Base.Save.Press();
            APDocumentRelease.ReleaseDoc(list, false);
            return list;
        }
        protected virtual bool PaymentRefMustBeUnique => PaymentRefAttribute.PaymentRefMustBeUnique(Base.paymenttype.Current);

        public void OverrideVoidCheck(PXAdapter adapter) { }
        public NMPayableCheck GetPayableCheck(string APRefNbr)
        {
            return PXSelect<NMPayableCheck,
                Where<NMPayableCheck.paymentDocNo, Equal<Required<NMPayableCheck.paymentDocNo>>>>
                .Select(Base, APRefNbr);
        }
        #endregion
    }
}