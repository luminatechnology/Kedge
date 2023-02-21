using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using PX.Data;
using PX.Objects.CM;
using PX.Objects.GL.DAC;
using PX.Objects.GL.FinPeriods;
using PX.Objects.GL.FinPeriods.TableDefinition;
using PX.Objects.GL.Descriptor;
using PX.SM;
using PX.Objects;
using PX.Objects.GL;
using Kedge.DAC;

/**
 * ====2021-07-15:12157====Alton
 * 	1.預算維護時, 可坐在存擋或是異動時, 需要檢查異動後的預算, 不能小於已經使用預算金額
 * 	2.已經使用的預算請計算預算View的下列欄位 : BatchAmt + EPTranAmt + APTranAmt - ADRAmt
 * 	3.預算View共有三個 KGCurrAdminBudgetV(系統日當年度預算), KGPastAdminBudgetV(系統日上一年度預算), KGNextAdminBudgetV(系統日下一年度預算), 請依照GLBudgetLine.FinYear判斷比較那個View的資料
 * 	4.如果異動後的預算小於已經使用預算金額, 請顯示錯誤訊息在欄位旁邊的紅色Ｘ, 訊息為 "預算金額不能小於已使用預算金額("+ 使用的預算金額 + ")"
 * 
 * ====2021-07-27:12157====Alton
 * 1.預算修改時的卡控, 需要判斷GLBudgetLine.UsrBudgetCheckType, 如果UsrBudgetCheckType = 'YTD', 需要累計當年一月到修改當月的總預算及累計'已用過的金額', 然後去比較修改後的預算, 是不是低於已使用的預算金額. 如果是'YTT', 則需要累計全年度的預算金額與全年度的'已用過的金額'去比較
 * 2.如果GLBudgetLine.UsrIsBudgetCheck不等於true, 修改預算是不用檢核
 * 
 * **/
namespace PX.Objects.GL
{
    public class GLBudgetEntry_Extension : PXGraphExtension<GLBudgetEntry>
    {
        #region Message
        public const string USE_BUDGET_MSG = "預算{0}不能小於已使用預算金額({1:0.##})";
        #endregion

