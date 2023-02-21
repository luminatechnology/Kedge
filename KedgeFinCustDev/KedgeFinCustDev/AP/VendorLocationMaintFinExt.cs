using PX.Data;
using PX.Objects.CR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PX.Objects.AP
{
    /**
     * =====2021-02-18:11950 ====== Alton
     * 1. Location 新增一個欄位 UsrIsReserveAcct, Bits, 預設 0
     * 2. 請將UsrIsReserveAcct 放在AP303010 供應商所在地的付款設定-->預設付款設定 的付款方法上面, 型態 checkbox, 非必填.
     * 
     * **/
    public class VendorLocationMaintFinExt : PXGraphExtension<VendorLocationMaint>
    {

        #region Event
        #region LocationAPPaymentInfo
        protected virtual void LocationAPPaymentInfo_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
        {
            LocationAPPaymentInfo record = (LocationAPPaymentInfo)e.Row;
            LocationAPPaymentInfoFinExt recordExt = sender.GetExtension<LocationAPPaymentInfoFinExt>(record);
            if (!sender.ObjectsEqual<
                LocationAPPaymentInfoFinExt.usrIsReserveAcct>(e.Row, e.OldRow))
            {
                Location mainloc = Base.Location.Current;
                LocationFinExt mainlocExt = Base.Location.Cache.GetExtension<LocationFinExt>(mainloc);
                mainlocExt.UsrIsReserveAcct = recordExt.UsrIsReserveAcct;

                Base.Location.Cache.MarkUpdated(mainloc);

                sender.Graph.Caches[typeof(Location)].IsDirty = true;
            }
        }

        #endregion

        #endregion

    }

    public class LocationAPPaymentInfoFinExt : PXCacheExtension<LocationAPPaymentInfo>
    {
        #region UsrIsReserveAcct
        [PXBool()]
        [PXUnboundDefault(typeof(Search<LocationFinExt.usrIsReserveAcct, Where<Location.locationID, Equal<Current<LocationAPPaymentInfo.locationID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Is Reserve Acct")]
        public virtual bool? UsrIsReserveAcct { get; set; }
        public abstract class usrIsReserveAcct : PX.Data.BQL.BqlBool.Field<usrIsReserveAcct> { }
        #endregion
    }
}
