using PX.Data;
using PX.Objects.GL;
using PX.Objects;
using System.Collections.Generic;
using System;

namespace PX.Objects.GL
{
    public class GLBudgetLineExt : PXCacheExtension<PX.Objects.GL.GLBudgetLine>
    {

        #region UsrIsBudgetCheck
        [PXDBBool]
        [PXUIField(DisplayName = "Is Budget Check")]
        [PXDefault(false,PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual bool? UsrIsBudgetCheck { get; set; }
        public abstract class usrIsBudgetCheck : PX.Data.BQL.BqlBool.Field<usrIsBudgetCheck> { }
        #endregion

        #region UsrBudgetCheckType
        [PXDBString]
        [PXUIField(DisplayName = "Budget Check Type")]
        [PXUIEnabled(typeof(Where<usrIsBudgetCheck,Equal<True>>))]
        [PXUIRequired(typeof(Where<usrIsBudgetCheck, Equal<True>>))]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXStringList(
            new string[] { "YTD", "YTT" },
            new string[] { "累計金額", "年度總額" }
            )]
        public virtual string UsrBudgetCheckType { get; set; }
        public abstract class usrBudgetCheckType : PX.Data.BQL.BqlString.Field<usrBudgetCheckType> { }
        #endregion
    }
}