        #region Event Handlers
        #region GLBudgetLine
        protected virtual void _(Events.RowPersisting<GLBudgetLine> e)
        {
            GLBudgetLine row = e.Row;
            if (row == null) return;
            GLBudgetLineExt rowExt = PXCache<GLBudgetLine>.GetExtension<GLBudgetLineExt>(row);
            //2021-07-27:12157 如果GLBudgetLine.UsrIsBudgetCheck不等於true, 修改預算是不用檢核
            if (rowExt.UsrIsBudgetCheck != true) return;
            bool isYTD = rowExt.UsrBudgetCheckType == "YTD";
            Dictionary<string, decimal> useBudgets = new Dictionary<string, decimal>();
            string[] ids = { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13" };
            //已使用量
            decimal totalUse = 0m;
            foreach (string seq in ids)
            {
                string finPeriodID = row.FinYear + seq;
                decimal useBudget = GetUseBudget(row.LedgerID, row.FinYear, row.AccountID, row.SubID, finPeriodID);
                useBudgets.Add(finPeriodID, totalUse + useBudget);
                //累加已使用量
                totalUse += useBudget;
            }

            //預算
            decimal total = 0m;
            foreach (GLBudgetLineDetail item in GetGLBudgetLineDetail(row.LedgerID, row.FinYear, row.AccountID, row.SubID, row.GroupID))
            {
                //累加預算
                total += (item.Amount ?? 0m);
                if (isYTD && total < useBudgets[item.FinPeriodID])
                {
                    string index = item.FinPeriodID.Substring(item.FinPeriodID.Length - 2);
                    SetError<GLBudgetLine.amount>(e.Cache, row, row.Amount, String.Format(USE_BUDGET_MSG, "期間" + index, useBudgets[item.FinPeriodID]), PXErrorLevel.RowError);
                }
            }
            if (!isYTD && total < totalUse)
            {
                SetError<GLBudgetLine.amount>(e.Cache, row, row.Amount, String.Format(USE_BUDGET_MSG, "", totalUse), PXErrorLevel.RowError);
            }

        }

        protected virtual void _(Events.FieldUpdated<GLBudgetLine, GLBudgetLineExt.usrIsBudgetCheck> e)
        {
            GLBudgetLine row = e.Row as GLBudgetLine;
            if (row == null) return;
            GLBudgetLineExt ext = PXCache<GLBudgetLine>.GetExtension<GLBudgetLineExt>(row);
            if ((ext.UsrIsBudgetCheck ?? false) == false)
            {
                ext.UsrBudgetCheckType = null;
                Base.Filter.Cache.SetValueExt<GLBudgetLineExt.usrBudgetCheckType>(ext, null);
            }
        }
        #endregion

        #endregion

        #region Method
        private bool SetError<Field>(PXCache cache, object row, object newValue, String errorMsg, PXErrorLevel errorLevel) where Field : PX.Data.IBqlField
        {
            cache.RaiseExceptionHandling<Field>(row, newValue,
                  new PXSetPropertyException(errorMsg, errorLevel));
            return false;
        }

        /// <summary>
        /// 取得對應年度使用預算
        /// </summary>
        /// <param name="ledgerID"></param>
        /// <param name="finYear"></param>
        /// <param name="accountID"></param>
        /// <param name="subID"></param>
        /// <param name="finPeriodID"></param>
        /// <returns></returns>
        public virtual decimal GetUseBudget(int? ledgerID, string finYear, int? accountID, int? subID, string finPeriodID)
        {
            int? sysYear = Base.Accessinfo.BusinessDate?.Year;
            int? dataYear = int.Parse(finYear);
            decimal useBudget = 0;

            if (dataYear == sysYear)
            {//當年度
                KGCurrAdminBudgetV budge = GetKGCurrAdminBudgetV(ledgerID, finYear, accountID, subID, finPeriodID);
                useBudget =
                    (budge?.BatchAmt ?? 0m) +
                    (budge?.EPTranAmt ?? 0m) +
                    (budge?.APTranAmt ?? 0m) -
                    (budge?.Adramt ?? 0m);
            }
            else if (dataYear > sysYear)
            {//下一年度
                KGNextAdminBudgetV budge = GetKGNextAdminBudgetV(ledgerID, finYear, accountID, subID, finPeriodID);
                useBudget =
                    (budge?.BatchAmt ?? 0m) +
                    (budge?.EPTranAmt ?? 0m) +
                    (budge?.APTranAmt ?? 0m) -
                    (budge?.Adramt ?? 0m);
            }
            else
            {//上一年度
                KGPastAdminBudgetV budge = GetKGPastAdminBudgetV(ledgerID, finYear, accountID, subID, finPeriodID);
                useBudget =
                    (budge?.BatchAmt ?? 0m) +
                    (budge?.EPTranAmt ?? 0m) +
                    (budge?.APTranAmt ?? 0m) -
                    (budge?.Adramt ?? 0m);
            }
            return useBudget;
        }
        #endregion

        #region BQL
        private PXResultset<GLBudgetLineDetail> GetGLBudgetLineDetail(int? ledgerID, string finYear, int? accountID, int? subID, Guid? groupID)
        {
            return PXSelect<GLBudgetLineDetail,
                Where<GLBudgetLineDetail.ledgerID, Equal<Required<GLBudgetLineDetail.ledgerID>>,
                And<GLBudgetLineDetail.finYear, Equal<Required<GLBudgetLineDetail.finYear>>,
                And<GLBudgetLineDetail.accountID, Equal<Required<GLBudgetLineDetail.accountID>>,
                And<GLBudgetLineDetail.subID, Equal<Required<GLBudgetLineDetail.subID>>,
                And<GLBudgetLineDetail.groupID, Equal<Required<GLBudgetLineDetail.groupID>>>>>>>,
                OrderBy<Asc<GLBudgetLineDetail.finPeriodID>>>.Select(Base, ledgerID, finYear, accountID, subID, groupID);

        }

        /// <summary>
        /// 取得當年度預算使用資訊
        /// </summary>
        /// <returns></returns>
        private KGCurrAdminBudgetV GetKGCurrAdminBudgetV(int? ledgerID, string finYear, int? accountID, int? subID, string finPeriodID)
        {
            return PXSelect<KGCurrAdminBudgetV,
                Where<KGCurrAdminBudgetV.ledgerID, Equal<Required<KGCurrAdminBudgetV.ledgerID>>,
                And<KGCurrAdminBudgetV.finYear, Equal<Required<KGCurrAdminBudgetV.finYear>>,
                And<KGCurrAdminBudgetV.accountID, Equal<Required<KGCurrAdminBudgetV.accountID>>,
                And<KGCurrAdminBudgetV.subid, Equal<Required<KGCurrAdminBudgetV.subid>>,
                And<KGCurrAdminBudgetV.finPeriodID, Equal<Required<KGCurrAdminBudgetV.finPeriodID>>>>>>
                >>.Select(new PXGraph(), ledgerID, finYear, accountID, subID, finPeriodID);
        }

        /// <summary>
        /// 取得上一年度預算使用資訊
        /// </summary>
        /// <returns></returns>
        private KGPastAdminBudgetV GetKGPastAdminBudgetV(int? ledgerID, string finYear, int? accountID, int? subID, string finPeriodID)
        {
            return PXSelect<KGPastAdminBudgetV,
                Where<KGPastAdminBudgetV.ledgerID, Equal<Required<KGPastAdminBudgetV.ledgerID>>,
                And<KGPastAdminBudgetV.finYear, Equal<Required<KGPastAdminBudgetV.finYear>>,
                And<KGPastAdminBudgetV.accountID, Equal<Required<KGPastAdminBudgetV.accountID>>,
                And<KGPastAdminBudgetV.subid, Equal<Required<KGPastAdminBudgetV.subid>>,
                And<KGPastAdminBudgetV.finPeriodID, Equal<Required<KGPastAdminBudgetV.finPeriodID>>>>>>
                >>.Select(new PXGraph(), ledgerID, finYear, accountID, subID, finPeriodID);
        }

        /// <summary>
        /// 取得下一年度預算使用資訊
        /// </summary>
        /// <returns></returns>
        private KGNextAdminBudgetV GetKGNextAdminBudgetV(int? ledgerID, string finYear, int? accountID, int? subID, string finPeriodID)
        {
            return PXSelect<KGNextAdminBudgetV,
                Where<KGNextAdminBudgetV.ledgerID, Equal<Required<KGNextAdminBudgetV.ledgerID>>,
                And<KGNextAdminBudgetV.finYear, Equal<Required<KGNextAdminBudgetV.finYear>>,
                And<KGNextAdminBudgetV.accountID, Equal<Required<KGNextAdminBudgetV.accountID>>,
                And<KGNextAdminBudgetV.subid, Equal<Required<KGNextAdminBudgetV.subid>>,
                And<KGNextAdminBudgetV.finPeriodID, Equal<Required<KGNextAdminBudgetV.finPeriodID>>>>>>
                >>.Select(new PXGraph(), ledgerID, finYear, accountID, subID, finPeriodID);
        }
        #endregion
    }
}