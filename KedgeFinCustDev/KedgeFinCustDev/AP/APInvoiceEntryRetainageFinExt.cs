using PX.Data;
using PX.Objects.CT;
using PX.Objects.PM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/**
 * ===
 * 
 * **/
namespace PX.Objects.AP
{
    public class APInvoiceEntryRetainageFinExt : PXGraphExtension<APInvoiceEntryRetainage, APInvoiceEntry>
    {
        bool isInsertAPInvoice = false;

        protected void _(Events.RowInserted<APInvoice> e)
        {
            APInvoice row = e.Row;
            //放行保留款(保留款退回)才做處理
            if (row == null || row.IsRetainageDocument == false) return;
            APRegisterFinExt rExt = e.Cache.GetExtension<APRegisterFinExt>(row);
            isInsertAPInvoice = true;
            rExt.UsrConfirmBy = null;
            rExt.UsrConfirmDate = null;
            rExt.UsrAccConfirmNbr = null;
            rExt.UsrIsConfirm = null;
        }
        protected void _(Events.RowPersisting<APTran> e)
        {
            APTran row = e.Row;
            if (row == null || isInsertAPInvoice == false) return;
            APInvoice master = Base.Document.Current;
            row.ProjectID = master.ProjectID;
            PMTask task = GetDefTask(row.ProjectID);
            if (task == null)
            {
                Contract c = GetContract(row.ProjectID);
                SetError<APTran.taskID>(e.Cache, row, row.TaskID, String.Format("請設定專案({0})預設任務", c?.ContractCD));
            }
            else
            {
                row.TaskID = task.TaskID;
            }
            PMCostBudget costb = GetPMCostBudget(row.ProjectID);
            row.CostCodeID = costb?.CostCodeID;

        }

        #region Method
        protected bool SetError<Field>(PXCache cache, object row, object newValue, String errorMsg) where Field : PX.Data.IBqlField
        {
            cache.RaiseExceptionHandling<Field>(row, newValue,
                  new PXSetPropertyException(errorMsg, PXErrorLevel.Error));
            return false;
        }
        #endregion

        #region BQL
        protected PMCostBudget GetPMCostBudget(int? projectID)
        {
            return PXSelect<PMCostBudget,
                Where<PMCostBudget.projectID, Equal<Required<PMCostBudget.projectID>>>,
                OrderBy<Desc<PMCostBudget.costCodeID>>>
                .Select(Base, projectID);
        }

        protected Contract GetContract(int? projectID)
        {
            return PXSelect<Contract,
                Where<Contract.contractID, Equal<Required<Contract.contractID>>>>
                .Select(Base, projectID);
        }

        protected PMTask GetDefTask(int? projectID)
        {
            return PXSelect<PMTask,
                Where<PMTask.isDefault, Equal<True>,
                And<PMTask.projectID, Equal<Required<PMTask.projectID>>>>>
                .Select(Base, projectID);
        }
        #endregion

    }
}
