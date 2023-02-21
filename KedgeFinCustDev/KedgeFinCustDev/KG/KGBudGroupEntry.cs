using System;
using PX.Data;
using Kedge.DAC;
using PX.Data.BQL.Fluent;


/**
 * ====2021-06-08:12083====Alton
 * 1.KGBudGroup �s�W�@�����, IsTravelExpense, bits, �w�]0
 * 2.�e���Цb�����W�٥k���IsTravelExpense, checkbox, �D����
 * ====2021-06-08:12083====Alton
 * 1.Fix�ˮ��޿�
 * 2.�K�[Excel�W��
 * 
 * **/
namespace KedgeFinCustDev
{
  public class KGBudGroupEntry : PXGraph<KGBudGroupEntry>
    {
        [PXImport(typeof(KGBudGroup))]
        public SelectFrom<KGBudGroup>
            .OrderBy<KGBudGroup.branchID.Asc,KGBudGroup.budGroupNO.Asc>.View BudGroup;

        public PXSave<KGBudGroup> Save;
        public PXCancel<KGBudGroup> Cancel;

        #region Event
        protected virtual void _(Events.RowPersisting<KGBudGroup> e)
        {
            KGBudGroup row = e.Row;
            if (row == null) return;
            if (row.BudGroupNO != null)
            {
                foreach (KGBudGroup group in BudGroup.Select())
                {
                    if (row.BudGroupID == group.BudGroupID) continue;
                    if (group.BudGroupNO == row.BudGroupNO && group.BranchID == row.BranchID)
                    {
                        SetError<KGBudGroup.budGroupNO>(e.Cache,row, row.BudGroupNO, "�P�@��GroupNo���୫��!!!.", PXErrorLevel.Error);
                    }
                }
            }

        }
        #endregion

        #region Method
        private bool SetError<Field>(PXCache cache, object row, object newValue, String errorMsg,PXErrorLevel errorLevel) where Field : PX.Data.IBqlField
        {
            cache.RaiseExceptionHandling<Field>(row, newValue,
                  new PXSetPropertyException(errorMsg, errorLevel));
            return false;
        }

        #endregion


    }
}