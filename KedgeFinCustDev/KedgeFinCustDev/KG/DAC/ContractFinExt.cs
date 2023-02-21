using PX.Data.EP;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.CM;
using PX.Objects.Common.Discount;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.CT;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.PM;
using PX.Objects;
using PX.TM;
using System.Collections.Generic;
using System.Collections;
using System;

namespace PX.Objects.CT
{
    /**
     * ===2021/08/10 :0012190 === Althea
     * Add UsrIsSettled Bool
     **/
    public class ContractFinExt : PXCacheExtension<PX.Objects.CT.Contract>
    {
        #region UsrFinStartDate
        [PXDBDate]
        [PXUIField(DisplayName = "FinStartDate")]

        public virtual DateTime? UsrFinStartDate { get; set; }
        public abstract class usrFinStartDate : PX.Data.BQL.BqlDateTime.Field<usrFinStartDate> { }
        #endregion

        #region UsrFinSettleDate
        [PXDBDate]
        [PXUIField(DisplayName = "UsrFinSettleDate")]

        public virtual DateTime? UsrFinSettleDate { get; set; }
        public abstract class usrFinSettleDate : PX.Data.BQL.BqlDateTime.Field<usrFinSettleDate> { }
        #endregion

        //以下為Unbound欄位

        #region UsrOriRevenue
        [PXDBDecimal()]
        [PXQuantity()]
        [PXUIField(DisplayName = "UsrOriRevenue")]
        public virtual Decimal? UsrOriRevenue { get; set; }
        public abstract class usrOriRevenue : PX.Data.BQL.BqlDecimal.Field<usrOriRevenue> { }
        #endregion

        #region UsrModRevenueTotal
        [PXDecimal()]
        [PXQuantity()]
        [PXUIField(DisplayName = "UsrModRevenueTotal",IsReadOnly =true)]
        public virtual Decimal? UsrModRevenueTotal { get; set; }
        public abstract class usrModRevenueTotal : PX.Data.BQL.BqlDecimal.Field<usrModRevenueTotal> { }
        #endregion

        #region UsrFinalRevenue
        [PXDBQuantity()]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "UsrFinalRevenue",IsReadOnly =true)]
        public virtual Decimal? UsrFinalRevenue { get; set; }
        public abstract class usrFinalRevenue : PX.Data.BQL.BqlDecimal.Field<usrFinalRevenue> { }
        #endregion

        #region UsrOriCost
        [PXDBDecimal()]
        [PXQuantity()]
        [PXUIField(DisplayName = "UsrOriCost")]
        public virtual Decimal? UsrOriCost { get; set; }
        public abstract class usrOriCost : PX.Data.BQL.BqlDecimal.Field<usrOriCost> { }
        #endregion

        #region UsrModCostTotal
        [PXDecimal()]
        [PXQuantity]
        [PXUIField(DisplayName = "UsrModCostTotal",IsReadOnly =true)]
        public virtual Decimal? UsrModCostTotal { get; set; }
        public abstract class usrModCostTotal : PX.Data.BQL.BqlDecimal.Field<usrModCostTotal> { }
        #endregion

        #region UsrFinalCost
        [PXDBQuantity]
        [PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "UsrFinalCost",IsReadOnly =true)]
        public virtual Decimal? UsrFinalCost { get; set; }
        public abstract class usrFinalCost : PX.Data.BQL.BqlDecimal.Field<usrFinalCost> { }
        #endregion

        //2021/08/10 Add Mantis: 0012190
        #region UsrIsSettled
        [PXDBBool]
        [PXUIField(DisplayName = "UsrIsSettled")]

        public virtual bool? UsrIsSettled { get; set; }
        public abstract class usrIsSettled : PX.Data.BQL.BqlBool.Field<usrIsSettled> { }
        #endregion
    }
}