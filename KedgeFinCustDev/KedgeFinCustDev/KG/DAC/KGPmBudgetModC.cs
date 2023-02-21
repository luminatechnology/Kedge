using System;
using PX.Data;

namespace Kedge.DAC
{
    [Serializable]
    public class KGPmBudgetModC : KGPmBudgetMod
    {
        #region ModifyClass
        [PXDBString(1, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Modify Class")]
        [PXDefault("C")]
        public override string ModifyClass { get; set; }
        #endregion
    }
}