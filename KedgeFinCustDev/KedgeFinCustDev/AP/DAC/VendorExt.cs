using PX.Data;
using System;

namespace PX.Objects.AP
{
    [Serializable]
    public class VendorExt : PXCacheExtension<Vendor>
    {
        #region UsrPersonalID
        [PXDBString(10, IsUnicode = true)]
        [PXUIField(DisplayName = "Personal ID")]

        public virtual string UsrPersonalID { get; set; }
        public abstract class usrPersonalID : PX.Data.BQL.BqlString.Field<usrPersonalID> { }
        #endregion

        #region UsrIsAlertVendor
        [PXDBBool()]
        [PXUIField(DisplayName = "UsrIsAlertVendor")]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]

        public virtual bool? UsrIsAlertVendor { get; set; }
        public abstract class usrIsAlertVendor : PX.Data.BQL.BqlBool.Field<usrIsAlertVendor> { }
        #endregion
    }
}
