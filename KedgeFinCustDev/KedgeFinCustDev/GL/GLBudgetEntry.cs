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
 * 	1.�w����@��, �i���b�s�שάO���ʮ�, �ݭn�ˬd���ʫ᪺�w��, ����p��w�g�ϥιw����B
 * 	2.�w�g�ϥΪ��w��Эp��w��View���U�C��� : BatchAmt + EPTranAmt + APTranAmt - ADRAmt
 * 	3.�w��View�@���T�� KGCurrAdminBudgetV(�t�Τ��~�׹w��), KGPastAdminBudgetV(�t�Τ�W�@�~�׹w��), KGNextAdminBudgetV(�t�Τ�U�@�~�׹w��), �Ш̷�GLBudgetLine.FinYear�P�_�������View�����
 * 	4.�p�G���ʫ᪺�w��p��w�g�ϥιw����B, ����ܿ��~�T���b�����䪺�����, �T���� "�w����B����p��w�ϥιw����B("+ �ϥΪ��w����B + ")"
 * 
 * ====2021-07-27:12157====Alton
 * 1.�w��ק�ɪ��d��, �ݭn�P�_GLBudgetLine.UsrBudgetCheckType, �p�GUsrBudgetCheckType = 'YTD', �ݭn�֭p��~�@���ק��몺�`�w��β֭p'�w�ιL�����B', �M��h����ק�᪺�w��, �O���O�C��w�ϥΪ��w����B. �p�G�O'YTT', �h�ݭn�֭p���~�ת��w����B�P���~�ת�'�w�ιL�����B'�h���
 * 2.�p�GGLBudgetLine.UsrIsBudgetCheck������true, �ק�w��O�����ˮ�
 * 
 * **/
namespace PX.Objects.GL
{
    public class GLBudgetEntry_Extension : PXGraphExtension<GLBudgetEntry>
    {
        #region Message
        public const string USE_BUDGET_MSG = "�w��{0}����p��w�ϥιw����B({1:0.##})";
        #endregion

        #region Event Handlers
        #region GLBudgetLine
        protected virtual void _(Events.RowPersisting<GLBudgetLine> e)
        {
            GLBudgetLine row = e.Row;
            if (row == null) return;
            GLBudgetLineExt rowExt = PXCache<GLBudgetLine>.GetExtension<GLBudgetLineExt>(row);
            //2021-07-27:12157 �p�GGLBudgetLine.UsrIsBudgetCheck������true, �ק�w��O�����ˮ�
            if (rowExt.UsrIsBudgetCheck != true) return;
            bool isYTD = rowExt.UsrBudgetCheckType == "YTD";
            Dictionary<string, decimal> useBudgets = new Dictionary<string, decimal>();
            string[] ids = { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13" };
            //�w�ϥζq
            decimal totalUse = 0m;
            foreach (string seq in ids)
            {
                string finPeriodID = row.FinYear + seq;
                decimal useBudget = GetUseBudget(row.LedgerID, row.FinYear, row.AccountID, row.SubID, finPeriodID);
                useBudgets.Add(finPeriodID, totalUse + useBudget);
                //�֥[�w�ϥζq
                totalUse += useBudget;
            }

            //�w��
            decimal total = 0m;
            foreach (GLBudgetLineDetail item in GetGLBudgetLineDetail(row.LedgerID, row.FinYear, row.AccountID, row.SubID, row.GroupID))
            {
                //�֥[�w��
                total += (item.Amount ?? 0m);
                if (isYTD && total < useBudgets[item.FinPeriodID])
                {
                    string index = item.FinPeriodID.Substring(item.FinPeriodID.Length - 2);
                    SetError<GLBudgetLine.amount>(e.Cache, row, row.Amount, String.Format(USE_BUDGET_MSG, "����" + index, useBudgets[item.FinPeriodID]), PXErrorLevel.RowError);
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
        /// ���o�����~�רϥιw��
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
            {//��~��
                KGCurrAdminBudgetV budge = GetKGCurrAdminBudgetV(ledgerID, finYear, accountID, subID, finPeriodID);
                useBudget =
                    (budge?.BatchAmt ?? 0m) +
                    (budge?.EPTranAmt ?? 0m) +
                    (budge?.APTranAmt ?? 0m) -
                    (budge?.Adramt ?? 0m);
            }
            else if (dataYear > sysYear)
            {//�U�@�~��
                KGNextAdminBudgetV budge = GetKGNextAdminBudgetV(ledgerID, finYear, accountID, subID, finPeriodID);
                useBudget =
                    (budge?.BatchAmt ?? 0m) +
                    (budge?.EPTranAmt ?? 0m) +
                    (budge?.APTranAmt ?? 0m) -
                    (budge?.Adramt ?? 0m);
            }
            else
            {//�W�@�~��
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
        /// ���o��~�׹w��ϥθ�T
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
        /// ���o�W�@�~�׹w��ϥθ�T
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
        /// ���o�U�@�~�׹w��ϥθ�T
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