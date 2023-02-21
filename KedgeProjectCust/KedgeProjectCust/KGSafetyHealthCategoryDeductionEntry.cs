using System;
using PX.Data;
using Kedge.DAC;

namespace Kedge
{
    public class KGSafetyHealthCategoryDeductionEntry : PXGraph<KGSafetyHealthCategoryDeductionEntry>
    {
        public PXSave<KGSafetyHealthCategoryDeductionSetup> Save;
        public PXCancel<KGSafetyHealthCategoryDeductionSetup> Cancel;
        public PXSelect<KGSafetyHealthCategoryDeductionSetup> SafetyHealthCategoryDeduction;


    }
}