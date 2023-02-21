using PX.Data;
using PX.Objects.AP;

/**
 * ====2021-08-05:12183 ====Alton
 * 除了扣款借方調整(UsrIsDeductionDoc == true)是依據原單單號回傳
 * 其他皆依據ProjectID & PONbr處理
 * 
 * **/
namespace Kedge.Util
{
    public class APInvoiceValuationPhaseUtil
    {

        /// <summary>
        /// 取得期別
        /// </summary>
        /// <returns></returns>
        public static int? GetUsrValuationPhase(APInvoiceEntry entry)
        {
            APInvoice master = entry.Document.Current;
            if (master == null) return 0;
            APRegisterExt masterExt = PXCache<APRegister>.GetExtension<APRegisterExt>(master);
            //判斷是否為新增
            if (PXEntryStatus.Inserted == entry.Document.Cache.GetStatus(master))
            {
                //判斷是否為-扣款借方調整
                if (master.DocType == APDocType.DebitAdj && masterExt.UsrIsDeductionDoc == true)
                {
                    //扣款借方調整：回傳原計價單的期別
                    APRegister oriDeductionAP = GetAPRegister(entry, masterExt.UsrOriDeductionRefNbr, masterExt.UsrOriDeductionDocType);
                    APRegisterExt oriDeductionAPExt = PXCache<APRegister>.GetExtension<APRegisterExt>(oriDeductionAP);
                    return oriDeductionAPExt?.UsrValuationPhase ?? 1;
                }
                //2021-08-12 add預付款固定0期
                else if (master.DocType == APDocType.Prepayment)
                {
                    return 0;
                }
                else
                {
                    int? valuationPhase = 0;
                    //否則依據ProjectID & PONbr 來取得
                    //2021-08-12 add排除自己
                    APRegister apRegister = GetAPRegisterByProjectAndPO(new PXGraph(), master.ProjectID, masterExt.UsrPONbr, masterExt.UsrPOOrderType, master.RefNbr);
                    if (apRegister != null)
                    {
                        APRegisterExt apRegisterExt = PXCache<APRegister>.GetExtension<APRegisterExt>(apRegister);
                        valuationPhase = apRegisterExt.UsrValuationPhase ?? 0;
                    }
                    valuationPhase++;
                    return valuationPhase;
                }

            }
            //非新增則回傳自己
            return masterExt.UsrValuationPhase;
        }


        #region BQL
        /// <summary>
        /// 取得最後一筆票期 by ProjectID & PONbr
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="poNbr"></param>
        /// <returns></returns>
        private static APRegister GetAPRegisterByProjectAndPO(PXGraph graph, int? projectID, string poNbr, string poType, string refNbr)
        {
            return PXSelectJoin<APRegister,
                InnerJoin<APInvoice,
                    On<APInvoice.refNbr,Equal<APRegister.refNbr>,
                    And<APInvoice.docType,Equal<APRegister.docType>>>>,
                Where<APRegister.projectID, Equal<Required<APRegister.projectID>>,
                And<APRegisterExt.usrPONbr, Equal<Required<APRegisterExt.usrPONbr>>,
                And<APRegisterExt.usrPOOrderType, Equal<Required<APRegisterExt.usrPOOrderType>>,
                And<APRegister.refNbr, NotEqual<Required<APRegister.refNbr>>,
                And<IsNull<APRegisterExt.usrIsDeductionDoc,False>, NotEqual<True>
                >>>>>,
                OrderBy<Desc<APRegisterExt.usrValuationPhase>>
                >.Select(graph, projectID, poNbr, poType, refNbr);
        }

        private static APRegister GetAPRegister(PXGraph graph, string refNbr, string docType)
        {
            return PXSelect<APRegister,
                Where<APRegister.refNbr, Equal<Required<APRegister.refNbr>>,
                And<APRegister.docType, Equal<Required<APRegister.docType>>>>
                >.Select(graph, refNbr, docType);
        }
        #endregion
    }
}
