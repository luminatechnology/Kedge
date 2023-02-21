using Kedge.DAC;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.PO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kedge
{
    /**
     * ====估驗明細(未稅)====
     * 估驗金額：PoValuationAmt
     * 前期累計：PoCumulativeAmt
     * 總累計金額：PoTotalAmt
     * 比例(%)：PoValuationPercent
     * 
     * ====預付(未稅)====
     * 預付款：PrepaymentAmt
     * 預付款前期累計：PrepaymentCumAmt
     * 預付款總累計：PrepaymentTotalAmt
     * 扣回金額：PrepaymentDuctAmt
     * 扣回金額前期累計：PrepaymentDuctCumAmt
     * 扣回金額總累計：PrepaymentDuctTotalAmt
     * 
     * ====加款(未稅)====
     * 加款金額：AdditionAmt
     * 前期累計：AdditionCumAmt
     * 總累計金額：AdditionTotalAmt
     * 
     * ====應付金額====
     * 應付金額：OriTotalAmt
     * 稅額：TaxAmt
     * 發票金額：GvInvWithTaxAmt
     * 
     * ====代扣====
     * 代扣稅：WhtAmt
     * 退休金：LbpAmt
     * 二代健保：Gnhi2Amt
     * 勞保費：LbiAmt
     * 健保費：HliAmt
     * 
     * ====扣款====
     * 扣款金額：DeductionWithTaxAmt
     * 前期累計：DeductionWithTaxCumAmt
     * 總累計金額：DeductionWithTaxTotalAmt
     * 
     * ===票貼=== 2021/06/18 Add by Althea
     * 票貼 : CheckDiscountAmt
     * 
     * ====保留款====
     * 保留款：RetentionWithTaxAmt
     * 保留款前期累計：RetentionWithTaxCumAmt
     * 保留款總累計金額：RetentionWithTaxTotalAmt
     * 保留款退回：RetentionReturnWithTaxAmt
     * 保留款退回前期累計：RetentionReturnWithTaxCumAmt
     * 保留款退回總累計金額：RetentionReturnWithTaxTotalAmt
     * 
     * ===實付金額===
     * 小計：TotalAmt
     * 
     * ====2021-07-02 :12120 ====Alton
     * 1.KGBillSummary.PrepaymentDuctAmt 請用APAdjust.CuryAdjdAmt * 稅額然後四捨五入到整數
     * 2.KGBillSummary.PrepaymentDuctTaxAmt請用APAdjust.CuryAdjdAmt - KGBillSummary.PrepaymentDuctAmt
     * 3.PrepaymentDuctWithTaxAmt請用APAdjust.CuryAdjdAmt
     * 4.畫面請在計價匯總的KGBillSummary.OriTotalAmt下面新增顯示KGBillSummary.PrepaymentDuctTaxAmt
     * 5.KGBillSummary.GvInvWithTaxAmt的計算請用 OriTotalAmt- PrepaymentDuctTaxAmt + TaxAmt
     * 
     * ====2021-07-06:12120====Alton
     * 1.KGBillSummary.TotalAmt 需要減掉 KGBillSummary.PrepaymentDuctTaxAmt(參考KedgeTest AP0008575)
     * 
     * ====2021-07-13:12143====Alton
     * KGBillSummary.oriPOTotalAmt = POOrder.CuryLineTotal + POOrder.CuryRetainageTotal
     * 
     * ====2021-07-21:12169====Alton
     * RetentionReturnTaxAmt改抓稅額明細中的APTaxTran.CuryTaxAmt
     * 
     * ===2021/08/17 :0012202 === Althea
     * 票貼金額原本抓curyLineAmt, 改抓 KGValuation.TotalAmt
     * 
     * ====2021-09-17:口頭====Alton
     * AdjustChange的觸發額外添加條件，當Sum(CuryAdjdAmt) != PrepaymentDuctWithTaxAmt才觸發異動
     * 避免原廠每次存檔都會重新insert Adjust 的問題
     * 
     * ====2021-04-27 ====Alton
     * 將原本APInvoiceEntry_Extension的KGBillSummary 相關計算改來這邊
     * **/
    public class APInvoiceEntryBillSummaryExt : PXGraphExtension<APInvoiceEntry_Extension, APInvoiceEntryRetainage, APInvoiceEntry>
    {
        #region Override
        public delegate void PersistDelegate();
        [PXOverride]
        public void Persist(PersistDelegate baseMethod)
        {
            APInvoice master = Base.Document.Current;
            //APRegisterExt masterExt = Base.Document.Cache.GetExtension<APRegisterExt>(master);
            
            //存檔的時候才重新計算前期, 只有"計價"才做前期累計
            if (!isDelete && master.DocType == APDocType.Invoice)
            {
                SetCumAndTotalSummary(master);
            }

            baseMethod();
        }
        #endregion

        #region Event Handlers

        #region APInvoice
        protected void _(Events.FieldUpdated<APInvoice, APInvoice.curyRetainageTotal> e)
        {
            KGBillSummary sum = Base2.getKGBillSummary();
            Base2.SummaryAmtFilters.Cache.SetDefaultExt<KGBillSummary.retentionAmt>(sum);
            Base2.SummaryAmtFilters.Cache.SetDefaultExt<KGBillSummary.retentionWithTaxAmt>(sum);
        }

        protected void _(Events.FieldUpdated<APRegisterExt.usrRetainageAmt> e)
        {
            KGBillSummary sum = Base2.getKGBillSummary();

            // Mantis [0012321] #4
            if (sum != null)
            {
                Base2.SummaryAmtFilters.Cache.SetDefaultExt<KGBillSummary.retentionAmt>(sum);
                Base2.SummaryAmtFilters.Cache.SetDefaultExt<KGBillSummary.retentionWithTaxAmt>(sum);
            }
        }

        bool isDelete = false;
        protected void _(Events.RowDeleted<APInvoice> e)
        {
            isDelete = true;
        }
        #endregion

        #region APTran
        protected void _(Events.RowUpdated<APTran> e)
        {
            APTran row = e.Row;
            APTran old = e.OldRow;
            if (row == null) return;
            APTranChange();
        }

        protected void _(Events.RowInserted<APTran> e)
        {
            APTran row = e.Row;
            APTranChange();
        }

        protected void _(Events.RowDeleted<APTran> e)
        {
            APTran row = e.Row;
            APTranChange();
        }
        #endregion

        #region APAdjust
        protected void _(Events.RowUpdated<APAdjust> e)
        {
            APAdjust row = e.Row;
            APAdjust old = e.OldRow;
            if (row == null) return;
            APAdjustChange();
        }

        protected void _(Events.RowInserted<APAdjust> e)
        {
            APAdjust row = e.Row;
            APAdjustChange();
        }

        protected void _(Events.RowDeleted<APAdjust> e)
        {
            if (isDelete) return;
            APAdjust row = e.Row;
            APAdjustChange();
        }
        #endregion

        #region APTaxTran
        protected void _(Events.RowUpdated<APTaxTran> e)
        {
            APTaxTran row = e.Row;
            APTaxTran old = e.OldRow;
            if (row == null) return;
            SetAllAmt();
            APTaxTranChange();
            Base2.SummaryAmtFilters.Cache.SetDefaultExt<KGBillSummary.retentionAmt>(Base2.getKGBillSummary());
        }

        protected void _(Events.RowInserted<APTaxTran> e)
        {
            APTaxTran row = e.Row;
            SetAllAmt();
            APTaxTranChange();
            Base2.SummaryAmtFilters.Cache.SetDefaultExt<KGBillSummary.retentionAmt>(Base2.getKGBillSummary());
        }

        protected void _(Events.RowDeleted<APTaxTran> e)
        {
            if (isDelete) return;
            APTaxTran row = e.Row;
            SetAllAmt();
            APTaxTranChange();
            Base2.SummaryAmtFilters.Cache.SetDefaultExt<KGBillSummary.retentionAmt>(Base2.getKGBillSummary());
        }
        #endregion

        #region KGDeductionAPTran
        protected void _(Events.RowUpdated<KGDeductionAPTran> e)
        {
            KGDeductionAPTran row = e.Row;
            KGDeductionAPTran old = e.OldRow;
            if (row == null) return;
            KGDeductionAPTranChange();
        }

        protected void _(Events.RowInserted<KGDeductionAPTran> e)
        {
            KGDeductionAPTran row = e.Row;
            KGDeductionAPTranChange();
        }

        protected void _(Events.RowDeleted<KGDeductionAPTran> e)
        {
            if (isDelete) return;
            KGDeductionAPTran row = e.Row;
            KGDeductionAPTranChange();
        }
        #endregion

        #region KGBillSummary

        #region 估驗金額
        protected void _(Events.FieldUpdated<KGBillSummary, KGBillSummary.poValuationAmt> e)
        {
            KGBillSummary row = e.Row;
            if (row == null) return;
            e.Cache.SetDefaultExt<KGBillSummary.poTotalAmt>(row);
            e.Cache.SetDefaultExt<KGBillSummary.oriTotalAmt>(row);
            e.Cache.SetDefaultExt<KGBillSummary.totalAmt>(row);
            // Add this calculation trigger to solve contract evaluation validation check timing. 2021/01/10
            e.Cache.SetDefaultExt<KGBillSummary.poValuationPercent>(row);
        }
        #endregion

        #region 預付款
        //預付款
        protected void _(Events.FieldUpdated<KGBillSummary, KGBillSummary.prepaymentAmt> e)
        {
            KGBillSummary row = e.Row;
            if (row == null) return;
            e.Cache.SetValueExt<KGBillSummary.prepaymentWithTaxAmt>(row, Base2.round(row.PrepaymentAmt * (1 + Base2.getTaxRate())));
            e.Cache.SetDefaultExt<KGBillSummary.prepaymentTotalAmt>(row);
            e.Cache.SetDefaultExt<KGBillSummary.oriTotalAmt>(row);
            e.Cache.SetDefaultExt<KGBillSummary.totalAmt>(row);
        }
        //預付款扣回
        protected void _(Events.FieldUpdated<KGBillSummary, KGBillSummary.prepaymentDuctWithTaxAmt> e)
        {
            KGBillSummary row = e.Row;
            if (row == null) return;
            decimal? taxRate = Base2.getTaxRate() ?? 0;
            decimal? prepaymentDuctAmt = Base2.round((row.PrepaymentDuctWithTaxAmt ?? 0) / (1 + taxRate));
            decimal? prepaymentDuctTaxAmt = row.PrepaymentDuctWithTaxAmt - prepaymentDuctAmt;

            e.Cache.SetValueExt<KGBillSummary.prepaymentDuctAmt>(row, prepaymentDuctAmt);
            e.Cache.SetValueExt<KGBillSummary.prepaymentDuctTaxAmt>(row, prepaymentDuctTaxAmt);
            e.Cache.SetDefaultExt<KGBillSummary.prepaymentDuctTotalAmt>(row);
            e.Cache.SetDefaultExt<KGBillSummary.oriTotalAmt>(row);
            e.Cache.SetDefaultExt<KGBillSummary.totalAmt>(row);
        }

        #endregion

        #region 加款
        protected void _(Events.FieldUpdated<KGBillSummary, KGBillSummary.additionAmt> e)
        {
            KGBillSummary row = e.Row;
            if (row == null) return;
            decimal? addTaxAmt = Base2.round(row.AdditionAmt * Base2.getTaxRate());
            e.Cache.SetValueExt<KGBillSummary.additionTaxAmt>(row, addTaxAmt);
            e.Cache.SetValueExt<KGBillSummary.additionWithTaxAmt>(row, row.AdditionAmt + addTaxAmt);
            e.Cache.SetDefaultExt<KGBillSummary.additionTotalAmt>(row);
            e.Cache.SetDefaultExt<KGBillSummary.oriTotalAmt>(row);
            e.Cache.SetDefaultExt<KGBillSummary.totalAmt>(row);
        }
        #endregion

        #region 代扣
        protected void _(Events.FieldUpdated<KGBillSummary, KGBillSummary.whtAmt> e)
        {
            KGBillSummary row = e.Row; if (row == null) return;
            e.Cache.SetDefaultExt<KGBillSummary.totalAmt>(row);
        }

        protected void _(Events.FieldUpdated<KGBillSummary, KGBillSummary.gnhi2Amt> e)
        {
            KGBillSummary row = e.Row; if (row == null) return;
            e.Cache.SetDefaultExt<KGBillSummary.totalAmt>(row);
        }

        protected void _(Events.FieldUpdated<KGBillSummary, KGBillSummary.hliAmt> e)
        {
            KGBillSummary row = e.Row; if (row == null) return;
            e.Cache.SetDefaultExt<KGBillSummary.totalAmt>(row);
        }

        protected void _(Events.FieldUpdated<KGBillSummary, KGBillSummary.lbiAmt> e)
        {
            KGBillSummary row = e.Row; if (row == null) return;
            e.Cache.SetDefaultExt<KGBillSummary.totalAmt>(row);
        }

        protected void _(Events.FieldUpdated<KGBillSummary, KGBillSummary.lbpAmt> e)
        {
            KGBillSummary row = e.Row; if (row == null) return;
            e.Cache.SetDefaultExt<KGBillSummary.totalAmt>(row);
        }
        #endregion

        #region 扣款
        protected void _(Events.FieldUpdated<KGBillSummary, KGBillSummary.deductionWithTaxAmt> e)
        {
            KGBillSummary row = e.Row;
            if (row == null) return;
            e.Cache.SetDefaultExt<KGBillSummary.deductionWithTaxTotalAmt>(row);
            e.Cache.SetDefaultExt<KGBillSummary.totalAmt>(row);
        }
        #endregion

        #region 票貼 2021/07/06 Add by Althea
        protected void _(Events.FieldUpdated<KGBillSummary, KGBillSummary.checkDiscountAmt> e)
        {
            KGBillSummary row = e.Row;
            if (row == null) return;

            e.Cache.SetDefaultExt<KGBillSummary.totalAmt>(row);
        }
        #endregion

        #region 保留款
        protected void _(Events.FieldDefaulting<KGBillSummary, KGBillSummary.retentionAmt> e)
        {
            KGBillSummary row = e.Row;
            APInvoice master = Base.Document.Current;

            if (row == null || master == null || !IsInvOrDebitAdj(master)) return;

            // Mantis [0012321] #4
            e.NewValue = (master.CuryRetainageTotal ?? 0) - (row?.RetentionTaxAmt ?? 0) + master.GetExtension<APRegisterExt>().UsrRetainageAmt; ;
        }

        protected void _(Events.FieldUpdated<KGBillSummary, KGBillSummary.retentionAmt> e)
        {
            KGBillSummary row = e.Row;
            APInvoice master = Base.Document.Current;
            if (row == null || master == null) return;
            if (!IsInvOrDebitAdj(master)) return;
            e.Cache.SetDefaultExt<KGBillSummary.retentionWithTaxAmt>(row);
        }

        protected void _(Events.FieldDefaulting<KGBillSummary, KGBillSummary.retentionWithTaxAmt> e)
        {
            KGBillSummary row = e.Row;
            APInvoice master = Base.Document.Current;

            if (row == null || master == null || !IsInvOrDebitAdj(master)) return;

            e.NewValue = (row.RetentionAmt ?? 0m) + (row.RetentionTaxAmt ?? 0m);
        }

        protected void _(Events.FieldUpdated<KGBillSummary, KGBillSummary.retentionWithTaxAmt> e)
        {
            KGBillSummary row = e.Row;
            APInvoice master = Base.Document.Current;
            if (row == null || master == null) return;
            if (!IsInvOrDebitAdj(master)) return;
            e.Cache.SetDefaultExt<KGBillSummary.retentionWithTaxTotalAmt>(row);
            e.Cache.SetDefaultExt<KGBillSummary.totalAmt>(row); ;
        }

        //保留款含稅(APTaxTranChange)
        protected void _(Events.FieldUpdated<KGBillSummary, KGBillSummary.retentionTaxAmt> e)
        {
            KGBillSummary row = e.Row;
            APInvoice master = Base.Document.Current;
            if (row == null || master == null) return;
            if (!IsInvOrDebitAdj(master)) return;
            e.Cache.SetDefaultExt<KGBillSummary.retentionAmt>(row);
            e.Cache.SetDefaultExt<KGBillSummary.retentionWithTaxAmt>(row);
        }

        protected void _(Events.FieldUpdated<KGBillSummary, KGBillSummary.retentionReturnAmt> e)
        {
            KGBillSummary row = e.Row;
            APInvoice master = Base.Document.Current;
            if (row == null || master == null) return;
            if (!IsInvOrDebitAdj(master)) return;
            //2021-07-21 taxAmt改抓APTaxTran.CuryTaxAmt ( 改到APTaxTran異動時處理)
            //decimal? taxAmt = Base2.round(row.RetentionReturnAmt * Base2.getTaxRate()) ?? 0;
            //e.Cache.SetValueExt<KGBillSummary.retentionReturnTaxAmt>(row, taxAmt);
            e.Cache.SetDefaultExt<KGBillSummary.retentionReturnWithTaxAmt>(row);
            e.Cache.SetDefaultExt<KGBillSummary.retentionReturnWithTaxTotalAmt>(row);
            e.Cache.SetDefaultExt<KGBillSummary.totalAmt>(row);
        }

        protected void _(Events.FieldUpdated<KGBillSummary, KGBillSummary.retentionReturnTaxAmt> e)
        {
            KGBillSummary row = e.Row;
            APInvoice master = Base.Document.Current;
            if (row == null || master == null) return;
            if (!IsInvOrDebitAdj(master)) return;
            e.Cache.SetDefaultExt<KGBillSummary.retentionReturnWithTaxAmt>(row);
        }

        protected void _(Events.FieldDefaulting<KGBillSummary, KGBillSummary.retentionReturnWithTaxAmt> e)
        {
            KGBillSummary row = e.Row;
            APInvoice master = Base.Document.Current;
            if (row == null || master == null) return;
            if (!IsInvOrDebitAdj(master)) return;
            e.NewValue = (row?.RetentionReturnAmt ?? 0m) + (row?.RetentionReturnTaxAmt ?? 0m);
        }

        protected void _(Events.FieldUpdated<KGBillSummary, KGBillSummary.retentionReturnWithTaxAmt> e)
        {
            KGBillSummary row = e.Row;
            APInvoice master = Base.Document.Current;
            if (row == null || master == null) return;
            if (!IsInvOrDebitAdj(master)) return;
            e.Cache.SetDefaultExt<KGBillSummary.retentionReturnWithTaxTotalAmt>(row);
            e.Cache.SetDefaultExt<KGBillSummary.totalAmt>(row);
        }
        #endregion

        #region 應付金額
        //應付金額
        protected void _(Events.FieldDefaulting<KGBillSummary, KGBillSummary.oriTotalAmt> e)
        {
            KGBillSummary row = e.Row;
            if (row == null) return;
            e.NewValue =
                  (row.PoValuationAmt ?? 0m)
                + (row.PrepaymentAmt ?? 0m)
                + (row.AdditionAmt ?? 0m)
                - (row.PrepaymentDuctAmt ?? 0m);
        }
        protected void _(Events.FieldUpdated<KGBillSummary, KGBillSummary.oriTotalAmt> e)
        {
            KGBillSummary row = e.Row;
            if (row == null) return;
            e.Cache.SetDefaultExt<KGBillSummary.gvInvWithTaxAmt>(row);
        }
        //稅額
        protected void _(Events.FieldUpdated<KGBillSummary, KGBillSummary.taxAmt> e)
        {
            KGBillSummary row = e.Row;
            if (row == null) return;
            e.Cache.SetDefaultExt<KGBillSummary.gvInvWithTaxAmt>(row);
            e.Cache.SetDefaultExt<KGBillSummary.totalAmt>(row);
        }
        //發票金額
        protected void _(Events.FieldDefaulting<KGBillSummary, KGBillSummary.gvInvWithTaxAmt> e)
        {
            KGBillSummary row = e.Row;
            if (row == null) return;
            //2021-07-02:12120 發票金額(GvInvWithTaxAmt) = 應付金額(OriTotalAmt) - 付款扣回含稅金額(PrepaymentDuctTaxAmt) + 稅額(TaxAmt)
            e.NewValue = (row.OriTotalAmt ?? 0) - (row.PrepaymentDuctTaxAmt ?? 0) + (row.TaxAmt ?? 0);
        }
        #endregion

        #region 總累計
        //估驗比例
        protected void _(Events.FieldDefaulting<KGBillSummary, KGBillSummary.poValuationPercent> e)
        {
            KGBillSummary row = e.Row;
            if (row != null && (row.OriPOTotalAmt ?? 0) > 0)
                e.NewValue = (row.PoTotalAmt ?? 0m) * 100 / (row.OriPOTotalAmt ?? 0m);
            else
                e.NewValue = 0m;
        }
        //估驗
        protected void _(Events.FieldDefaulting<KGBillSummary, KGBillSummary.poTotalAmt> e)
        {
            KGBillSummary row = e.Row;
            if (row != null)
                e.NewValue = (row.PoCumulativeAmt ?? 0m) + (row.PoValuationAmt ?? 0m);
        }
        //預付款
        protected void _(Events.FieldDefaulting<KGBillSummary, KGBillSummary.prepaymentTotalAmt> e)
        {
            KGBillSummary row = e.Row;
            if (row != null)
                e.NewValue = (row.PrepaymentCumAmt ?? 0m) + (row.PrepaymentAmt ?? 0m);
        }
        //預付款扣回
        protected void _(Events.FieldDefaulting<KGBillSummary, KGBillSummary.prepaymentDuctTotalAmt> e)
        {
            KGBillSummary row = e.Row;
            if (row != null)
                e.NewValue = (row.PrepaymentDuctCumAmt ?? 0m) + (row.PrepaymentDuctAmt ?? 0m);
        }
        //加款
        protected void _(Events.FieldDefaulting<KGBillSummary, KGBillSummary.additionTotalAmt> e)
        {
            KGBillSummary row = e.Row;
            if (row != null)
                e.NewValue = (row.AdditionCumAmt ?? 0m) + (row.AdditionAmt ?? 0m);
        }
        //扣款
        protected void _(Events.FieldDefaulting<KGBillSummary, KGBillSummary.deductionWithTaxTotalAmt> e)
        {
            KGBillSummary row = e.Row;
            if (row != null)
                e.NewValue = (row.DeductionWithTaxCumAmt ?? 0) + (row.DeductionWithTaxAmt ?? 0);
        }
        //保留款
        protected void _(Events.FieldDefaulting<KGBillSummary, KGBillSummary.retentionWithTaxTotalAmt> e)
        {
            if (e.Row != null)
            {
                e.NewValue = (e.Row.RetentionWithTaxCumAmt ?? 0) + (Base.Document.Current?.DocType == APDocType.DebitAdj ? -1 * (e.Row.RetentionWithTaxAmt ?? 0) : (e.Row.RetentionWithTaxAmt ?? 0));
            }
        }
        //保留款退回
        protected void _(Events.FieldDefaulting<KGBillSummary, KGBillSummary.retentionReturnWithTaxTotalAmt> e)
        {
            KGBillSummary row = e.Row;
            if (row != null)
                e.NewValue = (row.RetentionReturnWithTaxCumAmt ?? 0) + (row.RetentionReturnWithTaxAmt ?? 0);
        }
        //實付金額(小計)
        protected void _(Events.FieldDefaulting<KGBillSummary, KGBillSummary.totalAmt> e)
        {
            KGBillSummary row = e.Row;
            if (row != null)
                e.NewValue =
                       (row.PoValuationAmt ?? 0)//估驗金額(+)
                     + (row.PrepaymentAmt ?? 0)//預付款(+)
                     + (row.AdditionAmt ?? 0)//加款(+)
                     + (row.RetentionReturnWithTaxAmt ?? 0)//保留款退回(+)
                     + (row.TaxAmt ?? 0)//稅額(+)
                     - (row.PrepaymentDuctAmt ?? 0)//預付款扣回(-)
                     - (row.PrepaymentDuctTaxAmt ?? 0)//預付款扣回稅額(-)
                     - (row.DeductionWithTaxAmt ?? 0)//扣款(-)
                     - (row.RetentionWithTaxAmt ?? 0)//保留款(-)
                     - (row.WhtAmt ?? 0)//代扣稅(-)
                     - (row.Gnhi2Amt ?? 0)//二代健保(-)
                     - (row.HliAmt ?? 0)//健保費(-)
                     - (row.LbiAmt ?? 0)//勞保(-)
                     - (row.LbpAmt ?? 0)//退休金(-)
                     - (row.CheckDiscountAmt ?? 0);//票貼(-) 2021/06/18 add by althea
        }
        #endregion

        #endregion

        #endregion

        #region Methods
        public void SetAllAmt()
        {
            APTranChange();
            APAdjustChange();
            KGDeductionAPTranChange();
        }

        /// <summary>
        /// 計算前期累計 & 總累計
        /// </summary>
        public void SetCumAndTotalSummary(APInvoice master)
        {
            APRegisterExt masterExt = master.GetExtension<APRegisterExt>();
            //貸方調整不做計算
            //沒有PO 或 NonProject 不做累計計算
            //if (IsCreditAdj(master)) return;
            if (IsCreditAdj(master) || (masterExt.UsrPONbr == null || master.ProjectID == 0 || master.ProjectID == null) ) return;
            
            KGBillSummary sum = Base2.getKGBillSummary();

            decimal? sumPoCumulativeAmt = 0, sumPrepaymentCumAmt = 0, sumPrepaymentDuctCumAmt = 0, sumAdditionCumAmt = 0, sumDeductionWithTaxCumAmt = 0, sumRetentionWithTaxCumAmt = 0, sumRetentionReturnWithTaxCumAmt = 0;

            #region 計算前期累計
            foreach (PXResult<KGBillSummary, APInvoice, APRegister> result in GetCumKGBillSummary(sum, master, masterExt))
            {
                KGBillSummary cumSum = (KGBillSummary)result;
                APRegister apRegister = (APRegister)result;
                APRegisterExt apRegisterExt = PXCache<APRegister>.GetExtension<APRegisterExt>(apRegister);

                if (apRegisterExt.UsrRetainageHistType == RetHistType.Original) { continue; }

                //借方調整(反轉) 負 ，其他為 正
                if (apRegister.DocType == APDocType.DebitAdj && apRegisterExt.UsrIsDeductionDoc != true)
                {
                    //累積估驗金額
                    sumPoCumulativeAmt -= cumSum.PoValuationAmt ?? 0;
                    //累積預付款
                    sumPrepaymentCumAmt -= cumSum.PrepaymentAmt ?? 0;
                    //累計預付款扣回
                    sumPrepaymentDuctCumAmt -= cumSum.PrepaymentDuctAmt ?? 0;
                    //累積加款
                    sumAdditionCumAmt -= cumSum.AdditionAmt;
                    //累積扣款
                    sumDeductionWithTaxCumAmt -= cumSum.DeductionWithTaxAmt ?? 0;
                    //累積保留款
                    sumRetentionWithTaxCumAmt -= (cumSum.RetentionWithTaxAmt ?? 0);
                    //累積保留款退回
                    sumRetentionReturnWithTaxCumAmt -= (cumSum.RetentionReturnWithTaxAmt ?? 0);
                }
                else
                {
                    //累積估驗金額
                    sumPoCumulativeAmt += cumSum.PoValuationAmt ?? 0;
                    //累積預付款
                    sumPrepaymentCumAmt += cumSum.PrepaymentAmt ?? 0;
                    //累計預付款扣回
                    sumPrepaymentDuctCumAmt += cumSum.PrepaymentDuctAmt ?? 0;
                    //累積加款
                    sumAdditionCumAmt += cumSum.AdditionAmt;
                    //累積扣款
                    sumDeductionWithTaxCumAmt += cumSum.DeductionWithTaxAmt ?? 0;
                    //累積保留款
                    sumRetentionWithTaxCumAmt += (cumSum.RetentionWithTaxAmt ?? 0);
                    //累積保留款退回
                    sumRetentionReturnWithTaxCumAmt += (cumSum.RetentionReturnWithTaxAmt ?? 0);
                }
            }
            #endregion

            PXCache cache = Base2.SummaryAmtFilters.Cache;
            sum.PoCumulativeAmt = sumPoCumulativeAmt;
            sum.PrepaymentCumAmt = sumPrepaymentCumAmt;
            sum.PrepaymentDuctCumAmt = sumPrepaymentDuctCumAmt;
            sum.AdditionCumAmt = sumAdditionCumAmt;
            sum.DeductionWithTaxCumAmt = sumDeductionWithTaxCumAmt;
            sum.RetentionWithTaxCumAmt = sumRetentionWithTaxCumAmt;
            sum.RetentionReturnWithTaxCumAmt = sumRetentionReturnWithTaxCumAmt;
            sum = Base2.SummaryAmtFilters.Update(sum);
            cache.SetDefaultExt<KGBillSummary.poTotalAmt>(sum);
            cache.SetDefaultExt<KGBillSummary.prepaymentTotalAmt>(sum);
            cache.SetDefaultExt<KGBillSummary.prepaymentDuctTotalAmt>(sum);
            cache.SetDefaultExt<KGBillSummary.additionTotalAmt>(sum);
            cache.SetDefaultExt<KGBillSummary.deductionWithTaxTotalAmt>(sum);
            cache.SetDefaultExt<KGBillSummary.retentionWithTaxTotalAmt>(sum);
            cache.SetDefaultExt<KGBillSummary.retentionReturnWithTaxTotalAmt>(sum);

            POOrder order = GetPOOrder(masterExt.UsrPONbr, masterExt.UsrPOOrderType);
            cache.SetValueExt<KGBillSummary.oriPOTotalAmt>(sum, (order?.CuryLineTotal ?? 0) + (order?.CuryRetainageTotal ?? 0));
            cache.SetDefaultExt<KGBillSummary.poValuationPercent>(sum);
        }

        /// <summary>
        /// 計算估驗金額 或 預付款金額
        /// </summary>
        public void APTranChange()
        {
            APInvoice master = Base.Document.Current;
            //貸方調整不做計算
            if (IsCreditAdj(master)) return;

            decimal total = 0m;//估驗
            decimal prepaymentTotal = 0m; //預付
            decimal addTotal = 0m; //加款
            decimal refundTotal = 0m; //保留款退回

            bool isRefundRet = false;

            foreach (APTran apTran in Base.Transactions.Select())
            {
                APTranExt apTranExt = Base.Transactions.Cache.GetExtension<APTranExt>(apTran);
                String valuationType = apTranExt.UsrValuationType ?? ValuationTypeStringList.B;
                decimal curyLineAmt = apTran.CuryLineAmt ?? 0m;
                decimal curyTranAmt = apTran.CuryTranAmt ?? 0m;
                if (IsPrepayment(master))
                {
                    //(預付款) valuationType = 計價(B) 或 預付款(P)
                    if (In(valuationType, ValuationTypeStringList.B, ValuationTypeStringList.P))
                        prepaymentTotal += curyTranAmt;
                }
                else if (IsInvOrDebitAdj(master))
                {
                    //(計價、借方調整) valuationType = 計價(B)
                    if (valuationType == ValuationTypeStringList.B)
                        total += curyLineAmt;
                    //(計價、借方調整) valuationType = 加款(A)
                    else if (valuationType == ValuationTypeStringList.A)
                        addTotal += curyLineAmt;
                    //(計價、借方調整) valuationType = 加款(A)
                    else if (valuationType == ValuationTypeStringList.R)
                        refundTotal += curyLineAmt;
                }
                ///<remarks> Because the APTran event trigger faster than APRegister, use the following field is used as condition.</remarks>
                isRefundRet = !string.IsNullOrEmpty(apTranExt.UsrOrigRetainageDocType);
            }

            KGBillSummary kgSummary = Base2.getKGBillSummary();
            Base2.SummaryAmtFilters.Cache.SetValueExt<KGBillSummary.additionAmt>(kgSummary, addTotal);
            if (master.DocType == APDocType.Prepayment)
                Base2.SummaryAmtFilters.Cache.SetValueExt<KGBillSummary.prepaymentAmt>(kgSummary, prepaymentTotal);
            //保留款退回
            else if (master.IsRetainageDocument == true )
                Base2.SummaryAmtFilters.Cache.SetValueExt<KGBillSummary.retentionReturnAmt>(kgSummary, total);
            else if (isRefundRet == true)
                Base2.SummaryAmtFilters.Cache.SetValueExt<KGBillSummary.retentionReturnAmt>(kgSummary, refundTotal);
            else
                Base2.SummaryAmtFilters.Cache.SetValueExt<KGBillSummary.poValuationAmt>(kgSummary, total);
            Base2.SummaryAmtFilters.Update(kgSummary);
        }

        /// <summary>
        /// 預付款扣回金額
        /// </summary>
        public void APAdjustChange()
        {
            APInvoice master = Base.Document.Current;
            //貸方調整不做計算
            if (IsCreditAdj(master)) return;
            KGBillSummary kgSummary = Base2.getKGBillSummary();
            decimal total = 0m;
            foreach (APAdjust item in Base.Adjustments.Select())
            {
                if (item.DisplayDocType == APDocType.Prepayment)
                    total += item.CuryAdjdAmt ?? 0m;
            }
            if (kgSummary.PrepaymentDuctWithTaxAmt != total)
            {
                Base2.SummaryAmtFilters.Cache.SetValueExt<KGBillSummary.prepaymentDuctWithTaxAmt>(kgSummary, total);
                Base2.SummaryAmtFilters.Update(kgSummary);
            }
        }

        /// <summary>
        /// 稅額
        /// </summary>
        public void APTaxTranChange()
        {
            APInvoice master = Base.Document.Current;
            if (master == null) return;
            //貸方調整不做計算
            if (IsCreditAdj(master)) return;
            decimal curyTaxAmt = 0m;
            decimal curyRetainedTaxAmt = 0m;//保留款稅額
            foreach (APTaxTran item in Base.Taxes.Select())
            {
                curyTaxAmt += (item?.CuryTaxAmt ?? 0m);
                curyRetainedTaxAmt += (item?.CuryRetainedTaxAmt ?? 0m);
            }
            decimal total =
                      curyTaxAmt
                    + curyRetainedTaxAmt;
            KGBillSummary kgSummary = Base2.getKGBillSummary();
            if (master.IsRetainageDocument == true)
            {
                //保留款退回的話稅額為0
                Base2.SummaryAmtFilters.Cache.SetValueExt<KGBillSummary.taxAmt>(kgSummary, 0m);
                //2021-07-21 taxAmt改抓APTaxTran.CuryTaxAmt
                Base2.SummaryAmtFilters.Cache.SetValueExt<KGBillSummary.retentionReturnTaxAmt>(kgSummary, total);
                return;
            }
            Base2.SummaryAmtFilters.Cache.SetValueExt<KGBillSummary.retentionTaxAmt>(kgSummary, curyRetainedTaxAmt);
            Base2.SummaryAmtFilters.Cache.SetValueExt<KGBillSummary.taxAmt>(kgSummary, total);
            Base2.SummaryAmtFilters.Update(kgSummary);
        }

        public void KGDeductionAPTranChange()
        {
            APInvoice master = Base.Document.Current;
            //貸方調整不做計算
            if (IsCreditAdj(master)) return;
            if (!IsInvOrDebitAdj(master)) return;
            KGBillSummary kgSummary = Base2.getKGBillSummary();
            decimal total = 0m;
            decimal totalTax = 0m;
            decimal insDed = 0m;
            decimal stdDed = 0m;
            decimal cheDis = 0m;
            decimal taxRate = Base2.getTaxRate() ?? 0;
            foreach (KGDeductionAPTran item in Base2.deductionAPTranDetails.Select())
            {
                decimal curyLineAmt = (item.CuryLineAmt ?? 0);
                KGValuation valuation = GetKGValuation(item.ValuationID);
                if (valuation?.ValuationType == KGValuationTypeStringList._3)
                    //2021/08/17 Mantis: 0012202 原本為curyLineAmt, 改抓KGValuation.TotalAmt
                    cheDis += valuation.TotalAmt ?? 0;
                else
                {
                    total += curyLineAmt;
                    if ("應稅".Equals(item.TaxCategoryID)) totalTax += curyLineAmt;


                    //代扣代付
                    if (valuation?.ValuationType == "0") insDed += curyLineAmt;
                    //非代扣代付
                    else if (valuation != null) stdDed += curyLineAmt;
                }
            }
            totalTax = Base2.round(totalTax * taxRate) ?? 0;

            Base2.SummaryAmtFilters.Cache.SetValueExt<KGBillSummary.deductionAmt>(kgSummary, total);
            Base2.SummaryAmtFilters.Cache.SetValueExt<KGBillSummary.deductionTaxAmt>(kgSummary, totalTax);
            Base2.SummaryAmtFilters.Cache.SetValueExt<KGBillSummary.deductionWithTaxAmt>(kgSummary, total + totalTax);
            Base2.SummaryAmtFilters.Cache.SetValueExt<KGBillSummary.insDeductionAmt>(kgSummary, insDed);
            Base2.SummaryAmtFilters.Cache.SetValueExt<KGBillSummary.stdDeductionAmt>(kgSummary, stdDed);
            Base2.SummaryAmtFilters.Cache.SetValueExt<KGBillSummary.checkDiscountAmt>(kgSummary, cheDis);
            Base2.SummaryAmtFilters.Update(kgSummary);

        }

        /// <summary>
        /// 是否為 計價 或 借方調整
        /// </summary>
        /// <param name="master"></param>
        /// <returns></returns>
        public bool IsInvOrDebitAdj(APInvoice master)
        {
            return In(master.DocType, APDocType.Invoice, APDocType.DebitAdj);
        }

        /// <summary>
        /// 是否為預付款
        /// </summary>
        /// <param name="master"></param>
        /// <returns></returns>
        public bool IsPrepayment(APInvoice master)
        {
            return master.DocType == APDocType.Prepayment;
        }

        /// <summary>
        /// 是否為貸方調整
        /// </summary>
        /// <param name="master"></param>
        /// <returns></returns>
        public bool IsCreditAdj(APInvoice master)
        {
            return master.DocType == APDocType.CreditAdj;
        }

        public PXResultset<KGBillSummary> GetCumKGBillSummary(KGBillSummary thisSum, APInvoice master, APRegisterExt masterExt)
        {
            int? usrValuationPhase = masterExt.UsrValuationPhase ?? Base2.getUsrValuationPhase();
            return GetCumKGBillSummary
                (thisSum.BillSummaryID,
                 master.ProjectID,
                 masterExt.UsrPONbr,
                 masterExt.UsrPOOrderType,
                 usrValuationPhase,
                 master.VendorID);
        }

        public bool In(object baseVal, params object[] paramVals)
        {
            bool flag = false;
            foreach (object paramVal in paramVals)
            {
                flag = flag || (paramVal.Equals(baseVal));
            }
            return flag;
        }
        #endregion

        #region BQLs
        public POOrder GetPOOrder(string poNbr, string orderType)
        {
            return PXSelect<POOrder,
                Where<POOrder.orderType, Equal<Required<POOrder.orderType>>,
                And<POOrder.orderNbr, Equal<Required<POOrder.orderNbr>>>>>
                .Select(Base, orderType, poNbr);
        }

        public KGValuation GetKGValuation(int? valuationID)
        {
            return PXSelectReadonly<KGValuation,
                Where<KGValuation.valuationID, Equal<Required<KGValuation.valuationID>>>>
               .Select(Base, valuationID);
        }

        /// <summary>
        /// 取得前期KGBillSummary
        /// </summary>
        /// <returns></returns>
        public PXResultset<KGBillSummary> GetCumKGBillSummary(int? thisSumID, int? projectID, string poNbr, string poOrderType, int? valuationPhase, int? vendorID)
        {
            return PXSelectJoin<KGBillSummary,
                InnerJoin<APInvoice,
                    On<APInvoice.refNbr, Equal<KGBillSummary.refNbr>,
                    And<APInvoice.docType, Equal<KGBillSummary.docType>>>,
                InnerJoin<APRegister,
                    On<APRegister.refNbr, Equal<KGBillSummary.refNbr>,
                    And<APRegister.docType, Equal<KGBillSummary.docType>>>>>,
                Where<KGBillSummary.billSummaryID, NotEqual<Required<KGBillSummary.billSummaryID>>,//billSummaryID
                And<APInvoice.projectID, Equal<Required<APInvoice.projectID>>,//ProjectID
                And<APRegisterExt.usrPONbr, Equal<Required<APRegisterExt.usrPONbr>>,//PoNbr
                And<APRegisterExt.usrPOOrderType, Equal<Required<APRegisterExt.usrPOOrderType>>,//PoOrderType
                And<APRegisterExt.usrValuationPhase, Less<Required<APRegisterExt.usrValuationPhase>>,//ValuationPhase
                And<APRegister.vendorID, Equal<Required<APRegister.vendorID>>//vendorID
                >>>>>>
                >.Select(new PXGraph(), thisSumID, projectID, poNbr, poOrderType, valuationPhase, vendorID);
        }
        #endregion
    }
}
