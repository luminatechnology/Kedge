using PX.Data.EP;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.CM;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.EP;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.PM;
using PX.Objects.TX;
using PX.Objects;
using PX.TM;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;

namespace PX.Objects.PM
{
  public class PMTaskExt : PXCacheExtension<PX.Objects.PM.PMTask>
  {
    #region UsrActualStartDate
    [PXDBDate]
    [PXUIField(DisplayName="ActualStartDate")]

    public virtual DateTime? UsrActualStartDate { get; set; }
    public abstract class usrActualStartDate : IBqlField { }
    #endregion

    #region UsrActualEndDate
    [PXDBDate]
    [PXUIField(DisplayName="ActualEndDate")]

    public virtual DateTime? UsrActualEndDate { get; set; }
    public abstract class usrActualEndDate : IBqlField { }
    #endregion

    #region UsrConstructedPct
    [PXDBDecimal]
    [PXUIField(DisplayName = "ConstructedPct")]

    public virtual Decimal? UsrConstructedPct { get; set; }
    public abstract class usrConstructedPct : IBqlField { }
        #endregion

    #region UsrSchConstructedPct
    [PXDBDecimal]
    [PXUIField(DisplayName = "SchConstructedPct")]

    public virtual Decimal? UsrSchConstructedPct { get; set; }
    public abstract class usrSchConstructedPct : IBqlField { }
    #endregion
    }
}