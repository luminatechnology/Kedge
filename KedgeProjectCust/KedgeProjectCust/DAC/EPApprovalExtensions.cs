using PX.Data;
using PX.Objects.CM;
using PX.Objects.CR;
using PX.Objects.EP;
using PX.Objects;
using PX.TM;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;

namespace PX.Objects.EP
{
  public class EPApprovalExt : PXCacheExtension<PX.Objects.EP.EPApproval>
  {
    #region UsrReturnUrl
    [PXDBString(2048)]
    [PXUIField(DisplayName="ReturnUrl")]

    public virtual string UsrReturnUrl { get; set; }
    public abstract class usrReturnUrl : PX.Data.BQL.BqlString.Field<usrReturnUrl> { }
    #endregion
  }
}