using Kedge.DAC;
using PX.Data;
using PX.Objects.AP;
using System;

namespace KG.Util
{
    /// <summary>
    /// KG - 傳票號碼(格式:日期+流水號)
    /// </summary>
    public class KGBatchNbrUtil
    {
        private static object lockObj1 = new object();
        //private static object lockObj2 = new object();

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

        public static string GetLastBatchNbr(PXGraph g,DateTime? voucherdate)
        {
            string batchNbr = "";

            APRegister lastNbr = PXSelectGroupBy<APRegister, Where2<Where<APRegisterExt.usrIsDeductionDoc, Equal<False>,
                                                                          Or<APRegisterExt.usrIsDeductionDoc,IsNull>>,
                                                                    And<APRegisterFinExt.usrAccConfirmNbr, Like<Required<APRegisterFinExt.usrAccConfirmNbr>>>>,
                                                             Aggregate<Max<APRegisterFinExt.usrAccConfirmNbr>>>
                                                             .Select(g,voucherdate?.ToString("yyyyMMdd")+"%");

            if (lastNbr != null)
                //batchNbr = null;
            //else
            {
                batchNbr = PXCache<APRegister>.GetExtension<APRegisterFinExt>(lastNbr).UsrAccConfirmNbr;
            }

            return batchNbr;
        }
        //private static KGSetUp GetSetup(PXGraph g)
        //{
        //    return PXSelect<KGSetUp>.Select(g);
        //}
    }
}
