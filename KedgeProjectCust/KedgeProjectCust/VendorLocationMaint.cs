using System;
using PX.Data;
using System.Collections.Generic;
using System.Collections;
using PX.Objects.GL;
using PX.Objects.CR;
using PX.Objects.CA;
using PX.Objects.Common;
using CRLocation = PX.Objects.CR.Standalone.Location;
using PX.Objects.CS;
using PX.Objects;
using PX.Objects.AP;

namespace PX.Objects.AP
{
  public class VendorLocationMaint_Extension : PXGraphExtension<VendorLocationMaint>
  {
        #region Events
        protected virtual void _(Events.RowSelected<Location> e)
        {
            Location row = e.Row;
            PXUIFieldAttribute.SetRequired<Location.taxRegistrationID>(Base.Location.Cache, true);
        }
        protected virtual void _(Events.RowPersisting<Location> e)
        {
            Location row = e.Row;
            if (row == null) return;
            if (row.TaxRegistrationID == null)
                Base.Location.Cache.RaiseExceptionHandling<Location.taxRegistrationID>(e.Row, row.TaxRegistrationID, new PXSetPropertyException("此欄位不可為空!"));


        }
        #endregion
    }
}