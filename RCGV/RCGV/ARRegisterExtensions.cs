using ARCashSale = PX.Objects.AR.Standalone.ARCashSale;
using CRLocation = PX.Objects.CR.Standalone.Location;
using IRegister = PX.Objects.CM.IRegister;
using PX.Data.EP;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Data;
using PX.Objects.AR.BQL;
using PX.Objects.AR;
using PX.Objects.CM;
using PX.Objects.Common.Abstractions;
using PX.Objects.Common.MigrationMode;
using PX.Objects.Common;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects;
using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace PX.Objects.AR
{
  public class ARRegisterExt : PXCacheExtension<PX.Objects.AR.ARRegister>
  {
    #region UsrArGuiInvoiceNbr
    [PXDBString(40)]
    [PXUIField(DisplayName= "AR Gui Invoice Nbr",Enabled =false)]
    public virtual string UsrArGuiInvoiceNbr { get; set; }
    public abstract class usrArGuiInvoiceNbr: IBqlField { }
	
	//protected ARRegister Base { get; }
    #endregion
	
  }
    public class ARRegisterExtendsion : ARRegister
	{
        #region UsrArGuiInvoiceNbr
        [PXDBString(40)]
        [PXUIField(DisplayName = "AR Gui Invoice Nbr", Enabled = false)]
        public virtual string UsrArGuiInvoiceNbr { get; set; }
        public abstract class usrArGuiInvoiceNbr : IBqlField { }

        //protected ARRegister Base { get; }
        #endregion
    }
}