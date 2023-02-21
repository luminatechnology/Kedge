using PX.Common;
using PX.Data.DependencyInjection;
using PX.Data;
using PX.LicensePolicy;
using PX.Objects.AR;
using PX.Objects.CA;
using PX.Objects.CM;
using PX.Objects.Common;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.CT;
using PX.Objects.EP;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.PM;
using PX.Objects;
using PX.SM;
using PX.TM;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System;

namespace PX.Objects.PM
{
      [PXNonInstantiatedExtension]
  public class PM_ProjectEntry_CostBudgetFilter_ExistingColumn : PXCacheExtension<PX.Objects.PM.ProjectEntry.CostBudgetFilter>
  {
      #region GroupByTask  
        [PXMergeAttributes(Method = MergeMethod.Append)]

      public bool? GroupByTask { get; set; }
      #endregion
  }

  public class CostBudgetFilterExt : PXCacheExtension<PX.Objects.PM.ProjectEntry.CostBudgetFilter>
  {
    #region UsrGroupByCostCode
    [PXBool]
    [PXUIField(DisplayName="GroupByCostCode", Visible = false)]
    public virtual bool? UsrGroupByCostCode { get; set; }
    public abstract class usrGroupByCostCode : IBqlField { }
    #endregion
  }
}