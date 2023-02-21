using PX.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PX.Objects.CR
{
    /**
     * =====2021-02-18:11950 ====== Alton
     * 1. Location 新增一個欄位 UsrIsReserveAcct, Bits, 預設 0
     * 2. 請將UsrIsReserveAcct 放在AP303010 供應商所在地的付款設定-->預設付款設定 的付款方法上面, 型態 checkbox, 非必填.
     * 
     * **/
    public class LocationFinExt: PXCacheExtension<Location>
    {
        #region UsrIsReserveAcct
        [PXDBBool()]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Is Reserve Acct")]
        public virtual bool? UsrIsReserveAcct { get; set; }
        public abstract class usrIsReserveAcct : PX.Data.BQL.BqlBool.Field<usrIsReserveAcct> { }
        #endregion
    }
}
