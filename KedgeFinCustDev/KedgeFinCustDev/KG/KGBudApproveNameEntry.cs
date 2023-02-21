using System;
using PX.Data;
using Kedge.DAC;

namespace Kedge
{
    public class KGBudApproveNameEntry : PXGraph<KGBudApproveNameEntry, KGBudApproveName>
    {
        public PXSelect<KGBudApproveName> BudApproveNames;

        public KGBudApproveNameEntry()
        {
            if(getApproveName(this.Accessinfo.BranchID) !=null)
            {
                BudApproveNames.Current = getApproveName(this.Accessinfo.BranchID);
            }
        }

        private KGBudApproveName getApproveName(int? BranchID)
        {
            return PXSelect<KGBudApproveName,
                Where<KGBudApproveName.branch, Equal<Required<KGBudApproveName.branch>>>>
                .Select(this, BranchID);
        }
    }
}