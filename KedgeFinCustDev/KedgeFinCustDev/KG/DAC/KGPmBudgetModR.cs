using System;
using PX.Data;

namespace Kedge.DAC
{
    [Serializable]
    public class KGPmBudgetModR : KGPmBudgetMod
    { 
        #region ModifyClass
        [PXDBString(1, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Modify Class")]
        [PXDefault("R")]
        public virtual string ModifyClass { get; set; }
        public abstract class modifyClass : PX.Data.BQL.BqlString.Field<modifyClass> { }
        #endregion

    }
}