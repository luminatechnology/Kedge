using PX.Data;
using PX.Objects.AP;
using PX.Objects.CM;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PX.Objects.PO.GraphExtensions.APInvoiceEntryExt
{
    public class PrepaymentsExt : PXGraphExtension<Prepayments, APInvoiceEntry>
    {
        public delegate void AddPOOrderProcDelegate(POOrder order, bool createNew);
        [PXOverride]
        public virtual void AddPOOrderProc(POOrder order, bool createNew, AddPOOrderProcDelegate baseMethod)
        {
            baseMethod(order, createNew);
            if (createNew)
            {
                APInvoice prepayment = Base.Document.Current;
                prepayment.ProjectID = order.ProjectID;
            }

        }

        public delegate bool AddPOOrderLinesDelegate(IEnumerable<POLineRS> lines);
        /// <summary>
        /// 內容為原廠AddPOOrderLines，修正處額外註解
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="baseMethod"></param>
        /// <returns></returns>
        [PXOverride]
        public virtual bool AddPOOrderLines(IEnumerable<POLineRS> lines, AddPOOrderLinesDelegate baseMethod)
        {
            bool hasAdded = false;
            foreach (POLineRS line in lines.Where(l =>
                (l.CuryExtCost + l.CuryRetainageAmt > l.CuryReqPrepaidAmt)
                && (l.Billed == false || l.LineType == POLineType.Service && l.Closed == false)))
            {
                var tran = new APTran
                {
                    InventoryID = line.InventoryID,
                    ProjectID = line.ProjectID,
                    TaskID = line.TaskID,
                    CostCodeID = line.CostCodeID,
                    TaxID = line.TaxID,
                    TaxCategoryID = line.TaxCategoryID,
                    TranDesc = line.TranDesc,
                    UOM = line.UOM,
                    CuryUnitCost = line.CuryUnitCost,
                    DiscPct = line.DiscPct,
                    ManualPrice = true,
                    ManualDisc = true,
                    FreezeManualDisc = true,
                    DiscountID = line.DiscountID,
                    DiscountSequenceID = line.DiscountSequenceID,
                    RetainagePct = line.RetainagePct,
                    POOrderType = line.OrderType,
                    PONbr = line.OrderNbr,
                    POLineNbr = line.LineNbr,
                };
                #region add by Alton 20201223
                //先觸發Event避免某段Event造成Qty被清空
                tran = Base.Transactions.Insert(tran);
                #endregion
                decimal? billedAndPrepaidQty = line.ReqPrepaidQty + line.OrderBilledQty;
                tran.Qty = (line.OrderQty <= billedAndPrepaidQty) ? line.OrderQty : line.OrderQty - billedAndPrepaidQty;

                decimal? billedAndPrepaidAmt = line.CuryReqPrepaidAmt + line.CuryOrderBilledAmt;
                if (billedAndPrepaidAmt == 0m)
                {
                    tran.CuryLineAmt = line.CuryLineAmt;
                    tran.CuryDiscAmt = line.CuryDiscAmt;
                }
                else if (line.CuryExtCost + line.CuryRetainageAmt <= billedAndPrepaidAmt)
                {
                    tran.CuryLineAmt = 0m;
                    tran.CuryDiscAmt = 0m;
                    tran.CuryRetainageAmt = 0m;
                    tran.CuryTranAmt = 0m;
                }
                else
                {
                    decimal? prepaymentRatio = (line.CuryExtCost + line.CuryRetainageAmt - billedAndPrepaidAmt) / (line.CuryExtCost + line.CuryRetainageAmt);
                    tran.CuryLineAmt = PXCurrencyAttribute.Round(Base.Transactions.Cache, tran, (prepaymentRatio * line.CuryLineAmt) ?? 0m, CMPrecision.TRANCURY);
                    tran.CuryDiscAmt = PXCurrencyAttribute.Round(Base.Transactions.Cache, tran, (prepaymentRatio * line.CuryDiscAmt) ?? 0m, CMPrecision.TRANCURY);
                }

                #region edit by Alton 20201223
                //原廠Insert改為Update
                //Base.Transactions.Insert(tran);
                tran = Base.Transactions.Update(tran);
                #endregion
                hasAdded = true;
            }
            Base.AutoRecalculateDiscounts();
            return hasAdded;
        }

    }
}
