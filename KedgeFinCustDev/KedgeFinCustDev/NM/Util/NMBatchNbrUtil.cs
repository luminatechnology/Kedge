using Kedge.DAC;
using NM.DAC;
using PX.Data;
using System;

namespace NM.Util
{
    /// <summary>
    /// NM - 批次處理號碼
    /// </summary>
    public class NMBatchNbrUtil
    {
        private static object lockObj1 = new object();
        private static object lockObj2 = new object();

        /// <summary>
        /// 取得批次號碼，且更新KGBillPayment.usrNMBatchNbr
        /// </summary>
        /// <param name="businessDate"></param>
        /// <param name="billPaymentID"></param>
        /// <returns></returns>
        public static void UpdateBillPaymentBatchNbr(PXGraph g, String batchNbr, int? billPaymentID)
        {
            lock (lockObj1)
            {
                //更新KGBillPayment
                PXUpdate<
                        Set<KGBillPaymentExt.usrNMBatchNbr, Required<KGBillPaymentExt.usrNMBatchNbr>>,
                        KGBillPayment,
                        Where<KGBillPayment.billPaymentID, Equal<Required<KGBillPayment.billPaymentID>>>>
                        .Update(g, batchNbr, billPaymentID);
            }
        }

        /// <summary>
        /// 取得批次號碼
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        public static string GetNextBatchNbr(PXGraph g)
        {
            lock (lockObj2)
            {
                NMPreference nMPreference = GetPreference(g);
                DateTime? date = g.Accessinfo.BusinessDate;
                string sysDate = date?.ToString("yyyyMMdd");
                string batchDate = nMPreference.NMBatchDate?.ToString("yyyyMMdd");
                //當前BatchSeq
                int? seq = nMPreference.NMBatchSeq ?? 0;
                //當系統日不等於資料日，則刷新BatchSeq & Date
                if (sysDate != batchDate)
                {
                    batchDate = sysDate;
                    seq = 0;
                    PXUpdate<
                        Set<NMPreference.nmBatchDate, Required<NMPreference.nmBatchDate>,
                        Set<NMPreference.nmBatchSeq, Required<NMPreference.nmBatchSeq>>>
                        , NMPreference>.Update(g, date, seq);
                }
                //BatchSeq+1，且更新NMPreference.NMBatchSeq
                seq++;
                PXUpdate<
                        Set<NMPreference.nmBatchSeq, Required<NMPreference.nmBatchSeq>>
                        , NMPreference>.Update(g, seq);
                //BatchNbr = Date + "-" + Seq
                String batchNbr = String.Format("{0}_{1}", batchDate, seq);
                return batchNbr;
            }
        }

        private static NMPreference GetPreference(PXGraph g)
        {
            return PXSelect<NMPreference>.Select(g);
        }
    }
}
